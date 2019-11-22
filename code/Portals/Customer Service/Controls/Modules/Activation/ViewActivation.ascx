<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewActivation.ascx.cs" Inherits="Brierley.LWModules.Activation.ViewActivation" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls" TagPrefix="WebFramework" %>

<div id="ActivationContainer">

	<asp:PlaceHolder runat="server" ID="pchError" Visible="false">
	<div class="Negative">
		<asp:Literal runat="server" ID="litError"></asp:Literal>
	</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchUsername">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litUsername" meta:resourcekey="Username"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtUsername" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqUsername" Display="Dynamic" ControlToValidate="txtUsername" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqUsername" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchPrimaryEmail">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litPrimaryEmail" meta:resourcekey="PrimaryEmail"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtPrimaryEmail" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="req" Display="Dynamic" ControlToValidate="txtPrimaryEmail" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqPrimaryEmail" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchPrimaryPhone">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litPrimaryPhone" meta:resourcekey="PrimaryPhone"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtPrimaryPhone" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqPrimaryPhone" Display="Dynamic" ControlToValidate="txtPrimaryPhone" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqPrimaryPhone" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchHomePhone">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litHomePhone" meta:resourcekey="HomePhone"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtHomePhone" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqHomePhone" Display="Dynamic" ControlToValidate="txtHomePhone" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqHomePhone" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchMobilePhone">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litMobilePhone" meta:resourcekey="MobilePhone"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtMobilePhone" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqMobilePhone" Display="Dynamic" ControlToValidate="txtMobilePhone" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqMobilePhone" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchWorkPhone">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litWorkPhone" meta:resourcekey="WorkPhone"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtWorkPhone" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqWorkPhone" Display="Dynamic" ControlToValidate="txtWorkPhone" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqWorkPhone" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchLoyaltyId">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litLoyaltyId" meta:resourcekey="LoyaltyId"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtLoyaltyId" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqLoyaltyId" Display="Dynamic" ControlToValidate="txtLoyaltyId" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqLoyaltyId" />
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchAlternateId">
		<div class="form-group">
			<label>
				<asp:Literal runat="server" ID="litAlternateId" meta:resourcekey="AlternateId"></asp:Literal>
			</label>
			<asp:TextBox runat="server" ID="txtAlternateId" CssClass="form-control" MaxLength="255"></asp:TextBox>
			<asp:RequiredFieldValidator runat="server" ID="reqAlternateId" Display="Dynamic" ControlToValidate="txtAlternateId" 
			EnableClientScript="true" CssClass="Validator" meta:resourcekey="reqAlternateId" />
		</div>
	</asp:PlaceHolder>


	<div class="buttons">
		<WebFramework:LWLinkButton ButtonType="submit" runat="server" id="lnkSubmit" meta:resourcekey="SubmitButton" cssclass="btn"></WebFramework:LWLinkButton>
	</div>
</div>