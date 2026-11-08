using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Myou.Sunday
{
	
	public interface ISundayReporter
	{
		Task ReportErrorWithAttachmentAsync(string businessKey, string subKey, string errorType, string extraMessage, Exception exception, IDictionary<string, string> otherMessage, string attachmentPath);

		Task ReportErrorWithAttachmentAsync(string businessKey, string subKey, string errorType, string extraMessage, StackTrace stackTrace, IDictionary<string, string> otherMessage, string attachmentPath);
	}
}
