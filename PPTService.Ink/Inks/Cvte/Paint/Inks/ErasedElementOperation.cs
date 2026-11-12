using System.Collections.Generic;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Inks
{
	internal class ErasedElementOperation : ElementOperation
	{
		private readonly IReadOnlyList<EraseOperationInfo> _elementsEraseOperationInfos;

		public ErasedElementOperation(Slide slide, IReadOnlyList<EraseOperationInfo> infos)
			: base(slide)
		{
			_elementsEraseOperationInfos = infos;
		}

		protected override void OnRedo()
		{
			if (_elementsEraseOperationInfos == null)
			{
				return;
			}
			foreach (EraseOperationInfo elementsEraseOperationInfo in _elementsEraseOperationInfos)
			{
				elementsEraseOperationInfo.Element.ClipGeometry = elementsEraseOperationInfo.NewGeometry;
			}
		}

		protected override void OnUndo()
		{
			if (_elementsEraseOperationInfos == null)
			{
				return;
			}
			foreach (EraseOperationInfo elementsEraseOperationInfo in _elementsEraseOperationInfos)
			{
				elementsEraseOperationInfo.Element.ClipGeometry = elementsEraseOperationInfo.OldGeometry;
			}
		}
	}
}
