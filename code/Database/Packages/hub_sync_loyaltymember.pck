create or replace package hub_sync_loyaltymember is

PROCEDURE CREATE_TEMP_TABLES;

PROCEDURE MERGE_LW_LOYALTYMEMBER (p_mod INTEGER,
                                  p_instances INTEGER);

PROCEDURE COPY_LW_LOYALTYMEMBER_ARC;

PROCEDURE RECREATE_INDEXES;


-- Note BP_AE.FINAL_LM must exist for this package to compile properly.
-- The DDL for this is at the bottom of this package spec.

-- NOTE the create_temp_tables procedure of this package must be executed
-- before MERGE_LW_LOYALTYMEMBER can be executed properly.

-- Note:
-- MERGE_LW_LOYALTYMEMBER is designed to be run in  parallel instances.
-- For example - to run it in 5 parallel instances execute it as
--   Merge_Lw_LoyaltyMember (p_mod => 0, p_instances 5);
--   Merge_Lw_LoyaltyMember (p_mod => 1, p_instances 5);
--   Merge_Lw_LoyaltyMember (p_mod => 2, p_instances 5);
--   Merge_Lw_LoyaltyMember (p_mod => 3, p_instances 5);
--   Merge_Lw_LoyaltyMember (p_mod => 4, p_instances 5);

-- Any other combinations of parameters will not work properly.
-- To run it in only one instance, use:
--   Merge_Lw_LoyaltyMember (p_mod => 0, p_instances 1);

-- NOTE after all instances have finished successfully RECREATE_INDEXES needs to be run.

-- TODO - need to modify the log messages to indicate what the parameters
-- of the running instance are in MERGE_LW_LOYALTYMEMBER.

-- TODO - probably want to improve logging and exception handling.

/*
-- code to create final_LM table:

create table BP_AE.FINAL_LM
(
  old_ipcode                     NUMBER(20) not null,
  old_birthdate                  TIMESTAMP(4),
  old_alternateid                NVARCHAR2(255),
  old_failedpasswordattemptcount NUMBER(10) not null,
  old_firstname                  NVARCHAR2(50),
  old_isemployee                 NUMBER(1),
  old_lastactivitydate           TIMESTAMP(4),
  old_lastname                   NVARCHAR2(50),
  old_memberclosedate            TIMESTAMP(4),
  old_membercreatedate           TIMESTAMP(4) not null,
  old_memberstatus               NUMBER(10) not null,
  old_middlename                 NVARCHAR2(50),
  old_nameprefix                 NVARCHAR2(10),
  old_namesuffix                 NVARCHAR2(10),
  old_newstatus                  NUMBER(10),
  old_newstatuseffectivedate     TIMESTAMP(4),
  old_primaryemailaddress        NVARCHAR2(254),
  old_primaryphonenumber         NVARCHAR2(25),
  old_primarypostalcode          NVARCHAR2(15),
  old_resetcode                  VARCHAR2(32),
  old_resetcodedate              TIMESTAMP(4),
  old_statuschangereason         NVARCHAR2(255),
  old_statuschangedate           TIMESTAMP(4),
  old_username                   NVARCHAR2(254),
  old_updatedate                 TIMESTAMP(6),
  old_password                   NVARCHAR2(50),
  old_passwordexpiredate         TIMESTAMP(4),
  old_passwordchangerequired     NUMBER(1) not null,
  createdate                     TIMESTAMP(4) not null,
  changedby                      NVARCHAR2(25),
  new_ipcode                     NUMBER(20) not null,
  new_birthdate                  TIMESTAMP(4),
  new_alternateid                NVARCHAR2(255),
  new_failedpasswordattemptcount NUMBER(10) not null,
  new_firstname                  NVARCHAR2(50),
  new_isemployee                 NUMBER(1),
  new_lastactivitydate           TIMESTAMP(4),
  new_lastname                   NVARCHAR2(50),
  new_memberclosedate            TIMESTAMP(4),
  new_membercreatedate           TIMESTAMP(4) not null,
  new_memberstatus               NUMBER(10) not null,
  new_middlename                 NVARCHAR2(50),
  new_nameprefix                 NVARCHAR2(10),
  new_namesuffix                 NVARCHAR2(10),
  new_newstatus                  NUMBER(10),
  new_newstatuseffectivedate     TIMESTAMP(4),
  new_primaryemailaddress        NVARCHAR2(254),
  new_primaryphonenumber         NVARCHAR2(25),
  new_primarypostalcode          NVARCHAR2(15),
  new_resetcode                  VARCHAR2(32),
  new_resetcodedate              TIMESTAMP(4),
  new_statuschangereason         NVARCHAR2(255),
  new_statuschangedate           TIMESTAMP(4),
  new_username                   NVARCHAR2(254),
  new_updatedate                 TIMESTAMP(6),
  new_password                   NVARCHAR2(50),
  new_passwordexpiredate         TIMESTAMP(4),
  new_passwordchangerequired     NUMBER(1) not null
)
tablespace BP_AE_D
  pctfree 10
  initrans 1
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  );
*/

