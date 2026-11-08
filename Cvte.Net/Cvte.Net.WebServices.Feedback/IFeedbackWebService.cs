using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Feedback
{
	
	public interface IFeedbackWebService
	{
		void Init(string appId, string appName, string appVersion);

		Task<bool> Commit(UserInfo user, string title, string content, IList<string> pictures, IList<string> attachments, IList<CustomInfo> softwareInfos);
	}
}
