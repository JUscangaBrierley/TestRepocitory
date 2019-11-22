create or replace package AE_FIX_IPCODE is

  -- Author  : MMORALES
  -- Created : 8/2/2016 2:42:55 PM
  -- Purpose : Populate the ipcode field for history tables
  
  

  -- Public function and procedure declarations
  PROCEDURE upd_f_txn_detail(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2);
  PROCEDURE upd_f_txn_reward(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2);
  PROCEDURE upd_f_txn_tender(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2);
  PROCEDURE upd_f_txn_discount(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2);
  PROCEDURE fix_late_enrollment(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2); -- AEO-832
  

end AE_FIX_IPCODE;
/
create or replace package body AE_FIX_IPCODE IS

-- AEO-832 begin

PROCEDURE fix_late_enrollment(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2) IS
  
          v_Logsource        VARCHAR2(256) := 'AE_FIX_IPCODE.fix_late_enrollment';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AE_FIX_IPCODE.fix_late_enrollment';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;        
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'AE_FIX_IPCODE.fix_late_enrollment';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_RowKey                VARCHAR2(256);
          v_trycount           NUMBER :=0;
          v_recordcount        NUMBER :=0;
          v_24hr_Txn_Cnt          NUMBER:=0;
          v_Txnqualpurchaseamt    NUMBER:=0;
          v_Errormessage          VARCHAR2(256);
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
      
         l_prev_date DATE := to_date(p_prev_date,'MM/DD/YYYY');
         l_curr_date DATE := to_date(p_curr_date,'MM/DD/YYYY');         
         
         
       CURSOR get_data IS
        select hd.a_rowkey as HistoryRowKey
       , vc.ipcode, vc.vckey, hd.a_dtlquantity, hd.a_dtldiscountamount, hd.a_dtlclearanceitem, hd.a_dtldatemodified, hd.a_reconcilestatus, hd.a_txnheaderid, hd.a_txndetailid, hd.a_brandid
       , hd.a_fileid, hd.a_processid, hd.a_filelineitem, hd.a_cardid, hd.a_creditcardid, hd.a_txnloyaltyid, hd.a_txnmaskid, hd.a_txnnumber, hd.a_txndate, hd.a_txndatemodified
       , hd.a_txnregisternumber, hd.a_txnstoreid, hd.a_txntype, hd.a_txnamount, hd.a_txndiscountamount, hd.a_txnqualpurchaseamt, hd.a_txnemailaddress, hd.a_txnphonenumber, hd.a_txnemployeeid
       , hd.a_txnchannelid, hd.a_txnoriginaltxnrowkey, hd.a_txncreditsused, hd.a_dtlitemlinenbr, hd.a_dtlproductid, hd.a_dtltypeid, hd.a_dtlactionid, hd.a_dtlretailamount, hd.a_dtlsaleamount
       , hd.a_dtlclasscode, hd.a_errormessage, hd.a_shipdate, hd.a_ordernumber, hd.a_skunumber, hd.a_tenderamount, hd.a_storenumber, hd.statuscode, hd.createdate, hd.updatedate
       , hd.lastdmlid, 0 as nonmember, 0 as txnoriginalstoreid , '' as txnoriginaltxndate, '' as txnoriginaltxnnumber
            from   ats_historytxndetail hd
            inner join lw_virtualcard vc on hd.a_txnloyaltyid = vc.loyaltyidnumber
            inner join lw_loyaltymember lm on vc.ipcode = lm.ipcode
            where      ( hd.a_dtlclasscode = '9911' ) 
                   and hd.a_txnDate BETWEEN l_prev_date AND l_curr_date;

      TYPE t_tab IS TABLE OF get_data%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object                  
       
     lv_email_body       CLOB;
     
     CURSOR get_data2 IS
        select DISTINCT ipcode, txnheaderid, txndate, txnqualpurchaseamt
            from   lw_txndetail_stage hd;

      TYPE t_tab2 IS TABLE OF get_data2%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl2 t_tab2; ---<------ our arry object
      
