CREATE OR REPLACE PACKAGE Stage_Storefile IS
type rcursor IS REF CURSOR;

    PROCEDURE Stage_Store (p_filename  IN VARCHAR2, retval IN OUT rcursor);
END Stage_Storefile;
/

CREATE OR REPLACE PACKAGE BODY Stage_Storefile   IS
/*map external table with file */
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
/******************* Internal function to tranform date ************/
  PROCEDURE clear_infile(p_tablename IN VARCHAR2) IS


        lv_String                      VARCHAR2(32000);
        lv_fileHandle                  UTL_FILE.FILE_TYPE;
        lv_path                        VARCHAR2(50) DEFAULT 'USER_EXPORT';
        lv_filename                    VARCHAR2(512);

    BEGIN
        SELECT LOCATION
        ,      DIRECTORY_NAME
        INTO LV_FILENAME
        ,    LV_PATH
        FROM USER_EXTERNAL_LOCATIONS t
        WHERE UPPER(t.table_name) = UPPER(p_tablename);

        lv_fileHandle := UTL_FILE .FOPEN(lv_path, LV_FILENAME, 'W', 32000);

            --utl_file.put_line( lv_fileHandle, lv_colString );
            --utl_file.new_line(lv_filehandle);
        UTL_FILE.FCLOSE(lv_fileHandle);


  END clear_infile;


 /********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/
PROCEDURE Stage_Store (p_filename  IN VARCHAR2,
                         retval          IN OUT rcursor)
IS
 /*   version 1.0
      CreatedBy Bikash
      CreatedOn 06/09/2011
  */
  V_STAGE_PROC_ID         NUMBER:=0; /* for this proc only */
  v_partition_name        VARCHAR2(200);

  V_ProcessID             NUMBER:=0;
  V_BirthDate             DATE;
  V_Zip                   VARCHAR2(256);
  V_Country               VARCHAR2(256);
  V_ToDay                 DATE := TRUNC(SYSDATE);
  V_MemberSource          NUMBER;
  v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;

  /* need a method for looking up these values */
  v_MemberStatusCode      NUMBER; /* not used */

  --log job attributes
  v_jobdirection          NUMBER:=0;
  v_filename              VARCHAR2(512):=p_filename;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='bp_ae@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_filename           VARCHAR2(256):=p_filename ;
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256) ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  BEGIN
   /* get job id for this process and the dap process */
   v_my_log_id := utility_pkg.get_LIBJobID();
   v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'Storefile';
     v_logsource := v_jobname;
     v_membersource := 14; /* AE Store file */
     v_stage_proc_id := 1;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_filename
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'stage'||v_jobname);

ChangeExternalTable(p_ExtTableName => 'EXT_STORE01',p_FileName => p_FileName);

Execute Immediate 'Truncate Table lw_storedef_stage';

/* Insert data into staging table */
INSERT INTO lw_storedef_stage(StoreName, BrandName, addresslineone, addresslinetwo,city, stateorprovince,
ziporpostalcode,country,region,district,storenumber)
SELECT A.STORENAME,
(SELECT A_BRANDID FROM ats_refbrand WHERE SUBSTR(TO_CHAR(100 + a_brandnumber),2,2) = A.BRANDNAME and ROWNUM =1),
NVL(A.ADDRESSLINEONE, 'N/A'), A.ADDRESSLINETWO, NVL(A.CITY, 'N/A'), NVL(A.STATEORPROVINCE, 'N/A')
, A.ZIPORPOSTALCODE,
CASE
  When to_char(cast(a.storenumber as integer)) in
    ('9001','9002','9003','9004','9005','9006','9007','9008','9009','9011','9012','9013','9014','9015','9016','9017','9197','9198','9199','9201','9202','9203') Then CAST('MEX' as nvarchar2(10))
  Else NVL((SELECT A_CountryCode FROM ATS_REFSTATES B WHERE B.A_STATECODE = A.STATEORPROVINCE and ROWNUM =1), 'N/A')
END as Country,
A.REGION, A.DISTRICT , to_char(cast(a.storenumber as integer)) FROM ext_store01 A;


  DECLARE/* check log file for errors */
    lv_err   VARCHAR2(4000);
    lv_n     NUMBER;
  BEGIN
    EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)'||CHR(10)||
                      'FROM EXT_STORE01_LOG'||CHR(10)||
                      'WHERE rec LIKE ''ORA-%''' INTO lv_n, lv_err;

    IF lv_n > 0 THEN /* log error msg */
      /* increment jobs fail count */
      v_messagesfailed := v_messagesfailed + lv_n;
      v_reason := 'Failed reads by external table';
      v_message := '<StageProc>'||CHR(10)||
                   '  <Tables>'||CHR(10)||
                   '    <External>EXT_Store01'||LPAD(to_char(V_STAGE_PROC_ID),2,'0')||'</External>'||CHR(10)||
                   '    <Stage>LW_Storedef_STAGE</Stage>'||CHR(10)||
                   '    <FileName>'||p_filename||'</FileName>'||CHR(10)||
                   '  </Tables>'||CHR(10)||
                   '</StageProc>';
      utility_pkg.Log_msg(p_messageid         => v_messageid,
              p_envkey            => v_envkey   ,
              p_logsource         => v_logsource,
              p_filename          => p_filename ,
              p_batchid           => v_batchid  ,
              p_jobnumber         => v_my_log_id,
              p_message           => v_message  ,
              p_reason            => v_reason   ,
              p_error             => lv_err    ,
              p_trycount          => lv_n ,
              p_msgtime           => SYSDATE  );
    END IF;
  END;
  COMMIT;

  /* insert here */
  v_endtime := SYSDATE;
  v_jobstatus := 1;

  /* log end of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_filename
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'Stage-'||v_jobname);

  /* create job for dap */
  utility_pkg.Log_job(P_JOB                => v_dap_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_filename
         ,p_starttime          => SYSDATE
         ,p_endtime            => NULL
         ,p_messagesreceived   => NULL
         ,p_messagesfailed     => NULL
         ,p_jobstatus          => 0
         ,p_jobname            => 'DAP-'||v_jobname);

         open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN
  IF v_messagesfailed = 0 THEN
    v_messagesfailed := 0+1;
  END IF;

   utility_pkg.Log_msg(p_messageid         => v_messageid,
           p_envkey            => v_envkey   ,
           p_logsource         => v_logsource,
           p_filename          => p_filename ,
           p_batchid           => v_batchid  ,
           p_jobnumber         => v_my_log_id,
           p_message           => v_message  ,
           p_reason            => v_reason   ,
           p_error             => v_error    ,
           p_trycount          => v_trycount ,
           p_msgtime           => SYSDATE  );
  RAISE;

  END Stage_Store;

  END Stage_Storefile;
/

