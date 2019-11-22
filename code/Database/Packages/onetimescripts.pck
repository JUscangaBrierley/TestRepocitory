CREATE OR REPLACE PACKAGE BP_AE.OneTimeScripts IS

  TYPE Rcursor IS REF CURSOR;
  --Changes for PI 31509 BEGIN here  -- Rizwan
  PROCEDURE HEADERRECORD_MEMBERPOINTS_1(p_Dummy                VARCHAR2,
                                        p_ProcessDate          Date,
                                        p_MemberfileStarttime  Date,
                                        v_TransactionStartdate DATE);
  PROCEDURE MEMBERPOINTSDELTA_1(p_Dummy       VARCHAR2,
                                p_ProcessDate Date,
                                B_Name        varchar2,
                                Retval        IN OUT Rcursor);
  --Changes for PI 31509 END here  -- Rizwan

  --Changes for AEO 54 Begin
  PROCEDURE RevertDollarRewardMembersFlag(p_Dummy VARCHAR2,
                                          Retval  IN OUT Rcursor);
  --Changes for AEO 54 End
  PROCEDURE RevertDollarRwdfullfilldate(p_Dummy VARCHAR2,
                                           Retval  IN OUT Rcursor);
--**********UPDATED with SVN**********
  --Changes for AEO 1360 Begin
  PROCEDURE UpdateSMstatus(p_Dummy VARCHAR2, Retval  IN OUT Rcursor);
  PROCEDURE ResetSMstatus(p_Dummy VARCHAR2, Retval  IN OUT Rcursor);
  --Changes for AEO 1360 End

  PROCEDURE ExpireEmployees(p_Dummy      varchar2,
                            Retval       IN OUT Rcursor);

  --Changes for AEO AEO-1303 Begin
  PROCEDURE ResetAITupdateflag(filename VARCHAR2,
                                  Retval  IN OUT Rcursor);		
  --Changes for AEO AEO-1303 End
							
END OneTimeScripts;
/
CREATE OR REPLACE PACKAGE BODY BP_AE.OneTimeScripts IS

