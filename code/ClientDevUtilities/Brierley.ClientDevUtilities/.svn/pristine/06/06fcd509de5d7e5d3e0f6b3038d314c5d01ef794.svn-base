using System;
using System.Runtime.Serialization;

namespace Brierley.FrameWork.Common.Exceptions
{
    /// <summary>
    /// A generic LoyaltyWare application exception.  It is better
    /// to define and/or use a more specific exception if possible.
    /// </summary>
    [Serializable]
    public class LWException : ApplicationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LWException()
            : base()
        {
        }

        /// <summary>
        /// Initializes with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public LWException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes with a specified error 
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.
        /// </param>
        /// <param name="exception">The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference, the current exception 
        /// is raised in a catch block that handles the inner exception.
        /// </param>
        public LWException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Initializes with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.
        /// </param>
        protected LWException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public int ErrorCode { get; set; }
    }
}
