CREATE OR REPLACE PACKAGE PROGRAMCONVERSIONMERGE IS
	type rcursor IS REF CURSOR;

  PROCEDURE CONVERTONEPILOTTOLEGACY(P_PROCESSDATE DATE, LOYALTY_NUMBER VARCHAR2);

  PROCEDURE CONVERTONELEGACYTOPILOT(P_PROCESSDATE DATE, LOYALTY_NUMBER VARCHAR2);

  PROCEDURE LOAD_PROGRAMCONVERSIONMERGE(p_filename VARCHAR2, p_processDate VARCHAR2);

  PROCEDURE clear_infile(p_tablename VARCHAR2);

  PROCEDURE initialize(p_filename VARCHAR2);

  PROCEDURE PROCESS_PROGCONVMERGE(p_processDate DATE);

END PROGRAMCONVERSIONMERGE;
/

CREATE OR REPLACE PACKAGE BODY PROGRAMCONVERSIONMERGE IS

  PROCEDURE CONVERTONEPILOTTOLEGACY(P_PROCESSDATE DATE, LOYALTY_NUMBER VARCHAR2) IS

    V_CNT          NUMBER := 0;
    V_TXNSTARTDT   DATE;
    V_POINTTYPEID  NUMBER;
    V_POINTEVENTID NUMBER;
    V_CTRWDREDEEM  NUMBER := 0;
    V_REISSUEAMT   NUMBER := 0;
    --V_PROCESSDATE  DATE := TO_DATE(P_PROCESSDATE, 'YYYYMMddHH24miss');
    V_PROCESSDATE  DATE := P_PROCESSDATE;
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
       WHERE MD.A_EXTENDEDPLAYCODE IN (1,3) AND VC.LOYALTYIDNUMBER = LOYALTY_NUMBER
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
         AND MD.A_EXTENDEDPLAYCODE IN (1,3)
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

     v_jobname := 'CONVERTONEPILOTTOLEGACY';
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
 utility_pkg.Log_Process_Start(v_jobname, 'CONVERTONEPILOTTOLEGACY', v_processId);
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
		     AND A_TXNTYPEID <> 6 -- AEO-757
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
		     AND A_DTLTYPEID <> 6 -- AEO-757
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
           'PROGRAMCONVERSIONMERGE.ConvertOnePilotToLegacy' /*Ptchangedby*/,
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
         'PROGRAMCONVERSIONMERGE.ConvertOnePilotToLegacy' /*Ptchangedby*/,
         SYSDATE,
         NULL);

      UPDATE LW_MEMBERREWARDS MR
         SET MR.REDEMPTIONDATE = Y.V_PROCESSDATE, MR.LWORDERNUMBER = 3
       WHERE MR.MEMBERID = Y.IPCODE and mr.redemptiondate is null;
       end if;
       COMMIT;
      /*Update the AIT FLAG */
      UPDATE ATS_MEMBERDETAILS MD
         SET MD.A_AITUPDATE = 1,
		     MD.A_EXTENDEDPLAYCODE = 2 -- mmv
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
         WHERE MD1.A_EXTENDEDPLAYCODE IN (1,3)
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
        V_REASON         := 'Failed Procedure ConvertOnePilotToLegacy: ';
        V_MESSAGE        := 'VCKEY: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .VCKEY || ' ' || 'ipcode: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .IPCODE;

        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'ConvertOnePilotToLegacy',
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
                              'Other Exception detected in ConvertOnePilotToLegacy ');
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
                          P_LOGSOURCE => 'ConvertOnePilotToLegacy',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ConvertOnePilotToLegacy ');
  END CONVERTONEPILOTTOLEGACY;

  PROCEDURE CONVERTONELEGACYTOPILOT(P_PROCESSDATE DATE, LOYALTY_NUMBER VARCHAR2) IS

    V_CNT          NUMBER := 0;
    V_TXNSTARTDT   DATE;
    V_POINTTYPEID  NUMBER;
    V_POINTEVENTID NUMBER;
    V_CTRWDREDEEM  NUMBER := 0;
    V_REISSUEAMT   NUMBER := 0;
    V_PROCESSDATE  DATE := P_PROCESSDATE;
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
       WHERE (MD.A_EXTENDEDPLAYCODE NOT IN (1,3) OR MD.A_EXTENDEDPLAYCODE IS NULL) AND VC.LOYALTYIDNUMBER = LOYALTY_NUMBER
         AND TRUNC(MD.A_PROGRAMCHANGEDATE) = TRUNC(V_PROCESSDATE) AND PC.IPCODE IS NULL;
    TYPE T_TAB IS TABLE OF CUR_PILOT%ROWTYPE;
    V_TBL T_TAB;
  BEGIN
