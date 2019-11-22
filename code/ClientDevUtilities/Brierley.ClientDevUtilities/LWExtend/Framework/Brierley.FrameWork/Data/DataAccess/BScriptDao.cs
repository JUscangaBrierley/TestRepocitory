using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class BScriptDao : DaoBase<Bscript>
	{
		public BScriptDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public List<Bscript> RetrieveAll()
		{
			return Database.Fetch<Bscript>("select * from LW_BScript");
		}

		public List<Bscript> RetrieveAll(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<Bscript>();
            return RetrieveByArray<long>("select * from LW_BScript where id in(@0)", ids);
		}

		public List<Bscript> RetrieveAll(string[] names)
		{
            if (names == null || names.Length == 0)
                return new List<Bscript>();
            return RetrieveByArray<string>("select * from LW_BScript where name in(@0)", names);
		}

		public Bscript Retrieve(long id)
		{
			return Database.FirstOrDefault<Bscript>("select * from LW_BScript where id = @0", id);
		}

		public Bscript Retrieve(string name)
		{
			return Database.FirstOrDefault<Bscript>("select * from LW_BScript where name = @0", name);
		}

		public List<Bscript> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<Bscript>("select * from LW_BScript where UpdateDate >= @0", since);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public void Delete(string bsName)
		{
			Bscript bs = Retrieve(bsName);
			if (bs != null)
			{
				Delete(bs.Id);
			}
		}
		
		public IEnumerable<Bscript> Search(string search, ExpressionContexts context, string currentConditionAttributeSet, int maxResults)
		{
			//@0 currentConditionAttributeSet, @1 search
			string[] searchList = search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			string sql = "select b.* from LW_BScript b";

			List<string> clauses = new List<string>();
            var parameters = new List<object>();

			if (!string.IsNullOrEmpty(currentConditionAttributeSet))
			{
                clauses.Add(string.Format("b.CurrentConditionAttributeSet = @{0}", parameters.Count));
                parameters.Add(currentConditionAttributeSet);
			}
			else
			{
				clauses.Add("(b.CurrentConditionAttributeSet is null or b.CurrentConditionAttributeSet = '')");
			}

            List<int> contexts = new List<int>();
            foreach (ExpressionContexts val in Enum.GetValues(typeof(ExpressionContexts)))
                if (val == context || (val & context) != 0 || val == 0)
                    contexts.Add((int)val);
            clauses.Add(string.Format("b.ExpressionContext in (@{0})", parameters.Count));
            parameters.Add(contexts);

			List<string> searchClauses = new List<string>();
			if (searchList.Length > 0)
			{
                foreach(string searchItem in searchList)
				{
                    searchClauses.Add(string.Format("(lower(b.Name) like @{0} OR lower(b.Description) like @{0})", parameters.Count.ToString()));
                    parameters.Add("%" + searchItem.Replace("%", string.Empty).ToLower() + "%");
                }
			}

			if (clauses.Count > 0 || searchClauses.Count > 0)
			{
				sql += " WHERE ";
				if (clauses.Count > 0)
				{
					sql += string.Join(" AND ", clauses);
				}
				if (searchClauses.Count > 0)
				{
					if (clauses.Count > 0)
					{
						sql += " AND (";
					}

					sql += string.Join(" OR ", searchClauses);

					if (clauses.Count > 0)
					{
						sql += ")";
					}
				}
			}

			sql += " order by name";

            return Database.Page<Bscript>(1, maxResults, sql, parameters.ToArray()).Items;
		}
	}
}
