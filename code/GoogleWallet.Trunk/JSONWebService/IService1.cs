using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Xml;

namespace JSONWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {     
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "Walletrequest")]
        Stream WalletRequest(Stream JSONdataStream);
     //   [OperationContract]
     //   [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "Inserclass")]
     //   string InsertClass(Stream JSONdataStream);
    }   
}