end hub_sync_loyaltymember;
/
create or replace package body hub_sync_loyaltymember is



PROCEDURE CREATE_TEMP_TABLES IS
  V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    --V_FILENAME  VARCHAR2(256) := '';

    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'hub_sync_loyaltymember';

    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);


BEGIN
 V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
      V_JOBNAME   := 'create_temp_tables';
      V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);


     V_REASON := 'create_temp_tables start ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

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

    begin

        execute immediate 'drop table bp_ae.old_lm purge';

    exception


          WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'drop table bp_ae.old_lm ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
    end;

    execute immediate '
    create table bp_ae.old_lm tablespace bp_ae_d as
    SELECT /*+use_hash(m,l)*/
              m.old_ipcode AS OLD_IPCODE,
              l.birthdate  AS OLD_BIRTHDATE,
              l.alternateid AS OLD_ALTERNATEID,
              l.failedpasswordattemptcount AS OLD_failedpasswordattemptcount,
              l.firstname AS OLD_FIRSTNAME,
              l.isemployee AS OLD_isemployee,
              l.lastactivitydate AS OLD_lastactivitydate,
              l.lastname AS OLD_LASTNAME,
              l.memberclosedate AS OLD_memberclosedate,
              l.membercreatedate AS OLD_membercreatedate,
              l.memberstatus AS OLD_memberstatus,
              l.middlename AS OLD_MIDDLENAME,
              l.nameprefix AS old_nameprefix,
              l.namesuffix AS old_namesuffix,
              l.newstatus  AS old_newstatus,
              l.newstatuseffectivedate AS old_newstatuseffectivedate,
              l.primaryemailaddress AS old_primaryemailaddress,
              l.primaryphonenumber AS old_primaryphonenumber,
              l.primarypostalcode AS old_primarypostalcode,
              l.resetcode AS old_resetcode,
              l.resetcodedate AS old_resetcodedate,
              l.statuschangereason AS old_statuschangereason,
              l.statuschangedate AS old_statuschangedate,
              l.username AS old_username,
              l.updatedate as old_updatedate,
              l.password AS old_password,
              l.passwordexpiredate AS old_passwordexpiredate,
              l.passwordchangerequired   AS  old_passwordchangerequired,
              m.new_ipcode,
              l.createdate,
              l.changedby
          FROM   ( select old_ipcode, new_ipcode
                    from Bp_Ae.Ae_Merge
                    group by old_ipcode, new_ipcode) m, Bp_Ae.Lw_Loyaltymember l
          WHERE  l.ipcode = m.old_ipcode' ;

    -- create the second stage table:
    begin
        execute immediate 'drop table bp_ae.new_lm purge';
    exception
          WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'create table bp_ae.old_lm ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);

          NULL;
    end;

    BEGIN


     execute immediate '
    create table bp_ae.new_lm tablespace bp_ae_d as
    SELECT /*+use_hash(m,l)*/
              m.new_ipcode AS NEW_IPCODE,
              m.old_ipcode,
              l.birthdate  AS new_BIRTHDATE,
              l.alternateid AS new_ALTERNATEID,
              l.failedpasswordattemptcount AS new_failedpasswordattemptcount,
              l.firstname AS new_FIRSTNAME,
              l.isemployee AS new_isemployee,
              l.lastactivitydate AS new_lastactivitydate,
              l.lastname AS new_LASTNAME,
              l.memberclosedate AS new_memberclosedate,
              l.membercreatedate AS new_membercreatedate,
              l.memberstatus AS new_memberstatus,
              l.middlename AS new_MIDDLENAME,
              l.nameprefix AS new_nameprefix,
              l.namesuffix AS new_namesuffix,
              l.newstatus  AS new_newstatus,
              l.newstatuseffectivedate AS new_newstatuseffectivedate,
              l.primaryemailaddress AS new_primaryemailaddress,
              l.primaryphonenumber AS new_primaryphonenumber,
              l.primarypostalcode AS new_primarypostalcode,
              l.resetcode AS new_resetcode,
              l.resetcodedate AS new_resetcodedate,
              l.statuschangereason AS new_statuschangereason,
              l.statuschangedate AS new_statuschangedate,
              l.username AS new_username,
              l.updatedate as new_updatedate,
              l.password AS new_password,
              l.passwordexpiredate AS new_passwordexpiredate,
              l.passwordchangerequired   AS  new_passwordchangerequired
          FROM   ( select old_ipcode, new_ipcode
                    from Bp_Ae.Ae_Merge
                    group by old_ipcode, new_ipcode) m, Bp_Ae.Lw_Loyaltymember l
          WHERE  l.ipcode = m.new_ipcode ';
          EXCEPTION
              WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'create table bp_ae.new_lm ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;
    -- create the final table:

    BEGIN
         EXECUTE IMMEDIATE 'drop table bp_ae.final_lm purge';
    EXCEPTION
         WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'drop table bp_ae.final_lm ' || Err_Code || ' ' ||
                           Err_Msg;
              v_Message := v_Reason;
              v_Error   := v_Reason;
              Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                  p_Envkey    => v_Envkey,
                                  p_Logsource => v_Logsource,
                                  p_Filename  => NULL,
                                  p_Batchid   => v_Batchid,
                                  p_Jobnumber => v_My_Log_Id,
                                  p_Message   => v_Message,
                                  p_Reason    => v_Reason,
                                  p_Error     => v_Error,
                                  p_Trycount  => v_Trycount,
                                  p_Msgtime   => SYSDATE);
              NULL;
    END;