--**********UPDATED with SVN**********
  --Changes for AEO 1360 Begin
  PROCEDURE UpdateSMstatus (p_Dummy VARCHAR2,Retval  IN OUT Rcursor) IS
	CURSOR c IS SELECT ipcode FROM X$_AEO1360_SMmembers;
	TYPE c_type is TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
	rec c_type;
  BEGIN
	--Truncate table X$_AEO1360_SMmembers
	DELETE FROM X$_AEO1360_SMmembers;
	
	--Populate backup table
	INSERT INTO X$_AEO1360_SMmembers (ipcode)
		SELECT ipcode FROM lw_loyaltymember LM
		JOIN ats_memberdetails MD ON LM.IPCODE = MD.A_IPCODE
		WHERE MD.a_Pendingcellverification = 0
		AND MD.a_mobilephone IS NOT NULL
		AND length(MD.a_mobilephone) > 0
		AND MD.a_emailaddress is null
		AND LM.memberstatus = 1;
	
	OPEN c;
    LOOP

        FETCH c BULK COLLECT INTO rec LIMIT 100;			
			--Update system table
			FORALL indx IN 1 .. rec.COUNT
				UPDATE lw_loyaltymember 
				SET memberstatus = 4,
				statuschangedate = sysdate,
				statuschangereason = 'AEO-1360 UpdateSMstatus'
				WHERE ipcode = rec(indx).IPCODE;
			COMMIT;		
		EXIT WHEN rec.COUNT = 0;

    END LOOP;
	CLOSE c;
	
	COMMIT;
		
	EXCEPTION
		WHEN OTHERS THEN
		   dbms_output.put_line(SQLERRM);
		   ROLLBACK;
		   RAISE;
  END;
  PROCEDURE ResetSMstatus (p_Dummy VARCHAR2,Retval  IN OUT Rcursor) IS
	CURSOR c IS SELECT ipcode FROM X$_AEO1360_SMmembers;
	TYPE c_type is TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
	rec c_type;
  BEGIN
	OPEN c;
    LOOP

        FETCH c BULK COLLECT INTO rec LIMIT 100;			
			--Restore system table
			FORALL indx IN 1 .. rec.COUNT
				UPDATE lw_loyaltymember 
				SET memberstatus = 1,
				statuschangedate = sysdate,
				statuschangereason = 'AEO-1360 ResetSMstatus'
				WHERE ipcode = rec(indx).IPCODE;
			COMMIT WRITE batch NOWAIT;		
		EXIT WHEN rec.COUNT = 0;

    END LOOP;
	CLOSE c;
	
	COMMIT;
		
	EXCEPTION
		WHEN OTHERS THEN
		   dbms_output.put_line(SQLERRM);
		   ROLLBACK;
		   RAISE;
  END;
  --Changes for AEO 1360 End

  --Changes for PI 31509 BEGIN here  -- Rizwan
  PROCEDURE HEADERRECORD_MEMBERPOINTS_1(p_Dummy                VARCHAR2,
                                        p_ProcessDate          Date,
                                        p_MemberfileStarttime  Date,
                                        v_TransactionStartdate DATE) IS
    v_DollarRewardLevel  NUMBER := 150;
    v_DollarRewardCutoff NUMBER := 100;
    Mv_Rewardlevel_40    number := 500;
    Mv_Rewardlevel_30    number := 350;
    Mv_Rewardlevel_20    number := 200;
    Mv_Rewardlevel_15    number := 100;
    Mv_Rewardname_15     varchar2(10) := '15%';
    Mv_Rewardname_20     varchar2(10) := '20%';
    Mv_Rewardname_30     varchar2(10) := '30%';
    Mv_Rewardname_40     varchar2(10) := '40%';
    v_sql1               VARCHAR2(1000);
  BEGIN

    v_sql1 := ' Select to_char(value)  from lw_clientconfiguration cc where cc.key = ''DollarRewardsPoints''';
    EXECUTE IMMEDIATE v_sql1
      into v_DollarRewardLevel;
    EXECUTE IMMEDIATE 'Truncate Table AEpointheaderdelta';

    INSERT INTO AEpointheaderdelta
      (IPCODE,
       TXNCOUNT,
       RECORDTYPE,
       CUSTOMERKEY,
       LOYALTYIDNUMBER,
       TOTALPOINTS,
       REWARDLEVEL,
       POINTSTONEXTREWARD,
       BASEPOINTS,
       BONUSPOINTS)
      SELECT /*+ parallel(pts 8) */
       lm.ipcode,
       count(pts.vckey) as TxnCount,
       max('H') AS RecordType,
       max(vc2.linkkey) AS CustomerKey,
       vc2.loyaltyidnumber,
       /*            max(bl.a_totalpoints) AS TotalPoints, */
       GREATEST(SUM(CASE
                      WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                           Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                           Pts.Transactiondate <
                           Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                           Pts.Expirationdate > p_ProcessDate THEN
                       Pts.Points
                      ELSE
                       0
                    END),
                0) AS TotalPoints,
       --Rewardlevel
       CASE
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Mv_Rewardlevel_40 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardname_40
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Pts.Transactiondate >= v_TransactionStartdate AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Mv_Rewardlevel_30 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardname_30
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Mv_Rewardlevel_20 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardname_20
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= Mv_Rewardlevel_15 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
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
         WHEN md.a_extendedplaycode = 1 and
              Sum(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Transactiondate <
                         Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= v_DollarRewardLevel and
              to_char(floor(SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                  WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                       Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                       Pts.Transactiondate <
                                       Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                                       Pts.Expirationdate > p_ProcessDate THEN
                                   Pts.Points
                                  ELSE
                                   0
                                END) / v_DollarRewardLevel) * 10) <=
              v_DollarRewardCutoff THEN
          nullif(to_char(floor(SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                     WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                          Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                          Pts.Transactiondate <
                                          Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                                          Pts.Expirationdate > p_ProcessDate THEN
                                      Pts.Points
                                     ELSE
                                      0
                                   END) / v_DollarRewardLevel) * 10),
                 '0')
         WHEN md.a_extendedplaycode = 1 and
              Sum(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Transactiondate <
                         Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= v_DollarRewardLevel and
              to_char(floor(SUM(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                                  WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                       Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                       Pts.Transactiondate <
                                       Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                                       Pts.Expirationdate > p_ProcessDate THEN
                                   Pts.Points
                                  ELSE
                                   0
                                END) / v_DollarRewardLevel) * 10) >
              v_DollarRewardCutoff THEN
          to_char(v_DollarRewardCutoff)
         WHEN md.a_extendedplaycode = 1 and
              Sum(CASE -- nullif is used to change if final value is zero to a null,floor gets the quotient of total points/v_DollarRewardLevel
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Transactiondate <
                         Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) < v_DollarRewardLevel THEN
          null
       END AS Rewardlevel,
       --PointsToNextReward
       CASE
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) > Mv_Rewardlevel_40 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          0
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) BETWEEN Mv_Rewardlevel_30 AND (Mv_Rewardlevel_40 - 1) and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardlevel_40 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                         Pts.Expirationdate > p_ProcessDate THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) BETWEEN Mv_Rewardlevel_20 AND (Mv_Rewardlevel_30 - 1) and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardlevel_30 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                         Pts.Expirationdate > p_ProcessDate THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) BETWEEN Mv_Rewardlevel_15 AND (Mv_Rewardlevel_20 - 1) and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardlevel_20 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                         Pts.Expirationdate > p_ProcessDate THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) < Mv_Rewardlevel_15 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardlevel_15 - SUM(CASE
                                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                         Pts.Expirationdate > p_ProcessDate THEN
                                     Pts.Points
                                    ELSE
                                     0
                                  END)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Mv_Rewardlevel_40 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          0
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Mv_Rewardlevel_30 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          (Mv_Rewardlevel_40 - Mv_Rewardlevel_30)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Mv_Rewardlevel_20 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          (Mv_Rewardlevel_30 - Mv_Rewardlevel_20)
         WHEN SUM(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) = Mv_Rewardlevel_15 and
              (md.a_extendedplaycode <> 1 or md.a_extendedplaycode is null) THEN
          Mv_Rewardlevel_15

         WHEN md.a_extendedplaycode = 1 and
              Sum(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Transactiondate <
                         Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) >= 0 Then
          CheckDollarRewardLevel(SUM(CASE
                                       WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                                            Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                                            Pts.Expirationdate > p_ProcessDate AND
                                            Pts.Transactiondate <
                                            Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) THEN
                                        Pts.Points
                                       ELSE
                                        0
                                     END)) -
          SUM(CASE
                WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                     Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                     Pts.Expirationdate > p_ProcessDate AND
                     Pts.Transactiondate <
                     Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) THEN
                 Pts.Points
                ELSE
                 0
              END)

         WHEN md.a_extendedplaycode = 1 and
              Sum(CASE
                    WHEN Ty.name not like 'Jean%' and Ty.name not like 'Bra%' and
                         Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                         Pts.Transactiondate <
                         Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                         Pts.Expirationdate > p_ProcessDate THEN
                     Pts.Points
                    ELSE
                     0
                  END) < 0 Then
          v_DollarRewardLevel
       END AS Pointstonextreward,
       --BasePoints
       SUM(CASE
             WHEN Ty.Name IN ('Basic Points', 'Adjustment Points') AND
                  Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                  Pts.Transactiondate <
                  Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                  Pts.Expirationdate > p_ProcessDate THEN
              Pts.Points
             ELSE
              0
           END) AS BasePoints,
       --BonusPoints
       SUM(CASE
             WHEN Ty.Name IN
                  ('CS Points', 'Bonus Points', 'Adjustment Bonus Points') AND
                  Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate) AND
                  Pts.Transactiondate <
                  Add_Months(Trunc(p_ProcessDate, 'Mon'), 1) AND
                  Pts.Expirationdate > p_ProcessDate THEN
              Pts.Points
             ELSE
              0
           END) AS BonusPoints
        FROM AE_CurrentPointTransaction2 pts
       INNER JOIN lw_virtualcard vc
          on vc.vckey = pts.vckey
       INNER JOIN LW_POINTTYPE TY
          ON TY.POINTTYPEID = PTS.POINTTYPEID
       INNER JOIN lw_loyaltymember lm
          ON lm.ipcode = vc.ipcode
       INNER JOIN ATS_MEMBERDETAILS MD
          ON MD.A_IPCODE = VC.IPCODE
       INNER JOIN (select * from lw_virtualcard vc1 where vc1.isprimary = 1) vc2
          on vc.ipcode = vc2.ipcode
       INNER JOIN (select distinct (vc5.ipcode)
                     from lw_virtualcard vc5
                    inner join (select distinct (pta.vckey)
                                 from AE_CurrentPointTransaction2 pta
                                where pta.pointawarddate >=
                                      to_date('07/01/2014', 'mm/dd/yyyy')
                                  and pta.pointawarddate <
                                      to_date('08/20/2014', 'mm/dd/yyyy')
                                  AND Pta.points < 0) ptb
                       on vc5.vckey = ptb.vckey
                   union
                   select DR.A_IPCODE
                     from ATS_MEMBERDOLLARREWARDOPTOUT DR
                    where DR.CREATEDATE >= p_MemberfileStarttime) vc6
          on vc.ipcode = vc6.ipcode
       INNER JOIN lw_pointtype pnt
          on pnt.pointtypeid = pts.pointtypeid
      --    FULL OUTER JOIN (select * from  ATS_MEMBERDOLLARREWARDOPTOUT DR where DR.CREATEDATE >= p_MemberfileStarttime ) DR1 on DR1.A_IPCODE = vc.ipcode
       WHERE pnt.name NOT LIKE 'Jean%'
         and pnt.name NOT LIKE 'Bra%'
         AND Trunc(Pts.Transactiondate) >= Trunc(v_TransactionStartdate)
         AND Pts.Transactiondate <
             Add_Months(Trunc(p_ProcessDate, 'Mon'), 1)

      -- AND pts.transactiondate < ADD_MONTHS(TRUNC(p_ProcessDate, 'Q'), 3)
       GROUP BY lm.ipcode, vc2.loyaltyidnumber, md.a_extendedplaycode;
    COMMIT;
  END HEADERRECORD_MEMBERPOINTS_1;

  PROCEDURE MEMBERPOINTSDELTA_1(p_Dummy       VARCHAR2,
                                p_ProcessDate Date,
                                B_Name        varchar2,
                                Retval        IN OUT Rcursor) AS
    pdummy                 varchar2(2);
    pProcessDate           Date := p_ProcessDate;
    v_sql1                 VARCHAR2(1000);
    v_sql2                 VARCHAR2(1000);
    v_sql3                 VARCHAR2(1000);
    v_MemberfileStarttime  DATE;
    v_TransactionStartdate DATE;

  BEGIN
    v_sql1 := 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where trunc(pt.transactiondate) >= Add_Months(trunc(sysdate, ''Q''), -15)';
    v_sql2 := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'')  from lw_clientconfiguration cc where cc.key = ''LastAEMemberPointsSent''';
    v_sql3 := 'Select to_date(to_char(value), ''mm/dd/yyyy'')  from lw_clientconfiguration cc where cc.key = ''CalculateTransactionStartDate''';
    EXECUTE IMMEDIATE 'Drop Table AE_CurrentPointTransaction2';
    EXECUTE IMMEDIATE v_sql1;
    EXECUTE IMMEDIATE v_sql2
      into v_MemberfileStarttime;
    EXECUTE IMMEDIATE v_sql3
      into v_TransactionStartdate;
    --UpdateLastAEMemberPointsSent(pdummy);
    EXECUTE IMMEDIATE 'Truncate Table  ae_memberpointsDELTA';
    HEADERRECORD_MEMBERPOINTS_1(pdummy,
                                pProcessDate,
                                v_MemberfileStarttime,
                                v_TransactionStartdate);
    INSERT INTO ae_memberpointsDELTA
      (RECORDTYPE,
       CUSTOMERKEY,
       LOYALTYIDNUMBER,
       TOTALPOINTS,
       REWARDLEVEL,
       POINTSTONEXTREWARD,
       DESCRIPTION,
       BASEPOINTS,
       BONUSPOINTS,
       TXNDATE,
       TXNNUMBER,
       ORDERNUMBER,
       STORENUMBER,
       REGISTERNUMBER)
      SELECT mem.RecordType, -- member header with primary card if any, else any other available card
             mem.CustomerKey,
             mem.LoyaltyIDNumber,
             mem.TotalPoints,
             mem.RewardLevel,
             mem.PointsToNextReward,
             NULL AS Description,
             mem.BasePoints,
             mem.BonusPoints,
             CASE
               WHEN B_Name = 'QTR' THEN
                TO_CHAR(pProcessDate, 'MMDDYYYY')
               ELSE
                TO_CHAR(pProcessDate - 1, 'MMDDYYYY')
             END AS TXNDate,
             NULL AS TXNNumber,
             NULL AS OrderNumber,
             NULL AS StoreNumber,
             NULL AS RegisterNumber
        FROM AEpointheaderdelta mem
       WHERE mem.TxnCount > 0
      UNION ALL
      SELECT 'T' AS RecordType, -- point transactions against any txnheader
             mem.CustomerKey,
             mem.LoyaltyIDNumber,
             NULL AS TotalPoints,
             NULL AS RewardLevel,
             NULL AS PointsToNextReward,
             pt.pointeventid AS Description,
             CASE
               WHEN p.name IN ('Basic Points', 'Adjustment Points') THEN
                NVL(pt.points, 0)
             END AS BasePoints,
             CASE
               WHEN p.name IN
                    ('CS Points', 'Bonus Points', 'Adjustment Bonus Points') THEN
                NVL(pt.points, 0)
             END AS BonusPoints,
             TO_CHAR(pt.transactiondate, 'MMDDYYYY') AS TXNDate,
             h.a_txnnumber AS TXNNumber,
             h.a_ordernumber AS OrderNumber,
             h.a_storenumber AS StoreNumber,
             h.a_txnregisternumber AS RegisterNumber
        FROM lw_virtualcard vc
       INNER JOIN AEpointheaderdelta mem
          ON mem.ipcode = vc.ipcode
       INNER JOIN AE_CurrentPointTransaction2 pt
          ON vc.vckey = pt.vckey
       INNER JOIN lw_pointtype p
          ON pt.pointtypeid = p.pointtypeid
       INNER JOIN ats_txnheader h
          ON pt.rowkey = h.a_rowkey
      -- WHERE pt.transactiondate >= TRUNC(p_ProcessDate, 'Q') and pt.transactiondate < ADD_MONTHS(TRUNC(p_ProcessDate, 'Q'), 3)
       WHERE Trunc(Pt.Transactiondate) >= Trunc(v_TransactionStartdate)
         AND Pt.Transactiondate <
             Add_Months(Trunc(p_ProcessDate, 'Mon'), 1)
         AND Pt.Expirationdate > p_ProcessDate
      UNION all
      SELECT 'T' AS RecordType, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
             mem.CustomerKey,
             mem.LoyaltyIDNumber,
             NULL AS TotalPoints,
             NULL AS RewardLevel,
             NULL AS PointsToNextReward,
             pt.pointeventid AS Description,
             CASE
               WHEN p.name IN ('Basic Points', 'Adjustment Points') THEN
                NVL(pt.points, 0)
             END AS BasePoints,
             CASE
               WHEN p.name IN
                    ('CS Points', 'Bonus Points', 'Adjustment Bonus Points') THEN
                NVL(pt.points, 0)
             END AS BonusPoints,
             TO_CHAR(pt.transactiondate, 'MMDDYYYY') AS TXNDate,
             NULL AS TXNNumber,
             NULL AS OrderNumber,
             NULL AS StoreNumber,
             NULL AS RegisterNumber
        FROM lw_virtualcard vc
       INNER JOIN AEpointheaderdelta mem
          ON mem.ipcode = vc.ipcode
       INNER JOIN AE_CurrentPointTransaction2 pt
          ON vc.vckey = pt.vckey
       INNER JOIN lw_pointtype p
          ON pt.pointtypeid = p.pointtypeid
       INNER JOIN ats_txndetailitem dtl
          ON pt.rowkey = dtl.a_rowkey
       WHERE Trunc(Pt.Transactiondate) >= Trunc(v_TransactionStartdate)
         AND Pt.Transactiondate <
             Add_Months(Trunc(p_ProcessDate, 'Mon'), 1)
         AND Pt.Expirationdate > p_ProcessDate
         AND pt.pointtypeid IN
             (SELECT pointtypeid
                FROM lw_pointtype
               WHERE name NOT LIKE 'Jean%'
                 AND name NOT LIKE 'Bra%')
      UNION all
      SELECT 'T' AS RecordType, -- txnheader independent point transactions, like Email, SMS bonuses etc
             mem.CustomerKey,
             mem.LoyaltyIDNumber,
             NULL AS TotalPoints,
             NULL AS RewardLevel,
             NULL AS PointsToNextReward,
             pt.pointeventid AS Description,
             CASE
               WHEN p.name IN ('Basic Points', 'Adjustment Points') THEN
                NVL(pt.points, 0)
             END AS BasePoints,
             CASE
               WHEN p.name IN
                    ('CS Points', 'Bonus Points', 'Adjustment Bonus Points') THEN
                NVL(pt.points, 0)
             END AS BonusPoints,
             TO_CHAR(pt.transactiondate, 'MMDDYYYY') AS TXNDate,
             NULL AS TXNNumber,
             NULL AS OrderNumber,
             NULL AS StoreNumber,
             NULL AS RegisterNumber
        FROM lw_virtualcard vc
       INNER JOIN AEpointheaderdelta mem
          ON mem.ipcode = vc.ipcode
       INNER JOIN AE_CurrentPointTransaction2 pt
          ON vc.vckey = pt.vckey
       INNER JOIN lw_pointtype p
          ON pt.pointtypeid = p.pointtypeid
       WHERE Trunc(Pt.Transactiondate) >= Trunc(v_TransactionStartdate)
         AND Pt.Transactiondate <
             Add_Months(Trunc(p_ProcessDate, 'Mon'), 1)
         AND Pt.Expirationdate > p_ProcessDate
         AND pt.rowkey IN ('-1')
         AND pt.pointtypeid IN
             (SELECT pointtypeid
                FROM lw_pointtype
               WHERE name NOT LIKE 'Jean%'
                 AND name NOT LIKE 'Bra%')
       ORDER BY LoyaltyIDNumber, RecordType;
    COMMIT;
  END MEMBERPOINTSDELTA_1;
  --Changes for PI 31509 END here  -- Rizwan

  --Changes for AEO 54 Begin

  PROCEDURE RevertDollarRewardMembersFlag(p_Dummy VARCHAR2,
                                          Retval  IN OUT Rcursor) AS

  BEGIN

    EXECUTE IMMEDIATE 'ALTER TRIGGER ats_memberdetails_arc DISABLE';

    DECLARE
      CURSOR get_data IS
        SELECT md.a_ipcode
          FROM ats_memberdetails md
         WHERE NVL(md.a_extendedplaycode, 0) = 1;

      TYPE t_tab IS TABLE OF get_data%ROWTYPE;
      v_tbl t_tab;

    BEGIN

      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 1000;

        FORALL i IN 1 .. v_tbl.COUNT
          UPDATE ats_memberdetails md
             SET md.a_extendedplaycode = 3,
                 md.updatedate         = sysdate,
                 md.a_changedby        = 'Updating dollar reward members flag to 3 from 1'
           WHERE md.a_ipcode = v_tbl(i).a_ipcode;

        COMMIT;
        EXIT WHEN get_data%NOTFOUND;
      END LOOP;

      COMMIT;

      IF get_data%ISOPEN THEN
        CLOSE get_data;
      END IF;
    END;

    EXECUTE IMMEDIATE 'ALTER TRIGGER ats_memberdetails_arc ENABLE';

  END RevertDollarRewardMembersFlag;

