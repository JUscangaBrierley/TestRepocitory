﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCSTriggerUserEvent.ascx.cs" Inherits="Brierley.LWModules.CSTriggerUserEvent.ViewCSTriggerUserEvent" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel runat="server" ID="pnlCSTriggerUserEvent">
<script type="text/javascript">

    var _delimiter = '||';

    $(document).ready(function () { ExpandRows('<%=pnlCSTriggerUserEvent.ClientID %>'); });

	function ExpandRows(clientId) {
	    var expanded = $('#' + clientId + ' .hiddenRows input').val().split(_delimiter);
	    var links = $('#' + clientId + ' a');
	    for (var i = 0; i < expanded.length; i++) {
	        if (expanded[i] == '') {
	            continue;
	        }
	        for (var l = 0; l < links.length; l++) {
	            var onclick = $(links[l]).attr('onclick');
	            if (onclick != null) {
	                if (String(onclick).indexOf(expanded[i]) > -1) {
	                    ToggleNested(links[l], expanded[i]);
	                }
	            }
	        }
	    }
	}


	function ToggleNested(link, keyVal) {
	    var detail = $(link).parent().parent().next(); // <-- table row
	    var cell = detail.find('td');
	    if (detail.is(':visible')) {
	        detail.toggle();
	        $(link).html('+');
	        SetExpanded(link, keyVal, false);
	    }
	    else {
	        if (cell.html() != '') {
	            detail.toggle();
	            $(link).html('-');
	            SetExpanded(link, keyVal, true);
	        }
	        else {
	            $.ajax({
	                type: "POST",
	                url: "<%=Page.Request.RawUrl %>",
					data: { keyval: keyVal },
					success: function (msg) {
					    cell.html(msg);
					    detail.toggle();
					    $(link).html('-');
					    SetExpanded(link, keyVal, true);
					},
					dataType: "html",
					error: function (response, status, error) {
					    cell.html(error);
					    detail.toggle();
					    $(link).html('-');
					    SetExpanded(link, keyVal, true);
					}
				});
            }
        }
        return false;
    }

    function SetExpanded(link, keyVal, isExpanded) {

        var parent = $(link).parent();
        while (parent.attr('id') != 'CSTriggerUserEventContainer') {
            parent = parent.parent();
        }
        var expandedRows = parent.find('.hiddenRows input');

        var isSet = false;
        var index = -1;
        var expanded = expandedRows.val().split(_delimiter);
        for (var i = 0; i < expanded.length; i++) {
            if (expanded[i] == keyVal) {
                isSet = true;
                index = i;
                break;
            }
        }

        if (isExpanded != isSet) {
            if (isExpanded) {
                //add to expanded list
                var val = expandedRows.val();
                if (val.length > 0) {
                    val += _delimiter + keyVal;
                }
                else {
                    val = keyVal;
                }
                expandedRows.val(val);
            }
            else {
                //remove from expanded list
                var array = expandedRows.val().split(_delimiter);
                var val = '';
                for (var i = 0; i < array.length; i++) {
                    if (array[i] != keyVal) {
                        if (val.length > 0) {
                            val += _delimiter;
                        }
                        val += array[i];
                    }
                }
                expandedRows.val(val);
            }
        }
    }

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

<div id="CSTriggerUserEventContainer">
	<div class="hiddenRows" style="display:none;">
		<asp:HiddenField runat="server" ID="hdnExpandedRows" />
	</div>
	<h2 id="Title">
		<asp:Literal runat="server" ID="h2Title"></asp:Literal>
	</h2>
	<div class="DateRangeFilter">
		<asp:Panel ID="pnlDateTextBox" runat="server" Visible="false">
			<table cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td>
						<asp:Label ID="lblFromDate" runat="server" Text="" meta:resourcekey="lblFromDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dpFromDate" runat="server">
						</telerik:RadDatePicker>
					</td>
					<td>
						<asp:Label ID="lblToDate" runat="server" Text="" meta:resourcekey="lblToDate" />
					</td>
					<td>
						<telerik:RadDatePicker ID="dpToDate" runat="server">
						</telerik:RadDatePicker>
					</td>
					<td>
						<asp:LinkButton ID="btnSearch" runat="server" />
					</td>
				</tr>
				<tr>
					<td colspan="5">
						<asp:RequiredFieldValidator runat="server" ID="reqFromDate" ControlToValidate="dpFromDate"
							meta:resourcekey="reqFromDate" EnableClientScript="true" Display="Dynamic" CssClass="Validator"></asp:RequiredFieldValidator>
						<asp:RequiredFieldValidator runat="server" ID="reqToDate" ControlToValidate="dpToDate"
							meta:resourcekey="reqToDate" EnableClientScript="true" Display="Dynamic" CssClass="Validator"></asp:RequiredFieldValidator>
                        <asp:CustomValidator id="customToDateValidator" ControlToValidate="dpToDate"
                            ClientValidationFunction="ClientValidateFromToDates"
                            Display="Dynamic" CssClass="Validator" meta:resourcekey="customToDate" runat="server" />
					</td>
				</tr>
			</table>
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlDateRange" Visible="false">
			Select Date Range:
			<asp:DropDownList runat="server" ID="ddlDateRange" AutoPostBack="true">
			</asp:DropDownList>
		</asp:Panel>
	</div>
    <h3 id="H1">
		<asp:Literal runat="server" ID="lblEventsLabel"></asp:Literal>
	</h3>
	<asp:PlaceHolder ID="phCSTriggerUserEvent" runat="server" Visible="true" />
	<asp:Label runat="server" ID="lblNoResults" Visible="false"></asp:Label>    
    <div class="ActionButtons">
	   <asp:LinkButton ID="lnkPrintGrid" runat="server" meta:resourcekey="lnkPrintGrid"></asp:LinkButton>					
	</div>
</div>
</asp:Panel>
