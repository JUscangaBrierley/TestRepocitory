<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewVisitMap.ascx.cs" Inherits="Brierley.LWModules.VisitMap.ViewVisitMap" %>
<div id="VisitMapContainer">
	<asp:PlaceHolder ID="phVisitMap" runat="server" />
	<div class="DateRangeFilter">
		<asp:Panel runat="server" ID="pnlDateRange" Visible="false">
			<asp:Label ID="lblDateRange" runat="server" />
			<asp:DropDownList runat="server" ID="ddlDateRange" AutoPostBack="true">
			</asp:DropDownList>
		</asp:Panel>
	</div>
	<asp:PlaceHolder ID="phVisitList" runat="server" />
</div>
