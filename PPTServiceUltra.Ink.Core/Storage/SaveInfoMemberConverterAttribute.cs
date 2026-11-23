using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	[AttributeUsage(AttributeTargets.Property)]
	public class SaveInfoMemberConverterAttribute : Attribute
	{
		public Type ConverterType { get; private set; }

		public SaveInfoMemberConverterAttribute(Type converterType)
		{
			ConverterType = converterType;
		}
	}
}
