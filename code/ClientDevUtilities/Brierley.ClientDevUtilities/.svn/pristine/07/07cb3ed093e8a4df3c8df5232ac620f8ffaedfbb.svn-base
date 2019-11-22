using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Resources;
using System.Text;

namespace Brierley.FrameWork.Common.Globalization
{
	public class DBResourceReader : IResourceReader, IEnumerable<KeyValuePair<string, object>>
	{
		private ListDictionary _resourceDictionary;

		public DBResourceReader(ListDictionary resourceDictionary)
		{
			_resourceDictionary = resourceDictionary;
		}

		#region IResourceReader Members
		public void Close()
		{
			this.Dispose();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _resourceDictionary.GetEnumerator();
		}

		public void Dispose()
		{
			_resourceDictionary = null;
		}
		#endregion

		#region IEnumerable Members
		public IDictionaryEnumerator GetEnumerator()
		{
			return _resourceDictionary.GetEnumerator();
		}
		#endregion

		#region IEnumerable<KeyValuePair<string,object>> Members
		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return _resourceDictionary.GetEnumerator() as IEnumerator<KeyValuePair<string, object>>;
		}
		#endregion

	}
}
