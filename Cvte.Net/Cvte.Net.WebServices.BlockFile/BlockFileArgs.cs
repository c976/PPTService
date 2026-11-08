using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	public class BlockFileArgs : EventArgs
	{
		public string FileHash { get; }

		public BlockFileArgs(string fileHash)
		{
			FileHash = fileHash;
		}
	}
}
