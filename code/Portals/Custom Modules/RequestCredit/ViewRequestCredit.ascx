<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRequestCredit.ascx.cs"
    Inherits="Brierley.AEModules.RequestCredit.ViewRequestCredit" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script language="javascript" type="text/javascript">
    //AE specific code: TxnDate should be current year date
    $(document).ready(function () {
        $('input[id$=ViewRequestCredit_txt_TxnDate]').blur(function () {
            var txt = $(this).val().trim();
            if (txt.length > 0 && isDate(txt) && txt.lastIndexOf('/') > 0) {
                txt = txt.substring(txt.lastIndexOf('/') + 1);
                var d = new Date();
                if (d.getFullYear() != txt) {
                    $(this).parent().find('span:last-child').html("&nbsp;&nbsp;&nbsp;Please enter a valid transaction date. Transaction Date must be within the current calender year.").show();
                }
            }
        });
    });
    function isDate(dateStr) {

        var datePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{2}|\d{4})$/;
        var matchArray = dateStr.match(datePattern);

        //Check valid format
        if (matchArray == null) { return false; }

        month = matchArray[1];
        day = matchArray[3];
        year = matchArray[5];

        // check month range
        if (month < 1 || month > 12) { return false; }

        //Check day range
        if (day < 1 || day > 31) { return false; }

        //Check months with 30 days
        if ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30) { return false; }

        //Check Feb days
        if (month == 2) {
            var leapYr = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
            if (day > 29 || (day > 28 && !leapYr)) { return false; }
        }
        return true;
    }
//(Begin)[AEO-4225]->[Dev Francisco Sosa]
    function DisabledLinkBtn() {
        debugger;
        if (typeof ($('#<%=cmdSearch.ClientID%>').attr('disabled')) == 'string' && $('#<%=cmdSearch.ClientID%>').attr('disabled') == 'disabled')
        {
            return false;
        }
        $('#<%=cmdSearch.ClientID%>').attr("disabled", 'disabled');
        return true;
    }
    //(End)[AEO-4225]
</script>
<%--(Begin)[AEO-4225]->[Dev Francisco Sosa]--%>
<style>
    a[disabled=disabled] 
    {
        color:gray;
    }
</style>
<%--(End)[AEO-4225]--%>
 <telerik:RadCodeBlock id="RadCodeBlock1" runat="server">
<script type="text/javascript">
    function SetValDDL() {
        var insertIndex = document.getElementById('<%=hdnInsertVal.ClientID %>');
        insertIndex.value = document.getElementById('<%=ddlTransactionType.ClientID %>').value;
    }
    function SetValRDB(prm) {
        var insertIndex = document.getElementById('<%=hdnInsertVal.ClientID %>');
        insertIndex.value = prm;
    }

</script>
</telerik:RadCodeBlock>
<asp:Panel ID="pnlSearchForm" runat="server" Visible="false">
    <asp:Table ID="pnlSearchFormTable" Width="100%" CellSpacing="0" CellPadding="0" border="0"
        summary="Search Form" runat="server">
        <asp:TableRow ID="ddlRow" runat="server">
            <asp:TableCell ID="lblDDL" runat="server" HorizontalAlign="Right">Purchase Type:&nbsp;&nbsp; 
            </asp:TableCell>
            <asp:TableCell ID="ddlCell" runat="server">
                <asp:DropDownList ID="ddlTransactionType" AutoPostBack="true" runat="server" onchange="SetValDDL();"
                    OnSelectedIndexChanged="DdlTransactionType_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RadioButtonList ID="rdbTransactionType" AutoPostBack="true" RepeatDirection="Horizontal"
                    runat="server" OnSelectedIndexChanged="RdbTransactionType_SelectedIndexChanged">
                </asp:RadioButtonList>
                <asp:HiddenField ID="hdnInsertVal" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <%--(Begin)[AEO-4225]->[Dev Francisco Sosa] => addition of "OnClientClick="return DisabledLinkBtnSave();""--%>
    <table>
            <tr>
                <td> &nbsp; </td>
                <td >
                    <asp:LinkButton ID="cmdSearch" runat="server" Text="Submit" OnClientClick="return DisabledLinkBtn();" ValidationGroup="ValiDationSearchControls" onclick="CmdSearch_Click" ></asp:LinkButton>
                    
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" onclick="CmdCancel_Click" ></asp:LinkButton>
                </td>
            </tr>
        </table>
        <%--(End)[AEO-4225]--%>
</asp:Panel>
<br />
<asp:Table ID="tblSearchCtrls" Width="100%" CellSpacing="0" CellPadding="0" border="0"
    runat="server">
</asp:Table>
<br />
<asp:Label ID="lblApliedMsg" runat="server" Text=""></asp:Label>
<asp:Label ID="lblAddTransaction" EnableViewState="false" runat="server" Font-Bold="true"
    Text=""></asp:Label>
<asp:Label ID="lblEmptyResultMsg" Visible="false" runat="server" Text=""></asp:Label>
<br />
<asp:Label ID="lblResultGridTitle" runat="server" Font-Bold="true" Text=""></asp:Label>
<br />
<asp:Panel ID="pnlSearchResult" runat="server" Visible="false">
    <asp:PlaceHolder ID="phCSMemberSearch" runat="server" Visible="true" />
</asp:Panel>
<br />
<asp:Panel ID="PnlGetPointHistoryHeader" runat="server">
   <div style="background-color: #ccc; width: 100%; text-indent: 10px;">
        <h1>
            Get Points History</h1>
    </div>
</asp:Panel>
<asp:Panel ID="pnlGetPointHistory" runat="server" Visible="false">   
    <asp:PlaceHolder ID="phGetPointHistory" runat="server" Visible="true" />
    <br /> <br />
</asp:Panel>
<asp:Panel ID="pnlGetPointHistoryEmpty" runat="server" Visible="false">
    <br />
    <asp:Label ID="lblGetPointHistoryEmptyMessage" runat="server" Text=""></asp:Label>
    <br /> <br />
</asp:Panel>

