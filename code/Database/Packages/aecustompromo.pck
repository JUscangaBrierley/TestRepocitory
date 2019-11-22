CREATE OR REPLACE PACKAGE Aecustompromo IS

	TYPE Rcursor IS REF CURSOR;

	PROCEDURE Processcustompromos(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

	PROCEDURE Promoselector(p_Promotype  NUMBER, p_Max_Rowkey NUMBER);

	PROCEDURE Processbrapromo(p_Max_Rowkey NUMBER);

	PROCEDURE Processjeanspromo(p_Max_Rowkey NUMBER);

	PROCEDURE Processpromoexclusions;

  PROCEDURE ProcessPromoRedemptions;

  PROCEDURE ProcessPromoRollingBalance;

	PROCEDURE Fulfillbrapromo(p_Fulfillmentdate VARCHAR2, Retval IN OUT Rcursor);

	PROCEDURE BOY_RESET_BRA_COUNTERS(p_OverrideLastRunDate VARCHAR2, p_Dummy VARCHAR2, Retval IN OUT Rcursor);

	CURSOR Get_Promotionstoprocess(p_Max_Rowkey NUMBER) IS
		SELECT DISTINCT Que.a_Promotype
		FROM Ats_Memberpromoqueue Que
		WHERE Que.a_Rowkey <= p_Max_Rowkey;

	CURSOR Get_Promo_Summary IS
		SELECT Wt.Ipcode, Wt.Txnheaderid, Wt.Txndate, Wt.Txnstoreid, Wt.Txnnumber, Wt.Txnregisternumber, Wt.Promotionitemquantity, Wt.Promotionitemspurchased, Wt.Promotionitemsreturned, Wt.Promotiontype
		FROM Ae_Custompromo_Summary_Wrktble Wt
		ORDER BY Wt.Promotionitemquantity DESC;

END Aecustompromo;
/

CREATE OR REPLACE PACKAGE BODY Aecustompromo IS

	PROCEDURE Processcustompromos(p_Dummy VARCHAR2,	Retval  IN OUT Rcursor) AS
		v_Max_Rowkey     NUMBER;
	BEGIN

		SELECT MAX(a_Rowkey)
           INTO v_Max_Rowkey
    FROM Ats_Memberpromoqueue;

    -- for each promotion type in the que, execute the selector
    FOR y IN Get_Promotionstoprocess(v_Max_Rowkey) LOOP
        aecustompromo.promoselector(p_promotype => y.a_promotype, p_max_rowkey => v_Max_Rowkey);
    END LOOP;

    --calcualte the rolling 12 month balances for both jeans and bras
    ProcessPromoRollingBalance();

	END Processcustompromos;

	PROCEDURE Promoselector(p_Promotype  NUMBER, p_Max_Rowkey NUMBER) IS
	BEGIN
     IF p_Promotype = 1	THEN
        --call the bra promotion proc
			  Processbrapromo(p_Max_Rowkey);
      ELSIF p_Promotype = 2 THEN
        Processjeanspromo(p_Max_Rowkey);
      ELSE Raise_Application_Error(-20001, 'Promotion not recognized: ' || p_Promotype);
    END IF;


	END Promoselector;

	/********************************************************************
  ********************************************************************/
	PROCEDURE Processbrapromo(p_Max_Rowkey NUMBER) IS
		v_Sysdate              DATE;
		v_Fulfillmentthreshold INTEGER;
		v_Tmpfulfillmentdate   DATE;
    v_SearchBeginDate      DATE;
    v_SearchEndDate        DATE;
		v_Currentbalance       INTEGER;
		v_Remainder            INTEGER;
		v_Certstoissue         INTEGER;
		v_Currentdate          DATE;
		v_Brapromocurrentdate  CHAR(10);
		v_Certrowkey           NUMBER;
		--log job attributes
		v_My_Log_Id            NUMBER;
		v_Jobdirection         NUMBER := 0;
		v_Starttime            DATE   := SYSDATE;
		v_Endtime              DATE;
		v_Messagesreceived     NUMBER := 0;
		v_Messagesfailed       NUMBER := 0;
		v_Jobstatus            NUMBER := 0;
		v_Promotiontype        VARCHAR2(20)  := 'BraPromotion';
		v_Jobname              VARCHAR2(256) := 'ProcessCustomPromo : ' || v_Promotiontype;
		v_Filename             VARCHAR2(512) := '';
		--log msg attributes
		v_Messageid            NUMBER;
		v_Envkey               VARCHAR2(256) := 'BP_AE@' ||
																	 Upper(Sys_Context('userenv', 'instance_name'));
		v_Logsource            VARCHAR2(256) := 'Processbrapromo';
		v_Batchid              VARCHAR2(256) := 0;
		v_Message              VARCHAR2(256);
		v_Reason               VARCHAR2(256);
		v_Error                VARCHAR2(256);
		v_Trycount             NUMBER := 0;
		v_Table_Count          NUMBER;
	BEGIN
		SELECT SYSDATE INTO v_Sysdate FROM Dual;

		-- get job id for this process
		v_My_Log_Id  := Utility_Pkg.Get_Libjobid();

		/* log start of job */
		Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
												p_Jobdirection     => v_Jobdirection,
												p_Filename         => v_Filename,
												p_Starttime        => v_Starttime,
												p_Endtime          => v_Endtime,
												p_Messagesreceived => v_Messagesreceived,
												p_Messagesfailed   => v_Messagesfailed,
												p_Jobstatus        => v_Jobstatus,
												p_Jobname          => v_Jobname);

		SELECT To_Number(To_Char(c.Value))
		       INTO v_Fulfillmentthreshold
		FROM Lw_Clientconfiguration c
		WHERE c.Key = 'BraPromoFulFillmentThreshold';

		SELECT To_Char(c.Value)
		       INTO v_Brapromocurrentdate
		FROM Lw_Clientconfiguration c
		WHERE c.Key = 'BraPromoCurrentDate';

		EXECUTE IMMEDIATE 'TRUNCATE TABLE AE_CUSTOMPROMO_SKUS';

		-- INSERT QUALIFYING SKUS INTO AE_CUSTOMPROMO_SKUS
		-- since the list is comma delimted, we need to parse it out and then find the skus
		-- use a regular expression and also subquery factoring
		-- source: http://nuijten.blogspot.com/2009/07/splitting-comma-delimited-string-regexp.html
		INSERT INTO Ae_Custompromo_Skus (Classcode, Productsku, Productid)
			SELECT p.Classcode, p.Partnumber, p.id
			FROM Lw_Product p
			WHERE p.Classcode IN
						(WITH Promoclasses AS (SELECT To_Char(t.Value) AS Str
							                     FROM Lw_Clientconfiguration t
							                     WHERE t.Key = 'BraPromoClassCodes')
							SELECT Regexp_Substr(Str, '[^,]+', 1, Rownum) Split
							FROM Promoclasses
							CONNECT BY LEVEL <= Length(Regexp_Replace(Str, '[^,]+')) + 1
						 );

		SELECT COUNT(*)
           INTO v_Table_Count
    FROM Ae_Custompromo_Skus;

		IF v_Table_Count = 0
		THEN
			v_Messagesfailed := v_Messagesfailed + 1;

      v_Endtime := SYSDATE;

			Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
													p_Jobdirection     => v_Jobdirection,
													p_Filename         => v_Filename,
													p_Starttime        => v_Starttime,
													p_Endtime          => v_Endtime,
													p_Messagesreceived => v_Messagesreceived,
													p_Messagesfailed   => v_Messagesfailed,
													p_Jobstatus        => v_Jobstatus,
													p_Jobname          => v_Jobname);

			v_Messagesfailed := v_Messagesfailed + 1;
			v_Error          := SQLERRM;
			v_Reason         := '0 RECORDS FOUND IN Lw_Clientconfiguration for KEY: BraPromoClassCodes';
			v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
													'    <pkg>Aecustompromo</pkg>' || Chr(10) ||
													'    <proc>Processbrapromo</proc>' || Chr(10) ||
													'    <filename>' || v_Filename || '</filename>' ||
													Chr(10) || '  </details>' || Chr(10) ||
													'</failed>';

			/* log error */
			Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
													p_Envkey    => v_Envkey,
													p_Logsource => v_Logsource,
													p_Filename  => v_Filename,
													p_Batchid   => v_Batchid,
													p_Jobnumber => v_My_Log_Id,
													p_Message   => v_Message,
													p_Reason    => v_Reason,
													p_Error     => v_Error,
													p_Trycount  => v_Trycount,
													p_Msgtime   => SYSDATE);

			Raise_Application_Error(-20001, '0 RECORDS FOUND IN Lw_Clientconfiguration for KEY: BraPromoClassCodes ');
		END IF;

		-- first pull a list of reason codes
		-- if the table is empty, the stop processing bras all together and make the process fail.

		-- INSERT BRA REASONCODES INTO AE_CUSTOMPROMO_REASONCODES
		INSERT INTO Ae_Custompromo_Reasoncodes
		SELECT DISTINCT a_Reasoncode
    FROM Ats_Brareasoncodes;

		SELECT COUNT(*)
           INTO v_Table_Count
    FROM Ae_Custompromo_Reasoncodes;

		IF v_Table_Count = 0
		THEN
			v_Messagesfailed := v_Messagesfailed + 1;

      v_Endtime := SYSDATE;

			Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
													p_Jobdirection     => v_Jobdirection,
													p_Filename         => v_Filename,
													p_Starttime        => v_Starttime,
													p_Endtime          => v_Endtime,
													p_Messagesreceived => v_Messagesreceived,
													p_Messagesfailed   => v_Messagesfailed,
													p_Jobstatus        => v_Jobstatus,
													p_Jobname          => v_Jobname);

			/* log error */

			v_Messagesfailed := v_Messagesfailed + 1;
			v_Error          := SQLERRM;
			v_Reason         := '0 RECORDS FOUND IN Ats_Brareasoncodes';
			v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
													'    <pkg>Aecustompromo</pkg>' || Chr(10) ||
													'    <proc>Processbrapromo</proc>' || Chr(10) ||
													'    <filename>' || v_Filename || '</filename>' ||
													Chr(10) || '  </details>' || Chr(10) ||
													'</failed>';

			Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
													p_Envkey    => v_Envkey,
													p_Logsource => v_Logsource,
													p_Filename  => v_Filename,
													p_Batchid   => v_Batchid,
													p_Jobnumber => v_My_Log_Id,
													p_Message   => v_Message,
													p_Reason    => v_Reason,
													p_Error     => v_Error,
													p_Trycount  => v_Trycount,
													p_Msgtime   => SYSDATE);

			Raise_Application_Error(-20001, '0 RECORDS FOUND IN Ats_Brareasoncodes ');
		END IF;

		EXECUTE IMMEDIATE 'TRUNCATE TABLE Ae_Custompromo_Details_Wrktble';

    -- check if the current date is 1/1, then filter txns for 12/15 - 12/31 of previous year
    IF((EXTRACT(MONTH FROM SYSDATE)) = 1 AND (EXTRACT(DAY FROM SYSDATE)) = 1) THEN
        v_SearchBeginDate := to_date('12/15/' || EXTRACT(YEAR FROM (TRUNC(ADD_MONTHS(SYSDATE, -12),'MM'))), 'MM/DD/YYYY');
        v_SearchEndDate   := to_date('12/31/' || EXTRACT(YEAR FROM (TRUNC(ADD_MONTHS(SYSDATE, -12),'MM'))), 'MM/DD/YYYY');
      ELSE
        -- check for txns for the current year
        v_SearchBeginDate := to_date('1/1/' || EXTRACT(YEAR FROM (SYSDATE)), 'MM/DD/YYYY');
        v_SearchEndDate   := to_date('12/31/' || EXTRACT(YEAR FROM (SYSDATE)), 'MM/DD/YYYY');
    END IF;

    -- this will put all of the details from the txn detail based on the txn header match on the header key of the que
    -- the Ats_Txnlineitemdiscount reference was removed from the initial and will be later referenced to help
    -- elminate any possible bugs causing bra counts to be inflated for some members
    INSERT INTO Ae_Custompromo_Details_Wrktble (Ipcode, Txnheaderid, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Dtldetailid, Dtlquantity, Dtlclasscode, Skunumber, Dtlreasoncode, Dtltypeid, DtlSaleAmount, Employeeid, Txnrowkey)
    SELECT /*+ full (que) */
           que.a_ipcode, th.a_txnheaderid, th.a_txndate, th.a_txnstoreid, th.a_txnnumber, th.a_txnregisternumber, di.a_txndetailid, di.a_dtlquantity, di.a_dtlclasscode, skus.productsku, '', di.a_dtltypeid, di.a_dtlsaleamount, th.a_txnemployeeid, th.a_rowkey
    FROM ats_memberpromoqueue que
    , Ats_Txnheader Th
    , Ats_Txndetailitem Di
    , Ae_Custompromo_Skus skus
    WHERE Que.a_Rowkey <= p_Max_Rowkey
      AND Que.a_Storenumber = Th.a_Storenumber
      AND Th.a_Txnheaderid = Di.a_Txnheaderid
      AND di.a_dtlproductid = skus.productid
      AND Que.a_Txndate = Th.a_Txndate
      AND Que.a_Txnnumber = Th.a_Txnnumber
      AND Que.a_Txnregisternumber = Th.a_Txnregisternumber
      AND Que.a_Txndate BETWEEN v_SearchBeginDate AND v_SearchEndDate;

      COMMIT;


    --RKG - 4/27/2012
    --Moved the redemptions here because we had to put a bulk collect in here to handle duplicates.  Since there
    --are commits in here we moved this up to commit the work table changes so it wouldn't commit the perm table
    --changes from below.
    Processpromoredemptions();

    commit;

		EXECUTE IMMEDIATE 'TRUNCATE TABLE AE_CUSTOMPROMO_SUMMARY_WRKTBLE';

    -- insert all of the qualifying bra txns into the summary table
		INSERT INTO Ae_Custompromo_Summary_Wrktble (Ipcode, Txnheaderid, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Promotionitemquantity, Promotionlifequantity, Promotionitemspurchased, Promotionitemsreturned, Promotiontype, Txnrowkey, Iscurrentemployee)
		SELECT DISTINCT Dw.Ipcode, Dw.Txnheaderid, Dw.Txndate, Dw.Txnstoreid, Dw.Txnnumber, Dw.Txnregisternumber, 0, 0, 0, 0, v_Promotiontype, dw.txnrowkey, 'N'
		FROM Ae_Custompromo_Details_Wrktble Dw;

      COMMIT;
    -- update the current employee flag on the work table
    -- so it can eventually get propagated to the ats_memberpromodetails
		UPDATE Ae_Custompromo_Summary_Wrktble Swt
    SET SWT.Iscurrentemployee = 'Y'
		WHERE EXISTS (SELECT 1
						      FROM Ats_Memberdetails Md
						      WHERE Md.a_Ipcode = Swt.Ipcode
									AND Md.a_Employeecode = 1)
    OR(length(swt.employeeid) > 0);
      COMMIT;


    UPDATE Ae_Custompromo_Summary_Wrktble swt
    SET swt.Promotionitemspurchased = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt.txnheaderid
                                       AND dwt.ispromoredemption = 'N'
                                       AND swt.iscurrentemployee = 'N'
                                       AND Dwt.Dtltypeid = 1)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Details_Wrktble dwt
                 WHERE dwt.txnheaderid = swt.txnheaderid
                 AND dwt.ispromoredemption = 'N'
                 AND swt.iscurrentemployee = 'N'
                 AND Dwt.Dtltypeid = 1);
      COMMIT;

    UPDATE Ae_Custompromo_Summary_Wrktble swt
    SET swt.Promotionitemsreturned = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt.txnheaderid
                                       AND dwt.ispromoredemption = 'N'
                                       AND swt.iscurrentemployee = 'N'
                                       AND Dwt.Dtltypeid > 1)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Details_Wrktble dwt
                 WHERE dwt.txnheaderid = swt.txnheaderid
                 AND dwt.ispromoredemption = 'N'
                 AND swt.iscurrentemployee = 'N'
                 AND Dwt.Dtltypeid > 1);
      COMMIT;

    --RKG 4/30/2012
    --Had to change this one to get the quantity by getting the purchases and returns separately from the details tabla
    -- as in the previous two queries but without the checks for the redemption and employee so the quantity could work
    -- in the subsequent queries for lifetime.
        UPDATE Ae_Custompromo_Summary_Wrktble swt1
    SET swt1.Promotionlifequantity = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt1.txnheaderid
                                       AND Dwt.Dtltypeid = 1)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.txnheaderid = swt2.txnheaderid);
    COMMIT;

    UPDATE Ae_Custompromo_Summary_Wrktble swt1
    SET swt1.Promotionlifequantity = swt1.Promotionlifequantity -
                                       nvl((SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt1.txnheaderid
                                       AND Dwt.Dtltypeid > 1), 0)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.txnheaderid = swt2.txnheaderid);

    COMMIT;

    --RKG 4/30/2012
    --Had to Break out the item quantity field to keep the quantity separate for the lifetime counts
        UPDATE Ae_Custompromo_Summary_Wrktble swt1
    SET swt1.promotionitemquantity = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt1.txnheaderid
                                       AND dwt.ispromoredemption = 'N'
                                       AND swt1.iscurrentemployee = 'N'
                                       AND Dwt.Dtltypeid = 1)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.txnheaderid = swt2.txnheaderid);
    COMMIT;

    UPDATE Ae_Custompromo_Summary_Wrktble swt1
    SET swt1.promotionitemquantity = swt1.Promotionitemquantity -
                                       nvl((SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt1.txnheaderid
                                       AND dwt.ispromoredemption = 'N'
                                       AND swt1.iscurrentemployee = 'N'
                                       AND Dwt.Dtltypeid > 1), 0)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.txnheaderid = swt2.txnheaderid);

    COMMIT;

    -- add\update the ats_memberpromodetails with the counts before the removal of
    -- redemptions and employee data
    -- insert any rows that dont exist in the summary first so we can update them later

    -- cannot use a merge statement here because if a record doesnt exist
    -- and is inserted, the merge statement will not update it
    -- because it uses Read Consistency

    --RKG commented out because this is a duplicate from the one below
    /*
    UPDATE Ats_Memberbrapromosummary swt1
    SET swt1.a_bralifetimebalance = swt1.a_bralifetimebalance + (SELECT SUM(NVL(swt2.Promotionitemquantity, 0))
                                                                  FROM Ae_Custompromo_Summary_Wrktble swt2
                                                                  WHERE swt1.a_ipcode = swt2.ipcode
                                                                  GROUP BY swt2.Promotionitemquantity
                                                                 )
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.a_ipcode = swt2.ipcode);

    INSERT INTO Ats_Memberbrapromosummary	(a_Rowkey, a_Ipcode, a_ParentRowKey, a_Currentbalance, a_Fulfillmentbalance, a_Totalpurchased, a_Totalreturned, a_Brafirstpurchasedate, a_Bralifetimebalance, a_Brarollingbalance, a_Jeansfirstpurchasedate, a_Jeanslifetimebalance, a_Jeansrollingbalance, Statuscode, Createdate)
    SELECT Seq_Rowkey.Nextval, swt.Ipcode, swt.Ipcode, 0, 0, 0, 0, NULL, 0, 0, NULL, 0, 0, 1, SYSDATE
    FROM ae_custompromo_summary_wrktble swt
    WHERE NOT EXISTS(SELECT 1
                    FROM Ats_Memberbrapromosummary s
                    WHERE swt.ipcode = s.a_ipcode);
    */
    -- Update the bra lifetime count
    UPDATE Ats_Memberbrapromosummary ps
    SET ps.a_bralifetimebalance =
        case
          when NVL(ps.a_bralifetimebalance, 0) + (SELECT sum(swt.promotionlifequantity) FROM ae_custompromo_summary_wrktble swt where swt.ipcode = ps.a_ipcode) < 0 then 0
          else NVL(ps.a_bralifetimebalance, 0) + (SELECT sum(swt.promotionlifequantity) FROM ae_custompromo_summary_wrktble swt where swt.ipcode = ps.a_ipcode)
        end,
        ps.updatedate = SYSDATE
    WHERE EXISTS (Select 1 From ae_custompromo_summary_wrktble s Where ps.a_ipcode = s.ipcode);

    INSERT INTO Ats_Memberbrapromosummary ps (a_currentbalance, a_fulfillmentbalance, a_ipcode, a_brafirstpurchasedate, a_bralifetimebalance, a_brarollingbalance, a_parentrowkey, a_rowkey, a_totalpurchased, a_totalreturned, createdate, statuscode, updatedate)
    SELECT 0, 0, wk.ipcode, wk.txndate, wk.quantity, wk.quantity, wk.ipcode, seq_rowkey.nextval, 0, 0, SYSDATE, 1, SYSDATE
    FROM (select Ipcode, min(swk.txndate) as txndate, sum(swk.promotionlifequantity) as quantity from ae_custompromo_summary_wrktble swk group by ipcode) wk
    WHERE 1=1
    AND NOT EXISTS (Select 1 From Ats_Memberbrapromosummary pd Where pd.a_ipcode = wk.ipcode);

    --RKG 02/09/2012
    --Removed this code to populate memberpromodetails because we are using a different table for rolling 12 months
    /*
    -- Converted the Merge into a single Insert and Update statement
    Insert Into ats_memberpromodetails pd (a_txnbranet, a_ipcode, a_iscurrentemployee, a_txnjeannet, a_parentrowkey, a_rowkey, createdate, statuscode, updatedate, a_source)
    SELECT s.promotionitemquantity, s.ipcode, s.iscurrentemployee, 0, s.txnrowkey, seq_rowkey.nextval, SYSDATE, 1, SYSDATE, v_Logsource
           FROM ae_custompromo_summary_wrktble s
    WHERE NOT EXISTS (Select 1 From ats_memberpromodetails pd Where pd.a_ipcode = s.ipcode);

    Update ats_memberpromodetails pd
    SET pd.a_txnbranet = (SELECT swt.promotionitemquantity FROM ae_custompromo_summary_wrktble swt Where  pd.a_parentrowkey = swt.txnrowkey),
        pd.updatedate = SYSDATE,
        pd.a_source = pd.a_source || ', ' || v_Logsource
    WHERE EXISTS (Select 1 From ae_custompromo_summary_wrktble s Where pd.a_ipcode = s.ipcode);
    */
    /*
    MERGE INTO ats_memberpromodetails pd
    USING (SELECT swt.txnrowkey, swt.promotionitemquantity, swt.ipcode, swt.iscurrentemployee
           FROM ae_custompromo_summary_wrktble swt) s
    ON (pd.a_parentrowkey = s.txnrowkey)
    WHEN MATCHED THEN UPDATE
                      SET pd.a_txnbranet = s.promotionitemquantity,
                      pd.updatedate = SYSDATE,
                      pd.a_source = pd.a_source || ', ' || v_Logsource
    WHEN NOT MATCHED THEN INSERT (pd.a_txnbranet, pd.a_ipcode, pd.a_iscurrentemployee, pd.a_txnjeannet, pd.a_parentrowkey, pd.a_rowkey, pd.createdate, pd.statuscode, pd.updatedate, pd.a_source)
    VALUES(s.promotionitemquantity, s.ipcode, s.iscurrentemployee, 0, s.txnrowkey, seq_rowkey.nextval, SYSDATE, 1, SYSDATE, v_Logsource);
    */




