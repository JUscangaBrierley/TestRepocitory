<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewLogin.ascx.cs" Inherits="Brierley.LWModules.Login.ViewLogin" %>
<script type="text/javascript">
    $(document).ready(function () {
        $('#hfSLJoin').val('false');
        $('#hfSLIdentity').val('');
        $('#hfSLPassword').val('');
    });

    function OnIsAMemberClick() {
        $('#pnlLinkSocialAccount').dialog({height: 300});
        $('#pnlLinkToMember').show();
        $('#pnlGetRegistered').hide();

        if (!$('#btnYes').hasClass('link-social-button-selected')) {
            $('#btnYes').addClass('link-social-button-selected');
        }
        if ($('#btnNo').hasClass('link-social-button-selected')) {
            $('#btnNo').removeClass('link-social-button-selected');
        }
    }

    function OnNotAMemberClick() {
        $('#pnlLinkSocialAccount').dialog({ height: 300 });
        $('#pnlLinkToMember').hide();
        $('#pnlGetRegistered').show();

        if (!$('#btnNo').hasClass('link-social-button-selected')) {
            $('#btnNo').addClass('link-social-button-selected');
        }
        if ($('#btnYes').hasClass('link-social-button-selected')) {
            $('#btnYes').removeClass('link-social-button-selected');
        }
    }

    function DoLinkAndLogin() {
        $('#hfSLJoin').val('false');
        $('#hfSLIdentity').val($('#txtSLIdentity').val());
        $('#hfSLPassword').val($('#txtSLPassword').val());
        $('#pnlLinkSocialAccount').dialog("close");
        document.forms[0].submit();
    }

    function DoJoin() {
        $('#hfSLJoin').val('true');
        $('#hfSLIdentity').val('');
        $('#hfSLPassword').val('');
        $('#pnlLinkSocialAccount').dialog("close");
        document.forms[0].submit();
    }

    function ShowLinkSocialAccountDialog(socUserName, socProfileImageURL, identityLabel) {
        $('.pnlLinkSocialAccount').dialog({
            width: 450,
            height: 160,
            modal: true
        });
        $('#hfSLIdentity').val('');
        $('#hfSLPassword').val('');
        if (socUserName != '') {
            $('#spnSocUserName').html(' ' + socUserName);
        }
        if (socProfileImageURL != '') {
            $('#profileImage').attr('src', socProfileImageURL);
            $('#profileImage').show();
        } else {
            $('#profileImage').hide();
        }
        $('#spnIdentity').html(identityLabel);
    }
</script>
<input type="hidden" id="hfSLJoin" name="hfSLJoin" value="false" />
<input type="hidden" id="hfSLIdentity" name="hfSLIdentity" value="" />
<input type="hidden" id="hfSLPassword" name="hfSLPassword" value="" />

<div id="pnlLinkSocialAccount" class="pnlLinkSocialAccount" runat="server" style="display:none">
    <h2><asp:Literal id="litLinkSocialHeader" runat="server" /></h2>
    <table>
        <tr>
            <td style="vertical-align: top; padding-right: 5px;">
                <img id="profileImage" src="" width="50" height="50" />
            </td>
            <td style="vertical-align: top">
                <h3><asp:Literal id="litSocialWelcome" runat="server" /><span id="spnSocUserName"></span>!</h3>
                <p><asp:Literal id="litSocialQuestion" runat="server" /></p>
                <a id="btnYes" title="Yes" class="link-social-button" role="button" href="#" onclick="OnIsAMemberClick();return false;" ><asp:Literal id="litBtnYes" runat="server" /></a>
                <a id="btnNo" title="No" class="link-social-button" role="button" href="#" onclick="OnNotAMemberClick();return false;" ><asp:Literal id="litBtnNo" runat="server" /></a>
            </td>
        </tr>
    </table>
        <hr />
    <div id="pnlLinkToMember" style="display:none">
        <h5><asp:Literal id="litLinkMemberHeader" runat="server" /></h5>
        <p><asp:Literal id="litLinkMemberContent" runat="server" /></p>
        <table>
            <tr>
                <td><span id="spnIdentity"></span>:</td>
                <td><input id="txtSLIdentity" value="" /></td>
            </tr>
            <tr>
                <td><asp:Literal id="litPassword_SL" runat="server" /></td>
                <td><input id="txtSLPassword" type="Password" value="" /></td>
            </tr>
        </table>
        <a id="btnLinkAndLogin" Title="Link Accounts and Login" class="link-social-button" OnClick="DoLinkAndLogin();"><asp:Literal id="litLinkLogin" runat="server" /></a>
    </div>
    <div id="pnlGetRegistered" style="display:none">
        <h5><asp:Literal id="litRegSocialHeader" runat="server" /></h5>
        <p><asp:Literal id="litRegSocialContent" runat="server" /></p>
        <a id="btnJoin" title="Sign up" class="link-social-button" role="button" OnClick="DoJoin();"><asp:Literal id="litbtnJoin" runat="server" /></a>
    </div>
</div>