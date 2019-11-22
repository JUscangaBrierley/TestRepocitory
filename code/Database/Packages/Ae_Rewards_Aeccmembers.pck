CREATE OR REPLACE PACKAGE Ae_Rewards_Aeccmembers AS

     TYPE Rcursor IS REF CURSOR;

     PROCEDURE Issueaecc_Rewards(p_Typecode VARCHAR2,
                                 Retval     IN OUT Rcursor);

     PROCEDURE Consume_Barcodes(p_Dummy VARCHAR2,
                                Retval  IN OUT Rcursor);

     PROCEDURE Backup_Barcode_Assignment;

     PROCEDURE Assign_Barcodes;

     PROCEDURE Identify_Population;

     PROCEDURE Select_Barcodes (p_Typecode IN Bp_Ae.Ats_Rewardbarcodes.a_Typecode%TYPE);

     PROCEDURE Issue_rewards (p_DateIssued IN Bp_Ae.Lw_Memberrewards.Dateissued%TYPE); -- AEO-674
     PROCEDURE Issueaecc_ReminderRewards(p_Typecode VARCHAR2,
                                 Retval     IN OUT Rcursor);

     PROCEDURE Identify_RemindersPopulation;

     PROCEDURE Select_RemindersBarcodes (p_Typecode IN Bp_Ae.Ats_Rewardbarcodes.a_Typecode%TYPE);

     PROCEDURE Backup_reminderBarcode_Assign;

     PROCEDURE Assign_Barcodes_Reminder;

     PROCEDURE Issue_Reminderrewards (p_DateIssued IN Bp_Ae.Lw_Memberrewards.Dateissued%TYPE);

     PROCEDURE Consume_Barcodes_Reminder(p_Dummy VARCHAR2,
                                Retval  IN OUT Rcursor);


