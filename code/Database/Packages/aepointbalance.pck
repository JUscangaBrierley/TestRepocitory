CREATE OR REPLACE PACKAGE AEPointBalance IS

  TYPE Rcursor IS REF CURSOR;
  PROCEDURE CalculatePointBalances_1(p_Jobname     VARCHAR2,
                                   p_ProcessDate Date,
                                   Retval        IN OUT Rcursor);
  PROCEDURE CalculatePointBalances_2(p_Jobname     VARCHAR2,
                                   p_ProcessDate Date,
                                   Retval        IN OUT Rcursor);


END AEPointBalance;
/

CREATE OR REPLACE PACKAGE BODY AEPointBalance IS


  /********************************************************************
  **This is the new CalculatePointBalance proc that loads another point
  **transaction table with only the last 2 quarters in it to allow the query
  **to run faster for the jobs that just need the current and previous
  **quarter data.  Then #2 will update the rest of the bra counts using
  **the full points table.
  ********************************************************************/
PROCEDURE CalculatePointBalances_1(p_Jobname     VARCHAR2,
                                   p_ProcessDate Date,
                                   Retval        IN OUT Rcursor)

   AS
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
    v_Jobname          VARCHAR2(256) := 'CalculatePointBalances_1';
    v_RewardLevel_15   NUMBER;
    v_RewardLevel_20   NUMBER;
    v_RewardLevel_30   NUMBER;
    v_RewardLevel_40   NUMBER;
    v_RewardName_15    varchar2(10) := '15%';
    v_RewardName_20    varchar2(10) := '20%';
    v_RewardName_30    varchar2(10) := '30%';
    v_RewardName_40    varchar2(10) := '40%';

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

    EXECUTE IMMEDIATE 'Truncate Table AE_CurrentPointTransaction';

    Insert Into AE_CurrentPointTransaction
    Select /*+ append */ * From Lw_Pointtransaction pt
    Where pt.transactiondate > Add_Months(trunc(p_ProcessDate, 'Q'), -3);

    EXECUTE IMMEDIATE 'Truncate Table Ats_Memberpointbalances';

    INSERT INTO Ats_Memberpointbalances pb
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
       a_PriorDayTotalPoints, -- PI 26302: Akbar, Calculating previous day points balance
       a_BraLifeTimeBalance,
       a_BraRollingBalance,
       a_JeansLifeTimeBalance,
       a_JeansRollingBalance,
       Statuscode,
       Createdate,
       Updatedate)
      SELECT /*+ append full (vc) full (md) full (Pts) NO_PUSH_PRED(Lw_Clientconfiguration) NO_PUSH_PRED(Ats_Memberbrapromocertsummary) NO_PUSH_PRED(Ats_Memberbrapromocerthistory) NO_PUSH_PRED(Ats_Memberbrapromosummary) NO_PUSH_PRED(Ats_Memberbrand)*/
       Vc.Ipcode AS Rowkey,
       Vc.Ipcode,
       Vc.Ipcode,
       GREATEST(SUM(CASE
                      WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                           Trunc(Pts.Transactiondate, 'Q') =
                           Add_Months(Trunc(p_ProcessDate, 'Q'), -3) THEN
                       Pts.Points
                      ELSE
                       0
                    END),
                0) AS Lastqtr_Points,
       SUM(CASE
             WHEN Ty.Name IN ('Basic Points', 'Adjustment Points') AND
                  Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
              Pts.Points
             ELSE
              0
           END) AS Basic_Points,
       SUM(CASE
             /* PI 25127, Akbar, accomodate new point type 'Adjustment Bonus Points' in bonus points */
             WHEN Ty.Name IN ('CS Points', 'Bonus Points', 'Adjustment Bonus Points') AND
                  Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
              Pts.Points
             ELSE
              0
           END) AS Bonus_Points,
       SUM(CASE
             WHEN Ty.Name = 'StartingPoints' AND
                  Trunc(Pts.Transactiondate, 'Q') = TRUNC(p_ProcessDate, 'Q') THEN
              Pts.Points
             ELSE
              0
           END) AS Starting_Points,
       CASE
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Sv_Rewardlevel_40 THEN
          Sv_Rewardname_40
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Sv_Rewardlevel_30 THEN
          Sv_Rewardname_30
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Sv_Rewardlevel_20 THEN
          Sv_Rewardname_20
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Sv_Rewardlevel_15 THEN
          Sv_Rewardname_15
       END AS Rewardlevel,
       CASE
         -- PI25109: Condition to check if the points > 500 (Sv_Rewardlevel_40) Begin
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) > Sv_Rewardlevel_40 THEN
           SUM(CASE
               WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
               Pts.Points
               ELSE
               0
               END)- Sv_Rewardlevel_40
           --PI25109: Condition to check if the points > 500 (Sv_Rewardlevel_40) End
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) BETWEEN Sv_Rewardlevel_30 AND (Sv_Rewardlevel_40 - 1) THEN
          Sv_Rewardlevel_40 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) BETWEEN Sv_Rewardlevel_20 AND (Sv_Rewardlevel_30 - 1) THEN
          Sv_Rewardlevel_30 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) BETWEEN Sv_Rewardlevel_15 AND (Sv_Rewardlevel_20 - 1) THEN
          Sv_Rewardlevel_20 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) < Sv_Rewardlevel_15 THEN
          Sv_Rewardlevel_15 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Sv_Rewardlevel_40 THEN
          0
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Sv_Rewardlevel_30 THEN
          (Sv_Rewardlevel_40 - Sv_Rewardlevel_30)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Sv_Rewardlevel_20 THEN
          (Sv_Rewardlevel_30 - Sv_Rewardlevel_20)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Sv_Rewardlevel_15 THEN
          Sv_Rewardlevel_15
       END AS Pointstonextreward,
       0 AS Bracurrentpurchased,
       Nvl(sv_Bratotalfreemailed, 0) AS sv_Bratotalfreemailed,
       sv_Bralastfreemailed AS sv_Bralastfreemailed,
       Nvl(Ats_Memberbrand.Brandae, 0) AS Brandae,
       Nvl(Ats_Memberbrand.Brandaerie, 0) AS Brandaerie,
       Nvl(Ats_Memberbrand.Brandkids, 0) AS Brandkids,
       0 AS Bracurrentfreeearned,
       DECODE(To_Number(Nvl(Md.a_Employeecode, 0)), 1, 0, 1) AS a_Braemployeemultiplier,
       DECODE(Md.a_Addressmailable, 1, 1, 0) a_Bramailablemultiplier,
       GREATEST(SUM(CASE
                      WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' AND
                           Trunc(Pts.Transactiondate, 'Q') = Trunc(p_ProcessDate, 'Q') THEN
                       Pts.Points
                      ELSE
                       0
                    END),
                0) AS Ttl_Points,
       -- PI 26302: Akbar, Calculating previous day points balance Begin
       GREATEST(SUM(CASE
                      WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' AND
                           Trunc(Pts.Pointawarddate) BETWEEN Trunc(p_ProcessDate, 'Q') AND Trunc(p_ProcessDate - 1) THEN
                       Pts.Points
                      ELSE
                       0
                    END),
                0) AS PriorDay_Points,
       -- PI 26302: Akbar, Calculating previous day points balance End
       0 AS BraLifeTimeBalance,
       0 AS BraRollingBalance,
       0 AS JeansLifeTimeBalance,
       0 AS JeansRollingBalance,
       1 AS Statuscode,
       SYSDATE AS Createdate,
       SYSDATE AS Updatedate
        FROM AE_CurrentPointTransaction Pts,
             Lw_Virtualcard Vc,
             Lw_Pointtype Ty,
             Ats_Memberdetails Md,
             (SELECT CASE WHEN SUM(CASE   --PI25722 FLAG FIX BEGINS HERE--
                           WHEN b.a_Shortbrandname = 'AE' THEN
                            1
                           ELSE
                            0
                         END) >= 1 THEN 1 ELSE 0 END AS Brandae,
                 CASE WHEN SUM(CASE
                           WHEN b.a_Shortbrandname = 'aerie' THEN
                            1
                           ELSE
                            0
                         END)>= 1 THEN 1 ELSE 0 END AS Brandaerie,
                 CASE WHEN SUM(CASE
                           WHEN b.a_Shortbrandname = '77kids' THEN
                            1
                           ELSE
                            0
                         END) >= 1 THEN 1 ELSE 0 END AS Brandkids,  --PI25722 FLAG FIX ENDS HERE--
                     Mb.a_Ipcode
                FROM Ats_Memberbrand Mb, Ats_Refbrand b
               WHERE Mb.a_Brandid = b.a_Brandid
               GROUP BY Mb.a_Ipcode) Ats_Memberbrand,
             (SELECT SUM(CASE
                           WHEN rd.name Like 'Bra%' THEN
                            1
                           ELSE
                            0
                         END) AS sv_Bratotalfreemailed,
                     MAX(CASE
                           WHEN rd.name Like 'Bra%' THEN
                            mr.dateissued
                         END) AS sv_Bralastfreemailed,
                     mr.memberid
                FROM lw_memberrewards mr
               inner join lw_rewardsdef rd
                  on mr.rewarddefid = rd.id
               GROUP BY mr.memberid) lw_memberrewards,
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
                FROM Lw_Rewardsdef x) Lw_Rewardsdef
       WHERE Pts.Vckey = Vc.Vckey
         AND Pts.Pointtypeid = Ty.Pointtypeid
         AND Vc.Ipcode = Md.a_Ipcode
         AND md.a_ipcode = Ats_Memberbrand.a_ipcode(+)
         AND md.a_ipcode = lw_memberrewards.memberid(+)
         AND Transactiontype IN (1, 2, 4)
         AND Pts.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
         AND Pts.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
       GROUP BY Vc.Ipcode,
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
                sv_Bratotalfreemailed,
                sv_Bralastfreemailed
      HAVING SUM(CASE
        WHEN Trunc(Pts.Transactiondate, 'Q') =
             Trunc(p_ProcessDate, 'Q') THEN
         Pts.Points
        WHEN Trunc(Pts.Transactiondate, 'Q') =
             Add_Months(Trunc(p_ProcessDate, 'Q'), -3) THEN
         Pts.Points
        ELSE
         0
      END) > 0;

    Commit;

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

    v_Endtime := sysdate;
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

  END CalculatePointBalances_1;

  /********************************************************************
  **This is the 2nd new CalculatePointBalance proc that only updates the
  **bra counts using the full point transaction table
  ********************************************************************/
