using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using _0005;
using _0008;
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

		public unsafe override byte[] GetRequestData()
		{
			Dictionary<string, string> dictionary = default(Dictionary<string, string>);
			StringBuilder stringBuilder = default(StringBuilder);
			if (2 != 0)
			{
				dictionary = new Dictionary<string, string>();
				PropertyInfo[] array = SA_Proxy._007E_008D___001F(SA_Proxy._008C___001E(this));
				int num = default(int);
				if (0 == 0)
				{
					num = 0;
				}
				while (true)
				{
					int num2 = num;
					IntPtr intPtr = (IntPtr)(void*)array.LongLength;
					int num3;
					do
					{
						num3 = (int)(long)intPtr;
						intPtr = (IntPtr)num3;
					}
					while (1 == 0);
					if (num2 >= num3)
					{
						break;
					}
					PropertyInfo propertyInfo = array[num];
					DataMemberAttribute dataMemberAttribute = SA_Attr<DataMemberAttribute>._0001(propertyInfo, false);
					if (dataMemberAttribute != null)
					{
						object obj = SA_Proxy._007E_008E___007F(propertyInfo, this, null);
						dictionary.Add(SA_Proxy._007E_0081___0018(dataMemberAttribute), ((obj != null) ? SA_Proxy._007E_007F___0018(obj) : null) ?? "");
					}
					num++;
				}
				stringBuilder = new StringBuilder();
			}
			if (dictionary.Any())
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					string text = SA_Helpers.Str(item.Value, SA_Proxy._0003___0003());
					if (0 == 0)
					{
						if (flag || 7 == 0)
						{
							SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._008F___0080(item.Key, "=", text));
							flag = false;
						}
						else
						{
							SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._0090___0081("&", item.Key, "=", text));
						}
					}
				}
			}
			return SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), SA_Proxy._007E_007F___0018(stringBuilder));
		}
	}
}
