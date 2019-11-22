create or replace package HUB_SYNC_MEMBERDETAILS is

PROCEDURE create_temp_tables;

PROCEDURE Merge_ats_MemberDetails(p_mod INTEGER, p_instances INTEGER);

PROCEDURE Copy_ats_MemberDetails_arc;

PROCEDURE recreate_indexes;


-- Note BP_AE.FINAL_MD must exist for this package to compile properly.
-- The DDL for this is at the bottom of this package spec.

-- NOTE the create_temp_tables procedure of this package must be executed
-- before MERGE_LW_MEMBEDETAILS can be executed properly.

-- Note:
-- MERGE_LW_MEMBERDETAILS is designed to be run in  parallel instances.
-- For example - to run it in 5 parallel instances execute it as
--   MERGE_LW_MEMBERDETAILS (p_mod => 0, p_instances 5);
--   MERGE_LW_MEMBERDETAILS (p_mod => 1, p_instances 5);
--   MERGE_LW_MEMBERDETAILS (p_mod => 2, p_instances 5);
--   MERGE_LW_MEMBERDETAILS (p_mod => 3, p_instances 5);
--   MERGE_LW_MEMBERDETAILS (p_mod => 4, p_instances 5);

-- Any other combinations of parameters will not work properly.
-- To run it in only one instance, use:
--   MERGE_LW_MEMBERDETAILS (p_mod => 0, p_instances 1);

-- NOTE after all instances have finished successfully RECREATE_INDEXES needs to be run.

-- TODO - need to modify the log messages to indicate what the parameters
-- of the running instance are in MERGE_LW_MEMBERDETAILS.

-- TODO - probably want to improve logging and exception handling especially to the DDL.

