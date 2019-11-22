CREATE OR REPLACE PACKAGE PROFILEIN_PROMOTIONS IS       
  -- Author  : JUSCANGA
  -- Created : 5/24/2018 8:39:41 AM
  -- Purpose : Grant of Promotions  from a Profile-In file processed
  
  -- Public type declarations
  --type Recursor IS REF CURSOR;;
  
  -- Public constant declarations
  --Structure: <ConstantName> constant <Datatype> := <Value>;

  -- Public variable declarations
  --Structure: <VariableName> <Datatype>;

  -- Public function declarations
  -- Structure: function <FunctionName>(<Parameter> <Datatype>) return <Datatype>;

  -- Public procedure declarations
  -- Structure: procedure <ProcedureName>(<Parameter> <Datatype>);
  PROCEDURE Enrollment_Bonus_Summer2018;

end PROFILEIN_PROMOTIONS;
/
create or replace package body PROFILEIN_PROMOTIONS is
  
  PROCEDURE Enrollment_Bonus_Summer2018 IS
      v_EndPromoDate         TIMESTAMP := (TO_DATE('06/05/2018','MM/DD/YYYY') - numToDSInterval( 1,'second'));
      v_StartPromoDate       TIMESTAMP := TO_DATE('05/31/2018','MM/DD/YYYY');
      v_promoGrantedCounter  NUMBER:= -1;
      v_ipointtypeid         NUMBER(20);
      v_iPointeventID        NUMBER(20);
      v_PointEventExist      NUMBER:=0;
      v_PointTypeExist      NUMBER:=0;
      v_errors               NUMBER := 0;
      --log job attributes
      V_MY_LOG_ID        NUMBER;
      V_JOBDIRECTION     NUMBER := 0;
      V_STARTTIME        DATE := SYSDATE;
      V_ENDTIME          DATE;
      V_MESSAGESRECEIVED NUMBER := 0;
      V_MESSAGESFAILED   NUMBER := 0;
      V_JOBSTATUS        NUMBER := 0;
      V_JOBNAME          VARCHAR2(256);
      V_PROCESSID        NUMBER := 0;
      V_ERRORMESSAGE     VARCHAR2(256);
      --log msg attributes
      V_MESSAGEID   VARCHAR2(256);
      V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv','instance_name'));
      V_LOGSOURCE   VARCHAR2(256);
      V_BATCHID     VARCHAR2(256) := 0;
      V_MESSAGE     VARCHAR2(256) := ' ';
      V_REASON      VARCHAR2(256);
      V_ERROR       VARCHAR2(256);
      V_TRYCOUNT    NUMBER := 0;
      
      V_PROCESSDATE  DATE := sysdate;
      DML_ERRORS EXCEPTION;
      NO_POINTEVENT EXCEPTION;
      NO_POINTTYPE EXCEPTION;
      PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);
      p_email_list VARCHAR2(256) := 'mmorales@brierley.com; juscanga@brierley.com';
      
      --Validation AEO-2489 [BR2]: Between promotion dates
      /*Validation AEO-2489 [BR3]: Is not a excluded store*/
      /*Validation AEO-2489 [BR5]:If a member enrolls in the program and is merged to an existing member 
                                in the same Profile-In file, the member should not earn the bonus.*/
      CURSOR get_data IS     
          SELECT * FROM (     
            SELECT  vc.ipcode, vc.vckey, vc.isprimary, 
                    (
                       SELECT count(*)
                       FROM bp_ae.lw_virtualcard vch
                       WHERE vch.linkkey = p2.cid
                          AND vch.dateissued < trunc(p2.enrollmentdate)
                    ) AS previousExistentVC,
                    p2.*  
            FROM Bp_Ae.Ae_Profileupdates2 P2
                 JOIN bp_ae.lw_virtualcard vc ON(p2.loyaltynumber=vc.loyaltyidnumber)
            WHERE p2.enrollmentdate >= v_StartPromoDate
                  AND p2.enrollmentdate <= v_EndPromoDate
                  AND p2.storenumber IS NOT NULL
                  AND p2.storenumber NOT IN(
                       '146', '191', '2022', '2023', '2054', '2201', '2206', '2311', '2325', 
                       '2332', '2334', '2360', '2375', '2396', '251', '269', '302', '329', 
                       '339', '350', '408', '418', '420', '421', '481', '538', '580', '590', 
                       '648', '77', '803', '818', '828', '94', '113', '131', '150', '153', 
                       '187', '197', '2031', '2045', '2046', '2056', '209', '2157', '2212', 
                       '2230', '2254', '2369', '2405', '271', '280', '309', '417', '424', 
                       '425', '431', '498', '508', '558', '561', '565', '614', '624', '842', 
                       '894'
                  )
                 AND vc.isprimary = 1
          ) WHERE previousExistentVC=0;
      
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; 
      t_profileupdate t_tab;
      --r_profileupdate t_tab; 
      
  BEGIN   
     /*Logging*/
     v_my_log_id := utility_pkg.get_LIBJobID();
     v_jobname := 'Connected_Welcome_Bonus_Summer2018';
     v_logsource := v_jobname;

     /* log start of job */
     utility_pkg.Log_job(P_JOB    => v_my_log_id
            ,p_jobdirection       => v_jobdirection
            ,p_filename           => null
            ,p_starttime          => v_starttime
            ,p_endtime            => v_endtime
            ,p_messagesreceived   => v_messagesreceived
            ,p_messagesfailed     => v_messagesfailed
            ,p_jobstatus          => v_jobstatus
            ,p_jobname            => v_jobname);


     V_ERROR := 'AEO-2489 Enrollment Promotion';  
     utility_pkg.Log_Process_Start(v_jobname, 'AEO-2489 Enrollment Promotion', v_processId);
     
     /*PointEventId*/
     SELECT count(*)
       INTO v_PointEventExist
     FROM bp_ae.lw_pointevent pe
     WHERE name = 'SUMMER 2018: Join AEO Connected and get 500 points';
      
     IF (v_PointEventExist =  1) THEN
        SELECT pe.pointeventid INTO v_iPointeventID 
        FROM  bp_ae.lw_pointevent pe WHERE name ='SUMMER 2018: Join AEO Connected and get 500 points';
     ELSE
       RAISE NO_POINTEVENT;
     END IF;
     
     /*PointTypeId*/
     SELECT count(*)
       INTO v_PointTypeExist
     FROM  bp_ae.Lw_Pointtype
     WHERE  NAME = 'AEO Connected Bonus Points';
     
     IF(v_PointTypeExist = 1) THEN
        SELECT Pointtypeid INTO   v_ipointtypeid
        FROM bp_ae.Lw_Pointtype WHERE NAME = 'AEO Connected Bonus Points';
     ELSE
       RAISE NO_POINTTYPE;
     END IF;
     
     OPEN get_data;
     LOOP
       FETCH get_data BULK COLLECT INTO t_profileupdate LIMIT 100;
       FOR i IN 1 .. t_profileupdate.COUNT LOOP
       
       BEGIN
   -----------------------------------------------------          
           --IF v_iPointeventID IS NOT NULL THEN
           SELECT count(p.pointeventid) INTO v_promoGrantedCounter
           FROM Bp_Ae.Ae_Custompromo_Enrollment p
           WHERE p.pointeventid= v_iPointeventID
                 AND p.cid = t_profileupdate(i).cid;
           --END IF;
           /* Validation AEO-2489 [BR4]: Each member can only earn this promotion once.*/
           IF v_promoGrantedCounter = 0  THEN
              /*AEO-2489 [BR5, BR6, BR7, BR8, BR9]*/
                        
              /*Grant of the Points*/
              INSERT INTO Lw_Pointtransaction(
                     Pointtransactionid,
                     Vckey,
                     Pointtypeid,
                     Pointeventid,
                     Transactiontype,
                     Transactiondate,
                     Pointawarddate,
                     Points,
                     Expirationdate,
                     Notes,
                     Ownertype,
                     Ownerid,
                     Rowkey,
                     Parenttransactionid,
                     Pointsconsumed,
                     Pointsonhold,
                     Ptlocationid,
                     Ptchangedby,
                     Createdate,
                     Expirationreason)
              VALUES (
                     seq_pointtransactionid.nextval,
                     t_profileupdate(i).vckey,
                     v_ipointtypeid,
                     v_iPointeventID,
                     1 ,--purchase
                     SYSDATE,
                     SYSDATE,
                     500,
                     To_Date('12/31/2199', 'mm/dd/yyyy'),
                     'AEO Connected Welcome Bonus 5/31 - 6/4 500 Points',
                     0,
                     -1,
                     -1,
                     -1,
                     0 ,--Pointsconsumed
                     0,
                     NULL,
                     'Profile-in-Enrollment' ,--Ptchangedby
                     SYSDATE,
                     NULL);
                                    
              /*Associating the promotion granted*/
              INSERT INTO BP_AE.Ae_Custompromo_Enrollment(
                       pointeventid,
                       ipcode,
                       vckey,
                       CID,
                       storenumber,
                       enrollmentdate,
                       creationdate)
               VALUES (
                      v_iPointeventID,
                      t_profileupdate(i).ipcode,
                      t_profileupdate(i).vckey,
                      t_profileupdate(i).cid,
                      t_profileupdate(i).storenumber,
                      t_profileupdate(i).enrollmentdate,
                      SYSDATE);
                        
               COMMIT;
            END IF;                  
   -----------------------------------------------------------------       
       EXCEPTION
         WHEN OTHERS THEN
           v_errors := v_errors + 1;
           
           V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
           v_endtime := SYSDATE;
           V_MESSAGE := 'Grant of promotion fail:' 
                     ||' LoyaltyNumber=' || t_profileupdate(i).LoyaltyNumber
                     ||', CID=' || t_profileupdate(i).CID
                     ||', ENROLLEMENTDATE=' || TO_CHAR(t_profileupdate(i).enrollmentdate,'MM/DD/YYYY');
           utility_pkg.Log_job(P_JOB                => v_my_log_id
           ,p_jobdirection       => v_jobdirection
           ,p_filename           => null
           ,p_starttime          => v_starttime
           ,p_endtime            => v_endtime
           ,p_messagesreceived   => v_messagesreceived
           ,p_messagesfailed     => v_messagesfailed
           ,p_jobstatus          => v_jobstatus
           ,p_jobname            => v_jobname);
        V_ERROR := SQLERRM;
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'AEO-2489 Enrollment Promotion',
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);
       END;

       END LOOP;
       EXIT WHEN get_data%NOTFOUND;
     END LOOP;
     IF get_data%ISOPEN THEN
          CLOSE get_data;
     END IF;
     IF v_errors > 0 THEN
       RAISE DML_ERRORS;
     END IF;
     
     utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

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
    WHEN NO_POINTEVENT THEN
         v_errors := v_errors + 1;
           
           V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
           v_endtime := SYSDATE;
           V_MESSAGE := 'POINT EVENT "SUMMER 2018: Join AEO Connected and get 500 points" not found';
           utility_pkg.Log_job(P_JOB                => v_my_log_id
           ,p_jobdirection       => v_jobdirection
           ,p_filename           => null
           ,p_starttime          => v_starttime
           ,p_endtime            => v_endtime
           ,p_messagesreceived   => v_messagesreceived
           ,p_messagesfailed     => v_messagesfailed
           ,p_jobstatus          => v_jobstatus
           ,p_jobname            => v_jobname);
        V_ERROR := SQLERRM;
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'AEO-2489 Enrollment Promotion',
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);
      
       RAISE_APPLICATION_ERROR(-20002,'Has occured ' || v_errors || ' Exception detected in AEO-2489 Grant of Enrollment Promotion');
             
    WHEN NO_POINTTYPE THEN
      v_errors := v_errors + 1;
           
           V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
           v_endtime := SYSDATE;
           V_MESSAGE := 'POINT TYPE "AEO Connected Bonus Points" not found';
           utility_pkg.Log_job(P_JOB                => v_my_log_id
           ,p_jobdirection       => v_jobdirection
           ,p_filename           => null
           ,p_starttime          => v_starttime
           ,p_endtime            => v_endtime
           ,p_messagesreceived   => v_messagesreceived
           ,p_messagesfailed     => v_messagesfailed
           ,p_jobstatus          => v_jobstatus
           ,p_jobname            => v_jobname);
        V_ERROR := SQLERRM;
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'AEO-2489 Enrollment Promotion',
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);
        RAISE_APPLICATION_ERROR(-20002,'Has occured ' || v_errors || ' Exception detected in AEO-2489 Grant of Enrollment Promotion');
  WHEN DML_ERRORS THEN
    --this was loged record by record error
    RAISE_APPLICATION_ERROR(-20002,'Has occured ' || v_errors || ' Exception detected in AEO-2489 Grant of Enrollment Promotion');
  WHEN OTHERS THEN        
    v_errors := v_errors + 1;
           V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
           v_endtime := SYSDATE;
           V_MESSAGE := SQLERRM;
           utility_pkg.Log_job(P_JOB                => v_my_log_id
           ,p_jobdirection       => v_jobdirection
           ,p_filename           => null
           ,p_starttime          => v_starttime
           ,p_endtime            => v_endtime
           ,p_messagesreceived   => v_messagesreceived
           ,p_messagesfailed     => v_messagesfailed
           ,p_jobstatus          => v_jobstatus
           ,p_jobname            => v_jobname);
        V_ERROR := SQLERRM;
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'AEO-2489 Enrollment Promotion',
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);
    RAISE_APPLICATION_ERROR(-20002,'Has occured ' || v_errors || ' Exception detected in AEO-2489 Grant of Enrollment Promotion');
  END Enrollment_Bonus_Summer2018;
  

END PROFILEIN_PROMOTIONS;
/
