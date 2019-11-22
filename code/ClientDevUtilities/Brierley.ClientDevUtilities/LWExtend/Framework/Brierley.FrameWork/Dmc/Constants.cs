using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Dmc
{
	/// <summary>
	/// DMC's error code enumeration. 
	/// </summary>
	/// <remarks>
	/// One of these codes is returned with the known DMC exceptions found in 
	/// the <see cref="Brierley.FrameWork.Dmc.Exceptions"/> namespace.
	/// </remarks>
	public enum ErrorCode
	{
		ARCHIVED_ATTRIBUTE,			// Name of the attribute that is archived.
		INCORRECT_STATUS,
		INVALID_ATTRIBUTE,			// Name of the attribute that generates an error.
		INVALID_ATTRIBUTE_VALUE,	// Value of the attribute that generates an error.
		INVALID_JOB_ARGUMENT,
		INVALID_JOB_DEFINITION_ID,
		INVALID_PARAMETER,			// A parameter value is invalid.
		INVALID_REQUEST,			//	The request could not be processed. Please check the error detail message.
		MAXIMUM_ATTRIBUTE_COUNT_EXCEEDED, //	The maximum number of custom user attributes has been reached. No further attributes can be created.
		MAXIMUM_SIZE_EXCEEDED,
		MISSING_JOB_ARGUMENT,
		MISSING_PARAMETER,			//	A mandatory parameter is missing.
		NO_SUCH_ATTRIBUTE,			//	There is no attribute with the given name.
		NO_SUCH_OBJECT,				//	An object reference points to an object which does not exist any more.
		OBJECT_ALREADY_EXISTS,		//	An object to be created already exists.
		OPERATION_DISABLED,			//	This operation is disabled for this system.
		PERMISSION_DENIED,			//	The user operating the API does not have the required permission.
		UNEXPECTED_ERROR			//	An unexpected backend error has occurred.
	}

	public class Constants
	{
		//DMC framework configuration settings
		public const string DmcUrl = "DmcUrl";
		public const string DmcUsername = "DmcUsername";
		public const string DmcPassword = "DmcPassword";
		public const string DmcUseAlternateEmail = "DmcUseAlternateEmail";
        public const string DmcUseAlternateMobile = "DmcUseAlternateMobile";
	}
}
