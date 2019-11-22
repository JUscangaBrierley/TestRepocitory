if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_attr_attrset]') AND parent_object_id = OBJECT_ID('lw_attribute')) alter table lw_attribute drop constraint fk_attr_attrset;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_product_category]') AND parent_object_id = OBJECT_ID('lw_product')) alter table lw_product drop constraint fk_product_category;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_product_pointtype]') AND parent_object_id = OBJECT_ID('lw_product')) alter table lw_product drop constraint fk_product_pointtype;
go
	
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_lwproductvariant_productid]') AND parent_object_id = OBJECT_ID('lw_productvariant')) alter table lw_productvariant drop constraint fk_lwproductvariant_productid;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_productimage_product]') AND parent_object_id = OBJECT_ID('lw_productimage')) alter table lw_productimage drop constraint fk_productimage_product;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_rewards_product]') AND parent_object_id = OBJECT_ID('lw_rewardsdef')) alter table lw_rewardsdef drop constraint fk_rewards_product;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_rewards_tier]') AND parent_object_id = OBJECT_ID('lw_rewardsdef')) alter table lw_rewardsdef drop constraint fk_rewards_tier;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_pointtransaction_vckey]') AND parent_object_id = OBJECT_ID('lw_pointtransaction')) alter table lw_pointtransaction drop constraint fk_pointtransaction_vckey;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_pointtransaction_pointtype]') AND parent_object_id = OBJECT_ID('lw_pointtransaction')) alter table lw_pointtransaction drop constraint fk_pointtransaction_pointtype;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_pointtransaction_pointevent]') AND parent_object_id = OBJECT_ID('lw_pointtransaction')) alter table lw_pointtransaction drop constraint fk_pointtransaction_pointevent;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_memberpromotion_promocode]') AND parent_object_id = OBJECT_ID('lw_memberpromotion')) alter table lw_memberpromotion drop constraint fk_memberpromotion_promocode;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_memberpromotion_ipcode]') AND parent_object_id = OBJECT_ID('lw_memberpromotion')) alter table lw_memberpromotion drop constraint fk_memberpromotion_ipcode;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_memberreward_rewarddef]') AND parent_object_id = OBJECT_ID('lw_memberrewards')) alter table lw_memberrewards drop constraint fk_memberreward_rewarddef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_memberreward_ipcode]') AND parent_object_id = OBJECT_ID('lw_memberrewards')) alter table lw_memberrewards drop constraint fk_memberreward_ipcode;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_membertier_tierdef]') AND parent_object_id = OBJECT_ID('lw_membertiers')) alter table lw_membertiers drop constraint fk_membertier_tierdef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_membertier_ipcode]') AND parent_object_id = OBJECT_ID('lw_membertiers')) alter table lw_membertiers drop constraint fk_membertier_ipcode;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_membermessage_defid]') AND parent_object_id = OBJECT_ID('lw_membermessage')) alter table lw_membermessage drop constraint fk_membermessage_defid;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_rolefunc_role]') AND parent_object_id = OBJECT_ID('lw_csrolefunction')) alter table lw_csrolefunction drop constraint fk_rolefunc_role;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_rolefunc_func]') AND parent_object_id = OBJECT_ID('lw_csrolefunction')) alter table lw_csrolefunction drop constraint fk_rolefunc_func;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_csnote_memberid]') AND parent_object_id = OBJECT_ID('lw_csnote')) alter table lw_csnote drop constraint fk_csnote_memberid;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_document_template]') AND parent_object_id = OBJECT_ID('lw_document')) alter table lw_document drop constraint fk_document_template;
go
	
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_email_personal]') AND parent_object_id = OBJECT_ID('lw_emailpersonalization')) alter table lw_emailpersonalization drop constraint fk_email_personal;
go
	
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_mailing_email]') AND parent_object_id = OBJECT_ID('lw_mailing')) alter table lw_mailing drop constraint fk_mailing_email;
go
	
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_scd_sca]') AND parent_object_id = OBJECT_ID('lw_structuredcontentdata')) alter table lw_structuredcontentdata drop constraint fk_scd_sca;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_scd_batch]') AND parent_object_id = OBJECT_ID('lw_structuredcontentdata')) alter table lw_structuredcontentdata drop constraint fk_scd_batch;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_coupondef_category]') AND parent_object_id = OBJECT_ID('lw_coupondef')) alter table lw_coupondef drop constraint fk_coupondef_category;
go
	
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_memcpn_coupondef]') AND parent_object_id = OBJECT_ID('lw_membercoupon')) alter table lw_membercoupon drop constraint fk_memcpn_coupondef;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_membonus_bonusdef]') AND parent_object_id = OBJECT_ID('lw_memberbonus')) alter table lw_memberbonus drop constraint fk_membonus_bonusdef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_membonus_memid]') AND parent_object_id = OBJECT_ID('lw_memberbonus')) alter table lw_memberbonus drop constraint fk_membonus_memid;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_bonusdef_category]') AND parent_object_id = OBJECT_ID('lw_bonusdef')) alter table lw_bonusdef drop constraint fk_bonusdef_category;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_memberstores_stores]') AND parent_object_id = OBJECT_ID('lw_memberstore')) alter table lw_memberstore drop constraint fk_memberstores_stores;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_jobrun_jobid]') AND parent_object_id = OBJECT_ID('lw_scheduledjobrun')) alter table lw_scheduledjobrun drop constraint fk_jobrun_jobid;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_contentattribute_defid]') AND parent_object_id = OBJECT_ID('lw_contentattribute')) alter table lw_contentattribute drop constraint fk_contentattribute_defid;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_culturemap_language]') AND parent_object_id = OBJECT_ID('lw_sm_culturemap')) alter table lw_sm_culturemap drop constraint fk_culturemap_language;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_state_survey]') AND parent_object_id = OBJECT_ID('lw_sm_state')) alter table lw_sm_state drop constraint fk_state_survey;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_question_state]') AND parent_object_id = OBJECT_ID('lw_sm_question')) alter table lw_sm_question drop constraint fk_question_state;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_questioncontent_question]') AND parent_object_id = OBJECT_ID('lw_sm_questioncontent')) alter table lw_sm_questioncontent drop constraint fk_questioncontent_question;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_questioncontent_language]') AND parent_object_id = OBJECT_ID('lw_sm_questioncontent')) alter table lw_sm_questioncontent drop constraint fk_questioncontent_language;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_answercontent_question]') AND parent_object_id = OBJECT_ID('lw_sm_answercontent')) alter table lw_sm_answercontent drop constraint fk_answercontent_question;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_answercontent_language]') AND parent_object_id = OBJECT_ID('lw_sm_answercontent')) alter table lw_sm_answercontent drop constraint fk_answercontent_language;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_decision_state]') AND parent_object_id = OBJECT_ID('lw_sm_decision')) alter table lw_sm_decision drop constraint fk_decision_state;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_respondent_survey]') AND parent_object_id = OBJECT_ID('lw_sm_respondent')) alter table lw_sm_respondent drop constraint fk_respondent_survey;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_respondent_language]') AND parent_object_id = OBJECT_ID('lw_sm_respondent')) alter table lw_sm_respondent drop constraint fk_respondent_language;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_response_respondent]') AND parent_object_id = OBJECT_ID('lw_sm_response')) alter table lw_sm_response drop constraint fk_response_respondent;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_transition_state_src]') AND parent_object_id = OBJECT_ID('lw_sm_transition')) alter table lw_sm_transition drop constraint fk_transition_state_src;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_transition_state_dst]') AND parent_object_id = OBJECT_ID('lw_sm_transition')) alter table lw_sm_transition drop constraint fk_transition_state_dst;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_message_state]') AND parent_object_id = OBJECT_ID('lw_sm_message')) alter table lw_sm_message drop constraint fk_message_state;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_concept_survey]') AND parent_object_id = OBJECT_ID('lw_sm_concept')) alter table lw_sm_concept drop constraint fk_concept_survey;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_concept_language]') AND parent_object_id = OBJECT_ID('lw_sm_concept')) alter table lw_sm_concept drop constraint fk_concept_language;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_conceptview_concept]') AND parent_object_id = OBJECT_ID('lw_sm_conceptview')) alter table lw_sm_conceptview drop constraint fk_conceptview_concept;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_conceptview_respondent]') AND parent_object_id = OBJECT_ID('lw_sm_conceptview')) alter table lw_sm_conceptview drop constraint fk_conceptview_respondent;
go

