using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Common.Config
{
    /// <summary>
    /// This class contains the tuple that defines the environment, including Organization, Application and the Environment.
    /// </summary>
    public class LWConfigurationContext
    {
        private string _org = "";
        private string _env = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="org"></param>
        /// <param name="env"></param>
        internal LWConfigurationContext(string org, string env)
        {
            _org = org;
            _env = env;
        }

        /// <summary>
        /// Organization.
        /// </summary>
        public string Organization
        {
            get { return _org; }
        }

        /// <summary>
        /// Environment.
        /// </summary>
        public string Environment
        {
            get { return _env; }
        }
    }
}
