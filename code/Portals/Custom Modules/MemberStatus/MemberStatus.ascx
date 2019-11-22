<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MemberStatus.ascx.cs" Inherits="Brierley.AEModules.MemberStatus.MemberStatus" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadCodeBlock id="RadCodeBlock1" runat="server">
<script type="text/javascript">
    function TerminateCancelClick() {
        $('#<%=ddlTerminate.ClientID%>').val('0');
        $('#<%=txtNotes.ClientID%>').val('');
        return false;
    };
    $(document).ready(function () {
        if ($('#<%=ddlNewStatus.ClientID%> option:selected').val() == '3') {
            $('#trTerminateLabel').show();
            $('#trTerminateDDL').show();
        } else {
            $('#trTerminateLabel').hide();
            $('#trTerminateDDL').hide();
        };
        $('#<%=ddlNewStatus.ClientID%>').change(function (e) {
            if ($('#<%=ddlNewStatus.ClientID%> option:selected').val() == '3') {
                $('#trTerminateLabel').show();
                $('#trTerminateDDL').show();
            } else {
                $('#trTerminateLabel').hide();
                $('#trTerminateDDL').hide();
            }
        });
    });
    function validateTerminate(s, args) {
        if ($('#<%=ddlNewStatus.ClientID%> option:selected').val() == '3') {
            if ($('#<%=ddlTerminate.ClientID%> option:selected').val() != '0') {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
            }
        }
        else {
            args.IsValid = true;
        }
    }
</script>
</telerik:RadCodeBlock>
<asp:Panel ID="pnlBanner" runat="server" BackColor="#c0c0c0" Width="100%">
    <asp:Label ID="lblBanner" runat="server" Text="MEMBER STATUS" Font-Bold="True" Font-Size="Large" ForeColor="Black"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlMain" runat="server">
    <table width="100%">
        <tr>
            <td>
                <div style="font-size:large;font-weight:700">Current Status : 
                <asp:Label ID="lblCurrenStatus" runat="server" Text=""/>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div style="font-size:Large;">
                    <asp:Label ID="lblNewStatus" runat="server" CssClass="radLabelCss_Default" Text="Select new Status :" />
                    &nbsp;<asp:DropDownList ID="ddlNewStatus" runat="server">
                    </asp:DropDownList>
                </div>
            </td>
        </tr>
        <tr id="trTerminateLabel" style="display: none">
            <td>
                <asp:Label ID="lblTerminate" runat="server" Text="Select a Termination reason from the list below" CssClass="radLabelCss_Default" />
            </td>
        </tr>
        <tr id="trTerminateDDL" style="display: none">
            <td>
                <asp:DropDownList ID="ddlTerminate" runat="server">
                </asp:DropDownList>
                <asp:CustomValidator ID="cusTerminate" runat="server" Display="None"
                    ControlToValidate="ddlTerminate" ErrorMessage="Please select a terminate reason." InitialValue="0"
                    ValidationGroup="valSave" ClientValidationFunction="validateTerminate"></asp:CustomValidator>
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
                <asp:RequiredFieldValidator ID="rqNote" runat="server" Display="None" 
                    ControlToValidate="txtNotes" ErrorMessage="Please add a note."
                    ValidationGroup="valSave"></asp:RequiredFieldValidator>
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
            
                <asp:LinkButton ID="btnSave" runat="server" Text="Submit" ValidationGroup="valSave" onclick="btnSave_Click" ></asp:LinkButton>
                &nbsp;&nbsp;
                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return TerminateCancelClick();" onclick="btnCancel_Click" ></asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Panel>