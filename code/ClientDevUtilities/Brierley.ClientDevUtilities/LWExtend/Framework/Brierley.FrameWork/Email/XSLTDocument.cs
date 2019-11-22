using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;

namespace Brierley.FrameWork.Email
{
	public class XsltDocument
	{
		private Document _document = null;
		private Template _template = null;
		private FieldCollection _fields = null;

		public string XslHtml { get; private set; }

		public string XslText { get; private set; }

		public XsltDocument(long documentId, bool isPreview, bool postProcess = true)
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{

				var xslt = (XsltDocument)content.CacheManager.Get(CacheRegions.XsltByDocumentId, documentId);
				if (xslt != null)
				{
					XslHtml = xslt.XslHtml;
					XslText = xslt.XslText;
					return;
				}


				_document = content.GetDocument(documentId);
				_template = content.GetTemplate(_document.TemplateID);
				_fields = new FieldCollection(_template.Fields);


				if (!string.IsNullOrEmpty(_template.HtmlContent))
				{
					XslHtml = GenerateXslt(_template.HtmlContent, isPreview, false);
				}
				else
				{
					throw new Exception(string.Format("Template '{0}' ({1}) has no html content so unable to generate XSLT.", _template.Name, _template.ID));
				}

				if (!string.IsNullOrEmpty(_template.TextContent))
				{
					XslText = GenerateXslt(_template.TextContent, isPreview, true);
				}

				if (postProcess && _document.PostProcessors != null && _document.PostProcessors.Count > 0)
				{
					ExecutePostProcessors();
				}

				content.CacheManager.Update(CacheRegions.XsltByDocumentId, documentId, this);
			}
		}

		private void ExecutePostProcessors()
		{
			if (_document.PostProcessors == null || _document.PostProcessors.Count == 0)
			{
				//no processors configured
				return;
			}

			using (var svc = LWDataServiceUtil.DataServiceInstance())
			{
                foreach (var name in _document.PostProcessors)
                {
                    // Check if it's built-in first
                    System.Reflection.Assembly assembly = ClassLoaderUtil.LoadAssemblyFromName("Brierley.LoyaltyNavigator", false);
                    Type nativeType = assembly.GetExportedTypes().Where(x => x.Name == name && typeof(IXsltPostProcessor).IsAssignableFrom(x)).FirstOrDefault();
                    IXsltPostProcessor processor = null;
                    if (nativeType != null)
                    {
                        processor = ClassLoaderUtil.CreateInstance(assembly, nativeType.Name) as IXsltPostProcessor;
                    }
                    else // Check if it's custom
                    {
                        RemoteAssembly.ComponentReference reference = svc.GetComponentReference(CustomComponentTypeEnum.XsltPostProcessor, name);
                        if (reference == null)
                        {
                            throw new Exception(string.Format("Unable to execute XSLT post-processor {0}. The component does not exist.", name));
                        }

                        string className = reference.ClassName.Substring(0, reference.ClassName.IndexOf(","));
                        processor = ClassLoaderUtil.CreateInstance(reference.Assembly, className) as IXsltPostProcessor;
                    }

                    if (processor == null)
                    {
                        throw new Exception(string.Format("Unable to create instance of XSLT post-processor {0}.", name));
                    }

                    XslHtml = processor.Process(XslHtml, false);
                    if (!string.IsNullOrEmpty(_template.TextContent))
                    {
                        XslText = processor.Process(XslText, true);
                    }
                }
			}
		}


