CREATE OR REPLACE PACKAGE AE_POINTSCONVERSION AS

  TYPE RCURSOR IS REF CURSOR;
  PROCEDURE CONVERTPILOTTOLEGACY(P_PROCESSDATE VARCHAR2,
                                 RETVAL        IN OUT RCURSOR);
  PROCEDURE CONVERTLEGACYTOPILOT(P_PROCESSDATE VARCHAR2,
                                 RETVAL        IN OUT RCURSOR);
  PROCEDURE CONVERTLEGACYTOPILOTONETIME;
  PROCEDURE CLEARWORKTABLES(p_Dummy VARCHAR2);
END AE_POINTSCONVERSION;
/

CREATE OR REPLACE PACKAGE BODY AE_POINTSCONVERSION IS

  PROCEDURE CONVERTPILOTTOLEGACY(P_PROCESSDATE VARCHAR2,
                                 RETVAL        IN OUT RCURSOR) IS

    V_CNT          NUMBER := 0;
    V_TXNSTARTDT   DATE;
    V_POINTTYPEID  NUMBER;
    V_POINTEVENTID NUMBER;
    V_CTRWDREDEEM  NUMBER := 0;
    V_REISSUEAMT   NUMBER := 0;
    V_PROCESSDATE  DATE := TO_DATE(P_PROCESSDATE, 'YYYYMMddHH24miss');
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);
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
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    --cursor1-
    CURSOR CUR_LEGACY IS
      SELECT distinct(VC.VCKEY) AS VCKEY, MD.A_IPCODE AS IPCODE, V_PROCESSDATE
        FROM ATS_MEMBERDETAILS MD
       INNER JOIN LW_VIRTUALCARD VC
          ON VC.IPCODE = MD.A_IPCODE
       LEFT JOIN AE_HISTPOINTCONVERSION PC
          ON PC.IPCODE  = MD.A_IPCODE AND TRUNC(PC.PROGRAMCHANGEDATE) = TRUNC(MD.A_PROGRAMCHANGEDATE)
       WHERE MD.A_EXTENDEDPLAYCODE = 2
         AND TRUNC(MD.A_PROGRAMCHANGEDATE) = TRUNC(V_PROCESSDATE) AND PC.IPCODE IS NULL;

    CURSOR CUR_PILOT_BONUS IS

      SELECT PT.VCKEY,
             SUM(case
                   when PT.POINTEVENTID =
                        (SELECT POINTEVENTID
                           FROM LW_POINTEVENT
                          WHERE NAME = 'AEO or Aerie Credit Card Bonus') then
                    PT.POINTS / 3
                   when PT.POINTEVENTID =
                        (SELECT POINTEVENTID
                           FROM LW_POINTEVENT
                          WHERE NAME = 'AEO or Aerie Credit Card Bonus Return') then
                    PT.POINTS / 3
                   when PT.POINTEVENTID =
                        (SELECT POINTEVENTID
                           FROM LW_POINTEVENT
                          WHERE NAME = 'AE SilverTier Purchase') then
                    pt.points / 4
                   when PT.POINTEVENTID =
                        (SELECT POINTEVENTID
                           FROM LW_POINTEVENT
                          WHERE NAME = 'AE SilverTier Return') then
                    pt.points / 4
                   when PT.POINTEVENTID =
                        (SELECT POINTEVENTID
                           FROM LW_POINTEVENT
                          WHERE NAME = 'AEO Visa Card World Purchase') then
                    pt.points
                 end) AS TOTALPOINTS
        FROM LW_POINTTRANSACTION PT
       INNER JOIN LW_VIRTUALCARD VC
          ON VC.VCKEY = PT.VCKEY
       INNER JOIN ATS_MEMBERDETAILS MD
          ON MD.A_IPCODE = VC.IPCODE
        LEFT JOIN AE_HISTPOINTCONVERSION PC
          ON PC.IPCODE = MD.A_IPCODE
         AND TRUNC(PC.PROGRAMCHANGEDATE) = TRUNC(MD.A_PROGRAMCHANGEDATE)
       WHERE 1 = 1
         AND (PT.POINTEVENTID =
             (SELECT POINTEVENTID
                 FROM LW_POINTEVENT
                WHERE NAME = 'AEO or Aerie Credit Card Bonus') OR
             PT.POINTEVENTID =
             (SELECT POINTEVENTID
                 FROM LW_POINTEVENT
                WHERE NAME = 'AEO or Aerie Credit Card Bonus Return') OR
             PT.POINTEVENTID =
             (SELECT POINTEVENTID
                 FROM LW_POINTEVENT
                WHERE NAME = 'AE SilverTier Purchase') OR
             PT.POINTEVENTID =
             (SELECT POINTEVENTID
                 FROM LW_POINTEVENT
                WHERE NAME = 'AE SilverTier Return') OR
             PT.POINTEVENTID =
             (SELECT POINTEVENTID
                 FROM LW_POINTEVENT
                WHERE NAME = 'AEO Visa Card World Purchase'))
         AND MD.A_EXTENDEDPLAYCODE = 2
         AND TRUNC(MD.A_PROGRAMCHANGEDATE) = TRUNC(V_PROCESSDATE)
         AND PC.IPCODE IS NULL
       GROUP BY PT.VCKEY ;

    TYPE T_TAB IS TABLE OF CUR_LEGACY%ROWTYPE;
    TYPE T_TAB2 IS TABLE OF CUR_PILOT_BONUS%ROWTYPE;
    V_TBL  T_TAB;
    V_TBL2 T_TAB2;
  BEGIN