END Ae_Rewards_Aeccmembers;
/
CREATE OR REPLACE PACKAGE BODY Ae_Rewards_Aeccmembers IS

     PROCEDURE Issueaecc_Rewards(p_Typecode VARCHAR2,
                                 Retval     IN OUT Rcursor) IS
          -- Wraper procedure to identify the AECC population and attach bar codes
          -- cnelson 12/17/2015
     BEGIN
          Identify_Population;
          Select_Barcodes(p_Typecode => p_Typecode);
          Backup_Barcode_Assignment;
          Assign_Barcodes;
          Issue_Rewards(SYSDATE); -- AEO-676
          --consume_barcodes;   -- This is called externally; do not wrap into this call.
     END Issueaecc_Rewards;

     PROCEDURE Backup_Barcode_Assignment IS
          v_Cnt NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          /*cursor1  for backing up the previous run data*/
          CURSOR Cur_Rwd IS
               SELECT * FROM Bp_Ae.Ae_Aecc_20reward;
          TYPE t_Tab IS TABLE OF Cur_Rwd%ROWTYPE;
          --    TYPE t_tab2 IS TABLE OF cur_barcd%ROWTYPE;
          v_Tbl t_Tab;
          --    v_tbl2  t_tab2;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := Upper('backup_barcode_assignment');
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
          OPEN Cur_Rwd;
          LOOP
               FETCH Cur_Rwd BULK COLLECT
                    INTO v_Tbl LIMIT 1000;
               /*Backup*/
               FORALL j IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Ae_Aecc_20reward_Bkp
                         (Channel,
                          Customer_Nbr,
                          Loyalty_Number,
                          Email,
                          Mobile_Number,
                          Fname,
                          Lname,
                          Address1,
                          Address2,
                          City,
                          State,
                          Zip,
                          Country,
                          Auth_Cd,
                          Suc_Cd,
                          Campaign_Type,
                          Campaign_Exp_Date,
                          Eid,
                          Campaign_Id,
                          Language_Preference,
                          Gender,
                          Birthdate,
                          Store_Loyalty,
                          Tier_Status,
                          Points_Balance,
                          Points_Needed_For_Next_Reward,
                          Number_Of_Bras_Purchased,
                          Credits_To_Next_Free_Bra,
                          Number_Of_Jeans_Purchased,
                          Credits_To_Next_Free_Jean,
                          Number_Of_Active_5_Off_Reward,
                          Number_Of_Jeans_Reward,
                          Number_Bra_Reward,
                          Communication_Id,
                          Comm_Plan_Id,
                          Collateral_Id,
                          Package_Id,
                          Step_Id,
                          Message_Id,
                          Seg_Id,
                          Seg_Nm,
                          Aap_Flag,
                          Card_Type,
                          Run_Id,
                          Lead_Key_Id,
                          Site_Url,
                          Enable_Passbook_Pass,
                          TIMESTAMP)
                    VALUES
                         (v_Tbl(j).Channel,
                          v_Tbl(j).Customer_Nbr,
                          v_Tbl(j).Loyalty_Number,
                          v_Tbl(j).Email,
                          v_Tbl(j).Mobile_Number,
                          v_Tbl(j).Fname,
                          v_Tbl(j).Lname,
                          v_Tbl(j).Address1,
                          v_Tbl(j).Address2,
                          v_Tbl(j).City,
                          v_Tbl(j).State,
                          v_Tbl(j).Zip,
                          v_Tbl(j).Country,
                          v_Tbl(j).Auth_Cd,
                          v_Tbl(j).Suc_Cd,
                          v_Tbl(j).Campaign_Type,
                          v_Tbl(j).Campaign_Exp_Date,
                          v_Tbl(j).Eid,
                          v_Tbl(j).Campaign_Id,
                          v_Tbl(j).Language_Preference,
                          v_Tbl(j).Gender,
                          v_Tbl(j).Birthdate,
                          v_Tbl(j).Store_Loyalty,
                          v_Tbl(j).Tier_Status,
                          v_Tbl(j).Points_Balance,
                          v_Tbl(j).Points_Needed_For_Next_Reward,
                          v_Tbl(j).Number_Of_Bras_Purchased,
                          v_Tbl(j).Credits_To_Next_Free_Bra,
                          v_Tbl(j).Number_Of_Jeans_Purchased,
                          v_Tbl(j).Credits_To_Next_Free_Jean,
                          v_Tbl(j).Number_Of_Active_5_Off_Reward,
                          v_Tbl(j).Number_Of_Jeans_Reward,
                          v_Tbl(j).Number_Bra_Reward,
                          v_Tbl(j).Communication_Id,
                          v_Tbl(j).Comm_Plan_Id,
                          v_Tbl(j).Collateral_Id,
                          v_Tbl(j).Package_Id,
                          v_Tbl(j).Step_Id,
                          v_Tbl(j).Message_Id,
                          v_Tbl(j).Seg_Id,
                          v_Tbl(j).Seg_Nm,
                          v_Tbl(j).Aap_Flag,
                          v_Tbl(j).Card_Type,
                          v_Tbl(j).Run_Id,
                          v_Tbl(j).Lead_Key_Id,
                          v_Tbl(j).Site_Url,
                          v_Tbl(j).Enable_Passbook_Pass,
                          v_Tbl(j).Timestamp);
               COMMIT;
               EXIT WHEN Cur_Rwd%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Cur_Rwd%ISOPEN
          THEN
               CLOSE Cur_Rwd;
          END IF;
          COMMIT; -- this is 3 commits in a span of 7 lines of code.  Are we sure we understand what we're doing?  -- cnelson 12/17/2015
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Error   := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason  := 'Failed Procedure ' || v_Jobname || ': ';
                    v_Message := 'LOYALTY_NUMBER: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                .Loyalty_Number || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Jobname,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
          WHEN OTHERS THEN
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
                                   p_Logsource => 'ISSUEAECC_REWARDS',
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
     END Backup_Barcode_Assignment;

     PROCEDURE Assign_Barcodes IS
          v_Cnt NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          /*cursor2 that selects the barcodes and the reports */
          CURSOR Cur_Barcd IS
               SELECT T1.Channel,
                      T1.Customer_Nbr,
                      T1.Loyalty_Number,
                      T1.Email,
                      T1.Mobile_Number,
                      T1.Fname,
                      T1.Lname,
                      T1.Address1,
                      T1.Address2,
                      T1.City,
                      T1.State,
                      T1.Zip,
                      T1.Country,
                      T2.Auth_Cd,
                      T2.Suc_Cd,
                      T1.Campaign_Type,
                      T1.Campaign_Exp_Date,
                      T1.Eid,
                      T1.Campaign_Id,
                      T1.Language_Preference,
                      T1.Gender,
                      T1.Birthdate,
                      T1.Store_Loyalty,
                      T1.Tier_Status,
                      T1.Points_Balance,
                      T1.Points_Needed_For_Next_Reward,
                      T1.Number_Of_Bras_Purchased,
                      T1.Credits_To_Next_Free_Bra,
                      T1.Number_Of_Jeans_Purchased,
                      T1.Credits_To_Next_Free_Jean,
                      T1.Number_Of_Active_5_Off_Reward,
                      T1.Number_Of_Jeans_Reward,
                      T1.Number_Bra_Reward,
                      T1.Communication_Id,
                      T1.Comm_Plan_Id,
                      T1.Collateral_Id,
                      T1.Package_Id,
                      T1.Step_Id,
                      T1.Message_Id,
                      T1.Seg_Id,
                      T1.Seg_Nm,
                      T1.Aap_Flag,
                      T1.Card_Type,
                      T1.Run_Id,
                      T1.Lead_Key_Id,
                      T1.Site_Url,
                      T1.Enable_Passbook_Pass,
                      T1.Timestamp
               FROM   Stg_Aecc_Population T1, Stg_Aecc_Barcodes T2
               WHERE  1 = 1
                      AND T1.Rownumber = T2.Rownumber
                      AND 1 = 1;
          --TYPE t_tab IS TABLE OF cur_rwd%ROWTYPE;
          TYPE t_Tab2 IS TABLE OF Cur_Barcd%ROWTYPE;
          --v_tbl  t_tab;
          v_Tbl2 t_Tab2;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := Upper('assign_barcodes');
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
          /*Table backed up in prior step, now truncate for new run*/
          EXECUTE IMMEDIATE 'truncate table AE_AECC_20Reward';
          /*Write the new rewards report to table*/
          OPEN Cur_Barcd;
          LOOP
               FETCH Cur_Barcd BULK COLLECT
                    INTO v_Tbl2 LIMIT 1000;
               FORALL i IN 1 .. v_Tbl2.Count SAVE EXCEPTIONS
                    INSERT INTO Ae_Aecc_20reward
                         (Channel,
                          Customer_Nbr,
                          Loyalty_Number,
                          Email,
                          Mobile_Number,
                          Fname,
                          Lname,
                          Address1,
                          Address2,
                          City,
                          State,
                          Zip,
                          Country,
                          Auth_Cd,
                          Suc_Cd,
                          Campaign_Type,
                          Campaign_Exp_Date,
                          Eid,
                          Campaign_Id,
                          Language_Preference,
                          Gender,
                          Birthdate,
                          Store_Loyalty,
                          Tier_Status,
                          Points_Balance,
                          Points_Needed_For_Next_Reward,
                          Number_Of_Bras_Purchased,
                          Credits_To_Next_Free_Bra,
                          Number_Of_Jeans_Purchased,
                          Credits_To_Next_Free_Jean,
                          Number_Of_Active_5_Off_Reward,
                          Number_Of_Jeans_Reward,
                          Number_Bra_Reward,
                          Communication_Id,
                          Comm_Plan_Id,
                          Collateral_Id,
                          Package_Id,
                          Step_Id,
                          Message_Id,
                          Seg_Id,
                          Seg_Nm,
                          Aap_Flag,
                          Card_Type,
                          Run_Id,
                          Lead_Key_Id,
                          Site_Url,
                          Enable_Passbook_Pass,
                          TIMESTAMP)
                    VALUES
                         (v_Tbl2(i).Channel,
                          v_Tbl2(i).Customer_Nbr,
                          v_Tbl2(i).Loyalty_Number,
                          v_Tbl2(i).Email,
                          v_Tbl2(i).Mobile_Number,
                          v_Tbl2(i).Fname,
                          v_Tbl2(i).Lname,
                          v_Tbl2(i).Address1,
                          v_Tbl2(i).Address2,
                          v_Tbl2(i).City,
                          v_Tbl2(i).State,
                          v_Tbl2(i).Zip,
                          v_Tbl2(i).Country,
                          v_Tbl2(i).Auth_Cd,
                          v_Tbl2(i).Suc_Cd,
                          v_Tbl2(i).Campaign_Type,
                          v_Tbl2(i).Campaign_Exp_Date,
                          v_Tbl2(i).Eid,
                          v_Tbl2(i).Campaign_Id,
                          v_Tbl2(i).Language_Preference,
                          v_Tbl2(i).Gender,
                          v_Tbl2(i).Birthdate,
                          v_Tbl2(i).Store_Loyalty,
                          v_Tbl2(i).Tier_Status,
                          v_Tbl2(i).Points_Balance,
                          v_Tbl2(i).Points_Needed_For_Next_Reward,
                          v_Tbl2(i).Number_Of_Bras_Purchased,
                          v_Tbl2(i).Credits_To_Next_Free_Bra,
                          v_Tbl2(i).Number_Of_Jeans_Purchased,
                          v_Tbl2(i).Credits_To_Next_Free_Jean,
                          v_Tbl2(i).Number_Of_Active_5_Off_Reward,
                          v_Tbl2(i).Number_Of_Jeans_Reward,
                          v_Tbl2(i).Number_Bra_Reward,
                          v_Tbl2(i).Communication_Id,
                          v_Tbl2(i).Comm_Plan_Id,
                          v_Tbl2(i).Collateral_Id,
                          v_Tbl2(i).Package_Id,
                          v_Tbl2(i).Step_Id,
                          v_Tbl2(i).Message_Id,
                          v_Tbl2(i).Seg_Id,
                          v_Tbl2(i).Seg_Nm,
                          v_Tbl2(i).Aap_Flag,
                          v_Tbl2(i).Card_Type,
                          v_Tbl2(i).Run_Id,
                          v_Tbl2(i).Lead_Key_Id,
                          v_Tbl2(i).Site_Url,
                          v_Tbl2(i).Enable_Passbook_Pass,
                          v_Tbl2(i).Timestamp);
               COMMIT;
               EXIT WHEN Cur_Barcd%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Cur_Barcd%ISOPEN
          THEN
               CLOSE Cur_Barcd;
          END IF;
          COMMIT;
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Error   := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason  := 'Failed Procedure ' || v_Jobname || ': ';
                    v_Message := 'LOYALTY_NUMBER: ' || v_Tbl2(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                .Loyalty_Number || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Jobname,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
          WHEN OTHERS THEN
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
                                   p_Logsource => v_Jobname,
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
     END Assign_Barcodes;

     PROCEDURE Consume_Barcodes(p_Dummy IN VARCHAR2,
                                Retval  IN OUT Rcursor) IS
          v_Cnt          NUMBER := 0;
          v_Txnstartdt   DATE;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          v_Ctrwdredeem  NUMBER := 0;
          v_Reissueamt   NUMBER := 0;
          v_Bluetier     NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Cur_Updbarcd IS
               SELECT Rw.Auth_Cd,
                      Rw.Suc_Cd,
                      Rw.Loyalty_Number,
                      Vc.Ipcode AS Ipcode,
                      Rb.a_Rowkey
               FROM   Bp_Ae.Ae_Aecc_20reward Rw
               INNER  JOIN Bp_Ae.Lw_Virtualcard Vc
               ON     Vc.Loyaltyidnumber = Rw.Loyalty_Number
               INNER  JOIN Bp_Ae.Ats_Rewardbarcodes Rb
               ON     Rb.a_Barcode = Rw.Suc_Cd
                      AND Rb.a_Typecode = Rw.Auth_Cd;
          TYPE t_Tab IS TABLE OF Cur_Updbarcd%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'CONSUME_BARCODES';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'CONSUME_BARCODES',
                                        v_Processid);
          /*Logging*/
          /*consume the issued Barcodes*/
          OPEN Cur_Updbarcd;
          LOOP
               FETCH Cur_Updbarcd BULK COLLECT
                    INTO v_Tbl LIMIT 1000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    UPDATE Bp_Ae.Ats_Rewardbarcodes a
                    SET    a.a_Ipcode = v_Tbl(k).Ipcode,
                           a.a_Status = 1
                    WHERE  a.a_Rowkey = v_Tbl(k).a_Rowkey;
               COMMIT;
               v_Cnt              := v_Cnt + 1;
               v_Messagesreceived := v_Messagesreceived + 1;
               --  bad way to do an incremental commit, needs to be fixed. - cnelson 12/17/2015
               IF MOD(v_Cnt, 1000) = 0
               THEN
                    COMMIT;
               END IF;
               EXIT WHEN Cur_Updbarcd%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Cur_Updbarcd%ISOPEN
          THEN
               CLOSE Cur_Updbarcd;
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure CONSUME_BARCODES: ';
                    v_Message        := 'ipcode: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ipcode || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'CONSUME_BARCODES',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in CONSUME_BARCODES ');
          WHEN OTHERS THEN
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
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'CONSUME_BARCODES',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in CONSUME_BARCODES ');
     END Consume_Barcodes;

     PROCEDURE Identify_Population IS
          -- this procedure matierializes the base population
          -- written by cnelson 12/17/2015
          PRAGMA AUTONOMOUS_TRANSACTION;
     BEGIN
          DELETE FROM Stg_Aecc_Population;
          INSERT /*+ append */
          INTO Stg_Aecc_Population
