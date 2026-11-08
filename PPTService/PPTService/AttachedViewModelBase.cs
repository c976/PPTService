using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LinqToVisualTree;

namespace PPTService
{
	public abstract class AttachedViewModelBase : FrameworkElement, INotifyPropertyChanged
	{
		public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(object), typeof(AttachedViewModelBase), new PropertyMetadata(null, OnAttachChanged));

		public static readonly DependencyProperty DataContextPiggyBackProperty = DependencyProperty.RegisterAttached("DataContextPiggyBack", typeof(object), typeof(AttachedViewModelBase), new PropertyMetadata(null, OnDataContextPiggyBackChanged));

		protected ProgressBar _progressBar;

		private PropertyChangeNotifier valueNotifier;

		private PropertyChangeNotifier maximumNotifier;

		private PropertyChangeNotifier minimumNotifier;

		private FrameworkElement _element;

		public FrameworkElement AttachedElement
		{
			get
			{
				return _element;
			}
			set
			{
				_element = value;
				_element.SizeChanged += AttachedElement_SizeChanged;
			}
		}

		public double ElementWidth => _element.ActualWidth;

		public double ElementHeight => _element.ActualHeight;

		public event PropertyChangedEventHandler PropertyChanged;

		public static AttachedViewModelBase GetAttach(DependencyObject d)
		{
			return (AttachedViewModelBase)d.GetValue(AttachProperty);
		}

		public static void SetAttach(DependencyObject d, AttachedViewModelBase value)
		{
			d.SetValue(AttachProperty, value);
		}

		private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement frameworkElement = d as FrameworkElement;
			(e.NewValue as AttachedViewModelBase).AttachedElement = frameworkElement;
			frameworkElement.Loaded += Element_Loaded;
		}

		private static void Element_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement obj = sender as FrameworkElement;
			FrameworkElement source = obj.Parent as FrameworkElement;
			AttachedViewModelBase attachedViewModelBase = (AttachedViewModelBase)(obj.DataContext = GetAttach(obj));
			ProgressBar progressBar = obj.Ancestors<ProgressBar>().Single() as ProgressBar;
			attachedViewModelBase.SetProgressBar(progressBar);
			attachedViewModelBase.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext")
			{
				Source = source
			});
			attachedViewModelBase.SetBinding(DataContextPiggyBackProperty, new Binding("DataContext")
			{
				Source = source
			});
		}

		public static object GetDataContextPiggyBack(DependencyObject d)
		{
			return d.GetValue(DataContextPiggyBackProperty);
		}

		public static void SetDataContextPiggyBack(DependencyObject d, object value)
		{
			d.SetValue(DataContextPiggyBackProperty, value);
		}

		private static void OnDataContextPiggyBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as AttachedViewModelBase).OnDataContextChanged();
		}

		private void OnDataContextChanged()
		{
			INotifyPropertyChanged notifyPropertyChanged = DataContext as INotifyPropertyChanged;
			if (notifyPropertyChanged != null)
			{
				notifyPropertyChanged.PropertyChanged += AdaptedDataContextPropertyChanged;
			}
			OnPropertyChanged("");
		}

		protected abstract void AdaptedDataContextPropertyChanged(object sender, PropertyChangedEventArgs e);

		public virtual void ComputeViewModelProperties()
		{
		}

		private void SetProgressBar(ProgressBar progressBar)
		{
			_progressBar = progressBar;
			_progressBar.SizeChanged += (object s, SizeChangedEventArgs e) =>
			{
				ComputeViewModelProperties();
			};
			valueNotifier = new PropertyChangeNotifier(_progressBar, "Value");
			valueNotifier.ValueChanged += valueNotifier_ValueChanged;
			maximumNotifier = new PropertyChangeNotifier(_progressBar, "Maximum");
			maximumNotifier.ValueChanged += maximumNotifier_ValueChanged;
			minimumNotifier = new PropertyChangeNotifier(_progressBar, "Minimum");
			minimumNotifier.ValueChanged += minimumNotifier_ValueChanged;
			ComputeViewModelProperties();
		}

		private void minimumNotifier_ValueChanged(object sender, EventArgs e)
		{
			ComputeViewModelProperties();
		}

		private void maximumNotifier_ValueChanged(object sender, EventArgs e)
		{
			ComputeViewModelProperties();
		}

		private void valueNotifier_ValueChanged(object sender, EventArgs e)
		{
			ComputeViewModelProperties();
		}

		public AttachedViewModelBase()
		{
		}

		protected void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		protected virtual void AttachedElement_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			OnPropertyChanged("ElementHeight");
			OnPropertyChanged("ElementWidth");
		}
	}
}
