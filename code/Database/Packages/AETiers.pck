create or replace package AETiers is

  -- Author  : MMORALES
  -- Created : 4/19/2017 5:48:43 PM
  -- Purpose : Bussiness Logic to handle tiers upgrade, downgrade

   TYPE Rcursor IS REF CURSOR;

   PROCEDURE DowngradeTier( p_processdate IN VARCHAR2);

   PROCEDURE ResetTierNetSpend;




  PROCEDURE TierNomination(p_filename VARCHAR2, p_processDate VARCHAR2,
                            p_totpartitions INTEGER, p_currpartition INTEGER);


  PROCEDURE clear_infile(p_tablename VARCHAR2);

  PROCEDURE initialize(p_filename VARCHAR2);

  PROCEDURE Process_TierNomination(p_processDate DATE,
                            p_totpartitions INTEGER, p_currpartition INTEGER);

  FUNCTION GetExpirationDate( p_memberdate  DATE ,  p_processdate DATE ) return DATE;



end AETiers;
/
create or replace package body AETiers is


--AEO-1884 Begin Validations to DATA
  PROCEDURE Validate_Data(p_processdate  DATE , p_totpartitions INTEGER, p_currpartition INTEGER) IS
    
   CURSOR GET_DATA IS
    SELECT *
    FROM bp_ae.AE_TIERNOMINATION_EXCEPTION tn
    WHERE  1=1
    AND tn.processdate = p_processdate
    AND MOD (tn.ipcode, p_totpartitions) = p_currpartition; --AEO-1884 Get the partition by the last_dml_id

    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;
    
   CURSOR get_dup IS 
    SELECT T.IPCODE,
           T.LOYALTYNUMBER,
           T.CID,
           T.TIER,
           T.REASONCODE,
           T.AECCCARDHOLDER,
           T.FLAG_EXCEPTION,
           T.NUMROW
      FROM (SELECT TN.LOYALTYNUMBER,
                   TN.CID,
                   TN.TIER,
                   TN.REASONCODE,
                   TN.AECCCARDHOLDER,
                   TN.FLAG_EXCEPTION,
                   VC.IPCODE,
                   ROW_NUMBER() OVER(PARTITION BY VC.IPCODE ORDER BY VC.DATEISSUED DESC) AS NUMROW
              FROM BP_AE.AE_TIERNOMINATION TN
             INNER JOIN BP_AE.LW_VIRTUALCARD VC   ON VC.LOYALTYIDNUMBER = TN.LOYALTYNUMBER  AND MOD(VC.IPCODE, p_totpartitions) = p_currpartition) T
     WHERE T.NUMROW > 1;
     
   /*   SELECT tn2.*, vc2.ipcode
        FROM BP_AE.AE_TIERNOMINATION TN2
       INNER JOIN BP_AE.LW_VIRTUALCARD VC2     ON VC2.LOYALTYIDNUMBER = TN2.LOYALTYNUMBER
       INNER JOIN (
                   
                   SELECT VC.IPCODE
                     FROM BP_AE.AE_TIERNOMINATION TN
                    INNER JOIN BP_AE.LW_VIRTUALCARD VC
                       ON VC.LOYALTYIDNUMBER = TN.LOYALTYNUMBER
                    WHERE 1 = 1
                      AND MOD(VC.IPCODE, p_totpartitions) = p_currpartition
                    GROUP BY VC.IPCODE
                   HAVING COUNT(*) > 1) T     ON T.IPCODE = VC2.IPCODE;*/
                   
      TYPE T_TAB_DUP IS TABLE OF get_dup%ROWTYPE;
      V_TBL_DUP T_TAB_DUP;
      
      CURSOR get_no_dup IS 
      SELECT tn2.*, vc2.ipcode
        FROM BP_AE.AE_TIERNOMINATION TN2
       INNER JOIN BP_AE.LW_VIRTUALCARD VC2     ON VC2.LOYALTYIDNUMBER = TN2.LOYALTYNUMBER;
      
      TYPE T_TAB_NODUP IS TABLE OF get_no_dup%ROWTYPE;
      V_TBL_NODUP T_TAB_NODUP;
      
       CURSOR get_no_exist IS 
      SELECT tn2.*, vc2.ipcode
        FROM BP_AE.AE_TIERNOMINATION TN2
       LEFT JOIN BP_AE.LW_VIRTUALCARD VC2     ON VC2.LOYALTYIDNUMBER = TN2.LOYALTYNUMBER
       WHERE vc2.loyaltyidnumber IS NULL;
      
      TYPE T_TAB_NOEXIST IS TABLE OF get_no_exist%ROWTYPE;
      V_TBL_noexist T_TAB_NOEXIST;
    

      lv_isvalid NUMBER := 0;
      lstmpval  VARCHAR2(100) := NULL;
      lv_errormsg VARCHAR2(255) := NULL;
  BEGIN
    EXECUTE IMMEDIATE 'Truncate Table AE_TIERNOMINATION_EXCEPTION';
    
    -- catch non existing records
     OPEN get_no_exist;
    LOOP
      FETCH get_no_exist BULK COLLECT INTO V_TBL_noexist LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FORALL I IN 1 .. V_TBL_noexist.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
         INSERT INTO BP_AE.AE_TIERNOMINATION_EXCEPTION 
           (IPCODE,
            LOYALTYNUMBER,
            EXCEPTIONTYPE,
            FIELDNAME,
            VALUERECEIVED,
            ERRORCODE,
            PROCESSDATE,
            CREATEDATE)
         VALUES
           ( V_TBL_noexist(i).ipcode, 
             V_TBL_noexist(i).LOYALTYNUMBER,
             'REJECTED RECORD',
             'LOYALTYNUMBER',
              V_TBL_noexist(i).LOYALTYNUMBER,
              'INVALID LOYALTY NUMBER',
              p_processdate,
              SYSDATE); 
              
      COMMIT WRITE BATCH NOWAIT ;

      EXIT WHEN get_no_exist%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF get_no_exist%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE get_no_exist;
    END IF;
    
    COMMIT;
    
  -- CATCH duplicated ipcode records
    OPEN get_dup;
    LOOP
      FETCH get_dup BULK COLLECT INTO V_TBL_DUP LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FORALL I IN 1 .. V_TBL_DUP.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
         INSERT INTO BP_AE.AE_TIERNOMINATION_EXCEPTION 
           (IPCODE,
            LOYALTYNUMBER,
            EXCEPTIONTYPE,
            FIELDNAME,
            VALUERECEIVED,
            ERRORCODE,
            PROCESSDATE,
            CREATEDATE)
         VALUES
           ( V_TBL_DUP(i).ipcode, 
             v_tbl_dup(i).LOYALTYNUMBER,
             'REJECTED RECORD',
             'LOYALTYNUMBER',
              v_tbl_dup(i).LOYALTYNUMBER,
              'Duplicated IpCode',
              p_processdate,
              SYSDATE); 
              
      COMMIT WRITE BATCH NOWAIT ;

      EXIT WHEN get_dup%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF get_dup%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE get_dup;
    END IF;
    
    COMMIT;
    
    
       
    -- validate each record
    
    OPEN get_no_dup;
    LOOP
      FETCH get_no_dup BULK COLLECT INTO V_TBL_NODUP LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FOR I IN 1 .. V_TBL_NODUP.COUNT 
        LOOP
                 -- validate loyaltynumber
            utility_pkg.validate_loyaltynumber( V_TBL_NODUP(I).loyaltynumber,
                                             lv_errormsg , lv_isvalid) ;
                                             

            IF (  lv_isvalid = 0 ) THEN
               
            
               lstmpval :=  V_TBL_NODUP(I).loyaltynumber;
               INSERT INTO BP_AE.AE_TIERNOMINATION_EXCEPTION 
                 (IPCODE,
                  LOYALTYNUMBER,
                  EXCEPTIONTYPE,
                  FIELDNAME,
                  VALUERECEIVED,
                  errorcode,
                  PROCESSDATE,
                  CREATEDATE)
               VALUES
                 ( V_TBL_NODUP(i).ipcode, 
                   v_tbl_NOdup(i).LOYALTYNUMBER,
                   'REJECTED RECORD',
                   'LOYALTYNUMBER',
                    v_tbl_NOdup(i).LOYALTYNUMBER,
                    lv_errormsg,
                    p_processdate,
                    SYSDATE); 
            END IF  ;         
            
            -- validate cid
            lstmpval := 'X' || V_TBL_NODUP(I).CID;
            IF (lstmpval = 'X' ) OR  (NOT (NOT REGEXP_LIKE( lstmpval, '^[0-9]{1,13}$')))THEN
               INSERT INTO BP_AE.AE_TIERNOMINATION_EXCEPTION 
                 (IPCODE,
                  LOYALTYNUMBER,
                  EXCEPTIONTYPE,
                  FIELDNAME,
                  VALUERECEIVED,
                  errorcode,
                  PROCESSDATE,
                  CREATEDATE)
               VALUES
                 ( V_TBL_NODUP(i).ipcode, 
                   v_tbl_NOdup(i).LOYALTYNUMBER,
                   'REJECTED FIELD',
                   'CID',
                    v_tbl_NOdup(i).CID,
                    'INVALID CID NUMBER',
                    p_processdate,
                    SYSDATE); 
            END IF;
            
            
            -- validate tier
            lstmpval := 'X' || V_TBL_NODUP(I).TIER;
            IF (lstmpval = 'X') OR  ( (lstmpval <> 'X1' AND lstmpval <> 'X2'))THEN
               INSERT INTO BP_AE.AE_TIERNOMINATION_EXCEPTION 
                 (IPCODE,
                  LOYALTYNUMBER,
                  EXCEPTIONTYPE,
                  FIELDNAME,
                  VALUERECEIVED,
                  ERRORCODE,
                  PROCESSDATE,
                  CREATEDATE)
               VALUES
                 ( V_TBL_NODUP(i).ipcode, 
                   v_tbl_NOdup(i).LOYALTYNUMBER,
                   'REJECTED RECORD',
                   'TIER',
                    v_tbl_NOdup(i).TIER,
                    'INVALID VALUE',
                    p_processdate,
                    SYSDATE); 
            END IF;        
            
            -- validate reason code
            
            SELECT COUNT(*)
              INTO LV_ISVALID
              FROM BP_AE.ATS_REFTIERREASON RT
             WHERE RT.A_REASONCODE = V_TBL_NODUP(I).REASONCODE;
             
            IF (lv_isvalid = 0 ) THEN
                          
               INSERT INTO BP_AE.AE_TIERNOMINATION_EXCEPTION 
                 (IPCODE,
                  LOYALTYNUMBER,
                  EXCEPTIONTYPE,
                  FIELDNAME,
                  VALUERECEIVED,
                  ERRORCODE,
                  PROCESSDATE,
                  CREATEDATE)
               VALUES
                 ( V_TBL_NODUP(i).ipcode, 
                   v_tbl_NOdup(i).LOYALTYNUMBER,
                   'REJECTED RECORD',
                   'REASONCODE',
                    v_tbl_NOdup(i).reasoncode,
                    'INVALID VALUE',
                    p_processdate,
                    SYSDATE); 
              
            END IF;  
            
              
        END LOOP;
      
     
      COMMIT WRITE BATCH NOWAIT ;

      EXIT WHEN get_NO_dup%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF get_NO_dup%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE get_no_dup;
    END IF;
    
 
    
    
    --Flag record to exclude  REJECTED rows
    OPEN GET_DATA;
    LOOP
      FETCH GET_DATA BULK COLLECT INTO V_TBL LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FORALL I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
         UPDATE bp_ae.ae_tiernomination tn
             SET tn.flag_exception = 1
         WHERE tn.loyaltynumber = v_tbl(i).loyaltynumber AND
               v_tbl(i).exceptiontype = 'REJECTED RECORD' AND
               v_tbl(i).processdate = p_processdate;

      COMMIT  WRITE BATCH NOWAIT ;

      EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE GET_DATA;
    END IF;
  END;
  
  