		private string GenerateXslt(string templateContent, bool isPreview, bool isTextVersion)
		{
			Member member = new Member();
			StringBuilder xslt = new StringBuilder();
			MatchCollection matches = Regex.Matches(templateContent, @"<contentarea.*?>.*?</contentarea>");
			int nextpos = 0;

			List<TemplateContentArea> areas = null;
			if (isTextVersion)
			{
				areas = _template.GetTextContentAreas();
			}
			else
			{
				areas = _template.GetHtmlContentAreas();
			}

			if (matches.Count > 0)
			{
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					foreach (Match m in matches)
					{
						string name = XElement.Parse(m.ToString()).Attribute("name").Value;

						xslt.Append(ConvertContentSample(templateContent.Substring(nextpos, m.Index - nextpos)));

						nextpos = m.Index + m.Length;

						ContentArea area = null;
						XElement element = _document.GetHtmlAreaContent(name);
						if (element != null)
						{
							area = new ContentArea(element);

							if (area.HasMarkup)
							{
								string areaMarkup = string.Empty;
								TemplateContentArea templateArea = null;

								if (areas != null && areas.Count > 0)
								{
									foreach (TemplateContentArea tca in areas)
									{
										if (tca.Name == area.AreaName)
										{
											templateArea = tca;
											break;
										}
									}
								}
								if (templateArea != null)
								{
									area.StructuredElementId = templateArea.StructuredElementId;
								}

								if (area.StructuredElementId.GetValueOrDefault(0) > 0)
								{
									areaMarkup = XsltDocument.GetStructuredContent(area, content, _document.ID);
								}
								else
								{
									areaMarkup = area.Markup;
								}

								if (templateArea != null && !string.IsNullOrEmpty(templateArea.XsltVisibilityLeft) && !string.IsNullOrEmpty(templateArea.XsltVisibilityOperator) && !string.IsNullOrEmpty(templateArea.XsltVisibilityRight))
								{
									bool leftIsField = _fields.FieldNames.Contains(templateArea.XsltVisibilityLeft, StringComparer.OrdinalIgnoreCase);

									areaMarkup = XSLIfElement(areaMarkup,
										string.Format("'{0}{1}{0}'{2}'{3}'", leftIsField ? "##" : string.Empty, templateArea.XsltVisibilityLeft, templateArea.XsltVisibilityOperator, templateArea.XsltVisibilityRight));
								}

								if (
									area != null &&
									!string.IsNullOrWhiteSpace(area.XsltVisibilityLeft) &&
									!string.IsNullOrEmpty(area.XsltVisibilityOperator) &&
									!string.IsNullOrWhiteSpace(area.XsltVisibilityRight)
									)
								{
									bool leftIsField = _fields.FieldNames.Contains(area.XsltVisibilityLeft, StringComparer.OrdinalIgnoreCase);

									areaMarkup = XsltDocument.XSLIfElement(areaMarkup,
										string.Format("'{0}{1}{0}'{2}'{3}'", leftIsField ? "##" : string.Empty, area.XsltVisibilityLeft, area.XsltVisibilityOperator, area.XsltVisibilityRight));
								}

								areaMarkup = Regex.Replace(areaMarkup, @"<bscript.*?>.*?</bscript>", new MatchEvaluator(this.XmlEval));
								areaMarkup = Regex.Replace(areaMarkup, @"<textblock.*?>.*?</textblock>", new MatchEvaluator(this.XmlEval));
								areaMarkup = Regex.Replace(areaMarkup, @"<templatefield.*?>.*?</templatefield>", new MatchEvaluator(this.XmlEval));


								//check to see if there are any bScript expressions that need evaluation. If not, we can skip this section.
								bool hasNonFieldExpressions = false;

								MatchCollection mc = Regex.Matches(areaMarkup, @"\#\#(?<ExpressionOrField>.*?)\#\#");
								if (mc.Count > 0)
								{
									foreach (Match match in mc)
									{
										if (match.Groups["ExpressionOrField"].Success)
										{
											bool isField = false;
											foreach (Field f in _fields.Fields)
											{
												if (f.Name.Equals(match.Groups["ExpressionOrField"].Value, StringComparison.OrdinalIgnoreCase))
												{
													isField = true;
													break;
												}
											}
											if (!isField)
											{
												hasNonFieldExpressions = true;
												break;
											}
										}
									}
								}

								if (hasNonFieldExpressions)
								{
									areaMarkup = XsltDocument.ReplaceFields(new ContextObject(), areaMarkup);
								}

								xslt.Append(areaMarkup);
							}
						}
					}
				}
				if (templateContent.Length > nextpos)
				{
					//Finish writing from last ContentArea to end of Template
					xslt.Append(ConvertContentSample(templateContent.Substring(nextpos, templateContent.Length - nextpos)));
				}
			}
			else
			{
				xslt.Append(ConvertContentSample(templateContent));
			}

			string markup = xslt.ToString();
			markup = Regex.Replace(markup, @"\#\%\%\#(?<FieldName>.*?)\#\%\%\#", new MatchEvaluator(this.FieldPlaceholderRestore));

			markup = XSLStylesheet(markup, isPreview, isTextVersion);
			if (isPreview)
			{
				//todo: this appears to be evaluating out of order, or not as intended. The first evaluation is looking for "##" fields within xsl:if blocks, but it 
				//does not pick up our field in an xsl:if block. instead, it gets the second field, then the second line picks up our xsl:if field.
				markup = Regex.Replace(markup, @"(?<!\<xsl\:if test=""')\#\#(?<FieldName>[\w]*?)\#\#", new MatchEvaluator(this.FieldEval));
				markup = Regex.Replace(markup, @"'\#\#(?<FieldName>.*?)\#\#'", new MatchEvaluator(this.FieldEvalIfBlock));
				//markup = Regex.Replace(markup, @"<a.*?href=[""'](?<URL>.*?)[""'].*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval), RegexOptions.Singleline);
				//markup = Regex.Replace(markup, @"<a.*?<.*?>.*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval));
			}

