using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextReflection", Description = "映像")]
	public class ReflectionEffectSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("Opacity", Description = "不透明度", Aliases = new string[] { "ReflectionOpacity" })]
		public double Opacity { get; set; }

		[SaveInfoMember("Depth", Description = "深度", Aliases = new string[] { "ReflectionDepth" })]
		public double Depth { get; set; }

		[SaveInfoMember("Distance", Description = "距离", Aliases = new string[] { "ReflectionDistance" })]
		public double Distance { get; set; }

		public override bool Equals(object obj)
		{
			ReflectionEffectSaveInfo reflectionEffectSaveInfo = obj as ReflectionEffectSaveInfo;
			if (reflectionEffectSaveInfo == null)
			{
				return false;
			}
			if (!Converter.AreEquals(Opacity, reflectionEffectSaveInfo.Opacity))
			{
				return false;
			}
			if (!Converter.AreEquals(Depth, reflectionEffectSaveInfo.Depth))
			{
				return false;
			}
			if (!Converter.AreEquals(Distance, reflectionEffectSaveInfo.Distance))
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
			if (Depth < 0.0)
			{
				Depth = 0.0;
			}
			if (Depth > 1.0)
			{
				Depth = 1.0;
			}
			if (Distance < 0.0)
			{
				Distance = 0.0;
			}
			return new ReflectionEffectArgs
			{
				ReflectionDepth = Depth,
				ReflectionDistance = Distance,
				ReflectionOpacity = Opacity
			};
		}
	}
}
