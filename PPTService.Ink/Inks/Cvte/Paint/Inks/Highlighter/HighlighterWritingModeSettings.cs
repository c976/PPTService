using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Inks.Highlighter
{
	public class HighlighterWritingModeSettings : BoardSettings
	{
		private double _widthHeightRatio = 1.0;

		private double _highlighterOpacity = 0.3;

		public const double DefaultHighlighterOpacity = 0.3;

		public double HighlighterOpacity
		{
			get
			{
				return _highlighterOpacity;
			}
			set
			{
				if (!value.Equals(_highlighterOpacity))
				{
					_highlighterOpacity = value;
					OnPropertyChanged("HighlighterOpacity");
				}
			}
		}

		public double WidthHeightRatio
		{
			get
			{
				return _widthHeightRatio;
			}
			set
			{
				if (!value.Equals(_widthHeightRatio))
				{
					_widthHeightRatio = value;
					OnPropertyChanged("WidthHeightRatio");
				}
			}
		}
	}
}