/*
-- code to create final_MD table:

-- Create table
create table BP_AE.FINAL_MD
(
  old_ipcode                     NUMBER(20) not null,
  old_a_parentrowkey             NUMBER(20),
  old_a_cardtype                 NUMBER(20),
  old_a_changedby                NVARCHAR2(255),
  old_a_passvalidation           NUMBER(1),
  old_a_basebrandid              NUMBER(20),
  old_a_oldcellnumber            NVARCHAR2(50),
  old_a_extendedplaycode         NUMBER(20),
  old_a_emailaddressmailable     NUMBER(1),
  old_a_addresslineone           VARCHAR2(400),
  old_a_addresslinetwo           VARCHAR2(400),
  old_a_addresslinethree         NVARCHAR2(100),
  old_a_addresslinefour          NVARCHAR2(100),
  old_a_city                     VARCHAR2(200),
  old_a_stateorprovince          VARCHAR2(200),
  old_a_ziporpostalcode          VARCHAR2(100),
  old_a_county                   NVARCHAR2(50),
  old_a_country                  VARCHAR2(200),
  old_a_addressmailable          NUMBER,
  old_a_homephone                NVARCHAR2(25),
  old_a_mobilephone              NVARCHAR2(25),
  old_a_workphone                NVARCHAR2(25),
  old_a_secondaryemailaddress    NVARCHAR2(254),
  old_a_memberstatuscode         NUMBER(10),
  old_a_directmailoptindate      TIMESTAMP(4),
  old_a_emailoptindate           TIMESTAMP(4),
  old_a_smsoptindate             TIMESTAMP(4),
  old_a_enrolldate               TIMESTAMP(4),
  old_a_gender                   NVARCHAR2(10),
  old_a_membersource             NUMBER(10),
  old_a_languagepreference       NVARCHAR2(20),
  old_a_securityquestion         NVARCHAR2(255),
  old_a_securityanswer           NVARCHAR2(255),
  old_a_oldemailaddress          NVARCHAR2(100),
  old_a_homestoreid              NUMBER(10),
  old_a_hasemailbonuscredit      NUMBER(1),
  old_a_isunderage               NUMBER(1),
  old_a_employeecode             NUMBER(10),
  old_a_smsoptin                 NUMBER(1),
  old_a_directmailoptin          NUMBER(1),
  old_a_emailoptin               NUMBER(1),
  old_a_hhkey                    NVARCHAR2(20),
  old_statuscode                 NUMBER(20),
  old_createdate                 TIMESTAMP(4),
  old_updatedate                 TIMESTAMP(6),
  old_a_emailaddress             VARCHAR2(600 CHAR),
  old_a_aitupdate                NUMBER,
  old_a_brafirstpurchasedate     TIMESTAMP(4),
  old_a_jeansfirstpurchasedate   TIMESTAMP(4),
  old_a_lastbrastorenumber       NUMBER,
  old_a_lastpurchasepoints       NUMBER,
  old_a_lastbrapurchasedate      TIMESTAMP(4),
  old_a_gwlinked                 INTEGER,
  old_a_gwobjver                 INTEGER,
  old_a_hasgwbonuscredit         NUMBER(1),
  old_a_autoissuereward          NUMBER(1),
  old_a_primarycontact           NUMBER(1),
  old_a_netspend                 FLOAT,
  old_a_downloadmobileapp        NUMBER(1),
  old_a_pendingemailverification NUMBER(1),
  old_a_pendingcellverification  NUMBER(1),
  old_a_nextemailreminderdate    TIMESTAMP(6),
  old_a_storelastjeanspurchased  NVARCHAR2(10),
  old_a_lastjeanspurchasedate    TIMESTAMP(4),
  old_a_programchangedate        DATE,
  old_a_cardclosedate            TIMESTAMP(4),
  old_a_cardopendate             TIMESTAMP(4),
  new_new_ipcode                 NUMBER(20) not null,
  new_a_parentrowkey             NUMBER(20),
  new_a_cardtype                 NUMBER(20),
  new_a_changedby                NVARCHAR2(255),
  new_a_passvalidation           NUMBER(1),
  new_a_basebrandid              NUMBER(20),
  new_a_oldcellnumber            NVARCHAR2(50),
  new_a_extendedplaycode         NUMBER(20),
  new_a_emailaddressmailable     NUMBER(1),
  new_a_addresslineone           VARCHAR2(400),
  new_a_addresslinetwo           VARCHAR2(400),
  new_a_addresslinethree         NVARCHAR2(100),
  new_a_addresslinefour          NVARCHAR2(100),
  new_a_city                     VARCHAR2(200),
  new_a_stateorprovince          VARCHAR2(200),
  new_a_ziporpostalcode          VARCHAR2(100),
  new_a_county                   NVARCHAR2(50),
  new_a_country                  VARCHAR2(200),
  new_a_addressmailable          NUMBER,
  new_a_homephone                NVARCHAR2(25),
  new_a_mobilephone              NVARCHAR2(25),
  new_a_workphone                NVARCHAR2(25),
  new_a_secondaryemailaddress    NVARCHAR2(254),
  new_a_memberstatuscode         NUMBER(10),
  new_a_directmailoptindate      TIMESTAMP(4),
  new_a_emailoptindate           TIMESTAMP(4),
  new_a_smsoptindate             TIMESTAMP(4),
  new_a_enrolldate               TIMESTAMP(4),
  new_a_gender                   NVARCHAR2(10),
  new_a_membersource             NUMBER(10),
  new_a_languagepreference       NVARCHAR2(20),
  new_a_securityquestion         NVARCHAR2(255),
  new_a_securityanswer           NVARCHAR2(255),
  new_a_oldemailaddress          NVARCHAR2(100),
  new_a_homestoreid              NUMBER(10),
  new_a_hasemailbonuscredit      NUMBER(1),
  new_a_isunderage               NUMBER(1),
  new_a_employeecode             NUMBER(10),
  new_a_smsoptin                 NUMBER(1),
  new_a_directmailoptin          NUMBER(1),
  new_a_emailoptin               NUMBER(1),
  new_a_hhkey                    NVARCHAR2(20),
  new_statuscode                 NUMBER(20),
  new_createdate                 TIMESTAMP(4),
  new_updatedate                 TIMESTAMP(6),
  new_a_emailaddress             VARCHAR2(600 CHAR),
  new_a_aitupdate                NUMBER,
  new_a_brafirstpurchasedate     TIMESTAMP(4),
  new_a_jeansfirstpurchasedate   TIMESTAMP(4),
  new_a_lastbrastorenumber       NUMBER,
  new_a_lastpurchasepoints       NUMBER,
  new_a_lastbrapurchasedate      TIMESTAMP(4),
  new_a_gwlinked                 INTEGER,
  new_a_gwobjver                 INTEGER,
  new_a_hasgwbonuscredit         NUMBER(1),
  new_a_autoissuereward          NUMBER(1),
  new_a_primarycontact           NUMBER(1),
  new_a_netspend                 FLOAT,
  new_a_downloadmobileapp        NUMBER(1),
  new_a_pendingemailverification NUMBER(1),
  new_a_pendingcellverification  NUMBER(1),
  new_a_nextemailreminderdate    TIMESTAMP(6),
  new_a_storelastjeanspurchased  NVARCHAR2(10),
  new_a_lastjeanspurchasedate    TIMESTAMP(4),
  new_a_programchangedate        DATE,
  new_a_cardclosedate            TIMESTAMP(4),
  new_a_cardopendate             TIMESTAMP(4)
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
end HUB_SYNC_MEMBERDETAILS;
/
create or replace package body HUB_SYNC_MEMBERDETAILS is

PROCEDURE create_temp_tables IS

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
   -- V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'HUB_SYNC_MEMBERDETAILS';

    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);


begin


     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'hub_sync_memberdetails.create_temp_tables';
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
      V_REASON         := 'hub_sync_memberdetails.create_temp_tables start' ;
      V_MESSAGE        := 'hub_sync_memberdetails.create_temp_tables start' ;
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

/*
BEGIN
     EXECUTE IMMEDIATE 'alter trigger bp_ae.ats_memberdetails# disable';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'alter trigger bp_ae.ATS_MEMBERDETAILS# disable' ||
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

BEGIN
     EXECUTE IMMEDIATE 'alter trigger bp_ae.ats_memberdetails_arc disable';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'alter trigger bp_ae.ats_memberdetails_arc disable' ||
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
*/

     BEGIN
          EXECUTE IMMEDIATE 'drop table bp_ae.old_md purge';
     EXCEPTION
          WHEN OTHERS THEN
               Err_Code  := SQLCODE;
               Err_Msg   := Substr(SQLERRM, 1, 200);
               v_Reason  := 'drop table bp_ae.old_md purge ' || Err_Code || ' ' ||
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
      EXECUTE IMMEDIATE '
     create table bp_ae.old_md tablespace bp_ae_d as
     SELECT /*+use_hash(m,md)*/
              m.Old_Ipcode AS Old_Ipcode,
              m.new_ipcode as new_icpode,
              Md.a_Parentrowkey as   old_a_Parentrowkey,
              Md.a_Cardtype as old_a_Cardtype,
              Md.a_Changedby as old_a_Changedby,
              Md.a_Passvalidation as old_a_Passvalidation,
              Md.a_Basebrandid as old_a_Basebrandid,
              Md.a_Oldcellnumber as old_a_Oldcellnumber,
              Md.a_Extendedplaycode as old_a_Extendedplaycode,
              Md.a_Emailaddressmailable as old_a_Emailaddressmailable,
              Md.a_Addresslineone as old_a_Addresslineone,
              Md.a_Addresslinetwo as old_a_Addresslinetwo,
              Md.a_Addresslinethree as old_a_Addresslinethree,
              Md.a_Addresslinefour as old_a_Addresslinefour,
              Md.a_City as old_a_City,
              Md.a_Stateorprovince as old_a_Stateorprovince,
              Md.a_Ziporpostalcode as old_a_Ziporpostalcode,
              Md.a_County as old_a_County,
              Md.a_Country as old_a_Country,
              Md.a_Addressmailable as old_a_Addressmailable,
              Md.a_Homephone as old_a_Homephone,
              Md.a_Mobilephone as old_a_Mobilephone,
              Md.a_Workphone as old_a_Workphone,
              Md.a_Secondaryemailaddress as old_a_Secondaryemailaddress,
              Md.a_Memberstatuscode as old_a_Memberstatuscode,
              Md.a_Directmailoptindate as old_a_Directmailoptindate,
              Md.a_Emailoptindate as old_a_Emailoptindate,
              Md.a_Smsoptindate as old_a_Smsoptindate,
              Md.a_Enrolldate as old_a_Enrolldate,
              Md.a_Gender as old_a_Gender,
              Md.a_Membersource as old_a_Membersource,
              Md.a_Languagepreference as old_a_Languagepreference,
              Md.a_Securityquestion as old_a_Securityquestion,
              Md.a_Securityanswer as old_a_Securityanswer,
              Md.a_Oldemailaddress as old_a_Oldemailaddress,
              Md.a_Homestoreid as old_a_Homestoreid,
              Md.a_Hasemailbonuscredit as old_a_Hasemailbonuscredit,
              Md.a_Isunderage as old_a_Isunderage,
              Md.a_Employeecode as old_a_Employeecode,
              Md.a_Smsoptin as old_a_Smsoptin,
              Md.a_Directmailoptin as old_a_Directmailoptin,
              Md.a_Emailoptin as old_a_Emailoptin,
              Md.a_Hhkey as old_a_Hhkey,
              Md.Statuscode as old_Statuscode,
              Md.Createdate as old_Createdate,
              Md.Updatedate as old_Updatedate,
              Md.a_Emailaddress as old_a_Emailaddress,
              Md.a_Aitupdate as old_a_Aitupdate,
              Md.a_Brafirstpurchasedate as old_a_Brafirstpurchasedate,
              Md.a_Jeansfirstpurchasedate as old_a_Jeansfirstpurchasedate,
              Md.a_Lastbrastorenumber as old_a_Lastbrastorenumber,
              Md.a_Lastpurchasepoints as old_a_Lastpurchasepoints,
              Md.a_Lastbrapurchasedate as old_a_Lastbrapurchasedate,
              Md.a_Gwlinked as old_a_Gwlinked,
              Md.a_Gwobjver as old_a_Gwobjver,
              Md.a_Hasgwbonuscredit as old_a_Hasgwbonuscredit,
              Md.a_Autoissuereward as old_a_Autoissuereward,
              Md.a_Primarycontact as old_a_Primarycontact,
              Md.a_Netspend as old_a_Netspend,
              Md.a_Downloadmobileapp as old_a_Downloadmobileapp,
              Md.a_Pendingemailverification as old_a_Pendingemailverification,
              Md.a_Pendingcellverification as old_a_Pendingcellverification,
              Md.a_Nextemailreminderdate as old_a_Nextemailreminderdate,
              Md.a_Storelastjeanspurchased as old_a_Storelastjeanspurchased,
              Md.a_Lastjeanspurchasedate as old_a_Lastjeanspurchasedate,
              Md.a_Programchangedate as old_a_Programchangedate,
              Md.a_Cardclosedate as old_a_Cardclosedate,
              Md.a_Cardopendate as old_a_Cardopendate,
              MD.A_TERMINATIONREASONID AS  old_A_TERMINATIONREASONID
             FROM  ( select old_ipcode, new_ipcode
                    from Bp_Ae.Ae_Merge
                    group by old_ipcode, new_ipcode) m,
             Bp_Ae.Ats_Memberdetails Md
             WHERE  Md.a_Ipcode = m.Old_Ipcode';
 EXCEPTION
      WHEN OTHERS THEN
           Err_Code  := SQLCODE;
           Err_Msg   := Substr(SQLERRM, 1, 200);
           v_Reason  := 'create table bp_ae.old_md ' || Err_Code || ' ' ||
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
          EXECUTE IMMEDIATE 'drop table bp_ae.new_md purge';
     EXCEPTION
          WHEN OTHERS THEN
               Err_Code  := SQLCODE;
               Err_Msg   := Substr(SQLERRM, 1, 200);
               v_Reason  := 'drop table bp_ae.new_md purge ' || Err_Code || ' ' ||
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
          EXECUTE IMMEDIATE '
          create table bp_ae.new_md tablespace bp_ae_d as
            SELECT /*+use_hash(m,md)*/
              m.New_Ipcode as new_New_Ipcode,
              m.old_ipcode as new_old_ipcode,
              Md.a_Parentrowkey as new_a_Parentrowkey,
              Md.a_Cardtype as new_a_Cardtype,
              Md.a_Changedby as new_a_Changedby,
              Md.a_Passvalidation as new_a_Passvalidation,
              Md.a_Basebrandid as new_a_Basebrandid,
              Md.a_Oldcellnumber as new_a_Oldcellnumber,
              Md.a_Extendedplaycode as new_a_Extendedplaycode,
              Md.a_Emailaddressmailable as new_a_Emailaddressmailable,
              Md.a_Addresslineone as new_a_Addresslineone,
              Md.a_Addresslinetwo as new_a_Addresslinetwo,
              Md.a_Addresslinethree as new_a_Addresslinethree,
              Md.a_Addresslinefour as new_a_Addresslinefour,
              Md.a_City as new_a_City,
              Md.a_Stateorprovince as new_a_Stateorprovince,
              Md.a_Ziporpostalcode as new_a_Ziporpostalcode,
              Md.a_County as new_a_County,
              Md.a_Country as new_a_Country,
              Md.a_Addressmailable as new_a_Addressmailable,
              Md.a_Homephone as new_a_Homephone,
              Md.a_Mobilephone as new_a_Mobilephone,
              Md.a_Workphone as new_a_Workphone,
              Md.a_Secondaryemailaddress as new_a_Secondaryemailaddress,
              Md.a_Memberstatuscode as new_a_Memberstatuscode,
              Md.a_Directmailoptindate as new_a_Directmailoptindate,
              Md.a_Emailoptindate as new_a_Emailoptindate,
              Md.a_Smsoptindate as new_a_Smsoptindate,
              Md.a_Enrolldate as new_a_Enrolldate,
              Md.a_Gender as new_a_Gender,
              Md.a_Membersource as new_a_Membersource,
              Md.a_Languagepreference as new_a_Languagepreference,
              Md.a_Securityquestion as new_a_Securityquestion,
              Md.a_Securityanswer as new_a_Securityanswer,
              Md.a_Oldemailaddress as new_a_Oldemailaddress,
              Md.a_Homestoreid as new_a_Homestoreid,
              Md.a_Hasemailbonuscredit as new_a_Hasemailbonuscredit,
              Md.a_Isunderage as new_a_Isunderage,
              Md.a_Employeecode as new_a_Employeecode,
              Md.a_Smsoptin as new_a_Smsoptin,
              Md.a_Directmailoptin as new_a_Directmailoptin,
              Md.a_Emailoptin as new_a_Emailoptin,
              Md.a_Hhkey as new_a_Hhkey,
              Md.Statuscode as new_Statuscode,
              Md.Createdate as new_Createdate,
              Md.Updatedate as new_Updatedate,
              Md.a_Emailaddress as new_a_Emailaddress,
              Md.a_Aitupdate as new_a_Aitupdate,
              Md.a_Brafirstpurchasedate as new_a_Brafirstpurchasedate,
              Md.a_Jeansfirstpurchasedate as new_a_Jeansfirstpurchasedate,
              Md.a_Lastbrastorenumber as new_a_Lastbrastorenumber,
              Md.a_Lastpurchasepoints as new_a_Lastpurchasepoints,
              Md.a_Lastbrapurchasedate as new_a_Lastbrapurchasedate,
              Md.a_Gwlinked as new_a_Gwlinked,
              Md.a_Gwobjver as new_a_Gwobjver,
              Md.a_Hasgwbonuscredit as new_a_Hasgwbonuscredit,
              Md.a_Autoissuereward as new_a_Autoissuereward,
              Md.a_Primarycontact as new_a_Primarycontact,
              Md.a_Netspend as new_a_Netspend,
              Md.a_Downloadmobileapp as new_a_Downloadmobileapp,
              Md.a_Pendingemailverification as new_a_Pendingemailverification,
              Md.a_Pendingcellverification as new_a_Pendingcellverification,
              Md.a_Nextemailreminderdate as new_a_Nextemailreminderdate,
              Md.a_Storelastjeanspurchased as new_a_Storelastjeanspurchased,
              Md.a_Lastjeanspurchasedate as new_a_Lastjeanspurchasedate,
              Md.a_Programchangedate as new_a_Programchangedate,
              Md.a_Cardclosedate as new_a_Cardclosedate,
              Md.a_Cardopendate as new_a_Cardopendate,
              md.A_TERMINATIONREASONID as new_A_TERMINATIONREASONID
             FROM  ( select old_ipcode, new_ipcode
                    from Bp_Ae.Ae_Merge
                    group by old_ipcode, new_ipcode) m, Bp_Ae.Ats_Memberdetails Md
             WHERE  Md.a_Ipcode = m.New_Ipcode';
     EXCEPTION
          WHEN OTHERS THEN
               Err_Code  := SQLCODE;
               Err_Msg   := Substr(SQLERRM, 1, 200);
               v_Reason  := 'drop table bp_ae.new_md purge ' || Err_Code || ' ' ||
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
          EXECUTE IMMEDIATE 'drop table bp_ae.final_md purge';
     EXCEPTION
          WHEN OTHERS THEN
               Err_Code  := SQLCODE;
               Err_Msg   := Substr(SQLERRM, 1, 200);
               v_Reason  := ' create table bp_ae.new_md ' || Err_Code || ' ' ||
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
          EXECUTE IMMEDIATE '
     create table bp_ae.final_md tablespace bp_ae_d as
     SELECT /*+use_hash(o,n)*/
      o.Old_Ipcode,
      Old_a_Parentrowkey             AS Old_a_Parentrowkey,
      Old_a_Cardtype                 AS Old_a_Cardtype,
      Old_a_Changedby                AS Old_a_Changedby,
      Old_a_Passvalidation           AS Old_a_Passvalidation,
      Old_a_Basebrandid              AS Old_a_Basebrandid,
      Old_a_Oldcellnumber            AS Old_a_Oldcellnumber,
      Old_a_Extendedplaycode         AS Old_a_Extendedplaycode,
      Old_a_Emailaddressmailable     AS Old_a_Emailaddressmailable,
      Old_a_Addresslineone           AS Old_a_Addresslineone,
      Old_a_Addresslinetwo           AS Old_a_Addresslinetwo,
      Old_a_Addresslinethree         AS Old_a_Addresslinethree,
      Old_a_Addresslinefour          AS Old_a_Addresslinefour,
      Old_a_City                     AS Old_a_City,
      Old_a_Stateorprovince          AS Old_a_Stateorprovince,
      Old_a_Ziporpostalcode          AS Old_a_Ziporpostalcode,
      Old_a_County                   AS Old_a_County,
      Old_a_Country                  AS Old_a_Country,
      Old_a_Addressmailable          AS Old_a_Addressmailable,
      Old_a_Homephone                AS Old_a_Homephone,
      Old_a_Mobilephone              AS Old_a_Mobilephone,
      Old_a_Workphone                AS Old_a_Workphone,
      Old_a_Secondaryemailaddress    AS Old_a_Secondaryemailaddress,
      Old_a_Memberstatuscode         AS Old_a_Memberstatuscode,
      Old_a_Directmailoptindate      AS Old_a_Directmailoptindate,
      Old_a_Emailoptindate           AS Old_a_Emailoptindate,
      Old_a_Smsoptindate             AS Old_a_Smsoptindate,
      Old_a_Enrolldate               AS Old_a_Enrolldate,
      Old_a_Gender                   AS Old_a_Gender,
      Old_a_Membersource             AS Old_a_Membersource,
      Old_a_Languagepreference       AS Old_a_Languagepreference,
      Old_a_Securityquestion         AS Old_a_Securityquestion,
      Old_a_Securityanswer           AS Old_a_Securityanswer,
      Old_a_Oldemailaddress          AS Old_a_Oldemailaddress,
      Old_a_Homestoreid              AS Old_a_Homestoreid,
      Old_a_Hasemailbonuscredit      AS Old_a_Hasemailbonuscredit,
      Old_a_Isunderage               AS Old_a_Isunderage,
      Old_a_Employeecode             AS Old_a_Employeecode,
      Old_a_Smsoptin                 AS Old_a_Smsoptin,
      Old_a_Directmailoptin          AS Old_a_Directmailoptin,
      Old_a_Emailoptin               AS Old_a_Emailoptin,
      Old_a_Hhkey                    AS Old_a_Hhkey,
      Old_Statuscode                 AS Old_Statuscode,
      Old_Createdate                 AS Old_Createdate,
      Old_Updatedate                 AS Old_Updatedate,
      Old_a_Emailaddress             AS Old_a_Emailaddress,
      Old_a_Aitupdate                AS Old_a_Aitupdate,
      Old_a_Brafirstpurchasedate     AS Old_a_Brafirstpurchasedate,
      Old_a_Jeansfirstpurchasedate   AS Old_a_Jeansfirstpurchasedate,
      Old_a_Lastbrastorenumber       AS Old_a_Lastbrastorenumber,
      Old_a_Lastpurchasepoints       AS Old_a_Lastpurchasepoints,
      Old_a_Lastbrapurchasedate      AS Old_a_Lastbrapurchasedate,
      Old_a_Gwlinked                 AS Old_a_Gwlinked,
      Old_a_Gwobjver                 AS Old_a_Gwobjver,
      Old_a_Hasgwbonuscredit         AS Old_a_Hasgwbonuscredit,
      Old_a_Autoissuereward          AS Old_a_Autoissuereward,
      Old_a_Primarycontact           AS Old_a_Primarycontact,
      Old_a_Netspend                 AS Old_a_Netspend,
      Old_a_Downloadmobileapp        AS Old_a_Downloadmobileapp,
      Old_a_Pendingemailverification AS Old_a_Pendingemailverification,
      Old_a_Pendingcellverification  AS Old_a_Pendingcellverification,
      Old_a_Nextemailreminderdate    AS Old_a_Nextemailreminderdate,
      Old_a_Storelastjeanspurchased  AS Old_a_Storelastjeanspurchased,
      Old_a_Lastjeanspurchasedate    AS Old_a_Lastjeanspurchasedate,
      Old_a_Programchangedate        AS Old_a_Programchangedate,
      Old_a_Cardclosedate            AS Old_a_Cardclosedate,
      Old_a_Cardopendate             Old_a_Cardopendate,
      old_A_TERMINATIONREASONID      as old_A_TERMINATIONREASONID,
      new_New_Ipcode,
      New_a_Parentrowkey             AS New_a_Parentrowkey,
      New_a_Cardtype                 AS New_a_Cardtype,
      New_a_Changedby                AS New_a_Changedby,
      New_a_Passvalidation           AS New_a_Passvalidation,
      New_a_Basebrandid              AS New_a_Basebrandid,
      New_a_Oldcellnumber            AS New_a_Oldcellnumber,
      New_a_Extendedplaycode         AS New_a_Extendedplaycode,
      New_a_Emailaddressmailable     AS New_a_Emailaddressmailable,
      New_a_Addresslineone           AS New_a_Addresslineone,
      New_a_Addresslinetwo           AS New_a_Addresslinetwo,
      New_a_Addresslinethree         AS New_a_Addresslinethree,
      New_a_Addresslinefour          AS New_a_Addresslinefour,
      New_a_City                     AS New_a_City,
      New_a_Stateorprovince          AS New_a_Stateorprovince,
      New_a_Ziporpostalcode          AS New_a_Ziporpostalcode,
      New_a_County                   AS New_a_County,
      New_a_Country                  AS New_a_Country,
      New_a_Addressmailable          AS New_a_Addressmailable,
      New_a_Homephone                AS New_a_Homephone,
      New_a_Mobilephone              AS New_a_Mobilephone,
      New_a_Workphone                AS New_a_Workphone,
      New_a_Secondaryemailaddress    AS New_a_Secondaryemailaddress,
      New_a_Memberstatuscode         AS New_a_Memberstatuscode,
      New_a_Directmailoptindate      AS New_a_Directmailoptindate,
      New_a_Emailoptindate           AS New_a_Emailoptindate,
      New_a_Smsoptindate             AS New_a_Smsoptindate,
      New_a_Enrolldate               AS New_a_Enrolldate,
      New_a_Gender                   AS New_a_Gender,
      New_a_Membersource             AS New_a_Membersource,
      New_a_Languagepreference       AS New_a_Languagepreference,
      New_a_Securityquestion         AS New_a_Securityquestion,
      New_a_Securityanswer           AS New_a_Securityanswer,
      New_a_Oldemailaddress          AS New_a_Oldemailaddress,
      New_a_Homestoreid              AS New_a_Homestoreid,
      New_a_Hasemailbonuscredit      AS New_a_Hasemailbonuscredit,
      New_a_Isunderage               AS New_a_Isunderage,
      New_a_Employeecode             AS New_a_Employeecode,
      New_a_Smsoptin                 AS New_a_Smsoptin,
      New_a_Directmailoptin          AS New_a_Directmailoptin,
      New_a_Emailoptin               AS New_a_Emailoptin,
      New_a_Hhkey                    AS New_a_Hhkey,
      New_Statuscode                 AS New_Statuscode,
      New_Createdate                 AS New_Createdate,
      New_Updatedate                 AS New_Updatedate,
      New_a_Emailaddress             AS New_a_Emailaddress,
      New_a_Aitupdate                AS New_a_Aitupdate,
      New_a_Brafirstpurchasedate     AS New_a_Brafirstpurchasedate,
      New_a_Jeansfirstpurchasedate   AS New_a_Jeansfirstpurchasedate,
      New_a_Lastbrastorenumber       AS New_a_Lastbrastorenumber,
      New_a_Lastpurchasepoints       AS New_a_Lastpurchasepoints,
      New_a_Lastbrapurchasedate      AS New_a_Lastbrapurchasedate,
      New_a_Gwlinked                 AS New_a_Gwlinked,
      New_a_Gwobjver                 AS New_a_Gwobjver,
      New_a_Hasgwbonuscredit         AS New_a_Hasgwbonuscredit,
      New_a_Autoissuereward          AS New_a_Autoissuereward,
      New_a_Primarycontact           AS New_a_Primarycontact,
      New_a_Netspend                 AS New_a_Netspend,
      New_a_Downloadmobileapp        AS New_a_Downloadmobileapp,
      New_a_Pendingemailverification AS New_a_Pendingemailverification,
      New_a_Pendingcellverification  AS New_a_Pendingcellverification,
      New_a_Nextemailreminderdate    AS New_a_Nextemailreminderdate,
      New_a_Storelastjeanspurchased  AS New_a_Storelastjeanspurchased,
      New_a_Lastjeanspurchasedate    AS New_a_Lastjeanspurchasedate,
      New_a_Programchangedate        AS New_a_Programchangedate,
      New_a_Cardclosedate            AS New_a_Cardclosedate,
      New_a_Cardopendate             New_a_Cardopendate,
      new_A_TERMINATIONREASONID      as new_A_TERMINATIONREASONID
     from bp_ae.old_md o,
     bp_ae.new_md n
     where o.old_ipcode = n.new_old_ipcode
      and o.new_icpode = n.new_new_ipcode';
     EXCEPTION
          WHEN OTHERS THEN
               Err_Code  := SQLCODE;
               Err_Msg   := Substr(SQLERRM, 1, 200);
               v_Reason  := ' create table bp_ae.final_md ' || Err_Code || ' ' ||
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

      V_ERROR          :=  ' ';
      V_REASON         := 'hub_sync_memberdetails.create_temp_tables end' ;
      V_MESSAGE        := 'hub_sync_memberdetails.create_temp_tables end' ;
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


