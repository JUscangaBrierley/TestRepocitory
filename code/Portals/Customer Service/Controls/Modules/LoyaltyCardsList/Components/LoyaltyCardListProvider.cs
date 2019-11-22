using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Threading;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.WebFrameWork.Portal.Util;
using Brierley.FrameWork.Email;


namespace Brierley.LWModules.LoyaltyCardsList.Components
{
	public class LoyaltyCardListProvider : AspListProviderBase
    {
        #region Fields
        private const string _className = "LoyaltyCardListProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

		private Member _member = null;
		private LoyaltyCardsListConfig _config = null;
		private IList<IClientDataObject> refCardTypes = null;
        private IList<VirtualCard> _cards = null;

        private string _vcFilter = null;
        #endregion

        internal Member Member
		{
			get
			{
				if (_member == null)
				{
					_member = PortalState.CurrentMember;
				}
				return _member;
			}
		}

		public override string Id
		{
			get { return "lstLoyaltyCards"; }
		}

		public override bool IsListEditable()
		{
			return false;
		}

		public override IEnumerable<DynamicListItem> GetListItemSpecs()
		{
			if (_config == null)
			{
				return GetItemSpecs(null);
			}            
			return GetItemSpecs(_config.ListFieldsToShow);
		}

		public override IEnumerable<DynamicListItem> GetViewItemSpecs()
		{
			if (_config == null)
			{
				return GetItemSpecs(null);
			}
			return GetItemSpecs(_config.ViewFieldsToShow);
		}

