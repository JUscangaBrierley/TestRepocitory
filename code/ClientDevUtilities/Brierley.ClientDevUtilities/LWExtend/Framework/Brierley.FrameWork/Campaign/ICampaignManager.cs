using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Data.Cache;
using NHibernate;

namespace Brierley.FrameWork.CampaignManagement
{
	public interface ICampaignManager
	{
		CampaignManager ConcreteInstance();

		IDataCacheProvider CacheManager { get; set; }

		ISessionFactory SessionFactory { get; }

		TableInclusionType GetInclusionType(bool realTime);

		#region Audience
		
		Audience CreateAudience(Audience audience);

		Audience CreateAudience(string AudienceName, string AudienceDescription);

		Audience UpdateAudience(Audience audience);

		Audience GetAudience(long id);

		Audience GetAudience(string name);

		IList<Audience> GetAllAudiences();

		void DeleteAudience(long id);

		bool AudienceExists(string AudienceName);

		Dictionary<long, string> GetMappableAudiences(TableInclusionType inclusionType);

		#endregion


		#region Campaign


		Campaign CreateCampaign(Campaign campaign);

		Campaign CreateCampaign(string CampaignName, string CampaignDescription);

		Campaign UpdateCampaign(Campaign campaign);

		Campaign GetCampaign(long id);

		Campaign GetCampaign(string name);

		IList<Campaign> GetCampaigns();

		IList<Campaign> GetCampaigns(bool includeTemplates, params CampaignType[] campaignType);

		IList<Campaign> GetCampaignTemplates(CampaignType? campaignType = null);

		void DeleteCampaign(long ID, bool DropRelatedTables, bool DropOutputTables);

		bool CampaignExists(string CampaignName);

		List<Step> AddCollectionToCampaign(long campaignId, long collectionId, int desiredX = 0, int desiredY = 0);

		Campaign CreateCollection(string name, long fromCampaignId, List<long> steps);

		Campaign CreateCollection(string name, string description, long fromCampaignId, List<long> steps);

		#endregion


		#region CampaignTable


		CampaignTable CreateCampaignTable(CampaignTable table);

		CampaignTable CreateCampaignTable(string TableName, TableType TableType);

		CampaignTable UpdateCampaignTable(CampaignTable table);

		CampaignTable GetCampaignTable(long id);

		CampaignTable GetCampaignTable(string name);

		IList<CampaignTable> GetAllCampaignTables(TableType[] TableTypes, TableInclusionType inclusionType = TableInclusionType.IncludeAll);

		IList<CampaignTable> GetAllCampaignTables(TableInclusionType inclusionType = TableInclusionType.IncludeAll);

		void DeleteCampaignTable(long id);

		long ImportData(long tableId, Stream data, string columnDelimiter, string textQualifier, bool dataHasFieldNames, bool appendToTable);


		#endregion


		#region TableField

		TableField CreateTableField(TableField field);

		TableField UpdateTableField(TableField field);

		TableField GetTableField(long id);

		IList<TableField> GetTableFields(long tableId);

		void DeleteTableField(long id);

		void RefreshFieldValues(long fieldId);

		#endregion

		#region Step


		Step CreateStep(Step step);

		Step CreateStep(long CampaignID, string StepName, string StepDescription, StepType StepType);

		Step UpdateStep(Step step);

		Step GetStep(long id);

		IList<Step> GetSteps(string name);

		Step GetStepByTableId(long tableId);

		IList<Step> GetStepsByCampaignID(long CampaignID);
		
		void DeleteStep(long id, bool DropRelatedTables, bool DropOutputTables);
		
		#endregion


		#region StepIO
		
		StepIO Create(long InputStepID, long OutputStepID);
		StepIO Retrieve(long InputStepID, long OutputStepID);
		IList<StepIO> RetrieveInputs(long OutputStepID);
		IList<StepIO> RetrieveOutputs(long InputStepID);
		void Delete(long InputStepID, long OutputStepID);
		
		#endregion



		#region TableKey


		TableKey CreateTableKey(TableKey tkey);

		TableKey UpdateTableKey(TableKey tkey);

		TableKey GetTableKey(long id);

		List<string> GetAllTables(bool ExcludeConfigured, TableInclusionType inclusionType = TableInclusionType.IncludeAll);

		IList<TableKey> GetTableKeyByAudience(long audienceID);

		IList<TableKey> GetTableKeyByTable(long tableID);

		DataTable GetTableKeyReferences(long KeyID);

		void DeleteTableKey(long ID);


		#endregion


		#region Global
		

		Global CreateGlobal(Global global);

		Global UpdateGlobal(Global global);

		Global GetGlobal(long id);

		Global GetGlobal(string name);

		List<Global> GetGlobals();

		void DeleteGlobal(long ID);


		#endregion


		#region GlobalAttribute


		GlobalAttribute CreateGlobalAttribute(GlobalAttribute globalAttribute);

		GlobalAttribute UpdateGlobalAttribute(GlobalAttribute globalAttribute);

		IList<GlobalAttribute> GetGlobalAttributesByGlobal(long globalId);

		GlobalAttribute GetGlobalAttribute(long id);

		GlobalAttribute GetGlobalAttribute(string name);

		GlobalAttribute GetGlobalAttribute(long globalId, string name);

		void DeleteGlobalAttribute(long globalAttributeId);

		void DeleteGlobalsAttributesByGlobalId(long globalId);


		#endregion

	}
}
