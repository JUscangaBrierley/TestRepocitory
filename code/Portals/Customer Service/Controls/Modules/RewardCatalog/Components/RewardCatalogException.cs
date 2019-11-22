using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.LWModules.RewardCatalog
{
    public class RewardCatalogException : LWException
    {
        public RewardCatalogException()
            : base()
        {
        }
        
        public RewardCatalogException(string message)
            : base(message)
        {
        }

        public RewardCatalogException(string message, Exception exception)
            : base(message, exception)
        {
        }     
    }
}
