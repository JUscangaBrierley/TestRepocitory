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
    public class AppFabricCachingSection : ConfigurationSection
    {
        public AppFabricCachingSection()
        {
            SetupDefaults();
        }

        public RegionElementCollection Regions
        {
            get { return this["regions"] as RegionElementCollection; }
        }
		
        private void SetupDefaults()
        {
            Properties.Add(new ConfigurationProperty("regions", typeof(RegionElementCollection), null));
        }
    }
}
