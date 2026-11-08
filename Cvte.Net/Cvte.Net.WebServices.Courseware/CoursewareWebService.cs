using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	internal class CoursewareWebService : WebService, ICoursewareWebService, IWebService
	{
		public Task<GetListCoursewareHttpResponse> GetListAsync(int index, int pageSize)
		{
			throw new NotImplementedException();
		}

		public Task<CreateCoursewareHttpResponse> CreateAsync(CoursewarePackage coursewarePackage)
		{
			throw new NotImplementedException();
		}

		public Task<PushCoursewareHttpResponse> PushAsync(string coursewareId, long version, CoursewarePackage coursewarePackage)
		{
			throw new NotImplementedException();
		}

		public Task<CoursewareHttpResponse> CheckoutAsync(string coursewareId, string path, IProgress<CheckoutProgress> progress, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<PullCoursewareHttpResponse> PullAsync(string coursewareId, string path, IProgress<PullProgress> progress, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ResolveCoursewareHttpResponse> ResolveAsync(string coursewareId, long version)
		{
			throw new NotImplementedException();
		}

		public Task<CoursewareHttpResponse> DeleteAsync(string coursewareId)
		{
			throw new NotImplementedException();
		}

		public Task<CoursewareHttpResponse> UpdateNameAsync(string coursewareId, string newName)
		{
			throw new NotImplementedException();
		}
	}
}
