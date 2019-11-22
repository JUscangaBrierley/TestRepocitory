using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace JSONWebService
{
    [DataContract]
    [Serializable]


    public class WalletUser
    {
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string zipcode { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string phone { get; set; }
        [DataMember]
        public string gender { get; set; }
        [DataMember]
        public string addressLine1 { get; set; }
        [DataMember]
        public string addressLine2 { get; set; }
        [DataMember]
        public List<string> userModifiedFields { get; set; }
    }

    public class Params
    {
        [DataMember]
        public string linkingId { get; set; }
        [DataMember]
        public WalletUser walletUser { get; set; }
        [DataMember]
        public bool promotionalEmailOptIn { get; set; }
        [DataMember]
        public bool tosUserAcceptance { get; set; }
    }

    public class WalletRootObject
    {
        [DataMember]
        public string method { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public Params @params { get; set; }
        [DataMember]
        public string apiVersion { get; set; }
    }
    
    //public class WalletUser
    //{
    //    [DataMember]
    //    public string firstName { get; set; }
    //    [DataMember]
    //    public string lastName { get; set; }
    //    [DataMember]
    //    public string addressLine1 { get; set; }
    //    [DataMember]
    //    public string addressLine2 { get; set; }
    //    [DataMember]
    //    public string city { get; set; }
    //    [DataMember]
    //    public string state { get; set; }
    //    [DataMember]
    //    public string zipcode { get; set; }
    //    [DataMember]
    //    public string country { get; set; }
    //    [DataMember]
    //    public string email { get; set; }
    //    [DataMember]
    //    public string phone { get; set; }
    //    [DataMember]
    //    public string gender { get; set; }
    //    [DataMember]
    //    public List<string> userModifiedFields { get; set; }
    //}

    //public class Params
    //{
    //    [DataMember]
    //    public WalletUser walletUser { get; set; }
    //    [DataMember]
    //    public bool tosUserAcceptance { get; set; }
    //    [DataMember]
    //    public bool promotionalEmailOptIn { get; set; }
    //}

    //public class walletRootObject
    //{
    //    [DataMember]
    //    public string apiVersion { get; set; }
    //    [DataMember]
    //    public string method { get; set; }
    //    [DataMember]
    //    public string id { get; set; }
    //    [DataMember]
    //    public Params @params { get; set; }
    //}
    
}