using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Common
{
	public class LanguageChannelUtil
	{
		#region Language/Culture Utilities
		public static string GetDefaultCulture()
		{
			string defaultCulture = LWConfigurationUtil.GetConfigurationValue("LW_DefaultCulture");
			if (string.IsNullOrEmpty(defaultCulture))
			{
				defaultCulture = CultureInfo.CurrentCulture.Name;
			}
			if (string.IsNullOrEmpty(defaultCulture))
			{
				defaultCulture = "en";
			}

			return defaultCulture;
		}

		/// <summary>
		/// This method would expect something like en-US and return en.
		/// </summary>
		/// <param name="langCultureCode"></param>
		/// <returns></returns>
		public static string GetGroupCulture(string langCultureCode)
		{
			string group = langCultureCode;
			if (!string.IsNullOrEmpty(langCultureCode))
			{
				group = langCultureCode.Split('-')[0];
			}
			return group;
		}

		public static bool IsLanguageValid(ContentService service, string language)
		{
			LanguageDef def = null;
			if (!string.IsNullOrEmpty(language))
			{
				def = service.GetLanguageDef(language);
			}
			return def != null;
		}

		#endregion

		#region Channel Utilities
		public static string GetDefaultChannel()
		{
			string channel = "Web";
			string defaultChannel = LWConfigurationUtil.GetConfigurationValue("LW_DefaultChannel");
			if (!string.IsNullOrEmpty(defaultChannel))
			{
				channel = defaultChannel;
			}
			return channel;
		}

		public static bool IsChannelValid(ContentService contentService, string channel)
		{
			ChannelDef def = null;
			if (!string.IsNullOrEmpty(channel))
			{
				def = contentService.GetChannelDef(channel);
			}
			return def != null;
		}
		#endregion
	}
}
