CREATE OR REPLACE PACKAGE POINTADJUSTMENT_PKG AS

  TYPE Rcursor IS REF CURSOR;
  PROCEDURE Proc_adjust_negative_balance(p_qtr  IN DATE,
                                         Retval IN OUT Rcursor);
  PROCEDURE Proc_adjust_expdateoptout(p_date IN DATE,
                                      Retval IN OUT Rcursor);
  PROCEDURE Proc_adjust_expdatemerge(p_date IN DATE, Retval IN OUT Rcursor);
  --Changes for PI 30364 - Dollar reward program start here -- Akbar
  PROCEDURE Proc_adjust_neg_balance_new(p_ProcessDate IN DATE,
                                        Retval        IN OUT Rcursor);
  --Changes for PI 30364 - Dollar reward program end here -- Akbar
  PROCEDURE Proc_adjust_neg_brapoints(p_ProcessDate IN DATE,
                                      Retval        IN OUT Rcursor); -- Changes for PI 31769 bra points adjustments
END;
/

CREATE OR REPLACE PACKAGE BODY POINTADJUSTMENT_PKG AS

  PROCEDURE Proc_adjust_negative_balance(p_qtr  IN DATE,
                                         Retval IN OUT Rcursor) IS
    v_adj_pointtypeid      NUMBER;
    v_adj_bonuspointtypeid NUMBER;
    v_adj_pointeventid     NUMBER;

    CURSOR get_data IS
      WITH PTS_VALS AS
       (SELECT VCKEY, NAME, NVL(SUM(POINTS), 0) PNTS
          FROM LW_POINTTRANSACTION PTS, LW_POINTTYPE PT
         WHERE TRANSACTIONDATE >= TRUNC(sysdate, 'Q')
           AND TRANSACTIONDATE < ADD_MONTHS(TRUNC(sysdate, 'Q'), 3)
           AND PTS.POINTTYPEID = PT.POINTTYPEID
           AND NAME IN ('Basic Points',
                        'Adjustment Points',
                        'StartingPoints',
                        'CS Points',
                        'Bonus Points',
                        'Adjustment Bonus Points')
         GROUP BY VCKEY, NAME)
      SELECT VC.IPCODE,
             VC.VCKEY,
             sum(CASE
                   WHEN NAME = 'Basic Points' then
                    PTS.PNTS
                   WHEN NAME = 'Adjustment Points' then
                    PTS.PNTS
                   ELSE
                    0
                 END) AS TOTALBASICPOINTS,
             sum(CASE
                   WHEN NAME = 'StartingPoints' then
                    PTS.PNTS
                   WHEN NAME = 'CS Points' then
                    PTS.PNTS
                   WHEN NAME = 'Bonus Points' then
                    PTS.PNTS
                   WHEN NAME = 'Adjustment Bonus Points' then
                    PTS.PNTS
                   ELSE
                    0
                 END) AS TOTALBONUSPOINTS
        FROM LW_VIRTUALCARD VC, PTS_VALS PTS
       WHERE VC.VCKEY = PTS.VCKEY(+)
       GROUP BY (VC.IPCODE, VC.VCKEY);

    v_balance    NUMBER := 0;
    v_adjustment NUMBER := 0;
    v_vckey      NUMBER := 0;
    v_qtr        DATE := to_date('1900', 'yyyy');
    v_cnt        NUMBER := 0;

   --log job attributes
  v_my_log_id             NUMBER;
  v_dap_log_id            NUMBER;
  v_jobdirection          NUMBER:=0;
  v_starttime             DATE:=SYSDATE;
  v_endtime               DATE;
  v_messagesreceived      NUMBER:=0;
  v_messagesfailed        NUMBER:=0;
  v_jobstatus             NUMBER:=0;
  v_jobname               VARCHAR2(256);
  v_processId             NUMBER:=0;
  v_Errormessage          VARCHAR2(256);

  BEGIN
      /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();

  v_jobname := 'PointsAdjustment';
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

    Proc_adjust_neg_brapoints(p_ProcessDate => sysdate, Retval => Retval); -- Changes for PI 31769 bra points adjustments

    /* aquire adjustment point type id */
    SELECT pointtypeid
      INTO v_adj_pointtypeid
      FROM lw_pointtype
     WHERE NAME = 'Adjustment Points';

    /* aquire adjustment bonus point type id */
    SELECT pointtypeid
      INTO v_adj_bonuspointtypeid
      FROM lw_pointtype
     WHERE NAME = 'Adjustment Bonus Points';

    /* point event is AE Return for all adjustments */
    SELECT pe.pointeventid
      INTO v_adj_pointeventid
      FROM lw_pointevent pe
     WHERE pe.name = 'AE Return';

    FOR y IN get_data LOOP
      /* if virtual card changed then reset variables, also initial set on first record */
      IF y.vckey != v_vckey THEN
        v_balance    := 0;
        v_adjustment := 0;
        v_vckey      := y.vckey;
        v_qtr        := trunc(SYSDATE, 'Q');
      END IF;
      /* checking balance */
      v_balance := y.totalbasicpoints + y.totalbonuspoints;
      /* if balance dips below zero then we need adjusting */
      IF v_balance < 0 THEN
        /* if Bonus Points dips below zero then use Adjustment Bonus Points */
        IF y.totalbasicpoints >= 0 AND y.totalbonuspoints < 0 THEN
          v_adjustment := ABS(v_balance); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
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
            --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_bonuspointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             SYSDATE,
             SYSDATE,
             v_adjustment /*Points*/,
             add_months(trunc(sysdate, 'Q'), 3),
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);
        END IF;
        /* if Basic Points dips below zero then use Adjustment Bonus Points */
        IF y.totalbasicpoints < 0 AND y.totalbonuspoints >= 0 THEN
          v_adjustment := ABS(v_balance); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
             VcKey,
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
           --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_pointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             SYSDATE,
             SYSDATE,
             v_adjustment /*Points*/,
             add_months(trunc(sysdate, 'Q'), 3),
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);
        END IF;
        /* if both Basic Points and Bonus Points dip below zero then make seperate adjustments using Adjustment Points and Adjustment Bonus Points */
        IF y.totalbasicpoints < 0 AND y.totalbonuspoints < 0 THEN
          v_adjustment := ABS(y.totalbonuspoints); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
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
            --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_bonuspointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             SYSDATE,
             SYSDATE,
             v_adjustment /*Points*/,
             add_months(trunc(sysdate, 'Q'), 3),
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);

          v_adjustment := ABS(y.totalbasicpoints); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
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
                  --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_pointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             SYSDATE,
             SYSDATE,
             v_adjustment /*Points*/,
             add_months(trunc(sysdate, 'Q'), 3),
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);
        END IF;
        v_balance := 0; /* zero running balance */
        v_cnt     := v_cnt + 1;
        IF MOD(v_cnt, 100) = 0 THEN
          COMMIT;
        END IF;
      END IF;
    END LOOP;

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
  END Proc_adjust_negative_balance;

  PROCEDURE Proc_adjust_expdateoptout(p_date IN DATE,
                                      Retval IN OUT Rcursor) IS
    CURSOR get_data IS
      Select pt.pointtransactionid, pt.transactiondate
        from lw_pointtransaction pt
       INNER JOIN LW_POINTTYPE TY
          ON TY.POINTTYPEID = PT.POINTTYPEID
       inner join lw_virtualcard vc
          on vc.vckey = pt.vckey
       inner join ats_memberdetails md
          on md.a_ipcode = vc.ipcode
       inner join ATS_MEMBERDOLLARREWARDOPTOUT DR
          on dr.a_ipcode = vc.ipcode
       where nvl(md.a_extendedplaycode, '0') = 0
         and DR.a_Action = 'OptOut'
         and dr.createdate > (to_date(p_date) - interval '1' day)
         and trunc(pt.transactiondate) >=
             (Select to_date(to_char(value), 'mm/dd/yyyy')
                from lw_clientconfiguration cc
               where cc.key = 'CalculateTransactionStartDate')
         and Ty.name not like 'Jean%'
         and Ty.name not like 'Bra%'
         and trunc(pt.expirationdate) =
             add_months(TRUNC(p_date, 'YEAR'), 12) -- to_date('1/1/2015','mm/dd/yyyy');
         and to_number(to_char(p_date, 'Q')) <> 4; -- exclude last qtr,since expiration dates are going to begining of next year  01/01

    v_cnt NUMBER := 0;
  BEGIN
    FOR y IN get_data LOOP

      Update lw_pointtransaction pt
         set pt.expirationdate = add_months(trunc(y.transactiondate, 'Q'), 3),
             pt.notes          = 'Expirationdate changed on ' ||
                                 to_char(trunc(p_date)) ||
                                 ' due to Dollar Reward Tier Optout',
             pt.ptchangedby    = 'DollarRwdOptout'
       where pt.pointtransactionid = y.pointtransactionid;
      v_cnt := v_cnt + 1;
      IF MOD(v_cnt, 1000) = 0 THEN
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
      END IF;

    END LOOP;
    COMMIT;
    IF get_data%ISOPEN THEN
      --<--- dont forget to close cursor since we manually opened it.
      CLOSE get_data;
    END IF;
    COMMIT;
  END Proc_adjust_expdateoptout;

  PROCEDURE Proc_adjust_expdatemerge(p_date IN DATE, Retval IN OUT Rcursor) IS
    CURSOR get_data IS
      Select pt.pointtransactionid, pt.transactiondate
        from lw_pointtransaction pt
       INNER JOIN LW_POINTTYPE TY
          ON TY.POINTTYPEID = PT.POINTTYPEID
       inner join lw_virtualcard vc
          on vc.vckey = pt.vckey
       inner join ats_memberdetails md
          on md.a_ipcode = vc.ipcode
       inner join ATS_MEMBERDOLLARREWARDOPTOUT DR
          on dr.a_ipcode = vc.ipcode
       where nvl(md.a_extendedplaycode, '0') = 1
         and DR.a_Action = 'Merge'
         and dr.createdate > (to_date(p_date) - interval '1' day)
         and trunc(pt.transactiondate) >=
             (Select to_date(to_char(value), 'mm/dd/yyyy')
                from lw_clientconfiguration cc
               where cc.key = 'CalculateTransactionStartDate')
         and Ty.name not like 'Jean%'
         and Ty.name not like 'Bra%'
         and trunc(pt.expirationdate) <>
             add_months(TRUNC(p_date, 'YEAR'), 12) -- to_date('1/1/2015','mm/dd/yyyy');
         and to_number(to_char(p_date, 'Q')) <> 4; -- exclude last qtr,since expiration dates are going to begining of next year 01/01

    v_cnt NUMBER := 0;

  BEGIN
    FOR y IN get_data LOOP
      Update lw_pointtransaction pt
         set pt.expirationdate = add_months(TRUNC(y.transactiondate, 'YEAR'),
                                            12),
             pt.notes          = 'Expirationdate changed on ' ||
                                 to_char(trunc(p_date)) ||
                                 ' due to Dollar Reward Tier Merge',
             pt.ptchangedby    = 'DollarRwdMerge'
       where pt.pointtransactionid = y.pointtransactionid;
      v_cnt := v_cnt + 1;
      IF MOD(v_cnt, 1000) = 0 THEN
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
      END IF;

    END LOOP;
    COMMIT;
    IF get_data%ISOPEN THEN
      --<--- dont forget to close cursor since we manually opened it.
      CLOSE get_data;
    END IF;
    COMMIT;
  END Proc_adjust_expdatemerge;
  --Changes for PI 30364 - Dollar reward program start here -- Akbar
  PROCEDURE Proc_adjust_neg_balance_new(p_ProcessDate IN DATE,
                                        Retval        IN OUT Rcursor) IS
    v_adj_pointtypeid      NUMBER;
    v_adj_bonuspointtypeid NUMBER;
    v_adj_pointeventid     NUMBER;

    CURSOR get_data IS
      SELECT Vc.Ipcode,
             CASE
               WHEN (Ae_Isinpilot(md.a_Extendedplaycode) = 0) THEN
                Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
               WHEN Ae_Isinpilot(md.a_Extendedplaycode) = 1 THEN
            /*    Add_Months(Trunc(p_ProcessDate, 'Y'), 12)*/
                   to_date('12/31/2199','mm/dd/yyyy')
             END AS ExpirationDate,
             SUM(CASE
                   WHEN Ty.Name IN ('Basic Points', 'Adjustment Points','AEO Visa Card Points', 'AEO Connected Points') AND
                       /* Trunc(Pts.Transactiondate) >=
                        (Select to_date(to_char(value), 'mm/dd/yyyy')
                           from lw_clientconfiguration cc
                          where cc.key = 'CalculateTransactionStartDate') AND*/
                        Pts.Transactiondate <
                        Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                        Pts.Expirationdate > p_ProcessDate THEN
                    Pts.Points
                   ELSE
                    0
                 END) AS TotalBasicPoints,
             SUM(CASE
                   WHEN Ty.Name IN ('StartingPoints',
                                    'CS Points',
                                    'Bonus Points',
                                    'Adjustment Bonus Points',
                                    'AEO Customer Service Points',
                                    'AEO Connected Bonus Points') AND
                       /* Trunc(Pts.Transactiondate) >=
                        (Select to_date(to_char(value), 'mm/dd/yyyy')
                           from lw_clientconfiguration cc
                          where cc.key = 'CalculateTransactionStartDate') AND*/
                        Pts.Transactiondate <
                        Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                        Pts.Expirationdate > p_ProcessDate THEN
                    Pts.Points
                   ELSE
                    0
                 END) AS TotalBonusPoints
        FROM Lw_Pointtransaction Pts,
             Lw_Virtualcard      Vc,
             Lw_Pointtype        Ty,
             Ats_Memberdetails   Md
       WHERE Pts.Vckey = Vc.Vckey
         AND Pts.Pointtypeid = Ty.Pointtypeid
         AND Vc.Ipcode = Md.a_Ipcode
         AND Pts.Expirationdate > p_ProcessDate
       GROUP BY Vc.Ipcode, Md.a_extendedplaycode;
    v_balance         NUMBER := 0;
    v_adjustment      NUMBER := 0;
    v_vckey           NUMBER := 0;
    v_expiration_date DATE := to_date('1900', 'yyyy');
    v_cnt             NUMBER := 0;
  BEGIN
   /* Code commented because Dollar rewards program is over*/
   /* Proc_adjust_expdateoptout(p_date => p_ProcessDate, Retval => Retval);
    Proc_adjust_expdatemerge(p_date => p_ProcessDate, Retval => Retval);*/

		Proc_adjust_neg_brapoints(p_ProcessDate => p_ProcessDate, Retval => Retval); -- Changes for PI 31769 bra points adjustments

    /* aquire adjustment point type id */
    SELECT pointtypeid
      INTO v_adj_pointtypeid
      FROM lw_pointtype
     WHERE NAME = 'Adjustment Points';

    /* aquire adjustment bonus point type id */
    SELECT pointtypeid
      INTO v_adj_bonuspointtypeid
      FROM lw_pointtype
     WHERE NAME = 'Adjustment Bonus Points';

    /* point event is AE Return for all adjustments */
    SELECT pe.pointeventid
      INTO v_adj_pointeventid
      FROM lw_pointevent pe
     WHERE pe.name = 'AE Return';

    FOR y IN get_data LOOP
      SELECT vc.vckey
        INTO v_vckey
        FROM lw_virtualcard vc
       WHERE vc.ipcode = y.ipcode
         AND ROWNUM = 1
       ORDER BY vc.isprimary DESC;

      IF v_vckey IS NOT NULL THEN
        v_balance         := 0;
        v_adjustment      := 0;
        v_expiration_date := y.ExpirationDate;
      END IF;
      /* checking balance */
      v_balance := y.totalbasicpoints + y.totalbonuspoints;
      /* if balance dips below zero then we need adjusting */
      IF v_balance < 0 THEN
        /* if Bonus Points dips below zero then use Adjustment Bonus Points */
        IF y.totalbasicpoints >= 0 AND y.totalbonuspoints < 0 THEN
          v_adjustment := ABS(v_balance); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
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
                --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_bonuspointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             p_ProcessDate,
             p_ProcessDate,
             v_adjustment /*Points*/,
             v_expiration_date,
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);
        END IF;
        /* if Basic Points dips below zero then use Adjustment Bonus Points */
        IF y.totalbasicpoints < 0 AND y.totalbonuspoints >= 0 THEN
          v_adjustment := ABS(v_balance); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
             VcKey,
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
                --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_pointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             p_ProcessDate,
             p_ProcessDate,
             v_adjustment /*Points*/,
             v_expiration_date,
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);
        END IF;
        /* if both Basic Points and Bonus Points dip below zero then make seperate adjustments using Adjustment Points and Adjustment Bonus Points */
        IF y.totalbasicpoints < 0 AND y.totalbonuspoints < 0 THEN
          v_adjustment := ABS(y.totalbonuspoints); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
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
                --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_bonuspointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             p_ProcessDate,
             p_ProcessDate,
             v_adjustment /*Points*/,
             v_expiration_date,
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);

          v_adjustment := ABS(y.totalbasicpoints); /* aquire positive amount needed to zero out balance */
          INSERT INTO lw_pointtransaction
            (Pointtransactionid,
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
                 --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
             Expirationreason)
          VALUES
            (seq_pointtransactionid.nextval,
             v_vckey,
             v_adj_pointtypeid /*Pointtypeid*/,
             v_adj_pointeventid /*pointeventid=AE Return*/,
             2 /*Return*/,
             p_ProcessDate,
             p_ProcessDate,
             v_adjustment /*Points*/,
             v_expiration_date,
             'Negative balance adjustment' /*Notes*/,
             0,
             -1,
             -1,
             -1,
             0 /*Pointsconsumed*/,
             0 /*Pointsonhold*/,
             NULL,
             'NegativeBalADJ' /*Ptchangedby*/,
             SYSDATE,
             NULL);
        END IF;
        v_balance := 0; /* zero running balance */
        v_cnt     := v_cnt + 1;
        IF MOD(v_cnt, 100) = 0 THEN
          COMMIT;
        END IF;
      END IF;
    END LOOP;

    COMMIT;
  END Proc_adjust_neg_balance_new;
  --Changes for PI 30364 - Dollar reward program end here -- Akbar

  -- Changes for PI 31769 bra points adjustments - start
  PROCEDURE Proc_adjust_neg_brapoints(p_ProcessDate IN DATE,
                                      Retval        IN OUT Rcursor) IS
    v_bra_adj_pointtypeid  NUMBER;
    v_bra_adj_pointeventid NUMBER;

    CURSOR get_data IS
      SELECT Vc.Ipcode,
             SUM(CASE
                   WHEN Ty.Name IN ('Bra Points', 'Bra Adjustment Points') AND
                        TRUNC(Pts.Transactiondate) >= TO_DATE('01/01/' || EXTRACT(YEAR FROM(p_ProcessDate)), 'MM/DD/YYYY') AND
                        Pts.Transactiondate < TO_DATE('01/02/' || (EXTRACT(YEAR FROM(p_ProcessDate)) + 1), 'MM/DD/YYYY') AND
                        Pts.Expirationdate > p_ProcessDate THEN
                    Pts.Points
                   ELSE
                    0
                 END) AS TotalBraPoints,
             TO_DATE('01/02/' || (EXTRACT(YEAR FROM(p_ProcessDate)) + 1), 'MM/DD/YYYY') AS ExpirationDate
        FROM Lw_Pointtransaction Pts, Lw_Virtualcard Vc, Lw_Pointtype Ty, ats_memberdetails md
       WHERE Pts.Vckey = Vc.Vckey
         AND Pts.Pointtypeid = Ty.Pointtypeid
         AND Pts.Expirationdate > SYSDATE
		 AND Vc.IPCODE = md.a_Ipcode AND Ae_Isinpilot(md.a_Extendedplaycode) = 0
       GROUP BY Vc.Ipcode
      HAVING SUM(CASE
        WHEN Ty.Name IN ('Bra Points', 'Bra Adjustment Points') AND
              TRUNC(Pts.Transactiondate) >= TO_DATE('01/01/' || EXTRACT(YEAR FROM(p_ProcessDate)), 'MM/DD/YYYY') AND
              Pts.Transactiondate < TO_DATE('01/02/' || (EXTRACT(YEAR FROM(p_ProcessDate)) + 1), 'MM/DD/YYYY') AND
              Pts.Expirationdate > p_ProcessDate THEN
         Pts.Points
        ELSE
         0
      END) < 0;

    v_balance    NUMBER := 0;
    v_adjustment NUMBER := 0;
    v_ipcode     NUMBER := 0;
    v_vckey      NUMBER := 0;
    v_cnt        NUMBER := 0;
  BEGIN
    /* aquire bra adjustment point type id */
    SELECT pointtypeid
      INTO v_bra_adj_pointtypeid
      FROM lw_pointtype
     WHERE NAME = 'Bra Adjustment Points';

    /* point event is Bra Return for all adjustments */
    SELECT pe.pointeventid
      INTO v_bra_adj_pointeventid
      FROM lw_pointevent pe
     WHERE pe.name = 'Bra Return';

    FOR y IN get_data LOOP
      /* if new member is found then reset variables, also initial set on first record */
      IF y.Ipcode != v_ipcode THEN
        v_adjustment := 0;
        v_ipcode     := y.Ipcode;
        SELECT vc.vckey
          INTO v_vckey
          FROM lw_virtualcard vc
         WHERE vc.ipcode = v_ipcode
           AND ROWNUM = 1
         ORDER BY vc.isprimary DESC;
      END IF;
      v_adjustment := ABS(y.TotalBraPoints); /* aquire positive amount needed to zero out balance */
      INSERT INTO lw_pointtransaction
        (Pointtransactionid,
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
              --         Last_Dml_Date,   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
         createdate,
         --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
         Expirationreason)
      VALUES
        (seq_pointtransactionid.nextval,
         v_vckey,
         v_bra_adj_pointtypeid /*Pointtypeid*/,
         v_bra_adj_pointeventid /*pointeventid=Bra Return*/,
         2 /*Return*/,
         SYSDATE,
         SYSDATE,
         v_adjustment /*Points*/,
         y.ExpirationDate,
         'Negative balance adjustment' /*Notes*/,
         0,
         -1,
         -1,
         -1,
         0 /*Pointsconsumed*/,
         0 /*Pointsonhold*/,
         NULL,
         'NegativeBalADJ' /*Ptchangedby*/,
         SYSDATE,
         NULL);
      v_adjustment := 0; /* zero running balance */
      v_cnt        := v_cnt + 1;
      IF MOD(v_cnt, 1000) = 0 THEN
        COMMIT;
      END IF;
    END LOOP;
    COMMIT;
  END Proc_adjust_neg_brapoints;
  -- Changes for PI 31769 bra points adjustments - end
END POINTADJUSTMENT_PKG;
/

