using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks;

namespace Cvte.Paint.Settings
{
	public class InkBoardSettings : BoardSettings, IUpdateBoardSettings
	{
		private double _inkScale = 1.0;

		private byte _inkOpacity = 128;

		private BitmapImage _inkImage = new BitmapImage();

		private InkType _inkType;

		private double _enLargeInkThickness = 47.0;

		private double _inkThickness = 3.0;

		public static readonly Color DefaultRedColor = Color.FromRgb(byte.MaxValue, 16, 0);

		public static readonly Color DefaultWhiteColor = Colors.White;

		private Color _inkColor = DefaultRedColor;

		public double InkScale
		{
			get
			{
				return _inkScale;
			}
			set
			{
				if (!value.Equals(_inkScale) && !(value <= 0.0) && !double.IsNaN(value) && !double.IsInfinity(value))
				{
					_inkScale = value;
					OnPropertyChanged("InkScale");
				}
			}
		}

		public byte InkOpacity
		{
			get
			{
				return _inkOpacity;
			}
			set
			{
				if (!value.Equals(_inkOpacity))
				{
					_inkOpacity = value;
					OnPropertyChanged("InkOpacity");
				}
			}
		}

		public BitmapImage InkImage
		{
			get
			{
				return _inkImage;
			}
			set
			{
				if (!value.Equals(_inkImage))
				{
					_inkImage = value;
					OnPropertyChanged("InkImage");
				}
			}
		}

		public InkType InkType
		{
			get
			{
				return _inkType;
			}
			set
			{
				if (!value.Equals(_inkType))
				{
					_inkType = value;
					OnPropertyChanged("InkType");
				}
			}
		}

		[Obsolete("其实，没有小伙伴知道这个属性是用来做什么的，图案笔用的也是 InkThickness 所以我也不知道这个属性在哪里使用")]
		public double EnLargeInkThickness
		{
			get
			{
				return _enLargeInkThickness;
			}
			set
			{
				if (!value.Equals(_enLargeInkThickness))
				{
					_enLargeInkThickness = value;
					OnPropertyChanged("EnLargeInkThickness");
				}
			}
		}

		public double InkThickness
		{
			get
			{
				return _inkThickness;
			}
			set
			{
				if (!value.Equals(_inkThickness))
				{
					_inkThickness = value;
					OnPropertyChanged("InkThickness");
					BoardPenSizeChanged?.Invoke(this, new PropertyChangedEventArgs("InkThickness"));
				}
			}
		}

		public Color InkColor
		{
			get
			{
				return _inkColor;
			}
			set
			{
				if (!value.Equals(_inkColor))
				{
					_inkColor = value;
					OnPropertyChanged("InkColor");
					BoardPenColorChanged?.Invoke(this, new PropertyChangedEventArgs("InkColor"));
				}
			}
		}

		[Obsolete("请使用 RecordWritingModeSettings 的 NeedCollectStrokeDetails 方法代替")]
		public bool NeedCollectStrokeDetails { get; set; }

		public event PropertyChangedEventHandler BoardPenColorChanged;

		public event PropertyChangedEventHandler BoardPenSizeChanged;

		public void UpdateBoardSettings(BoardSettings boardSettings)
		{
			InkBoardSettings inkBoardSettings = (InkBoardSettings)boardSettings;
			InkColor = inkBoardSettings.InkColor;
			InkThickness = inkBoardSettings.InkThickness;
			InkScale = inkBoardSettings.InkScale;
			InkImage = inkBoardSettings.InkImage;
		}

		public void SetBoardSettingsFrom(InkBoardSettings settings)
		{
			InkColor = settings.InkColor;
			InkThickness = settings.InkThickness;
			InkScale = settings.InkScale;
			InkImage = settings.InkImage;
			InkOpacity = settings.InkOpacity;
			EnLargeInkThickness = settings.EnLargeInkThickness;
			InkType = settings.InkType;
		}
	}
}