SELECT Rownumber,
       Channel,
       Customer_Nbr,
       Loyalty_Number,
       Email,
       Mobile_Number,
       Fname,
       Lname,
       Address1,
       Address2,
       City,
       State,
       Zip,
       Country,
       Auth_Cd,
       Suc_Cd,
       Campaign_Type,
       Campaign_Exp_Date,
       eid,
       campaign_id,
       language_preference,
       gender,
       birthdate,
       store_loyalty,
       tier_status,
       points_balance,
       points_needed_for_next_reward,
       number_of_bras_purchased,
       credits_to_next_free_bra,
       number_of_jeans_purchased,
       credits_to_next_free_jean,
       number_of_active_5_off_reward,
       number_of_jeans_reward,
       number_bra_reward,
       communication_id,
       comm_plan_id,
       collateral_id,
       package_id,
       step_id,
       message_id,
       seg_id,
       seg_nm,
       aap_flag,
       card_type,
       run_id,
       lead_key_id,
       site_url,
       enable_passbook_pass,
       timestamp
            FROM (SELECT Rownum AS Rownumber,
                         CASE
                           WHEN (NVL(Md.a_Pendingcellverification, 1) = 0 AND
                                Md.a_Mobilephone IS NOT NULL) THEN
                            CAST(2 AS NVARCHAR2(100))
                           WHEN (NVL(Md.a_Pendingemailverification, 1) = 0 AND
                                Md.a_Emailaddress IS NOT NULL) THEN
                            CAST(1 AS NVARCHAR2(100))
                           WHEN (NVL(Md.a_Pendingemailverification, 1) = 1 AND
                                NVL(Md.a_Pendingcellverification, 1) = 1) THEN
                            CAST(3 AS NVARCHAR2(100))
                           ELSE
                            CAST(1 AS NVARCHAR2(100))
                         END AS Channel,
                         CAST(Vc.Linkkey AS NVARCHAR2(80)) AS Customer_Nbr,
                         CAST(Vc.Loyaltyidnumber AS NVARCHAR2(100)) AS Loyalty_Number,
                         CAST(Md.a_Emailaddress AS NVARCHAR2(150)) AS Email,
                         Regexp_Replace(CAST(Md.a_Mobilephone AS NVARCHAR2(100)),
                                        '[^[:digit:]]',
                                        NULL) AS Mobile_Number,
                         CAST(Lm.Firstname AS NVARCHAR2(100)) AS Fname,
                         CAST(Lm.Lastname AS NVARCHAR2(100)) AS Lname,
                         CAST(Md.a_Addresslineone AS NVARCHAR2(100)) AS Address1,
                         CAST(Md.a_Addresslinetwo AS NVARCHAR2(100)) AS Address2,
                         CAST(Md.a_City AS NVARCHAR2(100)) AS City,
                         CAST(Md.a_Stateorprovince AS NVARCHAR2(100)) AS State,
                         CAST(REPLACE(Md.a_Ziporpostalcode, '-') AS NVARCHAR2(100)) AS Zip,
                         CAST(Md.a_Country AS NVARCHAR2(100)) AS Country,
                         CAST('' AS NVARCHAR2(100)) AS Auth_Cd,
                         CAST('' AS NVARCHAR2(100)) AS Suc_Cd,
                         CAST('' AS NVARCHAR2(100)) AS Campaign_Type,
                         CAST('31-DEC-2016' AS NVARCHAR2(100)) AS Campaign_Exp_Date,
                         CAST('274211' AS NVARCHAR2(100)) AS Eid,
                         CAST('2204' AS NVARCHAR2(100)) AS Campaign_Id,
                         CASE
                           WHEN Md.a_Languagepreference IS NULL THEN
                            CAST(0 AS NVARCHAR2(100))
                           ELSE
                            CAST(Md.a_Languagepreference AS NVARCHAR2(100))
                         END AS Language_Preference,
                         CAST(To_Char(Nvl(Md.a_Gender, '0')) AS NVARCHAR2(100)) AS Gender,
                         CAST(To_Char(Lm.Birthdate, 'DD-MON-YYYY') AS NVARCHAR2(100)) AS Birthdate,
                         CAST(To_Char(Md.a_Homestoreid) AS NVARCHAR2(80)) AS Store_Loyalty,
                         (SELECT CASE
                                   WHEN t.Tiername = 'Blue' THEN
                                    CAST(1 AS NVARCHAR2(100))
                                   WHEN t.Tiername = 'Silver' THEN
                                    CAST(2 AS NVARCHAR2(100))
                                   WHEN t.Tiername = 'Gold' THEN
                                    CAST(3 AS NVARCHAR2(100))
                                   WHEN t.Tiername = 'Select' THEN
                                    CAST(4 AS NVARCHAR2(100))
                                 END
                            FROM Bp_Ae.Lw_Membertiers Mt
                           INNER JOIN Bp_Ae.Lw_Tiers t
                              ON Mt.Tierid = t.Tierid
                           WHERE 1 = 1
                             AND Mt.Memberid = Vc.Ipcode
                             AND SYSDATE BETWEEN Mt.Fromdate AND Mt.Todate
                             AND Rownum = 1
                             AND 1 = 1) AS Tier_Status,
                         CAST(p.a_Totalpoints AS NVARCHAR2(80)) AS Points_Balance,
                         CAST(p.a_Pointstonextreward AS NVARCHAR2(80)) AS Points_Needed_For_Next_Reward,
                         CAST(p.a_Bracurrentpurchased AS NVARCHAR2(80)) AS Number_Of_Bras_Purchased,
                         CAST(5 - p.a_Bracurrentpurchased AS NVARCHAR2(80)) AS Credits_To_Next_Free_Bra,
                         CAST(p.a_Jeansrollingbalance AS NVARCHAR2(80)) AS Number_Of_Jeans_Purchased,
                         CAST(5 - p.a_Jeansrollingbalance AS NVARCHAR2(80)) AS Credits_To_Next_Free_Jean,
                         Rew.Number_Of_Active_5_Off_Reward,
                         Rew.Number_Of_Jeans_Reward,
                         Rew.Number_Of_Bra_Reward AS Number_Bra_Reward,
                         CAST('' AS NVARCHAR2(100)) AS Communication_Id,
                         CAST('' AS NVARCHAR2(100)) AS Comm_Plan_Id,
                         CAST('' AS NVARCHAR2(100)) AS Collateral_Id,
                         CAST('' AS NVARCHAR2(100)) AS Package_Id,
                         CAST('' AS NVARCHAR2(100)) AS Step_Id,
                         CAST('' AS NVARCHAR2(100)) AS Message_Id,
                         CAST('' AS NVARCHAR2(100)) AS Seg_Id,
                         CAST('' AS NVARCHAR2(100)) AS Seg_Nm,
                         CAST('' AS NVARCHAR2(100)) AS Aap_Flag,
                         CAST(Md.a_Cardtype AS NVARCHAR2(80)) AS Card_Type,
                         CAST('' AS NVARCHAR2(100)) AS Run_Id,
                         CAST('' AS NVARCHAR2(100)) AS Lead_Key_Id,
                         CAST('' AS NVARCHAR2(100)) AS Site_Url,
                         CAST('' AS NVARCHAR2(100)) AS Enable_Passbook_Pass,
                         CAST('' AS NVARCHAR2(100)) AS TIMESTAMP
                         /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */,
                         ROW_NUMBER() OVER(PARTITION BY ak1.a_accountkey ORDER BY lm.lastactivitydate desc) as rn,
                         ROW_NUMBER() OVER(PARTITION BY vc.ipcode ORDER BY lm.lastactivitydate desc) as ipcodeRN
                  /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */
                    FROM Bp_Ae.Lw_Loyaltymember Lm
                   INNER JOIN Bp_Ae.Lw_Virtualcard Vc
                      ON Lm.Ipcode = Vc.Ipcode
                     AND Vc.Isprimary = 1
                   INNER JOIN Bp_Ae.Ats_Memberdetails Md
                      ON Lm.Ipcode = Md.a_Ipcode
                  /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */
                   INNER JOIN (select distinct (ak.a_accountkey), ak.a_vckey
                                from bp_ae.ats_synchronyaccountkey ak
                               where ak.a_accountkey is not null) ak1
                      on ak1.a_vckey = vc.vckey
                  /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */
                    LEFT JOIN Bp_Ae.Ats_Memberpointbalances p
                      ON Vc.Ipcode = p.a_Ipcode
                    LEFT JOIN (SELECT Mr.Memberid,
                                     SUM(CASE
                                           WHEN r.Name = 'AEO Rewards $5 Reward' THEN
                                            1
                                           ELSE
                                            0
                                         END) AS Number_Of_Active_5_Off_Reward,
                                     SUM(CASE
                                           WHEN r.Name = 'B5G1 Jean Reward' THEN
                                            1
                                           ELSE
                                            0
                                         END) AS Number_Of_Jeans_Reward,
                                     SUM(CASE
                                           WHEN r.Name = 'B5G1 Bra Reward' THEN
                                            1
                                           ELSE
                                            0
                                         END) AS Number_Of_Bra_Reward
                                FROM Bp_Ae.Lw_Memberrewards Mr, Bp_Ae.Lw_Rewardsdef r
                               WHERE 1 = 1
                                 AND Mr.Rewarddefid = r.Id
                                 AND r.Name IN ('AEO Rewards $5 Reward',
                                                'B5G1 Jean Reward',
                                                'B5G1 Bra Reward')
                                 AND Mr.Fulfillmentdate IS NULL
                                 AND Mr.Expiration > SYSDATE
                                 AND 1 = 1
                               GROUP BY Mr.Memberid) Rew
                      ON Rew.Memberid = Lm.Ipcode
                   WHERE 1 = 1
                        --  AND Bp_Ae.Ae_Isinpilot(Md.a_Extendedplaycode) = 1
                     AND md.a_extendedplaycode IN (1, 3) -- this accomplishes the same thing as the function (though not dynamic)
                     AND (Md.a_Cardtype = 1 OR Md.a_Cardtype = 2 OR Md.a_Cardtype = 3) -- why not use IN here?
                     AND ((NVL(Md.a_Pendingcellverification,1) = 0 AND
                         Md.a_Mobilephone IS NOT NULL) OR
                         (NVL(Md.a_Pendingemailverification,1) = 0 AND
                         Md.a_Emailaddress IS NOT NULL) OR
                         (NVL(Md.a_Pendingemailverification,1) = 1 AND
                         NVL(Md.a_Pendingcellverification,1) = 1))
                     AND NVL(MD.A_EMPLOYEECODE, 0) <> 1
                     AND LM.MEMBERSTATUS = 1
                     AND 1 = 1) TBL
            WHERE  1=1
           AND TBL.rn = 1
           AND TBL.IPCODERN = 1;

          COMMIT;
     END Identify_Population;

     PROCEDURE Select_Barcodes(p_Typecode IN Bp_Ae.Ats_Rewardbarcodes.a_Typecode%TYPE) IS
          PRAGMA AUTONOMOUS_TRANSACTION;
          Lv_Rowcount      NUMBER;
          Lv_Barcode_Count NUMBER;
     BEGIN
          DELETE FROM Stg_Aecc_Barcodes;
          SELECT COUNT(*) INTO Lv_Rowcount FROM Stg_Aecc_Population;
          INSERT INTO Stg_Aecc_Barcodes
               (Suc_Cd, Auth_Cd, Rownumber)
               SELECT Rb.a_Barcode  AS Suc_Cd,
                      Rb.a_Typecode AS Auth_Cd,
                      Rownum        AS Rownumber
               FROM   Bp_Ae.Ats_Rewardbarcodes Rb
               WHERE  1 = 1
                      AND Rb.a_Typecode = TRIM(p_Typecode)
                      AND Rb.a_Ipcode < 0
                      AND Rb.a_Status = 0
                      AND Rownum <= Lv_Rowcount
                      AND 1 = 1;
          Lv_Barcode_Count := SQL%ROWCOUNT;
          IF Lv_Barcode_Count < Lv_Rowcount
          THEN
               Raise_Application_Error(-20000,
                                       'Barcode selection did not return enough rows to attach to AECC population.');
          END IF;
          COMMIT;
     END Select_Barcodes;


