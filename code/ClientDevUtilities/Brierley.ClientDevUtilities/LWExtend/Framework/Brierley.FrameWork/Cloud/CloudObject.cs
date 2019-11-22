using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Cloud
{
	public class CloudObject
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public string URL { get; set; }
		public long Size { get; set; }
	}

	public class CloudFile : CloudObject
	{
	}

	public class CloudDirectory : CloudObject
	{
	}

	public class CloudObjectCollection : List<CloudObject>
	{
		public List<CloudDirectory> GetDirectories()
		{
			var dirs = from x in this where x is CloudDirectory orderby x.Name select x as CloudDirectory;
			List<CloudDirectory> result = dirs.ToList<CloudDirectory>();
			return result;
		}

		public List<CloudFile> GetFiles()
		{
			var dirs = from x in this where x is CloudFile orderby x.Name select x as CloudFile;
			List<CloudFile> result = dirs.ToList<CloudFile>();
			return result;
		}
	}
}
