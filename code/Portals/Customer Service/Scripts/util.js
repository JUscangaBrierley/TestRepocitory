$(document).ready(function () {
	$('.datepicker').datepicker({ constrainInput: true });
	ReadyListViews();
	HideEmptySidebars();
});

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

function showError(response, status, error) {
	location.href = 'error.html';
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

//client-side date range 
var rangeFilter = rangeFilter || {
	_initialized: false, 
	_callback: null, 
	init: function () {
		if (this._initialized) {
			return;
		}
		var btn = $('#TextRangeFilter a');
		var ddl = $('#DropdownRangeFilter select');

		if (btn && btn.length) {
			btn.prop('href', '#');
			btn.click(function (e) {
				e.preventDefault();
				e.stopPropagation();

				var start = $('#TextRangeFilter #RangeFrom input').val();
				var end = $('#TextRangeFilter #RangeTo input').val();

				rangeFilter._callback(rangeFilter.getRange());
			});
		}

		if (ddl && ddl.length) {
			ddl.change(function () {
				rangeFilter._callback(rangeFilter.getRange());
			});
		}
		this._initialized = true;
	},
	getRange: function () {
		var ret = {start: null, end: null};
		var btn = $('#TextRangeFilter a');		
		if (btn && btn.length) {
			ret.start = $('#TextRangeFilter #RangeFrom input').val();
			ret.end = $('#TextRangeFilter #RangeTo input').val();
		}
		else {
			var ddl = $('#DropdownRangeFilter select');
			if (ddl && ddl.length) {
				var range = ddl.val().split('|');
				ret.start = range[0];
				ret.end = range[1];
			}
		}
		return ret;
	},
	set onChange(callback) {
		if (!this._initialized) {
			this.init();
		}
		this._callback = callback;
	}
}


var messages = messages || {
	messageStatus: {Unread: 0, Read: 1, Deleted: 2}, 
	_autoRead: 0, 
	_resultsPerPage: null, 
	_currentMessageId: null,
	_readTimeout: null,
	_currentPage: 1,
	_totalPages: 0,
	_deleteConfirmation: null,
    _waiting: 0, 

	init: function(autoReadInterval, resultsPerPage, deleteConfirmation){
		this._autoRead = autoReadInterval;
		this._resultsPerPage = resultsPerPage;
		this._deleteConfirmation = deleteConfirmation;
		$('#MessageSorting input, #StatusFilter input').click(function () { messages.getMessages(); });
		$('#MessageDetail').hide();

		$(document).on('click', 'a.message', function (e) {
			var messageId = $(this).data('messageid');
			if (messageId) {
				$('#MessageDetail h3').html($(this).find('.message-description').html());
					
				var status = messages.messageStatus.Unread;
				if($(this).hasClass('deleted')) {
					status = messages.messageStatus.Deleted;
				}
				else if ($(this).hasClass('read')) {
					status = messages.messageStatus.Read;
				}

				switch(status) {
					case messages.messageStatus.Unread:
						if ($('#MessageDetail #btnDelete').length) {
							$('#MessageDetail #btnDelete').show();
						}
						$('#MessageDetail #btnMarkRead').show();
						$('#MessageDetail #btnMarkUnread').hide();
						$('#MessageDetail #btnUndelete').hide();
						break;
					case messages.messageStatus.Read:
						if ($('#MessageDetail #btnDelete').length) {
							$('#MessageDetail #btnDelete').show();
						}
						$('#MessageDetail #btnMarkRead').hide();
						$('#MessageDetail #btnMarkUnread').show();
						$('#MessageDetail #btnUndelete').hide();
						break;
					case messages.messageStatus.Deleted:
						if ($('#MessageDetail #btnDelete').length) {
							$('#MessageDetail #btnDelete').hide();
						}
						$('#MessageDetail #btnMarkRead').hide();
						$('#MessageDetail #btnMarkUnread').hide();
						$('#MessageDetail #btnUndelete').show();
						break;
				}
				messages._currentMessageId = messageId;
				messages.getMessage(status === messages.messageStatus.Unread);
			}
			e.preventDefault();
			e.stopPropagation();
		});
		$('#btnDone').click(function (e) {
			messages.stopTimeout();
			$('#MessageDetail').hide();
			$('#MessageList').show();
			e.preventDefault();
			e.stopPropagation();
		});
		if ($('#MessageDetail #btnDelete').length) {
			$('#btnDelete').click(function (e) {
				messages.stopTimeout();
				if (messages.setStatus(messages.messageStatus.Deleted)) {
					$('#MessageDetail').hide();
					$('#MessageList').show();
				}
				e.preventDefault();
				e.stopPropagation();
			});
		}
		$('#btnMarkRead').click(function(e){
			messages.stopTimeout();
			messages.setStatus(messages.messageStatus.Read);
			e.preventDefault();
			e.stopPropagation();
		});
		$('#btnMarkUnread').click(function(e){
			messages.stopTimeout();
			messages.setStatus(messages.messageStatus.Unread);
			e.preventDefault();
			e.stopPropagation();
		});
		$('#btnUndelete').click(function(e){
			messages.stopTimeout();
			messages.setStatus(messages.messageStatus.Read);
			e.preventDefault();
			e.stopPropagation();
		});
		$('#btnPrevious').click(function(e){
			if(messages._currentPage > 1) {
				messages._currentPage--;
				messages.getMessages();
			}
			e.preventDefault();
			e.stopPropagation();
		});
		$('#btnNext').click(function(e){
			if(messages._currentPage < messages._totalPages) {
				messages._currentPage++;
				messages.getMessages();
			}
			e.preventDefault();
			e.stopPropagation();
		});
	}, 

	statusString: function(status){
		switch(status){
			default:
			case this.messageStatus.Unread:
				return 'unread';
			case this.messageStatus.Read:
				return 'read';
			case this.messageStatus.Deleted:
				return 'deleted';
		}
	}, 

	stopTimeout: function() {
		if(this._readTimeout > 0) {
			window.clearTimeout(this._readTimeout);
			this._readTimeout = 0;
		}
	}, 
		
	getMessages: function () {
	    messages.Waiting = true;
		var range = rangeFilter.getRange();
		var order = $('#rdoOldest').is(':checked') ? 'oldest' : 'newest';
		var status = '';
		if($('#chkUnread').is(':checked')){
			status += '&status=Unread';
		}
		if($('#chkRead').is(':checked')){
			status += '&status=Read';
		}
		var deleted = $('#chkDeleted');
		if(deleted.length > 0 && deleted.is(':checked')){
			status += '&status=Deleted';
		}

		$.ajax({
			type: 'GET',
			url: 'api/Message?pageNumber=' + this._currentPage + 
				'&resultsPerPage=' + this._resultsPerPage + '&startDate=' + encodeURIComponent(range.start) + 
				'&endDate=' + encodeURIComponent(range.end) + status + '&order=' + order,
			success: function (msg) {
				$('#Messages').html('');

				for (var i = 0, len = msg.messages.length; i < len; i++) {
					var message = msg.messages[i];
					var a = $('<a href="#' + msg.messages[i].id + '" class="message ' + messages.statusString(message.status) + '" data-messageid="' + message.id + '" data-messagedefid="' + message.messageDefId + '">');
					var row = $('<div class="row">');
					row.append($('<div class="col-sm-10 col-xs-8 message-description">').html('<span class="icon"></span>' + message.description));
					row.append($('<div class="col-sm-2 col-xs-4 message-date">').html(message.dateIssued));
					a.append(row);
					$('#Messages').append(a).append($('<span class="message-separator">'));
				}
				if(msg.messages.length > 0){
					$('#lblNoMessages').hide();
				}
				else {
					$('#lblNoMessages').show();
				}

				messages._totalPages = msg.totalPages;
				$('#lblPage').html(messages._currentPage);
				$('#lblPages').html(msg.totalPages);

				if (messages._currentPage > 1 && messages._totalPages >= 1 && messages._totalPages < messages._currentPage) {
					messages._currentPage = 1;
					messages.getMessages();
				}
				messages.Waiting = false;
			},
			error: function (response, status, error) {
			    showError(response, status, error);
			    messages.Waiting = false;
			}
		});
	},

	getMessage: function (isUnread) {
	    messages.Waiting = true;
		$.ajax({
			type: "GET",
			url: "api/Message/" + this._currentMessageId,
			success: function (msg) {
				if (messages._autoRead && isUnread) {
				    messages._readTimeout = window.setTimeout(function () { messages.setStatus(messages.messageStatus.Read); }, messages._autoRead);
				    if (messages._autoRead < 1000) {
				        $('#MessageDetail #btnMarkRead').hide();
				    }
				}
				$('#MessageContent').html(msg);
				$('#MessageDetail').show();
				$('#MessageList').hide();
				messages.Waiting = false;
			},
			error: function (response, status, error) {
			    showError(response, status, error);
			    messages.Waiting = false;
			}
		});
	},

	setStatus: function(status) {
		if (status == this.messageStatus.Deleted && !window.confirm(this._deleteConfirmation)) {
			return false;
		}
		$.ajax({
			type: 'PUT',
			url: 'api/Message/' + this._currentMessageId,
			data: { status: status },
			success: function (msg) {
				//update our message list
				if (status === messages.messageStatus.Read) {
				    if (!$('#btnMarkUnread').is(':visible')) {
						//unread, marked read
					    $('#btnMarkRead').fadeOut(400, function () { $('#btnMarkUnread').fadeIn(); });
					}
					else {
						//deleted, undeleted
						if ($('#MessageDetail #btnDelete').length) {
							$('#MessageDetail #btnDelete').show();
						}
						$('#MessageDetail #btnMarkRead').show();
						$('#MessageDetail #btnMarkUnread').hide();
						$('#MessageDetail #btnUndelete').hide();
					}
				}
				else if (status === messages.messageStatus.Unread && $('#btnMarkUnread').is(':visible')) {
				    $('#btnMarkUnread').fadeOut(400, function () { $('#btnMarkRead').fadeIn(); });
				}
				messages.getMessages();
			},
			error: function (response, status, error) {
				showError(response, status, error);
			}
		});
		return true;
	},

	get Waiting() {
	    return !!this._waiting;
	},

	set Waiting(waiting) {
	    if (waiting) {
	        this._waiting = setTimeout(function () { $('#MessagesContainer .wait-animation').show(); }, 500);
	    }
	    else {
	        if (this._waiting) {
	            clearTimeout(this._waiting);
	        }
	        $('#MessagesContainer .wait-animation').hide();
        }
	},
}