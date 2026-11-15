using System;
using System.Windows;
using System.Windows.Interop;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.ScreenAdjuster
{
	public static class AdjustScreenBuilder
	{
		[CanBeNull]
		[PublicAPI]
		public static IAdjustScreen CreateAdjustScreen(Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			AdjustScreenByDxva2 adjustScreenByDxva = new AdjustScreenByDxva2();
			IntPtr handle = new WindowInteropHelper(window).Handle;
			short minBrightness = 0;
			short currentBrightness = 0;
			short maxBrightness = 0;
			if (adjustScreenByDxva.GetBrightness(handle, ref minBrightness, ref currentBrightness, ref maxBrightness))
			{
				return adjustScreenByDxva;
			}
			AdjustScreenByGdi32 adjustScreenByGdi = new AdjustScreenByGdi32();
			if (adjustScreenByGdi.GetBrightness(handle, ref minBrightness, ref currentBrightness, ref maxBrightness))
			{
				return adjustScreenByGdi;
			}
			return null;
		}
	}
}
