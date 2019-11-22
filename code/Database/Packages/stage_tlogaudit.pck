CREATE OR REPLACE PACKAGE Stage_TlogAudit IS
type rcursor IS REF CURSOR;

    PROCEDURE Process_TlogAudit (p_filename  IN VARCHAR2, p_processDate IN Date, p_email_list VARCHAR2, retval IN OUT rcursor);
    PROCEDURE Stage_TlogAudit01 (p_filename  IN VARCHAR2, p_processDate IN Date);
    PROCEDURE CopyTlogAudit(p_NewTableName  IN VARCHAR2);
    procedure GenerateEmail(p_email_list VARCHAR2, p_fileDate VARCHAR2);
END Stage_TlogAudit;
/

CREATE OR REPLACE PACKAGE BODY Stage_TlogAudit   IS


/********* Procedure to map external table to specified file ********/
PROCEDURE ChangeExternalTable( p_FileName     IN VARCHAR2)
IS
  e_MTable exception;
  e_MFileName exception;
  v_sql VARCHAR2(400);
BEGIN


    IF LENGTH(TRIM(p_FileName))=0 OR p_FileName is NULL THEN
      raise_application_error(-20001, 'Filename is required to link with external table', FALSE);
    END IF;

    v_sql := 'ALTER TABLE ext_tlogtxn LOCATION (AE_IN'||CHR(58)||''''||p_FileName||''')';
    EXECUTE IMMEDIATE v_sql;

END ChangeExternalTable;

PROCEDURE CopyTlogAudit (p_NewTableName  IN VARCHAR2) IS

  v_SQL 		  VARCHAR2(256);

  BEGIN


 v_SQL := 'Drop Table '||p_NewTableName;
 EXECUTE IMMEDIATE v_SQL;
 v_SQL := 'Create Table '||p_NewTableName||' as select * from ATS_TLOGAUDIT';
 EXECUTE IMMEDIATE v_SQL;


EXCEPTION
WHEN OTHERS THEN
 IF (SQLCODE = -942) THEN
 DBMS_Output.Put_Line (SQLERRM);
 ELSE
 RAISE;
 END IF;
 v_SQL := 'Create Table '||p_NewTableName||' as select * from ATS_TLOGAUDIT';
 EXECUTE IMMEDIATE v_SQL;


END CopyTlogAudit;
 /*********************************************************************************
 ** Process_TlogAudit
 **
 ** This is the main process for handling the Tlog Audit file from AE.	It has 3
 ** separate functions
 ** 1. Stage Tlog Audit - This procedure will get the data from the tlog audit
 **    external table attached to the current tlog audit file and produce the
 **    counts by store
 ** 2. The next step will make a backup of the ats_tlogaudit table for archive
 **    purposes
 ** 3. Then the last step will check to see if any of the counts between AE and BP
 **    are off by more than 10.  Then an email will be sent with those stores and
 **    counts that are greater than 10
 **********************************************************************************/
PROCEDURE Process_TlogAudit (p_filename  IN VARCHAR2, p_processDate Date, p_email_list VARCHAR2,
			 retval 	 IN OUT rcursor)
IS
  v_NewTableName	  VARCHAR2(256);
  v_OverThreshold	  NUMBER;
  v_FileDate		  VARCHAR2(256);

  --log job attributes
  v_my_log_id		  NUMBER;
  v_dap_log_id		  NUMBER;
  v_jobdirection	  NUMBER:=0;
  v_filename		  VARCHAR2(512):=p_filename;
  v_starttime		  DATE:=SYSDATE;
  v_endtime		  DATE;
  v_messagesreceived	  NUMBER:=0;
  v_messagesfailed	  NUMBER:=0;
  v_jobstatus		  NUMBER:=0;
  v_jobname		  VARCHAR2(256);
  V_STAGE_PROC_ID	  NUMBER:=0; /* for this proc only */

  --log msg attributes
  v_messageid	       VARCHAR2(256);
  v_envkey	       VARCHAR2(256):='BP_AE@'||UPPER(sys_context('userenv','instance_name'));
  v_logsource	       VARCHAR2(256);
  v_filename	       VARCHAR2(256):=p_filename ;
  v_batchid	       VARCHAR2(256):=0 ;
  v_message	       VARCHAR2(256) ;
  v_reason	       VARCHAR2(256) ;
  v_error	       VARCHAR2(256) ;
  v_trycount	       NUMBER :=0;

