using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	/// <summary>
	/// </summary>
	[Serializable]
	[DataContract(Namespace = "http://www.brierley.com/CampaignWare/CampaignService")]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("StepId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLStep")]
	public class Step : LWCoreObjectBase
	{
		private TableKey _key = null;
		private string _xmlQuery = null;
		private Query _query = null;
		private StepIOCollection _inputs = null;
		private StepIOCollection _outputs = null;
		private long _id = 0;

		/// <summary>
		/// Gets or sets the ID for the current Step
		/// </summary>
		[DataMember(Order = 1)]
		[PetaPoco.Column("StepId")]
		public long Id
		{
			get
			{
				return _id;
			}
			set
			{
				if (_id != value)
				{
					_id = value;
					_inputs = null;
					_outputs = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the StepType for the current Step
		/// </summary>
		[DataMember(Order = 2)]
		[PetaPoco.Column("StepTypeId")]
		public StepType StepType { get; set; }

		/// <summary>
		/// Gets or sets the UIPositionX for the current Step
		/// </summary>
		[DataMember(Order = 3)]
		[PetaPoco.Column]
		public int UIPositionX { get; set; }

		/// <summary>
		/// Gets or sets the UIPositionY for the current Step
		/// </summary>
		[DataMember(Order = 4)]
		[PetaPoco.Column]
		public int UIPositionY { get; set; }

		/// <summary>
		/// Gets or sets the UIName for the current Step
		/// </summary>
		[DataMember(Order = 5)]
		[PetaPoco.Column(Length = 50, IsNullable = false)]
		public string UIName { get; set; }

		/// <summary>
		/// Gets or sets the UIDescription for the current Step
		/// </summary>
		[DataMember(Order = 6)]
		[PetaPoco.Column(Length = 250)]
		public string UIDescription { get; set; }

		/// <summary>
		/// Gets or sets the LastError for the current Step
		/// </summary>
		[DataMember(Order = 7)]
		[PetaPoco.Column]
		public string LastError { get; set; }

		/// <summary>
		/// Gets the number of input steps that are connected to this step. Used by campaign builder to determine if the step
		/// is a starter step or if it is being fed data from another step.
		/// </summary>
		[DataMember(Order = 8)]
		public int InputCount
		{
			get
			{
				return Inputs.Count;
			}
			set
			{
			}
		}

		[DataMember(Order = 9)]
		public Query Query
		{
			get
			{
				if (_query == null)
				{
					if (!string.IsNullOrEmpty(_xmlQuery))
					{
						if (StepType == CampaignManagement.StepType.RealTimebScript)
						{
							//4.5 added bScript step for batch campaigns. For consistency, it was named "bScriptQuery" and the existing
							//bScriptQuery has been renamed RealTimebScriptQuery. We'll replace the serialized type declaration "bScriptQuery"
							//with "RealTimebScriptQuery" if the step type is RealTimebScript to prevent errors.
							_xmlQuery = _xmlQuery.Replace(@"xsi:type=""bScriptQuery""", @"xsi:type=""RealTimebScriptQuery""");
						}
						if (StepType == CampaignManagement.StepType.Output)
						{
							//4.5.6.0 removed MailFile as an output type. We'll swap it with the flat file output type.
							_xmlQuery = _xmlQuery.Replace(@"<OutputType>MailFile</OutputType>", @"<OutputType>FlatFile</OutputType>");
						}

						//deserialize
						XmlSerializer serializer = new XmlSerializer(typeof(Query));
						_query = (Query)serializer.Deserialize(new System.IO.StringReader(_xmlQuery));
					}
					if (_query == null)
					{
						_query = QueryFactory.GetQuery(this.StepType);
					}
					_query.Step = this;
				}
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		/// <summary>
		/// gets or sets the execution priority for the step.
		/// </summary>
		[DataMember(Order = 10)]
		[PetaPoco.Column]
		public int? ExecutionPriority { get; set; }

		/// <summary>
		/// Gets or sets the VerificationState for the current Step
		/// </summary>
		[DataMember(Order = 11)]
		[PetaPoco.Column]
		public VerificationState? VerificationState { get; set; }

		/// <summary>
		/// Gets or sets the CampaignID for the current Step
		/// </summary>
		[DataMember(Order = 12)]
		[PetaPoco.Column]
        [ForeignKey(typeof(Campaign), "Id")]
        [ColumnIndex]
		public long? CampaignId { get; set; }

		/// <summary>
		/// Gets or sets the KeyID for the current Step
		/// </summary>
		[DataMember(Order = 13)]
		[PetaPoco.Column]
		public long? KeyId { get; set; }

		/// <summary>
		/// Gets or sets the UILastRecordCount for the current Step
		/// </summary>
		[DataMember(Order = 14)]
		[PetaPoco.Column]
		public int? UILastRecordCount { get; set; }

		/// <summary>
		/// Gets or sets the LastRunDate for the current Step
		/// </summary>
		[DataMember(Order = 15)]
		[PetaPoco.Column]
		public DateTime? LastRunDate { get; set; }

		/// <summary>
		/// Gets or sets the UILastStatus for the step
		/// </summary>
		[DataMember(Order = 16)]
		[PetaPoco.Column(Length = 50)]
		public string UILastStatus { get; set; }

		/// <summary>
		/// Is the step currently executing?
		/// </summary>
		[DataMember(Order = 17)]
		[PetaPoco.Column]
		public bool? IsExecuting { get; set; }

		/// <summary>
		/// Gets or sets the OutputTableID for the current Step
		/// </summary>
		[DataMember(Order = 18)]
		[PetaPoco.Column]
        [ForeignKey(typeof(CampaignTable), "Id")]
		public long? OutputTableId { get; set; }

		[DataMember(Order = 19)]
		[PetaPoco.Column(Length = 2000)]
		public string EmailExecutionStartTo { get; set; }

		[DataMember(Order = 20)]
		[PetaPoco.Column(Length = 2000)]
		public string EmailExecutionEndTo { get; set; }

		[XmlIgnore]
		public TableKey Key
		{
			get
			{
				if (_key == null)
				{
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                    {
                        if (IsRealTimeStep())
                        {
                            var tables = manager.GetAllCampaignTables(new TableType[] { TableType.Framework });
                            foreach (var table in tables)
                            {
                                if (table.Name.ToLower() == "lw_loyaltymember")
                                {
                                    var keys = manager.GetTableKeyByTable(table.Id);
                                    foreach (var key in keys)
                                    {
                                        if (key.FieldName.ToLower() == "ipcode")
                                        {
                                            _key = key;
                                            KeyId = key.Id;
                                        }
                                    }
                                }
                            }
                        }

                        if (KeyId != null && KeyId > 0)
                        {
                            TableKey key = manager.GetTableKey(KeyId.Value);
                            if (key != null)
                            {
                                _key = key;
                            }
                        }

                        if (_key == null && Inputs != null && Inputs.Count > 0)
                        {
                            Step inputStep = manager.GetStep(Inputs[0]);
                            if (inputStep.NeedsOutputTable() && inputStep.OutputTableId.HasValue)
                            {
                                IList<TableKey> keys = manager.GetTableKeyByTable(inputStep.OutputTableId.Value);
                                if (keys != null && keys.Count > 0)
                                {
                                    _key = keys[0];
                                }
                            }
                        }
                        //this causes serialization to fail if the key doesn't exist
                        //if (_key == null)
                        //{
                        //    throw new Exception("Failed to determine key for query. The assigned keyid " + KeyId.ToString() + " does not exist.");
                        //}
                    }
				}
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		[XmlIgnore]
		public string OutputTableName
		{
			get
			{
				if (OutputTableId > -1)
				{
					return Constants.TempTableNamePrefix + OutputTableId.ToString();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// The datetime that the step began execution.
		/// </summary>
		[XmlIgnore]
		[PetaPoco.Column]
		public DateTime? ExecutionStart { get; set; }


		[XmlIgnore]
		public StepIOCollection Inputs
		{
			get
			{
				if (_inputs == null)
				{
					_inputs = new StepIOCollection(Id, StepCollectionType.Input);
				}
				return _inputs;
			}
		}


		[XmlIgnore]
		public StepIOCollection Outputs
		{
			get
			{
				if (_outputs == null)
				{
					_outputs = new StepIOCollection(Id, StepCollectionType.Output);
				}
				return _outputs;
			}
		}


		[XmlIgnore]
		[PetaPoco.Column("Query")]
		public string XmlQuery
		{
			get
			{
				if (Query != null)
				{
					System.IO.StringWriter configString = new System.IO.StringWriter();
					lock (Query)
					{
						Query.SerializingToDatabase = true;

						XmlSerializer serializer = new XmlSerializer(typeof(Query));

						serializer.Serialize(configString, this.Query);

						Query.SerializingToDatabase = false;
					}
					return configString.ToString();
				}
				return null;
			}
			set
			{
				_xmlQuery = value;
				_query = null;
			}
		}


		/// <summary>
		/// Initializes a new instance of the Step class
		/// </summary>
		public Step()
		{
			StepType = StepType.Select;
			CreateDate = DateTime.Now;
		}


		public bool NeedsOutputTable()
		{
			return !IsRealTimeStep() && !IsPlanStep();
		}

		public bool RequiresVerification()
		{
			if (this.StepType == CampaignManagement.StepType.Output /*&& this.Query != null*/)
			{
				//all output steps require verification. Promotions, bonuses, coupons and surveys need to commit, but tables and flat files
				//may also need the member history table updated, so we'll need verification on everything.
				return true;
				/*
				var option = this.Query.OutputOption;
				if (option != null)
				{
					if (
						option.OutputType == OutputType.Coupon ||
						option.OutputType == OutputType.Offer ||
						option.OutputType == OutputType.Promotion ||
						option.OutputType == OutputType.Survey
						)
					{
						return true;
					}
				}
				*/
			}
			return false;
		}

		public void ClearResults()
		{
			LastRunDate = null;
			UILastRecordCount = null;
			LastError = null;
			VerificationState = Brierley.FrameWork.CampaignManagement.VerificationState.None;
		}

		public bool IsRealTimeStep()
		{
			return StepType == StepType.RealTimeAssignment ||
				StepType == StepType.RealTimebScript ||
				StepType == StepType.RealTimeOutput ||
				StepType == StepType.RealTimeSelect ||
				StepType == StepType.RealTimeInputProcessing ||
				StepType == StepType.RealTimeSurvey;
		}

		public bool IsPlanStep()
		{
			return StepType == CampaignManagement.StepType.Campaign || StepType == CampaignManagement.StepType.Wait;
		}

		public bool RequiresInputStep()
		{
			if (StepType == StepType.Select || IsRealTimeStep() || IsPlanStep())
			{
				return false;
			}
			return true;
		}
	}
}