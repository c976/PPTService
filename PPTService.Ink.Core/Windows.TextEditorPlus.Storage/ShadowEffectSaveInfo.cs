using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextShadow", Description = "阴影")]
	public class ShadowEffectSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("Opacity", Description = "不透明度", Aliases = new string[] { "ShadowOpacity" })]
		public double Opacity { get; set; }

		[SaveInfoMember("Direction", Description = "角度", Aliases = new string[] { "ShadowDirection" })]
		public double Direction { get; set; }

		[SaveInfoMember("Distance", Description = "距离", Aliases = new string[] { "ShadowDistance" })]
		public double Distance { get; set; }

		[SaveInfoMember("Blur", Description = "模糊", Aliases = new string[] { "ShadowBlur" })]
		public double Blur { get; set; }

		[SaveInfoMember("Brush", Description = "画刷", Aliases = new string[] { "ShadowBrush" })]
		public Brush Brush { get; set; }

		public override bool Equals(object obj)
		{
			ShadowEffectSaveInfo shadowEffectSaveInfo = obj as ShadowEffectSaveInfo;
			if (shadowEffectSaveInfo == null)
			{
				return false;
			}
			if (!Converter.AreEquals(Opacity, shadowEffectSaveInfo.Opacity))
			{
				return false;
			}
			if (!Converter.AreEquals(Direction, shadowEffectSaveInfo.Direction))
			{
				return false;
			}
			if (!Converter.AreEquals(Distance, shadowEffectSaveInfo.Distance))
			{
				return false;
			}
			if (!Converter.AreEquals(Blur, shadowEffectSaveInfo.Blur))
			{
				return false;
			}
			if (!Converter.AreEquals(Brush, shadowEffectSaveInfo.Brush))
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
			return EffectSaveInfoExtension.ToEffectArgs(this);
		}
	}
}
