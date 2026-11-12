using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	internal class ElementVisualPanel : FrameworkElement
	{
		private readonly Visual _visualParent;

		private readonly Action<Visual> _addVisualChild;

		private readonly Action<Visual> _removeVisualChild;

		private bool _isEmpty = true;

		public IList<Visual> Children { get; }

		protected sealed override int VisualChildrenCount => Children.Count;

		internal ElementVisualPanel(Visual visualParent, Action<Visual> addVisualChild, Action<Visual> removeVisualChild)
		{
			Children = new NotificationVisualCollection(this, OnChildrenChanged);
			if (visualParent == null)
			{
				throw new ArgumentNullException("visualParent");
			}
			_visualParent = visualParent;
			if (addVisualChild == null)
			{
				throw new ArgumentNullException("addVisualChild");
			}
			_addVisualChild = addVisualChild;
			if (removeVisualChild == null)
			{
				throw new ArgumentNullException("removeVisualChild");
			}
			_removeVisualChild = removeVisualChild;
		}

		private void OnChildrenChanged(object sender, EventArgs e)
		{
			if (((ICollection<Visual>)sender).Count == 0)
			{
				if (!_isEmpty)
				{
					try
					{
						OnChildrenDisappeared();
					}
					finally
					{
						_isEmpty = true;
					}
				}
			}
			else if (_isEmpty)
			{
				try
				{
					OnChildrenAppeared();
				}
				finally
				{
					_isEmpty = false;
				}
			}
		}

		private void OnChildrenAppeared()
		{
			_addVisualChild(this);
		}

		private void OnChildrenDisappeared()
		{
			_removeVisualChild(this);
		}

		protected sealed override Visual GetVisualChild(int index)
		{
			return Children[index];
		}
	}
}
