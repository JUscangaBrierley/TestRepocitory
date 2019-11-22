CREATE OR REPLACE PACKAGE STAGE_PROFILEUPDATES2 IS
  TYPE RCURSOR IS REF CURSOR;
  FUNCTION Getexpirationdate(p_memberdate DATE,
                             p_processdate DATE) RETURN DATE;

  /*PROCEDURE load_tables( P_FILENAME IN VARCHAR2,
                                   RETVAL     IN OUT RCURSOR );
*/
  PROCEDURE load_exceptions(RETVAL     IN OUT RCURSOR );

  PROCEDURE get_mergelist(RETVAL     IN OUT RCURSOR );

  PROCEDURE ctas_member_details;

  PROCEDURE ctas_virtual_card;

  PROCEDURE ctas_loyalty_member;

  PROCEDURE ctas_ats_synkey;

  PROCEDURE Updatetieronly(p_Processdate        TIMESTAMP);

  procedure drop_constraint(p_table varchar2, p_constraint varchar2);

  PROCEDURE update_ats_mergehistory  (RETVAL     IN OUT RCURSOR);

END STAGE_PROFILEUPDATES2;
/
CREATE OR REPLACE PACKAGE BODY STAGE_PROFILEUPDATES2 IS


 FUNCTION Getexpirationdate( p_memberdate DATE,
                             p_processdate DATE     ) RETURN DATE IS

       Lv_Initialyear NUMBER := Extract(YEAR FROM Nvl(p_memberdate, SYSDATE));
       Lv_Endingyear  NUMBER := Extract(YEAR FROM Nvl(p_processdate, SYSDATE));
     --  Lv_nominationYear NUMBER := Extract(YEAR FROM Nvl(p_processdate, SYSDATE));
       Lv_resultValue DATE := SYSDATE;
       lv_elapsedyears NUMBER := 0;

  BEGIN
       lv_elapsedyears  := (lv_endingyear - Lv_initialyear);

       CASE
           WHEN (lv_elapsedyears < 3 AND lv_elapsedyears >=0) THEN
              Lv_resultValue := to_date('12/31/'|| (Lv_Endingyear+1),'mm/dd/yyyy')+1;
           WHEN (lv_elapsedyears < 0 ) THEN
              Lv_resultValue := to_date('12/31/2199','mm/dd/yyyy');
           ELSE
              Lv_resultValue :=   to_date('12/31/'|| Lv_Endingyear,'mm/dd/yyyy')+1;
       END CASE;

       RETURN(Lv_resultValue);
  END Getexpirationdate;


PROCEDURE ctas_profileupdates2 is

BEGIN


begin
  execute immediate 'drop  table bp_Ae.ae_profileupdates2_new ';
exception when others then
  null;
end;

--Create tablename_New
EXECUTE IMMEDIATE
'Create table bp_ae.ae_profileupdates2_new AS
 SELECT pu.cid,
       pu.firstname,
       pu.lastname,
       pu.addressline1,
       pu.addressline2,
       pu.city,
       pu.state,
       pu.zip,
       pu.country,
       CASE WHEN br.loyaltynumber IS NOT NULL THEN
           CASE
             WHEN br.birthdatefi IS NOT NULL THEN
              to_char(br.Birthdatefi,''MMDDYYYY'')
             ELSE
                  to_char(br.birthdatedb,''MMDDYYYY'')
            END
         ELSE
           to_char (pu.birthdate )
         END AS birthdate ,
       pu.phone,
       pu.gender,
       pu.loyaltynumber,
       pu.emailaddress,
       pu.emailflag,
       pu.mailflag,
       pu.comfortcode,
       CASE  WHEN mu.floyaltynumber IS NOT NULL THEN
            CASE WHEN hp.loyaltynumber IS NOT NULL THEN
               to_char(hp.newrecordtype)
              ELSE
                to_char(mu.newrecordtype)
            END
       ELSE
          CASE WHEN hp.loyaltynumber IS NOT NULL THEN
               to_char(hp.newrecordtype)
              ELSE
                to_char(pu.recordtype)
            END
       END AS recordtype,
       pu.emailvalidation,
       pu.cellvalidation,
       pu.accountkey,
       pu.updatedate
FROM bp_Ae.Ae_Profileupdates2 pu
INNER JOIN bp_Ae.Lw_Virtualcard vc ON vc.loyaltyidnumber = pu.loyaltynumber
LEFT JOIN bp_ae.ae_tmpvcbirthdate br ON br.loyaltynumber = vc.loyaltyidnumber
LEFT JOIN bp_ae.AE_MEBERSTOUPDATE mu ON mu.floyaltynumber = pu.loyaltynumber
LEFT JOIN bp_Ae.Ae_Hubsynchelpertbl hp ON hp.loyaltynumber = pu.loyaltynumber';
--Table rename to old




    BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.ae_profileupdates2_old PURGE';

    EXCEPTION

        WHEN OTHERS THEN
            IF sqlcode = -942 THEN
                NULL;
            ELSE
               RAISE;
            END IF;
    END;


EXECUTE IMMEDIATE 'Alter table bp_ae.ae_profileupdates2  rename to ae_profileupdates2_old';



--Drop indexes
    EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPDATECID_IX';
    EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPDATELID_IX';
    EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPD_LOYID';
    EXECUTE IMMEDIATE 'drop INDEX IX_LOYALTYNUMBER_TYPE';



    EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX AE_ProfUpdateLID_ix   ON Ae_Profileupdates2_new( SYS_OP_C2C("LOYALTYNUMBER") ) parallel COMPUTE STATISTICS';
    EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX AE_PROFUPD_LOYID   ON Ae_Profileupdates2_new(LOYALTYNUMBER) parallel COMPUTE STATISTICS';
    EXECUTE IMMEDIATE 'CREATE        INDEX ix_loyaltynumber_type ON AE_PROFILEUPDATES2_new(LOYALTYNUMBER, RECORDTYPE) parallel COMPUTE STATISTICS' ;
    EXECUTE IMMEDIATE 'CREATE        INDEX AE_ProfUpdateCID_ix   ON Ae_Profileupdates2_new( cid ) parallel COMPUTE STATISTICS';


--Rename Tablename_New
EXECUTE IMMEDIATE 'Alter Table  bp_ae.ae_profileupdates2_new  Rename  to ae_profileupdates2';


END;


PROCEDURE CHANGEEXTERNALTABLE(P_FILENAME IN VARCHAR2) IS
    E_MTABLE    EXCEPTION;
    E_MFILENAME EXCEPTION;
    V_SQL VARCHAR2(400);
  BEGIN

    IF LENGTH(TRIM(P_FILENAME)) = 0 OR P_FILENAME IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001,
                              'Filename is required to link with external table',
                              FALSE);
    END IF;

    V_SQL := 'ALTER TABLE EXT_PROFILEUPDATES2 LOCATION (AE_IN' || CHR(58) || '''' ||
             P_FILENAME || ''')';
    EXECUTE IMMEDIATE V_SQL;

  END CHANGEEXTERNALTABLE;



  PROCEDURE load_exceptions (RETVAL     IN OUT RCURSOR ) IS

      CURSOR GET_DATA IS
           SELECT  p2.loyaltynumber
             FROM AE_PROFILEUPDATES2 p2
             LEFT JOIN lw_virtualcard vc ON vc.loyaltyidnumber = p2.loyaltynumber
           WHERE vc.loyaltyidnumber IS NULL;

      CURSOR GET_DATA2 IS
      select /*+index(vc) no_parallel*/p.loyaltynumber,
              sysdate as procdate,
             'Members Merged in BP Db' AS TEXT
        from bp_ae.ae_profileupdates2 p,
             bp_ae.lw_virtualcard vc,
        (
         select b.loyaltynumber, b.cid, b.ipcode from
               (select /*+use_hash(p2, vc)*/
                       p2.loyaltynumber,
                       p2.cid,
                       vc.ipcode
                  from bp_ae.ae_profileupdates2 p2,
                       bp_ae.lw_virtualcard vc
                 where p2.recordtype = 'P'
                   and vc.loyaltyidnumber = p2.loyaltynumber) b,
               (SELECT /*+use_hash(p3, vc1)*/VC1.IPCODE
                  FROM bp_ae.AE_PROFILEUPDATES2 P3,
                       bp_ae.LW_VIRTUALCARD VC1
                 WHERE VC1.LOYALTYIDNUMBER = P3.LOYALTYNUMBER
                   AND P3.RECORDTYPE = 'P'
              GROUP BY VC1.IPCODE HAVING COUNT(VC1.IPCODE) > 1)c
            where b.ipcode = c.ipcode) a
         where vc.loyaltyidnumber = p.loyaltynumber
           and a.loyaltynumber = p.loyaltynumber
           and p.cid = a.cid
           and vc.ipcode = a.ipcode;


    TYPE T_TAB2 IS TABLE OF GET_DATA2%ROWTYPE;
    V_TBL2 T_TAB2;

    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;
      V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    P_FILENAME VARCHAR2(50) := 'STAGE_PROFILEUPDATES2';
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

    BEGIN
     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'LoadExceptions';
     V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

     V_ERROR          :=  ' ';
     V_REASON         := 'write exceptions 2 begin';
     V_MESSAGE        := 'write exceptions 2 begin';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 10000;

          FORALL I IN 1 .. V_TBL.COUNT SAVE EXCEPTIONS

                INSERT INTO ae_ProfileUpdateException
                     ( LoyaltyID  , processdate, notes)
                VALUES
                     (v_Tbl(i).loyaltynumber, SYSDATE,'Not found  LW_virtualcard');


          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

    V_ERROR          :=  ' ';
    V_REASON         := 'write exceptions 2 end';
    V_MESSAGE        := 'write exceptions 2 end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


    V_ERROR          :=  ' ';
    V_REASON         := 'write exceptions 1 begin';
    V_MESSAGE        := 'write exception 1 begin';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
      OPEN GET_DATA2;
      LOOP
        FETCH GET_DATA2 BULK COLLECT
          INTO V_TBL2 LIMIT 10000;

          FOR I IN 1 .. V_TBL2.COUNT LOOP
              INSERT INTO Ae_Profileupdateexception
                   (Loyaltyid, Processdate, Notes)
              VALUES
                   (v_Tbl2(i).Loyaltynumber, v_Tbl2(i).Procdate, v_Tbl2(i).Text);


          END LOOP;
          COMMIT;

          EXIT WHEN GET_DATA2%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA2%ISOPEN THEN
        CLOSE GET_DATA2;
      END IF;

      V_ERROR          :=  ' ';
      V_REASON         := 'write exceptions 1 end';
      V_MESSAGE        := 'write exception 1 end';
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

    OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;

  END load_exceptions;




PROCEDURE ctas_ats_synkey IS

  V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    P_FILENAME VARCHAR2(50) := 'STAGE_PROFILEUPDATES2';

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

