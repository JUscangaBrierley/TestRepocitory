CREATE OR REPLACE PACKAGE STAGE_PROFILEUPDATES_DAILY IS
  TYPE RCURSOR IS REF CURSOR;

  PROCEDURE STAGE_PROFILEUPDATES02(P_FILENAME IN VARCHAR2,
                                   RETVAL     IN OUT RCURSOR);
  PROCEDURE update_memberdetails( n_partition INTEGER, n_mod INTEGER);
  PROCEDURE update_loyaltymember( n_partition INTEGER, n_mod INTEGER);
  PROCEDURE update_virtualcard  ( n_partition INTEGER, n_mod INTEGER);
  PROCEDURE update_atssynkey;
  PROCEDURE get_mergelist ;
  FUNCTION isDateValid( p_date VARCHAR2, p_format VARCHAR2) RETURN INTEGER;



END STAGE_PROFILEUPDATES_DAILY;
/
CREATE OR REPLACE PACKAGE BODY STAGE_PROFILEUPDATES_DAILY IS


  CURSOR GET_ENROLLMENTS IS
        SELECT *
        FROM BP_AE.Ext_Profileupdates2 pu
        LEFT  JOIN Bp_Ae.lw_virtualcard vc ON     Pu.Loyaltynumber = Vc.Loyaltyidnumber
        WHERE  (vc.loyaltyidnumber IS NULL)
        ORDER BY pu.cid ASC, pu.recordtype DESC ;-- AEO-1950 force primary be the most recently issued

  TYPE T_TAB_ENR IS TABLE OF GET_ENROLLMENTS%ROWTYPE;

  -- AEO-1867 begin
  CURSOR GET_UPDATES IS
    SELECT pu.*
    FROM BP_AE.Ext_Profileupdates2 pu
    INNER JOIN BP_AE.ae_Profileupdates2 pu2 on pu2.LOYALTYNUMBER = pu.loyaltynumber
        WHERE  (pu2.isenrollment = 0);

  TYPE T_TAB_UPD IS TABLE OF GET_UPDATES%ROWTYPE;
  -- AEO-1867 END

  -- AEO-1835 begin
  TYPE T_ENROLL_FLAGS IS VARRAY(31) OF INTEGER;

  c_cid CONSTANT INTEGER := 1;
  c_FirstName CONSTANT INTEGER := 2;
  c_LastName  CONSTANT INTEGER := 3;
  c_Line1  CONSTANT INTEGER := 4;
  c_Line2  CONSTANT INTEGER := 5;
  c_City  CONSTANT INTEGER :=6;
  c_State  CONSTANT INTEGER := 7;
  c_Zip  CONSTANT INTEGER := 8;
  c_Country CONSTANT INTEGER := 9;
  c_BirthDate CONSTANT INTEGER := 10;
  c_MobileNumber CONSTANT INTEGER := 11;
  c_Gender CONSTANT INTEGER := 12;
  c_LoyaltyNumber  CONSTANT INTEGER := 13;
  c_EmailAddress CONSTANT INTEGER := 14;
  c_ValidAddress  CONSTANT INTEGER := 15;
  c_RecordType  CONSTANT INTEGER := 16;
  c_ValidEmailAddress  CONSTANT INTEGER := 17;
  c_CellDoubleOptIn CONSTANT INTEGER := 18;
  c_EmployeeStatus  CONSTANT INTEGER := 19;
  c_StoreNumber  CONSTANT INTEGER := 20;
  c_LanguagePreference CONSTANT INTEGER := 21;
  c_EnrollmentDate CONSTANT INTEGER := 22;
  c_EnrollmentSource CONSTANT INTEGER := 23;
  c_EnrollmentID CONSTANT INTEGER := 24;
  c_AECCStatus CONSTANT INTEGER := 25;
  c_AECCOpenDate CONSTANT INTEGER := 26;
  c_AECCCloseDate CONSTANT INTEGER := 27;
  c_AEVisaStatus CONSTANT INTEGER := 28;
  c_AEVisaAccountKey CONSTANT INTEGER := 29;
  c_AEVisaOpenDate CONSTANT INTEGER := 30;
  c_AEVisaCloseDate CONSTANT INTEGER := 31;

-- AEO-1835 END

/* AEO-1440 validate each field and  store an exception */

 FUNCTION isDateValid( p_date VARCHAR2, p_format VARCHAR2) RETURN INTEGER
  AS
    lbValid  INTEGER := 0;
    ldFecha  DATE := SYSDATE;

  BEGIN

    IF (regexp_like(p_date, '[[:digit:]]{4}[[:digit:]]{2}[[:digit:]]{2}')) THEN
      BEGIN
         ldFecha := to_date(p_date, p_format);
         lbvalid  := 1;
         EXCEPTION WHEN  OTHERS  THEN
           lbvalid := 0;
           NULL;
        END;

    END IF;

    RETURN lbValid;
  END isDateValid;


FUNCTION validate_row_enrollment( pnROW NUMBER ,
                                  v_enr T_TAB_ENR,
                                  flags OUT T_ENROLL_FLAGS,
                                  filename  VARCHAR2) RETURN NUMBER --AEO-1835
AS

   lbRetVal NUMBER := 0;
   lsTmpval VARCHAR(255) := NULL;
   ldTmp  DATE := SYSDATE;
   lvexist NUMBER := 0;
   lv_lyid NUMBER := NULL;
   lv_isvalid NUMBER :=0;
   lv_errormsg VARCHAR2(255) := NULL;
   lv_onlyprimary BOOLEAN := TRUE; --AEO-1950 begin-end
   lv_secondarycount NUMBER := 0;  --AEO-1950 begin-end
   lv_count NUMBER := 0;   --AEO-1950 begin-end



BEGIN


   flags := T_ENROLL_FLAGS(1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,1) ; -- AEO-1835



-- loyalty number

   utility_pkg.validate_loyaltynumber( v_enr(pnROW).loyaltynumber,
                                       lv_errormsg , lv_isvalid) ;

   IF (  lv_isvalid = 0 ) THEN

     flags(c_LoyaltyNumber) := 0;
     lstmpval := v_enr(pnrow).loyaltynumber;
     lbRetVal := -1;

     INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                    (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                      Errorcode,
                       sourcefilename)
     VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Enrollment',
                      'LoyaltyNumber',
                      Lstmpval,
                      lv_errormsg, filename);

   END IF;



-- first name
  lstmpval := v_enr(pnROW).firstname || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_FirstName) := 0;
           lstmpval := v_enr(pnROW).firstname;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                    (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                      Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'Firstname',
                      Lstmpval,
                      'Empty Field',filename);
       END IF ;
  END;

-- last name
  lstmpval := v_enr(pnROW).lastname || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
            flags(c_LastName) := 0;
           lstmpval := v_enr(pnROW).lastname;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                      Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'Lastname',
                      Lstmpval,
                      'Empty Field',filename);
       END IF ;
  END;


-- line one
  lstmpval := v_enr(pnROW).ADDRESSLINE1 || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_Line1) := 0;
           lstmpval := v_enr(pnROW).ADDRESSLINE1;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                      Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ADDRESSLINE1',
                      Lstmpval,
                      'Empty field', filename);
       END IF ;
  END;


-- line two
  lstmpval := v_enr(pnROW).ADDRESSLINE2 || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_Line2) := 0;
           lstmpval := v_enr(pnROW).ADDRESSLINE2;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                      Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ADDRESSLINE2',
                      Lstmpval,
                      'Empty field', filename);
       END IF ;
  END;



-- city
  lstmpval := v_enr(pnROW).CITY || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags( c_city) := 0;
           lstmpval := v_enr(pnROW).CITY;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                       Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'CITY',
                      Lstmpval,
                      'Empty field',filename);
       END IF ;
  END;

-- state or province
   lstmpval := v_enr(pnROW).state;

   SELECT COUNT(*)
   INTO   Lvexist
   FROM   Bp_Ae.Ats_Refstates t
   WHERE  Upper(t.a_Statecode) = Upper(Lstmpval);

  IF lvexist = 0  THEN
      --  lbretval := -1;
        flags(c_State):= 0;
      /*  INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'STATE/PROVINCE',
              lstmpval,
              'Invalid State/Province');*/
  END IF;


-- country

  lstmpval := v_enr(pnROW).country ||'X';

  IF ( lstmpval = 'X') THEN
     --lbretval := -1 ;
     flags(c_country) := 0;
     lstmpval := v_enr(pnROW).country;
    INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
         (Cid,
          Loyaltynumber,
          Exceptiontype,
          Fieldname,
          Valuereceived,
           Errorcode, sourcefilename)
    VALUES
         (v_Enr(Pnrow).Cid,
          v_Enr(Pnrow).Loyaltynumber,
          'Rejected Field',
          'COUNTRY',
          lstmpval,
          'Empty Field',filename);
  ELSE
     lstmpval := v_enr(pnROW).country;
     IF NOT ( trim(lsTmpVal) IN ('USA','US','CAN','CA' )) THEN --AEO-1711
        --lbretval := -1;
        flags(c_country) := 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'COUNTRY',
              lstmpval,
              'Invalid Country',filename);
     END IF;

  END IF;


-- zip
  lstmpval := v_enr(pnROW).ZIP || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_zip):= 0;
           lstmpval := v_enr(pnROW).ZIP;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                     Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ZIP',
                      Lstmpval,
                      'Empty field',filename);
       ELSE
          lstmpval := v_enr(pnROW).ZIP;
          IF (  v_enr(pnROW).country = 'CAN' or v_enr(pnROW).country = 'CA')  THEN
             IF ( NOT REGEXP_LIKE(  lsTmpval, '^([ABCEGHJKLMNPRSTVXY]{1}[0-9]{1}[A-Z]{1}\s[0-9]{1}[A-Z]{1}[0-9]{1}$)|(^[ABCEGHJKLMNPRSTVXY]{1}[0-9]{1}[A-Z]{1}[0-9]{1}[A-Z]{1}[0-9]{1}$)')) THEN
                   flags(c_zip):= 0;
                   lstmpval := v_enr(pnROW).ZIP;
                   INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                               (Cid,
                                Loyaltynumber,
                                Exceptiontype,
                                Fieldname,
                                Valuereceived,
                            Errorcode, sourcefilename)
                   VALUES
                               (v_Enr(Pnrow).Cid,
                                v_Enr(Pnrow).Loyaltynumber,
                                'Rejected Field',
                                'ZIP',
                                Lstmpval,
                                'Invalid value', filename);
               END IF ;
          ELSE
            IF ( v_enr(pnROW).country = 'USA' or v_enr(pnROW).country = 'US')  THEN
               IF ( NOT REGEXP_LIKE(  lsTmpval,'^\d{5}$') )THEN
                   flags(c_zip):= 0;
                   lstmpval := v_enr(pnROW).ZIP;
                   INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                               (Cid,
                                Loyaltynumber,
                                Exceptiontype,
                                Fieldname,
                                Valuereceived,
                                 Errorcode, sourcefilename)
                   VALUES
                               (v_Enr(Pnrow).Cid,
                                v_Enr(Pnrow).Loyaltynumber,
                                'Rejected Field',
                                'ZIP',
                                Lstmpval,
                                'Invalid value',filename);
               END IF  ;

            END IF ;
          END IF ;
       END IF ;
  END;


-- Birthdate date
  lstmpval := v_enr(pnROW).BIRTHDATE || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_BirthDate) := 0;
           lstmpval := v_enr(pnROW).BIRTHDATE;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                   Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'BIRTHDATE',
                  Lstmpval,
                  'Invalid Birthdate Date', filename);
       ELSE
          lstmpval := v_enr(pnROW).BIRTHDATE;
           Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
       END IF ;
  EXCEPTION
       WHEN OTHERS THEN
            --Lbretval := -1;
             flags(c_BirthDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                   Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'BIRTHDATE',
                  Lstmpval,
                  'Invalid Birthdate Date',filename);
            NULL;

  END;

-- record type
  lstmpval := v_enr(pnROW).recordtype || 'X';
  IF NOT ( trim(lstmpval)= 'SX' OR TRIM(lstmpval) ='PX')THEN
        lstmpval :=  v_enr(pnROW).recordtype;
        --lbretval := -1;
        flags(c_RecordType) := 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'RECORDTYPE',
              lstmpval,
              'Invalid Value',filename);
  END IF;



-- validemailaddress
  lstmpval := v_enr(pnROW).validemailaddress || 'X';
  IF NOT ( trim(lstmpval)= '1X' OR TRIM(lstmpval) ='0X')THEN
        lstmpval :=  v_enr(pnROW).validemailaddress;
        flags(c_ValidEmailAddress):= 0;
        --lbretval := -1;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'VALIDEMAILADDRESS',
              lstmpval,
              'Invalid Value', filename);
  END IF;


-- CELLDOUBLEOPTIN
  lstmpval := v_enr(pnROW).CELLDOUBLEOPTIN || 'X';
  IF NOT ( trim(lstmpval)= '1X' OR TRIM(lstmpval) ='0X')THEN
        lstmpval :=  v_enr(pnROW).CELLDOUBLEOPTIN;
        flags(c_CellDoubleOptIn):= 0;
        --lbretval := -1;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'CELLDOUBLEOPTIN',
              lstmpval,
              'Invalid Value', filename);
  END IF;

  -- Storenumber
   lstmpval := v_enr(pnROW).storenumber;

   SELECT COUNT(*)
   INTO   Lvexist
   FROM   Bp_Ae.Lw_Storedef t
   WHERE  t.storenumber = lstmpval;

  IF lvexist = 0  THEN
       -- lbretval := -1;
        flags(c_StoreNumber):= 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'INFORMATIONAL',
              'STORENUMBER',
              lstmpval,
              'Unrecognized Store Number', filename);
  END IF;


-- language
  lstmpval := v_enr(pnROW).LANGUAGEPREFERENCE || 'X';
  IF NOT ( trim(lstmpval)= '1X' OR TRIM(lstmpval) ='2X'OR TRIM(lstmpval) ='0X' )THEN
        lstmpval :=  v_enr(pnROW).LANGUAGEPREFERENCE;
        --lbretval := -1;
        flags(c_LanguagePreference) := 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'LANGUAGEPREFERENCE',
              lstmpval,
              'Invalid Value', filename);
  END IF;


-- Enrollment source

  lstmpval := v_enr(pnROW).enrollmentsource ||'X';

  IF ( lstmpval = 'X') THEN
     lbretval := -1 ;
     flags(c_EnrollmentSource) := 0;
     lstmpval := v_enr(pnROW).enrollmentsource;
    INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
         (Cid,
          Loyaltynumber,
          Exceptiontype,
          Fieldname,
          Valuereceived,
            Errorcode, sourcefilename)
    VALUES
         (v_Enr(Pnrow).Cid,
          v_Enr(Pnrow).Loyaltynumber,
          'Rejected Enrollment',
          'ENROLLMENTSOURCE',
          lstmpval,
          'Invalid Enrollment Source', filename);
  ELSE
     lstmpval := v_enr(pnROW).enrollmentsource;
     IF (v_enr(pnROW).recordtype = 'P') THEN

       SELECT COUNT(*)
       INTO lv_secondarycount
       FROM BP_AE.AE_PROFILEUPDATES2 PU2
       WHERE PU2.CID = V_ENR(PNROW).CID AND
             PU2.RECORDTYPE <> 'P';

       lv_onlyprimary := lv_secondarycount = 0;

       IF (lv_onlyprimary) THEN


           IF NOT ( trim(lsTmpVal) IN ('23','24','25','26','27','28','29','30','31' )) THEN --AEO-1711
              lbretval := -1;
              flags(c_EnrollmentSource) := 0;
              INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                   (Cid,
                    Loyaltynumber,
                    Exceptiontype,
                    Fieldname,
                    Valuereceived,
                    Errorcode, sourcefilename)
              VALUES
                   (v_Enr(Pnrow).Cid,
                    v_Enr(Pnrow).Loyaltynumber,
                    'Rejected Enrollment',
                    'ENROLLMENTSOURCE',
                    lstmpval,
                    'Invalid Enrollment Source', filename);
           END IF;

       ELSE

         SELECT COUNT(*)
         INTO lv_count
         FROM BP_AE.ATS_REFMEMBERSOURCE RM
         WHERE RM.A_MEMBERSOURCECODE IS NOT NULL  AND
               rm.a_membersourcecode  = v_enr(pnROW).enrollmentsource;

         IF ( lv_count = 0 ) THEN

              lbretval := -1;
              flags(c_EnrollmentSource) := 0;
              INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                   (Cid,
                    Loyaltynumber,
                    Exceptiontype,
                    Fieldname,
                    Valuereceived,
                   Errorcode, sourcefilename)
              VALUES
                   (v_Enr(Pnrow).Cid,
                    v_Enr(Pnrow).Loyaltynumber,
                    'Rejected Enrollment',
                    'ENROLLMENTSOURCE',
                    lstmpval,
                    'Invalid Enrollment Source', filename);

           END IF;


         END IF;
       END IF;
     END IF;


--  END IF;

 -- Enrollment date


  lstmpval := v_enr(pnROW).enrollmentdate || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           lbRetVal := -1;
           flags(c_Enrollmentdate) := 0;
           lstmpval := v_enr(pnROW).enrollmentdate;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                      Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Enrollment',
                      'ENROLLMENTDATE',
                      Lstmpval,
                      'Invalid Enrollment Date',filename);
       ELSE
         lstmpval := v_enr(pnROW).enrollmentdate;
         Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
       END IF ;
  EXCEPTION
       WHEN OTHERS THEN
            Lbretval := -1;
            flags(c_Enrollmentdate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                  Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Enrollment',
                  'ENROLLMENTDATE',
                  Lstmpval,
                  'Invalid Enrollment Date',filename);
            NULL;

  END;


-- aeccSTATUS
  lstmpval := v_enr(pnROW).AECCSTATUS || 'X';

  IF (lstmpval <>'X') AND NOT REGEXP_LIKE(  v_enr(pnROW).aeccstatus,'^\d{1}$') THEN
        lstmpval :=  v_enr(pnROW).AECCSTATUS;
        flags(c_AECCStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AECCSTATUS',
              lstmpval,
              'Invalid Value',filename);
  ELSE
     IF (lstmpval = 'X') THEN
        lstmpval :=  v_enr(pnROW).AECCSTATUS;
        flags(c_AECCStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AECCSTATUS',
              lstmpval,
              'Invalid Value',filename);
     END IF ;
  END IF;



-- aeccopendate
lstmpval := v_enr(pnROW).AECCOPENDATE || 'X';

IF  (lstmpval <> 'X') THEN
  lstmpval := v_enr(pnROW).AECCOPENDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
          --  Lbretval := -1;
           flags( c_AECCOpenDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                  Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AECCOPENDATE',
                  Lstmpval,
                  'Invalid value',filename);
            NULL;

  END;
END IF;

-- aeccclosedate

lstmpval := v_enr(pnROW).AECCCLOSEDATE || 'X';

IF  (lstmpval <> 'X') THEN
  lstmpval := v_enr(pnROW).AECCCLOSEDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
           -- Lbretval := -1;
            flags( c_AECCCloseDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                  Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AECCCLOSEDATE',
                  Lstmpval,
                  'Invalid value',filename);
            NULL;

  END;
END IF;



-- AEVISTATUS
  lstmpval := v_enr(pnROW).AEVISASTATUS || 'X';
  IF (lstmpval <>'X') AND NOT REGEXP_LIKE(  v_enr(pnROW).aevisastatus,'^\d{1}$') THEN
        lstmpval :=  v_enr(pnROW).AEVISASTATUS;
        flags(c_AEVisaStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AEVISTATUS',
              lstmpval,
              'Invalid Value');
  ELSE
    IF(lstmpval ='X') THEN
         lstmpval :=  v_enr(pnROW).AEVISASTATUS;
         flags(c_AEVisaStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode, sourcefilename)
            VALUES (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AEVISTATUS',
              lstmpval,
              'Invalid Value',filename);
    END IF;
  END IF;

  --AEVISAACCOUNTKEY
  lstmpval := v_enr(pnROW).AEVISAACCOUNTKEY ;
  IF ( (NOT REGEXP_LIKE( lstmpval, '^[0-9]{1,20}$')))THEN
        lstmpval :=  v_enr(pnROW).AEVISAACCOUNTKEY;
        --lbretval := -1;
        flags(c_AEVisaAccountKey):= 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AEVISAACCOUNTKEY',
              lstmpval,
              'Invalid Value',filename);
  END IF;

  -- Gender
 /* lstmpval := v_enr(pnROW).GENDER|| 'X';
  IF NOT ( trim(lstmpval)= 'X' or trim(lstmpval)= '1X' OR TRIM(lstmpval) ='2X') THEN
        lbretval := -1;
        lstmpval := v_enr(pnROW).GENDER;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'GENDER',
              lstmpval,
              'Invalid Value');

  END IF;*/




