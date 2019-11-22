using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Brierley.FrameWork.bScript
{
	/// <summary>
	/// The function names supported by bScript
	/// </summary>
	[Serializable]
	public enum FunctionNames
	{
		/// <summary>
		/// IsNull
		/// </summary>
		ISNULL,

		/// <summary>
		/// The Min Function Min(x,y)
		/// </summary>
		MIN,

		/// <summary>
		/// The Max Function Max(x,y)
		/// </summary>
		MAX,

		/// <summary>
		/// Absolute Value Abs(x)
		/// </summary>
		ABS,

		/// <summary>
		/// Today Name function TodayName()
		/// </summary>
		TODAYNAME,

		/// <summary>
		/// The Date Function Date()
		/// </summary>
		DATE,

		/// <summary>
		/// The date formating function()
		/// </summary>
		FORMATDATE,

        /// <summary>
        /// The date creation function()
        /// </summary>
        MAKEDATE,

		/// <summary>
		/// The Time function Time()
		/// </summary>
		TIME,

		/// <summary>
		/// The WeekEnd Function WeekEnd('date')
		/// </summary>
		WEEKEND,

		/// <summary>
		/// The WeekStart function WeekStart('date')
		/// </summary>
		WEEKSTART,

		/// <summary>
		/// The IsDateWithinRange function 
		/// </summary>
		ISDATEWITHINRANGE,

		ISEMAILSUPPRESSED,

		/// <summary>
		/// The StrCompare function
		/// </summary>
		STRCOMPARE, 

		/// <summary>
		/// The String Contains Function StrContains('string','string')
		/// </summary>
		STRCONTAINS,

		/// <summary>
		/// The string Length Function StrLength('string')
		/// </summary>
		STRLENGTH,

		/// <summary>
		/// The string Length Function ToString('string')
		/// </summary>
		TOSTRING,

		/// <summary>
		/// concatenates two strings.
		/// </summary>
		STRCONCAT,

		/// <summary>
		/// gets a substring given index.
		/// </summary>
		STRSUBSTRING,

		/// <summary>
		/// Returns true if the string that this expression evaluates to is null or empty.
		/// </summary>
		ISNULLOREMPTY,

		/// <summary>
		/// Returns the named configuration value from client configuration
		/// </summary>
		GETCONFIGVALUE,

        /// <summary>
        /// Returns the named configuration value from framework configuration
        /// </summary>
        GETFRAMEWORKCONFIGVALUE,

		/// <summary>
		/// Rounds a number using Math.Round
		/// </summary>
		ROUND,

		/// <summary>
		/// Returns the next whole number given a given a fraction of the x.y (5.2 = 6)
		/// </summary>
		NEXTWHOLENUMBER,

		/// <summary>
		/// Returns the point value for a given date range and point type if supplied
		/// </summary>
		GETPOINTS,

        /// <summary>
        /// Returns the point value for a given date range and point type if supplied
        /// </summary>
        GETPOINTSSUMMARY,

        /// <summary>
        /// Returns the point value for a given owner type
        /// </summary>
        GETPOINTSBYOWNERTYPE,

		/// <summary>
		/// Returns the point value for a given date range and point event
		/// </summary>
		GETPOINTSBYEVENT,

		/// <summary>
		/// Returns the point value for a given date range and a set of point types
		/// </summary>
		GETPOINTSINSET,

		/// <summary>
		/// Returns the point value for a given date range and excluding set of point types
		/// </summary>
		GETPOINTSEXCLUDINGSET,

		/// <summary>
		/// Returns the point value for a given date range and point type if supplied
		/// </summary>
		GETPOINTSEXPIRED,

        /// <summary>
        /// Returns the number of point transactions
        /// </summary>
        GETPOINTTRANSACTIONCOUNT,

        /// <summary>
        /// Returns the total awrded points
        /// </summary>
        GETTOTALPOINTSAWARDED,

		/// <summary>
		/// Returns the earned points
		/// </summary>
		GETEARNEDPOINTS,

		/// <summary>
		/// Returns the earned points
		/// </summary>
		GETEARNEDPOINTSINSET,

		/// <summary>
		/// Returns the point required to move to the next tier
		/// </summary>
		GETPOINTSTONEXTTIER,

		/// <summary>
		/// Returns the point required to move to the next tier
		/// </summary>
		GETPOINTSTONEXTREWARD,

        /// <summary>
        /// Returns the number of points required to earn the next chosen reward
        /// </summary>
        POINTSTOREWARDCHOICE, 

		/// <summary>
		/// Returns true if the member is in the named tier. False otherwise.
		/// </summary>
		ISINTIER,

		/// <summary>
		/// returns true if a specified item has been issued within the current rule execution context
		/// </summary>
		ISITEMISSUED, 

        ///// <summary>
        ///// Returns the count of Rows that match a given expression from an attribute set.
        ///// </summary>
        //GETCURRENTTIER,

        /// <summary>
        /// Returns the count of Rows that match a given expression from an attribute set.
        /// </summary>
        GETCURRENTTIERPROPERTY,

        /// <summary>
        /// Returns the property value of a specified tier
        /// </summary>
        GETTIERPROPERTY,

		/// <summary>
		/// Creates a criteria string.
		/// </summary>
		CREATECRITERIA,

		/// <summary>
		/// Returns the current tier of the member.
		/// </summary>
		ROWCOUNT,

		/// <summary>
		/// Returns the number of rows that match a criteria.
		/// </summary>
		ROWCOUNTWITHCRITERIA,

		/// <summary>
		/// Returns the value of the named attribute from the named attribute set.
		/// </summary>
		ATTRVALUE,

		/// <summary>
		/// Returns the value of the named attribute from the named attribute set.
		/// </summary>
		ATTRVALUEBYROWKEY,

		/// <summary>
		/// Returns the sum of a named attribute from the named attribute set
		/// </summary>
		RESOLVEATTRIBUTESET,

		/// <summary>
		/// Returns the sum of a named attribute from the named attribute set
		/// </summary>
		SUM,

		/// <summary>
		/// Returns the average of a named attribute from the named attribute set.
		/// </summary>
		AVG,

		/// <summary>
		/// Returns the row index number for the first row in a set that matches the expression.
		/// </summary>
		FIRSTINDEXOF,

		/// <summary>
		/// Returns the row index number for the last row in a set that matches the expression.
		/// </summary>
		LASTINDEXOF,

		/// <summary>
		/// Evaluates the first expression and if true returns the value of the second expression
		/// otherwise it returns the value of the third expression.
		/// </summary>
		IF,

		/// <summary>
		/// Returns true if the value of the first expression is contained within the set given by
		/// expression two.
		/// </summary>
		ISINSET,

		/// <summary>
		/// Returns true if the promotion is valid
		/// </summary>
		ISPROMOTIONVALID,

		/// <summary>
		/// Returns true if the member is in the specified promotion list
		/// </summary>
		ISINPROMOTION,

		/// <summary>
		/// Sets the value of an attribute within an attribute set.
		/// </summary>
		SETATTRVALUE,

		/// <summary>
		/// 
		/// </summary>
		YEAR,

		/// <summary>
		/// 
		/// </summary>
		MONTH,

		/// <summary>
		/// 
		/// </summary>
		DAY,

		/// <summary>
		/// 
		/// </summary>
		REFATTRVALUE,

        /// <summary>
        /// 
        /// </summary>
        REFATTRVALUEBYROWKEY,

		/// <summary>
		/// 
		/// </summary>
		EXPRWIZSET,

		/// <summary>
		/// 
		/// </summary>
		PRODUCTBYIDATTRvALUE,

		/// <summary>
		/// 
		/// </summary>
		PRODUCTBYPARTNUMBERATTRvALUE,

		/// <summary>
		/// 
		/// </summary>
		REWARDDEFATTRVALUE,

		/// <summary>
		/// 
		/// </summary>
		REWARDCERTIFICATECOUNT,

		/// <summary>
		/// 
		/// </summary>
		MEMBERREWARDATTRVALUE,

		/// <summary>
		/// 
		/// </summary>
		MEMBERTIERATTRVALUE,

		/// <summary>
		/// 
		/// </summary>
		INITIALIZEDATAITERATOR,

		/// <summary>
		/// 
		/// </summary>
		ADVANCEDATAROWINDEX,

		/// <summary>
		/// 
		/// </summary>
		GETCURRENTDATAROWINDEX,

		/// <summary>
		/// 
		/// </summary>
		GETSTRUCTUREDELEMENTDATA,

		/// <summary>
		/// 
		/// </summary>
		GETSTRUCTUREDATTRIBUTEDATA,

		/// <summary>
		/// 
		/// </summary>
		GETLISTFILEVALUE,

		/// <summary>
		/// 
		/// </summary>
		GETIMAGEURL,

		/// <summary>
		/// 
		/// </summary>
		HTMLIMAGE,

		/// <summary>
		/// 
		/// </summary>
		GETROWINDEXBYATTRIBUTEVALUE,

		/// <summary>
		/// 
		/// </summary>
		GETCONTENTATTRIBUTEDATAVALUESET,

		/// <summary>
		/// 
		/// </summary>
		SURVEYRESPONSE,

		/// <summary>
		/// 
		/// </summary>
		SURVEYURL,

		/// <summary>
		/// 
		/// </summary>
		SURVEYLINK,

		/// <summary>
		/// 
		/// </summary>
		SURVEYRESPONSEBYANSWER,

		/// <summary>
		/// 
		/// </summary>
		SURVEYRESPONSECONTAINS,

		/// <summary>
		/// 
		/// </summary>
		SURVEYCONCEPT,

		/// <summary>
		/// 
		/// </summary>
		GETRESPONDENTPROPERTY,

		/// <summary>
		/// 
		/// </summary>
		SETRESPONDENTPROPERTY,

		/// <summary>
		/// 
		/// </summary>
		DIDRESPONDENTVIEWCONCEPT,

		/// <summary>
		/// 
		/// </summary>
		ISQUOTAMETFORCONCEPT,

        /// <summary>
        /// 
        /// </summary>
        HTMLLINK,

		/// <summary>
		/// 
		/// </summary>
		HTMLIMAGELINK,

		/// <summary>
		/// 
		/// </summary>
		MTOUCHLINK,

		/// <summary>
		/// 
		/// </summary>
		LOYALTYCARDLINK,

		/// <summary>
		/// 
		/// </summary>
		COUPONLINK,

		/// <summary>
		/// 
		/// </summary>
		PASSWORDRESETLINK,

		/// <summary>
		/// 
		/// </summary>
		GETENUMNAME,

		/// <summary>
		/// 
		/// </summary>
		GETENUMVALUE,

		/// <summary>
		/// 
		/// </summary>
		ADDMONTH,

		/// <summary>
		/// 
		/// </summary>
		ADDDAY,

		/// <summary>
		/// 
		/// </summary>
		ADDYEAR,

        ADDHOUR, 

		/// <summary>
		/// 
		/// </summary>
		GETMONTH,

		/// <summary>
		/// 
		/// </summary>
		GETDAY,

		/// <summary>
		/// 
		/// </summary>
		GETYEAR,

        GETHOUR, 

		/// <summary>
		/// 
		/// </summary>
		GETBEGINNINGOFDAY,

		/// <summary>
		/// 
		/// </summary>
		GETENDOFDAY,

		/// <summary>
		/// 
		/// </summary>
		GETFIRSTDATEOFWEEK,

		/// <summary>
		/// 
		/// </summary>
		GETLASTDATEOFWEEK,

		/// <summary>
		/// 
		/// </summary>
		GETFIRSTDATEOFMONTH,

		/// <summary>
		/// 
		/// </summary>
		GETENVIRONMENTSTRING,

		/// <summary>
		/// 
		/// </summary>
		GETLASTDATEOFMONTH,

		/// <summary>
		/// 
		/// </summary>
		GETFIRSTDATEOFQUARTER,

		/// <summary>
		/// 
		/// </summary>
		GETLASTDATEOFQUARTER,

		/// <summary>
		/// 
		/// </summary>
		GETFIRSTDATEOFYEAR,

		/// <summary>
		/// 
		/// </summary>
		GETLASTDATEOFYEAR,

		/// <summary>
		/// 
		/// </summary>
		DATEDIFFINDAYS,

		/// <summary>
		/// Gets the member's current active Loyalty ID Number from VirtualCard.
		/// </summary>
		GETACTIVELOYALTYID,

		/// <summary>
		/// Determines if the loyalty card is of a certain type.
		/// </summary>
		ISVIRTUALCARDOFTYPE,

		/// <summary>
		/// Current store name
		/// </summary>
		STOREBYIDATTRVALUE,

		/// <summary>
		/// Current store name
		/// </summary>
		STORENAME,

		/// <summary>
		/// Store number of current store
		/// </summary>
		STORENUMBER,

		/// <summary>
		/// Address of current store
		/// </summary>
		STOREADDRESS,

		/// <summary>
		/// City/State/Zip of current store
		/// </summary>
		STORECITYSTATEZIP,

		/// <summary>
		/// Get SocialMedia Profile Image URL
		/// </summary>
		[ExpressionAssemblyAttribute("Brierley.WebFrameWork.dll", "Brierley.WebFrameWork.bScript.GetSocialMediaProfileImageURL")]
		GETSOCIALMEDIAPROFILEIMAGEURL,

        /// <summary>
        /// Get CSAgentHasFunction Profile Image URL
        /// </summary>
        [ExpressionAssemblyAttribute("Brierley.WebFrameWork.dll", "Brierley.WebFrameWork.bScript.CSAgentHasFunction")]
        CSAGENTHASFUNCTION,

        /// <summary>
        /// Get CSAgentInRole Profile Image URL
        /// </summary>
        [ExpressionAssemblyAttribute("Brierley.WebFrameWork.dll", "Brierley.WebFrameWork.bScript.CSAgentInRole")]
        CSAGENTINROLE,

		/// <summary>
		/// 
		/// </summary>
		GETCURRENTUICULTURE,

		/// <summary>
		/// 
		/// </summary>
		GETCURRENTUILANGUAGE,

		/// <summary>
		/// 
		/// </summary>
		GETCOOKIEVALUE,

		/// <summary>
		/// 
		/// </summary>
		ISMEMBERLOGGEDIN, 

		HASBONUS, 

		HASCOMPLETEDBONUS,

		GETVIEWEDCONCEPTFORSTATE,

		SURVEYREVIEWCONCEPT,

        ISSOCIALPUBLISHER,

        ISSOCIALSENTIMENT,

        GETSOCIALDATAPROPERTY,

		CACHEDRESULT, 

		CAMPAIGNSTEPRESULTCOUNT, 

		GETCAMPAIGNATTRIBUTEVALUE, 

		SETCAMPAIGNATTRIBUTEVALUE, 

		QUERYSTRING,

        HASREWARD, 

        REWARDCHOICE,

        CEILING,

        FLOOR,

        GETCURRENCYMONETARYVALUE,

        GETCONTENTATTRIBUTEVALUE,

        GETMEMBERREWARDCOUNT
    };

	/// <summary>
	/// Supported symbols used to represent the basic math logical operations.
	/// </summary>
	[Serializable]
	public struct Symbols
	{
		/// <summary>
		/// Logical AND &#40;&#38;#41;
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Performs a logical \"AND\"",
			DisplayName = "LogicalAnd (AND)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.Operator, WizardDescription = "AND")]
		public static string AND = "&";

		/// <summary>
		/// Logical OR (|)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Performs a logical \"OR\"",
			DisplayName = "LogicalOr (|)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.Operator, WizardDescription = "OR")]
		public static string OR = "|";

		/// <summary>
		/// Logical NOT (!)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Performs a logical \"NOT\"",
			DisplayName = "LogicalNot (!)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = "NOT")]
		public static string NOT = "!";

		/// <summary>
		/// Sum (+)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Adds one number to another.",
			DisplayName = "Sum (+)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.MathOperator, WizardDescription = "+")]
		public static string SUM = "+";

		/// <summary>
		/// Difference (-)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Subtracts one number from another.",
			DisplayName = "Difference (-)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.MathOperator, WizardDescription = "-")]
		public static string DIFFERENCE = "-";

		/// <summary>
		/// Quotient (/)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Divides one number by another.",
			DisplayName = "Difference (/)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.MathOperator, WizardDescription = "/")]
		public static string QUOTIENT = "/";

		/// <summary>
		/// Product (*)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "Multiplies one number by another.",
			DisplayName = "Product (*)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.MathOperator, WizardDescription = "*")]
		public static string PRODUCT = "*";

		/// <summary>
		/// Greater than (&gt;)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "The Greater Than operator",
			DisplayName = "GreaterThan (>)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = ">")]
		public static string GREATER = ">";

		/// <summary>
		/// Less Than (&lt;)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "The Less Than operator",
			DisplayName = "LessThan (<)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = "<")]
		public static string LESS = "<";

		/// <summary>
		/// Greater than or equal (&gt;=)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "The Greater Than or equal to operator",
			DisplayName = "GreaterEqual (>=)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = ">=")]
		public static string GREATEREQUAL = ">=";

		/// <summary>
		/// Less then or equal (&lt;=)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "The Less Than or Equal to operator",
			DisplayName = "LessEqual (<=)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = "<=")]
		public static string LESSEQUAL = "<=";

		/// <summary>
		/// Is Equal (==)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "The equality operator",
			DisplayName = "Equals (==)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = "=")]
		public static string EQUALS = "==";

		/// <summary>
		/// Is not equal (&lt;&gt;)
		/// </summary>
		[Browsable(true)]
		[ExpressionContext(Description = "The Not Equal Operator",
			DisplayName = "NotEqual (<>)",
			ExcludeContext = 0,
			ExpressionType = ExpressionTypes.Operator,
			WizardCategory = WizardCategories.LogicalOperator, WizardDescription = "<>")]
		public static string NOTEQUAL = "<>";

		/// <summary>
		/// Parameter seperator (,)
		/// </summary>
		public static string PARAMETERSEPERATOR = ",";

		/// <summary>
		/// Assignment operator (:=) NOT YET IMPLEMENTED
		/// </summary>
		public static string ASSIGNMENT = ":=";

	}

	/// <summary>
	/// Symbols used in XSLT/XPath expressions
	/// </summary>
	[Serializable]
	public struct XSLTSymbols
	{
		/// <summary>
		/// Logical AND (and)
		/// </summary>
		public static string AND = "and";
		/// <summary>
		/// Logical OR (or)
		/// </summary>
		public static string OR = "or";
		/// <summary>
		/// Logical NOT (!)
		/// </summary>
		public static string NOT = "!";
		/// <summary>
		/// Sum (+)
		/// </summary>
		public static string SUM = "+";
		/// <summary>
		/// Difference (-)
		/// </summary>
		public static string DIFFERENCE = "-";
		/// <summary>
		/// Quotient (/)
		/// </summary>
		public static string QUOTIENT = "/";
		/// <summary>
		/// Product (*)
		/// </summary>
		public static string PRODUCT = "*";
		/// <summary>
		/// Greater than (&gt;)
		/// </summary>
		public static string GREATER = ">";
		/// <summary>
		/// Less Than (&lt;)
		/// </summary>
		public static string LESS = "<";
		/// <summary>
		/// Greater than or equal (&gt;=)
		/// </summary>
		public static string GREATEREQUAL = ">=";
		/// <summary>
		/// Less then or equal (&lt;=)
		/// </summary>
		public static string LESSEQUAL = "<=";
		/// <summary>
		/// Is Equal (=)
		/// </summary>
		public static string EQUALS = "=";
		/// <summary>
		/// Is not equal (!=)
		/// </summary>
		public static string NOTEQUAL = "!=";
		/// <summary>
		/// Parameter seperator (,)
		/// </summary>
		public static string PARAMETERSEPERATOR = ",";
		/// <summary>
		/// Assignment operator (:=) NOT YET IMPLEMENTED
		/// </summary>
		public static string ASSIGNMENT = ":=";
	}
}
