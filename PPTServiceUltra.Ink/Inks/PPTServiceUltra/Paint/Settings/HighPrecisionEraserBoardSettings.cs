using System;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Paint.Framework;
using PPTServiceUltra.Paint.Framework.Settings;
using PPTServiceUltra.Paint.Inks.HighPrecision.Erasing;

namespace PPTServiceUltra.Paint.Settings
{
	public class HighPrecisionEraserBoardSettings : BoardSettings
	{
		[CanBeNull]
		public Func<ModeInputEventArgs, IHighPrecisionEraserHandler> HighPrecisionEraserInkHandlerCreator { get; set; }

		public HighPrecisionEraserInkHandlerType HighPrecisionEraserInkHandlerType { get; set; }
	}
}
