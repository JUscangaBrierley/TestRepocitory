CREATE OR REPLACE PACKAGE BP_AE.AEPointsJobs IS

     TYPE Rcursor IS REF CURSOR;

     --AEO-1223 Begin
  PROCEDURE RemovePointsOnHold(p_PointsOnHoldDate VARCHAR2, Retval IN OUT Rcursor);

  PROCEDURE ExpirePoints(p_Dummy VARCHAR2, Retval IN OUT Rcursor);
  --AEO-1223 End
  --AEO-1212 begin
  PROCEDURE AwardWelcomeBonus (p_processdate IN  VARCHAR2);
  -- AEO-1227 end
  PROCEDURE Updatelastaememberpointssent(p_Dummy VARCHAR2);

  PROCEDURE Memberpointsdelta_Connected(p_Dummy       VARCHAR2,
                                 p_Processdate DATE,
                                 b_Name        VARCHAR2,
                                 p_includetxns varchar2, -- AEO-2324
                                 Retval        IN OUT Rcursor);

  PROCEDURE Headerrecord_Mbrpoints_Conn(p_Dummy                VARCHAR2,
                                         p_Processdate          DATE,
                                         p_Memberfilestarttime  DATE,
                                         v_Transactionstartdate DATE);
  PROCEDURE changelastaememberpointssent(p_date VARCHAR2);
   PROCEDURE ChangeUnexpiredPointsStart(p_date VARCHAR2);

  PROCEDURE ProcessBraCodes(p_ProcessFrom           VARCHAR2,
                        p_ProcessTo           VARCHAR2,
                        p_BraCodes       VARCHAR2,
                        p_Processdate       TIMESTAMP);



