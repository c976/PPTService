using System.Reflection;

namespace Cvte.Aop
{
	public interface IInterceptor
	{
		object Intercept(object target, MethodInfo method, object[] args);
	}
}