lstmpval := v_enr(pnROW).AEVISAOPENDATE || 'X';

IF  (lstmpval <> 'X') THEN
  lstmpval := v_enr(pnROW).AEVISAOPENDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
            --Lbretval := -1;
            flags( c_AEVisaOpenDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                  Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AEVISAOPENDATE',
                  Lstmpval,
                  'Invalid value',filename);
            NULL;

  END;
END IF;


lstmpval := v_enr(pnROW).AEVISACLOSEDATE || 'X';
lstmpval := REPLACE(REPLACE(lstmpval, CHR(10)),CHR(13));
IF  ( trim(lstmpval) <> 'X') THEN
  lstmpval := v_enr(pnROW).AEVISACLOSEDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
           -- Lbretval := -1;
           flags( c_AEVisaCloseDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                  Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AEVISACLOSEDATE',
                  Lstmpval,
                  'Invalid Value',filename);
            NULL;

  END;
END IF;


  COMMIT;


   RETURN lbRetVal;

EXCEPTION
  WHEN OTHERS THEN
      lbRetVal :=-1;
      lstmpval:=  SUBSTR(SQLERRM, 1, 200);
      COMMIT;

  RETURN lbRetVal;


END;
/* AEO-1440  end */

FUNCTION validate_row_update ( pnROW NUMBER ,
                                  v_enr T_TAB_UPD,
                                  flags OUT T_ENROLL_FLAGS,
                                  filename VARCHAR2) RETURN NUMBER
AS

   lbRetVal NUMBER := 0;
   lsTmpval VARCHAR(255) := NULL;
   ldTmp  DATE := SYSDATE;
   lvexist NUMBER := 0;
   lvexist1 NUMBER := 0;
   lv_lyid NUMBER := NULL;


   lvenrdate VARCHAR(255) := NULL;
   lvmembersource NUMBER := 0;
   lvipcode  NUMBER := 0;
   lv_isvalid number := 0;
   lv_errormsg VARCHAR2(255) := NULL;



BEGIN


   flags := T_ENROLL_FLAGS(1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,1) ;
-- loyalty number

 utility_pkg.validate_loyaltynumber( v_enr(pnROW).loyaltynumber,
                                       lv_errormsg , lv_isvalid) ;


   IF (  lv_isvalid = 0 ) THEN

     flags(c_LoyaltyNumber) := 0;
     lstmpval := v_enr(pnrow).loyaltynumber;

     INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                    (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                        Errorcode, sourcefilename)
     VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Enrollment',
                      'LoyaltyNumber',
                      Lstmpval,
                      lv_errormsg, filename);

   END IF  ;



-- first name
  lstmpval := v_enr(pnROW).firstname || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_FirstName) := 0;
           lstmpval := v_enr(pnROW).firstname;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                    (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                       Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'Firstname',
                      Lstmpval,
                      'Empty Field', filename);
       END IF ;
  END;

-- last name
  lstmpval := v_enr(pnROW).lastname || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
            flags(c_LastName) := 0;
           lstmpval := v_enr(pnROW).lastname;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                       Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'Lastname',
                      Lstmpval,
                      'Empty Field',filename);
       END IF ;
  END;


-- line one
  lstmpval := v_enr(pnROW).ADDRESSLINE1 || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_Line1) := 0;
           lstmpval := v_enr(pnROW).ADDRESSLINE1;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                        Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ADDRESSLINE1',
                      Lstmpval,
                      'Empty field',filename);
       END IF ;
  END;


-- line two
  lstmpval := v_enr(pnROW).ADDRESSLINE2 || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_Line2) := 0;
           lstmpval := v_enr(pnROW).ADDRESSLINE2;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                        Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ADDRESSLINE2',
                      Lstmpval,
                      'Empty field',filename);
       END IF ;
  END;



-- city
  lstmpval := v_enr(pnROW).CITY || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags( c_city) := 0;
           lstmpval := v_enr(pnROW).CITY;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                        Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'CITY',
                      Lstmpval,
                      'Empty field', filename);
       END IF ;
  END;

-- state or province
   lstmpval := v_enr(pnROW).state;

   SELECT COUNT(*)
   INTO   Lvexist
   FROM   Bp_Ae.Ats_Refstates t
   WHERE  Upper(t.a_Statecode) = Upper(Lstmpval);

  IF lvexist = 0  THEN
      --  lbretval := -1;
        flags(c_State):= 0;
        /*
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'STATE/PROVINCE',
              lstmpval,
              'Invalid State/Province');*/
  END IF;


-- country

  lstmpval := v_enr(pnROW).country ||'X';

  IF ( lstmpval = 'X') THEN
     --lbretval := -1 ;
     flags(c_country) := 0;
     lstmpval := v_enr(pnROW).country;
    INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
         (Cid,
          Loyaltynumber,
          Exceptiontype,
          Fieldname,
          Valuereceived,
           Errorcode, sourcefilename)
    VALUES
         (v_Enr(Pnrow).Cid,
          v_Enr(Pnrow).Loyaltynumber,
          'Rejected Field',
          'COUNTRY',
          lstmpval,
          'Empty Field',filename);
  ELSE
     lstmpval := v_enr(pnROW).country;
     IF NOT ( trim(lsTmpVal) IN ('USA','CAN', 'US','CA' )) THEN --AEO-1711
        --lbretval := -1;
        flags(c_country) := 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'COUNTRY',
              lstmpval,
              'Invalid Country',filename);
     END IF;

  END IF;


-- zip
  lstmpval := v_enr(pnROW).ZIP || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_zip):= 0;
           lstmpval := v_enr(pnROW).ZIP;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                       Errorcode, sourcefilename)
           VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ZIP',
                      Lstmpval,
                      'Empty field',filename);
       ELSE
          lstmpval := v_enr(pnROW).ZIP;
          IF (  v_enr(pnROW).country = 'CAN' OR v_enr(pnROW).country = 'CA')  THEN
             IF ( NOT REGEXP_LIKE(  lsTmpval, '^([ABCEGHJKLMNPRSTVXY]{1}[0-9]{1}[A-Z]{1}\s[0-9]{1}[A-Z]{1}[0-9]{1}$)|(^[ABCEGHJKLMNPRSTVXY]{1}[0-9]{1}[A-Z]{1}[0-9]{1}[A-Z]{1}[0-9]{1}$)')) THEN
                   flags(c_zip):= 0;
                   lstmpval := v_enr(pnROW).ZIP;
                   INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                               (Cid,
                                Loyaltynumber,
                                Exceptiontype,
                                Fieldname,
                                Valuereceived,
                                 Errorcode, sourcefilename)
                   VALUES
                               (v_Enr(Pnrow).Cid,
                                v_Enr(Pnrow).Loyaltynumber,
                                'Rejected Field',
                                'ZIP',
                                Lstmpval,
                                'Invalid value',filename);
               END IF ;
          ELSE
            IF ( v_enr(pnROW).country = 'USA' OR v_enr(pnROW).country = 'US')  THEN
               IF ( NOT REGEXP_LIKE(  lsTmpval,'^\d{5}$') )THEN
                   flags(c_zip):= 0;
                   lstmpval := v_enr(pnROW).ZIP;
                   INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                               (Cid,
                                Loyaltynumber,
                                Exceptiontype,
                                Fieldname,
                                Valuereceived,
                                  Errorcode, sourcefilename)
                   VALUES
                               (v_Enr(Pnrow).Cid,
                                v_Enr(Pnrow).Loyaltynumber,
                                'Rejected Field',
                                'ZIP',
                                Lstmpval,
                                'Invalid value', filename);
               END IF  ;

            END IF ;
          END IF ;
       END IF ;
  END;


-- Birthdate date
  lstmpval := v_enr(pnROW).BIRTHDATE || 'X';
  BEGIN
       IF ( lstmpval = 'X') THEN
           --lbRetVal := -1;
           flags(c_BirthDate) := 0;
           lstmpval := v_enr(pnROW).BIRTHDATE;
           INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                    Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'BIRTHDATE',
                  Lstmpval,
                  'Invalid Birthdate Date', filename);
       ELSE
          lstmpval := v_enr(pnROW).BIRTHDATE;
           Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
       END IF ;
  EXCEPTION
       WHEN OTHERS THEN
            --Lbretval := -1;
             flags(c_BirthDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                    Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'BIRTHDATE',
                  Lstmpval,
                  'Invalid Birthdate Date', filename);
            NULL;

  END;

-- record type
  lstmpval := v_enr(pnROW).recordtype || 'X';
  IF NOT ( trim(lstmpval)= 'SX' OR TRIM(lstmpval) ='PX')THEN
        lstmpval :=  v_enr(pnROW).recordtype;
        --lbretval := -1;
        flags(c_RecordType) := 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'RECORDTYPE',
              lstmpval,
              'Invalid Value', filename);
  END IF;



-- validemailaddress
  lstmpval := v_enr(pnROW).validemailaddress || 'X';
  IF NOT ( trim(lstmpval)= '1X' OR TRIM(lstmpval) ='0X')THEN
        lstmpval :=  v_enr(pnROW).validemailaddress;
        flags(c_ValidEmailAddress):= 0;
        --lbretval := -1;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'VALIDEMAILADDRESS',
              lstmpval,
              'Invalid Value', filename);
  END IF;


-- CELLDOUBLEOPTIN
  lstmpval := v_enr(pnROW).CELLDOUBLEOPTIN || 'X';
  IF NOT ( trim(lstmpval)= '1X' OR TRIM(lstmpval) ='0X')THEN
        lstmpval :=  v_enr(pnROW).CELLDOUBLEOPTIN;
        flags(c_CellDoubleOptIn):= 0;
        --lbretval := -1;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
               Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'CELLDOUBLEOPTIN',
              lstmpval,
              'Invalid Value', filename);
  END IF;

  -- Storenumber
   lstmpval := v_enr(pnROW).storenumber;

   SELECT COUNT(*)
   INTO   Lvexist
   FROM   Bp_Ae.Lw_Storedef t
   WHERE  t.storenumber = lstmpval;

  IF lvexist = 0  THEN
       -- lbretval := -1;
        flags(c_StoreNumber):= 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'INFORMATIONAL',
              'STORENUMBER',
              lstmpval,
              'Unrecognized Store Number', filename);
  END IF;


-- language
  lstmpval := v_enr(pnROW).LANGUAGEPREFERENCE || 'X';
  IF NOT ( trim(lstmpval)= '1X' OR TRIM(lstmpval) ='2X'OR TRIM(lstmpval) ='0X' )THEN
        lstmpval :=  v_enr(pnROW).LANGUAGEPREFERENCE;
        --lbretval := -1;
        flags(c_LanguagePreference) := 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'LANGUAGEPREFERENCE',
              lstmpval,
              'Invalid Value', filename);
  END IF;


-- Enrollment date

  lstmpval := v_enr(pnROW).enrollmentdate;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
          --  Lbretval := -1;
           flags( c_EnrollmentDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                   Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'EnrollmentDate',
                  Lstmpval,
                  'Invalid value',filename);
            NULL;

  END;


  IF ( flags(c_LoyaltyNumber) = 1 AND flags(c_EnrollmentDate)= 1) THEN

    SELECT COUNT(*)
    INTO lvexist
    FROM bp_ae.lw_virtualcard vc
    WHERE vc.loyaltyidnumber = v_enr(pnROW).loyaltynumber;

    IF ( lvexist = 1 ) THEN

        SELECT trunc(vc.dateissued), vc.ipcode
        INTO lvenrdate, lvipcode
        FROM bp_ae.lw_virtualcard vc
        WHERE vc.loyaltyidnumber = v_enr(pnROW).loyaltynumber;

        IF ( lvenrdate <>  ldtmp) THEN

          flags(c_EnrollmentDate) := 0;
          lstmpval := v_enr(pnROW).enrollmentdate;
          INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
               (Cid,
                Loyaltynumber,
                Exceptiontype,
                Fieldname,
                Valuereceived,
                 Errorcode, sourcefilename)
          VALUES
               (v_Enr(Pnrow).Cid,
                v_Enr(Pnrow).Loyaltynumber,
                'Rejected Field',
                'ENROLLMENTDATE',
                lstmpval,
                'Invalid Enrollment Date', filename);

        END IF;

        SELECT COUNT(*)
        INTO lvexist1
        FROM bp_ae.ats_memberdetails md
        WHERE md.a_ipcode = lvipcode;

        IF (lvexist1 = 1) THEN

             SELECT md.a_membersource
             INTO lvmembersource
             FROM bp_ae.ats_memberdetails md
             WHERE md.a_ipcode = lvipcode;

             IF ( to_char(lvmembersource) <>  v_enr(pnROW).enrollmentsource ) THEN
               lstmpval := v_enr(pnROW).enrollmentsource;
               flags(c_EnrollmentSource) := 0;
               INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                     (Cid,
                      Loyaltynumber,
                      Exceptiontype,
                      Fieldname,
                      Valuereceived,
                        Errorcode, sourcefilename)
               VALUES
                     (v_Enr(Pnrow).Cid,
                      v_Enr(Pnrow).Loyaltynumber,
                      'Rejected Field',
                      'ENROLLMENTSOURCE',
                      lstmpval,
                      'Invalid Enrollment Source',filename);

             END IF;
        END IF;

    END IF;


  END IF;

-- aeccSTATUS
  lstmpval := v_enr(pnROW).AECCSTATUS || 'X';

  IF (lstmpval <>'X') AND NOT REGEXP_LIKE(  v_enr(pnROW).aeccstatus,'^\d{1}$') THEN
        lstmpval :=  v_enr(pnROW).AECCSTATUS;
        flags(c_AECCStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AECCSTATUS',
              lstmpval,
              'Invalid Value', filename);
  ELSE
     IF (lstmpval = 'X') THEN
        lstmpval :=  v_enr(pnROW).AECCSTATUS;
        flags(c_AECCStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AECCSTATUS',
              lstmpval,
              'Invalid Value', filename);
     END IF ;
  END IF;



-- aeccopendate
lstmpval := v_enr(pnROW).AECCOPENDATE || 'X';

IF  (lstmpval <> 'X') THEN
  lstmpval := v_enr(pnROW).AECCOPENDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
          --  Lbretval := -1;
           flags( c_AECCOpenDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                   Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AECCOPENDATE',
                  Lstmpval,
                  'Invalid value', filename);
            NULL;

  END;
END IF;

-- aeccclosedate

lstmpval := v_enr(pnROW).AECCCLOSEDATE || 'X';

IF  (lstmpval <> 'X') THEN
  lstmpval := v_enr(pnROW).AECCCLOSEDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
           -- Lbretval := -1;
            flags( c_AECCCloseDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                   Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AECCCLOSEDATE',
                  Lstmpval,
                  'Invalid value', filename);
            NULL;

  END;
END IF;



-- AEVISTATUS
  lstmpval := v_enr(pnROW).AEVISASTATUS || 'X';
  IF (lstmpval <>'X') AND NOT REGEXP_LIKE(  v_enr(pnROW).aevisastatus,'^\d{1}$') THEN
        lstmpval :=  v_enr(pnROW).AEVISASTATUS;
        flags(c_AEVisaStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AEVISTATUS',
              lstmpval,
              'Invalid Value', filename);
  ELSE
    IF(lstmpval ='X') THEN
         lstmpval :=  v_enr(pnROW).AEVISASTATUS;
         flags(c_AEVisaStatus):= 0;

        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
                Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AEVISTATUS',
              lstmpval,
              'Invalid Value', filename);
    END IF;
  END IF;

  --AEVISAACCOUNTKEY
  lstmpval := v_enr(pnROW).AEVISAACCOUNTKEY ;
  IF ( (NOT REGEXP_LIKE( lstmpval, '^[0-9]{1,20}$')))THEN
        lstmpval :=  v_enr(pnROW).AEVISAACCOUNTKEY;
        --lbretval := -1;
        flags(c_AEVisaAccountKey):= 0;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode, sourcefilename)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'AEVISAACCOUNTKEY',
              lstmpval,
              'Invalid Value', filename);
  END IF;

  -- Gender
 /* lstmpval := v_enr(pnROW).GENDER|| 'X';
  IF NOT ( trim(lstmpval)= 'X' or trim(lstmpval)= '1X' OR TRIM(lstmpval) ='2X') THEN
        lbretval := -1;
        lstmpval := v_enr(pnROW).GENDER;
        INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
             (Cid,
              Loyaltynumber,
              Exceptiontype,
              Fieldname,
              Valuereceived,
              Errorcode)
        VALUES
             (v_Enr(Pnrow).Cid,
              v_Enr(Pnrow).Loyaltynumber,
              'Rejected Field',
              'GENDER',
              lstmpval,
              'Invalid Value');

  END IF;*/




lstmpval := v_enr(pnROW).AEVISAOPENDATE || 'X';

IF  (lstmpval <> 'X') THEN
  lstmpval := v_enr(pnROW).AEVISAOPENDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
            --Lbretval := -1;
            flags( c_AEVisaOpenDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                  Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AEVISAOPENDATE',
                  Lstmpval,
                  'Invalid value', filename);
            NULL;

  END;
END IF;


lstmpval := v_enr(pnROW).AEVISACLOSEDATE || 'X';
lstmpval := REPLACE(REPLACE(lstmpval, CHR(10)),CHR(13));
IF  ( trim(lstmpval) <> 'X') THEN
  lstmpval := v_enr(pnROW).AEVISACLOSEDATE;
  BEGIN
       Ldtmp := To_Date(Lstmpval, 'MMDDYYYY'); -- Will raise ORA-1847, ORA-1843 or ora-1846
   EXCEPTION
       WHEN OTHERS THEN
           -- Lbretval := -1;
           flags( c_AEVisaCloseDate) := 0;
            INSERT INTO Bp_Ae.Ae_Profileupdates2_Exceptions
                 (Cid,
                  Loyaltynumber,
                  Exceptiontype,
                  Fieldname,
                  Valuereceived,
                   Errorcode, sourcefilename)
            VALUES
                 (v_Enr(Pnrow).Cid,
                  v_Enr(Pnrow).Loyaltynumber,
                  'Rejected Field',
                  'AEVISACLOSEDATE',
                  Lstmpval,
                  'Invalid Value', filename);
            NULL;

  END;
END IF;


  COMMIT;


   RETURN lbRetVal;

EXCEPTION
  WHEN OTHERS THEN
      lbRetVal :=-1;
      lstmpval:=  SUBSTR(SQLERRM, 1, 200);
      COMMIT;

  RETURN lbRetVal;


END;
/* AEO-1440  end */

  PROCEDURE RESET_ISPRIMARY IS
    -- this cursor has only the ipcode field
    -- we will update the field isprimary for all lw_virtualcrd rows for each member
    -- included in the ext file.
    -- we will not update those memebers that are ccancelled (status == 3)


			CURSOR GET_DATA IS
        SELECT   Vc1.vckey, t.cid
            FROM  bp_ae.lw_virtualcard vc1
            INNER JOIN (

        SELECT DISTINCT vc.ipcode, p.cid
         FROM bp_ae.AE_PROFILEUPDATES2 P
         INNER JOIN  bp_ae.Lw_Virtualcard Vc    ON     Vc.Loyaltyidnumber = p.Loyaltynumber
         INNER JOIN bp_ae.LW_LOYALTYMEMBER LM ON LM.IPCODE = VC.IPCODE
         WHERE LM.MEMBERSTATUS <> 3  AND
                p.loyaltynumber NOT IN ( SELECT pe.loyaltyid
                                        FROM bp_Ae.Ae_Profileupdateexception pe
                                           WHERE pe.notes = 'Members Merged in BP Db' AND
                                                          trunc(pe.processdate) > trunc(SYSDATE))) t ON vc1.ipcode = t.ipcode;
      --   WHERE vc1.isprimary = 1;

      /*
     sELECT  DISTINCT Vc.vckey, p.cid
         FROM bp_ae.AE_PROFILEUPDATES2 P
         INNER JOIN  bp_ae.Lw_Virtualcard Vc    ON     Vc.Loyaltyidnumber = p.Loyaltynumber
         INNER JOIN bp_ae.LW_LOYALTYMEMBER LM ON LM.IPCODE = VC.IPCODE
         WHERE LM.MEMBERSTATUS <> 3 AND vc.isprimary = 1 AND
                p.loyaltynumber NOT IN ( SELECT pe.loyaltyid FROM bp_Ae.Ae_Profileupdateexception pe
                                           WHERE pe.notes = 'Members Merged in BP Db' AND
                                                          trunc(pe.processdate) > trunc(SYSDATE));*/


        TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
        V_TBL T_TAB;
        i INT;
    BEGIN
        OPEN GET_DATA;
        LOOP
          FETCH GET_DATA BULK COLLECT
            INTO V_TBL LIMIT 100;
          FORALL I IN 1 .. V_TBL.COUNT
              UPDATE  lw_virtualcard vc
                  SET vc.isprimary = 0,
                      vc.linkkey =v_tbl(i).cid,
				              vc.updatedate = sysdate
              WHERE vc.vckey = V_TBL(I).vckey ;
          COMMIT;
          EXIT WHEN GET_DATA%NOTFOUND;
        END LOOP;
        COMMIT;
        IF GET_DATA%ISOPEN THEN
          CLOSE GET_DATA;
        END IF;



    END RESET_ISPRIMARY;