BEGIN
     /* get job id for this process and the dap process */
     v_my_log_id := utility_pkg.get_LIBJobID();
     v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'TlogAudit';
     v_logsource := v_jobname;
     v_stage_proc_id := 1;

  /* log start of job */
  utility_pkg.Log_job(P_JOB		   => v_my_log_id
	 ,p_jobdirection       => v_jobdirection
	 ,p_filename	       => p_filename
	 ,p_starttime	       => v_starttime
	 ,p_endtime	       => v_endtime
	 ,p_messagesreceived   => v_messagesreceived
	 ,p_messagesfailed     => v_messagesfailed
	 ,p_jobstatus	       => v_jobstatus
	 ,p_jobname	       => 'stage'||v_jobname);

   /************* Create Date for ATS_TlogAudit table	 *********************/
   v_FileDate:= substr(p_filename,17,2)||CHR(10)|| '-' ||substr(p_filename,19,2) ||CHR(10)|| '-' ||substr(p_filename,13,4);
   v_NewTableName := 'AE_TlogAudit_' || substr(p_filename,19,2)||substr(p_filename,17,2)||substr(p_filename,13,4);

   /**************************
   ***Stage the Tlog Audit ***
   **************************/
   Stage_TlogAudit01(p_filename, p_processDate);

   /**************************
   ***Copy to backup	   ***
   **************************/
   CopyTlogAudit(v_NewTableName);

   /*************************
   ***Check Threshold	  ***
   **************************/
    select Count(t.a_aestorenumber) into v_OverThreshold
    from ats_tlogaudit t
    where t.a_aetransactioncount != t.a_bptransactioncount
    and (t.a_bptransactioncount - t.a_aetransactioncount) > 10;

    IF v_OverThreshold > 0 THEN
      GenerateEmail(p_email_list, v_FileDate);
    END IF;


  DECLARE/* check log file for errors */
    lv_err   VARCHAR2(4000);
    lv_n     NUMBER;
  BEGIN
    EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)'||CHR(10)||
		      'FROM EXT_TlogTxn_log'||CHR(10)||
		      'WHERE rec LIKE ''ORA-%''' INTO lv_n, lv_err;

    IF lv_n > 0 THEN /* log error msg */
      /* increment jobs fail count */
      v_messagesfailed := v_messagesfailed + lv_n;
      v_reason := 'Failed reads by external table';
      v_message := '<StageProc>'||CHR(10)||
		   '  <Tables>'||CHR(10)||
		   '	<External>EXT_TLOGTXN'||LPAD(to_char(V_STAGE_PROC_ID),2,'0')||'</External>'||CHR(10)||
		   '	<Stage>ATS_TLOGAUDIT</Stage>'||CHR(10)||
		   '	<FileName>'||p_filename||'</FileName>'||CHR(10)||
		   '  </Tables>'||CHR(10)||
		   '</StageProc>';
      utility_pkg.Log_msg(p_messageid	      => v_messageid,
	      p_envkey		  => v_envkey	,
	      p_logsource	  => v_logsource,
	      p_filename	  => p_filename ,
	      p_batchid 	  => v_batchid	,
	      p_jobnumber	  => v_my_log_id,
	      p_message 	  => v_message	,
	      p_reason		  => v_reason	,
	      p_error		  => lv_err    ,
	      p_trycount	  => lv_n ,
	      p_msgtime 	  => SYSDATE  );
    END IF;
  END;

  /* insert here */
  v_endtime := SYSDATE;
  v_jobstatus := 1;

  /* log end of job */
  utility_pkg.Log_job(P_JOB		   => v_my_log_id
	 ,p_jobdirection       => v_jobdirection
	 ,p_filename	       => p_filename
	 ,p_starttime	       => v_starttime
	 ,p_endtime	       => v_endtime
	 ,p_messagesreceived   => v_messagesreceived
	 ,p_messagesfailed     => v_messagesfailed
	 ,p_jobstatus	       => v_jobstatus
	 ,p_jobname	       => 'Stage-'||v_jobname);

  /* create job for dap */
  utility_pkg.Log_job(P_JOB		   => v_dap_log_id
	 ,p_jobdirection       => v_jobdirection
	 ,p_filename	       => p_filename
	 ,p_starttime	       => SYSDATE
	 ,p_endtime	       => NULL
	 ,p_messagesreceived   => NULL
	 ,p_messagesfailed     => NULL
	 ,p_jobstatus	       => 0
	 ,p_jobname	       => 'DAP-'||v_jobname);

	 open retval for select v_dap_log_id from dual;

