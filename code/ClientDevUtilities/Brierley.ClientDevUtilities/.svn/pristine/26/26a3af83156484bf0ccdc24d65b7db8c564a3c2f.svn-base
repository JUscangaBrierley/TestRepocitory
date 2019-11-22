using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.CampaignManagement
{
	[Flags]
	public enum ValidationLevel
	{
		Warning = 1,
		Performance = 2,
		Exception = 4
	}

	public class ValidationMessage
	{
		public ValidationLevel ValidationLevel { get; set; }

		public string Message { get; set; }

		public string ExceptionMessage { get; set; }

		public ValidationMessage()
		{
		}

		public ValidationMessage(ValidationLevel Level, string Message) : this(Level, Message, null)
		{
		}

		public ValidationMessage(ValidationLevel validationLevel, string message, string exceptionMessage)
		{
			ValidationLevel = validationLevel;
			Message = message;
			ExceptionMessage = exceptionMessage;
		}
	}
}
