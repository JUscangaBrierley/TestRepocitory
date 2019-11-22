<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewLoyaltyCardsGrid.ascx.cs" Inherits="Brierley.LWModules.LoyaltyCardsGrid.ViewLoyaltyCardsGrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div id="LoyaltyCardsContainer">
	<h2 id="Title">
		<asp:Literal runat="server" ID="h2Title"></asp:Literal>
	</h2>
	<asp:Label runat="server" ID="lblNoResults" Visible="false"></asp:Label>
	<asp:PlaceHolder ID="phLoyaltyCards" runat="server" Visible="true" />
	<asp:LinkButton runat="server" ID="lnkAddNew" CausesValidation="false" meta:resourcekey="lnkAddNew"></asp:LinkButton>
	
	<asp:MultiView runat="server" ID="CardOperationViews">
		<asp:View runat="server" ID="AddCardView">
			<h3>
				<asp:Label runat="server" ID="lblAddCard" meta:resourcekey="lblAddCard"></asp:Label>
			</h3>
			<table>
				<tr>
					<td>
						<asp:Label ID="lblLoyaltyId" runat="server" meta:resourcekey="lblLoyaltyId" />
					</td>
					<td>
						<asp:TextBox runat="server" ID="txtLoyaltyId"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="reqLoyaltyId" ControlToValidate="txtLoyaltyId" CssClass="Validator" Display="Dynamic" EnableClientScript="true" meta:resourcekey="reqLoyaltyId"></asp:RequiredFieldValidator>
						<asp:CustomValidator runat="server" ID="vldLoyaltyIdExists" ControlToValidate="txtLoyaltyId" CssClass="Validator" meta:resourcekey="vldLoyaltyIdExists"></asp:CustomValidator>
					</td>
				</tr>
                <tr>
					<td>
						<asp:Label ID="lblLoyaltyIdConfirm" runat="server" meta:resourcekey="lblLoyaltyIdConfirm" Visible = "false"/>
					</td>
					<td>
						<asp:TextBox runat="server" ID="txtLoyaltyIdConfirm" Visible="false"></asp:TextBox>						
					</td>
				</tr>
				<asp:PlaceHolder runat="server" ID="pchCardType">
					<tr>
						<td>
							<asp:Label runat="server" ID="lblCardType" meta:resourcekey="lblCardType"></asp:Label>
						</td>
						<td>
							<asp:DropDownList runat="server" ID="ddlCardType"></asp:DropDownList>
						</td>
					</tr>
				</asp:PlaceHolder>
                <tr>
					<td>
						<asp:Label ID="lblExpirationDate" runat="server" meta:resourcekey="lblExpirationDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dtExpirationDate" runat="server" DateInput-DateFormat="d" />
					</td>
				</tr>
			</table>
			<div class="ActionButtons">
				<asp:LinkButton ID="lnkNewOk" runat="server" meta:resourcekey="lnkNewOk"></asp:LinkButton>
				<asp:LinkButton ID="lnkNewCancel" runat="server" meta:resourcekey="lnkNewCancel" CausesValidation="False"></asp:LinkButton>
			</div>

		</asp:View>
		<asp:View runat="server" ID="PrimaryConfirmationView">
			<div id="PrimaryConfirmation">
				<h3>
					<asp:Label runat="server" ID="lblPrimary" meta:resourcekey="lblPrimary"></asp:Label>
				</h3>
				<asp:Label runat="server" ID="lblConfirmationMessage"></asp:Label>
				<div class="ActionButtons">
					<asp:LinkButton ID="lnkPrimaryOk" runat="server" meta:resourcekey="lnkPrimaryOk"></asp:LinkButton>
					<asp:LinkButton ID="lnkPrimaryCancel" runat="server" meta:resourcekey="lnkPrimaryCancel" CausesValidation="False"></asp:LinkButton>
				</div>
			</div>
		</asp:View>
		<asp:View runat="server" ID="CardCancellationView">
			<h3>
				<asp:Label runat="server" ID="lblCancel" meta:resourcekey="lblCancel"></asp:Label>
			</h3>

			<table>
				<tr>
					<td>
						<asp:Label ID="lblCardToCancel" runat="server" meta:resourcekey="lblCardToCancel" />
					</td>
					<td>
						<asp:Label ID="lblCancelCardId" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblEffectiveDate" runat="server" meta:resourcekey="lblEffectiveDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dtEffective" runat="server" DateInput-DateFormat="d" />
					</td>
				</tr>
				<tr>
					<td valign="top">
						<asp:Label ID="lblCancelReason" runat="server" meta:resourcekey="lblReason" />
					</td>
					<td>
						<asp:TextBox ID="txtCancelReason" runat="server" TextMode="MultiLine"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblDeactivateMember" runat="server" meta:resourcekey="lblDeactivateMember" />
					</td>
					<td>
						<asp:CheckBox ID="chkDeactivateMember" runat="server" Checked="false"></asp:CheckBox>
					</td>
				</tr>
			</table>
			<div class="ActionButtons">
				<asp:LinkButton ID="lnkCancelOk" runat="server" meta:resourcekey="lnkCancelOk"></asp:LinkButton>
				<asp:LinkButton ID="lnkCancelCancel" runat="server" meta:resourcekey="lnkCancelCancel" CausesValidation="False"></asp:LinkButton>
			</div>
		</asp:View>
		<asp:View runat="server" ID="ReplaceCardView">
			<h3>
				<asp:Label runat="server" ID="lblReplace" meta:resourcekey="lblReplace"></asp:Label>
			</h3>

			<table>
				<tr>
					<td>
						<asp:Label ID="lblCardToReplace" runat="server" meta:resourcekey="lblCardToReplace" />
					</td>
					<td>
						<asp:Label ID="lblReplaceCardId" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblReplacementCardId" runat="server" meta:resourcekey="lblReplacementCardId" />
					</td>
					<td>
						<asp:DropDownList ID="ddlReplacementCards" runat="server"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblReplTransferPoints" runat="server" meta:resourcekey="lblTransferPoints" />
					</td>
					<td>
						<asp:CheckBox ID="chkReplacementTransferPoints" runat="server" Checked="true"></asp:CheckBox>
					</td>
				</tr>				
				<tr>
					<td>
						<asp:Label ID="lblReplEffectiveDate" runat="server" meta:resourcekey="lblEffectiveDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dtReplEffectiveDate" runat="server" DateInput-DateFormat="d" />
					</td>
				</tr>				
				<tr>
					<td valign="top">
						<asp:Label ID="lblReplReason" runat="server" meta:resourcekey="lblReason" />
					</td>
					<td>
						<asp:TextBox ID="txtReplReason" runat="server" TextMode="MultiLine"></asp:TextBox>
					</td>
				</tr>
			</table>
			<div class="ActionButtons">
				<asp:LinkButton ID="lnkReplOk" runat="server" meta:resourcekey="lnkReplOk"></asp:LinkButton>
				<asp:LinkButton ID="lnkReplCancel" runat="server" meta:resourcekey="lnkReplCancel" CausesValidation="False"></asp:LinkButton>
			</div>
		</asp:View>
        <asp:View runat="server" ID="RenewCardView">
            <h3>
				<asp:Label runat="server" ID="lblRenew" meta:resourcekey="lblRenew"></asp:Label>
			</h3>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblCardToRenew" runat="server" meta:resourceKey="lblCardToRenew" />
                    </td>
                    <td>
						<asp:Label ID="lblCardToRenewId" runat="server"></asp:Label>
					</td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblRenewCardStatusLabel" runat="server" meta:resourceKey="lblRenewCardStatusLabel" />
                    </td>
                    <td>
						<asp:Label ID="lblRenewCardStatus" runat="server"></asp:Label>
					</td>
                </tr>
                <tr>
					<td>
						<asp:Label ID="lblRenewCardCurrentExpirationLabel" runat="server" meta:resourcekey="lblRenewCardCurrentExpirationLabel" />
					</td>
					<td>
						<asp:Label ID="lblRenewCardCurrentExpiration" runat="server" Checked="true"></asp:Label>
					</td>
				</tr>
                <tr>
					<td>
						<asp:Label ID="lblRenewCardNewExpiration" runat="server" meta:resourcekey="lblRenewCardNewExpiration" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dtNewExpirationDate" runat="server" DateInput-DateFormat="d" />
					</td>
				</tr>
            </table>
            <div class="ActionButtons">
				<asp:LinkButton ID="lnkRenewOk" runat="server" meta:resourcekey="lnkRenewOk"></asp:LinkButton>
				<asp:LinkButton ID="lnkRenewCancel" runat="server" meta:resourcekey="lnkRenewCancel" CausesValidation="False"></asp:LinkButton>
			</div>
        </asp:View>
		<asp:View runat="server" ID="TransferCardView">
			<h3>
				<asp:Label runat="server" ID="lblTransfer" meta:resourcekey="lblTransfer"></asp:Label>
			</h3>

			<table>
				<tr>
					<td>
						<asp:Label ID="lblCardToTransfer" runat="server" meta:resourcekey="lblCardToTransfer" />
					</td>
					<td>
						<asp:Label ID="lblTransferCard" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblTargetMemberIdType" runat="server" meta:resourcekey="lblTargetMemberIdType" />
					</td>
					<td>
						<asp:DropDownList ID="ddlTargetMemberId" runat="server"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblTargetMemberId" runat="server" meta:resourcekey="lblTargetMemberId" />
					</td>
					<td>
						<asp:TextBox ID="txtMemberId" runat="server" Width="200"></asp:TextBox>
							
						<asp:RequiredFieldValidator runat="server" ID="reqMemberId" ControlToValidate="txtMemberId" Display="Dynamic" EnableClientScript="true" meta:resourcekey="reqTargetMemberId" CssClass="Validator"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblMakePrimary" runat="server" meta:resourcekey="lblMakePrimary" />
					</td>
					<td>
						<asp:CheckBox ID="chkMakeCardPrimary" runat="server" Checked="false"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="Label6" runat="server" meta:resourcekey="lblDeactivateMember" />
					</td>
					<td>
						<asp:CheckBox ID="txtTsfrDeactivateMember" runat="server" Checked="false"></asp:CheckBox>
					</td>
				</tr>
			</table>
			<div class="ActionButtons">
				<asp:LinkButton ID="lnkTransferOk" runat="server" meta:resourcekey="lnkTransferOk"></asp:LinkButton>
				<asp:LinkButton ID="lnkTransferCancel" runat="server" meta:resourcekey="lnkTransferCancel" CausesValidation="False"></asp:LinkButton>
			</div>
		</asp:View>
        <asp:View runat="server" ID="PassbookCardView">
            <h3>
				<asp:Label runat="server" ID="lblPassbook" meta:resourcekey="lblPassbook"></asp:Label>
			</h3>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblCardToSend" runat="server" meta:resourceKey="lblCardToSend" />
                    </td>
                    <td>
						<asp:Label ID="lblCardToSendId" runat="server"></asp:Label>
					</td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblMemberEmailLabel" runat="server" meta:resourceKey="lblMemberEmailLabel" />
                    </td>
                    <td>
						<asp:Label ID="lblMemberEmail" runat="server"></asp:Label>
					</td>
                </tr>                                
            </table>
            <div class="ActionButtons">
				<asp:LinkButton ID="lnkPassbookOk" runat="server" meta:resourcekey="lnkPassbookOk"></asp:LinkButton>
				<asp:LinkButton ID="lnkPassbookCancel" runat="server" meta:resourcekey="lnkPassbookCancel" CausesValidation="False"></asp:LinkButton>
			</div>
        </asp:View>
	</asp:MultiView>
</div>
