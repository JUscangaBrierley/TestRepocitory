using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class RealTimebScriptQuery : Query
	{
		public string Expression { get; set; }

		public override void EnsureSchema()
		{
			return;
		}

		public override List<SqlStatement> GetSqlStatement(Dictionary<string, string> overrideParameters = null)
		{
			return null;
		}

		public override System.Data.DataTable GetDataSample(string[] groupBy)
		{
			return null;
		}

		public override List<SqlStatement> GetVerifySqlStatement()
		{
			return null;
		}

		public override bool Validate(List<ValidationMessage> Warnings, bool ValidateSql)
		{
			if (Expression == null)
			{
				Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Expression is required for step {0}", Step.UIName)));
				return false;
			}
			try
			{
				new Brierley.FrameWork.bScript.ExpressionFactory().Create(Expression);
			}
			catch (Exception ex)
			{
				Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Error parsing bScript expression for step {0}: {1}", Step.UIName, ex.Message)));
				return false;
			}
			return true;
		}


		internal override List<CampaignResult> Execute(FrameWork.ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			//skipping validation on execution due to performance - validate calls ExpressionFactory.Create, same as here. It takes
			//too long to call the same method twice to execute a step. If expression caching is implemented, then we can add this back 
			//in. Otherwise, it's not suitable for real-time processing.
			/*
			List<ValidationMessage> warnings = new List<ValidationMessage>();
			if (!Validate(warnings, true))
			{
				string exception = "Failed to execute " + Step.UIName + " because the step is invalid.";
				foreach (ValidationMessage message in warnings)
				{
					exception += message.Message;
				}
				throw new Exception(exception);
			}
			*/

			Expression exp = new ExpressionFactory().Create(Expression);
			object result = exp.evaluate(co);
			if (result != null)
			{
				bool success = true;
				if (bool.TryParse(result.ToString(), out success))
				{
					return new List<CampaignResult>() { new CampaignResult(success ? 1 : 0) };
				}
			}

			return new List<CampaignResult>() { new CampaignResult(1) };
		}


	}
}