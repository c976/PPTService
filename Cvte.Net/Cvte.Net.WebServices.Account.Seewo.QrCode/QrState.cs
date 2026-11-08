namespace Cvte.Net.WebServices.Account.Seewo.QrCode
{
	public enum QrState
	{
		Ok = 200,
		Scaned = 201,
		Logined = 202,
		Disabled = 300,
		ServiceError = 500,
		NetworkError = -999,
		Unknow = -1
	}
}
