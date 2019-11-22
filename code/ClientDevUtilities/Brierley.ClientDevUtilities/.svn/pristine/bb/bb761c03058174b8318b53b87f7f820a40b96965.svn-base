using System;
using System.Text.RegularExpressions;

namespace Brierley.FrameWork.Common
{
	public static class PhoneUtil
	{
		/// <summary>
		/// Masks a phone number using the provided format.
		/// </summary>
		/// <param name="phone">The phone number to mask. All formats allowed - it will be converted to raw before masking</param>
		/// <param name="mask">The mask to apply</param>
		/// <exception cref="ArgumentNullException">thrown if mask is null or empty</exception>
		/// <exception cref="ArgumentException">thrown if mask does not contain at least one "#"</exception>
		/// <returns></returns>
		/// <example>
		/// MaskPhoneNumber("214-555-1234", "(###) ###-####")
		/// MaskPhoneNumber("214.123.4567", "###-###-####")
		/// </example>
		public static string MaskPhoneNumber(string phone, string mask)
		{
			const string escapedPound = "@@@!!POUND!!@@@";
			const string escapedBackslash = "@@@!!BACKSLASH!!@@@";

			if (string.IsNullOrEmpty(mask))
			{
				throw new ArgumentNullException("mask");
			}
			if (!mask.Contains("#"))
			{
				throw new ArgumentException(string.Format("Invalid mask format. Mask should contain at least one occurrence of \"#\". Mask: {0}", mask));
			}
			if (string.IsNullOrEmpty(phone))
			{
				return phone;
			}
			phone = ToRawPhoneNumber(phone);

			if (mask.Contains("\\\\"))
			{
				mask = mask.Replace("\\\\", escapedBackslash);
			}
			if (mask.Contains("\\#"))
			{
				mask = mask.Replace("\\#", escapedPound);
			}

			int index = 0;
			string ret = string.Empty;
			foreach (var character in mask)
			{
				if (character == '#')
				{
					if (phone.Length > index)
					{
						ret += phone[index++];
					}
				}
				else
				{
					ret += character;
				}
			}

			if (ret.Contains(escapedBackslash))
			{
				ret = ret.Replace(escapedBackslash, "\\");
			}
			if (ret.Contains(escapedPound))
			{
				ret = ret.Replace(escapedPound, "#");
			}

			return ret;
		}

		/// <summary>
		/// Converts the specified phone number into its raw format, removing all non-numeric characters.
		/// </summary>
		/// <param name="phone">The phone number to convert</param>
		/// <param name="convertLetters">
		/// When true, converts any letters in the phone number to their numeric mapping. 
		/// When false, any letters that appear in the phone number are removed.
		/// </param>
		/// <exception cref="ArgumentNullException">thrown if phone is null</exception>
		/// <returns></returns>
		public static string ToRawPhoneNumber(string phone, bool convertLetters = true)
		{
			if (phone == null)
			{
				throw new ArgumentNullException("phone");
			}
			if (convertLetters && Regex.IsMatch(phone, "[a-zA-Z]"))
			{
				phone = Regex.Replace(phone, "[a-zA-Z]", new MatchEvaluator(LetterEval));
			}
			return Regex.Replace(phone, "[^0-9]", string.Empty);
		}

		private static string LetterEval(Match m)
		{
			string ret = m.Value;
			switch (m.Value.ToLower())
			{
				case "a":
				case "b":
				case "c":
					ret = "2";
					break;
				case "d":
				case "e":
				case "f":
					ret = "3";
					break;
				case "g":
				case "h":
				case "i":
					ret = "4";
					break;
				case "j":
				case "k":
				case "l":
					ret = "5";
					break;
				case "m":
				case "n":
				case "o":
					ret = "6";
					break;
				case "p":
				case "q":
				case "r":
				case "s":
					ret = "7";
					break;
				case "t":
				case "u":
				case "v":
					ret = "8";
					break;
				case "w":
				case "x":
				case "y":
				case "z":
					ret = "9";
					break;
			}
			return ret;
		}
	}
}
