CREATE OR REPLACE FUNCTION CheckDollarRewardLevel(totalpoints number)
RETURN NUMBER
AS
  Dollarpoints NUMBER ;
  v_DollarRewardLevel NUMBER ;
  v_sql1  VARCHAR2(1000) ;
BEGIN
  v_sql1 := ' Select to_char(value)  from lw_clientconfiguration cc where cc.key = ''DollarRewardsPoints''';
  EXECUTE IMMEDIATE v_sql1 into v_DollarRewardLevel ;
  Dollarpoints := v_DollarRewardLevel;
  -- return 0 if invalid or older than 12
  while totalpoints >= Dollarpoints loop
    Dollarpoints := Dollarpoints + v_DollarRewardLevel;
  end loop;

RETURN Dollarpoints;
END;
/

