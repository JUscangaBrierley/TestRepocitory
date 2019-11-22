CREATE OR REPLACE PACKAGE BonusEventprocessing AS

TYPE Rcursor IS REF CURSOR;
PROCEDURE BonusEventpointissue(p_Dummy VARCHAR2, Retval IN OUT Rcursor);
PROCEDURE BonusEventpointreturn(p_Dummy VARCHAR2);
PROCEDURE UpdateEventprocessingLastrun(p_Dummy VARCHAR2);
END BonusEventprocessing;
/

CREATE OR REPLACE PACKAGE BODY BonusEventprocessing IS
PROCEDURE BonusEventpointissue(p_Dummy VARCHAR2,
                                      Retval        IN OUT Rcursor) IS
  pdummy varchar2(2);
  v_eventclasscodes        varchar2(4000);
  v_eventoverideclasscode  varchar2(4000);
  v_eventstartdate         date;
  v_eventenddate           date;
  v_sql1                   VARCHAR2(1000) ;
  v_EventprocessingLastrun DATE;
  v_cnt                    NUMBER := 0;
  v_eventmultiplier        number :=0;
  v_pointeventid           number := 0;
  v_pointtypeid            number :=0;
  v_eventoveridestartdate  date;
  v_eventoverideenddate    date;
  v_bonuspointtypeid       NUMBER;
  v_TxnStartDT        DATE; -- PI31900 changes

--cursor1- get the bonus rule from ATS_BONUSEVENTDETAIL
  CURSOR get_rule IS
      select t.a_eventstartdate,
             t.a_eventenddate,
             t.a_eventclasscodes,
             t.a_eventoverideclasscode,
             t.a_pointeventid,
             t.a_pointtypeid,
             t.a_eventmultiplier,
             t.a_eventoveridestartdate,
             t.a_eventoverideenddate
        from ATS_BONUSEVENTDETAIL t
       where t.a_eventstatus = 1;
--cursor2 - Insert point txns from  those purchase detailtxns excluding the txns that contain override classcode for inserting into point txn tble
  Cursor dtl_data(b_eventstartdate date,b_eventenddate date,b_eventclasscodes varchar2,b_eventoverideclasscode varchar2,b_pointeventid number,b_pointtypeid number,b_eventmultiplier number,b_eventoveridestartdate date,b_eventoverideenddate date ) is
      select max(dt.a_vckey) as v_vckey,
             dt.a_txnheaderid as v_txnheaderid,
             sum(dt.a_dtlsaleamount) as v_dtlsaleamount,
             max(dt.a_txndate) as v_txndate,
             max(dt.a_rowkey) as v_rowkey,
             max(b_pointeventid) as v2_pointeventid,
             max(b_pointtypeid) as v2_pointtypeid,
             round(sum(dt.a_dtlsaleamount) * max(b_eventmultiplier), 0) as v_bonuspoints
        from AE_dailytransactions dt
       where dt.a_txndate >= b_eventstartdate
         and dt.a_txndate < b_eventenddate
         and dt.a_dtlsaleamount > 0 -- only dtl purchases
            -- and NVL(dt.a_multiuseflag,0) = 0
            -- and dt.a_dtlclasscode in (select regexp_substr(str, '[^,]+', 1, level) classcodes   from (select b_eventclasscodes str from dual)
            --                          connect by regexp_substr(str, '[^,]+', 1, level) is not null )
         and dt.a_dtlclasscode in
             (SELECT COLUMN_VALUE
                FROM table(StringToTable((b_eventclasscodes), ',')))
            --exclude the txns in the overide classcode list
         and not exists
       (select 1
                from AE_dailytransactions dt2
               where dt2.a_txndate >= b_eventoveridestartdate
                 and dt2.a_txndate < b_eventoverideenddate
                 and
                    -- dt2.a_dtlclasscode in ( select regexp_substr(str1, '[^,]+', 1, level) classcodes   from ((select b_eventoverideclasscode str1 from  dual))
                    --connect by regexp_substr(str1, '[^,]+', 1, level) is not null )
                     dt2.a_dtlclasscode in
                     (SELECT COLUMN_VALUE
                        FROM table(StringToTable((b_eventoverideclasscode),
                                                 ',')))
                 and dt.a_txnheaderid = dt2.a_txnheaderid)
            --exclude the possible txns that were awarded bonus in the tlog process or any txn that already has a bonus awarded
         and not exists
       (select 1
                from AE_dailytransactions dt3
               inner join lw_pointtransaction pt
                  on pt.rowkey = dt3.a_rowkey
               where dt3.a_dtlsaleamount > 0
                 and pt.pointtypeid =
                     (select pointtypeid
                        from lw_pointtype
                       where name = 'Bonus Points')
                 and pt.points > 0
                 and dt.a_txnheaderid = dt3.a_txnheaderid)
       group by dt.a_txnheaderid;

      TYPE dtl_tab IS TABLE OF dtl_data%ROWTYPE;
    v_dtltbl dtl_tab;

