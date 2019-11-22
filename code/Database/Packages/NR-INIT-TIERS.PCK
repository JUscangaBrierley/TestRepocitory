create or replace package NR_INIT_TIERS is

  -- Author  : MMORALES
  -- Created : 4/27/2017 11:20:47 AM
  -- Purpose : One time stored procedures for new tiers ('Full access' and ' Extra Access')
  

  FUNCTION GetExpirationDate( p_memberdate  DATE ,  p_launchdate DATE ) return DATE;
  PROCEDURE ProcessLegacyMembers   (p_processdate IN VARCHAR2 ) ;
  PROCEDURE UndoProcessLegacyMembers;
  PROCEDURE ProcessPilotSilverMembers(p_processdate IN VARCHAR2 );
  PROCEDURE UndoProcessPilotSilverMembers;
  PROCEDURE ProcessPilotBlueMembers(p_processdate IN VARCHAR2 ) ;
  PROCEDURE UndoProcessPilotBlueMembers;  


end NR_INIT_TIERS;
/
create or replace package body NR_INIT_TIERS is

  /* 
       Return the expiration date according to FR#4.
      
  */
  FUNCTION Getexpirationdate(p_memberdate DATE,
                             p_launchdate DATE) RETURN DATE IS
                             
       Lv_Initialyear NUMBER := Extract(YEAR FROM Nvl(p_memberdate, SYSDATE));
       Lv_Endingyear  NUMBER := Extract(YEAR FROM Nvl(p_launchdate, SYSDATE));
       Lv_resultValue DATE := SYSDATE;
       lv_elapsedyears NUMBER := 0;
       
  BEGIN
         lv_elapsedyears  := (lv_endingyear - Lv_initialyear);
       
       CASE 
           WHEN (lv_elapsedyears < 3 AND lv_elapsedyears >=0) THEN
              Lv_resultValue := to_date('12/31/'|| (Lv_Endingyear+1),'mm/dd/yyyy')+1;
           WHEN (lv_elapsedyears < 0 ) THEN
              Lv_resultValue := to_date('12/31/2199','mm/dd/yyyy');
           ELSE
              Lv_resultValue :=   to_date('12/31/'|| Lv_Endingyear,'mm/dd/yyyy')+1;
       END CASE;
       
       RETURN(Lv_resultValue);
  END Getexpirationdate;
  
 
  
