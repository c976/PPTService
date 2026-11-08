using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	public class CoursewarePackage
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Hash { get; set; }

		public long Size { get; set; }

		public string Version { get; set; }

		public string Author { get; set; }

		public string ClientVersion { get; set; }

		public IList<CoursewareFile> Files { get; set; }
	}
}