--TODO: ADD UPDATE TO PROMOSUMMARY THAT UPDATES THE ROLLING 12
    /*
  	   * Process redemptions based on the current run.
		   * The redemptions that will be redeemed are pulled from the Ae_Custompromo_Details_Wrktble
		        and then the redemptions will be deleted from Ae_Custompromo_Details_Wrktble so when
		        the bra counts are calculated, we dont need to filter those out
       * Now that we have the lifetime\rolling counts calculated, we can now filter out
            employees and redemptions from the bra promotion.
       * Its duplicating the counts but it is needed to include employees\redemptions
            for lifetime\rolling and then exclude those from the actual bra promotion
    */

    --RKG - 4/27/2012
    --Moved the redemptions up where the work tables are created because we had to put a bulk collect in here to handle duplicates.  Since there
    --are commits in here we moved this up to commit the work table changes so it wouldn't commit the perm table
    --changes from below.
		/*Processpromoredemptions();*/

		-- can not filter out reason codes nor skus due to promotiontype specific
		-- remove employees and already processed records
		Processpromoexclusions();

    -- Since the lifetime counts were calculated, now we need to recalculate the counts
    --   for the bra promotion so we can exclude redemptions and employees for the actual
    --   bra promotion
 /*
    ** RKG 4/30/2012
    ** These 3 updates were also executed prior to the updates for lifetime with the exception of the check for the
    ** ispromoredemption and iscurrentemployee because the thought was we wanted to update the counts prior to updating
    ** lifetime so we could include redemptions and employees in the lifetime and rolling 12.
    ** This is causing redemptions to be counted in the items purchased in error.
    ** so we are commenting this code out here and adding the checks for the redemptions and employee above and then making
    ** sure they are counted in the lifetime and rolling 12.
    UPDATE Ae_Custompromo_Summary_Wrktble swt
    SET swt.Promotionitemspurchased = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt.txnheaderid
                                       AND Dwt.Dtltypeid = 1
                                       AND dwt.ispromoredemption = 'N'
                                       AND swt.iscurrentemployee = 'N'
                                       )
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Details_Wrktble dwt
                 WHERE dwt.txnheaderid = swt.txnheaderid
                 AND Dwt.Dtltypeid = 1
                 AND dwt.ispromoredemption = 'N'
                 AND swt.iscurrentemployee = 'N');

    UPDATE Ae_Custompromo_Summary_Wrktble swt
    SET swt.Promotionitemsreturned = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt.txnheaderid
                                       AND Dwt.Dtltypeid > 1
                                       AND dwt.ispromoredemption = 'N'
                                       AND swt.iscurrentemployee = 'N')
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Details_Wrktble dwt
                 WHERE dwt.txnheaderid = swt.txnheaderid
                 AND dwt.ispromoredemption = 'N'
                 AND swt.iscurrentemployee = 'N');

    UPDATE Ae_Custompromo_Summary_Wrktble swt1
    SET swt1.Promotionitemquantity = (SELECT (NVL(swt2.promotionitemspurchased, 0) -
                                              NVL(swt2.promotionitemsreturned, 0))
                                      FROM Ae_Custompromo_Summary_Wrktble swt2
                                      WHERE swt1.txnheaderid = swt2.txnheaderid
                                      AND swt2.iscurrentemployee = 'N')
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.txnheaderid = swt2.txnheaderid
                 AND swt2.iscurrentemployee = 'N');
*/

		IF v_Brapromocurrentdate = '99/99/9999'
		THEN
			-- IF TRUE, THEN PROCESS USING THE CURRENT DATE
			v_Currentdate := Trunc(SYSDATE);
		ELSE
			v_Currentdate := To_Date(v_Brapromocurrentdate, 'MM/DD/YYYY');
		END IF;

		FOR Rec IN Get_Promo_Summary LOOP

			SELECT MAX(Br.a_Mailfilepulldate)
      			INTO v_Tmpfulfillmentdate
			FROM Ats_Brareasoncodes Br
			WHERE Rec.Txndate >= Trunc(Br.a_Effectivestartdate)
			AND Rec.Txndate <= Trunc(Br.a_Effectiveenddate);

			IF v_Tmpfulfillmentdate <= v_Currentdate OR v_Tmpfulfillmentdate IS NULL
			THEN
				-- get the next fulfillmentdate based on v_Currentdate
				SELECT MAX(Br.a_Mailfilepulldate)
					INTO v_Tmpfulfillmentdate
				FROM Ats_Brareasoncodes Br
				WHERE Br.a_Mailfilepulldate > v_Currentdate
				AND ROWNUM = 1
				ORDER BY Br.a_Mailfilepulldate;
			END IF;

			BEGIN
        --ONCE THE DUPLCATE RECORDS IN Ats_Memberbrapromosummary HAS BEEN CORRECTED,
        -- PUT A UNIQUE INDEX ON IPCODE SO WE WONT HAVE THIS ISSUE ANYMORE
        -- THEN REMOVE THE 'AND ROWNUM = 1'
        -- after those changes, this query would run much faster
				SELECT Ps.a_Currentbalance
        				INTO v_Currentbalance
				FROM Ats_Memberbrapromosummary Ps
				WHERE Rec.Ipcode = Ps.a_Ipcode
				AND ROWNUM = 1
				ORDER BY ps.createdate;

			EXCEPTION
				WHEN No_Data_Found THEN

					INSERT INTO Ats_Memberbrapromosummary	(a_Rowkey, a_Ipcode, a_ParentRowKey, a_Currentbalance, a_Fulfillmentbalance, a_Totalpurchased, a_Totalreturned, a_Brafirstpurchasedate, a_Bralifetimebalance, a_Brarollingbalance, a_Jeansfirstpurchasedate, a_Jeanslifetimebalance, a_Jeansrollingbalance, Statuscode, Createdate)
					VALUES (Seq_Rowkey.Nextval, Rec.Ipcode, Rec.Ipcode, 0, 0, 0, 0, sysdate, 0, 0, null, 0, 0, 1, SYSDATE);

					v_Currentbalance := 0;
			END;

			v_Currentbalance := v_Currentbalance + Rec.Promotionitemquantity;

			IF v_Currentbalance = 0
			THEN
				UPDATE Ats_Memberbrapromosummary Ps
				SET Ps.a_Currentbalance = v_Currentbalance,
						Ps.a_Totalpurchased = Rec.Promotionitemspurchased,
						Ps.a_Totalreturned  = Rec.Promotionitemsreturned
				WHERE Ps.a_Ipcode = Rec.Ipcode;
			END IF;

			IF Rec.Promotionitemquantity > 0
			THEN
				-- The below rounding it up??
				v_Certstoissue := Floor(v_Currentbalance / v_Fulfillmentthreshold);

				IF v_Certstoissue > 0
				THEN
					SELECT MOD(v_Currentbalance, v_Fulfillmentthreshold)
          	INTO v_Remainder
					FROM Dual;

					WHILE v_Certstoissue > 0 LOOP
						INSERT INTO Ats_Memberbrapromocerthistory	(a_Rowkey, a_Ipcode, a_Threshold, a_Txnheaderid, a_Returntxnheaderid, a_Fulfillmentdate, a_Changedby, a_Isfulfilled, a_Isdeleted, Statuscode, Createdate)
						VALUES (Seq_Rowkey.Nextval, Rec.Ipcode, v_Fulfillmentthreshold, Rec.Txnheaderid, 0, v_Tmpfulfillmentdate, v_Jobname, 0, 0, 1, SYSDATE);

						v_Certstoissue := v_Certstoissue - 1;
					END LOOP;

					UPDATE Ats_Memberbrapromosummary Ps
					SET Ps.a_Currentbalance = v_Remainder,
							Ps.a_Totalpurchased = Rec.Promotionitemspurchased,
							Ps.a_Totalreturned  = Rec.Promotionitemsreturned
					WHERE Ps.a_Ipcode = Rec.Ipcode;
				ELSE
					UPDATE Ats_Memberbrapromosummary Ps
					SET Ps.a_Currentbalance = v_Currentbalance,
							Ps.a_Totalpurchased = Rec.Promotionitemspurchased,
							Ps.a_Totalreturned  = Rec.Promotionitemsreturned
					WHERE Ps.a_Ipcode = Rec.Ipcode;
				END IF;
			END IF;

			IF Rec.Promotionitemquantity < 0
			THEN
				-- process the returns
				IF v_Currentbalance > 0
				THEN
					UPDATE Ats_Memberbrapromosummary Ps
					SET Ps.a_Currentbalance = v_Currentbalance,
							Ps.a_Totalpurchased = Rec.Promotionitemspurchased,
							Ps.a_Totalreturned  = Rec.Promotionitemsreturned
					WHERE Ps.a_Ipcode = Rec.Ipcode;
				ELSE
					BEGIN
						WHILE v_Currentbalance < 0 LOOP
							SELECT *
							       INTO v_Certrowkey
							FROM (SELECT Ps.a_Rowkey
										FROM Ats_Memberbrapromocerthistory Ps
										WHERE Ps.a_Ipcode = Rec.Ipcode
										AND Ps.a_Isfulfilled = 0
										AND Ps.a_Isdeleted = 0
										ORDER BY Ps.a_Fulfillmentdate)
							WHERE Rownum = 1;

							UPDATE Ats_Memberbrapromocerthistory Pch
							SET Pch.a_Isdeleted = 1
							WHERE Pch.a_Rowkey = v_Certrowkey;

							v_Currentbalance := v_Currentbalance + v_Fulfillmentthreshold;
						END LOOP;
					EXCEPTION
						WHEN No_Data_Found THEN
							v_Currentbalance := 0;
					END;
				END IF;

				IF v_Currentbalance < 0
				THEN
					UPDATE Ats_Memberbrapromosummary Ps
					SET Ps.a_Currentbalance = 0,
							Ps.a_Totalpurchased = Rec.Promotionitemspurchased,
							Ps.a_Totalreturned  = Rec.Promotionitemsreturned
					WHERE Ps.a_Ipcode = Rec.Ipcode;

				ELSE
					UPDATE Ats_Memberbrapromosummary Ps
					SET Ps.a_Currentbalance = v_Currentbalance,
							Ps.a_Totalpurchased = Rec.Promotionitemspurchased,
							Ps.a_Totalreturned  = Rec.Promotionitemsreturned
					WHERE Ps.a_Ipcode = Rec.Ipcode;
				END IF;
			END IF;

			INSERT INTO Ae_Custompromo_Processed (Ipcode, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Promotiontype, Createdate,Txnpromonet)
      VALUES (Rec.Ipcode, Rec.Txndate, Rec.Txnstoreid, Rec.Txnnumber, Rec.Txnregisternumber, Rec.Promotiontype, SYSDATE, Rec.Promotionitemquantity);

			v_Certstoissue   := 0;
			v_Currentbalance := 0;
			v_Remainder      := 0;

			v_Messagesreceived := v_Messagesreceived + 1;

			COMMIT;
		END LOOP;

		DELETE FROM Ats_Memberpromoqueue Que
		WHERE Que.a_Rowkey <= p_Max_Rowkey
		AND Que.a_Promotype = 1;
		-- make the above dynamic in the future

    v_Endtime := SYSDATE;

		-- log end of job
		Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
												p_Jobdirection     => v_Jobdirection,
												p_Filename         => v_Filename,
												p_Starttime        => v_Starttime,
												p_Endtime          => v_Endtime,
												p_Messagesreceived => v_Messagesreceived,
												p_Messagesfailed   => v_Messagesfailed,
												p_Jobstatus        => v_Jobstatus,
												p_Jobname          => v_Jobname);

		COMMIT;
	END Processbrapromo;

	PROCEDURE Processjeanspromo(p_Max_Rowkey NUMBER) IS
		v_Sysdate              DATE;
    v_SearchBeginDate      DATE;
    v_SearchEndDate        DATE;
		--log job attributes
		v_My_Log_Id            NUMBER;
		v_Jobdirection         NUMBER := 0;
		v_Starttime            DATE   := SYSDATE;
		v_Endtime              DATE;
		v_Messagesreceived     NUMBER := 0;
		v_Messagesfailed       NUMBER := 0;
		v_Jobstatus            NUMBER := 0;
		v_Promotiontype        VARCHAR2(20)  := 'JeansPromotion';
		v_Jobname              VARCHAR2(256) := 'ProcessCustomPromo : ' || v_Promotiontype;
		v_Filename             VARCHAR2(512) := '';
		--log msg attributes
		v_Messageid            NUMBER;
		v_Envkey               VARCHAR2(256) := 'BP_AE@' ||
																	 Upper(Sys_Context('userenv', 'instance_name'));
		v_Logsource            VARCHAR2(256) := 'Processjeanspromo';
		v_Batchid              VARCHAR2(256) := 0;
		v_Message              VARCHAR2(256);
		v_Reason               VARCHAR2(256);
		v_Error                VARCHAR2(256);
		v_Trycount             NUMBER := 0;
		v_Table_Count          NUMBER;
    v_JeanCount            NUMBER;
	BEGIN
		SELECT SYSDATE INTO v_Sysdate FROM Dual;

		-- get job id for this process
		v_My_Log_Id  := Utility_Pkg.Get_Libjobid();

		/* log start of job */
		Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
												p_Jobdirection     => v_Jobdirection,
												p_Filename         => v_Filename,
												p_Starttime        => v_Starttime,
												p_Endtime          => v_Endtime,
												p_Messagesreceived => v_Messagesreceived,
												p_Messagesfailed   => v_Messagesfailed,
												p_Jobstatus        => v_Jobstatus,
												p_Jobname          => v_Jobname);


		EXECUTE IMMEDIATE 'TRUNCATE TABLE AE_CUSTOMPROMO_SKUS';

		-- INSERT QUALIFYING SKUS INTO AE_CUSTOMPROMO_SKUS
		-- since the list is comma delimted, we need to parse it out and then find the skus
		-- use a regular expression and also subquery factoring
		-- source: http://nuijten.blogspot.com/2009/07/splitting-comma-delimited-string-regexp.html
		INSERT INTO Ae_Custompromo_Skus (Classcode, Productsku, Productid)
			SELECT p.Classcode, p.Partnumber, p.id
			FROM Lw_Product p
			WHERE p.Classcode IN
						(WITH Promoclasses AS (SELECT To_Char(t.Value) AS Str
							                     FROM Lw_Clientconfiguration t
							                     WHERE t.Key = 'JeansPromoClassCodes')
							SELECT Regexp_Substr(Str, '[^,]+', 1, Rownum) Split
							FROM Promoclasses
							CONNECT BY LEVEL <= Length(Regexp_Replace(Str, '[^,]+')) + 1
						 );

		SELECT COUNT(*)
           INTO v_Table_Count
    FROM Ae_Custompromo_Skus;

		IF v_Table_Count = 0
		THEN
			v_Messagesfailed := v_Messagesfailed + 1;

      v_Endtime := SYSDATE;

			Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
													p_Jobdirection     => v_Jobdirection,
													p_Filename         => v_Filename,
													p_Starttime        => v_Starttime,
													p_Endtime          => v_Endtime,
													p_Messagesreceived => v_Messagesreceived,
													p_Messagesfailed   => v_Messagesfailed,
													p_Jobstatus        => v_Jobstatus,
													p_Jobname          => v_Jobname);

			v_Messagesfailed := v_Messagesfailed + 1;
			v_Error          := SQLERRM;
			v_Reason         := '0 RECORDS FOUND IN Lw_Clientconfiguration for KEY: JeansPromoClassCodes';
			v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
													'    <pkg>Aecustompromo</pkg>' || Chr(10) ||
													'    <proc>Processjeanspromo</proc>' || Chr(10) ||
													'    <filename>' || v_Filename || '</filename>' ||
													Chr(10) || '  </details>' || Chr(10) ||
													'</failed>';

			/* log error */
			Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
													p_Envkey    => v_Envkey,
													p_Logsource => v_Logsource,
													p_Filename  => v_Filename,
													p_Batchid   => v_Batchid,
													p_Jobnumber => v_My_Log_Id,
													p_Message   => v_Message,
													p_Reason    => v_Reason,
													p_Error     => v_Error,
													p_Trycount  => v_Trycount,
													p_Msgtime   => SYSDATE);

			Raise_Application_Error(-20001,
															'0 RECORDS FOUND IN Lw_Clientconfiguration for KEY: JeansPromoClassCodes ');
		END IF;

		EXECUTE IMMEDIATE 'TRUNCATE TABLE Ae_Custompromo_Details_Wrktble';

    -- check if the current date is 1/1, then filter txns for 12/15 - 12/31 of previous year
    IF((EXTRACT(MONTH FROM SYSDATE)) = 1 AND (EXTRACT(DAY FROM SYSDATE)) = 1) THEN
        v_SearchBeginDate := to_date('12/15/' || EXTRACT(YEAR FROM (TRUNC(ADD_MONTHS(SYSDATE, -12),'MM'))), 'MM/DD/YYYY');
        v_SearchEndDate   := to_date('12/31/' || EXTRACT(YEAR FROM (TRUNC(ADD_MONTHS(SYSDATE, -12),'MM'))), 'MM/DD/YYYY');
      ELSE
        -- check for txns for the current year
        v_SearchBeginDate := to_date('1/1/' || EXTRACT(YEAR FROM (SYSDATE)), 'MM/DD/YYYY');
        v_SearchEndDate   := to_date('12/31/' || EXTRACT(YEAR FROM (SYSDATE)), 'MM/DD/YYYY');
      END IF;

    -- this will put all of the details from the txn detail based on the txn header match on the header key of the que
    -- the Ats_Txnlineitemdiscount reference was removed from the initial and will be later referenced to help
    -- elminate any possible bugs causing bra counts to be inflated for some members
    INSERT INTO Ae_Custompromo_Details_Wrktble (Ipcode, Txnheaderid, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Dtldetailid, Dtlquantity, Dtlclasscode, Skunumber, Dtlreasoncode, Dtltypeid, DtlSaleAmount, Employeeid, Txnrowkey)
    SELECT /*+ full (que) */
           que.a_ipcode, th.a_txnheaderid, th.a_txndate, th.a_txnstoreid, th.a_txnnumber, th.a_txnregisternumber, di.a_txndetailid, di.a_dtlquantity, di.a_dtlclasscode, skus.productsku, '', di.a_dtltypeid, di.a_dtlsaleamount, th.a_txnemployeeid, th.a_rowkey
    FROM ats_memberpromoqueue que
    , Ats_Txnheader Th
    , Ats_Txndetailitem Di
    , Ae_Custompromo_Skus skus
    WHERE Que.a_Rowkey <= p_Max_Rowkey
      AND Que.a_Storenumber = Th.a_Storenumber
      AND Th.a_Txnheaderid = Di.a_Txnheaderid
      AND di.a_dtlproductid = skus.productid
      AND Que.a_Txndate = Th.a_Txndate
      AND Que.a_Txnnumber = Th.a_Txnnumber
      AND Que.a_Txnregisternumber = Th.a_Txnregisternumber
      AND Que.a_Txndate BETWEEN v_SearchBeginDate AND v_SearchEndDate;

		EXECUTE IMMEDIATE 'TRUNCATE TABLE AE_CUSTOMPROMO_SUMMARY_WRKTBLE';

    -- insert all of the qualifying bra txns into the summary table
		INSERT INTO Ae_Custompromo_Summary_Wrktble (Ipcode, Txnheaderid, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Promotionitemquantity, Promotionitemspurchased, Promotionitemsreturned, Promotiontype, Txnrowkey, Iscurrentemployee)
		SELECT DISTINCT Dw.Ipcode, Dw.Txnheaderid, Dw.Txndate, Dw.Txnstoreid, Dw.Txnnumber, Dw.Txnregisternumber, 0, 0, 0, v_Promotiontype, dw.txnrowkey, 'N'
		FROM Ae_Custompromo_Details_Wrktble Dw;

    -- update the current employee flag on the work table
    -- so it can eventually get propagated to the ats_memberpromodetails
		UPDATE Ae_Custompromo_Summary_Wrktble Swt
    SET SWT.Iscurrentemployee = 'Y'
		WHERE EXISTS (SELECT 1
						      FROM Ats_Memberdetails Md
						      WHERE Md.a_Ipcode = Swt.Ipcode
									AND Md.a_Employeecode = 1)
    OR(length(swt.employeeid) > 0);

    -- keep track of the lifetime counts
    UPDATE Ae_Custompromo_Summary_Wrktble swt
    SET swt.Promotionitemspurchased = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt.txnheaderid
                                       AND Dwt.Dtltypeid = 1)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Details_Wrktble dwt
                 WHERE dwt.txnheaderid = swt.txnheaderid
                 AND Dwt.Dtltypeid = 1);

    UPDATE Ae_Custompromo_Summary_Wrktble swt
    SET swt.Promotionitemsreturned = (SELECT SUM(dwt.dtlquantity)
                                       FROM Ae_Custompromo_Details_Wrktble dwt
                                       WHERE dwt.txnheaderid = swt.txnheaderid
                                       AND Dwt.Dtltypeid > 1)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Details_Wrktble dwt
                 WHERE dwt.txnheaderid = swt.txnheaderid
                 AND Dwt.Dtltypeid > 1);

    UPDATE Ae_Custompromo_Summary_Wrktble swt1
    SET swt1.Promotionitemquantity = (SELECT (NVL(swt2.promotionitemspurchased, 0) -
                                              NVL(swt2.promotionitemsreturned, 0))
                                      FROM Ae_Custompromo_Summary_Wrktble swt2
                                      WHERE swt1.txnheaderid = swt2.txnheaderid)
    WHERE EXISTS(SELECT 1
                 FROM Ae_Custompromo_Summary_Wrktble swt2
                 WHERE swt1.txnheaderid = swt2.txnheaderid);

    -- Update the jeans lifetime count
    UPDATE Ats_Memberbrapromosummary ps
    SET ps.a_jeanslifetimebalance =
        case
          when NVL(ps.a_jeanslifetimebalance, 0) +  (SELECT sum(swt.promotionitemquantity) FROM ae_custompromo_summary_wrktble swt where swt.ipcode = ps.a_ipcode) < 0 then 0
          else NVL(ps.a_jeanslifetimebalance, 0) +  (SELECT sum(swt.promotionitemquantity) FROM ae_custompromo_summary_wrktble swt where swt.ipcode = ps.a_ipcode)
        end,
        ps.updatedate = SYSDATE
    WHERE EXISTS (Select 1 From ae_custompromo_summary_wrktble s Where ps.a_ipcode = s.ipcode);

    INSERT INTO Ats_Memberbrapromosummary ps (a_jeansfirstpurchasedate, a_jeanslifetimebalance, a_jeansrollingbalance, a_currentbalance, a_fulfillmentbalance, a_ipcode, a_brafirstpurchasedate, a_bralifetimebalance, a_brarollingbalance, a_parentrowkey, a_rowkey, a_totalpurchased, a_totalreturned, createdate, statuscode, updatedate)
    SELECT wk.txndate, wk.quantity, 0, 0, 0, wk.ipcode, NULL, 0, 0, wk.ipcode, seq_rowkey.nextval, 0, 0, SYSDATE, 1, SYSDATE
    FROM (select Ipcode, min(swk.txndate) as txndate, sum(swk.promotionitemquantity) as quantity from ae_custompromo_summary_wrktble swk group by ipcode) wk
    WHERE 1=1
    AND NOT EXISTS (Select 1 From Ats_Memberbrapromosummary pd Where pd.a_ipcode = wk.ipcode);

    --RKG - commented out because we don't need memberpromodetails because we are using a differen table to calculate rolling 12 balance
    /*
    -- add\update the ats_memberpromodetails with the counts before the removal of
    -- redemptions and employee data
    -- a merge statement is used just in case a member has jeans and bras in the same transaction.
    MERGE INTO ats_memberpromodetails pd
    USING (SELECT swt.txnrowkey, swt.promotionitemquantity, swt.ipcode, swt.iscurrentemployee
           FROM ae_custompromo_summary_wrktble swt) s
    ON (pd.a_parentrowkey = s.txnrowkey)
    WHEN MATCHED THEN UPDATE
                      SET pd.a_txnbranet = s.promotionitemquantity,
                      pd.updatedate = SYSDATE,
                      pd.a_source = pd.a_source || ', ' || v_Logsource
    WHEN NOT MATCHED THEN INSERT (pd.a_txnbranet, pd.a_ipcode, pd.a_iscurrentemployee, pd.a_txnjeannet, pd.a_parentrowkey, pd.a_rowkey, pd.createdate, pd.statuscode, pd.updatedate, pd.a_source)
    VALUES(s.promotionitemquantity, s.ipcode, s.iscurrentemployee, 0, s.txnrowkey, seq_rowkey.nextval, SYSDATE, 1, SYSDATE, v_Logsource);
    */

    INSERT INTO Ae_Custompromo_Processed (Ipcode, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Promotiontype, Createdate, Txnpromonet)
    SELECT swt.Ipcode, swt.Txndate, swt.Txnstoreid, swt.Txnnumber, swt.Txnregisternumber, 2, SYSDATE, swt.promotionitemquantity
    FROM Ae_Custompromo_Summary_Wrktble swt;

		DELETE FROM Ats_Memberpromoqueue Que
		WHERE Que.a_Rowkey <= p_Max_Rowkey
		AND Que.a_Promotype = 2;
		-- make the above dynamic in the future

    v_Endtime := SYSDATE;

		-- log end of job
		Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
												p_Jobdirection     => v_Jobdirection,
												p_Filename         => v_Filename,
												p_Starttime        => v_Starttime,
												p_Endtime          => v_Endtime,
												p_Messagesreceived => v_Messagesreceived,
												p_Messagesfailed   => v_Messagesfailed,
												p_Jobstatus        => v_Jobstatus,
												p_Jobname          => v_Jobname);

		COMMIT;

  END Processjeanspromo;

	PROCEDURE Processpromoexclusions IS
	BEGIN

    -- mark employee transactions as processed
		INSERT INTO Ae_Custompromo_Processed (Ipcode, Txndate, Txnstoreid, Txnnumber, Txnregisternumber, Promotiontype, Createdate)
		SELECT Swt.Ipcode, Swt.Txndate, Swt.Txnstoreid, Swt.Txnnumber, Swt.Txnregisternumber, Swt.Promotiontype, SYSDATE
		FROM Ae_Custompromo_Summary_Wrktble Swt
		WHERE (swt.employeeid IS NOT NULL
           OR Length(swt.Employeeid) > 0
           OR swt.iscurrentemployee = 'Y');