PROCEDURE ProcessLegacyMembers   (p_processdate IN VARCHAR2 ) IS
  
   lv_launchdate DATE := to_date(p_processdate,'MM/DD/YYYY');      
   lv_fullaccesstierid   NUMBER := -1;
   lv_fullaccesstierdesc nvarchar2(50):= '';
   lv_exist NUMBER := -1;
   v_membertierid NUMBER := -1;
   lv_oldexpirationdate DATE := lv_launchdate;
     
   v_Logsource        VARCHAR2(256) := 'NR_INIT_TIERS.ProcessLegacyMembers';
   v_My_Log_Id        NUMBER;
   v_Jobdirection     NUMBER := 0;
   v_Filename         VARCHAR2(512) := 'NR_INIT_TIERS.ProcessLegacyMembers';
   v_Starttime        DATE := SYSDATE;
   v_Endtime          DATE;
   v_Messagesreceived NUMBER := 0;
   v_Messagesfailed   NUMBER := 0;
   v_Jobstatus        NUMBER := 0;
   v_Jobname          VARCHAR2(256) := 'ProcessLegacyMembers';
   v_Batchid          VARCHAR2(256) := 0;
   v_Message          VARCHAR2(256);
   v_Reason           VARCHAR2(256);
   v_Error            VARCHAR2(256);
   v_Messageid        NUMBER;       
   v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));
   
                              
  CURSOR cur_ipcode iS 
                SELECT Md.a_Ipcode,
                       Md.a_Netspend,
                       Nvl(lm.membercreatedate, To_Date('1/1/1900', 'mm/dd/yyyy')) AS Enrolldate
                FROM   Bp_Ae.Ats_Memberdetails Md
                INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm    ON     Lm.Ipcode = Md.a_Ipcode
                WHERE  Md.a_Extendedplaycode IN (0, 2)
                       OR Md.a_Extendedplaycode IS NULL;
                       
   CURSOR cur_membertier iS 
                SELECT Mt.Id,
                       Mt.Todate,
                       Nvl(lm.membercreatedate, To_Date('1/1/1900', 'mm/dd/yyyy')) AS Enrolldate
                FROM   Lw_Membertiers Mt
                INNER  JOIN Bp_Ae.Ats_Memberdetails Md   ON     Md.a_Ipcode = Mt.Memberid
                INNER  JOIN bp_ae.lw_loyaltymember lm ON lm.ipcode = md.a_ipcode
                WHERE  Mt.Todate > Lv_Launchdate AND
                        nvl(Md.a_Extendedplaycode,0) IN (0, 2)
                        OR Md.a_Extendedplaycode IS NULL;

 
 
   TYPE t_Tab IS TABLE OF cur_ipcode%ROWTYPE; 
   v_Tbl t_Tab; 
   
   TYPE t_Tab2 IS TABLE OF cur_membertier%ROWTYPE; 
   v_Tbl2 t_Tab2; 
   
   

 BEGIN
   
 
     v_My_Log_Id := Utility_Pkg.Get_Libjobid();
     Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                         p_Jobdirection     => v_Jobdirection,
                         p_Filename         => v_Filename,
                         p_Starttime        => v_Starttime,
                         p_Endtime          => v_Endtime,
                         p_Messagesreceived => v_Messagesreceived,
                         p_Messagesfailed   => v_Messagesfailed,
                         p_Jobstatus        => v_Jobstatus,
                         p_Jobname          => v_Jobname);
     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Full Access tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid ,t.tiername
     INTO lv_fullaccesstierid, lv_fullaccesstierdesc
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     
     OPEN cur_membertier;
     LOOP
          FETCH cur_membertier BULK COLLECT
               INTO v_Tbl2 LIMIT 100000;
               
          -- first we save in the nuke table all the info to update the expired member tiers
          FORALL i IN 1 .. v_Tbl2.Count 
                 INSERT INTO Bp_Ae.X$_Temp_Ae_Legacytiers
                      (Ipcode, Oldexpiration, Undo_Op)
                 VALUES
                      (v_Tbl2(i).Id, v_Tbl2(i).Todate, 'UPDATE');
          COMMIT;
                   
       
                                   
          FORALL i IN 1 .. v_Tbl2.Count 
               UPDATE lw_membertiers mt
                SET mt.todate =  lv_oldexpirationdate,
                mt.updatedate = SYSDATE                                     
               WHERE  v_Tbl2(i).id = mt.id ;
               
           v_Message := 'expiring '||v_Tbl2.Count||'  tiers ';
           Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);
             
       
                           
          COMMIT; 
          EXIT WHEN cur_membertier%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_membertier%ISOPEN      THEN
          CLOSE cur_membertier;
     END IF;       
     
     
     
     
     OPEN cur_ipcode;
     LOOP
          FETCH cur_ipcode BULK COLLECT
               INTO v_Tbl LIMIT 100000;
               
          -- first we save in the nuke table all the info to DELETE the new added membertier
          FORALL i IN 1 .. v_Tbl.Count 
                 INSERT INTO Bp_Ae.X$_Temp_Ae_Legacytiers
                      (Ipcode, Oldexpiration, Undo_Op)
                 VALUES
                      (v_Tbl(i).a_Ipcode, NULL, 'DELETE');
         COMMIT;             
                 
             
           -- for all legacy we add a new tier 
          FOR i IN 1 .. v_Tbl.Count 
            LOOP
               v_membertierid := hibernate_sequence.nextval;
               
               INSERT INTO Lw_Membertiers
                    (Id,
                     Tierid,
                     Memberid,
                     Fromdate,
                     Todate,
                     Description,
                     Createdate,
                     Updatedate)
               VALUES
                    ( v_membertierid,
                      lv_fullaccesstierid,
                      v_Tbl(i).a_ipcode,
                      lv_launchdate,        
                      Getexpirationdate(v_Tbl(i).enrolldate, lv_launchdate), 
                      lv_fullaccesstierdesc, 
                      SYSDATE,
                      SYSDATE )  ;   
                      
                      INSERT INTO Ats_Membernetspend
                            (a_Rowkey,
                             Lwidentifier,
                             a_Ipcode,
                             a_Parentrowkey,
                             a_Membertierid,
                             a_Netspend,
                             Statuscode,
                             Createdate,
                             Updatedate)
                       VALUES
                            (Seq_Rowkey.Nextval,
                             0,
                             v_Tbl(i).a_Ipcode,
                             -1,
                             v_membertierid,
                             v_Tbl(i).a_Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);                                     
              
               END LOOP;
            
               v_Message := 'adding '||v_Tbl.Count||'  tiers ';
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                       p_Envkey    => v_Envkey,
                                       p_Logsource => v_Logsource,
                                       p_Filename  => v_Filename,
                                       p_Batchid   => v_Batchid,
                                       p_Jobnumber => v_My_Log_Id,
                                       p_Message   => v_Message,
                                       p_Reason    => v_Reason,
                                       p_Error     => v_Error,
                                       p_Trycount  => 0,
                                       p_Msgtime   => SYSDATE);
                           
          COMMIT; 
          EXIT WHEN cur_ipcode%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_ipcode%ISOPEN      THEN
          CLOSE cur_ipcode;
     END IF;  
    
     v_endtime := SYSDATE;
     v_jobstatus := 1;
     utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

     EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               IF v_Messagesfailed = 0  THEN
                    v_Messagesfailed := 1;
               END IF;
               v_Jobstatus := 3;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure ProcessLegacyMembers';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>NR_INIT_TIERS</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>ProcessLegacyMembers</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '</failed>';
               /* log error */
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);     
 END; 

