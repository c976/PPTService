using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AspMvcAreaMasterLocationFormatAttribute : Attribute
	{
		public string Format { get; private set; }

		public AspMvcAreaMasterLocationFormatAttribute(string format)
		{
			Format = format;
		}
	}
}
