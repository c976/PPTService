using System;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Framework.Elements
{
	public class ElementGroupOperation : ElementOperation
	{
		public ElementGroupingResult ElementGroupResult { get; }

		public ElementGroupOperation(ElementGroupingResult elementGroupResult)
			: base(elementGroupResult.SlideId)
		{
			if (string.IsNullOrEmpty(elementGroupResult.SlideId))
			{
				throw new ArgumentNullException("如果传入的元素组合没有在页面上，是不支持撤销重做");
			}
			ElementGroupResult = elementGroupResult;
		}

		public new Slide GetSlide()
		{
			return base.GetSlide();
		}

		protected override void OnRedo()
		{
			ElementGroupResult.Redo(this);
		}

		protected override void OnUndo()
		{
			ElementGroupResult.Undo(this);
		}
	}
}
