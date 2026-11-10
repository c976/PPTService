using System.Runtime.CompilerServices;

namespace Cvte.WebService.Feedback
{
	public class UserInfo
	{
		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private string string_4;

		public string UserId
		{
			get
			{
				return string_0;
			}
			set
			{
				string_0 = value;
			}
		}

		public string Email
		{
			get
			{
				return string_1;
			}
			set
			{
				string_1 = value;
			}
		}

		public string Wenxin
		{
			get
			{
				return string_2;
			}
			set
			{
				string_2 = value;
			}
		}

		public string Mobile
		{
			get
			{
				return string_3;
			}
			set
			{
				string_3 = value;
			}
		}

		public string Device
		{
			get
			{
				return string_4;
			}
			set
			{
				string_4 = value;
			}
		}

		public bool Verify()
		{
			if (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Wenxin) && string.IsNullOrEmpty(Mobile))
			{
				return !string.IsNullOrEmpty(Device);
			}
			return true;
		}
	}
}
