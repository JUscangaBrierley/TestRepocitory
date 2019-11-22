using System;

namespace Brierley.FrameWork.Common.Logging
{
	/// <summary>
	/// Interface for logging Email and SMS communications.
	/// </summary>
	/// <remarks>
	/// This interface is intended to be used for logging failed communications (e.g., DMC's CreateUser, UpdateProfile and SendSingle calls; AWS and other triggered emails). 
	/// These failed calls would typically need to be queued for retry. 
	/// 
	/// The original EmailQueue had the ability to log successful calls, therefore DMCService will pass all calls, failure or otherwise, through to 
	/// the class implementing this interface. It is up to the implementation to determine how/whether to log the message.
	/// </remarks>
	public interface ICommunicationLogger
	{
		/// <summary>
		/// Logs a message.
		/// </summary>
		/// <param name="data"></param>
		void LogMessage(CommunicationLogData data);

		/// <summary>
		/// Logs a DMC message.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="queuedForRetry">
		/// boolean indicating whether or not the message was logged. A true value indicates that the caller may consider the message 
		/// "handled", meaning that a failed message attempt has been queued for retry and the exception should not be re-thrown.
		/// </param>
		void LogMessage(CommunicationLogData data, out bool queuedForRetry);
	}
}