		private IEnumerable<DynamicListItem> GetItemSpecs(List<ConfigurationItem> itemsToShow)
		{
			IList<DynamicListItem> colList = new List<DynamicListItem>()
			{
				new DynamicListColumnSpec("VcKey", null, typeof(long), null, false) { IsKey = true}
			};

			if (itemsToShow == null || itemsToShow.Count == 0)
			{
				return colList;
			}

			foreach (ConfigurationItem item in itemsToShow)
			{
				if (item.AttributeType == ItemTypes.DynamicListCommandButton)
				{
					DynamicListCommandSpec command = new DynamicListCommandSpec(new ListCommand(item.AttributeName), null) { BeginHtml = item.BeginHtml, EndHtml = item.EndHtml };
					switch (item.AttributeName)
					{
						case "View":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-View.Text", "View");
							break;
						case "Cancel":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Cancel.Text", "Cancel");
							break;
						case "Primary":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-MakePrimary.Text", "Make Primary");
							break;
						case "Replace":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Replace.Text", "Replace");
							break;
						case "Transfer":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Transfer.Text", "Transfer");
							break;
						case "Renew":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Renew.Text", "Renew");
							break;
						case "Passbook":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Passbook.Text", "Send To Apple Wallet");
                            command.CommandClicked += command_CommandClicked;
							break;
						case "Print":
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkPrint.Text", "Print");
							break;
                        case "Done":
                            command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Done.Text", "Done");
                            break;
                    }
					if (!string.IsNullOrEmpty(item.DisplayText))
					{
						command.Text = item.DisplayText;
					}
					if (!string.IsNullOrEmpty(item.ResourceKey))
					{
						command.Text = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, command.Text);
					}
					command.CssClass = item.ControlCSSClass;
					colList.Add(command);
				}
				else
				{
					string displayText = item.DisplayText;
					if (!string.IsNullOrEmpty(item.ResourceKey))
					{
						displayText = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, item.DisplayText);
					}

					var c = new DynamicListColumnSpec(item.AttributeName, displayText, string.IsNullOrEmpty(item.LabelCssClass) ? item.ControlCSSClass : item.LabelCssClass);
					c.BeginHtml = item.BeginHtml;
					c.EndHtml = item.EndHtml;
                    if (item.AttributeName == "BarCode")
                    {
                        c.DataType = typeof(HtmlImage);
                    }
					colList.Add(c);
				}
			}
			return colList;
		}
        
		public override bool IsButtonEnabled(ListCommand command, object key)
		{
			bool isEnabled = LoyaltyCardProviderUtil.IsButtonEnabled(command.CommandName, key, Member, true, true, true, true, true, true, _config.PassbookSendEmail);            
            return isEnabled;
		}


		public override string GetEmptyListMessage()
		{
			if (_config != null && !string.IsNullOrEmpty(_config.EmptyResultMessageResourceKey))
			{
				return ResourceUtils.GetLocalWebResource(ParentControl, _config.EmptyResultMessageResourceKey);
			}
			else
			{
				return ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EmptyResultMessage.Text");
			}
		}



		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (parmName == "Configuration")
			{
				_config = parmValue as LoyaltyCardsListConfig;
			}
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

		public override void LoadListData()
		{
            _cards = new List<VirtualCard>();
            if (Member != null)
            {
                string[] tokens = _config != null && !string.IsNullOrEmpty(_config.CardTypes) ? _config.CardTypes.Split(';') : new string[] { };
                foreach (VirtualCard card in Member.LoyaltyCards.Where(o => tokens.Length == 0 || tokens.Contains(o.CardType.ToString())))
                {
                    _cards.Add(card);
                }
            }
		}

		public override int GetNumberOfRows()
		{
			return _cards != null ? _cards.Count : 0;
		}

		public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
		{
            return LoyaltyCardProviderUtil.GetData(_cards[rowIndex], column.Name, GetCardLabels(), _config.CardTypeAttributeSetName, _config.CardTypeAttributeName, _config.CardLabelAttributeName, _config.BarcodeAssemblyName, _config.BarcodeFactoryName, _config.BarcodeSymbology);
		}

		public override object GetColumnData(object keyVal, DynamicListColumnSpec column)
		{
			long vcKey = Convert.ToInt64(keyVal);			
            VirtualCard vc = Member.GetLoyaltyCard(vcKey);            
            return LoyaltyCardProviderUtil.GetData(vc, column.Name, GetCardLabels(), _config.CardTypeAttributeSetName, _config.CardTypeAttributeName, _config.CardLabelAttributeName, _config.BarcodeAssemblyName, _config.BarcodeFactoryName, _config.BarcodeSymbology);
		}



		public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
		{
			
		}

		public override string GetAppPanelTotalText(int totalRecords)
		{
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoCardsMessage.Text", "No Cards.");
            }
            else
            {
                return string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "Total.Text", "Total") + " {0} ", totalRecords);
            }
        }

		internal IList<IClientDataObject> GetCardLabels()
		{
			if (refCardTypes == null)
			{
				refCardTypes = LoyaltyService.GetAttributeSetObjects(null, _config.CardTypeAttributeSetName, null, null, false);
			}
			return refCardTypes;
        }

        #region Custom Commands

        private HiddenField _hdnId = null;
        private Label _lbl1 = null;
        private Label _lbl2 = null;
        public override void CreateCommandControls(ListCommand command, System.Web.UI.Control container)
        {
            if (command == "Passbook")
            {
                var div = new HtmlGenericControl("div");
                div.Attributes.Add("style", "padding: 20px;");

                _lbl1 = new Label();                
                div.Controls.Add(_lbl1);
                div.Controls.Add(new LiteralControl("</br>"));
                _lbl2 = new Label();                
                div.Controls.Add(_lbl2);
                
                div.Controls.Add(new LiteralControl("<div class=\"Buttons\">"));

                var lnkSend = new LinkButton() { CssClass = "btn", Text = "Send" };
                lnkSend.Click += lnkSend_Click;
                div.Controls.Add(lnkSend);

                var lnkCancel = new LinkButton() { CssClass = "btn", Text = "Cancel" };
                lnkCancel.Click += delegate { OnCustomPanelFinished(); };
                div.Controls.Add(lnkCancel);

                div.Controls.Add(new LiteralControl("</div>"));

                _hdnId = new HiddenField();
                div.Controls.Add(_hdnId);

                container.Controls.Add(div);
            }            
        }

        void command_CommandClicked(object sender, ListCommand command, object keyVal)
        {
            //this is difficult to do. Sure, we can pass a web page/control context to the class and allow it to add child controls to it, but without the provider being 
            //able to take part in asp.net's lifecycle, we have no way of reliably re-creating those controls when the user initiates a postback.

            //for example, we have a custom action called "Add a note to this coupon" that lets the user add a note via some attribute set that links to the member coupon id. So when the
            //user clicks "add a note", we need to pass the current page context - better yet, a custom panel within AspDynamicList's multiview - so we can create controls for capturing the
            //note. So that all works fine, we'll add a textbox and submit button, set AspDynamicList's view to custom, and the user sees the form we created...

            //so how do we get that form back, without making it a gigantic hassle?

            //throw new Exception("Sorry, buddy, you can't get to that part of the web from here.");


            //we could, during AspDynamicList's CreateChildControls, for each custom command, call Provider.CreateCustomCommandControls and pass a new custom view for each custom command. That'd 
            //be the best we could come up with. The method would be invoked when the list's controls are created, and a teardown/rebuild would trigger the same here, so we'd have some reliability.
            //This class would wire up save/cancel and any other button click events, and could tell AspDynamicList when it's finished so AspDynamicList can revert back to list mode.
            LoadListData();
            _hdnId.Value = keyVal.ToString();

            long vcKey = Convert.ToInt64(keyVal);
            VirtualCard vc = Member.GetLoyaltyCard(vcKey);
            _lbl1.Text = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "CardSend.Text", "Card to Send:") + " {0}", vc.LoyaltyIdNumber);
            _lbl2.Text = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "EmailAddress.Text", "Email Address:") + " {0}", Member.PrimaryEmailAddress);
        }

        void lnkSend_Click(object sender, EventArgs e)
        {
            string methodName = "lnkSend_Click";

            LoadListData();
            long id = Convert.ToInt64(_hdnId.Value);

			#region Send to Apple Wallet
			VirtualCard vc = Member.GetLoyaltyCard(id);
            _logger.Debug(_className, methodName, string.Format("Sending Loyalty Card {0} to Passbook.", vc.LoyaltyIdNumber));

            MTouch mt = LoyaltyService.CreateMTouch(MTouchType.LoyaltyCard, Member.IpCode.ToString(), vc.LoyaltyIdNumber);

            // now send triggered email with the mtouch in the link
            if (!string.IsNullOrWhiteSpace(_config.PassbookSendEmail))
            {
                EmailDocument emailDoc = EmailService.GetEmail(_config.PassbookSendEmail);
				using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailDoc.Id))
				{
					Dictionary<string, string> fields = new Dictionary<string, string>();
					fields.Add("memberid", Member.IpCode.ToString());
					fields.Add("mtouch", mt.MTouchValue);
					fields.Add("iso639Code", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
					email.SendAsync(Member, fields).Wait();
					_logger.Debug(_className, methodName, string.Format("Email {0} ({1}) sent", _config.PassbookSendEmail, emailDoc.Id));
				}
            }
            
            #endregion

            OnCustomPanelFinished();
        }        
        #endregion
    }
}