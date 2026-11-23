namespace PPTServiceUltra.Ink.Core.Cli.Standard
{
	internal enum CommandLineArgsSource
	{
		Unknown = -1,
		MainArgs = 0,
		EnvironmentGetCommandLineArgs = 1,
		Native = 2,
		WithoutEntryPointName = MainArgs,
		WithEntryPointName = EnvironmentGetCommandLineArgs,
		WithLauncherAndExecutableName = Native,
		FromOtherProcessWithoutProcessName = 17,
		FromOtherProcess = 18
	}
}
