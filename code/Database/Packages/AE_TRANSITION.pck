CREATE OR REPLACE PACKAGE AE_TRANSITION IS
-- AEO-1634 begin
 TYPE RCURSOR IS REF CURSOR;

  PROCEDURE LoadSynchronyAccountKeys(retval IN OUT RCURSOR);

  PROCEDURE LoadSynchronyAccountKeys_Bk(retval IN OUT RCURSOR);

  PROCEDURE LoadSynchronyMembers   ( p_filename VARCHAR2  ,
                                    RETVAL     IN OUT RCURSOR) ;

  PROCEDURE SelectTransitionMembers( p_procdate  VARCHAR2 ,
                                      p_mod  INTEGER,
                                      p_instance  INTEGER,
                                      RETVAL  IN OUT RCURSOR ) ;

  PROCEDURE UpdateTransitionMembers(retval IN OUT RCURSOR);


-- AEO-1634 end

--AEO-1637 Begin
/*DEV - Transition - Expire all points*/

       --Clean Temp table
       PROCEDURE Clear_TempTable(p_Type INTEGER);

       --Stage data per months
       PROCEDURE Get_Points_Transactions(p_ProcessDate1 IN VARCHAR2, p_ProcessDate2 IN VARCHAR2, p_Line IN VARCHAR2);

       --Fill out temp table with points unexpired
       PROCEDURE Stage_DataEntry(p_ProcessDate1 IN TIMESTAMP, p_ProcessDate2 IN TIMESTAMP);

       --Expire points
       PROCEDURE ExpireAllPoints(p_mod INTEGER, p_instances INTEGER);

       --UnExpire points
       PROCEDURE UnExpireAllPoints(p_mod INTEGER, p_instances INTEGER);

       --Create or remove index to temp table
       PROCEDURE Index_TempTable(p_Type INTEGER);

       --Delete records from temp table
       PROCEDURE Delete_PointsTempTable(p_ProcessDate1 IN VARCHAR2, p_ProcessDate2 IN VARCHAR2, p_Line IN VARCHAR2);
--AEO-1637 End

--AEO-1638 Begin
/*DEV - Transition - Set Default Tiers*/
       PROCEDURE Set_DefaultTier(p_mod INTEGER, p_instances INTEGER);

       PROCEDURE Restore_Tier(p_mod INTEGER, p_instances INTEGER);

       PROCEDURE Update_DuplicateTiers;

       PROCEDURE Restore_DuplicateTiers;

       PROCEDURE Add_NewSpend(p_mod INTEGER, p_instances INTEGER);

       PROCEDURE Delete_NewSpend(p_mod INTEGER, p_instances INTEGER);
--AEO-1638 End

	   /*AEO-1718 Customer Service Agent Update Process*/
       PROCEDURE LoadCSAgents(p_filename IN VARCHAR2);

/* AEO-1635 BEGIN DEV - Transition - Welcome Bonus */
       PROCEDURE BulkBonusLoader(p_fileName IN NVARCHAR2, retval IN OUT rcursor);
/* AEO-1635 END */

/* AEO-1844 BEGIN - AH*/
       PROCEDURE UpdateSignUpBonus(retval IN OUT RCURSOR);
/* AEO-1844 END */

END AE_TRANSITION;
/
CREATE OR REPLACE PACKAGE BODY AE_TRANSITION IS

--AEO-2169 Begin - GD
  FUNCTION validate_row_BonusLoad(P_Bonus_Rec IN OUT TYPE_BONUSLOAD) RETURN NUMBER AS
   lsTmpval       VARCHAR(255) := NULL;
   lbRetVal       NUMBER := 0;
   v_ValidBonus   NUMBER := 1;
   v_ValidLYID    NUMBER := 0;
   v_VcKey        NUMBER := 0;
   v_MsgLYID      VARCHAR2(255) := NULL;
   V_ErrorMessage VARCHAR2(256) := NULL;
   V_PTNotes      VARCHAR2(255) := NULL;
   v_Found        NUMBER := 0;

   BEGIN
    -- Loyalty number Valid
       utility_pkg.validate_loyaltynumber(P_Bonus_Rec.LOYALTYNUMBER ,v_MsgLYID ,v_ValidLYID) ;
       IF v_ValidLYID = 0  THEN
         lbRetVal := -1;
         V_ErrorMessage := V_ErrorMessage || ' | ' || v_MsgLYID;
       ELSE
        --LYID exist on database?
          SELECT COUNT(*)
          INTO v_Found
          FROM bp_ae.lw_virtualcard vc
          WHERE 1=1
          AND vc.loyaltyidnumber = P_Bonus_Rec.LOYALTYNUMBER
          ;

          IF v_Found = 0 THEN
            v_ValidLYID := 0; --LYID Not Found
            lbRetVal := -1;
            V_ErrorMessage := V_ErrorMessage  || ' | ' || 'Member ' || P_Bonus_Rec.LOYALTYNUMBER || ' does not exist in the database';
          ELSE
            SELECT vc.vckey
            INTO v_VcKey
            FROM bp_ae.lw_virtualcard vc
            WHERE 1=1
            AND vc.loyaltyidnumber = P_Bonus_Rec.LOYALTYNUMBER
            ;
          
            v_ValidLYID := 1; --LYID Found
            P_Bonus_Rec.VCKEY := v_VcKey; --Update data
          END IF;
       END IF;

   -- Bonus
       IF (P_Bonus_Rec.POINTS IS NULL )THEN
         v_ValidBonus := 0;
         lbRetVal := -1;
         V_ErrorMessage := V_ErrorMessage  || ' | ' || 'Bonus Points is Null';
       END IF;

       IF NOT REGEXP_LIKE(P_Bonus_Rec.POINTS,'[[:digit:]]') THEN
         v_ValidBonus := 0;
         lbRetVal := -1;
         V_ErrorMessage := V_ErrorMessage  || ' | ' || 'Bonus Points is not digit';
       END IF;

    -- Record duplicated in the same file
       IF (v_ValidLYID <> 0) AND (v_ValidBonus <> 0) AND (P_Bonus_Rec.NUMREC <> 1) THEN
             lbRetVal := -1;
             V_ErrorMessage := V_ErrorMessage || ' | ' || 'Member ' || P_Bonus_Rec.LOYALTYNUMBER || ' duplicated';
       END IF;

    --Save if there is an exception
       IF (lbRetVal = -1) THEN
         INSERT INTO Bp_Ae.AE_BONUSLOADER_EXCEPTION
                        ( POINTTYPE
                        , POINTEVENT
                        , LOYALTYNUMBER
                        , POINTS
                        , EXCEPTIONMSG
                        , FILENAME
                        , CREATEDATE
                        , CHANGEDBY
                        )
         VALUES
                        ( P_Bonus_Rec.POINTTYPE
                        , P_Bonus_Rec.POINTEVENT
                        , P_Bonus_Rec.LOYALTYNUMBER
                        , P_Bonus_Rec.POINTS
                        , V_ErrorMessage
                        , P_Bonus_Rec.FILENAME
                        , SYSDATE
                        , 'BulkBonusLoader'
                        );

       END IF;

       COMMIT;
       RETURN lbRetVal;

  EXCEPTION
    WHEN OTHERS THEN
        lbRetVal :=-1;
        lstmpval:=  SUBSTR(SQLERRM, 1, 200);
        COMMIT;

    RETURN lbRetVal;
  END;
  
  FUNCTION Initialize_Bonus_Rec RETURN TYPE_BONUSLOAD IS
    /* resets the array */
  BEGIN
    RETURN TYPE_BONUSLOAD(NULL,
                          NULL,
                          NULL,
                          NULL,
                          NULL,
                          NULL,
                          NULL,
                          NULL
                          );
  END Initialize_Bonus_Rec;
  
--AEO-2169 End - GD

-- AEO-1634 begin
   /********* Procedure to map external table to specified file ********/
   /********* Procedure to map external table to specified file ********/
  PROCEDURE ChangeExternalTable(P_FILENAME IN VARCHAR2, p_tablename IN VARCHAR2) IS
    E_MTABLE    EXCEPTION;
    E_MFILENAME EXCEPTION;
    V_SQL VARCHAR2(400);
  BEGIN

    IF LENGTH(TRIM(P_FILENAME)) = 0 OR P_FILENAME IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001,
                              'Filename is required to link with external table',
                              FALSE);
    END IF;

    V_SQL := 'ALTER TABLE '|| p_tablename||' LOCATION (AE_IN' || CHR(58) || '''' ||
             P_FILENAME || ''')';
    EXECUTE IMMEDIATE V_SQL;

  END CHANGEEXTERNALTABLE;


 PROCEDURE clear_infile(p_tablename IN VARCHAR2) IS

    lv_fileHandle UTL_FILE.FILE_TYPE;
    lv_path       VARCHAR2(50) DEFAULT 'USER_EXPORT';
    lv_filename   VARCHAR2(512);

  BEGIN
    SELECT LOCATION, DIRECTORY_NAME
      INTO LV_FILENAME, LV_PATH
      FROM USER_EXTERNAL_LOCATIONS t
     WHERE UPPER(t.table_name) = UPPER(p_tablename);

    lv_fileHandle := UTL_FILE.FOPEN(lv_path, LV_FILENAME, 'W', 32000);

    --utl_file.put_line( lv_fileHandle, lv_colString );
    --utl_file.new_line(lv_filehandle);
    UTL_FILE.FCLOSE(lv_fileHandle);

  END clear_infile;

/*******************************************************************/
 PROCEDURE LoadSynchronyMembers( p_filename VARCHAR2 ,
                                 retval  IN OUT RCURSOR) IS

    V_MY_LOG_ID NUMBER;

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := P_FILENAME;
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
    V_FILENAME  VARCHAR2(256) := P_FILENAME;
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

  BEGIN

