<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewBonuses.ascx.cs" Inherits="Brierley.LWModules.Bonuses.ViewBonuses" %>
<%@ Register Assembly="Brierley.WebFrameWork" Namespace="Brierley.WebFrameWork.Controls" TagPrefix="WebFramework" %>
<%@ Import Namespace="Brierley.WebFrameWork.Portal" %>

<script type="text/javascript" src="JWPlayer/jwplayer.js"></script>

<asp:PlaceHolder runat="server">
<script type="text/javascript">
	var _id = -1;
	var _sharedVideoUrl = '';
	var _sharedVideoTitle = '';
	var _listenerInterval = null;
	var _currentBonus = null;
	var _calling = false;
	var _nextStep = '#<%=lnkButtonNextStep.ClientID %>';
	var _nextStepHref = null;
	var _scrollSpeed = 300;
	var _move = 0;
	var _heroIsFiller = false;

	$(document).ready(function () {
		var list = $('#BonusList');
		var hero = $('#HeroBonus');
		if (list.length > 0 && hero.length > 0 && hero.is(':visible')) {
			var bonus = $('#BonusList').children('.Bonus').first();
			if (bonus.length > 0) {
				_move = parseFloat(bonus.width());
				_move += parseFloat(bonus.css('margin-left').replace('px', ''));
				_move += parseFloat(bonus.css('margin-right').replace('px', ''));
				_move = Math.round(_move);

				//hack for phantom webkit margins
				if (navigator.userAgent.toLowerCase().indexOf('chrome') > 0) {
					var compensation = navigator.appVersion < 25 ? '-3px' : '11px';
					$('#BonusList').children('.Bonus').css('margin-right', compensation);
				}
				else if (navigator.userAgent.toLowerCase().indexOf('safari') > 0) {
					var compensation = '-3px';
					$('#BonusList').children('.Bonus').css('margin-right', compensation);
				}


			}

			$('#BonusScrollLeft').click(function () { ScrollLeft(); });
			$('#BonusScrollRight').click(function () { ScrollRight(); });
			$('.Bonus').mouseenter(function () { BonusMouseEnter($(this)); });
			$('.Bonus').mouseleave(function () { BonusMouseLeave($(this)); });
			$('#HeroImage').mouseenter(function () { HeroImageMouseEnter(); });
			$('#HeroImage').mouseleave(function () { HeroImageMouseLeave(); });
			ShowDefaultHero();

			if ($('#BonusList').children('.Bonus').length == 0) {
				//no more bonuses in the list
				$('#BonusScrollLeft, #BonusList, #BonusScrollRight').hide();
			}
		}

		var video = $('#<%=hdnVideoUrl.ClientID %>').val();
		if (typeof (video) != 'undefined' && video != '') {
			BonusAction(video);
		}
	});

	function BonusMouseEnter(bonus) {
		$(bonus).addClass('BonusHover');
	}

	function BonusMouseLeave(bonus) {
		$(bonus).removeClass('BonusHover');
	}

	function HeroImageMouseEnter() {
		if (!_heroIsFiller) {
			$('#HeroImage').addClass('HeroImageHover');
		}
	}

	function HeroImageMouseLeave() {
		if (!_heroIsFiller) {
			$('#HeroImage').removeClass('HeroImageHover');
		}
	}

	function ScrollLeft() {
		if ($('#BonusList').children('.Bonus').length == 0) {
			return;
		}
		var currentLeft = $('#BonusList').scrollLeft();
		$('#BonusList').animate(
			{ scrollLeft: currentLeft - _move },
			_scrollSpeed,
			function () {
				ToNearestOffer();
			}
		);
	}

	function ScrollRight() {
		if ($('#BonusList').children('.Bonus').length == 0) {
			return;
		}
		var currentLeft = $('#BonusList').scrollLeft();
		$('#BonusList').animate(
			{ scrollLeft: currentLeft + _move },
			_scrollSpeed,
			function () {
				ToNearestOffer();
			}
		);
	}

	function ToNearestOffer() {
		var currentLeft = parseFloat($('#BonusList').scrollLeft());
		var offset = currentLeft % _move;
		if (offset != 0) {
			if (offset > _move / 2) {
				$('#BonusList').scrollLeft(currentLeft - (currentLeft % _move) + _move);
			}
			else {
				$('#BonusList').scrollLeft(currentLeft - (currentLeft % _move));
			}
		}
	}

	function ShowDefaultHero() {
		var first = $('.Bonus').first();
		if (first.length > 0) {
			ShowHero(first);
			//remove item from carousel
			first.remove();
		}
		else {
			ShowHero(null);
		}
	}

	function ShowHero(bonus) {
		if (bonus != null) {
			var getHref = $(bonus).children('.ListGet').attr('href');
			$('#HeroBonus .heroButton').attr('href', getHref);
			$('#HeroImage #HeroPoints').attr('href', getHref);
			$('#HeroImage #HeroImageLink').attr('href', getHref);
			var removeHref = $(bonus).children('.ListRemove').attr('href');
			var removeTitle = $(bonus).children('.ListRemove').attr('title');
			$('#HeroRemove').attr('href', removeHref);
			$('#HeroRemove').attr('title', removeTitle);

			if ($(bonus).data('gobutton') != '' && $(bonus).data('action') != 'Survey') {
				$('#HeroBonus #lnkCustom').show().html($(bonus).data('gobutton'));
			}
			else {
				switch ($(bonus).data('action')) {
					case 'Video':
						$('#HeroBonus #lnkVideo').show();
						break;
					case 'Survey':
						$('#HeroBonus #lnkSurvey').show();
						break;
					case 'Html':
					default:
						$('#HeroBonus #lnkHtml').show();
						break;
				}
			}

			$('#HeroHeadline').html($(bonus).find('.Headline>span').html());
			$('#HeroDescription').html($(bonus).find('.BonusDesc>span').html());
			$('#HeroImage img').attr('src', $(bonus).data('logoimagehero'));
			var points = $(bonus).find('.BonusPoints span span');
			if (points.length > 0) {
				$('#HeroPoints span').html(points.html());
			}
			else {
				$('#HeroPoints').hide();
				//	$('#HeroPoints span').html('');
			}

			var id = parseInt($(bonus).data('bonusid'));
			if (id < 1) {
				_heroIsFiller = true;
			}
		}
		else {
			$('#HeroBonus').hide();
		}
	}

	function BonusAction(url) {
		try {
			_nextStepHref = $(_nextStep).attr('href');
			$(_nextStep).attr('href', '#').addClass('disabled');

			_sharedVideoUrl = url;
			_sharedVideoTitle = '';

			var videoSource = url;
			// 480*720  / 1.3
			var height = 400;
			var width = 600;

			if (videoSource.indexOf(',') > 0) {
				//video url has dimensions
				var arr = videoSource.split(',');
				if (arr.length == 3) {
					videoSource = arr[0];
					width = parseInt(arr[1]);
					height = parseInt(arr[2]);
				}
			}

			$('#VideoContainer').show();
			$('#VideoComplete').css('height', height).css('width', width);
			$('#VideoContainer').css('height', height).css('width', width);

			jwplayer("JWPlayer").setup({
				flashplayer: "JWPlayer/player.swf",
				file: videoSource,
				autoplay: true,
				height: height,
				width: width,
				controlbar: '<%=ControlBar %>',
				events: {
					onComplete: function () { <%= PauseVideoAtEnd ? "VideoComplete();" : "ActionComplete();" %> }
				}
			});
		}
		catch (err) {
			if (typeof (CustomErrorAlert) != 'undefined') {
				CustomErrorAlert(err);
			}
		}
		return false;
	}

	function RestartVideo() {
		$('#VideoComplete').fadeOut(500);
		jwplayer('JWPlayer').play();
	}

	function VideoComplete() {
		$('#VideoComplete').fadeIn(500);
		$(_nextStep).attr('href', _nextStepHref).removeClass('disabled');
	}

	function ActionComplete() {
		__doPostBack('<%=lnkBonusActionComplete.UniqueID%>', '');
	}

	<% if (EnableVideoSharing) { %>
	function ShareOnFacebook() {
		var fbsharewindow = window.open('http://www.facebook.com/sharer.php?s=100&p[title]=' + encodeURIComponent(_sharedVideoTitle) + '&p[url]=' + encodeURIComponent(_sharedVideoUrl) + '&t=', 'fbsharer', 'toolbar=0,status=0,width=626,height=436');
		var timer = setInterval(function () {
			if (fbsharewindow) {
				if (fbsharewindow.closed) {
					clearInterval(timer);
					//alert('facebook share window closed');
				}
			}
		}, 1000);
	}

	function ShareOnTwitter() {
		var twsharewindow = window.open('http://www.twitter.com/intent/tweet?url=' + encodeURIComponent(_sharedVideoUrl) + '&text=' + encodeURIComponent(_sharedVideoTitle), 'twsharer', 'toolbar=0,status=0,width=626,height=436');
		var timer = setInterval(function () {
			if (twsharewindow) {
				if (twsharewindow.closed) {
					clearInterval(timer);
					//alert('twitter share window closed');  
				}
			}
		}, 1000);
	}
	<% } %>
