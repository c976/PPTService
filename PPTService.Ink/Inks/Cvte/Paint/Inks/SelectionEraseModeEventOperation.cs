using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Windows.Input;

namespace Cvte.Paint.Inks
{
	internal sealed class SelectionEraseModeEventOperation : SelectionForBoardModeOperationBase
	{
		protected override void Tapped(Point position, DeviceType deviceType)
		{
			base.Tapped(position, deviceType);
			EraseEventHelper.OnSelectionEraseInvoked(this, new SelectionEraseEventArgs(SelectionEraseEventArgs.SelectionEraseType.Point)
			{
				AttachedSlide = CurrentSlide,
				TappedPosition = position,
				DeviceType = deviceType
			});
		}

		protected override void DragSelected(Geometry containedGeometry)
		{
			base.DragSelected(containedGeometry);
			EraseEventHelper.OnSelectionEraseInvoked(this, new SelectionEraseEventArgs(SelectionEraseEventArgs.SelectionEraseType.Path)
			{
				AttachedSlide = CurrentSlide,
				ContainedGeometry = containedGeometry
			});
		}

		protected override void DragSelected(Rect containedBounds)
		{
			base.DragSelected(containedBounds);
			EraseEventHelper.OnSelectionEraseInvoked(this, new SelectionEraseEventArgs(SelectionEraseEventArgs.SelectionEraseType.Rect)
			{
				AttachedSlide = CurrentSlide,
				ContainedBounds = containedBounds
			});
		}

		protected override void ClearSelection()
		{
		}

		protected override void TapSelect(Element element)
		{
		}

		protected override void DragSelect(ICollection<Element> elements)
		{
		}
	}
}
