using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	public class BlockFileInfo
	{
		public long StartOffset { get; set; }

		public long EndOffset { get; set; }

		public long BlockSize { get; set; }

		public long Index { get; set; }

		public string Ctx { get; set; }
	}
}
