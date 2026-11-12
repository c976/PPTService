using System.Windows;

namespace Cvte.Paint.Performance
{
	public class UIPerformance
	{
		public static readonly DependencyProperty PerformanceCollectorProperty = DependencyProperty.RegisterAttached("PerformanceCollector", typeof(IPerformanceCollector), typeof(UIPerformance), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetPerformanceCollector(DependencyObject element, IPerformanceCollector value)
		{
			value = value ?? new EmptyPerformanceCollector();
			element.SetValue(PerformanceCollectorProperty, value);
		}

		public static IPerformanceCollector GetPerformanceCollector(DependencyObject element)
		{
			IPerformanceCollector performanceCollector = (IPerformanceCollector)element.GetValue(PerformanceCollectorProperty);
			if (performanceCollector == null)
			{
				performanceCollector = new EmptyPerformanceCollector();
				element.SetValue(PerformanceCollectorProperty, performanceCollector);
			}
			return performanceCollector;
		}
	}
}