PROCEDURE UndoProcessLegacyMembers     IS
  
    lv_exist NUMBER := -1;
    lv_fullaccesstierid NUMBER := -1;
    
          v_Logsource        VARCHAR2(256) := 'NR_INIT_TIERS.UndoProcessLegacyMembers';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'NR_INIT_TIERS.UndoProcessLegacyMembers';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'UndoProcessLegacyMembers';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;       
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));
                              
  CURSOR cur_update iS 
                SELECT TR.IPCODE, TR.OLDEXPIRATION FROM Bp_Ae.X$_Temp_Ae_Legacytiers tr
                WHERE tr.undo_op = 'UPDATE' ;
                
                       
   CURSOR cur_delete iS 
                SELECT Tr.Ipcode, Mt.Id, mt.tierid
                FROM   Bp_Ae.X$_Temp_Ae_Legacytiers Tr
                INNER  JOIN Bp_Ae.Lw_Membertiers Mt  ON   Mt.Memberid = Tr.Ipcode
                WHERE  Tr.Undo_Op = 'DELETE'
                       AND Mt.Tierid = Lv_Fullaccesstierid;
 
 
   TYPE t_Tab IS TABLE OF cur_update%ROWTYPE; 
   v_Tbl t_Tab; 
   
   TYPE t_Tab2 IS TABLE OF cur_delete%ROWTYPE; 
   v_Tbl2 t_Tab2; 
   

 BEGIN
   
     v_My_Log_Id := Utility_Pkg.Get_Libjobid();
     Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                         p_Jobdirection     => v_Jobdirection,
                         p_Filename         => v_Filename,
                         p_Starttime        => v_Starttime,
                         p_Endtime          => v_Endtime,
                         p_Messagesreceived => v_Messagesreceived,
                         p_Messagesfailed   => v_Messagesfailed,
                         p_Jobstatus        => v_Jobstatus,
                         p_Jobname          => v_Jobname);

   
     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Full Access tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid 
     INTO lv_fullaccesstierid
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     
         
     OPEN cur_update;
     LOOP
          FETCH cur_update BULK COLLECT
               INTO v_Tbl LIMIT 100000;
         
           v_Message := 'Unexpiring '||v_Tbl.Count||' tiers ';
           Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);  
                      
          FORALL i IN 1 .. v_Tbl.Count    
               UPDATE bp_ae.lw_membertiers mt
                SET mt.todate = v_Tbl(i).oldexpiration,
                    mt.updatedate = SYSDATE
                WHERE mt.id = v_tbl(i).ipcode; -- for UPDATE ipcode store membertierid
                           
          COMMIT; 
           /* log error */
          
          EXIT WHEN cur_update%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_update%ISOPEN      THEN
          CLOSE cur_update;
     END IF;   
 
     
     OPEN cur_delete;
     LOOP
          FETCH cur_delete BULK COLLECT
               INTO v_Tbl2 LIMIT 100000;

           v_Message := 'Deleting '||v_Tbl.Count||' new tiers  and ats_membernetspent tiers ';
           Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);  
                      
         
                   
          -- delete el membernetspend linked a member tier that willbe deleted
          FORALL i IN 1 .. v_Tbl2.Count 
               DELETE Bp_Ae.Ats_Membernetspend Mt
               WHERE  Mt.a_Membertierid = v_Tbl2(i).Id; 
                      
          COMMIT;
                      
          FORALL i IN 1 .. v_Tbl2.Count    
               DELETE Bp_Ae.lw_membertiers Mt
               WHERE  Mt.Id = v_Tbl2(i).id  ; -- for DELETE id store membetierid
                           
          COMMIT; 
          EXIT WHEN cur_delete%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_delete%ISOPEN      THEN
          CLOSE cur_delete;
     END IF;  
     
     v_endtime := SYSDATE;
     v_jobstatus := 1;
     utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

     EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               IF v_Messagesfailed = 0  THEN
                    v_Messagesfailed := 1;
               END IF;
               v_Jobstatus := 3;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure UndoProcessLegacyMembers';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>NR_INIT_TIERS</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>UndoProcessLegacyMembers</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '</failed>';
               /* log error */
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);     
 
 END;
  
