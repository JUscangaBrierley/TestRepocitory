CREATE OR REPLACE PACKAGE Stage_Tlog5 IS

  /*
      usage

      Stage_Tlog05 is the main process
         It will run hist_Tlog05 for you.
         Hist_Tlog05 is called prior to processing... so only the privious data is pushed to hist.

      Hist_Tlog05
         this pushes the staged data to hist and empties the stage tables.
         It's caleed from the main stage process.
         Optionally call this proc after sucessfully running the main tlog processor (that populates ATS_ tables, aka DAP).


      if Stage data is written and needs to be backed out then dont rerun the stage process or you'll end up with dulplicated.
      for this reason the hist process probably shouldn't be called from stage and should be probably be run as a 3rd step --jcanada


  */

  /* processid states */
  Gv_Status_Ready        CONSTANT NUMBER := '0';
  Gv_Status_Processed    CONSTANT NUMBER := '1';
  Gv_Status_Unregcard    CONSTANT NUMBER := '2';
  Gv_Status_Nocard       CONSTANT NUMBER := '3';
  Gv_Status_Noproduct    CONSTANT NUMBER := '4';
  Gv_Status_Nolineitem   CONSTANT NUMBER := '5';
  /*PI 24762 begin */
  Gv_Status_Error        CONSTANT NUMBER := '8';
  /*PI 24762 end */
  Gv_Status_Employee     CONSTANT NUMBER := '0';
  Gv_Status_Dup          CONSTANT NUMBER := '12';
  Gv_Status_77child      CONSTANT NUMBER := '100';
  Gv_Status_77master     CONSTANT NUMBER := '101';
  Gv_Status_77merged     CONSTANT NUMBER := '102';
  Gv_Status_Giftcertitem CONSTANT NUMBER := '103';
  Gv_Status_Giftcarditem CONSTANT NUMBER := '104';

  Gv_Status_Ignore CONSTANT NUMBER := '-1';
  Gv_Process       CONSTANT VARCHAR2(256) := Lower(TRIM('stage_Tlog05'));
  Gv_Process_Id NUMBER := 0;
  Gv_GiftCard_Product_Id NUMBER := 0;

  TYPE Rcursor IS REF CURSOR;

  FUNCTION Get_Parthighvalue(p_Table_Name     VARCHAR2,
                             p_Partition_Name VARCHAR2) RETURN VARCHAR2;

  PROCEDURE Stage_Tlog05(p_Filename   VARCHAR2 DEFAULT NULL,
                         p_Test_Count NUMBER DEFAULT 0,
                         Retval       IN OUT Rcursor);

  PROCEDURE Hist_Tlog05(p_Dummy VARCHAR2, p_Process_Date Date,
                        Retval  IN OUT Rcursor);

  PROCEDURE Hist_Tlog05_ByJobNumber(p_Dummy VARCHAR2, p_JobNumber NUMBER,
                        Retval  IN OUT Rcursor);

  PROCEDURE Clear_Stage(p_Dummy VARCHAR2,
                        Retval  IN OUT Rcursor);

  PROCEDURE Stage_unprocessed_Tlog05 (p_Dummy VARCHAR2,
                                      Retval       IN OUT Rcursor);
  PROCEDURE Re_ProcessStagedTlog(p_Dummy VARCHAR2,
                                 Retval  IN OUT Rcursor);
                                 
FUNCTION is_duplicated(p_txnheaderid  number) RETURN number;                                 

  PROCEDURE log_unprocessed_Tlog05 (p_JobNumber NUMBER DEFAULT -1); /* this called by hist_tlog, so dont run manually*/

  CURSOR Get_Discounts(p_Header_Id  VARCHAR2,
                       p_Source     VARCHAR2,
                       p_Process_Id NUMBER) IS
    SELECT Rowkey,
           Ipcode,
           Processid,
           Txndiscountid,
           Txnheaderid,
           Txndate,
           Txndetailid,
           Discounttype,
           Discountamount,
           Txnchannel,
           Offercode,
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid,
           Fileid
      FROM Lw_Txndetaildiscount_Stage5
     WHERE Txnheaderid = p_Header_Id
       AND p_Source IN ('stage', 'hist')
       AND Processid = p_Process_Id
    UNION ALL
    SELECT a_Rowkey,
           a_Ipcode,
           a_Processid,
           a_Txndiscountid,
           a_Txnheaderid,
           a_Txndate,
           a_Txndetailid,
           a_Discounttype,
           a_Discountamount,
           a_Txnchannel,
           a_Offercode,
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid,
           a_Fileid
      FROM Ats_Historytxndetaildiscount
     WHERE a_Txnheaderid = p_Header_Id
       AND p_Source = 'hist'
       AND a_Processid = p_Process_Id;

  CURSOR Get_Rewards(p_Header_Id  VARCHAR2,
                     p_Source     VARCHAR2,
                     p_Process_Id NUMBER) IS
    SELECT Rowkey,
           Ipcode,
           Txnheaderid,
           Txndate,
           Txndetailid,
           Programid,
           Certificateredeemtype,
           Certificatecode,
           Certificatediscountamount,
           Txnrewardredeemid,
           Processid,
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid
      FROM Lw_Txnrewardredeem_Stage5
     WHERE Txnheaderid = p_Header_Id
       AND p_Source IN ('stage', 'hist')
       AND Processid = p_Process_Id
    UNION ALL
    SELECT a_Rowkey,
           a_Ipcode,
           a_Txnheaderid,
           a_Txndate,
           a_Txndetailid,
           a_Programid,
           a_Certificateredeemtype,
           a_Certificatecode,
           a_Certificatediscountamount,
           a_Txnrewardredeemid,
           a_Processid,
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid
      FROM Ats_Historytxnrewardredeem
     WHERE a_Txnheaderid = p_Header_Id
       AND p_Source = 'hist'
       AND a_Processid = p_Process_Id;

  CURSOR Get_Tenders(p_Header_Id  VARCHAR2,
                     p_Source     VARCHAR2,
                     p_Process_Id NUMBER) IS
    SELECT Rowkey,
           Ipcode,
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
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid,
           Fileid
      FROM Lw_Txntender_Stage5
     WHERE Txnheaderid = p_Header_Id
       AND p_Source IN ('stage', 'hist')
       AND Processid = p_Process_Id
    UNION ALL
    SELECT a_Rowkey,
           a_Ipcode,
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
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid,
           a_Fileid
      FROM Ats_Historytxntender
     WHERE a_Txnheaderid = p_Header_Id
       AND p_Source = 'hist'
       AND a_Processid = p_Process_Id;

  CURSOR Get_Details(p_Header_Id  VARCHAR2,
                     p_Source     VARCHAR2,
                     p_Process_Id NUMBER) IS
    SELECT Rowkey,
           Vckey,
           Ipcode,
           Dtlquantity,
           Dtldiscountamount,
           Dtlclearanceitem,
           Dtldatemodified,
           Reconcilestatus,
           Txnheaderid,
           Txndetailid,
           Brandid,
           Fileid,
           Processid,
           Filelineitem,
           Cardid,
           Creditcardid,
           Txnloyaltyid,
           Txnmaskid,
           Txnnumber,
           Txndate,
           Txndatemodified,
           Txnregisternumber,
           Txnstoreid,
           Storenumber,
           Txntypeid,
           Txnamount,
           Txnqualpurchaseamt,
           Txndiscountamount,
           Txnemailaddress,
           Txnphonenumber,
           Txnemployeeid,
           Txnchannelid,
           Txnoriginaltxnrowkey,
           Txncreditsused,
           Dtlitemlinenbr,
           Dtlproductid,
           Dtltypeid,
           Dtlactionid,
           Dtlretailamount,
           Dtlsaleamount,
           Dtlclasscode,
           Shipdate,
           TRIM(Ordernumber) AS Ordernumber,
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid,
           Skunumber
      FROM Lw_Txndetail_Stage5
     WHERE Txnheaderid = p_Header_Id
       AND p_Source IN ('stage', 'hist')
       AND Processid = p_Process_Id
    UNION ALL
    SELECT a_Rowkey,
           a_Vckey,
           a_Ipcode,
           a_Dtlquantity,
           a_Dtldiscountamount,
           a_Dtlclearanceitem,
           a_Dtldatemodified,
           a_Reconcilestatus,
           a_Txnheaderid,
           a_Txndetailid,
           a_Brandid,
           a_Fileid,
           a_Processid,
           a_Filelineitem,
           a_Cardid,
           a_Creditcardid,
           a_Txnloyaltyid,
           a_Txnmaskid,
           a_Txnnumber,
           a_Txndate,
           a_Txndatemodified,
           a_Txnregisternumber,
           a_Txnstoreid,
           a_Storenumber,
           a_Txntype,
           a_Txnamount,
           a_Txnqualpurchaseamt,
           a_Txndiscountamount,
           a_Txnemailaddress,
           a_Txnphonenumber,
           a_Txnemployeeid,
           a_Txnchannelid,
           a_Txnoriginaltxnrowkey,
           a_Txncreditsused,
           a_Dtlitemlinenbr,
           a_Dtlproductid,
           a_Dtltypeid,
           a_Dtlactionid,
           a_Dtlretailamount,
           a_Dtlsaleamount,
           a_Dtlclasscode,
           a_Shipdate,
           TRIM(a_Ordernumber) AS a_Ordernumber,
           Statuscode,
           Createdate,
           Updatedate,
           Lastdmlid,
           a_Skunumber
      FROM Ats_Historytxndetail
     WHERE a_Txnheaderid = p_Header_Id
       AND p_Source = 'hist'
       AND a_Processid = p_Process_Id;
END Stage_Tlog5;
/
CREATE OR REPLACE PACKAGE BODY Stage_Tlog5 IS
   gv_Detail_Tbl     Type_Tlog05_Stg_Detail_Tbl := Type_Tlog05_Stg_Detail_Tbl();
   gv_Tender_Tbl     Type_Tlog05_Stg_Tender_Tbl := Type_Tlog05_Stg_Tender_Tbl();
   gv_Discount_Tbl   Type_Tlog05_Stg_Discount_Tbl := Type_Tlog05_Stg_Discount_Tbl();
   gv_Reward_Tbl     Type_Tlog05_Stg_Reward_Tbl  := Type_Tlog05_Stg_Reward_Tbl();
   
     
-- AEO-2090 begin

  
FUNCTION is_duplicated(p_txnheaderid  number) RETURN number IS
    v_return number := 0;
   
    v_processid number := 0;
    v_exist number := 0;
    
  BEGIN
      
  select count(*)
  into v_exist 
  from table(gv_Detail_Tbl)  p
  where p.txnheaderid = p_txnheaderid and p.processid = 12;
  
  if ( v_exist >= 1) then
    v_processid := 12;
  end if;
    
  
  if v_processid = 12 then 
    v_return := 1 ;
  end if;

   
   
   /*FOR i IN 1..gv_Detail_Tbl.count
     loop
        if ( gv_Detail_Tbl(i).rowkey = p_rowkey) then
             if ( gv_Detail_Tbl(i).processid = 12) then
               v_return := 1;
             end if;  
             exit;
        end if;  
     end loop;*/
   
    RETURN v_return ;
  EXCEPTION
    WHEN OTHERS THEN
    reTURN v_return;
  END is_duplicated;
    
 procedure write_tlog_dup_exception  is
  PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
  BEGIN
   -- Iterate the list that contains the details of the txn loaded
   -- from the tlog file
   -- write a new row in exceptions table for all of them that have ar processid = 12
   
    FORALL i IN 1..gv_Detail_Tbl.count
    
    ---use merge instead of insert to avoid the use of an if sentence
    -- and be able to use FORALL instead of FOR loop
    MERGE INTO bp_ae.lw_txndetail_exceptions  exc
    USING ((SELECT Gv_Detail_Tbl(i).Rowkey as rowkey
             ,Gv_Detail_Tbl(i).Ipcode as ipcode
             ,Gv_Detail_Tbl(i).Vckey as vckey
             ,Gv_Detail_Tbl(i).Dtlquantity as Dtlquantity
             ,Gv_Detail_Tbl(i).Dtldiscountamount as Dtldiscountamount
             ,Gv_Detail_Tbl(i).Dtlclearanceitem  as  Dtlclearanceitem
             ,Gv_Detail_Tbl(i).Dtldatemodified   as  Dtldatemodified
             ,Gv_Detail_Tbl(i).Reconcilestatus   as  Reconcilestatus
             ,Gv_Detail_Tbl(i).Txnheaderid       as  Txnheaderid
             ,Gv_Detail_Tbl(i).Txndetailid       as  Txndetailid
             ,Gv_Detail_Tbl(i).Brandid           as  Brandid
             ,Gv_Detail_Tbl(i).Fileid            as  Fileid
             ,Gv_Detail_Tbl(i).Processid         as  Processid
             ,Gv_Detail_Tbl(i).Filelineitem      as  Filelineitem
             ,Gv_Detail_Tbl(i).Cardid            as  Cardid
             ,Gv_Detail_Tbl(i).Creditcardid      as  Creditcardid
             ,Gv_Detail_Tbl(i).Txnloyaltyid      as  Txnloyaltyid
             ,Gv_Detail_Tbl(i).Txnmaskid         as  Txnmaskid
             ,Gv_Detail_Tbl(i).Txnnumber         as  Txnnumber
             ,Gv_Detail_Tbl(i).Txndate           as  Txndate
             ,Gv_Detail_Tbl(i).Txndatemodified   as  Txndatemodified
             ,Gv_Detail_Tbl(i).Txnregisternumber as  Txnregisternumber
             ,Gv_Detail_Tbl(i).Txnstoreid        as  Txnstoreid
             ,Gv_Detail_Tbl(i).Txntypeid         as  Txntypeid
             ,Gv_Detail_Tbl(i).Txnamount         as  Txnamount
             ,Gv_Detail_Tbl(i).Txndiscountamount  as Txndiscountamount
             ,Gv_Detail_Tbl(i).Txnqualpurchaseamt as Txnqualpurchaseamt
             ,Gv_Detail_Tbl(i).Txnemailaddress    as Txnemailaddress
             ,Gv_Detail_Tbl(i).Txnphonenumber     as Txnphonenumber
             ,Gv_Detail_Tbl(i).Txnemployeeid      as Txnemployeeid
             ,Gv_Detail_Tbl(i).Txnchannelid       as Txnchannelid
             ,Gv_Detail_Tbl(i).Txnoriginaltxnrowkey as Txnoriginaltxnrowkey
             ,Gv_Detail_Tbl(i).Txncreditsused      as Txncreditsused
             ,Gv_Detail_Tbl(i).Dtlitemlinenbr      as Dtlitemlinenbr
             ,Gv_Detail_Tbl(i).Dtlproductid        as Dtlproductid
             ,Gv_Detail_Tbl(i).Dtltypeid           as Dtltypeid
             ,Gv_Detail_Tbl(i).Dtlactionid         as Dtlactionid
             ,Gv_Detail_Tbl(i).Dtlretailamount     as Dtlretailamount
             ,Gv_Detail_Tbl(i).Dtlsaleamount       as Dtlsaleamount
             ,Gv_Detail_Tbl(i).Dtlclasscode        as Dtlclasscode
             ,Gv_Detail_Tbl(i).Errormessage        as Errormessage
             ,Gv_Detail_Tbl(i).Shipdate            as Shipdate
             ,Gv_Detail_Tbl(i).Ordernumber         as Ordernumber
             ,Gv_Detail_Tbl(i).Skunumber           as Skunumber
             ,Gv_Detail_Tbl(i).Tenderamount        as Tenderamount
             ,Gv_Detail_Tbl(i).Storenumber         as Storenumber
             ,Gv_Detail_Tbl(i).Statuscode          as Statuscode
             ,Gv_Detail_Tbl(i).Createdate          as Createdate
             ,Gv_Detail_Tbl(i).Updatedate          as Updatedate
             ,Gv_Detail_Tbl(i).Lastdmlid           as Lastdmlid
             ,Gv_Detail_Tbl(i).Nonmember           as Nonmember
             ,Gv_Detail_Tbl(i).TxnOriginalStoreId  as TxnOriginalStoreId
             ,Gv_Detail_Tbl(i).TxnOriginalTxnDate  as TxnOriginalTxnDate
             ,Gv_Detail_Tbl(i).TxnOriginalTxnNumber as TxnOriginalTxnNumber
             ,Gv_Detail_Tbl(i).CURRENCY_Rate       as CURRENCY_Rate                        
             ,Gv_Detail_Tbl(i).CURRENCY_CODE       as CURRENCY_CODE                        
             ,Gv_Detail_Tbl(i).dtlsaleamount_org   as dtlsaleamount_org                    
             ,Gv_Detail_Tbl(i).AECCMultiplier      as AECCMultiplier                       
             ,Gv_Detail_Tbl(i).OriginalOrderNumber as OriginalOrderNumber                  
             ,Gv_Detail_Tbl(i).CashierNumber as CashierNumber 
             FROM dual 
             WHERE Gv_Detail_Tbl(i).Processid  = 12 )  ) t
    ON (exc.rowkey = t.rowkey  )
    WHEN NOT MATCHED THEN
           INSERT         
             (Rowkey
             ,Ipcode
             ,Vckey
             ,Dtlquantity
             ,Dtldiscountamount
             ,Dtlclearanceitem
             ,Dtldatemodified
             ,Reconcilestatus
             ,Txnheaderid
             ,Txndetailid
             ,Brandid
             ,Fileid
             ,Processid
             ,Filelineitem
             ,Cardid
             ,Creditcardid
             ,Txnloyaltyid
             ,Txnmaskid
             ,Txnnumber
             ,Txndate
             ,Txndatemodified
             ,Txnregisternumber
             ,Txnstoreid
             ,Txntypeid
             ,Txnamount
             ,Txndiscountamount
             ,Txnqualpurchaseamt
             ,Txnemailaddress
             ,Txnphonenumber
             ,Txnemployeeid
             ,Txnchannelid
             ,Txnoriginaltxnrowkey
             ,Txncreditsused
             ,Dtlitemlinenbr
             ,Dtlproductid
             ,Dtltypeid
             ,Dtlactionid
             ,Dtlretailamount
             ,Dtlsaleamount
             ,Dtlclasscode
             ,Errormessage
             ,Shipdate
             ,Ordernumber
             ,Skunumber
             ,Tenderamount
             ,Storenumber
             ,Statuscode
             ,Createdate
             ,Updatedate
             ,Lastdmlid
             ,Nonmember
             ,TxnOriginalStoreId
             ,TxnOriginalTxnDate
             ,TxnOriginalTxnNumber
             ,CurrencyRate                   
             ,CurrencyCode                   
             ,DTLSALEAmount_org              
             ,AECCMultiplier                   
             ,OriginalOrderNumber              
             ,CashierNumber                  
             )
           VALUES
             (t.Rowkey
             ,t.Ipcode
             ,t.Vckey
             ,t.Dtlquantity
             ,t.Dtldiscountamount
             ,t.Dtlclearanceitem
             ,t.Dtldatemodified
             ,t.Reconcilestatus
             ,t.Txnheaderid
             ,t.Txndetailid
             ,t.Brandid
             ,t.Fileid
             ,t.Processid
             ,t.Filelineitem
             ,t.Cardid
             ,t.Creditcardid
             ,t.Txnloyaltyid
             ,t.Txnmaskid
             ,t.Txnnumber
             ,t.Txndate
             ,t.Txndatemodified
             ,t.Txnregisternumber
             ,t.Txnstoreid
             ,t.Txntypeid
             ,t.Txnamount
             ,t.Txndiscountamount
             ,t.Txnqualpurchaseamt
             ,t.Txnemailaddress
             ,t.Txnphonenumber
             ,t.Txnemployeeid
             ,t.Txnchannelid
             ,t.Txnoriginaltxnrowkey
             ,t.Txncreditsused
             ,t.Dtlitemlinenbr
             ,t.Dtlproductid
             ,t.Dtltypeid
             ,t.Dtlactionid
             ,t.Dtlretailamount
             ,t.Dtlsaleamount
             ,t.Dtlclasscode
             ,t.Errormessage
             ,t.Shipdate
             ,upper(t.Ordernumber)
             ,t.Skunumber
             ,t.Tenderamount
             ,t.Storenumber
             ,t.Statuscode
             ,t.Createdate
             ,t.Updatedate
             ,t.Lastdmlid
             ,t.Nonmember
             ,t.TxnOriginalStoreId
             ,t.TxnOriginalTxnDate
             ,t.TxnOriginalTxnNumber
             ,t.CURRENCY_Rate                       
             ,t.CURRENCY_CODE                       
             ,t.dtlsaleamount_org                   
             ,t.AECCMultiplier                      
             ,t.OriginalOrderNumber                 
             ,t.CashierNumber
             );

     commit;
    end;
 
    -- AEO-2090 end
   
  /********************************************************************
  ********************************************************************
  **************************************
  ******************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/
  PROCEDURE Clear_Infile(p_Tablename IN VARCHAR2) IS
    Lv_Filehandle Utl_File.File_Type;
    Lv_Path       VARCHAR2(50) DEFAULT 'USER_EXPORT';
    Lv_Filename   VARCHAR2(512);
    /*
       used to delete content of flatfile
       i.e. the external table's log file.
    */
  BEGIN
    SELECT Location, Directory_Name
      INTO Lv_Filename, Lv_Path
      FROM User_External_Locations t
     WHERE Upper(t.Table_Name) = Upper(p_Tablename);

    Lv_Filehandle := Utl_File.Fopen(Lv_Path, Lv_Filename, 'W', 32000);

    --utl_file.put_line( lv_fileHandle, lv_colString );
    --utl_file.new_line(lv_filehandle);
    Utl_File.Fclose(Lv_Filehandle);

  END Clear_Infile;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/
  FUNCTION Valid_Number(p_Nbr_String VARCHAR2) RETURN NUMBER IS
    v_Nbr NUMBER;
    /*
       string to number conversion, returns null if non numeric data found.
    */
  BEGIN
    v_Nbr := TRIM(To_Number(p_Nbr_String));
    RETURN v_Nbr;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN NULL;
  END Valid_Number;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/
  FUNCTION Valid_Date(p_Date_String VARCHAR2,
                      p_Date_Format VARCHAR2) RETURN DATE IS
    v_Date DATE;
    /*
       string to date conversion, returns null if in unexpected format.
    */
  BEGIN
    IF TRIM(p_Date_String) IS NULL OR TRIM(p_Date_Format) IS NULL
    THEN
      RETURN NULL;
    END IF;
    v_Date := To_Date(p_Date_String, p_Date_Format);
    RETURN v_Date;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN NULL;
  END Valid_Date;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Get_Tlog05_00header(p_Rec IN VARCHAR2) RETURN Type_Tlog05_00header IS
    v_Return_Rec Type_Tlog05_00header;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    IF p_Rec LIKE '00%'
    THEN
      v_Return_Rec := Type_Tlog05_00header(Substr(p_Rec, 001, 2),
                                           Substr(p_Rec, 003, 5),
                                           Substr(p_Rec, 008, 3),
                                           Substr(p_Rec, 011, 9),
                                           Substr(p_Rec, 020, 6),
                                           Substr(p_Rec, 026, 6),
                                           Substr(p_Rec, 032, 4),
                                           Substr(p_Rec, 036, 1),
                                           Substr(p_Rec, 037, 2),
                                           Substr(p_Rec, 039, 1),
                                           Substr(p_Rec, 040, 1),
                                           Substr(p_Rec, 041, 1),
                                           Substr(p_Rec, 042, 10),
                                           Substr(p_Rec, 052, 11),
                                           Ltrim(TRIM(Substr(p_Rec, 063, 9)),
                                                 '0'),
                                           Substr(p_Rec, 072, 6),
                                           Substr(p_Rec, 078, 6),
                                           Substr(p_Rec, 084, 6),
                                           Substr(p_Rec, 090, 4),
                                           Substr(p_Rec, 094, 3),
                                           Substr(p_Rec, 097, 5));

      /*  tlog record spec
      STRING_TYPE                     VARCHAR2(2)  \*001 - 002*\
      STORE_NUMBER                    VARCHAR2(5)  \*003 - 007*\
      REGISTER_NUMBER                 VARCHAR2(3)  \*008 - 010*\
      CASHIER_NUMBER                  VARCHAR2(9)  \*011 - 019*\
      TRANSACTION_NUMBER              VARCHAR2(6)  \*020 - 025*\
      TRANSACTION_DATE                VARCHAR2(6)  \*026 - 031     MMDDYY*\
      TRANSACTION_TIME                VARCHAR2(4)  \*032 - 035     HHMM*\
      TRANSACTION_MODE                VARCHAR2(1)  \*036 - 036*\
      TRANSACTION_TYPE                VARCHAR2(2)  \*037 - 038*\
      TRANSACTION_STATUS              VARCHAR2(1)  \*039 - 039*\
      TAX_STATUS                      VARCHAR2(1)  \*040 - 040*\
      CANCEL_STATUS                   VARCHAR2(1)  \*041 - 041*\
      LAYAWAY_NUMBER                  VARCHAR2(10) \*042 - 051*\
      PREFERRED_CUSTOMER_NUMBER       VARCHAR2(11) \*052 - 062*\
      EMPLOYEE_NUMBER                 VARCHAR2(9)  \*063 - 071*\
      AR_CUSTOMER_NUMBER              VARCHAR2(6)  \*072 - 077*\
      ORIGINAL_TRANSACTION_NUMBER     VARCHAR2(6)  \*078 - 083*\
      RETRIEVED_TRANSACTION_NUMBER    VARCHAR2(6)  \*084 - 089*\
      AUTHORIZATION_CODE              VARCHAR2(4)  \*090 - 093*\
      USER_DATA                       VARCHAR2(3)  \*094 - 096*\
      NUMBER_OF_STRINGS               VARCHAR2(5)  \*097 - 101*\
      */

    END IF;
    RETURN v_Return_Rec;
  END Get_Tlog05_00header;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

    FUNCTION Get_Tlog05_01item(p_Rec IN VARCHAR2) RETURN Type_Tlog05_01item IS
    v_Return_Rec    Type_Tlog05_01item;
    v_Actual_Amount NUMBER;
    v_Retail_Amount NUMBER;
    v_Quantity      NUMBER;
    v_77Kids_ClassCode NUMBER;
    v_77Kids_ClassCodeName VARCHAR(255);
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Actual_Amount := To_Number(Substr(p_Rec, 035, 10)) * .01;
    --v_retail_amount := to_number(SUBSTR(p_rec,004,10)) * .01;
    v_Quantity := To_Number(Substr(p_Rec, 026, 9)) * .001;
    v_77Kids_ClassCodeName :=  TRIM(Substr(p_Rec, 122, 4));

