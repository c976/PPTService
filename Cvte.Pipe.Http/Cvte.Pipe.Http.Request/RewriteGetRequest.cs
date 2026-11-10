using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cvte.Pipe.Http.Internal;

namespace Cvte.Pipe.Http.Request
{
	public class RewriteGetRequest : RequestBase
	{
		public override string GetRequestUrl()
		{
			Dictionary<object, int> dictionary = new Dictionary<object, int>();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!(propertyInfo == null) && AttributeHelper<GetRequestParameterAttribute>.smethod_2(propertyInfo))
				{
					GetRequestParameterAttribute getRequestParameterAttribute = AttributeHelper<GetRequestParameterAttribute>.smethod_5(propertyInfo);
					int order = getRequestParameterAttribute.Order;
					object obj = propertyInfo.GetValue(this, null);
					if (!string.IsNullOrEmpty(getRequestParameterAttribute.Format))
					{
						obj = string.Format(getRequestParameterAttribute.Format, obj);
					}
					dictionary.Add(obj, order);
				}
			}
			string requestUrl = base.GetRequestUrl();
			return (from keyValuePair_0 in dictionary
				orderby keyValuePair_0.Value
				select keyValuePair_0.Key).Aggregate(requestUrl, (string string_0, object object_0) => string_0 + "/" + object_0);
		}

		public override byte[] GetRequestData()
		{
			return new byte[0];
		}
	}
}
