create or replace package PILOT_ADHOC_METRICS is

    /*********** Author  : EHERRERA ************/
    PROCEDURE RETRIEVE_DATA;

end PILOT_ADHOC_METRICS;
/

create or replace package body PILOT_ADHOC_METRICS is

    PROCEDURE RETRIEVE_DATA IS

    c_start_date CONSTANT DATE := TO_DATE('01/OCT/2015','dd/mon/yyyy');


/*
    --log job attributes
    v_jobdirection     NUMBER := 0;
    v_filename         VARCHAR2(512) := p_filename;
    v_starttime        DATE := SYSDATE;
    v_endtime          DATE;
    v_messagesreceived NUMBER := 0;
    v_messagesfailed   NUMBER := 0;
    v_jobstatus        NUMBER := 0;
    v_jobname          VARCHAR2(256);

    --log msg attributes
    v_messageid VARCHAR2(256);
    v_envkey    VARCHAR2(256) := 'BP_AE@' || UPPER(sys_context('userenv', 'instance_name'));
    v_logsource VARCHAR2(256);
    v_filename  VARCHAR2(256) := p_filename;
    v_batchid   VARCHAR2(256) := 0;
    v_message   VARCHAR2(256);
    v_reason    VARCHAR2(256);
    v_error     VARCHAR2(256);
    v_trycount  NUMBER := 0;
    */

    BEGIN


  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /*  Fill out Transactions Aux Table */

    EXECUTE IMMEDIATE 'TRUNCATE TABLE STG_ADHOC_METRICS_TXN_AUX';

    INSERT INTO STG_ADHOC_METRICS_TXN_AUX
        SELECT txn.a_txnheaderid
             , txn.a_vckey
             , txn.a_ordernumber
             , txn.a_storenumber
             , CASE WHEN EXISTS (SELECT 1
                                 FROM ATS_TXNHEADER txn2
                                 WHERE txn2.a_vckey = txn2.a_vckey
                                 AND txn2.a_txndate < c_start_date
                                )
                    THEN 'OLD'
                    ELSE 'NEW'
                END shopperType
              , d.a_homestoreid
        FROM ATS_TXNHEADER txn
           , LW_VIRTUALCARD vc
           , ATS_MEMBERDETAILS d
        WHERE vc.vckey = txn.a_vckey
          AND d.a_ipcode = vc.ipcode
          AND txn.a_txndate >= c_start_date
          AND txn.a_storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  );

    COMMIT;


    /* LOG COMMITED SUCCESFULLY */

  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /*  Fill out Enrollments Table */

    EXECUTE IMMEDIATE 'TRUNCATE TABLE STG_ADHOC_METRICS_ENROLLMENTS';

    INSERT INTO STG_ADHOC_METRICS_ENROLLMENTS (
           SUMMARY_DATE ,
           TOTAL ,
           AVERAGE ,
           ONLINE_SUMMARY ,
           STORE_107,
           STORE_140 ,
           STORE_180 ,
           STORE_209 ,
           STORE_225 ,
           STORE_228 ,
           STORE_260 ,
           STORE_269 ,
           STORE_282 ,
           STORE_291 ,
           STORE_300 ,
           STORE_312 ,
           STORE_366 ,
           STORE_418 ,
           STORE_482 ,
           STORE_728 ,
           STORE_825 ,
           STORE_880 ,
           STORE_882 ,
           STORE_2082 ,
           BALANCE
            )
      SELECT *
      FROM (
          SELECT ENROLLMENTDATE
               , STORE
               ,COUNT (1) OVER (PARTITION BY ENROLLMENTDATE ) total
               , ROUND ( (COUNT (1) OVER (PARTITION BY ENROLLMENTDATE ) / 22 ) , 3 ) AVERAGE
          FROM (
                SELECT TRUNC(lm.membercreatedate) AS ENROLLMENTDATE
                    , 'Online' Store
                FROM ATS_MEMBERDETAILS md
                JOIN LW_STOREDEF st
                     ON md.a_homestoreid = st.storenumber
                JOIN LW_LOYALTYMEMBER lm
                     ON md.a_ipcode = lm.ipcode
                WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
                AND md.a_membersource = 20
                AND md.a_homestoreid = st.storenumber
                AND lm.membercreatedate >= c_start_date
            UNION ALL
                SELECT TRUNC(lm.membercreatedate) AS ENROLLMENTDATE
                , TO_CHAR(st.storenumber) || ' ' || TO_CHAR(st.storename) AS Store
                FROM ATS_MEMBERDETAILS md
                JOIN LW_STOREDEF st
                     ON md.a_homestoreid = st.storenumber
                JOIN LW_LOYALTYMEMBER lm
                     ON md.a_ipcode = lm.ipcode
                WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
                AND md.a_membersource <> 20
                AND md.a_homestoreid = st.storenumber
                AND lm.membercreatedate >= c_start_date
             UNION ALL
                SELECT TRUNC(lm.membercreatedate) AS ENROLLMENTDATE
                    , 'BALANCE' AS Store
                FROM ATS_MEMBERDETAILS md
               JOIN LW_STOREDEF st
                     ON md.a_homestoreid = st.storenumber
                JOIN LW_LOYALTYMEMBER lm
                     ON md.a_ipcode = lm.ipcode
                WHERE st.storenumber NOT IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
                AND md.a_membersource <> 20
                AND lm.membercreatedate >= c_start_date
          )
   )