-- PI 31647 - 77kids class codes reassigned to AE - points not added - Start
/*    v_77Kids_ClassCode:= 0;
    SELECT COUNT(1) INTO v_77Kids_ClassCode FROM (SELECT DISTINCT classcode FROM ae_temp77kids_classcodes WHERE classcode = v_77Kids_ClassCodeName);

    IF  v_77Kids_ClassCode > 0
    THEN
    v_Actual_Amount :=0;
    END IF;
*/
-- PI 31647 - 77kids class codes reassigned to AE - points not added - Start


    IF p_Rec LIKE '01%'
    THEN
      v_Return_Rec := Type_Tlog05_01item(Substr(p_Rec, 001, 2),
                                         Substr(p_Rec, 003, 1),
                                         Substr(p_Rec, 004, 9),
                                         Ltrim(TRIM(Substr(p_Rec, 013, 12)),
                                               '0') /* sku */,
                                         Substr(p_Rec, 025, 1),
                                         v_Quantity /*SUBSTR(p_rec,026,9)*/,
                                         v_Actual_Amount /*SUBSTR(p_rec,035,10)*/,
                                         Substr(p_Rec, 045, 10),
                                         Substr(p_Rec, 055, 10),
                                         Substr(p_Rec, 065, 10),
                                         Substr(p_Rec, 075, 10),
                                         Substr(p_Rec, 085, 10),
                                         Substr(p_Rec, 095, 1),
                                         Substr(p_Rec, 096, 5),
                                         Substr(p_Rec, 101, 5),
                                         Substr(p_Rec, 106, 10),
                                         Substr(p_Rec, 116, 6),
                                         v_77Kids_ClassCodeName,
                                         Substr(p_Rec, 126, 4),
                                         Substr(p_Rec, 130, 1),
                                         Substr(p_Rec, 131, 1),
                                         Substr(p_Rec, 132, 1),
                                         Substr(p_Rec, 133, 1),
                                         Substr(p_Rec, 134, 1),
                                         Substr(p_Rec, 135, 1),
                                         Substr(p_Rec, 136, 1),
                                         Substr(p_Rec, 137, 3),
                                         Substr(p_Rec, 140, 1),
                                         Substr(p_Rec, 141, 5),
                                         Substr(p_Rec, 146, 5),
                                         Substr(p_Rec, 151, 1),
                                         Substr(p_Rec, 152, 1),
                                         Substr(p_Rec, 153, 1),
                                         Substr(p_Rec, 154, 3),
                                         Substr(p_Rec, 157, 3),
                                         Substr(p_Rec, 160, 3),
                                         Substr(p_Rec, 236, 3));
      /*  tlog record spec
      STRING_TYPE                     varchar2(2)   \*001 - 002*\
      MODE_INDICATOR                  varchar2(1)   \*003 - 003*\
      SALESPERSON_NUMBER              varchar2(9)   \*004 - 012*\
      SKU_CLASS_NUMBER                varchar2(12)  \*013 - 024*\
      SKU_CLASS_INDICATOR             varchar2(1)   \*025 - 025*\
      QUANTITY                        varchar2(9)   \*026 - 034       9(6)V999*\
      FINAL_SELLING_PRICE             varchar2(10)  \*035 - 044       9(8)V99*\
      EXTENDED_TOTAL                  varchar2(10)  \*045 - 054       9(8)V99*\
      ORIGINAL_PRICE                  varchar2(10)  \*055 - 064       9(8)V99*\
      PLU_PRICE                       varchar2(10)  \*065 - 074       9(8)V99*\
      PRE_DISCOUNT_PRICE              varchar2(10)  \*075 - 084       9(8)V99*\
      PRICE_ENTERED                   varchar2(10)  \*085 - 094       9(8)V99*\
      TAXABLE_STATUS                  varchar2(1)   \*095 - 095*\
      TAX_OVERRIDE_CODE               varchar2(5)   \*096 - 100*\
      EFFECTIVE_TAX_RATE              varchar2(5)   \*101 - 105       99V999*\
      AMOUNT_TAXABLE                  varchar2(10)  \*106 - 115       9(8)V99*\
      ORIGINAL_TRANSACTION_NUMBER     varchar2(6)   \*116 - 121*\
      CLASS_NUMBER                    varchar2(4)   \*122 - 125*\
      DEPARTMENT_NUMBER               varchar2(4)   \*126 - 129*\
      MAIN_REASON_CODE                varchar2(1)   \*130 - 130*\
      SUPPLEMENTAL_REASON_CODE        varchar2(1)   \*131 - 131*\
      NOT_ON_FILE_INDICATOR           varchar2(1)   \*132 - 132*\
      SPLIT_TRANSACTION_INDICATOR     varchar2(1)   \*133 - 133*\
      PRICE_OVERRIDE_INDICATOR        varchar2(1)   \*134 - 134*\
      ON_DEAL_INDICATOR               varchar2(1)   \*135 - 135*\
      IN_PACKAGE_INDICATOR            varchar2(1)   \*136 - 136*\
      BOTTLE_DEPOSIT                  varchar2(3)   \*137 - 139       9V99*\
      INCENTIVE_TYPE_INDICATOR        varchar2(1)   \*140 - 140*\
      INCENTIVE_DOLLARS               varchar2(5)   \*141 - 145       999V99*\
      INCENTIVE_POINTS                varchar2(5)   \*146 - 150*\
      LUXURY_TAX_BREAK_INDICATOR      varchar2(1)   \*151 - 151       1 - 9*\
      FLAT_RATE_TAX_BREAK_INDICATOR   varchar2(1)   \*152 - 152       1 - 9*\
      DEAL_RECORD_NUMBER              varchar2(1)   \*153 - 153*\
      COLOR                           varchar2(3)   \*154 - 156*\
      SIZE_                           varchar2(3)   \*157 - 159*\
      WIDTH                           varchar2(3)   \*160 ? 162*\
      MARKDOWN_FLAG                   varchar2(3)   \*236 - 236*\
      */

    END IF;
    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
    /********************************************************************
  AEO-820 changes begin here----------------------------------------SCJ
  ********************************************************************
  ********************************************************************/

    FUNCTION Get_Tlog05_99item(p_Rec IN VARCHAR2) RETURN Type_Tlog05_99item IS
    v_Return_Rec    Type_Tlog05_99item;
    v_Actual_Amount NUMBER;
    v_Retail_Amount NUMBER;
    v_Quantity      NUMBER;
    v_77Kids_ClassCode NUMBER;
    v_77Kids_ClassCodeName VARCHAR(255);
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Actual_Amount := To_Number(Substr(p_Rec, 035, 10)) * .01;
    --v_retail_amount := to_number(SUBSTR(p_rec,004,10)) * .01;
    v_Quantity := To_Number(Substr(p_Rec, 026, 9)) * .001;
    v_77Kids_ClassCodeName :=  TRIM(Substr(p_Rec, 122, 4));

    IF p_Rec LIKE '99%'
    THEN
      v_Return_Rec := Type_Tlog05_99item(Substr(p_Rec, 001, 2),
                                         Substr(p_Rec, 003, 1),
                                         Substr(p_Rec, 004, 9),
                                         Ltrim(TRIM(Substr(p_Rec, 013, 12)),
                                               '0') /* sku */,
                                         Substr(p_Rec, 025, 1),
                                         v_Quantity /*SUBSTR(p_rec,026,9)*/,
                                         v_Actual_Amount /*SUBSTR(p_rec,035,10)*/,
                                         Substr(p_Rec, 045, 10),
                                         Substr(p_Rec, 055, 10),
                                         Substr(p_Rec, 065, 10),
                                         Substr(p_Rec, 075, 10),
                                         Substr(p_Rec, 085, 10),
                                         Substr(p_Rec, 095, 1),
                                         Substr(p_Rec, 096, 5),
                                         Substr(p_Rec, 101, 5),
                                         Substr(p_Rec, 106, 10),
                                         Substr(p_Rec, 116, 6),
                                         v_77Kids_ClassCodeName,
                                         Substr(p_Rec, 126, 4),
                                         Substr(p_Rec, 130, 1),
                                         Substr(p_Rec, 131, 1),
                                         Substr(p_Rec, 132, 1),
                                         Substr(p_Rec, 133, 1),
                                         Substr(p_Rec, 134, 1),
                                         Substr(p_Rec, 135, 1),
                                         Substr(p_Rec, 136, 1),
                                         Substr(p_Rec, 137, 3),
                                         Substr(p_Rec, 140, 1),
                                         Substr(p_Rec, 141, 5),
                                         Substr(p_Rec, 146, 5),
                                         Substr(p_Rec, 151, 1),
                                         Substr(p_Rec, 152, 1),
                                         Substr(p_Rec, 153, 1),
                                         Substr(p_Rec, 154, 3),
                                         Substr(p_Rec, 157, 3),
                                         Substr(p_Rec, 160, 3),
                                         Substr(p_Rec, 236, 3));
      /*  tlog record spec
      STRING_TYPE                     varchar2(2)   \*001 - 002*\
      MODE_INDICATOR                  varchar2(1)   \*003 - 003*\
      SALESPERSON_NUMBER              varchar2(9)   \*004 - 012*\
      SKU_CLASS_NUMBER                varchar2(12)  \*013 - 024*\
      SKU_CLASS_INDICATOR             varchar2(1)   \*025 - 025*\
      QUANTITY                        varchar2(9)   \*026 - 034       9(6)V999*\
      FINAL_SELLING_PRICE             varchar2(10)  \*035 - 044       9(8)V99*\
      EXTENDED_TOTAL                  varchar2(10)  \*045 - 054       9(8)V99*\
      ORIGINAL_PRICE                  varchar2(10)  \*055 - 064       9(8)V99*\
      PLU_PRICE                       varchar2(10)  \*065 - 074       9(8)V99*\
      PRE_DISCOUNT_PRICE              varchar2(10)  \*075 - 084       9(8)V99*\
      PRICE_ENTERED                   varchar2(10)  \*085 - 094       9(8)V99*\
      TAXABLE_STATUS                  varchar2(1)   \*095 - 095*\
      TAX_OVERRIDE_CODE               varchar2(5)   \*096 - 100*\
      EFFECTIVE_TAX_RATE              varchar2(5)   \*101 - 105       99V999*\
      AMOUNT_TAXABLE                  varchar2(10)  \*106 - 115       9(8)V99*\
      ORIGINAL_TRANSACTION_NUMBER     varchar2(6)   \*116 - 121*\
      CLASS_NUMBER                    varchar2(4)   \*122 - 125*\
      DEPARTMENT_NUMBER               varchar2(4)   \*126 - 129*\
      MAIN_REASON_CODE                varchar2(1)   \*130 - 130*\
      SUPPLEMENTAL_REASON_CODE        varchar2(1)   \*131 - 131*\
      NOT_ON_FILE_INDICATOR           varchar2(1)   \*132 - 132*\
      SPLIT_TRANSACTION_INDICATOR     varchar2(1)   \*133 - 133*\
      PRICE_OVERRIDE_INDICATOR        varchar2(1)   \*134 - 134*\
      ON_DEAL_INDICATOR               varchar2(1)   \*135 - 135*\
      IN_PACKAGE_INDICATOR            varchar2(1)   \*136 - 136*\
      BOTTLE_DEPOSIT                  varchar2(3)   \*137 - 139       9V99*\
      INCENTIVE_TYPE_INDICATOR        varchar2(1)   \*140 - 140*\
      INCENTIVE_DOLLARS               varchar2(5)   \*141 - 145       999V99*\
      INCENTIVE_POINTS                varchar2(5)   \*146 - 150*\
      LUXURY_TAX_BREAK_INDICATOR      varchar2(1)   \*151 - 151       1 - 9*\
      FLAT_RATE_TAX_BREAK_INDICATOR   varchar2(1)   \*152 - 152       1 - 9*\
      DEAL_RECORD_NUMBER              varchar2(1)   \*153 - 153*\
      COLOR                           varchar2(3)   \*154 - 156*\
      SIZE_                           varchar2(3)   \*157 - 159*\
      WIDTH                           varchar2(3)   \*160 ? 162*\
      MARKDOWN_FLAG                   varchar2(3)   \*236 - 236*\
      */

    END IF;
    RETURN v_Return_Rec;
  END;

  /********************************************************************
  AEO-820 changes end here----------------------------------------SCJ
  ********************************************************************
  ********************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_02subtotal(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_02subtotal IS
    v_Return_Rec Type_Tlog05_02subtotal;
    v_Amount     NUMBER;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Amount     := To_Number(Substr(p_Rec, 004, 10)) * .01;
    v_Return_Rec := Type_Tlog05_02subtotal(Substr(p_Rec, 001, 2),
                                           Substr(p_Rec, 003, 1),
                                           v_Amount);
    /*  tlog record spec
    (STRING_TYPE               varchar2(2)   \*001 - 002*\
    ,MODE_INDICATOR            VARCHAR2(1)   \*003 - 003*\
    ,AMOUNT                    VARCHAR2(10)  \*004 - 013       9(8)V99*\
    )
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_03salestax(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_03salestax IS
    v_Return_Rec Type_Tlog05_03salestax;
    v_Amount     NUMBER;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Amount     := To_Number(Substr(p_Rec, 005, 10)) * .01;
    v_Return_Rec := Type_Tlog05_03salestax(Substr(p_Rec, 001, 1),
                                           Substr(p_Rec, 003, 1),
                                           Substr(p_Rec, 004, 1),
                                           v_Amount);

    /*  tlog record spec
    STRING_TYPE             VARCHAR2(2)   \*001 - 002*\
    MODE_INDICATOR          VARCHAR2(1)   \*003 - 003*\
    TAX_TYPE_INDICATOR      VARCHAR2(1)   \*004 - 004*\
    AMOUNT                  VARCHAR2(10)  \*005 - 014    9(8)V99*\
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_04tender(p_Rec IN VARCHAR2) RETURN Type_Tlog05_04tender IS
    v_Return_Rec Type_Tlog05_04tender;
    v_Amount     NUMBER;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Amount     := To_Number(Substr(p_Rec, 7, 10)) * .01;
    v_Return_Rec := Type_Tlog05_04tender(Substr(p_Rec, 001, 2),
                                         Substr(p_Rec, 003, 2),
                                         Substr(p_Rec, 005, 2),
                                         v_Amount);

    /*  tlog record spec
    STRING_TYPE             VARCHAR2(2)   \*001 - 002*\
    TENDER_TYPE             VARCHAR2(2)   \*003 - 004*\
    NUMBER_OF_TENDERS       VARCHAR2(2)   \*005 - 006*\
    AMOUNT                  VARCHAR2(10)  \*007 - 016      9(8)V99*\
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_05giftcert(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_05giftcert IS
    v_Return_Rec Type_Tlog05_05giftcert;
    v_Amount     NUMBER;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Amount     := To_Number(Substr(p_Rec, 18, 10)) * .01;
    v_Return_Rec := Type_Tlog05_05giftcert(Substr(p_Rec, 001, 2),
                                           Substr(p_Rec, 003, 1),
                                           Substr(p_Rec, 014, 9),
                                           Substr(p_Rec, 013, 5),
                                           v_Amount);
    /*  tlog record spec
    STRING_TYPE                        VARCHAR2(2)   \*001 - 002*\
    MODE_INDICATOR                      VARCHAR2(1)   \*003 - 003*\
    GIFT_CERTIFICATE_NUMBER             VARCHAR2(9)   \*004 - 012*\
    ISSUING_STORE_NUMBER                VARCHAR2(5)   \*013 - 017*\
    AMOUNT                              VARCHAR2(10)  \*018 - 027      9(8)V99*\
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_14foundorder(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_14foundorder IS
    v_Return_Rec Type_Tlog05_14foundorder;
    v_Amount     NUMBER;
    v_Taxrate    NUMBER;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Amount     := To_Number(Substr(p_Rec, 027, 10)) * .01;
--    v_Taxrate    := To_Number(Substr(p_Rec, 012, 5)) * .001; /*(Commented to avoid 10/26 tlog issue ,if tax is double digit,then exceeds column length - char2(5)) */
    v_Return_Rec := Type_Tlog05_14foundorder(Substr(p_Rec, 001, 2),
                                             Substr(p_Rec, 003, 1),
                                             Substr(p_Rec, 004, 1),
                                             Substr(p_Rec, 005, 1),
                                             Substr(p_Rec, 006, 1),
                                             Substr(p_Rec, 007, 5),
                                             v_Taxrate,
                                             Substr(p_Rec, 017, 10),
                                             v_Amount);

    /*  tlog record spec
    STRING_TYPE                      VARCHAR2(2)        \*001-002*\
    MODE_INDICATOR                   VARCHAR2(1)        \*003-003*\
    REASON_CODE                      VARCHAR2(1)        \*004-004*\
    FLAT_TAX_RATE_BREAK_INDICATOR    VARCHAR2(1)        \*005-005*\
    TAXABLE_STATUS                   VARCHAR2(1)        \*006-006*\
    TAX_OVERRIDE_CODE                VARCHAR2(5)        \*007-011*\
    EFFECTIVE_TAX_RATE               VARCHAR2(5)        \*012-016 99V999*\
    TAXABLE_AMOUNT                   VARCHAR2(10)       \*017-026*\
    AMOUNT                           VARCHAR2(10)       \*027-036 9(8)V99*\
    */
    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_15ceditcrdcheck(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_15ceditcrdcheck IS
    v_Return_Rec Type_Tlog05_15ceditcrdcheck;
    v_Amount     NUMBER;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Amount     := To_Number(Substr(p_Rec, 050, 10)) * .01;
    v_Return_Rec := Type_Tlog05_15ceditcrdcheck(Substr(p_Rec, 001, 2),
                                                Substr(p_Rec, 003, 2),
                                                Substr(p_Rec, 005, 27),
                                                Substr(p_Rec, 032, 4),
                                                Substr(p_Rec, 036, 4),
                                                Substr(p_Rec, 040, 2),
                                                Substr(p_Rec, 042, 6),
                                                Substr(p_Rec, 048, 1),
                                                Substr(p_Rec, 049, 1),
                                                v_Amount);

    /*  tlog record spec
    STRING_TYPE                 VARCHAR2(2)      \*001 - 002*\
    TENDER_TYPE                 VARCHAR2(2)   \*003 - 004*\
    ACCOUNT_NUMBER              VARCHAR2(27)  \*005 - 031*\
    EXPIRATION_DATE             VARCHAR2(4)   \*032 - 035      NON IF CHECK*\
    START_DATE                  VARCHAR2(4)   \*036 - 039      NON IF CHECK*\
    ISSUE_NUMBER                VARCHAR2(2)   \*040 - 041      NON IF CHECK*\
    AUTHORIZATION_CODE          VARCHAR2(6)   \*042 - 047*\
    ENTRY_METHOD                VARCHAR2(1)   \*048 - 048      NON IF CHECK*\
    AUTHORIZATION_METHOD        VARCHAR2(1)   \*049 - 049*\
    AMOUNT                      VARCHAR2(10)  \*050 - 059      9(8)V99*\
    */

    RETURN v_Return_Rec;
  END;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_18txndiscount(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_18txndiscount IS
    v_Return_Rec Type_Tlog05_18txndiscount;
    /*
      Parses tlog rec and loads into formatted array.
    */
    v_Amount1 NUMBER;
    v_Amount2 NUMBER;
  BEGIN
    v_Amount1    := To_Number(Substr(p_Rec, 005, 10)) * .01;
    v_Amount2    := To_Number(Substr(p_Rec, 015, 10)) * .01;
    v_Return_Rec := Type_Tlog05_18txndiscount(Substr(p_Rec, 001, 2),
                                              Substr(p_Rec, 003, 1),
                                              Substr(p_Rec, 004, 1),
                                              v_Amount1,
                                              v_Amount2);
    /*          tlog record spec
     STRING_TYPE                        VARCHAR2(2)   \*001 - 002*\
    ,MODE_INDICATOR                    VARCHAR2(1)     \*003 - 003*\
    ,REASON_CODE                       VARCHAR2(1)     \*004 - 004*\
    ,DISCOUNT_AMOUNT_TAXABLE           VARCHAR2(10)    \*005 - 014      9(8)V99*\
    ,DISCOUNT_AMOUNT_NONTAXABLE        VARCHAR2(10)    \*015 - 024    9(8)V99*\
    */

    RETURN v_Return_Rec;
  END Get_Tlog05_18txndiscount;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2733origtxn(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2733origtxn IS
    v_Return_Rec Type_Tlog05_2733origtxn;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Return_Rec := Type_Tlog05_2733origtxn(Substr(p_Rec, 001, 2),
                                            Substr(p_Rec, 003, 3),
                                            Substr(p_Rec, 006, 1),
                                            Substr(p_Rec, 007, 2),
                                            Substr(p_Rec, 009, 6),
                                            Substr(p_Rec, 015, 5),
                                            Substr(p_Rec, 020, 6),
                                            Substr(p_Rec, 026, 2));

    /*  tlog record spec
    STRING_TYPE              VARCHAR2(2)   \*001-002*\
    VENDOR_ID                 VARCHAR2(3)   \*003-005*\
    BEFORE_AFTER_INDICATOR   VARCHAR2(1)   \*006-006*\
    USER_IDENTIFICATION      VARCHAR2(2)   \*007-008 (33)*\
    ORIGINAL_TRANSACTION     VARCHAR2(6)   \*009-014*\
    ORIGINAL_STORE             VARCHAR2(5)   \*015-019*\
    ORIGINAL_TRANSATION_DATE VARCHAR2(6)   \*020-025*\
    ORIGINAL_TENDER_TYPE     VARCHAR2(2)   \*026-027*\
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2751discountrsn(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2751discountrsn IS
    v_Return_Rec Type_Tlog05_2751discountrsn;
    /*
      Parses tlog rec and loads into formatted array.
    */
   v_DiscountAmount NUMBER;
  BEGIN
    v_DiscountAmount    := To_Number(NVL(trim(Substr(p_Rec, 076, 10)),0)) * .01;
    /*
    v_Return_Rec := Type_Tlog05_2751discountrsn(Substr(p_Rec, 001, 2),
                                                Substr(p_Rec, 003, 3),
                                                Substr(p_Rec, 006, 1),
                                                Substr(p_Rec, 007, 2),
                                                Substr(p_Rec, 009, 1),
                                                Substr(p_Rec, 010, 2),
                                                Substr(p_Rec, 012, 1),
                                                Substr(p_Rec, 013, 1),
                                                Substr(p_Rec, 014, 1),
                                                Substr(p_Rec, 015, 5),
                                                Substr(p_Rec, 020, 3),
                                                Substr(p_Rec, 023, 1),
                                                Substr(p_Rec, 024, 1),
                                                Substr(p_Rec, 025, 1),
                                                Substr(p_Rec, 026, 1),
                                                Substr(p_Rec, 027, 13),
                                                Substr(p_Rec, 040, 1),
                                                Substr(p_Rec, 044, 84),
                                                Substr(p_Rec, 041, 3)); */  /* redesign changes begin -----------------------------    SCJ  */
   v_Return_Rec := Type_Tlog05_2751discountrsn(Substr(p_Rec, 001, 2),
                                                Substr(p_Rec, 003, 3),
                                                Substr(p_Rec, 006, 1),
                                                Substr(p_Rec, 007, 2),
                                                Substr(p_Rec, 009, 1),
                                                Substr(p_Rec, 010, 2),
                                                Substr(p_Rec, 012, 13),
                                                Substr(p_Rec, 025, 16),
                                                Substr(p_Rec, 041, 3),
                                                Substr(p_Rec, 044, 20),
                                                Substr(p_Rec, 064, 1),
                                                Substr(p_Rec, 065, 1),
                                                Substr(p_Rec, 066, 10),
                                                v_DiscountAmount,
                                                Substr(p_Rec, 086, 10),
                                                Substr(p_Rec, 096, 4),
                                                Substr(p_Rec, 100, 8));

                                                /* redesign changes begin -----------------------------    SCJ  */
    /*  tlog record spec (From Base-Mods)
    STRING TYPE 27 - 51 USER STRING ? DISCOUNT REASON CODE

    DESCRIPTION                            BYTES            LOCATION
    STRING TYPE(27)                  2              001-002
    VENDOR ID(999)                  3              003-005
    BEFORE/AFTER INDICATOR(0)        1              006-006
    USER IDENTIFICATION(51)          2              007-008
    DISCOUNT TYPE                    1              009-009
    DISCOUNT REASON CODE            2              010-011
    BARCODE #1                      13            012-024
    BARCODE #2                      16            025-040
    3-DIGIT DISCOUNT REASON CODE  3              041-043
    FILLER                          84            044-127

    * DISCOUNT TYPE - 1 = ITEM LEVEL; 2 = TRANSACTION LEVEL
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2762valuecard(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2762valuecard IS
    v_Return_Rec Type_Tlog05_2762valuecard;
    /*
      Parses tlog rec and loads into formatted array.
    */
    v_Load_Amount NUMBER;
  BEGIN
    v_Load_Amount := To_Number(Substr(p_Rec, 031, 6)) * .01;
    v_Return_Rec  := Type_Tlog05_2762valuecard(Substr(p_Rec, 001, 2),
                                               Substr(p_Rec, 003, 3),
                                               Substr(p_Rec, 006, 1),
                                               Substr(p_Rec, 007, 2),
                                               Substr(p_Rec, 009, 2),
                                               Substr(p_Rec, 011, 19),
                                               Substr(p_Rec, 030, 1),
                                               Substr(p_Rec, 031, 6),
                                               Substr(p_Rec, 037, 6),
                                               Substr(p_Rec, 043, 1),
                                               Substr(p_Rec, 044, 10),
                                               Substr(p_Rec, 054, 4),
                                               Substr(p_Rec, 058, 6),
                                               Substr(p_Rec, 064, 1),
                                               Substr(p_Rec, 065, 1),
                                               Substr(p_Rec, 066, 1),
                                               Substr(p_Rec, 067, 1),
                                               Substr(p_Rec, 068, 6),
                                               Substr(p_Rec, 074, 54));

    /*  tlog record spec
    STRING_TYPE                  VARCHAR2(2)   \*001-002*\
    VENDOR_ID                    VARCHAR2(3)   \*003-005(999)*\
    BEFORE_AFTER_INDICATOR       VARCHAR2(1)   \*006-006(0)*\
    USER_IDENTIFICATION          VARCHAR2(2)   \*007-008(62)*\
    VALUE_CARD_ACTION            VARCHAR2(2)   \*009-010*\
    ACCOUNT_NUMBER               VARCHAR2(19)  \*011-029*\
    SALE_RETURN                     VARCHAR2(1)   \*030-030*\
    LOAD_AMOUNT                     VARCHAR2(6)   \*031-036*\
    AVAILABLE_BALANCE             VARCHAR2(6)   \*037-042*\
    ENTRY_METHOD                 VARCHAR2(1)   \*043-043*\
    STORE_NUMBER                  VARCHAR2(10)  \*044-053*\
    TERMINAL_NUMBER              VARCHAR2(4)   \*054-057*\
    SYSTEM_TRACE_AUDIT_NUMBER      VARCHAR2(6)   \*058-063*\
    INCOMPLETE_TRANSACTION         VARCHAR2(1)   \*064-064*\
    LINE_VOID_INDICATOR             VARCHAR2(1)   \*065-065*\
    AUTHORIZATION_METHOD         VARCHAR2(1)   \*066-066*\
    REBOOT_INDICATOR             VARCHAR2(1)   \*067-067*\
    TRANSACTION_NUMBER             VARCHAR2(6)   \*068-073*\
    FILLER                         VARCHAR2(54)  \*074-127*\
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2777multidivmsttxn(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2777multidivmsttxn IS
    v_Return_Rec Type_Tlog05_2777multidivmsttxn;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Return_Rec := Type_Tlog05_2777multidivmsttxn(Substr(p_Rec, 001, 2),
                                                   Substr(p_Rec, 003, 3),
                                                   Substr(p_Rec, 006, 1),
                                                   Substr(p_Rec, 007, 2),
                                                   Substr(p_Rec, 009, 5),
                                                   Substr(p_Rec, 014, 3),
                                                   Substr(p_Rec, 017, 6),
                                                   Substr(p_Rec, 023, 6),
                                                   Substr(p_Rec, 029, 1));
    /*  tlog record spec
    STRING_TYPE                 VARCHAR2(2)   \*001-002*\
    VENDOR_ID                   VARCHAR2(3)   \*003-005(999)*\
    BEFORE_AFTER_INDICATOR      VARCHAR2(1)   \*006-006(0)*\
    USER_IDENTIFICATION         VARCHAR2(2)   \*007-008(80)*\
    ALT_DIV_STORE               VARCHAR2(5)   \*009-013*\
    ALT_DIV_REGISTER            VARCHAR2(3)   \*014-016*\
    ALT_TRANSACTION_DATE        VARCHAR2(6)   \*017-022*\
    ALT_TRANSACTION_NUM         VARCHAR2(6)   \*023-028*\
    IS_CHILD                    VARCHAR2(6)   \*029-029*\  ---undocumented
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2792pomogiftsku(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2792pomogiftsku IS
    v_Return_Rec Type_Tlog05_2792pomogiftsku;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Return_Rec := Type_Tlog05_2792pomogiftsku(Substr(p_Rec, 001, 2),
                                                Substr(p_Rec, 003, 3),
                                                Substr(p_Rec, 006, 1),
                                                Substr(p_Rec, 007, 2),
                                                Substr(p_Rec, 009, 12));
    /*  tlog record spec
    STRING_TYPE                VARCHAR2(2)   \*001-002*\
    VENDOR_ID                  VARCHAR2(3)   \*003- 005(999)*\
    BEFORE_AFTER_INDICATOR     VARCHAR2(1)   \*006- 006(1)*\
    USER_IDENTIFICATION        VARCHAR2(2)   \*007- 008(92)*\
    SKU_FOR_GIFT_EVENT_AWARD   VARCHAR2(12)  \*009- 020 Zero filled, right just.*\
    */

    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2793custloyalyt(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2793custloyalyt IS
    v_Return_Rec Type_Tlog05_2793custloyalyt;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Return_Rec := Type_Tlog05_2793custloyalyt(Substr(p_Rec, 001, 2),
                                                Substr(p_Rec, 003, 3),
                                                Substr(p_Rec, 006, 1),
                                                Substr(p_Rec, 007, 2),
                                                Substr(p_Rec, 009, 14));
    /*  tlog record spec
    (STRING_TYPE                varchar2(2)   \* 001-002*\
    ,VENDOR_ID                  varchar2(3)   \* 003 - 005(999)*\
    ,BEFORE_AFTER_INDICATOR     varchar2(1)   \* 006 - 006(1)*\
    ,USER_INDICATOR             varchar2(2)   \* 007 - 008(93)*\
    ,LOYALTY_NUMBER             varchar2(14)  \* 009 - 022 *\
    )
          */
    RETURN v_Return_Rec;
  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/

  FUNCTION Get_Tlog05_2799directordernbr(p_Rec IN VARCHAR2)
    RETURN Type_Tlog05_2799directordernbr IS
    v_Return_Rec Type_Tlog05_2799directordernbr;
    /*
      Parses tlog rec and loads into formatted array.
    */
  BEGIN
    v_Return_Rec := Type_Tlog05_2799directordernbr(Substr(p_Rec, 001, 2),
                                                   Substr(p_Rec, 003, 3),
                                                   Substr(p_Rec, 006, 1),
                                                   Substr(p_Rec, 007, 2),
                                                   Substr(p_Rec, 009, 10),   --AEO-863
                                                   Substr(p_Rec, 019, 3),    --AEO-863
                                                   Substr(p_Rec, 022, 10),   --AEO-863
                                                   Substr(p_Rec, 032, 8),
                                                   Substr(p_Rec, 040, 3),    --AEO-846 changes here------------------SCJ
                                                   Substr(p_Rec, 043, 101)); --AEO-846 changes here------------------SCJ
    /*  tlog record spec
    STRING_TYPE             VARCHAR2(2)   \*001-002*\
    VENDOR_ID               VARCHAR2(3)   \*003-005(999)*\
    BEFORE_AFTER_INDICATOR  VARCHAR2(1)   \*006-006(0)*\
    USER_IDENTIFICATION     VARCHAR2(2)   \*007-008(99)*\
    ORDER_NUMBER            VARCHAR2(8)   \*009-018*\
    SUFFIX_NUMBER           VARCHAR2(3)   \*019-021*\
    ORIGINAL_ORDER_NUMBER   VARCHAR2(8)   \*022-031*\
    ORDER_DATE              VARCHAR2(8)   \*032-040*\
    CURRENCY CODE           VARCHAR2(3)   \*040-043*\ --AEO-846 changes here------------------SCJ
    FILLER                  VARCHAR2(108) \*043-101*\
    */

    RETURN v_Return_Rec;
  END;

 /********************************************************************
  This is an internal procedure that is processed at the end of the
  Stage_Tlog05 procedure that will go back through and zero out any
  returns that don't have any matching original purchase records from
  within the current tlog
  ********************************************************************/
  PROCEDURE ProcessSameDayReturns AS

  BEGIN

    ---------------------------
    -- Process Returns
    ---------------------------
    DECLARE
      CURSOR get_data IS
      select distinct stg.txnheaderid
      from lw_txndetail_Stage5 stg
      Where  1=1
      and    stg.txntypeid = 2
      And    stg.txnoriginaltxnrowkey is null
      and  stg.ordernumber is null
      and    not exists (select 1 from lw_txndetail_Stage5 stg3 where stg3.txnstoreid = stg.txnoriginalstoreid and trunc(stg3.txndate) = stg.txnoriginaltxndate and stg3.txnnumber = stg.txnoriginaltxnnumber and stg3.ipcode > 0)
      union
            select distinct stg.txnheaderid
      from lw_txndetail_Stage5 stg
      Where  1=1
      and    stg.txntypeid = 2
      And    stg.txnoriginaltxnrowkey is null
      and  stg.ordernumber is not null
      and    not exists (select 1 from lw_txndetail_Stage5 stg3 where stg3.txnstoreid = stg.txnoriginalstoreid and stg3.txnnumber = stg.txnoriginaltxnnumber and stg3.ipcode > 0)
      ;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE lw_txndetail_Stage5 stg
             SET stg.txnqualpurchaseamt = 0
           WHERE stg.txnheaderid = v_tbl(i).txnheaderid;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;

  END ProcessSameDayReturns;


/*Redesign Changes begin Here ---------------------------------------------SCJ*/

PROCEDURE UpdateRewardRedemptiondate AS
  /*
  This procedure updates the Redemption date for any rewards redeemed(lw_memberrewards) from the lw_txnrewardredeemstage table
*/
  BEGIN
    ---------------------------
    -- Update The Certificate Redemption date in MemberRewards
    ---------------------------
    DECLARE
      CURSOR get_data2 IS
        SELECT certificatecode, txndate from lw_txnrewardredeem_stage5;
      TYPE t_tab IS TABLE OF get_data2%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data2;
      LOOP
        FETCH get_data2 BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count  --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE lw_memberrewards mr
             SET mr.redemptiondate = v_tbl(i).txndate, mr.lwordernumber = 3
           WHERE mr.certificatenmbr = v_tbl(i).certificatecode;

        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data2%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data2%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data2;
      END IF;
    END;

  END UpdateRewardRedemptiondate;
/*Redesign Changes begin Here ---------------------------------------------SCJ*/

    /********************************************************************
  ********************************************************************/
  PROCEDURE UpdateMemberAttributesFromTlog AS
    v_Logsource        VARCHAR2(256) := 'Stage_Tlog5.UpdateMemberAttributesFromTlog';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'Stage_Tlog5.UpdateMemberAttributesFromTlog';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'UpdateMemberAttributesFromTlog';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));

  BEGIN

    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    /* log start of job (lw logging)*/
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);

    ---------------------------
    -- Update Employee flag
    ---------------------------
    DECLARE
      CURSOR get_data IS
        SELECT txn.ipcode
          FROM lw_txndetail_Stage5 txn
         WHERE txn.ipcode > 0
           AND txn.txnemployeeid IS NOT NULL;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails mds
             SET mds.a_employeecode = 1
           WHERE mds.a_ipcode = v_tbl(i).ipcode
             AND (mds.a_employeecode IS NULL OR mds.a_employeecode = 0 OR
                 mds.a_employeecode = 2);
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;

    ---------------------------
    -- Update AECC and AEVisa
    ---------------------------
    DECLARE
      CURSOR get_data IS
        SELECT txn.ipcode, txn.tendertype
          FROM lw_txntender_Stage5 txn
         WHERE txn.ipcode > 0
           AND txn.tendertype in ('75', '78');
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails mds
             SET mds.a_cardtype = case
                                    when v_tbl(i).tendertype = '75' and mds.a_cardtype is null Then
                                     1
                                    when v_tbl(i).tendertype = '75' and mds.a_cardtype = 1 Then
                                     1
                                    when v_tbl(i).tendertype = '75' and mds.a_cardtype = 2 Then
                                     3
                                    when v_tbl(i).tendertype = '75' and mds.a_cardtype = 3 Then
                                     3
                                    when v_tbl(i).tendertype = '78' and mds.a_cardtype = 2 Then
                                     1
                                    when v_tbl(i).tendertype = '78' and mds.a_cardtype is null Then
                                     2
                                    when v_tbl(i).tendertype = '78' and mds.a_cardtype = 1 Then
                                     3
                                    when v_tbl(i).tendertype = '78' and mds.a_cardtype = 3 Then
                                     3
                                  end
           WHERE mds.a_ipcode = v_tbl(i).ipcode;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;

    ---------------------------
    -- Update UnderAge flag
    ---------------------------
    DECLARE
      CURSOR get_data IS
        SELECT lm.ipcode
          FROM lw_loyaltymember lm
         inner join ats_memberdetails md
            on lm.ipcode = md.a_ipcode
         WHERE lm.Memberstatus <> 3
           AND Md.a_IsUnderAge = 1
           AND Lm.Birthdate < Add_Months(SYSDATE, -156);
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails mds
             SET mds.a_isunderage = 0
           WHERE mds.a_ipcode = v_tbl(i).ipcode
             AND mds.a_isunderage = 1;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;

      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => v_Filename,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => sysdate,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

  EXCEPTION
    WHEN OTHERS THEN

      ROLLBACK;
      IF v_Messagesfailed = 0

       THEN
        v_Messagesfailed := 1;
      END IF;
      v_Jobstatus := 3;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => v_Filename,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => sysdate,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure UpdateMemberAttributesFromTlog5';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>Stage_Tlog5</pkg>' || Chr(10) ||
                          '    <proc>UpdateMemberAttributesFromTlog5</proc>' ||
                          Chr(10) || '    <filename>' || v_Filename ||
                          '</filename>' || Chr(10) || '  </details>' ||
                          Chr(10) || '</failed>';
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
                          p_Trycount  => 0,
                          p_Msgtime   => SYSDATE);
  END UpdateMemberAttributesFromTlog;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Initialize_Tbl(p_Filename VARCHAR2) IS
    v_Partition_Name VARCHAR2(256);
    v_Sql            VARCHAR2(4000);
    v_Inst           VARCHAR2(64) := Upper(Sys_Context('userenv',
                                                       'instance_name'));
  BEGIN
    /*
       set the external tables filename
       also clears out the external table log file
    */
    v_Sql := 'ALTER TABLE ext_Tlog05' || Chr(10) || 'LOCATION (AE_IN' ||
             Chr(58) || '''' || p_Filename || ''')';
    EXECUTE IMMEDIATE v_Sql;
    Clear_Infile('ext_Tlog05_log');
  END Initialize_Tbl;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Initialize_Head_Rec RETURN Type_Tlog05_Stg_Header IS
    /* resets the array */
  BEGIN
    RETURN Type_Tlog05_Stg_Header(NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,---------------AEO-844 changes here --------------SCJ
                                  NULL,---------------AEO-844 changes here --------------SCJ
                                  NULL,-------------National rollout Changes-------------SCJ
                                  NULL, -------------National rollout Changes-------------SCJ
                                  NULL,
                                  NULL--AEO-1533
                                 );
  END Initialize_Head_Rec;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Initialize_Detail_Rec RETURN Type_Tlog05_Stg_Detail IS
    /* resets the array */
  BEGIN
    RETURN Type_Tlog05_Stg_Detail(NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,---------------AEO-844 changes here --------------SCJ
                                  NULL,---------------AEO-844 changes here --------------SCJ
                                  NULL,---------------AEO-844 changes here --------------SCJ
                                  NULL,-------------National rollout Changes-------------SCJ
                                  NULL, -------------National rollout Changes-------------SCJ
                                  NULL, --AEO-1533
                                  NULL);
  END Initialize_Detail_Rec;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Initialize_Tender_Rec RETURN Type_Tlog05_Stg_Tender IS
    /* resets the array */
  BEGIN
    RETURN Type_Tlog05_Stg_Tender(NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL);
  END Initialize_Tender_Rec;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Initialize_Discount_Rec RETURN Type_Tlog05_Stg_Discount IS
    /* resets the array */
  BEGIN
    RETURN Type_Tlog05_Stg_Discount(NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL);
  END Initialize_Discount_Rec;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Initialize_Reward_Rec RETURN Type_Tlog05_Stg_Reward IS
    /* resets the array */
  BEGIN
    RETURN Type_Tlog05_Stg_Reward(NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL,
                                  NULL);
  END Initialize_Reward_Rec;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Get_Headerid(p_Store_Nbr IN VARCHAR2,
                         p_Reg_Nbr   IN VARCHAR2,
                         p_Txn_Date  IN DATE,
                         p_Txn_Nbr   IN VARCHAR2,
                         p_Processid IN OUT NUMBER,
                         p_Rec_Type  IN OUT VARCHAR2,
                         p_Action    OUT VARCHAR2,
                         p_Headerid  OUT NUMBER
                              /*AEO-727 changes begin here ---------------------------SCJ*/
                       ,p_Ordernumber IN VARCHAR2
                         ) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    v_Headerid  NUMBER;
    v_Rec_Type  VARCHAR2(30);
    v_Processid NUMBER;
    n           NUMBER;
    v_Ordernumber  VARCHAR2(15);     /*AEO-727 changes  here ---------------------------SCJ*/
    /*
       This process is used to generate a lookup table.
       The lookup table is used to keep track of transactions already processed.
       It's alow used to match up child/master data for 77 kids.

       This table should be managed by another process to purge old data.
    */
  BEGIN
    /* see if headerid is already defined from previous run */
    SELECT Txnheaderid, First_Rec_Type, Processid
      INTO v_Headerid, v_Rec_Type, v_Processid
      FROM Log_Txn_Keys
     WHERE Store_Nbr = TRIM(p_Store_Nbr)
       AND Register_Nbr = TRIM(p_Reg_Nbr)
       AND Txn_Date = Trunc(p_Txn_Date)
       AND Txn_Nbr = TRIM(p_Txn_Nbr)
       AND (Ordernumber = TRIM(p_Ordernumber) Or Ordernumber IS NULL);          /*AEO-727 changes  here ---------------------------SCJ*/

    p_Headerid := v_Headerid;
    p_Rec_Type := v_Rec_Type;
    IF v_Processid IN (Gv_Status_77child, Gv_Status_77master) AND
       p_Processid IN (Gv_Status_77child, Gv_Status_77master) AND
       v_Processid != p_Processid
    THEN
      p_Processid := Gv_Status_77merged;
      p_Action    := 'merge';
    ELSIF v_Processid = Gv_Status_77merged
    THEN
      p_Processid := v_Processid;
      p_Action    := 'merged';
    ELSIF v_Processid = Gv_Status_Ready
    THEN /* make sure txn really isn't processed */
      SELECT COUNT(*)
      INTO n
      FROM ats_txnheader t
      WHERE t.a_txnheaderid = to_char(p_Headerid);

      IF n > 1 THEN /* txn is processed, mark as such and move on */
        p_Processid := Gv_Status_Dup;
        p_Action    := 'found';
        UPDATE Log_Txn_Keys
        SET processid = Gv_Status_Processed
         WHERE Store_Nbr = TRIM(p_Store_Nbr)
           AND Register_Nbr = TRIM(p_Reg_Nbr)
           AND Txn_Date = Trunc(p_Txn_Date)
           AND Txn_Nbr = TRIM(p_Txn_Nbr)
           AND ordernumber = TRIM(p_Ordernumber) ;  /*AEO-727 changes  here ---------------------------SCJ*/

        COMMIT;
      ELSE
        p_Action := 'new';
      END IF;
    ELSE
      p_Processid := Gv_Status_Dup;
      p_Action    := 'found';
    END IF;

    /*    IF v_processid = gv_status_processed THEN
      p_processid := v_processid;
      P_action := 'found';
    ELSIF v_processid = gv_status_77merged THEN
     P_action := 'merged';
    ELSE
     P_action := 'found';
    END IF;*/

    RETURN;
  EXCEPTION
    WHEN No_Data_Found THEN
      /* headerid not in history */
      BEGIN
        SELECT Seq_Rowkey.Nextval INTO p_Headerid FROM Dual;
        /* try inserting association to stage table */
        INSERT INTO LOG_TXN_KEYS_Stage5
          (Txnheaderid,
           Store_Nbr,
           Register_Nbr,
           Txn_Date,
           Txn_Nbr,
           First_Rec_Type,
           Processid,
           Ordernumber
           )                              /*AEO-727 changes  here ---------------------------SCJ*/
        VALUES
          (p_Headerid,
           TRIM(p_Store_Nbr),
           TRIM(p_Reg_Nbr),
           Trunc(p_Txn_Date),
           TRIM(p_Txn_Nbr),
           Lower(Substr(p_Rec_Type, 1, 1)),
           p_Processid,
           p_Ordernumber);                   /*AEO-727 changes  here ---------------------------SCJ*/
        COMMIT;
        p_Action := 'new';
        RETURN;
      EXCEPTION
        WHEN Dup_Val_On_Index THEN
          /* header id already defined in this run */
          /* get the header id defined by this run */
          SELECT Txnheaderid, First_Rec_Type, Processid
            INTO v_Headerid, v_Rec_Type, v_Processid
            FROM LOG_TXN_KEYS_Stage5
           WHERE Store_Nbr = TRIM(p_Store_Nbr)
             AND Register_Nbr = TRIM(p_Reg_Nbr)
             AND Txn_Date = Trunc(p_Txn_Date)
             AND Txn_Nbr = TRIM(p_Txn_Nbr)
             AND ordernumber = TRIM(p_Ordernumber) ;   /*AEO-727 changes  here ---------------------------SCJ*/

          /*        p_headerid := v_headerid;
          p_rec_type := v_rec_type;
          IF v_processid = 11 THEN
           P_action := 'merged';
          ELSE
           P_action := 'found';
          END IF;*/
          p_Headerid := v_Headerid;
          p_Rec_Type := v_Rec_Type;
          IF v_Processid IN (Gv_Status_77child, Gv_Status_77master) AND
             p_Processid IN (Gv_Status_77child, Gv_Status_77master) AND
             v_Processid != p_Processid
          THEN
            p_Processid := Gv_Status_77merged;
            p_Action    := 'merge';
          ELSIF v_Processid = Gv_Status_77merged
          THEN
            p_Processid := v_Processid;
            p_Action    := 'merged';
          ELSE
            p_Processid := Gv_Status_Dup;
            p_Action    := 'found';
          END IF;
          RETURN;
      END;
  END Get_Headerid;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Get_24hr_Txn_Cnt(/*p_Vckey IN NUMBER,*/
                            p_ipcode IN NUMBER,
                            p_txnamount IN OUT FLOAT,  /*National Rollout Changes AEO-1218 here ----------------SCJ*/ -- OUT parm in a function, not going to be called from SQL, needs
                                                                                       --- to be changed to a procedure, when need for SQL to call this arises
                             p_qualamount OUT FLOAT, -- AEO-1748
                            p_Date  IN DATE) RETURN NUMBER IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    Lv_N1 NUMBER := 0;
    Lv_N2 NUMBER := 0;
    Lv_Ip NUMBER := p_ipcode;
    LV_txnamount FLOAT := p_txnamount;
    LV2_txnamount FLOAT := 0; -- AEO-1748
    Lv_Log_Cnt NUMBER := 0;
    lv_exist NUMBER := 0;
    /*
      to apply txn 24hr rule.

      Maintains a log table that should be managed via another process
      old data should not be kept.

    */
  BEGIN
