using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace Cvte.Pipe.Http.Request
{
	public class UploadFilesRequest : RequestBase
	{
		private IDictionary<string, string> _0001;

		public EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged;

		public EventHandler<UploadCompletedEventArgs> UploadCompleted;

		public IDictionary<string, string> KeyFilePathMapping
		{
			get
			{
				return _0001;
			}
			set
			{
				_0001 = value;
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
			foreach (PropertyInfo item in from propertyInfo in GetType().GetProperties().ToList()
				where SA_Attr<DataMemberAttribute>.Has(propertyInfo, false)
				select propertyInfo)
			{
				string name = SA_Attr<DataMemberAttribute>.Get(item, false).Name;
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