BEGIN
   v_sql1 := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'')  from lw_clientconfiguration cc where cc.key = ''EventprocessingLastrun''';
   EXECUTE IMMEDIATE 'Drop Table AE_dailytransactions';
   EXECUTE IMMEDIATE v_sql1 into v_EventprocessingLastrun ;
   UpdateEventprocessingLastrun(pdummy);
    EXECUTE IMMEDIATE 'Create Table AE_dailytransactions As select ht.a_vckey,ht.a_rowkey,dt.a_dtlclasscode,dt.a_txnheaderid,dt.a_txndate,dt.a_dtlsaleamount,ht.a_txnoriginaltxnrowkey from ats_txnheader ht
                 inner join ats_txndetailitem dt on dt.a_txnheaderid = ht.a_txnheaderid
                 inner join lw_virtualcard vc on vc.vckey = ht.a_vckey
                 inner join ats_memberdetails md on md.a_ipcode = vc.ipcode
                 where  NVL(md.a_employeecode,0) <> 1
                 and trim(ht.a_txnemployeeid) is null and
                 ht.createdate >=  to_date('''||to_char(v_EventprocessingLastrun,'mm/dd/yyyy hh24:mi:ss')||''',''mm/dd/yyyy hh24:mi:ss'') ' ;
    SELECT pointtypeid INTO v_bonuspointtypeid FROM  lw_pointtype WHERE NAME = 'Bonus Points';
    SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate'; -- PI31900 changes

 Open get_rule;
    Loop
      Fetch get_rule into v_eventstartdate,v_eventenddate,v_eventclasscodes,v_eventoverideclasscode,v_pointeventid,v_pointtypeid,v_eventmultiplier,v_eventoveridestartdate,v_eventoverideenddate;
       Exit when get_rule%NOTFOUND;
       OPEN dtl_data(v_eventstartdate,v_eventenddate,v_eventclasscodes,v_eventoverideclasscode,v_pointeventid,v_pointtypeid,v_eventmultiplier,v_eventoveridestartdate,v_eventoverideenddate);
         Loop
        fetch dtl_data BULK COLLECT
                 INTO v_dtltbl LIMIT 10000;
                 FORALL j IN 1 .. v_dtltbl.COUNT
             INSERT INTO lw_pointtransaction
         (Pointtransactionid
         ,Vckey
         ,Pointtypeid
         ,Pointeventid
         ,Transactiontype
         ,Transactiondate
         ,Pointawarddate
         ,Points
         ,Expirationdate
         ,Notes
         ,Ownertype
         ,Ownerid
         ,Rowkey
         ,Parenttransactionid
         ,Pointsconsumed
         ,Pointsonhold
         ,Ptlocationid
         ,Ptchangedby
         ,Last_Dml_Date
         ,Expirationreason)
       VALUES
         (seq_pointtransactionid.nextval
         ,v_dtltbl(j).v_vckey
         ,v_bonuspointtypeid  /*bonus Pointtypeid*/
         ,v_dtltbl(j).v2_pointeventid /*pointeventid=AE Return*/
         ,1 /*purchase*/
--         ,sysdate  -- PI31900 changes
         ,v_dtltbl(j).v_txndate -- PI31900 changes
         ,SYSDATE
         ,v_dtltbl(j).v_bonuspoints /*Points*/
         /* -- PI31900 changes
         ,case when (select nvl(md.a_extendedplaycode,0) from lw_virtualcard vc inner join ats_memberdetails md on md.a_ipcode = vc.ipcode where vc.vckey = v_dtltbl(j).v_vckey) = 1 then
             Add_Months(Trunc(sysdate, 'Y'), 12) else add_months(trunc(sysdate,'Q'),3) end
             */
         ,case when v_dtltbl(j).v_txndate >= v_TxnStartDT
               and  (select nvl(md.a_extendedplaycode,0)
                     from lw_virtualcard vc
                     inner join ats_memberdetails md on md.a_ipcode = vc.ipcode
                     where vc.vckey = v_dtltbl(j).v_vckey) = 1
               then  Add_Months(Trunc(v_dtltbl(j).v_txndate, 'Y'), 12) + 1
               else add_months(trunc(v_dtltbl(j).v_txndate,'Q'),3)
               end
         ,'Bonus Points' /*Notes*/
         ,0
         ,-1
         ,v_dtltbl(j).v_rowkey
         ,-1
         ,0 /*Pointsconsumed*/
         ,0 /*Pointsonhold*/
         ,NULL
         ,'Custom Bonus Processor' /*Ptchangedby*/
         ,SYSDATE
         ,NULL);
         v_cnt := v_cnt + 1;
       IF MOD(v_cnt,10000) = 0 THEN
         COMMIT; -- commit every 10000 inserts
       END IF;
       Exit when dtl_data%NOTFOUND;
     End loop;
     CLOSE dtl_data;
    END LOOP;
  CLOSE get_rule;
   COMMIT;
    BonusEventpointreturn(pdummy);
END BonusEventpointissue;

PROCEDURE BonusEventpointreturn(p_Dummy VARCHAR2            ) IS
  v_vckey           NUMBER := 0;
  v_cnt             NUMBER := 0;
  v_bonuspointtypeid       NUMBER;
  v_pointeventid     number;
  v_TxnStartDT        DATE; -- PI31900 changes

--cursor1-
  CURSOR get_returns IS with Mem as
(     select dts.a_txnheaderid,
       max(dts.a_vckey) as ret_vckey,
       sum(dts.a_dtlsaleamount) as ret_saleamount,
       max(dts.a_txndate) as ret_txndate,
       max(dts.a_txnoriginaltxnrowkey) as ret_originalrowkey,
       max(pt.pointeventid) as ret_pointeventid
  from AE_dailytransactions dts
 inner join lw_pointtransaction pt
    on pt.rowkey = dts.a_txnoriginaltxnrowkey
--  inner join ats_bonuseventdetail bd on bd.a_pointeventid = pt.pointeventid
 where dts.a_dtlsaleamount < 0
   and dts.a_txnoriginaltxnrowkey > 0
   and pt.pointtypeid =
       (select pointtypeid from lw_pointtype where name = 'Bonus Points')
   and pt.points > 0
   -- PI#31379, Rizwan, A condition is modified to accomodate transaction header level returns, Start
   and (dts.a_dtlclasscode in
       (SELECT t.COLUMN_VALUE AS classcode
          FROM table(StringToTable((select bd.a_eventclasscodes
                                     from ATS_BONUSEVENTDETAIL bd
                                    where bd.a_pointeventid =
                                          pt.pointeventid),
                                   ',')) t)
    or 'ALL' in (SELECT t.COLUMN_VALUE AS classcode
                   FROM table(StringToTable((select bd.a_eventclasscodes
                                              from ATS_BONUSEVENTDETAIL bd
                                             where bd.a_pointeventid =
                                                   pt.pointeventid),
                                            ',')) t))
   -- PI#31379, Rizwan, A condition is modified to accomodate transaction header level returns, End
 group by dts.a_txnheaderid)
      SELECT * FROM Mem;
      TYPE t_tab IS TABLE OF get_returns%ROWTYPE;
    v_tbl t_tab;
BEGIN
  SELECT pointtypeid INTO v_bonuspointtypeid FROM  lw_pointtype WHERE NAME = 'Bonus Points';
  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
 Open get_returns;
    Loop
     FETCH get_returns BULK COLLECT
        INTO v_tbl LIMIT 10000;
  FORALL k IN 1 .. v_tbl.COUNT
      INSERT INTO lw_pointtransaction
         (Pointtransactionid
         ,Vckey
         ,Pointtypeid
         ,Pointeventid
         ,Transactiontype
         ,Transactiondate
         ,Pointawarddate
         ,Points
         ,Expirationdate
         ,Notes
         ,Ownertype
         ,Ownerid
         ,Rowkey
         ,Parenttransactionid
         ,Pointsconsumed
         ,Pointsonhold
         ,Ptlocationid
         ,Ptchangedby
         ,Last_Dml_Date
         ,Expirationreason)
       VALUES
         (seq_pointtransactionid.nextval
         ,v_tbl(k).ret_vckey
         ,v_bonuspointtypeid /*Pointtypeid*/
         ,v_tbl(k).ret_pointeventid  /*pointeventid=AE Return*/
         ,2 /*return*/
 --         ,SYSDATE -- PI31900 changes
         ,v_tbl(k).ret_txndate  -- PI31900 changes
         ,SYSDATE
         ,Round(v_tbl(k).ret_saleamount *(select NVL((select bd.a_eventmultiplier from ats_bonuseventdetail bd where bd.a_pointeventid = v_tbl(k).ret_pointeventid),1)from dual),0) /*Points*/
         /*  -- PI31900 changes
         ,case when (select nvl(md.a_extendedplaycode,0) from lw_virtualcard vc inner join ats_memberdetails md on md.a_ipcode = vc.ipcode where vc.vckey = v_tbl(k).ret_vckey) = 1 then
             Add_Months(Trunc(sysdate, 'Y'), 12) else add_months(trunc(sysdate,'Q'),3) end
             */
         ,case when v_tbl(k).ret_txndate >= v_TxnStartDT
               and (select nvl(md.a_extendedplaycode,0)
                    from lw_virtualcard vc
                    inner join ats_memberdetails md on md.a_ipcode = vc.ipcode
                    where vc.vckey = v_tbl(k).ret_vckey) = 1
               then Add_Months(Trunc(v_tbl(k).ret_txndate, 'Y'), 12)+ 1
               else add_months(trunc(v_tbl(k).ret_txndate,'Q'),3)
          end
         ,'Bonus Points Return' /*Notes*/
         ,0
         ,-1
         ,v_tbl(k).ret_originalrowkey
         ,-1
         ,0 /*Pointsconsumed*/
         ,0 /*Pointsonhold*/
         ,NULL
         ,'Custom Bonus Processor' /*Ptchangedby*/
         ,SYSDATE
         ,NULL);
       COMMIT;
      EXIT WHEN get_returns%NOTFOUND;
    END LOOP;
   COMMIT;
    IF get_returns%ISOPEN THEN
      CLOSE get_returns;
    END IF;
    COMMIT;
 --END;
END BonusEventpointreturn;

 PROCEDURE UpdateEventprocessingLastrun(p_Dummy VARCHAR2) AS
  BEGIN

    UPDATE Lw_Clientconfiguration
       SET VALUE = To_Char(SYSDATE, 'mm/dd/yyyy hh24:mi:ss')
     WHERE Key = 'EventprocessingLastrun';
    COMMIT;

  END UpdateEventprocessingLastrun;

END BonusEventprocessing;
/