/*    SELECT Ipcode
      INTO Lv_Ip
      FROM Lw_Virtualcard Vc
     WHERE Vc.Vckey = p_Vckey;*/

-- AEO-1748 begin

  SELECT COUNT(*)
  INTO lv_exist
  FROM bp_ae.log_txn_counts t
  WHERE t.Ipcode = Lv_Ip
       AND t.sdate = Trunc(p_Date);


  IF (lv_exist >0 ) THEN

      SELECT  txnamount
      INTO LV2_txnamount
      FROM Log_Txn_Counts t
      WHERE t.Ipcode = Lv_Ip
           AND t.sdate = Trunc(p_Date);

  ELSE

      LV2_txnamount := 0;

  END IF;

  -- if this ipcoee has earned 1000 points o rmore in the current
  -- date hen qualified amoutn must be zero
  IF (LV2_txnamount)> 1000 THEN
           p_qualamount := 0;
  END IF;

  -- if the total amount is less than 1000 and this tn excedds then
  -- only accrue the points  needed to reach 1000
  IF (LV2_txnamount) < 1000 AND ( LV2_txnamount + LV_txnamount) > 1000 THEN
     p_qualamount := 1000 - LV2_txnamount;
  END IF;
-- AEO-1748 end

    UPDATE Log_Txn_Counts t
       SET Cnt = Cnt + 1, txnamount = txnamount + LV_txnamount
     WHERE t.Ipcode = Lv_Ip
       AND Sdate = Trunc(p_Date)
     RETURNING Cnt,txnamount INTO Lv_Log_Cnt,p_txnamount;

    IF SQL%ROWCOUNT = 0
    THEN
      Lv_Log_Cnt := 1;

     INSERT INTO Log_Txn_Counts
        (Ipcode, Sdate, Cnt,txnamount)
      VALUES
        (Lv_Ip, Trunc(p_Date), Lv_Log_Cnt,LV_txnamount);
    END IF;
  /*National Rollout Changes AEO-1218 ends here ----------------SCJ*/
    COMMIT;

    RETURN Lv_Log_Cnt;
  END Get_24hr_Txn_Cnt;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_Process_Count(p_Rowcount NUMBER) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    n VARCHAR2(64);
    /*
      This process allows the main process report progress.
    */
  BEGIN
    UPDATE Log_Process_Stat
       SET Current_Count = p_Rowcount
     WHERE Lower(Process) = Lower(Gv_Process);
    /* only commits activity in this local proc */
    COMMIT;
  END Log_Process_Count;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_Process_State(p_State VARCHAR2) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    n VARCHAR2(64);
    /*
      This process simply keeps track of the state/step of the main process.
    */
  BEGIN
    UPDATE Log_Process_Stat
       SET State = p_State
     WHERE Lower(Process) = Lower(Gv_Process);

    IF SQL%ROWCOUNT = 0
    THEN
      SELECT Seq_Rowkey.Nextval INTO Gv_Process_Id FROM Dual;

      INSERT INTO Log_Process_Stat
        (Id,
         Process,
         Is_Active,
         State,
         Sid,
         Inst_Id,
         Expiration_Date,
         Current_Count,
         Job_Start,
         Job_End)
      VALUES
        (Gv_Process_Id,
         Lower(Gv_Process),
         'yes',
         'new',
         Sys_Context('userenv', 'sessionid'),
         Sys_Context('userenv', 'instance'),
         To_Date('12/31/2999', 'mm/dd/yyyy'),
         0,
         SYSDATE,
         NULL);

    END IF;

    /* only commits activity in this local proc */
    COMMIT;
  END Log_Process_State;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Get_Parthighvalue(p_Table_Name     VARCHAR2,
                             p_Partition_Name VARCHAR2) RETURN VARCHAR2 IS
    v_High_Value LONG;
    /*
      This is a process to aqure partition table information to make
      decisions on how to manage the table.
    */
  BEGIN
    SELECT High_Value
      INTO v_High_Value
      FROM User_Tab_Partitions
     WHERE Table_Name = p_Table_Name
       AND Partition_Name = p_Partition_Name;

    RETURN TRIM(Substr(v_High_Value, 1, 4000));

  END;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Re-Stage Unprocessed data  ********************/
  PROCEDURE Stage_unprocessed_Tlog05 (p_Dummy VARCHAR2,
                                      Retval       IN OUT Rcursor) AS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    /*
      this process will move unprocessed staged data to the unprocessed tables prior to truncating the staging tables.
      This should be called after each sucessful run of the tlog process from the hist_tlog proc, prior to it's actuall operations...
    */
    v_jobNumber    NUMBER :=-1;
    v_My_Log_Id              NUMBER;
    v_Dap_Log_Id             NUMBER;

    --log job attributes
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_N1               NUMBER := 0;
    v_N2               NUMBER := 0;
    v_Txt1             VARCHAR2(512);
    v_Txt2             VARCHAR2(512);
    v_Dt               DATE;
    v_Jobstatus        NUMBER := 0;
    v_Filename         VARCHAR2(256) := 'UnProcessed Txns';
    v_Jobname          VARCHAR2(256) := 'UnProcessedTlog05';
  BEGIN
    /* data need to be written to stage tables */
    Log_Process_State('lw_txndetail_Stage5');

/* get job log id for this process and for the dap process */
    v_My_Log_Id  := Utility_Pkg.Get_Libjobid();
    v_Dap_Log_Id := Utility_Pkg.Get_Libjobid();

    /* log start of job (lw logging)*/
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => 'stage' || v_Jobname);

    select count(*) into v_Messagesreceived
      FROM Log_Txndtl_Unprocessed t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid))
      AND NOT EXISTS (SELECT 1
                      FROM lw_txndetail_Stage5 x
                      WHERE x.rowkey = t.rowkey);


    INSERT INTO lw_txndetail_Stage5
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Dtlquantity
            ,Dtldiscountamount
            ,Dtlclearanceitem
            ,Dtldatemodified
            ,Reconcilestatus
            ,Txnheaderid
            ,Txndetailid
            ,Brandid
            ,Fileid
            ,Processid
            ,Filelineitem
            ,Cardid
            ,Creditcardid
            ,Txnloyaltyid
            ,Txnmaskid
            ,Txnnumber
            ,Txndate
            ,Txndatemodified
            ,Txnregisternumber
            ,Txnstoreid
            ,Txntypeid
            ,Txnamount
            ,Txndiscountamount
            ,Txnqualpurchaseamt
            ,Txnemailaddress
            ,Txnphonenumber
            ,Txnemployeeid
            ,Txnchannelid
            ,Txnoriginaltxnrowkey
            ,Txncreditsused
            ,Dtlitemlinenbr
            ,Dtlproductid
            ,Dtltypeid
            ,Dtlactionid
            ,Dtlretailamount
            ,Dtlsaleamount
            ,Dtlclasscode
            ,Errormessage
            ,Shipdate
            ,Ordernumber
            ,Skunumber
            ,Tenderamount
            ,Storenumber
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid
            ,Nonmember)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Dtlquantity
            ,t.Dtldiscountamount
            ,t.Dtlclearanceitem
            ,t.Dtldatemodified
            ,t.Reconcilestatus
            ,t.Txnheaderid
            ,t.Txndetailid
            ,t.Brandid
            ,t.Fileid
            ,t.Processid
            ,t.Filelineitem
            ,t.Cardid
            ,t.Creditcardid
            ,t.Txnloyaltyid
            ,t.Txnmaskid
            ,t.Txnnumber
            ,t.Txndate
            ,t.Txndatemodified
            ,t.Txnregisternumber
            ,t.Txnstoreid
            ,t.Txntypeid
            ,t.Txnamount
            ,t.Txndiscountamount
            ,t.Txnqualpurchaseamt
            ,t.Txnemailaddress
            ,t.Txnphonenumber
            ,t.Txnemployeeid
            ,t.Txnchannelid
            ,t.Txnoriginaltxnrowkey
            ,t.Txncreditsused
            ,t.Dtlitemlinenbr
            ,t.Dtlproductid
            ,t.Dtltypeid
            ,t.Dtlactionid
            ,t.Dtlretailamount
            ,t.Dtlsaleamount
            ,t.Dtlclasscode
            ,t.Errormessage
            ,t.Shipdate
            ,UPPER(t.Ordernumber) as Ordernumber
            ,t.Skunumber
            ,t.Tenderamount
            ,t.Storenumber
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
            ,t.Nonmember
      FROM Log_Txndtl_Unprocessed t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid))
      AND NOT EXISTS (SELECT 1
                      FROM lw_txndetail_Stage5 x
                      WHERE x.rowkey = t.rowkey);

    COMMIT;
    /* data need to be written to hist tables */
    Log_Process_State('lw_txntender_Stage5');


    INSERT INTO lw_txntender_Stage5
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Processid
            ,Storeid
            ,Txndate
            ,Txnheaderid
            ,Txntenderid
            ,Tendertype
            ,Tenderamount
            ,Tendercurrency
            ,Tendertax
            ,Tendertaxrate
            ,Errormessage
            ,Fileid
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Processid
            ,t.Storeid
            ,t.Txndate
            ,t.Txnheaderid
            ,t.Txntenderid
            ,t.Tendertype
            ,t.Tenderamount
            ,t.Tendercurrency
            ,t.Tendertax
            ,t.Tendertaxrate
            ,t.Errormessage
            ,t.Fileid
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
      FROM Log_Txntend_Unprocessed t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM lw_txntender_Stage5 x
                      WHERE t.Rowkey = x.Rowkey)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid));


    COMMIT;
    /* data need to be written to hist tables */
    Log_Process_State('lw_txnrewardredeem_Stage5');



    INSERT INTO lw_txnrewardredeem_Stage5
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Txnheaderid
            ,Txndate
            ,Txndetailid
            ,Programid
            ,Certificateredeemtype
            ,Certificatecode
            ,Certificatediscountamount
            ,Errormessage
            ,Txnrewardredeemid
            ,Processid
            ,Fileid
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Txnheaderid
            ,t.Txndate
            ,t.Txndetailid
            ,t.Programid
            ,t.Certificateredeemtype
            ,t.Certificatecode
            ,t.Certificatediscountamount
            ,t.Errormessage
            ,t.Txnrewardredeemid
            ,t.Processid
            ,t.Fileid
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
      FROM Log_Txnrwdrdm_Unprocessed t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM lw_txnrewardredeem_Stage5 x
                      WHERE t.Rowkey = x.Rowkey)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid));

    COMMIT;
    /* data need to be written to hist tables */
    Log_Process_State('lw_txndetaildiscount_Stage5');

    INSERT INTO lw_txndetaildiscount_Stage5
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Processid
            ,Txndiscountid
            ,Txnheaderid
            ,Txndate
            ,Txndetailid
            ,Discounttype
            ,Discountamount
            ,Txnchannel
            ,Offercode
            ,Errormessage
            ,Fileid
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Processid
            ,t.Txndiscountid
            ,t.Txnheaderid
            ,t.Txndate
            ,t.Txndetailid
            ,t.Discounttype
            ,t.Discountamount
            ,t.Txnchannel
            ,t.Offercode
            ,t.Errormessage
            ,t.Fileid
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
      FROM Log_Txndtldisc_Unprocessed t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM lw_txndetaildiscount_Stage5 x
                      WHERE t.Rowkey = x.Rowkey)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid));

      COMMIT;
    Log_Process_State('Truncate_log_unprocessed');
    /* clear out staged data, hist_tlog job will reload this data if it needs to be maintained again */
    IF v_jobNumber = -1 THEN
      EXECUTE IMMEDIATE 'TRUNCATE TABLE log_txndtldisc_unprocessed';
      EXECUTE IMMEDIATE 'TRUNCATE TABLE log_txndtl_unprocessed';
      EXECUTE IMMEDIATE 'TRUNCATE TABLE log_txnrwdrdm_unprocessed';
      EXECUTE IMMEDIATE 'TRUNCATE TABLE log_txntend_unprocessed';
    ELSE /*only truncate specified partition*/
      DELETE log_txndtldisc_unprocessed WHERE fileid = v_jobNumber;
      DELETE log_txndtl_unprocessed WHERE fileid = v_jobNumber;
      DELETE log_txnrwdrdm_unprocessed WHERE fileid = v_jobNumber;
      DELETE log_txntend_unprocessed WHERE fileid = v_jobNumber;
    END IF;

    v_Messagesreceived := v_Messagespassed + v_Messagesfailed;
    v_Endtime          := SYSDATE;
    v_Jobstatus        := 1;

        /* log end of job */
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => 'Stage-' || v_Jobname);

    /* create job for dap */
    Utility_Pkg.Log_Job(p_Job              => v_Dap_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => SYSDATE,
                        p_Endtime          => NULL,
                        p_Messagesreceived => NULL,
                        p_Messagesfailed   => NULL,
                        p_Jobstatus        => 0,
                        p_Jobname          => 'DAP-' || v_Jobname);

    OPEN Retval FOR
      SELECT v_Dap_Log_Id FROM Dual;

  END stage_unprocessed_Tlog05;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to saves unprocessed txns ***/
  PROCEDURE log_unprocessed_Tlog05 (p_JobNumber NUMBER DEFAULT -1) AS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    /*
      this process will move unprocessed staged data to the unprocessed tables prior to truncating the staging tables.
      This should be called after each sucessful run of the tlog process from the hist_tlog proc, prior to it's actuall operations...
    */
    v_jobNumber    NUMBER :=nvl(p_JobNumber,-1);
  BEGIN
    /* data need to be written to hist tables */
    Log_Process_State('Log_Txndtl_Unprocessed');

    INSERT INTO Log_Txndtl_Unprocessed
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Dtlquantity
            ,Dtldiscountamount
            ,Dtlclearanceitem
            ,Dtldatemodified
            ,Reconcilestatus
            ,Txnheaderid
            ,Txndetailid
            ,Brandid
            ,Fileid
            ,Processid
            ,Filelineitem
            ,Cardid
            ,Creditcardid
            ,Txnloyaltyid
            ,Txnmaskid
            ,Txnnumber
            ,Txndate
            ,Txndatemodified
            ,Txnregisternumber
            ,Txnstoreid
            ,Txntypeid
            ,Txnamount
            ,Txndiscountamount
            ,Txnqualpurchaseamt
            ,Txnemailaddress
            ,Txnphonenumber
            ,Txnemployeeid
            ,Txnchannelid
            ,Txnoriginaltxnrowkey
            ,Txncreditsused
            ,Dtlitemlinenbr
            ,Dtlproductid
            ,Dtltypeid
            ,Dtlactionid
            ,Dtlretailamount
            ,Dtlsaleamount
            ,Dtlclasscode
            ,Errormessage
            ,Shipdate
            ,Ordernumber
            ,Skunumber
            ,Tenderamount
            ,Storenumber
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid
            ,Nonmember)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Dtlquantity
            ,t.Dtldiscountamount
            ,t.Dtlclearanceitem
            ,t.Dtldatemodified
            ,t.Reconcilestatus
            ,t.Txnheaderid
            ,t.Txndetailid
            ,t.Brandid
            ,t.Fileid
            ,t.Processid
            ,t.Filelineitem
            ,t.Cardid
            ,t.Creditcardid
            ,t.Txnloyaltyid
            ,t.Txnmaskid
            ,t.Txnnumber
            ,t.Txndate
            ,t.Txndatemodified
            ,t.Txnregisternumber
            ,t.Txnstoreid
            ,t.Txntypeid
            ,t.Txnamount
            ,t.Txndiscountamount
            ,t.Txnqualpurchaseamt
            ,t.Txnemailaddress
            ,t.Txnphonenumber
            ,t.Txnemployeeid
            ,t.Txnchannelid
            ,t.Txnoriginaltxnrowkey
            ,t.Txncreditsused
            ,t.Dtlitemlinenbr
            ,t.Dtlproductid
            ,t.Dtltypeid
            ,t.Dtlactionid
            ,t.Dtlretailamount
            ,t.Dtlsaleamount
            ,t.Dtlclasscode
            ,t.Errormessage
            ,t.Shipdate
            ,t.Ordernumber
            ,t.Skunumber
            ,t.Tenderamount
            ,t.Storenumber
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
            ,t.Nonmember
      FROM lw_txndetail_Stage5 t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid))
      AND NOT EXISTS (SELECT 1
                      FROM Log_Txndtl_Unprocessed x
                      WHERE x.rowkey = t.rowkey);

    COMMIT;
    /* data need to be written to hist tables */
    Log_Process_State('Log_Txntend_Unprocessed');



    INSERT INTO Log_Txntend_Unprocessed
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Processid
            ,Storeid
            ,Txndate
            ,Txnheaderid
            ,Txntenderid
            ,Tendertype
            ,Tenderamount
            ,Tendercurrency
            ,Tendertax
            ,Tendertaxrate
            ,Errormessage
            ,Fileid
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Processid
            ,t.Storeid
            ,t.Txndate
            ,t.Txnheaderid
            ,t.Txntenderid
            ,t.Tendertype
            ,t.Tenderamount
            ,t.Tendercurrency
            ,t.Tendertax
            ,t.Tendertaxrate
            ,t.Errormessage
            ,t.Fileid
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
      FROM lw_txntender_Stage5 t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM Log_Txntend_Unprocessed x
                      WHERE t.Rowkey = x.Rowkey)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid));


    COMMIT;
    /* data need to be written to hist tables */
    Log_Process_State('Log_Txnrwdrdm_Unprocessed');



    INSERT INTO Log_Txnrwdrdm_Unprocessed
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Txnheaderid
            ,Txndate
            ,Txndetailid
            ,Programid
            ,Certificateredeemtype
            ,Certificatecode
            ,Certificatediscountamount
            ,Errormessage
            ,Txnrewardredeemid
            ,Processid
            ,Fileid
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Txnheaderid
            ,t.Txndate
            ,t.Txndetailid
            ,t.Programid
            ,t.Certificateredeemtype
            ,t.Certificatecode
            ,t.Certificatediscountamount
            ,t.Errormessage
            ,t.Txnrewardredeemid
            ,t.Processid
            ,t.Fileid
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
      FROM lw_txnrewardredeem_Stage5 t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM Log_Txnrwdrdm_Unprocessed x
                      WHERE t.Rowkey = x.Rowkey)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid));

    COMMIT;
    /* data need to be written to hist tables */
    Log_Process_State('Log_Txndtldisc_Unprocessed');

    INSERT INTO Log_Txndtldisc_Unprocessed
            (Rowkey
            ,Ipcode
            ,Vckey
            ,Processid
            ,Txndiscountid
            ,Txnheaderid
            ,Txndate
            ,Txndetailid
            ,Discounttype
            ,Discountamount
            ,Txnchannel
            ,Offercode
            ,Errormessage
            ,Fileid
            ,Statuscode
            ,Createdate
            ,Updatedate
            ,Lastdmlid)
      SELECT t.Rowkey
            ,t.Ipcode
            ,t.Vckey
            ,t.Processid
            ,t.Txndiscountid
            ,t.Txnheaderid
            ,t.Txndate
            ,t.Txndetailid
            ,t.Discounttype
            ,t.Discountamount
            ,t.Txnchannel
            ,t.Offercode
            ,t.Errormessage
            ,t.Fileid
            ,t.Statuscode
            ,t.Createdate
            ,t.Updatedate
            ,t.Lastdmlid
      FROM lw_txndetaildiscount_Stage5 t
      WHERE 1=1
      AND t.processid = 0
      AND v_JobNumber IN (-1,t.fileid)
      AND NOT EXISTS (SELECT 1
                      FROM Log_Txndtldisc_Unprocessed x
                      WHERE t.Rowkey = x.Rowkey)
      AND NOT EXISTS (SELECT 1
                      FROM Ats_Txnheader x
                      WHERE x.a_Txnheaderid = To_Char(t.Txnheaderid));

      COMMIT;

  END log_unprocessed_Tlog05;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to write data to hist ********/
  PROCEDURE Hist_Tlog05(p_Dummy VARCHAR2, p_Process_Date Date,
                        Retval  IN OUT Rcursor) AS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    /*
      this process will move staged data to the history tables then truncate the staging tables.
      This should be called after each sucessful run of the tlog process... do not run it after an incomplete run.
    */
    v_Txn_Count              NUMBER(20) := 0;
    v_EndDate                TIMESTAMP(4);

    --log job attributes
    v_My_Log_Id              NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_N1               NUMBER := 0;
    v_N2               NUMBER := 0;
    v_Txt1             VARCHAR2(512);
    v_Txt2             VARCHAR2(512);
    v_Dt               DATE;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'Hist_Tlog05';
    --log msg attributes
    v_Messageid  NUMBER;
    v_Envkey     VARCHAR2(256) := 'BP_AE@' ||
                                  Upper(Sys_Context('userenv',
                                                    'instance_name'));
    v_Logsource  VARCHAR2(256) := 'Hist_Tlog05';
    v_Batchid    VARCHAR2(256) := 0;
    v_Message    VARCHAR2(256);
    v_Reason     VARCHAR2(256);
    v_Error      VARCHAR2(256);
    v_Trycount   NUMBER := 0;
    v_Process_Id NUMBER := 0;
    v_Skip       NUMBER := 0;
    v_ProcessDate Date;
    v_BypassDateCheck NUMBER := 0;
    v_exist number :=0;

