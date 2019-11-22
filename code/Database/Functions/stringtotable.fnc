CREATE OR REPLACE FUNCTION StringToTable(p_str IN VARCHAR2, p_delim IN VARCHAR2 DEFAULT ',')
RETURN AETableType
AS
  l_str   LONG DEFAULT p_str || p_delim;
  l_count     NUMBER;
  l_data  AETABLETYPE := AETableType();
BEGIN
   LOOP
       l_count := instr(l_str, p_delim);
       EXIT WHEN (nvl(l_count,0) = 0);
       l_data.extend;
       l_data(l_data.count) := ltrim(rtrim(substr(l_str, 1, l_count-1)));
       l_str := substr(l_str,l_count + length(p_delim));
   END LOOP;
  RETURN l_data;
END;
/

