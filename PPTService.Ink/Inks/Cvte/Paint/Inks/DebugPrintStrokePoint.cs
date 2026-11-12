using System;
using System.Text;
using System.Windows.Ink;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	[Obsolete("此类只能在 DEBUG 下调用")]
	public static class DebugPrintStrokePoint
	{
		public static void Write(Stroke stroke)
		{
			Write(stroke.StylusPoints);
		}

		public static void Write(StylusPointCollection spc)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("            StylusPointCollection spc;\r\n            spc = new StylusPointCollection();");
			foreach (StylusPoint item in spc)
			{
				stringBuilder.Append($"spc.Add(new StylusPoint({item.X}, {item.Y}, {item.PressureFactor}f));\r\n");
			}
		}
	}
}