--AEO-1884 End

 FUNCTION Getexpirationdate( p_memberdate DATE,
                             p_processdate DATE     ) RETURN DATE IS

       Lv_Initialyear NUMBER := Extract(YEAR FROM Nvl(p_memberdate, SYSDATE));
       Lv_Endingyear  NUMBER := Extract(YEAR FROM Nvl(p_processdate, SYSDATE));
     --  Lv_nominationYear NUMBER := Extract(YEAR FROM Nvl(p_processdate, SYSDATE));
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


FUNCTION BuildJobSuccessEmailHTML (p_JobNumber  NUMBER, p_procDate DATE)return clob is
  v_html_message  clob;
  lv_recfile NUMBER :=0;



BEGIN

 SELECT COUNT(*)
 INTO lv_recfile
 FROM ext_tiernomination;




  v_html_message := '<html>'||chr(10)||
        '<body>'||chr(10)||
        'Job#: '||p_JobNumber||chr(10)||
        '<br><br>' || chr(10)||
        '------------------------------------------------'||
        '<table>'||
         '<tr><td>Number of records contained in the file:</td><td>'||lv_recfile||'</td></t4>'||
        '<tr><td>Processing Date/Time:</td><td>'||to_char(p_procDate, 'mm/dd/yyyy hh:mi:ss')||'</td></t4>'||
        '</table>'||
        '<br>' || chr(10);

  v_html_message := v_html_message||chr(10)||'</body>'||chr(10)||'</html>';

  RETURN v_html_message;
