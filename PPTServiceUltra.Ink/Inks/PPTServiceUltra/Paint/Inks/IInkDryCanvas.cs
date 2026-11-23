namespace PPTServiceUltra.Paint.Inks
{
	internal interface IInkDryCanvas
	{
		IInkSynchronizer InkSynchronizer { get; set; }

		InkCanvas InkCanvas { get; }
	}
}
