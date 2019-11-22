create or replace procedure Update_txndetailprocessid
as
      CURSOR get_MDdata IS
        select th.a_txnheaderid from bp_ae.ats_txnheader th where th.a_txnheaderid in
         (select txnheaderid from bp_ae.lw_txndetail_stage where ipcode > 0 and ProcessId = 0 and NonMember = 0);
      TYPE t_tab IS TABLE OF get_MDdata%ROWTYPE; ---<------ creating array definition of cursor rowtype, we'll collect data in chunks
      v_tbl t_tab; ---<------ our arry object
    BEGIN
      OPEN get_MDdata;
      LOOP
        FETCH get_MDdata BULK COLLECT
          INTO v_tbl LIMIT 1000; --<-----  here we say collect 1000 rows at a time.
        FORALL i IN 1 .. v_tbl.count --<------ here we update all 1000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE bp_ae.lw_txndetail_stage ds
             SET  ds.processid  = 1
           WHERE   ds.txnheaderid = v_tbl(i).a_txnheaderid ;
        COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)
        EXIT WHEN get_MDdata%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
     COMMIT;
      IF get_MDdata%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE get_MDdata;
      END IF;
    END;
/

