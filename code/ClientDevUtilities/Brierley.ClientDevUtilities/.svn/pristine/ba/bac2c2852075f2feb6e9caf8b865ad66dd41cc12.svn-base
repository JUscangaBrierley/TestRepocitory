using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class StructuredContentDataDao : DaoBase<StructuredContentData>
	{
		public StructuredContentAttributeDao StructuredContentAttributeDao { get; set; }

		public StructuredContentDataDao(Database database, ServiceConfig config, StructuredContentAttributeDao dao)
			: base(database, config)
		{
			if (dao == null)
			{
				throw new ArgumentNullException("StructuredContentAttributeDao");
			}
			StructuredContentAttributeDao = dao;
		}

		public StructuredContentData Retrieve(long id)
		{
			return GetEntity(id);
		}

		public StructuredContentData Retrieve(long batchId, long seqId, long attributeId)
		{
			return Database.FirstOrDefault<StructuredContentData>("select * from LW_StructuredContentData where BatchId = @0 and SequenceId = @1 and AttributeId = @2", batchId, seqId, attributeId);
		}

		public List<StructuredContentData> RetrieveAll()
		{
			return Database.Fetch<StructuredContentData>("select * from LW_StructuredContentData order by Id");
		}

		public List<StructuredContentData> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<StructuredContentData>("select * from LW_StructuredContentData where UpdateDate >= @0 order by Id", changedSince);
		}

		public long GetNextSequenceID(long batchId, long elementId)
		{
			long result = 0;

			// Check for existing sequenceIDs 
			long count = Database.ExecuteScalar<long>(
				"select count(SequenceID) from LW_StructuredContentData where BatchId = @0 and AttributeID in (select Id from LW_StructuredContentAttribute where ElementID = @1)",
				batchId,
				elementId);

			if (count > 0)
			{
				// Result will be max(sequenceID) + 1
				result = Database.ExecuteScalar<long>(
					"select max(SequenceId) from LW_StructuredContentData where BatchId = @0 and AttributeId in (select Id from LW_StructuredContentAttribute where ElementId = @1)",
					batchId,
					elementId);

				result += 1;
			}
			return result;
		}

		public bool HasDataForElementID(long elementId)
		{
			// This method is used in LN to see if an element can be deleted (how many datums refer to an attribute for this element?)
			return Database.ExecuteScalar<int>(
				"select count(*) from LW_StructuredContentData where AttributeId in (select Id from LW_StructuredContentAttribute where ElementId = @0)",
				elementId) > 0;
		}

		public SortedList<string, long> RetrieveBatches()
		{
			// Get distinct batch IDs
			Dictionary<long, string> batchID2Name = new Dictionary<long, string>();
			List<long> batchIds = Database.Fetch<long>("select distinct BatchId from LW_StructuredContentData");

			foreach (long batchID in batchIds)
			{
				batchID2Name.Add(batchID, batchID.ToString());
			}

			// Get those with a batch name
			var batchNameDatums = Database.Fetch<StructuredContentData>(
				"select * from LW_StructuredContentData where AttributeId in (select Id from LW_StructuredContentAttribute where Name = @0)",
				StructuredContentAttribute.BATCH_NAME);

			foreach (StructuredContentData batchNameDatum in batchNameDatums)
			{
				batchID2Name[batchNameDatum.BatchID] = StringUtils.FriendlyString(batchNameDatum.Data);
			}

			SortedList<string, long> result = new SortedList<string, long>();
			foreach (KeyValuePair<long, string> mapping in batchID2Name)
			{
				if (result.ContainsKey(mapping.Value))
				{
					// Duplicate batch name
					string newBatchName = mapping.Value + "_" + mapping.Key;
					result.Add(newBatchName, mapping.Key);

					// Rename the batch so it is not a duplicate
					try
					{
						StructuredContentAttribute batchNameAttr = StructuredContentAttributeDao.Retrieve(StructuredContentAttribute.BATCH_NAME);
						if (batchNameAttr != null)
						{
							StructuredContentData batchNameDatum = Retrieve(mapping.Key, -1, batchNameAttr.ID);
							batchNameDatum.Data = newBatchName;
							Update(batchNameDatum);
						}
					}
					catch (Exception)
					{
						// Ignore
					}
				}
				else
				{
					result.Add(mapping.Value, mapping.Key);
				}
			}
			return result;
		}

		public List<StructuredContentData> RetrieveBatch(long batchId)
		{
			return Database.Fetch<StructuredContentData>("select * from LW_StructuredContentData where BatchId = @0 order by SequenceId, AttributeId", batchId);
		}

		//todo: this method has pl/sql in it and no tsql. Is it being used? It also doesn't make immediate sense - looking for a batch name in the data?
		public List<StructuredContentData> RetrieveBatch(string batchName)
		{
			return Database.Fetch<StructuredContentData>(
				@"select * from LW_StructuredContentData 
				where BatchId in 
				(select BatchId from LW_StructuredContentData d, LW_StructuredContentAttribute a where d.AttributeId = a.Id and a.Name = @0 and to_char(substr(d2.Data, 0, 2000)) = @1)
				order by BatchId, SequenceId, AttributeId",
				StructuredContentAttribute.BATCH_NAME,
				batchName);
		}

		public List<StructuredContentData> RetrieveGlobalData(long batchId)
		{
			long[] globalAttrIDs = GetGlobalAttrIDs();
			if (globalAttrIDs != null && globalAttrIDs.Length > 0)
			{
				return Database.Fetch<StructuredContentData>("select * from LW_StructuredContentData where BatchId = @0 and AttributeId in (@ids)", batchId, new { ids = globalAttrIDs });
			}
			return null;
		}

		public List<StructuredContentData> RetrieveElementData(long batchId, long[] elementAttrIds)
		{
            if (elementAttrIds == null || elementAttrIds.Length == 0)
                return new List<StructuredContentData>();
			return Database.Fetch<StructuredContentData>(
				"select * from LW_StructuredContentData where BatchId = @0 and AttributeId in (@ids) order by SequenceId, AttributeId",
				batchId,
				new { ids = elementAttrIds });
		}

		public List<StructuredContentData> RetrieveElementData(long batchID, long elementID)
		{
			long[] elementAttrIDs = GetElementAttrIDs(elementID);
			return RetrieveElementData(batchID, elementAttrIDs);
		}

		public List<StructuredContentData> RetrieveDataColumn(string attrName)
		{
			return Database.Fetch<StructuredContentData>(
				"select * from LW_StructuredContentData where AttributeId in (select Id from LW_StructuredContentAttribute where Name = @0)",
				attrName);
		}

		//todo: another blind pl/sql query with no tsql
		public List<StructuredContentData> RetrieveMatchingDatums(long batchId, long attributeId, string datumValue)
		{
			if (batchId == -1)
			{
				return Database.Fetch<StructuredContentData>(
					"select * from LW_StructuredContentData where AttributeId = @0 and to_char(substr(d.Data, 0, 2000)) = @1 order by BatchId, SequenceId",
					attributeId,
					datumValue);
			}
			else
			{
				return Database.Fetch<StructuredContentData>(
					"select * from LW_StructuredContentData where BatchId = @0 and AttributeId = @1 and to_char(substr(d.Data, 0, 2000)) = @2 order by BatchId, SequenceId",
					batchId,
					attributeId,
					datumValue);
			}
		}

		public List<StructuredContentData> Search(SearchDatumArgs args)
		{
			// Users can't specify wildcards
			string expression = "%" + args.SearchExpression.Replace("%", string.Empty) + "%";

			string sql = "select * from LW_StructuredContentData d where BatchId = @0 and AttributeID in (select ID from LW_StructuredContentAttribute where ElementId = @1) and ";

			// Case-sensitive or not
			if (args.IsCaseSensitiveSearch)
			{
				sql += "Data";
			}
			else
			{
				sql += "lower(" + GetDataColumn() + ")";
				expression = expression.ToLower();
			}
			sql += " like @2";

			// order by batch ID, seq ID, attr ID
			sql += " order by BatchId, SequenceId, AttributeId";

			// construct query
			return Database.Fetch<StructuredContentData>(sql, args.BatchID, args.ElementID, expression);
		}

		public StructuredDataRow RetrieveRow(long batchId, long sequenceId, long elementId)
		{
			string sql =
			   @"select d.Id as DATAID, d.BatchId as BATCHID, d.SequenceId as SEQUENCEID, d.AttributeID as ATTRIBUTEID, d.Data as DATA, 
				a.ID as ATTRIBUTEID, a.Name as ATTRIBUTENAME, a.DataType as DATATYPE, 
                    a.DefaultValue as DEFAULTVALUE, a.IsMandatory as ISMANDATORY, a.IsGlobal as ISGLOBAL, a.ElementID as ELEMENTID, a.IsFilter as ISFILTER, a.FilterOrder as FILTERORDER, a.ListField as LISTFIELD
                 from LW_StructuredContentData d, LW_StructuredContentAttribute a
                 where 
                    d.AttributeID = a.ID 
                    and d.BatchID = @0 
                    and d.SequenceID = @1 
                    and a.ElementID = @2 
                 order by a.Name asc";

			var list = Database.Fetch<dynamic>(sql.ToUpper(), batchId, sequenceId, elementId);

			StructuredDataRow result = new StructuredDataRow();
			if (list != null)
			{
				foreach (var row in list)
				{
					StructuredContentData datum = new StructuredContentData();
					datum.ID = StringUtils.FriendlyInt64(row.DATAID);
					datum.BatchID = StringUtils.FriendlyInt64(row.BATCHID);
					datum.SequenceID = StringUtils.FriendlyInt64(row.SEQUENCEID);
					datum.AttributeID = StringUtils.FriendlyInt64(row.ATTRIBUTEID);
					datum.Data = StringUtils.FriendlyString(row.DATA);

					StructuredContentAttribute attr = new StructuredContentAttribute();
					attr.ID = StringUtils.FriendlyInt64(row.ATTRIBUTEID);
					attr.Name = StringUtils.FriendlyString(row.ATTRIBUTENAME);
					attr.DataType = StringUtils.FriendlyString(row.DATATYPE);
					attr.DefaultValue = StringUtils.FriendlyString(row.DEFAULTVALUE);
					attr.IsMandatory = StringUtils.FriendlyBool(row.ISMANDATORY);
					attr.IsGlobal = StringUtils.FriendlyBool(row.ISGLOBAL);
					attr.ElementID = StringUtils.FriendlyInt64(row.ELEMENTID);
					attr.IsFilter = StringUtils.FriendlyBool(row.ISFILTER);
					attr.FilterOrder = StringUtils.FriendlyInt64(row.FILTERORDER);
					attr.ListField = StringUtils.FriendlyString(row.LISTFIELD);

					StructuredDataCell cell = new StructuredDataCell();
					cell.Datum = datum;
					cell.Attribute = attr;
					result.Add(cell);
				}
			}
			return result;
		}

		public FilterCollection RetrieveFilterCollection(long batchId, long elementId)
		{
			string dataColumn = GetDataColumn();

			string sql =
			   @"select distinct a.ID as ATTRIBUTEID,a.Name as ATTRIBUTENAME, a.FilterOrder as FILTERORDER, " + dataColumn + @" as THEDATACOLUMN
                 from LW_StructuredContentAttribute a, LW_StructuredContentData d 
                 where d.AttributeID = a.ID and a.IsFilter = 1 and a.FilterOrder > -1
                    and d.BatchID = @0 and (a.ElementID = @1 or a.ElementID = -1)
                 order by a.FilterOrder," + dataColumn;

			var rows = Database.Fetch<dynamic>(sql.ToUpper(), batchId, elementId);

			FilterCollection result = new FilterCollection();
			if (rows != null && rows.Count > 0)
			{
				long lastAttributeID = -1;
				Filter filter = null;
				foreach (dynamic row in rows)
				{
					long attributeID = StringUtils.FriendlyInt64(row.ATTRIBUTEID);
					string attributeName = StringUtils.FriendlyString(row.ATTRIBUTENAME);
					int filterOrder = StringUtils.FriendlyInt32(row.FILTERORDER);
					string filterValue = StringUtils.FriendlyString(row.THEDATACOLUMN);

					if (lastAttributeID == -1)
					{
						filter = new Filter();
						filter.AttributeID = attributeID;
						filter.AttributeName = attributeName;
						filter.FilterOrder = filterOrder;
						filter.Add(filterValue);
						lastAttributeID = attributeID;
					}
					else if (lastAttributeID == attributeID)
					{
						filter.Add(filterValue);
					}
					else
					{
						result.Add(filter);
						filter = new Filter();
						filter.AttributeID = attributeID;
						filter.AttributeName = attributeName;
						filter.FilterOrder = filterOrder;
						filter.Add(filterValue);
						lastAttributeID = attributeID;
					}
				}
				if (filter != null) result.Add(filter);
			}
			return result;
		}

		public StructuredDataRowList RetrieveFilteredAttributes(long batchId, long elementId, Dictionary<long, string> filterValues)
		{
			var parameters = new List<object>() { batchId, elementId };

			string sql = @"
               select d.ID as DATAID, d.BatchID as BATCHID, d.SequenceID as SEQUENCEID, d.Data as DATA, a.ID as ATTRIBUTEID, a.Name as ATTRIBUTENAME, a.DataType as DATATYPE,
                  a.DefaultValue as DEFAULTVALUE, a.IsMandatory as ISMANDATORY, a.IsGlobal as ISGLOBAL, a.ElementID as ELEMENTID, a.IsFilter as ISFILTER, a.FilterOrder as FILTERORDER, a.ListField as LISTFIELD
               from LW_StructuredContentData d, LW_StructuredContentAttribute a
               where 
                  d.AttributeID = a.ID 
                  and a.IsFilter = 0
                  and d.BatchID = @0
                  and a.ElementID = @1
               ";

			// Create where clause with bind variables for each filter
			if (filterValues != null && filterValues.Keys.Count > 0)
			{
				foreach (long attrID in filterValues.Keys)
				{
					sql += @" 
                      and exists (
                         select 1 from LW_StructuredContentData x 
                         where 
                            x.BatchID=d.BatchID 
                            and x.SequenceID=d.SequenceID 
                            and x.AttributeID=@" + (parameters.Count).ToString() + @" 
                            and x.Data like @" + (parameters.Count+1).ToString() + @"
                      )";
					parameters.Add(attrID);
					parameters.Add(filterValues[attrID]);
				}
			}

			sql += " order by d.BatchID, d.SequenceID, a.Name";

			var resultRows = Database.Fetch<dynamic>(sql.ToUpper(), parameters.ToArray());

			StructuredDataRowList result = new StructuredDataRowList();
			if (resultRows != null && resultRows.Count > 0)
			{
				long lastSequenceID = -1;
				StructuredDataRow structuredRow = null;
				foreach (dynamic row in resultRows)
				{
					StructuredContentData datum = new StructuredContentData();
					datum.ID = StringUtils.FriendlyInt64(row.DATAID);
					datum.BatchID = StringUtils.FriendlyInt64(row.BATCHID);
					datum.SequenceID = StringUtils.FriendlyInt64(row.SEQUENCEID);
					datum.AttributeID = StringUtils.FriendlyInt64(row.ATTRIBUTEID);
					datum.Data = StringUtils.FriendlyString(row.DATA);

					StructuredContentAttribute attr = new StructuredContentAttribute();
					attr.ID = StringUtils.FriendlyInt64(row.ATTRIBUTEID);
					attr.Name = StringUtils.FriendlyString(row.ATTRIBUTENAME);
					attr.DataType = StringUtils.FriendlyString(row.DATATYPE);
					attr.DefaultValue = StringUtils.FriendlyString(row.DEFAULTVALUE);
					attr.IsMandatory = StringUtils.FriendlyBool(row.ISMANDATORY);
					attr.IsGlobal = StringUtils.FriendlyBool(row.ISGLOBAL);
					attr.ElementID = StringUtils.FriendlyInt64(row.ELEMENTID);
					attr.IsFilter = StringUtils.FriendlyBool(row.ISFILTER);
					attr.FilterOrder = StringUtils.FriendlyInt64(row.FILTERORDER);
					attr.ListField = StringUtils.FriendlyString(row.LISTFIELD);

					StructuredDataCell cell = new StructuredDataCell();
					cell.Datum = datum;
					cell.Attribute = attr;

					if (lastSequenceID == -1)
					{
						structuredRow = new StructuredDataRow();
						structuredRow.Add(cell);
						lastSequenceID = datum.SequenceID;
					}
					else if (lastSequenceID == datum.SequenceID)
					{
						structuredRow.Add(cell);
					}
					else
					{
                        result.Add(structuredRow);
						structuredRow = new StructuredDataRow();
						structuredRow.Add(cell);
						lastSequenceID = datum.SequenceID;
					}
				}
				if (structuredRow != null) result.Add(structuredRow);
			}
			return result;
		}

		public void DeleteDatum(long id)
		{
			DeleteEntity(id);
		}

		public void DeleteBatch(long batchId)
		{
			Database.Execute("delete from LW_StructuredContentData where BatchId = @0", batchId);
		}

		public void DeleteBatchSequence(long batchId, long sequenceId)
		{
			Database.Execute("delete from LW_StructuredContentData where BatchId = @0 and SequenceId = @1", batchId, sequenceId);
		}

		private long[] GetGlobalAttrIDs()
		{
			long[] result = null;

			List<StructuredContentAttribute> globalAttributes = StructuredContentAttributeDao.RetrieveAllGlobals();
			if (globalAttributes != null && globalAttributes.Count > 0)
			{
				result = new long[globalAttributes.Count];
				int idx = 0;
				foreach (StructuredContentAttribute globalAttribute in globalAttributes)
				{
					result[idx++] = globalAttribute.ID;
				}
			}
			return result;
		}

		private long[] GetElementAttrIDs(long elementID)
		{
			long[] result = null;

			List<StructuredContentAttribute> elementAttributes = StructuredContentAttributeDao.RetrieveByElementId(elementID);
			if (elementAttributes != null && elementAttributes.Count > 0)
			{
				result = new long[elementAttributes.Count];
				int idx = 0;
				foreach (StructuredContentAttribute elementAttribute in elementAttributes)
				{
					result[idx++] = elementAttribute.ID;
				}
			}
			return result;
		}

		private string GetDataColumn()
		{
			string result = "d.Data";
			if (Database._dbType is PetaPoco.DatabaseTypes.OracleDatabaseType)
			{
				result = "to_char(substr(d.Data,1,2000))";
			}
			else
			{
				result = "substring(d.Data,1,datalength(d.Data))";
			}
			return result;
		}
	}
}
