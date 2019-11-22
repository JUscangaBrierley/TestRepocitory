<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewLoyaltyCardsList.ascx.cs" Inherits="Brierley.LWModules.LoyaltyCardsList.ViewLoyaltyCardsList" %>

<asp:PlaceHolder ID="HackToAvoidControlsCollectionCannotBeModifiedError" runat="server">
<script>
    function foobar(arg) {
		alert('foobar: ' + arg);
    }

    $(document).ready(function () {
		// TODO: javascript initialization
    });
</script>
</asp:PlaceHolder>

<div id="LoyaltyCardsListContainer">

	<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>

	<div id="LoyaltyCardsList">
		<asp:PlaceHolder ID="phLoyaltyCards" runat="server" Visible="true" />	
	</div>
</div>
