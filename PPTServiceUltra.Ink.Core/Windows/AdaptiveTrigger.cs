using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class AdaptiveTrigger : DataTrigger
	{
		private AdaptiveUI _adaptive;

		public AdaptiveUI Adaptive
		{
			get
			{
				return _adaptive;
			}
			set
			{
				AdaptiveConverter adaptiveConverter = Target?.Converter as AdaptiveConverter;
				if (adaptiveConverter != null)
				{
					adaptiveConverter.Adaptive = value;
				}
				_adaptive = value;
			}
		}

		[DependsOn("Adaptive")]
		public Binding Target
		{
			get
			{
				return (Binding)Binding;
			}
			set
			{
				value.Converter = new AdaptiveConverter
				{
					Adaptive = Adaptive
				};
				Binding = value;
			}
		}

		public AdaptiveTrigger()
		{
			Value = false;
			Setters.Add(new Setter(FrameworkElement.TagProperty, null));
		}
	}
}