BEGIN


     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'CTAS_ATS_SYNKEY';
     V_LOGSOURCE := V_JOBNAME;


     UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);


      V_ERROR          :=  ' ';
      V_REASON         := 'CTAS_ATS_SYNKEY start';
      V_MESSAGE        := 'CTAS_ATS_SYNKEY start';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

begin
  execute immediate 'drop  table bp_Ae.ATS_SYNCHRONYACCOUNTKEY_new';
exception when others then
  null;
end;

--Create tablename_New
EXECUTE IMMEDIATE
'Create table bp_ae.ATS_SYNCHRONYACCOUNTKEY_NEW AS
  SELECT /*+use_hash(sya,vc)*/
      sya.a_rowkey AS a_rowkey,
      sya.a_vckey  AS a_vckey,
      sya.a_parentrowkey AS a_parentrowkey,
       CASE
         WHEN pus.accountkey IS NOT NULL THEN
          to_char(pus.accountkey)
      ELSE
         to_char(sya.a_accountkey)
      END  AS a_accountkey,
      sya.statuscode     AS statuscode,
      sya.createdate     AS createdate,
      SYSDATE            AS updatedate,
      sya.lastdmlid      AS lastdmlid
  FROM  bp_Ae.ATS_SYNCHRONYACCOUNTKEY SYA
  INNER JOIN bp_Ae.LW_VIRTUALCARD VC            ON VC.VCKEY = SYA.A_VCKEY --AND VC.ISPRIMARY = 1
  LEFT JOIN  bp_Ae.LW_PROFILEUPDATES_STAGE PUS  ON VC.IPCODE = PUS.IPCODE';

  EXECUTE IMMEDIATE
  'INSERT INTO bp_ae.ats_synchronyaccountkey_new
( a_rowkey, a_vckey, a_parentrowkey , a_accountkey, statuscode, createdate, updatedate, lastdmlid)
  SELECT /*+use_hash(pus,vc) parallel(pus, 4) parallel(vc, 4) */
   SEQ_ROWKEY.NEXTVAL  AS a_rowkey,
   vc.vckey as a_vckey,
   vc.vckey as a_parentrowkey,
   to_char(pus.accountkey) a_accountkey,
   0 as a_statuscode,
   SYSDATE as a_createdate,
   sysdate as updatedate,
   0 as lastdmlid
  FROM bp_Ae.LW_PROFILEUPDATES_STAGE PUS
  INNER JOIN bp_Ae.Lw_Virtualcard vc ON vc.ipcode = pus.ipcode
  LEFT JOIN  bp_ae.ats_synchronyaccountkey syn ON syn.a_vckey = vc.vckey
  WHERE syn.a_vckey IS NULL';

--Table rename to old




    BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.ATS_SYNCHRONYACCOUNTKEY_old PURGE';

    EXCEPTION

        WHEN OTHERS THEN
            IF sqlcode = -942 THEN
                NULL;
            ELSE
               RAISE;
            END IF;
    END;

EXECUTE IMMEDIATE 'Alter table bp_ae.ATS_SYNCHRONYACCOUNTKEY  rename to ATS_SYNCHRONYACCOUNTKEY_old';

EXECUTE IMMEDIATE 'Drop Index BP_AE.IDX_ATS2857290383_PRK';
EXECUTE IMMEDIATE 'Drop Index BP_AE.IDX_SYNCHRONYACCOUNTKEY_VCKEY';
--EXECUTE IMMEDIATE 'Drop Index BP_AE.SYS_C00608433';


EXECUTE IMMEDIATE ' create index BP_AE.IDX_ATS2857290383_PRK ON BP_AE.ATS_SYNCHRONYACCOUNTKEY_new (A_PARENTROWKEY)
  PCTFREE 10
  INITRANS 2
  MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(
           INITIAL 65536
           NEXT 1048576
           MINEXTENTS 1
           MAXEXTENTS 2147483645
           PCTINCREASE 0
           FREELISTS 1
           FREELIST GROUPS 1
           BUFFER_POOL DEFAULT
           FLASH_CACHE DEFAULT
           CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D ';

  EXECUTE IMMEDIATE ' CREATE INDEX BP_AE.IDX_SYNCHRONYACCOUNTKEY_VCKEY ON BP_AE.ATS_SYNCHRONYACCOUNTKEY_new (A_VCKEY)
  PCTFREE 10
  INITRANS 2
  MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(
           INITIAL 65536
           NEXT 1048576
           MINEXTENTS 1
           MAXEXTENTS 2147483645
           PCTINCREASE 0
           FREELISTS 1
           FREELIST GROUPS 1
           BUFFER_POOL DEFAULT
           FLASH_CACHE DEFAULT
           CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D';

/*  EXECUTE IMMEDIATE ' CREATE INDEX 0BP_AE.SYS_C00608433 ON BP_AE.ATS_SYNCHRONYACCOUNTKEY_new ("A_ROWKEY")
  PCTFREE 10
  INITRANS 2
  MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(
           INITIAL 65536
           NEXT 1048576
           MINEXTENTS 1
           MAXEXTENTS 2147483645
           PCTINCREASE 0
           FREELISTS 1
           FREELIST GROUPS 1
           BUFFER_POOL DEFAULT
           FLASH_CACHE DEFAULT
           CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D';*/

  EXECUTE IMMEDIATE 'ALTER TABLE BP_AE.ATS_SYNCHRONYACCOUNTKEY_new ADD PRIMARY KEY (A_ROWKEY)';
--  EXECUTE IMMEDIATE 'ALTER TABLE BP_AE.ATS_SYNCHRONYACCOUNTKEY_new MODIFY (CREATEDATE NOT NULL ENABLE)';
--  EXECUTE IMMEDIATE 'ALTER TABLE BP_AE.ATS_SYNCHRONYACCOUNTKEY_new MODIFY (A_VCKEY NOT NULL ENABLE)';
--  EXECUTE IMMEDIATE 'ALTER TABLE BP_AE.ATS_SYNCHRONYACCOUNTKEY_new MODIFY (A_ROWKEY NOT NULL ENABLE)';
  EXECUTE IMMEDIATE 'GRANT SELECT ON BP_AE.ATS_SYNCHRONYACCOUNTKEY_new TO "ETLAE"';
  EXECUTE IMMEDIATE 'GRANT SELECT ON BP_AE.ATS_SYNCHRONYACCOUNTKEY_new TO "BP_AE_READONLY_R"';



--Rename Tablename_New
EXECUTE IMMEDIATE 'Alter Table  bp_ae.ATS_SYNCHRONYACCOUNTKEY_new  Rename  to ATS_SYNCHRONYACCOUNTKEY';


    V_ERROR          :=  ' ';
    V_REASON         := 'CTAS_ATS_SYNKEY end';
    V_MESSAGE        := 'CTAS_ATS_SYNKEY end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
END;

PROCEDURE get_mergelist (RETVAL     IN OUT RCURSOR ) IS
     V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    P_FILENAME VARCHAR2 (50):= 'STAGE_PROFILEUPDATES2';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);

    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;
    lv_secondaryipcode NUMBER := 0;
    lv_isprimarycredit BOOLEAN := FALSE;
    lv_issecondarycredit BOOLEAN := FALSE;
    lv_isprimarypilot BOOLEAN := FALSE;
    lv_issecondarypilot BOOLEAN := FALSE;
    lv_primaryDateIssued DATE := NULL;
    lv_secondaryDateIssued DATE := NULL;
    lv_salesamountprimary NUMBER := 0;
    lv_saleamountsecondry NUMBER := 0;

    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

      CURSOR GET_DATA IS
             SELECT
                  a.cid,
                  a.primary,
                  a.secondary,
                  v1.loyaltyidnumber as vc_primary,
                  v1.vckey           AS vckey_primary,
                  nvl(md1.a_extendedplaycode,0) AS primary_ext,
                  nvl(md1.a_cardtype,0) AS primary_ctype,
                  md1.a_ipcode AS primary_ipcode,
                  v1.dateissued AS  primary_date,
                  v2.loyaltyidnumber as vc_secondary,
                  v2.vckey           AS vckey_secondary,
                  nvl(a.a_extendedplaycode,0) AS  secondary_ext,
                  nvl(a.a_cardtype,0) AS  secondary_ctype,
                  v2.dateissued AS secondary_date,
                  a.a_ipcode AS secondary_ipcode,
                  a.primarystatus,
                  a.secondarystatus
             FROM
                           (SELECT P2.Cid,
                                   P2.Loyaltynumber     AS Primary,
                                   w.Loyaltynumber      AS Secondary,
                                   lm2.memberstatus    AS primarystatus,
                                   w.secondarystatus   AS secondarystatus,
                                   w.a_Extendedplaycode,
                                   w.a_cardtype,
                                   w.a_ipcode
                            FROM   Bp_Ae.Ae_Profileupdates2 P2,
                                   Bp_Ae.Lw_Virtualcard Vc,
                                   bp_ae.lw_loyaltymember lm2,
                                    (  SELECT p.Cid,
                                             p.Recordtype,
                                             p.Loyaltynumber,
                                             Md.a_Extendedplaycode,
                                             Lm.Memberstatus AS secondarystatus,
                                             md.a_cardtype,
                                             md.a_ipcode
                                      FROM   Bp_Ae.Ae_Profileupdates2 p,
                                             Bp_Ae.Lw_Virtualcard     Vc,
                                             Bp_Ae.Ats_Memberdetails  Md,
                                             BP_ae.Lw_Loyaltymember   Lm
                                      WHERE  Vc.Loyaltyidnumber = p.Loyaltynumber
                                             AND lm.ipcode = vc.ipcode
                                             AND Md.a_Ipcode = Vc.Ipcode
                                             AND p.Recordtype = 'S') w   ,

                                (  SELECT Ex.Loyaltyid
                                   FROM   Bp_Ae.Ae_Profileupdateexception Ex
                                   WHERE  Ex.Notes = 'Members Merged in BP Db')e
                            WHERE  vc.loyaltyidnumber = p2.loyaltynumber
                                   AND lm2.ipcode = vc.ipcode
                                   and  w.cid = p2.cid
                                   and p2.recordtype = 'P'
                                   and p2.loyaltynumber = e.loyaltyid(+)
                                   and e.loyaltyid is null) a,
                                      bp_ae.lw_virtualcard v1,
                                      bp_ae.lw_virtualcard v2,
                                      bp_ae.ats_memberdetails md1
              where a.primary = v1.loyaltyidnumber(+)
                     and a.secondary = v2.loyaltyidnumber(+)
                     AND md1.a_ipcode = v1.ipcode
                      and v1.loyaltyidnumber is not null
                       and v2.loyaltyidnumber is not null
                      and v1.loyaltyidnumber != v2.loyaltyidnumber
                      AND v1.ipcode <> v2.ipcode;




      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;


       CURSOR GET_DATA2 IS
          SELECT a_Vckey, SUM(Th.a_Txnqualpurchaseamt) AS Purchaseamt
          FROM   Bp_Ae.Ats_Txnheader Th
          WHERE  Th.a_Txndate BETWEEN To_Date('01-jan-2016', 'dd-mon-yyyy') AND
                 To_Date('31-dec-2016', 'dd-mon-yyyy')
          GROUP  BY a_Vckey;

      TYPE T_TAB2 IS TABLE OF GET_DATA2%ROWTYPE;
      V_TBL2 T_TAB2;

  BEGIN
     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'get_mergelist';
     V_LOGSOURCE := V_JOBNAME;


     UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);
    V_ERROR          :=  ' ';
    V_REASON         := 'write Ats_Profileupdatemerge begin';
    V_MESSAGE        := 'write Ats_Profileupdatemerge begin';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


