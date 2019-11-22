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
	public interface IMemberResponseHelper
	{
		/// <summary>
		/// Initialize with any parameters
		/// </summary>
		/// <param name="parameters"></param>
		void Initialize(NameValueCollection parameters);

		/// <summary>
		/// This operation is invoked after attribuute sets are loaded into the member.
		/// </summary>
		/// <param name="directive"></param>
		/// <param name="member"></param>
		Member ProcessMemberAfterAttributeSetLoad(string opName, LWIntegrationConfig.OperationResponseDirective directive, Member member);
	}
}