EXCEPTION
  WHEN OTHERS THEN
  IF v_messagesfailed = 0 THEN

    v_messagesfailed := 0+1;
  END IF;

   utility_pkg.Log_msg(p_messageid	   => v_messageid,
	   p_envkey	       => v_envkey   ,
	   p_logsource	       => v_logsource,
	   p_filename	       => p_filename ,
	   p_batchid	       => v_batchid  ,
	   p_jobnumber	       => v_my_log_id,
	   p_message	       => SQLERRM  ,
	   p_reason	       => v_reason   ,
	   p_error	       => v_error    ,
	   p_trycount	       => v_trycount ,
	   p_msgtime	       => SYSDATE  );
  RAISE;

END Process_TlogAudit;

 /*********************************************************************************
 ** This procedure will read the Audit file from the attached external table
 ** and insert records into the ats_TlogAudit table along with summing the store
 ** counts from the Tlog Staging table (lw_txndetail_stage.  Then it
 **********************************************************************************/
PROCEDURE Stage_TlogAudit01 (p_filename  IN VARCHAR2, p_processDate IN Date)
IS
   /******* Cursor definition **********/
  cursor crsr is
  SELECT substr(A.auditdetail,1,5) as StoreNumber
	 ,substr(A.auditdetail,7,9) as TransactionCount
	 ,substr(A.auditdetail,17,9) as LoyaltyCount
  FROM ext_tlogtxn A where A.recordtype='04';


  v_trun		  VARCHAR2(400); /* for this proc only */
  BPTransactionCount	  Number:=0; /* for this proc only */
  BPLoyaltyCount	  Number:=0; /* for this proc only */
  rCount		  NUMBER:=0; /* for this proc only */
  v_CreateDate		  VARCHAR2(200);/* to set the create date as per file */
  v_partition_name	  VARCHAR2(200);

  BEGIN

      ChangeExternalTable(p_FileName => p_FileName);

 /************* Create Date for ATS_TlogAudit table    *********************/
 v_CreateDate:= substr(p_filename,19,2)||CHR(10)|| '-' ||substr(p_filename,17,2) ||CHR(10)|| '-' ||substr(p_filename,13,4);

  Delete From ats_tlogaudit;
  commit;
   /************* Opening cursor*******************/
  FOR txn_rec in crsr
   LOOP
   DECLARE
      StoreNmbr ats_tlogaudit%rowtype;
      p_storeNumber number := to_number(txn_rec.StoreNumber);
    begin

    /*****************	Checking , If StoreNumber already exists in ATS_TlogAudit table. True: Update ATS_TlogAudit table, False: Insert data to ATS_TlogAudit table   *****************************/
    select count(*) into rCount from ats_tlogaudit  where a_aestorenumber=to_number(txn_rec.StoreNumber);


     IF(rCount!=0) then

     /************ Getting already exist Storenumbers record  ********************/
       select * into StoreNmbr from ats_tlogaudit  where a_aestorenumber=to_number(txn_rec.StoreNumber);


      /*******************  Updating the record   ************************/
       update ats_tlogaudit set a_aetransactioncount=to_number(txn_rec.TransactionCount)+to_number(StoreNmbr.a_aetransactioncount),a_aeloyaltycount=to_number(txn_rec.LoyaltyCount)+to_number(StoreNmbr.a_aeloyaltycount)
       where a_aestorenumber=to_number(StoreNmbr.a_aestorenumber);

    ELSE

    /************ getting BPTransaction counts*******************/
      select count(Distinct txn.txnheaderid) into BPTransactionCount
      from lw_txndetail_stage txn
      where 1=1
      and   txn.storenumber = p_storeNumber
      and   txn.processid not in(102)
      and   trunc(txn.createdate) = p_processDate;
      /************* Getting BPLoyalty counts  **********************/
      select count(Distinct txn.txnheaderid) into BPLoyaltyCount
      from lw_txndetail_stage txn
      where 1=1
      and   txn.storenumber = p_storeNumber
      and   txn.txnloyaltyid is not null
      and   trunc(txn.createdate) = p_processDate;

    /********************** Inserting the record *******************************/
      INSERT INTO ats_tlogaudit(a_rowkey,a_ipcode,a_aestorenumber,a_aetransactioncount,a_aeloyaltycount,a_bpstorenumber,a_bptransactioncount,a_bployaltycount,createdate)
      values(SEQ_ROWKEY.NEXTVAL,'0',txn_rec.StoreNumber,txn_rec.TransactionCount,txn_rec.LoyaltyCount,txn_rec.StoreNumber,BPTransactionCount,BPLoyaltyCount,to_date(v_CreateDate,'dd/mm/yyyy'));
      commit;
    END IF;


    END;