BEGIN
  
      v_My_Log_Id := Utility_Pkg.Get_Libjobid();
              
        
      OPEN get_data;
      LOOP
        FETCH get_data BULK COLLECT   INTO v_tbl LIMIT 100; 
        FOR i IN 1 .. v_tbl.count 
          LOOP
          v_RowKey := v_tbl(i).HistoryRowKey;

          insert into lw_txndetail_stage (
          rowkey, ipcode, vckey, dtlquantity, dtldiscountamount, dtlclearanceitem, dtldatemodified
         , reconcilestatus, txnheaderid, txndetailid, brandid, fileid, processid, filelineitem
         , cardid, creditcardid, txnloyaltyid, txnmaskid, txnnumber, txndate, txndatemodified
         , txnregisternumber, txnstoreid, txntypeid, txnamount, txndiscountamount, txnqualpurchaseamt
         , txnemailaddress, txnphonenumber, txnemployeeid, txnchannelid, txnoriginaltxnrowkey, txncreditsused, dtlitemlinenbr, dtlproductid, dtltypeid, dtlactionid
         , dtlretailamount, dtlsaleamount, dtlclasscode, errormessage, shipdate, ordernumber, skunumber, tenderamount, storenumber, statuscode, createdate, updatedate, lastdmlid, nonmember
         , txnoriginalstoreid, txnoriginaltxndate, txnoriginaltxnnumber)
         Values
         (
         v_tbl(i).HistoryRowKey, v_tbl(i).ipcode, v_tbl(i).vckey, v_tbl(i).a_dtlquantity, v_tbl(i).a_dtldiscountamount, v_tbl(i).a_dtlclearanceitem, v_tbl(i).a_dtldatemodified
         , v_tbl(i).a_reconcilestatus, v_tbl(i).a_txnheaderid, v_tbl(i).a_txndetailid, v_tbl(i).a_brandid, v_tbl(i).a_fileid, 0, v_tbl(i).a_filelineitem
         , v_tbl(i).a_cardid, v_tbl(i).a_creditcardid, v_tbl(i).a_txnloyaltyid, v_tbl(i).a_txnmaskid, v_tbl(i).a_txnnumber, v_tbl(i).a_txndate, v_tbl(i).a_txndatemodified
         , v_tbl(i).a_txnregisternumber, v_tbl(i).a_txnstoreid, v_tbl(i).a_txntype, v_tbl(i).a_txnamount, v_tbl(i).a_txndiscountamount, v_tbl(i).a_txnqualpurchaseamt
         , v_tbl(i).a_txnemailaddress, v_tbl(i).a_txnphonenumber, v_tbl(i).a_txnemployeeid, v_tbl(i).a_txnchannelid, v_tbl(i).a_txnoriginaltxnrowkey, v_tbl(i).a_txncreditsused
         , v_tbl(i).a_dtlitemlinenbr, v_tbl(i).a_dtlproductid, v_tbl(i).a_dtltypeid, v_tbl(i).a_dtlactionid, v_tbl(i).a_dtlretailamount, v_tbl(i).a_dtlsaleamount
         , v_tbl(i).a_dtlclasscode, v_Errormessage, v_tbl(i).a_shipdate, v_tbl(i).a_ordernumber, v_tbl(i).a_skunumber, v_tbl(i).a_tenderamount, v_tbl(i).a_storenumber
         , v_tbl(i).statuscode, v_tbl(i).createdate, v_tbl(i).updatedate, v_tbl(i).lastdmlid, v_tbl(i).nonmember, v_tbl(i).txnoriginalstoreid, v_tbl(i).txnoriginaltxndate
         , v_tbl(i).txnoriginaltxnnumber
         )
          LOG ERRORS INTO err$_lw_txndetail_stage('UPDATE') REJECT LIMIT UNLIMITED;

         Select count(*) into v_trycount From err$_lw_txndetail_stage where rowkey = v_RowKey;

        v_messagesreceived := v_messagesreceived + 1;
        v_recordcount := v_recordcount + 1;

         If(v_trycount > 0) THEN
            v_messagesfailed := v_messagesfailed + 1;
          END IF;

    -- Now go back and update the processId as Late Enrollment Processed
         Update ats_historytxndetail hst
         Set hst.a_processid = 9,
             hst.a_ipcode = v_tbl(i).ipcode, -- AEO-794
             hst.a_vckey = v_tbl(i).vckey    -- AEO-794
         Where hst.a_rowkey = v_tbl(i).HistoryRowKey;
          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        END LOOP;

        EXIT WHEN get_data%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_data%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data;
      END IF;
    

