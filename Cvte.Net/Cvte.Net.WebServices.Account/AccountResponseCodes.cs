namespace Cvte.Net.WebServices.Account
{
	public enum AccountResponseCodes
	{
		Default = -2,
		Unknow = -1,
		RequestParamsIllegal = 2004,
		ServerException = 4000,
		TokenInvalid = 4001,
		TokenIsNull = 4011,
		RequestArgError = 4901,
		ValidateCodeError = 4902,
		TooManyRequests = 4903,
		ValidateCodeInvalid = 4904,
		AccountAlreadyExists = 4905,
		NeedCaptcha = 4906,
		CaptchaError = 4907,
		AccountNotExists = 4908,
		PasswordError = 4909,
		PhoneAlreadyExists = 4910,
		IdentityError = 4914,
		Ok = 0
	}
}
