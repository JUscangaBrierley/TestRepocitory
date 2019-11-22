<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewForgotPassword.ascx.cs" Inherits="Brierley.LWModules.ForgotPassword.ViewForgotPassword" %>
<div id="ForgotPasswordContainer">
	<asp:MultiView ID="mvForgotPassword" runat="server">
		<asp:View ID="vwNone" runat="server"></asp:View>

		<asp:View ID="vwMessage" runat="server">
			<asp:Label ID="lblMessage" runat="server" />
		</asp:View>
		
		<asp:View ID="vwForgotPassword" runat="server">
			<div>
				<asp:Label ID="lblFPErrorMessage" runat="server" CssClass="Validator"/>
			</div>
			<div class="form-group">
                <label style="width:100% !important;">
					<asp:Literal ID="lblFPResetProcessExplanation" runat="server" />
				</label>
            </div>
            <div class="form-group">
				<label>
					<asp:Literal ID="lblFPIdentityLabel" runat="server" />
				</label>
				<asp:TextBox ID="tbFPIdentity" runat="server" CssClass="form-control" />
				<asp:RequiredFieldValidator ID="rfFPIdentity" runat="server" ControlToValidate="tbFPIdentity" Display="Dynamic" CssClass="Validator" />
			</div>
			<asp:PlaceHolder ID="phFPButton" runat="server" />
		</asp:View>

        <asp:View ID="vwResetOptions" runat="server">
            <div>
                <asp:Label ID="lblResetOptionsErrorMessage" runat="server" CssClass="Validator" />
            </div>
            <div class="form-group">
                <label style="width:100% !important;">
                    <asp:Literal ID="lblResetOptionsTitleMessage" runat="server" />
                </label>
            </div>
            <div class="form-group">
                <asp:RadioButtonList ID="rblResetOptionsList" runat="server" />
            </div>
            <asp:PlaceHolder ID="phResetOptionsButton" runat="server" />
        </asp:View>

        <asp:View ID="vwResetCode" runat="server">
            <div>
                <asp:Label ID="lblResetCodeErrorMessage" runat="server" CssClass="Validator" />
            </div>
            <div class="form-group">
                <label style="width:100% !important;">
					<asp:Literal ID="lblResetCodeExplanation" runat="server" />
				</label>
            </div>
            <div class="form-group">
				<label style="width:100% !important;">
					<asp:Literal ID="lblResetCodeText" runat="server" />
				</label>
				<asp:TextBox ID="tbResetCode" runat="server" CssClass="form-control" />
				<asp:RequiredFieldValidator ID="rfResetCode" runat="server" ControlToValidate="tbResetCode" Display="Dynamic" CssClass="Validator" />
			</div>
			<asp:PlaceHolder ID="phResetCodeButton" runat="server" />
        </asp:View>

        <asp:View ID="vwChangePassword" runat="server">
			<div>
				<asp:Label ID="lblCPErrorMessage" runat="server" CssClass="Validator" />
			</div>
			<div class="form-group">
                <label style="width:100% !important;">
					<asp:Literal ID="lblCPExplanation" runat="server" />
				</label>
            </div>
            <div class="form-group">
				<label>
					<asp:Literal ID="lblCPPassword" runat="server" />
				</label>
				<asp:TextBox ID="tbCPPassword" runat="server" TextMode="Password" CssClass="form-control" />
				<asp:RequiredFieldValidator ID="rfCPPassword" runat="server" ControlToValidate="tbCPPassword" Display="Dynamic" CssClass="Validator" />
			</div>
			<div class="form-group">
                <label>
					<asp:Literal ID="lblCPConfirm" runat="server" />
				</label>
				<asp:TextBox ID="tbCPConfirm" runat="server" TextMode="Password" CssClass="form-control" />
				<asp:RequiredFieldValidator ID="rfCPConfirm" runat="server" ControlToValidate="tbCPConfirm" Display="Dynamic" CssClass="Validator" />
				<asp:CompareValidator ID="cmpCPConfirm" runat="server" ControlToValidate="tbCPConfirm" ControlToCompare="tbCPPassword" Type="String" Operator="Equal" Display="Dynamic" CssClass="Validator" />
			</div>
			<asp:PlaceHolder ID="phCPButton" runat="server" />
			
		</asp:View>

		<asp:View ID="vwPasswordChanged" runat="server">
			<div>
				<asp:Label ID="lblPCMessage" runat="server" CssClass="Positive" />
			</div>
			<div>
				<asp:PlaceHolder ID="phPCButton" runat="server" />
			</div>
		</asp:View>
	</asp:MultiView>
</div>