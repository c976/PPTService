using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class GetRequestParameterAttribute : Attribute
	{
		private int int_0;

		private string string_0;

		public int Order
		{
			get
			{
				return int_0;
			}
			set
			{
				int_0 = value;
			}
		}

		public string Format
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
	}
}
