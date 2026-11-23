using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AspMvcMasterLocationFormatAttribute : Attribute
	{
		public string Format { get; private set; }

		public AspMvcMasterLocationFormatAttribute(string format)
		{
			Format = format;
		}
	}
}
