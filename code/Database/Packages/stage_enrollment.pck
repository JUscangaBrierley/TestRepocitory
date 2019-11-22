CREATE OR REPLACE PACKAGE Stage_Enrollment IS
  type rcursor IS REF CURSOR;

  FUNCTION get_PartHighValue(p_table_name     VARCHAR2,
                             p_partition_name VARCHAR2) RETURN VARCHAR2;
  PROCEDURE stage_enroll(p_filename IN VARCHAR2, retval IN OUT rcursor);
  PROCEDURE Update_EnrollDate(p_enrolltype IN VARCHAR2, retval IN OUT rcursor);
  PROCEDURE Update_LinkKey(retval IN OUT rcursor);
END Stage_Enrollment;
/

CREATE OR REPLACE PACKAGE BODY Stage_Enrollment IS
  /********************************************************************
  ******************************************************************vi**
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/
  PROCEDURE clear_infile(p_tablename IN VARCHAR2) IS

    lv_String     VARCHAR2(32000);
    lv_fileHandle UTL_FILE.FILE_TYPE;
    lv_path       VARCHAR2(50) DEFAULT 'USER_EXPORT';
    lv_filename   VARCHAR2(512);

  BEGIN
    SELECT LOCATION, DIRECTORY_NAME
      INTO LV_FILENAME, LV_PATH
      FROM USER_EXTERNAL_LOCATIONS t
     WHERE UPPER(t.table_name) = UPPER(p_tablename);

    lv_fileHandle := UTL_FILE.FOPEN(lv_path, LV_FILENAME, 'W', 32000);

    --utl_file.put_line( lv_fileHandle, lv_colString );
    --utl_file.new_line(lv_filehandle);
    UTL_FILE.FCLOSE(lv_fileHandle);

  END clear_infile;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/
  FUNCTION valid_date(p_date_string VARCHAR2, p_date_format VARCHAR2)
    RETURN DATE IS
    v_date DATE;
  BEGIN
    IF TRIM(p_date_string) IS NULL OR TRIM(p_date_format) IS NULL THEN
      RETURN NULL;
    END IF;
    v_date := to_date(p_date_string, p_date_format);
    RETURN v_date;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN NULL;
  END valid_date;
   /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /******************* Internal function to tranform date ************/
  FUNCTION get_basebrand(p_storenumber VARCHAR2, p_alternateid VARCHAR2)
    RETURN VARCHAR2 IS
    v_baseBrand VARCHAR2(10);
    v_exists INTEGER;
  BEGIN

    IF TRIM(p_storenumber) IS NULL AND TRIM(p_alternateid) IS NULL THEN
      RETURN NULL;
    END IF;

    v_baseBrand := null;
    select count(*) into v_exists from ats_refbrand where a_loyaltynumberprefix = substr(p_alternateid, 1, 2);

    if v_exists >= 1
    then
         Select rb.a_brandid into v_baseBrand from ats_refbrand rb where rb.a_loyaltynumberprefix = substr(p_alternateid, 1, 2) and rownum = 1;
         return v_baseBrand;
    else
      Select st.brandname into v_baseBrand from lw_storedef st where TRIM(st.storenumber) = TRIM(p_storenumber) and rownum = 1;
    end if;

     IF(v_baseBrand = '1' or v_baseBrand = '4' or v_baseBrand = '5' or v_baseBrand = '6' or v_baseBrand = '3' or v_baseBrand = '9' or v_baseBrand = '11')
      THEN
      v_baseBrand := '1';
    END IF;

    RETURN v_baseBrand;
  EXCEPTION
    WHEN OTHERS THEN
      RETURN NULL;
  END get_basebrand;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE initialize(p_stage_proc_id VARCHAR2, p_filename VARCHAR2) IS
    v_partition_name VARCHAR2(256);
    v_sql            VARCHAR2(4000);
    v_inst           VARCHAR2(64) := upper(sys_context('userenv',
                                                       'instance_name'));
  BEGIN
    /*              set the external table filename                                      */
    v_sql := 'ALTER TABLE ext_enroll' ||
             LPAD(TRIM(p_stage_proc_id), 2, '0') || CHR(10) ||
             'LOCATION (AE_IN' || CHR(58) || '''' || p_filename || ''')';
    EXECUTE IMMEDIATE v_sql;

    IF v_inst NOT IN ('XE') THEN
      /*            get partition name to truncate */
      begin
        SELECT t.partition_name
          INTO v_partition_name
          FROM USER_TAB_PARTITIONS t
         WHERE get_PartHighValue(t.table_name, t.partition_name) =
               TO_char((p_stage_proc_id + 1))
           AND t.table_name = 'LW_MEMBERDETAIL_STAGE';
        /*              Truncate the partition for this file/process                       */
        v_sql := 'ALTER TABLE lw_memberdetail_stage ' || CHR(10) ||
                 'TRUNCATE PARTITION ' || v_partition_name || CHR(10) ||
                 'UPDATE GLOBAL INDEXES';
        EXECUTE IMMEDIATE v_sql;

      EXCEPTION
        WHEN no_data_found THEN
          v_inst := 'NoPartition';
      END;
    END IF;

    IF v_inst IN ('XE', 'NoPartition') THEN
      /* partitions not allowed */
      v_sql := 'DELETE lw_memberdetail_stage ' || CHR(10) ||
               'WHERE STAGE_PROC_ID = :STAGE_PROC_ID';
      EXECUTE IMMEDIATE v_sql
        USING p_stage_proc_id;
    END if;

  END initialize;
  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION get_PartHighValue(p_table_name     VARCHAR2,
                             p_partition_name VARCHAR2) RETURN VARCHAR2 IS
    v_high_value LONG;
  BEGIN
    SELECT high_value
      INTO v_high_value
      FROM USER_TAB_PARTITIONS
     WHERE table_name = p_table_name
       AND partition_name = p_partition_name;

    RETURN TRIM(SUBSTR(v_high_value, 1, 4000));

  END;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE stage_enroll(p_filename IN VARCHAR2, retval IN OUT rcursor) IS
    /*  version 1.0
        CreatedBy jcanada
        CreatedOn 3/2/2011

    */
    V_STAGE_PROC_ID  NUMBER := 0; /* for this proc only */
    v_partition_name VARCHAR2(200);

    V_ProcessID    NUMBER := 0;
    V_BirthDate    DATE;
    V_OpenDate     DATE;
    V_CloseDate    DATE;
    V_Zip          VARCHAR2(256);
    V_Country      VARCHAR2(256);
