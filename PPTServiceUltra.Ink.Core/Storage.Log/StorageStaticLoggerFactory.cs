using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Log
{
	
	
	public class StorageStaticLoggerFactory
	{
		internal static IStorageStaticLogger Logger { get; set; }

		
		public static void SetStorageStaticLogger(IStorageStaticLogger logger)
		{
			Logger = logger;
		}
	}
}
