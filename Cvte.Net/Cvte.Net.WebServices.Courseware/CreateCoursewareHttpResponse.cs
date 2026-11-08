using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	public class CreateCoursewareHttpResponse : CoursewareHttpResponse
	{
		public string CoursewareId { get; internal set; }

		public long Version { get; internal set; }
	}
}