--    V_ToDay        DATE := SYSDATE;
    V_ToDay        DATE ;
    V_MemberSource NUMBER;
    v_my_log_id    NUMBER;
    v_dap_log_id   NUMBER;
    v_baseBrand    VARCHAR2(256);

    /* need a method for looking up these values */
    v_MemberStatusCode NUMBER; /* not used */

    --log job attributes
    v_jobdirection     NUMBER := 0;
    v_filename         VARCHAR2(512) := p_filename;
    v_starttime        DATE := SYSDATE;
    v_endtime          DATE;
    v_messagesreceived NUMBER := 0;
    v_messagesfailed   NUMBER := 0;
    v_jobstatus        NUMBER := 0;
    v_jobname          VARCHAR2(256);

    --log msg attributes
    v_messageid VARCHAR2(256);
    v_envkey    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(sys_context('userenv',
                                                   'instance_name'));
    v_logsource VARCHAR2(256);
    v_filename  VARCHAR2(256) := p_filename;
    v_batchid   VARCHAR2(256) := 0;
    v_message   VARCHAR2(256);
    v_reason    VARCHAR2(256);
    v_error     VARCHAR2(256);
    v_trycount  NUMBER := 0;

    CURSOR cur_CardHolder01 IS
      SELECT TRIM(UPPER(firstname)) AS firstname
             , TRIM(UPPER(middlename)) AS middlename
             , TRIM(UPPER(lastname)) AS lastname
             , TRIM(UPPER(addressline1)) AS addressline1
             , TRIM(UPPER(addressline2)) AS addressline2
             , TRIM(UPPER(city)) AS city
             , TRIM(UPPER(state)) AS state
             , TRIM(UPPER(zip)) AS zip
             , TRIM(UPPER(zip4)) AS zip4
             , TRIM(SUBSTR(birthdate, 5)) AS birthdate /* substr leading zeros off */
             , TRIM(filler2) AS filler2
             , TRIM(alternateid) AS alternateid
             , TRIM(LOWER(primaryemailaddress)) AS emailaddress
             , TRIM(cardtype) AS cardtype
             , CASE
               WHEN TRIM(storenumber) is not null THEN CAST(CAST(TRIM(storenumber) AS number) as varchar2(256))
               ELSE TRIM(storenumber)
             END as storenumber
             , TRIM(productid) as productid
             , TRIM(closedate) AS closedate   /*Redesign Changes  here-------------------------------------------SCJ */
             , TRIM(opendate) AS opendate   /*Redesign Changes  here-------------------------------------------RKG */
             , TRIM(cid) AS cid   /*Redesign Changes  here-------------------------------------------RKG */
             , TRIM(accountkey) AS accountkey   /*Redesign Changes  here-------------------------------------------RKG */
        FROM ext_enroll01;

    CURSOR cur_DataEntry01 IS
      SELECT TRIM(ALTERNATEID) AS ALTERNATEID,
             TRIM(UPPER(FIRSTNAME)) AS FIRSTNAME,
             TRIM(UPPER(LASTNAME)) AS LASTNAME,
             TRIM(UPPER(ADDRESSLINE1)) AS ADDRESSLINE1,
             TRIM(UPPER(ADDRESSLINE2)) AS ADDRESSLINE2,
             TRIM(UPPER(CITY)) AS CITY,
             TRIM(UPPER(STATE)) AS STATE,
             TRIM(UPPER(ZIP)) AS ZIP,
             TRIM(UPPER(COUNTRY)) AS COUNTRY,
             TRIM(PRIMARYPHONENUMBER) AS PRIMARYPHONENUMBER,
             LPAD(TRIM(BirthDate), 8, '0') AS BIRTHDATE,
             TRIM(GENDER) AS GENDER,
             TRIM(LOWER(PRIMARYEMAILADDRESS)) AS EMAILADDRESS,
             TRIM(ANSWER_TO_SURVEY_QUESTION_1) AS ANSWER_TO_SURVEY_QUESTION_1,
             TRIM(ANSWER_TO_SURVEY_QUESTION_2) AS ANSWER_TO_SURVEY_QUESTION_2,
             TRIM(ANSWER_TO_SURVEY_QUESTION_3) AS ANSWER_TO_SURVEY_QUESTION_3,
             CASE
               WHEN TRIM(STORE_NUMBER) is not null THEN CAST(CAST(TRIM(STORE_NUMBER) AS number) as varchar2(256))
               ELSE TRIM(STORE_NUMBER)
             END as STORE_NUMBER,
             TRIM(APPLICATION_TYPE) AS APPLICATION_TYPE,
             TRIM(UPPER(ADDRESSMAILABLE)) AS ADDRESSMAILABLE,
             TRIM(MOBILEPHONE) AS MOBILEPHONE,
             NVL(TRIM(LANGUAGEPREFERENCE), 0) AS LANGUAGEPREFERENCE,
             TRIM(SMS_MESSAGING) AS SMS_MESSAGING,
             CASE
               WHEN TRIM(Sms_Messaging) = '1' THEN
                v_today
               ELSE
                NULL
             END SMSOPTINDATE,
              /*Redesign Changes  begin here-------------------------------------------SCJ */
             TRIM(PRIMARY_CONTACT) AS PRIMARY_CONTACT,
             LPAD(TRIM(ENROLLMENT_DATE), 8, '0') AS ENROLLMENT_DATE
               /*Redesign Changes  end here-------------------------------------------SCJ */
        FROM EXT_ENROLL02;

    /*********** Internal procedure to process BPCardholder ************/
    PROCEDURE stage_CardHolder01 IS
    BEGIN

      /* initialize, truncates stage partition and set external table to read p_filename */
      initialize(V_STAGE_PROC_ID, p_filename);
      /* reset log file, read later for errors */
      clear_infile('ext_enroll01_log');
      FOR rec_CardHolder01 IN cur_CardHolder01 LOOP
        /* using year 1600 here because it is a leap year */
        V_BirthDate := valid_date(rec_CardHolder01.birthdate || '1600', 'mmddyyyy');
        V_OpenDate := valid_date(rec_CardHolder01.Opendate , 'mmddyyyy');
        V_CloseDate := valid_date(rec_CardHolder01.Closedate , 'mmddyyyy');

        /* add zip + 4 to zip code */
        IF rec_CardHolder01.zip4 IS NOT NULL AND
           rec_CardHolder01.zip IS NOT NULL THEN
          V_Zip := rec_CardHolder01.zip || '-' || rec_CardHolder01.zip4;
        ELSE
          V_Zip := rec_CardHolder01.zip;
        END IF;

        v_baseBrand := get_basebrand(rec_CardHolder01.Storenumber, rec_CardHolder01.alternateid);

        IF v_baseBrand is null THEN
          IF rec_CardHolder01.Productid = '020' THEN v_baseBrand := 1; END IF; --AE
          IF rec_CardHolder01.Productid = '021' THEN v_baseBrand := 2; END IF; --Aerie
          IF rec_CardHolder01.Productid = '022' THEN v_baseBrand := 1; END IF; --77 kids, default 77kids to AE
          IF rec_CardHolder01.Productid = '040' THEN v_baseBrand := 1; END IF; --AE
        END IF;

        IF rec_CardHolder01.State is not null Then
          SELECT nvl(a_countrycode, '')
            INTO V_Country
            FROM ATS_REFSTATES t
           WHERE a_statecode = rec_CardHolder01.State;
        END IF;
