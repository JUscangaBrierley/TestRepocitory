CREATE OR REPLACE PACKAGE Stage_Tlog8_WorldSpend IS

  /*
      usage

      Stage_tlog01 is the main process
         It will run hist_tlog01 for you.
         Hist_tlog01 is called prior to processing... so only the privious data is pushed to hist.

      Hist_tlog01
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
  Gv_Process       CONSTANT VARCHAR2(256) := Lower(TRIM('stage_tlog01'));
  Gv_Process_Id NUMBER := 0;
  Gv_GiftCard_Product_Id NUMBER := 0;

  TYPE Rcursor IS REF CURSOR;

  FUNCTION Get_Parthighvalue(p_Table_Name     VARCHAR2,
                             p_Partition_Name VARCHAR2) RETURN VARCHAR2;

  PROCEDURE Process_Tlog(p_Filename   VARCHAR2 DEFAULT NULL,
                         Retval       IN OUT Rcursor);
  PROCEDURE Hist_Tlog01(p_Dummy VARCHAR2, p_Process_Date Date,
                        Retval  IN OUT Rcursor);


END Stage_Tlog8_WorldSpend;
/
CREATE OR REPLACE PACKAGE BODY Stage_Tlog8_WorldSpend IS
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
  PROCEDURE Initialize_Tbl(p_Filename VARCHAR2) IS
    v_Sql            VARCHAR2(4000);

  BEGIN
    /*
       set the external tables filename
       also clears out the external table log file
    */
    v_Sql := 'ALTER TABLE EXT_TLOG08_WorldSpend' || Chr(10) || 'LOCATION (AE_IN' ||
             Chr(58) || '''' || p_Filename || ''')';
    EXECUTE IMMEDIATE v_Sql;
    Clear_Infile('EXT_TLOG08_WorldSpend_log');
  END Initialize_Tbl;


  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_Process_Count(p_Rowcount NUMBER) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
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
  PROCEDURE Get_Headerid(p_Store_Nbr IN VARCHAR2,
                         p_Reg_Nbr   IN VARCHAR2,
                         p_Txn_Date  IN DATE,
                         p_Txn_Nbr   IN VARCHAR2,
                         p_Processid IN OUT NUMBER,
                         p_Rec_Type  IN OUT VARCHAR2,
                         p_Action    OUT VARCHAR2,
                         p_Headerid  OUT NUMBER) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    v_Headerid  NUMBER;
    v_Rec_Type  VARCHAR2(30);
    v_Processid NUMBER;
    n           NUMBER;
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
       AND Txn_Nbr = TRIM(p_Txn_Nbr);

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
           AND Txn_Nbr = TRIM(p_Txn_Nbr);

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
        INSERT INTO Log_Txn_Keys_Stage
          (Txnheaderid,
           Store_Nbr,
           Register_Nbr,
           Txn_Date,
           Txn_Nbr,
           First_Rec_Type,
           Processid)
        VALUES
          (p_Headerid,
           TRIM(p_Store_Nbr),
           TRIM(p_Reg_Nbr),
           Trunc(p_Txn_Date),
           TRIM(p_Txn_Nbr),
           Lower(Substr(p_Rec_Type, 1, 1)),
           p_Processid);
        COMMIT;
        p_Action := 'new';
        RETURN;
      EXCEPTION
        WHEN Dup_Val_On_Index THEN
          /* header id already defined in this run */
          /* get the header id defined by this run */
          SELECT Txnheaderid, First_Rec_Type, Processid
            INTO v_Headerid, v_Rec_Type, v_Processid
            FROM Log_Txn_Keys_Stage
           WHERE Store_Nbr = TRIM(p_Store_Nbr)
             AND Register_Nbr = TRIM(p_Reg_Nbr)
             AND Txn_Date = Trunc(p_Txn_Date)
             AND Txn_Nbr = TRIM(p_Txn_Nbr);

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
  /******************* Internal function to tranform date ************/

  PROCEDURE Process_Tlog(p_Filename   VARCHAR2 DEFAULT NULL,
                         Retval       IN OUT Rcursor) IS

    v_Txn_Count              NUMBER := 0;
    v_Filename               VARCHAR2(512) := Nvl(p_Filename, 'tlog.txt');
    v_My_Log_Id              NUMBER;
    v_Dap_Log_Id             NUMBER;
    /* main cursor */
    CURSOR Cur_Raw IS
      SELECT LoyaltyNumber
            ,AccountKey
            ,StartDate
            ,EndDate
            ,NetWorldSales
            , Rownum AS Filelineitem
        FROM EXT_TLOG08_WorldSpend;
    c Cur_Raw%ROWTYPE;
    --log job attributes
    v_Jobdirection           NUMBER := 0;
    v_Starttime              DATE   := SYSDATE;
    v_Endtime                DATE;
    v_Messagesreceived       NUMBER := 0;
    v_Messagesfailed         NUMBER := 0;
    v_Messagespassed         NUMBER := 0;
    v_Txt1                   VARCHAR2(256);
    v_Dt                     DATE;
    v_Jobstatus              NUMBER := 0;
    v_Jobname                VARCHAR2(256) := 'Tlog8_WorldSpend';
    --log msg attributes
    v_Messageid              NUMBER;
    v_Envkey                 VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
    v_Logsource              VARCHAR2(256) := 'TLog01';
    v_Batchid                VARCHAR2(256) := 0;
    v_Message                VARCHAR2(256);
    v_Reason                 VARCHAR2(256);
    v_Error                  VARCHAR2(256);
    v_Trycount               NUMBER := 0;
    v_Process_Id             NUMBER := 0;
    v_IpCode                 NUMBER;
    v_VCKey                  NUMBER;
    v_StartDate              Timestamp(4);
    v_EndDate                Timestamp(4);
    v_NetWorldSales          FLOAT;
    v_HeaderID               NUMBER;
    v_RowKey                 NUMBER;
    v_detailid               NUMBER;
    v_RecType                VARCHAR2(256) := '00';
    v_Action                 VARCHAR2(256) := 'new';
    v_TxnNumber              Varchar2(25);
    v_StoreNumber            Varchar2(25);
    v_Reg_Nbr                VARCHAR2(3);
    v_Txn_Date               DATE;
    v_exception_msg          VARCHAR2(256);
    v_ExtendedPlayCode       NUMBER;
    v_found                  BOOLEAN;
    -- AEO-620 Exceptions
    RejectRecord             EXCEPTION;

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
    -- Log_process_state('S:hist_tlog01');
      /* prep staging tables, if data already exists in stage tables then it gets moved to hist */
    --  hist_tlog01();
    /* set job state */
    Log_Process_State('running');
    /* end of job state tracking */

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

    /* this will point the external table to passed filename and resets the ext tables log file */
    Initialize_Tbl(v_Filename);

    BEGIN
      OPEN Cur_Raw;
      FETCH Cur_Raw INTO c;

      /* main loop, hitting tlog file */
      LOOP
        EXIT WHEN cur_raw%NOTFOUND;
          BEGIN
              v_exception_msg := NULL;
              v_found := FALSE;
              -- AEP-620 Reject record if both Loyalty Number and Account key are blank
              IF c.LoyaltyNumber IS NULL AND c.AccountKey IS NULL THEN
                v_exception_msg := 'FAILURE: Loyalty Number and Account Key are NULL'; -- AEO-1885
                -- fatal row exception so reject the entire record
                RAISE RejectRecord;
              END IF;

              IF c.accountkey IS NOT NULL AND c.loyaltynumber IS NOT NULL THEN
                BEGIN
                  -- Match on account key and loyalty id
                  SELECT DISTINCT vc.vckey
                                , vc.ipcode
                                , md.a_extendedplaycode
                  INTO     v_vckey
                         , v_ipcode
                         , v_ExtendedPlayCode
                  FROM   bp_ae.ats_synchronyaccountkey k
                       , bp_ae.lw_virtualcard          vc
                       , bp_ae.ats_memberdetails       md
                  WHERE  k.a_accountkey = c.accountkey AND
                         vc.vckey = k.a_vckey AND
                         vc.loyaltyidnumber = c.LoyaltyNumber AND
                         --vc.isprimary = 1 AND --AEO-2034
                         md.a_ipcode = vc.ipcode;

                 v_found := TRUE;
                EXCEPTION
                  WHEN no_data_found THEN
                    -- will live to see another day
                    v_exception_msg := 'Loyalty Number and Account Key match failed';
                  WHEN too_many_rows THEN
                    -- fatal exception for the row
                    v_exception_msg := 'Loyalty Number and Account Key returned too many matches'; --AEO-1885
                   -- RAISE RejectRecord;
                  WHEN OTHERS THEN
                    NULL;
                    /*
                    -- non specified exception so get exception message from oracle
                    v_exception_msg := substr('FAILURE: '||v_exception_msg||' '|| SQLERRM,1,256);
                    RAISE RejectRecord;*/
                END;
              END IF; -- match on account key and loyalty id



              IF NOT v_found AND c.accountkey IS NOT NULL THEN
                BEGIN
                  -- match on account key
                  SELECT DISTINCT vc.vckey
                                , vc.ipcode
                                , md.a_extendedplaycode
                  INTO     v_vckey
                         , v_ipcode
                         , v_ExtendedPlayCode
                  FROM   bp_ae.ats_synchronyaccountkey k
                       , bp_ae.lw_virtualcard          vc
                       , bp_ae.ats_memberdetails       md
                  WHERE  k.a_accountkey = c.accountkey 
                         AND vc.vckey = k.a_vckey 
                         --AND vc.isprimary = 1 --AEO-2034
                         AND md.a_ipcode = vc.ipcode
                         ;

                  v_found := TRUE;

                  IF v_exception_msg IS NOT NULL THEN
                    v_exception_msg := v_exception_msg || ' | Matched using Account Key';
                  ELSE
                    v_exception_msg := 'Matched using Account Key';
                  END IF;

                EXCEPTION
                  WHEN no_data_found THEN
                    IF v_exception_msg IS NULL THEN
                      v_exception_msg := 'Account Key match failed';
                    ELSE
                      v_exception_msg := v_exception_msg||' | Account Key match failed';
                    END IF;
                  WHEN too_many_rows THEN
                    IF v_exception_msg IS NULL THEN
                       v_exception_msg := 'Account Key match returned too many matches';
                    ELSE
                      v_exception_msg := v_exception_msg || ' | Account Key match returned too many matches';
                    END IF;
                 --   RAISE RejectRecord;
                  WHEN OTHERS THEN
                    NULL;
                    /*
                    IF v_exception_msg IS NULL THEN
                      v_exception_msg := substr('FAILURE: '||SQLERRM,1,256);
                    ELSE
                      v_exception_msg := substr('FAILURE: '||v_exception_msg||' | '||SQLERRM,1,256);
                    END IF;
                    RAISE RejectRecord;*/
                END;
              END IF; -- match on account key

              -- AEO-1885 begin
              IF NOT v_found AND c.loyaltynumber IS NOT NULL THEN
                -- look up based on loyalty number only
                BEGIN
                  SELECT DISTINCT vc.vckey
                                , vc.ipcode
                                , md.a_extendedplaycode
                  INTO     v_vckey
                         , v_ipcode
                         , v_ExtendedPlayCode
                  FROM   bp_ae.lw_virtualcard          vc
                       , bp_ae.ats_memberdetails       md
                  WHERE  vc.loyaltyidnumber = c.LoyaltyNumber AND
                         --vc.isprimary = 1 AND --AEO-2034
                         md.a_ipcode = vc.ipcode;

                   v_found := TRUE;

                   IF v_exception_msg IS NOT NULL THEN
                     v_exception_msg := v_exception_msg||' | Matched using Loyalty Number';
                   ELSE
                     v_exception_msg := 'Matched using Loyalty Number';
                   END IF;

                EXCEPTION
                  WHEN no_data_found THEN
                    IF v_exception_msg IS NULL THEN
                      v_exception_msg := 'Loyalty Number match failed';
                    ELSE
                      v_exception_msg := v_exception_msg||' | Loyalty Number match failed';
                    END IF;
                  WHEN too_many_rows THEN
                    IF v_exception_msg IS NULL THEN
                      v_exception_msg := 'Loyalty ID returned too many rows';
                    ELSE
                      v_exception_msg := v_exception_msg || ' | Loyalty Number returned too many rows';
                    END IF;
                    --RAISE RejectRecord;
                  WHEN OTHERS THEN
                    NULL;
                    /*
                    -- non specified exception so get exception message from oracle
                    v_exception_msg := substr('FAILURE: '||v_exception_msg||' '||SQLERRM,1,256);
                    RAISE RejectRecord;*/
                END;
              END IF; -- match on loyalty number only
              -- AEO-1885 end

              IF NOT v_found THEN
                -- error message should be populated
                v_exception_msg := 'FAILURE: '||v_exception_msg;
                RAISE RejectRecord;
                -- AEO-1885 begin
              ELSE
                 IF v_exception_msg IS NOT NULL THEN
                    v_exception_msg := 'WARNING: '||v_exception_msg;
                 END IF;


                 /*
                INSERT INTO bp_ae.err_tlog08_worldspend
                   (     loyaltynumber
                       , accountkey
                       , startdate
                       , enddate
                       , networldsales
                       , exceptionmsg
                       , filename
                   )
                VALUES(  c.LoyaltyNumber
                       , c.AccountKey
                       , c.StartDate
                       , c.EndDate
                       , c.NetWorldSales
                       , v_exception_msg
                       , substr(p_Filename,1,256));
                 */
                 -- AEO-1885 end
              END IF;

              -- AEO-1885 begin
              /*
              IF bp_ae.ae_isinpilot(pstExtPlayCode => v_ExtendedPlayCode) = 0 THEN
                -- non member
                IF v_exception_msg IS NULL THEN
                  v_exception_msg := 'FAILURE: Member is not in pilot';
                ELSE
                  v_exception_msg := 'FAILURE: '||v_exception_msg || ' | Member is not in pilot';
                END IF;
                RAISE RejectRecord;
              END IF;*/
               -- AEO-1885 end

              -- Match found so can proceed
              v_RowKey := seq_rowkey.nextval;
              --AEP-523 change txndetailid to a serialized number rather than 0. -----------JHC
              v_detailid := seq_rowkey.nextval;
              v_NetWorldSales    := Round(TO_BINARY_FLOAT(c.networldsales),0);
              v_StartDate        := to_date(c.startdate, 'mmddyyyy');
              v_EndDate          := to_date(c.enddate, 'mmddyyyy');
              v_TxnNumber        := to_char(v_RowKey);
              v_StoreNumber      := to_char(c.accountkey);
              v_Reg_Nbr          := '999';
              v_Txn_Date         := v_StartDate;
              -- action and header id are outbound parameters of the Get_Headerid procedure
              Get_Headerid(p_Store_Nbr => v_StoreNumber --v_00_rec.STORE_NUMBER --  p_head_rec.altstorenumber
                          ,p_Reg_Nbr   => v_Reg_Nbr --p_head_rec.alttxnregisternumber
                          ,p_Txn_Date  => v_StartDate --p_head_rec.alttxndate
                          ,p_Txn_Nbr   => v_TxnNumber --p_head_rec.alttxnnumber
                          ,p_Processid => v_Process_Id
                          ,p_Headerid  => v_HeaderID
                          ,p_Rec_Type  => v_RecType --v_txt1
                          ,p_Action    => v_Action --v_txt2
                           );

              v_Process_Id := 0;

              insert into lw_txndetail_stage
                  (  rowkey
                   , ipcode
                   , vckey
                   , dtlquantity
                   , dtldiscountamount
                   , dtlclearanceitem
                   , dtldatemodified
                   , reconcilestatus
                   , txnheaderid
                   , txndetailid
                   , brandid
                   , fileid
                   , processid
                   , filelineitem
                   , cardid
                   , creditcardid
                   , txnloyaltyid
                   , txnmaskid
                   , txnnumber
                   , txndate
                   , txndatemodified
                   , txnregisternumber
                   , txnstoreid
                   , txntypeid
                   , txnamount
                   , txndiscountamount
                   , txnqualpurchaseamt
                   , txnemailaddress
                   , txnphonenumber
                   , txnemployeeid
                   , txnchannelid
                   , txnoriginaltxnrowkey
                   , txncreditsused
                   , dtlitemlinenbr
                   , dtlproductid
                   , dtltypeid
                   , dtlactionid
                   , dtlretailamount
                   , dtlsaleamount
                   , dtlclasscode
                   , errormessage
                   , shipdate
                   , ordernumber
                   , skunumber
                   , tenderamount
                   , storenumber
                   , statuscode
                   , createdate
                   , updatedate
                   , nonmember
                   , txnoriginalstoreid
                   , txnoriginaltxndate
                   , txnoriginaltxnnumber
                )
                values
                  (  v_RowKey
                   , v_ipcode
                   , v_vckey
                   , 0
                   , 0
                   , 0
                   , NULL
                   , 0
                   , v_HeaderID
                   , v_detailid
                   , 0
                   , v_My_Log_Id
                   , v_Process_Id
                   , c.filelineitem
                   , 0
                   , 0
                   , c.loyaltynumber
                   , c.accountkey
                   , v_TxnNumber
                   , v_StartDate
                   , NULL
                   , v_Reg_Nbr
                   , 0
                   , 6
                   , v_NetWorldSales
                   , 0
                   , v_NetWorldSales
                   , NULL
                   , NULL
                   , NULL
                   , 0
                   , 0
                   , NULL
                   , 0
                   , 0
                   , 0
                   , 0
                   , 0
                   , 0
                   , NULL
                   , NULL
                   , v_EndDate
                   , NULL
                   , NULL
                   , 0
                   , 0
                   , 1
                   , SYSDATE
                   , SYSDATE
                   , 0
                   , 0
                   , NULL
                   , 0
                );

              v_Messagespassed := v_Messagespassed + 1;

              -- Even though the record was matched it may have had
              -- issues along the way to log the error and adjust the error count
              IF v_exception_msg IS NOT NULL THEN
                v_Messagesfailed := v_Messagesfailed - 1;
              --  v_exception_msg := 'WARNING: '||v_exception_msg;
                RAISE RejectRecord;
              END IF;
              
              -- AEO-2352 begin
              IF ( v_found ) THEN
                UPDATE bp_ae.lw_loyaltymember lm
                     SET lm.lastactivitydate =  
                                                 CASE WHEN lm.lastactivitydate IS NULL THEN v_Txn_Date
                                                   WHEN lm.lastactivitydate < v_Txn_Date THEN v_txn_date
                                                   WHEN lm.lastactivitydate >= v_txn_date THEN lm.lastactivitydate
                                               END
                 WHERE lm.ipcode = v_IpCode;
              END IF;
			  commit;
              -- AEO-2352 end
          EXCEPTION
            WHEN OTHERS THEN
              -- AEO-1885 begin
              /*
              IF v_exception_msg IS NULL THEN
                v_exception_msg := substr('FAILURE: '||SQLERRM,1,256);
              ELSE
                IF SQLCODE <> 1 THEN
                  -- Only update the exception message is it is an Oracle error
                  v_exception_msg := substr('FAILURE: '||v_exception_msg || ' | '||SQLERRM,1,256);
                END IF;
              END IF;*/
               -- AEO-1885 end
              v_Messagesfailed := v_Messagesfailed + 1;

              INSERT INTO bp_ae.err_tlog08_worldspend
                 (     loyaltynumber
                     , accountkey
                     , startdate
                     , enddate
                     , networldsales
                     , exceptionmsg
                     , filename
                 )
              VALUES(  c.LoyaltyNumber
                     , c.AccountKey
                     , c.StartDate
                     , c.EndDate
                     , c.NetWorldSales
                     , v_exception_msg
                     , substr(p_Filename,1,256));
          END;

        /* skips go here */
        <<skip_Point>>
        FETCH Cur_Raw INTO c;

      END LOOP;

      <<the_End>>

      CLOSE Cur_Raw;
    END;
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
                     '    <pkg>Stage_Tlog</pkg>' || Chr(10) ||
                     '    <proc>Stage_Tlog01</proc>' || Chr(10) ||
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
                        p_Jobname          => v_Jobname);

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

    /* after returns is done then execute the job that will update all the member attributes that come from the tlog */
  --  UpdateMemberAttributesFromTlog(); --PI29741 tlog optimization changes procedure is called from stage_txncopy pkg


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
                          p_Jobname          => v_Jobname);
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure Stage_Tlog01';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>Stage_Tlog</pkg>' || Chr(10) ||
                          '    <proc>Stage_Tlog01</proc>' || Chr(10) ||
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
  END Process_Tlog;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to write data to hist ********/
  PROCEDURE Hist_Tlog01(p_Dummy VARCHAR2, p_Process_Date Date,
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
    v_Jobname          VARCHAR2(256) := 'Hist_TLog01';
    --log msg attributes
    v_Messageid  NUMBER;
    v_Envkey     VARCHAR2(256) := 'BP_AE@' ||
                                  Upper(Sys_Context('userenv',
                                                    'instance_name'));
    v_Logsource  VARCHAR2(256) := 'Hist_TLog01';
    v_Batchid    VARCHAR2(256) := 0;
    v_Message    VARCHAR2(256);
    v_Reason     VARCHAR2(256);
    v_Error      VARCHAR2(256);
    v_Trycount   NUMBER := 0;
    v_Process_Id NUMBER := 0;
    v_Skip       NUMBER := 0;
    v_ProcessDate Date;
    v_BypassDateCheck NUMBER := 0;

    CURSOR get_reprocessed IS SELECT rowkey
                              from Lw_Txndetail_Stage t
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
        where  lj.jobname = 'Tlog8_WorldSpend'
        AND    trunc(lj.starttime) = trunc(v_ProcessDate)
        AND    to_char(lj.starttime, 'mm/dd/yyyy hh:mm:ss') = (select to_char(max(starttime), 'mm/dd/yyyy hh:mm:ss') from lw_libjob where  jobname = 'Tlog8_WorldSpend');
       END IF;

    EXCEPTION
      WHEN No_Data_Found THEN
                      v_Messagesfailed := v_Messagesfailed + 1;
                      v_Error          := SQLERRM;
                      v_Reason         := 'DAP Tlog job is not finished';
                      v_Message        := 'DAP Tlog job is not finished';

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

      Raise_Application_Error(-20001, 'DAP Tlog job is not finished');

    END;



    IF V_EndDate is null
    THEN
                      v_Messagesfailed := v_Messagesfailed + 1;
                      v_Error          := SQLERRM;
                      v_Reason         := 'DAP Tlog job is not finished';
                      v_Message        := 'DAP Tlog job is not finished';

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

      Raise_Application_Error(-20001, 'DAP Tlog job is not finished');
    END IF;



    /* this loop will take care of any re-processing that occured from the log_unprocessed tables */
    OPEN get_reprocessed;
    LOOP <<get_reprocessed_loop>>
      FETCH get_reprocessed BULK COLLECT INTO v_tbl LIMIT 500;
         FORALL i IN 1..v_tbl.count
             UPDATE Ats_Historytxndetail
             SET a_processid = Gv_Status_Processed
             WHERE a_rowkey = v_tbl(i).rowkey;

         COMMIT;
      EXIT WHEN get_reprocessed%NOTFOUND;
    END LOOP get_reprocessed_loop;


    Log_Process_State('Log_Txn_Keys');
    INSERT INTO Log_Txn_Keys
      (Txnheaderid,
       Store_Nbr,
       Register_Nbr,
       Txn_Date,
       Txn_Nbr,
       First_Rec_Type,
       Processid)
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
             END AS Processid
        FROM Log_Txn_Keys_Stage t
       WHERE NOT EXISTS (SELECT 1
                FROM Log_Txn_Keys x
               WHERE x.Txnheaderid = t.Txnheaderid);
    COMMIT;

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
             t.Createdate,
             t.Updatedate,
             t.Lastdmlid
        FROM Lw_Txndetail_Stage t
       WHERE 1=1
       AND t.rowkey in
           (
           SELECT s.rowkey
           From lw_txndetail_stage s minus Select x.a_rowkey FROM Ats_Historytxndetail x Where x.a_txndate > trunc(sysdate, 'Q')
           );
 TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl1 t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl1 LIMIT 10000; --<-----  here we say collect 5,000 rows at a time.
        -- Then update address in ATS_MemberDetails table and commit
        FORALL i IN 1 .. v_tbl1.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop

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
       Createdate,
       Updatedate,
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
             v_tbl1(i).Createdate,
             v_tbl1(i).Updatedate,
             v_tbl1(i).Lastdmlid );
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
      END;

    -- Tlog Txn Processed Count
    select count(distinct txnheaderid) into v_Messagesreceived
    from LW_TXNDETAIL_STAGE
    where ipcode > 0 and ProcessId = 0 and NonMember = 0;

    --Tlog Txn Success Count
    select count(distinct a_txnheaderid) into v_Messagespassed
    from ats_txnheader where a_txnheaderid in
    (select txnheaderid from lw_txndetail_stage where ipcode > 0 and ProcessId = 0 and NonMember = 0);

    -- Tlog Txn ErrorCouunt = ProcessCount ? SuccessCount
    v_Messagesfailed := v_Messagesreceived - v_Messagespassed;


    EXECUTE IMMEDIATE 'truncate table LW_TXNDETAIL_STAGE';

    Log_Process_State('LOG_TXN_KEYS');

    EXECUTE IMMEDIATE 'truncate table LOG_TXN_KEYS_STAGE';

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
    EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk' ; -- clear work table before next insert
    EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_Wrk' ; -- clear work table before next insert

  END Hist_Tlog01;
END Stage_Tlog8_WorldSpend;
/