-- AEO-676 Begin

      PROCEDURE Issue_Rewards(p_DateIssued IN Bp_Ae.Lw_Memberrewards.Dateissued%TYPE) IS

          v_Cnt NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_prid        NUMBER := -1;
          v_rwdefid     NUMBER := -1;

          /*cursor1  for backing up the previous run data*/
          CURSOR Cur_Rwd IS
               SELECT rw.loyalty_number, vc.ipcode, rw.auth_cd, rw.suc_cd
               FROM Bp_Ae.Ae_Aecc_20reward rw
               INNER JOIN bp_ae.lw_virtualcard vc ON rw.loyalty_number = vc.loyaltyidnumber ;
          TYPE t_Tab IS TABLE OF Cur_Rwd%ROWTYPE;
          --    TYPE t_tab2 IS TABLE OF cur_barcd%ROWTYPE;
          v_Tbl t_Tab;
          --    v_tbl2  t_tab2;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := Upper('issue_reward');
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

          SELECT count(*)
          INTO v_rwdefid
          FROM lw_rewardsdef rd
          WHERE rd.name = '20% Reward';

          SELECT COUNT(*)
          INTO v_prid
          FROM lw_product pr
          WHERE pr.name = '20% Reward';

          IF ( v_rwdefid <= 0 ) THEN
             Raise_Application_Error(-20002,
                                       'No reward definition found for lw_rewarddef.name="20% Reward"');
          END IF;

           IF ( v_prid <= 0 ) THEN
             Raise_Application_Error(-20002,
                                       'No product definition found for lw_product.name="20% Reward"');
          END IF;


          SELECT rd.id
          INTO v_rwdefid
          FROM lw_rewardsdef rd
          WHERE rd.name = '20% Reward';

          SELECT pr.id
          INTO v_prid
          FROM lw_product pr
          WHERE pr.name = '20% Reward';

          OPEN Cur_Rwd;
          LOOP
               FETCH Cur_Rwd BULK COLLECT
                    INTO v_Tbl LIMIT 1000;
               /*Backup*/
               FORALL j IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO lw_memberrewards
                         ( ID,
                         REWARDDEFID,
                           CERTIFICATENMBR,
                           OFFERCODE,
                           AVAILABLEBALANCE,
                           FULFILlMENTOPTION,
                           MEMBERID,
                           PRODUCTID,
                           productvariantid,
                           dateissued,
                           expiration,
                           lwordernumber,
                           createdate)
                    VALUES ( hibernate_sequence.nextval,
                            v_rwdefid,
                            v_Tbl(j).suc_cd,
                            v_Tbl(j).auth_cd,
                            0,
                            NULL,
                            v_Tbl(j).ipcode,
                            v_prid,
                            -1,
                            p_DateIssued,
                            to_date('12/31/2016','MM/DD/YYYY'),
                            0,
                            SYSDATE );

               COMMIT;
               EXIT WHEN Cur_Rwd%NOTFOUND;

          END LOOP;
          COMMIT;
          IF Cur_Rwd%ISOPEN    THEN
               CLOSE Cur_Rwd;
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Error   := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason  := 'Failed Procedure ' || v_Jobname || ': ';
                    v_Message := 'LOYALTY_NUMBER: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                .Loyalty_Number || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Jobname,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
          WHEN OTHERS THEN
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
                                   p_Logsource => 'ISSUEAECC_REWARDS',
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
 END Issue_Rewards;
 -- AEO-676 End
 --AEO-750 begin ------------------------------SCJ

 PROCEDURE Issueaecc_ReminderRewards(p_Typecode VARCHAR2,
                                 Retval     IN OUT Rcursor) IS

     BEGIN
          Backup_reminderBarcode_Assign;
          Identify_RemindersPopulation;
          Select_RemindersBarcodes(p_Typecode => p_Typecode);
          Assign_Barcodes_Reminder;
          Issue_ReminderRewards(SYSDATE);
     END Issueaecc_ReminderRewards;
  PROCEDURE Identify_RemindersPopulation IS
          -- this procedure matierializes the base population
          -- written by cnelson 12/17/2015
          PRAGMA AUTONOMOUS_TRANSACTION;
     BEGIN
          DELETE FROM Stg_Aecc_reminderPopulation;
          INSERT /*+ append */
          INTO Stg_Aecc_reminderPopulation
