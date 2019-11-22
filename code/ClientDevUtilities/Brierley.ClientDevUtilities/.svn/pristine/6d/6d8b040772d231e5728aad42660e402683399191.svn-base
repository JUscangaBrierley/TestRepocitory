using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

using Brierley.FrameWork.Rules.UIDesign;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.bScript;

namespace Brierley.FrameWork.Rules
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class UpdateLastActivityDate : RuleBase
	{
		#region Results
		public class UpdateLastActivityRuleResult : ContextObject.RuleResult
		{
			public DateTime? NewActivityDate;
		}
		#endregion

		#region Private Variables
		[NonSerialized]
		private const string _className = "UpdateLastActivityDate";

		[NonSerialized]
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private string _activityDate = "Date()";

		#endregion

		/// <summary>
		/// 
		/// </summary>
        public UpdateLastActivityDate()
            : base("UpdateLastActivityDate")
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Validate()
		{			
		}

		#region Properties

        public override string DisplayText
        {
            get { return "Update Last Activity"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(Namespace = "http://www.brierley.com")]
        [Browsable(true)]
        [CategoryAttribute("General")]
        [DisplayName("Date to use for last activity date")]
        [Description("The last activity date of the member will be set to this date. This value is an expression and it defaults to the current date 'Date()'.")]
        [RuleProperty(true, false, false, null)]
        [RulePropertyOrder(1)]
        public string ActivityDate
        {
            get
            {
                return _activityDate;
            }
            set
            {
                _activityDate = value;
            }
        }        
		#endregion

		#region Helper Methods		
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Context"></param>
		/// <param name="PreviousResultCode"></param>
		/// <returns></returns>
		public override void Invoke(ContextObject Context)
		{
			string methodName = "Invoke";

			Member lwmember = null;
			VirtualCard lwvirtualCard = null;
            						
			ResolveOwners(Context.Owner, ref lwmember, ref lwvirtualCard);

			if (lwmember == null)
			{
				_logger.Error(_className, methodName, "Unable to resolve a member.");
				throw new System.Exception("Unable to resolve a member.");
			}

            ExpressionFactory exprF = new ExpressionFactory();
            lwmember.LastActivityDate = (DateTime)exprF.Create(this.ActivityDate).evaluate(Context);

			var result = new UpdateLastActivityRuleResult() 
			{ 
				NewActivityDate = lwmember.LastActivityDate, 
				Name = this.RuleName, 
                RuleType = this.GetType(),
                MemberId = lwmember.IpCode
			};
            AddRuleResult(Context, result);

			_logger.Debug(_className, methodName, "Finished invoking award point rule for member " + lwmember.IpCode);
			return;			
		}

        #region Migrartion Helpers

        public override List<string> GetBscriptsToMove()
        {
            List<string> bscriptList = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.ActivityDate) && ExpressionUtil.IsLibraryExpression(this.ActivityDate))
            {
                bscriptList.Add(ExpressionUtil.GetLibraryName(this.ActivityDate));
            }
            return bscriptList;
        }

		public override RuleBase MigrateRuleInstance(RuleBase source, ServiceConfig sourceConfig, ServiceConfig targetConfig)
        {
            string methodName = "MigrateRuleInstance";

            _logger.Trace(_className, methodName, "Migrating UpdateLastActivityDate rule.");

            UpdateLastActivityDate src = (UpdateLastActivityDate)source;

            ActivityDate = src.ActivityDate;

            RuleVersion = src.RuleVersion;
            RuleDescription = src.RuleDescription;

            return this;
        }
        #endregion
	}
}
