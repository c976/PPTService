using System.Runtime.CompilerServices;
using Cvte.WebService.Utils;

namespace Cvte.WebService.Internal
{
	internal class AccountSettings : IniSetting
	{
		public string this[string string_2]
		{
			get
			{
				return method_2(string_2, string.Empty);
			}
			set
			{
				method_5(string_2, value);
			}
		}

		public AccountSettings(string string_2, string string_3)
			: base(string_2, string_3)
		{
		}

		[SpecialName]
		public string method_6()
		{
			return this["LastUserResourceId"];
		}

		[SpecialName]
		public void method_7(string string_2)
		{
			this["LastUserResourceId"] = string_2;
		}

		[SpecialName]
		public string method_8()
		{
			return this["LastUserId"];
		}

		[SpecialName]
		public void method_9(string string_2)
		{
			this["LastUserId"] = string_2;
		}

		[SpecialName]
		public string method_10()
		{
			return this["LastUserName"];
		}

		[SpecialName]
		public void method_11(string string_2)
		{
			this["LastUserName"] = string_2;
		}

		[SpecialName]
		public string method_12()
		{
			return this["LastToken"];
		}

		[SpecialName]
		public void method_13(string string_2)
		{
			this["LastToken"] = string_2;
		}

		[SpecialName]
		public string method_14()
		{
			return this["Phone"];
		}

		[SpecialName]
		public void method_15(string string_2)
		{
			this["Phone"] = string_2;
		}

		[SpecialName]
		public string method_16()
		{
			return this["Stage"];
		}

		[SpecialName]
		public void method_17(string string_2)
		{
			this["Stage"] = string_2;
		}

		[SpecialName]
		public string method_18()
		{
			return this["Subject"];
		}

		[SpecialName]
		public void method_19(string string_2)
		{
			this["Subject"] = string_2;
		}

		[SpecialName]
		public string method_20()
		{
			return this["StageName"];
		}

		[SpecialName]
		public void method_21(string string_2)
		{
			this["StageName"] = string_2;
		}

		[SpecialName]
		public string method_22()
		{
			return this["SubjectName"];
		}

		[SpecialName]
		public void method_23(string string_2)
		{
			this["SubjectName"] = string_2;
		}
	}
}