PROCEDURE ctas_profileupdates2 is

BEGIN


begin
  execute immediate 'drop  table bp_Ae.ae_profileupdates2_new ';
exception when others then
  null;
end;

--Create tablename_New
--aeo-1440 BEGIN
EXECUTE IMMEDIATE
'Create table bp_ae.ae_profileupdates2_new AS
SELECT Pu.Cid,
       Pu.Firstname,
       Pu.Lastname,
       Pu.Addressline1,
       Pu.Addressline2,
       Pu.City,
       Pu.State,
       Pu.Zip,
       Pu.Country,
       CASE   WHEN Br.Loyaltynumber IS NOT NULL THEN
             CASE   WHEN Br.Birthdatefi IS NOT NULL THEN
                    BR.BIRTHDATEFI
                  ELSE
                   BR.BIRTHDATEDB
             END
       ELSE
             PU.BIRTHDATE
       END AS Birthdate,
       Pu.Mobilenumber,
       Pu.Gender,
       Pu.Loyaltynumber,
       Pu.Emailaddress,
       Pu.Validaddress,
       CASE   WHEN Mu.Floyaltynumber IS NOT NULL THEN
             CASE    WHEN Hp.Loyaltynumber IS NOT NULL THEN
                   To_Char(Hp.Newrecordtype)
                  ELSE
                   To_Char(Mu.Newrecordtype)
             END
            ELSE
             CASE    WHEN Hp.Loyaltynumber IS NOT NULL THEN
                   To_Char(Hp.Newrecordtype)
                  ELSE
                   To_Char(Pu.Recordtype)
             END
       END AS Recordtype,
       Pu.Validemailaddress,
       Pu.Celldoubleoptin,
       Pu.Employeestatus,
       Pu.Storenumber,
       Pu.Languagepreference,
       Pu.Enrollmentdate,
       Pu.Enrollmentsource,
       Pu.Enrollmentid,
       Pu.Aeccstatus,
       Pu.Aeccopendate,
       Pu.Aeccclosedate,
       Pu.Aevisastatus,
       Pu.Aevisaaccountkey,
       Pu.Aevisaopendate,
       Pu.Aevisaclosedate,
       Pu.Isenrollment
FROM   Bp_Ae.Ae_Profileupdates2 Pu
INNER  JOIN Bp_Ae.Lw_Virtualcard Vc ON     Vc.Loyaltyidnumber = Pu.Loyaltynumber
LEFT   JOIN Bp_Ae.Ae_Tmpvcbirthdate Br ON     Br.Loyaltynumber = Vc.Loyaltyidnumber
LEFT   JOIN Bp_Ae.Ae_Meberstoupdate Mu ON     Mu.Floyaltynumber = Pu.Loyaltynumber
LEFT   JOIN Bp_Ae.Ae_Hubsynchelpertbl Hp ON     Hp.Loyaltynumber = Pu.Loyaltynumber';
--Table rename to old

-- AEO-1440 END


    BEGIN

        EXECUTE IMMEDIATE 'DROP TABLE BP_AE.ae_profileupdates2_old PURGE';

    EXCEPTION

        WHEN OTHERS THEN
            IF sqlcode = -942 THEN
                NULL;
            ELSE
               RAISE;
            END IF;
    END;


EXECUTE IMMEDIATE 'Alter table bp_ae.ae_profileupdates2  rename to ae_profileupdates2_old';



--Drop indexes
    EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPDATECID_IX';
    EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPDATELID_IX';
    EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPD_LOYID';
    EXECUTE IMMEDIATE 'drop INDEX IX_LOYALTYNUMBER_TYPE';


-- AEO-1440 BEGIN
    EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX AE_ProfUpdateLID_ix   ON Ae_Profileupdates2_new( SYS_OP_C2C("LOYALTYNUMBER") ) ONLINE COMPUTE STATISTICS';
    EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX AE_PROFUPD_LOYID   ON Ae_Profileupdates2_new(LOYALTYNUMBER)  ONLINE COMPUTE STATISTICS';
    EXECUTE IMMEDIATE 'CREATE        INDEX ix_loyaltynumber_type ON AE_PROFILEUPDATES2_new(LOYALTYNUMBER, RECORDTYPE)  ONLINE COMPUTE STATISTICS' ;
    EXECUTE IMMEDIATE 'CREATE        INDEX AE_ProfUpdateCID_ix   ON Ae_Profileupdates2_new( cid )  ONLINE COMPUTE STATISTICS';
-- aeo-1440 END

--Rename Tablename_New
EXECUTE IMMEDIATE 'Alter Table  bp_ae.ae_profileupdates2_new  Rename  to ae_profileupdates2';


END;


  PROCEDURE RESET_RECORDTYPE IS
    -- this cursor has only the ipcode field
    -- we will update the field recordtype for all AE_PROFILEUPDATES2 rows for each member
    -- included in the ext file.
    -- we will not update those memebers that are ccancelled (status == 3)

      CURSOR GET_DATA IS
         SELECT Pu.Cid,
                Vc.Ipcode,
                Pu.Loyaltynumber,
                Pu.Recordtype AS Oldrecordtype,
                CASE
                     WHEN Vcprimary.Recordtype = 'P' THEN
                      'P'
                     ELSE
                      'S'
                END AS Recordtype
         FROM   Ae_Profileupdates2 Pu
         INNER  JOIN Lw_Virtualcard Vc      ON     Vc.Loyaltyidnumber = Pu.Loyaltynumber
         LEFT   OUTER JOIN (SELECT Vcard.Ipcode,
                                   Vcard.Loyaltyidnumber,
                                   'P' AS Recordtype
                            FROM   (SELECT Vc1.*,
                                           row_number() over(PARTITION BY vc1.ipcode ORDER BY vc1.dateissued DESC) AS Numrow
                                    FROM   Lw_Virtualcard Vc1) Vcard
                            WHERE  Vcard.Numrow = 1) Vcprimary
         ON     Vcprimary.Loyaltyidnumber = Pu.Loyaltynumber;

        TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
        V_TBL T_TAB;
        i INT;
    BEGIN

        UPDATE bp_ae.AE_PROFILEUPDATES2 pu
        SET pu.RECORDTYPE = 'S';
        COMMIT;


        OPEN GET_DATA;
        LOOP
          FETCH GET_DATA BULK COLLECT
            INTO V_TBL LIMIT 100;
          FORALL I IN 1 .. V_TBL.COUNT
              UPDATE  AE_PROFILEUPDATES2 PU
                  SET PU.Recordtype = V_TBL(I).RecordType
              WHERE PU.Loyaltynumber = V_TBL(I).loyaltynumber;
          COMMIT;
          EXIT WHEN GET_DATA%NOTFOUND;
        END LOOP;
        COMMIT;
        IF GET_DATA%ISOPEN THEN
          CLOSE GET_DATA;
        END IF;



    END RESET_RECORDTYPE;

PROCEDURE get_mergelist IS
     V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End
    err_code    VARCHAR2(10) := '';
    err_msg     VARCHAR2(200) := '';
    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    P_FILENAME VARCHAR2 (50):= 'STAGE_PROFILEUPDATES2';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);

    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
		V_NOMERGE   NUMBER := 0;
		V_REASONNOMERGE  VARCHAR2(256);
    DML_ERRORS EXCEPTION;
    lv_secondaryipcode NUMBER := 0;
    lv_isprimarycredit BOOLEAN := FALSE;
    lv_issecondarycredit BOOLEAN := FALSE;
    lv_isprimarypilot BOOLEAN := FALSE;
    lv_issecondarypilot BOOLEAN := FALSE;
    lv_primaryDateIssued DATE := NULL;
    lv_secondaryDateIssued DATE := NULL;

    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);

      CURSOR GET_DATA IS
         SELECT
                  a.cid,
                  a.primary,
                  a.secondary,
                  v1.loyaltyidnumber as vc_primary,
                  v1.vckey           AS vckey_primary,
                  nvl(md1.a_extendedplaycode,0) AS primary_ext,
                  nvl(md1.a_cardtype,0) AS primary_ctype,
                  md1.a_ipcode AS primary_ipcode,
                  v1.dateissued AS  primary_date,
                  v2.loyaltyidnumber as vc_secondary,
                  v2.vckey           AS vckey_secondary,
                  nvl(a.a_extendedplaycode,0) AS  secondary_ext,
                  nvl(a.a_cardtype,0) AS  secondary_ctype,
                  v2.dateissued AS secondary_date,
                  a.a_ipcode AS secondary_ipcode,
                  a.primarystatus,
                  a.secondarystatus
             FROM
                           (SELECT P2.Cid,
                                   P2.Loyaltynumber     AS Primary,
                                   w.Loyaltynumber      AS Secondary,
                                   lm2.memberstatus    AS primarystatus,
                                   w.secondarystatus   AS secondarystatus,
                                   w.a_Extendedplaycode,
                                   w.a_cardtype,
                                   w.a_ipcode
                            FROM   Bp_Ae.Ae_Profileupdates2 P2,
                                   Bp_Ae.Lw_Virtualcard Vc,
                                   bp_ae.lw_loyaltymember lm2,
                                    (  SELECT p.Cid,
                                             p.Recordtype,
                                             p.Loyaltynumber,
                                             Md.a_Extendedplaycode,
                                             Lm.Memberstatus AS secondarystatus,
                                             md.a_cardtype,
                                             md.a_ipcode
                                      FROM   Bp_Ae.Ae_Profileupdates2 p,
                                             Bp_Ae.Lw_Virtualcard     Vc,
                                             Bp_Ae.Ats_Memberdetails  Md,
                                             BP_ae.Lw_Loyaltymember   Lm
                                      WHERE  Vc.Loyaltyidnumber = p.Loyaltynumber
                                             AND lm.ipcode = vc.ipcode
                                             AND Md.a_Ipcode = Vc.Ipcode
                                             AND p.Recordtype = 'S') w   ,

                                (  SELECT Ex.Loyaltyid
                                   FROM   Bp_Ae.Ae_Profileupdateexception Ex
                                   WHERE  Ex.Notes = 'Members Merged in BP Db')e
                            WHERE  vc.loyaltyidnumber = p2.loyaltynumber
                                   AND lm2.ipcode = vc.ipcode
                                   and  w.cid = p2.cid
                                   and p2.recordtype = 'P'
                                   and p2.loyaltynumber = e.loyaltyid(+)
                                   and e.loyaltyid is null) a,
                                      bp_ae.lw_virtualcard v1,
                                      bp_ae.lw_virtualcard v2,
                                      bp_ae.ats_memberdetails md1
              where a.primary = v1.loyaltyidnumber(+)
                     and a.secondary = v2.loyaltyidnumber(+)
                     AND md1.a_ipcode = v1.ipcode
                      and v1.loyaltyidnumber is not null
                       and v2.loyaltyidnumber is not null
                      and v1.loyaltyidnumber != v2.loyaltyidnumber
                      AND v1.ipcode <> v2.ipcode;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;


  BEGIN
     V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
     V_JOBNAME   := 'get_mergelist';
     V_LOGSOURCE := V_JOBNAME;


     UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);
    V_ERROR          :=  ' ';
    V_REASON         := 'write Ats_Profileupdatemerge begin';
    V_MESSAGE        := 'write Ats_Profileupdatemerge begin';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;


          FOR I IN 1 .. V_TBL.COUNT LOOP


            -- detect exception when the accounts should not be merged
            --
            V_NOMERGE := 0; --AEO-1848
            lv_isprimarycredit     := v_tbl(i).primary_ctype > 0;
            lv_issecondarycredit   := v_tbl(i).secondary_ctype > 0;
            lv_primaryDateIssued   := v_tbl(i).primary_date;
            lv_secondaryDateIssued := v_tbl(i).secondary_date;

            IF ( v_tbl(i).primarystatus <> 1 OR v_tbl(i).secondarystatus <> 1 )	THEN
					     V_NOMERGE := 1;
					     V_REASONNOMERGE := '. Cannot merge inactive accounts';
			      END IF;

            IF ( V_NOMERGE > 0 ) THEN
                 INSERT INTO ae_ProfileUpdateException pe
                        ( LoyaltyID  , processdate, notes)
                 VALUES
                     (v_Tbl(i).vc_secondary,
                      SYSDATE,
                      'Account can''t be merged with '||
                      v_Tbl(i).vc_primary ||
                      V_REASONNOMERGE);
            ELSE
              -- add to the merge list
                 INSERT INTO Ats_Profileupdatemerge
                     (a_Rowkey,
                      a_Ipcode,
                      a_Parentrowkey,
                      a_Fromloyaltyid,
                      a_Toloyaltyid,
                      Statuscode,
                      Createdate,
                      Updatedate,
                      Lwidentifier,
                      Lastdmlid,
                      a_Cid,
                      Last_Dml_Id,
                      a_status)
                VALUES
                     (Seq_Rowkey.Nextval,
                      Lv_Secondaryipcode,
                      -1,
                      v_Tbl(i).Secondary,
                      v_Tbl(i).Primary,
                      0,
                      SYSDATE ,
                      SYSDATE ,
                      0,
                      0,
                      v_Tbl(i).Cid,
                      Last_Dml_Id#.Nextval,
                      0);

               IF ( v_tbl(i).primarystatus <> 1 OR v_tbl(i).secondarystatus <> 1 )THEN

                          INSERT INTO Ae_Profileupdateexception
                               (Loyaltyid, Processdate, Notes)
                          VALUES
                               (v_Tbl(i).Vc_Secondary,
                                SYSDATE,
                                'The account is not active and will be merged with ' || v_Tbl(i)
                                .Vc_Primary);

                END IF;

            END IF ;

          END LOOP;



          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;



      V_ERROR          :=  ' ';
      V_REASON         := 'write Ats_Profileupdatemerge end';
      V_MESSAGE        := 'write Ats_Profileupdatemerge end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


 EXCEPTION
          WHEN OTHERS THEN
             ROLLBACK;
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'get_mergelist'||  err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;


             UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
              RAISE ;
  END get_mergelist;

  /********* Procedure to map external table to specified file ********/
  PROCEDURE CHANGEEXTERNALTABLE(P_FILENAME IN VARCHAR2) IS
    E_MTABLE    EXCEPTION;
    E_MFILENAME EXCEPTION;
    V_SQL VARCHAR2(400);
  BEGIN

    IF LENGTH(TRIM(P_FILENAME)) = 0 OR P_FILENAME IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001,
                              'Filename is required to link with external table',
                              FALSE);
    END IF;

    V_SQL := 'ALTER TABLE EXT_PROFILEUPDATES2 LOCATION (AE_IN' || CHR(58) || '''' ||
             P_FILENAME || ''')';
    EXECUTE IMMEDIATE V_SQL;

  END CHANGEEXTERNALTABLE;




  /********************************************************************
  ********************************************************************
  ********************************************************************
  ********************************************************************/
  PROCEDURE STAGE_PROFILEUPDATES02(P_FILENAME IN VARCHAR2,
                                   RETVAL     IN OUT RCURSOR) IS

    V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_FILENAME         VARCHAR2(512) := P_FILENAME;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);
    V_PROCESSID        NUMBER := 0;

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := P_FILENAME;
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;
    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);
    v_flags   T_ENROLL_FLAGS;
    lv_exist NUMBER := 0; -- AEO-1950

  BEGIN

  v_flags := T_ENROLL_FLAGS(1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,1);

   CHANGEEXTERNALTABLE(P_FILENAME => P_FILENAME);
    /* get job id for this process and the dap process */
    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    -- V_DAP_LOG_ID := UTILITY_PKG.GET_LIBJOBID(); AEO-374 Begin - End

    V_JOBNAME   := 'ProfileUpdates';
    V_LOGSOURCE := V_JOBNAME;

    /* log start of job */
    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => 'stage' || V_JOBNAME);

    utility_pkg.Log_Process_Start(v_jobname, 'truncating tables', v_processId);
    V_REASON := 'truncating tables';
    v_messagesreceived := 0;
   -- EXECUTE IMMEDIATE  'CREATE        INDEX ix_loyaltynumber_type ON AE_PROFILEUPDATES2(LOYALTYNUMBER, RECORDTYPE)  COMPUTE STATISTICS' ;
   -- EXECUTE IMMEDIATE  'CREATE        INDEX AE_ProfUpdateCID_ix   ON Ae_Profileupdates2( cid )  COMPUTE STATISTICS';
   -- EXECUTE IMMEDIATE  'CREATE UNIQUE INDEX AE_ProfUpdateLID_ix   ON Ae_Profileupdates2( SYS_OP_C2C("LOYALTYNUMBER") ) COMPUTE STATISTICS';
   -- EXECUTE IMMEDIATE  'CREATE UNIQUE INDEX AE_PROFUPD_LOYID    ON Ae_Profileupdates2 (LOYALTYNUMBER)  COMPUTE STATISTICS';


   -- EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPDATECID_IX';
   -- EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPDATELID_IX';
   -- EXECUTE IMMEDIATE 'drop INDEX AE_PROFUPD_LOYID';
   -- EXECUTE IMMEDIATE 'drop INDEX IX_LOYALTYNUMBER_TYPE';


    SELECT COUNT(*)
    INTO lv_exist
    FROM BP_AE.LW_CLIENTCONFIGURATION CC
    WHERE CC.KEY = 'LastProfileInProcessing';

    IF ( lv_exist = 1 ) THEN
      UPDATE lw_clientconfiguration cc
        SET cc.key  = TO_CHAR(SYSDATE, 'MM/DD/YYYY HH24:mi:ss')
      WHERE cc.key ='LastProfileInProcessing';
    ELSE
        INSERT INTO LW_CLIENTCONFIGURATION
          (KEY, VALUE, CREATEDATE, LAST_DML_ID, UPDATEDATE, EXTERNALVALUE)
        VALUES
          ('LastProfileInProcessing',
           TO_CHAR(SYSDATE, 'MM/DD/YYYY HH24:mi:ss'),
           SYSDATE,
           LAST_DML_ID#.NEXTVAL,
           SYSDATE,
           1);
    END IF;
    COMMIT;

    EXECUTE IMMEDIATE 'Truncate Table ae_hubsynchelpertbl';
    EXECUTE IMMEDIATE 'Truncate Table Ae_Hubsynpointsvckey';
    EXECUTE IMMEDIATE 'Truncate Table ATS_ProfileUpdateMerge';
    EXECUTE IMMEDIATE 'Truncate Table ae_profileupdateexception';
    EXECUTE IMMEDIATE 'Truncate Table ae_tmpvcbirthdate';
    EXECUTE IMMEDIATE 'Truncate Table lw_profileupdates_stage';
    EXECUTE IMMEDIATE 'Truncate Table AE_PROFILEUPDATES2';
    EXECUTE IMMEDIATE 'Truncate Table AE_MEBERSTOUPDATE';
    EXECUTE IMMEDIATE 'Truncate Table ae_hubsyncchangeprogram';

   -- EXECUTE IMMEDIATE 'truncate table ae_profileupdates2_exceptions'; -- AEO-1440
    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
	----

	  DECLARE
      CURSOR GET_DATA IS
        SELECT * FROM EXT_PROFILEUPDATES2 P;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

    BEGIN

	  utility_pkg.Log_Process_Start(v_jobname, 'Write to AE_PROFILEUPDATES2', v_processId);
    v_messagesreceived := 0;
    V_REASON := 'Write to AE_PROFILEUPDATES2';

    OPEN GET_DATA;
    LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;
        FORALL I IN 1 .. V_TBL.COUNT

          INSERT INTO AE_PROFILEUPDATES2 --AEO-1450 begin
            (
                  CID ,
                  FIRSTNAME ,
                  LASTNAME,
                  ADDRESSLINE1 ,
                  ADDRESSLINE2 ,
                  CITY ,
                  STATE,
                  ZIP ,
                  COUNTRY ,
                  BIRTHDATE ,
                  MOBILENUMBER,
                  GENDER ,
                  LOYALTYNUMBER ,
                  EMAILADDRESS ,
                  VALIDADDRESS ,
                  RECORDTYPE ,
                  VALIDEMAILADDRESS ,
                  CELLDOUBLEOPTIN ,
                  EMPLOYEESTATUS ,
                  STORENUMBER ,
                  LANGUAGEPREFERENCE ,
                  ENROLLMENTDATE ,
                  ENROLLMENTSOURCE ,
                  ENROLLMENTID ,
                  AECCSTATUS ,
                  AECCOPENDATE  ,
                  AECCCLOSEDATE ,
                  AEVISASTATUS     ,
                  AEVISAACCOUNTKEY ,
                  AEVISAOPENDATE ,
                  AEVISACLOSEDATE ,
                  ISENROLLMENT
             )
          VALUES
            (
                v_tbl(i).CID ,
                v_tbl(i).FIRSTNAME ,
                  v_tbl(i).LASTNAME,
                  v_tbl(i).ADDRESSLINE1 ,
                  v_tbl(i).ADDRESSLINE2 ,
                  v_tbl(i).CITY ,
                  v_tbl(i).STATE,
                  v_tbl(i).ZIP,
                  v_tbl(i).COUNTRY,
                  CASE WHEN v_tbl(i).BIRTHDATE IS NOT NULL AND
                            isDateValid(v_Tbl(i).birthdate,'MMDDYYYY')=1 THEN
                        to_date(v_Tbl(i).birthdate,'MMDDYYYY')

                   ELSE NULL END,

                  v_tbl(i).MOBILENUMBER,
                  v_tbl(i).GENDER ,
                  v_tbl(i).LOYALTYNUMBER ,
                  v_tbl(i).EMAILADDRESS ,
                  v_tbl(i).VALIDADDRESS,
                  v_tbl(i).RECORDTYPE,
                  v_tbl(i).VALIDEMAILADDRESS,
                  v_tbl(i).CELLDOUBLEOPTIN ,
                  v_tbl(i).EMPLOYEESTATUS ,
                  v_tbl(i).STORENUMBER ,
                  v_tbl(i).LANGUAGEPREFERENCE ,
                  CASE WHEN v_tbl(i).ENROLLMENTDATE IS NOT NULL AND
                            isDateValid(v_Tbl(i).ENROLLMENTDATE,'MMDDYYYY')=1 THEN
                        to_date(v_Tbl(i).ENROLLMENTDATE,'MMDDYYYY')
                  ELSE NULL END,
                  v_tbl(i).ENROLLMENTSOURCE ,
                  v_tbl(i).ENROLLMENTID ,
                  v_tbl(i).AECCSTATUS,
                   CASE WHEN v_tbl(i).AECCOPENDATE IS NOT NULL  AND
                            isDateValid(v_Tbl(i).AECCOPENDATE,'MMDDYYYY')=1 THEN
                        to_date(v_Tbl(i).AECCOPENDATE,'MMDDYYYY')
                   ELSE NULL END  ,
                   CASE WHEN v_tbl(i).AECCCLOSEDATE IS NOT NULL  AND
                            isDateValid(v_Tbl(i).AECCCLOSEDATE,'MMDDYYYY')=1 THEN
                        to_date(v_Tbl(i).AECCCLOSEDATE,'MMDDYYYY')
                   ELSE NULL END     ,
                  CASE WHEN V_FLAGS(c_AEVisaStatus) =1 THEN  v_tbl(i).AEVISASTATUS
                  ELSE NULL
                  END   ,
                  TRIM ( LEADING '0' FROM v_tbl(i).AEVISAACCOUNTKEY) ,
                  CASE WHEN v_tbl(i).AEVISAOPENDATE IS NOT NULL   AND
                            isDateValid(v_Tbl(i).AEVISAOPENDATE,'MMDDYYYY')=1 THEN
                        to_date(v_Tbl(i).AEVISAOPENDATE,'MMDDYYYY')
                   ELSE NULL END  ,
                   CASE WHEN REPLACE(REPLACE(v_tbl(i).AEVISACLOSEDATE, CHR(10)),CHR(13)) IS NOT NULL   AND
                            isDateValid(REPLACE(REPLACE(v_tbl(i).AEVISACLOSEDATE, CHR(10)),CHR(13)),'MMDDYYYY')=1 THEN
                        to_date(REPLACE(REPLACE(v_tbl(i).AEVISACLOSEDATE, CHR(10)),CHR(13)),'MMDDYYYY')
                   ELSE NULL END,
                  0  -- AEO-1450 end
             );
        COMMIT;
        EXIT WHEN GET_DATA%NOTFOUND;
    END LOOP;
    COMMIT;
    IF GET_DATA%ISOPEN THEN
       CLOSE GET_DATA;
    END IF;

    -- AEO-1440 begin (adding online )
   -- EXECUTE IMMEDIATE  'CREATE        INDEX ix_loyaltynumber_type ON AE_PROFILEUPDATES2(LOYALTYNUMBER, RECORDTYPE) online  COMPUTE STATISTICS' ;
   -- EXECUTE IMMEDIATE  'CREATE        INDEX AE_ProfUpdateCID_ix   ON Ae_Profileupdates2( cid )  online COMPUTE STATISTICS';
   -- EXECUTE IMMEDIATE  'CREATE UNIQUE INDEX AE_ProfUpdateLID_ix   ON Ae_Profileupdates2( SYS_OP_C2C("LOYALTYNUMBER") ) online COMPUTE STATISTICS';
   -- EXECUTE IMMEDIATE  'CREATE UNIQUE INDEX AE_PROFUPD_LOYID    ON Ae_Profileupdates2 (LOYALTYNUMBER) online  COMPUTE STATISTICS';
      -- AEO-1440 begin (addingend )

    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    END;