/*
      OPEN GET_DATA2;
      LOOP
        FETCH GET_DATA2 BULK COLLECT  INTO V_TBL2 LIMIT 10000;
        FORALL I IN 1 .. V_TBL2.COUNT
              INSERT INTO Bp_Ae.Ae_Hubsynpointsvckey Pk
                   (Vckey, Points)
              VALUES
                   (v_Tbl2(i).a_Vckey, v_Tbl2(i).Purchaseamt);
         COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN GET_DATA2%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA2%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA2;
      END IF;
*/

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 10000;

          FOR I IN 1 .. V_TBL.COUNT LOOP


            -- detect exception when the accounts should not be merged
            --
            lv_isprimarycredit     := v_tbl(i).primary_ctype > 0;
            lv_issecondarycredit   := v_tbl(i).secondary_ctype > 0;
            lv_isprimarypilot      := v_tbl(i).primary_ext IN (1,3);
            lv_issecondarypilot    := v_tbl(i).secondary_ext IN (1,3);
            lv_primaryDateIssued   := v_tbl(i).primary_date;
            lv_secondaryDateIssued := v_tbl(i).secondary_date;

            -- if both are credit card but not for the same loyalty programa then
            -- add an excepetion and do not include in the merge list

            IF ( lv_isprimarycredit AND lv_issecondarycredit AND
              ( lv_issecondarypilot <> lv_isprimarypilot) ) THEN

                 INSERT INTO ae_ProfileUpdateException pe
                        ( LoyaltyID  , processdate, notes)
                 VALUES
                     (v_Tbl(i).vc_secondary,
                      SYSDATE,
                      'Account can''t be merged with '||
                      v_Tbl(i).vc_primary ||
                      '. Different credit card loyalty programs');


            ELSE
              -- add to the merge list
                 INSERT INTO Ats_Profileupdatemerge
                     (a_Rowkey,
                      a_Ipcode,
                      a_Parentrowkey,
                      a_Fromloyaltyid,
                      a_Toloyaltyid,
                      Statuscode,
                      Createdate,
                      Updatedate,
                      Lwidentifier,
                      Lastdmlid,
                      a_Cid,
                      Last_Dml_Id,
                      a_status)
                VALUES
                     (Seq_Rowkey.Nextval,
                      Lv_Secondaryipcode,
                      -1,
                      v_Tbl(i).Secondary,
                      v_Tbl(i).Primary,
                      0,
                      SYSDATE ,
                      SYSDATE ,
                      0,
                      0,
                      v_Tbl(i).Cid,
                      Last_Dml_Id#.Nextval,
                      0);

                IF ( v_tbl(i).primarystatus = 1 AND v_tbl(i).secondarystatus <> 1 )THEN

                          INSERT INTO Ae_Profileupdateexception
                               (Loyaltyid, Processdate, Notes)
                          VALUES
                               (v_Tbl(i).Vc_Secondary,
                                SYSDATE,
                                'The account is not active and will be merged with ' || v_Tbl(i)
                                .Vc_Primary);

                END IF;

                -- now check if we need to change the loyalty program fields
                IF ( lv_isprimarycredit AND lv_issecondarycredit) THEN

                   IF (lv_isprimarypilot) THEN

                          IF (NOT lv_issecondarypilot) THEN

                              -- change the secondary loyalty program to pilot
                             /* UPDATE ats_memberdetails md
                              SET md.a_extendedplaycode = 3,
                                  md.a_programchangedate = SYSDATE + 1
                               WHERE md.a_ipcode =V_TBL(i).secondary_ipcode;*/
                               INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                    (Ipcode,
                                     Extendedplaycode,
                                     Programchangedate)
                               VALUES
                                    (v_Tbl(i).Secondary_Ipcode, 3, SYSDATE + 1);

                          END IF  ;

                   ELSE
                         IF (  lv_issecondarypilot) THEN

                             -- change the primary loyalty program to legacy
                             /* UPDATE ats_memberdetails md
                              SET md.a_extendedplaycode = 3,
                                  md.a_programchangedate = SYSDATE + 1
                               WHERE md.a_ipcode =V_TBL(i).primary_ipcode;*/

                               INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                    (Ipcode,
                                     Extendedplaycode,
                                     Programchangedate)
                               VALUES
                                    (V_TBL(i).primary_ipcode, 3, SYSDATE + 1);

                         END IF;
                   END IF;

                ELSE

                    IF (lv_isprimarycredit) THEN
                     -- chnage the program to the program specified by the primary
                       IF (lv_isprimarypilot <> lv_issecondarypilot) THEN

                             /* UPDATE ats_memberdetails md
                              SET md.a_extendedplaycode = CASE WHEN v_tbl(i).primary_ext IN (1,3)  THEN
                                                              3
                                                          ELSE
                                                            2
                                                          END ,
                                  md.a_programchangedate = SYSDATE + 1
                               WHERE md.a_ipcode =V_TBL(i).secondary_ipcode;*/

                               INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                    (Ipcode,
                                     Extendedplaycode,
                                     Programchangedate)
                               VALUES
                                    (v_Tbl(i).Secondary_Ipcode,
                                     CASE WHEN v_Tbl(i).Primary_Ext IN (1, 3) THEN 3 ELSE 2 END,
                                     SYSDATE + 1);

                       END IF;

                    ELSE

                        IF (lv_issecondarycredit) THEN
                          -- chnage the program to the program specified by the secondary
                          IF (lv_isprimarypilot <> lv_issecondarypilot) THEN

                            /* UPDATE ats_memberdetails md
                              SET md.a_extendedplaycode = CASE WHEN  v_tbl(i).secondary_ext IN (1,3)THEN
                                                              3
                                                          ELSE
                                                            2
                                                          END ,
                                  md.a_programchangedate = SYSDATE + 1
                               WHERE md.a_ipcode =V_TBL(i).primary_ipcode;*/

                               INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                    (Ipcode,
                                     Extendedplaycode,
                                     Programchangedate)
                               VALUES
                                    (v_Tbl(i).Primary_Ipcode,
                                     CASE WHEN v_Tbl(i).Secondary_Ext IN (1, 3) THEN 3 ELSE 2 END,
                                     SYSDATE + 1);

                          END IF;

                        ELSE
                             -- if none of the account is credit card

                             -- get the total amount purchased for each account

                             SELECT nvl(SUM(th.points),0)
                             INTO lv_salesamountprimary
                                  FROM bp_Ae.Ae_Hubsynpointsvckey th
                                  WHERE th.vckey = V_TBL(i).vckey_primary;



                             SELECT nvl(SUM(th.points),0)
                             INTO lv_saleamountsecondry
                                  FROM bp_Ae.Ae_Hubsynpointsvckey th
                                  WHERE th.vckey = V_TBL(i).vckey_secondary;

                             IF ( lv_saleamountsecondry = 0 AND lv_salesamountprimary = 0 ) THEN
                                          -- if no sales for any loyalty program then
                                          -- we will move account to pilto program if needed

                                          IF ( lv_isprimarypilot OR lv_issecondarypilot) THEN

                                             IF ( NOT lv_isprimarypilot ) THEN

                                             /* UPDATE ats_memberdetails md
                                                SET md.a_extendedplaycode = 3,
                                                  md.a_programchangedate = SYSDATE + 1
                                               WHERE md.a_ipcode =V_TBL(i).primary_ipcode;*/

                                               INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                                    (Ipcode,
                                                     Extendedplaycode,
                                                     Programchangedate)
                                               VALUES
                                                    (v_Tbl(i).Primary_Ipcode,
                                                     3,
                                                     SYSDATE + 1);

                                            END IF;

                                             IF ( NOT lv_issecondarypilot ) THEN

                                            /*  UPDATE ats_memberdetails md
                                                SET md.a_extendedplaycode = 3,
                                                  md.a_programchangedate = SYSDATE + 1
                                               WHERE md.a_ipcode =V_TBL(i).secondary_ipcode;*/

                                               INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                                    (Ipcode,
                                                     Extendedplaycode,
                                                     Programchangedate)
                                               VALUES
                                                    (v_Tbl(i).secondary_ipcode,
                                                     3,
                                                     SYSDATE + 1);

                                            END IF;

                                          END IF;

                            ELSE

                                    IF ( lv_salesamountprimary > lv_saleamountsecondry) THEN

                                       IF ( lv_isprimarypilot <> lv_issecondarypilot) THEN
                                               /* UPDATE ats_memberdetails md
                                                 SET md.a_extendedplaycode = CASE WHEN v_tbl(i).primary_ext IN (1,3)  THEN
                                                                  3
                                                              ELSE
                                                                2
                                                              END ,
                                                   md.a_programchangedate = SYSDATE + 1
                                                 WHERE md.a_ipcode =V_TBL(i).secondary_ipcode;*/



                                             INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                                  (Ipcode,
                                                   Extendedplaycode,
                                                   Programchangedate)
                                             VALUES
                                                  (v_Tbl(i).secondary_ipcode,
                                                   CASE WHEN v_tbl(i).primary_ext IN (1,3)  THEN
                                                                  3
                                                              ELSE
                                                                2
                                                              END,
                                                   SYSDATE + 1);
                                       END IF;

                                    ELSE

                                        IF ( lv_isprimarypilot <> lv_issecondarypilot) THEN
                                         /* UPDATE ats_memberdetails md
                                                 SET md.a_extendedplaycode = CASE WHEN v_tbl(i).primary_ext IN (1,3)  THEN
                                                                  3
                                                              ELSE
                                                                2
                                                              END ,
                                                   md.a_programchangedate = SYSDATE + 1
                                         WHERE md.a_ipcode =V_TBL(i).secondary_ipcode;*/

                                          INSERT INTO Bp_Ae.Ae_Hubsyncchangeprogram
                                               (Ipcode,
                                                Extendedplaycode,
                                                Programchangedate)
                                          VALUES
                                               (v_Tbl(i).primary_ipcode,
                                                CASE WHEN v_Tbl(i)
                                                .secondary_ext IN (1, 3) THEN 3 ELSE 2 END,
                                                SYSDATE + 1);
                                        END IF ;


                                    END IF;

                            END IF;


                        END IF;
                    END IF;

                END IF;

            END IF ;

          END LOOP;


          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

      V_ERROR          :=  ' ';
      V_REASON         := 'write Ats_Profileupdatemerge end';
      V_MESSAGE        := 'write Ats_Profileupdatemerge end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;
  END get_mergelist;



