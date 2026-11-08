namespace Cvte.Net.WebServices.CloudDisk
{
	public enum CloudDiskResponseCodes
	{
		Default = -2,
		Unknow = -1,
		Success = 200,
		AuthFail = 401,
		FileOrDirectoryNotFound = 404,
		ServerError = 500,
		InvalidFileState = 600,
		InvalidFileFormat = 601,
		ArgumentError = 602,
		InvalidOperation = 603
	}
}