--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
Dml_Errors EXCEPTION;
PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
--AE0-1245: END Create DML error exception handling for post tlog processing

    CURSOR get_reprocessed IS SELECT rowkey
                              from lw_txndetail_Stage5 t
                              WHERE EXISTS (SELECT 1
                                            FROM ats_txnheader x
                                            WHERE t.Txnheaderid = x.a_Txnheaderid)
                              AND EXISTS (SELECT 1
                                          FROM Ats_Historytxndetail x
                                          WHERE x.a_rowkey = t.rowkey
                                          AND x.a_processid = Gv_Status_Ready);
    TYPE t_tbl IS TABLE OF   get_reprocessed%ROWTYPE;
    v_tbl   t_tbl;
  BEGIN

    /* get job log id for this process and for the dap process */
    v_My_Log_Id  := Utility_Pkg.Get_Libjobid();


    If p_Process_Date is null
      THEN v_ProcessDate := sysdate;
      ELSE v_ProcessDate := p_Process_Date;
    END IF;

    /*
    If we pass in 12/31/9999 as the date then the v_BypassDateCheck flag will be set to 1 and we will bypass the date check and process anyway.
    */
    IF p_Process_Date = to_date('12/31/9999', 'mm/dd/yyyy') THEN
      v_BypassDateCheck := 1;
    END IF;

    /* log start of job (lw logging)*/
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Jobname,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);

    /*
    Get the log record for the DAP tlog job for the current date.  use this to check to make sure the DAP tlog job is finished before we run
    the POST and clear out the stage table.
    If by chance the DAP job on one day and the History started on the next day there is an override.  If we pass in 12/31/9999 as the date then
      the v_BypassDateCheck flag will be set to 1 and we will bypass the date check and process anyway.
    */

   BEGIN

       IF v_BypassDateCheck = 1 THEN
         v_EndDate := sysdate;
         v_Txn_Count := 100;

          /* log message that we bypassed date check */
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => v_Jobname,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => 'Bypassing Date check for Tlog Post',
                              p_Reason    => 'Bypassing Date check for Tlog Post',
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);

       ELSE
        SELECT lj.endtime, NVL(lj.messagesreceived, 0)
        INTO   v_EndDate, v_Txn_Count
        FROM   LW_LIBJOB lj
        where  lj.jobname = 'DAP-Tlog05'
        AND    trunc(lj.starttime) = trunc(v_ProcessDate)
        AND    to_char(lj.starttime, 'mm/dd/yyyy hh:mm:ss') = (select to_char(max(starttime), 'mm/dd/yyyy hh:mm:ss') from lw_libjob where  jobname = 'DAP-Tlog05');
       END IF;

    EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'DML_Error';
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
      WHEN No_Data_Found THEN
                      v_Messagesfailed := v_Messagesfailed + 1;
                      v_Error          := SQLERRM;
                      v_Reason         := 'DAP Tlog05 job is not finished';
                      v_Message        := 'DAP Tlog05 job is not finished';

                      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                          p_Jobdirection     => v_Jobdirection,
                                          p_Filename         => v_Jobname,
                                          p_Starttime        => v_Starttime,
                                          p_Endtime          => sysdate,
                                          p_Messagesreceived => v_Messagesreceived,
                                          p_Messagesfailed   => v_Messagesfailed,
                                          p_Jobstatus        => v_Jobstatus,
                                          p_Jobname          => v_Jobname);

                      /* log error */
                      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                          p_Envkey    => v_Envkey,
                                          p_Logsource => v_Logsource,
                                          p_Filename  => v_Jobname,
                                          p_Batchid   => v_Batchid,
                                          p_Jobnumber => v_My_Log_Id,
                                          p_Message   => v_Message,
                                          p_Reason    => v_Reason,
                                          p_Error     => v_Error,
                                          p_Trycount  => v_Trycount,
                                          p_Msgtime   => SYSDATE);

      Raise_Application_Error(-20001, 'DAP Tlog05 job is not finished');

    END;



    IF V_EndDate is null
    THEN
                      v_Messagesfailed := v_Messagesfailed + 1;
                      v_Error          := SQLERRM;
                      v_Reason         := 'DAP Tlog05 job is not finished';
                      v_Message        := 'DAP Tlog05 job is not finished';

                      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                          p_Jobdirection     => v_Jobdirection,
                                          p_Filename         => v_Jobname,
                                          p_Starttime        => v_Starttime,
                                          p_Endtime          => sysdate,
                                          p_Messagesreceived => v_Messagesreceived,
                                          p_Messagesfailed   => v_Messagesfailed,
                                          p_Jobstatus        => v_Jobstatus,
                                          p_Jobname          => v_Jobname);

                      /* log error */
                      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                          p_Envkey    => v_Envkey,
                                          p_Logsource => v_Logsource,
                                          p_Filename  => v_Jobname,
                                          p_Batchid   => v_Batchid,
                                          p_Jobnumber => v_My_Log_Id,
                                          p_Message   => v_Message,
                                          p_Reason    => v_Reason,
                                          p_Error     => v_Error,
                                          p_Trycount  => v_Trycount,
                                          p_Msgtime   => SYSDATE);

      Raise_Application_Error(-20001, 'DAP Tlog05 job is not finished');
    END IF;

    /* PI#29666, Rizwan, Commented out for accepting empty tlog, start

    IF v_Txn_Count = 0
    THEN
                      v_Messagesfailed := v_Messagesfailed + 1;
                      v_Error          := SQLERRM;
                      v_Reason         := 'DAP Tlog05 did not process any records';
                      v_Message        := 'DAP Tlog05 did not process any records';

                      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                          p_Jobdirection     => v_Jobdirection,
                                          p_Filename         => v_Jobname,
                                          p_Starttime        => v_Starttime,
                                          p_Endtime          => sysdate,
                                          p_Messagesreceived => v_Messagesreceived,
                                          p_Messagesfailed   => v_Messagesfailed,
                                          p_Jobstatus        => v_Jobstatus,
                                          p_Jobname          => v_Jobname);

                      \* log error *\
                      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                          p_Envkey    => v_Envkey,
                                          p_Logsource => v_Logsource,
                                          p_Filename  => v_Jobname,
                                          p_Batchid   => v_Batchid,
                                          p_Jobnumber => v_My_Log_Id,
                                          p_Message   => v_Message,
                                          p_Reason    => v_Reason,
                                          p_Error     => v_Error,
                                          p_Trycount  => v_Trycount,
                                          p_Msgtime   => SYSDATE);
      Raise_Application_Error(-20001, 'DAP Tlog05 did not process any records');
    END IF;

    PI#29666, Rizwan, Commented out for accepting empty tlog, end */

    /* this loop will take care of any re-processing that occured from the log_unprocessed tables */
BEGIN
    OPEN get_reprocessed;
    LOOP <<get_reprocessed_loop>>
      FETCH get_reprocessed BULK COLLECT INTO v_tbl LIMIT 500;
         FORALL i IN 1..v_tbl.count SAVE EXCEPTIONS
             UPDATE Ats_Historytxndetail
             SET a_processid = Gv_Status_Processed
             WHERE a_rowkey = v_tbl(i).rowkey;

         COMMIT;
      EXIT WHEN get_reprocessed%NOTFOUND;
    END LOOP get_reprocessed_loop;
--AEO-771 BEGIN
EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'DML_Error';
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;
      v_endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => null,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

      v_Error   := SQLERRM;
      v_Message := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                   '    <pkg>STAGE_TLOG5</pkg>' || Chr(10) ||
                   '    <proc>Hist_Tlog05.Update.Ats_Historytxndetail</proc>' || Chr(10) ||
                   '  </details>' || Chr(10) || '</failed>';
      Utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => null,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);

      Raise_Application_Error(-20002, 'STAGE_TLOG5 job is not finished');

END;
--AEO-771 END

    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNDETAIL');

    DECLARE
      CURSOR get_data IS
       SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             t.Dtlquantity,
             t.Dtldiscountamount,
             t.Dtlclearanceitem,
             t.Dtldatemodified,
             t.Reconcilestatus,
             t.Txnheaderid,
             t.Txndetailid,
             t.Brandid,
             t.Fileid,
             CASE
               WHEN t.Processid = 0 THEN
             /* PI 28864  changes begin here -subquery ats header table removed to improve processing rate -SCJ */
            /* Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = to_char(t.Txnheaderid)),
                    t.Processid)
               ELSE
                t.Processid */
                /* PI 28864  changes end here,*/
                Nvl(( Gv_Status_Processed),t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Filelineitem,
             t.Cardid,
             t.Creditcardid,
             t.Txnloyaltyid,
             t.Txnmaskid,
             t.Txnnumber,
             t.Txndate,
             t.Txndatemodified,
             t.Txnregisternumber,
             t.Txnstoreid,
             t.Storenumber,
             t.Txntypeid,
             t.Txnamount,
             t.Txnqualpurchaseamt,
             t.Txndiscountamount,
             t.Txnemailaddress,
             t.Txnphonenumber,
             t.Txnemployeeid,
             t.Txnchannelid,
             t.Txnoriginaltxnrowkey,
             t.Txncreditsused,
             t.Dtlitemlinenbr,
             t.Dtlproductid,
             t.Dtltypeid,
             t.Dtlactionid,
             t.Dtlretailamount,
             t.Dtlsaleamount,
             t.Dtlclasscode,
             t.Shipdate,
             TRIM(t.Ordernumber) AS Ordernumber,
             t.Skunumber,
             t.tenderamount,
             t.Vckey,
             t.Errormessage,
             t.Statuscode,
             --AEO-845 BEGIN
             t.currencyrate,
             t.currencycode,
             t.dtlsaleamount_org,
             --AEO-845 END
             t.Createdate,
             t.Updatedate,
             t.aeccmultiplier,                              /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             t.originalordernumber,                           /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             t.Cashiernumber,                          ---AEO-1533
             t.Lastdmlid
        FROM lw_txndetail_Stage5 t
       WHERE 1=1
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxndetail x
               WHERE t.Rowkey = x.a_Rowkey);
 TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl1 t_tab; ---<------ our arry object
BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl1 LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        -- Then update address in ATS_MemberDetails table and commit
        FORALL i IN 1 .. v_tbl1.count SAVE EXCEPTIONS--<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop

      INSERT INTO Ats_Historytxndetail(
      a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
       a_Dtlquantity,
       a_Dtldiscountamount,
       a_Dtlclearanceitem,
       a_Dtldatemodified,
       a_Reconcilestatus,
       a_Txnheaderid,
       a_Txndetailid,
       a_Brandid,
       a_Fileid,
       a_Processid,
       a_Filelineitem,
       a_Cardid,
       a_Creditcardid,
       a_Txnloyaltyid,
       a_Txnmaskid,
       a_Txnnumber,
       a_Txndate,
       a_Txndatemodified,
       a_Txnregisternumber,
       a_Txnstoreid,
       a_Storenumber,
       a_Txntype,
       a_Txnamount,
       a_Txnqualpurchaseamt,
       a_Txndiscountamount,
       a_Txnemailaddress,
       a_Txnphonenumber,
       a_Txnemployeeid,
       a_Txnchannelid,
       a_Txnoriginaltxnrowkey,
       a_Txncreditsused,
       a_Dtlitemlinenbr,
       a_Dtlproductid,
       a_Dtltypeid,
       a_Dtlactionid,
       a_Dtlretailamount,
       a_Dtlsaleamount,
       a_Dtlclasscode,
       a_Shipdate,
       a_Ordernumber,
       a_Skunumber,
       a_TenderAmount,
       a_Vckey,
       a_Errormessage,
       Statuscode,
       --AEO-845 BEGIN
       a_currencyrate,
       a_currencycode,
       a_dtlsaleamount_org,
       --AEO-845 END
       Createdate,
       Updatedate,
       a_aeccmultiplier,                    /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
       a_Originalordernumber,                /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
       a_CashierNumber,                      --AEO-1533
       Lastdmlid)
       VALUES
       (     v_tbl1(i).Rowkey,
             v_tbl1(i).Ipcode,
             v_tbl1(i).Parentrowkey,
             v_tbl1(i).Dtlquantity,
             v_tbl1(i).Dtldiscountamount,
             v_tbl1(i).Dtlclearanceitem,
             v_tbl1(i).Dtldatemodified,
             v_tbl1(i).Reconcilestatus,
             v_tbl1(i).Txnheaderid,
             v_tbl1(i).Txndetailid,
             v_tbl1(i).Brandid,
             v_tbl1(i).Fileid,
             v_tbl1(i).Processid,
             v_tbl1(i).Filelineitem,
             v_tbl1(i).Cardid,
             v_tbl1(i).Creditcardid,
             v_tbl1(i).Txnloyaltyid,
             v_tbl1(i).Txnmaskid,
             v_tbl1(i).Txnnumber,
             v_tbl1(i).Txndate,
             v_tbl1(i).Txndatemodified,
             v_tbl1(i).Txnregisternumber,
             v_tbl1(i).Txnstoreid,
             v_tbl1(i).Storenumber,
             v_tbl1(i).Txntypeid,
             v_tbl1(i).Txnamount,
             v_tbl1(i).Txnqualpurchaseamt,
             v_tbl1(i).Txndiscountamount,
             v_tbl1(i).Txnemailaddress,
             v_tbl1(i).Txnphonenumber,
             v_tbl1(i).Txnemployeeid,
             v_tbl1(i).Txnchannelid,
             v_tbl1(i).Txnoriginaltxnrowkey,
             v_tbl1(i).Txncreditsused,
             v_tbl1(i).Dtlitemlinenbr,
             v_tbl1(i).Dtlproductid,
             v_tbl1(i).Dtltypeid,
             v_tbl1(i).Dtlactionid,
             v_tbl1(i).Dtlretailamount,
             v_tbl1(i).Dtlsaleamount,
             v_tbl1(i).Dtlclasscode,
             v_tbl1(i).Shipdate,
             v_tbl1(i).Ordernumber,
             v_tbl1(i).Skunumber,
             v_tbl1(i).tenderamount,
             v_tbl1(i).Vckey,
             v_tbl1(i).Errormessage,
             v_tbl1(i).Statuscode,
             --AEO-845 BEGIN
             v_tbl1(i).currencyrate,
             v_tbl1(i).currencycode,
             v_tbl1(i).dtlsaleamount_org,
             --AEO-845 END
             v_tbl1(i).Createdate,
             v_tbl1(i).Updatedate,
             v_tbl1(i).aeccmultiplier,                               /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl1(i).Originalordernumber,                           /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl1(i).CashierNumber,                               --AEO-1533
             v_tbl1(i).Lastdmlid );
            COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