PROCEDURE ProcessPilotSilverMembers(p_processdate IN VARCHAR2 ) IS
  
   lv_launchdate DATE := to_date(p_processdate,'MM/DD/YYYY');       
   lv_extraaccesstierid   NUMBER := -1;
   lv_extraaccesstierdesc nvarchar2(50):= ' ';
   lv_silvertierid   NUMBER := -1;
   lv_exist NUMBER := -1;
   lv_oldexpirationdate DATE := lv_launchdate;
   v_membertierid NUMBER := -1;
  v_Logsource        VARCHAR2(256) := 'NR_INIT_TIERS.ProcessPilotSilverMembers';
  v_My_Log_Id        NUMBER;
  v_Jobdirection     NUMBER := 0;
  v_Filename         VARCHAR2(512) := 'NR_INIT_TIERS.ProcessPilotSilverMembers';
  v_Starttime        DATE := SYSDATE;
  v_Endtime          DATE;
  v_Messagesreceived NUMBER := 0;
  v_Messagesfailed   NUMBER := 0;
  v_Jobstatus        NUMBER := 0;
  v_Jobname          VARCHAR2(256) := 'ProcessPilotSilverMembers';
  v_Batchid          VARCHAR2(256) := 0;
  v_Message          VARCHAR2(256);
  v_Reason           VARCHAR2(256);
  v_Error            VARCHAR2(256);
  v_Messageid        NUMBER;       
  v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));   
 
   
   /* cursor used to insert new tiers */
   CURSOR cur_ipcode iS 
                SELECT Md.a_Ipcode,
                       Md.a_Netspend,
                       Nvl(Lm.Membercreatedate,
                           To_Date('1/1/1900', 'mm/dd/yyyy')) AS Enrolldate
                FROM   Bp_Ae.Ats_Memberdetails Md
                INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm    ON     Lm.Ipcode = Md.a_Ipcode
                WHERE  Md.a_Extendedplaycode IN (1, 3);
                
     /* cursor used to insert new tiers */        
   CURSOR cur_membertier iS 
                SELECT Mt.Id,
                       Mt.Todate,
                       Mt.Memberid                     
                FROM   Lw_Membertiers Mt
                INNER  JOIN Bp_Ae.Ats_Memberdetails Md      ON     Md.a_Ipcode = Mt.Memberid
                INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm       ON     Lm.Ipcode = Md.a_Ipcode
                WHERE  Mt.Todate > Lv_Launchdate
                       AND Mt.Tierid = Lv_Silvertierid
                       AND Md.a_Extendedplaycode IN (1, 3);
                       
 
 
   TYPE t_Tab2 IS TABLE OF cur_ipcode%ROWTYPE; 
   v_Tbl2 t_Tab2; 
   
   
   TYPE t_Tab IS TABLE OF cur_membertier%ROWTYPE; 
   v_Tbl t_Tab; 
  
   

 BEGIN
   
   v_My_Log_Id := Utility_Pkg.Get_Libjobid();
     Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                         p_Jobdirection     => v_Jobdirection,
                         p_Filename         => v_Filename,
                         p_Starttime        => v_Starttime,
                         p_Endtime          => v_Endtime,
                         p_Messagesreceived => v_Messagesreceived,
                         p_Messagesfailed   => v_Messagesfailed,
                         p_Jobstatus        => v_Jobstatus,
                         p_Jobname          => v_Jobname);

     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Silver';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Silver tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid 
     INTO lv_silvertierid
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Silver';  
 
 
     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Extra Access';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Extra Access tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid ,t.tiername
     INTO lv_extraaccesstierid, lv_extraaccesstierdesc
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Extra Access';
     
     
     ---------
     
     OPEN cur_membertier;
     LOOP
          FETCH cur_membertier BULK COLLECT
               INTO v_Tbl LIMIT 100000;
               
         
          FORALL i IN 1 .. v_Tbl.Count 
                 INSERT INTO Bp_Ae.X$_Temp_Ae_Pilotsilver
                      (ID, oldexpiration, undo_op)
                 VALUES
                      (v_Tbl(i).Id, v_Tbl(i).Todate, 'UPDATE');
          COMMIT;
          
         
                   
          -- fisrt we expire all membertiers that are not expired
          FORALL i IN 1 .. v_Tbl.Count 
               UPDATE lw_membertiers mt
                SET mt.todate =  lv_oldexpirationdate,
                mt.updatedate = SYSDATE                                     
               WHERE  v_Tbl(i).id = mt.id ;  
          COMMIT;   
          
          v_Message:= 'Expiring '||v_tbl.count ||' tiers' ;    
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);                
         
          EXIT WHEN cur_membertier%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_membertier%ISOPEN      THEN
          CLOSE cur_membertier;
     END IF;   
     
     
     
     OPEN cur_ipcode;
     LOOP
       FETCH cur_ipcode BULK COLLECT
               INTO v_Tbl2 LIMIT 100000;
               
                FORALL i IN 1 .. v_Tbl2.Count 
                  INSERT INTO Bp_Ae.X$_Temp_Ae_Pilotsilver
                        (ID, oldexpiration, undo_op)
                   VALUES
                        (v_Tbl2(i).a_ipcode, NULL, 'DELETE');
                COMMIT; 
                        
                FOR i IN 1 .. v_Tbl2.Count 
                  LOOP
                    
                     v_membertierid := hibernate_sequence.nextval;
                     
                    
                     INSERT INTO Lw_Membertiers
                          (Id,
                           Tierid,
                           Memberid,
                           Fromdate,
                           Todate,
                           Description,
                           Createdate,
                           Updatedate)
                     VALUES
                          ( v_membertierid ,
                            lv_extraaccesstierid,
                            v_Tbl2(i).a_ipcode,
                            lv_launchdate,         
                            getexpirationdate(v_tbl2(i).enrolldate, lv_launchdate),   
                            lv_extraaccesstierdesc, 
                            SYSDATE,
                            SYSDATE )  ; 
                            
                    INSERT INTO Ats_Membernetspend
                         (a_Rowkey,
                          Lwidentifier,
                          a_Ipcode,
                          a_Parentrowkey,
                          a_Membertierid,
                          a_Netspend,
                          Statuscode,
                          Createdate,
                          Updatedate)
                    VALUES
                         (Seq_Rowkey.Nextval,
                          0,
                          v_Tbl2(i).a_ipcode,
                          -1,
                          v_Membertierid,
                          v_Tbl2(i).a_Netspend,
                          0,
                          SYSDATE,
                          SYSDATE);

                     END LOOP                
                     COMMIT;
                     v_Message := 'Adding '|| v_tbl2.Count || 'tiers';
                      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE); 
                     EXIT WHEN cur_ipcode%NOTFOUND;
         
           
     END LOOP;
     COMMIT;
     IF cur_ipcode%ISOPEN      THEN
          CLOSE cur_ipcode;
     END IF;
     
     v_endtime := SYSDATE;
     v_jobstatus := 1;
     utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
     
   EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               IF v_Messagesfailed = 0  THEN
                    v_Messagesfailed := 1;
               END IF;
               v_Jobstatus := 3;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure ProcessPilotSilverMembers';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>NR_INIT_TIERS</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>ProcessPilotSilverMembers</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '</failed>';
               /* log error */
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);       
              
 
 END;

