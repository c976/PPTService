using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Cvte.Aop
{
	public class DynamicProxy<SA_T0> : RealProxy, IDisposable where SA_T0 : MarshalByRefObject
	{
		private readonly IInterceptor _interceptor;

		public DynamicProxy(SA_T0 target, IInterceptor interceptor)
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
			SA_T0 val = (SA_T0)GetUnwrappedServer();
			if (_interceptor == null)
			{
				goto IL_006a;
			}
			object ret = _interceptor.Intercept(val, methodInfo, methodCallMessageWrapper.Args);
			goto IL_007d;
			IL_006a:
			ret = methodInfo.Invoke(val, methodCallMessageWrapper.Args);
			goto IL_007d;
			IL_007d:
			if (0 == 0)
			{
				return new ReturnMessage(ret, methodCallMessageWrapper.Args, methodCallMessageWrapper.ArgCount, methodCallMessageWrapper.LogicalCallContext, methodCallMessageWrapper);
			}
			goto IL_006a;
		}

		public void Dispose()
		{
			DetachServer();
		}
	}
}
