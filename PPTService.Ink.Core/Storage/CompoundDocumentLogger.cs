using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class CompoundDocumentLogger : IStorageLogger
	{
		private readonly IList<SectionLogger> _sections = new List<SectionLogger>();

		
		private SectionLogger _currentSectionLogger;

		public CompoundDocumentSaveInfo Document { get; set; }

		public IList<StorageException> WarningExceptions => _sections.SelectMany( (SectionLogger s) => s.WarningExceptions).ToList();

		public IList<StorageException> Errors => _sections.SelectMany( (SectionLogger s) => s.Errors).ToList();

		public void Info(string msg)
		{
			_currentSectionLogger?.Info(msg);
		}

		public void Warning(string msg)
		{
			_currentSectionLogger?.Warning(msg);
		}

		public void Warning(StorageException exception)
		{
			_currentSectionLogger?.Warning(exception);
		}

		public void Error(StorageException exception)
		{
			_currentSectionLogger?.Error(exception);
		}

		public void SaveReport(string reportPath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<html>");
			stringBuilder.AppendLine("<head>");
			stringBuilder.AppendLine("<meta content=\"en-us\" http-equiv=\"Content-Language\">");
			stringBuilder.AppendLine("<meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\">");
			stringBuilder.AppendLine("<title>存储报告</title>");
			stringBuilder.AppendFormat("<style>{0}</style>\r\n", GetReportStyle());
			stringBuilder.AppendFormat("<script type=\"text/javascript\" language=\"javascript\"> {0}</script>\r\n", GetReportScript());
			stringBuilder.AppendLine("</style>");
			stringBuilder.AppendLine("</head>");
			stringBuilder.AppendLine("<h1>存储报告</h2>");
			stringBuilder.AppendLine("<div id=\"content\">");
			if (Document != null)
			{
				stringBuilder.AppendLine("<h2>文档信息</h2>");
				string[][] array = new string[8][]
				{
					new string[2] { "CreatedDocumentVersion", Document.CreatedDocumentVersion },
					new string[2] { "DocumentVersion", Document.DocumentVersion },
					new string[2] { "AppVersion", Document.AppVersion },
					new string[2] { "CreatedAppVersion", Document.CreatedAppVersion },
					new string[2] { "Creator", Document.Creator },
					new string[2] { "LastModifiedBy", Document.LastModifiedBy },
					new string[2]
					{
						"CreatedDateTime",
						Document.CreatedDateTime.ToString(CultureInfo.InvariantCulture)
					},
					new string[2]
					{
						"ModifiedDateTime",
						Document.ModifiedDateTime.ToString(CultureInfo.InvariantCulture)
					}
				};
				stringBuilder.AppendLine("<table><tbody>");
				stringBuilder.AppendFormat("<tr><th>{0}</th><th>{1}</th></tr>\r\n", "属性", "值");
				foreach (string[] item in (IEnumerable<string[]>)array)
				{
					stringBuilder.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>\r\n", item[0], item[1]);
				}
				stringBuilder.AppendLine("</tbody></table>");
			}
			stringBuilder.AppendLine("<h2>概要</h2>");
			stringBuilder.AppendLine("<table><tbody>");
			stringBuilder.AppendFormat("<tr><th>{0}</th><th>{1}</th><th>{2}</th><th>{3}</th><th>{4}</th><th>{5}</th></tr>\r\n", "", "文件", "路径", "错误", "警告", "信息");
			List<SectionLogger> list = _sections.ToList();
			foreach (SectionLogger item2 in list)
			{
				string text = "IconSuccessEncoded";
				if (item2.Errors.Any())
				{
					text = "IconErrorEncoded";
				}
				else if (item2.Warnings.Any() || item2.WarningExceptions.Any())
				{
					text = "IconWarningEncoded";
				}
				stringBuilder.AppendFormat("<tr><td class=\"{0}\"></td><td><a href=\"#{6}\">{1}</a></td><td>{2}</td><td class=\"textCentered\">{3}</td><td class=\"textCentered\">{4}</td><td class=\"textCentered\">{5}</td></tr>\r\n", text, Path.GetFileName(item2.Name), item2.Name, item2.Errors.Count, item2.Warnings.Count + item2.WarningExceptions.Count, item2.Infos.Count, Path.GetFileNameWithoutExtension(item2.Name));
			}
			stringBuilder.AppendLine("</tbody></table>");
			stringBuilder.AppendLine("<h2>详情</h2>");
			stringBuilder.AppendLine("<div id=\"messages\">");
			foreach (SectionLogger item3 in list)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item3.Name);
				stringBuilder.AppendFormat("<h3><a name=\"{0}\">{0}</a></h3>\r\n", fileNameWithoutExtension);
				stringBuilder.AppendLine("<table><tbody>");
				stringBuilder.AppendFormat("<tr><th>{0}</th><th class=\"messageCell\">{1}</th></tr>\r\n", "", "信息");
				foreach (StorageException error in item3.Errors)
				{
					stringBuilder.AppendFormat("<tr><td class=\"IconErrorEncoded\"></td><td><xmp>{0}</xmp></td></tr>\r\n", error);
				}
				foreach (string warning in item3.Warnings)
				{
					stringBuilder.AppendFormat("<tr><td class=\"IconWarningEncoded\"></td><td><xmp>{0}</xmp></td></tr>\r\n", warning);
				}
				foreach (StorageException warningException in item3.WarningExceptions)
				{
					stringBuilder.AppendFormat("<tr><td class=\"IconWarningEncoded\"></td><td><xmp>{0}</xmp></td></tr>\r\n", warningException.Message);
				}
				if (item3.Infos.Any())
				{
					stringBuilder.AppendFormat("<tr name=\"MessageRowHeaderShow{0}\"><td class=\"IconInfoEncoded\"></td><td><a href=\"#\" name=\"{0}Message\" onclick=\"ToggleMessageVisibility('{0}'); return false;\">显示 {1} 其他消息</a></td></tr>\r\n", fileNameWithoutExtension, item3.Infos.Count);
					foreach (string info in item3.Infos)
					{
						stringBuilder.AppendFormat("<tr name=\"MessageRowClass{0}\" style=\"display: none;\"><td class=\"IconInfoEncoded\"></td><td>{1}</td></tr>\r\n", fileNameWithoutExtension, info);
					}
					stringBuilder.AppendFormat("<tr name=\"MessageRowHeaderHide{0}\" style=\"display: none;\"><td class=\"IconInfoEncoded\"></td><td><a href=\"#\" name=\"{0}Message\" onclick=\"ToggleMessageVisibility('{0}'); return false;\">隐藏 {1} 其他消息</a></td></tr>\r\n", fileNameWithoutExtension, item3.Infos.Count);
				}
				stringBuilder.AppendLine("</tbody></table>");
				stringBuilder.AppendLine("<div>" + $"耗时：{item3.ElapsedMilliseconds}" + "</div>");
			}
			stringBuilder.AppendLine("</div>");
			stringBuilder.AppendLine("</div>");
			stringBuilder.AppendLine("</html>");
			string content = stringBuilder.ToString();
			try
			{
				TextFileWriter.WriteAllTextSafety(Path.Combine(reportPath, (Document?.Name ?? string.Empty) + "_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss", CultureInfo.InvariantCulture) + " .html"), content);
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (Exception)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
		}

		public void Dispose()
		{
			_sections.Clear();
			_currentSectionLogger = null;
		}

		public SectionLogger Section(string name)
		{
			SectionLogger sectionLogger = new SectionLogger(name);
			_sections.Add(sectionLogger);
			_currentSectionLogger = sectionLogger;
			return sectionLogger;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<----------------------存储报告-----------------------");
			if (Document != null)
			{
				stringBuilder.AppendLine("文档信息");
				foreach (string[] item in (IEnumerable<string[]>)new string[8][]
				{
					new string[2] { "CreatedDocumentVersion", Document.CreatedDocumentVersion },
					new string[2] { "DocumentVersion", Document.DocumentVersion },
					new string[2] { "AppVersion", Document.AppVersion },
					new string[2] { "CreatedAppVersion", Document.CreatedAppVersion },
					new string[2] { "Creator", Document.Creator },
					new string[2] { "LastModifiedBy", Document.LastModifiedBy },
					new string[2]
					{
						"CreatedDateTime",
						Document.CreatedDateTime.ToString(CultureInfo.InvariantCulture)
					},
					new string[2]
					{
						"ModifiedDateTime",
						Document.ModifiedDateTime.ToString(CultureInfo.InvariantCulture)
					}
				})
				{
					stringBuilder.AppendFormat("{0}:{1}\r\n", item[0], item[1]);
				}
				stringBuilder.AppendLine();
			}
			foreach (SectionLogger section in _sections)
			{
				stringBuilder.AppendLine(section.Name);
				stringBuilder.AppendLine($"耗时：{section.ElapsedMilliseconds}");
				foreach (StorageException error in section.Errors)
				{
					stringBuilder.AppendLine(error.ToString());
				}
				foreach (string warning in section.Warnings)
				{
					stringBuilder.AppendLine(warning);
				}
				foreach (StorageException warningException in section.WarningExceptions)
				{
					stringBuilder.AppendLine(warningException.Message);
				}
			}
			stringBuilder.AppendLine("-----------------------存储报告---------------------->");
			return stringBuilder.ToString();
		}

		private static string GetReportStyle()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" /* Body style, for the entire document */");
			stringBuilder.AppendLine("body");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    background: #F3F3F4;");
			stringBuilder.AppendLine("    color: #1E1E1F;");
			stringBuilder.AppendLine("    font-family: \"Segoe UI\", Tahoma, Geneva, Verdana, sans-serif;");
			stringBuilder.AppendLine("    padding: 0;");
			stringBuilder.AppendLine("    margin: 0;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Header1 style, used for the main title */");
			stringBuilder.AppendLine("h1");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    padding: 10px 0px 10px 10px;");
			stringBuilder.AppendLine("    font-size: 21pt;");
			stringBuilder.AppendLine("    background-color: #E2E2E2;");
			stringBuilder.AppendLine("    border-bottom: 1px #C1C1C2 solid; ");
			stringBuilder.AppendLine("    color: #201F20;");
			stringBuilder.AppendLine("    margin: 0;");
			stringBuilder.AppendLine("    font-weight: normal;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Header2 style, used for \"Overview\" and other sections */");
			stringBuilder.AppendLine("h2");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    font-size: 18pt;");
			stringBuilder.AppendLine("    font-weight: normal;");
			stringBuilder.AppendLine("    padding: 15px 0 5px 0;");
			stringBuilder.AppendLine("    margin: 0;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Header3 style, used for sub-sections, such as project name */");
			stringBuilder.AppendLine("h3");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    font-weight: normal;");
			stringBuilder.AppendLine("    font-size: 15pt;");
			stringBuilder.AppendLine("    margin: 0;");
			stringBuilder.AppendLine("    padding: 15px 0 5px 0;");
			stringBuilder.AppendLine("    background-color: transparent;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Color all hyperlinks one color */");
			stringBuilder.AppendLine("a");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    color: #1382CE;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Table styles */ ");
			stringBuilder.AppendLine("table");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    border-spacing: 0 0;");
			stringBuilder.AppendLine("    border-collapse: collapse;");
			stringBuilder.AppendLine("    font-size: 10pt;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("table th");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    background: #E7E7E8;");
			stringBuilder.AppendLine("    text-align: left;");
			stringBuilder.AppendLine("    text-decoration: none;");
			stringBuilder.AppendLine("    font-weight: normal;");
			stringBuilder.AppendLine("    padding: 3px 6px 3px 6px;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("table td");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    vertical-align: top;");
			stringBuilder.AppendLine("    padding: 3px 6px 5px 5px;");
			stringBuilder.AppendLine("    margin: 0px;");
			stringBuilder.AppendLine("    border: 1px solid #E7E7E8;");
			stringBuilder.AppendLine("    background: #F7F7F8;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Local link is a style for hyperlinks that link to file:/// content, there are lots so color them as 'normal' text until the user mouse overs */");
			stringBuilder.AppendLine(".localLink");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    color: #1E1E1F;");
			stringBuilder.AppendLine("    background: #EEEEED;");
			stringBuilder.AppendLine("    text-decoration: none;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine(".localLink:hover");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    color: #1382CE;");
			stringBuilder.AppendLine("    background: #FFFF99;");
			stringBuilder.AppendLine("    text-decoration: none;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Center text, used in the over views cells that contain message level counts */ ");
			stringBuilder.AppendLine(".textCentered");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    text-align: center;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* The message cells in message tables should take up all avaliable space */");
			stringBuilder.AppendLine(".messageCell");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    width: 100%;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Padding around the content after the h1 */ ");
			stringBuilder.AppendLine("#content ");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("  padding: 0px 12px 12px 12px; ");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* The overview table expands to width, with a max width of 97% */ ");
			stringBuilder.AppendLine("#overview table");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    width: auto;");
			stringBuilder.AppendLine("    max-width: 75%; ");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* The messages tables are always 97% width */");
			stringBuilder.AppendLine("#messages table");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    width: 97%;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* All Icons */");
			stringBuilder.AppendLine(".IconSuccessEncoded, .IconInfoEncoded, .IconWarningEncoded, .IconErrorEncoded");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    min-width:18px;");
			stringBuilder.AppendLine("    min-height:18px; ");
			stringBuilder.AppendLine("    background-repeat:no-repeat;");
			stringBuilder.AppendLine("    background-position:center;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Success icon encoded */");
			stringBuilder.AppendLine(".IconSuccessEncoded");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    /* Note: Do not delete the comment below. It is used to verify the correctness of the encoded image resource below before the product is released */");
			stringBuilder.AppendLine("    /* [---XsltValidateInternal-Base64EncodedImage:IconSuccess#Begin#background-image: url(data:image/png;base64,#Separator#);#End#] */");
			stringBuilder.AppendLine("    background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAA7EAAAOxAGVKw4bAAABOElEQVR4XqVTIWvDQBh9HVWBwWzEYGow19KpQiBqrqKiYqpVkf0BcTX9ASdr2jBZMRFRe1CYysjcYDAoTIS5wCB6eXAXsjt2YezBme/73vfeF156MFBV1QLAHEBotCSAxPO8HVo4axGv6pfnn/k2lnEYPARoP9bY4wxnoUAHUIV8/bS+OLwf4MLsZobl7bIEMKzdnLSDR5GJTjKxf92Ds+SAJ/Dm2tqAjd8wuZ4gizKk92mzhBxy6WDOggvRKAKxed78cEIuF4THj6NT3T/3UXwVSN9SaChOyAV/ULfRhwHeScWVXIEw1C2YDqiklTvV9QIZXAZQoBIVqexUVxzJBQnDYbuAU11xEp1EpnBgBcmdxpc6iUOdxGk8jkvDSVeUp81HZKaZbW4Vd0LdZ9/MHmc4qzj49+/8DReTp9S7vY7uAAAAAElFTkSuQmCC);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Information icon encoded */");
			stringBuilder.AppendLine(".IconInfoEncoded");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    /* Note: Do not delete the comment below. It is used to verify the correctness of the encoded image resource below before the product is released */");
			stringBuilder.AppendLine("    /* [---XsltValidateInternal-Base64EncodedImage:IconInformation#Begin#background-image: url(data:image/png;base64,#Separator#);#End#] */");
			stringBuilder.AppendLine("    background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAA7EAAAOxAGVKw4bAAABBElEQVR4Xs2TsYrCQBCG1wVFo3DXWHkHNvZJIdem8AXiA5i8xRXWFvcWiVibF7BIKxamtwmolY2FxsNrnF9GiVkSCGnuh2GL2e9nZvm3IlKK49ihw6YyU62AytM0zU0ClQTYpcNfHv50d3MRi/31hR50asLpNcRXuxoSYJFRxAZPeP29Or3Po1+RJ5iM9eaRMAMmkhv+JDwr8G70ea+kMB3uggEosfNj7BwpJmDAYgI7C/6YblGZJmBhYKYfTF1BFTOmFCX1PwwChKSomAlg4CEcRcWMJ5FtxHPYrReCwYCVDFg//dYRjcwcqFG2no/IH8OgRjgz33g/dWf0cAd3mRGlv/MNZL1xS4xD8eEAAAAASUVORK5CYII=);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Warning icon encoded */");
			stringBuilder.AppendLine(".IconWarningEncoded");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    /* Note: Do not delete the comment below. It is used to verify the correctness of the encoded image resource below before the product is released */");
			stringBuilder.AppendLine("    /* [---XsltValidateInternal-Base64EncodedImage:IconWarning#Begin#background-image: url(data:image/png;base64,#Separator#);#End#] */");
			stringBuilder.AppendLine("    background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAx0lEQVR4XpWSMQ7CMAxFf4xAyBMLCxMrO8dhaBcuwdCJS3RJBw7SA/QGTCxdWJgiQYWKXJWKIXHIlyw5lqr34tQgEOdcBsCOx5yZK3hCCKdYXneQkh4pEfqzLfu+wVDSyyzFoJjfz9NB+pAF+eizx2Vruts0k15mPgvS6GYvpVtQhB61IB/dk6AF6fS4Ben0uIX5odtFe8Q/eW1KvFeH4e8khT6+gm5B+t3juyDt7n0jpe+CANTd+oTUjN/U3yVaABnSUjFz/gFq44JaVSCXeQAAAABJRU5ErkJggg==);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("/* Error icon encoded */");
			stringBuilder.AppendLine(".IconErrorEncoded");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("    /* Note: Do not delete the comment below. It is used to verify the correctness of the encoded image resource below before the product is released */");
			stringBuilder.AppendLine("    /* [---XsltValidateInternal-Base64EncodedImage:IconError#Begin#background-image: url(data:image/png;base64,#Separator#);#End#] */");
			stringBuilder.AppendLine("    background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAA7EAAAOxAGVKw4bAAABP0lEQVR4XqWTvUoDQRSFr4NBWRAD+gA+gMXmDWJlFbC3iCBY2Qm2YmttKyhYaaFtGiPY2CWFD2BjEwiugkv8XecbcmHmEgTJgUs2371ndmY4OyNGZVluedD21TStW19nWZadKrDGFV+915tONdhsVU/LkhSMHjPMqi/sYAx6b3s79Y/Lc/lLc9u7kh0cFR40/G4e3ZhflYf7wVxbzUNZKX8/ORZm8cAdZ/66v8tpYFy67lLJIpYziwev48JGHsRyi3UdVnNgsfDg5fzpZTXz6rt4rhC/0TO9ZBbvrBh9PvRluLGWvPXnpYCFnpWTKWUXiM/Mm6n4TiYGqKvBMefn/0SmwcLriOe8D4fKnlnvBBYLD15NIinMNUjIXFjMNY19n8RGEmUSRkj+HWUeAKy6cNGR2npLrGD0mFEzfOrP+Rf4+xT8EskwMAAAAABJRU5ErkJggg==);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine(" ");
			return stringBuilder.ToString();
		}

		private static string GetReportScript()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("// Startup ");
			stringBuilder.AppendLine("// Hook up the the loaded event for the document/window, to linkify the document content");
			stringBuilder.AppendLine("var startupFunction = function () { linkifyElement(\"messages\"); };");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("if (window.attachEvent) {");
			stringBuilder.AppendLine("    window.attachEvent('onload', startupFunction);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("else if (window.addEventListener) {");
			stringBuilder.AppendLine("    window.addEventListener('load', startupFunction, false);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("else {");
			stringBuilder.AppendLine("    document.addEventListener('load', startupFunction, false);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("// Toggles the visibility of table rows with the specified name ");
			stringBuilder.AppendLine("function toggleTableRowsByName(name) {");
			stringBuilder.AppendLine("    var allRows = document.getElementsByTagName('tr');");
			stringBuilder.AppendLine("    for (i = 0; i < allRows.length; i++) {");
			stringBuilder.AppendLine("        var currentName = allRows[i].getAttribute('name');");
			stringBuilder.AppendLine("        if (!!currentName && currentName.indexOf(name) == 0) {");
			stringBuilder.AppendLine("            var isVisible = allRows[i].style.display == '';");
			stringBuilder.AppendLine("            isVisible ? allRows[i].style.display = 'none' : allRows[i].style.display = '';");
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("function scrollToFirstVisibleRow(name) {");
			stringBuilder.AppendLine("    var allRows = document.getElementsByTagName('tr');");
			stringBuilder.AppendLine("    for (i = 0; i < allRows.length; i++) {");
			stringBuilder.AppendLine("        var currentName = allRows[i].getAttribute('name');");
			stringBuilder.AppendLine("        var isVisible = allRows[i].style.display == '';");
			stringBuilder.AppendLine("        if (!!currentName && currentName.indexOf(name) == 0 && isVisible) {");
			stringBuilder.AppendLine("            allRows[i].scrollIntoView(true);");
			stringBuilder.AppendLine("            return true;");
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("    return false;");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("// Linkifies the specified text content, replaces candidate links with html links ");
			stringBuilder.AppendLine("function linkify(text) {");
			stringBuilder.AppendLine("    if (!text || 0 === text.length) {");
			stringBuilder.AppendLine("        return text;");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("    // Find http, https and ftp links and replace them with hyper links ");
			stringBuilder.AppendLine("    var urlLink = /(http|https|ftp)\\:\\/\\/[a-zA-Z0-9\\-\\.]+(:[a-zA-Z0-9]*)?\\/?([a-zA-Z0-9\\-\\._\\?\\,\\/\\\\\\+&%\\$#\\=~;\\{\\}])*/gi;");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("    return text.replace(urlLink, '<a href=\"$&\">$&</a>');");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("// Linkifies the specified element by ID");
			stringBuilder.AppendLine("function linkifyElement(id) {");
			stringBuilder.AppendLine("    var element = document.getElementById(id);");
			stringBuilder.AppendLine("    if (!!element) {");
			stringBuilder.AppendLine("        element.innerHTML = linkify(element.innerHTML);");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("function ToggleMessageVisibility(projectName) {");
			stringBuilder.AppendLine("    if (!projectName || 0 === projectName.length) {");
			stringBuilder.AppendLine("        return;");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("    toggleTableRowsByName(\"MessageRowClass\" + projectName);");
			stringBuilder.AppendLine("    toggleTableRowsByName('MessageRowHeaderShow' + projectName);");
			stringBuilder.AppendLine("    toggleTableRowsByName('MessageRowHeaderHide' + projectName);");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("function ScrollToFirstVisibleMessage(projectName) {");
			stringBuilder.AppendLine("    if (!projectName || 0 === projectName.length) {");
			stringBuilder.AppendLine("        return;");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("    // First try the 'Show messages' row");
			stringBuilder.AppendLine("    if (!scrollToFirstVisibleRow('MessageRowHeaderShow' + projectName)) {");
			stringBuilder.AppendLine("        // Failed to find a visible row for 'Show messages', try an actual message row ");
			stringBuilder.AppendLine("        scrollToFirstVisibleRow('MessageRowClass' + projectName);");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}
}
