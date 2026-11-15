using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[DebuggerDisplay("Text={Text}\r\nFontSize={FontSize}")]
	[SaveInfoContract("TextRun")]
	public class RunSaveInfo : SaveInfo
	{
		[SaveInfoMember("Text", Description = "纯文本")]
		public string Text { get; set; } = string.Empty;

		[SaveInfoMember("FontSize", Description = "字号")]
		[DoubleRangeChecker(Minimum = 1.0, Maximun = 10000.0)]
		public double FontSize { get; set; } = 40.0;

		[SaveInfoMember("FontVariants", Description = "上下标", Aliases = new string[] { "Variants" })]
		public FontVariants FontVariants { get; set; }

		[SaveInfoMember("FontStyle", Description = "斜体")]
		public PPTService.Ink.Core.Storage.Standard.FontStyle FontStyle { get; set; } = FontStyles.Normal;

		[SaveInfoMember("FontWeight", Description = "加粗")]
		public PPTService.Ink.Core.Storage.Standard.FontWeight FontWeight { get; set; }

		[SaveInfoMember("FontFamily", Description = "字体")]
		public FontFamily FontFamily { get; set; } = TextContext.DefaultFontFamily;

		[SaveInfoMember("Background", Description = "背景色")]
		public Brush Background { get; set; } = TextContext.DefaultBackground;

		[SaveInfoMember("Foreground", Description = "前景色")]
		public Brush Foreground { get; set; } = TextContext.DefaultForeground;

		[SaveInfoMember("Opacity", Description = "不透明度", DefaultValue = 1.0)]
		public double Opacity { get; set; } = 1.0;

		[SaveInfoMember("Decoration", Description = "下划线")]
		public PPTService.Ink.Core.Storage.Standard.TextDecorationCollection Decoration { get; set; } = new PPTService.Ink.Core.Storage.Standard.TextDecorationCollection();

		[SaveInfoMember("TextEffects", Description = "艺术效果")]
		public List<EffectSaveInfo> TextEffects { get; set; } = new List<EffectSaveInfo>();

		[Obsolete("拼写错误")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SaveInfoMember("Hyerlink", Description = "超链接(拼写错误)")]
		public string Hyerlink { get; set; }

		[SaveInfoMember("Hyperlink", Description = "超链接")]
		public string Hyperlink { get; set; }

		[SaveInfoMember("UnderlineBrush", Description = "下划线画刷")]
		public Brush UnderlineBrush { get; set; }

		[SaveInfoMember("DashStyle", Description = "下划线虚线样式")]
		public DashStyle UnderlineDashStyle { get; set; }

		public RunSaveInfo()
		{
			Extensions = new List<SaveInfo>();
		}
	}
}
