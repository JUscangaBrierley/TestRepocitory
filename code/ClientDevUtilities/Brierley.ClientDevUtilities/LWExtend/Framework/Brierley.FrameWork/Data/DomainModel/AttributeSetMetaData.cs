using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("AttributeSetCode", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AttributeSet")]
    [AuditLog(true)]
	public class AttributeSetMetaData : LWCoreObjectBase
	{
		private Dictionary<string, AttributeMetaData> _attributesByName;
		private IList<AttributeMetaData> _attributes = new List<AttributeMetaData>();

		/// <summary>
		/// Gets or sets the ID for the current AttributeSet
		/// </summary>
		[PetaPoco.Column("AttributeSetCode", IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current AttributeSet
		/// </summary>
		[PetaPoco.Column("AttributeSetName", Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
		public string Name { get; set; }

        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string DisplayText { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the Type for the current AttributeSet
        /// </summary>
        [PetaPoco.Column("TypeCode", IsNullable = false)]
		public AttributeSetType Type { get; set; }

		/// <summary>
		/// Gets or sets the Category for the current AttributeSet
		/// </summary>
		[PetaPoco.Column("CategoryCode", IsNullable = false)]
		public AttributeSetCategory Category { get; set; }

		/// <summary>
		/// Gets or sets the EditableFromProgram for the current AttributeSet
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool EditableFromProgram { get; set; }

		/// <summary>
		/// Gets or sets the EditableFromCampaign for the current AttributeSet
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public Boolean EditableFromCampaign { get; set; }

		/// <summary>
		/// Gets or sets the ParentID for the current AttributeSet
		/// </summary>
		[PetaPoco.Column("ParentAttributeSetCode", IsNullable = false)]
		public long ParentID { get; set; }

		/// <summary>
		/// Gets or sets the TableCreationDate for the current AttributeSet
		/// </summary>
        [PetaPoco.Column]
		public DateTime? TableCreationDate { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public IList<AttributeMetaData> Attributes
		{
			get { return this._attributes; }
			set
			{
                this._attributes = value ?? new List<AttributeMetaData>();
				this._attributesByName = new Dictionary<string, AttributeMetaData>();
				if (this._attributes != null && this._attributes.Count > 0)
				{
					foreach (AttributeMetaData att in this._attributes)
					{
						this._attributesByName.Add(att.Name.ToLower(), att);
					}
				}
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public List<AttributeSetMetaData> ChildAttributeSets { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public List<RuleTrigger> RuleTriggers
		{
			get
			{
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					return service.GetRuleByAttributeSetCode(ID);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the AttributeSet class
		/// </summary>
		public AttributeSetMetaData()
		{
			ParentID = -1;
			_attributes = new List<AttributeMetaData>();
			ChildAttributeSets = new List<AttributeSetMetaData>();
		}

		public AttributeSetMetaData GetChildAttributeSet(string attSetName)
		{
			AttributeSetMetaData attSet = null;
			if (ChildAttributeSets != null && ChildAttributeSets.Count > 0)
			{
				foreach (AttributeSetMetaData child in ChildAttributeSets)
				{
					if (child.Name == attSetName)
					{
						attSet = child;
					}
					else
					{
						attSet = child.GetChildAttributeSet(attSetName);
					}
					if (attSet != null)
					{
						break;
					}
				}
			}
			return attSet;
		}

		public AttributeSetMetaData GetChildAttributeSet(long attSetCode)
		{
			AttributeSetMetaData attSet = null;
			if (ChildAttributeSets != null && ChildAttributeSets.Count > 0)
			{
				foreach (AttributeSetMetaData child in ChildAttributeSets)
				{
					if (child.ID == attSetCode)
					{
						attSet = child;
					}
					else
					{
						attSet = child.GetChildAttributeSet(attSetCode);
					}
					if (attSet != null)
					{
						break;
					}
				}
			}
			return attSet;
		}

		public AttributeMetaData GetAttribute(long attId)
		{
			AttributeMetaData attribute = null;
			lock (_attributes)
			{
				foreach (AttributeMetaData att in _attributes)
				{
					if (att.ID == attId)
					{
						attribute = att;
						break;
					}
				}
			}
			return attribute;
		}

		public AttributeMetaData GetAttribute(string attName)
		{
			attName = attName.ToLower();
			AttributeMetaData attribute = null;
			if (_attributesByName != null && _attributesByName.ContainsKey(attName))
				attribute = _attributesByName[attName];
			return attribute;
		}

		public int GetNumberOfAttributes()
		{
			return _attributes.Count;
		}

		// TODO: Implement this.
		public IList<ValidatorTrigger> GetValidatorTriggers()
		{
			return new List<ValidatorTrigger>();
		}

		public AttributeSetMetaData Clone()
		{
			return Clone(new AttributeSetMetaData());
		}

		public AttributeSetMetaData Clone(AttributeSetMetaData dest)
		{
			dest.Name = Name;
			dest.DisplayText = DisplayText;
			dest.Description = Description;
            dest.Alias = Alias;
			dest.Type = Type;
			dest.Category = Category;
			dest.EditableFromProgram = EditableFromProgram;
			dest.EditableFromCampaign = EditableFromCampaign;
			dest.ParentID = ParentID;
			dest.TableCreationDate = TableCreationDate;
			dest.Attributes.Clear();
			foreach (AttributeMetaData att in Attributes)
			{
				dest.Attributes.Add(att.Clone());
			}
			return (AttributeSetMetaData)base.Clone(dest);
		}


		/// <summary>
		/// This property is really there for Xml serialization because serialization does not work
		/// on interfaces.
		/// </summary>
		public List<AttributeMetaData> AsetAttributes
		{
			get
			{
				return new List<AttributeMetaData>(Attributes);
			}
			set
			{
				Attributes = new List<AttributeMetaData>(value);
			}
		}

		#region AuditLogging
		public override LWObjectAuditLogBase GetArchiveObject()
		{
            AttributeSetMetaData_AL ar = new AttributeSetMetaData_AL()
            {
                ObjectId = this.ID,
                Name = this.Name,
                DisplayText = this.DisplayText,
                Description = this.Description,
                Alias = this.Alias,
				Type = this.Type,
				Category = this.Category,
				ParentID = this.ParentID,
				EditableFromProgram = this.EditableFromProgram,
				EditableFromCampaign = this.EditableFromCampaign,
				TableCreationDate = this.TableCreationDate,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
		#endregion
	}

    [PetaPoco.PrimaryKey("", sequenceName = "SEQ_ROWKEY", autoIncrement = false)]
    public class AttributeSetTableSequence
    {
    }
}