using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Flags]
	public enum SMValidationLevel
	{
		Warning = 1,
		Performance = 2,
		Exception = 4
	}

	public class SMValidationMessage
	{
		private SMValidationLevel _validationLevel;
		private string _message = "";
		private string _exceptionMessage = null;

		public SMValidationLevel ValidationLevel
		{
			get { return _validationLevel; }
			set { _validationLevel = value; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public string ExceptionMessage
		{
			get { return _exceptionMessage; }
			set { _exceptionMessage = value; }
		}

		public SMValidationMessage()
		{
		}

		public SMValidationMessage(SMValidationLevel Level, string Message) : this(Level, Message, null)
		{
		}

		public SMValidationMessage(SMValidationLevel Level, string Message, string ExceptionMessage)
		{
			_validationLevel = Level;
			_message = Message;
			_exceptionMessage = ExceptionMessage;
		}

	}


}
