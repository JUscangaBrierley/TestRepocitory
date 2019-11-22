using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.Cache;
using Brierley.FrameWork.Data.DataAccess;
using DM = Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Brierley.FrameWork.CampaignManagement
{
	public class CampaignManager : ServiceBase
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);

		private AudienceDao _audienceDao;
		private CampaignDao _campaignDao;
		private CampaignTableDao _campaignTableDao;
		private StepDao _stepDao;
		private StepIODao _stepIODao;
		private TableKeyDao _tableKeyDao;
		private TableFieldDao _tableFieldDao;
		private FieldValueDao _fieldValueDao;
		private GlobalDao _globalDao;
		private GlobalAttributeDao _globalAttributeDao;
		private AttributeDao _attributeDao;
		private CampaignAttributeDao _campaignAttributeDao;
		private CampaignOfferDao _campaignOfferDao;
		private CampaignSegmentDao _campaignSegmentDao;
		private CampaignOfferSegmentDao _campaignOfferSegmentDao;

		/// <summary>
		/// gets or sets the default mode for executing campaigns (foreground or via job scheduler)
		/// </summary>
		public ExecutionTypes ExecutionType
		{
			get
			{
				return Config.CampaignConfig != null ? Config.CampaignConfig.ExecutionType : ExecutionTypes.Foreground;
			}
		}

		/// <summary>
		/// gets or sets whether scheduled immediate jobs will send an email to the user who initiated execution.
		/// </summary>
		/// <remarks>
		/// this value is global to each client and must be set through the DBConfig screen. It will only cause an email
		/// to be sent if campaing execution mode is set to scheduled.
		/// </remarks>
		public bool SendExecutionEmail
		{
			get
			{
				return Config.CampaignConfig != null ? Config.CampaignConfig.SendExecutionEmail : false;
			}
		}


		#region Dao Properties

		public AudienceDao AudienceDao
		{
			get
			{
				if (_audienceDao == null)
				{
					_audienceDao = new AudienceDao(Database, Config);
				}
				return _audienceDao;
			}
		}

		public CampaignDao CampaignDao
		{
			get
			{
				if (_campaignDao == null)
				{
					_campaignDao = new CampaignDao(Database, Config);
				}
				return _campaignDao;
			}
		}

		public CampaignTableDao CampaignTableDao
		{
			get
			{
				if (_campaignTableDao == null)
				{
					_campaignTableDao = new CampaignTableDao(Database, Config);
				}
				return _campaignTableDao;
			}
		}

		public StepDao StepDao
		{
			get
			{
				if (_stepDao == null)
				{
					_stepDao = new StepDao(Database, Config);
				}
				return _stepDao;
			}
		}

		public StepIODao StepIODao
		{
			get
			{
				if (_stepIODao == null)
				{
					_stepIODao = new StepIODao(Database, Config);
				}
				return _stepIODao;
			}
		}

		public TableKeyDao TableKeyDao
		{
			get
			{
				if (_tableKeyDao == null)
				{
					_tableKeyDao = new TableKeyDao(Database, Config);
				}
				return _tableKeyDao;
			}
		}

		public TableFieldDao TableFieldDao
		{
			get
			{
				if (_tableFieldDao == null)
				{
					_tableFieldDao = new TableFieldDao(Database, Config);
				}
				return _tableFieldDao;
			}
		}

		public FieldValueDao FieldValueDao
		{
			get
			{
				if (_fieldValueDao == null)
				{
					_fieldValueDao = new FieldValueDao(Database, Config);
				}
				return _fieldValueDao;
			}
		}

		public GlobalDao GlobalDao
		{
			get
			{
				if (_globalDao == null)
				{
					_globalDao = new GlobalDao(Database, Config);
				}
				return _globalDao;
			}
		}

		public GlobalAttributeDao GlobalAttributeDao
		{
			get
			{
				if (_globalAttributeDao == null)
				{
					_globalAttributeDao = new GlobalAttributeDao(Database, Config);
				}
				return _globalAttributeDao;
			}
		}

		public AttributeDao AttributeDao
		{
			get
			{
				if (_attributeDao == null)
				{
					_attributeDao = new AttributeDao(Database, Config);
				}
				return _attributeDao;
			}
		}

		public CampaignAttributeDao CampaignAttributeDao
		{
			get
			{
				if (_campaignAttributeDao == null)
				{
					_campaignAttributeDao = new CampaignAttributeDao(Database, Config);
				}
				return _campaignAttributeDao;
			}
		}

		public CampaignOfferDao CampaignOfferDao
		{
			get
			{
				if (_campaignOfferDao == null)
				{
					_campaignOfferDao = new CampaignOfferDao(Database, Config);
				}
				return _campaignOfferDao;
			}
		}

		public CampaignSegmentDao CampaignSegmentDao
		{
			get
			{
				if (_campaignSegmentDao == null)
				{
					_campaignSegmentDao = new CampaignSegmentDao(Database, Config);
				}
				return _campaignSegmentDao;
			}
		}

		public CampaignOfferSegmentDao CampaignOfferSegmentDao
		{
			get
			{
				if (_campaignOfferSegmentDao == null)
				{
					_campaignOfferSegmentDao = new CampaignOfferSegmentDao(Database, Config);
				}
				return _campaignOfferSegmentDao;
			}
		}

		#endregion

		public CampaignDataProvider BatchProvider
		{
			get
			{
				return Config.CampaignConfig != null ? Config.CampaignConfig.BatchProvider : null;
			}
		}

		public CampaignDataProvider RealTimeProvider
		{
			get
			{
				return Config.CampaignConfig != null ? Config.CampaignConfig.RealTimeProvider : null;
			}
		}


		internal CampaignManager(ServiceConfig config)
			: base(config)
		{
		}

		public TableInclusionType GetInclusionType(bool realTime)
		{
			if (BatchProvider.UsesFramework)
			{
				return TableInclusionType.IncludeAll;
			}

			if (realTime)
			{
				return TableInclusionType.IncludeOnlyFramework;
			}
			return TableInclusionType.IncludeOnlyWarehouse;
		}

		public Brierley.FrameWork.Data.DomainModel.ScheduledJobRun GetExecutionJob(long campaignId)
		{
			Brierley.FrameWork.Data.DomainModel.ScheduledJobRun ret = null;
            using (var service = LWDataServiceUtil.DataServiceInstance(Organization, Environment))
			{
				var jobs = service.GetScheduledJob("Brierley.FrameWork", "Brierley.FrameWork.CampaignManagement.Jobs.CampaignJobFactory");
				foreach (var job in jobs)
				{
					if (!string.IsNullOrEmpty(job.Parms))
					{
						XElement e = XElement.Parse(job.Parms);
						long id = 0;
						if (long.TryParse(e.AttributeValue("id", "0"), out id) && id == campaignId)
						{
							foreach (var run in service.GetAllScheduledJobRuns(job.ID))
							{
								if (!run.EndTime.HasValue)
								{
									ret = run;
									break;
								}
							}
							if (ret != null)
							{
								break;
							}
						}
					}
				}
			}
			return ret;
		}

		#region Audience

		public void CreateAudience(Audience audience)
		{
			AudienceDao.Create(audience);
		}

		public Audience CreateAudience(string audienceName, string audienceDescription)
		{
			Audience audience = new Audience() { Name = audienceName, Description = audienceDescription };
			AudienceDao.Create(audience);
			return audience;
		}

		public void UpdateAudience(Audience audience)
		{
			AudienceDao.Update(audience);
		}

		public Audience GetAudience(long id)
		{
			return AudienceDao.Retrieve(id);
		}

		public Audience GetAudience(string name)
		{
			return AudienceDao.Retrieve(name);
		}

		public List<Audience> GetAllAudiences()
		{
			return AudienceDao.RetrieveAll() ?? new List<Audience>();
		}

		public void DeleteAudience(long id)
		{
			AudienceDao.Delete(id);
		}

		public bool AudienceExists(string AudienceName)
		{
			//todo: put an Exists method in AudienceDao instead of fetching
			return AudienceDao.Retrieve(AudienceName) != null;
		}

		public bool AttributeExists(string attributeName)
		{
			return AttributeDao.Retrieve(attributeName) != null;
		}

		public Dictionary<long, string> GetMappableAudiences(TableInclusionType inclusionType)
		{
			return TableKeyDao.GetMappableAudiences(inclusionType);
		}

		#endregion

		#region Campaign

		public void CreateCampaign(Campaign campaign)
		{
			CampaignDao.Create(campaign);
		}

		public void CreateCampaign(Campaign campaign, List<CampaignAttribute> attributes)
		{
			using (var txn = Database.GetTransaction())
			{
				CampaignDao.Create(campaign);
				if (attributes != null && attributes.Count > 0)
				{
					foreach (CampaignAttribute attribute in attributes)
					{
						if (!string.IsNullOrEmpty(attribute.AttributeValue))
						{
							attribute.CampaignId = campaign.Id;
							CampaignAttributeDao.Create(attribute);
						}
					}
				}
				txn.Complete();
			}
		}

		public Campaign CreateCampaign(string CampaignName, string CampaignDescription)
		{
			Campaign campaign = new Campaign(CampaignName, CampaignDescription, DateTime.Now);
			CreateCampaign(campaign);
			return campaign;
		}

		public Campaign CloneCampaign(long cloningCampaignId, string clonedCampaignName, string clonedCampaignDescription, bool cloneAsTemplate, List<CampaignAttribute> campaignAttributes, long? folderId)
		{
			return CloneCampaign(cloningCampaignId, clonedCampaignName, clonedCampaignDescription, cloneAsTemplate, campaignAttributes, folderId, null);
		}


		public Campaign CloneCampaign(long cloningCampaignId, string clonedCampaignName, string clonedCampaignDescription, bool cloneAsTemplate, List<CampaignAttribute> campaignAttributes, long? folderId, List<long> steps)
		{

			Campaign cloningCampaign = GetCampaign(cloningCampaignId);
			Campaign clonedCampaign = new Campaign(clonedCampaignName, clonedCampaignDescription, DateTime.Now);
			clonedCampaign.IsTemplate = cloneAsTemplate;
			clonedCampaign.FolderId = folderId;
			clonedCampaign.CampaignType = cloningCampaign.CampaignType;
			CreateCampaign(clonedCampaign, campaignAttributes);

			CloneCampaign(clonedCampaign, cloningCampaign, steps);
			return clonedCampaign;
		}

		public List<Step> CloneCampaign(Campaign destination, Campaign source, List<long> steps, int desiredX = 0, int desiredY = 0)
		{
			const int stepWidth = 150; //campaign control is about 150px wide
			const int stepHeight = 100; //and 100px high


			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}

			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (steps == null)
			{
				steps = source.Steps.Select(o => o.Id).ToList();
			}

			//if the destination campaign has steps (i.e., we're adding a step collection to an existing campaign, not creating a campaign
			//from scratch), then we neeed to find an empty area large enough to hold the new steps. 
			bool hasSteps = destination.Steps != null && destination.Steps.Count > 0;

			var ret = new List<Step>();
			Dictionary<long, long> ioMap = new Dictionary<long, long>();

			foreach (long stepId in steps)
			{
				Step cloningStep = source.Steps[stepId];
				Step step = CloneStep(stepId, destination.Id);

				destination.Steps.Add(step);
				ioMap.Add(cloningStep.Id, step.Id);
				ret.Add(step);
			}

			//any step that had inputs in the source but no input in the destination should be converted from the table key 
			//of the input step to its audience level. For example, a user has a campaign with a select that feeds an output.
			//the select uses the member key, the output's key is the key of the select step. A collection is created containing
			//only the output step. We need to swap the select step's key in the output step with the audience level key.
			foreach (Step cloningStep in source.Steps.Where(o => ioMap.ContainsKey(o.Id) && o.StepType != StepType.Merge))
			{
				foreach (StepIO io in RetrieveInputs(cloningStep.Id))
				{
					if (!ioMap.ContainsKey(io.InputStepId))
					{
						//the cloned step has inputs that are not being cloned. switch the destination step to their audience level
						var key = cloningStep.Key;
						if (key != null)
						{
							var convertToKey = GetTableKeyByAudience(key.AudienceId).Where(o => o.IsAudienceLevel).FirstOrDefault();
							if (convertToKey != null)
							{
								destination.Steps[ioMap[cloningStep.Id]].KeyId = convertToKey.Id;
								UpdateStep(destination.Steps[ioMap[cloningStep.Id]]);
							}
						}

					}
				}
			}

			//swap old input step ids with their new cloned ids
			foreach (Step cloningStep in source.Steps)
			{
				List<StepIO> inputs = RetrieveInputs(cloningStep.Id);
				foreach (StepIO input in inputs)
				{
					if (!ioMap.ContainsKey(input.InputStepId) || !ioMap.ContainsKey(cloningStep.Id))
					{
						continue;
					}
					else if (input.MergeType != null)
					{
						destination.Steps[ioMap[cloningStep.Id]].Inputs.Add(ioMap[input.InputStepId], input.MergeType.GetValueOrDefault(), input.MergeOrder.GetValueOrDefault());
					}
					else
					{
						destination.Steps[ioMap[cloningStep.Id]].Inputs.Add(ioMap[input.InputStepId]);
					}
				}
			}

			//update any query columns that reference the input step with the new input step's id
			//this would not be necessary if CloneQuery works properly - it takes parent step and 
			//travels up to its input table to retrieve the id...
			foreach (Step clonedStep in destination.Steps)
			{
				if (clonedStep.Query != null && clonedStep.Query.Columns.Count > 0)
				{
					long inputTableId = 0;
					List<StepIO> inputs = RetrieveInputs(clonedStep.Id);
					if (inputs.Count == 1)
					{
						inputTableId = destination.Steps[inputs[0].InputStepId].OutputTableId.GetValueOrDefault(0);

						foreach (QueryColumn column in clonedStep.Query.Columns)
						{
							if (column.TableId < 1)
							{
								if (column.ColumnType != ColumnType.Append || clonedStep.StepType == StepType.DeDupe)
								{
									column.TableId = inputTableId;
									UpdateStep(clonedStep);
								}
							}
						}
					}
				}
			}

			//offer map = dictionary of original id to new id
			var offerMap = new Dictionary<long, long>();

			var sourceOffers = GetAllOffers(source.Id, true);
			var sourceSegments = GetAllSegments(source.Id, true);

			if (sourceOffers != null)
			{
				foreach (var offer in sourceOffers)
				{
					var o = new Offer() { CampaignId = destination.Id, OfferCode = offer.OfferCode };
					foreach (var attribute in offer.Attributes)
					{
						var a = new CampaignAttribute();
						a.CampaignId = destination.Id;
						a.AttributeId = attribute.AttributeId;
						a.AttributeValue = attribute.AttributeValue;
						o.Attributes.Add(a);
					}
					CreateOffer(o);
					offerMap.Add(offer.Id, o.Id);
				}
			}

			if (sourceSegments != null)
			{
				foreach (var segment in sourceSegments)
				{
					var s = new Segment() { CampaignId = destination.Id, SegmentCode = segment.SegmentCode };
					foreach (var attribute in segment.Attributes)
					{
						var a = new CampaignAttribute();
						a.CampaignId = destination.Id;
						a.AttributeId = attribute.AttributeId;
						a.AttributeValue = attribute.AttributeValue;
						s.Attributes.Add(a);
					}
					foreach (var segmentOffer in segment.Offers)
					{
						if (offerMap.ContainsKey(segmentOffer))
						{
							s.Offers.Add(offerMap[segmentOffer]);
						}
					}
					CreateSegment(s);
				}
			}

			if (hasSteps)
			{
				//find a fit for the new steps
				Point idealPoint = Point.Empty;

				if (desiredX > 0 && desiredY > 0)
				{
					//user has chosen a place for the steps. Honor that position
					idealPoint = new Point(desiredX, desiredY);
				}
				else
				{
					//find the best available spot for the new steps

					var bounds = new Rectangle(
						source.Steps.Min(o => o.UIPositionX),
						source.Steps.Min(o => o.UIPositionY),
						source.Steps.Max(o => o.UIPositionX) + stepWidth,
						source.Steps.Max(o => o.UIPositionY) + stepHeight);

					//remove offset
					bounds.Width -= bounds.X;
					bounds.X = 0;
					bounds.Height -= bounds.Y;
					bounds.Y = 0;

					Func<int, int, bool> intersects = delegate(int x, int y)
					{
						//for each step already in the campaign
						foreach (var step in destination.Steps.Where(o => !ret.Contains(o)))
						{
							//test whether or not the rectangle (offset by x and y) overlaps the step
							if (
								step.UIPositionX + stepWidth >= x &&
								step.UIPositionX < bounds.Width + x &&
								step.UIPositionY + stepHeight >= y &&
								step.UIPositionY < bounds.Height + y
								)
							{
								return true;
							}
						}
						return false;
					};

					for (int y = 0; y < 4000 - bounds.Width; y += 10)
					{
						for (int x = 0; x < 6000 - bounds.Height; x += 10)
						{
							if (!intersects(x, y))
							{
								idealPoint = new Point(x, y);
								break;
							}
						}
						if (idealPoint != Point.Empty)
						{
							break;
						}
					}
				}

				if (idealPoint != Point.Empty)
				{
					//now offset all steps to the new point
					int offsetX = source.Steps.Min(o => o.UIPositionX);
					int offsetY = source.Steps.Min(o => o.UIPositionY);

					foreach (var step in ret)
					{
						step.UIPositionX += idealPoint.X - offsetX;
						step.UIPositionY += idealPoint.Y - offsetY;
						UpdateStep(step);
					}
				}
			}

			return ret;
		}

		public Step CloneStep(long cloningStepId, long? campaignId)
		{

			Step cloningStep = StepDao.Retrieve(cloningStepId);

			Step step = new Step();
			step.LastRunDate = null;
			step.OutputTableId = null;
			step.StepType = cloningStep.StepType;
			step.UIDescription = cloningStep.UIDescription;
			step.UIName = cloningStep.UIName;
			step.UIPositionX = cloningStep.UIPositionX;
			step.UIPositionY = cloningStep.UIPositionY;

			step.CampaignId = campaignId;

			step.XmlQuery = cloningStep.XmlQuery;

			//assign key, if starter step
			if (cloningStep.Inputs.Count == 0 && cloningStep.KeyId.HasValue)
			{
				step.KeyId = cloningStep.KeyId;
			}

			CreateStep(step);

			//update step - this will ensure an output table is created and configured for the step.

			if (step.OutputTableId.HasValue && step.Query != null && step.Key == null && cloningStep.Query != null && cloningStep.Key != null)
			{
				//Create the table key for the step
				TableKey stepKey = new TableKey();
				stepKey.AudienceId = cloningStep.Key.AudienceId;
				stepKey.TableId = step.OutputTableId.Value;
				stepKey.FieldName = cloningStep.Key.FieldName;
				stepKey.FieldType = cloningStep.Key.FieldType;
				CreateTableKey(stepKey);
			}
			return step;
		}

		public void UpdateCampaign(Campaign campaign)
		{

			CampaignDao.Update(campaign);
		}

		public void UpdateCampaign(Campaign campaign, List<CampaignAttribute> attributes)
		{
			CampaignDao.Update(campaign);
			if (attributes != null)
			{
				foreach (CampaignAttribute attribute in attributes)
				{
					if (CampaignAttributeDao.Retrieve(attribute.CampaignId, attribute.AttributeId) != null)
					{
						if (!string.IsNullOrEmpty(attribute.AttributeValue))
						{
							CampaignAttributeDao.Update(attribute);
						}
						else
						{
							CampaignAttributeDao.Delete(campaign.Id, attribute.AttributeId);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(attribute.AttributeValue))
						{
							CampaignAttributeDao.Create(attribute);
						}
					}
				}
			}
		}


		public Campaign GetCampaign(long id)
		{

			return CampaignDao.Retrieve(id);
		}


		public Campaign GetCampaign(string name)
		{

			return CampaignDao.Retrieve(name);
		}


		public List<Campaign> GetCampaigns()
		{

			return CampaignDao.RetrieveAll(false);
		}


		public List<Campaign> GetCampaigns(bool includeTemplates, params CampaignType[] campaignType)
		{

			return CampaignDao.RetrieveAll(includeTemplates, campaignType);
		}

        public List<Campaign> GetCampaigns(bool includeTemplates, long pageNumber, long pageSize, CampaignType campaignType = CampaignType.Batch)
        {
            return CampaignDao.RetrieveAll(includeTemplates, campaignType, pageNumber, pageSize);
        }


		public List<Campaign> GetCampaignsByFolder(long folderId)
		{

			return GetCampaignsByFolder(folderId, false);
		}


		public List<Campaign> GetCampaignsByFolder(long folderId, bool includeTemplates)
		{

			return CampaignDao.RetrieveByFolder(folderId, includeTemplates);
		}


		public List<Campaign> GetCampaignTemplates(CampaignType? campaignType = null)
		{

			return CampaignDao.RetrieveAllTemplates(campaignType);
		}

		public List<Campaign> GetCampaignTemplatesByFolder(long folderId)
		{

			return CampaignDao.RetrieveTemplatesByFolder(folderId);
		}

        public List<Campaign> GetCampaignsById(List<long> campaignIds, bool? isExecuting = null, CampaignType? campaignType = null)
        {
            return CampaignDao.Retrieve(campaignIds, isExecuting, campaignType);
        }


        /// <summary>
        /// Deletes a campaign
        /// </summary>
        /// <param name="CampaignID">The ID of the campaign to delete.</param>
        /// <param name="DropRelatedTables">
        /// Drop all tables related to the campaign.
        ///		"Related Tables" refers to any tables created behind the scenes - table type 2 (Step) 
        ///		in CLTableType - as part of a campaign step.
        /// </param>
        /// <param name="DropOutputTables">
        /// Drop all tables created using output steps.
        ///		Refers to any table specifically created and named by the user for use as an output.
        /// </param>
        public void DeleteCampaign(long CampaignID, bool DropRelatedTables, bool DropOutputTables)
		{

			Campaign campaign = GetCampaign(CampaignID);
            ClearCampaign(campaign);

			CampaignDao.Delete(CampaignID);
		}

        public void ClearCampaign(Campaign campaign)
        {
            foreach (Step step in campaign.Steps)
            {
                DeleteStep(step.Id, true, false);
            }
            foreach (CampaignAttribute campaignAttribute in GetAllCampaignAttributes(campaign.Id))
            {
                CampaignAttributeDao.Delete(campaignAttribute.CampaignId, campaignAttribute.AttributeId);
            }

            foreach (var offer in GetAllOffers(campaign.Id, false))
            {
                DeleteOffer(offer.Id);
            }

            foreach (var segment in GetAllSegments(campaign.Id, false))
            {
                DeleteSegment(segment.Id);
            }
        }


		public bool CampaignExists(string CampaignName)
		{

			return CampaignDao.Retrieve(CampaignName) != null;
		}


		/// <summary>
		/// Adds a collection of steps to a campaign
		/// </summary>
		/// <param name="campaignId"></param>
		/// <param name="collectionId"></param>
		public List<Step> AddCollectionToCampaign(long campaignId, long collectionId, int desiredX = 0, int desiredY = 0)
		{

			Campaign collection = GetCampaign(collectionId);
			Campaign destination = GetCampaign(campaignId);
			var steps = collection.Steps.Select(o => o.Id).ToList();
			return CloneCampaign(destination, collection, steps, desiredX, desiredY);
		}

		/// <summary>
		/// Creates a new collection from a selection of steps in a campaign
		/// </summary>
		/// <param name="name"></param>
		/// <param name="steps"></param>
		public Campaign CreateCollection(string name, long fromCampaignId, List<long> steps)
		{

			return CreateCollection(name, null, fromCampaignId, steps);
		}

		public Campaign CreateCollection(string name, string description, long fromCampaignId, List<long> steps)
		{

			if (CampaignExists(name))
			{
				throw new Exception(string.Format("A campaign already exists with the name {0}", name));
			}
			return CloneCampaign(fromCampaignId, name, description, true, null, null, steps);
		}

		#endregion


		#region CampaignTable


		public void CreateCampaignTable(CampaignTable table)
		{
            LWDataServiceUtil.GetServiceConfiguration(Organization, Environment).CacheManager.RemoveRegion(Constants.CacheRegions.MappableTablesByAudienceId);
			CampaignTableDao.Create(table);
		}


		public CampaignTable CreateCampaignTable(string TableName, TableType TableType)
		{
			CampaignTable table = new CampaignTable();
			table.Name = TableName;
			table.TableType = TableType;
			CreateCampaignTable(table);
			return table;
		}


		public void UpdateCampaignTable(CampaignTable table)
		{
            LWDataServiceUtil.GetServiceConfiguration(Organization, Environment).CacheManager.RemoveRegion(Constants.CacheRegions.MappableTablesByAudienceId);
			CampaignTableDao.Update(table);
		}


		public CampaignTable GetCampaignTable(long id)
		{

			return CampaignTableDao.Retrieve(id);
		}

		public CampaignTable GetCampaignTable(string name)
		{

			return CampaignTableDao.Retrieve(name);
		}


		public List<CampaignTable> GetAllCampaignTables(TableType[] TableTypes, TableInclusionType inclusionType = TableInclusionType.IncludeAll)
		{

			List<CampaignTable> list = CampaignTableDao.RetrieveAllByType(TableTypes, inclusionType);
			if (list == null)
			{
				list = new List<CampaignTable>();
			}
			return list;
		}


		public List<CampaignTable> GetAllCampaignTables(TableInclusionType inclusionType = TableInclusionType.IncludeAll)
		{

			List<CampaignTable> list = CampaignTableDao.RetrieveAll(inclusionType);
			if (list == null)
			{
				list = new List<CampaignTable>();
			}
			return list;
		}


		public void DeleteCampaignTable(long id)
		{

            LWDataServiceUtil.GetServiceConfiguration(Organization, Environment).CacheManager.RemoveRegion(Constants.CacheRegions.MappableTablesByAudienceId);
			List<TableField> fields = TableFieldDao.RetrieveByTable(id);
			{
				foreach (TableField field in fields)
				{
					if (field.ValueGenerationType == ValueGenerationType.Cached)
					{
						FieldValueDao.DeleteByFieldId(field.Id);
					}
					TableFieldDao.Delete(field.Id);
				}
			}
			CampaignTableDao.Delete(id);
		}


		public long ImportData(long tableId, Stream data, string columnDelimiter, string textQualifier, bool dataHasFieldNames, bool appendToTable)
		{

			CampaignTable table = GetCampaignTable(tableId);
			if (table.IsFrameworkSchema && table.Name.StartsWith("LW_", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(string.Format("Table {0} cannot be imported to because it appears to be a core framework table. Import is not supported on framework database tables that begin with \"LW_\" or \"ATS_\".", table.Name));
			}

			CampaignDataProvider provider = table.IsFrameworkSchema ? RealTimeProvider : BatchProvider;
			DataTable importTable = provider.GetTableDetails(table.Name, table.ResidesInAlternateSchema);

			StreamReader reader = new StreamReader(data);
			string[] fieldNames = null;
			const string delimiterPlaceholder = "!$LW_DELIM$!";

			Func<string, string> removeQualifier = delegate(string val)
			{
				if (val.StartsWith(textQualifier) && val.EndsWith(textQualifier) && val.Length > 1)
				{
					val = val.Substring(textQualifier.Length, val.Length - textQualifier.Length * 2);
				}
				return val;
			};

			if (dataHasFieldNames)
			{
				string header = reader.ReadLine();
				if (string.IsNullOrEmpty(header))
				{
					throw new Exception("Could not import file. The file does not appear to be in the correct format.");
				}

				fieldNames = (from x in header.Split(',') select removeQualifier(x)).ToArray();

				if (fieldNames.Length == 0)
				{
					throw new Exception("Could not import file. No column names were found in line 1, or the file is in an incorrect format.");
				}

				for (int fileIndex = 0; fileIndex < fieldNames.Length; fileIndex++)
				{
					bool exists = false;
					for (int tableIndex = 0; tableIndex < importTable.Rows.Count; tableIndex++)
					{
						if (fieldNames[fileIndex].Equals((string)importTable.Rows[tableIndex]["FieldName"], StringComparison.OrdinalIgnoreCase))
						{
							exists = true;
							break;
						}
					}
					if (!exists)
					{
						throw new Exception(string.Format("Could not import the file. Column {0}, named in the header does not exist in the table.", fieldNames[fileIndex]));
					}
				}
			}
			else
			{
				fieldNames = new string[importTable.Rows.Count];
				for (int i = 0; i < importTable.Rows.Count; i++)
				{
					fieldNames[i] = (string)importTable.Rows[i]["FieldName"];
				}
			}

			string tableName = table.Name;
			if (table.ResidesInAlternateSchema)
			{
				tableName = provider.DataSchema + "." + tableName;
			}

			if (!appendToTable)
			{
				provider.TruncateTable(tableName);
			}

			string query = string.Format(
				"INSERT INTO {0}({1}) VALUES({2})",
				tableName,
				string.Join(", ", fieldNames),
				string.Join(", ", (from x in fieldNames select provider.ParameterPrefix + x)));

			long count = 0;

			using (var connection = provider.Factory.CreateConnection())
			{
				connection.ConnectionString = provider.ConnectionString;
				connection.Open();
				using (var cmd = provider.Factory.CreateCommand())
				{
					cmd.Transaction = connection.BeginTransaction();
					cmd.Connection = connection;
					cmd.CommandText = query;

					IDbDataParameter[] parameters = new IDbDataParameter[fieldNames.Length];
					for (int i = 0; i < fieldNames.Length; i++)
					{
						var param = cmd.CreateParameter();
						param.ParameterName = fieldNames[i];
						param.DbType = DbType.String;
						foreach (DataRow row in importTable.Rows)
						{
							if (((string)row["FieldName"]).Equals(fieldNames[i], StringComparison.OrdinalIgnoreCase))
							{
								param.DbType = Utils.DbTypeFromString((string)row["DataType"]);
								param.Size = Convert.ToInt32(row["Length"]);
								if (row["Precision"] != DBNull.Value && row["Scale"] != DBNull.Value)
								{
									param.Precision = Convert.ToByte(row["Precision"]);
									param.Scale = Convert.ToByte(row["Scale"]);
								}
								break;
							}
						}
						cmd.Parameters.Add(param);
						parameters[i] = param;
					}

					cmd.Prepare();

					Func<Match, string> eval = delegate(Match m)
					{
						try
						{
							return m.Groups["data"].ToString().Replace(columnDelimiter, delimiterPlaceholder);
						}
						catch
						{
							return m.ToString();
						}
					};


					while (true)
					{
						string line = reader.ReadLine();
						if (line == null)
						{
							//out of data - we're finished.
							break;
						}

						line = Regex.Replace(line, textQualifier + @"(?<data>.*?)" + textQualifier, new MatchEvaluator(eval));

						string[] fields = line.Split(',');
						for (int i = 0; i < fields.Length; i++)
						{
							var parameter = parameters[i];
							parameter.Value = null;
							string val = removeQualifier(fields[i]).Replace(delimiterPlaceholder, columnDelimiter);
							if (parameter.DbType == DbType.Date)
							{
								if (!string.IsNullOrEmpty(val))
								{
									parameter.Value = DateTime.Parse(val);
								}
							}
							else
							{
								parameter.Value = val;
							}
						}
						cmd.ExecuteNonQuery();
						count++;
					}
					cmd.Transaction.Commit();
				}
			}
			return count;
		}


		#endregion


		#region TableField


		public void CreateTableField(TableField field)
		{
			TableFieldDao.Create(field);
		}


		public void UpdateTableField(TableField field)
		{
			TableFieldDao.Update(field);
		}

		public TableField GetTableField(long id)
		{

			return TableFieldDao.Retrieve(id);
		}

		public List<TableField> GetTableFields(long tableId)
		{

			List<TableField> list = TableFieldDao.RetrieveByTable(tableId);
			if (list == null)
			{
				list = new List<TableField>();
			}
			return list;
		}

		public void DeleteTableField(long id)
		{

			FieldValueDao.DeleteByFieldId(id);
			TableFieldDao.Delete(id);
		}


		/// <summary>
		/// refreshes a field's list of values.
		/// </summary>
		/// <remarks>
		///	Distinct values can be retrieved on the fly, stored manually as a list or retrieved and stored in a table.
		/// </remarks>
		/// <param name="fieldId"></param>
		public void RefreshFieldValues(long fieldId)
		{

			TableField field = GetTableField(fieldId);
			if (field.ValueGenerationType != ValueGenerationType.Cached && field.ValueGenerationType != ValueGenerationType.SqlCached)
			{
				throw new ArgumentException("Distinct values may only be generated for ValueGenerationType.Cached and ValueGenerationType.SqlCached", field.ValueGenerationType.ToString());
			}

			CampaignTable table = GetCampaignTable(field.TableId);

			List<FieldValue> existingValues = FieldValueDao.RetrieveAllByFieldId(fieldId);
			DataTable values = null;

			if (field.ValueGenerationType == ValueGenerationType.Cached)
			{
				if (table.IsFrameworkSchema && !BatchProvider.UsesFramework)
				{
					values = RealTimeProvider.GetDistinctValues(table, field.Name);
				}
				else
				{
					values = BatchProvider.GetDistinctValues(table, field.Name);
				}
			}
			else
			{
				if (table.IsFrameworkSchema && !BatchProvider.UsesFramework)
				{
					values = RealTimeProvider.ExecuteDataTable(field.ValueList, null);
				}
				else
				{
					values = BatchProvider.ExecuteDataTable(field.ValueList, null);
				}
			}

			bool hasValues = existingValues.Count > 0;

			foreach (DataRow row in values.Rows)
			{
				bool hasValue = false;
				foreach (FieldValue existingValue in existingValues)
				{
					if (row["Value"].ToString() == existingValue.Value)
					{
						existingValue.Count = long.Parse(row["ValueCount"].ToString());
						if (field.ValueGenerationType == ValueGenerationType.SqlCached)
						{
							existingValue.DisplayValue = row["DisplayValue"].ToString();
						}
						FieldValueDao.Update(existingValue);
						hasValue = true;
						break;
					}
				}
				if (!hasValue)
				{
					FieldValue value = new FieldValue();
					value.FieldId = fieldId;
					value.Count = long.Parse(row["ValueCount"].ToString());
					value.Value = row["Value"].ToString();
					if (field.ValueGenerationType == ValueGenerationType.SqlCached)
					{
						value.DisplayValue = row["DisplayValue"].ToString();
					}

					FieldValueDao.Create(value);
				}
			}

			//delete any values that no longer exist
			if (hasValues)
			{
				foreach (FieldValue existingValue in existingValues)
				{
					bool exists = false;
					foreach (DataRow row in values.Rows)
					{
						if (row["Value"].ToString() == existingValue.Value)
						{
							exists = true;
							break;
						}
					}
					if (!exists)
					{
						FieldValueDao.Delete(existingValue.Id);
					}
				}
			}
		}


		#endregion


		#region Step


		public void CreateStep(Step step)
		{
			using (var txn = Database.GetTransaction())
			{
				//create a table definition for the step's output
				if (step.NeedsOutputTable() && (step.OutputTableId == null || step.OutputTableId < 1))
				{
					//table name will be "LW_CL_<table id>". We don't know the ID yet...
					CampaignTable table = CreateCampaignTable(Constants.TempTableNamePrefix, TableType.Step);
					table.Name = Constants.TempTableNamePrefix + table.Id.ToString();
					UpdateCampaignTable(table);
					step.OutputTableId = table.Id;
				}
				if (step.OutputTableId.GetValueOrDefault(0) > 0 && step.Query != null)
				{
					if (GetTableKeyByTable(step.OutputTableId.GetValueOrDefault(0)).Count == 0 && step.Key != null)
					{
						TableKey stepKey = new TableKey();
						stepKey.AudienceId = step.Key.AudienceId;
						stepKey.TableId = (long)step.OutputTableId;
						stepKey.FieldName = step.Key.FieldName;
						stepKey.FieldType = step.Key.FieldType;
						CreateTableKey(stepKey);
					}
				}
				StepDao.Create(step);
				txn.Complete();
			}
		}

		public Step CreateStep(long CampaignID, string StepName, string StepDescription, StepType StepType)
		{
			Step step = new Step();
			step.CampaignId = CampaignID;
			step.UIName = StepName;
			step.UIDescription = StepDescription;
			step.StepType = StepType;
			CreateStep(step);
			return step;
		}

		public void UpdateStep(Step step)
		{
			using (var txn = Database.GetTransaction())
			{
				//create a table definition for the step's output
				if (step.NeedsOutputTable() && step.OutputTableId.GetValueOrDefault(0) < 1)
				{
					//table name will be "LW_CL_<table id>". We don't know the ID yet...
					CampaignTable table = CreateCampaignTable(Constants.TempTableNamePrefix, TableType.Step);
					table.Name = Constants.TempTableNamePrefix + table.Id.ToString();
					UpdateCampaignTable(table);
					step.OutputTableId = table.Id;
				}

				if (step.OutputTableId != null && step.OutputTableId > 0)
				{
					TableKey outputKey = null;
					List<TableKey> currentKeys = GetTableKeyByTable(step.OutputTableId.GetValueOrDefault(0));
					if (currentKeys != null && currentKeys.Count > 0)
					{
						outputKey = currentKeys[0];
					}

					//Table key exists, make sure it still matches the query's key
					TableKey inputKey = null;
					if (step.StepType == StepType.ChangeAudience)
					{
						inputKey = ((ChangeAudienceQuery)step.Query).ConvertToKey;
					}
					else
					{
						inputKey = step.Key;
					}

					if (inputKey != null && outputKey != null)
					{
						if (outputKey.AudienceId != inputKey.AudienceId)
						{
							//keys are mismatched. Remove and recreate the key.
							//delete the table key only if it is tied to a step table, not a framework table.
							CampaignTable keyTable = GetCampaignTable(outputKey.TableId);
							if (keyTable != null && keyTable.TableType == TableType.Step)
							{
								DeleteTableKey(outputKey.Id);
							}
							outputKey = null;
						}
					}

					if (outputKey == null && inputKey != null)
					{
						//Table key does not exist, create the key
						TableKey stepKey = new TableKey();
						stepKey.AudienceId = inputKey.AudienceId;
						stepKey.TableId = (long)step.OutputTableId;
						stepKey.FieldName = inputKey.FieldName;
						stepKey.FieldType = inputKey.FieldType;
						CreateTableKey(stepKey);
					}
				}
				StepDao.Update(step);
				txn.Complete();
			}
		}

		public Step GetStep(long id)
		{
			return StepDao.Retrieve(id);
		}

		public List<Step> GetSteps(string name)
		{
			return StepDao.Retrieve(name);
		}

		public Step GetStepByTableId(long tableId)
		{
			return StepDao.RetrieveByTableId(tableId);
		}

		public List<Step> GetStepsByCampaignID(long CampaignID)
		{
			return StepDao.RetrieveAllByCampaignID(CampaignID);
		}

		public void DeleteStep(long id, bool DropRelatedTables, bool DropOutputTables)
		{
			Step step = GetStep(id);
			step.Inputs.Clear();
			step.Outputs.Clear();
			StepDao.Delete(id);

			if (DropRelatedTables && step.OutputTableId != null && step.OutputTableId > -1)
			{
				if (step.StepType != StepType.Output)
				{
					foreach (TableKey key in GetTableKeyByTable((long)step.OutputTableId))
					{
						DeleteTableKey(key.Id);
					}
					CampaignTable outputTable = GetCampaignTable((long)step.OutputTableId);
					BatchProvider.DropTable(outputTable.Name);
					DeleteCampaignTable(outputTable.Id);
				}
			}
		}

		#endregion


		#region TableKey


		public void CreateTableKey(TableKey tkey)
		{
			TableKeyDao.Create(tkey);
		}


		public void UpdateTableKey(TableKey tkey)
		{
			TableKeyDao.Update(tkey);
		}


		public TableKey GetTableKey(long id)
		{
			return TableKeyDao.Retrieve(id);
		}


		public List<string> GetAllTables(bool ExcludeConfigured, TableInclusionType inclusionType)
		{
			List<string> tables = new List<string>();
			var provider = inclusionType == TableInclusionType.IncludeOnlyFramework ? RealTimeProvider : BatchProvider;
			DataSet dbTables = provider.GetAvailableTables();

			List<CampaignTable> configuredTables = null;
			if (ExcludeConfigured)
			{
				configuredTables = CampaignTableDao.RetrieveAll();
			}

			foreach (DataRow row in dbTables.Tables["CampaignSchema"].Rows)
			{
				bool canAdd = true;
				if (ExcludeConfigured)
				{
					foreach (CampaignTable configuredTable in configuredTables)
					{
						if (row["TableName"].ToString().ToLower() == configuredTable.Name.ToLower())
						{
							canAdd = false;
							break;
						}
					}
				}
				if (canAdd)
				{
					tables.Add(row["TableName"].ToString());
				}
			}

			if (dbTables.Tables.Count > 1 && dbTables.Tables["DataSchema"] != null)
			{
				foreach (DataRow row in dbTables.Tables["DataSchema"].Rows)
				{
					bool canAdd = true;
					if (ExcludeConfigured)
					{
						foreach (CampaignTable configuredTable in configuredTables)
						{
							if (row["TableName"].ToString().ToLower() == configuredTable.Name.ToLower() /*&& !configuredTable.IsCampaignSchema*/)
							{
								canAdd = false;
								break;
							}
						}
					}
					if (canAdd)
					{
						tables.Add(provider.DataSchema + "." + row["TableName"].ToString());
					}
				}
			}
			return tables;
		}


		public List<TableKey> GetTableKeyByAudience(long audienceID)
		{
			List<TableKey> list = TableKeyDao.RetrieveByAudience(audienceID);
			if (list == null)
			{
				list = new List<TableKey>();
			}
			return list;
		}


		public List<TableKey> GetTableKeyByTable(long tableID)
		{
			List<TableKey> list = TableKeyDao.RetrieveByTable(tableID);
			if (list == null)
			{
				list = new List<TableKey>();
			}
			return list;
		}

		public void DeleteTableKey(long id)
		{
            LWDataServiceUtil.GetServiceConfiguration(Organization, Environment).CacheManager.RemoveRegion(Constants.CacheRegions.MappableTablesByAudienceId);
			TableKeyDao.Delete(id);
		}

		public DataTable GetTableKeyReferences(long KeyID)
		{
			return TableKeyDao.GetTableKeyReferences(KeyID);
		}


		#endregion


		#region StepIO


		public StepIO Create(long InputStepID, long OutputStepID)
		{
			return StepIODao.Create(InputStepID, OutputStepID);
		}


		public StepIO Retrieve(long InputStepID, long OutputStepID)
		{
			return StepIODao.Retrieve(InputStepID, OutputStepID);
		}


		public List<StepIO> RetrieveInputs(long OutputStepID)
		{
			return StepIODao.RetrieveInputs(OutputStepID);
		}


		public List<StepIO> RetrieveOutputs(long InputStepID)
		{
			return StepIODao.RetrieveOutputs(InputStepID);
		}


		public void Delete(long InputStepID, long OutputStepID)
		{
			StepIODao.Delete(InputStepID, OutputStepID);
		}


		#endregion


		#region Global


		public void CreateGlobal(Global global)
		{
			GlobalDao.Create(global);
		}


		public void UpdateGlobal(Global global)
		{
			GlobalDao.Update(global);
		}


		public Global GetGlobal(long id)
		{
			return GlobalDao.Retrieve(id);
		}


		public Global GetGlobal(string name)
		{
			return GlobalDao.Retrieve(name);
		}


		public List<Global> GetGlobals()
		{
			return (List<Global>)GlobalDao.RetrieveAll();
		}


		public void DeleteGlobal(long Id)
		{
			using (var txn = Database.GetTransaction())
			{
				GlobalAttributeDao.DeleteByGlobalId(Id);
				GlobalDao.Delete(Id);
				txn.Complete();
			}
		}


		#endregion


		#region GlobalAttribute


		public void CreateGlobalAttribute(GlobalAttribute globalAttribute)
		{
			GlobalAttributeDao.Create(globalAttribute);
		}


		public void UpdateGlobalAttribute(GlobalAttribute globalAttribute)
		{
			GlobalAttributeDao.Update(globalAttribute);
		}


		public List<GlobalAttribute> GetGlobalAttributesByGlobal(long globalId)
		{
			return GlobalAttributeDao.RetrieveAllByGlobalId(globalId);
		}


		public GlobalAttribute GetGlobalAttribute(long id)
		{
			return GlobalAttributeDao.Retrieve(id);
		}


		public GlobalAttribute GetGlobalAttribute(string attributeName)
		{
			return GlobalAttributeDao.Retrieve(attributeName);
		}

		public GlobalAttribute GetGlobalAttribute(long globalId, string attributeName)
		{
			return GlobalAttributeDao.Retrieve(globalId, attributeName);
		}


		public void DeleteGlobalAttribute(long globalAttributeId)
		{
			GlobalAttributeDao.Delete(globalAttributeId);
		}


		public void DeleteGlobalsAttributesByGlobalId(long globalId)
		{
			GlobalAttributeDao.DeleteByGlobalId(globalId);
		}


		#endregion


		#region Attribute


		public void CreateAttribute(DM.Attribute attribute)
		{
			AttributeDao.Create(attribute);
		}


		public void UpdateAttribute(DM.Attribute attribute)
		{
			AttributeDao.Update(attribute);
		}


		public List<DM.Attribute> GetAllAttributes()
		{
			return AttributeDao.RetrieveAll();
		}

		public List<DM.Attribute> GetAllAttributesByType(AttributeTypes attributeType)
		{
			return AttributeDao.RetrieveByType(attributeType);
		}

		public DM.Attribute GetAttribute(long id)
		{
			return AttributeDao.Retrieve(id);
		}

		public DM.Attribute GetAttribute(string attributeName)
		{
			return AttributeDao.Retrieve(attributeName);
		}

		public void DeleteAttribute(long id)
		{
			AttributeDao.Delete(id);
		}


		#endregion


		#region CampaignAttribute

		public void CreateCampaignAttribute(CampaignAttribute campaignAttribute)
		{
			CampaignAttributeDao.Create(campaignAttribute);
		}

		public void UpdateCampaignAttribute(CampaignAttribute campaignAttribute)
		{
			CampaignAttributeDao.Update(campaignAttribute);
		}

		public List<CampaignAttribute> GetAllCampaignAttributes(long campaignId)
		{
			return CampaignAttributeDao.RetrieveByCampaign(campaignId);
		}

		public CampaignAttribute GetCampaignAttribute(long campaignId, long attributeId)
		{
			return CampaignAttributeDao.Retrieve(campaignId, attributeId);
		}

		public void DeleteCampaignAttribute(long campaignId, long attributeId)
		{
			CampaignAttributeDao.Delete(campaignId, attributeId);
		}

		#endregion


		#region Offer

		public void CreateOffer(Offer offer)
		{
			using (var txn = Database.GetTransaction())
			{
				CampaignOfferDao.Create(offer);

				if (offer.Attributes != null)
				{
					foreach (var attribute in offer.Attributes)
					{
						attribute.OfferId = offer.Id;
						CampaignAttributeDao.Create(attribute);
					}
				}

				if (offer.Segments != null)
				{
					foreach (long segment in offer.Segments)
					{
						CampaignOfferSegmentDao.Create(offer.Id, segment);
					}
				}
				txn.Complete();
			}
		}

		public void UpdateOffer(Offer offer)
		{
			using (var txn = Database.GetTransaction())
			{
				CampaignOfferDao.Update(offer);
				if (offer.Attributes != null)
				{
					foreach (CampaignAttribute attribute in offer.Attributes)
					{
						attribute.OfferId = offer.Id;
						attribute.CampaignId = offer.CampaignId;
						attribute.SegmentId = null;

						if (attribute.Id > 0)
						{
							if (!string.IsNullOrEmpty(attribute.AttributeValue))
							{
								CampaignAttributeDao.Update(attribute);
							}
							else
							{
								CampaignAttributeDao.Delete(attribute.Id);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(attribute.AttributeValue))
							{
								CampaignAttributeDao.Create(attribute);
							}
						}
					}
				}

				CampaignOfferSegmentDao.DeleteByOffer(offer.Id);
				if (offer.Segments != null)
				{
					foreach (long segment in offer.Segments)
					{
						CampaignOfferSegmentDao.Create(offer.Id, segment);
					}
				}
				txn.Complete();
			}
		}

		public IEnumerable<Offer> GetAllOffers(long campaignId, bool populateAttributes)
		{
			var offers = CampaignOfferDao.RetrieveByCampaign(campaignId);
			if (offers != null)
			{
				if (populateAttributes)
				{
					foreach (var offer in offers)
					{
						offer.Attributes = CampaignAttributeDao.RetrieveByOffer(offer.Id);
					}
				}

				foreach (var offer in offers)
				{
					var segments = CampaignOfferSegmentDao.RetrieveSegments(offer.Id);
					if (segments != null)
					{
						offer.Segments = segments.ToList();
					}
				}
			}
			return offers;
		}

		public Offer GetOffer(long id, bool populateAttributes)
		{
			var offer = CampaignOfferDao.Retrieve(id);
			if (offer != null)
			{
				if (populateAttributes)
				{
					offer.Attributes = CampaignAttributeDao.RetrieveByOffer(offer.Id);
				}

				var segments = CampaignOfferSegmentDao.RetrieveSegments(id);
				if (segments != null)
				{
					offer.Segments = segments.ToList();
				}
			}
			return offer;
		}

		public Offer GetOffer(long campaignId, string offerCode, bool populateAttributes)
		{
			var offer = CampaignOfferDao.Retrieve(campaignId, offerCode);
			if (offer != null)
			{
				if (populateAttributes)
				{
					offer.Attributes = CampaignAttributeDao.RetrieveByOffer(offer.Id);
				}

				var segments = CampaignOfferSegmentDao.RetrieveSegments(offer.Id);
				if (segments != null)
				{
					offer.Segments = segments.ToList();
				}
			}
			return offer;
		}

		public void DeleteOffer(long id)
		{
			var attributes = CampaignAttributeDao.RetrieveByOffer(id);
			if (attributes != null)
			{
				foreach (var attribute in attributes)
				{
					CampaignAttributeDao.Delete(attribute.Id);
				}
			}
			CampaignOfferSegmentDao.DeleteByOffer(id);
			CampaignOfferDao.Delete(id);
		}

		#endregion


		#region Segment

		public void CreateSegment(Segment segment)
		{
			using (var txn = Database.GetTransaction())
			{
				CampaignSegmentDao.Create(segment);

				if (segment.Attributes != null)
				{
					foreach (var attribute in segment.Attributes)
					{
						attribute.SegmentId = segment.Id;
						CampaignAttributeDao.Create(attribute);
					}
				}

				if (segment.Offers != null)
				{
					foreach (long offer in segment.Offers)
					{
						CampaignOfferSegmentDao.Create(offer, segment.Id);
					}
				}
				txn.Complete();
			}
		}

		public void UpdateSegment(Segment segment)
		{
			using (var txn = Database.GetTransaction())
			{
				CampaignSegmentDao.Update(segment);

				if (segment.Attributes != null)
				{
					foreach (CampaignAttribute attribute in segment.Attributes)
					{
						attribute.SegmentId = segment.Id;
						attribute.CampaignId = segment.CampaignId;
						attribute.OfferId = null;

						if (attribute.Id > 0)
						{
							if (!string.IsNullOrEmpty(attribute.AttributeValue))
							{
								CampaignAttributeDao.Update(attribute);
							}
							else
							{
								CampaignAttributeDao.Delete(segment.CampaignId, attribute.AttributeId);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(attribute.AttributeValue))
							{
								CampaignAttributeDao.Create(attribute);
							}
						}
					}
				}

				CampaignOfferSegmentDao.DeleteBySegment(segment.Id);
				if (segment.Offers != null)
				{
					foreach (long offer in segment.Offers)
					{
						CampaignOfferSegmentDao.Create(offer, segment.Id);
					}
				}
				txn.Complete();
			}
		}

		public IEnumerable<Segment> GetAllSegments(long campaignId, bool populateAttributes)
		{
			var segments = CampaignSegmentDao.RetrieveByCampaign(campaignId);
			if (segments != null)
			{
				if (populateAttributes)
				{
					foreach (var segment in segments)
					{
						segment.Attributes = CampaignAttributeDao.RetrieveBySegment(segment.Id);
					}
				}

				foreach (var segment in segments)
				{
					var offers = CampaignOfferSegmentDao.RetrieveOffers(segment.Id);
					if (offers != null)
					{
						segment.Offers = offers.ToList();
					}
				}
			}
			return segments;
		}

		public Segment GetSegment(long id, bool populateAttributes)
		{
			var segment = CampaignSegmentDao.Retrieve(id);
			if (segment != null)
			{
				if (populateAttributes)
				{
					segment.Attributes = CampaignAttributeDao.RetrieveBySegment(segment.Id);
				}

				var offers = CampaignOfferSegmentDao.RetrieveOffers(segment.Id);
				if (offers != null)
				{
					segment.Offers = offers.ToList();
				}
			}
			return segment;
		}

		public Segment GetSegment(long campaignId, string segmentCode, bool populateAttributes)
		{
			var segment = CampaignSegmentDao.Retrieve(campaignId, segmentCode);
			if (segment != null)
			{
				if (populateAttributes)
				{
					segment.Attributes = CampaignAttributeDao.RetrieveBySegment(segment.Id);
				}

				var offers = CampaignOfferSegmentDao.RetrieveOffers(segment.Id);
				if (offers != null)
				{
					segment.Offers = offers.ToList();
				}
			}
			return segment;
		}

		public void DeleteSegment(long id)
		{
			var attributes = CampaignAttributeDao.RetrieveBySegment(id);
			if (attributes != null)
			{
				foreach (var attribute in attributes)
				{
					CampaignAttributeDao.Delete(attribute.CampaignId, attribute.AttributeId);
				}
			}
			CampaignOfferSegmentDao.DeleteBySegment(id);
			CampaignSegmentDao.Delete(id);
		}


		#endregion


		#region OfferSegmentXRef

		public OfferSegment CreateOfferSegment(long offerId, long segmentId)
		{
			return CampaignOfferSegmentDao.Create(offerId, segmentId);
		}

		public OfferSegment CreateOfferSegment(OfferSegment offerSegment)
		{
			return CampaignOfferSegmentDao.Create(offerSegment.OfferId, offerSegment.SegmentId);
		}

		public OfferSegment GetOfferSegment(long offerId, long segmentId)
		{
			return CampaignOfferSegmentDao.Retrieve(offerId, segmentId);
		}

		public IEnumerable<long> GetSegmentsByOffer(long offerId)
		{
			return CampaignOfferSegmentDao.RetrieveSegments(offerId);
		}

		public IEnumerable<long> GetOffersBySegment(long segmentId)
		{
			return CampaignOfferSegmentDao.RetrieveOffers(segmentId);
		}

		public void DeleteOfferSegment(long offerId, long segmentId)
		{
			CampaignOfferSegmentDao.Delete(offerId, segmentId);
		}

		public void DeleteOfferSegmentByOffer(long offerId)
		{
			CampaignOfferSegmentDao.DeleteByOffer(offerId);
		}

		public void DeleteOfferSegmentBySegment(long segmentId)
		{
			CampaignOfferSegmentDao.DeleteByOffer(segmentId);
		}

		#endregion
	}
}
