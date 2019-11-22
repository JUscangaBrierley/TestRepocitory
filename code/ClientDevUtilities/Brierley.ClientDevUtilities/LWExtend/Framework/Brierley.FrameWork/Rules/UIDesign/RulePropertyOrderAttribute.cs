using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Rules.UIDesign
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RulePropertyOrderAttribute : System.Attribute
    {
        private int _order = 0;        
        
		public RulePropertyOrderAttribute(int order)
        {
            _order = order;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Order
        {
			get { return _order; }
			set { _order = value; }
        }                
    }
}
