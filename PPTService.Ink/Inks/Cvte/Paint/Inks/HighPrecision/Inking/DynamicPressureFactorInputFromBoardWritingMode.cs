using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class DynamicPressureFactorInputFromBoardWritingMode : HardPenInputFromBoardWritingMode
	{
		protected class DynamicPressureFactorDynamicRenderer : HardPenInputFromBoardDynamicRenderer
		{
			protected class DynamicPressureFactorInkStrokeTipBuilder : InkStrokeTipBuilder
			{
				private readonly DynamicPressureFactorInputFromBoardWritingMode _softInputFromBoardWritingMode;

				private StylusPointCollection _stylusPointCollection;

				private StylusPointQueue _stylusPointQueue;

				public override StylusPointCollection StrokeTipCollection
				{
					get
					{
						if (_stylusPointQueue == null || _stylusPointQueue.Count == 0)
						{
							return null;
						}
						if (_stylusPointCollection == null)
						{
							_stylusPointCollection = new StylusPointCollection(_stylusPointQueue[0].Description, 18);
						}
						StylusPointCollection stylusPointCollection = _stylusPointCollection;
						_stylusPointQueue.CopyToStylusPointCollection(stylusPointCollection);
						return stylusPointCollection;
					}
				}

				public DynamicPressureFactorInkStrokeTipBuilder(DynamicPressureFactorInputFromBoardWritingMode softInputFromBoardWritingMode)
				{
					_softInputFromBoardWritingMode = softInputFromBoardWritingMode;
				}

				public override StylusPointCollection AddPoint(StylusPoint stylusPoint)
				{
					stylusPoint.PressureFactor = _softInputFromBoardWritingMode.Converter.ConvertPressureFactor(stylusPoint.PressureFactor);
					if (_stylusPointQueue == null)
					{
						_stylusPointQueue = new StylusPointQueue(18);
						_stylusPointQueue.Enqueue(stylusPoint);
						return null;
					}
					StylusPointCollection stylusPointCollection = null;
					if (_stylusPointQueue.Count == 16)
					{
						stylusPointCollection = new StylusPointCollection(stylusPoint.Description);
						for (int i = 0; i < 8; i++)
						{
							stylusPointCollection.Add(_stylusPointQueue.Dequeue());
						}
						stylusPointCollection.Add(_stylusPointQueue[0]);
					}
					_stylusPointQueue.Enqueue(stylusPoint);
					return stylusPointCollection;
				}

				public override void Redraw()
				{
					if (_stylusPointQueue != null && _stylusPointQueue.Count != 0)
					{
						base.Redraw();
					}
				}
			}

			private readonly DynamicPressureFactorInputFromBoardWritingMode _softPenMode;

			public DynamicPressureFactorDynamicRenderer(InkCanvas inkCanvas, DynamicPressureFactorInputFromBoardWritingMode softPenMode)
				: base(inkCanvas)
			{
				_softPenMode = softPenMode;
			}

			public override InkStrokeTipBuilder GetInkStrokeTipBuilder()
			{
				return new DynamicPressureFactorInkStrokeTipBuilder(_softPenMode);
			}
		}

		public delegate float PressureFactorConverter(float originPressureFactor);

		private class DelegateDynamicPressureFactorConverter : IDynamicPressureFactorConverter
		{
			private PressureFactorConverter Converter { get; }

			public DelegateDynamicPressureFactorConverter(PressureFactorConverter converter)
			{
				Converter = converter;
			}

			public float ConvertPressureFactor(float originPressureFactor)
			{
				return Converter(originPressureFactor);
			}
		}

		private IDynamicPressureFactorConverter Converter { get; }

		protected DynamicPressureFactorInputFromBoardWritingMode()
		{
			IDynamicPressureFactorConverter dynamicPressureFactorConverter = this as IDynamicPressureFactorConverter;
			if (dynamicPressureFactorConverter != null)
			{
				Converter = dynamicPressureFactorConverter;
				return;
			}
			throw new InvalidOperationException("默认继承类需要继承 IDynamicPressureFactorConverter 接口才能使用此构造函数");
		}

		public DynamicPressureFactorInputFromBoardWritingMode(PressureFactorConverter converter)
		{
			Converter = new DelegateDynamicPressureFactorConverter(converter);
		}

		public DynamicPressureFactorInputFromBoardWritingMode(IDynamicPressureFactorConverter converter)
		{
			Converter = converter;
		}

		protected override HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new DynamicPressureFactorDynamicRenderer(inkCanvas, this);
		}

		protected override Element StrokeSynchronizerToElement(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			StylusPointCollection stylusPointCollection = synchronizer.StylusPointCollection;
			if (stylusPointCollection == null || !stylusPointCollection.Any())
			{
				return null;
			}
			DrawingAttributes drawingAttributes = synchronizer.DrawingAttributes.Clone();
			return new Ink(new CombinedStroke(stylusPointCollection, drawingAttributes));
		}
	}
}
