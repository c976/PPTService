using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.Pipe.Http
{
	public class PostFileRequest : RequestBase
	{
		private string _0001;

		public string FilePath
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

		public override byte[] GetRequestData()
		{
			return null;
		}

		public IDictionary<string, string> GetParams()
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
	}
}
