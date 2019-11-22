CREATE OR REPLACE PACKAGE Tlog_Util IS
type rcursor IS REF CURSOR;

    PROCEDURE ReActivateMember (p_loyaltyIdnumber  IN VARCHAR2);
END Tlog_Util;
/
CREATE OR REPLACE PACKAGE BODY Tlog_Util   IS

PROCEDURE ReActivateMember(p_loyaltyIdnumber IN VARCHAR2)
IS
  v_memberstatus number(10);
  
BEGIN
   
    UPDATE lw_loyaltymember
       SET memberstatus = 1
     WHERE memberstatus = 2
       AND 1=1
       AND IPCODE in
           (SELECT VC.IPCODE
              FROM lw_virtualcard vc
             WHERE vc.loyaltyidnumber = p_loyaltyIdnumber);
   COMMIT;
END ReActivateMember;


  END Tlog_Util;
/
