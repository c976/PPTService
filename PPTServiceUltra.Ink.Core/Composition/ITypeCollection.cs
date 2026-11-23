using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Composition
{
	
	internal interface ITypeCollection
	{
		IEnumerable<T> OfType< T>(Predicate<Type> typeFilter);
	}
}
