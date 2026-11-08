namespace Cvte.Net.WebServices.Feedback
{
	internal enum FeedbackResponseCode
	{
		Ok = 200,
		TimestampFailed = 40101,
		AuthFailed = 40102,
		InvalidParams = 40000,
		ResourceNotFound = 40004,
		ServerError = 50000
	}
}
