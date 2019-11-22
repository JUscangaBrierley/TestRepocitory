<%@ Control Language="C#" AutoEventWireup="true" 
CodeBehind="ViewRedemptions.ascx.cs" Inherits="Brierley.AEModules.Redemptions.ViewRedemptions" %>

<asp:Panel ID="pnlMain" runat="server" Visible="true">
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
                <asp:Label ID="lblInstructionCopy" runat="server" resourcekey="lblInstructionCopy" Text=""></asp:Label>
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
                &nbsp;
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
                <asp:Panel ID="pnlRedemptionsGrid" runat="server">
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Panel>