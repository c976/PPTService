using Cvte.Pipe.Http.Request;
using Cvte.Pipe.Http.Response;

namespace Cvte.Pipe.Http
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
