using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;
using Cvte.Pipe.Http.Internal;

namespace Cvte.Pipe.Http
{
	public class PostFileRequest : RequestBase
	{
		private string string_0;

		public string FilePath
		{
			get
			{
				return string_0;
			}
			set
			{
				string_0 = value;
			}
		}

		public override byte[] GetRequestData()
		{
			return null;
		}

		public IDictionary<string, string> GetParams()
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
	}
}
