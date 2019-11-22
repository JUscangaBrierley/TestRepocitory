using System;
using System.Collections.Generic;
using System.Text;


namespace Brierley.FrameWork.bScript
{
	/// <summary>
	/// The string constant class works much like the Constant class except its value is a string
	/// </summary>
	[Serializable]
	public class StringConstant : Expression
	{
		private string _value;

		/// <summary>
		/// Internal constructor
		/// </summary>
		/// <param name="value">The string value to hold</param>
		public StringConstant(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			if (!value.Contains("'"))
			{
				_value = value;
				return;
			}

			//remove quotes, if the value is wrapped in them. This will turn the string literal 'hello\' into hello\
			//This may look like a bug, but the expression parser would never recognize it as a string, since the ending 
			//quote is escaped; we shouldn't see any string like that make it through.
			if (value.Length > 1 && value.StartsWith("'") && value.EndsWith("'"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			//placeholder for escaped backslashes
			string bs = "]";
			while (value.Contains(bs))
			{
				bs += "X";
			}

			_value = value
				.Replace("\\\\", bs)			//first pull all escaped backslashes so we don't mistakenly interpret \\' as ' instead of \'
				.Replace("\\r", "\r")			//... now run through all of the escape characters: r
				.Replace("\\n", "\n")			//													n
				.Replace("\\t", "\t")			//													t
				.Replace("\\'", "\'")			//													'
				.Replace(bs, "\\");				//and then put back the escaped backslashes
		}

		/// <summary>
		/// returns the string value being held by the container.
		/// </summary>
		/// <param name="contextObject">the current evaluation context</param>
		/// <returns>The string</returns>
		public override object evaluate(ContextObject contextObject)
		{
			return _value ?? string.Empty;
		}

		/// <summary>
		/// returns the string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _value;
		}

		public override string parseMetaData()
		{
			return string.Format("'{0}'", _value);
		}
	}
}
