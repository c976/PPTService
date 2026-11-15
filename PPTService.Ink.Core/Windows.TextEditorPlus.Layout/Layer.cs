using System.Windows;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	public class Layer : UIElement
	{
		public KnownLayer KnownLayer { get; }

		static Layer()
		{
			UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(Layer), new UIPropertyMetadata(false));
		}

		public Layer(KnownLayer knownLayer)
		{
			KnownLayer = knownLayer;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}

		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return null;
		}
	}
}