SELECT rownum AS Rownumber,
       Channel,
       Customer_Nbr,
       Loyalty_Number,
       Email,
       Mobile_Number,
       Fname,
       Lname,
       Address1,
       Address2,
       City,
       State,
       Zip,
       Country,
       Auth_Cd,
       Suc_Cd,
       Campaign_Type,
       Campaign_Exp_Date,
       eid,
       campaign_id,
       language_preference,
       gender,
       birthdate,
       store_loyalty,
       tier_status,
       points_balance,
       points_needed_for_next_reward,
       number_of_bras_purchased,
       credits_to_next_free_bra,
       number_of_jeans_purchased,
       credits_to_next_free_jean,
       number_of_active_5_off_reward,
       number_of_jeans_reward,
       number_bra_reward,
       communication_id,
       comm_plan_id,
       collateral_id,
       package_id,
       step_id,
       message_id,
       seg_id,
       seg_nm,
       aap_flag,
       card_type,
       run_id,
       lead_key_id,
       site_url,
       enable_passbook_pass,
       timestamp
            FROM (SELECT --Rownum AS Rownumber,
                         CASE
                         WHEN (NVL(length(md.a_emailaddress),0) > 1) THEN                    --AEO-938 AH
                            CAST(1 AS NVARCHAR2(100))
                           WHEN (NVL(Md.a_Pendingcellverification, 1) = 0 AND
                                Md.a_Mobilephone IS NOT NULL) THEN
                            CAST(2 AS NVARCHAR2(100))
                           WHEN (NVL(length(md.a_emailaddress),0) < 1 AND
                                NVL(Md.a_Pendingcellverification, 1) = 1) THEN
                            CAST(3 AS NVARCHAR2(100))
                           ELSE
                            CAST(1 AS NVARCHAR2(100))
                         END AS Channel,
                         CAST(Vc.Linkkey AS NVARCHAR2(80)) AS Customer_Nbr,
                         CAST(Vc.Loyaltyidnumber AS NVARCHAR2(100)) AS Loyalty_Number,
                         CAST(Md.a_Emailaddress AS NVARCHAR2(150)) AS Email,
                         Regexp_Replace(CAST(Md.a_Mobilephone AS NVARCHAR2(100)),
                                        '[^[:digit:]]',
                                        NULL) AS Mobile_Number,
                         CAST(Lm.Firstname AS NVARCHAR2(100)) AS Fname,
                         CAST(Lm.Lastname AS NVARCHAR2(100)) AS Lname,
                         CAST(Md.a_Addresslineone AS NVARCHAR2(100)) AS Address1,
                         CAST(Md.a_Addresslinetwo AS NVARCHAR2(100)) AS Address2,
                         CAST(Md.a_City AS NVARCHAR2(100)) AS City,
                         CAST(Md.a_Stateorprovince AS NVARCHAR2(100)) AS State,
                         CAST(REPLACE(Md.a_Ziporpostalcode, '-') AS NVARCHAR2(100)) AS Zip,
                         CAST(Md.a_Country AS NVARCHAR2(100)) AS Country,
                         CAST('' AS NVARCHAR2(100)) AS Auth_Cd,
                         CAST('' AS NVARCHAR2(100)) AS Suc_Cd,
                         CAST('' AS NVARCHAR2(100)) AS Campaign_Type,
                         CAST( concat('31-DEC-', extract(YEAR FROM SYSDATE)) AS NVARCHAR2(100)) AS Campaign_Exp_Date,   --AEO 750
                         CAST('274211' AS NVARCHAR2(100)) AS Eid,
                         CAST('2204' AS NVARCHAR2(100)) AS Campaign_Id,
                         CASE
                           WHEN Md.a_Languagepreference IS NULL THEN
                            CAST(0 AS NVARCHAR2(100))
                           ELSE
                            CAST(Md.a_Languagepreference AS NVARCHAR2(100))
                         END AS Language_Preference,
                         CAST(To_Char(Nvl(Md.a_Gender, '0')) AS NVARCHAR2(100)) AS Gender,
                         CAST(To_Char(Lm.Birthdate, 'DD-MON-YYYY') AS NVARCHAR2(100)) AS Birthdate,
                         CAST(To_Char(Md.a_Homestoreid) AS NVARCHAR2(80)) AS Store_Loyalty,
                         (SELECT CASE
                                   WHEN t.Tiername = 'Blue' THEN
                                    CAST(1 AS NVARCHAR2(100))
                                   WHEN t.Tiername = 'Silver' THEN
                                    CAST(2 AS NVARCHAR2(100))
                                   WHEN t.Tiername = 'Gold' THEN
                                    CAST(3 AS NVARCHAR2(100))
                                   WHEN t.Tiername = 'Select' THEN
                                    CAST(4 AS NVARCHAR2(100))
                                 END
                            FROM Bp_Ae.Lw_Membertiers Mt
                           INNER JOIN Bp_Ae.Lw_Tiers t
                              ON Mt.Tierid = t.Tierid
                           WHERE 1 = 1
                             AND Mt.Memberid = Vc.Ipcode
                             AND SYSDATE BETWEEN Mt.Fromdate AND Mt.Todate
                             AND Rownum = 1
                             AND 1 = 1) AS Tier_Status,
                         CAST(p.a_Totalpoints AS NVARCHAR2(80)) AS Points_Balance,
                         CAST(p.a_Pointstonextreward AS NVARCHAR2(80)) AS Points_Needed_For_Next_Reward,
                         CAST(p.a_Bracurrentpurchased AS NVARCHAR2(80)) AS Number_Of_Bras_Purchased,
                         CAST(5 - p.a_Bracurrentpurchased AS NVARCHAR2(80)) AS Credits_To_Next_Free_Bra,
                         CAST(p.a_Jeansrollingbalance AS NVARCHAR2(80)) AS Number_Of_Jeans_Purchased,
                         CAST(5 - p.a_Jeansrollingbalance AS NVARCHAR2(80)) AS Credits_To_Next_Free_Jean,
                         Rew.Number_Of_Active_5_Off_Reward,
                         Rew.Number_Of_Jeans_Reward,
                         Rew.Number_Of_Bra_Reward AS Number_Bra_Reward,
                         CAST('' AS NVARCHAR2(100)) AS Communication_Id,
                         CAST('' AS NVARCHAR2(100)) AS Comm_Plan_Id,
                         CAST('' AS NVARCHAR2(100)) AS Collateral_Id,
                         CAST('' AS NVARCHAR2(100)) AS Package_Id,
                         CAST('' AS NVARCHAR2(100)) AS Step_Id,
                         CAST('' AS NVARCHAR2(100)) AS Message_Id,
                         CAST('' AS NVARCHAR2(100)) AS Seg_Id,
                         CAST('' AS NVARCHAR2(100)) AS Seg_Nm,
                         CAST('' AS NVARCHAR2(100)) AS Aap_Flag,
                         CAST(Md.a_Cardtype AS NVARCHAR2(80)) AS Card_Type,
                         CAST('' AS NVARCHAR2(100)) AS Run_Id,
                         CAST('' AS NVARCHAR2(100)) AS Lead_Key_Id,
                         CAST('' AS NVARCHAR2(100)) AS Site_Url,
                         CAST('' AS NVARCHAR2(100)) AS Enable_Passbook_Pass,
                         CAST('' AS NVARCHAR2(100)) AS TIMESTAMP
                         /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */,
                         ROW_NUMBER() OVER(PARTITION BY ak1.a_accountkey ORDER BY lm.lastactivitydate desc) as rn,
                         ROW_NUMBER() OVER(PARTITION BY vc.ipcode ORDER BY lm.lastactivitydate desc) as ipcodeRN
                  /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */
                    FROM Bp_Ae.Lw_Loyaltymember Lm
                   INNER JOIN Bp_Ae.Lw_Virtualcard Vc
                      ON Lm.Ipcode = Vc.Ipcode
                     AND Vc.Isprimary = 1
                   INNER JOIN Bp_Ae.Ats_Memberdetails Md
                      ON Lm.Ipcode = Md.a_Ipcode
                  /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */
                   INNER JOIN (select distinct (ak.a_accountkey), ak.a_vckey
                                from bp_ae.ats_synchronyaccountkey ak
                               where ak.a_accountkey is not null) ak1
                      on ak1.a_vckey = vc.vckey
                  /*aeo-614 ADDING SYNCHRONY ACCOUNT KEY LOGIC----------------scj */
                    LEFT JOIN Bp_Ae.Ats_Memberpointbalances p
                      ON Vc.Ipcode = p.a_Ipcode
                    LEFT JOIN (SELECT Mr.Memberid,
                                     SUM(CASE
                                           WHEN r.Name = 'AEO Rewards $10 Reward' THEN
                                            1
                                           ELSE
                                            0
                                         END) AS Number_Of_Active_5_Off_Reward,
                                     SUM(CASE
                                           WHEN r.Name = 'B5G1 Jean Reward' THEN
                                            1
                                           ELSE
                                            0
                                         END) AS Number_Of_Jeans_Reward,
                                     SUM(CASE
                                           WHEN r.Name = 'B5G1 Bra Reward' THEN
                                            1
                                           ELSE
                                            0
                                         END) AS Number_Of_Bra_Reward
                                FROM Bp_Ae.Lw_Memberrewards Mr, Bp_Ae.Lw_Rewardsdef r
                               WHERE 1 = 1
                                 AND Mr.Rewarddefid = r.Id
                                 AND r.Name IN ('AEO Rewards $10 Reward',
                                                'B5G1 Jean Reward',
                                                'B5G1 Bra Reward')
                                 AND Mr.Fulfillmentdate IS NULL
                                 AND Mr.Expiration > SYSDATE
                                 AND 1 = 1
                               GROUP BY Mr.Memberid) Rew
                      ON Rew.Memberid = Lm.Ipcode
                   WHERE 1 = 1
                        --  AND Bp_Ae.Ae_Isinpilot(Md.a_Extendedplaycode) = 1
                     AND md.a_extendedplaycode IN (1, 3) -- this accomplishes the same thing as the function (though not dynamic)
                     AND (Md.a_Cardtype = 1 OR Md.a_Cardtype = 2 OR Md.a_Cardtype = 3) -- why not use IN here?
                     and md.a_ipcode not in (Select vc.ipcode from bp_ae.Ae_Aecc_20reward RW
                                              inner join bp_Ae.Lw_Virtualcard vc on vc.loyaltyidnumber = RW.loyalty_number
                                              UNION ALL
                                              SELECT VC1.IPCODE FROM BP_AE.Ae_Aecc_20rewardreminder_Bkp REM
                                              inner join bp_Ae.Lw_Virtualcard vc1 on vc1.loyaltyidnumber = REM.loyalty_number
                                             )
                     --START AEO 750 fix-----------------------------------------------------------------------
                     AND Md.a_languagepreference IN ('0','1','2')             -- end AECC20STG_REMCHK_LANGPREF
                     AND nvl(Md.a_gender,'0') IN ('0','1','2')                         -- end AECC20STG_REMCHK_GENDER
                     AND ((NVL(Md.a_Pendingcellverification,1) = 0
                             AND Md.a_Mobilephone IS NOT NULL)                         -- channel = 2 - SM
                             OR (NVL(length(md.a_emailaddress),0) > 1)                        -- channel = 1 - EM /* AEO-938 AH */
                             OR (NVL(length(md.a_emailaddress),0) < 1
                                 AND NVL(Md.a_Pendingcellverification,1) = 1              -- channel = 3 - DM ( channel in (1,2,3) )
                                 AND Md.a_Addresslineone IS NOT NULL
                                 AND Md.a_city IS NOT NULL
                                 AND Md.a_stateorprovince IS NOT NULL
                                 AND Md.a_ziporpostalcode IS NOT NULL))
                     --END AEO 750 fix-------------------------------------------------------------------------   
                     AND NVL(MD.A_EMPLOYEECODE, 0) <> 1
                     AND LM.MEMBERSTATUS = 1
                     AND 1 = 1) TBL
            WHERE  1=1
           AND TBL.rn = 1
           AND TBL.IPCODERN = 1;

          COMMIT;
