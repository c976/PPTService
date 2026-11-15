using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli
{
	
	public interface ICommandLineOptionParser< out T>
	{
		
		string Verb
		{
			
			get;
		}

		void SetValue(IReadOnlyList<string> values);

		void SetValue(char shortName, bool value);

		void SetValue(char shortName, string value);

		void SetValue(char shortName, IReadOnlyList<string> values);

		void SetValue(string longName, bool value);

		void SetValue(string longName, string value);

		void SetValue(string longName, IReadOnlyList<string> values);

		T Commit();
	}
}
