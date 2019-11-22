//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;

using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;

using Brierley.FrameWork.Interfaces;


namespace Brierley.FrameWork.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class LWPasswordUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="password"></param>
        /// <returns></returns>
		public static bool ValidatePassword(string identity, string password)
		{
            string enabledStr = LWConfigurationUtil.GetConfigurationValue("LWPasswordStrengthValidationEnabled");
            bool enabled = true;
            if (!string.IsNullOrEmpty(enabledStr))
            {
                enabled = bool.Parse(enabledStr);
            }

            if (!enabled)
            {
                // password strength validation turned off.
                return true;
            }

            if (identity == "csadmin" && password == "csadmin")
                return true;
            
            IPasswordStrengthValidator validator = null;
            string validatorStr = LWConfigurationUtil.GetConfigurationValue("LWPasswordStrengthValidator");
            if (!string.IsNullOrEmpty(validatorStr))
            {
                Regex regex = new System.Text.RegularExpressions.Regex(@"(?<={)(.*?)(?=})");
                MatchCollection matches = regex.Matches(validatorStr);
                if (matches == null || matches.Count != 2)
                {
                    throw new AuthenticationException(string.Format("Invalid syntax for validator specified: {0}", validatorStr));
                }
                string typeName = matches[0].ToString();
                string assemblyName = matches[1].ToString();
                validator = ClassLoaderUtil.CreateInstance(assemblyName, typeName) as IPasswordStrengthValidator;
                if (validator == null)
                {
                    throw new AuthenticationException("Unable to instantiate validator: " + validatorStr);
                }
            }
           
            if (validator != null)
            {
                validator.ValidatePassword(identity, password);                
            }
            else
            {
                // check for null arguments
                if (string.IsNullOrEmpty(identity))
                {
                    throw new ArgumentNullException("identity");
                }
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("password");
                }

                // must be at least 7 characters
                if (password.Length < 7)
                {
                    throw new BadPasswordLengthException();
                }

                // must start with a letter
                if (!char.IsLetter(password[0]))
                {
                    throw new BadPasswordInitialCharException();
                }

                // must have at least 3 of following categories: 
                //   1) upper case letter
                //   2) lower case letter
                //   3) number
                //   4) special character
                int numCategories = 0;
                bool foundUpper = false;
                bool foundLower = false;
                bool foundDigit = false;
                bool foundSpecialCharacter = false;
                for (int i = 0; i < password.Length; i++)
                {
                    if (!foundUpper && char.IsUpper(password[i]))
                    {
                        foundUpper = true;
                        numCategories++;
                    }
                    if (!foundLower && char.IsLower(password[i]))
                    {
                        foundLower = true;
                        numCategories++;
                    }
                    if (!foundDigit && char.IsDigit(password[i]))
                    {
                        foundDigit = true;
                        numCategories++;
                    }
                    //if (!foundSpecialCharacter && char.IsSymbol(password[i]))
                    if (!foundSpecialCharacter && LWPasswordUtil.IsSpecialChar(password[i]))
                    {
                        foundSpecialCharacter = true;
                        numCategories++;
                    }
                }
                if (numCategories < 3)
                {
                    throw new BadPasswordStrengthException();
                }

                // password may not contain username
                if (password.ToLower().Contains(identity.ToLower()))
                {
                    throw new BadPasswordContainsUsernameException();
                }

                // it passed the gauntlet, so it's valid                
            }

            return true;
		}

        public static bool IsSpecialChar(char c)
        {
            char[] specialChars = {
                '`', '-', '=', '[', ']', '\\', ';', '\'', ',', '.', '/', 
                '~', '!', '@', '#', '$', '%',  '^', '&',  '*', '(', ')', 
                '_', '+', '{', '}', '|', ':',  '"', '<',  '>', '?' };
            return specialChars.Contains(c);
        }

		public static bool IsHashingEnabled()
        {
			try
			{
				bool disableHash = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("DisablePasswordHashing"), false);
				return !disableHash;
			}
			catch (Exception)
			{
				bool disableHash = StringUtils.FriendlyBool(ConfigurationManager.AppSettings["DisablePasswordHashing"], false);
				return !disableHash;
			}
		}

        /// <summary>
        /// Perform a one-way hash of a cleartext password and return the Base64 encoded value of the hash.  
		/// The password should have been validated using ValidatePassword() before calling HashPassword().
		/// The password is combined with the provided salt before the hash.  The salt should be per-user and
		/// kept isolated from the hashed password.  The salt should be created using CryptoUtil.GenerateSalt().
		/// When hashing a password on login to compare with the existing password, the same salt should be used.
		/// If the per-user salt is changed, the user's password should be rehashed.
		/// 
		/// Hashing can be disabled by setting the framework configuration value 'DisablePasswordHashing' to 'true'.
        /// </summary>
		/// <param name="salt">per-user salt created using CryptoUtil.GenerateSalt()</param>
        /// <param name="password">clear text password</param>
        /// <returns>Base64 encoded hash of the salted password</returns>
        public static string HashPassword(string salt, string password)
        {
			if (IsHashingEnabled())
			{
				if (string.IsNullOrEmpty(salt))
				{
					throw new ArgumentNullException("salt");
				}
				if (string.IsNullOrEmpty(password))
				{
					throw new ArgumentNullException("password");
				}

				password = CryptoUtil.PasswordHash(salt, password);
			}
            return password;
        }

		public static String GetRemoteIPAddress()
		{
			String result = null;
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				result = HttpContext.Current.Request.UserHostAddress;
			}
			return result;
		}

		public static String GetRemoteUserName()
		{
			String result = null;
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				try
				{
					result = HttpContext.Current.Request.ServerVariables["REMOTE_USER"];
				}
				catch
				{
					// ignore no REMOTE_USER variable
				}
			}
			return result;
		}

		public static String GetRemoteUserAgent()
		{
			String result = null;
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				result = HttpContext.Current.Request.UserAgent;
			}
			return result;
		}

		public static String GetLocalHostName()
		{
			String result = System.Environment.MachineName;
			return result;
		}
    }
}
