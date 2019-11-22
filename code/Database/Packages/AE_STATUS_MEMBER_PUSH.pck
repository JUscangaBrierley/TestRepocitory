CREATE OR REPLACE PACKAGE AE_STATUS_MEMBER IS

  -- AUTHOR  : GDZUL
  -- CREATED : 11/04/2016 2:42:55 PM
  -- PURPOSE : Lock 'Non-Validated' Members when the member has 1000+ or unlock if the member has 1000-
  -- PROCEDURE DECLARATION
  
  PROCEDURE STATUS_MEMBER;

END AE_STATUS_MEMBER;
/
CREATE OR REPLACE PACKAGE BODY AE_STATUS_MEMBER IS

PROCEDURE STATUS_MEMBER IS
            --log job attributes
            V_MY_LOG_ID        NUMBER := 0;
            V_JOBDIRECTION     NUMBER := 0;
            V_STARTTIME        DATE := SYSDATE;
            V_ENDTIME          DATE;
            V_MESSAGESRECEIVED NUMBER := 0;
            V_MESSAGESFAILED   NUMBER := 0;
            V_JOBSTATUS        NUMBER := 0;
            V_JOBNAME          VARCHAR2(256);
            V_PROCESSID        NUMBER := 0;
            V_ERRORMESSAGE     VARCHAR2(256);

            --log msg attributes
            V_MESSAGEID   VARCHAR2(256);
            V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                           UPPER(SYS_CONTEXT('userenv',
                                                             'instance_name'));
            V_LOGSOURCE   VARCHAR2(256);
            V_BATCHID     VARCHAR2(256) := 0;
            V_MESSAGE     VARCHAR2(256) := ' ';
            V_REASON      VARCHAR2(256);
            V_ERROR       VARCHAR2(256);
            V_TRYCOUNT    NUMBER := 0;
            V_RECORDCOUNT NUMBER := 0;  

         --Get Active 'Non-validated' members with points 1000+
         CURSOR get_data1 IS
          SELECT vc.ipcode AS IPCODE
          FROM bp_ae.lw_pointtransaction pt
          INNER JOIN bp_ae.lw_pointevent pe ON pt.pointeventid = pe.pointeventid
          INNER JOIN bp_ae.lw_pointtype p ON pt.pointtypeid = p.pointtypeid
          INNER JOIN bp_ae.lw_virtualcard vc ON pt.vckey = vc.vckey
          INNER JOIN bp_ae.ats_memberdetails md ON vc.ipcode = md.a_ipcode
          INNER JOIN bp_ae.Lw_Loyaltymember lm ON vc.ipcode = lm.ipcode
          WHERE 1 = 1
          AND upper(trim(p.name)) IN ('AEO CONNECTED POINTS','AEO CONNECTED BONUS POINTS', 'ADJUSTMENT POINTS', 'AEO CUSTOMER SERVICE POINTS', 'ADJUSTMENT BONUS POINTS', 'AEO VISA CARD POINTS')
          AND ((pt.transactiondate  Between to_date('10/1/2015', 'mm/dd/yyyy') AND (SYSDATE -13) AND pt.transactiontype = 1) OR (pt.transactiontype = 2))
          AND (md.a_Extendedplaycode IN (1,3)) -- AEO-Point Conversion
          AND pt.pointsconsumed = 0
          AND pt.transactiontype <> 4
          AND pt.expirationdate > SYSDATE
          AND nvl(md.a_employeecode,0) != 1
          AND lm.memberstatus = 1
          AND ((nvl(md.a_pendingemailverification, 1) = 1)) AND (nvl(md.a_pendingcellverification, 1) = 1) --‘Non-Validated’ members
          AND md.a_emailaddress is not null
          GROUP BY vc.ipcode
          HAVING SUM (pt.points) >= 1000
          ;

         TYPE t_tab1 IS TABLE OF get_data1%ROWTYPE;
         v_tbl1 t_tab1; 
         
         --Get Locked members with points 1000-
         CURSOR get_data2 IS
          SELECT vc.ipcode AS IPCODE
          FROM bp_ae.lw_pointtransaction pt
          INNER JOIN bp_ae.lw_pointevent pe ON pt.pointeventid = pe.pointeventid
          INNER JOIN bp_ae.lw_pointtype p ON pt.pointtypeid = p.pointtypeid
          INNER JOIN bp_ae.lw_virtualcard vc ON pt.vckey = vc.vckey
          INNER JOIN bp_ae.ats_memberdetails md ON vc.ipcode = md.a_ipcode
          INNER JOIN bp_ae.Lw_Loyaltymember lm ON vc.ipcode = lm.ipcode
          WHERE 1 = 1
          AND upper(trim(p.name)) IN ('AEO CONNECTED POINTS','AEO CONNECTED BONUS POINTS', 'ADJUSTMENT POINTS', 'AEO CUSTOMER SERVICE POINTS', 'ADJUSTMENT BONUS POINTS', 'AEO VISA CARD POINTS')
          AND ((pt.transactiondate  Between to_date('10/1/2015', 'mm/dd/yyyy') AND (SYSDATE -13) AND pt.transactiontype = 1) OR (pt.transactiontype = 2))
          AND (md.a_Extendedplaycode IN (1,3)) -- AEO-Point Conversion
          AND pt.pointsconsumed = 0
          AND pt.transactiontype <> 4
          AND pt.expirationdate > SYSDATE
          AND nvl(md.a_employeecode,0) != 1
          AND lm.memberstatus = 4
          GROUP BY vc.ipcode
          HAVING SUM (pt.points) < 1000
          ;

         TYPE t_tab2 IS TABLE OF get_data2%ROWTYPE;
         v_tbl2 t_tab2;
                  
         v_attachments cio_mail.attachment_tbl_type;
         lv_email_body CLOB;
      
