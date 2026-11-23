using System;

namespace PPTServiceUltra.Ink.Core.Windows.ScreenAdjuster
{
	public interface IAdjustScreen
	{
		bool GetBrightness(IntPtr handle, ref short minBrightness, ref short currentBrightness, ref short maxBrightness);

		bool SetBrightness(IntPtr handle, short brightness);
	}
}
