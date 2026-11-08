using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("users/updateUser", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class UpdateUserSettingRequest : AccountRequestBase<UpdateUserSettingBody>
	{
		public UpdateUserSettingRequest(string resourceId, string tokenId, string cnName, string stageId, string subjectId)
		{
			Body = new UpdateUserSettingBody
			{
				CnName = cnName,
				StageId = stageId,
				TokenId = tokenId,
				SubjectId = subjectId,
				ResourceId = resourceId,
				TimeSpan = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}
