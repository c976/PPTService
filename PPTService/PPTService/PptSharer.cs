using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using PPTService.Core.Windows;
using PPTService.Collection;

namespace PPTService
{
	public static class PptSharer
	{
		public static bool IsCancelShare;

		private static bool _isAddExportTime;

		public static CancellationTokenSource CancellationTokenSource;

		private static readonly Dictionary<string, string> PptMd5AndQrFilePathDictionary = new Dictionary<string, string>();

		// 二维码上传实现位于可选插件 PPTService.NetShared.dll。这里只用反射调用，
		// 主程序对它没有编译期引用；DLL 不存在时仅二维码功能不可用。
		private const string QrServiceTypeName = "PPTService.NetShared.QrShareService, PPTService.NetShared";

		private static bool TryPublishQr(IList<string> imageFiles, string title, double scale,
			CancellationToken cancellationToken, Action<double> progressChanged, out string url)
		{
			url = string.Empty;
			try
			{
				Type serviceType = Type.GetType(QrServiceTypeName, false);
				if (serviceType == null)
				{
					return false;
				}
				MethodInfo publish = serviceType.GetMethod("Publish", BindingFlags.Public | BindingFlags.Static);
				if (publish == null)
				{
					return false;
				}
				object[] args = { imageFiles, title, scale, cancellationToken, progressChanged, url };
				bool result = (bool)publish.Invoke(null, args);
				url = args[5] as string ?? string.Empty;
				return result;
			}
			catch (Exception)
			{
				// 插件版本不匹配 / 加载失败都退化为「二维码不可用」，绝不影响放映。
				return false;
			}
		}

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
				string title = Path.GetFileName(pptFilePath);
				double scale = PptAnalyzer.ThumbnailRate;
				CancellationToken token = CancellationTokenSource.Token;
				Action<double> progressChanged = (double progress) =>
				{
					LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
					{
						loadingView.Progress = Convert.ToInt32(_isAddExportTime ? (30.0 + progress * 70.0) : (progress * 100.0));
					});
				};
				if (!TryPublishQr(imageFiles, title, scale, token, progressChanged, out url))
				{
					string message = Type.GetType(QrServiceTypeName, false) == null
						? "二维码组件缺失，无法创建二维码"
						: "网络开小差，无法创建二维码";
					AfterShareFailedDo(message);
					return;
				}
				AfterShareSuccessfullyDo(pptFilePath, url);
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

		private static void AfterShareFailedDo(string message = "网络开小差，无法创建二维码")
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
					MessageWindow messageWindow = new MessageWindow(message);
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
