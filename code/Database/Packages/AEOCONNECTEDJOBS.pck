CREATE OR REPLACE PACKAGE AEOCONNECTEDJOBS AS

  Gv_Process_Id NUMBER := 0;

  FUNCTION GENERATE_INSERT(pi_schema_txt IN VARCHAR2,
                           pi_table_txt  IN VARCHAR2,
                           pi_column_txt IN VARCHAR2,
                           pi_value_txt  IN VARCHAR2) RETURN CLOB;

  FUNCTION GENERATE_UPDATE(pi_schema_txt IN VARCHAR2,
                           pi_table_txt  IN VARCHAR2,
                           pi_column_txt IN VARCHAR2,
                           pi_value_txt  IN VARCHAR2) RETURN CLOB;
  FUNCTION Get_24hr_Txn_Cnt(p_ipcode IN NUMBER, p_Date IN DATE) RETURN NUMBER;
  PROCEDURE Log_Process_Start(p_JobName   VARCHAR2,
                              p_StepName  VARCHAR2,
                              p_ProcessId OUT NUMBER);
  PROCEDURE Log_Process_Step_End(p_ProcessId NUMBER, p_Rowcount NUMBER);
  PROCEDURE Log_Process_Job_End(p_ProcessId NUMBER, p_Rowcount NUMBER);
  PROCEDURE Log_job(P_JOB              NUMBER,
                    p_jobdirection     NUMBER DEFAULT NULL,
                    p_filename         VARCHAR2 DEFAULT NULL,
                    p_starttime        DATE DEFAULT NULL,
                    p_endtime          DATE DEFAULT NULL,
                    p_messagesreceived NUMBER DEFAULT NULL,
                    p_messagesfailed   NUMBER DEFAULT NULL,
                    p_jobstatus        NUMBER DEFAULT NULL,
                    p_jobname          VARCHAR2 DEFAULT NULL);

  PROCEDURE Log_msg(p_messageid IN OUT VARCHAR2,
                    p_envkey    VARCHAR2 DEFAULT NULL,
                    p_logsource VARCHAR2 DEFAULT NULL,
                    p_filename  VARCHAR2 DEFAULT NULL,
                    p_batchid   VARCHAR2 DEFAULT NULL,
                    p_jobnumber NUMBER DEFAULT NULL,
                    p_message   VARCHAR2 DEFAULT NULL,
                    p_reason    VARCHAR2 DEFAULT NULL,
                    p_error     VARCHAR2 DEFAULT NULL,
                    p_trycount  NUMBER DEFAULT NULL,
                    p_msgtime   DATE DEFAULT SYSDATE);

  FUNCTION get_LIBJobID RETURN NUMBER;

  -- AEO-2054
  PROCEDURE ReactivateUnderAge(p_procdate IN VARCHAR2);

