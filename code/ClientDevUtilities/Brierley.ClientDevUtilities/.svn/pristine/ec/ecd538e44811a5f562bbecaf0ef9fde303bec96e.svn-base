using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.XML;

using Brierley.FrameWork.Rules.UIDesign;

namespace Brierley.FrameWork.Rules
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public abstract class RuleBase : ICloneable
	{
		#region fields
		private string _ruleName = string.Empty;
		private string _ruleDescription = string.Empty;
		private string _ruleid = string.Empty;
		private string _ruleversion = string.Empty;
		#endregion

		#region constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="RuleName"></param>
		public RuleBase(string RuleName)
		{
			this._ruleName = RuleName;
		}
		#endregion

		#region Serialization

		/// <summary>
		/// /Provides deserialization from a byte buffer.
		/// </summary>
		/// <param name="Data"></param>
		/// <returns></returns>
		public static RuleBase DeSerialize(string Data)
		{			
			return (RuleBase)RuleInstanceSerializationUtil.DeSerializeRuleInstance(Data);
		}

		/// <summary>
		/// Privides a byte[] array of survey data for mass storage.
		/// </summary>
		/// <returns>a byte[] array of binary survey data.</returns>
		public string Serialize(string fwkVersion)
		{            
			return RuleInstanceSerializationUtil.SerializeRuleInstance(this, fwkVersion);
		}

		#endregion

		#region Protected Helpers

		[NonSerialized]
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		/// <summary>
		/// 
		/// </summary>
		//protected Member lwmember = null;

		/// <summary>
		/// 
		/// </summary>
		//protected VirtualCard lwvirtualCard = null;

		/// <summary>
		/// 
		/// </summary>
		//protected VirtualCard ovirtualCard = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public void ResolveOwners(object owner, ref Member lwmember, ref VirtualCard lwvirtualCard)
		{
			string methodName = "ResolveOwners";

			if (owner == null)
			{
				_logger.Error("RuleBase", methodName, "Owner is null");
				return;
			}

			if (owner is Member)
			{
				lwmember = (Member)owner;
				lwvirtualCard = null;
				_logger.Debug("RuleBase", methodName, "Owner resolved from a member = Ipcode = " + lwmember.IpCode);
			}
			else if (owner is VirtualCard)
			{
				lwvirtualCard = (VirtualCard)owner;
				lwmember = lwvirtualCard.Member;
				_logger.Debug("RuleBase", methodName,
					string.Format("Owner resolved from virtual card. Member Ipcode = {0}, Virtual card key = {1}, Loyalty id = {2}.", lwmember.IpCode, lwvirtualCard.VcKey, lwvirtualCard.LoyaltyIdNumber));
			}
			else
			{
				IClientDataObject obj = owner as ClientDataObject;
				if (obj != null)
				{
					_logger.Debug("RuleBase", methodName,
					string.Format("Owner resolved from attribute set."));
					ResolveOwners(obj.Parent, ref lwmember, ref lwvirtualCard);
				}
				else
				{
					_logger.Debug("RuleBase", methodName,
						string.Format("Unknown type encountered for resolving ownership. The type of object the rule is invoked on = {0}", owner.GetType().FullName)
						);
					lwmember = null;
					lwvirtualCard = null;
				}
			}
		}

        public void AddRuleResult(ContextObject context, ContextObject.RuleResult result)
        {
            context.Results.Add(result);
            if (context.InvokingRow != null)
            {
                context.InvokingRow.Results.Add(result);
            }
            else if (context.Owner != null)
            {
                context.Owner.Results.Add(result);
            }             
        }

		#endregion

		#region General Rule Properties

        public abstract string DisplayText { get; }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Rule Version")]
		[Description("A client specific versioning property")]
		[RuleProperty(false, false, false, null, false, true)]
		public string RuleVersion
		{
			get { return _ruleversion; }
			set { _ruleversion = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Rule Id")]
		[Description("Defines the ID of this rule")]
		public string RuleId
		{
			get { return _ruleid; }
		}


		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Rule Name")]
		[Description("Defines the name of this rule")]
		public string RuleName
		{
			get { return _ruleName; }
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlElement(Namespace = "http://www.brierley.com")]
		[Browsable(true)]
		[CategoryAttribute("General")]
		[DisplayName("Rule Description")]
		[Description("A description of this rule")]
        [RuleProperty(false, false, false, null, false, true)]
		public string RuleDescription
		{
			get { return _ruleDescription; }
			set { _ruleDescription = value; }
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public virtual void Validate()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Context"></param>
		/// <param name="PreviousResultCode"></param>
		/// <returns></returns>
		public virtual void Invoke(ContextObject Context)
		{
			return;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public System.Reflection.MemberInfo[] GetConfigurableMembers()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();			
			foreach (System.Reflection.MemberInfo memberInfo in this.GetType().FindMembers(System.Reflection.MemberTypes.Property, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null))
			{
				object[] attributes = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), true);
				if (attributes.Length > 0)
				{

					list.Add(new SortableMemberInfo(memberInfo));
				}
			}
			list.Sort();
			System.Collections.ArrayList nlist = new System.Collections.ArrayList();
			foreach (SortableMemberInfo s in list)
			{
				nlist.Add(s.Info);
			}
			return nlist.ToArray(typeof(System.Reflection.MemberInfo)) as System.Reflection.MemberInfo[];
		}

		/// <summary>
		/// This method gets the integral value of the enumeration.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public object GetEnumerationValue(Type t, string s)
		{
			// first get the enum's underlying type.
			Type ut = Enum.GetUnderlyingType(t);
			object v = Convert.ChangeType(Enum.Parse(t, s), ut);
			return v;
		}
        
		#region ICloneable Members

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
            using (DataService dataService = LWDataServiceUtil.DataServiceInstance())
                return RuleBase.DeSerialize(this.Serialize(dataService.Version));
		}

		#endregion

        #region Migrartion Helpers
        
        #region Helpers for Migration
        protected string BuildSourceHeirarchy(Category srcCat, string str, ServiceConfig sourceConfig)
        {
            if (srcCat.ParentCategoryID <= 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str += ";";
                }
                str += srcCat.Name;
            }
            else
            {
				using (var svc = new ContentService(sourceConfig))
				{
					Category parent = svc.GetCategory(srcCat.ParentCategoryID);
					string catstr = BuildSourceHeirarchy(parent, str, sourceConfig);
					str = catstr + ";" + srcCat.Name;
				}
            }
            return str;
        }

        protected Category RetrieveDestinationParentCategoryFromSource(Category srcCat, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
			string str = BuildSourceHeirarchy(srcCat, string.Empty, sourceConfig);
            string[] tokens = str.Split(';');
            Category cat = null;
            Category parent = null;
            if (tokens.Length >= 1)
            {
				using (var svc = new ContentService(targetConfig))
				{
					for (int i = 0; i < tokens.Length; i++)
					{
						long parentId = parent != null ? parent.ID : 0;
						cat = svc.GetCategory(parentId, tokens[i]);
						parent = cat;
					}
				}
            }
            return parent;
        }
        #endregion

		public abstract RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig);
        public virtual List<string> GetBscriptsToMove()
        {
            return new List<string>();
        }

        #endregion
    }
}
