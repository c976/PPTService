using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class ReflectionEffectArgs : ITextEffectArgs
	{
		public double Opacity { get; set; } = 1.0;

		public double ReflectionOpacity { get; set; } = 1.0;

		public double ReflectionDepth { get; set; }

		public double ReflectionDistance { get; set; }

		public int Priority => -1;

		public override bool Equals(object obj)
		{
			ReflectionEffectArgs reflectionEffectArgs = obj as ReflectionEffectArgs;
			if (reflectionEffectArgs == null)
			{
				return false;
			}
			if (!Converter.AreEquals(ReflectionOpacity, reflectionEffectArgs.ReflectionOpacity))
			{
				return false;
			}
			if (!Converter.AreEquals(ReflectionDepth, reflectionEffectArgs.ReflectionDepth))
			{
				return false;
			}
			if (!Converter.AreEquals(ReflectionDistance, reflectionEffectArgs.ReflectionDistance))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public ITextEffectArgs Clone()
		{
			return new ReflectionEffectArgs
			{
				ReflectionOpacity = ReflectionOpacity,
				ReflectionDepth = ReflectionDepth,
				ReflectionDistance = ReflectionDistance
			};
		}

		public EffectSaveInfo ToSaveInfo()
		{
			return new ReflectionEffectSaveInfo
			{
				Depth = ReflectionDepth,
				Distance = ReflectionDistance,
				Opacity = ReflectionOpacity
			};
		}
	}
}
