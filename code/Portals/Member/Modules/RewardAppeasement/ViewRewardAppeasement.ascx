<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardAppeasement.ascx.cs" Inherits="Brierley.LWModules.RewardAppeasement.ViewRewardAppeasement" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:PlaceHolder runat="server">
<script type="text/javascript">
	function RewardAppeasementCancelClick() {
		$('#<%=ddlReward.ClientID%>').val('');
		$('#<%=txtNotes.ClientID%>').val('');
		$('#<%=reqReward.ClientID%>').css('display', 'none');
		return false;
	}
</script>
</asp:PlaceHolder>

<div id="RewardAppeasementContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="h2Title"></asp:Literal>
	</h2>

	<asp:PlaceHolder runat="server" ID="pchSuccessMessage" Visible="false">
		<div class="Positive">
			<asp:Literal runat="server" ID="litSuccessMessage"></asp:Literal>
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchNoResults" Visible="false">
		<div id="NoResults">
			<asp:Label ID="lblNoResults" runat="server" Visible="false" meta:resourcekey="lblNoResults"></asp:Label>
		</div>
	</asp:PlaceHolder>

	<span id="RewardLabel">
		<asp:Label ID="lblReward" runat="server" meta:resourcekey="lblReward"/>
	</span>
	<span id="RewardSelect">
		<asp:DropDownList ID="ddlReward" runat="server"></asp:DropDownList>
		<asp:RequiredFieldValidator ID="reqReward" runat="server" Display="Dynamic" ControlToValidate="ddlReward" meta:resourcekey="reqReward" CssClass="Validator"></asp:RequiredFieldValidator>
	</span>

	<asp:PlaceHolder runat="server" ID="pchAdditionalNotes">
		<div id="AdditionalNotes">
			<span id="AdditionalNotesLabel">
				<asp:Label runat="server" ID="lblAdditionalNotes" meta:resourcekey="lblAdditionalNotes"></asp:Label>
			</span>

			<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine"></asp:TextBox>
		</div>
	</asp:PlaceHolder>

	<div class="ActionButtons">
		<asp:LinkButton ID="btnSave" runat="server" meta:resourcekey="btnSave" />
		<asp:LinkButton ID="btnCancel" runat="server" meta:resourcekey="btnCancel" CausesValidation="false" OnClientClick="return RewardAppeasementCancelClick();" />
	</div>
</div>
