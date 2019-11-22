using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.LWIntegration.Util
{
    public class CategoryUtil
    {
        #region Fields
        private const string _className = "LWDAPCouponProcessor";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);        
        #endregion

        public static Category CreateCategory(string catName)
        {
            string methodName = "CreateCategory(synchronized)";
            string[] cdelim = { "->" };
            string[] catList = catName.Split(cdelim, StringSplitOptions.RemoveEmptyEntries);
            Category cat = null;
            Category parent = null;

			using (var service = LWDataServiceUtil.ContentServiceInstance())
			{
				foreach (string cName in catList)
				{
					long parentId = parent != null ? parent.ID : 0;
					_logger.Debug(_className, methodName, "Looking for " + cName + " with parent id = " + parentId);
					cat = service.GetCategory(parentId, cName);
					if (cat == null)
					{
						_logger.Debug(_className, methodName, "Category " + cName + " with parent id = " + parentId + " not found.  Creating it.");
						// create it
						cat = new Category();
						cat.ParentCategoryID = parentId;
						cat.Name = cName;
						cat.CategoryType = CategoryType.Product;
						service.AddCategory(cat);
						_logger.Debug(_className, methodName, "Created category " + cat.Name + " with id " + cat.ID);
					}
					parent = cat;
				}

				return cat;
			}
        }

        public static Category RetrieveCategory(string catName)
        {
            string[] cdelim = { "->" };
            string[] catList = catName.Split(cdelim, StringSplitOptions.RemoveEmptyEntries);
            Category cat = null;
            Category parent = null;

			using (var service = LWDataServiceUtil.ContentServiceInstance())
			{
				foreach (string cName in catList)
				{
					long parentId = parent != null ? parent.ID : 0;
					cat = service.GetCategory(parentId, cName);
					if (cat != null)
					{
						parent = cat;
					}
					else
					{
						break;
					}
				}
				return cat;
			}
        }
        
        protected static string BuildCategoryName(string catName, Category category)
        {
            if (category.ParentCategoryID <= 0)
            {
                // we have reached the top level category
                catName = category.Name;
            }
            else
            {
                // not a parent category
				using (var service = LWDataServiceUtil.ContentServiceInstance())
				{
					Category parent = service.GetCategory(category.ParentCategoryID);
					catName = BuildCategoryName(catName, parent);
					catName = catName + "->" + category.Name;
				}
            }
            return catName;            
        }

        public static string GetCategoryName(Category cat)
        {
            string name = BuildCategoryName("", cat);
            return name;
        }
    }
}
