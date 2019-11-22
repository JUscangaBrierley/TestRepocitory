//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Brierley.FrameWork.Data.Cache.AppFabric.Config
{
    [ConfigurationCollection(typeof(RegionElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class RegionElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RegionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RegionElement)element).Name;
        }

        public void Add(RegionElement regionElement)
        {
            BaseAdd(regionElement);
        }
    }
}
