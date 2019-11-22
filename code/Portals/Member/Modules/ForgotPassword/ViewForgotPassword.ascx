<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewForgotPassword.ascx.cs" Inherits="Brierley.LWModules.ForgotPassword.ViewForgotPassword" %>
<div id="ForgotPasswordContainer">
	<asp:MultiView ID="mvForgotPassword" runat="server">
		<asp:View ID="vwNone" runat="server"></asp:View>
		<asp:View ID="vwMessage" runat="server">
			<asp:Label ID="lblMessage" runat="server" />
		</asp:View>
		<asp:View ID="vwFP" runat="server">
			<table>
				<tr>
					<td><asp:Label ID="lblFPIdentityLabel" runat="server" /></td>
					<td>
						<asp:TextBox ID="tbFPIdentity" runat="server" />
						<asp:RequiredFieldValidator ID="rfFPIdentity" runat="server" ControlToValidate="tbFPIdentity" Display="Dynamic" CssClass="Validator" />
					</td>
				</tr>
				<tr><td colspan="2"><asp:PlaceHolder ID="phFPButton" runat="server" /></td></tr>
				<tr><td colspan="2"><asp:Label ID="lblFPErrorMessage" runat="server" CssClass="Validator"/></td></tr>
			</table>
		</asp:View>
		<asp:View ID="vwCP" runat="server">
			<table>
				<tr>
					<td><asp:Label ID="lblCPPassword" runat="server" /></td>
					<td><asp:TextBox ID="tbCPPassword" runat="server" TextMode="Password" /></td>
					<td>
						<asp:RequiredFieldValidator ID="rfCPPassword" runat="server" ControlToValidate="tbCPPassword" Display="Dynamic" CssClass="Validator" />
					</td>
				</tr>
				<tr>
					<td><asp:Label ID="lblCPConfirm" runat="server" /></td>
					<td><asp:TextBox ID="tbCPConfirm" runat="server" TextMode="Password" /></td>
					<td>
						<asp:RequiredFieldValidator ID="rfCPConfirm" runat="server" ControlToValidate="tbCPConfirm" Display="Dynamic" CssClass="Validator" />
						<asp:CompareValidator ID="cmpCPConfirm" runat="server" ControlToValidate="tbCPConfirm" ControlToCompare="tbCPPassword" Type="String" Operator="Equal" Display="Dynamic" CssClass="Validator" />
					</td>
				</tr>
				<tr><td colspan="2"><asp:PlaceHolder ID="phCPButton" runat="server" /></td></tr>
				<tr><td colspan="2"><asp:Label ID="lblCPErrorMessage" runat="server" CssClass="Validator" /></td></tr>
			</table>
		</asp:View>
		<asp:View ID="vwPC" runat="server">
			<table>
				<tr><td><asp:Label ID="lblPCMessage" runat="server" /></td></tr>
				<tr><td><asp:PlaceHolder ID="phPCButton" runat="server" /></td></tr>
			</table>
		</asp:View>
	</asp:MultiView>
</div>