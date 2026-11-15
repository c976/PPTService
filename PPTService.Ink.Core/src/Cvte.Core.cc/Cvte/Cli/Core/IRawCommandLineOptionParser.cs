using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli.Core
{
	
	internal interface IRawCommandLineOptionParser<out T>
	{
		void SetValue(char shortName, SingleOptimizedStrings values);

		
		void SetValue(string longName,  SingleOptimizedStrings values);
	}
}
