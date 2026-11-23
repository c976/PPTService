using System;
using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IPaintInitializerCollector
	{
		IEnumerable<Func<IPaintInitializer>> Collect(IPaintInitializerCollectionContext context);
	}
}
