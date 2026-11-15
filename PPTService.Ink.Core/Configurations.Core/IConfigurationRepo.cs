using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	public interface IConfigurationRepo
	{
		IAppConfigurator CreateAppConfigurator();

		
		string GetValue(string key);

		void SetValue(string key,  string value);

		void ClearValues(Predicate<string> keyFilter);
	}
}
