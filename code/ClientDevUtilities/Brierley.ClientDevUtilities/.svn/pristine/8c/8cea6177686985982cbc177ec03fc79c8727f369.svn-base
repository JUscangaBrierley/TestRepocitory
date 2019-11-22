using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailInterceptor : ILWInterceptor
    {        
        /// <summary>
        /// This method is caleld to generate a subject line.  If it returns null and a subject line is specified
        /// in the xml configuration then that subject line is used.
        /// </summary>
        /// <returns></returns>
        string GetSubject(AsyncJob job);

        /// <summary>
        /// This operation is used to override the default body of the email.
        /// </summary>
        /// <returns></returns>
        string GetBody(AsyncJob job);
    }
}
