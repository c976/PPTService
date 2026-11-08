using System;
using System.Collections.Generic;

namespace Cvte.Composition
{
	internal interface ITypeCollection
	{
		IEnumerable<T> OfType<T>(Predicate<Type> typeFilter);
	}
}