--Changes for AEO 54 End
 PROCEDURE RevertDollarRwdfullfilldate(p_Dummy VARCHAR2,
                                          Retval  IN OUT Rcursor) AS

 BEGIN
   DECLARE
      CURSOR get_data
      IS
        select pt.pointtransactionid from lw_pointtransaction pt where pt.pointeventid in
        (select pe.pointeventid from bp_ae.lw_pointevent pe where pe.name = 'Dollar Reward')
         and trunc(pt.pointawarddate) = trunc(sysdate);

      TYPE t_tab IS TABLE OF get_data%ROWTYPE;

      v_tbl   t_tab;
   BEGIN
      OPEN get_data;

      LOOP
         FETCH get_data
         BULK COLLECT INTO v_tbl
         LIMIT 1000;

         FORALL i IN 1 .. v_tbl.COUNT
            UPDATE lw_pointtransaction pt
               SET pt.pointawarddate = TO_DATE ('01/01/2015', 'mm/dd/yyyy')
             WHERE pt.pointtransactionid = v_tbl (i).pointtransactionid;

         COMMIT;
         EXIT WHEN get_data%NOTFOUND;
      END LOOP;

      COMMIT;

      IF get_data%ISOPEN
      THEN
         CLOSE get_data;
      END IF;
   END;
  END RevertDollarRwdfullfilldate;

  PROCEDURE ExpireEmployees(p_Dummy      Varchar2,
                            retval       in out Rcursor) AS
  begin
    declare
      cursor c is
        select pt.pointtransactionid
          from bp_ae.lw_pointtransaction pt
          join bp_ae.lw_virtualcard vc
            on vc.vckey = pt.vckey
          join bp_ae.ats_memberdetails md
            on md.a_ipcode = vc.ipcode
         where 1 = 1
           and md.a_extendedplaycode in (1, 3)
           and md.a_employeecode = 1
           and pt.expirationdate > SYSDATE;
      TYPE c_type is TABLE OF c%ROWTYPE;
      rec c_type;
    begin
      open c;
      loop
        fetch c bulk collect
          into rec limit 1000;
        for i in 1 .. rec.COUNT loop
          update bp_ae.lw_pointtransaction p
             set p.expirationdate = SYSDATE
           where 1 = 1
             and p.pointtransactionid = rec(i).pointtransactionid;
        end loop;
        EXIT WHEN c%NOTFOUND;
      end loop;
      commit;
      IF c%ISOPEN THEN
        CLOSE c;
      END IF;
      commit;
    end;

  end ExpireEmployees;

  --Changes for AEO AEO-1303 Begin
  PROCEDURE ChangeExternalTable(p_ExtTableName IN VARCHAR2,
                              p_FileName     IN VARCHAR2)
	IS
	  e_MTable exception;
	  e_MFileName exception;
	  v_sql VARCHAR2(400);
	BEGIN

		IF LENGTH(TRIM(p_ExtTableName))=0 OR p_ExtTableName is NULL THEN
		  raise_application_error(-20000, 'External tablename is required', FALSE);
		ELSIF LENGTH(TRIM(p_FileName))=0 OR p_FileName is NULL THEN
		  raise_application_error(-20001, 'Filename is required to link with external table', FALSE);
		END IF;

		v_sql := 'ALTER TABLE '||p_ExtTableName||' LOCATION (AE_IN'||CHR(58)||''''||p_FileName||''')';
		EXECUTE IMMEDIATE v_sql;

	END ChangeExternalTable;
	
	PROCEDURE ResetAITupdateflag(filename VARCHAR2,
                                  Retval  IN OUT Rcursor) AS
          v_Logsource        VARCHAR2(256) := 'OneTimeScripts.ResetAITUpdateFlag';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := filename;
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Messagespassed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'DAPOntimeResetAITFlag';
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
          -- Populate External Table
          ---------------------------
		  ChangeExternalTable(p_ExtTableName => 'EXT_ResetAITFlag',p_FileName => filename);
          ---------------------------
          -- Update AITUpdate flag
          ---------------------------
          DECLARE
               CURSOR Get_Data IS
                    SELECT loyaltynumber
                    FROM   BP_AE.EXT_ResetAITFlag;

               TYPE t_Tab IS TABLE OF Get_Data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
               v_Tbl t_Tab; ---<------ our arry object
          BEGIN
               OPEN Get_Data;
               LOOP
                    FETCH Get_Data BULK COLLECT
                         INTO v_Tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
                    FORALL i IN 1 .. v_Tbl.Count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
                         UPDATE BP_AE.Ats_Memberdetails Mds
                         SET    Mds.a_Aitupdate = 1
								,Mds.updatedate = sysdate
								,Mds.a_changedby = 'ResetAITUpdateFlag'
                         WHERE  Mds.a_Ipcode = (SELECT VC.ipcode
												FROM BP_AE.lw_virtualcard VC 
												WHERE VC.loyaltyidnumber = v_Tbl(i).loyaltynumber);
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
                                   Chr(10) || '    <pkg>OneTimeScripts</pkg>' ||
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
	
  --Changes for AEO AEO-1303 End

END OneTimeScripts;
/
