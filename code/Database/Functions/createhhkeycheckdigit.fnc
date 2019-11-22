CREATE OR REPLACE FUNCTION createHHKeyCheckDigit(p_HHKey_string VARCHAR2)
      RETURN VARCHAR2 IS
      v_total int;
      v_digit int;
      v_pointer number;
      v_returnstring nvarchar2(20);
      v_tempDate char(20);

    BEGIN
      IF TRIM(p_HHKey_string) IS NULL THEN
        RETURN NULL;
      END IF;

      v_pointer := 1;
      v_total := 0;

      WHILE Length(p_HHKey_string) > v_pointer
      LOOP
           v_digit := Cast(Substr(p_HHKey_string, v_pointer, 1) as int);
           v_total := v_total + v_digit;
           v_pointer := v_pointer + 1;
      END LOOP;

      v_total := mod(v_total,10);
      v_returnstring := '00' || p_HHKey_string || Cast(v_total as nvarchar2);


      RETURN v_returnstring;
    EXCEPTION
      WHEN OTHERS THEN
      RETURN NULL;
    END createHHKeyCheckDigit;
/