/* AEO 159 - use date from filename for enrolldate -------------------SCJ */
         select to_date(substr(p_filename,17,8),'mm/dd/yyyy') into V_ToDay from dual;
/* AEO 159 - use date from filename for enrolldate -------------------SCJ */
        BEGIN
          INSERT INTO LW_MEMBERDETAIL_STAGE
            (ROWKEY
             , ipcode
             , statuscode
             , FIRSTNAME
             , MIDDLENAME
             , LASTNAME
             , BIRTHDATE
             , NAMEPREFIX
             , NAMESUFFIX
             , ALTERNATEID
             , USERNAME
             , PASSWORD
             , EMAILADDRESS
             , PRIMARYPHONENUMBER
             , PRIMARYPOSTALCODE
             , ADDRESSLINEone
             , ADDRESSLINEtwo
             , ADDRESSLINEthree
             , ADDRESSLINEfour
             , CITY
             , stateorprovince
             , ziporpostalcode
             , COUNTY
             , COUNTRY
             , ADDRESSMAILABLE
             , HOMEPHONE
             , MOBILEPHONE
             , WORKPHONE
             , SECONDARYEMAILADDRESS
             , EMAILADDRESSMAILABLE
             , MEMBERSTATUSCODE
             , DIRECTMAILOPTINDATE
             , EMAILOPTINDATE
             , SMSOPTINDATE
             , ENROLLDATE
             , GENDER
             , MEMBERSOURCE
             , LANGUAGEPREFERENCE
             , SECURITYQUESTION
             , SECURITYANSWER
             , STAGE_PROC_ID
             , HOMESTOREID
             , BASEBRAND
             , CLOSEDATE            /*Redesign Changes  here-------------------------------------------SCJ */
             , OPENDATE            /*Redesign Changes  here-------------------------------------------RKG */
             , CID                /*Redesign Changes  here-------------------------------------------RKG */
             , ACCOUNTKEY         /*Redesign Changes  here-------------------------------------------RKG */
             , CARDTYPE          /*Redesign Changes  here-------------------------------------------RKG */
             , createdate)
          VALUES
            (seq_rowkey.nextval
             ,0
             ,V_ProcessID
             ,rec_CardHolder01.FIRSTNAME
             ,rec_CardHolder01.MIDDLENAME
             ,rec_CardHolder01.LASTNAME
             ,V_BirthDate
             ,NULL /*NAMEPREFIX*/
             ,NULL /*NAMESUFFIX*/
             ,rec_CardHolder01.ALTERNATEID
             ,NULL /*USERNAME*/
             ,NULL /*PASSWORD*/
             ,rec_CardHolder01.EMAILADDRESS
             ,NULL /*PRIMARYPHONENUMBER*/
             ,V_Zip /*PRIMARYPOSTALCODE*/
             ,rec_CardHolder01.ADDRESSLINE1
             ,rec_CardHolder01.ADDRESSLINE2
             ,NULL /*ADDRESSLINE3*/
             ,NULL /*ADDRESSLINE4*/
             ,rec_CardHolder01.CITY
             ,rec_CardHolder01.STATE
             ,V_Zip
             ,NULL /*COUNTY*/
             ,V_Country /*COUNTRY*/
             ,CASE WHEN length(rec_CardHolder01.ADDRESSLINE1) > 0 THEN 1 ELSE 0 END /*ADDRESSMAILABLE*/
             ,NULL /*HOMEPHONE*/
             ,NULL /*MOBILEPHONE*/
             ,NULL /*WORKPHONE*/
             ,NULL /*SECONDARYEMAILADDRESS*/
             ,NULL /*EMAILADDRESSMAILABLE*/
             ,v_MemberStatusCode
             ,NULL /*DIRECTMAILOPTINDATE*/
             ,NULL /*EMAILOPTINDATE*/
             ,NULL /*SMSOPTINDATE*/
             ,V_ToDay
          --  to_date(substr(v_filename,11,8),'yyyy/mm/dd'),
             ,'U' /*GENDER*/
             ,v_MemberSource
             ,0 /*LANGUAGEPREFERENCE*/
             ,NULL /*SECURITYQUESTION*/
             ,NULL /*SECURITYANSWER*/
             ,V_STAGE_PROC_ID
             ,rec_CardHolder01.Storenumber
             ,v_baseBrand
             ,V_CloseDate                            /*Redesign Changes  here-------------------------------------------RKG */
             ,V_OpenDate                            /*Redesign Changes  here-------------------------------------------RKG */
             ,rec_CardHolder01.cid                  /*Redesign Changes  here-------------------------------------------RKG */
             ,rec_CardHolder01.accountkey           /*Redesign Changes  here-------------------------------------------RKG */
             ,rec_CardHolder01.Cardtype           /*Redesign Changes  here-------------------------------------------RKG */
             ,SYSDATE);

          v_messagesreceived := v_messagesreceived + 1;
        EXCEPTION
          WHEN OTHERS THEN
            v_messagesfailed := v_messagesfailed + 1;
            v_error          := SQLERRM;
            v_reason         := 'Failed insert to LW_MEMBERDETAIL_STAGE';
            v_message        := '<Members>' || CHR(10) || '  <Member>' ||
                                CHR(10) || '    <FirstName>' ||
                                rec_CardHolder01.FIRSTNAME ||
                                '</FirstName>' || CHR(10) ||
                                '    <LastName>' ||
                                rec_CardHolder01.LASTNAME || '</LastName>' ||
                                CHR(10) || '    <PrimaryEmail>' ||
                                rec_CardHolder01.EMAILADDRESS ||
                                '</PrimaryEmail>' || CHR(10) ||
                                '    <AlternateId>' ||
                                rec_CardHolder01.ALTERNATEID ||
                                '</AlternateId>' || CHR(10) ||
                                '  </Member>' || CHR(10) || '</Members>';
            /* log error */
            utility_pkg.Log_msg(p_messageid => v_messageid,
                                p_envkey    => v_envkey,
                                p_logsource => v_logsource,
                                p_filename  => p_filename,
                                p_batchid   => v_batchid,
                                p_jobnumber => v_my_log_id,
                                p_message   => v_message,
                                p_reason    => v_reason,
                                p_error     => v_error,
                                p_trycount  => v_trycount,
                                p_msgtime   => SYSDATE);
        END;
      END LOOP;
      COMMIT;

    END stage_Cardholder01;

    /*********** Internal procedure to process DataEntry File ************/
    PROCEDURE stage_DataEntry01 IS
    BEGIN

      /* initialize, truncates stage partition and set external table to read p_filename */
      initialize(V_STAGE_PROC_ID, p_filename);
      /* reset log file, read later for errors */
      clear_infile('ext_enroll02_log');

      FOR rec_DataEntry01 IN cur_DataEntry01 LOOP
        V_BirthDate := valid_date(rec_DataEntry01.BirthDate, 'mmddrrrr');

        IF rec_DataEntry01.State is not null Then
          SELECT nvl(a_countrycode, '')
            INTO V_Country
            FROM ATS_REFSTATES t
           WHERE a_statecode = rec_DataEntry01.State;
        END IF;

        v_baseBrand := get_basebrand(rec_DataEntry01.Store_Number, rec_DataEntry01.ALTERNATEID);

        IF v_baseBrand is null THEN
          v_baseBrand := 1; --AE
        END IF;
        /*Redesign Changes  begin here-------------------------------------------SCJ */
 /* AEO 159 - use date from filename for enrolldate -------------------SCJ */
