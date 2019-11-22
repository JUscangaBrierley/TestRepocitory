﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Brierley.LoyaltyWare.LWIntegrationSvc
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="LWAPIResponse", Namespace="http://schemas.datacontract.org/2004/07/Brierley.LoyaltyWare.LWIntegrationSvc")]
    public partial class LWAPIResponse : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double ElapsedTimeField;
        
        private int ResponseCodeField;
        
        private string ResponseDescriptionField;
        
        private string ResponseDetailField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double ElapsedTime
        {
            get
            {
                return this.ElapsedTimeField;
            }
            set
            {
                this.ElapsedTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ResponseCode
        {
            get
            {
                return this.ResponseCodeField;
            }
            set
            {
                this.ResponseCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ResponseDescription
        {
            get
            {
                return this.ResponseDescriptionField;
            }
            set
            {
                this.ResponseDescriptionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ResponseDetail
        {
            get
            {
                return this.ResponseDetailField;
            }
            set
            {
                this.ResponseDetailField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="urn:Brierley.LoyaltyWare.LWIntegrationSvc", ConfigurationName="ILWIntegrationService")]
public interface ILWIntegrationService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="urn:Brierley.LoyaltyWare.LWIntegrationSvc/ILWIntegrationService/Execute", ReplyAction="urn:Brierley.LoyaltyWare.LWIntegrationSvc/ILWIntegrationService/ExecuteResponse")]
    Brierley.LoyaltyWare.LWIntegrationSvc.LWAPIResponse Execute(string operationName, string source, string sourceEnv, string externalId, string payload);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface ILWIntegrationServiceChannel : ILWIntegrationService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class LWIntegrationServiceClient : System.ServiceModel.ClientBase<ILWIntegrationService>, ILWIntegrationService
{
    
    public LWIntegrationServiceClient()
    {
    }
    
    public LWIntegrationServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public LWIntegrationServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public LWIntegrationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public LWIntegrationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public Brierley.LoyaltyWare.LWIntegrationSvc.LWAPIResponse Execute(string operationName, string source, string sourceEnv, string externalId, string payload)
    {
        return base.Channel.Execute(operationName, source, sourceEnv, externalId, payload);
    }
}
