using System.IO;

namespace PPTService
{
	public class ConfigHelper
	{
		private static readonly string Section = "StartConfig";

		private static readonly string CheckedTrue = "ChceckedTrue";

		private static readonly string CheckedFalse = "ChceckedFalse";

		private static readonly string AppConfigFileName = "app.config";

		private static readonly ConfigController ConfigController = new ConfigController(Path.Combine(AppPath.RootPath(), AppConfigFileName), Section);

		public static ConfigHelper Instance { get; set; }

		public bool IsInitApplicationSuccessful
		{
			get
			{
				return ConfigController.IsInitApplicationSuccessful;
			}
			set
			{
				ConfigController.IsInitApplicationSuccessful = value;
			}
		}

		public bool IsComOk
		{
			get
			{
				return ConfigController.IsComOk;
			}
			set
			{
				ConfigController.IsComOk = value;
			}
		}

		public string OfficeVersion
		{
			get
			{
				return ConfigController.OfficeVersion;
			}
			set
			{
				ConfigController.OfficeVersion = value;
			}
		}

		public bool IsShowing
		{
			get
			{
				return ConfigController.IsShowing;
			}
			set
			{
				ConfigController.IsShowing = value;
			}
		}

		public bool IsPptShowTypeSpeaker
		{
			get
			{
				return ConfigController.IsPptShowTypeSpeaker;
			}
			set
			{
				ConfigController.IsPptShowTypeSpeaker = value;
			}
		}

		public ConfigHelper()
		{
			CheckSeewoDevice();
		}

		private void CheckSeewoDevice()
		{
			if (!(ConfigController.IsSeewoDevice != ""))
			{
				bool flag = McuService.FindMcu();
				ConfigController.IsNeedStart = true;
				ConfigController.IsSeewoDevice = (flag ? CheckedTrue : CheckedFalse);
			}
		}

		public bool IsNeedStart()
		{
			// 模拟放映模式下不依赖辅助设备检测结果，始终允许启动界面。
			return DebugMode.IsEnabled || ConfigController.IsNeedStart;
		}
	}
}
