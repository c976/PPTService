using System;
using System.Reflection;

namespace Cvte.Aop
{
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
