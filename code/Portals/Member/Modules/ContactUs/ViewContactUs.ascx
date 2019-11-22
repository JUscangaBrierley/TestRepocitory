<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContactUs.ascx.cs" Inherits="Brierley.LWModules.ContactUs.ViewContactUs" %>

<asp:PlaceHolder runat="server">
<script  language="javascript">
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
		<table>
			<tr>
				<td nowrap><asp:Label ID="lblMemberEmailAddress" runat="server" /></td>
				<td><asp:TextBox ID="tbMemberEmailAddress" runat="server" />
                <asp:RequiredFieldValidator runat="server" ID="reqEmailAddress" Display="Dynamic" ControlToValidate="tbMemberEmailAddress" 
                    ValidationGroup="" meta:resourcekey="ReqEmailAddress" CssClass="Validator"></asp:RequiredFieldValidator>
                </td>
			</tr>
			<tr>
				<td nowrap><asp:Label ID="lblEmailSubject" runat="server" /></td>
				<td><asp:PlaceHolder ID="phEmailSubject" runat="server" /></td>
			</tr>
			<tr>
				<td nowrap><asp:Label ID="lblEmailMessage" runat="server" /></td>
				<td><asp:TextBox ID="tbEmailMessage" runat="server" TextMode="MultiLine" style="resize:none" />
                <asp:RequiredFieldValidator runat="server" ID="reqMessage" Display="Dynamic" ControlToValidate="tbEmailMessage" 
                    ValidationGroup="" meta:resourcekey="ReqMessage" CssClass="Validator" style="float:right;"></asp:RequiredFieldValidator>
                </td>
			</tr>
			<tr id="trCharsRemaining" runat="server">
				<td>&nbsp;</td>
				<td nowrap>
					<asp:Label ID="lblCharsRemaining" runat="server" Text = "Characters remaining: " />
					&nbsp;
					<asp:Label ID="lblCharsRemainingValue" runat="server" />
				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
				<td><asp:CheckBox ID="cbCCMessage" runat="server" /></td>
			</tr>
			<tr>
				<td colspan="2"><asp:LinkButton ID="btnSend" runat="server" CssClass="Button" /></td>
			</tr><tr>
				<td colspan="2"><asp:Label ID="lblStatus" runat="server" /></td>
			</tr>
		</table>
	</div>
</div>