END AEPointsJobs;
/
CREATE OR REPLACE PACKAGE BODY BP_AE.AEPointsJobs IS

 -- AEO-1227
 PROCEDURE AwardWelcomeBonus (p_processdate IN  VARCHAR2) IS



          v_Pointtypeid          NUMBER;

       --   v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);

          --log job attributes
          v_processdate      DATE  := to_date(p_processdate,'MM/DD/YYYY');
          v_Jobdirection     NUMBER := 0;
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256):= 'AwardWelcomeBonus' ;
          v_Filename         VARCHAR2(512):= v_jobname;
         -- v_Processid        NUMBER := 0;
          --v_Errormessage     VARCHAR2(256);
          --log msg attributes
          v_My_Log_Id  NUMBER :=0;
          v_Messageid   VARCHAR2(256);
          v_Envkey      VARCHAR2(256) := 'bp_ae@' ||
                                         Upper(Sys_Context('userenv',
                                                           'instance_name'));
         -- v_Logsource   VARCHAR2(256);
          v_Batchid     VARCHAR2(256) := 0;
          v_Message     VARCHAR2(256) := ' ';
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          --v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS

               SELECT vc.vckey AS Vckey, v_Processdate
               FROM   Bp_Ae.Lw_Virtualcard Vc
               INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm  ON  lm.Ipcode = Vc.Ipcode
               WHERE  1 = 1 AND
                       vc.isprimary = 1 AND
                       Lm.Lastactivitydate BETWEEN Add_Months(v_Processdate, -12) AND (v_Processdate);

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN

          v_My_Log_Id  := Utility_Pkg.Get_Libjobid();

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

          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AEO Connected Welcome Bonus';

         OPEN Process_Rule;
          LOOP
           FETCH Process_Rule BULK COLLECT   INTO v_Tbl LIMIT 10000;
           FORALL k IN 1 .. v_tbl.count


              INSERT INTO Lw_Pointtransaction
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
                    Createdate,
                    Expirationreason)
              VALUES
                   (Seq_Pointtransactionid.Nextval,
                    v_Tbl(k).Vckey,
                    v_Pointtypeid,
                    v_Pointeventid,
                    1 /*purchase*/,
                    v_Tbl(k).v_Processdate,
                    v_Tbl(k).v_Processdate,
                    500,
                    To_Date('12/31/2199', 'mm/dd/yyyy'),
                    'AEO Connected Welcome Bonus',
                    -1,
                    -1,
                    -1,
                    -1,
                    0 /*Pointsconsumed*/,
                    0,
                    NULL,
                    'PointsJob.AwardWelcomeBonus' /*Ptchangedby*/,
                    SYSDATE,
                    NULL)

              LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;


             COMMIT;

        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
          /* log end of job */
          v_Endtime := SYSDATE;
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          =>  v_Jobname);



     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure PointsJob.AwardWelcomeBonus: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index).Vckey ;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AwardWelcomeBonus',
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
                                   p_Logsource => 'AwardWelcomeBonus',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AwardWelcomeBonus ');


   END AwardWelcomeBonus;
 -- AEO-1227

     PROCEDURE Updatelastaememberpointssent(p_Dummy VARCHAR2) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(SYSDATE - 5 / 1440, 'mm/dd/yyyy hh24:mi:ss') -- sets 5 mins before the sysdate,to allow for vc procesing time overlap
          WHERE  cKey = 'LastAEMemberPointsSent';
          COMMIT;
     END Updatelastaememberpointssent;


    -- AEO-1223 Begin
   PROCEDURE RemovePointsOnHold(p_PointsOnHoldDate VARCHAR2, Retval IN OUT Rcursor) as

    --log job attributes
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE   := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256);
    v_Processid        NUMBER := 0;
    v_Errormessage     VARCHAR2(256);
    --log msg attributes
    v_Messageid   VARCHAR2(256);
    v_Envkey      VARCHAR2(256) := 'bp_ae@' || Upper(Sys_Context('userenv', 'instance_name'));
    v_Logsource   VARCHAR2(256);
    v_Batchid     VARCHAR2(256) := 0;
    v_Message     VARCHAR2(256) := ' ';
    v_Reason      VARCHAR2(256);
    v_Error       VARCHAR2(256);
    v_Trycount    NUMBER := 0;

    v_Points   FLOAT := -9999;

    cursor c is
      select p.pointtransactionid,
             p.parenttransactionid,
             p.vckey,
             v.ipcode,                  --AEO 1855 AH
             v.loyaltyidnumber,         --AEO 1855 AH
             p.points
      from bp_ae.lw_pointtransaction p  --AEO 1855 AH
      JOIN bp_ae.lw_virtualcard v ON v.vckey = p.vckey
      where 1=1
      and p.transactiontype = 3
      and p.transactiondate <= trunc(to_date(p_PointsOnHoldDate, 'mm/dd/yyyy')-14)
      --AND v.isprimary = 1;              --AEO 2066 LAPP
			AND p.transactiondate > to_date('9/6/2017', 'MM/DD/YYYY');  --AEO 2066 LAPP

    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;

    begin
      /*Logging*/
      v_My_Log_Id := Utility_Pkg.Get_Libjobid();
      v_Jobname   := Upper('RemovePointsOnHold');
      v_Logsource := v_Jobname;
      /* log start of job */
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => NULL,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);
      Utility_Pkg.Log_Process_Start(v_Jobname, v_Jobname, v_Processid);
      /*End of Logging*/

      open c;

      EXECUTE IMMEDIATE 'truncate table ats_pointsonhold'; --AEO-1855 AH

      loop
        fetch c bulk collect into rec limit 1000;
        for i in 1 .. rec.COUNT LOOP

          select COUNT(*) into v_Points
          from bp_ae.lw_pointtransaction p
          where 1=1
          and p.pointtransactionid = rec(i).parenttransactionid;

          IF (v_points<>0) THEN
              select p.pointsonhold into v_Points
              from bp_ae.lw_pointtransaction p
              where 1=1
              and p.pointtransactionid = rec(i).parenttransactionid;

              if v_Points = rec(i).points then
                update bp_ae.lw_pointtransaction p
                set p.pointsonhold = 0,
                    p.updatedate = SYSDATE  --AEO-1855
                where 1=1
                and p.pointtransactionid = rec(i).parenttransactionid;

                /* Remove transaciton type = 3 record */
                delete bp_ae.lw_pointtransaction p
                where 1=1
                and p.transactiontype = 3
                and p.pointtransactionid = rec(i).pointtransactionid;

                /* AEO-1855 AH Start */
                INSERT INTO bp_ae.ats_pointsonhold (
                    a_ipcode,
                    a_parenttransactionid,
                    a_pointtransactionid,
                    createdate
                ) VALUES (
                    rec(i).ipcode,
                    rec(i).parenttransactionid,
                    rec(i).pointtransactionid,
                    SYSDATE
                );
                /* AEO-1855 AH End */


               END if ;
          END IF;

        end loop;
        commit;

        exit when c%NOTFOUND;
      end loop;

      commit;

      if c%ISOPEN then
        close c;
      end if;
      v_Endtime   := SYSDATE;
      v_Jobstatus := 1;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                    p_Jobdirection     => v_Jobdirection,
                    p_Filename         => NULL,
                    p_Starttime        => v_Starttime,
                    p_Endtime          => v_Endtime,
                    p_Messagesreceived => v_Messagesreceived,
                    p_Messagesfailed   => v_Messagesfailed,
                    p_Jobstatus        => v_Jobstatus,
                    p_Jobname          => v_Jobname);
   exception
     when others then
       dbms_output.put_line(SQLERRM);
       v_Error          := substr(SQLERRM,1,255);
       v_Messagesfailed := v_Messagesfailed + 1;
       v_Endtime        := SYSDATE;
       Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                           p_Jobdirection     => v_Jobdirection,
                           p_Filename         => NULL,
                           p_Starttime        => v_Starttime,
                           p_Endtime          => v_Endtime,
                           p_Messagesreceived => v_Messagesreceived,
                           p_Messagesfailed   => v_Messagesfailed,
                           p_Jobstatus        => v_Jobstatus,
                           p_Jobname          => v_Jobname);
       Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                           p_Envkey    => v_Envkey,
                           p_Logsource => 'AEPOINTSJOBS',
                           p_Filename  => NULL,
                           p_Batchid   => v_Batchid,
                           p_Jobnumber => v_My_Log_Id,
                           p_Message   => v_Message,
                           p_Reason    => v_Reason,
                           p_Error     => v_Error,
                           p_Trycount  => v_Trycount,
                           p_Msgtime   => SYSDATE);
       Raise_Application_Error(-20002,
                               'Other Exception detected in ' ||
                               v_Jobname || ' ');
       ROLLBACK;
       RAISE;

  END RemovePointsOnHold;

  PROCEDURE ExpirePoints(p_Dummy VARCHAR2, Retval IN OUT Rcursor) as

    --log job attributes
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256);
    v_Processid        NUMBER := 0;
    v_Errormessage     VARCHAR2(256);
    --log msg attributes
    v_Messageid   VARCHAR2(256);
    v_Envkey      VARCHAR2(256) := 'bp_ae@' ||
                                   Upper(Sys_Context('userenv',
                                                     'instance_name'));
    v_Logsource   VARCHAR2(256);
    v_Batchid     VARCHAR2(256) := 0;
    v_Message     VARCHAR2(256) := ' ';
    v_Reason      VARCHAR2(256);
    v_Error       VARCHAR2(256);
    v_Trycount    NUMBER := 0;

    v_Points   FLOAT;

	c_ExpireCodeInactivity CONSTANT NUMBER := 1;

    cursor c is
      select p.pointtransactionid,
             v.ipcode
      from bp_ae.lw_pointtransaction p
      join bp_ae.lw_virtualcard v on v.vckey = p.vckey
      join bp_ae.lw_loyaltymember lm on lm.ipcode = v.ipcode
      where 1=1
      and trunc(lm.lastactivitydate) = trunc(sysdate - 375)       /* This may change in the future, but for now we'll use = instead of <= */
      and lm.memberstatus not in (2,3,5)              -- Don't want to get people that are terminated or already marked inactive
	  -- AOE-1861 adding 5 (Closed) to the ignored member status
      ;

    type c_type is table of c%ROWTYPE index by pls_integer;
    rec c_type;

    begin
      /*Logging*/
      v_My_Log_Id := Utility_Pkg.Get_Libjobid();
      --  v_dap_log_id := utility_pkg.get_LIBJobID();
      v_Jobname   := Upper('ExpirePoints');
      v_Logsource := v_Jobname;
      /* log start of job */
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                        p_Jobdirection     => v_Jobdirection,
                        p_Filename         => NULL,
                        p_Starttime        => v_Starttime,
                        p_Endtime          => v_Endtime,
                        p_Messagesreceived => v_Messagesreceived,
                        p_Messagesfailed   => v_Messagesfailed,
                        p_Jobstatus        => v_Jobstatus,
                        p_Jobname          => v_Jobname);
      Utility_Pkg.Log_Process_Start(v_Jobname, v_Jobname, v_Processid);
      open c;

      loop
        fetch c bulk collect into rec limit 1000;
        for i in 1 .. rec.COUNT loop

          update bp_ae.lw_pointtransaction p
          set p.expirationdate = sysdate,
              p.expirationreason = c_ExpireCodeInactivity,
              p.notes = 'Expired Points due to inactivity'
          where 1=1
          and p.pointtransactionid = rec(i).pointtransactionid
          and p.expirationdate > sysdate;

          update bp_ae.lw_loyaltymember lm
          set lm.memberstatus = 2,
              lm.memberclosedate = sysdate
          where 1=1
          and lm.ipcode = rec(i).ipcode;

        end loop;
        commit;

        exit when c%NOTFOUND;
      end loop;

      commit;

      if c%ISOPEN then
        close c;
      end if;
      v_Endtime   := SYSDATE;
      v_Jobstatus := 1;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                    p_Jobdirection     => v_Jobdirection,
                    p_Filename         => NULL,
                    p_Starttime        => v_Starttime,
                    p_Endtime          => v_Endtime,
                    p_Messagesreceived => v_Messagesreceived,
                    p_Messagesfailed   => v_Messagesfailed,
                    p_Jobstatus        => v_Jobstatus,
                    p_Jobname          => v_Jobname);
   exception
     when others then
       dbms_output.put_line(SQLERRM);
       v_Error          := SQLERRM;
       v_Messagesfailed := v_Messagesfailed + 1;
       v_Endtime        := SYSDATE;
       Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                           p_Jobdirection     => v_Jobdirection,
                           p_Filename         => NULL,
                           p_Starttime        => v_Starttime,
                           p_Endtime          => v_Endtime,
                           p_Messagesreceived => v_Messagesreceived,
                           p_Messagesfailed   => v_Messagesfailed,
                           p_Jobstatus        => v_Jobstatus,
                           p_Jobname          => v_Jobname);
       Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                           p_Envkey    => v_Envkey,
                           p_Logsource => 'AEPOINTSJOBS',
                           p_Filename  => NULL,
                           p_Batchid   => v_Batchid,
                           p_Jobnumber => v_My_Log_Id,
                           p_Message   => v_Message,
                           p_Reason    => v_Reason,
                           p_Error     => v_Error,
                           p_Trycount  => v_Trycount,
                           p_Msgtime   => SYSDATE);
       Raise_Application_Error(-20002,
                               'Other Exception detected in ' ||
                               v_Jobname || ' ');
       ROLLBACK;
       RAISE;
  END ExpirePoints;
     -- AEO-1223 End

