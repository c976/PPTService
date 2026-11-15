using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[SaveInfoContract("Adjust")]
	public class AdjustSaveInfo : SaveInfo
	{
		public string Id { get; set; }

		public double ScaleX { get; set; }

		public double ScaleY { get; set; }
	}
}
