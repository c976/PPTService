using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Log
{
	
	public interface IStorageStaticLogger
	{
		void Info(string message);

		void Warning(string message);

		void Warning(string message, Exception ex);

		void Error(string message);

		void Error(string message, Exception ex);
	}
}
