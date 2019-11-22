using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Interfaces
{
	public interface IAuthenticateMemberInterceptor : ILWInterceptor
	{
		/// <summary>
		/// Called just before authenticating the member.  Interceptor can throw an exception
		/// to veto authentication of the user before the standard logic is called.
		/// </summary>
		/// <param name="identityType">type of identity, e.g., username</param>
		/// <param name="identity">identity provided by the user</param>
		/// <param name="password">cleartext password provided by the user</param>
		void BeforeAuthenticate(AuthenticationFields identityType, string identity, string password, string resetCode);

        Member AuthenticateMember(AuthenticationFields identityType, string identity, string password, string resetCode, ref LoginStatusEnum loginStatus);

		/// <summary>
		/// Called when the user has authenticated successfully.  Interceptor can veto
		/// authentication by throwing an exception.
		/// </summary>
		/// <param name="member">resolved member</param>
		/// <param name="loginStatus">login status enum</param>
		void AfterAuthenticateOK(Member member, LoginStatusEnum loginStatus);

		/// <summary>
		/// Called when an AuthenticationException has been caught during authentication process.  Interceptor
		/// can handle the error and return a boolean 'true' indicating that the member should be authenticated anyway 
		/// or 'false' to continue error processing or can throw a different exception type.
		/// 
		/// By default, if the loginStatus indicates the member's password needs to be changed, the member will not
		/// authenticate.  This interceptor can override this behavior so that the member will authenticate in such case.
		/// </summary>
		/// <param name="member">resolved member (can be null)</param>
		/// <param name="loginStatus">login status enum</param>
		/// <param name="authenticationException">the AuthenticationException that was caught</param>
		/// <returns>boolean indicating whether the user should be authenticated anyway</returns>
		bool HandleAuthenticationException(Member member, LoginStatusEnum loginStatus, AuthenticationException authenticationException);
	}
}
