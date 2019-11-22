using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWMobileGateway;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
	public static class MGAttributeSetContainerUtility
	{
		#region Fields
		private static string _className = "MGAttributeSetContainerUtility";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		#endregion

		#region Helper Methods
		private static void SetLastActivityDate(LWIntegrationConfig.AttributeSetDirective directive, object owner)
		{
			if (directive != null && directive.UpdateLastActivityDate)
			{
				Member member = null;
				if (owner != null)
				{
					if (owner.GetType() == typeof(Member))
					{
						member = (Member)owner;
					}
					else if (owner.GetType() == typeof(VirtualCard))
					{
						member = ((VirtualCard)owner).Member;
					}
					else
					{
						IClientDataObject obj = owner as ClientDataObject;
						if (obj != null)
						{
							SetLastActivityDate(directive, obj.Parent);
						}
					}
				}
				if (member != null)
				{
					member.LastActivityDate = DateTime.Now;
				}
			}
		}

		private static IList<IClientDataObject> GetChildAttributeSets(IAttributeSetContainer owner, string attSetname)
		{
			if (owner != null)
			{
				return owner.GetChildAttributeSets(attSetname);
			}
			else
			{
				return LWIntegrationUtilities.GetGlobalAttributeSet(attSetname, null);
			}
		}

		private static bool AreEqual(object attValue, object xmlValue)
		{
			if (attValue == null && xmlValue == null)
			{
				return true;
			}
			else if (attValue == null || xmlValue == null)
			{
				return false;
			}
			if (attValue is string)
			{
				return attValue.ToString() == xmlValue.ToString();
			}

			if (attValue is DateTime)
			{
				if (xmlValue is DateTime)
				{
					return (DateTime)attValue == (DateTime)xmlValue;
				}
				return false;
			}

			if (attValue is bool || xmlValue is bool)
			{
				return bool.Parse(attValue.ToString()) == bool.Parse(xmlValue.ToString());
			}

			if (attValue is double || attValue is decimal || attValue is short || attValue is int || attValue is long || attValue is float)
			{

				double left = Convert.ToDouble(attValue);
				double right = double.NaN;
				if (xmlValue is double || xmlValue is decimal || xmlValue is short || xmlValue is int || xmlValue is long || xmlValue is float)
				{
					right = Convert.ToDouble(xmlValue);
				}
				else
				{
					double.TryParse(xmlValue.ToString(), out right);
				}
				return left == right;
			}
			return false;
		}

		private static IClientDataObject FindMemberAttributeSetInList(
			IList<IClientDataObject> attList,
			LWIntegrationConfig.AttributeSetUpdateDirective updDirective,
			MGClientEntity aNode)
		{
			IClientDataObject obj = null;
			switch (updDirective.FindMethod)
			{
				case LWIntegrationConfig.AttributeSetFindMethodEnum.First:
					if (attList != null && attList.Count > 0)
					{
						obj = attList[0];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Last:
					if (attList != null && attList.Count > 0)
					{
						obj = attList[attList.Count - 1];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Index:
					int index = int.Parse(updDirective.FindValue);
					if (attList != null && attList.Count > 0 && index <= attList.Count)
					{
						obj = attList[index];
					}
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Newest:
					break;
				case LWIntegrationConfig.AttributeSetFindMethodEnum.Attribute:
					MGClientEntityAttribute findAttNode = aNode.GetAttribute(updDirective.FindValue);
					if (findAttNode != null)
					{
						foreach (IClientDataObject row in attList)
						{
							// it is important to perform the correct type of comparison
							object attValue = row.GetAttributeValue(updDirective.FindValue);
							if (attValue != null)
							{
								if (AreEqual(attValue, findAttNode.Value))
								{
									obj = row;
									break;
								}
							}
							else
							{
								//TODO: raise an exception.
							}
						}
					}
					break;
				default:
					break;
			}
			return obj;
		}

		public static IClientDataObject GetMemberAttributeSetToModify(
			LWIntegrationConfig.AttributeSetDirective directive,
			IAttributeSetContainer owner,
			AttributeSetMetaData def,
			MGClientEntity aNode)
		{
			string methodname = "GetMemberAttributeSetToModify";

			IList<IClientDataObject> attList = null;
			IClientDataObject thisRow = null;
			if (directive == null)
			{
				_logger.Debug(_className, methodname, string.Format("No directives provided for {0}.", def.Name));
				thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
				if (owner != null)
				{
					owner.AddChildAttributeSet(thisRow);
				}
			}
			else
			{
				if (directive.GetType().Name.Equals("AttributeSetCreateDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing create directives for {0}.", def.Name));
					LWIntegrationConfig.AttributeSetCreateDirective crtDirective = (LWIntegrationConfig.AttributeSetCreateDirective)directive;
					if (crtDirective.MarkTrueAttributes.Count > 0)
					{
						_logger.Debug(_className, methodname, string.Format("Loading {0}.", def.Name));
						attList = GetChildAttributeSets(owner, def.Name);
					}
					else if (crtDirective.LoadExisting)
					{
						attList = GetChildAttributeSets(owner, def.Name);
					}
					MGClientEntityAttribute attNode = ((MGClientEntity)aNode).GetAttribute(crtDirective.IfPresent);
					if (attNode != null && !string.IsNullOrEmpty(attNode.Value as string))
					{
						thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
						if (owner != null)
						{
							owner.AddChildAttributeSet(thisRow);
						}
					}
					else
					{
						_logger.Trace(_className, methodname,
							string.Format("Skipping the creation of {0} because attribute {1} is not present.",
							def.Name, crtDirective.IfPresent));
					}
					//Process mark active attributes.
					System.Collections.IList list = crtDirective.MarkTrueAttributes;
					if (thisRow != null)
					{
						foreach (string mt in list)
						{
							AttributeMetaData att = def.GetAttribute(mt);
							if (att.DataType != Enum.GetName(typeof(DataType), DataType.Boolean))
							{
								string err = "The type of " + def.Name + "." + mt + " has to be a boolean for it to be marked true.";
								_logger.Error(_className, methodname, err);
								throw new LWIntegrationException(err);
							}
							thisRow.SetAttributeValue(mt, true);
							if (attList != null)
							{
								foreach (IClientDataObject row in attList)
								{
									if (row != thisRow)
									{
										row.SetAttributeValue(mt, false);
									}
								}
							}
						}
					}
					//SetLastActivityDate(directive, owner);
				}
				else if (directive.GetType().Name.Equals("AttributeSetUpdateDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing update directives for {0}.", def.Name));
					LWIntegrationConfig.AttributeSetUpdateDirective updDirective = (LWIntegrationConfig.AttributeSetUpdateDirective)directive;
					// find the existing row in the attribute set
					attList = GetChildAttributeSets(owner, def.Name);
					if (attList == null || attList.Count == 0)
					{
						if (owner != null)
						{
							_logger.Debug(_className, methodname, string.Format("No existing rows exist for {0} for owner {1}.", def.Name, owner.MyKey));
						}
						else
						{
							_logger.Debug(_className, methodname, string.Format("No existing rows exist for {0}.", def.Name));
						}
						if (updDirective.CreateIfNotFound)
						{
							thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
							if (owner != null)
							{
								owner.AddChildAttributeSet(thisRow);
							}
						}
						else
						{
							//string err = "Could not find " + updDirective.FindBy + " in input Xml for update.";
							//string err = "Could not find required attribute to update in input Xml for update.";
							string err = string.Format("Could not find required attribute '{0}' in input Xml for update.", def.Name);
							_logger.Error(_className, methodname, err);
							throw new LWIntegrationException(err);
						}
					}
					else
					{
						if (owner != null)
						{
							_logger.Debug(_className, methodname, string.Format("{0} rows exist for {1} for owner {2}.", attList.Count, def.Name, owner.MyKey));
						}
						else
						{
							_logger.Debug(_className, methodname, string.Format("{0} rows exist for {1}.", attList.Count, def.Name));
						}
						thisRow = FindMemberAttributeSetInList(attList, updDirective, aNode);
						if (thisRow == null)
						{
							if (updDirective.CreateIfNotFound)
							{
								// create a new row
								thisRow = DataServiceUtil.GetNewClientDataObject(def.Name);
								if (owner != null)
								{
									owner.AddChildAttributeSet(thisRow);
								}
							}
							else
							{
								/*
								* This attribute set was supposed to be updated based on certain criteria
								* value.  However, that attribute value is not present in the input record.                            
								* */
								string err = "Could not find attribute to update in input Xml for update.";
								_logger.Error(_className, methodname, err);
								throw new LWIntegrationException(err);
							}
						}
					}
					//if (thisRow != null && thisRow.IsDirty)
					//{
					//    SetLastActivityDate(directive, owner);
					//}
				}
				else if (directive.GetType().Name.Equals("AttributeSetModifyDirective"))
				{
					_logger.Debug(_className, methodname, string.Format("Processing modify directives for {0}.  NOT IMPLEMENTED YET.", def.Name));
					// not implemented yet.
				}
			}
			return thisRow;
		}
		#endregion

		#region Public Methods
		public static void ProcessAttributeSet(
			LWIntegrationConfig config,
			IDictionary<string, LWIntegrationConfig.AttributeSetDirective> createDirectives,
			IAttributeSetContainer owner,
			MGClientEntity asNode,
			AttributeSetMetaData asDef,
			string dateConversionFormat,
			bool trimStrings,
			bool checkForChangedValues)
		{
			string methodName = "ProcessAttributeSet";

			_logger.Debug(_className, methodName, string.Format("Processing attribute set {0}.", asNode.Name));

			LWIntegrationConfig.AttributeSetDirective directive = null;
			IClientDataObject rowToModify = null;
			// first see if the member has any attributes
			if (!asNode.Name.Equals("Member") && !asNode.Name.Equals("VirtualCard"))
			{
				_logger.Debug(_className, methodName, string.Format("Processing attributes of {0}.", asNode.Name));
				directive = createDirectives.ContainsKey(asNode.Name) ? createDirectives[asNode.Name] : null;
				MGClientEntity entity = asNode as MGClientEntity;
				if (entity != null && entity.Attributes != null && entity.Attributes.Count > 0)
				{
					foreach (MGClientEntityAttribute attribute in entity.Attributes)
					{
						string attValue = (attribute.Value ?? string.Empty).ToString();
						// make sure that the attribute matches the metadata
						if (asDef.GetAttribute(attribute.Name) != null)
						{
							// set the attribute name                            
							bool addMissingPropertyAsTransient = directive != null ? directive.AddMissingPropertyAsTransient : false;
							LWIntegrationUtilities.ProcessAttribute(config, (IClientDataObject)owner, attribute.Name, attribute.DataType, attValue, dateConversionFormat, trimStrings, checkForChangedValues, addMissingPropertyAsTransient, asDef, asDef.GetAttribute(attribute.Name));
						}
						else if (attribute.Name == "StatusCode")
						{
							((IClientDataObject)owner).StatusCode = long.Parse(attValue);
							owner.IsDirty = true;
						}
						else
						{
							if (!string.IsNullOrEmpty(attValue))
							{
								_logger.Debug(_className, methodName,
									string.Format("Adding transient property {0} to {1}", attribute.Name, asNode.Name));
								((IClientDataObject)owner).AddTransientProperty(attribute.Name, attValue);
							}
						}
					}
					if (owner != null && owner.IsDirty)
					{
						SetLastActivityDate(directive, owner);
					}
				}
			}

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				_logger.Debug(_className, methodName, string.Format("Processing child attribute sets {0}.", asNode.Name));
				if (asNode.Name != "VirtualCard" && asNode.ChildEntities != null && asNode.ChildEntities.Count > 0)
				{
					foreach (MGClientEntity entity in asNode.ChildEntities)
					{
						AttributeSetMetaData sasDef = loyalty.GetAttributeSetMetaData(entity.Name);
						if (sasDef != null)
						{
							directive = createDirectives.ContainsKey(entity.Name) ? createDirectives[entity.Name] : null;
							rowToModify = GetMemberAttributeSetToModify(directive, owner, sasDef, entity);
							if (rowToModify != null)
							{
								ProcessAttributeSet(config, createDirectives, rowToModify, entity, sasDef, dateConversionFormat, trimStrings, checkForChangedValues);
							}
							else
							{
								_logger.Debug(_className, methodName, "No row could be obtained to modify for " + sasDef.Name);
							}
						}
						else
						{
							if (!asNode.Name.Equals("VirtualCard"))
							{
								// this is probably a severe error
								string msg = "Could not find metadata definition for " + asNode.Name;
								_logger.Error(_className, methodName, msg);
								throw new LWIntegrationException(msg) { ErrorCode = 9987 };
							}
						}
					}
				}
			}
		}
		#endregion
	}
}