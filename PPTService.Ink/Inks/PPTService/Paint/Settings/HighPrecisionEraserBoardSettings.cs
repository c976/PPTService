using System;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Inks.HighPrecision.Erasing;

namespace PPTService.Paint.Settings
{
	public class HighPrecisionEraserBoardSettings : BoardSettings
	{
		[CanBeNull]
		public Func<ModeInputEventArgs, IHighPrecisionEraserHandler> HighPrecisionEraserInkHandlerCreator { get; set; }

		public HighPrecisionEraserInkHandlerType HighPrecisionEraserInkHandlerType { get; set; }
	}
}
