namespace Cvte.Net.Http
{
	public enum QiniuResponseCode
	{
		Ok = 200,
		RequestFormatError = 400,
		IllegalToken = 401,
		ExceedFileSize = 413,
		CallbackFailed = 579,
		UnknownError = 599,
		FileAlreadyExists = 614
	}
}
