using System;

namespace Cvte.Pipe.Http.Request
{
	public class ReqeustAttribute : Attribute
	{
		// 原版 SmartAssembly 把多个字段命名为同一字符、靠类型区分，C# 无法表达；
		// 反编译时被合并成一个 dynamic 字段，导致 Api 与 Type 互相覆盖。这里还原为独立字段。
		private string _api;
		private ReqeustType _type;
		private string _apiPrefix;

		public string Api
		{
			get
			{
				return _api;
			}
			private set
			{
				_api = value;
			}
		}

		public ReqeustType Type
		{
			get
			{
				return _type;
			}
			private set
			{
				_type = value;
			}
		}

		public string ApiPrefix
		{
			get
			{
				return _apiPrefix;
			}
			set
			{
				_apiPrefix = value;
			}
		}

		public ReqeustAttribute(string api, ReqeustType type)
		{
			Api = api;
			Type = type;
		}
	}
}
