CREATE OR REPLACE PACKAGE AE_TERMINATE_MEMBERS IS
	type rcursor IS REF CURSOR;



  PROCEDURE LOAD_TERMINATE_MEMBERS(p_filename VARCHAR2, p_processDate VARCHAR2);

  PROCEDURE clear_infile(p_tablename VARCHAR2);

  PROCEDURE initialize(p_filename VARCHAR2);

  PROCEDURE PROCESS_TERMINATEMEMBERS(p_processDate DATE);

END AE_TERMINATE_MEMBERS;
/
CREATE OR REPLACE PACKAGE BODY AE_TERMINATE_MEMBERS IS

FUNCTION BuildJobSuccessEmailHTML (p_JobNumber  NUMBER, p_procDate DATE)return clob is
  v_html_message  clob;
  lv_recfile NUMBER :=0;
  lv_recproc NUMBER :=0;
  lv_terminated NUMBER :=0;
  lv_test NUMBER :=0;


BEGIN

 SELECT COUNT(*)
 INTO lv_recfile
 FROM ext_terminatemembers;

 SELECT COUNT(*)
 INTO lv_recproc
 FROM ae_terminatemembers t
 WHERE t.terminatedate = p_procDate;


 SELECT COUNT(*)
 INTO lv_test
 FROM ae_terminatemembers t
 WHERE t.terminatedate = p_procDate AND instr( t.notes,'AE for testing') > 0;

 SELECT COUNT(*)
 INTO lv_terminated
 FROM ae_terminatemembers t
 WHERE t.terminatedate = p_procDate AND instr( t.notes,'Member already terminated') > 0 ;



  v_html_message := '<html>'||chr(10)||
        '<body>'||chr(10)||
        'Job#: '||p_JobNumber||chr(10)||
        '<br><br>' || chr(10)||
        '------------------------------------------------'||
        '<table>'||
        '<tr><td>Records Processed:</td><td>'||lv_recproc||'</td></t4>'||
        '<tr><td>Number of records contained in the file:</td><td>'||lv_recfile||'</td></t4>'||
        '<tr><td>Number of records already terminated :</td><td>'||lv_terminated||'</td></t4>'||
        '<tr><td>Number of test accounts terminated :</td><td>'||lv_test||'</td></t4>'||
        '<tr><td>Processing Date/Time:</td><td>'||to_char(p_procDate, 'mm/dd/yyyy hh:mi:ss')||'</td></t4>'||
        '</table>'||
        '<br>' || chr(10);

  v_html_message := v_html_message||chr(10)||'</body>'||chr(10)||'</html>';

  RETURN v_html_message;
END BuildJobSuccessEmailHTML;

  PROCEDURE LOAD_TERMINATE_MEMBERS(p_filename IN VARCHAR2, p_processDate IN VARCHAR2) IS

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
    v_envkey    VARCHAR2(256) := '';
    lv_err VARCHAR2(4000);
    lv_n   NUMBER;
    v_attachments cio_mail.attachment_tbl_type;

   BEGIN
    v_my_log_id     := utility_pkg.get_LIBJobID();
    v_dap_log_id    := utility_pkg.get_LIBJobID();
    v_jobname       := 'TERMINATE_MEMBERS';
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
    IF UPPER(TRIM(p_filename)) LIKE UPPER('TERMINATEMEMBERS%.TXT') THEN
     --  processing the file
     /* initialize, truncates set external table to read p_filename */
      initialize( p_filename);
      /* reset log file, read later for errors */
      clear_infile('ext_terminatemembers_log');
      PROCESS_TERMINATEMEMBERS(v_processDate);

    ELSE

      raise_application_error(-20001, 'Unrecognized file name');
    END IF;

    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM ext_terminatemembers_log' ||
                        CHR(10) || 'WHERE rec LIKE ''ORA-%'''
      INTO lv_n, lv_err;

      IF lv_n > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        v_messagesfailed := v_messagesfailed + lv_n;
        v_reason         := 'Failed reads by external table';
        v_message        := '<TERMINATE_MEMBERS>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>ext_terminatemembers' ||
                            '</External>' || '    <FileName>' || p_filename ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</TERMINATE_MEMBERS>';
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
          p_subject       => 'AE Terminate Member Succes',
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
      v_reason         := 'Failed Procedure TERMINATE_MEMBERS';
      v_message        := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                          '    <pkg>AE_TERMINATE_MEMBERS</pkg>' || CHR(10) ||
                          '    <proc>TERMINATE_MEMBERS</proc>' || CHR(10) ||
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
  END LOAD_TERMINATE_MEMBERS;

  PROCEDURE initialize( p_filename VARCHAR2) IS
    v_partition_name VARCHAR2(256);
    v_sql            VARCHAR2(4000);
    v_inst           VARCHAR2(64) := upper(sys_context('userenv',
                                                       'instance_name'));
  BEGIN
    /*              set the external table filename                                      */
    v_sql := 'ALTER TABLE ext_terminatemembers' || CHR(10) ||
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

