<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewEmailFeedbackHistory.ascx.cs" Inherits="Brierley.LWModules.EmailFeedbackHistory.ViewEmailFeedbackHistory" %>

<div id="EmailFeedbackHistoryContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle" meta:resourcekey="Title"></asp:Literal>
	</h2>

    <asp:PlaceHolder runat="server" ID="pchEmailSuppressed">
        <h2 id="SuppressedEmailMessage">
            <asp:Literal runat="server" id="litEmailSuppressed"></asp:Literal>
        </h2>
    </asp:PlaceHolder>

	<asp:PlaceHolder ID="pchFeedback" runat="server" Visible="true" />

    <div class="buttons">
        <asp:LinkButton runat="server" ID="lnkClearAll" CssClass="button" meta:resourcekey="lnkClearAll"></asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkTestEmail" CssClass="button" meta:resourcekey="lnkSendTest"></asp:LinkButton>
    </div>

</div>