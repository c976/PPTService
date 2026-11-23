using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class MarkerStyleExtension
	{
		public static Dictionary<MarkerStyle, string> MarkerCharDic { get; } = new Dictionary<MarkerStyle, string>
		{
			{
				MarkerStyle.Rect,
				"n"
			},
			{
				MarkerStyle.Box,
				"u"
			},
			{
				MarkerStyle.Circle,
				"l"
			},
			{
				MarkerStyle.Tick,
				"ü"
			},
			{
				MarkerStyle.Square,
				"«"
			}
		};
	}
}