END Identify_RemindersPopulation;

PROCEDURE Select_RemindersBarcodes(p_Typecode IN Bp_Ae.Ats_Rewardbarcodes.a_Typecode%TYPE) IS
          PRAGMA AUTONOMOUS_TRANSACTION;
          Lv_Rowcount      NUMBER;
          Lv_Barcode_Count NUMBER;
     BEGIN
          DELETE FROM Stg_Aecc_Reminder_Barcodes;
          SELECT COUNT(*) INTO Lv_Rowcount FROM Stg_Aecc_reminderPopulation;
          INSERT INTO Stg_Aecc_Reminder_Barcodes
               (Suc_Cd, Auth_Cd, Rownumber)
               SELECT Rb.a_Barcode  AS Suc_Cd,
                      Rb.a_Typecode AS Auth_Cd,
                      Rownum        AS Rownumber
               FROM   Bp_Ae.Ats_Rewardbarcodes Rb
               WHERE  1 = 1
                      AND Rb.a_Typecode = TRIM(p_Typecode)
                      AND Rb.a_Ipcode < 0
                      AND Rb.a_Status = 0
                      AND Rownum <= Lv_Rowcount
                      AND 1 = 1;
          Lv_Barcode_Count := SQL%ROWCOUNT;
          IF Lv_Barcode_Count < Lv_Rowcount
          THEN
               Raise_Application_Error(-20000,
                                       'Barcode selection did not return enough rows to attach to AECC population.');
          END IF;
          COMMIT;
END Select_RemindersBarcodes;

