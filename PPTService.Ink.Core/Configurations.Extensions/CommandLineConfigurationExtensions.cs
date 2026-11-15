using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Extensions
{
	
	
	public static class CommandLineConfigurationExtensions
	{
		public static IAppConfigurator AddCommandLine(this IAppConfigurator appConfigurator, string[] args)
		{
			return appConfigurator.AddCommandLine(args, null);
		}

		public static IAppConfigurator AddCommandLine(this IAppConfigurator appConfigurator, string[] args,  IDictionary<string, string> switchMappings)
		{
			foreach (KeyValuePair<string, string> item in new CommandLineConfigurationProvider(args, switchMappings).Load())
			{
				appConfigurator.Default.SetValue(item.Key, item.Value);
			}
			return appConfigurator;
		}
	}
}
