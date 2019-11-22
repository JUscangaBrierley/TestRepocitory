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
	public interface ILWInterceptor : IDisposable
	{
		/// <summary>
		/// Initialize with any parameters
		/// </summary>
		/// <param name="parameters"></param>
		void Initialize(NameValueCollection parameters);		
	}
}
