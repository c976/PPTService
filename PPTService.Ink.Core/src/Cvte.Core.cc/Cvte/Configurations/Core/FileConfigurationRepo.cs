using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PPTService.Ink.Core.Configurations.Concurrent;
using PPTService.Ink.Core.IO;
using PPTService.Ink.Core.Threading;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	
	public class FileConfigurationRepo : AsynchronousConfigurationRepo
	{
		private readonly FileInfo _file;

		private readonly FileWatcher _watcher;

		private readonly PartialAwaitableRetry _saveLoop;

		private readonly FileDictionarySynchronizer<string, string> _keyValueSynchronizer;

		private Task _currentReadingFileTask;

		private bool _isPendingReload;

		private bool _isPendingReloadReentered;

		private volatile int _syncWaitingCount;

		public TimeSpan DelayReadTime { get; set; } = TimeSpan.FromMilliseconds(1.0);

		public TimeSpan DelaySaveTime { get; set; } = TimeSpan.FromMilliseconds(1.0);

		public int FileSyncingCount => _keyValueSynchronizer.FileSyncingCount;

		public int FileSyncingErrorCount => _keyValueSynchronizer.FileSyncingErrorCount;

		public int SyncWaitingCount => _syncWaitingCount;

		public bool SupportsHighResolutionFileTime => _keyValueSynchronizer.SupportsHighResolutionFileTime;

		[Obsolete("请改用线程安全的 ConfigurationFactory 来创建实例。")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FileConfigurationRepo(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			string fullPath = Path.GetFullPath(fileName);
			_file = new FileInfo(fullPath);
			_saveLoop = new PartialAwaitableRetry(LoopSyncTask);
			_keyValueSynchronizer = new FileDictionarySynchronizer<string, string>(_file, (IReadOnlyDictionary<string, string> x) => CoinConfigurationSerializer.Serialize(x), (string x) => CoinConfigurationSerializer.Deserialize(x), FileEqualsComparison.KeyValueEquals);
			_watcher = new FileWatcher(_file);
			_currentReadingFileTask = FastSynchronizeAsync();
			_watcher.Changed += OnFileChanged;
			_watcher.WatchAsync();
		}

		protected override ICollection<string> GetKeys()
		{
			return _keyValueSynchronizer.Dictionary.Keys;
		}

		
		protected override async Task<string> ReadValueCoreAsync(string key)
		{
			await _currentReadingFileTask.ConfigureAwait(false);
			string value;
			return _keyValueSynchronizer.Dictionary.TryGetValue(key, out value) ? value : null;
		}

		protected override async Task WriteValueCoreAsync(string key, string value)
		{
			string text = value;
			if (text == null)
			{
				throw new ArgumentNullException("value");
			}
			value = text;
			value = value.Replace(Environment.NewLine, "\n");
			await _currentReadingFileTask.ConfigureAwait(false);
			_keyValueSynchronizer.Dictionary[key] = value;
		}

		protected override async Task RemoveValueCoreAsync(string key)
		{
			await _currentReadingFileTask.ConfigureAwait(false);
			string value;
			_keyValueSynchronizer.Dictionary.TryRemove(key, out value);
		}

		protected override void OnChanged(AsynchronousConfigurationChangeContext context)
		{
			AsynchronousConfigurationChangeContext asynchronousConfigurationChangeContext = context;
			if (asynchronousConfigurationChangeContext == null)
			{
				throw new ArgumentNullException("context");
			}
			context = asynchronousConfigurationChangeContext;
			Task action = SaveAsync(-1);
			context.TrackAsyncAction(action);
		}

		public async Task SaveAsync(int tryCount = 10)
		{
			Interlocked.Increment(ref _syncWaitingCount);
			await Task.Delay(DelaySaveTime).ConfigureAwait(false);
			await SynchronizeAsync(tryCount).ConfigureAwait(false);
		}

		public async Task ReloadExternalChangesAsync()
		{
			await _currentReadingFileTask.ConfigureAwait(false);
			_currentReadingFileTask = FastSynchronizeAsync();
			await _currentReadingFileTask.ConfigureAwait(false);
		}

		private void OnFileChanged( object sender, EventArgs e)
		{
			HandleFileChangedAsync();
		}

		private async Task HandleFileChangedAsync()
		{
			if (_keyValueSynchronizer.DangerousCheckIfThisFileChangeIsFromSelf())
			{
				return;
			}
			if (_isPendingReload)
			{
				_isPendingReloadReentered = true;
				return;
			}
			_isPendingReload = true;
			try
			{
				do
				{
					_isPendingReloadReentered = false;
					await Task.Delay((int)DelayReadTime.TotalMilliseconds / 2).ConfigureAwait(false);
				}
				while (_isPendingReloadReentered);
				await _currentReadingFileTask.ConfigureAwait(false);
				SynchronizeAsync();
			}
			finally
			{
				_isPendingReload = false;
			}
		}

		private async Task<OperationResult> LoopSyncTask(PartialRetryContext context)
		{
			context.StepCount = 10;
			_keyValueSynchronizer.Synchronize();
			Interlocked.Increment(ref _syncWaitingCount);
			await Task.Delay(DelaySaveTime).ConfigureAwait(false);
			return true;
		}

		private Task FastSynchronizeAsync()
		{
			return Task.Run(() =>
			{
				_keyValueSynchronizer.Synchronize();
			});
		}

		private async Task SynchronizeAsync(int tryCount = -1)
		{
			if (_currentReadingFileTask != null)
			{
				await _currentReadingFileTask.ConfigureAwait(false);
			}
			await _saveLoop.JoinAsync(tryCount);
		}
	}
}
