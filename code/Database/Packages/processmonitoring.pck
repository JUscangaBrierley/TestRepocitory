CREATE OR REPLACE PACKAGE ProcessMonitoring IS

  TYPE Rcursor IS REF CURSOR;

  procedure GenerateProcessEmail(p_date in date default sysdate, p_email_list varchar2,
                                   Retval        IN OUT Rcursor);
  procedure GenerateNotProcessEmail(p_date in date default sysdate, p_email_list varchar2,
                                   Retval        IN OUT Rcursor);

END ProcessMonitoring;
/

CREATE OR REPLACE PACKAGE BODY ProcessMonitoring IS


  /********************************************************************
  ********************************************************************
  ********************************************************************/

procedure BuildWorkTable(p_date in date default sysdate, p_frequency varchar2) is

BEGIN
    EXECUTE IMMEDIATE 'Truncate Table AE_PROCESSES_Wrk';

    Insert Into AE_PROCESSES_Wrk
    select jobnumber as jobnumber,
           Case
             When jobname = 'RequestCredit_Staging' Then Cast('Request Credit' as nvarchar2(256))
             When jobname = 'LateEnrollmentTxn' Then Cast('Late Enrollment' as nvarchar2(256))
             When jobname = 'DAP-UpdateEmployeeCode' Then Cast('Update Employee Code' as nvarchar2(256))
             When jobname = 'DAP-TLog01' Then Cast('Tlog Chunk-1 DAP Process' as nvarchar2(256))
             When jobname = 'Stage-TLog01' Then Cast('Tlog Chunk-1 Stage Process' as nvarchar2(256))
             When jobname = 'UpdateMemberAttributesFromTlog' And FileName = 'Stage_Tlog.UpdateMemberAttributesFromTlog' Then Cast('Tlog Chunk-1 Update Member Attributes' as nvarchar2(256))
             When jobname = 'Hist_TLog01' Then Cast('Tlog Chunk-1 Post Process' as nvarchar2(256))
             When jobname = 'DAP-Tlog02' Then Cast('Tlog Chunk-2 DAP Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog02' Then Cast('Tlog Chunk-2 Stage Process' as nvarchar2(256))
             When jobname = 'UpdateMemberAttributesFromTlog' And FileName = 'Stage_Tlog2.UpdateMemberAttributesFromTlog' Then Cast('Tlog Chunk-2 Update Member Attributes' as nvarchar2(256))
             When jobname = 'Hist_Tlog02' Then Cast('Tlog Chunk-2 Post Process' as nvarchar2(256))
             When jobname = 'DAP-Tlog03' Then Cast('Tlog Chunk-3 DAP Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog03' Then Cast('Tlog Chunk-3 Stage Process' as nvarchar2(256))
             When jobname = 'UpdateMemberAttributesFromTlog' And FileName = 'Stage_Tlog3.UpdateMemberAttributesFromTlog' Then Cast('Tlog Chunk-3 Update Member Attributes' as nvarchar2(256))
             When jobname = 'Hist_Tlog03' Then Cast('Tlog Chunk-3 Post Process' as nvarchar2(256))
             When jobname = 'DAP-Tlog04' Then Cast('Tlog Chunk-4 DAP Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog04' Then Cast('Tlog Chunk-4 Stage Process' as nvarchar2(256))
             When jobname = 'UpdateMemberAttributesFromTlog' And FileName = 'Stage_Tlog4.UpdateMemberAttributesFromTlog' Then Cast('Tlog Chunk-4 Update Member Attributes' as nvarchar2(256))
             When jobname = 'Hist_Tlog04' Then Cast('Tlog Chunk-4 Post Process' as nvarchar2(256))
             When jobname = 'DAP-Tlog05' Then Cast('Tlog Chunk-5 DAP Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog05' Then Cast('Tlog Chunk-5 Stage Process' as nvarchar2(256))
             When jobname = 'UpdateMemberAttributesFromTlog' And FileName = 'Stage_Tlog5.UpdateMemberAttributesFromTlog' Then Cast('Tlog Chunk-5 Update Member Attributes' as nvarchar2(256))
             When jobname = 'Hist_Tlog05' Then Cast('Tlog Chunk-5 Post Process' as nvarchar2(256))
             When jobname = 'DAP-Tlog06' Then Cast('Tlog Chunk-6 DAP Process' as nvarchar2(256))
             When jobname = 'Stage-Tlog06' Then Cast('Tlog Chunk-6 Stage Process' as nvarchar2(256))
             When jobname = 'UpdateMemberAttributesFromTlog' And FileName = 'Stage_Tlog6.UpdateMemberAttributesFromTlog' Then Cast('Tlog Chunk-6 Update Member Attributes' as nvarchar2(256))
             When jobname = 'Hist_Tlog06' Then Cast('Tlog Chunk-6 Post Process' as nvarchar2(256))
             When jobname = 'Stage-Productfile' Then Cast('Product File' as nvarchar2(256))
             When jobname = 'DAP-Storefile' Then Cast('Store File' as nvarchar2(256))
             When jobname = 'DAP-DSD01' Then Cast('POS Enrollment(DSD)' as nvarchar2(256))
             When jobname = 'VendorEmailSend' Then Cast('Cheetah Mail Send' as nvarchar2(256))
             When jobname = 'DAP-CalculatePointBalances' Then Cast('SMS Mobile Send' as nvarchar2(256))
             When jobname = 'DAP-ProactiveMerge' Then Cast('Proactive Merge' as nvarchar2(256))
             When jobname = 'BonusLoader' And FileName Like 'E:\AmericanEagle\Files\Inbound\Decrypted\AE_Bonus_signin_Mobile%' Then Cast('Bonus Loader - Sign-In' as nvarchar2(256))
             When jobname = 'BonusLoader' And FileName Like 'E:\AmericanEagle\Files\Inbound\Decrypted\AE_Bonus_scan_Mobile%' Then Cast('Bonus Loader - Scan' as nvarchar2(256))
             When jobname = 'BonusLoader' And FileName Like 'E:\AmericanEagle\Files\Inbound\Decrypted\AE_Bonus_purchasetwo_Mobile%' Then Cast('Bonus Loader - Purchase Two' as nvarchar2(256))
             When jobname = 'BonusLoader' And FileName Like 'E:\AmericanEagle\Files\Inbound\Decrypted\AE_Bonus_photoshare_Mobile%' Then Cast('Bonus Loader - Photo Share' as nvarchar2(256))
             When jobname = 'BonusLoader' And FileName Like 'E:\AmericanEagle\Files\Inbound\Decrypted\AE_Bonus_checkin_Mobile%' Then Cast('Bonus Loader - Check-In' as nvarchar2(256))
             When jobname = 'BonusLoader' And FileName Like 'E:\AmericanEagle\Files\Inbound\Decrypted\AE_Bonus_Shopkick_Redemption%' Then Cast('Bonus Loader - Shop Kick Redemption' as nvarchar2(256))
             When jobname = 'TempCardReplaceLoader' Then Cast('Temp Card Load' as nvarchar2(256))
             When jobname = 'CardReplaceLoader' Then Cast('Card Replace Load' as nvarchar2(256))
             When jobname = 'TempCardSend' Then Cast('Temp Card Send' as nvarchar2(256))
             When jobname = 'CardReplaceSend' Then Cast('Card Replace Send' as nvarchar2(256))
             When jobname = 'Stage-ProfileUpdates' Then Cast('Profile Updates Load' as nvarchar2(256))
             When jobname = 'BraFulfillment' Then Cast('Bra Fulfillment' as nvarchar2(256))
             When jobname = 'AIT_Profile_Update_Send' Then Cast('AIT Weekly Profile Updates Send' as nvarchar2(256))
             When jobname = 'VendorEmailUpdate' Then Cast('Vendor Email Update' as nvarchar2(256)) --PI25605
             When jobname = 'DAP-Cardholder01' Then Cast('Enrollment(BPCardholder)' as nvarchar2(256))
             When jobname = 'DAPMemberPointsSend' Then Cast('Member Points Send' as nvarchar2(256))
             When jobname = 'DAPPassBookPointsSend' Then Cast('PassBook Points Send' as nvarchar2(256))
             When jobname = 'WalletUpdatetest' Then Cast('Google Wallet Points Update' as nvarchar2(256))
             When jobname = 'HeaderRules_Processor' Then Cast('Tlog Chunk-1 Header Rule Processor' as nvarchar2 (256))
             When jobname = 'HeaderRules_Processor2' Then Cast('Tlog Chunk-2 Header Rule Processor' as nvarchar2 (256))
             When jobname = 'HeaderRules_Processor3' Then Cast('Tlog Chunk-3 Header Rule Processor' as nvarchar2 (256))
             When jobname = 'HeaderRules_Processor4' Then Cast('Tlog Chunk-4 Header Rule Processor' as nvarchar2 (256))
             When jobname = 'HeaderRules_Processor5' Then Cast('Tlog Chunk-5 Header Rule Processor' as nvarchar2 (256))
             When jobname = 'HeaderRules_Processor6' Then Cast('Tlog Chunk-6 Header Rule Processor' as nvarchar2 (256))
             When jobname = 'DetailItemRules_Processor' Then Cast('Tlog Chunk-1 Detail Rule Processor' as nvarchar2 (256))
             When jobname = 'DetailItemRules_Processor2' Then Cast('Tlog Chunk-2 Detail Rule Processor' as nvarchar2 (256))
             When jobname = 'DetailItemRules_Processor3' Then Cast('Tlog Chunk-3 Detail Rule Processor' as nvarchar2 (256))
             When jobname = 'DetailItemRules_Processor4' Then Cast('Tlog Chunk-4 Detail Rule Processor' as nvarchar2 (256))
             When jobname = 'DetailItemRules_Processor5' Then Cast('Tlog Chunk-5 Detail Rule Processor' as nvarchar2 (256))
             When jobname = 'DetailItemRules_Processor6' Then Cast('Tlog Chunk-6 Detail Rule Processor' as nvarchar2 (256))
             When jobname = 'PointConversionToLegacy' Then Cast('Point Conversion(Pilot to Legacy)' as nvarchar2 (256))
             When jobname = 'PointConversionToPilot' Then Cast('Point Conversion(Legacy to Pilot)' as nvarchar2 (256))
             Else Cast('Unknown' || '-' || jobname as nvarchar2(256))
           End as JobName,
           Case
               When jobname = 'DAP-DSD01' Then (Select Filename from lw_libjob where jobname = 'stageDSD01' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-TLog01' Then (Select Filename from lw_libjob where jobname = 'Stage-TLog01' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog02' Then (Select Filename from lw_libjob where jobname = 'Stage-Tlog02' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog03' Then (Select Filename from lw_libjob where jobname = 'Stage-Tlog03' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog04' Then (Select Filename from lw_libjob where jobname = 'Stage-Tlog04' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog05' Then (Select Filename from lw_libjob where jobname = 'Stage-Tlog05' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog06' Then (Select Filename from lw_libjob where jobname = 'Stage-Tlog06' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Storefile' Then (Select Filename from lw_libjob where jobname = 'Stage-Storefile' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Productfile' Then (Select Filename from lw_libjob where jobname = 'Stage-Productfile' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Cardholder01' Then (Select Filename from lw_libjob where jobname = 'stageCardholder01' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-UpdateEmployeeCode' Then CAST('N/A' AS NVARCHAR2(255))
               When jobname = 'LateEnrollmentTxn' Then CAST('N/A' AS NVARCHAR2(255))
               When jobname = 'UpdateMemberAttributesFromTlog' Then CAST('N/A' AS NVARCHAR2(255))
               When jobname = 'ProcessRequestCreditJob' Then CAST('N/A' AS NVARCHAR2(255))
               When jobname = 'DAP-ProactiveMerge' Then CAST('N/A' AS NVARCHAR2(255))
               When jobname = 'WalletUpdatetest' Then CAST('N/A' AS NVARCHAR2(255))
               Else Replace(filename, 'E:\AmericanEagle\Files\Inbound\Decrypted\', null)
           End as Filename
           ,Case
               When jobname = 'DAP-TLog01' Then (Select messagesreceived from lw_libjob where jobname = 'Hist_TLog01' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog02' Then (Select messagesreceived from lw_libjob where jobname = 'Hist_Tlog02' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog03' Then (Select messagesreceived from lw_libjob where jobname = 'Hist_Tlog03' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog04' Then (Select messagesreceived from lw_libjob where jobname = 'Hist_Tlog04' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog05' Then (Select messagesreceived from lw_libjob where jobname = 'Hist_Tlog05' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog06' Then (Select messagesreceived from lw_libjob where jobname = 'Hist_Tlog06' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-ProactiveMerge' Then
                 (Select count(distinct a_ipcode) from bp_ae.ats_memberproactivemerge mp
                               where mp.a_changedby = 'Merge'
                               and mp.updatedate > to_date(to_char(p_date-1, 'mm/dd/yyyy')||' 11:59:59 am', 'mm/dd/yyyy hh:mi:ss am')
                               and mp.updatedate < to_date(to_char(p_date-1, 'mm/dd/yyyy')||' 11:59:59 pm', 'mm/dd/yyyy hh:mi:ss am')
                  )
               Else messagesreceived
           End as RecordsProcessed
           ,Case
               When jobname = 'DAP-TLog01' Then (Select messagesfailed from lw_libjob where jobname = 'Hist_TLog01' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog02' Then (Select messagesfailed from lw_libjob where jobname = 'Hist_Tlog02' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog03' Then (Select messagesfailed from lw_libjob where jobname = 'Hist_Tlog03' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog04' Then (Select messagesfailed from lw_libjob where jobname = 'Hist_Tlog04' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog05' Then (Select messagesfailed from lw_libjob where jobname = 'Hist_Tlog05' and trunc(starttime) = trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog06' Then (Select messagesfailed from lw_libjob where jobname = 'Hist_Tlog06' and trunc(starttime) = trunc(p_date) and rownum = 1)
               Else messagesfailed
           End as RecordsFailed
           ,Case
               When jobname = 'DAP-TLog01' Then (Select to_char(starttime, 'MM/dd/yyyy - hh:mi AM') from lw_libjob where jobname = 'Stage-TLog01' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog02' Then (Select to_char(starttime, 'MM/dd/yyyy - hh:mi AM') from lw_libjob where jobname = 'Stage-Tlog02' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog03' Then (Select to_char(starttime, 'MM/dd/yyyy - hh:mi AM') from lw_libjob where jobname = 'Stage-Tlog03' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog04' Then (Select to_char(starttime, 'MM/dd/yyyy - hh:mi AM') from lw_libjob where jobname = 'Stage-Tlog04' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog05' Then (Select to_char(starttime, 'MM/dd/yyyy - hh:mi AM') from lw_libjob where jobname = 'Stage-Tlog05' and trunc(starttime) =trunc(p_date) and rownum = 1)
               When jobname = 'DAP-Tlog06' Then (Select to_char(starttime, 'MM/dd/yyyy - hh:mi AM') from lw_libjob where jobname = 'Stage-Tlog06' and trunc(starttime) =trunc(p_date) and rownum = 1)
               Else to_char(starttime, 'MM/dd/yyyy - hh:mi AM')
           End as StartTime
           , to_char(endtime, 'MM/dd/yyyy - hh:mi AM') as EndTime
           , null
    from lw_libjob lj
    where  1=1
    and        trunc(starttime) = trunc(p_date)
    And        jobname not in ('UpdateEmployeeCode', 'DAPTlogPOSTProcess', 'Stage-Storefile', 'Stage-CalculatePointBalances', 'stageDSD01', 'BraRewards', 'ResetAITUpdateFlag', 'stageCardholder01')
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
  v_DayOfWeek     varchar(10);
  v_DayOfMonth    varchar(10);
  v_Run           varchar(20);
  v_RunHour       number;
  v_ProcessDate   timestamp(4);
  v_NextRunDate   timestamp(4);
  v_JobNameH VARCHAR2(50) := 'HeaderRules_Processor';
  v_JobNameD VARCHAR2(50) := 'DetailItemRules_Processor';
  v_JobName nvarchar2(100);

  TYPE Heads IS TABLE OF NUMBER INDEX BY BINARY_INTEGER;
  v_existH Heads;

  TYPE Details IS TABLE OF NUMBER INDEX BY BINARY_INTEGER;
  v_existD Details;
begin

  BuildWorkTable(p_date, 'Daily');
  v_DayOfWeek := to_char(p_date, 'D');
  v_DayOfMonth := to_char(p_date, 'dd');
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
                    '<h3> American Eagle Job Summary ('||to_char(p_date, 'mm/dd/yyyy - DAY - ')||v_Run||'} </h3>'||chr(10)||
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
      order by p.jobnumber
    ) loop

    /***************************************************
     ** Daily Jobs that have not run yet, flag them   **
     ***************************************************/
      IF rec.processname in ('Tlog Chunk-1 Header Rule Processor', 'Tlog Chunk-2 Header Rule Processor', 'Tlog Chunk-3 Header Rule Processor', 'Tlog Chunk-4 Header Rule Processor', 'Tlog Chunk-5 Header Rule Processor', 'Tlog Chunk-6 Header Rule Processor', 'Tlog Chunk-1 Detail Rule Processor', 'Tlog Chunk-2 Detail Rule Processor', 'Tlog Chunk-3 Detail Rule Processor', 'Tlog Chunk-4 Detail Rule Processor', 'Tlog Chunk-5 Detail Rule Processor', 'Tlog Chunk-6 Detail Rule Processor', 'POS Enrollment(DSD)','Store File','SMS Mobile Send','Cheetah Mail Send','Request Credit','Product File') And rec.jobnumber is null Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>Job not Run</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '</tr>';
    /************************************************************************
     ** Daily Jobs that run in the afternoon have not run yet, flag them   **
     ************************************************************************/
        ELSIF rec.processname in ('Request Credit', 'Late Enrollment', 'Product File', 'Proactive Merge', 'Enrollment(BPCardholder)', 'Profile Updates Load', 'Point Conversion(Pilot to Legacy)', 'Point Conversion(Legacy to Pilot)') And rec.jobnumber is null And v_Run = 'Evening' Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>Job not Run</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
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
 v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
                 '</table>';

/*AEO-754 BEGIN*/
  v_html_program := null;

  FOR I IN 1..6
  LOOP
    IF I = 1 THEN
       v_JobName := v_JobNameH;
    ELSE
       v_JobName := v_JobNameH || I;
    END IF;

     EXECUTE IMMEDIATE 'begin select count(*) into :1 from bp_ae.lw_libjob lb where lb.jobname = :2 and lb.createdate between to_date(to_char(trunc(sysdate), ''mm/dd/yyyy'') || ''00:00:00'', ''mm/dd/yyyy HH24:MI:SS'') and to_date(to_char(trunc(sysdate), ''mm/dd/yyyy'') || ''06:00:00'', ''mm/dd/yyyy HH24:MI:SS''); end;'
     USING OUT v_existH(I), v_JobName;

    IF I = 1 THEN
       v_JobName := v_JobNameD;
    ELSE
       v_JobName := v_JobNameD || I;
    END IF;

     EXECUTE IMMEDIATE 'begin select count(*) into :1 from bp_ae.lw_libjob lb where lb.jobname = :2 and lb.createdate between to_date(to_char(trunc(sysdate), ''mm/dd/yyyy'') || ''00:00:00'', ''mm/dd/yyyy HH24:MI:SS'') and to_date(to_char(trunc(sysdate), ''mm/dd/yyyy'') || ''06:00:00'', ''mm/dd/yyyy HH24:MI:SS''); end;'
     USING OUT v_existD(I), v_JobName;
  END LOOP;

  IF v_existH(1) = 0 OR v_existH(2) = 0 OR v_existH(3) = 0 OR v_existH(4) = 0 OR v_existH(5) = 0 OR v_existH(6) = 0 OR
    v_existD(1) = 0 OR v_existD(2) = 0 OR v_existD(3) = 0 OR v_existD(4) = 0 OR v_existD(5) = 0 OR v_existD(6) = 0 THEN

    v_html_message := v_html_message||chr(10)||'<h2> Missing Jobs </h2>'||chr(10)||
                      '<table border = "1" cellpadding = "2">'||chr(10)||
                      '<tr style="background-color:white">' ||chr(10)||
                      '<td><b>Process Header Name</b></td>' ||chr(10)||
                      '<td><b>Process Detail Name</b></td>' ||chr(10)||
                      '</tr>';

    FOR I IN 1..6
    LOOP
      IF v_existH(I) = 0 AND v_existD(I) = 0 THEN
         v_html_program := v_html_program||chr(10)||
                 '<tr style="background-color:Yellow;color:Black">' ||chr(10)||
                 '<td>Tlog Chunk-'||I||' Header Rule Processor</td>'||chr(10)||
                 '<td>Tlog Chunk-'||I||' Detail Rule Processor</td>'||chr(10)||
                 '</tr>';
      ELSIF v_existH(I) = 0 AND v_existD(I) <> 0 THEN
         v_html_program := v_html_program||chr(10)||
                 '<tr style="background-color:Yellow;color:Black">' ||chr(10)||
                 '<td>Tlog Chunk-'||I||' Header Rule Processor</td>'||chr(10)||
                 '<td>'||'&'||'nbsp;</td>'||chr(10)||
                 '</tr>';
      ELSIF v_existH(I) <> 0 AND v_existD(I) = 0 THEN
         v_html_program := v_html_program||chr(10)||
                 '<tr style="background-color:Yellow;color:Black">' ||chr(10)||
                 '<td>'||'&'||'nbsp;</td>'||chr(10)||
                 '<td>Tlog Chunk-'||I||' Detail Rule Processor</td>'||chr(10)||
                 '</tr>';
      END IF;
    END LOOP;

    v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)|| '</table>';
  END IF;
/*AEO-754 END*/

/**************
**Weekly Jobs**
***************/

  BuildWorkTable(p_date, 'Weekly');

  v_html_message := v_html_message||chr(10)||'<h2> Weekly Jobs </h2>'||chr(10)||
                    '<table border = "1" cellpadding = "2">'||chr(10)||
                    '<tr style="background-color:white">' ||chr(10)||
                    '<td><b>Job Number</b></td>' ||chr(10)||
                    '<td><b>Process Name</b></td>' ||chr(10)||
                    '<td><b>Process Type</b></td>' ||chr(10)||
                    '<td><b>File Name</b></td>' ||chr(10)||
                    '<td><b>Records Processed</b></td>' ||chr(10)||
                    '<td><b>Records Failed</b></td>' ||chr(10)||
                    '<td><b>Process Day</b></td>' ||chr(10)||
                    '<td><b>Start Time</b></td>' ||chr(10)||
                    '<td><b>End Time</b></td>' ||chr(10)||
                    '</tr>';

  v_html_program := null;

  for rec in
    (
      select p.jobnumber as JobNumber, t.processname as ProcessName, t.processtype as ProcessType
             , p.filename as FileName, p.recordsprocessed as RecordsProcessed, p.recordsfailed as RecordsFailed, t.dayofweek as DayOfWeek, p.starttime as StartTime
             , p.endtime as EndTime
      from AE_PROCESSES t
           left join AE_PROCESSES_Wrk p on t.processname = p.jobname
      where t.frequency = 'Weekly'
      order by t.id
    ) loop

    /******************************************************************
     ** Weekly Jobs that run on Friday have not run yet, flag them   **
     ******************************************************************/
      IF rec.processname in ('AIT Weekly Profile Updates Send', 'Vendor Email Update') And rec.jobnumber is null and v_DayOfWeek = '6' Then -- PI25605
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>Job not Run</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.dayofweek,'')||'</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
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
                           '<td>'||nvl(rec.dayofweek,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.StartTime,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.EndTime,'')||'</td>'||chr(10)||
                           '</tr>';
        END IF;
  end loop;
 v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
                 '</table>';

/*****************
**Bi-Weekly Jobs**
******************/

  BuildWorkTable(p_date, 'Bi-Weekly');

  v_html_message := v_html_message||chr(10)||'<h2> Bi-Weekly Jobs </h2>'||chr(10)||
                    '<table border = "1" cellpadding = "2">'||chr(10)||
                    '<tr style="background-color:white">' ||chr(10)||
                    '<td><b>Job Number</b></td>' ||chr(10)||
                    '<td><b>Process Name</b></td>' ||chr(10)||
                    '<td><b>Process Type</b></td>' ||chr(10)||
                    '<td><b>File Name</b></td>' ||chr(10)||
                    '<td><b>Records Processed</b></td>' ||chr(10)||
                    '<td><b>Records Failed</b></td>' ||chr(10)||
                    '<td><b>Process Day</b></td>' ||chr(10)||
                    '<td><b>Start Time</b></td>' ||chr(10)||
                    '<td><b>End Time</b></td>' ||chr(10)||
                    '<td><b>Next Run Date</b></td>' ||chr(10)||
                    '</tr>';

  v_html_program := null;

  for rec in
    (
      select p.jobnumber as JobNumber, t.processname as ProcessName, t.processtype as ProcessType
             , p.filename as FileName, p.recordsprocessed as RecordsProcessed, p.recordsfailed as RecordsFailed, t.dayofweek as DayOfWeek, p.starttime as StartTime
             , p.endtime as EndTime, t.lastrundate+14 as NextRunDate
      from AE_PROCESSES t
           left join AE_PROCESSES_Wrk p on t.processname = p.jobname
      where t.frequency = 'Bi-Weekly'
      order by t.id
    ) loop

    /*********************************************************************
     ** Bi_Weekly Jobs that run on Monday have not run yet, flag them   **
     *********************************************************************/
      IF rec.processname in ('Temp Card Send','Card Replace Send') And rec.jobnumber is null and v_DayOfWeek = '2' and trunc(rec.nextrundate) = trunc(p_date) Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>Job not Run</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.dayofweek,'')||'</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(to_char(rec.NextRunDate, 'mm/dd/yyyy'),'')||'</td>'||chr(10)||
                           '</tr>';
    /*******************************************************************************
     ** Bi_Weekly Jobs that run on the 1st and 15th have not run yet, flag them   **
     *******************************************************************************/
        ELSIF rec.processname in ('Bra Fulfillment') And rec.jobnumber is null and v_DayOfMonth in ('8', '21') Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>Job not Run</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||nvl(rec.dayofweek,'')||'</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '<td>'||'&'||'nbsp;</td>'||chr(10)||
                           '</tr>';
        ELSE
             v_NextRunDate := rec.nextrundate;
             IF rec.processname  = 'Temp Card Send' And rec.jobnumber is not null and v_DayOfWeek = '2' and trunc(rec.nextrundate) = trunc(p_date) Then
               Update ae_processes p
               Set p.lastrundate = p_date
               Where p.processname = 'Temp Card Send';
               commit;
               v_NextRunDate := rec.nextrundate+14;
             END IF;

             IF rec.processname  = 'Card Replace Send' And rec.jobnumber is not null and v_DayOfWeek = '2' and trunc(rec.nextrundate) = trunc(p_date) Then
               Update ae_processes p
               Set p.lastrundate = p_date
               Where p.processname = 'Card Replace Send';
               commit;
               v_NextRunDate := rec.nextrundate+14;
             END IF;
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:white">' ||chr(10)||
                           '<td>'||nvl(rec.JobNumber,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.FileName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(to_char(rec.RecordsProcessed, '9,999,999'),'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.RecordsFailed,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.dayofweek,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.StartTime,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.EndTime,'')||'</td>'||chr(10)||
                           '<td>'||nvl(to_char(v_NextRunDate, 'mm/dd/yyyy'),'')||'</td>'||chr(10)||
                           '</tr>';

        END IF;
  end loop;

 v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
                 '</table>';

  v_html_message := v_html_message||chr(10)||v_html_dtl||chr(10)||
                   '</table>'||chr(10)||
                   '</body>'||chr(10)||
                   '</html>';
  return v_html_message;
end BuildEmailHTML;


function BuildNotProcessedEmailHTML(p_date in date default sysdate) return clob is
  v_html_message  clob;
  v_html_program  clob;
  v_html_dtl      clob;
  v_DayOfWeek     varchar(10);
  v_DayOfMonth    varchar(10);
  v_Run           varchar(20);
  v_RunHour       number;
  v_ProcessDate   timestamp(4);
  v_NextRunDate   timestamp(4);
  v_Count         number;

begin

  BuildWorkTable(p_date, 'Daily');
  v_DayOfWeek := to_char(p_date, 'D');
  v_DayOfMonth := to_char(p_date, 'dd');
  v_ProcessDate := to_date(to_char(p_date, 'mm/dd/yyyy ')||' '||to_char(sysdate, 'hh24:mi:ss'), 'mm/dd/yyyy hh24:mi:ss');
  v_RunHour := CAST(to_char(v_ProcessDate, 'HH24') as number);

  IF v_RunHour < 12 THEN
    v_Run := 'Morning';
  ELSIF v_RunHour < 20 THEN
    v_Run := 'Afternoon';
  ELSE
    v_Run := 'Evening';
  END IF;

  v_Count := 0;

    v_html_message := '<html>'||chr(10)||
                    '<body>'||chr(10)||
                    '<h3> American Eagle Jobs Not Run Today ('||to_char(p_date, 'mm/dd/yyyy - DAY - ')||v_Run||'} </h3>'||chr(10)||
                    '<br>' || chr(10)||
                    '<h2> Daily Jobs </h2>'||chr(10)||
                    '<table border = "1" cellpadding = "2">'||chr(10)||
                    '<tr style="background-color:white">' ||chr(10)||
                    '<td><b>Process Name</b></td>' ||chr(10)||
                    '<td><b>Process Type</b></td>' ||chr(10)||
                    '</tr>';


  for rec in
    (
      select p.jobnumber, t.processname as ProcessName, t.processtype as ProcessType
      from AE_PROCESSES t
           left join AE_PROCESSES_Wrk p on t.processname = p.jobname
      where t.frequency = 'Daily'
      order by t.id
    ) loop

    /***************************************************
     ** Daily Jobs that have not run yet, flag them   **
     ***************************************************/
      IF rec.processname in ('TLog Process', 'POS Enrollment(DSD)','Store File','SMS Mobile Send','Cheetah Mail Send') And rec.jobnumber is null Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '</tr>';
            v_Count := v_Count + 1;
    /************************************************************************
     ** Daily Jobs that run in the afternoon have not run yet, flag them   **
     ************************************************************************/
        ELSIF rec.processname in ('Request Credit', 'Late Enrollment', 'Product File', 'Proactive Merge') And rec.jobnumber is null And v_Run = 'Evening' Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '</tr>';
            v_Count := v_Count + 1;
      END IF;
  end loop;

 v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
                 '</table>';
/**************
**Weekly Jobs**
***************/

  BuildWorkTable(p_date, 'Weekly');

---- AEO-667: Removing the Enrollment(BPCardholder) but leaving the code as an example. JHC
/*

  v_html_message := v_html_message||chr(10)||'<h2> Weekly Jobs </h2>'||chr(10)||
                    '<table border = "1" cellpadding = "2">'||chr(10)||
                    '<tr style="background-color:white">' ||chr(10)||
                    '<td><b>Process Name</b></td>' ||chr(10)||
                    '<td><b>Process Type</b></td>' ||chr(10)||
                    '</tr>';

  v_html_program := null;

  for rec in
    (
      select p.jobnumber, t.processname as ProcessName, t.processtype as ProcessType
      from AE_PROCESSES t
           left join AE_PROCESSES_Wrk p on t.processname = p.jobname
      where t.frequency = 'Weekly'
      order by t.id
    ) loop

    \*********************************************************************************
     ** Weekly Jobs that run on Friday in the evening have not run yet, flag them   **
     *********************************************************************************\
        IF rec.processname in ('Enrollment(BPCardholder)') And rec.jobnumber is null and v_DayOfWeek = '6' And v_Run = 'Evening' Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '</tr>';
            v_Count := v_Count + 1;
        END IF;
  end loop;
 v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
                 '</table>';
*/

/*****************
**Bi-Weekly Jobs**
******************/

  BuildWorkTable(p_date, 'Bi-Weekly');

  v_html_message := v_html_message||chr(10)||'<h2> Bi-Weekly Jobs </h2>'||chr(10)||
                    '<table border = "1" cellpadding = "2">'||chr(10)||
                    '<tr style="background-color:white">' ||chr(10)||
                    '<td><b>Process Name</b></td>' ||chr(10)||
                    '<td><b>Process Type</b></td>' ||chr(10)||
                    '</tr>';

  v_html_program := null;

  for rec in
    (
      select p.jobnumber, t.processname as ProcessName, t.processtype as ProcessType, t.lastrundate+14 as NextRunDate
      from AE_PROCESSES t
           left join AE_PROCESSES_Wrk p on t.processname = p.jobname
      where t.frequency = 'Bi-Weekly'
      order by t.id
    ) loop

    /*********************************************************************
     ** Bi_Weekly Jobs that run on Monday have not run yet, flag them   **
     *********************************************************************/
      IF rec.processname in ('Temp Card Send','Card Replace Send') And rec.jobnumber is null and v_DayOfWeek = '2' and trunc(rec.nextrundate) = trunc(p_date) Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '</tr>';
            v_Count := v_Count + 1;
    /*******************************************************************************
     ** Bi_Weekly Jobs that run on the 1st and 15th have not run yet, flag them   **
     *******************************************************************************/
      ELSIF rec.processname in ('Bra Fulfillment') And rec.jobnumber is null and v_DayOfMonth in ('8', '21') Then
            v_html_program := v_html_program||chr(10)||
                           '<tr style="background-color:Red;color:White">' ||chr(10)||
                           '<td>'||nvl(rec.ProcessName,'')||'</td>'||chr(10)||
                           '<td>'||nvl(rec.ProcessType,'')||'</td>'||chr(10)||
                           '</tr>';
            v_Count := v_Count + 1;
      END IF;
  end loop;

  If v_Count = 0 Then
    v_html_message := null;
  Else
     v_html_message:=v_html_message||chr(10)||v_html_program||chr(10)||
                 '</table>';

     v_html_message := v_html_message||chr(10)||v_html_dtl||chr(10)||
                   '</table>'||chr(10)||
                   '</body>'||chr(10)||
                   '</html>';
  End If;
  return v_html_message;
end BuildNotProcessedEmailHTML;

  /********************************************************************
  ********************************************************************
  ********************************************************************/

procedure GenerateProcessEmail(p_date in date default sysdate, p_email_list VARCHAR2,
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
      p_subject       => 'American Eagle Job Summary ('||to_char(p_date, 'mm/dd/yyyy - DAY - ')||v_Run||')',
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

end GenerateProcessEmail;

procedure GenerateNotProcessEmail(p_date in date default sysdate, p_email_list VARCHAR2,
                                   Retval        IN OUT Rcursor) is
   v_message clob;
   v_attachments cio_mail.attachment_tbl_type;
   v_Dap_Log_Id           NUMBER;
  v_Run           varchar(20);
  v_RunHour       number;
begin
   v_message := BuildNotProcessedEmailHTML(p_date => p_date);
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

   If v_message is not null Then
     cio_mail.send(
        p_from_email    => 'AEJobs@brierley.com',
        p_from_replyto  => 'ae_jobs@brierley.com',
        p_to_list       => p_email_list,
        p_cc_list       => NULL,
        p_bcc_list      => NULL,
        p_subject       => 'American Eagle Jobs Not Run ('||to_char(p_date, 'mm/dd/yyyy - DAY - ')||v_Run||')',
        p_text_message  => v_message,
        p_content_type  => 'text/html;charset=UTF8',
        p_attachments   => v_attachments, --v_attachments,
        p_priority      => '3',
        p_auth_username => NULL,
        p_auth_password => NULL,
        p_mail_server   => 'cypwebmail.brierleyweb.com'
     );
   End If;

   OPEN Retval FOR
      SELECT v_Dap_Log_Id FROM Dual;

end GenerateNotProcessEmail;

END ProcessMonitoring;
/

