<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewStoreLocator.ascx.cs" Inherits="Brierley.LWModules.StoreLocator.ViewStoreLocator" %>

<asp:PlaceHolder ID="HackToAvoidControlsCollectionCannotBeModifiedError" runat="server">
<script type="text/javascript" src="<%= GoogleMapsURL %>"></script>
<script>
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

    var map;
    function mapstore(storeName, lattitude, longitude) {
        $('#StoreMapPanel').show();
        $('#lblMapTitle').html(storeName);
        var centerLocation = new google.maps.LatLng(lattitude, longitude);
        var mapOptions = {
            zoom: 13,
            center: centerLocation,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('GoogleMapDiv'), mapOptions);
        var storemarker = new google.maps.Marker({ position: centerLocation, title: storeName, map: map });
    }

    function closeMap() {
        $('#StoreMapPanel').hide();
    }

    $(document).ready(function () {
        $('#<%= tbSearch.ClientID %>').focus(function () {
            var tbSearch = $('#<%= tbSearch.ClientID %>');
            if (tbSearch.val() == '<%= GetResource("lblSearchPrompt.Text") %>') {
                tbSearch.val('');
            }
        });
        $('#<%= tbSearch.ClientID %>').blur(function () {
            var tbSearch = $('#<%= tbSearch.ClientID %>');
            if (tbSearch.val() == '') {
                tbSearch.val('<%= GetResource("lblSearchPrompt.Text") %>');
            }
        });
    });
</script>
</asp:PlaceHolder>

<div id="StoreLocatorContainer">

	<h1 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" meta:resourcekey="lblModuleTitle" />
	</h1>

	<div id="StoreLocatorModule">
		<div id="StoreSearchPanel">
            <h3><asp:Label ID="lblSearch" runat="server" meta:resourcekey="lblSearch" /></h3>
            <asp:TextBox ID="tbSearch" runat="server" Columns="35" meta:resourcekey="tbSearch" />
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="Button" OnClick="btnSearch_Click" meta:resourcekey="btnSearch" />
            <asp:LinkButton ID="btnDetect" runat="server" CssClass="Button" OnClick="btnDetect_Click" meta:resourcekey="btnDetect" OnClientClick="getLocation();return false;" />
            <asp:HiddenField ID="hdnLatitude" runat="server" Value="" />
            <asp:HiddenField ID="hdnLongitude" runat="server" Value="" />
		</div>
        <div id="StoreSearchStatusPanel"></div>
        <div id="StoreSearchResultsPanel">
            <asp:PlaceHolder ID="phResults" runat="server" />
        </div>
        <div id="StoreMapPanel" style="display:none">
            <h3 id="StoreMapTitle">
                <span id="lblMapTitle"></span>
                <a id="btnCloseMap" class="panel_close_btn" onclick="closeMap();">
                    <img id="imgCloseMapPanel" src="~/images/redx.bmp" runat="server" meta:resourcekey="imgCloseMapPanel" />
                </a>
            </h3>
            <div id="GoogleMapDiv"></div>
        </div>
	</div>
</div>
