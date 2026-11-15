using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AspMvcViewLocationFormatAttribute : Attribute
	{
		public string Format { get; private set; }

		public AspMvcViewLocationFormatAttribute(string format)
		{
			Format = format;
		}
	}
}
