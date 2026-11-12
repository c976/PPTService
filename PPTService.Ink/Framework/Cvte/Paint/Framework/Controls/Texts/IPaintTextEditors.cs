using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Controls.Texts
{
	public interface IPaintTextEditors
	{
		[NotNull]
		IReadOnlyList<PaintTextEditor> GetContainsTextEditors();

		[NotNull]
		IReadOnlyList<PaintTextEditor> GetSelectedTextEditors();
	}
}
