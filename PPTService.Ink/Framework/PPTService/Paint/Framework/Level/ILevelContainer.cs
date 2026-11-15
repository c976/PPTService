namespace PPTService.Paint.Framework.Level
{
	public interface ILevelContainer : IElementContainer
	{
		ILevelManager LevelStorage { get; }
	}
}
