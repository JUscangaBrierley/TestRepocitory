using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PetaPoco
{
	public static partial class Mappers
	{
		/// <summary>
		/// revokes an assembly by its full name
		/// </summary>
		/// <remarks>
		/// Loaded data model assemblies do not always match on reference. This method revokes mappers based on the full name, 
		/// rather than relying on the reference to the assembly.
		/// </remarks>
		/// <param name="a"></param>
		public static void RevokeAssembly(Assembly a)
		{
			_lock.EnterWriteLock();
			try
			{
				var matches = _mappers.Keys.Where(o => o is Type && ((Type)o).Assembly.FullName == a.FullName);
				foreach (var match in matches.ToList())
				{
					_mappers.Remove(match);
				}
			}
			finally
			{
				_lock.ExitWriteLock();
				FlushCaches();
			}
		}
	}
}
