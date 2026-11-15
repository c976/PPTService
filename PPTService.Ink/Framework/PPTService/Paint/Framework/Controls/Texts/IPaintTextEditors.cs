using System.Collections.Generic;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Paint.Framework.Controls.Texts
{
	public interface IPaintTextEditors
	{
		[NotNull]
		IReadOnlyList<PaintTextEditor> GetContainsTextEditors();

		[NotNull]
		IReadOnlyList<PaintTextEditor> GetSelectedTextEditors();
	}
}
