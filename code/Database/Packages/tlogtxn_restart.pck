CREATE OR REPLACE PACKAGE TLOGTXN_Restart IS
type rcursor IS REF CURSOR;
  /*
PI29741 tlog optimization changes begin here                 -----scj
*/

      PROCEDURE TxnHeaderRules_Restart(p_Dummy VARCHAR2,Chunknum number,retval IN OUT rcursor);
      PROCEDURE TxnDetailitemRules_Restart(p_Dummy VARCHAR2,Chunknum number,retval IN OUT rcursor);

END TLOGTXN_Restart;
/

CREATE OR REPLACE PACKAGE BODY TLOGTXN_Restart   IS

PROCEDURE TxnHeaderRules_Restart(p_Dummy VARCHAR2,Chunknum number,retval IN OUT rcursor) AS
     dml_errors  EXCEPTION;  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
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

--BEGIN
BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'TXN_HeaderRules'||Chunknum||'_Restart';
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

utility_pkg.Log_Process_Start(v_jobname, 'TxnHeader'||Chunknum||'Restart', v_processId);
  DECLARE
      CURSOR get_data IS
          WITH Mem AS
           (
          select hw.header_rowkey from lw_txnheader_wrk hw
          inner join lw_pointtransaction pt on pt.rowkey = hw.header_rowkey
          union
          select hw.header_rowkey from lw_txnheader_wrk hw
          inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
          )
          SELECT * FROM Mem;
        TYPE t_tab IS TABLE OF get_data%ROWTYPE;
        v_tbl t_tab;
        CURSOR get_data2 IS
      WITH Mem AS
       (
      select hw.header_rowkey from lw_txnheader_wrk2 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.header_rowkey
      union
      select hw.header_rowkey from lw_txnheader_wrk2 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab2 IS TABLE OF get_data2%ROWTYPE;
    v_tbl2 t_tab2;
    CURSOR get_data3 IS
      WITH Mem AS
       (
      select hw.header_rowkey from lw_txnheader_wrk3 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.header_rowkey
      union
      select hw.header_rowkey from lw_txnheader_wrk3 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab3 IS TABLE OF get_data3%ROWTYPE;
    v_tbl3 t_tab3;
    CURSOR get_data4 IS
      WITH Mem AS
       (
      select hw.header_rowkey from lw_txnheader_wrk4 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.header_rowkey
      union
      select hw.header_rowkey from lw_txnheader_wrk4 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab4 IS TABLE OF get_data4%ROWTYPE;
    v_tbl4 t_tab4;
    CURSOR get_data5 IS
      WITH Mem AS
       (
      select hw.header_rowkey from lw_txnheader_wrk5 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.header_rowkey
      union
      select hw.header_rowkey from lw_txnheader_wrk5 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab5 IS TABLE OF get_data5%ROWTYPE;
    v_tbl5 t_tab5;
    CURSOR get_data6 IS
      WITH Mem AS
       (
      select hw.header_rowkey from lw_txnheader_wrk6 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.header_rowkey
      union
      select hw.header_rowkey from lw_txnheader_wrk6 hw
      inner join lw_pointtransaction pt on pt.rowkey = hw.txnoriginaltxnrowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab6 IS TABLE OF get_data6%ROWTYPE;
    v_tbl6 t_tab6;
  BEGIN
    IF Chunknum = 1 THEN /*CHUNK1 */
       OPEN get_data;
         LOOP
            FETCH get_data BULK COLLECT
              INTO v_tbl LIMIT 10000;
            FORALL j IN 1 .. v_tbl.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNHEADER_WRK HW WHERE  HW.HEADER_ROWKEY = v_tbl(j).header_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data%ISOPEN THEN
          CLOSE get_data;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 2 THEN /*CHUNK2 */
       OPEN get_data2;
         LOOP
            FETCH get_data2 BULK COLLECT
              INTO v_tbl2 LIMIT 10000;
            FORALL j IN 1 .. v_tbl2.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNHEADER_WRK2 HW WHERE  HW.HEADER_ROWKEY = v_tbl2(j).header_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data2%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data2%ISOPEN THEN
          CLOSE get_data2;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 3 THEN /*CHUNK3 */
       OPEN get_data3;
         LOOP
            FETCH get_data3 BULK COLLECT
              INTO v_tbl3 LIMIT 10000;
            FORALL j IN 1 .. v_tbl3.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNHEADER_WRK3 HW WHERE  HW.HEADER_ROWKEY = v_tbl3(j).header_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data3%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data3%ISOPEN THEN
          CLOSE get_data3;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 4 THEN /*CHUNK4 */
       OPEN get_data4;
         LOOP
            FETCH get_data4 BULK COLLECT
              INTO v_tbl4 LIMIT 10000;
            FORALL j IN 1 .. v_tbl4.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNHEADER_WRK4 HW WHERE  HW.HEADER_ROWKEY = v_tbl4(j).header_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data4%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data4%ISOPEN THEN
          CLOSE get_data4;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 5 THEN /*CHUNK5 */
       OPEN get_data5;
         LOOP
            FETCH get_data5 BULK COLLECT
              INTO v_tbl5 LIMIT 10000;
            FORALL j IN 1 .. v_tbl5.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNHEADER_WRK5 HW WHERE  HW.HEADER_ROWKEY = v_tbl5(j).header_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data5%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data5%ISOPEN THEN
          CLOSE get_data5;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 6 THEN /*CHUNK6 */
       OPEN get_data6;
         LOOP
            FETCH get_data6 BULK COLLECT
              INTO v_tbl6 LIMIT 10000;
            FORALL j IN 1 .. v_tbl6.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNHEADER_WRK6 HW WHERE  HW.HEADER_ROWKEY = v_tbl6(j).header_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data6%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data6%ISOPEN THEN
          CLOSE get_data6;
       END IF;
       COMMIT;
    END IF;

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

  --       open retval for select v_dap_log_id from dual;

EXCEPTION
   WHEN dml_errors THEN
 -- IF v_messagesfailed = 0 THEN
 --   v_messagesfailed := 0+1;
 -- END IF;

  FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT
      LOOP
         v_Messagesfailed := v_Messagesfailed + 1;
         v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE);
          v_Reason         := 'Failed Records RestartProcedure TxnHeaderRules_Restart'||Chunknum||':';
         v_Message        :=  ' ';

         Utility_pkg.Log_msg(p_messageid  => v_messageid,
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

      END LOOP;

      v_Messagesfailed := v_Messagesfailed + 1;
    v_Error          := SQLERRM;
    v_Reason         := 'Failed Procedure TxnHeaderRules_Restart'||Chunknum||': ';
    v_Message        := '<failed>' || Chr(10) ||
                        '  <details>' || Chr(10) ||
                        '    <pkg>TLOGTXN_Restart</pkg>' ||
                        Chr(10) ||
                        '    <proc>TxnHeaderRules_Restart'||Chunknum||'</proc>' ||
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

          Raise_Application_Error(-20002, 'TLOGTXN_Restart job is not finished');

 WHEN OTHERS THEN
             v_Messagesfailed := v_Messagesfailed + 1;

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
            v_Reason         := 'Failed Procedure TxnHeaderRules_Restart'||Chunknum||': ';
            v_Message        := '<failed>' || Chr(10) ||
                        '  <details>' || Chr(10) ||
                        '    <pkg>TLOGTXN_Restart</pkg>' ||
                        Chr(10) ||
                        '    <proc>TxnHeaderRules_Restart'||Chunknum||'</proc>' ||
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

           Raise_Application_Error(-20002, 'TLOGTXN_Restart job is not finished');
   END;


END TxnHeaderRules_Restart;

PROCEDURE TxnDetailitemRules_Restart(p_Dummy VARCHAR2,Chunknum number,retval IN OUT rcursor) AS
     dml_errors  EXCEPTION;  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
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

--BEGIN
BEGIN
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'TXN_DetailitemRules'||Chunknum||'_Restart';
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

utility_pkg.Log_Process_Start(v_jobname, 'TxnDetailitem'||Chunknum||'Restart', v_processId);
  DECLARE
      CURSOR get_data IS
          WITH Mem AS
           (
           select dw.dtl_rowkey from lw_txndetailitem_wrk dw
           inner join lw_pointtransaction pt on pt.rowkey = dw.dtl_rowkey
          )
          SELECT * FROM Mem;
        TYPE t_tab IS TABLE OF get_data%ROWTYPE;
        v_tbl t_tab;
        CURSOR get_data2 IS
      WITH Mem AS
       (
     select dw.dtl_rowkey from lw_txndetailitem_wrk2 dw
           inner join lw_pointtransaction pt on pt.rowkey = dw.dtl_rowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab2 IS TABLE OF get_data2%ROWTYPE;
    v_tbl2 t_tab2;
    CURSOR get_data3 IS
      WITH Mem AS
       (
      select dw.dtl_rowkey from lw_txndetailitem_wrk3 dw
           inner join lw_pointtransaction pt on pt.rowkey = dw.dtl_rowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab3 IS TABLE OF get_data3%ROWTYPE;
    v_tbl3 t_tab3;
    CURSOR get_data4 IS
      WITH Mem AS
       (
      select dw.dtl_rowkey from lw_txndetailitem_wrk4 dw
           inner join lw_pointtransaction pt on pt.rowkey = dw.dtl_rowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab4 IS TABLE OF get_data4%ROWTYPE;
    v_tbl4 t_tab4;
    CURSOR get_data5 IS
      WITH Mem AS
       (
      select dw.dtl_rowkey from lw_txndetailitem_wrk5 dw
           inner join lw_pointtransaction pt on pt.rowkey = dw.dtl_rowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab5 IS TABLE OF get_data5%ROWTYPE;
    v_tbl5 t_tab5;
    CURSOR get_data6 IS
      WITH Mem AS
       (
      select dw.dtl_rowkey from lw_txndetailitem_wrk6 dw
           inner join lw_pointtransaction pt on pt.rowkey = dw.dtl_rowkey
      )
      SELECT * FROM Mem;
    TYPE t_tab6 IS TABLE OF get_data6%ROWTYPE;
    v_tbl6 t_tab6;
  BEGIN
    IF Chunknum = 1 THEN /*CHUNK1 */
       OPEN get_data;
         LOOP
            FETCH get_data BULK COLLECT
              INTO v_tbl LIMIT 10000;
            FORALL j IN 1 .. v_tbl.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNDETAILITEM_WRK DW WHERE  DW.DTL_ROWKEY = v_tbl(j).dtl_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data%ISOPEN THEN
          CLOSE get_data;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 2 THEN /*CHUNK2 */
       OPEN get_data2;
         LOOP
            FETCH get_data2 BULK COLLECT
              INTO v_tbl2 LIMIT 10000;
            FORALL j IN 1 .. v_tbl2.COUNT  SAVE EXCEPTIONS
               DELETE  from LW_TXNDETAILITEM_WRK2 DW WHERE  DW.DTL_ROWKEY = v_tbl2(j).dtl_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data2%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data2%ISOPEN THEN
          CLOSE get_data2;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 3 THEN /*CHUNK3 */
       OPEN get_data3;
         LOOP
            FETCH get_data3 BULK COLLECT
              INTO v_tbl3 LIMIT 10000;
            FORALL j IN 1 .. v_tbl3.COUNT  SAVE EXCEPTIONS
              DELETE  from LW_TXNDETAILITEM_WRK3 DW WHERE  DW.DTL_ROWKEY = v_tbl3(j).dtl_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data3%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data3%ISOPEN THEN
          CLOSE get_data3;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 4 THEN /*CHUNK4 */
       OPEN get_data4;
         LOOP
            FETCH get_data4 BULK COLLECT
              INTO v_tbl4 LIMIT 10000;
            FORALL j IN 1 .. v_tbl4.COUNT  SAVE EXCEPTIONS
               DELETE  from LW_TXNDETAILITEM_WRK4 DW WHERE  DW.DTL_ROWKEY = v_tbl4(j).dtl_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data4%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data4%ISOPEN THEN
          CLOSE get_data4;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 5 THEN /*CHUNK5 */
       OPEN get_data5;
         LOOP
            FETCH get_data5 BULK COLLECT
              INTO v_tbl5 LIMIT 10000;
            FORALL j IN 1 .. v_tbl5.COUNT  SAVE EXCEPTIONS
               DELETE  from LW_TXNDETAILITEM_WRK5 DW WHERE  DW.DTL_ROWKEY = v_tbl5(j).dtl_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data5%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data5%ISOPEN THEN
          CLOSE get_data5;
       END IF;
       COMMIT;
    END IF;
    IF Chunknum = 6 THEN /*CHUNK6 */
       OPEN get_data6;
         LOOP
            FETCH get_data6 BULK COLLECT
              INTO v_tbl6 LIMIT 10000;
            FORALL j IN 1 .. v_tbl6.COUNT  SAVE EXCEPTIONS
               DELETE  from LW_TXNDETAILITEM_WRK6 DW WHERE  DW.DTL_ROWKEY = v_tbl6(j).dtl_rowkey;
              v_messagesreceived := v_messagesreceived + sql%rowcount;
            COMMIT;
            EXIT WHEN get_data6%NOTFOUND;
         END LOOP;
       COMMIT;
       IF get_data6%ISOPEN THEN
          CLOSE get_data6;
       END IF;
       COMMIT;
    END IF;

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

  --       open retval for select v_dap_log_id from dual;

EXCEPTION
   WHEN dml_errors THEN
 -- IF v_messagesfailed = 0 THEN
 --   v_messagesfailed := 0+1;
 -- END IF;

  FOR indx IN 1 .. sql%BULK_EXCEPTIONS.COUNT
      LOOP
         v_Messagesfailed := v_Messagesfailed + 1;
         v_Error          := SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE);
          v_Reason         := 'Failed Records RestartProcedure TxnDetailitemRules_Restart'||Chunknum||':';
         v_Message        :=  ' ';

         Utility_pkg.Log_msg(p_messageid  => v_messageid,
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

      END LOOP;

      v_Messagesfailed := v_Messagesfailed + 1;
    v_Error          := SQLERRM;
    v_Reason         := 'Failed Procedure TxnDetailitemRules_Restart'||Chunknum||': ';
    v_Message        := '<failed>' || Chr(10) ||
                        '  <details>' || Chr(10) ||
                        '    <pkg>TLOGTXN_Restart</pkg>' ||
                        Chr(10) ||
                        '    <proc>TxnDetailitemRules_Restart'||Chunknum||'</proc>' ||
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

          Raise_Application_Error(-20002, 'TLOGTXN_Restart job is not finished');

 WHEN OTHERS THEN
             v_Messagesfailed := v_Messagesfailed + 1;

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
            v_Reason         := 'Failed Procedure TxnDetailitemRules_Restart'||Chunknum||': ';
            v_Message        := '<failed>' || Chr(10) ||
                        '  <details>' || Chr(10) ||
                        '    <pkg>TLOGTXN_Restart</pkg>' ||
                        Chr(10) ||
                        '    <proc>TxnDetailitemRules_Restart'||Chunknum||'</proc>' ||
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

           Raise_Application_Error(-20002, 'TLOGTXN_Restart job is not finished');
   END;


END TxnDetailitemRules_Restart;

END TLOGTXN_Restart;
/

