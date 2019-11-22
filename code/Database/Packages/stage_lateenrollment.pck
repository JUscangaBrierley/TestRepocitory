CREATE OR REPLACE PACKAGE Stage_LateEnrollment IS
type rcursor IS REF CURSOR;

    PROCEDURE LateEnrollment_Staging (retval IN OUT rcursor);
END Stage_LateEnrollment;
/
CREATE OR REPLACE PACKAGE BODY Stage_LateEnrollment   IS

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
PROCEDURE LateEnrollment_Staging (retval          IN OUT rcursor)
IS
 /*   version 1.0
      CreatedBy RKG
      CreatedOn 09/25/2014
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
  v_Errormessage          VARCHAR2(256);
  v_RowKey                VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256) ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  v_recordcount        NUMBER :=0;
  v_attachments cio_mail.attachment_tbl_type;

  BEGIN
   /* get job id for this process and the dap process */
   v_my_log_id := utility_pkg.get_LIBJobID();
   v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'LateEnrollment_Staging';
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
utility_pkg.Log_Process_Start(v_jobname, 'Staging TxnDetail', v_processId);

----------------------------------------------------
--Insert TxnDetail records into Staging table
----------------------------------------------------
DECLARE
      CURSOR get_data IS
        select hd.a_rowkey as HistoryRowKey
       , vc.ipcode, vc.vckey, hd.a_dtlquantity, hd.a_dtldiscountamount, hd.a_dtlclearanceitem, hd.a_dtldatemodified, hd.a_reconcilestatus, hd.a_txnheaderid, hd.a_txndetailid, hd.a_brandid
       , hd.a_fileid, hd.a_processid, hd.a_filelineitem, hd.a_cardid, hd.a_creditcardid, hd.a_txnloyaltyid, hd.a_txnmaskid, hd.a_txnnumber, hd.a_txndate, hd.a_txndatemodified
       , hd.a_txnregisternumber, hd.a_txnstoreid, hd.a_txntype, hd.a_txnamount, hd.a_txndiscountamount, hd.a_txnqualpurchaseamt, hd.a_txnemailaddress, hd.a_txnphonenumber, hd.a_txnemployeeid
       , hd.a_txnchannelid, hd.a_txnoriginaltxnrowkey, hd.a_txncreditsused, hd.a_dtlitemlinenbr, hd.a_dtlproductid, hd.a_dtltypeid, hd.a_dtlactionid, hd.a_dtlretailamount, hd.a_dtlsaleamount
       , hd.a_dtlclasscode, hd.a_errormessage, hd.a_shipdate, hd.a_ordernumber, hd.a_skunumber, hd.a_tenderamount, hd.a_storenumber, hd.statuscode, hd.createdate, hd.updatedate
       , hd.lastdmlid, 0 as nonmember, 0 as txnoriginalstoreid , '' as txnoriginaltxndate, '' as txnoriginaltxnnumber
       --AEO-846 BEGIN
       , hd.a_currencyrate, hd.a_currencycode, hd.a_dtlsaleamount_org
       --AEO-846 END
            from   ats_historytxndetail hd
            inner join lw_virtualcard vc on hd.a_txnloyaltyid = vc.loyaltyidnumber
            inner join lw_loyaltymember lm on vc.ipcode = lm.ipcode
            where (hd.a_processid = 2 OR hd.a_dtlclasscode = '9911') -- AEO-832
            and lm.membercreatedate >=
                (
                select trunc(to_date(to_char(value), 'mm/dd/yyyy hh24:mi:ss'))
                from lw_clientconfiguration cc
                where cc.key = 'LateEnrollStartDate'
                )
            and lm.memberstatus = 1
            and hd.a_txnDate >=  trunc(sysdate,'q') - 10;

      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          LOOP
          v_RowKey := v_tbl(i).HistoryRowKey;

          insert into lw_txndetail_stage (
          rowkey, ipcode, vckey, dtlquantity, dtldiscountamount, dtlclearanceitem, dtldatemodified
         , reconcilestatus, txnheaderid, txndetailid, brandid, fileid, processid, filelineitem
         , cardid, creditcardid, txnloyaltyid, txnmaskid, txnnumber, txndate, txndatemodified
         , txnregisternumber, txnstoreid, txntypeid, txnamount, txndiscountamount, txnqualpurchaseamt
         , txnemailaddress, txnphonenumber, txnemployeeid, txnchannelid, txnoriginaltxnrowkey, txncreditsused, dtlitemlinenbr, dtlproductid, dtltypeid, dtlactionid
         , dtlretailamount, dtlsaleamount, dtlclasscode, errormessage, shipdate, ordernumber, skunumber, tenderamount, storenumber, statuscode, createdate, updatedate, lastdmlid, nonmember
         , txnoriginalstoreid, txnoriginaltxndate, txnoriginaltxnnumber
         --AEO-846 BEGIN
         , currencyrate,currencycode,dtlsaleamount_org
         --AEO-846 END
         )         
         Values
         (
         v_tbl(i).HistoryRowKey, v_tbl(i).ipcode, v_tbl(i).vckey, v_tbl(i).a_dtlquantity, v_tbl(i).a_dtldiscountamount, v_tbl(i).a_dtlclearanceitem, v_tbl(i).a_dtldatemodified
         , v_tbl(i).a_reconcilestatus, v_tbl(i).a_txnheaderid, v_tbl(i).a_txndetailid, v_tbl(i).a_brandid, v_tbl(i).a_fileid, 0, v_tbl(i).a_filelineitem
         , v_tbl(i).a_cardid, v_tbl(i).a_creditcardid, v_tbl(i).a_txnloyaltyid, v_tbl(i).a_txnmaskid, v_tbl(i).a_txnnumber, v_tbl(i).a_txndate, v_tbl(i).a_txndatemodified
         , v_tbl(i).a_txnregisternumber, v_tbl(i).a_txnstoreid, v_tbl(i).a_txntype, v_tbl(i).a_txnamount, v_tbl(i).a_txndiscountamount, v_tbl(i).a_txnqualpurchaseamt
         , v_tbl(i).a_txnemailaddress, v_tbl(i).a_txnphonenumber, v_tbl(i).a_txnemployeeid, v_tbl(i).a_txnchannelid, v_tbl(i).a_txnoriginaltxnrowkey, v_tbl(i).a_txncreditsused
         , v_tbl(i).a_dtlitemlinenbr, v_tbl(i).a_dtlproductid, v_tbl(i).a_dtltypeid, v_tbl(i).a_dtlactionid, v_tbl(i).a_dtlretailamount, v_tbl(i).a_dtlsaleamount
         , v_tbl(i).a_dtlclasscode, v_Errormessage, v_tbl(i).a_shipdate, v_tbl(i).a_ordernumber, v_tbl(i).a_skunumber, v_tbl(i).a_tenderamount, v_tbl(i).a_storenumber
         , v_tbl(i).statuscode, v_tbl(i).createdate, v_tbl(i).updatedate, v_tbl(i).lastdmlid, v_tbl(i).nonmember, v_tbl(i).txnoriginalstoreid, v_tbl(i).txnoriginaltxndate
         , v_tbl(i).txnoriginaltxnnumber
         --AEO-846 BEGIN
         ,v_tbl(i).a_currencyrate, v_tbl(i).a_currencycode, v_tbl(i).a_dtlsaleamount_org
         --AEO-846 END
         )
          LOG ERRORS INTO err$_lw_txndetail_stage('UPDATE') REJECT LIMIT UNLIMITED;

         Select count(*) into v_trycount From err$_lw_txndetail_stage where rowkey = v_RowKey;

        v_messagesreceived := v_messagesreceived + 1;
        v_recordcount := v_recordcount + 1;

         If(v_trycount > 0) THEN
            v_messagesfailed := v_messagesfailed + 1;
          END IF;

    -- Now go back and update the processId as Late Enrollment Processed
         Update ats_historytxndetail hst
         Set hst.a_processid = 9,
             hst.a_ipcode = v_tbl(i).ipcode, -- AEO-794
             hst.a_vckey = v_tbl(i).vckey    -- AEO-794
         Where hst.a_rowkey = v_tbl(i).HistoryRowKey;
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

