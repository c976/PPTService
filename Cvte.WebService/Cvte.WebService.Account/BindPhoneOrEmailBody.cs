using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	internal class BindPhoneOrEmailBody : AccountRequestBody
	{
		private string _0001;

		private string _0002;

		private string _0003;

		private string _0004;

		private string _0005;

		[DataMember(Name = "tokenId")]
		public string TokenId
		{
			get
			{
				return _0001;
			}
			set
			{
				_0001 = value;
			}
		}

		[DataMember(Name = "timeStamp")]
		public string TimeSpan
		{
			get
			{
				return _0002;
			}
			set
			{
				_0002 = value;
			}
		}

		[DataMember(Name = "target")]
		public string PhoneOrEmail
		{
			get
			{
				return _0003;
			}
			set
			{
				_0003 = value;
			}
		}

		[DataMember(Name = "code")]
		public string ValidateCode
		{
			get
			{
				return _0004;
			}
			set
			{
				_0004 = value;
			}
		}

		[DataMember(Name = "sendType")]
		public string SendType
		{
			get
			{
				return _0005;
			}
			set
			{
				_0005 = value;
			}
		}
	}
}
