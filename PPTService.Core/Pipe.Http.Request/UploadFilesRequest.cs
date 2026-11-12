using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PPTService.Core.Pipe.Http.Internal;

namespace PPTService.Core.Pipe.Http.Request
{
	public class UploadFilesRequest : RequestBase
	{
		private IDictionary<string, string> idictionary_0;

		public EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged;

		public EventHandler<UploadCompletedEventArgs> UploadCompleted;

		public IDictionary<string, string> KeyFilePathMapping
		{
			get
			{
				return idictionary_0;
			}
			set
			{
				idictionary_0 = value;
			}
		}

		public override string ContentType => "multipart/form-data;";

		public UploadFilesRequest()
		{
			KeyFilePathMapping = new Dictionary<string, string>();
		}

		public override byte[] GetRequestData()
		{
			return null;
		}

		public virtual IDictionary<string, string> GetParams()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (PropertyInfo item in from propertyInfo_0 in GetType().GetProperties().ToList()
				where AttributeHelper<DataMemberAttribute>.smethod_2(propertyInfo_0)
				select propertyInfo_0)
			{
				string name = AttributeHelper<DataMemberAttribute>.smethod_5(item).Name;
				if (!dictionary.ContainsKey(name))
				{
					string value = item.GetValue(this, null).ToString();
					dictionary.Add(name, value);
				}
			}
			return dictionary;
		}

		public void UpdateProgress(long current, long total)
		{
			if (current == total)
			{
				UploadCompleted?.Invoke(this, new UploadCompletedEventArgs());
				return;
			}
			UploadProgressChanged?.Invoke(this, new UploadProgressChangedEventArgs
			{
				CurrentProgress = current,
				TotalProgress = total
			});
		}
	}
}
