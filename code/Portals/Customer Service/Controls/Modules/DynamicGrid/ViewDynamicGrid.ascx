<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDynamicGrid.ascx.cs" Inherits="Brierley.LWModules.DynamicGrid.ViewDynamicGrid" %>

<div id="DynamicGridContainer">
	<h2 id="Title">
		<asp:Literal runat="server" ID="h2Title"></asp:Literal>
	</h2>
	<asp:PlaceHolder runat="server" ID="pchGrid"></asp:PlaceHolder>
</div>
