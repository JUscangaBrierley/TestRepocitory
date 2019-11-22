//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Config;

namespace Brierley.FrameWork.CodeGen
{
    public class DataModelGenerator
    {
        private const string _className = "DataModelGenerator";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        //private ILWDataService metaMgr = null;
        IList<string> messages = new List<string>();        

        private enum AttributeSetCategoryCode
        {
            NotSet = 0,
            TransactionProfiles = 1,
            BasicProfiles = 2
        }

        protected string xsdFile = null;               
        protected bool overwrite;
                        
        public DataModelGenerator(string schemaFile, bool overwrite)
        {
            xsdFile = schemaFile;            
            this.overwrite = overwrite;                        
        }

        public IList<string> GenerateDataModel()
        {            
            XmlReader reader = new XmlTextReader(xsdFile);            
            try
            {
                #region Namespace specific method
                //LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
                //string tns = string.Format("http://{0}.MemberProcessing.Schemas.{0}", ctx.Organization);
                //XmlSchemaSet set = new XmlSchemaSet();
                //set.Add(tns, reader);
                //set.Compile();
                //if (set.IsCompiled)
                //{
                //    // we expect to have the root element AttributeSets.
                //    foreach (XmlSchemaElement parentElement in set.GlobalElements.Values)
                //    {
                //        if (parentElement.Name == "AttributeSets")
                //        {
                //            // found the root we are looking for.
                //            ProcessAttributeSetsRoot(parentElement);
                //        }
                //    }
                //}
                #endregion
                #region Obsolete but namespace independent method
                XmlSchema xmlSchema = XmlSchema.Read(reader,
                    new ValidationEventHandler(ShowCompileError));
                xmlSchema.Compile(new ValidationEventHandler(ShowCompileError));
                if (xmlSchema.IsCompiled)
                {
                    // we expect to have the root element AttributeSets.
                    foreach (XmlSchemaElement parentElement in xmlSchema.Elements.Values)
                    {
                        if (parentElement.Name == "AttributeSets")
                        {
                            // found the root we are looking for.
                            ProcessAttributeSetsRoot(parentElement);
                        }
                    }
                }
                #endregion
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return messages;
        }

        private void ProcessAttributeSetsRoot(XmlSchemaElement root)
        {
            XmlSchemaComplexType ct = root.ElementSchemaType as XmlSchemaComplexType;
            //XmlSchemaComplexType ct = root.ElementType as XmlSchemaComplexType;    //Casting to complex type
            if (ct != null)
            {
                XmlSchemaSequence seq = (XmlSchemaSequence)ct.ContentTypeParticle;
                foreach (XmlSchemaParticle p in seq.Items)
                {
                    XmlSchemaElement elem = p as XmlSchemaElement; //Check if particle in seq is XmlSchemaElement
                    if (elem != null)
                    {
                        if (elem.Name == "Member")
                        {
                            ProcessMember(elem);
                            Debug.WriteLine(elem.QualifiedName.ToString());
                        }
                        else if (elem.Name == "Global")
                        {
                            ProcessGlobal(elem);
                            Debug.WriteLine(elem.QualifiedName.ToString());
                        }
                    }
                }                
            }
        }

        private void ProcessMember(XmlSchemaElement member)
        {
            XmlSchemaComplexType ct = member.ElementSchemaType as XmlSchemaComplexType;            
            if (ct != null)
            {
                XmlSchemaSequence seq = (XmlSchemaSequence)ct.ContentTypeParticle;
                foreach (XmlSchemaParticle p in seq.Items)
                {
                    XmlSchemaElement elem = p as XmlSchemaElement; //Check if particle in seq is XmlSchemaElement
                    if (elem != null)
                    {
                        if (elem.Name == "VirtualCard")
                        {
                            ProcessVirtualCard(elem);
                            Debug.WriteLine(elem.QualifiedName.ToString());
                        }
                        else
                        {
                            ProcessAttributeSet(elem,AttributeSetType.Member,-1);
                            Debug.WriteLine(elem.QualifiedName.ToString());
                        }
                    }
                }
            }
        }

        private static XmlSchemaSequence GetSequenceFromComplex(XmlSchemaComplexType complex)
        {
            XmlSchemaSequence seq = null;
            try
            {
                seq = (XmlSchemaSequence)complex.ContentTypeParticle;
            }
            catch (InvalidCastException)
            {
                seq = null;
            }
            catch (Exception)
            {
                throw;
            }
            return seq;
        }

        private void ProcessVirtualCard(XmlSchemaElement vc)
        {
            XmlSchemaComplexType ct = vc.ElementSchemaType as XmlSchemaComplexType;            
            if (ct != null)
            {
                XmlSchemaSequence seq = GetSequenceFromComplex(ct);                                
                if (seq != null)
                {
                    foreach (XmlSchemaParticle p in seq.Items)
                    {
                        XmlSchemaElement elem = p as XmlSchemaElement; //Check if particle in seq is XmlSchemaElement
                        if (elem != null)
                        {
                            ProcessAttributeSet(elem, AttributeSetType.VirtualCard, -1);
                            Debug.WriteLine(elem.QualifiedName.ToString());
                        }
                    }
                }
            }
        }

        private void ProcessGlobal(XmlSchemaElement global)
        {
            XmlSchemaComplexType ct = global.ElementSchemaType as XmlSchemaComplexType;            
            if (ct != null)
            {
                XmlSchemaSequence seq = GetSequenceFromComplex(ct); 
                //XmlSchemaSequence seq = (XmlSchemaSequence)ct.ContentTypeParticle;
                if (seq != null)
                {
                    foreach (XmlSchemaParticle p in seq.Items)
                    {
                        XmlSchemaElement elem = p as XmlSchemaElement; //Check if particle in seq is XmlSchemaElement
                        if (elem != null)
                        {
                            ProcessAttributeSet(elem, AttributeSetType.Global, -1);
                            Debug.WriteLine(elem.QualifiedName.ToString());
                        }
                    }
                }
            }
        }
        
        protected static DataType GetDataTypeCode(XmlSchemaAttribute attribute)
        {
            DataType dataType = DataType.String;
            XmlSchemaSimpleType type = attribute.AttributeSchemaType;
            if (type.TypeCode == XmlTypeCode.Long)
            {
                dataType = DataType.Number;                
            }
            else if (type.TypeCode == XmlTypeCode.Integer)
            {
                dataType = DataType.Integer;
            }
            else if (type.TypeCode == XmlTypeCode.Double || type.TypeCode == XmlTypeCode.Decimal)
            {
                dataType = DataType.Decimal;                
            }            
            else if (type.TypeCode == XmlTypeCode.String)
            {
                dataType = DataType.String;                
            }
            else if (type.TypeCode == XmlTypeCode.Date || type.TypeCode == XmlTypeCode.DateTime)
            {
                dataType = DataType.Date;                
            }
            else if (type.TypeCode == XmlTypeCode.Boolean)
            {
                dataType = DataType.Boolean;                
            }            
            return dataType;
        }

		protected virtual AttributeSetMetaData ProcessAttributeSet(XmlSchemaElement attSet, AttributeSetType type, long parentId)
		{
			using (var metaMgr = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				bool OkToCreate = true;
				Console.Out.WriteLine("Processing attribute set: " + attSet.Name);
				AttributeSetMetaData nAttSet = null;
				XmlSchemaComplexType ct = attSet.ElementSchemaType as XmlSchemaComplexType;
				if (ct != null)
				{
					AttributeSetMetaData asetMetadata = metaMgr.GetAttributeSetMetaData(attSet.Name);
					if (asetMetadata == null || (asetMetadata != null && overwrite))
					{
						long existingID = -1;
						if (asetMetadata != null && overwrite)
						{
							// delete the existing attribute set
							try
							{
								metaMgr.DeleteAttributeSet(asetMetadata.ID);
								string msg = "Deleted existing attribute set " + asetMetadata.Name;
								_logger.Trace(_className, "ProcessAttributeSet", msg);
								existingID = asetMetadata.ID;
							}
							catch (LWAttributeSetHasDataException)
							{
								// This attribute set could not be deleted because it may already have some data.
								string msg = "Attribute set " + asetMetadata.Name + " could not be deleted because is not empty.";
								_logger.Error(_className, "ProcessAttributeSet", msg);
								messages.Add(msg);
								OkToCreate = false;
							}
							catch (Exception)
							{
								// this may simply mean that the attribute set may have already been deleted because
								// the parent attribute set was deleted.                            
							}
						}
						if (OkToCreate)
						{
							AttributeSetCategory category = AttributeSetCategory.BasicProfiles;
							if (type == AttributeSetType.VirtualCard)
								category = AttributeSetCategory.TransactionProfiles;
							// add attribute set.                                                
							try
							{
                                AttributeSetMetaData attSetMeta = new AttributeSetMetaData()
                                {
                                    Name = attSet.Name,
                                    Type = type,
                                    Category = category,
                                    ParentID = parentId,
                                    EditableFromCampaign = false,
                                    EditableFromProgram = false                                 
                                };

                                // Look through the annotations to see if Alias, Description, or DisplayText were set
                                if (attSet.Annotation != null && attSet.Annotation.Items != null && attSet.Annotation.Items.Count > 0)
                                {
                                    foreach (var item in attSet.Annotation.Items)
                                    {
                                        XmlSchemaAppInfo appInfo = item as XmlSchemaAppInfo;
                                        if (appInfo == null)
                                            continue;

                                        XmlNode markup = appInfo.Markup[0];
                                        string text = markup.InnerText;
                                        string[] tokens = text.Split('=');
                                        if (tokens[0].Equals("Alias", StringComparison.InvariantCultureIgnoreCase))
                                            attSetMeta.Alias = tokens[1];
                                        else if (tokens[0].Equals("Description", StringComparison.InvariantCultureIgnoreCase))
                                            attSetMeta.Description = tokens[1];
                                        else if (tokens[0].Equals("DisplayText", StringComparison.InvariantCultureIgnoreCase))
                                            attSetMeta.DisplayText = tokens[1];
                                    }
                                }

								metaMgr.CreateAttributeSet(attSetMeta);
                                nAttSet = attSetMeta;
							}
							catch (Exception ex)
							{
								_logger.Error(_className, "ProcessAttributeSet", "Exception creating attribute set " + attSet.Name + ": " + ex.Message, ex);
								throw;
							}
							// process its attributes
							try
							{
								foreach (XmlSchemaAttribute attribute in ct.Attributes)
								{
									Console.Out.WriteLine("This is an attribute: " + attribute.Name);
									XmlSchemaAnnotation attAnn = attribute.Annotation;
                                    AttributeMetaData meta = new AttributeMetaData()
                                    {
                                        Name = attribute.Name,
                                        DisplayText = attribute.Name,
                                        Description = attribute.Name,
                                        DefaultValues = string.Empty,
                                        AttributeSetCode = nAttSet.ID,
                                        IsUnique = false,
                                        IsRequired = attribute.Use == XmlSchemaUse.Required,
                                        EncryptionType = AttributeEncryptionType.None,
                                        MinLength = 0,
                                        MaxLength = 100,
                                        DataType = Enum.GetName(typeof(DataType), GetDataTypeCode(attribute)),
                                        Status = 1
                                    };
                                    Type metaType = meta.GetType();
									if (attAnn != null)
									{
										foreach (var item in attAnn.Items)
										{
											XmlSchemaAppInfo appInfo = item as XmlSchemaAppInfo;
											if (appInfo == null)
											{
												throw new Exception("Unable to cast item as XmlSchemaAppInfo");
											}
											XmlNode markup = appInfo.Markup[0];
											string text = markup.InnerText;
											string[] tokens = text.Split('=');

                                            var propertyInfo = metaType.GetProperty(tokens[0]);
                                            if(propertyInfo != null)
                                            {
                                                if (propertyInfo.PropertyType.IsEnum)
                                                    propertyInfo.SetValue(meta, Enum.Parse(propertyInfo.PropertyType, tokens[1]));
                                                else
                                                    propertyInfo.SetValue(meta, Convert.ChangeType(tokens[1], propertyInfo.PropertyType));
                                            }
											else if (tokens[0] == "LWType")
											{
												try
												{
                                                    meta.DataType = Enum.GetName(typeof(DataType), Enum.Parse(typeof(DataType), tokens[1]));
												}
												catch (Exception)
												{
													string msg = string.Format("Invalid LWType {0} for Attribute {1} of AttributeSet {2} provided.", tokens[1], attribute.Name, attSet.Name);
													throw new LWDataModelGenException(msg);
												}
											}
										}
									}
									try
									{
                                        metaMgr.CreateAttribute(meta);
									}
									catch (Exception ex)
									{
										_logger.Error(_className, "ProcessAttributeSet", "Exception was ignored", ex);
									}
								}
								//metaMgr.CreateAttributeSetTable(attSetCode);
							}
							catch (Exception)
							{
								// failed to create one or more attributes
								// undo 
								// log error
								metaMgr.DeleteAttributeSet(nAttSet.ID);
								throw;
							}
						}
					}
					if (ct.ContentTypeParticle.GetType() == typeof(XmlSchemaSequence))
					{
						XmlSchemaSequence seq = (XmlSchemaSequence)ct.ContentTypeParticle;
						foreach (XmlSchemaParticle p in seq.Items)
						{
							XmlSchemaElement elem = p as XmlSchemaElement; //Check if particle in seq is XmlSchemaElement
							if (elem != null)
							{
								long itsParentId = nAttSet != null ? nAttSet.ID : -1;
                                ProcessAttributeSet(elem, type, itsParentId);
								Debug.WriteLine(elem.QualifiedName.ToString());
							}
						}
					}
				}
				return nAttSet;
			}
		}

        private static void ShowCompileError(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.WriteLine("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
            else
                Console.WriteLine("\tValidation error: " + args.Message);

        }
    }
}
