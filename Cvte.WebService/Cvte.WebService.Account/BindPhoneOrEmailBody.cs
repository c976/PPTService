using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	internal class BindPhoneOrEmailBody : AccountRequestBody
	{
		private string string_0;

		private string string_1;

		private string string_2;

		private string string_3;

		private string string_4;

		[DataMember(Name = "tokenId")]
		public string TokenId
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

		[DataMember(Name = "timeStamp")]
		public string TimeSpan
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

		[DataMember(Name = "target")]
		public string PhoneOrEmail
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

		[DataMember(Name = "code")]
		public string ValidateCode
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

		[DataMember(Name = "sendType")]
		public string SendType
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
	}
}
