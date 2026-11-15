using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class SectionLogger : IStorageLogger, IDisposable
	{
		private readonly Stopwatch _stopwatch;

		public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

		public string Name { get; }

		public IList<string> Infos { get; }

		public IList<string> Warnings { get; }

		public IList<StorageException> WarningExceptions { get; }

		public IList<StorageException> Errors { get; }

		public SectionLogger(string name)
		{
			Name = name;
			Infos = new List<string>();
			Warnings = new List<string>();
			Errors = new List<StorageException>();
			WarningExceptions = new List<StorageException>();
			_stopwatch = new Stopwatch();
			_stopwatch.Start();
		}

		public void Info(string msg)
		{
			Infos.Add(msg);
		}

		public void Warning(string msg)
		{
			Warnings.Add(msg);
		}

		public void Warning(StorageException exception)
		{
			WarningExceptions.Add(exception);
		}

		public void Error(StorageException exception)
		{
			Errors.Add(exception);
		}

		public void SaveReport(string reportPath)
		{
		}

		public void Dispose()
		{
			_stopwatch.Stop();
		}
	}
}
