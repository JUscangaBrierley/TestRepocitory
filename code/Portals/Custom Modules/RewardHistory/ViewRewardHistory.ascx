<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardHistory.ascx.cs"
    Inherits="Brierley.AEModules.RewardHistory.ViewRewardHistory" %>
<asp:Panel ID="pnlMain" runat="server" Visible="true">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.myexclusiveclass').each(function () {
                var tr = $(this).parent().parent().parent();
                if ($(this).text() == '0') {
                    $('td:last', tr).html('');
                } else {
                    $('td:last', tr).find('a').text('REPLACE');
                }
            });

        });
    </script>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td align="left">
                <asp:Panel ID="pnlBanner" runat="server" BackColor="Gray" Width="100%">
                    <asp:Label ID="lblBannerText" runat="server" Text="" Font-Size="Large" ForeColor="White"></asp:Label>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="lblInstructionCopy" runat="server" resourcekey="lblInstructionCopy"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:DropDownList ID="ddlDate" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlDate_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="lblInstructionalReplacement" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlRewardHistoryGrid" runat="server">
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Panel>