-- change the external file linked to the extenral table
-- and clear the log file
--
  CHANGEEXTERNALTABLE(P_FILENAME,'Ext_Synchrony');
   clear_infile('EXT_SYNCHRONY_LOG');

   V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();


   V_JOBNAME   := 'LoadSynchronyMembers';
   V_LOGSOURCE := V_JOBNAME;

    /* log start of job */
   UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => 'stage' || V_JOBNAME);


    V_ERROR          :=  ' ';
    V_REASON         := 'truncating tables start';
    V_MESSAGE        := 'truncating tables start';
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

    -- clear out the wroking table
    -- this to make the stored procedure
    -- abel to be executed more than on time.
    EXECUTE IMMEDIATE 'Truncate Table ae_synchrony';




   V_ERROR          :=  ' ';
   V_REASON         := 'truncating tables end';
   V_MESSAGE        := 'truncating tables end';
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
  ----
  ---  We had to define two cursor because the re could be more thant one record for the same LYID in the incomeing file
  ---- what i'm doing is that the most recent (that with the hiughest value in the CID field) is inserted
  ---- the others are used to update the information

    DECLARE
    -- cusor to get the rows from the external table
    -- that are going to be inserted on the work table

      CURSOR GET_DATA IS
        SELECT *
                FROM   Bp_Ae.Ext_Synchrony p
                WHERE p.cardholder_loyalty_id IS NOT NULL AND length(trim(p.cardholder_loyalty_id))= 14 AND
                      p.cardholder_loyalty_id <> '00000000000000' AND length(nvl(p.middle_initial,'x')) = 1
                ORDER BY p.cardholder_loyalty_id DESC ,p.Acct_Key DESC;


      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;



    BEGIN

    V_ERROR          :=  ' ';
    V_REASON         := 'Write to ae_synchrony start';
    V_MESSAGE        := 'Write to ae_synchrony start';
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
          INTO V_TBL LIMIT 1000;

        FORALL I IN 1 .. V_TBL.COUNT

          MERGE INTO Bp_Ae.Ae_Synchrony Sync
          USING (
              SELECT v_Tbl(i).Acct_Key AS Acct_Key,
                    v_Tbl(i).Cardholder_Loyalty_Id AS Loyaltynumber,
                    v_Tbl(i).Fname AS Fname,
                    v_Tbl(i).Middle_Initial AS Middle_Initial,
                    v_Tbl(i).Lname AS Lname,
                    v_Tbl(i).Cardholder_Addr1 AS Cardholder_Addr1,
                    v_Tbl(i).Cardholder_Addr2 AS Cardholder_Addr2,
                    v_Tbl(i).Cardholder_City AS Cardholder_City,
                    v_Tbl(i).Cardholder_State AS Cardholder_State,
                    v_Tbl(i).Cardholder_Zip AS Cardholder_Zip,
                    v_Tbl(i).Cardholder_Email AS Cardholder_Email,
                    v_Tbl(i).Cardholder_Product AS Cardholder_Product,
                    v_Tbl(i).Pilot_Flag AS Pilot_Flag
                     FROM   Dual) PT
          ON (Sync.Cardholder_Loyalty_Id = Pt.Loyaltynumber)
          WHEN NOT MATCHED THEN
               INSERT
                    (Acct_Key,
                     Cardholder_Loyalty_Id,
                     Fname,
                     Middle_Initial,
                     Lname,
                     Cardholder_Addr1,
                     Cardholder_Addr2,
                     Cardholder_City,
                     Cardholder_State,
                     Cardholder_Zip,
                     Cardholder_Email,
                     Cardholder_Product,
                     Pilot_Flag)
               VALUES
                    (Pt.Acct_Key,
                     Pt.Loyaltynumber,
                     Pt.Fname,
                     Pt.Middle_Initial,
                     Pt.Lname,
                     Pt.Cardholder_Addr1,
                     Pt.Cardholder_Addr2,
                     Pt.Cardholder_City,
                     Pt.Cardholder_State,
                     Pt.Cardholder_Zip,
                     Pt.Cardholder_Email,
                     Pt.Cardholder_Product,
                     Pt.Pilot_Flag)
          WHEN MATCHED THEN
               UPDATE SET  Acct_Key           = Pt.Acct_Key,
                      Fname              = Pt.Fname,
                      Middle_Initial     = Pt.Middle_Initial,
                      Lname              = Pt.Lname,
                      Cardholder_Addr1   = Pt.Cardholder_Addr1,
                      Cardholder_Addr2   = Pt.Cardholder_Addr2,
                      Cardholder_City    = Pt.Cardholder_City,
                      Cardholder_State   = Pt.Cardholder_State,
                      Cardholder_Zip     = Pt.Cardholder_Zip,
                      Cardholder_Email   = Pt.Cardholder_Email,
                      Cardholder_Product = Pt.Cardholder_Product,
                      Pilot_Flag         = Pt.Pilot_Flag;


        COMMIT WRITE batch NOWAIT;
        EXIT WHEN GET_DATA%NOTFOUND;
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
       CLOSE GET_DATA;
    END IF;


    V_ERROR          :=  ' ';
    V_REASON         := 'Write to ae_synchrony end';
    V_MESSAGE        := 'Write to ae_synchrony end';
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

