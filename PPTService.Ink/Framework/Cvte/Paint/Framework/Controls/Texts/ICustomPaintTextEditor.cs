namespace Cvte.Paint.Framework.Controls.Texts
{
	public interface ICustomPaintTextEditor : IPaintTextEditors
	{
		bool IsSupportPropertyFeedback { get; }

		bool IsContextMenuExtendable { get; }
	}
}
