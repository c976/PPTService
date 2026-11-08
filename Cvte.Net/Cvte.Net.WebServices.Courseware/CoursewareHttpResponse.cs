using Cvte.Net.Http;
using Cvte.Net.WebServices.Monitors;

namespace Cvte.Net.WebServices.Courseware
{
	public class CoursewareHttpResponse : HttpResponse, IApiMonitorResponse
	{
		public CoursewareHttpResponseCode ResponseCode { get; internal set; }

		public override bool IsSuccessBusiness
		{
			get
			{
				if (ResponseCode == CoursewareHttpResponseCode.Ok)
				{
					return Error == null;
				}
				return false;
			}
		}
	}
}