--AEO-771 BEGIN
EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'RowKey: ' || v_tbl1(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Rowkey
                                        || ', IpCode: ' || v_tbl1(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Ipcode
                                        || ', VCKey: ' || v_tbl1(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Vckey;
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;
      v_endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => null,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

      v_Error   := SQLERRM;
      v_Message := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                   '    <pkg>STAGE_TLOG5</pkg>' || Chr(10) ||
                   '    <proc>Hist_Tlog05.Insert.Ats_Historytxndetail</proc>' || Chr(10) ||
                   '  </details>' || Chr(10) || '</failed>';
      Utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => null,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);

      Raise_Application_Error(-20002, 'STAGE_TLOG5 job is not finished');
END;
--AEO-771 END

    -- Tlog Txn Processed Count
    select count(distinct txnheaderid) into v_Messagesreceived
    from lw_txndetail_Stage5
    where ipcode > 0 and ProcessId = 0 and NonMember = 0;

    --Tlog Txn Success Count
    select count(distinct a_txnheaderid) into v_Messagespassed
    from ats_txnheader where a_txnheaderid in
    (select txnheaderid from lw_txndetail_Stage5 where ipcode > 0 and ProcessId = 0 and NonMember = 0);

    -- Tlog Txn ErrorCouunt = ProcessCount ? SuccessCount
    v_Messagesfailed := v_Messagesreceived - v_Messagespassed;


    EXECUTE IMMEDIATE 'truncate table lw_txndetail_Stage5';

    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNTENDER');

DECLARE
      CURSOR get_data IS
       SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             CASE
               WHEN t.Processid = 0 THEN
                 /* PI 28864  changes begin here -subquery ats header table removed to improve processing rate -SCJ */
                  Nvl(( Gv_Status_Processed),t.Processid)
                --Nvl(Gv_Status_Processed),t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Storeid,
             t.Txndate,
             t.Txnheaderid,
             t.Txntenderid,
             t.Tendertype,
             t.Tenderamount,
             t.Tendercurrency,
             t.Tendertax,
             t.Tendertaxrate,
             t.Vckey,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txntender_Stage5 t
       WHERE 1=1
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxntender x
               WHERE t.Rowkey = x.a_Rowkey);
 TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl2 t_tab; ---<------ our arry object
BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl2 LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        -- Then update address in ATS_MemberDetails table and commit
        FORALL i IN 1 .. v_tbl2.count SAVE EXCEPTIONS--<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
      INSERT INTO Ats_Historytxntender(
      a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
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
       a_Vckey,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
       VALUES
       (     v_tbl2(i).Rowkey,
             v_tbl2(i).Ipcode,
             v_tbl2(i).Parentrowkey,
             v_tbl2(i).Processid,
             v_tbl2(i).Storeid,
             v_tbl2(i).Txndate,
             v_tbl2(i).Txnheaderid,
             v_tbl2(i).Txntenderid,
             v_tbl2(i).Tendertype,
             v_tbl2(i).Tenderamount,
             v_tbl2(i).Tendercurrency,
             v_tbl2(i).Tendertax,
             v_tbl2(i).Tendertaxrate,
             v_tbl2(i).Vckey,
             v_tbl2(i).Errormessage,
             v_tbl2(i).Statuscode,
             v_tbl2(i).Createdate,
             v_tbl2(i).Updatedate,
             v_tbl2(i).Lastdmlid
 );
             COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
--AEO-771 BEGIN
EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'RowKey: ' || v_tbl2(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Rowkey
                                        || ', IpCode: ' || v_tbl2(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Ipcode
                                        || ', VCKey: ' || v_tbl2(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Vckey;
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;
      v_endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => null,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

      v_Error   := SQLERRM;
      v_Message := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                   '    <pkg>STAGE_TLOG5</pkg>' || Chr(10) ||
                   '    <proc>Hist_Tlog05.Insert.Ats_Historytxntender</proc>' || Chr(10) ||
                   '  </details>' || Chr(10) || '</failed>';
      Utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => null,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);

      Raise_Application_Error(-20002, 'STAGE_TLOG5 job is not finished');
END;
--AEO-771 END
/* PI28864 changes end here --  SCJ */
    EXECUTE IMMEDIATE 'truncate table lw_txntender_Stage5';

    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNREWARDREDEEM');

               DECLARE
      CURSOR get_data IS
       SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             CASE
               WHEN t.Processid = 0 THEN
                  /* PI 28864  changes begin here -subquery ats header table removed to improve processing rate -SCJ */
                Nvl(( Gv_Status_Processed),t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Txnheaderid,
             t.Txndate,
             t.Txndetailid,
             t.Programid,
             t.Certificateredeemtype,
             t.Certificatecode,
             t.Certificatediscountamount,
             t.Vckey,
             t.Txnrewardredeemid,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txnrewardredeem_Stage5 t
       WHERE 1=1
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxnrewardredeem x
               WHERE t.Rowkey = x.a_Rowkey);
 TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl3 t_tab; ---<------ our arry object
BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl3 LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        -- Then update address in ATS_MemberDetails table and commit
        FORALL i IN 1 .. v_tbl3.count SAVE EXCEPTIONS--<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
      INSERT INTO Ats_Historytxnrewardredeem(
      a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
       a_Processid,
       a_Txnheaderid,
       a_Txndate,
       a_Txndetailid,
       a_Programid,
       a_Certificateredeemtype,
       a_Certificatecode,
       a_Certificatediscountamount,
       a_Vckey,
       a_Txnrewardredeemid,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
       VALUES
       (
             v_tbl3(i).Rowkey,
             v_tbl3(i).Ipcode,
             v_tbl3(i).Parentrowkey,
             v_tbl3(i).Processid,
             v_tbl3(i).Txnheaderid,
             v_tbl3(i).Txndate,
             v_tbl3(i).Txndetailid,
             v_tbl3(i).Programid,
             v_tbl3(i).Certificateredeemtype,
             v_tbl3(i).Certificatecode,
             v_tbl3(i).Certificatediscountamount,
             v_tbl3(i).Vckey,
             v_tbl3(i).Txnrewardredeemid,
             v_tbl3(i).Errormessage,
             v_tbl3(i).Statuscode,
             v_tbl3(i).Createdate,
             v_tbl3(i).Updatedate,
             v_tbl3(i).Lastdmlid
 );
             COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
--AEO-771 BEGIN
EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'RowKey: ' || v_tbl3(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Rowkey
                                        || ', IpCode: ' || v_tbl3(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Ipcode
                                        || ', VCKey: ' || v_tbl3(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Vckey;
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;
      v_endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => null,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

      v_Error   := SQLERRM;
      v_Message := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                   '    <pkg>STAGE_TLOG5</pkg>' || Chr(10) ||
                   '    <proc>Hist_Tlog05.Insert.Ats_Historytxnrewardredeem</proc>' || Chr(10) ||
                   '  </details>' || Chr(10) || '</failed>';
      Utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => null,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);

      Raise_Application_Error(-20002, 'STAGE_TLOG5 job is not finished');
END;
--AEO-771 END
/* PI28864 changes end here --  SCJ */
    EXECUTE IMMEDIATE 'truncate table lw_txnrewardredeem_Stage5';

    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNDETAILDISCOUNT');

DECLARE
      CURSOR get_data IS
       SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             CASE
               WHEN t.Processid = 0 THEN
                 /* PI 28864  changes  here -subquery ats header table removed to improve processing rate -SCJ */
                 Nvl(( Gv_Status_Processed),t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Txndiscountid,
             t.Txnheaderid,
             t.Txndate,
             t.Txndetailid,
             t.Discounttype,
             t.Discountamount,
             t.Txnchannel,
             t.Offercode,
             t.Vckey,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txndetaildiscount_Stage5 t
       WHERE 1=1
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxndetaildiscount x
               WHERE t.Rowkey = x.a_Rowkey);
 TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl4 t_tab; ---<------ our arry object
BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl4 LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        -- Then update address in ATS_MemberDetails table and commit
        FORALL i IN 1 .. v_tbl4.count SAVE EXCEPTIONS--<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
      INSERT INTO Ats_Historytxndetaildiscount(
      a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
       a_Processid,
       a_Txndiscountid,
       a_Txnheaderid,
       a_Txndate,
       a_Txndetailid,
       a_Discounttype,
       a_Discountamount,
       a_Txnchannel,
       a_Offercode,
       a_Vckey,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
       VALUES
       (
             v_tbl4(i).Rowkey,
             v_tbl4(i).Ipcode,
             v_tbl4(i).Parentrowkey,
             v_tbl4(i).Processid,
             v_tbl4(i).Txndiscountid,
             v_tbl4(i).Txnheaderid,
             v_tbl4(i).Txndate,
             v_tbl4(i).Txndetailid,
             v_tbl4(i).Discounttype,
             v_tbl4(i).Discountamount,
             v_tbl4(i).Txnchannel,
             v_tbl4(i).Offercode,
             v_tbl4(i).Vckey,
             v_tbl4(i).Errormessage,
             v_tbl4(i).Statuscode,
             v_tbl4(i).Createdate,
             v_tbl4(i).Updatedate,
             v_tbl4(i).Lastdmlid
 );
             COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
--AEO-771 BEGIN
EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'RowKey: ' || v_tbl4(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Rowkey
                                        || ', IpCode: ' || v_tbl4(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Ipcode
                                        || ', VCKey: ' || v_tbl4(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Vckey;
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;
      v_endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => null,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

      v_Error   := SQLERRM;
      v_Message := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                   '    <pkg>STAGE_TLOG5</pkg>' || Chr(10) ||
                   '    <proc>Hist_Tlog05.Insert.Ats_Historytxndetaildiscount</proc>' || Chr(10) ||
                   '  </details>' || Chr(10) || '</failed>';
      Utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => null,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);

      Raise_Application_Error(-20002, 'STAGE_TLOG5 job is not finished');
END;

    EXECUTE IMMEDIATE 'truncate table lw_txndetaildiscount_Stage5';
--AEO-771 END
      /* PI28864 changes end here --  SCJ */
--AEO-771 BEGIN
BEGIN
    Log_Process_State('Log_Txn_Keys');

    INSERT INTO Log_Txn_Keys
      (Txnheaderid,
       Store_Nbr,
       Register_Nbr,
       Txn_Date,
       Txn_Nbr,
       First_Rec_Type,
       Processid,
       Ordernumber)             /*AEO-727 changes  here ---------------------------SCJ*/
      SELECT Txnheaderid,
             Store_Nbr,
             Register_Nbr,
             Txn_Date,
             Txn_Nbr,
             First_Rec_Type,
             CASE
               WHEN t.Processid = 0 THEN
                Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = to_char(t.Txnheaderid)),
                    t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             ORDERNUMBER            /*AEO-727 changes  here ---------------------------SCJ*/
        FROM LOG_TXN_KEYS_Stage5 t
       WHERE NOT EXISTS (SELECT 1
                FROM Log_Txn_Keys x
               WHERE x.Txnheaderid = t.Txnheaderid);
    COMMIT;
EXCEPTION
--AE0-1245: BEGIN Create DML error exception handling for post tlog processing
  WHEN Dml_Errors THEN
    FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
    LOOP
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
      v_Reason         := 'Failed Procedure Hist_Tlog05: DML_Error ';
      v_Message        := 'DML_Error';
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                p_Envkey    => v_Envkey,
                p_Logsource => 'Hist_Tlog05',
                p_Filename  => 'Hist_Tlog05',
                p_Batchid   => v_Batchid,
                p_Jobnumber => v_My_Log_Id,
                p_Message   => v_Message,
                p_Reason    => v_Reason,
                p_Error     => v_Error,
                p_Trycount  => v_Trycount,
                p_Msgtime   => SYSDATE);
    END LOOP;
--AE0-1245: END Create DML error exception handling for post tlog processing
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;
      v_endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => null,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);

      v_Error   := SQLERRM;
      v_Message := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                   '    <pkg>STAGE_TLOG</pkg>' || Chr(10) ||
                   '    <proc>Hist_Tlog01.Insert.Log_Txn_Keys</proc>' || Chr(10) ||
                   '  </details>' || Chr(10) || '</failed>';
      Utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => null,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);

      Raise_Application_Error(-20002, 'STAGE_TLOG job is not finished');
END;

    Log_Process_State('LOG_TXN_KEYS');
    log_unprocessed_Tlog05();
--AEO-771 END

    EXECUTE IMMEDIATE 'truncate table LOG_TXN_KEYS_Stage5';

                      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                          p_Jobdirection     => v_Jobdirection,
                                          p_Filename         => v_Jobname,
                                          p_Starttime        => v_Starttime,
                                          p_Endtime          => sysdate,
                                          p_Messagesreceived => v_Messagespassed,
                                          p_Messagesfailed   => v_Messagesfailed,
                                          p_Jobstatus        => v_Jobstatus,
                                          p_Jobname          => v_Jobname);
    --commented out this statement because txns can come in late
    --    EXECUTE IMMEDIATE 'truncate tablel LOG_TXN_COUNTS';
    EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk5' ; -- clear work table before next insert
    EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_Wrk5' ; -- clear work table before next insert

  END Hist_Tlog05;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Procedure to clear stage table without copying to Hist ********/
  PROCEDURE Clear_Stage(p_Dummy VARCHAR2,
                        Retval  IN OUT Rcursor) AS
    /*
      this process will delete staged data without moving to the history tables
    */
  BEGIN

    EXECUTE IMMEDIATE 'truncate table lw_txndetail_Stage5';

    EXECUTE IMMEDIATE 'truncate table lw_txntender_Stage5';

    EXECUTE IMMEDIATE 'truncate table lw_txnrewardredeem_Stage5';

    EXECUTE IMMEDIATE 'truncate table lw_txndetaildiscount_Stage5';

    EXECUTE IMMEDIATE 'truncate table LOG_TXN_KEYS_Stage5';

--AEO-771 BEGIN
    EXECUTE IMMEDIATE 'truncate table LW_txnheader_Wrk5';

    EXECUTE IMMEDIATE 'truncate table LW_txndetailitem_Wrk5';
--AEO-771 END

  END Clear_Stage;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Hist_Tlog05_ByJobNumber(p_Dummy VARCHAR2,
                                    p_JobNumber NUMBER,
                                    Retval  IN OUT Rcursor) AS
    /*
      this process will move staged data to the history tables then truncate the staging tables.
      This should be called after each sucessful run of the tlog process... do not run it after an incomplete run.
    */
    v_jobNumber  NUMBER := nvl(p_JobNumber,-1);
  BEGIN
    log_unprocessed_Tlog05 (p_JobNumber => p_JobNumber);
    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNDETAIL');

    INSERT INTO Ats_Historytxndetail
      (a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
       a_Dtlquantity,
       a_Dtldiscountamount,
       a_Dtlclearanceitem,
       a_Dtldatemodified,
       a_Reconcilestatus,
       a_Txnheaderid,
       a_Txndetailid,
       a_Brandid,
       a_Fileid,
       a_Processid,
       a_Filelineitem,
       a_Cardid,
       a_Creditcardid,
       a_Txnloyaltyid,
       a_Txnmaskid,
       a_Txnnumber,
       a_Txndate,
       a_Txndatemodified,
       a_Txnregisternumber,
       a_Txnstoreid,
       a_Storenumber,
       a_Txntype,
       a_Txnamount,
       a_Txnqualpurchaseamt,
       a_Txndiscountamount,
       a_Txnemailaddress,
       a_Txnphonenumber,
       a_Txnemployeeid,
       a_Txnchannelid,
       a_Txnoriginaltxnrowkey,
       a_Txncreditsused,
       a_Dtlitemlinenbr,
       a_Dtlproductid,
       a_Dtltypeid,
       a_Dtlactionid,
       a_Dtlretailamount,
       a_Dtlsaleamount,
       a_Dtlclasscode,
       a_Shipdate,
       a_Ordernumber,
       a_Skunumber,
       a_Vckey,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
      SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             t.Dtlquantity,
             t.Dtldiscountamount,
             t.Dtlclearanceitem,
             t.Dtldatemodified,
             t.Reconcilestatus,
             t.Txnheaderid,
             t.Txndetailid,
             t.Brandid,
             t.Fileid,
             CASE
               WHEN t.Processid = 0 THEN
                Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = t.Txnheaderid),
                    t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Filelineitem,
             t.Cardid,
             t.Creditcardid,
             t.Txnloyaltyid,
             t.Txnmaskid,
             t.Txnnumber,
             t.Txndate,
             t.Txndatemodified,
             t.Txnregisternumber,
             t.Txnstoreid,
             t.Storenumber,
             t.Txntypeid,
             t.Txnamount,
             t.Txnqualpurchaseamt,
             t.Txndiscountamount,
             t.Txnemailaddress,
             t.Txnphonenumber,
             t.Txnemployeeid,
             t.Txnchannelid,
             t.Txnoriginaltxnrowkey,
             t.Txncreditsused,
             t.Dtlitemlinenbr,
             t.Dtlproductid,
             t.Dtltypeid,
             t.Dtlactionid,
             t.Dtlretailamount,
             t.Dtlsaleamount,
             t.Dtlclasscode,
             t.Shipdate,
             TRIM(t.Ordernumber) AS Ordernumber,
             t.Skunumber,
             t.Vckey,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txndetail_Stage5 t
        --, LOG_TXN_KEYS_Stage5 s
       WHERE 1=1
       --and t.Txnheaderid = s.Txnheaderid
         AND v_JobNumber IN (-1,t.fileid)
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxndetail x
               WHERE t.Rowkey = x.a_Rowkey);


    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNTENDER');

    INSERT INTO Ats_Historytxntender
      (a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
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
       a_Vckey,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
      SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             CASE
               WHEN t.Processid = 0 THEN
                Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = t.Txnheaderid),
                    t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Storeid,
             t.Txndate,
             t.Txnheaderid,
             t.Txntenderid,
             t.Tendertype,
             t.Tenderamount,
             t.Tendercurrency,
             t.Tendertax,
             t.Tendertaxrate,
             t.Vckey,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txntender_Stage5 t
        --, LOG_TXN_KEYS_Stage5 s
       WHERE 1=1
       --and t.Txnheaderid = s.Txnheaderid
         AND v_JobNumber IN (-1,t.fileid)
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxntender x
               WHERE t.Rowkey = x.a_Rowkey);

    Delete From lw_txntender_Stage5 t Where t.fileid = p_JobNumber;

    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNREWARDREDEEM');

    INSERT INTO Ats_Historytxnrewardredeem
      (a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
       a_Processid,
       a_Txnheaderid,
       a_Txndate,
       a_Txndetailid,
       a_Programid,
       a_Certificateredeemtype,
       a_Certificatecode,
       a_Certificatediscountamount,
       a_Vckey,
       a_Txnrewardredeemid,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
      SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             CASE
               WHEN t.Processid = 0 THEN
                Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = t.Txnheaderid),
                    t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Txnheaderid,
             t.Txndate,
             t.Txndetailid,
             t.Programid,
             t.Certificateredeemtype,
             t.Certificatecode,
             t.Certificatediscountamount,
             t.Vckey,
             t.Txnrewardredeemid,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txnrewardredeem_Stage5 t
        --, LOG_TXN_KEYS_Stage5 s
       WHERE 1=1
       --and t.Txnheaderid = s.Txnheaderid
         AND v_JobNumber IN (-1,t.fileid)
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxnrewardredeem x
               WHERE t.Rowkey = x.a_Rowkey);

    Delete From lw_txnrewardredeem_Stage5 t Where t.fileid = p_JobNumber;

    /* data need to be written to hist tables */
    Log_Process_State('ATS_HISTORYTXNDETAILDISCOUNT');

    INSERT INTO Ats_Historytxndetaildiscount
      (a_Rowkey,
       a_Ipcode,
       a_Parentrowkey,
       a_Processid,
       a_Txndiscountid,
       a_Txnheaderid,
       a_Txndate,
       a_Txndetailid,
       a_Discounttype,
       a_Discountamount,
       a_Txnchannel,
       a_Offercode,
       a_Vckey,
       a_Errormessage,
       Statuscode,
       Createdate,
       Updatedate,
       Lastdmlid)
      SELECT t.Rowkey,
             t.Ipcode,
             0 AS Parentrowkey,
             CASE
               WHEN t.Processid = 0 THEN
                Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = t.Txnheaderid),
                    t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             t.Txndiscountid,
             t.Txnheaderid,
             t.Txndate,
             t.Txndetailid,
             t.Discounttype,
             t.Discountamount,
             t.Txnchannel,
             t.Offercode,
             t.Vckey,
             t.Errormessage,
             t.Statuscode,
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM lw_txndetaildiscount_Stage5 t
        --, LOG_TXN_KEYS_Stage5 s
       WHERE 1=1
       --and t.Txnheaderid = s.Txnheaderid
         AND v_JobNumber IN (-1,t.fileid)
         AND NOT EXISTS (SELECT 1
                FROM Ats_Historytxndetaildiscount x
               WHERE t.Rowkey = x.a_Rowkey);

    Delete From lw_txndetaildiscount_Stage5 t Where t.fileid = p_JobNumber;

    Log_Process_State('LOG_TXN_KEYS');

    INSERT INTO Log_Txn_Keys
      (Txnheaderid,
       Store_Nbr,
       Register_Nbr,
       Txn_Date,
       Txn_Nbr,
       First_Rec_Type,
       Processid,
       Ordernumber)   /*AEO-727 changes  here ---------------------------SCJ*/
      SELECT t.Txnheaderid,
             Store_Nbr,
             Register_Nbr,
             Txn_Date,
             Txn_Nbr,
             First_Rec_Type,
             CASE
               WHEN t.Processid = 0 THEN
                Nvl((SELECT Gv_Status_Processed
                      FROM Ats_Txnheader x
                     WHERE x.a_Txnheaderid = t.Txnheaderid),
                    t.Processid)
               ELSE
                t.Processid
             END AS Processid,
             Ordernumber                  /*AEO-727 changes  here ---------------------------SCJ*/
        FROM LOG_TXN_KEYS_Stage5 t,
             (SELECT DISTINCT txnheaderid
              FROM lw_txndetail_Stage5 x
              WHERE v_JobNumber IN (-1,x.fileid)) txn
        WHERE t.txnheaderid = txn.txnheaderid
        AND NOT EXISTS (SELECT 1
                        FROM Log_Txn_Keys x
                        WHERE x.Txnheaderid = t.Txnheaderid);

    DELETE FROM LOG_TXN_KEYS_Stage5 t
    WHERE t.txnheaderid IN (SELECT txn.txnheaderid FROM lw_txndetail_Stage5 txn WHERE txn.fileid = p_JobNumber);

    Delete From lw_txndetail_Stage5 t Where t.fileid = p_JobNumber;
    commit;

    --commented out this statement because txns can come in late
    --    EXECUTE IMMEDIATE 'truncate tablel LOG_TXN_COUNTS';

  END Hist_Tlog05_ByJobNumber;

  PROCEDURE Re_ProcessStagedTlog(p_Dummy VARCHAR2,
                                 Retval  IN OUT Rcursor) AS
  BEGIN

    /* re-pre-process (on dap failure instead of re-staging) */
    UPDATE lw_txndetail_Stage5 t
       SET Processid = Gv_Status_Processed
     WHERE EXISTS (SELECT NULL
              FROM Ats_Txnheader x
             WHERE x.a_Txnheaderid = t.Txnheaderid);
    COMMIT;

    INSERT INTO Log_Txn_Keys
      (Txnheaderid,
       Store_Nbr,
       Register_Nbr,
       Txn_Date,
       Txn_Nbr,
       First_Rec_Type,
       Processid,
       Ordernumber)   /*AEO-727 changes  here ---------------------------SCJ*/
      SELECT Txnheaderid,
             Store_Nbr,
             Register_Nbr,
             Txn_Date,
             Txn_Nbr,
             First_Rec_Type,
             CASE
               WHEN t.Processid = 0 THEN
                Gv_Status_Processed
               ELSE
                t.Processid
             END AS Processid,
             Ordernumber     /*AEO-727 changes here ---------------------------SCJ*/
        FROM LOG_TXN_KEYS_Stage5 t
       WHERE NOT EXISTS (SELECT 1
                FROM Log_Txn_Keys x
               WHERE x.Txnheaderid = t.Txnheaderid)
         AND EXISTS (SELECT NULL
                FROM Ats_Txnheader x
               WHERE x.a_Txnheaderid = t.Txnheaderid);
    COMMIT;
  END Re_ProcessStagedTlog;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to Load  date ************/

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to Load  date ************/
  PROCEDURE write_Stage_Data IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
  BEGIN
    /* dump arrays into stage tables */
    FORALL i IN 1..gv_Detail_Tbl.count
           INSERT INTO Lw_Txndetail_Stage5
             (Rowkey
             ,Ipcode
             ,Vckey
             ,Dtlquantity
             ,Dtldiscountamount
             ,Dtlclearanceitem
             ,Dtldatemodified
             ,Reconcilestatus
             ,Txnheaderid
             ,Txndetailid
             ,Brandid
             ,Fileid
             ,Processid
             ,Filelineitem
             ,Cardid
             ,Creditcardid
             ,Txnloyaltyid
             ,Txnmaskid
             ,Txnnumber
             ,Txndate
             ,Txndatemodified
             ,Txnregisternumber
             ,Txnstoreid
             ,Txntypeid
             ,Txnamount
             ,Txndiscountamount
             ,Txnqualpurchaseamt
             ,Txnemailaddress
             ,Txnphonenumber
             ,Txnemployeeid
             ,Txnchannelid
             ,Txnoriginaltxnrowkey
             ,Txncreditsused
             ,Dtlitemlinenbr
             ,Dtlproductid
             ,Dtltypeid
             ,Dtlactionid
             ,Dtlretailamount
             ,Dtlsaleamount
             ,Dtlclasscode
             ,Errormessage
             ,Shipdate
             ,Ordernumber
             ,Skunumber
             ,Tenderamount
             ,Storenumber
             ,Statuscode
             ,Createdate
             ,Updatedate
             ,Lastdmlid
             ,Nonmember
             ,TxnOriginalStoreId
             ,TxnOriginalTxnDate
             ,TxnOriginalTxnNumber
             ,CurrencyRate                    ---------------AEO-844 changes here
             ,CurrencyCode                    ---------------AEO-844 changes here
             ,DTLSALEAmount_org               ---------------AEO-844 changes here
             ,AECCMultiplier                   /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             ,OriginalOrderNumber               /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             ,CashierNumber                     ---AEO-1533
             )
             select            
             Gv_Detail_Tbl(i).Rowkey
             ,Gv_Detail_Tbl(i).Ipcode
             ,Gv_Detail_Tbl(i).Vckey
             ,Gv_Detail_Tbl(i).Dtlquantity
             ,Gv_Detail_Tbl(i).Dtldiscountamount
             ,Gv_Detail_Tbl(i).Dtlclearanceitem
             ,Gv_Detail_Tbl(i).Dtldatemodified
             ,Gv_Detail_Tbl(i).Reconcilestatus
             ,Gv_Detail_Tbl(i).Txnheaderid
             ,Gv_Detail_Tbl(i).Txndetailid
             ,Gv_Detail_Tbl(i).Brandid
             ,Gv_Detail_Tbl(i).Fileid
             ,Gv_Detail_Tbl(i).Processid
             ,Gv_Detail_Tbl(i).Filelineitem
             ,Gv_Detail_Tbl(i).Cardid
             ,Gv_Detail_Tbl(i).Creditcardid
             ,Gv_Detail_Tbl(i).Txnloyaltyid
             ,Gv_Detail_Tbl(i).Txnmaskid
             ,Gv_Detail_Tbl(i).Txnnumber
             ,Gv_Detail_Tbl(i).Txndate
             ,Gv_Detail_Tbl(i).Txndatemodified
             ,Gv_Detail_Tbl(i).Txnregisternumber
             ,Gv_Detail_Tbl(i).Txnstoreid
             ,Gv_Detail_Tbl(i).Txntypeid
             ,Gv_Detail_Tbl(i).Txnamount
             ,Gv_Detail_Tbl(i).Txndiscountamount
             ,Gv_Detail_Tbl(i).Txnqualpurchaseamt
             ,Gv_Detail_Tbl(i).Txnemailaddress
             ,Gv_Detail_Tbl(i).Txnphonenumber
             ,Gv_Detail_Tbl(i).Txnemployeeid
             ,Gv_Detail_Tbl(i).Txnchannelid
             ,Gv_Detail_Tbl(i).Txnoriginaltxnrowkey
             ,Gv_Detail_Tbl(i).Txncreditsused
             ,Gv_Detail_Tbl(i).Dtlitemlinenbr
             ,Gv_Detail_Tbl(i).Dtlproductid
             ,Gv_Detail_Tbl(i).Dtltypeid
             ,Gv_Detail_Tbl(i).Dtlactionid
             ,Gv_Detail_Tbl(i).Dtlretailamount
             ,Gv_Detail_Tbl(i).Dtlsaleamount
             ,Gv_Detail_Tbl(i).Dtlclasscode
             ,Gv_Detail_Tbl(i).Errormessage
             ,Gv_Detail_Tbl(i).Shipdate
             ,UPPER(Gv_Detail_Tbl(i).Ordernumber)
             ,Gv_Detail_Tbl(i).Skunumber
             ,Gv_Detail_Tbl(i).Tenderamount
             ,Gv_Detail_Tbl(i).Storenumber
             ,Gv_Detail_Tbl(i).Statuscode
             ,Gv_Detail_Tbl(i).Createdate
             ,Gv_Detail_Tbl(i).Updatedate
             ,Gv_Detail_Tbl(i).Lastdmlid
             ,Gv_Detail_Tbl(i).Nonmember
             ,Gv_Detail_Tbl(i).TxnOriginalStoreId
             ,Gv_Detail_Tbl(i).TxnOriginalTxnDate
             ,Gv_Detail_Tbl(i).TxnOriginalTxnNumber
             ,Gv_Detail_Tbl(i).CURRENCY_Rate                         --------------AEO-844 changes here
             ,Gv_Detail_Tbl(i).CURRENCY_CODE                          --------------AEO-844 changes here
             ,Gv_Detail_Tbl(i).dtlsaleamount_org                       --------------AEO-844 changes here
             ,Gv_Detail_Tbl(i).AECCMultiplier                           /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             ,Gv_Detail_Tbl(i).OriginalOrderNumber                       /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             ,Gv_Detail_Tbl(i).CashierNumber
              from dual where Gv_Detail_Tbl(i).Processid <> 12 ;
              
    FORALL i IN 1..gv_Tender_Tbl.count
        INSERT INTO Lw_Txntender_Stage5
             (Rowkey
             ,Ipcode
             ,Vckey
             ,Processid
             ,Storeid
             ,Txndate
             ,Txnheaderid
             ,Txntenderid
             ,Tendertype
             ,Tenderamount
             ,Tendercurrency
             ,Tendertax
             ,Tendertaxrate
             ,Errormessage
             ,Fileid
             ,Statuscode
             ,Createdate
             ,Updatedate
             ,Lastdmlid)
           select
             Gv_Tender_Tbl(i).Rowkey
             ,Gv_Tender_Tbl(i).Ipcode
             ,Gv_Tender_Tbl(i).Vckey
             ,Gv_Tender_Tbl(i).Processid
             ,Gv_Tender_Tbl(i).Storeid
             ,Gv_Tender_Tbl(i).Txndate
             ,Gv_Tender_Tbl(i).Txnheaderid
             ,Gv_Tender_Tbl(i).Txntenderid
             ,Gv_Tender_Tbl(i).Tendertype
             ,Gv_Tender_Tbl(i).Tenderamount
             ,Gv_Tender_Tbl(i).Tendercurrency
             ,Gv_Tender_Tbl(i).Tendertax
             ,Gv_Tender_Tbl(i).Tendertaxrate
             ,Gv_Tender_Tbl(i).Errormessage
             ,Gv_Tender_Tbl(i).Fileid
             ,Gv_Tender_Tbl(i).Statuscode
             ,Gv_Tender_Tbl(i).Createdate
             ,Gv_Tender_Tbl(i).Updatedate
             ,Gv_Tender_Tbl(i).Lastdmlid from dual 
             where is_duplicated(to_number(nvl(Gv_Tender_Tbl(i).txnheaderid,-1))) = 0  ;
   
    FORALL i IN 1..gv_Discount_Tbl.count
           INSERT INTO Lw_Txndetaildiscount_Stage5
             (Rowkey
             ,Ipcode
             ,Vckey
             ,Processid
             ,Txndiscountid
             ,Txnheaderid
             ,Txndate
             ,Txndetailid
             ,Discounttype
             ,Discountamount
             ,Txnchannel
             ,Offercode
             ,Errormessage
             ,Fileid
             ,Statuscode
             ,Createdate
             ,Updatedate
             ,Lastdmlid)
           select
             Gv_Discount_Tbl(i).Rowkey
             ,Gv_Discount_Tbl(i).Ipcode
             ,Gv_Discount_Tbl(i).Vckey
             ,Gv_Discount_Tbl(i).Processid
             ,Gv_Discount_Tbl(i).Txndiscountid
             ,Gv_Discount_Tbl(i).Txnheaderid
             ,Gv_Discount_Tbl(i).Txndate
             ,Gv_Discount_Tbl(i).Txndetailid
             ,Gv_Discount_Tbl(i).Discounttype
             ,Gv_Discount_Tbl(i).Discountamount
             ,Gv_Discount_Tbl(i).Txnchannel
             ,Gv_Discount_Tbl(i).Offercode
             ,Gv_Discount_Tbl(i).Errormessage
             ,Gv_Discount_Tbl(i).Fileid
             ,Gv_Discount_Tbl(i).Statuscode
             ,Gv_Discount_Tbl(i).Createdate
             ,Gv_Discount_Tbl(i).Updatedate
             ,Gv_Discount_Tbl(i).Lastdmlid from dual
            where is_duplicated(to_number(nvl(Gv_discount_Tbl(i).txnheaderid,-1))) = 0  ;
             
    FORALL i IN 1..gv_Reward_Tbl.count
           INSERT INTO Lw_Txnrewardredeem_Stage5
             (Rowkey
             ,Ipcode
             ,Vckey
             ,Txnheaderid
             ,Txndate
             ,Txndetailid
             ,Programid
             ,Certificateredeemtype
             ,Certificatecode
             ,Certificatediscountamount
             ,Errormessage
             ,Txnrewardredeemid
             ,Processid
             ,Fileid
             ,Statuscode
             ,Createdate
             ,Updatedate
             ,Lastdmlid)
           select
             Gv_Reward_Tbl(i).Rowkey
             ,Gv_Reward_Tbl(i).Ipcode
             ,Gv_Reward_Tbl(i).Vckey
             ,Gv_Reward_Tbl(i).Txnheaderid
             ,Gv_Reward_Tbl(i).Txndate
             ,Gv_Reward_Tbl(i).Txndetailid
             ,0 --Gv_Reward_Tbl(i).Programid
             ,Gv_Reward_Tbl(i).Certificateredeemtype
             ,Gv_Reward_Tbl(i).Certificatecode
             ,Gv_Reward_Tbl(i).Certificatediscountamount
             ,Gv_Reward_Tbl(i).Errormessage
             ,Gv_Reward_Tbl(i).Txnrewardredeemid
             ,Gv_Reward_Tbl(i).Processid
             ,Gv_Reward_Tbl(i).Fileid
             ,Gv_Reward_Tbl(i).Statuscode
             ,Gv_Reward_Tbl(i).Createdate
             ,Gv_Reward_Tbl(i).Updatedate
             ,Gv_Reward_Tbl(i).Lastdmlid from dual
          where is_duplicated(to_number(nvl(Gv_reward_Tbl(i).txnheaderid,-1))) = 0  ;
    COMMIT;
    /* empty out arrays */
    gv_Detail_Tbl     := Type_Tlog05_Stg_Detail_Tbl();
    gv_Tender_Tbl     := Type_Tlog05_Stg_Tender_Tbl();
    gv_Discount_Tbl   := Type_Tlog05_Stg_Discount_Tbl();
    gv_Reward_Tbl     := Type_Tlog05_Stg_Reward_Tbl();
  END write_Stage_Data;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to Load  date ************/
  PROCEDURE Load_Stage_Data(p_Head_Rec       IN OUT Type_Tlog05_Stg_Header,
                            p_Detail_Tbl     IN Type_Tlog05_Stg_Detail_Tbl,
                            p_Tender_Tbl     IN Type_Tlog05_Stg_Tender_Tbl,
                            p_Discount_Tbl   IN Type_Tlog05_Stg_Discount_Tbl,
                            p_Reward_Tbl     IN Type_Tlog05_Stg_Reward_Tbl,
                            p_My_Log_Id      IN NUMBER,
                            p_Valid_01_Count IN NUMBER,
                            p_Txn_Count      IN OUT NUMBER,
                            p_Store_Nbr      IN VARCHAR2,
                            p_Rec_Type       IN OUT VARCHAR2,
                            p_Action         IN OUT VARCHAR2) IS

    v_24hr_Txn_Cnt NUMBER := 0;
    v_MexicoStores NUMBER := 0;
    v_StoreNumber NUMBER := 0;
    p_txnamount FLOAT ;  /*National Rollout Changes  here        ----------------SCJ*/
    v_qualamount NUMBER := 0; -- AEO-1748


    /*
      This subroutine will receive the arrays from the main process and peform
      the final processes then insert into the staging tables.

      This is done here so the code doesn't have to be replicated... nature of the looping
      requires this to occur in the loop and once the loop completed.
    */
  BEGIN
    IF p_Valid_01_Count = 0
    THEN
      /* <************************************************* REVIEW THIS */
      /* define txn type as giftcard */
      p_Head_Rec.Txntype      := 5;
      p_Head_Rec.Errormessage := 'valid 01 record not found';
    END IF;

    IF Nvl(p_Head_Rec.Ttltenderamount, 0) != 0
    THEN
      p_Head_Rec.Taxrate := Round((Nvl(p_Head_Rec.Ttltaxamount, 0) /
                                  p_Head_Rec.Ttltenderamount),
                                  4);
    END IF;
    /* set non-member vckey to zero */
    IF p_Head_Rec.Vckey IS NULL
    THEN
      p_Head_Rec.Vckey := 0;
    END IF;

    /* dont allow kids77 master or child records processid to change, they are for loggin only */
    IF p_Head_Rec.Processid NOT IN
       (Gv_Status_77child, Gv_Status_77master, Gv_Status_Dup)
    THEN
      IF p_Head_Rec.Processid = Gv_Status_77merged
      THEN
        p_Head_Rec.Processid    := Gv_Status_Ready;
        p_Head_Rec.Errormessage := 'Merged 77';
      END IF;

      IF p_Valid_01_Count = 0
      THEN
        p_Head_Rec.Processid    := Gv_Status_Nolineitem;
        p_Head_Rec.Errormessage := 'No Valid Line Items';
      ELSIF p_Head_Rec.Txnloyaltyid IS NULL
      THEN
        p_Head_Rec.Processid    := Gv_Status_Nocard;
        p_Head_Rec.Errormessage := 'No Card Swipe';
      END IF;

/*
      IF p_Head_Rec.Txnemployeeid IS NOT NULL
      THEN
        p_Head_Rec.Processid := Gv_Status_Employee;
      END IF;
*/
    END IF;

    p_Txn_Count := p_Txn_Count + 1;

    IF p_Head_Rec.Txnheaderid IS NULL
    THEN
      /* only kids77 records will already have headerid assigned
      this is because the kids77 comes in split and we must merge */
      Get_Headerid(p_Store_Nbr => p_Store_Nbr --v_00_rec.STORE_NUMBER --  p_head_rec.altstorenumber
                  ,
                   p_Reg_Nbr   => p_Head_Rec.Txnregisternumber --p_head_rec.alttxnregisternumber
                  ,
                   p_Txn_Date  => p_Head_Rec.Txndate --p_head_rec.alttxndate
                  ,
                   p_Txn_Nbr   => p_Head_Rec.Txnnumber --p_head_rec.alttxnnumber
                  ,
                   p_Processid => p_Head_Rec.Processid,
                   p_Headerid  => p_Head_Rec.Txnheaderid,
                   p_Rec_Type  => p_Rec_Type --v_txt1
                  ,
                   p_Action    => p_Action --v_txt2
                  /*AEO-727 changes begin here ---------------------------SCJ*/
                  ,p_Ordernumber => p_Head_Rec.ordernumber
                  /*AEO-727 changes begin here ---------------------------SCJ*/
                   );
    END IF;

    p_Head_Rec.Rowkey := p_Head_Rec.Txnheaderid;

    /* zero out txn amount if more then 3 txn in 24hrs */
    IF p_Head_Rec.Processid = Gv_Status_Ready AND
       p_Head_Rec.Txnqualpurchaseamt > 0
       /*AEO-85 Exclude onine Order from 3x 24hr rule  -----------SCJ*/
       --  and p_HEAD_REC.Ordernumber is null AEO-1748
        /*AEO-85 Exclude onine Order from 3x 24hr rule  -----------SCJ*/
    THEN /*National Rollout Changes AEO-1218 begin here        ----------------SCJ*/
        p_txnamount := p_Head_Rec.Txnqualpurchaseamt;
      v_24hr_Txn_Cnt := Get_24hr_Txn_Cnt(/*p_Vckey => p_Head_Rec.Vckey,*/
                                         p_ipcode => p_Head_Rec.Ipcode,
                                          p_txnamount => p_txnamount,
                                          p_qualamount => v_qualamount,
                                         p_Date  => p_Head_Rec.Txndate);
      IF v_24hr_Txn_Cnt > 5
      THEN
        p_Head_Rec.Txnqualpurchaseamt := 0;
        p_Head_Rec.Errormessage       := '>5 txns found on txndate';
      END IF;
       IF p_txnamount > 1000
      THEN
        p_Head_Rec.Txnqualpurchaseamt := v_qualamount; -- AEO-1748
        p_Head_Rec.Errormessage       := 'Txn amount > 1000  found on txndate';
      END IF;
    END IF;
/*National Rollout Changes AEO-1218 end here        ----------------SCJ*/
    /*
       RKG - 2/5/2013
       PI 23350 - Update points to zero for Mexico stores
    */

    v_StoreNumber := CAST(p_Store_Nbr as NUMBER);

    Select Count(*) Into v_MexicoStores
    From   lw_storedef st
    Where st.country = 'MEX'
    And   st.storenumber = v_StoreNumber;

    IF v_MexicoStores > 0
      THEN p_Head_Rec.Txnqualpurchaseamt := 0;
    END IF;

    IF p_Head_Rec.Ipcode > 0 AND p_Head_Rec.Org_Txnnbr IS NOT NULL
    THEN
      SELECT MAX(a_Rowkey) /* not unique lookup so grabbing the max */
        INTO p_Head_Rec.Txnoriginaltxnrowkey
        FROM Ats_Txnheader x
       WHERE x.a_Txnstoreid = p_Head_Rec.Org_Txnstoreid
         --AND TRUNC(x.a_Txndate) = TRUNC(p_Head_Rec.Org_Txndate)
         AND X.A_TXNDATE BETWEEN cast(trunc(to_date(p_Head_Rec.Org_Txndate, 'DD-MON-YY')) as timestamp)  AND cast((trunc(to_date(p_Head_Rec.Org_Txndate, 'DD-MON-YY'))+.99999) as timestamp)
         AND x.a_Txnnumber = p_Head_Rec.Org_Txnnbr
         AND x.a_Vckey IN
             (SELECT /*+ cardinality (vc 2) */
               Vckey
                FROM Lw_Virtualcard Vc
               WHERE Vc.Ipcode = p_Head_Rec.Ipcode);
    END IF;
/*National Rollout Changes AEO-1218 here        ----------------SCJ*/
  IF p_Head_Rec.Ipcode > 0 AND p_Head_Rec.OriginalOrderNumber  IS NOT NULL
    THEN
      SELECT MAX(a_Rowkey) /* not unique lookup so grabbing the max */
        INTO p_Head_Rec.Txnoriginaltxnrowkey
        FROM Ats_Txnheader x
       WHERE 1=1
         AND x.a_ordernumber = p_Head_Rec.OriginalOrderNumber
         AND x.a_Vckey IN
             (SELECT /*+ cardinality (vc 2) */
               Vckey
                FROM Lw_Virtualcard Vc
               WHERE Vc.Ipcode = p_Head_Rec.Ipcode);
    END IF;
  /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
    /* comment this code out because we are going to check the returns for the stage table in the DAP Interceptor
       and if that doesn't exist then we will set the qualpurchamount = 0

    IF p_Head_Rec.Txntype = 2 -- return
       AND Nvl(p_Head_Rec.Txnoriginaltxnrowkey, 0) = 0
    THEN
      -- only ding returns if the member got credit for the original sale
      p_Head_Rec.Txnqualpurchaseamt := 0;
    END IF;
    */
    IF p_Detail_Tbl.Count > 0 THEN
      FOR i IN 1..p_Detail_Tbl.count LOOP

        gv_Detail_Tbl.extend;
        gv_Detail_Tbl(gv_Detail_Tbl.count) :=
                         Type_Tlog05_Stg_Detail(/* rowkey               */p_Detail_Tbl(i).Rowkey,
                                                /* ipcode               */ p_Head_Rec.Ipcode,
                                                /* vckey                */ p_Head_Rec.Vckey,
                                                /* parentrowkey         */ NULL,
                                                /* dtlquantity          */ p_Detail_Tbl(i).Dtlquantity,
                                                /* dtldiscountamount    */ p_Detail_Tbl(i).Dtldiscountamount,
                                                /* dtlclearanceitem     */ Nvl(p_Detail_Tbl(i).Dtlclearanceitem, 0),
                                                /* dtldatemodified      */ p_Detail_Tbl(i).Dtldatemodified,
                                                /* reconcilestatus      */ p_Detail_Tbl(i).Reconcilestatus,
                                                /* txnheaderid          */ p_Head_Rec.Rowkey,
                                                /* txndetailid          */ p_Detail_Tbl(i).Rowkey,
                                                /* brandid              */ Nvl(p_Detail_Tbl(i).Brandid, p_Head_Rec.Brandid),
                                                /* fileid               */ p_My_Log_Id,
                                                /* processid            */ CASE WHEN p_Head_Rec.Processid IN (Gv_Status_77child, Gv_Status_77master, Gv_Status_Dup)
                                                                                THEN  p_Head_Rec.Processid
                                                                                ELSE  Nvl(p_Detail_Tbl(i).Processid, p_Head_Rec.Processid)
                                                                               END,
                                                 /* filelineitem         */p_Detail_Tbl(i).Filelineitem,
                                                 /* cardid               */p_Detail_Tbl(i).Cardid,
                                                 /* creditcardid         */p_Detail_Tbl(i).Creditcardid,
                                                 /* txnloyaltyid         */p_Head_Rec.Txnloyaltyid,
                                                 /* txnmaskid            */p_Head_Rec.Txnmaskid,
                                                 /* txnnumber            */p_Head_Rec.Txnnumber,
                                                 /* txndate              */p_Head_Rec.Txndate,
                                                 /* txndatemodified      */p_Detail_Tbl(i).Txndatemodified,
                                                 /* txnregisternumber    */p_Head_Rec.Txnregisternumber,
                                                 /* txnstoreid           */p_Head_Rec.Txnstoreid,
                                                 /* txntype              */p_Head_Rec.Txntype,
                                                 /* txnamount            */Nvl(p_Head_Rec.Txnamount, 0),
                                                 /* txndiscountamount    */Nvl(p_Head_Rec.Txndiscountamount, 0),
                                                 /* txnemailaddress      */p_Head_Rec.Txnemailaddress,
                                                 /* txnphonenumber       */p_Head_Rec.Txnphonenumber,
                                                 /* txnemployeeid        */p_Head_Rec.Txnemployeeid,
                                                 /* txnchannelid         */p_Head_Rec.Txnchannelid,
                                                 /* txnoriginaltxnrowkey */p_Head_Rec.Txnoriginaltxnrowkey,
                                                 /* txncreditsused       */Nvl(p_Head_Rec.Txncreditsused, 0),
                                                 /* dtlitemlinenbr       */p_Detail_Tbl(i).Dtlitemlinenbr,
                                                 /* dtlproductid         */Nvl(p_Detail_Tbl(i).Dtlproductid, 0),
                                                 /* dtltypeid            */p_Detail_Tbl(i).Dtltypeid,
                                                 /* dtlactionid          */p_Detail_Tbl(i).Dtlactionid,
                                                 /* dtlretailamount      */p_Detail_Tbl(i).Dtlretailamount,
                                                 /* dtlsaleamount        */p_Detail_Tbl(i).Dtlsaleamount,
                                                 /* dtlsaleamount_org    */p_Detail_Tbl(i).Dtlsaleamount_org,  ---------------AEO-844 changes HERE -scj
                                                 /* CURRENCY_CODE         */p_Head_Rec.CURRENCY_CODE,      ---------------AEO-844 changes HERE -scj
                                                 /* CURRENCY_rate         */p_Head_Rec.CURRENCY_RATE,       ---------------AEO-844 changes HERE -scj
                                                 /* errormessage         */Nvl(p_Detail_Tbl(i).Errormessage, p_Head_Rec.Errormessage),
                                                 /* shipdate             */p_Head_Rec.Shipdate,
                                                 /* statuscode           */p_Detail_Tbl(i).Statuscode,
                                                 /* createdate           */SYSDATE,
                                                 /* updatedate           */SYSDATE,
                                                 /* lastdmlid            */NULL,
                                                 /* dtlclasscode         */p_Detail_Tbl(i).Dtlclasscode,
                                                 /* skunumber            */p_Detail_Tbl(i).Skunumber,
                                                 /* NONMEMBER            */0,
                                                 /* STORENUMBER          */p_Head_Rec.Txnstorenumber,
                                                 /* ORDERNUMBER          */TRIM(p_Head_Rec.Ordernumber),
                                                 /* TENDERAMOUNT         */Nvl(p_Head_Rec.Ttltenderamount, 0),
                                                 /* TXNQUALPURCHASEAMT   */Round(p_Head_Rec.Txnqualpurchaseamt, 0),
                                                 /* TXNTYPEID            */p_Head_Rec.Txntype,
                                                 /* TxnOriginalStoreId   */p_Head_Rec.Org_Txnstoreid,
                                                 /* TxnOriginalTxnDate   */p_Head_Rec.Org_Txndate,
                                                 /* TxnOriginalTxnNumber */p_Head_Rec.Org_Txnnbr,
                                                 /*AECCMultiplier        */p_Head_Rec.AECCMultiplier,   /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                                                 /*OriginalOrderNumber   */p_Head_Rec.OriginalOrderNumber, /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                                                 /*CashierNumber         */p_Head_Rec.CashierNumber  ); /*AEO-1533 LPP*/

      END LOOP;
    --AEO-629 - BEGIN
    ELSIF p_Detail_Tbl.Count = 0 AND v_StoreNumber = 659 AND
          p_Head_Rec.Processid NOT IN (Gv_Status_77child, Gv_Status_77master) THEN
      /*fake detail for any transaction from store number 659(online) that do not have a 01 record type(datil record)*/
      gv_Detail_Tbl.extend;
      gv_Detail_Tbl(gv_Detail_Tbl.count) :=
                       Type_Tlog05_Stg_Detail(/* rowkey               */ Seq_Rowkey.Nextval,
                                              /* ipcode               */       p_Head_Rec.Ipcode,
                                              /* vckey                */       p_Head_Rec.Vckey,
                                              /* parentrowkey         */       NULL,
                                              /* dtlquantity          */       0,
                                              /* dtldiscountamount    */       0,
                                              /* dtlclearanceitem     */       0,
                                              /* dtldatemodified      */       NULL,
                                              /* reconcilestatus      */       NULL,
                                              /* txnheaderid          */       p_Head_Rec.Txnheaderid ,
                                              /* txndetailid          */       Seq_Rowkey.Currval,
                                              /* brandid              */       p_Head_Rec.Brandid,
                                              /* fileid               */       p_My_Log_Id,
                                              /* processid            */       0,
                                              /* filelineitem         */       p_Head_Rec.Filelineitem,
                                              /* cardid               */       NULL,
                                              /* creditcardid         */       NULL,
                                              /* txnloyaltyid         */       p_Head_Rec.Txnloyaltyid,
                                              /* txnmaskid            */       p_Head_Rec.Txnmaskid,
                                              /* txnnumber            */       p_Head_Rec.Txnnumber,
                                              /* txndate              */       p_Head_Rec.Txndate,
                                              /* txndatemodified      */       NULL,
                                              /* txnregisternumber    */       p_Head_Rec.Txnregisternumber,
                                              /* txnstoreid           */       p_Head_Rec.Txnstoreid,
                                              /* txntype              */       8,
                                              /* txnamount            */       Nvl(p_Head_Rec.Txnamount, 0),
                                              /* txndiscountamount    */       Nvl(p_Head_Rec.Txndiscountamount, 0),
                                              /* txnemailaddress      */       p_Head_Rec.Txnemailaddress,
                                              /* txnphonenumber       */       p_Head_Rec.Txnphonenumber,
                                              /* txnemployeeid        */       p_Head_Rec.Txnemployeeid,
                                              /* txnchannelid         */       p_Head_Rec.Txnchannelid,
                                              /* txnoriginaltxnrowkey */       p_Head_Rec.Txnoriginaltxnrowkey,
                                              /* txncreditsused       */       Nvl(p_Head_Rec.Txncreditsused, 0),
                                              /* dtlitemlinenbr       */       0,
                                              /* dtlproductid         */       0,
                                              /* dtltypeid            */       NULL,
                                              /* dtlactionid          */       NULL,
                                              /* dtlretailamount      */       0,
                                              /* dtlsaleamount        */       0,
                                              /* dtlsaleamount_org    */       0,  ---------------AEO-844 changes HERE -scj
                                              /* CURRENCY_CODE        */       NULL,      ---------------AEO-844 changes HERE -scj
                                              /* CURRENCY_rate        */       NULL,       ---------------AEO-844 changes HERE -scj
                                              /* errormessage         */       'Fake detail to support BOSS Order w/out details ',
                                              /* shipdate             */       p_Head_Rec.Shipdate,
                                              /* statuscode           */       NULL,
                                              /* createdate           */       SYSDATE,
                                              /* updatedate           */       SYSDATE,
                                              /* lastdmlid            */       NULL,
                                              /* dtlclasscode         */       NULL,
                                              /* skunumber            */       NULL,
                                              /* NONMEMBER            */       0,
                                              /* STORENUMBER          */       p_Head_Rec.Txnstorenumber,
                                              /* ORDERNUMBER          */       TRIM(p_Head_Rec.Ordernumber),
                                              /* TENDERAMOUNT         */       Nvl(p_Head_Rec.Ttltenderamount, 0),
                                              /* TXNQUALPURCHASEAMT   */       p_Head_Rec.Txnqualpurchaseamt,
                                              /* TXNTYPEID            */       8,
                                              /* TxnOriginalStoreId   */       p_Head_Rec.Org_Txnstoreid,
                                              /* TxnOriginalTxnDate   */       p_Head_Rec.Org_Txndate,
                                              /* TxnOriginalTxnNumber */       p_Head_Rec.Org_Txnnbr,
                                              /*AECCMultiplier        */       p_Head_Rec.AECCMultiplier,   /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                                              /*OriginalOrderNumber   */       p_Head_Rec.OriginalOrderNumber, /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                                              /*CashierNumber         */       p_Head_Rec.CashierNumber  ); /*AEO-1533 LPP*/
    --AEO-629 - END
    ELSIF p_Detail_Tbl.Count = 0 AND
          p_Head_Rec.Processid IN (Gv_Status_77child, Gv_Status_77master)  THEN

    /* need to store a fake detail rec so this can be matched up with rest of txn later. */
      gv_Detail_Tbl.extend;
      gv_Detail_Tbl(gv_Detail_Tbl.count) :=
                       Type_Tlog05_Stg_Detail(/* rowkey               */ Seq_Rowkey.Nextval,
                                              /* ipcode               */       p_Head_Rec.Ipcode,
                                              /* vckey                */       p_Head_Rec.Vckey,
                                              /* parentrowkey         */       NULL,
                                              /* dtlquantity          */       0,
                                              /* dtldiscountamount    */       0,
                                              /* dtlclearanceitem     */       0,
                                              /* dtldatemodified      */       NULL,
                                              /* reconcilestatus      */       NULL,
                                              /* txnheaderid          */       p_Head_Rec.Txnheaderid ,
                                              /* txndetailid          */       Seq_Rowkey.Currval,
                                              /* brandid              */       p_Head_Rec.Brandid,
                                              /* fileid               */       p_My_Log_Id,
                                              /* processid            */       p_Head_Rec.Processid,
                                              /* filelineitem         */       p_Head_Rec.Filelineitem,
                                              /* cardid               */       NULL,
                                              /* creditcardid         */       NULL,
                                              /* txnloyaltyid         */       p_Head_Rec.Txnloyaltyid,
                                              /* txnmaskid            */       p_Head_Rec.Txnmaskid,
                                              /* txnnumber            */       p_Head_Rec.Txnnumber,
                                              /* txndate              */       p_Head_Rec.Txndate,
                                              /* txndatemodified      */       NULL,
                                              /* txnregisternumber    */       p_Head_Rec.Txnregisternumber,
                                              /* txnstoreid           */       p_Head_Rec.Txnstoreid,
                                              /* txntype              */       p_Head_Rec.Txntype,
                                              /* txnamount            */       Nvl(p_Head_Rec.Txnamount, 0),
                                              /* txndiscountamount    */       Nvl(p_Head_Rec.Txndiscountamount, 0),
                                              /* txnemailaddress      */       p_Head_Rec.Txnemailaddress,
                                              /* txnphonenumber       */       p_Head_Rec.Txnphonenumber,
                                              /* txnemployeeid        */       p_Head_Rec.Txnemployeeid,
                                              /* txnchannelid         */       p_Head_Rec.Txnchannelid,
                                              /* txnoriginaltxnrowkey */       p_Head_Rec.Txnoriginaltxnrowkey,
                                              /* txncreditsused       */       Nvl(p_Head_Rec.Txncreditsused, 0),
                                              /* dtlitemlinenbr       */       0,
                                              /* dtlproductid         */       0,
                                              /* dtltypeid            */       NULL,
                                              /* dtlactionid          */       NULL,
                                              /* dtlretailamount      */       0,
                                              /* dtlsaleamount        */       0,
                                              /* dtlsaleamount_org    */       0,  ---------------AEO-844 changes HERE -scj
                                              /* CURRENCY_CODE        */       NULL,      ---------------AEO-844 changes HERE -scj
                                              /* CURRENCY_rate        */       NULL,       ---------------AEO-844 changes HERE -scj
                                              /* errormessage         */       'Fake detail to support 77kid w/out details ',
                                              /* shipdate             */       p_Head_Rec.Shipdate,
                                              /* statuscode           */       NULL,
                                              /* createdate           */       SYSDATE,
                                              /* updatedate           */       SYSDATE,
                                              /* lastdmlid            */       NULL,
                                              /* dtlclasscode         */       NULL,
                                              /* skunumber            */       NULL,
                                              /* NONMEMBER            */       0,
                                              /* STORENUMBER          */       NULL,
                                              /* ORDERNUMBER          */       TRIM(p_Head_Rec.Ordernumber),
                                              /* TENDERAMOUNT         */       Nvl(p_Head_Rec.Ttltenderamount, 0),
                                              /* TXNQUALPURCHASEAMT   */       p_Head_Rec.Txnqualpurchaseamt,
                                              /* TXNTYPEID            */       p_Head_Rec.Txntype,
                                              /* TxnOriginalStoreId   */       p_Head_Rec.Org_Txnstoreid,
                                              /* TxnOriginalTxnDate   */       p_Head_Rec.Org_Txndate,
                                              /* TxnOriginalTxnNumber */       p_Head_Rec.Org_Txnnbr,
                                              /*AECCMultiplier        */       p_Head_Rec.AECCMultiplier,   /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                                              /*OriginalOrderNumber   */       p_Head_Rec.OriginalOrderNumber, /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                                              /*CashierNumber         */       p_Head_Rec.CashierNumber  ); /*AEO-1533 LPP*/
    END IF;
    IF p_Discount_Tbl.Count > 0 THEN
      /* insert dicount records from previous txn */
      FOR i IN 1..p_Discount_Tbl.count LOOP
         gv_Discount_Tbl.extend;
         gv_Discount_Tbl(gv_Discount_Tbl.count) :=
              Type_Tlog05_Stg_Discount    (/* rowkey         */p_Discount_Tbl(i).Rowkey,
                                           /* ipcode         */p_Head_Rec.Ipcode,
                                           /* vckey          */p_Head_Rec.Vckey,
                                           /* processid      */p_Head_Rec.Processid ,
                                           /* txndiscountid  */p_Discount_Tbl(i).Txndiscountid,
                                           /* txnheaderid    */p_Head_Rec.Txnheaderid,
                                           /* txndate        */p_Head_Rec.Txndate,
                                           /* txndetailid    */p_Discount_Tbl(i).Txndetailid,
                                           /* discounttype   */p_Discount_Tbl(i).Discounttype,
                                           /* discountamount */p_Discount_Tbl(i).Discountamount,
                                           /* txnchannel     */p_Discount_Tbl(i).Txnchannel,
                                           /* offercode      */p_Discount_Tbl(i).Offercode,
                                           /* errormessage   */Nvl(p_Discount_Tbl(i).Errormessage, p_Head_Rec.Errormessage),
                                           /* statuscode     */p_Discount_Tbl(i).Statuscode,
                                           /* createdate     */SYSDATE,
                                           /* updatedate     */SYSDATE,
                                           /* lastdmlid      */ NULL,
                                           /* fileid         */ p_My_Log_Id);
      END LOOP;
    END IF;

    IF p_Reward_Tbl.Count > 0 THEN
      FOR i IN 1..p_Reward_Tbl.count LOOP
      /* insert redeem records from previous txn */
        gv_Reward_Tbl.extend;
        gv_Reward_Tbl(gv_Reward_Tbl.count) :=
                     Type_Tlog05_Stg_Reward( /*rowkey                    */p_Reward_Tbl(i).Rowkey,
                                             /*ipcode                    */p_Head_Rec.Ipcode,
                                             /*vckey                     */p_Head_Rec.Vckey,
                                             /*processid                 */p_Head_Rec.Processid,
                                             /*txnrewardredeemid         */p_Reward_Tbl(i).Txnrewardredeemid,
                                             /*txnheaderid               */p_Head_Rec.Txnheaderid,
                                             /*txndate                   */p_Head_Rec.Txndate,
                                             /*txndetailid               */p_Reward_Tbl(i).Txndetailid,
                                             /*programid                 */p_Reward_Tbl(i).Programid,
                                             /*certificateredeemtype     */p_Reward_Tbl(i).Certificateredeemtype,
                                             /*certificatecode           */p_Reward_Tbl(i).Certificatecode,
                                             /*certificatediscountamount */p_Reward_Tbl(i).Certificatediscountamount,
                                             /*errormessage              */Nvl(p_Reward_Tbl(i).Errormessage, p_Head_Rec.Errormessage),
                                             /*statuscode                */p_Reward_Tbl(i).Statuscode,
                                             /*createdate                */SYSDATE,
                                             /*updatedate                */SYSDATE,
                                             /*lastdmlid                 */NULL,
                                             /*FILEID                    */p_My_Log_Id);
      END LOOP;
    END IF;
    IF p_Tender_Tbl.Count > 0 THEN
      FOR i IN 1..p_Tender_Tbl.count LOOP
      /* insert tenders from previous txn */
        gv_Tender_Tbl.extend;
        gv_Tender_Tbl(gv_Tender_Tbl.count) :=
                     Type_Tlog05_Stg_Tender( /* rowkey         */p_Tender_Tbl(i).Rowkey,
                                             /* ipcode         */p_Head_Rec.Ipcode,
                                             /* vckey          */p_Head_Rec.Vckey,
                                             /* processid      */p_Head_Rec.Processid,
                                             /* storeid        */p_Head_Rec.Txnstoreid,
                                             /* txndate        */p_Head_Rec.Txndate,
                                             /* txnheaderid    */p_Head_Rec.Txnheaderid,
                                             /* txntenderid    */p_Tender_Tbl(i).Txntenderid,
                                             /* tendertype     */p_Tender_Tbl(i).Tendertype,
                                             /* tenderamount   */p_Tender_Tbl(i).Tenderamount,
                                             /* tendercurrency */p_Tender_Tbl(i).Tendercurrency,
                                             /* tendertax      */Round(p_Tender_Tbl(i).Tenderamount * p_Head_Rec.Taxrate, 2),
                                             /* tendertaxrate  */p_Head_Rec.Taxrate,
                                             /* errormessage   */Nvl(p_Tender_Tbl(i).Errormessage, p_Head_Rec.Errormessage),
                                             /* statuscode     */p_Tender_Tbl(i).Statuscode,
                                             /* createdate     */SYSDATE,
                                             /* updatedate     */SYSDATE,
                                             /* lastdmlid      */NULL,
                                             /* fileid         */p_My_Log_Id);

      END LOOP;
    ELSE
      /* write a fake tender if one didn't exist */
      gv_Tender_Tbl.extend;
      gv_Tender_Tbl(gv_Tender_Tbl.count) :=
                   Type_Tlog05_Stg_Tender( /* rowkey         */Seq_Rowkey.Nextval,
                                           /* ipcode         */p_Head_Rec.Ipcode,
                                           /* vckey          */p_Head_Rec.Vckey,
                                           /* processid      */p_Head_Rec.Processid,
                                           /* storeid        */p_Head_Rec.Txnstoreid,
                                           /* txndate        */p_Head_Rec.Txndate,
                                           /* txnheaderid    */p_Head_Rec.Txnheaderid,
                                           --/* txntenderid    */Tlog02_Id.Nextval,---------------AEO 250 fix ------------------SCJ
                                           /* txntenderid    */Seq_Rowkey.Nextval,
                                           /* tendertype     */60, /* default cash? */
                                           /* tenderamount   */0,
                                           /* tendercurrency */NULL,
                                           /* tendertax      */0 ,
                                           /* tendertaxrate  */0,
                                           /* errormessage   */p_Head_Rec.Errormessage,
                                           /* statuscode     */NULL,
                                           /* createdate     */SYSDATE,
                                           /* updatedate     */SYSDATE,
                                           /* lastdmlid      */NULL,
                                           /* fileid         */p_My_Log_Id);
    END IF;
  END Load_Stage_Data;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/

  PROCEDURE Stage_Tlog05(p_Filename   VARCHAR2 DEFAULT NULL,
                         p_Test_Count NUMBER DEFAULT 0,
                         Retval       IN OUT Rcursor) IS

    /* version 1.0

       v_this_line_cnt >0 means active transaction processing
       v_this_line_cnt =0 means looking for next txn, current ended or was rejected.
           reject reasons
               voided record
               txn records dont match headed defined count (to high or low)
       00 - header record
       01 - item record (many to 00)
       02 - ttl amount (one to 00)
       03 - ttl sales tax (one to 00)
       04 - tender record (many to 00)
       05 - gift certificate used (take off 02(ttl amount) to give qp)
    ** 14 - found order (dont really understand this yet, one to 00)
       18 - ttl discount (one to 00)
    ** 99 - kids77 gift card sale (configure later)
       27-33 - original txn info (on return txn, one to 00)
       27-51 - gift card redemption (one to 00)
       27-62 - gift card purchase (many to 00)
    ** 27-77 - kids77, define later
       27-93 - loyalty id, (one to 00)

    */
    /* raw data records */
    v_00_Rec   Type_Tlog05_00header;
    v_01_Rec   Type_Tlog05_01item;
    v_02_Rec   Type_Tlog05_02subtotal;
    v_03_Rec   Type_Tlog05_03salestax;
    v_04_Rec   Type_Tlog05_04tender;
    v_05_Rec   Type_Tlog05_05giftcert;
    v_14_Rec   Type_Tlog05_14foundorder;
    v_15_Rec   Type_Tlog05_15ceditcrdcheck;
    v_18_Rec   Type_Tlog05_18txndiscount;
    v_2733_Rec Type_Tlog05_2733origtxn;
    v_2751_Rec Type_Tlog05_2751discountrsn;
    v_2762_Rec Type_Tlog05_2762valuecard;
    v_2777_Rec Type_Tlog05_2777multidivmsttxn;
    v_2792_Rec Type_Tlog05_2792pomogiftsku;
    v_2793_Rec Type_Tlog05_2793custloyalyt;
    v_2799_Rec Type_Tlog05_2799directordernbr;
    v_99_Rec   Type_Tlog05_99item;    /*AEO-820 changes here -----------------SCJ*/
    /* records and collections used to stage a single txn */
    v_Head_Rec     Type_Tlog05_Stg_Header;
    v_Detail_Rec   Type_Tlog05_Stg_Detail;
    v_Detail_Tbl   Type_Tlog05_Stg_Detail_Tbl := Type_Tlog05_Stg_Detail_Tbl();
    v_Tender_Rec   Type_Tlog05_Stg_Tender;
    v_Tender_Tbl   Type_Tlog05_Stg_Tender_Tbl := Type_Tlog05_Stg_Tender_Tbl();
    v_Discount_Rec Type_Tlog05_Stg_Discount := Type_Tlog05_Stg_Discount(NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL,
                                                                        NULL);
    v_Discount_Tbl Type_Tlog05_Stg_Discount_Tbl := Type_Tlog05_Stg_Discount_Tbl();
    v_Reward_Rec   Type_Tlog05_Stg_Reward;
    v_Reward_Tbl   Type_Tlog05_Stg_Reward_Tbl := Type_Tlog05_Stg_Reward_Tbl();
    /* misc variables */
    v_Txn_Count              NUMBER := 0;
    v_Date                   DATE;
    v_Prev_Rec_Type          VARCHAR2(2);
    v_Prev_Detailid          NUMBER;
    v_Prev_Txndiscountamount NUMBER;
    v_This_Line_Cnt          NUMBER := 0;
    v_Ttl_Line_Cnt           NUMBER := 0;
    v_Item_Count             NUMBER := 0;
    v_Void_Txn_Type          VARCHAR2(8) := '40';
    v_Filename               VARCHAR2(512) := Nvl(p_Filename, 'tlog.txt');
    v_Pur_Giftcard_Amount    NUMBER := 0;
    v_Pur_Giftcard_Count     NUMBER := 0;
    v_Valid_01_Count         NUMBER := 0;
    v_Min_Headerid_This_Job  NUMBER := 0;
    v_My_Log_Id              NUMBER;
    v_Dap_Log_Id             NUMBER;
    v_Unknown_Product_Id     NUMBER;
    v_Matched_Processid      NUMBER := 0;
    v_24hr_Txn_Cnt           NUMBER := 0;
    v_Pointtypeid            NUMBER := 0;
    v_Categoryid             NUMBER := 0;
    /* main cursor */
    CURSOR Cur_Raw IS
      SELECT Rec_Type, User_Id, Rec, Rownum AS Filelineitem
        FROM Ext_Tlog05;
    c Cur_Raw%ROWTYPE;
    --log job attributes
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_N1               NUMBER := 0;
    v_N2               NUMBER := 0;
    v_Txt1             VARCHAR2(512);
    v_Txt2             VARCHAR2(512);
    v_Dt               DATE;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'Tlog05';
    --log msg attributes
    v_Messageid  NUMBER;
    v_Envkey     VARCHAR2(256) := 'BP_AE@' ||
                                  Upper(Sys_Context('userenv',
                                                    'instance_name'));
    v_Logsource  VARCHAR2(256) := 'Tlog05';
    v_Batchid    VARCHAR2(256) := 0;
    v_Message    VARCHAR2(1024);
    v_Reason     VARCHAR2(256);
    v_Error      VARCHAR2(256);
    v_Trycount   NUMBER := 0;
    v_Process_Id NUMBER := 0;
    v_Skip       NUMBER := 0;
    v_string     varchar2(512);
    v_ShortCdCount NUMBER := 0;
    v_Stylecode   NVARCHAR2(50);   -------------------AEO-820 changes
    v_Colorcode   NVARCHAR2(50);   -------------------AEO-820 changes
    v_FINAL_SELLING_PRICE  NUMBER; ---------------AEO-844 changes-- changed from varchar2(10)
    v_Conversion_Rate    float;             ---------------AEO-844 changes
    v_memberstatus     NUMBER(10);  /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
     V_GiftWrapCount    NUMBER := 0;
     v_loyaltyidcount   NUMBER := 0;
  BEGIN

    /* Start of job state tracking */
    Log_Process_State('initializing');

    /* this is a means to disable a job with out actually killing the session
    as well as track the current job's process */
    SELECT Nvl(MAX(Id), 0), MAX(Is_Active), MAX(Expiration_Date)
      INTO v_Process_Id, v_Txt1, v_Dt
      FROM Log_Process_Stat t
     WHERE Lower(t.Process) = Lower(Gv_Process);

    IF v_Txt1 != 'yes'
    THEN
      /* job in inactive state */
      Raise_Application_Error(-20001,
                              'process is in a non-active state: log_process_stat.is_active=' ||
                              v_Txt1);
    ELSIF v_Dt < SYSDATE
    THEN
      /* job expired */
      Raise_Application_Error(-20001,
                              'process is in an expired state: log_process_stat.expiration_date=' ||
                              To_Char(v_Dt, 'mm/dd/yyyy hh24:mi'));
   /* ELSIF v_N1 > 0
    THEN

      \* initialize starting state *\
      UPDATE Log_Process_Stat
         SET Job_Start     = SYSDATE,
             Job_End       = NULL,
             State         = 'Starting',
             Sid           = Sys_Context('userenv', 'session_id'),
             Inst_Id       = Sys_Context('userenv', 'inst_id'),
             Current_Count = 0
       WHERE Id = v_Process_Id;

      */
    END IF;

      /* initialize starting state */
      UPDATE Log_Process_Stat
         SET Job_Start     = SYSDATE,
             Job_End       = NULL,
             State         = 'Starting',
             Sid           = Sys_Context('userenv', 'SESSIONID'),
             Inst_Id       = Sys_Context('userenv', 'instance'),
             Current_Count = 0
       WHERE Lower(Process) = Lower(Gv_Process);
       COMMIT;
    /* set job state */
    /* suggest calling hist proc seperatly so that re-processing can occur.
    -- Log_process_state('S:hist_Tlog05');
      /* prep staging tables, if data already exists in stage tables then it gets moved to hist */
    --  hist_Tlog05();
    /* set job state */
    Log_Process_State('running');
    /* end of job state tracking */

    /* initialize the global variable with the product id of Gift Card items
       this will be used to assign the dtlproductid in the ats_txndetailitem table
    */
    SELECT prd.id INTO Gv_GiftCard_Product_Id FROM lw_product prd
    WHERE prd.classcode = '9911' AND prd.partnumber = '15022312';

    LOOP
      /* this initiates a lock, preventing the process from being run run more then one time at the same time */
      UPDATE Log_Process_Lock
         SET Id = Id
       WHERE Lower(Process) = Gv_Process;

      EXIT WHEN SQL%ROWCOUNT > 0;
      /* if no update occured, write the needed lock record, loop around again to initiate lock then exit loop */
      INSERT INTO Log_Process_Lock
        (Id, Process)
      VALUES
        (Seq_Rowkey.Nextval, Gv_Process);
      COMMIT; /* ok to commit this once, next loop will initiate the lock, this commit will be skipped at that point */
    END LOOP;
    /* dont commit past this point until the end... or else thread carefully and consider the impact */

    /* get job log id for this process and for the dap process */
    v_My_Log_Id  := Utility_Pkg.Get_Libjobid();
    v_Dap_Log_Id := Utility_Pkg.Get_Libjobid();

    /* log start of job (lw logging)*/
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => 'stage05' || v_Jobname);

    /* this will point the external table to passed filename and resets the ext tables log file */
    Initialize_Tbl(v_Filename);

    /* I need to know the range of staged id's for 77kid matching...
       to know if the matched record is in history or still in stage
       This saves time so i dont have to query both tables
    */
    SELECT Seq_Rowkey.Nextval INTO v_Min_Headerid_This_Job FROM Dual;

    /*
       use this id for the product id where products are missing from lw_product
       We wont process txn's with missing products, but i want to still give it
       a product id which will also specify it's missing.  Also will be a big help
       for DW, i suspect.
    */
    BEGIN

      SELECT t.Id
        INTO v_Unknown_Product_Id
        FROM Lw_Product t
       WHERE t.Name = 'UNKNOWN';

    EXCEPTION
      WHEN No_Data_Found THEN
        v_Unknown_Product_Id := Seq_Rowkey.Nextval;

        SELECT Pt.Pointtypeid
          INTO v_Pointtypeid
          FROM Lw_Pointtype Pt
         WHERE Lower(Pt.Name) = 'startingpoints';

        SELECT Ct.Id
          INTO v_Categoryid
          FROM Lw_Category Ct
         WHERE Lower(Ct.Name) = 'misc';

        INSERT INTO Lw_Product
          (Id,
           Categoryid,
           Isvisibleinln,
           NAME,
           Brandname,
           Partnumber,
           Baseprice,
           Pointtype,
          --// AEO-74 Upgrade 4.5 changes here -----------SCJ
         --  Createddate,
         --  Last_Dml_Date,
           Createdate,
           Updatedate,
           --// AEO-74 Upgrade 4.5 changes here -----------SCJ
           Classcode,
           Classdescription,
           Stylecode,
           Styledescription,
           Deptcode,
           Deptdescription,
           Divisioncode,
           Divisiondescription,
           Companycode,
           Companydescription,
           Struserfield)
        VALUES
          (v_Unknown_Product_Id,
           v_Categoryid,
           0,
           'UNKNOWN',
           '',
           'UNKNOWN',
           0,
           v_Pointtypeid,
           SYSDATE,
           SYSDATE,
           '',
           '',
           '',
           '',
           '',
           '',
           '',
           '',
           '',
           '',
           '');
    END;
    BEGIN
    OPEN Cur_Raw;
    FETCH Cur_Raw INTO c;
    IF Cur_Raw%FOUND THEN
    /* main loop, hitting tlog file */
    LOOP
    EXIT WHEN Cur_Raw%NOTFOUND;
      /*  this counts the txn records, only start if header's already initialized */
      IF v_This_Line_Cnt > 0
      THEN
        v_This_Line_Cnt := v_This_Line_Cnt + 1;
      END IF;

     v_string := c.Rec;

      /* process the previous txn here but only if the line count matched*/
      /* This is where we actually write the data to the stage tables
      It will occur again after the loop to capture the last txn on file */
      IF (v_This_Line_Cnt - 1) = v_Ttl_Line_Cnt AND c.Rec_Type = '00' AND
         v_Skip = 0
      THEN

        /*  write arrarys to staging tables */
        Load_Stage_Data(p_Head_Rec       => v_Head_Rec,
                        p_Detail_Tbl     => v_Detail_Tbl,
                        p_Tender_Tbl     => v_Tender_Tbl,
                        p_Discount_Tbl   => v_Discount_Tbl,
                        p_Reward_Tbl     => v_Reward_Tbl,
                        p_My_Log_Id      => v_My_Log_Id,
                        p_Valid_01_Count => v_Valid_01_Count,
                        p_Txn_Count      => v_Txn_Count,
                        p_Store_Nbr      => v_00_Rec.Store_Number,
                        p_Rec_Type       => v_Txt1,
                        p_Action         => v_Txt2);

        /* count processed/failed transactions
        ignore transactions w/out details   */
        IF v_Detail_Tbl.Count > 0
        THEN
          IF v_Head_Rec.Processid = Gv_Status_Ready
          THEN
            v_Messagespassed := v_Messagespassed + 1;
             /*PI 24762 begin */
          ELSIF v_Head_Rec.Processid = Gv_Status_Noproduct
             or v_Head_Rec.Processid = Gv_Status_Error
             or v_Head_Rec.Processid = Gv_Status_Dup
          THEN
            /*PI 24762 end */
            v_Messagesfailed := v_Messagesfailed + 1;
          END IF;
        END IF;
        /* reset variables for next pass */
        v_This_Line_Cnt := 0;
        v_Ttl_Line_Cnt  := 0;
        v_Item_Count    := 0;

        /* log every 500 txns to report progress */
        IF MOD(v_Txn_Count, 500) = 0
        THEN
          write_tlog_dup_exception();
          write_Stage_Data(); /* writes txns to stage table from memory */
          Log_Process_Count(v_Txn_Count);
          COMMIT;
        END IF;

        /* for testing only
           in test mode commit occurs every 100 txn's
           and the process will stop once the test count is reached.
        */
        IF Lower(Nvl(p_Test_Count, 0)) > 0
        THEN
          IF MOD(v_Txn_Count, 100) = 0
          THEN
            COMMIT;
          END IF;
          IF v_Txn_Count >= p_Test_Count
          THEN
            GOTO The_End;
          END IF;
        END IF;
        /* check txn count, reset txn if unexpected record count found */
      ELSIF (c.Rec_Type != '00' AND v_This_Line_Cnt > v_Ttl_Line_Cnt) /* too many rows */
            OR (v_This_Line_Cnt <= v_Ttl_Line_Cnt AND c.Rec_Type = '00') /* not enough rows */
      THEN
        /* skip/reset this txn */
        v_This_Line_Cnt := 0;
        v_Ttl_Line_Cnt  := 0;
      END IF;

      /* only care about these string types */
      IF ((c.Rec_Type IN ('00',
                          '01',
                          '02',
                          '03',
                          '04',
                          '05',
                          '06',
                          '07',
                          '08',
                          '10',
                          '11',
                          '14',
                          '15',
                          '18',
                          '97',
                          '99')) OR
         (c.Rec_Type = '27' AND
         c.User_Id IN ('33', '51', '62', '77', '92', '93', '99'))) AND
         (v_Skip = 0 OR c.Rec_Type = '00')
      THEN
        /* looking for new txn, header rec */
        IF v_This_Line_Cnt = 0 AND c.Rec_Type = '00'
        THEN
          /* v_skip is a flag, zero flags that its currently process a txn
                               one flags that its looking for the next txn
          */
          v_Skip := 0;
          /* load temp header rec/arrary with external table record */
          v_00_Rec := Get_Tlog05_00header(p_Rec => c.Rec);
          /* check that new txn is valid */
          IF v_00_Rec.Transaction_Type != v_Void_Txn_Type
          THEN
            v_This_Line_Cnt := 1; /* if left zero then nothing txn gets skipped */
            v_Ttl_Line_Cnt  := v_00_Rec.Number_Of_Strings;
          END IF;
        END IF;

        /* process txn rows */
        IF v_This_Line_Cnt > 0
        THEN
          /* good txn */
          /* header row */
          IF v_This_Line_Cnt = 1
          THEN
            /* reset the txn, we're nulling out the arrays and reseting variables for the new txn */
            v_Detail_Tbl          := Type_Tlog05_Stg_Detail_Tbl();
            v_Head_Rec            := Initialize_Head_Rec();
            v_Tender_Tbl          := Type_Tlog05_Stg_Tender_Tbl();
            v_Discount_Tbl        := Type_Tlog05_Stg_Discount_Tbl();
            v_Reward_Tbl          := Type_Tlog05_Stg_Reward_Tbl();
            v_Pur_Giftcard_Amount := 0;
            v_Pur_Giftcard_Count  := 0;
            v_Valid_01_Count      := 0;

            /* initilize header attributes/array with details from 00 rec */
            /* v_head_rec is the array that will be used later to insert data into staging table */
            v_Head_Rec.Processid          := Gv_Status_Nocard;
            v_Head_Rec.Txnregisternumber  := v_00_Rec.Register_Number;
            v_Head_Rec.Txndate            := Valid_Date(v_00_Rec.Transaction_Date || ' ' ||
                                                        v_00_Rec.Transaction_Time,
                                                        'mmddrr hh24mi');
            v_Head_Rec.Txnnumber          := v_00_Rec.Transaction_Number;
            v_Head_Rec.Txnemployeeid      := v_00_Rec.Employee_Number;
            v_Head_Rec.Txnqualpurchaseamt := 0;
            v_Head_Rec.Txndiscountamount  := 0;
            v_Head_Rec.Filelineitem       := c.Filelineitem;
            v_Head_Rec.Ipcode             := 0;
            v_Head_Rec.CashierNumber      := v_00_Rec.Cashier_Number;       --AEO-1533

            /* get rowkey for header */
            SELECT Seq_Rowkey.Nextval INTO v_Head_Rec.Rowkey FROM Dual;

            /* get txnheaderid, just set to rowkey as the client does not provide a key */
            --v_Head_Rec.Txnheaderid := v_Head_Rec.Rowkey;

            /* set txn type */
            IF v_00_Rec.Transaction_Type IN ('80', '00')
            THEN
              /* purchase */
              v_Head_Rec.Txntype := 1;
            ELSIF v_00_Rec.Transaction_Type = '01'
            THEN
              /* Return */
              v_Head_Rec.Txntype := 2;
            ELSE
              /* something else, will probably skip this txn... needs investigating */
              v_Head_Rec.Txntype := 0;
            END IF;

            /* get store brand */
            BEGIN
              SELECT s.Storeid, s.Storenumber, s.Brandname
                INTO v_Head_Rec.Txnstoreid,
                     v_Head_Rec.Txnstorenumber,
                     v_Head_Rec.Brandid
                FROM Lw_Storedef s
               WHERE Storenumber = to_char(Nvl(To_Number(v_00_Rec.Store_Number), 0))
                 AND s.Storename != 'UNKNOWN';
            EXCEPTION
              WHEN No_Data_Found THEN
                BEGIN
                  SELECT s.Storeid, s.Storenumber, s.Brandname
                    INTO v_Head_Rec.Txnstoreid,
                         v_Head_Rec.Txnstorenumber,
                         v_Head_Rec.Brandid
                    FROM Lw_Storedef s
                   WHERE Storenumber = v_00_Rec.Store_Number
                     AND s.Storename != 'UNKNOWN';
                EXCEPTION
                  WHEN No_Data_Found THEN
                    BEGIN
                      -- since no data was found, try looking for the record by not filtering out UNKNOWNS
                      -- It is possible for 1 tlog to contain the same missing store code.

                      v_Head_Rec.Errormessage := 'UNKNOWN STORE';
                      -- set the store info null
                      v_Head_Rec.Txnstoreid     := 0;
                      v_Head_Rec.Txnstorenumber := v_00_Rec.Store_Number;
                      v_Head_Rec.Brandid        := -1;

                      v_Messagesfailed := v_Messagesfailed + 1;
                      v_Error          := SQLERRM;
                      v_Reason         := 'Unknown Store Number: ' ||
                                          v_Head_Rec.Txnstorenumber ||
                                          ' HeaderID : ' ||
                                          v_Head_Rec.Txnheaderid;
                      v_Message        := '<failed>' || Chr(10) ||
                                          '  <details>' || Chr(10) ||
                                          '    <pkg>Stage_Tlog5</pkg>' ||
                                          Chr(10) ||
                                          '    <proc>Stage_Tlog05</proc>' ||
                                          Chr(10) || '    <filename>' ||
                                          v_Filename || '</filename>' ||
                                          Chr(10) || '  </details>' ||
                                          Chr(10) || '</failed>';

                      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                          p_Jobdirection     => v_Jobdirection,
                                          p_Filename         => v_Filename,
                                          p_Starttime        => v_Starttime,
                                          p_Endtime          => v_Endtime,
                                          p_Messagesreceived => v_Messagesreceived,
                                          p_Messagesfailed   => v_Messagesfailed,
                                          p_Jobstatus        => v_Jobstatus,
                                          p_Jobname          => 'stage05' ||
                                                                v_Jobname);

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

                    END;
                END;
            END;
            /* set elsewhere (other rec types) or not valid for AE, confirm this!

                   v_head_rec.cardid :=NULL;
                   v_head_rec.creditcardid :=NULL;
                   v_head_rec.txnmaskid :=NULL;
                   v_head_rec.txndatemodified :=NULL;
                   v_head_rec.txnemailaddress :=NULL;
                   v_head_rec.txnphonenumber :=NULL;
                   v_head_rec.txnchannelid :=NULL;
                   v_head_rec.txncreditsused :=NULL;
                   v_head_rec.reconcilestatus :=NULL;
                   v_head_rec.ordernumber :=NULL;
            */
          END IF;

          /* txn line item record */
          IF c.Rec_Type = '01'
          THEN
            /* load rawdata to record type */
            v_01_Rec := Get_Tlog05_01item(c.Rec);

            IF v_01_Rec.Mode_Indicator IN (1, 2, 6) /* only types we care about */
            THEN
              /* reset/initialize temp array/record */
              v_Detail_Rec := Initialize_Detail_Rec();
              /* counter used for detail linenumber, value not provided from client */
              v_Item_Count := v_Item_Count + 1;

              /* load array/rec attributes
                 v_detail_rec will later be loaded into v_detail_tbl (array of arrays)
                 v_detail_tbl will be used to load the stage table later
              */
              v_Detail_Rec.Txntype   := v_01_Rec.Mode_Indicator;
              v_Detail_Rec.Dtltypeid := v_01_Rec.Mode_Indicator;
              v_Detail_Rec.Skunumber := v_01_Rec.Sku_Class_Number;
              /*AEO-844 Changes begin here-----------SCJ*/
              v_FINAL_SELLING_PRICE  := null; --initialize
              v_Conversion_Rate := null;   --initialize
              IF UPPER(v_Head_Rec.CURRENCY_CODE) = 'CAD' --CHECKIN THIS THE TRANSACTION IS CANADIAN BOSS TXN, IF SO CONVERTING TO CANADIAN DOLL
                THEN
                  v_Conversion_Rate := checkconversionrate(v_Head_Rec.CURRENCY_CODE,v_Head_Rec.txndate);
                  v_Detail_Rec.dtlsaleamount_org := v_01_Rec.Final_Selling_Price;
                  v_Head_Rec.CURRENCY_RATE  := v_Conversion_Rate;
                  v_FINAL_SELLING_PRICE  := v_01_Rec.Final_Selling_Price * v_Conversion_Rate;
                  ELSE
                  v_FINAL_SELLING_PRICE  := v_01_Rec.Final_Selling_Price;
              END IF;
              IF (v_Detail_Rec.Txntype = 2 OR v_Detail_Rec.Txntype = 6)
              THEN
                /* a return, flip sign */
                v_Detail_Rec.Dtlsaleamount := (v_FINAL_SELLING_PRICE * -1);
              ELSE
                /* normal sale, keep as is */
                v_Detail_Rec.Dtlsaleamount := v_FINAL_SELLING_PRICE;
              END IF;
               /*AEO-844 Changes end here-----------SCJ*/
               /*National Rollout Changes AEO-1218 here        ----------------SCJ*/

              v_Detail_Rec.Dtlquantity    := v_01_Rec.Quantity;
              v_Detail_Rec.Dtlitemlinenbr := v_Item_Count;
              v_Detail_Rec.Filelineitem   := c.Filelineitem;
              v_Detail_Rec.Dtlclasscode   := v_01_Rec.Class_Number;

              /* get rowkey for detail */
              v_Detail_Rec.Rowkey := Seq_Rowkey.Nextval;

              /* get txndetail id, use rowkey as client does not provide a key */
              v_Detail_Rec.Txndetailid := v_Detail_Rec.Rowkey;

              v_Prev_Detailid               := v_Detail_Rec.Txndetailid;
              v_Valid_01_Count              := v_Valid_01_Count + 1;
              v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,
                                                   0) +
                                               (Nvl(v_Detail_Rec.Dtlquantity,
                                                    0) * Nvl(v_Detail_Rec.Dtlsaleamount,
                                                             0));
   --  IF TRIM(v_01_Rec.Markdown_Flag) IS NOT NULL
            IF TRIM(v_01_Rec.Markdown_Flag) = 'C'
               /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
              THEN
                v_Detail_Rec.Dtlclearanceitem := 1;
                /* v_Detail_Rec.Dtlsaleamount := 0; --So no jean/bra credits are issued. AEO-1900 AH*/
              END IF;

              /* get the product id and brand */
              BEGIN

                SELECT p.Brandname, p.Id, p.Classcode
                  INTO v_Detail_Rec.Brandid,
                       v_Detail_Rec.Dtlproductid,
                       v_Detail_Rec.Dtlclasscode
                  FROM Lw_Product p
                 WHERE p.Partnumber = v_01_Rec.Sku_Class_Number;
              EXCEPTION
                WHEN No_Data_Found THEN
                  /* log missing products, this log table should be managed elsewhere. */
                  -- TODO: log message
                  INSERT INTO Log_Missingproduct
                    (Txndetailid,
                     Sku_Class_Number,
                     Class_Number,
                     Transaction_Date,
                     Store_Number,
                     Transaction_Number,
                     Register_Number,
                     Error_Date,
                     Filename,
                     Jobnumber)
                  VALUES
                    (v_Detail_Rec.Txndetailid,
                     v_01_Rec.Sku_Class_Number,
                     v_01_Rec.Class_Number,
                     v_Head_Rec.Txndate,
                     v_Head_Rec.Txnstoreid,
                     v_Head_Rec.Txnnumber,
                     v_Head_Rec.Txnregisternumber,
                     SYSDATE,
                     v_Filename,
                     v_My_Log_Id);

                  /* assign unknown/default productid */
                  v_Detail_Rec.Dtlproductid := v_Unknown_Product_Id;

                  /* mark 01 record so it wont be process, but keep for reporting (dw) */
                  v_Detail_Rec.Processid  := Gv_Status_Noproduct;
                  v_Head_Rec.Errormessage := 'UNKNOWN PRODUCT';

                  v_Messagesfailed := v_Messagesfailed + 1;
                  v_Error          := SQLERRM;
                  v_Reason         := 'Unknown SKU Number: ' ||
                                      v_01_Rec.Sku_Class_Number ||
                                      ' HeaderID : ' ||
                                      v_Head_Rec.Txnheaderid;
                  v_Message        := '<failed>' || Chr(10) ||
                                      '  <details>' || Chr(10) ||
                                      '    <pkg>Stage_Tlog5</pkg>' ||
                                      Chr(10) ||
                                      '    <proc>Stage_Tlog05</proc>' ||
                                      Chr(10) || '    <filename>' ||
                                      v_Filename || '</filename>' ||
                                      Chr(10) || '  </details>' || Chr(10) ||
                                      '</failed>';

                  Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                      p_Jobdirection     => v_Jobdirection,
                                      p_Filename         => v_Filename,
                                      p_Starttime        => v_Starttime,
                                      p_Endtime          => v_Endtime,
                                      p_Messagesreceived => v_Messagesreceived,
                                      p_Messagesfailed   => v_Messagesfailed,
                                      p_Jobstatus        => v_Jobstatus,
                                      p_Jobname          => 'stage' ||
                                                            v_Jobname);

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

              END;
              /* upload rec to collection
                 v_detail_tbl will be used to populat the stage table
                 it will hold all the details for this txn
              */
              v_Detail_Tbl.Extend;
              v_Detail_Tbl(v_Detail_Tbl.Count) := v_Detail_Rec;
            END IF;
          END IF;
