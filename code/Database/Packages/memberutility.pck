CREATE OR REPLACE PACKAGE MEMBERUTILITY IS
  TYPE RCURSOR IS REF CURSOR;

  -- Public function and procedure declarations

  PROCEDURE FINDMEMBER(LOYALTYIDNUMBER   IN VARCHAR2 DEFAULT NULL,
                       EMAILADDRESS      IN VARCHAR2 DEFAULT NULL,                      
                       MOBILEPHONE       IN VARCHAR2 DEFAULT NULL,
                       LASTNAME          IN VARCHAR2 DEFAULT NULL,
                       PRIMARYPOSTALCODE IN VARCHAR2 DEFAULT NULL,
                       RETVAL            IN OUT RCURSOR);

END MEMBERUTILITY;
/
CREATE OR REPLACE PACKAGE BODY MEMBERUTILITY IS

  -- Function and procedure implementations
  PROCEDURE FINDMEMBER(

                       LOYALTYIDNUMBER   IN VARCHAR2 DEFAULT NULL,
                       EMAILADDRESS      IN VARCHAR2 DEFAULT NULL,                      
                       MOBILEPHONE       IN VARCHAR2 DEFAULT NULL,
                       LASTNAME          IN VARCHAR2 DEFAULT NULL,
                       PRIMARYPOSTALCODE IN VARCHAR2 DEFAULT NULL,
                       -- AEO-1333 BEGIN
                     
                       -- AEO-1333 END
                       RETVAL            IN OUT RCURSOR) AS
    V_EMAILADDRESS         VARCHAR2(255) := TRIM(EMAILADDRESS);
    V_MOBILEPHONE         VARCHAR2(255) := TRIM(UPPER(MOBILEPHONE));
    V_LOYALTYID         VARCHAR2(255) := TRIM(LOYALTYIDNUMBER);
    V_LASTNAME          VARCHAR2(255) := TRIM(UPPER(LASTNAME));
    V_PRIMARYPOSTALCODE VARCHAR2(255) := REGEXP_REPLACE(TRIM(UPPER(PRIMARYPOSTALCODE)),
                                                        '[-[:space:]]',
                                                        '');

    -- fields you are going to return back.
    --  V_IPCODE NUMBER;

    V_SQL_STRING VARCHAR2(20000);
    V_FLAG       BOOLEAN := FALSE;

  BEGIN

    V_SQL_STRING := 'SELECT LM.IPCODE' || CHR(10) ||
                    'FROM LW_LOYALTYMEMBER      LM,' || CHR(10) ||
                    '     LW_VIRTUALCARD        VC,' || CHR(10) ||
                    '     ATS_MEMBERDETAILS     MD' || CHR(10) ||
                    'WHERE 1=1' || CHR(10) ||
                    'AND LM.IPCODE    = VC.IPCODE' || CHR(10) ||
                    'AND md.a_IPCODE    = VC.IPCODE' || CHR(10);
                    

     V_LASTNAME   := REPLACE(V_LASTNAME, '''', '''''');

      -- AEO-500 Begin
          v_Lastname := REPLACE(v_Lastname, '' || Unistr('\2019') || '', '''''');
          v_Lastname := REPLACE(v_Lastname, '' || Unistr('\2018') || '', '''''');
      -- AEO-500 End

    IF V_LOYALTYID IS NOT NULL THEN
      V_SQL_STRING := V_SQL_STRING ||
                      'AND VC.LoyaltyIDNumber = :V_LoyaltyID' || CHR(10);
      OPEN RETVAL FOR V_SQL_STRING
        USING V_LOYALTYID;
    ELSE
      -- aeo-1333 BEGIN
      -- IF NO LOYALTY MEBER TRY TO SEARCH BY E-MAIL ADDRESS
      IF V_EMAILADDRESS IS NOT NULL THEN
        V_SQL_STRING := V_SQL_STRING ||
                        'AND lower(MD.A_EMAILADDRESS) = lower(:V_EMAILADDRESS)' || CHR(10);
        OPEN RETVAL FOR V_SQL_STRING
          USING V_EMAILADDRESS;
      ELSE
        
        IF V_MOBILEPHONE IS NOT NULL THEN
            V_SQL_STRING := V_SQL_STRING ||
                            'AND MD.A_MOBILEPHONE = :V_MOBILEPHONE' || CHR(10);
            OPEN RETVAL FOR V_SQL_STRING
              USING V_MOBILEPHONE;
       ELSE
           --Searching criteria with lastname
          IF V_LASTNAME IS NOT NULL THEN
            V_SQL_STRING := V_SQL_STRING || ' AND upper(LM.LastName) LIKE ''' ||   --AEO-512 changes here
                            UPPER(V_LASTNAME) || '%''' || CHR(10);
            -- OPEN retval FOR v_sql_string USING V_LastName;

          END IF;

          --Searching criteria with postal code
          IF V_PRIMARYPOSTALCODE IS NOT NULL THEN
            V_SQL_STRING := V_SQL_STRING ||
                            ' AND REGEXP_REPLACE(upper(LM.PrimaryPostalCode), ''[-[:space:]]'', '''')  LIKE ''' ||
                            V_PRIMARYPOSTALCODE || '%''' || CHR(10);
            --OPEN retval FOR v_sql_string USING V_PrimaryPostalCode;
            --ELSE

          END IF;

          V_FLAG := V_LASTNAME IS NOT NULL AND V_PRIMARYPOSTALCODE IS NOT NULL;

          IF V_FLAG THEN
            OPEN RETVAL FOR V_SQL_STRING;
          END IF;
            
       END IF ; 
        
      END IF ; 
      -- AEO-1333 END   
    

    END IF;

  END FINDMEMBER;

END MEMBERUTILITY;
/
