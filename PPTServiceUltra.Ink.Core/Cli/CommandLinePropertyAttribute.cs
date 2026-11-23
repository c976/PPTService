using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli
{
	
	
	public abstract class CommandLinePropertyAttribute : CommandLineAttribute
	{
		internal string TypeName { get; set; }

		internal CommandLinePropertyAttribute()
		{
		}
	}
}