/*
  PROCEDURE get_mergelist (RETVAL     IN OUT RCURSOR ) IS
     V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    P_FILENAME VARCHAR2 (50):= 'STAGE_PROFILEUPDATES2';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);

    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;
    lv_primaryipcode NUMBER := 0;
    lv_secondaryipcode NUMBER := 0;

    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

      CURSOR GET_DATA IS
           SELECT
                  a.cid,
                  a.primary,
                  a.secondary,
                  v1.loyaltyidnumber as vc_primary,
                  v2.loyaltyidnumber as vc_secondary,
                  a.primarystatus,
                  a.secondarystatus
             FROM
                           (SELECT P2.Cid,
                                   P2.Loyaltynumber     AS Primary,
                                   w.Loyaltynumber      AS Secondary,
                                   lm2.memberstatus    AS primarystatus,
                                   w.secondarystatus   AS secondarystatus,
                                   w.a_Extendedplaycode
                            FROM   Bp_Ae.Ae_Profileupdates2 P2,
                                   Bp_Ae.Lw_Virtualcard Vc,
                                   bp_ae.lw_loyaltymember lm2,
                                    (  SELECT p.Cid,
                                             p.Recordtype,
                                             p.Loyaltynumber,
                                             Md.a_Extendedplaycode,
                                             Lm.Memberstatus AS secondarystatus
                                      FROM   Bp_Ae.Ae_Profileupdates2 p,
                                             Bp_Ae.Lw_Virtualcard     Vc,
                                             Bp_Ae.Ats_Memberdetails  Md,
                                             BP_ae.Lw_Loyaltymember   Lm
                                      WHERE  Vc.Loyaltyidnumber = p.Loyaltynumber
                                             AND lm.ipcode = vc.ipcode
                                             AND Md.a_Ipcode = Vc.Ipcode
                                             AND p.Recordtype = 'S') w   ,

                                (  SELECT Ex.Loyaltyid
                                   FROM   Bp_Ae.Ae_Profileupdateexception Ex
                                   WHERE  Ex.Notes = 'Members Merged in BP Db')e
                            WHERE  vc.loyaltyidnumber = p2.loyaltynumber
                                   AND lm2.ipcode = vc.ipcode
                                   and  w.cid = p2.cid
                                   and p2.recordtype = 'P'
                                   and p2.loyaltynumber = e.loyaltyid(+)
                                   and e.loyaltyid is null) a,
                                      bp_ae.lw_virtualcard v1,
                                      bp_ae.lw_virtualcard v2,
                                      bp_ae.ats_memberdetails md1
              where a.primary = v1.loyaltyidnumber(+)
                     and a.secondary = v2.loyaltyidnumber(+)
                     AND md1.a_ipcode = v1.ipcode
                      AND decode( md1.a_extendedplaycode,1,1,3,1,0 ) =
                          decode( a.a_extendedplaycode,1,1,3,1,0)
                      and v1.loyaltyidnumber is not null
                       and v2.loyaltyidnumber is not null
                      and v1.loyaltyidnumber != v2.loyaltyidnumber
                      AND v1.ipcode <> v2.ipcode;



      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

  BEGIN
     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'get_mergelist';
     V_LOGSOURCE := V_JOBNAME;


     UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);
    V_ERROR          :=  ' ';
    V_REASON         := 'write Ats_Profileupdatemerge begin';
    V_MESSAGE        := 'write Ats_Profileupdatemerge begin';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 10000;

          FORALL I IN 1 .. V_TBL.COUNT
                INSERT INTO Ats_Profileupdatemerge
                     (a_Rowkey,
                      a_Ipcode,
                      a_Parentrowkey,
                      a_Fromloyaltyid,
                      a_Toloyaltyid,
                      Statuscode,
                      Createdate,
                      Updatedate,
                      Lwidentifier,
                      Lastdmlid,
                      a_Cid,
                      Last_Dml_Id,
                      a_status)
                VALUES
                     (Seq_Rowkey.Nextval,
                      Lv_Secondaryipcode,
                      -1,
                      v_Tbl(i).Secondary,
                      v_Tbl(i).Primary,
                      0,
                      SYSDATE ,
                      SYSDATE ,
                      0,
                      0,
                      v_Tbl(i).Cid,
                      Last_Dml_Id#.Nextval,
                      0);





          FOR I IN 1 .. V_TBL.COUNT LOOP
            IF ( v_tbl(i).primarystatus = 1 AND v_tbl(i).secondarystatus <> 1 )THEN

                 INSERT INTO Ae_Profileupdateexception
                   (Loyaltyid, Processdate, Notes)
                VALUES
                   (v_Tbl(i).vc_secondary,SYSDATE,
                    'The account is not active and will be merged with '||
                     v_Tbl(i).vc_primary );

            END IF;


          END LOOP;


          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

    V_ERROR          :=  ' ';
      V_REASON         := 'write Ats_Profileupdatemerge end';
      V_MESSAGE        := 'write Ats_Profileupdatemerge end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;
  END get_mergelist;

*/
  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
PROCEDURE ctas_member_details IS
  V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    --V_FILENAME  VARCHAR2(256) := '';

    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'STAGE_PROFILEUPDATES2';

    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

BEGIN
      V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
      V_JOBNAME   := 'ctas_member_details';
      V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);


     V_REASON := 'ctas_member_details start ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
    BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.ATS_MEMBERDETAILS_new PURGE';

    EXCEPTION

        WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'DROP NEW TABLE ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
    END;



EXECUTE IMMEDIATE '
Create table bp_ae.ats_memberdetails_new AS
select
    md.A_ROWKEY ,
    md.A_IPCODE,
    md.A_PARENTROWKEY,
    md.A_CARDTYPE,
    md.A_CHANGEDBY,
    md.A_PASSVALIDATION,
    md.A_BASEBRANDID,
    md.A_OLDCELLNUMBER,
     case when (hs.ipcode IS not NULL) then
      to_number(hs.extendedplaycode)
    else
      md.A_EXTENDEDPLAYCODE
    end as A_EXTENDEDPLAYCODE,
    md.A_EMAILADDRESSMAILABLE,
    case
             when ( pu.ADDRESSLINE1 IS NOT NULL   AND
                  (NVL(TRIM(md.a_ADDRESSLINEONE), ''x'') != pu.ADDRESSLINE1 ))  then
                       to_char(pu.ADDRESSLINE1)
       else
                        to_char(md.A_ADDRESSLINEone)
    end as  A_ADDRESSLINEONE,
    case
            when ( (NVL(TRIM(md.a_ADDRESSLINETWO),''x'') != pu.ADDRESSLINE2 OR pu.addressline2 IS NULL  ))  then
                       to_char(pu.ADDRESSLINE2)

            else
                       to_char(md.a_ADDRESSLINETWO)

    end as a_ADDRESSLINETWO,
    md.A_ADDRESSLINETHREE,
    md.A_ADDRESSLINEFOUR,
    case
        when ( pu.CITY IS NOT NULL   AND
                 (NVL(TRIM(md.a_CITY),''x'') != pu.CITY ))  then
           to_char(pu.city)
        else
           to_char(md.A_city)
    end  as A_city,
    case
        when ( pu.state IS NOT NULL   AND
                 (NVL(TRIM(md.a_STATEORPROVINCE), ''x'') != pu.state ))  then
             to_char(pu.state)
        else
             to_char(md.a_STATEORPROVINCE)
    end as a_STATEORPROVINCE,
    case
        when ( pu.ZIP IS NOT NULL and
                NVL(TRIM(md.a_ZIPORPOSTALCODE), ''x'') != pu.ZIP ) then
           to_char(pu.zip)
    else
     to_char(md.A_ZIPORPOSTALCODE)
    end as A_ZIPORPOSTALCODE,
    md.A_COUNTY,
    case
        when (   (NVL(TRIM(md.a_COUNTRY), ''x'') != pu.COUNTRY AND pu.COUNTRY IS NOT NULL)) then
           to_char(pu.COUNTRY)
    else
     to_char(md.A_COUNTRY)
    end as a_COUNTRY,
   case
   when pu.ADDRESSMAILABLE != md.A_ADDRESSMAILABLE THEN
        case
            WHEN  (pu.ADDRESSMAILABLE = 0)  THEN
              0
            WHEN (pu.ADDRESSMAILABLE = 1 AND pu.ADDRESSLINE1 IS NOT NULL AND
                pu.CITY IS NOT NULL AND pu.ZIP IS NOT NULL AND
                pu.STATE IS NOT NULL )  THEN
              1
            ELSE
               md.A_ADDRESSMAILABLE
        end
        else
               md.A_ADDRESSMAILABLE
        END as A_ADDRESSMAILABLE,
    md.A_HOMEPHONE,
    md.A_MOBILEPHONE,
    md.A_WORKPHONE,
    md.A_SECONDARYEMAILADDRESS,
    md.A_MEMBERSTATUSCODE,
    md.A_DIRECTMAILOPTINDATE,
    md.A_EMAILOPTINDATE,
    md.A_SMSOPTINDATE,
    md.A_ENROLLDATE,
    md.A_GENDER,
    md.A_MEMBERSOURCE,
    md.A_LANGUAGEPREFERENCE,
    md.A_SECURITYQUESTION,
    md.A_SECURITYANSWER,
    md.A_OLDEMAILADDRESS,
    md.A_HOMESTOREID,
    md.A_HASEMAILBONUSCREDIT,
    md.A_ISUNDERAGE,
    md.A_EMPLOYEECODE,
    md.A_SMSOPTIN,
    md.A_DIRECTMAILOPTIN,
    md.A_EMAILOPTIN,
    md.A_HHKEY,
    md.STATUSCODE,
    md.CREATEDATE,
    cast (systimestamp as timestamp(6)) as updatedate,
    md.LASTDMLID,
    md.LAST_DML_ID,
    case when (md.A_EMAILADDRESS IS NULL AND PU.Emailaddress IS NOT NULL ) then
       cast(to_char(PU.Emailaddress) as nvarchar2(150))
     else
       cast(to_char(md.A_EMAILADDRESS)as nvarchar2(150))
     end as  A_EMAILADDRESS,
    case when (mtu.Ipcode IS NOT NULL) then
           1
    else
        md.A_AITUPDATE
    end as A_AITUPDATE,
    md.A_BRAFIRSTPURCHASEDATE,
    md.A_JEANSFIRSTPURCHASEDATE,
    md.A_LASTBRASTORENUMBER,
    md.A_LASTPURCHASEPOINTS,
    md.A_LASTBRAPURCHASEDATE,
    md.A_GWLINKED,
    md.A_GWOBJVER,
    md.A_HASGWBONUSCREDIT,
    md.A_AUTOISSUEREWARD,
    md.A_PRIMARYCONTACT,
    md.A_NETSPEND,
    md.A_DOWNLOADMOBILEAPP,
    md.A_PENDINGEMAILVERIFICATION,
    md.A_PENDINGCELLVERIFICATION,
    md.A_NEXTEMAILREMINDERDATE,
    md.A_STORELASTJEANSPURCHASED,
    md.A_LASTJEANSPURCHASEDATE,
     case WHEN (hs.ipcode IS not  null ) then
        cast ( hs.PROGRAMCHANGEDATE as date)
     else
        cast ( md.A_PROGRAMCHANGEDATE as date) END as
       A_PROGRAMCHANGEDATE,
    md.A_CARDCLOSEDATE,
    md.A_CARDOPENDATE,
    MD.A_TERMINATIONREASONID
  from bp_Ae.ATS_MEMBERDETAILS MD
       left join  (
          SELECT P.IPCODE,
               P.CID,
               P.FIRSTNAME,
               P.LASTNAME,
               P.ADDRESSLINE1,
               P.ADDRESSLINE2,
               P.CITY,
               P.STATE,
               P.ZIP,
               P.COUNTRY,
               P.ADDRESSMAILABLE,
               P.RECORDTYPE,
               P.Emailaddress,
               p.birthdate
            FROM bp_ae.LW_PROFILEUPDATES_STAGE P
            INNER JOIN bp_ae.ats_refstates st           ON st.a_statecode = p.state
           WHERE P.ERRORCODE = 0
     )  pu  on md.A_IPCODE = pu.IPCODE
    left join (select distinct Ipcode from bp_Ae.AE_MEBERSTOUPDATE) mtu
    on mtu.Ipcode = md.a_ipcode
    left JOIN (   SELECT distinct w.ipcode ,w.programchangedate, w.extendedplaycode
                  from bp_ae.ae_hubsyncchangeprogram  w
                  where (ipcode, programchangedate) in
                       (select ipcode, max(programchangedate)
                        from bp_ae.ae_hubsyncchangeprogram s
                        where w.ipcode = s.ipcode
                        GROUP BY s.ipcode) ) hs ON hs.ipcode  = pu.ipcode';

BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.ATS_MEMBERDETAILS_OLD PURGE';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'DROP old TABLE ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;



BEGIN

       EXECUTE IMMEDIATE 'Alter table bp_ae.ats_memberdetails  rename to ats_memberdetails_old';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'rename to ats_memberdetails_old ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;



BEGIN

      EXECUTE IMMEDIATE 'Drop index BP_AE.ATS_MBRDTL_PARENTROWKEY';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Drop BP_AE.ATS_MBRDTL_PARENTROWKEY ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;



BEGIN

EXECUTE IMMEDIATE 'Drop Index BP_AE.ATS_MEMBERDETAILS$#';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Drop  BP_AE.ATS_MEMBERDETAILS$# ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;

/*
BEGIN


EXECUTE IMMEDIATE 'Drop Index BP_AE.ATS_MEMBERDETAILSPK';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Drop BP_AE.ATS_MEMBERDETAILSPK ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;
*/

BEGIN

EXECUTE IMMEDIATE 'Drop Index BP_AE.ATS_MEMBERDETAIL_UK';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Drop BP_AE.ATS_MEMBERDETAIL_UK ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;


BEGIN

EXECUTE IMMEDIATE 'Drop Index BP_AE.IDX_MEMBERDETAILS_REWARDS';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Drop BP_AE.IDX_MEMBERDETAILS_REWARDS ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;



BEGIN

EXECUTE IMMEDIATE 'CREATE INDEX BP_AE.ATS_MBRDTL_PARENTROWKEY ON bp_ae.ats_memberdetails_new(A_PARENTROWKEY)
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D ';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Create BP_AE.ATS_MBRDTL_PARENTROWKEY ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;

BEGIN

EXECUTE IMMEDIATE '  CREATE UNIQUE INDEX BP_AE.ATS_MEMBERDETAILS$# ON bp_ae.ats_memberdetails_new(LAST_DML_ID)
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D ';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Create BP_AE.ATS_MEMBERDETAILS$# ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;

/*
BEGIN

EXECUTE IMMEDIATE '  CREATE UNIQUE INDEX BP_AE.ATS_MEMBERDETAILSPK ON bp_ae.ats_memberdetails_new (A_ROWKEY)
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D ';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Create BP_AE.ATS_MEMBERDETAILSPK ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;
*/
BEGIN

EXECUTE IMMEDIATE '  CREATE UNIQUE INDEX BP_AE.ATS_MEMBERDETAIL_UK ON bp_ae.ats_memberdetails_new (A_IPCODE)
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D ';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Create BP_AE.ATS_MEMBERDETAIL_UK ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;

BEGIN

EXECUTE IMMEDIATE '  CREATE INDEX BP_AE.IDX_MEMBERDETAILS_REWARDS ON bp_ae.ats_memberdetails_new (A_EXTENDEDPLAYCODE, A_PENDINGEMAILVERIFICATION, A_PENDINGCELLVERIFICATION, A_CARDTYPE)
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE BP_AE_D ';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Create BP_AE.IDX_MEMBERDETAILS_REWARDS ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;


BEGIN


EXECUTE IMMEDIATE 'alter table BP_AE.ATS_MEMBERDETAILS_new
  add unique (A_ROWKEY)';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := '  add unique ATS_MEMBERDETAILS_NEW (A_ROWKEY) '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN


EXECUTE IMMEDIATE '  Alter Table  BP_AE.ATS_MEMBERDETAILS_new Rename  to ATS_MEMBERDETAILS';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Rename  to ATS_MEMBERDETAILS ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;


BEGIN

EXECUTE IMMEDIATE 'DROP trigger BP_AE.ATS_MEMBERDETAILS#';

EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := ' DROP trigger BP_AE.ATS_MEMBERDETAILS# ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;


BEGIN

EXECUTE IMMEDIATE '  CREATE OR REPLACE TRIGGER "BP_AE"."ATS_MEMBERDETAILS#"
 BEFORE UPDATE OR INSERT ON bp_ae.ATS_MEMBERDETAILS
  FOR EACH ROW
BEGIN
  :NEW.last_dml_id := LAST_DML_ID#.nextval;
END;';


EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'CREATE trigger BP_AE.ATS_MEMBERDETAILS#  ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;


BEGIN


EXECUTE IMMEDIATE '  DROP TRIGGER  BP_AE.ATS_MEMBERDETAILS_ARC ';


EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'DROP TRIGGER  BP_AE.ATS_MEMBERDETAILS_ARC ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;




BEGIN
EXECUTE IMMEDIATE '  CREATE OR REPLACE TRIGGER "BP_AE"."ATS_MEMBERDETAILS_ARC"
 BEFORE UPDATE ON bp_ae.ATS_MEMBERDETAILS
   FOR EACH ROW
  BEGIN
     insert into ats_memberdetails_arc
       (
            ID
          , a_ipcode
          , a_cardtype
          , a_changedby
          , a_passvalidation
          , a_basebrandid
          , a_oldcellnumber
          , a_emailaddressmailable
          , a_addresslineone
          , a_addresslinetwo
          , a_city
          , a_stateorprovince
          , a_ziporpostalcode
          , a_country
          , a_addressmailable
          , a_homephone
          , a_mobilephone
          , a_directmailoptindate
          , a_emailoptindate
          , a_smsoptindate
          , a_gender
          , a_membersource
          , a_languagepreference
          , a_oldemailaddress
          , a_homestoreid
          , a_hasemailbonuscredit
          , a_isunderage
          , a_employeecode
          , a_smsoptin
          , a_directmailoptin
          , a_emailoptin
          , a_hhkey
          , a_emailaddress
          , a_aitupdate
          , updatedate
          , a_extendedplaycode
          , a_programchangedate
          , a_autoissuereward
          , a_netspend
          , a_downloadmobileapp
          , a_pendingemailverification
          , a_pendingcellverification
          , a_nextemailreminderdate
          , a_cardclosedate
          , a_cardopendate
       )
     values
       (
            seq_rowkey.nextval
          , :OLD.a_ipcode
          , :OLD.a_cardtype
          , :OLD.a_changedby
          , :OLD.a_passvalidation
          , :OLD.a_basebrandid
          , :OLD.a_oldcellnumber
          , :OLD.a_emailaddressmailable
          , :OLD.a_addresslineone
          , :OLD.a_addresslinetwo
          , :OLD.a_city
          , :OLD.a_stateorprovince
          , :OLD.a_ziporpostalcode
          , :OLD.a_country
          , :OLD.a_addressmailable
          , :OLD.a_homephone
          , :OLD.a_mobilephone
          , :OLD.a_directmailoptindate
          , :OLD.a_emailoptindate
          , :OLD.a_smsoptindate
          , :OLD.a_gender
          , :OLD.a_membersource
          , :OLD.a_languagepreference
          , :OLD.a_oldemailaddress
          , :OLD.a_homestoreid
          , :OLD.a_hasemailbonuscredit
          , :OLD.a_isunderage
          , :OLD.a_employeecode
          , :OLD.a_smsoptin
          , :OLD.a_directmailoptin
          , :OLD.a_emailoptin
          , :OLD.a_hhkey
          , :OLD.a_emailaddress
          , :OLD.a_aitupdate
          , :OLD.updatedate
          , :OLD.a_extendedplaycode
          , :OLD.a_programchangedate
          , :OLD.a_autoissuereward
          , :OLD.a_netspend
          , :OLD.a_downloadmobileapp
          , :OLD.a_pendingemailverification
          , :OLD.a_pendingcellverification
          , :OLD.a_nextemailreminderdate
          , :OLD.a_cardclosedate
          , :OLD.a_cardopendate
       );

END;';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'Create trigger BP_AE.ATS_MEMBERDETAILS_ARC  ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;



     V_REASON := 'ctas_member_details end ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

END ctas_member_details;

PROCEDURE ctas_virtual_card is


    V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
   -- V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'STAGE_PROFILEUPDATES2';

    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

BEGIN

      V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
      V_JOBNAME   := 'ctas_virtual_card';
      V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);


     V_REASON := 'ctas_virtual_card start ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


BEGIN
     EXECUTE IMMEDIATE 'drop  table bp_Ae.Lw_Virtualcard_new ';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'DROP NEW TABLE ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;

