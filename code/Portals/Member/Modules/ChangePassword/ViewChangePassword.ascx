<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewChangePassword.ascx.cs" Inherits="Brierley.LWModules.ChangePassword.ViewChangePassword" %>
<div id="ChangePasswordContainer">
	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle" />
	</h2>

	<asp:PlaceHolder runat="server" ID="phSuccessMessage" Visible="false">
		<div class="Positive">
			<asp:Literal runat="server" ID="litSuccessMessage" />
		</div>
	</asp:PlaceHolder>

	<table>
		<tr>
			<td><asp:Label ID="lblOldPassword" runat="server" meta:resourcekey="lblOldPassword" /></td>
			<td>
				<asp:TextBox ID="tbOldPassword" runat="server" TextMode="Password" Columns="30" MaxLength="255" />
				<asp:RequiredFieldValidator ID="rfOldPassword" runat="server" Display="Dynamic" ControlToValidate="tbOldPassword" 
					EnableClientScript="true" CssClass="Validator" meta:resourcekey="rfOldPassword" />
			</td>
		</tr>
		<tr>
			<td><asp:Label ID="lblNewPassword" runat="server" meta:resourcekey="lblNewPassword" /></td>
			<td>
				<asp:TextBox ID="tbNewPassword" runat="server" TextMode="Password" Columns="30" MaxLength="255" />
				<asp:RequiredFieldValidator ID="rfNewPassword" runat="server" Display="Dynamic" ControlToValidate="tbNewPassword" 
				EnableClientScript="true" CssClass="Validator" meta:resourcekey="rfNewPassword" />
			</td>
		</tr>
		<tr>
			<td><asp:Label ID="lblConfirmPassword" runat="server" meta:resourcekey="lblConfirmPassword" /></td>
			<td>
				<asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" Columns="30" MaxLength="255" />
				<asp:RequiredFieldValidator ID="rfConfirmPassword" runat="server" Display="Dynamic" ControlToValidate="tbConfirmPassword" 
					EnableClientScript="true" CssClass="Validator" meta:resourcekey="rfConfirmPassword" />
				<asp:CompareValidator ID="cmpConfirmPassword" runat="server" Display="Dynamic" ControlToValidate="tbConfirmPassword" 
					ControlToCompare="tbNewPassword" EnableClientScript="true" CssClass="Validator" Operator="Equal" meta:resourcekey="cmpConfirmPassword" />
			</td>
		</tr>
	</table>

	<div class="ActionButtons">
		<asp:LinkButton ID="btnSave" runat="server" meta:resourckey="btnSave" CssClass="default-button" Text="Save" />
		<asp:LinkButton ID="btnCancel" runat="server" meta:resourckey="btnCancel" CausesValidation="false" CssClass="default-button" Text="Cancel" />
	</div>

</div>