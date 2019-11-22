create or replace package AE_Quarterly_Reward_Load is
type rcursor IS REF CURSOR;

     PROCEDURE IssueQuarterlyRewards(p_DMFileName IN VARCHAR2, p_CodesFileName IN VARCHAR2,Retval        IN OUT Rcursor);
     PROCEDURE ChangeExternalTable(p_ExtTableName IN VARCHAR2,p_FileName IN VARCHAR2);
     PROCEDURE ProcessRewardsFile(p_filename IN VARCHAR2);
     PROCEDURE ProcessCodeFiles(p_filename IN VARCHAR2);
     PROCEDURE AddNewRewards(p_RewardName IN NVARCHAR2);
     PROCEDURE InsertMemberRewards;

END AE_Quarterly_Reward_Load;
/
create or replace package body AE_Quarterly_Reward_Load is

  PROCEDURE IssueQuarterlyRewards(p_DMFileName IN VARCHAR2, p_CodesFileName IN VARCHAR2,
            Retval        IN OUT Rcursor) IS
  BEGIN
            delete from bp_ae.x$_ae_qrwd_missing;
            ProcessRewardsFile(p_filename => p_DMFileName );
            ProcessCodeFiles(p_filename => p_CodesFileName);
            InsertMemberRewards;

  END IssueQuarterlyRewards;

  /*
   * Used to parse file into external table
   */
  PROCEDURE ChangeExternalTable(p_ExtTableName IN VARCHAR2,
                              p_FileName     IN VARCHAR2)
    IS
      e_MTable exception;
      e_MFileName exception;
      v_sql VARCHAR2(400);
    BEGIN

        IF LENGTH(TRIM(p_ExtTableName))=0 OR p_ExtTableName is NULL THEN
          raise_application_error(-20000, 'External tablename is required', FALSE);
        ELSIF LENGTH(TRIM(p_FileName))=0 OR p_FileName is NULL THEN
          raise_application_error(-20001, 'Filename is required to link with external table', FALSE);
        END IF;

        v_sql := 'ALTER TABLE '||p_ExtTableName||' LOCATION (AE_IN'||CHR(58)||''''||p_FileName||''')';
        EXECUTE IMMEDIATE v_sql;

  END ChangeExternalTable;

  /*
   * Insert Members from AE Rewards File
   */
  PROCEDURE ProcessRewardsFile(p_filename IN VARCHAR2) IS
        member_status number;
        curr_auth1 number;
        curr_auth2 number;
        /* Process file into external table */
        BEGIN
        ChangeExternalTable(p_ExtTableName => 'EXT_AE_DMREWARDS', p_FileName => p_filename);
        -- Begin AEO-833 Changes

        --delete from bp_ae.x$_ae_qrwd_missing; --Clean previous entries.
        --commit;
                                                                                --AH */
        for rec in (select dr.loyaltyid, dr.auth1 from bp_ae.ext_ae_dmrewards dr) LOOP  --AEO-1936
          /* Make sure that member exists in lw_virtualcard */
          select count(*) into member_status from bp_ae.lw_virtualcard vc where vc.loyaltyidnumber = rec.loyaltyid;
          if ( member_status = 0 ) then
            insert into bp_ae.x$_ae_qrwd_missing
                        (loyaltyid,
                        auth_code,
                        notes,
                        createdate)
                 values
                   (rec.loyaltyid,
                    null,
                    'Member Not Found',
                    sysdate);
            commit;
            continue;
          end if;

          select nvl(lm.memberstatus,0) into member_status from bp_ae.lw_loyaltymember lm
           where 1=1
           and lm.ipcode in (select vc.ipcode from bp_ae.lw_virtualcard vc
                  where 1=1
                  and vc.loyaltyidnumber = rec.loyaltyid );

          if ( member_status in (2,3) ) then
            insert into bp_ae.x$_ae_qrwd_missing
                        (loyaltyid,
                        auth_code,
                        notes,
                        createdate)
                 values
                   (rec.loyaltyid,
                    null,
                    'Member Status: ' || member_status,
                    sysdate);
             commit;
          end if;

          select count(rsc.a_shortcode) into curr_auth1 from bp_ae.ats_rewardshortcode rsc where rsc.a_shortcode = replace(replace(rec.auth1, CHR(13),''), CHR(10),'');
          --select count(rsc.a_shortcode) into curr_auth2 from bp_ae.ats_rewardshortcode rsc where rsc.a_shortcode = replace(replace(rec.auth2, CHR(13),''), CHR(10),''); --AEO-1936 AH
          if ( curr_auth1 = 0 and rec.auth1 <> NULL) then
            insert into bp_ae.x$_ae_qrwd_missing (loyaltyid,
                                                  auth_code,
                                                  notes,
                                                  createdate)
            values ( null,
                     rec.auth1,
                     'Auth Code Added to ats_rewardshortcode',
                     sysdate );

            insert into bp_ae.ats_rewardshortcode
                   (a_rowkey,
                    lwidentifier,
                    a_ipcode,
                    a_parentrowkey,
                    a_shortcode,
                    a_effectivestartdate,
                    a_effectiveenddate,
                    a_description,
                    a_typecode,
                    statuscode,
                    createdate,
                    updatedate,
                    lastdmlid)
            values (hibernate_sequence.nextval,
                    0,
                    0,
                    0,
                    rec.auth1,
                    Trunc(sysdate, 'Q'),
                    Trunc(sysdate, 'Q') + 45,
                    'AE Quarterly Reward',
                    null,
                    0,
                    sysdate,
                    sysdate,
                    null);
             commit;
          end if;

          /*if ( curr_auth2 = 0 and rec.auth2 <> NULL) then
            insert into bp_ae.x$_ae_qrwd_missing (loyaltyid,
                                                  auth_code,
                                                  notes,
                                                  createdate)
            values ( null,
                     rec.auth2,
                     'Auth Code Added to ats_rewardshortcode',
                     sysdate );

            insert into bp_ae.ats_rewardshortcode
                   (a_rowkey,
                    lwidentifier,
                    a_ipcode,
                    a_parentrowkey,
                    a_shortcode,
                    a_effectivestartdate,
                    a_effectiveenddate,
                    a_description,
                    a_typecode,
                    statuscode,
                    createdate,
                    updatedate,
                    lastdmlid)
            values (hibernate_sequence.nextval,
                    0,
                    0,
                    0,
                    rec.auth2,
                    Trunc(sysdate, 'Q'),
                    Trunc(sysdate, 'Q') + 45,
                    'AE Quarterly Reward',
                    null,
                    0,
                    sysdate,
                    sysdate,
                    null);
             commit;
          end if;*/ --AEO-1936 AH

        end loop;

        /*-- Clean external table
        delete from bp_ae.ext_ae_dmrewards dr
              where 1=1
              and dr.loyaltyid in ( select qr.loyaltyid from bp_ae.x$_ae_qrwd_missing qr );
        -- End AEO-833 changes                                                                                    -- AH */
  END ProcessRewardsFile;

  /*
   * Get codes used in AE Rewards File
   */
  PROCEDURE ProcessCodeFiles(p_filename IN VARCHAR2) IS

            v_temp NUMBER;
            shortcode_count NUMBER;
      BEGIN
          ChangeExternalTable(p_ExtTableName => 'EXT_AE_DMREWARDS_CODES', p_FileName => p_filename);
          --check codes for new instances
          FOR rec IN (SELECT replace(replace(drc.reward, CHR(13),''), CHR(10),'') AS reward, drc.offercode FROM ext_ae_dmrewards_codes drc)
          LOOP
              SELECT count(rd.name) INTO v_temp FROM bp_ae.lw_rewardsdef rd WHERE rd.name IN (rec.reward);
              SELECT count(rsc.a_shortcode) INTO shortcode_count FROM bp_ae.ats_rewardshortcode rsc WHERE rsc.a_shortcode IN (rec.offercode);
              -- If the shortcode exists and there isn't a valid product id, then we add it to the product table and rewardsdef
              IF ( v_temp = 0) THEN
                  AddNewRewards(p_RewardName => trim(rec.reward));
              END IF;
              IF ( shortcode_count = 0 ) THEN
                insert into bp_ae.ats_rewardshortcode
                   (a_rowkey,
                    lwidentifier,
                    a_ipcode,
                    a_parentrowkey,
                    a_shortcode,
                    a_effectivestartdate,
                    a_effectiveenddate,
                    a_description,
                    a_typecode,
                    statuscode,
                    createdate,
                    updatedate,
                    lastdmlid)
            values (hibernate_sequence.nextval,
                    0,
                    0,
                    0,
                    rec.offercode,
                    Trunc(sysdate, 'Q'),
                    Trunc(sysdate, 'Q') + 45,
                    'AE Quarterly Reward',
                    null,
                    0,
                    sysdate,
                    sysdate,
                    null);
            end if;
          END LOOP;
  END ProcessCodeFiles;

  /*
   * Add new rewards into lw_product and lw_rewardsdef
   */
  PROCEDURE AddNewRewards(p_RewardName IN NVARCHAR2) IS
  BEGIN
    --------------
    --Products
    --------------
    INSERT into bp_ae.lw_product
           (id,categoryid,isvisibleinln,NAME,baseprice,quantity,createdate,createdbyuser)
    VALUES
           (hibernate_sequence.nextval,(select id from lw_category where name = 'Reward'),1,p_RewardName,0,1,sysdate,0);
    commit;
    ------------------
    -- Insert Rewards
    ------------------
    insert into bp_ae.lw_rewardsdef
        (id,name,howmanypointstoearn,productid,threshhold,active,createdate)
    VALUES
        (hibernate_sequence.nextval,p_RewardName,99999,                  --set points ridiculously high
        (select id from bp_ae.lw_product where name = p_RewardName),0,1,sysdate);
    commit;
  END AddNewRewards;

  /*
   * Insert members from Rewards DM file into lw_memberrewards
   */
  PROCEDURE InsertMemberRewards is
    ---------------------------------
    --Insert into lw_memberrewards
    ---------------------------------
              --log job attributes
              v_My_Log_Id        NUMBER;
              v_Jobdirection     NUMBER := 0;
              v_Starttime        DATE := SYSDATE;
              v_Endtime          DATE;
              v_Messagesreceived NUMBER := 0;
              v_Messagesfailed   NUMBER := 0;
              v_Jobstatus        NUMBER := 0;
              v_Jobname          VARCHAR2(256);
              v_Processid        NUMBER := 0;
              v_Errormessage     VARCHAR2(256);
              --log msg attributes
              v_Messageid   VARCHAR2(256);
              v_Envkey      VARCHAR2(256) := 'bp_ae@' ||
                                             Upper(Sys_Context('userenv',
                                                               'instance_name'));
              v_Logsource   VARCHAR2(256);
              v_Batchid     VARCHAR2(256) := 0;
              v_Message     VARCHAR2(256) := ' ';
              v_Reason      VARCHAR2(256);
              v_Error       VARCHAR2(256);
              v_Trycount    NUMBER := 0;
              v_prid        NUMBER := -1;
              v_rwdefid     NUMBER := -1;
              p_StartDate nvarchar2(12);
              p_ExpDate nvarchar2(12);
              v_temp        number;

             CURSOR c IS
                  SELECT    vc.ipcode,
                            br.suc1,
                            br.auth1,
                            --br.suc2,              --AEO-1936 AH
                            --br.auth2,             --AEO-1936 AH
                            br.expiration_Date,
                            rd_1.productid AS pid1,
                            rd_1.id AS r_id1
                         --rd_2.productid AS pid2, rd_2.id AS r_id2,  r_name.Reward, r_name2.Reward AS Reward2  --AEO-1936 AH
                  FROM bp_ae.EXT_AE_DMREWARDS br
                  JOIN bp_ae.lw_virtualcard vc ON vc.loyaltyidnumber = br.loyaltyid
                  LEFT JOIN bp_ae.EXT_AE_DMREWARDS_CODES r_name ON r_name.OfferCode = br.auth1
                  LEFT JOIN bp_ae.lw_rewardsdef rd_1 ON rd_1.name = r_name.Reward
                  --LEFT JOIN bp_ae.EXT_AE_DMREWARDS_CODES r_name2 ON r_name2.OfferCode = br.auth2          --AEO-1936 AH
                  --LEFT JOIN bp_ae.lw_rewardsdef rd_2 ON rd_2.name = r_name2.Reward                        --AEO-1936 AH
                  where 1=1
                  and vc.loyaltyidnumber not in (select qm.loyaltyid from bp_ae.x$_ae_qrwd_missing qm);

            TYPE c_type IS TABLE OF c%ROWTYPE INDEX BY PLS_INTEGER;
            rec c_type;

    BEGIN
            /*Logging*/
            v_My_Log_Id := Utility_Pkg.Get_Libjobid();
            --  v_dap_log_id := utility_pkg.get_LIBJobID();
            v_Jobname   := Upper('insert_member_rewards');
            v_Logsource := v_Jobname;
            /* log start of job */
            Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                              p_Jobdirection     => v_Jobdirection,
                              p_Filename         => NULL,
                              p_Starttime        => v_Starttime,
                              p_Endtime          => v_Endtime,
                              p_Messagesreceived => v_Messagesreceived,
                              p_Messagesfailed   => v_Messagesfailed,
                              p_Jobstatus        => v_Jobstatus,
                              p_Jobname          => v_Jobname);
            Utility_Pkg.Log_Process_Start(v_Jobname, v_Jobname, v_Processid);
            /*End of Logging*/
           OPEN c;
           LOOP
                FETCH c BULK COLLECT INTO rec LIMIT 1000;
                FOR i IN 1 .. rec.COUNT
                LOOP
                  --May want to remove
                /*select count(*) into v_temp from bp_ae.lw_virtualcard vc where vc.ipcode in (rec(i).ipcode);

                if ( v_temp = 0 ) then
                  --ipcode into external "bad lw table"
                  continue;
                end if;*/
                  --End remove

                --Check if value is there.
                --IF nvl(rec(i).suc1, '') != '' OR rec(i).suc1 IS NOT NULL THEN --AEO-1936 AH
                  -- get start and expiration dates
                    /*
                    select dmc.startdate into p_StartDate
                    from bp_ae.ext_ae_dmrewards_codes dmc
                    where rec(i).auth1 in (dmc.offercode);
                    */        --AEO-1936 AH
                    /*
                    select replace(replace(dmc.expdate, CHR(13),''), CHR(10),'')
                    into p_ExpDate from bp_ae.ext_ae_dmrewards_codes dmc
                    where rec(i).auth1 = dmc.offercode;
                    */        --AEO-1936 AH
                    
                    INSERT INTO bp_ae.lw_memberrewards
                                 ( ID,
                                 REWARDDEFID,
                                   CERTIFICATENMBR,
                                   OFFERCODE,
                                   AVAILABLEBALANCE,
                                   FULFILlMENTOPTION,
                                   MEMBERID,
                                   PRODUCTID,
                                   productvariantid,
                                   dateissued,
                                   expiration,
                                   lwordernumber,
                                   createdate)
                    VALUES ( hibernate_sequence.nextval,
                                    rec(i).r_id1,
                                    rec(i).suc1,
                                    rec(i).auth1,
                                    0,
                                    NULL,
                                    rec(i).ipcode,
                                    rec(i).pid1,
                                    -1,
                                    SYSDATE,       --Check these dates            --AEO-1936 AH
                                    to_date(rec(i).expiration_date,'MM/DD/YYYY'),       --Check these dates --AEO-1936 AH
                                    0,
                                    SYSDATE );

                --END IF;--AEO-1936 AH
                COMMIT;
                --Check if second code is available
                /*IF nvl(rec(i).suc2, '') != '' OR rec(i).suc2 IS NOT NULL THEN
                    select dmc.startdate into p_StartDate from bp_ae.ext_ae_dmrewards_codes dmc where rec(i).auth2 in (dmc.offercode);
                    select replace(replace(dmc.expdate, CHR(13),''), CHR(10),'') into p_ExpDate from bp_ae.ext_ae_dmrewards_codes dmc where rec(i).auth2 = dmc.offercode;
                    INSERT INTO bp_ae.lw_memberrewards
                                 ( ID,
                                 REWARDDEFID,
                                   CERTIFICATENMBR,
                                   OFFERCODE,
                                   AVAILABLEBALANCE,
                                   FULFILlMENTOPTION,
                                   MEMBERID,
                                   PRODUCTID,
                                   productvariantid,
                                   dateissued,
                                   expiration,
                                   lwordernumber,
                                   createdate)
                    VALUES ( hibernate_sequence.nextval,
                                    rec(i).r_id2,
                                    rec(i).suc2,
                                    rec(i).auth2,
                                    0,
                                    NULL,
                                    rec(i).ipcode,
                                    rec(i).pid2,
                                    -1,
                                    to_date(p_StartDate,'MM/DD/YYYY'),       --Check these dates
                                    to_date(p_ExpDate,'MM/DD/YYYY'),       --Check these dates
                                    0,
                                    SYSDATE );

                END IF;*/           --AEO-1936 AH
                COMMIT;
                END LOOP;
           EXIT WHEN c%NOTFOUND;
           END LOOP;
           COMMIT;
           IF c%ISOPEN
          THEN
               CLOSE c;
          END IF;
           v_Endtime   := SYSDATE;
           v_Jobstatus := 1;
           Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                          p_Jobdirection     => v_Jobdirection,
                          p_Filename         => NULL,
                          p_Starttime        => v_Starttime,
                          p_Endtime          => v_Endtime,
                          p_Messagesreceived => v_Messagesreceived,
                          p_Messagesfailed   => v_Messagesfailed,
                          p_Jobstatus        => v_Jobstatus,
                          p_Jobname          => v_Jobname);
    EXCEPTION
      WHEN OTHERS THEN
                   v_Error          := SQLERRM;
                   v_Messagesfailed := v_Messagesfailed + 1;
                   v_Endtime        := SYSDATE;
                   Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                       p_Jobdirection     => v_Jobdirection,
                                       p_Filename         => NULL,
                                       p_Starttime        => v_Starttime,
                                       p_Endtime          => v_Endtime,
                                       p_Messagesreceived => v_Messagesreceived,
                                       p_Messagesfailed   => v_Messagesfailed,
                                       p_Jobstatus        => v_Jobstatus,
                                       p_Jobname          => v_Jobname);
                   Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                       p_Envkey    => v_Envkey,
                                       p_Logsource => 'AE_QUARTERLY_REWARD_LOAD',
                                       p_Filename  => NULL,
                                       p_Batchid   => v_Batchid,
                                       p_Jobnumber => v_My_Log_Id,
                                       p_Message   => v_Message,
                                       p_Reason    => v_Reason,
                                       p_Error     => v_Error,
                                       p_Trycount  => v_Trycount,
                                       p_Msgtime   => SYSDATE);
                   Raise_Application_Error(-20002,
                                           'Other Exception detected in ' ||
                                           v_Jobname || ' ');

    END InsertMemberRewards;

end AE_Quarterly_Reward_Load;
/
