CREATE OR REPLACE PACKAGE AE_REMOVE_EMPLOYEE IS
	type rcursor IS REF CURSOR;



  PROCEDURE LOAD_REMOVE_EMPLOYEE(p_filename VARCHAR2, p_processDate VARCHAR2);

  PROCEDURE clear_infile(p_tablename VARCHAR2);

  PROCEDURE initialize(p_filename VARCHAR2);

  PROCEDURE PROCESS_REMOVE_EMPLOYEE(p_processDate IN DATE ,p_startdate   IN DATE) ;

END AE_REMOVE_EMPLOYEE;
/
CREATE OR REPLACE PACKAGE BODY AE_REMOVE_EMPLOYEE IS

FUNCTION BuildJobSuccessEmailHTML (p_JobNumber  NUMBER, p_procDate DATE)return clob is

  v_html_message  clob;
  lv_recfile NUMBER :=0;
  lv_recproc NUMBER :=0;

  CURSOR cur IS
      SELECT * FROM bp_ae.ae_rememp re
      WHERE trunc(re.processingdate) = trunc(p_procDate) ;


 TYPE cur_type  is TABLE OF cur%ROWTYPE INDEX BY PLS_INTEGER;
 rec  cur_type;



BEGIN

   v_html_message := '<html>'||chr(10)||
        '<body>'||chr(10)||
        '<table>'||
        '<tr><td>Loyalty Id</td></tr>' ;


   OPEN cur;
   LOOP
     FETCH cur BULK COLLECT INTO rec LIMIT 10000;

	   FOR i IN 1 .. rec.COUNT
		   LOOP
         v_html_message := v_html_message || '<tr><td>' ||
                         rec(i).loyaltynumber ||'</td></tr>' ;
       END LOOP;
      EXIT WHEN cur%NOTFOUND;

   END LOOP;
   CLOSE cur;


  v_html_message := v_html_message||'</table>'||chr(10)||'</body>'||chr(10)||'</html>';

  RETURN v_html_message;
END BuildJobSuccessEmailHTML;

  PROCEDURE LOAD_REMOVE_EMPLOYEE(p_filename IN VARCHAR2,
                                 p_processDate IN VARCHAR2) IS

    v_processid    NUMBER := 0;
    v_my_log_id    NUMBER :=0;
    v_dap_log_id   NUMBER :=0;
    --log job attributes
    v_jobdirection     NUMBER := 0;
    v_filename         VARCHAR2(512) := p_filename;
    v_starttime        DATE := SYSDATE;
    v_endtime          DATE;
    v_messagesreceived NUMBER := 0;
    v_messagesfailed   NUMBER := 0;
    v_jobstatus        NUMBER := 0;
    v_jobname          VARCHAR2(256);
    v_messageid VARCHAR2(256);
    v_logsource VARCHAR2(256);
    v_batchid   VARCHAR2(256) := 0;
    v_message   VARCHAR2(4000);
    v_reason    VARCHAR2(4000);
    v_error     VARCHAR2(4000);
    v_trycount  NUMBER := 0;
    v_processDate DATE := To_Date(p_processDate,'MM/DD/YYYY HH24:MI:SS');
    v_startquarter DATE ;
    v_envkey    VARCHAR2(256) := '';
    lv_err VARCHAR2(4000);
    lv_n   NUMBER;

    v_attachments cio_mail.attachment_tbl_type;

   BEGIN
    v_startquarter  := trunc(v_processDate,'Q');
    v_my_log_id     := utility_pkg.get_LIBJobID();
    v_dap_log_id    := utility_pkg.get_LIBJobID();
    v_jobname       := 'REMOVE_EMPLOYEE';
    v_logsource     := v_jobname;
    v_processid     := 1;
    v_envkey        := 'BP_AE@' || UPPER(sys_context('userenv','instance_name'));

    /* log start of job */
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => p_filename,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    /* call sub program that run's data insert */
    IF UPPER(TRIM(p_filename)) LIKE UPPER('REMOVEEMPLOYEE%.TXT') THEN
     --  processing the file
     /* initialize, truncates set external table to read p_filename */
      initialize( p_filename);
      /* reset log file, read later for errors */
      clear_infile('ext_rememp_log');
      PROCESS_REMOVE_EMPLOYEE(v_processDate, v_startquarter);

    ELSE

      raise_application_error(-20001, 'Unrecognized file name');
    END IF;

    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM ext_rememp_log' ||
                        CHR(10) || 'WHERE rec LIKE ''ORA-%'''
      INTO lv_n, lv_err;

      IF lv_n > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        v_messagesfailed := v_messagesfailed + lv_n;
        v_reason         := 'Failed reads by external table';
        v_message        := '<REMOVE_EMPLOYEE>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>ext_rememp' ||
                            '</External>' || '    <FileName>' || p_filename ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</REMOVE_EMPLOYEE>';
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
                        p_jobname          => v_jobname);

    cio_mail.send(
          p_from_email    => 'AEREWARDS',
          p_from_replyto  => 'info@aerewards.ae.com',
          p_to_list       => 'aerewards_support@brierley.com',
          p_cc_list       => NULL,
          p_bcc_list      => NULL,
          p_subject       => 'AE Remove Employee Succes',
          p_text_message  => BuildJobSuccessEmailHTML( v_my_log_id, v_processDate),
          p_content_type  => 'text/html;charset=UTF8',
          p_attachments   => v_attachments, --v_attachments,
          p_priority      => '3',
          p_auth_username => NULL,
          p_auth_password => NULL,
          p_mail_server   => 'cypwebmail.brierleyweb.com');

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
                          p_jobname          => v_jobname);
      v_messagesfailed := v_messagesfailed + 1;
      v_error          := SQLERRM;
      v_reason         := 'Failed Procedure REMOVE_EMPLOYEE';
      v_message        := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                          '    <pkg>AE_REMOVE_EMPLOYEE</pkg>' || CHR(10) ||
                          '    <proc>PPROCESS_REMOVE_EMPLOYEE</proc>' || CHR(10) ||
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
  END LOAD_REMOVE_EMPLOYEE;

  PROCEDURE initialize( p_filename VARCHAR2) IS
    v_partition_name VARCHAR2(256);
    v_sql            VARCHAR2(4000);
    v_inst           VARCHAR2(64) := upper(sys_context('userenv',
                                                       'instance_name'));
  BEGIN
    /*              set the external table filename                                      */
    v_sql := 'ALTER TABLE ext_rememp' || CHR(10) ||
             'LOCATION (AE_IN' || CHR(58) || '''' || p_filename || ''')';
    EXECUTE IMMEDIATE v_sql;
  END initialize;

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

    UTL_FILE.FCLOSE(lv_fileHandle);

  END clear_infile;

