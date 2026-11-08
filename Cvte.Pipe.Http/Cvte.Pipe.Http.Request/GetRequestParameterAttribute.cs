using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class GetRequestParameterAttribute : Attribute
	{
		[CompilerGenerated]
		public dynamic _0001;

		[CompilerGenerated]

		public int Order
		{
			[CompilerGenerated]
			get
			{
				return this._0001;
			}
			[CompilerGenerated]
			set
			{
				this._0001 = value;
			}
		}

		public string Format
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
	}
}
