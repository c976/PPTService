using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Framework.Controls.Texts
{
	public interface IPaintTextEditors
	{
		[NotNull]
		IReadOnlyList<PaintTextEditor> GetContainsTextEditors();

		[NotNull]
		IReadOnlyList<PaintTextEditor> GetSelectedTextEditors();
	}
}