if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clstep_clcampaign]') AND parent_object_id = OBJECT_ID('lw_clstep')) alter table lw_clstep drop constraint fk_clstep_clcampaign;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clstep_cltable]') AND parent_object_id = OBJECT_ID('lw_clstep')) alter table lw_clstep drop constraint fk_clstep_cltable;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clstepioxref_clstep]') AND parent_object_id = OBJECT_ID('lw_clstepioxref')) alter table lw_clstepioxref drop constraint fk_clstepioxref_clstep;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clstepioxref_clstep1]') AND parent_object_id = OBJECT_ID('lw_clstepioxref')) alter table lw_clstepioxref drop constraint fk_clstepioxref_clstep1;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_cltablekey_claudience]') AND parent_object_id = OBJECT_ID('lw_cltablekey')) alter table lw_cltablekey drop constraint fk_cltablekey_claudience;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_cltablekey_cltable]') AND parent_object_id = OBJECT_ID('lw_cltablekey')) alter table lw_cltablekey drop constraint fk_cltablekey_cltable;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_cltblfldval_cltblfld]') AND parent_object_id = OBJECT_ID('lw_cltablefieldvalue')) alter table lw_cltablefieldvalue drop constraint fk_cltblfldval_cltblfld;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_cltablefield_cltable]') AND parent_object_id = OBJECT_ID('lw_cltablefield')) alter table lw_cltablefield drop constraint fk_cltablefield_cltable;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clglbattr_clglb]') AND parent_object_id = OBJECT_ID('lw_clglobalattribute')) alter table lw_clglobalattribute drop constraint fk_clglbattr_clglb;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clcampattr_clcamp]') AND parent_object_id = OBJECT_ID('lw_clcampaignattribute')) alter table lw_clcampaignattribute drop constraint fk_clcampattr_clcamp;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_clcampattr_clattr]') AND parent_object_id = OBJECT_ID('lw_clcampaignattribute')) alter table lw_clcampaignattribute drop constraint fk_clcampattr_clattr;
go