----------------------------------------------------
--Check TxnDetail records for > 3 Txns in 24 hour period
----------------------------------------------------
      OPEN get_data2;
      LOOP
        FETCH get_data2 BULK COLLECT
          INTO v_tbl2 LIMIT 100; --<-----  here we say collect 1,000 rows at a time.
        FOR i IN 1 .. v_tbl2.count --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
        LOOP
          v_Errormessage       := '';
          v_Txnqualpurchaseamt := v_tbl2(i).txnqualpurchaseamt;
          /* zero out txn amount if more then 3 txn in 24hrs */
          v_24hr_Txn_Cnt := utility_pkg.Get_24hr_Txn_Cnt(
                                              p_ipcode => v_tbl2(i).ipcode,
                                              p_Date  => v_tbl2(i).txndate);
          IF v_24hr_Txn_Cnt > 3
          THEN
            v_Txnqualpurchaseamt := 0;
            v_Errormessage       := '>3 txns found on txndate';
          END IF;

          update lw_txndetail_stage
          Set txnqualpurchaseamt = v_Txnqualpurchaseamt,
              errormessage = v_Errormessage
          Where txnheaderid = v_tbl2(i).txnheaderid;

          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        END LOOP;

        EXIT WHEN get_data2%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;

      IF get_data2%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_data2;
      END IF;

           
    COMMIT;   

    v_Endtime := SYSDATE;
         
    Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
                               
        
    
    EXCEPTION 
       WHEN OTHERS THEN   
            v_Endtime := SYSDATE;
            v_Messagesfailed := v_Messagesfailed + 1;
            lv_email_body := 'The process failed with - '|| SQLCODE||'-'||SQLERRM||CHR(10)
                                 ||'Date range :'|| l_prev_date||' - '||l_curr_date;
              
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname); 
               
               
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure fix_late_enrollment';
               v_Message        := '<failed>' || Chr(10) || '<details>' ||
                                   Chr(10) || '<pkg>AE_FIX_IPCODE</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>fix_late_enrollment</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '<Text>' || Chr(10)|| lv_email_body  || Chr(10) ||'</Text>'||'</failed>';
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
                  
         RAISE;    --reraise

END fix_late_enrollment;
  


-- AEO-832 end

PROCEDURE upd_f_txn_discount(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2) IS
          v_Logsource        VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_discount';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AE_FIX_IPCODE.upd_f_txn_discount';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;        
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_discount';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));
      
         l_prev_date DATE := to_date(p_prev_date,'MM/DD/YYYY');
         l_curr_date DATE := to_date(p_curr_date,'MM/DD/YYYY');         
         
         CURSOR curHDIS IS
          SELECT hds.a_rowkey,
               hds.a_txndate,
               hds.a_txnheaderid, 
               hds.a_txndetailid,
               hds.a_txndiscountid,
               vc.ipcode,
               vc.vckey
          FROM bp_ae.ats_historytxndetaildiscount hds
          INNER JOIN bp_ae.ats_txnlineitemdiscount tt ON tt.a_Txnheaderid   = hds.a_txnheaderid AND
                                                         tt.a_Txndetailid   = hds.a_txndetailid AND
                                                         tt.a_txndiscountid = hds.a_Txndiscountid
          INNER JOIN bp_Ae.Ats_Txnheader th  ON th.a_txnheaderid = tt.a_txnheaderid
          INNER JOIN bp_ae.lw_virtualcard vc ON vc.vckey = th.a_vckey
          WHERE hds.a_ipcode = 0 AND
                hds.a_txndate BETWEEN l_prev_date AND l_curr_date AND
                tt.a_txndate  BETWEEN l_prev_date AND l_curr_date;          



         TYPE curHDIS_type is TABLE OF curHDIS%ROWTYPE INDEX BY PLS_INTEGER;
         recHDIS curHDIS_type; 
                  
       
         lv_email_body       CLOB;
      
