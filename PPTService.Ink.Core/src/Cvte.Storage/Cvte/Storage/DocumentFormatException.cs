using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class DocumentFormatException : DocumentException
	{
		public override string Message => InternalExceptions.FirstOrDefault()?.Message ?? "文档包含无法识别的内容";

		public IList<StorageException> InternalExceptions { get; }

		public IList<StorageException> Warnings { get; set; }

		
		
		public StorageModel Model
		{
			
			get;
		}

		public DocumentFormatException(IList<StorageException> exceptions)
			: this(null, new List<StorageException>(0), exceptions)
		{
		}

		public DocumentFormatException( StorageModel model, IList<StorageException> warnings, IList<StorageException> errors)
		{
			Model = model;
			Warnings = warnings;
			InternalExceptions = errors;
			if (errors.Any())
			{
				Info = errors.First().Info;
			}
			else if (warnings.Any())
			{
				Info = warnings.First().Info;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString());
			foreach (StorageException internalException in InternalExceptions)
			{
				stringBuilder.AppendLine(internalException?.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