/* COMMENTED OUT SO Research can be done
		-- remove employees
		DELETE FROM Ae_Custompromo_Summary_Wrktble t
		WHERE (t.employeeid IS NOT NULL
           OR Length(t.Employeeid) > 0
           OR t.iscurrentemployee = 'Y');
*/
		-- remove records that have already been processed for a particular promotion
		DELETE FROM Ae_Custompromo_Summary_Wrktble Dwt
		WHERE EXISTS (SELECT 1
					        FROM Ae_Custompromo_Processed Proc
					        WHERE Proc.Txndate = Dwt.Txndate
								  AND Proc.Txnstoreid = Dwt.Txnstoreid
								  AND Proc.Txnnumber = Dwt.Txnnumber
								  AND Proc.Txnregisternumber = Dwt.Txnregisternumber
								  AND Proc.Promotiontype = Dwt.Promotiontype);

		COMMIT;

	END Processpromoexclusions;

PROCEDURE ProcessPromoRedemptions IS
  BEGIN

    -- set a flag so it can be used for research...
    DECLARE
        CURSOR get_data IS SELECT /*+ cardinality (t 100) */
                                  max(lid.a_offercode) AS a_offercode
                           ,      t.rowid AS the_rowid
                           FROM ats_txnlineitemdiscount lid
                           ,    ae_custompromo_reasoncodes rc
                           ,    Ae_Custompromo_Details_Wrktble t
                           WHERE lid.a_parentrowkey = t.txnrowkey
                           AND lid.a_offercode = rc.reasoncode
                           group by t.rowid;
        TYPE t_tbl IS TABLE OF get_data%ROWTYPE;
        v_tbl t_tbl := t_tbl();
      BEGIN
        OPEN get_data;
        LOOP
          FETCH get_data BULK COLLECT INTO v_tbl LIMIT 500;
            FORALL i IN 1..v_tbl.count
                   UPDATE Ae_Custompromo_Details_Wrktble t
                   SET t.ispromoredemption = 'Y'
                   ,   t.dtlreasoncode = v_tbl(i).a_offercode
                   WHERE ROWID = v_tbl(i).the_rowid;
            COMMIT;
            EXIT WHEN get_data%NOTFOUND;
        END LOOP;
        IF get_data%ISOPEN THEN
          CLOSE get_data;
        END IF;
      END;


    -- since the AE_CUSTOMPROMO_REDEMPTIONS is a global temp table, we do not need to clear the data from the last run
    -- because its automatically deleted once there is a commit;
    -- insert data into a global temp table
    -- it should be faster to search the work table once to pull all of the data to insert into
    -- the redemption table and then delete from the work table based on the dtldetailid since
    -- that field is indexed.
    INSERT INTO AE_CUSTOMPROMO_REDEMPTIONS
      (dtldetailid, ipcode, redemptionamount, reasoncode, redemptiondate)
      SELECT dwt.dtldetailid, dwt.ipcode, 1, dwt.dtlreasoncode, dwt.txndate
        FROM AE_CUSTOMPROMO_DETAILS_WRKTBLE dwt
       WHERE (dwt.dtlsaleamount = 0 OR dwt.dtlsaleamount = .01)
         AND dwt.ispromoredemption = 'Y';

    -- remove unknown promo reason codes
    DELETE FROM AE_CUSTOMPROMO_REDEMPTIONS red
     WHERE NOT EXISTS (SELECT 1
              FROM Ae_Custompromo_Reasoncodes rc
             WHERE red.reasoncode = rc.reasoncode);

    --  ATS_MEMBERBRAPROMOCERTREDEEM is the table for holding bra redemptions
    INSERT INTO ATS_MEMBERBRAPROMOCERTREDEEM(A_RowKey, A_ParentRowKey, A_IpCode, A_RedemptionAmount, A_ReasonCode, A_RedemptionDate, StatusCode, CreateDate, UpdateDate)
    SELECT SEQ_ROWKEY.NEXTVAL, -1, rd.IPCode, rd.redemptionamount, rd.reasoncode, rd.redemptiondate, 1, SYSDATE, SYSDATE
    FROM AE_CUSTOMPROMO_REDEMPTIONS rd;