BEGIN 
         v_My_Log_Id := Utility_Pkg.Get_Libjobid();
         
        /*Logging*/
        v_my_log_id := utility_pkg.get_LIBJobID();
        v_jobname := 'AE_STATUS_MEMBER';
        v_logsource := v_jobname;

        /* log start of job */
        utility_pkg.Log_job(P_JOB    => v_my_log_id
               ,p_jobdirection       => v_jobdirection
               ,p_filename           => null
               ,p_starttime          => v_starttime
               ,p_endtime            => v_endtime
               ,p_messagesreceived   => v_messagesreceived
               ,p_messagesfailed     => v_messagesfailed
               ,p_jobstatus          => v_jobstatus
               ,p_jobname            => v_jobname);

       utility_pkg.Log_Process_Start(v_jobname, 'AE_STATUS_MEMBER', v_processId);
           
        --Active 'Non-validated' members with points 1000+
        OPEN get_data1;
        LOOP
        FETCH get_data1 BULK COLLECT INTO v_tbl1 LIMIT 1000;                              
              FORALL j IN 1 .. v_tbl1.COUNT
                   UPDATE bp_ae.lw_loyaltymember lm
                   SET lm.memberstatus = 4
                   WHERE lm.ipcode = v_tbl1(j).IPCODE
                   ;
                   
              COMMIT;
              EXIT WHEN get_data1%NOTFOUND;       

         END LOOP;
         CLOSE get_data1;
         COMMIT;
         
        --Locked members with points 1000-
        OPEN get_data2;
        LOOP
        FETCH get_data2 BULK COLLECT INTO v_tbl2 LIMIT 1000;                              
              FORALL j IN 1 .. v_tbl2.COUNT
                   UPDATE bp_ae.lw_loyaltymember lm
                   SET lm.memberstatus = 1
                   WHERE lm.ipcode = v_tbl2(j).IPCODE
                   ;
                   
              COMMIT;
              EXIT WHEN get_data2%NOTFOUND;       

         END LOOP;
         CLOSE get_data2;
         COMMIT;
                      
        -- Save logs
        v_endtime := SYSDATE;
        v_jobstatus := 1;
        utility_pkg.Log_job(P_JOB    => v_my_log_id
               ,p_jobdirection       => v_jobdirection
               ,p_filename           => null
               ,p_starttime          => v_starttime
               ,p_endtime            => v_endtime
               ,p_messagesreceived   => v_messagesreceived
               ,p_messagesfailed     => v_messagesfailed
               ,p_jobstatus          => v_jobstatus
               ,p_jobname            => v_jobname);
               
        EXCEPTION 
          WHEN OTHERS THEN   
             V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
             v_endtime := SYSDATE;
             utility_pkg.Log_job(P_JOB => v_my_log_id
             ,p_jobdirection       => v_jobdirection
             ,p_filename           => null
             ,p_starttime          => v_starttime
             ,p_endtime            => v_endtime
             ,p_messagesreceived   => v_messagesreceived
             ,p_messagesfailed     => v_messagesfailed
             ,p_jobstatus          => v_jobstatus
             ,p_jobname            => v_jobname);
             V_ERROR := SQLERRM;
             UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                              P_ENVKEY    => V_ENVKEY,
                              P_LOGSOURCE => 'AE_STATUS_MEMBER.STATUS_MEMBER',
                              P_FILENAME  => NULL,
                              P_BATCHID   => V_BATCHID,
                              P_JOBNUMBER => V_MY_LOG_ID,
                              P_MESSAGE   => V_MESSAGE,
                              P_REASON    => V_REASON,
                              P_ERROR     => V_ERROR,
                              P_TRYCOUNT  => V_TRYCOUNT,
                              P_MSGTIME   => SYSDATE); 
        ROLLBACK;
        RAISE;

END STATUS_MEMBER;

END AE_STATUS_MEMBER;
/
