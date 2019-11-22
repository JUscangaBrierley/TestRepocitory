CREATE OR REPLACE PACKAGE Aejobs IS

     TYPE Rcursor IS REF CURSOR;

     PROCEDURE Updatelastsmsdatesent(p_Dummy VARCHAR2,
                                     Retval  IN OUT Rcursor);

     PROCEDURE Updatelastcardreplacementdate(p_Dummy VARCHAR2,
                                             Retval  IN OUT Rcursor);

     PROCEDURE Updatelasttempcardreplacedate(p_Dummy VARCHAR2,
                                             Retval  IN OUT Rcursor);
     /*AEO 567 - Begin here      JHC */
     PROCEDURE Stageaitprofilesentdate(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor);
     /*AEO 567 - Ends here      JHC */

     PROCEDURE Updatelastaitprofilesentdate(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor);

     /*  AEO-411:Update SVS Verification Date
     v.1.0 EHP UpdtLastSMSVerificationSentDate*/
     PROCEDURE Updtlastsmsverificsentdate(p_Dummy VARCHAR2,
                                          Retval  IN OUT Rcursor);

     /*Redesign 2015 begin here*******RON*/
     PROCEDURE Updatelasttiernotificationdate(p_Dummy VARCHAR2);

     /*Redesign 2015 begin here*******RON*/
     /*Changes for PI29896 begin here****SCJ*/
     PROCEDURE Updatelastaememberpointssent(p_Dummy VARCHAR2);

     /*Changes for PI29896 end here***SCJ*/
     PROCEDURE Updatememberrewardredemption(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor);

     PROCEDURE Clearmobilealerts(Retval IN OUT Rcursor);

     PROCEDURE Resetaitupdateflag(p_Dummy VARCHAR2,
                                  Retval  IN OUT Rcursor);

     /*Redesign 2015 begin here*******RKG*/
     PROCEDURE Updaterewardsequences(p_Dummy VARCHAR2);


     PROCEDURE Updateb5g1rewardsequence(p_Dummy VARCHAR2);

     PROCEDURE Updateemailbirthdaysequences(p_Dummy VARCHAR2);

     /*AEO-541 begin here*******JHC*/
     PROCEDURE UpdateB5G1DMRewardSequence(p_Dummy VARCHAR2);
     PROCEDURE UpdateRewardsDMSequenceNumber(p_Dummy VARCHAR2);
     /*AEO-541 end here*******JHC*/

     PROCEDURE Updatesmsbirthdaysequences(p_Dummy VARCHAR2);

     PROCEDURE Updatedmbirthdaysequences(p_Dummy VARCHAR2);

     /*Redesign 2015 begin here*******RKG*/
     PROCEDURE Calculatepointbalances(p_Jobname     VARCHAR2,
                                      p_Processdate DATE,
                                      Retval        IN OUT Rcursor);

     --Changes for PI30364 begin here  -- SCJ
     PROCEDURE Calculatepointbalances2(p_Jobname     VARCHAR2,
                                       p_Processdate DATE,
                                       Retval        IN OUT Rcursor);

     --Changes for PI30364 end here  -- SCJ
     PROCEDURE Processbrapromoredemptions;

     PROCEDURE Updateemployeecode(p_Dummy VARCHAR2,
                                  Retval  IN OUT Rcursor);

     PROCEDURE Exirebrapoints(p_Dummy VARCHAR2,
                              Retval  IN OUT Rcursor);

     PROCEDURE Matchpostalcodes(p_Dummy VARCHAR2,
                                Retval  IN OUT Rcursor);

     PROCEDURE Aepointheaderload(p_Dummy       VARCHAR2,
                                 p_Processdate DATE);

     PROCEDURE Memberpointsfullload(p_Dummy       VARCHAR2,
                                    p_Processdate DATE,
                                    Retval        IN OUT Rcursor);

     PROCEDURE Aepointheaderdeltaload(p_Dummy       VARCHAR2,
                                      p_Processdate DATE);

     PROCEDURE Memberpointsdeltafullload(p_Dummy       VARCHAR2,
                                         p_Processdate DATE,
                                         Retval        IN OUT Rcursor);

     PROCEDURE Aepointheaderdeltaload2(p_Dummy       VARCHAR2,
                                       p_Processdate DATE);

     PROCEDURE Memberpointsdeltafullload2(p_Dummy       VARCHAR2,
                                          p_Processdate DATE,
                                          Retval        IN OUT Rcursor);

     PROCEDURE Aepointheaderdeltaload3(p_Dummy       VARCHAR2,
                                       p_Processdate DATE);

     PROCEDURE Memberpointsdeltafullload3(p_Dummy       VARCHAR2,
                                          p_Processdate DATE,
                                          Retval        IN OUT Rcursor);

     PROCEDURE Aepointheaderdeltaloadqr(p_Dummy       VARCHAR2,
                                        p_Processdate DATE);

     PROCEDURE Memberpointsdeltafullloadqr(p_Dummy       VARCHAR2,
                                           p_Processdate DATE,
                                           Retval        IN OUT Rcursor);

     PROCEDURE Aepointheaderdeltaload_New(p_Dummy               VARCHAR2,
                                          p_Processdate         DATE,
                                          p_Memberfilestarttime DATE);

     PROCEDURE Memberpointsdeltafullload_New(p_Dummy       VARCHAR2,
                                             p_Processdate DATE,
                                             Retval        IN OUT Rcursor);

     PROCEDURE Memberpointsdeltafullload_Qr(p_Dummy       VARCHAR2,
                                            p_Processdate DATE,
                                            Retval        IN OUT Rcursor);

     --Changes for PI30364 BEGIN here  -- SCJ
     PROCEDURE Headerrecord_Memberpoints(p_Dummy                VARCHAR2,
                                         p_Processdate          DATE,
                                         p_Memberfilestarttime  DATE,
                                         v_Transactionstartdate DATE);

     PROCEDURE Memberpointsdelta(p_Dummy       VARCHAR2,
                                 p_Processdate DATE,
                                 b_Name        VARCHAR2,
                                 Retval        IN OUT Rcursor);

     --Changes for PI30364 END here  -- SCJ
     PROCEDURE Custombrafullfilment(p_Dummy          VARCHAR2,
                                    p_Startissuedate VARCHAR2,
                                    p_Endissuedate   VARCHAR2,
                                    Retval           IN OUT Rcursor);

     --Changes for PI 30364 - Dollar reward program start here -- Akbar
     PROCEDURE Updatedollarrewardoptoutdate(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor);

     --Changes for PI 30364 - Dollar reward program end here -- Akbar
     PROCEDURE Updatelastsmsverifysend(p_Dummy VARCHAR2,
                                       Retval  IN OUT Rcursor);

     PROCEDURE Buildemailreminderlist(p_Dummy VARCHAR2,
                                      Retval  IN OUT Rcursor);

     PROCEDURE Updateemailreminderdate(p_Dummy VARCHAR2,
                                       Retval  IN OUT Rcursor);

     PROCEDURE Missedemailverif_081115(p_Dummy VARCHAR2,
                                       Retval  IN OUT Rcursor); --  Missed e-mail verification sedn 08Nov2015

    -- AEO-817 begin
     PROCEDURE UpdateRewards10DMSeqNumber(p_number INTEGER) ;

     PROCEDURE UpdateRewards10SeqNumber(p_number INTEGER) ;
   -- AEO-817 end

   -- AEO-907
     PROCEDURE CheckSequenceCodes( p_SequencePrefix VARCHAR2, p_SequenceName VARCHAR2 );

-- AEO-880 begin
     PROCEDURE UpdateStackedSequences(p_Dummy VARCHAR2);
  -- AEO-880 end

--AEO-1592 begin GD
     PROCEDURE ResetEmployeeCode(p_Dummy VARCHAR2, Retval IN OUT Rcursor);