/*
        -- commented out so research can be done
    -- this will delete all of the redemption items from the work table so we dont have to worry about filtering them out later when pulling the counts
    DELETE FROM AE_CUSTOMPROMO_DETAILS_WRKTBLE dwt
     WHERE EXISTS (SELECT 1
              FROM AE_CUSTOMPROMO_REDEMPTIONS rd
             WHERE rd.dtldetailid = dwt.dtldetailid);
*/
  END ProcessPromoRedemptions;

  	PROCEDURE ProcessPromoRollingBalance IS
	BEGIN

    ----------------------------------------------------------
    --Build the work table with the counts at the month level
    ----------------------------------------------------------
    DECLARE
       v_delta_start_id         NUMBER;
       CURSOR get_data IS select /*+ cardinality (ref 100) */ txn.a_ipcode AS vckey
                         ,      TRUNC(a_txndate,'mm') AS txn_date
                         ,      sum(CASE WHEN ref.category = 'bras'
                                         THEN CASE when txn.a_dtltypeid = 1 Then txn.a_dtlquantity else txn.a_dtlquantity * -1 END
                                         ELSE 0
                                         END) AS bra_count
                         ,      sum(CASE WHEN ref.category = 'jeans'
                                         THEN CASE when txn.a_dtltypeid = 1 Then txn.a_dtlquantity else txn.a_dtlquantity * -1 END
                                         ELSE 0
                                         END) AS jean_count
                         ,      MAX(txn.a_rowkey) AS source_id
                         from ats_txndetailitem txn
                         ,    (select column_value as dtlclasscode, 'bras' as category
                               from Lw_Clientconfiguration x
                               ,    table(parse_varchar(x.Value,','))
                               WHERE x.Key = 'BraPromoClassCodes'
                               union all
                               select column_value as dtlclasscode, 'jeans' as category
                               from Lw_Clientconfiguration x
                               ,    table(parse_varchar(x.Value,','))
                               WHERE x.Key = 'JeansPromoClassCodes'
                               ) ref

                         where  1=1
                         AND txn.a_txndate > (sysdate-366)
                         and txn.a_dtlclasscode = ref.dtlclasscode
                         --and txn.a_rowkey >  v_delta_start_id
                         group by txn.a_ipcode
                         ,        TRUNC(a_txndate,'mm')
                         ORDER BY source_id;  /* must do this order by */
          TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
          v_tbl t_tab; ---<------ our arry object
    BEGIN
      SELECT nvl(MAX(source_id),0)
      INTO v_delta_start_id
      FROM worktbl_product_summary;

      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 100;
          FORALL i IN 1..v_tbl.count
              UPDATE worktbl_product_summary
              SET source_id = v_tbl(i).source_id
              ,   jean_count = nvl(v_tbl(i).jean_count,0)
              ,   bra_count = nvl(v_tbl(i).bra_count,0)
              WHERE txn_date = v_tbl(i).txn_date
              AND vckey = v_tbl(i).vckey;
          /* dont commit here */
          FORALL i IN 1..v_tbl.count
              INSERT INTO worktbl_product_summary
                (source_id,jean_count,bra_count,vckey,txn_date)
              SELECT
                v_tbl(i).source_id,v_tbl(i).jean_count,v_tbl(i).bra_count,v_tbl(i).vckey,v_tbl(i).txn_date
              FROM dual
              WHERE NOT EXISTS (SELECT 1
                                FROM worktbl_product_summary
                                WHERE txn_date = v_tbl(i).txn_date
                                AND vckey = v_tbl(i).vckey);
              COMMIT;
        EXIT WHEN get_data%NOTFOUND;
      END LOOP get_data;

      IF get_data%ISOPEN THEN
        CLOSE get_data;
      END IF;
    END;

    ------------------------------------------------------------------------------------------------------
    --Update the bra promo summary from the counts in the work table with the counts at the month level
    ------------------------------------------------------------------------------------------------------
    DECLARE
       v_delta_start_id         NUMBER;
       v_Exists                 NUMBER;
       CURSOR get_data IS select vc.ipcode
                                 ,case
                                   when sum(ps.bra_count) < 0 then 0
                                 else sum(ps.bra_count)
                                 end as bra_count
                                 ,case
                                   when sum(ps.jean_count) < 0 then 0
                                 else sum(ps.jean_count)
                                 end as jean_count
                             from worktbl_product_summary ps
                                  inner join lw_virtualcard vc on ps.vckey = vc.vckey
                             where      ps.txn_date > (sysdate-366)
                             group by vc.ipcode;  /* must do this order by */
          TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
          v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 100;
          FORALL i IN 1..v_tbl.count
                  UPDATE ats_memberbrapromosummary bs
                  SET bs.a_brarollingbalance = nvl(v_tbl(i).bra_count, 0)
                  ,   bs.a_jeansrollingbalance = nvl(v_tbl(i).jean_count, 0)
                  WHERE bs.a_ipcode = v_tbl(i).ipcode;

          FORALL i IN 1..v_tbl.count
                 INSERT INTO ats_memberbrapromosummary (a_rowkey, a_ipcode, a_parentrowkey, a_currentbalance, a_fulfillmentbalance, a_totalpurchased, a_totalreturned, statuscode, createdate, updatedate, a_bralifetimebalance, a_brarollingbalance, a_brafirstpurchasedate, a_jeanslifetimebalance, a_jeansrollingbalance, a_jeansfirstpurchasedate)
                  SELECT seq_rowkey.nextval, v_tbl(i).ipcode, -1, 0, 0, 0, 0, 1, sysdate, sysdate, v_tbl(i).bra_count, 0, null, 0, v_tbl(i).jean_count, null
                  FROM dual
                  WHERE NOT EXISTS (SELECT 1
                                FROM ats_memberbrapromosummary
                                WHERE a_ipcode = v_tbl(i).ipcode);

          COMMIT;
        EXIT WHEN get_data%NOTFOUND;
      END LOOP get_data;

      IF get_data%ISOPEN THEN
        CLOSE get_data;
      END IF;
    END;

	END ProcessPromoRollingBalance;

	PROCEDURE Fulfillbrapromo(p_Fulfillmentdate VARCHAR2,	Retval IN OUT Rcursor) IS
		-- variable declarations
		v_Fulfillmentdate      DATE;
		v_Inhomedate           DATE;
		v_Reasoncode           NUMBER;
		v_Fulfillmentthreshold INTEGER;
	BEGIN

		SELECT To_Date(p_Fulfillmentdate, 'MM/DD/YYYY')
		       INTO v_Fulfillmentdate
		FROM Dual;

		SELECT To_Number(To_Char(c.Value))
		       INTO v_Fulfillmentthreshold
		FROM Lw_Clientconfiguration c
		WHERE c.Key = 'BraPromoFulFillmentThreshold';

		-- probably will need to format v_FulFillmentDate
		-- get reasoncode for the current run
		SELECT Brc.a_Reasoncode, Brc.a_Inhomedate
		       INTO v_Reasoncode, v_Inhomedate
		FROM Ats_Brareasoncodes Brc
		WHERE Brc.a_Mailfilepulldate = Trunc(v_Fulfillmentdate);

		EXECUTE IMMEDIATE 'TRUNCATE TABLE AE_CUSTOMPROMO_FULFILL_CERT_WT';

		INSERT INTO Ae_Custompromo_Fulfill_Cert_Wt (Ipcode, Cert_Row_Key)
			SELECT Cert.a_Ipcode, Cert.a_Rowkey
			FROM Ats_Memberbrapromocerthistory Cert
			     INNER JOIN Lw_Loyaltymember Lm ON Cert.a_Ipcode = Lm.Ipcode
			WHERE Cert.a_Isfulfilled = 0
			AND Cert.a_Isdeleted = 0
			AND Cert.a_Fulfillmentdate <= v_Fulfillmentdate
			AND lm.memberstatus = 1
			AND EXISTS (SELECT 1
						      FROM Ats_Memberdetails Md
						      WHERE Cert.a_Ipcode = Md.a_Ipcode
									AND md.a_addressmailable = 1
									AND (Md.a_Employeecode IN(0, 2) OR Md.a_Employeecode IS NULL));

		EXECUTE IMMEDIATE 'TRUNCATE TABLE Ats_Brapromofulfillment';

		INSERT INTO Ats_Brapromofulfillment	(a_Rowkey, a_Ipcode, a_Loyaltyidnumber, a_Firstname, a_Lastname, a_Addresslineone, a_Addresslinetwo, a_City, a_Stateorprovince, a_Ziporpostalcode, a_Country, a_Reasoncode, a_Languagepreference, a_Isunderage, a_Purchasedaebrand, a_Purchasedaeriebrand, a_Purchasedkidsbrand, a_Baseloyaltybrand)
			SELECT Hibernate_Sequence.Nextval, Md.a_Ipcode, Vc.loyaltyidnumber, Lm.Firstname, Lm.Lastname, Md.a_Addresslineone, Md.a_Addresslinetwo, Md.a_City, Md.a_Stateorprovince, Md.a_Ziporpostalcode, Md.a_Country, v_Reasoncode, Md.a_Languagepreference, Md.a_Isunderage, 0, 0, 0, Md.a_Basebrandid
			FROM Ats_Memberdetails Md
      			INNER JOIN Lw_Loyaltymember Lm ON Md.a_Ipcode = Lm.Ipcode
			      INNER JOIN Lw_Virtualcard Vc ON Lm.Ipcode = Vc.Ipcode AND Vc.Isprimary = 1
			WHERE EXISTS (SELECT 1
						        FROM Ae_Custompromo_Fulfill_Cert_Wt Wt
						        WHERE Md.a_Ipcode = Wt.Ipcode)
			AND lm.memberstatus = 1;

		-- UPDATE BASEBRAND
		UPDATE Ats_Brapromofulfillment Pf
		SET Pf.a_Numberofcerts = (SELECT COUNT(Wt.Ipcode)
				                      FROM Ae_Custompromo_Fulfill_Cert_Wt Wt
				                      WHERE Wt.Ipcode = Pf.a_Ipcode);

		UPDATE Ats_Brapromofulfillment Pf
		SET Pf.a_Purchasedaebrand = 1
		WHERE EXISTS (SELECT 1 FROM Ats_Memberbrand Mb
					        WHERE Mb.a_Ipcode = Pf.a_Ipcode
								  AND Mb.a_Brandid = 1);

		UPDATE Ats_Brapromofulfillment Pf
		SET Pf.a_Purchasedaeriebrand = 1
		WHERE EXISTS (SELECT 1
					        FROM Ats_Memberbrand Mb
					        WHERE Mb.a_Ipcode = Pf.a_Ipcode
								  AND Mb.a_Brandid = 2);

		UPDATE Ats_Brapromofulfillment Pf
		SET Pf.a_Purchasedkidsbrand = 1
		WHERE EXISTS (SELECT 1
					        FROM Ats_Memberbrand Mb
					        WHERE Mb.a_Ipcode = Pf.a_Ipcode
								  AND Mb.a_Brandid = 3);

		UPDATE Ats_Memberbrapromocerthistory Cert
		SET Cert.a_Isfulfilled = 1,
				Cert.a_Changedby   = Cert.a_Changedby || ' FulFillmentSend'
		WHERE EXISTS (SELECT 1
					        FROM Ae_Custompromo_Fulfill_Cert_Wt Wt
					        WHERE Wt.Cert_Row_Key = Cert.a_Rowkey);

		INSERT INTO Ats_Memberbrapromocertsummary (a_Rowkey, a_Ipcode, a_Fulfillmentperiod, a_Reasoncode, a_Numcertsawarded, a_Numbercertsredeemed, a_Processdate, a_Inhomedate, Createdate, Updatedate)
		SELECT Hibernate_Sequence.Nextval, Ful.a_Ipcode, v_Fulfillmentdate, v_Reasoncode, Ful.a_Numberofcerts, 0, Trunc(SYSDATE), v_Inhomedate, SYSDATE, SYSDATE
		FROM Ats_Brapromofulfillment Ful;
