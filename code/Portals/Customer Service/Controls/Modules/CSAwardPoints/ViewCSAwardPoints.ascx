<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCSAwardPoints.ascx.cs" Inherits="Brierley.LWModules.CSAwardPoints.ViewCSAwardPoints" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:PlaceHolder runat="server">
<script type="text/javascript">

	var <%=this.ClientID%>_pointTypes = null;
	var <%=this.ClientID%>_hdnSelectedPointType = '#<%=hdnSelectedPointType.ClientID %>';
	var <%=this.ClientID%>_hdnSelectedPointEvent = '#<%=hdnSelectedPointEvent.ClientID %>';

	$(document).ready(function () {
		if (<%=this.ClientID%>_pointTypes != null) {
			var pointType = $('#PointType');
			var pointEvent = $('#PointEvent');

			if (pointType.length > 0) {

				if (<%=this.ClientID%>_pointTypes.length == 1) {
					$('#PointTypeSingle').html(<%=this.ClientID%>_pointTypes[0].name);
					$('#PointTypeSingle').show();
				}
				else {
					$('#ddlPointType').bind('change', function () { <%=this.ClientID%>_PointTypeChanged(); });

					$('#PointTypeSelect').show();
					for (var i = 0; i < <%=this.ClientID%>_pointTypes.length; i++) {
						$('#ddlPointType').append($('<option></option').val(<%=this.ClientID%>_pointTypes[i].id).html(<%=this.ClientID%>_pointTypes[i].name));
					}
				}
				if ($(<%=this.ClientID%>_hdnSelectedPointType).val() == '') {
					$(<%=this.ClientID%>_hdnSelectedPointType).val($('#ddlPointType').val());
				}
				else {
					$('#ddlPointType').val($(<%=this.ClientID%>_hdnSelectedPointType).val());
				}
				<%=this.ClientID%>_ShowPointEvents($(<%=this.ClientID%>_hdnSelectedPointType).val());
				if ($(<%=this.ClientID%>_hdnSelectedPointEvent).val() != '') {
					$('#ddlPointEvent').val($(<%=this.ClientID%>_hdnSelectedPointEvent).val());
				}
				else {
					$(<%=this.ClientID%>_hdnSelectedPointEvent).val($('#ddlPointEvent').val());
				}
			}
			else {
				$('#PointTypeSelect').hide();
				<%=this.ClientID%>_ShowPointEvents($(<%=this.ClientID%>_hdnSelectedPointType).val());
				if (pointEvent.length > 0) {
					<%=this.ClientID%>_PointEventChanged();
				}
			}
		}

		if (pointEvent.length > 0) {
			$('#ddlPointEvent').bind('change', function () { <%=this.ClientID%>_PointEventChanged(); });
		}
	});


	function <%=this.ClientID%>_PointTypeChanged() {
		$(<%=this.ClientID%>_hdnSelectedPointType).val($('#ddlPointType').val());
		<%=this.ClientID%>_ShowPointEvents($('#ddlPointType').val());
	}

	function <%=this.ClientID%>_PointEventChanged() {
		$(<%=this.ClientID%>_hdnSelectedPointEvent).val($('#ddlPointEvent').val());
	}


	function <%=this.ClientID%>_ShowPointEvents(pointTypeId) {
		$('#ddlPointEvent > option').remove();
		for (var i = 0; i < <%=this.ClientID%>_pointTypes.length; i++) {
			if (<%=this.ClientID%>_pointTypes[i].id == pointTypeId) {
				if (<%=this.ClientID%>_pointTypes[i].pointEvents.length > 1) {
					for (var ii = 0; ii < <%=this.ClientID%>_pointTypes[i].pointEvents.length; ii++) {
						$('#ddlPointEvent').append($('<option></option').val(<%=this.ClientID%>_pointTypes[i].pointEvents[ii].id).html(<%=this.ClientID%>_pointTypes[i].pointEvents[ii].name));
					}
					$('#PointEventSingle').hide();
					$('#ddlPointEvent').show();
				}
				else {
					$('#PointEventSingle').html(<%=this.ClientID%>_pointTypes[i].pointEvents[0].name);
					$(<%=this.ClientID%>_hdnSelectedPointEvent).val(<%=this.ClientID%>_pointTypes[i].pointEvents[0].id);
					$('#ddlPointEvent').hide();
					$('#PointEventSingle').show();
				}
				break;
			}
		}
		$('#PointEventSelect').show();
	}

