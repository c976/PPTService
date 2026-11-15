using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	
	public class MemoryConfigurationRepo : AsynchronousConfigurationRepo
	{
		private readonly ConcurrentDictionary<string, string> _memoryConfiguration;

		private Task CompleteTask { get; } = Task.FromResult(true);

		public MemoryConfigurationRepo()
		{
			_memoryConfiguration = new ConcurrentDictionary<string, string>();
		}

		public MemoryConfigurationRepo( IEnumerable<KeyValuePair<string, string>> initData)
		{
			_memoryConfiguration = new ConcurrentDictionary<string, string>(initData);
		}

		public MemoryConfigurationRepo(ConcurrentDictionary<string, string> memoryStorageDictionary)
		{
			_memoryConfiguration = memoryStorageDictionary;
		}

		public ConcurrentDictionary<string, string> GetMemoryStorageDictionary()
		{
			return _memoryConfiguration;
		}

		protected override ICollection<string> GetKeys()
		{
			return _memoryConfiguration.Keys;
		}

		
		protected override Task<string> ReadValueCoreAsync(string key)
		{
			string value;
			if (_memoryConfiguration.TryGetValue(key, out value))
			{
				return Task.FromResult(value);
			}
			return Task.FromResult<string>(null);
		}

		protected override Task WriteValueCoreAsync(string key, string value)
		{
			_memoryConfiguration.AddOrUpdate(key, (string _) => value, (string _, string __) => value);
			return CompleteTask;
		}

		protected override Task RemoveValueCoreAsync(string key)
		{
			string value;
			_memoryConfiguration.TryRemove(key, out value);
			return CompleteTask;
		}

		protected override void OnChanged(AsynchronousConfigurationChangeContext context)
		{
		}
	}
}
