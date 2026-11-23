using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Core
{
	
	
	internal sealed class ConcurrentAppConfigurator : IAppConfigurator
	{
		private readonly object _locker = new object();

		private readonly IConfigurationRepo _repo;

		private readonly Dictionary<Type, Configuration> _configurationDictionary = new Dictionary<Type, Configuration>();

		public DefaultConfiguration Default => Of<DefaultConfiguration>();

		internal ConcurrentAppConfigurator(IConfigurationRepo repo)
		{
			if (repo == null)
			{
				throw new ArgumentNullException("repo");
			}
			_repo = repo;
		}

		public TConfiguration Of< TConfiguration>() where TConfiguration : Configuration, new()
		{
			Configuration value;
			if (_configurationDictionary.TryGetValue(typeof(TConfiguration), out value))
			{
				return (TConfiguration)value;
			}
			lock (_locker)
			{
				Configuration value2;
				if (_configurationDictionary.TryGetValue(typeof(TConfiguration), out value2))
				{
					return (TConfiguration)value2;
				}
				value = new TConfiguration
				{
					Repo = _repo
				};
				_configurationDictionary[typeof(TConfiguration)] = value;
				return (TConfiguration)value;
			}
		}
	}
}
