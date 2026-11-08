namespace Cvte.Net.WebServices.Courseware
{
	public enum CoursewareHttpResponseCode
	{
		Ok = 0,
		ServerError = 1001,
		IllegalUrl = 1010,
		IllegalArgument = 2004,
		UnknowError = 4000,
		AuthFail = 4001,
		CoursewareNotFound = 4101,
		CoursewareConflict = 4102,
		CoursewareIsLatest = 4103,
		CoursewareNeedPush = 4104,
		CoursewareVersionNotFound = 4105,
		InvalidFileType = 4201,
		ServerBusy = 4301,
		CloudSpaceSizeLimit = 4510
	}
}
