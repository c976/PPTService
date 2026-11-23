using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public class ContentAdorner : Adorner
	{
		private readonly VisualCollection _visuals;

		private readonly ContentPresenter _contentPresenter;

		public object Content
		{
			get
			{
				return _contentPresenter.Content;
			}
			set
			{
				_contentPresenter.Content = value;
			}
		}

		protected override int VisualChildrenCount => _visuals.Count;

		public ContentAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
			_visuals = new VisualCollection(this);
			_contentPresenter = new ContentPresenter();
			_visuals.Add(_contentPresenter);
		}

		public ContentAdorner(UIElement adornedElement, Visual content)
			: this(adornedElement)
		{
			Content = content;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_contentPresenter.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
			return _contentPresenter.RenderSize;
		}

		protected override Visual GetVisualChild(int index)
		{
			return _visuals[index];
		}
	}
}