PROCEDURE Merge_ats_MemberDetails (p_mod INTEGER, p_instances INTEGER) IS

     CURSOR Driver IS
     SELECT *
       FROM bp_ae.final_md a
      WHERE mod(a.old_ipcode, p_instances) = p_mod;

     TYPE Driver_Table IS TABLE OF Driver%ROWTYPE;
     Dt Driver_Table;
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
     V_JOBNAME   := 'Merge_ats_MemberDetails';
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
      V_REASON         := 'Merge_ats_MemberDetails start';
      V_MESSAGE        := 'Merge_ats_MemberDetails start';
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

                             -- disable the trigger

     OPEN Driver;
     LOOP
          FETCH Driver BULK COLLECT
               INTO Dt LIMIT 100;
          FORALL i IN 1 .. Dt.Count
            UPDATE Bp_Ae.Ats_Memberdetails Md
            SET
                   Md.a_Jeansfirstpurchasedate = CASE
                                                      WHEN (Dt(i)
                                                           .Old_a_Jeansfirstpurchasedate IS NOT NULL AND Dt(i)
                                                           .New_a_Jeansfirstpurchasedate IS NULL) THEN
                                                       Dt(i)
                                                       .Old_a_Jeansfirstpurchasedate
                                                      ELSE
                                                       Md.a_Jeansfirstpurchasedate
                                                 END,
                   Md.a_Lastbrastorenumber = CASE
                                                  WHEN (Dt(i)
                                                       .Old_a_Lastbrastorenumber IS NOT NULL AND Dt(i)
                                                       .New_a_Lastbrastorenumber IS NULL) THEN
                                                   Dt(i).Old_a_Lastbrastorenumber
                                                  ELSE
                                                   Md.a_Lastbrastorenumber
                                             END,
                   Md.a_Lastpurchasepoints = CASE
                                                  WHEN (Dt(i)
                                                       .Old_a_Lastpurchasepoints IS NOT NULL AND Dt(i)
                                                       .New_a_Lastpurchasepoints IS NULL) THEN
                                                   Dt(i).Old_a_Lastpurchasepoints
                                                  ELSE
                                                   Md.a_Lastpurchasepoints
                                             END,
                   Md.a_Lastbrapurchasedate = CASE
                                                   WHEN (Dt(i)
                                                        .Old_a_Lastbrapurchasedate IS NOT NULL AND Dt(i)
                                                        .New_a_Lastbrapurchasedate IS NULL) THEN
                                                    Dt(i)
                                                    .Old_a_Lastbrapurchasedate
                                                   ELSE
                                                    Md.a_Lastbrapurchasedate
                                              END,
                   Md.a_Gwlinked = CASE
                                        WHEN (Dt(i).Old_a_Gwlinked IS NOT NULL AND Dt(i)
                                             .New_a_Gwlinked IS NULL) THEN
                                         Dt(i).Old_a_Gwlinked
                                        ELSE
                                         Md.a_Gwlinked
                                   END,
                   Md.Createdate = CASE
                                        WHEN (Dt(i).Old_Createdate IS NOT NULL AND Dt(i)
                                             .New_Createdate IS NULL) THEN
                                         Dt(i).Old_Createdate
                                        ELSE
                                         Md.Createdate
                                   END,
                   Md.a_Emailaddress = CASE
                                            WHEN (Dt(i).Old_a_Emailaddress IS NOT NULL AND Dt(i)
                                                 .New_a_Emailaddress IS NULL) THEN
                                             cast((Dt(i).Old_a_Emailaddress) as  nvarchar2(150))
                                            ELSE
                                             cast(to_char(Md.a_Emailaddress) AS NVARCHAR2(150))
                                       END,
                   Md.a_Aitupdate = CASE
                                         WHEN (Dt(i).Old_a_Aitupdate=1 OR Dt(i).new_a_Aitupdate =1) THEN
                                          1
                                         ELSE
                                          0
                                    END,
                   Md.a_Brafirstpurchasedate = CASE
                                                    WHEN (Dt(i)
                                                         .Old_a_Brafirstpurchasedate IS NOT NULL AND Dt(i)
                                                         .New_a_Brafirstpurchasedate IS NULL) THEN
                                                     Dt(i)
                                                     .Old_a_Brafirstpurchasedate
                                                    ELSE
                                                     Md.a_Brafirstpurchasedate
                                               END,
                   Md.a_Smsoptin = CASE
                                        WHEN (Dt(i).Old_a_Smsoptin IS NOT NULL AND Dt(i)
                                             .New_a_Smsoptin IS NULL) THEN
                                         Dt(i).Old_a_Smsoptin
                                        ELSE
                                         Md.a_Smsoptin
                                   END,
                   Md.a_Directmailoptin = CASE
                                               WHEN (Dt(i)
                                                    .Old_a_Directmailoptin IS NOT NULL AND Dt(i)
                                                    .New_a_Directmailoptin IS NULL) THEN
                                                Dt(i).Old_a_Directmailoptin
                                               ELSE
                                                Md.a_Directmailoptin
                                          END,
                   Md.a_Emailoptin = CASE
                                          WHEN (Dt(i).Old_a_Emailoptin IS NOT NULL AND Dt(i)
                                               .New_a_Emailoptin IS NULL) THEN
                                           Dt(i).Old_a_Emailoptin
                                          ELSE
                                           Md.a_Emailoptin
                                     END,
                   Md.a_Hhkey = CASE
                                     WHEN (Dt(i).Old_a_Hhkey IS NOT NULL AND Dt(i)
                                          .New_a_Hhkey IS NULL) THEN
                                      Dt(i).Old_a_Hhkey
                                     ELSE
                                      Md.a_Hhkey
                                END,
                   Md.Statuscode = CASE
                                        WHEN (Dt(i).Old_Statuscode IS NOT NULL AND Dt(i)
                                             .New_Statuscode IS NULL) THEN
                                         Dt(i).Old_Statuscode
                                        ELSE
                                         Md.Statuscode
                                   END,
                   Md.a_Lastjeanspurchasedate = CASE
                                                     WHEN (Dt(i)
                                                          .Old_a_Lastjeanspurchasedate IS NOT NULL AND Dt(i)
                                                          .New_a_Lastjeanspurchasedate IS NULL) THEN
                                                      Dt(i)
                                                      .Old_a_Lastjeanspurchasedate
                                                     ELSE
                                                      Md.a_Lastjeanspurchasedate
                                                END,
                   Md.a_Storelastjeanspurchased = CASE
                                                       WHEN (Dt(i)
                                                            .Old_a_Storelastjeanspurchased IS NOT NULL AND Dt(i)
                                                            .New_a_Storelastjeanspurchased IS NULL) THEN
                                                        Dt(i)
                                                        .Old_a_Storelastjeanspurchased
                                                       ELSE
                                                        Md.a_Storelastjeanspurchased
                                                  END,
                   Md.a_Nextemailreminderdate = CASE
                                                     WHEN (Dt(i)
                                                          .Old_a_Nextemailreminderdate IS NOT NULL AND Dt(i)
                                                          .New_a_Nextemailreminderdate IS NULL) THEN
                                                      Dt(i)
                                                      .Old_a_Nextemailreminderdate
                                                     ELSE
                                                      Md.a_Nextemailreminderdate
                                                END,
                   Md.a_Pendingcellverification = CASE
                                                       WHEN (Dt(i)
                                                            .Old_a_Pendingcellverification IS NOT NULL AND Dt(i)
                                                            .New_a_Pendingcellverification IS NULL) THEN
                                                        Dt(i)
                                                        .Old_a_Pendingcellverification
                                                       ELSE
                                                        Md.a_Pendingcellverification
                                                  END,
                   Md.a_Pendingemailverification = CASE
                                                        WHEN (Dt(i)
                                                             .Old_a_Pendingemailverification IS NOT NULL AND Dt(i)
                                                             .New_a_Pendingemailverification IS NULL) THEN
                                                         Dt(i)
                                                         .Old_a_Pendingemailverification
                                                        ELSE
                                                         Md.a_Pendingemailverification
                                                   END,
                   Md.a_Downloadmobileapp = CASE
                                                 WHEN (Dt(i)
                                                      .Old_a_Downloadmobileapp IS NOT NULL AND Dt(i)
                                                      .New_a_Downloadmobileapp IS NULL) THEN
                                                  Dt(i).Old_a_Downloadmobileapp
                                                 ELSE
                                                  Md.a_Downloadmobileapp
                                            END,
                      Md.a_Netspend  = NVL(Dt(i).Old_a_Netspend,0) + nvl(Dt(i).New_a_Netspend,0),
                      Md.a_Autoissuereward = CASE
                                               WHEN (Dt(i)
                                                    .Old_a_Autoissuereward IS NOT NULL AND Dt(i)
                                                    .New_a_Autoissuereward IS NULL) THEN
                                                Dt(i).Old_a_Autoissuereward
                                               ELSE
                                                Md.a_Autoissuereward
                                          END,
                   Md.a_Hasgwbonuscredit = CASE
                                                WHEN (Dt(i)
                                                     .Old_a_Hasgwbonuscredit IS NOT NULL AND Dt(i)
                                                     .New_a_Hasgwbonuscredit IS NULL) THEN
                                                 Dt(i).Old_a_Hasgwbonuscredit
                                                ELSE
                                                 Md.a_Hasgwbonuscredit
                                           END,
                   Md.a_Gwobjver = CASE
                                        WHEN (Dt(i).Old_a_Gwobjver IS NOT NULL AND Dt(i)
                                             .New_a_Gwobjver IS NULL) THEN
                                         Dt(i).Old_a_Gwobjver
                                        ELSE
                                         Md.a_Gwobjver
                                   END,
                   Md.a_Isunderage = CASE
                                          WHEN (Dt(i).Old_a_Isunderage IS NOT NULL AND Dt(i)
                                               .New_a_Isunderage IS NULL) THEN
                                           Dt(i).Old_a_Isunderage
                                          ELSE
                                           Md.a_Isunderage
                                     END,
                   Md.a_Employeecode = CASE
                                            WHEN (Dt(i).Old_a_Employeecode IS NOT NULL AND Dt(i)
                                                 .New_a_Employeecode IS NULL) THEN
                                             Dt(i).Old_a_Employeecode
                                            ELSE
                                             Md.a_Employeecode
                                       END,
                   Md.a_Cardopendate = CASE
                                            WHEN (Dt(i).Old_a_Cardopendate IS NOT NULL AND Dt(i)
                                                 .New_a_Cardopendate IS NULL) THEN
                                             Dt(i).Old_a_Cardopendate
                                            ELSE
                                             Md.a_Cardopendate
                                       END,
                   Md.a_Cardclosedate = CASE
                                             WHEN (Dt(i)
                                                  .Old_a_Cardclosedate IS NOT NULL AND Dt(i)
                                                  .New_a_Cardclosedate IS NULL) THEN
                                              Dt(i).Old_a_Cardclosedate
                                             ELSE
                                              Md.a_Cardclosedate
                                        END,
                                        -- AEO-1150 begin
                                        /*
                   Md.a_Programchangedate = CASE
                                                 WHEN (Dt(i).Old_a_Programchangedate IS NOT NULL AND
                                                       Dt(i).New_a_Programchangedate IS NULL) THEN
                                                       Dt(i).Old_a_Programchangedate
                                                 ELSE
                                                  Md.a_Programchangedate
                                            END,
                                            */
                                        -- AEO-1150 end
                   Md.a_Cardtype = CASE
                                        WHEN (Dt(i).Old_a_Cardtype IS NOT NULL AND Dt(i)
                                             .New_a_Cardtype IS NULL) THEN
                                         Dt(i).Old_a_Cardtype
                                        ELSE
                                         Md.a_Cardtype
                                   END,
                   Md.a_Changedby = CASE
                                         WHEN (Dt(i).Old_a_Changedby IS NOT NULL AND Dt(i)
                                              .New_a_Changedby IS NULL) THEN
                                          Dt(i).Old_a_Changedby
                                         ELSE
                                          Md.a_Changedby
                                    END,
                   Md.a_Passvalidation = CASE
                                              WHEN (Dt(i)
                                                   .Old_a_Passvalidation IS NOT NULL AND Dt(i)
                                                   .New_a_Passvalidation IS NULL) THEN
                                               Dt(i).Old_a_Passvalidation
                                              ELSE
                                               Md.a_Passvalidation
                                         END,
                   Md.a_Basebrandid = CASE
                                           WHEN (Dt(i).Old_a_Basebrandid IS NOT NULL AND Dt(i)
                                                .New_a_Basebrandid IS NULL) THEN
                                            Dt(i).Old_a_Basebrandid
                                           ELSE
                                            Md.a_Basebrandid
                                      END,
                   Md.a_Oldcellnumber = CASE
                                             WHEN (Dt(i)
                                                  .Old_a_Oldcellnumber IS NOT NULL AND Dt(i)
                                                  .New_a_Oldcellnumber IS NULL) THEN
                                              Dt(i).Old_a_Oldcellnumber
                                             ELSE
                                              Md.a_Oldcellnumber
                                        END,
                                        -- AEO-1150 begin
                                        /*
                   Md.a_Extendedplaycode = CASE
                                                WHEN (Dt(i).Old_a_Extendedplaycode IS NOT NULL AND
                                                      Dt(i).New_a_Extendedplaycode IS NULL) THEN
                                                   Dt(i).Old_a_Extendedplaycode
                                                ELSE
                                                 Md.a_Extendedplaycode
                                           END,*/
                                        -- AEO-1150 end
                   /* AEO--1108 begin */                     
                   /*Md.a_Emailaddressmailable = CASE
                                                    WHEN (Dt(i)
                                                         .Old_a_Emailaddressmailable IS NOT NULL AND Dt(i)
                                                         .New_a_Emailaddressmailable IS NULL) THEN
                                                     Dt(i)
                                                     .Old_a_Emailaddressmailable
                                                    ELSE
                                                     Md.a_Emailaddressmailable
                                               END,*/
                    Md.a_Emailaddressmailable =CASE WHEN ( md.a_emailaddress IS NOT NULL AND   
                                                        REGEXP_LIKE (md.a_emailaddress,
                                                        '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$')) THEN  
                                                 1
                                               ELSE
                                               0
                                               END,                                   
                  /* AEO-1108 end */      
                   
                                   
                   Md.a_Addresslineone = CASE
                                              WHEN (Dt(i)
                                                   .Old_a_Addresslineone IS NOT NULL AND Dt(i)
                                                   .New_a_Addresslineone IS NULL) THEN
                                        cast((Dt(i).old_a_Addresslineone) as  varchar2(400))
                                              ELSE
                                               cast(to_char(Md.a_Addresslineone) AS VARCHAR2(400))
                                         END,
                   Md.a_Addresslinetwo = CASE
                                              WHEN (Dt(i)
                                                   .Old_a_Addresslinetwo IS NOT NULL AND Dt(i)
                                                   .New_a_Addresslinetwo IS NULL) THEN
                                        cast((Dt(i).Old_a_Addresslinetwo) as  varchar2(400))

                                              ELSE
                                               cast(to_char(Md.a_Addresslinetwo) AS VARCHAR2(400))
                                         END,
                   Md.a_Addresslinethree = CASE
                                                WHEN (Dt(i)
                                                     .Old_a_Addresslinethree IS NOT NULL AND Dt(i)
                                                     .New_a_Addresslinethree IS NULL) THEN
                                                       cast(to_char(  Dt(i).Old_a_Addresslinethree) AS nVARCHAR2(100))

                                                ELSE

                                                   cast(to_char(  Md.a_Addresslinethree) AS nVARCHAR2(100))
                                           END,
                   Md.a_Addresslinefour = CASE
                                               WHEN (Dt(i)
                                                    .Old_a_Addresslinefour IS NOT NULL AND Dt(i)
                                                    .New_a_Addresslinefour IS NULL) THEN
                                                            cast(to_char(  Dt(i).Old_a_Addresslinefour) AS nVARCHAR2(100))

                                                ELSE

                                                   cast(to_char(  Md.a_Addresslinefour) AS nVARCHAR2(100))
                                          END,
                   Md.a_City = CASE
                                    WHEN (Dt(i).Old_a_City IS NOT NULL AND Dt(i)
                                         .New_a_City IS NULL) THEN
                                        cast((Dt(i).Old_a_City) as  varchar2(200))
                                    ELSE
                                     CAST(to_char(Md.a_City) AS VARCHAR2(200))
                               END,
                   Md.a_Stateorprovince = CASE
                                               WHEN (Dt(i)
                                                    .Old_a_Stateorprovince IS NOT NULL AND Dt(i)
                                                    .New_a_Stateorprovince IS NULL) THEN
                                        cast((Dt(i).Old_a_Stateorprovince) as  varchar2(200))
                                               ELSE
                                              CAST( to_char(Md.a_Stateorprovince) AS VARCHAR2(200))
                                          END,
                   Md.a_Parentrowkey = CASE
                                            WHEN (Dt(i).Old_a_Parentrowkey IS NOT NULL AND Dt(i)
                                                 .New_a_Parentrowkey IS NULL) THEN
                                             Dt(i).Old_a_Parentrowkey
                                            ELSE
                                             Md.a_Parentrowkey
                                       END,
                   Md.a_County = CASE
                                      WHEN (Dt(i).Old_a_County IS NOT NULL AND Dt(i)
                                           .New_a_County IS NULL) THEN
                                       Dt(i).Old_a_County
                                      ELSE
                                       Md.a_County
                                 END,
                   Md.a_Country = CASE
                                       WHEN (Dt(i).Old_a_Country IS NOT NULL AND Dt(i)
                                            .New_a_Country IS NULL) THEN
                                        cast((Dt(i).Old_a_Country) as  varchar2(200))
                                       ELSE
                                        CAST(to_char(Md.a_Country) AS VARCHAR2(200))
                                  END,
                                   
                   Md.a_Addressmailable = CASE
                                               WHEN (Dt(i)
                                                    .Old_a_Addressmailable IS NOT NULL AND Dt(i)
                                                    .New_a_Addressmailable IS NULL) THEN
                                                Dt(i).Old_a_Addressmailable
                                               ELSE
                                                Md.a_Addressmailable
                                          END,
                                         
                   Md.a_Homephone = CASE
                                         WHEN (Dt(i).Old_a_Homephone IS NOT NULL AND Dt(i)
                                              .New_a_Homephone IS NULL) THEN
                                          Dt(i).Old_a_Homephone
                                         ELSE
                                          Md.a_Homephone
                                    END,
                   Md.a_Mobilephone = CASE
                                           WHEN (Dt(i).Old_a_Mobilephone IS NOT NULL AND Dt(i)
                                                .New_a_Mobilephone IS NULL) THEN
                                            Dt(i).Old_a_Mobilephone
                                           ELSE
                                            Md.a_Mobilephone
                                      END,
                   Md.a_Oldemailaddress = CASE
                                               WHEN (Dt(i)
                                                    .Old_a_Oldemailaddress IS NOT NULL AND Dt(i)
                                                    .New_a_Oldemailaddress IS NULL) THEN
                                                Dt(i).Old_a_Oldemailaddress
                                               ELSE
                                                Md.a_Oldemailaddress
                                          END,
                   Md.a_Homestoreid = CASE
                                           WHEN (Dt(i).Old_a_Homestoreid IS NOT NULL AND Dt(i)
                                                .New_a_Homestoreid IS NULL) THEN
                                            Dt(i).Old_a_Homestoreid
                                           ELSE
                                            Md.a_Homestoreid
                                      END,
                   Md.a_Hasemailbonuscredit = CASE
                                                   WHEN (Dt(i)
                                                        .Old_a_Hasemailbonuscredit IS NOT NULL AND Dt(i)
                                                        .New_a_Hasemailbonuscredit IS NULL) THEN
                                                    Dt(i)
                                                    .Old_a_Hasemailbonuscredit
                                                   ELSE
                                                    Md.a_Hasemailbonuscredit
                                              END,
                   Md.a_Securityanswer = CASE
                                              WHEN (Dt(i)
                                                   .Old_a_Securityanswer IS NOT NULL AND Dt(i)
                                                   .New_a_Securityanswer IS NULL) THEN
                                               Dt(i).Old_a_Securityanswer
                                              ELSE
                                               Md.a_Securityanswer
                                         END,
                   Md.a_Securityquestion = CASE
                                                WHEN (Dt(i)
                                                     .Old_a_Securityquestion IS NOT NULL AND Dt(i)
                                                     .New_a_Securityquestion IS NULL) THEN
                                                 Dt(i).Old_a_Securityquestion
                                                ELSE
                                                 Md.a_Securityquestion
                                           END,
                   Md.a_Languagepreference = CASE
                                                  WHEN (Dt(i)
                                                       .Old_a_Languagepreference IS NOT NULL AND Dt(i)
                                                       .New_a_Languagepreference IS NULL) THEN
                                                   Dt(i).Old_a_Languagepreference
                                                  ELSE
                                                   Md.a_Languagepreference
                                             END,
                   Md.a_Membersource = CASE
                                            WHEN (Dt(i).Old_a_Membersource IS NOT NULL AND Dt(i)
                                                 .New_a_Membersource IS NULL) THEN
                                             Dt(i).Old_a_Membersource
                                            ELSE
                                             Md.a_Membersource
                                       END,
                   Md.a_Gender = CASE
                                      WHEN (Dt(i).Old_a_Gender IS NOT NULL AND Dt(i)
                                           .New_a_Gender IS NULL) THEN
                                       Dt(i).Old_a_Gender
                                      ELSE
                                       Md.a_Gender
                                 END,
                   Md.a_Enrolldate = CASE
                                          WHEN (Dt(i).Old_a_Enrolldate IS NOT NULL AND Dt(i)
                                               .New_a_Enrolldate IS NULL) THEN
                                           Dt(i).Old_a_Enrolldate
                                          ELSE
                                           Md.a_Enrolldate
                                     END,
                   Md.a_Smsoptindate = CASE
                                            WHEN (Dt(i).Old_a_Smsoptindate IS NOT NULL AND Dt(i)
                                                 .New_a_Smsoptindate IS NULL) THEN
                                             Dt(i).Old_a_Smsoptindate
                                            ELSE
                                             Md.a_Smsoptindate
                                       END,
                   Md.a_Emailoptindate = CASE
                                              WHEN (Dt(i)
                                                   .Old_a_Emailoptindate IS NOT NULL AND Dt(i)
                                                   .New_a_Emailoptindate IS NULL) THEN
                                               Dt(i).Old_a_Emailoptindate
                                              ELSE
                                               Md.a_Emailoptindate
                                         END,
                   Md.a_Directmailoptindate = CASE
                                                   WHEN (Dt(i)
                                                        .Old_a_Directmailoptindate IS NOT NULL AND Dt(i)
                                                        .New_a_Directmailoptindate IS NULL) THEN
                                                    Dt(i)
                                                    .Old_a_Directmailoptindate
                                                   ELSE
                                                    Md.a_Directmailoptindate
                                              END,
                   Md.a_Memberstatuscode = CASE
                                                WHEN (Dt(i)
                                                     .Old_a_Memberstatuscode IS NOT NULL AND Dt(i)
                                                     .New_a_Memberstatuscode IS NULL) THEN
                                                 Dt(i).Old_a_Memberstatuscode
                                                ELSE
                                                 Md.a_Memberstatuscode
                                           END,
                   Md.a_Secondaryemailaddress = CASE
                                                     WHEN (Dt(i)
                                                          .Old_a_Secondaryemailaddress IS NOT NULL AND Dt(i)
                                                          .New_a_Secondaryemailaddress IS NULL) THEN
                                                      Dt(i)
                                                      .Old_a_Secondaryemailaddress
                                                     ELSE
                                                      Md.a_Secondaryemailaddress
                                                END,
                   Md.a_Workphone = CASE
                                         WHEN (Dt(i).Old_a_Workphone IS NOT NULL AND Dt(i)
                                              .New_a_Workphone IS NULL) THEN
                                          Dt(i).Old_a_Workphone
                                         ELSE
                                          Md.a_Workphone
                                    END,
                                    md.a_terminationreasonid = CASE
                                         WHEN (Dt(i).Old_a_terminationreasonid IS NOT NULL AND Dt(i)
                                              .New_a_terminationreasonid IS NULL) THEN
                                          Dt(i).Old_a_terminationreasonid
                                         ELSE
                                          Md.a_terminationreasonid
                                    END,

                   Md.Last_Dml_Id = bp_ae.last_dml_id#.nextval,
                   Md.Updatedate = systimestamp
            WHERE  Md.a_Ipcode = Dt(i).new_New_Ipcode;

          COMMIT;
          EXIT WHEN Driver%NOTFOUND;
     END LOOP;

     COMMIT;
     CLOSE Driver;

      V_ERROR          :=  ' ';
      V_REASON         := 'Merge_ats_MemberDetails end';
      V_MESSAGE        := 'Merge_ats_MemberDetails end';
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

