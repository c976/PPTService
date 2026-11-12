using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.HighPrecision.Elements;
using Cvte.Paint.Inks.PointToGeometry;
using Cvte.Paint.Inks.Visual_;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class HighPrecisionInputFromBoardWritingMode : DynamicPressureFactorInputFromBoardWritingMode
	{
		private class HighPrecisionDynamicRenderer : DynamicPressureFactorDynamicRenderer
		{
			private class HighPrecisionInkStrokeTipBuilder : DynamicPressureFactorInkStrokeTipBuilder
			{
				public InkStrokeRender InkStrokeRender { get; }

				private HighPrecisionStrokeTipVisual StrokeTipVisual { get; set; }

				public HighPrecisionInkStrokeTipBuilder(DynamicPressureFactorInputFromBoardWritingMode softInputFromBoardWritingMode, InkStrokeRender inkStrokeRender)
					: base(softInputFromBoardWritingMode)
				{
					InkStrokeRender = inkStrokeRender;
				}

				public override DrawingVisual CreateStroke()
				{
					StrokeTipVisual = new HighPrecisionStrokeTipVisual(Brush, DrawingAttributes, InkStrokeRender);
					return StrokeTipVisual;
				}

				public override void Redraw()
				{
					StrokeTipVisual.Redraw(StrokeTipCollection);
				}
			}

			public class HighPrecisionStrokeTipVisual : DrawingVisual
			{
				private readonly SolidColorBrush _brush;

				private readonly DrawingAttributes _drawingAttributes;

				private readonly InkStrokeRender _inkStrokeRender;

				public HighPrecisionStrokeTipVisual(SolidColorBrush brush, DrawingAttributes drawingAttributes, InkStrokeRender inkStrokeRender)
				{
					_brush = brush;
					_drawingAttributes = drawingAttributes;
					_inkStrokeRender = inkStrokeRender;
				}

				public void Redraw(StylusPointCollection stylusPoints)
				{
					using (DrawingContext drawingContext = RenderOpen())
					{
						_inkStrokeRender.RenderInk(drawingContext, stylusPoints, _drawingAttributes, _brush);
					}
				}
			}

			private readonly HighPrecisionInputFromBoardWritingMode _softPenMode;

			private InkStrokeRender _inkStrokeRender;

			private InkStrokeRender InkStrokeRender
			{
				get
				{
					if (_inkStrokeRender == null)
					{
						InkStrokeRenderSettings inkStrokeRenderSettings = Settings?.InkStrokeRenderSettings;
						if (inkStrokeRenderSettings == null)
						{
							_inkStrokeRender = HighPrecisionInkStrokeRender.Instance;
						}
						else
						{
							_inkStrokeRender = new InkStrokeRender(inkStrokeRenderSettings);
						}
					}
					return _inkStrokeRender;
				}
			}

			private WritingInkRecorder WritingInkRecorder => _softPenMode.WritingInkRecorder;

			private bool NeedCollectStrokeDetails => _softPenMode.NeedCollectStrokeDetails;

			private HighPrecisionWritingModeBoardSettings Settings => _softPenMode.HighPrecisionWritingModeBoardSettings;

			public HighPrecisionDynamicRenderer(InkCanvas inkCanvas, HighPrecisionInputFromBoardWritingMode softPenMode)
				: base(inkCanvas, softPenMode)
			{
				_softPenMode = softPenMode;
			}

			protected override void OnInit()
			{
				Render = new HighPrecisionInkRender(InkStrokeRender);
			}

			public override InkStrokeTipBuilder GetInkStrokeTipBuilder()
			{
				return new HighPrecisionInkStrokeTipBuilder(_softPenMode, InkStrokeRender);
			}

			protected override void OnDown(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				CollectStrokePointDetails(stylusPointCollectionInfo);
				base.OnDown(stylusPointCollectionInfo);
			}

			protected override void OnMove(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				CollectStrokePointDetails(stylusPointCollectionInfo);
				base.OnMove(stylusPointCollectionInfo);
			}

			protected override void OnUp(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				CollectStrokePointDetails(stylusPointCollectionInfo);
				base.OnUp(stylusPointCollectionInfo);
			}

			private void CollectStrokePointDetails(StylusPointCollectionInfo stylusPointCollectionInfo)
			{
				if (NeedCollectStrokeDetails)
				{
					WritingInkRecorder.CollectStrokePointDetails(stylusPointCollectionInfo);
				}
			}
		}

		private class HighPrecisionInkRender : DynamicRenderer.InkRenderer
		{
			private InkStrokeRender InkStrokeRender { get; }

			public HighPrecisionInkRender(InkStrokeRender inkStrokeRender)
			{
				InkStrokeRender = inkStrokeRender;
			}

			protected override Cvte.Paint.Inks.Visual_.StrokeVisual CreateStrokeVisual(DynamicRenderer.StrokeInfo strokeInfo, StylusPointCollection stylusPointCollection)
			{
				return new HighPrecisionStrokeVisual(strokeInfo.FillSolidColorBrush, strokeInfo.DrawingAttributes, stylusPointCollection, InkStrokeRender);
			}
		}

		private class HighPrecisionStrokeVisual : Cvte.Paint.Inks.Visual_.StrokeVisual
		{
			private readonly DrawingAttributes _drawingAttributes;

			private readonly Brush _brush;

			private InkStrokeRender InkStrokeRender { get; }

			public HighPrecisionStrokeVisual(SolidColorBrush brush, DrawingAttributes drawingAttributes, StylusPointCollection stylusPointCollection, InkStrokeRender inkStrokeRender)
				: base(brush, drawingAttributes, stylusPointCollection)
			{
				InkStrokeRender = inkStrokeRender;
				ShouldUseCustomRedraw = true;
				_drawingAttributes = drawingAttributes;
				_brush = brush;
			}

			protected override void CustomRedraw()
			{
				using (DrawingContext drawingContext = RenderOpen())
				{
					InkStrokeRender.RenderInk(drawingContext, Stroke.StylusPoints, _drawingAttributes, _brush);
				}
			}
		}

		private bool NeedCollectStrokeDetails { get; set; }

		private WritingInkRecorder WritingInkRecorder { get; } = new WritingInkRecorder();

		public HardPenInputFromBoardDynamicRenderer DynamicRenderer { get; private set; }

		private HighPrecisionWritingModeBoardSettings HighPrecisionWritingModeBoardSettings { get; set; }

		public HighPrecisionInputFromBoardWritingMode(PressureFactorConverter converter)
			: base(converter)
		{
		}

		protected HighPrecisionInputFromBoardWritingMode()
		{
		}

		protected override void OnSwitchOn()
		{
			Board board = GetBoard();
			HighPrecisionWritingModeBoardSettings = board.GetSettings<HighPrecisionWritingModeBoardSettings>();
			NeedCollectStrokeDetails = HighPrecisionWritingModeBoardSettings.NeedCollectStrokeDetails;
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
		}

		protected override Element StrokeSynchronizerToElement(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			StylusPointCollection stylusPointCollection = synchronizer.StylusPointCollection;
			if (stylusPointCollection == null || !stylusPointCollection.Any())
			{
				return null;
			}
			DrawingAttributes drawingAttributes = synchronizer.DrawingAttributes.Clone();
			CombinedStroke combinedStroke = new CombinedStroke(stylusPointCollection, drawingAttributes);
			if (NeedCollectStrokeDetails)
			{
				combinedStroke.StrokePointDetailsList = WritingInkRecorder.PopStrokePointDetails(synchronizer.StylusDeviceId);
			}
			return new HighPrecisionInkElement(combinedStroke, HighPrecisionWritingModeBoardSettings.InkStrokeRenderSettings)
			{
				IsDebugMode = false
			};
		}

		protected override HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return DynamicRenderer = new HighPrecisionDynamicRenderer(inkCanvas, this);
		}
	}
}