/*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
 --  v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'CONVERTPILOTTOLEGACY';
     v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
 utility_pkg.Log_Process_Start(v_jobname, 'CONVERTPILOTTOLEGACY', v_processId);
 /*Logging*/

   SELECT TO_DATE(TO_CHAR(VALUE), 'mm/dd/yyyy')
      INTO V_TXNSTARTDT
      FROM LW_CLIENTCONFIGURATION CC
     WHERE CC.KEY = 'PilotStartDate';

    SELECT POINTTYPEID
      INTO V_POINTTYPEID
      FROM LW_POINTTYPE
     WHERE NAME = 'Bonus Points';
    SELECT POINTEVENTID
      INTO V_POINTEVENTID
      FROM LW_POINTEVENT PE
     WHERE NAME = 'Points Conversion';
    OPEN CUR_LEGACY;
    LOOP
      FETCH CUR_LEGACY BULK COLLECT
        INTO V_TBL LIMIT 1000;
      FORALL K IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        UPDATE LW_POINTTRANSACTION PT
           SET PT.EXPIRATIONDATE = V_PROCESSDATE,
               PT.NOTES          = PT.NOTES ||
                                   ' |Convert from Pilot to Legacy: previous rowkey = ' ||
                                   PT.ROWKEY,
               PT.ROWKEY         = NULL
         WHERE PT.TRANSACTIONDATE >= V_TXNSTARTDT
           AND PT.VCKEY = V_TBL(K).VCKEY;
      COMMIT;
      FORALL J IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_TXNHEADER_WRK
          (ROWKEY,
           HEADER_ROWKEY,
           VCKEY,
           SHIPDATE,
           ORDERNUMBER,
           TXNQUALPURCHASEAMT,
           TXNHEADERID,
           BRANDID,
           CREDITCARDID,
           TXNMASKID,
           TXNNUMBER,
           TXNDATE,
           TXNSTOREID,
           TXNTYPEID,
           TXNAMOUNT,
           TXNDISCOUNTAMOUNT,
           STORENUMBER,
           TXNEMPLOYEEID,
           TXNCHANNEL,
           TXNORIGINALTXNROWKEY,
           TXNCREDITSUSED,
           TXNREGISTERNUMBER,
           TXNORIGINALSTOREID,
           TXNORIGINALTXNDATE,
           TXNORIGINALTXNNUMBER,
           STATUSCODE,
           CREATEDATE,
           UPDATEDATE)
          SELECT A_ROWKEY,
                 A_ROWKEY,
                 A_VCKEY,
                 A_SHIPDATE,
                 A_ORDERNUMBER,
                 A_TXNQUALPURCHASEAMT,
                 A_TXNHEADERID,
                 A_BRANDID,
                 A_CREDITCARDID,
                 A_TXNMASKID,
                 A_TXNNUMBER,
                 A_TXNDATE,
                 A_TXNSTOREID,
                 A_TXNTYPEID,
                 A_TXNAMOUNT,
                 A_TXNDISCOUNTAMOUNT,
                 A_STORENUMBER,
                 A_TXNEMPLOYEEID,
                 A_TXNCHANNEL,
                 A_TXNORIGINALTXNROWKEY,
                 A_TXNCREDITSUSED,
                 A_TXNREGISTERNUMBER,
                 A_TXNORIGINALSTOREID,
                 A_TXNORIGINALTXNDATE,
                 A_TXNORIGINALTXNNUMBER,
                 STATUSCODE,
                 CREATEDATE,
                 UPDATEDATE
            FROM ATS_TXNHEADER
           WHERE A_VCKEY = V_TBL(J).VCKEY
             AND A_TXNDATE >= V_TXNSTARTDT;
      COMMIT;
      FORALL M IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_TXNDETAILITEM_WRK
          (ROWKEY,
           DTL_ROWKEY,
           IPCODE,
           PARENTROWKEY,
           TXNHEADERID,
           TXNDATE,
           TXNSTOREID,
           TXNDETAILID,
           DTLITEMLINENBR,
           DTLPRODUCTID,
           DTLTYPEID,
           DTLACTIONID,
           DTLRETAILAMOUNT,
           DTLSALEAMOUNT,
           DTLQUANTITY,
           DTLDISCOUNTAMOUNT,
           DTLCLEARANCEITEM,
           DTLCLASSCODE,
           STATUSCODE,
           CREATEDATE,
           UPDATEDATE)
          SELECT A_ROWKEY,
                 A_ROWKEY,
                 A_IPCODE,
                 A_PARENTROWKEY,
                 A_TXNHEADERID,
                 A_TXNDATE,
                 A_TXNSTOREID,
                 A_TXNDETAILID,
                 A_DTLITEMLINENBR,
                 A_DTLPRODUCTID,
                 A_DTLTYPEID,
                 A_DTLACTIONID,
                 A_DTLRETAILAMOUNT,
                 A_DTLSALEAMOUNT,
                 A_DTLQUANTITY,
                 A_DTLDISCOUNTAMOUNT,
                 A_DTLCLEARANCEITEM,
                 A_DTLCLASSCODE,
                 STATUSCODE,
                 CREATEDATE,
                 UPDATEDATE
            FROM ATS_TXNDETAILITEM
           WHERE A_IPCODE = V_TBL(M).VCKEY
             AND A_TXNDATE >= V_TXNSTARTDT;
      COMMIT;

      EXIT WHEN CUR_LEGACY%NOTFOUND;
    END LOOP;
    COMMIT;
    IF CUR_LEGACY%ISOPEN THEN
      CLOSE CUR_LEGACY;
    END IF;
    OPEN CUR_PILOT_BONUS;
    LOOP
      FETCH CUR_PILOT_BONUS BULK COLLECT
        INTO V_TBL2 LIMIT 10000;
      FORALL P IN 1 .. V_TBL2.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_POINTTRANSACTION
          (POINTTRANSACTIONID,
           VCKEY,
           POINTTYPEID,
           POINTEVENTID,
           TRANSACTIONTYPE,
           TRANSACTIONDATE,
           POINTAWARDDATE,
           POINTS,
           EXPIRATIONDATE,
           NOTES,
           OWNERTYPE,
           OWNERID,
           ROWKEY,
           PARENTTRANSACTIONID,
           POINTSCONSUMED,
           POINTSONHOLD,
           PTLOCATIONID,
           PTCHANGEDBY,
           CREATEDATE,
           EXPIRATIONREASON)
        VALUES
          (SEQ_POINTTRANSACTIONID.NEXTVAL,
           V_TBL2(P).VCKEY,
           V_POINTTYPEID /*Pointtypeid*/,
           V_POINTEVENTID /*pointeventid*/,
           1,
           V_PROCESSDATE, --txndate
           --SYSDATE, --pointawarddate
           V_PROCESSDATE,
           ROUND(V_TBL2(P).TOTALPOINTS, 0), /*Points*/
           ADD_MONTHS(TRUNC(V_PROCESSDATE, 'Q'), 3), --expiration date
           'PointConversion Points', /*Notes*/
           0,
           -1,
           -1,
           -1,
           0 /*Pointsconsumed*/,
           0 /*Pointsonhold*/,
           NULL,
           'AE_PointsConversion.ConvertPilotToLegacy' /*Ptchangedby*/,
           SYSDATE,
           NULL);
      COMMIT;
      EXIT WHEN CUR_PILOT_BONUS%NOTFOUND;
    END LOOP;
    COMMIT;
    IF CUR_PILOT_BONUS%ISOPEN THEN
      CLOSE CUR_PILOT_BONUS;
    END IF;
    COMMIT;
    FOR Y IN CUR_LEGACY LOOP
      SELECT COUNT(MR.ID)
        INTO V_CTRWDREDEEM
        FROM LW_MEMBERREWARDS MR
       WHERE MR.REDEMPTIONDATE IS NULL
         AND MR.MEMBERID = Y.IPCODE
         AND MR.DATEISSUED >= V_TXNSTARTDT
         AND NVL(MR.LWORDERNUMBER, 0) <> 3;
      If (V_CTRWDREDEEM > 0) then

      INSERT INTO LW_POINTTRANSACTION
        (POINTTRANSACTIONID,
         VCKEY,
         POINTTYPEID,
         POINTEVENTID,
         TRANSACTIONTYPE,
         TRANSACTIONDATE,
         POINTAWARDDATE,
         POINTS,
         EXPIRATIONDATE,
         NOTES,
         OWNERTYPE,
         OWNERID,
         ROWKEY,
         PARENTTRANSACTIONID,
         POINTSCONSUMED,
         POINTSONHOLD,
         PTLOCATIONID,
         PTCHANGEDBY,
         CREATEDATE,
         EXPIRATIONREASON)
      VALUES
        (SEQ_POINTTRANSACTIONID.NEXTVAL,
         Y.VCKEY,
         V_POINTTYPEID /*Pointtypeid*/,
         V_POINTEVENTID /*pointeventid*/,
         1 /*award points*/,
         Y.V_PROCESSDATE,
         Y.V_PROCESSDATE,
         Round(((1000 * V_CTRWDREDEEM)/5),0), /*Points= 1000 here*/
         ADD_MONTHS(TRUNC(Y.V_PROCESSDATE, 'Q'), 3),
         'Returned Reward Points', /*Notes*/
         0,
         -1,
         -1,
         -1,
         0 /*Pointsconsumed*/,
         0 /*Pointsonhold*/,
         NULL,
         'AE_PointsConversion.ConvertPilotToLegacy' /*Ptchangedby*/,
         SYSDATE,
         NULL);

      UPDATE LW_MEMBERREWARDS MR
         SET MR.REDEMPTIONDATE = Y.V_PROCESSDATE, MR.LWORDERNUMBER = 3
       WHERE MR.MEMBERID = Y.IPCODE and mr.redemptiondate is null;
       end if;
       COMMIT;
      /*Update the AIT FLAG */
      UPDATE ATS_MEMBERDETAILS MD
         SET MD.A_AITUPDATE = 1
       WHERE MD.A_IPCODE = Y.IPCODE;
        COMMIT;
      /*Expire the Member Tier Flag*/
      UPDATE LW_MEMBERTIERS MT
         SET MT.TODATE = Y.V_PROCESSDATE
       WHERE MT.MEMBERID = Y.IPCODE
         AND MT.TODATE = TO_DATE('12/31/2199', 'mm/dd/yyyy');
      COMMIT;
      INSERT INTO AE_HISTPOINTCONVERSION
        (IPCODE, PILOTFLAG, PROGRAMCHANGEDATE)
        SELECT MD1.A_IPCODE,
               MD1.A_EXTENDEDPLAYCODE,
               MD1.A_PROGRAMCHANGEDATE
          FROM ATS_MEMBERDETAILS MD1
         INNER JOIN LW_VIRTUALCARD VC1
            ON VC1.IPCODE = MD1.A_IPCODE
          LEFT JOIN AE_HISTPOINTCONVERSION PC1
            ON PC1.IPCODE = MD1.A_IPCODE
           AND TRUNC(PC1.PROGRAMCHANGEDATE) =
               TRUNC(MD1.A_PROGRAMCHANGEDATE)
         WHERE MD1.A_EXTENDEDPLAYCODE = 2
           AND TRUNC(MD1.A_PROGRAMCHANGEDATE) = TRUNC(V_PROCESSDATE)
           AND PC1.IPCODE IS NULL;
           COMMIT;
      V_CNT := V_CNT + 1;
      v_messagesreceived := v_messagesreceived + 1;
      IF MOD(V_CNT, 1000) = 0 THEN
        COMMIT;
      END IF;
    END LOOP;
    IF CUR_LEGACY%ISOPEN THEN
      CLOSE CUR_LEGACY;
    END IF;
    COMMIT;
    --END;
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
    WHEN DML_ERRORS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP

        V_ERROR          := SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE);
        V_REASON         := 'Failed Procedure ConvertPilotToLegacy: ';
        V_MESSAGE        := 'VCKEY: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .VCKEY || ' ' || 'ipcode: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .IPCODE;

        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'ConvertPilotToLegacy',
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);

      END LOOP;
         RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ConvertPilotToLegacy ');
    WHEN OTHERS THEN
      V_ERROR := SQLERRM;
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'ConvertPilotToLegacy',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ConvertPilotToLegacy ');
  END CONVERTPILOTTOLEGACY;

  PROCEDURE CONVERTLEGACYTOPILOT(P_PROCESSDATE VARCHAR2,
                                 RETVAL        IN OUT RCURSOR) IS

    V_CNT          NUMBER := 0;
    V_TXNSTARTDT   DATE;
    V_POINTTYPEID  NUMBER;
    V_POINTEVENTID NUMBER;
    V_CTRWDREDEEM  NUMBER := 0;
    V_REISSUEAMT   NUMBER := 0;
    V_PROCESSDATE  DATE := TO_DATE(P_PROCESSDATE, 'YYYYMMddHH24miss');
    V_BLUETIER     NUMBER := 0;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);
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
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;
    v_Ct_Tierid NUMBER := 0; --AEO-682

    --cursor1-
    CURSOR CUR_PILOT IS
      SELECT distinct(VC.VCKEY) AS VCKEY, MD.A_IPCODE AS IPCODE, V_PROCESSDATE
        FROM ATS_MEMBERDETAILS MD
       INNER JOIN LW_VIRTUALCARD VC
          ON VC.IPCODE = MD.A_IPCODE
        LEFT JOIN AE_HISTPOINTCONVERSION PC
          ON PC.IPCODE  = MD.A_IPCODE AND TRUNC(PC.PROGRAMCHANGEDATE) = TRUNC(MD.A_PROGRAMCHANGEDATE)
       WHERE MD.A_EXTENDEDPLAYCODE = 3
         AND TRUNC(MD.A_PROGRAMCHANGEDATE) = TRUNC(V_PROCESSDATE) AND PC.IPCODE IS NULL;
    TYPE T_TAB IS TABLE OF CUR_PILOT%ROWTYPE;
    V_TBL T_TAB;
  BEGIN
