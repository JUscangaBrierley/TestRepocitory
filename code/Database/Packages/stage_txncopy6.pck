CREATE OR REPLACE PACKAGE STAGE_TXNCOPY6 IS
type rcursor IS REF CURSOR;
  /*
PI29741 tlog optimization changes begin here                 -----scj
*/
      PROCEDURE StageToTxnHeadWrk6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnDetailWrk6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnHeader6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnDetail6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnRedeem6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnDetailDiscount6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnTender6(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE StageToTxnTables6(p_Processdate varchar2,p_email_list VARCHAR2);
      PROCEDURE TlogInterceptorProcessing6(p_Dummy VARCHAR2,retval IN OUT rcursor);
      PROCEDURE UpdateMemberAttributeFromTlog6(p_Processdate varchar2,retval IN OUT rcursor);
      PROCEDURE Giftcard24HrPurchase(p_Dummy VARCHAR2,retval IN OUT rcursor);
      PROCEDURE BasicReturns24HrAdjust(p_Dummy VARCHAR2,retval IN OUT rcursor);
      PROCEDURE TxnHeaderWrk6Bkp(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      PROCEDURE TxnDetailWrk6Bkp(p_Dummy VARCHAR2,p_email_list VARCHAR2,retval IN OUT rcursor);
      /*Redesign Changes start here --------------------------------------------------------------SCJ */
      PROCEDURE ReturnRedeemTxnPoints(p_Processdate varchar2,p_email_list VARCHAR2,retval IN OUT rcursor) ;
      /*Redesign Changes end here --------------------------------------------------------------SCJ */

END STAGE_TXNCOPY6;
/
CREATE OR REPLACE PACKAGE BODY STAGE_TXNCOPY6   IS

PROCEDURE StageToTxnHeadWrk6(p_Dummy      VARCHAR2,
                             p_email_list VARCHAR2,
                             retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  v_headerwkcount NUMBER := 0;
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN

  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TxnHeadWrk_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnHeader_Wrk6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (select CASE
                 WHEN MAX(STG.BRANDID) in ('-1', '3') THEN
                  unistr('\0031') -- if store not found or if it is 77 kids brandId, then default to '1' AE brandid
                 ELSE
                  MAX(STG.BRANDID)
               END AS BRANDID,
               max(stg.rowkey) as rowkey,
               stg.vckey,
               stg.shipdate,
               stg.ordernumber,
               max(stg.txnqualpurchaseamt) as txnqualpurchaseamt,
               stg.txnheaderid,
               stg.creditcardid,
               stg.txnmaskid,
               stg.txnnumber,
               stg.txndate,
               stg.txnstoreid,
               stg.txntypeid,
               stg.txnamount,
               stg.txndiscountamount,
               stg.storenumber,
               stg.txnemployeeid,
               stg.txnchannelid,
               stg.txnoriginaltxnrowkey,
               stg.txncreditsused,
               stg.txnregisternumber,
               stg.txnoriginalstoreid,
               stg.txnoriginaltxndate,
               stg.txnoriginaltxnnumber,
               stg.statuscode,
                /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
               stg.aeccmultiplier,
               stg.originalordernumber,
                /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
               stg.cashiernumber,  --AEO-1533 LPP
               ---AEO-846 BEGIN
               stg.currencycode,
               stg.currencyrate
               --AEO-846 END
          from LW_TXNDETAIL_STAGE6 stg
         where stg.ipcode > 0
           and stg.processid = 0
           and not exists
         (select 1 -- Checking for duplicate txn header records
                  from ats_txnheader ht
                 where ht.a_txnheaderid = stg.txnheaderid)
         group by stg.txnheaderid,
                  stg.vckey,
                  stg.shipdate,
                  stg.ordernumber,
                  stg.txnheaderid,
                  stg.creditcardid,
                  stg.txnmaskid,
                  stg.txnnumber,
                  stg.txndate,
                  stg.txnstoreid,
                  stg.txntypeid,
                  stg.txnamount,
                  stg.txndiscountamount,
                  stg.storenumber,
                  stg.txnemployeeid,
                  stg.txnchannelid,
                  stg.txnoriginaltxnrowkey,
                  stg.txncreditsused,
                  stg.txnregisternumber,
                  stg.txnoriginalstoreid,
                  stg.txnoriginaltxndate,
                  stg.txnoriginaltxnnumber,
                  stg.statuscode,
                   /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                   stg.aeccmultiplier,
                   stg.originalordernumber,
                    /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
                  stg.cashiernumber,  --AEO-1533 LPP
                  --AEO-846 BEGIN
                  stg.currencycode,
                  stg.currencyrate
                  --AEO-846 END
                  )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    --  EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk6' ; -- clear work table before next insert
    --clear work table only if there is data , since the tables are truncated after Post process(Hist_Tlog01)
    SELECT count(hw.header_rowkey)
      INTO v_headerwkcount -- making sure, if there is a record, to delete it.
      FROM LW_txnheader_Wrk6 hw;
    If v_headerwkcount > 0 then
      Delete from LW_txnheader_Wrk6; -- not using truncate because this causes ORA-08103 in the header rules processing
      commit;
    End if;
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into LW_txnheader_Wrk6
            (rowkey,
             header_rowkey,
             vckey,
             parentrowkey,
             shipdate,
             ordernumber,
             txnqualpurchaseamt,
             txnheaderid,
             brandid,
             creditcardid,
             txnmaskid,
             txnnumber,
             txndate,
             txnstoreid,
             txntypeid,
             txnamount,
             txndiscountamount,
             storenumber,
             txnemployeeid,
             txnchannel,
             txnoriginaltxnrowkey,
             txncreditsused,
             txnregisternumber,
             txnoriginalstoreid,
             txnoriginaltxndate,
             txnoriginaltxnnumber,
             statuscode,
             --AEO-846 BEGIN
             currencycode,
             currencyrate,
             --AEO-846 END
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             aeccmultiplier,
             originalordernumber,
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             cashiernumber,   --AEO-1533
             createdate,
             updatedate)
          VALUES
            (v_tbl(j).rowkey,
             seq_rowkey.nextval,
             v_tbl(j).vckey,
             v_tbl(j).vckey,
             v_tbl(j).shipdate,
             v_tbl(j).ordernumber,
             NVL(v_tbl(j).txnqualpurchaseamt, 0),
             v_tbl(j).txnheaderid,
             v_tbl(j).brandid,
             v_tbl(j).creditcardid,
             v_tbl(j).txnmaskid,
             v_tbl(j).txnnumber,
             v_tbl(j).txndate,
             v_tbl(j).txnstoreid,
             v_tbl(j).txntypeid,
             NVL(v_tbl(j).txnamount, 0),
             NVL(v_tbl(j).txndiscountamount, 0),
             v_tbl(j).storenumber,
             v_tbl(j).txnemployeeid,
             v_tbl(j).txnchannelid,
             v_tbl(j).txnoriginaltxnrowkey,
             v_tbl(j).txncreditsused,
             v_tbl(j).txnregisternumber,
             v_tbl(j).txnoriginalstoreid,
             v_tbl(j).txnoriginaltxndate,
             v_tbl(j).txnoriginaltxnnumber,
             NVL(v_tbl(j).statuscode, 0),
             --AEO-846 BEGIN
             v_tbl(j).currencycode,
             v_tbl(j).currencyrate,
             --AEO-846 END
              /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl(j).aeccmultiplier,
             v_tbl(j).originalordernumber,
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl(j).cashiernumber,   --AEO-1533
             sysdate,
             sysdate);
      EXCEPTION
        WHEN dml_errors THEN
          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnHeadWrk6: ';
            v_Message        := 'vckey: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .vckey || 'txnheaderid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txnheaderid;

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

          END LOOP;
          -- IF v_messagesfailed = 0 THEN
          --   v_messagesfailed := 0+1;
          -- END IF;
          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnHeadWrk6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnHeadWrk6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --           IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');

            v_email_flag := v_email_flag + 1;
          end if;
      END;
      -- v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --  COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --  COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;

  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnHeadWrk6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');

  END;
END StageToTxnHeadWrk6;


PROCEDURE StageToTxnDetailWrk6(p_Dummy      VARCHAR2,
                               p_email_list VARCHAR2,
                               retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  v_detailitemwkcount NUMBER := 0;
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TxnDetailWrk_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnDetail_Wrk6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (select *
          from LW_TXNDETAIL_STAGE6 stg
         where stg.ipcode > 0
           and stg.processid = 0
           and not exists
         (select 1 -- Checking for duplicate txn detailitem records
                  from ats_txndetailitem dt
                 where dt.a_txnheaderid = stg.txnheaderid))
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    -- EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_Wrk6' ; -- clear work table before next insert
    --clear work table only if there is data , since the tables are truncated after Post process(Hist_Tlog01)
    SELECT count(dw.dtl_rowkey)
      INTO v_detailitemwkcount -- making sure, if there is a record, to delete it.
      FROM LW_txndetailitem_Wrk6 dw;
    If v_detailitemwkcount > 0 then
      Delete from LW_txndetailitem_Wrk6; -- not using truncate because this causes ORA-08103 in the header rules processing
      commit;
    End if;
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;

      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;

      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into LW_txndetailitem_Wrk6
            (rowkey,
             dtl_rowkey,
             ipcode,
             parentrowkey,
             txnheaderid,
             txndate,
             txnstoreid,
             txndetailid,
             dtlitemlinenbr,
             dtlproductid,
             dtltypeid,
             dtlactionid,
             dtlretailamount,
             dtlsaleamount,
             dtlquantity,
             dtldiscountamount,
             dtlclearanceitem,
             dtlclasscode,
             statuscode,
             dtlsaleamount_org, --AEO-846
             createdate,
             updatedate)
          VALUES
            (v_tbl(j).rowkey,
             seq_rowkey.nextval,
             v_tbl(j).VCKEY,
             v_tbl(j).rowkey,
             v_tbl(j).txnheaderid,
             v_tbl(j).txndate,
             v_tbl(j).txnstoreid,
             v_tbl(j).txndetailid,
             v_tbl(j).dtlitemlinenbr,
             v_tbl(j).dtlproductid,
             v_tbl(j).dtltypeid,
             v_tbl(j).dtlactionid,
             NVL(v_tbl(j).dtlretailamount, 0),
             NVL(v_tbl(j).dtlsaleamount, 0),
             v_tbl(j).dtlquantity,
             NVL(v_tbl(j).dtldiscountamount, 0),
             v_tbl(j).dtlclearanceitem,
             v_tbl(j).dtlclasscode,
             NVL(v_tbl(j).statuscode, 0),
             v_tbl(j).dtlsaleamount_org, --AEO-846
             sysdate,
             sysdate);
      EXCEPTION
        WHEN dml_errors THEN
          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnDetailWrk6: ';
            v_Message        := 'ipcode: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .ipcode || 'txndetailid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txndetailid;

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

          END LOOP;
          -- IF v_messagesfailed = 0 THEN
          --  v_messagesfailed := 0+1;
          -- END IF;
          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnDetailWrk6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnDetailWrk6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --           IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR

          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure  DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');
            v_email_flag := v_email_flag + 1;
          end if;
      END;

      --  v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --   COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;

  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnDetailWrk6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');

  END;
END StageToTxnDetailWrk6;

PROCEDURE StageToTxnHeader6(p_Dummy      VARCHAR2,
                            p_email_list VARCHAR2,
                            retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_Header_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnHeader6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (
       select * from LW_txnheader_Wrk6 hw
       --AEO-858 BEGIN
       where 1=1
       and not exists
         (select 1 from ats_txnheader ht where ht.a_txnheaderid = hw.txnheaderid) -- Checking for duplicate txn header records
       --AEO-858 END
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into ats_txnheader
            (a_rowkey,
             a_vckey,
             a_parentrowkey,
             a_shipdate,
             a_ordernumber,
             a_txnqualpurchaseamt,
             a_txnheaderid,
             a_brandid,
             a_creditcardid,
             a_txnmaskid,
             a_txnnumber,
             a_txndate,
             a_txnstoreid,
             a_txntypeid,
             a_txnamount,
             a_txndiscountamount,
             a_storenumber,
             a_txnemployeeid,
             a_txnchannel,
             a_txnoriginaltxnrowkey,
             a_txncreditsused,
             a_txnregisternumber,
             a_txnoriginalstoreid,
             a_txnoriginaltxndate,
             a_txnoriginaltxnnumber,
             statuscode,
             --AEO-846 BEGIN
             a_currencycode,
             a_currencyrate,
             --AEO-846 END
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             a_aeccmultiplier,
             a_originalordernumber,
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             a_cashiernumber,                         --AEO-1533
             createdate,
             updatedate)
          VALUES
            (v_tbl(j).header_rowkey,
             v_tbl(j).vckey,
             v_tbl(j).vckey,
             v_tbl(j).shipdate,
             v_tbl(j).ordernumber,
             NVL(v_tbl(j).txnqualpurchaseamt, 0),
             v_tbl(j).txnheaderid,
             v_tbl(j).brandid,
             v_tbl(j).creditcardid,
             v_tbl(j).txnmaskid,
             v_tbl(j).txnnumber,
             v_tbl(j).txndate,
             v_tbl(j).txnstoreid,
             v_tbl(j).txntypeid,
             NVL(v_tbl(j).txnamount, 0),
             NVL(v_tbl(j).txndiscountamount, 0),
             v_tbl(j).storenumber,
             v_tbl(j).txnemployeeid,
             v_tbl(j).txnchannel,
             v_tbl(j).txnoriginaltxnrowkey,
             v_tbl(j).txncreditsused,
             v_tbl(j).txnregisternumber,
             v_tbl(j).txnoriginalstoreid,
             v_tbl(j).txnoriginaltxndate,
             v_tbl(j).txnoriginaltxnnumber,
             v_tbl(j).statuscode,
             --AEO-846 BEGIN
             v_tbl(j).currencycode,
             v_tbl(j).currencyrate,
             --AEO-846 END
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl(j).aeccmultiplier,
             v_tbl(j).originalordernumber,
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl(j).cashiernumber,                    --AEO-1533
             sysdate,
             sysdate);
      EXCEPTION
        WHEN dml_errors THEN
          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnHeader6: ';
            v_Message        := 'vckey: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .vckey || 'txnheaderid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txnheaderid;

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

          END LOOP;
          -- IF v_messagesfailed = 0 THEN
          --   v_messagesfailed := 0+1;
          -- END IF;
          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnHeader6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnHeader6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --   IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');
            v_email_flag := v_email_flag + 1;
          end if;
      END;
      -- v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --    COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --  COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;

  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnHeader6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');
  END;
END StageToTxnHeader6;

PROCEDURE StageToTxnDetail6(p_Dummy      VARCHAR2,
                            p_email_list VARCHAR2,
                            retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_Detailitem_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnDetailitem6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (
       select dw.*, hw.header_rowkey
       from LW_txndetailitem_Wrk6 dw
       inner join lw_txnheader_Wrk6 hw
       on hw.txnheaderid = dw.txnheaderid
       --AEO-858 BEGIN
       and not exists
       (select 1 from ats_txndetailitem dt where dt.a_txndetailid = dw.txndetailid) -- Checking for duplicate txn detailitem
       --AEO-858 END
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;

      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into ats_txndetailitem
            (a_rowkey,
             a_ipcode,
             a_parentrowkey,
             a_txnheaderid,
             a_txndate,
             a_txnstoreid,
             a_txndetailid,
             a_dtlitemlinenbr,
             a_dtlproductid,
             a_dtltypeid,
             a_dtlactionid,
             a_dtlretailamount,
             a_dtlsaleamount,
             a_dtlquantity,
             a_dtldiscountamount,
             a_dtlclearanceitem,
             a_dtlclasscode,
             statuscode,
             a_dtlsaleamount_org, --AEO-846
             createdate,
             updatedate)
          VALUES
            (v_tbl(j).dtl_rowkey,
             v_tbl(j).ipcode,
             v_tbl(j).header_rowkey,
             v_tbl(j).txnheaderid,
             v_tbl(j).txndate,
             v_tbl(j).txnstoreid,
             v_tbl(j).txndetailid,
             v_tbl(j).dtlitemlinenbr,
             v_tbl(j).dtlproductid,
             v_tbl(j).dtltypeid,
             v_tbl(j).dtlactionid,
             NVL(v_tbl(j).dtlretailamount, 0),
             NVL(v_tbl(j).dtlsaleamount, 0),
             v_tbl(j).dtlquantity,
             NVL(v_tbl(j).dtldiscountamount, 0),
             v_tbl(j).dtlclearanceitem,
             v_tbl(j).dtlclasscode,
             v_tbl(j).statuscode,
             v_tbl(j).dtlsaleamount_org, --AEO-846
             sysdate,
             sysdate);

      EXCEPTION
        WHEN dml_errors THEN
          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnDetailWrk6: ';
            v_Message        := 'ipcode: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .ipcode || 'txndetailid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txndetailid;

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

          END LOOP;
          --  IF v_messagesfailed = 0 THEN
          --    v_messagesfailed := 0+1;
          --  END IF;
          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnDetail6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnDetail6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --           IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');

            v_email_flag := v_email_flag + 1;
          end if;

      END;
      --  v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --   COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;
  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnDetail6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');

  END;
END StageToTxnDetail6;

PROCEDURE StageToTxnRedeem6(p_Dummy      VARCHAR2,
                            p_email_list VARCHAR2,
                            retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_Redeem_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnRedeem6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (select rr.*, hw.header_rowkey
          from LW_TXNREWARDREDEEM_STAGE6 rr
         inner join LW_txnheader_Wrk6 hw
            on hw.txnheaderid = rr.txnheaderid
         where rr.ipcode > 0)
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into ats_txnrewardredeem
            (a_rowkey,
             a_ipcode,
             a_parentrowkey,
             a_txnheaderid,
             a_txndate,
             a_txndetailid,
             a_programid,
             a_certificateredeemtype,
             a_certificatecode,
             a_certificatediscountamount,
             statuscode,
             createdate,
             updatedate)
          VALUES
            (seq_rowkey.nextval,
             v_tbl(j).VCKEY,
             v_tbl(j).header_rowkey,
             v_tbl(j).txnheaderid,
             v_tbl(j).txndate,
             v_tbl(j).txndetailid,
             v_tbl(j).programid,
             v_tbl(j).certificateredeemtype,
             v_tbl(j).certificatecode,
             NVL(v_tbl(j).certificatediscountamount, 0),
             v_tbl(j).statuscode,
             sysdate,
             sysdate);

      EXCEPTION
        WHEN dml_errors THEN
          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnRedeem6: ';
            v_Message        := 'ipcode: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .ipcode || 'txndetailid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txndetailid;

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

          END LOOP;

          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnRedeem6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnRedeem6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --           IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');

            v_email_flag := v_email_flag + 1;
          end if;
      END;
      --     v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --   COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;
  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnRedeem6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');
  END;
END StageToTxnRedeem6;

PROCEDURE StageToTxnDetailDiscount6(p_Dummy      VARCHAR2,
                                    p_email_list VARCHAR2,
                                    retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_DetailDiscount_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnDetailDiscount6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (
       select dds.*, dw.dtl_rowkey
       from LW_TXNDETAILDISCOUNT_STAGE6 dds
       inner join LW_txndetailitem_Wrk6 dw
       on dw.txndetailid = dds.txndetailid
       where dds.ipcode > 0
       --AEO-858 BEGIN
       and not exists
       (select 1 from ATS_TXNLINEITEMDISCOUNT dd where dd.a_txndiscountid = dds.txndiscountid) -- Checking for duplicate txn detailitem
       --AEO-858 END
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into ATS_TXNLINEITEMDISCOUNT
            (a_rowkey,
             a_ipcode,
             a_parentrowkey,
             a_txndiscountid,
             a_txnheaderid,
             a_txndate,
             a_txndetailid,
             a_discounttype,
             a_discountamount,
             a_txnchannel,
             a_offercode,
             statuscode,
             createdate,
             updatedate)
          VALUES
            (seq_rowkey.nextval,
             v_tbl(j).VCKEY,
             v_tbl(j).dtl_rowkey,
             v_tbl(j).txndiscountid,
             v_tbl(j).txnheaderid,
             v_tbl(j).txndate,
             v_tbl(j).txndetailid,
             v_tbl(j).discounttype,
             NVL(v_tbl(j).discountamount, 0),
             v_tbl(j).txnchannel,
             v_tbl(j).offercode,
             v_tbl(j).statuscode,
             sysdate,
             sysdate);
      EXCEPTION
        WHEN dml_errors THEN

          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnDetailDiscount6: ';
            v_Message        := 'ipcode: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .ipcode || 'txndetailid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txndetailid;

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

          END LOOP;

          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnDetailDiscount6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnDetailDiscount6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --           IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR

          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');
            v_email_flag := v_email_flag + 1;
          end if;
      END;

      --   v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --   COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;

  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnDetailDiscount6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');
  END;
END StageToTxnDetailDiscount6;

PROCEDURE StageToTxnTender6(p_Dummy      VARCHAR2,
                            p_email_list VARCHAR2,
                            retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  -- v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_Tender_Staging';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'Staging TxnTender6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (
       select ts.*, hw.header_rowkey
       from LW_TXNTENDER_STAGE6 ts
       inner join lw_txnheader_Wrk6 hw
       on hw.txnheaderid = ts.txnheaderid
       where ts.ipcode > 0
       --AEO-858 BEGIN
       and not exists
         (select 1 from ATS_TXNTENDER tt where tt.a_txntenderid = ts.txntenderid) -- Checking for duplicate txn header records
       --AEO-858 END
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin

        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into ATS_TXNTENDER
            (a_rowkey,
             a_ipcode,
             a_parentrowkey,
             a_txndate,
             a_txnstoreid,
             a_txntenderid,
             a_txnheaderid,
             a_tendertype,
             a_tenderamount,
             a_tendercurrency,
             a_tendertaxamount,
             a_tendertaxrate,
             statuscode,
             createdate,
             updatedate)
          VALUES
            (seq_rowkey.nextval,
             v_tbl(j).VCKEY,
             v_tbl(j).header_rowkey,
             v_tbl(j).txndate,
             v_tbl(j).storeid,
             v_tbl(j).txntenderid,
             v_tbl(j).txnheaderid,
             v_tbl(j).tendertype,
             NVL(v_tbl(j).tenderamount, 0),
             v_tbl(j).tendercurrency,
             NVL(v_tbl(j).tendertax, 0),
             NVL(v_tbl(j).tendertaxrate, 0),
             v_tbl(j).statuscode,
             sysdate,
             sysdate);

      EXCEPTION
        WHEN dml_errors THEN
          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure StageToTxnTender6: ';
            v_Message        := 'ipcode: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .ipcode || 'txntenderid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txntenderid;

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

          END LOOP;

          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure StageToTxnTender6: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>StageToTxnTender6</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --          IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy6 failure DML ERRORS  ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');
            v_email_flag := v_email_flag + 1;
          end if;
      END;
      --      v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --    COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    -- open retval for select v_dap_log_id from dual;
  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
    WHEN OTHERS THEN
      v_Messagesfailed := v_Messagesfailed + 1;

      v_endtime := SYSDATE;

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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>StageToTxnTender6</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');
  END;
END StageToTxnTender6;

PROCEDURE StageToTxnTables6(p_Processdate varchar2,p_email_list VARCHAR2) AS
   p_Dummy2 VARCHAR2(1);
  crsr STAGE_TXNCOPY6.rcursor;
BEGIN

  DECLARE
  BEGIN
   StageToTxnHeadWrk6(p_Dummy2,p_email_list,crsr);
   StageToTxnDetailWrk6(p_Dummy2,p_email_list,crsr);
   Giftcard24HrPurchase(p_Dummy2,crsr);
   BasicReturns24HrAdjust(p_Dummy2,crsr);
   TxnHeaderWrk6Bkp(p_Dummy2,p_email_list,crsr);
   TxnDetailWrk6Bkp(p_Dummy2,p_email_list,crsr);
   StageToTxnHeader6(p_Dummy2,p_email_list,crsr);
   StageToTxnDetail6(p_Dummy2,p_email_list,crsr);
   StageToTxnDetailDiscount6(p_Dummy2,p_email_list,crsr);
   StageToTxnRedeem6(p_Dummy2,p_email_list,crsr);
   StageToTxnTender6(p_Dummy2,p_email_list,crsr);
   --ReturnRedeemTxnPoints(p_Processdate ,p_email_list,crsr);
   END;
END StageToTxnTables6;

PROCEDURE TlogInterceptorProcessing6(p_Dummy VARCHAR2, Retval  IN OUT Rcursor) AS

  bcheckitembrand number := 0;
  bMemberHasBrand number := 1;
  bDtlMemberHasBrand number := 1;
  lStoreBrandID number := 0;
  lDtlBrandID number := 0;
  v_cnt              NUMBER:=0;
  v_itembrandcnt     Number :=0;
  v_MemberHasBrandcnt Number :=0;
  v_DtlMemberHasBrand Number :=0;

   --log job attributes
  v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;
  v_jobdirection          NUMBER:=0;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);
  v_processId             NUMBER:=0;
  v_Errormessage          VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256):= ' ' ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  v_recordcount        NUMBER :=0;

  BEGIN
     v_jobname := 'TLog6-TlogInterceptorProcessing';
     v_logsource := v_jobname;

    v_my_log_id := utility_pkg.get_LIBJobID();
   -- v_dap_log_id := utility_pkg.get_LIBJobID();

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
 utility_pkg.Log_Process_Start(v_jobname, 'TlogInterceptorProcessing6', v_processId);
 /*Logging*/
 DECLARE
   CURSOR get_data IS
      WITH Mem AS
       (
      select tw.txnstoreid,tw.ipcode as vckey,vc.ipcode as ipcode,tw.dtlproductid,tw.dtltypeid
      from lw_txndetailitem_wrk6 tw
      inner join lw_txnheader_wrk6 hw on hw.txnheaderid = tw.txnheaderid
      inner join lw_virtualcard vc on vc.vckey = tw.ipcode
      where hw.txntypeid = 1 or hw.txntypeid = 4
      )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

 BEGIN
   Open get_data;
     Loop
     FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 10000;
     --   FOR y in    get_data LOOP
     FOR i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
        LOOP
          SELECT Count(sd.brandname) into v_itembrandcnt          --  making sure, if there is a record.
            from lw_storedef sd
           inner join ats_refbrand rb
              on rb.a_brandid = sd.brandname
           where sd.storeid = v_tbl(i).txnstoreid;

           If (v_itembrandcnt = 0)
          -- THEN select 0 into bcheckitembrand from dual; /*AEO-134 here ------- Changes to check brand flags for all txns ------------SCJ*/
             THEN select 1 into bcheckitembrand from dual;
           else
          SELECT sd.brandname,--rb.a_brandnumber,
          /*AEO-134 Begin ------- Changes to check brand flags for all txns ------------SCJ*/
                /* case
                   when rb.a_brandnumber >= 0 then
                    0
                   when rb.a_brandnumber < 0 then
                    1
                   when rb.a_brandnumber is null then
                    0
                 end */
                 1
           /*AEO-134 End ------- Changes to check brand flags for all txns ------------SCJ*/
            into lStoreBrandID, bcheckitembrand
            from lw_storedef sd
           inner join ats_refbrand rb
              on rb.a_brandid = sd.brandname
           where sd.storeid = v_tbl(i).txnstoreid;
           end if;
          --if we need to check the item brands, skip over store branding
         IF  (bcheckitembrand = 0) then    -- #region Check for Item brands, No need for item brands check,so store branding
                IF  (lStoreBrandID > 0)  then
                    SELECT count(*)  INTO v_MemberHasBrandcnt          --  making sure, if there is a record.
                    from ats_memberbrand mb
                    where  1=1
                    and mb.a_ipcode = v_tbl(i).ipcode
                    and  mb.a_brandid = lStoreBrandID;

                    IF ((v_MemberHasBrandcnt = 0)) then
                       SELECT 0 into bMemberHasBrand from dual;
                    END IF;
                    IF ((v_MemberHasBrandcnt > 0))  then
                       SELECT CASE WHEN COUNT(mb.a_brandid) > 0 THEN 1 ELSE 0
                              END INTO bMemberHasBrand    -- setting of memberbrandFlag
                       from ats_memberbrand mb
                       where  1=1
                       and mb.a_ipcode = v_tbl(i).ipcode
                       and  mb.a_brandid = lStoreBrandID;
                    END IF;
                END IF;
                IF  (lStoreBrandID <= 0)  then
                    bcheckitembrand := 1;
                END IF;
                IF (bMemberHasBrand = 0) THEN  --- Member does not have brands, so add it
                    INSERT INTO ats_memberbrand
                      (a_rowkey,
                       a_ipcode,
                       a_parentrowkey,
                       a_changedby,
                       a_brandid,
                       statuscode,
                       createdate,
                       updatedate)
                    VALUES
                      (SEQ_ROWKEY.NEXTVAL,
                       v_tbl(i).ipcode,
                       v_tbl(i).ipcode,
                       'Tlog Processor',
                       lStoreBrandID,
                       0,
                       SYSDATE,
                       SYSDATE)
                    LOG ERRORS INTO err$_ats_memberbrand('INSERT') REJECT LIMIT UNLIMITED;
                       COMMIT;
                END IF;
         END IF;
         -- For those records whose brands are not set, check each product at detail item
          IF  (bcheckitembrand = 1) then
                   SELECT count(PD.BRANDNAME) INTO v_DtlMemberHasBrand  -- making sure, if there is a record.
                   FROM lw_product pd
                   WHERE PD.ID = v_tbl(i).dtlproductid;
                   IF (v_DtlMemberHasBrand > 0)  then
                        IF  v_tbl(i).dtltypeid = 1  then
                            SELECT PD.BRANDNAME INTO lDtlBrandID
                            FROM lw_product pd
                            WHERE PD.ID = v_tbl(i).dtlproductid;
                            SELECT CASE
                                WHEN COUNT(mb.a_brandid) > 0 THEN  1 ELSE     0        -- setting of memberbrandFlag
                                END INTO bDtlMemberHasBrand
                            from ats_memberbrand mb
                            where 1=1
                            and   mb.a_ipcode = v_tbl(i).ipcode
                            and   mb.a_brandid = lDtlBrandID;
                        END IF;
                   END IF;
                   IF (bDtlMemberHasBrand = 0) THEN
                      INSERT INTO ats_memberbrand
                                  (a_rowkey,
                                  a_ipcode,
                                  a_parentrowkey,
                                  a_changedby,
                                  a_brandid,
                                  statuscode,
                                  createdate,
                                  updatedate)
                               VALUES
                                 (SEQ_ROWKEY.NEXTVAL,
                                  v_tbl(i).ipcode,
                                  v_tbl(i).ipcode,
                                  'Tlog Processor',
                                  lDtlBrandID,
                                  0,
                                  SYSDATE,
                                  SYSDATE)
                      LOG ERRORS INTO err$_ats_memberbrand('INSERT') REJECT LIMIT UNLIMITED;
                                  COMMIT;
                   END IF;
                             COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
                   If (v_DtlMemberHasBrand = 0) Then
                            v_Messagesfailed := v_Messagesfailed + 1;
                            v_Error          := SQLERRM;
                            v_Reason         := 'Unknown product SKU during bcheckitembrand : ' ||v_tbl(i).dtlproductid ;
                            v_Message        := 'TlogInterceptorProcessing6 is not finished';

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


                   End if;
          End if;
        END LOOP;
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;
     COMMIT;

     IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
     END IF;
    END;

  v_endtime := SYSDATE;
  v_jobstatus := 1;
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => 0
         ,p_messagesfailed     => 0
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

         -- open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN
--  IF v_messagesfailed = 0 THEN
--    v_messagesfailed := 0+1;
--  END IF;
    v_Messagesfailed := v_Messagesfailed + 1;
    v_Error          := SQLERRM;
    v_Reason         := 'Failed Procedure TlogInterceptorProcessing6: ';
    v_Message        := '<failed>' || Chr(10) ||
                        '  <details>' || Chr(10) ||
                        '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                        Chr(10) ||
                        '    <proc>TlogInterceptorProcessing6</proc>' ||
                        Chr(10) || '  </details>' ||
                        Chr(10) || '</failed>';

    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => null,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);

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
  RAISE;

END TlogInterceptorProcessing6;

PROCEDURE UpdateMemberAttributeFromTlog6(p_Processdate varchar2, Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'STAGE_TXNCOPY6.UpdateMemberAttributesFromTlog6';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'STAGE_TXNCOPY6.UpdateMemberAttributesFromTlog6';

    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'UpdateMemberAttributesFromTlog6';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));

    v_bra_codes            AETableType ;
    v_jean_codes           AETableType ;
    v_cclist               varchar2(1000);
    v_cclist2              varchar2(1000);
    v_Processdate       timestamp:=to_date(p_Processdate, 'YYYYMMddHH24miss');

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

/*
PI29741 tlog optimization changes begin here                 -----scj
*/
/*
    ---------------------------
    -- Update Employee flag
    ---------------------------
    DECLARE
      CURSOR get_data IS
        SELECT txn.ipcode
          FROM lw_txndetail_stage txn
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
*/
 select cc.value into v_cclist from Lw_Clientconfiguration cc
                                             where cc.key = 'BraPromoClassCodes';
   select cc.value into v_cclist2 from Lw_Clientconfiguration cc
                                             where cc.key = 'JeansPromoClassCodes';
   v_bra_codes:=StringToTable(v_cclist,';');
   v_jean_codes:=StringToTable(v_cclist2,';');

DECLARE
      CURSOR get_data IS
         select max(st.storenumber) AS storenumber,
                st.ipcode,
                st.txnqualpurchaseamt,
                st.promo,
                st.txndate,
                ST.dtltypeid,
                MAX(ST.TXNEMPLOYEEID) AS TXNEMPLOYEEID,
                max(ST.primaryemailaddress) as email
           from (SELECT sd.storenumber,
                        txn.ipcode,
                        txn.dtlsaleamount,
                        txn.txnqualpurchaseamt,
                        txn.dtlclasscode,
                        txn.dtlproductid,
                        txn.txndate,
                        txn.dtltypeid,
                        TXN.TXNEMPLOYEEID,
                        CHECKAEPROMO(TXN.DTLCLASSCODE,
                                     TXN.DTLPRODUCTID,
                                     v_bra_codes,
                                     v_jean_codes) as promo,
                        LM.PRIMARYEMAILADDRESS
                   FROM lw_txndetail_stage6 txn
                  inner join lw_storedef sd
                     on sd.storeid = txn.txnstoreid
                  inner join lw_loyaltymember lm
                     on lm.ipcode = txn.ipcode
                  WHERE txn.ipcode > 0
                  order by txn.ipcode desc) st
          group by st.ipcode,
                   st.storenumber,
                   st.txnqualpurchaseamt,
                   st.txndate,
                   st.promo,
                   ST.dtltypeid;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count -- SAVE EXCEPTIONS--<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
           UPDATE ats_memberdetails mds
              SET MDS.a_Lastbrastorenumber = CASE
                                               WHEN v_tbl(i).PROMO = 'BRAPROMO' AND
                                                     (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) THEN
                                                to_number(v_tbl(i).STORENUMBER)
                                                Else
                                                  mds.a_Lastbrastorenumber
                                             END,
                  MDS.A_LASTBRAPURCHASEDATE = CASE
                                                WHEN v_tbl(i).PROMO = 'BRAPROMO' AND
                                                      (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) THEN
                                                 v_tbl(i).txndate
                                                 else
                                                   MDS.A_LASTBRAPURCHASEDATE
                                              END,
                  MDS.A_BRAFIRSTPURCHASEDATE = CASE
                                                 WHEN v_tbl(i).PROMO = 'BRAPROMO' AND
                                                       (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) AND
                                                       MDS.A_BRAFIRSTPURCHASEDATE IS NULL THEN
                                                  v_tbl(i).txndate
                                                  else
                                                   MDS.A_BRAFIRSTPURCHASEDATE
                                               END,
                  MDS.A_JEANSFIRSTPURCHASEDATE = CASE
                                                   WHEN v_tbl(i).PROMO = 'JEANPROMO' AND
                                                         (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) AND
                                                         MDS.A_JEANSFIRSTPURCHASEDATE IS NULL THEN
                                                    v_tbl(i).txndate
                                                    else
                                                    MDS.A_JEANSFIRSTPURCHASEDATE
                                                 END,
                   MDS.a_Lastjeanspurchasedate = CASE
                                                WHEN v_tbl(i).PROMO = 'JEANPROMO' AND
                                                      (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) THEN
                                                 v_tbl(i).txndate
                                                 else
                                                   MDS.a_Lastjeanspurchasedate
                                              END,
                  MDS.a_storelastjeanspurchased = CASE
                                               WHEN v_tbl(i).PROMO = 'JEANPROMO' AND
                                                     (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) THEN
                                                v_tbl(i).STORENUMBER
                                                Else
                                                 mds.a_storelastjeanspurchased

                                             END,
                  MDS.A_LASTPURCHASEPOINTS = CASE
                                               WHEN v_tbl(i).txnqualpurchaseamt >= 0 AND
                                                     (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) THEN
                                                v_tbl(i).txnqualpurchaseamt
                                               WHEN v_tbl(i).txnqualpurchaseamt < 0 AND
                                                     (v_tbl(i).dtltypeid = 1 OR v_tbl(i).dtltypeid = 4) THEN
                                                0
                                             END,
                  MDS.A_EMPLOYEECODE = CASE
                                         WHEN v_tbl(i).TXNEMPLOYEEID IS NOT NULL AND
                                               (NVL(MDS.A_EMPLOYEECODE, 0) = 0 OR
                                                MDS.A_EMPLOYEECODE = 2) THEN
                                          1
                                          else
                                          MDS.A_EMPLOYEECODE
                                       END,
                  MDS.A_EMAILADDRESS = CASE
                                        WHEN v_tbl(i).email IS NOT NULL
                                          THEN v_tbl(i).EMAIL
                                            ELSE MDS.A_EMAILADDRESS
                                       END

            WHERE mds.a_ipcode = v_tbl(i).ipcode;

        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        FOR J IN 1 .. v_tbl.count
          LOOP
          UPDATE LW_LOYALTYMEMBER LM
          set LM.LASTACTIVITYDATE  = CASE
                                      WHEN LM.LASTACTIVITYDATE IS NULL
                                        THEN v_tbl(J).txndate
                                      WHEN LM.LASTACTIVITYDATE < v_tbl(J).txndate
                                        THEN v_tbl(J).txndate
                                       ELSE LM.LASTACTIVITYDATE
                                      END ,
               LM.PRIMARYEMAILADDRESS = null
           WHERE LM.IPCODE = v_tbl(J).ipcode
         LOG ERRORS INTO err$_LW_LOYALTYMEMBER('UPDATE') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;
/*
PI29741 tlog optimization changes end here                 -----scj
*/

      /* AEO 623 ------- Change TLOG process to NOT set cardtype in memberdetails based on tender type removed cardholder check here.*/

    ---------------------------
    -- Update UnderAge flag
    ---------------------------
	/* AEO-2285 begin
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
        FORALL i IN 1 .. v_tbl.count -- SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
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
	AEO-2285 end */
    /*redesign changes begin here  -------------------------------------------------------SCJ  */
    ---------------------------
    -- Update NetSpend field
    ---------------------------
  /*  DECLARE
      CURSOR get_data2 IS
        SELECT max(txn.ipcode) as ipcode,
              max(txn.txnqualpurchaseamt) as txnqualpurchaseamt,
              txn.txnheaderid
                   FROM lw_txndetail_stage6 txn
                  WHERE txn.ipcode > 0
                   group by txn.txnheaderid;
      TYPE t_tab IS TABLE OF get_data2%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data2;
      LOOP
        FETCH get_data2 BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count -- SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails mds
             SET mds.a_Netspend = case
                                    when (Nvl(MDS.a_Netspend, 0) +
                                         Nvl(v_tbl(i).txnqualpurchaseamt, 0) < 0) then
                                     0
                                    else
                                     Nvl(MDS.a_Netspend, 0) +
                                     Nvl(v_tbl(i).txnqualpurchaseamt, 0)
                                  end
           WHERE mds.a_ipcode = v_tbl(i).ipcode;

        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data2%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data2%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data2;
      END IF;
    END;


 ---------------------------
    -- Update Tier Level and Tier Expiration date
    ---------------------------
    DECLARE
      CURSOR get_nettier IS
        SELECT tds.ipcode,max(md1.a_netspend) as netspend ,max(vc1.status) as status
          FROM lw_txndetail_stage6 tds
         inner join ats_memberdetails md1 on md1.a_ipcode = tds.ipcode
         inner join lw_virtualcard vc1 on vc1.ipcode = md1.a_ipcode
         WHERE 1 = 1
         and tds.ipcode > 0
         and NVL(md1.a_extendedplaycode,0) = 1
         group by tds.ipcode;
       v_tierid       NUMBER := 0;
       v_tierdesc     VARCHAR2(50);
       v_Bluetier     NUMBER := 0;
       v_Silvertier   NUMBER := 0;
       v_cnt          NUMBER := 0;
       v_ct_tierid    NUMBER := 0;
       v_ct_updtierid NUMBER := 0;
     -- TYPE t_tab IS TABLE OF get_nettier%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
     -- v_tbl t_tab; ---<------ our arry object
    BEGIN


       SELECT tr.tierid
         INTO v_Bluetier
         FROM lw_tiers tr
        WHERE tr.tiername = 'Blue';
       SELECT tr.tierid
         INTO v_Silvertier
         FROM lw_tiers tr
        WHERE tr.tiername = 'Silver';

     FOR y IN get_nettier LOOP

        SELECT count(mt.tierid)  --mt.tierid
         INTO v_ct_tierid
         FROM lw_membertiers mt
        WHERE mt.memberid = y.ipcode
          AND trunc(mt.todate) = to_date('12/31/2199', 'mm/dd/yyyy');
       If v_ct_tierid > 0 then
          SELECT mt.tierid,mt.description  --mt.tierid
         INTO v_tierid,v_tierdesc
         FROM lw_membertiers mt
        WHERE mt.memberid = y.ipcode
          AND trunc(mt.todate) = to_date('12/31/2199', 'mm/dd/yyyy');
       End if;

       IF y.netspend >= 250 and y.status = 1  then
              IF v_tierid = v_bluetier THEN  --IF THIS IS BLUETIER,THEN UPDATE BLUE TIER RECORD AND ADD A SILVER TIER
                UPDATE lw_membertiers mt1
                SET    mt1.todate = SYSDATE
                WHERE  mt1.memberid = y.ipcode
                AND    mt1.tierid = v_bluetier;
                Update ats_memberdetails md2
                set md2.a_aitupdate = 1
                where md2.a_ipcode = y.ipcode;
                INSERT INTO lw_membertiers
                            (
                                        id,
                                        tierid,
                                        memberid,
                                        fromdate,
                                        todate,
                                        description,
                                        createdate,
                                        updatedate
                            )
                            VALUES
                            (
                                        hibernate_sequence.NEXTVAL,
                                        v_silvertier,
                                        y.ipcode,
                                        SYSDATE,
                                        To_date('12/31/2199 ', 'mm/dd/yyyy'),
                                        'Qualifier',
                                        SYSDATE,
                                        SYSDATE
                           );
              END IF;
              IF v_ct_tierid = 0 THEN   --IF THERE ARE NEITHER OF THE TIERS,THEN ADD A SILVER TIER
                INSERT INTO lw_membertiers
                            (
                                        id,
                                        tierid,
                                        memberid,
                                        fromdate,
                                        todate,
                                        description,
                                        createdate,
                                        updatedate
                            )
                            VALUES
                            (
                                        hibernate_sequence.NEXTVAL,
                                        v_silvertier,
                                        y.ipcode,
                                        SYSDATE,
                                        To_date('12/31/2199 ', 'mm/dd/yyyy'),
                                        'Qualifier',
                                        SYSDATE,
                                        SYSDATE
                           );

                Update ats_memberdetails md2
                set md2.a_aitupdate = 1
                where md2.a_ipcode = y.ipcode;

              END IF;

       END IF;
       IF y.netspend < 250 then
          IF v_tierid = v_Silvertier and Upper(v_tierdesc)  not like Upper('%Nomination%')  THEN  --IF THIS IS SILVER TIER and not an Nominated Status,THEN UPDATE SILVER TIER RECORD AND ADD A BLUE TIER
                UPDATE lw_membertiers mt1
                SET    mt1.todate = SYSDATE
                WHERE  mt1.memberid = y.ipcode
                AND    mt1.tierid = v_Silvertier;
                Update ats_memberdetails md2
                set md2.a_aitupdate = 1
                where md2.a_ipcode = y.ipcode;
                INSERT INTO lw_membertiers
                            ( id,
                              tierid,
                              memberid,
                              fromdate,
                              todate,
                              description,
                              createdate,
                              updatedate
                            )
                            VALUES
                            ( hibernate_sequence.NEXTVAL,
                              v_Bluetier,
                              y.ipcode,
                              SYSDATE,
                              To_date('12/31/2199 ', 'mm/dd/yyyy'),
                              'Qualifier',
                              SYSDATE,
                              SYSDATE
                           );
                INSERT INTO lw_csnote
                            (id,
                                 memberid,
                                 note,
                                 createdby,
                                 createdate,
                            updatedate)
                            VALUES
                           (seq_csnote.nextval,
                            y.ipcode,
                            'Tier downgraded due to net spend dropping below threshold',
                            1410, --Brierley System
                            SYSDATE,
                            SYSDATE);

          END IF;
          IF v_ct_tierid = 0 THEN   --IF THERE ARE NEITHER OF THE TIERS,THEN ADD A Blue TIER
                INSERT INTO lw_membertiers
                            (
                                        id,
                                        tierid,
                                        memberid,
                                        fromdate,
                                        todate,
                                        description,
                                        createdate,
                                        updatedate
                            )
                            VALUES
                            (
                                        hibernate_sequence.NEXTVAL,
                                        v_bluetier,
                                        y.ipcode,
                                        SYSDATE,
                                        To_date('12/31/2199 ', 'mm/dd/yyyy'),
                                        'Qualifier',
                                        SYSDATE,
                                        SYSDATE
                           );
                Update ats_memberdetails md2
                set md2.a_aitupdate = 1
                where md2.a_ipcode = y.ipcode;
                           --Did the member have a Silver Tier status?
          SELECT count(mt.tierid)
            INTO v_ct_updtierid
            FROM lw_membertiers mt
           WHERE mt.memberid = y.ipcode
             AND mt.tierid = v_Silvertier;

             IF v_ct_updtierid > 0 then
               INSERT INTO lw_csnote
                            (id,
                                 memberid,
                                 note,
                                 createdby,
                                 createdate,
                            updatedate)
                            VALUES
                           (seq_csnote.nextval,
                            y.ipcode,
                            'Tier downgraded due to net spend dropping below threshold',
                            1410, --Brierley System
                            SYSDATE,
                            SYSDATE);
              END IF;

          END IF;
       END IF;
      v_cnt        := v_cnt + 1;
      IF MOD(v_cnt, 1000) = 0 THEN
        COMMIT;
      END IF;
    END LOOP;
    COMMIT;
    END;
    */
 /*redesign changes begin end  SCJ  */

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
      v_Reason         := 'Failed Procedure UpdateMemberAttributesFromTlog6';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                          '    <proc>UpdateMemberAttributesFromTlog6</proc>' ||
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

    RAISE;
  END UpdateMemberAttributeFromTlog6;

    PROCEDURE Giftcard24HrPurchase(p_Dummy VARCHAR2,retval IN OUT rcursor) AS
--log job attributes
 v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;
  v_jobdirection          NUMBER:=0;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);
  v_processId             NUMBER:=0;
  v_Errormessage          VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256):= ' ' ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  v_recordcount        NUMBER :=0;
BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
   -- v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'TLog6-Giftcard24HrPurchase';
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

utility_pkg.Log_Process_Start(v_jobname, 'Giftcard24HrPurchase', v_processId);
  DECLARE
        CURSOR get_data IS
      WITH Mem AS
       (
     Select ct.ipcode, ct.txnheaderid
       from (select max(dw.ipcode) as ipcode,
                    dw.txnheaderid,
                    trunc(dw.txndate) as txndate,
                    ROW_NUMBER() OVER(PARTITION BY max(dw.ipcode), trunc(dw.txndate) ORDER BY max(dw.ipcode)) as rn
               from lw_txndetailitem_wrk6 dw
              where 1 = 1
                and dw.dtlclasscode = '9911'
                and dw.dtlsaleamount > 0
              group by dw.txnheaderid, trunc(dw.txndate)) ct
      where ct.rn > 3
      )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
     -- clear work table before next insert
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 10000;
    FORALL j IN 1 .. v_tbl.COUNT -- SAVE EXCEPTIONS
        UPDATE  LW_txndetailitem_Wrk6 DW
        SET DW.DTLSALEAMOUNT = 0
        WHERE 1 = 1
        AND DW.TXNHEADERID  = v_tbl(J).txnheaderid
        AND DW.DTLCLASSCODE = '9911'
        AND DW.IPCODE       = v_tbl(J).ipcode ;
     v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    COMMIT;
  END;
   v_endtime := SYSDATE;
  v_jobstatus := 1;
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

         -- open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN
 -- IF v_messagesfailed = 0 THEN
  --  v_messagesfailed := 0+1;
 -- END IF;
  v_Messagesfailed := v_Messagesfailed + 1;
  v_Error          := SQLERRM;
  v_Reason         := 'Failed Procedure Giftcard24HrPurchase: ';
  v_Message        := '<failed>' || Chr(10) ||
                      '  <details>' || Chr(10) ||
                      '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                      Chr(10) ||
                      '    <proc>Giftcard24HrPurchase</proc>' ||
                      Chr(10) || '  </details>' ||
                      Chr(10) || '</failed>';

  Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                      p_Jobdirection     => v_Jobdirection,
                      p_Filename         => null,
                      p_Starttime        => v_Starttime,
                      p_Endtime          => v_Endtime,
                      p_Messagesreceived => v_Messagesreceived,
                      p_Messagesfailed   => v_Messagesfailed,
                      p_Jobstatus        => v_Jobstatus,
                      p_Jobname          => v_Jobname);
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
  RAISE;
END Giftcard24HrPurchase;

  PROCEDURE BasicReturns24HrAdjust(p_Dummy VARCHAR2,retval IN OUT rcursor) AS
--log job attributes
 v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;
  v_jobdirection          NUMBER:=0;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);
  v_processId             NUMBER:=0;
  v_Errormessage          VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256):= ' ' ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  v_recordcount        NUMBER :=0;
BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
-- v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'TLog6-BasicReturns24HrAdjust';
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

utility_pkg.Log_Process_Start(v_jobname, 'BasicReturns24HrAdjust', v_processId);
  DECLARE
        CURSOR get_data IS
      WITH Mem AS
       (
       select hw.txnheaderid from lw_txnheader_wrk6 hw
       inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
       where 1=1
       and pt.points = 0
       and hw.txntypeid = 2
       and hw.txnqualpurchaseamt < 0
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
     -- clear work table before next insert
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 10000;
    FORALL j IN 1 .. v_tbl.COUNT -- SAVE EXCEPTIONS
        UPDATE  Lw_Txnheader_Wrk6 HW
        SET  HW.Txnqualpurchaseamt = 0
        WHERE 1 = 1
        AND HW.Txnheaderid   = v_tbl(J).txnheaderid ;
     v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    COMMIT;
  END;
   v_endtime := SYSDATE;
  v_jobstatus := 1;
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

        -- open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN
 -- IF v_messagesfailed = 0 THEN
  --  v_messagesfailed := 0+1;
 -- END IF;
  v_Messagesfailed := v_Messagesfailed + 1;
  v_Error          := SQLERRM;
  v_Reason         := 'Failed Procedure BasicReturns24HrAdjust: ';
  v_Message        := '<failed>' || Chr(10) ||
                      '  <details>' || Chr(10) ||
                      '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                      Chr(10) ||
                      '    <proc>BasicReturns24HrAdjust</proc>' ||
                      Chr(10) || '  </details>' ||
                      Chr(10) || '</failed>';

  Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                      p_Jobdirection     => v_Jobdirection,
                      p_Filename         => null,
                      p_Starttime        => v_Starttime,
                      p_Endtime          => v_Endtime,
                      p_Messagesreceived => v_Messagesreceived,
                      p_Messagesfailed   => v_Messagesfailed,
                      p_Jobstatus        => v_Jobstatus,
                      p_Jobname          => v_Jobname);
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
  RAISE;
END BasicReturns24HrAdjust;


PROCEDURE TxnHeaderWrk6Bkp(p_Dummy      VARCHAR2,
                           p_email_list VARCHAR2,
                           retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

  --BEGIN
BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  --  v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_Header2_Bkp';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'BackUp TxnHeaderWrk6',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (
       select * from LW_txnheader_Wrk6 wk
       --AEO-858 BEGIN
       where 1=1
       and not exists
         (select 1 from LW_txnheader_Wrk6Bkp wb where wb.txnheaderid = wk.txnheaderid) -- Checking for duplicate txn header records
       --AEO-858 END
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into LW_txnheader_Wrk6Bkp
            (rowkey,
             header_rowkey,
             vckey,
             parentrowkey,
             shipdate,
             ordernumber,
             txnqualpurchaseamt,
             txnheaderid,
             brandid,
             creditcardid,
             txnmaskid,
             txnnumber,
             txndate,
             txnstoreid,
             txntypeid,
             txnamount,
             txndiscountamount,
             storenumber,
             txnemployeeid,
             txnchannel,
             txnoriginaltxnrowkey,
             txncreditsused,
             txnregisternumber,
             txnoriginalstoreid,
             txnoriginaltxndate,
             txnoriginaltxnnumber,
             statuscode,
             --AEO-846 BEGIN
             currencyrate,
             currencycode,
             --AEO-846 END
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             aeccmultiplier,
             originalordernumber,
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             createdate,
             updatedate)
          VALUES
            (v_tbl  (j).rowkey,
             v_tbl  (j).header_rowkey,
             v_tbl  (j).vckey,
             v_tbl  (j).parentrowkey,
             v_tbl  (j).shipdate,
             v_tbl  (j).ordernumber,
             v_tbl  (j).txnqualpurchaseamt,
             v_tbl  (j).txnheaderid,
             v_tbl  (j).brandid,
             v_tbl  (j).creditcardid,
             v_tbl  (j).txnmaskid,
             v_tbl  (j).txnnumber,
             v_tbl  (j).txndate,
             v_tbl  (j).txnstoreid,
             v_tbl  (j).txntypeid,
             v_tbl  (j).txnamount,
             v_tbl  (j).txndiscountamount,
             v_tbl  (j).storenumber,
             v_tbl  (j).txnemployeeid,
             v_tbl  (j).txnchannel,
             v_tbl  (j).txnoriginaltxnrowkey,
             v_tbl  (j).txncreditsused,
             v_tbl  (j).txnregisternumber,
             v_tbl  (j).txnoriginalstoreid,
             v_tbl  (j).txnoriginaltxndate,
             v_tbl  (j).txnoriginaltxnnumber,
             v_tbl  (j).statuscode,
             --AEO-846 BEGIN
             v_tbl  (j).currencyrate,
             v_tbl  (j).currencycode,
             --AEO-846 END
              /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             v_tbl  (j).aeccmultiplier,
             v_tbl  (j).originalordernumber,
             /*National Rollout Changes AEO-1218 here        ----------------SCJ*/
             sysdate,
             sysdate);

      EXCEPTION
        WHEN dml_errors THEN
          -- IF v_messagesfailed = 0 THEN
          --   v_messagesfailed := 0+1;
          -- END IF;

          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP

            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure TxnHeaderWrk6Bkp: ';
            v_Message        := 'vckey: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .vckey || 'txnheaderid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txnheaderid;

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

          END LOOP;
          Commit;

          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure TxnHeaderWrk6Bkp: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>TxnHeaderWrk6Bkp</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --   IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy failure DML ERRORS ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');

            v_email_flag := v_email_flag + 1;
          end if;
      END;

      -- v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --    COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    --       open retval for select v_dap_log_id from dual;

  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>TxnHeaderWrk6Bkp</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');
  END;

END TxnHeaderWrk6Bkp;

PROCEDURE TxnDetailWrk6Bkp(p_Dummy      VARCHAR2,
                           p_email_list VARCHAR2,
                           retval       IN OUT rcursor) AS
  v_attachments cio_mail.attachment_tbl_type;
  dml_errors EXCEPTION;
  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
  --log job attributes
  v_my_log_id        NUMBER;
  v_dap_log_id       NUMBER;
  v_jobdirection     NUMBER := 0;
  v_starttime        DATE := SYSDATE;
  v_endtime          DATE;
  v_messagesreceived NUMBER := 0;
  v_messagesfailed   NUMBER := 0;
  v_jobstatus        NUMBER := 0;
  v_jobname          VARCHAR2(256);
  v_processId        NUMBER := 0;
  v_Errormessage     VARCHAR2(256);

  --log msg attributes
  v_messageid   VARCHAR2(256);
  v_envkey      VARCHAR2(256) := 'bp_ae@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
  v_logsource   VARCHAR2(256);
  v_batchid     VARCHAR2(256) := 0;
  v_message     VARCHAR2(256) := ' ';
  v_reason      VARCHAR2(256);
  v_error       VARCHAR2(256);
  v_trycount    NUMBER := 0;
  v_recordcount NUMBER := 0;
  v_email_flag  NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  --  v_dap_log_id := utility_pkg.get_LIBJobID();

  v_jobname   := 'TLog6-TXN_Detailitem_WrkBkp';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB              => v_my_log_id,
                      p_jobdirection     => v_jobdirection,
                      p_filename         => null,
                      p_starttime        => v_starttime,
                      p_endtime          => v_endtime,
                      p_messagesreceived => v_messagesreceived,
                      p_messagesfailed   => v_messagesfailed,
                      p_jobstatus        => v_jobstatus,
                      p_jobname          => v_jobname);

  utility_pkg.Log_Process_Start(v_jobname,
                                'TxnDetailitem Wrk6 Bkp',
                                v_processId);
  DECLARE
    CURSOR get_data IS
      WITH Mem AS
       (
       select dw.*, hw.header_rowkey
       from LW_txndetailitem_Wrk6 dw
       inner join lw_txnheader_Wrk6 hw
       on hw.txnheaderid = dw.txnheaderid
       --AEO-858 BEGIN
       and not exists
       (select 1 from LW_txndetailitem_Wrk6Bkp db where db.txndetailid = dw.txndetailid) -- Checking for duplicate txn detailitem
       --AEO-858 END
       )
      SELECT * FROM Mem;
    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;
  BEGIN
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT
        INTO v_tbl LIMIT 1000;
      IF v_tbl.count > 0 THEN
        --AEO 654 changes begin ------------------SCJ
        v_messagesreceived := v_messagesreceived + v_tbl.count;
      END IF;
      Begin
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
          Insert into LW_txndetailitem_Wrk6Bkp
            (rowkey,
             dtl_rowkey,
             ipcode,
             parentrowkey,
             txnheaderid,
             txndate,
             txnstoreid,
             txndetailid,
             dtlitemlinenbr,
             dtlproductid,
             dtltypeid,
             dtlactionid,
             dtlretailamount,
             dtlsaleamount,
             dtlquantity,
             dtldiscountamount,
             dtlclearanceitem,
             dtlclasscode,
             statuscode,
             dtlsaleamount_org, --AEO-846
             createdate,
             updatedate)
          VALUES
            (v_tbl  (j).rowkey,
             v_tbl  (j).dtl_rowkey,
             v_tbl  (j).ipcode,
             v_tbl  (j).parentrowkey,
             v_tbl  (j).txnheaderid,
             v_tbl  (j).txndate,
             v_tbl  (j).txnstoreid,
             v_tbl  (j).txndetailid,
             v_tbl  (j).dtlitemlinenbr,
             v_tbl  (j).dtlproductid,
             v_tbl  (j).dtltypeid,
             v_tbl  (j).dtlactionid,
             v_tbl  (j).dtlretailamount,
             v_tbl  (j).dtlsaleamount,
             v_tbl  (j).dtlquantity,
             v_tbl  (j).dtldiscountamount,
             v_tbl  (j).dtlclearanceitem,
             v_tbl  (j).dtlclasscode,
             v_tbl  (j).statuscode,
             v_tbl  (j).dtlsaleamount_org, --AEO-846
             sysdate,
             sysdate);
      EXCEPTION
        WHEN dml_errors THEN

          FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT LOOP
            v_Messagesfailed := v_Messagesfailed + 1;
            v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx)
                                        .ERROR_CODE);
            v_Reason         := 'Failed Records in Procedure TxnDetailWrk6Bkp: ';
            v_Message        := 'ipcode: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .ipcode || 'txndetailid: ' || v_tbl(sql%BULK_EXCEPTIONS(indx).ERROR_INDEX)
                               .txndetailid;

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

          END LOOP;
          Commit;
          v_endtime        := SYSDATE;
          v_Messagesfailed := v_Messagesfailed;
          v_Error          := SQLERRM;
          v_Reason         := 'Failed Procedure TxnDetailWrk6Bkp: ';
          v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                              Chr(10) || '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                              Chr(10) ||
                              '    <proc>TxnDetailWrk6Bkp</proc>' ||
                              Chr(10) || '  </details>' || Chr(10) ||
                              '</failed>';

          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => null,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

          --           IF v_Messagesreceived = 0 AND v_Messagesfailed > 0  then  --FATAL ERROR
          if v_email_flag = 0 then
            cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                          p_from_replyto  => 'ae_jobs@brierley.com',
                          p_to_list       => p_email_list,
                          p_cc_list       => NULL,
                          p_bcc_list      => NULL,
                          p_subject       => v_envkey ||
                                             ' American Eagle Tlog Chunk6 txncopy failure DML ERROR ' ||
                                             sysdate,
                          p_text_message  => v_message,
                          p_content_type  => 'text/html;charset=UTF8',
                          p_attachments   => v_attachments, --v_attachments,
                          p_priority      => '3',
                          p_auth_username => NULL,
                          p_auth_password => NULL,
                          p_mail_server   => 'cypwebmail.brierleyweb.com');

            v_email_flag := v_email_flag + 1;
          end if;
      END;
      --  v_messagesreceived := v_messagesreceived + sql%rowcount;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
    --   COMMIT;
    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;
    --   COMMIT;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => null,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    --        open retval for select v_dap_log_id from dual;

  EXCEPTION
    --AEO 654 changes END HERE------------------SCJ
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
                   '    <pkg>STAGE_TXNCOPY6</pkg>' || Chr(10) ||
                   '    <proc>TxnDetailWrk6Bkp</proc>' || Chr(10) ||
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
      cio_mail.send(p_from_email    => 'AEJobs@brierley.com',
                    p_from_replyto  => 'ae_jobs@brierley.com',
                    p_to_list       => p_email_list,
                    p_cc_list       => NULL,
                    p_bcc_list      => NULL,
                    p_subject       => v_envkey ||
                                       ' American Eagle Tlog Chunk6 txncopy failure on ' ||
                                       sysdate,
                    p_text_message  => v_message,
                    p_content_type  => 'text/html;charset=UTF8',
                    p_attachments   => v_attachments, --v_attachments,
                    p_priority      => '3',
                    p_auth_username => NULL,
                    p_auth_password => NULL,
                    p_mail_server   => 'cypwebmail.brierleyweb.com');
      Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');

  END;
END TxnDetailWrk6Bkp;

/*Redesign Changes start here --------------------------------------------------------------SCJ */

PROCEDURE ReturnRedeemTxnPoints(p_Processdate varchar2,p_email_list VARCHAR2,retval IN OUT rcursor)  AS
   v_attachments cio_mail.attachment_tbl_type;
    dml_errors  EXCEPTION;  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
     v_Processdate       timestamp:=to_date(p_Processdate, 'YYYYMMddHH24miss');
  --log job attributes
 v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;
  v_jobdirection          NUMBER:=0;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);
  v_processId             NUMBER:=0;
  v_Errormessage          VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256):= ' ' ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  v_recordcount        NUMBER :=0;
  v_cnt             NUMBER := 0;

BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'TLog6-ReturnRedeemTxnPoints';
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

utility_pkg.Log_Process_Start(v_jobname, 'ReturnRedeemTxnPoints wk6', v_processId);
  DECLARE
      CURSOR get_retdtls IS
                     select max(hw.txnheaderid) as txnheaderid,
                            max(hw.vckey) as vckey,
                            max(hw.rowkey) as rowkey,
                            max(hw.txndate) as txndate,
                            count(distinct(dw.rowkey)) as ct_dtlretitem,
                            count(distinct(dt.a_rowkey)) as max_orgitems,
                            sum(dt.a_dtlsaleamount) as org_purchaseamt,
                            ht.a_txnheaderid as org_txnheaderid
                       from lw_txnheader_wrk6 hw
                       inner join lw_txndetailitem_wrk6 dw
                         on hw.txnheaderid = dw.txnheaderid
                      inner join ats_txnheader ht
                         on ht.a_rowkey = hw.txnoriginaltxnrowkey
                      inner join ats_txndetailitem dt
                         on dt.a_txnheaderid = ht.a_txnheaderid
                      inner join lw_virtualcard vc
                         on vc.vckey = ht.a_vckey
                      inner join ats_memberdetails md
                         on md.a_ipcode = vc.ipcode
                      Where 1 = 1
                        and hw.txntypeid = 2
                        and hw.txnoriginaltxnrowkey is not null
                        and ae_isinpilot(md.a_extendedplaycode) = 1 /* Only redesign/Pilot members */
                        and NVL(dt.a_dtlsaleamount,0) > 0 /* excludes free items and returns*/
                        and dt.a_dtltypeid in (1,4)       /* only purchases and no Giftcards */
                        and dt.a_dtlclasscode <> '9911'  /* only original purchases and no Giftcards */
                        and dw.dtltypeid = 2             /* only returned items are considered*/
                      group by ht.a_txnheaderid;

         TYPE t_tab IS TABLE OF get_retdtls%ROWTYPE;
          v_tbl t_tab;
          v_CtRwdRedeem           NUMBER := 0;
          v_ReIssueAmt            NUMBER := 0;
          v_ReIssueAmtCtr         NUMBER := 0;
          v_PilotRwdRate          NUMBER := 5;
          v_PilotRwdPoints        NUMBER := 1000;
          v_pointtypeid_redesign  NUMBER;
          v_pointeventid_redesign NUMBER;
    BEGIN
        SELECT pointtypeid INTO v_pointtypeid_redesign  FROM  lw_pointtype WHERE NAME = 'AEO Connected Points';
        SELECT pointeventid INTO v_pointeventid_redesign FROM  lw_pointevent pe WHERE NAME = 'Returned Reward Points';
     FOR y IN get_retdtls LOOP
       select count(trr.a_rowkey) into v_CtRwdRedeem from ats_txnrewardredeem trr where trr.a_txnheaderid = y.org_txnheaderid;
       If v_CtRwdRedeem = 1 then
           If y.ct_dtlretitem = y.max_orgitems then /*(if the returned items are not the same, then no points reissued */
             v_ReIssueAmt := 1000;
                        --return 1000 points  Returned Reward Points
              INSERT INTO lw_pointtransaction
              (Pointtransactionid,
               Vckey,
               Pointtypeid,
               Pointeventid,
               Transactiontype,
               Transactiondate,
               Pointawarddate,
               Points,
               Expirationdate,
               Notes,
               Ownertype,
               Ownerid,
               Rowkey,
               Parenttransactionid,
               Pointsconsumed,
               Pointsonhold,
               Ptlocationid,
               Ptchangedby,
               createdate,
               Expirationreason)
            VALUES
              (seq_pointtransactionid.nextval,
               y.vckey,
               v_pointtypeid_redesign /*Pointtypeid*/,
               v_pointeventid_redesign /*pointeventid*/,
               1 /*award points*/,
               y.txndate,
              -- SYSDATE,
               v_Processdate,
               v_ReIssueAmt, /*Points= 1000 here*/
               to_date('12/31/2199', 'mm/dd/yyyy'),
               'Returned Reward Points', /*Notes*/
               1,
               101,
               y.rowkey,
               -1,
               0 /*Pointsconsumed*/,
               0 /*Pointsonhold*/,
               NULL,
               'Tlog Txncopy' /*Ptchangedby*/,
               SYSDATE,
               NULL);
            COMMIT;
           End if;
       End if;

           If v_CtRwdRedeem > 1 then
             --
             /*Calculation Logic of v_ReIssueAmt
             v_ReIssueAmt = 1000
             Pilot_Reward_AMT = 5

               Each return item's reissuance Percentage(RetIRePct) = (detailitem Sale amount/ original txn'stotal purchase amt) *( #_Rewards_Redeemed * Pilot_Reward_AMT)
               Reissue Amount Counter(v_ReIssueAmtCtr) =  Sum(RetIRePct )/ Pilot_Reward_AMT
                v_ReIssueAmt = v_ReIssueAmtCtr * 1000

             */
                 select trunc(sum(RetIRePct) / (v_PilotRwdRate))
                   into v_ReIssueAmtCtr                                                     /*   (3*5)*1000 */
                   from (select abs(dw.dtlsaleamount),
                                ((abs(round(dw.dtlsaleamount,1)) / y.org_purchaseamt) * (v_CtRwdRedeem * v_PilotRwdRate)) as RetIRePct
                           from lw_txndetailitem_wrk6 dw
                          where dw.txnheaderid = y.txnheaderid);

                 If  v_ReIssueAmtCtr > 0 then
                           v_ReIssueAmt :=  (v_ReIssueAmtCtr * 1000);
                            -- return v_ReIssueAmt points as Returned Reward Points
                  INSERT INTO lw_pointtransaction
                  (Pointtransactionid,
                   Vckey,
                   Pointtypeid,
                   Pointeventid,
                   Transactiontype,
                   Transactiondate,
                   Pointawarddate,
                   Points,
                   Expirationdate,
                   Notes,
                   Ownertype,
                   Ownerid,
                   Rowkey,
                   Parenttransactionid,
                   Pointsconsumed,
                   Pointsonhold,
                   Ptlocationid,
                   Ptchangedby,
                   createdate,
                   Expirationreason)
                VALUES
                  (seq_pointtransactionid.nextval,
                   y.vckey,
                   v_pointtypeid_redesign /*Pointtypeid*/,
                   v_pointeventid_redesign /*pointeventid*/,
                   1 /*award points*/,
                   y.txndate,
                  -- SYSDATE,
                   v_Processdate,
                   v_ReIssueAmt, /*Points here*/
                   to_date('12/31/2199', 'mm/dd/yyyy'),
                   'Returned Reward Points', /*Notes*/
                   1,
                   101,
                   y.rowkey,
                   -1,
                   0 /*Pointsconsumed*/,
                   0 /*Pointsonhold*/,
                   NULL,
                   'Tlog Txncopy' /*Ptchangedby*/,
                   SYSDATE,
                   NULL);
                  COMMIT;
                 End if;
           End if;

          /* If  v_CtRwdRedeem = 0 then
             --no return of points
           End if;*/

      v_cnt        := v_cnt + 1;
      IF MOD(v_cnt, 1000) = 0 THEN
        COMMIT;
      END IF;
    END LOOP;
    COMMIT;
  --  END;
    IF get_retdtls%ISOPEN THEN
      CLOSE get_retdtls;
    END IF;
    COMMIT;
   v_endtime := SYSDATE;
  v_jobstatus := 1;
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);



    v_endtime := SYSDATE;
    v_Messagesfailed := v_Messagesfailed ;
    v_Error          := SQLERRM;
    v_Reason         := 'Failed Procedure ReturnRedeemTxnPoints: ';
    v_Message        := '<failed>' || Chr(10) ||
                        '  <details>' || Chr(10) ||
                        '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                        Chr(10) ||
                        '    <proc>ReturnRedeemTxnPoints</proc>' ||
                        Chr(10) || '  </details>' ||
                        Chr(10) || '</failed>';

    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => null,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);



