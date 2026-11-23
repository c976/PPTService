using System.Windows;
using System.Windows.Markup;

namespace PPTServiceUltra.Ink.Core.Windows
{
	[ContentProperty("Setters")]
	public class AdaptiveCondition : DependencyObject
	{
		public static readonly DependencyProperty MinWindowWidthProperty = DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(AdaptiveCondition), new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty MinWindowHeightProperty = DependencyProperty.Register("MinWindowHeight", typeof(double), typeof(AdaptiveCondition), new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty MinWindowSizeProperty = DependencyProperty.Register("MinWindowSize", typeof(Size), typeof(AdaptiveCondition), new PropertyMetadata(Size.Empty));

		public double MinWindowWidth
		{
			get
			{
				return (double)GetValue(MinWindowWidthProperty);
			}
			set
			{
				SetValue(MinWindowWidthProperty, value);
			}
		}

		public double MinWindowHeight
		{
			get
			{
				return (double)GetValue(MinWindowHeightProperty);
			}
			set
			{
				SetValue(MinWindowHeightProperty, value);
			}
		}

		public Size MinWindowSize
		{
			get
			{
				return (Size)GetValue(MinWindowSizeProperty);
			}
			set
			{
				SetValue(MinWindowSizeProperty, value);
			}
		}

		public SetterBaseCollection Setters { get; }

		public TriggerActionCollection EnterActions { get; }

		public TriggerActionCollection ExitActions { get; }

		public AdaptiveCondition()
			: this(new SetterBaseCollection())
		{
		}

		internal AdaptiveCondition(SetterBaseCollection setters)
		{
			Setters = setters;
			EnterActions = new TriggerActionCollection();
			ExitActions = new TriggerActionCollection();
		}
	}
}