BEGIN
    execute immediate '
    create table bp_ae.final_lm tablespace bp_ae_d as
       SELECT /*+use_hash(n,o)*/
              o.old_ipcode,
              old_birthdate,
              old_alternateid,
              old_failedpasswordattemptcount,
              old_firstname,
              old_isemployee,
              old_lastactivitydate,
              old_lastname,
              old_memberclosedate,
              old_membercreatedate,
              old_memberstatus,
              old_middlename,
              old_nameprefix,
              old_namesuffix,
              old_newstatus,
              old_newstatuseffectivedate,
              old_primaryemailaddress,
              old_primaryphonenumber,
              old_primarypostalcode,
              old_resetcode,
              old_resetcodedate,
              old_statuschangereason,
              old_statuschangedate,
              old_username,
              old_updatedate,
              old_password,
              old_passwordexpiredate,
              old_passwordchangerequired,
              createdate,
              changedby,
              n.new_ipcode,
              new_birthdate,
              new_alternateid,
              new_failedpasswordattemptcount,
              new_firstname,
              new_isemployee,
              new_lastactivitydate,
              new_lastname,
              new_memberclosedate,
              new_membercreatedate,
              new_memberstatus,
              new_middlename,
              new_nameprefix,
              new_namesuffix,
              new_newstatus,
              new_newstatuseffectivedate,
              new_primaryemailaddress,
              new_primaryphonenumber,
              new_primarypostalcode,
              new_resetcode,
              new_resetcodedate,
              new_statuschangereason,
              new_statuschangedate,
              new_username,
              new_updatedate,
              new_password,
              new_passwordexpiredate,
              new_passwordchangerequired
         FROM bp_ae.new_lm n,
              bp_ae.old_lm o
          WHERE N.NEW_IPCODE = o.new_ipcode
            AND N.OLD_IPCODE = o.old_ipcode ';
     EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'create table bp_ae.final_lm ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;
    -- here we drop triggers, indexes, constraints.

    BEGIN
       EXECUTE IMMEDIATE ' alter trigger BP_AE."LW_LOYALTYMEMBER#" disable';
     EXCEPTION WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'trigger BP_AE."LW_LOYALTYMEMBER#" disable ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
    END;
    
    BEGIN
         EXECUTE IMMEDIATE 'alter trigger bp_ae.LW_LOYALTYMEMBER_ARC disable';
    EXCEPTION
         WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'trigger BP_AE."LW_LOYALTYMEMBER_ARC" disable' ||
                           Err_Code || ' ' || Err_Msg;
              v_Message := v_Reason;
              v_Error   := v_Reason;
              Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                  p_Envkey    => v_Envkey,
                                  p_Logsource => v_Logsource,
                                  p_Filename  => NULL,
                                  p_Batchid   => v_Batchid,
                                  p_Jobnumber => v_My_Log_Id,
                                  p_Message   => v_Message,
                                  p_Reason    => v_Reason,
                                  p_Error     => v_Error,
                                  p_Trycount  => v_Trycount,
                                  p_Msgtime   => SYSDATE);
              NULL;
    END;





 V_REASON := 'create_temp_tables end ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

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

