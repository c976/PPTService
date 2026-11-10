using System;
using System.Collections.Generic;
using System.Reflection;
using Cvte.Linq;

namespace Cvte.Aop
{
	public class Interceptor : IInterceptor
	{
		private readonly IList<AspectAdvice> _advices;

		public Interceptor(params AspectAdvice[] advices)
		{
			_advices = advices;
			if (_advices == null)
			{
				throw new ArgumentNullException("advices");
			}
		}

		public object Intercept(object target, MethodInfo method, object[] args)
		{
			try
			{
				_advices.ForEach((AspectAdvice advice) =>
				{
					advice.Before(target, method, args);
				});
				object result = method.Invoke(target, args);
				_advices.ForEach((AspectAdvice advice) =>
				{
					advice.After(target, method, args);
				});
				return result;
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Exception ex3 = ex2;
				_advices?.ForEach((AspectAdvice advice) =>
				{
					advice.Throw(target, method, ex3, args);
				});
				return null;
			}
			finally
			{
				_advices?.ForEach((AspectAdvice advice) =>
				{
					advice.AfterEnsure(target, method, args);
				});
			}
		}
	}
}
