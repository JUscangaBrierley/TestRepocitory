using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.StoreLocator
{
	public partial class ViewStoreLocator : ModuleControlBase
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

		private enum SearchExpressionType { Undefined, LatLong, ZipOrPostalCode, CityAndStateOrProvince };

		private const string _className = "ViewStoreLocator";
		private const string _modulePath = "~/Controls/Modules/StoreLocator/ViewStoreLocator.ascx";
		private const string _storeSearchCookieName = "StoreSearchExpression";
		private const int _maxStoresToRetrieve = 50;

		private StoreLocatorConfig _config;
		private IStoreLocatorInterceptor _interceptor;
		private List<MemberStore> _favoriteStores;
		private bool _storesHaveLatsAndLongs = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("StoresHaveLatsAndLongs"), true);
		private string _defaultSearchPrompt;

		private IStoreLocatorInterceptor Interceptor
		{
			get
			{
				if (_interceptor == null)
				{
					if (!string.IsNullOrWhiteSpace(_config.InterceptorAssembly) && !string.IsNullOrWhiteSpace(_config.InterceptorClass))
					{
						try
						{
							_interceptor = (IStoreLocatorInterceptor)ClassLoaderUtil.CreateInstance(_config.InterceptorAssembly, _config.InterceptorClass);
						}
						catch (Exception ex)
						{
							_logger.Error(_className, "Interceptor",
								string.Format("Failed to load interceptor '{0}'/'{1}': {2}",
								_config.InterceptorAssembly, _config.InterceptorClass, ex.Message), ex);
						}
					}

					if (_interceptor == null)
					{
						_interceptor = new DefaultStoreLocatorInterceptor();
					}
				}
				return _interceptor;
			}
		}

		protected string GoogleMapsURL
		{
			get
			{
				string googleurl = "http://maps.googleapis.com/maps/api/js?sensor=false";
				bool forceSsl = StringUtils.FriendlyBool(ConfigurationManager.AppSettings["ForceSSL"], false);
				if (forceSsl)
				{
					googleurl = googleurl.Replace("http://", "https://");
				}
				return googleurl;
			}
		}

		protected bool ResultsTwoColumn
		{
			get
			{
				return _config != null ? _config.ResultsTwoColumn : true;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			try
			{
				btnSearch.Click += btnSearch_Click;
				btnDetect.Click += btnDetect_Click;
				lstStores.ItemDataBound += lstStores_ItemDataBound;
				lstStores.ItemCommand += lstStores_ItemCommand;
				InitializeConfig();

				// hitting enter in the search textbox will cause search button click
				Page.Form.DefaultButton = btnSearch.UniqueID;

				btnDetect.Visible = _storesHaveLatsAndLongs;

				LoadFavoriteStores();
				DoSearchFromCookie();
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "OnLoad", "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		void lstStores_ItemCommand(object sender, ListViewCommandEventArgs e)
		{
			const string methodName = "lstStores_ItemCommand";
			try
			{
				long storeId = long.Parse(e.CommandArgument.ToString());
				if (storeId < 0)
				{
					return;
				}
				if (PortalState.CurrentMember == null)
				{
					_logger.Error(_className, methodName, "Store favorited while PortalState.CurrentMember is null.");
					return;
				}
				List<MemberStore> memberStores = LoyaltyService.GetMemberStoresByMember(PortalState.CurrentMember.IpCode);
				var existing = from x in memberStores where x.StoreDefId == storeId select x;
				if (existing != null && existing.Count() > 0)
				{
					_logger.Warning(_className, methodName, "Existing favorite store was favorited.");
					return;
				}

				_logger.Trace(_className, methodName,
					string.Format("Member '{0}' favorited store '{1}'",
						PortalState.CurrentMember.IpCode, storeId
					)
				);

				long maxPref = memberStores.Count > 0 ? (from x in memberStores select x.PreferenceOrder).Max() : 0;
				DateTime now = DateTime.Now;
				MemberStore newMemberStore = new MemberStore()
				{
					MemberId = PortalState.CurrentMember.IpCode,
					StoreDefId = storeId,
					PreferenceOrder = maxPref + 1,
					CreateDate = now,
					UpdateDate = now
				};
				LoyaltyService.SaveMemberStore(newMemberStore);

				// reset the search results so the favorite indicator will be shown
				// instead of the add favorite store button
				_favoriteStores = null;
				LoadFavoriteStores();
				DoSearchFromCookie();
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		void lstStores_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType != ListViewItemType.DataItem)
			{
				return;
			}
			var current = e.Item.DataItem as NearbyStoreItem;
			bool isFavorite = false;
			bool canfavorite = false;
			if (PortalState.CurrentMember != null)
			{
				isFavorite = (from x in _favoriteStores where x.StoreDefId == current.Store.StoreId select x).FirstOrDefault<MemberStore>() != null;
				canfavorite = !isFavorite;
			}
			e.Item.FindControl("pchFavorite").Visible = isFavorite;
			e.Item.FindControl("lnkFavorite").Visible = canfavorite;
			e.Item.FindControl("pchMapIt").Visible = current.Store.Latitude.HasValue && current.Store.Longitude.HasValue;
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(tbSearch.Text) && (tbSearch.Text != _defaultSearchPrompt))
				{
					SetStoreSearchCookie(tbSearch.Text, null, null);
					DoSearch(tbSearch.Text, null, null);
				}
				else
				{
					tbSearch.Text = _defaultSearchPrompt;
					base.AddInvalidField(GetResource("NoCriteriaMessage.Text"), tbSearch);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "btnSearch_Click", "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected void btnDetect_Click(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(hdnLatitude.Value) && !string.IsNullOrWhiteSpace(hdnLongitude.Value))
				{
					double latitude = double.Parse(hdnLatitude.Value);
					double longitude = double.Parse(hdnLongitude.Value);

					LocationItem nearestLocation = _storesHaveLatsAndLongs ? Interceptor.NearestLocationItem(latitude, longitude) : null;
					if (nearestLocation != null)
					{
						tbSearch.Text = string.Format("{0}, {1}", nearestLocation.City, nearestLocation.StateOrProvince);
					}
					else
					{
						tbSearch.Text = string.Format("{0}, {1}", hdnLatitude.Value, hdnLongitude.Value);
					}
					SetStoreSearchCookie(tbSearch.Text, latitude, longitude);
					DoSearch(tbSearch.Text, latitude, longitude);
				}
				else
				{
					// client side javascript should not allow this case to get to the server
					ShowNegative(GetResource("lblLocErrorPositionNotAvailable.Text"));
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "btnDetect_Click", "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected void btnAddFavoriteStore_Click(object sender, EventArgs e)
		{
			const string methodName = "btnAddFavoriteStore_Click";
			try
			{
				if (PortalState.CurrentMember == null)
				{
					_logger.Error(_className, methodName, "Store favorited while PortalState.CurrentMember is null.  How can that happen?");
					return;
				}
				LinkButton btnAddFavoriteStore = sender as LinkButton;
				if (btnAddFavoriteStore == null)
				{
					_logger.Error(_className, methodName, string.Format("Sender is not a LinkButton, it is a '{0}'.  How can that happen?", sender.GetType().Name));
					return;
				}
				long storeID = StringUtils.FriendlyInt64(btnAddFavoriteStore.Attributes["StoreID"], -1);
				if (storeID < 0)
				{
					_logger.Error(_className, methodName, string.Format("StoreID attribute is invalid: {0}.  How can that happen?", storeID));
					return;
				}
				List<MemberStore> memberStores = LoyaltyService.GetMemberStoresByMember(PortalState.CurrentMember.IpCode);
				var existing = from x in memberStores where x.StoreDefId == storeID select x;
				if (existing != null && existing.Count() > 0)
				{
					_logger.Warning(_className, methodName, "Existing favorite store was favorited.  How can that happen?");
					return;
				}

				_logger.Trace(_className, methodName,
					string.Format("Member '{0}' favorited store '{1}'",
						PortalState.CurrentMember.IpCode, storeID
					)
				);

				long maxPref = memberStores.Count > 0 ? (from x in memberStores select x.PreferenceOrder).Max() : 0;
				DateTime now = DateTime.Now;
				MemberStore newMemberStore = new MemberStore()
				{
					MemberId = PortalState.CurrentMember.IpCode,
					StoreDefId = storeID,
					PreferenceOrder = maxPref + 1,
					CreateDate = now,
					UpdateDate = now
				};
				LoyaltyService.SaveMemberStore(newMemberStore);

				// reset the search results so the favorite indicator will be shown
				// instead of the add favorite store button
				_favoriteStores = null;
				LoadFavoriteStores();
				DoSearchFromCookie();
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		private void LoadFavoriteStores()
		{
			if (PortalState.CurrentMember != null && _favoriteStores == null)
			{
				_favoriteStores = LoyaltyService.GetMemberStoresByMember(PortalState.CurrentMember.IpCode);
			}
		}

		private void DoSearchFromCookie()
		{
			// search expression is either from cookie, or member property, or else a default prompt is used
			_defaultSearchPrompt = GetResource("lblSearchPrompt.Text");
			string searchExpression = _defaultSearchPrompt;
			double? latitude = null;
			double? longitude = null;
			if (!GetStoreSearchCookie(ref searchExpression, ref latitude, ref longitude) && PortalState.CurrentMember != null)
			{
				GetMemberAttributeSearchExpression(ref searchExpression, ref latitude, ref longitude);
			}

			if (!Page.IsPostBack)
			{
				tbSearch.Text = searchExpression;
			}

			if (tbSearch.Text != _defaultSearchPrompt)
			{
				DoSearch(searchExpression, latitude, longitude);
			}
		}

		private void DoSearch(string searchExpression, double? latitude, double? longitude)
		{
			bool hasMatches = false;
			string item1 = null;
			string item2 = null;
			switch (DetermineSearchExpressionType(searchExpression, latitude, longitude, ref item1, ref item2))
			{
				case SearchExpressionType.LatLong:
					if (!latitude.HasValue)
						latitude = Double.Parse(item1);
					if (!longitude.HasValue)
						longitude = Double.Parse(item2);
					hasMatches = ShowResultsByLatLong(latitude.Value, longitude.Value);
					break;

				case SearchExpressionType.CityAndStateOrProvince:
					LocationItem nearestCity = _storesHaveLatsAndLongs ? Interceptor.NearestLocationItem(item1, item2) : null;
					if (nearestCity != null)
					{
						hasMatches = ShowResultsByLatLong(nearestCity.Latitude, nearestCity.Longitude);
					}
					else
					{
						hasMatches = ShowResultsByCityAndStateOrProvince(item1, item2);
					}
					break;

				case SearchExpressionType.ZipOrPostalCode:
					LocationItem nearestZip = _storesHaveLatsAndLongs ? Interceptor.NearestLocationItem(searchExpression) : null;
					if (nearestZip != null)
					{
						hasMatches = ShowResultsByLatLong(nearestZip.Latitude, nearestZip.Longitude);
					}
					else
					{
						hasMatches = ShowResultsByZipOrPostalCode(searchExpression);
					}
					break;

				case SearchExpressionType.Undefined:
					// drop out and get the no stores found message
					break;
			}

			if (!hasMatches)
			{
				phResults.Controls.Add(new LiteralControl(GetResource("EmptyMessage.Text")));
			}
		}

		private bool ShowResultsByLatLong(double latitude, double longitude)
		{
			NearbyStoreCollection nearbyStores = GetNearbyStores(latitude, longitude);
			if (nearbyStores != null && nearbyStores.Count > 0)
			{
				lstStores.DataSource = nearbyStores.Take(_maxStoresToRetrieve);
				lstStores.DataBind();

				/*
				for (int i = 0; i < nearbyStores.Count && i < _config.MaxStoresInSearchResults; i++)
				{
					var nearbyStore = nearbyStores[i];

					if (_config.ResultsTwoColumn)
					{
						if (i % 2 == 0)
						{
							phResults.Controls.Add(new LiteralControl(@"<div class=""row"">"));
						}
						phResults.Controls.Add(new LiteralControl(@"<div class=""col-lg-6 col-sm-12"">"));
					}
					CreateTableCell(nearbyStore.Store, nearbyStore.DistanceInKM);
					if (_config.ResultsTwoColumn)
					{
						if (i % 2 != 0 || i == _config.MaxStoresInSearchResults - 1 || i == nearbyStores.Count - 1)
						{
							phResults.Controls.Add(new LiteralControl(@"</div>"));
						}
					}
				}
				*/
				return true;
			}
			return false;
		}

		private bool ShowResultsByCityAndStateOrProvince(string city, string stateOrProvince)
		{
			IList<StoreDef> stores = ContentService.GetStoreDefsByCityAndStateOrProvince(city, stateOrProvince, _config.MaxStoresInSearchResults);

			bool hasMatches = false;
			bool needClose = false;
			int numStores = 0;
			var sortedStores = from x in stores orderby x.StoreName select x;

			lstStores.DataSource = GetNearbyStoreCollection(sortedStores);
			lstStores.DataBind();

			/*
			foreach (var store in sortedStores)
			{
				if (_config.ResultsTwoColumn)
				{
					if ((numStores % 2) == 0)
					{
						phResults.Controls.Add(new LiteralControl(@"<tr class=""storerow""><td class=""storecolumnleft"">"));
						needClose = true;
					}
					else
					{
						phResults.Controls.Add(new LiteralControl(@"<td class=""storecolumnright"">"));
					}
					CreateTableCell(store, null);
					if ((numStores % 2) == 1)
					{
						phResults.Controls.Add(new LiteralControl("</tr>"));
						needClose = false;
					}
				}
				else
				{
					phResults.Controls.Add(new LiteralControl(@"<tr class=""storerow""><td class=""storecolumnleft"">"));
					CreateTableCell(store, null);
					phResults.Controls.Add(new LiteralControl("</td></tr>"));
					needClose = false;
				}
				hasMatches = true;
				if (++numStores > _config.MaxStoresInSearchResults) break;
			}
			if (needClose)
			{
				phResults.Controls.Add(new LiteralControl("</tr>"));
			}
			 * */
			return hasMatches;
		}

		private bool ShowResultsByZipOrPostalCode(string zipOrPostalCode)
		{
			IList<StoreDef> stores = ContentService.GetStoreDefsByZipOrPostalCode(zipOrPostalCode, _config.MaxStoresInSearchResults);

			bool hasMatches = false;
			bool needClose = false;
			int numStores = 0;
			var sortedStores = from x in stores orderby x.StoreName select x;

			lstStores.DataSource = GetNearbyStoreCollection(sortedStores);
			lstStores.DataBind();

			/*
			foreach (var store in sortedStores)
			{
				if (_config.ResultsTwoColumn)
				{
					if ((numStores % 2) == 0)
					{
						phResults.Controls.Add(new LiteralControl(@"<tr class=""storerow""><td class=""storecolumnleft"">"));
						needClose = true;
					}
					else
					{
						phResults.Controls.Add(new LiteralControl(@"<td class=""storecolumnright"">"));
					}
					CreateTableCell(store, null);
					if ((numStores % 2) == 1)
					{
						phResults.Controls.Add(new LiteralControl("</tr>"));
						needClose = false;
					}
				}
				else
				{
					phResults.Controls.Add(new LiteralControl(@"<tr class=""storerow""><td class=""storecolumnleft"">"));
					CreateTableCell(store, null);
					phResults.Controls.Add(new LiteralControl("</td></tr>"));
					needClose = false;
				}
				hasMatches = true;
				if (++numStores > _config.MaxStoresInSearchResults) break;
			}
			if (needClose)
			{
				phResults.Controls.Add(new LiteralControl("</tr>"));
			}*/
			return hasMatches;
		}

		private SearchExpressionType DetermineSearchExpressionType(string searchExpression, double? latitude, double? longitude, ref string item1, ref string item2)
		{
			SearchExpressionType result = SearchExpressionType.Undefined;
			item1 = item2 = null;
			if (_storesHaveLatsAndLongs && latitude.HasValue && longitude.HasValue)
			{
				result = SearchExpressionType.LatLong;
			}
			else if (!string.IsNullOrWhiteSpace(searchExpression))
			{
				if (searchExpression.Contains(","))
				{
					string[] tokens = searchExpression.Split(',');
					if (tokens != null && tokens.Length > 1)
					{
						item1 = tokens[0].Trim();
						item2 = tokens[1].Trim();
						double tmp = 0;
						if (Double.TryParse(item1, out tmp) && Double.TryParse(item2, out tmp))
						{
							if (_storesHaveLatsAndLongs)
							{
								result = SearchExpressionType.LatLong;
							}
						}
						else
						{
							result = SearchExpressionType.CityAndStateOrProvince;
						}
					}
				}
				else
				{
					result = SearchExpressionType.ZipOrPostalCode;
				}
			}
			return result;
		}

		private void GetMemberAttributeSearchExpression(ref string searchExpression, ref double? latitude, ref double? longitude)
		{
			if (PortalState.CurrentMember == null) return;

			if (_config.EnableMemberAttributeLookup)
			{
				object obj1 = null;
				object obj2 = null;
				switch (_config.MemberAttributeLookupType)
				{
					case StoreLocatorConfig.MemberAttributeLookupTypeEnum.None:
						break;

					case StoreLocatorConfig.MemberAttributeLookupTypeEnum.CityAndStateOrProvince:
						obj1 = GetAttrValue(PortalState.CurrentMember, _config.MemberItem1AttrSetName, _config.MemberItem1AttrName);
						obj2 = GetAttrValue(PortalState.CurrentMember, _config.MemberItem2AttrSetName, _config.MemberItem2AttrName);
						if (obj1 != null && obj2 != null)
						{
							searchExpression = string.Format("{0}, {1}", obj1.ToString(), obj2.ToString());
						}
						break;

					case StoreLocatorConfig.MemberAttributeLookupTypeEnum.ZipOrPostalCode:
						obj1 = GetAttrValue(PortalState.CurrentMember, _config.MemberItem1AttrSetName, _config.MemberItem1AttrName);
						if (obj1 != null)
						{
							searchExpression = obj1.ToString();
						}
						break;

					case StoreLocatorConfig.MemberAttributeLookupTypeEnum.LatLong:
						obj1 = GetAttrValue(PortalState.CurrentMember, _config.MemberItem1AttrSetName, _config.MemberItem1AttrName);
						obj2 = GetAttrValue(PortalState.CurrentMember, _config.MemberItem2AttrSetName, _config.MemberItem2AttrName);
						if (obj1 != null && obj2 != null)
						{
							searchExpression = string.Format("{0}, {1}", obj1.ToString(), obj2.ToString());
							latitude = Double.Parse(obj1.ToString());
							longitude = Double.Parse(obj2.ToString());
						}
						break;
				}
			}
		}

		private object GetAttrValue(Member member, string attrSetName, string attrName)
		{
			object result = null;
			if (!member.IsLoaded(attrSetName))
			{
				LoyaltyService.LoadAttributeSetList(member, attrSetName, false);
			}
			IList<IClientDataObject> atsList = member.GetChildAttributeSets(attrSetName);
			if (atsList != null && atsList.Count > 0)
			{
				result = atsList[0].GetAttributeValue(attrName);
			}
			return result;
		}

		private bool GetStoreSearchCookie(ref string searchExpression, ref double? latitude, ref double? longitude)
		{
			bool result = false;
			HttpCookie storeSearchCookie = HttpContext.Current.Request.Cookies[_storeSearchCookieName];
			if (storeSearchCookie != null)
			{
				searchExpression = StringUtils.FriendlyString(storeSearchCookie["SearchExpression"]);
				bool hasPosition = StringUtils.FriendlyBool(storeSearchCookie["HasPosition"], false);
				if (hasPosition)
				{
					double tmp;
					if (Double.TryParse(storeSearchCookie["Latitude"], out tmp))
					{
						latitude = tmp;
					}
					if (Double.TryParse(storeSearchCookie["Longitude"], out tmp))
					{
						longitude = tmp;
					}
				}
				else
				{
					latitude = longitude = null;
				}

				if (!string.IsNullOrWhiteSpace(searchExpression) || (latitude != null && longitude != null))
				{
					result = true;
				}
			}
			return result;
		}

		private void SetStoreSearchCookie(string lastSearchExpression, double? latitude, double? longitude)
		{
			if (lastSearchExpression != _defaultSearchPrompt)
			{
				HttpCookie storeSearchCookie = HttpContext.Current.Request.Cookies[_storeSearchCookieName];
				if (storeSearchCookie == null)
				{
					storeSearchCookie = new HttpCookie(_storeSearchCookieName);
				}
				storeSearchCookie["SearchExpression"] = lastSearchExpression;
				storeSearchCookie["HasPosition"] = (latitude.HasValue && longitude.HasValue).ToString();
				storeSearchCookie["Latitude"] = (latitude.HasValue ? latitude.Value : 0).ToString();
				storeSearchCookie["Longitude"] = (longitude.HasValue ? longitude.Value : 0).ToString();
				HttpContext.Current.Response.Cookies.Add(storeSearchCookie);
			}
		}

		private void CreateTableCell(StoreDef store, double? distanceInKM)
		{
			bool favorite = false;
			if (PortalState.CurrentMember != null)
			{
				favorite = (from x in _favoriteStores where x.StoreDefId == store.StoreId select x).FirstOrDefault<MemberStore>() != null;
			}

			StringBuilder markup = new StringBuilder();

			markup.Append(@"<div class=""store"">");

			if (!string.IsNullOrEmpty(store.StoreName))
			{
				markup.Append(@"<span class=""name"">").Append(store.StoreName).Append(@"</span>");
			}
			markup.Append(@"<p class=""address"">");
			if (!string.IsNullOrEmpty(store.AddressLineOne))
			{
				markup.Append(@"<span class=""address1"">").Append(store.AddressLineOne).Append(@"</span>");
			}
			if (!string.IsNullOrEmpty(store.AddressLineTwo))
			{
				markup.Append(@"<span class=""address2"">").Append(store.AddressLineTwo).Append(@"</span>");
			}
			if (!string.IsNullOrEmpty(store.City) || !string.IsNullOrEmpty(store.StateOrProvince) || !string.IsNullOrEmpty(store.ZipOrPostalCode))
			{
				markup.Append(@"<span class=""citystatezip"">");
				if (!string.IsNullOrEmpty(store.City))
				{
					markup.Append(store.City).Append(", ");
				}
				if (!string.IsNullOrEmpty(store.City))
				{
					markup.Append(store.StateOrProvince).Append(" ");
				}
				if (!string.IsNullOrEmpty(store.City))
				{
					markup.Append(store.ZipOrPostalCode);
				}
				markup.Append(@"</span>");
			}
			markup.Append(@"</p>");
			if (distanceInKM.HasValue)
			{
				markup.Append(@"<span class=""distance"">").Append(GetStoreDistance(distanceInKM.Value)).Append(@"</span>");
			}

			if (favorite)
			{
				markup.Append(@"<i class=""favorite""></i>");
			}

			if (store.Latitude.HasValue && store.Longitude.HasValue)
			{
				markup.Append(
					string.Format(@"<a class=""btn btn-sm map-button"" onclick=""mapstore('{0}', '{1}', '{2}'); return false;"">{3}</a>",
						store.StoreName,
						store.Latitude,
						store.Longitude,
						GetResource("btnMapIt.Text")));
			}

			if (PortalState.CurrentMember != null)
			{
				if (favorite)
				{
					markup.Append(@"<div class=""clearfix""></div></div><!--store--></div><!--column-->");
					phResults.Controls.Add(new LiteralControl(markup.ToString()));
				}
				else
				{
					phResults.Controls.Add(new LiteralControl(markup.ToString()));
					LinkButton btnAddFavoriteStore = new LinkButton()
					{
						ID = "btnAddFavoriteStore_" + store.StoreId.ToString(),
						Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnAddFavoriteStore.Text"),
						CssClass = "btn btn-sm favorite-button",
						EnableViewState = false
					};
					btnAddFavoriteStore.Attributes.Add("StoreID", store.StoreId.ToString());
					btnAddFavoriteStore.Click += btnAddFavoriteStore_Click;
					phResults.Controls.Add(btnAddFavoriteStore);
					phResults.Controls.Add(new LiteralControl(@"<div class=""clearfix""></div></div><!--store--></div><!--column-->"));
				}
			}
			else
			{
				markup.Append(@"<div class=""clearfix""></div></div><!--store--></div><!--column-->");
				phResults.Controls.Add(new LiteralControl(markup.ToString()));
			}
		}

		protected string GetResource(string key)
		{
			if (!key.EndsWith(".Text")) key += ".Text";
			return ResourceUtils.GetLocalWebResource(_modulePath, key);
		}

		private NearbyStoreCollection GetNearbyStores(double latitude, double longitude)
		{
			NearbyStoreCollection nearbyStores = ContentService.GetStoreDefsNearby(latitude, longitude, _config.MapRadiusInMiles, _config.MaxStoresInSearchResults);
			return nearbyStores;
		}

		private NearbyStoreCollection GetNearbyStoreCollection(IEnumerable<StoreDef> stores)
		{
			var ret = new NearbyStoreCollection();
			if (stores != null)
			{
				foreach (var store in stores)
				{
					ret.Add(new NearbyStoreItem() { Store = store });
				}
			}
			return ret;
		}

		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<StoreLocatorConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new StoreLocatorConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}

		protected string GetStoreDistance(double valueInKM)
		{
			string result = string.Empty;
			if (RegionInfo.CurrentRegion.IsMetric)
			{
				result = string.Format("{0:0.##} {1}", valueInKM, GetResource("lblKilometersAbbrev.Text"));
			}
			else
			{
				result = string.Format("{0:0.##} {1}", GeoLocationUtils.Kilometers2StatuteMiles(valueInKM), GetResource("lblMilesAbbrev.Text"));
			}
			return result;
		}
	}
}
