CREATE OR REPLACE FUNCTION AE_IsInPilot( pstExtPlayCode VARCHAR2) RETURN NUMBER
AS
   lstExtPlayCode CLOB := NULL;
   lbRetVal NUMBER := 0;

   CURSOR c1 is
   SELECT cc.value
     FROM lw_clientconfiguration cc
     WHERE cc.key = 'PilotStatusCodes';

BEGIN

   IF (pstExtPlayCode  IS NULL) THEN
     RETURN lbRetVal;
   END IF;


   OPEN c1;
   FETCH c1 INTO lstExtPlayCode;

   IF c1%NOTFOUND THEN
      lstExtPlayCode := '';
   END IF;

   CLOSE c1;

   IF  INSTR( lstExtPlayCode, TRIM(pstExtPlayCode)) <> 0 THEN
      lbRetVal := 1;
   END IF;

   RETURN lbRetVal;

EXCEPTION
  WHEN OTHERS THEN
      lbRetVal := 0;

  RETURN lbRetVal;


END;
/