</script>



<div id="BonusesContainer">

	<h2 id="Title">
		<asp:Literal runat="server" ID="litTitle"></asp:Literal>
	</h2>

	<div class="header">
		<asp:LinkButton runat="server" ID="lnkBackToOffersHeader" meta:resourcekey="lnkBackToOffersHeader"></asp:LinkButton>
	</div>

	<div style="display: none;">
		<asp:LinkButton runat="server" ID="lnkBonusActionComplete" Text="Complete"></asp:LinkButton>
	</div>

	<asp:HiddenField runat="server" ID="hdnBonusId" />
	<asp:HiddenField runat="server" ID="hdnVideoUrl" />

	<asp:MultiView runat="server" ID="mvMain">

		<asp:View runat="server" ID="viewNoBonuses">
			<asp:PlaceHolder runat="server" ID="pchNoBonuses"></asp:PlaceHolder>
		</asp:View>

		<asp:View runat="server" ID="viewBonusList">
			<div id="Bonuses">
				<div class="header"></div>
				<div id="BonusesInner">
					<div id="HeroBonus">
						<div id="HeroText">
							<p id="HeroHeadline"></p>
							<p id="HeroDescription"></p>
							<p><a id="lnkVideo" href="#" class="heroButton"><asp:Literal runat="server" ID="litWatchVideo" meta:resourcekey="litWatchVideo"></asp:Literal></a></p>
							<p><a id="lnkHtml" href="#" class="heroButton"><asp:Literal runat="server" ID="litViewHtml" meta:resourcekey="litViewHtml"></asp:Literal></a></p>
							<p><a id="lnkSurvey" href="#" class="heroButton"><asp:Literal runat="server" ID="litTakeSurvey" meta:resourcekey="litTakeSurvey"></asp:Literal></a></p>
							<p><a id="lnkCustom" href="#" class="heroButton"></a></p>
						</div>
				

						<div id="HeroImage">
							<a href="#" id="HeroRemove"></a>
							<a href="#" id="HeroPoints"><span></span><asp:Literal runat="server" ID="lblPointsHero" meta:resourcekey="lblPointsHero"></asp:Literal></a>
							<a href="#" id="HeroImageLink">
								<img style="padding:8px; border:none;" alt="" />
							</a>
						</div>
					</div>

					<div id="BonusNavigation">
						<asp:PlaceHolder runat="server" ID="pchCategories"></asp:PlaceHolder>
						<asp:LinkButton runat="server" ID="lnkPreferences" CssClass="DashboardLink" Text="Preferences" meta:resourcekey="lnkProfile"></asp:LinkButton>
					</div>

					<asp:ListView runat="server" ID="lstBonuses" ItemPlaceholderID="ItemPlaceholder">
				
						<LayoutTemplate>
							<div id="BonusListWrapper">
								<div id="BonusListHeader"></div>
								<div id="BonusScrollLeft"><div></div></div>
								<div id="BonusList">
									<asp:PlaceHolder runat="server" ID="ItemPlaceholder"></asp:PlaceHolder>
								</div>
								<div id="BonusScrollRight"><div></div></div>
							</div>
						</LayoutTemplate>
						<ItemTemplate>
							<div class="Bonus" id="Bonus_<%#Eval("Id")%>" data-bonusid="<%#Eval("Id")%>" data-logoimagemobile="<%# Eval("LogoImageMobile")%>" data-logoimageweb="<%# Eval("LogoImageWeb")%>" data-logoimagehero="<%# Eval("LogoImageHero")%>" data-action="<%# Eval("NextAction")%>" data-gobutton="<%#Eval("GoButtonLabel") %>">
								
								<asp:LinkButton runat="server" ID="lnkGetBonus" CommandName="GetBonus" CommandArgument='<%#Eval("Id")%>' CssClass="ListGet">
									<div class="BonusImage">
										<img alt="<%#Eval("Headline")%>" src="<%# Eval( PortalState.UserChannel == "Mobile" ? "LogoImageMobile" : "LogoImageWeb")%>" />
									</div>
									<div class="BonusDesc">
										<span><%#Eval("Description")%></span>
									</div>
									<div class="<%#Eval("Points").ToString() != "0" ? "BonusPoints" : "BonusNoPoints" %>">
										<span><span><%#Eval("Points")%></span><asp:Literal runat="server" ID="lblPointsSmall" meta:resourcekey="lblPointsSmall"></asp:Literal></span>
									</div>
									<div class="Headline">
										<span><%#Eval("Headline")%></span>
									</div>
								</asp:LinkButton>
								<asp:LinkButton runat="server" ID="lnkRemove" CommandName="RemoveBonus" CommandArgument='<%#Eval("Id")%>' CssClass="ListRemove" meta:resourcekey="lnkRemove"></asp:LinkButton>
							</div>
							<div class="separator"></div>
						</ItemTemplate>

						<EmptyDataTemplate>
							<div class="EmptyBonus" data-logoimagemobile="" data-logoimageweb="" data-logoimagehero="">
									<div class="BonusDesc">
										<span></span>
									</div>
									<div class="BonusNoPoints">
										<span></span>
									</div>
									<div class="Headline">
										<span></span>
									</div>
							</div>
							<div class="separator"></div>
						</EmptyDataTemplate>
				
					</asp:ListView>
				</div>
			</div>			
			
		</asp:View>
	
        <asp:View runat="server" ID="viewHtmlOffer">
			<div id="BonusDetail">
				<div class="header"></div>
				<div id="BonusDetailInner">
					<div id="VideoContainer">
						<div id="VideoComplete">
							<div>
								<a href="javascript:RestartVideo();"><asp:Literal ID="lblPlayAgain" runat="server" meta:resourcekey="lblPlayAgain" /></a>

								<a href="javascript:ActionComplete();"><asp:Literal ID="lblFinished" runat="server" meta:resourcekey="lblFinished" /></a>
								<% if (EnableVideoSharing) { %>
								<div id="VideoShare">
									<asp:Literal ID="lblShare" runat="server" meta:resourcekey="lblShare" />
									<a href="javascript:ShareOnFacebook();">
										<img src="skin/images/facebook_32.png"/>
									</a>
									<a href="javascript:ShareOnTwitter();">
										<img src="skin/images/twitter_32.png"/>
									</a>
								</div>
								<% } %>
							</div>
						</div>
						<div id="JWPlayer"></div>
					</div>

					<div id="divHtmlOffer">
						<asp:PlaceHolder runat="server" ID="pchHtmlOffer"></asp:PlaceHolder>
						<div class="buttons">
							
							<asp:LinkButton runat="server" ID="lnkButtonNextStep" CssClass="button" Text="Complete" meta:resourcekey="lnkNextStep"></asp:LinkButton>
						</div>
					</div>
				</div>
			</div>
		</asp:View>
        	
		<asp:View runat="server" ID="viewSurvey">
			<div id="BonusSurvey">
				<div class="header"></div>
				<div id="Survey">
					<WebFramework:SurveyRunnerControl ID="SurveyRunner" runat="server" meta:resourcekey="SurveyRunner"
					SurveySelectionMethod="SurveyID" UseAppCache="true" NextButtonUseLinkButton="true" />
				</div>
			</div>
		</asp:View>
	
        <asp:View runat="server" ID="viewFinishedHtml">
			<div id="BonusComplete">
				<div class="header"></div>
				<div id="BonusCompleteInner">
					<div id="divFinishedHtml">
						<asp:PlaceHolder runat="server" ID="pchFinishedHtml"></asp:PlaceHolder>
					</div>

					<div class="buttons">
						<asp:HyperLink runat="server" ID="lnkReferralUrl" CssClass="button ReferralLink" Target="_blank"></asp:HyperLink>
						<asp:LinkButton runat="server" ID="lnkBackToOffersFinished" CssClass="button" Text="Back to Offers" meta:resourcekey="lnkBackToOffers"></asp:LinkButton>
					</div>
				</div>
			</div>
		</asp:View>

		<asp:View runat="server" ID="viewProfileSurveys">
			
			<div id="ProfileSurvey">
				<div class="header"></div>
				<div id="ProfileSurveyInner">
					<asp:ListView runat="server" ID="lstProfileSurveys" ItemPlaceholderID="ItemPlaceholder">
						<LayoutTemplate>
							<div id="ProfileSurveyList">
								<asp:PlaceHolder runat="server" ID="ItemPlaceholder"></asp:PlaceHolder>
							</div>
						</LayoutTemplate>
						<ItemTemplate>
							<div class="ProfileSurvey" id="Survey_<%#Eval("Id")%>">
								<asp:LinkButton runat="server" ID="lnkGetBonus" CommandName="GetSurvey" CommandArgument='<%#Eval("Id")%>'>
									<span><%#Eval("Name")%></span>
								</asp:LinkButton>
							</div>
							<div class="separator"></div>
						</ItemTemplate>
					</asp:ListView>

					<asp:PlaceHolder runat="server" ID="pchNoSurveys" Visible="false"></asp:PlaceHolder>

					<div>
						<asp:LinkButton runat="server" ID="lnkBackToOffersProfile" CssClass="button" Text="Back to Offers" meta:resourcekey="lnkBackToOffers"></asp:LinkButton>
					</div>
				</div>
			</div>
		</asp:View>

	</asp:MultiView>
</div>
</asp:PlaceHolder>