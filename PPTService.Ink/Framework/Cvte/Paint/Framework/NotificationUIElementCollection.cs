using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Cvte.Paint.Framework
{
	internal class NotificationUIElementCollection : UIElementCollection
	{
		private readonly EventHandler _onChanged;

		public NotificationUIElementCollection(UIElement visualParent, FrameworkElement logicalParent, EventHandler onChanged)
			: base(visualParent, logicalParent)
		{
			_onChanged = onChanged;
		}

		public override int Add(UIElement element)
		{
			int result = base.Add(element);
			EventHandler onChanged = _onChanged;
			if (onChanged != null)
			{
				onChanged(this, EventArgs.Empty);
				return result;
			}
			return result;
		}

		public override void Insert(int index, UIElement element)
		{
			base.Insert(index, element);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void Remove(UIElement element)
		{
			base.Remove(element);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void RemoveAt(int index)
		{
			base.RemoveAt(index);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void Clear()
		{
			base.Clear();
			_onChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