EXCEPTION
WHEN OTHERS THEN
             v_Messagesfailed := v_Messagesfailed + 1;
            v_endtime := SYSDATE;
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                p_Jobdirection     => v_Jobdirection,
                                p_Filename         => null,
                                p_Starttime        => v_Starttime,
                                p_Endtime          => v_Endtime,
                                p_Messagesreceived => v_Messagesreceived,
                                p_Messagesfailed   => v_Messagesfailed,
                                p_Jobstatus        => v_Jobstatus,
                                p_Jobname          => v_Jobname);

           v_Error          := SQLERRM;
            v_Message        := '<failed>' || Chr(10) ||
                                  '  <details>' || Chr(10) ||
                                  '    <pkg>STAGE_TXNCOPY6</pkg>' ||
                                  Chr(10) ||
                                  '    <proc>ReturnRedeemTxnPoints</proc>' ||
                                  Chr(10) || '  </details>' ||
                                  Chr(10) || '</failed>';
           Utility_pkg.Log_msg(p_messageid         => v_messageid,
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
           cio_mail.send(
                p_from_email    => 'AEJobs@brierley.com',
                p_from_replyto  => 'ae_jobs@brierley.com',
                p_to_list       => p_email_list,
                p_cc_list       => NULL,
                p_bcc_list      => NULL,
                p_subject       =>  v_envkey ||' American Eagle Tlog Chunk6 txncopy failure on ' || sysdate  ,
                p_text_message  => v_message,
                p_content_type  => 'text/html;charset=UTF8',
                p_attachments   => v_attachments, --v_attachments,
                p_priority      => '3',
                p_auth_username => NULL,
                p_auth_password => NULL,
                p_mail_server   => 'cypwebmail.brierleyweb.com'
             );
           Raise_Application_Error(-20002, 'STAGE_TXNCOPY6 job is not finished');

   END;
END ReturnRedeemTxnPoints;

/*Redesign Changes end here --------------------------------------------------------------SCJ */
END STAGE_TXNCOPY6;
/
