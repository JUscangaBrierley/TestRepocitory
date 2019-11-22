<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewCSNotes.ascx.cs" Inherits="Brierley.LWModules.CSNotes.ViewCSNotes" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="cc1" Namespace="Brierley.WebFrameWork.Controls" Assembly="Brierley.WebFrameWork" %>

<script type="text/javascript">
	function ValidateNoteLength(sender, args) {
	    if (args.Value.length > 512) {
	        args.IsValid = false;
	        sender.errormessage = sender.innerHTML = 'Note cannot exceed 512 characters (currently ' + args.Value.length + ').';
	    }
	    else if (WordTooLong(args.Value)) {
	        args.IsValid = false;
	        sender.errormessage = sender.innerHTML = 'Note cannot contain words that exceed 45 characters.';
	    }
	    else {
	        args.IsValid = true;
	    }
    }
    function WordTooLong(s) {
        if (s != null && s.length > 0) {
            var words = s.split(' ');
            for (i=0; i<words.length; i++) {
                if (words[i].length > 45)
                    return true;
            }
        }
        return false;
    }
</script>

<div id="CSNotesContainer">
	<h2 id="Title">
		Customer Service Notes
	</h2>

	<asp:PlaceHolder ID="phCSNotes" runat="server" Visible="true" />

</div>
