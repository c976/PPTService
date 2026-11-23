using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Configurations.Core;

namespace PPTServiceUltra.Ink.Core.Configurations
{
	
	
	public sealed class DefaultConfiguration : Configuration
	{
		private static readonly ConcurrentDictionary<string, WeakReference<DefaultConfiguration>> Configurations = new ConcurrentDictionary<string, WeakReference<DefaultConfiguration>>(StringComparer.OrdinalIgnoreCase);

		public ConfigurationString? this[string key]
		{
			get
			{
				return GetString(key);
			}
			set
			{
				SetValue(value, key);
			}
		}

		public DefaultConfiguration()
			: base(null)
		{
		}

		public static DefaultConfiguration FromFile(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException("文件名不能使用空字符串。", "fileName");
			}
			string fullPath = Path.GetFullPath(fileName);
			WeakReference<DefaultConfiguration> orAdd = Configurations.GetOrAdd(fullPath, CreateConfigurationReference);
			DefaultConfiguration target;
			if (!orAdd.TryGetTarget(out target))
			{
				lock (orAdd)
				{
					if (!orAdd.TryGetTarget(out target))
					{
						target = CreateConfiguration(fullPath);
						orAdd.SetTarget(target);
					}
				}
			}
			return target;
		}

		private static WeakReference<DefaultConfiguration> CreateConfigurationReference(string path)
		{
			return new WeakReference<DefaultConfiguration>(CreateConfiguration(path));
		}

		private static DefaultConfiguration CreateConfiguration(string path)
		{
			return ConfigurationFactory.FromFile(path).CreateAppConfigurator().Of<DefaultConfiguration>();
		}
	}
}