END;


PROCEDURE Merge_Lw_LoyaltyMember (p_mod INTEGER,
                                  p_instances INTEGER) IS


     --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    P_FILENAME VARCHAR2(50) := 'hub_sync_merge';
    V_MY_LOG_ID NUMBER;

    CURSOR DRIVER IS
        SELECT *
          FROM BP_AE.FINAL_LM t
         WHERE mod(t.old_ipcode, p_instances) = p_mod
         ORDER BY new_ipcode;


    TYPE DRIVER_TABLE_TYPE IS TABLE OF DRIVER%rowtype;

    DT DRIVER_TABLE_TYPE;

BEGIN

     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'Merge_Lw_LoyaltyMember';
     V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

      V_ERROR          :=  ' ';
      V_REASON         := 'Merge_Lw_LoyaltyMember start';
      V_MESSAGE        := 'Merge_Lw_LoyaltyMember start';
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


    --disable trigger;
    --execute immediate 'alter trigger bp_ae.lw_loyaltymember_arc disable';


-- create the first stage table:



     OPEN Driver;
     LOOP
          FETCH Driver BULK COLLECT
               INTO Dt LIMIT 100;
          FORALL i IN 1 .. Dt.Count
               UPDATE Bp_Ae.Lw_Loyaltymember Lm
               SET
                      Lm.Birthdate = CASE
                                          WHEN (Dt(i).Old_Birthdate IS NOT NULL AND Dt(i)
                                               .New_Birthdate IS NULL) THEN
                                           Dt(i).Old_Birthdate
                                          ELSE
                                           Lm.Birthdate
                                     END,
                      Lm.Resetcode = CASE
                                          WHEN (Dt(i).Old_Resetcode IS NOT NULL AND Dt(i)
                                               .New_Resetcode IS NULL) THEN
                                           Dt(i).Old_Resetcode
                                          ELSE
                                           Lm.Resetcode
                                     END,
                      Lm.Statuschangedate = CASE
                                                 WHEN (Dt(i)
                                                      .Old_Statuschangedate IS NOT NULL AND Dt(i)
                                                      .New_Statuschangedate IS NULL) THEN
                                                  Dt(i).Old_Statuschangedate
                                                 ELSE
                                                  Lm.Statuschangedate
                                            END,
                      Lm.Username = CASE
                                         WHEN (Dt(i).Old_Username IS NOT NULL AND Dt(i)
                                              .New_Username IS NULL) THEN
                                          Dt(i).Old_Username
                                         ELSE
                                          Lm.Username
                                    END,
                      Lm.Updatedate = CASE
                                           WHEN (Dt(i).Old_Updatedate IS NOT NULL AND Dt(i)
                                                .New_Updatedate IS NULL) THEN
                                            Dt(i).Old_Updatedate
                                           ELSE
                                            Lm.Updatedate
                                      END,
                      Lm.Password = CASE
                                         WHEN (Dt(i).Old_Password IS NOT NULL AND Dt(i)
                                              .New_Password IS NULL) THEN
                                          Dt(i).Old_Password
                                         ELSE
                                          Lm.Password
                                    END,
                      Lm.Passwordexpiredate = CASE
                                                   WHEN (Dt(i)
                                                        .Old_Passwordexpiredate IS NOT NULL AND Dt(i)
                                                        .New_Passwordexpiredate IS NULL) THEN
                                                    Dt(i).Old_Passwordexpiredate
                                                   ELSE
                                                    Lm.Passwordexpiredate
                                              END,
                      Lm.Resetcodedate = CASE
                                              WHEN (Dt(i)
                                                   .Old_Resetcodedate IS NOT NULL AND Dt(i)
                                                   .New_Resetcodedate IS NULL) THEN
                                               Dt(i).Old_Resetcodedate
                                              ELSE
                                               Lm.Resetcodedate
                                         END,
                      Lm.Primaryphonenumber = CASE
                                                   WHEN (Dt(i)
                                                        .Old_Primaryphonenumber IS NOT NULL AND Dt(i)
                                                        .New_Primaryphonenumber IS NULL) THEN
                                                    Dt(i).Old_Primaryphonenumber
                                                   ELSE
                                                    Lm.Primaryphonenumber
                                              END,
                      Lm.Statuschangereason = CASE
                                                   WHEN (Dt(i)
                                                        .Old_Statuschangereason IS NOT NULL AND Dt(i)
                                                        .New_Statuschangereason IS NULL) THEN
                                                    Dt(i).Old_Statuschangereason
                                                   ELSE
                                                    Lm.Statuschangereason
                                              END,
                       Lm.Primaryemailaddress = NULL, --AEO-1290 | Primary Member
                      --AEO-1290 BEGIN
                      --CASE
                      --WHEN (Dt(i)
                      --.Old_Primaryemailaddress IS NOT NULL AND Dt(i)
                      --.New_Primaryemailaddress IS NULL) THEN
                      --Dt(i).Old_Primaryemailaddress
                      --ELSE
                      --Lm.Primaryemailaddress
                      --END,
                      --AEO-1290 END
                      Lm.Primarypostalcode = CASE
                                                  WHEN (Dt(i)
                                                       .Old_Primarypostalcode IS NOT NULL AND Dt(i)
                                                       .New_Primarypostalcode IS NULL) THEN
                                                   Dt(i).Old_Primarypostalcode
                                                  ELSE
                                                   Lm.Primarypostalcode
                                             END,
                      Lm.Newstatus = CASE
                                          WHEN (Dt(i).Old_Newstatus IS NOT NULL AND Dt(i)
                                               .New_Newstatus IS NULL) THEN
                                           Dt(i).Old_Newstatus
                                          ELSE
                                           Lm.Newstatus
                                     END,
                      Lm.Newstatuseffectivedate = CASE
                                                       WHEN (Dt(i)
                                                            .Old_Newstatuseffectivedate IS NOT NULL AND Dt(i)
                                                            .New_Newstatuseffectivedate IS NULL) THEN
                                                        Dt(i)
                                                        .Old_Newstatuseffectivedate
                                                       ELSE
                                                        Lm.Newstatuseffectivedate
                                                  END,
                      Lm.Middlename = CASE
                                           WHEN (Dt(i).Old_Middlename IS NOT NULL AND Dt(i)
                                                .New_Middlename IS NULL) THEN
                                            Dt(i).Old_Middlename
                                           ELSE
                                            Lm.Middlename
                                      END,
                      Lm.Memberstatus = CASE
                                             WHEN (Dt(i).Old_Memberstatus IS NOT NULL AND Dt(i)
                                                  .New_Memberstatus IS NULL) THEN
                                              Dt(i).Old_Memberstatus
                                             ELSE
                                              Lm.Memberstatus
                                        END,
                      Lm.Alternateid = CASE
                                            WHEN (Dt(i).Old_Alternateid IS NOT NULL AND Dt(i)
                                                 .New_Alternateid IS NULL) THEN
                                             Dt(i).Old_Alternateid
                                            ELSE
                                             Lm.Alternateid
                                       END,
                      Lm.Failedpasswordattemptcount = CASE
                                                           WHEN (Dt(i)
                                                                .Old_Failedpasswordattemptcount IS NOT NULL AND Dt(i)
                                                                .New_Failedpasswordattemptcount IS NULL) THEN
                                                            Dt(i)
                                                            .Old_Failedpasswordattemptcount
                                                           ELSE
                                                            Lm.Failedpasswordattemptcount
                                                      END,
                      Lm.Firstname = CASE
                                          WHEN (Dt(i).Old_Firstname IS NOT NULL AND Dt(i)
                                               .New_Firstname IS NULL) THEN
                                           Dt(i).Old_Firstname
                                          ELSE
                                           Lm.Firstname
                                     END,
                      Lm.Isemployee = CASE
                                           WHEN (Dt(i).Old_Isemployee IS NOT NULL AND Dt(i)
                                                .New_Isemployee IS NULL) THEN
                                            Dt(i).Old_Isemployee
                                           ELSE
                                            Lm.Isemployee
                                      END,
                      Lm.Lastactivitydate = CASE
                                                 WHEN (Dt(i)
                                                      .Old_Lastactivitydate IS NOT NULL AND Dt(i)
                                                      .New_Lastactivitydate IS NULL) THEN
                                                  Dt(i).Old_Lastactivitydate
                                                 ELSE
                                                  Lm.Lastactivitydate
                                            END,
                      Lm.Lastname = CASE
                                         WHEN (Dt(i).Old_Lastname IS NOT NULL AND Dt(i)
                                              .New_Lastname IS NULL) THEN
                                          Dt(i).Old_Lastname
                                         ELSE
                                          Lm.Lastname
                                    END,
                      Lm.Membercreatedate = CASE
                                                 WHEN (Dt(i)
                                                      .Old_Membercreatedate IS NOT NULL AND Dt(i)
                                                      .New_Membercreatedate IS NULL) THEN
                                                  Dt(i).Old_Membercreatedate
                                                 ELSE
                                                  Lm.Membercreatedate
                                            END,
                      Lm.Memberclosedate = CASE
                                                WHEN (Dt(i)
                                                     .Old_Memberclosedate IS NOT NULL AND Dt(i)
                                                     .New_Memberclosedate IS NULL) THEN
                                                 Dt(i).Old_Memberclosedate
                                                ELSE
                                                 Lm.Memberclosedate
                                           END,
                      Lm.Nameprefix = CASE
                                           WHEN (Dt(i).Old_Nameprefix IS NOT NULL AND Dt(i)
                                                .New_Nameprefix IS NULL) THEN
                                            Dt(i).Old_Nameprefix
                                           ELSE
                                            Lm.Nameprefix
                                      END,
                      Lm.Namesuffix = CASE
                                           WHEN (Dt(i).Old_Namesuffix IS NOT NULL AND Dt(i)
                                                .New_Namesuffix IS NULL) THEN
                                            Dt(i).Old_Namesuffix
                                           ELSE
                                            Lm.Namesuffix
                                      END
               WHERE  Lm.Ipcode = Dt(i).New_Ipcode;
          COMMIT;
          EXIT WHEN Driver%NOTFOUND;
     END LOOP;
     COMMIT;
     CLOSE Driver;

    --enable trigger;
   -- execute immediate 'alter trigger bp_ae.lw_loyaltymember_arc enable';

      V_ERROR          :=  ' ';
      V_REASON         := 'Merge_Lw_LoyaltyMember end';
      V_MESSAGE        := 'Merge_Lw_LoyaltyMember end';
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

