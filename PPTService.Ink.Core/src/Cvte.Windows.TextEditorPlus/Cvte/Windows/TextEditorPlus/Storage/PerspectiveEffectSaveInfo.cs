using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextPerspective", Description = "透视")]
	public class PerspectiveEffectSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("Opacity", Description = "不透明度", Aliases = new string[] { "PerspectiveOpacity" })]
		public double Opacity { get; set; }

		[SaveInfoMember("Angle", Description = "角度", Aliases = new string[] { "PerspectiveAngle" })]
		public double Angle { get; set; }

		[SaveInfoMember("Depth", Description = "深度", Aliases = new string[] { "PerspectiveDepth" })]
		public double Depth { get; set; }

		[SaveInfoMember("Blur", Description = "模糊", Aliases = new string[] { "PerspectiveBlur" })]
		public double Blur { get; set; }

		[SaveInfoMember("Brush", Description = "画刷", Aliases = new string[] { "PerspectiveBrush" })]
		public Brush Brush { get; set; }

		public override bool Equals(object obj)
		{
			PerspectiveEffectSaveInfo perspectiveEffectSaveInfo = obj as PerspectiveEffectSaveInfo;
			if (perspectiveEffectSaveInfo == null)
			{
				return false;
			}
			if (!Converter.AreEquals(Opacity, perspectiveEffectSaveInfo.Opacity))
			{
				return false;
			}
			if (!Converter.AreEquals(Angle, perspectiveEffectSaveInfo.Angle))
			{
				return false;
			}
			if (!Converter.AreEquals(Depth, perspectiveEffectSaveInfo.Depth))
			{
				return false;
			}
			if (!Converter.AreEquals(Blur, perspectiveEffectSaveInfo.Blur))
			{
				return false;
			}
			if (!Converter.AreEquals(Brush, perspectiveEffectSaveInfo.Brush))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public override ITextEffectArgs ToEffectArgs()
		{
			if (Opacity < 0.0)
			{
				Opacity = 0.0;
			}
			if (Opacity > 1.0)
			{
				Opacity = 1.0;
			}
			if (Blur < 0.0)
			{
				Blur = 0.0;
			}
			if (Blur > 200.0)
			{
				Blur = 200.0;
			}
			if (Depth < 0.0)
			{
				Depth = 0.0;
			}
			if (Depth > 1.0)
			{
				Depth = 1.0;
			}
			return new PerspectiveEffectArgs
			{
				PerspectiveBrush = Brush,
				PerspectiveAngle = Angle,
				PerspectiveBlur = Blur,
				PerspectiveDepth = Depth,
				PerspectiveOpacity = Opacity
			};
		}
	}
}