PROCEDURE UndoProcessPilotSilverMembers IS
  lv_extraaccesstierid   NUMBER := -1;
  lv_exist NUMBER := -1;
  
  v_Logsource        VARCHAR2(256) := 'NR_INIT_TIERS.UndoProcessPilotSilverMembers';
  v_My_Log_Id        NUMBER;
  v_Jobdirection     NUMBER := 0;
  v_Filename         VARCHAR2(512) := 'NR_INIT_TIERS.UndoProcessPilotSilverMembers';
  v_Starttime        DATE := SYSDATE;
  v_Endtime          DATE;
  v_Messagesreceived NUMBER := 0;
  v_Messagesfailed   NUMBER := 0;
  v_Jobstatus        NUMBER := 0;
  v_Jobname          VARCHAR2(256) := 'UndoProcessPilotSilverMembers';
  v_Batchid          VARCHAR2(256) := 0;
  v_Message          VARCHAR2(256);
  v_Reason           VARCHAR2(256);
  v_Error            VARCHAR2(256);
  v_Messageid        NUMBER;       
  v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));  
                          
   CURSOR cur_pilotsilver_exp iS 
               SELECT Tmp.Id, tmp.oldexpiration
               FROM   Bp_Ae.X$_TEMP_AE_PILOTSILVER Tmp
               WHERE tmp.undo_op = 'UPDATE';
               
   CURSOR cur_pilotsilver_del iS 
         SELECT Tr.Id AS ipcode, Mt.Id, mt.tierid
                FROM   Bp_Ae.X$_TEMP_AE_PILOTSILVER Tr
                INNER  JOIN Bp_Ae.Lw_Membertiers Mt  ON   Mt.Memberid = Tr.id
                WHERE  Tr.Undo_Op = 'DELETE'
                       AND Mt.Tierid = lv_extraaccesstierid;
  
   
   TYPE t_Tab IS TABLE OF cur_pilotsilver_exp%ROWTYPE; 
   v_Tbl t_Tab; 
  
   TYPE t_Tab2 IS TABLE OF cur_pilotsilver_del%ROWTYPE; 
   v_Tbl2 t_Tab2; 
   

 BEGIN
      v_My_Log_Id := Utility_Pkg.Get_Libjobid();
     Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                         p_Jobdirection     => v_Jobdirection,
                         p_Filename         => v_Filename,
                         p_Starttime        => v_Starttime,
                         p_Endtime          => v_Endtime,
                         p_Messagesreceived => v_Messagesreceived,
                         p_Messagesfailed   => v_Messagesfailed,
                         p_Jobstatus        => v_Jobstatus,
                         p_Jobname          => v_Jobname);

     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Extra Access';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Extra Access tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid 
     INTO lv_extraaccesstierid
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Extra Access';
     
     OPEN cur_pilotsilver_exp;
     LOOP
          FETCH cur_pilotsilver_exp BULK COLLECT
               INTO v_Tbl LIMIT 100000;
               
          FORALL i IN 1 .. v_Tbl.Count 
               UPDATE lw_membertiers mt
                SET mt.todate =  v_Tbl(i).Oldexpiration,
                mt.updatedate = SYSDATE                                     
               WHERE  v_Tbl(i).id = mt.id ;
                       
          COMMIT; 
          EXIT WHEN cur_pilotsilver_exp%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_pilotsilver_exp%ISOPEN      THEN
          CLOSE cur_pilotsilver_exp;
     END IF;       
     
     
     OPEN cur_pilotsilver_del;
     LOOP
          FETCH cur_pilotsilver_del BULK COLLECT
               INTO v_Tbl2 LIMIT 100000;
               
          FORALL i IN 1 .. v_Tbl2.Count 
               DELETE bp_ae.ats_membernetspend mn                                                  
               WHERE   mn.a_membertierid = v_Tbl2(i).id ;     
          COMMIT;
                
          FORALL i IN 1 .. v_Tbl2.Count 
               DELETE lw_membertiers mt                                                  
               WHERE  v_Tbl2(i).id = mt.id  ;
                       
          COMMIT; 
          EXIT WHEN cur_pilotsilver_del%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_pilotsilver_del%ISOPEN      THEN
          CLOSE cur_pilotsilver_del;
     END IF;   
     
     v_endtime := SYSDATE;
     v_jobstatus := 1;
     utility_pkg.Log_job(P_JOB => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
     
   EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               IF v_Messagesfailed = 0  THEN
                    v_Messagesfailed := 1;
               END IF;
               v_Jobstatus := 3;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure UndoProcessPilotSilverMembers';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>NR_INIT_TIERS</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>UndoProcessPilotSilverMembers</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '</failed>';
               /* log error */
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);       
                   
 END; 
 