BEGIN
  
         v_My_Log_Id := Utility_Pkg.Get_Libjobid();
              
         OPEN curHDIS;
         LOOP
              FETCH curHDIS BULK COLLECT INTO recHDIS LIMIT 10000;
                
                 -- Update in bulk
              FORALL i IN 1 .. recHDIS.COUNT
                        UPDATE BP_AE.Ats_Historytxndetaildiscount hdi
                           SET  hdi.a_ipcode = recHDIS(i).ipcode,
                                hdi.a_vckey = recHDIS(i).vckey   -- Code review 04Aug2016
                        WHERE hdi.a_rowkey = recHDIS(i).a_rowkey;
                              
              FORALL i IN 1 .. recHDIS.COUNT
                        insert into  X$_aeo787 (id,origin)
                         values ( recHDIS(i).a_rowkey,UPPER('Ats_Historytxndetaildiscount'));
                 
                
              v_Messagesreceived :=  v_Messagesreceived + recHDIS.COUNT;      
              COMMIT WRITE BATCH NOWAIT;
              EXIT WHEN curHDIS%NOTFOUND;       

         END LOOP; 
         CLOSE curHDIS;
        -----------               
         COMMIT;   

                      
        -- lv_email_body := 'Completed Date range :'|| p_prev_date||' - '||p_curr_date;

         v_Endtime := SYSDATE;
         
         Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
                               
        
    
        EXCEPTION 
          WHEN OTHERS THEN   
            v_Endtime := SYSDATE;
            v_Messagesfailed := v_Messagesfailed + 1;
            lv_email_body := 'The process failed with - '|| SQLCODE||'-'||SQLERRM||CHR(10)
                                 ||'Date range :'|| l_prev_date||' - '||l_curr_date;
              
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname); 
               
               
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure upd_f_txn_discount';
               v_Message        := '<failed>' || Chr(10) || '<details>' ||
                                   Chr(10) || '<pkg>AE_FIX_IPCODE</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>upd_f_txn_discount</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '<Text>' || Chr(10)|| lv_email_body  || Chr(10) ||'</Text>'||'</failed>';
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
                  
         RAISE;    --reraise

END upd_f_txn_discount;
  
PROCEDURE upd_f_txn_tender ( p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2) IS

          v_Logsource        VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_tender';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AE_FIX_IPCODE.upd_f_txn_tender';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;        
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_tender';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));


        
         l_prev_date DATE := to_date(p_prev_date,'MM/DD/YYYY');
         l_curr_date DATE := to_date(p_curr_date,'MM/DD/YYYY');         
         
		

		 
         CURSOR curHTT IS
          SELECT htt.a_rowkey
				 ,vc.ipcode
				 ,vc.vckey
				 ,tt.a_ipcode AS vc_key -- LW stores vc_key in ipcode column in ats_txndetail and ats_txntender
			 FROM bp_ae.ats_historytxntender htt
			INNER JOIN bp_ae.ats_txntender tt
			   ON (tt.a_txntenderid = htt.a_txntenderid)
			INNER JOIN bp_ae.lw_virtualcard vc
			   ON (vc.vckey = tt.a_ipcode)
			WHERE 1 = 1
			  AND NVL(htt.a_vckey,0) != tt.a_ipcode        
			  AND htt.a_txndate BETWEEN p_prev_date AND p_curr_date --'01-Jan-2016' AND '01-Feb-2016'
			  AND tt.a_txndate BETWEEN p_prev_date AND p_curr_date --'01-Jan-2016' AND '01-Feb-2016'
			  AND 1 = 1;
     /*      SELECT htt.a_rowkey, 
                 htt.a_txndate, 
                 htt.a_txnheaderid,               
                 htt.a_txntenderid,
                 vc.ipcode,
                 vc.vckey                      
          FROM bp_ae.ats_historytxntender htt
          INNER JOIN bp_ae.ats_txntender tt ON tt.a_txnheaderid = htt.a_txnheaderid AND
                                               tt.a_txntenderid = htt.a_txntenderid
          INNER JOIN bp_Ae.Ats_Txnheader th ON th.a_txnheaderid = tt.a_txnheaderid                   
          INNER JOIN bp_ae.lw_virtualcard vc ON vc.vckey = th.a_vckey
          WHERE htt.a_ipcode = 0 AND
                htt.a_txndate BETWEEN l_prev_date AND l_curr_date AND
                tt.a_txndate BETWEEN  l_prev_date AND l_curr_date; -- Code review 04Aug2016*/
 
             
         TYPE curHTT_type is TABLE OF curHTT%ROWTYPE INDEX BY PLS_INTEGER;
         recHTT curHTT_type; 
         
       
        lv_email_body       CLOB; 
    
        
