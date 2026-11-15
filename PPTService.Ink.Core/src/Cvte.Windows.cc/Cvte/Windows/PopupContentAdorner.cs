using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows
{
	[ContentProperty("Child")]
	internal class PopupContentAdorner : Adorner
	{
		private UIElement _child;

		public IPopupRootContent Child
		{
			get
			{
				return (IPopupRootContent)_child;
			}
			set
			{
				if (!object.Equals(_child, value))
				{
					if (_child != null)
					{
						RemoveVisualChild(_child);
					}
					_child = (UIElement)value;
					if (_child != null)
					{
						AddVisualChild(_child);
					}
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get
			{
				if (_child != null)
				{
					return 1;
				}
				return 0;
			}
		}

		public PopupContentAdorner([NotNull] AdornerDecorator decorator)
			: base(decorator.Child)
		{
		}

		protected override Visual GetVisualChild(int index)
		{
			return _child;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_child?.Arrange(new Rect(finalSize));
			return finalSize;
		}
	}
}
