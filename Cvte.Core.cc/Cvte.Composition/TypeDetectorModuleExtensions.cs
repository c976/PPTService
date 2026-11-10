using System;
using System.Collections.Generic;
using System.Linq;

namespace Cvte.Composition
{
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

		public static void DetectModules(this ITypeDetector typeDetector)
		{
			typeDetector.DetectModules((Type t) => true);
		}

		public static void DetectModules(this ITypeDetector typeDetector, Predicate<Type> typeFilter)
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

		private unsafe static void InitializeModule(IModule module, TypeDetectorData data)
		{
			void* ptr = stackalloc byte[5];
			if (!data.PendingModules.Contains(module))
			{
				data.PendingModules.Add(module);
			}
			((sbyte*)ptr)[4] = 1;
			while (((sbyte*)ptr)[4] != 0)
			{
				((sbyte*)ptr)[4] = 0;
				IModule[] array = data.PendingModules.ToArray();
				*(int*)ptr = 0;
				while (*(int*)ptr < array.Length)
				{
					IModule module2 = array[*(int*)ptr];
					if (CanInitialize(module2, data.InitializedModuleNames))
					{
						module2.Initialize();
						data.InitializedModuleNames.Add(module2.GetType().FullName);
						data.PendingModules.Remove(module2);
						((sbyte*)ptr)[4] = 1;
					}
					(*(int*)ptr)++;
				}
			}
		}

		private static bool CanInitialize(IModule module, HashSet<string> initializedModuleNames)
		{
			ModuleAttribute moduleAttribute = module.GetType().GetCustomAttributes(typeof(ModuleAttribute), true).FirstOrDefault() as ModuleAttribute;
			if (moduleAttribute == null)
			{
				return true;
			}
			string[] dependsOn = moduleAttribute.DependsOn;
			if (dependsOn != null && dependsOn.Any())
			{
				return dependsOn.All(initializedModuleNames.Contains);
			}
			return true;
		}
	}
}
