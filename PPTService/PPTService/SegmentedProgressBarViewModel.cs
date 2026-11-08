using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PPTService
{
	public class SegmentedProgressBarViewModel : CircularProgressBarViewModel
	{
		public class SegmentData : INotifyPropertyChanged
		{
			private double _opacity;

			public double StartAngle { get; private set; }

			public double WedgeAngle { get; private set; }

			public double Opacity
			{
				get
				{
					return _opacity;
				}
				set
				{
					_opacity = value;
					OnPropertyChanged("Opacity");
				}
			}

			public SegmentedProgressBarViewModel Parent { get; private set; }

			public event PropertyChangedEventHandler PropertyChanged;

			public SegmentData(double start, double wedge, SegmentedProgressBarViewModel parent)
			{
				StartAngle = start;
				WedgeAngle = wedge;
				Parent = parent;
				Opacity = 0.0;
			}

			protected void OnPropertyChanged(string property)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property));
				}
			}
		}

		private int _segmentCount = 8;

		private List<SegmentData> _segments;

		public int SegmentCount
		{
			get
			{
				return _segmentCount;
			}
			set
			{
				_segmentCount = value;
				BuildSegments();
				ComputeViewModelProperties();
			}
		}

		public List<SegmentData> Segments
		{
			get
			{
				return _segments;
			}
			set
			{
				_segments = value;
				OnPropertyChanged("Segments");
			}
		}

		public SegmentedProgressBarViewModel()
		{
			BuildSegments();
		}

		private void BuildSegments()
		{
			List<SegmentData> list = new List<SegmentData>();
			double wedge = 360.0 / (double)SegmentCount;
			for (int i = 0; i < SegmentCount; i++)
			{
				double start = (double)i * 360.0 / (double)SegmentCount;
				list.Add(new SegmentData(start, wedge, this));
			}
			Segments = list;
		}

		public override void ComputeViewModelProperties()
		{
			if (_progressBar != null)
			{
				double num = _progressBar.Value / (_progressBar.Maximum - _progressBar.Minimum);
				for (int i = 0; i < SegmentCount; i++)
				{
					double num2 = (double)i / (double)SegmentCount;
					double num3 = (double)(i + 1) / (double)SegmentCount;
					double val = (num - num2) / (num3 - num2);
					val = Math.Min(1.0, Math.Max(0.0, val));
					_segments[i].Opacity = val;
				}
				base.ComputeViewModelProperties();
			}
		}
	}
}
