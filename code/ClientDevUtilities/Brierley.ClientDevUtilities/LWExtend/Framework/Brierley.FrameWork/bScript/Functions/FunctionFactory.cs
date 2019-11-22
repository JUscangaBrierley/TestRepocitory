using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The Function Factory class manages the library's internal function implementation strategy. The class is based on the factory design pattern and uses
	/// information passed by the parsing engine to determine which function to create and how to supply that function with it's parameter list.
	/// </summary>
	[Serializable]
	public static class FunctionFactory
	{
		private static string _className = "FunctionFactory";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		
		/// <summary>
		/// Static method to create a function. Used internally by the expression parsing engine.
		/// </summary>
		/// <param name="function">The Name of the function to create</param>
		/// <param name="fExpr">An expression object. This object may be of type <see cref="Expression"/> or of type <see cref="ParameterList"/> depending upon
		/// the Function being asked for.</param>
		/// <returns></returns>        
		public static Expression GetFunction(string function, Expression fExpr)
		{
			string methodName = "GetFunction";

			ParameterList plist = fExpr as ParameterList;
			Expression theExpr = null;
			switch (function.ToLower())
			{
				case "isnull":
					if (fExpr != null)
					{
						theExpr = new IsNull(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsNull function requires one parameter.");
					}
					break;
				case "year":
					if (fExpr == null)
					{
						return new Year();
					}
					else
					{
						return new Year(fExpr);
					}
				case "month":
					if (fExpr == null)
					{
						return new Month();
					}
					else
					{
						return new Month(fExpr);
					}
				case "day":
					if (fExpr == null)
					{
						return new Day();
					}
					else
					{
						return new Day(fExpr);
					}
				case "todayname":
					return new TodayName(fExpr);
				case "formatdate":
					if (fExpr != null)
					{
						theExpr = new FormatDate(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. FormatDate");
					}
					break;
				case "makedate":
					if (fExpr != null)
					{
						theExpr = new MakeDate(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. MakeDate");
					}
					break;
				case "date":
					return new Date();
				case "time":
					return new Time();
				case "isdatewithinrange":
					if (fExpr != null)
					{
						theExpr = new IsDateWithinRange(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. IsDateWithinRange");
					}
					break;
				case "isemailsuppressed":
					if (fExpr != null)
					{
						theExpr = new IsEmailSuppressed(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. IsEmailSuppressed");
					}
					break;
				case "abs":
					if (fExpr != null)
					{
						theExpr = new ABS(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. ABS");
					}
					break;
				case "round":
					if (fExpr != null)
					{
						theExpr = new Round(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. Round");
					}
					break;
				case "nextwholenumber":
					if (fExpr != null)
					{
						theExpr = new NextWholeNumber(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. NextWholeNumber.");
					}
					break;
				case "getconfigvalue":
					if (fExpr != null)
					{
						theExpr = new GetConfigValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetConfigValue");
					}
					break;
                case "getframeworkconfigvalue":
                    if (fExpr != null)
                    {
                        theExpr = new GetFrameworkConfigValue(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. GetFWConfigValue");
                    }
                    break;
				case "getpoints":
					if (fExpr != null)
					{
						theExpr = new GetPoints(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetPoints.");
					}
					break;
				case "getpointsbyevent":
					if (fExpr != null)
					{
						theExpr = new GetPointsByEvent(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetPointsByEvent.");
					}
					break;
				case "getpointsinset":
					if (fExpr != null)
					{
						theExpr = new GetPointsInSet(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetPointsInSet.");
					}
					break;
				case "getpointsbyownertype":
					if (fExpr != null)
					{
						theExpr = new GetPointsByOwnerType(fExpr);
					}
					else
					{
						theExpr = new GetPointsByOwnerType();
					}
					break;
				case "getpointsexcludingset":
					if (fExpr != null)
					{
						theExpr = new GetPointsExcludingSet(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetPointsExcludingSet.");
					}
					break;
				case "getpointsexpired":
					if (fExpr != null)
					{
						theExpr = new GetPointsExpired(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetPointsExpired.");
					}
					break;
				case "getpointssummary":
					if (fExpr != null)
					{
						theExpr = new GetPointsSummary(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetPointsSummary.");
					}
					break;
				case "getpointtransactioncount":
					theExpr = new GetPointTransactionCount(fExpr);
					break;
				case "gettotalpointsawarded":
					theExpr = new GetTotalPointsAwarded(fExpr);
					break;
				case "getearnedpoints":
					theExpr = new GetEarnedPoints(fExpr);
					break;
				case "getearnedpointsinset":
					theExpr = new GetEarnedPointsInSet(fExpr);
					break;
				case "getpointstonexttier":
					if (fExpr != null)
					{
						theExpr = new GetPointsToNextTier(fExpr);
					}
					else
					{
						theExpr = new GetPointsToNextTier();
						//throw new CRMException("Invalid function call. GetPointsToNextTier.");
					}
					break;
				case "getpointstonextreward":
					if (fExpr != null)
					{
						theExpr = new GetPointsToNextReward(fExpr);
					}
					else
					{
						theExpr = new GetPointsToNextReward();
						//throw new CRMException("Invalid function call. GetPointsToNextReward.");
					}
					break;
				case "min": // this function should have two arguments contained in parts[1]
					if (fExpr != null && plist.Expressions.Length == 2)
					{
						theExpr = new MIN(plist.Expressions[0], plist.Expressions[1]);
					}
					else
					{
						throw new CRMException("Invalid function call. The MIN function requires two parameters.");
					}
					break;
				case "max": // this function should have two arguments contained in a parameter list expression					
					if (fExpr != null && plist.Expressions.Length == 2)
					{
						theExpr = new MAX(plist.Expressions[0], plist.Expressions[1]);
					}
					else
					{
						throw new CRMException("Invalid function call. The MAX function requires two parameters.");
					}
					break;
                case "pointstorewardchoice":
                    if (fExpr != null)
                    {
                        theExpr = new PointsToRewardChoice(fExpr);
                    }
                    else
                    {
                        theExpr = new PointsToRewardChoice();
                    }
                    break;
                case "rewardchoice":
                    if (fExpr != null)
                    {
                        theExpr = new Brierley.FrameWork.bScript.Functions.RewardChoice(fExpr);
                    }
                    else
                    {
                        theExpr = new Brierley.FrameWork.bScript.Functions.RewardChoice();
                    }
                    break;

                case "strconcat":
					if (fExpr != null && plist.Expressions.Length == 2)
					{
						theExpr = new StrConcat(plist.Expressions[1], plist.Expressions[0]);
					}
					else
					{
						throw new CRMException("Invalid function call. The StrConcat function requires two parameters.");
					}
					break;
				case "weekend":
					if (fExpr != null)
					{
						theExpr = new WeekEnd(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The WEEKEND function requires one parameters.");
					}
					break;
				case "weekstart":
					if (fExpr != null)
					{
						theExpr = new WeekStart(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The WEEKSTART function requires one parameters.");
					}
					break;
				case "strcompare":
					if (fExpr != null)
					{
						theExpr = new StrCompare(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The STRCOMPARE function requires two parameters.");
					}
					break;
				case "strcontains":
					if (fExpr != null && plist.Expressions.Length == 2)
					{
						theExpr = new STRContains(plist.Expressions[0], plist.Expressions[1]);
					}
					else
					{
						throw new CRMException("Invalid function call. The STRCONTAINS function requires two parameters.");
					}
					break;
				case "strlength":
					if (fExpr != null)
					{
						theExpr = new STRLength(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The STRLENGTH function requires one parameter.");
					}
					break;
				case "tostring":
					if (fExpr != null)
					{
						theExpr = new ToString(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The ToString function requires one parameter.");
					}
					break;
				case "strsubstring":
					if (fExpr != null)
					{
						theExpr = new STRSubstring(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The STRSubstring function requires two or more parameters.");
					}
					break;
				case "isnullorempty":
					if (fExpr != null)
					{
						theExpr = new IsNullOrEmpty(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsNullOrEmpty function requires one parameter.");
					}
					break;
				case "isintier":
					if (fExpr != null)
					{
						theExpr = new IsInTier(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsInTier function requires one parameter.");
					}
					break;
				case "isitemissued":
					if (fExpr != null)
					{
						theExpr = new IsItemIssued(fExpr);
					}
					else
					{
						theExpr = new IsItemIssued();
					}
					break;
				case "ispromotionvalid":
					if (fExpr != null)
					{
						theExpr = new IsPromotionValid(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsPromotionValid function requires one parameter.");
					}
					break;
				case "isinpromotion":
					if (fExpr != null)
					{
						theExpr = new IsInPromotion(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsInPromotion function requires one parameter.");
					}
					break;
				//case "getcurrenttier":
				//    theExpr = new GetCurrentTier();
				//    break;
				case "getcurrenttierproperty":
					if (fExpr != null)
					{
						theExpr = new GetCurrentTierProperty(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetCurrentTierProperty function requires one parameter.");
					}
					break;
                case "gettierproperty":
                    if(fExpr != null)
                    {
                        theExpr = new GetTierProperty(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The GetTierProperty function requires two parameters.");
                    }
                    break;
				case "getcurrentuiculture":
					theExpr = new GetCurrentUICulture();
					break;
				case "getcurrentuilanguage":
					theExpr = new GetCurrentUILanguage();
					break;
				case "getcookievalue":
					if (fExpr != null)
					{
						theExpr = new GetCookieValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetCookieValue");
					}
					break;
				case "createcriteria":
					if (fExpr != null)
					{
						theExpr = new CreateCriteria(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. CreateCriteria");
					}
					break;
				case "rowcount":
					if (fExpr != null)
					{
						theExpr = new RowCount(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. RowCount");
					}
					break;
				case "rowcountwithcriteria":
					if (fExpr != null)
					{
						theExpr = new RowCountWithCriteria(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. RowCountWithCriteria");
					}
					break;
				case "attrvalue":
					if (fExpr != null)
					{
						theExpr = new AttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. AttrValue");
					}
					break;
				case "attrvaluebyrowkey":
					if (fExpr != null)
					{
						theExpr = new AttrValueByRowKey(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. AttrValueByRowKey");
					}
					break;
				case "resolveattributeset":
					if (fExpr != null)
					{
						theExpr = new ResolveAttributeSet(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. ResolveAttributeSet");
					}
					break;
				case "refattrvalue":
					if (fExpr != null)
					{
						theExpr = new RefAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. RefAttrValue");
					}
					break;
				case "refattrvaluebyrowkey":
					if (fExpr != null)
					{
						theExpr = new RefAttrValueByRowKey(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. RefAttrValueByRowKey");
					}
					break;
				case "exprwizset":
					if (fExpr != null)
					{
						theExpr = new ExprWizSet(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. ExprWizSet");
					}
					break;
				case "productbyidattrvalue":
					if (fExpr != null)
					{
						theExpr = new ProductByIdAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. ProductByIdAttrValue");
					}
					break;
				case "productbypartnumberattrvalue":
					if (fExpr != null)
					{
						theExpr = new ProductByPartNumberAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. ProductByPartNumberAttrValue");
					}
					break;
				case "rewarddefattrvalue":
					if (fExpr != null)
					{
						theExpr = new RewardDefAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. RewardDefAttrValue");
					}
					break;
				case "rewardcertificatecount":
					if (fExpr != null)
					{
						theExpr = new RewardCertificateCount(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. RewardCertificateCount");
					}
					break;
				case "memberrewardattrvalue":
					if (fExpr != null)
					{
						theExpr = new MemberRewardAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. MemberRewardAttrValue");
					}
					break;
				case "membertierattrvalue":
					if (fExpr != null)
					{
						theExpr = new MemberTierAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. MemberTierAttrValue");
					}
					break;
				case "initializedataiterator":
					theExpr = new InitializeDataIterator(fExpr);
					//if (fExpr != null)
					//{
					//    theExpr = new InitializeDataIterator(fExpr);
					//}
					//else
					//{
					//    throw new CRMException("Invalid function call. IntializeDataIterator");
					//}
					break;
				case "advancedatarowindex":
					theExpr = new AdvanceDataRowIndex();
					break;
				case "getcurrentdatarowindex":
					theExpr = new GetCurrentDataRowIndex();
					break;
				case "getstructuredelementdata":
					if (fExpr != null)
					{
						theExpr = new GetStructuredElementData(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. StructuredElementData");
					}
					break;
				case "getstructuredattributedata":
					if (fExpr != null)
					{
						theExpr = new GetStructuredAttributeData(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetStructuredAttributeData");
					}
					break;
				case "getrowindexbyattributevalue":
					if (fExpr != null)
					{
						theExpr = new GetRowIndexByAttributeValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetRowIndexByAttributeValue");
					}
					break;
				case "getcontentattributedatavalueset":
					if (fExpr != null)
					{
						theExpr = new GetContentAttributeDataValueSet(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetContentAttributeDataValueSet");
					}
					break;
				case "getimageurl":
					if (fExpr != null)
					{
						theExpr = new GetImageUrl(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetImageUrl");
					}
					break;
				case "htmlimage":
					if (fExpr != null)
					{
						theExpr = new HtmlImage(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. HtmlImage");
					}
					break;
				case "getlistfilevalue":
					if (fExpr != null)
					{
						theExpr = new GetListFileValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. GetListFileValue");
					}
					break;
				case "sum":
					if (fExpr != null)
					{
						theExpr = new Sum(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. Sum");
					}
					break;
				case "avg":
					if (fExpr != null)
					{
						theExpr = new Avg(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. Avg");
					}
					break;
				case "firstindexof":
					if (fExpr != null)
					{
						theExpr = new FirstIndexOf(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. FirstIndexOf");
					}
					break;
				case "lastindexof":
					if (fExpr != null)
					{
						theExpr = new LastIndexOf(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. LastIndexOf");
					}
					break;
				case "if":
					if (fExpr != null)
					{
						theExpr = new If(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. IF");
					}
					break;
				case "isinset":
					if (fExpr != null)
					{
						theExpr = new IsInSet(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. IsInSet");
					}
					break;
				case "setattrvalue":
					if (fExpr != null)
					{
						theExpr = new SetAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. SetAttrValue");
					}
					break;
				case "surveyresponse":
					if (fExpr != null)
					{
						theExpr = new SurveyResponse(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. SurveyResponse");
					}
					break;
				case "surveyurl":
					{
						theExpr = new SurveyURL(fExpr);
					}
					break;
				case "surveylink":
					if (fExpr != null)
					{
						theExpr = new SurveyLink(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. SurveyLink");
					}
					break;
				case "surveyresponsebyanswer":
					if (fExpr != null)
					{
						theExpr = new SurveyResponseByAnswer(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. SurveyResponseByAnswer");
					}
					break;
				case "surveyresponsecontains":
					if (fExpr != null)
					{
						theExpr = new SurveyResponseContains(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. SurveyResponseContains");
					}
					break;
				case "surveyconcept":
					{
						theExpr = new SurveyConcept(fExpr);
					}
					break;
				case "getrespondentproperty":
					{
						theExpr = new GetRespondentProperty(fExpr);
					}
					break;
				case "setrespondentproperty":
					{
						theExpr = new SetRespondentProperty(fExpr);
					}
					break;
				case "didrespondentviewconcept":
					{
						theExpr = new DidRespondentViewConcept(fExpr);
					}
					break;
				case "isquotametforconcept":
					{
						theExpr = new IsQuotaMetForConcept(fExpr);
					}
					break;
				case "htmllink":
					if (fExpr != null)
					{
						theExpr = new HtmlLink(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. HtmlLink");
					}
					break;
				case "htmlimagelink":
					if (fExpr != null)
					{
						theExpr = new HtmlImageLink(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. HtmlImageLink");
					}
					break;
				case "mtouchlink":
					if (fExpr != null)
					{
						theExpr = new MTouchLink(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. MTouchLink");
					}
					break;
				case "loyaltycardlink":
					{
						theExpr = new LoyaltyCardLink(fExpr);
					}
					break;
				case "couponlink":
					{
						theExpr = new CouponLink(fExpr);
					}
					break;
				case "passwordresetlink":
					if (fExpr != null)
					{
						theExpr = new PasswordResetLink(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. PasswordResetLink");
					}
					break;
				case "getenumname":
					if (fExpr != null)
					{
						theExpr = new GetEnumName(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetEnumName function requires two parameters.");
					}
					break;
				case "getenumvalue":
					if (fExpr != null)
					{
						theExpr = new GetEnumValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetEnumValue function requires two parameters.");
					}
					break;
				case "getenvironmentstring":
					if (fExpr != null)
					{
						theExpr = new GetEnvironmentString(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetEnvironmentString function requires one parameter.");
					}
					break;

				case "addmonth":
					if (fExpr != null)
					{
						theExpr = new AddMonth(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The AddMonth function requires two parameters.");
					}
					break;
				case "addday":
					if (fExpr != null)
					{
						theExpr = new AddDay(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The AddDay function requires two parameters.");
					}
					break;
                case "addyear":
                    if (fExpr != null)
                    {
                        theExpr = new AddYear(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The AddYear function requires two parameters.");
                    }
                    break;
                case "addhour":
                    if (fExpr != null)
                    {
                        theExpr = new AddHour(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The AddHour function requires two parameters.");
                    }
                    break;
                case "getmonth":
					if (fExpr != null)
					{
						theExpr = new GetMonth(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetMonth function requires one parameter.");
					}
					break;
				case "getday":
					if (fExpr != null)
					{
						theExpr = new GetDay(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetDay function requires one parameter.");
					}
					break;
				case "getyear":
					if (fExpr != null)
					{
						theExpr = new GetYear(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetYear function requires one parameter.");
					}
					break;
                case "gethour":
                    if (fExpr != null)
                    {
                        theExpr = new GetHour(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The GetHour function requires one parameter.");
                    }
                    break;
                case "getbeginningofday":
					if (fExpr != null)
					{
						theExpr = new GetBeginningOfDay(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetBeginningOfDay function requires one parameter.");
					}
					break;
				case "getendofday":
					if (fExpr != null)
					{
						theExpr = new GetEndOfDay(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetEndOfDay function requires one parameter.");
					}
					break;
				case "getfirstdateofweek":
					if (fExpr != null)
					{
						theExpr = new GetFirstDateOfWeek(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetFirstDateOfWeek function requires one parameter.");
					}
					break;
				case "getlastdateofweek":
					if (fExpr != null)
					{
						theExpr = new GetLastDateOfWeek(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetLastDateOfWeek function requires one parameter.");
					}
					break;
				case "getfirstdateofmonth":
					if (fExpr != null)
					{
						theExpr = new GetFirstDateOfMonth(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetFirstDateOfMonth function requires one parameter.");
					}
					break;
				case "getlastdateofmonth":
					if (fExpr != null)
					{
						theExpr = new GetLastDateOfMonth(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetLastDateOfMonth function requires one parameter.");
					}
					break;
				case "getfirstdateofquarter":
					if (fExpr != null)
					{
						theExpr = new GetFirstDateOfQuarter(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetFirstDateOfQuarter function requires one parameter.");
					}
					break;
				case "getlastdateofquarter":
					if (fExpr != null)
					{
						theExpr = new GetLastDateOfQuarter(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetLastDateOfQuarter function requires one parameter.");
					}
					break;
				case "getfirstdateofyear":
					if (fExpr != null)
					{
						theExpr = new GetFirstDateOfYear(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetFirstDateOfYear function requires one parameter.");
					}
					break;
				case "getlastdateofyear":
					if (fExpr != null)
					{
						theExpr = new GetLastDateOfYear(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetLastDateOfYear function requires one parameter.");
					}
					break;
				case "datediffindays":
					if (fExpr != null)
					{
						theExpr = new DateDiffInDays(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The DateDiffInDays function requires one parameter.");
					}
					break;
				case "getactiveloyaltyid":
					if (fExpr == null)
					{
						return new GetActiveLoyaltyId();
					}
					else
					{
						return new GetActiveLoyaltyId(fExpr);
					}
				case "isvirtualcardoftype":
					if (fExpr != null)
					{
						theExpr = new IsVirtualCardOfType(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsVirtualCardOfType function requires one parameter.");
					}
					break;
				case "storebyidattrvalue":
					if (fExpr != null)
					{
						theExpr = new StoreByIdAttrValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The StoreByIdAttrValue function requires parameters.");
					}
					break;
				case "getsocialmediaprofileimageurl":
					if (fExpr != null)
					{
						string typeName = "Brierley.WebFrameWork.bScript.GetSocialMediaProfileImageURL";
						System.Reflection.Assembly assembly = ClassLoaderUtil.LoadAssembly("Brierley.WebFrameWork.dll");
						if (assembly == null)
						{
							throw new CRMException("Could not resolve assembly Brierley.WebFrameWork.dll");
						}
						Type[] argTypes = new Type[] { typeof(Brierley.FrameWork.bScript.Expression) };
						object[] args = new object[1];
						args[0] = fExpr;
						theExpr = (Expression)ClassLoaderUtil.CreateInstance(assembly, typeName, argTypes, args);
					}
					else
					{
						throw new CRMException("Invalid function call. GetSocialMediaProfileImageURL");
					}
					break;
				case "csagenthasfunction":
					if (fExpr != null)
					{
						string typeName = "Brierley.WebFrameWork.bScript.CSAgentHasFunction";
						System.Reflection.Assembly assembly = ClassLoaderUtil.LoadAssembly("Brierley.WebFrameWork.dll");
						if (assembly == null)
						{
							throw new CRMException("Could not resolve assembly Brierley.WebFrameWork.dll");
						}
						Type[] argTypes = new Type[] { typeof(Expression) };
						object[] args = new Expression[1];
						args[0] = fExpr;
						theExpr = (Expression)ClassLoaderUtil.CreateInstance(assembly, typeName, argTypes, args);
					}
					else
					{
						throw new CRMException("Invalid function call. CSAgentHasFunction");
					}
					break;
				case "csagentinrole":
					if (fExpr != null)
					{
						string typeName = "Brierley.WebFrameWork.bScript.CSAgentInRole";
						System.Reflection.Assembly assembly = ClassLoaderUtil.LoadAssembly("Brierley.WebFrameWork.dll");
						if (assembly == null)
						{
							throw new CRMException("Could not resolve assembly Brierley.WebFrameWork.dll");
						}
						Type[] argTypes = new Type[] { typeof(Brierley.FrameWork.bScript.Expression) };
						object[] args = new object[1];
						args[0] = fExpr;
						theExpr = (Expression)ClassLoaderUtil.CreateInstance(assembly, typeName, argTypes, args);
					}
					else
					{
						throw new CRMException("Invalid function call. CSAgentInRole");
					}
					break;
				case "ismemberloggedin":
					theExpr = new IsMemberLoggedIn();
					break;
				case "hasbonus":
					if (fExpr != null)
					{
						theExpr = new HasBonus(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The HasBonus function requires one parameter.");
					}
					break;
				case "hascompletedbonus":
					if (fExpr != null)
					{
						theExpr = new HasCompletedBonus(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The HasCompletedBonus function requires one parameter.");
					}
					break;
				case "getviewedconceptforstate":
					if (fExpr != null)
					{
						theExpr = new GetViewedConceptForState(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetViewedConceptForState function requires one parameter.");
					}
					break;
				case "surveyreviewconcept":
					if (fExpr != null)
					{
						theExpr = new SurveyReviewConcept(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The SurveyReviewConcept function requires one or two parameters.");
					}
					break;
				case "issocialpublisher":
					if (fExpr != null)
					{
						theExpr = new IsSocialPublisher(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsSocialPublisher function requires one parameter.");
					}
					break;
				case "issocialsentiment":
					if (fExpr != null)
					{
						theExpr = new IsSocialSentiment(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The IsSocialSentiment function requires one parameter.");
					}
					break;
				case "getsocialdataproperty":
					if (fExpr != null)
					{
						theExpr = new GetSocialDataProperty(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetSocialDataProperty function requires one parameter.");
					}
					break;
				case "cachedresult":
					if (fExpr != null)
					{
						theExpr = new CachedResult(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The CachedResult function requires one parameter.");
					}

					break;
				case "campaignstepresultcount":
					if (fExpr != null)
					{
						theExpr = new CampaignStepResultCount(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The CampaignStepResultCount function requires one parameter.");
					}
					break;
				case "getcampaignattributevalue":
					if (fExpr != null)
					{
						theExpr = new GetCampaignAttributeValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The GetCampaignAttributeValue function requires one parameter.");
					}
					break;
				case "setcampaignattributevalue":
					if (fExpr != null)
					{
						theExpr = new SetCampaignAttributeValue(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The SetCampaignAttributeValue function requires one parameter.");
					}
					break;
				case "querystring":
					if (fExpr != null)
					{
						theExpr = new QueryString(fExpr);
					}
					else
					{
						throw new CRMException("Invalid function call. The QueryString function requires one parameter.");
					}
					break;
                case "hasreward":
                    if (fExpr != null)
                    {
                        theExpr = new HasReward(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The HasReward function requires one or three parameters.");
                    }
                    break;
                case "getmemberrewardcount":
                    theExpr = new GetMemberRewardCount(fExpr);
                    break;
                case "ceiling":
                    if(fExpr != null)
                    {
                        theExpr = new Ceiling(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The Ceiling function requires one parameter.");
                    }
                    break;
                case "floor":
                    if (fExpr != null)
                    {
                        theExpr = new Floor(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The Floor function requires one parameter.");
                    }
                    break;
                case "getcurrencymonetaryvalue":
                    theExpr = new GetCurrencyMonetaryValue(fExpr);
                    break;
                case "getcontentattributevalue":
                    if(fExpr != null)
                    {
                        theExpr = new GetContentAttributeValue(fExpr);
                    }
                    else
                    {
                        throw new CRMException("Invalid function call. The GetContentAttributeValue function requires one parameter.");
                    }
                    break;
                default:
					_logger.Debug(_className, methodName, "No builtin function matched " + function + ".  Trying custom functions.");
					// This is not one of the built-in functions.  Try custom bScript function.
					using (var svc = LWDataServiceUtil.DataServiceInstance())
					{
                        // TODO: Look through custom assemblies for reference components with a name = function.ToLower.
                        //       If there's a match, do a CreateInstance (with an expression arg if fExpr isn't null)
                        RemoteAssembly.ComponentReference reference = svc.GetComponentReference(CustomComponentTypeEnum.BScript, function);
                        if(reference != null)
                        {
                            _logger.Debug(_className, methodName, "Custom bScript function " + function + " found.  Now instantiating it.");
                            try
                            {
                                int idx = reference.ClassName.IndexOf(",");
                                string className = reference.ClassName.Substring(0, idx);
                                if(fExpr == null)
                                {
                                    theExpr = (Expression)ClassLoaderUtil.CreateInstance(reference.Assembly, className);
                                }
                                else
                                {
                                    Type[] argTypes = new Type[1];
                                    argTypes[0] = typeof(Expression);
                                    object[] args = new Expression[1];
                                    args[0] = fExpr;
                                    theExpr = (Expression)ClassLoaderUtil.CreateInstance(reference.Assembly, className, argTypes, args);
                                }
                            }
                            catch(Exception ex)
                            {
                                string msg = string.Format(
                                    "Error while trying to instantiate a {0}.  Error Message: {1}",
                                    function,
                                    ex.Message);
                                _logger.Error(_className, methodName, msg, ex);
                                throw new LWBScriptException("Error while instantiating custom bscript function " + function, ex);
                            }
                        }
					}
					break;
			}
			return theExpr;
		}
	}
}
