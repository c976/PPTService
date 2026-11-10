using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class AccountRequestBody
	{
		public string GetSign()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				DataMemberAttribute dataMemberAttribute = PropertyAttributeHelper<DataMemberAttribute>.smethod_0(propertyInfo);
				if (dataMemberAttribute == null || dictionary.ContainsKey(dataMemberAttribute.Name))
				{
					continue;
				}
				object value = propertyInfo.GetValue(this, null);
				if (value is string)
				{
					propertyInfo.SetValue(this, SignatureUrlEncoder.smethod_0(value.ToString(), Encoding.UTF8), null);
				}
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
						goto IL_0085;
					}
				}
				obj = "";
				goto IL_0085;
				IL_0085:
				dictionary.Add(name, (string)obj);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (dictionary.Any())
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> item in dictionary.OrderBy((KeyValuePair<string, string> keyValuePair_0) => keyValuePair_0.Key))
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
			return Hmacsha256(stringBuilder.ToString(), "EN5_key");
		}

		public override string ToString()
		{
			return SignatureUrlEncoder.smethod_0(JsonConvert.SerializeObject((object)this), Encoding.UTF8);
		}

		private static string Hmacsha256(string value, string password)
		{
			using (HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(password)))
			{
				byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(value));
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				return stringBuilder.ToString();
			}
		}
	}
}
