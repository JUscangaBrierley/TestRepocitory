<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMemberTier.ascx.cs"
    Inherits="Brierley.AEModules.MemberTier.ViewMemberTier" %>
<asp:Panel ID="pnlMain" runat="server" Visible="true">
    <script language="javascript" type="text/javascript">
        function TerminateCancelClick() {
            $('#<%=txtNotes.ClientID%>').val('');
            return false;
        };

        $(document).ready(function () {
            $("#<%=btnSave.ClientID%>").click(function (event) {
                if (!AreValidDates() || !IsValidTier()) {
                    event.preventDefault();
                } else {
                    return;
                }
            });
        });
        
        function AreValidDates() {
            var IsValidStartDate = false;
            IsValidStartDate = DateIsValid(document.getElementById("<%=StartDate.ClientID %>").value);
            IsValidStartDate = IsValidStartDate ? IsGreaterThanToday(document.getElementById("<%=StartDate.ClientID %>").value) : false;
            

            var IsValidEndDate = false;
            IsValidEndDate = DateIsValid(document.getElementById("<%=EndDate.ClientID %>").value);
            IsValidEndDate = IsValidEndDate? IsLowerThanRule(document.getElementById("<%=EndDate.ClientID %>").value) : false;

            if (!IsValidStartDate) {
                Today = new Date();
                alert("Enter a Start Date in mm/dd/yyyy format and after " + (Today.getMonth() + 1).toString() + "/" + Today.getDate().toString() + "/" + Today.getFullYear().toString());
                return false;
            }

            if (!IsValidEndDate) {
                Today = new Date;
                var maxYear = Today.getFullYear() + 2;
                alert("Enter an End Date in mm/dd/yyyy format and before 1/1/" + maxYear.toString());
                return false;
            }

            return true;
        }

        function IsValidTier() {
            if ($('#<%=ddlTiers.ClientID%> option:selected').val() != '--SELECT--') {
                return true;
            }
            alert("Please select a Valid Tier");
            return false;
        }

        function IsGreaterThanToday(dateText) {
            Today = new Date();
            var txtdate = dateText;
            var arr = txtdate.split("/");
            var day = arr[1];
            var month = arr[0];
            var year = arr[2];

            if (year < Today.getFullYear()) {
                return false;
            } 
            if ((month < (Today.getMonth() + 1)) && year == Today.getFullYear()) {
                return false;
            }

            if (day < Today.getDate() && (month == (Today.getMonth() + 1)) && year == Today.getFullYear()) {
                return false;
            }
            else {
                return true;
            }
        }

        function IsLowerThanRule(dateText) {
            Today = new Date;
            var txtdate = dateText;
            var arr = txtdate.split("/");
            var day = arr[1];
            var month = arr[0];
            var year = arr[2];

            var maxYear = Today.getFullYear();
            maxYear += 2;
            var CurrentDate = new Date(year, month-1, day);
            var MaxDate = new Date(maxYear, 0, 1);

            if (CurrentDate > MaxDate) {
                return false;
            }
            else {
                return true;
            }
        }

        function DateIsValid(sDate) {
            if (typeof sDate != 'string')
                return false;

            var splitdate = sDate.split("/");
            //Validating three sections of format mm/dd/yyyy
            if( splitdate.length==3){
    	        var month = splitdate[0];
    	        var day = splitdate[1];
                var year = splitdate[2];
                //validating lengths in format sections and if are Integer values
                if( month.length<=0 ||month.length>2 || isNaN(parseInt(month))
        	        || day.length<=0 || day.length>2 || isNaN(parseInt(day))
                    || parseInt(year).toString().length != 4 || isNaN(year)
                ){return false;}
                if( (month.length == 2 && isNaN(parseInt(month[1])))
        	        || ( day.length == 2 && isNaN(parseInt(day[1])))
                ){return false;}
                // Getting Max day for month
                var monthAsInt=parseInt(month);
                var dayAsMonth = parseInt(day);        
                var maxday = 0;
                switch(monthAsInt){
        	        case 1: case 3: case 5: case 7: case 8: case 10: case 12:
            	        maxday=31;
            	        break;            
                    case 4: case 6: case 9: case 11:
            	        maxday=30;
            	        break;
                    case 2:
            	        //Get max day of month (including leap year validation)
                        if(!isleap(year))
                	        maxday=28;
                        else
                	        maxday=29;
            	        break;
                    default:
            	        return false; //Invalid Month
                }
                //Validate day of Month (including year leap on February)
                if(dayAsMonth<0 || dayAsMonth>maxday){ return false; }
                //Date Is valid
                return true;
            }
            else {return false;}
        }


            function isleap(leapyear) {
                var yr = leapyear;
                if ((parseInt(yr) % 4) == 0) {
                    if (parseInt(yr) % 100 == 0) {
                        if (parseInt(yr) % 400 != 0) {
                            return false;
                        }
                        if (parseInt(yr) % 400 == 0) {
                            return true;
                        }
                    }
                    if (parseInt(yr) % 100 != 0) {
                        return true;
                    }
                }
                if ((parseInt(yr) % 4) != 0) {
                    return false;
                }
            }
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
                <asp:DropDownList ID="ddlTiers" runat="server" AutoPostBack="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
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
                <asp:Label ID="lblStartDate" runat="server" Text="Start Date:   "></asp:Label>
                <asp:TextBox ID="StartDate" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rqStartDate" runat="server" Display="None" 
                    ControlToValidate="StartDate" ErrorMessage="Please add a Start Date."
                    ValidationGroup="valSave"></asp:RequiredFieldValidator>

                <asp:Label ID="lblEndDate" runat="server" Text="End Date:   "></asp:Label>
                <asp:TextBox ID="EndDate" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rqEndDate" runat="server" Display="None" 
                    ControlToValidate="EndDate" ErrorMessage="Please add an End Date."
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
                <br />
                <asp:LinkButton ID="btnSave" runat="server" Text="Submit" ValidationGroup="valSave" disabled="true" onclick="btnSave_Click" ></asp:LinkButton>
                &nbsp;&nbsp;
                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return TerminateCancelClick();" onclick="btnCancel_Click" ></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlMemberTierGrid" runat="server">
                </asp:Panel>
                <div style="text-align: right;">
                    <asp:Label ID="lblNetSpendText" runat="server" Text="*Net Spend as of Tier Change" ></asp:Label>
                </div>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Panel>