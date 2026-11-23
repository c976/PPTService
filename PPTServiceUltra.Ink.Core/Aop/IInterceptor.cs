using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Aop
{
	
	[Obsolete("AOP 功能无法在跨平台的核心库实现，请寻找其他替代方案。", true)]
	public interface IInterceptor
	{
		object Intercept(object target, MethodInfo method, object[] args);
	}
}
