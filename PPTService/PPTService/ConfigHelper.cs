using System.IO;

namespace PPTService
{
	public class ConfigHelper
	{
		private static readonly string Section = "StartConfig";

		// 希沃设备检测已停用，下面两个结果常量随之注释保留备查。
		//private static readonly string CheckedTrue = "ChceckedTrue";
		//private static readonly string CheckedFalse = "ChceckedFalse";

		// 运行时状态已从 INI 格式的 app.config 迁移到 JSON（见 JsonSetting）。
		private static readonly string RuntimeConfigFileName = "RuntimeConfig.json";

		private static readonly ConfigController ConfigController = new ConfigController(Path.Combine(AppPath.RootPath(), RuntimeConfigFileName), Section);

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
			// 希沃设备检测已停用：IsSeewoDevice 全项目无读取点，IsNeedStart 又恒为 true，
			// 这个检测除了启动时枚举一次 HID、在 app.config 多写两行外没有任何作用。
			//CheckSeewoDevice();
		}

		// 原检测代码保留备查；若将来真要按设备决定是否启用，再取消上面的调用和此处注释。
		//private void CheckSeewoDevice()
		//{
		//	if (!(ConfigController.IsSeewoDevice != ""))
		//	{
		//		bool flag = McuService.FindMcu();
		//		ConfigController.IsNeedStart = true;
		//		ConfigController.IsSeewoDevice = (flag ? CheckedTrue : CheckedFalse);
		//	}
		//}

		public bool IsNeedStart()
		{
			// 模拟放映模式下不依赖辅助设备检测结果，始终允许启动界面。
			return DebugMode.IsEnabled || ConfigController.IsNeedStart;
		}
	}
}
