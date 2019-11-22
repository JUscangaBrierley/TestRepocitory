DELIMITER //

CREATE PROCEDURE `FindMember`(
	IN LoyaltyID varchar(255), 
	IN FirstName varchar(250), 
	IN LastName varchar(250), 
	IN Email varchar(250),
	IN NonMember bit)
BEGIN
  DECLARE operator varchar(4); 
	DECLARE whereClause varchar(2000);
	DECLARE whereExists bit;
	DECLARE sqlString varchar(4000);
		
	SET whereExists = 0;


	set sqlString = '
		SELECT 
			LM.IPCode, LM.FirstName, LM.LastName, LM.PrimaryEmailAddress, VC.LoyaltyIDNumber
		FROM
			LW_LoyaltyMember LM
			LEFT JOIN LW_VirtualCard VC on VC.IPCode = LM.IPCode
		WHERE ';

        
	IF LoyaltyID IS NOT NULL THEN
		-- searching only by loyalty id	        
			-- SET whereClause = ' VC.LoyaltyIDNumber = ' + '''' + LoyaltyID + '''';
			set whereClause = concat(' VC.LoyaltyIDNumber = ',char(39),LoyaltyID,char(39));
			SET whereExists = 1;		
	ELSE 		
			IF FirstName IS NOT NULL	THEN		
				IF(field('*', FirstName) > 1 OR field('%', FirstName) > 1) THEN
					set FirstName = replace(FirstName, '*', '');
					set FirstName = replace(FirstName, '%', '');
					set FirstName = FirstName + '%';
					SET operator = 'like';				
				ELSE				
					SET operator = '=';
				END IF;
				-- SET whereClause = 'FirstName ' + operator + ' ''' + FirstName + '''';
				set whereClause = concat('FirstName ',operator,' ',char(39),FirstName,char(39));
				SET whereExists = 1;
			END IF;

		 
			IF LastName IS NOT NULL THEN			
				IF(field('*', LastName) > 1 OR field('%', LastName) > 1)	THEN			
					set LastName = replace(LastName, '*', '');
					set LastName = replace(LastName, '%', '');
					set LastName = LastName + '%';
					SET operator = 'like';				
				ELSE				
					SET operator = '=';
				END IF;
			
				IF(whereExists = 1) THEN												
					set whereClause = concat(whereClause,' AND ','LastName ',operator,' ',char(39),LastName,char(39));
				ELSE
				        set whereClause = concat('LastName ',operator,' ',char(39),LastName,char(39));
				END IF;
							
				SET whereExists = 1;
			END IF;


			IF NOT Email IS NULL THEN
				IF(field('*', Email) > 1 OR field('%', Email) > 1) THEN
					set Email = replace(Email, '*', '');
					set Email = replace(Email, '%', '');
					set Email = Email + '%';
					SET operator = 'like';
				ELSE
					SET operator = '=';				
				END IF;
			
				IF(whereExists = 1) THEN
					set whereClause = concat(whereClause,' AND ','PrimaryEmailAddress ',operator,' ',char(39),Email,char(39));
			        ELSE
				        set whereClause = concat('PrimaryEmailAddress ',operator,' ',char(39),Email,char(39));                    
				END IF;
							
				-- SET whereClause = whereClause + 'PrimaryEmailAddress ' + operator + ' ''' + Email + '''';
				SET whereExists = 1;
			END IF;
		END IF;
		

	-- SET whereClause = sqlString + whereClause;
	-- set whereClause = concat(sqlString,whereClause);
	set @stmtStr = concat(sqlString,whereClause);
 
	-- select whereClause;
	prepare stm from @stmtStr;
	execute stm;


END //