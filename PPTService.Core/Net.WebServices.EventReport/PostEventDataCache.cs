using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTService.Core.ComponentModel;
using PPTService.Core.Net.WebServices.EventReport.Models;
using PPTService.Core.Net.WebServices.EventReport.Utils;
using Newtonsoft.Json;

namespace PPTService.Core.Net.WebServices.EventReport
{
	
	
	internal class PostEventDataCache
	{
		private string _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Myou", "Cache");

		public static int MaxCacheCount { get; set; } = 100;

		private PostEventRequestBuilder RequestBuilder { get; }

		private string FridayAppId { get; }

		private PostEventWebService WebService { get; }

		public bool IncludeDefaultProperties { get; set; }

		public string CachePath
		{
			get
			{
				if (!Directory.Exists(_cachePath))
				{
					Directory.CreateDirectory(_cachePath);
				}
				return _cachePath;
			}
			set
			{
				try
				{
					if (!Directory.Exists(value))
					{
						Directory.CreateDirectory(value);
					}
					_cachePath = value;
				}
				catch (Exception)
				{
				}
			}
		}

		public PostEventDataCache(string fridayAppId, PostEventWebService webService)
		{
			FridayAppId = fridayAppId;
			WebService = webService;
			RequestBuilder = new PostEventRequestBuilder(fridayAppId, PostEventHelper.SessionId);
		}

		public async Task CacheDataAsync(IList<Event> events, IDictionary<string, object> commonProperties)
		{
			try
			{
				await Task.Run(() =>
				{
					RequestBuilder.IncludeDefaultProperties = IncludeDefaultProperties;
					PostEventRequest request = RequestBuilder.Build(events, commonProperties);
					string contents = JsonConvert.SerializeObject(new PostEventRequestWrapper
					{
						Request = request,
						AppId = FridayAppId,
						SessionId = PostEventHelper.SessionId
					});
					File.WriteAllText(Path.Combine(CachePath, DateTime.Now.ToString("yy.MM.dd,HH-mm-ss,fff", CultureInfo.InvariantCulture) + ".edc"), contents);
				}).ConfigureAwait(false);
			}
			catch (IOException)
			{
			}
			catch (Exception)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw;
				}
			}
		}

		public async Task FlushCacheDataAsync()
		{
			DirectoryInfo dir = new DirectoryInfo(CachePath);
			if (!dir.Exists || !dir.EnumerateFiles().Any())
			{
				return;
			}
			await Task.Run( async () =>
			{
				FileInfo[] files = dir.GetFiles("*.edc");
				if (files.Length > MaxCacheCount)
				{
					try
					{
						Directory.Delete(dir.FullName, true);
						return;
					}
					catch (IOException)
					{
						return;
					}
					catch (Exception)
					{
						if (DebuggingProperties.IsDebug)
						{
							throw;
						}
						return;
					}
				}
				FileInfo[] array = files;
				foreach (FileInfo cacheFile in array)
				{
					await FlushFileDataAsync(cacheFile).ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
		}

		private async Task FlushFileDataAsync(FileInfo cacheFile)
		{
			string path = cacheFile.FullName;
			if (!cacheFile.Exists)
			{
				return;
			}
			try
			{
				PostEventRequestWrapper wrapper = JsonConvert.DeserializeObject<PostEventRequestWrapper>(File.ReadAllText(path));
				if (await ReportCacheDataAsync(wrapper).ConfigureAwait(false))
				{
					File.Delete(path);
				}
			}
			catch (IOException)
			{
			}
			catch (Exception)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw;
				}
				try
				{
					File.Delete(path);
				}
				catch (IOException)
				{
				}
			}
		}

		private async Task<bool> ReportCacheDataAsync(PostEventRequestWrapper wrapper)
		{
			if (wrapper == null)
			{
				return false;
			}
			return (await WebService.PostEvent(wrapper).ConfigureAwait(false)).Success;
		}
	}
}
