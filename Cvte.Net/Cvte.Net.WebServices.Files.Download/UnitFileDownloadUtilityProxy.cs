using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;

namespace Cvte.Net.WebServices.Files.Download
{
	
	
	internal class UnitFileDownloadUtilityProxy
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{
			
			public string filePath;

			public int timeOut;

			internal Task<HttpWebResponseWrapper> _003CDownloadFile_003Eg__DownloadFunc_007C0(string u, string h)
			{
				return UnitFileDownloadUtility.DownloadFile(u, h, filePath, timeOut);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass3_0
		{
			
			public string filePath;

			public long rangeFrom;

			
			public IProgress<DownloadProgress> downloadProgress;

			public CancellationToken cancellationToken;

			public int timeOut;

			internal Task<HttpWebResponseWrapper> _003CDownloadFile_003Eg__DownloadFunc_007C0(string u, string h)
			{
				return UnitFileDownloadUtility.DownloadFile(u, h, filePath, rangeFrom, downloadProgress, cancellationToken, timeOut);
			}
		}

		
		private readonly IHttpRequestHostBackupFilter _hostBackupFilter;

		
		public UnitFileDownloadUtilityProxy(IHttpRequestHostBackupFilter hostBackupFilter)
		{
			_hostBackupFilter = hostBackupFilter;
		}

		public Task<HttpWebResponseWrapper> DownloadFile(string url, string filePath, int timeOut = 30000)
		{
			_003C_003Ec__DisplayClass2_0 obj = new _003C_003Ec__DisplayClass2_0();
			obj.filePath = filePath;
			obj.timeOut = timeOut;
			return HttpHostBackupUtils.ActionByHostBackup(_hostBackupFilter, obj._003CDownloadFile_003Eg__DownloadFunc_007C0, url, "GET");
		}

		public Task<HttpWebResponseWrapper> DownloadFile(string url, string filePath, long rangeFrom, IProgress<DownloadProgress> downloadProgress, CancellationToken cancellationToken, int timeOut = 30000)
		{
			_003C_003Ec__DisplayClass3_0 obj = new _003C_003Ec__DisplayClass3_0();
			obj.filePath = filePath;
			obj.rangeFrom = rangeFrom;
			obj.downloadProgress = downloadProgress;
			obj.cancellationToken = cancellationToken;
			obj.timeOut = timeOut;
			return HttpHostBackupUtils.ActionByHostBackup(_hostBackupFilter, obj._003CDownloadFile_003Eg__DownloadFunc_007C0, url, "GET");
		}
	}
}
