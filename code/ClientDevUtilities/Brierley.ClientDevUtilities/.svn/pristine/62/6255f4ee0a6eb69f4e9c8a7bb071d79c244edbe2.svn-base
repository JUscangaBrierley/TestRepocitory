using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Data
{
    public class LWObjectBinderUtil
    {
        #region Enumerations
        public enum BindableLWObjects { Promotion, Store };
        #endregion

        #region General Methods        
        public static string[] GetBindableObjects()
        {
            return Enum.GetNames(typeof(BindableLWObjects));
        }

        public static string[] GetBindableProperties(string objectname)
        {
            BindableLWObjects fwObject = (BindableLWObjects)Enum.Parse(typeof(BindableLWObjects), objectname);
            switch (fwObject)
            {
                case BindableLWObjects.Store:
                    return GetBindableStoreProperties();
                case BindableLWObjects.Promotion:
                    return GetBindablePromotionProperties();
                default:
                    return new string[] { };
            }
        }

        public static Dictionary<long, string> GetObjectValues(string objectName, string bindableProperty, string sqlString)
        {
            BindableLWObjects fwObject = (BindableLWObjects)Enum.Parse(typeof(BindableLWObjects), objectName);
            switch (fwObject)
            {
                case BindableLWObjects.Promotion:
                    return GetPromotions(bindableProperty, sqlString);
                case BindableLWObjects.Store:
                    return GetStores(bindableProperty, sqlString);
                default:
                    throw new LWException("Invalid object specified.");                    
            }
        }
        #endregion

        #region Promotion
        private static string[] GetBindablePromotionProperties()
        {
            return new string[] { "Code", "Name " };
        }

        private static Dictionary<long, string> GetPromotions(string bindableProperty, string hql)
        {
            bool propertyAllowable = (from x in GetBindablePromotionProperties() where bindableProperty == x select 1).Count() > 0;
            if (!propertyAllowable)
            {
                // probably should throw an exception
				return new Dictionary<long, string>();
            }
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				return content.GetPromotionsByProperty(bindableProperty, hql);
			}
        }
        #endregion

        #region Store
        private static string[] GetBindableStoreProperties()
        {
            return new string[] { "StoreNumber", "StoreName ", "BrandStoreNumber" };
        }

		private static Dictionary<long, string> GetStores(string bindableProperty, string sql)
        {
            bool propertyAllowable = (from x in GetBindableStoreProperties() where bindableProperty == x select 1).Count() > 0;
            if (!propertyAllowable)
            {
                // probably should throw an exception
				return new Dictionary<long, string>();
            }
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				return content.GetStoreDefsByProperty(bindableProperty, sql);
			}
        }
        #endregion
    }
}
