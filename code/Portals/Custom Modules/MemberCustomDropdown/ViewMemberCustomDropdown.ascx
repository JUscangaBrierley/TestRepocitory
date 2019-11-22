<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMemberCustomDropdown.ascx.cs"
    Inherits="Brierley.AEModules.MemberCustomDropdown.ViewMemberCustomDropdown" %>
<asp:Panel ID="pnlMain" runat="server">
    <table cellpadding="3" cellspacing="0" border="0">
        <tr>
            <td>
                <asp:DropDownList ID="ddlDate" EnableViewState="true" runat="server" AutoPostBack="True" 
                    onselectedindexchanged="ddlDate_SelectedIndexChanged" 
                    CausesValidation="True" ontextchanged="ddlDate_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td id="tdPointBalance" runat="server">
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblPointBalanceText" runat="server" Font-Bold="true" Text="Available Point Balance: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblPointBalanceValue" runat="server" Font-Bold="true" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                &nbsp;&nbsp;&nbsp;
            </td>
            <!--PI 30364 - Dollar reward program - Start -->
            <td id="tdMonthlyPointBalance" runat="server">
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblMonthlyPointBalanceText" runat="server" Font-Bold="true" Text="Current Point Balance for your Selection: "></asp:Label>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:Label ID="lblMonthlyPointBalanceValue" runat="server" Font-Bold="true" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <!--PI 30364 - Dollar reward program - End -->
            <td>
                &nbsp;&nbsp;&nbsp;
            </td>
            <td id="tdBrandPurchased" runat="server">
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblBrandPurchasedText" runat="server" Font-Bold="true" Text="Current Brand Purchased:"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBoxList ID="ChkLBrandPurchased" Font-Bold="true" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Enabled="False" Value="1">AE</asp:ListItem>
                                <asp:ListItem Enabled="False" Value="2">aerie</asp:ListItem>
                                <%--AEO Redesign 2015 Begin  --%>
                                <%--<asp:ListItem Enabled="False" Value="3">77kids</asp:ListItem>--%>
                                <%--AEO Redesign 2015 End  --%>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
