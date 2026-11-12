using System.Windows.Media;

namespace Cvte.Paint.Framework.Settings
{
	public class SelectionStyleSettings : BoardSettings
	{
		private Color _color = Color.FromRgb(0, 133, 205);

		private Pen _rectPen;

		private Brush _rectFill;

		private Pen _pathPen;

		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
				if (SetValue(ref _color, value, "Color"))
				{
					_rectPen = null;
					_rectFill = null;
					_pathPen = null;
				}
			}
		}

		public Pen RectPen
		{
			get
			{
				if (_rectPen == null)
				{
					SolidColorBrush solidColorBrush = new SolidColorBrush(Color);
					solidColorBrush.Freeze();
					double roundDpiScaledThickness = SnapDrawingExtensions.GetRoundDpiScaledThickness(1.0);
					Pen pen = new Pen(solidColorBrush, roundDpiScaledThickness);
					pen.DashCap = PenLineCap.Square;
					pen.DashStyle = new DashStyle(new DoubleCollection(new double[2] { 4.0, 2.0 }), 0.0);
					Pen pen2 = pen;
					pen2.Freeze();
					_rectPen = pen2;
				}
				return _rectPen;
			}
			set
			{
				SetValue(ref _rectPen, value, "RectPen");
			}
		}

		public Brush RectFill
		{
			get
			{
				if (_rectFill == null)
				{
					SolidColorBrush solidColorBrush = new SolidColorBrush(Color)
					{
						Opacity = 0.2
					};
					solidColorBrush.Freeze();
					_rectFill = solidColorBrush;
				}
				return _rectFill;
			}
			set
			{
				_rectFill = value;
			}
		}

		public Pen PathPen
		{
			get
			{
				if (_pathPen == null)
				{
					SolidColorBrush solidColorBrush = new SolidColorBrush(Color);
					solidColorBrush.Freeze();
					Pen pen = new Pen(solidColorBrush, 5.0);
					pen.DashCap = PenLineCap.Round;
					pen.DashStyle = new DashStyle(new double[2] { 0.0, 1.0 }, -0.5);
					Pen pen2 = pen;
					pen2.Freeze();
					_pathPen = pen2;
				}
				return _pathPen;
			}
			set
			{
				_pathPen = value;
			}
		}
	}
}
