<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCouponsList.ascx.cs" Inherits="Brierley.LWModules.CouponsList.ViewCouponsList" %>

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

<div id="CouponsListContainer">

	<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>

	<div id="CouponsList">
		<asp:PlaceHolder ID="phCouponsList" runat="server" />
	</div>
</div>