BEGIN
  
         v_My_Log_Id := Utility_Pkg.Get_Libjobid();    
         OPEN curHTT;
         LOOP
              FETCH curHTT BULK COLLECT INTO recHTT LIMIT 10000;
                
                 -- Update in bulk
              FORALL i IN 1 .. recHTT.COUNT
                        UPDATE BP_AE.Ats_Historytxntender htt
                          SET   htt.a_ipcode = recHTT(i).ipcode,
                                htt.a_vckey  = recHTT(i).vckey   -- Code review 04Aug2016
                        WHERE htt.a_rowkey = recHTT(i).a_rowkey;
                 
              FORALL i IN 1 .. recHTT.COUNT
                        insert into  X$_aeo787 (id,origin)
                         values ( recHTT(i).a_rowkey,UPPER('Ats_Historytxntender'));
				
              v_Messagesreceived :=  v_Messagesreceived + recHTT.COUNT; 
              COMMIT WRITE BATCH NOWAIT;
              EXIT WHEN curHTT%NOTFOUND;       

         END LOOP; 
         CLOSE curHTT;
         COMMIT;
         
         v_Endtime := SYSDATE;
         
         Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

       
    
        EXCEPTION 
          WHEN OTHERS THEN               
             v_Endtime := SYSDATE;
            v_Messagesfailed := v_Messagesfailed + 1;
            lv_email_body := 'The process failed with - '|| SQLCODE||'-'||SQLERRM||CHR(10)
                                 ||'Date range :'|| l_prev_date||' - '||l_curr_date;
              
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname); 
               
               
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure upd_f_txn_tender';
               v_Message        := '<failed>' || Chr(10) || '<details>' ||
                                   Chr(10) || '<pkg>AE_FIX_IPCODE</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>upd_f_txn_tendert</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '<Text>' || Chr(10)|| lv_email_body  || Chr(10) ||'</Text>'||'</failed>';
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
                  
         RAISE;    --reraise

END upd_f_txn_tender;



PROCEDURE upd_f_txn_reward(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2) IS 
          v_Logsource        VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_reward';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AE_FIX_IPCODE.upd_f_txn_reward';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;        
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_reward';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(256);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));


        

         l_prev_date DATE := to_date(p_prev_date,'MM/DD/YYYY');
         l_curr_date DATE := to_date(p_curr_date,'MM/DD/YYYY');         
         
          -- Bulk cursor
          
     CURSOR curHRED IS
        SELECT hrw.a_rowkey ,
               hrw.a_txndate,
               hrw.a_txnheaderid, 
               hrw.a_txndetailid,
               hrw.a_txnrewardredeemid ,
               vc.vckey,
               vc.ipcode
        FROM  bp_ae.ats_historytxnrewardredeem hrw
        INNER JOIN bp_ae.ats_txnrewardredeem tt ON  tt.a_txnheaderid = hrw.a_txnheaderid AND
                                                    tt.a_rowkey      = hrw.a_txnrewardredeemid
        INNER JOIN bp_Ae.Ats_Txnheader th  ON th.a_txnheaderid = tt.a_txnheaderid
        INNER JOIN bp_ae.lw_virtualcard vc ON vc.vckey = th.a_vckey
        WHERE hrw.a_ipcode = 0 AND
              hrw.a_txndate BETWEEN l_prev_date AND l_curr_date AND
              tt.a_txndate  BETWEEN l_prev_date AND l_curr_date;
              
         
         TYPE curHRED_type is TABLE OF curHRED%ROWTYPE INDEX BY PLS_INTEGER;
         recHRED curHRED_type; 
         
      
         lv_email_body       CLOB;
    
        
      
