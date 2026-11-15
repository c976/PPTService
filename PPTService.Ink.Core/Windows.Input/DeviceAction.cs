using System.Windows;

namespace PPTService.Ink.Core.Windows.Input
{
	public abstract class DeviceAction : DependencyObject
	{
		protected DependencyObject AssociatedObject { get; private set; }

		public void Attach(DependencyObject associatedObject)
		{
			AssociatedObject = associatedObject;
			OnAttached();
		}

		public void Detach()
		{
			try
			{
				OnDetaching();
			}
			finally
			{
				AssociatedObject = null;
			}
		}

		protected abstract void OnAttached();

		protected virtual void OnDetaching()
		{
		}
	}
}