PROCEDURE ProcessPilotBlueMembers(p_processdate IN VARCHAR2 ) IS
   
   lv_launchdate     DATE := to_date(p_processdate,'mm/dd/yyyy');       
   lv_bluetierid   NUMBER := -1;
   lv_exist        NUMBER := -1;
  v_Logsource        VARCHAR2(256) := 'NR_INIT_TIERS.ProcessPilotBlueMembers';
  v_My_Log_Id        NUMBER;
  v_Jobdirection     NUMBER := 0;
  v_Filename         VARCHAR2(512) := 'NR_INIT_TIERS.ProcessPilotBlueMembers';
  v_Starttime        DATE := SYSDATE;
  v_Endtime          DATE;
  v_Messagesreceived NUMBER := 0;
  v_Messagesfailed   NUMBER := 0;
  v_Jobstatus        NUMBER := 0;
  v_Jobname          VARCHAR2(256) := 'ProcessPilotBlueMembers';
  v_Batchid          VARCHAR2(256) := 0;
  v_Message          VARCHAR2(256);
  v_Reason           VARCHAR2(256);
  v_Error            VARCHAR2(256);
  v_Messageid        NUMBER;       
  v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name')); 
  v_membertierid   NUMBER := 1;  
    
                              
  CURSOR cur_pilotblue iS 
        SELECT Mt.Id, Mt.Todate
        FROM   Lw_Membertiers Mt
        WHERE  Mt.Todate > Lv_Launchdate
               AND Mt.Tierid = Lv_Bluetierid
               AND Mt.Memberid IN
               (SELECT Md.a_Ipcode
                    FROM   Bp_Ae.Ats_Memberdetails Md
                    INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm
                    ON     Lm.Ipcode = Md.a_Ipcode
                    WHERE  Md.a_Extendedplaycode IN (1, 3));
                    
 /* cursor used to insert new tiers */
   CURSOR cur_ipcode iS 
                SELECT Md.a_Ipcode,
                       Md.a_Netspend,
                       Nvl(Lm.Membercreatedate,
                           To_Date('1/1/1900', 'mm/dd/yyyy')) AS Enrolldate
                FROM   Bp_Ae.Ats_Memberdetails Md
                INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm    ON     Lm.Ipcode = Md.a_Ipcode
                WHERE  Md.a_Extendedplaycode IN (1, 3);                    
                    
  
                                  
   
   
 
   TYPE t_Tab2 IS TABLE OF cur_ipcode%ROWTYPE; 
   v_Tbl2 t_Tab2; 
   
   TYPE t_Tab IS TABLE OF cur_pilotblue%ROWTYPE; 
   v_Tbl t_Tab; 
   
   lv_fullaccesstierid NUMBER := -1;
   lv_fullaccesstierdesc VARCHAR2(50) :=' ';
   

 BEGIN
   
    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                         p_Jobdirection     => v_Jobdirection,
                         p_Filename         => v_Filename,
                         p_Starttime        => v_Starttime,
                         p_Endtime          => v_Endtime,
                         p_Messagesreceived => v_Messagesreceived,
                         p_Messagesfailed   => v_Messagesfailed,
                         p_Jobstatus        => v_Jobstatus,
                         p_Jobname          => v_Jobname);
   
     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Blue';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Blue tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid 
     INTO lv_bluetierid
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Blue';
     
     
     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Full Access tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid ,t.tiername
     INTO lv_fullaccesstierid, lv_fullaccesstierdesc
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     
     
     OPEN cur_pilotblue;
     LOOP
          FETCH cur_pilotblue BULK COLLECT
               INTO v_Tbl LIMIT 100000;
               
          -- first we save in the nuke table all the info to update the expired member tiers
          FORALL i IN 1 .. v_Tbl.Count 
                 INSERT INTO Bp_Ae.X$_Temp_Ae_Pilotblue
                      ( ID, oldexpiration, op_undo)
                 VALUES
                      (v_Tbl(i).Id, v_Tbl(i).Todate,'UPDATE');
          COMMIT;
                   
          -- fisrt we expire all membertiers that are not expired
          FORALL i IN 1 .. v_Tbl.Count 
               UPDATE lw_membertiers mt
                SET mt.todate =  lv_launchdate,
                mt.updatedate = SYSDATE                                     
               WHERE  v_Tbl(i).id = mt.id ;
             
       
                           
          COMMIT; 
          EXIT WHEN cur_pilotblue%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_pilotblue%ISOPEN      THEN
          CLOSE cur_pilotblue;
     END IF; 
     
      
     OPEN cur_ipcode;
     LOOP
       FETCH cur_ipcode BULK COLLECT
               INTO v_Tbl2 LIMIT 100000;
               
                FORALL i IN 1 .. v_Tbl2.Count 
                  INSERT INTO Bp_Ae.X$_Temp_Ae_Pilotblue
                        (ID, oldexpiration, op_undo)
                   VALUES
                        (v_Tbl2(i).a_ipcode, NULL, 'DELETE');
                COMMIT; 
                        
                FOR i IN 1 .. v_Tbl2.Count 
                  LOOP
                    
                     v_membertierid := hibernate_sequence.nextval;
                     
                    
                     INSERT INTO Lw_Membertiers
                          (Id,
                           Tierid,
                           Memberid,
                           Fromdate,
                           Todate,
                           Description,
                           Createdate,
                           Updatedate)
                     VALUES
                          ( v_membertierid ,
                            lv_fullaccesstierid,
                            v_Tbl2(i).a_ipcode,
                            lv_launchdate,         
                            getexpirationdate(v_tbl2(i).enrolldate, lv_launchdate),   
                            lv_fullaccesstierdesc, 
                            SYSDATE,
                            SYSDATE )  ; 
                            
                    INSERT INTO Ats_Membernetspend
                         (a_Rowkey,
                          Lwidentifier,
                          a_Ipcode,
                          a_Parentrowkey,
                          a_Membertierid,
                          a_Netspend,
                          Statuscode,
                          Createdate,
                          Updatedate)
                    VALUES
                         (Seq_Rowkey.Nextval,
                          0,
                          v_Tbl2(i).a_ipcode,
                          -1,
                          v_Membertierid,
                          v_Tbl2(i).a_Netspend,
                          0,
                          SYSDATE,
                          SYSDATE);

                     END LOOP                
                     COMMIT;
                    
                      v_Message := 'Adding '|| v_tbl2.Count || 'tiers';
                      Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);     
                     EXIT WHEN CUR_IPCODE%NOTFOUND;
         
           
     END LOOP;
     COMMIT;
     IF cur_ipcode%ISOPEN      THEN
          CLOSE cur_ipcode;
     END IF;

     v_endtime := SYSDATE;
     v_jobstatus := 1;
     utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
          
           
    EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               IF v_Messagesfailed = 0  THEN
                    v_Messagesfailed := 1;
               END IF;
               v_Jobstatus := 3;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure ProcessPilotBlueMembers';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>NR_INIT_TIERS</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>ProcessPilotBlueMembers</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '</failed>';
               /* log error */
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);
 END;