PROCEDURE Memberpointsdelta_Connected(p_Dummy       VARCHAR2,
                                 p_Processdate DATE,
                                 b_Name        VARCHAR2,
                                 p_includetxns varchar2, -- AEO-2324
                                 Retval        IN OUT Rcursor) AS
          Pdummy                 VARCHAR2(2);
          Pprocessdate           DATE := p_Processdate;
          v_Sql1                 VARCHAR2(1000);
          v_Sql2                 VARCHAR2(1000);
          v_Sql3                 VARCHAR2(1000);
          v_Sql4                 VARCHAR2(1000);
          v_Sql5                 VARCHAR2(1000);
          v_Memberfilestarttime  DATE;
          v_Transactionstartdate DATE;
     BEGIN
       --   v_Sql1 := 'Create Table AE_CurrentPointTrans_Connected As Select * from lw_pointtransaction pt Where pt.transactiondate >= Add_Months(trunc(sysdate, ''Q''), -15)';
          v_Sql1 := 'CREATE TABLE AE_CurrentPointTrans_Connected
            COMPRESS FOR ALL OPERATIONS
            NOLOGGING
            PCTFREE  0
            INITRANS 1
            MAXTRANS 255
            STORAGE (
                INITIAL 16M NEXT 16M
                MINEXTENTS 1
                MAXEXTENTS UNLIMITED
                )
            AS
            Select rowkey,
                expirationdate,
                vckey,
                Pointtypeid,
                Ownerid,
                Pointawarddate,
                Transactiondate,
                Transactiontype,
                Pointeventid,
                Points,
                pointsonhold from lw_pointtransaction pt Where pt.transactiondate >= (Select to_date(to_char(value), ''mm/dd/yyyy'')  
            from lw_clientconfiguration cc where cc.ckey = ''UnexpiredPointsStartDate'') ';
          v_Sql2 := 'Select to_date(to_char(value), ''mm/dd/yyyy hh24:mi:ss'')  from lw_clientconfiguration cc where cc.ckey = ''LastAEMemberPointsSent''';
          v_Sql3 := 'Select to_date(to_char(value), ''mm/dd/yyyy'')  from lw_clientconfiguration cc where cc.ckey = ''CalculateTransactionStartDate''';
          v_Sql4 := 'CREATE INDEX AE_CURRENTPTSAEC_RKED  ON AE_CurrentPointTrans_Connected(rowkey,expirationdate)  ONLINE COMPUTE STATISTICS TABLESPACE bp_ae_idx1';
          v_Sql5 := 'CREATE INDEX AE_CURRENTPTSAEC_VCKY  ON AE_CurrentPointTrans_Connected(vckey)  ONLINE COMPUTE STATISTICS TABLESPACE bp_ae_idx1';
          EXECUTE IMMEDIATE 'Drop Table AE_CurrentPointTrans_Connected';
          EXECUTE IMMEDIATE v_Sql1;
          EXECUTE IMMEDIATE v_Sql4;
          EXECUTE IMMEDIATE v_Sql5;
 --Running of stats on AE_CurrentPointTransaction2 becasue it was recreated
          dbms_stats.gather_table_stats(ownname => 'bp_ae', tabname => 'AE_CurrentPointTrans_Connected',  cascade => TRUE );
          EXECUTE IMMEDIATE v_Sql2
               INTO v_Memberfilestarttime;
          EXECUTE IMMEDIATE v_Sql3
               INTO v_Transactionstartdate;
          Updatelastaememberpointssent(Pdummy);
          EXECUTE IMMEDIATE 'Truncate Table  Ae_Memberpointsdelta_Connected';
          Headerrecord_Mbrpoints_Conn(Pdummy,
                                       Pprocessdate,
                                       v_Memberfilestarttime,
                                       v_Transactionstartdate);
          INSERT  /*+ APPEND*/ INTO Ae_Memberpointsdelta_Connected
               (RECORDTYPE,
								CUSTOMERKEY,
								LOYALTYIDNUMBER,
								IPCODE,
								TOTALPOINTS,
								POINTSTONEXTREWARD,
								AVAILABLEPOINTS,
								PENDINGPOINTS,
								AvailableBraCredits,
								PendingBraCredits,
								TotalBraCredits,
								AvailableJeanCredits,
								PendingJeanCredits,
								TotalJeanCredits,
								NetSpendCurrentYear,
								NetSpendPreviousYear,
								NetSpendToCurrentTier,
								NetSpendToNextTier,
								NetSpendLifetime,
								PointCreditExpDate,
								MemberRewardID,
								PointTXNType,
								PointEventID,
								BasePoints,
								BonusPoints,
								BraCredits,
								JeanCredits,
								TXNDATE,
								TXNNUMBER,
								OrderNumber,
								STORENUMBER,
								REGISTERNUMBER)
               SELECT Mem.Recordtype, -- member header with primary card if any, else any other available card
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
											Mem.Ipcode,
                      Mem.Totalpoints,
                      Mem.Pointstonextreward,
											Mem.Availablepoints,
											Mem.Pendingpoints,
											Mem.Availablebracredits,
											Mem.Pendingbracredits,
											Mem.Totalbracredits,
											Mem.Availablejeancredits,
											Mem.Pendingjeancredits,
											Mem.Totaljeancredits,
											Mem.Netspendcurrentyear,
											Mem.Netspendpreviousyear,
											Mem.Netspendtocurrenttier,
											Mem.Netspendtonexttier,
											Mem.Netspendlifetime,
											Mem.Pointcreditexpdate,
											Null, Null, Null, Null,Null, Null, Null,
                      (CASE
                           WHEN b_Name = 'QTR' THEN
                            To_Char(Pprocessdate, 'MMDDYYYY')
                           ELSE
                            To_Char(Pprocessdate - 1, 'MMDDYYYY')
                      END) AS Txndate,
											Null, Null, Null, Null
               FROM   Aepointheaderdelta_Connected Mem
               UNION ALL
               SELECT
                'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Ipcode,
                      null as TotalPoints,
                      null as POINTSTONEXTREWARD,
                      null as AVAILABLEPOINTS,
                      null as PENDINGPOINTS,
                      null as AvailableBraCredits,
                      null as PendingBraCredits,
                      null as TotalBraCredits,
                      null as AvailableJeanCredits,
                      null as PendingJeanCredits,
                      null as TotalJeanCredits,
                      null as NetSpendCurrentYear,
                      null as NetSpendPreviousYear,
                      null as NetSpendToCurrentTier,
                      null as NetSpendToNextTier,
                      null as NetSpendLifetime,
                      null as PointCreditExpDate,
                      null as MemberRewardID,
                      Pt.Transactiontype as PointTXNType,
                      Pt.Pointeventid,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points')                    THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      (CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                            -- AEO-2668 Begin
                                           'AEO Connected Engagement Points'
                                            -- AEO-2668 End

                                           ) THEN
                            Nvl(Pt.Points, 0)
                      END) AS Bonuspoints,
                      case when p.name like 'Bra%' then Pt.points
                          else 0
                      end as BraCredits,
                      case when p.name like 'Jean%' then Pt.points
                          else 0
                      end as JeanCredits,
                      case when att.attributesetcode is not null THEN
                        To_Char(h.a_txndate, 'MMDDYYYY') end AS Txndate,
                      case when att.attributesetcode is not null THEN
                        h.a_Txnnumber end AS Txnnumber,
                      case when att.attributesetcode is not null THEN
                        h.a_Ordernumber end AS Ordernumber,
                      case when att.attributesetcode is not null THEN
                        h.a_Storenumber end AS Storenumber,
                      case when att.attributesetcode is not null THEN
                        h.a_Txnregisternumber end AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_Connected Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN AE_CurrentPointTrans_Connected Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txnheader h
               ON     Pt.Rowkey = h.a_Rowkey
               INNER JOIN lw_attributeset att
               ON     att.attributesetcode = Pt.Ownerid and attributesetname = 'TxnHeader'
               -- WHERE pt.transactiondate >= TRUNC(p_ProcessDate, 'Q') and pt.transactiondate < ADD_MONTHS(TRUNC(p_ProcessDate, 'Q'), 3)
               WHERE  (Pt.Pointawarddate) >=(v_Memberfilestarttime) -- AEO-2352 Begin-End
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
											AND Pt.Transactiontype <> 3
											AND Pt.TransactionType <> 4
                      and lower (p_includetxns)= 'true' -- AEO-2324
         UNION ALL
               SELECT 'T' AS Recordtype, -- point transactions against any detail items some promotions work only on detail items like Gift Card etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Ipcode,
                      null as TotalPoints,
                      null as POINTSTONEXTREWARD,
                      null as AVAILABLEPOINTS,
                      null as PENDINGPOINTS,
                      null as AvailableBraCredits,
                      null as PendingBraCredits,
                      null as TotalBraCredits,
                      null as AvailableJeanCredits,
                      null as PendingJeanCredits,
                      null as TotalJeanCredits,
                      null as NetSpendCurrentYear,
                      null as NetSpendPreviousYear,
                      null as NetSpendToCurrentTier,
                      null as NetSpendToNextTier,
                      null as NetSpendLifetime,
                      null as PointCreditExpDate,
                      null as MemberRewardID,
                      Pt.Transactiontype as PointTXNType,
                      Pt.Pointeventid,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points'
                                           ) THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      (CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                            -- AEO-2668 Begin
                                           'AEO Connected Engagement Points'
                                            -- AEO-2668 End
                                           ) THEN
                            Nvl(Pt.Points, 0)
                      END) AS Bonuspoints,
                      case when p.name like 'Bra%' then Pt.points
                          else 0
                      end as BraCredits,
                      case when p.name like 'Jean%' then Pt.points
                          else 0
                      end as JeanCredits,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_Connected Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN AE_CurrentPointTrans_Connected Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               INNER  JOIN Ats_Txndetailitem Dtl
               ON     Pt.Rowkey = Dtl.a_Rowkey
               WHERE  Pt.Pointawarddate >=     v_Memberfilestarttime -- AEO-2352 Begin-End
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
                      AND Pt.Transactiontype <> 3
					  AND Pt.TransactionType <> 4
                      and lower (p_includetxns)= 'true' -- AEO-2324
         UNION ALL
               SELECT 'T' AS Recordtype, -- txnheader independent point transactions, like Email, SMS bonuses etc
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Ipcode,
                      null as TotalPoints,
                      null as POINTSTONEXTREWARD,
                      null as AVAILABLEPOINTS,
                      null as PENDINGPOINTS,
                      null as AvailableBraCredits,
                      null as PendingBraCredits,
                      null as TotalBraCredits,
                      null as AvailableJeanCredits,
                      null as PendingJeanCredits,
                      null as TotalJeanCredits,
                      null as NetSpendCurrentYear,
                      null as NetSpendPreviousYear,
                      null as NetSpendToCurrentTier,
                      null as NetSpendToNextTier,
                      null as NetSpendLifetime,
                      null as PointCreditExpDate,
                      null as MemberRewardID,
                      Pt.Transactiontype as PointTXNType,
                      Pt.Pointeventid,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points') THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      (CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                            -- AEO-2668 Begin
                                           'AEO Connected Engagement Points'
                                            -- AEO-2668 End
                                                 ) THEN
                            Nvl(Pt.Points, 0)
                      END) AS Bonuspoints,
                      case when p.name like 'Bra%' then Pt.points
                          else 0
                      end as BraCredits,
                      case when p.name like 'Jean%' then Pt.points
                          else 0
                      end as JeanCredits,
                      To_Char(Pt.Transactiondate, 'MMDDYYYY') AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_Connected Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN AE_CurrentPointTrans_Connected Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               WHERE  Pt.Pointawarddate >=(v_Memberfilestarttime) -- AEO-2352 Begin-End
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
                      AND Pt.Rowkey IN ('-1')
                      AND Pt.Transactiontype <> 3
					  AND Pt.TransactionType <> 4
                      and lower (p_includetxns)= 'true' -- AEO-2324
		         UNION ALL
                SELECT
                'T' AS Recordtype, -- point transactions against any txnheader
                      Mem.Customerkey,
                      Mem.Loyaltyidnumber,
                      Mem.Ipcode,
                      null as TotalPoints,
                      null as POINTSTONEXTREWARD,
                      null as AVAILABLEPOINTS,
                      null as PENDINGPOINTS,
                      null as AvailableBraCredits,
                      null as PendingBraCredits,
                      null as TotalBraCredits,
                      null as AvailableJeanCredits,
                      null as PendingJeanCredits,
                      null as TotalJeanCredits,
                      null as NetSpendCurrentYear,
                      null as NetSpendPreviousYear,
                      null as NetSpendToCurrentTier,
                      null as NetSpendToNextTier,
                      null as NetSpendLifetime,
                      null as PointCreditExpDate,
                      case when Pt.Transactiontype = 4 then Pt.Rowkey
                        end as MemberRewardID,
                      Pt.Transactiontype as PointTXNType,
                      Pt.Pointeventid,
                      CASE
                           WHEN p.Name IN ('AEO Visa Card Points',
                                           'AEO Connected Points',
                                           'Basic Points')                    THEN
                            Nvl(Pt.Points, 0)
                      END AS Basepoints,
                      (CASE
                           WHEN p.Name IN ('AEO Customer Service Points',
                                           'AEO Connected Bonus Points',
                                            -- AEO-2668 Begin
                                           'AEO Connected Engagement Points'
                                            -- AEO-2668 End
                                           ) THEN
                            Nvl(Pt.Points, 0)
                      END) AS Bonuspoints,
                      case when p.name like 'Bra%' then Pt.points
                          else 0
                      end as BraCredits,
                      case when p.name like 'Jean%' then Pt.points
                          else 0
                      end as JeanCredits,
                      To_Char(pt.transactiondate, 'MMDDYYYY')  AS Txndate,
                      NULL AS Txnnumber,
                      NULL AS Ordernumber,
                      NULL AS Storenumber,
                      NULL AS Registernumber
               FROM   Lw_Virtualcard Vc
               INNER  JOIN Aepointheaderdelta_Connected Mem
               ON     Mem.Ipcode = Vc.Ipcode
               INNER  JOIN AE_CurrentPointTrans_Connected Pt
               ON     Vc.Vckey = Pt.Vckey
               INNER  JOIN Lw_Pointtype p
               ON     Pt.Pointtypeid = p.Pointtypeid
               LEFT JOIN Lw_Pointevent pe  --AEO-2566
               ON     Pt.Pointeventid = pe.pointeventid --AEO-2566
               LEFT JOIN BP_AE.LW_MEMBERREWARDS Mr --AEO-2566
			         ON     Mr.ID = Pt.ROWKEY
               WHERE  --Pt.Transactiondate >= v_Memberfilestarttime
                      Pt.Pointawarddate >= v_Memberfilestarttime  -- AEO-2264
                      AND Pt.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
                      AND Pt.Expirationdate > p_Processdate
                      AND Pt.Transactiontype <> 3
                      AND (Mr.ID IS NOT null OR (Mr.ID IS null AND pe.name = 'Member Inactivation')) --AEO-2566
					  and lower (p_includetxns)= 'true' -- AEO-2324
               ORDER  BY Loyaltyidnumber, Recordtype;
          COMMIT;
     END Memberpointsdelta_Connected;


