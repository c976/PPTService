namespace PPTService.Ink.Core.Windows
{
	public static class LoadingScreenExtensions
	{
		public static void Show(this ILoadingScreen loadingScreen)
		{
			loadingScreen.Show("");
		}
	}
}
