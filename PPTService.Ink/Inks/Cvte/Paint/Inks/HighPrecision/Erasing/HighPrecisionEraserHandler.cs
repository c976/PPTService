using System;
using System.Collections.Generic;
using System.Windows.Ink;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public abstract class HighPrecisionEraserHandler : IHighPrecisionEraserHandler
	{
		public int Id { get; }

		public Slide CurrentSlide { get; }

		public EraserSettings EraserSettings { get; }

		public Board Board { get; }

		public ErasingSynchronizer ErasingSynchronizer { get; }

		protected abstract IErasingTester ErasingTester { get; }

		protected EraserTouchEventArgs? CurrentEraserTouch { get; private set; }

		public HighPrecisionEraserHandler(int id, Slide currentSlide, EraserSettings eraserSettings, Board board, ErasingSynchronizer erasingSynchronizer)
		{
			Id = id;
			CurrentSlide = currentSlide;
			EraserSettings = eraserSettings;
			Board = board;
			ErasingSynchronizer = erasingSynchronizer;
		}

		public void StartEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CurrentEraserTouch = eraserTouchEventArgs;
			OnStartEraser(eraserTouchEventArgs);
			ContinueEraser(eraserTouchEventArgs);
		}

		protected virtual void OnStartEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
		}

		public void ContinueEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			if (!CurrentEraserTouch.HasValue)
			{
				CurrentEraserTouch = eraserTouchEventArgs;
				return;
			}
			EraserTouchEventArgs value = CurrentEraserTouch.Value;
			if (Math.Abs(value.X - eraserTouchEventArgs.X) > 1.0 || Math.Abs(value.Y - eraserTouchEventArgs.Y) > 1.0 || Math.Abs(value.Rotation - eraserTouchEventArgs.Rotation) > 1.0)
			{
				CurrentEraserTouch = eraserTouchEventArgs;
				OnContinueEraser(eraserTouchEventArgs);
			}
		}

		protected virtual void OnContinueEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			ErasingTester.Move(ref eraserTouchEventArgs);
			List<Stroke> list = new List<Stroke>();
			foreach (Ink erasableInk2 in ErasingSynchronizer.GetErasableInks())
			{
				IErasableInk erasableInk = erasableInk2;
				list.Clear();
				ErasingSynchronizer.StartEraseInk(erasableInk2);
				for (int i = 0; i < erasableInk.TempInkContainer.Count; i++)
				{
					Stroke stroke = erasableInk.TempInkContainer[i];
					ValueTuple<bool, IEnumerable<Stroke>> valueTuple = ErasingTester.EraseTest(stroke);
					bool item = valueTuple.Item1;
					IEnumerable<Stroke> item2 = valueTuple.Item2;
					if (item)
					{
						erasableInk.TempInkContainer.RemoveAt(i);
						i--;
						list.AddRange(item2);
						erasableInk2.InvalidateVisual();
					}
				}
				foreach (Stroke item7 in list)
				{
					erasableInk.TempInkContainer.Add(item7);
				}
			}
			foreach (KeyValuePair<Element, ValueTuple<Geometry, Transform>> erasableElement in ErasingSynchronizer.GetErasableElements())
			{
				Element key;
				ValueTuple<Geometry, Transform> value;
				erasableElement.Deconstruct(out key, out value);
				Element element = key;
				ValueTuple<Geometry, Transform> valueTuple2 = value;
				Geometry item3 = valueTuple2.Item1;
				Transform item4 = valueTuple2.Item2;
				ValueTuple<bool, Geometry> valueTuple3 = ErasingTester.EraseTest(item3, item4);
				bool item5 = valueTuple3.Item1;
				Geometry item6 = valueTuple3.Item2;
				if (item5)
				{
					ErasingSynchronizer.StartEraseElement(element);
					if (!element.IsErased)
					{
						element.IsErased = true;
					}
					if (element.HasMask)
					{
						element.SetMaskClip(item6, false);
					}
					element.SetErasedClip(item6, false);
				}
			}
		}

		public void FinishEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CurrentEraserTouch = eraserTouchEventArgs;
			OnFinishEraser(eraserTouchEventArgs);
		}

		protected virtual void OnFinishEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
		}

		public void CompleteEraser()
		{
		}
	}
}
