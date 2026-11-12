using System;

namespace Cvte.Paint.Framework.Controls.Texts
{
	public interface INotifyTextEditorCollectionChanged : IPaintTextEditors
	{
		event EventHandler<InternalTextEditorCollectionChangedEventArgs> TextEditorCollectionChanged;
	}
}