/*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
 --  v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'CONVERTLEGACYTOPILOT';
     v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
 utility_pkg.Log_Process_Start(v_jobname, 'CONVERTLEGACYTOPILOT', v_processId);
 /*Logging*/
 -- AEO584/586 coding change ------------------SCJ
 /*
    SELECT TO_DATE(TO_CHAR(VALUE), 'mm/dd/yyyy')
      INTO V_TXNSTARTDT
      FROM LW_CLIENTCONFIGURATION CC
     WHERE CC.KEY = 'PilotStartDate';
     */
     SELECT  Trunc(V_PROCESSDATE, 'Q')  INTO V_TXNSTARTDT from dual ;
     -- AEO584/586 coding change ------------------SCJ

    SELECT POINTTYPEID
      INTO V_POINTTYPEID
      FROM LW_POINTTYPE
     WHERE NAME = 'Bonus Points';
    SELECT POINTEVENTID
      INTO V_POINTEVENTID
      FROM LW_POINTEVENT PE
     WHERE NAME = 'Returned Reward Points';
    SELECT TR.TIERID
      INTO V_BLUETIER
      FROM LW_TIERS TR
     WHERE TR.TIERNAME = 'Blue';
    OPEN CUR_PILOT;
    LOOP
      FETCH CUR_PILOT BULK COLLECT
        INTO V_TBL LIMIT 1000;
      FORALL K IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        UPDATE LW_POINTTRANSACTION PT
           SET PT.EXPIRATIONDATE = V_PROCESSDATE, --to_date('12/31/2199','mm/dd/yyyy'),
               PT.NOTES          = PT.NOTES ||
                                   ' |Convert from Legacy to Pilot: previous rowkey = ' ||
                                   PT.ROWKEY,
               PT.ROWKEY         = NULL
         WHERE PT.TRANSACTIONDATE >= V_TXNSTARTDT
              -- ADD_MONTHS(TRUNC(V_TBL(K).V_PROCESSDATE, 'Q'), -3)
           AND PT.VCKEY = V_TBL(K).VCKEY;
      COMMIT;
      FORALL J IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_TXNHEADER_WRK
          (ROWKEY,
           HEADER_ROWKEY,
           VCKEY,
           SHIPDATE,
           ORDERNUMBER,
           TXNQUALPURCHASEAMT,
           TXNHEADERID,
           BRANDID,
           CREDITCARDID,
           TXNMASKID,
           TXNNUMBER,
           TXNDATE,
           TXNSTOREID,
           TXNTYPEID,
           TXNAMOUNT,
           TXNDISCOUNTAMOUNT,
           STORENUMBER,
           TXNEMPLOYEEID,
           TXNCHANNEL,
           TXNORIGINALTXNROWKEY,
           TXNCREDITSUSED,
           TXNREGISTERNUMBER,
           TXNORIGINALSTOREID,
           TXNORIGINALTXNDATE,
           TXNORIGINALTXNNUMBER,
           STATUSCODE,
           CREATEDATE,
           UPDATEDATE)
          SELECT A_ROWKEY,
                 A_ROWKEY,
                 A_VCKEY,
                 A_SHIPDATE,
                 A_ORDERNUMBER,
                 A_TXNQUALPURCHASEAMT,
                 A_TXNHEADERID,
                 A_BRANDID,
                 A_CREDITCARDID,
                 A_TXNMASKID,
                 A_TXNNUMBER,
                 A_TXNDATE,
                 A_TXNSTOREID,
                 A_TXNTYPEID,
                 A_TXNAMOUNT,
                 A_TXNDISCOUNTAMOUNT,
                 A_STORENUMBER,
                 A_TXNEMPLOYEEID,
                 A_TXNCHANNEL,
                 A_TXNORIGINALTXNROWKEY,
                 A_TXNCREDITSUSED,
                 A_TXNREGISTERNUMBER,
                 A_TXNORIGINALSTOREID,
                 A_TXNORIGINALTXNDATE,
                 A_TXNORIGINALTXNNUMBER,
                 STATUSCODE,
                 CREATEDATE,
                 UPDATEDATE
            FROM ATS_TXNHEADER
           WHERE A_VCKEY = V_TBL(J).VCKEY
             AND A_TXNDATE >= V_TXNSTARTDT;
                -- ADD_MONTHS(TRUNC(V_TBL(J).V_PROCESSDATE, 'Q'), -3);
      COMMIT;
      FORALL M IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_TXNDETAILITEM_WRK
          (ROWKEY,
           DTL_ROWKEY,
           IPCODE,
           PARENTROWKEY,
           TXNHEADERID,
           TXNDATE,
           TXNSTOREID,
           TXNDETAILID,
           DTLITEMLINENBR,
           DTLPRODUCTID,
           DTLTYPEID,
           DTLACTIONID,
           DTLRETAILAMOUNT,
           DTLSALEAMOUNT,
           DTLQUANTITY,
           DTLDISCOUNTAMOUNT,
           DTLCLEARANCEITEM,
           DTLCLASSCODE,
           STATUSCODE,
           CREATEDATE,
           UPDATEDATE)
          SELECT A_ROWKEY,
                 A_ROWKEY,
                 A_IPCODE,
                 A_PARENTROWKEY,
                 A_TXNHEADERID,
                 A_TXNDATE,
                 A_TXNSTOREID,
                 A_TXNDETAILID,
                 A_DTLITEMLINENBR,
                 A_DTLPRODUCTID,
                 A_DTLTYPEID,
                 A_DTLACTIONID,
                 A_DTLRETAILAMOUNT,
                 A_DTLSALEAMOUNT,
                 A_DTLQUANTITY,
                 A_DTLDISCOUNTAMOUNT,
                 A_DTLCLEARANCEITEM,
                 A_DTLCLASSCODE,
                 STATUSCODE,
                 CREATEDATE,
                 UPDATEDATE
            FROM ATS_TXNDETAILITEM
           WHERE A_IPCODE = V_TBL(M).VCKEY
             AND A_TXNDATE >= V_TXNSTARTDT;
                -- ADD_MONTHS(TRUNC(V_TBL(M).V_PROCESSDATE, 'Q'), -3);
      COMMIT;

      EXIT WHEN CUR_PILOT%NOTFOUND;
    END LOOP;
    COMMIT;
    IF CUR_PILOT%ISOPEN THEN
      CLOSE CUR_PILOT;
    END IF;
    FOR Y IN CUR_PILOT LOOP

      /*Update the AIT FLAG AND RESET NETSPEND*/
      UPDATE ATS_MEMBERDETAILS MD
         SET MD.A_AITUPDATE = 1,
             MD.A_NETSPEND = 0, -- AEO-559 BEGIN - END
             -- AEO-599 BEGIN
              MD.A_NEXTEMAILREMINDERDATE = CASE
                                                WHEN MD.A_PENDINGEMAILVERIFICATION IS NULL THEN  TRUNC(SYSDATE+1)
                                                WHEN  MD.A_PENDINGEMAILVERIFICATION IS NOT NULL THEN MD.A_NEXTEMAILREMINDERDATE
                                             END  ,
             MD.A_PENDINGCELLVERIFICATION= CASE
                                                WHEN  MD.A_PENDINGCELLVERIFICATION IS NULL THEN  1
                                                WHEN  MD.A_PENDINGCELLVERIFICATION IS NOT NULL THEN   MD.A_PENDINGCELLVERIFICATION
                                             END ,
             MD.A_PENDINGEMAILVERIFICATION= CASE
                                                WHEN  MD.A_PENDINGEMAILVERIFICATION IS NULL THEN  1
                                                WHEN  MD.A_PENDINGEMAILVERIFICATION IS NOT NULL THEN   MD.A_PENDINGEMAILVERIFICATION
                                             END
             -- AEO-599 END
       WHERE MD.A_IPCODE = Y.IPCODE;
      /*Insert a blue Tier*/
      /*AEO-682: first we need to validate if the tier doesn't exist already so we don't duplicate the record*/
        SELECT COUNT(Mt.Tierid)
        INTO v_Ct_Tierid
        FROM Lw_Membertiers Mt
        WHERE Mt.Memberid = y.Ipcode
        AND Trunc(Mt.Todate) = To_Date('12/31/2199', 'mm/dd/yyyy');

        IF v_Ct_Tierid = 0 THEN
      /*AEO-682: END*/
      INSERT INTO LW_MEMBERTIERS
        (ID,
         TIERID,
         MEMBERID,
         FROMDATE,
         TODATE,
         DESCRIPTION,
         CREATEDATE,
         UPDATEDATE)
      VALUES
        (HIBERNATE_SEQUENCE.NEXTVAL,
         V_BLUETIER,
         Y.IPCODE,
         Y.V_PROCESSDATE,
         TO_DATE('12/31/2199 ', 'mm/dd/yyyy'),
         'Qualifier',
         Y.V_PROCESSDATE,
         Y.V_PROCESSDATE);
         END IF;   --AEO-682
      COMMIT;
       INSERT INTO AE_HISTPOINTCONVERSION
        (IPCODE, PILOTFLAG, PROGRAMCHANGEDATE)
        SELECT  MD1.A_IPCODE,MD1.A_EXTENDEDPLAYCODE,MD1.A_PROGRAMCHANGEDATE
        FROM ATS_MEMBERDETAILS MD1
       INNER JOIN LW_VIRTUALCARD VC1
          ON VC1.IPCODE = MD1.A_IPCODE
        LEFT JOIN AE_HISTPOINTCONVERSION PC1
          ON PC1.IPCODE  = MD1.A_IPCODE AND TRUNC(PC1.PROGRAMCHANGEDATE) = TRUNC(MD1.A_PROGRAMCHANGEDATE)
       WHERE MD1.A_EXTENDEDPLAYCODE = 3
         AND TRUNC(MD1.A_PROGRAMCHANGEDATE) = TRUNC(V_PROCESSDATE) AND PC1.IPCODE IS NULL;
         COMMIT;
      V_CNT := V_CNT + 1;
      v_messagesreceived := v_messagesreceived + 1;
      IF MOD(V_CNT, 1000) = 0 THEN
        COMMIT;
      END IF;
    END LOOP;
    IF CUR_PILOT%ISOPEN THEN
      CLOSE CUR_PILOT;
    END IF;
    COMMIT;
    --END;
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
    WHEN DML_ERRORS THEN
 V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE);
        V_REASON         := 'Failed Procedure ConvertLegacyToPilot: ';
        V_MESSAGE        := 'VCKEY: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .VCKEY || ' ' || 'ipcode: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .IPCODE;

        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'ConvertLegacyToPilot',
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);

      END LOOP;

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ConvertLegacyToPilot ');
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
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
                          P_LOGSOURCE => 'ConvertLegacyToPilot',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ConvertLegacyToPilot ');
  END CONVERTLEGACYTOPILOT;

  PROCEDURE CONVERTLEGACYTOPILOTONETIME IS

  v_ddl VARCHAR2(1200) := 'CREATE TABLE x$_pointtransaction AS
                   select ptran.*
                     from lw_pointtransaction ptran
                     join lw_pointtype ptype on ptran.pointtypeid = ptype.pointtypeid
                     join lw_pointevent pevent on ptran.pointeventid = pevent.pointeventid
                     inner join lw_virtualcard vc on vc.vckey = ptran.vckey
                     inner join ats_memberdetails md on md.a_ipcode = vc.ipcode
                       where ptype.name in (  ''Basic Points'', ''Adjustment Points'', ''CS Points'', ''Bonus Points'', ''Adjustment Bonus Points'' )
                       and md.a_extendedplaycode in (1, 3)
                       and ptran.transactiondate between to_date(''09/30/15'', ''mm/dd/yy'') and to_date(''10/06/15'', ''mm/dd/yy'')
                       and rowkey is not null';

  /*
  * Create temp table of all legacy points that need to be converted over to pilot.
  */
  --cursor1-
  CURSOR CUR_LEGACY IS
         select ptran.rowkey AS ROWKEY, ptran.pointtransactionid AS POINTTRANSACTIONID, vc.vckey AS VCKEY, vc.ipcode AS IPCODE
           from lw_pointtransaction ptran
           join lw_pointtype ptype on ptran.pointtypeid = ptype.pointtypeid
           join lw_pointevent pevent on ptran.pointeventid = pevent.pointeventid
           inner join lw_virtualcard vc on vc.vckey = ptran.vckey
           inner join ats_memberdetails md on md.a_ipcode = vc.ipcode
             where ptype.name in (  'Basic Points', 'Adjustment Points', 'CS Points', 'Bonus Points', 'Adjustment Bonus Points' )
             and md.a_extendedplaycode in (1, 3)
             and ptran.transactiondate between to_date('09/30/15', 'mm/dd/yy') and to_date('10/06/15', 'mm/dd/yy')
             and rowkey is not null;

    TYPE T_TAB IS TABLE OF CUR_LEGACY%ROWTYPE;
    V_TBL  T_TAB;
    V_TEMP_TABLE int;

    CURSOR CUR_TEMP_TABLE IS
          select count(*) from user_tables where table_name = upper('x$_pointtransaction');

  BEGIN

  OPEN CUR_TEMP_TABLE;
  FETCH CUR_TEMP_TABLE
          INTO V_TEMP_TABLE;

  if V_TEMP_TABLE = 1 then
      execute immediate 'drop table x$_pointtransaction';
  end if;

  EXECUTE IMMEDIATE v_ddl;

    /*
    *  Lets Expire all the points from the temp table we created.
    */

    OPEN CUR_LEGACY;
    LOOP
      FETCH CUR_LEGACY BULK COLLECT
        INTO V_TBL LIMIT 1000;
      FORALL K IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
          UPDATE LW_POINTTRANSACTION PT
             SET PT.EXPIRATIONDATE = SYSDATE,
                 PT.NOTES          = PT.NOTES ||
                                     ' |Convert from Legacy to Pilot: previous rowkey = ' ||
                                     PT.ROWKEY,
                 PT.ROWKEY         = NULL
           WHERE PT.POINTTRANSACTIONID = V_TBL(K).POINTTRANSACTIONID
             AND PT.VCKEY = V_TBL(K).VCKEY;
      COMMIT;

      /*
      * loop through them all the items in my temp table and put them into the work table
      * where rowkey is in the temp table
      */

      FORALL J IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_TXNHEADER_WRK
          (ROWKEY,
           HEADER_ROWKEY,
           VCKEY,
           SHIPDATE,
           ORDERNUMBER,
           TXNQUALPURCHASEAMT,
           TXNHEADERID,
           BRANDID,
           CREDITCARDID,
           TXNMASKID,
           TXNNUMBER,
           TXNDATE,
           TXNSTOREID,
           TXNTYPEID,
           TXNAMOUNT,
           TXNDISCOUNTAMOUNT,
           STORENUMBER,
           TXNEMPLOYEEID,
           TXNCHANNEL,
           TXNORIGINALTXNROWKEY,
           TXNCREDITSUSED,
           TXNREGISTERNUMBER,
           TXNORIGINALSTOREID,
           TXNORIGINALTXNDATE,
           TXNORIGINALTXNNUMBER,
           STATUSCODE,
           CREATEDATE,
           UPDATEDATE)
          SELECT A_ROWKEY,
                 A_ROWKEY,
                 A_VCKEY,
                 A_SHIPDATE,
                 A_ORDERNUMBER,
                 A_TXNQUALPURCHASEAMT,
                 A_TXNHEADERID,
                 A_BRANDID,
                 A_CREDITCARDID,
                 A_TXNMASKID,
                 A_TXNNUMBER,
                 A_TXNDATE,
                 A_TXNSTOREID,
                 A_TXNTYPEID,
                 A_TXNAMOUNT,
                 A_TXNDISCOUNTAMOUNT,
                 A_STORENUMBER,
                 A_TXNEMPLOYEEID,
                 A_TXNCHANNEL,
                 A_TXNORIGINALTXNROWKEY,
                 A_TXNCREDITSUSED,
                 A_TXNREGISTERNUMBER,
                 A_TXNORIGINALSTOREID,
                 A_TXNORIGINALTXNDATE,
                 A_TXNORIGINALTXNNUMBER,
                 STATUSCODE,
                 CREATEDATE,
                 UPDATEDATE
            FROM ATS_TXNHEADER
           WHERE A_ROWKEY = V_TBL(J).ROWKEY;

      COMMIT;

      /*
      * Do the same for the details records
      * where A_PARENTROWKEY is the rowkey in the temp table.
      */

      FORALL M IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS
        INSERT INTO LW_TXNDETAILITEM_WRK
          (ROWKEY,
           DTL_ROWKEY,
           IPCODE,
           PARENTROWKEY,
           TXNHEADERID,
           TXNDATE,
           TXNSTOREID,
           TXNDETAILID,
           DTLITEMLINENBR,
           DTLPRODUCTID,
           DTLTYPEID,
           DTLACTIONID,
           DTLRETAILAMOUNT,
           DTLSALEAMOUNT,
           DTLQUANTITY,
           DTLDISCOUNTAMOUNT,
           DTLCLEARANCEITEM,
           DTLCLASSCODE,
           STATUSCODE,
           CREATEDATE,
           UPDATEDATE)
          SELECT A_ROWKEY,
                 A_ROWKEY,
                 A_IPCODE,
                 A_PARENTROWKEY,
                 A_TXNHEADERID,
                 A_TXNDATE,
                 A_TXNSTOREID,
                 A_TXNDETAILID,
                 A_DTLITEMLINENBR,
                 A_DTLPRODUCTID,
                 A_DTLTYPEID,
                 A_DTLACTIONID,
                 A_DTLRETAILAMOUNT,
                 A_DTLSALEAMOUNT,
                 A_DTLQUANTITY,
                 A_DTLDISCOUNTAMOUNT,
                 A_DTLCLEARANCEITEM,
                 A_DTLCLASSCODE,
                 STATUSCODE,
                 CREATEDATE,
                 UPDATEDATE
            FROM ATS_TXNDETAILITEM
           WHERE A_PARENTROWKEY = V_TBL(M).ROWKEY;

      COMMIT;

      EXIT WHEN CUR_LEGACY%NOTFOUND;
    END LOOP;
    COMMIT;
    IF CUR_LEGACY%ISOPEN THEN
      CLOSE CUR_LEGACY;
    END IF;
  END CONVERTLEGACYTOPILOTONETIME;

   PROCEDURE CLEARWORKTABLES (p_Dummy VARCHAR2) AS
     BEGIN
    EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk' ; -- clear work table before next insert
    EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_Wrk' ; -- clear work table before next insert
     END CLEARWORKTABLES;
END AE_POINTSCONVERSION;
/

