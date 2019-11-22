<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContactHistory.ascx.cs" Inherits="Brierley.LWModules.ContactHistory.ViewContactHistory" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">

    function ClientValidateFromToDates(source, arguments) {
        arguments.IsValid = true;
        var fromDateValue = $('input[id$="dpFromDate_dateInput"]').val();
        var toDateValue = $('input[id$="dpToDate_dateInput"]').val();
        if (fromDateValue != null && toDateValue != null && fromDateValue.length > 0 && toDateValue.length > 0) {
            var fromDate = Date.parse(fromDateValue);
            var toDate = Date.parse(toDateValue);
            if (!isNaN(fromDate) && !isNaN(toDate))
                arguments.IsValid = (fromDate <= toDate);
        }
    }
</script>

<div id="ContactHistoryContainer">
	<h2 id="Title">
		<asp:Literal runat="server" ID="h2Title"></asp:Literal>
	</h2>
	<div class="DateRangeFilter">
		<asp:Panel ID="pnlDateTextBox" runat="server" Visible="false">
			<table cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td>
						<asp:Label ID="lblFromDate" runat="server" Text="" meta:resourcekey="lblFromDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dpFromDate" runat="server">
						</telerik:RadDatePicker>
					</td>
					<td>
						<asp:Label ID="lblToDate" runat="server" Text="" meta:resourcekey="lblToDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dpToDate" runat="server">
						</telerik:RadDatePicker>
					</td>
					<td>
						<asp:LinkButton ID="btnSearch" runat="server" />
					</td>
				</tr>
				<tr>
					<td colspan="5">
						<asp:RequiredFieldValidator runat="server" ID="reqFromDate" ControlToValidate="dpFromDate"
							meta:resourcekey="reqFromDate" EnableClientScript="true" Display="Dynamic" CssClass="Validator"></asp:RequiredFieldValidator>
						<asp:RequiredFieldValidator runat="server" ID="reqToDate" ControlToValidate="dpToDate"
							meta:resourcekey="reqToDate" EnableClientScript="true" Display="Dynamic" CssClass="Validator"></asp:RequiredFieldValidator>
                        <asp:CustomValidator id="customToDateValidator" ControlToValidate="dpToDate"
                            ClientValidationFunction="ClientValidateFromToDates"
                            Display="Dynamic" CssClass="Validator" meta:resourcekey="customToDate" runat="server" />
					</td>
				</tr>
			</table>
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlDateRange" Visible="false">
			Select Date Range:
			<asp:DropDownList runat="server" ID="ddlDateRange" AutoPostBack="true">
			</asp:DropDownList>
		</asp:Panel>
	</div>
	<asp:PlaceHolder ID="phContactHistoryGrid" runat="server" Visible="true" />
	<asp:Label runat="server" ID="lblNoResults" Visible="false"></asp:Label>
</div>
