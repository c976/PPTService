using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Composition
{
	
	
	public class ExportNotFoundException : Exception
	{
		public Type Type { get; }

		public ExportNotFoundException(Type type)
			: base($"{type} is not found.")
		{
			Type = type;
		}
	}
}
