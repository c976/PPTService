using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Aop
{
	[Obsolete("AOP 功能无法在跨平台的核心库实现，请寻找其他替代方案。", true)]
	public class DynamicProxy<T> : IDisposable where T :  MarshalByRefObject
	{
		void IDisposable.Dispose()
		{
		}
	}
}
