using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Core
{
	
	
	public abstract class ConfigurationRepo : IConfigurationRepo
	{
		public IAppConfigurator CreateAppConfigurator()
		{
			return new ConcurrentAppConfigurator(this);
		}

		
		public abstract string GetValue(string key);

		public abstract void SetValue(string key,  string value);

		public abstract void ClearValues(Predicate<string> keyFilter);
	}
}
