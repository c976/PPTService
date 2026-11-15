using System;

namespace PPTService.Ink.Core.Windows
{
	public class OtherInstanceStartedUpEventArgs : EventArgs
	{
		public string[] CommandLineArgs { get; private set; }

		public OtherInstanceStartedUpEventArgs(string[] commandLineArgs)
		{
			CommandLineArgs = commandLineArgs;
		}
	}
}
