namespace Cvte.Net.WebServices.Account.Seewo.UserCenter.Models
{
	internal enum QrAccountResponseCodes
	{
		Default = -2,
		Unknow = -1,
		Ok = 200,
		QrCodeScaned = 201,
		QrLogined = 202,
		QrCodeDisabled = 300,
		QrCodeServiceError = 500
	}
}
