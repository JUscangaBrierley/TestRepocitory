--Had to create ae_attribute set table because LW didn't have the description field in 4.1

Drop TABLE DataModelWiki;

CREATE TABLE DataModelWiki
(
  page       NVARCHAR2(200),
  CONTENT    CLOB
);

DECLARE
  CURSOR atsTabs IS
         SELECT * FROM (
         SELECT 'ATS_' || UPPER(b.attributesetname) AS tableName
              , b.Description
              , NVL(connect_by_root owner, b.owner) AS owner
              , LEVEL AS L1
              , LEVEL+1 AS L2
              , DECODE(NVL(connect_by_root owner, b.owner), 'MEMBER', 1, 'VIRTUAL CARD', 2, 'GLOBAL', 3, 4) AS O
              , NVL(connect_by_root attributesetname, b.attributesetname) AS PARENT
         FROM (SELECT a.*, (CASE a.typecode WHEN 1 THEN 'MEMBER' WHEN 2 THEN 'VIRTUAL CARD' WHEN 3 THEN 'GLOBAL' ELSE 'PARENT' END) AS owner FROM ae_attributeset a order by a.attributesetname) b 
         START WITH parentattributesetcode = -1 CONNECT BY PRIOR attributesetcode = parentattributesetcode) ORDER BY O, PARENT, L1;

  dat         CLOB;
  chDat       CLOB;
  lines       NVARCHAR2(4000);
  rowspan     NVARCHAR2(1000);
  prvOwner    NVARCHAR2(20);
  prvParent   NVARCHAR2(20);
  cnt         NUMBER;
