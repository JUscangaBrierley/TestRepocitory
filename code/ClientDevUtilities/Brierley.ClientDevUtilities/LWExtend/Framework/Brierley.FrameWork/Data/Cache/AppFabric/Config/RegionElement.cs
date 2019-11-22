//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.Cache.AppFabric.Config
{
    public class RegionElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get { return StringUtils.FriendlyBool(this["enabled"], true); }
            set { this["enabled"] = value; }
        }

        [ConfigurationProperty("timeToLive")]
        public int TimeToLive
        {
            get { return StringUtils.FriendlyInt32(this["timeToLive"], 60); }
            set { this["timeToLive"] = value; }
        }
    }
}

