/*
* Created on 12-10-2003
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/
using System;
using System.IO;
using System.Text;

using Commons.Collections;

using NVelocity.Runtime.Resource;
using NVelocity.Runtime.Resource.Loader;

using StringInputStream = System.IO.StringReader;

namespace Brierley.FrameWork.CodeGen.ClientDataModel
{
	/// <author>  MAX
	/// 
	/// To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	public class StringResourceLoader : ResourceLoader
	{
		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#init(org.apache.commons.collections.ExtendedProperties)
		*/

		public override void Init(ExtendedProperties configuration)
		{
			// TODO Auto-generated method stub
		}

		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#getResourceStream(java.lang.String)
		*/

        public override Stream GetResourceStream(string source)
		{
			return new MemoryStream(Encoding.ASCII.GetBytes(source));
		}

		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#isSourceModified(org.apache.velocity.runtime.resource.Resource)
		*/

        public override bool IsSourceModified(Resource resource)
		{
			return false;
		}

		/* (non-Javadoc)
		* @see org.apache.velocity.runtime.resource.loader.ResourceLoader#getLastModified(org.apache.velocity.runtime.resource.Resource)
		*/

        public override long GetLastModified(Resource resource)
		{
			return (DateTime.Now.Ticks - 621355968000000000) / 10000;
		}
	}
}