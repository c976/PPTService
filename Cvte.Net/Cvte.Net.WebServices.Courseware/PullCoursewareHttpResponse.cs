using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	public class PullCoursewareHttpResponse : CoursewareHttpResponse
	{
		public string CoursewareId { get; internal set; }

		public long Version { get; internal set; }
	}
}
