<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewVisitMap.ascx.cs" Inherits="Brierley.LWModules.VisitMap.ViewVisitMap" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls.Mobile" TagPrefix="Mobile" %>
<div id="VisitMapContainer">
	<asp:placeholder runat="server" id="pchFilter">
	<div class="form-inline map-filter">
		<label><asp:literal runat="server" id="lblMapFilter"></asp:literal></label>
		<span>
			<Mobile:CheckBox runat="server" id="chkVisited" autopostback="true" cssclass="checkbox"></Mobile:CheckBox>
			<Mobile:CheckBox runat="server" id="chkNotVisited" autopostback="true" cssclass="checkbox"></Mobile:CheckBox>
			<Mobile:CheckBox runat="server" id="chkQualifyingSpend" autopostback="true" cssclass="checkbox"></Mobile:CheckBox>
			<Mobile:CheckBox runat="server" id="chkAll" autopostback="true" cssclass="checkbox"></Mobile:CheckBox>
		</span>
	</div>
	</asp:placeholder>

	
	<asp:Placeholder runat="server" ID="pnlDateRange" Visible="false">
		<div class="form-inline map-filter">
			<label><asp:Literal ID="lblDateRange" runat="server" /></label>
			<span>
				<asp:placeholder runat="server" id="pchDateList"></asp:placeholder>
			</span>
		</div>
	</asp:Placeholder>		
	
	<div class="visit-map" id="<%=MapDivId %>"></div>
	
	<asp:placeholder runat="server" id="pchShare"></asp:placeholder>

	<asp:PlaceHolder ID="phVisitList" runat="server" />

	<script type="text/javascript" src="<%=GoogleUrl %>"></script>
	<script type="text/javascript" src="Scripts/visit_map.js"></script>
	<script type="text/javascript">	

		$(document).ready(function(){
			InitializeVisitMap(_visitMapConfig);
		});

		var _mapCenterLat = <%=MapCenterLat%>;
		var _mapCenterLong = <%=MapCenterLong%>;
		var _mapZoom = <%=MapZoom%>;
		var _mapType = google.maps.MapTypeId.<%=MapType%>;
		var _markerUrl = '<%=MarkerUrl%>';
		var _mapDivId = '<%=MapDivId%>';

		var _visitMapConfig = {
			centerLocation: new google.maps.LatLng(_mapCenterLat, _mapCenterLong),
			mapZoom: _mapZoom,
			mapDivID: _mapDivId,
			mapTypeId: _mapType,
			markerURL: _markerUrl,
			storeLocations: new Array()
		};

		<%=StoreMarkers%>

		function VisitMap_GetNumVisits() {
			return '<%=NumVisits%>';
		}

		function VisitMap_GetNumOrdinalVisits() {
			return '<%=NumOrdinalVisits%>';
		}

		function VisitMap_GetMapShareURL() {
			return '<%=MapShareUrl%>';
		}

	</script>

</div>