--Create tablename_New
EXECUTE IMMEDIATE
'Create table bp_ae.lw_virtualcard_new AS
  select  vc.vckey   ,
  vc.loyaltyidnumber       ,
  vc.ipcode                ,
  CASE
         WHEN t.cid IS NOT NULL THEN
          CAST (t.cid AS Number(20))
      ELSE
         vc.linkkey
      END  as linkkey      ,
  vc.dateissued            ,
  vc.dateregistered        ,
  vc.status                ,
  vc.newstatus             ,
  vc.newstatuseffectivedate ,
  vc.statuschangereason     ,
  case when t.loyaltynumber is not null then
   T.Recordtype
   else
     vc.isprimary
     end AS ISPRIMARY,
  vc.cardtype               ,
  vc.createdate             ,
  vc.last_dml_id            ,
  vc.changedby              ,
  vc.expirationdate         ,
    cast (systimestamp as timestamp(6)) as updatedate           from
  bp_Ae.Lw_Virtualcard vc
  left join ( SELECT
               P.CID,
               p.loyaltynumber,
             case when p.recordtype = ''P'' then
                    1
          else 0
               end as Recordtype
          FROM bp_ae.ae_profileupdates2 P
--          WHERE p.loyaltynumber NOT IN (  SELECT  pe.loyaltyid  FROM  bp_ae.ae_profileupdateexception pe
--                                          where pe.notes <> ''AIT Update Changed''   )
                  ORDER BY p.Recordtype DESC) T ON t.loyaltynumber = vc.loyaltyidnumber';
--Table rename to old

/*
BEGIN
   execute immediate 'alter table BP_AE.LW_VIRTUALCARD drop unique (VCKEY)';
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'drop unique (VCKEY) ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;


BEGIN
     execute immediate 'drop index BP_AE.IDX_VIRTUALCARD_PK' ;
EXCEPTION
     WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'drop index BP_AE.IDX_VIRTUALCARD_PK ' || Err_Code || ' ' || Err_Msg;
          v_Message := v_Reason;
          v_Error   := v_Reason;
          Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                              p_Envkey    => v_Envkey,
                              p_Logsource => v_Logsource,
                              p_Filename  => NULL,
                              p_Batchid   => v_Batchid,
                              p_Jobnumber => v_My_Log_Id,
                              p_Message   => v_Message,
                              p_Reason    => v_Reason,
                              p_Error     => v_Error,
                              p_Trycount  => v_Trycount,
                              p_Msgtime   => SYSDATE);
          NULL;
END;

*/

    BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.LW_VIRTUALCARD_OLD PURGE';

    EXCEPTION

        WHEN OTHERS THEN
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'DROP OLD TABLE. '|| err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

         NULL;
    END;

begin
   EXECUTE IMMEDIATE 'Alter table bp_ae.lw_virtualcard  rename to lw_virtualcard_old';
exception when others THEN
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'RENAMING TO OLD_tablename. '|| err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;



begin

--Drop indexes
EXECUTE IMMEDIATE 'Drop Index BP_AE.IDX_VIRTUALCARD_IPCODE';
exception when others THEN
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := ' Drop index BP_AE.IDX_VIRTUALCARD_IPCODE '||err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;

BEGIN
EXECUTE IMMEDIATE 'create index BP_AE.IDX_VIRTUALCARD_IPCODE on BP_AE.LW_VIRTUALCARD_new (IPCODE)
  tablespace BP_AE_IDX1
  pctfree 0
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  ) parallel compute statistics';
  exception when others THEN
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'create index BP_AE.IDX_VIRTUALCARD_IPCODE '|| err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;

BEGIN
EXECUTE IMMEDIATE ' Drop Index  BP_AE.LW_VIRTUALCARD$#';
exception when others THEN
       err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'Drop Index  BP_AE.LW_VIRTUALCARD$# '|| err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;



BEGIN

EXECUTE IMMEDIATE 'create unique index BP_AE.LW_VIRTUALCARD$# on BP_AE.LW_VIRTUALCARD_new (LAST_DML_ID)
  tablespace BP_AE_D
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  ) parallel compute statistics';
  exception when others THEN
       err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := 'Create Index  BP_AE.LW_VIRTUALCARD$# '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;


/*
BEGIN

EXECUTE IMMEDIATE 'create unique index BP_AE.IDX_VIRTUALCARD_PK on BP_AE.LW_VIRTUALCARD_new (VCKEY)
  tablespace BP_AE_D
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  ) parallel compute statistics';
  exception when others THEN
       err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := 'Create Index  BP_AE.IDX_VIRTUALCARD_PK '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;
*/
begin

EXECUTE IMMEDIATE 'alter table BP_AE.LW_VIRTUALCARD_new
  add primary key (VCKEY)';

exception when others THEN
   err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := ' add primary key (VCKEY) '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;


begin
EXECUTE IMMEDIATE 'alter table BP_AE.LW_VIRTUALCARD_new
  add unique (LOYALTYIDNUMBER)
  using index
  tablespace BP_AE_D
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  ) compute statistics';

exception when others THEN
       err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := '  add unique (LOYALTYIDNUMBER) '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;


BEGIN
EXECUTE IMMEDIATE '  Drop Index  BP_AE.LW_VIRTUALCARD_UK01';
exception when others THEN
      err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := ' Drop Index  BP_AE.LW_VIRTUALCARD_UK01'|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;


BEGIN

EXECUTE IMMEDIATE 'DROP TRIGGER  BP_AE.LW_VIRTUALCARD# ';

exception when others THEN
       err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := ' DROP TRIGGER  BP_AE.LW_VIRTUALCARD# '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

  null;
end;



begin
--Rename Tablename_New
EXECUTE IMMEDIATE 'Alter Table  bp_ae.lw_virtualcard_new  Rename  to lw_virtualcard';

exception when others THEN
      err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := '  Renaming to original tablename '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;

  DECLARE
      CURSOR GET_DATA IS
          SELECT *
          FROM   (SELECT Vc2.Ipcode,
                         Vc2.Loyaltyidnumber,
                         Vc2.Dateissued,
                         Vc2.Vckey,
                         Row_Number() Over(PARTITION BY Vc2.Ipcode ORDER BY Vc2.Dateissued DESC) r
                  FROM   Bp_Ae.Lw_Virtualcard Vc2,
                         (SELECT Vc.Ipcode, SUM(Vc.Isprimary) AS Primarycount
                          FROM   Bp_Ae.Lw_Virtualcard Vc
                          GROUP  BY Vc.Ipcode
                          HAVING SUM(Vc.Isprimary) > 1) t
                  WHERE  t.Ipcode = Vc2.Ipcode
                  ORDER  BY Vc2.Ipcode, Vc2.Dateissued DESC)
          WHERE  r > 1;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      V_TBL T_TAB; ---<-

BEGIN
     V_REASON := 'fixing isprimary = 1start ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT  INTO V_TBL LIMIT 10000;
        FORALL I IN 1 .. V_TBL.COUNT
              UPDATE lw_virtualcard vc
                  SET vc.isprimary = 0
              WHERE vc.vckey = v_tbl(i).vckey;
         COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;


     V_REASON := 'fixing isprimary = 1 end ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);



END;



BEGIN
EXECUTE IMMEDIATE 'create unique index BP_AE.LW_VIRTUALCARD_UK01 on BP_AE.LW_VIRTUALCARD (CASE ISPRIMARY WHEN 1 THEN TO_CHAR(ISPRIMARY)||''/''||TO_CHAR(IPCODE) ELSE NULL END)
  tablespace BP_AE_D
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  ) parallel compute statistics';
exception when others THEN
    err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := ' create Index  BP_AE.LW_VIRTUALCARD_UK01'|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;


--Drop and re-add trigger

BEGIN
EXECUTE IMMEDIATE 'CREATE OR REPLACE TRIGGER BP_AE.LW_VIRTUALCARD#
 BEFORE UPDATE OR INSERT ON LW_VIRTUALCARD
  FOR EACH ROW
BEGIN
  :NEW.last_dml_id := LAST_DML_ID#.nextval;

end;';

exception when others THEN
    err_code := SQLCODE;
       err_msg := SUBSTR(SQLERRM, 1, 200);
       V_REASON := ' CREATE TRIGGER  BP_AE.LW_VIRTUALCARD# '|| err_code ||' ' ||err_msg;
       V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;
end;

     V_REASON := 'ctas_virtual_card end ' ;
     V_MESSAGE:= v_reason;
     V_ERROR:= ' ';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

END ctas_virtual_card;

PROCEDURE ctas_loyalty_member is
    V_MY_LOG_ID NUMBER;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(1024);
    V_REASON    VARCHAR2(1024);
    V_ERROR     VARCHAR2(1024);
    V_TRYCOUNT  NUMBER := 0;
    err_code VARCHAR2(10) := '';
    err_msg  VARCHAR2(200) := '';
    P_FILENAME VARCHAR2(50) := 'STAGE_PROFILEUPDATES2';
BEGIN

      V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
       V_JOBNAME   := 'ctas_loyalty_member';
      V_LOGSOURCE := V_JOBNAME;


       UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

      V_ERROR          :=  ' ';
      V_REASON         := 'drop and recreate LW_LOYALTYMEMBER_new start';
      V_MESSAGE        := 'drop and recreate LW_LOYALTYMEMBER_new start';
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

    BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.LW_LOYALTYMEMBER_new PURGE';

    EXCEPTION

        WHEN OTHERS THEN
            IF sqlcode = -942 THEN
                NULL;
            ELSE
               RAISE;
            END IF;
    END;



EXECUTE IMMEDIATE '
create table bp_ae.lw_loyaltymember_new tablespace bp_ae_d as
    select
            lm.ipcode,
            membercreatedate,
            memberclosedate,
            memberstatus,
            case
               when pu.birthdate is not null then
                   pu.birthdate
               else
                   lm.birthdate
            end as birthdate  ,
            cast ( case when pu.firstname is not null
                        then pu.firstname
                        else to_char(lm.firstname)
                        end as nvarchar2(50))as firstname,
            cast ( CASE WHEN  pu.LASTNAME IS NOT NULL
                  THEN pu.LASTNAME
                        ELSE TO_char(lm.lastname)
                        END as nvarchar2(50)) as lastname,
            middlename,
            nameprefix,
            namesuffix,
            mobiledevicetype,
            username,
            password,
            primaryphonenumber,
            cast (nvl(pu.zip, primarypostalcode) as nvarchar2(15)) as primarypostalcode,
            lastactivitydate,
            isemployee,
            cast (''Profile Update'' as nvarchar2(25)) as changedby,
            newstatus,
            newstatuseffectivedate,
            lm.createdate,
            last_dml_id,
            statuschangereason,
            resetcode,
            resetcodedate,
            cast (systimestamp as timestamp(6)) as updatedate,
            alternateid,
            salt,
            passwordexpiredate,
            statuschangedate,
            mergedtomember,
            failedpasswordattemptcount,
            passwordchangerequired,
            primaryemailaddress
       from bp_ae.lw_loyaltymember lm,
            bp_ae.lw_profileupdates_stage pu
      where lm.ipcode = pu.ipcode(+)';

      V_ERROR          :=  ' ';
      V_REASON         := 'drop and recreate LW_LOYALTYMEMBER_new end';
      V_MESSAGE        := 'drop and recreate LW_LOYALTYMEMBER_new end';
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
      V_ERROR          :=  ' ';
      V_REASON         := 'rename lw_membertiers to old and index creation for new start';
      V_MESSAGE        := 'rename lw_membertiers to old and index creation for new start';
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

  BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.LW_LOYALTYMEMBER_hub PURGE';

    EXCEPTION
        WHEN OTHERS THEN
           err_code := SQLCODE;
           err_msg := SUBSTR(SQLERRM, 1, 200);
           V_REASON := 'DROP old TABLE LW_LOYALTYMEMBER_hub'||err_code ||' ' ||err_msg;
           V_MESSAGE:= v_reason;
           V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

    END;

