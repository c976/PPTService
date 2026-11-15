using System.Windows;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.Threading
{
	internal class VisualTargetPresentationSource : PresentationSource
	{
		private VisualTarget _visualTarget;

		public override Visual RootVisual
		{
			get
			{
				return _visualTarget.RootVisual;
			}
			set
			{
				Visual rootVisual = _visualTarget.RootVisual;
				_visualTarget.RootVisual = value;
				RootChanged(rootVisual, value);
			}
		}

		public override bool IsDisposed => false;

		public VisualTargetPresentationSource(HostVisual hostVisual)
		{
			_visualTarget = new VisualTarget(hostVisual);
		}

		protected override CompositionTarget GetCompositionTargetCore()
		{
			return _visualTarget;
		}
	}
}
