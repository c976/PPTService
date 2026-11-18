using System;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Pipe.Http.Request
{
	public class ReqeustAttribute : Attribute
	{
		private string string_0;

		private ReqeustType reqeustType_0;

		private string string_1;

		public string Api
		{
			get
			{
				return string_0;
			}
			private set
			{
				string_0 = value;
			}
		}

		public ReqeustType Type
		{
			get
			{
				return reqeustType_0;
			}
			private set
			{
				reqeustType_0 = value;
			}
		}

		public string ApiPrefix
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

		public ReqeustAttribute(string api, ReqeustType type)
		{
			Api = api;
			Type = type;
		}
	}
}
