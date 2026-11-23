using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextMarker", Description = "文本项目符号")]
	public class MarkerSaveInfo : SaveInfo
	{
		[SaveInfoMember("FontFamily", Description = "项目符号字体类型")]
		public FontFamily FontFamily { get; set; }

		[SaveInfoMember("Char", Description = "项目符号字符")]
		public string Char { get; set; }

		[SaveInfoMember("AutoNumber", Description = "项目符号数字")]
		public AutoNumberSaveInfo AutoNumber { get; set; }

		[CanBeNull]
		[SaveInfoMember("Foreground", Description = "项目符号前景色")]
		public Brush Foreground { get; set; }

		[SaveInfoMember("FontSize", Description = "项目符号字体大小")]
		[DoubleRangeChecker(Minimum = 1.0, Maximun = 10000.0)]
		public double FontSize { get; set; }

		public MarkerSaveInfo Clone()
		{
			return new MarkerSaveInfo
			{
				FontFamily = FontFamily,
				Char = Char,
				AutoNumber = AutoNumber
			};
		}

		public bool IsEmpty()
		{
			if (string.IsNullOrEmpty(Char) && AutoNumber == null)
			{
				return true;
			}
			return false;
		}
	}
}
