using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli.Core
{
	
	
	internal class SingleOptimizedStrings : SingleOptimizedList<string>
	{
		public SingleOptimizedStrings(string firstValue)
			: base(firstValue)
		{
		}
	}
}
