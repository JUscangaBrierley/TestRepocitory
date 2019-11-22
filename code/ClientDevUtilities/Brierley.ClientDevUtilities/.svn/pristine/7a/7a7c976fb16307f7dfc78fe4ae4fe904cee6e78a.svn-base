CREATE FUNCTION distance (@Lat1 as DECIMAL(19,5),
                          @Lon1 as DECIMAL(19,5),
                          @Lat2 as DECIMAL(19,5),
                          @Lon2 as DECIMAL(19,5),
                          @Radius as DECIMAL(19,5) = 3963) RETURNS DECIMAL(19,5) AS
BEGIN
  -- Convert degrees to radians
  DECLARE @DegToRad AS DECIMAL(19,5) = 57.29577951;
  RETURN(ISNULL(@Radius,0) * ACOS((sin(ISNULL(@Lat1,0) / @DegToRad) * SIN(ISNULL(@Lat2,0) / @DegToRad)) +
        (COS(ISNULL(@Lat1,0) / @DegToRad) * COS(ISNULL(@Lat2,0) / @DegToRad) *
         COS(ISNULL(@Lon2,0) / @DegToRad - ISNULL(@Lon1,0)/ @DegToRad))));
END; ;
go