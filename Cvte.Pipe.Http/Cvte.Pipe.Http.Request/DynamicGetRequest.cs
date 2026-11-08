using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
namespace Cvte.Pipe.Http.Request
{
	public abstract class DynamicGetRequest : RequestBase
	{
		public override string GetRequestUrl()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				DataMemberAttribute dataMemberAttribute = SA_Attr<DataMemberAttribute>.Get(propertyInfo, false);
				if (dataMemberAttribute != null)
				{
					object value = propertyInfo.GetValue(this, null);
					dictionary.Add(dataMemberAttribute.Name, value?.ToString() ?? "");
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetRequestUrl());
			if (dictionary.Any())
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					if (flag)
					{
						stringBuilder.AppendFormat("?{0}={1}", item.Key, item.Value);
						flag = false;
					}
					else
					{
						stringBuilder.AppendFormat("&{0}={1}", item.Key, item.Value);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public override byte[] GetRequestData()
		{
			return new byte[0];
		}
	}
}
