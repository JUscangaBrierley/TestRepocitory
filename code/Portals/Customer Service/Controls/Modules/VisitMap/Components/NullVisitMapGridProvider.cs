using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Controls.VisitMap;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.VisitMap.Components
{
	public class NullVisitMapGridProvider : IVisitMapProvider
	{
		private VisitMapConfig _config = null;

		#region IVisitMapProvider implementation
		public bool HasGridProvider()
		{
			return false;
		}

		public IDynamicGridProvider GetGridProvider()
		{
			return null;
		}

		public bool HasListProvider()
		{
			return false;
		}

		public AspListProviderBase GetListProvider()
		{
			return null;
		}

		public int GetNumVisits()
		{
			return 0;
		}

		public void SetVisitMapConfig(VisitMapConfig config)
		{
			_config = config;
		}

		public IList<string> GetJSonStoresVisited()
		{
			IList<string> result = new List<string>();
			return result;
		}

		public IList<string> GetJSonStoresNotVisited()
		{
			IList<string> result = new List<string>();
			return result;
		}

		public IList<string> GetJSonStoresWithQualifiedSpend()
		{
			IList<string> result = new List<string>();
			return result;
		}

		public IList<string> GetJSonStoresAll()
		{
			IList<string> result = new List<string>();
			return result;
		}

		public string GetMarkerImageUrl(MarkerType markerType, string userSpecifiedURL)
		{
			string result = "http://maps.google.com/mapfiles/ms/micons/red-dot.png";
			return result;
		}

		public string GetVisitMapShareUrl()
		{
			string result = "http://maps.google.com/";
			return result;
		}
		#endregion
	}
}
