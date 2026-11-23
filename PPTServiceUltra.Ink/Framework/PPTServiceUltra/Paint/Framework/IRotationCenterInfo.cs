namespace PPTServiceUltra.Paint.Framework
{
	public interface IRotationCenterInfo
	{
		bool IsVisible { get; }

		void SetVisible(bool isVisible, bool isSetByUser);
	}
}
