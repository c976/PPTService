using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Cvte.Net.Http
{
	
	
	public static class HttpRequestExtensions
	{
		private const string HttpPrefix = "http://";

		private const string HttpsPrefix = "https://";

		public static string GetRequestUrl(this HttpRequest request)
		{
			string text = ((!string.IsNullOrEmpty(request.Url)) ? request.Url : (request.GetType().GetCustomAttributes<RequestAttribute>().FirstOrDefault()?.Api ?? ""));
			if (request is RestfulRequest)
			{
				foreach (PropertyInfo item in from m in request.GetType().GetProperties()
					where m.IsDefined(typeof(RestfulMemberAttribute))
					select m)
				{
					RestfulMemberAttribute restfulMemberAttribute = (RestfulMemberAttribute)item.GetCustomAttributes(typeof(RestfulMemberAttribute)).First();
					string newValue = item.GetValue(request, null)?.ToString() ?? "";
					text = text.Replace("{" + restfulMemberAttribute.Name + "}", newValue);
				}
			}
			else if (request.Method == null || request.Method.ToUpper() == "GET")
			{
				string queryString = request.GetQueryString();
				if (!string.IsNullOrEmpty(queryString))
				{
					text = text + "?" + queryString;
				}
			}
			if (!text.StartsWith("http://") && !text.StartsWith("https://") && !string.IsNullOrEmpty(request.Host))
			{
				text = request.Host + text;
			}
			if (!text.StartsWith("http://") && !text.StartsWith("https://"))
			{
				text = "http://" + text;
			}
			request.RuntimeUrl = text;
			return text;
		}

		
		public static IDictionary<string, string> GetParams(this HttpRequest request)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (PropertyInfo item in request.GetType().GetProperties().ToList())
			{
				DataMemberAttribute dataMemberAttribute = item.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
				if (dataMemberAttribute != null)
				{
					object value = item.GetValue(request, null);
					if (item.GetCustomAttribute<IgnoreMemberWhenNullAttribute>() != null && value != null)
					{
						dictionary[dataMemberAttribute.Name] = value.ToString();
					}
					else
					{
						dictionary[dataMemberAttribute.Name] = value?.ToString();
					}
				}
			}
			return dictionary;
		}

		internal static string BuildQueryString(this IDictionary<string, string> urlParams, bool encodeParamValue = false)
		{
			return urlParams.Aggregate("",  (string current, KeyValuePair<string, string> next) =>
			{
				string key = next.Key;
				string text = next.Value ?? "";
				if (encodeParamValue)
				{
					text = UrlEncodeUtility.JavaUrlEncode(text, Encoding.UTF8);
				}
				return string.IsNullOrEmpty(current) ? (current + key + "=" + text) : (current + "&" + key + "=" + text);
			});
		}
	}
}
