using System;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.HighPrecision.Erasing;

namespace Cvte.Paint.Settings
{
	public class HighPrecisionEraserBoardSettings : BoardSettings
	{
		[CanBeNull]
		public Func<ModeInputEventArgs, IHighPrecisionEraserHandler> HighPrecisionEraserInkHandlerCreator { get; set; }

		public HighPrecisionEraserInkHandlerType HighPrecisionEraserInkHandlerType { get; set; }
	}
}
