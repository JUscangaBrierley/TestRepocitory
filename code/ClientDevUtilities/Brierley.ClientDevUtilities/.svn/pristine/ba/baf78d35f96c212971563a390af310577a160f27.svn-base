//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	[ExpressionContext(Description = "Returns string containing an html 'img' tag for the provided image in the image store.",
		DisplayName = "GetImageUrl",
		ExcludeContext = ExpressionContexts.Member,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Content,
		ExpressionReturns = ExpressionApplications.Strings)]
	public class GetImageUrl : UnaryOperation
	{
		private string _imageName = string.Empty;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public GetImageUrl()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public GetImageUrl(Expression rhs)
			: base("GetImageUrl", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetImageUrl('ImageName')";
			}
		}

		private string MakeImageUrl()
		{
			string lwContentRootURL = ConfigurationManager.AppSettings["LWContentRootURL"];
			if (!string.IsNullOrEmpty(lwContentRootURL))
			{
				LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
				if (ctx != null)
				{
					if (!lwContentRootURL.EndsWith("/"))
					{
						lwContentRootURL += "/";
					}
					string imageUrl = string.Format("{0}{1}/{2}", lwContentRootURL, ctx.Organization, _imageName);
					return imageUrl;
				}
				else
				{
					throw new CRMException("Current environment context not defined.");
				}
			}
			else
			{
				throw new CRMException("LWContentRootURL property not defined.");
			}
		}

		/// <summary>
		/// Performs the operation defined by this function
		/// </summary>
		/// <param name="contextObject">An instance of ContextObject</param>
		/// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
			_imageName = GetRight().evaluate(contextObject).ToString();

			string result = MakeImageUrl();
			return result;
		}
	}
}