BEGIN
         v_My_Log_Id := Utility_Pkg.Get_Libjobid();    
                
         OPEN curHRED;
         LOOP
              FETCH curHRED BULK COLLECT INTO recHRED LIMIT 10000;
                
                 -- Update in bulk
              FORALL i IN 1 .. recHRED.COUNT
                        UPDATE BP_AE.Ats_Historytxnrewardredeem hrd
                            SET  hrd.a_ipcode = recHRED(i).ipcode,
                                 hrd.a_vckey  = recHRED(i).vckey   -- Code review 04Aug2016
                        WHERE (recHRED(i).a_rowkey =hrd.a_rowkey) ;
                 
              
              FORALL i IN 1 .. recHRED.COUNT
                        insert into  X$_aeo787 (id,origin)
                         values ( recHRED(i).a_rowkey,UPPER('Ats_Historytxnrewardredeem'));
             
           
              v_Messagesreceived :=  v_Messagesreceived + recHRED.COUNT;         
              COMMIT WRITE BATCH NOWAIT;
              EXIT WHEN curHRED%NOTFOUND;       

         END LOOP; 
         CLOSE curHRED;
         COMMIT;   
         
       

         v_Endtime := SYSDATE;
         
         Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

       
        EXCEPTION 
          WHEN OTHERS THEN               
            v_Endtime := SYSDATE;
            v_Messagesfailed := v_Messagesfailed + 1;
            lv_email_body := 'The process failed with - '|| SQLCODE||'-'||SQLERRM||CHR(10)
                                 ||'Date range :'|| l_prev_date||' - '||l_curr_date;
              
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname); 
               
               
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure upd_f_txn_reward';
               v_Message        := '<failed>' || Chr(10) || '<details>' ||
                                   Chr(10) || '<pkg>AE_FIX_IPCODE</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>upd_f_txn_reward</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '<Text>' || Chr(10)|| lv_email_body  || Chr(10) ||'</Text>'||'</failed>';
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
                  
         RAISE;    --reraise


END upd_f_txn_reward;  
 
