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
			object result = default(object);
			if (0 == 0)
			{
				object target2 = target;
				MethodInfo method2 = method;
				object[] args2 = args;
				try
				{
					_advices.ForEach((AspectAdvice advice) =>
					{
						advice.Before(target2, method2, args2);
					});
					do
					{
						if (6 != 0 && 0 == 0)
						{
							object obj = method2.Invoke(target2, args2);
							_advices.ForEach((AspectAdvice advice) =>
							{
								advice.After(target2, method2, args2);
							});
							result = obj;
						}
					}
					while (4 == 0);
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					Exception ex3 = ex2;
					do
					{
						IList<AspectAdvice> advices = _advices;
						if (advices == null)
						{
							continue;
						}
						advices.ForEach((AspectAdvice advice) =>
						{
							if (4 != 0)
							{
								advice.Throw(target2, method2, ex3, args2);
							}
						});
						break;
					}
					while (7 == 0);
					result = null;
				}
				finally
				{
					_advices?.ForEach((AspectAdvice advice) =>
					{
						advice.AfterEnsure(target2, method2, args2);
					});
				}
			}
			return result;
		}
	}
}
