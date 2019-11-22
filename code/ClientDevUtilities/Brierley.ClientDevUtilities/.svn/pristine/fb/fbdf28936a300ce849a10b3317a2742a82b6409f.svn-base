using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class LanguageChannelUtil : ILanguageChannelUtil
    {
        public static LanguageChannelUtil Instance { get; private set; }

        static LanguageChannelUtil()
        {
            Instance = new LanguageChannelUtil();
        }

        public string GetDefaultChannel()
        {
            return Brierley.FrameWork.Common.LanguageChannelUtil.GetDefaultChannel();
        }

        public string GetDefaultCulture()
        {
            return Brierley.FrameWork.Common.LanguageChannelUtil.GetDefaultCulture();
        }

        public string GetGroupCulture(string langCultureCode)
        {
            return Brierley.FrameWork.Common.LanguageChannelUtil.GetGroupCulture(langCultureCode);
        }

        public bool IsChannelValid(IContentService contentService, string channel)
        {
            ChannelDef def = null;
            if (!string.IsNullOrEmpty(channel))
            {
                def = contentService.GetChannelDef(channel);
            }
            return def != null;
        }

        public bool IsLanguageValid(IContentService service, string language)
        {
            LanguageDef def = null;
            if (!string.IsNullOrEmpty(language))
            {
                def = service.GetLanguageDef(language);
            }
            return def != null;
        }
    }
}
