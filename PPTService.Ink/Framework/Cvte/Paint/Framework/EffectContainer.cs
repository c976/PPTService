using System;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	internal class EffectContainer : ElementVisualPanel
	{
		internal EffectContainer(UIElement visualParent, Action<Visual> addVisualChild, Action<Visual> removeVisualChild)
			: base(visualParent, addVisualChild, removeVisualChild)
		{
			IsHitTestVisible = false;
		}

		public void AddEffect(Visual effect)
		{
			Children.Add(effect);
		}

		public void InsertEffect(int index, Visual effect)
		{
			Children.Insert(index, effect);
		}

		public void RemoveEffect(Visual effect)
		{
			Children.Remove(effect);
		}
	}
}
