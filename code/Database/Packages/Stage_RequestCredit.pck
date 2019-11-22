CREATE OR REPLACE PACKAGE Stage_RequestCredit IS
type rcursor IS REF CURSOR;
  PROCEDURE RequestCredit_Staging (retval IN OUT rcursor);
END Stage_RequestCredit;
/
CREATE OR REPLACE PACKAGE BODY Stage_RequestCredit   IS
  /********************************************************************
  ********************************************************************
  ********************************************************************/
function BuildJobSuccessEmailHTML(p_JobNumber in varchar, p_RecordCount in varchar, p_Errors in varchar
         , p_JobStart in date, p_JobEnd in date) return clob is
  v_html_message  clob;

begin

  v_html_message := '<html>'||chr(10)||
        '<body>'||chr(10)||
        'Job#: '||p_JobNumber||chr(10)||
        '<br><br>' || chr(10)||
        '------------------------------------------------'||
        '<table>'||
        '<tr><td>Records Processed:</td><td>'||p_RecordCount||'</td></t4>'||
        '<tr><td>Number of Errors:</td><td>'||p_Errors||'</td></t4>'||
        '<tr><td>Start Date/Time:</td><td>'||to_char(p_JobStart, 'mm/dd/yyyy hh:mi:ss')||'</td></t4>'||
        '<tr><td>End Date/Time:</td><td>'||to_char(p_JobEnd, 'mm/dd/yyyy hh:mi:ss')||'</td></t4>'||
        '</table>'||
        '<br>' || chr(10);

  v_html_message := v_html_message||chr(10)||'</body>'||chr(10)||'</html>';

  return v_html_message;
end BuildJobSuccessEmailHTML;

 /********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/
PROCEDURE RequestCredit_Staging (retval          IN OUT rcursor)
IS
 /*   version 1.1
      CreatedBy RKG
      CreatedOn 09/25/2014
      1.1    EHP    Replaced ExtendedPlayCode comparision with func'AE_ISINPILOT' func to determine the member pilot status
  */
  v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;

  --log job attributes
  v_jobdirection          NUMBER:=0;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);
  v_processId             NUMBER:=0;
  v_24hr_Txn_Cnt          NUMBER:=0;
  v_Txnqualpurchaseamt    NUMBER:=0;
  v_returnmessage         VARCHAR2(1000);
  v_Errormessage          VARCHAR2(1000);
  v_FirstName             VARCHAR2(256);
  v_LastName              VARCHAR2(256);
  v_EmailAddress          VARCHAR2(256);
  v_attachments cio_mail.attachment_tbl_type;
  v_RowKey                VARCHAR2(256);
  v_LoyaltyNumber         VARCHAR2(256);

  --log msg attributes
  v_messageid             VARCHAR2(256);
  v_envkey                VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource             VARCHAR2(256);
  v_batchid               VARCHAR2(256):=0 ;
  v_message               VARCHAR2(1000) ;
  v_reason                VARCHAR2(1000) ;
  v_error                 VARCHAR2(1000) ;
  v_trycount              NUMBER :=0;
  v_recordcount           NUMBER :=0;
  v_ipcode                NUMBER :=0;
  v_countVirtualCard      NUMBER :=0; --AEO-543
  v_countVirtualCard2     NUMBER :=0; --AEO-543
  --<AEO-1466>
  v_ReqCredFound          UTL_FILE.FILE_TYPE;
  v_ReqCredNotFoundWeb    UTL_FILE.FILE_TYPE;
  v_ReqCredNotFoundStore  UTL_FILE.FILE_TYPE;
  v_fileLocation          VARCHAR2(300) := 'AE_IN';
  v_ReqCredFoundName      VARCHAR2(120):='AEO_RequestCredit_Found_'|| to_char(sysdate, 'mmddyyyy') ||'.csv';
  v_RCNotFoundWebName     VARCHAR2(120):='AEO_RequestCredit_NotFoundWeb_'|| to_char(sysdate, 'mmddyyyy') ||'.csv';
  v_RCNotFoundStoreName   VARCHAR2(120):='AEO_RequestCredit_NotFoundStore_'|| to_char(sysdate, 'mmddyyyy') ||'.csv';
  --</AEO-1466>
  
