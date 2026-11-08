using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class ReqeustAttribute : Attribute
	{
		[CompilerGenerated]
		public dynamic _0001;

		[CompilerGenerated]
		private string _0002;

		public string Api
		{
			[CompilerGenerated]
			get
			{
				return this._0001;
			}
			[CompilerGenerated]
			private set
			{
				this._0001 = value;
			}
		}

		public ReqeustType Type
		{
			[CompilerGenerated]
			get
			{
				return _0001;
			}
			[CompilerGenerated]
			private set
			{
				_0001 = value;
			}
		}

		public string ApiPrefix
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

		public ReqeustAttribute(string api, ReqeustType type)
		{
			Api = api;
			Type = type;
		}
	}
}
