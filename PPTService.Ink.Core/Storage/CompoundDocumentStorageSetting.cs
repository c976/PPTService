using System.IO;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class CompoundDocumentStorageSetting
	{
		public string CachePath { get; set; }

		public string ReportPath { get; set; }

		public bool ReportEnable { get; set; }

		public CompoundDocumentStorageSetting()
		{
			CachePath = Path.GetTempPath();
			ReportEnable = false;
		}
	}
}