PIVOT ( COUNT(1)  FOR (store) IN ('Online'
                                         ,'107 GREENWOOD MALL'
                                         ,'140 CHERRYVALE MALL'
                                         ,'180 MILLCREEK MALL'
                                         ,'209 RIVER HILLS MALL'
                                         ,'225 GATEWAY CENTER'
                                         ,'228 VALLEY VIEW MALL'
                                         ,'260 RIVER VALLEY MALL'
                                         ,'269 GOVERNORS SQUARE'
                                         ,'282 VALDOSTA COLONIAL MALL'
                                         ,'291 OAKWOOD MALL'
                                         ,'300 FINDLAY VILLAGE MALL'
                                         ,'312 WEST PARK MALL'
                                         ,'366 BOISE TOWNE SQUARE'
                                         ,'418 THE GALLERIA'
                                         ,'482 HAYWOOD MALL'
                                         ,'728 SOUTHLAKE MALL- WESTFIELD'
                                         ,'825 SOUTHPARK MALL'
                                         ,'880 THE CROSSROADS'
                                         ,'882 HONEY CREEK MALL'
                                         ,'2082 THE SHOPS AT CENTERRA'
                                         ,'BALANCE'
                                          )
      )
ORDER BY 1 ;


    COMMIT;

    /* todo insert log that ENROLLMENTS commit was succesfull /*




  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /*  Fill out Sales Table */


    EXECUTE IMMEDIATE 'TRUNCATE TABLE STG_ADHOC_METRICS_SALES';

    INSERT INTO STG_ADHOC_METRICS_SALES (
           SUMMARY_DATE ,
           TOTAL ,
           AVERAGE,
           ONLINE_SUMMARY ,
           STORE_107,
           STORE_140 ,
           STORE_180 ,
           STORE_209 ,
           STORE_225 ,
           STORE_228 ,
           STORE_260 ,
           STORE_269 ,
           STORE_282 ,
           STORE_291 ,
           STORE_300 ,
           STORE_312 ,
           STORE_366 ,
           STORE_418 ,
           STORE_482 ,
           STORE_728 ,
           STORE_825 ,
           STORE_880 ,
           STORE_882 ,
           STORE_2082 ,
           BALANCE
            )
       SELECT *
       FROM (
      SELECT ENROLLMENTDATE
           , STORE
           , a_txnamount
           ,SUM (a_txnamount) OVER (PARTITION BY ENROLLMENTDATE ) total
           , ROUND ( (SUM (a_txnamount) OVER (PARTITION BY ENROLLMENTDATE ) / 22 ) , 3 ) AVERAGE
      FROM (
            SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                 , 'Online' Store
                 , txn.a_txnamount
            FROM ATS_TXNHEADER txn
            JOIN LW_STOREDEF st
                 ON txn.a_storenumber = st.storenumber
            JOIN LW_VIRTUALCARD vc
                 ON txn.a_vckey = vc.vckey
            JOIN ATS_MEMBERDETAILS md
                 ON vc.ipcode = md.a_ipcode
            WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
            AND txn.a_ordernumber IS NOT NULL
            AND md.a_homestoreid = st.storenumber
            AND txn.a_txndate >= c_start_date
        UNION ALL
            SELECT TRUNC(txn.a_txndate) AS ENROLLMENTDATE
                 , TO_CHAR(st.storenumber) || ' ' || TO_CHAR(st.storename) AS Store
                 , txn.a_txnamount
            FROM ATS_TXNHEADER txn
            JOIN LW_STOREDEF st
                 ON txn.a_storenumber = st.storenumber
            JOIN LW_VIRTUALCARD vc
                 ON txn.a_vckey = vc.vckey
            JOIN ATS_MEMBERDETAILS md
                 ON vc.ipcode = md.a_ipcode
            WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
            AND txn.a_ordernumber IS NULL
            AND md.a_homestoreid = st.storenumber
            AND md.a_homestoreid = txn.a_storenumber
            AND txn.a_txndate >= c_start_date
        UNION ALL
              SELECT TRUNC(txn.a_txndate) AS ENROLLMENTDATE
                   , 'BALANCE' AS Store
                   , txn.a_txnamount
              FROM ATS_TXNHEADER txn
              JOIN LW_STOREDEF st
                   ON txn.a_storenumber = st.storenumber
              JOIN LW_VIRTUALCARD vc
                   ON txn.a_vckey = vc.vckey
              JOIN ATS_MEMBERDETAILS md
                   ON vc.ipcode = md.a_ipcode
              WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
              AND txn.a_ordernumber IS NULL
              AND md.a_homestoreid = st.storenumber
              AND md.a_homestoreid != txn.a_storenumber
              AND txn.a_txndate >= c_start_date
      )
   )
