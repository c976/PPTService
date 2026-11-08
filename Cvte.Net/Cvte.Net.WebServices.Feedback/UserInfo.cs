using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	public class UserInfo
	{
		public string UserId { get; set; }

		public string Email { get; set; }

		public string Wenxin { get; set; }

		public string Mobile { get; set; }

		public string Device { get; set; }

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