END;


procedure Copy_lw_loyaltymember_arc is

    CURSOR driver IS
   select OLD_ipcode,
   OLD_memberclosedate,
   OLD_memberstatus,
   OLD_firstname,
   OLD_lastname,
   OLD_primaryemailaddress,
   OLD_primaryphonenumber,
   OLD_primarypostalcode,
   OLD_lastactivitydate,
   changedby,
   OLD_newstatus,
   OLD_newstatuseffectivedate,
   createdate,
   OLD_statuschangereason,
   OLD_Birthdate
          from bp_ae.FINAL_LM;


    type driver_table is table of driver%rowtype;
    dt driver_table;
--log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    P_FILENAME VARCHAR2(50) := 'hub_sync_merge';
    V_MY_LOG_ID NUMBER;

BEGIN


     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'Copy_lw_loyaltymember_arc';
     V_LOGSOURCE := V_JOBNAME;

       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

      V_ERROR          :=  ' ';
      V_REASON         := 'Copy_lw_loyaltymember_arc start';
      V_MESSAGE        := 'Copy_lw_loyaltymember_arc start';
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

      OPEN driver;
      LOOP

        FETCH driver BULK COLLECT
          INTO dt LIMIT 1000;


       FORALL I IN 1 .. dt.COUNT

                   insert into lw_loyaltymember_arc
          (id,
           ipcode,
           memberclosedate,
           memberstatus,
           firstname,
           lastname,
           primaryemailaddress,
           primaryphonenumber,
           primarypostalcode,
           lastactivitydate,
           changedby,
           newstatus,
           newstatuseffectivedate,
           last_dml_date,
           statuschangereason,
           birthdate)
        values
          ( seq_rowkey.nextval,
           dt(i).OLD_ipcode,
           dt(i).OLD_memberclosedate,
           dt(i).OLD_memberstatus,
           dt(i).OLD_firstname,
           dt(i).OLD_lastname,
           dt(i).OLD_primaryemailaddress,
           dt(i).OLD_primaryphonenumber,
           dt(i).OLD_primarypostalcode,
           dt(i).OLD_lastactivitydate,
           dt(i).changedby,
           dt(i).OLD_newstatus,
           dt(i).OLD_newstatuseffectivedate,
           dt(i).createdate,
           dt(i).OLD_statuschangereason,
           dt(i).OLD_Birthdate );

        COMMIT;

            EXIT WHEN driver%NOTFOUND;

      END LOOP;
      COMMIT;

      CLOSE driver;

      V_ERROR          :=  ' ';
      V_REASON         := 'Copy_lw_loyaltymember_arc end';
      V_MESSAGE        := 'Copy_lw_loyaltymember_arc end';
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
end;


