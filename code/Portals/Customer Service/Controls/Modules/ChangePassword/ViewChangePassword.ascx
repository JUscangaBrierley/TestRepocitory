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

	<div class="form-group">
		<label><asp:Literal ID="lblOldPassword" runat="server" meta:resourcekey="lblOldPassword" /></label>
		<asp:TextBox ID="tbOldPassword" runat="server" TextMode="Password" MaxLength="255" CssClass="form-control" />
		<asp:RequiredFieldValidator ID="rfOldPassword" runat="server" Display="Dynamic" ControlToValidate="tbOldPassword" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="rfOldPassword" />
	</div>


	<div class="form-group">
		<label><asp:Literal ID="lblNewPassword" runat="server" meta:resourcekey="lblNewPassword" /></label>
		<asp:TextBox ID="tbNewPassword" runat="server" TextMode="Password" MaxLength="255" CssClass="form-control" />
		<asp:RequiredFieldValidator ID="rfNewPassword" runat="server" Display="Dynamic" ControlToValidate="tbNewPassword" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="rfNewPassword" />
	</div>

	<div class="form-group">
		<label><asp:Literal ID="lblConfirmPassword" runat="server" meta:resourcekey="lblConfirmPassword" /></label>
		<asp:TextBox ID="tbConfirmPassword" runat="server" TextMode="Password" MaxLength="255" CssClass="form-control" />
		<asp:RequiredFieldValidator ID="rfConfirmPassword" runat="server" Display="Dynamic" ControlToValidate="tbConfirmPassword" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="rfConfirmPassword" />
		<asp:CompareValidator ID="cmpConfirmPassword" runat="server" Display="Dynamic" ControlToValidate="tbConfirmPassword" 
			ControlToCompare="tbNewPassword" EnableClientScript="true" CssClass="Validator" Operator="Equal" meta:resourcekey="cmpConfirmPassword" />
	</div>

	<div class="ActionButtons">
		<asp:LinkButton ID="btnSave" runat="server" meta:resourckey="btnSave" CssClass="btn" Text="Save" />
		<asp:LinkButton ID="btnCancel" runat="server" meta:resourckey="btnCancel" CausesValidation="false" CssClass="btn" Text="Cancel" />
	</div>

</div>