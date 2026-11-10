using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Cvte.Windows.Controls;

namespace Cvte.Windows.Data
{
	public static class ValueConverters
	{
		private static BooleanToVisibilityConverter _booleanToVisibilityConverter;

		private static BooleanReverseConverter _booleanReverseConverter;

		private static VisibilityConverter _visibilityReverseConverter;

		private static VisibilityConverter _hiddenWhenFalse;

		private static VisibilityConverter _collapsedWhenFalse;

		private static VisibilityConverter _hiddenWhenTrue;

		private static VisibilityConverter _collapsedWhenTrue;

		public static IValueConverter BooleanReverse => _booleanReverseConverter ?? (_booleanReverseConverter = new BooleanReverseConverter());

		public static IValueConverter BooleanToVisibility => _booleanToVisibilityConverter ?? (_booleanToVisibilityConverter = new BooleanToVisibilityConverter());

		public static IValueConverter VisibilityReverse
		{
			get
			{
				VisibilityConverter visibilityConverter = _visibilityReverseConverter;
				if (visibilityConverter == null)
				{
					VisibilityConverter visibilityConverter2 = new VisibilityConverter();
					visibilityConverter2.Visible = Visibility.Collapsed;
					visibilityConverter2.Collapsed = Visibility.Visible;
					visibilityConverter2.Hidden = Visibility.Visible;
					visibilityConverter = (_visibilityReverseConverter = visibilityConverter2);
				}
				return visibilityConverter;
			}
		}

		public static IValueConverter HiddenWhenFalse
		{
			get
			{
				VisibilityConverter visibilityConverter = _hiddenWhenFalse;
				if (visibilityConverter == null)
				{
					VisibilityConverter visibilityConverter2 = new VisibilityConverter();
					visibilityConverter2.Visible = true;
					visibilityConverter2.Hidden = false;
					visibilityConverter = (_hiddenWhenFalse = visibilityConverter2);
				}
				return visibilityConverter;
			}
		}

		public static IValueConverter CollapsedWhenFalse
		{
			get
			{
				VisibilityConverter visibilityConverter = _collapsedWhenFalse;
				if (visibilityConverter == null)
				{
					VisibilityConverter visibilityConverter2 = new VisibilityConverter();
					visibilityConverter2.Visible = true;
					visibilityConverter2.Collapsed = false;
					visibilityConverter = (_collapsedWhenFalse = visibilityConverter2);
				}
				return visibilityConverter;
			}
		}

		public static IValueConverter HiddenWhenTrue
		{
			get
			{
				VisibilityConverter visibilityConverter = _hiddenWhenTrue;
				if (visibilityConverter == null)
				{
					VisibilityConverter visibilityConverter2 = new VisibilityConverter();
					visibilityConverter2.Visible = false;
					visibilityConverter2.Hidden = true;
					visibilityConverter = (_hiddenWhenTrue = visibilityConverter2);
				}
				return visibilityConverter;
			}
		}

		public static IValueConverter CollapsedWhenTrue
		{
			get
			{
				VisibilityConverter visibilityConverter = _collapsedWhenTrue;
				if (visibilityConverter == null)
				{
					VisibilityConverter visibilityConverter2 = new VisibilityConverter();
					visibilityConverter2.Visible = false;
					visibilityConverter2.Collapsed = true;
					visibilityConverter = (_collapsedWhenTrue = visibilityConverter2);
				}
				return visibilityConverter;
			}
		}
	}
}
