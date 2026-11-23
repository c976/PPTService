using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Aop
{
	
	
	[Obsolete("AOP 功能无法在跨平台的核心库实现，请寻找其他替代方案。", true)]
	public class Interceptor : IInterceptor
	{
		private readonly IList<AspectAdvice> _advices;

		public Interceptor(params AspectAdvice[] advices)
		{
			if (advices == null)
			{
				throw new ArgumentNullException("advices");
			}
			_advices = advices;
		}

		public object Intercept(object target, MethodInfo method, object[] args)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			try
			{
				foreach (AspectAdvice advice in _advices)
				{
					advice.Before(target, method, args);
				}
				object result = method.Invoke(target, args);
				foreach (AspectAdvice advice2 in _advices)
				{
					advice2.After(target, method, args);
				}
				return result;
			}
			catch (Exception ex)
			{
				foreach (AspectAdvice advice3 in _advices)
				{
					advice3.Throw(target, method, ex, args);
				}
				return null;
			}
			finally
			{
				foreach (AspectAdvice advice4 in _advices)
				{
					advice4.AfterEnsure(target, method, args);
				}
			}
		}
	}
}
