using System.IO;

namespace PPTService
{
	internal class ConfigController : IniSetting
	{
		private static readonly string IsComOkKey = "IsComOk";

		private static readonly string IsShowingKey = "IsShowing";

		private static readonly string IsNeedStartKey = "IsNeedStart";

		private static readonly string OfficeVersionKey = "OfficeVersion";

		private static readonly string IsSeewoDeviceKey = "IsSeewoDevice";

		private static readonly string IsPptShowTypeSpeakerKey = "IsPptShowTypeSpeaker";

		private static readonly string IsInitApplicationSuccessfulKey = "IsInitApplicationSuccessful";

		public bool IsComOk
		{
			get
			{
				return GetSettingValue(IsComOkKey, true);
			}
			set
			{
				SetSettingValue(IsComOkKey, value);
			}
		}

		public bool IsInitApplicationSuccessful
		{
			get
			{
				return GetSettingValue(IsInitApplicationSuccessfulKey, true);
			}
			set
			{
				SetSettingValue(IsInitApplicationSuccessfulKey, value);
			}
		}

		public bool IsNeedStart
		{
			get
			{
				return GetSettingValue(IsNeedStartKey, true);
			}
			set
			{
				SetSettingValue(IsNeedStartKey, value);
			}
		}

		public string OfficeVersion
		{
			get
			{
				return GetSettingValue(OfficeVersionKey, "");
			}
			set
			{
				SetSettingValue(OfficeVersionKey, value);
			}
		}

		public bool IsShowing
		{
			get
			{
				return GetSettingValue(IsShowingKey, false);
			}
			set
			{
				SetSettingValue(IsShowingKey, value);
			}
		}

		public string IsSeewoDevice
		{
			get
			{
				return GetSettingValue(IsSeewoDeviceKey, "");
			}
			set
			{
				SetSettingValue(IsSeewoDeviceKey, value);
			}
		}

		public bool IsPptShowTypeSpeaker
		{
			get
			{
				return GetSettingValue(IsPptShowTypeSpeakerKey, true);
			}
			set
			{
				SetSettingValue(IsPptShowTypeSpeakerKey, value);
			}
		}

		public ConfigController(string settingPath, string section)
			: base(settingPath, section)
		{
			if (!File.Exists(settingPath))
			{
				SetSettingValue(IsNeedStartKey, true, section);
			}
		}
	}
}
