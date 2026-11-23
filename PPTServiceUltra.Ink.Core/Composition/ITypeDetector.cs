using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Composition
{
	
	public interface ITypeDetector
	{
		event EventHandler<NewTypesDetectedEventArgs> NewTypesDetected;

		void Detect(Assembly assembly);

		ReadOnlyCollection<T> GetDetectedObjects< T>();
	}
}
