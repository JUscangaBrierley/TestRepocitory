<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRewardChoice.ascx.cs" Inherits="LoyaltyWareWebsite.Controls.Modules.RewardChoice.ViewRewardChoice" %>

<div id="RewardChoiceContainer">
	<div id="lblModuleUnavailable" class="error">
		<span>
			<asp:Literal runat="server" ID="litModuleUnavailable" meta:resourcekey="ModuleUnavailable"></asp:Literal>
		</span>
	</div>

	<div id="lblSuccess" class="positive">
		<span>
			<asp:Literal runat="server" ID="litSuccess" meta:resourcekey="lblSuccess"></asp:Literal>
		</span>
	</div>
	
	<asp:PlaceHolder runat="server" ID="pchRewardList"></asp:PlaceHolder>

	<div id="SelectedReward" style="display:none;">
		<h2>
            <asp:Literal runat="server" meta:resourcekey="YouHaveChosen"></asp:Literal>
		</h2>
		<img />
		<h3 id="DisplayName"></h3>
		<p id="LongDescription"></p>
		<aside>
			<h4>
				<asp:Literal runat="server" meta:resourcekey="EarnedEvery"></asp:Literal> 
			</h4>
			<div id="RewardProgress">
				<div></div>
			</div>
			<div id="CurrentBalance">
				<p><%= PointBalance %> Points</p>
			</div>
		</aside>
		<sub id="LegalText"></sub>

		<div id="form-menu">
			<asp:LinkButton runat="server" ID="btnSave" CausesValidation="false" CssClass="btn" meta:resourcekey="btnSave"></asp:LinkButton>
		</div>
	</div>




<script type="text/javascript">

	$(document).ready(function () {

		var pointBalance = <%= PointBalance %>;
		var pointsToEarnText = $('#SelectedReward aside h4').html();

		var setSelection = function (e) {
			if (e) {
				e.find('input').prop('checked', true);
			}
			$('#RewardChoiceContainer .reward').removeClass('selected');

			var radio = $('#RewardChoiceContainer .reward input:checked');
			if(radio && radio.length){
			    var checked = radio.parent();
			    checked.addClass('selected');

			    $('#DisplayName').html(checked.data('display-name'));
			    $('#LongDescription').html(checked.data('long-description'));
			    $('#LegalText').html(checked.data('legal-text'));
			    $('#SelectedReward img').prop('src', checked.data('image'));

			    var pointsToEarn = parseInt(checked.data('points'));
			    var width = Math.max(0, Math.min(100, pointBalance / pointsToEarn * 100)).toFixed();

			    $('#SelectedReward aside h4').html(pointsToEarnText.replace('{0}', pointsToEarn).replace('{1}', pointBalance) );
			    $('#RewardProgress div').css('width', width.toString() + '%');

			    $('#CurrentBalance p').css('margin-right', (100 - width).toString() + '%');

			    if(!$('#SelectedReward').is(':visible')) {
			        $('#SelectedReward').show();
			    }
			}
			else {
			    $('#SelectedReward').hide();
			}
		};

		$('#RewardChoiceContainer .reward').click(function () {
			setSelection($(this));
		});
		setSelection();
	});
</script>