/*
 AEO-820 changes begin here----------------------------------------SCJ
*/
          /* txn line item record */
          IF c.Rec_Type = '99'
          THEN
            /* load rawdata to record type */
            v_99_Rec := Get_Tlog05_99item(c.Rec);

            IF (trunc(v_Head_Rec.Txndate) is not null )
            THEN
               IF  (v_99_Rec.Sku_Class_Number is not null )           -- AEO-860 changes  here----------------------------------------SCJ
                 THEN
              /* reset/initialize temp array/record */
              v_Detail_Rec := Initialize_Detail_Rec();
               /* get the product id and brand */
              BEGIN

                SELECT p.Brandname, p.Id, p.Classcode,p.stylecode,p.companycode
                  INTO v_Detail_Rec.Brandid,
                       v_Detail_Rec.Dtlproductid,
                       v_Detail_Rec.Dtlclasscode,
                       v_Stylecode,
                       v_Colorcode
                  FROM Lw_Product p
                 WHERE p.Partnumber = v_99_Rec.Sku_Class_Number;
              EXCEPTION
                WHEN No_Data_Found THEN
                  /* log missing products, this log table should be managed elsewhere. */
                  -- TODO: log message
                  INSERT INTO Log_Missingproduct
                    (Txndetailid,
                     Sku_Class_Number,
                     Class_Number,
                     Transaction_Date,
                     Store_Number,
                     Transaction_Number,
                     Register_Number,
                     Error_Date,
                     Filename,
                     Jobnumber)
                  VALUES
                    (v_Detail_Rec.Txndetailid,
                     v_99_Rec.Sku_Class_Number,
                     v_99_Rec.Class_Number,
                     v_Head_Rec.Txndate,
                     v_Head_Rec.Txnstoreid,
                     v_Head_Rec.Txnnumber,
                     v_Head_Rec.Txnregisternumber,
                     SYSDATE,
                     v_Filename,
                     v_My_Log_Id);

                  /* assign unknown/default productid */
                  v_Detail_Rec.Dtlproductid := v_Unknown_Product_Id;

                  /* mark 01 record so it wont be process, but keep for reporting (dw) */
                  v_Detail_Rec.Processid  := Gv_Status_Noproduct;
                  v_Head_Rec.Errormessage := 'UNKNOWN PRODUCT';

                  v_Messagesfailed := v_Messagesfailed + 1;
                  v_Error          := SQLERRM;
                  v_Reason         := 'Unknown SKU Number: ' ||
                                      v_99_Rec.Sku_Class_Number ||
                                      ' HeaderID : ' ||
                                      v_Head_Rec.Txnheaderid;
                  v_Message        := '<failed>' || Chr(10) ||
                                      '  <details>' || Chr(10) ||
                                      '    <pkg>Stage_Tlog5</pkg>' ||
                                      Chr(10) ||
                                      '    <proc>Stage_Tlog05</proc>' ||
                                      Chr(10) || '    <filename>' ||
                                      v_Filename || '</filename>' ||
                                      Chr(10) || '  </details>' || Chr(10) ||
                                      '</failed>';

                  Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                      p_Jobdirection     => v_Jobdirection,
                                      p_Filename         => v_Filename,
                                      p_Starttime        => v_Starttime,
                                      p_Endtime          => v_Endtime,
                                      p_Messagesreceived => v_Messagesreceived,
                                      p_Messagesfailed   => v_Messagesfailed,
                                      p_Jobstatus        => v_Jobstatus,
                                      p_Jobname          => 'stage' ||
                                                            v_Jobname);

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

              END;

              IF (ChkStyleNum_99(trunc(v_Head_Rec.Txndate),v_Detail_Rec.Dtlclasscode,v_Stylecode,v_Colorcode)  = 1)
                then
              /* counter used for detail linenumber, value not provided from client */
              v_Item_Count := v_Item_Count + 1;

              /* load array/rec attributes
                 v_detail_rec will later be loaded into v_detail_tbl (array of arrays)
                 v_detail_tbl will be used to load the stage table later
              */
              v_Detail_Rec.Txntype   := v_99_Rec.Mode_Indicator;
              v_Detail_Rec.Dtltypeid := v_99_Rec.Mode_Indicator;
              v_Detail_Rec.Skunumber := v_99_Rec.Sku_Class_Number;
              IF (v_Detail_Rec.Txntype = 2 OR v_Detail_Rec.Txntype = 6)
              THEN
                /* a return, flip sign */
                v_Detail_Rec.Dtlsaleamount := (v_99_Rec.Final_Selling_Price * -1);
              ELSE
                /* normal sale, keep as is */
                v_Detail_Rec.Dtlsaleamount := v_99_Rec.Final_Selling_Price;
              END IF;
              IF TRIM(v_99_Rec.Markdown_Flag) IS NOT NULL
              THEN
                v_Detail_Rec.Dtlclearanceitem := 1;
              END IF;
              v_Detail_Rec.Dtlquantity    := v_99_Rec.Quantity;
              v_Detail_Rec.Dtlitemlinenbr := v_Item_Count;
              v_Detail_Rec.Filelineitem   := c.Filelineitem;
              v_Detail_Rec.Dtlclasscode   := v_99_Rec.Class_Number;

              /* get rowkey for detail */
              v_Detail_Rec.Rowkey := Seq_Rowkey.Nextval;

              /* get txndetail id, use rowkey as client does not provide a key */
              v_Detail_Rec.Txndetailid := v_Detail_Rec.Rowkey;

              v_Prev_Detailid               := v_Detail_Rec.Txndetailid;
              v_Valid_01_Count              := v_Valid_01_Count + 1;
              v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,
                                                   0) +
                                               (Nvl(v_Detail_Rec.Dtlquantity,
                                                    0) * Nvl(v_Detail_Rec.Dtlsaleamount,
                                                             0));


              /* upload rec to collection
                 v_detail_tbl will be used to populat the stage table
                 it will hold all the details for this txn
              */
              v_Detail_Tbl.Extend;
              v_Detail_Tbl(v_Detail_Tbl.Count) := v_Detail_Rec;
            END IF;
            END IF;
          END IF;
         END IF;