/*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
 --  v_dap_log_id := utility_pkg.get_LIBJobID();

     v_jobname := 'CONVERTONELEGACYTOPILOT';
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
 utility_pkg.Log_Process_Start(v_jobname, 'CONVERTONELEGACYTOPILOT', v_processId);
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
		     MD.A_EXTENDEDPLAYCODE = 3, -- mmv
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
       WHERE MD1.A_EXTENDEDPLAYCODE NOT IN (1,3)
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
        V_REASON         := 'Failed Procedure ConvertOneLegacyToPilot: ';
        V_MESSAGE        := 'VCKEY: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .VCKEY || ' ' || 'ipcode: ' || V_TBL(SQL%BULK_EXCEPTIONS(INDX).ERROR_INDEX)
                           .IPCODE;

        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => 'ConvertOneLegacyToPilot',
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
                              'Other Exception detected in ConvertOneLegacyToPilot ');
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
                          P_LOGSOURCE => 'ConvertOneLegacyToPilot',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ConvertOneLegacyToPilot ');
  END CONVERTONELEGACYTOPILOT;

  PROCEDURE LOAD_PROGRAMCONVERSIONMERGE(p_filename IN VARCHAR2, p_processDate IN VARCHAR2) IS

    v_processid    NUMBER := 0;
    v_my_log_id    NUMBER :=0;
    v_dap_log_id   NUMBER :=0;
    --log job attributes
    v_jobdirection     NUMBER := 0;
    v_filename         VARCHAR2(512) := p_filename;
    v_starttime        DATE := SYSDATE;
    v_endtime          DATE;
    v_messagesreceived NUMBER := 0;
    v_messagesfailed   NUMBER := 0;
    v_jobstatus        NUMBER := 0;
    v_jobname          VARCHAR2(256);
    v_messageid VARCHAR2(256);
    v_logsource VARCHAR2(256);
    v_batchid   VARCHAR2(256) := 0;
    v_message   VARCHAR2(256);
    v_reason    VARCHAR2(256);
    v_error     VARCHAR2(256);
    v_trycount  NUMBER := 0;
    v_processDate DATE := To_Date(p_processDate,'MM/DD/YYYY HH:MI:SS AM');
    v_envkey    VARCHAR2(256) := '';
    lv_err VARCHAR2(4000);
    lv_n   NUMBER;

   BEGIN
    v_my_log_id := utility_pkg.get_LIBJobID();
    v_dap_log_id := utility_pkg.get_LIBJobID();
    v_jobname       := 'AE ProgramConversionMerge - DB version';
    v_logsource     := v_jobname;
    v_processid := 1;
    v_envkey  := 'BP_AE@' || UPPER(sys_context('userenv','instance_name'));

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
    IF UPPER(TRIM(p_filename)) LIKE UPPER('AE_Mergeconversion%.TXT%') THEN
     --  processing the file
     /* initialize, truncates set external table to read p_filename */
      initialize( p_filename);
      /* reset log file, read later for errors */
      clear_infile('EXT_PROGCONVERSIONMERGE_LOG');
      PROCESS_PROGCONVMERGE(v_processDate);

    ELSE

      raise_application_error(-20001, 'Unrecognized file name');
    END IF;

    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM EXT_PROGCONVERSIONMERGE_LOG' ||
                        CHR(10) || 'WHERE rec LIKE ''ORA-%'''
        INTO lv_n, lv_err;

      IF lv_n > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        v_messagesfailed := v_messagesfailed + lv_n;
        v_reason         := 'Failed reads by external table';
        v_message        := '<ProgrameConversionMerge>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>EXT_PROGCONVERSIONMERGE' ||
                            '</External>' || '    <FileName>' || p_filename ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</ProgrameConversionMerge>';
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
                          p_jobname          => 'stage' || v_jobname);
      v_messagesfailed := v_messagesfailed + 1;
      v_error          := SQLERRM;
      v_reason         := 'Failed Procedure ProgrameConversionMerge';
      v_message        := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                          '    <pkg>AEJOBS</pkg>' || CHR(10) ||
                          '    <proc>ProgrameConversionMerge</proc>' || CHR(10) ||
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
  END LOAD_PROGRAMCONVERSIONMERGE;

  PROCEDURE initialize( p_filename VARCHAR2) IS
    v_partition_name VARCHAR2(256);
    v_sql            VARCHAR2(4000);
    v_inst           VARCHAR2(64) := upper(sys_context('userenv',
                                                       'instance_name'));
  BEGIN
    /*              set the external table filename                                      */
    v_sql := 'ALTER TABLE EXT_PROGRAMECONVERSIONMERGE' || CHR(10) ||
             'LOCATION (AE_IN' || CHR(58) || '''' || p_filename || ''')';
    EXECUTE IMMEDIATE v_sql;
  END initialize;

  PROCEDURE clear_infile(p_tablename IN VARCHAR2) IS

    lv_String     VARCHAR2(32000);
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

