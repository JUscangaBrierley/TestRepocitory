using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class GetLoyaltyCurrencyBalance : OperationProviderBase
	{
		public GetLoyaltyCurrencyBalance() : base("GetLoyaltyCurrencyBalance") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				Member member = LoadMember(args);

				string[] cardIds = args.ContainsKey("CardIDs") ? (string[])args["CardIDs"] : null;
				string[] pointTypeNames = args.ContainsKey("LoyaltyCurrencyNames") ? (string[])args["LoyaltyCurrencyNames"] : null;
				string[] pointEventNames = args.ContainsKey("LoyaltyEventNames") ? (string[])args["LoyaltyEventNames"] : null;
				DateTime? startDate = args.ContainsKey("ActivityStartDate") ? (DateTime?)args["ActivityStartDate"] : null;
				DateTime? endDate = args.ContainsKey("ActivityEndDate") ? (DateTime?)args["ActivityEndDate"] : null;
				DateTime? awardStartDate = args.ContainsKey("PointAwardStartDate") ? (DateTime?)args["PointAwardStartDate"] : null;
				DateTime? awardEndDate = args.ContainsKey("PointAwardEndDate") ? (DateTime?)args["PointAwardEndDate"] : null;
				string changedBy = args.ContainsKey("AwardedBy") ? (string)args["AwardedBy"] : null;
				string locationId = args.ContainsKey("Location") ? (string)args["Location"] : null;
				string ownerTypeStr = args.ContainsKey("OwnerType") ? (string)args["OwnerType"] : null;
				long? ownerId = args.ContainsKey("OwnerId") ? (long?)args["OwnerId"] : null;
				long[] rowKeys = args.ContainsKey("RowKeys") ? (long[])args["RowKeys"] : null;
				bool earnedOnly = args.ContainsKey("EarnedPointsOnly") ? (bool)args["EarnedPointsOnly"] : false;
                bool includeExpiredPoints = args.ContainsKey("IncludeExpiredPoints") ? (bool)args["IncludeExpiredPoints"] : false;

                List <long> vcList = new List<long>();
				if (cardIds == null)
				{
					// all cards
					foreach (VirtualCard vc in member.LoyaltyCards)
					{
						if (!vc.IsValid())
						{
							continue;
						}
						vcList.Add(vc.VcKey);
					}
				}
				else
				{
					// specified cards
					foreach (string cardId in cardIds)
					{
						VirtualCard vc = member.GetLoyaltyCard(cardId);
						if (!vc.IsValid())
						{
							// TODO: throw an exception here.                            
						}
						else
						{
							vcList.Add(vc.VcKey);
						}
					}
				}
				long[] vcKeys = vcList.ToArray<long>();

				long[] pointTypeIds = null;
				if (pointTypeNames != null && pointTypeNames.Length > 0)
				{
					IList<PointType> ptList = LoyaltyDataService.GetPointTypes(pointTypeNames);
					if (ptList.Count < pointTypeNames.Length)
					{
						throw new LWOperationInvocationException("Unable to find loyalty currencies.") { ErrorCode = 3311 };
					}
					if (ptList.Count > 0)
					{
						pointTypeIds = new long[ptList.Count];
						int idx = 0;
						foreach (PointType pt in ptList)
						{
							pointTypeIds[idx++] = pt.ID;
						}
					}
				}

				long[] pointEventIds = null;
				if (pointEventNames != null && pointEventNames.Length > 0)
				{
					IList<PointEvent> peList = LoyaltyDataService.GetPointEvents(pointEventNames);
					if (peList.Count < pointEventNames.Length)
					{
						throw new LWOperationInvocationException("Unable to find loyalty events.") { ErrorCode = 3310 };
					}
					if (peList.Count > 0)
					{
						pointEventIds = new long[peList.Count];
						int idx = 0;
						foreach (PointEvent pe in peList)
						{
							pointEventIds[idx++] = pe.ID;
						}
					}
				}

				PointTransactionOwnerType? ownerType = null;
				if (!string.IsNullOrEmpty(ownerTypeStr))
				{
					try
					{
						ownerType = (PointTransactionOwnerType)Enum.Parse(typeof(PointTransactionOwnerType), ownerTypeStr);
					}
					catch (ArgumentException)
					{
						// invalid owner type specified.
					}
				}
				else
				{
					ownerId = null;
					rowKeys = null;
				}

				decimal balance = 0;
				if (!earnedOnly)
				{
					balance = LoyaltyDataService.GetPointBalance(vcKeys, pointTypeIds, pointEventIds, null, startDate, endDate, awardStartDate, awardEndDate, changedBy, locationId, ownerType, ownerId, rowKeys, includeExpiredPoints);
				}
				else
				{
					PointBankTransactionType[] tt = { PointBankTransactionType.Credit, PointBankTransactionType.Debit };
					balance = LoyaltyDataService.GetPointBalance(vcKeys, pointTypeIds, pointEventIds, tt, startDate, endDate, awardStartDate, awardEndDate, changedBy, locationId, ownerType, ownerId, rowKeys, includeExpiredPoints);
				}

				APIArguments resultParams = new APIArguments();
				resultParams.Add("CurrencyBalance", balance);

				response = SerializationUtils.SerializeResult(Name, Config, resultParams);
				return response;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
