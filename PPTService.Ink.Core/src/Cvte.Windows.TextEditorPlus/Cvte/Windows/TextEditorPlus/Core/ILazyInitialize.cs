namespace PPTService.Ink.Core.Windows.TextEditorPlus.Core
{
	internal interface ILazyInitialize
	{
		void InitializeUntilEditable();

		void InitializeUntilNotEmpty();
	}
}