/*
 AEO-820 changes end here----------------------------------------SCJ
*/
          /*subtotal record*/
          IF c.Rec_Type = '02'
          THEN
            v_02_Rec             := Get_Tlog05_02subtotal(c.Rec);
            v_Head_Rec.Txnamount := v_02_Rec.Amount;
          END IF;
          /* sales tax  record */
          IF c.Rec_Type = '03'
          THEN
            v_03_Rec                := Get_Tlog05_03salestax(c.Rec);
            v_Head_Rec.Ttltaxamount := v_03_Rec.Amount;
            IF v_03_Rec.Mode_Indicator = 2
            THEN
              /* 1 to 1 with header so just set the value in the header array/rec */
              v_Head_Rec.Ttltaxamount := v_Head_Rec.Ttltaxamount * -1;
            END IF;
          END IF;
          /* tender   record */
          IF c.Rec_Type = '04'
          THEN
            /* reset/initialize record
               record will be added to the v_tender_tbl array
               v_tender_tbl is used to populate the stage table
            */
            v_Tender_Rec := Initialize_Tender_Rec();
            /* load temporary array */
            v_04_Rec := Get_Tlog05_04tender(c.Rec);

            /* beging setting tender attributes to record */
            v_Tender_Rec.Tendertype   := v_04_Rec.Tender_Type;
            v_Tender_Rec.Tenderamount := Nvl(v_04_Rec.Amount, 0);

            /* flip sign if found to be of a negative tender type */
            IF v_04_Rec.Tender_Type IN (80,
                                        81,
                                        82,
                                        83,
                                        84,
                                        85,
                                        86,
                                        87,
                                        88,
                                        89,
                                        90,
                                        91,
                                        92,
                                        93,
                                        94,
                                        95) AND
               v_Tender_Rec.Tenderamount != 0
            THEN
              v_Tender_Rec.Tenderamount := v_Tender_Rec.Tenderamount * -1;
            END IF;
            /* update running tender amount ttl for this txn, add to header rec */
            /* added 20120410 (if but not contents) */
            IF v_tender_rec.Tendertype != 69 THEN
              v_Head_Rec.Ttltenderamount := Nvl(v_Head_Rec.Ttltenderamount, 0) +
                                            v_Tender_Rec.Tenderamount;
            END IF;
              /*National Rollout Changes AEO-1218 begin here        ----------------SCJ*/
            IF v_tender_rec.Tendertype in (75,78) THEN
             select TO_NUMBER(cc.value) into v_Head_Rec.AECCMultiplier  from bp_Ae.Lw_Clientconfiguration cc where cc.key = 'AECCMultiplier';
            END IF;
            /*National Rollout Changes AEO-1218 begin here        ----------------SCJ*/
            /* get rowkey */
            v_Tender_Rec.Rowkey := Seq_Rowkey.Nextval;

            /* get tender_id, use rowkey as client does not provide a key */
            v_Tender_Rec.Txntenderid := v_Tender_Rec.Rowkey;

            /* set elsewhere or not valid for AE, confirm! */
            --v_tender_rec.tendercurrency,
            --v_tender_rec.tendertax,
            --v_tender_rec.tendertaxrate,
            --v_tender_rec.errormessage,
            --v_tender_rec.statuscode;

            /* load rec to collection, v_tender_tbl will populate the stage table */
            v_Tender_Tbl.Extend;
            v_Tender_Tbl(v_Tender_Tbl.Count) := v_Tender_Rec;
            /* reset rec for next 04 rec */
            v_Tender_Rec := NULL;
          END IF;
          /* Gift Certificate   record */
          /* only expecting one of these, assigning value to header rec */
          IF c.Rec_Type = '05'
          THEN
            /* load temp record with tlog record */
            v_05_Rec := Get_Tlog05_05giftcert(c.Rec);
            /*v_head_rec.txndiscountamount := v_05_rec.AMOUNT;*/

            /* create a line item for the certificate */
            v_Detail_Rec              := Initialize_Detail_Rec();
            v_Detail_Rec.Processid    := Gv_Status_Giftcertitem;
            v_Detail_Rec.Txntype      := -1;
            v_Detail_Rec.Skunumber    := 'GiftCert';
            v_Detail_Rec.Errormessage := 'GiftCert(05)';
            v_Item_Count              := v_Item_Count + 1;

            v_Detail_Rec.Dtlsaleamount    := (v_05_Rec.Amount * -1);
            v_Detail_Rec.Dtlclearanceitem := 0;
            v_Detail_Rec.Dtlquantity      := 1;
            v_Detail_Rec.Dtlitemlinenbr   := v_Item_Count;
            v_Detail_Rec.Filelineitem     := c.Filelineitem;

            v_Detail_Rec.Brandid := 1;

            -- default this to 0 so it account activity on the customer service site will not throw an error.
            v_Detail_Rec.Dtlclasscode := 0;

            v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,
                                                 0) + Nvl(v_Detail_Rec.Dtlsaleamount,
                                                          0);
            /* get rowkey for detail */
            v_Detail_Rec.Rowkey := Seq_Rowkey.Nextval;

            /* get txndetail id, using rowkey as no key provide from client */
            v_Detail_Rec.Txndetailid := v_Detail_Rec.Rowkey;

            /* load detail table array, will be used to populate stage table */
            v_Detail_Tbl.Extend;
            v_Detail_Tbl(v_Detail_Tbl.Count) := v_Detail_Rec;
          END IF;
          /* foundOrder (in store web order)   record */
          IF c.Rec_Type = '14'
          THEN
            v_14_Rec := Get_Tlog05_14foundorder(c.Rec);
            IF v_14_Rec.Reason_Code = 'F'
            THEN
              /* define transaction type as a found order */
              v_Head_Rec.Txntype := 4;
            END IF;
/* redesign changes  begin here --------------------------------------------------  SCJ  */
            IF v_14_Rec.Reason_Code = '1'
            THEN
              /* define transaction type as a GiftWrap */
              v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,
                                                 0) + Nvl(v_14_Rec.AMOUNT ,
                                                          0);
            /*National Rollout Changes AEO-1218 begin here        ----------------SCJ*/

              v_Detail_Rec := Initialize_Detail_Rec();
              v_Item_Count := v_Item_Count + 1;
              v_Detail_Rec.Txntype   := v_14_Rec.MODE_INDICATOR ;
              v_Detail_Rec.Dtltypeid := v_14_Rec.MODE_INDICATOR ;
              select count(p.partnumber)
                into V_GiftWrapCount
                from bp_Ae.Lw_Product p
               where p.name = 'GiftWrapTxn';
              IF (V_GiftWrapCount >0)
                THEN
               select p.partnumber,p.id,p.classcode
                into v_Detail_Rec.Skunumber,v_Detail_Rec.DtlProductid,v_Detail_Rec.Dtlclasscode
                from bp_Ae.Lw_Product p
               where p.name = 'GiftWrapTxn';
               /* select p.id
                into v_Detail_Rec.DtlProductid
                from bp_Ae.Lw_Product p
               where p.name = 'GiftWrapTxn';*/
              END IF;

             v_Detail_Rec.Dtlquantity      := 1;
              v_Detail_Rec.Dtlitemlinenbr := v_Item_Count;
              v_Detail_Rec.Filelineitem   := c.Filelineitem;
              v_Detail_Rec.dtlsaleamount  := v_14_Rec.AMOUNT;
              v_Detail_Rec.Brandid        := 1;
             -- v_Detail_Rec.Dtlclasscode   := v_99_Rec.Class_Number;
               v_Detail_Rec.Rowkey := Seq_Rowkey.Nextval;
              /* get txndetail id, use rowkey as client does not provide a key */
              v_Detail_Rec.Txndetailid := v_Detail_Rec.Rowkey;
              v_Prev_Detailid               := v_Detail_Rec.Txndetailid;
              v_Detail_Tbl.Extend;
              v_Detail_Tbl(v_Detail_Tbl.Count) := v_Detail_Rec;
            /*National Rollout Changes AEO-1218 end here        ----------------SCJ*/
            END IF;
/* redesign changes  end here --------------------------------------------------  SCJ  */
          END IF;
          /* credit card check   record */
          IF c.Rec_Type = '15'
          THEN
            NULL; /* have found nothing to do with this so far... */
            --v_15_rec :=get_Tlog05_15ceditcrdcheck (c.rec);
          END IF;
          /* txn discount string   record */
          IF c.Rec_Type = '18'
          THEN
            /* load raw data to record type */
            v_18_Rec := Get_Tlog05_18txndiscount(c.Rec);
            /* strange previous looping requirement, confirm this logic else where in the proc */
            v_Prev_Txndiscountamount := v_18_Rec.Discount_Amount_Taxable -
                                        v_18_Rec.Discount_Amount_Nontaxable;

            IF v_18_Rec.Mode_Indicator = '1'
            THEN
              /* flip sign */
              v_Prev_Txndiscountamount := v_Prev_Txndiscountamount * -1;
            END IF;

            /* include this amount on the header discount field */
            v_Head_Rec.Txndiscountamount := Nvl(v_Head_Rec.Txndiscountamount,
                                                0) +
                                            Nvl(v_Prev_Txndiscountamount, 0);

            /* adjust the header txnqualpurchaseamt, this is our qp */
            v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,
                                                 0) + Nvl(v_Prev_Txndiscountamount,
                                                          0);
          END IF;
          /* Original TXN  record */
          IF c.Rec_Type = '27' AND c.User_Id = '33'
          THEN
            /* load raw data to record type */
            v_2733_Rec := Get_Tlog05_2733origtxn(c.Rec);

            /* get store id */
            BEGIN
              SELECT s.storeid
                INTO v_Head_Rec.Org_Txnstoreid
                FROM Lw_Storedef s
               WHERE (Storenumber = TRIM(v_2733_Rec.Original_Store) OR
                     StoreNumber = to_char((TO_NUMBER(v_2733_Rec.Original_Store))));

              /* valid date function used to prevent format errors */
              /* if store not found then date/txnnbr wont be set */
              v_Head_Rec.Org_Txndate := Valid_Date(v_2733_Rec.Original_Transation_Date,
                                                   'MMDDRR');
              v_Head_Rec.Org_Txnnbr  := v_2733_Rec.Original_Transaction;
            EXCEPTION
              WHEN No_Data_Found THEN
                v_Head_Rec.Errormessage := 'OrgTXN Store not found: ' ||
                                           v_2733_Rec.Original_Store;
                /*
                   i want to insert the missing store here with default data.
                   or do we skip the entire txn?
                */
            END;
          END IF;
          /* discount reason  record  */
          IF c.Rec_Type = '27' AND c.User_Id = '51'
          THEN
            /* load raw data to record type */
            v_2751_Rec := Get_Tlog05_2751discountrsn(c.Rec);
           /* IF v_2751_Rec.Discount_Type = 2
            THEN
              \* at txn level, ats_txnrewardredeem  *\
              \* reset/initialize record *\
              v_Reward_Rec                 := Initialize_Reward_Rec();
\* redesign changes  begin here --------------------------------------------------  SCJ  *\
        --  v_Reward_Rec.Certificatecode := v_2751_Rec.Discount_Reason_Code;
            v_Reward_Rec.Certificatecode := NVL(trim(v_2751_Rec.N_20_DIGIT_BARCODE),0);
            v_Reward_Rec.Certificateredeemtype     := NVL(trim(v_2751_Rec.N_3_DIGIT_DISCOUNT_REASON_CODE),0);
            v_Head_Rec.Txndiscountamount := Nvl(v_Head_Rec.Txndiscountamount,0) +
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0); \* include this amount on the header discount field   *\
           \* v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,0) -
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);  \* adjust the header txnqualpurchaseamt, this is our qp *\       *\
            \*When discount amount is greater than qualifying purchase amount,then sero out qualifying purchase amount *\
            \*if  (Nvl(v_Head_Rec.Txnqualpurchaseamt,0) >= 0 )
              then

                if ((NVL(v_2751_Rec.DISCOUNT_AMOUNT,0) > Nvl(v_Head_Rec.Txnqualpurchaseamt,0))  )
                 then v_Head_Rec.Txnqualpurchaseamt := 0;
                end if;

                if (NVL(v_2751_Rec.DISCOUNT_AMOUNT,0) < Nvl(v_Head_Rec.Txnqualpurchaseamt,0) )
                 then   v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,0) -
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);  \* adjust the header txnqualpurchaseamt, this is our qp *\
                end if;
            end if;
            if   (Nvl(v_Head_Rec.Txnqualpurchaseamt,0) < 0 )
                 then
               v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,0) -
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);  \* adjust the header txnqualpurchaseamt, this is our qp *\
            end if;
            *\
\* redesign changes  end here ---------------------------------------------------  SCJ  *\
              v_Reward_Rec.Txnheaderid     := v_Head_Rec.Txnheaderid;
              v_Reward_Rec.programid       := 0;

              \* get rowkey *\
              SELECT Seq_Rowkey.Nextval INTO v_Reward_Rec.Rowkey FROM Dual;
              \* get rewardid *\
              v_Reward_Rec.Txnrewardredeemid := v_Reward_Rec.Rowkey;

              \* load rec to collection *\
              v_Reward_Tbl.Extend;
              v_Reward_Tbl(v_Reward_Tbl.Count) := v_Reward_Rec;
              \* reset rec for next reward *\
              v_Reward_Rec := NULL;
            ELSIF v_2751_Rec.Discount_Type = 1*/
                IF v_2751_Rec.Discount_Type = 1
            THEN
                 /* AEO 510  Changes begin here ------------------------------------------------------ SCJ   */
              /* reset/initialize record */
              v_ShortCdCount := 0;
