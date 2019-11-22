alter table lw_attribute drop constraint fk_attr_attrset;
/

alter table lw_product drop constraint fk_product_category;
/
alter table lw_product drop constraint fk_product_pointtype;
/

alter table lw_productvariant drop constraint fk_lwproductvariant_productid;
/

alter table lw_productimage drop constraint fk_productimage_product;
/

alter table lw_rewardsdef drop constraint fk_rewards_product;
/
alter table lw_rewardsdef drop constraint fk_rewards_tier;
/

alter table lw_pointtransaction drop constraint fk_pointtransaction_vckey;
/
alter table lw_pointtransaction drop constraint fk_pointtransaction_pointtype;
/
alter table lw_pointtransaction drop constraint fk_pointtransaction_pointevent;
/

alter table lw_memberpromotion drop constraint fk_memberpromotion_promocode;
/
alter table lw_memberpromotion drop constraint fk_memberpromotion_ipcode;
/

alter table lw_memberrewards drop constraint fk_memberreward_rewarddef;
/
alter table lw_memberrewards drop constraint fk_memberreward_ipcode;
/

alter table lw_membertiers drop constraint fk_membertier_tierdef;
/
alter table lw_membertiers drop constraint fk_membertier_ipcode;
/

alter table lw_membermessage drop constraint fk_membermessage_defid;
/

alter table lw_csrolefunction drop constraint fk_rolefunc_role;
/
alter table lw_csrolefunction drop constraint fk_rolefunc_func;
/

alter table lw_csnote drop constraint fk_csnote_memberid;
/

alter table lw_document drop constraint fk_document_template;
/
alter table lw_emailpersonalization drop constraint fk_email_personal;
/
alter table lw_mailing drop constraint fk_mailing_email;
/
alter table lw_structuredcontentdata drop constraint fk_scd_sca;
/
alter table lw_structuredcontentdata drop constraint fk_scd_batch;
/

alter table lw_coupondef drop constraint fk_coupondef_category;
/
alter table lw_membercoupon drop constraint fk_memcpn_coupondef;
/

alter table lw_memberbonus drop constraint fk_membonus_bonusdef;
/
alter table lw_memberbonus drop constraint fk_membonus_memid;
/

alter table lw_bonusdef drop constraint fk_bonusdef_category;
/

alter table lw_memberstore drop constraint fk_memberstores_stores;
/

alter table lw_scheduledjobrun drop constraint fk_jobrun_jobid;
/

alter table lw_contentattribute drop constraint fk_contentattribute_defid;
/

alter table lw_sm_culturemap drop constraint fk_culturemap_language;
/
alter table lw_sm_state drop constraint fk_state_survey;
/
alter table lw_sm_question drop constraint fk_question_state;
/
alter table lw_sm_questioncontent drop constraint fk_questioncontent_question;
/
alter table lw_sm_questioncontent drop constraint fk_questioncontent_language;
/
alter table lw_sm_answercontent drop constraint fk_answercontent_question;
/
alter table lw_sm_answercontent drop constraint fk_answercontent_language;
/
alter table lw_sm_decision drop constraint fk_decision_state;
/
alter table lw_sm_respondent drop constraint fk_respondent_survey;
/
alter table lw_sm_respondent drop constraint fk_respondent_language;
/
alter table lw_sm_response drop constraint fk_response_respondent;
/
alter table lw_sm_transition drop constraint fk_transition_state_src;
/
alter table lw_sm_transition drop constraint fk_transition_state_dst;
/
alter table lw_sm_message drop constraint fk_message_state;
/
alter table lw_sm_concept drop constraint fk_concept_survey;
/
alter table lw_sm_concept drop constraint fk_concept_language;
/
alter table lw_sm_conceptview drop constraint fk_conceptview_concept;
/
alter table lw_sm_conceptview drop constraint fk_conceptview_respondent;
/

alter table lw_clstep drop constraint fk_clstep_clcampaign;
/
alter table lw_clstep drop constraint fk_clstep_cltable;
/
alter table lw_clstepioxref drop constraint fk_clstepioxref_clstep;
/
alter table lw_clstepioxref drop constraint fk_clstepioxref_clstep1;
/
alter table lw_cltablekey drop constraint fk_cltablekey_claudience;
/
alter table lw_cltablekey drop constraint fk_cltablekey_cltable;
/
alter table lw_cltablefieldvalue drop constraint fk_cltblfldval_cltblfld;
/
alter table lw_cltablefield drop constraint fk_cltablefield_cltable;
/
alter table lw_clglobalattribute drop constraint fk_clglbattr_clglb;
/
alter table lw_clcampaignattribute drop constraint fk_clcampattr_clcamp;
/
alter table lw_clcampaignattribute drop constraint fk_clcampattr_clattr;
/

alter table LW_CLOffer drop constraint fk_campaignoffer;
/
alter table LW_CLSegment drop constraint FK_CampaignSegment;
/
alter table lw_cloffersegmentxref drop constraint FK_OfferXRef;
/
alter table LW_CLOfferSegmentXRef drop constraint FK_SegmentXRef;
/
alter table LW_CLCampaignAttribute drop constraint FK_CampaignAttributeOffer;
/
alter table LW_CLCampaignAttribute drop constraint FK_CampaignAttributeSegment;
/

alter table lw_pbpassdef drop constraint fk_pbpassdef_cert;
/
alter table lw_loyaltycardpassdef drop constraint fk_lwcardpassdef;
/
alter table lw_couponpassdef drop constraint fk_lwcouponpassdef;
/
alter table lw_loyaltymember drop constraint fk_pref_language;
/

drop FUNCTION distance;
/