----
    DECLARE
      /* check log file for errors */
      LV_ERR VARCHAR2(4000);
      LV_N   NUMBER;
    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM EXT_SYNCHRONY_LOG' || CHR(10) ||
                        'WHERE rec LIKE ''ORA-%'''
        INTO LV_N, LV_ERR;

      IF LV_N > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        V_MESSAGESFAILED := V_MESSAGESFAILED + LV_N;
        V_REASON         := 'Failed reads by external table';
        V_MESSAGE        := '<StageProc>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>EXT_SYNCHRONY' ||
                            CHR(10) ||
                            CHR(10) || '    <FileName>' || P_FILENAME ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</StageProc>';
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => V_LOGSOURCE,
                            P_FILENAME  => P_FILENAME,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => LV_ERR,
                            P_TRYCOUNT  => LV_N,
                            P_MSGTIME   => SYSDATE);
      END IF;
    END;

    /* insert here */
    V_ENDTIME   := SYSDATE;
    V_JOBSTATUS := 1;

    /* log end of job */
    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          =>  V_JOBNAME);

    OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;
  EXCEPTION
    WHEN DML_ERRORS THEN

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := substr(SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE),1,255);
        V_REASON         := 'Failed Records in Procedure LoadSynchronyMembers: ';
        V_MESSAGE        := ' ';

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

      END LOOP;
      COMMIT;
      V_ENDTIME        := SYSDATE;
      V_MESSAGESFAILED := V_MESSAGESFAILED;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);
    WHEN OTHERS THEN
      V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
      V_ENDTIME        := SYSDATE;
      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);

      V_ERROR   := SQLERRM;
      V_MESSAGE := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                   '    <pkg>AE_Transition</pkg>' || CHR(10) ||
                   '    <proc>LoadSynchronyMembers</proc>' || CHR(10) ||
                   '  </details>' || CHR(10) || '</failed>';
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

      RAISE;

  END LoadSynchronyMembers;

   PROCEDURE SelectTransitionMembers( p_procdate  VARCHAR2 ,
                                      p_mod  INTEGER,
                                      p_instance  INTEGER,
                                      RETVAL  IN OUT RCURSOR ) IS

    v_processDate DATE := To_Date(p_procdate,'MM/DD/YYYY');

    /*
    -- get primry loyaltynumber not in the temp table
    CURSOR GET_DATA1 IS
        SELECT vc.ipcode, vc.loyaltyidnumber, vc.linkkey
        FROM bp_ae.lw_virtualcard vc
        INNER JOIN bp_ae.x$_aeo_connected_members mc ON mc.loyaltynumber = vc.loyaltyidnumber
        WHERE vc.isprimary = 1 AND
              mc.loyaltynumber IS NULL
              MOD( vc.ipcode, p_mod) = p_instance;


    TYPE T_TAB1 IS TABLE OF GET_DATA1%ROWTYPE;
    V_TBL1 T_TAB1;
    */

   -- get loyltynumbers that are not in the temp table and that have had activity in the last 18 months

    CURSOR GET_DATA2 IS
        SELECT Lm.Ipcode, Vc.Loyaltyidnumber, Vc.Linkkey
        FROM   Bp_Ae.Lw_Loyaltymember Lm
        INNER  JOIN Bp_Ae.Lw_Virtualcard Vc                ON     Vc.Ipcode = Lm.Ipcode
        LEFT   JOIN Bp_Ae.X$_Aeo_Connected_Members Cm      ON     Cm.Loyaltynumber = Vc.Loyaltyidnumber
        WHERE  Lm.Lastactivitydate BETWEEN Trunc(Add_Months(v_processDate, -18)) AND   Trunc(v_processDate)
               AND Cm.Loyaltynumber IS NULL AND
               vc.isprimary = 1 AND
               LENGTH(vc.loyaltyidnumber) = 14 AND
               MOD( LM.ipcode, p_mod) = p_instance;

    TYPE T_TAB2 IS TABLE OF GET_DATA2%ROWTYPE;
    V_TBL2 T_TAB2;

    -- get loyltynumbers that are not in the temp table that has  memberstatus 3 and whose name is not 'trminated'
    CURSOR GET_DATA3 IS
      SELECT lm.ipcode, vc.loyaltyidnumber, vc.linkkey
      FROM   Bp_Ae.Lw_Loyaltymember Lm
      INNER JOIN bp_ae.lw_virtualcard vc ON vc.ipcode = lm.ipcode
      LEFT JOIN  bp_ae.x$_aeo_connected_members cm ON cm.loyaltynumber = vc.loyaltyidnumber
      WHERE  lm.memberstatus = 3 AND
             lower(lm.firstname) <> 'account terminated' AND
             cm.loyaltynumber IS NULL AND
               vc.isprimary = 1 AND
                  LENGTH(vc.loyaltyidnumber) = 14 AND
             MOD( LM.ipcode, p_mod) = p_instance;



    TYPE T_TAB3 IS TABLE OF GET_DATA3%ROWTYPE;
    V_TBL3 T_TAB3;

    -- get lotaltynumbers that are not in the temp table but that are in the incoming file
    CURSOR GET_DATA4 IS
      SELECT lm.Ipcode, Sy.Cardholder_Loyalty_Id, Sy.Acct_Key
      FROM   Bp_Ae.Ae_Synchrony Sy
      INNER  JOIN Bp_Ae.Lw_Virtualcard Vc ON     Vc.Loyaltyidnumber = Sy.Cardholder_Loyalty_Id
      INNER  JOIN Bp_Ae.lw_loyaltymember lm ON     lm.ipcode = vc.ipcode
      LEFT   JOIN Bp_Ae.X$_Aeo_Connected_Members Cm ON     Cm.Loyaltynumber = Sy.Cardholder_Loyalty_Id
      WHERE  Cm.Loyaltynumber IS NULL AND
               vc.isprimary = 1 AND
                  LENGTH(vc.loyaltyidnumber) = 14 AND
             MOD( LM.ipcode, p_mod) = p_instance;

    TYPE T_TAB4 IS TABLE OF GET_DATA4%ROWTYPE;
    V_TBL4 T_TAB4;


  CURSOR GET_DATA5 IS
      SELECT Lm.Ipcode, Vc.Loyaltyidnumber, Vc.Linkkey
      FROM   Bp_Ae.Lw_Loyaltymember Lm
      INNER  JOIN Bp_Ae.Lw_Virtualcard Vc    ON     Vc.Ipcode = Lm.Ipcode
      LEFT   JOIN bp_ae.x$_aeo_connected_members cm ON cm.loyaltynumber = vc.loyaltyidnumber
      WHERE  lm.createdate BETWEEN Trunc(Add_Months(v_processDate, -6)) AND  Trunc(v_processDate)  AND
             cm.loyaltynumber IS NULL AND
             vc.isprimary = 1 AND
                LENGTH(vc.loyaltyidnumber) = 14 AND
             NOT vc.vckey IN (SELECT th.a_vckey FROM bp_ae.ats_txnheader th
                              WHERE th.a_vckey = vc.vckey AND
                                   th.a_txndate  BETWEEN Trunc(Add_Months(v_processDate, -6)) AND  Trunc(v_processDate) ) AND
                    MOD( LM.ipcode, p_mod) = p_instance;



    TYPE T_TAB5 IS TABLE OF GET_DATA5%ROWTYPE;
    V_TBL5 T_TAB5;

    V_MY_LOG_ID NUMBER;
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) :='SelectTransitionMembers' ;
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
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

     BEGIN

         V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();


         V_JOBNAME   := 'SelectTransitionMembers';
         V_LOGSOURCE := V_JOBNAME;

          /* log start of job */
         UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                              P_JOBDIRECTION     => V_JOBDIRECTION,
                              P_FILENAME         => V_FILENAME,
                              P_STARTTIME        => V_STARTTIME,
                              P_ENDTIME          => V_ENDTIME,
                              P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                              P_MESSAGESFAILED   => V_MESSAGESFAILED,
                              P_JOBSTATUS        => V_JOBSTATUS,
                              P_JOBNAME          => 'stage' || V_JOBNAME);

           IF (p_instance = 0 ) THEN
                 V_ERROR          :=  ' ';
                 V_REASON         := 'truncating tables start';
                 V_MESSAGE        := 'truncating tables start';
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


                 EXECUTE IMMEDIATE 'Truncate table x$_aeo_connected_members';



                 V_ERROR          :=  ' ';
                 V_REASON         := 'truncating tables end';
                 V_MESSAGE        := 'truncating tables end';
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
           END IF;


           ---
           -- insert all rows marked as primary in lw_virtualcard
           --
           /*
             V_ERROR          :=  ' ';
             V_REASON         := 'begin insert all rows marked as primary in lw_virtualcard';
             V_MESSAGE        := 'begin insert all rows marked as primary in lw_virtualcard';
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
           OPEN GET_DATA1;
           LOOP
              FETCH GET_DATA1 BULK COLLECT
                INTO V_TBL1 LIMIT 1000;

              FORALL I IN 1 .. V_TBL1.COUNT


                INSERT INTO x$_aeo_connected_members
                     ( loyaltynumber,
                       linkkey,
                       ipcode
                     )
                VALUES
                     ( V_TBL1(i).loyaltyidnumber,
                       v_tbl1(i).linkkey,
                       v_tbl1(i).ipcode
                     );
              COMMIT WRITE batch NOWAIT;
              EXIT WHEN GET_DATA1%NOTFOUND;
           END LOOP;
           COMMIT;
           IF GET_DATA1%ISOPEN THEN
             CLOSE GET_DATA1;
           END IF;
            V_ERROR          :=  ' ';
            V_REASON         := 'end insert all rows marked as primary in lw_virtualcard';
            V_MESSAGE        := 'end insert all rows marked as primary in lw_virtualcard';
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
            */
           --
           -- insert all  ipcodes that have made transactions on the last 18 months
           -- that are not already in the  temp table
           --
             V_ERROR          :=  ' ';
             V_REASON         := 'begin populate temp table step 2';
             V_MESSAGE        := 'begin populate temp table step 2';
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
                INTO V_TBL2 LIMIT 1000;

              FORALL I IN 1 .. V_TBL2.COUNT


                INSERT INTO x$_aeo_connected_members
                     ( loyaltynumber,
                       linkkey,
                       ipcode,
                       selectiontype
                     )
                VALUES
                     ( V_TBL2(i).loyaltyidnumber,
                       v_tbl2(i).linkkey,
                       v_tbl2(i).ipcode,
                       1
                     );
              COMMIT WRITE batch NOWAIT;
              EXIT WHEN GET_DATA2%NOTFOUND;
           END LOOP;
           COMMIT;
           IF GET_DATA2%ISOPEN THEN
             CLOSE GET_DATA2;
           END IF;

            V_ERROR          :=  ' ';
            V_REASON         := 'end populate temp table step 2';
            V_MESSAGE        := 'end populate temp table step 2';
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

           --
           -- insert all  ipcodes that have made transactions on the last 18 months
           -- that are not already in the  temp table
           --

            V_ERROR          :=  ' ';
            V_REASON         := 'begin populate temp table step 3';
            V_MESSAGE        := 'begin populate temp table step 3';
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
           OPEN GET_DATA3;
           LOOP
              FETCH GET_DATA3 BULK COLLECT
                INTO V_TBL3 LIMIT 1000;

              FORALL I IN 1 .. V_TBL3.COUNT


                INSERT INTO x$_aeo_connected_members
                     ( loyaltynumber,
                       linkkey,
                       ipcode,
                       selectiontype
                     )
                VALUES
                     ( V_TBL3(i).loyaltyidnumber,
                       v_tbl3(i).linkkey,
                       v_tbl3(i).ipcode,
                       2
                     );
              COMMIT WRITE batch NOWAIT;
              EXIT WHEN GET_DATA3%NOTFOUND;
           END LOOP;
           COMMIT;
           IF GET_DATA3%ISOPEN THEN
             CLOSE GET_DATA3;
           END IF;

           V_ERROR          :=  ' ';
           V_REASON         := 'end populate temp table step 3';
           V_MESSAGE        := 'end populate temp table step 3';
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
           --
           -- insert all  ipcodes that are in the file but bot in the temp table
           --

            V_ERROR          :=  ' ';
            V_REASON         := 'begin populate temp table step 4';
            V_MESSAGE        := 'begin populate temp table step 4';
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
           OPEN GET_DATA4;
           LOOP
              FETCH GET_DATA4 BULK COLLECT
                INTO V_TBL4 LIMIT 1000;

              FORALL I IN 1 .. V_TBL4.COUNT


                INSERT INTO x$_aeo_connected_members
                     ( loyaltynumber,
                       linkkey,
                       ipcode,
                       selectiontype
                     )
                VALUES
                     ( V_TBL4(i).Cardholder_Loyalty_Id,
                       v_tbl4(i).acct_key,
                       v_tbl4(i).ipcode,
                       3
                     );
              COMMIT WRITE batch NOWAIT;
              EXIT WHEN GET_DATA4%NOTFOUND;
           END LOOP;
           COMMIT;
           IF GET_DATA4%ISOPEN THEN
             CLOSE GET_DATA4;
           END IF;

            V_ERROR          :=  ' ';
            V_REASON         := 'begin populate temp table step 4';
            V_MESSAGE        := 'begin populate temp table step 4';
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
           --
           -- insert all  ipcodes that are in the file but bot in the temp table
           --
            V_ERROR          :=  ' ';
            V_REASON         := 'begin populate temp table step 5';
            V_MESSAGE        := 'begin populate temp table step 5';
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
           OPEN GET_DATA5;
           LOOP
              FETCH GET_DATA5 BULK COLLECT
                INTO V_TBL5 LIMIT 1000;

              FORALL I IN 1 .. V_TBL5.COUNT


                INSERT INTO x$_aeo_connected_members
                     ( loyaltynumber,
                       linkkey,
                       ipcode,
                       selectiontype
                     )
                VALUES
                     ( V_TBL5(i).loyaltyidnumber,
                       v_tbl5(i).linkkey,
                       v_tbl5(i).ipcode,
                       4
                     );
              COMMIT WRITE batch NOWAIT;
              EXIT WHEN GET_DATA5%NOTFOUND;
           END LOOP;
           COMMIT;
           IF GET_DATA5%ISOPEN THEN
             CLOSE GET_DATA5;
           END IF;

            V_ERROR          :=  ' ';
            V_REASON         := 'end populate temp table step 5';
            V_MESSAGE        := 'end populate temp table step 5';
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
               /* insert here */
          V_ENDTIME   := SYSDATE;
          V_JOBSTATUS := 1;

          /* log end of job */
          UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                              P_JOBDIRECTION     => V_JOBDIRECTION,
                              P_FILENAME         => V_FILENAME,
                              P_STARTTIME        => V_STARTTIME,
                              P_ENDTIME          => V_ENDTIME,
                              P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                              P_MESSAGESFAILED   => V_MESSAGESFAILED,
                              P_JOBSTATUS        => V_JOBSTATUS,
                              P_JOBNAME          =>  V_JOBNAME);

          OPEN RETVAL FOR
            SELECT V_MY_LOG_ID FROM DUAL;

  EXCEPTION
    WHEN DML_ERRORS THEN

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := substr(SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE),1,255);
        V_REASON         := 'Failed Records in Procedure SelectTransitionMembers: ';
        V_MESSAGE        := ' ';

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

      END LOOP;
      COMMIT;
      V_ENDTIME        := SYSDATE;
      V_MESSAGESFAILED := V_MESSAGESFAILED;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);
    WHEN OTHERS THEN
      V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
      V_ENDTIME        := SYSDATE;
      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);

      V_ERROR   := SQLERRM;
      V_MESSAGE := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                   '    <pkg>AE_Transition</pkg>' || CHR(10) ||
                   '    <proc>SelectTransitionMembers</proc>' || CHR(10) ||
                   '  </details>' || CHR(10) || '</failed>';
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

      RAISE;
     END SelectTransitionMembers;


    PROCEDURE UpdateTransitionMembers(retval IN OUT RCURSOR) IS

     Err_Code  VARCHAR(512) := NULL;
     Err_Msg   VARCHAR2(200):= NULL;

     V_MY_LOG_ID NUMBER;
     V_JOBDIRECTION     NUMBER := 0;
     V_FILENAME         VARCHAR2(512) :='SelectTransitionMembers' ;
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
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);


      BEGIN
         V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();


         V_JOBNAME   := 'UpdateTransitionMembers';
         V_LOGSOURCE := V_JOBNAME;

          /* log start of job */
         UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                              P_JOBDIRECTION     => V_JOBDIRECTION,
                              P_FILENAME         => V_FILENAME,
                              P_STARTTIME        => V_STARTTIME,
                              P_ENDTIME          => V_ENDTIME,
                              P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                              P_MESSAGESFAILED   => V_MESSAGESFAILED,
                              P_JOBSTATUS        => V_JOBSTATUS,
                              P_JOBNAME          => 'stage' || V_JOBNAME);



         -- drop original backup table if exist

          begin
              execute immediate 'drop table bp_ae.lw_loyaltymember_con_new';


          exception

              WHEN OTHERS THEN
                Err_Code  := SQLCODE;
                Err_Msg   := Substr(SQLERRM, 1, 200);
                v_Reason  := 'drop table bp_ae.lw_loyaltymember_con_new ' || Err_Code || ' ' || Err_Msg;
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
          end;

          begin
              execute immediate 'drop table bp_ae.lw_loyaltymember_con_old';


          exception

              WHEN OTHERS THEN
                Err_Code  := SQLCODE;
                Err_Msg   := Substr(SQLERRM, 1, 200);
                v_Reason  := 'drop table bp_ae.lw_loyaltymember_con_old ' || Err_Code || ' ' || Err_Msg;
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
          end;

         -- create the new table
         BEGIN
           EXECUTE IMMEDIATE '
              CREATE TABLE bp_ae.lw_loyaltymember_con_new AS
              SELECT lm.ipcode ,
                     lm.membercreatedate,
                     lm.memberclosedate,
                     CASE WHEN lm.memberstatus = 3   THEN CAST( 3 as number(10))
                          WHEN cm.ipcode IS NULL     THEN CAST( 5 AS NUMBER(10))
                          WHEN cm.ipcode IS NOT NULL THEN CAST( 1 AS NUMBER(10))
                      END AS memberstatus,
                     lm.birthdate,
                     lm.firstname,
                     lm.lastname,
                     lm.middlename,
                     lm.nameprefix,
                     lm.namesuffix,
                     lm.mobiledevicetype,
                     lm.username,
                     lm.password,
                     lm.primaryphonenumber,
                     lm.primarypostalcode,
                     lm.lastactivitydate,
                     lm.isemployee,
                     lm.changedby,
                     lm.newstatus,
                     lm.newstatuseffectivedate,
                     lm.createdate,
                     lm.last_dml_id,
                     lm.statuschangereason,
                     lm.resetcode,
                     lm.resetcodedate,
                     lm.updatedate,
                     lm.alternateid,
                     lm.salt,
                     lm.passwordexpiredate,
                     lm.statuschangedate,
                     lm.mergedtomember,
                     lm.failedpasswordattemptcount,
                     lm.passwordchangerequired,
                     lm.primaryemailaddress
              FROM bp_ae.lw_loyaltymember lm
              LEFT JOIN bp_ae.x$_aeo_connected_members cm ON cm.ipcode = lm.ipcode
           ';
           EXCEPTION
                   WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := ' CREATE TABLE bp_ae.lw_loyaltymember_con_new ' || Err_Code || ' ' || Err_Msg;
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



         -- drop trigers in the original table
          begin
           execute immediate 'drop trigger BP_AE."LW_LOYALTYMEMBER#" ';
         exception
                WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'drop trigger BP_AE."LW_LOYALTYMEMBER#" ' || Err_Code || ' ' || Err_Msg;
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
        end;

    begin
        execute immediate 'drop trigger bp_ae.LW_LOYALTYMEMBER_ARC ';

    exception
            WHEN OTHERS THEN
          Err_Code  := SQLCODE;
          Err_Msg   := Substr(SQLERRM, 1, 200);
          v_Reason  := 'drop trigger BP_AE."LW_LOYALTYMEMBER_ARC" ' || Err_Code || ' ' || Err_Msg;
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
    end;

         -- drop constraints before renaming the current table

             begin
                 execute immediate 'alter table bp_ae.lw_csnote drop constraint fk_csnote_memberid';
                  exception
                          WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'drop constraint fk_csnote_memberid ' || Err_Code || ' ' || Err_Msg;
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
             end;

            begin
                execute immediate 'alter table bp_ae.lw_membertiers drop constraint fk_membertier_ipcode';
            exception
                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'drop constraint fk_membertier_ipcode ' || Err_Code || ' ' || Err_Msg;
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
            end;

         -- drop indexes in the orignal table

            begin
                execute immediate 'drop index BP_AE.IDX_MEMBER_ALTERNATEID';
            exception
                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'drop index BP_AE.IDX_MEMBER_ALTERNATEID' || Err_Code || ' ' || Err_Msg;
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
            end;


            BEGIN
              EXECUTE IMMEDIATE 'drop index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE';
            EXCEPTION
              WHEN OTHERS THEN
                   Err_Code  := SQLCODE;
                   Err_Msg   := Substr(SQLERRM, 1, 200);
                   v_Reason  := 'drop index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE' || Err_Code || ' ' ||
                                Err_Msg;
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


            begin
                execute immediate 'drop index BP_AE.LW_LOYALTYMEMBER$#';
            exception
                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'drop index BP_AE.LW_LOYALTYMEMBER$#' || Err_Code || ' ' || Err_Msg;
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
            end;


            begin
                execute immediate 'drop index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP';
            exception

                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'drop index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP' || Err_Code || ' ' || Err_Msg;
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
            end;


            begin
                execute immediate 'drop index BP_AE.LW_LOYALTYMEMBER_UK1';
            exception
                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'drop index BP_AE.LW_LOYALTYMEMBER_UK1' || Err_Code || ' ' || Err_Msg;
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
            end;

            begin
                execute immediate 'drop index BP_AE.LW_LOYALTYMEMBER_EMAILIDX';
            exception
                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'drop index BP_AE.LW_LOYALTYMEMBER_EMAILIDX' || Err_Code || ' ' || Err_Msg;
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
            end;



            begin
                execute immediate 'alter table BP_AE.LW_LOYALTYMEMBER drop unique (USERNAME)';
            exception
                    WHEN OTHERS THEN
                  Err_Code  := SQLCODE;
                  Err_Msg   := Substr(SQLERRM, 1, 200);
                  v_Reason  := 'BP_AE.LW_LOYALTYMEMBER drop unique (USERNAME)' || Err_Code || ' ' || Err_Msg;
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
            end;

         -- rename the current table to the backup table
             begin
                 execute immediate 'RENAME  lw_loyaltymember TO lw_Loyaltymember_con_old';
                  exception
                          WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'rename lw_loyaltymember to lw_Loyaltymember_con_old' || Err_Code || ' ' || Err_Msg;
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
             end;

         -- rename the new table
             begin
                 execute immediate 'RENAME  lw_Loyaltymember_con_new tO lw_Loyaltymember';
                  exception
                          WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'rename lw_loyaltymember_con_new to lw_loyaltymember' || Err_Code || ' ' || Err_Msg;
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
             end;

         -- re create indexes
            begin
                execute immediate '
                    create index BP_AE.IDX_MEMBER_ALTERNATEID on BP_AE.LW_LOYALTYMEMBER (ALTERNATEID)
                      tablespace BP_AE_D
                      pctfree 10
                      initrans 2
                      parallel compute statistics
                      maxtrans 255
                      storage
                      (
                        initial 64K
                        next 1M
                        minextents 1
                        maxextents unlimited
                      )
                ';
            EXCEPTION
                 WHEN OTHERS THEN
                      Err_Code  := SQLCODE;
                      Err_Msg   := Substr(SQLERRM, 1, 200);
                      v_Reason  := 'create index BP_AE.IDX_MEMBER_ALTERNATEID' ||
                                   Err_Code || ' ' || Err_Msg;
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
            end;

            BEGIN
             EXECUTE IMMEDIATE '
                create index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE on BP_AE.LW_LOYALTYMEMBER (ALTERNATEID, IPCODE)
                  tablespace BP_AE_D
                  pctfree 10
                  initrans 2
                  maxtrans 255
                  parallel compute statistics
                  storage
                  (
                    initial 64K
                    next 1M
                    minextents 1
                    maxextents unlimited
                  )    ';
            EXCEPTION
                 WHEN OTHERS THEN
                      Err_Code  := SQLCODE;
                      Err_Msg   := Substr(SQLERRM, 1, 200);
                      v_Reason  := 'create index BP_AE.IDX_MEMBER_ALTERNATEID_IPCODE' ||
                                   Err_Code || ' ' || Err_Msg;
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

            begin
              execute immediate '
                  create index BP_AE.LW_LOYALTYMEMBER$# on BP_AE.LW_LOYALTYMEMBER (LAST_DML_ID)
                    tablespace BP_AE_D
                    pctfree 10
                    initrans 2
                    maxtrans 255
                    parallel compute statistics
                    storage
                    (
                      initial 64K
                      next 1M
                      minextents 1
                      maxextents unlimited
                    )
              ';
            EXCEPTION
              WHEN OTHERS THEN
                    Err_Code  := SQLCODE;
                    Err_Msg   := Substr(SQLERRM, 1, 200);
                    v_Reason  := 'create index BP_AE.LW_LOYALTYMEMBER$#' ||
                                 Err_Code || ' ' || Err_Msg;
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
            end;

            begin
                execute immediate '
                    create index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP on BP_AE.LW_LOYALTYMEMBER (UPPER(LASTNAME), UPPER(PRIMARYPOSTALCODE))
                      tablespace BP_AE_D
                      pctfree 10
                      initrans 2
                      maxtrans 255
                      parallel compute statistics
                      storage
                      (
                        initial 64K
                        next 1M
                        minextents 1
                        maxextents unlimited
                      )
                ';
                 EXCEPTION
                WHEN OTHERS THEN
                      Err_Code  := SQLCODE;
                      Err_Msg   := Substr(SQLERRM, 1, 200);
                      v_Reason  := 'create index BP_AE.LW_LOYALTYMEMBER_NAME_ZIP' ||
                                   Err_Code || ' ' || Err_Msg;
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
            end;

            BEGIN
              EXECUTE IMMEDIATE '
                      ALTER TABLE BP_AE.LW_LOYALTYMEMBER ADD PRIMARY KEY ("IPCODE")
              ';
             EXCEPTION
                  WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'ADD PRIMARY KEY ' ||
                                     Err_Code || ' ' || Err_Msg;
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
              EXECUTE IMMEDIATE '
                      ALTER TABLE BP_AE.LW_LOYALTYMEMBER ADD UNIQUE("USERNAME")
              ';
             EXCEPTION
                  WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'ADD UNIQUE KEY ' ||
                                     Err_Code || ' ' || Err_Msg;
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
            begin
                  execute immediate '
                      create unique index BP_AE.LW_LOYALTYMEMBER_UK1 on BP_AE.LW_LOYALTYMEMBER (USERNAME)
                        tablespace BP_AE_D
                        pctfree 10
                        initrans 2
                        maxtrans 255
                        parallel compute statistics
                        storage
                        (
                          initial 64K
                          next 1M
                          minextents 1
                          maxextents unlimited
                        )
                  ';

            EXCEPTION
                  WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'create index BP_AE.LW_LOYALTYMEMBER_UK1' ||
                                     Err_Code || ' ' || Err_Msg;
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
              end;


           BEGIN
               EXECUTE IMMEDIATE '
               CREATE INDEX BP_AE.LW_LOYALTYMEMBER_EMAILIDX ON BP_AE.LW_LOYALTYMEMBER (PRIMARYEMAILADDRESS)
                PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS
                STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
                PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
                TABLESPACE BP_AE_D
                PARALLEL '  ;
              EXCEPTION
               WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := 'create index BP_AE.LW_LOYALTYMEMBER_EMAILIDX' ||
                                     Err_Code || ' ' || Err_Msg;
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



         -- create constraints
            begin
              execute immediate '
                  alter table BP_AE.LW_MEMBERTIERS
                    add constraint FK_MEMBERTIER_IPCODE foreign key (MEMBERID)
                    references BP_AE.LW_LOYALTYMEMBER (IPCODE)
                    enable
                    validate ';
              EXCEPTION
              WHEN OTHERS THEN
                    Err_Code  := SQLCODE;
                    Err_Msg   := Substr(SQLERRM, 1, 200);
                    v_Reason  := 'add constraint FK_MEMBERTIER_IPCODE' ||
                                 Err_Code || ' ' || Err_Msg;
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
            end;

       begin
        execute immediate '
            alter table BP_AE.LW_CSNOTE
              add constraint FK_CSNOTE_MEMBERID foreign key (MEMBERID)
              references BP_AE.LW_LOYALTYMEMBER (IPCODE)
              enable
              validate
        ';
              EXCEPTION
        WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'add constraint FK_CSNOTE_MEMBERID' ||
                           Err_Code || ' ' || Err_Msg;
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

            end;



           -- create triggers


            begin
              execute immediate '
                    CREATE OR REPLACE TRIGGER "BP_AE"."LW_LOYALTYMEMBER#"
                     BEFORE UPDATE OR INSERT ON bp_ae.LW_LOYALTYMEMBER
                      FOR EACH ROW
                    BEGIN
                      :new.primarypostalcode := REGEXP_REPLACE(TRIM(UPPER(:new.primarypostalcode)), ''[-[:space:]]'', '''');

                      :NEW.last_dml_id := LAST_DML_ID#.nextval;
                    END;
                   ';
              EXCEPTION
              WHEN OTHERS THEN
                    Err_Code  := SQLCODE;
                    Err_Msg   := Substr(SQLERRM, 1, 200);
                    v_Reason  := 'create trigger LW_LOYALTYMEMBER#' ||
                                 Err_Code || ' ' || Err_Msg;
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

            begin
              execute immediate '
                        CREATE OR REPLACE TRIGGER "BP_AE"."LW_LOYALTYMEMBER_ARC"
                         BEFORE UPDATE ON bp_ae.LW_LOYALTYMEMBER
                          FOR EACH ROW
                        BEGIN
                        insert into lw_loyaltymember_arc (id, ipcode, memberclosedate, memberstatus, firstname, lastname, primaryemailaddress, primaryphonenumber, primarypostalcode, lastactivitydate, changedby, newstatus, newstatuseffectivedate, last_dml_date, last_dml_id, statuschangereason, birthdate)
                        values (seq_rowkey.nextval, :OLD.ipcode, :OLD.memberclosedate, :OLD.memberstatus, :OLD.firstname, :OLD.lastname, :OLD.primaryemailaddress, :OLD.primaryphonenumber, :OLD.primarypostalcode, :OLD.lastactivitydate,
                               :OLD.changedby, :OLD.newstatus, :OLD.newstatuseffectivedate, :OLD.createdate, :OLD.last_dml_id, :OLD.statuschangereason, :OLD.Birthdate);

                        END;
                   ';
              EXCEPTION
              WHEN OTHERS THEN
                    Err_Code  := SQLCODE;
                    Err_Msg   := Substr(SQLERRM, 1, 200);
                    v_Reason  := 'create trigger LW_LOYALTYMEMBER_ARC' ||
                                 Err_Code || ' ' || Err_Msg;
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

            -- add default values
          begin
           execute immediate 'ALTER TABLE bp_ae.LW_LOYALTYMEMBER MODIFY failedpasswordattemptcount DEFAULT 0';
         exception
                WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'DEFAULT VALUE TO failedpasswordattemptcount ' || Err_Code || ' ' || Err_Msg;
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
        end;

        begin
           execute immediate 'ALTER TABLE bp_ae.LW_LOYALTYMEMBER MODIFY passwordchangerequired DEFAULT 0';
         exception
                WHEN OTHERS THEN
              Err_Code  := SQLCODE;
              Err_Msg   := Substr(SQLERRM, 1, 200);
              v_Reason  := 'DEFAULT VALUE TO passwordchangerequired ' || Err_Code || ' ' || Err_Msg;
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
        end;

                   /* insert here */
          V_ENDTIME   := SYSDATE;
          V_JOBSTATUS := 1;

          /* log end of job */
          UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                              P_JOBDIRECTION     => V_JOBDIRECTION,
                              P_FILENAME         => V_FILENAME,
                              P_STARTTIME        => V_STARTTIME,
                              P_ENDTIME          => V_ENDTIME,
                              P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                              P_MESSAGESFAILED   => V_MESSAGESFAILED,
                              P_JOBSTATUS        => V_JOBSTATUS,
                              P_JOBNAME          =>  V_JOBNAME);

          OPEN RETVAL FOR
            SELECT V_MY_LOG_ID FROM DUAL;

  EXCEPTION
    WHEN DML_ERRORS THEN

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := substr(SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE),1,255);
        V_REASON         := 'Failed Records in Procedure UpateTransitionMembers: ';
        V_MESSAGE        := ' ';

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

      END LOOP;
      COMMIT;
      V_ENDTIME        := SYSDATE;
      V_MESSAGESFAILED := V_MESSAGESFAILED;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);
    WHEN OTHERS THEN
      V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
      V_ENDTIME        := SYSDATE;
      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);

      V_ERROR   := SQLERRM;
      V_MESSAGE := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                   '    <pkg>AE_Transition</pkg>' || CHR(10) ||
                   '    <proc>UpdateTransitionMembers</proc>' || CHR(10) ||
                   '  </details>' || CHR(10) || '</failed>';
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

      RAISE;

      END  UpdateTransitionMembers  ;


-- AEO-1634 end

-- AEO-1637 Begin
  PROCEDURE Clear_TempTable(p_Type INTEGER) AS
    BEGIN
      CASE
        WHEN p_Type = 1 THEN
            EXECUTE IMMEDIATE 'truncate table X$_NR_EXPIREDPOINTS';
        WHEN p_Type = 2 THEN
           EXECUTE IMMEDIATE 'truncate table X$_NR_EXPIREDTIERS';
        WHEN p_Type = 3 THEN
            EXECUTE IMMEDIATE 'truncate table X$_AEO1638_TIERDUPLICATED';
      END CASE;
  END Clear_TempTable;

  PROCEDURE Get_Points_Transactions(p_ProcessDate1 IN VARCHAR2, p_ProcessDate2 IN VARCHAR2, p_Line IN VARCHAR2) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    --Days
    V_FIRST_DAY TIMESTAMP;
    V_LAST_DAY  TIMESTAMP;
    V_MONTHS    NUMBER := 0;
    V_START     NUMBER;
    V_PROCESSDATE1  DATE := TO_DATE(p_ProcessDate1, 'YYYYMMddHH24miss');
    V_PROCESSDATE2  DATE := TO_DATE(p_ProcessDate2, 'YYYYMMddHH24miss');

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_STAGEPOINTS_' || p_Line;
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_STAGEPOINTS', v_processId);

      V_START := DBMS_UTILITY.get_time;
      --Months from since the begining
      SELECT TRUNC(MONTHS_BETWEEN (TRUNC(V_PROCESSDATE2), TRUNC(V_PROCESSDATE1)))
      INTO V_MONTHS
      FROM DUAL
      ;

      V_MONTHS    := V_MONTHS + 1;
      V_FIRST_DAY := TRUNC(V_PROCESSDATE1);

      FOR i IN 1.. V_MONTHS LOOP
          IF i = 1 THEN
             V_LAST_DAY := LAST_DAY(TRUNC(V_FIRST_DAY));
          ELSE
             V_FIRST_DAY := V_LAST_DAY + 1;
             V_LAST_DAY := LAST_DAY(TRUNC(V_FIRST_DAY));
          END IF;

          V_FIRST_DAY := TO_DATE(TO_CHAR((TRUNC(V_FIRST_DAY)),'MM/DD/YYYY') || ' 00:00:00' ,'MM/DD/YYYY HH24:MI:SS');
          V_LAST_DAY := TO_DATE(TO_CHAR((TRUNC(V_LAST_DAY)),'MM/DD/YYYY') || ' 23:59:59' ,'MM/DD/YYYY HH24:MI:SS');

          --Fill out temp table per partition
          Stage_DataEntry(V_FIRST_DAY,V_LAST_DAY);
      END LOOP;

      DBMS_OUTPUT.put_line('INSERT: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Get_Points_Transactions_'|| p_Line,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Get_Points_Transactions_'|| p_Line);

  END Get_Points_Transactions;

  /*********** Internal procedure to fill out the temp table ************/
PROCEDURE Stage_DataEntry (p_ProcessDate1 IN TIMESTAMP, p_ProcessDate2 IN TIMESTAMP) IS

    CURSOR get_data IS
      SELECT pt.pointtransactionid, pt.transactiondate, pt.notes, pt.expirationdate, pt.ptchangedby, pt.updatedate
      FROM bp_ae.X$_AEO_Connected_Members cm
      INNER JOIN bp_ae.lw_virtualcard vc On vc.loyaltyidnumber = cm.loyaltynumber
      INNER JOIN bp_ae.lw_pointtransaction pt ON pt.vckey = vc.vckey
      WHERE pt.transactiondate BETWEEN p_ProcessDate1 AND p_ProcessDate2
      AND pt.expirationdate > TRUNC(SYSDATE)
      ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN

      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;

        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
            INSERT INTO bp_ae.X$_NR_EXPIREDPOINTS
              (pointtransactionid,
              transactiondate,
              original_notes,
              original_expiration_date,
              original_ptchangedby,
              original_updatedate)
            VALUES
              (v_tbl(j).pointtransactionid,
              v_tbl(j).transactiondate,
              v_tbl(j).notes,
              v_tbl(j).expirationdate,
              v_tbl(j).ptchangedby,
              v_tbl(j).updatedate)
            ;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;

  END Stage_DataEntry;

  PROCEDURE ExpireAllPoints(p_mod INTEGER, p_instances INTEGER) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    v_NRExpirationNotes NVARCHAR2(50) := ' -NRExpire';
    v_ExpireDate        DATE := TRUNC(SYSDATE) - 1/86400;

    CURSOR Driver IS
     SELECT *
     FROM bp_ae.X$_NR_EXPIREDPOINTS a
     WHERE 1=1
     AND mod(a.pointtransactionid, p_instances) = p_mod;

     TYPE Driver_Table IS TABLE OF Driver%ROWTYPE;
     Dt Driver_Table;
     V_START             NUMBER;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_EXPIREDPOINTS_' || TO_CHAR(p_mod);
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_EXPIREDPOINTS', v_processId);

     V_START := DBMS_UTILITY.get_time;
     OPEN Driver;
     LOOP
          FETCH Driver BULK COLLECT INTO Dt LIMIT 1000;
          FORALL i IN 1 .. Dt.Count SAVE EXCEPTIONS
                UPDATE bp_ae.lw_pointtransaction pt
                SET pt.expirationdate = v_ExpireDate,
                           pt.ptchangedby = 'nr_expiredpoints',
                           pt.updatedate  = systimestamp,
                           pt.notes =
                           (
                           CASE
                             WHEN INSTR(pt.notes,v_NRExpirationNotes,1) = 0 THEN
                                pt.notes || v_NRExpirationNotes
                             ELSE
                               pt.notes
                           END
                           )
                WHERE pt.pointtransactionid = Dt(i).pointtransactionid
                ;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN Driver%NOTFOUND;
     END LOOP;

     COMMIT;
     CLOSE Driver;
     DBMS_OUTPUT.put_line('EXPIRE: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');
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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'ExpireAllPoints_'|| TO_CHAR(p_mod),
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in ExpireAllPoints_'|| TO_CHAR(p_mod));
  END ExpireAllPoints;

  PROCEDURE UnExpireAllPoints(p_mod INTEGER, p_instances INTEGER) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    V_START       NUMBER;

    CURSOR Driver IS
     SELECT *
       FROM bp_ae.X$_NR_EXPIREDPOINTS a
      WHERE mod(a.pointtransactionid, p_instances) = p_mod;

     TYPE Driver_Table IS TABLE OF Driver%ROWTYPE;
     Dt Driver_Table;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_UNEXPIREDPOINTS_' || TO_CHAR(p_mod);
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_UNEXPIREDPOINTS', v_processId);

     V_START := DBMS_UTILITY.get_time;

     OPEN Driver;
     LOOP
          FETCH Driver BULK COLLECT INTO Dt LIMIT 1000;
          FORALL i IN 1 .. Dt.Count SAVE EXCEPTIONS
                UPDATE bp_ae.lw_pointtransaction pt
                SET pt.expirationdate = Dt(i).original_expiration_date,
                           pt.notes = Dt(i).original_notes,
                           pt.ptchangedby = Dt(i).original_ptchangedby,
                           pt.updatedate  = Dt(i).original_updatedate
                WHERE pt.pointtransactionid = Dt(i).pointtransactionid
              ;

          COMMIT WRITE BATCH NOWAIT;
          EXIT WHEN Driver%NOTFOUND;
     END LOOP;

     COMMIT;
     CLOSE Driver;
     DBMS_OUTPUT.put_line('UN EXPIRE: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');
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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'UnExpireAllPoints_'|| TO_CHAR(p_mod),
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in UnExpireAllPoints_'|| TO_CHAR(p_mod));
  END UnExpireAllPoints;

  PROCEDURE Index_TempTable(p_Type INTEGER) AS
    BEGIN
      CASE
        WHEN p_Type = 1 THEN --Create index POINTTRANSACTIONID
            EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX IDX_POINTTRANSACTIONID ON BP_AE.X$_NR_EXPIREDPOINTS(POINTTRANSACTIONID)';
        WHEN p_Type = 2 THEN
           EXECUTE IMMEDIATE 'DROP INDEX IDX_POINTTRANSACTIONID';
        WHEN p_Type = 3 THEN --Create index TRANSACTIONDATE
            EXECUTE IMMEDIATE 'CREATE INDEX IDX_TRANSACTIONDATE ON BP_AE.X$_NR_EXPIREDPOINTS(TRANSACTIONDATE)';
        WHEN p_Type = 4 THEN
           EXECUTE IMMEDIATE 'DROP INDEX IDX_TRANSACTIONDATE';
        WHEN p_Type = 5 THEN --Create index ID
            EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX IDX_ID ON BP_AE.X$_NR_EXPIREDTIERS(ID)';
        WHEN p_Type = 6 THEN
           EXECUTE IMMEDIATE 'DROP INDEX IDX_ID';
        WHEN p_Type = 7 THEN --Create index MEMBERID
            EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX IDX_MEMBERID ON BP_AE.X$_NR_EXPIREDTIERS(MEMBERID)';
        WHEN p_Type = 8 THEN
           EXECUTE IMMEDIATE 'DROP INDEX IDX_MEMBERID';
        WHEN p_Type = 9 THEN --Create index ID
            EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX INDX_ID ON BP_AE.X$_AEO1638_TIERDUPLICATED(ID)';
        WHEN p_Type = 10 THEN
           EXECUTE IMMEDIATE 'DROP INDEX INDX_ID';
      END CASE;
  END Index_TempTable;

  PROCEDURE Delete_PointsTempTable(p_ProcessDate1 IN VARCHAR2, p_ProcessDate2 IN VARCHAR2, p_Line IN VARCHAR2) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    --Days
    V_PROCESSDATE1  TIMESTAMP := TO_DATE(p_ProcessDate1, 'YYYYMMddHH24miss');
    V_PROCESSDATE2  TIMESTAMP := TO_DATE(p_ProcessDate2, 'YYYYMMddHH24miss');
    V_START         NUMBER;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DELSTAGEPOINTS_' || p_Line;
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DELSTAGEPOINTS', v_processId);

      V_START := DBMS_UTILITY.get_time;

      --Delete staging points
      DELETE FROM bp_ae.X$_NR_EXPIREDPOINTS temp
      WHERE temp.transactiondate BETWEEN V_PROCESSDATE1 AND V_PROCESSDATE2;
      COMMIT;

      DBMS_OUTPUT.put_line('EXPIRE: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');
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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Delete_PointsTempTable_'|| p_Line,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Delete_PointsTempTable_'|| p_Line);

  END Delete_PointsTempTable;

--AEO-1637 End

--AEO-1638 BEGIN
  PROCEDURE Set_DefaultTier(p_mod INTEGER, p_instances INTEGER) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    v_ExpireDate  TIMESTAMP := TO_DATE('01/01/2019 23:59:59', 'mm/dd/yyyy HH24:MI:SS');
    V_START       NUMBER;
    V_ROWS        NUMBER;
    V_TIERID_FULLACCESS NUMBER;
    V_TIERID_EXTRA_ACCESS NUMBER;

    CURSOR get_data IS
    SELECT tmp.ipcode, tempo.tiername, tempo.id, tempo.todate, tempo.updatedate,
    CASE
      WHEN tempo.description IS NOT NULL THEN
           tempo.description
      ELSE
        CAST('Base' AS NVARCHAR2(150))
    END
    AS description,
    CASE
      WHEN tempo.NewTierID IS NOT NULL THEN
        tempo.NewTierID
      ELSE
        V_TIERID_FULLACCESS
    END
    AS NewTierID
    FROM bp_ae.X$_AEO_Connected_Members tmp
    LEFT OUTER JOIN
    (
      --Active tiers
      SELECT mt.memberid, t.tiername, mt.id, mt.todate, mt.updatedate, mt.description,
      CASE
        WHEN t.tiername = 'Blue' THEN
             V_TIERID_FULLACCESS
        WHEN t.tiername = 'Silver' THEN
             V_TIERID_EXTRA_ACCESS
      END AS NewTierID
      FROM bp_ae.lw_membertiers mt
      INNER JOIN bp_ae.lw_tiers t ON mt.tierid = t.tierid
      WHERE mt.todate > (TRUNC(SYSDATE+1) - 1/86400) --Curent date at 23:59:59
    ) tempo
    ON tempo.memberid = tmp.ipcode
    WHERE 1=1
    AND mod(tmp.ipcode, p_instances) = p_mod
    AND (tempo.tiername IN ('Blue','Silver') OR tempo.tiername IS NULL)--Members with blue or silver tier OR not active tier
    ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DEFAULTTIER';
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DEFAULTTIER', v_processId);

  V_START := DBMS_UTILITY.get_time;

--Get New tiers
SELECT Tr.Tierid
  INTO V_TIERID_FULLACCESS
 FROM bp_ae.Lw_Tiers Tr
WHERE Tr.Tiername = 'Full Access';

SELECT Tr.Tierid
  INTO V_TIERID_EXTRA_ACCESS
  FROM bp_ae.Lw_Tiers Tr
 WHERE Tr.Tiername = 'Extra Access';

-----------------------------------------------
--Add Members to the temporal table, for nuke--
-----------------------------------------------
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;
        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           MERGE INTO bp_ae.X$_NR_EXPIREDTIERS nxp
              USING (SELECT v_tbl(j).ipcode AS memberid,
              v_tbl(j).tiername AS tiername,
              v_tbl(j).id AS id,
              v_tbl(j).todate AS todate,
              v_tbl(j).updatedate AS updatedate,
              v_tbl(j).description AS description,
              v_tbl(j).newtierid AS newtierid
                  FROM dual) temp
              ON (nxp.id = temp.id)
            WHEN NOT MATCHED THEN
                 INSERT (memberid, tiername, id, todate, updatedate, description, newtierid)
                 VALUES (temp.memberid, temp.tiername, temp.id, temp.todate, temp.updatedate, temp.description, temp.newtierid)
            ;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;

      COMMIT;
      IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
      END IF;
------------------------------------
--Expire old tier and add new tier--
------------------------------------
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;
--Expire All Active Records
        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           UPDATE bp_ae.lw_membertiers mt
           SET mt.todate = systimestamp,
               mt.updatedate  = systimestamp
           WHERE mt.id = v_tbl(J).id
           ;

           COMMIT WRITE BATCH NOWAIT;

--Add New Tier
        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           INSERT INTO bp_ae.lw_membertiers mt (Id,
                       Tierid,
                       Memberid,
                       Fromdate,
                       Todate,
                       Description,
                       Createdate,
                       Updatedate)
           VALUES
                      (Hibernate_Sequence.Nextval,
                       v_tbl(j).newtierid,
                       v_tbl(j).ipcode,
                       systimestamp,
                       v_ExpireDate,
                       v_tbl(j).description,
                       systimestamp,
                       systimestamp)
           ;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;
     DBMS_OUTPUT.put_line('SET DEFAULT TIER: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Set_DefaultTier',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Set_DefaultTier');
  END Set_DefaultTier;

  PROCEDURE Restore_Tier(p_mod INTEGER, p_instances INTEGER) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    v_ExpireDate  TIMESTAMP := TO_DATE('01/01/2019 23:59:59', 'mm/dd/yyyy HH24:MI:SS');
    V_START       NUMBER;
    V_ROWS        NUMBER;

    CURSOR get_data IS
    SELECT *
    FROM bp_ae.X$_NR_EXPIREDTIERS tp
    WHERE 1=1
    AND mod(NVL(tp.id,0), p_instances) = p_mod
    ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DEFAULTTIER';
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DEFAULTTIER', v_processId);

  V_START := DBMS_UTILITY.get_time;

      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;

--Delete new tier
        FORALL j IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
               DELETE bp_ae.lw_membertiers mt
               WHERE 1=1
               AND mt.memberid = v_tbl(J).memberid
               AND mt.todate = v_ExpireDate
               ;

            COMMIT WRITE BATCH NOWAIT;

--Unexpire Tier
        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           MERGE INTO bp_ae.lw_membertiers lm
              USING (
                    SELECT v_tbl(j).id AS id, v_tbl(j).todate AS todate, v_tbl(j).updatedate AS updatedate
                    FROM dual
                    ) temp
              ON (lm.id = temp.id)
            WHEN MATCHED THEN
              UPDATE SET lm.todate = temp.todate,
                         lm.updatedate  = temp.updatedate
            ;

            COMMIT WRITE BATCH NOWAIT;

            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;
     DBMS_OUTPUT.put_line('RESTORE TIER: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Restore_Tier',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Restore_Tier');
  END Restore_Tier;

  PROCEDURE Add_NewSpend(p_mod INTEGER, p_instances INTEGER) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    V_START       NUMBER;

    V_EMAIL_FLAG  NUMBER := 0;
    V_ATTACHMENTS cio_mail.attachment_tbl_type;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

    CURSOR get_data IS
    SELECT mt.id, mt.memberid,NVL(md.a_netspend,0) AS a_netspend
    FROM bp_ae.X$_NR_EXPIREDTIERS tp
    INNER JOIN bp_ae.lw_membertiers mt
    ON mt.memberid = tp.memberid
    INNER JOIN bp_ae.ats_memberdetails md
    ON md.a_ipcode = mt.memberid
    WHERE 1=1
    AND mt.todate > (TRUNC(SYSDATE+1) - 1/86400) -- Current date @ 23:59:59
    AND mod(NVL(tp.id,0), p_instances) = p_mod
    ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DEFAULTTIER_ADDNEWSPEND_' || TO_CHAR(p_mod);
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DEFAULTTIER_ADDNEWSPEND_', v_processId);

     V_START := DBMS_UTILITY.get_time;
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;
        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           INSERT INTO bp_ae.ats_Membernetspend np
                   (
                   a_Rowkey,
                   Lwidentifier,
                   a_Ipcode,
                   a_Parentrowkey,
                   a_Membertierid,
                   a_Netspend,
                   Statuscode,
                   Createdate,
                   Updatedate,
                   a_changedby
                   )
             VALUES
                  (
                   Seq_Rowkey.Nextval,
                   0,
                   v_tbl(j).memberid,
                   -1,
                   v_tbl(j).id,
                   v_tbl(j).a_netspend,
                   0,
                   SYSDATE,
                   SYSDATE,
                   'nr_defaultier'
                   )
                   ;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;
     DBMS_OUTPUT.put_line('ADD NEW SPEND: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Add_NewSpend_'|| TO_CHAR(p_mod),
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Add_NewSpend_'|| TO_CHAR(p_mod));
    END Add_NewSpend;

    PROCEDURE Delete_NewSpend(p_mod INTEGER, p_instances INTEGER) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    V_EMAIL_FLAG  NUMBER := 0;
    V_ATTACHMENTS cio_mail.attachment_tbl_type;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

    V_START       NUMBER;

    CURSOR get_data IS
    SELECT mt.id, mt.memberid
    FROM bp_ae.X$_NR_EXPIREDTIERS tp
    INNER JOIN bp_ae.lw_membertiers mt
    ON mt.memberid = tp.memberid
    WHERE 1=1
    AND mt.todate > TRUNC(SYSDATE)
    AND mod(NVL(tp.id,0), p_instances) = p_mod
    ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DEFAULTTIER_DELNEWSPEND_' || TO_CHAR(p_mod);
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DEFAULTTIER_DELNEWSPEND_', v_processId);

     V_START := DBMS_UTILITY.get_time;
------------------
-- GET THE DATA --
------------------
    OPEN get_data;
    LOOP
      FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;
        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
               DELETE bp_ae.Ats_Membernetspend np
               WHERE 1=1
               AND np.a_ipcode = v_tbl(J).memberid
               AND np.a_membertierid = v_tbl(J).id
               ;

      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN get_data%NOTFOUND;
    END LOOP;

    IF get_data%ISOPEN THEN
      CLOSE get_data;
    END IF;

     DBMS_OUTPUT.put_line('DELETE NEW SPEND: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Delete_NewSpend_'|| TO_CHAR(p_mod),
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Delete_NewSpend_'|| TO_CHAR(p_mod));
  END Delete_NewSpend;

  PROCEDURE Update_DuplicateTiers AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    v_ExpireDate  DATE := TRUNC(SYSDATE) - 1/86400; --Date before to the current day at 23:59:59
    V_START       NUMBER;
    V_ROWS        NUMBER;
---------------------------------------------------------------------------
--@Prod on 08/14/2017 = 14 members with duplicates
---------------------------------------------------------------------------
--Expire duplicates
    CURSOR get_data IS
      SELECT * FROM
      (
        SELECT mt.id, mt.memberid, mt.tierid, mt.todate, mt.updatedate,
        Row_Number() Over(PARTITION BY mt.memberid ORDER BY mt.id DESC) AS r
        FROM bp_ae.lw_membertiers mt
        INNER JOIN bp_ae.lw_virtualcard vc
        ON vc.ipcode = mt.memberid AND vc.isprimary = 1
        WHERE 1=1
        AND mt.memberid IN
        (
            --Members with active tiers - duplicated
            SELECT mt.memberid
            FROM bp_ae.lw_membertiers mt
            WHERE 1=1
            AND mt.todate > (TRUNC(SYSDATE+1) - 1/86400) --Curent date at 23:59:59
            GROUP BY mt.memberid
            HAVING COUNT(mt.memberid) > 1
        )
        AND mt.todate > (TRUNC(SYSDATE+1) - 1/86400) --Curent date at 23:59:59
      )tmp
      WHERE 1=1
      AND r <> 1
      ORDER BY tmp.memberid
      ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DEFAULTTIER_DUPLICATETIER';
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DEFAULTTIER_DUPLICATETIER', v_processId);

    V_START := DBMS_UTILITY.get_time;

--Save tiers for nuke
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;

        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           INSERT INTO bp_ae.x$_aeo1638_tierduplicated
                  (
                  id,
                  memberid,
                  tierid,
                  todate,
                  updatedate
                  )
                  VALUES
                  (
                  v_tbl(J).id,
                  v_tbl(J).memberid,
                  v_tbl(J).tierid,
                  v_tbl(J).todate,
                  v_tbl(J).updatedate
                  );

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;

--Expire records
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;

        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           UPDATE bp_ae.lw_membertiers mt
              SET mt.todate = v_ExpireDate,
              mt.updatedate  = systimestamp
           WHERE mt.id = v_tbl(J).id
            ;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;
     DBMS_OUTPUT.put_line('UPDATE DUPLICATE TIERS: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Update_DuplicateTiers',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Update_DuplicateTiers');
  END Update_DuplicateTiers;

  PROCEDURE Restore_DuplicateTiers AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    v_ExpireDate  TIMESTAMP := TO_DATE('01/01/2019 23:59:59', 'mm/dd/yyyy HH24:MI:SS');
    V_START       NUMBER;
    V_ROWS        NUMBER;
    V_MEMBERID    NUMBER(20) := 0;

--Member with their different tiers
    CURSOR get_data IS
    SELECT *
    FROM bp_ae.x$_aeo1638_tierduplicated dt
    WHERE 1=1
    ;

    TYPE t_tab IS TABLE OF get_data%ROWTYPE;
    v_tbl t_tab;

  BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'NR_DEFAULTTIER_RESTOREDUPTIER';
  v_logsource := v_jobname;

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
  utility_pkg.Log_Process_Start(v_jobname, 'NR_DEFAULTTIER_RESTOREDUPTIER', v_processId);

     V_START := DBMS_UTILITY.get_time;

/*UnExpire old tiers*/
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT INTO v_tbl LIMIT 1000;

        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS
           UPDATE bp_ae.lw_membertiers mt
              SET mt.todate = v_tbl(J).todate,
              mt.updatedate  = v_tbl(J).updatedate
           WHERE mt.id = v_tbl(J).id
            ;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF GET_DATA%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE GET_DATA;
        END IF;
     DBMS_OUTPUT.put_line('RESTORE DUPLICATE TIERS: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');

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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'Restore_DuplicateTiers',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in Restore_DuplicateTiers');
  END Restore_DuplicateTiers;

    PROCEDURE LoadCSAgents(P_FILENAME IN VARCHAR2) IS

--log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    v_ExpireDate  TIMESTAMP := TO_DATE('01/01/2019 23:59:59', 'mm/dd/yyyy HH24:MI:SS');
    V_START       NUMBER;
    V_ROWS        NUMBER;
    V_MEMBERID    NUMBER(20) := 0;

--Existing csagents will be included in this cursor for update
    CURSOR ex_agent IS
    SELECT TRIM(cs.username) username, cs.roleid, cs.status
    FROM bp_ae.lw_csagent_stage cs, bp_ae.lw_csagent ca
    WHERE 1=1
	AND cs.username = ca.username
    ;

--Not Existing csagents will be included in this cursor	for insert
	CURSOR new_agent IS
    SELECT TRIM(cs.username) username, cs.roleid, cs.status, TRIM(cs.firstname) firstname, REGEXP_REPLACE(cs.lastname, '\s+$') lastname
    FROM bp_ae.lw_csagent_stage cs
    WHERE 1=1
	AND NOT EXISTS(
		SELECT 1
		FROM bp_ae.lw_csagent ca
		WHERE 1=1
		AND ca.username = cs.username
	)
    ;

    TYPE t_tab IS TABLE OF ex_agent%ROWTYPE;
    v_tbl t_tab;

	TYPE t_tab2 IS TABLE OF new_agent%ROWTYPE;
    v_tbl2 t_tab2;

 BEGIN
  /*Logging*/
  v_my_log_id := utility_pkg.get_LIBJobID();
  v_jobname := 'LOAD_CSAGENTS';
  v_logsource := v_jobname;

  /* log start of job */
  utility_pkg.Log_job(P_JOB    => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,P_FILENAME           => P_FILENAME
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
  utility_pkg.Log_Process_Start(v_jobname, 'LOAD_CSAGENTS', v_processId);

     V_START := DBMS_UTILITY.get_time;

	 CHANGEEXTERNALTABLE(P_FILENAME,'EXT_CSAGENT_STAGE');

	 EXECUTE IMMEDIATE 'TRUNCATE TABLE bp_ae.lw_csagent_stage';

	 INSERT INTO bp_ae.lw_csagent_stage
	 SELECT
	 s.username,
     c.id roleid,
     s.status,
     s.firstname,
     s.lastname
	 FROM bp_ae.EXT_CSAGENT_STAGE s,
	      bp_ae.lw_csrole c
	 WHERE 1=1
     AND CASE roleid
			   WHEN '1' THEN 'Supervisor'
			   WHEN '2' THEN 'CSR'
			   WHEN '3' THEN 'Super Admin'
			   WHEN '4' THEN 'Loss Prevention'
			   WHEN '5' THEN 'Synchrony'
			   ELSE 'CSR'
         END = c.name;
	 COMMIT;


	  OPEN ex_agent;
      LOOP
        FETCH ex_agent BULK COLLECT INTO v_tbl LIMIT 1000;

        FORALL J IN 1 .. v_tbl.COUNT SAVE EXCEPTIONS

           UPDATE bp_ae.lw_csagent ca
              SET ca.roleid = v_tbl(J).roleid,
				  ca.status  = v_tbl(J).status,
				  ca.updatedate = systimestamp
           WHERE ca.username = v_tbl(J).username
            ;

            COMMIT WRITE BATCH NOWAIT;
            EXIT WHEN ex_agent%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        IF ex_agent%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE ex_agent;
        END IF;
     DBMS_OUTPUT.put_line('UPDATE CS AGENTS: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');
	 V_START := DBMS_UTILITY.get_time;

	  OPEN new_agent;
      LOOP
        FETCH new_agent BULK COLLECT INTO v_tbl2 LIMIT 1000;

        FORALL J IN 1 .. v_tbl2.COUNT SAVE EXCEPTIONS

              INSERT INTO bp_ae.lw_csagent ca (
           ca.id,
           roleid,
           firstname,
           lastname,
           username,
           password,
           passwordchangerequired,
           passwordexpiredate,
           status,
           createdby,
           createdate,
           updatedate,
           salt
           )
           VALUES
           (
           Hibernate_Sequence.Nextval,
           v_tbl2(J).roleid,
           v_tbl2(J).firstname,
           v_tbl2(J).lastname,
           v_tbl2(J).username,
           'muvlAUdjQ6A0Q6HpVoPBRKu9aADiNZGZA+R5x0635kg=',
           1,
           ADD_MONTHS(systimestamp,3),
           v_tbl2(J).status,
           0,
           systimestamp,
           systimestamp,
           'S3TNStUbFuOi1Wjm6quwgyStxDKNk05izM6taEf0ThA='
           );

            COMMIT WRITE BATCH NOWAIT;

            EXIT WHEN new_agent%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
        END LOOP;

        COMMIT;
        IF new_agent%ISOPEN THEN
          --<--- dont forget to close cursor since we manually opened it.
          CLOSE new_agent;
        END IF;
     DBMS_OUTPUT.put_line('INSERT CS AGENTS: ' || (DBMS_UTILITY.get_time - V_START) || ' hsecs');


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
  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'LoadCSAgents',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in LoadCSAgents');
  END LoadCSAgents;

--AEO-1638 END

/* AEO-1635 Begin */
  PROCEDURE StageBonusLoader(V_MY_LOG_ID IN NUMBER, V_JOBNAME IN VARCHAR2, p_fileName IN NVARCHAR2, P_PEVENTNAME OUT VARCHAR2) AS
    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    /* local variables */
    v_PointTypeName      VARCHAR2(100);
    v_PointEventName     VARCHAR2(100);
    v_PointTypeID        NUMBER(20) := 0;
    v_PointEventID       NUMBER(20) := 0;
    v_Counter            INTEGER;
    v_row_valid          NUMBER := 0;
    v_Bonus_Rec          TYPE_BONUSLOAD;

       CURSOR c IS
        --Skip first record for the header
        SELECT loyaltyid, points, Row_Number() Over(PARTITION BY loyaltyid ORDER BY loyaltyid DESC) AS reco
        FROM
        (
          SELECT ebbl.field1 AS loyaltyid,
          ebbl.field2 AS points,rownum r
          FROM bp_ae.Ext_Bulkbonusloader ebbl
        )
        WHERE 1=1
        AND r > 1
        ;
       TYPE c_type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
       rec c_type;
    BEGIN

    utility_pkg.Log_Process_Start(v_jobname, 'StageBonusLoader', v_processId); --AEO-2169

      /* Get Bonus Loader Point Type Name and Point Event Name */
      SELECT ebbl.field1 AS pointTypeName, ebbl.field2 AS pointEventName
      INTO v_PointTypeName, v_PointEventName
      FROM bp_ae.ext_bulkbonusloader ebbl
      WHERE 1=1
      AND ROWNUM = 1;

      /* check for pointtype */
      SELECT count(*) INTO v_Counter
      FROM BP_AE.lw_pointtype p
      WHERE 1=1
      AND UPPER(nvl(p.name, '')) = UPPER(v_PointTypeName);

      IF v_Counter = 1 THEN
        SELECT p.pointtypeid
        INTO v_PointTypeID
        FROM bp_ae.Lw_Pointtype p
        WHERE 1=1
        AND UPPER(p.name) = UPPER(v_PointTypeName); --<--- use whatever they gave us if it exists already
      ELSE
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
        utility_pkg.Log_job(P_JOB                => v_my_log_id
             ,p_jobdirection       => v_jobdirection
             ,p_filename           => null
             ,p_starttime          => v_starttime
             ,p_endtime            => v_endtime
             ,p_messagesreceived   => v_messagesreceived
             ,p_messagesfailed     => v_messagesfailed
             ,p_jobstatus          => v_jobstatus
             ,p_jobname            => v_jobname);
        V_ERROR := SQLERRM;
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                              P_ENVKEY    => V_ENVKEY,
                              P_LOGSOURCE => 'StageBonusLoader',
                              P_FILENAME  => NULL,
                              P_BATCHID   => V_BATCHID,
                              P_JOBNUMBER => V_MY_LOG_ID,
                              P_MESSAGE   => V_MESSAGE,
                              P_REASON    => V_REASON,
                              P_ERROR     => V_ERROR,
                              P_TRYCOUNT  => V_TRYCOUNT,
                              P_MSGTIME   => SYSDATE);

        RAISE_APPLICATION_ERROR(-20002,
                                'PointType ' || v_PointTypeName || ' does not exist in lw_pointtype');
      END IF;

      /* check for pointevent */
      SELECT count(*)
      INTO v_Counter
      FROM BP_AE.lw_pointevent p
      WHERE 1=1
      AND UPPER(nvl(p.name, '')) = UPPER(v_PointEventName);

      IF v_Counter = 1 THEN
        SELECT p.pointeventid, p.name
        INTO v_PointEventID, P_PEVENTNAME
        FROM bp_ae.lw_pointevent p
        WHERE 1=1
        AND UPPER(p.name) = UPPER(v_PointEventName); --<--- use whatever they gave us if it exists already
      ELSE
        /* Create new pointevent with new name */
        INSERT into bp_ae.lw_pointevent (
               pointeventid ,
               NAME,
               DESCRIPTION ,
               createdate
        )
        VALUES (
               Hibernate_Sequence.Nextval ,
               v_PointEventName,
               v_PointEventName,
               SYSDATE
         );
         COMMIT;

         SELECT p.pointeventid, p.name
         INTO v_PointEventID, P_PEVENTNAME
         FROM bp_ae.lw_pointevent p
         WHERE 1=1
         AND UPPER(p.name) = UPPER(v_PointEventName);
      END IF;

      /* Clear staging table */
      EXECUTE IMMEDIATE 'truncate table bp_ae.lw_bonusloader_stage';      

      OPEN c;
      LOOP
        FETCH c BULK COLLECT
        INTO rec LIMIT 1000;
        FOR i IN 1 .. rec.COUNT LOOP
            --Reset the txn, we're nulling out the arrays and reseting variables for the new txn
            v_Bonus_Rec := Initialize_Bonus_Rec();
            --Fill out type for validate data
            v_Bonus_Rec.POINTTYPE := v_PointTypeID ;
            v_Bonus_Rec.POINTEVENT := v_PointEventID;
            v_Bonus_Rec.PEVENTNAME := v_PointEventName;
            v_Bonus_Rec.LOYALTYNUMBER := rec(i).loyaltyid;
            v_Bonus_Rec.POINTS := rec(i).points;
            v_Bonus_Rec.FILENAME := p_fileName;
            v_Bonus_Rec.NUMREC := rec(i).reco;
            v_Bonus_Rec.VCKEY := NULL;

           v_row_valid := validate_row_BonusLoad(v_Bonus_Rec);
           IF (v_row_valid <> -1) THEN
              INSERT INTO bp_ae.lw_bonusloader_stage(
                     pointtype,
                     pointevent,
                     loyaltynumber,
                     points,
                     vckey
              ) VALUES (
                     v_PointTypeID,
                     v_PointEventID,
                     rec(i).loyaltyid,
                     rec(i).points,
                     v_Bonus_Rec.VCKEY
              );
              
              v_messagesreceived := v_messagesreceived + 1;
           END IF;
        END LOOP;
        COMMIT;
        EXIT WHEN c%NOTFOUND;
      END LOOP;
    COMMIT;

    IF c%ISOPEN THEN
      CLOSE c;
    END IF;

    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived); --AEO-2169

  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'StageBonusLoader',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in StageBonusLoader');


  END StageBonusLoader;

  PROCEDURE BulkBonusLoader(p_fileName IN NVARCHAR2, retval IN OUT rcursor) AS
    --log job attributes
    V_MY_LOG_ID        NUMBER := 0;
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;
    V_ERRORMESSAGE     VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID   VARCHAR2(256);
    V_ENVKEY      VARCHAR2(256) := 'bp_ae@' ||
                                   UPPER(SYS_CONTEXT('userenv',
                                                     'instance_name'));
    V_LOGSOURCE   VARCHAR2(256);
    V_BATCHID     VARCHAR2(256) := 0;
    V_MESSAGE     VARCHAR2(256) := ' ';
    V_REASON      VARCHAR2(256);
    V_ERROR       VARCHAR2(256);
    V_TRYCOUNT    NUMBER := 0;
    V_RECORDCOUNT NUMBER := 0;

    V_PEVENTNAME         VARCHAR2(100);
    V_PTNotes            VARCHAR2(255) := NULL;

    CURSOR c IS
      SELECT
          bls.loyaltynumber AS loyaltyid,
          bls.points AS points,
          bls.pointtype AS pointtypeid,
          bls.pointevent AS pointeventid,
          bls.vckey
      FROM bp_ae.lw_bonusloader_stage bls
      WHERE 1=1;

    TYPE c_type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
    rec c_type;

  BEGIN
    /*Logging*/
    v_my_log_id := utility_pkg.get_LIBJobID();
    V_JOBNAME := 'BulkBonusLoader';
    v_logsource := v_jobname;

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

    /*
     * Load file to staging table so stagebonusloader can do its thing.
     * lw_bonusloader_stage
     *       field 1: pointtype, loyaltyid
     *       field 2: pointevent, points
     */
     V_MESSAGE := 'Loading data from ' || p_fileName || ' to ext_bulkbonusloader';
     V_ERRORMESSAGE := 'Error loading ' || p_filename || ' to ext_bulkbonusloader (Does file exist?)';
     CHANGEEXTERNALTABLE(P_FILENAME,'ext_bulkbonusloader');
     --AEO-2169 Begin - GD
     STAGEBONUSLOADER(V_MY_LOG_ID,V_JOBNAME, p_fileName, V_PEVENTNAME); 

     utility_pkg.Log_Process_Start(v_jobname, 'BulkBonusLoader', v_processId);
     V_PTNotes := V_PEVENTNAME || ' on ' || p_fileName;

     OPEN c;
     LOOP
          FETCH c BULK COLLECT INTO rec LIMIT 1000;
          FORALL i IN 1 .. rec.Count SAVE EXCEPTIONS
          INSERT INTO bp_ae.Lw_Pointtransaction
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
                          rec(i).Vckey,
                          rec(i).pointtypeid,
                          rec(i).pointeventid,
                          1,
                          SYSDATE,
                          SYSDATE,
                          rec(i).points,
                          to_date('12/31/2199','mm/dd/yyyy'),--new expiration date
                          V_PTNotes, /*Notes*/
                          -1,
                          1,
                          -1,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'BulkBonusLoader' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          ;
      COMMIT WRITE BATCH NOWAIT;
      EXIT WHEN c%NOTFOUND;
     END LOOP;

     COMMIT;
     CLOSE c;

    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);    
    --AEO-2169 End - GD
    
    v_endtime := SYSDATE;
    v_jobstatus := 1;
    /* log end of job */
    utility_pkg.Log_job(P_JOB                => v_my_log_id
           ,p_jobdirection       => v_jobdirection
           ,p_filename           => p_Filename
           ,p_starttime          => v_starttime
           ,p_endtime            => v_endtime
           ,p_messagesreceived   => v_messagesreceived
           ,p_messagesfailed     => v_messagesfailed
           ,p_jobstatus          => v_jobstatus
           ,p_jobname            => v_jobname);

  EXCEPTION
    WHEN OTHERS THEN
       V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        v_endtime := SYSDATE;
         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);
      V_ERROR := SQLERRM;
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'BulkBonusLoader',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,
                              'Other Exception detected in BulkBonusLoader');
  END BulkBonusLoader;
/* AEO-1635 End */

  PROCEDURE LoadSynchronyAccountKeys (retval IN OUT RCURSOR) IS
    v_Logsource        VARCHAR2(256) := 'AE_Transition.LoadSynchronyAccountKeys';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AE_Transition.LoadSynchronyAccountKeys';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'LoadSynchronyAccountKeys';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    Err_Code           VARCHAR(512) := NULL;
    Err_Msg            VARCHAR2(200):= NULL;
    V_TRYCOUNT         NUMBER := 0;
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

         -- drop the backup table
        BEGIN
           EXECUTE IMMEDIATE 'DROP TABLE X$_AccountKeys_Backup';
        EXCEPTION
           WHEN OTHERS THEN
              IF SQLCODE != -942 THEN
                 RAISE;
              END IF;
        END;

         -- create the backup table
         BEGIN
           EXECUTE IMMEDIATE '
              CREATE TABLE bp_ae.X$_AccountKeys_Backup AS
              select a_rowkey, a_vckey, a_parentrowkey, a_accountkey, statuscode, createdate, updatedate, lastdmlid
              from ats_synchronyaccountkey
              where a_accountkey is not null
           ';
           EXCEPTION
                   WHEN OTHERS THEN
                        Err_Code  := SQLCODE;
                        Err_Msg   := Substr(SQLERRM, 1, 200);
                        v_Reason  := ' CREATE TABLE bp_ae.X$_AccountKeys_Backup ' || Err_Code || ' ' || Err_Msg;
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


    -- clear out the Account Key table
    EXECUTE IMMEDIATE 'Truncate Table ats_synchronyaccountkey';

    ---------------------------
    -- Ok, now populate the Account Key table with the new Account Keys from Synchrony file.
    ---------------------------
    DECLARE
      CURSOR get_data IS
      select t.sync_accountkey, vc.vckey
        from
        (
        select s.acct_key as Sync_AccountKey, vc.ipcode
        from Bp_Ae.Ae_Synchrony s
            left join bp_ae.lw_virtualcard vc on s.cardholder_loyalty_id = vc.loyaltyidnumber
        where     1=1
        and       vc.loyaltyidnumber is not null
        ) t
        inner join bp_ae.lw_virtualcard vc on t.ipcode = vc.ipcode and vc.isprimary = 1
        ;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          insert into ats_synchronyaccountkey (a_rowkey, a_vckey, a_parentrowkey, a_accountkey, statuscode, createdate, updatedate)
          values (hibernate_sequence.nextval, v_tbl(i).vckey, v_tbl(i).vckey, v_tbl(i).sync_accountkey, 0, sysdate, sysdate);
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
      IF v_Messagesfailed = 0

       THEN
        v_Messagesfailed := 1;
      END IF;
      v_Jobstatus := 3;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => v_Filename,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => sysdate,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure LoadSynchronyAccountKeys';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AE_Transition</pkg>' || Chr(10) ||
                          '    <proc>LoadSynchronyAccountKeys</proc>' ||
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
  END LoadSynchronyAccountKeys;

  PROCEDURE LoadSynchronyAccountKeys_Bk (retval IN OUT RCURSOR) IS
    v_Logsource        VARCHAR2(256) := 'AE_Transition.LoadSynchronyAccountKeys_Bk';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AE_Transition.LoadSynchronyAccountKeys_Bk';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'LoadSynchronyAccountKeys_Bk';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    Err_Code           VARCHAR(512) := NULL;
    Err_Msg            VARCHAR2(200):= NULL;
    V_TRYCOUNT         NUMBER := 0;
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

    -- clear out the Account Key table
    EXECUTE IMMEDIATE 'Truncate Table ats_synchronyaccountkey';

    ---------------------------
    -- Ok, now populate the Account Key table with the new Account Keys from Synchrony file.
    ---------------------------
    DECLARE
      CURSOR get_data IS
      select ab.a_rowkey, ab.a_vckey, ab.a_parentrowkey, ab.a_accountkey, ab.statuscode, ab.createdate, ab.updatedate, ab.lastdmlid
        from X$_AccountKeys_Backup ab
        ;
      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT
          INTO v_tbl LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          insert into ats_synchronyaccountkey (a_rowkey, a_vckey, a_parentrowkey, a_accountkey, statuscode, createdate, updatedate, lastdmlid)
          values (v_tbl(i).a_rowkey, v_tbl(i).a_vckey, v_tbl(i).a_parentrowkey, v_tbl(i).a_accountkey, v_tbl(i).statuscode, v_tbl(i).createdate, v_tbl(i).updatedate, v_tbl(i).lastdmlid);
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
      IF v_Messagesfailed = 0

       THEN
        v_Messagesfailed := 1;
      END IF;
      v_Jobstatus := 3;
      Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => v_Filename,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => sysdate,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);
      v_Messagesfailed := v_Messagesfailed + 1;
      v_Error          := SQLERRM;
      v_Reason         := 'Failed Procedure LoadSynchronyAccountKeys_Bk';
      v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                          '    <pkg>AE_Transition</pkg>' || Chr(10) ||
                          '    <proc>LoadSynchronyAccountKeys_Bk</proc>' ||
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
  END LoadSynchronyAccountKeys_Bk;

/* AEO-1844 BEGIN - AH */
  PROCEDURE UpdateSignUpBonus(retval IN OUT RCURSOR) IS
    v_Logsource        VARCHAR2(256) := 'AE_Transition.UpdateSignUpBonus';
    v_My_Log_Id        NUMBER;
    v_Jobdirection     NUMBER := 0;
    v_Filename         VARCHAR2(512) := 'AE_Transition.UpdateSignUpBonus';
    v_Starttime        DATE := SYSDATE;
    v_Endtime          DATE;
    v_Messagesreceived NUMBER := 0;
    v_Messagesfailed   NUMBER := 0;
    v_Messagespassed   NUMBER := 0;
    v_Jobstatus        NUMBER := 0;
    v_Jobname          VARCHAR2(256) := 'UpdateSignUpBonus';
    v_Batchid          VARCHAR2(256) := 0;
    v_Message          VARCHAR2(256);
    v_Reason           VARCHAR2(256);
    v_Error            VARCHAR2(256);
    v_Messageid        NUMBER;
    Err_Code           VARCHAR(512) := NULL;
    Err_Msg            VARCHAR2(200):= NULL;
    V_TRYCOUNT         NUMBER := 0;
    v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                        Upper(Sys_Context('userenv',
                                                          'instance_name'));

    v_BonusPointTypeID                  NUMBER(20);


    CURSOR c IS
      SELECT p.pointtransactionid AS pointTxnID
      FROM bp_ae.lw_pointtransaction p
      WHERE 1=1
      AND p.pointtypeid = (SELECT pt.pointtypeid
                           FROM bp_ae.Lw_Pointtype pt
                           WHERE 1=1
                           AND pt.name = 'AEO Connected Sign Up Bonus');

    TYPE c_type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
    rec c_type;
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

      SELECT p.pointtypeid INTO v_BonusPointTypeID
      FROM bp_ae.Lw_Pointtype p
      WHERE 1=1
      AND p.name = 'AEO Connected Bonus Points';

      OPEN c;
      LOOP
        FETCH c BULK COLLECT
        INTO rec LIMIT 1000;
        FOR i IN 1 .. rec.COUNT LOOP
          UPDATE bp_ae.lw_pointtransaction p
          SET p.pointtypeid = v_BonusPointTypeID
          WHERE 1=1
          AND p.pointtransactionid = rec(i).pointTxnID;
        END LOOP;
        COMMIT;

        EXIT WHEN c%NOTFOUND;
      END LOOP;

      IF c%ISOPEN THEN
        CLOSE c;
      END IF;
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
                            p_Endtime          => sysdate,
                            p_Messagesreceived => v_Messagesreceived,
                            p_Messagesfailed   => v_Messagesfailed,
                            p_Jobstatus        => v_Jobstatus,
                            p_Jobname          => v_Jobname);
        v_Messagesfailed := v_Messagesfailed + 1;
        v_Error          := SQLERRM;
        v_Reason         := 'Failed Procedure UpdateSignUpBonus';
        v_Message        := '<failed>' || Chr(10) || '  <details>' || Chr(10) ||
                            '    <pkg>AE_Transition</pkg>' || Chr(10) ||
                            '    <proc>UpdateSignUpBonus</proc>' ||
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
  END UpdateSignUpBonus;

END AE_TRANSITION;
/
