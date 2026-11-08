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
				VisibilityConverter visibilityConverter2;
				if (visibilityConverter == null)
				{
					if (0 == 0)
					{
						visibilityConverter2 = new VisibilityConverter();
					}
					if (2 != 0)
					{
						visibilityConverter2.Visible = Visibility.Collapsed;
						goto IL_001d;
					}
					goto IL_0037;
				}
				goto IL_0041;
				IL_0037:
				if (2 == 0)
				{
					goto IL_001d;
				}
				visibilityConverter = (_visibilityReverseConverter = visibilityConverter2);
				goto IL_0041;
				IL_0041:
				return visibilityConverter;
				IL_001d:
				do
				{
					if (0 == 0)
					{
						visibilityConverter2.Collapsed = Visibility.Visible;
					}
				}
				while (3 == 0);
				visibilityConverter2.Hidden = Visibility.Visible;
				goto IL_0037;
			}
		}

		public static IValueConverter HiddenWhenFalse
		{
			get
			{
				VisibilityConverter visibilityConverter = _hiddenWhenFalse;
				if (visibilityConverter == null)
				{
					VisibilityConverter visibilityConverter2;
					do
					{
						visibilityConverter2 = new VisibilityConverter();
					}
					while (false);
					visibilityConverter2.Visible = true;
					object hidden = false;
					if (0 == 0)
					{
						visibilityConverter2.Hidden = hidden;
					}
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
					VisibilityConverter visibilityConverter2;
					do
					{
						visibilityConverter2 = new VisibilityConverter();
					}
					while (false);
					visibilityConverter2.Visible = true;
					object collapsed = false;
					if (0 == 0)
					{
						visibilityConverter2.Collapsed = collapsed;
					}
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
					VisibilityConverter visibilityConverter2;
					do
					{
						visibilityConverter2 = new VisibilityConverter();
					}
					while (false);
					visibilityConverter2.Visible = false;
					object hidden = true;
					if (0 == 0)
					{
						visibilityConverter2.Hidden = hidden;
					}
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
					VisibilityConverter visibilityConverter2;
					do
					{
						visibilityConverter2 = new VisibilityConverter();
					}
					while (false);
					visibilityConverter2.Visible = false;
					object collapsed = true;
					if (0 == 0)
					{
						visibilityConverter2.Collapsed = collapsed;
					}
					visibilityConverter = (_collapsedWhenTrue = visibilityConverter2);
				}
				return visibilityConverter;
			}
		}
	}
}
