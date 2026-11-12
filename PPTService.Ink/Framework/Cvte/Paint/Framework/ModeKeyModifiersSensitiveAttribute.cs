using System;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	[Obsolete("通过特性获取的性能太差，请使用 IModeKeyboardSensitive 接口代替", true)]
	public sealed class ModeKeyModifiersSensitiveAttribute : Attribute
	{
		public ModifierKeys Modifiers { get; set; }

		public ModeKeyModifiersSensitiveAttribute(ModifierKeys modifiers)
		{
			Modifiers = modifiers;
		}
	}
}
