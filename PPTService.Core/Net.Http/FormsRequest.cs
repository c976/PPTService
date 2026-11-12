using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace PPTService.Core.Net.Http
{
	
	
	[DataContract]
	public abstract class FormsRequest : HttpRequest
	{
		public override string ContentType => "application/x-www-form-urlencoded;charset=utf-8";

		protected FormsRequest()
		{
			Method = "POST";
		}

		public override byte[] GetRequestData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				DataMemberAttribute dataMemberAttribute = propertyInfo.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
				if (dataMemberAttribute != null)
				{
					string name = dataMemberAttribute.Name;
					object value = propertyInfo.GetValue(this, null);
					stringBuilder.AppendFormat((stringBuilder.Length == 0) ? "{0}={1}" : "&{0}={1}", name, value);
				}
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}
	}
}
