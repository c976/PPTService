using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class FillEffectArgs : ITextEffectArgs
	{
		[CanBeNull]
		public System.Windows.Media.Brush FillBrush { get; set; }

		public double Opacity { get; set; } = 1.0;

		public int Priority => 50;

		public override bool Equals(object obj)
		{
			FillEffectArgs fillEffectArgs = obj as FillEffectArgs;
			if (fillEffectArgs == null)
			{
				return false;
			}
			if (!Converter.AreEquals(FillBrush, fillEffectArgs.FillBrush))
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
			return new FillEffectArgs
			{
				FillBrush = (PPTServiceUltra.Ink.Core.Storage.Standard.Brush)FillBrush
			};
		}

		public EffectSaveInfo ToSaveInfo()
		{
			return new FillEffectSaveInfo
			{
				FillBrush = FillBrush
			};
		}
	}
}
