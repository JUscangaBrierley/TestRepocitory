CREATE OR REPLACE PACKAGE STAGE_PROFILEUPDATES IS
  TYPE RCURSOR IS REF CURSOR;

  PROCEDURE STAGE_PROFILEUPDATES01(P_FILENAME IN VARCHAR2,
                                   RETVAL     IN OUT RCURSOR);
END STAGE_PROFILEUPDATES;
/

CREATE OR REPLACE PACKAGE BODY STAGE_PROFILEUPDATES IS

  /********* Procedure to map external table to specified file ********/
  PROCEDURE CHANGEEXTERNALTABLE(P_FILENAME IN VARCHAR2) IS
    E_MTABLE    EXCEPTION;
    E_MFILENAME EXCEPTION;
    V_SQL VARCHAR2(400);
  BEGIN

    IF LENGTH(TRIM(P_FILENAME)) = 0 OR P_FILENAME IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001,
                              'Filename is required to link with external table',
                              FALSE);
    END IF;

    V_SQL := 'ALTER TABLE EXT_PROFILEUPDATES LOCATION (AE_IN' || CHR(58) || '''' ||
             P_FILENAME || ''')';
    EXECUTE IMMEDIATE V_SQL;

  END CHANGEEXTERNALTABLE;

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE STAGE_PROFILEUPDATES01(P_FILENAME IN VARCHAR2,
                                   RETVAL     IN OUT RCURSOR) IS

    V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := P_FILENAME;
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
    V_FILENAME  VARCHAR2(256) := P_FILENAME;
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);
  BEGIN

    CHANGEEXTERNALTABLE(P_FILENAME => P_FILENAME);
    /* get job id for this process and the dap process */
    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    -- V_DAP_LOG_ID := UTILITY_PKG.GET_LIBJOBID(); AEO-374 Begin - End

    V_JOBNAME   := 'ProfileUpdates';
    V_LOGSOURCE := V_JOBNAME;

    /* log start of job */
    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => 'stage' || V_JOBNAME);

    EXECUTE IMMEDIATE 'Truncate Table lw_profileupdates_stage';

    -----------------------------------------------------------------------------------------
    -- Insert Into Stage table from external table joining virtualcard and memberdetails to
    -- determine if a member exists and if the aitflag is set to bypass the update.
    -----------------------------------------------------------------------------------------
    DECLARE
      CURSOR GET_DATA IS
        SELECT /*+ full(md)*/
         NVL(VC.IPCODE, 0) AS IPCODE,
         P.CID,
         P.FIRSTNAME,
         P.LASTNAME,
         P.ADDRESSLINE1,
         P.ADDRESSLINE2,
         P.CITY,
         P.STATE,
         P.ZIP,
         ST.A_COUNTRYCODE AS COUNTRY,
         P.LOYALTYNUMBER,
         CASE
           WHEN P.COMFORTCODE = '00' THEN
            1
           ELSE
            0
         END AS ADDRESSMAILABLE,
         NVL(MD.A_AITUPDATE, 0) AS AITFLAG,
         P.ACCOUNTKEY,
         P.EMAILADDRESS -- AEO-266
          FROM EXT_PROFILEUPDATES P,
               (SELECT /*+ full(vc)*/
                 IPCODE, MAX(LOYALTYIDNUMBER) AS LOYALTYIDNUMBER
                  FROM LW_VIRTUALCARD VC, EXT_PROFILEUPDATES P
                 WHERE VC.LOYALTYIDNUMBER = P.LOYALTYNUMBER
                 GROUP BY VC.IPCODE) VC,
               ATS_MEMBERDETAILS MD,
               ATS_REFSTATES ST,
               LW_LOYALTYMEMBER LM -- AEO-374
         WHERE P.LOYALTYNUMBER = VC.LOYALTYIDNUMBER(+)
           AND VC.IPCODE = LM.IPCODE(+)
           AND VC.IPCODE = MD.A_IPCODE(+)
           AND P.STATE = ST.A_STATECODE(+)
           AND LM.MEMBERSTATUS <> 3; --AEO-374

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      V_TBL T_TAB; ---<------ our arry object
    BEGIN
      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          INSERT INTO LW_PROFILEUPDATES_STAGE
            (ID,
             IPCODE,
             CID,
             FIRSTNAME,
             LASTNAME,
             ADDRESSLINE1,
             ADDRESSLINE2,
             CITY,
             STATE,
             ZIP,
             COUNTRY,
             ADDRESSMAILABLE,
             CREATEDATE,
             ERRORCODE,
             ACCOUNTKEY,
             EMAILADDRESS)
          VALUES
            (SEQ_ROWKEY.NEXTVAL,
             V_TBL(I).IPCODE,
             V_TBL(I).CID,
             V_TBL(I).FIRSTNAME,
             V_TBL(I).LASTNAME,
             V_TBL(I).ADDRESSLINE1,
             V_TBL(I).ADDRESSLINE2,
             V_TBL(I).CITY,
             V_TBL(I).STATE,
             V_TBL(I).ZIP,
             V_TBL(I).COUNTRY,
             V_TBL(I).ADDRESSMAILABLE,
             SYSDATE,
             CASE
               WHEN V_TBL(I).IPCODE = 0 THEN
                1
               WHEN V_TBL(I).AITFLAG = 1 THEN
                2
               ELSE
                0
             END,

             SUBSTR(TRIM(V_TBL(I).ACCOUNTKEY), 1, 20),
             V_TBL(I).EMAILADDRESS); -- AEO-266
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;
    END;

    -----------------------------------------------------------------------------------------
    -- Now using the data in the Staging table update the memberdetails for only records
    -- where the ipcode is greater than 0 and the error code = 0
    -----------------------------------------------------------------------------------------
    DECLARE
      CURSOR GET_DATA IS
        SELECT DISTINCT VC.VCKEY         AS VCKEY,
               PUS.ACCOUNTKEY   AS STAGEACCOUNTKEY,
               SYA.A_ACCOUNTKEY AS SYNCHACCOUNTKEY,
               VC.IPCODE        AS IPCODE, -- AEO-266 Begin
               PUS.EMAILADDRESS AS EMAILADDRESS -- AEO-266 End
          FROM LW_PROFILEUPDATES_STAGE PUS
         INNER JOIN LW_VIRTUALCARD VC
            ON VC.IPCODE = PUS.IPCODE
           AND VC.ISPRIMARY = 1
          LEFT JOIN ATS_SYNCHRONYACCOUNTKEY SYA
            ON VC.VCKEY = SYA.A_VCKEY;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      V_TBL T_TAB; ---<------ our arry object
    BEGIN
      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.

        FOR I IN 1 .. V_TBL.COUNT LOOP

          IF V_TBL(I).SYNCHACCOUNTKEY IS NULL THEN
            INSERT INTO ATS_SYNCHRONYACCOUNTKEY
              (A_ROWKEY,
               A_VCKEY,
               A_PARENTROWKEY,
               STATUSCODE,
               CREATEDATE,
               UPDATEDATE)
            VALUES
              (SEQ_ROWKEY.NEXTVAL,
               V_TBL             (I).VCKEY,
               V_TBL             (I).VCKEY,
               0,
               SYSDATE,
               SYSDATE);
          ELSIF V_TBL(I).STAGEACCOUNTKEY <> V_TBL(I).SYNCHACCOUNTKEY THEN
            UPDATE ATS_SYNCHRONYACCOUNTKEY
               SET A_ACCOUNTKEY = V_TBL(I).STAGEACCOUNTKEY
             WHERE A_VCKEY = V_TBL(I).VCKEY;
          END IF;

          -- AEO-266 Begin
          IF V_TBL(I).EMAILADDRESS IS NOT NULL THEN
            UPDATE ATS_MEMBERDETAILS
               SET A_EMAILADDRESS = V_TBL(I).EMAILADDRESS
             WHERE A_IPCODE = V_TBL(I).IPCODE
               AND (A_MEMBERSOURCE <> 19 AND A_MEMBERSOURCE <> 20);
          END IF;
          -- AEO-266 End
          COMMIT;
        END LOOP;
        EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;
    END;

    -----------------------------------------------------------------------------------------
    -- Now using the data in the Staging table update the memberdetails for only records
    -- where the ipcode is greater than 0 and the error code = 0
    -----------------------------------------------------------------------------------------
    DECLARE
      CURSOR GET_DATA IS
        SELECT P.IPCODE,
               P.CID,
               P.FIRSTNAME,
               P.LASTNAME,
               P.ADDRESSLINE1,
               P.ADDRESSLINE2,
               P.CITY,
               P.STATE,
               P.ZIP,
               P.COUNTRY,
               P.ADDRESSMAILABLE
          FROM LW_PROFILEUPDATES_STAGE P
         WHERE P.ERRORCODE = 0
         ORDER BY P.ID;
      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      V_TBL T_TAB; ---<------ our arry object
      --    dml_errors  EXCEPTION;  PRAGMA EXCEPTION_INIT(dml_errors, -24381);
    BEGIN
      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.

        -- Update AddressMailable flag in ATS_MemberDetails table
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ATS_MEMBERDETAILS MD
             SET MD.A_ADDRESSMAILABLE = CASE
                                          WHEN V_TBL(I).ADDRESSMAILABLE = 0 THEN
                                           0
                                          ELSE
                                           CASE
                                             WHEN (V_TBL(I).ADDRESSMAILABLE = 1 AND V_TBL(I).ADDRESSLINE1 IS NOT NULL AND V_TBL(I)
                                                  .CITY IS NOT NULL AND V_TBL(I).ZIP IS NOT NULL AND V_TBL(I)
                                                  .STATE IS NOT NULL AND V_TBL(I)
                                                  .STATE IN (SELECT A_STATECODE FROM ATS_REFSTATES)) THEN
                                              1
                                           END
                                        END,
                 MD.A_CHANGEDBY       = 'Profile Updates Process',
                 MD.UPDATEDATE        = SYSDATE
           WHERE MD.A_IPCODE = V_TBL(I).IPCODE
             AND NVL(MD.A_ADDRESSMAILABLE, -1) != V_TBL(I).ADDRESSMAILABLE;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

        -- Update address1 and zip in ATS_MemberProactiveMerge table if there is address change
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE (SELECT MD.A_IPCODE          AS IPCODE,
                         MD.A_ADDRESSLINEONE  AS ADDRESS1,
                         MD.A_ADDRESSLINETWO  AS ADDRESS2,
                         MD.A_CITY            AS CITY,
                         MD.A_STATEORPROVINCE AS STATE,
                         MD.A_ZIPORPOSTALCODE AS ZIP,
                         MD.A_COUNTRY         AS COUNTRY,
                         PM.A_ADDRESSLINEONE  AS PMADDRESS1,
                         PM.A_ZIPCODE         AS PMZIP,
                         PM.A_CHANGEDBY       AS PMCHANGEDBY,
                         PM.UPDATEDATE        AS PMUPDATEDATE
                    FROM ATS_MEMBERDETAILS MD
                   INNER JOIN ATS_MEMBERPROACTIVEMERGE PM
                      ON MD.A_IPCODE = PM.A_IPCODE
                   WHERE PM.A_MEMBERSTATUS = 1) VW
             SET VW.PMADDRESS1 = LOWER(SUBSTR(REGEXP_REPLACE(TRIM(V_TBL(I)
                                                                  .ADDRESSLINE1),
                                                             '[-| |.|,|'']',
                                                             ''),
                                              1,
                                              7)),
                 -- PI 29639 - Use the first five digits of the incoming zip code from the file
                 -- vw.pmZip = lower(regexp_replace(trim(v_tbl(i).zip), '[-| |.|,|'']','')),
                 VW.PMZIP        = SUBSTR(LOWER(REGEXP_REPLACE(TRIM(V_TBL(I).ZIP),
                                                               '[-| |.|,|'']',
                                                               '')),
                                          0,
                                          5),
                 VW.PMCHANGEDBY  = 'Profile Updates Process - Addr',
                 VW.PMUPDATEDATE = SYSDATE
           WHERE VW.IPCODE = V_TBL(I).IPCODE
             AND V_TBL(I).STATE IS NOT NULL
             AND V_TBL(I).STATE IN (SELECT A_STATECODE FROM ATS_REFSTATES)
             AND V_TBL(I).ADDRESSLINE1 IS NOT NULL
             AND V_TBL(I).CITY IS NOT NULL
             AND V_TBL(I).ZIP IS NOT NULL
             AND (NVL(TRIM(VW.ADDRESS1), 'x') != V_TBL(I).ADDRESSLINE1 OR
                 (NVL(TRIM(VW.ADDRESS2), 'x') != V_TBL(I).ADDRESSLINE2 AND V_TBL(I)
                 .ADDRESSLINE2 IS NOT NULL) OR
                 NVL(TRIM(VW.CITY), 'x') != V_TBL(I).CITY OR
                 NVL(TRIM(VW.STATE), 'x') != V_TBL(I).STATE OR
                 NVL(TRIM(VW.ZIP), 'x') != V_TBL(I).ZIP OR
                 (NVL(TRIM(VW.COUNTRY), 'x') != V_TBL(I).COUNTRY AND V_TBL(I)
                 .COUNTRY IS NOT NULL));

        -- Then update address in ATS_MemberDetails table and commit
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ATS_MEMBERDETAILS MD
             SET MD.A_ADDRESSLINEONE  = V_TBL(I).ADDRESSLINE1,
                 MD.A_ADDRESSLINETWO  = V_TBL(I).ADDRESSLINE2,
                 MD.A_CITY            = V_TBL(I).CITY,
                 MD.A_STATEORPROVINCE = V_TBL(I).STATE,
                 MD.A_ZIPORPOSTALCODE = V_TBL(I).ZIP,
                 MD.A_COUNTRY         = V_TBL(I).COUNTRY,
                 MD.A_CHANGEDBY       = 'Profile Updates Process - Addr',
                 MD.UPDATEDATE        = SYSDATE
           WHERE MD.A_IPCODE = V_TBL(I).IPCODE
             AND V_TBL(I).STATE IS NOT NULL
             AND V_TBL(I).STATE IN (SELECT A_STATECODE FROM ATS_REFSTATES)
             AND V_TBL(I).ADDRESSLINE1 IS NOT NULL
             AND V_TBL(I).CITY IS NOT NULL
             AND V_TBL(I).ZIP IS NOT NULL
             AND (NVL(TRIM(MD.A_ADDRESSLINEONE), 'x') != V_TBL(I)
                 .ADDRESSLINE1 OR
                 (NVL(TRIM(MD.A_ADDRESSLINETWO), 'x') != V_TBL(I)
                 .ADDRESSLINE2 AND V_TBL(I).ADDRESSLINE2 IS NOT NULL) OR
                 NVL(TRIM(MD.A_CITY), 'x') != V_TBL(I).CITY OR
                 NVL(TRIM(MD.A_STATEORPROVINCE), 'x') != V_TBL(I).STATE OR
                 NVL(TRIM(MD.A_ZIPORPOSTALCODE), 'x') != V_TBL(I).ZIP OR
                 (NVL(TRIM(MD.A_COUNTRY), 'x') != V_TBL(I).COUNTRY AND V_TBL(I)
                 .COUNTRY IS NOT NULL));
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

        -- Update firstName and LastName in ATS_MemberProactiveMerge table if there is name change
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE (SELECT LM.IPCODE      AS IPCODE,
                         LM.FIRSTNAME   AS FIRSTNAME,
                         LM.LASTNAME    AS LASTNAME,
                         PM.A_FIRSTNAME AS PMFIRSTNAME,
                         PM.A_LASTNAME  AS PMLASTNAME,
                         PM.A_CHANGEDBY AS PMCHANGEDBY,
                         PM.UPDATEDATE  AS PMUPDATEDATE
                    FROM LW_LOYALTYMEMBER LM
                   INNER JOIN ATS_MEMBERPROACTIVEMERGE PM
                      ON LM.IPCODE = PM.A_IPCODE
                   WHERE PM.A_MEMBERSTATUS = 1) VW
             SET VW.PMFIRSTNAME  = LOWER(SUBSTR(REGEXP_REPLACE(TRIM(V_TBL(I)
                                                                    .FIRSTNAME),
                                                               '[ |.|,|'']',
                                                               ''),
                                                1,
                                                4)),
                 VW.PMLASTNAME   = LOWER(REGEXP_REPLACE(TRIM(V_TBL(I)
                                                             .LASTNAME),
                                                        '[ |.|,|'']',
                                                        '')),
                 VW.PMCHANGEDBY  = 'Profile Updates Process',
                 VW.PMUPDATEDATE = SYSDATE
           WHERE VW.IPCODE = V_TBL(I).IPCODE
             AND ((NVL(TRIM(VW.FIRSTNAME), 'x') != V_TBL(I).FIRSTNAME AND V_TBL(I)
                 .FIRSTNAME IS NOT NULL) OR
                 (NVL(TRIM(VW.LASTNAME), 'x') != V_TBL(I).LASTNAME AND V_TBL(I)
                 .LASTNAME IS NOT NULL));

        -- Then update names in LW_LoyaltyMember table and commit
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE LW_LOYALTYMEMBER LM
             SET LM.FIRSTNAME  = V_TBL(I).FIRSTNAME,
                 LM.LASTNAME   = V_TBL(I).LASTNAME,
                 LM.CHANGEDBY  = 'Profile Updates Process'
                 ,LM.Updatedate = SYSDATE
                 --,LM.CREATEDATE = SYSDATE  //Commented as per AEO-702- -----------------------SCJ
           WHERE LM.IPCODE = V_TBL(I).IPCODE
             AND ((NVL(TRIM(LM.FIRSTNAME), 'x') != V_TBL(I).FIRSTNAME AND V_TBL(I)
                 .FIRSTNAME IS NOT NULL) OR
                 (NVL(TRIM(LM.LASTNAME), 'x') != V_TBL(I).LASTNAME AND V_TBL(I)
                 .LASTNAME IS NOT NULL));
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

        -- Then update zip codes in LW_LoyaltyMember table and commit
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE LW_LOYALTYMEMBER LM
             SET LM.PRIMARYPOSTALCODE = V_TBL(I).ZIP,
                 LM.CHANGEDBY         = 'Profile Updates Process',
                 LM.Updatedate        = SYSDATE
               --  , LM.CREATEDATE        = SYSDATE //Commented as per AEO-702- -----------------------SCJ
           WHERE LM.IPCODE = V_TBL(I).IPCODE;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

        -- Then update LinkKey to the CID in LW_LoyaltyMember table and commit
        FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE LW_VIRTUALCARD VC
             SET VC.LINKKEY = V_TBL(I).CID
           WHERE VC.IPCODE = V_TBL(I).IPCODE;

        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

        EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;
    END;

    DECLARE
      /* check log file for errors */
      LV_ERR VARCHAR2(4000);
      LV_N   NUMBER;
    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM EXT_PROFILEUPDATES_LOG' || CHR(10) ||
                        'WHERE rec LIKE ''ORA-%'''
        INTO LV_N, LV_ERR;

      IF LV_N > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        V_MESSAGESFAILED := V_MESSAGESFAILED + LV_N;
        V_REASON         := 'Failed reads by external table';
        V_MESSAGE        := '<StageProc>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>EXT_ProfileUpdates' ||
                            CHR(10) ||
                            '    <Stage>LW_ProfileUpdates_Stage</Stage>' ||
                            CHR(10) || '    <FileName>' || P_FILENAME ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</StageProc>';
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => V_LOGSOURCE,
                            P_FILENAME  => P_FILENAME,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => LV_ERR,
                            P_TRYCOUNT  => LV_N,
                            P_MSGTIME   => SYSDATE);
      END IF;
    END;

    /* insert here */
    V_ENDTIME   := SYSDATE;
    V_JOBSTATUS := 1;

    /* log end of job */
    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => 'Stage-' || V_JOBNAME);

    OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;
  EXCEPTION
    WHEN DML_ERRORS THEN

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE);
        V_REASON         := 'Failed Records in Procedure Stage_ProfileUpdates01: ';
        V_MESSAGE        := ' ';

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

      END LOOP;
      COMMIT;
      V_ENDTIME        := SYSDATE;
      V_MESSAGESFAILED := V_MESSAGESFAILED;
      --V_ERROR          := SQLERRM;
      -- V_REASON         := 'Failed Procedure Stage_ProfileUpdates01: ';
      -- V_MESSAGE        := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
      --'    <pkg>Stage_ProfileUpdates</pkg>' || CHR(10) || '    <proc>Stage_ProfileUpdates01</proc>' || CHR(10) || '  </details>' || CHR(10) || '</failed>';

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);
    WHEN OTHERS THEN
      V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
      V_ENDTIME        := SYSDATE;
      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);

      V_ERROR   := SQLERRM;
      V_MESSAGE := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                   '    <pkg>Stage_ProfileUpdates</pkg>' || CHR(10) ||
                   '    <proc>Stage_ProfileUpdates01</proc>' || CHR(10) ||
                   '  </details>' || CHR(10) || '</failed>';
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

      RAISE;

  END STAGE_PROFILEUPDATES01;

END STAGE_PROFILEUPDATES;
/

