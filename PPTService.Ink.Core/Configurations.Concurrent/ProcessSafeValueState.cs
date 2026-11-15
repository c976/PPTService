namespace PPTService.Ink.Core.Configurations.Concurrent
{
	internal enum ProcessSafeValueState
	{
		NotChanged = 0,
		Changed = 2,
		Deleted = 3
	}
}
