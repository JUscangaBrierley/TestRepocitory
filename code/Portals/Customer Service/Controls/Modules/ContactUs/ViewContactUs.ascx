<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContactUs.ascx.cs" Inherits="Brierley.LWModules.ContactUs.ViewContactUs" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls.Mobile" TagPrefix="Mobile" %>

<asp:PlaceHolder runat="server">
	<script language="javascript">
		function TextAreaMaxLength(textArea, maxLength) {
			var nMaxlength = new Number(maxLength);
			if (textArea.value.length > nMaxlength) {
				textArea.value = textArea.value.substring(0, nMaxlength);
			}

			var lblCharsRemainingValue = document.getElementById('<%= lblCharsRemainingValue.ClientID %>');
	if (lblCharsRemainingValue) {
		lblCharsRemainingValue.innerHTML = (nMaxlength - textArea.value.length);
	}
}
	</script>
</asp:PlaceHolder>

<div id="ContactUsContainer">

	<h2 id="Title">
		<asp:Label runat="server" ID="lblModuleTitle" />
	</h2>

	<div id="ContactUs">
		<div>
			<asp:Label ID="lblStatus" runat="server" />
		</div>

		<div class="form-group">
			<label>
				<asp:Label ID="lblMemberEmailAddress" runat="server" />
			</label>
			<asp:TextBox ID="tbMemberEmailAddress" runat="server" CssClass="form-control" />
			<asp:RequiredFieldValidator runat="server" ID="reqEmailAddress" Display="Dynamic" ControlToValidate="tbMemberEmailAddress"
				ValidationGroup="" meta:resourcekey="ReqEmailAddress" CssClass="Validator"></asp:RequiredFieldValidator>
		</div>

		<div class="form-group">
			<asp:PlaceHolder ID="phEmailSubject" runat="server" />
		</div>

		<asp:PlaceHolder ID="phAdditionalSubjectFields" runat="server" />

		<div class="form-group">
			<label><asp:Label ID="lblEmailMessage" runat="server" /></label>
			<asp:TextBox ID="tbEmailMessage" runat="server" TextMode="MultiLine" CssClass="form-control" />
			<asp:RequiredFieldValidator runat="server" ID="reqMessage" Display="Dynamic" ControlToValidate="tbEmailMessage"
				ValidationGroup="" meta:resourcekey="ReqMessage" CssClass="Validator" Style="float: right;"></asp:RequiredFieldValidator>
		</div>
				
		<asp:PlaceHolder runat="server" ID="trCharsRemaining">
			<div class="characters-remaining">
				<asp:Label ID="lblCharsRemaining" runat="server" meta:resourcekey="CharactersRemaining" />&nbsp;<asp:Label ID="lblCharsRemainingValue" runat="server" />
			</div>
		</asp:PlaceHolder>
		
		<div class="form-group">
			<Mobile:CheckBox runat="server" id="cbCCMessage" cssclass="checkbox"></Mobile:CheckBox>
		</div>

		<asp:LinkButton ID="btnSend" runat="server" CssClass="btn" />
	</div>
</div>
