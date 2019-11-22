<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewMemberProfile.ascx.cs" Inherits="Brierley.LWModules.MemberProfile.ViewMemberProfile" %>
<div id="MemberProfileContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>

<script type="text/javascript">

	var _confirmationPrompts;
	var _originalStatus;
	var _accountStatusClientId; 
	var _accountStatusDisplayType;
	var _valueCount;

	var _parentLists = null;
	var _relations = null;

	$(document).ready(function () {
		BindParentEvents();
	});


	function BindParentEvents() {
		if (_parentLists == null) {
			return;
		}
		for (var i = 0; i < _parentLists.length; i++) {
			var list = $('#' + _parentLists[i]);
			if (list != null && list.length > 0) {
				list.bind('change', function () { ParentChanged($(this)); });
				ParentChanged(list);
			}
		}
	}

	function ParentChanged(parent) {
		if (parent != null && parent.length > 0) {
			var parentId = $(parent).attr('id');
			for (var i = 0; i < _relations.length; i++) {
				if (_relations[i].parentId == parentId) {
					var relation = _relations[i];
					var children = relation.childValues;
					var child = $('#' + relation.childId);
					var currentVal = child.val();
					var parentVal = $(parent).val();
					var hasValue = false;

					$('#' + relation.childId + ' > option').remove();

					for (var i = 0; i < children.length; i++) {
						if (children[i].parentKey == parentVal || children[i].parentKey == '') {
							child.append($('<option></option').val(children[i].key).html(children[i].text));
							if (children[i].key == currentVal) {
								hasValue = true;
							}
						}
					}
					child.val(hasValue ? currentVal : '');
					if ($.inArray(child.attr('id'), _parentLists)) {
						ParentChanged(child);
					}
				}
			}
		}
	}


	function Relation(parentId, childId, childValues) {
		this.parentId = parentId;
		this.childId = childId;
		this.childValues = childValues;
		return true;
	}

	function Item(parentKey, key, text) {
		this.parentKey = parentKey;
		this.key = key;
		this.text = text;
	}
	
	function ConfirmStatusChange() {
		if(_confirmationPrompts == null || _originalStatus == null || _accountStatusClientId == null || _accountStatusDisplayType == null) {
			return true;
		}
		else {
			var newAccountStatus;
			var confirmationPrompt;
			
			if(_accountStatusDisplayType == 'dropdown') {
				var control = document.getElementById(_accountStatusClientId);
				newAccountStatus = control.value;
				for(var i=0; i<_confirmationPrompts.length; i++) {
					if(_confirmationPrompts[i][0] == newAccountStatus) {
						confirmationPrompt = _confirmationPrompts[i][1];
					}
				}
			}
			else {
				for(var i=0; i<_valueCount; i++) {
					var radioControl = document.getElementById(_accountStatusClientId + '_' + i);
					if(radioControl != null) {
						if(radioControl.checked) {
							newAccountStatus = radioControl.value;
							confirmationPrompt = _confirmationPrompts[i][1];
							break;
						}
					}
				}
			}
			if(newAccountStatus != null && newAccountStatus != _originalStatus && confirmationPrompt != null) {
				return window.confirm(confirmationPrompt);
			}
			return true;
		}
	}

</script>

<asp:Label runat="server" ID="lblError" CssClass="MemberProfileErrorLabel"></asp:Label>


<asp:PlaceHolder runat="server" ID="plcContent"></asp:PlaceHolder>


<asp:Panel runat="server" ID="pnlHidden" Visible="false">

	<asp:Button runat="server" ID="btnReset" OnClientClick="document.forms[0].reset();return false;"/>
	<asp:Button runat="server" ID="btnSubmitOne"/>
	<asp:Button runat="server" ID="btnSubmitTwo" />
    <asp:Button runat="server" ID="btnCancel" CausesValidation="false" />
	
	<asp:LinkButton runat="server" ID="lnkReset" OnClientClick="document.forms[0].reset();return false;"></asp:LinkButton>
	<asp:LinkButton runat="server" ID="lnkSubmitOne"></asp:LinkButton>
	<asp:LinkButton runat="server" ID="lnkSubmitTwo"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkCancel" CausesValidation="false"></asp:LinkButton>

</asp:Panel>


</div>