--         select to_date(substr(p_filename,11,8),'yyyy/mm/dd') into V_ToDay from dual;
             V_ToDay := valid_date(rec_DataEntry01.ENROLLMENT_DATE, 'mmddrrrr');
/* AEO 159 - use date from filename for enrolldate -------------------SCJ */
/*Redesign Changes  end here-------------------------------------------SCJ */

        BEGIN
          INSERT INTO LW_MEMBERDETAIL_STAGE
            (ROWKEY,
             ipcode,
             statuscode,
             FIRSTNAME,
             MIDDLENAME,
             LASTNAME,
             BIRTHDATE,
             NAMEPREFIX,
             NAMESUFFIX,
             ALTERNATEID,
             USERNAME,
             PASSWORD,
             EMAILADDRESS,
             PRIMARYPHONENUMBER,
             PRIMARYPOSTALCODE,
             ADDRESSLINEone,
             ADDRESSLINEtwo,
             ADDRESSLINEthree,
             ADDRESSLINEfour,
             CITY,
             stateorprovince,
             ziporpostalcode,
             COUNTY,
             COUNTRY,
             ADDRESSMAILABLE,
             HOMEPHONE,
             MOBILEPHONE,
             WORKPHONE,
             SECONDARYEMAILADDRESS,
             EMAILADDRESSMAILABLE,
             MEMBERSTATUSCODE,
             DIRECTMAILOPTINDATE,
             EMAILOPTINDATE,
             SMSOPTINDATE,
             ENROLLDATE,
             GENDER,
             MEMBERSOURCE,
             LANGUAGEPREFERENCE,
             SECURITYQUESTION,
             SECURITYANSWER,
             STAGE_PROC_ID,
             HOMESTOREID,
             BASEBRAND,
             PRIMARYCONTACT,   /*Redesign Changes  here-------------------------------------------SCJ */
             createdate)
          VALUES
            (seq_rowkey.nextval,
             0,
             V_ProcessID,
             rec_DataEntry01.FIRSTNAME,
             NULL /*MIDDLENAME*/,
             rec_DataEntry01.LASTNAME,
             V_BirthDate,
             NULL /*NAMEPREFIX*/,
             NULL /*NAMESUFFIX*/,
             rec_DataEntry01.ALTERNATEID,
             NULL /*USERNAME*/,
             NULL /*PASSWORD*/,
             rec_DataEntry01.EMAILADDRESS,
             NULL /*PRIMARYPHONENUMBER*/,
             rec_DataEntry01.zip /*PRIMARYPOSTALCODE*/, /* did not include plus 4 here */
             rec_DataEntry01.ADDRESSLINE1,
             rec_DataEntry01.ADDRESSLINE2,
             NULL /*ADDRESSLINE3*/,
             NULL /*ADDRESSLINE4*/,
             rec_DataEntry01.CITY,
             rec_DataEntry01.STATE,
             rec_DataEntry01.Zip,
             NULL /*COUNTY*/,
             V_Country,
             CASE WHEN rec_DataEntry01.ADDRESSMAILABLE = 0 THEN 1 ELSE 0 END,
             NULL /*HOMEPHONE*/,
             rec_DataEntry01.MOBILEPHONE,
             NULL /*WORKPHONE*/,
             NULL /*SECONDARYEMAILADDRESS*/,
             NULL /*EMAILADDRESSMAILABLE*/,
             v_MemberStatusCode,
             NULL /*DIRECTMAILOPTINDATE*/,
             NULL /*EMAILOPTINDATE*/,
             rec_DataEntry01.SMSOPTINDATE,
             V_ToDay,
             rec_DataEntry01.GENDER,
             v_MemberSource,
             rec_DataEntry01.LANGUAGEPREFERENCE /*LANGUAGEPREFERENCE*/,
             NULL /*SECURITYQUESTION*/,
             NULL /*SECURITYANSWER*/,
             V_STAGE_PROC_ID,
             rec_DataEntry01.STORE_NUMBER /*STORENUMBER*/,
             v_baseBrand,
             rec_DataEntry01.PRIMARY_CONTACT,  /*Redesign Changes  here-------------------------------------------SCJ */
             SYSDATE);
          v_messagesreceived := v_messagesreceived + 1;
        EXCEPTION
          WHEN OTHERS THEN
            v_messagesfailed := v_messagesfailed + 1;
            v_error          := SQLERRM;
            v_reason         := 'Failed insert to LW_MEMBERDETAIL_STAGE';
            v_message        := '<Members>' || CHR(10) || '  <Member>' ||
                                CHR(10) || '    <FirstName>' ||
                                rec_DataEntry01.FIRSTNAME || '</FirstName>' ||
                                CHR(10) || '    <LastName>' ||
                                rec_DataEntry01.LASTNAME || '</LastName>' ||
                                CHR(10) || '    <PrimaryEmail>' ||
                                rec_DataEntry01.EMAILADDRESS ||
                                '</PrimaryEmail>' || CHR(10) ||
                                '    <AlternateId>' ||
                                rec_DataEntry01.ALTERNATEID ||
                                '</AlternateId>' || CHR(10) ||
                                '  </Member>' || CHR(10) || '</Members>';
            /* log error */
            utility_pkg.Log_msg(p_messageid => v_messageid,
                                p_envkey    => v_envkey,
                                p_logsource => v_logsource,
                                p_filename  => p_filename,
                                p_batchid   => v_batchid,
                                p_jobnumber => v_my_log_id,
                                p_message   => v_message,
                                p_reason    => v_reason,
                                p_error     => v_error,
                                p_trycount  => v_trycount,
                                p_msgtime   => SYSDATE);
        END;
      END LOOP;
      COMMIT;

    END stage_DataEntry01;
  BEGIN
    /* get job id for this process and the dap process */
    v_my_log_id  := utility_pkg.get_LIBJobID();
    v_dap_log_id := utility_pkg.get_LIBJobID();

    /* initialize some variables based on filename */
    IF UPPER(TRIM(p_filename)) LIKE UPPER('AE_BPCardholder_%.dat%') THEN
      v_jobname       := 'Cardholder01';
      v_logsource     := v_jobname;
      v_membersource  := 14; /* AE Card Holder File */
      v_stage_proc_id := 1;
    ELSIF UPPER(TRIM(p_filename)) LIKE UPPER('ddbp_data_%.txt%') THEN
      v_jobname       := 'DSD01';
      v_logsource     := v_jobname;
      v_membersource  := 8; /* In-Store Registration */
      v_stage_proc_id := 2;
    END IF;
    /* log start of job */
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => p_filename,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => 'stage' || v_jobname);

    /* call sub program that run's data insert */
    IF V_STAGE_PROC_ID = 1 THEN
      stage_Cardholder01();
    ELSIF V_STAGE_PROC_ID = 2 THEN
      stage_DataEntry01();
    ELSE
      raise_application_error(-20001, 'Unreconized file name');
    END IF;

    DECLARE
      /* check log file for errors */
      lv_err VARCHAR2(4000);
      lv_n   NUMBER;
    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM EXT_ENROLL' ||
                        LPAD(to_char(V_STAGE_PROC_ID), 2, '0') || '_LOG' ||
                        CHR(10) || 'WHERE rec LIKE ''ORA-%'''
        INTO lv_n, lv_err;

      IF lv_n > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        v_messagesfailed := v_messagesfailed + lv_n;
        v_reason         := 'Failed reads by external table';
        v_message        := '<StageProc>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>EXT_ENROLL' ||
                            LPAD(to_char(V_STAGE_PROC_ID), 2, '0') ||
                            '</External>' || CHR(10) ||
                            '    <Stage>LW_MEMBERDETAIL_STAGE</Stage>' ||
                            CHR(10) || '    <FileName>' || p_filename ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</StageProc>';
        utility_pkg.Log_msg(p_messageid => v_messageid,
                            p_envkey    => v_envkey,
                            p_logsource => v_logsource,
                            p_filename  => p_filename,
                            p_batchid   => v_batchid,
                            p_jobnumber => v_my_log_id,
                            p_message   => v_message,
                            p_reason    => v_reason,
                            p_error     => lv_err,
                            p_trycount  => lv_n,
                            p_msgtime   => SYSDATE);
      END IF;
    END;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;

    /* log end of job */
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => p_filename,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => 'stage' || v_jobname);

    /* create job for dap */
    utility_pkg.Log_job(P_JOB              => v_dap_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => p_filename,
                        p_starttime        => SYSDATE,
                        p_endtime          => NULL,
                        p_messagesreceived => NULL,
                        p_messagesfailed   => NULL,
                        p_jobstatus        => 0,
                        p_jobname          => 'DAP-' || v_jobname);

    open retval for
      select v_dap_log_id from dual;

  EXCEPTION
    WHEN OTHERS THEN
      IF v_messagesfailed = 0 THEN
        v_messagesfailed := 0 + 1;
      END IF;
      utility_pkg.Log_job(P_JOB              => v_my_log_id,
                          p_jobdirection     => v_jobdirection,
                          p_filename         => p_filename,
                          p_starttime        => v_starttime,
                          p_endtime          => v_endtime,
                          p_messagesreceived => v_messagesreceived,
                          p_messagesfailed   => v_messagesfailed,
                          p_jobstatus        => v_jobstatus,
                          p_jobname          => 'stage' || v_jobname);
      v_messagesfailed := v_messagesfailed + 1;
      v_error          := SQLERRM;
      v_reason         := 'Failed Procedure stage_enroll';
      v_message        := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                          '    <pkg>Stage_Enrollment</pkg>' || CHR(10) ||
                          '    <proc>Stage_Enroll</proc>' || CHR(10) ||
                          '    <filename>' || p_filename || '</filename>' ||
                          CHR(10) || '  </details>' || CHR(10) ||
                          '</failed>';
      /* log error */
      utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => p_filename,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);
      RAISE;
  END stage_enroll;

