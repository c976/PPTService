using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

		private sealed class _003C_003Ec__DisplayClass1_0
		{
			public TypeDetectorData data;

			internal void _003CDetectModules_003Eb__0(IModule module)
			{
				InitializeModule(module, data);
			}
		}

		private static readonly HashSet<ITypeDetector> Detectors = new HashSet<ITypeDetector>();

		public static void DetectModules(this ITypeDetector typeDetector)
		{
			do
			{
				if (0 == 0)
				{
					typeDetector.DetectModules((Type t) => true);
				}
			}
			while (false);
		}

		public static void DetectModules(this ITypeDetector typeDetector, Predicate<Type> typeFilter)
		{
			if (2 == 0)
			{
				return;
			}
			_003C_003Ec__DisplayClass1_0 obj2 = default(_003C_003Ec__DisplayClass1_0);
			if (0 == 0)
			{
				_003C_003Ec__DisplayClass1_0 obj = new _003C_003Ec__DisplayClass1_0();
				if (0 == 0)
				{
					obj2 = obj;
				}
				if (false)
				{
					return;
				}
				if (Detectors.Contains(typeDetector))
				{
					if (6 != 0)
					{
						throw new InvalidOperationException("此TypeDetector已经在检测IModule，不可重复调用此方法。");
					}
					return;
				}
				do
				{
					obj2.data = new TypeDetectorData();
				}
				while (false);
				Detectors.Add(typeDetector);
			}
			typeDetector.OnTypeDetected<IModule>(obj2._003CDetectModules_003Eb__0, typeFilter, true);
		}

		private unsafe static void InitializeModule(IModule module, TypeDetectorData data)
		{
			void* ptr = stackalloc byte[5];
			if (!data.PendingModules.Contains(module))
			{
				data.PendingModules.Add(module);
			}
			((sbyte*)ptr)[4] = 1;
			while (true)
			{
				if (((bool*)ptr)[4])
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
				else if (3 != 0)
				{
					break;
				}
			}
		}

		private static bool CanInitialize(IModule module, HashSet<string> initializedModuleNames)
		{
			ModuleAttribute obj = module.GetType().GetCustomAttributes(typeof(ModuleAttribute), true).FirstOrDefault() as ModuleAttribute;
			ModuleAttribute moduleAttribute = default(ModuleAttribute);
			if (0 == 0)
			{
				moduleAttribute = obj;
			}
			string[] dependsOn = default(string[]);
			if (moduleAttribute == null)
			{
				if (8 != 0)
				{
					return true;
				}
			}
			else
			{
				dependsOn = moduleAttribute.DependsOn;
				if (dependsOn == null)
				{
					goto IL_0041;
				}
			}
			bool num = dependsOn.Any();
			if (false)
			{
				goto IL_0042;
			}
			if (!num)
			{
				goto IL_0041;
			}
			bool result = dependsOn.All(initializedModuleNames.Contains);
			goto IL_0059;
			IL_0041:
			goto IL_0045;
			IL_0045:
			return true;
			IL_0042:
			int num2;
			if (num2 != 0)
			{
				goto IL_0045;
			}
			goto IL_0059;
			IL_0059:
			if (0 == 0)
			{
				return result;
			}
			goto IL_0042;
		}
	}
}
