using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework
{
	internal class SelectionTranslateTransformer : SelectionPartitionTransformer
	{
		private Rect _originBounds;

		private Rect _originAlignBounds;

		private Rect _originWholeAlignBounds;

		private bool _altPressed;

		private bool _ctrlPressed;

		protected override void OnStarted()
		{
			_originBounds = Owner.Bounds;
			_originAlignBounds = Aligner.GetAlignmentBounds(Owner.Elements);
			_originWholeAlignBounds = Aligner.GetAlignmentBounds(TransformingElements);
			IAlignmentEngine aligner = Aligner;
			Slide slide = Slide;
			IEnumerable<Element> staticElements;
			if (!SelectionPartitionTransformer.WithCtrl)
			{
				staticElements = Slide.Elements.Except(TransformingElements);
			}
			else
			{
				IEnumerable<Element> elements = Slide.Elements;
				staticElements = elements;
			}
			aligner.Prepair(slide, staticElements, TransformingElements);
		}

		protected override void OnDelta(Vector userOffset, object[] args)
		{
			Rect desiredBounds = new Rect(_originBounds.Left + userOffset.X, _originBounds.Top + userOffset.Y, _originBounds.Width, _originBounds.Height);
			Rect rect = new Rect(_originAlignBounds.Left + userOffset.X, _originAlignBounds.Top + userOffset.Y, _originAlignBounds.Width, _originAlignBounds.Height);
			Rect rect2 = new Rect(_originWholeAlignBounds.X + userOffset.X, _originWholeAlignBounds.Top + userOffset.Y, _originWholeAlignBounds.Width, _originWholeAlignBounds.Height);
			if (SelectionPartitionTransformer.WithAlt)
			{
				if (!_altPressed)
				{
					Aligner.Clear();
					_altPressed = true;
				}
			}
			else
			{
				bool withCtrl = SelectionPartitionTransformer.WithCtrl;
				if (withCtrl && !_ctrlPressed)
				{
					Aligner.Prepair(Slide, Slide.Elements, TransformingElements);
				}
				else if (!withCtrl && _ctrlPressed)
				{
					Aligner.Prepair(Slide, Slide.Elements.Except(TransformingElements), TransformingElements);
				}
				_ctrlPressed = withCtrl;
				_altPressed = false;
				bool flag = Nearly.Equals(rect2, rect);
				AlignResult alignResult = (flag ? Aligner.Align(rect) : Aligner.Align(rect2, rect));
				desiredBounds.Offset(alignResult.Offset);
				rect.Offset(alignResult.Offset);
				rect2.Offset(alignResult.Offset);
				if (flag)
				{
					Aligner.Draw(rect);
				}
				else
				{
					Aligner.Draw(rect2, rect);
				}
			}
			DesiredBounds = desiredBounds;
		}

		protected override void OnCompleted()
		{
			Aligner.Clear();
		}
	}
}
