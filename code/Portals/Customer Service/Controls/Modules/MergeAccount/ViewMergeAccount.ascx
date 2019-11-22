<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMergeAccount.ascx.cs" Inherits="Brierley.LWModules.MergeAccount.ViewMergeAccount" %>
<script type="text/javascript">
    $(document).ready(function () {
        $('input[id$="txtFromMemberLoyaltyId"]').change(function (event) {
            $('span[id$="lblError"]').text('');
        });
    });	  
</script>
<div id="MergeAccountContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>

	<asp:Panel ID="pnlMain" runat="server">

		<asp:PlaceHolder runat="server" ID="pchSuccessMessage" Visible="false">
			<div class="Positive">
				<asp:Label runat="server" ID="lblSuccess" meta:resourcekey="lblSuccess"></asp:Label>
			</div>
		</asp:PlaceHolder>

		<div id="Instructions">
			<asp:Label ID="lblInstructions" runat="server" meta:resourcekey="lblInstructions"></asp:Label>
		</div>

		<div id="ToMember">
			<asp:Label ID="lblToMemberLabel" runat="server" meta:resourcekey="lblToMember"></asp:Label>
			<asp:Label ID="lblToMemberLoyaltyIDText" runat="server" Text=""></asp:Label>
		</div>

		<div id="FromMember">
			<asp:Label ID="lblFromMemberLabel" runat="server" meta:resourcekey="lblFromMember"></asp:Label>
			<asp:TextBox ID="txtFromMemberLoyaltyId" MaxLength="30" runat="server"></asp:TextBox>
			<asp:LinkButton ID="lnkValidate" runat="server" meta:resourcekey="lnkValidate" />

			<div>
				<asp:RequiredFieldValidator runat="server" ID="reqLoyaltyId" Display="Dynamic" ControlToValidate="txtFromMemberLoyaltyId" meta:resourcekey="reqLoyaltyId" CssClass="Validator"></asp:RequiredFieldValidator>
			</div>
		</div>

		<asp:PlaceHolder runat="server" ID="pchMergeConfirmation" Visible="false">
			<div id="MergeConfirmation">
				<h3>
					<asp:Label ID="lblToMemberConfirm" runat="server" meta:resourcekey="lblToMemberConfirm"></asp:Label>
				</h3>				
				
				<div class="table_wrapper"><div class="table_wrapper_inner"><div>
				<table>
					<tr>
						<th>
							<asp:Label runat="server" ID="lblToLoyaltyIdHeader" meta:resourcekey="lblToLoyaltyIdHeader"></asp:Label>
						</th>
						<th>
							<asp:Label runat="server" ID="lblToNameHeader" meta:resourcekey="lblToNameHeader"></asp:Label>
						</th>
						<th>
							<asp:Label runat="server" ID="lblToEmailHeader" meta:resourcekey="lblToEmailHeader"></asp:Label>
						</th>
						<th>
							<asp:Label runat="server" ID="lblToAddressHeader" meta:resourcekey="lblToAddressHeader"></asp:Label>
						</th>
                        <th>
							<asp:Label runat="server" ID="lblToMemberStatus" meta:resourcekey="lblToMemberStatus"></asp:Label>
						</th>
					</tr>
					<tr>
						<td>
							<asp:Label runat="server" ID="lblToLoyaltyId"></asp:Label>
						</td>
						<td>
							<asp:Label runat="server" ID="lblToName"></asp:Label>
						</td>
						<td>
							<asp:Label runat="server" ID="lblToEmail"></asp:Label>
						</td>
						<td>
							<asp:Label runat="server" ID="lblToAddress"></asp:Label>
						</td>
                        <td>
							<asp:Label runat="server" ID="lblToStatus"></asp:Label>
						</td>
					</tr>
				</table>
				</div></div></div>

				<h3>
					<asp:Label ID="lblFromMemberConfirm" runat="server" meta:resourcekey="lblFromMemberConfirm"></asp:Label>
				</h3>
				
				<div class="table_wrapper"><div class="table_wrapper_inner"><div>
				<table>
					<tr>
						<th>
							<asp:Label runat="server" ID="lblFromLoyaltyIdHeader" meta:resourcekey="lblFromLoyaltyIdHeader"></asp:Label>
						</th>
						<th>
							<asp:Label runat="server" ID="lblFromNameHeader" meta:resourcekey="lblFromNameHeader"></asp:Label>
						</th>
						<th>
							<asp:Label runat="server" ID="lblFromEmailHeader" meta:resourcekey="lblFromEmailHeader"></asp:Label>
						</th>
						<th>
							<asp:Label runat="server" ID="lblFromAddressHeader" meta:resourcekey="lblFromAddressHeader"></asp:Label>
						</th>
                        <th>
							<asp:Label runat="server" ID="lblFromMemberStatus" meta:resourcekey="lblFromMemberStatus"></asp:Label>
						</th>
					</tr>
					<tr>
						<td>
							<asp:Label runat="server" ID="lblFromLoyaltyId"></asp:Label>
						</td>
						<td>
							<asp:Label runat="server" ID="lblFromName"></asp:Label>
						</td>
						<td>
							<asp:Label runat="server" ID="lblFromEmail"></asp:Label>
						</td>
						<td>
							<asp:Label runat="server" ID="lblFromAddress"></asp:Label>
						</td>
                        <td>
							<asp:Label runat="server" ID="lblFromStatus"></asp:Label>
						</td>
					</tr>
				</table>
				</div></div></div>
			</div>
		</asp:PlaceHolder>

		<asp:PlaceHolder runat="server" ID="pchAdditionalNotes">
			<div id="AdditionalNotes">
				<span id="AdditionalNotesLabel">
					<asp:Label runat="server" ID="lblAdditionalNotes" meta:resourcekey="lblAdditionalNotes"></asp:Label>
				</span>

				<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine"></asp:TextBox>

			</div>
		</asp:PlaceHolder>

		<div class="ActionButtons">
			<asp:LinkButton runat="server" ID="lnkMerge" meta:resourcekey="lnkMerge"></asp:LinkButton>
		</div>

	</asp:Panel>
</div>