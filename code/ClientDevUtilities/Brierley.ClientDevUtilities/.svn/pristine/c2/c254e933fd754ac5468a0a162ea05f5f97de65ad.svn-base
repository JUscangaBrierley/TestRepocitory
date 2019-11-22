using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
    public interface IClientDataObject : IAttributeSetContainer
    {
        #region Common Properties
        Int64 RowKey { get; set; }
        Int64 ParentRowKey { get; set; }
        Int64 StatusCode { get; set; }
        //DateTime LastDmlDate { get; set; }
        DateTime CreateDate { get; set; }
        DateTime? UpdateDate { get; set; }
        Int64? LastDmlId { get; set; }
        #endregion

        #region Interface Methods
        void SetLinkKey(IAttributeSetContainer owner, IAttributeSetContainer root);
		void SetAttributeValue(string attributueName, object attributeValue);
		void SetAttributeValue(string attributueName, object attributeValue, AttributeMetaData attMeta);
        object GetAttributeValue(string attributueName);
        bool HasAttribute(string attributeName);
        #endregion
    }
}
