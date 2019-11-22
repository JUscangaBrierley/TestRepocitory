--LOD reporting table-index DDL.sql


create table LW_RPT_TOP_VISITS
(
  visit_count     NUMBER,
  visit_rank      NUMBER,
  loyaltyidnumber NVARCHAR2(255) not null,
  firstname       NVARCHAR2(50),
  lastname        NVARCHAR2(50),
  last_visit_date TIMESTAMP(4),
  create_date     DATE
);
/

CREATE OR REPLACE VIEW v_lw_rpt_top_visits AS SELECT * FROM LW_RPT_TOP_VISITS;
/

GRANT SELECT ON v_lw_rpt_top_visits TO cognos_r;
/


-- Create table
create table LW_RPT_TOP_RWD_VISIT_SUMMARY
(
  ipcode      NUMBER(18) not null,
  visit_count NUMBER,
  store_count NUMBER,
  action      NVARCHAR2(25) not null
);
/

create or replace view v_LW_RPT_TOP_RWD_VISIT_SUMMARY as select * from LW_RPT_TOP_RWD_VISIT_SUMMARY;
/
grant select on v_LW_RPT_TOP_RWD_VISIT_SUMMARY to cognos_r;
/


-- Create table
create table LW_RPT_ENROLLMENT_SUMMARY
(
  member_create_count NUMBER,
  member_create_date  DATE,
  create_date         DATE default SYSDATE,
  update_date         DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWRPT_ENROLL_SUMM_UIDX on LW_RPT_ENROLLMENT_SUMMARY (MEMBER_CREATE_DATE);
/


-- Create table
create table LW_RPT_TXN_SUMMARY
(
  txn_count        NUMBER,
  ttl_qp           NUMBER,
  ttl_txn_amount   NUMBER,
  ttl_txn_discount NUMBER,
  ttl_quantity     NUMBER,
  storeid          NUMBER,
  txn_date         DATE,
  create_date      DATE default SYSDATE,
  update_date      DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWPRT_TXNSUMMARY_UIDX on LW_RPT_TXN_SUMMARY (TXN_DATE, STOREID);
/


-- Create table
create table LW_RPT_POINTS_SUMMARY
(
  ttl_points_earned          NUMBER,
  ttl_points_consumed_inline NUMBER,
  pts_outstanding            NUMBER,
  ttl_points_consumed_debits NUMBER,
  transactiontype            NUMBER,
  points_earn_date           DATE,
  points_consumed_date       DATE,
  points_expiration_date     DATE,
  create_date                DATE default SYSDATE,
  update_date                DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWRPT_PTSSUMMARY_UIDX on LW_RPT_POINTS_SUMMARY (POINTS_EARN_DATE, POINTS_CONSUMED_DATE, POINTS_EXPIRATION_DATE, TRANSACTIONTYPE);
/


-- Create table
create table LW_RPT_REWARDS_SUMMARY
(
  reward_count           NUMBER,
  rewarddefid            NUMBER(20) not null,
  reward_issue_date      DATE,
  reward_expiration_date DATE,
  reward_redemption_date DATE,
  create_date            DATE default SYSDATE,
  update_date            DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWPRT_REWARDSUMM_UIDX on LW_RPT_REWARDS_SUMMARY (REWARD_ISSUE_DATE, REWARD_EXPIRATION_DATE, REWARD_REDEMPTION_DATE, REWARDDEFID);
/


-- Create table
create table LW_RPT_MOBILE_EVENT_SUMMARY
(
  member_count      NUMBER,
  storeid           NUMBER(20),
  mobile_event_date DATE,
  action            nvarchar2(25),
  create_date       DATE default SYSDATE,
  update_date       DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWRPT_MOBILEEVENTSUMM_UIDX on LW_RPT_MOBILE_EVENT_SUMMARY (STOREID, ACTION, MOBILE_EVENT_DATE);
/


-- Create table
create table LW_RPT_TOP_POINTS_EARNERS
(
  ttl_points      NUMBER,
  ipcode          NUMBER(20) not null,
  loyaltyidnumber NVARCHAR2(255) not null,
  earning_rank    NUMBER,
  create_date     DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWRPT_TOPPTS_EARN_UIDX on LW_RPT_TOP_POINTS_EARNERS (ipcode);
/

-- Create table
create table LW_RPT_TOP_POINTS_TXN_SUMMARY
(
  ipcode              NUMBER(20) not null,
  txn_count           NUMBER,
  ttl_discount_amount NUMBER,
  ttl_qp              NUMBER,
  ttl_txn_amount      NUMBER,
  ttl_txn_discount    NUMBER,
  last_txn_date       TIMESTAMP(4),
  txn_type            NUMBER(10) not null,
  create_date         DATE default SYSDATE
);
/
-- no indexes necessary for top points txn summary


-- Create table
create table LW_RPT_TOP_REWARD_REDEEMERS
(
  rewards_redeemed_count NUMBER,
  ipcode                 NUMBER(20) not null,
  loyaltyidnumber        NVARCHAR2(255) not null,
  rewards_redeemed_rank  NUMBER,
  create_date            DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWRPT_TOPRWD_REDEEM_UIDX on LW_RPT_TOP_REWARD_REDEEMERS (ipcode);
/

-- Create table
create table LW_RPT_TOP_RWD_PTS_SUMMARY
(
  ipcode              NUMBER(20),
  ttl_points_consumed NUMBER,
  rewards_issued      NUMBER,
  create_date         DATE default SYSDATE
);
/
-- no indexes necessary for top reward pts summary

-- Create table
create table LW_RPT_TOP_RWD_TXN_SUMMARY
(
  ipcode              NUMBER(20) not null,
  txn_count           NUMBER,
  store_count         NUMBER,
  ttl_discount_amount NUMBER,
  ttl_qp              NUMBER,
  ttl_txn_amount      NUMBER,
  ttl_txn_discount    NUMBER,
  last_txn_date       TIMESTAMP(4),
  txn_type            NUMBER(10) not null,
  create_date         DATE default SYSDATE
);
/
-- no indexes necessary for top reward txn summary


-- Create table
create table LW_RPT_CSADJUSTMENTS
(
  csagent_id         NUMBER(20) not null,
  pointtransactionid NUMBER(20) not null,
  pointawarddate     TIMESTAMP(4) not null,
  loyaltyidnumber    NVARCHAR2(255) not null,
  points             FLOAT not null,
  pointtypeid        NUMBER(20) not null,
  create_date        DATE default SYSDATE
);
/
-- Create/Recreate indexes 
create unique index LWRPT_CSADJ_UIDX on LW_RPT_CSADJUSTMENTS (CSAGENT_ID, POINTTRANSACTIONID);
/

--LOD reporting views.sql
CREATE OR REPLACE VIEW V_LW_RPT_ENROLLMENT_SUMMARY AS SELECT * FROM lw_rpt_enrollment_summary;
/

GRANT SELECT ON V_LW_RPT_ENROLLMENT_SUMMARY TO cognos_r;
/

CREATE OR REPLACE VIEW V_LW_RPT_TXN_SUMMARY AS SELECT * FROM lw_rpt_txn_summary;
/
GRANT SELECT ON V_LW_RPT_TXN_SUMMARY TO cognos_r;
/


CREATE OR REPLACE VIEW V_LW_RPT_POINTS_SUMMARY AS SELECT * FROM lw_rpt_points_summary;
/
GRANT SELECT ON V_LW_RPT_POINTS_SUMMARY TO cognos_r;
/


CREATE OR REPLACE VIEW V_LW_RPT_REWARDS_SUMMARY AS SELECT * FROM lw_rpt_rewards_summary;
/
GRANT SELECT ON V_LW_RPT_REWARDS_SUMMARY TO cognos_r;
/


CREATE OR REPLACE VIEW V_LW_RPT_MOBILE_EVENT_SUMMARY AS SELECT * FROM lw_rpt_mobile_event_summary;
/
GRANT SELECT ON V_LW_RPT_MOBILE_EVENT_SUMMARY TO cognos_r;
/


CREATE OR REPLACE VIEW V_LW_RPT_TOP_POINTS_EARNERS AS SELECT * FROM lw_rpt_top_points_earners;
/
GRANT SELECT ON V_LW_RPT_TOP_POINTS_EARNERS TO cognos_r;
/

CREATE OR REPLACE VIEW V_LW_RPT_TOP_PTS_TXN_SUMMARY AS SELECT * FROM lw_rpt_top_points_txn_summary;
/
GRANT SELECT ON V_LW_RPT_TOP_PTS_TXN_SUMMARY TO cognos_r;
/

CREATE OR REPLACE VIEW V_LW_RPT_TOP_REWARD_REDEEMERS AS SELECT * FROM lw_rpt_top_reward_redeemers;
/
GRANT SELECT ON V_LW_RPT_TOP_REWARD_REDEEMERS TO cognos_r;
/

CREATE OR REPLACE VIEW V_LW_RPT_TOP_RWD_PTS_SUMMARY AS SELECT * FROM lw_rpt_top_rwd_pts_summary;
/
GRANT SELECT ON V_LW_RPT_TOP_RWD_PTS_SUMMARY TO cognos_r;
/

CREATE OR REPLACE VIEW V_LW_RPT_TOP_RWD_TXN_SUMMARY AS SELECT * FROM lw_rpt_top_rwd_txn_summary;
/
GRANT SELECT ON V_LW_RPT_TOP_RWD_TXN_SUMMARY TO cognos_r;
/


CREATE OR REPLACE VIEW V_LW_RPT_CSADJUSTMENTS AS SELECT * FROM LW_RPT_CSADJUSTMENTS;
/
GRANT SELECT ON V_LW_RPT_CSADJUSTMENTS TO cognos_r;
/

GRANT SELECT ON LW_STOREDEF TO cognos_r;
/
GRANT SELECT ON LW_REWARDSDEF TO cognos_r;
/
GRANT SELECT ON LW_CSAgent TO cognos_r;
/
GRANT SELECT ON LW_PointType TO cognos_r;
/


--RPT_POPULATE_LOD.pck
create or replace package RPT_POPULATE_LOD is

  -- Author  : CNELSON
  -- Created : 27-Jun-2016 3:46:13 PM
  -- Purpose : To populate LOD reporting tables

-- public variables
gv_enrollment_month_offset  NUMBER := 6;
gv_txn_summary_month_offset NUMBER := 1;
gv_pts_summary_month_offset NUMBER := 12;
gv_reward_earn_month_offset NUMBER := 24;
gv_top_visits_day_offset    NUMBER := 90;
gv_lp_top_earner_day_offset NUMBER := 90;
gv_lp_top_reward_day_offset NUMBER := 90;
gv_lp_csradj_day_offset     NUMBER := 90;

PROCEDURE build_mbr_enrollment_summary;
PROCEDURE build_mbr_sales_summary;
PROCEDURE build_points_summary;
PROCEDURE build_rewards_summary;
PROCEDURE build_mobile_event_summary;
PROCEDURE build_top_visits;

PROCEDURE build_LP_top_points_earners;
PROCEDURE build_LP_top_reward_redeemers;
PROCEDURE build_LP_top_rwrd_redeemers_v;
PROCEDURE build_LP_CSR_adjustments;

end RPT_POPULATE_LOD; ;
/
create or replace package body RPT_POPULATE_LOD is


PROCEDURE log_rpt_job_start(p_job_id    OUT NUMBER
                          , p_jobnumber OUT NUMBER
                          , p_job_name  IN  lw_libjob.jobname%TYPE
                            ) IS

    PRAGMA AUTONOMOUS_TRANSACTION;

BEGIN
    UPDATE lw_idgenerator i
    SET i.prevvalue = i.prevvalue + i.incrvalue
    WHERE i.objectname = 'LIBJob'
    RETURNING i.prevvalue INTO p_jobnumber;

    INSERT INTO lw_libjob
      (id, jobnumber, jobtype, jobname, jobdirection, messagesreceived, messagesfailed, starttime, jobstatus, createdate, updatedate)
    VALUES
      (hibernate_sequence.nextval
     , p_jobnumber
     , 'LOD Reporting'
     , upper(p_job_name)
     , 0
     , 0
     , 0
     , CURRENT_TIMESTAMP
     , 1
     , CURRENT_TIMESTAMP
     , CURRENT_TIMESTAMP)
    RETURNING id INTO p_job_id;

    COMMIT;

END log_rpt_job_start;

PROCEDURE log_rpt_job_end(p_job_id     IN NUMBER
                        , p_job_status IN NUMBER DEFAULT 1) IS

    PRAGMA AUTONOMOUS_TRANSACTION;

BEGIN
    UPDATE lw_libjob j
    SET j.endtime    = CURRENT_TIMESTAMP
      , j.updatedate = CURRENT_TIMESTAMP
      , j.jobstatus  = p_job_status
    WHERE j.id = p_job_id;

    COMMIT;

END log_rpt_job_end;

PROCEDURE log_error(p_job_id     IN NUMBER
                  , p_jobnumber  IN NUMBER
                  --, p_sql_errnum IN NUMBER
                  , p_sql_errmsg IN VARCHAR2
                    ) IS

    PRAGMA AUTONOMOUS_TRANSACTION;

BEGIN
    INSERT INTO lw_libmessagelog
      (id, envkey, logsource, jobnumber, message, reason, error, trycount, msgtime)
    VALUES
      (p_job_id
     , 'LOD Reporting'
     , ' '  -- taken from other client examples?
     , p_jobnumber
     , p_sql_errmsg
     , p_sql_errmsg
     , p_sql_errmsg
     , 1
     , current_timestamp);

    COMMIT;

END log_error;

PROCEDURE build_mbr_enrollment_summary IS
    -- operation runs in approx 60 seconds for 2M member
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_MBR_ENROLLMENT_SUMMARY';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Summarizes member creation by member create date

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    MERGE INTO lw_rpt_enrollment_summary d
    USING (
            SELECT COUNT(*)                   AS member_create_count
                 , TRUNC(lm.membercreatedate) AS member_create_date
            FROM lw_loyaltymember lm
            WHERE 1 = 1
              AND lm.membercreatedate >= last_day(add_months(SYSDATE, - gv_enrollment_month_offset)) + 1
              AND 1 = 1
            GROUP BY TRUNC(lm.membercreatedate)
           ) s
    ON (s.member_create_date = d.member_create_date)
    WHEN MATCHED THEN UPDATE
      SET d.member_create_count = s.member_create_count
        , d.update_date         = SYSDATE
    WHEN NOT MATCHED THEN INSERT
      (member_create_count
     , member_create_date
     , create_date
     , update_date)
     VALUES (s.member_create_count
           , s.member_create_date
           , SYSDATE
           , SYSDATE);

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_mbr_enrollment_summary;

PROCEDURE build_mbr_sales_summary IS
 -- builds in beteen 200 - 800 seconds
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_MBR_SALES_SUMMARY';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Summarizes transaction detail activity at the store + date grain, for all txn headers in the prior n months.
    -- Assumes all transaction records should be included (purchases, returns, non-qualifying transactions (e.g. fuel, gift card), etc)
    -- Assumes that qualifying spend (QP) is handled prior to this summarization.

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    MERGE INTO lw_rpt_txn_summary d
    USING (
            SELECT COUNT(*)                      AS txn_count
                 , TRUNC(hdr.a_txndate)          AS txn_date
                 , hdr.a_txnstoreid             AS storeID  -- this is typically mapped in as storeID, but is storedefID in some places
                 , SUM(hdr.a_txnqualpurchaseamt) AS ttl_QP
                 , SUM(hdr.a_txnamount)          AS ttl_txn_amount
                 , SUM(hdr.a_txndiscountamount)  AS ttl_txn_discount
                 , sum(dtl.ttl_quantity)         AS ttl_quantity
                 --, SUM(dtl.ttl_retail_amnt_via_details) AS ttl_retail_amnt_via_details  -- will only confuse people
            FROM ats_txnheader hdr
               , (SELECT td.a_parentrowkey
                       , SUM(td.a_dtlquantity)            AS ttl_quantity
                       , SUM(td.a_dtlretailamount)        AS ttl_retail_amnt_via_details
                  FROM ats_txndetailitem td
                  WHERE 1 = 1
                    AND td.a_txndate >= last_day(add_months(SYSDATE, - gv_txn_summary_month_offset)) + 1
                    AND 1 = 1
                  GROUP BY td.a_parentrowkey) dtl
            WHERE 1 = 1
              AND hdr.a_rowkey = dtl.a_parentrowkey
              AND 1 = 1
            GROUP BY TRUNC(hdr.a_txndate)
                   , hdr.a_txnstoreid
          ) s
    ON (s.txn_date = d.txn_date
    AND s.storeID  = d.storeID)
    WHEN MATCHED THEN UPDATE
      SET d.txn_count        = s.txn_count
        , d.ttl_QP           = s.ttl_QP
        , d.ttl_txn_amount = s.ttl_txn_amount
        , d.ttl_txn_discount = s.ttl_txn_discount
        , d.ttl_quantity     = s.ttl_quantity
        , d.update_date      = SYSDATE
    WHEN NOT MATCHED THEN INSERT
      (txn_count
     , txn_date
     , storeID
     , ttl_QP
     , ttl_txn_amount
     , ttl_txn_discount
     , ttl_quantity
     , create_date
     , update_date)
    VALUES (s.txn_count
          , s.txn_date
          , s.storeID
          , s.ttl_QP
          , s.ttl_txn_amount
          , s.ttl_txn_discount
          , s.ttl_quantity
          , SYSDATE
          , SYSDATE);

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_mbr_sales_summary;

PROCEDURE build_points_summary IS
  -- builds in 110 seconds
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_POINTS_SUMMARY';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Summarizes point transactions at the txn date + exp date + create date + transaction type grain
    -- This grain allows for independent summarization of earns, consumptions, and expirations on a day-by-day basis
    -- "Inline Consumption" is based on the points consumed field, as opposed to using the transaction type = 4 records (aka debits)
    -- Assumes the create date on the consumption record is the consumption date.
    -- Assumes the transaction date is the point earn date.

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    MERGE INTO lw_rpt_points_summary d
    USING (
            SELECT SUM(CASE WHEN pt.transactiontype IN (1, 2) THEN pt.points ELSE 0 END)                       AS ttl_points_earned
                 , SUM(CASE WHEN pt.transactiontype IN (1, 2) THEN pt.pointsconsumed ELSE 0 END)               AS ttl_points_consumed_inline
                 , SUM(CASE WHEN pt.transactiontype IN (1, 2) THEN (pt.points - pt.pointsconsumed) ELSE 0 END) AS pts_outstanding
                 , sum(CASE WHEN pt.transactiontype = 4 THEN pt.points ELSE 0 END)                             AS ttl_points_consumed_debits
                 , pt.transactiontype
                 , TRUNC(pt.transactiondate) AS points_earn_date
                 , TRUNC(pt.createdate)      AS points_consumed_date  -- there is no stated "consumption date" in LW, have to use create date on consumption
                 , TRUNC(pt.expirationdate)  AS points_expiration_date
            FROM lw_pointtransaction pt
            WHERE 1 = 1
              AND pt.transactiondate >= last_day(add_months(SYSDATE, - gv_pts_summary_month_offset)) + 1
              AND 1 = 1
            GROUP BY TRUNC(pt.transactiondate)
                   , TRUNC(pt.expirationdate)
                   , TRUNC(pt.createdate)
                   , pt.transactiontype
          ) s
    ON (s.points_earn_date       = d.points_earn_date       -- will nulls be a problem here?
    AND s.points_consumed_date   = d.points_consumed_date   -- will nulls be a problem here?
    AND s.points_expiration_date = d.points_expiration_date -- will nulls be a problem here?
    AND s.transactiontype        = d.transactiontype)
    WHEN MATCHED THEN UPDATE
      SET d.ttl_points_earned          = s.ttl_points_earned
        , d.ttl_points_consumed_inline = s.ttl_points_consumed_inline
        , d.pts_outstanding            = s.pts_outstanding
        , d.ttl_points_consumed_debits = s.ttl_points_consumed_debits
    WHEN NOT MATCHED THEN INSERT
      (ttl_points_earned
     , ttl_points_consumed_inline
     , pts_outstanding
     , ttl_points_consumed_debits
     , transactiontype
     , points_earn_date
     , points_consumed_date
     , points_expiration_date
     , create_date
     , update_date)
    VALUES (s.ttl_points_earned
          , s.ttl_points_consumed_inline
          , s.pts_outstanding
          , s.ttl_points_consumed_debits
          , s.transactiontype
          , s.points_earn_date
          , s.points_consumed_date
          , s.points_expiration_date
          , SYSDATE
          , SYSDATE);

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_points_summary;



PROCEDURE build_rewards_summary IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_REWARDS_SUMMARY';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Summarizes reward issuance by issue date + redemption date + expiration date + reward def ID
    -- Where the reward was issued within the last n months
    -- The existing summarizations for rewards earned in the last n months are first deleted,
    --    to avoid merge complications on rewards that were redeemed since the last summarization run.

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    DELETE FROM lw_rpt_rewards_summary s
    WHERE s.reward_issue_date >= last_day(add_months( SYSDATE, - gv_reward_earn_month_offset)) + 1;

    MERGE INTO lw_rpt_rewards_summary d
    USING (
            SELECT COUNT(*)                   AS reward_count
                 , r.rewarddefid
                 , TRUNC(r.dateissued)        AS reward_issue_date
                 , TRUNC(r.expiration)        AS reward_expiration_date
                 , TRUNC(r.redemptiondate)    AS reward_redemption_date
            FROM lw_memberrewards r
            WHERE 1 = 1
              AND r.dateissued >= last_day(add_months( SYSDATE, - gv_reward_earn_month_offset)) + 1
              AND 1 = 1
            GROUP BY TRUNC(r.redemptiondate)
                   , TRUNC(r.dateissued)
                   , TRUNC(r.expiration)
                   , r.rewarddefid
           ) s
    ON (nvl(s.reward_issue_date, to_date('31-dec-2099', 'dd-mon-yyyy'))      = nvl(d.reward_issue_date, to_date('31-dec-2099', 'dd-mon-yyyy'))  -- set to arbitrary date to ensure nulls don't get lost in the merge
    AND nvl(s.reward_expiration_date, to_date('31-dec-2099', 'dd-mon-yyyy')) = nvl(d.reward_expiration_date, to_date('31-dec-2099', 'dd-mon-yyyy'))
    AND nvl(s.reward_redemption_date, to_date('31-dec-2099', 'dd-mon-yyyy')) = nvl(d.reward_redemption_date, to_date('31-dec-2099', 'dd-mon-yyyy'))  --  What happens if a redemption date was null, but then isn't?  Creates a new row, right?  That's why we delete first.
    AND s.rewarddefid            = d.rewarddefid)
    WHEN MATCHED THEN UPDATE
      SET d.reward_count = d.reward_count
        , d.update_date  = SYSDATE
    WHEN NOT MATCHED THEN INSERT
      (reward_count
     , rewarddefid
     , reward_issue_date
     , reward_expiration_date
     , reward_redemption_date
     , create_date
     , update_date)
    VALUES (s.reward_count
          , s.rewarddefid
          , s.reward_issue_date
          , s.reward_expiration_date
          , s.reward_redemption_date
          , SYSDATE
          , SYSDATE);

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_rewards_summary;


PROCEDURE build_mobile_event_summary IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_MOBILE_EVENT_SUMMARY';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Summarizes mobile events at the store + action + create date grain
    -- Mobile events are note currently in use by anyone, so this hasn't been tested enough.

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    MERGE INTO lw_rpt_mobile_event_summary d
    USING (
            SELECT COUNT(*) AS member_count
                 , e.storedefid AS storeID -- this is to keep the name consistent across several uses
                 , e.action  -- defined by an enumeration in LW
                 , TRUNC(e.createdate) AS mobile_event_date
            FROM lw_membermobileevent e
            WHERE 1 = 1
              -- currently this table only stores check-ins, if other events are introduced this may need to be updated
              AND 1 = 1
            GROUP BY e.storedefid
                   , e.action
                   , TRUNC(e.createdate)
           ) s
    ON (s.storeID = d.storeID
    AND s.action  = d.action
    AND s.mobile_event_date = d.mobile_event_date)
    WHEN MATCHED THEN UPDATE
      SET d.member_count = s.member_count
        , d.update_date  = SYSDATE
    WHEN NOT MATCHED THEN INSERT
      (member_count
     , storeID
     , mobile_event_date
     , action
     , create_date
     , update_date)
    VALUES (s.member_count
          , s.storeID
          , s.mobile_event_date
          , s.action
          , SYSDATE
          , SYSDATE);

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_mobile_event_summary;



PROCEDURE build_top_visits IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_MOBILE_EVENT_SUMMARY';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Ranks members by how many visits they've had in the time period

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    DELETE FROM lw_rpt_top_visits;
    
    INSERT INTO lw_rpt_top_visits
        (visit_count
       , visit_rank
       , loyaltyidnumber
       , firstname
       , lastname
       , last_visit_date
       , create_date)
        SELECT x.visit_count
             , x.visit_rank
             , vc.loyaltyidnumber
             , x.firstname
             , x.lastname
             , x.last_visit_date
             , SYSDATE
        FROM (
              SELECT COUNT(*)     AS visit_count
                   , mme.memberid AS ipcode
                   , lm.firstname
                   , lm.lastname
                   , MAX(mme.createdate) AS last_visit_date
                   , RANK() OVER (ORDER BY COUNT(*) DESC) AS visit_rank 
              FROM lw_membermobileevent mme
                 , lw_loyaltymember lm
              WHERE 1 = 1
                AND mme.memberid = lm.ipcode
                AND mme.createdate > SYSDATE - gv_top_visits_day_offset
                AND 1 = 1
              GROUP BY mme.memberid
                     , lm.firstname
                     , lm.lastname) x
            , lw_virtualcard vc
        WHERE 1 = 1
          AND x.ipcode = vc.ipcode
          AND vc.isprimary = 1
          AND visit_rank <= 1000
          AND 1 = 1;

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_top_visits;


PROCEDURE build_LP_top_points_earners IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_LP_TOP_POINTS_EARNERS';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Two-step process:
    --    1)  identify the top 1,000 points earners over the last n days  (not used in the final report)
    --    2)  use that population to summarize their transaction activity (used in the final report)
    -- The deletes/inserts are done as a single transaction to ensure that the reporting tables are always populated, even during the refresh.
    -- DENSE RANK was specified by the reporting team.  I believe RANK is the better choice here.  //cnelson 09/28/2016

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    DELETE FROM lw_rpt_top_points_earners;

    INSERT INTO lw_rpt_top_points_earners
        (ttl_points
       , ipcode
       , loyaltyidnumber
       , earning_rank)
        SELECT t.ttl_points
             , t.ipcode
             , vc.loyaltyidnumber
             , t.earning_rank
        FROM (SELECT COUNT(*) AS point_transactions
                   , SUM(pt.points) AS ttl_points
                   , vc.ipcode
                   , DENSE_RANK() OVER (ORDER BY SUM(pt.points) DESC) AS earning_rank
              FROM lw_pointtransaction pt
                 , lw_virtualcard vc
              WHERE 1 = 1
                AND pt.vckey = vc.vckey
                AND pt.transactiondate > SYSDATE - gv_lp_top_earner_day_offset
                AND pt.transactiontype IN (1, 2) -- earn records only
                AND 1 = 1
              GROUP BY vc.ipcode
              ) t
          , lw_virtualcard vc
        WHERE 1 = 1
          AND t.ipcode = vc.ipcode
          AND vc.isprimary = 1
          AND t.earning_rank <= 1000
          AND 1 = 1;


    DELETE FROM lw_rpt_top_points_txn_summary;

    INSERT INTO lw_rpt_top_points_txn_summary
        (ipcode
       , txn_count
       , ttl_discount_amount
       , ttl_QP
       , ttl_txn_amount
       , ttl_txn_discount
       , last_txn_date
       , txn_type)
        SELECT vc.ipcode
             , COUNT(*)                               AS txn_count
             , SUM(hdr.a_txndiscountamount)           AS ttl_discount_amount
             , SUM(hdr.a_txnqualpurchaseamt)          AS ttl_QP
             , SUM(hdr.a_txnamount)                   AS ttl_txn_amount
             , SUM(hdr.a_txndiscountamount)           AS ttl_txn_discount
             , MAX(hdr.a_txndate)                     AS last_txn_date
             , a_txntypeid                            AS txn_type
        FROM ats_txnheader hdr
           , lw_virtualcard vc
        WHERE 1 = 1
          AND hdr.a_txntypeid IN (1, 2) -- assumed to be 1 = purchase, 2 = return....the business doesn't always adhere to this.
          AND hdr.a_vckey = vc.vckey
          AND hdr.a_txndate > SYSDATE - gv_lp_top_earner_day_offset
          AND EXISTS (SELECT NULL
                      FROM lw_rpt_top_points_earners p
                      WHERE 1 = 1
                        AND p.ipcode = vc.ipcode
                        AND 1 = 1)
        GROUP BY vc.ipcode
               , hdr.a_txntypeid;

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_LP_top_points_earners;


PROCEDURE build_LP_top_reward_redeemers IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_LP_TOP_REWARD_REDEEMERS';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Three-step process:
    --    1)  identify top 10,000 reward redeemers to (hopefully) ensure we end up with 100 after all the ties in ranks.
    --    2)  use the result from (1) to identify the points earned/consumed (used in the final report)
    --    3)  use the result from (1) to summarize transaction activity (used in the final report)
    -- The deletes/inserts are done as a single transaction to ensure that the reporting tables are always populated, even during the refresh.

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    DELETE FROM lw_rpt_top_reward_redeemers;

    INSERT INTO lw_rpt_top_reward_redeemers
        (rewards_redeemed_count
       , ipcode
       , loyaltyidnumber
       , rewards_redeemed_rank)
    SELECT t.rewards_redeemed_count
         , t.ipcode
         , vc.loyaltyidnumber
         , t.rewards_redeemed_rank
    FROM (SELECT COUNT(*) AS rewards_redeemed_count
               , mr.memberid AS ipcode
               , RANK() OVER (ORDER BY COUNT(*) DESC) AS rewards_redeemed_rank
          FROM lw_memberrewards mr
          WHERE 1 = 1
            AND mr.redemptiondate > SYSDATE - gv_lp_top_reward_day_offset
            AND 1 = 1
          GROUP BY mr.memberid
          ) t
       , lw_virtualcard vc
    WHERE 1 = 1
      AND t.ipcode = vc.ipcode
      AND vc.isprimary = 1
      AND t.rewards_redeemed_rank <= 10000
      AND 1 = 1;


    DELETE FROM lw_rpt_top_rwd_pts_summary;

    INSERT INTO lw_rpt_top_rwd_pts_summary
        (ipcode
       , ttl_points_consumed
       , rewards_issued)
    SELECT vc.ipcode
         , SUM(pt.points) AS ttl_points_consumed
         , COUNT(DISTINCT pt.rowkey) AS rewards_issued
    FROM lw_pointtransaction pt
       , lw_virtualcard vc
    WHERE 1 = 1
      AND pt.vckey = vc.vckey
      AND pt.transactiontype = 4
      AND pt.createdate > SYSDATE - gv_lp_top_reward_day_offset
      AND EXISTS (SELECT NULL
                  FROM lw_rpt_top_reward_redeemers r
                  WHERE 1 = 1
                    AND r.ipcode = vc.ipcode
                    AND 1 = 1)
      AND 1 = 1
    GROUP BY vc.ipcode;


    DELETE FROM lw_rpt_top_rwd_txn_summary;

    INSERT INTO lw_rpt_top_rwd_txn_summary
        (ipcode
       , txn_count
       , store_count
       , ttl_discount_amount
       , ttl_QP
       , ttl_txn_amount
       , ttl_txn_discount
       , last_txn_date
       , txn_type)
    SELECT vc.ipcode
         , COUNT(*) AS txn_count
         , COUNT(DISTINCT hdr.a_txnstoreid)      AS store_count
         , SUM(hdr.a_txndiscountamount)           AS ttl_discount_amount
         , SUM(hdr.a_txnqualpurchaseamt)          AS ttl_QP
         , SUM(hdr.a_txnamount)                   AS ttl_txn_amount
         , SUM(hdr.a_txndiscountamount)           AS ttl_txn_discount
         , MAX(hdr.a_txndate)                     AS last_txn_date
         , a_txntypeid                            AS txn_type
    FROM ats_txnheader hdr
       , lw_virtualcard vc
    WHERE 1 = 1
      AND hdr.a_txntypeid IN (1, 2) -- assumed to be 1 = purchase, 2 = return....the business doesn't always adhere to this.
      AND hdr.a_vckey = vc.vckey
      AND hdr.a_txndate > SYSDATE - gv_lp_top_reward_day_offset
      AND EXISTS (SELECT NULL
                  FROM lw_rpt_top_reward_redeemers r
                  WHERE 1 = 1
                    AND r.ipcode = vc.ipcode
                    AND 1 = 1)
    GROUP BY vc.ipcode
           , hdr.a_txntypeid;

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_LP_top_reward_redeemers;



PROCEDURE build_LP_top_rwrd_redeemers_v IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_LP_TOP_RWRD_REDEEMERS_V';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Quasi-two-step process:
    --    1)  Use the first 2 intermediate results from the build_top_reward_redeemers step
    --    2)  use the result from (1) to summarize visit activity (used in the final report)
    -- The deletes/inserts are done as a single transaction to ensure that the reporting tables are always populated, even during the refresh.
    -- build_LP_top_reward_redeemers MUST complete before this step can run.
    
    -- Why not just bolt the visists portion onto the other redeemers process?  
    --  Because it involves an inner join on transactions, and this will use an 
    --  inner join on visits....changing these to outer joins & combining them 
    --  adds an unecessary complication.
    
BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    DELETE FROM lw_rpt_top_rwd_visit_summary;

    INSERT INTO lw_rpt_top_rwd_visit_summary
        (ipcode
       , visit_count
       , store_count
       , action)
    SELECT mme.memberid AS ipcode
         , COUNT(*) AS visit_count
         , COUNT(DISTINCT mme.storedefid)      AS store_count
         , mme.action
    FROM lw_membermobileevent mme
    WHERE 1 = 1
      AND mme.createdate > SYSDATE - gv_lp_top_reward_day_offset
      AND mme.action = 'CheckIn' -- currently only required to report on Check Ins.  Included in the select/group by to make expansion easier later
      AND EXISTS (SELECT NULL
                  FROM lw_rpt_top_reward_redeemers r
                  WHERE 1 = 1
                    AND r.ipcode = mme.memberid
                    AND 1 = 1)
    GROUP BY mme.memberid
           , mme.action;

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_LP_top_rwrd_redeemers_v;



PROCEDURE build_LP_CSR_adjustments IS
    c_job_name      CONSTANT lw_libjob.jobname%TYPE := 'BUILD_LP_CSR_ADJUSTMENTS';
    lv_job_id                lw_libjob.id%TYPE;
    lv_job_number            lw_libjob.jobnumber%TYPE;
    lv_errmsg                VARCHAR2(4000);

    -- Isolates all points issued by a CS Agent (based on ptchangedby being a user in the lw_csagent table).