BEGIN
   /* get job id for this process and the dap process */
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname := 'RequestCredit_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
                     ,p_jobdirection       => v_jobdirection
                     ,p_filename           => null
                     ,p_starttime          => v_starttime
                     ,p_endtime            => v_endtime
                     ,p_messagesreceived   => v_messagesreceived
                     ,p_messagesfailed     => v_messagesfailed
                     ,p_jobstatus          => v_jobstatus
                     ,p_jobname            => v_jobname);

  Execute Immediate 'Truncate Table lw_txndetail_stage';
  Execute Immediate 'Truncate Table err$_lw_txndetail_stage';

  utility_pkg.Log_Process_Start(v_jobname, 'Stage TxnDetail Store Txns', v_processId);

  v_ReqCredFound := UTL_FILE.FOPEN(location     => v_fileLocation,
								   filename     => v_ReqCredFoundName,
                                   open_mode    => 'w',
                                   max_linesize => 32767);
  ---Create header record for fieldnames
  UTL_FILE.PUT_LINE(v_ReqCredFound,
	  'EmailAddress'           || ',' ||
	  'FirstName'              || ',' ||
	  'LastName');

  ----------------------------------------------------
  --Insert TxnDetail records into Staging table for Store Txns
  --This Bulk Collect will log any errors to the err$_lw_txndetail_stage table.  
  --The rowkey of the MemberReceipts record will be stored in the lastdmlid (not the last_dml_id
  --so the error can be tracked back to the member receipt that caused the error.
  ----------------------------------------------------
  DECLARE
    CURSOR get_data IS
    select r.a_rowkey as ReceiptRowKey, r.a_ipcode, vc.vckey, hd.a_txnheaderid, md.a_emailaddress, 
		                  lm.firstname, lm.lastname, count(hd.a_txnheaderid), vc.loyaltyidnumber
            from ats_memberreceipts r
            inner join ats_memberdetails md on r.a_ipcode = md.a_ipcode
            inner join lw_loyaltymember lm on r.a_ipcode = lm.ipcode
            inner join ats_historytxndetail hd on r.a_txnstoreid = hd.a_storenumber
                  and r.a_txnregisternumber = hd.a_txnregisternumber and r.a_txnnumber = hd.a_txnnumber and trunc(r.a_txndate) = trunc(hd.a_txndate)
            inner  join lw_virtualcard vc on r.a_ipcode = vc.ipcode and isprimary = 1
            /*Redesign changes  start here --------------------------------------SCJ           */
            where 1=1
            and   
/*						(r.createdate > TRUNC(sysdate,'YEAR')
            and   r.statuscode = 1
            and   r.a_receipttype = 1
            and   r.a_expirationdate > sysdate
            --and   AE_ISINPILOT(md.a_extendedplaycode) <> 1
				    )    \*   V.1.1 Changed for Point Conversion                                --EHP *\
            or  */  
						(r.createdate > TRUNC(sysdate,'YEAR')
            and   r.statuscode = 1
            and   r.a_receipttype = 1
            and   r.a_expirationdate > sysdate
            --and   AE_ISINPILOT(md.a_extendedplaycode) = 1    /*   V.1.1 Changed for Point Conversion                                --EHP */
            and   hd.a_txndate >= Trunc(sysdate -60)
            and   hd.a_txndate <= sysdate)
            group by r.a_rowkey, r.a_ipcode, vc.vckey, hd.a_txnheaderid, md.a_emailaddress, lm.firstname, lm.lastname, vc.loyaltyidnumber
            order by r.a_rowkey;
            /*Redesign changes  end here --------------------------------------SCJ           */
    TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
    v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count
         LOOP
           v_RowKey := v_tbl(i).ReceiptRowKey;
           v_FirstName := v_tbl(i).firstname;
					 v_LastName := v_tbl(i).lastname;
           v_EmailAddress := v_tbl(i).a_emailaddress;
           v_ipcode       := v_tbl(i).a_ipcode;

          insert into lw_txndetail_stage (rowkey, ipcode, vckey, dtlquantity, dtldiscountamount, dtlclearanceitem, dtldatemodified, reconcilestatus, txnheaderid, txndetailid, brandid, fileid, processid
         , filelineitem, cardid, creditcardid, txnloyaltyid, txnmaskid, txnnumber, txndate, txndatemodified, txnregisternumber, txnstoreid, txntypeid, txnamount, txndiscountamount
         , txnqualpurchaseamt, txnemailaddress, txnphonenumber, txnemployeeid, txnchannelid, txnoriginaltxnrowkey, txncreditsused, dtlitemlinenbr, dtlproductid, dtltypeid, dtlactionid
         , dtlretailamount, dtlsaleamount, dtlclasscode, errormessage, shipdate, ordernumber, skunumber, tenderamount, storenumber, statuscode, createdate, updatedate, lastdmlid, nonmember
         , txnoriginalstoreid, txnoriginaltxndate, txnoriginaltxnnumber,currencyrate,currencycode,dtlsaleamount_org)
         Select  seq_rowkey.nextval, v_tbl(i).a_ipcode, v_tbl(i).vckey, hst.a_dtlquantity, hst.a_dtldiscountamount, hst.a_dtlclearanceitem, hst.a_dtldatemodified, hst.a_reconcilestatus, hst.a_txnheaderid, hst.a_txndetailid, hst.a_brandid, hst.a_fileid, 0
               , hst.a_filelineitem, hst.a_cardid, hst.a_creditcardid, v_tbl(i).loyaltyidnumber, hst.a_txnmaskid, hst.a_txnnumber, hst.a_txndate, hst.a_txndatemodified, hst.a_txnregisternumber, hst.a_txnstoreid, hst.a_txntype, hst.a_txnamount, hst.a_txndiscountamount
               , hst.a_txnqualpurchaseamt, hst.a_txnemailaddress, hst.a_txnphonenumber, hst.a_txnemployeeid, hst.a_txnchannelid, hst.a_txnoriginaltxnrowkey, hst.a_txncreditsused, hst.a_dtlitemlinenbr, hst.a_dtlproductid, hst.a_dtltypeid, hst.a_dtlactionid
               , hst.a_dtlretailamount, hst.a_dtlsaleamount, hst.a_dtlclasscode, hst.a_errormessage, hst.a_shipdate, hst.a_ordernumber, hst.a_skunumber, hst.a_tenderamount, hst.a_storenumber, hst.statuscode, hst.createdate, hst.updatedate, v_tbl(i).ReceiptRowKey, 0
               , 0, '', '' ,
               --AEO-846 BEGIN
               hst.a_currencyrate, hst.a_currencycode, hst.a_dtlsaleamount_org
               --AEO-846 END
         From    ats_historytxndetail hst
         Where   hst.a_txnheaderid = v_tbl(i).a_txnheaderid
         /* Changes begin for AEO45 AEO52                                --SCJ */
         and not exists (select 1                 -- Checking if the txnheader id is not already posted
                         from ats_txnheader ht
                         where ht.a_txnheaderid = v_tbl(i).a_txnheaderid)
         /* Changes end for AEO45 AEO52                                --SCJ */
--         LOG ERRORS INTO err$_lw_txndetail_stage('UPDATE') REJECT LIMIT UNLIMITED;
         LOG ERRORS INTO err$_lw_txndetail_stage('INSERT') REJECT LIMIT UNLIMITED;

         Select count(*) into v_trycount From err$_lw_txndetail_stage where lastdmlid = v_RowKey;

         v_recordcount := v_recordcount + 1;
         v_messagesreceived := v_messagesreceived + 1;


          --Mark the HistoryTxn record as Request Credit Processed
         Update ats_historytxndetail hst
         Set hst.a_processid = 7,
             hst.a_txnloyaltyid = v_tbl(i).loyaltyidnumber
/* Changes begin for AEO707                                --FJCB*/
             ,hst.a_ipcode = v_tbl(i).a_ipcode
             , hst.a_vckey = v_tbl(i).vckey
/* Changes end for AEO707                                --FJCB*/
         Where hst.a_txnheaderid = v_tbl(i).a_txnheaderid;

          --Mark the Receipt record as Posted and set the TxnHeaderId
         Update ats_memberreceipts mr
         Set mr.statuscode = 2,
             mr.a_txnheaderid = v_tbl(i).a_txnheaderid,
             mr.updatedate = sysdate,
             mr.a_changedby = 'Request Credit Job'
         Where mr.a_rowkey = v_tbl(i).ReceiptRowKey
         and not exists (select 1                 -- Checking if the txnheader id is not already posted /* Changes begin for AEO45 AEO52                                --SCJ */
                         from ats_txnheader ht
                         where ht.a_txnheaderid = v_tbl(i).a_txnheaderid)
         and  exists (select 1
                         from bp_ae.lw_txndetail_stage tds
                         where tds.txnheaderid = v_tbl(i).a_txnheaderid) ; --AEO-793
         commit;

         --Mark the Receipt record as AlreadyPosted and set the TxnHeaderId
         Update ats_memberreceipts mr
         Set mr.statuscode = 200,
             mr.a_txnheaderid = v_tbl(i).a_txnheaderid,
             mr.updatedate = sysdate,
             mr.a_changedby = 'Request Credit Job'
         Where mr.a_rowkey = v_tbl(i).ReceiptRowKey
         and exists (select 1                 -- Checking if the txnheader id is not already posted /* Changes begin for AEO45 AEO52                                --SCJ */
                         from ats_txnheader ht
                         where ht.a_txnheaderid = v_tbl(i).a_txnheaderid) ;
         commit;

         If(v_trycount = 0 and trim(v_EmailAddress) is not null  ) THEN
            --Send the success record AEO-1466
					  UTL_FILE.PUT_LINE(v_ReqCredFound,
            				  v_EmailAddress                  || ',' ||
                      v_FirstName              || ',' ||
				              v_LastName);

            If(v_returnmessage != 'SUCCESS') Then
                    v_Error          := v_returnmessage;
                    v_Reason         := 'Failed Procedure RequestCredit_Staging';
                    v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                                        '    <pkg>Stage_RequestCredit</pkg>' || Chr(10) ||
                                        '    <proc>RequestCredit_Staging</proc>' ||
                                        Chr(10) || '    <rowkey>' || v_RowKey ||
                                        '</rowkey>' || Chr(10) || '  </details>' ||
                                        Chr(10) || '</failed>';
                    utility_pkg.Log_msg(p_messageid         => v_messageid,
                         p_envkey            => v_envkey   ,
                         p_logsource         => v_logsource,
                         p_filename          => null ,
                         p_batchid           => v_batchid  ,
                         p_jobnumber         => v_my_log_id,
                         p_message           => v_message  ,
                         p_reason            => v_reason   ,
                         p_error             => v_error    ,
                         p_trycount          => v_trycount ,
                         p_msgtime           => SYSDATE  );
            END IF;
          ELSE
            v_messagesfailed := v_messagesfailed + 1;
          END IF;


        END LOOP;


        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    v_messagesreceived := 0;

----------------------------------------------------
--Insert TxnDetail records into Staging table for Online Txns
--This Bulk Collect will log any errors to the err$_lw_txndetail_stage table.  The rowkey of the MemberReceipts record will be stored in the lastdmlid (not the last_dml_id
--so the error can be tracked back to the member receipt that caused the error.
----------------------------------------------------
  utility_pkg.Log_Process_Start(v_jobname, 'Stage TxnDetail Online Txns', v_processId);
  DECLARE
      CURSOR get_data IS
       select r.a_rowkey as ReceiptRowKey, r.a_ipcode, vc.vckey, hd.a_txnheaderid, 
			        md.a_emailaddress, lm.firstname, lm.lastname, count(hd.a_txnheaderid), vc.loyaltyidnumber
            from ats_memberreceipts r
            inner join ats_memberdetails md on r.a_ipcode = md.a_ipcode
            inner join lw_loyaltymember lm on r.a_ipcode = lm.ipcode
            inner join ats_historytxndetail hd on r.a_ordernumber = hd.a_ordernumber
            inner  join lw_virtualcard vc on r.a_ipcode = vc.ipcode and isprimary = 1
            /*Redesign changes  start here --------------------------------------SCJ           */
            where  1 = 1
            and   
/*						(r.createdate > TRUNC(sysdate,'YEAR')
            and   r.statuscode = 1
            and   r.a_receipttype = 2
            and   r.a_expirationdate > sysdate
            and   hd.a_txnheaderid is not null
            --and   AE_ISINPILOT(md.a_extendedplaycode) <> 1
			      )    \*   V.1.1 Changed for Point Conversion                                --EHP *\
            or */   
						(r.createdate > TRUNC(sysdate,'YEAR')
            and   r.statuscode = 1
            and   r.a_receipttype = 2
            and   r.a_expirationdate > sysdate
            and   hd.a_txnheaderid is not null
            and   hd.a_shipdate >= Trunc(sysdate -60)
            and   hd.a_shipdate <= sysdate
            --and   AE_ISINPILOT(md.a_extendedplaycode) = 1
			)    /*   V.1.1 Changed for Point Conversion                                --EHP */
            group by r.a_rowkey, r.a_ipcode, vc.vckey, hd.a_txnheaderid, md.a_emailaddress, lm.firstname, lm.lastname, vc.loyaltyidnumber
            order by r.a_rowkey;
            /*Redesign changes  end here --------------------------------------SCJ           */
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count
         LOOP
           v_RowKey := v_tbl(i).ReceiptRowKey;
           v_FirstName := v_tbl(i).firstname;
           v_EmailAddress := v_tbl(i).a_emailaddress;
					 v_LastName := v_tbl(i).lastname;
           v_ipcode       := v_tbl(i).a_ipcode;

          insert into lw_txndetail_stage (rowkey, ipcode, vckey, dtlquantity, dtldiscountamount, dtlclearanceitem, dtldatemodified, reconcilestatus, txnheaderid, txndetailid, brandid, fileid, processid
         , filelineitem, cardid, creditcardid, txnloyaltyid, txnmaskid, txnnumber, txndate, txndatemodified, txnregisternumber, txnstoreid, txntypeid, txnamount, txndiscountamount
         , txnqualpurchaseamt, txnemailaddress, txnphonenumber, txnemployeeid, txnchannelid, txnoriginaltxnrowkey, txncreditsused, dtlitemlinenbr, dtlproductid, dtltypeid, dtlactionid
         , dtlretailamount, dtlsaleamount, dtlclasscode, errormessage, shipdate, ordernumber, skunumber, tenderamount, storenumber, statuscode, createdate, updatedate, lastdmlid, nonmember
         , txnoriginalstoreid, txnoriginaltxndate, txnoriginaltxnnumber,currencyrate,currencycode,dtlsaleamount_org)
         Select  seq_rowkey.nextval, v_tbl(i).a_ipcode, v_tbl(i).vckey, hst.a_dtlquantity, hst.a_dtldiscountamount, hst.a_dtlclearanceitem, hst.a_dtldatemodified, hst.a_reconcilestatus, hst.a_txnheaderid, hst.a_txndetailid, hst.a_brandid, hst.a_fileid, 0
               , hst.a_filelineitem, hst.a_cardid, hst.a_creditcardid, v_tbl(i).loyaltyidnumber, hst.a_txnmaskid, hst.a_txnnumber, hst.a_txndate, hst.a_txndatemodified, hst.a_txnregisternumber, hst.a_txnstoreid, hst.a_txntype, hst.a_txnamount, hst.a_txndiscountamount
               , hst.a_txnqualpurchaseamt, hst.a_txnemailaddress, hst.a_txnphonenumber, hst.a_txnemployeeid, hst.a_txnchannelid, hst.a_txnoriginaltxnrowkey, hst.a_txncreditsused, hst.a_dtlitemlinenbr, hst.a_dtlproductid, hst.a_dtltypeid, hst.a_dtlactionid
               , hst.a_dtlretailamount, hst.a_dtlsaleamount, hst.a_dtlclasscode, hst.a_errormessage, hst.a_shipdate, hst.a_ordernumber, hst.a_skunumber, hst.a_tenderamount, hst.a_storenumber, hst.statuscode, hst.createdate, hst.updatedate, v_tbl(i).ReceiptRowKey, 0
               , 0, '', '', hst.a_currencyrate, hst.a_currencycode, hst.a_dtlsaleamount_org
         From    ats_historytxndetail hst
         Where   hst.a_txnheaderid = v_tbl(i).a_txnheaderid
         /* Changes begin for AEO45 AEO52                                --SCJ */
         and not exists (select 1                 -- Checking if the txnheader id is not already posted
                         from ats_txnheader ht
                         where ht.a_txnheaderid = v_tbl(i).a_txnheaderid)
         /* Changes end for AEO45 AEO52                                --SCJ */
--         LOG ERRORS INTO err$_lw_txndetail_stage('UPDATE') REJECT LIMIT UNLIMITED;
         LOG ERRORS INTO err$_lw_txndetail_stage('INSERT') REJECT LIMIT UNLIMITED;

         Select count(*) into v_trycount From err$_lw_txndetail_stage where lastdmlid = v_RowKey;

         v_recordcount := v_recordcount + 1;
         v_messagesreceived := v_messagesreceived + 1;

          --Mark the HistoryTxn record as Request Credit Processed
         Update ats_historytxndetail hst
         Set hst.a_processid = 7,
             hst.a_txnloyaltyid = v_tbl(i).loyaltyidnumber,
             hst.a_ipcode = v_tbl(i).a_ipcode, -- AEO-794
             hst.a_vckey = v_tbl(i).vckey -- AEO-794
         Where hst.a_txnheaderid = v_tbl(i).a_txnheaderid;


          --Mark the Receipt record as Posted and set the TxnHeaderId
         Update ats_memberreceipts mr
         Set mr.statuscode = 2,
             mr.a_txnheaderid = v_tbl(i).a_txnheaderid,
             mr.updatedate = sysdate,
             mr.a_changedby = 'Request Credit Job'
         Where mr.a_rowkey = v_tbl(i).ReceiptRowKey
         and not exists (select 1                 -- Checking if the txnheader id is not already posted /* Changes begin for AEO45 AEO52                                --SCJ */
                         from ats_txnheader ht
                         where ht.a_txnheaderid = v_tbl(i).a_txnheaderid)
         and  exists (select 1
                         from bp_ae.lw_txndetail_stage tds
                         where tds.txnheaderid = v_tbl(i).a_txnheaderid) ; --AEO-793
         Commit;

            --Mark the Receipt record as AlreadyPosted and set the TxnHeaderId
         Update ats_memberreceipts mr
         Set mr.statuscode = 200,
             mr.a_txnheaderid = v_tbl(i).a_txnheaderid,
             mr.updatedate = sysdate,
             mr.a_changedby = 'Request Credit Job'
         Where mr.a_rowkey = v_tbl(i).ReceiptRowKey
         and exists (select 1                 -- Checking if the txnheader id is not already posted /* Changes begin for AEO45 AEO52                                --SCJ */
                         from ats_txnheader ht
                         where ht.a_txnheaderid = v_tbl(i).a_txnheaderid) ;
         commit;

         If(v_trycount = 0 and trim(v_EmailAddress) is not null) THEN
            --Send the success record AEO-1466
            UTL_FILE.PUT_LINE(v_ReqCredFound,
                      v_EmailAddress                  || ',' ||
                      v_FirstName              || ',' ||
                      v_LastName);

						If(v_returnmessage != 'SUCCESS') Then
						v_Error          := v_returnmessage;
						v_Reason         := 'Failed Procedure RequestCredit_Staging';
						v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
											'    <pkg>Stage_RequestCredit</pkg>' || Chr(10) ||
											'    <proc>RequestCredit_Staging</proc>' ||
											Chr(10) || '    <rowkey>' || v_RowKey ||
											'</rowkey>' || Chr(10) || '  </details>' ||
											Chr(10) || '</failed>';
						utility_pkg.Log_msg(p_messageid         => v_messageid,
							 p_envkey            => v_envkey   ,
							 p_logsource         => v_logsource,
							 p_filename          => null ,
							 p_batchid           => v_batchid  ,
							 p_jobnumber         => v_my_log_id,
							 p_message           => v_message  ,
							 p_reason            => v_reason   ,
							 p_error             => v_error    ,
							 p_trycount          => v_trycount ,
							 p_msgtime           => SYSDATE  );
						END IF;
          ELSE
            v_messagesfailed := v_messagesfailed + 1;
          END IF;


        END LOOP;


        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    v_messagesreceived := 0;
	
	
	UTL_FILE.FCLOSE(v_ReqCredFound); --AEO-1466

----------------------------------------------------
--Check TxnDetail records for > 3 Txns in 24 hour period
        --- only for store orders and excluding online orders
----------------------------------------------------
    utility_pkg.Log_Process_Start(v_jobname, 'Checking for > 3 Txn in 24 hours', v_processId);
    DECLARE
      CURSOR get_data IS
        select DISTINCT ipcode, txnheaderid, txndate, txnqualpurchaseamt
            from   lw_txndetail_stage hd where hd.ordernumber is null;

      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
        LOOP
          v_Errormessage       := '';
          v_Txnqualpurchaseamt := v_tbl(i).txnqualpurchaseamt;
          /* zero out txn amount if more then 3 txn in 24hrs */
          v_24hr_Txn_Cnt := utility_pkg.Get_24hr_Txn_Cnt(
                                              p_ipcode => v_tbl(i).ipcode,
                                              p_Date  => v_tbl(i).txndate);
          IF v_24hr_Txn_Cnt > 3
          THEN
            v_Txnqualpurchaseamt := 0;
            v_Errormessage       := '>3 txns found on txndate';
          END IF;

          update lw_txndetail_stage
          Set txnqualpurchaseamt = v_Txnqualpurchaseamt,
              errormessage = v_Errormessage
          Where txnheaderid = v_tbl(i).txnheaderid;

          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        END LOOP;

        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

----------------------------------------------------
--Insert TxnDetailDiscount records into Staging table
----------------------------------------------------
    Execute Immediate 'Truncate Table lw_txndetaildiscount_stage';
    utility_pkg.Log_Process_Start(v_jobname, 'Stage TxnDetailDiscount ', v_processId);

     -- AEO-830 begin
     INSERT INTO Lw_Txndetaildiscount_Stage
          (Rowkey,
           Ipcode,
           Vckey,
           Processid,
           Txndiscountid,
           Txnheaderid,
           Txndate,
           Txndetailid,
           Discounttype,
           Discountamount,
           Txnchannel,
           Offercode,
           Errormessage,
           Fileid,
           Statuscode,
           Createdate,
           Updatedate)
     SELECT a_Rowkey,
               Ipcode,
               Vckey,
               a_Processid,
               a_Txndiscountid,
               a_Txnheaderid,
               a_Txndate,
               a_Txndetailid,
               a_Discounttype,
               a_Discountamount,
               a_Txnchannel,
               a_Offercode,
               a_Errormessage,
               a_Fileid,
               Statuscode,
               SYSDATE,
               SYSDATE
        FROM   (SELECT DISTINCT a_Rowkey,
                                Stg.Ipcode,
                                Stg.Vckey,
                                a_Processid,
                                a_Txndiscountid,
                                a_Txnheaderid,
                                a_Txndate,
                                a_Txndetailid,
                                a_Discounttype,
                                a_Discountamount,
                                a_Txnchannel,
                                a_Offercode,
                                a_Errormessage,
                                a_Fileid,
                                Dsc.Statuscode,
                                Row_Number() Over(PARTITION BY a_Rowkey ORDER BY a_Rowkey) R
                FROM   Bp_Ae.Ats_Historytxndetaildiscount Dsc
                INNER  JOIN Bp_Ae.Lw_Txndetail_Stage Stg
                ON     Stg.Txnheaderid = Dsc.a_Txnheaderid
                       AND Stg.Txndetailid = Dsc.a_Txndetailid)
        WHERE  R = 1;
    -- AEO-830 end
    v_messagesreceived := sql%rowcount;
    commit;

    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

----------------------------------------------------
--Update processid in historytxn tables
----------------------------------------------------

    /* Changes begin for AEO707                                --FJCB*/
DECLARE
      CURSOR get_stg_data IS
      select *
      from lw_txndetail_stage stg
      ;
TYPE t_tab IS TABLE OF get_stg_data%ROWTYPE;
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_stg_data;
      LOOP
        FETCH get_stg_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count
         LOOP
         Update ats_historytxndetaildiscount hst
         Set hst.a_processid = 7
             ,hst.a_ipcode = v_tbl(i).ipcode,
             hst.a_vckey = v_tbl(i).vckey -- AEO-794
         Where 1=1
           and hst.a_txnheaderid = v_tbl(i).txnheaderid
           and hst.a_txndetailid = v_tbl(i).txndetailid;

        END LOOP;
        EXIT WHEN get_stg_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_stg_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_stg_data;
      END IF;
    END;
    /* Changes end for AEO707                                --FJCB*/

----------------------------------------------------
--Insert TxnRewardRedeem records into Staging table
----------------------------------------------------
    Execute Immediate 'Truncate Table lw_txnrewardredeem_stage';
    utility_pkg.Log_Process_Start(v_jobname, 'Stage TxnRewardRedeem', v_processId);



    -- AEO-830 begin
     INSERT INTO lw_txnrewardredeem_stage
          (Rowkey,
           Ipcode,
           Vckey,
           Txnheaderid,
           Txndate,
           Txndetailid,
           Programid,
           Certificateredeemtype,
           Certificatecode,
           Certificatediscountamount,
           Txnrewardredeemid,
           Processid,
           Fileid,
           Statuscode,
           Createdate,
           Updatedate)
      SELECT a_Rowkey,
             Ipcode,
             Vckey,
             a_Txnheaderid,
             a_Txndate,
             a_Txndetailid,
             a_Programid,
             a_Certificateredeemtype,
             a_Certificatecode,
             a_Certificatediscountamount,
             a_Txnrewardredeemid,
             a_Processid,
             a_Fileid,
             Statuscode,
             SYSDATE,
             SYSDATE
      FROM   ( select distinct(a_rowkey),
               stg.ipcode,
               stg.vckey,
               a_txnheaderid,
               a_txndate,
               a_txndetailid,
               a_programid,
               a_certificateredeemtype,
               a_certificatecode,
               a_certificatediscountamount,
               a_txnrewardredeemid,
               a_processid,
               a_fileid,
               rwd.statuscode,
               Row_Number() Over(PARTITION BY a_Rowkey ORDER BY a_Rowkey) R
    from ats_historytxnrewardredeem rwd
    INNER JOIN lw_txndetail_stage stg  ON stg.txnheaderid = rwd.a_txnheaderid AND stg.txndetailid = rwd.a_txndetailid)
        WHERE  R = 1;
    -- AEO-830 end

    v_messagesreceived := sql%rowcount;
    commit;


    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

----------------------------------------------------
--Update processid in historytxn tables
----------------------------------------------------

    /* Changes begin for AEO707                                --FJCB*/
DECLARE
      CURSOR get_stg_data IS
      select *
      from lw_txndetail_stage stg
      ;
TYPE t_tab IS TABLE OF get_stg_data%ROWTYPE;
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_stg_data;
      LOOP
        FETCH get_stg_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count
         LOOP

         Update ats_historytxnrewardredeem hst
         Set hst.a_processid = 7
             ,hst.a_ipcode =  v_tbl(i).ipcode
             ,hst.a_vckey = v_tbl(i).vckey
         Where 1=1
           and hst.a_txnheaderid = v_tbl(i).txnheaderid;
        --   and hst.a_txndetailid = v_tbl(i).txndetailid;

        END LOOP;
        EXIT WHEN get_stg_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_stg_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_stg_data;
      END IF;
    END;
    /* Changes end for AEO707                                --FJCB*/

----------------------------------------------------
--Insert TxnTender records into Staging table
----------------------------------------------------
    Execute Immediate 'Truncate Table lw_txntender_stage';
    utility_pkg.Log_Process_Start(v_jobname, 'Stage TxnTender', v_processId);

   -- AEO-830 begin
   INSERT INTO Lw_Txntender_Stage
         (Rowkey,
          Ipcode,
          Vckey,
          Processid,
          Storeid,
          Txndate,
          Txnheaderid,
          Txntenderid,
          Tendertype,
          Tenderamount,
          Tendercurrency,
          Tendertax,
          Tendertaxrate,
          Errormessage,
          Fileid,
          Statuscode,
          Createdate,
          Updatedate)
         SELECT DISTINCT (a_Rowkey),
                         Ipcode,
                         Vckey,
                         a_Processid,
                         a_Storeid,
                         a_Txndate,
                         a_Txnheaderid,
                         a_Txntenderid,
                         a_Tendertype,
                         a_Tenderamount,
                         a_Tendercurrency,
                         a_Tendertax,
                         a_Tendertaxrate,
                         a_Errormessage,
                         a_Fileid,
                         Statuscode,
                         SYSDATE,
                         SYSDATE
         FROM   (SELECT DISTINCT (a_Rowkey),
                         Stg.Ipcode,
                         Stg.Vckey,
                         a_Processid,
                         a_Storeid,
                         a_Txndate,
                         a_Txnheaderid,
                         a_Txntenderid,
                         a_Tendertype,
                         a_Tenderamount,
                         a_Tendercurrency,
                         a_Tendertax,
                         a_Tendertaxrate,
                         a_Errormessage,
                         a_Fileid,
                         Tnd.Statuscode,
                          Row_Number() Over(PARTITION BY a_Rowkey ORDER BY a_Rowkey) r
                 FROM    Ats_Historytxntender Tnd
                 INNER  JOIN Lw_Txndetail_Stage Stg
                 ON     Stg.Txnheaderid = Tnd.a_Txnheaderid)
         WHERE  r = 1;


    -- AEO-830 end

    v_messagesreceived := sql%rowcount;
    commit;
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);


----------------------------------------------------
--Update processid in historytxn tables
----------------------------------------------------

    /* Changes begin for AEO707                                --FJCB*/
DECLARE
      CURSOR get_stg_data IS
      select *
      from lw_txndetail_stage stg
      ;
TYPE t_tab IS TABLE OF get_stg_data%ROWTYPE;
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_stg_data;
      LOOP
        FETCH get_stg_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count
         LOOP

         Update ats_historytxntender hst
         Set hst.a_processid = 7
             ,hst.a_ipcode = v_tbl(i).ipcode
             , hst.a_vckey = v_tbl(i).vckey
         Where 1=1
           and hst.a_txnheaderid = v_tbl(i).txnheaderid;
        END LOOP;
        EXIT WHEN get_stg_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_stg_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_stg_data;
      END IF;
    END;
    /* Changes end for AEO707                                --FJCB*/


----------------------------------------------------
--Send out Expiration Notifications
----------------------------------------------------
utility_pkg.Log_Process_Start(v_jobname, 'Send Expiration Notifications', v_processId);
DECLARE
      CURSOR get_data IS
       select r.a_rowkey as ReceiptRowKey, md.a_emailaddress, lm.firstname, lm.lastname,
			        r.a_txnstoreid, r.a_txnregisternumber, r.a_txnnumber
              , r.a_ordernumber, r.a_tenderamount, to_char(r.a_txndate, 'mm/dd/yyyy') as TxnDate, r.a_ipcode
            from ats_memberreceipts r
            inner join ats_memberdetails md on r.a_ipcode = md.a_ipcode
            inner join lw_loyaltymember lm on r.a_ipcode = lm.ipcode
            where r.createdate > TRUNC(sysdate,'YEAR')
            and   r.statuscode = 1
            and   r.a_expirationdate < sysdate
            order by r.a_rowkey
            ;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
	  v_ReqCredNotFoundWeb := UTL_FILE.FOPEN(location     => v_fileLocation,
								   filename     => v_RCNotFoundWebName,
                                   open_mode    => 'w',
                                   max_linesize => 32767);
	  ---Create header record for fieldnames
	  UTL_FILE.PUT_LINE(v_ReqCredNotFoundWeb,
		  'EmailAddress'           || ',' ||
		  'FirstName'              || ',' ||
		  'LastName'               || ',' ||
		  'LoyaltyNumber'          || ',' ||
		  'OrderNumber'            || ',' ||
		  'TenderAmount');

      v_ReqCredNotFoundStore := UTL_FILE.FOPEN(location     => v_fileLocation,
								   filename     => v_RCNotFoundStoreName,
                                   open_mode    => 'w',
                                   max_linesize => 32767);
	  ---Create header record for fieldnames
	  UTL_FILE.PUT_LINE(v_ReqCredNotFoundStore,
		  'EmailAddress'      || ',' ||
		  'Firstname'         || ',' ||
		  'Lastname'          || ',' ||
		  'LoyaltyNumber'     || ',' ||
		  'StoreId'           || ',' ||
		  'Registernumber'    || ',' ||
		  'Txnnumber'         || ',' ||
		  'TxnDate');
	  
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count
         LOOP
           v_EmailAddress := v_tbl(i).a_emailaddress; ---AEO-850
           
          --Mark the Receipt record as No Match
         Update ats_memberreceipts mr
         Set mr.statuscode = 3
         Where mr.a_rowkey = v_tbl(i).ReceiptRowKey;
         
-- AEO-543 Begin
         SELECT COUNT(*) INTO v_countVirtualCard FROM (  SELECT *
           FROM  lw_virtualcard
           where  isprimary = 1 and ipcode = v_tbl(i).a_ipcode and rownum = 1);

         IF (v_countVirtualCard > 0)  THEN

           Select LoyaltyIdNumber
           into v_LoyaltyNumber
           From lw_virtualcard
           where  isprimary = 1 and ipcode = v_tbl(i).a_ipcode and rownum = 1;

           ELSE
               SELECT COUNT(*) INTO  v_countVirtualCard2 FROM (
               Select *
               From lw_virtualcard
               where  isprimary = 1
                      and ipcode IN (
                          SELECT mh.a_ipcode FROM ats_membermergehistory  mh
                           WHERE mh.a_fromipcode =  v_tbl(i).a_ipcode AND rownum = 1
                           )
                      and rownum = 1);

             IF (v_countVirtualCard2 > 0 )   THEN

               Select LoyaltyIdNumber
               into v_LoyaltyNumber
               From lw_virtualcard
               where  isprimary = 1
                      and ipcode IN (
                          SELECT mh.a_ipcode FROM ats_membermergehistory  mh
                           WHERE mh.a_fromipcode =  v_tbl(i).a_ipcode AND rownum = 1
                           )
                      and rownum = 1;
           ELSE
             v_LoyaltyNumber := ' ';
           END IF;
         END IF;
-- AEO-543 end

         COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
          
         --Send the expire email
         if (  trim(v_EmailAddress) is not null  ) then
            if( v_tbl(i).a_ordernumber is not null ) then
				UTL_FILE.PUT_LINE(v_ReqCredNotFoundWeb,
                      v_EmailAddress                  || ',' ||
                      v_tbl(i).firstname              || ',' ||
                      v_tbl(i).lastname               || ',' ||
                      v_LoyaltyNumber                 || ',' ||
                      v_tbl(i).a_ordernumber          || ',' ||
                      v_tbl(i).a_tenderamount);
			ELSE
				UTL_FILE.PUT_LINE(v_ReqCredNotFoundStore,
                      v_EmailAddress                  || ',' ||
                      v_tbl(i).firstname              || ',' ||
                      v_tbl(i).lastname               || ',' ||
                      v_LoyaltyNumber                 || ',' ||
                      v_tbl(i).a_txnstoreid           || ',' ||
                      v_tbl(i).a_txnregisternumber    || ',' ||
                      v_tbl(i).a_txnnumber            || ',' ||
                      v_tbl(i).TxnDate);
			END IF;

                If(v_returnmessage != 'SUCCESS') Then
                  v_Error          := v_returnmessage;
                  v_Reason         := 'Failed Procedure RequestCredit_Staging';
                  v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                            '    <pkg>Stage_RequestCredit</pkg>' || Chr(10) ||
                            '    <proc>RequestCredit_Staging</proc>' ||
                            Chr(10) || '    <rowkey>' || v_RowKey ||
                            '</rowkey>' || Chr(10) || '  </details>' ||
                            Chr(10) || '</failed>';
                  utility_pkg.Log_msg(p_messageid         => v_messageid,
                     p_envkey            => v_envkey   ,
                     p_logsource         => v_logsource,
                     p_filename          => null ,
                     p_batchid           => v_batchid  ,
                     p_jobnumber         => v_my_log_id,
                     p_message           => v_message  ,
                     p_reason            => v_reason   ,
                     p_error             => v_error    ,
                     p_trycount          => v_trycount ,
                     p_msgtime           => SYSDATE  );
                   END IF;
         else
            v_messagesreceived := v_messagesreceived + 1;
         end if;
        END LOOP;


        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
	  UTL_FILE.FCLOSE(v_ReqCredNotFoundWeb);
	  UTL_FILE.FCLOSE(v_ReqCredNotFoundStore);
    END;

    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    utility_pkg.Log_Process_Job_End(v_processId, v_recordcount);

  v_endtime := SYSDATE;
  v_jobstatus := 1;

  /* log end of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_recordcount
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

 /* log start of DAP job */
  utility_pkg.Log_job(P_JOB                => v_dap_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => 0
         ,p_messagesfailed     => 0
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

    --Send the job success email
      cio_mail.send(
          p_from_email    => 'AEREWARDS',
          p_from_replyto  => 'info@aerewards.ae.com',
          p_to_list       => 'aerewards_support@brierley.com',
          p_cc_list       => NULL,
          p_bcc_list      => NULL,
          p_subject       => 'AE Request Credit Job Success',
          p_text_message  => BuildJobSuccessEmailHTML(v_my_log_id, v_recordcount, v_messagesfailed, v_starttime, v_endtime),
          p_content_type  => 'text/html;charset=UTF8',
          p_attachments   => v_attachments, --v_attachments,
          p_priority      => '3',
          p_auth_username => NULL,
          p_auth_password => NULL,
          p_mail_server   => 'cypwebmail.brierleyweb.com');

         open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN
  IF v_messagesfailed = 0 THEN
    v_messagesfailed := 0+1;
  END IF;

  /* log end of job */
  utility_pkg.Log_job(P_JOB    => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_recordcount
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);


      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure RequestCredit_Staging';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>Stage_RequestCredit</pkg>' || Chr(10) ||
                          '    <proc>RequestCredit_Staging</proc>' ||
                          Chr(10) || '    <rowkey>' || v_RowKey ||
                          '</rowkey>' || Chr(10) || '  </details>' ||
                          Chr(10) || '</failed>';
   utility_pkg.Log_msg(p_messageid         => v_messageid,
           p_envkey            => v_envkey   ,
           p_logsource         => v_logsource,
           p_filename          => null ,
           p_batchid           => v_batchid  ,
           p_jobnumber         => v_my_log_id,
           p_message           => v_message  ,
           p_reason            => v_reason   ,
           p_error             => v_error    ,
           p_trycount          => v_trycount ,
           p_msgtime           => SYSDATE  );

    --Send the job fail email
      cio_mail.send(
          p_from_email    => 'AEREWARDS',
          p_from_replyto  => 'info@aerewards.ae.com',
          p_to_list       => 'aerewards_support@brierley.com',
          p_cc_list       => NULL,
          p_bcc_list      => NULL,
          p_subject       => 'AE Request Credit Job Failed',
          p_text_message  => 'Job Number #'||v_my_log_id||' has failed. See lw_libmessagelog table for error',
          p_content_type  => 'text/html;charset=UTF8',
          p_attachments   => v_attachments, --v_attachments,
          p_priority      => '3',
          p_auth_username => NULL,
          p_auth_password => NULL,
          p_mail_server   => 'cypwebmail.brierleyweb.com');

END RequestCredit_Staging;

END Stage_RequestCredit;
/