--Removing shortcode checking before staging redemptions
/*
              select count(sc.a_shortcode)
                into v_ShortCdCount
                from bp_ae.ats_rewardshortcode sc
               where sc.a_shortcode =
                     NVL(trim(v_2751_Rec.N_3_DIGIT_DISCOUNT_REASON_CODE), 0);
              IF v_ShortCdCount > 0 then
*/
                    v_Reward_Rec                 := Initialize_Reward_Rec();
                    v_Reward_Rec.Certificatecode := NVL(trim(v_2751_Rec.N_20_DIGIT_BARCODE),0);
                    v_Reward_Rec.Certificateredeemtype     := NVL(trim(v_2751_Rec.N_3_DIGIT_DISCOUNT_REASON_CODE),0);
                    v_Reward_Rec.Txnheaderid     := v_Head_Rec.Txnheaderid;
                    v_Reward_Rec.programid       := 0;
                     /* get rowkey */
                    SELECT Seq_Rowkey.Nextval INTO v_Reward_Rec.Rowkey FROM Dual;
                    /* get rewardid */
                    v_Reward_Rec.Txnrewardredeemid := v_Reward_Rec.Rowkey;
                    /* load rec to collection */
                    v_Reward_Tbl.Extend;
                    v_Reward_Tbl(v_Reward_Tbl.Count) := v_Reward_Rec;
                    /* reset rec for next reward */
                    v_Reward_Rec := NULL;
             --END IF ;
           /* AEO 510  Changes end here ------------------------------------------------------ SCJ   */

              /* at item level, ats_txnlineitemdiscount  */
              /* reset/initialize record
              v_discount rec will be added to v_discount_tbl later */

              v_Discount_Rec             := Initialize_Discount_Rec();
              v_Discount_Rec.Offercode   := v_2751_Rec.Discount_Reason_Code;
              v_Discount_Rec.Txndetailid := v_Prev_Detailid;
              v_Discount_Rec.Txnheaderid := v_Head_Rec.Txnheaderid;
               /* redesign changes  begin here --------------------------------------------------  SCJ  */
             /* v_Prev_Txndiscountamount := Nvl(v_Prev_Txndiscountamount,0) - NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);*/
              v_Discount_Rec.Discountamount := NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);
              v_Head_Rec.Txndiscountamount := Nvl(v_Head_Rec.Txndiscountamount,0) +
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0); /* include this amount on the header discount field */
           /*   v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,0) -
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);  \* adjust the header txnqualpurchaseamt, this is our qp *\*/
               /*When discount amount is greater than qualifying purchase amount,then sero out qualifying purchase amount */
            /*if  (Nvl(v_Head_Rec.Txnqualpurchaseamt,0) >= 0 )
              then

                if ((NVL(v_2751_Rec.DISCOUNT_AMOUNT,0) > Nvl(v_Head_Rec.Txnqualpurchaseamt,0))  )
                 then v_Head_Rec.Txnqualpurchaseamt := 0;
                end if;

                if (NVL(v_2751_Rec.DISCOUNT_AMOUNT,0) < Nvl(v_Head_Rec.Txnqualpurchaseamt,0) )
                 then   v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,0) -
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);  \* adjust the header txnqualpurchaseamt, this is our qp *\
                end if;
            end if;
            if   (Nvl(v_Head_Rec.Txnqualpurchaseamt,0) < 0 )
                 then
               v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,0) -
                                                     NVL(trim(v_2751_Rec.DISCOUNT_AMOUNT),0);  \* adjust the header txnqualpurchaseamt, this is our qp *\
            end if;                                       */
              /* redesign changes  begin here --------------------------------------------------  SCJ  */
              IF v_Prev_Rec_Type = '18'
              THEN
                /* collected this from the previous 18 record */
                v_Discount_Rec.Discountamount := v_Prev_Txndiscountamount;
              END IF;
              /* get rowkey */
              SELECT Seq_Rowkey.Nextval
                INTO v_Discount_Rec.Rowkey
                FROM Dual;
              /* get discountid */
              v_Discount_Rec.Txndiscountid := v_Discount_Rec.Rowkey;

              /* load rec to collection, v_discount_tbl will be used to populate stage table */
              v_Discount_Tbl.Extend;
              v_Discount_Tbl(v_Discount_Tbl.Count) := v_Discount_Rec;
              /* reset rec for next discount */
              v_Discount_Rec := NULL;
            END IF;
          END IF;
          /* value card   record */
          IF c.Rec_Type = '27' AND c.User_Id = '62'
          THEN
            /* load raw data to record type */
            v_2762_Rec := Get_Tlog05_2762valuecard(c.Rec);
            v_Detail_Rec              := Initialize_Detail_Rec();
             /*PI 27532 - Let the Gift Cards in the TLOG process start */
            v_Detail_Rec.Processid    := 0;
            v_Detail_Rec.Txntype      := 1;
            v_Detail_Rec.Skunumber    := '15022312';
            v_Detail_Rec.Dtlclasscode := '9911';
            v_Detail_Rec.DtlProductid := Gv_GiftCard_Product_Id;
            v_Detail_Rec.Errormessage := '';
            v_Valid_01_Count          := v_Valid_01_Count + 1;
            /* track count of */
            v_Item_Count := v_Item_Count + 1;

            IF v_2762_Rec.Value_Card_Action = '03'
            THEN
              /* flip sign */
              v_Detail_Rec.Dtlsaleamount := ((To_Number(v_2762_Rec.Load_Amount) * .01) * -1);
              v_Detail_Rec.Dtltypeid     := 2;
            ELSE
              v_Detail_Rec.Dtlsaleamount := (To_Number(v_2762_Rec.Load_Amount) * .01);
              v_Detail_Rec.Dtltypeid     := 1;
            END IF;
            v_Detail_Rec.Dtlsaleamount := Round(v_Detail_Rec.Dtlsaleamount);
            /*PI 27532 - Let the Gift Cards in the TLOG process end */
            v_Detail_Rec.Dtlclearanceitem := 0;
            v_Detail_Rec.Dtlquantity      := 1;
            v_Detail_Rec.Dtlitemlinenbr   := v_Item_Count;
            v_Detail_Rec.Filelineitem     := c.Filelineitem;
            /* get rowkey for detail */
            v_Detail_Rec.Rowkey := Seq_Rowkey.Nextval;

            /* get txndetail id, use rowkey as client does not provide a key */
            v_Detail_Rec.Txndetailid := v_Detail_Rec.Rowkey;
            --v_head_rec.txnqualpurchaseamt := nvl(v_head_rec.txnqualpurchaseamt,0) + (v_detail_rec.dtlquantity * v_detail_rec.dtlsaleamount);

            /* load record to main array, this will be used to load stage table */
            v_Detail_Tbl.Extend;
            v_Detail_Tbl(v_Detail_Tbl.Count) := v_Detail_Rec;

          END IF;
          /* multi division master transaciton  record / kids master  */
          IF c.Rec_Type = '27' AND c.User_Id = '77'
          THEN
            /* load temp array with exteral tbl rec */
            v_2777_Rec := Get_Tlog05_2777multidivmsttxn(c.Rec);
            /* load alt info */
            v_Head_Rec.Alttxnnumber         := Nvl(TRIM(v_2777_Rec.Alt_Transaction_Num),
                                                   v_00_Rec.Transaction_Number);
            v_Head_Rec.Alttxndate           := Nvl(Valid_Date(v_2777_Rec.Alt_Transaction_Date,
                                                              'MMDDRR'),
                                                   v_Head_Rec.Txndate);
            v_Head_Rec.Alttxnregisternumber := Nvl(TRIM(v_2777_Rec.Alt_Div_Register),
                                                   v_00_Rec.Register_Number);
            v_Head_Rec.Altstorenumber       := Nvl(TRIM(v_2777_Rec.Alt_Div_Store),
                                                   v_00_Rec.Store_Number);
            /* define kids type (child or master) */
            /* set processid type */
            IF v_2777_Rec.Is_Child = 'Y'
            THEN
              v_Head_Rec.Kids77type := 'c'; /* child */
              v_Head_Rec.Processid  := Gv_Status_77child;
            ELSE
              v_Head_Rec.Kids77type := 'm'; /* master */
              v_Head_Rec.Processid  := Gv_Status_77master;
            END IF;

            /* temp data for later processing in id lookups */
            v_Txt1 := v_Head_Rec.Kids77type;
            v_N1   := v_Head_Rec.Txnheaderid;

            /* determine what the match process id should be before a merge can be determined */
            IF v_Head_Rec.Processid = Gv_Status_77child
            THEN
              v_Matched_Processid := Gv_Status_77master;
            ELSE
              v_Matched_Processid := Gv_Status_77child;
            END IF;
            /* log headerid if new, quire matched kid77 headerid if already logged */
            IF v_Head_Rec.Processid = Gv_Status_77master
            THEN
              Get_Headerid(p_Store_Nbr => v_Head_Rec.Altstorenumber,
                           p_Reg_Nbr   => v_Head_Rec.Alttxnregisternumber,
                           p_Txn_Date  => v_Head_Rec.Alttxndate,
                           p_Txn_Nbr   => v_Head_Rec.Alttxnnumber,
                           p_Processid => v_Head_Rec.Processid,
                           p_Headerid  => v_Head_Rec.Txnheaderid,
                           p_Rec_Type  => v_Txt1,
                           p_Action    => v_Txt2
                  /*AEO-727 changes begin here ---------------------------SCJ*/
                  ,p_Ordernumber => v_Head_Rec.ordernumber
                  /*AEO-727 changes begin here ---------------------------SCJ*/
                           );
            ELSE
              /* if child rec has zeros for regnbr/txnnbr then we need this */
              Get_Headerid(p_Store_Nbr => v_00_Rec.Store_Number --  p_head_rec.altstorenumber
                          ,
                           p_Reg_Nbr   => v_Head_Rec.Txnregisternumber --p_head_rec.alttxnregisternumber
                          ,
                           p_Txn_Date  => v_Head_Rec.Txndate --p_head_rec.alttxndate
                          ,
                           p_Txn_Nbr   => v_Head_Rec.Txnnumber --p_head_rec.alttxnnumber
                          ,
                           p_Processid => v_Head_Rec.Processid,
                           p_Headerid  => v_Head_Rec.Txnheaderid,
                           p_Rec_Type  => v_Txt1 --v_txt1
                          ,
                           p_Action    => v_Txt2 --v_txt2
             /*AEO-727 changes begin here ---------------------------SCJ*/
                          ,p_Ordernumber => v_Head_Rec.ordernumber
             /*AEO-727 changes begin here ---------------------------SCJ*/
                           );
            END IF;
            v_Head_Rec.Rowkey := v_Head_Rec.Txnheaderid;
            /***** start kids 77 merge process ****/
            /*  a lot of replication will occur here.
                which ever processed fisr (child or master) will be abandoned in the hist table
                this process will aquire the ogrinal processed data and merge it
                in with this txn.
            */

            /* if found we must merge the master with the child */
            /* v_txt2 acuried form previous proc call (out variable) */
            IF v_Txt2 = 'merge'
              /* check that the original is not the same rec type... could just have processed master or child twice, treat like no match */
               AND v_Txt1 != v_Head_Rec.Kids77type
            THEN
              /* determine if matching data is staged or in history */
              IF v_Head_Rec.Txnheaderid > v_Min_Headerid_This_Job
              THEN
                v_Txt1 := 'stage';
              ELSE
                v_Txt1 := 'hist';
              END IF;
              /* get all discounts from matched 77 txn, replicate to this txn */
              FOR y IN Get_Discounts(v_Head_Rec.Txnheaderid,
                                     v_Txt1,
                                     v_Matched_Processid) LOOP
                v_Discount_Rec := Initialize_Discount_Rec;

                v_Discount_Rec.Ipcode         := y.Ipcode;
                v_Discount_Rec.Txndiscountid  := y.Txndiscountid;
                v_Discount_Rec.Txnheaderid    := y.Txnheaderid;
                v_Discount_Rec.Txndate        := y.Txndate;
                v_Discount_Rec.Txndetailid    := y.Txndetailid;
                v_Discount_Rec.Discounttype   := y.Discounttype;
                v_Discount_Rec.Discountamount := y.Discountamount;
                v_Discount_Rec.Txnchannel     := y.Txnchannel;
                v_Discount_Rec.Offercode      := y.Offercode;
                v_Discount_Rec.Statuscode     := y.Statuscode;
                v_Discount_Rec.Fileid         := y.Fileid;
                --v_discount_rec.CREATEDATE := y.CREATEDATE;
                --v_discount_rec.UPDATEDATE := y.UPDATEDATE;
                --v_discount_rec.LASTDMLID := y.LASTDMLID;
                /* get new id's */
                --v_discount_rec.ROWKEY := y.ROWKEY;
                --v_discount_rec.TXNDETAILID := y.TXNDETAILID;
                /* get rowkey */
                v_Discount_Rec.Rowkey := Seq_Rowkey.Nextval;
                /* get discountid */
                v_Discount_Rec.Txndiscountid := v_Discount_Rec.Rowkey;

                v_Discount_Tbl.Extend;
                v_Discount_Tbl(v_Discount_Tbl.Count) := v_Discount_Rec;
              END LOOP;

              /* replicate rewards from matched txn to this one */
              FOR y IN Get_Rewards(v_Head_Rec.Txnheaderid,
                                   v_Txt1,
                                   v_Matched_Processid) LOOP
                v_Reward_Rec                           := Initialize_Reward_Rec;
                v_Reward_Rec.Txnheaderid               := y.Txnheaderid;
                v_Reward_Rec.Txndate                   := y.Txndate;
                v_Reward_Rec.Txndetailid               := y.Txndetailid;
                v_Reward_Rec.Programid                 := y.Programid;
                v_Reward_Rec.Certificateredeemtype     := y.Certificateredeemtype;
                v_Reward_Rec.Certificatecode           := y.Certificatecode;
                v_Reward_Rec.Certificatediscountamount := y.Certificatediscountamount;
                v_Reward_Rec.Statuscode                := y.Statuscode;
                --v_reward_rec.CREATEDATE := y.CREATEDATE;
                --v_reward_rec.UPDATEDATE := y.UPDATEDATE;
                --v_reward_rec.LASTDMLID := y.LASTDMLID;
                /* get new id's */
                --v_reward_rec.ROWKEY := y.ROWKEY;
                --v_reward_rec.TXNREWARDREDEEMID := y.TXNREWARDREDEEMID;

                /* get rowkey */
                v_Reward_Rec.Rowkey := Seq_Rowkey.Nextval;
                /* get rewardid */
                v_Reward_Rec.Txnrewardredeemid := v_Reward_Rec.Rowkey;

                v_Reward_Tbl.Extend;
                v_Reward_Tbl(v_Reward_Tbl.Count) := v_Reward_Rec;
              END LOOP;
              /* replicate matched tenders to this txn */
              FOR y IN Get_Tenders(v_Head_Rec.Txnheaderid,
                                   v_Txt1,
                                   v_Matched_Processid) LOOP
                v_Tender_Rec                := Initialize_Tender_Rec;
                v_Tender_Rec.Processid      := y.Processid;
                v_Tender_Rec.Storeid        := y.Storeid;
                v_Tender_Rec.Txndate        := y.Txndate;
                v_Tender_Rec.Txnheaderid    := y.Txnheaderid;
                v_Tender_Rec.Tendertype     := y.Tendertype;
                v_Tender_Rec.Tenderamount   := y.Tenderamount;
                v_Tender_Rec.Tendercurrency := y.Tendercurrency;
                v_Tender_Rec.Tendertax      := y.Tendertax;
                v_Tender_Rec.Tendertaxrate  := y.Tendertaxrate;
                v_Tender_Rec.Statuscode     := y.Statuscode;
                v_Tender_Rec.Fileid         := y.Fileid;
                --v_tender_rec.CREATEDATE := y.CREATEDATE;
                --v_tender_rec.UPDATEDATE := y.UPDATEDATE;
                --v_tender_rec.LASTDMLID := y.LASTDMLID;
                /* get new id's */
                --v_tender_rec.TXNTENDERID := y.TXNTENDERID;
                --v_tender_rec.ROWKEY := y.ROWKEY;
                /* get rowkey */
                v_Tender_Rec.Rowkey := Seq_Rowkey.Nextval;
                /* get tender_id */
                v_Tender_Rec.Txntenderid := Seq_Rowkey.Currval;

                v_Tender_Tbl.Extend;
                v_Tender_Tbl(v_Tender_Tbl.Count) := v_Tender_Rec;
                /* added 20120410 (if and contents) */
                IF v_tender_rec.Tendertype != 69 THEN
                   v_Head_Rec.Ttltenderamount := Nvl(v_Head_Rec.Ttltenderamount, 0) +
                                                     v_Tender_Rec.Tenderamount;
                END IF;
              END LOOP;
              /* replicate matched details to this txn */
              FOR y IN Get_Details(v_Head_Rec.Txnheaderid,
                                   v_Txt1,
                                   v_Matched_Processid) LOOP
                v_Detail_Rec := Initialize_Detail_Rec;
                IF Get_Details%ROWCOUNT = 1
                THEN
                  IF v_Head_Rec.Kids77type = 'm'
                  THEN
                    /* these details are children */
                    /* if child rec is a purchase then use it's storeid */
                    IF v_Detail_Rec.Txntype = 1
                    THEN
                      v_Head_Rec.Txnstoreid := y.Txnstoreid;
                    END IF;
                    /* default these from child */
                    v_Head_Rec.Txnregisternumber    := y.Txnregisternumber;
                    v_Head_Rec.Txnoriginaltxnrowkey := y.Txnoriginaltxnrowkey;
                  ELSE
                    /* these details are the master */
                    IF v_Head_Rec.Txntype != 1
                    THEN
                      /* if child rec is not a purchase then use master's storeid */
                      v_Head_Rec.Txnstoreid := y.Txnstoreid;
                    END IF;
                    /* take master infor for these attributes */
                    v_Head_Rec.Txnnumber   := y.Txnnumber;
                    v_Head_Rec.Ordernumber := y.Ordernumber;
                    v_Head_Rec.Txndate     := y.Txndate;
                  END IF;
                  /* do this once, values are already summarized on cursor */
                  v_Head_Rec.Txnamount          := Nvl(v_Head_Rec.Txnamount,
                                                       0) + y.Txnamount;
                  v_Head_Rec.Txnqualpurchaseamt := Nvl(v_Head_Rec.Txnqualpurchaseamt,
                                                       0) +
                                                   y.Txnqualpurchaseamt;
                  v_Head_Rec.Txndiscountamount  := Nvl(v_Head_Rec.Txndiscountamount,
                                                       0) +
                                                   y.Txndiscountamount;
                  /* aquire best loyalty/vckey info */
                  IF v_Head_Rec.Vckey IS NULL
                  THEN
                    IF y.Vckey IS NOT NULL
                    THEN
                      v_Head_Rec.Vckey        := y.Vckey;
                      v_Head_Rec.Txnloyaltyid := y.Txnloyaltyid;
                    ELSIF y.Txnloyaltyid IS NOT NULL
                    THEN
                      v_Head_Rec.Txnloyaltyid := y.Txnloyaltyid;
                    END IF;
                  END IF;
                  v_Head_Rec.Kids77type := 'merged';
                END IF;
                v_Item_Count := v_Item_Count + 1;
                /* get rowkey for detail */
                v_Detail_Rec.Rowkey := Seq_Rowkey.Nextval;

                /* get txndetail id */
                v_Detail_Rec.Txndetailid := v_Detail_Rec.Rowkey;

                v_Detail_Rec.Dtlquantity          := y.Dtlquantity;
                v_Detail_Rec.Dtldiscountamount    := y.Dtldiscountamount;
                v_Detail_Rec.Dtlclearanceitem     := y.Dtlclearanceitem;
                v_Detail_Rec.Dtldatemodified      := y.Dtldatemodified;
                v_Detail_Rec.Reconcilestatus      := y.Reconcilestatus;
                v_Detail_Rec.Brandid              := y.Brandid;
                v_Detail_Rec.Fileid               := y.Fileid;
                v_Detail_Rec.Processid            := v_Detail_Rec.Processid;
                v_Detail_Rec.Filelineitem         := y.Filelineitem;
                v_Detail_Rec.Cardid               := y.Cardid;
                v_Detail_Rec.Creditcardid         := y.Creditcardid;
                v_Detail_Rec.Txnloyaltyid         := y.Txnloyaltyid;
                v_Detail_Rec.Txnmaskid            := y.Txnmaskid;
                v_Detail_Rec.Txnnumber            := y.Txnnumber;
                v_Detail_Rec.Txndate              := y.Txndate;
                v_Detail_Rec.Txndatemodified      := y.Txndatemodified;
                v_Detail_Rec.Txnregisternumber    := y.Txnregisternumber;
                v_Detail_Rec.Txnstoreid           := y.Txnstoreid;
                v_Detail_Rec.Txntype              := y.Txntypeid;
                v_Detail_Rec.Txnamount            := y.Txnamount;
                v_Detail_Rec.Txndiscountamount    := y.Txndiscountamount;
                v_Detail_Rec.Txnemailaddress      := y.Txnemailaddress;
                v_Detail_Rec.Txnphonenumber       := y.Txnphonenumber;
                v_Detail_Rec.Txnemployeeid        := y.Txnemployeeid;
                v_Detail_Rec.Txnchannelid         := y.Txnchannelid;
                v_Detail_Rec.Txnoriginaltxnrowkey := y.Txnoriginaltxnrowkey;
                v_Detail_Rec.Txncreditsused       := y.Txncreditsused;
                v_Detail_Rec.Dtlitemlinenbr       := v_Item_Count;
                v_Detail_Rec.Dtlproductid         := y.Dtlproductid;
                v_Detail_Rec.Dtltypeid            := y.Dtltypeid;
                v_Detail_Rec.Dtlactionid          := y.Dtlactionid;
                v_Detail_Rec.Dtlretailamount      := y.Dtlretailamount;
                v_Detail_Rec.Dtlsaleamount        := y.Dtlsaleamount;
                v_Detail_Rec.Shipdate             := y.Shipdate;
                v_Detail_Rec.Statuscode           := y.Statuscode;
                v_Detail_Rec.Createdate           := y.Createdate;
                v_Detail_Rec.Updatedate           := y.Updatedate;
                v_Detail_Rec.Lastdmlid            := y.Lastdmlid;
                v_Detail_Rec.Dtlclasscode         := y.Dtlclasscode;
                v_Detail_Rec.Skunumber            := y.Skunumber;
                v_Valid_01_Count                  := v_Valid_01_Count + 1;
                IF v_Detail_Rec.Processid IN
                   (Gv_Status_77child, Gv_Status_77master)
                THEN
                  v_Detail_Rec.Processid := NULL;
                END IF;
                IF v_Detail_Rec.Dtlproductid = v_Unknown_Product_Id
                THEN
                  /* check if product is there now */
                  BEGIN
                    SELECT p.Brandname, p.Id
                      INTO v_Detail_Rec.Brandid, v_Detail_Rec.Dtlproductid
                      FROM Lw_Product p
                     WHERE p.Partnumber = v_Detail_Rec.Skunumber;
                  EXCEPTION
                    WHEN No_Data_Found THEN
                      /* log missing product, has new header id now. */
                      INSERT INTO Log_Missingproduct
                        (Txndetailid,
                         Sku_Class_Number,
                         Class_Number,
                         Transaction_Date,
                         Store_Number,
                         Transaction_Number,
                         Register_Number,
                         Error_Date,
                         Filename,
                         Jobnumber)
                      VALUES
                        (v_Detail_Rec.Txndetailid,
                         v_01_Rec.Sku_Class_Number,
                         v_01_Rec.Class_Number,
                         v_Head_Rec.Txndate,
                         v_Head_Rec.Txnstoreid,
                         v_Head_Rec.Txnnumber,
                         v_Head_Rec.Txnregisternumber,
                         SYSDATE,
                         v_Filename,
                         v_My_Log_Id);
                      /* assign unknown/default productid */
                      v_Detail_Rec.Dtlproductid := v_Unknown_Product_Id;
                      /* mark 01 record so it wont be process, but keep for reporting */
                      v_Detail_Rec.Processid    := Gv_Status_Noproduct;
                      v_Detail_Rec.Errormessage := 'Unknown SKU';
                  END;
                ELSIF v_Detail_Rec.Skunumber = 'GiftCard'
                THEN
                  v_Detail_Rec.Processid := Gv_Status_Giftcarditem;
                  v_Detail_Rec.Brandid   := 1;

                  -- default this to 0 so it account activity on the customer service site will not throw an error.
                  v_Detail_Rec.Dtlclasscode := 0;

                ELSIF v_Detail_Rec.Skunumber = 'GiftCert'
                THEN
                  v_Detail_Rec.Processid := Gv_Status_Giftcertitem;
                  v_Detail_Rec.Brandid   := 1;

                  -- default this to 0 so it account activity on the customer service site will not throw an error.
                  v_Detail_Rec.Dtlclasscode := 0;
                END IF;
                v_Detail_Tbl.Extend;
                v_Detail_Tbl(v_Detail_Tbl.Count) := v_Detail_Rec;
              END LOOP;
              v_Head_Rec.Errormessage := 'Merged 77kids rec';
            END IF;

            /***** end kids 77 merge process ****/
          END IF;
          /*    dont need this, not on tlog spec doc
                  IF c.rec_type = '27' AND c.user_id ='92' THEN
                    v_2792_rec :=get_Tlog05_2792pomogiftsku (c.rec);
                    --do something here
                  END IF;
          */
          /* Loyalty ID   record */
          IF c.Rec_Type = '27' AND c.User_Id = '93'
          THEN
            /* load raw data to record type */
            v_2793_Rec              := Get_Tlog05_2793custloyalyt(p_Rec => c.Rec);
            v_Head_Rec.Txnloyaltyid := TRIM(v_2793_Rec.Loyalty_Number);
              /*National Rollout Changes AEO-1218 BEGIN here        ----------------SCJ*/
            v_memberstatus := 1;
            select count(VC.VCKEY)
              into v_loyaltyidcount
              from LW_VIRTUALCARD VC
             WHERE Vc.Loyaltyidnumber = TRIM(v_Head_Rec.Txnloyaltyid);
            IF (v_loyaltyidcount > 0) THEN
              SELECT Nvl(MAX(Vckey), 0),
                     Nvl(MAX(Vc.Ipcode), 0),
                     NVL(max(LM.MEMBERSTATUS), 1)
                INTO v_Head_Rec.Vckey, v_Head_Rec.Ipcode, v_memberstatus
                FROM Lw_Virtualcard Vc
               INNER JOIN LW_LOYALTYMEMBER LM
                  ON LM.IPCODE = VC.IPCODE
              /* inner join lw_loyaltymember lm on Vc.Ipcode = lm.ipcode*/
               WHERE Vc.Loyaltyidnumber = TRIM(v_Head_Rec.Txnloyaltyid)
               group by vc.ipcode;
            ELSE
              v_Head_Rec.Vckey  := 0;
              v_Head_Rec.Ipcode := 0;
              v_memberstatus    := 1;
            END IF;
            IF v_Head_Rec.txntype = 1 AND v_memberstatus = 2 AND
               v_Head_Rec.Vckey > 0 THEN
              Tlog_UTIL.ReActivateMember(TRIM(v_Head_Rec.Txnloyaltyid));
            END IF;
            /*National Rollout Changes AEO-1218 END here        ----------------SCJ*/

            IF v_Head_Rec.Vckey = 0 AND
               v_Head_Rec.Processid NOT IN
               (Gv_Status_77child, Gv_Status_77master, Gv_Status_Dup)
            THEN
              v_Head_Rec.Processid    := Gv_Status_Unregcard;
              v_Head_Rec.Errormessage := 'LoyaltyID not found in: lw_virtualcard';
            ELSIF v_Head_Rec.Processid NOT IN
                  (Gv_Status_77child, Gv_Status_77master, Gv_Status_Dup)
            THEN
              v_Head_Rec.Processid := Gv_Status_Ready;
            END IF;
          END IF;
          /* direct/web order number   record */
          IF c.Rec_Type = '27' AND c.User_Id = '99'
          THEN
            /* load raw data to record type */
            v_2799_Rec := Get_Tlog05_2799directordernbr(c.Rec);
            /* aquire new txndate based on the order date */
            v_Date := Valid_Date(v_2799_Rec.Order_Date, 'yyyymmdd');
            IF v_2799_Rec.CURRENCY_CODE is not null -------------------AEO-844 Changes begin here ---------SCJ
              THEN
             v_Head_Rec.CURRENCY_CODE := v_2799_Rec.CURRENCY_CODE;
             END IF; -------------------AEO-844 Changes end here ---------SCJ
            IF v_Date IS NOT NULL
            THEN
              /* re-assign txndate to the order date, and set shipdate as txndate */
              v_Head_Rec.OriginalOrderNumber := trim(v_2799_Rec.ORIGINAL_ORDER_NUMBER);    /*National Rollout Changes AEO-1218 END here        ----------------SCJ*/
              v_Head_Rec.Shipdate    := v_Head_Rec.Txndate;
              v_Head_Rec.Ordernumber := v_2799_Rec.Order_Number;
              v_Head_Rec.Txndate     := v_Date;
            END IF;
          END IF;
        END IF;
      END IF;
      /* skips go here */
      <<skip_Point>>
      IF c.Rec_Type NOT IN ('27') AND c.User_Id NOT IN ('51')
      THEN
        v_Prev_Rec_Type := c.Rec_Type;
      END IF;
      FETCH Cur_Raw INTO c;

    END LOOP;

    /* end of looping, we still have the last txn to process */
    <<the_End>>
  /*
    --------------------------------------------------------------------------------------------
    --------------final write of last row, duplicated process from top of proc ----------------- */
    /* process the previous txn here but only if the line count matched*/
    IF ( (v_This_Line_Cnt - 1) = v_Ttl_Line_Cnt AND v_Skip = 0
      OR (v_This_Line_Cnt = v_Ttl_Line_Cnt AND v_Skip = 0))
    THEN
      /* writes to stage tables */
      Load_Stage_Data(p_Head_Rec       => v_Head_Rec,
                      p_Detail_Tbl     => v_Detail_Tbl,
                      p_Tender_Tbl     => v_Tender_Tbl,
                      p_Discount_Tbl   => v_Discount_Tbl,
                      p_Reward_Tbl     => v_Reward_Tbl,
                      p_My_Log_Id      => v_My_Log_Id,
                      p_Valid_01_Count => v_Valid_01_Count,
                      p_Txn_Count      => v_Txn_Count,
                      p_Store_Nbr      => v_00_Rec.Store_Number,
                      p_Rec_Type       => v_Txt1,
                      p_Action         => v_Txt2);
      /* add to final count */
      IF v_Detail_Tbl.Count > 0
      THEN
        IF      v_Head_Rec.Processid = Gv_Status_Ready
        THEN
          v_Messagespassed := v_Messagespassed + 1;
         /*PI 24762 begin */
        ELSIF   v_Head_Rec.Processid = Gv_Status_Noproduct
             or v_Head_Rec.Processid = Gv_Status_Error
             or v_Head_Rec.Processid = Gv_Status_Dup
        THEN
            /*PI 24762 end */
          v_Messagesfailed := v_Messagesfailed + 1;
        END IF;
      END IF;
    END IF;
    END IF;
    CLOSE Cur_Raw;
    END;
    /* make final write to tables */
    write_tlog_dup_exception(); -- AEO-2090
    write_Stage_Data();
    /* log final count */
    Log_Process_Count(v_Txn_Count);
    /*--------------------------------------------------------------------------------------------
      run final state/logging processes
    */
    Log_Process_State('Ending');

    v_Messagesreceived := v_Messagespassed + v_Messagesfailed;
    v_Endtime          := SYSDATE;
    v_Jobstatus        := 1;
    IF v_Messagesreceived > v_Messagespassed AND v_Messagespassed = 0
    THEN
      /* nothing to process, mark as failure */
      v_Jobstatus := 3;
      v_Error     := 'No valid rows found';
      v_Reason    := 'All rows failed validation';
      v_Message   := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                     '    <pkg>Stage_Tlog5</pkg>' || Chr(10) ||
                     '    <proc>Stage_Tlog05</proc>' || Chr(10) ||
                     '    <filename>' || v_Filename || '</filename>' ||
                     Chr(10) || '  </details>' || Chr(10) || '</failed>';
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
    END IF;

    /* log end of job */
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => 'Stage-' || v_Jobname);

    /* create job for dap */
    /*
    Utility_Pkg.Log_Job(p_Job              => v_Dap_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => SYSDATE,
                        p_Endtime          => NULL,
                        p_Messagesreceived => NULL,
                        p_Messagesfailed   => NULL,
                        p_Jobstatus        => 0,
                        p_Jobname          => 'DAP-' || v_Jobname);

    OPEN Retval FOR
      SELECT v_Dap_Log_Id FROM Dual;
  */
    /* end state on job tracking */
    UPDATE Log_Process_Stat
       SET Job_End       = SYSDATE,
           Sid           = 0,
           Inst_Id       = 0,
           State         = 'complete',
           Current_Count = v_Txn_Count
     WHERE Id = v_Process_Id;

    /* commit does final write, all data saved at this point */
    COMMIT;



/*Redesign changes begin here ------------------------------------SCJ */
    UpdateRewardRedemptiondate();
/*Redesign changes end here ------------------------------------SCJ */
    /* logs error then raises exception */
  EXCEPTION
    WHEN OTHERS THEN
      /* rollback activity that occured ? or replace with commit? */
      ROLLBACK;
      IF v_Messagesfailed = 0
      THEN
        v_Messagesfailed := 1;
      END IF;
      v_Jobstatus := 3;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => v_Filename,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => 'Stage5' || v_Jobname);
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure Stage_Tlog05';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>Stage_Tlog5</pkg>' || Chr(10) ||
                          '    <proc>Stage_Tlog05</proc>' || Chr(10) ||
                          '    <filename>' || v_Filename || '</filename>' ||
                          '    <string>' || v_string || '</string>' ||
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
      /* end state on job tracking */
      UPDATE Log_Process_Stat
         SET Job_End       = SYSDATE,
             Sid           = 0,
             Inst_Id       = 0,
             State         = 'failure',
             Current_Count = v_Txn_Count
       WHERE Id = v_Process_Id;

      /* commit this logging actions */
      COMMIT;
      /* raise error so calling process knows it failed */
      RAISE;
  END Stage_Tlog05;


  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
END Stage_Tlog5;
/
