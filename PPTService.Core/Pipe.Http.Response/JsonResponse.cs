using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using PPTService.Core.Pipe.Http.Internal;

namespace PPTService.Core.Pipe.Http.Response
{
	[DataContract]
	public abstract class JsonResponse : ResponseBase
	{
		public override void ParseHtml(string html)
		{
			if (string.IsNullOrEmpty(html))
			{
				return;
			}
			Html = html;
			byte[] bytes = Encoding.UTF8.GetBytes(html);
			object response = DataContractJsonHelper.smethod_6(GetType(), bytes);
			IEnumerable<PropertyInfo> responseProps = from p in response.GetType().GetProperties()
				where AttributeHelper<DataMemberAttribute>.smethod_2(p)
				select p;
			GetType().GetProperties().ToList().ForEach((PropertyInfo p) =>
			{
				foreach (PropertyInfo item in responseProps.Where((PropertyInfo prop) => p.Name == prop.Name))
				{
					try
					{
						p.SetValue(this, item.GetValue(response, null), null);
					}
					catch (ArgumentException)
					{
					}
				}
			});
		}

		public static TResponse FromHtml<TResponse>(string html) where TResponse : ResponseBase, new()
		{
			byte[] bytes = Encoding.UTF8.GetBytes(html);
			return DataContractJsonHelper.smethod_6(typeof(TResponse), bytes) as TResponse;
		}
	}
}
