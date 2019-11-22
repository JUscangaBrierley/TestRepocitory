create or replace package AE_Pilot_Email_Campaign is
  type rcursor IS REF CURSOR;
  procedure generate_root_file(p_dummy in out varchar2,
                               Retval  IN OUT Rcursor);
  procedure generate_child_file(p_StartDate in varchar2,
                                p_EndDate   in varchar2,
                                retval      in out Rcursor);
end AE_Pilot_Email_Campaign;
/
create or replace package body AE_Pilot_Email_Campaign is
  procedure generate_root_file(p_dummy in out varchar2,
                               retval  in out rcursor) is
    --log job attributes
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256);
    v_Processid        NUMBER := 0;
    v_Errormessage     VARCHAR2(256);
    --log msg attributes
    v_Messageid VARCHAR2(256);
    v_Envkey    VARCHAR2(256) := 'bp_ae@' ||
                                 Upper(Sys_Context('userenv',
                                                   'instance_name'));
    v_Logsource VARCHAR2(256);
    v_Batchid   VARCHAR2(256) := 0;
    v_Message   VARCHAR2(256) := ' ';
    v_Reason    VARCHAR2(256);
    v_Error     VARCHAR2(256);
    v_Trycount  NUMBER := 0;
    v_prid      NUMBER := -1;
    v_rwdefid   NUMBER := -1;
    p_StartDate nvarchar2(12);
    p_ExpDate   nvarchar2(12);
    v_temp      number;
    --populate table with all members who's email is valid
    cursor c is
      select *
        from (select distinct lower(md.a_emailaddress) as emailaddress,
                              vc.loyaltyidnumber,
                              lm.membercreatedate,
                              ROW_NUMBER() OVER(PARTITION BY lower(md.a_emailaddress) ORDER BY lower(md.a_emailaddress), lm.membercreatedate desc) AS emp_id
                from bp_ae.ats_memberdetails md
                join bp_ae.lw_virtualcard vc
                  on vc.ipcode = md.a_ipcode
                join bp_ae.lw_loyaltymember lm
                  on lm.ipcode = vc.ipcode
               where 1 = 1
                 and md.a_extendedplaycode in (1, 3)
                 and md.a_emailaddress is not null)
       where emp_id = 1;
    TYPE c_type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
    rec c_type;
  
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('generate_root_file');
    v_Logsource := v_Jobname;
    /* log start of job */
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => NULL,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);
    Utility_Pkg.Log_Process_Start(v_Jobname, v_Jobname, v_Processid);
    
    -- Remove old entries --
    execute immediate 'truncate table x$_aeo_pilot_email_campaign';
    
    open c;
    loop
      fetch c BULK COLLECT
        INTO rec LIMIT 1000;
      for i in 1 .. rec.COUNT loop
        --insert loyalty members into x$_ae_pilot_email_campaign
        insert into bp_ae.x$_aeo_pilot_email_campaign
          (loyaltyidnumber, email_address)
        values
          (rec(i).loyaltyidnumber, rec(i).emailaddress);
      end loop;
      commit;
      exit when c%notfound;
    end loop;
    if c%isopen then
      close c;
    end if;
    v_Endtime   := SYSDATE;
    v_Jobstatus := 1;
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => NULL,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);
  EXCEPTION
    WHEN OTHERS THEN
      v_Error          := SQLERRM;
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => NULL,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                          p_Envkey    => v_Envkey,
                          p_Logsource => 'AE_Pilot_Email_Campaign',
                          p_Filename  => NULL,
                          p_Batchid   => v_Batchid,
                          p_Jobnumber => v_My_Log_Id,
                          p_Message   => v_Message,
                          p_Reason    => v_Reason,
                          p_Error     => v_Error,
                          p_Trycount  => v_Trycount,
                          p_Msgtime   => SYSDATE);
      Raise_Application_Error(-20002,
                              'Other Exception detected in ' || v_Jobname || ' ');
  end generate_root_file;

  --remove loyaltyid's with valid purchases in given date_range    
  procedure generate_child_file(p_StartDate in varchar2,
                                p_EndDate   in varchar2,
                                retval      in out Rcursor) is
  
    --log job attributes
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256);
    v_Processid        NUMBER := 0;
    v_Errormessage     VARCHAR2(256);
    --log msg attributes
    v_Messageid VARCHAR2(256);
    v_Envkey    VARCHAR2(256) := 'bp_ae@' ||
                                 Upper(Sys_Context('userenv',
                                                   'instance_name'));
    v_Logsource VARCHAR2(256);
    v_Batchid   VARCHAR2(256) := 0;
    v_Message   VARCHAR2(256) := ' ';
    v_Reason    VARCHAR2(256);
    v_Error     VARCHAR2(256);
    v_Trycount  NUMBER := 0;
  
    cursor c is
      select *
        from (select distinct lower(md.a_emailaddress) as emailaddress,
                              vc.loyaltyidnumber,
                              lm.membercreatedate,
                              ROW_NUMBER() OVER(PARTITION BY lower(md.a_emailaddress) ORDER BY lower(md.a_emailaddress), lm.membercreatedate desc) AS emp_id
                from bp_ae.ats_memberdetails md
                join bp_ae.lw_virtualcard vc
                  on vc.ipcode = md.a_ipcode
                join bp_ae.lw_loyaltymember lm
                  on lm.ipcode = vc.ipcode
                left outer join bp_ae.ats_txnheader txn
                  on txn.a_vckey = vc.vckey
               where 1 = 1
                 and md.a_extendedplaycode in (1, 3)
                 and md.a_emailaddress is not null
                 and txn.a_txndate between
                     to_date(p_StartDate, 'MM/DD/YYYY') and
                     to_date(p_EndDate, 'MM/DD/YYYY'))
       where emp_id = 1;
    TYPE c_type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
    rec c_type;
  
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('generate_child_file');
    v_Logsource := v_Jobname;
    /* log start of job */
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => NULL,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);
    Utility_Pkg.Log_Process_Start(v_Jobname, v_Jobname, v_Processid);
    open c;
    loop
      fetch c bulk collect
        into rec limit 1000;
      for i in 1 .. rec.COUNT loop
        delete from bp_ae.x$_aeo_pilot_email_campaign pec
         where 1 = 1
           and pec.loyaltyidnumber = rec(i).loyaltyidnumber
           and pec.email_address = rec(i).emailaddress;
      end loop;
      commit;
      exit when c%notfound;
    end loop;
    if c%isopen then
      close c;
    end if;
    v_Endtime   := SYSDATE;
    v_Jobstatus := 1;
    --Logging--
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => NULL,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);
  EXCEPTION
    WHEN OTHERS THEN
      v_Error          := SQLERRM;
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Endtime        := SYSDATE;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => NULL,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);
      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                          p_Envkey    => v_Envkey,
                          p_Logsource => 'AE_Pilot_Email_Campaign',
                          p_Filename  => NULL,
                          p_Batchid   => v_Batchid,
                          p_Jobnumber => v_My_Log_Id,
                          p_Message   => v_Message,
                          p_Reason    => v_Reason,
                          p_Error     => v_Error,
                          p_Trycount  => v_Trycount,
                          p_Msgtime   => SYSDATE);
      Raise_Application_Error(-20002,
                              'Other Exception detected in ' || v_Jobname || ' ');
    
  end generate_child_file;
  --End Generate Child File
end AE_Pilot_Email_Campaign;
/
