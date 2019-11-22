CREATE OR REPLACE PACKAGE STAGE_COUPONS IS
type rcursor IS REF CURSOR;

    PROCEDURE Stage_SingleUse (p_SingleUseFileName  IN VARCHAR2, retval IN OUT rcursor);
    PROCEDURE Stage_ShortCode (p_ShortCodeFileName  IN VARCHAR2, retval IN OUT rcursor);
    PROCEDURE Update_ShortCode (p_Shortcode IN VARCHAR2, p_TypeCode IN VARCHAR2, retval IN OUT rcursor);  --AEO-987 AH
END STAGE_COUPONS;
/
CREATE OR REPLACE PACKAGE BODY STAGE_COUPONS   IS
 /********* Procedure to map external table to specified file ********/
PROCEDURE ChangeExternalTable(p_ExtTableName IN VARCHAR2,
                              p_FileName     IN VARCHAR2)
IS
  e_MTable exception;
  e_MFileName exception;
  v_sql VARCHAR2(400);
BEGIN

    IF LENGTH(TRIM(p_ExtTableName))=0 OR p_ExtTableName is NULL THEN
      raise_application_error(-20000, 'External tablename is required', FALSE);
    ELSIF LENGTH(TRIM(p_FileName))=0 OR p_FileName is NULL THEN
      raise_application_error(-20001, 'Filename is required to link with external table', FALSE);
    END IF;

    v_sql := 'ALTER TABLE '||p_ExtTableName||' LOCATION (AE_IN'||CHR(58)||''''||p_FileName||''')';
    EXECUTE IMMEDIATE v_sql;

