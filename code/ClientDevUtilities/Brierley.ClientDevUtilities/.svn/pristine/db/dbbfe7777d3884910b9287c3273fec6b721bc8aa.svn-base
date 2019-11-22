using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Information on social media apps used for member authentication and real-time communication.
	/// </summary>
	public class SocialMediaApp
	{
		public string Name { get; set; }
		public SocialNetworkProviderType Type { get; set; }
		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public string CallbackUrl { get; set; }

		public SocialMediaApp()
		{
		}

		public SocialMediaApp(string name, SocialNetworkProviderType type, string consumerKey = null, string consumerSecret = null, string callbackUrl = null)
		{
			Name = name;
			Type = type;
			ConsumerKey = consumerKey;
			ConsumerSecret = consumerSecret;
			CallbackUrl = callbackUrl;
		}
	}
}
