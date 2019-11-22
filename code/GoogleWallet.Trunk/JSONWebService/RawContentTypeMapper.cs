using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace JSONWebService
{
   
   public class RawContentTypeMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {

                if (contentType.Contains("application/json"))
                {

                    return WebContentFormat.Raw;

                }

                else
                {

                    return WebContentFormat.Default;

                }
            }
        }
   
}