PROCEDURE UndoProcessPilotBlueMembers IS
  lv_fullaccesstierid   NUMBER := -1;
  lv_exist NUMBER := -1;
  
  v_Logsource        VARCHAR2(256) := 'NR_INIT_TIERS.UndoProcessPilotBlueMembers';
  v_My_Log_Id        NUMBER;
  v_Jobdirection     NUMBER := 0;
  v_Filename         VARCHAR2(512) := 'NR_INIT_TIERS.UndoProcessPilotBlueMembers';
  v_Starttime        DATE := SYSDATE;
  v_Endtime          DATE;
  v_Messagesreceived NUMBER := 0;
  v_Messagesfailed   NUMBER := 0;
  v_Jobstatus        NUMBER := 0;
  v_Jobname          VARCHAR2(256) := 'UndoProcessPilotBlueMembers';
  v_Batchid          VARCHAR2(256) := 0;
  v_Message          VARCHAR2(256);
  v_Reason           VARCHAR2(256);
  v_Error            VARCHAR2(256);
  v_Messageid        NUMBER;       
  v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));  
                                                  
   CURSOR cur_pilotblue_exp iS 
               SELECT Tmp.Id, tmp.oldexpiration
               FROM   Bp_Ae.X$_Temp_Ae_Pilotblue Tmp 
               WHERE tmp.op_undo = 'UPDATE';
               
   CURSOR cur_pilotblue_del iS 
         SELECT Tr.Id AS Ipcode, Mt.Id, Mt.Tierid
         FROM   Bp_Ae.X$_Temp_Ae_Pilotblue Tr
         INNER  JOIN Bp_Ae.Lw_Membertiers Mt   ON     Mt.Memberid = Tr.Id
         WHERE  Tr.op_undo = 'DELETE' AND 
                Mt.Tierid = lv_fullaccesstierid;
  
    
   TYPE t_Tab IS TABLE OF cur_pilotblue_exp%ROWTYPE; 
   v_Tbl t_Tab; 
  
  TYPE t_Tab2 IS TABLE OF cur_pilotblue_del%ROWTYPE; 
   v_Tbl2 t_Tab2; 
   

 BEGIN
     v_My_Log_Id := Utility_Pkg.Get_Libjobid();
     Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                         p_Jobdirection     => v_Jobdirection,
                         p_Filename         => v_Filename,
                         p_Starttime        => v_Starttime,
                         p_Endtime          => v_Endtime,
                         p_Messagesreceived => v_Messagesreceived,
                         p_Messagesfailed   => v_Messagesfailed,
                         p_Jobstatus        => v_Jobstatus,
                         p_Jobname          => v_Jobname);

     SELECT COUNT(*)
     INTO lv_exist
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Full Access';
     
     IF (lv_exist = 0) THEN
        RAISE_APPLICATION_ERROR(-20010, 'No Full Access tier in LW_tiers');
     END IF;
     
     SELECT  t.tierid 
     INTO lv_fullaccesstierid
     FROM bp_Ae.Lw_Tiers t
     WHERE t.tiername = 'Extra Access';
     
     
     OPEN cur_pilotblue_exp;
     LOOP
          FETCH cur_pilotblue_exp BULK COLLECT
               INTO v_Tbl LIMIT 100000;
               
          FORALL i IN 1 .. v_Tbl.Count 
               UPDATE lw_membertiers mt
                SET mt.todate =  v_Tbl(i).Oldexpiration,
                mt.updatedate = SYSDATE                                     
               WHERE  v_Tbl(i).id = mt.id ;
                       
          COMMIT; 
          EXIT WHEN cur_pilotblue_exp%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_pilotblue_exp%ISOPEN      THEN
          CLOSE cur_pilotblue_exp;
     END IF;       
     
     OPEN cur_pilotblue_del;
     LOOP
          FETCH cur_pilotblue_del BULK COLLECT
               INTO v_Tbl2 LIMIT 100000;
               
          FORALL i IN 1 .. v_Tbl2.Count 
               DELETE bp_ae.ats_membernetspend mn                                                  
               WHERE   mn.a_membertierid = v_Tbl2(i).id ;     
          COMMIT;
                
          FORALL i IN 1 .. v_Tbl2.Count 
               DELETE lw_membertiers mt                                                  
               WHERE  v_Tbl2(i).id = mt.id  ;
                       
          COMMIT; 
          EXIT WHEN cur_pilotblue_del%NOTFOUND; 
     END LOOP;
     COMMIT;
     IF cur_pilotblue_del%ISOPEN      THEN
          CLOSE cur_pilotblue_del;
     END IF;   
     
     v_endtime := SYSDATE;
     v_jobstatus := 1;
     utility_pkg.Log_job(P_JOB => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
     
   EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               IF v_Messagesfailed = 0  THEN
                    v_Messagesfailed := 1;
               END IF;
               v_Jobstatus := 3;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure UndoProcessPilotBlueMembers';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>NR_INIT_TIERS</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>UndoProcessPilotBlueMembers</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '</failed>';
               /* log error */
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => v_Filename,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => 0,
                                   p_Msgtime   => SYSDATE);       
                  
      
 END;
   
END;
/
