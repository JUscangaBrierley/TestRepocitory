using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The GetContentAttributeDataValueSet function returns an XSLT block of if statements
    /// that have condition on all the possible values of a particular field.  The function first retrieves all
    /// the values from the template for the named field.  Then it iterates over the dataset and for each value
    /// of the value set, it generates an if block.
    /// </summary>
    /// <example>
    ///     Usage : GetContentAttributeDataValueSet('AttributeName','TestAttributeName','ListFieldName',Repeat)
    /// </example>
    /// <remarks>
    /// AttributeName must be the name of a valid attribute in the specified structured element.
    /// TestAttributeName is the name of an attribute in the data set whose value is to be tested.
    /// ListFieldName must be the name of a template field.
    /// Repeat - If set to true then all will be repeated.  Otherwise only the first value.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns an XSLT block of if statements that have condition on all the possible values of a particular field. The function first retrieves all the values from the template for the named field and iterates over the dataset and generates an if block for each value of the set.", 
		DisplayName = "GetContentAttributeDataValueSet", 
		ExcludeContext = ExpressionContexts.Member | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = ExpressionApplications.Strings)]
	public class GetContentAttributeDataValueSet : UnaryOperation
    {
        private Expression AttributeName = null;
        private Expression TestAttributeName = null;
        private Expression ListFieldName = null;
        bool Repeat = false;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetContentAttributeDataValueSet()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public GetContentAttributeDataValueSet(Expression rhs)
            : base("GetContentAttributeDataValueSet", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs >= 3 )
            {
                AttributeName = ((ParameterList)rhs).Expressions[0];
                TestAttributeName = ((ParameterList)rhs).Expressions[1];
                ListFieldName = ((ParameterList)rhs).Expressions[2];
                if (numArgs == 4)
                {
                    Repeat = bool.Parse(((ParameterList)rhs).Expressions[3].ToString());
                }
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetContentAttributeDataValueSet.");
        }

        #region Private Methods

		//private DataRow[] GetTemplateFieldValues(TemplateDocument template)
		//{
		//    TemplateFieldCollection fields = new TemplateFieldCollection(template.FieldsXML);
		//    return fields.FieldValues(ListFieldName);
		//}

		private IList<FieldValue> GetTemplateFieldValues(Template template, ContextObject contextObject)
		{
			FieldCollection fields = new FieldCollection(template.Fields);
			Field field = fields.GetFieldByName(ListFieldName.evaluate(contextObject).ToString());
			return field.GetValues();
			//return fields.FieldValues(ListFieldName);
		}

        private string GetEncodedBlock(Template template, /*List<NameValueCollection>*/ StructuredDataRows rows, ContextObject contextObject)
        {
            StringBuilder ifBlock = new StringBuilder();

            Dictionary<string, string> map = new Dictionary<string, string>();

			//DataRow[] fieldValueRows = GetTemplateFieldValues(template);
			IList<FieldValue> fieldValueRows = GetTemplateFieldValues(template, contextObject);
			//if (fieldValueRows != null && fieldValueRows.Length > 0)
			//{
				//foreach (DataRow fieldValueRow in fieldValueRows)
			foreach (FieldValue fieldValueRow in fieldValueRows)
                {
					//if (ListFieldName == (string)fieldValueRow[1])
					//{
						//string listvalue = (string)fieldValueRow[2];
					string listvalue = fieldValueRow.Value.ToString();
                        /*foreach (NameValueCollection row in rows)*/
						for(int i = 0; i < rows.Count; i++)
                        {
							DataRow row = rows[i];
                            string testval = row[TestAttributeName.evaluate(contextObject).ToString()].ToString();
                            if (listvalue == testval)
                            {
                                if (Repeat || !map.ContainsKey(testval))
                                {
                                    string attrVal = row[AttributeName.evaluate(contextObject).ToString()].ToString();
                                    string strIf = XSLIfElement(attrVal, string.Format("'#%%#{0}#%%#'='{1}'", ListFieldName.evaluate(contextObject).ToString(), testval));
                                    ifBlock.Append(strIf);
                                    map.Add(testval, testval);
                                }
                            }
                        }
					//}
                }
			//}
            return ifBlock.ToString();
        }     
        
        private static string XSLIfElement(string str, string condition)
        {
            string rstr = Environment.NewLine + @"<xsl:if test=""{0}"">{1}</xsl:if>" + Environment.NewLine;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(rstr, condition, str);
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetContentAttributeDataValueSet('AttributeName','TestAttributeName','ListFieldName',Repeat)";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            if (contextObject.Environment == null) return null;

			StructuredDataRows rows = null;
			if (contextObject.Environment.ContainsKey("StructuredDataRows"))
			{
				rows = contextObject.Environment["StructuredDataRows"] as StructuredDataRows;
			}
            if (rows == null || rows.Count < 1) return null;

            return GetEncodedBlock((Template)contextObject.Template, rows, contextObject);                        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string parseMetaData()
        {
            string meta = AttributeName.ToString();
            return meta;
        }
    }
}
