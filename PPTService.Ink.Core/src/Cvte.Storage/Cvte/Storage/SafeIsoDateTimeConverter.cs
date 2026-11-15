using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PPTService.Ink.Core.Storage
{
	internal class SafeIsoDateTimeConverter : IsoDateTimeConverter
	{
		
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			try
			{
				return base.ReadJson(reader, objectType, existingValue, serializer);
			}
			catch (FormatException)
			{
				return DateTime.Now;
			}
		}
	}
}
