CREATE OR REPLACE PACKAGE TlogMonitoring IS

  TYPE Rcursor IS REF CURSOR;

  procedure GenerateEmail(p_date in date default sysdate, p_email_list varchar2,
                                  Retval        IN OUT Rcursor);

END TlogMonitoring;
/

CREATE OR REPLACE PACKAGE BODY TlogMonitoring IS


  /********************************************************************
  ********************************************************************
  ********************************************************************/

procedure BuildWorkTable(p_date in date default sysdate, p_frequency varchar2) is

BEGIN
    EXECUTE IMMEDIATE 'Truncate Table AE_PROCESSES_Wrk';

    Insert Into AE_PROCESSES_Wrk
    select jobnumber as jobnumber,
           Case
             When jobname = 'Stage-TLog01' Then Cast('Tlog Chunk-1 Stage Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog02' Then Cast('Tlog Chunk-2 Stage Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog03' Then Cast('Tlog Chunk-3 Stage Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog04' Then Cast('Tlog Chunk-4 Stage Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog05' Then Cast('Tlog Chunk-5 Stage Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog06' Then Cast('Tlog Chunk-6 Stage Process' as nvarchar2(256))
             Else Cast('Unknown' || '-' || jobname as nvarchar2(256))
           End as JobName
           , Filename
           , messagesreceived as RecordsProcessed
           , messagesfailed as RecordsFailed
           , to_char(starttime, 'MM/dd/yyyy - hh:mi AM') as StartTime
           , to_char(endtime, 'MM/dd/yyyy - hh:mi AM') as EndTime
           , null
    from lw_libjob lj
    where  1=1
    and        trunc(starttime) = trunc(p_date)
    And        jobname in ('Stage-TLog01', 'Stage-Tlog02', 'Stage-Tlog03', 'Stage-Tlog04', 'Stage-Tlog05', 'Stage-Tlog06')
    order by id ;
    commit;

    delete from AE_PROCESSES_Wrk p where p.jobname not in
           (select t.processname
                 from AE_PROCESSES t
                 left join AE_PROCESSES_Wrk p on t.processname = p.jobname
            where 1=1
            and t.frequency = p_frequency);
    commit;


end BuildWorkTable;
  /********************************************************************
  ********************************************************************
  ********************************************************************/
function BuildEmailHTML(p_date in date default sysdate) return clob is
  v_html_message  clob;
  v_html_program  clob;
  v_html_dtl      clob;
  v_Run           varchar(20);
  v_RunHour       number;
  v_ProcessDate   timestamp(4);

begin

  BuildWorkTable(p_date, 'Daily');
  v_ProcessDate := to_date(to_char(p_date, 'mm/dd/yyyy ')||' '||to_char(sysdate, 'hh24:mi:ss'), 'mm/dd/yyyy hh24:mi:ss');
  v_RunHour := CAST(to_char(v_ProcessDate, 'HH24') as number);

  IF v_RunHour < 12 THEN
    v_Run := 'Morning';
  ELSIF v_RunHour < 20 THEN
    v_Run := 'Afternoon';
  ELSE
    v_Run := 'Evening';
  END IF;

  v_html_message := '<html>'||chr(10)||
                    '<body>'||chr(10)||
                    '<h3> American Eagle Tlog Summary ('||to_char(p_date, 'mm/dd/yyyy - DAY - ')||v_Run||'} </h3>'||chr(10)||
                    '<br>' || chr(10)||
                    '<h2> Daily Jobs </h2>'||chr(10)||
                    '<table border = "1" cellpadding = "2">'||chr(10)||
                    '<tr style="background-color:white">' ||chr(10)||
                    '<td><b>Job Number</b></td>' ||chr(10)||
                    '<td><b>Process Name</b></td>' ||chr(10)||
                    '<td><b>Process Type</b></td>' ||chr(10)||
                    '<td><b>File Name</b></td>' ||chr(10)||
                    '<td><b>Records Processed</b></td>' ||chr(10)||
                    '<td><b>Records Failed</b></td>' ||chr(10)||
                    '<td><b>Start Time</b></td>' ||chr(10)||
                    '<td><b>End Time</b></td>' ||chr(10)||
                    '</tr>';

  for rec in
    (
      select p.jobnumber as JobNumber, t.processname as ProcessName, t.processtype as ProcessType
             , p.filename as FileName, p.recordsprocessed as RecordsProcessed, p.recordsfailed as RecordsFailed, p.starttime as StartTime
             , p.endtime as EndTime
      from AE_PROCESSES t
           left join AE_PROCESSES_Wrk p on t.processname = p.jobname
      where t.frequency = 'Daily'
      and t.processname in ('Tlog Chunk-1 Stage Process', 'Tlog Chunk-2 Stage Process', 'Tlog Chunk-3 Stage Process', 'Tlog Chunk-4 Stage Process', 'Tlog Chunk-5 Stage Process', 'Tlog Chunk-6 Stage Process')
      order by t.processname
    ) loop

    /***************************************************
     ** Daily Jobs that have not run yet, flag them   **
     ***************************************************/
      IF rec.jobnumber is null Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>&nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>Job not Run</td>'||chr(10)||
                           '<td>&nbsp;</td>'||chr(10)||
                           '<td>&nbsp;</td>'||chr(10)||
                           '<td>&nbsp;</td>'||chr(10)||
                           '<td>&nbsp;</td>'||chr(10)||
                           '</tr>';
        ELSE
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:white">' ||chr(10)||
                           '<td>'||nvl(rec.JobNumber,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.FileName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(to_char(rec.RecordsProcessed, '9,999,999'),'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.RecordsFailed,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.StartTime,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.EndTime,'')||'</td>'||chr(10)||
                           '</tr>';
      END IF;
  end loop;
  v_html_message := v_html_message||chr(10)||v_html_program||chr(10)||
                   '</table>'||chr(10)||
                   '</body>'||chr(10)||
                   '</html>';
  return v_html_message;
end BuildEmailHTML;




  /********************************************************************
  ********************************************************************
  ********************************************************************/

procedure GenerateEmail(p_date in date default sysdate, p_email_list VARCHAR2,
                                   Retval        IN OUT Rcursor) is
   v_message clob;
   v_attachments cio_mail.attachment_tbl_type;
   v_Dap_Log_Id           NUMBER;
  v_Run           varchar(20);
  v_RunHour       number;
begin
   v_message := BuildEmailHTML(p_date => p_date);
  v_RunHour := CAST(to_char(sysdate, 'HH24') as number);

  IF v_RunHour < 12 THEN
    v_Run := 'Morning';
  ELSIF v_RunHour < 20 THEN
    v_Run := 'Afternoon';
  ELSE
    v_Run := 'Evening';
  END IF;

       /* get job id for this process and the dap process */
    v_Dap_Log_Id := Utility_Pkg.Get_Libjobid();

   cio_mail.send(
      p_from_email    => 'AEJobs@brierley.com',
      p_from_replyto  => 'ae_jobs@brierley.com',
      p_to_list       => p_email_list,
      p_cc_list       => NULL,
      p_bcc_list      => NULL,
      p_subject       => 'American Eagle Tlog Summary ('||to_char(p_date, 'mm/dd/yyyy - DAY - ')||v_Run||')',
      p_text_message  => v_message,
      p_content_type  => 'text/html;charset=UTF8',
      p_attachments   => v_attachments, --v_attachments,
      p_priority      => '3',
      p_auth_username => NULL,
      p_auth_password => NULL,
      p_mail_server   => 'cypwebmail.brierleyweb.com'
   );

   OPEN Retval FOR
      SELECT v_Dap_Log_Id FROM Dual;

end GenerateEmail;



END TlogMonitoring;
/

