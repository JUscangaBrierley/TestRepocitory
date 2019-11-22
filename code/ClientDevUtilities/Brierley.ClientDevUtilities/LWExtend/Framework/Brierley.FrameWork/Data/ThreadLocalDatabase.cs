using System;
using PetaPoco;

namespace Brierley.FrameWork.Data
{
	internal class ThreadLocalDatabase
	{
		public Database Database { get; set; }
		public int ReferenceCount { get; set; }
	}
}
