namespace Cvte.Net.WebServices.CloudDisk
{
	public enum FileState
	{
		Unknow = -1,
		Uploading = 1,
		Uploaded = 2,
		Processing = 3,
		ProcessingFailed = 4,
		Finish = 5
	}
}