PROCEDURE upd_f_txn_detail(p_prev_date IN VARCHAR2, p_curr_date IN VARCHAR2) IS
  
   v_Logsource        VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_detail';
          v_My_Log_Id        NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Filename         VARCHAR2(512) := 'AE_FIX_IPCODE.upd_f_txn_detail';
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;        
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256) := 'AE_FIX_IPCODE.upd_f_txn_detail';
          v_Batchid          VARCHAR2(256) := 0;
          v_Message          VARCHAR2(4000);
          v_Reason           VARCHAR2(256);
          v_Error            VARCHAR2(256);
          v_Messageid        NUMBER;
          v_Envkey           VARCHAR2(256) := 'BP_AE@' ||
                                              Upper(Sys_Context('userenv',
                                                                'instance_name'));

  
     
      lv_email_body       CLOB;
      
         l_prev_date DATE := to_date(p_prev_date,'MM/DD/YYYY');
         l_curr_date DATE := to_date(p_curr_date,'MM/DD/YYYY');         
         
    
    
    -- CURSOR TO get THE LIST of ROWS whose ipcode field must be UPDATED ON EACH history TABLE     
	

	
      CURSOR curHTD IS
            SELECT htt.a_rowkey

               ,vc.ipcode
               ,vc.vckey
               ,td.a_ipcode          AS vc_key  -- LW stores vc_key in ipcode column in ats_txndetail and ats_txntender
           FROM bp_ae.ats_historytxndetail htt
          INNER JOIN bp_ae.ats_txndetailitem td
             ON (td.a_txndetailid = htt.a_txndetailid)
          INNER JOIN bp_ae.lw_virtualcard vc
             ON (vc.vckey = td.a_ipcode)
          WHERE 1 = 1
            AND NVL(htt.a_vckey,0) != td.a_ipcode 
            AND htt.a_txndate BETWEEN l_prev_date AND l_curr_date -- sample dates '01-Jan-2016' AND '01-Feb-2016'
            AND td.a_txndate  BETWEEN l_prev_date AND l_curr_date --sample dates '01-Jan-2016' AND '01-Feb-2016'
         AND 1 = 1;  
  /*      SELECT htd.a_rowkey, 
               htd.a_txndate, 
               htd.a_txnheaderid, 
               htd.a_txndetailid ,
               vc.ipcode,
               vc.vckey    
        FROM bp_ae.ats_historytxndetail htd
        INNER JOIN bp_ae.ats_txndetailitem td ON td.a_Txnheaderid = htd.a_txnheaderid AND
                                                 td.a_Txndetailid = htd.a_Txndetailid
        INNER JOIN bp_Ae.Ats_Txnheader th ON th.a_txnheaderid = td.a_txnheaderid
        INNER JOIN bp_ae.lw_virtualcard vc ON vc.vckey = th.a_vckey
         WHERE htd.a_ipcode = 0 AND
               htd.a_txndate BETWEEN l_prev_date AND l_curr_date AND
               td.a_txndate  BETWEEN l_prev_date AND l_curr_date; -- Code review 04Aug2016*/
              
          -- Bulk cursor
         TYPE curHTD_type is TABLE OF curHTD%ROWTYPE INDEX BY PLS_INTEGER;
         recHTD curHTD_type; 
  
BEGIN
  
         v_My_Log_Id := Utility_Pkg.Get_Libjobid(); 
         OPEN curHTD;
         LOOP
              FETCH curHTD BULK COLLECT INTO recHTD LIMIT 10000;
                
                 -- Update in bulk
              FORALL i IN 1 .. recHTD.COUNT
                        UPDATE BP_AE.Ats_Historytxndetail htd
                          SET  htd.a_ipcode = recHTD(i).ipcode,
                               htd.a_vckey = recHTD(i).vckey   -- Code review 04Aug2016
                        WHERE htd.a_rowkey = recHTD(i).a_rowkey;
                      
              FORALL i IN 1 .. recHTD.COUNT
                        insert into  X$_aeo787 (id,origin)
                         values ( recHTD(i).a_rowkey,UPPER('Ats_Historytxndetail'));
				   
               v_Messagesreceived :=  v_Messagesreceived + recHTD.COUNT; 
              COMMIT WRITE BATCH NOWAIT;
              EXIT WHEN curHTD%NOTFOUND;       

         END LOOP; 
         CLOSE curHTD;
         COMMIT;
         
         
       

         v_Endtime := SYSDATE;
         
         Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => v_Filename,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);

       
    
        EXCEPTION 
          WHEN OTHERS THEN               
         v_Endtime := SYSDATE;
            v_Messagesfailed := v_Messagesfailed + 1;
            lv_email_body := 'The process failed with - '|| SQLCODE||'-'||SQLERRM||CHR(10)
                                 ||'Date range :'|| l_prev_date||' - '||l_curr_date;
              
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => v_Filename,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname); 
               
               
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure upd_f_txn_detail';
               v_Message        := '<failed>' || Chr(10) || '<details>' ||
                                   Chr(10) || '<pkg>AE_FIX_IPCODE</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>upd_f_txn_reward</proc>' ||
                                   Chr(10) || '    <filename>' || v_Filename ||
                                   '</filename>' || Chr(10) || '  </details>' ||
                                   Chr(10) || '<Text>' || Chr(10)|| lv_email_body  || Chr(10) ||'</Text>'||'</failed>';
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
                  
         RAISE;    --reraise

END upd_f_txn_detail;
   
 
 
end AE_FIX_IPCODE;
/
