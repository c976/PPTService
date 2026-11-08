using System;
using System.ComponentModel;

namespace PPTService
{
	public class CircularProgressBarViewModel : AttachedViewModelBase
	{
		private double _angle;

		private double _centreX;

		private double _centreY;

		private double _radius;

		private double _innerRadius;

		private double _diameter;

		private double _percent;

		private double _holeSizeFactor;

		public double Percent
		{
			get
			{
				return _percent;
			}
			set
			{
				_percent = value;
				OnPropertyChanged("Percent");
			}
		}

		public double Diameter
		{
			get
			{
				return _diameter;
			}
			set
			{
				_diameter = value;
				OnPropertyChanged("Diameter");
			}
		}

		public double Radius
		{
			get
			{
				return _radius;
			}
			set
			{
				_radius = value;
				OnPropertyChanged("Radius");
			}
		}

		public double InnerRadius
		{
			get
			{
				return _innerRadius;
			}
			set
			{
				_innerRadius = value;
				OnPropertyChanged("InnerRadius");
			}
		}

		public double CentreX
		{
			get
			{
				return _centreX;
			}
			set
			{
				_centreX = value;
				OnPropertyChanged("CentreX");
			}
		}

		public double CentreY
		{
			get
			{
				return _centreY;
			}
			set
			{
				_centreY = value;
				OnPropertyChanged("CentreY");
			}
		}

		public double Angle
		{
			get
			{
				return _angle;
			}
			set
			{
				_angle = value;
				OnPropertyChanged("Angle");
			}
		}

		public double HoleSizeFactor
		{
			get
			{
				return _holeSizeFactor;
			}
			set
			{
				_holeSizeFactor = value;
				ComputeViewModelProperties();
			}
		}

		public override void ComputeViewModelProperties()
		{
			if (_progressBar != null)
			{
				Angle = (_progressBar.Value - _progressBar.Minimum) * 360.0 / (_progressBar.Maximum - _progressBar.Minimum);
				CentreX = _progressBar.ActualWidth / 2.0;
				CentreY = _progressBar.ActualHeight / 2.0;
				Radius = Math.Min(CentreX, CentreY);
				Diameter = Radius * 2.0;
				InnerRadius = Radius * HoleSizeFactor;
				Percent = Angle / 360.0;
			}
		}

		protected override void AdaptedDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}
	}
}
