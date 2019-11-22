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
        $('#pnlLinkSocialAccount').dialog({
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

<div id="pnlLinkSocialAccount" title="Link Your Social Account to Your Loyalty Account" style="display:none">
    <h2>Accessing Client.com</h2>
    <table>
        <tr>
            <td style="vertical-align: top; padding-right: 5px;">
                <img id="profileImage" src="" width="50" height="50" />
            </td>
            <td style="vertical-align: top">
                <h3>Welcome<span id="spnSocUserName"></span>!</h3>
                <p>Do you have an existing loyalty account?</p>
                <a id="btnYes" title="Yes" class="link-social-button" role="button" href="#" onclick="OnIsAMemberClick();return false;">Yes</a>
                <a id="btnNo" title="No" class="link-social-button" role="button" href="#" onclick="OnNotAMemberClick();return false;">No</a>
            </td>
        </tr>
    </table>
        <hr />
    <div id="pnlLinkToMember" style="display:none">
        <h5>Great! Thanks for being a loyalty member.</h5>
        <p>For quick secure access, please link your loyalty account.</p>
        <table>
            <tr>
                <td><span id="spnIdentity"></span>:</td>
                <td><input id="txtSLIdentity" value="" /></td>
            </tr>
            <tr>
                <td>Password:</td>
                <td><input id="txtSLPassword" type="Password" value="" /></td>
            </tr>
        </table>
        <a id="btnLinkAndLogin" Title="Link Accounts and Login" class="link-social-button" OnClick="DoLinkAndLogin();">Link Accounts and Login</a>
    </div>
    <div id="pnlGetRegistered" style="display:none">
        <h5>Not a Loyalty Member? Let's get you registered.</h5>
        <p>Click <em>Join</em> to register as a loyalty member.  Once you become a loyalty member, 
            you can start accruing points immediately and be on your way to earning the points
            necessary for added rewards.
        </p>
        <a id="btnJoin" title="Sign up" class="link-social-button" role="button" OnClick="DoJoin();">Join</a>
    </div>
</div>