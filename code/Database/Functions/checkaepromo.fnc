CREATE OR REPLACE FUNCTION CheckAEPromo(CLASSCD varchar2, PID varchar2,v_bra_codes AETableType,v_jean_codes AETableType)
RETURN varchar2
AS
  Classcodes NVARCHAR2(50):= 0;
  ProductID NVARCHAR2(50):= 0;
  RET_PROMOTYPE NVARCHAR2(50) := 'NotBRAorJEAN';
  BRACODES CLOB;
  JEANCODES CLOB;
  v_sql1  VARCHAR2(1000) ;
BEGIN
    Classcodes := CLASSCD ;
     IF NVL(TRIM(PID),0) = 0 THEN
         ProductID := 0;
      ELSE
        ProductID := PID;
     END IF;
  IF NVL(TRIM(CLASSCD),0) = 0  then
   Select p.classcode into Classcodes from lw_product p where p.id = ProductID ;
  end  if;
  --select cc.value INTO BRACODES from lw_clientconfiguration cc where cc.key  = 'BraPromoClassCodes';
  --select cc.value INTO JEANCODES from lw_clientconfiguration cc where cc.key  = 'JeansPromoClassCodes';
SELECT CASE
  WHEN Classcodes IN
  --(  SELECT COLUMN_VALUE  FROM table(StringToTable((BRACODES),';'))  )
  (select t.COLUMN_VALUE  from table(v_bra_codes) t)
  THEN 'BRAPROMO'
  WHEN Classcodes IN
  --(  SELECT COLUMN_VALUE  FROM table(StringToTable((JEANCODES),';'))  )
   (select t2.COLUMN_VALUE  from table(v_jean_codes) t2)
  THEN  'JEANPROMO'
    ELSE 'NotBRAorJEAN' END  INTO RET_PROMOTYPE  FROM DUAL;

 RETURN RET_PROMOTYPE;
 EXCEPTION
WHEN NO_DATA_FOUND THEN
  Classcodes := 0 ;
  RETURN RET_PROMOTYPE;
  -- dbms_output.put_line(Classcodes);

END;
/

