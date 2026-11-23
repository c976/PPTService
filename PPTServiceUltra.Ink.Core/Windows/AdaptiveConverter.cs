using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PPTServiceUltra.Ink.Core.Windows
{
	internal class AdaptiveConverter : DependencyObject, IValueConverter
	{
		internal AdaptiveUI Adaptive { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			AdaptiveUI.SetAdaptive((DependencyObject)value, CreateResourceCopy());
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private AdaptiveUI CreateResourceCopy()
		{
			AdaptiveUI adaptiveUI = new AdaptiveUI();
			foreach (AdaptiveCondition trigger in Adaptive.Triggers)
			{
				AdaptiveCondition adaptiveCondition = new AdaptiveCondition();
				foreach (SetterBase setter in trigger.Setters)
				{
					adaptiveCondition.Setters.Add(setter);
				}
				adaptiveCondition.MinWindowWidth = trigger.MinWindowWidth;
				adaptiveCondition.MinWindowHeight = trigger.MinWindowHeight;
				adaptiveCondition.MinWindowSize = trigger.MinWindowSize;
				foreach (TriggerAction enterAction in trigger.EnterActions)
				{
					adaptiveCondition.EnterActions.Add(enterAction);
				}
				foreach (TriggerAction exitAction in trigger.ExitActions)
				{
					adaptiveCondition.ExitActions.Add(exitAction);
				}
				adaptiveUI.Triggers.Add(adaptiveCondition);
			}
			return adaptiveUI;
		}
	}
}
