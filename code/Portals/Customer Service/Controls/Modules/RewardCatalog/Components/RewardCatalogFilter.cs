using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;

namespace Brierley.LWModules.RewardCatalog.Components
{
	public class RewardCatalogFilter : ContentSearchFilter
	{
		private ViewRewardCatalog _viewRewardCatalog { get; set; }

		public List<Dictionary<string, object>> SearchParams { get { return ContentSearchParms; } }

		public RewardCatalogFilter(ViewRewardCatalog viewRewardCatalog) :
			base(ContentObjType.Reward)
		{
			SearchValues = new Dictionary<string, string>();
			SearchValues.Add("CertificateTypeCode", "Certificate Type Code");
			SearchValues.Add("Name", "Reward Name");
			SearchValues.Add("HowManyPointsToEarn", "Points Required To Earn");
			SearchValues.Add("PointType", "Loyalty Currency");
			SearchValues.Add("ProductId", "Product");
			SearchValues.Add("TierId", "Tier");
			SearchValues.Add("CatalogStartDate", "Catalog Start Date");
			SearchValues.Add("CatalogEndDate", "Catalog End Date");
			SearchValues.Add("Active", "Active");

			_viewRewardCatalog = viewRewardCatalog;
		}

		public override object ValdateAndParseSearchValue(
			string propertyName,
			TextBox txtValue,
			DropDownList drpValue,
			RadDateTimePicker cldrValue,
			out string errMsg)
		{
			object searchValue = null;
			errMsg = string.Empty;

			if (txtValue.Visible)
			{
				searchValue = txtValue.Text;
				if (propertyName == "HowManyPointsToEarn")
				{
					try
					{
						long.Parse(txtValue.Text);
					}
					catch (Exception)
					{
						errMsg = ResourceUtils.GetLocalWebResource("~/Controls/Modules/RewardCatalog/ViewRewardCatalog.ascx", "NumericValueMessage.Text", "Please enter a valid numeric value to search for.");
					}
				}
			}
			else if (drpValue.Visible)
			{
				if (propertyName == "Active")
				{
					searchValue = bool.Parse(drpValue.SelectedValue);
				}
				else
				{
					searchValue = drpValue.SelectedValue;
				}
			}
			else if (cldrValue.Visible)
			{
				if (!cldrValue.SelectedDate.HasValue)
				{
					errMsg = string.Format(ResourceUtils.GetLocalWebResource("~/Controls/Modules/RewardCatalog/ViewRewardCatalog.ascx", "NotNullMessage.Text", "Please provide a non-null value for {0} to search."), propertyName);
				}
				else
				{
					searchValue = cldrValue.SelectedDate.Value;
				}
			}
			return searchValue;
		}

		public override Dictionary<string, object> GetSearchItem(
			string propertyName,
			string selectedOperator,
			LWCriterion.Predicate predicate, object searchValue)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			switch (propertyName)
			{
				case "CertificateTypeCode":
				case "Name":
				case "HowManyPointsToEarn":
				case "PointType":
				case "ProductId":
				case "TierId":
				case "CatalogStartDate":
				case "CatalogEndDate":
				case "Active":
					break;
				default:
					dictionary.Add("IsAttribute", true);
					break;
			}

			dictionary.Add("Property", propertyName);
			dictionary.Add("Predicate", predicate);
			dictionary.Add("Value", searchValue);

			return dictionary;
		}

		public override bool InitializeSearchValueControl(string propName, TextBox txtValue, DropDownList drpValue, RadDateTimePicker cldrValue)
		{
			bool success = true;

			if (!string.IsNullOrEmpty(propName))
			{
                using(ContentService ContentService = LWDataServiceUtil.ContentServiceInstance())
                using (LoyaltyDataService LoyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    if (propName == "Name" || propName == "HowManyPointsToEarn" || propName == "CertificateTypeCode")
                    {
                        txtValue.Visible = true;
                        drpValue.Visible = false;
                        cldrValue.Visible = false;
                    }
                    else if (propName == "PointType")
                    {
                        drpValue.Items.Clear();
                        IList<PointType> ptList = LoyaltyService.GetAllPointTypes();
                        ListItem li = new ListItem();
                        li.Text = "All";
                        li.Value = "All";
                        drpValue.Items.Add(li);
                        foreach (PointType pt in ptList)
                        {
                            li = new ListItem();
                            li.Text = pt.Name;
                            li.Value = pt.Name;
                            drpValue.Items.Add(li);
                        }
                        txtValue.Visible = false;
                        drpValue.Visible = true;
                        cldrValue.Visible = false;
                    }
                    else if (propName == "ProductId")
                    {
                        drpValue.Items.Clear();
                        IList<Product> pdList = ContentService.GetAllProductsSortedByName(true);
                        foreach (Product pt in pdList)
                        {
                            ListItem li = new ListItem();
                            li.Text = pt.Name;
                            li.Value = pt.Id.ToString();
                            drpValue.Items.Add(li);
                        }
                        txtValue.Visible = false;
                        drpValue.Visible = true;
                        cldrValue.Visible = false;
                    }
                    else if (propName == "TierId")
                    {
                        drpValue.Items.Clear();
                        IList<TierDef> list = LoyaltyService.GetAllTierDefs();
                        foreach (TierDef pt in list)
                        {
                            ListItem li = new ListItem();
                            li.Text = pt.Name;
                            li.Value = pt.Id.ToString();
                            drpValue.Items.Add(li);
                        }
                        txtValue.Visible = false;
                        drpValue.Visible = true;
                        cldrValue.Visible = false;
                    }
                    else if (propName == "CatalogStartDate" || propName == "CatalogEndDate")
                    {
                        txtValue.Visible = false;
                        drpValue.Visible = false;
                        cldrValue.Visible = true;
                    }
                    else if (propName == "Active")
                    {
                        drpValue.Items.Clear();
                        ListItem li = new ListItem();
                        li.Text = "true";
                        li.Value = "true";
                        drpValue.Items.Add(li);
                        li = new ListItem();
                        li.Text = "false";
                        li.Value = "false";
                        drpValue.Items.Add(li);
                        txtValue.Visible = false;
                        drpValue.Visible = true;
                        cldrValue.Visible = false;
                    }
                    else
                    {
                        success = false;
                    }
                }
			}
			return success;
		}

		public override void LoadContent()
		{
			_viewRewardCatalog.LoadRewardList();
		}
	}
}