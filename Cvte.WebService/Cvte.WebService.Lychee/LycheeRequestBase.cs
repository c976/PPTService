using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Pipe.Http.Request;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Lychee
{
	[DataContract]
	internal class LycheeRequestBase : RequestBase
	{
		[DataMember(Name = "sign")]
		public string Sign { get; set; }

		[DataMember(Name = "app")]
		public string App { get; set; }

		[DataMember(Name = "timestamp")]
		public long TimeStamp { get; set; }

		[DataMember(Name = "version")]
		public int Version { get; set; }

		public override byte[] GetRequestData()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				DataMemberAttribute dataMemberAttribute = PropertyAttributeHelper<DataMemberAttribute>.smethod_0(propertyInfo);
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
			if (dictionary.Any())
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					string text = SignatureUrlEncoder.smethod_0(item.Value, Encoding.UTF8);
					if (flag)
					{
						stringBuilder.Append(item.Key + "=" + text);
						flag = false;
					}
					else
					{
						stringBuilder.Append("&" + item.Key + "=" + text);
					}
				}
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}
	}
}
