CREATE OR REPLACE FUNCTION Extract_Prevrowkey( pstNotes VARCHAR2) RETURN NUMBER
AS
   lstNotes CLOB := NULL;
   lnRetVal NUMBER := 0;
   lnPos    NUMBER :=0;



BEGIN

   IF (pstNotes IS NULL) THEN
     RETURN 0;
   END IF;


   lstNotes:= pstNotes;
   lnPos := INSTR( lstNotes, 'previous rowkey = ');

   IF  lnPos <> 0 THEN
      lnPos := lnPos + LENGTH('previous rowkey = ');
      lstNotes := SUBSTR( lstNotes,lnPos);
      lnRetVal := to_number(lstNotes);

   END IF;

   RETURN lnRetVal;

EXCEPTION
  WHEN OTHERS THEN
      lnRetVal := 0;

  RETURN lnRetVal;


END;
/

