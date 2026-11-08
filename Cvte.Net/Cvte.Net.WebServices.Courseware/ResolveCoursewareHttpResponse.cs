using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	public class ResolveCoursewareHttpResponse : CoursewareHttpResponse
	{
		public string NewCoursewareId { get; internal set; }

		public long NewVersion { get; internal set; }
	}
}
