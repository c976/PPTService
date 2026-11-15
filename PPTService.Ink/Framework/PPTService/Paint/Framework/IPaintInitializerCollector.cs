using System;
using System.Collections.Generic;

namespace PPTService.Paint.Framework
{
	public interface IPaintInitializerCollector
	{
		IEnumerable<Func<IPaintInitializer>> Collect(IPaintInitializerCollectionContext context);
	}
}