/********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/
PROCEDURE Update_EnrollDate(p_enrolltype IN VARCHAR2, retval IN OUT rcursor)
IS
 lv_row_count             NUMBER := 0;
 lv_stats_id              NUMBER;
 v_categoryid             NUMBER;
 v_pointtypeid            NUMBER;

    CURSOR curA IS                   /* this selects the members that were staged as part of the POS enrollment*/
        SELECT mds.alternateid, mds.enrolldate, vc.ipcode
          from bp_ae.LW_MEMBERDETAIL_STAGE mds
         inner join bp_ae.lw_virtualcard vc
            on vc.loyaltyidnumber = CAST(mds.alternateid AS VARCHAR(20))
         where 1 = 1
           and trunc(mds.createdate) = trunc(sysdate)
           and mds.membersource = 8;
    TYPE curA_type is TABLE OF curA%ROWTYPE INDEX BY PLS_INTEGER;
    rec1 curA_type;

     CURSOR curB IS             /* this selects the members that were staged as part of the Card enrollment*/
       SELECT mds.alternateid, mds.enrolldate, vc.ipcode
         from bp_ae.LW_MEMBERDETAIL_STAGE mds
        inner join bp_ae.lw_virtualcard vc
           on vc.loyaltyidnumber = CAST(mds.alternateid AS VARCHAR(20))
        where 1 = 1
          and trunc(mds.createdate) = trunc(sysdate)
          and mds.membersource = 14;
    TYPE curB_type is TABLE OF curB%ROWTYPE INDEX BY PLS_INTEGER;
    rec2 curB_type;

