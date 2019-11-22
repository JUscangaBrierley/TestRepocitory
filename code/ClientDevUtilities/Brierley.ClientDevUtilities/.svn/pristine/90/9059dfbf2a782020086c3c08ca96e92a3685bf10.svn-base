using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork.bScript;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	public class LangChanContentBase : LWCoreObjectBase
	{
		private const string _className = "LangChanContentBase";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private static Dictionary<string, string> _cultureMap = new Dictionary<string, string>();
		public ContentObjType ContentType { get; set; }

		/// <summary>
		/// Gets or sets the ID for the current MessageDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public Int64 Id { get; set; }

		///// <summary>
		///// 
		///// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public Dictionary<string, List<LangChanContent>> Contents { get; set; }

		protected LangChanContentBase(ContentObjType type)
		{
			ContentType = type;
			Contents = new Dictionary<string, List<LangChanContent>>();
		}


		public static string GetMappedCulture(string culture)
		{
			string methodName = "GetMappedCulture";
			lock (_cultureMap)
			{
				if (!_cultureMap.ContainsKey(culture))
				{
					string mappedCulture = culture;
					using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
					{
						LanguageDef def = service.GetLanguageDef(mappedCulture);
						if (def == null)
						{
							_logger.Trace(_className, methodName, string.Format("Culture {0} is not mapped. Looking for its group culture.", culture));
							mappedCulture = LanguageChannelUtil.GetGroupCulture(culture);
							if (culture != mappedCulture)
							{
								_logger.Debug(_className, methodName, string.Format("Group culture {0} is {1}.", culture, mappedCulture));
								def = service.GetLanguageDef(mappedCulture);
							}
						}
						if (def != null)
						{
							_cultureMap.Add(culture, def.Culture);
							return def.Culture;
						}
						else
						{
							string errMsg = string.Format("Unable to find definition for culture {0}", mappedCulture);
							_logger.Error(_className, methodName, errMsg);
							throw new LWException(errMsg) { ErrorCode = 3213 };
						}
					}
				}
				else
				{
					return _cultureMap[culture];
				}
			}
		}

		public string GetContent(string culture, string channel, string name)
		{
			string content = string.Empty;
			string mappedCulture = GetMappedCulture(culture);
			if (Contents.Count > 0)
			{
				if (Contents.ContainsKey(name))
				{
					List<LangChanContent> langChanContent = Contents[name];
					content = (from x in langChanContent where x.LanguageCulture == mappedCulture && x.Channel == channel select x.Content).FirstOrDefault();
				}
			}
			return content;
		}

		public List<LangChanContent> GetContent(string name)
		{
			List<LangChanContent> langChanContent = new List<LangChanContent>();
			if (Contents.Count > 0)
			{
				if (Contents.ContainsKey(name))
				{
					langChanContent = Contents[name];
				}
			}
			return langChanContent;
		}

		public void SetContent(string culture, string channel, string name, string content)
		{
			List<LangChanContent> langChanContent = null;
			string mappedCulture = GetMappedCulture(culture);
			if (Contents.ContainsKey(name))
			{
				langChanContent = Contents[name];
			}
			else
			{
				langChanContent = new List<LangChanContent>();
				Contents.Add(name, langChanContent);
			}
			LangChanContent lc = (from x in langChanContent where x.LanguageCulture == mappedCulture && x.Channel == channel && x.Name == name select x).FirstOrDefault();
			if (lc == null)
			{
				lc = new LangChanContent()
				{
					LanguageCulture = mappedCulture,
					Channel = channel,
					LangChanType = ContentType,
					RefId = Id,
					Name = name
				};
				langChanContent.Add(lc);
			}
			lc.Content = content;
		}

		public void RemoveEmptyContentContent()
		{
			var newContent = new Dictionary<string, List<LangChanContent>>();
			if (Contents.Count > 0)
			{
				foreach (string name in Contents.Keys)
				{
					List<LangChanContent> newList = new List<LangChanContent>();
					if (Contents.ContainsKey(name))
					{
						List<LangChanContent> langChanContent = Contents[name];
						foreach (LangChanContent c in langChanContent)
						{
							if (!string.IsNullOrWhiteSpace(c.Content))
							{
								newList.Add(c);
							}
						}
					}
					if (newList.Count > 0)
					{
						newContent.Add(name, newList);
					}
				}
			}
			Contents = newContent;
		}

		public string EvaluateBScript(Member member, string content)
		{
			if (member != null && !string.IsNullOrWhiteSpace(content))
			{
				content = ExpressionUtil.ParseExpressions(content, new ContextObject() { Owner = member });
			}
			return content;
		}

		public LangChanContentBase Clone(LangChanContentBase dest)
		{
			dest.Contents.Clear();
			foreach (string name in Contents.Keys)
			{
				List<LangChanContent> contentList = null;
				if (dest.Contents.ContainsKey(name))
				{
					contentList = dest.Contents[name];
				}
				else
				{
					contentList = new List<LangChanContent>();
					dest.Contents.Add(name, contentList);
				}
				foreach (LangChanContent lc in Contents[name])
				{
					LangChanContent c = lc.Clone();
					contentList.Add(c);
				}
			}
			base.Clone(dest);
			return dest;
		}
	}
}
