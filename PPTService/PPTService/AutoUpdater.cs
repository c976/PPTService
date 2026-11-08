using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace PPTService
{
	internal static class AutoUpdater
	{
		private static int _checkTimes;

		private static int MaxCheckTimes = 2;

		private const string UpdaterName = "Updater.exe";

		public static void Check()
		{
			if (_checkTimes < MaxCheckTimes)
			{
				_checkTimes++;
				ThreadPool.QueueUserWorkItem((object o) =>
				{
					InnerCheck();
				});
			}
		}

		public static void Update()
		{
			InnerUpdate();
		}

		private static void InnerCheck()
		{
			Version newVersion = new Version();
			string text = TryToGetUpdater();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			Process process = new Process();
			process.StartInfo.Arguments = "-task check -type userstart -lang zh_chs";
			process.StartInfo.WorkingDirectory = Path.GetDirectoryName(text);
			process.StartInfo.FileName = text;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
			{
				Version result;
				if (Version.TryParse(e.Data, out result))
				{
					newVersion = result;
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();
			if (newVersion != new Version())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					UpdateWindow updateWindow = new UpdateWindow();
					updateWindow.NewVersion = newVersion.ToString();
					updateWindow.ShowDialog();
				});
			}
		}

		private static void InnerUpdate()
		{
			string text = TryToGetUpdater();
			Process process = new Process();
			process.StartInfo.Arguments = "-task update -type userstart -lang zh_chs";
			process.StartInfo.WorkingDirectory = Path.GetDirectoryName(text);
			process.StartInfo.FileName = text;
			process.Start();
		}

		private static string TryToGetUpdater()
		{
			string directoryName = Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ApplicationBase));
			if (!string.IsNullOrEmpty(directoryName))
			{
				string text = Path.Combine(directoryName, "Updater.exe");
				if (File.Exists(text))
				{
					return text;
				}
			}
			Log.WriteError("Updater missing.");
			return string.Empty;
		}
	}
}
