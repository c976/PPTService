using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Aop
{
	
	
	[Obsolete("AOP 功能无法在跨平台的核心库实现，请寻找其他替代方案。", true)]
	public abstract class AspectAdvice
	{
		public virtual void Before(object target, MethodInfo method, params object[] args)
		{
		}

		public virtual void After(object target, MethodInfo method, params object[] args)
		{
		}

		public virtual void AfterEnsure(object target, MethodInfo method, params object[] args)
		{
		}

		public virtual void Throw(object target, MethodInfo method, Exception ex, params object[] args)
		{
		}
	}
}