-- AEO-1440 BEGIN
DECLARE

  V_TBL_ENR T_TAB_ENR;
  V_ROW_VALID NUMBER(1) := 1;
  v_ipcode lw_loyaltymember.ipcode%TYPE;
  v_membertier_id lw_membertiers.id%TYPE;
  v_membertier_tierid lw_membertiers.tierid%TYPE;
  v_vckey  lw_virtualcard.vckey%TYPE;
  v_year NUMBER := 0;
  v_exist NUMBER := 0;
  v_ipointtypeid NUMBER := -1;
  v_ipointeventid NUMBER := -1;
  v_issueAEOSignUpbonus  BOOLEAN := FALSE;
  v_issueAECCSignUpbonus BOOLEAN := FALSE;
  v_issueSignUpJan2018  BOOLEAN := FALSE; -- AEO-2109
  v_frozen BOOLEAN := FALSE;
  v_status_member NUMBER := 1;
  agent_numer number(20); --AEO-2215
  v_issueSignUpMarchCanStore2018  BOOLEAN := FALSE; 
  v_iscanada NUMBER := 0;
  
  

BEGIN

	utility_pkg.Log_Process_Start(v_jobname, 'Create new records', v_processId);
  v_messagesreceived := 0;
   SELECT COUNT(*)
   INTO   v_exist
   FROM   Lw_Tiers mt
   WHERE  mt.tiername = 'Full Access';

    --GET ID FROM USERNAME SYSTEM
   SELECT ca.id
   INTO agent_numer
   from bp_ae.lw_csagent ca
   where lower(ca.username) like '%system%'
   or lower(ca.username) like '%sys_system%';

   IF (v_exist <> 0) THEN

      SELECT mt.tierid
      INTO   v_Membertier_Tierid
      FROM   Lw_Tiers mt
      WHERE  mt.tiername = 'Full Access';

      OPEN GET_ENROLLMENTS;
      LOOP
         FETCH GET_ENROLLMENTS BULK COLLECT INTO V_TBL_ENR LIMIT 100;

               -- USE FORALL  to update each table
         FOR I IN 1 .. V_TBL_ENR.COUNT LOOP

           v_exist := 0;
           v_issueAEOSignUpbonus   := FALSE;
           v_issueAECCSignUpbonus  := FALSE;
           v_issueSignUpJan2018    :=  FALSE; --AEO-2109
           v_issueSignUpMarchCanStore2018 :=FALSE; --AEO-2215
           v_iscanada := 0 ; --AEO-22215
         -- validate each row and insert errors


           v_row_valid := validate_row_enrollment(i, V_TBL_ENR, v_flags, p_filename);
           IF (v_row_valid <> -1) THEN

                  -- AEO-1820 begin
                  IF ( to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY') >= to_date('10/1/2017','MM/DD/YYYY') AND
                       to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY') < to_date ('10/11/2017','MM/DD/YYYY') ) THEN

                     IF ( (v_flags(c_AECCStatus) = 1 AND
                          V_TBL_ENR(i).aeccstatus IN (1,3)) OR ( v_flags(c_AEVisaStatus) = 1 AND
                          V_TBL_ENR(i).aevisastatus IN (1,3)) ) THEN

                        v_issueAECCSignUpBonus := TRUE;

                     END IF;
                  END IF;

                  -- AEO-1820 end

                   -- AEO-1788 begin
                    IF ( Extract ( YEAR FROM  to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY')) = 2017) AND
                          NOT ( v_issueAECCSignUpBonus) THEN -- AEO-1820 begin-end
                      v_issueAEOSignupBonus := TRUE;

                    END IF;
                   -- AEO-1788 end

                   -- AEO-2109 begin

                  IF ( to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY') >= to_date('1/25/2018','MM/DD/YYYY') AND
                       to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY') < to_date ('2/1/2018','MM/DD/YYYY') ) THEN
                     v_issueSignUpJan2018 := TRUE;
                  END IF;
                   -- AEO-2109 end
                   --AEO-2215
                    IF ( to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY') >= to_date('03/14/2018','MM/DD/YYYY') AND
                       to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY') <= to_date ('03/28/2018','MM/DD/YYYY') ) THEN
                        --CHECK IF IS A CANADIAN STORE 
                        IF( v_flags(c_StoreNumber) =1) THEN
                        
                              -- CHECK IF EXIST IN DATABASE AND IS CANADA STORE
                              SELECT count(*)
                              INTO v_iscanada
                              from bp_ae.lw_storedef d
                              where d.storenumber = v_Tbl_Enr(i).Storenumber
                              and lower(d.country) = 'can';
                              
                           IF(v_iscanada > 0) THEN
                                IF(v_flags(v_Tbl_Enr(i).ENROLLMENTSOURCE) =1) THEN
                                    --ENROLLMENT SOURCE VALID 24,25,26,27
                                    IF (v_Tbl_Enr(i).ENROLLMENTSOURCE IN (24,25,26,27) ) THEN
                                       v_issueSignUpMarchCanStore2018 :=TRUE;
                                    END IF;
                                END IF;
                            END IF;
                         END IF;
                      END IF;
                   --AEO-2215 END

              -- AEO-1820 begin
              v_year :=  extract (YEAR FROM to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY'));
              -- First add a row in loyalty member
              v_ipcode := Ipcode_Seq.Nextval;

           -- AEO-2054 Begin, FROZEN MEMBER THAR ARE UNDERAGE  > 0 AND < 15
             V_REASON := 'Create new record on Lw_Loyaltymember';
             v_frozen := False;
             v_status_member := 1;

             IF (v_flags(c_BirthDate) = 1) AND
             (
             
              (( to_date(to_char(sysdate,'MMDDYYYY'),'MMDDYYYY') -  to_date(v_Tbl_Enr(i).BirthDate,'MMDDYYYY')) /   365.242199) >= 1 AND
              (( to_date(to_char(sysdate,'MMDDYYYY'),'MMDDYYYY') -  to_date(v_Tbl_Enr(i).BirthDate, 'MMDDYYYY')) /   365.242199) < 15
              /*
               ((sysdate - to_date(v_Tbl_Enr(i).Birthdate, 'MMDDYYYY')) /365.242199) > 1 and
               ((sysdate - to_date(v_Tbl_Enr(i).Birthdate, 'MMDDYYYY')) /365.242199) < 15*/
             ) THEN
                 v_frozen := TRUE;
                 v_status_member :=4;
             END IF;

              INSERT INTO Lw_Loyaltymember
                   (Ipcode,
                    Firstname,
                    Lastname,
                    Birthdate,
                    Membercreatedate,
                    Memberstatus,
                    createdate)
              VALUES
                   (v_ipcode,
                    v_Tbl_Enr(i).Firstname ,
                    v_Tbl_Enr(i).Lastname,
                    CASE WHEN v_flags(c_BirthDate)=0 THEN
                      NULL
                    ELSE
                       to_date(v_Tbl_Enr(i).Birthdate,'MMDDYYYY')
                    END   ,
                    to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY'),
                    v_status_member,
                    SYSDATE);

                   --ADD ROW IN CSNOTE, IF ITS FROZEN , THEN INSERT THE NOTE
                 IF (v_frozen) THEN
                   INSERT INTO bp_ae.lw_csnote
                  (id, memberid, note, createdby, createdate, deleted, last_dml_id)
                   VALUES
                  (seq_csnote.nextval,
                   v_ipcode,
                   'Member has been frozen due to age restrictions. Member must be 15 years or older to participate in the AEO Connected Program',
                   agent_numer,
                   SYSDATE,
                   0,
                   LAST_DML_ID#.NEXTVAL);
                 END IF;
           --AEO-2054 End

           -- add row in membertier
            V_REASON := 'Create new record on Lw_Membertiers';
            v_Membertier_id := hibernate_sequence.nextval;
            INSERT INTO Lw_Membertiers
                  (Id,
                   Tierid,
                   Memberid,
                   Fromdate,
                   Todate,
                   Description,
                   Createdate,
                   Updatedate)
            VALUES
                  (v_Membertier_Id,
                   v_Membertier_Tierid,
                   v_Ipcode,
                   SYSDATE,
                   To_Date('1/1/' || To_Char(v_Year+2), 'mm/dd/yyyy'),
                   'Base',
                   SYSDATE,
                   SYSDATE);

               -- add membernetspend
            V_MESSAGE := 'Create new record on Ats_Membernetspend';
            INSERT INTO Ats_Membernetspend
                    (a_Ipcode,
                     a_Rowkey,
                     a_Membertierid,
                     a_Netspend,
                     Createdate,
                     Updatedate)
            VALUES
                    (v_Ipcode,
                     Seq_Rowkey.Nextval,
                     v_Membertier_Id,
                     0,
                     SYSDATE,
                     SYSDATE);


               -- add virtualcard
            V_REASON := 'Create new record on Lw_Virtualcard';
            v_vckey := Seq_Virtualcard.Nextval;
            INSERT INTO Lw_Virtualcard
                    (Ipcode,
                     Vckey,
                     Loyaltyidnumber,
                     Linkkey,
                     Dateissued,
                     Dateregistered,
                     Status,
                     Isprimary,
                     Cardtype,
                     Createdate,
                     Updatedate)
            VALUES
                    (v_Ipcode,
                     v_vckey,
                     v_Tbl_Enr(i).Loyaltynumber,
                     v_Tbl_Enr(i).Cid,
                     to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY'),
                     to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY'),
                     1,
                     1,
                     0,
                     SYSDATE,
                     SYSDATE);


            -- add synchronyaccountkey
            IF NOT (v_tbl_enr(i).AEVISAACCOUNTKEY  IS NULL) THEN
              V_REASON := 'Create new record on Ats_Synchronyaccountkey';
              INSERT INTO Ats_Synchronyaccountkey
                    (a_Rowkey,
                     a_Vckey,
                     a_Parentrowkey,
                     a_Accountkey,
                     Statuscode,
                     Createdate,
                     Updatedate)
            VALUES
                    (Seq_Rowkey.Nextval,
                     v_Vckey,
                     v_Vckey,
                      CASE WHEN ( v_flags(c_AEVisaAccountKey) =1) THEN  TRIM ( LEADING '0' FROM v_Tbl_Enr(i).AEVisaAccountKey )
                     ELSE NULL
                     END,
                     0,
                     SYSDATE,
                     SYSDATE);
            END IF;




               -- add memberdetails
            V_REASON := 'Create new record on Ats_Memberdetails';
            INSERT INTO Ats_Memberdetails
                    (a_Rowkey,
                     a_Ipcode,
                     a_Addresslineone,
                     a_Addresslinetwo,
                     a_City,
                     a_Stateorprovince,
                     a_Ziporpostalcode,
                     a_Country,
                     a_Mobilephone,
                     a_Gender,
                     a_Emailaddress,
                     a_Emailaddressmailable,
                     a_Smsoptin,
                     a_Smsoptindate,
                     a_Employeecode,
                     a_Homestoreid,
                     a_Languagepreference,
                     a_Membersource,
                     a_Enrollmentid,
                     a_Cardtype,
                     a_Cardopendate,
                     a_Cardclosedate,
                     a_Extendedplaycode,
                     a_Changedby,
                     a_Isunderage,
                     a_addressmailable,
                     a_aitupdate
                     )
            VALUES
                    (Seq_Rowkey.Nextval,
                     v_Ipcode,
                     v_Tbl_Enr(i).Addressline1,
                     v_Tbl_Enr(i).Addressline2 ,
                     v_Tbl_Enr(i).City,
                     CASE WHEN v_flags(c_LanguagePreference)= 0 THEN NULL
                       ELSE
                         v_Tbl_Enr(i).State
                         END ,
                     CASE WHEN v_flags(c_Zip) = 1 THEN v_Tbl_Enr(i).Zip
                     ELSE NULL
                       END  ,
                     CASE WHEN v_flags(c_country) = 1 THEN v_Tbl_Enr(i).Country
                     ELSE NULL
                     END ,
                     v_Tbl_Enr(i).MOBILENUMBER,
                     v_Tbl_Enr(i).Gender,
                     v_Tbl_Enr(i).Emailaddress,
                     CASE WHEN v_flags(c_ValidEmailAddress)= 1 THEN v_Tbl_Enr(i).Validemailaddress
                     ELSE NULL
                       END,
                     CASE WHEN v_flags(c_CellDoubleOptIn) =1  THEN v_Tbl_Enr(i).Celldoubleoptin
                      ELSE NULL
                      END,
                     CASE WHEN v_flags(c_CellDoubleOptIn) =1  THEN
                        to_date(v_Tbl_Enr(i).Enrollmentdate,'MMDDYYYY')
                      ELSE NULL END,
                     v_Tbl_Enr(i).Employeestatus,

                     CASE WHEN  v_flags(c_StoreNumber) =1 THEN  v_Tbl_Enr(i).Storenumber
                       ELSE NULL
                       END,

                     CASE WHEN v_flags(c_LanguagePreference)= 0 THEN NULL
                     ELSE  v_Tbl_Enr(i).Languagepreference END,

                     v_Tbl_Enr(i).ENROLLMENTSOURCE,
                     v_Tbl_Enr(i).Enrollmentid,
                     CASE  WHEN ( v_flags(c_Aeccstatus)= 0 OR v_flags(c_AEVisaStatus) = 0) THEN
                       0
                      WHEN v_Tbl_Enr(i).Aeccstatus = 1 AND v_Tbl_Enr(i).Aevisastatus IS NULL THEN
                       1
                      WHEN v_Tbl_Enr(i).Aeccstatus IS NULL AND v_Tbl_Enr(i).Aevisastatus = 1 THEN
                        2
                      WHEN v_Tbl_Enr(i).Aeccstatus = 1 AND v_Tbl_Enr(i).Aevisastatus = 1 THEN 3
                        ELSE 0 END,
                     CASE WHEN ( v_flags(c_AECCOpenDate)= 0 OR v_flags(c_AEVisaOpenDate) = 0) THEN
                        NULL
                     WHEN Nvl(to_date(v_Tbl_Enr(i).Aeccopendate,'MMDDYYYY'), To_Date('1/1/1900', 'mm/dd/yyyy')) >    Nvl(to_Date(v_Tbl_Enr(i).Aevisaopendate,'MMDDYYYY'), To_Date('1/1/1900', 'mm/dd/yyyy')) THEN
                            to_date(v_Tbl_Enr(i).Aeccopendate,'MMDDYYYY')
                     ELSE to_Date(v_Tbl_Enr(i).Aevisaopendate,'MMDDYYYY') END,
                     CASE WHEN ( v_flags(c_AECCCloseDate)= 0 OR v_flags(c_AEVisaCloseDate)= 0 ) THEN
                       NULL
                      WHEN Nvl(to_date(v_Tbl_Enr(i).Aeccclosedate,'MMDDYYYY'),To_Date('1/1/1900', 'mm/dd/yyyy')) > Nvl(to_date(replace(replace(v_Tbl_Enr(i).Aevisaclosedate,CHR(10)),chr(13)),'MMDDYYYY'),To_Date('1/1/1900', 'mm/dd/yyyy')) THEN
                            nvl2( v_Tbl_Enr(i).Aeccclosedate, to_date(v_Tbl_Enr(i).Aeccclosedate,'MMDDYYYY'), NULL)
                     ELSE
                         nvl2( replace(replace(v_Tbl_Enr(i).Aeccclosedate,CHR(10)),(CHR(13))) ,to_date(replace(replace(v_Tbl_Enr(i).Aeccclosedate,CHR(10)),(CHR(13))),'MMDDYYYY'), NULL)
                     END,
                     0,
                     'Enrollment: Profile In',
                     CASE WHEN v_flags(c_BirthDate) = 0 THEN 0
                     WHEN  v_status_member = 4 THEN 1 ELSE 0 END,
                     v_Tbl_Enr(i).VALIDADDRESS,
                     1);

               -- update ae_profileupdtes
               UPDATE ae_profileupdates2 pu2
                  SET pu2.isenrollment = 1
               WHERE pu2.loyaltynumber = v_tbl_enr(i).loyaltynumber;



               -- aeo-2109 begin
               -- add 500 signup bonus for new enrollments from 1/25/2018 upto 1/31/2018
               -- check no other vckey for this ipcode has alreary recieved the bonus
               --
               V_REASON := 'Create new bonus';
               IF ( v_issueSignUpJan2018 ) THEN

                     --- check that no other vckey for this ipcoade already
                     --- has received the bonus
                     ---
                     SELECT COUNT(*)
                     INTO v_exist
                     FROM bp_Ae.Lw_Pointtransaction pt
                     INNER JOIN bp_Ae.Lw_Pointtype   pty ON pty.pointtypeid = pt.pointtypeid
                     INNER JOIN bp_Ae.Lw_Pointevent  pte ON pte.pointeventid = pt.pointtypeid
                     INNER JOIN ( SELECT vc.vckey FROM bp_Ae.Lw_Virtualcard vc WHERE vc.ipcode =  v_ipcode) vc2 ON vc2.vckey = pt.vckey
                     WHERE pty.name  ='AEO Connected Bonus Points' AND
                           pte.name = 'SPRING: Loyalty New Sign Up Offer: 1/25/18-1/31/18' ;

                     IF (v_exist > 0 ) THEN
                         v_issueSignUpJan2018 := FALSE;
                     END IF;

               END IF;
               IF (v_issueSignUpJan2018) THEN

                   SELECT Pointtypeid
                   INTO   v_exist
                   FROM   Lw_Pointtype
                   WHERE  NAME = 'AEO Connected Bonus Points';

                   IF (v_exist > 0 ) THEN
                       SELECT Pointtypeid
                       INTO   v_ipointtypeid
                       FROM   Lw_Pointtype
                       WHERE  NAME = 'AEO Connected Bonus Points';

                        SELECT Pointeventid
                        INTO   v_exist
                        FROM   Lw_Pointevent
                        WHERE  NAME = 'SPRING: Loyalty New Sign Up Offer: 1/25/18-1/31/18';

                        IF (v_exist > 0 ) THEN

                          SELECT Pointeventid
                          INTO   v_ipointeventid
                          FROM   Lw_Pointevent
                          WHERE  NAME ='SPRING: Loyalty New Sign Up Offer: 1/25/18-1/31/18';

                          INSERT INTO Lw_Pointtransaction
                               (Pointtransactionid,
                                Vckey,
                                Pointtypeid,
                                Pointeventid,
                                Transactiontype,
                                Transactiondate,
                                Pointawarddate,
                                Points,
                                Expirationdate,
                                Notes,
                                Ownertype,
                                Ownerid,
                                Rowkey,
                                Parenttransactionid,
                                Pointsconsumed,
                                Pointsonhold,
                                Ptlocationid,
                                Ptchangedby,
                                Createdate,
                                Expirationreason)
                          VALUES
                               (seq_pointtransactionid.nextval,
                                v_vckey,
                                v_iPointtypeid,
                                v_iPointeventid,
                                1 /*purchase*/,
                                SYSDATE,
                                SYSDATE,
                                500,
                                To_Date('12/31/2199', 'mm/dd/yyyy'),
                                'Join AEO Connected and get 500 points!',
                                -1,
                                -1,
                                -1,
                                -1,
                                0 /*Pointsconsumed*/,
                                0,
                                NULL,
                                'Profile-in-Enrollment' /*Ptchangedby*/,
                                SYSDATE,
                                NULL);
                          END IF;


                   END IF ;

               END IF;

               -- aeo-2109 end
               -- aeo-1788 begin
               -- add 500 wellcome bonus for new enrollments upto 31dec2017
               -- check no other vckey for this ipcode has alreary recieved the bonus
               --
               IF ( v_issueAEOSignUpbonus ) THEN  -- is this in in 2017

                     --- check that no other vckey for this ipcoade already
                     --- has received the bonus
                     ---
                     SELECT COUNT(*)
                     INTO v_exist
                     FROM bp_Ae.Lw_Pointtransaction pt
                     INNER JOIN bp_Ae.Lw_Pointtype   pty ON pty.pointtypeid = pt.pointtypeid
                     INNER JOIN bp_Ae.Lw_Pointevent  pte ON pte.pointeventid = pt.pointtypeid
                     INNER JOIN ( SELECT vc.vckey FROM bp_Ae.Lw_Virtualcard vc WHERE vc.ipcode =  v_ipcode) vc2 ON vc2.vckey = pt.vckey
                     WHERE pty.name  ='AEO Connected Bonus Points' AND
                           pte.name = 'AEO Connected Welcome Bonus' ;

                     IF (v_exist > 0 ) THEN
                         v_issueAEOSignUpbonus := FALSE;
                     END IF;

               END IF;
               IF (v_issueAEOSignUpbonus) THEN

                   SELECT Pointtypeid
                   INTO   v_exist
                   FROM   Lw_Pointtype
                   WHERE  NAME = 'AEO Connected Bonus Points';

                   IF (v_exist > 0 ) THEN
                       SELECT Pointtypeid
                       INTO   v_ipointtypeid
                       FROM   Lw_Pointtype
                       WHERE  NAME = 'AEO Connected Bonus Points';

                        SELECT Pointeventid
                        INTO   v_exist
                        FROM   Lw_Pointevent
                        WHERE  NAME = 'AEO Connected Welcome Bonus';

                        IF (v_exist > 0 ) THEN

                          SELECT Pointeventid
                          INTO   v_ipointeventid
                          FROM   Lw_Pointevent
                          WHERE  NAME ='AEO Connected Welcome Bonus';

                          INSERT INTO Lw_Pointtransaction
                               (Pointtransactionid,
                                Vckey,
                                Pointtypeid,
                                Pointeventid,
                                Transactiontype,
                                Transactiondate,
                                Pointawarddate,
                                Points,
                                Expirationdate,
                                Notes,
                                Ownertype,
                                Ownerid,
                                Rowkey,
                                Parenttransactionid,
                                Pointsconsumed,
                                Pointsonhold,
                                Ptlocationid,
                                Ptchangedby,
                                Createdate,
                                Expirationreason)
                          VALUES
                               (seq_pointtransactionid.nextval,
                                v_vckey,
                                v_iPointtypeid,
                                v_iPointeventid,
                                1 /*purchase*/,
                                SYSDATE,
                                SYSDATE,
                                500,
                                To_Date('12/31/2199', 'mm/dd/yyyy'),
                                'AEO Connected Welcome Bonus',
                                -1,
                                -1,
                                -1,
                                -1,
                                0 /*Pointsconsumed*/,
                                0,
                                NULL,
                                'Profile-in-Enrollment' /*Ptchangedby*/,
                                SYSDATE,
                                NULL);
                          END IF;


                   END IF ;

               END IF;

               -- aeo-1788 end

               IF ( v_issueAECCSignUpbonus ) THEN

                     --- check that no other vckey for this ipcoade already
                     --- has received the bonus
                     ---
                     SELECT COUNT(*)
                     INTO v_exist
                     FROM bp_Ae.Lw_Pointtransaction pt
                     INNER JOIN bp_Ae.Lw_Pointtype   pty ON pty.pointtypeid = pt.pointtypeid
                     INNER JOIN bp_Ae.Lw_Pointevent  pte ON pte.pointeventid = pt.pointtypeid
                     INNER JOIN ( SELECT vc.vckey FROM bp_Ae.Lw_Virtualcard vc WHERE vc.ipcode =  v_ipcode) vc2 ON vc2.vckey = pt.vckey
                     WHERE    pty.name  ='AEO Connected Bonus Points' AND
                              pte.name = 'AECC Sign Up Bonus' ;

                     IF (v_exist > 0 ) THEN
                         v_issueAECCSignUpbonus := FALSE;
                     END IF;

               END IF;
               IF (v_issueAECCSignUpbonus) THEN

                   SELECT Pointtypeid
                   INTO   v_exist
                   FROM   Lw_Pointtype
                   WHERE  NAME = 'AEO Connected Bonus Points';

                   IF (v_exist > 0 ) THEN
                       SELECT Pointtypeid
                       INTO   v_ipointtypeid
                       FROM   Lw_Pointtype
                       WHERE  NAME = 'AEO Connected Bonus Points';

                        SELECT Pointeventid
                        INTO   v_exist
                        FROM   Lw_Pointevent
                        WHERE  NAME = 'AECC Sign Up Bonus';

                        IF (v_exist > 0 ) THEN

                          SELECT Pointeventid
                          INTO   v_ipointeventid
                          FROM   Lw_Pointevent
                          WHERE  NAME ='AECC Sign Up Bonus';

                          INSERT INTO Lw_Pointtransaction
                               (Pointtransactionid,
                                Vckey,
                                Pointtypeid,
                                Pointeventid,
                                Transactiontype,
                                Transactiondate,
                                Pointawarddate,
                                Points,
                                Expirationdate,
                                Notes,
                                Ownertype,
                                Ownerid,
                                Rowkey,
                                Parenttransactionid,
                                Pointsconsumed,
                                Pointsonhold,
                                Ptlocationid,
                                Ptchangedby,
                                Createdate,
                                Expirationreason)
                          VALUES
                               (seq_pointtransactionid.nextval,
                                v_vckey,
                                v_iPointtypeid,
                                v_iPointeventid,
                                1 /*purchase*/,
                                SYSDATE,
                                SYSDATE,
                                2500,
                                To_Date('12/31/2199', 'mm/dd/yyyy'),
                                '2500 Bonus Points for signing up for AECC',
                                -1,
                                -1,
                                -1,
                                -1,
                                0 /*Pointsconsumed*/,
                                0,
                                NULL,
                                'Profile-in-Enrollment' /*Ptchangedby*/,
                                SYSDATE,
                                NULL);
                          END IF;


                   END IF ;

               END IF;
               --aeo 2215
               IF ( v_issueSignUpMarchCanStore2018 ) THEN
                     --- check that no other vckey for this ipcoade already
                     --- has received the bonus
                     ---
                     SELECT COUNT(*)
                     INTO v_exist
                     FROM bp_Ae.Lw_Pointtransaction pt
                     INNER JOIN bp_Ae.Lw_Pointtype   pty ON pty.pointtypeid = pt.pointtypeid
                     INNER JOIN bp_Ae.Lw_Pointevent  pte ON pte.pointeventid = pt.pointeventid
                     INNER JOIN ( SELECT vc.vckey FROM bp_Ae.Lw_Virtualcard vc WHERE vc.ipcode =  v_ipcode) vc2 ON vc2.vckey = pt.vckey
                     WHERE pty.name  ='AEO Connected Bonus Points' AND
                           pte.name = 'SPRING: Loyalty New Sign Up Offer: 3/14/18-3/28/18' ;
                     IF (v_exist > 0 ) THEN
                         v_issueSignUpMarchCanStore2018 := FALSE;
                     END IF;
                 
                  END IF;
               IF (v_issueSignUpMarchCanStore2018) THEN

                   SELECT Pointtypeid
                   INTO   v_exist
                   FROM   Lw_Pointtype
                   WHERE  NAME = 'AEO Connected Bonus Points';

                   IF (v_exist > 0 ) THEN
                       SELECT Pointtypeid
                       INTO   v_ipointtypeid
                       FROM   Lw_Pointtype
                       WHERE  NAME = 'AEO Connected Bonus Points';

                        SELECT Pointeventid
                        INTO   v_exist
                        FROM   Lw_Pointevent
                        WHERE  NAME = 'SPRING: Loyalty New Sign Up Offer: 3/14/18-3/28/18';

                        IF (v_exist > 0 ) THEN

                          SELECT Pointeventid
                          INTO   v_ipointeventid
                          FROM   Lw_Pointevent
                          WHERE  NAME ='SPRING: Loyalty New Sign Up Offer: 3/14/18-3/28/18';

                          INSERT INTO Lw_Pointtransaction
                               (Pointtransactionid,
                                Vckey,
                                Pointtypeid,
                                Pointeventid,
                                Transactiontype,
                                Transactiondate,
                                Pointawarddate,
                                Points,
                                Expirationdate,
                                Notes,
                                Ownertype,
                                Ownerid,
                                Rowkey,
                                Parenttransactionid,
                                Pointsconsumed,
                                Pointsonhold,
                                Ptlocationid,
                                Ptchangedby,
                                Createdate,
                                Expirationreason)
                          VALUES
                               (seq_pointtransactionid.nextval,
                                v_vckey,
                                v_iPointtypeid,
                                v_iPointeventid,
                                1 /*purchase*/,
                                SYSDATE,
                                SYSDATE,
                                500,
                                To_Date('12/31/2199', 'mm/dd/yyyy'),
                                'Join AEO Connected and get 500 points! (Canada stores only)',
                                -1,
                                -1,
                                -1,
                                -1,
                                0 /*Pointsconsumed*/,
                                0,
                                NULL,
                                'Profile-in-Enrollment' /*Ptchangedby*/,
                                SYSDATE,
                                NULL);
                          END IF;


                   END IF ;

               END IF;
           END IF;

         END LOOP ;

         COMMIT WRITE BATCH NOWAIT;
         EXIT WHEN GET_ENROLLMENTS%NOTFOUND;
      END LOOP;

      COMMIT;

      IF GET_ENROLLMENTS%ISOPEN THEN
             CLOSE GET_ENROLLMENTS;
      END IF;

   END IF;

  utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

  EXCEPTION
    WHEN OTHERS THEN
         V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
         v_endtime := SYSDATE;

         utility_pkg.Log_job(P_JOB                => v_my_log_id
         ,p_jobdirection       => v_jobdirection
         ,p_filename           => null
         ,p_starttime          => v_starttime
         ,p_endtime            => v_endtime
         ,p_messagesreceived   => v_messagesreceived
         ,p_messagesfailed     => v_messagesfailed
         ,p_jobstatus          => v_jobstatus
         ,p_jobname            => v_jobname);

      V_ERROR   := SQLERRM;
      V_MESSAGE := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                   '    <pkg>STAGE_PROFILEUPDATES_DAILY</pkg>' || CHR(10) ||
                   '    <proc>STAGE_PROFILEUPDATES02</proc>' || CHR(10) ||
                   '  </details>' || CHR(10) || '</failed>';

      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => 'STAGE_PROFILEUPDATES02',
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE_APPLICATION_ERROR(-20002,'Other Exception detected in STAGE_PROFILEUPDATES02');
END;
-- AEO-1440 END



DECLARE

  V_TBL_UPD T_TAB_UPD;
  V_ROW_VALID NUMBER(1) := 1;


BEGIN
  utility_pkg.Log_Process_Start(v_jobname, 'Validate update records', v_processId);
  v_messagesreceived := 0;
  V_REASON := 'Validate update records';
  OPEN GET_UPDATES;
  LOOP
     FETCH GET_UPDATES BULK COLLECT INTO V_TBL_UPD LIMIT 100;


     FOR I IN 1 .. v_TBL_UPD.COUNT LOOP

         v_row_valid := validate_row_update(i, V_TBL_UPD, v_flags, p_filename);

         IF (v_row_valid = 0 AND v_flags(c_LoyaltyNumber)= 1) THEN -- aeo-2007 begin - end

            UPDATE bp_ae.ae_profileupdates2 pu2
                   SET pu2.firstname = CASE WHEN v_flags(c_FirstName)= 1 THEN V_TBL_UPD(i).firstname
                                       ELSE NULL
                                       END,
                       pu2.lastname  =  CASE WHEN v_flags(c_lastname) = 1 THEN V_TBL_UPD(i).lastname
                                        ELSE NULL
                                        END,
                       pu2.addressline1 =  CASE WHEN v_flags(c_Line1) = 1 THEN V_TBL_UPD(i).addressline1
                                           ELSE NULL
                                           END,
                       pu2.addressline2 = CASE WHEN v_flags(c_Line2) = 1 THEN V_TBL_UPD(i).addressline2
                                          ELSE NULL
                                          END,
                       pu2.city         = CASE WHEN v_flags(c_City) = 1 THEN V_TBL_UPD(i).city
                                          ELSE NULL
                                          END,
                       pu2.state        = CASE WHEN v_flags(c_state) =1 THEN v_tbl_upd(i).state
                                          ELSE NULL
                                          END,
                       pu2.zip           =  CASE WHEN v_flags(c_zip) =1 THEN v_tbl_upd(i).zip
                                            ELSE NULL
                                            END,
                       pu2.country       = CASE WHEN v_flags(c_country) = 1 THEN v_tbl_upd(i).country
                                           ELSE NULL
                                           END,
                       pu2.birthdate     = CASE WHEN v_flags(c_BirthDate) = 1 THEN to_date(v_tbl_upd(i).birthdate,'MMDDYYYY')
                                           ELSE NULL
                                           END ,
                       pu2.emailaddress   =  CASE WHEN v_flags(c_EmailAddress) = 1 THEN  v_tbl_upd(i).emailaddress
                                             ELSE NULL
                                             END ,
                       pu2.validaddress  = CASE WHEN v_flags(c_ValidAddress) = 1 THEN v_tbl_upd(i).validaddress
                                           ELSE NULL
                                           END,
                       pu2.recordtype    = CASE WHEN v_flags(c_RecordType) = 1 THEN v_tbl_upd(i).recordtype
                                           ELSE 'S'
                                           END,
                       pu2.validemailaddress =  CASE WHEN v_flags(c_ValidEmailAddress) = 1 THEN v_tbl_upd(i).validemailaddress
                                                ELSE NULL
                                                END,
                       pu2.celldoubleoptin = CASE WHEN v_flags(c_CellDoubleOptIn) = 1 THEN v_tbl_upd(i).celldoubleoptin
                                                ELSE NULL
                                                END ,

                       pu2.storenumber = CASE WHEN v_flags(c_storenumber) = 1 THEN v_tbl_upd(i).storenumber
                                                ELSE NULL
                                                END ,

                       pu2.languagepreference = CASE WHEN v_flags(c_LanguagePreference) = 1 THEN v_tbl_upd(i).languagepreference
                                                ELSE NULL
                                                END,
                       pu2.aeccstatus = CASE WHEN v_flags(c_AECCStatus) = 1 THEN v_tbl_upd(i).aeccstatus
                                        ELSE NULL
                                        END,
                       pu2.aeccopendate = CASE WHEN v_flags(c_AECCOpenDate) = 1 THEN to_date(v_tbl_upd(i).aeccopendate,'MMDDYYYY')
                                          ELSE NULL
                                          END,
                       pu2.aeccclosedate = CASE WHEN v_flags(c_AECcclosedate) = 1 THEN to_date(v_tbl_upd(i).aeccclosedate,'MMDDYYYY')
                                           ELSE NULL
                                           END,

                       pu2.aevisastatus = CASE WHEN v_flags(c_AEvisastatus) = 1 THEN v_tbl_upd(i).aevisastatus
                                          ELSE NULL
                                          END,
                       pu2.aevisaopendate = CASE WHEN v_flags(c_aevisaopendate) = 1 THEN to_date(v_tbl_upd(i).aevisaopendate,'MMDDYYYY')
                                            ELSE NULL
                                            END,

                       pu2.aevisaclosedate =  CASE WHEN v_flags(c_aevisaclosedate) = 1 THEN to_date(v_tbl_upd(i).aevisaclosedate,'MMDDYYYY')
                                            ELSE NULL
                                            END,
                       pu2.aevisaaccountkey = CASE WHEN v_flags(c_AEVisaAccountKey) = 1 THEN  TRIM ( LEADING '0' FROM v_tbl_upd(i).aevisaaccountkey)
                                              ELSE NULL
                                              END,
                       pu2.enrollmentdate = CASE WHEN v_flags(c_EnrollmentDate) = 1 THEN  pu2.enrollmentdate
                                              ELSE NULL
                                              END

            WHERE pu2.loyaltynumber = v_tbl_upd(i).loyaltynumber;

         END IF;


     END LOOP ;

     COMMIT WRITE BATCH NOWAIT;
     EXIT WHEN GET_UPDATES%NOTFOUND;
  END LOOP;

  COMMIT;

  IF GET_UPDATES%ISOPEN THEN
         CLOSE GET_UPDATES;
  END IF;



  utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
END;





DECLARE
  CURSOR GET_DATA4 IS
        SELECT /*+ hash(vc,lm)*/
         Vc.Loyaltyidnumber AS Loyaltynumber,
          PU.BIRTHDATE AS Birthdatefi, --AEO-1440
        -- To_Date(Pu.Birthdate, 'MMDDYYYY') AS Birthdatefi,
         Lm.Birthdate AS Birthdatedb
        FROM   Bp_Ae.Lw_Virtualcard Vc
        INNER  JOIN Bp_Ae.Ae_Profileupdates2 Pu
        ON     Pu.Loyaltynumber = Vc.Loyaltyidnumber
        INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm
        ON     Lm.Ipcode = Vc.Ipcode
        WHERE  (Pu.Recordtype = 'P' AND
               Pu.Birthdate <> Lm.Birthdate); --AEO-1440

  TYPE T_TAB4 IS TABLE OF GET_DATA4%ROWTYPE;
  V_TBL4 T_TAB4;

BEGIN

  utility_pkg.Log_Process_Start(v_jobname, 'Write tmpvcbirthdate', v_processId);
  V_REASON := 'Write tmpvcbirthdate';
  v_messagesreceived := 0;

  OPEN GET_DATA4;
  LOOP
     FETCH GET_DATA4 BULK COLLECT INTO V_TBL4 LIMIT 100;

           -- USE FORALL  to update each table
     FORALL I IN 1 .. V_TBL4.COUNT
                INSERT INTO Ae_Tmpvcbirthdate
                     (Loyaltynumber, Birthdatedb, Birthdatefi)
                VALUES
                     (v_Tbl4(i).Loyaltynumber,
                      v_Tbl4(i).Birthdatedb,
                      v_Tbl4(i).Birthdatefi);

     COMMIT WRITE BATCH NOWAIT;
     EXIT WHEN GET_DATA4%NOTFOUND;
  END LOOP;
  COMMIT;
  IF GET_DATA4%ISOPEN THEN
         CLOSE GET_DATA4;
  END IF;

  utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
END;

DECLARE
 CURSOR GET_DATA7 IS
  SELECT *
  FROM   (SELECT p.Cid,
                 p.Loyaltynumber,
                 p.Recordtype,
               --  p.Updatedate, AEO-1440
                 Vc.Ipcode,
                 'S' Newrecordtype,
                 Row_Number() Over(PARTITION BY p.cid ORDER BY Vc.Dateissued DESC) r -- changed partition over cid to ipcode ?
          FROM   Bp_Ae.Ae_Profileupdates2 p
          INNER  JOIN Bp_Ae.Lw_Virtualcard Vc
          ON     Vc.Loyaltyidnumber = p.Loyaltynumber
          WHERE  1 = 1
                 AND p.Recordtype = 'P')
  WHERE  r > 1;

   TYPE T_TAB7 IS TABLE OF GET_DATA7%ROWTYPE;
   V_TBL7 T_TAB7;

BEGIN
      utility_pkg.Log_Process_Start(v_jobname, 'Remove duplictae primary for the same ipcode but different CID', v_processId);
      V_REASON := 'Remove duplictae primary for the same ipcode but different CID';
      v_messagesreceived := 0;
      OPEN GET_DATA7;
      LOOP

        FETCH GET_DATA7 BULK COLLECT
          INTO V_TBL7 LIMIT 100;

        FORALL I IN 1 .. V_TBL7.COUNT
          INSERT INTO Bp_Ae.Ae_Hubsynchelpertbl
               (Loyaltynumber, Newrecordtype)
          VALUES
               (v_Tbl7(i).Loyaltynumber, v_Tbl7(i).Newrecordtype);

         COMMIT WRITE BATCH NOWAIT;

         EXIT WHEN GET_DATA7%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA7%ISOPEN THEN
         CLOSE GET_DATA7;
      END IF;

      utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);


END;

DECLARE
  CURSOR GET_DATA3 IS
    WITH v AS
    (
    SELECT
           vc2.ipcode
          ,vc2.dateissued AS dateissued
          ,vc2.loyaltyidnumber
          ,pu.recordtype
          ,pu.cid
          ,pu.birthdate
          ,row_number() over(PARTITION BY pu.cid ORDER BY vc2.dateissued DESC) r -- changed partition over ipcode by cid ?
      FROM bp_ae.lw_virtualcard vc2
      INNER JOIN bp_ae.ae_profileupdates2 pu
         ON pu.loyaltynumber = vc2.loyaltyidnumber
    )
    SELECT v.ipcode
          ,v.loyaltyidnumber              AS floyaltynumber
          ,v.recordtype
          ,v.cid
          ,v.birthdate                   AS fbirthdate
          ,to_char(lm.birthdate  ,'MMDDYYYY') birthdate
          ,CASE WHEN v.recordtype ='S' THEN
                                         'P'
                                       ELSE
                                         'S'
                                       END AS NewRecordType
      FROM v
    INNER JOIN bp_ae.lw_loyaltymember lm
        ON lm.ipcode = v.ipcode
    WHERE (v.r = 1  AND v.recordtype = 'S') OR
          (v.r <> 1 AND v.recordtype ='P') ;

      TYPE T_TAB3 IS TABLE OF GET_DATA3%ROWTYPE;
      V_TBL3 T_TAB3;

BEGIN
      utility_pkg.Log_Process_Start(v_jobname, 'Load AE_MEBERSTOUPDATE', v_processId);
      V_REASON := 'Load AE_MEBERSTOUPDATE';
      v_messagesreceived := 0;
      OPEN GET_DATA3;
      LOOP
        FETCH GET_DATA3 BULK COLLECT
          INTO V_TBL3 LIMIT 100; -- increase the bulk size to 100,000

        FORALL I IN 1 .. V_TBL3.COUNT
                INSERT INTO AE_MEBERSTOUPDATE
                     (ipcode         ,
                      floyaltynumber ,
                      recordtype     ,
                      cid            ,
                      fbirthdate     ,
                      birthdate      ,
                      NewRecordType
                      )
                VALUES
                     (V_TBL3(i).ipcode         ,
                      V_TBL3(i).floyaltynumber ,
                      V_TBL3(i).recordtype     ,
                      V_TBL3(i).cid            ,
                      V_TBL3(i).fbirthdate     ,
                      V_TBL3(i).birthdate      ,
                      V_TBL3(i).NewRecordType
                      );
           COMMIT WRITE BATCH NOWAIT;

        EXIT WHEN GET_DATA3%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA3%ISOPEN THEN
         CLOSE GET_DATA3;
      END IF;

      utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

END;

DECLARE
  CURSOR GET_DATA6 IS
      SELECT ipcode    ,
             floyaltynumber ,
             recordtype     ,
             cid            ,
             fbirthdate     ,
             birthdate      ,
             NewRecordType
      FROM AE_MEBERSTOUPDATE;

    TYPE T_TAB6 IS TABLE OF GET_DATA6%ROWTYPE;
     V_TBL6 T_TAB6;

BEGIN
      utility_pkg.Log_Process_Start(v_jobname, 'Load exception ATIUpdate  Changed', v_processId);
      V_REASON := 'Load exception ATIUpdate  Changed';
      v_messagesreceived := 0;

      OPEN GET_DATA6;
      LOOP
        FETCH GET_DATA6 BULK COLLECT
          INTO V_TBL6 LIMIT 100;

           FORALL I IN 1 .. V_TBL6.COUNT
                INSERT INTO Ae_Profileupdateexception
                     (Loyaltyid, Processdate, Notes)
                VALUES
                     (V_TBL6(i).FLoyaltynumber,
                      trunc(SYSDATE),
                      'AIT Update Changed');
           COMMIT WRITE BATCH NOWAIT;


        EXIT WHEN GET_DATA6%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA6%ISOPEN THEN
         CLOSE GET_DATA6;
      END IF;

      utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
END;

	  utility_pkg.Log_Process_Start(v_jobname, 'Reset RECORD TYPE ON DATE ISSUED', v_processId);
	  V_REASON := 'Reset RECORD TYPE ON DATE ISSUED';
    v_messagesreceived := 0;

   --RESET_RECORDTYPE();
    ctas_profileupdates2;
	  utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

	----


--- write to exceptions type  1 table
DECLARE
      CURSOR GET_DATA IS
           SELECT  p2.loyaltynumber
             FROM AE_PROFILEUPDATES2 p2
             LEFT JOIN lw_virtualcard vc ON vc.loyaltyidnumber = p2.loyaltynumber
           WHERE vc.loyaltyidnumber IS NULL AND
                nvl(p2.isenrollment ,0)  = 0 ;-- AEO-1440

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

    BEGIN

      utility_pkg.Log_Process_Start(v_jobname, 'write exceptions 2', v_processId);
      V_REASON := 'write exceptions 2';
      v_messagesreceived := 0;

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;

          FORALL I IN 1 .. V_TBL.COUNT

                INSERT INTO ae_ProfileUpdateException
                     ( LoyaltyID  , processdate, notes)
                VALUES
                     (v_Tbl(i).loyaltynumber, SYSDATE,'Not found  LW_virtualcard');

          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

	    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

    END;
--- write exceptions type 1

DECLARE

 CURSOR GET_DATA IS
    select /*+index(vc)*/p.loyaltynumber,
              sysdate as procdate,
             'Members Merged in BP Db' AS TEXT
        from bp_ae.ae_profileupdates2 p,
             bp_ae.lw_virtualcard vc,
        (
         select b.loyaltynumber, b.cid, b.ipcode from
               (select /*+use_hash(p2, vc)*/
                       p2.loyaltynumber,
                       p2.cid,
                       vc.ipcode
                  from bp_ae.ae_profileupdates2 p2,
                       bp_ae.lw_virtualcard vc
                 where p2.recordtype = 'P'
                   and vc.loyaltyidnumber = p2.loyaltynumber) b,
               (SELECT /*+use_hash(p3, vc1)*/VC1.IPCODE
                  FROM bp_ae.AE_PROFILEUPDATES2 P3,
                       bp_ae.LW_VIRTUALCARD VC1
                 WHERE VC1.LOYALTYIDNUMBER = P3.LOYALTYNUMBER
                   AND P3.RECORDTYPE = 'P'
              GROUP BY VC1.IPCODE HAVING COUNT(VC1.IPCODE) > 1)c
            where b.ipcode = c.ipcode) a
         where vc.loyaltyidnumber = p.loyaltynumber
           and a.loyaltynumber = p.loyaltynumber
           and p.cid = a.cid
           and vc.ipcode = a.ipcode;

	  TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

    BEGIN
      utility_pkg.Log_Process_Start(v_jobname, 'write exceptions 1', v_processId);
      V_REASON := 'write exceptions 1';
      v_messagesreceived := 0;

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;

          FOR I IN 1 .. V_TBL.COUNT LOOP
              INSERT INTO Ae_Profileupdateexception
              (
              Loyaltyid
              ,Processdate
              ,Notes
              )
              values
              (
              v_Tbl(i).loyaltynumber
              ,v_Tbl(i).procdate
              ,v_Tbl(i).TEXT
              );
          END LOOP;
          COMMIT;

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;
	    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    END;

    utility_pkg.Log_Process_Start(v_jobname, 'Reset ISPRIMARY', v_processId);
    V_REASON := 'Reset ISPRIMARY';
    v_messagesreceived := 0;

    RESET_ISPRIMARY();

    utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);

    -----------------------------------------------------------------------------------------
    -- Insert Into Stage table from external table joining virtualcard and memberdetails to
    -- determine if a member exists and if the aitflag is set to bypass the update.
    -----------------------------------------------------------------------------------------
    DECLARE
      CURSOR GET_DATA IS
        SELECT
         NVL(VC.IPCODE, 0) AS IPCODE,
         P.CID,
         P.FIRSTNAME,
         P.LASTNAME,
         P.ADDRESSLINE1,
         P.ADDRESSLINE2,
         P.CITY,
         P.STATE,
         P.ZIP,
         ST.A_COUNTRYCODE AS COUNTRY,
         P.LOYALTYNUMBER,
         p.validaddress AS ADDRESSMAILABLE,-- AEO-1440
         NVL(MD.A_AITUPDATE, 0) AS AITFLAG,
         P.AEVISAACCOUNTKEY, -- AEO-1440
         P.EMAILADDRESS ,
         P.recordtype,
				 P.birthdate
          FROM AE_PROFILEUPDATES2 P,
              /* (SELECT
                 IPCODE, MAX(LOYALTYIDNUMBER) AS LOYALTYIDNUMBER
                  FROM LW_VIRTUALCARD VC, AE_PROFILEUPDATES2 P
                 WHERE VC.LOYALTYIDNUMBER = P.LOYALTYNUMBER
                 GROUP BY VC.IPCODE)*/
                 (SELECT  vc1.ipcode, vc1.loyaltyidnumber, vc1.r  FROM
                       (SELECT /*+full(vc2) parallel(vc2,4)*/
                           vc2.ipcode
                          ,vc2.loyaltyidnumber
                          ,row_number() over(PARTITION BY vc2.ipcode ORDER BY vc2.dateissued DESC) r -- changed partition over ipcode by cid ?
                         FROM bp_ae.lw_virtualcard vc2
                         INNER JOIN bp_ae.ae_profileupdates2 pu
                            ON pu.loyaltynumber = vc2.loyaltyidnumber  ) vc1
                  WHERE vc1.r =1) VC,
             --  lw_virtualcard vc  ,
               ATS_MEMBERDETAILS MD,
               ATS_REFSTATES ST,
               LW_LOYALTYMEMBER LM
         WHERE P.LOYALTYNUMBER = VC.LOYALTYIDNUMBER(+)
           AND VC.IPCODE = LM.IPCODE(+)
           AND VC.IPCODE = MD.A_IPCODE(+)
           AND P.STATE = ST.A_STATECODE(+)
           AND (p.isenrollment =0 OR p.isenrollment IS NULL) -- AEO-1440
           AND LM.MEMBERSTATUS <> 3;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

    BEGIN

	  utility_pkg.Log_Process_Start(v_jobname, 'Write to LW_PROFILEUPDATES_STAGE', v_processId);
    V_REASON := 'Write to LW_PROFILEUPDATES_STAGE';
    v_messagesreceived := 0;

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;
        FORALL I IN 1 .. V_TBL.COUNT
          INSERT INTO LW_PROFILEUPDATES_STAGE
            (ID,
             IPCODE,
             CID,
             FIRSTNAME,
             LASTNAME,
             ADDRESSLINE1,
             ADDRESSLINE2,
             CITY,
             STATE,
             ZIP,
             COUNTRY,
             ADDRESSMAILABLE,
             CREATEDATE,
             ERRORCODE,
             ACCOUNTKEY,
             EMAILADDRESS,
             RECORDTYPE,
						 BIRTHDATE)
          VALUES
            (SEQ_ROWKEY.NEXTVAL,
             V_TBL(I).IPCODE,
             V_TBL(I).CID,
             V_TBL(I).FIRSTNAME,
             V_TBL(I).LASTNAME,
             V_TBL(I).ADDRESSLINE1,
             V_TBL(I).ADDRESSLINE2,
             V_TBL(I).CITY,
             V_TBL(I).STATE,
             V_TBL(I).ZIP,
             V_TBL(I).COUNTRY,
             V_TBL(I).ADDRESSMAILABLE,
             SYSDATE,
             CASE
               WHEN V_TBL(I).IPCODE = 0 THEN
                1
               WHEN V_TBL(I).AITFLAG = 1 THEN
                2
               ELSE
                0
             END,

             SUBSTR(TRIM(V_TBL(I).AEVISAACCOUNTKEY), 1, 20),
             V_TBL(I).EMAILADDRESS,
             V_TBL(I).RECORDTYPE,
						 V_TBL(I).BIRTHDATE);
        COMMIT;
        EXIT WHEN GET_DATA%NOTFOUND;
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
         CLOSE GET_DATA;
      END IF;

	  utility_pkg.Log_Process_Step_End(v_processId, v_messagesreceived);
    END;



--- write to ats_profileupdatemerge
/*
DECLARE
      CURSOR GET_DATA IS
        select
              a.cid,
              a.primary,
              a.secondary,
               v1.loyaltyidnumber as vc_primary,
               v2.loyaltyidnumber as vc_secondary
         from
        (select p2.cid,
               p2.loyaltynumber as primary,
               w.loyaltynumber as secondary,
               w.a_extendedplaycode
          from bp_ae.ae_profileupdates2 p2,
               bp_ae.lw_virtualcard vc,
               (SELECT p.Cid, p.Recordtype, p.Loyaltynumber, md.a_extendedplaycode
                                FROM   bp_ae.AE_PROFILEUPDATES2 p,
                                        bp_ae.Lw_Virtualcard Vc ,
                                        bp_ae.ats_memberdetails md
                                WHERE  Vc.Loyaltyidnumber = p.Loyaltynumber AND
                                       md.a_ipcode = vc.ipcode      AND
                                 p.Recordtype = 'S') w,
               (select ex.loyaltyid
                  from bp_ae.ae_profileupdateexception ex
                 where ex.notes = 'Members Merged in BP Db'
                  AND  trunc(ex.processdate)  > trunc(SYSDATE))e
         where  vc.loyaltyidnumber = p2.loyaltynumber
           and  w.cid = p2.cid
           and p2.recordtype = 'P'
           and p2.loyaltynumber = e.loyaltyid(+)
           and e.loyaltyid is null) a,
         bp_ae.lw_virtualcard v1,
         bp_ae.lw_virtualcard v2,
         bp_ae.ats_memberdetails md1
        where a.primary = v1.loyaltyidnumber(+)
          and a.secondary = v2.loyaltyidnumber(+)
          AND md1.a_ipcode = v1.ipcode
          AND ae_isinpilot( md1.a_ipcode ) = ae_isinpilot( a.a_extendedplaycode)
          and v1.loyaltyidnumber is not null
          and v2.loyaltyidnumber is not null
          and v1.loyaltyidnumber != v2.loyaltyidnumber;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

    BEGIN

      V_ERROR          :=  ' ';
      V_REASON         := 'write Ats_Profileupdatemerge begin';
      V_MESSAGE        := 'write Ats_Profileupdatemerge begin';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;

          FOR I IN 1 .. V_TBL.COUNT LOOP

            SELECT COUNT(*) INTO lv_primaryipcode  FROM lw_virtualcard vc
            WHERE  vc.loyaltyidnumber = v_tbl(i).primary;

            IF ( lv_primaryipcode > 0 ) THEN
               SELECT vc.ipcode INTO lv_primaryipcode  FROM lw_virtualcard vc
               WHERE  vc.loyaltyidnumber = v_tbl(i).primary;
            END IF;

            SELECT COUNT(*) INTO lv_secondaryipcode  FROM lw_virtualcard vc
            WHERE  vc.loyaltyidnumber = v_tbl(i).secondary;

            IF ( lv_secondaryipcode > 0 ) THEN
               SELECT vc.ipcode INTO lv_secondaryipcode  FROM lw_virtualcard vc
               WHERE  vc.loyaltyidnumber = v_tbl(i).secondary;
            END IF;

            IF ( (lv_primaryipcode > 0 AND lv_secondaryipcode > 0) AND
                  (lv_primaryipcode <> lv_secondaryipcode )) THEN
                INSERT INTO Ats_Profileupdatemerge
                     (a_Rowkey,
                      a_Ipcode,
                      a_Parentrowkey,
                      a_Fromloyaltyid,
                      a_Toloyaltyid,
                      Statuscode,
                      Createdate,
                      Updatedate,
                      Lwidentifier,
                      Lastdmlid,
                      a_Cid,
                      Last_Dml_Id,
                      a_status)
                VALUES
                     (Seq_Rowkey.Nextval,
                      Lv_Secondaryipcode,
                      -1,
                      v_Tbl(i).Secondary,
                      v_Tbl(i).Primary,
                      0,
                      SYSDATE ,
                      SYSDATE ,
                      0,
                      0,
                      v_Tbl(i).Cid,
                      Last_Dml_Id#.Nextval,
                      0);
            END IF;
          END LOOP;
          COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

	  V_ERROR          :=  ' ';
      V_REASON         := 'write Ats_Profileupdatemerge end';
      V_MESSAGE        := 'write Ats_Profileupdatemerge end';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
    END;


----*/

----
    DECLARE
      /* check log file for errors */
      LV_ERR VARCHAR2(4000);
      LV_N   NUMBER;
    BEGIN
      EXECUTE IMMEDIATE 'SELECT COUNT(*), MAX(rec)' || CHR(10) ||
                        'FROM EXT_PROFILEUPDATES2_LOG' || CHR(10) ||
                        'WHERE rec LIKE ''ORA-%'''
        INTO LV_N, LV_ERR;

      IF LV_N > 0 THEN
        /* log error msg */
        /* increment jobs fail count */
        V_MESSAGESFAILED := V_MESSAGESFAILED + LV_N;
        V_REASON         := 'Failed reads by external table';
        V_MESSAGE        := '<StageProc>' || CHR(10) || '  <Tables>' ||
                            CHR(10) || '    <External>EXT_ProfileUpdates2' ||
                            CHR(10) ||
                            '    <Stage>LW_ProfileUpdates_Stage</Stage>' ||
                            CHR(10) || '    <FileName>' || P_FILENAME ||
                            '</FileName>' || CHR(10) || '  </Tables>' ||
                            CHR(10) || '</StageProc>';
        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => V_LOGSOURCE,
                            P_FILENAME  => P_FILENAME,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => LV_ERR,
                            P_TRYCOUNT  => LV_N,
                            P_MSGTIME   => SYSDATE);
      END IF;
    END;

    /* insert here */
    V_ENDTIME   := SYSDATE;
    V_JOBSTATUS := 1;

    /* log end of job */
    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => P_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => 'Stage-' || V_JOBNAME);

    OPEN RETVAL FOR
      SELECT V_MY_LOG_ID FROM DUAL;
  EXCEPTION
    WHEN DML_ERRORS THEN

      FOR INDX IN 1 .. SQL%BULK_EXCEPTIONS.COUNT LOOP
        V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
        V_ERROR          := SQLERRM(-SQL%BULK_EXCEPTIONS(INDX).ERROR_CODE);
        V_REASON         := 'Failed Records in Procedure Stage_ProfileUpdates02: ';
        V_MESSAGE        := ' ';

        UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                            P_ENVKEY    => V_ENVKEY,
                            P_LOGSOURCE => V_LOGSOURCE,
                            P_FILENAME  => NULL,
                            P_BATCHID   => V_BATCHID,
                            P_JOBNUMBER => V_MY_LOG_ID,
                            P_MESSAGE   => V_MESSAGE,
                            P_REASON    => V_REASON,
                            P_ERROR     => V_ERROR,
                            P_TRYCOUNT  => V_TRYCOUNT,
                            P_MSGTIME   => SYSDATE);

      END LOOP;
      COMMIT;
      V_ENDTIME        := SYSDATE;
      V_MESSAGESFAILED := V_MESSAGESFAILED;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);
    WHEN OTHERS THEN
      V_MESSAGESFAILED := V_MESSAGESFAILED + 1;
      V_ENDTIME        := SYSDATE;
      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                          P_JOBDIRECTION     => V_JOBDIRECTION,
                          P_FILENAME         => NULL,
                          P_STARTTIME        => V_STARTTIME,
                          P_ENDTIME          => V_ENDTIME,
                          P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                          P_MESSAGESFAILED   => V_MESSAGESFAILED,
                          P_JOBSTATUS        => V_JOBSTATUS,
                          P_JOBNAME          => V_JOBNAME);

      V_ERROR   := SQLERRM;
      V_MESSAGE := '<failed>' || CHR(10) || '  <details>' || CHR(10) ||
                   '    <pkg>STAGE_PROFILEUPDATES_DAILY</pkg>' || CHR(10) ||
                   '    <proc>STAGE_PROFILEUPDATES02</proc>' || CHR(10) ||
                   '  </details>' || CHR(10) || '</failed>';
      UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

      RAISE;

  END STAGE_PROFILEUPDATES02;

  PROCEDURE update_memberdetails( n_partition INTEGER, n_mod INTEGER) AS

    CURSOR GET_DATAMTU IS
       SELECT DISTINCT Ipcode
       FROM Bp_Ae.Ae_Meberstoupdate;

    TYPE T_TAB3 IS TABLE OF GET_DATAMTU%ROWTYPE;
    V_TBL3 T_TAB3;

   -- resultData RCURSOR := NULL;

   /* AEO-1440 begin
    CURSOR GET_DATAHS IS
          SELECT DISTINCT w.Ipcode, w.Programchangedate, w.Extendedplaycode
          FROM   Bp_Ae.Ae_Hubsyncchangeprogram w
          WHERE  (Ipcode, Programchangedate) IN
                 (SELECT Ipcode, MAX(Programchangedate)
                  FROM   Bp_Ae.Ae_Hubsyncchangeprogram s
                  WHERE  w.Ipcode = s.Ipcode
                  GROUP  BY s.Ipcode);

    TYPE T_TAB2 IS TABLE OF GET_DATAHS%ROWTYPE;
    V_TBL2 T_TAB2;
    AEO-1440 end*/


