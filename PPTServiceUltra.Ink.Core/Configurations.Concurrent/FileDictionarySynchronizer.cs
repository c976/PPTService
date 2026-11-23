using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using PPTServiceUltra.Ink.Core.Configurations.Utils;

namespace PPTServiceUltra.Ink.Core.Configurations.Concurrent
{
	
	
	internal class FileDictionarySynchronizer< TKey,  TValue>
	{
		private static readonly HashSet<string> KnownHighResolutionDriveFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "NTFS", "APFS", "EXT4" };

		private readonly FileInfo _file;

		private readonly Func<IReadOnlyDictionary<TKey, TValue>, string> _serializer;

		private readonly Func<string, IReadOnlyDictionary<TKey, TValue>> _deserializer;

		private readonly FileEqualsComparison _fileEqualsComparison;

		private DateTimeOffset _fileLastWriteTime = DateTimeOffset.MinValue;

		private bool? _lastFileExists;

		private string _lastSyncedFileContent = "";

		private volatile int _fileSyncingCount;

		private volatile int _fileSyncingErrorCount;

		private bool _isInWritingFileRegion;

		private bool _hasCheckedFileChange;

		public bool SupportsHighResolutionFileTime { get; }

		
		
		public string FileDriveFormat
		{
			
			get;
		}

		public int FileSyncingCount => _fileSyncingCount;

		public int FileSyncingErrorCount => _fileSyncingErrorCount;

		public ProcessConcurrentDictionary<TKey, TValue> Dictionary { get; } = new ProcessConcurrentDictionary<TKey, TValue>();

