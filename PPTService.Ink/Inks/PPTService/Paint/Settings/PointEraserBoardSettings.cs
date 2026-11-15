using System.Windows;
using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Settings
{
	public class PointEraserBoardSettings : BoardSettings, IUpdateBoardSettings
	{
		private Size _pointEraserSize = new Size(60.0, 90.0);

		public Size PointEraserSize
		{
			get
			{
				return _pointEraserSize;
			}
			set
			{
				if (!value.Equals(_pointEraserSize))
				{
					_pointEraserSize = value;
					OnPropertyChanged("PointEraserSize");
				}
			}
		}

		public void UpdateBoardSettings(BoardSettings boardSettings)
		{
			PointEraserSize = ((PointEraserBoardSettings)boardSettings).PointEraserSize;
		}
	}
}