----------------------------------------------------
--Check TxnDetail records for > 3 Txns in 24 hour period
----------------------------------------------------
    DECLARE
      CURSOR get_data IS
        select DISTINCT ipcode, txnheaderid, txndate, txnqualpurchaseamt
            from   lw_txndetail_stage hd;

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
    utility_pkg.Log_Process_Start(v_jobname, 'Staging TxnDetailDiscount', v_processId);

    insert into lw_txndetaildiscount_stage
           (
           rowkey, ipcode , vckey, processid, txndiscountid, txnheaderid, txndate, txndetailid, discounttype, discountamount, txnchannel, offercode, errormessage, fileid
           , statuscode, createdate, updatedate, lastdmlid
           )
            -- AEO-794 begin
            -- ipcode and vckey are populated from lw_virtualcard
    select a_rowkey, stg.ipcode, stg.vckey, a_processid, a_txndiscountid, a_txnheaderid, a_txndate, a_txndetailid, a_discounttype, a_discountamount, a_txnchannel, a_offercode
           , a_errormessage, a_fileid, dsc.statuscode, dsc.createdate, dsc.updatedate, dsc.lastdmlid
    from ats_historytxndetaildiscount dsc
            -- Inner joins needed to extract data from vitualcard
    INNER JOIN lw_txndetail_stage stg  ON stg.txnheaderid = dsc.a_txnheaderid  AND stg.txndetailid = dsc.a_txndetailid;
    --INNER JOIN lw_virtualcard vc ON vc.vckey = stg.vckey;
            -- where dsc.a_txnheaderid in (select stg.txnheaderid from lw_txndetail_stage stg)  ;
    v_messagesreceived := sql%rowcount;
            -- AEO-794 End
    commit;