BEGIN
    log_rpt_job_start(p_job_id    => lv_job_id
                    , p_jobnumber => lv_job_number
                    , p_job_name  => c_job_name);

    -- there should be no updates to pointtransaction records
    -- so we'll only insert new records
    INSERT INTO lw_rpt_csadjustments
       (csagent_id
      , pointtransactionid
      , pointawarddate
      , loyaltyidnumber
      , points
      , pointtypeid)
    SELECT csa.id AS csagent_id
         , pt.pointtransactionid
         , pt.pointawarddate
         , vc.loyaltyidnumber
         , pt.points
         , pt.pointtypeid
    FROM lw_pointtransaction pt
       , lw_virtualcard vc
       , lw_csagent csa
    WHERE 1 = 1
      AND pt.vckey = vc.vckey
      AND pt.createdate > SYSDATE - gv_lp_csradj_day_offset
      AND pt.ptchangedby = csa.username
      AND NOT EXISTS (SELECT NULL
                      FROM lw_rpt_csadjustments x
                      WHERE x.csagent_id         = csa.id
                        AND x.pointtransactionid = pt.pointtransactionid)
      AND 1 = 1;

    COMMIT;

    log_rpt_job_end(p_job_id => lv_job_id);

EXCEPTION
    WHEN OTHERS THEN
        log_rpt_job_end(p_job_id     => lv_job_id
                      , p_job_status => 0);

        lv_errmsg := SQLERRM;

        log_error(p_job_id     => lv_job_id
                , p_jobnumber  => lv_job_number
                , p_sql_errmsg => lv_errmsg);

        RAISE;

END build_LP_CSR_adjustments;



end RPT_POPULATE_LOD; ;
/