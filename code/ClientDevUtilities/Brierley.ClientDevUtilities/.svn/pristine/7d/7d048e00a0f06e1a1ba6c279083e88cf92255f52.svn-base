//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;

using System.Collections;

namespace Brierley.FrameWork.Common
{
	/// <summary>
	/// This class can be used to parse the arguments for a program.  It supports the folloging style:
    /// program -arg1 value1 -arg2 -arg3 -arg4 value4
	/// </summary>
	public class ProgramOptions
	{
		Hashtable options_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
		public ProgramOptions(string []args)
		{
			Init(args);
		}

		private void Init(string [] args)
		{
			options_ = new Hashtable();
			for ( int i = 0; i < args.Length; i++ )
			{
				String opt = args[i];
				if ( opt.StartsWith("-") == true )
				{
					String optName = opt.Substring(1);
					/*
					 * Check for options without a value
					 * e.g. -help
					 * */
					if ( i == args.Length-1 || args[i+1].StartsWith("-") == true )
					{
						options_.Add(optName,"XXX");
					}
					else
					{
						String optValue = args[++i];
						options_.Add(optName,optValue);
					}
				}
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
		public String GetOption(String optionName)
		{
			String optValue = (String) options_[optionName];
			if ( optValue == null || optValue.Equals("XXX") == true )
			{
				// this is an option without a value
				return null;
			}
			return optValue;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
		public Boolean ContainsOption(String optionName)
		{
			return options_.ContainsKey(optionName);
		}

        public string[] GetOptionNames()
        {
            string[] keys = null;
            if (options_ != null && options_.Count > 0)
            {
                keys = new string[options_.Count];
                options_.Keys.CopyTo(keys, 0);
            }
            return keys;
        }        
	}
}