BEGIN
  prvOwner := 'A';
  prvParent := 'A';
  
  INSERT INTO DataModelWiki(Page, Content)
  VALUES('Main', empty_Clob())
  RETURNING CONTENT INTO dat;
  
  lines := '==''''''Data Dictionary''''''==
<style>
.dataDictionaryTable
{
   border-collapse:collapse;
   font-family:Helvetica;
   margin-left:30px;
}
.dataDictionaryTable td
{
   border-color:black;
   border-style:solid;
   border-width:1px;
   padding:4px 8px;
}
.dataDictionaryTable .level1IndentColumn
{
   background-color:white !important;
}
.dataDictionaryTable .level2IndentColumn
{
   background-color:white !important;
}
.dataDictionaryTable .level1Title
{
   background-color:#AAA;
   font-size:18px;
   font-weight:bold;
}
.dataDictionaryTable .level2Title
{
   background-color:#DDD; 
   height:1em;
   font-weight:bold;
   font-size:12px;
}
.dataDictionaryTable .level3Title
{
   background-color:#FFF; 
   height:1em;
   font-weight:bold;
   font-size:12px;
}
.dataDictionaryTable .level2Row
{

}
.dataDictionaryTable .level3Row
{

}
</style>
<table class="dataDictionaryTable">';

  dbms_lob.write(lob_loc =>  dat, amount => LENGTH(lines), offset =>  1, buffer => lines);

  FOR atsTab_rec IN atsTabs
  LOOP
    lines := '';
    ----- Set up main page first
    -- Check if we have a new owner
    IF prvOwner <> atsTab_rec.owner THEN
      prvOwner := atsTab_rec.owner;
      lines := '<tr class="level1Title">
    <td colspan="4" class="level1Row">' || prvOwner || '</td>
</tr>';
      dbms_lob.writeappend(dat, LENGTH(lines), lines);
      
      SELECT COUNT(*) INTO cnt FROM (SELECT * FROM (
         SELECT 'ATS_' || UPPER(b.attributesetname) AS tableName
              , b.Description
              , NVL(connect_by_root owner, b.owner) AS owner
              , LEVEL AS L1
              , LEVEL+1 AS L2
              , DECODE(NVL(connect_by_root owner, b.owner), 'MEMBER', 1, 'VIRTUAL CARD', 2, 'GLOBAL', 3, 4) AS O
              , NVL(connect_by_root attributesetname, b.attributesetname) AS PARENT
         FROM (SELECT a.*, (CASE a.typecode WHEN 1 THEN 'MEMBER' WHEN 2 THEN 'VIRTUAL CARD' WHEN 3 THEN 'GLOBAL' ELSE 'PARENT' END) AS owner FROM ae_attributeset a order by a.attributesetname) b 
         START WITH parentattributesetcode = -1 CONNECT BY PRIOR attributesetcode = parentattributesetcode)) WHERE owner = prvOwner;
      
      lines := '';
      rowspan := '<td rowspan="' || cnt || '" class="level' || atsTab_rec.L1 || 'IndentColumn"></td>';
    ELSE
      rowspan := '';
    END IF;
    
    -- Check if this is a new child
    IF atsTab_rec.L1 > 1 AND prvParent <> atsTab_rec.Parent THEN
      prvParent := atsTab_rec.Parent;
      SELECT COUNT(*) INTO cnt FROM (SELECT * FROM (
      SELECT 'ATS_' || UPPER(b.attributesetname) AS tableName
           , b.Description
           , NVL(connect_by_root owner, b.owner) AS owner
           , LEVEL AS L1
           , LEVEL+1 AS L2
           , DECODE(NVL(connect_by_root owner, b.owner), 'MEMBER', 1, 'VIRTUAL CARD', 2, 'GLOBAL', 3, 4) AS O
           , NVL(connect_by_root attributesetname, b.attributesetname) AS PARENT
      FROM (SELECT a.*, (CASE a.typecode WHEN 1 THEN 'MEMBER' WHEN 2 THEN 'VIRTUAL CARD' WHEN 3 THEN 'GLOBAL' ELSE 'PARENT' END) AS owner FROM ae_attributeset a order by a.attributesetname) b 
      START WITH parentattributesetcode = -1 CONNECT BY PRIOR attributesetcode = parentattributesetcode)) WHERE owner = prvOwner AND PARENT = prvParent AND L1 > 1;
      
      lines := '';
      rowspan := '<td rowspan="' || cnt || '" class="level' || atsTab_rec.L1 || 'IndentColumn"></td>';
    ELSIF atsTab_rec.L1 > 1 THEN
      rowspan := '';
    END IF;
    
    lines := '<tr class="level' || atsTab_rec.L2 || 'Title">' || rowspan || '
    <td>[AmericanEagle-Data-Dictionary-' || atsTab_rec.owner || '-' || atsTab_rec.tableName || '|' || atsTab_rec.tableName || ']</td>
    <td colspan="2">'|| atsTab_rec.description ||'</td>
</tr>';
    dbms_lob.writeappend(dat, LENGTH(lines), lines);
  
    ---- Now iterate over child pages
    INSERT INTO DataModelWiki(Page, Content)
    VALUES(atsTab_rec.tableName, empty_Clob())
    RETURNING CONTENT INTO chDat;
    
    -- Set up page header and table header
    lines := '==' || atsTab_rec.tableName || '==
<BR />' || atsTab_rec.description || '
<BR />
<BR />
===Data Dictionary Definition===' || chr(10) || '
  <STYLE>.HEADER_ROW td{background-color:#AAA; height:1em;font-weight:bold;font-size:14px;}.STYLED_TABLE{border-collapse:collapse;font-family:Helvetica;margin-left:30px;}.STYLED_TABLE td{border-color:black;border-style:solid;border-width:1px;padding:4px 8px;}.STYLED_TABLE tr:nth-child(odd){background-color:#DDD;}</STYLE>
  <BR />
  <TABLE CLASS="STYLED_TABLE">
  <TR CLASS="HEADER_ROW"><TD>NAME</TD><TD>TYPE</TD><TD>LENGTH</TD><TD>NULLABLE</TD><TD>DESCRIPTION</TD></TR>' || chr(10);
    
    dbms_lob.write(chDat, LENGTH(lines), 1, lines);
    -- Get the table specs
    FOR tabSpec IN 
      (
        SELECT column_name, data_type, data_length, nullable, a.displaytext
        FROM all_tab_columns t
             left join lw_attributeset ats on lower(substr(t.TABLE_NAME, 5, length(t.TABLE_NAME))) = lower(ats.attributesetname)
             left join lw_attribute a on lower(substr(t.column_name, 3, length(t.column_name))) = lower(a.attributename) and ats.attributesetcode = a.attributesetcode
        WHERE table_name = atsTab_rec.tableName ORDER BY column_id
        --SELECT column_name, data_type, data_length, nullable, 'desc' as Description FROM all_tab_columns WHERE table_name = atsTab_rec.tableName ORDER BY column_id
      )
    LOOP
      lines := '<TR><TD>' || tabSpec.Column_Name || '</TD><TD>' || tabSpec.Data_Type || '</TD><TD>' || tabSpec.Data_Length || '</TD><TD>' || tabSpec.Nullable || '</TD><TD>' || tabSpec.displaytext || '</TD></TR>' || chr(10);
      dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    END LOOP;
    -- Setup keys header
    lines := '<TR CLASS="HEADER_ROW"><TD COLSPAN="5">KEYS</TD></TR>
  <TR CLASS="HEADER_ROW"><TD COLSPAN="2">NAME</TD><TD>TYPE</TD><TD COLSPAN="2">COLUMN</TD></TR>' || chr(10);
    dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    -- Get the table keys
    FOR keySpec IN (SELECT i.INDEX_NAME, DECODE(c.CONSTRAINT_TYPE, 'P', 'PRIMARY', 'UNIQUE') AS TYPE, ic.COLUMN_NAME FROM All_Indexes i
                    INNER JOIN All_Ind_Columns ic ON i.INDEX_NAME = ic.INDEX_NAME
                    LEFT JOIN All_Constraints c ON i.INDEX_NAME = c.INDEX_NAME
                    WHERE i.table_name = atsTab_rec.tableName AND i.UNIQUENESS = 'UNIQUE')
    LOOP
      lines := '<TR><TD COLSPAN="2">' || keySpec.Index_Name || '</TD><TD>' || keySpec.Type || '</TD><TD COLSPAN="2">' || keySpec.Column_Name || '</TD></TR>' || chr(10);
      dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    END LOOP;
    -- Setup foreign keys header
    lines := '<TR CLASS="HEADER_ROW"><TD COLSPAN="5">FORIEGN KEYS</TD></TR>
  <TR CLASS="HEADER_ROW"><TD COLSPAN="2">NAME</TD><TD>COLUMN</TD><TD>REFERENCING TABLE</TD><TD>REFERENCING COLUMN</TD></TR>' || chr(10);
    dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    -- Get the foreign keys
    FOR fkSpec IN (SELECT c_list.CONSTRAINT_NAME as NAME,
                          c_src.COLUMN_NAME as SRC_COLUMN,
                          c_dest.TABLE_NAME as DEST_TABLE,
                          c_dest.COLUMN_NAME as DEST_COLUMN
                   FROM ALL_CONSTRAINTS c_list, ALL_CONS_COLUMNS c_src, ALL_CONS_COLUMNS c_dest
                   WHERE c_list.CONSTRAINT_NAME = c_src.CONSTRAINT_NAME
                   AND c_list.R_CONSTRAINT_NAME = c_dest.CONSTRAINT_NAME
                   AND c_list.CONSTRAINT_TYPE = 'R'
                   AND c_src.TABLE_NAME = atsTab_rec.tableName
                   GROUP BY c_list.CONSTRAINT_NAME, c_src.TABLE_NAME, c_src.COLUMN_NAME, c_dest.TABLE_NAME,    c_dest.COLUMN_NAME)
    LOOP
      lines := '<TR><TD COLSPAN="2">' || fkSpec.Name || '</TD><TD>' || fkSpec.Src_Column || '</TD><TD>' || fkSpec.Dest_Table || '</TD><TD>' || fkSpec.Dest_Column || '</TD></TR>' || chr(10);
      dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    END LOOP;
    -- Setup indexes header
    lines := '<TR CLASS="HEADER_ROW"><TD COLSPAN="5">INDEXES</TD></TR>
  <TR CLASS="HEADER_ROW"><TD colspan="2">NAME</TD><TD>TYPE</TD><TD colspan="2">COLUMN</TD></TR>' || chr(10);
    dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    -- Get the indexes
    FOR indSpec IN (SELECT i.INDEX_NAME, i.index_type, c.COLUMN_NAME FROM All_Indexes i
                    INNER JOIN all_ind_columns c ON i.INDEX_NAME = c.INDEX_NAME
                    WHERE i.table_name = atsTab_rec.tableName AND i.UNIQUENESS = 'NONUNIQUE')
    LOOP
      lines := '<TR><TD colspan="2">' || indSpec.Index_Name || '</TD><TD>' || indSpec.Index_Type || '</TD><TD colspan="2">' || indSpec.Column_Name || '</TD></TR>' || chr(10);
      dbms_lob.writeappend(chDat, LENGTH(lines), lines);
    END LOOP;
    -- Close the table
    lines := '</TABLE>';
    dbms_lob.writeappend(chDat, LENGTH(lines), lines);
  
  END LOOP;
  lines := '</table>';
  -- Write out the lines
  dbms_lob.writeappend(dat, LENGTH(lines), lines);
  COMMIT;

END;
/
SELECT * FROM datamodelwiki ORDER BY page;
