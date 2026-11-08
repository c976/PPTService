using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Lychee
{
	
	public interface ILycheeWebService : IWebService
	{
		Task<ParsingPptLycheeHttpResponse> ParsingPptAsync(string filePath);

		Task<PublishFileLycheeHttpResponse> PublishFileAsync(string filePath);

		Task<PublishFileLycheeHttpResponse> PublishFilesAsync(IList<string> files, PublicInfo info);

		Task<PublishFileLycheeHttpResponse> PublishFilesAsync(IList<string> files, PublicInfo info, CancellationToken cancellationToken);

		Task<PublishFileLycheeHttpResponse> PublishFilesAsync(IList<string> files, PublicInfo info, CancellationToken cancellationToken, Action<double> progressChanged);
	}
}
