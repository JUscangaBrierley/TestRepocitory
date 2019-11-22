//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.CodeGen
{
    public class DotNetProjectMapping
    {
        private string ns;
        private string assemblyName;        
        private IList<AttributeSetMetaData> attSets = null;
        private IList<string> ioClasses = null;

        public DotNetProjectMapping(string ns, string assemblyName, IList<AttributeSetMetaData> attSets, IList<string> ioClasses)
        {
            this.ns = ns;
            this.assemblyName = assemblyName;
            this.attSets = attSets;
            this.ioClasses = ioClasses; 
        }

        public virtual string Namespace
        {
            get { return ns; }
        }

        public virtual string AssemblyName
        {
            get { return assemblyName; }
        }

        public virtual string FrameworkAssemblyName
        {
            get { return DataServiceUtil.FrameworkAssembly; }
        }

        public virtual IList<AttributeSetMetaData> AttributeSets
        {
            get { return attSets; }
        }


        public virtual IList<string> IOClasses
        {
            get { return ioClasses; }
        }

        //public virtual string GetAttributeSetName(AttributeSetMetaData attSet)
        //{
        //    return attSet != null ? attSet.Name : string.Empty;
        //}

        public virtual string GeIncludedClassName(AttributeSetMetaData attSet)
        {
            return attSet != null ? @"DomainModel\Client\" + attSet.Name : string.Empty;
        }

        public virtual string GeIncludedIOClassName(string clName)
        {
            return @"DomainModel\Client\" + clName;
        }
    }
}
