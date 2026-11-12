using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class ModeInputEventArgs : DeviceInputEventArgs, ICloneable
	{
		[Obsolete("不要使用。此属性不符合 API 设计原则，可能在未来版本中被移除。")]
		public InputSource Source { get; set; }

		[CanBeNull]
		public IModeHitTestProvider HitTestProvider { get; set; }

		public Slide Slide => HitTestProvider?.Slide;

		[CanBeNull]
		public Element Element => HitTestProvider?.Element;

		public DependencyObject HittedSource { get; }

		[Obsolete("请使用 ModeInputEventArgs.FromDeviceInput 替代。")]
		public ModeInputEventArgs(int id, Point position, DeviceType type, DeviceButton button, IEnumerable<StylusPoint> touchPoints, InputEventArgs args)
			: this(id, position, type, button, InputSource.Local, touchPoints, args)
		{
		}

		[Obsolete("请使用 ModeInputEventArgs.FromDeviceInput 替代。创建后，手动设置 Source 的值。")]
		public ModeInputEventArgs(int id, Point position, DeviceType type, DeviceButton button, InputSource source, IEnumerable<StylusPoint> touchPoints, InputEventArgs args)
			: this(args, id, position, type, button, touchPoints)
		{
			Source = source;
		}

		protected ModeInputEventArgs([CanBeNull] InputEventArgs args, int id, Point position, DeviceType type, DeviceButton button, IEnumerable<StylusPoint> touchPoints)
			: base(id, position, type, button, touchPoints, args)
		{
			HittedSource = args?.OriginalSource as DependencyObject;
		}

		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}

		public ModeInputEventArgs Clone()
		{
			return (ModeInputEventArgs)MemberwiseClone();
		}

		public static ModeInputEventArgs FromDeviceInput(DeviceInputEventArgs e)
		{
			ModeInputDragEventArgs e2 = e as ModeInputDragEventArgs;
			if (e2 != null)
			{
				return e2.Clone();
			}
			ModeInputEventArgs e3 = e as ModeInputEventArgs;
			if (e3 != null)
			{
				return e3.Clone();
			}
			return new ModeInputEventArgs(e.OriginEventArgs, e.Id, e.Position, e.Type, e.Button, e.GetStylusPoints())
			{
				VirtualType = e.VirtualType,
				HitTestProvider = new ModeHitTestProvider(e.OriginEventArgs?.OriginalSource as DependencyObject, e.Position, e.Type)
			};
		}
	}
}
