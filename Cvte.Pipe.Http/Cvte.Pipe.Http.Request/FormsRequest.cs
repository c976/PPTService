using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Pipe.Http.Internal;

namespace Cvte.Pipe.Http.Request
{
	public abstract class FormsRequest : RequestBase
	{
		public override byte[] GetRequestData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				DataMemberAttribute dataMemberAttribute = AttributeHelper<DataMemberAttribute>.smethod_5(propertyInfo);
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