PROCEDURE PROCESS_PROGCONVMERGE(p_processDate IN DATE) AS
  /* Stored procedure will determine which among the two loyaltyids in the file from AE is primary and the populate
  the table ats_programeconversionmerge(criteria mentioned in Jira).The merged date would the sysdate.
  This stored procedure will also replicate what pointconversion is doing and execute the same staging steps
  */
  v_Found BOOLEAN := False;
  v_extendedplaycode_1 NUMBER :=0;
  v_extendedplaycode_2 NUMBER :=0;
  v_CID_1 NUMBER :=0;
  v_CID_2 NUMBER :=0;
  v_LID_1 VARCHAR2(255) := '0';
  v_LID_2 VARCHAR2(255) := '0';
  v_IPC_2 NUMBER :=0;

  v_primaryLID VARCHAR2(255) := '0';
  v_PrimaryCID NUMBER :=0;
  v_Primaryextendedplaycode NUMBER :=0;
  v_SecondaryLID VARCHAR2(255) := '0';
  v_SecondaryCID NUMBER :=0;
  v_Secondaryextendedplaycode NUMBER :=0;
  v_exist1 number :=0;
  v_exist2 number :=0;

  v_headeritemwkcount NUMBER := 0;
  v_detailitemwkcount NUMBER := 0;

  crsr1 PROGRAMCONVERSIONMERGE.Rcursor;
  crsr2 PROGRAMCONVERSIONMERGE.Rcursor;
  crsr3 PROGRAMCONVERSIONMERGE.Rcursor;
  crsr4 PROGRAMCONVERSIONMERGE.Rcursor;
  V_DUMMY VARCHAR2(256) := ' ';

  /*Get the LIDs from external table*/
  BEGIN
    DECLARE
    CURSOR GET_DATA IS
    SELECT epc.loyaltyid_1, epc.loyaltyid_2 FROM EXT_PROGRAMECONVERSIONMERGE epc;

    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
    V_TBL T_TAB; ---<------ our arry object
    BEGIN
    OPEN GET_DATA;
    LOOP
      FETCH GET_DATA BULK COLLECT INTO V_TBL LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
      FOR I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
      LOOP
        v_Found := False;
        v_exist1 :=0;
        v_exist2 :=0;

        SELECT count(*)
        INTO v_exist1
        FROM bp_ae.ats_memberdetails md
        Inner join bp_ae.lw_virtualcard vc on vc.ipcode = md.a_ipcode
        WHERE vc.loyaltyidnumber = V_TBL(I).loyaltyid_1;

        SELECT count(*)
        INTO v_exist2
        FROM bp_ae.ats_memberdetails md, bp_ae.lw_virtualcard vc
        WHERE md.a_ipcode = vc.ipcode AND vc.loyaltyidnumber = V_TBL(I).loyaltyid_2;

        IF (v_exist1 <> 0 and v_exist2 <> 0) THEN
            /*Find extended play code*/
            SELECT (CASE WHEN md.a_extendedplaycode IS NULL THEN 0 ELSE md.a_extendedplaycode END),
            (CASE WHEN vc.linkkey IS NULL THEN 0 ELSE vc.linkkey END),
            vc.loyaltyidnumber
            INTO v_extendedplaycode_1, v_CID_1, v_LID_1
            FROM bp_ae.ats_memberdetails md
            Inner join bp_ae.lw_virtualcard vc on vc.ipcode = md.a_ipcode
            WHERE vc.loyaltyidnumber = V_TBL(I).loyaltyid_1;

            SELECT (CASE WHEN md.a_extendedplaycode IS NULL THEN 0 ELSE md.a_extendedplaycode END),
            (CASE WHEN vc.linkkey IS NULL THEN 0 ELSE vc.linkkey END),
            vc.loyaltyidnumber,
            md.a_ipcode
            INTO v_extendedplaycode_2, v_CID_2, v_LID_2,v_IPC_2
            FROM bp_ae.ats_memberdetails md, bp_ae.lw_virtualcard vc
            WHERE md.a_ipcode = vc.ipcode AND vc.loyaltyidnumber = V_TBL(I).loyaltyid_2;

            --When merging 2 accounts, accept 2 loyalty IDs. The ID in the first position should be the primary.
            --When submitting IDs to be merged, the Primary will be declared in the text file provided to development
            v_primaryLID                := v_LID_1;
            v_PrimaryCID                := v_CID_1;
            v_Primaryextendedplaycode   := v_extendedplaycode_1;
            v_SecondaryLID              := v_LID_2;
            v_SecondaryCID              := v_CID_2;
            v_Secondaryextendedplaycode := v_extendedplaycode_2;
            v_Found := True;

            --If the Primary and Sencondary account were found
            IF v_Found = True THEN


              --Clean the detail work table
              SELECT COUNT(dw.dtl_rowkey)
              INTO v_detailitemwkcount -- making sure, if there is a record, to delete it.
              FROM bp_ae.LW_txndetailitem_Wrk dw;

              If v_detailitemwkcount > 0 then
                Delete from bp_ae.LW_txndetailitem_Wrk; -- not using truncate because this causes ORA-08103 in the header rules processing
                commit;
              End if;

			  --Clean the header work table
              SELECT COUNT(hw.header_rowkey)
              INTO v_headeritemwkcount -- making sure, if there is a record, to delete it.
              FROM bp_ae.lw_txnheader_wrk hw;

              If v_headeritemwkcount > 0 then
                Delete from bp_ae.lw_txnheader_wrk; -- not using truncate because this causes ORA-08103 in the header rules processing
                commit;
              End if;

              --Update A_PROGRAMCHANGEDATE from the secondary account with the current date in order to convert the point
              update bp_ae.ats_memberdetails md
              set md.a_programchangedate = trunc(sysdate)
              where md.a_ipcode = v_IPC_2;
              commit;

              --Primary = Legacy and Secondary = Pilot
              IF v_Primaryextendedplaycode NOT IN (1,3) AND v_Secondaryextendedplaycode IN (1,3) THEN
                -- The secondary account (pilot) should be converted to legacy points.
                CONVERTONEPILOTTOLEGACY(to_char(p_processDate), v_SecondaryLID);

                --Primary = Pilot and Secondary = Legacy
              ELSIF v_Primaryextendedplaycode IN (1,3) AND v_Secondaryextendedplaycode NOT IN (1,3) THEN
                --The secondary account (legacy) should be converted to pilot points.
                CONVERTONELEGACYTOPILOT(p_processDate, v_SecondaryLID);
              END IF;

              --Call the rules package
              BP_AE.RULESPROCESSING.Headerrulesprocess(V_DUMMY,to_char(p_processDate,'YYYYMMddHH24miss'),crsr1);
              BP_AE.RULESPROCESSING.Detailtxnrulesprocess(V_DUMMY,to_char(p_processDate,'YYYYMMddHH24miss'),crsr2);
              BP_AE.RULESPROCESSING.Bonuseventreturn(V_DUMMY,to_char( p_processDate,'YYYYMMddHH24miss'), crsr3);
              -- BP_AE.STAGE_TXNCOPY.TlogInterceptorProcessing( V_DUMMY,crsr4);
              BP_AE.Ae_Pointsconversion.CLEARWORKTABLES(V_DUMMY);



              --Clean the detail work table
              SELECT COUNT(dw.dtl_rowkey)
              INTO v_detailitemwkcount -- making sure, if there is a record, to delete it.
              FROM bp_ae.LW_txndetailitem_Wrk dw;

              If v_detailitemwkcount > 0 then
                Delete from bp_ae.LW_txndetailitem_Wrk; -- not using truncate because this causes ORA-08103 in the header rules processing
                commit;
              End if;

			   --Clean the header work table
              SELECT COUNT(hw.header_rowkey)
              INTO v_headeritemwkcount -- making sure, if there is a record, to delete it.
              FROM bp_ae.lw_txnheader_wrk hw;

              If v_headeritemwkcount > 0 then
                Delete from bp_ae.lw_txnheader_wrk; -- not using truncate because this causes ORA-08103 in the header rules processing
                commit;
              End if;

              --Insert the members for merge
              INSERT INTO ATS_PROGRAMECONVERSIONMERGE
              ( A_PRIMARYLID,
              A_PRIMARYCID,
              A_PRIMARYEXTENDEDPLAYCODE,
              A_SECONDARYLID,
              A_SECONDARYCID,
              A_SECONDARYEXTENDEDPLAYCODE,
              A_REQUESTEDMERGEDATE
              )
              VALUES
              ( v_primaryLID,
              v_PrimaryCID,
              v_Primaryextendedplaycode,
              v_SecondaryLID,
              v_SecondaryCID,
              v_Secondaryextendedplaycode,
              TRUNC(p_processDate)
              );
            END IF;
            COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        ELSE
          raise_application_error(-20001, 'no virtual card found for lid=' + V_TBL(I).loyaltyid_1 + ' or lid=' + V_TBL(I).loyaltyid_2);
        END IF;

      END LOOP;
      EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
    --<--- dont forget to close cursor since we manually opened it.
    CLOSE GET_DATA;
    END IF;
  END;
END PROCESS_PROGCONVMERGE;

END PROGRAMCONVERSIONMERGE;
/

