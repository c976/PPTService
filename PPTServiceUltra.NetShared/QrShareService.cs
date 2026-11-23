using System;
using System.Collections.Generic;
using System.Threading;
using PPTServiceUltra.Core.WebService.Lychee;

namespace PPTServiceUltra.NetShared
{
	/// <summary>
	/// 二维码上传服务入口。主程序通过反射调用本类型，不在编译期引用本程序集；
	/// 删除 PPTServiceUltra.NetShared.dll 后主程序照常运行，仅二维码功能不可用。
	/// </summary>
	public static class QrShareService
	{
		/// <summary>由 PptSharer 按固定签名反射调用，参数全部是 BCL 类型，不跨程序集暴露自定义类型。</summary>
		public static bool Publish(IList<string> files, string title, double scale,
			CancellationToken cancellationToken, Action<double> progressChanged, out string url)
		{
			LycheeService service = new LycheeService("http://lizhi.seewo.com", "pptservice", "0f6bb4dc70e901e21eba5dfdec6c68f6");
			PublicInfo info = new PublicInfo
			{
				Author = "",
				Title = title,
				Scale = scale
			};
			return service.Publish(files, info, cancellationToken, progressChanged, out url);
		}
	}
}
