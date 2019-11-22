CREATE OR REPLACE PACKAGE STAGE_PRODUCTS IS
type rcursor IS REF CURSOR;

    PROCEDURE stage_product (p_Productfilename  IN VARCHAR2, retval IN OUT rcursor);
END Stage_Products;
/

CREATE OR REPLACE PACKAGE BODY Stage_PRODUCTS   IS
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
/*AEO-169 : Product File failing processing ---------------------changes begin-----SCJ */
PROCEDURE Stage_ProductLoad(p_dummy IN VARCHAR2)
IS
 lv_row_count             NUMBER := 0;
 lv_stats_id              NUMBER;
 v_categoryid             NUMBER;
 v_pointtypeid            NUMBER;

    CURSOR curA IS                   /* this selects the products from the staging table that exist in the product table*/
        SELECT ps.name,
               ps.brandname,
               ps.partnumber,
               ps.classcode,
               ps.classdescription,
               ps.deptcode,
               ps.deptdescription,
               ps.style,
               ps.color,
               ps.colordescription,
               ps.division,
               ps.divisiondescription
          from bp_ae.lw_product_stage ps
         INNER JOIN bp_ae.lw_product p
            ON p.partnumber = ps.partnumber;
    TYPE curA_type is TABLE OF curA%ROWTYPE INDEX BY PLS_INTEGER;
    rec1 curA_type;

     CURSOR curB IS            /* this selects the products from the staging table that do notexist in the product table*/
        SELECT ps.name,
               ps.brandname,
               ps.partnumber,
               ps.classcode,
               ps.classdescription,
               ps.deptcode,
               ps.deptdescription,
               ps.style,
               ps.color,
               ps.colordescription,
               ps.division,
               ps.divisiondescription
          from bp_ae.lw_product_stage ps
         left join bp_ae.lw_product p
            ON p.partnumber = ps.partnumber
            where p.partnumber is null;
    TYPE curB_type is TABLE OF curB%ROWTYPE INDEX BY PLS_INTEGER;
    rec2 curB_type;

BEGIN
    OPEN curA;
    LOOP
        FETCH curA BULK COLLECT INTO rec1 LIMIT 1000;
            -- FORALL Update
            FORALL i IN 1 .. rec1.COUNT
            UPDATE bp_ae.lw_product p1
               SET p1.name = rec1(i).partnumber,
               p1.brandname = rec1(i).brandname,
               p1.classcode = rec1(i).classcode,
               p1.classdescription = rec1(i).classdescription,
               p1.deptcode = rec1(i).deptcode,
               p1.deptdescription = rec1(i).deptdescription,
               p1.styledescription = rec1(i).name,
               p1.updatedate = sysdate,
               p1.stylecode = rec1(i).style,
               p1.companycode = rec1(i).color,
               p1.companydescription = rec1(i).colordescription,
               p1.divisioncode = rec1(i).division,
               p1.divisiondescription = rec1(i).divisiondescription
            WHERE p1.partnumber = rec1(i).partnumber;
            lv_row_count := lv_row_count + SQL%ROWCOUNT;
            -- Commit, release lock
            COMMIT WRITE BATCH NOWAIT;
        EXIT WHEN curA%NOTFOUND;

    END LOOP;
    CLOSE curA;
    COMMIT;

 SELECT id INTO v_categoryid FROM lw_category WHERE name = 'Misc';
 SELECT pointtypeid INTO v_pointtypeid FROM  lw_pointtype WHERE NAME = 'StartingPoints';
 OPEN curB;
    LOOP
        FETCH curB BULK COLLECT INTO rec2 LIMIT 1000;
            -- FORALL Update
            FORALL j IN 1 .. rec2.COUNT
            Insert into bp_ae.Lw_Product
              (id,
               categoryid,
               isvisibleinln,
               name,
               brandname,
               partnumber,
               baseprice,
               pointtype,
               classcode,
               classdescription,
               styledescription,
               deptcode,
               deptdescription,
               createdate,
               createdbyuser,
               stylecode,
               companycode,
               companydescription,
               divisioncode,
               divisiondescription)
               values
                    ( hibernate_sequence.nextval,
                     v_categoryid,
                     0,
                     rec2(j).partnumber,
                     rec2(j).brandname,
                     rec2(j).partnumber,
                     0,
                     v_pointtypeid,
                     rec2(j).classcode,
                     rec2(j).classdescription,
                     rec2(j).name,
                     rec2(j).deptcode,
                     rec2(j).deptdescription,
                     sysdate,
                     0,
                     rec2(j).style,
                     rec2(j).color,
                     rec2(j).colordescription,
                     rec2(j).division,
                     rec2(j).divisiondescription);
            lv_row_count := lv_row_count + SQL%ROWCOUNT;
            -- Commit, release lock
            COMMIT WRITE BATCH NOWAIT;
        EXIT WHEN curB%NOTFOUND;

    END LOOP;
    CLOSE curB;
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END Stage_ProductLoad;
/*AEO-169 : Product File failing processing ---------------------changes end-----SCJ */



 /********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/


  PROCEDURE Stage_Product (p_Productfilename  IN VARCHAR2, retval          IN OUT rcursor)
IS
 /*   version 1.0
      CreatedBy Bikash
      CreatedOn 3/2/2011
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
  v_filename              VARCHAR2(512):=p_Productfilename;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);

  --log msg attributes
  v_messageid          VARCHAR2(256);
  v_envkey             VARCHAR2(256):='PD_AE_DEV@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource          VARCHAR2(256);
  v_filename           VARCHAR2(256):=p_Productfilename ;
  v_batchid            VARCHAR2(256):=0 ;
  v_message            VARCHAR2(256) ;
  v_reason             VARCHAR2(256) ;
  v_error              VARCHAR2(256) ;
  v_trycount           NUMBER :=0;
  BEGIN
   /* get job id for this process and the dap process */
   v_my_log_id := utility_pkg.get_LIBJobID();
   v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'Productfile';
     v_logsource := v_jobname;
     v_membersource := 15; /* AE Store file */
     v_stage_proc_id := 1;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_Productfilename
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'Stage-'||v_jobname);


