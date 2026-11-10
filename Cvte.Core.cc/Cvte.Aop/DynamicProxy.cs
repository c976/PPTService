using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Cvte.Aop
{
	public class DynamicProxy<T> : RealProxy, IDisposable where T : MarshalByRefObject
	{
		private readonly IInterceptor _interceptor;

		public DynamicProxy(T target, IInterceptor interceptor)
			: base(target.GetType())
		{
			_interceptor = interceptor;
			AttachServer(target);
		}

		public override IMessage Invoke(IMessage msg)
		{
			MethodCallMessageWrapper methodCallMessageWrapper = new MethodCallMessageWrapper((IMethodCallMessage)msg);
			MethodInfo methodInfo = methodCallMessageWrapper.MethodBase as MethodInfo;
			if (methodInfo == null)
			{
				return null;
			}
			T val = (T)GetUnwrappedServer();
			object ret = ((_interceptor == null) ? methodInfo.Invoke(val, methodCallMessageWrapper.Args) : _interceptor.Intercept(val, methodInfo, methodCallMessageWrapper.Args));
			return new ReturnMessage(ret, methodCallMessageWrapper.Args, methodCallMessageWrapper.ArgCount, methodCallMessageWrapper.LogicalCallContext, methodCallMessageWrapper);
		}

		public void Dispose()
		{
			DetachServer();
		}
	}
}