-- aeo-1440 begin
    CURSOR GET_DATA IS
     SELECT p.Ipcode,
           p.Cid,
           p.Firstname,
           p.Lastname,
           p.Addressline1,
           p.Addressline2,
           p.City,
           p.State,
           p.Zip,
           p.Country,
           p.Addressmailable,
           p.Recordtype,
           p.Birthdate,
           p.Emailaddress,
           Pu2.Enrollmentid,
           Pu2.Validemailaddress,
           Pu2.Celldoubleoptin,
           to_char( nvl(Pu2.Enrollmentdate, SYSDATE),'MMDDYYYY') AS Enrollmentdate,
          -- Pu2.Enrollmentdate,
           Pu2.Employeestauts,
           Pu2.Storenumber,
           Pu2.Enrollmentsource,
           Pu2.Aeccstatus,
           Pu2.Aeccopendate,
           Pu2.Aeccclosedate,
           Pu2.Aevisastatus,
           Pu2.Aevisaopendate,
           Pu2.Aevisaclosedate,
           pu2.loyaltyidnumber
    FROM   Lw_Profileupdates_Stage p
    INNER  JOIN (SELECT Vc1.Ipcode,
                        Vc1.Enrollmentid,
                        Vc1.Validemailaddress,
                        Vc1.Celldoubleoptin,
                        Vc1.Enrollmentdate,
                        Vc1.Employeestauts,
                        Vc1.Storenumber,
                        Vc1.Enrollmentsource,
                        Vc1.Aeccstatus,
                        Vc1.Aeccopendate,
                        Vc1.Aeccclosedate,
                        Vc1.Aevisastatus,
                        Vc1.Aevisaopendate,
                        Vc1.Aevisaclosedate,
                        vc1.loyaltyidnumber
                 FROM   (SELECT /*+full(vc2) parallel(vc2,4)*/
                          Vc2.Ipcode,
                          Vc2.Loyaltyidnumber,
                          Row_Number() Over(PARTITION BY Vc2.Ipcode ORDER BY Vc2.Dateissued DESC) r, -- changed partition over ipcode by cid ?
                          Nvl(Pu.Enrollmentid, 0) AS Enrollmentid,
                          Nvl(Pu.Validemailaddress, 0) AS Validemailaddress,
                          Nvl(Pu.Celldoubleoptin, 0) AS Celldoubleoptin,
                          Nvl(Pu.Enrollmentdate, SYSDATE) AS Enrollmentdate,
                          Nvl(Pu.Employeestatus, 0) AS Employeestauts,
                          Nvl(Pu.Storenumber, 0) AS Storenumber,
                          Nvl(Pu.Enrollmentsource, 0) AS Enrollmentsource,
                          Nvl(Pu.Aeccstatus, 0) AS Aeccstatus,
                          Nvl(Pu.Aeccopendate, SYSDATE) AS Aeccopendate,
                          Nvl(Pu.Aeccclosedate, SYSDATE) AS Aeccclosedate,
                          Nvl(Pu.Aevisastatus, 0) AS Aevisastatus,
                          Nvl(Pu.Aevisaopendate, SYSDATE) AS Aevisaopendate,
                          Nvl(Pu.Aevisaclosedate, SYSDATE) AS Aevisaclosedate
                         FROM   Bp_Ae.Lw_Virtualcard Vc2
                         INNER  JOIN Bp_Ae.Ae_Profileupdates2 Pu
                         ON     Pu.Loyaltynumber = Vc2.Loyaltyidnumber
                         WHERE  Nvl(Pu.Isenrollment, 0) = 0) Vc1
                 WHERE  Vc1.r = 1) Pu2
    ON     Pu2.Ipcode = p.Ipcode
    WHERE  p.Errorcode = 0  AND MOD(p.Id, n_Mod) = n_Partition
    ORDER  BY p.Id;


