using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class SaveInfoConverterAttribute : Attribute
	{
		public string[] Names { get; private set; }

		public SaveInfoConverterAttribute(params string[] names)
		{
			Names = names;
		}
	}
}
