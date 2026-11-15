using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations
{
	
	public interface IAppConfigurator
	{
		DefaultConfiguration Default { get; }

		TConfiguration Of< TConfiguration>() where TConfiguration : Configuration, new();
	}
}
