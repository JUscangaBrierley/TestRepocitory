using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;


namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The ResolveAttributeSet function returns the the attribute set using the provided criteria.
    /// </summary>
    /// <example>
    ///     Usage : ResolveAttributeSet('AttributeSetName','criteria',RowIndex)
    /// </example>
    /// <remarks>
    /// Attribute Set Name must be the name of a valid attribute set.
    /// criteria is an expression that can be applied to the objects in that attribute set.
    /// RowIndex must be either a number or a bScript expression that evaluates to a number.
    ///</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the the attribute set using the provided criteria.", 
		DisplayName = "ResolveAttributeSet", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member, 
		ExpressionReturns = ExpressionApplications.Objects,
		EvalRequiresMember = true)]
	public class ResolveAttributeSet : UnaryOperation
    {
        private const string _className = "ResolveAttributeSet";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private Expression AttributeSetName = null;
        private Expression Criteria = null;
        private Expression RowIndex = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public ResolveAttributeSet()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal ResolveAttributeSet(Expression rhs)
            : base("ResolveAttributeSet", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                AttributeSetName = ((ParameterList)rhs).Expressions[0];
                //Criteria = ((ParameterList)rhs).Expressions[1].evaluate(cObj).ToString();
                Criteria = ((ParameterList)rhs).Expressions[1];
                RowIndex = ((ParameterList)rhs).Expressions[2];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to ResolveAttributeSet.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "ResolveAttributeSet('AttributeSetName','Criteria',RowIndex)";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            string methodName = "evaluate";
            string msg = "";

            string attributeSetName = (string)AttributeSetName.evaluate(contextObject);

            IAttributeSetContainer resultObj = null;
			Member member = ResolveMember(contextObject.Owner);            
            if (member != null)
            {
                msg = string.Format("Resolving attribute set {0} using expression {0} {1}.", attributeSetName, Criteria.ToString());
                _logger.Debug(_className, methodName, msg);

				//object attrValue = string.Empty;
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					AttributeSetMetaData metadata = service.GetAttributeSetMetaData(attributeSetName);
					if (metadata != null)
					{
						int theIndex = System.Int32.Parse(this.RowIndex.evaluate(contextObject).ToString());
						IAttributeSetContainer thisowner = null;
						if (metadata.Type == AttributeSetType.Member)
						{
							thisowner = member;
						}
						else
						{
							thisowner = contextObject.Owner;
						}
						IList<IClientDataObject> selected = new List<IClientDataObject>();
						if (!thisowner.IsLoaded(attributeSetName))
						{
							service.LoadAttributeSetList(thisowner, attributeSetName, false);
						}
						IList<IClientDataObject> atsList = thisowner.GetChildAttributeSets(attributeSetName);
						if (atsList != null && atsList.Count > 0 && theIndex >= 0 && theIndex < atsList.Count)
						{
							foreach (IClientDataObject aset in atsList)
							{
								ContextObject ctx = new ContextObject();
								ctx.Owner = contextObject.Owner;
								ctx.InvokingRow = aset;
								bool result = (bool)Criteria.evaluate(ctx);
								if (result)
								{
									selected.Add(aset);
								}
							}
							if (selected.Count > 0 && theIndex < selected.Count)
							{
								resultObj = selected[theIndex];
							}
						}
					}
					return resultObj;
				}
            }
            else
            {
                throw new CRMException("AttrValue must be evaluated in the context of a loyalty member.");
            }
        }
        
    }
}
