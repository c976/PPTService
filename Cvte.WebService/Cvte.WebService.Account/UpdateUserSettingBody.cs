using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	internal class UpdateUserSettingBody : AccountRequestBody
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

		[CompilerGenerated]
		private string _0006;

		[DataMember(Name = "resourceid")]
		public string ResourceId
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

		[DataMember(Name = "tokenId")]
		public string TokenId
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

		[DataMember(Name = "timeStamp")]
		public string TimeSpan
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

		[DataMember(Name = "cnName")]
		public string CnName
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

		[DataMember(Name = "stageId")]
		public string StageId
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

		[DataMember(Name = "subjectId")]
		public string SubjectId
		{
			[CompilerGenerated]
			get
			{
				return _0006;
			}
			[CompilerGenerated]
			set
			{
				_0006 = value;
			}
		}
	}
}
