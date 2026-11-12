using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework.Controls.Texts
{
	[Obsolete("发现一些小伙伴对这个接口的理解和实现五花八门，导致新业务依赖于此接口时获取到的可能不是正确的结果，因此我们决定更换为 IPaintTextEditors 接口。")]
	public interface IContainsTextEditors
	{
		List<PaintTextEditor> GetContainsTextEditors();
	}
}
