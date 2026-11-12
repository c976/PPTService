namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	internal interface IEraserView
	{
		double X { get; set; }

		double Y { get; set; }

		double Rotation { get; set; }

		double Width { get; set; }

		double Height { get; set; }
	}
}
