using PPTService.Core.Pipe.Http.Request;
using PPTService.Core.Pipe.Http.Response;

namespace PPTService.Core.Pipe.Http
{
	public interface IHttpPipe
	{
		TResponse Get<TRequest, TResponse>(TRequest request) where TRequest : RequestBase where TResponse : ResponseBase, new();

		TResponse Post<TRequest, TResponse>(TRequest request) where TRequest : RequestBase where TResponse : ResponseBase, new();

		TResponse UploadFile<TRequest, TResponse>(TRequest request) where TRequest : UploadFileRequest where TResponse : ResponseBase, new();

		TResponse UploadFiles<TRequest, TResponse>(TRequest request) where TRequest : UploadFilesRequest where TResponse : ResponseBase, new();

		TResponse DownloadFile<TRequest, TResponse>(TRequest request) where TRequest : DownloadFileRequest where TResponse : ResponseBase, new();
	}
}