PROCEDURE Headerrecord_Mbrpoints_Conn(p_Dummy                VARCHAR2,
                                            p_Processdate          DATE,
                                            p_Memberfilestarttime  DATE,
                                            v_Transactionstartdate DATE) IS
          v_Sql1 VARCHAR2(1000);
     BEGIN
	      EXECUTE IMMEDIATE 'Truncate Table AEPOINTHEADERDELTA_CONNECTED';
          INSERT /*+ APPEND*/ INTO AEPOINTHEADERDELTA_CONNECTED
               (IPCODE,
									RECORDTYPE,
									CUSTOMERKEY,
									LOYALTYIDNUMBER,
									POINTSTONEXTREWARD,
									TOTALPOINTS,
									AVAILABLEPOINTS,
									PENDINGPOINTS,
									AvailableBraCredits,
									PendingBraCredits,
									TotalBraCredits,
									AvailableJeanCredits,
									PendingJeanCredits,
									TotalJeanCredits,
									NetSpendCurrentYear,
									NetSpendPreviousYear,
									NetSpendToCurrentTier,
									NetSpendToNextTier,
									NetSpendLifetime,
									PointCreditExpDate)
--             SELECT /*+ parallel(pts 8) */   AEO 598 changes  here ---------------SCJ
               SELECT
                Lm.Ipcode,
                'H' AS Recordtype,
                MAX(Vc2.Linkkey) AS Customerkey,
                Vc2.Loyaltyidnumber,
				--AEO-2083 PointsToNextReward = 2500 - MOD(TotalPoints, 2500)
                2500 - MOD(Greatest(
                             SUM(CASE
                                   WHEN Ty.Name IN ('AEO Connected Points',
                                             'AEO Connected Bonus Points',
                                             -- AEO-2668 Begin
                                             'AEO Connected Engagement Points',
                                             -- AEO-2668 End
                                             'AEO Visa Card Points',
                                             'AEO Customer Service Points')
                                    AND Pts.Expirationdate > p_Processdate
                                   THEN Pts.Points
                                   ELSE 0 END)
                                  ,0)
                        , 2500) as Pointstonextreward,
                /* PointsToNextReward = 2500 - MOD(AvailablePoint, 2500)
                2500 - MOD(Greatest(
                             SUM(CASE
                                   WHEN Ty.Name IN ('AEO Connected Points',
                                             'AEO Connected Bonus Points',
                                             'AEO Visa Card Points',
                                             'AEO Customer Service Points')
                                    AND Pts.Expirationdate > p_Processdate
                                   THEN Pts.Points
                                   ELSE 0 END)
                                  ,0) -
                           Greatest(
                             SUM(CASE
                                   WHEN Ty.Name IN ('AEO Connected Points',
                                             'AEO Connected Bonus Points',
                                             'AEO Visa Card Points',
                                             'AEO Customer Service Points')
                                    AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0
                                   THEN Pts.pointsonhold
                                   ELSE 0 END)
                                 ,0)
                        , 2500) as Pointstonextreward, */
                /*TotalPoints, */
								Greatest(SUM(CASE
															 WHEN Ty.Name IN ('AEO Connected Points',
															'AEO Connected Bonus Points',
                              -- AEO-2668 Begin
                              'AEO Connected Engagement Points',
                              -- AEO-2668 End
															'AEO Visa Card Points',
															'AEO Customer Service Points')
															AND Pts.Expirationdate > p_Processdate THEN
															Pts.Points
															ELSE
																0
								       END),
                0) AS Totalpoints,
                --AvailablePoints = Total - Pending
                  Greatest(SUM(CASE
                               WHEN Ty.Name IN ('AEO Connected Points',
                              'AEO Connected Bonus Points',
                              'AEO Visa Card Points',
                              -- AEO-2668 Begin
                              'AEO Connected Engagement Points',
                              -- AEO-2668 End
                              'AEO Customer Service Points')
                              AND Pts.Expirationdate > p_Processdate THEN
                              Pts.Points
                              ELSE
                                0
                       END),0) -
                  Greatest(SUM(CASE
                         WHEN Ty.Name IN ('AEO Connected Points',
                            'AEO Connected Bonus Points',
                            'AEO Visa Card Points',
                             -- AEO-2668 Begin
                            'AEO Connected Engagement Points',
                             -- AEO-2668 End
                            'AEO Customer Service Points')
                            AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0 THEN
                              Pts.pointsonhold
                            ELSE
                              0
                          END),0)
                AS AvailablePoints,
                --PendingPoints
                Greatest(SUM(CASE
                         WHEN Ty.Name IN ('AEO Connected Points',
														'AEO Connected Bonus Points',
														'AEO Visa Card Points',
                            -- AEO-2668 Begin
                            'AEO Connected Engagement Points',
                            -- AEO-2668 End
       									   	'AEO Customer Service Points')
                            AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0 THEN
														  Pts.pointsonhold
													  ELSE
															0
													END),
								0) as PendingPoints,
								--AvailableBraCredits = TotalBraCredits - PendingBraCredits
                  Greatest(SUM(CASE
                             WHEN Ty.Name LIKE 'Bra%'
                                      AND Pts.Expirationdate > p_Processdate THEN
                                      Pts.Points
                             ELSE
                               0
                        END),
                      0) -
									Greatest(SUM(CASE
                            WHEN Ty.Name LIKE 'Bra%'
                                 AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0 THEN
                                 Pts.pointsonhold
                            ELSE
                                 0
                       END),
                      0)
								  as AvailableBraCredits,
								--PendingBraCredits
								Greatest(SUM(CASE
								            WHEN Ty.Name LIKE 'Bra%'
												         AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0 THEN
												         Pts.pointsonhold
												    ELSE
													       0
											 END),
							  0) as PendingBraCredits,
								--TotalBraCredits,
                Greatest(SUM(CASE
                             WHEN Ty.Name LIKE 'Bra%'
                                      AND Pts.Expirationdate > p_Processdate THEN
                                      Pts.Points
                             ELSE
															 0
												END),
                0) as TotalBraCredits,
								--AvailableJeanCredits = TotalJeanCredits - PendingJeanCredits
                  Greatest(SUM(CASE
                             WHEN Ty.Name LIKE 'Jean%'
                                  AND Pts.Expirationdate > p_Processdate THEN
                                    Pts.Points
                             ELSE
                               0
                             END),
                  0) -
									Greatest(SUM(CASE
                             WHEN Ty.Name LIKE 'Jean%'
                                  AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0 THEN
                                      Pts.pointsonhold
                             ELSE
                               0
                             END),
                  0)
								  as AvailableJeanCredits,
								--PendingJeanCredits,
                Greatest(SUM(CASE
                             WHEN Ty.Name LIKE 'Jean%'
                                  AND Pts.Expirationdate > p_Processdate AND Pts.pointsonhold > 0 THEN
                                      Pts.pointsonhold
                             ELSE
															 0
                             END),
                0) as PendingJeanCredits,
								--TotalJeanCredits,
                Greatest(SUM(CASE
                             WHEN Ty.Name LIKE 'Jean%'
                                  AND Pts.Expirationdate > p_Processdate THEN
																	  Pts.Points
                             ELSE
															 0
                             END),
                0) as TotalJeanCredits,
                --  NetSpendCurrentYear,
								max(Md.A_NetSpend) as NetSpendCurrentYear,
                --  NetSpendPreviousYear,
								max(Md.A_PrevNetSpend) as NetSpendPreviousYear,
                --  NetSpendToCurrentTier,
                MIN(CASE WHEN Ti.TierName = 'Extra Access' THEN (Case When 350 - nvl(Md.A_NetSpend, 0) > 0 THEN  350 - nvl(Md.A_NetSpend, 0) Else 0 End) ELSE 0 END)
                as NetSpendToCurrentTier,
                --  NetSpendToNextTier,
                MIN(CASE WHEN Ti.TierName = 'Full Access' THEN (Case When 350 - nvl(Md.A_NetSpend,0) > 0 THEN 350 - nvl(Md.A_NetSpend,0) Else 0 End) ELSE 0 END)
                as NetSpendToNextTier,
                /*--  NetSpendToCurrentTier,
                MIN(CASE WHEN Ti.TierName = 'Extra Access' THEN nvl(Md.A_NetSpend, 0) - 350 ELSE 0 END)
                as NetSpendToCurrentTier,
                --  NetSpendToNextTier,
                MIN(CASE WHEN Ti.TierName = 'Full Access' THEN 350 - nvl(Md.A_NetSpend,0) ELSE 0 END)
                as NetSpendToNextTier, */
                --  NetSpendLifetime,
								Max(Md.A_LifeTimeNetSpend) as NetSpendLifetime,
                --  PointCreditExpDate
								To_Char(max(LM.LastActivityDate + 375), 'MMDDYYYY') as PointCreditExpDate
               FROM   AE_CurrentPointTrans_Connected Pts
               INNER  JOIN Lw_Virtualcard Vc
               ON     Vc.Vckey = Pts.Vckey
               INNER  JOIN Lw_Pointtype Ty
               ON     Ty.Pointtypeid = Pts.Pointtypeid
               INNER  JOIN Lw_Loyaltymember Lm
               ON     Lm.Ipcode = Vc.Ipcode
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
                     -- AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
               INNER  JOIN (SELECT *
                            FROM   Lw_Virtualcard Vc1
                            WHERE  Vc1.Isprimary = 1) Vc2
               ON     Vc.Ipcode = Vc2.Ipcode
               INNER  JOIN (SELECT DISTINCT (Vc5.Ipcode)
                            FROM   Lw_Virtualcard Vc5
                            INNER  JOIN (SELECT DISTINCT (Pta.Vckey)
                                        FROM   AE_CurrentPointTrans_Connected Pta
                                        WHERE  Pta.Pointawarddate >=
                                               p_Memberfilestarttime) Ptb
                            ON     Vc5.Vckey = Ptb.Vckey
                            UNION
                            SELECT Dr.a_Ipcode
                            FROM   ats_pointsonhold Dr
                            WHERE  Dr.Createdate >= p_Memberfilestarttime) Vc6
               ON     Vc.Ipcode = Vc6.Ipcode
               INNER  JOIN Lw_Pointtype Pnt
               ON     Pnt.Pointtypeid = Pts.Pointtypeid
							 INNER JOIN LW_MEMBERTIERS mt
                     on lm.ipcode = mt.memberid and sysdate > mt.fromdate and sysdate < mt.todate
               INNER JOIN LW_TIERS Ti
                     on Ti.Tierid = mt.tierid
               WHERE  Pts.Transactiondate <
                      Add_Months(Trunc(p_Processdate, 'Mon'), 1)
											AND lm.memberstatus = 1
											--AEO-1675 Ignore the transaction type for points on hold
											AND Pts.TransactionType <> 3
               GROUP  BY Lm.Ipcode, Vc2.Loyaltyidnumber, Md.a_Extendedplaycode
							 HAVING COUNT(Pts.Vckey) > 0;  --TxnNumber > 0
          COMMIT;
     END Headerrecord_Mbrpoints_Conn;


