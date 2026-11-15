using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class AutoNumberProperty
	{
		public int StartAt { get; set; } = 1;

		public AutoNumberType AutoNumberType { get; set; } = AutoNumberType.ArabicPeriod;

		public AutoNumberProperty Clone()
		{
			return new AutoNumberProperty
			{
				AutoNumberType = AutoNumberType,
				StartAt = StartAt
			};
		}

		public override bool Equals(object o)
		{
			AutoNumberProperty autoNumberProperty = o as AutoNumberProperty;
			if (autoNumberProperty == null)
			{
				return false;
			}
			if (!autoNumberProperty.AutoNumberType.Equals(AutoNumberType))
			{
				return false;
			}
			if (!autoNumberProperty.StartAt.Equals(StartAt))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
	}
}
