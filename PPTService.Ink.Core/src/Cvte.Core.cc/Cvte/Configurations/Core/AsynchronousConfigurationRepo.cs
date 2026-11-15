using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	
	public abstract class AsynchronousConfigurationRepo : IConfigurationRepo
	{
		
		protected sealed class AsynchronousConfigurationChangeContext
		{
			
			private Task _trackedAction;

			public IReadOnlyCollection<string> ChangedKeys { get; }

			internal AsynchronousConfigurationChangeContext(IEnumerable<string> changedKeys)
			{
				IEnumerable<string> enumerable = changedKeys;
				if (enumerable == null)
				{
					throw new ArgumentNullException("changedKeys");
				}
				changedKeys = enumerable;
				IReadOnlyCollection<string> readOnlyCollection = changedKeys as IReadOnlyCollection<string>;
				if (readOnlyCollection != null)
				{
					ChangedKeys = readOnlyCollection;
				}
				else
				{
					ChangedKeys = changedKeys.ToArray();
				}
			}

			public void TrackAsyncAction(Task action)
			{
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				_trackedAction = action;
			}

			
			public Task GetTrackedAction()
			{
				return _trackedAction;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass3_0
		{
			
			public ICollection<string> keys;

			
			public Predicate<string> keyFilter;

			
			public AsynchronousConfigurationRepo _003C_003E4__this;

			internal async Task _003CCvte_002EConfigurations_002ECore_002EIConfigurationRepo_002EClearValues_003Eg__RemoveKeys_007C1()
			{
				bool isChanged = false;
				foreach (string item in keys.Where(keyFilter.Invoke))
				{
					isChanged = true;
					await _003C_003E4__this.RemoveValueCoreAsync(item).ConfigureAwait(false);
				}
				if (isChanged)
				{
					_003C_003E4__this.NotifyChanged(keys);
				}
			}
		}

		public IAppConfigurator CreateAppConfigurator()
		{
			return new ConcurrentAppConfigurator(this);
		}

		
		string IConfigurationRepo.GetValue(string key)
		{
			VerifyKey(key);
			return TryReadAsync(key).Result;
		}

		void IConfigurationRepo.SetValue(string key,  string value)
		{
			VerifyKey(key, true);
			WriteAsync(key, value).Wait();
		}

		void IConfigurationRepo.ClearValues(Predicate<string> keyFilter)
		{
			_003C_003Ec__DisplayClass3_0 obj = new _003C_003Ec__DisplayClass3_0();
			obj.keyFilter = keyFilter;
			obj._003C_003E4__this = this;
			obj.keyFilter = obj.keyFilter ?? ((Predicate<string>)((string _) => true));
			obj.keys = GetKeys();
			if (obj.keys == null)
			{
				throw new InvalidOperationException("重写 GetKeys 方法时，不应该返回 null，而应该返回空集合。");
			}
			obj._003CCvte_002EConfigurations_002ECore_002EIConfigurationRepo_002EClearValues_003Eg__RemoveKeys_007C1().Wait();
		}

		public async Task<string> TryReadAsync(string key, string @default = "")
		{
			if (@default == null)
			{
				throw new ArgumentNullException("default");
			}
			VerifyKey(key);
			return (await ReadValueCoreAsync(key).ConfigureAwait(false)) ?? @default;
		}

		public async Task WriteAsync(string key,  string value)
		{
			VerifyKey(key, true);
			if (!(await TryReadAsync(key).ConfigureAwait(false)).Equals(value, StringComparison.InvariantCulture))
			{
				if (value != null && !string.IsNullOrEmpty(value))
				{
					await WriteValueCoreAsync(key, value).ConfigureAwait(false);
				}
				else
				{
					await RemoveValueCoreAsync(key).ConfigureAwait(false);
				}
				NotifyChanged(new string[1] { key });
			}
		}

		private async Task NotifyChanged(IEnumerable<string> keys)
		{
			AsynchronousConfigurationChangeContext asynchronousConfigurationChangeContext = new AsynchronousConfigurationChangeContext(keys);
			OnChanged(asynchronousConfigurationChangeContext);
			Task trackedAction = asynchronousConfigurationChangeContext.GetTrackedAction();
			if (trackedAction != null)
			{
				await trackedAction.ConfigureAwait(false);
			}
		}

		protected abstract ICollection<string> GetKeys();

		
		protected abstract Task<string> ReadValueCoreAsync(string key);

		protected abstract Task WriteValueCoreAsync(string key, string value);

		protected abstract Task RemoveValueCoreAsync(string key);

		protected abstract void OnChanged(AsynchronousConfigurationChangeContext context);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void VerifyKey(string key, bool createNew = false)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("不允许使用空字符串作为配置项的 Key。", "key");
			}
			if (createNew && (key.Contains('\n') || key.Contains('\r')))
			{
				throw new ArgumentException("不允许使用换行符串作为配置项的 Key。", "key");
			}
		}
	}
}