		public FileDictionarySynchronizer(FileInfo file, Func<IReadOnlyDictionary<TKey, TValue>, string> serializer, Func<string, IReadOnlyDictionary<TKey, TValue>> deserializer, FileEqualsComparison fileEqualsComparison = FileEqualsComparison.WholeTextEquals)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			_file = file;
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			_serializer = serializer;
			if (deserializer == null)
			{
				throw new ArgumentNullException("deserializer");
			}
			_deserializer = deserializer;
			_fileEqualsComparison = fileEqualsComparison;
			try
			{
				DriveInfo driveInfo = new DriveInfo(file.FullName);
				FileDriveFormat = driveInfo.DriveFormat;
				SupportsHighResolutionFileTime = KnownHighResolutionDriveFormats.Contains(FileDriveFormat);
			}
			catch (Exception)
			{
				FileDriveFormat = null;
				SupportsHighResolutionFileTime = false;
			}
		}

		public bool DangerousCheckIfThisFileChangeIsFromSelf()
		{
			if (_isInWritingFileRegion)
			{
				return true;
			}
			if (_hasCheckedFileChange)
			{
				_hasCheckedFileChange = false;
				return true;
			}
			return false;
		}

		public void Synchronize()
		{
			Dictionary.UpdateValuesFromExternal(_file, ( ICriticalReadWriteContext<TKey, TValue> context) =>
			{
				try
				{
					SynchronizeCore(context);
				}
				catch (IOException)
				{
					Interlocked.Increment(ref _fileSyncingErrorCount);
					throw;
				}
				finally
				{
				}
			});
		}

		private void SynchronizeCore(ICriticalReadWriteContext<TKey, TValue> context)
		{
			_file.Refresh();
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;
			DateTimeOffset dateTimeOffset = (_file.Exists ? FixFileTime(_file.LastWriteTimeUtc, utcNow) : utcNow);
			if (_file.Exists && ((!_lastFileExists) ?? false))
			{
				dateTimeOffset = utcNow;
			}
			DateTimeOffset dateTimeOffset2;
			if (SupportsHighResolutionFileTime && dateTimeOffset == _fileLastWriteTime && _file.Exists == _lastFileExists)
			{
				dateTimeOffset2 = SyncWhenFileHasNotBeenUpdated(context, dateTimeOffset);
			}
			else
			{
				if (_file.Exists)
				{
					bool supportsHighResolutionFileTime = SupportsHighResolutionFileTime;
				}
				dateTimeOffset2 = SyncWhenFileHasBeenUpdated(context, dateTimeOffset);
			}
			if (dateTimeOffset != dateTimeOffset2.UtcDateTime)
			{
				_file.LastWriteTimeUtc = dateTimeOffset2.UtcDateTime;
			}
			_lastFileExists = _file.Exists;
			_fileLastWriteTime = (_file.Exists ? dateTimeOffset2 : DateTimeOffset.MinValue);
		}

		private DateTimeOffset SyncWhenFileHasBeenUpdated(ICriticalReadWriteContext<TKey, TValue> context, DateTimeOffset lastWriteTime)
		{
			Interlocked.Increment(ref _fileSyncingCount);
			string text = ReadAllText();
			if (text == null)
			{
				return lastWriteTime;
			}
			_lastSyncedFileContent = text;
			DateTimeOffset updatedWriteTime;
			bool hasChanged;
			string text2 = MergeFileTextAndKeyValueText(context, lastWriteTime, text, out updatedWriteTime, out hasChanged);
			if (hasChanged)
			{
				WriteAllText(text2);
				_lastSyncedFileContent = text2;
				return updatedWriteTime;
			}
			return lastWriteTime;
		}

		private DateTimeOffset SyncWhenFileHasNotBeenUpdated(ICriticalReadWriteContext<TKey, TValue> context, DateTimeOffset lastWriteTime)
		{
			DateTimeOffset updatedWriteTime;
			bool hasChanged;
			string text = MergeFileTextAndKeyValueText(context, lastWriteTime, _lastSyncedFileContent, out updatedWriteTime, out hasChanged);
			if (hasChanged)
			{
				Interlocked.Increment(ref _fileSyncingCount);
				WriteAllText(text);
				_lastSyncedFileContent = text;
				return updatedWriteTime;
			}
			return lastWriteTime;
		}

		
		private string ReadAllText()
		{
			if (_file.Exists)
			{
				return DoIOActionWithRetry( (int i) =>
				{
					using (FileStream stream = new FileStream(_file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
					{
						using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, true, 4096, true))
						{
							return streamReader.ReadToEnd();
						}
					}
				});
			}
			return "";
		}

		private void WriteAllText(string text)
		{
			try
			{
				_isInWritingFileRegion = true;
				_hasCheckedFileChange = true;
				DoIOActionWithRetry((int i) =>
				{
					using (FileStream fileStream = new FileStream(_file.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false, false), 4096, true))
						{
							fileStream.Position = 0L;
							streamWriter.Write(text);
							streamWriter.Flush();
							fileStream.SetLength(fileStream.Position);
						}
					}
				});
			}
			finally
			{
				_isInWritingFileRegion = false;
			}
		}

		private string MergeFileTextAndKeyValueText(ICriticalReadWriteContext<TKey, TValue> context, DateTimeOffset lastWriteTime, string text, out DateTimeOffset updatedWriteTime, out bool hasChanged)
		{
			IReadOnlyDictionary<TKey, TValue> readOnlyDictionary = _deserializer(text);
			TimedKeyValues<TKey, TValue> timedKeyValues = context.MergeExternalKeyValues(readOnlyDictionary, lastWriteTime);
			Dictionary<TKey, TValue> dictionary = timedKeyValues.KeyValues.ToDictionary( (KeyValuePair<TKey, TValue> x) => x.Key,  (KeyValuePair<TKey, TValue> x) => x.Value);
			string text2 = _serializer(dictionary);
			updatedWriteTime = timedKeyValues.Time;
			if (_fileEqualsComparison == FileEqualsComparison.KeyValueEquals)
			{
				hasChanged = !((ICollection<KeyValuePair<TKey, TValue>>)readOnlyDictionary).SequenceEqualsIgnoringOrder(dictionary);
			}
			else
			{
				hasChanged = !string.Equals(text, text2, StringComparison.Ordinal);
			}
			return text2;
		}

		private static void DoIOActionWithRetry(Action<int> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (int i = 0; i < 10; i++)
			{
				try
				{
					action(i);
				}
				catch (IOException)
				{
					Thread.Yield();
				}
			}
		}

		
		private static T DoIOActionWithRetry<T>(Func<int, T> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			for (int i = 0; i < 10; i++)
			{
				try
				{
					return func(i);
				}
				catch (IOException)
				{
					Thread.Yield();
				}
			}
			return default(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DateTimeOffset FixFileTime(DateTimeOffset time, DateTimeOffset utcNow)
		{
			if (!(time > utcNow))
			{
				return time;
			}
			return utcNow;
		}
	}
}