/*
		UPDATE Ats_Memberbrapromosummary Ps
		SET Ps.a_Fulfillmentbalance = nvl(Ps.a_Fulfillmentbalance, 0) +
																	(SELECT SUM(Pf.a_Numberofcerts *
																							v_Fulfillmentthreshold)
																	 FROM Ats_Brapromofulfillment Pf
																	 WHERE Pf.a_Ipcode = Ps.a_Ipcode);
*/
		COMMIT;
	END Fulfillbrapromo;

	/*******************************************************************
    * BOY - beginning of year
    * This stored procedure is designed to 'reset' bra counters for
      all members at the beginning of the year
    * It will reset all of the bra counters = 0 by updating the
         PromoSummary.CurrentBalance = 0

  ********************************************************************/
	PROCEDURE BOY_RESET_BRA_COUNTERS(p_OverrideLastRunDate VARCHAR2, p_Dummy VARCHAR2,	Retval  IN OUT Rcursor) AS
		v_Sysdate              DATE;
 		v_LastBraResetDate     VARCHAR2(30) := '';

		--log job attributes
		v_My_Log_Id            NUMBER;
		v_Jobdirection         NUMBER := 0;
		v_Starttime            DATE := SYSDATE;
		v_Endtime              DATE;
		v_Messagesreceived     NUMBER := 0;
		v_Messagesfailed       NUMBER := 0;
		v_Jobstatus            NUMBER := 0;
		v_Jobname              VARCHAR2(256) := 'BOY_RESET_BRA_COUNTERS';
		v_Filename             VARCHAR2(512) := '';

    --log msg attributes
    v_Messageid  NUMBER;
    v_Envkey     VARCHAR2(256) := 'BP_AE@' ||
                                  Upper(Sys_Context('userenv',
                                                    'instance_name'));
    v_Logsource  VARCHAR2(256) := 'Aecustompromo.BOY_RESET_BRA_COUNTERS';
    v_Batchid    VARCHAR2(256) := 0;
    v_Message    VARCHAR2(256);
    v_Reason     VARCHAR2(256);
    v_Error      VARCHAR2(256);
    v_Trycount   NUMBER := 0;

    CURSOR get_promo_summary_reset IS
        SELECT t.a_ipcode AS IPCode
        FROM ats_memberbrapromosummary t
        WHERE t.a_currentbalance > 0;

     TYPE t_tabsum IS TABLE OF get_promo_summary_reset%ROWTYPE;
     v_tblsum t_tabsum;

     CURSOR get_promo_cert IS
        SELECT t.a_rowkey AS CertRowKey
        FROM ats_memberbrapromocerthistory t
        WHERE t.a_isdeleted = 0
        AND   t.a_isfulfilled = 0;

     TYPE t_tabcert IS TABLE OF get_promo_cert%ROWTYPE;
     v_tblcert t_tabcert;


	BEGIN
		SELECT SYSDATE INTO v_Sysdate FROM Dual;

		-- get job id for this process
		v_My_Log_Id  := Utility_Pkg.Get_Libjobid();

		/* log start of job */
		Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
												p_Jobdirection     => v_Jobdirection,
												p_Filename         => v_Filename,
												p_Starttime        => v_Starttime,
												p_Endtime          => v_Endtime,
												p_Messagesreceived => v_Messagesreceived,
												p_Messagesfailed   => v_Messagesfailed,
												p_Jobstatus        => v_Jobstatus,
												p_Jobname          => v_Jobname);


    -- to prevent possible loss of bra counts, since this is a once a year process,
    -- add in a check to be sure that it will throw an error if run more than once
    -- in a given year
    IF (p_OverrideLastRunDate IS NULL OR p_OverrideLastRunDate = '0') THEN
       SELECT config.value
              INTO v_LastBraResetDate
       FROM lw_clientconfiguration config
       WHERE config.key = 'LastBraResetDate';

       -- The first date was set to 99/99/9999
       -- if it equals 99/99/9999 then by pass the date check
       IF v_LastBraResetDate <> '99/99/9999' THEN

         IF (extract(YEAR FROM to_date(v_LastBraResetDate, 'mm/dd/yyyy')) = extract(YEAR FROM SYSDATE)) THEN
             -- the process has already been run for the current year, we need to
             -- throw an error and exit the stored proc

             v_Messagesfailed := v_Messagesfailed + 1;
             v_Message := 'BOY_RESET_BRA_COUNTERS: Process was already run for the current year.';
             /*log error*/
              Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                  p_Envkey    => v_Envkey,
                                  p_Logsource => v_Logsource,
                                  p_Filename  => v_Filename,
                                  p_Batchid   => v_Batchid,
                                  p_Jobnumber => v_My_Log_Id,
                                  p_Message   => v_Message,
                                  p_Reason    => v_Reason,
                                  p_Error     => v_Error,
                                  p_Trycount  => v_Trycount,
                                  p_Msgtime   => SYSDATE);


              v_Endtime := SYSDATE;

              Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                  p_Jobdirection     => v_Jobdirection,
                                  p_Filename         => v_Filename,
                                  p_Starttime        => v_Starttime,
                                  p_Endtime          => v_Endtime,
                                  p_Messagesreceived => v_Messagesreceived,
                                  p_Messagesfailed   => v_Messagesfailed,
                                  p_Jobstatus        => v_Jobstatus,
                                  p_Jobname          => v_Jobname);

              COMMIT;


         			Raise_Application_Error(-20001, 'BOY_RESET_BRA_COUNTERS: Process was already run for the current year. ');
           END IF;
       END IF;
    END IF;


    -- Get distinct count of members that will have their counters reset
    -- Note: using union will remove all duplicate matching rows
    --       since we are only pulling ipcode, it will pull a distinct ipcode list
    SELECT count(x.a_ipcode)
           INTO v_Messagesreceived
    FROM (
         SELECT DISTINCT ps.a_ipcode
         FROM ats_memberbrapromosummary ps
         WHERE ps.a_currentbalance > 0

         UNION

         SELECT DISTINCT cert.a_ipcode
         FROM ats_memberbrapromocerthistory cert
         WHERE cert.a_isfulfilled = 0 AND cert.a_isdeleted = 0
    ) x;

    OPEN get_promo_summary_reset;
      LOOP
        FETCH get_promo_summary_reset BULK COLLECT
          INTO v_tblsum LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tblsum.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop

          UPDATE ats_memberbrapromosummary ps
          SET ps.a_currentbalance = 0,
              ps.updatedate = SYSDATE
          WHERE ps.a_ipcode = v_tblsum(i).IPCode;

        COMMIT;
        EXIT WHEN get_promo_summary_reset%NOTFOUND;
      END LOOP;
      COMMIT;
      IF get_promo_summary_reset%ISOPEN THEN
        CLOSE get_promo_summary_reset;
      END IF;

      OPEN get_promo_cert;
      LOOP
        FETCH get_promo_cert BULK COLLECT
          INTO v_tblcert LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tblcert.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop

          UPDATE ats_memberbrapromocerthistory ch
          SET ch.a_isdeleted = 1,
              ch.a_isfulfilled = 1,
              ch.a_changedby = 'BOY_RESET_BRA_COUNTERS',
              ch.updatedate = SYSDATE
          WHERE ch.a_rowkey = v_tblcert(i).CertRowKey;

        COMMIT;
        EXIT WHEN get_promo_cert%NOTFOUND;
      END LOOP;
      COMMIT;
      IF get_promo_cert%ISOPEN THEN
        CLOSE get_promo_cert;
      END IF;

    UPDATE lw_clientconfiguration config
    SET config.value = to_char(EXTRACT(MONTH FROM SYSDATE) || '/' || EXTRACT(DAY FROM SYSDATE) || '/' || EXTRACT(YEAR FROM SYSDATE))
    WHERE config.key = 'LastBraResetDate';

		-- log end of job
    v_Endtime := SYSDATE;

		Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
												p_Jobdirection     => v_Jobdirection,
												p_Filename         => v_Filename,
												p_Starttime        => v_Starttime,
												p_Endtime          => v_Endtime,
												p_Messagesreceived => v_Messagesreceived,
												p_Messagesfailed   => v_Messagesfailed,
												p_Jobstatus        => v_Jobstatus,
												p_Jobname          => v_Jobname);

		COMMIT;
	END BOY_RESET_BRA_COUNTERS;

END Aecustompromo;
/

