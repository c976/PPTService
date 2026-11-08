using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using _0005;
using _0008;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class AccountRequestBody
	{
		public string GetSign()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			PropertyInfo[] array = SA_Proxy._007E_008D___001F(SA_Proxy._008C___001E(this));
			foreach (PropertyInfo propertyInfo in array)
			{
				DataMemberAttribute dataMemberAttribute = SA_Attr<DataMemberAttribute>._0001(propertyInfo, false);
				if (dataMemberAttribute != null && !dictionary.ContainsKey(SA_Proxy._007E_0081___0018(dataMemberAttribute)))
				{
					object obj = SA_Proxy._007E_008E___007F(propertyInfo, this, null);
					if (obj is string)
					{
						SA_Proxy._007E_0003_0002___008E(propertyInfo, this, SA_Helpers.Str(SA_Proxy._007E_007F___0018(obj), SA_Proxy._0003___0003()), null);
					}
					dictionary.Add(SA_Proxy._007E_0081___0018(dataMemberAttribute), ((obj != null) ? SA_Proxy._007E_007F___0018(obj) : null) ?? "");
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (dictionary.Any())
			{
				bool flag = true;
				IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.OrderBy((KeyValuePair<string, string> P_0) => P_0.Key).GetEnumerator();
				try
				{
					while (SA_Proxy._007E_0096___0083(enumerator))
					{
						KeyValuePair<string, string> current = enumerator.Current;
						string text = SA_Helpers.Str(current.Value, SA_Proxy._0003___0003());
						if (flag)
						{
							SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._008F___0080(current.Key, "=", text));
							flag = false;
						}
						else
						{
							SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._0090___0081("&", current.Key, "=", text));
						}
					}
				}
				finally
				{
					if (enumerator != null)
					{
						SA_Proxy._007E_0013___000F(enumerator);
					}
				}
			}
			return Hmacsha256(SA_Proxy._007E_007F___0018(stringBuilder), "EN5_key");
		}

		public override string ToString()
		{
			return SA_Helpers.Str(SA_Proxy._009E___008A(this), SA_Proxy._0003___0003());
		}

		private unsafe static string Hmacsha256(string value, string password)
		{
			HMACSHA256 hMACSHA = new HMACSHA256(SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), password));
			try
			{
				byte[] array = SA_Proxy._007E_0087___0019(hMACSHA, SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), value));
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array2 = array;
				int num = 0;
				while (true)
				{
					int num2 = num;
					while (true)
					{
						IntPtr intPtr = (IntPtr)(void*)array2.LongLength;
						int num3;
						while (true)
						{
							num3 = (int)(long)intPtr;
							if (5 == 0)
							{
								break;
							}
							intPtr = (IntPtr)num3;
							if (false)
							{
								continue;
							}
							goto IL_007b;
						}
						goto IL_006c;
						IL_0068:
						if (7 == 0)
						{
							continue;
						}
						num3 = 1;
						goto IL_006c;
						IL_007b:
						if (num2 < num3)
						{
							byte b = array2[num];
							SA_Proxy._007E_0088___001A(stringBuilder, b.ToString("x2"));
							num2 = num;
							goto IL_0068;
						}
						return SA_Proxy._007E_007F___0018(stringBuilder);
						IL_006c:
						num2 += num3;
						if (0 == 0)
						{
							break;
						}
						goto IL_0068;
					}
					num = num2;
				}
			}
			finally
			{
				if (hMACSHA != null)
				{
					SA_Proxy._007E_0013___000F(hMACSHA);
				}
			}
		}
	}
}
