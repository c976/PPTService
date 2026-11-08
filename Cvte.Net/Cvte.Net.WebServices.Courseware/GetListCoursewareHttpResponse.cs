using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	public class GetListCoursewareHttpResponse : CoursewareHttpResponse
	{
		public IList<CoursewareInfoEntry> Items { get; internal set; }
	}
}