PROCEDURE PROCESS_TERMINATEMEMBERS(p_processDate IN DATE) AS

    lc_terminated_value CONSTANT NUMBER := 3;
    lc_notes_text_value NVARCHAR2(512) := '';

     v_rows NUMBER := 0;
     v_exist NUMBER := 0;
     v_agentid NUMBER := 0;

	 CURSOR GET_DATA IS
       SELECT ext.loyaltynumber as loyaltynumber, ext.terminatereasoncode as terminatereasoncode, lm.ipcode, vc.vckey, md.a_membersource, lm.memberstatus, lm.memberclosedate ,1 AS Origin
        FROM ext_terminatemembers ext
        INNER JOIN bp_ae.lw_virtualcard vc ON vc.loyaltyidnumber = ext.loyaltynumber
        INNER JOIN bp_ae.lw_loyaltymember lm ON lm.ipcode = vc.ipcode
        INNER JOIN bp_ae.ats_memberdetails md ON md.a_ipcode = vc.ipcode
       UNION
        SELECT vc.loyaltyidnumber as loyaltynumber,4 as terminatereasoncode, lm.ipcode , vc.vckey, md.a_membersource, lm.memberstatus, lm.memberclosedate ,0 AS Origin
        FROM bp_ae.lw_virtualcard vc
        INNER JOIN bp_ae.lw_loyaltymember lm ON lm.ipcode = vc.ipcode
        INNER JOIN bp_ae.ats_memberdetails md ON md.a_ipcode = vc.ipcode
		    WHERE (upper(lm.firstname) = 'AEO' AND UPPER(lm.lastname) = 'USER') or
                lower(md.a_emailaddress) like '%generated@ae.com' ;



    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;

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
		    IF ( v_tbl(i).memberstatus <> lc_terminated_value) THEN

		            --- expire rewards that are not reedemed yet
                      UPDATE bp_ae.lw_memberrewards mr
                          SET mr.expiration = TRUNC(SYSDATE),
                              mr.changedby  = 'TERMINATE_MEMBERS'
                      WHERE mr.memberid = v_tbl(i).ipcode AND
                            mr.expiration >= TRUNC(p_processDate) AND
                            mr.redemptiondate IS NULL;

                      --- expire membertiers rows

                      UPDATE bp_ae.lw_membertiers mt
                      SET mt.todate = TRUNC(p_processDate),
                          mt.updatedate = TRUNC(p_processDate)
                      WHERE mt.memberid = v_tbl(i).ipcode AND
                            (mt.todate IS NULL OR
                             mt.todate >= TRUNC(p_processDate));

                     -- expire all points
                     UPDATE bp_ae.lw_pointtransaction ptt
                            SET ptt.expirationdate = TRUNC(p_processDate),
                                ptt.notes = ptt.notes || '( expired by TERMINATE_MEMBERS)',
                                ptt.updatedate = TRUNC(p_processDate)
                     WHERE (ptt.expirationdate IS NULL OR ptt.expirationdate >= TRUNC(p_processDate)) AND
                            ptt.points - ptt.pointsconsumed > 0  AND
                            ptt.vckey = v_tbl(i).vckey;

                     -- update memberdetails
                     UPDATE bp_Ae.Ats_Memberdetails md
                            SET --md.a_emailaddress = NULL, --AEO-1037 AH
                              --  md.a_addresslineone = 'Account Terminated',
                              --  md.a_addresslinetwo = 'Account Terminated',
                              --  md.a_city = 'Account Terminated' ,
                                md.a_aitupdate = 1,
                                md.a_membersource =
                                CASE
                                    WHEN md.a_membersource IN  (19,20) THEN 21
                                    ELSE md.a_membersource
                                 END,
                                 md.a_terminationreasonid = v_tbl(i).terminatereasoncode  -- AEO-885
                     WHERE md.a_ipcode = v_tbl(i).ipcode;

                     -- update loyaltymember
                     UPDATE bp_ae.lw_loyaltymember lm
                          SET -- lm.firstname ='Account Terminated',
                              -- lm.lastname  = 'Account Terminated',
                              --lm.primaryemailaddress =NULL, --AEO-1037 AH
                              lm.memberclosedate = TRUNC(p_processDate),
                              lm.memberstatus =  lc_terminated_value
                           WHERE lm.ipcode = v_tbl(i).ipcode;

                     -- create a csnote
				             SELECT COUNT(*)
                     INTO v_exist
                     FROM bp_ae.lw_csnote cs
                     WHERE instr(lower(cs.note),'terminated') > 0 and
                           cs.memberid = v_tbl(i).ipcode;

					           IF (v_exist = 0) THEN
                       --AEO-885 BEGIN
                       --Get the correct description per reason code
                       IF v_tbl(i).terminatereasoncode = 1 THEN --Fraudulent Account
                         lc_notes_text_value :=
                         'As a result of an audit, this member''s Points have been removed and account terminated, as ' ||
                         'authorized by Store Operations Personnel, Store Operations Management and AAP Marketing. ' ||
                         'Please contact your Supervisor with any questions.';
                       ELSIF v_tbl(i).terminatereasoncode = 2 THEN --Customer Requested Termination
                           lc_notes_text_value :=
                           'Member has requested termination of their account, this member''s Points have been removed and ' ||
                           'account terminated, as authorized by Store Operations Personnel, Store Operations Management ' ||
                           'and AAP Marketing. Please contact your Supervisor with any questions.';
                       ELSIF v_tbl(i).terminatereasoncode = 3 THEN --Inactive Account
                           lc_notes_text_value :=
                           'This account has been identified as an inactive account, this member''s Points have been removed ' ||
                           'and account terminated, as authorized by Store Operations Personnel, Store Operations ' ||
                           'Management and AAP Marketing. Please contact your Supervisor with any questions.';
                       ELSIF v_tbl(i).terminatereasoncode = 4 THEN --Test Account
                           lc_notes_text_value :=
                           'This account has been identified as a test account, this member''s Points have been removed and ' ||
                           'account terminated, as authorized by Store Operations Personnel, Store Operations Management ' ||
                           'and AAP Marketing. Please contact your Supervisor with any questions.';
                       ELSIF v_tbl(i).terminatereasoncode = 5 THEN --Other - See Notes
                           lc_notes_text_value :=
                           'Note can be determined when group termination occurs.';
                       ELSE
                           lc_notes_text_value :=
                           'Terminate description is not available';
                       END IF;
                       --AEO-885 END

                       --Insert Note
							        INSERT INTO lw_csnote  (
								           ID, memberid,note,createdate,createdby,deleted,last_dml_id
							        )
							        VALUES (
         								seq_csnote.nextval,
								        v_tbl(i).ipcode,
								        lc_notes_text_value,
								        SYSDATE,v_agentid,0,
								        LAST_DML_ID#.NEXTVAL);
					          END IF;


                    IF (v_tbl(i).origin = 1) then
                      INSERT INTO ae_terminatemembers
                        (loyaltynumber, terminatedate, notes)
                      VALUES ( v_tbl(i).loyaltynumber,
                         p_processDate,
                         'Member included in the list of members to terminate');
                    ELSE
                      INSERT INTO ae_terminatemembers
                        (loyaltynumber, terminatedate, notes)
                      VALUES ( v_tbl(i).loyaltynumber,
                         p_processDate,
                         'Member autogenerated by AE for testing');
                     END IF;

      ELSE
          INSERT INTO ae_terminatemembers
          (loyaltynumber, terminatedate, notes)
          VALUES ( v_tbl(i).loyaltynumber,
               p_processDate,
               'Member already terminated ');

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

 END PROCESS_TERMINATEMEMBERS;

END AE_TERMINATE_MEMBERS;
/
