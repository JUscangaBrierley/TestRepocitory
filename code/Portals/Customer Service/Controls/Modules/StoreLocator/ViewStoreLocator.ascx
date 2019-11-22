<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewStoreLocator.ascx.cs" Inherits="Brierley.LWModules.StoreLocator.ViewStoreLocator" %>
<%@ Register TagPrefix="cc1" Namespace="Brierley.WebFrameWork.Controls" Assembly="Brierley.WebFrameWork" %>

<div id="StoreLocatorContainer">
	<h1 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" meta:resourcekey="lblModuleTitle" />
	</h1>
	<h3><asp:Label ID="lblSearch" runat="server" meta:resourcekey="lblSearch" /></h3>
	<div class="form-inline">
		<div class="form-group">
			<asp:TextBox ID="tbSearch" runat="server" CssClass="form-control" meta:resourcekey="tbSearch" />
		</div>
		<asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-sm" meta:resourcekey="btnSearch" />
        <asp:LinkButton ID="btnDetect" runat="server" CssClass="btn btn-sm" meta:resourcekey="btnDetect" OnClientClick="getLocation();return false;" />
	</div>
    <div id="StoreSearchStatusPanel"></div>
    <div id="StoreSearchResultsPanel">
        <asp:PlaceHolder ID="phResults" runat="server" />
    </div>
    	
	<asp:ListView runat="server" ID="lstStores">
		<LayoutTemplate>
			

			<div class="section table_section">
					<div class="title_wrapper">
						<h2><asp:literal runat="server" ID="litTitle" meta:resourcekey="lblResults"></asp:literal></span>
						<span id="ctl15_ctl06_ctl00_leftWrapper" class="title_wrapper_left"></span>
						<span id="ctl15_ctl06_ctl00_rightWrapper" class="title_wrapper_right"></span>
					</div>

					<div id="ctl15_ctl06_ctl00" class="section_content">
						<div class="sct">
							<div class="sct_left">
								<div class="sct_right">
									<div class="sct_leftInner">
										<div class="sct_rightInner">
											<div class="DynamicList">
												<div class="row">
													<asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<span class="scb"><span class="scb_left"></span><span class="scb_right"></span></span>
				</div>

		</LayoutTemplate>
		<ItemTemplate>
			<div class="col-md-6 col-sm-12">
				<div class="store">
					<div class="store-buttons">
						<asp:PlaceHolder runat="server" ID="pchFavorite">
							<i class="favorite"></i>
						</asp:PlaceHolder>
						<asp:LinkButton runat="server" ID="lnkFavorite" CommandName="Favorite" CommandArgument='<%#Eval("Store.StoreId") %>' meta:ResourceKey="btnAddFavoriteStore" cssclass="btn btn-sm favorite-button"></asp:LinkButton>
						<asp:PlaceHolder runat="server" ID="pchMapIt">
							<a class="btn btn-sm map-button" data-toggle="modal" data-target="#mapModal" onclick="mapstore('<%#Eval("Store.StoreName").ToString().Replace("'", "\'")%>', '<%#Eval("Store.Latitude")%>', '<%#Eval("Store.Longitude")%>'); return false;">Map It</a>
						</asp:PlaceHolder>
					</div>
					<h3 class="name"><%#Eval("Store.StoreName")%></h3>
					<p>
						<span class="address1"><%#Eval("Store.AddressLineOne")%></span>
						<span class="address2"><%#Eval("Store.AddressLineTwo")%></span>
						<span class="citystatezip"><%#Eval("Store.City")%>, <%#Eval("Store.StateOrProvince")%> <%#Eval("Store.ZipOrPostalCode")%></span>
					</p>
					<p class="distance"><%#GetStoreDistance((double)Eval("DistanceInMiles")) %></p>
					<div class="clearfix"></div>
				</div>
			</div>
		</ItemTemplate>
	</asp:ListView>

	<div id="mapModal" class="modal fade">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 id="lblMapTitle" class="modal-title">Example Receipt</h4>
				</div>
				<div class="modal-body">
					<div id="GoogleMapDiv"></div>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				</div>
			</div>
		</div>
	</div>

</div>

<asp:HiddenField ID="hdnLatitude" runat="server" Value="" />
<asp:HiddenField ID="hdnLongitude" runat="server" Value="" />


<asp:PlaceHolder ID="HackToAvoidControlsCollectionCannotBeModifiedError" runat="server">
<script type="text/javascript" src="<%= GoogleMapsURL %>"></script>
<script>
	var _map;
	var _center;

	$(document).ready(function () {
		$("#mapModal").on("shown.bs.modal", function () {
			google.maps.event.trigger(_map, "resize");
			_map.setCenter(_center);
		});
		$('#<%= tbSearch.ClientID %>').attr('placeholder', '<%= GetResource("lblSearchPrompt.Text") %>');
	});

	function getLocation() {
		if (navigator.geolocation) {
			navigator.geolocation.getCurrentPosition(showPosition, showError, { maximumAge: 60000, timeout: 10000, enableHighAccuracy: true });
			$('#StoreSearchStatusPanel').html('<%= GetResource("lblDeterminingLocation.Text") %>');
		}
		else {
			$('#StoreSearchStatusPanel').html('<%= GetResource("lblGeolocationNotSupported.Text") %>');
        }
	}

	function showPosition(position) {
		$('#StoreSearchStatusPanel').html('<%= GetResource("lblSendingLocationToServer.Text") %>');
    	$('#<%= hdnLatitude.ClientID %>').val(position.coords.latitude);
    	$('#<%= hdnLongitude.ClientID %>').val(position.coords.longitude);
    	__doPostBack('<%= btnDetect.UniqueID %>', '');
    }

    function showError(error) {
    	var reason = '<%= GetResource("lblLocErrorDefault.Text") %>';
    	if (error) {
    		switch (error.code) {
    			case 1:
    				reason = '<%= GetResource("lblLocErrorPermissionDenied.Text") %>';
                	break;
				case 2:
					reason = '<%= GetResource("lblLocErrorPositionNotAvailable.Text") %>';
                	break;
				case 3:
					reason = '<%= GetResource("lblLocErrorTimeout.Text") %>';
                	break;
			}
		}
		$('#StoreSearchStatusPanel').innerHTML(reason);
		ShowNegative(reason);
	}

	function mapstore(storeName, lattitude, longitude) {
		$('#lblMapTitle').html(storeName);
		var centerLocation = new google.maps.LatLng(lattitude, longitude);
		var mapOptions = {
			zoom: 13,
			center: centerLocation,
			mapTypeId: google.maps.MapTypeId.ROADMAP
		};
		_map = new google.maps.Map(document.getElementById('GoogleMapDiv'), mapOptions);
		var storemarker = new google.maps.Marker({ position: centerLocation, title: storeName, map: _map });
		_center = _map.getCenter();
	}
</script>
</asp:PlaceHolder>