PIVOT ( SUM(a_txnamount)  FOR (store) IN ('Online'
                                         ,'107 GREENWOOD MALL'
                                         ,'140 CHERRYVALE MALL'
                                         ,'180 MILLCREEK MALL'
                                         ,'209 RIVER HILLS MALL'
                                         ,'225 GATEWAY CENTER'
                                         ,'228 VALLEY VIEW MALL'
                                         ,'260 RIVER VALLEY MALL'
                                         ,'269 GOVERNORS SQUARE'
                                         ,'282 VALDOSTA COLONIAL MALL'
                                         ,'291 OAKWOOD MALL'
                                         ,'300 FINDLAY VILLAGE MALL'
                                         ,'312 WEST PARK MALL'
                                         ,'366 BOISE TOWNE SQUARE'
                                         ,'418 THE GALLERIA'
                                         ,'482 HAYWOOD MALL'
                                         ,'728 SOUTHLAKE MALL- WESTFIELD'
                                         ,'825 SOUTHPARK MALL'
                                         ,'880 THE CROSSROADS'
                                         ,'882 HONEY CREEK MALL'
                                         ,'2082 THE SHOPS AT CENTERRA'
                                         ,'BALANCE'
                                          )
      )
ORDER BY 1;

    COMMIT;

    /* todo insert log that SALES commit was succesfull /*



  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /*  Fill out TRANSACTIONS Table */


  EXECUTE IMMEDIATE 'TRUNCATE TABLE STG_ADHOC_METRICS_TXN';


  INSERT INTO STG_ADHOC_METRICS_TXN (
           SUMMARY_DATE ,
           TOTAL ,
           AVERAGE ,
           ONLINE_SUMMARY ,
           STORE_107,
           STORE_140 ,
           STORE_180 ,
           STORE_209 ,
           STORE_225 ,
           STORE_228 ,
           STORE_260 ,
           STORE_269 ,
           STORE_282 ,
           STORE_291 ,
           STORE_300 ,
           STORE_312 ,
           STORE_366 ,
           STORE_418 ,
           STORE_482 ,
           STORE_728 ,
           STORE_825 ,
           STORE_880 ,
           STORE_882 ,
           STORE_2082 ,
           BALANCE
            )
