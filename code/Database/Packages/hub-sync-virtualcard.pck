create or replace package hub_sync_virtualcard is

PROCEDURE DROP_INDEXES;

PROCEDURE MERGE_LW_VIRTUALCARD (p_mod INTEGER,
                                  p_instances INTEGER);


PROCEDURE RECREATE_INDEXES;



-- NOTE the drop_indexes procedure of this package must be executed
-- before MERGE_LW_VIRTUALCARD can be executed properly.

-- Note:
-- MERGE_LW_VIRTUALCARD is designed to be run in  parallel instances.
-- For example - to run it in 5 parallel instances execute it as
--   MERGE_LW_VIRTUALCARD (p_mod => 0, p_instances => 5);
--   MERGE_LW_VIRTUALCARD (p_mod => 1, p_instances => 5);
--   MERGE_LW_VIRTUALCARD (p_mod => 2, p_instances => 5);
--   MERGE_LW_VIRTUALCARD (p_mod => 3, p_instances => 5);
--   MERGE_LW_VIRTUALCARD (p_mod => 4, p_instances => 5);

-- Any other combinations of parameters will not work properly.
-- To run it in only one instance, use:
--   MERGE_LW_VIRTUALCARD (p_mod => 0, p_instances 1);

-- NOTE after all instances have finished successfully RECREATE_INDEXES needs to be run.

-- TODO - need to modify the log messages to indicate what the parameters
-- of the running instance are in MERGE_LW_VIRTUALCARD.

-- TODO - probably want to improve logging and exception handling.


end hub_sync_virtualcard;
/
create or replace package body hub_sync_virtualcard is


PROCEDURE DROP_INDEXES IS
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
    P_FILENAME VARCHAR2(50) := 'hub_sync_virtualcard';
    V_MY_LOG_ID NUMBER;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';

BEGIN
     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'Merge_Lw_VirtualCard.Drop_indexes';
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
      V_REASON         := 'Merge_Lw_VirtualCard.Drop_indexes start' ;
      V_MESSAGE        := 'Merge_Lw_VirtualCard.Drop_indexes start' ;
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
    execute immediate 'alter trigger bp_ae.LW_VIRTUALCARD# disable';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'alter trigger bp_ae.LW_VIRTUALCARD# disable' || Err_Code || ' ' || Err_Msg;
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
      V_REASON         := 'Merge_Lw_VirtualCard.Drop_indexes ends' ;
      V_MESSAGE        := 'Merge_Lw_VirtualCard.Drop_indexes ends' ;
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


PROCEDURE MERGE_LW_VIRTUALCARD   (p_mod INTEGER,
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
    P_FILENAME VARCHAR2(50) := 'hub_sync_virtualcard';
    V_MY_LOG_ID NUMBER;

     CURSOR Driver IS
          SELECT /*+use_hash(m,c)*/
           New_Ipcode, c.Vckey
          FROM   Bp_Ae.Ae_Merge m, Bp_Ae.Lw_Virtualcard c
          WHERE  c.loyaltyidnumber = m.old_loyalty_id
            and mod(m.old_ipcode, p_instances) = p_mod;



    TYPE DRIVER_TABLE_TYPE IS TABLE OF DRIVER%rowtype;

    DT DRIVER_TABLE_TYPE;

BEGIN


     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'Merge_Lw_VirtualCard';
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
      V_REASON         := 'Merge_Lw_VirtualCard start' || to_char(p_mod);
      V_MESSAGE        := 'Merge_Lw_VirtualCard start' || to_char(p_mod);
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







     OPEN Driver;
     LOOP
          FETCH Driver BULK COLLECT
               INTO Dt LIMIT 100;
          FORALL i IN 1 .. Dt.Count

               UPDATE Bp_Ae.Lw_Virtualcard Vc
               SET    Vc.Ipcode = Dt(i).New_Ipcode,
                      vc.isprimary = 0,
                      vc.changedby  = 'Profile Update',
                      vc.updatedate = SYSDATE
               WHERE  Vc.Vckey = Dt(i).Vckey;


          COMMIT;
          EXIT WHEN Driver%NOTFOUND;
     END LOOP;
     COMMIT;
     CLOSE Driver;


      V_ERROR          :=  ' ';
      V_REASON         := 'Merge_Lw_VirtualCard end'|| to_char(p_mod);
      V_MESSAGE        := 'Merge_Lw_VirtualCard end'|| to_char(p_mod);
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



PROCEDURE RECREATE_INDEXES IS
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
    P_FILENAME VARCHAR2(50) := 'hub_sync_virtualcard';
    V_MY_LOG_ID NUMBER;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
begin


     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'hub_sync_virtualcard.recreate_index';
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
      V_REASON         := 'hub_sync_virtualcard.recreate_index start' ;
      V_MESSAGE        := 'hub_sync_virtualcard.recreate_index start' ;
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
    execute immediate 'alter trigger bp_ae.LW_VIRTUALCARD# enable';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'alter trigger bp_ae.LW_VIRTUALCARD# enable' || Err_Code || ' ' || Err_Msg;
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
      V_REASON         := 'hub_sync_virtualcard.recreate_index end' ;
      V_MESSAGE        := 'hub_sync_virtualcard.recreate_index end' ;
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


end hub_sync_virtualcard;
/
