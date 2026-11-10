using System;

namespace Cvte.Pipe.Http.Request
{
	public class GetRequestParameterAttribute : Attribute
	{
		// 原版是 int / string 两个独立字段，反编译被合并为 dynamic，这里拆开。
		private int _order;
		private string _format;

		public int Order
		{
			get
			{
				return _order;
			}
			set
			{
				_order = value;
			}
		}

		public string Format
		{
			get
			{
				return _format;
			}
			set
			{
				_format = value;
			}
		}
	}
}
