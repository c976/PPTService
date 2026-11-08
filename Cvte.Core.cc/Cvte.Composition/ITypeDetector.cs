using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Cvte.Composition
{
	public interface ITypeDetector
	{
		event EventHandler<NewTypesDetectedEventArgs> NewTypesDetected;

		void Detect(Assembly assembly);

		ReadOnlyCollection<T> GetDetectedObjects<T>();
	}
}
