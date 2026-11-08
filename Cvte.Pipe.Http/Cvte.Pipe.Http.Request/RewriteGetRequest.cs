using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
				if (!(propertyInfo == null) && SA_Attr<GetRequestParameterAttribute>.Has(propertyInfo, false))
				{
					GetRequestParameterAttribute getRequestParameterAttribute = SA_Attr<GetRequestParameterAttribute>.Get(propertyInfo, false);
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
			return (from keyValuePair in dictionary
				orderby keyValuePair.Value
				select keyValuePair.Key).Aggregate(requestUrl, (string text, object obj2) => text + "/" + obj2);
		}

		public override byte[] GetRequestData()
		{
			return new byte[0];
		}
	}
}