PROCEDURE Backup_ReminderBarcode_Assign IS
          v_Cnt NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          /*cursor1  for backing up the previous run data*/
          CURSOR Cur_Rwd IS
               SELECT * FROM Bp_Ae.Ae_Aecc_20rewardreminder;
          TYPE t_Tab IS TABLE OF Cur_Rwd%ROWTYPE;
          --    TYPE t_tab2 IS TABLE OF cur_barcd%ROWTYPE;
          v_Tbl t_Tab;
          --    v_tbl2  t_tab2;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := Upper('backup_Reminderbarcode_assign');
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
          OPEN Cur_Rwd;
          LOOP
               FETCH Cur_Rwd BULK COLLECT
                    INTO v_Tbl LIMIT 1000;
               /*Backup*/
               FORALL j IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Ae_Aecc_20rewardReminder_Bkp
                         (Channel,
                          Customer_Nbr,
                          Loyalty_Number,
                          Email,
                          Mobile_Number,
                          Fname,
                          Lname,
                          Address1,
                          Address2,
                          City,
                          State,
                          Zip,
                          Country,
                          Auth_Cd,
                          Suc_Cd,
                          Campaign_Type,
                          Campaign_Exp_Date,
                          Eid,
                          Campaign_Id,
                          Language_Preference,
                          Gender,
                          Birthdate,
                          Store_Loyalty,
                          Tier_Status,
                          Points_Balance,
                          Points_Needed_For_Next_Reward,
                          Number_Of_Bras_Purchased,
                          Credits_To_Next_Free_Bra,
                          Number_Of_Jeans_Purchased,
                          Credits_To_Next_Free_Jean,
                          Number_Of_Active_5_Off_Reward,
                          Number_Of_Jeans_Reward,
                          Number_Bra_Reward,
                          Communication_Id,
                          Comm_Plan_Id,
                          Collateral_Id,
                          Package_Id,
                          Step_Id,
                          Message_Id,
                          Seg_Id,
                          Seg_Nm,
                          Aap_Flag,
                          Card_Type,
                          Run_Id,
                          Lead_Key_Id,
                          Site_Url,
                          Enable_Passbook_Pass,
                          TIMESTAMP)
                    VALUES
                         (v_Tbl(j).Channel,
                          v_Tbl(j).Customer_Nbr,
                          v_Tbl(j).Loyalty_Number,
                          v_Tbl(j).Email,
                          v_Tbl(j).Mobile_Number,
                          v_Tbl(j).Fname,
                          v_Tbl(j).Lname,
                          v_Tbl(j).Address1,
                          v_Tbl(j).Address2,
                          v_Tbl(j).City,
                          v_Tbl(j).State,
                          v_Tbl(j).Zip,
                          v_Tbl(j).Country,
                          v_Tbl(j).Auth_Cd,
                          v_Tbl(j).Suc_Cd,
                          v_Tbl(j).Campaign_Type,
                          v_Tbl(j).Campaign_Exp_Date,
                          v_Tbl(j).Eid,
                          v_Tbl(j).Campaign_Id,
                          v_Tbl(j).Language_Preference,
                          v_Tbl(j).Gender,
                          v_Tbl(j).Birthdate,
                          v_Tbl(j).Store_Loyalty,
                          v_Tbl(j).Tier_Status,
                          v_Tbl(j).Points_Balance,
                          v_Tbl(j).Points_Needed_For_Next_Reward,
                          v_Tbl(j).Number_Of_Bras_Purchased,
                          v_Tbl(j).Credits_To_Next_Free_Bra,
                          v_Tbl(j).Number_Of_Jeans_Purchased,
                          v_Tbl(j).Credits_To_Next_Free_Jean,
                          v_Tbl(j).Number_Of_Active_5_Off_Reward,
                          v_Tbl(j).Number_Of_Jeans_Reward,
                          v_Tbl(j).Number_Bra_Reward,
                          v_Tbl(j).Communication_Id,
                          v_Tbl(j).Comm_Plan_Id,
                          v_Tbl(j).Collateral_Id,
                          v_Tbl(j).Package_Id,
                          v_Tbl(j).Step_Id,
                          v_Tbl(j).Message_Id,
                          v_Tbl(j).Seg_Id,
                          v_Tbl(j).Seg_Nm,
                          v_Tbl(j).Aap_Flag,
                          v_Tbl(j).Card_Type,
                          v_Tbl(j).Run_Id,
                          v_Tbl(j).Lead_Key_Id,
                          v_Tbl(j).Site_Url,
                          v_Tbl(j).Enable_Passbook_Pass,
                          v_Tbl(j).Timestamp);
               COMMIT;
               EXIT WHEN Cur_Rwd%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Cur_Rwd%ISOPEN
          THEN
               CLOSE Cur_Rwd;
          END IF;
          COMMIT; -- this is 3 commits in a span of 7 lines of code.  Are we sure we understand what we're doing?  -- cnelson 12/17/2015
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Error   := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason  := 'Failed Procedure ' || v_Jobname || ': ';
                    v_Message := 'LOYALTY_NUMBER: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                .Loyalty_Number || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Jobname,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
          WHEN OTHERS THEN
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
                                   p_Logsource => 'ISSUEAECC_REWARDS',
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
END Backup_ReminderBarcode_Assign;

