using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	internal class BindPhoneOrEmailBody : AccountRequestBody
	{
		[CompilerGenerated]
		private string _0001;

		[CompilerGenerated]
		private string _0002;

		[CompilerGenerated]
		private string _0003;

		[CompilerGenerated]
		private string _0004;

		[CompilerGenerated]
		private string _0005;

		[DataMember(Name = "tokenId")]
		public string TokenId
		{
			[CompilerGenerated]
			get
			{
				return _0001;
			}
			[CompilerGenerated]
			set
			{
				_0001 = value;
			}
		}

		[DataMember(Name = "timeStamp")]
		public string TimeSpan
		{
			[CompilerGenerated]
			get
			{
				return _0002;
			}
			[CompilerGenerated]
			set
			{
				_0002 = value;
			}
		}

		[DataMember(Name = "target")]
		public string PhoneOrEmail
		{
			[CompilerGenerated]
			get
			{
				return _0003;
			}
			[CompilerGenerated]
			set
			{
				_0003 = value;
			}
		}

		[DataMember(Name = "code")]
		public string ValidateCode
		{
			[CompilerGenerated]
			get
			{
				return _0004;
			}
			[CompilerGenerated]
			set
			{
				_0004 = value;
			}
		}

		[DataMember(Name = "sendType")]
		public string SendType
		{
			[CompilerGenerated]
			get
			{
				return _0005;
			}
			[CompilerGenerated]
			set
			{
				_0005 = value;
			}
		}
	}
}
