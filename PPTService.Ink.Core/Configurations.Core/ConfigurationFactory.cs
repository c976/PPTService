using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	
	public static class ConfigurationFactory
	{
		private static readonly ConcurrentDictionary<string, WeakReference<FileConfigurationRepo>> Configurations = new ConcurrentDictionary<string, WeakReference<FileConfigurationRepo>>();

		public static FileConfigurationRepo FromFile(string fileName)
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
			WeakReference<FileConfigurationRepo> orAdd = Configurations.GetOrAdd(fullPath, CreateConfigurationReference);
			FileConfigurationRepo target;
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

		private static WeakReference<FileConfigurationRepo> CreateConfigurationReference(string path)
		{
			return new WeakReference<FileConfigurationRepo>(CreateConfiguration(path));
		}

		private static FileConfigurationRepo CreateConfiguration(string path)
		{
			return new FileConfigurationRepo(path);
		}

		public static Task ReloadExternalChangesAsync(this IAppConfigurator configs)
		{
			if (configs == null)
			{
				throw new ArgumentNullException("configs");
			}
			FileConfigurationRepo fileConfigurationRepo = configs.Of<DefaultConfiguration>().Repo as FileConfigurationRepo;
			if (fileConfigurationRepo != null)
			{
				return fileConfigurationRepo.ReloadExternalChangesAsync();
			}
			return Task.FromResult<object>(null);
		}
	}
}