begin
    drop_constraint('bp_ae.lw_membertiers', 'fk_membertier_ipcode');
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := 'DROP constraint fk_membertier_ipcode '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


---
begin
    EXECUTE IMMEDIATE 'drop index BP_AE.LW_LOYALTYMEMBER_EMAILIDX ';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' drop index BP_AE.LW_LOYALTYMEMBER_EMAILIDX  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;

---
  begin
    drop_constraint('bp_ae.lw_csnote', 'fk_csnote_memberid');
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := 'DROP constraint fk_csnote_memberid '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;

--EXECUTE IMMEDIATE 'alter table bp_ae.lw_membertiers drop constraint fk_membertier_ipcode';

--EXECUTE IMMEDIATE 'alter table bp_ae.lw_csnote drop constraint fk_csnote_memberid';
begin
    EXECUTE IMMEDIATE 'drop index BP_AE.IDX_MEMBER_ALTERNATEID ';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' drop index BP_AE.IDX_MEMBER_ALTERNATEID  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


begin
    EXECUTE IMMEDIATE 'drop index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE ';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' drop index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


begin
    EXECUTE IMMEDIATE 'drop index BP_AE.LW_LOYALTYMEMBER$# ';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' drop index BP_AE.LW_LOYALTYMEMBER$#  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


begin
    EXECUTE IMMEDIATE 'drop index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP ';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' drop index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN
  EXECUTE IMMEDIATE 'ALTER TABLE bp_Ae.Lw_Loyaltymember DROP UNIQUE (USERNAME)';
  exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := 'ALTER TABLE bp_Ae.Lw_Loyaltymember DROP UNIQUE (USERNAME)'||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;


END;

/*
begin
    EXECUTE IMMEDIATE 'drop index BP_AE.LW_LOYALTYMEMBER_UK1 ';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' drop index BP_AE.LW_LOYALTYMEMBER_UK1  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;

*/

begin
    EXECUTE IMMEDIATE 'Alter table bp_ae.lw_loyaltymember  rename to lw_loyaltymember_hub';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' rename to lw_loyaltymember_hub '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;

begin
   EXECUTE IMMEDIATE 'create index BP_AE.IDX_MEMBER_ALTERNATEID on BP_AE.LW_LOYALTYMEMBER_NEW (ALTERNATEID)
  tablespace BP_AE_D parallel compute statistics';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' create index BP_AE.IDX_MEMBER_ALTERNATEID '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN
   EXECUTE IMMEDIATE 'create index BP_AE.LW_LOYALTYMEMBER_EMAILIDX on BP_AE.LW_LOYALTYMEMBER_new (PRIMARYEMAILADDRESS)
                      tablespace BP_AE_D parallel compute statistics';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' create index BP_AE.LW_LOYALTYMEMBER_EMAILIDX '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


begin

EXECUTE IMMEDIATE 'create index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE on BP_AE.LW_LOYALTYMEMBER_NEW (ALTERNATEID, IPCODE)
  tablespace BP_AE_D  parallel compute statistics';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' create index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


begin

EXECUTE IMMEDIATE 'create index BP_AE.LW_LOYALTYMEMBER$# on BP_AE.LW_LOYALTYMEMBER_NEW (LAST_DML_ID)
  tablespace BP_AE_D  parallel compute statistics';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' create index BP_AE.LW_LOYALTYMEMBER$#  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;



begin

EXECUTE IMMEDIATE 'create index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP on BP_AE.LW_LOYALTYMEMBER_NEW (UPPER(LASTNAME), UPPER(PRIMARYPOSTALCODE))
  tablespace BP_AE_D  parallel compute statistics';


exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' create index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


/*
begin

EXECUTE IMMEDIATE 'create unique index BP_AE.LW_LOYALTYMEMBER_UK1 on BP_AE.LW_LOYALTYMEMBER_NEW (USERNAME)
  tablespace BP_AE_D  parallel compute statistics';


exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' create index BP_AE.LW_LOYALTYMEMBER_UK1  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;
*/

-- Create/Recreate primary, unique and foreign key constraints

begin
EXECUTE IMMEDIATE 'alter table BP_AE.LW_LOYALTYMEMBER_NEW
  add primary key (IPCODE)
  using index
  tablespace BP_AE_D';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' add primary key (IPCODE)  '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN


EXECUTE IMMEDIATE 'alter table BP_AE.LW_LOYALTYMEMBER_NEW
  add unique (USERNAME)';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := '  add unique (USERNAME) '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN
EXECUTE IMMEDIATE 'alter table bp_ae.lw_loyaltymember_new modify failedpasswordattemptcount default 0';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := '  failedpasswordattemptcount default 0 '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN
EXECUTE IMMEDIATE 'alter table bp_ae.lw_loyaltymember_new modify passwordchangerequired default 0';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := 'passwordchangerequired default 0'||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN


EXECUTE IMMEDIATE 'alter table bp_ae.lw_loyaltymember_new rename to lw_loyaltymember';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := 'rename to lw_loyaltymember '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;



BEGIN


EXECUTE IMMEDIATE 'alter table BP_AE.LW_CSNOTE
  add constraint FK_CSNOTE_MEMBERID foreign key (MEMBERID)
  references BP_AE.LW_LOYALTYMEMBER (IPCODE)
  enable
  validate';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' add constraint FK_CSNOTE_MEMBERID '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN


EXECUTE IMMEDIATE 'alter table BP_AE.LW_MEMBERTIERS
  add constraint FK_MEMBERTIER_IPCODE foreign key (MEMBERID)
  references BP_AE.LW_LOYALTYMEMBER (IPCODE)
  enable
  validate';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' add constraint FK_MEMBERTIER_IPCODE '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;





-- create triggers:
BEGIN


EXECUTE IMMEDIATE 'DROP  TRIGGER BP_AE.LW_LOYALTYMEMBER#';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' DROP TRIGGER BP_AE.LW_LOYALTYMEMBER# '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;

BEGIN


EXECUTE IMMEDIATE 'DROP  TRIGGER BP_AE.LW_LOYALTYMEMBER_ARC';
exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' DROP TRIGGER BP_AE.LW_LOYALTYMEMBER_ARC '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;


BEGIN



EXECUTE IMMEDIATE 'CREATE OR REPLACE TRIGGER BP_AE."LW_LOYALTYMEMBER#"
 BEFORE UPDATE OR INSERT ON bp_ae.LW_LOYALTYMEMBER
  FOR EACH ROW
