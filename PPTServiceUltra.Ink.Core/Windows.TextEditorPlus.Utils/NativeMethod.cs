using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class NativeMethod
	{
		internal enum SystemMetric
		{
			SM_CXDOUBLECLK = 36,
			SM_CYDOUBLECLK
		}

		private const string USER32DLL = "user32.dll";

		[DllImport("user32.dll")]
		public static extern uint GetDoubleClickTime();

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(SystemMetric smIndex);
	}
}
