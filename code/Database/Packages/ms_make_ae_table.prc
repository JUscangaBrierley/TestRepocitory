create or replace procedure ms_make_ae_table as
begin
execute immediate 'drop table bp_ae.AE_CurrentPointTransaction2';
execute immediate 'Create Table bp_ae.AE_CurrentPointTransaction2 tablespace bp_ae_d As Select * from lw_pointtransaction pt Where pt.transactiondate > Add_Months(trunc(to_date(''12/31/2013'', ''mm/dd/yyyy''), ''Q''), -3)';
execute immediate 'alter package bp_ae.AEPOINTBALANCE2 compile';
execute immediate 'analyze table bp_ae.AE_CurrentPointTransaction2 estimate statistics sample 10 percent';
end;
/

