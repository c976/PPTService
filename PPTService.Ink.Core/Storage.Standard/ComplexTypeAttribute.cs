using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class ComplexTypeAttribute : Attribute
	{
		public Type Belong { get; private set; }

		public string Key { get; private set; }

		public ComplexTypeAttribute(Type type, string key)
		{
			Belong = type;
			Key = key;
		}
	}
}
