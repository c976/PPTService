namespace Cvte.Paint.Framework.Level
{
	public interface ILevelContainer : IElementContainer
	{
		ILevelManager LevelStorage { get; }
	}
}