BEGIN
  if p_enrolltype = 'POS' then /* enrollment is store/POS */
    OPEN curA;
    LOOP
        FETCH curA BULK COLLECT INTO rec1 LIMIT 1000;
            -- FORALL Update
            FORALL i IN 1 .. rec1.COUNT
            UPDATE bp_ae.lw_loyaltymember lm
               SET lm.membercreatedate = rec1(i).enrolldate
            WHERE lm.ipcode = rec1(i).ipcode;
            lv_row_count := lv_row_count + SQL%ROWCOUNT;
            -- Commit, release lock
            COMMIT WRITE BATCH NOWAIT;
        EXIT WHEN curA%NOTFOUND;

    END LOOP;
    CLOSE curA;
    COMMIT;
End if;
if  p_enrolltype = 'CARD' then

 OPEN curB;
    LOOP
       FETCH curB BULK COLLECT INTO rec2 LIMIT 1000;
            -- FORALL Update
       FORALL i IN 1 .. rec2.COUNT
            UPDATE bp_ae.lw_loyaltymember lm
               SET lm.membercreatedate = rec2(i).enrolldate
            WHERE lm.ipcode = rec2(i).ipcode;
            lv_row_count := lv_row_count + SQL%ROWCOUNT;
            -- Commit, release lock
            COMMIT WRITE BATCH NOWAIT;
        EXIT WHEN curB%NOTFOUND;

    END LOOP;
    CLOSE curB;
    COMMIT;
