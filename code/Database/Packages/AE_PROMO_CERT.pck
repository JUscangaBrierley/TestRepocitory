CREATE OR REPLACE PACKAGE AE_PROMO_CERT IS

  -- AUTHOR  : GDZUL
  -- CREATED : 11/08/2016 2:42:55 PM
  -- PURPOSE : Fill out the table lw_promotioncertificate
  -- PROCEDURE DECLARATION
  
  PROCEDURE PROMO_CERT(P_TYPE_CODE IN VARCHAR2);

END AE_PROMO_CERT;
/

CREATE OR REPLACE PACKAGE BODY AE_PROMO_CERT IS

PROCEDURE PROMO_CERT(P_TYPE_CODE IN VARCHAR2) IS
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

         --Rewards
         CURSOR get_data IS
         select rb.a_barcode,
                rb.a_typecode,
                rb.a_effectivestartdate,
                rb.a_effectiveenddate,
                rb.createdate,
                rb.updatedate
           from bp_ae.Ats_Rewardbarcodes RB
           left join bp_ae.lw_promotioncertificate pc on pc.certnmbr = rb.a_barcode
          WHERE RB.A_EFFECTIVESTARTDATE > SYSDATE -10
          and rb.a_typecode = P_TYPE_CODE
            and rb.a_status = 0 and pc.certnmbr is null ;

         TYPE t_tab IS TABLE OF get_data%ROWTYPE;
         v_tbl t_tab; 
         
         v_attachments cio_mail.attachment_tbl_type;
         lv_email_body CLOB;
      
BEGIN 
         v_My_Log_Id := Utility_Pkg.Get_Libjobid();
         
        /*Logging*/
        v_my_log_id := utility_pkg.get_LIBJobID();
        v_jobname := 'AE_PROMO_CERT';
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

       utility_pkg.Log_Process_Start(v_jobname, 'AE_PROMO_CERT', v_processId);
           
        --Active 'Non-validated' members with points 1000+
        OPEN get_data;
        LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;                              
              FORALL i IN 1 .. v_tbl.COUNT
                insert into bp_ae.lw_promotioncertificate
                  (certnmbr,
                   typecode,
                   objecttype,
                   available,
                   startdate,
                   expirydate,
                   createdate,
                   updatedate)
                values
                  (v_tbl (i).a_barcode,
                   v_tbl (i).a_typecode,
                   'Reward',
                   1,
                   v_tbl (i).a_effectivestartdate,
                   v_tbl (i).a_effectiveenddate,
                   v_tbl (i).createdate,
                   v_tbl (i).updatedate);
                   
              COMMIT;
              EXIT WHEN get_data%NOTFOUND;       

         END LOOP;
         CLOSE get_data;
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
                              P_LOGSOURCE => 'AE_PROMO_CERT.PROMO_CERT',
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

END PROMO_CERT;

END AE_PROMO_CERT;
/
