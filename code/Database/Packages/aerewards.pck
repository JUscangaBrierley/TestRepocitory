CREATE OR REPLACE PACKAGE AERewards IS

  TYPE Rcursor IS REF CURSOR;
  PROCEDURE UpdateEmployeeCode(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsSelection(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE UpdateUnMailableAddress(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsPrelimCounts(p_Dummy VARCHAR2, p_IncludeEmployees VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsCreateRewards(p_Dummy VARCHAR2, p_IncludeEmployees NUMBER, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsBackupWkTbl(p_Dummy VARCHAR2, p_ProcessDate Date, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsResetEmpCode(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsPostCounts(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsPostCounts_Emp(p_Dummy VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE QuarterlyRewardsRollback(p_Dummy VARCHAR2, p_ProcessDate Date, Retval IN OUT Rcursor);

  PROCEDURE EarlyRewardsSelection(p_Dummy VARCHAR2, Retval  IN OUT Rcursor);


  PROCEDURE EarlyRewardsBackupWkTbl(p_Dummy VARCHAR2, p_ProcessDate Date,
                                         Retval  IN OUT Rcursor);

  PROCEDURE LoadSample
      (
        p_Reward		       VARCHAR2,
        p_Country	         VARCHAR2,
        p_Country2	       VARCHAR2,
        p_State		         VARCHAR2,
        p_PurchasedAEBrand	NUMBER,
        p_PurchasedAerieBrand NUMBER,
        p_Purchased77KidsBrand NUMBER,
        p_PurchaseBrand VARCHAR2
      );
  PROCEDURE LoadSampleTotalAE
      (
        p_Reward		       VARCHAR2,
        p_Country	         VARCHAR2,
        p_Country2	       VARCHAR2,
        p_State		         VARCHAR2,
        p_PurchaseBrand VARCHAR2
      );
  PROCEDURE LoadSampleTotalAerie
      (
        p_Reward		       VARCHAR2,
        p_Country	         VARCHAR2,
        p_Country2	       VARCHAR2,
        p_State		         VARCHAR2,
        p_PurchaseBrand VARCHAR2
      );
  PROCEDURE LoadSampleTotalKids
      (
        p_Reward		       VARCHAR2,
        p_Country	         VARCHAR2,
        p_Country2	       VARCHAR2,
        p_State		         VARCHAR2,
        p_PurchaseBrand VARCHAR2
      );
  PROCEDURE LoadSampleCombo
      (
        p_Reward		       VARCHAR2,
        p_Country	         VARCHAR2,
        p_Country2	       VARCHAR2,
        p_State		         VARCHAR2,
        p_PurchaseBrand VARCHAR2
      );
PROCEDURE LoadSample_Employee
      (
        p_Reward		       VARCHAR2,
        p_Country	         VARCHAR2,
        p_Country2	       VARCHAR2,
        p_State		         VARCHAR2,
        p_PurchasedAEBrand	NUMBER,
        p_PurchasedAerieBrand NUMBER,
        p_Purchased77KidsBrand NUMBER,
        p_PurchaseBrand VARCHAR2
      );
      -- aeo-2114 BEGIN      

PROCEDURE GetRewardsToIssue;           

Procedure SetLastRewardDate( p_newDateTime varchar2) ;
                
      -- AEO-2114 END
END AERewards;
/
CREATE OR REPLACE PACKAGE BODY AERewards IS

/********************************************************************
       Update the EmployeeCode on MemberDetails based on the
       employeeid from HistoryTxnDetail to make sure we catch any
       employee txns that were missed.
  ********************************************************************/
PROCEDURE UpdateEmployeeCode(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.UpdateEmployeeCode';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.UpdateEmployeeCode';
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
      CURSOR get_data IS
        select distinct hst.a_ipcode as ipcode
          from   ats_historytxndetail hst
          where  hst.a_txnemployeeid is not null
          and    trunc(hst.createdate, 'Q') = Trunc(SYSDATE-10, 'Q')
          and    hst.a_ipcode is not null;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails mds
             SET mds.a_employeecode = 1
           WHERE mds.a_ipcode = v_tbl(i).ipcode
             AND (mds.a_employeecode IS NULL OR mds.a_employeecode = 0 OR mds.a_employeecode = 2);
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
      v_Reason         := 'Failed Procedure UpdateEmployeeCode';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
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

  END UpdateEmployeeCode;

/********************************************************************
       Update the address mailable flag on MemberDetails based on
       some requirements for a bad address
  ********************************************************************/
PROCEDURE UpdateUnMailableAddress(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.UpdateUnMailableAddress';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.UpdateUnMailableAddress';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'UpdateUnMailableAddress';
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
    -- Update AddressMailable flag
    ---------------------------
    DECLARE
      CURSOR get_data IS
        select  md.a_ipcode as ipcode
          from   ats_memberdetails md
          where  (md.a_ziporpostalcode = '00000' and md.a_addressmailable = 1)
          or     (length(md.a_ziporpostalcode) > 10  and md.a_addressmailable = 1)
          or     ((md.a_addresslineone like '%"%' or md.a_addresslinetwo like '%"%' or md.a_city like '%"%' or md.a_stateorprovince like '%"%' or md.a_ziporpostalcode like '%"%' or md.a_country like '%"%') and md.a_addressmailable = 1)
          or     ((not exists(select 1 from ats_refstates st where st.a_statecode = md.a_stateorprovince) and md.a_addressmailable = 1))
          or     (md.a_country not in ('CAN', 'USA') and md.a_addressmailable = 1)
          or     (md.a_country = 'CAN' and md.a_stateorprovince in (select a_statecode from ats_refstates st where st.a_countrycode = 'USA') and md.a_addressmailable = 1);
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_memberdetails mds
             SET mds.a_addressmailable = 0
           WHERE mds.a_ipcode = v_tbl(i).ipcode;
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
      v_Reason         := 'Failed Procedure UpdateUnMailableAddress';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>UpdateUnMailableAddress</proc>' ||
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

  END UpdateUnMailableAddress;

/********************************************************************
       Calculate the preliminary reward counts
  ********************************************************************/
PROCEDURE QuarterlyRewardsPrelimCounts(p_Dummy VARCHAR2, p_IncludeEmployees VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsPrelimCounts';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsPrelimCounts';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsPrelimCounts';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' || Upper(Sys_Context('userenv', 'instance_name'));
    v_RewardString VARCHAR(100) := '15%20%30%40%';
    v_IncludeEmployees VARCHAR2(100);
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

  EXECUTE IMMEDIATE 'Truncate Table AE_RewardCounts';
  EXECUTE IMMEDIATE 'Truncate Table AE_RewardSamples';

  If(p_IncludeEmployees is null OR Length(p_IncludeEmployees) = 0) Then
    v_IncludeEmployees := 'NO';
  Else
    v_IncludeEmployees := Upper(p_IncludeEmployees);
  End If;

  ---------------------------
  -- Get the Preliminary Counts
  ---------------------------
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Preliminary Rewards Counts for: ', to_char(trunc(sysdate, 'Q'), 'mm/dd/yyyy') , '', '' From Dual;
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Include Employees:', v_IncludeEmployees , '', '' From Dual;

  -------------------------------------------------------------------------------------------------------------
  ---Run these 3 queries to load the spreadsheet (<Year> <Quarter> Preliminary Rewards Counts.xls)
  ---The sorting should be correct so you can just copy the Count column into the matching spreadsheet column
  ---for that category (Total Members, Total Mailed, Total Un-Mailed) and Country
  -------------------------------------------------------------------------------------------------------------
  ------------------
  --Total Members
  ------------------
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Total Members------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'USA', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE wk.a_country = 'USA' GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Canada', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'CAN' AND (wk.a_stateorprovince is null or wk.a_stateorprovince <> 'QC') GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Quebec', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE wk.a_stateorprovince = 'QC' GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
  INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Undefined', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  (wk.a_country is null or wk.a_country not in ('CAN', 'USA')) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
  If(v_IncludeEmployees = 'NO') Then
    -------------------------------------------------------------------------------------------------------------
    ---Get the counts based on not including employees in the rewards file(s)
    -------------------------------------------------------------------------------------------------------------
    ------------------
    --Total Mailed
    ------------------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Total Mailed------', '', '', '' From Dual;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'USA', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'USA' AND wk.a_addressmailable > 0  AND wk.a_employeecode in (0,2) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Canada', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'CAN' AND wk.a_stateorprovince <> 'QC' AND wk.a_addressmailable > 0  AND wk.a_employeecode in (0,2) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Quebec', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE wk.a_country = 'CAN' AND wk.a_stateorprovince = 'QC' AND wk.a_addressmailable > 0  AND wk.a_employeecode in (0,2) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Undefined', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  (wk.a_country is null or wk.a_country not in ('CAN', 'USA')) AND wk.a_addressmailable > 0  AND wk.a_employeecode in (0,2) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;

    --------------------
    --Total Un-Mailed
    --------------------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Total Un-Mailed------', '', '', '' From Dual;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'USA', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'USA' AND (wk.a_addressmailable = 0  OR wk.a_employeecode = 1) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Canada', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'CAN' AND (wk.a_stateorprovince is null or wk.a_stateorprovince <> 'QC') AND (wk.a_addressmailable = 0  OR wk.a_employeecode = 1) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Quebec', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE wk.a_country = 'CAN' AND wk.a_stateorprovince = 'QC' AND (wk.a_addressmailable = 0  OR wk.a_employeecode = 1) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Undefined', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  (wk.a_country is null or wk.a_country not in ('CAN', 'USA')) AND (wk.a_addressmailable = 0  OR wk.a_employeecode = 1) GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;

    ------------------------
    --Language Preference
    ------------------------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Language Preference------', '', '', '' From Dual;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT NVL(st.a_countrycode, 'Undefined'),
      CASE
        When wk.a_languagepreference = 1 Then 'Spanish'
        When wk.a_languagepreference = 2 Then 'French'
        Else 'English'
      End AS LanguagePref, '',
      COUNT(wk.a_ipcode) AS COUNT
    FROM  ats_rewardfulfillment_wrk wk
        LEFT JOIN ats_refstates st ON wk.a_stateorprovince = st.a_statecode
    GROUP BY st.a_countrycode, wk.a_languagepreference
    ORDER BY st.a_countrycode, wk.a_languagepreference;

    -----------
    --Samples
    -----------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Samples------', '', '', '' From Dual;

    ----------------
    --USA Samples
    ----------------
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'USA', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_country = 'USA'
        AND   rc.a_addressmailable > 0
        AND   rc.a_employeecode = 0
        AND rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;

    ----------------
    --Canada Samples
    ----------------
    v_RewardString := '15%20%30%40%';
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'CAN', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_country = 'CAN'
        AND    rc.a_stateorprovince <> 'QC'
        AND   rc.a_addressmailable > 0
        AND   rc.a_employeecode = 0
        AND rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;

    ----------------
    --Quebec Samples
    ----------------
    v_RewardString := '15%20%30%40%';
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'Quebec', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_stateorprovince = 'QC'
        AND   rc.a_employeecode = 0
        AND   rc.a_addressmailable > 0
        AND rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;

    ----------------
    --Undefined Samples
    ----------------
    v_RewardString := '15%20%30%40%';
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'Undefined', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_country = 'Undefined'
        AND   rc.a_addressmailable > 0
        AND   rc.a_employeecode = 0
        AND  rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;
  End If;

  If(v_IncludeEmployees = 'YES') Then
    -------------------------------------------------------------------------------------------------------------
    ---Get the counts based on including employees in the rewards file(s)
    -------------------------------------------------------------------------------------------------------------
    ------------------
    --Total Mailed
    ------------------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Total Mailed------', '', '', '' From Dual;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'USA', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'USA' AND wk.a_addressmailable > 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Canada', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'CAN' AND wk.a_stateorprovince <> 'QC' AND wk.a_addressmailable > 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Quebec', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE wk.a_stateorprovince = 'QC' AND wk.a_addressmailable > 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Undefined', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  (wk.a_country is null or wk.a_country not in ('CAN', 'USA')) AND wk.a_addressmailable > 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;

    --------------------
    --Total Un-Mailed
    --------------------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Total Un-Mailed------', '', '', '' From Dual;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'USA', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'USA' AND wk.a_addressmailable = 0 GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Canada', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  wk.a_country = 'CAN' AND wk.a_stateorprovince <> 'QC' AND wk.a_addressmailable = 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Quebec', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE wk.a_stateorprovince = 'QC' AND wk.a_addressmailable = 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT 'Undefined', wk.a_rewarddescription, wk.a_rewardtier, COUNT(*) AS COUNT FROM ats_rewardfulfillment_wrk wk WHERE  (wk.a_country is null or wk.a_country not in ('CAN', 'USA')) AND wk.a_addressmailable = 0  GROUP BY wk.a_rewarddescription, wk.a_rewardtier ORDER BY wk.a_rewarddescription, wk.a_rewardtier;

    ------------------------
    --Language Preference
    ------------------------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Language Preference------', '', '', '' From Dual;
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT NVL(st.a_countrycode, 'Undefined'),
      CASE
        When wk.a_languagepreference = 1 Then 'Spanish'
        When wk.a_languagepreference = 2 Then 'French'
        Else 'English'
      End AS LanguagePref, '',
      COUNT(wk.a_ipcode) AS COUNT
    FROM  ats_rewardfulfillment_wrk wk
        LEFT JOIN ats_refstates st ON wk.a_stateorprovince = st.a_statecode
    GROUP BY st.a_countrycode, wk.a_languagepreference
    ORDER BY st.a_countrycode, wk.a_languagepreference;

    -----------
    --Samples
    -----------
    INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords) SELECT '------Samples------', '', '', '' From Dual;

    ----------------
    --USA Samples
    ----------------
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'USA', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_country = 'USA'
        AND   rc.a_addressmailable > 0
        AND rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;

    ----------------
    --Canada Samples
    ----------------
    v_RewardString := '15%20%30%40%';
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'CAN', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_country = 'CAN'
        AND    rc.a_stateorprovince <> 'QC'
        AND   rc.a_addressmailable > 0
        AND rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;

    ----------------
    --Quebec Samples
    ----------------
    v_RewardString := '15%20%30%40%';
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'Quebec', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_stateorprovince = 'QC'
        AND   rc.a_addressmailable > 0
        AND rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;

    ----------------
    --Undefined Samples
    ----------------
    v_RewardString := '15%20%30%40%';
    WHILE (length(v_RewardString) > 0) LOOP
        INSERT INTO AE_RewardCounts (Country, Reward, Tier, Numberofrecords)
        SELECT  'Undefined', rc.a_rewarddescription, rc.a_loyaltynumber, ''
        FROM  ats_rewardfulfillment_wrk rc
        WHERE   rc.a_country = 'Undefined'
        AND   rc.a_addressmailable > 0
        AND  rc.a_rewarddescription = substr(v_RewardString, 1, 3)
        AND rownum < 6;

        v_RewardString := SubStr(v_RewardString, 4, length(v_RewardString));
      END LOOP;
  End If;

  COMMIT;


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
      v_Reason         := 'Failed Procedure QuarterlyRewardsPrelimCounts';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsPrelimCounts</proc>' ||
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

  END QuarterlyRewardsPrelimCounts;
/********************************************************************
  This process will create the MemberRewardsFulfillment work table and
  build it from the balances from the MemberPointBalance table.  This
  table will be used to get the preliminary counts and then will
  eventually be used to create the MemberRewards and then renamed to
  the current MemberRewardsFulfillment table.
  ********************************************************************/
PROCEDURE QuarterlyRewardsSelection(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsSelection';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsSelection';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsSelection';
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

    EXECUTE IMMEDIATE 'Truncate Table ats_rewardfulfillment_wrk';

    -----------------------------------------------------------------
    -- Insert into MemberRewards from MemberRewardFulfillment Work table
    -----------------------------------------------------------------
    DECLARE
      CURSOR get_data IS
    Select  /*+ full(md) full(pb) full(vc) full(lm)*/
        pb.a_ipcode as ipcode
       , pb.a_totalpoints as totalpoints
       , pb.a_rewardlevel as rewardlevel
       , rd.howmanypointstoearn as PointsRedeemed
       , nvl(md.a_employeecode,0) as employeecode
       , nvl(md.a_extendedplaycode,0) as extendedplaycode
       , 0 as MemberRewardId
       , lm.firstname
       , lm.lastname
       , md.a_addresslineone as addresslineone
       , md.a_addresslinetwo as addresslinetwo
       , md.a_city as city
       , md.a_stateorprovince as state
       , md.a_ziporpostalcode as postalcode
       , nvl(md.a_country,(select st.a_countrycode from ats_refstates st where st.a_statecode = md.a_stateorprovince))as country
       , nvl(md.a_addressmailable,0) as addressmailable
       , md.a_directmailoptindate as directmailoptindate
       , nvl(md.a_languagepreference,0) as languagepreference
       , rl.shortdescription as rewardTier
       , vc.loyaltyidnumber as loyaltyidnumber
       , CreateHHKeyCheckDigit(md.a_hhkey) as HHKeyBarCode
       , nvl(md.a_isunderage,0) as isunderage
       , pb.a_startingpoints as startingpoints
       , pb.a_basepoints as basepoints
       , pb.a_bonuspoints as bonuspoints
       , pd.partnumber as partnumber
       , pd.struserfield as BarCode
       , nvl(md.a_basebrandid,0) as basebrand
       , pb.a_brandae as brandae
       , pb.a_brandaerie as brandaerie
       , pb.a_brandkids as brandkids
       , nvl(md.a_directmailoptin , 0)as directmailoptin
       , 1 as Statuscode
       , sysdate as createdate
       , sysdate as updatedate
       , 'Qtr Rewards' as RewardType
       , 0 as ParentRewardId
from ats_memberpointbalances pb
INNER JOIN (SELECT /*+ full (x) */ x.ipcode, max(x.loyaltyidnumber) AS loyaltyidnumber
            from lw_virtualcard x
            WHERE isprimary = 1
            GROUP BY x.ipcode) vc ON vc.ipcode = pb.a_ipcode
--PI 30364 Dollar reward Changes begin here        SCJ
--inner join ats_memberdetails md on pb.a_ipcode = md.a_ipcode
inner join ats_memberdetails md on pb.a_ipcode = md.a_ipcode and nvl(md.a_extendedplaycode,0) <> 1
--PI 30364 Dollar reward Changes end here        SCJ
inner join lw_loyaltymember lm on pb.a_ipcode = lm.ipcode and lm.memberstatus = 1
inner join lw_rewardsdef rd on pb.a_rewardlevel = replace(rd.name, ' - Reward', null) and rd.name like '%Reward%' and rd.active = 1
inner join lw_product pd on rd.productid = pd.id and pd.isvisibleinln = 1
inner join lw_rewardlanguage rl on rd.id = rl.rewardid AND rl.languagecode = 1
Where pb.a_ipcode not in (select er.a_ipcode from ats_earlyrewardfulfillment er);
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
            insert into ats_rewardfulfillment_wrk (a_rowkey, a_ipcode, a_parentrowkey, a_pointbalance, a_rewarddescription, a_pointsredeemed,
                        a_employeecode, a_extendedplaycode, a_memberrewardid, a_firstname, a_lastname, a_addresslineone, a_addresslinetwo, a_city,
                        a_stateorprovince, a_ziporpostalcode, a_country, a_addressmailable, a_directmailoptindate, a_languagepreference, a_rewardtier,
                        a_loyaltynumber, a_hhkeybarcode, a_isunderage, a_startingpoints, a_basepointbalance, a_bonuspointbalance, a_rewardpartnumber,
                        a_barcode, a_baseloyaltybrand, a_purchasedaebrand, a_purchasedaeriebrand, a_purchasedkidsbrand, a_directmailoptin, statuscode,
                        createdate, updatedate, a_rewardtype, a_parentrewardid)
            values (seq_rowkey.nextval, v_tbl(i).ipcode, v_tbl(i).ipcode, v_tbl(i).totalpoints, v_tbl(i).rewardlevel, v_tbl(i).pointsredeemed,
                   v_tbl(i).employeecode, v_tbl(i).extendedplaycode, 0, v_tbl(i).firstname, v_tbl(i).lastname, v_tbl(i).addresslineone, v_tbl(i).addresslinetwo,
                   v_tbl(i).city, v_tbl(i).state, v_tbl(i).postalcode, v_tbl(i).country, v_tbl(i).addressmailable, v_tbl(i).directmailoptindate,
                   v_tbl(i).languagepreference, v_tbl(i).rewardtier, v_tbl(i).loyaltyidnumber, v_tbl(i).hhkeybarcode, v_tbl(i).isunderage, v_tbl(i).startingpoints,
                   v_tbl(i).basepoints, v_tbl(i).bonuspoints, v_tbl(i).partnumber, v_tbl(i).barcode, v_tbl(i).basebrand, v_tbl(i).brandae,
                   v_tbl(i).brandaerie, v_tbl(i).brandkids, v_tbl(i).directmailoptin, 1, sysdate, sysdate, 'Quarterly Rewards', 0);
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
      v_Reason         := 'Failed Procedure QuarterlyRewardsSelection';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsSelection</proc>' ||
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

  END QuarterlyRewardsSelection;


  /********************************************************************
  This process will create the MemberRewards records from the
  MemberRewardsFulfillment work table.
  ********************************************************************/
PROCEDURE QuarterlyRewardsCreateRewards(p_Dummy VARCHAR2, p_IncludeEmployees NUMBER,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsCreateRewards';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsCreateRewards';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsCreateRewards';
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
    -- Insert into MemberRewards from MemberRewardFulfillment Work table
    -----------------------------------------------------------------
    DECLARE
      CURSOR get_data IS
        Select rd.id as RewardId, pd.partnumber, wk.a_ipcode as ipcode, pd.id as ProductId,
               case
                 when wk.a_addressmailable = 1 then '4'
               else '0'
               end as ordernumber,
               wk.a_employeecode as employeecode
        From   ats_rewardfulfillment_wrk wk
              inner join lw_rewardsdef rd on wk.a_rewarddescription = replace(rd.name, ' - Reward', null) and rd.name like '%Reward%' and rd.active = 1
              inner join lw_product pd on rd.productid = pd.id and pd.isvisibleinln = 1;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
              insert into lw_memberrewards (id, rewarddefid, certificatenmbr, offercode, availablebalance, fulfillmentoption, memberid, productid, productvariantid,
                                    dateissued, expiration, fulfillmentdate, redemptiondate, changedby, lwordernumber, createdate)
              Values (hibernate_sequence.nextval, v_tbl(i).RewardId, v_tbl(i).partnumber, null, 0, null, v_tbl(i).ipcode, v_tbl(i).ProductId, null, sysdate,
                     null, null, null, 'SYSTEM',
                     case
                       when v_tbl(i).employeecode = 1 and p_IncludeEmployees = 0 Then '0'
                       when v_tbl(i).employeecode = 1 and p_IncludeEmployees = 1 Then v_tbl(i).ordernumber
                     else v_tbl(i).ordernumber
                     end,
                     sysdate);
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    END;
    -----------------------------------------------------------------
    -- Update MemberRewardId from MemberRewardFulfillment Work table
    -----------------------------------------------------------------
    DECLARE
      CURSOR get_data IS
        SELECT r.memberid as ipcode, r.id as memberrewardid
          FROM lw_memberrewards r
               inner join lw_rewardsdef rd on r.rewarddefid = rd.id
         WHERE trunc(r.dateissued) = trunc(sysdate)
           AND rd.name like '%Reward%' and rd.active = 1;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE ats_rewardfulfillment_wrk wk
             SET wk.a_memberrewardid = v_tbl(i).memberrewardid
           WHERE wk.a_ipcode = v_tbl(i).ipcode;
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
      v_Reason         := 'Failed Procedure QuarterlyRewardsCreateRewards';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsCreateRewards</proc>' ||
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

  END QuarterlyRewardsCreateRewards;

  /********************************************************************
  This process will rename the current MemberRewardsFulfillment to a backup
  name using the quarter and year and then rename the
  MemberRewardsFulfillment work table to the current MemberRewardsFulfillment
  table.
  If there was an early rewards process run for this quarter then that
    table will be truncated to prepare for the next
  ********************************************************************/
PROCEDURE QuarterlyRewardsBackupWkTbl(p_Dummy VARCHAR2, p_ProcessDate Date,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsBackupWkTbl';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsBackupWkTbl';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsBackupWkTbl';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));
    v_NewTableName VARCHAR2(256) := 'AE_REWARDFULFILLMENT_Q' || to_char(add_months(p_ProcessDate, -3)-10, 'Q') || '_' || to_char(TRUNC(add_months(p_ProcessDate, -3)-10), 'yyyy');
    v_NewEarlyTableName VARCHAR2(256) := 'AE_EARLYREWARDFULFILL_Q' || to_char(p_ProcessDate-10, 'Q') || '_' || to_char(TRUNC(p_ProcessDate-10), 'yyyy');

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


    EXECUTE IMMEDIATE 'Create Table '||v_NewTableName||' as select * from ATS_MEMBERREWARDFULFILLMENT';

    EXECUTE IMMEDIATE 'Truncate Table ATS_MEMBERREWARDFULFILLMENT';
    EXECUTE IMMEDIATE 'Insert Into ATS_MEMBERREWARDFULFILLMENT Select * From ats_rewardfulfillment_wrk';
    EXECUTE IMMEDIATE 'Truncate Table ats_rewardfulfillment_wrk';

    EXECUTE IMMEDIATE 'Create Table '||v_NewEarlyTableName||' as select * from ATS_EARLYREWARDFULFILLMENT';
    EXECUTE IMMEDIATE 'Truncate Table ATS_EARLYREWARDFULFILLMENT';


    COMMIT;

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
      v_Reason         := 'Failed Procedure QuarterlyRewardsBackupWkTbl';
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

  END QuarterlyRewardsBackupWkTbl;

  /********************************************************************
  This process will reset the EmployeeCode from Previous Employee to
  zero and then from Current Employee to Previous Employee
  ********************************************************************/
PROCEDURE QuarterlyRewardsResetEmpCode(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsResetEmpCode';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsResetEmpCode';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsResetEmpCode';
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
                 end
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
      v_Reason         := 'Failed Procedure QuarterlyRewardsResetEmpCode';
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

  END QuarterlyRewardsResetEmpCode;

  /********************************************************************
  This process will populate the reward counts table with the post counts
  ********************************************************************/
PROCEDURE QuarterlyRewardsPostCounts(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsPostCounts';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsPostCounts';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsPostCounts';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));

    v_Reward           VARCHAR2(50);
    v_Country           VARCHAR2(70);
    v_Country2         VARCHAR2(70);
    v_State             VARCHAR2(2);
    v_SampleAlternateID  VARCHAR2(20);
    v_PurchasedAEBrand  NUMBER;
    v_PurchasedAerieBrand NUMBER;
    v_Purchased77KidsBrand NUMBER;
    v_PurchaseBrand         VARCHAR2(70);

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


  EXECUTE IMMEDIATE 'Truncate Table AE_RewardCounts';

  INSERT INTO AE_RewardCounts (recordType, Country, FullName, State, NumberOfRecords) SELECT 1, 'Post Rewards Counts for Non High Tier: ' || trunc(sysdate, 'Q'), '', '', '' From Dual;
  INSERT INTO AE_RewardCounts (recordType, Country, FullName, State, NumberOfRecords) SELECT 1, 'Country',  'State\Province', '', 'COUNT' From Dual;

  ------------------------------------------------
  --INSERT all of the states into the Counts table
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, '------ Canada ------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, st.a_countrycode, st.a_fullname, st.a_statecode, 0 FROM ats_refstates st WHERE st.a_countrycode = 'CAN' AND st.a_statecode <> 'QC' ORDER BY st.a_fullname;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, '------ Quebec ------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, st.a_countrycode, st.a_fullname, st.a_statecode, 0 FROM ats_refstates st WHERE st.a_countrycode = 'CAN' AND st.a_statecode = 'QC' ORDER BY st.a_fullname;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, '------ USA ------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, st.a_countrycode, st.a_fullname, st.a_statecode, 0 FROM ats_refstates st WHERE st.a_countrycode = 'USA' ORDER BY st.a_fullname;

  COMMIT;
  ------------------------------------------------
  --Get the Counts per state
  ------------------------------------------------
  UPDATE AE_RewardCounts  prc
  SET    prc.NumberOfRecords =
  NVL((
  SELECT COUNT(fl.a_rewarddescription)
  FROM   ats_rewardfulfillment_wrk fl
  WHERE  fl.a_stateorprovince = prc.State
  AND    fl.a_country = prc.country
  AND    fl.a_addressmailable > 0
  AND    fl.a_employeecode in (0, 2)
  ), 0)
  WHERE prc.recordtype = 1
  AND prc.State is not null;
  COMMIT;

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped AE ONLY
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription, 'Reward Level: Main ', 'CAN', 0,1,0,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0 AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0,1,0,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0,1,0,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;
  COMMIT;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 0;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;


  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped Aerie Shopper Only
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 1, 0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 1, 0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 1, 0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  -- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped 77Kids ONLY
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'77Kids Shopper' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'77Kids Shopper' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'77Kids Shopper' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH AE AND Aerie
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE and aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,1,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE and aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,1,0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE and aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,1,0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  -----------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH AE AND 77Kids
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,0,1 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,0,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,0,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH Aerie AND 77Kids
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .aerie and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0,1,1 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .aerie and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .aerie and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped ALL AE, Aerie AND 77Kids
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE aerie and 77Kids Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,1,1 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE Aerie and 77Kids Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE Aerie and 77Kids Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped AE Total
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Total AE Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'AE Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Total AE Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'AE Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Total AE Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'AE Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;


  --Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 0; v_PurchaseBrand := 'AE Shopper Total';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN'; v_State := 'QC';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalAE(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped aerie Total
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Total aerie Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'aerie Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Total aerie Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'aerie Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Total aerie Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'aerie Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;


  --Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0; v_PurchaseBrand := 'aerie Shopper Total';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN'; v_State := 'QC';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalAerie(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(Country) SELECT '' From Dual;


  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH AE AND aerie brands onl
  -- Note: Display the count of members with the AE and aerie brand flag checked,
  --broken out by reward level. These members may also have the 77kids brand
  --flag checked, but this count should include anyone with both the AE and aerie
  --brand flag.
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Combo Total Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,1,0, 'Combo Total Shoppers' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Combo Total Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,1,0, 'Combo Total Shoppers' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Combo Total Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,1,0, 'Combo Total Shoppers' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0; v_PurchaseBrand := 'Combo Total Shoppers';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleCombo(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped 77Kids Total
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Total 77Kids Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'77Kids Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Total 77Kids Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'77Kids Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Total 77Kids Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'77Kids Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_employeecode = 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;


  --Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_State := null; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 1; v_PurchaseBrand := '77Kids Shopper Total';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN'; v_State := 'QC';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSampleTotalKids(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchaseBrand          => v_PurchaseBrand);

  COMMIT;

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
      v_Reason         := 'Failed Procedure QuarterlyRewardsPostCounts';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsPostCounts</proc>' ||
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

  END QuarterlyRewardsPostCounts;

  /********************************************************************
  This process will populate the reward counts table with the post counts
  ********************************************************************/
PROCEDURE QuarterlyRewardsPostCounts_Emp(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsPostCounts_Emp';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsPostCounts_Emp';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsPostCounts_Emp';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));

    v_Reward           VARCHAR2(50);
    v_Country           VARCHAR2(70);
    v_Country2         VARCHAR2(70);
    v_State             VARCHAR2(2);
    v_SampleAlternateID  VARCHAR2(20);
    v_PurchasedAEBrand  NUMBER;
    v_PurchasedAerieBrand NUMBER;
    v_Purchased77KidsBrand NUMBER;
    v_PurchaseBrand         VARCHAR2(70);

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


  EXECUTE IMMEDIATE 'Truncate Table AE_RewardCounts';

  INSERT INTO AE_RewardCounts (recordType, Country, FullName, State, NumberOfRecords) SELECT 1, 'Post Rewards Counts for Non High Tier: ' || trunc(sysdate, 'Q'), '', '', '' From Dual;
  INSERT INTO AE_RewardCounts (recordType, Country, FullName, State, NumberOfRecords) SELECT 1, 'Country',  'State\Province', '', 'COUNT' From Dual;

  ------------------------------------------------
  --INSERT all of the states into the Counts table
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, '------ Canada ------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, st.a_countrycode, st.a_fullname, st.a_statecode, 0 FROM ats_refstates st WHERE st.a_countrycode = 'CAN' AND st.a_statecode <> 'QC' ORDER BY st.a_fullname;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, '------ Quebec ------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, st.a_countrycode, st.a_fullname, st.a_statecode, 0 FROM ats_refstates st WHERE st.a_countrycode = 'CAN' AND st.a_statecode = 'QC' ORDER BY st.a_fullname;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, '------ USA ------', '', '', '' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Country, FullName, State, NumberOfRecords) SELECT 1, st.a_countrycode, st.a_fullname, st.a_statecode, 0 FROM ats_refstates st WHERE st.a_countrycode = 'USA' ORDER BY st.a_fullname;

  ------------------------------------------------
  --Get the Counts per state
  ------------------------------------------------
  UPDATE AE_RewardCounts  prc
  SET    prc.NumberOfRecords =
  NVL((
  SELECT COUNT(fl.a_rewarddescription)
  FROM   ats_rewardfulfillment_wrk fl
  WHERE  fl.a_stateorprovince = prc.State
  AND    fl.a_addressmailable > 0
  ), 0)
  WHERE prc.recordType = 1
  AND prc.State <> '';

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped AE ONLY
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0,1,0,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0,1,0,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0,1,0,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 0;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;


  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped Aerie Shopper Only
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 1, 0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 1, 0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 1, 0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  -- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped 77Kids ONLY
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'77Kids Shopper' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'77Kids Shopper' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'77Kids Shopper' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH AE AND Aerie
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE and aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,1,0 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE and aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,1,0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE and aerie Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,1,0  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  -----------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH AE AND 77Kids
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,0,1 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,0,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,0,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := ''; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH Aerie AND 77Kids
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .aerie and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0,1,1 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .aerie and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .aerie and 77Kids Only Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped ALL AE, Aerie AND 77Kids
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .AE aerie and 77Kids Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,1,1 FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .AE Aerie and 77Kids Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .AE Aerie and 77Kids Shopper' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,1,1  FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 1;
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped AE Total
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Total AE Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'AE Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Total AE Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'AE Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Total AE Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'AE Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;


  --Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 0; v_PurchaseBrand := 'AE Shopper Total';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN'; v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped aerie Total
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Total aerie Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'aerie Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Total aerie Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'aerie Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Total aerie Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'aerie Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;


  --Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0; v_PurchaseBrand := 'aerie Shopper Total';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN'; v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(Country) SELECT '' From Dual;


  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped BOTH AE AND aerie brands onl
  -- Note: Display the count of members with the AE and aerie brand flag checked,
  --broken out by reward level. These members may also have the 77kids brand
  --flag checked, but this count should include anyone with both the AE and aerie
  --brand flag.
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Combo Total Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 1,1,0, 'Combo Total Shoppers' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Combo Total Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 1,1,0, 'Combo Total Shoppers' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Combo Total Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 1,1,0, 'Combo Total Shoppers' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;

  ---- Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 1; v_PurchasedAerieBrand := 1; v_Purchased77KidsBrand := 0; v_PurchaseBrand := 'Combo Total Shoppers';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country2 := 'CAN';  v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ------------------------------------------------
  --Spacer
  ------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '' From Dual;

  ------------------------------------------------------------------
  --Get the Counts per Reward if they Shopped 77Kids Total
  ------------------------------------------------------------------
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Canada ------ .Total 77Kids Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'CAN', 0, 0, 0,1,'77Kids Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'CAN' AND fl.a_stateorprovince <> 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ Quebec ------ .Total 77Kids Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'QC', 0, 0, 0,1,'77Kids Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_stateorprovince = 'QC' ORDER BY fl.a_rewarddescription;
  INSERT INTO AE_RewardCounts(recordType, Country)        SELECT 2, '------ USA ------ .Total 77Kids Shoppers' From Dual;
  INSERT INTO AE_RewardCounts(recordType, Reward, RewardDescription, Country2, CurrentEmployee, PurchasedAEBrand, PurchasedAerieBrand, Purchased77KidsBrand, PurchasedBrand)
  SELECT distinct 2, fl.a_rewarddescription,  'Reward Level: Main ', 'USA', 0, 0, 0,1,'77Kids Shopper Total' FROM ats_rewardfulfillment_wrk fl WHERE Length(fl.a_rewardtier) > 0 AND fl.a_addressmailable > 0  AND fl.a_country = 'USA' ORDER BY fl.a_rewarddescription;


  --Update the Canada Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN';  v_PurchasedAEBrand := 0; v_PurchasedAerieBrand := 0; v_Purchased77KidsBrand := 1; v_PurchaseBrand := '77Kids Shopper Total';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the QC Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'CAN'; v_State := 'QC';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  ---- Update the USA Tier Counts and provide a sample
  v_Reward  := '15%'; v_Country := '' ; v_Country2 := 'USA';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);


  v_Reward  := '20%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '30%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  v_Reward  := '40%';
  AERewards.LoadSample_Employee(p_Reward                  => v_Reward,
              p_Country                => v_Country,
              p_Country2               => v_Country2,
              p_State                  => v_State,
              p_PurchasedAEBrand       => v_PurchasedAEBrand,
              p_PurchasedAerieBrand    => v_PurchasedAerieBrand,
              p_Purchased77KidsBrand   => v_Purchased77KidsBrand,
              p_PurchaseBrand          => v_PurchaseBrand);

  COMMIT;

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
      v_Reason         := 'Failed Procedure QuarterlyRewardsPostCounts_Emp';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsPostCounts_Emp</proc>' ||
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

  END QuarterlyRewardsPostCounts_Emp;

/********************************************************************
  This process will work the same as the QuarterlyRewardsSelect but
  will build the work table with the Early Reward Earners based on the
  following criteria.
   - 40% earners
   - CountryCode is not Canada
   - LanguagePreference is not French
  ********************************************************************/
PROCEDURE EarlyRewardsSelection(p_Dummy VARCHAR2,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.EarlyRewardsSelection';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.EarlyRewardsSelection';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'EarlyRewardsSelection';
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

    EXECUTE IMMEDIATE 'Truncate Table ats_rewardfulfillment_wrk';

    DECLARE
      CURSOR get_data IS
    Select  /*+ full(md) full(pb) full(vc) full(lm)*/
        pb.a_ipcode as ipcode
       , pb.a_totalpoints as totalpoints
       , pb.a_rewardlevel as rewardlevel
       , rd.howmanypointstoearn as PointsRedeemed
       , nvl(md.a_employeecode,0) as employeecode
       , nvl(md.a_extendedplaycode,0) as extendedplaycode
       , 0 as MemberRewardId
       , lm.firstname
       , lm.lastname
       , md.a_addresslineone as addresslineone
       , md.a_addresslinetwo as addresslinetwo
       , md.a_city as city
       , md.a_stateorprovince as state
       , md.a_ziporpostalcode as postalcode
       , nvl(md.a_country,(select st.a_countrycode from ats_refstates st where st.a_statecode = md.a_stateorprovince))as country
       , nvl(md.a_addressmailable,0) as addressmailable
       , md.a_directmailoptindate as directmailoptindate
       , nvl(md.a_languagepreference,0) as languagepreference
       , rl.shortdescription as rewardTier
       , vc.loyaltyidnumber as loyaltyidnumber
       , CreateHHKeyCheckDigit(md.a_hhkey) as HHKeyBarCode
       , nvl(md.a_isunderage,0) as isunderage
       , pb.a_startingpoints as startingpoints
       , pb.a_basepoints as basepoints
       , pb.a_bonuspoints as bonuspoints
       , pd.partnumber as partnumber
       , pd.struserfield as BarCode
       , nvl(md.a_basebrandid,0) as basebrand
       , pb.a_brandae as brandae
       , pb.a_brandaerie as brandaerie
       , pb.a_brandkids as brandkids
       , nvl(md.a_directmailoptin , 0)as directmailoptin
       , 1 as Statuscode
       , sysdate as createdate
       , sysdate as updatedate
       , 'Qtr Rewards' as RewardType
       , 0 as ParentRewardId
from ats_memberpointbalances pb
INNER JOIN (SELECT /*+ full (x) */ x.ipcode, max(x.loyaltyidnumber) AS loyaltyidnumber
            from lw_virtualcard x
            WHERE isprimary = 1
            GROUP BY x.ipcode) vc ON vc.ipcode = pb.a_ipcode
--PI 30364 Dollar reward Changes begin here        SCJ
--inner join ats_memberdetails md on pb.a_ipcode = md.a_ipcode
inner join ats_memberdetails md on pb.a_ipcode = md.a_ipcode and nvl(md.a_extendedplaycode,0) <> 1
--PI 30364 Dollar reward Changes end here        SCJ
inner join lw_loyaltymember lm on pb.a_ipcode = lm.ipcode and lm.memberstatus = 1
inner join lw_rewardsdef rd on pb.a_rewardlevel = replace(rd.name, ' - Reward', null) and rd.name like '%Reward%' and rd.active = 1
inner join lw_product pd on rd.productid = pd.id and pd.isvisibleinln = 1
inner join lw_rewardlanguage rl on rd.id = rl.rewardid AND rl.languagecode = 1
where pb.a_rewardlevel = '40%'
and   md.a_country <> 'CAN'
and   md.a_languagepreference in ('0','1');
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 1000; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
            insert into ats_rewardfulfillment_wrk (a_rowkey, a_ipcode, a_parentrowkey, a_pointbalance, a_rewarddescription, a_pointsredeemed,
                        a_employeecode, a_extendedplaycode, a_memberrewardid, a_firstname, a_lastname, a_addresslineone, a_addresslinetwo, a_city,
                        a_stateorprovince, a_ziporpostalcode, a_country, a_addressmailable, a_directmailoptindate, a_languagepreference, a_rewardtier,
                        a_loyaltynumber, a_hhkeybarcode, a_isunderage, a_startingpoints, a_basepointbalance, a_bonuspointbalance, a_rewardpartnumber,
                        a_barcode, a_baseloyaltybrand, a_purchasedaebrand, a_purchasedaeriebrand, a_purchasedkidsbrand, a_directmailoptin, statuscode,
                        createdate, updatedate, a_rewardtype, a_parentrewardid)
            values (seq_rowkey.nextval, v_tbl(i).ipcode, v_tbl(i).ipcode, v_tbl(i).totalpoints, v_tbl(i).rewardlevel, v_tbl(i).pointsredeemed,
                   v_tbl(i).employeecode, v_tbl(i).extendedplaycode, 0, v_tbl(i).firstname, v_tbl(i).lastname, v_tbl(i).addresslineone, v_tbl(i).addresslinetwo,
                   v_tbl(i).city, v_tbl(i).state, v_tbl(i).postalcode, v_tbl(i).country, v_tbl(i).addressmailable, v_tbl(i).directmailoptindate,
                   v_tbl(i).languagepreference, v_tbl(i).rewardtier, v_tbl(i).loyaltyidnumber, v_tbl(i).hhkeybarcode, v_tbl(i).isunderage, v_tbl(i).startingpoints,
                   v_tbl(i).basepoints, v_tbl(i).bonuspoints, v_tbl(i).partnumber, v_tbl(i).barcode, v_tbl(i).basebrand, v_tbl(i).brandae,
                   v_tbl(i).brandaerie, v_tbl(i).brandkids, v_tbl(i).directmailoptin, 1, sysdate, sysdate, 'Quarterly Rewards', 0);
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
      v_Reason         := 'Failed Procedure EarlyRewardsSelection';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>EarlyRewardsSelection</proc>' ||
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

  END EarlyRewardsSelection;


  /********************************************************************
  This process will rename the current EarlyRewardsFulfillment to a backup
  name using the quarter and year and then rename the
  MemberRewardsFulfillment work table to the current EarlyRewardsFulfillment
  table.
  RKG - Leaving this in for now.  May not need this as the Quarterly process
    will backup the early rewards table before truncating it and that should
    be sufficient for a backup.
  ********************************************************************/
PROCEDURE EarlyRewardsBackupWkTbl(p_Dummy VARCHAR2, p_ProcessDate Date,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.EarlyRewardsBackupWkTbl';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.EarlyRewardsBackupWkTbl';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'EarlyRewardsBackupWkTbl';
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


    EXECUTE IMMEDIATE 'Truncate Table ATS_EARLYREWARDFULFILLMENT';
    EXECUTE IMMEDIATE 'insert into ats_earlyrewardfulfillment
       (a_rowkey, a_ipcode, a_parentrowkey, a_pointbalance, a_rewarddescription, a_pointsredeemed, a_employeecode, a_extendedplaycode, a_memberrewardid, a_firstname, a_lastname, a_addresslineone,
       a_addresslinetwo, a_city, a_stateorprovince, a_ziporpostalcode, a_country, a_addressmailable, a_directmailoptindate, a_languagepreference, a_rewardtier, a_loyaltynumber, a_hhkeybarcode,
       a_isunderage, a_startingpoints, a_basepointbalance, a_bonuspointbalance, a_rewardpartnumber, a_barcode, a_baseloyaltybrand, a_purchasedaebrand, a_purchasedaeriebrand, a_purchasedkidsbrand,
       a_directmailoptin, statuscode, createdate, updatedate, lastdmlid, last_dml_id, a_rewardtype, a_parentrewardid)
select a_rowkey, a_ipcode, a_parentrowkey, a_pointbalance, a_rewarddescription, a_pointsredeemed, a_employeecode, a_extendedplaycode, a_memberrewardid, a_firstname, a_lastname, a_addresslineone,
       a_addresslinetwo, a_city, a_stateorprovince, a_ziporpostalcode, a_country, a_addressmailable, a_directmailoptindate, a_languagepreference, a_rewardtier, a_loyaltynumber, a_hhkeybarcode,
       a_isunderage, a_startingpoints, a_basepointbalance, a_bonuspointbalance, a_rewardpartnumber, a_barcode, a_baseloyaltybrand, a_purchasedaebrand, a_purchasedaeriebrand, a_purchasedkidsbrand,
       a_directmailoptin, statuscode, createdate, updatedate, lastdmlid, last_dml_id, a_rewardtype, a_parentrewardid
from ats_rewardfulfillment_wrk';
    EXECUTE IMMEDIATE 'Truncate Table ats_rewardfulfillment_wrk';


    COMMIT;

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
      v_Reason         := 'Failed Procedure EarlyRewardsBackupWkTbl';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>EarlyRewardsBackupWkTbl</proc>' ||
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

  END EarlyRewardsBackupWkTbl;
 /********************************************************************
 This procedure will look at the different parameters for post counts
 and load a sample record into the record counts.  This procedure
 will only load non-employee records.
 ********************************************************************/
    PROCEDURE LoadSample
      (
        p_Reward           VARCHAR2,
        p_Country           VARCHAR2,
        p_Country2         VARCHAR2,
        p_State             VARCHAR2,
        p_PurchasedAEBrand  NUMBER,
        p_PurchasedAerieBrand NUMBER,
        p_Purchased77KidsBrand NUMBER,
        p_PurchaseBrand VARCHAR2
      ) AS

      v_SampleAlternateID  VARCHAR2(20);

    BEGIN

    If(p_Country2 = 'CAN' And p_State is null) Then
      ---- Update the Canada Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince <> 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = p_PurchasedAEBrand
      AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand
      AND fl.a_purchasedkidsbrand = p_Purchased77KidsBrand
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND rc.currentemployee = 0 AND purchasedaebrand = p_PurchasedAEBrand AND purchasedaeriebrand = p_PurchasedAerieBrand AND  purchased77kidsbrand = p_Purchased77KidsBrand;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    If(p_Country2 = 'CAN' And Length(p_State) > 0) Then
      ---- Update the Quebec Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince = 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = p_PurchasedAEBrand
      AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand
      AND fl.a_purchasedkidsbrand = p_Purchased77KidsBrand
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND purchasedaebrand = p_PurchasedAEBrand AND purchasedaeriebrand = p_PurchasedAerieBrand AND  purchased77kidsbrand = p_Purchased77KidsBrand;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If        ;
    End If;

    If(p_Country2 = 'USA') Then
      ---- Update the USA Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = p_PurchasedAEBrand
      AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand
      AND fl.a_purchasedkidsbrand = p_Purchased77KidsBrand
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND fl.a_rewarddescription = p_Reward AND fl.a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND purchasedaebrand = p_PurchasedAEBrand AND purchasedaeriebrand = p_PurchasedAerieBrand AND  purchased77kidsbrand = p_Purchased77KidsBrand;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;




    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      NULL;
    WHEN OTHERS THEN
      RAISE;
    END LoadSample;

/********************************************************************
 This procedure will total all AE purchases
 ********************************************************************/
    PROCEDURE LoadSampleTotalAE
      (
        p_Reward           VARCHAR2,
        p_Country           VARCHAR2,
        p_Country2         VARCHAR2,
        p_State             VARCHAR2,
        p_PurchaseBrand VARCHAR2
      ) AS

      v_SampleAlternateID  VARCHAR2(20);

    BEGIN

    If(p_Country2 = 'CAN' And p_State is null) Then
      ---- Update the Canada Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince <> 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND rc.currentemployee = 0 AND purchasedaebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 ) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    If(p_Country2 = 'CAN' And Length(p_State) > 0) Then
      ---- Update the Quebec Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince = 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND purchasedaebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If        ;
    End If;

    If(p_Country2 = 'USA') Then
      ---- Update the USA Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND fl.a_rewarddescription = p_Reward AND fl.a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND purchasedaebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;




    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      NULL;
    WHEN OTHERS THEN
      RAISE;
    END LoadSampleTotalAE;

/********************************************************************
 This procedure will total all Aerie purchases
 ********************************************************************/
    PROCEDURE LoadSampleTotalAerie
      (
        p_Reward           VARCHAR2,
        p_Country           VARCHAR2,
        p_Country2         VARCHAR2,
        p_State             VARCHAR2,
        p_PurchaseBrand VARCHAR2
      ) AS

      v_SampleAlternateID  VARCHAR2(20);

    BEGIN

    If(p_Country2 = 'CAN' And p_State is null) Then
      ---- Update the Canada Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince <> 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaeriebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND rc.currentemployee = 0 AND rc.purchasedaeriebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaeriebrand = 1 ) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    If(p_Country2 = 'CAN' And Length(p_State) > 0) Then
      ---- Update the Quebec Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince = 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaeriebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedaeriebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If        ;
    End If;

    If(p_Country2 = 'USA') Then
      ---- Update the USA Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaeriebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND fl.a_rewarddescription = p_Reward AND fl.a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedaeriebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;




    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      NULL;
    WHEN OTHERS THEN
      RAISE;
    END LoadSampleTotalAerie;

/********************************************************************
 This procedure will total all 77 kids purchases
 ********************************************************************/
    PROCEDURE LoadSampleTotalKids
      (
        p_Reward           VARCHAR2,
        p_Country           VARCHAR2,
        p_Country2         VARCHAR2,
        p_State             VARCHAR2,
        p_PurchaseBrand VARCHAR2
      ) AS

      v_SampleAlternateID  VARCHAR2(20);

    BEGIN

    If(p_Country2 = 'CAN' And p_State is null) Then
      ---- Update the Canada Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince <> 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedkidsbrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedkidsbrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND rc.currentemployee = 0 AND rc.purchased77kidsbrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedkidsbrand = 1 ) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    If(p_Country2 = 'CAN' And Length(p_State) > 0) Then
      ---- Update the Quebec Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince = 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedkidsbrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedkidsbrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND purchased77kidsbrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedkidsbrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If        ;
    End If;

    If(p_Country2 = 'USA') Then
      ---- Update the USA Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_employeecode = 0
      AND fl.a_purchasedkidsbrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND fl.a_rewarddescription = p_Reward AND fl.a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedkidsbrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND purchased77kidsbrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedkidsbrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      NULL;
    WHEN OTHERS THEN
      RAISE;
    END LoadSampleTotalKids;

/********************************************************************
 This procedure will total combo purchases
 ********************************************************************/
    PROCEDURE LoadSampleCombo
      (
        p_Reward           VARCHAR2,
        p_Country           VARCHAR2,
        p_Country2         VARCHAR2,
        p_State             VARCHAR2,
        p_PurchaseBrand VARCHAR2
      ) AS

      v_SampleAlternateID  VARCHAR2(20);

    BEGIN

    If(p_Country2 = 'CAN' And p_State is null) Then
      ---- Update the Canada Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince <> 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = 1
      AND fl.a_purchasedaeriebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND rc.currentemployee = 0 AND rc.purchasedaebrand = 1 AND rc.purchasedaeriebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    If(p_Country2 = 'CAN' And Length(p_State) > 0) Then
      ---- Update the Quebec Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince = 'QC'
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = 1
      AND fl.a_purchasedaeriebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedaebrand = 1 AND rc.purchasedaeriebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If        ;
    End If;

    If(p_Country2 = 'USA') Then
      ---- Update the USA Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_employeecode = 0
      AND fl.a_purchasedaebrand = 1
      AND fl.a_purchasedaeriebrand = 1
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND fl.a_rewarddescription = p_Reward AND fl.a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedaebrand = 1 AND rc.purchasedaeriebrand = 1;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = 1 AND fl.a_purchasedaeriebrand = 1) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND currentemployee = 0 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      NULL;
    WHEN OTHERS THEN
      RAISE;
    END LoadSampleCombo;

 /********************************************************************
 This procedure will do the same as the LoadSample procedure but it
 will load everyone including employees.
 ********************************************************************/
    PROCEDURE LoadSample_Employee
      (
        p_Reward           VARCHAR2,
        p_Country           VARCHAR2,
        p_Country2         VARCHAR2,
        p_State             VARCHAR2,
        p_PurchasedAEBrand  NUMBER,
        p_PurchasedAerieBrand NUMBER,
        p_Purchased77KidsBrand NUMBER,
        p_PurchaseBrand VARCHAR2
      ) AS

      v_SampleAlternateID  VARCHAR2(20);

    BEGIN

    If(p_Country2 = 'CAN' And p_State is null) Then
      ---- Update the Canada Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince <> 'QC'
      AND fl.a_purchasedaebrand = p_PurchasedAEBrand
      AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand
      AND fl.a_purchasedkidsbrand = p_Purchased77KidsBrand
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND purchasedaebrand = p_PurchasedAEBrand AND purchasedaeriebrand = p_PurchasedAerieBrand AND  purchased77kidsbrand = p_Purchased77KidsBrand;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND state is null AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;

    If(p_Country2 = 'CAN' And Length(p_State) > 0) Then
      ---- Update the Quebec Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_stateorprovince = 'QC'
      AND fl.a_purchasedaebrand = p_PurchasedAEBrand
      AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand
      AND fl.a_purchasedkidsbrand = p_Purchased77KidsBrand
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_employeecode = 0 AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND currentemployee = 0 AND purchasedaebrand = p_PurchasedAEBrand AND purchasedaeriebrand = p_PurchasedAerieBrand AND  purchased77kidsbrand = p_Purchased77KidsBrand;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince = 'QC' AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = 'QC' AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If        ;
    End If;

    If(p_Country2 = 'USA') Then
      ---- Update the USA Tier Counts and provide a sample
      SELECT nvl(fl.a_loyaltynumber, '') Into v_SampleAlternateID
      FROM ats_rewardfulfillment_wrk fl
      WHERE fl.a_addressmailable = 1
      AND a_rewarddescription = p_Reward
      AND a_country = p_Country2
      AND fl.a_purchasedaebrand = p_PurchasedAEBrand
      AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand
      AND fl.a_purchasedkidsbrand = p_Purchased77KidsBrand
      AND rownum = 1;

      If(p_PurchaseBrand is null) Then
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND state <> 'QC' AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND purchasedaebrand = p_PurchasedAEBrand AND purchasedaeriebrand = p_PurchasedAerieBrand AND  purchased77kidsbrand = p_Purchased77KidsBrand;
        COMMIT;
      Else
        UPDATE AE_RewardCounts rc
        SET NumberOfRecords = NVL((SELECT COUNT(*) FROM ats_rewardfulfillment_wrk fl WHERE fl.a_addressmailable = 1 AND a_rewarddescription = p_Reward AND a_country = p_Country2 AND fl.a_stateorprovince <> 'QC' AND fl.a_purchasedaebrand = p_PurchasedAEBrand AND fl.a_purchasedaeriebrand = p_PurchasedAerieBrand AND  fl.a_purchasedkidsbrand = p_Purchased77KidsBrand) , 0),
          SampleAlternateID = v_SampleAlternateID
        WHERE rc.reward = p_Reward AND Country2 = p_Country2 AND rc.purchasedbrand = p_PurchaseBrand;
        COMMIT;
      End If;
    End If;



    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      NULL;
    WHEN OTHERS THEN
      RAISE;

    END LoadSample_Employee;
  /********************************************************************
  This process will roll back the rewards data by doing the following
  1. Deleting from MemberRewards where the Id is in the MemberRewardsFulfillment
  2. Copy the data from the MemberRewardsFulfillment to the MemberRewardsFulfillment_wrk table
  3. Copy the data from the AE_RewardFulfillment_Q4_2011 (or whatever the date) to the MemberRewardsFulfillment
  4. Delete the AE_RewardFulfillment_Q4_2011
  ********************************************************************/
PROCEDURE QuarterlyRewardsRollback(p_Dummy VARCHAR2, p_ProcessDate Date,
                                         Retval  IN OUT Rcursor) AS
    v_Logsource        VARCHAR2(256) := 'AERewards.QuarterlyRewardsRollback';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.QuarterlyRewardsRollback';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'QuarterlyRewardsRollback';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));
    v_NewTableName  VARCHAR2(256) := 'AE_REWARDFULFILLMENT_Q' || to_char(add_months(p_ProcessDate, -3)-10, 'Q') || '_' || to_char(TRUNC(add_months(p_ProcessDate, -3)-10), 'yyyy');

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


    --  1. Deleting from MemberRewards where the Id is in the MemberRewardsFulfillment
    Delete From lw_memberrewards where id in (select a_memberrewardid from ATS_MEMBERREWARDFULFILLMENT fl);
    commit;

    --  2. Copy the data from the MemberRewardsFulfillment to the MemberRewardsFulfillment_wrk table
    Insert Into ats_rewardfulfillment_wrk Select * From ATS_MEMBERREWARDFULFILLMENT;

    --  3. Copy the data from the AE_RewardFulfillment_Q4_2011 (or whatever the date) to the MemberRewardsFulfillment
    EXECUTE IMMEDIATE 'Truncate Table ATS_MEMBERREWARDFULFILLMENT';
    EXECUTE IMMEDIATE 'Insert Into ATS_MEMBERREWARDFULFILLMENT Select * From '||v_NewTableName;

    --  4. Delete the AE_RewardFulfillment_Q4_2011
    EXECUTE IMMEDIATE 'Drop Table '||v_NewTableName;



    COMMIT;

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
      v_Reason         := 'Failed Procedure QuarterlyRewardsRollback';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AEJobs</pkg>' || Chr(10) ||
                          '    <proc>QuarterlyRewardsRollback</proc>' ||
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

  END QuarterlyRewardsRollback;

-- AEO-2114 begin


FUNCTION is_valid_date_format ( p_date varchar2, p_format IN VARCHAR2 ) 
RETURN BOOLEAN IS
    l_date VARCHAR2(100) := NULL;
BEGIN
    l_date := TO_date( p_date , p_format );
    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RETURN FALSE;
END is_valid_date_format;

PROCEDURE GetRewardsToIssue as
  
   cursor get_data is
 -- jean rewards
select w.IPCODE, w.ROWTYPE, w.POINTS, nvl(md2.a_cardtype,0) as CARDTYPE, tier.tierlevel as TIERLEVEL
 from (
  SELECT IPCODE, ROWTYPE, points
  FROM (SELECT vc.ipcode AS IPCODE,
               'J' AS ROWTYPE,
               row_number() OVER(PARTITION BY vc.ipcode ORDER BY vc.ipcode) rn,
               sum(pt.points - pt.pointsconsumed) as points
          FROM bp_ae.lw_pointtransaction pt
         INNER JOIN bp_ae.lw_pointevent pe       ON pt.pointeventid = pe.pointeventid
         INNER JOIN bp_ae.lw_pointtype p         ON pt.pointtypeid = p.pointtypeid
         INNER JOIN bp_ae.lw_virtualcard vc      ON pt.vckey = vc.vckey
         INNER JOIN bp_ae.ats_memberdetails md   ON vc.ipcode = md.a_ipcode
         INNER JOIN bp_ae.lw_loyaltymember lm    ON vc.ipcode = lm.ipcode
         WHERE 1 = 1
           AND  ((p.Name = 'Jean Points') OR (pe.name = 'Jean Credit Appeasement'))                      
           AND ((length(md.a_emailaddress) > 0) OR
                --(nvl(md.a_smsoptin, 1) = 0 AND length(md.a_mobilephone) > 0) OR
                (md.a_cardtype IN (1, 2, 3) AND
                 md.a_cardopendate < to_date('9/5/2017', 'mm/dd/yyyy')))
           AND pt.pointsonhold = 0
           AND pt.transactiontype NOT IN (3, 4)
           AND pt.expirationdate > sysdate
         --  AND nvl(md.a_employeecode, 0) NOT IN (1, 2)
           AND lm.memberstatus = 1
         GROUP BY vc.ipcode
        HAVING SUM(pt.points - pt.pointsconsumed) >= 5)
  WHERE rn = 1 ) w 
  inner join bp_ae.ats_memberdetails md2 on md2.a_ipcode = w.ipcode
  inner join (
      select t.tierlevel, t.memberid 
      from (SELECT ti.tiername as tierlevel,
                   mt.memberid,
                   row_number() OVER(PARTITION BY mt.memberid ORDER BY mt.todate desc, mt.fromdate ASC) rn
              FROM bp_ae.lw_membertiers mt
             INNER JOIN bp_ae.lw_tiers ti
                ON ti.tierid = MT.TIERID
             WHERE mt.todate > SYSDATE) t
      where t.rn = 1
  ) tier on tier.memberid = md2.a_ipcode
-- bra rewards
union 
select w.IPCODE, w.ROWTYPE, w.POINTS, nvl(md2.a_cardtype,0) as CARDTYPE, tier.tierlevel as TIERLEVEL
 from (
  SELECT IPCODE, ROWTYPE, points
  FROM (SELECT vc.ipcode AS IPCODE,
               'B' AS ROWTYPE,
               row_number() OVER(PARTITION BY vc.ipcode ORDER BY vc.ipcode) rn,
               sum(pt.points - pt.pointsconsumed) as points
          FROM bp_ae.lw_pointtransaction pt
         INNER JOIN bp_ae.lw_pointevent pe       ON pt.pointeventid = pe.pointeventid
         INNER JOIN bp_ae.lw_pointtype p         ON pt.pointtypeid = p.pointtypeid
         INNER JOIN bp_ae.lw_virtualcard vc      ON pt.vckey = vc.vckey
         INNER JOIN bp_ae.ats_memberdetails md   ON vc.ipcode = md.a_ipcode
         INNER JOIN bp_ae.lw_loyaltymember lm    ON vc.ipcode = lm.ipcode
         WHERE 1 = 1
           AND  ((p.Name = 'Bra Points') OR (pe.name = 'Bra Credit Appeasement'))                      
           AND ((length(md.a_emailaddress) > 0) OR
               -- (nvl(md.a_smsoptin, 1) = 0 AND length(md.a_mobilephone) > 0) OR
                (md.a_cardtype IN (1, 2, 3) AND
                 md.a_cardopendate < to_date('9/5/2017', 'mm/dd/yyyy')))
           AND pt.pointsonhold = 0
           AND pt.transactiontype NOT IN (3, 4)
           AND pt.expirationdate > sysdate
         --  AND nvl(md.a_employeecode, 0) NOT IN (1, 2)
           AND lm.memberstatus = 1
         GROUP BY vc.ipcode
        HAVING SUM(pt.points - pt.pointsconsumed) >= 5)
  WHERE rn = 1 ) w 
  inner join bp_ae.ats_memberdetails md2 on md2.a_ipcode = w.ipcode
  inner join (
      select t.tierlevel, t.memberid 
      from (SELECT ti.tiername as tierlevel,
                   mt.memberid,
                   row_number() OVER(PARTITION BY mt.memberid ORDER BY mt.todate desc, mt.fromdate ASC) rn
              FROM bp_ae.lw_membertiers mt
             INNER JOIN bp_ae.lw_tiers ti
                ON ti.tierid = MT.TIERID
             WHERE mt.todate > SYSDATE) t
      where t.rn = 1
  ) tier on tier.memberid = md2.a_ipcode  
union
-- dollar rewards
  select w.IPCODE, w.ROWTYPE, w.POINTS, nvl(md2.a_cardtype,0) as CARDTYPE, tier.tierlevel as TIERLEVEL
  from (SELECT IPCODE, ROWTYPE, points
          from (SELECT IPCODE, ROWTYPE, POINTS
                  FROM (SELECT vc.ipcode AS IPCODE,
                               '$' AS ROWTYPE,
                               row_number() OVER(PARTITION BY vc.ipcode ORDER BY vc.ipcode) rn,
                               SUM(pt.points - pt.pointsconsumed) as Points
                          FROM bp_ae.lw_pointtransaction pt
                         INNER JOIN bp_ae.lw_pointevent pe                ON pt.pointeventid = pe.pointeventid
                         INNER JOIN bp_ae.lw_pointtype p                  ON pt.pointtypeid = p.pointtypeid
                         INNER JOIN bp_ae.lw_virtualcard vc               ON pt.vckey = vc.vckey
                         INNER JOIN bp_ae.ats_memberdetails md            ON vc.ipcode = md.a_ipcode
                         INNER JOIN bp_ae.Lw_Loyaltymember lm             ON vc.ipcode = lm.ipcode
                         WHERE 1 = 1
                           AND ((upper(trim(p.name)) IN
                               ('AEO CONNECTED POINTS',--
                                  'AEO CONNECTED BONUS POINTS',--
                                  -- 'ADJUSTMENT POINTS',
                                  'AEO CUSTOMER SERVICE POINTS',
                                  -- 'ADJUSTMENT BONUS POINTS',
                                  'AEO VISA CARD POINTS') AND--
                               (pt.transactiontype = 1 OR
                               pt.transactiontype = 2)) OR
                               upper(trim(p.name)) IN ('BONUS POINTS'))--
                           AND ((length(md.a_emailaddress) > 0) OR
                               /*(nvl(md.a_smsoptin, 1) = 0 AND
                               length(md.a_mobilephone) > 0) OR*/
                               (md.a_cardtype IN (1, 2, 3) AND
                               md.a_cardopendate <
                               to_date('9/5/2017', 'mm/dd/yyyy')))
                           AND pt.pointsonhold = 0
                           AND pt.transactiontype NOT IN (3, 4)
                           AND pt.expirationdate > sysdate
                           -- AND nvl(md.a_employeecode, 0) NOT IN (1, 2)
                           AND lm.memberstatus = 1
                         GROUP BY vc.ipcode
                        HAVING SUM(pt.points - pt.pointsconsumed) >= 2500)
                 where rn = 1)) w
  inner join bp_ae.ats_memberdetails md2   on md2.a_ipcode = w.ipcode
  inner join (select t.tierlevel, t.memberid
           from (SELECT ti.tiername as tierlevel,
                mt.memberid,
                row_number() OVER(PARTITION BY mt.memberid ORDER BY mt.todate desc, mt.fromdate ASC) rn
               FROM bp_ae.lw_membertiers mt
              INNER JOIN bp_ae.lw_tiers ti
               ON ti.tierid = MT.TIERID
              WHERE mt.todate > SYSDATE) t
          where t.rn = 1) tier    on tier.memberid = md2.a_ipcode;
    
  
    v_Logsource        VARCHAR2(256) := 'AERewards.GetRewardsToIssue';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AERewards.GetRewardsToIssue';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'GetRewardsToIssue';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));
                                                          
      v_processdate timestamp := null;
         
                                                          
      TYPE t_tab2 IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl2 t_tab2; ---<------ our arry object
      
    
begin
  
  

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

   
      v_processdate := SYSDATE;
     
     

      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl2 LIMIT 1000;
          
        FORALL i IN 1 .. v_tbl2.count 
          insert into bp_ae.ae_rewardtoissue
            (ipcode, points, rewardtype, tierlevel,cardtype, datestaged, createdate)
          values
            (v_tbl2(i).ipcode,
             v_tbl2(i).points,
             v_tbl2(i).rowtype,
             v_tbl2(i).tierlevel,
             v_tbl2(i).cardtype,
             v_processdate,
             sysdate);
             
        COMMIT; 
        EXIT WHEN get_data%NOTFOUND; 
      END LOOP;
      COMMIT;
      
      IF get_data%ISOPEN THEN       
        CLOSE get_data;
      END IF;
      
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
      v_Reason         := 'Failed Procedure GetRewardsToIssue';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AERewards</pkg>' || Chr(10) ||
                          '    <proc>GetRewardsToIssue</proc>' ||
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
    
  end GetRewardsToIssue;



Procedure SetLastRewardDate( p_newDateTime varchar2) is
      v_tmpDate Date := null;
      v_exist   number := 0;
      ex_invalid_date exception;
       
  begin
    
     select count(*) 
     into v_exist 
     from bp_ae.lw_clientconfiguration cc
     where cc.key = 'LastRewardDate' ;
     
     if ( is_valid_date_format (p_newDatetime,'mm/dd/yyyy HH24:mi:ss')) then
        
         if (v_exist = 0) then
           insert into bp_Ae.lw_clientconfiguration
             (key, Value, Externalvalue, Createdate, Last_Dml_Id, Updatedate)
           values
             ('LastRewardDate',p_newDateTime,null, sysdate, last_dml_id#.nextval, null   );
         else 
            update bp_ae.lw_clientconfiguration cc
              set cc.value = p_newDateTime
              where cc.key =  'LastRewardDate';                
         end if;
         commit;
     else
       raise ex_invalid_date;      
       
       
     end if;  
     
  
  end SetLastRewardDate ;

  
-- AEO-2114 end

END AERewards;
/
