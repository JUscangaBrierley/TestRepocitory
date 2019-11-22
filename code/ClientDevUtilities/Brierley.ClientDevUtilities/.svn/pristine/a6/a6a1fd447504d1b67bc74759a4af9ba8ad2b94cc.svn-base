using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
    public interface IAttributeSetContainer
    {
        #region Properties
        bool IsDirty { get; set; }
        long MyKey { get; set; }
        IAttributeSetContainer Parent { get; set; }
        List<ContextObject.RuleResult> Results { get; }
        #endregion

        #region Interface Methods
		
        #region Transient Properties
		void AddTransientProperty(string propertyName, object propertyValue);
		object GetTransientProperty(string propertyName);
        bool HasTransientProperty(string propertyName);
        void UpdateTransientProperty(string propertyName, object propertyValue);
        ICollection GetTransientPropertyNames();
		#endregion

        string GetAttributeSetName();
        AttributeSetMetaData GetMetaData();
        bool IsLoaded(string attSetName);
        void AddChildAttributeSet(IClientDataObject attSet);
        void SetChildAttributeSet(string attSetName, IEnumerable<IClientDataObject> attSetList);
        Dictionary<string, List<IClientDataObject>> GetChildAttributeSets();
        List<IClientDataObject> GetChildAttributeSets(string attSetName, bool deep = false);
        #endregion

        #region Clone
        IAttributeSetContainer Clone();
        IAttributeSetContainer Clone(IAttributeSetContainer dest);        
        #endregion
    }
}
