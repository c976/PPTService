using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Utils
{
	internal static class CT
	{
		
		[Conditional("UNITTEST")]
		internal static void Log(string message, params string[] tags)
		{
			Console.WriteLine(string.Format("[{0:O}] {1} {2}", DateTime.Now, string.Join(" ", tags.Select((string x) => "[" + x + "]")), message));
		}
	}
}
