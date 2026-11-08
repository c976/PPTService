using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public class CStoreUploadFileRequest : UploadFileRequest
	{
		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "x:md5")]
		public string Hash { get; set; } = string.Empty;

		[DataMember(Name = "x:type")]
		public int Type { get; set; }

		public bool IsIgnoreHashTypeField { get; set; }

		public IList<FieldEntry> FormFields { get; set; } = new List<FieldEntry>();

		public IList<FieldEntry> HeaderFields { get; set; } = new List<FieldEntry>();

		public virtual IDictionary<string, string> GetParams()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerable<PropertyInfo> enumerable = from p in GetType().GetProperties().ToList()
				where AttributeUtility<DataMemberAttribute>.IsDefined(p)
				select p;
			foreach (FieldEntry formField in FormFields)
			{
				if (!dictionary.ContainsKey(formField.Key))
				{
					dictionary.Add(formField.Key, formField.Value);
				}
			}
			foreach (PropertyInfo item in enumerable)
			{
				string name = AttributeUtility<DataMemberAttribute>.GetFirstCustomAttribute(item).Name;
				if (!dictionary.ContainsKey(name) && (!IsIgnoreHashTypeField || (!string.Equals(name, "x:md5", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "x:type", StringComparison.OrdinalIgnoreCase))))
				{
					string value = item.GetValue(this, null).ToString();
					dictionary.Add(name, value);
				}
			}
			return dictionary;
		}

		public override IDictionary<string, string> GetFormDataParams()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerable<PropertyInfo> enumerable = from p in GetType().GetProperties().ToList()
				where AttributeUtility<DataMemberAttribute>.IsDefined(p)
				select p;
			foreach (FieldEntry formField in FormFields)
			{
				if (!dictionary.ContainsKey(formField.Key))
				{
					dictionary.Add(formField.Key, formField.Value);
				}
			}
			foreach (PropertyInfo item in enumerable)
			{
				string name = AttributeUtility<DataMemberAttribute>.GetFirstCustomAttribute(item).Name;
				if (!dictionary.ContainsKey(name) && (!IsIgnoreHashTypeField || (!string.Equals(name, "x:md5", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "x:type", StringComparison.OrdinalIgnoreCase))))
				{
					string value = item.GetValue(this, null).ToString();
					dictionary.Add(name, value);
				}
			}
			if (IsIgnoreHashTypeField)
			{
				dictionary.Remove("x:md5");
				dictionary.Remove("x:type");
			}
			return dictionary;
		}
	}
}
