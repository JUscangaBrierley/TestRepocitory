$(document).ready(function () { ReadyListViews(); HideEmptySidebars(); });

function ShowModal(divID) {
	$('#' + divID).dialog({
		width: 700,
		modal: true,
		buttons: {
			OK: function () {
				$(this).dialog("close");
			}
		}
	});
}

//printing is done using media attributes in CSS, which is of course limited to CSS selectors for styling. In the wallet screen in particular, when 
//the user attempts to print a single item, we want to be able to hide the other lists. This isn't possible with CSS alone, because there is no
//selector that will determine that we have a list in detail view and others in list view, so we'll use some javascript to make that determination, 
//and hide the lists if something of higher priority should be shown.
function ReadyListViews() {
	if ($('.DynamicView').length > 0) {
		$('div[id^=DynamicList_]').each(function (index, value) {
			$(value).parentsUntil($('div[id$=Container]')).hide();
		});
	}
}


function HideEmptySidebars() {
	$('#earnings:empty').hide();
	$('#benefits:empty').hide();
}

function showSampleReceiptDialog() {
	ShowModal('requestCreditSampleReceiptDialog');
}