-- aeo-1440 end


      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

       V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    err_code    VARCHAR2(10) := '';
    err_msg     VARCHAR2(200) := '';
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := 'STAGE_PROFILEUPDATES2';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    v_exist     NUMBER := 0;
    v_exist2    NUMBER := 0;
    v_cardtype  NUMBER := 0;
    v_ipointtypeid NUMBER := -1;
    v_ipointeventid NUMBER := -1;
    v_vckey NUMBER := 0;
    v_issueaeccbonus BOOLEAN := FALSE;
    DML_ERRORS EXCEPTION;

    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);



BEGIN

    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    V_JOBNAME   := 'update_memberdetails';
    V_LOGSOURCE := V_JOBNAME;

    UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => V_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

    V_ERROR          :=  ' ';
    V_REASON         := 'UPDATE profile data ats_memberdetail start';
    V_MESSAGE        := 'UPDATE profile data ats_memberdetail start';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
    OPEN GET_DATA;

      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;


        FOR i IN 1 .. v_tbl.count LOOP
        IF (v_Tbl(i).Enrollmentdate IS NOT NULL) THEN

        IF ( to_date(v_Tbl(i).Enrollmentdate,'MMDDYYYY') >= to_date('10/1/2017','MM/DD/YYYY') AND
             to_date(v_Tbl(i).Enrollmentdate,'MMDDYYYY') < to_date ('10/11/2017','MM/DD/YYYY') ) THEN


          v_issueaeccbonus := FALSE;
          v_exist  := 0;
          v_exist2 := 0;

         SELECT COUNT(*)
         INTO v_exist
         FROM bp_Ae.Lw_Pointtransaction pt
         INNER JOIN bp_Ae.Lw_Pointtype   pty ON pty.pointtypeid = pt.pointtypeid
         INNER JOIN bp_Ae.Lw_Pointevent  pte ON pte.pointeventid = pt.pointtypeid
         INNER JOIN ( SELECT vc.vckey FROM bp_Ae.Lw_Virtualcard vc WHERE vc.ipcode =  v_tbl(i).ipcode) vc2 ON vc2.vckey = pt.vckey
         WHERE  pty.name  ='AEO Connected Bonus Points' AND
                pte.name = 'AECC Sign Up Bonus' ;

         IF (v_exist = 0) THEN

          SELECT COUNT(*)
          INTO v_exist
          FROM bp_ae.ats_memberdetails md
          WHERE md.a_ipcode = V_TBL(I).IPCODE;

          SELECT COUNT(*)
          INTO v_exist2
          FROM bp_ae.lw_virtualcard vc
          WHERE vc.ipcode = V_TBL(I).IPCODE AND vc.loyaltyidnumber =V_TBL(I).loyaltyidnumber;

          IF (v_exist >0 AND v_exist2 > 0) THEN

              SELECT md.a_cardtype , vc.vckey
              INTO v_cardtype, v_vckey
              FROM bp_ae.ats_memberdetails md
              INNER JOIN bp_Ae.Lw_Virtualcard vc ON vc.ipcode = md.a_ipcode
              WHERE md.a_ipcode  = v_tbl(i).ipcode AND vc.loyaltyidnumber =V_TBL(I).loyaltyidnumber;

              IF ( (v_tbl(i).aeccstatus   = 1  OR v_tbl(i).aeccstatus   =3  OR
                    v_tbl(i).aevisastatus = 1  OR v_tbl(i).aevisastatus =3 )AND nvl(v_cardtype,0) = 0 ) THEN
                   v_issueaeccbonus := TRUE;
              END IF;

              IF (v_issueaeccbonus) THEN

                SELECT Pointtypeid
                INTO   v_exist
                FROM   Lw_Pointtype
                WHERE  NAME = 'AEO Connected Bonus Points';

                IF (v_exist > 0 ) THEN
                    SELECT Pointtypeid
                    INTO   v_ipointtypeid
                    FROM   Lw_Pointtype
                    WHERE  NAME = 'AEO Connected Bonus Points';

                     SELECT Pointeventid
                     INTO   v_exist
                     FROM   Lw_Pointevent
                     WHERE  NAME = 'AECC Sign Up Bonus';

                     IF (v_exist > 0 ) THEN

                       SELECT Pointeventid
                       INTO   v_ipointeventid
                       FROM   Lw_Pointevent
                       WHERE  NAME = 'AECC Sign Up Bonus';

                       INSERT INTO Lw_Pointtransaction
                            (Pointtransactionid,
                             Vckey,
                             Pointtypeid,
                             Pointeventid,
                             Transactiontype,
                             Transactiondate,
                             Pointawarddate,
                             Points,
                             Expirationdate,
                             Notes,
                             Ownertype,
                             Ownerid,
                             Rowkey,
                             Parenttransactionid,
                             Pointsconsumed,
                             Pointsonhold,
                             Ptlocationid,
                             Ptchangedby,
                             Createdate,
                             Expirationreason)
                       VALUES
                            (seq_pointtransactionid.nextval,
                             v_vckey,
                             v_iPointtypeid,
                             v_iPointeventid,
                             1 /*purchase*/,
                             SYSDATE,
                             SYSDATE,
                             2500,
                             To_Date('12/31/2199', 'mm/dd/yyyy'),
                             '2500 Bonus Points for signing up for AECC',
                             -1,
                             -1,
                             -1,
                             -1,
                             0 /*Pointsconsumed*/,
                             0,
                             NULL,
                             'Profile-in-Enrollment' /*Ptchangedby*/,
                             SYSDATE,
                             NULL);
                       END IF;


                END IF ;

            END IF;

        -- aeo-1788 end


          END IF;

         END IF;

         END IF;
         END IF;

        END LOOP;



        FORALL I IN 1 .. V_TBL.COUNT
          UPDATE ATS_MEMBERDETAILS MD

             SET MD.A_ADDRESSMAILABLE = CASE
                                          WHEN NVL(MD.A_ADDRESSMAILABLE, -1) != V_TBL(I).ADDRESSMAILABLE THEN
                                              CASE
                                                 WHEN v_tbl(i).ADDRESSMAILABLE = 0 THEN
                                                  0
                                                 WHEN NVL(MD.A_ADDRESSMAILABLE, -1) != V_TBL(I).ADDRESSMAILABLE AND (V_TBL(I).ADDRESSMAILABLE = 1 AND V_TBL(I).ADDRESSLINE1 IS NOT NULL AND V_TBL(I)
                                                        .CITY IS NOT NULL AND V_TBL(I).ZIP IS NOT NULL AND V_TBL(I)
                                                        .STATE IS NOT NULL AND V_TBL(I)
                                                        .STATE IN (SELECT A_STATECODE FROM ATS_REFSTATES)) THEN
                                                    1
                                                  ELSE
                                                     md.A_ADDRESSMAILABLE
                                              END
                                          ELSE
                                            md.a_addressmailable
                                        END,
                 MD.A_CHANGEDBY       = 'Profile Updates Process/Profile changes',
                 md.a_emailaddress = case when (V_TBL(I).Emailaddress IS NOT NULL AND V_TBL(I).Emailaddress<> md.a_emailaddress ) then
                                      cast(to_char(V_TBL(I).Emailaddress) as nvarchar2(150))
                                     else
                                      cast(to_char(md.A_EMAILADDRESS)as nvarchar2(150))
                                      end ,
                                      --  AEO-1108 begin

                 md.a_emailaddressmailable = CASE WHEN ( md.a_emailaddress IS NOT NULL AND   REGEXP_LIKE (md.a_emailaddress,'^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$')  AND
                                                      nvl(v_tbl(i).validEmailAddress,0 )=1 )THEN
                                                 1
                                             ELSE
                                               0
                                             END,
                                             --  AEO-1108 end

                 MD.A_ADDRESSLINEONE  = cast(nvl(V_TBL(I).ADDRESSLINE1,MD.A_ADDRESSLINEONE) AS VARCHAR2(400)),
                 MD.A_ADDRESSLINETWO  =  case
                                             when ( (NVL(TRIM(md.a_ADDRESSLINETWO),'x') != V_TBL(I).ADDRESSLINE2 OR V_TBL(I).ADDRESSLINE2 IS NULL ) )  then
                                                  CAST(V_TBL(I).ADDRESSLINE2 AS VARCHAR2(400))

                                         else
                                                 CAST(md.a_ADDRESSLINETWO AS varchar2(400))
                                         END,

                 MD.A_CITY            = cast(nvl(V_TBL(I).CITY,MD.A_CITY ) AS VARCHAR2(200)),
                 MD.A_STATEORPROVINCE = cast(nvl(V_TBL(I).STATE,MD.A_STATEORPROVINCE) AS VARCHAR2(200)),
                 MD.A_ZIPORPOSTALCODE = nvl(V_TBL(I).ZIP,MD.A_ZIPORPOSTALCODE),
                 MD.A_COUNTRY         = cast(nvl(V_TBL(I).COUNTRY,MD.A_COUNTRY) AS VARCHAR(200)),
                 MD.UPDATEDATE        = SYSDATE,


                 /* AEO-1440 begin */
                   MD.a_Enrollmentid  = cast(nvl(V_TBL(I).Enrollmentid,MD.a_Enrollmentid ) AS nVARCHAR2(100)),
                 --  md.a_emailaddressmailable = nvl(v_tbl(i).validEmailAddress,0 ),
                   md.a_smsoptin = nvl(v_tbl(i).Celldoubleoptin,0 ),
                   md.a_smsoptindate =  CASE  WHEN v_Tbl(i).Enrollmentdate IS NULL THEN md.a_smsoptindate
                                        ELSE to_date(v_Tbl(i).Enrollmentdate,'MMDDYYYY') END,
                   md.a_employeecode  = v_tbl(i).Employeestauts,
                   md.a_homestoreid =    v_tbl(i).Storenumber,
                   md.a_membersource = v_tbl(i).EnrollmentSource,
                   md.a_cardopendate =  CASE
                                       WHEN (v_tbl(i).aeccstatus =1 AND nvl(md.a_cardtype,0) =0) THEN
                                          v_tbl(i).aeccopendate
                                       WHEN  (v_tbl(i).aeccstatus =1 AND nvl(md.a_cardtype,0) =2) THEN
                                         v_tbl(i).aeccopendate
                                         --
                                        WHEN (v_tbl(i).aevisastatus =1 AND nvl(md.a_cardtype,0) =0) THEN
                                          v_tbl(i).aevisaopendate
                                        WHEN (v_tbl(i).aevisastatus =1 AND nvl(md.a_cardtype,0) =2) THEN
                                          v_tbl(i).aeccopendate
                                         WHEN (v_tbl(i).aevisastatus =1 AND nvl(md.a_cardtype,0) =2) THEN
                                          v_tbl(i).aeVISAopendate
                                       ELSE
                                          md.a_cardopendate
                                      END,
                   md.a_cardclosedate =  CASE
                                       WHEN  (v_tbl(i).aeccstatus =2 AND nvl(md.a_cardtype,0) =3) THEN
                                             v_tbl(i).Aeccclosedate
                                             --
                                        WHEN  (v_tbl(i).aevisastatus =2 AND nvl(md.a_cardtype,0) =2) THEN
                                             v_tbl(i).Aeccclosedate
                                         WHEN  (v_tbl(i).aevisastatus =2 AND nvl(md.a_cardtype,0) =3) THEN
                                             v_tbl(i).Aeccclosedate
                                       ELSE
                                          md.a_cardclosedate
                                      END,

                   md.a_cardtype =  CASE
                                       WHEN (v_tbl(i).aeccstatus =1 AND nvl(md.a_cardtype,0) =0) THEN
                                         1
                                       WHEN  (v_tbl(i).aeccstatus =1 AND nvl(md.a_cardtype,0) =2) THEN
                                         3
                                       WHEN  (v_tbl(i).aeccstatus =2 AND nvl(md.a_cardtype,0) =3) THEN
                                         2
                                         --
                                       WHEN (v_tbl(i).aevisastatus =1 AND nvl(md.a_cardtype,0) =0) THEN
                                         2
                                       WHEN  (v_tbl(i).aevisastatus =1 AND nvl(md.a_cardtype,0) =1) THEN
                                         3
                                       WHEN (v_tbl(i).aevisastatus =2 AND nvl(md.a_cardtype,0) =2) THEN
                                         NULL
                                       WHEN  (v_tbl(i).aevisastatus =2 AND nvl(md.a_cardtype,0) =3) THEN
                                         1
                                       ELSE
                                         md.a_cardtype
                                      END


                 /* AEO-1440 end */
           WHERE MD.A_IPCODE = V_TBL(I).IPCODE;
          COMMIT;




          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

      V_ERROR          :=  ' ';
      V_REASON         := ' update profile data to ats_memberdetails finished';
      V_MESSAGE        := ' update profile data to ats_memberdetails finished';
	    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


