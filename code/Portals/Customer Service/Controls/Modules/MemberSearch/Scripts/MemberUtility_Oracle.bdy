create or replace package body MemberUtility is


-- Function and procedure implementations
procedure FindMember(

                         LoyaltyID       IN VARCHAR2 DEFAULT NULL,
                         FirstName       IN VARCHAR2 DEFAULT NULL,
                         LastName        IN VARCHAR2 DEFAULT NULL,
                         Email           IN VARCHAR2 DEFAULT NULL,
						 NonMember       IN NUMBER,
                         retval          IN OUT rcursor) AS


      V_LoyaltyID              VARCHAR2(255) := TRIM(LoyaltyID);
      V_FIRSTNAME              VARCHAR2(255) := TRIM(upper(FirstName))||'%';
      V_LASTNAME               VARCHAR2(255) := TRIM(upper(LastName)||'%');
      V_Email                  VARCHAR2(255) := TRIM(upper(Email)||'%');
      V_NonMember              NUMBER(1) := NonMember;
      v_sql_string             VARCHAR2(20000);

    BEGIN
      v_sql_string := 'SELECT LM.IPCODE,'||chr(10)||
                      '       LM.FIRSTNAME FirstName,'||chr(10)||
                      '       LM.LASTNAME LastName,'||chr(10)||
                      '       LM.PrimaryEmailAddress Email,'||chr(10)||
                      '       VC.LoyaltyIDNumber LoyaltyID'||chr(10)||
                      'FROM LW_LOYALTYMEMBER      LM'||chr(10)||
                      'LEFT OUTER JOIN LW_VIRTUALCARD VC on LM.IPCODE = VC.IPCODE'||chr(10)||
                      'WHERE 1=1'||chr(10);

      IF V_LoyaltyID IS NOT NULL THEN
        v_sql_string := v_sql_string
                     || 'AND VC.LoyaltyIDNumber = :V_LoyaltyID'||chr(10)||
                        'AND LM.IPCODE    = VC.IPCODE'||chr(10);
        OPEN retval FOR v_sql_string USING  V_LoyaltyID;
    ELSIF V_Email IS NOT NULL AND length(V_Email) > 1 then
    v_sql_string := v_sql_string
        || 'AND upper(LM.PRIMARYEMAILADDRESS) LIKE :V_EMAIL'||chr(10);
    OPEN retval FOR v_sql_string USING V_EMAIL;
      ELSIF V_FIRSTNAME IS NOT NULL AND V_LASTNAME IS NOT NULL THEN
        v_sql_string := v_sql_string
                     || 'AND upper(LM.FIRSTNAME) LIKE :V_FIRSTNAME'||chr(10)||
                        'AND upper(LM.LASTNAME) LIKE :V_LASTNAME'||chr(10);
    if V_Email is not null then
            v_sql_string := v_sql_string
                    || 'AND upper(LM.PRIMARYEMAILADDRESS) LIKE :V_EMAIL'||chr(10);
            OPEN retval FOR v_sql_string USING v_firstname, v_lastname, V_EMAIL;
         else
            OPEN retval FOR v_sql_string USING v_firstname, v_lastname;
         end if;
      END IF;
    END FindMember;

end MemberUtility;