PROCEDURE Assign_Barcodes_Reminder IS
          v_Cnt NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          /*cursor2 that selects the barcodes and the reports */
          CURSOR Cur_Barcd IS
               SELECT T1.Channel,
                      T1.Customer_Nbr,
                      T1.Loyalty_Number,
                      T1.Email,
                      T1.Mobile_Number,
                      T1.Fname,
                      T1.Lname,
                      T1.Address1,
                      T1.Address2,
                      T1.City,
                      T1.State,
                      T1.Zip,
                      T1.Country,
                      T2.Auth_Cd,
                      T2.Suc_Cd,
                      T1.Campaign_Type,
                      T1.Campaign_Exp_Date,
                      T1.Eid,
                      T1.Campaign_Id,
                      T1.Language_Preference,
                      T1.Gender,
                      T1.Birthdate,
                      T1.Store_Loyalty,
                      T1.Tier_Status,
                      T1.Points_Balance,
                      T1.Points_Needed_For_Next_Reward,
                      T1.Number_Of_Bras_Purchased,
                      T1.Credits_To_Next_Free_Bra,
                      T1.Number_Of_Jeans_Purchased,
                      T1.Credits_To_Next_Free_Jean,
                      T1.Number_Of_Active_5_Off_Reward,
                      T1.Number_Of_Jeans_Reward,
                      T1.Number_Bra_Reward,
                      T1.Communication_Id,
                      T1.Comm_Plan_Id,
                      T1.Collateral_Id,
                      T1.Package_Id,
                      T1.Step_Id,
                      T1.Message_Id,
                      T1.Seg_Id,
                      T1.Seg_Nm,
                      T1.Aap_Flag,
                      T1.Card_Type,
                      T1.Run_Id,
                      T1.Lead_Key_Id,
                      T1.Site_Url,
                      T1.Enable_Passbook_Pass,
                      T1.Timestamp
               FROM   Stg_Aecc_ReminderPopulation T1, Stg_Aecc_Reminder_Barcodes T2
               WHERE  1 = 1
                      AND T1.Rownumber = T2.Rownumber
                      AND 1 = 1;
          --TYPE t_tab IS TABLE OF cur_rwd%ROWTYPE;
          TYPE t_Tab2 IS TABLE OF Cur_Barcd%ROWTYPE;
          --v_tbl  t_tab;
          v_Tbl2 t_Tab2;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := Upper('assign_barcodesreminder');
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
          /*Table backed up in prior step, now truncate for new run*/
          EXECUTE IMMEDIATE 'truncate table AE_AECC_20RewardReminder';
          /*Write the new rewards report to table*/
          OPEN Cur_Barcd;
          LOOP
               FETCH Cur_Barcd BULK COLLECT
                    INTO v_Tbl2 LIMIT 1000;
               FORALL i IN 1 .. v_Tbl2.Count SAVE EXCEPTIONS
                    INSERT INTO AE_AECC_20RewardReminder
                         (Channel,
                          Customer_Nbr,
                          Loyalty_Number,
                          Email,
                          Mobile_Number,
                          Fname,
                          Lname,
                          Address1,
                          Address2,
                          City,
                          State,
                          Zip,
                          Country,
                          Auth_Cd,
                          Suc_Cd,
                          Campaign_Type,
                          Campaign_Exp_Date,
                          Eid,
                          Campaign_Id,
                          Language_Preference,
                          Gender,
                          Birthdate,
                          Store_Loyalty,
                          Tier_Status,
                          Points_Balance,
                          Points_Needed_For_Next_Reward,
                          Number_Of_Bras_Purchased,
                          Credits_To_Next_Free_Bra,
                          Number_Of_Jeans_Purchased,
                          Credits_To_Next_Free_Jean,
                          Number_Of_Active_5_Off_Reward,
                          Number_Of_Jeans_Reward,
                          Number_Bra_Reward,
                          Communication_Id,
                          Comm_Plan_Id,
                          Collateral_Id,
                          Package_Id,
                          Step_Id,
                          Message_Id,
                          Seg_Id,
                          Seg_Nm,
                          Aap_Flag,
                          Card_Type,
                          Run_Id,
                          Lead_Key_Id,
                          Site_Url,
                          Enable_Passbook_Pass,
                          TIMESTAMP)
                    VALUES
                         (v_Tbl2(i).Channel,
                          v_Tbl2(i).Customer_Nbr,
                          v_Tbl2(i).Loyalty_Number,
                          v_Tbl2(i).Email,
                          v_Tbl2(i).Mobile_Number,
                          v_Tbl2(i).Fname,
                          v_Tbl2(i).Lname,
                          v_Tbl2(i).Address1,
                          v_Tbl2(i).Address2,
                          v_Tbl2(i).City,
                          v_Tbl2(i).State,
                          v_Tbl2(i).Zip,
                          v_Tbl2(i).Country,
                          v_Tbl2(i).Auth_Cd,
                          v_Tbl2(i).Suc_Cd,
                          v_Tbl2(i).Campaign_Type,
                          v_Tbl2(i).Campaign_Exp_Date,
                          v_Tbl2(i).Eid,
                          v_Tbl2(i).Campaign_Id,
                          v_Tbl2(i).Language_Preference,
                          v_Tbl2(i).Gender,
                          v_Tbl2(i).Birthdate,
                          v_Tbl2(i).Store_Loyalty,
                          v_Tbl2(i).Tier_Status,
                          v_Tbl2(i).Points_Balance,
                          v_Tbl2(i).Points_Needed_For_Next_Reward,
                          v_Tbl2(i).Number_Of_Bras_Purchased,
                          v_Tbl2(i).Credits_To_Next_Free_Bra,
                          v_Tbl2(i).Number_Of_Jeans_Purchased,
                          v_Tbl2(i).Credits_To_Next_Free_Jean,
                          v_Tbl2(i).Number_Of_Active_5_Off_Reward,
                          v_Tbl2(i).Number_Of_Jeans_Reward,
                          v_Tbl2(i).Number_Bra_Reward,
                          v_Tbl2(i).Communication_Id,
                          v_Tbl2(i).Comm_Plan_Id,
                          v_Tbl2(i).Collateral_Id,
                          v_Tbl2(i).Package_Id,
                          v_Tbl2(i).Step_Id,
                          v_Tbl2(i).Message_Id,
                          v_Tbl2(i).Seg_Id,
                          v_Tbl2(i).Seg_Nm,
                          v_Tbl2(i).Aap_Flag,
                          v_Tbl2(i).Card_Type,
                          v_Tbl2(i).Run_Id,
                          v_Tbl2(i).Lead_Key_Id,
                          v_Tbl2(i).Site_Url,
                          v_Tbl2(i).Enable_Passbook_Pass,
                          v_Tbl2(i).Timestamp);
               COMMIT;
               EXIT WHEN Cur_Barcd%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Cur_Barcd%ISOPEN
          THEN
               CLOSE Cur_Barcd;
          END IF;
          COMMIT;
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Error   := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason  := 'Failed Procedure ' || v_Jobname || ': ';
                    v_Message := 'LOYALTY_NUMBER: ' || v_Tbl2(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                .Loyalty_Number || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Jobname,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
          WHEN OTHERS THEN
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
                                   p_Logsource => v_Jobname,
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
END Assign_Barcodes_Reminder;

PROCEDURE Issue_ReminderRewards(p_DateIssued IN Bp_Ae.Lw_Memberrewards.Dateissued%TYPE) IS

          v_Cnt NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_prid        NUMBER := -1;
          v_rwdefid     NUMBER := -1;

          /*cursor1  for backing up the previous run data*/
          CURSOR Cur_Rwd IS
               SELECT rw.loyalty_number, vc.ipcode, rw.auth_cd, rw.suc_cd
               FROM Bp_Ae.AE_AECC_20RewardReminder rw
               INNER JOIN bp_ae.lw_virtualcard vc ON rw.loyalty_number = vc.loyaltyidnumber ;
          TYPE t_Tab IS TABLE OF Cur_Rwd%ROWTYPE;
          --    TYPE t_tab2 IS TABLE OF cur_barcd%ROWTYPE;
          v_Tbl t_Tab;
          --    v_tbl2  t_tab2;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := Upper('issue_reminderreward');
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

          SELECT count(*)
          INTO v_rwdefid
          FROM lw_rewardsdef rd
          WHERE rd.name = '20% Reward';

          SELECT COUNT(*)
          INTO v_prid
          FROM lw_product pr
          WHERE pr.name = '20% Reward';

          IF ( v_rwdefid <= 0 ) THEN
             Raise_Application_Error(-20002,
                                       'No reward definition found for lw_rewarddef.name="20% Reward"');
          END IF;

           IF ( v_prid <= 0 ) THEN
             Raise_Application_Error(-20002,
                                       'No product definition found for lw_product.name="20% Reward"');
          END IF;


          SELECT rd.id
          INTO v_rwdefid
          FROM lw_rewardsdef rd
          WHERE rd.name = '20% Reward';

          SELECT pr.id
          INTO v_prid
          FROM lw_product pr
          WHERE pr.name = '20% Reward';

          OPEN Cur_Rwd;
          LOOP
               FETCH Cur_Rwd BULK COLLECT
                    INTO v_Tbl LIMIT 1000;
               /*Backup*/
               FORALL j IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO lw_memberrewards
                         ( ID,
                         REWARDDEFID,
                           CERTIFICATENMBR,
                           OFFERCODE,
                           AVAILABLEBALANCE,
                           FULFILlMENTOPTION,
                           MEMBERID,
                           PRODUCTID,
                           productvariantid,
                           dateissued,
                           expiration,
                           lwordernumber,
                           createdate)
                    VALUES ( hibernate_sequence.nextval,
                            v_rwdefid,
                            v_Tbl(j).suc_cd,
                            v_Tbl(j).auth_cd,
                            0,
                            NULL,
                            v_Tbl(j).ipcode,
                            v_prid,
                            -1,
                            p_DateIssued,
                            to_date( concat('12/31/', extract(YEAR FROM SYSDATE)), 'mm/dd/yyyy' ),          /* AEO-938 AH */
                            0,
                            SYSDATE );

               COMMIT;
               EXIT WHEN Cur_Rwd%NOTFOUND;

          END LOOP;
          COMMIT;
          IF Cur_Rwd%ISOPEN    THEN
               CLOSE Cur_Rwd;
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Error   := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason  := 'Failed Procedure ' || v_Jobname || ': ';
                    v_Message := 'LOYALTY_NUMBER: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                .Loyalty_Number || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Jobname,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ' ||
                                       v_Jobname || ' ');
          WHEN OTHERS THEN
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
                                   p_Logsource => 'ISSUEAECC_ReminderREWARDS',
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
END Issue_ReminderRewards;

PROCEDURE Consume_Barcodes_Reminder(p_Dummy IN VARCHAR2,
                                Retval  IN OUT Rcursor) IS
          v_Cnt          NUMBER := 0;
          v_Txnstartdt   DATE;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          v_Ctrwdredeem  NUMBER := 0;
          v_Reissueamt   NUMBER := 0;
          v_Bluetier     NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Cur_Updbarcd IS
               SELECT Rw.Auth_Cd,
                      Rw.Suc_Cd,
                      Rw.Loyalty_Number,
                      Vc.Ipcode AS Ipcode,
                      Rb.a_Rowkey
               FROM   Bp_Ae.AE_AECC_20RewardReminder Rw
               INNER  JOIN Bp_Ae.Lw_Virtualcard Vc
               ON     Vc.Loyaltyidnumber = Rw.Loyalty_Number
               INNER  JOIN Bp_Ae.Ats_Rewardbarcodes Rb
               ON     Rb.a_Barcode = Rw.Suc_Cd
                      AND Rb.a_Typecode = Rw.Auth_Cd;
          TYPE t_Tab IS TABLE OF Cur_Updbarcd%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'CONSUME_BARCODES_Reminder';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'CONSUME_BARCODES_Reminder',
                                        v_Processid);
          /*Logging*/
          /*consume the issued Barcodes*/
          OPEN Cur_Updbarcd;
          LOOP
               FETCH Cur_Updbarcd BULK COLLECT
                    INTO v_Tbl LIMIT 1000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    UPDATE Bp_Ae.Ats_Rewardbarcodes a
                    SET    a.a_Ipcode = v_Tbl(k).Ipcode,
                           a.a_Status = 1
                    WHERE  a.a_Rowkey = v_Tbl(k).a_Rowkey;
               COMMIT;
               v_Cnt              := v_Cnt + 1;
               v_Messagesreceived := v_Messagesreceived + 1;
               --  bad way to do an incremental commit, needs to be fixed. - cnelson 12/17/2015
               IF MOD(v_Cnt, 1000) = 0
               THEN
                    COMMIT;
               END IF;
               EXIT WHEN Cur_Updbarcd%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Cur_Updbarcd%ISOPEN
          THEN
               CLOSE Cur_Updbarcd;
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
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure CONSUME_BARCODES_Reminder: ';
                    v_Message        := 'ipcode: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ipcode || ' ';
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'CONSUME_BARCODES_Reminder',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               Raise_Application_Error(-20002,
                                       'Other Exception detected in CONSUME_BARCODES_Reminder ');
          WHEN OTHERS THEN
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
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'CONSUME_BARCODES_Reminder',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in CONSUME_BARCODES_Reminder ');
     END Consume_Barcodes_Reminder;
--AEO-750 end-----------------------SCJ
END Ae_Rewards_Aeccmembers;
/
