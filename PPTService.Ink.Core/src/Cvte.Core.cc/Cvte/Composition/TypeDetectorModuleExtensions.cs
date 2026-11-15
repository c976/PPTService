using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Composition
{
	
	
	[Obsolete("TypeDetector 使用传统的反射实现，性能较低。建议使用 dotnetCampus.Telescope 替代。")]
	public static class TypeDetectorModuleExtensions
	{
		
		private class TypeDetectorData
		{
			public List<IModule> PendingModules { get; private set; }

			public HashSet<string> InitializedModuleNames { get; private set; }

			public TypeDetectorData()
			{
				PendingModules = new List<IModule>();
				InitializedModuleNames = new HashSet<string>();
			}
		}

		private static readonly HashSet<ITypeDetector> Detectors = new HashSet<ITypeDetector>();

		[Obsolete("TypeDetector 使用传统的反射实现，性能较低。建议使用 dotnetCampus.Telescope 替代。")]
		public static void StartWatchingOnNewModules(this ITypeDetector typeDetector)
		{
			typeDetector.StartWatchingOnNewModules((Type t) => true);
		}

		[Obsolete("TypeDetector 使用传统的反射实现，性能较低。建议使用 dotnetCampus.Telescope 替代。")]
		public static void StartWatchingOnNewModules(this ITypeDetector typeDetector, Predicate<Type> typeFilter)
		{
			if (Detectors.Contains(typeDetector))
			{
				throw new InvalidOperationException("此TypeDetector已经在检测IModule，不可重复调用此方法。");
			}
			TypeDetectorData data = new TypeDetectorData();
			Detectors.Add(typeDetector);
			typeDetector.OnTypeDetected((IModule module) =>
			{
				InitializeModule(module, data);
			}, typeFilter, true);
		}

		[Obsolete("请使用 StartWatchingOnNewModules 替代。")]
		public static void DetectModules(this ITypeDetector typeDetector)
		{
			typeDetector.StartWatchingOnNewModules();
		}

		[Obsolete("请使用 StartWatchingOnNewModules 替代。")]
		public static void DetectModules(this ITypeDetector typeDetector, Predicate<Type> typeFilter)
		{
			typeDetector.StartWatchingOnNewModules(typeFilter);
		}

		private static void InitializeModule(IModule module, TypeDetectorData data)
		{
			if (!data.PendingModules.Contains(module))
			{
				data.PendingModules.Add(module);
			}
			ModulesInitializationException.Builder builder = new ModulesInitializationException.Builder();
			bool flag = true;
			while (flag)
			{
				flag = false;
				IModule[] array = data.PendingModules.ToArray();
				foreach (IModule module2 in array)
				{
					if (CanInitialize(module2, data.InitializedModuleNames))
					{
						try
						{
							module2.Initialize();
						}
						catch (Exception exception)
						{
							builder.Append(module, exception);
						}
						data.InitializedModuleNames.Add(module2.GetType().FullName);
						data.PendingModules.Remove(module2);
						flag = true;
					}
				}
			}
			builder.ThrowsIfExceptionsAny("Modules initializing failed. See Exceptions property for more help.");
		}

		private static bool CanInitialize(IModule module, HashSet<string> initializedModuleNames)
		{
			ModuleAttribute moduleAttribute = module.GetType().GetCustomAttributes(typeof(ModuleAttribute), true).FirstOrDefault() as ModuleAttribute;
			if (moduleAttribute == null)
			{
				return true;
			}
			string[] dependsOn = moduleAttribute.DependsOn;
			if (dependsOn == null || !dependsOn.Any())
			{
				return true;
			}
			return dependsOn.All(initializedModuleNames.Contains);
		}
	}
}