-- AEO-794 begin
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
             Set hst.a_processid = 9
                 ,hst.a_ipcode = v_tbl(i).ipcode,
                 hst.a_vckey = v_tbl(i).vckey
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

  -- AEO-794 end
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

----------------------------------------------------
--Insert TxnRewardRedeem records into Staging table
----------------------------------------------------
    Execute Immediate 'Truncate Table lw_txnrewardredeem_stage';
    utility_pkg.Log_Process_Start(v_jobname, 'Staging TxnRewardRedeem', v_processId);

    insert into lw_txnrewardredeem_stage
           (
                rowkey, ipcode, vckey, txnheaderid, txndate, txndetailid, programid,
                certificateredeemtype, certificatecode, certificatediscountamount,
                txnrewardredeemid, processid, fileid, statuscode, createdate, updatedate, lastdmlid
           )
           -- AEO-794 begin
     -- AEO-794 begin
           -- ipcode and vckey are populated from lw_virtualcard
    select a_rowkey, stg.ipcode, stg.vckey, a_txnheaderid, a_txndate, a_txndetailid, a_programid, a_certificateredeemtype, a_certificatecode, a_certificatediscountamount
           , a_txnrewardredeemid, a_processid, a_fileid, rwd.statuscode, rwd.createdate, rwd.updatedate, rwd.lastdmlid
    from ats_historytxnrewardredeem rwd
       -- Inner joins needed to extract data from vitualcard
    INNER JOIN lw_txndetail_stage stg  ON stg.txnheaderid = rwd.a_txnheaderid AND stg.txndetailid = rwd.a_txndetailid;
  -- INNER JOIN lw_virtualcard      vc  ON vc.vckey = stg.vckey;
   -- where dsc.a_txnheaderid in (s
       -- AEO-794 end
    v_messagesreceived := sql%rowcount;
    commit;

   -- AEO-794 begin
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
               Set hst.a_processid = 9
                   ,hst.a_ipcode =  v_tbl(i).ipcode
                   ,hst.a_vckey = v_tbl(i).vckey
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



    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

----------------------------------------------------
--Insert TxnTender records into Staging table
----------------------------------------------------
    Execute Immediate 'Truncate Table lw_txntender_stage';
    utility_pkg.Log_Process_Start(v_jobname, 'Staging TxnTender', v_processId);


     -- AEO-794 begin
    insert into lw_txntender_stage
           (
           rowkey        , ipcode     , vckey     , processid   , storeid       , txndate   ,
           txnheaderid   , txntenderid, tendertype, tenderamount, tendercurrency, tendertax ,
           tendertaxrate , errormessage, fileid   , statuscode  , createdate    , updatedate, lastdmlid
           )
           -- ipcode and vckey are populated from lw_virtualcard
    SELECT DISTINCT( a_rowkey)     ,stg.ipcode      , stg.vckey      , a_processid   , a_storeid       , a_txndate,
           a_txnheaderid,a_txntenderid  , a_tendertype  , a_tenderamount, a_tendercurrency,
           a_tendertax  ,a_tendertaxrate, a_errormessage, a_fileid      , tnd.statuscode      ,
           tnd.createdate   , tnd.updatedate    , tnd.lastdmlid
    from ats_historytxntender tnd
           -- Inner joins needed to extract data from vitualcard
          INNER JOIN  lw_txndetail_stage stg ON  stg.txnheaderid = tnd.a_txnheaderid ;
    -- AEO-794 end
          v_messagesreceived := sql%rowcount;
    commit;

      -- AEO-794 begin
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
               Set hst.a_processid = 9
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
 -- AEO-794 end
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    utility_pkg.Log_Process_Job_End(v_processId, v_messagesreceived);

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

    --Send the job success email
      cio_mail.send(
          p_from_email    => 'AEREWARDS',
          p_from_replyto  => 'info@aerewards.ae.com',
          p_to_list       => 'aerewards_support@brierley.com',
          p_cc_list       => NULL,
          p_bcc_list      => NULL,
          p_subject       => 'AE Late Enrollment Job Success',
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
          p_subject       => 'AE Late Enrollment Job Failed',
          p_text_message  => 'Job Number #'||v_my_log_id||' has failed. See lw_libmessagelog table for error',
          p_content_type  => 'text/html;charset=UTF8',
          p_attachments   => v_attachments, --v_attachments,
          p_priority      => '3',
          p_auth_username => NULL,
          p_auth_password => NULL,
          p_mail_server   => 'cypwebmail.brierleyweb.com');
  RAISE;

  END LateEnrollment_Staging;

  END Stage_LateEnrollment;
/