//	var <%=this.ClientID%>_pointTypes = [
//		new <%=this.ClientID%>_PointType(1, 'Some Point Type', [ new <%=this.ClientID%>_PointEvent(1, 'some event') ])
//	];


	function <%=this.ClientID%>_PointType(id, name, pointEvents) {
		this.id = id;
		this.name = name;
		this.pointEvents = pointEvents
		return true;
	}


	function <%=this.ClientID%>_PointEvent(id, name) {
		this.id = id;
		this.name = name;
		return true;
	}
		
</script>
</asp:PlaceHolder>

<div id="AwardPointsContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>

	<asp:PlaceHolder runat="server" ID="pchSuccessMessage" Visible="false">
		<div class="Positive">
			<asp:Literal runat="server" ID="litSuccessMessage"></asp:Literal>
		</div>
	</asp:PlaceHolder>

	<asp:PlaceHolder runat="server" ID="pchPointType">
		<div id="PointType">
			<span id="PointTypeLabel">
				<asp:Label runat="server" ID="lblPointType" meta:resourcekey="lblPointType"></asp:Label>
			</span>

			<span id="PointTypeSelect" style="display:none;">
				<select id="ddlPointType"></select>
			</span>

			<span id="PointTypeSingle" style="display:none;"></span>

		</div>
	</asp:PlaceHolder>


	<asp:PlaceHolder runat="server" ID="pchPointEvent">
		<div id="PointEvent">
			<span id="PointEventLabel">
				<asp:Label runat="server" ID="lblPointEvent" meta:resourcekey="lblPointEvent"></asp:Label>
			</span>

			<span id="PointEventSelect" style="display:none;">
				<select id="ddlPointEvent"></select>
			</span>

			<span id="PointEventSingle" style="display:none;"></span>

		</div>
	</asp:PlaceHolder>


	<asp:PlaceHolder runat="server" ID="pchPointAwardDate">
		<div id="PointTxnDate">
			<span id="PointTxnDateLabel">
				<asp:Label runat="server" ID="lblPointTxnDate" meta:resourcekey="lblPointTxnDate"></asp:Label>
			</span>

			<telerik:RadDatePicker ID="rdpPointTxnDate" runat="server">
			</telerik:RadDatePicker>

			<asp:RequiredFieldValidator ID="rqTxnDate" runat="server" Display="Dynamic" 
				ControlToValidate="rdpPointTxnDate" meta:resourcekey="reqTxnDate" CssClass="Validator">
			</asp:RequiredFieldValidator>

		</div>
	</asp:PlaceHolder>


	<div id="Points">
		<span id="PointsLabel">
			<asp:Label runat="server" ID="lblPoints" meta:resourcekey="lblPoints"></asp:Label>
		</span>

		<asp:TextBox ID="txtPoints" runat="server" MaxLength="10"></asp:TextBox>

		<asp:RequiredFieldValidator ID="reqPoints" runat="server" 
			ControlToValidate="txtPoints" meta:resourcekey="reqPoints" Display="Dynamic" EnableClientScript="true" CssClass="Validator">
		</asp:RequiredFieldValidator>

		<asp:RangeValidator ID="vldPoints" runat="server" 
			ControlToValidate="txtPoints" Display="Dynamic" meta:resourcekey="vldPoints" 
			Type="Integer" MinimumValue="0" MaximumValue="2147483647" CssClass="Validator">
		</asp:RangeValidator>
	</div>


	<asp:PlaceHolder runat="server" ID="pchAdditionalNotes">
		<div id="AdditionalNotes">
			<span id="AdditionalNotesLabel">
				<asp:Label runat="server" ID="lblAdditionalNotes" meta:resourcekey="lblAdditionalNotes"></asp:Label>
			</span>

			<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine"></asp:TextBox>
		</div>
	</asp:PlaceHolder>


	<div class="ActionButtons">

		<asp:LinkButton ID="btnSave" runat="server" meta:resourcekey="btnSave" />
		<asp:LinkButton ID="btnCancel" runat="server" meta:resourcekey="btnCancel" CausesValidation="false" />

	</div>

	<asp:HiddenField runat="server" ID="hdnSelectedPointType" />
	<asp:HiddenField runat="server" ID="hdnSelectedPointEvent" />

</div>