-- Urgent changes
     PROCEDURE changelastaememberpointssent(p_date VARCHAR2) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = to_char(to_date( p_date , 'mm/dd/yyyy hh24:mi:ss'),'mm/dd/yyyy hh24:mi:ss')
          WHERE  cKey = 'LastAEMemberPointsSent';
          COMMIT;
     END changelastaememberpointssent;

 PROCEDURE ChangeUnexpiredPointsStart(p_date VARCHAR2) AS
     BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = to_char(to_date( p_date , 'mm/dd/yyyy'),'mm/dd/yyyy')
          WHERE  cKey = 'UnexpiredPointsStartDate';
          COMMIT;
     END ChangeUnexpiredPointsStart;
     -- Urgent changes

     PROCEDURE ProcessBraCodes(p_ProcessFrom           VARCHAR2,
												p_ProcessTo           VARCHAR2,
                        p_BraCodes       VARCHAR2,
                        p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          v_My_Log_Id  NUMBER :=0;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
          v_Jobdirection     NUMBER := 0;
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256):= 'ProcessBraCodes' ;
          v_Filename         VARCHAR2(512):= v_jobname;
          v_Processid        NUMBER := 0;
          v_Errormessage     VARCHAR2(256);
          --log msg attributes
          v_Messageid   VARCHAR2(256);
          v_Envkey      VARCHAR2(256) := 'bp_ae@' ||
                                         Upper(Sys_Context('userenv',
                                                           'instance_name'));
          v_Logsource   VARCHAR2(256);
          v_Batchid     VARCHAR2(256) := 0;
          v_Message     VARCHAR2(256) := ' ';
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
					v_ProcessFrom DATE;
					v_ProcessTo DATE;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                  td.a_rowkey AS Originalrowkey,
                  td.a_txndate           AS Txndate,
                  th.a_txntypeid         AS v_Txntypeid,
                  sysdate p_Processdate,
                  Case
                  When trunc(td.a_txndate) >= trunc(sysdate - 14) and
                     td.a_dtlsaleamount > 0 then
                   1
                  else
                   0
                  End as v_onhold --Pointsonhold flag
               FROM bp_ae.ats_txndetailitem td
               INNER JOIN Lw_Virtualcard vc
