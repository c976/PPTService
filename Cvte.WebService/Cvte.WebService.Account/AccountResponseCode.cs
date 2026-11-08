namespace Cvte.WebService.Account
{
	public enum AccountResponseCode
	{
		Unknow = -1,
		WebError = 1,
		InvalidUri = 2,
		TimeOut = 3,
		UserNameOrPasswordIncorrect = 401,
		UserNameNotExist = 402,
		PasswordIncorrect = 403,
		UserNameNotExist2 = 404,
		OK = 200,
		UserIdAlreadyExists = 703
	}
}
