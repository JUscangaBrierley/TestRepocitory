using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Threading;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;

using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Util;

namespace Brierley.LWModules.CouponsList.Components
{
	public class CouponsListProvider : AspListProviderBase
	{
		
		private const string _className = "CouponsListProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

		private CouponsListConfig _config = null;
		private long _memberId = 0;
		private IEnumerable<MemberCoupon> _coupons = null;
		private List<CouponDef> _definitions = new List<CouponDef>();
		private List<Dictionary<string, object>> _parms = null;
        private string _statusFilter = "Active";
		
		
		public override string Id
		{
			get { return "lstCoupons"; }
		}

		public override bool IsListEditable()
		{
			return _config.AllowEdits;
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
			if (itemsToShow == null || itemsToShow.Count == 0)
			{
				IList<DynamicListItem> colList = new List<DynamicListItem>()
				{
					new DynamicListColumnSpec("ID", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-ID.Text", "Id:"), typeof(long), null, false) { IsKey = true}, 
					new DynamicListColumnSpec("CouponName", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-CouponName.Text", "Coupon Name:"), typeof(string), "Name"), 
					new DynamicListColumnSpec("Logo", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Logo.Text", "Logo:"), typeof(HtmlImage), "Logo") {EditControlType = AspDynamicList.ListItemControlType.ImageUrl}, 
					new DynamicListColumnSpec("CertificateNmbr", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-CertificateNmbr.Text", "Certificate Number:"), typeof(string), "CertificateNumber"), 
					new DynamicListColumnSpec("ShortDescription", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-ShortDescription.Text", "Description:"), typeof(string), "ShortDescription"), 
					new DynamicListColumnSpec("Description", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Description.Text", "Description:"), typeof(string), "Description"), 
					new DynamicListColumnSpec("DateIssued", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-DateIssued.Text", "Date Issued:"), typeof(DateTime), "DateIssued") {EditControlType = AspDynamicList.ListItemControlType.Date}, 
					new DynamicListColumnSpec("DateRedeemed", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-DateRedeemed.Text", "Date Redeemed:"), typeof(DateTime), "DateRedeemed") {EditControlType = AspDynamicList.ListItemControlType.Date}, 
					new DynamicListColumnSpec("ExpiryDate", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-ExpiryDate.Text", "Expiration Date:"), typeof(DateTime), "ExpiryDate") {EditControlType = AspDynamicList.ListItemControlType.Date}, 
					new DynamicListColumnSpec("Status", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Status.Text", "Status:"), typeof(string), "Status"), 
					new DynamicListColumnSpec("UsesLeft", ResourceUtils.GetLocalWebResource(ParentControl, Id + "-UsesLeft.Text", "Uses Left:"), typeof(long), "UsesLeft"), 
					new DynamicListCommandSpec(new ListCommand("Redeem"), ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkRedeem.Text", "Redeem" )) //no tie in to CommandClicked because the main coupon view handles custom actions
				};
				return colList;
			}
			else
			{
				var items = new List<DynamicListItem>()
				{
					new DynamicListColumnSpec("RowKey", "RowKey", typeof(long), null, false, false, false) { IsKey = true }
				};

				foreach (ConfigurationItem item in itemsToShow)
				{
					if (item.AttributeType == ItemTypes.DynamicListCommandButton)
					{
						DynamicListCommandSpec command = new DynamicListCommandSpec(new ListCommand(item.AttributeName), null) { BeginHtml = item.BeginHtml, EndHtml = item.EndHtml };
						if (item.AttributeName == ListCommand.EditItem)
						{
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkEdit.Text", "Edit");
						}
						else if (item.AttributeName == "Redeem")
						{
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkRedeem.Text", "Redeem");
						}
						else if (item.AttributeName == "Print")
						{
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkPrint.Text", "Print");
						}
                        else if (item.AttributeName == "Passbook")
                        {
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-lnkPassbook.Text", "Send To Apple Wallet");
                            command.CommandClicked += command_CommandClicked;
                        }
 
						if (!string.IsNullOrEmpty(item.DisplayText))
						{
							command.Text = item.DisplayText;
						}
						if (!string.IsNullOrEmpty(item.ResourceKey))
						{
							command.Text = ResourceUtils.GetLocalWebResource(ParentControl, item.ResourceKey, command.Text);
						}
						if (!string.IsNullOrEmpty(item.ControlCSSClass))
						{
							command.CssClass = item.ControlCSSClass;
						}

						if (command.CommandName == ListCommand.Share.CommandName)
						{
							string socialShareUrl = string.Empty;
							if (_config.UseExternalSharedUrl)
							{
								socialShareUrl = StringUtils.FriendlyString(_config.ExternalSharedUrl);
							}
							else
							{
								if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.Url != null)
								{
									socialShareUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).ToString();
								}
								socialShareUrl += "/SocialShare.aspx";
							}
							SocialShareConfig socialShareConfig = new SocialShareConfig()
							{
								IsFacebookEnabled = _config.IsFacebookEnabled,
								Facebook = new FacebookShareParams()
								{
									AppID = _config.FacebookAppID,
									SharedUrl = socialShareUrl,
									Layout = FacebookShareLayoutEnum.Button
								},
								IsTwitterEnabled = _config.IsTwitterEnabled,
								Twitter = new TwitterShareParams()
								{
									SharedUrl = socialShareUrl,
									TextToTweet = _config.TwitterTweetText,
									HashTag = _config.TwitterHashTag,
									ViaTwitterUser = string.Empty,
									RecommendTwitterUser = string.Empty,
									UseLargeButton = false,
									ShowShareCount = false,
									DoNotTailor = true
								},
								IsGooglePlusEnabled = _config.IsGooglePlusEnabled,
								GooglePlus = new GooglePlusShareParams()
								{
									SharedUrl = socialShareUrl
								}
							};
							command.SocialShareConfig = socialShareConfig;
						}

						items.Add(command);
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
						c.FormatString = item.Format;

						if (item.AttributeName == "DateRedeemed" ||
							item.AttributeName == "ExpiryDate" ||
							item.AttributeName == "TimesUsed")
						{
							c.IsEditable = true;
						}
						else
						{
							c.IsEditable = false;
						}
						if (item.AttributeName == "Logo" || item.AttributeName == "BarCode")
						{
							c.DataType = typeof(HtmlImage);
						}
						items.Add(c);
					}
				}
				return items;
			}
		}

		public override bool IsButtonVisible(ListCommand commandName)
		{
			if (commandName == ListCommand.Create ||
				commandName == ListCommand.Delete)
			{
				return false;
			}
			else
			{
				return true;
			};
		}

		public override bool IsButtonEnabled(ListCommand command, object key)
		{
			if (command == ListCommand.Delete ||
				command == ListCommand.Create)
			{
				return false;
			}
			else if (command == ListCommand.View)
			{
				return _config.ShowDetailsScreen;
			}
			else if (command == ListCommand.Print)
			{
				return true;
			}
            else if (command == ListCommand.EditItem)
            {
                long id = long.Parse(key.ToString());
                MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
                return mc.IsActive() ? true : false;
            }
            else if (command == "Redeem")
            {
                long id = long.Parse(key.ToString());
                MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
                return mc.IsActive();
            }
            else
            {
                return true;
            }
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
			if (parmName == "Config")
			{
                _config = (CouponsListConfig)parmValue;
			}
			if (parmName == "MemberIpCode")
			{
				_memberId = (long)parmValue;
			}
		}

		public override void SetSearchParm(List<Dictionary<string, object>> parms)
		{
			_parms = parms;
		}

        public override List<WebFrameWork.Controls.Grid.DynamicGridFilter> GetFilters()
        {
            var filters = new List<DynamicGridFilter>();
            filters.Add(new DynamicGridFilter(ResourceUtils.GetLocalWebResource(ParentControl, "CouponStatusLabel.Text", "Coupon Status:"),
                FilterDisplayTypes.DropDownList,
                ResourceUtils.GetLocalWebResource(ParentControl, "DefaultSelect.Text", "-- Select --"),
                ResourceUtils.GetLocalWebResource(ParentControl, "ActiveFilter.Text", "Active"),
                ResourceUtils.GetLocalWebResource(ParentControl, "ExpiredFilter.Text", "Expired")));            
            return filters;
        }

        public override void SetFilter(string filterName, string filterValue)
        {
            _statusFilter = filterValue;
        }

        public override void LoadListData()
        {
            if (_parms == null)
            {
                _coupons = LoyaltyService.GetMemberCouponsByMember(_memberId, null, false, false, null, null);
            }
            else
            {
                DateTime? to = null;
                DateTime? from = null;
                foreach (var dict in _parms)
                {
                    if (dict.ContainsKey("FromDate"))
                    {
                        from = DateTime.Parse(dict["FromDate"].ToString());
                    }
                    if (dict.ContainsKey("ToDate"))
                    {
                        to = DateTime.Parse(dict["ToDate"].ToString());
                    }
                }
                _coupons = LoyaltyService.GetMemberCouponsByMember(_memberId, null, false, false, from, to);
            }
            if (!string.IsNullOrEmpty(_statusFilter))
            {
                if (_statusFilter == ResourceUtils.GetLocalWebResource(ParentControl, "ActiveFilter.Text", "Active"))
                {
                    _coupons = _coupons.Where(o => o.StartDate <= DateTime.Now && o.ExpiryDate.GetValueOrDefault(DateTime.MaxValue) >= DateTime.Now).ToList();
                }
                else if (_statusFilter == ResourceUtils.GetLocalWebResource(ParentControl, "ExpiredFilter.Text", "Expired"))
                {
                    _coupons = _coupons.Where(o => o.ExpiryDate.GetValueOrDefault(DateTime.MaxValue) < DateTime.Now).ToList();
                }
            }
        }

		public override int GetNumberOfRows()
		{
			return _coupons != null ? _coupons.Count() : 0;
		}

		public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
		{
			MemberCoupon mc = _coupons.ElementAt<MemberCoupon>(rowIndex);
			return GetColumnData(mc, column, LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel());
		}

        public override object GetColumnData(object keyVal, DynamicListColumnSpec column)
        {
            long id = long.Parse(keyVal.ToString());
            MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
            return GetColumnData(mc, column, LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel());
        }

		public override string GetAppPanelTotalText(int totalRecords)
		{
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoCouponsMessage.Text", "No Coupons");
            }
            else
            {
                return string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "Total.Text", "Total") + " {0} ", totalRecords);
            }
        }

		public override bool Validate(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
		{
			string methodName = "Validate";
			bool valid = true;

			if (listAction != AspDynamicList.ListActions.Update)
			{
				string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "MemberCouponNotCreatedMessage.Text", "Member coupon cannot be created."));
				_logger.Error(_className, methodName, errMsg);
				throw new LWValidationException(errMsg);
			}

			MemberCoupon coupon = null;
			long couponId = 0;
			
			if (columns.First().Data != null)
			{
				couponId = long.Parse(columns.First().Data.ToString());
			}
			if (couponId != 0)
			{
                coupon = LoyaltyService.GetMemberCoupon(couponId);
			}

			CouponDef cdef = GetCoupon(coupon.CouponDefId);

			DateTime? redeemDate = null;
			DateTime? expiryDate = coupon.ExpiryDate;
			long timesUsed = coupon.TimesUsed;

			DynamicListColumnSpec redeemColumn = null;
			DynamicListColumnSpec expiryColumn = null;
			DynamicListColumnSpec timesColumn = null;

			foreach (DynamicListColumnSpec column in columns)
			{
				//string columnName = columnMap[column.Name];
				string columnName = column.Name;
				if (columnName == "DateRedeemed")
				{
					redeemColumn = column;
					if (column.Data != null)
					{
						if (column.Data.GetType() == typeof(string))
						{
							if (!string.IsNullOrEmpty((string)column.Data))
							{
								redeemDate = DateTime.Parse((string)column.Data);
							}
						}
						if (column.Data.GetType() == typeof(DateTime))
						{
							redeemDate = (DateTime)column.Data;
						}
						redeemDate = DateTimeUtil.GetBeginningOfDay((DateTime)redeemDate);
					}
					else
					{
						redeemDate = DateTime.Today;
					}
				}
				else if (columnName == "ExpiryDate")
				{
					expiryColumn = column;
					if (column.Data != null)
					{
						if (column.Data.GetType() == typeof(string))
						{
							if (!string.IsNullOrEmpty((string)column.Data))
							{
								expiryDate = DateTime.Parse((string)column.Data);
							}
						}
						if (column.Data.GetType() == typeof(DateTime))
						{
							expiryDate = (DateTime)column.Data;
						}
						if (expiryDate.HasValue)
						{
							expiryDate = DateTimeUtil.GetEndOfDay(expiryDate.Value);
						}
					}
				}
				else if (columnName == "TimesUsed" && column.Data != null)
				{
					timesColumn = column;
					try
					{
						timesUsed = long.Parse(column.Data.ToString());
					}
					catch (FormatException)
					{
						string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "TimesUsedNotNumeric.Text", "Please provide a valid numeric value for TimesUsed."));
						_logger.Error(_className, methodName, errMsg);
						OnValidationError(errMsg, column);
						valid = false;
					}
				}
			}

			if (redeemDate != null && expiryDate != null &&
						DateTimeUtil.LessThan(expiryDate.Value, (DateTime)redeemDate))
			{
				string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "InvalidRedeemDate.Text", "Redeem date cannot be greater than expiriation date."));
				_logger.Error(_className, methodName, errMsg);
				OnValidationError(errMsg, redeemColumn);
				valid = false;
			}

			if (timesUsed < 0)
			{
				string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "InvalidTimesUsed.Text", "The TimesUsed cannot be < 0."));
				_logger.Error(_className, methodName, errMsg);
				OnValidationError(errMsg, timesColumn);
				valid = false;
			}

