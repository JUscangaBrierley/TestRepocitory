<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewFavoriteStores.ascx.cs" Inherits="Brierley.LWModules.FavoriteStores.ViewFavoriteStores" %>

<asp:PlaceHolder ID="HackToAvoidControlsCollectionCannotBeModifiedError" runat="server">
<script type="text/javascript" src="<%= GoogleMapsURL %>"></script>
<script>
    var map;
    function mapfavstore(storeName, lattitude, longitude) {
        $('#FavoriteStoreMapPanel').show();
        $('#lblMapTitle').html(storeName);
        var centerLocation = new google.maps.LatLng(lattitude, longitude);
        var mapOptions = {
            zoom: 13,
            center: centerLocation,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('FavoriteStoreGoogleMapDiv'), mapOptions);
        var storemarker = new google.maps.Marker({ position: centerLocation, title: storeName, map: map });
    }

    function closeMap() {
        $('#FavoriteStoreMapPanel').hide();
    }
</script>
</asp:PlaceHolder>

<div id="FavoriteStoresContainer">
	<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>

    <div id="FavoriteStoresHeaderContent">
        <asp:PlaceHolder ID="phHeaderContent" runat="server" />
	</div>

    <div class="FavoriteStoresTopButtons">
        <asp:LinkButton ID="btnAddNewFavoriteStore" runat="server" CssClass="Button" meta:resourcekey="btnAddNewFavoriteStore" />
    </div>

    <div id="FavoriteStores">
		<asp:PlaceHolder ID="phFavoriteStores" runat="server" />
	</div>

    <div id="FavoriteStoreMapPanel" style="display:none; clear:both;">
        <h3 id="FavoriteStoreMapTitle">
            <span id="lblMapTitle"></span>
                <a id="btnCloseMap" class="panel_close_btn" onclick="closeMap();">
                    <img id="imgCloseMapPanel" src="~/images/redx.bmp" runat="server" meta:resourcekey="imgCloseMapPanel" />
                </a>
            </h3>
        <div id="FavoriteStoreGoogleMapDiv"></div>
    </div>
</div>
