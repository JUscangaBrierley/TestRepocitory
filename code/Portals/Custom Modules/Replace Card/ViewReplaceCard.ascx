<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewReplaceCard.ascx.cs"
    Inherits="Brierley.AEModules.ReplaceCard.ViewReplaceCard" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="pnlRepCard" runat="server" BackColor="Gray" Width="100%" HorizontalAlign="Left">
    <asp:Label ID="lblRepCard" runat="server" Font-Size="Large" ForeColor="White"></asp:Label>
</asp:Panel>
<br />
<table width="100%" cellspacing="0" cellpadding="0" border="0" summary="Edit Table">
    <tr>
        <td>
            <table width="100%" cellspacing="0" cellpadding="0" border="0" summary="Edit Table">
                <tr>
                    <td style="width: 30%">
                        Select Brand:
                    </td>
                    <td>
                        <asp:RadioButtonList ID="radioBaseBrand" runat="server" RepeatDirection="Horizontal"
                            TabIndex="1">
                            <asp:ListItem Value="1">AE</asp:ListItem>
                            <asp:ListItem Value="2">aerie</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td align="left">
            <asp:Label ID="lblCaption" runat="server" Text="Click Submit to request a replacement card"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 100%" align="center">
            <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click"></asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td align="left">
            <br />
            <asp:PlaceHolder ID="pMessage" runat="server"></asp:PlaceHolder>
            <asp:Label ID="lblSucc" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<br />
<br />
<asp:Panel ID="pnlCardHistory" runat="server" BackColor="Gray" Width="100%" HorizontalAlign="Left">
    <asp:Label ID="lblCardHistory" runat="server" Font-Size="Large" ForeColor="White"></asp:Label>
</asp:Panel>
<br />
<table width="100%" cellspacing="0" cellpadding="0" border="0" summary="Edit Table">
    <tr>
        <td>
            <asp:PlaceHolder ID="phCardHistory" runat="server" Visible="true" />
        </td>
    </tr>
</table>
