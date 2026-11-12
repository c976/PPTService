using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.Highlighter
{
	public class HighlighterWritingMode : HardPenInputFromBoardWritingMode
	{
		private class HighlighterDynamicRenderer : HardPenInputFromBoardDynamicRenderer
		{
			private HighlighterWritingMode HighlighterWritingMode { get; }

			private HighlighterWritingModeSettings HighlighterWritingModeSettings => HighlighterWritingMode.HighlighterWritingModeSettings;

			private ContainerVisual MouseContainerVisual { get; set; }

			private List<ContainerVisual> TouchContainerVisualCacheList { get; } = new List<ContainerVisual>();

			public HighlighterDynamicRenderer(InkCanvas inkCanvas, HighlighterWritingMode highlighterWritingMode)
				: base(inkCanvas)
			{
				HighlighterWritingMode = highlighterWritingMode;
			}

			public override void UpdateInkBoardSettings(InkBoardSettings inkBoardSettings)
			{
				base.UpdateInkBoardSettings(inkBoardSettings);
				double opacity = HighlighterWritingModeSettings.HighlighterOpacity;
				if (HighlighterWritingMode == null)
				{
					return;
				}
				ContainerVisual mouseContainerVisual = MouseContainerVisual;
				if (mouseContainerVisual != null)
				{
					mouseContainerVisual.Dispatcher.InvokeAsync(() => mouseContainerVisual.Opacity = opacity);
				}
				ContainerVisual[] touchContainerVisualCacheList = TouchContainerVisualCacheList.ToArray();
				if (!touchContainerVisualCacheList.Any())
				{
					return;
				}
				touchContainerVisualCacheList[0].Dispatcher.InvokeAsync(() =>
				{
					ContainerVisual[] array = touchContainerVisualCacheList;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Opacity = opacity;
					}
				});
			}

			protected override DrawingAttributes UpdateDrawingAttributes(InkBoardSettings inkBoardSettings)
			{
				if (HighlighterWritingMode == null)
				{
					return base.UpdateDrawingAttributes(inkBoardSettings);
				}
				return new DrawingAttributes
				{
					FitToCurve = false,
					IgnorePressure = false,
					StylusTip = StylusTip.Ellipse,
					Color = inkBoardSettings.InkColor,
					Width = inkBoardSettings.InkThickness,
					Height = inkBoardSettings.InkThickness * HighlighterWritingModeSettings.WidthHeightRatio
				};
			}

			protected override ContainerVisual GetContainerVisual(int stylusDeviceId)
			{
				if (stylusDeviceId == 0)
				{
					if (MouseContainerVisual == null)
					{
						ContainerVisual containerVisual = new ContainerVisual
						{
							Opacity = HighlighterWritingModeSettings.HighlighterOpacity
						};
						InkCanvas.InkFromMouseContainerVisual.Children.Add(containerVisual);
						MouseContainerVisual = containerVisual;
					}
					return MouseContainerVisual;
				}
				ContainerVisual containerVisual2 = null;
				InkCanvas.InkFromTouchContainerVisual.Dispatcher.Invoke(() =>
				{
					containerVisual2 = GetTouchContainerVisual();
				});
				TouchContainerVisualCacheList.Add(containerVisual2);
				return containerVisual2;
			}

			public override void Stop()
			{
				if (MouseContainerVisual != null)
				{
					InkCanvas.RunInMouseDispatcher((ContainerVisual visual) =>
					{
						ContainerVisual mouseContainerVisual = MouseContainerVisual;
						MouseContainerVisual = null;
						mouseContainerVisual.Children.Clear();
						visual.Children.Clear();
					});
				}
				List<ContainerVisual> touchContainerVisualCacheList = TouchContainerVisualCacheList.ToList();
				TouchContainerVisualCacheList.Clear();
				if (!touchContainerVisualCacheList.Any())
				{
					return;
				}
				InkCanvas.RunInTouchDispatcher((ContainerVisual visual) =>
				{
					foreach (ContainerVisual item in touchContainerVisualCacheList)
					{
						item.Children.Clear();
					}
					visual.Children.Clear();
				});
			}

			private ContainerVisual GetTouchContainerVisual()
			{
				ContainerVisual containerVisual = TouchContainerVisualCacheList.FirstOrDefault((ContainerVisual temp) => temp.Children.Count == 0);
				if (containerVisual == null)
				{
					containerVisual = new ContainerVisual
					{
						Opacity = HighlighterWritingModeSettings.HighlighterOpacity
					};
					InkCanvas.InkFromTouchContainerVisual.Children.Add(containerVisual);
				}
				return containerVisual;
			}
		}

		private HighlighterWritingModeSettings HighlighterWritingModeSettings { get; set; }

		protected override void OnSwitchOn()
		{
			HighlighterWritingModeSettings = CurrentSlide.GetSettings<HighlighterWritingModeSettings>();
			HighlighterWritingModeSettings.PropertyChanged += HighlighterWritingModeSettings_PropertyChanged;
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			HighlighterWritingModeSettings.PropertyChanged -= HighlighterWritingModeSettings_PropertyChanged;
			base.OnSwitchOff();
		}

		private void HighlighterWritingModeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			InkCanvas.DynamicRenderer.UpdateInkBoardSettings(InkBoardSettings);
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
			combinedStroke.GetGeometry(combinedStroke.DrawingAttributes);
			HighlighterInk highlighterInk = HighlighterInk.CreateHighlighterInkFromSlideStroke(combinedStroke);
			highlighterInk.HighlighterOpacity = HighlighterWritingModeSettings.HighlighterOpacity;
			return highlighterInk;
		}

		protected override void EndDry(InkSynchronizer synchronizer)
		{
			CurrentSlide.Dispatcher.InvokeAsync(() =>
			{
				base.EndDry(synchronizer);
			}, DispatcherPriority.Normal);
		}

		protected override HardPenInputFromBoardDynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new HighlighterDynamicRenderer(inkCanvas, this);
		}
	}
}
