using System;

namespace Cvte.Composition
{
	public static class TypeDetectorExtensions
	{
		public static void OnTypeDetected<T>(this ITypeDetector typeDetector, Action<T> typeProcessingMethod, bool processTypesAlreadyDetected = false)
		{
			typeDetector.OnTypeDetected(typeProcessingMethod, (Type t) => true, processTypesAlreadyDetected);
		}

		public static void OnTypeDetected<T>(this ITypeDetector typeDetector, Action<T> typeProcessingMethod, Predicate<Type> typeFilter, bool processTypesAlreadyDetected = false)
		{
			if (processTypesAlreadyDetected)
			{
				foreach (T detectedObject in typeDetector.GetDetectedObjects<T>())
				{
					typeProcessingMethod(detectedObject);
				}
			}
			typeDetector.NewTypesDetected += (object sender, NewTypesDetectedEventArgs e) =>
			{
				foreach (T item in e.GetTypeOf<T>(typeFilter))
				{
					typeProcessingMethod(item);
				}
			};
		}
	}
}
