using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AspMvcAreaViewLocationFormatAttribute : Attribute
	{
		public string Format { get; private set; }

		public AspMvcAreaViewLocationFormatAttribute(string format)
		{
			Format = format;
		}
	}
}
