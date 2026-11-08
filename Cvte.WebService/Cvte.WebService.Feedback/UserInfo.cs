using System.Runtime.CompilerServices;

namespace Cvte.WebService.Feedback
{
	public class UserInfo
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

		public string UserId
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

		public string Email
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

		public string Wenxin
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

		public string Mobile
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

		public string Device
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

		public bool Verify()
		{
			bool flag = SA_Proxy._001D___0016(UserId);
			int num;
			while (true)
			{
				if (flag)
				{
					flag = SA_Proxy._001D___0016(Email);
					if (2 == 0)
					{
						continue;
					}
					if (flag && SA_Proxy._001D___0016(Wenxin))
					{
						num = (SA_Proxy._001D___0016(Mobile) ? 1 : 0);
						while (num != 0)
						{
							num = ((!SA_Proxy._001D___0016(Device)) ? 1 : 0);
							if (5 != 0)
							{
								return (byte)num != 0;
							}
							if (num != 0)
							{
								goto end_IL_000e;
							}
						}
					}
				}
				num = 1;
				break;
				continue;
				end_IL_000e:
				break;
			}
			return (byte)num != 0;
		}
	}
}
