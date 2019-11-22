using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Brierley.FrameWork.Common
{
    public class WCFUtil
    {
        public static Binding GetBinding(string url)
        {
            return GetBinding(new Uri(url));            
        }

        public static Binding GetBinding(Uri uri)
        {
            Binding binding = null;
            if (uri.Scheme == "http")
            {
                binding = new BasicHttpBinding();
                ((BasicHttpBinding)binding).ReaderQuotas.MaxStringContentLength = 16384;
            }
            else if (uri.Scheme == "https")
            {
                binding = new BasicHttpBinding();
                ((BasicHttpBinding)binding).ReaderQuotas.MaxStringContentLength = 16384;
                ((BasicHttpBinding)binding).Security.Mode = BasicHttpSecurityMode.Transport;
            }
            else if (uri.Scheme == "net.tcp")
            {
                binding = new NetTcpBinding();
            }
            else if (uri.Scheme == "net.pipe")
            {
                binding = new NetNamedPipeBinding();
            }
            else
            {
                return null;
            }
            return binding;
        }
    }
}
