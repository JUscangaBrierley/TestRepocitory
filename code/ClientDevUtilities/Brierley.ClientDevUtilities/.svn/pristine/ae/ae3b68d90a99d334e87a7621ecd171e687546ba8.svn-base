//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

using System.Xml.Linq;

using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;

namespace Brierley.FrameWork.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
    public interface IInboundMobileInterceptor : ILWInterceptor
	{		
		/// <summary>
		/// DAP configuration allows loading a member through an interceptor instead of builtin load
		/// methods.
		/// </summary>
		/// <param name="config">This is the configuration for the data acquisition processor.</param>
		/// <param name="memberNode">This is the member node that may have the information needed to load a member.</param>
		/// <returns></returns>
		Member LoadMember(LWIntegrationConfig config, Member template);

		/// <summary>
		/// This method is invoked after all attempts to load a member based on the specified load directives
		/// have failed and the HandleMemberNotFoundInInterceptor attribute on the member load directive.
		/// Typically, this is used to log non-member transactions in the TxnHistory attribute set.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="memberNode"></param>
        void HandleMemberNotFound(LWIntegrationConfig config, Member template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="member"></param>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        object ProcessMemberBeforePopulation(LWIntegrationConfig config, object rawMember);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="repository"></param>
		/// <param name="member"></param>
		/// <param name="memberNode"></param>
		/// <returns></returns>
		Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="repository"></param>
		/// <param name="member"></param>
		/// <param name="memberNode"></param>
		/// <returns></returns>
        Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, IList<ContextObject.RuleResult> results = null);	
	}
}
