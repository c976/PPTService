using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;

namespace Cvte.Paint.Framework
{
	public interface IStylusPlugInSource : IInputElement
	{
		event MouseButtonEventHandler PreviewMouseDown;

		event MouseButtonEventHandler MouseDown;

		event MouseButtonEventHandler PreviewMouseUp;

		event MouseButtonEventHandler MouseUp;

		event EventHandler<TouchEventArgs> PreviewTouchDown;

		event EventHandler<TouchEventArgs> TouchDown;

		event EventHandler<TouchEventArgs> PreviewTouchMove;

		event EventHandler<TouchEventArgs> TouchMove;

		event EventHandler<TouchEventArgs> PreviewTouchUp;

		event EventHandler<TouchEventArgs> TouchUp;

		event EventHandler<ManipulationStartingEventArgs> ManipulationStarting;

		event EventHandler<ManipulationStartedEventArgs> ManipulationStarted;

		event EventHandler<ManipulationDeltaEventArgs> ManipulationDelta;

		event EventHandler<ManipulationInertiaStartingEventArgs> ManipulationInertiaStarting;

		event EventHandler<ManipulationBoundaryFeedbackEventArgs> ManipulationBoundaryFeedback;

		event EventHandler<ManipulationCompletedEventArgs> ManipulationCompleted;

		void AddStylusPlugIns(StylusPlugIn stylusPlugIns);

		void RemoveStylusPlugIns(StylusPlugIn stylusPlugIns);

		bool ContainsStylusPlugIns(StylusPlugIn stylusPlugIns);
	}
}
