package com.brierley.loyaltyware.clientlib.proxy;

public class ILWIntegrationServiceProxy implements com.brierley.loyaltyware.clientlib.proxy.ILWIntegrationService {
  private String _endpoint = null;
  private com.brierley.loyaltyware.clientlib.proxy.ILWIntegrationService iLWIntegrationService = null;
  
  public ILWIntegrationServiceProxy() {
    _initILWIntegrationServiceProxy();
  }
  
  public ILWIntegrationServiceProxy(String endpoint) {
    _endpoint = endpoint;
    _initILWIntegrationServiceProxy();
  }
  
  private void _initILWIntegrationServiceProxy() {
    try {
      iLWIntegrationService = (new com.brierley.loyaltyware.clientlib.proxy.LWIntegrationServiceLocator()).getBasicHttpBinding_ILWIntegrationService();
      if (iLWIntegrationService != null) {
        if (_endpoint != null)
          ((javax.xml.rpc.Stub)iLWIntegrationService)._setProperty("javax.xml.rpc.service.endpoint.address", _endpoint);
        else
          _endpoint = (String)((javax.xml.rpc.Stub)iLWIntegrationService)._getProperty("javax.xml.rpc.service.endpoint.address");
      }
      
    }
    catch (javax.xml.rpc.ServiceException serviceException) {}
  }
  
  public String getEndpoint() {
    return _endpoint;
  }
  
  public void setEndpoint(String endpoint) {
    _endpoint = endpoint;
    if (iLWIntegrationService != null)
      ((javax.xml.rpc.Stub)iLWIntegrationService)._setProperty("javax.xml.rpc.service.endpoint.address", _endpoint);
    
  }
  
  public com.brierley.loyaltyware.clientlib.proxy.ILWIntegrationService getILWIntegrationService() {
    if (iLWIntegrationService == null)
      _initILWIntegrationServiceProxy();
    return iLWIntegrationService;
  }
  
  public com.brierley.loyaltyware.clientlib.datacontract.LWAPIResponse execute(java.lang.String operationName, java.lang.String source, java.lang.String payload) throws java.rmi.RemoteException{
    if (iLWIntegrationService == null)
      _initILWIntegrationServiceProxy();
    return iLWIntegrationService.execute(operationName, source, payload);
  }
  
  
}