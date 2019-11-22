<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRequestCredit.ascx.cs" 
    Inherits="Brierley.LWModules.RequestCredit.ViewRequestCredit" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls.Mobile" TagPrefix="Mobile" %>

<div id="RequestCreditContainer">
	<asp:PlaceHolder runat="server">
		<script type="text/javascript">
			var _clientIds = { SearchDiv: "<%= divSearchForm.ClientID%>", ddlTransactionType: "<%= ddlTransactionType.ClientID%>" };

			$(document).ready(function () {
				var ddl = $('#' + _clientIds.ddlTransactionType);
				if (ddl.length > 0) {
					ddl.bind('change', function () { SetTransactionType(this); });
					var type = $('#' + _clientIds.ddlTransactionType).val();
					SetFocus(type);
				}
			});

			function SetTransactionType(listItem) {
				var type = $(listItem).val();
				$('#' + _clientIds.SearchDiv + ' > div').each(function () { $(this).hide(); });
				$('#' + _clientIds.SearchDiv + ' .' + type).show();
				SetFocus(type);
			}

			function SetFocus(type) {
				$('#' + _clientIds.SearchDiv + ' .' + type + ' input')[0].focus();
			}

			function ClearSearch() {
				$('#' + _clientIds.SearchDiv + ' input').val('');
				if (typeof (Page_Validators) != "undefined") {
					for (var i = 0; i < Page_Validators.length; i++) {
						var validator = Page_Validators[i];
						validator.isvalid = true;
						ValidatorUpdateDisplay(validator);
					}
				}
				$('#RequestCreditContainer #AlreadyApplied').hide();
				$('#RequestCreditContainer #Success').hide();
				$('#RequestCreditContainer #NoResults').hide();
				$('#RequestCreditContainer #SearchResults').hide();
			}
		</script>
	</asp:PlaceHolder>
	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>

	<div id="TransactionType" class="form-inline">
		<asp:PlaceHolder ID="divTransactionType" runat="server">
			<span><asp:Label ID="lblTransactionType" runat="server" Text=""></asp:Label></span>
			<asp:DropDownList ID="ddlTransactionType" runat="server"></asp:DropDownList>
			<Mobile:RadioButtonList runat="server" id="rdoTransactionType"></Mobile:RadioButtonList>
		</asp:PlaceHolder>
	</div>

	<div id="divSearchForm" runat="server">
	</div>

	<div id="AlreadyApplied">
		<asp:Label ID="lblApliedMsg" runat="server" Text=""></asp:Label>
	</div>

	<div id="Success">
		<asp:Label ID="lblSuccess" runat="server"></asp:Label>
	</div>

	<div id="NoResults" class="empty-list">
		<asp:Label ID="lblNoResults" runat="server"></asp:Label>
	</div>

	<div id="SearchResults">
		<asp:Panel ID="pnlSearchResult" runat="server" Visible="false">
		</asp:Panel>
	</div>
</div>