END ChangeExternalTable;

 /********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/


  PROCEDURE Stage_SingleUse (p_SingleUseFileName  IN VARCHAR2, retval          IN OUT rcursor)
IS
 v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;

  --log job attributes
  v_jobdirection          NUMBER:=0;
  v_filename              VARCHAR2(512):=p_SingleUseFileName;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);


  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='BP_AE@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_filename           VARCHAR2(256):=p_SingleUseFileName ;
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256) ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  BEGIN

    v_my_log_id := utility_pkg.get_LIBJobID();
     v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'SingleUseLoad';
     v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_SingleUseFileName
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'Stage-'||v_jobname);


ChangeExternalTable(p_ExtTableName => 'EXT_DMSSINGLEUSECOUPON',p_FileName => p_SingleUseFileName);


    ---------------------------
    -- Load Single Use Coupon
    ---------------------------
    DECLARE
      CURSOR get_data IS
 
      SELECT substr(shortcd, 1, 3) as shortcode, suc
          from ext_dmssingleusecoupon sc
          LEFT JOIN ats_rewardbarcodes rb ON rb.a_barcode = sc.suc
          WHERE 1=1
          AND rb.a_barcode IS NULL;    --<----AH - AEO 782: filter barcodes that have values already in ats_rewardbarcodes

      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop

          insert into ats_rewardbarcodes (a_rowkey, a_ipcode, a_parentrowkey, a_effectivestartdate, a_typecode, a_barcode, a_status, a_effectiveenddate, statuscode, createdate, updatedate)
          values (seq_rowkey.nextval, -1, -1, null, v_tbl(i).shortcode, v_tbl(i).suc, 0, null, 0, sysdate, sysdate);

        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
   END;

  /* insert here */
  v_endtime := SYSDATE;
  v_jobstatus := 1;

  /* log end of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_SingleUseFileName
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'Stage-'||v_jobname);

         open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN

   utility_pkg.Log_msg(p_messageid         => v_messageid,
           p_envkey            => v_envkey   ,
           p_logsource         => v_logsource,
           p_filename          => p_SingleUseFileName ,
           p_batchid           => v_batchid  ,
           p_jobnumber         => v_my_log_id,
           p_message           => v_message  ,
           p_reason            => v_reason   ,
           p_error             => v_error    ,
           p_trycount          => v_trycount ,
           p_msgtime           => SYSDATE  );
  RAISE;

  END Stage_SingleUse;

 /********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/


  PROCEDURE Stage_ShortCode (p_ShortCodeFileName  IN VARCHAR2, retval          IN OUT rcursor)
IS
 v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;

  --log job attributes
  v_jobdirection          NUMBER:=0;
  v_filename              VARCHAR2(512):=p_ShortCodeFileName;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='BP_AE@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_filename           VARCHAR2(256):=p_ShortCodeFileName ;
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256) ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  BEGIN

    v_my_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'ShortCodeLoad';
     v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_ShortCodeFileName
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

ChangeExternalTable(p_ExtTableName => 'EXT_DMSSHORTCODE',p_FileName => p_ShortCodeFileName);



/* Insert data into ATS_REWARDSHORTCODE */
insert into ats_rewardshortcode(a_rowkey, lwidentifier, a_ipcode, a_parentrowkey, a_shortcode, a_effectivestartdate, a_effectiveenddate, a_description, a_typecode, statuscode, createdate, updatedate)
Select seq_rowkey.nextval, 0, 0, 0, a.shortcd, TO_DATE(a.startdt,'MM/dd/YYYY'), TO_DATE(a.enddt,'MM/dd/YYYY'), a.description, replace(a.typecode, chr(13)), 0, SYSDATE, SYSDATE
from ext_dmsshortcode a 
LEFT JOIN bp_ae.ats_rewardshortcode rsc ON rsc.a_shortcode = a.shortcd
where a.shortcd != 'AUTH_CD'
AND rsc.a_shortcode IS NULL;        --<----AH - AEO 782: Filter short codes already in RewardShortCode table
COMMIT;



  /* insert here */
  v_endtime := SYSDATE;
  v_jobstatus := 1;

  /* log end of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_ShortCodeFileName
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'Stage-'||v_jobname);

         open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN

   utility_pkg.Log_msg(p_messageid         => v_messageid,
           p_envkey            => v_envkey   ,
           p_logsource         => v_logsource,
           p_filename          => p_ShortCodeFileName ,
           p_batchid           => v_batchid  ,
           p_jobnumber         => v_my_log_id,
           p_message           => v_message  ,
           p_reason            => v_reason   ,
           p_error             => v_error    ,
           p_trycount          => v_trycount ,
           p_msgtime           => SYSDATE  );
  RAISE;

  END Stage_ShortCode;
  
  --AEO-987 AH Start
  PROCEDURE Update_ShortCode (p_Shortcode IN VARCHAR2,
                              p_TypeCode IN VARCHAR2,
                              retval IN OUT rcursor)
    IS
    v_my_log_id             NUMBER;
    v_dap_log_id            NUMBER;

    --log job attributes
    v_jobdirection          NUMBER:=0;
    v_filename              VARCHAR2(512);
    v_starttime             DATE:=SYSDATE;
    v_endtime               DATE;
    v_messagesreceived      NUMBER:=0;
    v_messagesfailed        NUMBER:=0;
    v_jobstatus             NUMBER:=0;
    v_jobname               VARCHAR2(256);

    --log msg attributes
    v_messageid          VARCHAR2(256);
    v_envkey             VARCHAR2(256):='BP_AE@'||UPPER(sys_context('userenv','instance_name'));
    v_logsource          VARCHAR2(256);
    v_batchid            VARCHAR2(256):=0 ;
    v_message            VARCHAR2(256) ;
    v_reason             VARCHAR2(256);
    v_error              VARCHAR2(256) ;
    v_trycount           NUMBER :=0;
    begin
      v_my_log_id := utility_pkg.get_LIBJobID();

      v_jobname := 'Update_ShortCode';
      v_logsource := v_jobname;

      /* log start of job */
      utility_pkg.Log_job(P_JOB                => v_my_log_id
             ,p_jobdirection       => v_jobdirection
             ,p_filename           => v_filename
             ,p_starttime          => v_starttime
             ,p_endtime            => v_endtime
             ,p_messagesreceived   => v_messagesreceived
             ,p_messagesfailed     => v_messagesfailed
             ,p_jobstatus          => v_jobstatus
             ,p_jobname            => v_jobname);
      
      update bp_ae.ats_rewardshortcode sc
      set sc.a_typecode = p_TypeCode
      where 1=1
      and sc.a_shortcode = p_Shortcode;
      commit;
      
      v_endtime := SYSDATE;
      v_jobstatus := 1;

      /* log end of job */
      utility_pkg.Log_job(P_JOB                => v_my_log_id
             ,p_jobdirection       => v_jobdirection
             ,p_filename           => v_filename
             ,p_starttime          => v_starttime
             ,p_endtime            => v_endtime
             ,p_messagesreceived   => v_messagesreceived
             ,p_messagesfailed     => v_messagesfailed
             ,p_jobstatus          => v_jobstatus
             ,p_jobname            => 'Stage-'||v_jobname);
      
    EXCEPTION
      WHEN OTHERS THEN

       utility_pkg.Log_msg(p_messageid         => v_messageid,
               p_envkey            => v_envkey   ,
               p_logsource         => v_logsource,
               p_filename          => v_filename ,
               p_batchid           => v_batchid  ,
               p_jobnumber         => v_my_log_id,
               p_message           => v_message  ,
               p_reason            => v_reason   ,
               p_error             => v_error    ,
               p_trycount          => v_trycount ,
               p_msgtime           => SYSDATE  );
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
  RAISE;
    
  end Update_ShortCode;
  --AEO-987 AH END
  END Stage_COUPONS;
/
