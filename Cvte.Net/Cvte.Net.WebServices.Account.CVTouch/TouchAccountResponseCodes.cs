namespace Cvte.Net.WebServices.Account.CVTouch
{
	public enum TouchAccountResponseCodes
	{
		Default = -2,
		Unknow = -1,
		ServerError = 50000,
		UserNameOrPasswordIncorrect = 40300,
		IllegalId = 40002,
		CaptchaError = 40203,
		AppNameError = 40302,
		ArgumentError = 40304,
		Ok = 0
	}
}
