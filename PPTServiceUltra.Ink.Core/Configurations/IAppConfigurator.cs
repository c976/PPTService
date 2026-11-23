using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations
{
	
	public interface IAppConfigurator
	{
		DefaultConfiguration Default { get; }

		TConfiguration Of< TConfiguration>() where TConfiguration : Configuration, new();
	}
}
