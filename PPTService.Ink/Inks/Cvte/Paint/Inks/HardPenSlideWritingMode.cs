using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class HardPenSlideWritingMode : WritingPenMode, IPenMode, IDisposable
	{
		private int _dryingCount;

		protected InkCanvas InkCanvas { get; set; }

		private InkBoardSettings InkBoardSettings { get; set; }

		protected override void OnSwitchOn()
		{
			Slide currentSlide = CurrentSlide;
			InkBoardSettings = currentSlide.GetSettings<InkBoardSettings>();
			if (InkCanvas == null)
			{
				InkCanvas = new InkCanvas(false);
				InkCanvas.DynamicRenderer = GetDynamicRenderer(InkCanvas);
				currentSlide.AddUtilities(InkCanvas);
				InkCanvas.Init();
				InkCanvas.StrokesCollected += InkCanvas_StrokesCollected;
			}
			else if (!currentSlide.ContainsUtilities(InkCanvas))
			{
				currentSlide.AddUtilities(InkCanvas);
				InkCanvas.Start();
			}
			InkBoardSettings.PropertyChanged += InkBoardSettings_OnPropertyChanged;
			UpdateInkBoardSettings();
			base.OnSwitchOn();
		}

		protected override void OnSwitchOff()
		{
			Slide currentSlide = CurrentSlide;
			InkCanvas.Stop();
			currentSlide.RemoveUtilities(InkCanvas);
			InkBoardSettings.PropertyChanged -= InkBoardSettings_OnPropertyChanged;
			base.OnSwitchOff();
		}

		protected virtual DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new DynamicRenderer(inkCanvas);
		}

		private void InkBoardSettings_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateInkBoardSettings();
		}

		protected virtual async void InkCanvas_StrokesCollected(object sender, InkSynchronizer e)
		{
			foreach (InkSynchronizer.StrokeSynchronizer item in e.BeginDry())
			{
				AsInk(item);
			}
			_dryingCount++;
			await Task.Delay(15);
			_dryingCount--;
			if (_dryingCount == 0)
			{
				CurrentSlide.Dispatcher.InvokeAsync(() =>
				{
					e.EndDry();
				}, DispatcherPriority.Background);
			}
		}

		protected virtual void AsInk(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			StylusPointCollection stylusPointCollection = synchronizer.StylusPointCollection;
			if (stylusPointCollection != null && stylusPointCollection.Any())
			{
				Slide currentSlide = CurrentSlide;
				CombinedStroke combinedStroke = new CombinedStroke(stylusPointCollection, synchronizer.DrawingAttributes);
				combinedStroke.GetGeometry(synchronizer.DrawingAttributes);
				Ink element = new Ink(combinedStroke);
				currentSlide.Add(element);
			}
		}

		protected void UpdateInkBoardSettings()
		{
			InkCanvas.DynamicRenderer.UpdateInkBoardSettings(InkBoardSettings);
		}

		public void Dispose()
		{
			InkCanvas?.Dispose();
		}
	}
}