END LOOP;


  END Stage_TlogAudit01;

function BuildEmailHTML (p_fileDate  IN VARCHAR2) return clob is
  v_html_message  clob;
  v_html_program  clob;
  v_html_dtl	  clob;
  v_DayOfWeek	  varchar(10);
  v_DayOfMonth	  varchar(10);
  v_Run 	  varchar(20);
  v_RunHour	  number;
  v_ProcessDate   timestamp(4);
  v_NextRunDate   timestamp(4);

begin


  v_html_message := '<html>'||chr(10)||
		    '<body>'||chr(10)||
		    '<h3>  Tlog Audit Exceptions over 10 ('||p_fileDate||')</h3>'||chr(10)||
		    '<br>' || chr(10)||
		    '<table border = "1" cellpadding = "2">'||chr(10)||
		    '<tr style="background-color:white">' ||chr(10)||
		    '<td><b>Store Number</b></td>' ||chr(10)||
		    '<td><b>AE Count</b></td>' ||chr(10)||
		    '<td><b>BP Count</b></td>' ||chr(10)||
		    '<td><b>Difference</b></td>' ||chr(10)||
		    '</tr>';

  for rec in
    (
      select t.a_aestorenumber as StoreNumber, t.a_aetransactioncount as AETxnCount, t.a_bptransactioncount as BPTxnCount,
	     (t.a_bptransactioncount - t.a_aetransactioncount) as Diff
      from ats_tlogaudit t
      where t.a_aetransactioncount != t.a_bptransactioncount
      and (t.a_bptransactioncount - t.a_aetransactioncount) > 10
    ) loop

    /***************************************************
     ** Daily Jobs that have not run yet, flag them   **
     ***************************************************/
	    v_html_program := v_html_program||chr(10)||
			   '<tr style="background-color:Red;color:White">' ||chr(10)||
			   '<td>'||nvl(rec.StoreNumber,'')||'</td>'||chr(10)||
			   '<td>'||nvl(rec.AETxnCount,'')||'</td>'||chr(10)||
			   '<td>'||nvl(rec.BPTxnCount,'')||'</td>'||chr(10)||
			   '<td>'||nvl(rec.Diff,'')||'</td>'||chr(10)||
			   '</tr>';
  end loop;
 v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
		 '</table>';



  v_html_message := v_html_message||chr(10)||v_html_dtl||chr(10)||
		   '</table>'||chr(10)||
		   '</body>'||chr(10)||
		   '</html>';
  return v_html_message;
end BuildEmailHTML;

procedure GenerateEmail(p_email_list VARCHAR2, p_fileDate VARCHAR2) is
   v_message clob;
   v_attachments cio_mail.attachment_tbl_type;
   v_Dap_Log_Id 	  NUMBER;
  v_Run 	  varchar(20);
  v_RunHour	  number;
begin
   v_message := BuildEmailHTML(p_fileDate);
  v_RunHour := CAST(to_char(sysdate, 'HH24') as number);


   cio_mail.send(
      p_from_email    => 'AEJobs@brierley.com',
      p_from_replyto  => 'ae_jobs@brierley.com',
      p_to_list       => p_email_list,
      p_cc_list       => NULL,
      p_bcc_list      => NULL,
      p_subject       => 'American Eagle Tlog Audit ('||to_char(sysdate, 'mm/dd/yyyy')||')',
      p_text_message  => v_message,
      p_content_type  => 'text/html;charset=UTF8',
      p_attachments   => v_attachments, --v_attachments,
      p_priority      => '3',
      p_auth_username => NULL,
      p_auth_password => NULL,
      p_mail_server   => 'cypwebmail.brierleyweb.com'
   );

end GenerateEmail;

  END Stage_TlogAudit;
/

