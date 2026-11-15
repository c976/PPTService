using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextLine")]
	public class ParagraphSaveInfo : SaveInfo
	{
		[SaveInfoMember("LineSpacing")]
		public double LineSpacing { get; set; } = 1.0;

		[SaveInfoMember("FixedLineSpacing")]
		public double FixedLineSpacing { get; set; } = double.NaN;

		[SaveInfoMember("TextAlignment")]
		public TextAlignment TextAlignment { get; set; }

		[SaveInfoMember("TextRuns")]
		public List<RunSaveInfo> TextRuns { get; set; } = new List<RunSaveInfo>();

		[ComplexSaveInfo]
		[SaveInfoMember("DefaultRunProperty")]
		public RunSaveInfo DefaultRunProperty { get; set; } = new RunSaveInfo();

		[SaveInfoMember("MarginLeft", Description = "文本之前距离")]
		public double MarginLeft { get; set; }

		[SaveInfoMember("Indent", Description = "度量值")]
		public double Indent { get; set; }

		[SaveInfoMember("TextMarkerStyle", Description = "项目符号类")]
		public MarkerSaveInfo TextMarkerSaveInfo { get; set; } = new MarkerSaveInfo();

		[SaveInfoMember("SpaceBefore", Description = "段前间距")]
		public double SpaceBefore { get; set; }

		[SaveInfoMember("SpaceAfter", Description = "段后间距")]
		public double SpaceAfter { get; set; }

		[SaveInfoMember("Lines", Description = "行信息")]
		public List<LineSaveInfo> Lines { get; set; } = new List<LineSaveInfo>();

		[SaveInfoMember("Direction", Description = "段落方向(从左向右、从右向左)")]
		public FlowDirection Direction { get; set; }

		[SaveInfoMember("IndentLevel")]
		public int IndentLevel { get; set; }

		[SaveInfoMember("TextMarker", Description = "项目符号")]
		public MarkerStyle TextMarkerStyle { get; set; }

		[SaveInfoMember("IndentType", Description = "缩进类型")]
		public IndentType IndentType { get; set; }
	}
}
