using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public interface IPaintInitializerCollector
	{
		IEnumerable<Func<IPaintInitializer>> Collect(IPaintInitializerCollectionContext context);
	}
}
