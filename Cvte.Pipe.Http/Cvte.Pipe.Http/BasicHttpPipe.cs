using System;
using System.Collections.Generic;
using Cvte.Pipe.Http.Request;

namespace Cvte.Pipe.Http
{
	public class BasicHttpPipe : HttpPipe
	{
		private readonly IList<Tuple<Func<RequestBase, bool>, Action<RequestBase>>> _0001 = new List<Tuple<Func<RequestBase, bool>, Action<RequestBase>>>();

		public void AddRequestFilter(Func<RequestBase, bool> func, Action<RequestBase> action)
		{
			_0001.Add(new Tuple<Func<RequestBase, bool>, Action<RequestBase>>(func, action));
		}

		protected sealed override void PreRequest(RequestBase request)
		{
			base.PreRequest(request);
			foreach (Tuple<Func<RequestBase, bool>, Action<RequestBase>> item in _0001)
			{
				if (item.Item1(request))
				{
					item.Item2(request);
				}
			}
		}
	}
}
