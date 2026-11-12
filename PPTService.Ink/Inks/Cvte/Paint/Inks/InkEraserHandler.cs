using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using Cvte.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Inks
{
	public class InkEraserHandler : EraserHandler
	{
		private readonly Slide _currentSlide;

		private IncrementalStrokeHitTester _hitTester;

		private StrokeCollection _strokes;

		public override string SlideId => _currentSlide?.Id;

		public List<Ink> DeleteInks => AllRemovedInks;

		private List<Ink> AllRemovedInks { get; set; } = new List<Ink>();

		private List<IErasableInk> Inks { get; set; }

		public InkEraserHandler(Slide currentSlide)
		{
			_currentSlide = currentSlide;
		}

		public override void OnErasingStarted()
		{
			Inks = (from IErasableInk temp in _currentSlide.Elements.OfType<Ink>()
				where temp.IsErasable
				select temp).ToList();
			foreach (IErasableInk ink in Inks)
			{
				ink.OnErasingStarted();
			}
			_strokes = new StrokeCollection();
			foreach (StrokeCollection item in Inks.Select((IErasableInk temp) => temp.TempInkContainer).Distinct())
			{
				_strokes.Add(item);
			}
		}

		public override IOperation OnErasingCompleted()
		{
			OnPreviewErasingCompleted();
			List<Element> list = new List<Element>();
			AllRemovedInks = AllRemovedInks.Distinct().ToList();
			foreach (IErasableInk item in ((IEnumerable<Ink>)AllRemovedInks).Select((Func<Ink, IErasableInk>)((Ink temp) => temp)))
			{
				list.AddRange(item.CreateNewInkOnErased());
			}
			_currentSlide.Add(list);
			foreach (Ink allRemovedInk in AllRemovedInks)
			{
				if (_currentSlide.Contains(allRemovedInk))
				{
					_currentSlide.Remove(allRemovedInk);
				}
			}
			if (CanActivateEventHelper)
			{
				EraseEventHelper.OnEraseComplete(this, new EraseCompleteEventArgs
				{
					CurrentEraser = this,
					DeleteInks = AllRemovedInks.Cast<Element>().ToList(),
					NewInks = list,
					CurrentSlideId = _currentSlide?.Id,
					AttachedSlide = _currentSlide
				});
			}
			_currentSlide.RaiseEvent(new InkErasingCompleteRoutedEventArgs(InkErasingEvents.InkErasingCompleteEvent, _currentSlide, this, AllRemovedInks.Cast<Element>().ToList(), list));
			foreach (IErasableInk ink in Inks)
			{
				ink.OnErasingCompleted();
			}
			Inks.Clear();
			if (AllRemovedInks.Any() || list.Any())
			{
				return new ElementCombinationOperation(_currentSlide, new AddElementsOperation(_currentSlide, list), new RemoveElementsOperation(_currentSlide, AllRemovedInks.ToArray()));
			}
			return null;
		}

		public override void OnErasingCanceled()
		{
			InkTracer.LogTracer.Debug(" Cancel erase.", "[InkEraserHandler]");
			foreach (IErasableInk ink in Inks)
			{
				ink.OnErasingCompleted();
			}
			Inks.Clear();
			AllRemovedInks.Clear();
			_currentSlide.RaiseEvent(new InkErasingCanceledRoutedEventArgs(InkErasingEvents.InkErasingCanceledEvent, _currentSlide, this));
		}

		public override void OnErasingSectionStarted(Point point)
		{
			_hitTester = _strokes.GetIncrementalStrokeHitTester(new RectangleStylusShape(EraserSize.Width, EraserSize.Height));
			_hitTester.StrokeHit += OnStrokeHit;
			if (CanActivateEventHelper)
			{
				EraseEventHelper.OnEraseStart(this, new EraseStartEventArgs
				{
					CurrentEraser = this,
					Width = (int)(EraserSize.Width * 1000.0),
					Height = (int)(EraserSize.Height * 1000.0),
					AttachedSlide = _currentSlide,
					X = (int)(point.X * 1000.0),
					Y = (int)(point.Y * 1000.0)
				});
			}
			_currentSlide.RaiseEvent(new InkErasingStartedRoutedEventArgs(InkErasingEvents.InkErasingStartedEvent, _currentSlide, this));
		}

		public override void OnErasingSectionCompleted()
		{
			_hitTester.StrokeHit -= OnStrokeHit;
			_hitTester.EndHitTesting();
		}

		public override void EraseCore(IList<Point> points)
		{
			_hitTester.AddPoints(points);
			if (CanActivateEventHelper && points.Any())
			{
				Point point = points.Last();
				EraseEventHelper.OnEraseRunning(this, new EraseRunningEventArgs
				{
					CurrentEraser = this,
					X = (int)(point.X * 1000.0),
					Y = (int)(point.Y * 1000.0),
					Width = (int)(EraserSize.Width * 1000.0),
					Height = (int)(EraserSize.Height * 1000.0),
					AttachedSlide = _currentSlide
				});
			}
		}

		private void OnStrokeHit(object sender, StrokeHitEventArgs e)
		{
			IErasableInk erasableInk = Inks.FirstOrDefault((IErasableInk temp) => temp.InnerStroke == e.HitStroke);
			if (erasableInk == null)
			{
				erasableInk = Inks.FirstOrDefault((IErasableInk temp) => temp.TempInkContainer.Any((Stroke t) => t == e.HitStroke));
				if (erasableInk == null)
				{
					throw new ArgumentException("擦除的笔迹没有在当前Slide");
				}
			}
			StrokeCollection pointEraseResults = e.GetPointEraseResults();
			_strokes.Add(pointEraseResults);
			_strokes.Remove(e.HitStroke);
			erasableInk.TempInkContainer.Add(pointEraseResults);
			erasableInk.TempInkContainer.Remove(e.HitStroke);
			((Ink)erasableInk).InvalidateVisual();
			AllRemovedInks.Add((Ink)erasableInk);
		}
	}
}
