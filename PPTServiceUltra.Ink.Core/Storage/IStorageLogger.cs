using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface IStorageLogger
	{
		IList<StorageException> WarningExceptions { get; }

		IList<StorageException> Errors { get; }

		void Info(string msg);

		void Warning(string msg);

		void Warning(StorageException exception);

		void Error(StorageException exception);

		void SaveReport(string reportPath);
	}
}
