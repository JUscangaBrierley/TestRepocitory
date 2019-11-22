using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Interfaces
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class XsltProcessorAttribute : System.Attribute
	{
		/// <summary>
		/// Processor's display name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description of the processor
		/// </summary>
		public string Description { get; set; }

		public XsltProcessorAttribute()
		{
		}
	}


	/// <summary>
	/// Interface used for post-processing XSLT documents. Classes implementing this interface are able to make final 
	/// modifications to email documents before they are transferred to the email server.
	/// </summary>
	public interface IXsltPostProcessor
	{
		/// <summary>
		/// Processes the XSLT body.
		/// </summary>
		/// <param name="xslt">the XSLT body</param>
		/// <returns>The modified XSLT body</returns>
		string Process(string xslt, bool isTextVersion);
	}
}
