using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.Http.HostBackup
{
	internal static class HttpHostBackupUtils
	{
		
		public static async Task<HttpWebResponseWrapper> ActionByHostBackup( IHttpRequestHostBackupFilter hostBackupFilter, Func<string, string, Task<HttpWebResponseWrapper>> action, string url, string methodName)
		{
			if (hostBackupFilter == null)
			{
				return await action(url, "").ConfigureAwait(false);
			}
			HttpRequestHostBackupContext context = new HttpRequestHostBackupContext(url, methodName);
			IHttpRequestHostBackupHandler handler = hostBackupFilter.BuildHandler(context);
			HttpWebResponseWrapper httpWebResponseWrapper = ((!(await handler.TryGetBackupContext(context))) ? (await action(url, "").ConfigureAwait(false)) : (await action(context.Url, context.Host).ConfigureAwait(false)));
			HttpWebResponseWrapper responseWrapper = httpWebResponseWrapper;
			while (await handler.TryGetNextBackupContext(responseWrapper, context))
			{
				responseWrapper = await action(context.Url, context.Host);
			}
			return responseWrapper;
		}
	}
}
