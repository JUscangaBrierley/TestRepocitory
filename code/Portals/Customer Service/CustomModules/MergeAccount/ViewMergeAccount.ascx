<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMergeAccount.ascx.cs"
    Inherits="Brierley.AEModules.MergeAccount.ViewMergeAccount" %>
<style type="text/css">
    .banner
    {
        text-indent: 10px;
        background-color: #c0c0c0;
    }
</style>
<asp:Panel ID="pnlMain" runat="server">
    <table cellpadding="1" cellspacing="0" border="0">
        <tr>
            <td>
                <h1><asp:Label ID="lblMergeHistoryBanner" Width="100%" CssClass="banner" resourcekey="lblMergeHistoryBanner" runat="server" Text=""></asp:Label></h1>
            </td>
        </tr>
        <tr>
            <td>
                <asp:PlaceHolder ID="phMergeHistory" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