End if;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END Update_EnrollDate;

/********************************************************************
 ********************************************************************
 ********************************************************************
 ********************************************************************/
PROCEDURE Update_LinkKey(retval IN OUT rcursor)
IS
 lv_row_count             NUMBER := 0;
 lv_stats_id              NUMBER;
 v_categoryid             NUMBER;
 v_pointtypeid            NUMBER;

     CURSOR curB IS             /* this selects the members that were staged as part of the Card enrollment*/
       SELECT vc.ipcode, mds.cid
         from bp_ae.LW_MEMBERDETAIL_STAGE mds
        inner join bp_ae.lw_virtualcard vc
           on vc.loyaltyidnumber = CAST(mds.alternateid AS VARCHAR(20))
        where 1 = 1
          and trunc(mds.createdate) = trunc(sysdate)
          and mds.membersource = 14;
    TYPE curB_type is TABLE OF curB%ROWTYPE INDEX BY PLS_INTEGER;
    rec2 curB_type;

BEGIN
 OPEN curB;
    LOOP
       FETCH curB BULK COLLECT INTO rec2 LIMIT 1000;
            -- FORALL Update
       FORALL i IN 1 .. rec2.COUNT
            UPDATE bp_ae.lw_virtualcard lm
               SET lm.linkkey = rec2(i).cid
            WHERE lm.ipcode = rec2(i).ipcode;
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
END Update_LinkKey;

END Stage_Enrollment;
/

