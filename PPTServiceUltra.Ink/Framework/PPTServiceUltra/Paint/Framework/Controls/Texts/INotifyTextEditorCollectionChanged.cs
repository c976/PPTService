using System;

namespace PPTServiceUltra.Paint.Framework.Controls.Texts
{
	public interface INotifyTextEditorCollectionChanged : IPaintTextEditors
	{
		event EventHandler<InternalTextEditorCollectionChangedEventArgs> TextEditorCollectionChanged;
	}
}
