<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCSDataHook.ascx.cs" Inherits="Brierley.LWModules.CSDataHook.ViewCSDataHook" %>

<asp:PlaceHolder runat="server" ID="js">

	<style type="text/css">
		.DataHooked
		{
			background-color: #ccffcc;
		}
	</style>

	<script type="text/javascript">
		var _hooks = null;

		<%if(!Page.IsPostBack || AllowPostbacks){ %>
		$(document).ready(function () { ProcessHooks(); });
		<%} %>

		function ProcessHooks() {
			if (_hooks != null) {
				var labels = $('label, span, td');
				for (var iHook = 0; iHook < _hooks.length; iHook++) {
					for (var iLbl = 0; iLbl < labels.length; iLbl++) {
						if ($(labels[iLbl]).html().trim() == _hooks[iHook][0]) {
							PopulateSibling($(labels[iLbl]), _hooks[iHook][1]);
							break;
						}
					}
				}
			}
		}

		function PopulateSibling(label, value) {
			var findString = 'input, select';
			var lblFor = label.attr('for');
			if (lblFor != null && lblFor.length > 0) {
				var input = $('#' + lblFor);
			}

			if (input == null || input.length == 0) {
				var children = label.next().find(findString);
				if (children.length == 0) {
					var parent = $(label).parent();
					var td = null;
					if (parent[0].nodeName.toLowerCase() == 'td') {
						td = parent;
						parent = parent.next();
					}
					children = parent.find(findString);
					if (children.length == 0) {
						parent = parent.next();
						children = parent.find(findString);
						if (children.length == 0 && td != null) {
							var colIndex = td.index();
							var tr = td;
							while (tr.length > 0 && tr[0].nodeName.toLowerCase() != 'tr') {
								tr = tr.parent();
							}
							if (tr.length > 0) {
								var nextRow = tr.next();
								if (nextRow.length >= colIndex) {
									var td = $(nextRow.children()[colIndex]);
									children = td.find(findString);
								}
							}
						}
					}
				}
				if (children.length > 0) {
					var input = children.first();
				}
			}

			if (input != null && input.length > 0) {
			    if (input.attr('type') && input.attr('type').toLowerCase() == 'checkbox') {
					input.attr('checked', value).addClass('DataHooked');
				}
				else {
					input.val(value).addClass('DataHooked');
				}
			}
		}

	</script>

</asp:PlaceHolder>