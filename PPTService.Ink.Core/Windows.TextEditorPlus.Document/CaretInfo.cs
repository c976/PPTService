using System;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class CaretInfo
	{
		private Rect _bounds = Rect.Empty;

		private Brush _caretBrush = Brushes.Black;

		private CaretOffset _currentCaretOffset = CaretOffset.Default;

		private ArrangingType _arrangingType;

		private double _caretScale = 1.0;

		public CaretOffset CurrentCaretOffset
		{
			get
			{
				return _currentCaretOffset;
			}
			internal set
			{
				CaretOffset currentCaretOffset = _currentCaretOffset;
				_currentCaretOffset = value;
				if (!currentCaretOffset.Equals(value))
				{
					OffsetChanged?.Invoke(this, new ValueChangeEventArgs<CaretOffset>(currentCaretOffset, value));
				}
			}
		}

		public Brush Brush => _caretBrush;

		public Rect Bounds => _bounds;

		public ArrangingType ArrangingType
		{
			get
			{
				return _arrangingType;
			}
			internal set
			{
				ArrangingType arrangingType = _arrangingType;
				_arrangingType = value;
				if (arrangingType != value)
				{
					ArrangingTypeChanged?.Invoke(this, new ValueChangeEventArgs<ArrangingType>(arrangingType, value));
				}
			}
		}

		public double Weight => SystemCaretWidth * CaretScale;

		public double CaretScale
		{
			get
			{
				return _caretScale;
			}
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentOutOfRangeException("CaretScale", "光标缩放比必须大于0");
				}
				_caretScale = value;
			}
		}

		private double SystemCaretWidth
		{
			get
			{
				if (!(SystemParameters.CaretWidth > 1.0))
				{
					return 1.0;
				}
				return SystemParameters.CaretWidth;
			}
		}

		public bool IsAutoBrush { get; internal set; } = true;

		public event ValueChangedEventHandler<CaretOffset> OffsetChanged;

		public event ValueChangedEventHandler<Brush> BrushChanged;

		public event ValueChangedEventHandler<ArrangingType> ArrangingTypeChanged;

		public event ValueChangedEventHandler<Rect> BoundsChanged;

		internal void UpdateCaretBrush(Brush brush, bool withNotify = false)
		{
			if (withNotify)
			{
				Brush caretBrush = _caretBrush;
				_caretBrush = brush;
				if (!Converter.AreEquals(caretBrush, brush))
				{
					BrushChanged?.Invoke(this, new ValueChangeEventArgs<Brush>(caretBrush, brush));
				}
			}
			else
			{
				_caretBrush = brush;
			}
		}

		internal void UpdateCaretBounds(Rect bounds, bool withNotify = false)
		{
			if (withNotify)
			{
				Rect bounds2 = _bounds;
				_bounds = bounds;
				if (bounds2 != bounds)
				{
					BoundsChanged?.Invoke(this, new ValueChangeEventArgs<Rect>(bounds2, bounds));
				}
			}
			else
			{
				_bounds = bounds;
			}
		}
	}
}
