//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.IntelligentSearch;

namespace Brierley.FrameWork.Common
{
	public class AddressCleansingUtility : IDisposable
	{
		private const string _className = "AddressCleansingUtility";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string _url;
		private long _timeout = -1; // in milliseconds
		private CorrectAddressServicePortTypeClient _proxy = null;
		private bool _disposed = false;


		public AddressCleansingUtility(string url)
		{
			_url = url;
			Initialize();
		}

		public AddressCleansingUtility(string url, long timeout)
		{
			_url = url;
			_timeout = timeout;
			Initialize();
		}

		public void Initialize()
		{
			string methodName = "Initializing";
			if (_proxy == null)
			{
				_logger.Debug(_className, methodName, "Initializing address cleansing proxy with Url: " + _url);
				try
				{
					EndpointAddress addr = new EndpointAddress(_url);
					Binding wsBinding = new WSHttpBinding();
					((WSHttpBinding)wsBinding).Security.Mode = SecurityMode.None;
					if (_timeout > 0)
					{
						wsBinding.SendTimeout = TimeSpan.FromMilliseconds(_timeout);
					}
					_proxy = new CorrectAddressServicePortTypeClient(wsBinding, addr);
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Unable to initilaize address cleansing proxy.", ex);
					throw;
				}
			}
		}


		public string ServiceUrl
		{
			get { return _url; }
			set { _url = value; }
		}

		public void Dispose()
		{
			Dispose(true);
		}
		public void Dispose(bool disposing)
		{
			string methodName = "Dispose";
			if (!_disposed)
			{
				if (disposing)
				{
					if (_proxy != null)
					{
						_proxy.Close();
						_proxy = null;
						_logger.Debug(_className, methodName, "Address cleansing proxy closed.");
					}
				}
				_disposed = true;
			}
			
		}

		public Address CleanAddress(Address addr)
		{
			string methodName = "CleanAddress";
			if (_proxy != null)
			{				
				try
				{
					LWCAAddress addrIn = new LWCAAddress();
					addrIn.lineOne = addr.AddressLineOne;
					//addrIn.lineTwo = addr.LineTwo;
					//addrIn.lineThree = addr.LineThree;
					//addrIn.lineFour = addr.LineFour;
					addrIn.city = addr.City;
					addrIn.stateOrProvince = addr.StateOrProvince;
					addrIn.zipOrPostalCode = addr.ZipOrPostalCode;					
					addrIn.county = addr.County;
					addrIn.country = addr.Country;
                    //addrIn.longitude = addr.Longitude;
                    //addrIn.latitude = addr.Longitude;

					LWCAAddress addrOut = _proxy.getCleanAddress(addrIn);

					if (addrOut.returnCode == 1)
					{
						addr.AddressLineOne = addrOut.lineOne;
						addr.City = addrOut.city;
						addr.StateOrProvince = addrOut.stateOrProvince;
						addr.ZipOrPostalCode = addrOut.zipOrPostalCode;
						addr.Longitude = addrOut.longitude;
						addr.Latitude = addrOut.latitude;
					}
					else
					{
						string msg = string.Format("Unable to clean address.  DPV = {0}, Return code = {1}, Error codes = {2}",
							addrOut.dpv,addrOut.returnCode,addrOut.errorCode);
						_logger.Error(_className, methodName, msg);
						throw new LWAddressValidationException(addrOut.returnCode, addrOut.dpv, addrOut.errorCode, msg); 
					}
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Error getting clean address.", ex);
					throw;
				}								
			}
			else
			{
				_logger.Error(_className, methodName, "Adrdess cleansing service has not been initialized yet.");
				throw new LWException("Adrdess cleansing service has not been initialized yet.");
			}
			return addr;
		}
	}
}
