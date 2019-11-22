using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberDao : DaoBase<Member>
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private const string _className = "MemberDao";

		public MemberDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public override void Create(Member member)
		{
			using (var txn = Database.GetTransaction())
			{
				SaveEntity(member);
				foreach (VirtualCard card in member.LoyaltyCards)
				{
					if (card.MyKey == -1)
					{
						CreateVirtualCard(member, card);
					}
					else
					{
						// this should be considered an error.
						string err = string.Format(
							"A virtual card found in the member with ipcode {0} that already has a key {1}",
							member.IpCode,
							card.MyKey);
						throw new LWDataServiceException(err);
					}
				}
				txn.Complete();
			}
		}

		public void Create(IList<Member> members)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				for (int i = 0; i < members.Count; i++)
				{
					Member m = members[i];
					SaveEntity(m);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
				}
				SaveVirtualCards(members);
				if (txn != null)
				{
					txn.Complete();
					txn.Dispose();
					txn = null;
				}
			}
			catch
			{
				if (txn != null)
				{
					txn.Dispose();
				}
				throw;
			}
		}

		public override void Update(Member member)
		{
			using (var txn = Database.GetTransaction())
			{
				if (member.IsDirty)
				{
					UpdateEntity(member);
				}
				foreach (VirtualCard card in member.LoyaltyCards)
				{
					if (card.IsDirty)
					{
						if (card.MyKey == -1)
						{
							CreateVirtualCard(member, card);
						}
						else
						{
							UpdateVirtualCard(card);
						}
						card.IsDirty = false;
					}
				}
				txn.Complete();
			}
		}

		public void Update(IList<Member> members)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				for (int i = 0; i < members.Count; i++)
				{
					Member m = members[i];
					UpdateEntity(m);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
				}
				SaveVirtualCards(members);
				if (txn != null)
				{
					txn.Complete();
					txn.Dispose();
					txn = null;
				}
			}
			catch
			{
				if (txn != null)
				{
					txn.Dispose();
				}
				throw;
			}
		}

		public Member Retrieve(long ipcode, bool retrieveCards)
		{
			Member member = Database.FirstOrDefault<Member>("where ipcode = @0", ipcode);
			if (member != null && retrieveCards)
			{
				RetrieveVirtualCards(member);
			}
			return member;
		}

		//todo: This doesn't make any sense. Method suggests we're checking for the existence of a member with a certain loyalty id, but we're actually checking
		//loyalty id belonging to another member (<> ipcode). could have been named better - consider renaming this.
		public bool MemberWithLoyaltyIDExists(long ipCode, string loyaltyIDNumber)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_VirtualCard v where v.LoyaltyIdNumber = @0 and IpCode <> @1", loyaltyIDNumber, ipCode) > 0;
		}

		public Member RetrieveByLoyaltyIDNumber(string loyaltyIDNumber, bool retrieveCards)
		{
			Member member = Database.FirstOrDefault<Member>("select m.* from LW_LoyaltyMember m inner join LW_VirtualCard v on v.ipcode = m.ipcode where v.LoyaltyIdNumber = @0", loyaltyIDNumber);
			if (member != null)
			{
				if (retrieveCards)
				{
					RetrieveVirtualCards(member);
				}
				member.IsDirty = false;
			}
			return member;
		}

		public Member RetrieveByVcKey(long vcKey, bool retrieveCards)
		{
			Member member = Database.FirstOrDefault<Member>("select m.* from LW_LoyaltyMember m inner join LW_VirtualCard v on v.ipcode = m.ipcode where v.VcKey = @0", vcKey);
			if (member != null)
			{
				if (retrieveCards)
				{
					RetrieveVirtualCards(member);
				}
				member.IsDirty = false;
			}
			return member;
		}

		public bool MemberWithEmailAddressExists(long ipCode, string emailAddress)
		{
			if (!string.IsNullOrEmpty(emailAddress))
			{
				emailAddress = emailAddress.ToLower();
			}
			return Database.ExecuteScalar<int>("select count(*) from LW_LoyaltyMember where lower(PrimaryEmailAddress) = @0 and IpCode <> @1", emailAddress, ipCode) > 0;
		}

		public Member RetrieveByEmailAddress(string emailAddress, bool retrieveCards)
		{
			Member member = Database.FirstOrDefault<Member>("select * from LW_LoyaltyMember where lower(PrimaryEmailAddress) = lower(@0)", emailAddress);
			if (member != null)
			{
				if (retrieveCards)
				{
					RetrieveVirtualCards(member);
				}
				member.IsDirty = false;
			}
			return member;
		}

		public bool MemberWithAlternateIDExists(long ipCode, string alternateId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_LoyaltyMember where AlternateId = @0 and IpCode <> @1", alternateId, ipCode) > 0;
		}

		public Member RetrieveByAlternateID(string alternateID, bool retrieveCards)
		{
			Member member = Database.FirstOrDefault<Member>("where AlternateId = @0", alternateID);
			if (member != null)
			{
				if (retrieveCards)
				{
					RetrieveVirtualCards(member);
				}
				member.IsDirty = false;
			}
			return member;
		}

		public bool MemberWithUserNameExists(long ipCode, string username)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_LoyaltyMember where Username = @0 and IpCode <> @1", username, ipCode) > 0;
		}

		public Member RetrieveByUserName(string username, bool retrieveCards)
		{
			Member member = Database.FirstOrDefault<Member>("where lower(Username) = lower(@0)", username);
			if (member != null)
			{
				if (retrieveCards)
				{
					RetrieveVirtualCards(member);
				}
				member.IsDirty = false;
			}
			return member;
		}

		public List<Member> RetrieveByName(string firstName, string lastName, string middleName, LWQueryBatchInfo batchInfo)
		{
			var parameters = new object[] { firstName, lastName, middleName };
			string where = string.Empty;

			if (!string.IsNullOrEmpty(firstName))
			{
				where += "FirstName = @0";
			}
			if (!string.IsNullOrEmpty(lastName))
			{
				where += string.Format("{0}LastName = @1", where.Length > 0 ? " and " : string.Empty);
			}
			if (!string.IsNullOrEmpty(middleName))
			{
				where += string.Format("{0}MiddleName = @2", where.Length > 0 ? " and " : string.Empty);
			}

			if (string.IsNullOrEmpty(where))
			{
				//stop this or we'll load the entire database
				throw new InvalidOperationException("At least one parameter must be passed to RetrieveByName: firstName, middleName or lastName");
			}

            string sql = "select m.* from LW_LoyaltyMember m where " + where;

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			var members = Database.Fetch<Member>(sql, parameters);
			if (members != null && members.Count > 0)
			{
				RetrieveVirtualCards(members);
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}

		public List<Member> RetrieveByPhoneNumber(string phoneNumber, LWQueryBatchInfo batchInfo)
		{
			var parameters = new object[] { phoneNumber };
            string sql = "select m.* from LW_LoyaltyMember m where PrimaryPhoneNumber = @0";

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			var members = Database.Fetch<Member>(sql, parameters);
			if (members != null && members.Count > 0)
			{
				RetrieveVirtualCards(members);
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}

		public List<Member> RetrieveByPostalCode(string postalCode, LWQueryBatchInfo batchInfo)
		{
			var parameters = new object[] { postalCode };
            string sql = "select m.* from LW_LoyaltyMember m where PrimaryPostalCode = @0";

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			var members = Database.Fetch<Member>(sql, parameters);
			if (members != null && members.Count > 0)
			{
				RetrieveVirtualCards(members);
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}

		public Member RetrieveByResetCode(string resetCode)
		{
			var member = Database.FirstOrDefault<Member>("where ResetCode = @0", resetCode);
			if (member != null)
			{
				member.IsDirty = false;
			}
			return member;
		}

		public List<Member> RetrieveAll(long[] ipCodes, bool loadVirtualCards)
		{
			int ipCodesRemaining = ipCodes.Length;
			int startIdx = 0;
			List<Member> members = new List<Member>();
			while (ipCodesRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(ipCodes, ref startIdx, ref ipCodesRemaining);
				var set = Database.Fetch<Member>("where ipcode in (@ipcodes)", new { ipcodes = ids });
				if (set != null && set.Count > 0)
				{
					if (loadVirtualCards)
					{
						RetrieveVirtualCards(set);
					}
					members.AddRange(set);
				}
			}
			if (members != null && members.Count > 0)
			{
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}

		public List<Member> RetrieveAllByVcKeys(long[] vcKeys)
		{
			int ipCodesRemaining = vcKeys.Length;
			int startIdx = 0;
			List<Member> members = new List<Member>();
			while (ipCodesRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(vcKeys, ref startIdx, ref ipCodesRemaining);
				var set = Database.Fetch<Member>("select m.* from LW_LoyaltyMember m inner join LW_VirtualCard v on v.IPCode = m.IPCode where v.VcKey in (@VcKeys)", new { VcKeys = ids });
				if (set != null && set.Count > 0)
				{
					RetrieveVirtualCards(set);
					members.AddRange(set);
				}
			}
			if (members != null && members.Count > 0)
			{
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}
		
		public List<Member> Retrieve(LWQueryBatchInfo batchInfo)
		{
			var parameters = new object[0];
			var sql = "select m.* from LW_LoyaltyMember m";

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			var members = Database.Fetch<Member>(sql, parameters);
			if (members != null && members.Count > 0)
			{
				RetrieveVirtualCards(members);
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}

		public List<Member> Retrieve(string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo)
		{
			const string methodName = "Retrieve";

            string sql = string.Format("select {0}.* from LW_LoyaltyMember {0}", string.IsNullOrEmpty(alias) ? "a" : alias);

			if (whereClause != null && !string.IsNullOrEmpty(whereClause.Where))
			{
				sql += " where " + whereClause.Where;
			}

			if (!string.IsNullOrEmpty(orderByClause))
			{
				sql += " " + orderByClause;
			}

			_logger.Debug(_className, methodName, "Executing query: " + sql);

			var args = new List<object>();
			if (whereClause != null)
			{
				int index = 0;
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					args.Add(parm.Value);
					sql = sql.Replace(":" + parm.Key, "@" + index.ToString());
				    index++;
				}
			}
			var parameters = args.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			var members = Database.Fetch<Member>(sql, parameters);
			if (members != null && members.Count > 0)
			{
				RetrieveVirtualCards(members);
				foreach (Member m in members)
				{
					m.IsDirty = false;
				}
			}
			return members;
		}

		public List<long> RetrieveIds(string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo)
		{
			const string methodName = "RetrieveIds";

			string sql = "select ipcode from LW_LoyaltyMember";

			if (!string.IsNullOrEmpty(alias))
			{
				sql += " as " + alias;
			}

			if (whereClause != null && !string.IsNullOrEmpty(whereClause.Where))
			{
				sql += " where " + whereClause.Where;
			}

			if (!string.IsNullOrEmpty(orderByClause))
			{
				sql += " " + orderByClause;
			}

			_logger.Debug(_className, methodName, "Executing query: " + sql);
			
			var args = new List<object>();
			if (whereClause != null)
			{
				int index = 0;
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					args.Add(parm);
					sql = sql.Replace(":" + parm.Key, "@" + index.ToString());
				    index++;
				}
			}
			var parameters = args.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref parameters);
			}

			Stopwatch timer = new Stopwatch();
			timer.Start();
			List<long> ret = Database.Fetch<long>(sql, parameters);
			timer.Stop();

			long count = ret.Count;

			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));

			return ret;
		}

        public List<Member> RetrieveAllByUniqueIdentifiers(Member member)
        {
            string sql = "select * from LW_LoyaltyMember where ";
            List<string> where = new List<string>();
            List<object> parameters = new List<object>();

            if (member.IsDirty) // Make sure we aren't checking when the member isn't going to be saved
            {
                if (!string.IsNullOrEmpty(member.PrimaryEmailAddress))
                {
                    where.Add(sql + "lower(PrimaryEmailAddress) = lower(@" + parameters.Count + ")");
                    parameters.Add(member.PrimaryEmailAddress);
                }
                if (!string.IsNullOrEmpty(member.Username))
                {
                    where.Add(sql + "Username = @" + parameters.Count);
                    parameters.Add(member.Username);
                }
                if (!string.IsNullOrEmpty(member.AlternateId))
                {
                    where.Add(sql + "AlternateId = @" + parameters.Count);
                    parameters.Add(member.AlternateId);
                }
                if(member.IpCode > 0)
                {
                    where.Add(sql + "Ipcode = @" + parameters.Count);
                    parameters.Add(member.IpCode);
                }
            }
            if (member.IsCardDirty && member.LoyaltyCards != null && member.LoyaltyCards.Count > 0)
            {
                List<string> cardsWhere = new List<string>();
                foreach (VirtualCard card in member.LoyaltyCards)
                {
                    if (card.IsDirty)
                    {
                        cardsWhere.Add("@" + parameters.Count);
                        parameters.Add(card.LoyaltyIdNumber);
                    }
                }
                if (cardsWhere.Count > 0)
                    where.Add(sql + "ipcode in (select ipcode from LW_VirtualCard where LoyaltyIdNumber in (" + string.Join(", ", cardsWhere) + "))");
            }

            if (where.Count == 0)
                return new List<Member>();

            var members = Database.Fetch<Member>(string.Join(" union ", where), parameters.ToArray());

            if (members != null && members.Count > 0)
            {
                RetrieveVirtualCards(members);
                foreach (Member m in members)
                {
                    m.IsDirty = false;
                }
            }
            return members;
        }

		//todo: aside from virtual cards, this method does not appear to delete ANYTHING member related (tiers, coupons, bonuses, attribute sets)
		public void Delete(long ipcode)
		{
			Member member = Retrieve(ipcode, false);
			if (member != null)
			{
				using (var txn = Database.GetTransaction())
				{
					DeleteVirtualCards(member);
					Database.Delete(member);
					txn.Complete();
				}
			}
		}

		public int Delete(long[] ipCodes)
		{
			using (var txn = Database.GetTransaction())
			{
				DeleteVirtualCards(ipCodes);
				int keysRemaining = ipCodes.Length;
				int startIdx = 0;
				int nRows = 0;
				while (keysRemaining > 0)
				{
					long[] ids = LimitInClauseList<long>(ipCodes, ref startIdx, ref keysRemaining);
					nRows += Database.Execute("delete from LW_LoyaltyMember where IpCode in (@ipcodes)", new { ipcodes = ids });
				}
				txn.Complete();
				return nRows;
			}
		}
		
		public void RetrieveVirtualCards(Member member)
		{
			string sql = "where IpCode = @0";
			var cards = Database.Fetch<VirtualCard>(sql, member.IpCode);
			if (cards != null)
			{
				foreach (VirtualCard card in cards)
				{
					card.Member = member;
                    card.Parent = member;
					card.IsDirty = false;
				}
				member.LoyaltyCards = cards;
			}
			else
			{
				member.LoyaltyCards = new List<VirtualCard>();
			}
		}

		public void RetrieveVirtualCards(List<Member> members)
		{
			IDictionary<long, Member> memberMap = new Dictionary<long, Member>();
			var ipcodeList = new List<long>();
			foreach (Member m in members)
			{
				memberMap.Add(m.IpCode, m);
				ipcodeList.Add(m.IpCode);
			}

			List<VirtualCard> cards = Database.Fetch<VirtualCard>("where IPCode in (@ipcodes)", new { ipcodes = ipcodeList.ToArray() });
			if (cards != null && cards.Count > 0)
			{
				foreach (VirtualCard vc in cards)
				{
					Member m = memberMap[vc.IpCode];
					vc.Member = m;
                    vc.Parent = m;
					m.LoyaltyCards.Add(vc);
					vc.IsDirty = false;
				}
			}
		}

		public void DeleteVirtualCards(Member member)
		{
			Database.Execute("delete from LW_VirtualCard where IPCode = @0", member.IpCode);
		}

		public void DeleteVirtualCards(long[] ipCodes)
		{
			int keysRemaining = ipCodes.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				nRows += Database.Execute("delete from LW_VirtualCard where IpCode in (@ipcodes)", new { ipcodes = LimitInClauseList<long>(ipCodes, ref startIdx, ref keysRemaining) });
			}
		}

		private void CreateVirtualCard(Member member, VirtualCard card)
		{
			card.IpCode = member.IpCode;
			SaveEntity(card);
		}

		private void CreateVirtualCards(IList<VirtualCard> cards)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				for (int i = 0; i < cards.Count; i++)
				{
					VirtualCard card = cards[i];
					SaveEntity(card);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
				}
				if (txn != null)
				{
					txn.Complete();
				}
			}
			finally
			{
				if (txn != null)
				{
					txn.Dispose();
				}
			}
		}

		private void SaveVirtualCards(IEnumerable<Member> members)
		{
			using (var txn = Database.GetTransaction())
			{
				List<VirtualCard> createList = new List<VirtualCard>();
				List<VirtualCard> updateList = new List<VirtualCard>();
				foreach (Member m in members)
				{
					foreach (VirtualCard card in m.LoyaltyCards)
					{
						if (card.MyKey == -1)
						{
							card.IpCode = m.IpCode;
							createList.Add(card);
						}
						else
						{
							updateList.Add(card);
						}
					}
				}
				if (createList.Count > 0)
				{
					CreateVirtualCards(createList);
				}
				if (updateList.Count > 0)
				{
					UpdateVirtualCards(updateList);
				}
				txn.Complete();
			}
		}

		private void UpdateVirtualCard(VirtualCard card)
		{
			UpdateEntity(card);
		}

		private void UpdateVirtualCards(List<VirtualCard> cards)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				for (int i = 0; i < cards.Count; i++)
				{
					VirtualCard card = cards[i];
					UpdateEntity(card);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
				}
				if (txn != null)
				{
					txn.Complete();
					txn.Dispose();
					txn = null;
				}
			}
			catch
			{
				if (txn != null)
				{
					txn.Dispose();
				}
				throw;
			}
		}



	}
}
