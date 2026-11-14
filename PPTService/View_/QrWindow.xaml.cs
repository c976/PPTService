using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace PPTService
{
	public partial class QrWindow : Window, IComponentConnector
	{
		public QrWindow(string qrFilePath)
		{
			InitializeComponent();
			Loaded += QrWindow_Loaded;
			Unloaded += QrWindow_Unloaded;
			Top = ViewHelper.MainWindow.Top;
			Left = ViewHelper.MainWindow.Left;
			Width = ViewHelper.MainWindow.Width;
			Height = ViewHelper.MainWindow.Height;
			CloseButton.Click += (object s, RoutedEventArgs e) =>
			{
				Close();
			};
			QrImage.Source = new BitmapImage(new Uri(qrFilePath));
		}

		private void QrWindow_Loaded(object sender, RoutedEventArgs e)
		{
			AdjustImageSize();
			PptOperator.Instance.ShowEnd += PPT_ShowEnd;
			(Resources["StoryBoard.ShowQrImage"] as Storyboard)?.Begin();
		}

		private void QrWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			});
			PptOperator.Instance.ShowEnd -= PPT_ShowEnd;
		}

		private void PPT_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(Close);
		}

		private void QrBorder_OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
		{
			e.Mode = ManipulationModes.Scale;
		}

		private void QrBorder_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			Enlarge(e.DeltaManipulation.Scale.X);
		}

		private void QrBorder_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scaleX = 1.05;
			if (e.Delta < 0)
			{
				scaleX = 0.95;
			}
			Enlarge(scaleX);
		}

		private void Enlarge(double scaleX)
		{
			double width = QrBorder.Width * scaleX;
			double num = QrBorder.Height * scaleX;
			if (!(num <= 300.0) && !(num > ViewHelper.MainWindow.Height))
			{
				QrBorder.Width = width;
				QrBorder.Height = num;
			}
		}

		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			BitmapSource bitmapSource = QrImage.Source as BitmapSource;
			if (bitmapSource == null)
			{
				new MessageWindow("当前二维码无法保存").ShowDialog();
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "PNG 图片 (*.png)|*.png",
				FileName = "二维码.png",
				DefaultExt = "png",
				OverwritePrompt = true
			};
			if (saveFileDialog.ShowDialog() == true)
			{
				try
				{
					bitmapSource.SaveToFile(saveFileDialog.FileName, overwrite: true);
					new MessageWindow("二维码已保存到\n" + saveFileDialog.FileName).ShowDialog();
				}
				catch (Exception ex)
				{
					new MessageWindow("二维码保存失败：" + ex.Message).ShowDialog();
				}
			}
		}

		private void AdjustImageSize()
		{
			if (Width > 3800.0)
			{
				QrBorder.Width = 1200.0;
				QrBorder.Height = 1200.0;
			}
			if (Width > 1440.0 && Width <= 3800.0)
			{
				QrBorder.Width = 600.0;
				QrBorder.Height = 600.0;
			}
			if (Width > 0.0 && Width <= 1400.0)
			{
				QrBorder.Width = 400.0;
				QrBorder.Height = 400.0;
			}
		}
	}
}