SELECT *
FROM (
      SELECT ENROLLMENTDATE
           , STORE
           ,COUNT (DISTINCT(loyaltyidnumber)) OVER (PARTITION BY ENROLLMENTDATE ) total
           , ROUND ( (COUNT (loyaltyidnumber) OVER (PARTITION BY ENROLLMENTDATE) / 22 ) , 3 )avg2
           , loyaltyidnumber
      FROM (
            SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                , 'Online' Store
                , vc.loyaltyidnumber
            FROM ATS_TXNHEADER txn
            JOIN LW_STOREDEF st
                 ON txn.a_storenumber = st.storenumber
            JOIN LW_VIRTUALCARD vc
                 ON txn.a_vckey = vc.vckey
            JOIN ATS_MEMBERDETAILS md
                 ON vc.ipcode = md.a_ipcode
            WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
            AND txn.a_ordernumber IS NOT NULL
            AND md.a_homestoreid = st.storenumber
            AND txn.a_txndate >= c_start_date
        UNION ALL
            SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                 , TO_CHAR(st.storenumber) || ' ' || TO_CHAR(st.storename) AS Store
                 , vc.loyaltyidnumber
            FROM ATS_TXNHEADER txn
            JOIN LW_STOREDEF st
                 ON txn.a_storenumber = st.storenumber
            JOIN LW_VIRTUALCARD vc
                 ON txn.a_vckey = vc.vckey
            JOIN ATS_MEMBERDETAILS md
                 ON vc.ipcode = md.a_ipcode
            WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
            AND txn.a_ordernumber IS NULL
            AND md.a_homestoreid = st.storenumber
            AND md.a_homestoreid = txn.a_storenumber
            AND txn.a_txndate >= c_start_date
        UNION ALL
            SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                 , 'BALANCE' AS Store
                 , vc.loyaltyidnumber
            FROM ATS_TXNHEADER txn
            JOIN LW_STOREDEF st
                 ON txn.a_storenumber = st.storenumber
            JOIN LW_VIRTUALCARD vc
                 ON txn.a_vckey = vc.vckey
            JOIN ATS_MEMBERDETAILS md
                 ON vc.ipcode = md.a_ipcode
            WHERE st.storenumber IN ( 107, 140, 180, 2082, 209, 225, 228, 260, 269, 282, 291, 300, 312, 366, 418, 482, 728 ,825, 880, 882  )
            AND txn.a_ordernumber IS NULL
            AND md.a_homestoreid = st.storenumber
            AND md.a_homestoreid != txn.a_storenumber
            AND txn.a_txndate >= c_start_date
      )
   )
PIVOT ( COUNT( DISTINCT (loyaltyidnumber) ) FOR (store) IN ('Online'
                                         ,'107 GREENWOOD MALL'
                                         ,'140 CHERRYVALE MALL'
                                         ,'180 MILLCREEK MALL'
                                         ,'209 RIVER HILLS MALL'
                                         ,'225 GATEWAY CENTER'
                                         ,'228 VALLEY VIEW MALL'
                                         ,'260 RIVER VALLEY MALL'
                                         ,'269 GOVERNORS SQUARE'
                                         ,'282 VALDOSTA COLONIAL MALL'
                                         ,'291 OAKWOOD MALL'
                                         ,'300 FINDLAY VILLAGE MALL'
                                         ,'312 WEST PARK MALL'
                                         ,'366 BOISE TOWNE SQUARE'
                                         ,'418 THE GALLERIA'
                                         ,'482 HAYWOOD MALL'
                                         ,'728 SOUTHLAKE MALL- WESTFIELD'
                                         ,'825 SOUTHPARK MALL'
                                         ,'880 THE CROSSROADS'
                                         ,'882 HONEY CREEK MALL'
                                         ,'2082 THE SHOPS AT CENTERRA'
                                         ,'BALANCE'
                                          )
      )