procedure Copy_ats_MemberDetails_arc is

    CURSOR driver IS
   select OLD_ipcode
          , OLD_a_cardtype
          , OLD_a_changedby
          , OLD_a_passvalidation
          , OLD_a_basebrandid
          , OLD_a_oldcellnumber
          , OLD_a_emailaddressmailable
          , OLD_a_addresslineone
          , OLD_a_addresslinetwo
          , OLD_a_city
          , OLD_a_stateorprovince
          , OLD_a_ziporpostalcode
          , OLD_a_country
          , OLD_a_addressmailable
          , OLD_a_homephone
          , OLD_a_mobilephone
          , OLD_a_directmailoptindate
          , OLD_a_emailoptindate
          , OLD_a_smsoptindate
          , OLD_a_gender
          , OLD_a_membersource
          , OLD_a_languagepreference
          , OLD_a_oldemailaddress
          , OLD_a_homestoreid
          , OLD_a_hasemailbonuscredit
          , OLD_a_isunderage
          , OLD_a_employeecode
          , OLD_a_smsoptin
          , OLD_a_directmailoptin
          , OLD_a_emailoptin
          , OLD_a_hhkey
          , OLD_a_emailaddress
          , OLD_a_aitupdate
          , OLD_updatedate
          , OLD_a_extendedplaycode
          , OLD_a_programchangedate
          , OLD_a_autoissuereward
          , OLD_a_netspend
          , OLD_a_downloadmobileapp
          , OLD_a_pendingemailverification
          , OLD_a_pendingcellverification
          , OLD_a_nextemailreminderdate
          , OLD_a_cardclosedate
          , OLD_a_cardopendate
          from bp_ae.FINAL_MD;


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
     V_JOBNAME   := 'Copy_ats_MemberDetails_arc';
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
      V_REASON         := 'Copy_ats_MemberDetails_arc start';
      V_MESSAGE        := 'Copy_ats_MemberDetails_arc start';
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
          INTO dt LIMIT 100;


       FORALL I IN 1 .. dt.COUNT

           insert into ats_memberdetails_arc
       (
            ID
          , a_ipcode
          , a_cardtype
          , a_changedby
          , a_passvalidation
          , a_basebrandid
          , a_oldcellnumber
          , a_emailaddressmailable
          , a_addresslineone
          , a_addresslinetwo
          , a_city
          , a_stateorprovince
          , a_ziporpostalcode
          , a_country
          , a_addressmailable
          , a_homephone
          , a_mobilephone
          , a_directmailoptindate
          , a_emailoptindate
          , a_smsoptindate
          , a_gender
          , a_membersource
          , a_languagepreference
          , a_oldemailaddress
          , a_homestoreid
          , a_hasemailbonuscredit
          , a_isunderage
          , a_employeecode
          , a_smsoptin
          , a_directmailoptin
          , a_emailoptin
          , a_hhkey
          , a_emailaddress
          , a_aitupdate
          , updatedate
          , a_extendedplaycode
          , a_programchangedate
          , a_autoissuereward
          , a_netspend
          , a_downloadmobileapp
          , a_pendingemailverification
          , a_pendingcellverification
          , a_nextemailreminderdate
          , a_cardclosedate
          , a_cardopendate
       )
     values
          ( seq_rowkey.nextval
          , dt(i).OLD_ipcode
          , dt(i).OLD_a_cardtype
          , dt(i).OLD_a_changedby
          , dt(i).OLD_a_passvalidation
          , dt(i).OLD_a_basebrandid
          , dt(i).OLD_a_oldcellnumber
          , dt(i).OLD_a_emailaddressmailable
          , dt(i).OLD_a_addresslineone
          , dt(i).OLD_a_addresslinetwo
          , dt(i).OLD_a_city
          , dt(i).OLD_a_stateorprovince
          , dt(i).OLD_a_ziporpostalcode
          , dt(i).OLD_a_country
          , dt(i).OLD_a_addressmailable
          , dt(i).OLD_a_homephone
          , dt(i).OLD_a_mobilephone
          , dt(i).OLD_a_directmailoptindate
          , dt(i).OLD_a_emailoptindate
          , dt(i).OLD_a_smsoptindate
          , dt(i).OLD_a_gender
          , dt(i).OLD_a_membersource
          , dt(i).OLD_a_languagepreference
          , dt(i).OLD_a_oldemailaddress
          , dt(i).OLD_a_homestoreid
          , dt(i).OLD_a_hasemailbonuscredit
          , dt(i).OLD_a_isunderage
          , dt(i).OLD_a_employeecode
          , dt(i).OLD_a_smsoptin
          , dt(i).OLD_a_directmailoptin
          , dt(i).OLD_a_emailoptin
          , dt(i).OLD_a_hhkey
          , dt(i).OLD_a_emailaddress
          , dt(i).OLD_a_aitupdate
          , dt(i).OLD_updatedate
          , dt(i).OLD_a_extendedplaycode
          , dt(i).OLD_a_programchangedate
          , dt(i).OLD_a_autoissuereward
          , dt(i).OLD_a_netspend
          , dt(i).OLD_a_downloadmobileapp
          , dt(i).OLD_a_pendingemailverification
          , dt(i).OLD_a_pendingcellverification
          , dt(i).OLD_a_nextemailreminderdate
          , dt(i).OLD_a_cardclosedate
          , dt(i).OLD_a_cardopendate );


        COMMIT;

            EXIT WHEN driver%NOTFOUND;

      END LOOP;
      COMMIT;

      CLOSE driver;

      V_ERROR          :=  ' ';
      V_REASON         := 'Copy_ats_MemberDetails_arc end';
      V_MESSAGE        := 'Copy_ats_MemberDetails_arc end';
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

procedure recreate_indexes IS
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
   -- V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'HUB_SYNC_MEMBERDETAILS';

    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);


BEGIN

     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'HUB_SYNC_MEMBERDETAILS.recreate_Indexes';
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
      V_REASON         := 'HUB_SYNC_MEMBERDETAILS.recreate_Indexes start';
      V_MESSAGE        := 'HUB_SYNC_MEMBERDETAILS.recreate_Indexes start';
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




/*

BEGIN
   execute immediate 'alter trigger bp_ae.ATS_MEMBERDETAILS# enable';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'alter trigger bp_ae.ATS_MEMBERDETAILS# enable' || Err_Code || ' ' || Err_Msg;
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

    execute immediate 'alter trigger bp_ae.ATS_MEMBERDETAILS_ARC enable';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'alter trigger bp_ae.ATS_MEMBERDETAILS_ARC enable' || Err_Code || ' ' || Err_Msg;
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

*/


      V_ERROR          :=  ' ';
      V_REASON         := 'HUB_SYNC_MEMBERDETAILS.recreate_Indexes end';
      V_MESSAGE        := 'HUB_SYNC_MEMBERDETAILS.recreate_Indexes end';
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



end HUB_SYNC_MEMBERDETAILS;
/
