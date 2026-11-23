namespace PPTServiceUltra.Paint.Framework
{
	public interface ITransformSensitive
	{
		void TransformStarted(TransformRoutedEventArgs args);

		void TransformCompleted(TransformRoutedEventArgs args);
	}
}
