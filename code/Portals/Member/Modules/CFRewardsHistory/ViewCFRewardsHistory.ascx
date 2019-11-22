<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCFRewardsHistory.ascx.cs" Inherits="Brierley.LWModules.CFRewardsHistory.ViewCFRewardsHistory" %>

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

<div id="CFRewardsHistoryContainer">

	<%--<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>--%>

	<div id="CFRewardsHistory">
		<asp:PlaceHolder ID="phRewardsList" runat="server" />        
	</div>
</div>