			//without strongmail, we need to convert these links. copied from above "isPreview" block...
			markup = Regex.Replace(markup, @"<a.*?href=[""'](?<URL>.*?)[""'].*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval), RegexOptions.Singleline);
			markup = Regex.Replace(markup, @"<a.*?<.*?>.*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval));

			if (isTextVersion)
			{
				markup = Regex.Replace(markup, "<br[ ]{0,1}/>", "\r\n");
			}

			//finally, we need to replace html codes with allowable escape sequences:
			try
			{
				XsltEscapeSequenceMap map = new XsltEscapeSequenceMap();
				foreach (string token in map.GetTokens())
				{
					if (markup.Contains(token))
					{
						markup = markup.Replace(token, map.GetEscapeText(token));
					}
				}
			}
			catch (Exception)
			{
				//this should not fail completely if the escape sequences file is missing or could not be opened, 
				//however, there should at least be a warning presented to the user. The email xslt may still be valid and, 
				//if for any reason it is not, the validator will catch any errors and present them.
			}
			return markup;
		}


		protected string XmlEval(Match m)
		{
			string ret = string.Empty;
			XElement e = XElement.Parse(m.ToString());
			switch (e.Name.LocalName)
			{
				case "bscript":
					ret = "##" + e.AttributeValue("name") + "##";
					break;
				case "templatefield":
					ret = string.Format("#%%#{0}#%%#", e.AttributeValue("name"));
					break;
				case "textblock":
					string filter = e.AttributeValue(ContentArea.AttributeNames.VisibilityFilter);
					if (string.IsNullOrEmpty(filter) || EvaluateConditionalExpression(new ContextObject(), filter))
					{
						long blockId = -1;
						long.TryParse(e.AttributeValue("data-blockid"), out blockId);
						if (blockId > -1)
						{
							using (var content = LWDataServiceUtil.ContentServiceInstance())
							{
								TextBlock tb = content.GetTextBlock(blockId);
								if (tb == null)
								{
									throw new Exception(string.Format("Failed to load text block with id {0}. The text block does not exist.", blockId));
								}
								ret = tb.GetContent("en", "Web");
							}
						}
					}
					break;
			}
			return ret;
		}


		private string FieldEval(Match m)
		{
			return FieldEval(m, true);
		}


		private string FieldEvalIfBlock(Match m)
		{
			return FieldEval(m, false);
		}


		private string FieldEval(Match m, bool WrapValueOf)
		{
			string data = m.ToString();

			if (data.StartsWith("'##") && data.EndsWith("##'"))
			{
				data = data.Replace("'##", string.Empty).Replace("##'", string.Empty);
			}
			else
			{
				data = data.Replace("##", string.Empty);
			}

			string fieldName = m.Groups["FieldName"].ToString();
			string valueof = string.Empty;
			if (fieldName.Contains(@"\"))
			{
				//TODO: Redirection: ##\fieldname## resolves to field alias
				fieldName = fieldName.Replace(@"\", "");
				valueof = string.Format(@"##<xsl:value-of select=""{0}""/>##", fieldName);
			}
			else
			{
				if (WrapValueOf)
				{
					valueof = string.Format(@"<xsl:value-of select=""{0}""/>", data);
				}
				else
				{
					valueof = data;
				}
			}
			return valueof;
		}


		private string ConvertContentSample(string content)
		{
			content = Regex.Replace(content, @"<templatefield.*?>.*?</templatefield>", new MatchEvaluator(this.TemplateFieldEval));
			return content;
		}


		protected string TemplateFieldEval(Match m)
		{
			XDocument doc = XDocument.Parse(m.ToString());
			return string.Format("#%%#{0}#%%#", doc.Root.AttributeValue("name"));
		}


		/// <summary>
		/// Replaces hyperlink tags with xsl to allow field names to exist in the link href.
		/// </summary>
		/// <remarks>
		/// Field names are converted to <xsl:value-of> blocks when previewing emails, which fails xslt validation if they 
		/// exist inside an anchor tag's href. This evaluation method replaces anchor tags with xsl blocks to represent the 
		/// anchor while preserving the link.
		/// </remarks>
		/// <param name="m"></param>
		/// <returns></returns>
		public static string UrlPreviewEval(Match m)
		{
			string link = m.ToString();

			if (link.Contains("data-lninternalusage"))
			{
				return link;
			}

			MatchCollection valueOfMatches = Regex.Matches(m.ToString(), "<xsl:value-of select=.*?>");

			if (valueOfMatches.Count > 0)
			{
				foreach (Match match in valueOfMatches)
				{
					link = link.Replace(match.ToString(), "{value-of:" + match.Index.ToString() + "}");
				}

				//xsl blocks have been removed, we should be able to parse the actual link now...
				Match urlMatch = Regex.Match(link, @"<a.*?href=[""'](?<URL>.*?)[""'].*?>(?<text>.*?)</a>", RegexOptions.Singleline);

				string url = urlMatch.Groups["URL"].Value;
				string urlText = urlMatch.Groups["text"].Value;
				string urlStyle = Regex.Match(urlMatch.Value, @"style=[""'](?<style>.*?)[""']").Groups["style"].ToString();


				//now add value-of blocks back into the url:
				foreach (Match match in valueOfMatches)
				{
					url = url.Replace("{value-of:" + match.Index.ToString() + "}", match.ToString());
				}

				string clickUrl = @"<xsl:element name=""a""><xsl:attribute name=""href"">{0}</xsl:attribute><xsl:attribute name=""style"">{1}</xsl:attribute>{2}</xsl:element>";
				url = Regex.Replace(url, "\\&(?!amp;)", "&amp;");
				return string.Format(clickUrl, url, urlStyle, urlText);
			}
			else
			{
				Match urlMatch = Regex.Match(link, @"<a.*?href=[""'](?<URL>.*?)[""'].*?>(?<text>.*?)</a>", RegexOptions.Singleline);

				string url = urlMatch.Groups["URL"].Value;
				string urlText = urlMatch.Groups["text"].Value;
				string urlStyle = Regex.Match(urlMatch.Value, @"style=[""'](?<style>.*?)[""']").Groups["style"].ToString();

				string clickUrl = @"<xsl:element name=""a""><xsl:attribute name=""href"">{0}</xsl:attribute><xsl:attribute name=""style"">{1}</xsl:attribute>{2}</xsl:element>";
				url = Regex.Replace(url, "\\&(?!amp;)", "&amp;");
				return string.Format(clickUrl, url, urlStyle, urlText);
			}
		}


		/// <summary>
		/// Convert a template field to a placeholder value so that it is not evaluated as a bScript expression
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private string FieldPlaceholderSet(Match m)
		{
			string data = m.ToString();

			string fieldName = m.Groups["FieldName"].ToString();
			if (!string.IsNullOrEmpty(data))
			{
				foreach (Field field in _fields.Fields)
				{
					if (fieldName.Equals(field.Name, StringComparison.OrdinalIgnoreCase))
					{
						data = data.Replace("##", "#%%#");
					}
				}
			}
			return data;
		}


		/// <summary>
		/// Convert a template field from its placeholder value
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private string FieldPlaceholderRestore(Match m)
		{
			string data = m.ToString();

			string fieldName = m.Groups["FieldName"].ToString();
			if (!string.IsNullOrEmpty(data))
			{
				foreach (Field field in _fields.Fields)
				{
					if (fieldName.Equals(field.Name, StringComparison.OrdinalIgnoreCase))
					{
						data = data.Replace("#%%#", "##");
					}
				}
			}
			return data;
		}


		public static string XSLIfElement(string str, string condition)
		{
			string rstr = Environment.NewLine + @"<xsl:if test=""{0}"">{1}</xsl:if>" + Environment.NewLine;
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(rstr, condition, str);
			return sb.ToString();
		}


		public static string GetStructuredContent(ContentArea area, ContentService cms, long documentId, bool editable = false /*, Dictionary<BatchCacheIdentifier, DataTable> batchCache*/)
		{
			if (!area.StructuredElementId.HasValue)
			{
				throw new ArgumentException("Content area is not bound to a structureed type.");
			}

			string markup = string.Empty;
			ContextObject contextObject = new ContextObject();

			string result = string.Empty;
			if (EvaluateConditionalExpression(contextObject, area.VisibilityFilter))
			{
				Batch batch = GetBatchForArea(area, cms);

				if (batch == null)
				{
					throw new Exception("Content evaluation failed. Could not locate an appropriate batch.");
				}

				StructuredContentElement sce = cms.GetContentElement(area.StructuredElementId.Value);

				if (sce == null)
				{
					throw new Exception("Invalid element id " + area.StructuredElementId.Value.ToString());
				}

				List<StructuredContentAttribute> elementAttrs = cms.GetGlobalAttributes();
				elementAttrs.AddRange(cms.GetElementAttributes(sce.ID));
				List<StructuredContentData> globalDatums = cms.GetBatchGlobals(batch.ID);
				List<StructuredContentData> datums = cms.GetBatchElements(batch.ID, sce.ID);

				DataTable table = new DataTable();
				table.Columns.Add(new DataColumn("SequenceId", typeof(int)));
				Dictionary<long, string> attrID2Name = new Dictionary<long, string>();
				foreach (StructuredContentAttribute elementAttr in elementAttrs)
				{
					table.Columns.Add(elementAttr.Name);
					attrID2Name.Add(elementAttr.ID, elementAttr.Name);
				}

				if (datums != null && globalDatums != null && datums.Count > 0)
				{
					long lastSeqID = datums[0].SequenceID;
					DataRow row = table.NewRow();
					row["SequenceId"] = lastSeqID;
					foreach (StructuredContentData globalDatum in globalDatums)
					{
						if (globalDatum.SequenceID == lastSeqID)
						{
							row[attrID2Name[globalDatum.AttributeID]] = globalDatum.Data;
						}
					}
					foreach (StructuredContentData datum in datums)
					{
						if (datum.SequenceID != lastSeqID)
						{
							table.Rows.Add(row);
							lastSeqID = datum.SequenceID;
							row = table.NewRow();
							row["SequenceId"] = lastSeqID;
							foreach (StructuredContentData globalDatum in globalDatums)
							{
								if (globalDatum.SequenceID == lastSeqID)
								{
									row[attrID2Name[globalDatum.AttributeID]] = globalDatum.Data;
								}
							}
						}
						row[attrID2Name[datum.AttributeID]] = datum.Data;
					}
					table.Rows.Add(row);
				}

				if (table.Rows.Count > 0)
				{
					int startingRow = 0;
					int endingRow = 0;

					if (area.RowSelectionType == RowSelectionTypes.FirstRow)
					{
						startingRow = 0;
						endingRow = 0;
					}
					else if (area.RowSelectionType == RowSelectionTypes.IterateAll)
					{
						startingRow = 0;
						endingRow = table.Rows.Count;
					}
					else if (area.RowSelectionType == RowSelectionTypes.bScript)
					{
						Expression rowExpression = new ExpressionFactory().Create(area.RowSelectionExpression);
						object evaluatedRow = rowExpression.evaluate(contextObject);
						if (evaluatedRow != null)
						{
							if (int.TryParse(evaluatedRow.ToString(), out startingRow))
							{
								endingRow = startingRow;
							}
							else
							{
								throw new Exception("Content evaluation failed because a the row selection expression failed to return a valid number.");
							}
						}
					}

					if (endingRow == startingRow)
					{
						endingRow++;
					}

					//todo: should be able to just use the filters included with the ContentArea, rather than going to database:
					Dictionary<string, ActiveFilter> activeFilters = new Dictionary<string, ActiveFilter>();
					List<StructuredContentAttribute> attributes = cms.GetGlobalAttributes();
					attributes.AddRange(cms.GetElementAttributes(sce.ID));
					foreach (string key in area.Filters.Keys)
					{
						foreach (StructuredContentAttribute attribute in attributes)
						{
							if (attribute.Name == key)
							{
								//listField = the field selected for the area filter
								string listField = area.Filters[key];
								//if no field was selected for the area filter, then listField = the default filter field
								if (string.IsNullOrEmpty(listField))
								{
									listField = attribute.ListField;
								}
								activeFilters.Add(attribute.Name, new ActiveFilter(listField));
								break;
							}
						}
					}

					if (activeFilters.Count > 0)
					{
						string sortExpression = string.Empty;
						foreach (string s in activeFilters.Keys)
						{
							if (!string.IsNullOrEmpty(sortExpression))
							{
								sortExpression += ", ";
							}
							sortExpression += s;
						}

						DataRow[] rows = table.Select(string.Empty, sortExpression);

						int currentRowIndex = 0;
						int currentFilterLevel = 0;
						int rowIndex = 0;
						foreach (DataRow row in rows)
						{
							string[] keys = new string[activeFilters.Count];
							activeFilters.Keys.CopyTo(keys, 0);
							bool changed = false; //has a filtered value changed
							bool xslChanged = false; //same as above, used for first loop to write "</xsl:if>" blocks

							for (int i = 0; i < keys.Length; i++)
							{
								string s = keys[i];
								//the "&& string.IsNullOrEmpty(activeFilters[s].CurrentValue)" causes the system to think it is on the same row when the row filter has 
								//changed, leaving an opening <xsl:if with not closing tag. Not sure what the intended result was, but removing for now
								if (rowIndex != 0 && (xslChanged || (row[s].ToString() != activeFilters[s].CurrentValue /* && !string.IsNullOrEmpty(activeFilters[s].CurrentValue)*/)))
								{
									markup += "</xsl:if>";
									xslChanged = true;
								}
							}

							for (int i = 0; i < keys.Length; i++)
							{
								string s = keys[i];

								if (changed || row[s].ToString() != activeFilters[s].CurrentValue)
								{
									changed = true;
									//filter column has changed, generate new xsl statement
									string listColumn = string.Empty;
									foreach (StructuredContentAttribute attribute in elementAttrs)
									{
										if (attribute.Name == s)
										{
											if (activeFilters.ContainsKey(attribute.Name))
											{
												listColumn = activeFilters[attribute.Name].ListField; // attribute.ListField;
											}
											break;
										}
									}

									//markup += Environment.NewLine + @"<xsl:if test=""'#%%#" + listColumn + @"#%%#'='" + row[s].ToString() + @"'"">" + Environment.NewLine;
									markup += Environment.NewLine + @"<xsl:if test=""&quot;#%%#" + listColumn + @"#%%#&quot;=&quot;" + row[s].ToString() + @"&quot;"">" + Environment.NewLine;

									activeFilters[s].CurrentValue = row[s].ToString();

									currentFilterLevel = i;
								}
								if (changed)
								{
									currentRowIndex = 0;
								}
							}

							//format markup for the row
							if (currentRowIndex >= startingRow && currentRowIndex < endingRow)
							{
								string rowMarkup = area.Markup;
								MatchCollection matches = Regex.Matches(rowMarkup,
									/*formerly ContentEditorControl.AttributeStart and End*/  "\\[\\[\\[(?<AttributeName>.*?)\\]\\]\\]"
									);
								foreach (Match m in matches)
								{
									if (m.Groups["AttributeName"].Success)
									{
										string attributeValue = string.Empty;
										if (table.Columns.Contains(m.Groups["AttributeName"].Value))
										{
											attributeValue = row[m.Groups["AttributeName"].Value].ToString();
										}
										if (editable)
										{
											rowMarkup = rowMarkup.Replace(m.Value, WrapEditableContent(area.StructuredElementId.Value, batch.ID, sce.Name, batch.Name, (int)row["SequenceId"], m.Groups["AttributeName"].Value, attributeValue));
										}
										else
										{
											rowMarkup = rowMarkup.Replace(m.Value, attributeValue);
										}
									}
								}
								markup += rowMarkup;
							}
							currentRowIndex++;
							rowIndex++;
						}
						while (currentFilterLevel-- >= 0)
						{
							markup += "</xsl:if>";
						}
					}
					else
					{
						for (int i = startingRow; i < endingRow; i++)
						{
							string rowMarkup = area.Markup;
							MatchCollection matches = Regex.Matches(rowMarkup,
								ContentArea.ClientAttributes.ContentAttributeStart.Replace("[", "\\[") + @"(?<AttributeName>.*?)" + ContentArea.ClientAttributes.ContentAttributeEnd.Replace("]", "\\]")
								);
							foreach (Match m in matches)
							{
								if (m.Groups["AttributeName"].Success)
								{
									string attributeValue = string.Empty;
									if (table.Columns.Contains(m.Groups["AttributeName"].Value))
									{
										attributeValue = table.Rows[i][m.Groups["AttributeName"].Value].ToString();
									}
									if (editable)
									{
										rowMarkup = rowMarkup.Replace(m.Value, WrapEditableContent(area.StructuredElementId.Value, batch.ID, sce.Name, batch.Name, (int)table.Rows[i]["SequenceId"], m.Groups["AttributeName"].Value, attributeValue));
									}
									else
									{
										rowMarkup = rowMarkup.Replace(m.Value, attributeValue);
									}
								}
							}
							markup += rowMarkup;
						}
					}

				}
			}

			//should we be calling this? DocumentEditor calls it, so why should we?
	//		markup = ReplaceFields(contextObject, markup);
			return markup;
		}


		public class InvalidContent
		{
			public long StructuredElementId { get; set; }
			public long BatchId { get; set; }
			public int RowId { get; set; }

			public string ElementName { get; set; }
			public string BatchName { get; set; }
			public string AttributeName { get; set; }
			public string AttributeValue { get; set; }
			public string Error { get; set; }

			public InvalidContent(long structuredElementId, long batchId, int rowIndex, string elementName, string batchName, string attributeName, string attributeValue, string error)
			{
				StructuredElementId = structuredElementId;
				BatchId = batchId;
				RowId = rowIndex;
				ElementName = elementName;
				BatchName = batchName;
				AttributeName = attributeName;
				AttributeValue = attributeValue;
				Error = error;
			}
		}

		/// <summary>
		/// Performs XSLT validation against structured content
		/// </summary>
		/// <remarks>
		/// This function extracts each block of content in use for the content area and checks to see if it is valid xml.
		/// </remarks>
		/// <param name="area"></param>
		/// <param name="cms"></param>
		/// <param name="documentId"></param>
		/// <param name="editable"></param>
		/// <returns></returns>
		public static List<InvalidContent> GetInvalidStructuredContent(ContentArea area, ContentService cms, long documentId)
		{
			List<InvalidContent> ret = new List<InvalidContent>();
			//Func<string, bool> isValidXml = delegate(string content) 
			//{
			//	try
			//	{
			//		XElement.Parse(string.Format("<html>{0}</html>", content));
			//	}
			//	catch
			//	{
			//		return false;
			//	}
			//	return true;
			//};

			//todo: much code is duplicated between this function and GetStructuredContent(). This should be refactored (e.g., GetBatchForArea() has already been created; need to cover the rest of the code).
			if (!area.StructuredElementId.HasValue)
			{
				throw new ArgumentException("Content area is not bound to a structureed type.");
			}

			string markup = string.Empty;
			ContextObject contextObject = new ContextObject();

			string result = string.Empty;

			Batch batch = GetBatchForArea(area, cms);
			if (batch == null)
			{
				throw new Exception("Content evaluation failed. Could not locate an appropriate batch.");
			}

			StructuredContentElement sce = cms.GetContentElement(area.StructuredElementId.Value);
			if (sce == null)
			{
				throw new Exception("Invalid element id " + area.StructuredElementId.Value.ToString());
			}


			List<StructuredContentAttribute> elementAttrs = cms.GetGlobalAttributes();
			elementAttrs.AddRange(cms.GetElementAttributes(sce.ID));
			List<StructuredContentData> globalDatums = cms.GetBatchGlobals(batch.ID);
			List<StructuredContentData> datums = cms.GetBatchElements(batch.ID, sce.ID);

			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("SequenceId", typeof(int)));
			Dictionary<long, string> attrID2Name = new Dictionary<long, string>();
			foreach (StructuredContentAttribute elementAttr in elementAttrs)
			{
				table.Columns.Add(elementAttr.Name);
				attrID2Name.Add(elementAttr.ID, elementAttr.Name);
			}

			if (datums != null && globalDatums != null && datums.Count > 0)
			{
				long lastSeqID = datums[0].SequenceID;
				DataRow row = table.NewRow();
				row["SequenceId"] = lastSeqID;
				foreach (StructuredContentData globalDatum in globalDatums)
				{
					if (globalDatum.SequenceID == lastSeqID)
					{
						row[attrID2Name[globalDatum.AttributeID]] = globalDatum.Data;
					}
				}
				foreach (StructuredContentData datum in datums)
				{
					if (datum.SequenceID != lastSeqID)
					{
						table.Rows.Add(row);
						lastSeqID = datum.SequenceID;
						row = table.NewRow();
						row["SequenceId"] = lastSeqID;
						foreach (StructuredContentData globalDatum in globalDatums)
						{
							if (globalDatum.SequenceID == lastSeqID)
							{
								row[attrID2Name[globalDatum.AttributeID]] = globalDatum.Data;
							}
						}
					}
					row[attrID2Name[datum.AttributeID]] = datum.Data;
				}
				table.Rows.Add(row);
			}

			if (table.Rows.Count > 0)
			{
				int startingRow = 0;
				int endingRow = 0;

				if (area.RowSelectionType == RowSelectionTypes.FirstRow)
				{
					startingRow = 0;
					endingRow = 0;
				}
				else if (area.RowSelectionType == RowSelectionTypes.IterateAll)
				{
					startingRow = 0;
					endingRow = table.Rows.Count;
				}
				else if (area.RowSelectionType == RowSelectionTypes.bScript)
				{
					Expression rowExpression = new ExpressionFactory().Create(area.RowSelectionExpression);
					object evaluatedRow = rowExpression.evaluate(contextObject);
					if (evaluatedRow != null)
					{
						if (int.TryParse(evaluatedRow.ToString(), out startingRow))
						{
							endingRow = startingRow;
						}
						else
						{
							throw new Exception("Content evaluation failed because a the row selection expression failed to return a valid number.");
						}
					}
				}

				if (endingRow == startingRow)
				{
					endingRow++;
				}

				//todo: should be able to just use the filters included with the ContentArea, rather than going to database:
				Dictionary<string, string> activeFilters = new Dictionary<string, string>();
				List<StructuredContentAttribute> attributes = cms.GetGlobalAttributes();
				attributes.AddRange(cms.GetElementAttributes(sce.ID));
				foreach (string key in area.Filters.Keys)
				{
					foreach (StructuredContentAttribute attribute in attributes)
					{
						if (attribute.Name == key)
						{
							string listField = area.Filters[key];
							if (string.IsNullOrEmpty(listField))
							{
								listField = attribute.ListField;
							}
							activeFilters.Add(attribute.Name, listField);
							break;
						}
					}
				}



				if (activeFilters.Count > 0)
				{
					string sortExpression = string.Empty;
					foreach (string s in activeFilters.Keys)
					{
						if (!string.IsNullOrEmpty(sortExpression))
						{
							sortExpression += ", ";
						}
						sortExpression += s;
					}

					DataRow[] rows = table.Select(string.Empty, sortExpression);

					int currentRowIndex = 0;
					int rowIndex = 0;
					foreach (DataRow row in rows) //these are the chosen rows for our content area. check each "key" (filter) and each column referenced in markup and validate.
					{
						string[] keys = new string[activeFilters.Count];
						activeFilters.Keys.CopyTo(keys, 0);

						for (int i = 0; i < keys.Length; i++)
						{
							string s = keys[i];
							string errors = string.Empty;
							if (!Validate(s, ref errors, true))
							{
								ret.Add(new InvalidContent(area.StructuredElementId.Value, batch.ID, (int)row["SequenceId"], sce.Name, batch.Name, activeFilters[keys[i]], s, errors));
							}
						}

						//format markup for the row
						if (currentRowIndex >= startingRow && currentRowIndex < endingRow)
						{
							string rowMarkup = area.Markup;
							MatchCollection matches = Regex.Matches(rowMarkup, "\\[\\[\\[(?<AttributeName>.*?)\\]\\]\\]");
							foreach (Match m in matches)
							{
								if (m.Groups["AttributeName"].Success)
								{
									string attributeValue = string.Empty;
									if (table.Columns.Contains(m.Groups["AttributeName"].Value))
									{
										attributeValue = row[m.Groups["AttributeName"].Value].ToString();
										string errors = string.Empty;
										if (!Validate(attributeValue, ref errors, true))
										{
											ret.Add(new InvalidContent(area.StructuredElementId.Value, batch.ID, (int)row["SequenceId"], sce.Name, batch.Name, m.Groups["AttributeName"].Value, attributeValue, errors));
										}
									}
								}
							}
							markup += rowMarkup;
						}
						currentRowIndex++;
						rowIndex++;
					}
				}
				else
				{
					MatchCollection matches = Regex.Matches(area.Markup, "\\[\\[\\[(?<AttributeName>.*?)\\]\\]\\]");

					for (int i = startingRow; i < endingRow; i++)
					{
						DataRow row = table.Rows[i];
						string rowMarkup = string.Empty;
						foreach (Match m in matches)
						{
							if (m.Groups["AttributeName"].Success)
							{
								//create an XSLT document using the single piece of content to see if it will compile. If not, add the content to the invalid content stack
								string attributeValue = string.Empty;
								if (table.Columns.Contains(m.Groups["AttributeName"].Value))
								{

									attributeValue = row[m.Groups["AttributeName"].Value].ToString();
									if (!string.IsNullOrWhiteSpace(attributeValue))
									{
										attributeValue = Regex.Replace(attributeValue, @"<a.*?href=[""'](?<URL>.*?)[""'].*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval), RegexOptions.Singleline);
										attributeValue = Regex.Replace(attributeValue, @"<a.*?href=[""'](?<URL>.*?)[""'].*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval), RegexOptions.Singleline);
										attributeValue = Regex.Replace(attributeValue, @"<a.*?<.*?>.*?>(?<text>.*?)</a>", new MatchEvaluator(XsltDocument.UrlPreviewEval));

										//finally, we need to replace html codes with allowable escape sequences:
										try
										{
											XsltEscapeSequenceMap map = new XsltEscapeSequenceMap();
											foreach (string token in map.GetTokens())
											{
												if (attributeValue.Contains(token))
												{
													attributeValue = attributeValue.Replace(token, map.GetEscapeText(token));
												}
											}
										}
										catch (Exception)
										{
											//this should not fail completely if the escape sequences file is missing or could not be opened, 
											//however, there should at least be a warning presented to the user. The email xslt may still be valid and, 
											//if for any reason it is not, the validator will catch any errors and present them.
										}

										//string xslt = XSLStylesheet(attributeValue, true, area.AreaType == ContentArea.ContentAreaTypes.Text);

										string errors = string.Empty;
										if (!Validate(attributeValue, ref errors, true))
										{
											ret.Add(new InvalidContent(area.StructuredElementId.Value, batch.ID, (int)row["SequenceId"], sce.Name, batch.Name, m.Groups["AttributeName"].Value, attributeValue, errors));
										}
									}
								}
							}
						}
					}
				}
			}
			return ret;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="emailHtml"></param>
		/// <param name="emailText"></param>
		/// <param name="xsltErrors"></param>
		/// <returns></returns>
		public static bool Validate(string body, ref string xsltErrors, bool applyXslt = false, bool includeBodyInErrors = false)
		{
			XslCompiledTransform xslt = new XslCompiledTransform();

			if (string.IsNullOrWhiteSpace(body))
			{
				return true;
			}

			if (applyXslt)
			{
				body = XSLStylesheet(body, true, false);
			}

			try
			{
				StringReader sreader = new StringReader(body);
				XmlTextReader xreader = new XmlTextReader(sreader);
				xslt.Load(xreader);
			}
			catch (XsltException ex)
			{
				if (ex.InnerException != null)
				{
					xsltErrors += ex.InnerException.Message;
					if (includeBodyInErrors)
					{
						if (System.Web.HttpContext.Current != null)
						{
							xsltErrors += "XSLTBODY:" + System.Web.HttpContext.Current.Server.HtmlEncode(body);
						}
						else
						{
							xsltErrors += "XSLTBODY:" + body;
						}
					}
				}
				return false;
			}
			return true;
		}


		private static string WrapEditableContent(long structuredElementId, long batchId, string elementName, string batchName, int rowId, string attributeName, string attributeValue)
		{
			const string editBlock =
			@"<div class=""EditableContent"" data-structuredtypeid=""{0}"" data-batchid=""{1}"" data-rowid=""{2}"" data-attributename=""{3}"">
				<div class=""ContentEditControls"" style=""display:none;"">
					<a class=""button"" href=""#"" data-lninternalusage=""true"">Save</a>
					<a class=""button"" href=""#"" data-lninternalusage=""true"">Cancel</a>
					<span>You are editing: {4} <span class=""AppPanelSeparator"">&nbsp;&nbsp;</span> {3}, Row {2}, Batch ""{5}""</span>
				</div>
				<textarea class=""ContentEditor""></textarea>
				<input type=""hidden"" />
				<div class=""Content"">{6}</div>
			</div>";

			return string.Format(editBlock, structuredElementId, batchId, rowId, attributeName, elementName, batchName, attributeValue);
		}

		private static bool EvaluateConditionalExpression(ContextObject contextObject, string conditionalExpression)
		{
			if (string.IsNullOrEmpty(conditionalExpression))
			{
				return true;
			}

			Expression e = new ExpressionFactory().Create(conditionalExpression);
			object obj = e.evaluate(contextObject);
			if (obj == null || !(obj is bool))
			{
				throw new Exception("Error evaluating rule: " + conditionalExpression);
			}
			return (bool)obj;
		}


		private static string XSLStylesheet(string content, bool isPreview, bool isTextVersion)
		{
			string ret = @"<?xml version=""1.0"" encoding=""UTF-16""?>" + Environment.NewLine +
				@"<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns=""http://www.w3.org/TR/REC-html40"">" + Environment.NewLine +
				@"<xsl:output method=""" + (isTextVersion ? "text" : "xml") + "" + @"""/>" + Environment.NewLine +
				@"<xsl:template match=""recipient"">" + Environment.NewLine;

			if (content.StartsWith("<!DOCTYPE") && content.Contains(">"))
			{
				ret += string.Format("<xsl:text disable-output-escaping=\"yes\">{0}</xsl:text>", HttpUtility.HtmlEncode(content.Substring(0, content.IndexOf(">") + 1)));
				content = content.Substring(content.IndexOf(">") + 1);
			}

			//If this is the text version and not preview mode, or if an "<html..." tag exists, we don't wrap in html and body tags. 
			if ((isTextVersion && !isPreview) || content.StartsWith("<html"))
			{
				ret += "{0}" + Environment.NewLine;
			}
			else
			{

				ret += @"<html>" + Environment.NewLine +
				@"<body>" + Environment.NewLine + "{0}" + Environment.NewLine +
				@"</body>" + Environment.NewLine +
				@"</html>" + Environment.NewLine;
			}
			ret += @"</xsl:template>" + Environment.NewLine + @"</xsl:stylesheet>";
			return string.Format(ret, content);
		}


		private static Batch GetBatchForArea(ContentArea area, ContentService cms)
		{
			Batch batch = null;
			ContextObject contextObject = new ContextObject();

			if (area.BatchSelectionType == BatchSelectionTypes.bScript)
			{
				Expression batchExpression = new ExpressionFactory().Create(area.BatchSelectionExpression);
				object evaluatedBatch = batchExpression.evaluate(contextObject);
				if (evaluatedBatch != null)
				{
					batch = cms.GetBatch(evaluatedBatch.ToString());
				}
			}
			else if (area.BatchSelectionType == BatchSelectionTypes.ActiveBatch)
			{
				List<Batch> batches = cms.GetActiveBatches();
				if (batches.Count > 0)
				{
					batch = batches[0];
				}
				else
				{
					throw new Exception("Content evaluation failed because an active batch could not be located.");
				}
			}
			else if (area.BatchSelectionType == BatchSelectionTypes.BatchName)
			{
				if (!string.IsNullOrEmpty(area.BatchSelectionExpression))
				{
					batch = cms.GetBatch(area.BatchSelectionExpression);
				}
				else
				{
					throw new Exception("Batch cannot be retrieved because no batch name was specified.");
				}
			}

			return batch;
		}

		public static string ReplaceFields(ContextObject contextObject, string content)
		{
			ContentEvaluator evaluator = new ContentEvaluator(content, contextObject);
			string result = evaluator.Evaluate();
			return result;
		}
	}
}