PROCEDURE CalculatePointBalances_2(p_Jobname     VARCHAR2,
                                   p_ProcessDate Date,
                                   Retval        IN OUT Rcursor)

   AS
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
    v_Jobname          VARCHAR2(256) := 'CalculatePointBalances_2';

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
      INTO v_Fulfillmentthreshold
      FROM Lw_Clientconfiguration c
     WHERE c.Key = 'BraPromoFulFillmentThreshold';

    Update Ats_Memberpointbalances pb
       Set a_Bracurrentpurchased = (SELECT GREATEST(SUM(
                     CASE
                      WHEN Ty.Name IN ('Bra Points', 'Bra Adjustment Points') AND pt.Expirationdate > p_ProcessDate and pt.Transactiontype in (1, 2, 4) THEN pt.Points
                      ELSE 0
                     END),
                     0) From LW_PointTransaction pt,
                             Lw_Virtualcard vc,
                             Lw_Pointtype Ty
                        where vc.ipcode = pb.a_ipcode
                        and   pt.vckey = vc.Vckey
                        and   pt.Pointtypeid = Ty.Pointtypeid
                        and   Transactiontype IN (1, 2, 4)
                        and   pt.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
                        and   pt.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
                        ),
       pb.a_bracurrentfreeearned = (SELECT GREATEST(Round(SUM(
                     CASE
                      WHEN Ty.Name IN ('Bra Points', 'Bra Adjustment Points') AND pt.Expirationdate > p_ProcessDate and pt.Transactiontype in (1, 2, 4) THEN pt.Points
                      ELSE 0
                     END) / v_Fulfillmentthreshold,
                      0),
                     0) From LW_PointTransaction pt,
                             Lw_Virtualcard vc,
                             Lw_Pointtype Ty
                        where vc.ipcode = pb.a_ipcode
                        and   pt.vckey = vc.Vckey
                        and   pt.Pointtypeid = Ty.Pointtypeid
                        and   Transactiontype IN (1, 2, 4)
                        and   pt.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
                        and   pt.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
                        ),
       a_BraLifeTimeBalance = (SELECT GREATEST(SUM(
                     CASE
                      WHEN Ty.Name IN ('Bra Points',
                                       'Bra Adjustment Points',
                                       'Bra Legacy Points',
                                       'Bra Redemptions',
                                       'Bra Employee Points') AND
                           pt.Transactiondate > to_date('1/1/2009', 'mm/dd/yyyy') and
                           pt.Transactiontype in (1, 2) THEN pt.Points
                      ELSE 0
                    END),
                     0) From LW_PointTransaction pt,
                             Lw_Virtualcard vc,
                             Lw_Pointtype Ty
                        where vc.ipcode = pb.a_ipcode
                        and   pt.vckey = vc.Vckey
                        and   pt.Pointtypeid = Ty.Pointtypeid
                        and   Transactiontype IN (1, 2, 4)
                        and   pt.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
                        and   pt.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
                        ),
       a_BraRollingBalance = (SELECT GREATEST(SUM(
                      CASE
                       WHEN Ty.Name IN ('Bra Points',
                                       'Bra Adjustment Points',
                                       'Bra Redemptions',
                                       'Bra Employee Points') AND
                           pt.Transactiondate >
                           Add_Months(Trunc(p_ProcessDate, 'Q'), -12) and
                           pt.Transactiontype in (1, 2) THEN pt.Points
                      ELSE 0
                    END),
                     0) From LW_PointTransaction pt,
                             Lw_Virtualcard vc,
                             Lw_Pointtype Ty
                        where vc.ipcode = pb.a_ipcode
                        and   pt.vckey = vc.Vckey
                        and   pt.Pointtypeid = Ty.Pointtypeid
                        and   Transactiontype IN (1, 2, 4)
                        and   pt.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
                        and   pt.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
                        ),
       a_JeansLifeTimeBalance = (SELECT GREATEST(SUM(
                     CASE
                      WHEN Ty.Name IN ('Jean Points', 'Jean Legacy Points') AND
                           pt.Transactiondate > to_date('1/1/2009', 'mm/dd/yyyy') THEN pt.Points
                      ELSE 0
                    END),
                     0) From LW_PointTransaction pt,
                             Lw_Virtualcard vc,
                             Lw_Pointtype Ty
                        where vc.ipcode = pb.a_ipcode
                        and   pt.vckey = vc.Vckey
                        and   pt.Pointtypeid = Ty.Pointtypeid
                        and   Transactiontype IN (1, 2, 4)
                        and   pt.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
                        and   pt.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
                        ),
       a_JeansRollingBalance = (SELECT GREATEST(SUM(
                     CASE
                      WHEN Ty.Name IN ('Jean Points') AND pt.Transactiondate > Add_Months(Trunc(p_ProcessDate, 'Q'), -12) THEN pt.Points
                      ELSE 0
                    END),
                     0) From LW_PointTransaction pt,
                             Lw_Virtualcard vc,
                             Lw_Pointtype Ty
                        where vc.ipcode = pb.a_ipcode
                        and   pt.vckey = vc.Vckey
                        and   pt.Pointtypeid = Ty.Pointtypeid
                        and   Transactiontype IN (1, 2, 4)
                        and   pt.Transactiondate >= to_date('1/1/2009', 'mm/dd/yyyy')
                        and   pt.Transactiondate < Add_Months(Trunc(p_ProcessDate, 'Q'), 3)
                        );

    Commit;

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

    v_Endtime := sysdate;
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

  END CalculatePointBalances_2;




END AEPointBalance;
/

