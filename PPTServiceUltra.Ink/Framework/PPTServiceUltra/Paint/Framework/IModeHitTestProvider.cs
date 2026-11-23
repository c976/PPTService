namespace PPTServiceUltra.Paint.Framework
{
	public interface IModeHitTestProvider
	{
		Slide Slide { get; }

		Element Element { get; }
	}
}
