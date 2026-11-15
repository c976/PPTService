using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using PPTService.Ink.Core.Cache;
using PPTService.Ink.Core.Storage.SaveInfoManagerInjections;
using PPTService.Ink.Core.Threading;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class SaveInfoManager : ISaveInfoManager
	{
		private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(true);

		private readonly ConcurrentDictionary<Assembly, Assembly> _loadedAssemblies = new ConcurrentDictionary<Assembly, Assembly>();

		private readonly ConcurrentDictionary<string, Func<SaveInfo>> _creatorDictionary = new ConcurrentDictionary<string, Func<SaveInfo>>(StringComparer.Ordinal);

		private volatile int _loadingCount;

		private readonly CachePool<Type, Func<SaveInfo>> _constructorCache = new CachePool<Type, Func<SaveInfo>>((Type x) => Expression.Lambda<Func<SaveInfo>>(Expression.New(x), new ParameterExpression[0]).Compile(), true);

		public SaveInfoManager()
		{
			Assembly assembly = typeof(SaveInfoManager).Assembly;
			_loadedAssemblies.TryAdd(assembly, assembly);
			AssemblyStorageGeneratedSaveInfoProvider.AddStorageAssemblySaveInfo(this);
		}

		public bool TryGet(string typeName, [NotNullWhen(true)] out SaveInfo saveInfo)
		{
			_resetEvent.Wait();
			Func<SaveInfo> value;
			if (_creatorDictionary.TryGetValue(typeName, out value))
			{
				saveInfo = value();
				return true;
			}
			saveInfo = null;
			return false;
		}

		public async void Load(params Assembly[] assemblies)
		{
			_resetEvent.Reset();
			Interlocked.Increment(ref _loadingCount);
			try
			{
				Task[] tasks = new Task[assemblies.Length];
				for (int i = 0; i < assemblies.Length; i++)
				{
					int index = i;
					Assembly assembly = assemblies[index];
					_loadedAssemblies.AddOrUpdate(assembly, (Assembly key) =>
					{
						tasks[index] = Task.Run(() =>
						{
							Load(key.GetTypes());
						});
						return assembly;
					}, (Assembly key, Assembly existed) =>
					{
						tasks[index] = CompletedTask.Value;
						return assembly;
					});
				}
				await Task.WhenAll(tasks).ConfigureAwait(false);
			}
			finally
			{
				if (Interlocked.Decrement(ref _loadingCount) == 0)
				{
					_resetEvent.Set();
				}
			}
		}

		private void Load(IEnumerable<Type> types)
		{
			foreach (Type type in types)
			{
				TryRegisterType(type);
			}
		}

		public bool TryRegisterType(Type type)
		{
			if (!type.IsDefined(typeof(SaveInfoContractAttribute), false))
			{
				return false;
			}
			SaveInfoContractAttribute customAttribute = type.GetCustomAttribute<SaveInfoContractAttribute>(false);
			if (string.IsNullOrEmpty(customAttribute.Name))
			{
				return false;
			}
			RegisterSaveInfoCreator(type, customAttribute);
			return true;
		}

		public void RegisterSaveInfoCreator(Type saveInfoType, SaveInfoContractAttribute attribute,  Func<SaveInfo> creator = null)
		{
			string name = attribute.Name;
			if (creator == null)
			{
				creator = BuildCreator(saveInfoType);
			}
			if (attribute.Overwrite)
			{
				_creatorDictionary.AddOrUpdate(name, (string key) => creator, (string key, Func<SaveInfo> existed) => creator);
			}
			else
			{
				_creatorDictionary.GetOrAdd(name, creator);
				Func<SaveInfo> func = creator;
			}
		}

		public void RegisterAssemblySaveInfoCreators([In] ref ManualRegisterAssemblySaveInfoCreatorsInfo creatorsInfo)
		{
			_loadedAssemblies.TryAdd(creatorsInfo.RegisterAssembly, creatorsInfo.RegisterAssembly);
			foreach (SaveInfoCreatorInfo saveInfoCreatorInfo in creatorsInfo.SaveInfoCreatorInfoList)
			{
				RegisterSaveInfoCreator(saveInfoCreatorInfo.SaveInfoType, saveInfoCreatorInfo.SaveInfoContractAttribute, saveInfoCreatorInfo.Creator);
			}
		}

		internal void RegisterSaveInfoCreator(string attributeName, Func<SaveInfo> creator)
		{
			_creatorDictionary.TryAdd(attributeName, creator);
		}

		private Func<SaveInfo> BuildCreator(Type type)
		{
			return () => _constructorCache[type]();
		}
	}
}