PROCEDURE PROCESS_REMOVE_EMPLOYEE(p_processDate IN DATE ,
                                  p_startdate   IN DATE) AS


   lc_notes_text_value CONSTANT NVARCHAR2(512) := ' Employee Flag Removed';

   v_rows NUMBER := 0;
   v_exist NUMBER := 0;
   v_agentid NUMBER := 0;
   v_non_empl_value  NUMBER:= 0;
   v_addcsnote BOOLEAN := TRUE;

	 CURSOR GET_DATA IS
       SELECT ext.loyaltynumber as loyaltynumber,
               lm.ipcode ,
               vc.vckey,
               md.a_employeecode
        FROM EXT_REMEMP ext
        INNER JOIN bp_ae.lw_virtualcard vc ON vc.loyaltyidnumber = ext.Loyaltynumber
        INNER JOIN bp_ae.lw_loyaltymember lm ON lm.ipcode = vc.ipcode
        INNER JOIN bp_ae.ats_memberdetails md ON md.a_ipcode = vc.ipcode;



    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;
    v_ipcode NUMBER := 0;

  BEGIN

	SELECT COUNT(*)
    INTO v_exist
    FROM bp_ae.lw_csagent ag
    WHERE lower(ag.username) = 'system';

    IF (v_exist <> 1) THEN
         raise_application_error(-20001, 'No system Agent defined');
    ELSE
          SELECT ag.id
          INTO v_agentid
          FROM bp_ae.lw_csagent ag
          WHERE lower(ag.username) = 'system';
    END IF;

    OPEN GET_DATA;
    LOOP
      FETCH GET_DATA BULK COLLECT INTO V_TBL LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
      FOR I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
      LOOP
        v_ipcode := 0;
        v_exist  :=0;
        v_addcsnote := TRUE;

		    IF ( v_tbl(i).a_employeecode <>  v_non_empl_value ) THEN

		            -- update employee flag to non-employee value
                UPDATE ats_memberdetails md
                   SET md.a_employeecode = v_non_empl_value
                WHERE  v_tbl(i).ipcode = md.a_ipcode;


                -- remove employee ID on transaction dates  from the current quarter
                UPDATE ats_txnheader th
                       SET th.a_txnemployeeid = NULL
                WHERE (th.a_txndate BETWEEN p_startdate AND p_processDate) AND
                       th.a_vckey = v_tbl(i).vckey;


                v_exist := 0;

                -- check if the account has been merged and this
                -- if a from account

                SELECT COUNT(*)
                INTO v_exist
                FROM ats_membermergehistory mh
                WHERE mh.a_fromipcode = v_tbl(i).ipcode;

                IF ( v_exist > 0 ) THEN

                    -- get the ipcode of the  memeber
                    -- of the most recent account merged
                    -- and validate that this account was not merged too

                    SELECT t.a_ipcode
                    INTO v_ipcode
                    FROM (
                         SELECT *
                         FROM bp_ae.ats_membermergehistory mh
                         WHERE mh.a_fromipcode =  v_tbl(i).ipcode
                         ORDER BY mh.createdate DESC ) t
                    WHERE ROWNUM = 1;
                                        
                        
                    SELECT COUNT(*)
                    INTO v_exist
                    FROM ats_membermergehistory mh
                    WHERE mh.a_fromipcode = v_ipcode;
                    
                    v_addcsnote := v_exist = 0;
                    

                ELSE
                  v_addcsnote := TRUE;
                  v_ipcode := v_tbl(i).ipcode;
                END IF;
                
                IF (v_addcsnote) THEN
                  
                    -- create a csnote
                    v_exist := 0;

                    SELECT COUNT(*)
                    INTO v_exist
                    FROM lw_csnote nt
                    WHERE nt.memberid = v_ipcode AND  nt.note = lc_notes_text_value;

                    IF ( v_exist = 0  ) THEN
                      INSERT INTO lw_csnote  (
                         ID, memberid,note,createdate,createdby,deleted,last_dml_id
                      )
                      VALUES (seq_csnote.nextval, v_ipcode, lc_notes_text_value,
                              SYSDATE,v_agentid,0, LAST_DML_ID#.NEXTVAL);
                    END IF;
                
                END IF;
            



                INSERT INTO ae_rememp
                  (loyaltynumber, quarterstartdate, processingdate, csnote)
                VALUES ( v_tbl(i).loyaltynumber,  p_startdate, p_processDate,
                   'Member marked as non-employee');

				COMMIT WRITE BATCH NOWAIT;

        END IF;
      END LOOP;
	    COMMIT;

      EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE GET_DATA;
    END IF;

    --- send an e-mail with some numbers

 END PROCESS_REMOVE_EMPLOYEE;

END AE_REMOVE_EMPLOYEE;
/