--AEO-1592 end GD
END Aejobs;
/
CREATE OR REPLACE PACKAGE BODY Aejobs IS

     /********************************************************************
     ********************************************************************/
     /******************* Internal function to creat check digits*********/
     FUNCTION Createhhkeycheckdigit(p_Hhkey_String VARCHAR2) RETURN VARCHAR2 IS
          v_Total        INT;
          v_Digit        INT;
          v_Pointer      NUMBER;
          v_Returnstring NVARCHAR2(20);
     BEGIN
          IF TRIM(p_Hhkey_String) IS NULL
          THEN
               RETURN NULL;
          END IF;
          v_Pointer := 1;
          v_Total   := 0;
          WHILE Length(p_Hhkey_String) > v_Pointer
          LOOP
               v_Digit   := CAST(Substr(p_Hhkey_String, v_Pointer, 1) AS INT);
               v_Total   := v_Total + v_Digit;
               v_Pointer := v_Pointer + 1;
          END LOOP;
          v_Total        := MOD(v_Total, 10);
          v_Returnstring := '00' || p_Hhkey_String ||
                            CAST(v_Total AS NVARCHAR2);
          RETURN v_Returnstring;
     EXCEPTION
          WHEN OTHERS THEN
               RETURN NULL;
     END Createhhkeycheckdigit;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Updatelastsmsdatesent(p_Dummy VARCHAR2,
                                     Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastSMSDateSent';
          COMMIT;
     END Updatelastsmsdatesent;

     PROCEDURE Updatelastcardreplacementdate(p_Dummy VARCHAR2,
                                             Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastCardReplacementDate';
          COMMIT;
     END Updatelastcardreplacementdate;

     PROCEDURE Updatelasttempcardreplacedate(p_Dummy VARCHAR2,
                                             Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastTempCardReplacementDate';
          COMMIT;
     END Updatelasttempcardreplacedate;

     /*AEO 567 - Begin here      JHC */
     PROCEDURE Stageaitprofilesentdate(p_Dummy VARCHAR2,
                                             Retval  IN OUT Rcursor) AS
     BEGIN
         EXECUTE IMMEDIATE 'Truncate Table BP_AE.AE_profile_updates';

         INSERT INTO BP_AE.AE_profile_updates
          --AEO-1441 Begin GD
            SELECT
            regexp_replace(vc.linkkey,chr(10)||'|'||CHR(13)||'|'||CHR(9), '')  AS CID
            , vc.ipcode
            , vc.loyaltyidnumber
            , regexp_replace(REPLACE(md.a_emailaddress , '-'),CHR(10)||'|'||CHR(13)||'|'||CHR(9),'') AS "EmailAddress"
            , CASE WHEN vc.isprimary = 1 THEN 'P' ELSE 'S' END AS RecordType
            , CASE
              WHEN lm.memberstatus IN (2,3,5) THEN TO_CHAR(lm.memberstatus)
              WHEN lm.memberstatus IN (4) THEN TO_CHAR(lm.memberstatus)
              ELSE TO_CHAR(1)
              END AS MemberStatus
            , CASE md.a_terminationreasonid
                   WHEN NULL THEN
                     ''
                   ELSE
                    CASE
                      WHEN lm.memberstatus = 3 THEN TO_CHAR(md.a_terminationreasonid)
                      ELSE ''
                    END
              END AS TerminationReasonCode --AEO-1676
            , CASE md.updatedate WHEN NULL THEN '' ELSE TO_CHAR(md.updatedate,'mmddyyyy') END AS UpdateDate
            , CASE md.a_changedby WHEN NULL THEN CAST('' AS NVARCHAR2(100)) ELSE regexp_replace(md.a_changedby,chr(10)||'|'||CHR(13)||'|'||CHR(9), '') END AS ChangedBy
            , CASE lm.memberclosedate WHEN NULL THEN '' ELSE TO_CHAR(lm.memberclosedate,'mmddyyyy') END AS MemberCloseDate
            , CASE ti.tiername WHEN NULL THEN CAST('' AS NVARCHAR2(50)) ELSE ti.tiername END AS TierName
            , CASE ti.fromdate WHEN NULL THEN '' ELSE TO_CHAR(ti.fromdate,'mmddyyyy') END AS TierStartDate
            , CASE ti.todate WHEN NULL THEN '' ELSE TO_CHAR(ti.todate,'mmddyyyy') END AS TierEndDate
            , CASE md.a_emailaddressupdatedate WHEN NULL THEN '' ELSE TO_CHAR(md.a_emailaddressupdatedate,'DDMMYYYYHH24MISS') END AS EmailAddressUpdateDate
            , TO_CHAR(Ae_Isinpilot(Md.a_Extendedplaycode)) AS PilotFlag --AEO-1676
            FROM   bp_ae.lw_virtualcard vc
            INNER JOIN bp_ae.lw_loyaltymember lm   ON vc.ipcode = lm.ipcode
            INNER JOIN bp_ae.ats_memberdetails md  ON vc.ipcode = md.a_ipcode
            LEFT OUTER JOIN (
            SELECT * FROM
            (
              SELECT mt.memberid, t.tiername, mt.fromdate, mt.todate, Row_Number() Over(PARTITION BY mt.memberid ORDER BY mt.id DESC) AS r
              FROM bp_ae.lw_membertiers mt
              INNER JOIN bp_ae.lw_tiers t ON mt.tierid = t.tierid
              WHERE SYSDATE BETWEEN mt.fromdate AND mt.todate
            )
            WHERE r = 1
            ) ti
            ON ti.memberid = vc.ipcode
            WHERE 1=1
            AND md.a_aitupdate = 1
            AND vc.loyaltyidnumber NOT IN ((  SELECT vc1.loyaltyidnumber AS loyalty
                    FROM bp_Ae.Lw_Virtualcard vc1
                    WHERE SUBSTR(vc1.loyaltyidnumber, 1, 1) = '0' OR
                           LENGTH(vc1.loyaltyidnumber) <> 14 ))
            ORDER BY vc.IPCode;
          --AEO-1441 End GD

     END Stageaitprofilesentdate;

     PROCEDURE Updatelastaitprofilesentdate(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastAITProfileSentDate';
          COMMIT;
     END Updatelastaitprofilesentdate;
     /*AEO 567 - Ends here      JHC */

     /*  AEO-411:Update SVS Verification Date
     v.1.0 EHP UpdateLastSMSVerificationSentDate*/
     PROCEDURE Updtlastsmsverificsentdate(p_Dummy VARCHAR2,
                                          Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastSMSVerificationSendDate';
          COMMIT;
     END Updtlastsmsverificsentdate;

     /***********Changes for AEO Redesign 2015 begin here****RON***********************************************************/
     PROCEDURE Updatelasttiernotificationdate(p_Dummy VARCHAR2) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE - 5 / 1440, 'mm/dd/yyyy hh24:mi:ss') -- sets 5 mins before the sysdate,to allow for vc procesing time overlap
          WHERE  Key = 'LastTierNotificationDate';
          COMMIT;
     END Updatelasttiernotificationdate;

     /***********Changes for AEO Redesign 2015 END here****RON***********************************************************/
     /*********************************************************************
     ********************************************************************
     ********************************************************************
     */
     PROCEDURE Updaterewardsequences(p_Dummy VARCHAR2) AS
          v_Rewardsequence INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Rewardsequence
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'RewardsSequenceNumber';
          v_Rewardsequence := v_Rewardsequence + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Rewardsequence)
          WHERE  Key = 'RewardsSequenceNumber';
          COMMIT;
     END Updaterewardsequences;

     /*********************************************************************
     ********************************************************************
     ********************************************************************
     */
     PROCEDURE Updateb5g1rewardsequence(p_Dummy VARCHAR2) AS
          v_Rewardsequence INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Rewardsequence
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'B5G1RewardSequence';
          v_Rewardsequence := v_Rewardsequence + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Rewardsequence)
          WHERE  Key = 'B5G1RewardSequence';
          COMMIT;
     END Updateb5g1rewardsequence;

     /*********************************************************************
     ********************************************************************
     ********************************************************************
     */
     PROCEDURE Updateemailbirthdaysequences(p_Dummy VARCHAR2) AS
          v_Birthdaysequence INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Birthdaysequence
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'EmailBirthdaySequenceNumber';
          v_Birthdaysequence := v_Birthdaysequence + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Birthdaysequence)
          WHERE  Key = 'EmailBirthdaySequenceNumber';
          COMMIT;
     END Updateemailbirthdaysequences;

     /*********************************************************************
     ********************************************************************
     ********************************************************************
     */
     PROCEDURE Updatesmsbirthdaysequences(p_Dummy VARCHAR2) AS
          v_Birthdaysequence INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Birthdaysequence
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'SMSBirthdaySequenceNumber';
          v_Birthdaysequence := v_Birthdaysequence + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Birthdaysequence)
          WHERE  Key = 'SMSBirthdaySequenceNumber';
          COMMIT;
     END Updatesmsbirthdaysequences;

     /*********************************************************************
     ********************************************************************
     ********************************************************************
     */
     PROCEDURE Updatedmbirthdaysequences(p_Dummy VARCHAR2) AS
          v_Birthdaysequence INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Birthdaysequence
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'DMBirthdaySequenceNumber';
          v_Birthdaysequence := v_Birthdaysequence + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Birthdaysequence)
          WHERE  Key = 'DMBirthdaySequenceNumber';
          COMMIT;
     END Updatedmbirthdaysequences;

     /*********************************************************************
     *AEO-541 begin here*******JHC*
     ********************************************************************
     */
     PROCEDURE UpdateRewardsDMSequenceNumber(p_Dummy VARCHAR2) AS
          v_RewardsDMSequenceNumber INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_RewardsDMSequenceNumber
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'RewardsDMSequenceNumber';
          v_RewardsDMSequenceNumber := v_RewardsDMSequenceNumber + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_RewardsDMSequenceNumber)
          WHERE  Key = 'RewardsDMSequenceNumber';
          COMMIT;
     END UpdateRewardsDMSequenceNumber;

     /*********************************************************************
     ********************************************************************
     ********************************************************************
     */
     PROCEDURE UpdateB5G1DMRewardSequence(p_Dummy VARCHAR2) AS
          v_B5G1DMRewardSequence INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_B5G1DMRewardSequence
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'B5G1DMRewardSequence';
          v_B5G1DMRewardSequence := v_B5G1DMRewardSequence + 1;
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_B5G1DMRewardSequence)
          WHERE  Key = 'B5G1DMRewardSequence';
          COMMIT;
     END UpdateB5G1DMRewardSequence;
     /*AEO-541 end here*******JHC*/

     /***********Changes for AEO Redesign 2015 END here****RON***********************************************************/
     /***********Changes for PI29896 begin here****SCJ***********************************************************/
     PROCEDURE Updatelastaememberpointssent(p_Dummy VARCHAR2) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE - 5 / 1440, 'mm/dd/yyyy hh24:mi:ss') -- sets 5 mins before the sysdate,to allow for vc procesing time overlap
          WHERE  Key = 'LastAEMemberPointsSent';
          COMMIT;
     END Updatelastaememberpointssent;

     /*********Changes for PI29896 end here****SCJ********************************************************/
     PROCEDURE Updatelastaitemailsentdate(p_Dummy VARCHAR2,
                                          Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastAITEmailSentDate';
          COMMIT;
     END Updatelastaitemailsentdate;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Resetaitupdateflag(p_Dummy VARCHAR2,
                                  Retval  IN OUT Rcursor) AS
          v_Logsource        VARCHAR2(256) := 'AEJobs.ResetAITUpdateFlag';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AEJobs.ResetAITUpdateFlag';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Messagespassed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'ResetAITUpdateFlag';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
     BEGIN
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          /* log start of job (lw logging)*/
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
          EXECUTE IMMEDIATE 'Truncate Table AE_AITRecordsSent';
          INSERT INTO Ae_Aitrecordssent
               SELECT Ipcode
               FROM   AE_PROFILE_UPDATES;
          COMMIT;
          ---------------------------
          -- Update AITUpdate flag
          ---------------------------
          DECLARE
               CURSOR Get_Data IS
                    SELECT Ipcode
                    FROM   AE_PROFILE_UPDATES;

               TYPE t_Tab IS TABLE OF Get_Data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
               v_Tbl t_Tab; ---<------ our arry object
          BEGIN
               OPEN Get_Data;
               LOOP
                    FETCH Get_Data BULK COLLECT
                         INTO v_Tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
                    FORALL i IN 1 .. v_Tbl.Count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
                         UPDATE Ats_Memberdetails Mds
                         SET    Mds.a_Aitupdate = 0
                         WHERE  Mds.a_Ipcode = v_Tbl(i).Ipcode;
                    COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
                    EXIT WHEN Get_Data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
               END LOOP;
               COMMIT;
               IF Get_Data%ISOPEN
               THEN
                    --<--- dont forget to close cursor since we manually opened it.
                    CLOSE Get_Data;
               END IF;
          END;
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
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure ResetAITUpdateFlag';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>AEJobs</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>ResetAITUpdateFlag</proc>' ||
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
     END Resetaitupdateflag;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Updatememberrewardredemption(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor) AS
          v_Logsource        VARCHAR2(256) := 'AEJobs.UpdateMemberRewardRedemption';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AEJobs.UpdateMemberRewardRedemption';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Messagespassed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'UpdateMemberRewardRedemption';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
     BEGIN
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          /* log start of job (lw logging)*/
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
          --This code uses the Merge function to update the MemberReward redemption date based off of the txn rewardredeem table and the rewardfulfillemnt
          --tables.  the txn rewardredeem table is the current tlog in the staging table and the rewardfulfillment table is the current rewards from the
          --last quarterly rewards run.
          MERGE INTO Lw_Memberrewards Mr
          USING (SELECT Rr.Txndate, Rrf.a_Memberrewardid
                 FROM   Ats_Memberrewardfulfillment Rrf,
                        Lw_Txnrewardredeem_Stage    Rr
                 WHERE  Rrf.a_Rewardpartnumber = Rr.Certificatecode
                        AND Rrf.a_Ipcode = Rr.Ipcode) v
          ON (Mr.Id = v.a_Memberrewardid)
          WHEN MATCHED THEN
          --    UPDATE SET Mr.Redemptiondate = v.Txndate, Mr.Ordernumber = '3'; --// AEO-74 Upgrade 4.5 changes here -----------SCJ
               UPDATE
               SET    Mr.Redemptiondate = v.Txndate,
                      Mr.Lwordernumber  = '3';
          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
          --This statement updates the reward if it is a replacement reward.  The Reward History module in CS creates a record in the rewardfulfillment table
          --and marks the parentrewardid to be able to tie back to the original reward.
          MERGE INTO Lw_Memberrewards Mr
          USING (SELECT Rr.Txndate, Rrf.a_Parentrewardid
                 FROM   Ats_Memberrewardfulfillment Rrf,
                        Lw_Txnrewardredeem_Stage    Rr
                 WHERE  Rrf.a_Rewardpartnumber = Rr.Certificatecode
                        AND Rrf.a_Ipcode = Rr.Ipcode) v
          ON (Mr.Id = v.a_Parentrewardid)
          WHEN MATCHED THEN
          --    UPDATE SET Mr.Redemptiondate = v.Txndate, Mr.Ordernumber = '3'; --// AEO-74 Upgrade 4.5 changes here -----------SCJ
               UPDATE
               SET    Mr.Redemptiondate = v.Txndate,
                      Mr.Lwordernumber  = '3';
          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
          COMMIT;
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
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure UpdateMemberRewardRedemption';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>AEJobs</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>UpdateMemberRewardRedemption</proc>' ||
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
     END Updatememberrewardredemption;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Clearmobilealerts(Retval IN OUT Rcursor) AS
          v_Logsource        VARCHAR2(256) := 'AEJobs.ClearMobileAlerts';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AEJobs.ClearMobileAlerts';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Messagespassed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'ClearMobileAlerts';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
     BEGIN
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          /* log start of job (lw logging)*/
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
          --This code is needed to clear out the MobileAlerts table to be ready for the next batch of mobile alert loyalty numbers
          EXECUTE IMMEDIATE 'TRUNCATE TABLE ats_MobileAlerts';
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
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure ClearMobileAlerts';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>AEJobs</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>ClearMobileAlerts</proc>' ||
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
     END Clearmobilealerts;

     /********************************************************************
     ********************************************************************/
     /******************* Internal function to tranform date ************/
     FUNCTION Getquarterdates(p_Date_String      VARCHAR2,
                              p_Previousquarter  BOOLEAN,
                              p_Quarterstartdate OUT DATE,
                              p_Quarterenddate   OUT DATE) RETURN VARCHAR2 IS
          v_Date         DATE;
          v_Quarter      CHAR;
          v_Year         CHAR(4);
          v_Returnstring NVARCHAR2(20);
          v_Tempdate     CHAR(20);
     BEGIN
          IF TRIM(p_Date_String) IS NULL
          THEN
               RETURN NULL;
          END IF;
          v_Quarter := To_Char(To_Date(p_Date_String, 'mm/dd/yyyy'), 'Q');
          v_Year    := To_Char(To_Date(p_Date_String, 'mm/dd/yyyy'), 'yyyy');
          IF v_Quarter = '1'
          THEN
               v_Tempdate := '1/1/' || v_Year;
          END IF;
          IF v_Quarter = '2'
          THEN
               v_Tempdate := '4/1/' || v_Year;
          END IF;
          IF v_Quarter = '3'
          THEN
               v_Tempdate := '7/1/' || v_Year;
          END IF;
          IF v_Quarter = '4'
          THEN
               v_Tempdate := '10/1/' || v_Year;
          END IF;
          p_Quarterstartdate := To_Date(v_Tempdate, 'mm/dd/yyyy');
          p_Quarterenddate   := Add_Months(p_Quarterstartdate, 3) - 1;
          IF p_Previousquarter
          THEN
               p_Quarterstartdate := Add_Months(p_Quarterstartdate, -3);
               p_Quarterenddate   := Add_Months(p_Quarterenddate, -3);
          END IF;
          RETURN v_Returnstring;
     EXCEPTION
          WHEN OTHERS THEN
               RETURN NULL;
     END Getquarterdates;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Calculatepointbalances(p_Jobname     VARCHAR2,
                                      p_Processdate DATE,
                                      Retval        IN OUT Rcursor) AS
          v_Fulfillmentthreshold INTEGER;
          v_Startdate            CHAR(10);
          v_Enddate              CHAR(10);
          v_Returnstring         NVARCHAR2(20);
          v_My_Log_Id            NUMBER;
          v_Dap_Log_Id           NUMBER;
          --log job attributes
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512);
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'CalculatePointBalances';
          v_Rewardlevel_15   NUMBER;
          v_Rewardlevel_20   NUMBER;
          v_Rewardlevel_30   NUMBER;
          v_Rewardlevel_40   NUMBER;
          v_Rewardname_15    VARCHAR2(10) := '15%';
          v_Rewardname_20    VARCHAR2(10) := '20%';
          v_Rewardname_30    VARCHAR2(10) := '30%';
          v_Rewardname_40    VARCHAR2(10) := '40%';
     BEGIN
          /* get job id for this process and the dap process */
          v_My_Log_Id  := Utility_Pkg.Get_Libjobid();
          v_Dap_Log_Id := Utility_Pkg.Get_Libjobid();
          /* log start of job */
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Fulfillmentthreshold
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'BraPromoFulFillmentThreshold';
          EXECUTE IMMEDIATE 'Truncate Table Ats_Memberpointbalances';
          INSERT INTO Ats_Memberpointbalances Pb
               (a_Rowkey,
                a_Ipcode,
                a_Parentrowkey,
                a_Previoustotalpoints,
                a_Basepoints,
                a_Bonuspoints,
                a_Startingpoints,
                a_Rewardlevel,
                a_Pointstonextreward,
                a_Bracurrentpurchased,
                a_Bratotalfreemailed,
                a_Bralastfreemailed,
                a_Brandae,
                a_Brandaerie,
                a_Brandkids,
                a_Bracurrentfreeearned,
                a_Braemployeemultiplier,
                a_Bramailablemultiplier,
                a_Totalpoints,
                a_Priordaytotalpoints, -- PI 26302: Akbar, Calculating previous day points balance
                a_Bralifetimebalance,
                a_Brarollingbalance,
                a_Jeanslifetimebalance,
                a_Jeansrollingbalance,
                Statuscode,
                Createdate,
                Updatedate)
               SELECT /*+ full (vc) full (md) full (Pts) NO_PUSH_PRED(Lw_Clientconfiguration) NO_PUSH_PRED(Ats_Memberbrapromocertsummary) NO_PUSH_PRED(Ats_Memberbrapromocerthistory) NO_PUSH_PRED(Ats_Memberbrapromosummary) NO_PUSH_PRED(Ats_Memberbrand)*/
                Vc.Ipcode AS Rowkey,
                Vc.Ipcode,
                Vc.Ipcode,
                Greatest(SUM(CASE
                                  WHEN Ty.Name NOT LIKE 'Jean%'
                                       AND Ty.Name NOT LIKE 'Bra%'
                                       AND Trunc(Pts.Transactiondate, 'Q') =
                                       Add_Months(Trunc(p_Processdate, 'Q'), -3) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Lastqtr_Points,
                SUM(CASE
                         WHEN Ty.Name IN ('Basic Points', 'Adjustment Points')
                              AND Trunc(Pts.Transactiondate, 'Q') =
                              Trunc(p_Processdate, 'Q') THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Basic_Points,
                SUM(CASE
                    /* PI 25127, Akbar, accomodate new point type 'Adjustment Bonus Points' in bonus points */
                         WHEN Ty.Name IN ('CS Points',
                                          'Bonus Points',
                                          'Adjustment Bonus Points')
                              AND Trunc(Pts.Transactiondate, 'Q') =
                              Trunc(p_Processdate, 'Q') THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Bonus_Points,
                SUM(CASE
                         WHEN Ty.Name = 'StartingPoints'
                              AND Trunc(Pts.Transactiondate, 'Q') =
                              Trunc(p_Processdate, 'Q') THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Starting_Points,
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_40 THEN
                      Sv_Rewardname_40
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_30 THEN
                      Sv_Rewardname_30
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_20 THEN
                      Sv_Rewardname_20
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_15 THEN
                      Sv_Rewardname_15
                END AS Rewardlevel,
                CASE
                -- PI25109: Condition to check if the points > 500 (Sv_Rewardlevel_40) Begin
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) > Sv_Rewardlevel_40 THEN
                      SUM(CASE
                               WHEN Ty.Name NOT LIKE 'Jean%'
                                    AND Ty.Name NOT LIKE 'Bra%'
                                    AND Trunc(Pts.Transactiondate, 'Q') =
                                    Trunc(p_Processdate, 'Q') THEN
                                Pts.Points
                               ELSE
                                0
                          END) - Sv_Rewardlevel_40
                --PI25109: Condition to check if the points > 500 (Sv_Rewardlevel_40) End
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Sv_Rewardlevel_30 AND
                          (Sv_Rewardlevel_40 - 1) THEN
                      Sv_Rewardlevel_40 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Sv_Rewardlevel_20 AND
                          (Sv_Rewardlevel_30 - 1) THEN
                      Sv_Rewardlevel_30 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Sv_Rewardlevel_15 AND
                          (Sv_Rewardlevel_20 - 1) THEN
                      Sv_Rewardlevel_20 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) < Sv_Rewardlevel_15 THEN
                      Sv_Rewardlevel_15 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_40 THEN
                      0
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_30 THEN
                      (Sv_Rewardlevel_40 - Sv_Rewardlevel_30)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_20 THEN
                      (Sv_Rewardlevel_30 - Sv_Rewardlevel_20)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_15 THEN
                      Sv_Rewardlevel_15
                END AS Pointstonextreward,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Bra Points')
                                       AND -- AEO-70 removed Bra Adjustment points
                                       Pts.Expirationdate > p_Processdate
                                       AND Pts.Transactiontype IN (1, 2, 4) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Bracurrentpurchased,
                Nvl(Sv_Bratotalfreemailed, 0) AS Sv_Bratotalfreemailed,
                Sv_Bralastfreemailed AS Sv_Bralastfreemailed,
                Nvl(Ats_Memberbrand.Brandae, 0) AS Brandae,
                Nvl(Ats_Memberbrand.Brandaerie, 0) AS Brandaerie,
                Nvl(Ats_Memberbrand.Brandkids, 0) AS Brandkids,
                Greatest(Round(SUM(CASE
                                        WHEN Ty.Name IN ('Bra Points')
                                             AND -- AEO-70 removed Bra Adjustment points
                                             Pts.Expirationdate > p_Processdate
                                             AND Pts.Transactiontype IN (1, 2, 4) THEN
                                         Pts.Points
                                        ELSE
                                         0
                                   END) / Sv_Fulfillmentthreshold,
                               0),
                         0) AS Bracurrentfreeearned,
                Decode(To_Number(Nvl(Md.a_Employeecode, 0)), 1, 0, 1) AS a_Braemployeemultiplier,
                Decode(Md.a_Addressmailable, 1, 1, 0) a_Bramailablemultiplier,
                Greatest(SUM(CASE
                                  WHEN Ty.Name NOT LIKE 'Jean%'
                                       AND Ty.Name NOT LIKE 'Bra%'
                                       AND Trunc(Pts.Transactiondate, 'Q') =
                                       Trunc(p_Processdate, 'Q') THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Ttl_Points,
                -- PI 26302: Akbar, Calculating previous day points balance Begin
                Greatest(SUM(CASE
                                  WHEN Ty.Name NOT LIKE 'Jean%'
                                       AND Ty.Name NOT LIKE 'Bra%'
                                       AND Trunc(Pts.Pointawarddate) BETWEEN
                                       Trunc(p_Processdate, 'Q') AND Trunc(p_Processdate - 1)
                                       AND Trunc(Pts.Transactiondate, 'Q') =
                                       Trunc(p_Processdate, 'Q') THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Priorday_Points,
                -- PI 26302: Akbar, Calculating previous day points balance End
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Bra Points',
                                                   'Bra Adjustment Points',
                                                   'Bra Legacy Points',
                                                   'Bra Redemptions',
                                                   'Bra Employee Points')
                                       AND Pts.Transactiondate >
                                       To_Date('1/1/2009', 'mm/dd/yyyy')
                                       AND Pts.Transactiontype IN (1, 2) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Bralifetimebalance,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Bra Points',
                                                   'Bra Adjustment Points',
                                                   'Bra Redemptions',
                                                   'Bra Employee Points')
                                       AND Pts.Transactiondate >
                                       Add_Months(Trunc(p_Processdate, 'Q'), -12)
                                       AND Pts.Transactiontype IN (1, 2) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Brarollingbalance,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Jean Points', 'Jean Legacy Points')
                                       AND Pts.Transactiondate >
                                       To_Date('1/1/2009', 'mm/dd/yyyy') THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Jeanslifetimebalance,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Jean Points')
                                       AND Pts.Transactiondate >
                                       Add_Months(Trunc(p_Processdate, 'Q'), -12) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Jeansrollingbalance,
                1 AS Statuscode,
                SYSDATE AS Createdate,
                SYSDATE AS Updatedate
               FROM   Lw_Pointtransaction Pts,
                      Lw_Virtualcard Vc,
                      Lw_Pointtype Ty,
                      Ats_Memberdetails Md,
                      (SELECT CASE
                                   WHEN SUM(CASE --PI25722 FLAG FIX BEGINS HERE--
                                                 WHEN b.a_Shortbrandname = 'AE' THEN
                                                  1
                                                 ELSE
                                                  0
                                            END) >= 1 THEN
                                    1
                                   ELSE
                                    0
                              END AS Brandae,
                              CASE
                                   WHEN SUM(CASE
                                                 WHEN b.a_Shortbrandname = 'aerie' THEN
                                                  1
                                                 ELSE
                                                  0
                                            END) >= 1 THEN
                                    1
                                   ELSE
                                    0
                              END AS Brandaerie,
                              CASE
                                   WHEN SUM(CASE
                                                 WHEN b.a_Shortbrandname = '77kids' THEN
                                                  1
                                                 ELSE
                                                  0
                                            END) >= 1 THEN
                                    1
                                   ELSE
                                    0
                              END AS Brandkids, --PI25722 FLAG FIX ENDS HERE--
                              Mb.a_Ipcode
                       FROM   Ats_Memberbrand Mb, Ats_Refbrand b
                       WHERE  Mb.a_Brandid = b.a_Brandid
                       GROUP  BY Mb.a_Ipcode) Ats_Memberbrand,
                      (SELECT SUM(CASE
                                       WHEN Rd.Name LIKE 'Bra%' THEN
                                        1
                                       ELSE
                                        0
                                  END) AS Sv_Bratotalfreemailed,
                              MAX(CASE
                                       WHEN Rd.Name LIKE 'Bra%' THEN
                                        Mr.Dateissued
                                  END) AS Sv_Bralastfreemailed,
                              Mr.Memberid
                       FROM   Lw_Memberrewards Mr
                       INNER  JOIN Lw_Rewardsdef Rd
                       ON     Mr.Rewarddefid = Rd.Id
                       GROUP  BY Mr.Memberid) Lw_Memberrewards,
                      (SELECT /*+ cardinality ( x 1 ) */
                        MAX(CASE
                                 WHEN x.Name = '40% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_40,
                        MAX(CASE
                                 WHEN x.Name = '30% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_30,
                        MAX(CASE
                                 WHEN x.Name = '20% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_20,
                        MAX(CASE
                                 WHEN x.Name = '15% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_15,
                        '15%' AS Sv_Rewardname_15,
                        '20%' AS Sv_Rewardname_20,
                        '30%' AS Sv_Rewardname_30,
                        '40%' AS Sv_Rewardname_40
                       FROM   Lw_Rewardsdef x) Lw_Rewardsdef,
                      (SELECT /*+ cardinality ( x 1 ) */
                        To_Number(To_Char(x.Value)) AS Sv_Fulfillmentthreshold
                       FROM   Lw_Clientconfiguration x
                       WHERE  x.Key = 'BraPromoFulFillmentThreshold') Lw_Clientconfiguration
               WHERE  Pts.Vckey = Vc.Vckey
                      AND Pts.Pointtypeid = Ty.Pointtypeid
                      AND Vc.Ipcode = Md.a_Ipcode
                      AND Md.a_Ipcode = Ats_Memberbrand.a_Ipcode(+)
                      AND Md.a_Ipcode = Lw_Memberrewards.Memberid(+)
                      AND Transactiontype IN (1, 2, 4)
                      AND
                      Pts.Transactiondate >= To_Date('1/1/2009', 'mm/dd/yyyy')
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               GROUP  BY Vc.Ipcode,
                         Md.a_Employeecode,
                         Md.a_Addressmailable,
                         Ats_Memberbrand.Brandae,
                         Ats_Memberbrand.Brandaerie,
                         Ats_Memberbrand.Brandkids,
                         Sv_Rewardlevel_40,
                         Sv_Rewardlevel_30,
                         Sv_Rewardlevel_20,
                         Sv_Rewardlevel_15,
                         Sv_Rewardname_15,
                         Sv_Rewardname_20,
                         Sv_Rewardname_30,
                         Sv_Rewardname_40,
                         Sv_Bratotalfreemailed,
                         Sv_Bralastfreemailed,
                         Sv_Fulfillmentthreshold
               HAVING SUM(CASE
                    WHEN Trunc(Pts.Transactiondate, 'Q') =
                         Trunc(p_Processdate, 'Q') THEN
                     Pts.Points
                    WHEN Trunc(Pts.Transactiondate, 'Q') =
                         Add_Months(Trunc(p_Processdate, 'Q'), -3) THEN
                     Pts.Points
                    ELSE
                     0
               END) > 0;
          COMMIT;
          /* log end of job */
          Utility_Pkg.Log_Job(p_Job              => v_Dap_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => 'DAP-' || v_Jobname);
          v_Endtime := SYSDATE;
          /* log end of job */
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => 'Stage-' || v_Jobname);
          OPEN Retval FOR
               SELECT v_Dap_Log_Id FROM Dual;
     END Calculatepointbalances;

     /*----------------------------------------------------------------------------------------------------------
     ---------------------------------------------------------------------------------------------------------------------*/
     --Changes for PI30364 begin here   -- SCJ
     PROCEDURE Calculatepointbalances2(p_Jobname     VARCHAR2,
                                       p_Processdate DATE,
                                       Retval        IN OUT Rcursor) AS
          v_Fulfillmentthreshold INTEGER;
          v_Startdate            CHAR(10);
          v_Enddate              CHAR(10);
          v_Returnstring         NVARCHAR2(20);
          v_My_Log_Id            NUMBER;
          v_Dap_Log_Id           NUMBER;
          --log job attributes
          v_Jobdirection         NUMBER := 0;
          v_Filename             VARCHAR2(512);
          v_Starttime            DATE := SYSDATE;
          v_Endtime              DATE;
          v_Messagesreceived     NUMBER := 0;
          v_Messagesfailed       NUMBER := 0;
          v_Jobstatus            NUMBER := 0;
          v_Jobname              VARCHAR2(256) := 'CalculatePointBalances';
          v_Rewardlevel_15       NUMBER;
          v_Rewardlevel_20       NUMBER;
          v_Rewardlevel_30       NUMBER;
          v_Rewardlevel_40       NUMBER;
          v_Dollarrewardlevel    NUMBER := 0;
          v_Dollarrewardcutoff   NUMBER := 100;
          v_Rewardname_10        VARCHAR2(10) := '10';
          v_Rewardname_15        VARCHAR2(10) := '15%';
          v_Rewardname_20        VARCHAR2(10) := '20%';
          v_Rewardname_30        VARCHAR2(10) := '30%';
          v_Rewardname_40        VARCHAR2(10) := '40%';
          v_Transactionstartdate DATE;
          v_Sql1                 VARCHAR2(1000);
          v_Sql2                 VARCHAR2(1000);
     BEGIN
          /* get job id for this process and the dap process */
          v_My_Log_Id  := Utility_Pkg.Get_Libjobid();
          v_Dap_Log_Id := Utility_Pkg.Get_Libjobid();
          /* log start of job */
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Fulfillmentthreshold
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = 'BraPromoFulFillmentThreshold';
          --  v_sql1 := ' Select to_date(to_char(value), ''mm/dd/yyyy'')  from lw_clientconfiguration cc where cc.key = ''CalculateTransactionStartDate''';
          v_Sql1 := ' Select to_date(to_char(value), ''mm/dd/yyyy'')  from lw_clientconfiguration cc where cc.key = ''PilotStartDate''';
          v_Sql2 := ' Select to_char(value)  from lw_clientconfiguration cc where cc.key = ''DollarRewardsPoints''';
          EXECUTE IMMEDIATE 'Truncate Table Ats_Memberpointbalances';
          EXECUTE IMMEDIATE v_Sql1
               INTO v_Transactionstartdate;
          EXECUTE IMMEDIATE v_Sql2
               INTO v_Dollarrewardlevel;
          INSERT INTO Ats_Memberpointbalances Pb
               (a_Rowkey,
                a_Ipcode,
                a_Parentrowkey,
                a_Previoustotalpoints,
                a_Basepoints,
                a_Bonuspoints,
                a_Startingpoints,
                a_Rewardlevel,
                a_Pointstonextreward,
                a_Bracurrentpurchased,
                a_Bratotalfreemailed,
                a_Bralastfreemailed,
                a_Brandae,
                a_Brandaerie,
                a_Brandkids,
                a_Bracurrentfreeearned,
                a_Braemployeemultiplier,
                a_Bramailablemultiplier,
                a_Totalpoints,
                a_Priordaytotalpoints, -- PI 26302: Akbar, Calculating previous day points balance
                a_Bralifetimebalance,
                a_Brarollingbalance,
                a_Jeanslifetimebalance,
                a_Jeansrollingbalance,
                Statuscode,
                Createdate,
                Updatedate)
               SELECT /*+ full (vc) full (md) full (Pts) NO_PUSH_PRED(Lw_Clientconfiguration) NO_PUSH_PRED(Ats_Memberbrapromocertsummary) NO_PUSH_PRED(Ats_Memberbrapromocerthistory) NO_PUSH_PRED(Ats_Memberbrapromosummary) NO_PUSH_PRED(Ats_Memberbrand)*/
                Vc.Ipcode AS Rowkey,
                Vc.Ipcode,
                Vc.Ipcode,
                CASE Ae_Isinpilot(Md.a_Extendedplaycode)
                     WHEN 0 THEN
                      SUM(CASE
                               WHEN Ty.Name NOT LIKE 'Jean%'
                                    AND Ty.Name NOT LIKE 'Bra%'
                                    AND Trunc(Pts.Transactiondate, 'Q') =
                                    Add_Months(Trunc(p_Processdate, 'Q'), -3) THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     WHEN 1 THEN
                      SUM(CASE
                               WHEN Ty.Name NOT LIKE 'Jean%'
                                    AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Trunc(Pts.Transactiondate) <=
                                    Add_Months(Trunc(p_Processdate, 'Mon'), -1) THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                END AS Lastqtr_Points,
                CASE
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      SUM(CASE
                               WHEN Ty.Name IN ('Basic Points', 'Adjustment Points') --AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Pts.Transactiondate <
                                    Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) THEN
                      SUM(CASE
                               WHEN Ty.Name IN ('AEO Connected Points',
                                                'AEO Visa Card Points',
                                                'Adjustment Points') --AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Pts.Transactiondate <
                                    Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                END AS Basic_Points,
                CASE Ae_Isinpilot(Md.a_Extendedplaycode)
                     WHEN 0 THEN
                      SUM(CASE
                          /* PI 25127, Akbar, accomodate new point type 'Adjustment Bonus Points' in bonus points */
                               WHEN Ty.Name IN ('CS Points',
                                                'Bonus Points',
                                                'Adjustment Bonus Points') --AND  Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Pts.Transactiondate <
                                    Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     WHEN 1 THEN
                      SUM(CASE
                               WHEN Ty.Name IN ('AEO Customer Service Points',
                                                'AEO Connected Bonus Points',
                                                'Adjustment Bonus Points',
                                                'Bonus Points') --AND  Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Pts.Transactiondate <
                                    Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                END AS Bonus_Points,
                SUM(CASE
                         WHEN Ty.Name = 'StartingPoints' --AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                              AND Pts.Transactiondate <
                              Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                              AND Pts.Expirationdate > p_Processdate THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Starting_Points,
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_40
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardname_40
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Pts.Transactiondate >= v_TransactionStartdate
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_30
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardname_30
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' -- and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_20
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardname_20
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Sv_Rewardlevel_15
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardname_15
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                      THEN
                      To_Char(Floor(SUM(CASE
                                             WHEN Ty.Name IN ('AEO Connected Points',
                                                              'AEO Connected Bonus Points',
                                                              'AEO Visa Card Points',
                                                              'AEO Customer Service Points',
                                                              'Adjustment Points',
                                                              'Adjustment Bonus Points')
                                                  AND Pts.Expirationdate > p_Processdate THEN
                                              Pts.Points
                                             ELSE
                                              0
                                        END) / 1000))
                END AS Rewardlevel,
                CASE
                -- PI25109: Condition to check if the points > 500 (Sv_Rewardlevel_40) Begin
                -- PI25109 was corrected to show 0 points to next rewards,if total points > 400 in PI PI30364
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) > Sv_Rewardlevel_40
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      0
                --PI25109: Condition to check if the points > 500 (Sv_Rewardlevel_40) End
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Sv_Rewardlevel_30 AND
                          (Sv_Rewardlevel_40 - 1)
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardlevel_40 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Sv_Rewardlevel_20 AND
                          (Sv_Rewardlevel_30 - 1)
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardlevel_30 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%' -- and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Sv_Rewardlevel_15 AND
                          (Sv_Rewardlevel_20 - 1)
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardlevel_20 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) < Sv_Rewardlevel_15
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardlevel_15 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_40
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      0
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_30
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      (Sv_Rewardlevel_40 - Sv_Rewardlevel_30)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_20
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      (Sv_Rewardlevel_30 - Sv_Rewardlevel_20)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%' --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Sv_Rewardlevel_15
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Sv_Rewardlevel_15
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) --AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                          AND SUM(CASE
                                       WHEN Ty.Name IN ('AEO Connected Points',
                                                        'AEO Connected Bonus Points',
                                                        'AEO Visa Card Points',
                                                        'AEO Customer Service Points',
                                                        'Adjustment Points',
                                                        'Adjustment Bonus Points',
                                                        'Bonus Points')
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) > 1000 THEN
                      (Floor(SUM(CASE
                                      WHEN Ty.Name IN ('AEO Connected Points',
                                                       'AEO Connected Bonus Points',
                                                       'AEO Visa Card Points',
                                                       'AEO Customer Service Points',
                                                       'Adjustment Points',
                                                       'Adjustment Bonus Points',
                                                       'Bonus Points')
                                           AND Pts.Expirationdate > p_Processdate THEN
                                       Pts.Points
                                      ELSE
                                       0
                                 END) / 1000) * 1000) + 1000 -
                      SUM(CASE
                               WHEN Ty.Name IN ('AEO Connected Points',
                                                'AEO Connected Bonus Points',
                                                'AEO Visa Card Points',
                                                'AEO Customer Service Points',
                                                'Adjustment Points',
                                                'Adjustment Bonus Points',
                                                'Bonus Points')
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) -- AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                          AND SUM(CASE
                                       WHEN Ty.Name IN ('AEO Connected Points',
                                                        'AEO Connected Bonus Points',
                                                        'AEO Visa Card Points',
                                                        'AEO Customer Service Points',
                                                        'Adjustment Points',
                                                        'Adjustment Bonus Points',
                                                        'Bonus Points')
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) < 1000 THEN
                      1000 - SUM(CASE
                                      WHEN Ty.Name IN ('AEO Connected Points',
                                                       'AEO Connected Bonus Points',
                                                       'AEO Visa Card Points',
                                                       'AEO Customer Service Points',
                                                       'Adjustment Points',
                                                       'Adjustment Bonus Points',
                                                       'Bonus Points')
                                           AND Pts.Expirationdate > p_Processdate THEN
                                       Pts.Points
                                      ELSE
                                       0
                                 END)
                END AS Pointstonextreward,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Bra Points')
                                       AND -- AEO-70 removed Bra Adjustment points
                                       Pts.Expirationdate > p_Processdate
                                       AND Pts.Transactiontype IN (1, 2, 4) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Bracurrentpurchased,
                Nvl(Sv_Bratotalfreemailed, 0) AS Sv_Bratotalfreemailed,
                Sv_Bralastfreemailed AS Sv_Bralastfreemailed,
                Nvl(Ats_Memberbrand.Brandae, 0) AS Brandae,
                Nvl(Ats_Memberbrand.Brandaerie, 0) AS Brandaerie,
                Nvl(Ats_Memberbrand.Brandkids, 0) AS Brandkids,
                Greatest(Round(SUM(CASE
                                        WHEN Ty.Name IN ('Bra Points')
                                             AND -- AEO-70 removed Bra Adjustment points
                                             Pts.Expirationdate > p_Processdate
                                             AND Pts.Transactiontype IN (1, 2, 4) THEN
                                         Pts.Points
                                        ELSE
                                         0
                                   END) / Sv_Fulfillmentthreshold,
                               0),
                         0) AS Bracurrentfreeearned,
                Decode(To_Number(Nvl(Md.a_Employeecode, 0)), 1, 0, 1) AS a_Braemployeemultiplier,
                Decode(Md.a_Addressmailable, 1, 1, 0) a_Bramailablemultiplier,
                Greatest(SUM(CASE
                                  WHEN Ty.Name NOT LIKE 'Jean%'
                                       AND Ty.Name NOT LIKE 'Bra%'
                                      --and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                       AND Pts.Transactiondate <
                                       Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                       AND Pts.Expirationdate > p_Processdate THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Ttl_Points,
                CASE
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      SUM(CASE
                               WHEN Ty.Name NOT LIKE 'Jean%'
                                    AND Ty.Name NOT LIKE 'Bra%'
                                    AND Trunc(Pts.Pointawarddate) BETWEEN
                                    Trunc(p_Processdate, 'Q') AND Trunc(p_Processdate - 1)
                                   -- AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) THEN
                      SUM(CASE
                               WHEN Ty.Name NOT LIKE 'Jean%'
                                    AND Ty.Name NOT LIKE 'Bra%'
                                    AND Trunc(Pts.Pointawarddate) BETWEEN
                                    Trunc(v_Transactionstartdate) AND
                                    Trunc(p_Processdate - 1)
                                   --AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                    AND Pts.Transactiondate <
                                    Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                END AS Priorday_Points,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Bra Points',
                                                   'Bra Adjustment Points',
                                                   'Bra Legacy Points',
                                                   'Bra Redemptions',
                                                   'Bra Employee Points')
                                       AND Pts.Transactiondate >
                                       To_Date('1/1/2009', 'mm/dd/yyyy')
                                       AND Pts.Transactiontype IN (1, 2) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Bralifetimebalance,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Bra Points',
                                                   'Bra Adjustment Points',
                                                   'Bra Redemptions',
                                                   'Bra Employee Points')
                                       AND Pts.Transactiondate >
                                       Add_Months(Trunc(p_Processdate, 'Q'), -12)
                                       AND Pts.Transactiontype IN (1, 2) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Brarollingbalance,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Jean Points', 'Jean Legacy Points')
                                      -- AEO-478 Begin
                                       AND Pts.Transactiondate >=
                                       To_Date('10/01/2015', 'mm/dd/yyyy') THEN
                                  /* AND Pts.Transactiondate >
                                  To_Date('1/1/2009', 'mm/dd/yyyy') THEN*/
                                   Pts.Points
                             -- AEO-478 End
                                  ELSE
                                   0
                             END),
                         0) AS Jeanslifetimebalance,
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('Jean Points')
                                      -- AEO-478 Begin
                                       AND Pts.Transactiondate >=
                                       To_Date('10/01/2015', 'mm/dd/yyyy')
                                       AND Pts.Transactiondate >
                                      -- AEO-478 end
                                       Add_Months(Trunc(p_Processdate, 'Q'), -12) THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Jeansrollingbalance,
                1 AS Statuscode,
                SYSDATE AS Createdate,
                SYSDATE AS Updatedate
               FROM   Lw_Pointtransaction Pts,
                      Lw_Virtualcard Vc,
                      Lw_Pointtype Ty,
                      Ats_Memberdetails Md,
                      (SELECT CASE
                                   WHEN SUM(CASE --PI25722 FLAG FIX BEGINS HERE--
                                                 WHEN b.a_Shortbrandname = 'AE' THEN
                                                  1
                                                 ELSE
                                                  0
                                            END) >= 1 THEN
                                    1
                                   ELSE
                                    0
                              END AS Brandae,
                              CASE
                                   WHEN SUM(CASE
                                                 WHEN b.a_Shortbrandname = 'aerie' THEN
                                                  1
                                                 ELSE
                                                  0
                                            END) >= 1 THEN
                                    1
                                   ELSE
                                    0
                              END AS Brandaerie,
                              CASE
                                   WHEN SUM(CASE
                                                 WHEN b.a_Shortbrandname = '77kids' THEN
                                                  1
                                                 ELSE
                                                  0
                                            END) >= 1 THEN
                                    1
                                   ELSE
                                    0
                              END AS Brandkids, --PI25722 FLAG FIX ENDS HERE--
                              Mb.a_Ipcode
                       FROM   Ats_Memberbrand Mb, Ats_Refbrand b
                       WHERE  Mb.a_Brandid = b.a_Brandid
                       GROUP  BY Mb.a_Ipcode) Ats_Memberbrand,
                      (SELECT SUM(CASE
                                       WHEN Rd.Name LIKE 'Bra%' THEN
                                        1
                                       ELSE
                                        0
                                  END) AS Sv_Bratotalfreemailed,
                              MAX(CASE
                                       WHEN Rd.Name LIKE 'Bra%' THEN
                                        Mr.Dateissued
                                  END) AS Sv_Bralastfreemailed,
                              Mr.Memberid
                       FROM   Lw_Memberrewards Mr
                       INNER  JOIN Lw_Rewardsdef Rd
                       ON     Mr.Rewarddefid = Rd.Id
                       GROUP  BY Mr.Memberid) Lw_Memberrewards,
                      (SELECT /*+ cardinality ( x 1 ) */
                        MAX(CASE
                                 WHEN x.Name = '40% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_40,
                        MAX(CASE
                                 WHEN x.Name = '30% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_30,
                        MAX(CASE
                                 WHEN x.Name = '20% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_20,
                        MAX(CASE
                                 WHEN x.Name = '15% - Reward' THEN
                                  x.Howmanypointstoearn
                                 ELSE
                                  NULL
                            END) AS Sv_Rewardlevel_15,
                        --  '10' AS Sv_Rewardname_10,
                        '15%' AS Sv_Rewardname_15,
                        '20%' AS Sv_Rewardname_20,
                        '30%' AS Sv_Rewardname_30,
                        '40%' AS Sv_Rewardname_40
                       FROM   Lw_Rewardsdef x) Lw_Rewardsdef,
                      (SELECT /*+ cardinality ( x 1 ) */
                        To_Number(To_Char(x.Value)) AS Sv_Fulfillmentthreshold
                       FROM   Lw_Clientconfiguration x
                       WHERE  x.Key = 'BraPromoFulFillmentThreshold') Lw_Clientconfiguration
               WHERE  Pts.Vckey = Vc.Vckey
                      AND Pts.Pointtypeid = Ty.Pointtypeid
                      AND Vc.Ipcode = Md.a_Ipcode
                      AND Md.a_Ipcode = Ats_Memberbrand.a_Ipcode(+)
                      AND Md.a_Ipcode = Lw_Memberrewards.Memberid(+)
                      AND Transactiontype IN (1, 2, 4)
                      AND
                      Pts.Transactiondate >= To_Date('1/1/2009', 'mm/dd/yyyy')
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               GROUP  BY Vc.Ipcode,
                         Md.a_Extendedplaycode,
                         Md.a_Employeecode,
                         Md.a_Addressmailable,
                         Ats_Memberbrand.Brandae,
                         Ats_Memberbrand.Brandaerie,
                         Ats_Memberbrand.Brandkids,
                         Sv_Rewardlevel_40,
                         Sv_Rewardlevel_30,
                         Sv_Rewardlevel_20,
                         Sv_Rewardlevel_15,
                         --      Sv_Rewardname_10,
                         Sv_Rewardname_15,
                         Sv_Rewardname_20,
                         Sv_Rewardname_30,
                         Sv_Rewardname_40,
                         Sv_Bratotalfreemailed,
                         Sv_Bralastfreemailed,
                         Sv_Fulfillmentthreshold
               --   Pts.Transactiondate
               HAVING SUM(CASE
                    WHEN
                    --      Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                     Pts.Transactiondate <
                     Add_Months(Trunc(p_Processdate, 'Q'), 3)
                     AND Pts.Expirationdate > p_Processdate THEN
                     Pts.Points
               --        WHEN Trunc(Pts.Transactiondate, 'Q') =
               --             Trunc(p_ProcessDate, 'Q') THEN
               --       Pts.Points
               --      WHEN Trunc(Pts.Transactiondate, 'Q') =
               --           Add_Months(Trunc(p_ProcessDate, 'Q'), -3) THEN
               --       Pts.Points
                    ELSE
                     0
               END) > 0;
          COMMIT;
          /* log end of job */
          Utility_Pkg.Log_Job(p_Job              => v_Dap_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => 'DAP-' || v_Jobname);
          v_Endtime := SYSDATE;
          /* log end of job */
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => 'Stage-' || v_Jobname);
          OPEN Retval FOR
               SELECT v_Dap_Log_Id FROM Dual;
     END Calculatepointbalances2;

     --Changes for PI30364 end here   -- SCJ
     /********************************************************************
     ********************************************************************/
     PROCEDURE Processbrapromoredemptions IS
     BEGIN
          -- since the AE_CUSTOMPROMO_REDEMPTIONS is a global temp table, we do not need to clear the data from the last run
          -- because its automatically deleted once there is a commit;
          -- insert data into a global temp table
          -- it should be faster to search the work table once to pull all of the data to insert into
          -- the redemption table and then delete from the work table based on the dtldetailid since
          -- that field is indexed.
          INSERT INTO Ae_Custompromo_Redemptions
               (Dtldetailid,
                Ipcode,
                Redemptionamount,
                Reasoncode,
                Redemptiondate)
               SELECT Dwt.Dtldetailid,
                      Dwt.Ipcode,
                      1,
                      Dwt.Dtlreasoncode,
                      Dwt.Txndate
               FROM   Ae_Custompromo_Details_Wrktble Dwt
               WHERE  (Dwt.Dtlsaleamount = 0 OR Dwt.Dtlsaleamount = .01)
                      AND (Dwt.Dtlreasoncode IS NOT NULL OR
                      Length(Dwt.Dtlreasoncode) > 0);
          -- remove unknown bra reason codes
          DELETE FROM Ae_Custompromo_Redemptions Red
          WHERE  NOT EXISTS (SELECT 1
                  FROM   Ae_Custompromo_Reasoncodes Rc
                  WHERE  Red.Reasoncode = Rc.Reasoncode);
          --  ATS_MEMBERBRAPROMOCERTREDEEM is the table for holding bra redemptions
          INSERT INTO Ats_Memberbrapromocertredeem
               (a_Rowkey,
                a_Parentrowkey,
                a_Ipcode,
                a_Redemptionamount,
                a_Reasoncode,
                a_Redemptiondate,
                Statuscode,
                Createdate,
                Updatedate)
               SELECT Seq_Rowkey.Nextval,
                      -1,
                      Rd.Ipcode,
                      Rd.Redemptionamount,
                      Rd.Reasoncode,
                      Rd.Redemptiondate,
                      1,
                      SYSDATE,
                      SYSDATE
               FROM   Ae_Custompromo_Redemptions Rd;
          -- this will delete all of the redemption items from the work table so we dont have to worry about filtering them out later when pulling the counts
          DELETE FROM Ae_Custompromo_Details_Wrktble Dwt
          WHERE  EXISTS (SELECT 1
                  FROM   Ae_Custompromo_Redemptions Rd
                  WHERE  Rd.Dtldetailid = Dwt.Dtldetailid);
     END Processbrapromoredemptions;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Updateemployeecode(p_Dummy VARCHAR2,
                                  Retval  IN OUT Rcursor) AS
          v_Logsource        VARCHAR2(256) := 'AEJobs.UpdateEmployeeCode';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AEJobs.UpdateEmployeeCode';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Messagespassed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'UpdateEmployeeCode';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
     BEGIN
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          /* log start of job (lw logging)*/
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
          ---------------------------
          -- Update Employee flag
          ---------------------------
          DECLARE
               CURSOR Get_Data IS
                    SELECT DISTINCT v.Ipcode AS Ipcode
                    FROM   Ats_Historytxndetail Hst
                    INNER  JOIN Lw_Virtualcard v
                    ON     Hst.a_Txnloyaltyid = v.Loyaltyidnumber
                    WHERE  Hst.a_Txnemployeeid IS NOT NULL
                           AND Trunc(Hst.a_Txndate, 'Q') = Trunc(SYSDATE, 'Q')
                           AND Hst.a_Ipcode IS NOT NULL;
               TYPE t_Tab IS TABLE OF Get_Data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
               v_Tbl t_Tab; ---<------ our arry object
          BEGIN
               OPEN Get_Data;
               LOOP
                    FETCH Get_Data BULK COLLECT
                         INTO v_Tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
                    FORALL i IN 1 .. v_Tbl.Count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
                         UPDATE Ats_Memberdetails Mds
                         SET    Mds.a_Employeecode = 1,
                                Mds.a_Changedby    = 'UpdateEmployeeCode'
                         WHERE  Mds.a_Ipcode = v_Tbl(i).Ipcode
                                AND (Mds.a_Employeecode IS NULL OR
                                Mds.a_Employeecode = 0 OR
                                Mds.a_Employeecode = 2);
                    COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
                    EXIT WHEN Get_Data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
               END LOOP;
               COMMIT;
               IF Get_Data%ISOPEN
               THEN
                    --<--- dont forget to close cursor since we manually opened it.
                    CLOSE Get_Data;
               END IF;
               v_Endtime := SYSDATE;
               /* log end of job (lw logging)*/
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
          END;
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
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure UpdateEmployeeCode';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>AEJobs</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>UpdateEmployeeCode</proc>' ||
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
     END Updateemployeecode;

     /********************************************************************
     ********************************************************************/
     PROCEDURE Exirebrapoints(p_Dummy VARCHAR2,
                              Retval  IN OUT Rcursor) AS
          /*
            1.  We need to be able to run this procedure after Quarterly Rewards are run on 1/1 every YEAR
                that will expire all bra points (purchases and returns) by changing the expiration date to 12/31 of PREVIOUS year.
            2.  The job should also update the expiration date for all bra purchases between 12/16 of Previous year and 12/31 Previous year to 1/22 of Current year,
                so that these points can be included for the 1/15 of Current year bra fulfillment file.
                This is to permit purchases made in the last 2 weeks of the year to be included for qualifying purchases during the 2-week holding period.
                After the 1/15 bra fulfillment process, all points from Previous year will be consumed and/or expired.
            3.  The job should also update the expiration date for all points with the point type of bra redemption to 1/1/2115.
                These points cannot expire, as they are used in a count of lifetime bra redemptions that is displayed in CS Portal (Member Account Summary - Total Free Bras Redeemed).
          */
     BEGIN
          DECLARE
               CURSOR Get_Data IS
                    SELECT Pt.Pointtransactionid,
                           CASE
                                WHEN Pe.Name <> 'Bra Redemptions'
                                     AND
                                     Pt.Transactiondate BETWEEN
                                     To_Date(To_Char('01/01/' ||
                                                     (To_Char(SYSDATE, 'yyyy') - 1) ||
                                                     ' 12:00:00 AM'),
                                             'mm/dd/yyyy HH:MI:SS AM') AND
                                     To_Date(To_Char('12/15/' ||
                                                     (To_Char(SYSDATE, 'yyyy') - 1) ||
                                                     ' 11:59:59 PM'),
                                             'mm/dd/yyyy HH:MI:SS PM') THEN
                                 To_Date(To_Char('12/31/' ||
                                                 (To_Char(SYSDATE, 'yyyy') - 1)),
                                         'mm/dd/yyyy')
                                WHEN Pe.Name <> 'Bra Redemptions'
                                     AND
                                     Pt.Transactiondate BETWEEN
                                     To_Date(To_Char('12/16/' ||
                                                     (To_Char(SYSDATE, 'yyyy') - 1) ||
                                                     ' 12:00:00 AM'),
                                             'mm/dd/yyyy HH:MI:SS AM') AND
                                     To_Date(To_Char('12/31/' ||
                                                     (To_Char(SYSDATE, 'yyyy') - 1) ||
                                                     ' 11:59:59 PM'),
                                             'mm/dd/yyyy HH:MI:SS PM') THEN
                                 To_Date(To_Char('01/22/' ||
                                                 (To_Char(SYSDATE, 'yyyy'))),
                                         'mm/dd/yyyy')
                                WHEN Pe.Name = 'Bra Redemptions' THEN
                                 To_Date('01/01/2115', 'mm/dd/yyyy')
                           END AS Expirydate
                    FROM   Lw_Pointtransaction Pt
                    INNER  JOIN Lw_Pointevent Pe
                    ON     Pt.Pointeventid = Pe.Pointeventid
                    WHERE  Pe.Name LIKE 'Bra%'
                           AND Extract(YEAR FROM Pt.Transactiondate) =
                           Extract(YEAR FROM SYSDATE) - 1;
               TYPE t_Tab IS TABLE OF Get_Data%ROWTYPE;
               v_Tbl t_Tab;
          BEGIN
               OPEN Get_Data;
               LOOP
                    FETCH Get_Data BULK COLLECT
                         INTO v_Tbl LIMIT 100;
                    FORALL i IN 1 .. v_Tbl.Count
                         UPDATE Lw_Pointtransaction Lwpts
                         SET    Lwpts.Expirationdate = v_Tbl(i).Expirydate,
                                --    lwpts.last_dml_date  = sysdate  --// AEO-74 Upgrade 4.5 changes here -----------SCJ
                                Lwpts.Createdate = SYSDATE
                         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         WHERE  Lwpts.Pointtransactionid = v_Tbl(i)
                               .Pointtransactionid;
                    COMMIT;
                    EXIT WHEN Get_Data%NOTFOUND;
               END LOOP;
               COMMIT;
               IF Get_Data%ISOPEN
               THEN
                    CLOSE Get_Data;
               END IF;
          END;
     END Exirebrapoints;

     PROCEDURE Matchpostalcodes(p_Dummy VARCHAR2,
                                Retval  IN OUT Rcursor) AS
          /*
           Whenever called resolves the conflict between primary postal codes and zip postal codes
          */
     BEGIN
          DECLARE
               CURSOR Get_Data IS
                    SELECT Lm.Ipcode, Md.a_Ziporpostalcode
                    FROM   Lw_Loyaltymember Lm
                    INNER  JOIN Ats_Memberdetails Md
                    ON     Lm.Ipcode = Md.a_Ipcode
                           AND Lm.Memberstatus <> 3
                    WHERE  Lm.Primarypostalcode <> Md.a_Ziporpostalcode;
               TYPE t_Tab IS TABLE OF Get_Data%ROWTYPE;
               v_Tbl t_Tab;
          BEGIN
               OPEN Get_Data;
               LOOP
                    FETCH Get_Data BULK COLLECT
                         INTO v_Tbl LIMIT 100;
                    FORALL i IN 1 .. v_Tbl.Count
                         UPDATE Lw_Loyaltymember Lms
                         SET    Lms.Primarypostalcode = v_Tbl(i)
                                                        .a_Ziporpostalcode,
                                Lms.Changedby         = 'PI 13825 ? Postal Code'
                         WHERE  Lms.Ipcode = v_Tbl(i).Ipcode;
                    COMMIT;
                    EXIT WHEN Get_Data%NOTFOUND;
               END LOOP;
               COMMIT;
               IF Get_Data%ISOPEN
               THEN
                    CLOSE Get_Data;
               END IF;
          END;
     END Matchpostalcodes;

     PROCEDURE Aepointheaderload(p_Dummy       VARCHAR2,
                                 p_Processdate DATE) IS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AEpointheader';
          INSERT INTO Aepointheader
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
               SELECT /*+ parallel(pts 8) */
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(' ') AS Customerkey,
                Vc.Loyaltyidnumber,
                MAX(Bl.a_Totalpoints) AS Totalpoints,
                MAX(Bl.a_Rewardlevel) AS Rewardlevel,
                CASE
                     WHEN MAX(Bl.a_Totalpoints) > 499 THEN
                      0
                     ELSE
                      MAX(Bl.a_Pointstonextreward)
                END AS Pointstonextreward,
                MAX(Bl.a_Basepoints) AS Basepoints,
                MAX(Bl.a_Bonuspoints) AS Bonuspoints
               FROM   Ats_Memberpointbalances Bl
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Bl.a_Ipcode
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Ipcode = Bl.a_Ipcode
                      AND Vc.Isprimary = 1
               INNER  JOIN Lw_Pointtransaction Pts
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Pts.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               GROUP  BY Lm.Ipcode, Vc.Loyaltyidnumber;
          COMMIT;
     END Aepointheaderload;

     PROCEDURE Memberpointsfullload(p_Dummy       VARCHAR2,
                                    p_Processdate DATE,
                                    Retval        IN OUT Rcursor) IS
          Pdummy VARCHAR2(2);
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table ae_memberpointsonetime';
          Aepointheaderload(Pdummy, p_Processdate);
          INSERT INTO Ae_Memberpointsonetime
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(SYSDATE - 1, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheader Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheader Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheader Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheader Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsfullload;

     PROCEDURE Aepointheaderdeltaload(p_Dummy       VARCHAR2,
                                      p_Processdate DATE) IS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta';
          INSERT INTO Aepointheaderdelta
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
               SELECT /*+ parallel(pts 8) */
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(' ') AS Customerkey,
                Vc.Loyaltyidnumber,
                MAX(Bl.a_Totalpoints) AS Totalpoints,
                MAX(Bl.a_Rewardlevel) AS Rewardlevel,
                CASE
                     WHEN MAX(Bl.a_Totalpoints) > 499 THEN
                      0
                     ELSE
                      MAX(Bl.a_Pointstonextreward)
                END AS Pointstonextreward,
                MAX(Bl.a_Basepoints) AS Basepoints,
                MAX(Bl.a_Bonuspoints) AS Bonuspoints
               FROM   Ats_Memberpointbalances Bl
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Bl.a_Ipcode
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Ipcode = Bl.a_Ipcode
                      AND Vc.Isprimary = 1
               INNER  JOIN Lw_Pointtransaction Pts
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Pts.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Bl.a_Priordaytotalpoints <> Bl.a_Totalpoints
               GROUP  BY Lm.Ipcode, Vc.Loyaltyidnumber;
          COMMIT;
     END Aepointheaderdeltaload;

     PROCEDURE Memberpointsdeltafullload(p_Dummy       VARCHAR2,
                                         p_Processdate DATE,
                                         Retval        IN OUT Rcursor) AS
          Pdummy       VARCHAR2(2);
          Pprocessdate DATE := p_Processdate;
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA';
          Aepointheaderdeltaload(Pdummy, Pprocessdate);
          INSERT INTO Ae_Memberpointsdelta
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(Pprocessdate - 1, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdeltafullload;

     PROCEDURE Aepointheaderdeltaload2(p_Dummy       VARCHAR2,
                                       p_Processdate DATE) IS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta_New';
          INSERT INTO Aepointheaderdelta_New
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
               SELECT /*+ parallel(pts 8) */
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(' ') AS Customerkey,
                Vc.Loyaltyidnumber,
                MAX(Bl.a_Totalpoints) AS Totalpoints,
                MAX(Bl.a_Rewardlevel) AS Rewardlevel,
                CASE
                     WHEN MAX(Bl.a_Totalpoints) > 499 THEN
                      0
                     ELSE
                      MAX(Bl.a_Pointstonextreward)
                END AS Pointstonextreward,
                MAX(Bl.a_Basepoints) AS Basepoints,
                MAX(Bl.a_Bonuspoints) AS Bonuspoints
               FROM   Ae_Memberpointbalances_010114 Bl
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Bl.a_Ipcode
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Ipcode = Bl.a_Ipcode
                      AND Vc.Isprimary = 1
               INNER  JOIN Lw_Pointtransaction Pts
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Pts.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pts.Transactiondate) < Trunc(p_Processdate)
                      AND Bl.a_Priordaytotalpoints <> Bl.a_Totalpoints
               GROUP  BY Lm.Ipcode, Vc.Loyaltyidnumber;
          COMMIT;
     END Aepointheaderdeltaload2;

     PROCEDURE Memberpointsdeltafullload2(p_Dummy       VARCHAR2,
                                          p_Processdate DATE,
                                          Retval        IN OUT Rcursor) AS
          Pdummy       VARCHAR2(2);
          Pprocessdate DATE := p_Processdate;
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA_New';
          -- AEPOINTHEADERDELTALOAD2(pdummy,pProcessDate);
          INSERT INTO Ae_Memberpointsdelta_New
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(Pprocessdate - 1, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta_New Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pt.Transactiondate) < Trunc(p_Processdate)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pt.Transactiondate) < Trunc(p_Processdate)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pt.Transactiondate) < Trunc(p_Processdate)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdeltafullload2;

     PROCEDURE Aepointheaderdeltaload3(p_Dummy       VARCHAR2,
                                       p_Processdate DATE) IS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta';
          INSERT INTO Aepointheaderdelta
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
               SELECT /*+ parallel(pts 8) */
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(' ') AS Customerkey,
                Vc.Loyaltyidnumber,
                MAX(Bl.a_Totalpoints) AS Totalpoints,
                MAX(Bl.a_Rewardlevel) AS Rewardlevel,
                CASE
                     WHEN MAX(Bl.a_Totalpoints) > 499 THEN
                      0
                     ELSE
                      MAX(Bl.a_Pointstonextreward)
                END AS Pointstonextreward,
                MAX(Bl.a_Basepoints) AS Basepoints,
                MAX(Bl.a_Bonuspoints) AS Bonuspoints
               FROM   Ats_Memberpointbalances Bl
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Bl.a_Ipcode
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Ipcode = Bl.a_Ipcode
                      AND Vc.Isprimary = 1
               INNER  JOIN Lw_Pointtransaction Pts
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Pts.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pts.Transactiondate) < Trunc(p_Processdate)
                      AND Bl.a_Priordaytotalpoints <> Bl.a_Totalpoints
               GROUP  BY Lm.Ipcode, Vc.Loyaltyidnumber;
          COMMIT;
     END Aepointheaderdeltaload3;

     PROCEDURE Memberpointsdeltafullload3(p_Dummy       VARCHAR2,
                                          p_Processdate DATE,
                                          Retval        IN OUT Rcursor) AS
          Pdummy       VARCHAR2(2);
          Pprocessdate DATE := p_Processdate;
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA';
          -- AEPOINTHEADERDELTALOAD3(pdummy,pProcessDate);
          INSERT INTO Ae_Memberpointsdelta
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(Pprocessdate - 1, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pt.Transactiondate) < Trunc(p_Processdate)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pt.Transactiondate) < Trunc(p_Processdate)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Trunc(Pt.Transactiondate) < Trunc(p_Processdate)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdeltafullload3;

     PROCEDURE Aepointheaderdeltaloadqr(p_Dummy       VARCHAR2,
                                        p_Processdate DATE) IS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta';
          INSERT INTO Aepointheaderdelta
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
               SELECT /*+ parallel(pts 8) */
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(' ') AS Customerkey,
                Vc.Loyaltyidnumber,
                MAX(Bl.a_Totalpoints) AS Totalpoints,
                MAX(Bl.a_Rewardlevel) AS Rewardlevel,
                CASE
                     WHEN MAX(Bl.a_Totalpoints) > 499 THEN
                      0
                     ELSE
                      MAX(Bl.a_Pointstonextreward)
                END AS Pointstonextreward,
                MAX(Bl.a_Basepoints) AS Basepoints,
                MAX(Bl.a_Bonuspoints) AS Bonuspoints
               FROM   Ats_Memberpointbalances Bl
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Bl.a_Ipcode
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Ipcode = Bl.a_Ipcode
                      AND Vc.Isprimary = 1
               INNER  JOIN Lw_Pointtransaction Pts
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Pts.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Bl.a_Priordaytotalpoints <> Bl.a_Totalpoints
               GROUP  BY Lm.Ipcode, Vc.Loyaltyidnumber;
          COMMIT;
     END Aepointheaderdeltaloadqr;

     PROCEDURE Memberpointsdeltafullloadqr(p_Dummy       VARCHAR2,
                                           p_Processdate DATE,
                                           Retval        IN OUT Rcursor) AS
          Pdummy       VARCHAR2(2);
          Pprocessdate DATE := p_Processdate;
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA';
          Aepointheaderdeltaloadqr(Pdummy, Pprocessdate);
          INSERT INTO Ae_Memberpointsdelta
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(Pprocessdate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Lw_Pointtransaction Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdeltafullloadqr;

     /*PI 29896   changes begin here - scj */
     PROCEDURE Aepointheaderdeltaload_New(p_Dummy               VARCHAR2,
                                          p_Processdate         DATE,
                                          p_Memberfilestarttime DATE) IS
          Mv_Rewardlevel_40 NUMBER := 500;
          Mv_Rewardlevel_30 NUMBER := 350;
          Mv_Rewardlevel_20 NUMBER := 200;
          Mv_Rewardlevel_15 NUMBER := 100;
          Mv_Rewardname_15  VARCHAR2(10) := '15%';
          Mv_Rewardname_20  VARCHAR2(10) := '20%';
          Mv_Rewardname_30  VARCHAR2(10) := '30%';
          Mv_Rewardname_40  VARCHAR2(10) := '40%';
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta_NEW';
          INSERT INTO Aepointheaderdelta_New
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
               SELECT /*+ parallel(pts 8) */
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(Vc2.Linkkey) AS Customerkey,
                Vc2.Loyaltyidnumber,
                /*            max(bl.a_totalpoints) AS TotalPoints, */
                Greatest(SUM(CASE
                                  WHEN Ty.Name NOT LIKE 'Jean%'
                                       AND Ty.Name NOT LIKE 'Bra%'
                                       AND Trunc(Pts.Transactiondate, 'Q') =
                                       Trunc(p_Processdate, 'Q') THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Totalpoints,
                /*            max(bl.a_rewardlevel) AS RewardLevel, */
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_40 THEN
                      Mv_Rewardname_40
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_30 THEN
                      Mv_Rewardname_30
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_20 THEN
                      Mv_Rewardname_20
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_15 THEN
                      Mv_Rewardname_15
                END AS Rewardlevel,
                /*            CASE WHEN max(bl.a_totalpoints)  > 499  THEN  0
                else max(bl.a_pointstonextreward)
                       END AS PointsToNextReward, */
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) > Mv_Rewardlevel_40 THEN
                     /*
                     SUM(CASE
                         WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                         Pts.Points
                         ELSE
                         0
                         END)- Mv_Rewardlevel_40 */
                      0
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Mv_Rewardlevel_30 AND
                          (Mv_Rewardlevel_40 - 1) THEN
                      Mv_Rewardlevel_40 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Mv_Rewardlevel_20 AND
                          (Mv_Rewardlevel_30 - 1) THEN
                      Mv_Rewardlevel_30 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Mv_Rewardlevel_15 AND
                          (Mv_Rewardlevel_20 - 1) THEN
                      Mv_Rewardlevel_20 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) < Mv_Rewardlevel_15 THEN
                      Mv_Rewardlevel_15 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                                        Trunc(p_Processdate, 'Q') THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_40 THEN
                      0
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_30 THEN
                      (Mv_Rewardlevel_40 - Mv_Rewardlevel_30)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_20 THEN
                      (Mv_Rewardlevel_30 - Mv_Rewardlevel_20)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate, 'Q') =
                                        Trunc(p_Processdate, 'Q') THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_15 THEN
                      Mv_Rewardlevel_15
                END AS Pointstonextreward,
                /*            max(bl.a_basepoints) AS BasePoints, */
                SUM(CASE
                         WHEN Ty.Name IN ('Basic Points', 'Adjustment Points')
                              AND Trunc(Pts.Transactiondate, 'Q') =
                              Trunc(p_Processdate, 'Q') THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Basepoints,
                /*            max(bl.a_bonuspoints) AS BonusPoints */
                SUM(CASE
                         WHEN Ty.Name IN ('CS Points',
                                          'Bonus Points',
                                          'Adjustment Bonus Points')
                              AND Trunc(Pts.Transactiondate, 'Q') =
                              Trunc(p_Processdate, 'Q') THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Bonuspoints
               FROM   Ae_Currentpointtransaction2 Pts
               --Changes for PI30280 begin here
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Vckey = Pts.Vckey --AND vc.isprimary=1
               INNER  JOIN Lw_Pointtype Ty
               ON     Ty.Pointtypeid = Pts.Pointtypeid
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Vc.Ipcode
               INNER  JOIN (SELECT *
                            FROM   Lw_Virtualcard Vc1
                            WHERE  Vc1.Isprimary = 1) Vc2
               ON     Vc.Ipcode = Vc2.Ipcode
               --  INNER JOIN ATS_MEMBERDETAILS MD ON MD.A_IPCODE = VC.IPCODE
               --  INNER JOIN (select distinct(pta.vckey) from bp_ae.AE_CurrentPointTransaction2 pta where pta.pointawarddate >= p_MemberfileStarttime )ptb  on pts.vckey = ptb.vckey
               INNER  JOIN (SELECT DISTINCT (Vc5.Ipcode)
                            FROM   Lw_Virtualcard Vc5
                            INNER  JOIN (SELECT DISTINCT (Pta.Vckey)
                                        FROM   Ae_Currentpointtransaction2 Pta
                                        WHERE  Pta.Pointawarddate >=
                                               p_Memberfilestarttime) Ptb
                            ON     Vc5.Vckey = Ptb.Vckey) Vc6
               ON     Vc.Ipcode = Vc6.Ipcode
               --Changes for PI30280 end here
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Pts.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               -- and pts.pointawarddate >= p_ProcessDate -1
               -- AND bl.a_priordaytotalpoints <> bl.a_totalpoints
               GROUP  BY Lm.Ipcode, Vc2.Loyaltyidnumber;
          COMMIT;
     END Aepointheaderdeltaload_New;

     PROCEDURE Memberpointsdeltafullload_New(p_Dummy       VARCHAR2,
                                             p_Processdate DATE,
                                             Retval        IN OUT Rcursor) AS
          Pdummy                VARCHAR2(2);
          Pprocessdate          DATE := p_Processdate;
          v_Sql1                VARCHAR2(1000); -- := 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where pt.transactiondate > Add_Months(trunc(p_ProcessDate, ''Q''), -3)';
          v_Sql2                VARCHAR2(1000); -- := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'') into v_MemberfileStarttime from lw_clientconfiguration cc where cc.key = ''LastAEMemberPointsSent''';
          v_Memberfilestarttime DATE;
     BEGIN
          EXECUTE IMMEDIATE 'Drop Table AE_CurrentPointTransaction2';
          --v_sql1 := ' Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where pt.transactiondate > Add_Months(trunc(to_date('''||to_char(p_ProcessDate,'dd-mm-yy')||''',''DD-MM-YY''),''Q''),-3)';
          v_Sql1 := 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where pt.transactiondate > Add_Months(trunc(sysdate, ''Q''), -3)';
          v_Sql2 := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'')  from lw_clientconfiguration cc where cc.key = ''LastAEMemberPointsSent''';
          EXECUTE IMMEDIATE v_Sql1;
          EXECUTE IMMEDIATE v_Sql2
               INTO v_Memberfilestarttime;
          Updatelastaememberpointssent(Pdummy);
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA_NEW';
          Aepointheaderdeltaload_New(Pdummy,
                                     Pprocessdate,
                                     v_Memberfilestarttime);
          INSERT INTO Ae_Memberpointsdelta_New
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(Pprocessdate - 1, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta_New Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdeltafullload_New;

     /*PI 29896   changes end here - scj */
     PROCEDURE Memberpointsdeltafullload_Qr(p_Dummy       VARCHAR2,
                                            p_Processdate DATE,
                                            Retval        IN OUT Rcursor) AS
          Pdummy                VARCHAR2(2);
          Pprocessdate          DATE := p_Processdate;
          v_Sql1                VARCHAR2(1000); -- := 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where pt.transactiondate > Add_Months(trunc(p_ProcessDate, ''Q''), -3)';
          v_Sql2                VARCHAR2(1000); -- := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'') into v_MemberfileStarttime from lw_clientconfiguration cc where cc.key = ''LastAEMemberPointsSent''';
          v_Memberfilestarttime DATE;
     BEGIN
          EXECUTE IMMEDIATE 'Drop Table AE_CurrentPointTransaction2';
          --v_sql1 := ' Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where pt.transactiondate > Add_Months(trunc(to_date('''||to_char(p_ProcessDate,'dd-mm-yy')||''',''DD-MM-YY''),''Q''),-3)';
          v_Sql1 := 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where trunc(pt.transactiondate) >= Add_Months(trunc(sysdate, ''Q''), -4)';
          v_Sql2 := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'')  from lw_clientconfiguration cc where cc.key = ''LastAEMemberPointsSent''';
          EXECUTE IMMEDIATE v_Sql1;
          EXECUTE IMMEDIATE v_Sql2
               INTO v_Memberfilestarttime;
          Updatelastaememberpointssent(Pdummy);
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA_NEW';
          Aepointheaderdeltaload_New(Pdummy,
                                     Pprocessdate,
                                     v_Memberfilestarttime);
          INSERT INTO Ae_Memberpointsdelta_New
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      To_Char(Pprocessdate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta_New Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('Basic Points', 'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_New Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Transactiondate >= Trunc(p_Processdate, 'Q')
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Q'), 3)
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdeltafullload_Qr;

     --Changes for PI30364 BEGIN here  -- SCJ
     PROCEDURE Headerrecord_Memberpoints(p_Dummy                VARCHAR2,
                                         p_Processdate          DATE,
                                         p_Memberfilestarttime  DATE,
                                         v_Transactionstartdate DATE) IS
          v_Dollarrewardlevel  NUMBER := 150;
          v_Dollarrewardcutoff NUMBER := 100;
          Mv_Rewardlevel_40    NUMBER := 500;
          Mv_Rewardlevel_30    NUMBER := 350;
          Mv_Rewardlevel_20    NUMBER := 200;
          Mv_Rewardlevel_15    NUMBER := 100;
          Mv_Rewardname_15     VARCHAR2(10) := '15%';
          Mv_Rewardname_20     VARCHAR2(10) := '20%';
          Mv_Rewardname_30     VARCHAR2(10) := '30%';
          Mv_Rewardname_40     VARCHAR2(10) := '40%';
          v_Sql1               VARCHAR2(1000);
     BEGIN
          v_Sql1 := ' Select to_char(value)  from lw_clientconfiguration cc where cc.key = ''DollarRewardsPoints''';
          EXECUTE IMMEDIATE v_Sql1
               INTO v_Dollarrewardlevel;
          EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta';
          INSERT INTO Aepointheaderdelta
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
--            SELECT /*+ parallel(pts 8) */   AEO 598 changes  here ---------------SCJ
               SELECT
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(Vc2.Linkkey) AS Customerkey,
                Vc2.Loyaltyidnumber,
                /*            max(bl.a_totalpoints) AS TotalPoints, */
                Greatest(SUM(CASE
                                  WHEN Ty.Name NOT LIKE 'Jean%'
                                       AND Ty.Name NOT LIKE 'Bra%'
                                       AND Trunc(Pts.Transactiondate) >=
                                       Trunc(v_Transactionstartdate)
                                       AND Pts.Transactiondate <
                                       Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                       AND Pts.Expirationdate > p_Processdate THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Totalpoints,
                --Rewardlevel
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_40
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardname_40
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Pts.Transactiondate >= v_Transactionstartdate
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_30
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardname_30
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_20
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardname_20
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) >= Mv_Rewardlevel_15
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardname_15
                /*  WHEN md.a_extendedplaycode  = 1 THEN
                NULLIF(to_char(floor(SUM(CASE             -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                              WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%'
                                   and Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
                                     AND Pts.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Mon'), 1)
                                    AND    Pts.Expirationdate > p_ProcessDate   THEN
                               Pts.Points
                              ELSE
                               0
                       END)/v_DollarRewardLevel) * 10),'0') */
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                          AND SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                       WHEN Ty.Name NOT LIKE 'Jean%'
                                            AND Ty.Name NOT LIKE 'Bra%'
                                            AND Trunc(Pts.Transactiondate) >=
                                            Trunc(v_Transactionstartdate)
                                            AND Pts.Transactiondate <
                                            Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) >= v_Dollarrewardlevel
                          AND To_Char(Floor(SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                                     WHEN Ty.Name NOT LIKE 'Jean%'
                                                          AND Ty.Name NOT LIKE 'Bra%'
                                                          AND Trunc(Pts.Transactiondate) >=
                                                          Trunc(v_Transactionstartdate)
                                                          AND Pts.Transactiondate <
                                                          Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                                          AND Pts.Expirationdate > p_Processdate THEN
                                                      Pts.Points
                                                     ELSE
                                                      0
                                                END) / v_Dollarrewardlevel) * 10) <=
                          v_Dollarrewardcutoff THEN
                      Nullif(To_Char(Floor(SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                                    WHEN Ty.Name NOT LIKE 'Jean%'
                                                         AND Ty.Name NOT LIKE 'Bra%'
                                                         AND Trunc(Pts.Transactiondate) >=
                                                         Trunc(v_Transactionstartdate)
                                                         AND Pts.Transactiondate <
                                                         Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                                         AND Pts.Expirationdate > p_Processdate THEN
                                                     Pts.Points
                                                    ELSE
                                                     0
                                               END) / v_Dollarrewardlevel) * 10),
                             '0')
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                          AND SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                       WHEN Ty.Name NOT LIKE 'Jean%'
                                            AND Ty.Name NOT LIKE 'Bra%'
                                            AND Trunc(Pts.Transactiondate) >=
                                            Trunc(v_Transactionstartdate)
                                            AND Pts.Transactiondate <
                                            Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) >= v_Dollarrewardlevel
                          AND To_Char(Floor(SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                                     WHEN Ty.Name NOT LIKE 'Jean%'
                                                          AND Ty.Name NOT LIKE 'Bra%'
                                                          AND Trunc(Pts.Transactiondate) >=
                                                          Trunc(v_Transactionstartdate)
                                                          AND Pts.Transactiondate <
                                                          Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                                          AND Pts.Expirationdate > p_Processdate THEN
                                                      Pts.Points
                                                     ELSE
                                                      0
                                                END) / v_Dollarrewardlevel) * 10) >
                          v_Dollarrewardcutoff THEN
                      To_Char(v_Dollarrewardcutoff)
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                          AND SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                       WHEN Ty.Name NOT LIKE 'Jean%'
                                            AND Ty.Name NOT LIKE 'Bra%'
                                            AND Trunc(Pts.Transactiondate) >=
                                            Trunc(v_Transactionstartdate)
                                            AND Pts.Transactiondate <
                                            Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) < v_Dollarrewardlevel THEN
                      NULL
                END AS Rewardlevel,
                --PointsToNextReward
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) > Mv_Rewardlevel_40
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      0
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Mv_Rewardlevel_30 AND
                          (Mv_Rewardlevel_40 - 1)
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardlevel_40 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate) >=
                                                        Trunc(v_Transactionstartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Mv_Rewardlevel_20 AND
                          (Mv_Rewardlevel_30 - 1)
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardlevel_30 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate) >=
                                                        Trunc(v_Transactionstartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) BETWEEN Mv_Rewardlevel_15 AND
                          (Mv_Rewardlevel_20 - 1)
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardlevel_20 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate) >=
                                                        Trunc(v_Transactionstartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) < Mv_Rewardlevel_15
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardlevel_15 - SUM(CASE
                                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                                        AND Ty.Name NOT LIKE 'Bra%'
                                                        AND Trunc(Pts.Transactiondate) >=
                                                        Trunc(v_Transactionstartdate)
                                                        AND Pts.Expirationdate > p_Processdate THEN
                                                    Pts.Points
                                                   ELSE
                                                    0
                                              END)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_40
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      0
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_30
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      (Mv_Rewardlevel_40 - Mv_Rewardlevel_30)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_20
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      (Mv_Rewardlevel_30 - Mv_Rewardlevel_20)
                     WHEN SUM(CASE
                                   WHEN Ty.Name NOT LIKE 'Jean%'
                                        AND Ty.Name NOT LIKE 'Bra%'
                                        AND Trunc(Pts.Transactiondate) >=
                                        Trunc(v_Transactionstartdate)
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) = Mv_Rewardlevel_15
                          AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0) THEN
                      Mv_Rewardlevel_15
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                          AND SUM(CASE
                                       WHEN Ty.Name NOT LIKE 'Jean%'
                                            AND Ty.Name NOT LIKE 'Bra%'
                                            AND Trunc(Pts.Transactiondate) >=
                                            Trunc(v_Transactionstartdate)
                                            AND Pts.Transactiondate <
                                            Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) >= 0 THEN
                      Checkdollarrewardlevel(SUM(CASE
                                                      WHEN Ty.Name NOT LIKE 'Jean%'
                                                           AND Ty.Name NOT LIKE 'Bra%'
                                                           AND Trunc(Pts.Transactiondate) >=
                                                           Trunc(v_Transactionstartdate)
                                                           AND Pts.Expirationdate > p_Processdate
                                                           AND Pts.Transactiondate <
                                                           Add_Months(Trunc(p_Processdate, 'Mon'), 1) THEN
                                                       Pts.Points
                                                      ELSE
                                                       0
                                                 END)) -
                      SUM(CASE
                               WHEN Ty.Name NOT LIKE 'Jean%'
                                    AND Ty.Name NOT LIKE 'Bra%'
                                    AND Trunc(Pts.Transactiondate) >=
                                    Trunc(v_Transactionstartdate)
                                    AND Pts.Expirationdate > p_Processdate
                                    AND Pts.Transactiondate <
                                    Add_Months(Trunc(p_Processdate, 'Mon'), 1) THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                          AND SUM(CASE
                                       WHEN Ty.Name NOT LIKE 'Jean%'
                                            AND Ty.Name NOT LIKE 'Bra%'
                                            AND Trunc(Pts.Transactiondate) >=
                                            Trunc(v_Transactionstartdate)
                                            AND Pts.Transactiondate <
                                            Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                                            AND Pts.Expirationdate > p_Processdate THEN
                                        Pts.Points
                                       ELSE
                                        0
                                  END) < 0 THEN
                      v_Dollarrewardlevel
                END AS Pointstonextreward,
                --BasePoints
                SUM(CASE
                         WHEN Ty.Name IN ('Basic Points', 'Adjustment Points')
                              AND Trunc(Pts.Transactiondate) >=
                              Trunc(v_Transactionstartdate)
                              AND Pts.Transactiondate <
                              Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                              AND Pts.Expirationdate > p_Processdate THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Basepoints,
                --BonusPoints
                SUM(CASE
                         WHEN Ty.Name IN ('CS Points',
                                          'Bonus Points',
                                          'Adjustment Bonus Points')
                              AND Trunc(Pts.Transactiondate) >=
                              Trunc(v_Transactionstartdate)
                              AND Pts.Transactiondate <
                              Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                              AND Pts.Expirationdate > p_Processdate THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Bonuspoints
               FROM   Ae_Currentpointtransaction2 Pts
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Ty
               ON     Ty.Pointtypeid = Pts.Pointtypeid
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Vc.Ipcode
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
                      AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 0)
               INNER  JOIN (SELECT *
                            FROM   Lw_Virtualcard Vc1
                            WHERE  Vc1.Isprimary = 1) Vc2
               ON     Vc.Ipcode = Vc2.Ipcode
               INNER  JOIN (SELECT DISTINCT (Vc5.Ipcode)
                            FROM   Lw_Virtualcard Vc5
                            INNER  JOIN (SELECT DISTINCT (Pta.Vckey)
                                        FROM   Ae_Currentpointtransaction2 Pta
                                        WHERE  Pta.Pointawarddate >=
                                               p_Memberfilestarttime) Ptb
                            ON     Vc5.Vckey = Ptb.Vckey
                            UNION
                            SELECT Dr.a_Ipcode
                            FROM   Ats_Memberdollarrewardoptout Dr
                            WHERE  Dr.Createdate >= p_Memberfilestarttime) Vc6
               ON     Vc.Ipcode = Vc6.Ipcode
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               --    FULL OUTER JOIN (select * from  ATS_MEMBERDOLLARREWARDOPTOUT DR where DR.CREATEDATE >= p_MemberfileStarttime ) DR1 on DR1.A_IPCODE = vc.ipcode
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Trunc(Pts.Transactiondate) >=
                      Trunc(v_Transactionstartdate)
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
               -- AND pts.transactiondate < ADD_MONTHS(TRUNC(p_ProcessDate, 'Q'), 3)
               GROUP  BY Lm.Ipcode, Vc2.Loyaltyidnumber, Md.a_Extendedplaycode;
          COMMIT;
     END Headerrecord_Memberpoints;

     PROCEDURE Headerrecord_Mbrpoints_Pilot(p_Dummy                VARCHAR2,
                                            p_Processdate          DATE,
                                            p_Memberfilestarttime  DATE,
                                            v_Transactionstartdate DATE) IS
          v_Sql1 VARCHAR2(1000);
     BEGIN
          INSERT INTO Aepointheaderdelta
               (Ipcode,
                Txncount,
                Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Basepoints,
                Bonuspoints)
--             SELECT /*+ parallel(pts 8) */   AEO 598 changes  here ---------------SCJ
               SELECT
                Lm.Ipcode,
                COUNT(Pts.Vckey) AS Txncount,
                MAX('H') AS Recordtype,
                MAX(Vc2.Linkkey) AS Customerkey,
                Vc2.Loyaltyidnumber,
                /*            max(bl.a_totalpoints) AS TotalPoints, */
                Greatest(SUM(CASE
                                  WHEN Ty.Name IN ('AEO Connected Points',
                                                   'AEO Connected Bonus Points',
                                                   'AEO Visa Card Points',
                                                   'AEO Customer Service Points',
                                                   'Adjustment Points',
                                                   'Adjustment Bonus Points')
                                       AND Pts.Expirationdate > p_Processdate THEN
                                   Pts.Points
                                  ELSE
                                   0
                             END),
                         0) AS Totalpoints,
                --Rewardlevel
                Floor(SUM(CASE
                               WHEN Ty.Name IN ('AEO Connected Points',
                                                'AEO Connected Bonus Points',
                                                'AEO Visa Card Points',
                                                'AEO Customer Service Points',
                                                'Adjustment Points',
                                                'Adjustment Bonus Points')
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END) / 1000) AS Rewardlevel,
                --PointsToNextReward
                CASE
                     WHEN SUM(CASE
                                   WHEN Ty.Name IN ('AEO Connected Points',
                                                    'AEO Connected Bonus Points',
                                                    'AEO Visa Card Points',
                                                    'AEO Customer Service Points',
                                                    'Adjustment Points',
                                                    'Adjustment Bonus Points')
                                        AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                                   ELSE
                                    0
                              END) > 1000 THEN
                      (Floor(SUM(CASE
                                      WHEN Ty.Name IN ('AEO Connected Points',
                                                       'AEO Connected Bonus Points',
                                                       'AEO Visa Card Points',
                                                       'AEO Customer Service Points',
                                                       'Adjustment Points',
                                                       'Adjustment Bonus Points')
                                           AND Pts.Expirationdate > p_Processdate THEN
                                       Pts.Points
                                      ELSE
                                       0
                                 END) / 1000) * 1000) + 1000 -
                      SUM(CASE
                               WHEN Ty.Name IN ('AEO Connected Points',
                                                'AEO Connected Bonus Points',
                                                'AEO Visa Card Points',
                                                'AEO Customer Service Points',
                                                'Adjustment Points',
                                                'Adjustment Bonus Points')
                                    AND Pts.Expirationdate > p_Processdate THEN
                                Pts.Points
                               ELSE
                                0
                          END)
                     ELSE
                      1000 - SUM(CASE
                                      WHEN Ty.Name IN ('AEO Connected Points',
                                                       'AEO Connected Bonus Points',
                                                       'AEO Visa Card Points',
                                                       'AEO Customer Service Points',
                                                       'Adjustment Points',
                                                       'Adjustment Bonus Points')
                                           AND Pts.Expirationdate > p_Processdate THEN
                                       Pts.Points
                                      ELSE
                                       0
                                 END)
                END AS Pointstonextreward,
                --BasePoints
                SUM(CASE
                         WHEN Ty.Name IN ('AEO Visa Card Points',
                                          'AEO Connected Points',
                                          'Adjustment Points')
                              AND Trunc(Pts.Transactiondate) >=
                              Trunc(v_Transactionstartdate)
                              AND Pts.Transactiondate <
                              Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                              AND Pts.Expirationdate > p_Processdate THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Basepoints,
                --BonusPoints
                SUM(CASE
                         WHEN Ty.Name IN ('AEO Customer Service Points',
                                          'AEO Connected Bonus Points',
                                          'Adjustment Bonus Points')
                              AND Trunc(Pts.Transactiondate) >=
                              Trunc(v_Transactionstartdate)
                              AND Pts.Transactiondate <
                              Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                              AND Pts.Expirationdate > p_Processdate THEN
                          Pts.Points
                         ELSE
                          0
                    END) AS Bonuspoints
               FROM   Ae_Currentpointtransaction2 Pts
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Ty
               ON     Ty.Pointtypeid = Pts.Pointtypeid
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Vc.Ipcode
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
                      AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
               INNER  JOIN (SELECT *
                            FROM   Lw_Virtualcard Vc1
                            WHERE  Vc1.Isprimary = 1) Vc2
               ON     Vc.Ipcode = Vc2.Ipcode
               INNER  JOIN (SELECT DISTINCT (Vc5.Ipcode)
                            FROM   Lw_Virtualcard Vc5
                            INNER  JOIN (SELECT DISTINCT (Pta.Vckey)
                                        FROM   Ae_Currentpointtransaction2 Pta
                                        WHERE  Pta.Pointawarddate >=
                                               p_Memberfilestarttime) Ptb
                            ON     Vc5.Vckey = Ptb.Vckey
                            UNION
                            SELECT Dr.a_Ipcode
                            FROM   Ats_Memberdollarrewardoptout Dr
                            WHERE  Dr.Createdate >= p_Memberfilestarttime) Vc6
               ON     Vc.Ipcode = Vc6.Ipcode
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
               WHERE  Pnt.Name NOT LIKE 'Jean%'
                      AND Pnt.Name NOT LIKE 'Bra%'
                      AND Trunc(Pts.Transactiondate) >=
                      Trunc(v_Transactionstartdate)
                      AND Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
               GROUP  BY Lm.Ipcode, Vc2.Loyaltyidnumber, Md.a_Extendedplaycode;
          COMMIT;
     END Headerrecord_Mbrpoints_Pilot;

     PROCEDURE Memberpointsdelta(p_Dummy       VARCHAR2,
                                 p_Processdate DATE,
                                 b_Name        VARCHAR2,
                                 Retval        IN OUT Rcursor) AS
          Pdummy                 VARCHAR2(2);
          Pprocessdate           DATE := p_Processdate;
          v_Sql1                 VARCHAR2(1000);
          v_Sql2                 VARCHAR2(1000);
          v_Sql3                 VARCHAR2(1000);
          v_Sql4                 VARCHAR2(1000);
          v_Sql5                 VARCHAR2(1000);
          v_Memberfilestarttime  DATE;
          v_Transactionstartdate DATE;
     BEGIN
          v_Sql1 := 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where trunc(pt.transactiondate) >= Add_Months(trunc(sysdate, ''Q''), -15)';
          v_Sql2 := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'')  from lw_clientconfiguration cc where cc.key = ''LastAEMemberPointsSent''';
          v_Sql3 := 'Select to_date(to_char(value), ''mm/dd/yyyy'')  from lw_clientconfiguration cc where cc.key = ''CalculateTransactionStartDate''';
          v_Sql4 := 'CREATE INDEX AE_CURRENTPTS2_RKED  ON AE_CURRENTPOINTTRANSACTION2(rowkey,expirationdate)  ONLINE COMPUTE STATISTICS TABLESPACE bp_ae_idx1';
          v_Sql5 := 'CREATE INDEX AE_CURRENTPTS2_VCKY  ON AE_CURRENTPOINTTRANSACTION2(vckey)  ONLINE COMPUTE STATISTICS TABLESPACE bp_ae_idx1';
          EXECUTE IMMEDIATE 'Drop Table AE_CurrentPointTransaction2';
          EXECUTE IMMEDIATE v_Sql1;
          EXECUTE IMMEDIATE v_Sql4;
          EXECUTE IMMEDIATE v_Sql5;
 --Running of stats on AE_CurrentPointTransaction2 becasue it was recreated
          dbms_stats.gather_table_stats(ownname => 'bp_ae', tabname => 'AE_CurrentPointTransaction2',  cascade => TRUE );
          EXECUTE IMMEDIATE v_Sql2
               INTO v_Memberfilestarttime;
          EXECUTE IMMEDIATE v_Sql3
               INTO v_Transactionstartdate;
          Updatelastaememberpointssent(Pdummy);
          EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA';
          Headerrecord_Memberpoints(Pdummy,
                                    Pprocessdate,
                                    v_Memberfilestarttime,
                                    v_Transactionstartdate);
          Headerrecord_Mbrpoints_Pilot(Pdummy,
                                       Pprocessdate,
                                       v_Memberfilestarttime,
                                       v_Transactionstartdate);
          INSERT INTO Ae_Memberpointsdelta
               (Recordtype,
                Customerkey,
                Loyaltyidnumber,
                Totalpoints,
                Rewardlevel,
                Pointstonextreward,
                Description,
                Basepoints,
                Bonuspoints,
                Txndate,
                Txnnumber,
                Ordernumber,
                Storenumber,
                Registernumber)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Totalpoints,
                      Mem.Rewardlevel,
                      Mem.Pointstonextreward,
                      NULL AS Description,
                      Mem.Basepoints,
                      Mem.Bonuspoints,
                      CASE
                           WHEN b_Name = 'QTR' THEN
                            To_Char(Pprocessdate, 'MMDDYYYY')
                           ELSE
                            To_Char(Pprocessdate - 1, 'MMDDYYYY')
                      END AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Aepointheaderdelta Mem
               WHERE  Mem.Txncount > 0
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points',
                                           'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                           'CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      h.a_Txnnumber AS Txnnumber,
                      h.a_Ordernumber AS Ordernumber,
                      h.a_Storenumber AS Storenumber,
                      h.a_Txnregisternumber AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               -- WHERE pt.transactiondate >= TRUNC(p_ProcessDate, 'Q') and pt.transactiondate < ADD_MONTHS(TRUNC(p_ProcessDate, 'Q'), 3)
               WHERE  Trunc(Pt.Transactiondate) >=
                      Trunc(v_Transactionstartdate)
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
               UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points',
                                           'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                           'CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Trunc(Pt.Transactiondate) >=
                      Trunc(v_Transactionstartdate)
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      NULL AS Totalpoints,
                      NULL AS Rewardlevel,
                      NULL AS Pointstonextreward,
                      Pt.Pointeventid AS Description,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points',
                                           'Adjustment Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                           'CS Points',
                                           'Bonus Points',
                                           'Adjustment Bonus Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Bonuspoints,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Currentpointtransaction2 Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Trunc(Pt.Transactiondate) >=
                      Trunc(v_Transactionstartdate)
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Pointtypeid IN
                      (SELECT Pointtypeid
                           FROM   Lw_Pointtype
                           WHERE  NAME NOT LIKE 'Jean%'
                                  AND NAME NOT LIKE 'Bra%')
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdelta;

     --Changes for PI30364 END here  -- SCJ
     PROCEDURE Custombrafullfilment(p_Dummy          VARCHAR2,
                                    p_Startissuedate VARCHAR2,
                                    p_Endissuedate   VARCHAR2,
                                    Retval           IN OUT Rcursor) AS
          Pstartissuedate DATE := To_Date(p_Startissuedate,
                                          'MM/DD/YYYY HH:MI:SS AM');
          Pendissuedate   DATE := To_Date(p_Endissuedate,
                                          'MM/DD/YYYY HH:MI:SS AM');
     BEGIN
          EXECUTE IMMEDIATE 'Truncate Table AE_TEMP_BRAFULFILLMENT';
          INSERT INTO Ae_Temp_Brafulfillment
               (Loyaltynumber,
                Promocode,
                Firstname,
                Lastname,
                Line1,
                Line2,
                City,
                State,
                Postalcode,
                Country,
                Addressmailable,
                Reasoncode,
                Languagepreference,
                Under13,
                Brandflag_Ae,
                Brandflag_Aerie,
                Brandflag_77kids,
                Basebrand,
                Promotioncount)
               SELECT Vc.Loyaltyidnumber AS Loyaltynumber,
                      'BRAS002' AS Promocode,
                      Lm.Firstname AS Firstname,
                      Lm.Lastname AS Lastname,
                      REPLACE(Md.a_Addresslineone, ',', '') AS Line1,
                      REPLACE(Md.a_Addresslinetwo, ',', '') AS Line2,
                      REPLACE(Md.a_City, ',', '') AS City,
                      Md.a_Stateorprovince AS State,
                      Md.a_Ziporpostalcode AS Postalcode,
                      Md.a_Country AS Country,
                      Md.a_Addressmailable AS Addressmailable,
                      '720' AS Reasoncode,
                      Md.a_Languagepreference AS Languagepreference,
                      CASE
                           WHEN Md.a_Isunderage IS NULL THEN
                            'N'
                           WHEN Md.a_Isunderage = 0 THEN
                            'N'
                           WHEN Md.a_Isunderage = 1 THEN
                            'Y'
                      END AS Under13,
                      Nvl(Ats_Memberbrand.Brandae, 0) AS Brandflag_Ae,
                      Nvl(Ats_Memberbrand.Brandaerie, 0) AS Brandflag_Aerie,
                      Nvl(Ats_Memberbrand.Brandkids, 0) AS Brandflag_77kids,
                      CASE
                           WHEN Mdb.a_Brandnumber = 20 THEN
                            CAST(To_Char(Mdb.a_Brandnumber) AS NVARCHAR2(100))
                           ELSE
                            CAST('00' AS NVARCHAR2(100))
                      END AS Basebrand,
                      COUNT(Mr.Id) AS Promotioncount
               FROM   Lw_Memberrewards Mr,
                      Lw_Rewardsdef Rd,
                      Lw_Virtualcard Vc,
                      Lw_Loyaltymember Lm,
                      Ats_Memberdetails Md,
                      Ats_Refbrand Mdb,
                      (SELECT SUM(CASE
                                       WHEN b.a_Shortbrandname = 'AE' THEN
                                        1
                                       ELSE
                                        0
                                  END) AS Brandae,
                              SUM(CASE
                                       WHEN b.a_Shortbrandname = 'aerie' THEN
                                        1
                                       ELSE
                                        0
                                  END) AS Brandaerie,
                              SUM(CASE
                                       WHEN b.a_Shortbrandname = '77kids' THEN
                                        1
                                       ELSE
                                        0
                                  END) AS Brandkids,
                              Mb.a_Ipcode
                       FROM   Ats_Memberbrand Mb, Ats_Refbrand b
                       WHERE  Mb.a_Brandid = b.a_Brandid
                       GROUP  BY Mb.a_Ipcode) Ats_Memberbrand
               WHERE  1 = 1
                      AND Mr.Rewarddefid = Rd.Id
                      AND Rd.Name LIKE 'Bra Reward%'
                      AND Mr.Memberid = Vc.Ipcode
                      AND Vc.Isprimary = 1
                      AND Mr.Memberid = Lm.Ipcode
                      AND Lm.Ipcode = Md.a_Ipcode
                      AND Md.a_Basebrandid = Mdb.a_Brandid(+)
                      AND Mr.Memberid = Ats_Memberbrand.a_Ipcode(+)
                      AND Mr.Dateissued BETWEEN Pstartissuedate AND
                      Pendissuedate
                      AND Mr.Fulfillmentdate IS NULL
               GROUP  BY Vc.Loyaltyidnumber,
                         Lm.Firstname,
                         Lm.Lastname,
                         Md.a_Addresslineone,
                         Md.a_Addresslinetwo,
                         Md.a_City,
                         Md.a_Stateorprovince,
                         Md.a_Ziporpostalcode,
                         Md.a_Country,
                         Md.a_Addressmailable,
                         '720',
                         Md.a_Languagepreference,
                         Md.a_Isunderage,
                         Ats_Memberbrand.Brandae,
                         Ats_Memberbrand.Brandaerie,
                         Ats_Memberbrand.Brandkids,
                         Mdb.a_Brandnumber;
          COMMIT;
     END Custombrafullfilment;

     --Changes for PI 30364 - Dollar reward program start here -- Akbar
     PROCEDURE Updatedollarrewardoptoutdate(p_Dummy VARCHAR2,
                                            Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastDollarRewardOptOutDate';
          COMMIT;
     END Updatedollarrewardoptoutdate;

     --Changes for PI 30364 - Dollar reward program end here -- Akbar
     PROCEDURE Buildemailreminderlist(p_Dummy VARCHAR2,
                                      Retval  IN OUT Rcursor) AS
     BEGIN
          INSERT INTO Ae_Emailremindershistory
               (Ipcode, Emailaddress, Senddate)
               SELECT Ipcode, Emailaddress, Senddate FROM Ae_Emailreminders;
          EXECUTE IMMEDIATE 'Truncate Table AE_EmailReminders';
          INSERT INTO Ae_Emailreminders
               (Ipcode, Emailaddress, Senddate)
               SELECT Lm.Ipcode, Md.a_Emailaddress, SYSDATE
               FROM   Lw_Loyaltymember Lm
               JOIN   Ats_Memberdetails Md
               ON     Lm.Ipcode = Md.a_Ipcode
               JOIN   Lw_Virtualcard Vc
               ON     Vc.Ipcode = Md.a_Ipcode
               WHERE  Length(Md.a_Emailaddress) > 0
                      AND Md.a_Pendingemailverification = 1 -- MMV409
                      AND To_Char(Md.a_Nextemailreminderdate, 'DD/MM/YYYY') =
                      To_Char(SYSDATE, 'DD/MM/YYYY')
                      AND (SELECT COUNT(*)
                           FROM   Ae_Emailremindershistory
                           WHERE  Ipcode = Lm.Ipcode) <= 3; --MMV-409
          COMMIT;
     END Buildemailreminderlist;

     /* --------------------------------------------------------------------
       This procedure  assigns today's date plus 15 days to the a_nextemailreminderdate
       when the account only has 0 or 1 row in reminders history, or simply 15 days more
       if the account had a purchase.

       This procedure is executed at the end of the JOB configure in VC
       to create the  initial e-mail validation file and the e-mail validation
       reminder file.

     ---------------------------------------------------------------------- */
     PROCEDURE Updateemailreminderdate(p_Dummy VARCHAR2,
                                       Retval  IN OUT Rcursor) AS
     BEGIN
          -- AEO-409 begin
          -- change the next e-mail reminders date
          UPDATE Ats_Memberdetails Md
          SET    Md.a_Nextemailreminderdate =
                 (SYSDATE + 15)
          WHERE  Md.a_Ipcode IN (SELECT Ipcode FROM Ae_Emailreminders)
                 AND (SELECT COUNT(*)
                      FROM   Ae_Emailremindershistory
                      WHERE  Ipcode = Md.a_Ipcode) < 2;
          UPDATE Ats_Memberdetails Md
          SET    Md.a_Nextemailreminderdate = Md.a_Nextemailreminderdate + 15
          WHERE  Md.a_Ipcode IN
                 (SELECT To_Char(Lm.Ipcode)
                  FROM   Lw_Loyaltymember Lm
                  JOIN   Ats_Memberdetails Md
                  ON     Lm.Ipcode = Md.a_Ipcode
                  JOIN   Lw_Virtualcard Vc
                  ON     Vc.Ipcode = Md.a_Ipcode
                  WHERE  Length(Md.a_Emailaddress) > 0
                         AND Md.a_Pendingemailverification = 1
                         AND Ae_Isinpilot(Md.a_Extendedplaycode) = 1
                         AND (Trunc(Lm.Lastactivitydate) = Trunc(SYSDATE))
                         AND
                         (Lm.Lastactivitydate > Md.a_Nextemailreminderdate AND
                          (Trunc(To_Number((Trunc(Lm.Lastactivitydate) -
                                           Trunc(Lm.Createdate)))) < 45))
                         AND (SELECT COUNT(*)
                              FROM   Ae_Emailremindershistory
                              WHERE  Ipcode = Lm.Ipcode) = 3);
          COMMIT;
          -- AEO-409 end
     END Updateemailreminderdate;

     PROCEDURE Updatelastsmsverifysend(p_Dummy VARCHAR2,
                                       Retval  IN OUT Rcursor) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastSMSVerificationSendDate';
          COMMIT;
     END Updatelastsmsverifysend;

     PROCEDURE Missedemailverif_081115(p_Dummy VARCHAR2,
                                       Retval  IN OUT Rcursor) AS
          Lv_Row_Count NUMBER := 0;
          Lv_Stats_Id  NUMBER;
          CURSOR c IS
               SELECT Md.a_Nextemailreminderdate,
                      Md.a_Ipcode,
                      Md.a_Extendedplaycode
               FROM   Bp_Ae.Ats_Memberdetails Md
               INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm
               ON     Md.a_Ipcode = Lm.Ipcode
               WHERE  Trunc(Md.a_Nextemailreminderdate) =
                      To_Date('11/08/2015', 'mm/dd/yyyy')
                      AND
                      Trunc(Lm.Membercreatedate) NOT BETWEEN Trunc(SYSDATE - 1) AND
                      Trunc(SYSDATE)
                      AND Nvl(Md.a_Extendedplaycode, 0) IN (1, 3);
          TYPE c_Type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
          Rec c_Type;
     BEGIN
          OPEN c;
          LOOP
               FETCH c BULK COLLECT
                    INTO Rec LIMIT 1000;
               -- FORALL Update
               FORALL i IN 1 .. Rec.Count
                    UPDATE Bp_Ae.Ats_Memberdetails Md
                    SET    Md.a_Nextemailreminderdate = SYSDATE + 15
                    WHERE  Md.a_Ipcode = Rec(i).a_Ipcode;
               Lv_Row_Count := Lv_Row_Count + SQL%ROWCOUNT;
               -- Commit, release lock
               COMMIT WRITE Batch NOWAIT;
               EXIT WHEN c%NOTFOUND;
          END LOOP;
          CLOSE c;
          COMMIT;
     EXCEPTION
          WHEN OTHERS THEN
               ROLLBACK;
               RAISE;
     END;

-- AEO-817 begin
     PROCEDURE UpdateRewards10DMseqnumber(p_Number INTEGER) AS
          v_Rewards10dmsequencenumber INTEGER;
     BEGIN
          SELECT To_Number(To_Char(c.Value))
          INTO   v_Rewards10dmsequencenumber
          FROM   Lw_Clientconfiguration c
          WHERE  c.Key = '10RewardsDMSequenceNumber';

          v_Rewards10dmsequencenumber := v_Rewards10dmsequencenumber + p_Number;

          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Rewards10dmsequencenumber)
          WHERE  Key = '10RewardsDMSequenceNumber';
          COMMIT;
     END UpdateRewards10DMseqnumber;


   PROCEDURE Updaterewards10seqnumber(p_Number INTEGER) AS
        v_Rewards10sequencenumber INTEGER;
   BEGIN
        SELECT To_Number(To_Char(c.Value))
        INTO   v_Rewards10sequencenumber
        FROM   Lw_Clientconfiguration c
        WHERE  c.Key = '10RewardsSequenceNumber';

        v_Rewards10sequencenumber := v_Rewards10sequencenumber + p_Number;

        UPDATE Lw_Clientconfiguration
        SET    VALUE = To_Char(v_Rewards10sequencenumber)
        WHERE  Key = '10RewardsSequenceNumber';
        COMMIT;
   END Updaterewards10seqnumber;
   -- AEO-817 end

   -- AEO-907 Start AH
   PROCEDURE CheckSequenceCodes( p_SequencePrefix VARCHAR2, p_SequenceName VARCHAR2 ) AS
     v_sequence_number varchar2(3);
     v_sequence_month INTEGER;
     v_sequence       varchar2(26);
     v_authCount      INTEGER := 0;
     v_authCode       varchar2(10);
     v_count          NUMBER(2) := 0;

     -- Email Variables
     v_html_message   clob;
     v_attachments cio_mail.attachment_tbl_type;
     v_returnmessage         VARCHAR2(1000);

     -- Logging
     v_my_log_id             NUMBER;
     v_jobname               VARCHAR2(256);
     v_envkey                VARCHAR2(256)        := 'bp_ae@'||UPPER(sys_context('userenv','instance_name'));
     v_batchid               VARCHAR2(256)        := 0 ;
     v_messageid             VARCHAR2(256);
     v_message               VARCHAR2(1000) ;
     v_reason                VARCHAR2(1000) ;
     v_error                 VARCHAR2(1000) ;
     v_logsource             VARCHAR2(1000) ;

   BEGIN
     v_my_log_id := utility_pkg.get_LIBJobID();
     v_jobname   := 'CheckSequenceCodes';
     v_logsource := v_jobname;

     SELECT count(To_Char(cc.Value))
     INTO v_count
     FROM Lw_ClientConfiguration cc
     WHERE 1=1
     AND cc.key = p_SequenceName;

     IF ( v_count > 0 ) THEN
       SELECT To_Char(cc.Value)
       INTO v_sequence_number
       FROM Lw_ClientConfiguration cc
       WHERE 1=1
       AND cc.key = p_SequenceName;
     ELSE
       Raise_Application_Error(-20000,
                               'Sequence: ' || p_SequenceName || ' does not exist.');
     END IF;


     v_sequence_month := To_Number(To_Char(SYSDATE, 'MM'));

     IF (p_SequenceName LIKE '%Birthday%') THEN
       v_sequence_month := v_sequence_month + 1;
       IF (v_sequence_month = 13) THEN
         v_sequence_month := 1;
       END IF;

     END IF;

     v_sequence := p_SequencePrefix || '_' || to_char(v_sequence_month) || '_' || v_sequence_number;

     select count(*)
     into v_authCount
     from ats_rewardshortcode sc
     where 1=1
     and sc.a_typecode = v_sequence;

     IF ( v_authCount > 0 ) THEN
        SELECT sc.a_shortcode
        INTO v_authCode
        FROM ats_rewardshortcode sc
        WHERE 1=1
        AND sc.a_typecode = v_sequence
        AND rownum = 1;
     ELSE
       v_authCode := '__NoAuth__';
     END IF;

     SELECT COUNT(*)
     INTO v_authCount
     FROM ats_rewardbarcodes rb
     WHERE 1=1
     AND rb.a_typecode = v_authCode
     AND ROWNUM <=2;

     /* Assume invalid barcode and notice support that there's an issue */
     IF ( v_authCount < 1 ) THEN
       v_html_message := '<html>' || chr(10) ||
                      '<body>' || chr(10) ||
                      '<br>' || chr(10) ||
                      'There was a problem with the sequence number when trying to issue rewards related to <b>' || p_SequenceName ||
                      '</b><br>' || chr(10) ||
                      'The sequence was updated to ' || v_sequence || ' with auth code <b>' || v_authCode || '</b>.<br><br>' || chr(10) ||
                      'REMINDER: Do not bump sequence after issue is resolved <br><br>' || chr(10) ||
                      'Thanks, <br>' || chr(10) ||
                      'The failed job. <br>' || chr(10) ||
                      '</body>' || chr(10) ||
                      '</html>';
       cio_mail.send_external(
                    p_from_email       => 'AEREWARDS',
                    p_from_replyto     => NULL,
                    p_to_list          => 'AERewards_Support@brierley.com;nicox@brierley.com;lreynolds@brierley.com;kward@brierley.com;rsekinger@brierley.com',
                    p_cc_list          => NULL,
                    p_bcc_list         => NULL,
                    p_subject          => 'Rewards - AuthCode Failure',
                    p_text_message     => v_html_message,
                    p_content_type     => 'text/html;charset=UTF8',
                    p_attachments      => v_attachments,
                    p_priority         => '1',
                    p_auth_username    => NULL,
                    p_auth_password    => NULL,
                    p_mail_server      => 'cypwebmail.brierleyweb.com',
                    p_port             => 25,
                    p_returnmessage    => v_returnmessage);
                If(v_returnmessage != 'SUCCESS') Then
                    v_Error          := v_returnmessage;
                    v_Reason         := 'Failed Procedure CheckSequenceCodes';
                    v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                                        '    <pkg>AEJOBS</pkg>' || Chr(10) ||
                                        '    <proc>CheckSequenceCodes</proc>' ||
                                        Chr(10) || '  </details>' ||
                                        Chr(10) || '</failed>';
                    utility_pkg.Log_msg(p_messageid         => v_messageid,
                         p_envkey            => v_envkey   ,
                         p_logsource         => v_logsource,
                         p_filename          => null ,
                         p_batchid           => 0,
                         p_jobnumber         => v_my_log_id,
                         p_message           => v_message  ,
                         p_reason            => v_reason   ,
                         p_error             => v_error    ,
                         p_trycount          => 1 ,
                         p_msgtime           => SYSDATE  );
                END IF;
       ELSE
         v_html_message := '<html>' || chr(10) ||
                      '<body>' || chr(10) ||
                      '<br>' || chr(10) ||
                      'There are valid rewards related to <b>' || p_SequenceName ||
                      '</b><br>' || chr(10) ||
                      'The sequence was updated to ' || v_sequence || ' with auth code <b>' ||
                      v_authCode || '</b>.<br><br>' || chr(10) ||
                      'Thanks, <br>' || chr(10) ||
                      'The successful job. <br>' || chr(10) ||
                      '</body>' || chr(10) ||
                      '</html>';
             cio_mail.send_external(
                    p_from_email       => 'AEREWARDS',
                    p_from_replyto     => NULL,
                    p_to_list          => 'AERewards_Support@brierley.com;nicox@brierley.com;lreynolds@brierley.com;kward@brierley.com;rsekinger@brierley.com',
                    p_cc_list          => NULL,
                    p_bcc_list         => NULL,
                    p_subject          => 'Rewards - Valid Authcode Found',
                    p_text_message     => v_html_message,
                    p_content_type     => 'text/html;charset=UTF8',
                    p_attachments      => v_attachments,
                    p_priority         => '3',
                    p_auth_username    => NULL,
                    p_auth_password    => NULL,
                    p_mail_server      => 'cypwebmail.brierleyweb.com',
                    p_port             => 25,
                    p_returnmessage    => v_returnmessage);
             If(v_returnmessage != 'SUCCESS') Then
                          v_Error          := v_returnmessage;
                          v_Reason         := 'Failed Procedure CheckSequenceCodes';
                          v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                                              '    <pkg>AEJOBS</pkg>' || Chr(10) ||
                                              '    <proc>CheckSequenceCodes</proc>' ||
                                              Chr(10) || '  </details>' ||
                                              Chr(10) || '</failed>';
                          utility_pkg.Log_msg(p_messageid         => v_messageid,
                               p_envkey            => v_envkey   ,
                               p_logsource         => v_logsource,
                               p_filename          => null ,
                               p_batchid           => 0,
                               p_jobnumber         => v_my_log_id,
                               p_message           => v_message  ,
                               p_reason            => v_reason   ,
                               p_error             => v_error    ,
                               p_trycount          => 1 ,
                               p_msgtime           => SYSDATE  );
             END IF;

     END IF;
   END CheckSequenceCodes;
   -- AEO-907 End AH

     -- aeo-880 begin
     PROCEDURE UpdateStackedSequences(p_Dummy VARCHAR2) AS

     BEGIN

          UPDATE Lw_Clientconfiguration cc
          SET    VALUE = To_Char( To_Number(To_Char(cc.Value)) + 1)
          WHERE  Key LIKE '%StackedSequenceNumber%';
          COMMIT;

     END UpdateStackedSequences;
     -- AEO 880-end

-- AEO-1592 begin GD
  /********************************************************************
  This process will reset the EmployeeCode from Previous Employee to
  zero and then from Current Employee to Previous Employee
  ********************************************************************/
PROCEDURE ResetEmployeeCode(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AEJobs.ResetEmployeeCode';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AEJobs.ResetEmployeeCode';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'ResetEmployeeCode';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));

  BEGIN

    v_My_Log_Id := Utility_Pkg.Get_Libjobid();
    /* log start of job (lw logging)*/
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => v_Filename,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);


    -----------------------------------------------------------------
    -- Truncate the MemberBrand table to clear out the purchase brands
    -- for the quarter.
    -----------------------------------------------------------------
    EXECUTE IMMEDIATE 'Truncate Table ats_memberbrand';

    -----------------------------------------------------------------
    -- Reset EmployeeCode
    -----------------------------------------------------------------
    DECLARE
      CURSOR get_data IS
        SELECT md.a_ipcode as ipcode,
               md.a_employeecode as employeecode
          FROM ats_memberdetails md
         WHERE md.a_employeecode in (1, 2);
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails md
             SET md.a_employeecode =
                 case
                   when v_tbl(i).employeecode = 1 then 2
                 else 0
                 end,
                 md.a_changedby = 'ResetEmployeeCode',
                 md.updatedate = SYSDATE
           WHERE md.a_ipcode = v_tbl(i).ipcode;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;

  EXCEPTION
    WHEN OTHERS THEN

      ROLLBACK;
      IF v_Messagesfailed = 0 THEN
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
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure ResetEmployeeCode';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsBackupWkTbl</proc>' ||
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

  END ResetEmployeeCode;
-- AEO-1592 end GD
END Aejobs;
/