PROCEDURE RECREATE_INDEXES IS
  V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    --V_FILENAME  VARCHAR2(256) := '';

    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'hub_sync_loyaltymember';

    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);


begin
  V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
      V_JOBNAME   := 'recreate_indexes';
      V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);


     V_REASON := 'recreate_indexes start ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

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

    BEGIN
       EXECUTE IMMEDIATE ' alter trigger BP_AE."LW_LOYALTYMEMBER#" enable';
     EXCEPTION WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'trigger BP_AE."LW_LOYALTYMEMBER#" enable ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
    END;

    BEGIN
       EXECUTE IMMEDIATE ' alter trigger bp_ae.LW_LOYALTYMEMBER_ARC enable   ';
    EXCEPTION
         WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'trigger BP_AE."LW_LOYALTYMEMBER_ARC" enable ' ||
                           Err_Code || ' ' || Err_Msg;
              v_Message := v_Reason;
              v_Error   := v_Reason;
              Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                  p_Envkey    => v_Envkey,
                                  p_Logsource => v_Logsource,
                                  p_Filename  => NULL,
                                  p_Batchid   => v_Batchid,
                                  p_Jobnumber => v_My_Log_Id,
                                  p_Message   => v_Message,
                                  p_Reason    => v_Reason,
                                  p_Error     => v_Error,
                                  p_Trycount  => v_Trycount,
                                  p_Msgtime   => SYSDATE);
              NULL;
    END;



---
     V_REASON := 'recreate_indexes end ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

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
end;


end hub_sync_loyaltymember;
/