BEGIN
  -- this removes spaces/dashes from postal codes before they are written.  Added by cnelson 12/16/2015
  -- regexp matches what is used in the memberlookup proc
  :new.primarypostalcode := REGEXP_REPLACE(TRIM(UPPER(:new.primarypostalcode)), ''[-[:space:]]'', '''');

  :NEW.last_dml_id := LAST_DML_ID#.nextval;
END;';

exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' TRIGGER BP_AE."LW_LOYALTYMEMBER#'||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;

BEGIN


EXECUTE IMMEDIATE 'CREATE OR REPLACE TRIGGER BP_AE."LW_LOYALTYMEMBER_ARC"
 BEFORE UPDATE ON bp_ae.LW_LOYALTYMEMBER
  FOR EACH ROW
BEGIN
insert into lw_loyaltymember_arc (id, ipcode, memberclosedate, memberstatus, firstname, lastname, primaryemailaddress, primaryphonenumber, primarypostalcode, lastactivitydate, changedby, newstatus, newstatuseffectivedate, last_dml_date, last_dml_id, statuschangereason, birthdate)
values (seq_rowkey.nextval, :OLD.ipcode, :OLD.memberclosedate, :OLD.memberstatus, :OLD.firstname, :OLD.lastname, :OLD.primaryemailaddress, :OLD.primaryphonenumber, :OLD.primarypostalcode, :OLD.lastactivitydate,
       :OLD.changedby, :OLD.newstatus, :OLD.newstatuseffectivedate, :OLD.createdate, :OLD.last_dml_id, :OLD.statuschangereason, :OLD.Birthdate);

END;';



exception when others THEN

      err_code := SQLCODE;
      err_msg := SUBSTR(SQLERRM, 1, 200);
      V_REASON := ' TRIGGER BP_AE.LW_LOYALTYMEMBER_ARC '||err_code ||' ' ||err_msg;
      V_MESSAGE:= v_reason;
       V_ERROR:= v_reason;

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
  null;

end;






END ctas_loyalty_member;


PROCEDURE Updatetieronly(p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id          NUMBER := 384838;
          v_Messagesfailed   NUMBER := 0;
          --log msg attributes
          v_Messageid   VARCHAR2(256);
          v_Envkey      VARCHAR2(256) := 'bp_ae@' ||
                                         Upper(Sys_Context('userenv',
                                                           'instance_name'));
          v_Batchid     VARCHAR2(256) := 0;
          v_Message     VARCHAR2(256) := ' ';
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          ---v_Processdate        date :=  to_date(p_Processdate,'DD/MM/YYYY HH:MI AM');
          CURSOR Process_Rule IS
               SELECT DISTINCT  md.a_Ipcode            AS Ipcode,
                      nvl(Md.a_Netspend,0)        AS Netspend,
                      nvl(lm.membercreatedate,to_date('1/1/1900','mm/dd/yyyy')) AS EnrollDate --aeo-1214
               FROM   bp_ae.ae_merge m
               INNER  JOIN Bp_Ae.Ats_Memberdetails Md ON Md.a_Ipcode = m.new_ipcode
               INNER  JOIN Bp_Ae.lw_loyaltymember  Lm ON Lm.Ipcode = m.new_ipcode
               WHERE  1 = 1 AND
                      nvl(md.a_netspend,0)  >0 ;
                     -- nvl(md.a_extendedplaycode,0) IN (1,3);

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl          t_Tab;
          v_Tierid       NUMBER := 0;
          v_Tierdesc     VARCHAR2(50);
          v_fullaccesstier     NUMBER := 0;
          v_extraaccesstier    NUMBER := 0;
          v_Ct_Tierid    NUMBER := 0;
         -- v_Ct_Updtierid NUMBER := 0;
          v_membertierid NUMBER := -1;


     BEGIN
          SELECT Tr.Tierid INTO   v_fullaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Full Access';

          SELECT Tr.Tierid INTO   v_extraaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Extra Access';

          FOR y IN Process_Rule
          LOOP
               SELECT COUNT(Mt.Tierid) --mt.tierid
               INTO   v_Ct_Tierid
               FROM   Lw_Membertiers Mt
               WHERE  Mt.Memberid = y.Ipcode
               AND    Trunc(Mt.Todate) > Trunc(SYSDATE); --AEO-1069
               IF v_Ct_Tierid > 0
               THEN
                    SELECT Mt.Tierid, Mt.Description --mt.tierid
                    INTO   v_Tierid, v_Tierdesc
                    FROM   Lw_Membertiers Mt
                    WHERE  Mt.Memberid = y.Ipcode
                    AND    Trunc(Mt.Todate) > Trunc(SYSDATE) --AEO-1069
                    AND Rownum = 1;
               END IF;
               IF y.Netspend >= 350
               THEN
                    IF v_Tierid <> v_extraaccesstier
                    THEN

                         UPDATE Lw_Membertiers Mt1
                         SET    Mt1.Todate = p_processdate
                         WHERE  Mt1.Memberid = y.Ipcode
                                AND mt1.todate > p_processdate
                                AND Mt1.Tierid <> v_extraaccesstier ;

                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;

                         v_membertierid := Hibernate_Sequence.Nextval;
                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (Hibernate_Sequence.Nextval,
                               v_extraaccesstier,
                               y.Ipcode,
                               --  y.txndate,
                               p_Processdate + 1/86400,
                               Getexpirationdate(y.enrolldate, p_Processdate), -- AEO-1214
                               'Merged',
                               SYSDATE,
                               SYSDATE);

                       INSERT INTO Ats_Membernetspend
                            (a_Rowkey,
                             Lwidentifier,
                             a_Ipcode,
                             a_Parentrowkey,
                             a_Membertierid,
                             a_Netspend,
                             Statuscode,
                             Createdate,
                             Updatedate)
                       VALUES
                            (Seq_Rowkey.Nextval,
                             0,
                             y.Ipcode,
                             -1,
                             v_Membertierid,
                             y.Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);

                    END IF;
                    IF v_Ct_Tierid = 0
                    THEN

                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (Hibernate_Sequence.Nextval,
                               v_extraaccesstier,
                               y.Ipcode,
                               -- y.txndate,
                               p_Processdate,
                             Getexpirationdate(y.enrolldate, p_processdate),  -- AEO1214
                               'Base',-- AEO-503
                               -- AEO-2007
                               Sysdate,
                               SYSDATE);
                              --AEO=2007

                       INSERT INTO Ats_Membernetspend
                            (a_Rowkey,
                             Lwidentifier,
                             a_Ipcode,
                             a_Parentrowkey,
                             a_Membertierid,
                             a_Netspend,
                             Statuscode,
                             Createdate,
                             Updatedate)
                       VALUES
                            (Seq_Rowkey.Nextval,
                             0,
                             y.Ipcode,
                             -1,
                             v_Membertierid,
                             y.Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);

                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;
                    END IF;
               END IF;
               /*
               IF y.Netspend < 250
               THEN
                    IF v_Tierid = v_Silvertier
                       AND Upper(v_Tierdesc) NOT LIKE Upper('%Nomination%')
                       AND
                       Upper(v_Tierdesc) NOT LIKE Upper('%Pilot Test Group%')
                    THEN
                         --IF THIS IS SILVER TIER and not an Nominated Status,THEN UPDATE SILVER TIER RECORD AND ADD A BLUE TIER
                         UPDATE Lw_Membertiers Mt1
                         SET    Mt1.Todate = SYSDATE
                         WHERE  Mt1.Memberid = y.Ipcode
                                AND Mt1.Tierid = v_Silvertier;
                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;
                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (Hibernate_Sequence.Nextval,
                               v_Bluetier,
                               y.Ipcode,
                               --y.txndate,
                               p_Processdate,
                               v_expirationdate, -- AEO-1214
                               'Merged',
                               p_Processdate,
                               p_Processdate);
                         INSERT INTO Lw_Csnote
                              (Id,
                               Memberid,
                               Note,
                               Createdby,
                               Createdate,
                               Updatedate)
                         VALUES
                              (Seq_Csnote.Nextval,
                               y.Ipcode,
                               'Tier downgraded due to net spend dropping below threshold',
                               1410, --Brierley System
                               p_Processdate,
                               p_Processdate);
                    END IF;
                    IF v_Ct_Tierid = 0
                    THEN
                         --IF THERE ARE NEITHER OF THE TIERS,THEN ADD A Blue TIER
                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (Hibernate_Sequence.Nextval,
                               v_Bluetier,
                               y.Ipcode,
                               --  y.txndate,
                               p_Processdate,
                               v_expirationdate, -- AEO-1214
                               'Base',
                               p_Processdate,
                               p_Processdate);
                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;
                         --Did the member have a Silver Tier status?
                         SELECT COUNT(Mt.Tierid)
                         INTO   v_Ct_Updtierid
                         FROM   Lw_Membertiers Mt
                         WHERE  Mt.Memberid = y.Ipcode
                                AND Mt.Tierid = v_Silvertier
                                AND upper(mt.description)  NOT LIKE Upper('%Nomination%') -- aeo-485
                                AND Upper(mt.description) NOT LIKE Upper('%Pilot Test Group%');  -- aeo-485

                         IF v_Ct_Updtierid > 0
                         THEN
                              INSERT INTO Lw_Csnote
                                   (Id,
                                    Memberid,
                                    Note,
                                    Createdby,
                                    Createdate,
                                    Updatedate)
                              VALUES
                                   (Seq_Csnote.Nextval,
                                    y.Ipcode,
                                    'Tier downgraded due to net spend dropping below threshold',
                                    1410, --Brierley System
                                    p_Processdate,
                                    p_Processdate);
                         END IF;
                    END IF;
               END IF;
               */
               v_Cnt := v_Cnt + 1;
               IF MOD(v_Cnt, 1000) = 0
               THEN
                    COMMIT;
               END IF;
          END LOOP;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure UpdateTierOnly: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .ipcode || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ipcode;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'UpdateTierOnly',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'UpdateTierOnly',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in UpdateTierOnly');
     END Updatetieronly;


    procedure drop_constraint (p_table varchar2, p_constraint varchar2) is
       e exception;
       pragma exception_init (e, -2443);
    begin
       execute immediate 'alter table ' || p_table || ' drop constraint '||p_constraint;
    exception
       when e then null;
    end drop_constraint;


  PROCEDURE update_ats_mergehistory  (RETVAL     IN OUT RCURSOR) IS

    CURSOR GET_DATA IS
           SELECT  me.old_ipcode, me.old_loyalty_id, me.new_ipcode, me.new_loyalty_id
             FROM bp_ae.ae_merge me
             INNER JOIN lw_virtualcard vc ON vc.loyaltyidnumber = me.old_loyalty_id
             INNER JOIN lw_virtualcard vc ON vc.loyaltyidnumber = me.new_loyalty_id;


    TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
    V_TBL T_TAB;

--AEO-1290 BEGIN
    CURSOR GET_DATA1 IS
           SELECT DISTINCT me.old_ipcode AS ipcode FROM bp_ae.ae_merge me;

    TYPE T_TAB1 IS TABLE OF GET_DATA1%ROWTYPE;
    V_TBL1 T_TAB1;
--AEO-1290 END

    V_MY_LOG_ID NUMBER;


    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := '';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    P_FILENAME VARCHAR2(50) := 'STAGE_PROFILEUPDATES2';
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);


    BEGIN

     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'update_ats_mergehistory';
     V_LOGSOURCE := V_JOBNAME;


     UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);



     V_ERROR          :=  ' ';
     V_REASON         := 'update_ats_mergehistory begin';
     V_MESSAGE        := 'update_ats_mergehistory begin';
     UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 10000;
        FORALL I IN 1 .. V_TBL.COUNT


             INSERT INTO bp_ae.ats_membermergehistory
                  (a_rowkey,
                   a_ipcode,
                   a_parentrowkey,
                   a_fromloyaltyid,
                   a_fromipcode,
                   a_changedby,
                   statuscode,
                   createdate,
                   updatedate,
                   Last_Dml_Id
                   )
             VALUES
                  ( hibernate_sequence.nextval,
                   v_Tbl(i).new_ipcode,
                   v_Tbl(i).new_ipcode,
                   v_Tbl(i).old_loyalty_id,
                   v_Tbl(i).old_ipcode,
                   'Hub Sync Initial Load',
                   0,
                   SYSDATE,
                   SYSDATE,
                   LAST_DML_ID#.Nextval);
        COMMIT;
        EXIT WHEN GET_DATA%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
         CLOSE GET_DATA;
      END IF;

	  --AEO-1290 BEGIN
--Update to 6 the status and clean the primary email for the secondary members merged
   --UTILITY_PKG.LOG_PROCESS_START(V_JOBNAME, 'update_lw_loyaltymember', V_PROCESSID);

      OPEN GET_DATA1;
      LOOP
        FETCH GET_DATA1 BULK COLLECT
          INTO V_TBL1 LIMIT 10000;
        FORALL I IN 1 .. V_TBL1.COUNT
               UPDATE BP_AE.Lw_Loyaltymember lm
               SET lm.memberstatus = 6,
               lm.primaryemailaddress = NULL
               WHERE lm.ipcode = v_Tbl1(i).ipcode
               ;
        COMMIT;
        EXIT WHEN GET_DATA1%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA1%ISOPEN THEN
         CLOSE GET_DATA1;
      END IF;
--AEO-1290 END

--AEO-1331 BEGIN
--Update to 0 the AIT Flag for the secondary members merged
   --UTILITY_PKG.LOG_PROCESS_START(V_JOBNAME, 'update_ats_memberdetails', V_PROCESSID);

      OPEN GET_DATA1;
      LOOP
        FETCH GET_DATA1 BULK COLLECT
          INTO V_TBL1 LIMIT 10000;
        FORALL I IN 1 .. V_TBL1.COUNT
               UPDATE BP_AE.ats_memberdetails md
               SET md.a_aitupdate = 0
               WHERE md.a_ipcode = v_Tbl1(i).ipcode
               ;
        COMMIT;
        EXIT WHEN GET_DATA1%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA1%ISOPEN THEN
         CLOSE GET_DATA1;
      END IF;
--AEO-1331 END

     V_ERROR          :=  ' ';
     V_REASON         := 'update_ats_mergehistory end';
     V_MESSAGE        := 'update_ats_mergehistory end';
     UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


    OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;


   END;


END STAGE_PROFILEUPDATES2;
/
