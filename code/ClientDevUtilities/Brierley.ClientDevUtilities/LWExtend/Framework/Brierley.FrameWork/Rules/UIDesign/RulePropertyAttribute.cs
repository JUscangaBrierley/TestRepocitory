using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Rules.UIDesign
{
    /// <summary>
    /// 
    /// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RulePropertyAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExpression"></param>
        /// <param name="isArrayProperty"></param>
        /// <param name="ArraySource"></param>
		/// <param name="Advanced"></param>
        public RulePropertyAttribute(bool isExpression, bool isArrayProperty, bool isMultiSelect, string arraySource, bool isRequired = false, bool advanced = false)
        {
            IsExpression = isExpression;
            IsArrayProperty = isArrayProperty;
            IsMultiSelect = isMultiSelect;
            IsRequired = isRequired;
            ArraySource = arraySource;
			Advanced = advanced;
        }

        /// <summary>
        /// 
        /// </summary>
		public bool IsExpression { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsArrayProperty{get;set;}

        /// <summary>
        /// 
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public bool IsMultiSelect { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public string ArraySource { get; set; }


		/// <summary>
		/// Gets or sets whether the property should only be shown in the configuration screen's advanced view
		/// </summary>
		public bool Advanced { get; set; }

    }
}