END BuildJobSuccessEmailHTML;



 -- National rollout AEO-1214 begin

   PROCEDURE DowngradeTier( p_processdate IN VARCHAR2) AS

      TIER_THRESHOLD CONSTANT NUMBER := 345;


      v_processDate DATE := To_Date(p_processDate,'MM/DD/YYYY');

     CURSOR cur_downgradetier IS
        SELECT Mt.Memberid,
               MAX(Nvl(Md.a_Netspend, 0)) AS a_Netspend,
               MAX(Md.a_Aitupdate) AS Aitflag,
               MAX( nvl(lm.membercreatedate, to_date('01/01/1900','mm/dd/yyyy'))) AS Enrolldate
        FROM   Lw_Membertiers Mt
        INNER   JOIN lw_loyaltymember lm ON lm.ipcode = mt.memberid
        INNER  JOIN Ats_Memberdetails Md ON Mt.Memberid = Md.a_Ipcode
        WHERE  1 = 1  AND
               Mt.Tierid = (SELECT t.Tierid
                            FROM   Bp_Ae.Lw_Tiers t
                            WHERE  t.Tiername = 'Extra Access') AND
               Mt.Todate > v_processDate
        GROUP  BY Mt.Memberid;


          v_Logsource        VARCHAR2(256) := 'AEJobs.DowngradeTier';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AEJobs.DowngradeTier';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'DowngradeTier';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_fullaccesstier       NUMBER := 0;
          V_extraaccesstier      NUMBER := 0;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));
          v_tierid NUMBER := 0;
          v_rows   NUMBER :=0;
          v_reasonCode NVARCHAR2(50) := 'Qualifier';

          v_membertierid NUMBER := 0;


         TYPE cur_type is TABLE OF cur_downgradetier%ROWTYPE INDEX BY PLS_INTEGER;
         rec1 cur_type;

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


          SELECT Tr.Tierid
          INTO   v_fullaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Full Access';

          SELECT Tr.Tierid
          INTO   V_extraaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Extra Access';

          IF (v_fullaccesstier = 0) THEN
             raise_application_error(-20001, 'No tier found with name = "Full Access"');
          END IF;

          IF (v_extraaccesstier = 0) THEN
               raise_application_error(-20001, 'No tier found with name = "Extra Access"');
          END IF;



         OPEN cur_downgradetier;
         LOOP
             FETCH cur_downgradetier BULK COLLECT INTO rec1 LIMIT 1000;


                 FOR i IN 1 .. rec1.COUNT
                 LOOP
                   /*Selected by memberid, to expire any duplicate-active  membertiers, if any*/
                   UPDATE Bp_Ae.Lw_Membertiers Mt
                   SET    Mt.Todate = Trunc(SYSDATE)
                   WHERE  Mt.Memberid = Rec1(i).Memberid
                          AND Mt.Todate > v_processDate;

                   ---
                   --- determine the expiration date based on the number of years
                   ---


                   v_membertierid := Hibernate_Sequence.Nextval;

                   IF (rec1(i).a_netspend >= TIER_THRESHOLD) THEN
                       v_tierid := v_extraaccesstier;
                       v_reasonCode :='Qualifier';
                       INSERT INTO Lw_Membertiers Mt
                            (Id,
                             Tierid,
                             Memberid,
                             Fromdate,
                             Todate,
                             Description,
                             Createdate,
                             Updatedate)
                       VALUES
                            (v_membertierid,
                             v_Tierid,
                             Rec1(i).Memberid,
                             v_Processdate,
                             Getexpirationdate( Rec1(i).enrolldate , v_processdate),
                             v_Reasoncode,
                             SYSDATE,
                             SYSDATE);

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
                             Rec1(i).Memberid,
                             -1,
                             v_membertierid,
                             Rec1(i).a_Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);

                   ELSE
                      v_tierid := v_fullaccesstier;
                      v_reasonCode :='Base';
                      INSERT INTO Lw_Membertiers Mt
                            (Id,
                             Tierid,
                             Memberid,
                             Fromdate,
                             Todate,
                             Description,
                             Createdate,
                             Updatedate)
                      VALUES
                            (v_membertierid,
                             v_Tierid,
                             Rec1(i).Memberid,
                             v_Processdate,
                              Getexpirationdate( Rec1(i).enrolldate , v_processdate),
                             v_Reasoncode,
                             SYSDATE,
                             SYSDATE);

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
                             Rec1(i).Memberid,
                             -1,
                             v_membertierid,
                             Rec1(i).a_Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);

                      IF (rec1(i).AITflag <> 1) THEN
                          UPDATE Ats_Memberdetails Md
                          SET    Md.a_Aitupdate = 1
                          WHERE  Md.a_Ipcode = Rec1(i).Memberid;
                      END IF;

                   END IF;

                   v_rows := v_rows+1;
                   IF (MOD(v_rows,100)= 0 )THEN
                         COMMIT;
                   END IF;

                END LOOP;
                COMMIT;

            -- Commit, release lock
            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN cur_downgradetier%NOTFOUND;
          END LOOP;
          CLOSE cur_downgradetier;
          COMMIT;

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
               IF v_Messagesfailed = 0
               THEN
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
               v_Reason         := 'Failed Procedure DowngradeTier';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>AEJobs</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DowngradeTier</proc>' ||
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
     END DowngradeTier;

     PROCEDURE ResetTierNetSpend AS

          CURSOR cur_resetnetspend IS
            SELECT md.a_ipcode FROM bp_ae.ats_memberdetails md
            WHERE md.a_netspend > 0 ;


          v_Logsource        VARCHAR2(256) := 'AEJobs.ResetTierNetSpend';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AEJobs.ResetTierNetSpend';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'ResetTierNetSpend';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                     'instance_name'));



         TYPE cur_type is TABLE OF cur_resetnetspend%ROWTYPE INDEX BY PLS_INTEGER;
         rec1 cur_type;

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


         OPEN cur_resetnetspend;
         LOOP
             FETCH cur_resetnetspend BULK COLLECT INTO rec1 LIMIT 10000;

                 FORALL i IN 1 .. rec1.COUNT
                   UPDATE ats_memberdetails md
                       SET md.a_prevnetspend = md.a_netspend,
                           md.a_netspend = 0,
                           md.updatedate = SYSDATE,
                           md.a_changedby = 'ResetTierNetSpend'
                   WHERE md.a_ipcode IN rec1(i).a_ipcode;

                COMMIT;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN cur_resetnetspend%NOTFOUND;
          END LOOP;

          CLOSE cur_resetnetspend;
          COMMIT;

  v_endtime := SYSDATE;
  v_jobstatus := 1;
  utility_pkg.Log_job(P_JOB    => v_my_log_id
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
               IF v_Messagesfailed = 0
               THEN
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
               v_Reason         := 'Failed Procedure ResetTierNetSpend';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>AETiers</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>ResetTierNetSpend</proc>' ||
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
    eND;


  PROCEDURE initialize( p_filename VARCHAR2) IS
   -- v_partition_name VARCHAR2(256);
    v_sql            VARCHAR2(4000);
    /*v_inst           VARCHAR2(64) := upper(sys_context('userenv',
                                                       'instance_name'));*/
  BEGIN
    /*              set the external table filename                                      */
    v_sql := 'ALTER TABLE ext_tiernomination' || CHR(10) ||
             'LOCATION (AE_IN' || CHR(58) || '''' || p_filename || ''')';
    EXECUTE IMMEDIATE v_sql;
  END initialize;

  PROCEDURE clear_infile(p_tablename IN VARCHAR2) IS

    --lv_String     VARCHAR2(32000);
    lv_fileHandle UTL_FILE.FILE_TYPE;
    lv_path       VARCHAR2(50) DEFAULT 'USER_EXPORT';
    lv_filename   VARCHAR2(512);

  BEGIN
    SELECT LOCATION, DIRECTORY_NAME
      INTO LV_FILENAME, LV_PATH
      FROM USER_EXTERNAL_LOCATIONS t
     WHERE UPPER(t.table_name) = UPPER(p_tablename);

    lv_fileHandle := UTL_FILE.FOPEN(lv_path, LV_FILENAME, 'W', 32000);

    UTL_FILE.FCLOSE(lv_fileHandle);

  END clear_infile;

     -- National

  PROCEDURE TierNomination(p_filename VARCHAR2, p_processDate VARCHAR2,
                            p_totpartitions INTEGER, p_currpartition INTEGER )  IS


   CURSOR GET_EXT IS
        SELECT Tn.Loyaltynumber,
               Tn.Cid,
               Tn.Tier,
               Tn.Reasoncode,
               Tn.Cardholdercode
        FROM   Bp_Ae.Ext_Tiernomination Tn;


  --  v_processid    NUMBER := 0;
    v_my_log_id    NUMBER :=0;
    --v_dap_log_id   NUMBER :=0;
    --log job attributes
    v_jobdirection     NUMBER := 0;
    --v_filename         VARCHAR2(512) := p_filename;
    v_starttime        DATE := SYSDATE;
    v_endtime          DATE;
    v_messagesreceived NUMBER := 0;
    v_messagesfailed   NUMBER := 0;
    v_jobstatus        NUMBER := 0;
    v_jobname          VARCHAR2(256);
    v_messageid VARCHAR2(256);
    v_logsource VARCHAR2(256);
    v_batchid   VARCHAR2(256) := 0;
    v_message   VARCHAR2(4000);
    v_reason    VARCHAR2(4000);
    v_error     VARCHAR2(4000);
    v_trycount  NUMBER := 0;
    v_processDate DATE := To_Date(p_processDate,'MM/DD/YYYY HH24:MI:SS');
    v_envkey    VARCHAR2(256) := '';
    lv_err VARCHAR2(4000);
    lv_n   NUMBER;
    v_attachments cio_mail.attachment_tbl_type;
      TYPE T_TABEXT IS TABLE OF GET_EXT%ROWTYPE;
     V_TBLEXT T_TABEXT;

   BEGIN
    v_my_log_id     := utility_pkg.get_LIBJobID();
   -- v_dap_log_id    := utility_pkg.get_LIBJobID();
    v_jobname       := 'TierNomination';
    v_logsource     := v_jobname;
   -- v_processid     := 1;
    v_envkey        := 'BP_AE@' || UPPER(sys_context('userenv','instance_name'));

    /* log start of job */
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => p_filename,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    /* call sub program that run's data insert */
    IF UPPER(TRIM(p_filename)) LIKE UPPER('AEO_Tier_Upgrade_%.TXT') THEN
     --  processing the file
     /* initialize, truncates set external table to read p_filename */
     IF (  p_currpartition = 0) THEN
       initialize( p_filename);
      /* reset log file, read later for errors */
      clear_infile('ext_tiernomination_log');

      EXECUTE IMMEDIATE 'Truncate table ae_tiernomination';

      OPEN GET_EXT;
      LOOP
        FETCH GET_EXT BULK COLLECT INTO V_TBLEXT LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL I IN 1 .. V_TBLEXT.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop


             INSERT INTO Ae_Tiernomination
                  (Loyaltynumber, Cid, Tier, Reasoncode, Aecccardholder,Flag_Exception)
             VALUES
                  (v_tblext(i).loyaltynumber, v_tblext(i).cid, v_tblext(i).tier, v_tblext(i).reasoncode, v_tblext(i).cardholdercode,0);


        COMMIT  WRITE BATCH NOWAIT ;

        EXIT WHEN GET_EXT%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_EXT%ISOPEN THEN
      --<--- dont forget to close cursor since we manually opened it.
      CLOSE GET_EXT;
      END IF;
       END IF ;

     Process_TierNomination(v_processDate, p_totpartitions, p_currpartition);

    ELSE

      raise_application_error(-20001, 'Unrecognized file name');
    END IF;

    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM ext_tiernomination_log ' ||
                        CHR(10) || 'WHERE rec LIKE ''ORA-%'''
      INTO lv_n, lv_err;

      IF lv_n > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        v_messagesfailed := v_messagesfailed + lv_n;
        v_reason         := 'Failed reads by external table';
        v_message        := '<TIERNOMINATION>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>ext_tiernomination' ||
                            '</External>' || '    <FileName>' || p_filename ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</TIERNOMINATION>';
        utility_pkg.Log_msg(p_messageid => v_messageid,
                            p_envkey    => v_envkey,
                            p_logsource => v_logsource,
                            p_filename  => p_filename,
                            p_batchid   => v_batchid,
                            p_jobnumber => v_my_log_id,
                            p_message   => v_message,
                            p_reason    => v_reason,
                            p_error     => lv_err,
                            p_trycount  => lv_n,
                            p_msgtime   => SYSDATE);
      END IF;
    END;

    v_endtime   := SYSDATE;
    v_jobstatus := 1;

    /* log end of job */
    utility_pkg.Log_job(P_JOB              => v_my_log_id,
                        p_jobdirection     => v_jobdirection,
                        p_filename         => p_filename,
                        p_starttime        => v_starttime,
                        p_endtime          => v_endtime,
                        p_messagesreceived => v_messagesreceived,
                        p_messagesfailed   => v_messagesfailed,
                        p_jobstatus        => v_jobstatus,
                        p_jobname          => v_jobname);

    cio_mail.send(
          p_from_email    => 'AEREWARDS',
          p_from_replyto  => 'info@aerewards.ae.com',
          p_to_list       => 'aerewards_support@brierley.com',
          p_cc_list       => NULL,
          p_bcc_list      => NULL,
          p_subject       => 'AE Tier Nomination Succes',
          p_text_message  =>  BuildJobSuccessEmailHTML( v_my_log_id, v_processDate),
          p_content_type  => 'text/html;charset=UTF8',
          p_attachments   => v_attachments, --v_attachments,
          p_priority      => '3',
          p_auth_username => NULL,
          p_auth_password => NULL,
          p_mail_server   => 'cypwebmail.brierleyweb.com');

  EXCEPTION
    WHEN OTHERS THEN
      IF v_messagesfailed = 0 THEN
        v_messagesfailed := 0 + 1;
      END IF;
      utility_pkg.Log_job(P_JOB              => v_my_log_id,
                          p_jobdirection     => v_jobdirection,
                          p_filename         => p_filename,
                          p_starttime        => v_starttime,
                          p_endtime          => v_endtime,
                          p_messagesreceived => v_messagesreceived,
                          p_messagesfailed   => v_messagesfailed,
                          p_jobstatus        => v_jobstatus,
                          p_jobname          => v_jobname);
     -- v_messagesfailed := v_messagesfailed + 1;
      v_error          := SQLERRM;
      v_reason         := 'Failed Procedure TierNomination';
      v_message        := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                          '    <pkg>AE_Tiers</pkg>' || CHR(10) ||
                          '    <proc>TierNomination</proc>' || CHR(10) ||
                          '    <filename>' || p_filename || '</filename>' ||
                          CHR(10) || '  </details>' || CHR(10) ||
                          '</failed>';
      /* log error */
      utility_pkg.Log_msg(p_messageid => v_messageid,
                          p_envkey    => v_envkey,
                          p_logsource => v_logsource,
                          p_filename  => p_filename,
                          p_batchid   => v_batchid,
                          p_jobnumber => v_my_log_id,
                          p_message   => v_message,
                          p_reason    => v_reason,
                          p_error     => v_error,
                          p_trycount  => v_trycount,
                          p_msgtime   => SYSDATE);
      RAISE;
  END TierNomination;


 PROCEDURE Process_TierNomination(p_processDate DATE,
                            p_totpartitions INTEGER, p_currpartition INTEGER) IS

   -- lv_tierid NUMBER := -1;
    --lv_reasonid NUMBER:=1;
    lv_membertierid NUMBER := -1;

   CURSOR GET_TIERS IS
      SELECT Mt.Id
      FROM   Bp_Ae.Ae_Tiernomination Tn
      INNER  JOIN Bp_Ae.Lw_Virtualcard Vc    ON     Tn.Loyaltynumber = Vc.Loyaltyidnumber
      INNER  JOIN Bp_Ae.Lw_Membertiers Mt    ON     Mt.Memberid = Vc.Ipcode
      WHERE  1=1
      AND Mt.Todate > p_processDate
      AND tn.Flag_Exception = 0 --AEO-1884 Field without exception
      AND MOD ( vc.ipcode, p_totpartitions) = p_currpartition; --AEO-1884 Get the partition by the Tiernomination.last_dml_id

	 CURSOR GET_DATA IS
    SELECT t.Ipcode, t.Reason, t.reasoncode, t.enrolldate , ti.tierid , ti.tiername, t.netspend,
          Hibernate_Sequence.Nextval AS id
    FROM   (SELECT Vc.Ipcode,
                   Rt.a_Description AS Reason,
                   rt.a_reasoncode AS ReasonCode,

                   CASE
                        WHEN Tn.Tier = '1' THEN
                         'Full Access'
                        ELSE
                         'Extra Access'
                   END AS Tiername,
                   md.a_netspend AS netspend,
                   nvl(lm.membercreatedate,to_date('1/1/1900','mm/dd/yyyy')) AS enrolldate
            FROM   Bp_Ae.Ae_Tiernomination Tn
            INNER  JOIN Bp_Ae.Lw_Virtualcard Vc      ON     Vc.Loyaltyidnumber = Tn.Loyaltynumber
            INNER  JOIN Bp_Ae.Ats_Memberdetails Md   ON     Md.a_Ipcode        = VC.IPCODE
            INNER  JOIN Bp_Ae.Lw_Loyaltymember  lm   ON     lm.ipcode        = VC.IPCODE
            INNER  JOIN Bp_Ae.Ats_Reftierreason Rt   ON     Rt.a_Reasoncode = Tn.Reasoncode
            WHERE 1=1
            AND MOD ( vc.ipcode, p_totpartitions) = p_currpartition --AEO-1884 Get the partition by the Tiernomination.last_dml_id
            AND tn.Flag_Exception = 0 --AEO-1884 Field without exception
            ) t
    INNER  JOIN Bp_Ae.Lw_Tiers Ti  ON  Ti.Tiername = t.Tiername
    WHERE  1=1
    ;

    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;

    TYPE T_TABTIERS IS TABLE OF GET_TIERS%ROWTYPE;
    V_TBLTIERS T_TABTIERS;

  BEGIN
    Validate_Data(p_processDate,  p_totpartitions, p_currpartition); --AEO-1884 Validation on Data

	  OPEN GET_TIERS;
    LOOP
      FETCH GET_TIERS BULK COLLECT INTO V_TBLTIERS LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FORALL I IN 1 .. V_TBLTIERS.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop


           UPDATE bp_ae.lw_membertiers md
           SET md.todate = p_processDate,
               md.updatedate = SYSDATE
           WHERE md.id =   v_tblTIERS(i).id;


	    COMMIT  WRITE BATCH NOWAIT ;

      EXIT WHEN GET_TIERS%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF GET_TIERS%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE GET_TIERS;
    END IF;

  ----


    OPEN GET_DATA;
    LOOP
      FETCH GET_DATA BULK COLLECT INTO V_TBL LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FORALL I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop

           INSERT INTO Lw_Membertiers Mt
                (Id,
                 Tierid,
                 Memberid,
                 Fromdate,
                 Todate,
                 Description,
                 Createdate,
                 Updatedate)
           VALUES
                ( v_tbl(i).id,
                 v_tbl(i).tierid,
                 v_tbl(i).ipcode,
                 p_processdate,
                 Getexpirationdate( v_tbl(i).enrolldate, p_processdate),
                 v_tbl(i).reason,
                 SYSDATE,
                 SYSDATE);



      FORALL I IN 1 .. V_TBL.COUNT
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
                 v_tbl(i).ipcode,
                 -1,
                 v_tbl(i).id,
                 v_tbl(i).Netspend,
                 0,
                 SYSDATE,
                 SYSDATE);

--AEO-1884 Begin - Update AIT flag in tier nomination
     FORALL I IN 1 .. V_TBL.COUNT
       UPDATE bp_ae.ats_memberdetails md
           SET md.a_aitupdate = 1
    	       , md.updatedate = SYSDATE
             , md.a_changedby = 'TierNomination'
       WHERE md.a_ipcode = v_tbl(i).ipcode
       ;
--AEO-1884 End

	    COMMIT  WRITE BATCH NOWAIT ;

      EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE GET_DATA;
    END IF;

    --- send an e-mail with some numbers

END;
--rollout AEO-1214 end


END AEtiers;
/