END AEOCONNECTEDJOBS;
/
CREATE OR REPLACE PACKAGE BODY AEOCONNECTEDJOBS AS

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION Get_24hr_Txn_Cnt( /*p_Vckey IN NUMBER,*/p_ipcode IN NUMBER,
                            p_Date   IN DATE) RETURN NUMBER IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    Lv_N1 NUMBER := 0;
    Lv_N2 NUMBER := 0;
    Lv_Ip NUMBER := p_ipcode;
  
    Lv_Log_Cnt NUMBER := 0;
    /*
      to apply txn 24hr rule.
    
      Maintains a log table that should be managed via another process
      old data should not be kept.
    
    */
  BEGIN
    /*    SELECT Ipcode
     INTO Lv_Ip
     FROM Lw_Virtualcard Vc
    WHERE Vc.Vckey = p_Vckey;*/
  
    UPDATE Log_Txn_Counts t
       SET Cnt = Cnt + 1
     WHERE t.Ipcode = Lv_Ip
       AND Sdate = Trunc(p_Date)
    RETURNING Cnt INTO Lv_Log_Cnt;
  
    IF SQL%ROWCOUNT = 0 THEN
      Lv_Log_Cnt := 1;
    
      INSERT INTO Log_Txn_Counts
        (Ipcode, Sdate, Cnt)
      VALUES
        (Lv_Ip, Trunc(p_Date), Lv_Log_Cnt);
    END IF;
  
    COMMIT;
  
    RETURN Lv_Log_Cnt;
  END Get_24hr_Txn_Cnt; /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_Process_Start(p_JobName   VARCHAR2,
                              p_StepName  VARCHAR2,
                              p_ProcessId OUT NUMBER) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    n VARCHAR2(64);
    /*
      This process simply keeps track of the state/step of the main process.
    */
  BEGIN
    SELECT Seq_Rowkey.Nextval INTO p_ProcessId FROM Dual;
  
    insert into log_process
      (id,
       jobname,
       state,
       stepname,
       sid,
       inst_id,
       record_count,
       job_start,
       step_start,
       step_end,
       job_end,
       last_dml_id)
    values
      (p_ProcessId,
       p_jobname,
       'Start',
       p_stepname,
       Sys_Context('userenv', 'sessionid'),
       Sys_Context('userenv', 'instance'),
       0,
       sysdate,
       sysdate,
       null,
       null,
       null);
  
    /* only commits activity in this local proc */
    COMMIT;
  END Log_Process_Start;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_Process_Step_End(p_ProcessId NUMBER, p_Rowcount NUMBER) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    n VARCHAR2(64);
    /*
      This process allows the main process report progress.
    */
  BEGIN
    UPDATE log_process
       SET Step_End     = SYSDATE,
           State        = 'Complete',
           record_count = p_Rowcount
     WHERE Id = p_ProcessId;
    /* only commits activity in this local proc */
    COMMIT;
  END Log_Process_Step_End;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_Process_Job_End(p_ProcessId NUMBER, p_Rowcount NUMBER) IS
    PRAGMA AUTONOMOUS_TRANSACTION; /* <---  allow to only commits activity in this local proc */
    n VARCHAR2(64);
    /*
      This process allows the main process report progress.
    */
  BEGIN
    UPDATE log_process
       SET Job_End      = SYSDATE,
           Step_End     = SYSDATE,
           State        = 'Complete',
           record_count = p_Rowcount
     WHERE Id = p_ProcessId;
    /* only commits activity in this local proc */
    COMMIT;
  END Log_Process_Job_End;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_job(P_JOB              NUMBER,
                    p_jobdirection     NUMBER DEFAULT NULL,
                    p_filename         VARCHAR2 DEFAULT NULL,
                    p_starttime        DATE DEFAULT NULL,
                    p_endtime          DATE DEFAULT NULL,
                    p_messagesreceived NUMBER DEFAULT NULL,
                    p_messagesfailed   NUMBER DEFAULT NULL,
                    p_jobstatus        NUMBER DEFAULT NULL,
                    p_jobname          VARCHAR2 DEFAULT NULL) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    n NUMBER;
  BEGIN
  
    UPDATE lw_libjob
       SET jobdirection     = NVL(p_jobdirection, jobdirection),
           filename         = NVL(p_filename, filename),
           starttime        = NVL(p_starttime, starttime),
           endtime          = NVL(p_endtime, endtime),
           messagesreceived = NVL(p_messagesreceived, messagesreceived),
           messagesfailed   = NVL(p_messagesfailed, messagesfailed),
           jobstatus        = NVL(p_jobstatus, jobstatus),
           createdate       = SYSDATE,
           jobname          = NVL(p_jobname, jobname)
     WHERE jobnumber = p_job;
  
    IF SQL%ROWCOUNT = 0 THEN
      INSERT INTO lw_libjob
        (ID,
         jobnumber,
         jobdirection,
         filename,
         starttime,
         endtime,
         messagesreceived,
         messagesfailed,
         jobstatus,
         createdate,
         jobname)
      VALUES
        (hibernate_sequence.nextval,
         P_JOB,
         p_jobdirection,
         p_filename,
         p_starttime,
         p_endtime,
         p_messagesreceived,
         p_messagesfailed,
         p_jobstatus,
         SYSDATE,
         p_jobname);
    END IF;
  
    COMMIT;
  END Log_job;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  FUNCTION get_LIBJobID RETURN NUMBER IS
    n NUMBER;
  BEGIN
    --get nextvalue and lock row
    SELECT prevvalue
      INTO n
      FROM LW_IDGENERATOR
     WHERE objectname = 'LIBJob'
       FOR UPDATE;
  
    --increment value
    UPDATE LW_IDGENERATOR
       SET prevvalue = prevvalue + incrvalue
     WHERE objectname = 'LIBJob';
  
    --save changes and release lock
    COMMIT;
    RETURN n;
  END get_libjobid;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE Log_msg(p_messageid IN OUT VARCHAR2,
                    p_envkey    VARCHAR2 DEFAULT NULL,
                    p_logsource VARCHAR2 DEFAULT NULL,
                    p_filename  VARCHAR2 DEFAULT NULL,
                    p_batchid   VARCHAR2 DEFAULT NULL,
                    p_jobnumber NUMBER DEFAULT NULL,
                    p_message   VARCHAR2 DEFAULT NULL,
                    p_reason    VARCHAR2 DEFAULT NULL,
                    p_error     VARCHAR2 DEFAULT NULL,
                    p_trycount  NUMBER DEFAULT NULL,
                    p_msgtime   DATE DEFAULT SYSDATE) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    n VARCHAR2(64);
  BEGIN
    --n := NVL(p_messageid,to_char(p_jobnumber) || '|0');
    --n := SUBSTR(n,INSTR(n,'|')+1);
    --p_messageid := p_jobnumber||'|'||to_char(to_number(n) + 1);
  
    INSERT INTO lw_libmessagelog
      (ID,
       envkey,
       logsource,
       filename,
       jobnumber,
       message,
       reason,
       ERROR,
       trycount,
       msgtime)
    VALUES
      (hibernate_sequence.nextval,
       p_envkey,
       p_logsource,
       p_filename,
       p_jobnumber,
       p_message,
       p_reason,
       p_error,
       p_trycount,
       p_msgtime);
    COMMIT;
  END Log_msg;
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE handle_special_char(p_txt IN OUT CLOB) IS
  BEGIN
    /* do  chr(39) first or you'll have problems!!! */
    p_txt := REPLACE(p_txt, CHR(39), '''||chr(39)||''');
    p_txt := REPLACE(p_txt, CHR(38), '''||chr(38)||''');
    p_txt := REPLACE(p_txt, CHR(13), '''||chr(13)||''');
    p_txt := REPLACE(p_txt, CHR(10), '''||chr(10)||''');
    p_txt := REPLACE(p_txt, CHR(1), '''||chr(1)||''');
    p_txt := REPLACE(p_txt, CHR(9), '''||chr(9)||''');
    p_txt := REPLACE(p_txt, CHR(14844070), '...'); --... (elipsis)
  END;

  FUNCTION GENERATE_INSERT(pi_schema_txt IN VARCHAR2,
                           pi_table_txt  IN VARCHAR2,
                           pi_column_txt IN VARCHAR2,
                           pi_value_txt  IN VARCHAR2)
  
   RETURN CLOB AS
    po_statement_txt    CLOB := CHR(10) || 'BEGIN';
    v_declare           VARCHAR(32000) := 'DECLARE';
    v_column_rec        VARCHAR(32000);
    v_data_type_rec     VARCHAR2(50);
    v_columns_txt       VARCHAR2(32000);
    v_values_txt        VARCHAR2(32000);
    v_counter_num       NUMBER := 1;
    v_insert_string_txt VARCHAR2(32000);
    v_values_string_txt VARCHAR2(32000);
    v_value_txt         VARCHAR2(32000);
    v_num               NUMBER;
    v_txt               VARCHAR2(32000);
    v_broken_txt        VARCHAR2(32000);
    v_break_flag        NUMBER := 0;
    V_TEST              VARCHAR2(32000);
    v_is_string         NUMBER := 0;
    --cursor gets column names and datatypes
    CURSOR get_columns_cur IS
      SELECT column_name, data_type
        FROM all_tab_columns
       WHERE owner = UPPER(TRIM(pi_schema_txt))
         AND table_name = UPPER(TRIM(pi_table_txt))
       ORDER BY column_id;
  
  BEGIN
    OPEN get_columns_cur;
    LOOP
    
      FETCH get_columns_cur
        INTO v_column_rec, v_data_type_rec;
      EXIT WHEN get_columns_cur%NOTFOUND;
    
      IF v_insert_string_txt IS NULL THEN
        v_insert_string_txt := 'INSERT INTO ' || pi_schema_txt || '.' ||
                               pi_table_txt || ' (' || v_column_rec;
      ELSE
        v_insert_string_txt := v_insert_string_txt || ',' || v_column_rec;
      END IF;
      v_is_string := 0;
    
      IF v_data_type_rec IN ('FLOAT', 'NUMBER') THEN
        EXECUTE IMMEDIATE 'SELECT ' || v_column_rec || ' FROM ' ||
                          pi_schema_txt || '.' || pi_table_txt || ' WHERE ' ||
                          pi_column_txt || ' = ' || pi_value_txt || ' '
          INTO v_txt;
      
      ELSIF v_data_type_rec = 'DATE' OR v_data_type_rec LIKE 'TIMESTAMP%' THEN
        EXECUTE IMMEDIATE 'SELECT DECODE(' || v_column_rec || ',NULL,NULL,' ||
                          '''TO_DATE(''''''||TO_CHAR(' || v_column_rec ||
                          ', ''MM/DD/YYYY HH24:MI:SS'')||'''''' ,''''MM/DD/YYYY HH24:MI:SS'''')'') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
      
      ELSIF v_data_type_rec LIKE '%CHAR%' THEN
        v_declare := v_declare || CHR(10) || ' ' || v_column_rec ||
                     ' VARCHAR2(32000);';
        EXECUTE IMMEDIATE 'SELECT ' || v_column_rec || ' FROM ' ||
                          pi_schema_txt || '.' || pi_table_txt || ' WHERE ' ||
                          pi_column_txt || ' = ' || pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
        v_is_string := 1;
      ELSIF v_data_type_rec = 'CLOB' THEN
        v_declare := v_declare || CHR(10) || ' ' || v_column_rec ||
                     ' CLOB;';
        EXECUTE IMMEDIATE 'SELECT ' || v_column_rec || ' FROM ' ||
                          pi_schema_txt || '.' || pi_table_txt || ' WHERE ' ||
                          pi_column_txt || ' = ' || pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
        v_is_string := 1;
      ELSIF v_data_type_rec = 'XMLTYPE' THEN
      
        EXECUTE IMMEDIATE 'SELECT TO_CHAR((' || v_column_rec ||
                          ').getClobVal())' || ' FROM ' || pi_schema_txt || '.' ||
                          pi_table_txt || ' WHERE ' || pi_column_txt ||
                          ' = ' || pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
        v_is_string := 1;
      ELSE
        --PRAY IT'S A STRING
        v_declare := v_declare || CHR(10) || ' ' || v_column_rec ||
                     ' VARCHAR2(32000);';
        EXECUTE IMMEDIATE 'SELECT ' || v_column_rec || ' FROM ' ||
                          pi_schema_txt || '.' || pi_table_txt || ' WHERE ' ||
                          pi_column_txt || ' = ' || pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
        v_is_string := 1;
      END IF;
    
      IF v_txt IS NULL THEN
        po_statement_txt := po_statement_txt || CHR(10) || '  ' ||
                            v_column_rec || ' := NULL;';
        v_txt            := v_column_rec;
      ELSIF v_is_string = 1 THEN
        v_broken_txt := NULL;
        FOR i IN 1 .. length(v_txt) LOOP
          IF MOD(i, 2000) = 0 THEN
            v_break_flag := 1;
          END IF;
          IF v_break_flag = 1 AND
             INSTR(SUBSTR(v_txt, (i - 30), 60), '''') = 0 THEN
            po_statement_txt := po_statement_txt || CHR(10) || '  ' ||
                                v_column_rec || ' := ' || v_column_rec ||
                                ' || ''' || v_broken_txt || ''';';
            v_broken_txt     := NULL;
            v_break_flag     := 0;
          END IF;
          v_broken_txt := v_broken_txt || SUBSTR(v_txt, i, 1);
        END LOOP;
        po_statement_txt := po_statement_txt || CHR(10) || '  ' ||
                            v_column_rec || ' := ' || v_column_rec ||
                            ' || ''' || v_broken_txt || ''';';
        v_txt            := v_column_rec;
      END IF;
    
      IF v_values_string_txt IS NULL THEN
        v_values_string_txt := ' VALUES(' || v_txt;
      ELSE
        v_values_string_txt := v_values_string_txt || ', ' || v_txt;
      END IF;
    
      v_counter_num := v_counter_num + 1;
    
    END LOOP;
    po_statement_txt    := v_declare || po_statement_txt;
    v_insert_string_txt := v_insert_string_txt || ') ';
    v_values_string_txt := v_values_string_txt || '); ';
  
    po_statement_txt := po_statement_txt || CHR(10) || v_insert_string_txt ||
                        CHR(10) || v_values_string_txt || CHR(10) || 'END;' ||
                        CHR(10) || '/';
    RETURN po_statement_txt;
  
  END GENERATE_INSERT;

  FUNCTION GENERATE_UPDATE(pi_schema_txt IN VARCHAR2,
                           pi_table_txt  IN VARCHAR2,
                           pi_column_txt IN VARCHAR2,
                           pi_value_txt  IN VARCHAR2)
  
   RETURN CLOB AS
    po_statement_txt VARCHAR2(4000);
  
    v_column_rec        VARCHAR2(50);
    v_data_type_rec     VARCHAR2(50);
    v_columns_txt       VARCHAR2(4000);
    v_values_txt        VARCHAR2(4000);
    v_counter_num       NUMBER := 1;
    v_update_string_txt VARCHAR2(4000);
    v_value_txt         VARCHAR2(200);
    v_num               NUMBER;
    v_txt               VARCHAR2(4000);
  
    --cursor gets column names and datatypes
    CURSOR get_columns_cur IS
      SELECT column_name, data_type
        FROM all_tab_columns
       WHERE owner = UPPER(TRIM(pi_schema_txt))
         AND table_name = UPPER(TRIM(pi_table_txt))
       ORDER BY column_id;
  
  BEGIN
    OPEN get_columns_cur;
    LOOP
    
      FETCH get_columns_cur
        INTO v_column_rec, v_data_type_rec;
      EXIT WHEN get_columns_cur%NOTFOUND;
    
      IF v_data_type_rec = 'NUMBER' THEN
        EXECUTE IMMEDIATE 'SELECT DECODE(' || v_column_rec ||
                          ',NULL,''NULL'',' || v_column_rec || ') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
      
      ELSIF v_data_type_rec = 'DATE' THEN
        EXECUTE IMMEDIATE 'SELECT DECODE(' || v_column_rec ||
                          ',NULL,''NULL'',' || '''TO_DATE(''''''||TO_CHAR(' ||
                          v_column_rec ||
                          ', ''MM/DD/YYYY HH24:MI:SS'')||'''''' ,''''MM/DD/YYYY HH24:MI:SS'''')'') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
      
      ELSIF v_data_type_rec = 'VARCHAR2' THEN
        EXECUTE IMMEDIATE 'SELECT DECODE(' || v_column_rec ||
                          ',NULL,''NULL'',' || '''''''''||REPLACE(' ||
                          v_column_rec ||
                          ',CHR(39),CHR(39)||CHR(39))||'''''''') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
      ELSIF v_data_type_rec = 'CLOB' THEN
      
        EXECUTE IMMEDIATE 'SELECT DECODE(TO_CHAR((' || v_column_rec ||
                          ')),NULL,''NULL'',' ||
                          '''''''''||REPLACE(to_char(' || v_column_rec ||
                          '),CHR(39),CHR(39)||CHR(39))||'''''''') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
      ELSIF v_data_type_rec = 'XMLTYPE' THEN
      
        EXECUTE IMMEDIATE 'SELECT DECODE(TO_CHAR((' || v_column_rec ||
                          ').getClobVal()),NULL,''NULL'',' ||
                          '''''''''||REPLACE((' || v_column_rec ||
                          ').getClobVal(),CHR(39),CHR(39)||CHR(39))||'''''''') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
      ELSE
        --PRAY IT'S A STRING
        EXECUTE IMMEDIATE 'SELECT DECODE(' || v_column_rec ||
                          ',NULL,''NULL'',' || '''''''''||REPLACE(' ||
                          v_column_rec ||
                          ',CHR(39),CHR(39)||CHR(39))||'''''''') ' ||
                          ' FROM ' || pi_schema_txt || '.' || pi_table_txt ||
                          ' WHERE ' || pi_column_txt || ' = ' ||
                          pi_value_txt || ' '
          INTO v_txt;
        handle_special_char(v_txt);
      END IF;
      IF v_update_string_txt IS NULL THEN
        v_update_string_txt := 'UPDATE ' || pi_schema_txt || '.' ||
                               pi_table_txt || ' Set ' || v_column_rec ||
                               ' = ' || v_txt;
      ELSE
        v_update_string_txt := v_update_string_txt || ', ' || v_column_rec ||
                               ' = ' || v_txt;
      END IF;
    
      v_counter_num := v_counter_num + 1;
    
    END LOOP;
  
    v_update_string_txt := v_update_string_txt || ' WHERE ' ||
                           pi_column_txt || ' = ' || pi_value_txt || ';';
  
    RETURN v_update_string_txt;
  
  END GENERATE_UPDATE;

  -- AEO-2054 begin
  PROCEDURE ReactivateUnderAge(p_procdate IN VARCHAR2) IS
  
    v_procdate DATE := to_date(p_procdate, 'MM/DD/YYYY');
     agent_numer number(20);
  
    CURSOR GET_DATA IS
      SELECT lm.ipcode, lm.memberstatus
        FROM bp_ae.lw_loyaltymember lm, bp_ae.ats_memberdetails md
       where md.a_ipcode = lm.ipcode
         and lm.memberstatus in (4)
         and (md.a_isunderage = 1 /*or md.a_isunderage is null*/ )
         and ((v_procdate -
             to_date(to_char(lm.birthdate, 'MM/DD/YYYY'), 'MM/DD/YYYY')) /
             365.242199) >= 15;
  
    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;
  
    V_MY_LOG_ID        NUMBER;
    err_code           VARCHAR2(10) := '';
    err_msg            VARCHAR2(200) := '';
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_MESSAGEID        VARCHAR2(256);
    V_ENVKEY           VARCHAR2(256) := 'BP_AE@' ||
                                        UPPER(SYS_CONTEXT('userenv',
                                                          'instance_name'));
    V_LOGSOURCE        VARCHAR2(256);
    V_FILENAME         VARCHAR2(256) := 'ReactivateUnderAge';
    V_BATCHID          VARCHAR2(256) := 0;
    V_MESSAGE          VARCHAR2(256);
    V_REASON           VARCHAR2(256);
    V_ERROR            VARCHAR2(256);
    V_TRYCOUNT         NUMBER := 0;
    DML_ERRORS EXCEPTION;
  
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);
  
  BEGIN
    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    V_JOBNAME   := 'ReactivateUnderAge';
    V_LOGSOURCE := V_JOBNAME;
  
    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => V_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);
  
    V_ERROR   := ' ';
    V_REASON  := 'ReactivateUnderAge start';
    V_MESSAGE := 'ReactivateUnderAge start';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                        P_ENVKEY    => V_ENVKEY,
                        P_LOGSOURCE => V_LOGSOURCE,
                        P_FILENAME  => NULL,
                        P_BATCHID   => V_BATCHID,
                        P_JOBNUMBER => V_MY_LOG_ID,
                        P_MESSAGE   => V_MESSAGE,
                        P_REASON    => V_REASON,
                        P_ERROR     => V_ERROR,
                        P_TRYCOUNT  => V_TRYCOUNT,
                        P_MSGTIME   => SYSDATE);
                        
    --GET ID FROM USERNAME SYSTEM
   SELECT ca.id
   INTO agent_numer
   from bp_ae.lw_csagent ca
   where lower(ca.username) like '%system%'
   or lower(ca.username) like '%sys_system%';
   
    OPEN GET_DATA;
    LOOP
      FETCH GET_DATA BULK COLLECT
        INTO V_TBL LIMIT 100;
      --Active member  
      FORALL I IN 1 .. V_TBL.COUNT
        UPDATE bp_ae.lw_loyaltymember lm
           SET lm.memberstatus = 1,
           lm.changedby = 'ReactivateUnderAge'
         WHERE lm.ipcode = V_TBL(I).ipcode;
      --create note
      FORALL I IN 1 .. V_TBL.COUNT
       INSERT INTO bp_ae.lw_csnote
        (id, memberid, note, createdby, createdate, deleted, last_dml_id)
        VALUES
        (seq_csnote.nextval,
         V_TBL(I).ipcode,
         'Member status has been set to Active as they now meet the age requirements to participate in the AEO Connected Program',
         agent_numer,
         SYSDATE,
         0,
         LAST_DML_ID#.NEXTVAL);
      --update memberdetails   
      FORALL I IN 1 .. V_TBL.COUNT
       UPDATE bp_ae.ats_memberdetails md set md.a_aitupdate=1, md.a_isunderage=0 ,md.a_changedby='ReactivateUnderAge' WHERE md.a_ipcode = V_TBL(I).ipcode
       and ( md.a_isunderage=1 /*or md.a_isunderage is null aeo-*/ );  
        
      COMMIT WRITE batch NOWAIT;
      EXIT WHEN GET_DATA%NOTFOUND;
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
      CLOSE GET_DATA;
    END IF;
  
    V_ERROR   := ' ';
    V_REASON  := ' ReactivateUnderAge end';
    V_MESSAGE := ' ReactivateUnderAge end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                        P_ENVKEY    => V_ENVKEY,
                        P_LOGSOURCE => V_LOGSOURCE,
                        P_FILENAME  => NULL,
                        P_BATCHID   => V_BATCHID,
                        P_JOBNUMBER => V_MY_LOG_ID,
                        P_MESSAGE   => V_MESSAGE,
                        P_REASON    => V_REASON,
                        P_ERROR     => V_ERROR,
                        P_TRYCOUNT  => V_TRYCOUNT,
                        P_MSGTIME   => SYSDATE);
  
  EXCEPTION
    WHEN OTHERS THEN
      ROLLBACK;
      err_code  := SQLCODE;
      err_msg   := SUBSTR(SQLERRM, 1, 200);
      V_REASON  := 'ReactivateUnderAge' || err_code || ' ' || err_msg;
      V_MESSAGE := v_reason;
      V_ERROR   := v_reason;
    
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
      RAISE;
    
  END ReactivateUnderAge;
  --  AEO-2054 end

END AEOCONNECTEDJOBS;
/
