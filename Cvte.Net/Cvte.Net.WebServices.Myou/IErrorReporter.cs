using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Myou
{
	
	public interface IErrorReporter
	{
		Task ReportErrorAsync(Exception exception);

		Task ReportErrorAsync(Exception exception, string type);

		Task ReportErrorAsync(Exception exception, string type, ErrorPriority priority);

		Task ReportErrorAsync(Exception exception, string type, ErrorPriority priority, string context);

		[Obsolete("此方法使用可能造成误导，请使用 SundayReporter。", true)]
		Task ReportErrorWithAttachmentAsync(Exception exception, string type, ErrorPriority priority, string context, string attachmentPath);

		[Obsolete("此方法使用可能造成误导，请使用 SundayReporter。", true)]
		Task ReportErrorWithAttachmentAsync(Exception exception, string type, ErrorPriority priority, IDictionary<string, string> properties, string attachmentPath);
	}
}
