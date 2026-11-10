using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Pipe.Http.Internal;

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
				DataMemberAttribute dataMemberAttribute = AttributeHelper<DataMemberAttribute>.smethod_5(propertyInfo);
				if (dataMemberAttribute == null)
				{
					continue;
				}
				object value = propertyInfo.GetValue(this, null);
				string name = dataMemberAttribute.Name;
				object obj;
				if (value == null)
				{
					obj = null;
				}
				else
				{
					obj = value.ToString();
					if (obj != null)
					{
						goto IL_0053;
					}
				}
				obj = "";
				goto IL_0053;
				IL_0053:
				dictionary.Add(name, (string)obj);
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
