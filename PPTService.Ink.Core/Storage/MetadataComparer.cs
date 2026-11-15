using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class MetadataComparer : IEqualityComparer<MetadataSaveInfo>
	{
		public bool Equals(MetadataSaveInfo x, MetadataSaveInfo y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return x.Name.Equals(y.Name);
		}

		public int GetHashCode(MetadataSaveInfo obj)
		{
			return obj.Name.GetHashCode();
		}
	}
}
