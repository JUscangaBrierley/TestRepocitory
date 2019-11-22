using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Util;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Interceptors;

namespace Brierley.LWModules.LoyaltyCardsGrid.Components
{	
	public class DefaultGridProvider : AspGridProviderBase
	{
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private ICustomGridAction[] actions = null;
        private string _vcFilter = null;        
        private Member _member = null;
        private LoyaltyCardsGridConfig _config = null;
        private IList<IClientDataObject> refCardTypes = null;
        private IList<VirtualCard> _cards = null;

        #region Private Helpers



        #endregion

        #region Public Properties
        internal Member Member
        {
			get {
				if (_member == null)
				{
					_member = PortalState.CurrentMember;
				}
				return _member; 
			}
        }
        #endregion

        #region Constructor
        public DefaultGridProvider(LoyaltyCardsGridConfig config)
        {
            _config = config;
        }

        #endregion

        #region Grid Properties

        public override string Id
        {
            get { return "grdLoyaltyCards"; }
        }

        protected override string GetGridName()
        {
            return "LoyaltyCards";
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
            IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();

			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "VcKey";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-VcKey.Text", "VcKey");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;			
			c.IsSortable = false;
            colList.Add(c);
			
            var fields = _config.ListFieldsToShow;
			if (fields != null)
			{
				foreach (ConfigurationItem item in fields.Where(o => o.AttributeType != ItemTypes.DynamicListCommandButton))
				{
					string field = item.AttributeName;
					c = new DynamicGridColumnSpec();
					c.Name = field;
					string resourceKey = string.Format("{0}-{1}.Text", Id, field);
					c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, resourceKey, field);
					c.DataType = typeof(string);
					c.IsEditable = false;
					c.IsVisible = true;
					c.IsSortable = false;
					colList.Add(c);
					if (field == "LoyaltyIdNumber")
					{
						c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, "reqLoyaltyId.ErrorMessage", "Please enter a Loyalty Id."), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
					}
					else if (field == "CardType")
					{
						if (!string.IsNullOrEmpty(_config.CardTypeAttributeSetName) &&
					!string.IsNullOrEmpty(_config.CardTypeAttributeName) &&
					!string.IsNullOrEmpty(_config.CardLabelAttributeName))
						{
							c.EditControlType = DynamicGridColumnSpec.DROPDOWNLIST;
							c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, "CardTypeSelectReq.Text", "Please select a Card Type."), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
						}
						else
						{
							c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
							c.Validators.Add(new RequiredFieldValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, "CardTypeEntryReq.Text", "Please enter a Card Type."), CssClass = "Validator", ValidationGroup = this.ValidationGroup });
							c.Validators.Add(new RangeValidator() { ErrorMessage = ResourceUtils.GetLocalWebResource(ParentControl, "InvalidCardType.Text", "Enter a valid Card Type."), CssClass = "Validator", MinimumValue = "0", MaximumValue = int.MaxValue.ToString(), Type = ValidationDataType.Integer, ValidationGroup = this.ValidationGroup });
						}
					}                    
				}
			}
            return colList.ToArray<DynamicGridColumnSpec>();
		}

		public override bool IsGridEditable()
		{
            return _config != null ? _config.AllowUpdates : true;
		}

        public override List<DynamicGridFilter> GetFilters()
        {
            var filters = new List<DynamicGridFilter>();
            filters.Add(new DynamicGridFilter("Status:", FilterDisplayTypes.DropDownList, ResourceUtils.GetLocalWebResource(ParentControl, "DefaultSelect.Text", "-- Select --"), VirtualCardStatusType.Active.ToString(), VirtualCardStatusType.Cancelled.ToString(), VirtualCardStatusType.InActive.ToString(), VirtualCardStatusType.Replaced.ToString()));
            return filters;
        }

        public override void SetFilter(string filterName, string filterValue)
        {
            _vcFilter = filterValue;
        }

        public override bool IsButtonVisible(string commandName)
        {
			if (commandName == "AddNew" || commandName == "EditRow" || commandName == "DeleteRow")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		public override bool IsButtonEnabled(string commandName, object key)
		{
			return LoyaltyCardProviderUtil.IsButtonEnabled(commandName, key, Member, _config.EnableReplaceCard, _config.EnableMakePrimary, _config.EnableTransferCard, _config.EnableCancelCard, _config.EnableRenewCard, _config.EnablePassbookCard, _config.PassbookSendEmail);
		}

        #endregion

        #region Data Source
        public override void SetSearchParm(string parmName, object parmValue)
        {
			if (parmName == "Configuration")
            {
                _config = parmValue as LoyaltyCardsGridConfig;
            }
        }

        public override void LoadGridData()
		{
            _cards = new List<VirtualCard>();
            if (Member != null)
            {
                string[] tokens = _config != null && !string.IsNullOrEmpty(_config.CardTypes) ? _config.CardTypes.Split(';') : new string[] { };
                foreach (VirtualCard card in Member.LoyaltyCards.Where(o => tokens.Contains(o.CardType.ToString())))
                {
                    _cards.Add(card);
                }
            }
		}

		public override int GetNumberOfRows()
		{
            return _cards != null ? _cards.Count : 0;
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
            return LoyaltyCardProviderUtil.GetData(_cards[rowIndex], column.Name, GetCardLabels(), _config.CardTypeAttributeSetName, _config.CardTypeAttributeName, _config.CardLabelAttributeName, string.Empty, string.Empty, string.Empty);
		}

        public override object GetColumnData(object keyVal, DynamicGridColumnSpec column)
        {
            long vcKey = (long)keyVal;
            VirtualCard vc = Member.GetLoyaltyCard(vcKey);            
            return LoyaltyCardProviderUtil.GetData(vc, column.Name, GetCardLabels(), _config.CardTypeAttributeSetName, _config.CardTypeAttributeName, _config.CardLabelAttributeName, string.Empty, string.Empty, string.Empty);
        }
        
        //TODO:  Add Card functionality needs to be fixed
        // It seems like this is not being used.  Instead the addign new functionality is being handled 
        // by void lnkAddNew_Click(object sender, EventArgs e) and void lnkNewOk_Click(object sender, EventArgs e)
        // handlers of ViewLoyaltyCards.ascx.cs.  It has the adverse effect of allowing a card to be updated even 
        // if the grid is marked as un-editable.
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{
		}

        public override object GetDefaultValues(DynamicGridColumnSpec column)
        {
            if (!string.IsNullOrEmpty(_config.CardTypeAttributeSetName) &&
                !string.IsNullOrEmpty(_config.CardTypeAttributeName) &&
                !string.IsNullOrEmpty(_config.CardLabelAttributeName) && column.Name == "CardType")
            {
                GetCardLabels();
                List<ListItem> vals = new List<ListItem>();
                foreach (IClientDataObject refObj in refCardTypes)
                {
                    ListItem i = new ListItem();
                    i.Text = refObj.GetAttributeValue(_config.CardLabelAttributeName).ToString();
                    i.Value = refObj.GetAttributeValue(_config.CardTypeAttributeName).ToString();
                    vals.Add(i);
                }
                return vals;
            }
            else
            {
                return null;
            }
        }

        #endregion


		internal IList<IClientDataObject> GetCardLabels()
		{
			if (refCardTypes == null)
			{
				if (!string.IsNullOrEmpty(_config.CardTypeAttributeSetName))
				{
					refCardTypes = LoyaltyService.GetAttributeSetObjects(null, _config.CardTypeAttributeSetName, null, null, false);
				}
			}
			return refCardTypes;
		}

        #region Command Handling
        public override ICustomGridAction[] GetCustomCommands()
        {
            if (actions == null)
            {
                if (_config == null || _config.AllowUpdates)
                {
                    actions = new ICustomGridAction[6];
					actions[0] = new CardPrimaryCommand(this);
                    actions[1] = new CardCancelCommand(this);
                    actions[2] = new CardReplaceCommand(this);
                    actions[3] = new CardTransferCommand(this);
                    actions[4] = new CardRenewalCommand(this);
                    actions[5] = new CardPassbookCommand(this);
                }                
            }
            return actions;
        }
        #endregion
        
	}	
}
