using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	[PetaPoco.ExplicitColumns]
    public class AttributeSetContainer : LWCoreObjectBase, IAttributeSetContainer
    {
        #region Private Fields
        private const string _className = "AttributeSetContainer";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        Dictionary<string, List<IClientDataObject>> children = new Dictionary<string, List<IClientDataObject>>();
        Dictionary<string, bool> loadedMap = new Dictionary<string, bool>();
        private bool isDirty = false;
        private long myKey = -1;
        IAttributeSetContainer parent = null;
        private List<ContextObject.RuleResult> _results = null;
		OrderedDictionary _transientProperties = new OrderedDictionary();
        #endregion

        #region Properties
        public virtual bool IsDirty 
        {
            get { return isDirty; }
            set { isDirty = value; }
        }
        public virtual long MyKey
        {
            get { return myKey; }
            set { myKey = value; }
        }

        [System.Xml.Serialization.XmlIgnore]
        public virtual IAttributeSetContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        
        [System.Xml.Serialization.XmlIgnore]
        public virtual List<ContextObject.RuleResult> Results
        {
            get
            {
                if (_results == null)
                {
                    _results = new List<ContextObject.RuleResult>();
                }
                return _results;
            }
        }
        #endregion

        #region Private Helpers
		protected bool IsMyChild(string attSetName)
		{
			bool isMyChild = false;
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData childMeta = service.GetAttributeSetMetaData(attSetName);
				if (this.GetType() == typeof(Member) && childMeta.ParentID == -1)
				{
					if (childMeta.Type == AttributeSetType.Member)
					{
						isMyChild = true;
					}
				}
				else if (this.GetType() == typeof(VirtualCard) && childMeta.ParentID == -1)
				{
					if (childMeta.Type == AttributeSetType.VirtualCard)
					{
						isMyChild = true;
					}
				}
				else
				{
					AttributeSetMetaData myMeta = GetMetaData();
					isMyChild = (myMeta.GetChildAttributeSet(childMeta.ID) != null) ? true : false;
				}
				return isMyChild;
			}
		}
        
        private void SetLoaded(string attSetName,bool loadedFlag)
        {
            if (loadedMap.ContainsKey(attSetName))
            {
                loadedMap[attSetName] = loadedFlag;
            }
            else
            {
                loadedMap.Add(attSetName, loadedFlag);
            }            
        }

        #endregion

        #region Interface Methods

		#region Transient Properties
        public virtual void AddTransientProperty(string propertyName, object propertyValue)
        {
            lock (_transientProperties)
            {
                _transientProperties.Add(propertyName.ToLower(), propertyValue);
            }
        }

        public virtual void UpdateTransientProperty(string propertyName, object propertyValue)
		{
			lock (_transientProperties)
			{
                if (_transientProperties.Contains(propertyName.ToLower()))
                {
                    _transientProperties.Remove(propertyName.ToLower());
                }
				_transientProperties.Add(propertyName.ToLower(), propertyValue);
			}
		}

        public virtual object GetTransientProperty(string propertyName)
		{
			lock (_transientProperties)
			{
				return _transientProperties[propertyName.ToLower()];
			}
		}
        public virtual bool HasTransientProperty(string propertyName)
		{
			lock (_transientProperties)
			{
				return _transientProperties.Contains(propertyName.ToLower());
			}
		}
        public virtual ICollection GetTransientPropertyNames()
        {
            lock (_transientProperties)
            {
                return _transientProperties.Keys;
            }
        }

		#endregion 

        public virtual bool IsLoaded(string attSetName)
        {
            bool isLoaded = false;
            if (loadedMap.ContainsKey(attSetName))
            {
                isLoaded = loadedMap[attSetName];
            }
            return isLoaded;
        }

        public virtual string GetAttributeSetName()
        {
            return "ClientDataObject";
        }

        public virtual AttributeSetMetaData GetMetaData()
        {
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				return svc.GetAttributeSetMetaData(GetAttributeSetName());
			}
        }

        public virtual Dictionary<string, List<IClientDataObject>> GetChildAttributeSets()
        {
            return children;
        }

        public virtual List<IClientDataObject> GetChildAttributeSets(string attSetName, bool deep = false)
        {
            if (!IsLoaded(attSetName))
            {
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					svc.LoadAttributeSetList(this, attSetName, deep);
				}
            }
            List<IClientDataObject> list = null;
            lock (children)
            {
                if (children.ContainsKey(attSetName))
                {
                    list = children[attSetName];
                }
                else
                {
                    list = new List<IClientDataObject>();
                    children.Add(attSetName, list);
                }
            }
            return list;
        }

        public virtual void AddChildAttributeSet(IClientDataObject attSet)
        {
            string methodName = "AddChildAttributeSet";

            try
            {
                AttributeSetMetaData meta = attSet.GetMetaData();
                if (!IsMyChild(meta.Name))
                {
                    throw new LWMetaDataException("Not a child attribute set.");
                }
                List<IClientDataObject> list = null;
                if (children.ContainsKey(meta.Name))
                {
                    list = children[meta.Name];
                }
                else
                {
                    list = new List<IClientDataObject>();
                    children.Add(meta.Name, list);
                }
                attSet.Parent = this;
                attSet.ParentRowKey = MyKey;
                list.Add(attSet);
                SetLoaded(meta.Name, true);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error adding attribute set.", ex);
                throw;
            }
        }

        public virtual void SetChildAttributeSet(string attSetName,IEnumerable<IClientDataObject> attSetList)
        {
            string methodName = "AddChildAttributeSet";

            try
            {
                if (!IsMyChild(attSetName))
                {
                    throw new LWMetaDataException("Not a child attribute set.");
                }
                if (children.ContainsKey(attSetName))
                {
					children[attSetName] = attSetList.ToList();
                }
                else
                {
                    children.Add(attSetName, attSetList.ToList());
                }
                SetLoaded(attSetName, true);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error adding attribute set.", ex);
                throw;
            }
        }

        #endregion

        #region Clone
        public virtual IAttributeSetContainer Clone()
        {
            return Clone(new AttributeSetContainer());
        }

        public virtual IAttributeSetContainer Clone(IAttributeSetContainer dest)
        {
            return dest;
        }
        #endregion
    }
}