-----
/* AEO-1440 begin
    V_ERROR          :=  ' ';
    V_REASON         := 'UPDATE program change data ats_memberdetail start';
    V_MESSAGE        := 'UPDATE program change data ats_memberdetail start';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
    OPEN GET_DATAHS;

      LOOP
        FETCH GET_DATAHS BULK COLLECT INTO V_TBL2 LIMIT 100;

        -- AEO-1150 begin
        FORALL I IN 1 .. V_TBL2.COUNT
           UPDATE ATS_MEMBERDETAILS MD

             SET   md.a_aitupdate     = CASE WHEN v_tbl2(i).Extendedplaycode <> md.a_extendedplaycode THEN
                                           1
                                      ELSE
                                         md.a_aitupdate
                                      END,
                 MD.a_Extendedplaycode  =  v_tbl2(i).Extendedplaycode,
                 md.a_programchangedate =  v_tbl2(i).Programchangedate,
                 MD.a_Changedby       = 'Profile Updates Process/Change Loyalty Program',
                 MD.UPDATEDATE        = SYSDATE
           WHERE MD.A_IPCODE = V_TBL2(I).IPCODE ;
AEO-1440 end */

/*
           UPDATE ATS_MEMBERDETAILS MD
             SET MD.a_programchangedate = TRUNC(SYSDATE),
                 md.a_extendedplaycode = CASE WHEN md.a_extendedplaycode IN (1,3) THEN
                                           3
                                         ELSE
                                           2
                                         END,
                 MD.a_Changedby         = 'Profile Updates Process/Change Loyalty Program',
                 MD.UPDATEDATE          =  trunc(SYSDATE)
           WHERE MD.a_ipcode IN (
                   SELECT vc.ipcode
                   FROM bp_ae.lw_virtualcard vc
                   WHERE vc.loyaltyidnumber  IN ( SELECT DISTINCT pum.a_toloyaltyid
                                                  FROM  bp_Ae.Ats_Profileupdatemerge pum
                                                  WHERE pum.a_fromloyaltyid  IN (
                                                      SELECT DISTINCT vc2.loyaltyidnumber
                                                      FROM   Bp_Ae.Ae_Hubsyncchangeprogram w
                                                      INNER JOIN  bp_ae.lw_virtualcard vc2 ON vc2.ipcode = w.ipcode
                                                      WHERE  (w.Ipcode, w.Programchangedate) IN
                                                             (SELECT Ipcode, MAX(Programchangedate)
                                                              FROM   Bp_Ae.Ae_Hubsyncchangeprogram s
                                                              WHERE  w.Ipcode = s.Ipcode
                                                              GROUP  BY s.Ipcode))
                                                        AND  pum.createdate >= trunc(SYSDATE) AND
                                                             pum.createdate < trunc(SYSDATE+1) ));


          COMMIT;

          EXIT WHEN GET_DATAHS%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
        -- AEO-1150 end
      COMMIT;
      IF GET_DATAHS%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATAHS;
      END IF;



      V_ERROR          :=  ' ';
      V_REASON         := 'UPDATE program change data ats_memberdetail end';
      V_MESSAGE        := 'UPDATE program change data ats_memberdetail end';
    	UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);



      -- AEO-1150 BEgin

    V_ERROR          :=  ' ';
    V_REASON         := 'converting points begin';
    V_MESSAGE        := 'converting points begin';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

    --
    -- code to mimic the execution of the point conversion job from legacy to pilot
    --
    AE_POINTSCONVERSION.CLEARWORKTABLES(NULL); -- clear working tables
    AE_POINTSCONVERSION.CONVERTLEGACYTOPILOT( to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    Rulesprocessing.Headerrulesprocess(NULL,to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    RulesProcessing.BonusEventReturn(NULL,to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    RulesProcessing.DetailTxnRulesProcess(NULL,to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    STAGE_TXNCOPY.TlogInterceptorProcessing(NULL,resultData);
 --   Stage_Tlog.Hist_Tlog01(NULL,SYSDATE, resultData);
    AE_POINTSCONVERSION.CLEARWORKTABLES(NULL); -- clear workign tables again

    --
    -- code to mimic the execution of the point conversion job from pilot to legay
    --
    AE_POINTSCONVERSION.CLEARWORKTABLES(NULL); -- clear working tables
    AE_POINTSCONVERSION.CONVERTPILOTTOLEGACY( to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    Rulesprocessing.Headerrulesprocess(NULL,to_char(SYSDATE,'YYYYmmddHH24miss'), resultData) ;
    RulesProcessing.BonusEventReturn(NULL,to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    RulesProcessing.DetailTxnRulesProcess(NULL,to_char(SYSDATE,'YYYYmmddHH24miss'), resultData);
    STAGE_TXNCOPY.TlogInterceptorProcessing(NULL, resultData);
    AE_POINTSCONVERSION.CLEARWORKTABLES(NULL); -- clear workign tables again

    V_ERROR          :=  ' ';
    V_REASON         := 'converting points end';
    V_MESSAGE        := 'converting points end';
    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


     AEO-1440 end                */

   -- AEO-1150 BEgin


    V_ERROR          :=  ' ';
    V_REASON         := 'UPDATE aitflag ats_memberdetail start';
    V_MESSAGE        := 'UPDATE aitflag ats_memberdetail start';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
    OPEN GET_DATAMTU;

      LOOP
        FETCH GET_DATAMTU BULK COLLECT INTO V_TBL3 LIMIT 100;

        FORALL I IN 1 .. V_TBL3.COUNT
          UPDATE ATS_MEMBERDETAILS MD
             SET md.a_aitupdate     = 1,
                 MD.A_CHANGEDBY       = 'Profile Updates Process/Primary accouunt changes',
                 MD.UPDATEDATE        = SYSDATE
             WHERE MD.A_IPCODE = V_TBL3(I).IPCODE;


          COMMIT;

          EXIT WHEN GET_DATAMTU%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;


      COMMIT;
      IF GET_DATAMTU%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATAMTU;
      END IF;



      V_ERROR          :=  ' ';
      V_REASON         := 'UPDATE aitflag ats_memberdetail start';
      V_MESSAGE        := 'UPDATE aitflag ats_memberdetail start';
    	UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


   EXCEPTION
          WHEN OTHERS THEN
             ROLLBACK;
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 3000);
             V_REASON := 'update_memberdetails'||  err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;


             UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
              RAISE ;

