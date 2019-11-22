//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class ClientDataObject : AttributeSetContainer, IClientDataObject
	{
		private const string _className = "ClientDataObject";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private PropertyInfo[] _pi = null;
		private Dictionary<string, PropertyInfo> _propertyInfoByName;
		private long _parentRowKey = -1;

		[PetaPoco.Column("A_RowKey")]
		public virtual long RowKey
		{
			get { return MyKey; }
			set { MyKey = value; }
		}

		[PetaPoco.Column("A_ParentRowKey")]
		public virtual long ParentRowKey
		{
			get
			{
				if (Parent != null)
				{
					_parentRowKey = Parent.MyKey;
				}
				return _parentRowKey;
			}
			set
			{
				_parentRowKey = value;
				if (Parent != null)
				{
					Parent.MyKey = _parentRowKey;
				}
			}
		}

		[PetaPoco.Column]
		public virtual long StatusCode { get; set; }

		[PetaPoco.Column]
		public virtual long? LastDmlId { get; set; }

		public virtual void SetLinkKey(IAttributeSetContainer owner, IAttributeSetContainer root)
		{
			if (root != null)
			{
				if (root.GetType() == typeof(Member))
				{
					SetAttributeValue("IpCode", root.MyKey);
				}
				else if (root.GetType() == typeof(VirtualCard))
				{
					SetAttributeValue("VcKey", root.MyKey);
				}
				else
				{
					SetLinkKey(owner, root.Parent);
				}
			}
		}

		public virtual void SetAttributeValue(string attributeName, object attributeValue)
		{
			const string methodName = "SetAttributeValue";
			AttributeMetaData attMeta = null;
			try
			{
				if (!IsImplicitProperty(attributeName))
				{
					// make sure that this attribute exists
					AttributeSetMetaData attSetMeta = GetMetaData();
					if (attSetMeta == null)
					{
						string msg = string.Format("Unable to get meta data for attribute set {0}", GetAttributeSetName());
						throw new LWMetaDataException(msg);
					}

					attMeta = attSetMeta.GetAttribute(attributeName);
					if (attMeta == null)
					{
						string msg = string.Format(
							"Unable to get meta data for attribute {0} in attribute set {1}",
							attributeName,
							GetAttributeSetName());
						throw new LWMetaDataException(msg);
					}
				}
			}
			catch (Exception ex)
			{
				string msg = string.Empty;
				if (attributeValue != null)
				{
					msg = string.Format("Unable to set value of attribute {0} to {1}", attributeName, attributeValue.ToString());
				}
				else
				{
					msg = string.Format("Unable to set value of attribute {0} null", attributeName);
				}
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
			SetAttributeValue(attributeName, attributeValue, attMeta);
		}

		public virtual void SetAttributeValue(string attributeName, object attributeValue, AttributeMetaData attMeta)
		{
			const string methodName = "SetAttributeValue";
			try
			{
				if (!IsImplicitProperty(attributeName))
				{
					if (attMeta == null)
					{
						throw new ArgumentNullException("attMeta");
					}
					Validate(attMeta, attributeValue);
				}
				PropertyInfo info = GetProperty(attributeName);
				if (info == null)
				{
					string msg = string.Format("Unable to find attribute {0}. Possible the data model assembly is out of date.", attributeName);
					throw new LWMetaDataException(msg);
				}
				info.SetValue(this, attributeValue, null);
				IsDirty = true;
			}
			catch (Exception ex)
			{
				string msg = string.Empty;
				if (attributeValue != null)
				{
					msg = string.Format("Unable to set value of attribute {0} to {1}", attributeName, attributeValue.ToString());
				}
				else
				{
					msg = string.Format("Unable to set value of attribute {0} null", attributeName);
				}
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
		}

		public virtual object GetAttributeValue(string attributueName)
		{
			PropertyInfo info = GetProperty(attributueName);
			if (info == null)
			{
				string msg = string.Format("Unable to find attribute {0}", attributueName);
				throw new LWMetaDataException(msg);
			}
			return info.GetValue(this, null);
		}

        public virtual bool HasAttribute(string attributeName)
        {
            return GetProperty(attributeName) != null;
        }

		public override IAttributeSetContainer Clone()
		{
			return Clone(new ClientDataObject());
		}

		public override IAttributeSetContainer Clone(IAttributeSetContainer dest)
		{
			IClientDataObject cobj = (IClientDataObject)dest;
			cobj.ParentRowKey = ParentRowKey;
			cobj.StatusCode = StatusCode;

			return (IClientDataObject)base.Clone(dest);
		}

		protected static bool IsImplicitProperty(string propName)
		{
			bool ret = false;
			switch (propName)
			{
				case "MyKey":
				case "IpCode":
				case "VcKey":
				case "RowKey":
				case "ParentRowKey":
				case "StatusCode":
				case "LastDmlDate":
					ret = true;
					break;
				default:
					break;
			}
			return ret;
		}

		protected static void Validate(AttributeMetaData attMeta, object attributeValue)
		{
			const string method = "Validate";
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				var triggers = loyalty.GetValidatorTriggers(attMeta.ID);
				if (triggers != null && triggers.Count > 0)
				{
					foreach (ValidatorTrigger trigger in triggers)
					{
						try
						{
							trigger.Validate(attributeValue);
						}
						catch (Exception ex)
						{
							_logger.Error(_className, method, ex.Message, ex);
							if (trigger.OnErrorContinue == false)
							{
								throw;
							}
						}
					}
				}
			}
		}

		protected void LoadProperties()
		{
			if (_pi == null)
			{
				_pi = GetType().GetProperties();
				_propertyInfoByName = new Dictionary<string, PropertyInfo>();
				foreach (PropertyInfo pi in _pi)
					_propertyInfoByName.Add(pi.Name.ToUpper(), pi);
			}
		}

		protected PropertyInfo GetProperty(string attName)
		{
			LoadProperties();
			if (_propertyInfoByName != null && _propertyInfoByName.ContainsKey(attName.ToUpper()))
				return _propertyInfoByName[attName.ToUpper()];
			return null;
		}
	}
}
