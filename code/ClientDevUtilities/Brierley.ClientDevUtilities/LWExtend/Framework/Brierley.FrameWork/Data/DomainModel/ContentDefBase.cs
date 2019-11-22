//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines a base for content.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
    public class ContentDefBase : LangChanContentBase
	{

		/// <summary>
		/// Initializes a new instance of the CouponDef class
		/// </summary>
        public ContentDefBase(ContentObjType type) : base(type)
		{			
            Attributes = new List<ContentAttribute>();
		}
        		
        /// <summary>
        /// Gets or sets the Attributes for the current RewardDef
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public virtual IList<ContentAttribute> Attributes { get; set; }

        /// <summary>
        /// This property is really there for Xml serialization because serialization does not work
        /// on interfaces.
        /// </summary>
        public virtual List<ContentAttribute> ContentAttributes
        {
            get
            {
                return new List<ContentAttribute>(Attributes);
            }
            set
            {
                Attributes = new List<ContentAttribute>(value);
            }
        }

        public virtual ContentDefBase Clone(ContentDefBase dest)
		{
            return (ContentDefBase)base.Clone(dest);			
		}        
	}
}
