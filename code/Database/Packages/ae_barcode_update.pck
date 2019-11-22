create or replace package AE_BARCODE_UPDATE is

  TYPE Rcursor IS REF CURSOR;
  PROCEDURE main(p_badAuth  in VARCHAR2,
                 p_goodAuth in varchar2,
                 Retval     IN OUT Rcursor);

  PROCEDURE Select_AffectedPopulation(p_badAuth in varchar2);

  PROCEDURE Select_ValidBarcodes(p_goodAuth in varchar2, p_codeLimit in number);

  PROCEDURE Assign_Barcodes;

  PROCEDURE consume_ValidBarcodes;

  PROCEDURE update_MemberRewards;

  --  PROCEDURE Award_Export_Table_QAD;
  PROCEDURE award_export_table(p_goodAuth in varchar2);

  PROCEDURE clear_award_table(p_Dummy in varchar2, Retval IN OUT Rcursor);

end AE_BARCODE_UPDATE;
/
create or replace package body AE_BARCODE_UPDATE is

  PROCEDURE main(p_badAuth  in VARCHAR2,
                 p_goodAuth in varchar2,
                 Retval     IN OUT Rcursor) is
    v_codeLimit number;
  begin
    /* Clean out the old junk */
    EXECUTE IMMEDIATE 'truncate table ae_valid_barcodes';
    EXECUTE IMMEDIATE 'truncate table ae_affected_population';
    commit;
  
    Select_AffectedPopulation(p_badAuth => p_badAuth);
    select count(*) into v_codeLimit from ae_affected_population;
    Select_ValidBarcodes(p_goodAuth => p_goodAuth, p_codelimit => v_codelimit);
    Assign_Barcodes;
    update_MemberRewards;
    consume_ValidBarcodes;
    award_export_table(p_goodAuth => p_goodAuth);
  
  END main;

  /* ---------------------------------------------------
  * Select the population of people that were issued
  * incorrect rewards and insert them into an auxiliary
  * table.
  * --------------------------------------------------- */
  PROCEDURE Select_AffectedPopulation(p_badAuth varchar2) is
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
  
    /* 
    * Probably need more values but that's all I can
    * think of right now.
    */
    cursor c is
      select vc.loyaltyidnumber as loyaltyid,
             mr.memberid        as ipcode,
             mr.offercode       as bad_auth,
             mr.certificatenmbr as bad_cert
        from bp_ae.lw_memberrewards mr
        join bp_ae.ats_memberdetails md
          on md.a_ipcode = mr.memberid
        join bp_ae.lw_virtualcard vc
          on vc.ipcode = mr.memberid
       where 1 = 1
         AND mr.offercode = p_badAuth
         AND nvl(length(md.a_emailaddress), 0) > 2
         and vc.isprimary = 1;
    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('Select_AffectedPopulation');
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
    /*End of Logging*/
    open c;
    loop
      fetch c bulk collect
        into rec limit 1000;
      for i in 1 .. rec.COUNT loop
        insert into ae_affected_population
          (loyaltyid, ipcode, bad_cert, bad_auth, good_cert, good_auth)
        values
          (rec (i).loyaltyid,
           rec (i).ipcode,
           rec (i).bad_cert,
           rec (i).bad_auth,
           NULL, /* These get populated in the assign step */
           NULL);
      end loop;
    
      commit;
      exit when c%notfound;
    
    end loop;
  
    /* Clean up */
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
    
  end Select_AffectedPopulation;

  /* ---------------------------------------------- */

  PROCEDURE Select_ValidBarcodes(p_goodAuth in varchar2, p_codeLimit in number) IS
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
  
    /* This way we're not just dumping as many barcodes as we can into the table */
    v_codeLimit number;
  
    cursor c is
      select b.a_barcode as good_cert, b.a_typecode as good_auth
        from ats_rewardbarcodes b
       where 1 = 1
         and b.a_typecode = p_goodAuth
         and b.a_status = 0
         and rownum <= p_codeLimit;
  
    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('Select_validbarcodes');
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
    /* End of Logging */
  
    open c;
    loop
      fetch c bulk collect
        into rec limit 1000;
      for i in 1 .. rec.COUNT loop
        insert into ae_valid_barcodes
          (good_cert, good_auth)
        values
          (rec(i).good_cert, rec(i).good_auth);
      end loop;
    
      commit;
      exit when c%NOTFOUND;
    
    end loop;
  
    /* Clean up */
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
    
  END Select_ValidBarcodes;

  /* ---------------------------------------------- */

  PROCEDURE Assign_Barcodes is
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
    p_StartDate nvarchar2(12);
  
    /*
    * Join both tables together based on row number.
    * This way the cursor essentially distributes
    * unique barcodes to each member and we update
    * ae_affected_population based on the invalid
    * certificate.
    */
    cursor c is
      select b.good_cert, b.good_auth, p.bad_cert
        from (select rownum as row_num, c.good_cert, c.good_auth
                from ae_valid_barcodes c) b
        join (select rownum as row_num, c.bad_cert
                from ae_affected_population c) p
          on p.row_num = b.row_num;
  
    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('assign_barcodes');
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
    /* End of Logging */
  
    open c;
    loop
      fetch c bulk collect
        into rec limit 1000;
      for i in 1 .. rec.COUNT loop
        update ae_affected_population p
           set p.good_cert = rec(i).good_cert,
               p.good_auth = rec(i).good_auth
         where 1 = 1
           and p.bad_cert = rec(i).bad_cert;
      end loop;
    
      commit;
      exit when c%NOTFOUND;
    
    end loop;
  
    /* Clean up */
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
    
  end Assign_Barcodes;

  procedure update_MemberRewards is
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
  
    /*
    * Using the bad certificate is more specific
    * than using ipcode and eliminates over
    * rewarding risk.
    */
    cursor c is
      select p.good_cert as good_cert,
             p.good_auth as good_auth,
             p.bad_cert  as bad_cert
        from ae_affected_population p;
  
    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('update_memberrewards');
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
    /* End of Logging */
  
    /* 
    * -----------------------------------------------------------
    * Danger!
    * We make the assumption here that the authcode being used
    * is that of the same reward-id and product id
    * -----------------------------------------------------------
    */
    open c;
    loop
    
      fetch c bulk collect
        into rec limit 1000;
      for i in 1 .. rec.COUNT loop
        update lw_memberrewards mr
           set mr.certificatenmbr = rec(i).good_cert,
               mr.offercode       = rec(i).good_auth
         where 1 = 1
           and mr.certificatenmbr = rec(i).bad_cert;
      end loop;
    
      commit;
      exit when c%NOTFOUND;
    
    end loop;
  
    /* Clean up */
    if c%ISOPEN then
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
  end update_MemberRewards;

  procedure consume_ValidBarcodes is
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
    p_StartDate nvarchar2(12);
  
    /*
    * Update ats_rewardbarcodes to change
    * the status to 1 and update the certificate
    * to show the ipcode of the awardee.
    */
    cursor c is
      select p.ipcode    as ipcode,
             p.good_cert as good_cert,
             p.good_auth as good_auth,
             p.bad_cert  as bad_cert
        from ae_affected_population p;
  
    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;
  
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('consume_validbarcodes');
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
    /* End of Logging */
  
    open c;
    loop
      fetch c bulk collect
        into rec limit 1000;
      for i in 1 .. rec.COUNT loop
        update ats_rewardbarcodes r
           set r.a_ipcode = rec(i).ipcode, r.a_status = 1
         where 1 = 1
           and r.a_barcode = rec(i).good_cert;
      end loop;
    
      commit;
      exit when c%NOTFOUND;
    
    end loop;
  
    /* Clean up */
    if c%ISOPEN then
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
    
  end consume_ValidBarcodes;
  /* ---------------------------------------------- */

  /* ---------------------------------------------- */
  procedure award_export_table(p_goodAuth in varchar2) is
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

    v_rewardname nvarchar2(50);
    v_eid number;
    v_campaign_type number;
    v_campaign_id number;
  
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('award_export_table');
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
    /* End of Logging */
    
    /* This will cause an issue if the reward name pops up more than once */
    select distinct r.name
      into v_rewardname
      FROM Bp_Ae.Lw_Memberrewards Mr, Bp_Ae.Lw_Rewardsdef r
     WHERE 1 = 1
       AND Mr.Rewarddefid = r.id
       and mr.offercode = p_goodAuth
       AND r.Name IN
           ('AEO Rewards $10 Reward', 'B5G1 Jean Reward', 'B5G1 Bra Reward');
  
    /* should we make this a cursor? */
    insert into x$_export_updated_awards
      SELECT --rownum AS Rownumber,
       Channel,
       Customer_Nbr,
       Loyalty_Number,
       Email,
       Mobile_Number,
       Fname,
       Lname,
       Address1,
       Address2,
       City,
       State,
       Zip,
       Country,
       Auth_Cd,
       Suc_Cd,
       Campaign_Type,
       Campaign_Exp_Date,
       eid,
       campaign_id,
       language_preference,
       gender,
       birthdate,
       store_loyalty,
       tier_status,
       points_balance,
       points_needed_for_next_reward,
       number_of_bras_purchased,
       credits_to_next_free_bra,
       number_of_jeans_purchased,
       credits_to_next_free_jean,
       number_of_active_5_off_reward,
       number_of_jeans_reward,
       number_bra_reward,
       communication_id,
       comm_plan_id,
       collateral_id,
       package_id,
       step_id,
       message_id,
       seg_id,
       seg_nm,
       aap_flag,
       card_type,
       run_id,
       lead_key_id,
       site_url,
       enable_passbook_pass,
       timestamp
        FROM (SELECT --Rownum AS Rownumber,
               1 --AEO-914 special because this is email output
               /*CASE
                 WHEN (NVL(Md.a_Pendingemailverification, 1) = 0 AND
                      Md.a_Emailaddress IS NOT NULL) THEN
                  CAST(1 AS NVARCHAR2(100))
                 WHEN (NVL(Md.a_Pendingcellverification, 1) = 0 AND
                      Md.a_Mobilephone IS NOT NULL) THEN
                  CAST(2 AS NVARCHAR2(100))
                 WHEN (NVL(Md.a_Pendingemailverification, 1) = 1 AND
                      NVL(Md.a_Pendingcellverification, 1) = 1) THEN
                  CAST(3 AS NVARCHAR2(100))
                 ELSE
                  CAST(1 AS NVARCHAR2(100))
               END*/ AS Channel,
               CAST(Vc.Linkkey AS NVARCHAR2(80)) AS Customer_Nbr,
               CAST(ap.Loyaltyid AS NVARCHAR2(100)) AS Loyalty_Number, --AEO-914
               CAST(Md.a_Emailaddress AS NVARCHAR2(150)) AS Email,
               Regexp_Replace(CAST(Md.a_Mobilephone AS NVARCHAR2(100)),
                              '[^[:digit:]]',
                              NULL) AS Mobile_Number,
               CAST(Lm.Firstname AS NVARCHAR2(100)) AS Fname,
               CAST(Lm.Lastname AS NVARCHAR2(100)) AS Lname,
               CAST(Md.a_Addresslineone AS NVARCHAR2(100)) AS Address1,
               CAST(Md.a_Addresslinetwo AS NVARCHAR2(100)) AS Address2,
               CAST(Md.a_City AS NVARCHAR2(100)) AS City,
               CAST(Md.a_Stateorprovince AS NVARCHAR2(100)) AS State,
               CAST(REPLACE(Md.a_Ziporpostalcode, '-') AS NVARCHAR2(100)) AS Zip,
               CAST(Md.a_Country AS NVARCHAR2(100)) AS Country,
               CAST(ap.good_auth AS NVARCHAR2(100)) AS Auth_Cd,
               CAST(ap.good_cert AS NVARCHAR2(100)) AS Suc_Cd,
               (CASE
                  when(v_rewardname = 'AEO Rewards $10 Reward') then
                                    cast('1' as nvarchar2(6))
                  when(v_rewardname = 'B5G1 Jean Reward') then
                                    cast('3' as nvarchar2(6))
                  when(v_rewardname = 'B5G1 Bra Reward') then
                                    cast('2' as nvarchar2(6))
               end) AS Campaign_Type,
               CAST(concat('31-DEC-', extract(YEAR FROM SYSDATE)) AS
                    NVARCHAR2(100)) AS Campaign_Exp_Date, --AEO 750
               (CASE
                  when(v_rewardname = 'AEO Rewards $10 Reward') then
                                    cast('266999' as nvarchar2(6))
                  when(v_rewardname = 'B5G1 Jean Reward') then
                                    cast('267000' as nvarchar2(6))
                  when(v_rewardname = 'B5G1 Bra Reward') then
                                    cast('267004' as nvarchar2(6))
                end) AS Eid,
               (CASE
                  when(v_rewardname = 'AEO Rewards $10 Reward') then
                                    cast('2201' as nvarchar2(6))
                  when(v_rewardname = 'B5G1 Jean Reward') then
                                    cast('2202' as nvarchar2(6))
                  when(v_rewardname = 'B5G1 Bra Reward') then
                                    cast('2203' as nvarchar2(6))
                end) AS Campaign_Id,
               CASE
                 WHEN Md.a_Languagepreference IS NULL THEN
                  CAST(0 AS NVARCHAR2(100))
                 ELSE
                  CAST(Md.a_Languagepreference AS NVARCHAR2(100))
               END AS Language_Preference,
               CAST(To_Char(Nvl(Md.a_Gender, '0')) AS NVARCHAR2(100)) AS Gender,
               CAST(To_Char(Lm.Birthdate, 'DD-MON-YYYY') AS NVARCHAR2(100)) AS Birthdate,
               CAST(To_Char(Md.a_Homestoreid) AS NVARCHAR2(80)) AS Store_Loyalty,
               (SELECT CASE
                         WHEN t.Tiername = 'Blue' THEN
                          CAST(1 AS NVARCHAR2(100))
                         WHEN t.Tiername = 'Silver' THEN
                          CAST(2 AS NVARCHAR2(100))
                         WHEN t.Tiername = 'Gold' THEN
                          CAST(3 AS NVARCHAR2(100))
                         WHEN t.Tiername = 'Select' THEN
                          CAST(4 AS NVARCHAR2(100))
                       END
                  FROM Bp_Ae.Lw_Membertiers Mt
                 INNER JOIN Bp_Ae.Lw_Tiers t
                    ON Mt.Tierid = t.Tierid
                 WHERE 1 = 1
                   AND Mt.Memberid = Vc.Ipcode
                   AND SYSDATE BETWEEN Mt.Fromdate AND Mt.Todate
                   AND Rownum = 1
                   AND 1 = 1) AS Tier_Status,
               CAST(p.a_Totalpoints AS NVARCHAR2(80)) AS Points_Balance,
               CAST(p.a_Pointstonextreward AS NVARCHAR2(80)) AS Points_Needed_For_Next_Reward,
               CAST(p.a_Bracurrentpurchased AS NVARCHAR2(80)) AS Number_Of_Bras_Purchased,
               CAST(5 - p.a_Bracurrentpurchased AS NVARCHAR2(80)) AS Credits_To_Next_Free_Bra,
               CAST(p.a_Jeansrollingbalance AS NVARCHAR2(80)) AS Number_Of_Jeans_Purchased,
               CAST(5 - p.a_Jeansrollingbalance AS NVARCHAR2(80)) AS Credits_To_Next_Free_Jean,
               Rew.Number_Of_Active_5_Off_Reward,
               Rew.Number_Of_Jeans_Reward,
               Rew.Number_Of_Bra_Reward AS Number_Bra_Reward,
               CAST('' AS NVARCHAR2(100)) AS Communication_Id,
               CAST('' AS NVARCHAR2(100)) AS Comm_Plan_Id,
               CAST('' AS NVARCHAR2(100)) AS Collateral_Id,
               CAST('' AS NVARCHAR2(100)) AS Package_Id,
               CAST('' AS NVARCHAR2(100)) AS Step_Id,
               CAST('' AS NVARCHAR2(100)) AS Message_Id,
               CAST('' AS NVARCHAR2(100)) AS Seg_Id,
               CAST('' AS NVARCHAR2(100)) AS Seg_Nm,
               CAST('' AS NVARCHAR2(100)) AS Aap_Flag,
               CAST(Md.a_Cardtype AS NVARCHAR2(80)) AS Card_Type,
               CAST('' AS NVARCHAR2(100)) AS Run_Id,
               CAST('' AS NVARCHAR2(100)) AS Lead_Key_Id,
               CAST('' AS NVARCHAR2(100)) AS Site_Url,
               CAST('' AS NVARCHAR2(100)) AS Enable_Passbook_Pass,
               CAST('' AS NVARCHAR2(100)) AS TIMESTAMP
                FROM Bp_Ae.Lw_Loyaltymember Lm
               INNER JOIN Bp_Ae.Lw_Virtualcard Vc
                  ON Lm.Ipcode = Vc.Ipcode
                 AND Vc.Isprimary = 1
               INNER JOIN Bp_Ae.Ats_Memberdetails Md
                  ON Lm.Ipcode = Md.a_Ipcode
               INNER JOIN bp_ae.ae_affected_population ap --AEO-914
                  on ap.ipcode = md.a_ipcode --AEO-914
                LEFT JOIN Bp_Ae.Ats_Memberpointbalances p
                  ON Vc.Ipcode = p.a_Ipcode
                LEFT JOIN (SELECT Mr.Memberid,
                                 SUM(CASE
                                       WHEN r.Name = 'AEO Rewards $10 Reward' THEN
                                        1
                                       ELSE
                                        0
                                     END) AS Number_Of_Active_5_Off_Reward,
                                 SUM(CASE
                                       WHEN r.Name = 'B5G1 Jean Reward' THEN
                                        1
                                       ELSE
                                        0
                                     END) AS Number_Of_Jeans_Reward,
                                 SUM(CASE
                                       WHEN r.Name = 'B5G1 Bra Reward' THEN
                                        1
                                       ELSE
                                        0
                                     END) AS Number_Of_Bra_Reward
                            FROM Bp_Ae.Lw_Memberrewards Mr,
                                 Bp_Ae.Lw_Rewardsdef    r
                           WHERE 1 = 1
                             AND Mr.Rewarddefid = r.Id
                             AND r.Name IN ('AEO Rewards $10 Reward',
                                            'B5G1 Jean Reward',
                                            'B5G1 Bra Reward')
                             AND Mr.Fulfillmentdate IS NULL
                             AND Mr.Expiration > SYSDATE
                             AND 1 = 1
                           GROUP BY Mr.Memberid) Rew
                  ON Rew.Memberid = Lm.Ipcode
               WHERE 1 = 1
                    --  AND Bp_Ae.Ae_Isinpilot(Md.a_Extendedplaycode) = 1
                 AND md.a_extendedplaycode IN (1, 3) -- this accomplishes the same thing as the function (though not dynamic)
                 AND (Md.a_Cardtype = 1 OR Md.a_Cardtype = 2 OR
                     Md.a_Cardtype = 3)) TBL -- why not use IN here?
       WHERE 1 = 1;
    commit;
  
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
  end award_export_table;

  PROCEDURE clear_award_table(p_Dummy in varchar2, Retval IN OUT Rcursor) is
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
  begin
    /*Logging*/
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    --  v_dap_log_id := utility_pkg.get_LIBJobID();
    v_Jobname   := Upper('clear_award_table');
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
    /* End of Logging */
  
    delete x$_export_updated_awards;
    commit;
  
    /* More logging */
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
                          p_Logsource => 'AE_BARCODE_UPDATE',
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
  END clear_award_table;

end AE_BARCODE_UPDATE;
/
