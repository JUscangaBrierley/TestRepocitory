<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewEventProcessing.ascx.cs" Inherits="Brierley.LWModules.EventProcessing.ViewEventProcessing" %>
<asp:PlaceHolder runat="server">
<script type="text/javascript">
	function TriggerEvent(eventName, context, success, error) {
		$.ajax({
			type: "POST",
			url: "<%=Page.Request.RawUrl %>",
			data: { eventName: eventName, context: context },
			success: success,
			dataType: "xml",
			error: error
		});
	}
</script>
</asp:PlaceHolder>