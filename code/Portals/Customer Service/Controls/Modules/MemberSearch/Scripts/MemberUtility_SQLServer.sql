CREATE PROCEDURE [dbo].[FindMember](
	@LoyaltyID varchar(255) = null, 
	@FirstName varchar(250) = null, 
	@LastName varchar(250) = null, 
	@Email varchar(250) = null,
	@NonMember bit = true)

AS
DECLARE
	@operator varchar(4), 
	@whereClause varchar(2000), 
	@whereExists bit, 
	@sqlString varchar(4000)
BEGIN
	SET NOCOUNT ON;

	SET @whereExists = 0


	set @sqlString = '
		SELECT 
			LM.IPCode, LM.FirstName, LM.LastName, LM.PrimaryEmailAddress, VC.LoyaltyIDNumber
		FROM
			LW_LoyaltyMember LM
			LEFT JOIN LW_VirtualCard VC on VC.IPCode = LM.IPCode
		WHERE '

        
	IF @LoyaltyID IS NOT NULL
		--searching only by loyalty id
		BEGIN
			SET @whereClause = ' VC.LoyaltyIDNumber = ' + '''' + @LoyaltyID + '''';
			SET @whereExists = 1;
		END
	ELSE 
		BEGIN
			IF @FirstName IS NOT NULL
			BEGIN
				IF(charindex('*', @FirstName) > 1 OR charindex('%', @FirstName) > 1)
				BEGIN
					set @FirstName = replace(@FirstName, '*', '')
					set @FirstName = replace(@FirstName, '%', '')
					set @FirstName = @FirstName + '%'
					SET @operator = 'like';
				END
				ELSE
				BEGIN
					SET @operator = '=';
				END
				SET @whereClause = 'FirstName ' + @operator + ' ''' + @FirstName + '''';
				SET @whereExists = 1;
			END

		 
			IF NOT @LastName IS NULL
			BEGIN
				IF(charindex('*', @LastName) > 1 OR charindex('%', @LastName) > 1)
				BEGIN
					set @LastName = replace(@LastName, '*', '')
					set @LastName = replace(@LastName, '%', '')
					set @LastName = @LastName + '%'
					SET @operator = 'like';
				END
				ELSE
				BEGIN
					SET @operator = '=';
				END

				IF(@whereExists = 1)
				BEGIN
					SET @whereClause = @whereClause + ' AND ';
				END
				SET @whereClause = @whereClause + 'LastName ' + @operator + ' ''' + @LastName + '''';
				SET @whereExists = 1;
			END


			IF NOT @Email IS NULL
			BEGIN
				IF(charindex('*', @Email) > 1 OR charindex('%', @Email) > 1)
				BEGIN
					set @Email = replace(@Email, '*', '')
					set @Email = replace(@Email, '%', '')
					set @Email = @Email + '%'
					SET @operator = 'like';
				END
				ELSE
				BEGIN
					SET @operator = '=';
				END

				IF(@whereExists = 1)
				BEGIN
					SET @whereClause = @whereClause + ' AND ';
				END
				SET @whereClause = @whereClause + 'PrimaryEmailAddress ' + @operator + ' ''' + @Email + '''';
				SET @whereExists = 1;
			END
		END


	SET @whereClause = @sqlString + @whereClause
 
	exec(@whereClause)

	--remove this line if not debugging
	print @whereClause
	SET NOCOUNT OFF


END