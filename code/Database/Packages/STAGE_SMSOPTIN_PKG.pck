CREATE OR REPLACE PACKAGE STAGE_SMSOPTIN_PKG IS
  TYPE RCURSOR IS REF CURSOR;

  PROCEDURE STAGE_SMSOPTIN(P_FILENAME IN VARCHAR2,
                           RETVAL     IN OUT RCURSOR);
 



END STAGE_SMSOPTIN_PKG;
/
CREATE OR REPLACE PACKAGE BODY STAGE_SMSOPTIN_PKG IS


  /********* Procedure to map external table to specified file ********/
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

    V_SQL := 'ALTER TABLE EXT_SMSOPTIN_LOAD LOCATION (AE_IN' || CHR(58) || '''' ||
             P_FILENAME || ''')';
    EXECUTE IMMEDIATE V_SQL;

  END CHANGEEXTERNALTABLE;
  
  
  

  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE STAGE_SMSOPTIN(P_FILENAME IN VARCHAR2,
                            RETVAL     IN OUT RCURSOR) IS
                            
   
    
    CURSOR GET_DATA  IS
        SELECT Ext.External_Person_Id, Ext.Opt_In_Date, Ext.Opt_Out_Date
        FROM   Bp_Ae.Ext_Smsoptin_Load Ext
        WHERE  Ext.External_Person_Id IS NOT NULL;
        
      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;


    V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

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

  -- change the external table file to the one whiose name was specified as paramters
  
   CHANGEEXTERNALTABLE(P_FILENAME => P_FILENAME);
   
   
    /* get job id for this process and the dap process */
    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    V_JOBNAME   := 'SMSOptIn';
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


-----
-- clear OUT THE staging table
-----
   DELETE STAGE_SMSOPTIN;
   COMMIT;
   
   ----
   -- Populates the stagin tables with the data form the externl table
   ----
   OPEN GET_DATA;

   LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 1000;

          FORALL I IN 1 .. V_TBL.COUNT  --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
             INSERT INTO Bp_Ae.Stage_Smsoptin
                  (Loyaltynumber, opt_in_date, opt_out_date)
             VALUES
                  (v_Tbl(i).external_person_id, 
				     to_Timestamp_tz(v_Tbl(i).opt_in_date,'yyyy-mm-dd HH24:MI:SS TZH:TZM'), 
					 to_Timestamp_tz(v_Tbl(i).opt_out_date,'yyyy-mm-dd HH24:MI:SS TZH:TZM'));

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
   END LOOP;
   COMMIT;
   IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
   END IF;
	  

   
----
    DECLARE
      /* check log file for errors */
      LV_ERR VARCHAR2(4000);
      LV_N   NUMBER;
    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM EXT_SMSOPTIN_LOAD_LOG' || CHR(10) ||
                        'WHERE rec LIKE ''ORA-%'''
        INTO LV_N, LV_ERR;

      IF LV_N > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        V_MESSAGESFAILED := V_MESSAGESFAILED + LV_N;
        V_REASON         := 'Failed reads by external table';
        V_MESSAGE        := '<StageProc>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>EXT_SMSOPTIN_LOAD' ||
                            CHR(10) ||
                            '    <Stage>EXT_SMSOPTIN_LOAD</Stage>' ||
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
                        P_JOBNAME          => 'Stage-' || V_JOBNAME);

    OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;
      
  EXCEPTION
    WHEN DML_ERRORS THEN

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE);
        V_REASON         := 'Failed Records in Procedure stage_smsoptin';
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
      V_JOBSTATUS := -1;

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
      V_JOBSTATUS := -1;
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
                   '    <pkg>stage_SMSOPTIN</pkg>' || CHR(10) ||
                   '    <proc>stage_SMSOPTIN</proc>' || CHR(10) ||
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

  END stage_SMSOPTIN;

END STAGE_SMSOPTIN_PKG;
/
