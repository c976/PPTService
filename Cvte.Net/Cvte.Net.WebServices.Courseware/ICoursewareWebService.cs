using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Courseware
{
	
	internal interface ICoursewareWebService : IWebService
	{
		Task<GetListCoursewareHttpResponse> GetListAsync(int index, int pageSize);

		Task<CreateCoursewareHttpResponse> CreateAsync(CoursewarePackage coursewarePackage);

		Task<PushCoursewareHttpResponse> PushAsync(string coursewareId, long version, CoursewarePackage coursewarePackage);

		Task<CoursewareHttpResponse> CheckoutAsync(string coursewareId, string path, IProgress<CheckoutProgress> progress, CancellationToken cancellationToken);

		Task<PullCoursewareHttpResponse> PullAsync(string coursewareId, string path, IProgress<PullProgress> progress, CancellationToken cancellationToken);

		Task<ResolveCoursewareHttpResponse> ResolveAsync(string coursewareId, long version);

		Task<CoursewareHttpResponse> DeleteAsync(string coursewareId);

		Task<CoursewareHttpResponse> UpdateNameAsync(string coursewareId, string newName);
	}
}
