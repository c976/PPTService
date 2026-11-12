using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PPTService.Core.Net.Http
{
	
	
	[DataContract]
	public abstract class JsonResponse : HttpResponse
	{
		protected JsonResponse()
		{
		}

		
		protected JsonResponse(HttpStatusCode statusCode, Exception error)
			: base(statusCode, error)
		{
		}

		protected override void ParseHtml(string html)
		{
			object obj = JsonConvert.DeserializeObject(html, GetType());
			if (obj == null)
			{
				throw new InvalidOperationException("response is null when parsing html! html string is " + html);
			}
			SelfAssignment(obj);
		}

		protected void SelfAssignment(object response)
		{
			PropertyInfo[] responseProps = response.GetType().GetProperties();
			(from p in GetType().GetProperties()
				where p.IsDefined(typeof(DataMemberAttribute), false)
				select p).ToList().ForEach( (PropertyInfo p) =>
			{
				string name = p.Name;
				foreach (PropertyInfo item in responseProps.Where( (PropertyInfo prop) => prop.Name == name))
				{
					object value = item.GetValue(response, null);
					try
					{
						p.SetValue(this, value, null);
					}
					catch (ArgumentException ex)
					{
						Console.WriteLine("[ParseHtmlError]" + ex.Message);
					}
				}
			});
		}
	}
}
