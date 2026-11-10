using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Pipe.Http.Request;

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
			// 与反混淆后的原版一致：按反射属性顺序收集 [DataMember] 参数，
			// 值逐字符 URL 编码后拼 key=value&...，再以 UTF-8 编码成请求体。
			Dictionary<string, string> values = new Dictionary<string, string>();
			foreach (PropertyInfo property in GetType().GetProperties())
			{
				DataMemberAttribute dataMember = LycheeSign.GetDataMember(property);
				if (dataMember == null)
				{
					continue;
				}
				object value = property.GetValue(this, null);
				values.Add(dataMember.Name, (value == null) ? "" : (value.ToString() ?? ""));
			}

			StringBuilder builder = new StringBuilder();
			bool first = true;
			foreach (KeyValuePair<string, string> item in values)
			{
				string text = LycheeSign.UrlEncode(item.Value, Encoding.UTF8);
				if (first)
				{
					builder.Append(item.Key + "=" + text);
					first = false;
				}
				else
				{
					builder.Append("&" + item.Key + "=" + text);
				}
			}
			return Encoding.UTF8.GetBytes(builder.ToString());
		}
	}
}
