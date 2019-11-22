<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardAppeasement.ascx.cs"  Inherits="Brierley.AEModules.RewardAppeasement.ViewRewardAppeasement" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadCodeBlock id="RadCodeBlock1" runat="server">
<script type="text/javascript">
    function RewardAppeasementCancelClick() {
        $('#<%=ddlReward.ClientID%>').val('0');
        $('#<%=txtNotes.ClientID%>').val('');
        //(Begin)[AEO-2372]->[Dev Jonatan Uscanga]
        $('#<%=btnSave.ClientID%>').removeAttr('disabled');
        //(End)[AEO-2372]
        return false;
    }

    //(Begin)[AEO-2372]->[Dev Jonatan Uscanga]
    function DisabledLinkBtnSave() {
        debugger;
        if (typeof ($('#<%=btnSave.ClientID%>').attr('disabled')) == 'string' && $('#<%=btnSave.ClientID%>').attr('disabled') == 'disabled')
        {
            return false;
        }
        $('#<%=btnSave.ClientID%>').attr("disabled", 'disabled');
        return true;
    }
    //(End)[AEO-2372]
</script>
<%--(Begin)[AEO-2372]->[Dev Jonatan Uscanga]--%>
<style>
    a[disabled=disabled] 
    {
        color:gray;
    }
</style>
<%--(End)[AEO-2372]--%>
</telerik:RadCodeBlock>
<asp:Panel ID="pnlBanner" runat="server" BackColor="Gray" Width="100%">
    <asp:Label ID="lblBanner" runat="server" Font-Size="Large" ForeColor="White"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlMain" runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:Label ID="lblReward" runat="server" CssClass="radLabelCss_Default" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="ddlReward" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlReward_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rqPointType" runat="server" Display="None" 
                    ControlToValidate="ddlReward" ErrorMessage="Please select a reward." InitialValue="0" 
                    ValidationGroup="valSave"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblAddNotes" runat="server" resourcekey="lblAddNotes"  CssClass="radLabelCss_Default" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Height="66px" Width="435px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                    ShowSummary="False" ValidationGroup="valSave" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <%--(Begin)[AEO-2372]->[Dev Jonatan Uscanga] => addition of "OnClientClick="return DisabledLinkBtnSave();""--%>
                <asp:LinkButton ID="btnSave" runat="server" Text="Submit" OnClientClick="return DisabledLinkBtnSave();" ValidationGroup="valSave" onclick="btnSave_Click" ></asp:LinkButton>
                <%--(End)[AEO-2372]--%>
                &nbsp;&nbsp;
                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return RewardAppeasementCancelClick();" onclick="btnCancel_Click" ></asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>
