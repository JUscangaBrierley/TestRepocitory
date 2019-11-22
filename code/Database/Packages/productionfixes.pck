CREATE OR REPLACE PACKAGE ProductionFixes IS

  TYPE Rcursor IS REF CURSOR;

  PROCEDURE CTAS_Pointtranction2(p_Dummy       VARCHAR2,
                                Retval        IN OUT Rcursor);
  PROCEDURE Upt_membersentdate(p_Dummy       VARCHAR2,
                                p_ProcessDate VARCHAR2,
                                Retval        IN OUT Rcursor);

END ProductionFixes;
/

CREATE OR REPLACE PACKAGE BODY ProductionFixes IS

PROCEDURE CTAS_Pointtranction2(p_Dummy VARCHAR2, Retval IN OUT Rcursor) AS
        begin
          execute immediate 'Create Table AE_CurrentPointTransaction2 As Select * from lw_pointtransaction pt Where pt.transactiondate > sysdate-1 and rownum =1';
        exception
          when others then
            null;
END CTAS_Pointtranction2;

PROCEDURE Upt_membersentdate(p_Dummy VARCHAR2,  p_Processdate VARCHAR2,  Retval IN OUT Rcursor) AS
   v_Processdate       timestamp:=to_date(REGEXP_REPLACE(p_Processdate,'_',' '), 'MM/dd/YYYY HH24:mi:ss');
        BEGIN
          UPDATE Lw_Clientconfiguration
          SET    VALUE = To_Char(v_Processdate, 'mm/dd/yyyy hh24:mi:ss')
          WHERE  Key = 'LastAEMemberPointsSent';
          COMMIT;

END Upt_membersentdate;

END ProductionFixes;
/

