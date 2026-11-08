using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;

namespace Cvte.Net.WebServices.Files.Upload
{
	
	
	internal class UnitFileUploadUtilityProxy
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{
			
			public string fileName;

			
			public IDictionary<string, string> withParams;

			
			public WebHeaderCollection headerCollection;

			
			public IProgress<UploadProgress> uploadProgress;

			
			public string fileKey;

			internal Task<HttpWebResponseWrapper> _003CUploadFile_003Eg__UploadFunc_007C0(string u, string h)
			{
				return UnitFileUploadUtility.UploadFile(u, h, fileName, withParams, headerCollection, uploadProgress, CancellationToken.None, fileKey);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass3_0
		{
			
			public string fileName;

			
			public Stream fileStream;

			
			public IDictionary<string, string> withParams;

			
			public WebHeaderCollection headerCollection;

			
			public IProgress<UploadProgress> uploadProgress;

			public CancellationToken cancellationToken;

			
			public string fileKey;

			public int timeOut;

			internal Task<HttpWebResponseWrapper> _003CUploadFile_003Eg__UploadFunc_007C0(string u, string h)
			{
				return UnitFileUploadUtility2.UploadFile(u, h, fileName, fileStream, withParams, headerCollection, uploadProgress, cancellationToken, fileKey, timeOut);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			
			public string fileName;

			
			public IDictionary<string, string> withParams;

			
			public WebHeaderCollection headerCollection;

			
			public IProgress<UploadProgress> uploadProgress;

			public CancellationToken cancellationToken;

			
			public string fileKey;

			public int timeOut;

			internal Task<HttpWebResponseWrapper> _003CUploadFile_003Eg__UploadFunc_007C0(string u, string h)
			{
				return UnitFileUploadUtility.UploadFile(u, h, fileName, withParams, headerCollection, uploadProgress, cancellationToken, fileKey, timeOut);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			
			public Stream stream;

			public long startOffset;

			public long blockSize;

			
			public WebHeaderCollection headerCollection;

			public CancellationToken cancellationToken;

			
			public string fileKey;

			internal Task<HttpWebResponseWrapper> _003CUploadBlockFile_003Eg__UploadFunc_007C0(string u, string h)
			{
				return UnitFileUploadUtility.UploadBlockFile(u, h, stream, startOffset, blockSize, headerCollection, cancellationToken, fileKey);
			}
		}

		
		private readonly IHttpRequestHostBackupFilter _hostBackupFilter;

		
		public UnitFileUploadUtilityProxy(IHttpRequestHostBackupFilter hostBackupFilter)
		{
			_hostBackupFilter = hostBackupFilter;
		}

		public Task<HttpWebResponseWrapper> UploadFile(string url, string fileName, IDictionary<string, string> withParams, WebHeaderCollection headerCollection,  IProgress<UploadProgress> uploadProgress, string fileKey = "file")
		{
			_003C_003Ec__DisplayClass2_0 obj = new _003C_003Ec__DisplayClass2_0();
			obj.fileName = fileName;
			obj.withParams = withParams;
			obj.headerCollection = headerCollection;
			obj.uploadProgress = uploadProgress;
			obj.fileKey = fileKey;
			return HttpHostBackupUtils.ActionByHostBackup(_hostBackupFilter, obj._003CUploadFile_003Eg__UploadFunc_007C0, url, "POST");
		}

		public Task<HttpWebResponseWrapper> UploadFile(string url, string fileName, Stream fileStream, IDictionary<string, string> withParams, WebHeaderCollection headerCollection, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file", int timeOut = 100000)
		{
			_003C_003Ec__DisplayClass3_0 obj = new _003C_003Ec__DisplayClass3_0();
			obj.fileName = fileName;
			obj.fileStream = fileStream;
			obj.withParams = withParams;
			obj.headerCollection = headerCollection;
			obj.uploadProgress = uploadProgress;
			obj.cancellationToken = cancellationToken;
			obj.fileKey = fileKey;
			obj.timeOut = timeOut;
			return HttpHostBackupUtils.ActionByHostBackup(_hostBackupFilter, obj._003CUploadFile_003Eg__UploadFunc_007C0, url, "POST");
		}

		public Task<HttpWebResponseWrapper> UploadFile(string url, string fileName, IDictionary<string, string> withParams, WebHeaderCollection headerCollection, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file", int timeOut = 100000)
		{
			_003C_003Ec__DisplayClass4_0 obj = new _003C_003Ec__DisplayClass4_0();
			obj.fileName = fileName;
			obj.withParams = withParams;
			obj.headerCollection = headerCollection;
			obj.uploadProgress = uploadProgress;
			obj.cancellationToken = cancellationToken;
			obj.fileKey = fileKey;
			obj.timeOut = timeOut;
			return HttpHostBackupUtils.ActionByHostBackup(_hostBackupFilter, obj._003CUploadFile_003Eg__UploadFunc_007C0, url, "POST");
		}

		public Task<HttpWebResponseWrapper> UploadBlockFile(string url, Stream stream, long startOffset, long blockSize, WebHeaderCollection headerCollection, CancellationToken cancellationToken, string fileKey = "file")
		{
			_003C_003Ec__DisplayClass5_0 obj = new _003C_003Ec__DisplayClass5_0();
			obj.stream = stream;
			obj.startOffset = startOffset;
			obj.blockSize = blockSize;
			obj.headerCollection = headerCollection;
			obj.cancellationToken = cancellationToken;
			obj.fileKey = fileKey;
			return HttpHostBackupUtils.ActionByHostBackup(_hostBackupFilter, obj._003CUploadBlockFile_003Eg__UploadFunc_007C0, url, "POST");
		}
	}
}
