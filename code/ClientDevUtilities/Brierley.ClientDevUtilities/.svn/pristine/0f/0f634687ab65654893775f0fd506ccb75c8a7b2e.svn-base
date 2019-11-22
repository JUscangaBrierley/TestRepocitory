using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// A hierchical category for a Category. 
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Category")]
    [AuditLog(true)]
    [UniqueIndex(ColumnName = "ParentCategoryID,Name", RequiresLowerFunction = false)]
	public class Category : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current Category
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the ParentCategoryID for the current Category
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ColumnIndex]
		public long ParentCategoryID { get; set; }

		/// <summary>
		/// Gets or sets the IsVisibleInLn for the current Category
		/// </summary>
        [PetaPoco.Column]
		public bool? IsVisibleInLn { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current Category
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current Category
		/// </summary>
        [PetaPoco.Column(Length = 1000)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the CategoryType for the current Category
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public CategoryType CategoryType { get; set; }


		/// <summary>
		/// Initializes a new instance of the Category class
		/// </summary>
		public Category()
		{
			ID = -1;
			ParentCategoryID = 0;
			IsVisibleInLn = true;
			CategoryType = CategoryType.Product;
		}

		public Category Clone()
		{
			return Clone(new Category());
		}

		public Category Clone(Category dest)
		{
			dest.ParentCategoryID = ParentCategoryID;
			dest.Name = Name;
			dest.Description = Description;
			dest.CategoryType = CategoryType;
			base.Clone(dest);
			return dest;
		}

		public Category RetrieveDestinationCategoryFromSource(ServiceConfig sourceConfig, ServiceConfig targetConfig)
		{
			string str = BuildSourceHeirarchy(this, string.Empty, sourceConfig);
			string[] tokens = str.Split(';');
			Category cat = null;
			Category parent = null;
			using (var contentService = new ContentService(targetConfig))
			{
				foreach (string cName in tokens)
				{
					long parentId = parent != null ? parent.ID : 0;
					cat = contentService.GetCategory(parentId, cName);
					parent = cat;
				}
			}
			return cat;
		}

		public Category RetrieveDestinationParentCategoryFromSource(ServiceConfig sourceConfig, ServiceConfig targetConfig)
		{
			string str = BuildSourceHeirarchy(this, string.Empty, sourceConfig);
			string[] tokens = str.Split(';');
			Category cat = null;
			Category parent = null;
			if (tokens.Length > 1)
			{
				using (var contentService = new ContentService(targetConfig))
				{
					for (int i = 0; i < tokens.Length - 1; i++)
					{
						long parentId = parent != null ? parent.ID : 0;
						cat = contentService.GetCategory(parentId, tokens[i]);
						parent = cat;
					}
				}
			}
			return parent;
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			Category_AL ar = new Category_AL()
			{
				ObjectId = this.ID,
				Name = this.Name,
				ParentCategoryID = this.ParentCategoryID,
				IsVisibleInLn = this.IsVisibleInLn,
				Description = this.Description,
				CategoryType = this.CategoryType,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}

		private string BuildSourceHeirarchy(Category srcCat, string str, ServiceConfig sourceConfig)
		{
			if (srcCat.ParentCategoryID <= 0)
			{
				if (!string.IsNullOrEmpty(str))
				{
					str += ";";
				}
				str += srcCat.Name;
			}
			else
			{
				using (var contentService = new ContentService(sourceConfig))
				{
					Category parent = contentService.GetCategory(srcCat.ParentCategoryID);
					string catstr = BuildSourceHeirarchy(parent, str, sourceConfig);
					str = catstr + ";" + srcCat.Name;
				}
			}
			return str;
		}
	}
}