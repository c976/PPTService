using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Cvte.WebService.Lychee;
using Cvte.Windows;
using PPTService.Collection;

namespace PPTService
{
	public static class PptSharer
	{
		public static bool IsCancelShare;

		private static bool _isAddExportTime;

		public static CancellationTokenSource CancellationTokenSource;

		private static readonly Dictionary<string, string> PptMd5AndQrFilePathDictionary = new Dictionary<string, string>();

		public static void Share(string pptFilePath)
		{
			IsCancelShare = false;
			_isAddExportTime = false;
			SyntacticSugar.TryCatchAction(() =>
			{
				ViewHelper.StopMainWindowUpdateTimerAndMouseGesture();
				if (RegexHelper.IsLocalPath(pptFilePath) && File.Exists(pptFilePath))
				{
					string md5 = Md5SecurityUtility.GetShaFromFile(pptFilePath);
					if (PptMd5AndQrFilePathDictionary.ContainsKey(md5))
					{
						string value = PptMd5AndQrFilePathDictionary.FirstOrDefault((KeyValuePair<string, string> d) => d.Key == md5).Value;
						if (File.Exists(value))
						{
							FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "ShowExistentQrImage");
							QrWindow qrWindow = new QrWindow(value);
							qrWindow.Owner = ViewHelper.MainWindow;
							qrWindow.ShowDialog();
							return;
						}
					}
				}
				if (!NetworkHelper.IsNetworkConnect)
				{
					FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "NetwrokIsNotConnect");
					MessageWindow messageWindow = new MessageWindow("无网络连接，无法创建二维码");
					messageWindow.Owner = ViewHelper.MainWindow;
					messageWindow.ShowDialog();
				}
				else
				{
					LycheeShare(pptFilePath);
				}
			}, ViewHelper.BeginMainWindowUpdateTimerAndMouseGesture);
		}

		private static void LycheeShare(string pptFilePath)
		{
			LoadingScreen<LoadingView>.Current.Show(new Rect
			{
				X = ViewHelper.MainWindow.Left,
				Y = ViewHelper.MainWindow.Top,
				Width = ViewHelper.MainWindow.Width,
				Height = ViewHelper.MainWindow.Height
			});
			LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
			{
				loadingView.Start();
				loadingView.Tip = "创建二维码中...";
				loadingView.CancelAction = Cancel;
			});
			List<string> imageFiles;
			if (!PptAnalyzer.FindAlreadyAnalyzedFiles(pptFilePath, out imageFiles))
			{
				PptAnalyzer.PptExportAllImages(pptFilePath, (double progress) =>
				{
					LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
					{
						_isAddExportTime = true;
						loadingView.Progress = Convert.ToInt32(progress * 30.0);
					});
				}, out imageFiles);
			}
			if (!imageFiles.Any())
			{
				LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
				{
					loadingView.Continue();
					LoadingScreen<LoadingView>.Current.Hide();
				});
				if (IsCancelShare)
				{
					WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
					return;
				}
				FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "NoPicture");
				MessageWindow messageWindow = new MessageWindow("图片解析出错，无法创建二维码");
				messageWindow.Owner = ViewHelper.MainWindow;
				messageWindow.ShowDialog();
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				return;
			}
			CancellationTokenSource = new CancellationTokenSource();
			try
			{
				string url;
				if (new LycheeService("http://lizhi.seewo.com", "pptservice", "0f6bb4dc70e901e21eba5dfdec6c68f6").Publish(imageFiles, new PublicInfo
				{
					Author = "",
					Title = Path.GetFileName(pptFilePath),
					Scale = PptAnalyzer.ThumbnailRate
				}, CancellationTokenSource.Token, (double progress) =>
				{
					LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
					{
						loadingView.Progress = Convert.ToInt32(_isAddExportTime ? (30.0 + progress * 70.0) : (progress * 100.0));
					});
				}, out url))
				{
					AfterShareSuccessfullyDo(pptFilePath, url);
					return;
				}
				LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
				{
					loadingView.Cancel();
				});
				AfterShareFailedDo();
			}
			catch (Exception)
			{
				AfterShareFailedDo();
			}
		}

		private static void AfterShareSuccessfullyDo(string pptFilePath, string url)
		{
			string qrFilePath = Path.Combine(AppPath.TemPath(), Guid.NewGuid().ToString() + ".png");
			if (!string.IsNullOrEmpty(url))
			{
				new WebClient().DownloadFile(url, qrFilePath);
			}
			LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
			{
				loadingView.PushFrame();
				LoadingScreen<LoadingView>.Current.Hide();
				SyntacticSugar.CurrentDispatcherInvoke(() =>
				{
					if (!string.IsNullOrEmpty(qrFilePath) && File.Exists(qrFilePath))
					{
						FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "Success");
						SyntacticSugar.TryCatchAction(() =>
						{
							ViewHelper.StopMainWindowUpdateTimerAndMouseGesture();
							QrWindow qrWindow = new QrWindow(qrFilePath)
							{
								Owner = ViewHelper.MainWindow
							};
							if (RegexHelper.IsLocalPath(pptFilePath) && File.Exists(pptFilePath))
							{
								string shaFromFile = Md5SecurityUtility.GetShaFromFile(pptFilePath);
								PptMd5AndQrFilePathDictionary.Add(shaFromFile, qrFilePath);
							}
							qrWindow.ShowDialog();
						}, ViewHelper.BeginMainWindowUpdateTimerAndMouseGesture);
					}
				});
			});
		}

		private static void AfterShareFailedDo()
		{
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			if (IsCancelShare)
			{
				return;
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionShare, "Failed");
			LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
			{
				loadingView.Continue();
				LoadingScreen<LoadingView>.Current.Hide();
				SyntacticSugar.CurrentDispatcherInvoke(() =>
				{
					MessageWindow messageWindow = new MessageWindow("网络开小差，无法创建二维码");
					messageWindow.Owner = ViewHelper.MainWindow;
					messageWindow.ShowDialog();
				});
			});
		}

		private static void Cancel()
		{
			IsCancelShare = true;
			PptAnalyzer.IsBreakExport = true;
			CancellationTokenSource?.Cancel();
		}
	}
}
