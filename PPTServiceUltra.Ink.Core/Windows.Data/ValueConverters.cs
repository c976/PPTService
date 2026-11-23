using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PPTServiceUltra.Ink.Core.Windows.Controls;

namespace PPTServiceUltra.Ink.Core.Windows.Data
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
					visibilityConverter = new VisibilityConverter
					{
						Visible = Visibility.Collapsed,
						Collapsed = Visibility.Visible,
						Hidden = Visibility.Visible
					};
					_visibilityReverseConverter = visibilityConverter;
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
					visibilityConverter = new VisibilityConverter
					{
						Visible = true,
						Hidden = false
					};
					_hiddenWhenFalse = visibilityConverter;
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
					visibilityConverter = new VisibilityConverter
					{
						Visible = true,
						Collapsed = false
					};
					_collapsedWhenFalse = visibilityConverter;
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
					visibilityConverter = new VisibilityConverter
					{
						Visible = false,
						Hidden = true
					};
					_hiddenWhenTrue = visibilityConverter;
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
					visibilityConverter = new VisibilityConverter
					{
						Visible = false,
						Collapsed = true
					};
					_collapsedWhenTrue = visibilityConverter;
				}
				return visibilityConverter;
			}
		}
	}
}