/* STEP 3: Change external table file */
ChangeExternalTable(p_ExtTableName => 'EXT_PROD01',p_FileName => p_Productfilename);

/* clear the stage table */
EXECUTE IMMEDIATE 'Truncate Table lw_product_stage';


/* Insert data into product staging table */
INSERT INTO lw_Product_stage(ID, NAME, BRANDNAME, PARTNUMBER,CLASSCODE, CLASSDESCRIPTION, DEPTCODE, DEPTDESCRIPTION, STYLE, COLOR, COLORDESCRIPTION, DIVISION, DIVISIONDESCRIPTION )
SELECT hibernate_sequence.nextval,
case
  when a.skudescription is null then 'TEST SKU'
  else a.skudescription
end skudescription,
(SELECT A_BRANDID FROM ats_refbrand WHERE A_BRANDNUMBER = A.BRANDNUMBER  and ROWNUM =1),
--a.skunumber, A.CLASSCODE, a.classdesription, /*AEO-169 : Product File failing processing ---------------------changes here-----SCJ */
ltrim(a.skunumber,'0'),
a.classcode,
a.classdesription,
a.deptcode,
a.deptdescription,
a.style,
a.color,
a.colordescription,
a.division,
--AEO551- eliminating blank lines from staging table---------------------- SCJ
replace(a.divisiondescription, CHR(13), NULL)
FROM EXT_PROD01 A
where replace(a.classcode, CHR(13), NULL) is not null;
--AEO551- eliminating blank lines from staging table---------------------- SCJ
/*AEO-169 : Product File failing processing ---------------------changes begin-----SCJ */
     Stage_ProductLoad(p_dummy => ' '); /* inserting/updating lw_product table
/*AEO-169 : Product File failing processing ---------------------changes end-----SCJ */

  DECLARE/* check log file for errors */
    lv_err   VARCHAR2(4000);
    lv_n     NUMBER;
  BEGIN
    EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)'||CHR(10)||
                      'FROM EXT_PROD01_LOG'||CHR(10)||
                      'WHERE rec LIKE ''ORA-%''' INTO lv_n, lv_err;

    IF lv_n > 0 THEN /* log error msg */
      /* increment jobs fail count */
      v_messagesfailed := v_messagesfailed + lv_n;
      v_reason := 'Failed reads by external table';
      v_message := '<StageProc>'||CHR(10)||
                   '  <Tables>'||CHR(10)||
                   '    <External>EXT_PROD01'||LPAD(to_char(V_STAGE_PROC_ID),2,'0')||'</External>'||CHR(10)||
                   '    <Stage>LW_PRODUCT_STAGE</Stage>'||CHR(10)||
                   '    <FileName>'||p_Productfilename||'</FileName>'||CHR(10)||
                   '  </Tables>'||CHR(10)||
                   '</StageProc>';
      utility_pkg.Log_msg(p_messageid         => v_messageid,
              p_envkey            => v_envkey   ,
              p_logsource         => v_logsource,
              p_filename          => p_Productfilename ,
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
         ,p_filename           => p_Productfilename
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => 'Stage-'||v_jobname);

  /* create job for dap */
  utility_pkg.Log_job(P_JOB                => v_dap_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => p_Productfilename
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
           p_filename          => p_Productfilename ,
           p_batchid           => v_batchid  ,
           p_jobnumber         => v_my_log_id,
           p_message           => v_message  ,
           p_reason            => v_reason   ,
           p_error             => v_error    ,
           p_trycount          => v_trycount ,
           p_msgtime           => SYSDATE  );
  RAISE;

  END Stage_Product;

  END Stage_PRODUCTS;
/

