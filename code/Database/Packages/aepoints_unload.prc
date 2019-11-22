CREATE OR REPLACE PROCEDURE AEPOINTS_UNLOAD AS
BEGIN
	execute immediate 'truncate table bp_ae.AEpointheader';
	INSERT /*+ APPEND */ INTO bp_ae.AEpointheader
	(
		IPCODE,
		TXNCOUNT,
		RECORDTYPE,
		CUSTOMERKEY,
		LOYALTYIDNUMBER,
		TOTALPOINTS,
		REWARDLEVEL,
		POINTSTONEXTREWARD,
		BASEPOINTS,
		BONUSPOINTS
	)
	SELECT /*+ parallel(pts 8) */ lm.ipcode,
				count(pts.vckey) as TxnCount,
				max('H') AS RecordType,
				max('  ') AS CustomerKey,
				vc.loyaltyidnumber,
				max(bl.a_totalpoints) AS TotalPoints,
				max(bl.a_rewardlevel) AS RewardLevel,
				CASE WHEN max(bl.a_totalpoints)  > 499  THEN  0
				 else max(bl.a_pointstonextreward)
						END AS PointsToNextReward,
				max(bl.a_basepoints) AS BasePoints,
				max(bl.a_bonuspoints) AS BonusPoints
			   FROM ats_memberpointbalances bl
			   INNER JOIN lw_loyaltymember lm ON lm.ipcode = bl.a_ipcode
				 INNER JOIN lw_virtualcard vc on vc.ipcode = bl.a_ipcode AND vc.isprimary=1
				 INNER JOIN lw_pointtransaction pts on vc.vckey = pts.vckey
				 INNER JOIN  lw_pointtype pnt on pnt.pointtypeid = pts.pointtypeid
				 WHERE pnt.name NOT LIKE 'Jean%' and pnt.name NOT LIKE 'Bra%'
				 AND pts.transactiondate >= TRUNC(sysdate, 'Q') and pts.transactiondate < ADD_MONTHS(TRUNC(sysdate, 'Q'), 3)
	GROUP BY lm.ipcode, vc.loyaltyidnumber;
	COMMIT;

	execute immediate 'truncate table bp_ae.ae_memberpointsonetime';
	INSERT /*+ APPEND */ INTO bp_ae.ae_memberpointsonetime
	(
		RECORDTYPE,
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
		REGISTERNUMBER
	)
     SELECT mem.RecordType, -- member header with primary card if any, else any other available card
         mem.CustomerKey,
         mem.LoyaltyIDNumber,
         mem.TotalPoints,
         mem.RewardLevel,
         mem.PointsToNextReward,
         NULL AS Description,
         mem.BasePoints,
         mem.BonusPoints,
         TO_CHAR (SYSDATE - 1, 'MMDDYYYY'),
         NULL AS TXNNumber,
         NULL AS OrderNumber,
         NULL AS StoreNumber,
         NULL AS RegisterNumber
      FROM AEpointheader mem
     WHERE mem.TxnCount > 0
    UNION ALL
    SELECT 'T' AS RecordType,          -- point transactions against any txnheader
         mem.CustomerKey,
         mem.LoyaltyIDNumber,
         NULL AS TotalPoints,
         NULL AS RewardLevel,
         NULL AS PointsToNextReward,
         pt.pointeventid AS Description,
         CASE
          WHEN p.name IN ('Basic Points', 'Adjustment Points')
          THEN
           NVL (pt.points, 0)
         END
          AS BasePoints,
         CASE
          WHEN p.name IN
              ('CS Points', 'Bonus Points', 'Adjustment Bonus Points')
          THEN
           NVL (pt.points, 0)
         END
          AS BonusPoints,
         TO_CHAR (pt.transactiondate, 'MMDDYYYY') AS TXNDate,
         h.a_txnnumber AS TXNNumber,
         h.a_ordernumber AS OrderNumber,
         h.a_storenumber AS StoreNumber,
         h.a_txnregisternumber AS RegisterNumber
      FROM lw_virtualcard vc
         INNER JOIN AEpointheader mem
          ON mem.ipcode = vc.ipcode
         INNER JOIN lw_pointtransaction pt
          ON vc.vckey = pt.vckey
         INNER JOIN lw_pointtype p
          ON pt.pointtypeid = p.pointtypeid
         INNER JOIN ats_txnheader h
          ON pt.rowkey = h.a_rowkey
     WHERE pt.transactiondate >= TRUNC(sysdate, 'Q') and pt.transactiondate < ADD_MONTHS(TRUNC(sysdate, 'Q'), 3)
    UNION all
    SELECT 'T' AS RecordType, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
         mem.CustomerKey,
         mem.LoyaltyIDNumber,
         NULL AS TotalPoints,
         NULL AS RewardLevel,
         NULL AS PointsToNextReward,
         pt.pointeventid AS Description,
         CASE
          WHEN p.name IN ('Basic Points', 'Adjustment Points')
          THEN
           NVL (pt.points, 0)
         END
          AS BasePoints,
         CASE
          WHEN p.name IN
              ('CS Points', 'Bonus Points', 'Adjustment Bonus Points')
          THEN
           NVL (pt.points, 0)
         END
          AS BonusPoints,
         TO_CHAR (pt.transactiondate, 'MMDDYYYY') AS TXNDate,
         NULL AS TXNNumber,
         NULL AS OrderNumber,
         NULL AS StoreNumber,
         NULL AS RegisterNumber
      FROM lw_virtualcard vc
         INNER JOIN AEpointheader mem
          ON mem.ipcode = vc.ipcode
         INNER JOIN lw_pointtransaction pt
          ON vc.vckey = pt.vckey
         INNER JOIN lw_pointtype p
          ON pt.pointtypeid = p.pointtypeid
         INNER JOIN ats_txndetailitem dtl
          ON pt.rowkey = dtl.a_rowkey
     WHERE pt.transactiondate >= TRUNC(sysdate, 'Q') and pt.transactiondate < ADD_MONTHS(TRUNC(sysdate, 'Q'), 3)
         AND pt.pointtypeid IN
            (SELECT pointtypeid
             FROM lw_pointtype
            WHERE name NOT LIKE 'Jean%' AND name NOT LIKE 'Bra%')
    UNION all
    SELECT 'T' AS RecordType, -- txnheader independent point transactions, like Email, SMS bonuses etc
         mem.CustomerKey,
         mem.LoyaltyIDNumber,
         NULL AS TotalPoints,
         NULL AS RewardLevel,
         NULL AS PointsToNextReward,
         pt.pointeventid AS Description,
         CASE
          WHEN p.name IN ('Basic Points', 'Adjustment Points')
          THEN
           NVL (pt.points, 0)
         END
          AS BasePoints,
         CASE
          WHEN p.name IN
              ('CS Points', 'Bonus Points', 'Adjustment Bonus Points')
          THEN
           NVL (pt.points, 0)
         END
          AS BonusPoints,
         TO_CHAR (pt.transactiondate, 'MMDDYYYY') AS TXNDate,
         NULL AS TXNNumber,
         NULL AS OrderNumber,
         NULL AS StoreNumber,
         NULL AS RegisterNumber
      FROM lw_virtualcard vc
         INNER JOIN AEpointheader mem
          ON mem.ipcode = vc.ipcode
         INNER JOIN lw_pointtransaction pt
          ON vc.vckey = pt.vckey
         INNER JOIN lw_pointtype p
          ON pt.pointtypeid = p.pointtypeid
     WHERE pt.transactiondate >= TRUNC(sysdate, 'Q') and pt.transactiondate < ADD_MONTHS(TRUNC(sysdate, 'Q'), 3)
         AND pt.rowkey IN ('-1')
         AND pt.pointtypeid IN
            (SELECT pointtypeid
             FROM lw_pointtype
            WHERE name NOT LIKE 'Jean%' AND name NOT LIKE 'Bra%')
    ORDER BY LoyaltyIDNumber, RecordType;
	COMMIT;
END;
/