if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_campaignoffer]') AND parent_object_id = OBJECT_ID('LW_CLOffer')) alter table LW_CLOffer drop constraint fk_campaignoffer;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_CampaignSegment]') AND parent_object_id = OBJECT_ID('LW_CLSegment')) alter table LW_CLSegment drop constraint FK_CampaignSegment;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_OfferXRef]') AND parent_object_id = OBJECT_ID('lw_cloffersegmentxref')) alter table lw_cloffersegmentxref drop constraint FK_OfferXRef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_SegmentXRef]') AND parent_object_id = OBJECT_ID('LW_CLOfferSegmentXRef')) alter table LW_CLOfferSegmentXRef drop constraint FK_SegmentXRef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_CampaignAttributeOffer]') AND parent_object_id = OBJECT_ID('LW_CLCampaignAttribute')) alter table LW_CLCampaignAttribute drop constraint FK_CampaignAttributeOffer;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_CampaignAttributeSegment]') AND parent_object_id = OBJECT_ID('LW_CLCampaignAttribute')) alter table LW_CLCampaignAttribute drop constraint FK_CampaignAttributeSegment;
go


if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_pbpassdef_cert]') AND parent_object_id = OBJECT_ID('lw_pbpassdef')) alter table lw_pbpassdef drop constraint fk_pbpassdef_cert;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_lwcardpassdef]') AND parent_object_id = OBJECT_ID('lw_loyaltycardpassdef')) alter table lw_loyaltycardpassdef drop constraint fk_lwcardpassdef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_lwcouponpassdef]') AND parent_object_id = OBJECT_ID('lw_couponpassdef')) alter table lw_couponpassdef drop constraint fk_lwcouponpassdef;
go
if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[fk_pref_language]') AND parent_object_id = OBJECT_ID('lw_loyaltymember')) alter table lw_loyaltymember drop constraint fk_pref_language;
go

if exists(select 1 from sys.objects where name = 'distance') drop function distance;
go