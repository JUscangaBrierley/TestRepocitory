<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardsHistory.ascx.cs"
    Inherits="Brierley.LWModules.RewardsHistory.ViewRewardsHistory" %>
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

<div id="RewardsHistoryContainer">
    <h2 id="Title">
        <asp:Literal runat="server" ID="h2Title"></asp:Literal>
    </h2>
    <div class="DateRangeFilter">
        <asp:PlaceHolder ID="pchDateTextBox" runat="server" Visible="false">
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td>
                        <asp:Label ID="lblFromDate" runat="server" meta:resourcekey="lblFromDate" />
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="dpFromDate" runat="server">
                        </telerik:RadDatePicker>
                    </td>
                    <td>
                        <asp:Label ID="lblToDate" runat="server" meta:resourcekey="lblToDate" />
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="dpToDate" runat="server">
                        </telerik:RadDatePicker>
                    </td>
                    <td>
                        <asp:LinkButton ID="btnSearch" runat="server" meta:resourcekey="btnSearch" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <asp:RequiredFieldValidator runat="server" ID="reqFromDate" ControlToValidate="dpFromDate"
                            EnableClientScript="true" Display="Dynamic" meta:resourcekey="reqFromDate" />
                        <asp:RequiredFieldValidator runat="server" ID="reqToDate" ControlToValidate="dpToDate"
                            EnableClientScript="true" Display="Dynamic" meta:resourcekey="reqToDate" />
                        <asp:CustomValidator id="customToDateValidator" ControlToValidate="dpToDate"
                            ClientValidationFunction="ClientValidateFromToDates"
                            Display="Dynamic" CssClass="Validator" ErrorMessage="From Date should be smaller than To Date" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="pchDateRange" Visible="false">
		    <asp:Label ID="lblSelectDateRange" runat="server" meta:resourcekey="lblSelectDateRange" />
            <asp:DropDownList runat="server" ID="ddlDateRange" AutoPostBack="true">
            </asp:DropDownList>
        </asp:PlaceHolder>
    </div>
    <asp:PlaceHolder ID="phRewardsHistory" runat="server" Visible="true" />
    <asp:Label runat="server" ID="lblNoResults" Visible="false"></asp:Label>
    <asp:Panel ID="pnlCancelOrder" runat="server" Visible="false">
        <table cellspacing="0" cellpadding="0" border="0">
            <h3>
                <asp:Label ID="lblCancelRewardTitle" runat="server" meta:resourcekey="lblCancelRewardTitle" />
            </h3>
            <tr>
                <td>
                    <asp:Label ID="lblOrderNumberLabel" runat="server" meta:resourcekey="lblOrderNumberLabel" />
                </td>
                <td>
                    <asp:Label ID="lblOrderNumber" runat="server" meta:resourcekey="lblOrderNumber" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblRewardNameLabel" runat="server" meta:resourcekey="lblRewardNameLabel" />
                </td>
                <td>
                    <asp:Label ID="lblRewardName" runat="server" meta:resourcekey="lblRewardName" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblNotes" runat="server" meta:resourcekey="lblNotes" />
                </td>
                <td>
                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" meta:resourcekey="txtNotes"
                        Columns="80" Rows="3" />
                </td>
            </tr>
        </table>
        <div class="ActionButtons">
            <asp:LinkButton ID="lnkCancelOk" runat="server" meta:resourcekey="lnkCancelOk" />
            <asp:LinkButton ID="lnkCancelCancel" runat="server" meta:resourcekey="lnkCancelCancel"
                CausesValidation="False" />
        </div>
    </asp:Panel>    
</div>
