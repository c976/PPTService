using System.IO;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.WeakEvents;

namespace PPTService.Ink.Core.Configurations.IO
{
	
	
	internal sealed class FileSystemWatcherWeakEventRelay : WeakEventRelay<FileSystemWatcher>
	{
		private readonly WeakEvent<FileSystemEventArgs> _created = new WeakEvent<FileSystemEventArgs>();

		private readonly WeakEvent<FileSystemEventArgs> _changed = new WeakEvent<FileSystemEventArgs>();

		private readonly WeakEvent<RenamedEventArgs> _renamed = new WeakEvent<RenamedEventArgs>();

		private readonly WeakEvent<FileSystemEventArgs> _deleted = new WeakEvent<FileSystemEventArgs>();

		public event FileSystemEventHandler Created
		{
			add
			{
				Subscribe((FileSystemWatcher o) =>
				{
					o.Created += OnCreated;
				}, () =>
				{
					_created.Add(value, value.Invoke);
				}, "Created");
			}
			remove
			{
				_created.Remove(value);
			}
		}

		public event FileSystemEventHandler Changed
		{
			add
			{
				Subscribe((FileSystemWatcher o) =>
				{
					o.Changed += OnChanged;
				}, () =>
				{
					_changed.Add(value, value.Invoke);
				}, "Changed");
			}
			remove
			{
				_changed.Remove(value);
			}
		}

		public event RenamedEventHandler Renamed
		{
			add
			{
				Subscribe((FileSystemWatcher o) =>
				{
					o.Renamed += OnRenamed;
				}, () =>
				{
					_renamed.Add(value, value.Invoke);
				}, "Renamed");
			}
			remove
			{
				_renamed.Remove(value);
			}
		}

		public event FileSystemEventHandler Deleted
		{
			add
			{
				Subscribe((FileSystemWatcher o) =>
				{
					o.Deleted += OnDeleted;
				}, () =>
				{
					_deleted.Add(value, value.Invoke);
				}, "Deleted");
			}
			remove
			{
				_deleted.Remove(value);
			}
		}

		public FileSystemWatcherWeakEventRelay(FileSystemWatcher eventSource)
			: base(eventSource)
		{
		}

		private void OnCreated(object sender, FileSystemEventArgs e)
		{
			TryInvoke(_created, sender, e);
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			TryInvoke(_changed, sender, e);
		}

		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			TryInvoke(_renamed, sender, e);
		}

		private void OnDeleted(object sender, FileSystemEventArgs e)
		{
			TryInvoke(_deleted, sender, e);
		}

		protected override void OnReferenceLost(FileSystemWatcher source)
		{
			source.Created -= OnCreated;
			source.Changed -= OnChanged;
			source.Renamed -= OnRenamed;
			source.Deleted -= OnDeleted;
			source.Dispose();
		}
	}
}
