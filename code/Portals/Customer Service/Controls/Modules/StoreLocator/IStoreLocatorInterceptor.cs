using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.LWModules.StoreLocator
{
	public interface IStoreLocatorInterceptor
	{
		LocationItem NearestLocationItem(double lattitude, double longitude);
		LocationItem NearestLocationItem(string city, string stateOrProvince);
		LocationItem NearestLocationItem(string zipOrPostalCode);
	}
}