--               ON     td.a_ipcode = vc.vckey and vc.isprimary = 1 --AEO2649
               ON     td.a_vckey = vc.vckey and vc.isprimary = 1 --AEO2649
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               INNER JOIN BP_AE.ATS_TXNHEADER TH
               ON     td.a_parentrowkey = TH.a_Rowkey
               INNER  JOIN (select t.COLUMN_VALUE as classcodes from table(StringToTable(p_BraCodes,';')) t ) Bp
               ON     Bp.classcodes = td.a_dtlclasscode
               WHERE  1 = 1
                      AND td.a_dtlsaleamount > .01
                      AND nvl(td.a_dtlclearanceitem,0) = 0
                      AND th.a_txntypeid = 1
                      AND td.a_txndate BETWEEN v_ProcessFrom AND v_ProcessTo;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
					v_ProcessFrom := to_date(p_ProcessFrom, 'MM/DD/YYYY');
					v_ProcessTo := to_date(p_ProcessTo, 'MM/DD/YYYY');

          v_My_Log_Id  := Utility_Pkg.Get_Libjobid();

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

          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Bra Purchase';
         OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
           FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then

                    INSERT INTO Lw_Pointtransaction
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
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          1 /*Bra Purchase Point Pointsonhold*/,
                          NULL,
                          'ProcessBraCodes' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          returning Pointtransactionid into v_Pointtransactionid;
                    --Insert PointsOnHold Record
                    INSERT INTO Lw_Pointtransaction
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
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'ProcessBraCodes' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;

               elsif (v_Tbl(k).v_onhold = 0) then

                    INSERT INTO Lw_Pointtransaction
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
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'ProcessBraCodes' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
			                    LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               end if;
							 v_Messagesreceived:= v_Messagesreceived+1;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;

          v_Endtime   := SYSDATE;
          v_Jobstatus := 1;
          Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => NULL,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure ProcessBraCodes: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'ProcessBraCodes',
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
                                   p_Logsource => 'ProcessBraCodes',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ProcessBraCodes ');
     END ProcessBraCodes;

END AEPointsJobs;
/
