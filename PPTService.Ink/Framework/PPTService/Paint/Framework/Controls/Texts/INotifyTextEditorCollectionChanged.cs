using System;

namespace PPTService.Paint.Framework.Controls.Texts
{
	public interface INotifyTextEditorCollectionChanged : IPaintTextEditors
	{
		event EventHandler<InternalTextEditorCollectionChangedEventArgs> TextEditorCollectionChanged;
	}
}