			if (timesUsed > cdef.UsesAllowed)
			{
				string errMsg = string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "TimesUsedLeft.Text", "Only {0} uses are left."), cdef.UsesAllowed - coupon.TimesUsed);
				_logger.Error(_className, methodName, errMsg);
				OnValidationError(errMsg, timesColumn);
				valid = false;
			}

			return valid;
		}

        public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
        {
            string errMsg = string.Empty;

            if (listAction != AspDynamicList.ListActions.Update)
            {
                throw new LWException("Member coupon cannot be created.");
            }

            MemberCoupon coupon = null;
            long couponId = 0;
            if (columns.First().Data != null)
            {
                couponId = long.Parse(columns.First().Data.ToString());
            }
            if (couponId != 0)
            {
                coupon = LoyaltyService.GetMemberCoupon(couponId);
            }

            foreach (DynamicListColumnSpec column in columns)
            {
                if (column.Name == "CertificateNmbr")
                {
                    coupon.CertificateNmbr = (string)column.Data;
                }
                else if (column.Name == "DateRedeemed")
                {
                    if (column.Data != null)
                    {
                        if (column.Data.GetType() == typeof(string))
                        {
                            if (!string.IsNullOrEmpty((string)column.Data))
                            {
                                coupon.DateRedeemed = DateTime.Parse((string)column.Data);
                            }
                        }
                        if (column.Data.GetType() == typeof(DateTime))
                        {
                            coupon.DateRedeemed = (DateTime)column.Data;
                        }
                        coupon.DateRedeemed = DateTimeUtil.GetBeginningOfDay((DateTime)coupon.DateRedeemed);
                    }
                    else
                    {
                        coupon.DateRedeemed = DateTime.Today;
                    }
                }
                else if (column.Name == "ExpiryDate")
                {
                    if (column.Data != null)
                    {
                        if (column.Data.GetType() == typeof(string))
                        {
                            if (!string.IsNullOrEmpty((string)column.Data))
                            {
                                coupon.ExpiryDate = DateTime.Parse((string)column.Data);
                            }
                        }
                        if (column.Data.GetType() == typeof(DateTime))
                        {
                            coupon.ExpiryDate = (DateTime)column.Data;
                        }
                        if (coupon.ExpiryDate.HasValue)
                        {
                            coupon.ExpiryDate = DateTimeUtil.GetEndOfDay(coupon.ExpiryDate.Value);
                        }
                    }
                }
                else if (column.Name == "TimesUsed" && column.Data != null)
                {
                    try
                    {
                        coupon.TimesUsed = long.Parse(column.Data.ToString());
                    }
                    catch (Exception)
                    {
                        throw new LWException(ResourceUtils.GetLocalWebResource(ParentControl, "TimesUsedNotNumeric.Text", "Please provide a valid numeric value for TimesUsed."));
                    }
                }
            }

            LoyaltyService.UpdateMemberCoupon(coupon);
        }


		private object GetColumnData(MemberCoupon mc, DynamicListColumnSpec column, string language, string channel)
		{
			object value = null;
			CouponDef def = GetCoupon(mc.CouponDefId);

			if (column.Name == "ID" || column.Name == "RowKey")
			{
				value = mc.ID;
				return value;
			}

			string columnName = column.Name;
			foreach (var i in _config.ViewFieldsToShow)
			{
				if (i.DataKey == column.Name)
				{
					columnName = i.AttributeName;
					break;
				}
			}

			if (columnName == "CouponName")
			{
				value = def.Name;
			}
			else if (columnName == "TypeCode")
			{
				value = def.CouponTypeCode;
			}
            else if (columnName == "BarCode")
            {
                if (!string.IsNullOrEmpty(_config.BarcodeAssemblyName) && !string.IsNullOrEmpty(_config.BarcodeFactoryName) && !string.IsNullOrWhiteSpace(mc.CertificateNmbr))
                {
                    var link = new BarcodeLink(_config.BarcodeAssemblyName, _config.BarcodeFactoryName, _config.BarcodeSymbology, mc.CertificateNmbr);
                    string imageName = string.Format("~/Barcode.aspx?barcode={0}", link.ToString());
                    value = imageName;
                }
            }
			else if (columnName == "Logo")
			{
				value = PortalState.GetImageUrl(string.Empty) + def.LogoFileName;
			}
			else if (columnName == "CertificateNmbr")
			{
				value = mc.CertificateNmbr;
			}
			else if (columnName == "ShortDescription")
			{
				value = def.GetShortDescription(language, channel);
                value = def.EvaluateBScript(PortalState.CurrentMember, value as string);
			}
			else if (columnName == "Description")
			{
				value = def.GetDescription(language, channel);
                value = def.EvaluateBScript(PortalState.CurrentMember, value as string);
			}
			else if (columnName == "DateIssued")
			{
				value = mc.DateIssued;
			}
			else if (columnName == "DateRedeemed")
			{
				value = mc.DateRedeemed;
			}
			else if (columnName == "ExpiryDate")
			{
				value = mc.ExpiryDate;
			}
			else if (columnName == "Status")
			{
				value = mc.Status.GetValueOrDefault().ToString();
			}
			else if (columnName == "UsesAllowed")
			{
				value = def.UsesAllowed;
			}
			else if (columnName == "TimesUsed")
			{
				value = mc.TimesUsed;
			}
			else if (columnName == "UsesLeft")
			{
				value = def.UsesAllowed - mc.TimesUsed;
			}
			return value;
		}

		private CouponDef GetCoupon(long id)
		{
			var ret = _definitions.FirstOrDefault(o => o.Id == id);
			if(ret == null)
			{
				ret = ContentService.GetCouponDef(id);
				if(ret != null)
				{
					_definitions.Add(ret);
				}
			}
			return ret;
		}

		public override string GetSocialSharedUrlArgument(object key)
		{
			string result = string.Empty;
			if (key != null)
			{
				string args = string.Format("command=coupon&couponID={0}", key.ToString());

				string encryptionKey = CryptoUtil.EncodeUTF8("sOc14l5hAr3");
				result = "?id=" + CryptoUtil.Encrypt(encryptionKey, args);
			}
			return result;
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
            _hdnId.Value = keyVal.ToString();
            long id = Convert.ToInt64(keyVal);
            MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
            if (mc != null)
            {
                CouponDef def = GetCoupon(mc.CouponDefId);
                _lbl1.Text = string.Format("Coupon to Send: {0}", def.Name);
                _lbl2.Text = string.Format("Email Address: {0}", PortalState.CurrentMember.PrimaryEmailAddress);
            }
        }

        void lnkSend_Click(object sender, EventArgs e)
        {
            string methodName = "lnkSend_Click";

            LoadListData();
            long id = Convert.ToInt64(_hdnId.Value);

			#region Send to Apple Wallet
			var member = PortalState.CurrentMember;
            MemberCoupon mc = LoyaltyService.GetMemberCoupon(id);
            CouponDef def = ContentService.GetCouponDef(mc.CouponDefId);
            _logger.Debug(_className, methodName, string.Format("Sending coupon {0} to Passbook.", def.Name));

            MTouch mt = LoyaltyService.CreateMTouch(MTouchType.Coupon, member.IpCode.ToString(), def.Id.ToString());
            // now send triggered email with the mtouch in the link
            if (!string.IsNullOrWhiteSpace(_config.PassbookSendEmail))
            {
                EmailDocument emailDoc = EmailService.GetEmail(_config.PassbookSendEmail);
                using (ITriggeredEmail email = TriggeredEmailFactory.Create(emailDoc.Id))
                {
                    Dictionary<string, string> fields = new Dictionary<string, string>();
                    fields.Add("memberid", member.IpCode.ToString());
                    fields.Add("mtouch", mt.MTouchValue);
                    fields.Add("iso639Code", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
                    email.SendAsync(member, fields).Wait();
                    _logger.Debug(_className, methodName, string.Format("Email {0} ({1}) sent", _config.PassbookSendEmail, emailDoc.Id));
                }
            }
            #endregion

            OnCustomPanelFinished();
        } 
        #endregion

	}
}