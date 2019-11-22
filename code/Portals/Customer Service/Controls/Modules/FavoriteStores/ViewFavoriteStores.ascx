<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewFavoriteStores.ascx.cs" Inherits="Brierley.LWModules.FavoriteStores.ViewFavoriteStores" %>
<%@ Register TagPrefix="cc1" Namespace="Brierley.WebFrameWork.Controls" Assembly="Brierley.WebFrameWork" %>

<div id="FavoriteStoresContainer">
	<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>
	<div id="FavoriteStoresHeaderContent">
        <asp:PlaceHolder ID="phHeaderContent" runat="server" />
	</div>
	<div class="FavoriteStoresTopButtons">
        <a href="<%=LocationsUrl %>" class="btn"><asp:Literal runat="server" ID="litAddStores" meta:resourcekey="btnAddNewFavoriteStore"></asp:Literal></a>
    </div>

	
	<cc1:LWApplicationPanel runat="server" ID="pnlMain" Visible="true">
		<Content>
			<div class="DynamicList">
				<asp:ListView runat="server" ID="lstStores">
					<LayoutTemplate>
						<h4><asp:Label runat="server" ID="lblResults" meta:resourcekey="lblResults"></asp:Label></h4>
						<div class="row">
							<asp:PlaceHolder runat="server" ID="ItemPlaceholder"></asp:PlaceHolder>
						</div>
					</LayoutTemplate>
					<ItemTemplate>
						<div class="col-md-6 col-sm-12">
							<div class="store">
								<div class="store-buttons">
									<asp:LinkButton runat="server" ID="lnkDelete" CommandName="Delete" CommandArgument='<%#Eval("StoreId") %>' meta:ResourceKey="lnkDelete" cssclass="btn btn-sm"></asp:LinkButton>
									<asp:PlaceHolder runat="server" ID="pchMapIt">
										<a class="btn btn-sm map-button" data-toggle="modal" data-target="#mapModal" onclick="mapstore('<%#Eval("StoreName").ToString().Replace("'", "\'")%>', '<%#Eval("Latitude")%>', '<%#Eval("Longitude")%>'); return false;">Map It</a>
									</asp:PlaceHolder>
								</div>
								<h3 class="name"><%#Eval("StoreName")%></h3>
								<p>
									<span class="address1"><%#Eval("AddressLineOne")%></span>
									<span class="address2"><%#Eval("AddressLineTwo")%></span>
									<span class="citystatezip"><%#Eval("City")%>, <%#Eval("StateOrProvince")%> <%#Eval("ZipOrPostalCode")%></span>
								</p>
								<div class="clearfix"></div>
							</div>
						</div>
					</ItemTemplate>
				</asp:ListView>
			</div>
		</Content>
	</cc1:LWApplicationPanel>

		
	
	<div id="mapModal" class="modal fade">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 id="lblMapTitle" class="modal-title" meta:ResourceKey="ExampleReceipt"></h4>
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


<asp:PlaceHolder ID="HackToAvoidControlsCollectionCannotBeModifiedError" runat="server">
<script type="text/javascript" src="//maps.googleapis.com/maps/api/js"></script>
<script>
	var _map;
	var _center;

	$(document).ready(function () {
		$("#mapModal").on("shown.bs.modal", function () {
			google.maps.event.trigger(_map, "resize");
			_map.setCenter(_center);
		});
	});

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
