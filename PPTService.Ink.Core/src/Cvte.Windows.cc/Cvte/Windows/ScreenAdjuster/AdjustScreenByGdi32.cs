using System;
using System.Drawing;
using System.Linq;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.ScreenAdjuster
{
	internal class AdjustScreenByGdi32 : IAdjustScreen
	{
		private double CalAllGammaVal(Win32.RAMP ramp)
		{
			return Math.Round((CalColorGammaVal(ramp.Blue) + CalColorGammaVal(ramp.Red) + CalColorGammaVal(ramp.Green)) / 3.0, 2);
		}

		[UsedImplicitly]
		internal AdjustScreenByGdi32()
		{
		}

		private double CalColorGammaVal(ushort[] line)
		{
			ushort max = line.Max();
			ushort num = line[0];
			int num2 = Array.FindIndex(line, (ushort n) => n == max);
			return Math.Round((double)(max - num) / (double)num2 / 255.0, 2);
		}

		public bool GetBrightness(IntPtr handle, ref short minBrightness, ref short currentBrightness, ref short maxBrightness)
		{
			handle = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
			minBrightness = 50;
			maxBrightness = 100;
			Win32.RAMP lpRamp = default(Win32.RAMP);
			bool deviceGammaRamp = Win32.Gdi32.GetDeviceGammaRamp(handle, ref lpRamp);
			currentBrightness = (short)((deviceGammaRamp ? CalAllGammaVal(lpRamp) : 0.5) * 100.0);
			return deviceGammaRamp;
		}

		public bool SetBrightness(IntPtr handle, short brightness)
		{
			handle = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
			double num = (double)brightness / 100.0;
			Win32.RAMP lpRamp = new Win32.RAMP
			{
				Red = new ushort[256],
				Green = new ushort[256],
				Blue = new ushort[256]
			};
			for (int i = 1; i < 256; i++)
			{
				ushort val = (ushort)((double)(i * 255) * num);
				lpRamp.Red[i] = (lpRamp.Green[i] = (lpRamp.Blue[i] = Math.Max((ushort)0, Math.Min(ushort.MaxValue, val))));
			}
			return Win32.Gdi32.SetDeviceGammaRamp(handle, ref lpRamp);
		}
	}
}
