using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.IO
{
	
	
	[Obsolete("当前 http://iipdev.gz.cvte.cn:8888/add 已停止维护，请更换使用 Cvte.DotNet.DataCollection 库代替，此类毫无作用")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class HttpAppender
	{
		private string _url = "http://iipdev.gz.cvte.cn:8888/add";

		[Obsolete("当前 http://iipdev.gz.cvte.cn:8888/add 已停止维护，请更换使用 Cvte.DotNet.DataCollection 库代替，此类毫无作用")]
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		[Obsolete("当前 http://iipdev.gz.cvte.cn:8888/add 已停止维护，请更换使用 Cvte.DotNet.DataCollection 库代替，此类毫无作用")]
		public bool IsDebug { get; set; }
	}
}
