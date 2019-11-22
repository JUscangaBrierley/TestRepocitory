<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCFRewardsHistory.ascx.cs" Inherits="Brierley.LWModules.CFRewardsHistory.ViewCFRewardsHistory" %>

<asp:PlaceHolder ID="HackToAvoidControlsCollectionCannotBeModifiedError" runat="server">
<script>
    $(document).ready(function () {
		// TODO: javascript initialization
    });
</script>
</asp:PlaceHolder>

<div id="CFRewardsHistoryContainer">
	<div id="CFRewardsHistoryList">
		<asp:PlaceHolder ID="phRewardsList" runat="server" />        
	</div>
</div>
