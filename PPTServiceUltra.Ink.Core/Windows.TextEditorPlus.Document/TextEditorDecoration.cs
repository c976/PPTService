using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public abstract class TextEditorDecoration : IEquatable<TextEditorDecoration>
	{
		private Rect? _currentDecorationBounds;

		public TextDecorationLocation TextDecorationLocation { get; }

		protected TextEditorDecoration(TextDecorationLocation textDecorationLocation)
		{
			TextDecorationLocation = textDecorationLocation;
		}

		public abstract Drawing OnBuildDecoration(Rect bounds, Brush foreground);

		public Drawing BuildDecoration(Rect bounds, Brush foreground)
		{
			_currentDecorationBounds = bounds;
			return OnBuildDecoration(bounds, foreground);
		}

		public void HandleMouseMove(Point position)
		{
			Rect? currentDecorationBounds = _currentDecorationBounds;
			if (currentDecorationBounds.HasValue)
			{
				Hover(position, _currentDecorationBounds.Value);
			}
		}

		protected virtual void Hover(Point position, Rect decorationBounds)
		{
		}

		public virtual bool Equals(TextEditorDecoration other)
		{
			if (other == null)
			{
				return false;
			}
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((TextEditorDecoration)obj);
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
	}
}