ORDER BY 1;

  COMMIT;

  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /*  Fill out NEW SHOPERS Table */


  EXECUTE IMMEDIATE 'TRUNCATE TABLE STG_ADHOC_METRICS_NEW';

  INSERT INTO STG_ADHOC_METRICS_NEW (
           SUMMARY_DATE ,
           TOTAL ,
           AVERAGE ,
           ONLINE_SUMMARY ,
           STORE_107,
           STORE_140 ,
           STORE_180 ,
           STORE_209 ,
           STORE_225 ,
           STORE_228 ,
           STORE_260 ,
           STORE_269 ,
           STORE_282 ,
           STORE_291 ,
           STORE_300 ,
           STORE_312 ,
           STORE_366 ,
           STORE_418 ,
           STORE_482 ,
           STORE_728 ,
           STORE_825 ,
           STORE_880 ,
           STORE_882 ,
           STORE_2082 ,
           BALANCE
            )
        SELECT *
        FROM (
              SELECT ENROLLMENTDATE
                   , STORE
                   ,COUNT (1) OVER (PARTITION BY ENROLLMENTDATE ) total
                  , ROUND ( (COUNT (1) OVER (PARTITION BY ENROLLMENTDATE ) / 22 ) , 3 )avg2
              FROM (
                    SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                        , 'Online' Store
                    FROM ATS_TXNHEADER txn
                    JOIN STG_ADHOC_METRICS_TXN_AUX aux
                         ON txn.a_txnheaderid = aux.txn_headerid
                    WHERE aux.homestoreid = aux.storenumber
                    AND aux.Ordernumber IS NOT NULL
                    AND aux.shopertype = 'NEW'
                UNION ALL
                    SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                         , TO_CHAR(st.storenumber) || ' ' || TO_CHAR(st.storename) AS Store
                    FROM ATS_TXNHEADER txn
                    JOIN LW_STOREDEF st
                         ON txn.a_storenumber = st.storenumber
                    JOIN STG_ADHOC_METRICS_TXN_AUX aux
                         ON txn.a_txnheaderid = aux.txn_headerid
                    WHERE aux.homestoreid = aux.storenumber
                    AND aux.homestoreid = txn.a_storenumber
                    AND aux.Ordernumber IS NULL
                    AND aux.shopertype = 'NEW'
                UNION ALL
                    SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                         , 'BALANCE' AS Store
                    FROM ATS_TXNHEADER txn
                    JOIN LW_STOREDEF st
                         ON txn.a_storenumber = st.storenumber
                    JOIN STG_ADHOC_METRICS_TXN_AUX aux
                         ON txn.a_txnheaderid = aux.txn_headerid
                    WHERE aux.homestoreid = aux.storenumber
                    AND aux.homestoreid != txn.a_storenumber
                    AND aux.Ordernumber IS NULL
                    AND aux.shopertype = 'NEW'
              )
           )
        PIVOT ( COUNT(1)  FOR (store) IN ('Online'
                                         ,'107 GREENWOOD MALL'
                                         ,'140 CHERRYVALE MALL'
                                         ,'180 MILLCREEK MALL'
                                         ,'209 RIVER HILLS MALL'
                                         ,'225 GATEWAY CENTER'
                                         ,'228 VALLEY VIEW MALL'
                                         ,'260 RIVER VALLEY MALL'
                                         ,'269 GOVERNORS SQUARE'
                                         ,'282 VALDOSTA COLONIAL MALL'
                                         ,'291 OAKWOOD MALL'
                                         ,'300 FINDLAY VILLAGE MALL'
                                         ,'312 WEST PARK MALL'
                                         ,'366 BOISE TOWNE SQUARE'
                                         ,'418 THE GALLERIA'
                                         ,'482 HAYWOOD MALL'
                                         ,'728 SOUTHLAKE MALL- WESTFIELD'
                                         ,'825 SOUTHPARK MALL'
                                         ,'880 THE CROSSROADS'
                                         ,'882 HONEY CREEK MALL'
                                         ,'2082 THE SHOPS AT CENTERRA'
                                         ,'BALANCE'
                                                  )
              )
        ORDER BY 1;


        COMMIT;


  /*************************************************************************************/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  /*  Fill out EXISTING SHOPERS Table */


  EXECUTE IMMEDIATE 'TRUNCATE TABLE STG_ADHOC_METRICS_EXISTING';

  INSERT INTO STG_ADHOC_METRICS_EXISTING (
           SUMMARY_DATE ,
           TOTAL ,
           AVERAGE ,
           ONLINE_SUMMARY ,
           STORE_107,
           STORE_140 ,
           STORE_180 ,
           STORE_209 ,
           STORE_225 ,
           STORE_228 ,
           STORE_260 ,
           STORE_269 ,
           STORE_282 ,
           STORE_291 ,
           STORE_300 ,
           STORE_312 ,
           STORE_366 ,
           STORE_418 ,
           STORE_482 ,
           STORE_728 ,
           STORE_825 ,
           STORE_880 ,
           STORE_882 ,
           STORE_2082 ,
           BALANCE
            )
        SELECT *
        FROM (
              SELECT ENROLLMENTDATE
                   , STORE
                   ,COUNT (1) OVER (PARTITION BY ENROLLMENTDATE ) total
                   , ROUND ( (COUNT (1) OVER (PARTITION BY ENROLLMENTDATE ) / 22 ) , 3 )avg2
              FROM (
                    SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                        , 'Online' Store
                    FROM ATS_TXNHEADER txn
                    JOIN STG_ADHOC_METRICS_TXN_AUX aux
                         ON txn.a_txnheaderid = aux.txn_headerid
                    WHERE aux.homestoreid = aux.storenumber
                    AND aux.Ordernumber IS NOT NULL
                    AND aux.shopertype = 'OLD'
                UNION ALL
                    SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                         , TO_CHAR(st.storenumber) || ' ' || TO_CHAR(st.storename) AS Store
                    FROM ATS_TXNHEADER txn
                    JOIN LW_STOREDEF st
                         ON txn.a_storenumber = st.storenumber
                    JOIN STG_ADHOC_METRICS_TXN_AUX aux
                         ON txn.a_txnheaderid = aux.txn_headerid
                    WHERE aux.homestoreid = aux.storenumber
                    AND aux.homestoreid = txn.a_storenumber
                    AND aux.Ordernumber IS NULL
                    AND aux.shopertype = 'OLD'
                UNION ALL
                    SELECT TRUNC(txn.a_txndate) ENROLLMENTDATE
                         , 'BALANCE' AS Store
                    FROM ATS_TXNHEADER txn
                    JOIN LW_STOREDEF st
                         ON txn.a_storenumber = st.storenumber
                    JOIN STG_ADHOC_METRICS_TXN_AUX aux
                         ON txn.a_txnheaderid = aux.txn_headerid
                    WHERE aux.homestoreid = aux.storenumber
                    AND aux.homestoreid != txn.a_storenumber
                    AND aux.Ordernumber IS NULL
                    AND aux.shopertype = 'OLD'
              )
           )
        PIVOT ( COUNT(1)  FOR (store) IN ('Online'
                                         ,'107 GREENWOOD MALL'
                                         ,'140 CHERRYVALE MALL'
                                         ,'180 MILLCREEK MALL'
                                         ,'209 RIVER HILLS MALL'
                                         ,'225 GATEWAY CENTER'
                                         ,'228 VALLEY VIEW MALL'
                                         ,'260 RIVER VALLEY MALL'
                                         ,'269 GOVERNORS SQUARE'
                                         ,'282 VALDOSTA COLONIAL MALL'
                                         ,'291 OAKWOOD MALL'
                                         ,'300 FINDLAY VILLAGE MALL'
                                         ,'312 WEST PARK MALL'
                                         ,'366 BOISE TOWNE SQUARE'
                                         ,'418 THE GALLERIA'
                                         ,'482 HAYWOOD MALL'
                                         ,'728 SOUTHLAKE MALL- WESTFIELD'
                                         ,'825 SOUTHPARK MALL'
                                         ,'880 THE CROSSROADS'
                                         ,'882 HONEY CREEK MALL'
                                         ,'2082 THE SHOPS AT CENTERRA'
                                         ,'BALANCE'
                                         )
              )
        ORDER BY 1;


        COMMIT;




 /*
    EXCEPTION
          WHEN OTHERS THEN
            v_messagesfailed := v_messagesfailed + 1;
            v_error          := SQLERRM;
            v_reason         := 'Failed insert to LW_MEMBERDETAIL_STAGE';
            v_message        := '<Members>' || CHR(10) || '  <Member>' ||
                                CHR(10) || '    <FirstName>' ||
                                rec_CardHolder01.FIRSTNAME ||
                                '</FirstName>' || CHR(10) ||
                                '    <LastName>' ||
                                rec_CardHolder01.LASTNAME || '</LastName>' ||
                                CHR(10) || '    <PrimaryEmail>' ||
                                rec_CardHolder01.EMAILADDRESS ||
                                '</PrimaryEmail>' || CHR(10) ||
                                '    <AlternateId>' ||
                                rec_CardHolder01.ALTERNATEID ||
                                '</AlternateId>' || CHR(10) ||
                                '  </Member>' || CHR(10) || '</Members>';



            /* log error */
  /*               utility_pkg.Log_msg(p_messageid => v_messageid,
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

     */


    END RETRIEVE_DATA;


end PILOT_ADHOC_METRICS;
/

