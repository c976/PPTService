using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
	public sealed class MacroAttribute : Attribute
	{
		public string Expression { get; set; }

		public int Editable { get; set; }

		public string Target { get; set; }
	}
}
