//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using System.Xml.Linq;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;

namespace Brierley.FrameWork.Interfaces
{
	/// <summary>
	/// This is an interceptor interface for the AddAttributeSet API call.
	/// </summary>
    public interface IAddAttributeSetInterceptor : ILWInterceptor
	{		
		/// <summary>
		/// This intercept allows for each XML element that is parsed out of the arguments to be validated before attempting to add the container or if we need to update anything on the input.
		/// </summary>
		/// <param name="config">This is the configuration.</param>
		/// <param name="member">This is the member.</param>
		/// <param name="attributeNode">This is the attributeNode currently being worked on.</param>
		/// <returns></returns>
		XElement ProcessRawXmlElement(LWIntegrationConfig config, Member member, XElement attributeNode);

        /// <summary>
        /// Before we add the processed objects to the member, virtualcard, or parent, we may want to do some validation.
        /// </summary>
        /// <param name="config">This is the configuration.</param>
        /// <param name="member">This is the member.</param>
        /// <param name="attributeObject">This is the IClientDataObject that we are about to add to the member, virtualcard, or parent.</param>
        /// <param name="objectMetadata">This is the metadata about the IClientDataObject that we are about to add to the member, virtualcard, or parent.</param>
        /// <returns></returns>
        IClientDataObject ProcessObjectBeforeAdd(LWIntegrationConfig config, Member member, IClientDataObject attributeObject, AttributeSetMetaData objectMetadata);


		/// <summary>
		/// Before saving the member and all attribute sets, we may want to manipulate something.
		/// </summary>
		/// <param name="config">This is the configuration.</param>
		/// <param name="member">This is the member.</param>
		/// <returns></returns>
		Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member);

        /// <summary>
        /// After saving the member and all attribute sets, we may want to manipulate something.
        /// </summary>
        /// <param name="config">This is the configuration.</param>
        /// <param name="member">This is the member.</param>
        /// <returns></returns>
        Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, List<ContextObject.RuleResult> results = null);
	}
}
