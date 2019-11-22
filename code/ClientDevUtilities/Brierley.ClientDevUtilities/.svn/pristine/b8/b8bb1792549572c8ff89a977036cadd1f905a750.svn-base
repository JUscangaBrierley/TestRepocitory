//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.CodeGen.ClientDataModel
{
	public class ProjectMapping
	{
		private string assemblyName;
		private string ns;
		//private string publicKeyToken;
		private string org;
		private List<AttributeSetMetaData> attSets = null;

		public ProjectMapping(string ns, string assemblyName, string org, List<AttributeSetMetaData> attSets)
		{
			this.ns = ns;
			this.assemblyName = assemblyName;
			this.attSets = attSets;
			this.org = org;
		}

		public virtual string Namespace
		{
			get { return ns; }
		}

		public virtual string AssemblyName
		{
			get { return assemblyName; }
		}

		public virtual string OrganizationName
		{
			get { return org; }
		}

		public virtual string FrameworkAssemblyName
		{
			get { return DataServiceUtil.FrameworkAssembly; }
		}

		public virtual List<AttributeSetMetaData> AttributeSets
		{
			get { return attSets; }
		}

		public virtual List<AttributeSetMetaData> EncryptedAttributeSets
		{
			get
			{
				return attSets.Where(o => o.Attributes.Where(x => x.DataType == "String" && x.EncryptionType != AttributeEncryptionType.None).Count() > 0).ToList();
			}
		}

		public virtual string GetAttributeSetName(AttributeSetMetaData attSet)
		{
			return attSet != null ? attSet.Name : string.Empty;
		}
	}
}