END update_memberdetails;


  PROCEDURE update_loyaltymember( n_partition INTEGER, n_mod INTEGER) AS

    CURSOR GET_DATA IS
        SELECT P.IPCODE,
               P.CID,
               P.FIRSTNAME,
               P.LASTNAME,
               P.ADDRESSLINE1,
               P.ADDRESSLINE2,
               P.CITY,
               P.STATE,
               P.ZIP,
               P.COUNTRY,
               P.ADDRESSMAILABLE,
               P.RECORDTYPE,
							 P.BIRTHDATE,
               p.emailaddress
          FROM LW_PROFILEUPDATES_STAGE P
         WHERE P.ERRORCODE = 0 AND MOD(p.id, n_mod) = n_partition
         ORDER BY P.ID;
      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

       V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
      err_code    VARCHAR2(10) := '';
    err_msg     VARCHAR2(200) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := 'STAGE_PROFILEUPDATES2';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;

    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);



BEGIN

    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    V_JOBNAME   := 'update_loyaltymember';
    V_LOGSOURCE := V_JOBNAME;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => V_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

    V_ERROR          :=  ' ';
    V_REASON         := 'update_loyaltymember start';
    V_MESSAGE        := 'update_loyaltymember start';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
    OPEN GET_DATA;

      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;

          FORALL I IN 1 .. V_TBL.COUNT  --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
              UPDATE LW_LOYALTYMEMBER LM
                 SET LM.FIRSTNAME = CASE  WHEN (NVL(TRIM(LM.FIRSTNAME), 'x') != V_TBL(I).FIRSTNAME AND V_TBL(I).FIRSTNAME IS NOT NULL) THEN
                       to_char(V_TBL(I).FIRSTNAME)
                      ELSE
                        to_char(lm.firstname)
                        END,
                      LM.LASTNAME  =CASE WHEN NVL(TRIM(LM.LASTNAME), 'x') != V_TBL(I).LASTNAME AND V_TBL(I).LASTNAME IS NOT NULL THEN
                         to_char(V_TBL(I).LASTNAME)
                     ELSE
                        to_char(lm.lastname)
                     END,

                     LM.CHANGEDBY  = 'Profile Updates Process',
                     LM.Updatedate = SYSDATE,
                     LM.Birthdate =CASE WHEN (V_TBL(I).birthdate IS NOT NULL ) THEN
                         V_TBL(I).BIRTHDATE
                       ELSE
                         lm.birthdate
                         END,
                     LM.PRIMARYPOSTALCODE = V_TBL(I).ZIP

               WHERE LM.IPCODE = V_TBL(I).IPCODE;



          EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

      V_ERROR          :=  ' ';
      V_REASON         := ' update_loyaltymember';
      V_MESSAGE        := ' update_loyaltymember';
	    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);


  EXCEPTION
          WHEN OTHERS THEN
             ROLLBACK;
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'update_loyaltymember'||  err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;


             UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
              RAISE ;

END update_loyaltymember;




  PROCEDURE update_virtualcard( n_partition INTEGER, n_mod INTEGER) AS

    CURSOR GET_DATA IS


   SELECT * FROM (
   SELECT p.Loyaltynumber,
          p.Recordtype,
          p.isenrollment,
          Vc.Ipcode,
          Row_Number() Over(PARTITION BY Vc.Ipcode ORDER BY Vc.Dateissued DESC) r
   FROM   Bp_Ae.Ae_Profileupdates2 p
   INNER  JOIN Bp_Ae.Lw_Virtualcard Vc ON     Vc.Loyaltyidnumber = p.Loyaltynumber
   INNER  JOIN Bp_Ae.Lw_Loyaltymember Lm  ON     Lm.Ipcode = Vc.Ipcode
   WHERE  Lm.Memberstatus <> 3
        --  AND Nvl(p.Isenrollment, 0) = 0
          AND MOD(Vc.Ipcode, n_Mod) = n_Partition
          AND p.Recordtype = 'P'
          AND p.Loyaltynumber NOT IN
          (SELECT Pe.Loyaltyid
               FROM   Bp_Ae.Ae_Profileupdateexception Pe
               WHERE  Pe.Notes = 'Members Merged in BP Db'
                      AND Trunc(Pe.Processdate) > Trunc(SYSDATE)))
   WHERE  r = 1;

      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

       V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
      err_code    VARCHAR2(10) := '';
    err_msg     VARCHAR2(200) := '';
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := 'STAGE_PROFILEUPDATES2';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;

    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);



BEGIN

    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    V_JOBNAME   := 'update_virtualcard';
    V_LOGSOURCE := V_JOBNAME;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => V_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

    V_ERROR          :=  ' ';
    V_REASON         := 'update_virtualcard start';
    V_MESSAGE        := 'update_virtualcard start';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
      OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;

          FORALL I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE LW_VIRTUALCARD VC
             SET VC.Isprimary = 0
           WHERE VC.ipcode = V_TBL(I).ipcode AND v_TBL(I).isenrollment =0 ;


          FORALL I IN 1 .. V_TBL.COUNT --<------ here we update all 1,000 rows at once, limiting the statement i/o by 9999 calls per loop
          UPDATE LW_VIRTUALCARD VC
             SET VC.Isprimary = 1
           WHERE VC.Loyaltyidnumber = V_TBL(I).loyaltynumber;

           COMMIT; --<-------- commit to prevent long rollbacks on error (but you cant rollback whats already been commited)

        EXIT WHEN GET_DATA%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF GET_DATA%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE GET_DATA;
      END IF;

      V_ERROR          :=  ' ';
      V_REASON         := ' update_virtualcard';
      V_MESSAGE        := ' update_virtualcard';
	    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);

  EXCEPTION
          WHEN OTHERS THEN
             ROLLBACK;
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'update_virtualcard'||  err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;


             UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
              RAISE ;


END update_virtualcard;



  PROCEDURE update_atssynkey AS

   /* CURSOR GET_DATA2 IS
         SELECT *
         FROM (SELECT VC2.IPCODE,
                      VC2.DATEISSUED AS DATEISSUED,
                      VC2.VCKEY,
                      VC2.LOYALTYIDNUMBER,
                      ROW_NUMBER() OVER(PARTITION BY PU.CID ORDER BY VC2.DATEISSUED ,vc2.vckey DESC) R
                 FROM BP_AE.LW_VIRTUALCARD VC2
                INNER JOIN BP_AE.LW_PROFILEUPDATES_STAGE PU ON PU.IPCODE = VC2.IPCODE) T
         WHERE T.R = 1;

      TYPE T_TAB2 IS TABLE OF GET_DATA2%ROWTYPE;
      V_TBL2 T_TAB2;    /*/

    CURSOR GET_DATA IS
         SELECT DISTINCT VC.VCKEY         AS VCKEY,
               PUS.ACCOUNTKEY            AS STAGEACCOUNTKEY,
               SYA.A_ACCOUNTKEY          AS SYNCHACCOUNTKEY,
               VC.IPCODE                 AS IPCODE,
               PUS.EMAILADDRESS          AS EMAILADDRESS,
               CASE WHEN sya.a_vckey IS NULL THEN 0
                 ELSE 1
                   END AS hassynkey
          FROM LW_PROFILEUPDATES_STAGE PUS
          INNER JOIN LW_VIRTUALCARD VC            ON VC.IPCODE = PUS.IPCODE AND VC.ISPRIMARY = 1
          LEFT JOIN ATS_SYNCHRONYACCOUNTKEY SYA  ON VC.VCKEY = SYA.A_VCKEY;


      TYPE T_TAB IS TABLE OF GET_DATA%ROWTYPE;
      V_TBL T_TAB;

       V_MY_LOG_ID NUMBER;
    --V_DAP_LOG_ID NUMBER; -AEO-374 Begin -End

    --log job attributes
    V_JOBDIRECTION     NUMBER := 0;
    V_STARTTIME        DATE := SYSDATE;
    V_ENDTIME          DATE;
      err_code    VARCHAR2(10) := '';
    err_msg     VARCHAR2(200) := '';
    V_MESSAGESRECEIVED NUMBER := 0;
    V_MESSAGESFAILED   NUMBER := 0;
    V_JOBSTATUS        NUMBER := 0;
    V_JOBNAME          VARCHAR2(256);

    --log msg attributes
    V_MESSAGEID VARCHAR2(256);
    V_ENVKEY    VARCHAR2(256) := 'BP_AE@' ||
                                 UPPER(SYS_CONTEXT('userenv',
                                                   'instance_name'));
    V_LOGSOURCE VARCHAR2(256);
    V_FILENAME  VARCHAR2(256) := 'STAGE_PROFILEUPDATES2';
    V_BATCHID   VARCHAR2(256) := 0;
    V_MESSAGE   VARCHAR2(256);
    V_REASON    VARCHAR2(256);
    V_ERROR     VARCHAR2(256);
    V_TRYCOUNT  NUMBER := 0;
    DML_ERRORS EXCEPTION;


    PRAGMA EXCEPTION_INIT(DML_ERRORS, -24381);



BEGIN

    V_MY_LOG_ID := UTILITY_PKG.GET_LIBJOBID();
    V_JOBNAME   := 'update_atssynkey';
    V_LOGSOURCE := V_JOBNAME;

      UTILITY_PKG.LOG_JOB(P_JOB              => V_MY_LOG_ID,
                        P_JOBDIRECTION     => V_JOBDIRECTION,
                        P_FILENAME         => V_FILENAME,
                        P_STARTTIME        => V_STARTTIME,
                        P_ENDTIME          => V_ENDTIME,
                        P_MESSAGESRECEIVED => V_MESSAGESRECEIVED,
                        P_MESSAGESFAILED   => V_MESSAGESFAILED,
                        P_JOBSTATUS        => V_JOBSTATUS,
                        P_JOBNAME          => v_jobname);

    V_ERROR          :=  ' ';
    V_REASON         := 'update_atssynkey start';
    V_MESSAGE        := 'update_atssynkey start';
	  UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);




    /*

     OPEN Get_Data2;

     LOOP

         FETCH Get_Data2 BULK COLLECT INTO v_Tbl2 LIMIT 1000;

         FORALL i IN 1 .. v_Tbl2.Count
                 UPDATE bp_ae.lw_virtualcard vc
                 SET    vc.isprimary = 1
                 WHERE  vc.vckey = v_Tbl2(i).vckey;

         COMMIT;

         EXIT WHEN Get_Data2%NOTFOUND;

     END LOOP;

     COMMIT;

     IF Get_Data2%ISOPEN  THEN
        CLOSE Get_Data2;
     END IF;
*/

     OPEN GET_DATA;
      LOOP
        FETCH GET_DATA BULK COLLECT
          INTO V_TBL LIMIT 100;

        FOR I IN 1 .. V_TBL.COUNT LOOP

          IF NOT (V_TBL(I).STAGEACCOUNTKEY IS NULL) THEN
             IF (v_tbl(i).hassynkey = 0) THEN
               INSERT INTO ATS_SYNCHRONYACCOUNTKEY
                  (A_ROWKEY,
                   A_VCKEY,
                   a_accountkey ,
                   A_PARENTROWKEY,
                   STATUSCODE,
                   CREATEDATE,
                   UPDATEDATE)
                VALUES
                  (SEQ_ROWKEY.NEXTVAL,
                   V_TBL             (I).VCKEY,
                   v_tbl(i).STAGEACCOUNTKEY,
                   V_TBL             (I).VCKEY,
                   0,
                   SYSDATE,
                   SYSDATE);
             ELSE
               IF V_TBL(I).STAGEACCOUNTKEY <> nvl(V_TBL(I).SYNCHACCOUNTKEY,0) THEN
                UPDATE ATS_SYNCHRONYACCOUNTKEY
                   SET A_ACCOUNTKEY = V_TBL(I).STAGEACCOUNTKEY
                 WHERE A_VCKEY = V_TBL(I).VCKEY;
              END IF;
             END IF;
          END IF;
             /*
              IF V_TBL(I).SYNCHACCOUNTKEY IS NULL THEN
                INSERT INTO ATS_SYNCHRONYACCOUNTKEY
                  (A_ROWKEY,
                   A_VCKEY,
                   A_PARENTROWKEY,
                   STATUSCODE,
                   CREATEDATE,
                   UPDATEDATE)
                VALUES
                  (SEQ_ROWKEY.NEXTVAL,
                   V_TBL             (I).VCKEY,
                   V_TBL             (I).VCKEY,
                   0,
                   SYSDATE,
                   SYSDATE);
              ELSIF V_TBL(I).STAGEACCOUNTKEY <> V_TBL(I).SYNCHACCOUNTKEY THEN
                UPDATE ATS_SYNCHRONYACCOUNTKEY
                   SET A_ACCOUNTKEY = V_TBL(I).STAGEACCOUNTKEY
                 WHERE A_VCKEY = V_TBL(I).VCKEY;
              END IF;
          END IF;  */


          COMMIT;
        END LOOP;
        EXIT WHEN GET_DATA%NOTFOUND;
      END LOOP;
      COMMIT;

      IF GET_DATA%ISOPEN THEN
        CLOSE GET_DATA;
      END IF;

      /*

     OPEN Get_Data2;

     LOOP

         FETCH Get_Data2 BULK COLLECT INTO v_Tbl2 LIMIT 1000;

         FORALL i IN 1 .. v_Tbl2.Count
                 UPDATE bp_ae.lw_virtualcard vc
                 SET    vc.isprimary = 0
                 WHERE  vc.vckey = v_Tbl2(i).vckey;

         COMMIT;

         EXIT WHEN Get_Data2%NOTFOUND;

     END LOOP;

     COMMIT;

     IF Get_Data2%ISOPEN  THEN
        CLOSE Get_Data2;
     END IF;
*/
      V_ERROR          :=  ' ';
      V_REASON         := ' update_atssynkey end';
      V_MESSAGE        := ' update_atssynkey end ';
	    UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
       EXCEPTION
          WHEN OTHERS THEN
             ROLLBACK;
             err_code := SQLCODE;
             err_msg := SUBSTR(SQLERRM, 1, 200);
             V_REASON := 'update_atssynkey'||  err_code ||' ' ||err_msg;
             V_MESSAGE:= v_reason;
             V_ERROR:= v_reason;


             UTILITY_PKG.LOG_MSG(P_MESSAGEID => V_MESSAGEID,
                          P_ENVKEY    => V_ENVKEY,
                          P_LOGSOURCE => V_LOGSOURCE,
                          P_FILENAME  => NULL,
                          P_BATCHID   => V_BATCHID,
                          P_JOBNUMBER => V_MY_LOG_ID,
                          P_MESSAGE   => V_MESSAGE,
                          P_REASON    => V_REASON,
                          P_ERROR     => V_ERROR,
                          P_TRYCOUNT  => V_TRYCOUNT,
                          P_MSGTIME   => SYSDATE);
              RAISE ;
END update_atssynkey;


END STAGE_PROFILEUPDATES_DAILY;
/
