using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PPTServiceUltra.Ink.Core.Configurations.IO;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	
	public sealed class FileWatcher
	{
		[StructLayout(LayoutKind.Auto)]
		
		
		
		private struct FolderPair
		{
			internal readonly string _directory;

			internal readonly string _file;

			public FolderPair(string directory, string file)
			{
				_directory = directory;
				_file = file;
			}
		}

		private readonly FileInfo _file;

		
		private FileSystemWatcher _watcher;

		
		private Task _waitingAsyncAction;

		
		
		
		public event EventHandler Changed;

		public FileWatcher(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			_file = new FileInfo(fileName);
		}

		public FileWatcher(FileInfo file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			_file = file;
		}

		private void OnChanged()
		{
			Changed?.Invoke(this, EventArgs.Empty);
		}

		public async Task WatchAsync()
		{
			await WaitPendingTaskAsync().ConfigureAwait(false);
			_waitingAsyncAction = Task.Run((Action)Watch);
			await _waitingAsyncAction.ConfigureAwait(false);
		}

		public async Task StopAsync()
		{
			await WaitPendingTaskAsync().ConfigureAwait(false);
			_waitingAsyncAction = null;
			Stop();
		}

		private void Watch()
		{
			Watch(10);
		}

		private void Watch(int retryTime)
		{
			Stop();
			retryTime--;
			if (retryTime <= 0)
			{
				return;
			}
			FolderPair folderPair = FindWatchableLevel();
			string directory = folderPair._directory;
			string file = folderPair._file;
			if (string.IsNullOrEmpty(directory))
			{
				return;
			}
			if (File.Exists(_file.FullName))
			{
				_watcher = new FileSystemWatcher(directory, file)
				{
					EnableRaisingEvents = true,
					NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite)
				};
				FileSystemWatcherWeakEventRelay fileSystemWatcherWeakEventRelay = new FileSystemWatcherWeakEventRelay(_watcher);
				fileSystemWatcherWeakEventRelay.Changed += FinalFile_Changed;
				fileSystemWatcherWeakEventRelay.Deleted += FileOrDirectory_CreatedOrDeleted;
				return;
			}
			try
			{
				_watcher = new FileSystemWatcher(directory, file)
				{
					EnableRaisingEvents = true
				};
				FileSystemWatcherWeakEventRelay fileSystemWatcherWeakEventRelay2 = new FileSystemWatcherWeakEventRelay(_watcher);
				fileSystemWatcherWeakEventRelay2.Created += FileOrDirectory_CreatedOrDeleted;
				fileSystemWatcherWeakEventRelay2.Renamed += FileOrDirectory_CreatedOrDeleted;
				fileSystemWatcherWeakEventRelay2.Deleted += FileOrDirectory_CreatedOrDeleted;
			}
			catch (FileNotFoundException)
			{
				Watch(retryTime);
			}
		}

		private void Stop()
		{
			_watcher?.Dispose();
			_watcher = null;
		}

		private void FileOrDirectory_CreatedOrDeleted(object sender, FileSystemEventArgs e)
		{
			Watch();
			OnChanged();
		}

		private void FinalFile_Changed(object sender, FileSystemEventArgs e)
		{
			OnChanged();
		}

		private FolderPair FindWatchableLevel()
		{
			string path = _file.FullName;
			if (File.Exists(path))
			{
				return new FolderPair(Path.GetDirectoryName(path), Path.GetFileName(path));
			}
			string directoryName;
			string fileName;
			while (true)
			{
				directoryName = Path.GetDirectoryName(path);
				fileName = Path.GetFileName(path);
				if (string.IsNullOrEmpty(directoryName))
				{
					return new FolderPair(directoryName, fileName);
				}
				if (Directory.Exists(directoryName))
				{
					break;
				}
				path = directoryName;
			}
			return new FolderPair(directoryName, fileName);
		}

		private async Task WaitPendingTaskAsync()
		{
			if (_waitingAsyncAction != null)
			{
				await _waitingAsyncAction.ConfigureAwait(false);
			}
		}
	}
}
