CREATE OR REPLACE PACKAGE Rulesprocessing AS


     TYPE Rcursor IS REF CURSOR;

     FUNCTION GetExpirationDate( p_memberdate  DATE ,
                                  p_processdate DATE ) return DATE;


     PROCEDURE Truncatetxnwrk(p_Dummy VARCHAR2,
                              Retval  IN OUT Rcursor);

     PROCEDURE Truncatetxnwrk2(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor);

     PROCEDURE Truncatetxnwrk3(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor);

     PROCEDURE Truncatetxnwrk4(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor);

     PROCEDURE Truncatetxnwrk5(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor);

     PROCEDURE Truncatetxnwrk6(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor);

     PROCEDURE Refreshpromotables(p_Dummy VARCHAR2,
                                  Retval  IN OUT Rcursor);

     PROCEDURE Headerrulesprocess(p_Dummy       VARCHAR2,
                                  p_Processdate VARCHAR2,
                                  Retval        IN OUT Rcursor);

     PROCEDURE Detailtxnrulesprocess(p_Dummy       VARCHAR2,
                                     p_Processdate VARCHAR2,
                                     Retval        IN OUT Rcursor);

     PROCEDURE Headerrulesprocess2(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor);

     PROCEDURE Detailtxnrulesprocess2(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor);

     PROCEDURE Headerrulesprocess3(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor);

     PROCEDURE Detailtxnrulesprocess3(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor);

     PROCEDURE Headerrulesprocess4(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor);

     PROCEDURE Detailtxnrulesprocess4(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor);

     PROCEDURE Headerrulesprocess5(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor);

     PROCEDURE Detailtxnrulesprocess5(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor);

     PROCEDURE Headerrulesprocess6(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor);

     PROCEDURE Detailtxnrulesprocess6(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor);

     PROCEDURE Bonuseventreturn(p_Dummy       VARCHAR2,
                                p_Processdate VARCHAR2,
                                Retval        IN OUT Rcursor);

     PROCEDURE Bonuseventreturn2(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor);

     PROCEDURE Bonuseventreturn3(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor);

     PROCEDURE Bonuseventreturn4(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor);

     PROCEDURE Bonuseventreturn5(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor);

     PROCEDURE Bonuseventreturn6(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor);

     PROCEDURE Ae_Basic_Purchase(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP);

     PROCEDURE Ae_Basic_Return(v_Header_Rowkey      NUMBER,
                               v_Vckey              NUMBER,
                               v_Txntypeid          NUMBER,
                               v_Brandid            NUMBER,
                               v_Txndate            DATE,
                               v_Txnqualpurchaseamt FLOAT,
                               v_Txnemployeeid      NVARCHAR2,
                               v_Txnheaderid        NVARCHAR2,
                               v_My_Log_Id          NUMBER,
                               p_Processdate        TIMESTAMP);

     PROCEDURE Aerie_Basic_Purchase(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP);

     PROCEDURE Aerie_Basic_Return(v_Header_Rowkey      NUMBER,
                                  v_Vckey              NUMBER,
                                  v_Txntypeid          NUMBER,
                                  v_Brandid            NUMBER,
                                  v_Txndate            DATE,
                                  v_Txnqualpurchaseamt FLOAT,
                                  v_Txnemployeeid      NVARCHAR2,
                                  v_Txnheaderid        NVARCHAR2,
                                  v_My_Log_Id          NUMBER,
                                  p_Processdate        TIMESTAMP);

     PROCEDURE Worldspend_Purchase(v_Header_Rowkey      NUMBER,
                                   v_Vckey              NUMBER,
                                   v_Txntypeid          NUMBER,
                                   v_Brandid            NUMBER,
                                   v_Txndate            DATE,
                                   v_Txnqualpurchaseamt FLOAT,
                                   v_Txnemployeeid      NVARCHAR2,
                                   v_Txnheaderid        NVARCHAR2,
                                   v_My_Log_Id          NUMBER,
                                   p_Processdate        TIMESTAMP);


     PROCEDURE Bra_Redemption(v_Detailitem_Rowkey NUMBER,
                              v_Ipcode            NUMBER,
                              v_Txntypeid         NUMBER,
                              v_Dtlsaleamount     FLOAT,
                              v_Txndate           DATE,
                              v_Txnheaderid       NVARCHAR2,
                              v_Dtlclasscode      NVARCHAR2,
                              v_My_Log_Id         NUMBER,
                              p_Processdate       TIMESTAMP);

     PROCEDURE Bra_Employee(v_Detailitem_Rowkey NUMBER,
                            v_Ipcode            NUMBER,
                            v_Txntypeid         NUMBER,
                            v_Dtlsaleamount     FLOAT,
                            v_Txndate           DATE,
                            v_Txnheaderid       NVARCHAR2,
                            v_Dtlclasscode      NVARCHAR2,
                            v_My_Log_Id         NUMBER,
                            p_Processdate       TIMESTAMP);

     PROCEDURE Bra_Employee_Return(v_Detailitem_Rowkey NUMBER,
                                   v_Ipcode            NUMBER,
                                   v_Txntypeid         NUMBER,
                                   v_Dtlsaleamount     FLOAT,
                                   v_Txndate           DATE,
                                   v_Txnheaderid       NVARCHAR2,
                                   v_Dtlclasscode      NVARCHAR2,
                                   v_My_Log_Id         NUMBER,
                                   p_Processdate       TIMESTAMP);

     PROCEDURE Jean_Purchase(v_Detailitem_Rowkey NUMBER,
                             v_Ipcode            NUMBER,
                             v_Txntypeid         NUMBER,
                             v_Dtlsaleamount     FLOAT,
                             v_Txndate           DATE,
                             v_Txnheaderid       NVARCHAR2,
                             v_Dtlclasscode      NVARCHAR2,
                             v_My_Log_Id         NUMBER,
                             p_Processdate       TIMESTAMP);

     PROCEDURE Jean_Return(v_Detailitem_Rowkey NUMBER,
                           v_Ipcode            NUMBER,
                           v_Txntypeid         NUMBER,
                           v_Dtlsaleamount     FLOAT,
                           v_Txndate           DATE,
                           v_Txnheaderid       NVARCHAR2,
                           v_Dtlclasscode      NVARCHAR2,
                           v_My_Log_Id         NUMBER,
                           p_Processdate       TIMESTAMP);


     --AEO-140
     PROCEDURE B5g1_Bra(v_Detailitem_Rowkey NUMBER,
                        v_Ipcode            NUMBER,
                        v_Txntypeid         NUMBER,
                        v_Dtlsaleamount     FLOAT,
                        v_Txndate           DATE,
                        v_Txnheaderid       NVARCHAR2,
                        v_Dtlclasscode      NVARCHAR2,
                        v_My_Log_Id         NUMBER,
                        p_Processdate       TIMESTAMP);

     PROCEDURE B5g1_Jean(v_Detailitem_Rowkey NUMBER,
                         v_Ipcode            NUMBER,
                         v_Txntypeid         NUMBER,
                         v_Dtlsaleamount     FLOAT,
                         v_Txndate           DATE,
                         v_Txnheaderid       NVARCHAR2,
                         v_Dtlclasscode      NVARCHAR2,
                         v_My_Log_Id         NUMBER,
                         p_Processdate       TIMESTAMP);

     PROCEDURE B5g1_Bra_Return(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP);

     PROCEDURE B5g1_Jean_Return(v_Detailitem_Rowkey NUMBER,
                                v_Ipcode            NUMBER,
                                v_Txntypeid         NUMBER,
                                v_Dtlsaleamount     FLOAT,
                                v_Txndate           DATE,
                                v_Txnheaderid       NVARCHAR2,
                                v_Dtlclasscode      NVARCHAR2,
                                v_My_Log_Id         NUMBER,
                                p_Processdate       TIMESTAMP);

     PROCEDURE Aecc_Basic_Purchase(v_Header_Rowkey      NUMBER,
                                   v_Vckey              NUMBER,
                                   v_Txntypeid          NUMBER,
                                   v_Brandid            NUMBER,
                                   v_Txndate            DATE,
                                   v_Txnqualpurchaseamt FLOAT,
                                   v_Txnemployeeid      NVARCHAR2,
                                   v_Txnheaderid        NVARCHAR2,
                                   v_My_Log_Id          NUMBER,
                                   p_Processdate        TIMESTAMP);

     PROCEDURE Aecc_Basic_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP);



     --AEO-252
     PROCEDURE Updatetierandnetspend(v_Header_Rowkey      NUMBER,
                                     v_Vckey              NUMBER,
                                     v_Txntypeid          NUMBER,
                                     v_Brandid            NUMBER,
                                     v_Txndate            DATE,
                                     v_Txnqualpurchaseamt FLOAT,
                                     v_Txnemployeeid      NVARCHAR2,
                                     v_Txnheaderid        NVARCHAR2,
                                     v_My_Log_Id          NUMBER,
                                     p_Processdate        TIMESTAMP);

     --AEO-252
     --AEO-364
     PROCEDURE Updatetieronly(v_Header_Rowkey      NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     --AEO-364

     -- AEO-472
     PROCEDURE Jean_Redemption(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP);

     -- AEP-472


-- AEO-629 START

PROCEDURE AECC_bonus_points(v_header_rowkey number,
                                             v_vckey number,
                                             v_txntypeid  number,
                                             v_brandid number,
                                             v_txndate date,
                                             v_txnqualpurchaseamt float,
                                             v_txnemployeeid nvarchar2,
                                             v_txnheaderid nvarchar2,
                                             v_my_log_id number,
                                             p_Processdate TIMESTAMP);

-- AEO-629 END

--AEO-1818 Begin GD
     PROCEDURE DblePoints_Tuesdays_2017(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePoints_Tuesdays_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-1818 End GD

--AEO-1821 Begin LAPP
     PROCEDURE Bonus_Transact_AECC_2017(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-1821 END LAPP
--AEO-1938 Begin GD
     PROCEDURE DblePoints_Everything_102017(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePoints_102017_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-1938 End GD
--AEO-1822 Begin LAPP
     PROCEDURE DblePnts_JeanBra_112017(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePnts_JeanBra_112017_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-1822 END LAPP
--AEO-1888 START LAPP
     PROCEDURE ExtraAccess_Reward_112017(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

--AEO-1888 END LAPP
--AEO-1823 START LAPP
     PROCEDURE DblePoints_Everything_122017(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePoints_122017_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-1823 END LAPP
--AEO-2100 START LAPP
     PROCEDURE InactivityBonus072018(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

--AEO-2100 END LAPP
     --AEO-2111 Begin LAPP
     PROCEDURE DbleCred_Bra_012018(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP);

     PROCEDURE DbleCred_Bra_012018_Return(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP);
    --AEO-2111 END LAPP
--AEO-2110 Begin GD
     PROCEDURE DblePnts_Jean_012018(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePnts_Jean_012018_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-2110 End GD

--AEO-2363 Begin
     PROCEDURE DblePnts_Swimwear_2018(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePnts_Swimwear_2018_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-2363 End
-- AEO-2364 negin
PROCEDURE B5g1_Shorts(v_Detailitem_Rowkey NUMBER,
                         v_Ipcode            NUMBER,
                         v_Txntypeid         NUMBER,
                         v_Dtlsaleamount     FLOAT,
                         v_Txndate           DATE,
                         v_Txnheaderid       NVARCHAR2,
                         v_Dtlclasscode      NVARCHAR2,
                         v_My_Log_Id         NUMBER,
                         p_Processdate       TIMESTAMP);

PROCEDURE B5g1_Shorts_Return(v_Detailitem_Rowkey NUMBER,
                                v_Ipcode            NUMBER,
                                v_Txntypeid         NUMBER,
                                v_Dtlsaleamount     FLOAT,
                                v_Txndate           DATE,
                                v_Txnheaderid       NVARCHAR2,
                                v_Dtlclasscode      NVARCHAR2,
                                v_My_Log_Id         NUMBER,
                                p_Processdate       TIMESTAMP);
--AEO-2364 end

--AEO-2448 Begin MM
     PROCEDURE DblePoints_Everything_052018(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePoints_052018_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-2448 End MM
--AEO-2495 Begin GD
     PROCEDURE DblePnts_Dorm_062018(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);

     PROCEDURE DblePnts_Dorm_062018_Return(v_Header_Rowkey NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP);
--AEO-2495 End GD
END Rulesprocessing;
/
CREATE OR REPLACE PACKAGE BODY Rulesprocessing IS

 FUNCTION Getexpirationdate( p_memberdate DATE,
                             p_processdate DATE     ) RETURN DATE IS

       Lv_Initialyear NUMBER := Extract(YEAR FROM Nvl(p_memberdate, SYSDATE));
       Lv_Endingyear  NUMBER := Extract(YEAR FROM Nvl(p_processdate, SYSDATE));
     --  Lv_nominationYear NUMBER := Extract(YEAR FROM Nvl(p_processdate, SYSDATE));
       Lv_resultValue DATE := SYSDATE;
       lv_elapsedyears NUMBER := 0;

  BEGIN
       lv_elapsedyears  := (lv_endingyear - Lv_initialyear);

       CASE
           WHEN (lv_elapsedyears < 3 AND lv_elapsedyears >=0) THEN
              Lv_resultValue := to_date('12/31/'|| (Lv_Endingyear+1),'mm/dd/yyyy')+1;
           WHEN (lv_elapsedyears < 0 ) THEN
              Lv_resultValue := to_date('12/31/2199','mm/dd/yyyy');
           ELSE
              Lv_resultValue :=   to_date('12/31/'|| Lv_Endingyear,'mm/dd/yyyy')+1;
       END CASE;

       RETURN(Lv_resultValue);
  END Getexpirationdate;


     PROCEDURE Truncatetxnwrk(p_Dummy VARCHAR2,
                              Retval  IN OUT Rcursor) AS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk';
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_wrk';
     END Truncatetxnwrk;

     PROCEDURE Truncatetxnwrk2(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor) AS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk2';
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_wrk2';
     END Truncatetxnwrk2;

     PROCEDURE Truncatetxnwrk3(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor) AS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk3';
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_wrk3';
     END Truncatetxnwrk3;

     PROCEDURE Truncatetxnwrk4(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor) AS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk4';
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_wrk4';
     END Truncatetxnwrk4;

     PROCEDURE Truncatetxnwrk5(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor) AS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk5';
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_wrk5';
     END Truncatetxnwrk5;

     PROCEDURE Truncatetxnwrk6(p_Dummy VARCHAR2,
                               Retval  IN OUT Rcursor) AS
     BEGIN
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txnheader_Wrk6';
          EXECUTE IMMEDIATE 'Truncate TABLE LW_txndetailitem_wrk6';
     END Truncatetxnwrk6;

     PROCEDURE Refreshpromotables(p_Dummy VARCHAR2,
                                  Retval  IN OUT Rcursor) IS
          v_Sql1 VARCHAR2(1000);
          v_Sql2 VARCHAR2(1000);
     BEGIN
          v_Sql1 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                  where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
          v_Sql2 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                  where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';
          EXECUTE IMMEDIATE 'truncate Table AE_BRAPROMOCODES';
          EXECUTE IMMEDIATE 'truncate Table AE_JEANSPROMOCODES';
          EXECUTE IMMEDIATE v_Sql1;
          EXECUTE IMMEDIATE v_Sql2;
          COMMIT;
     END Refreshpromotables;

     PROCEDURE Headerrulesprocess(p_Dummy       VARCHAR2,
                                  p_Processdate VARCHAR2,
                                  Retval        IN OUT Rcursor) IS
          Pdummy               VARCHAR2(2);
          v_Header_Rowkey      NUMBER := 0;
          v_Vckey              NUMBER := 0;
          v_Brandid            NUMBER := 0;
          v_Txntypeid          NUMBER := 0;
          v_Txndate            DATE;
          v_Processdate        DATE;
          v_Txnqualpurchaseamt FLOAT;
          v_Txnheaderid        NVARCHAR2(50) := '';
          v_Txnemployeeid      NVARCHAR2(25) := '';
          v_Rulename           NVARCHAR2(50) := '';
          v_Cnt                NUMBER := 0;
          v_Sql1               VARCHAR2(100);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txnheader_Wrk
          CURSOR Get_Txn IS
               SELECT Hw.Header_Rowkey,
                      Hw.Vckey,
                      Hw.Txntypeid,
                      Hw.Brandid,
                      Hw.Txndate,
                      Hw.Txnqualpurchaseamt,
                      Nvl(Hw.Txnemployeeid, 0),
                      Hw.Txnheaderid
               FROM   Lw_Txnheader_Wrk Hw;
          CURSOR Get_Rules IS
          SELECT TRIM(Rn.a_Rulename)
             FROM   bp_ae.Ats_Rulenames Rn
             WHERE  Rn.a_Rulestatus = 1
                    AND Rn.a_Rulepath = 'H'
             ORDER  BY (CASE
                            WHEN Rn.a_Rulename = 'UpdateTierandNetSpend' THEN
                             0
                            WHEN Rn.a_Rulename = 'AECC_bonus_points' THEN
                             2
                            WHEN Rn.a_Rulename = 'UpdateTierOnly' THEN
                              999
                            WHEN Rn.a_Rulename = 'ExtraAccess_Reward_112017' THEN
                              1000
                            ELSE
                             1
                         END);
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'HeaderRules_Processor';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_HeaderRules',
                                        v_Processid);
          /*Logging*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Header_Rowkey,
                         v_Vckey,
                         v_Txntypeid,
                         v_Brandid,
                         v_Txndate,
                         v_Txnqualpurchaseamt,
                         v_Txnemployeeid,
                         v_Txnheaderid;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i, :j); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Header_Rowkey, v_Vckey, v_Txntypeid, v_Brandid, v_Txndate, v_Txnqualpurchaseamt, v_Txnemployeeid, v_Txnheaderid, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --       open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN No_Data_Found THEN
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Headerrulesprocess;

     PROCEDURE Detailtxnrulesprocess(p_Dummy       VARCHAR2,
                                     p_Processdate VARCHAR2,
                                     Retval        IN OUT Rcursor) IS
          Pdummy              VARCHAR2(2);
          v_Detailitem_Rowkey NUMBER := 0;
          v_Ipcode            NUMBER := 0;
          v_Brandid           NUMBER := 0;
          v_Txntypeid         NUMBER := 0;
          v_Txndate           DATE;
          v_Dtlsaleamount     FLOAT;
          v_Txnheaderid       NVARCHAR2(50) := '';
          v_Rulename          NVARCHAR2(50) := '';
          v_Dtlclasscode      NVARCHAR2(50) := '';
          v_Cnt               NUMBER := 0;
          v_Sql1              VARCHAR2(100);
          v_Bra_Codes         Aetabletype;
          v_Jean_Codes        Aetabletype;
          v_Cclist            VARCHAR2(1000);
          v_Cclist2           VARCHAR2(1000);
          v_Sql2              VARCHAR2(1000);
          v_Sql3              VARCHAR2(1000);
          v_Sql4              VARCHAR2(1000);
          v_Sql5              VARCHAR2(1000);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txndetailitem_Wrk
          CURSOR Get_Txn IS
               SELECT --dw.rowkey, AEO-51 changes here ---------SCJ
                Dw.Dtl_Rowkey, --AEO-51 changes here ---------SCJ
                Dw.Ipcode,
                Dw.Dtltypeid,
                Dw.Dtlsaleamount,
                Dw.Txndate,
                Dw.Txnheaderid,
                Dw.Dtlclasscode
               FROM   Lw_Txndetailitem_Wrk Dw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
               SELECT TRIM(Rn.a_Rulename)
               FROM   Ats_Rulenames Rn
               WHERE  Rn.a_Rulestatus = 1
                      AND Rn.a_Rulepath = 'D'; -- active Detailitem rules
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'DetailItemRules_Processor';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_DetailitemRules',
                                        v_Processid);
          /*Logging*/
          /*
          select cc.value into v_cclist from Lw_Clientconfiguration cc
                                                    where cc.key = 'BraPromoClassCodes';
          select cc.value into v_cclist2 from Lw_Clientconfiguration cc
                                                    where cc.key = 'JeansPromoClassCodes';
          v_bra_codes:=StringToTable(v_cclist,';');
          v_jean_codes:=StringToTable(v_cclist2,';');
          */
          /*
           v_sql2 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                          where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
           v_sql4 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                          where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';
                                                                                          -- v_sql3 := 'CREATE INDEX AE_BRAPROMOCODES_CC  ON AE_BRAPROMOCODES(classcodes)  ONLINE COMPUTE STATISTICS TABLESPACE bp_ae_idx1';
          --  v_sql5 := 'CREATE INDEX AE_JEANSPROMOCODES_CC  ON AE_JEANSPROMOCODES(classcodes)  ONLINE COMPUTE STATISTICS TABLESPACE bp_ae_idx1';
            EXECUTE IMMEDIATE 'truncate Table AE_BRAPROMOCODES';
            EXECUTE IMMEDIATE 'truncate Table AE_JEANSPROMOCODES';
            EXECUTE IMMEDIATE v_sql2 ;
          --  EXECUTE IMMEDIATE v_sql3 ;
            EXECUTE IMMEDIATE v_sql4 ;
          --  EXECUTE IMMEDIATE v_sql5 ;*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Detailitem_Rowkey,
                         v_Ipcode,
                         v_Txntypeid,
                         v_Dtlsaleamount,
                         v_Txndate,
                         v_Txnheaderid,
                         v_Dtlclasscode;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i); END;';
                    EXECUTE IMMEDIATE v_Sql1
                    --               USING v_detailitem_rowkey,v_ipcode,v_txntypeid,v_dtlsaleamount,v_txndate,v_txnheaderid,v_dtlclasscode,v_bra_codes,v_jean_codes;  -- v_rulename(a, b, c, d, e, f, g, h, i)
                         USING v_Detailitem_Rowkey, v_Ipcode, v_Txntypeid, v_Dtlsaleamount, v_Txndate, v_Txnheaderid, v_Dtlclasscode, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --        open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure DetailTxnRulesProcess: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DetailTxnRulesProcess</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Detailtxnrulesprocess;

     PROCEDURE Headerrulesprocess2(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor) IS
          Pdummy               VARCHAR2(2);
          v_Header_Rowkey      NUMBER := 0;
          v_Vckey              NUMBER := 0;
          v_Brandid            NUMBER := 0;
          v_Txntypeid          NUMBER := 0;
          v_Txndate            DATE;
          v_Txnqualpurchaseamt FLOAT;
          v_Txnheaderid        NVARCHAR2(50) := '';
          v_Txnemployeeid      NVARCHAR2(25) := '';
          v_Rulename           NVARCHAR2(50) := '';
          v_Cnt                NUMBER := 0;
          v_Sql1               VARCHAR2(100);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txnheader_Wrk
          CURSOR Get_Txn IS
               SELECT Hw.Header_Rowkey,
                      Hw.Vckey,
                      Hw.Txntypeid,
                      Hw.Brandid,
                      Hw.Txndate,
                      Hw.Txnqualpurchaseamt,
                      Nvl(Hw.Txnemployeeid, 0),
                      Hw.Txnheaderid
               FROM   Lw_Txnheader_Wrk2 Hw;
          --Cursor2 Get_Rules
          CURSOR Get_Rules IS
          SELECT TRIM(Rn.a_Rulename)
             FROM   bp_ae.Ats_Rulenames Rn
             WHERE  Rn.a_Rulestatus = 1
                    AND Rn.a_Rulepath = 'H'
             ORDER  BY (CASE
                            WHEN Rn.a_Rulename = 'UpdateTierandNetSpend' THEN
                             0
                            WHEN Rn.a_Rulename = 'AECC_bonus_points' THEN
                             2
                            WHEN Rn.a_Rulename = 'UpdateTierOnly' THEN
                              999
                            ELSE
                             1
                         END);
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'HeaderRules_Processor2';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_HeaderRules2',
                                        v_Processid);
          /*Logging*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Header_Rowkey,
                         v_Vckey,
                         v_Txntypeid,
                         v_Brandid,
                         v_Txndate,
                         v_Txnqualpurchaseamt,
                         v_Txnemployeeid,
                         v_Txnheaderid;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i, :j); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Header_Rowkey, v_Vckey, v_Txntypeid, v_Brandid, v_Txndate, v_Txnqualpurchaseamt, v_Txnemployeeid, v_Txnheaderid, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --      open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess2: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess2</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Headerrulesprocess2;

     PROCEDURE Detailtxnrulesprocess2(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor) IS
          Pdummy              VARCHAR2(2);
          v_Detailitem_Rowkey NUMBER := 0;
          v_Ipcode            NUMBER := 0;
          v_Brandid           NUMBER := 0;
          v_Txntypeid         NUMBER := 0;
          v_Txndate           DATE;
          v_Dtlsaleamount     FLOAT;
          v_Txnheaderid       NVARCHAR2(50) := '';
          v_Rulename          NVARCHAR2(50) := '';
          v_Dtlclasscode      NVARCHAR2(50) := '';
          v_Cnt               NUMBER := 0;
          v_Sql1              VARCHAR2(100);
          v_Bra_Codes         Aetabletype;
          v_Jean_Codes        Aetabletype;
          v_Cclist            VARCHAR2(1000);
          v_Cclist2           VARCHAR2(1000);
          v_Bracdcnt          NUMBER := 0;
          v_Jeancdcnt         NUMBER := 0;
          v_Sql2              VARCHAR2(1000);
          v_Sql3              VARCHAR2(1000);
          v_Sql4              VARCHAR2(1000);
          v_Sql5              VARCHAR2(1000);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txndetailitem_Wrk
          CURSOR Get_Txn IS
               SELECT --dw.rowkey, AEO-51 changes here ---------SCJ
                Dw.Dtl_Rowkey, --AEO-51 changes here ---------SCJ
                Dw.Ipcode,
                Dw.Dtltypeid,
                Dw.Dtlsaleamount,
                Dw.Txndate,
                Dw.Txnheaderid,
                Dw.Dtlclasscode
               FROM   Lw_Txndetailitem_Wrk2 Dw;
          --  Cursor Get_Rules is
          CURSOR Get_Rules IS
               SELECT TRIM(Rn.a_Rulename)
               FROM   Ats_Rulenames Rn
               WHERE  Rn.a_Rulestatus = 1
                      AND Rn.a_Rulepath = 'D'; -- active Detailitem rules
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'DetailItemRules_Processor2';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_DetailitemRules2',
                                        v_Processid);
          /*Logging*/
          /* select count(bc.classcodes) into v_bracdcnt from AE_BRAPROMOCODES bc;
          select count(jc.classcodes) into v_jeancdcnt from AE_JEANSPROMOCODES jc;

          v_sql2 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
          v_sql4 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';
          if (v_bracdcnt = 0)
            then
              EXECUTE IMMEDIATE v_sql2 ;
          end if;
           if (v_jeancdcnt = 0)
            then
            EXECUTE IMMEDIATE v_sql4 ;
          end if;                   */
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Detailitem_Rowkey,
                         v_Ipcode,
                         v_Txntypeid,
                         v_Dtlsaleamount,
                         v_Txndate,
                         v_Txnheaderid,
                         v_Dtlclasscode;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i ); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Detailitem_Rowkey, v_Ipcode, v_Txntypeid, v_Dtlsaleamount, v_Txndate, v_Txnheaderid, v_Dtlclasscode, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h, i)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --       open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure DetailTxnRulesProcess2: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DetailTxnRulesProcess2</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Detailtxnrulesprocess2;

     PROCEDURE Headerrulesprocess3(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor) IS
          Pdummy               VARCHAR2(2);
          v_Header_Rowkey      NUMBER := 0;
          v_Vckey              NUMBER := 0;
          v_Brandid            NUMBER := 0;
          v_Txntypeid          NUMBER := 0;
          v_Txndate            DATE;
          v_Txnqualpurchaseamt FLOAT;
          v_Txnheaderid        NVARCHAR2(50) := '';
          v_Txnemployeeid      NVARCHAR2(25) := '';
          v_Rulename           NVARCHAR2(50) := '';
          v_Cnt                NUMBER := 0;
          v_Sql1               VARCHAR2(100);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txnheader_Wrk
          CURSOR Get_Txn IS
               SELECT Hw.Header_Rowkey,
                      Hw.Vckey,
                      Hw.Txntypeid,
                      Hw.Brandid,
                      Hw.Txndate,
                      Hw.Txnqualpurchaseamt,
                      Nvl(Hw.Txnemployeeid, 0),
                      Hw.Txnheaderid
               FROM   Lw_Txnheader_Wrk3 Hw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
          SELECT TRIM(Rn.a_Rulename)
             FROM   bp_ae.Ats_Rulenames Rn
             WHERE  Rn.a_Rulestatus = 1
                    AND Rn.a_Rulepath = 'H'
             ORDER  BY (CASE
                            WHEN Rn.a_Rulename = 'UpdateTierandNetSpend' THEN
                             0
                            WHEN Rn.a_Rulename = 'AECC_bonus_points' THEN
                             2
                            WHEN Rn.a_Rulename = 'UpdateTierOnly' THEN
                              999
                            ELSE
                             1
                         END);
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'HeaderRules_Processor3';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_HeaderRules3',
                                        v_Processid);
          /*Logging*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Header_Rowkey,
                         v_Vckey,
                         v_Txntypeid,
                         v_Brandid,
                         v_Txndate,
                         v_Txnqualpurchaseamt,
                         v_Txnemployeeid,
                         v_Txnheaderid;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i, :j); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Header_Rowkey, v_Vckey, v_Txntypeid, v_Brandid, v_Txndate, v_Txnqualpurchaseamt, v_Txnemployeeid, v_Txnheaderid, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --       open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess3: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess3</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Headerrulesprocess3;

     PROCEDURE Detailtxnrulesprocess3(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor) IS
          Pdummy              VARCHAR2(2);
          v_Detailitem_Rowkey NUMBER := 0;
          v_Ipcode            NUMBER := 0;
          v_Brandid           NUMBER := 0;
          v_Txntypeid         NUMBER := 0;
          v_Txndate           DATE;
          v_Dtlsaleamount     FLOAT;
          v_Txnheaderid       NVARCHAR2(50) := '';
          v_Rulename          NVARCHAR2(50) := '';
          v_Dtlclasscode      NVARCHAR2(50) := '';
          v_Cnt               NUMBER := 0;
          v_Sql1              VARCHAR2(100);
          v_Bra_Codes         Aetabletype;
          v_Jean_Codes        Aetabletype;
          v_Cclist            VARCHAR2(1000);
          v_Cclist2           VARCHAR2(1000);
          v_Bracdcnt          NUMBER := 0;
          v_Jeancdcnt         NUMBER := 0;
          v_Sql2              VARCHAR2(1000);
          v_Sql3              VARCHAR2(1000);
          v_Sql4              VARCHAR2(1000);
          v_Sql5              VARCHAR2(1000);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txndetailitem_Wrk
          CURSOR Get_Txn IS
               SELECT --dw.rowkey, AEO-51 changes here ---------SCJ
                Dw.Dtl_Rowkey, --AEO-51 changes here ---------SCJ
                Dw.Ipcode,
                Dw.Dtltypeid,
                Dw.Dtlsaleamount,
                Dw.Txndate,
                Dw.Txnheaderid,
                Dw.Dtlclasscode
               FROM   Lw_Txndetailitem_Wrk3 Dw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
               SELECT TRIM(Rn.a_Rulename)
               FROM   Ats_Rulenames Rn
               WHERE  Rn.a_Rulestatus = 1
                      AND Rn.a_Rulepath = 'D'; -- active Detailitem rules
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'DetailItemRules_Processor3';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_DetailitemRules3',
                                        v_Processid);
          /*Logging*/
          /*  select count(bc.classcodes) into v_bracdcnt from AE_BRAPROMOCODES bc;
          select count(jc.classcodes) into v_jeancdcnt from AE_JEANSPROMOCODES jc;
             v_sql2 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
            v_sql4 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';
          if (v_bracdcnt = 0)
            then
              EXECUTE IMMEDIATE v_sql2 ;
          end if;
           if (v_jeancdcnt = 0)
            then
            EXECUTE IMMEDIATE v_sql4 ;
          end if;                           */
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Detailitem_Rowkey,
                         v_Ipcode,
                         v_Txntypeid,
                         v_Dtlsaleamount,
                         v_Txndate,
                         v_Txnheaderid,
                         v_Dtlclasscode;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Detailitem_Rowkey, v_Ipcode, v_Txntypeid, v_Dtlsaleamount, v_Txndate, v_Txnheaderid, v_Dtlclasscode, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h, i)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --    open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure DetailTxnRulesProcess3: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DetailTxnRulesProcess3</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Detailtxnrulesprocess3;

     PROCEDURE Headerrulesprocess4(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor) IS
          Pdummy               VARCHAR2(2);
          v_Header_Rowkey      NUMBER := 0;
          v_Vckey              NUMBER := 0;
          v_Brandid            NUMBER := 0;
          v_Txntypeid          NUMBER := 0;
          v_Txndate            DATE;
          v_Txnqualpurchaseamt FLOAT;
          v_Txnheaderid        NVARCHAR2(50) := '';
          v_Txnemployeeid      NVARCHAR2(25) := '';
          v_Rulename           NVARCHAR2(50) := '';
          v_Cnt                NUMBER := 0;
          v_Sql1               VARCHAR2(100);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txnheader_Wrk
          CURSOR Get_Txn IS
               SELECT Hw.Header_Rowkey,
                      Hw.Vckey,
                      Hw.Txntypeid,
                      Hw.Brandid,
                      Hw.Txndate,
                      Hw.Txnqualpurchaseamt,
                      Nvl(Hw.Txnemployeeid, 0),
                      Hw.Txnheaderid
               FROM   Lw_Txnheader_Wrk4 Hw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
          SELECT TRIM(Rn.a_Rulename)
             FROM   bp_ae.Ats_Rulenames Rn
             WHERE  Rn.a_Rulestatus = 1
                    AND Rn.a_Rulepath = 'H'
             ORDER  BY (CASE
                            WHEN Rn.a_Rulename = 'UpdateTierandNetSpend' THEN
                             0
                            WHEN Rn.a_Rulename = 'AECC_bonus_points' THEN
                             2
                            WHEN Rn.a_Rulename = 'UpdateTierOnly' THEN
                              999
                            ELSE
                             1
                         END);
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'HeaderRules_Processor4';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_HeaderRules4',
                                        v_Processid);
          /*Logging*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Header_Rowkey,
                         v_Vckey,
                         v_Txntypeid,
                         v_Brandid,
                         v_Txndate,
                         v_Txnqualpurchaseamt,
                         v_Txnemployeeid,
                         v_Txnheaderid;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i, :j); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Header_Rowkey, v_Vckey, v_Txntypeid, v_Brandid, v_Txndate, v_Txnqualpurchaseamt, v_Txnemployeeid, v_Txnheaderid, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --     open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess4: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess4</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Headerrulesprocess4;

     PROCEDURE Detailtxnrulesprocess4(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor) IS
          Pdummy              VARCHAR2(2);
          v_Detailitem_Rowkey NUMBER := 0;
          v_Ipcode            NUMBER := 0;
          v_Brandid           NUMBER := 0;
          v_Txntypeid         NUMBER := 0;
          v_Txndate           DATE;
          v_Dtlsaleamount     FLOAT;
          v_Txnheaderid       NVARCHAR2(50) := '';
          v_Rulename          NVARCHAR2(50) := '';
          v_Dtlclasscode      NVARCHAR2(50) := '';
          v_Cnt               NUMBER := 0;
          v_Sql1              VARCHAR2(100);
          v_Bra_Codes         Aetabletype;
          v_Jean_Codes        Aetabletype;
          v_Cclist            VARCHAR2(1000);
          v_Cclist2           VARCHAR2(1000);
          v_Bracdcnt          NUMBER := 0;
          v_Jeancdcnt         NUMBER := 0;
          v_Sql2              VARCHAR2(1000);
          v_Sql3              VARCHAR2(1000);
          v_Sql4              VARCHAR2(1000);
          v_Sql5              VARCHAR2(1000);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txndetailitem_Wrk
          CURSOR Get_Txn IS
               SELECT --dw.rowkey, AEO-51 changes here ---------SCJ
                Dw.Dtl_Rowkey, --AEO-51 changes here ---------SCJ
                Dw.Ipcode,
                Dw.Dtltypeid,
                Dw.Dtlsaleamount,
                Dw.Txndate,
                Dw.Txnheaderid,
                Dw.Dtlclasscode
               FROM   Lw_Txndetailitem_Wrk4 Dw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
               SELECT TRIM(Rn.a_Rulename)
               FROM   Ats_Rulenames Rn
               WHERE  Rn.a_Rulestatus = 1
                      AND Rn.a_Rulepath = 'D'; -- active Detailitem rules
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'DetailItemRules_Processor4';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_DetailitemRules4',
                                        v_Processid);
          /*Logging*/
          /*select count(bc.classcodes) into v_bracdcnt from AE_BRAPROMOCODES bc;
          select count(jc.classcodes) into v_jeancdcnt from AE_JEANSPROMOCODES jc;
          v_sql2 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
          v_sql4 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';
          if (v_bracdcnt = 0)
            then
              EXECUTE IMMEDIATE v_sql2 ;
          end if;
           if (v_jeancdcnt = 0)
            then
            EXECUTE IMMEDIATE v_sql4 ;
          end if;                    */
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Detailitem_Rowkey,
                         v_Ipcode,
                         v_Txntypeid,
                         v_Dtlsaleamount,
                         v_Txndate,
                         v_Txnheaderid,
                         v_Dtlclasscode;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :j ); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Detailitem_Rowkey, v_Ipcode, v_Txntypeid, v_Dtlsaleamount, v_Txndate, v_Txnheaderid, v_Dtlclasscode, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --      open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure DetailTxnRulesProcess4: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DetailTxnRulesProcess4</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Detailtxnrulesprocess4;

     PROCEDURE Headerrulesprocess5(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor) IS
          Pdummy               VARCHAR2(2);
          v_Header_Rowkey      NUMBER := 0;
          v_Vckey              NUMBER := 0;
          v_Brandid            NUMBER := 0;
          v_Txntypeid          NUMBER := 0;
          v_Txndate            DATE;
          v_Txnqualpurchaseamt FLOAT;
          v_Txnheaderid        NVARCHAR2(50) := '';
          v_Txnemployeeid      NVARCHAR2(25) := '';
          v_Rulename           NVARCHAR2(50) := '';
          v_Cnt                NUMBER := 0;
          v_Sql1               VARCHAR2(100);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txnheader_Wrk
          CURSOR Get_Txn IS
               SELECT Hw.Header_Rowkey,
                      Hw.Vckey,
                      Hw.Txntypeid,
                      Hw.Brandid,
                      Hw.Txndate,
                      Hw.Txnqualpurchaseamt,
                      Nvl(Hw.Txnemployeeid, 0),
                      Hw.Txnheaderid
               FROM   Lw_Txnheader_Wrk5 Hw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
          SELECT TRIM(Rn.a_Rulename)
             FROM   bp_ae.Ats_Rulenames Rn
             WHERE  Rn.a_Rulestatus = 1
                    AND Rn.a_Rulepath = 'H'
             ORDER  BY (CASE
                            WHEN Rn.a_Rulename = 'UpdateTierandNetSpend' THEN
                             0
                            WHEN Rn.a_Rulename = 'AECC_bonus_points' THEN
                             2
                            WHEN Rn.a_Rulename = 'UpdateTierOnly' THEN
                              999
                            ELSE
                             1
                         END);
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'HeaderRules_Processor5';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_HeaderRules5',
                                        v_Processid);
          /*Logging*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Header_Rowkey,
                         v_Vckey,
                         v_Txntypeid,
                         v_Brandid,
                         v_Txndate,
                         v_Txnqualpurchaseamt,
                         v_Txnemployeeid,
                         v_Txnheaderid;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i, :j); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Header_Rowkey, v_Vckey, v_Txntypeid, v_Brandid, v_Txndate, v_Txnqualpurchaseamt, v_Txnemployeeid, v_Txnheaderid, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --     open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess5: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess5</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Headerrulesprocess5;

     PROCEDURE Detailtxnrulesprocess5(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor) IS
          Pdummy              VARCHAR2(2);
          v_Detailitem_Rowkey NUMBER := 0;
          v_Ipcode            NUMBER := 0;
          v_Brandid           NUMBER := 0;
          v_Txntypeid         NUMBER := 0;
          v_Txndate           DATE;
          v_Dtlsaleamount     FLOAT;
          v_Txnheaderid       NVARCHAR2(50) := '';
          v_Rulename          NVARCHAR2(50) := '';
          v_Dtlclasscode      NVARCHAR2(50) := '';
          v_Cnt               NUMBER := 0;
          v_Sql1              VARCHAR2(100);
          v_Bra_Codes         Aetabletype;
          v_Jean_Codes        Aetabletype;
          v_Cclist            VARCHAR2(1000);
          v_Cclist2           VARCHAR2(1000);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
          v_Jobdirection     NUMBER := 0;
          v_Starttime        DATE := SYSDATE;
          v_Endtime          DATE;
          v_Messagesreceived NUMBER := 0;
          v_Messagesfailed   NUMBER := 0;
          v_Jobstatus        NUMBER := 0;
          v_Jobname          VARCHAR2(256);
          v_Processid        NUMBER := 0;
          v_Errormessage     VARCHAR2(256);
          v_Bracdcnt         NUMBER := 0;
          v_Jeancdcnt        NUMBER := 0;
          v_Sql2             VARCHAR2(1000);
          v_Sql3             VARCHAR2(1000);
          v_Sql4             VARCHAR2(1000);
          v_Sql5             VARCHAR2(1000);
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txndetailitem_Wrk
          CURSOR Get_Txn IS
               SELECT --dw.rowkey, AEO-51 changes here ---------SCJ
                Dw.Dtl_Rowkey, --AEO-51 changes here ---------SCJ
                Dw.Ipcode,
                Dw.Dtltypeid,
                Dw.Dtlsaleamount,
                Dw.Txndate,
                Dw.Txnheaderid,
                Dw.Dtlclasscode
               FROM   Lw_Txndetailitem_Wrk5 Dw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
               SELECT TRIM(Rn.a_Rulename)
               FROM   Ats_Rulenames Rn
               WHERE  Rn.a_Rulestatus = 1
                      AND Rn.a_Rulepath = 'D'; -- active Detailitem rules
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --   v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'DetailItemRules_Processor5';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_DetailitemRules5',
                                        v_Processid);
          /*Logging*/
          /* select count(bc.classcodes) into v_bracdcnt from AE_BRAPROMOCODES bc;
          select count(jc.classcodes) into v_jeancdcnt from AE_JEANSPROMOCODES jc;
          v_sql2 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
          v_sql4 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';

          if (v_bracdcnt = 0)
            then
              EXECUTE IMMEDIATE v_sql2 ;
          end if;
           if (v_jeancdcnt = 0)
            then
            EXECUTE IMMEDIATE v_sql4 ;
          end if;            */
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Detailitem_Rowkey,
                         v_Ipcode,
                         v_Txntypeid,
                         v_Dtlsaleamount,
                         v_Txndate,
                         v_Txnheaderid,
                         v_Dtlclasscode;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :i, :j); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Detailitem_Rowkey, v_Ipcode, v_Txntypeid, v_Dtlsaleamount, v_Txndate, v_Txnheaderid, v_Dtlclasscode, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --     open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure DetailTxnRulesProcess5: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DetailTxnRulesProcess5</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Detailtxnrulesprocess5;

     PROCEDURE Headerrulesprocess6(p_Dummy       VARCHAR2,
                                   p_Processdate VARCHAR2,
                                   Retval        IN OUT Rcursor) IS
          Pdummy               VARCHAR2(2);
          v_Header_Rowkey      NUMBER := 0;
          v_Vckey              NUMBER := 0;
          v_Brandid            NUMBER := 0;
          v_Txntypeid          NUMBER := 0;
          v_Txndate            DATE;
          v_Txnqualpurchaseamt FLOAT;
          v_Txnheaderid        NVARCHAR2(50) := '';
          v_Txnemployeeid      NVARCHAR2(25) := '';
          v_Rulename           NVARCHAR2(50) := '';
          v_Cnt                NUMBER := 0;
          v_Sql1               VARCHAR2(100);
          v_Cnt                NUMBER := 0;
          v_Processdate        TIMESTAMP;
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txnheader_Wrk
          CURSOR Get_Txn IS
               SELECT Hw.Header_Rowkey,
                      Hw.Vckey,
                      Hw.Txntypeid,
                      Hw.Brandid,
                      Hw.Txndate,
                      Hw.Txnqualpurchaseamt,
                      Nvl(Hw.Txnemployeeid, 0),
                      Hw.Txnheaderid
               FROM   Lw_Txnheader_Wrk6 Hw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
          SELECT TRIM(Rn.a_Rulename)
             FROM   bp_ae.Ats_Rulenames Rn
             WHERE  Rn.a_Rulestatus = 1
                    AND Rn.a_Rulepath = 'H'
             ORDER  BY (CASE
                            WHEN Rn.a_Rulename = 'UpdateTierandNetSpend' THEN
                             0
                            WHEN Rn.a_Rulename = 'AECC_bonus_points' THEN
                             2
                            WHEN Rn.a_Rulename = 'UpdateTierOnly' THEN
                              999
                            ELSE
                             1
                         END);
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'HeaderRules_Processor6';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_HeaderRules6',
                                        v_Processid);
          /*Logging*/
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Header_Rowkey,
                         v_Vckey,
                         v_Txntypeid,
                         v_Brandid,
                         v_Txndate,
                         v_Txnqualpurchaseamt,
                         v_Txnemployeeid,
                         v_Txnheaderid;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :h, :i,:j); END;';
                    --select to_date(p_Processdate, 'YYYYMMddHH24miss') into v_Processdate from dual;
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Header_Rowkey, v_Vckey, v_Txntypeid, v_Brandid, v_Txndate, v_Txnqualpurchaseamt, v_Txnemployeeid, v_Txnheaderid, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g, h)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --     open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure HeaderRulesProcess6: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>HeaderRulesProcess6</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Headerrulesprocess6;

     PROCEDURE Detailtxnrulesprocess6(p_Dummy       VARCHAR2,
                                      p_Processdate VARCHAR2,
                                      Retval        IN OUT Rcursor) IS
          Pdummy              VARCHAR2(2);
          v_Detailitem_Rowkey NUMBER := 0;
          v_Ipcode            NUMBER := 0;
          v_Brandid           NUMBER := 0;
          v_Txntypeid         NUMBER := 0;
          v_Txndate           DATE;
          v_Dtlsaleamount     FLOAT;
          v_Txnheaderid       NVARCHAR2(50) := '';
          v_Rulename          NVARCHAR2(50) := '';
          v_Dtlclasscode      NVARCHAR2(50) := '';
          v_Cnt               NUMBER := 0;
          v_Sql1              VARCHAR2(100);
          v_Bra_Codes         Aetabletype;
          v_Jean_Codes        Aetabletype;
          v_Cclist            VARCHAR2(1000);
          v_Cclist2           VARCHAR2(1000);
          v_Bracdcnt          NUMBER := 0;
          v_Jeancdcnt         NUMBER := 0;
          v_Sql2              VARCHAR2(1000);
          v_Sql3              VARCHAR2(1000);
          v_Sql4              VARCHAR2(1000);
          v_Sql5              VARCHAR2(1000);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1- get the Txns from LW_txndetailitem_Wrk
          CURSOR Get_Txn IS
               SELECT --dw.rowkey, AEO-51 changes here ---------SCJ
                Dw.Dtl_Rowkey, --AEO-51 changes here ---------SCJ
                Dw.Ipcode,
                Dw.Dtltypeid,
                Dw.Dtlsaleamount,
                Dw.Txndate,
                Dw.Txnheaderid,
                Dw.Dtlclasscode
               FROM   Lw_Txndetailitem_Wrk6 Dw;
          --  Cursor Get_Rules(b_brandid number,b_txntypeid number,b_txndate date,b_txnqualpurchaseamt float ) is
          CURSOR Get_Rules IS
               SELECT TRIM(Rn.a_Rulename)
               FROM   Ats_Rulenames Rn
               WHERE  Rn.a_Rulestatus = 1
                      AND Rn.a_Rulepath = 'D'; -- active Detailitem rules
          TYPE Rules_Tab IS TABLE OF Get_Rules%ROWTYPE;
          v_Rulestbl Rules_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          --  v_dap_log_id := utility_pkg.get_LIBJobID();
          v_Jobname   := 'DetailItemRules_Processor6';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'Get_DetailitemRules6',
                                        v_Processid);
          /*Logging*/
          /*  select count(bc.classcodes) into v_bracdcnt from AE_BRAPROMOCODES bc;
          select count(jc.classcodes) into v_jeancdcnt from AE_JEANSPROMOCODES jc;
          v_sql2 := 'insert into AE_BRAPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''BraPromoClassCodes''),'';'') ) t)';
          v_sql4 := 'insert into AE_JEANSPROMOCODES ( select t.COLUMN_VALUE as classcodes  from table(StringToTable((select cc.value  from Lw_Clientconfiguration cc
                                                                                         where cc.key = ''JeansPromoClassCodes''),'';'') ) t)';
           if (v_bracdcnt = 0)
            then
              EXECUTE IMMEDIATE v_sql2 ;
          end if;
           if (v_jeancdcnt = 0)
            then
            EXECUTE IMMEDIATE v_sql4 ;
          end if;                    */
          OPEN Get_Txn;
          LOOP
               FETCH Get_Txn
                    INTO v_Detailitem_Rowkey,
                         v_Ipcode,
                         v_Txntypeid,
                         v_Dtlsaleamount,
                         v_Txndate,
                         v_Txnheaderid,
                         v_Dtlclasscode;
               EXIT WHEN Get_Txn%NOTFOUND;
               OPEN Get_Rules;
               LOOP
                    FETCH Get_Rules
                         INTO v_Rulename;
                    EXIT WHEN Get_Rules%NOTFOUND;
                    v_Sql1 := 'BEGIN RulesProcessing.' || v_Rulename ||
                              '(:a, :b, :c, :d, :e, :f, :g, :i,:j ); END;';
                    EXECUTE IMMEDIATE v_Sql1
                         USING v_Detailitem_Rowkey, v_Ipcode, v_Txntypeid, v_Dtlsaleamount, v_Txndate, v_Txnheaderid, v_Dtlclasscode, v_My_Log_Id, To_Date(p_Processdate, 'YYYYMMddHH24miss'); -- v_rulename(a, b, c, d, e, f, g)
                    v_Messagesreceived := v_Messagesreceived + SQL%ROWCOUNT;
               END LOOP;
               CLOSE Get_Rules;
          END LOOP;
          CLOSE Get_Txn;
          COMMIT;
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
          --     open retval for select v_dap_log_id from dual;
     EXCEPTION
          WHEN OTHERS THEN
               --  IF v_messagesfailed = 0 THEN
               --    v_messagesfailed := 0+1;
               --  END IF;
               v_Messagesfailed := v_Messagesfailed + 1;
               v_Error          := SQLERRM;
               v_Reason         := 'Failed Procedure DetailTxnRulesProcess6: ';
               v_Message        := '<failed>' || Chr(10) || '  <details>' ||
                                   Chr(10) || '    <pkg>RulesProcessing</pkg>' ||
                                   Chr(10) ||
                                   '    <proc>DetailTxnRulesProcess6</proc>' ||
                                   Chr(10) || '  </details>' || Chr(10) ||
                                   '</failed>';
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
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               RAISE;
     END Detailtxnrulesprocess6;

     /*Bonus event returns */
     PROCEDURE Bonuseventreturn(p_Dummy       VARCHAR2,
                                p_Processdate VARCHAR2,
                                Retval        IN OUT Rcursor) IS
          v_Vckey            NUMBER := 0;
          v_Cnt              NUMBER := 0;
          v_Bonuspointtypeid NUMBER;
          v_Pointeventid     NUMBER;
          v_Txnstartdt       DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Processdate TIMESTAMP := To_Date(p_Processdate, 'YYYYMMddHH24miss');
          --cursor1-
          CURSOR Get_Returns IS
               WITH Mem AS
                (SELECT Dw.Txnheaderid,
                        MAX(Hw.Vckey) AS Ret_Vckey,
                        SUM(Dw.Dtlsaleamount)   AS Ret_Saleamount,
                        MAX(Dw.Txndate) AS Ret_Txndate,
                        MAX(Hw.Txnoriginaltxnrowkey) AS Ret_Originalrowkey,
                        MAX(Pt.Pointeventid) AS Ret_Pointeventid,
                        MAX(Hw.Txndate) AS Org_Txndate,
                        MAX(Nvl(Md.a_Extendedplaycode, 0)) AS Extendedplaycode,
                        MAX(v_Processdate) AS p_Processdate
                 FROM   Lw_Txndetailitem_Wrk Dw
                 INNER  JOIN Lw_Txnheader_Wrk Hw
                 ON     Hw.Txnheaderid = Dw.Txnheaderid
                 INNER  JOIN Lw_Pointtransaction Pt
                 ON     Pt.Rowkey = Hw.Txnoriginaltxnrowkey
                 INNER  JOIN Lw_Virtualcard Vc
                 ON     Vc.Vckey = Dw.Ipcode
                 INNER  JOIN Ats_Memberdetails Md
                 ON     Md.a_Ipcode = Vc.Ipcode
                 WHERE  Dw.Dtlsaleamount < 0
                       --  and hw.txnqualpurchaseamt > 0
                        AND Hw.Txnoriginaltxnrowkey > 0
                        AND Pt.Pointtypeid =
                        (SELECT Pointtypeid
                             FROM   Lw_Pointtype
                             WHERE  NAME = 'Bonus Points')
                        AND Pt.Points > 0
                        AND
                        (Dw.Dtlclasscode IN
                        (SELECT t.Column_Value AS Classcode
                          FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                     FROM   Ats_Bonuseventdetail Bd
                                                     WHERE  Bd.a_Pointeventid =
                                                            Pt.Pointeventid),
                                                     ',')) t) OR
                        'ALL' IN (SELECT t.Column_Value AS Classcode
                                   FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid =
                                                                     Pt.Pointeventid),
                                                              ',')) t))
                 GROUP  BY Dw.Txnheaderid)
               SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Get_Returns%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          v_Jobname   := 'BonusEventReturn';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'BonusEventReturn',
                                        v_Processid);
          SELECT Pointtypeid
          INTO   v_Bonuspointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bonus Points';
          -- SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Get_Returns;
          LOOP
               FETCH Get_Returns BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Ret_Vckey,
                          v_Bonuspointtypeid /*Pointtypeid*/,
                          v_Tbl(k).Ret_Pointeventid /*Pointeventid*/,
                          2 /*return*/,
                          v_Tbl(k).Ret_Txndate
                          --  ,SYSDATE
                          ,
                          v_Tbl(k).p_Processdate,
                          CASE WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                    Round((v_Tbl(k).Ret_Saleamount *5)* (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid),1) FROM   Dual),0)
                               ELSE
                                    Round(v_Tbl(k).Ret_Saleamount * (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                                     FROM   Ats_Bonuseventdetail Bd
                                                                     WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid), 1) FROM   Dual),0)
                          END/*Points*/,
                          CASE
                               WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                To_Date('12/31/2199', 'mm/dd/yyyy')
                               ELSE
                                Add_Months(Trunc(v_Tbl(k).Ret_Txndate, 'Q'), 3)
                          END,
                          'Bonus Points Return' /*Notes*/,
                          0,
                          -1,
                          v_Tbl(k).Ret_Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Get_Returns%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Get_Returns%ISOPEN
          THEN
               CLOSE Get_Returns;
          END IF;
          COMMIT;
          --END;
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
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure BonusEventReturn: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Logsource,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
          WHEN OTHERS THEN
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'BonusEventReturn job is not finished');
     END Bonuseventreturn;

     PROCEDURE Bonuseventreturn2(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor) IS
          v_Vckey            NUMBER := 0;
          v_Cnt              NUMBER := 0;
          v_Bonuspointtypeid NUMBER;
          v_Pointeventid     NUMBER;
          v_Txnstartdt       DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Processdate TIMESTAMP := To_Date(p_Processdate, 'YYYYMMddHH24miss');
          --cursor1-
          CURSOR Get_Returns IS
               WITH Mem AS
                (SELECT Dw.Txnheaderid,
                        MAX(Hw.Vckey) AS Ret_Vckey,
                        SUM(Dw.Dtlsaleamount) AS Ret_Saleamount,
                        MAX(Dw.Txndate) AS Ret_Txndate,
                        MAX(Hw.Txnoriginaltxnrowkey) AS Ret_Originalrowkey,
                        MAX(Pt.Pointeventid) AS Ret_Pointeventid,
                        MAX(Hw.Txndate) AS Org_Txndate,
                        MAX(Nvl(Md.a_Extendedplaycode, 0)) AS Extendedplaycode,
                        MAX(v_Processdate) AS p_Processdate
                 FROM   Lw_Txndetailitem_Wrk2 Dw
                 INNER  JOIN Lw_Txnheader_Wrk2 Hw
                 ON     Hw.Txnheaderid = Dw.Txnheaderid
                 INNER  JOIN Lw_Pointtransaction Pt
                 ON     Pt.Rowkey = Hw.Txnoriginaltxnrowkey
                 INNER  JOIN Lw_Virtualcard Vc
                 ON     Vc.Vckey = Dw.Ipcode
                 INNER  JOIN Ats_Memberdetails Md
                 ON     Md.a_Ipcode = Vc.Ipcode
                 WHERE  Dw.Dtlsaleamount < 0
                       --  and hw.txnqualpurchaseamt > 0
                        AND Hw.Txnoriginaltxnrowkey > 0
                        AND Pt.Pointtypeid =
                        (SELECT Pointtypeid
                             FROM   Lw_Pointtype
                             WHERE  NAME = 'Bonus Points')
                        AND Pt.Points > 0
                        AND
                        (Dw.Dtlclasscode IN
                        (SELECT t.Column_Value AS Classcode
                          FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                     FROM   Ats_Bonuseventdetail Bd
                                                     WHERE  Bd.a_Pointeventid =
                                                            Pt.Pointeventid),
                                                     ',')) t) OR
                        'ALL' IN (SELECT t.Column_Value AS Classcode
                                   FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid =
                                                                     Pt.Pointeventid),
                                                              ',')) t))
                 GROUP  BY Dw.Txnheaderid)
               SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Get_Returns%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          v_Jobname   := 'BonusEventReturn2';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'BonusEventReturn2',
                                        v_Processid);
          SELECT Pointtypeid
          INTO   v_Bonuspointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bonus Points';
          --  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Get_Returns;
          LOOP
               FETCH Get_Returns BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                         ,
                          Createdate
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Ret_Vckey,
                          v_Bonuspointtypeid /*Pointtypeid*/,
                          v_Tbl(k).Ret_Pointeventid /*Pointeventid*/,
                          2 /*return*/,
                          v_Tbl(k).Ret_Txndate
                          -- ,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate,
                          CASE WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                    Round((v_Tbl(k).Ret_Saleamount *5)* (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid),1) FROM   Dual),0)
                               ELSE
                                    Round(v_Tbl(k).Ret_Saleamount * (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                                     FROM   Ats_Bonuseventdetail Bd
                                                                     WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid), 1) FROM   Dual),0)
                          END/*Points*/,

                          CASE
                               WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                To_Date('12/31/2199', 'mm/dd/yyyy')
                               ELSE
                                Add_Months(Trunc(v_Tbl(k).Ret_Txndate, 'Q'), 3)
                          END,
                          'Bonus Points Return' /*Notes*/,
                          0,
                          -1,
                          v_Tbl(k).Ret_Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Get_Returns%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Get_Returns%ISOPEN
          THEN
               CLOSE Get_Returns;
          END IF;
          COMMIT;
          --END;
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
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure BonusEventReturn2: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Logsource,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
          WHEN OTHERS THEN
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'BonusEventReturn2 job is not finished');
     END Bonuseventreturn2;

     PROCEDURE Bonuseventreturn3(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor) IS
          v_Vckey            NUMBER := 0;
          v_Cnt              NUMBER := 0;
          v_Bonuspointtypeid NUMBER;
          v_Pointeventid     NUMBER;
          v_Txnstartdt       DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Processdate TIMESTAMP := To_Date(p_Processdate, 'YYYYMMddHH24miss');
          --cursor1-
          CURSOR Get_Returns IS
               WITH Mem AS
                (SELECT Dw.Txnheaderid,
                        MAX(Hw.Vckey) AS Ret_Vckey,
                        SUM(Dw.Dtlsaleamount) AS Ret_Saleamount,
                        MAX(Dw.Txndate) AS Ret_Txndate,
                        MAX(Hw.Txnoriginaltxnrowkey) AS Ret_Originalrowkey,
                        MAX(Pt.Pointeventid) AS Ret_Pointeventid,
                        MAX(Hw.Txndate) AS Org_Txndate,
                        MAX(Nvl(Md.a_Extendedplaycode, 0)) AS Extendedplaycode,
                        MAX(v_Processdate) AS p_Processdate
                 FROM   Lw_Txndetailitem_Wrk3 Dw
                 INNER  JOIN Lw_Txnheader_Wrk3 Hw
                 ON     Hw.Txnheaderid = Dw.Txnheaderid
                 INNER  JOIN Lw_Pointtransaction Pt
                 ON     Pt.Rowkey = Hw.Txnoriginaltxnrowkey
                 INNER  JOIN Lw_Virtualcard Vc
                 ON     Vc.Vckey = Dw.Ipcode
                 INNER  JOIN Ats_Memberdetails Md
                 ON     Md.a_Ipcode = Vc.Ipcode
                 WHERE  Dw.Dtlsaleamount < 0
                       --  and hw.txnqualpurchaseamt > 0
                        AND Hw.Txnoriginaltxnrowkey > 0
                        AND Pt.Pointtypeid =
                        (SELECT Pointtypeid
                             FROM   Lw_Pointtype
                             WHERE  NAME = 'Bonus Points')
                        AND Pt.Points > 0
                        AND
                        (Dw.Dtlclasscode IN
                        (SELECT t.Column_Value AS Classcode
                          FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                     FROM   Ats_Bonuseventdetail Bd
                                                     WHERE  Bd.a_Pointeventid =
                                                            Pt.Pointeventid),
                                                     ',')) t) OR
                        'ALL' IN (SELECT t.Column_Value AS Classcode
                                   FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid =
                                                                     Pt.Pointeventid),
                                                              ',')) t))
                 GROUP  BY Dw.Txnheaderid)
               SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Get_Returns%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          v_Jobname   := 'BonusEventReturn3';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'BonusEventReturn3',
                                        v_Processid);
          SELECT Pointtypeid
          INTO   v_Bonuspointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bonus Points';
          --  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Get_Returns;
          LOOP
               FETCH Get_Returns BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                         ,
                          Createdate
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Ret_Vckey,
                          v_Bonuspointtypeid /*Pointtypeid*/,
                          v_Tbl(k).Ret_Pointeventid /*Pointeventid*/,
                          2 /*return*/,
                          v_Tbl(k).Ret_Txndate
                          -- ,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate,
                          CASE WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                    Round((v_Tbl(k).Ret_Saleamount *5)* (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid),1) FROM   Dual),0)
                               ELSE
                                    Round(v_Tbl(k).Ret_Saleamount * (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                                     FROM   Ats_Bonuseventdetail Bd
                                                                     WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid), 1) FROM   Dual),0)
                          END/*Points*/,
                          CASE
                               WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                To_Date('12/31/2199', 'mm/dd/yyyy')
                               ELSE
                                Add_Months(Trunc(v_Tbl(k).Ret_Txndate, 'Q'), 3)
                          END,
                          'Bonus Points Return' /*Notes*/,
                          0,
                          -1,
                          v_Tbl(k).Ret_Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Get_Returns%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Get_Returns%ISOPEN
          THEN
               CLOSE Get_Returns;
          END IF;
          COMMIT;
          --END;
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
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure BonusEventReturn3: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Logsource,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Messagesfailed := v_Messagesfailed + 1;
          WHEN OTHERS THEN
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'BonusEventReturn3 job is not finished');
     END Bonuseventreturn3;

     PROCEDURE Bonuseventreturn4(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor) IS
          v_Vckey            NUMBER := 0;
          v_Cnt              NUMBER := 0;
          v_Bonuspointtypeid NUMBER;
          v_Pointeventid     NUMBER;
          v_Txnstartdt       DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Processdate TIMESTAMP := To_Date(p_Processdate, 'YYYYMMddHH24miss');
          --cursor1-
          CURSOR Get_Returns IS
               WITH Mem AS
                (SELECT Dw.Txnheaderid,
                        MAX(Hw.Vckey) AS Ret_Vckey,
                        SUM(Dw.Dtlsaleamount) AS Ret_Saleamount,
                        MAX(Dw.Txndate) AS Ret_Txndate,
                        MAX(Hw.Txnoriginaltxnrowkey) AS Ret_Originalrowkey,
                        MAX(Pt.Pointeventid) AS Ret_Pointeventid,
                        MAX(Hw.Txndate) AS Org_Txndate,
                        MAX(Nvl(Md.a_Extendedplaycode, 0)) AS Extendedplaycode,
                        MAX(v_Processdate) AS p_Processdate
                 FROM   Lw_Txndetailitem_Wrk4 Dw
                 INNER  JOIN Lw_Txnheader_Wrk4 Hw
                 ON     Hw.Txnheaderid = Dw.Txnheaderid
                 INNER  JOIN Lw_Pointtransaction Pt
                 ON     Pt.Rowkey = Hw.Txnoriginaltxnrowkey
                 INNER  JOIN Lw_Virtualcard Vc
                 ON     Vc.Vckey = Dw.Ipcode
                 INNER  JOIN Ats_Memberdetails Md
                 ON     Md.a_Ipcode = Vc.Ipcode
                 WHERE  Dw.Dtlsaleamount < 0
                        AND Hw.Txnoriginaltxnrowkey > 0
                       --   and hw.txnqualpurchaseamt > 0
                        AND Pt.Pointtypeid =
                        (SELECT Pointtypeid
                             FROM   Lw_Pointtype
                             WHERE  NAME = 'Bonus Points')
                        AND Pt.Points > 0
                        AND
                        (Dw.Dtlclasscode IN
                        (SELECT t.Column_Value AS Classcode
                          FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                     FROM   Ats_Bonuseventdetail Bd
                                                     WHERE  Bd.a_Pointeventid =
                                                            Pt.Pointeventid),
                                                     ',')) t) OR
                        'ALL' IN (SELECT t.Column_Value AS Classcode
                                   FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid =
                                                                     Pt.Pointeventid),
                                                              ',')) t))
                 GROUP  BY Dw.Txnheaderid)
               SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Get_Returns%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          v_Jobname   := 'BonusEventReturn4';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'BonusEventReturn4',
                                        v_Processid);
          SELECT Pointtypeid
          INTO   v_Bonuspointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bonus Points';
          --  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Get_Returns;
          LOOP
               FETCH Get_Returns BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                         ,
                          Createdate
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Ret_Vckey,
                          v_Bonuspointtypeid /*Pointtypeid*/,
                          v_Tbl(k).Ret_Pointeventid /*Pointeventid*/,
                          2 /*return*/,
                          v_Tbl(k).Ret_Txndate
                          --         ,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate,
                         CASE WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                    Round((v_Tbl(k).Ret_Saleamount *5)* (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid),1) FROM   Dual),0)
                               ELSE
                                    Round(v_Tbl(k).Ret_Saleamount * (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                                     FROM   Ats_Bonuseventdetail Bd
                                                                     WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid), 1) FROM   Dual),0)
                          END/*Points*/,
                          CASE
                               WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                To_Date('12/31/2199', 'mm/dd/yyyy')
                               ELSE
                                Add_Months(Trunc(v_Tbl(k).Ret_Txndate, 'Q'), 3)
                          END,
                          'Bonus Points Return' /*Notes*/,
                          0,
                          -1,
                          v_Tbl(k).Ret_Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Get_Returns%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Get_Returns%ISOPEN
          THEN
               CLOSE Get_Returns;
          END IF;
          COMMIT;
          --END;
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
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure BonusEventReturn4: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Logsource,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
          WHEN OTHERS THEN
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'BonusEventReturn4 job is not finished');
     END Bonuseventreturn4;

     PROCEDURE Bonuseventreturn5(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor) IS
          v_Vckey            NUMBER := 0;
          v_Cnt              NUMBER := 0;
          v_Bonuspointtypeid NUMBER;
          v_Pointeventid     NUMBER;
          v_Txnstartdt       DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Processdate TIMESTAMP := To_Date(p_Processdate, 'YYYYMMddHH24miss');
          --cursor1-
          CURSOR Get_Returns IS
               WITH Mem AS
                (SELECT Dw.Txnheaderid,
                        MAX(Hw.Vckey) AS Ret_Vckey,
                        SUM(Dw.Dtlsaleamount) AS Ret_Saleamount,
                        MAX(Dw.Txndate) AS Ret_Txndate,
                        MAX(Hw.Txnoriginaltxnrowkey) AS Ret_Originalrowkey,
                        MAX(Pt.Pointeventid) AS Ret_Pointeventid,
                        MAX(Hw.Txndate) AS Org_Txndate,
                        MAX(Nvl(Md.a_Extendedplaycode, 0)) AS Extendedplaycode,
                        MAX(v_Processdate) AS p_Processdate
                 FROM   Lw_Txndetailitem_Wrk5 Dw
                 INNER  JOIN Lw_Txnheader_Wrk5 Hw
                 ON     Hw.Txnheaderid = Dw.Txnheaderid
                 INNER  JOIN Lw_Pointtransaction Pt
                 ON     Pt.Rowkey = Hw.Txnoriginaltxnrowkey
                 INNER  JOIN Lw_Virtualcard Vc
                 ON     Vc.Vckey = Dw.Ipcode
                 INNER  JOIN Ats_Memberdetails Md
                 ON     Md.a_Ipcode = Vc.Ipcode
                 WHERE  Dw.Dtlsaleamount < 0
                       --   and hw.txnqualpurchaseamt > 0
                        AND Hw.Txnoriginaltxnrowkey > 0
                        AND Pt.Pointtypeid =
                        (SELECT Pointtypeid
                             FROM   Lw_Pointtype
                             WHERE  NAME = 'Bonus Points')
                        AND Pt.Points > 0
                        AND
                        (Dw.Dtlclasscode IN
                        (SELECT t.Column_Value AS Classcode
                          FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                     FROM   Ats_Bonuseventdetail Bd
                                                     WHERE  Bd.a_Pointeventid =
                                                            Pt.Pointeventid),
                                                     ',')) t) OR
                        'ALL' IN (SELECT t.Column_Value AS Classcode
                                   FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid =
                                                                     Pt.Pointeventid),
                                                              ',')) t))
                 GROUP  BY Dw.Txnheaderid)
               SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Get_Returns%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          v_Jobname   := 'BonusEventReturn5';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'BonusEventReturn5',
                                        v_Processid);
          SELECT Pointtypeid
          INTO   v_Bonuspointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bonus Points';
          --  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Get_Returns;
          LOOP
               FETCH Get_Returns BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                         ,
                          Createdate
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Ret_Vckey,
                          v_Bonuspointtypeid /*Pointtypeid*/,
                          v_Tbl(k).Ret_Pointeventid /*Pointeventid*/,
                          2 /*return*/,
                          v_Tbl(k).Ret_Txndate
                          --,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate,
                          CASE WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                    Round((v_Tbl(k).Ret_Saleamount *5)* (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid),1) FROM   Dual),0)
                               ELSE
                                    Round(v_Tbl(k).Ret_Saleamount * (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                                     FROM   Ats_Bonuseventdetail Bd
                                                                     WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid), 1) FROM   Dual),0)
                          END/*Points*/,
                          CASE
                               WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                To_Date('12/31/2199', 'mm/dd/yyyy')
                               ELSE
                                Add_Months(Trunc(v_Tbl(k).Ret_Txndate, 'Q'), 3)
                          END,
                          'Bonus Points Return' /*Notes*/,
                          0,
                          -1,
                          v_Tbl(k).Ret_Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Get_Returns%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Get_Returns%ISOPEN
          THEN
               CLOSE Get_Returns;
          END IF;
          COMMIT;
          --END;
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
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure BonusEventReturn5: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Logsource,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
          WHEN OTHERS THEN
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'BonusEventReturn5 job is not finished');
     END Bonuseventreturn5;

     PROCEDURE Bonuseventreturn6(p_Dummy       VARCHAR2,
                                 p_Processdate VARCHAR2,
                                 Retval        IN OUT Rcursor) IS
          v_Vckey            NUMBER := 0;
          v_Cnt              NUMBER := 0;
          v_Bonuspointtypeid NUMBER;
          v_Pointeventid     NUMBER;
          v_Txnstartdt       DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          v_My_Log_Id        NUMBER;
          v_Dap_Log_Id       NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Processdate TIMESTAMP := To_Date(p_Processdate, 'YYYYMMddHH24miss');
          --cursor1-
          CURSOR Get_Returns IS
               WITH Mem AS
                (SELECT Dw.Txnheaderid,
                        MAX(Hw.Vckey) AS Ret_Vckey,
                        SUM(Dw.Dtlsaleamount) AS Ret_Saleamount,
                        MAX(Dw.Txndate) AS Ret_Txndate,
                        MAX(Hw.Txnoriginaltxnrowkey) AS Ret_Originalrowkey,
                        MAX(Pt.Pointeventid) AS Ret_Pointeventid,
                        MAX(Hw.Txndate) AS Org_Txndate,
                        MAX(Nvl(Md.a_Extendedplaycode, 0)) AS Extendedplaycode,
                        MAX(v_Processdate) AS p_Processdate
                 FROM   Lw_Txndetailitem_Wrk6 Dw
                 INNER  JOIN Lw_Txnheader_Wrk6 Hw
                 ON     Hw.Txnheaderid = Dw.Txnheaderid
                 INNER  JOIN Lw_Pointtransaction Pt
                 ON     Pt.Rowkey = Hw.Txnoriginaltxnrowkey
                 INNER  JOIN Lw_Virtualcard Vc
                 ON     Vc.Vckey = Dw.Ipcode
                 INNER  JOIN Ats_Memberdetails Md
                 ON     Md.a_Ipcode = Vc.Ipcode
                 WHERE  Dw.Dtlsaleamount < 0
                       --  and hw.txnqualpurchaseamt > 0
                        AND Hw.Txnoriginaltxnrowkey > 0
                        AND Pt.Pointtypeid =
                        (SELECT Pointtypeid
                             FROM   Lw_Pointtype
                             WHERE  NAME = 'Bonus Points')
                        AND Pt.Points > 0
                        AND
                        (Dw.Dtlclasscode IN
                        (SELECT t.Column_Value AS Classcode
                          FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                     FROM   Ats_Bonuseventdetail Bd
                                                     WHERE  Bd.a_Pointeventid =
                                                            Pt.Pointeventid),
                                                     ',')) t) OR
                        'ALL' IN (SELECT t.Column_Value AS Classcode
                                   FROM   TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid =
                                                                     Pt.Pointeventid),
                                                              ',')) t))
                 GROUP  BY Dw.Txnheaderid)
               SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Get_Returns%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          /*Logging*/
          v_My_Log_Id := Utility_Pkg.Get_Libjobid();
          v_Jobname   := 'BonusEventReturn6';
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
          Utility_Pkg.Log_Process_Start(v_Jobname,
                                        'BonusEventReturn6',
                                        v_Processid);
          SELECT Pointtypeid
          INTO   v_Bonuspointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bonus Points';
          --  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Get_Returns;
          LOOP
               FETCH Get_Returns BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                         ,
                          Createdate
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Ret_Vckey,
                          v_Bonuspointtypeid /*Pointtypeid*/,
                          v_Tbl(k).Ret_Pointeventid /*Pointeventid*/,
                          2 /*return*/,
                          v_Tbl(k).Ret_Txndate
                          --,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate,
                        CASE WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                    Round((v_Tbl(k).Ret_Saleamount *5)* (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                              FROM   Ats_Bonuseventdetail Bd
                                                              WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid),1) FROM   Dual),0)
                               ELSE
                                    Round(v_Tbl(k).Ret_Saleamount * (SELECT Nvl((SELECT Bd.a_Eventmultiplier
                                                                     FROM   Ats_Bonuseventdetail Bd
                                                                     WHERE  Bd.a_Pointeventid = v_Tbl(k).Ret_Pointeventid), 1) FROM   Dual),0)
                          END/*Points*/,
                          CASE
                               WHEN (Ae_Isinpilot(v_Tbl(k).Extendedplaycode) = 1)
                                    AND v_Tbl(k).Org_Txndate >= v_Txnstartdt THEN
                                To_Date('12/31/2199', 'mm/dd/yyyy')
                               ELSE
                                Add_Months(Trunc(v_Tbl(k).Ret_Txndate, 'Q'), 3)
                          END,
                          'Bonus Points Return' /*Notes*/,
                          0,
                          -1,
                          v_Tbl(k).Ret_Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Get_Returns%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Get_Returns%ISOPEN
          THEN
               CLOSE Get_Returns;
          END IF;
          COMMIT;
          --END;
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
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure BonusEventReturn6: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ret_Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => v_Logsource,
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
          WHEN OTHERS THEN
               v_Messagesfailed := v_Messagesfailed + 1;
               Utility_Pkg.Log_Job(p_Job              => v_My_Log_Id,
                                   p_Jobdirection     => v_Jobdirection,
                                   p_Filename         => NULL,
                                   p_Starttime        => v_Starttime,
                                   p_Endtime          => v_Endtime,
                                   p_Messagesreceived => v_Messagesreceived,
                                   p_Messagesfailed   => v_Messagesfailed,
                                   p_Jobstatus        => v_Jobstatus,
                                   p_Jobname          => v_Jobname);
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => v_Logsource,
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'BonusEventReturn6 job is not finished');
     END Bonuseventreturn6;

     PROCEDURE Ae_Basic_Purchase(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS

               SELECT v_Header_Rowkey AS Originalrowkey,
                      v_Vckey AS Vckey,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                      /*CASE
                        WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) AND
                             v_Txndate >= v_Txnstartdt THEN
                         5
                        ELSE
                         1
                      END */ 10 AS Pointmultiplier,
                      /*CASE
                        WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) AND
                             v_Txndate >= v_Txnstartdt THEN
                         To_Date('12/31/2199', 'mm/dd/yyyy')
                        ELSE
                         Add_Months(Trunc(v_Txndate, 'Q'), 3)
                      END */   To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      Nvl(Md.a_Extendedplaycode, 0) AS Extendedplaycode,
                      p_Processdate,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             v_Txnqualpurchaseamt > 0 then
                         1
                        else
                         0
                      End as v_onhold --Pointsonhold flag
                 FROM bp_ae.Lw_Virtualcard Vc
                INNER JOIN bp_ae.Ats_Memberdetails Md
                   ON Md.a_Ipcode = Vc.Ipcode
                WHERE 1 = 1
                  AND Vc.Vckey = v_Vckey
                  AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                  AND (v_Brandid = 1 OR v_Brandid = 3 OR v_Brandid = 4 OR
                      v_Brandid = 5);
          /*redesign changes ends here  SCJ  */
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Basic Points';
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AE Purchase';
          -- SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
         OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
           FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END,
                          v_Pointeventid /*pointeventid=AE Purchase*/,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'AE_Basic_Purchase', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          returning Pointtransactionid into v_Pointtransactionid;

                          INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END,
                          v_Pointeventid /*pointeventid=AE Purchase*/,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'AE_Basic_Purchase', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                Commit;
               elsif (v_Tbl(k).v_onhold = 0) then
               INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END,
                          --v_pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=AE Purchase*/,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate
                          -- SYSDATE
                         ,
                          v_Tbl(k).p_Processdate
                          -- Round(v_tbl(k).txnqualpurchaseamt, 0) /*Points*/,
                         ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier /*Points*/,
                          v_Tbl(k).Expirationdate --new expiration date
                         ,
                          'AE_Basic_Purchase' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;

     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AE_Basic_Purchase: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AE_Basic_Purchase',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AE_Basic_Purchase',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AE_Basic_Purchase ');
     END Ae_Basic_Purchase;

     PROCEDURE Ae_Basic_Return(v_Header_Rowkey      NUMBER,
                               v_Vckey              NUMBER,
                               v_Txntypeid          NUMBER,
                               v_Brandid            NUMBER,
                               v_Txndate            DATE,
                               v_Txnqualpurchaseamt FLOAT,
                               v_Txnemployeeid      NVARCHAR2,
                               v_Txnheaderid        NVARCHAR2,
                               v_My_Log_Id          NUMBER,
                               p_Processdate        TIMESTAMP) IS
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_Txnstartdt           DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;

          --cursor1-
          CURSOR Process_Rule IS
          --  CURSOR process_rule IS with Mem as
          --- /*redesign changes begin here  SCJ  */

               SELECT v_Header_Rowkey AS Originalrowkey,
                      v_Vckey AS Vckey,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                     /* CASE
                           WHEN v_Txnqualpurchaseamt > 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND v_Txndate >= v_Txnstartdt THEN
                            5
                           WHEN v_Txnqualpurchaseamt < 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND (SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk2
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk3
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk4
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk5
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk6
                                     WHERE  Txnheaderid = v_Txnheaderid) >=
                                v_Txnstartdt THEN
                            5
                           ELSE
                            1
                      END */ 10 AS Pointmultiplier,
                      /*CASE
                           WHEN v_Txnqualpurchaseamt > 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND v_Txndate >= v_Txnstartdt THEN
                            To_Date('12/31/2199', 'mm/dd/yyyy')
                           WHEN v_Txnqualpurchaseamt < 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND (SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk2
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk3
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk4
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk5
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk6
                                     WHERE  Txnheaderid = v_Txnheaderid) >=
                                v_Txnstartdt THEN
                            To_Date('12/31/2199', 'mm/dd/yyyy')
                           ELSE
                            Add_Months(Trunc(v_Txndate, 'Q'), 3)
                      END */  To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      Nvl(Md.a_Extendedplaycode, 0) AS Extendedplaycode,
                      p_Processdate
               FROM   bp_ae.Lw_Virtualcard Vc
               INNER  JOIN bp_ae.Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               WHERE  1 = 1
                      AND Vc.Vckey = v_Vckey
                      AND v_Txntypeid = 2
                      AND (v_Brandid = 1 OR v_Brandid = 3 OR v_Brandid = 4 OR
                      v_Brandid = 5);
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Basic Points';
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AE Return';
          -- SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
                 FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                         ,
                          Createdate
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END
                          --,v_pointtypeid /*Pointtypeid*/
                         ,
                          v_Pointeventid /*pointeventid=AE Return*/,
                          2 /*return*/,
                          v_Tbl(k).Txndate
                          --,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate
                          -- Round(v_tbl(k).txnqualpurchaseamt, 0) /*Points*/,
                         ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Points*/
                         ,
                          v_Tbl(k).Expirationdate --new expiration date
                         ,
                          'AE_Basic_Return' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  ;
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AE_Basic_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AE_Basic_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AE_Basic_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AE_Basic_Return ');
     END Ae_Basic_Return;

     PROCEDURE Aerie_Basic_Purchase(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
            v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS
          --  CURSOR process_rule IS with Mem as
          -- (
          --- /*redesign changes begin here  SCJ  */

               SELECT v_Header_Rowkey AS Originalrowkey,
                      v_Vckey AS Vckey,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                     /* CASE
                           WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND v_Txndate >= v_Txnstartdt THEN
                            5
                           ELSE
                            1
                      END */ 10 AS Pointmultiplier,
                      /*CASE
                           WHEN (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND v_Txndate >= v_Txnstartdt THEN
                            To_Date('12/31/2199', 'mm/dd/yyyy')
                           ELSE
                            Add_Months(Trunc(v_Txndate, 'Q'), 3)
                      END */ To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      Nvl(Md.a_Extendedplaycode, 0) AS Extendedplaycode,
                      p_Processdate,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             v_Txnqualpurchaseamt > 0 then
                         1
                        else
                         0
                      End as v_onhold --Pointsonhold flag
               FROM   bp_ae.Lw_Virtualcard Vc
               INNER  JOIN bp_ae.Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               WHERE  1 = 1
                      AND Vc.Vckey = v_Vckey
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                      AND (v_Brandid = 2);
          --  )
          --    SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Basic Points';
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Aerie Purchase';
          -- SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                         ,
                          Createdate
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END
                          --         ,v_pointtypeid /*Pointtypeid*/
                         ,
                          v_Pointeventid /*pointeventid=AE Purchase*/,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate
                          -- ,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate
                          -- Round(v_tbl(k).txnqualpurchaseamt, 0) /*Points*/,
                         ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Points*/
                         ,
                          v_Tbl(k).Expirationdate --new expiration date
                         ,
                          'AERIE_Basic_Purchase' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END,
                          v_Pointeventid /*pointeventid=AE Purchase*/,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                           'AERIE_Basic_Purchase', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                Commit;
               elsif (v_Tbl(k).v_onhold = 0) then
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                         ,
                          Createdate
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END
                          --         ,v_pointtypeid /*Pointtypeid*/
                         ,
                          v_Pointeventid /*pointeventid=AE Purchase*/,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate
                          -- ,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate
                          -- Round(v_tbl(k).txnqualpurchaseamt, 0) /*Points*/,
                         ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Points*/
                         ,
                          v_Tbl(k).Expirationdate --new expiration date
                         ,
                          'AERIE_Basic_Purchase' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AERIE_Basic_Purchase: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AERIE_Basic_Purchase',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AERIE_Basic_Purchase',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AERIE_Basic_Purchase ');
     END Aerie_Basic_Purchase;

 PROCEDURE Aerie_Basic_Return(v_Header_Rowkey      NUMBER,
                                  v_Vckey              NUMBER,
                                  v_Txntypeid          NUMBER,
                                  v_Brandid            NUMBER,
                                  v_Txndate            DATE,
                                  v_Txnqualpurchaseamt FLOAT,
                                  v_Txnemployeeid      NVARCHAR2,
                                  v_Txnheaderid        NVARCHAR2,
                                  v_My_Log_Id          NUMBER,
                                  p_Processdate        TIMESTAMP) IS
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_Txnstartdt           DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;

          --cursor1-
          CURSOR Process_Rule IS
          --  CURSOR process_rule IS with Mem as
          -- (
          --  )
               SELECT v_Header_Rowkey AS Originalrowkey,
                      v_Vckey AS Vckey,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                      /*CASE
                           WHEN v_Txnqualpurchaseamt > 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND v_Txndate >= v_Txnstartdt THEN
                            5
                           WHEN v_Txnqualpurchaseamt < 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND (SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk2
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk3
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk4
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk5
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk6
                                     WHERE  Txnheaderid = v_Txnheaderid) >=
                                v_Txnstartdt THEN
                            5
                           ELSE
                            1
                      END */ 10 AS Pointmultiplier,
                      /*CASE
                           WHEN v_Txnqualpurchaseamt > 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND v_Txndate >= v_Txnstartdt THEN
                            To_Date('12/31/2199', 'mm/dd/yyyy')
                           WHEN v_Txnqualpurchaseamt < 0
                                AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                                AND (SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk2
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk3
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk4
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk5
                                     WHERE  Txnheaderid = v_Txnheaderid
                                     UNION ALL
                                     SELECT Txnoriginaltxndate
                                     FROM   Lw_Txnheader_Wrk6
                                     WHERE  Txnheaderid = v_Txnheaderid) >=
                                v_Txnstartdt THEN
                            To_Date('12/31/2199', 'mm/dd/yyyy')
                           ELSE
                            Add_Months(Trunc(v_Txndate, 'Q'), 3)
                      END */ To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      p_Processdate
               FROM   bp_ae.Lw_Virtualcard Vc
               INNER  JOIN bp_ae.Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               WHERE  1 = 1
                      AND Vc.Vckey = v_Vckey
                      AND v_Txntypeid = 2
                      AND v_Brandid = 2;
          --    SELECT * FROM Mem;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Basic Points';
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Aerie Return';
          --  SELECT to_date(to_char(value), 'mm/dd/yyyy') INTO v_TxnStartDT FROM  lw_clientconfiguration cc WHERE  cc.key = 'CalculateTransactionStartDate';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
                 FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                      INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                         ,
                          Createdate
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                         ,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          CASE
                               WHEN v_Tbl(k).Pointmultiplier = 10 THEN
                                v_Pointtypeid_Redesign
                               ELSE
                                v_Pointtypeid
                          END
                          --         ,v_pointtypeid /*Pointtypeid*/
                         ,
                          v_Pointeventid /*pointeventid=AE Return*/,
                          2 /*return*/,
                          v_Tbl(k).Txndate
                          --,SYSDATE
                         ,
                          v_Tbl(k).p_Processdate
                          -- Round(v_tbl(k).txnqualpurchaseamt, 0) /*Points*/,
                         ,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier /*Points*/
                          /*,case when v_tbl(k).txndate >= v_TxnStartDT
                               and  (select nvl(md.a_extendedplaycode,0)
                                     from lw_virtualcard vc
                                     inner join ats_memberdetails md on md.a_ipcode = vc.ipcode
                                     where vc.vckey = v_tbl(k).vckey) = 1
                                     then  Add_Months(Trunc(v_tbl(k).txndate, 'Y'), 12) + 1
                              else add_months(trunc(v_tbl(k).txndate,'Q'),3)
                          end */,
                          v_Tbl(k).Expirationdate --new expiration date
                         ,
                          'AERIE_Basic_Return' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AERIE_Basic_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AERIE_Basic_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AERIE_Basic_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AERIE_Basic_Return ');
     END Aerie_Basic_Return;


     /*
     *********************************************************************************************************
     ** Rule for processing Txns from Synchrony (AE Credit Card Company) with purchases from members using
     ** the AE Visa Card
     *********************************************************************************************************
     */
     PROCEDURE Worldspend_Purchase(v_Header_Rowkey      NUMBER,
                                   v_Vckey              NUMBER,
                                   v_Txntypeid          NUMBER,
                                   v_Brandid            NUMBER,
                                   v_Txndate            DATE,
                                   v_Txnqualpurchaseamt FLOAT,
                                   v_Txnemployeeid      NVARCHAR2,
                                   v_Txnheaderid        NVARCHAR2,
                                   v_My_Log_Id          NUMBER,
                                   p_Processdate        TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Txnstartdt   DATE;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || v_Vckey || ', ROWKEY: ' || v_Header_Rowkey;
          v_Reason      VARCHAR2(256) := 'Failed Worldspend_Purchase';
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
           v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT v_Header_Rowkey AS Originalrowkey,
                      v_Vckey AS Vckey,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                      5 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      p_Processdate
               FROM   bp_ae.Lw_Virtualcard Vc
               INNER  JOIN bp_ae.Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               WHERE  1 = 1
                      AND Vc.Vckey = v_Vckey
                      AND (v_Txntypeid = 6);
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Visa Card Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AEO Visa Card World Purchase';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'CalculateTransactionStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_tbl.count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=AEO Visa Card World Purchase*/,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate
                          -- SYSDATE
                         ,
                          v_Tbl(k).p_Processdate,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier /*Points*/,
                          v_Tbl(k).Expirationdate --new expiration date
                         ,
                          'WorldSpend_Purchase' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);

               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
          END LOOP;
          COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure WorldSpend_Purchase: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'WorldSpend_Purchase',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'WorldSpend_Purchase',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in WorldSpend_Purchase ');
     END Worldspend_Purchase;


     PROCEDURE Bra_Redemption(v_Detailitem_Rowkey NUMBER,
                              v_Ipcode            NUMBER,
                              v_Txntypeid         NUMBER,
                              v_Dtlsaleamount     FLOAT,
                              v_Txndate           DATE,
                              v_Txnheaderid       NVARCHAR2,
                              v_Dtlclasscode      NVARCHAR2,
                              v_My_Log_Id         NUMBER,
                              p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;

          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               --  from ats_memberdetails md
               --  inner join lw_virtualcard vc on vc.ipcode = md.a_ipcode and vc.isprimary = 1 and rownum =1
               --  inner join (select COLUMN_VALUE  from table(v_bra_codes))cc on cc.COLUMN_VALUE = v_dtlclasscode
               INNER  JOIN Ae_Brapromocodes Bp
               ON     Bp.Classcodes = v_Dtlclasscode
               WHERE  1 = 1
                     --  and md.a_ipcode = v_ipcode
                      AND Nvl(Md.a_Employeecode, 0) = 0
                      AND v_Dtlsaleamount = .01
                      AND v_Txntypeid = 1;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Redemptions';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Bra Redemptions';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Redemptions*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                         Add_Months(Trunc(v_Tbl(k).Txndate, 'Y'), 12) + 21,
                                  -- 01/02/2015
                          'Bra Redemptions' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Bra_Redemption: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Bra_Redemption',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Bra_Redemption',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Bra_Redemption ');
     END Bra_Redemption;

     PROCEDURE Bra_Employee(v_Detailitem_Rowkey NUMBER,
                            v_Ipcode            NUMBER,
                            v_Txntypeid         NUMBER,
                            v_Dtlsaleamount     FLOAT,
                            v_Txndate           DATE,
                            v_Txnheaderid       NVARCHAR2,
                            v_Dtlclasscode      NVARCHAR2,
                            v_My_Log_Id         NUMBER,
                            p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               --  from ats_memberdetails md
               --  inner join lw_virtualcard vc on vc.ipcode = md.a_ipcode and vc.isprimary = 1 and rownum =1
               --  inner join (select COLUMN_VALUE  from table(v_bra_codes))cc on cc.COLUMN_VALUE = v_dtlclasscode
               INNER  JOIN Ae_Brapromocodes Bp
               ON     Bp.Classcodes = v_Dtlclasscode
               WHERE  1 = 1
                     --  and md.a_ipcode = v_ipcode
                      AND (Nvl(Md.a_Employeecode, 0) = 1 OR
                      Nvl(Md.a_Employeecode, 0) = 2)
                      AND v_Dtlsaleamount > .01
                      AND v_Txntypeid = 1;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Employee Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Bra Employee';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          Add_Months(Trunc(v_Tbl(k).Txndate, 'Y'), 12) + 21, -- 01/02/2015
                          'Bra Employee' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Bra_Employee: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Bra_Employee',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Bra_Employee',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Bra_Employee ');
     END Bra_Employee;

     PROCEDURE Bra_Employee_Return(v_Detailitem_Rowkey NUMBER,
                                   v_Ipcode            NUMBER,
                                   v_Txntypeid         NUMBER,
                                   v_Dtlsaleamount     FLOAT,
                                   v_Txndate           DATE,
                                   v_Txnheaderid       NVARCHAR2,
                                   v_Dtlclasscode      NVARCHAR2,
                                   v_My_Log_Id         NUMBER,
                                   p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               -- from ats_memberdetails md
               -- inner join lw_virtualcard vc on vc.ipcode = md.a_ipcode and vc.isprimary = 1 and rownum =1
               --  inner join (select COLUMN_VALUE  from table(v_bra_codes))cc on cc.COLUMN_VALUE = v_dtlclasscode
               INNER  JOIN Ae_Brapromocodes Bp
               ON     Bp.Classcodes = v_Dtlclasscode
               WHERE  1 = 1
                     --  and md.a_ipcode = v_ipcode
                      AND (Nvl(Md.a_Employeecode, 0) = 1 OR
                      Nvl(Md.a_Employeecode, 0) = 2)
                      AND v_Txntypeid = 2;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Employee Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Bra Employee Return';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --         SYSDATE,
                          v_Tbl(k).p_Processdate,
                          -1 /*Bra Return Point*/,
                          Add_Months(Trunc(v_Tbl(k).Txndate, 'Y'), 12) + 21, -- 01/02/2015
                          'Bra Employee Return' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Bra_Employee_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Bra_Employee_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Bra_Employee_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Bra_Employee_Return ');
     END Bra_Employee_Return;

     PROCEDURE Jean_Purchase(v_Detailitem_Rowkey NUMBER,
                             v_Ipcode            NUMBER,
                             v_Txntypeid         NUMBER,
                             v_Dtlsaleamount     FLOAT,
                             v_Txndate           DATE,
                             v_Txnheaderid       NVARCHAR2,
                             v_Dtlclasscode      NVARCHAR2,
                             v_My_Log_Id         NUMBER,
                             p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          v_Txnstartdt   DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               --  from ats_memberdetails md
               --  inner join lw_virtualcard vc on vc.ipcode = md.a_ipcode and vc.isprimary = 1 and rownum =1
               --  inner join (select COLUMN_VALUE  from table(v_jean_codes))cc on cc.COLUMN_VALUE = v_dtlclasscode
               INNER  JOIN Ae_Jeanspromocodes Jp
               ON     Jp.Classcodes = v_Dtlclasscode
               WHERE  1 = 1
                     --  and md.a_ipcode = v_ipcode
                      AND v_Txntypeid = 1
                      AND v_Dtlsaleamount > .01 -- AEO-472
                      AND ((Ae_Isinpilot(Md.a_Extendedplaycode) = 0) OR
                      ((Ae_Isinpilot(Md.a_Extendedplaycode) = 1) AND
                      v_Txndate < v_Txnstartdt));
          -- and v_dtlclasscode    in (SELECT t.COLUMN_VALUE AS classcode
          --                           FROM table(StringToTable((select cc.value
          --                                                     from Lw_Clientconfiguration cc
          --                                                     where cc.key = 'JeansPromoClassCodes'),';')) t  ) ;-- if classcode is in the clob value
          --  and v_dtlclasscode    in (select t.COLUMN_VALUE  from table(v_jean_codes) t) ;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Jean Purchase';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Jean Purchase Point*/,
                          Add_Months(Trunc(v_Tbl(k).Txndate, 'Y'), 12) + 1, -- 01/02/2015
                          'Jean Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Jean_Purchase: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Jean_Purchase',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Jean_Purchase',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Jean_Purchase ');
     END Jean_Purchase;

     PROCEDURE Jean_Return(v_Detailitem_Rowkey NUMBER,
                           v_Ipcode            NUMBER,
                           v_Txntypeid         NUMBER,
                           v_Dtlsaleamount     FLOAT,
                           v_Txndate           DATE,
                           v_Txnheaderid       NVARCHAR2,
                           v_Dtlclasscode      NVARCHAR2,
                           v_My_Log_Id         NUMBER,
                           p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          v_Txnstartdt   DATE;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               --  from  ats_memberdetails md
               --  inner join lw_virtualcard vc on vc.ipcode = md.a_ipcode and vc.isprimary = 1 and rownum =1
               --  inner join (select COLUMN_VALUE  from table(v_jean_codes))cc on cc.COLUMN_VALUE = v_dtlclasscode
               INNER  JOIN Ae_Jeanspromocodes Jp
               ON     Jp.Classcodes = v_Dtlclasscode
               WHERE  1 = 1
                     --  and md.a_ipcode = v_ipcode
                      AND v_Txntypeid = 2
                      AND Nvl(Md.a_Employeecode, 0) = 0
                      AND ((Ae_Isinpilot(Md.a_Extendedplaycode) = 0) OR
                      ((Ae_Isinpilot(Md.a_Extendedplaycode) = 1) AND
                      (SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk2
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk3
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk4
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk5
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk6
                         WHERE  Txnheaderid = v_Txnheaderid) < v_Txnstartdt));
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Jean Return';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          -- SYSDATE,
                          v_Tbl(k).p_Processdate,
                          -1 /*Jean Purchase Point*/,
                          Add_Months(Trunc(v_Tbl(k).Txndate, 'Y'), 12) + 1, -- 01/02/2015
                          'Jean Return' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Jean_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Jean_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Jean_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Jean_Return ');
     END Jean_Return;

     -- AEO-472 begin

     PROCEDURE Jean_Redemption(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate,
                      nvl(md.a_extendedplaycode,0) AS extendedplaycode
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               --  from ats_memberdetails md
               --  inner join lw_virtualcard vc on vc.ipcode = md.a_ipcode and vc.isprimary = 1 and rownum =1
               --  inner join (select COLUMN_VALUE  from table(v_bra_codes))cc on cc.COLUMN_VALUE = v_dtlclasscode
               INNER  JOIN Ae_Jeanspromocodes Bp
               ON     Bp.Classcodes = v_Dtlclasscode
               WHERE  1 = 1
                     --  and md.a_ipcode = v_ipcode
                      AND Nvl(Md.a_Employeecode, 0) = 0
                      AND (v_Dtlsaleamount = .01 or v_Dtlsaleamount = 0) -- AEO-573
                      AND v_Txntypeid = 1;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Redemptions';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Jean Redemptions';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Jean Redemptions*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*/jean Purchase Point*/,
                          -- AEO-573 Begin
                          CASE
                               WHEN ae_isinpilot( v_Tbl(k).extendedplaycode) = 1  THEN  to_date('12/31/2199','MM/DD/YYYY')
                               WHEN ae_isinpilot( v_Tbl(k).extendedplaycode) <> 1  THEN  Add_Months(Trunc(v_Tbl(k).Txndate, 'Y'), 12) + 21
                            END,
                            -- AEO-573 End

                          'Jean Redemptions' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Jean_Redemption: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Jean_Redemption',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Jean_Redemption',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Jean_Redemption ');
     END Jean_Redemption;


     --AEO-140
     PROCEDURE B5g1_Bra(v_Detailitem_Rowkey NUMBER,
                        v_Ipcode            NUMBER,
                        v_Txntypeid         NUMBER,
                        v_Dtlsaleamount     FLOAT,
                        v_Txndate           DATE,
                        v_Txnheaderid       NVARCHAR2,
                        v_Dtlclasscode      NVARCHAR2,
                        v_My_Log_Id         NUMBER,
                        p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             v_Dtlsaleamount > 0 then
                         1
                        else
                         0
                      End as v_onhold --Pointsonhold flag
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc --vckey is passed as ipcode from detailitem table
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Brapromocodes Bp
               ON     Bp.Classcodes = v_Dtlclasscode
               INNER JOIN (SELECT Dw.dtl_rowkey,
                                   Dw.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw2.dtl_rowkey,
                                   Dw2.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw3.dtl_rowkey,
                                   Dw3.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw4.dtl_rowkey,
                                   Dw4.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw5.dtl_rowkey,
                                   Dw5.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw6.dtl_rowkey,
                                   Dw6.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey) tdi
               ON     tdi.dtl_rowkey = v_Detailitem_rowkey		-- AEO-1900 AH
               WHERE  1 = 1
                      /*AND Nvl(Md.a_Employeecode, 0) = 0        AEO-816 AH */
                      AND v_Dtlsaleamount > .01
                      AND nvl(tdi.dtlclearanceitem,0) = 0				-- AEO-1900 AH
                      AND v_Txntypeid = 1
                      AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                      AND v_Txndate >= v_Txnstartdt;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Bra Purchase';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
         OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
           FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          1 /*Bra Purchase Point Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          returning Pointtransactionid into v_Pointtransactionid;
                    --Insert PointsOnHold Record
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;

               elsif (v_Tbl(k).v_onhold = 0) then

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Bra Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
			                    LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure B5G1_Bra: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'B5G1_Bra',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'B5G1_Bra',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in B5G1_Bra ');
     END B5g1_Bra;

     PROCEDURE B5g1_Jean(v_Detailitem_Rowkey NUMBER,
                         v_Ipcode            NUMBER,
                         v_Txntypeid         NUMBER,
                         v_Dtlsaleamount     FLOAT,
                         v_Txndate           DATE,
                         v_Txnheaderid       NVARCHAR2,
                         v_Dtlclasscode      NVARCHAR2,
                         v_My_Log_Id         NUMBER,
                         p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             v_Dtlsaleamount > 0 then
                         1
                        else
                         0
                      End as v_onhold --Pointsonhold flag
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md
               ON     Md.a_Ipcode = Vc.Ipcode
               INNER  JOIN Ae_Jeanspromocodes Jp
               ON     Jp.Classcodes = v_Dtlclasscode
			   INNER JOIN (SELECT Dw.dtl_rowkey,
                                   Dw.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw2.dtl_rowkey,
                                   Dw2.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw3.dtl_rowkey,
                                   Dw3.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw4.dtl_rowkey,
                                   Dw4.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw5.dtl_rowkey,
                                   Dw5.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                            UNION ALL
                            SELECT Dw6.dtl_rowkey,
                                   Dw6.dtlclearanceitem
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey) tdi
               ON     tdi.dtl_rowkey = v_Detailitem_rowkey			-- AEO-1900 AH
               WHERE  1 = 1
                      AND v_Txntypeid = 1
                      AND v_Dtlsaleamount > .01
					  AND nvl(tdi.dtlclearanceitem, 0) = 0					-- AEO-1900 AH
                      AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                      AND v_Txndate >= v_Txnstartdt;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Jean Purchase';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
         OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
           FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Jean Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Jean Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          1 /*Jean Purchase Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          returning Pointtransactionid into v_Pointtransactionid;
                    --Insert PointsOnHold Record
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Jean Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Jean Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
													LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue

               elsif (v_Tbl(k).v_onhold = 0) then

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          1 /*Jean Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Jean Purchase' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
													LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure B5G1_Jean: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'B5G1_Jean',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'B5G1_Jean',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in B5G1_Jean ');
     END B5g1_Jean;

     PROCEDURE B5g1_Bra_Return(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          --AEO-2168 Begin
          CURSOR Process_Rule IS
               SELECT v_Ipcode            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   Ae_Brapromocodes Bp
               INNER JOIN (SELECT Dw.dtl_rowkey,
                             Dw.Dtlclasscode,
                             Dw.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk Dw
                      WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw2.dtl_rowkey,
                             Dw2.Dtlclasscode,
                             Dw2.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk2 Dw2
                      WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw3.dtl_rowkey,
                             Dw3.Dtlclasscode,
                             Dw3.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk3 Dw3
                      WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw4.dtl_rowkey,
                             Dw4.Dtlclasscode,
                             Dw4.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk4 Dw4
                      WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw5.dtl_rowkey,
                             Dw5.Dtlclasscode,
                             Dw5.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk5 Dw5
                      WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw6.dtl_rowkey,
                             Dw6.Dtlclasscode,
                             Dw6.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk6 Dw6
                      WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey) tdi
               ON tdi.dtlclasscode = bp.classcodes
               WHERE  1 = 1
                      AND v_Txntypeid = 2
                      AND Bp.Classcodes = v_Dtlclasscode
                      AND nvl(tdi.dtlclearanceitem,0) = 0 --Exclude clearance
                      ;
          --AEO-2168 End
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Bra Return';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Return*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          -1 /*Bra Return Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bra Return' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure B5G1_Bra_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'B5G1_Bra_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'B5G1_Bra_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in B5G1_Bra_Return ');
     END B5g1_Bra_Return;

     PROCEDURE B5g1_Jean_Return(v_Detailitem_Rowkey NUMBER,
                                v_Ipcode            NUMBER,
                                v_Txntypeid         NUMBER,
                                v_Dtlsaleamount     FLOAT,
                                v_Txndate           DATE,
                                v_Txnheaderid       NVARCHAR2,
                                v_Dtlclasscode      NVARCHAR2,
                                v_My_Log_Id         NUMBER,
                                p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
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
          v_Recordcount NUMBER := 0;
          --cursor1-
          --AEO-2168 Begin
          CURSOR Process_Rule IS
               SELECT v_Ipcode            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate
               FROM   Ae_Jeanspromocodes Jp
               INNER JOIN (SELECT Dw.dtl_rowkey,
                             Dw.Dtlclasscode,
                             Dw.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk Dw
                      WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw2.dtl_rowkey,
                             Dw2.Dtlclasscode,
                             Dw2.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk2 Dw2
                      WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw3.dtl_rowkey,
                             Dw3.Dtlclasscode,
                             Dw3.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk3 Dw3
                      WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw4.dtl_rowkey,
                             Dw4.Dtlclasscode,
                             Dw4.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk4 Dw4
                      WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw5.dtl_rowkey,
                             Dw5.Dtlclasscode,
                             Dw5.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk5 Dw5
                      WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw6.dtl_rowkey,
                             Dw6.Dtlclasscode,
                             Dw6.dtlclearanceitem
                      FROM   Lw_Txndetailitem_Wrk6 Dw6
                      WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey) tdi
               ON tdi.dtlclasscode = Jp.Classcodes
               WHERE  1 = 1
                      AND v_Txntypeid = 2
                      AND Jp.Classcodes = v_Dtlclasscode
                      AND nvl(tdi.dtlclearanceitem,0) = 0 --Exclude clearance
                      ;
          --AEO-2168 End
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Jean Return';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          -1 /*Jean Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Jean Return' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure B5G1_Jean_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'B5G1_Jean_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'B5G1_Jean_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in B5G1_Jean_Return ');
     END B5g1_Jean_Return;

     PROCEDURE Aecc_Basic_Purchase(v_Header_Rowkey      NUMBER,
                                   v_Vckey              NUMBER,
                                   v_Txntypeid          NUMBER,
                                   v_Brandid            NUMBER,
                                   v_Txndate            DATE,
                                   v_Txnqualpurchaseamt FLOAT,
                                   v_Txnemployeeid      NVARCHAR2,
                                   v_Txnheaderid        NVARCHAR2,
                                   v_My_Log_Id          NUMBER,
                                   p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Vckey) AS Vckey,
                      SUM(Amount) AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                      5 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      p_Processdate,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             v_Txnqualpurchaseamt > 0 then
                         1
                        else
                         0
                      End as v_onhold --Pointsonhold flag

               FROM   (SELECT v_Header_Rowkey,
                              v_Vckey,
                              a_Tendertype,
                              --                        ratio_to_report(tt.a_tenderamount) over(partition by tt.a_txnheaderid),           /*Percentage of tenderamount over entire tender amount*/
                              Round(Ht.a_Txnqualpurchaseamt *
                                    Ratio_To_Report(Tt.a_Tenderamount)
                                    Over(PARTITION BY Tt.a_Txnheaderid)) AS Amount
                       FROM   bp_ae.Lw_Virtualcard Vc
                       INNER  JOIN bp_ae.Ats_Memberdetails Md
                       ON     Md.a_Ipcode = Vc.Ipcode
                       INNER  JOIN bp_ae.Ats_Txntender Tt
                       ON     Tt.a_Txnheaderid = v_Txnheaderid
                       INNER  JOIN bp_ae.Ats_Txnheader Ht
                       ON     Ht.a_Txnheaderid = Tt.a_Txnheaderid
                       WHERE  1 = 1
                              AND Vc.Vckey = v_Vckey
                             /* AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                              AND v_Txndate >= v_Txnstartdt */
                              AND (v_Txntypeid = 1 OR v_Txntypeid = 4))
               GROUP  BY a_Tendertype
               HAVING a_Tendertype IN('75', '78');
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AEO or Aerie Credit Card Bonus';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
                 FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid_Redesign /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          1,
                          v_Tbl(k).Txndate, --txndate
                          --SYSDATE, --pointawarddate
                          v_Tbl(k).p_Processdate,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier, /*Points*/
                          v_Tbl(k).Expirationdate, --expiration date
                          'AECC_Basic_Purchase', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier/*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL) returning Pointtransactionid into v_Pointtransactionid;
                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid_Redesign /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          3,
                          v_Tbl(k).Txndate, --txndate
                          --SYSDATE, --pointawarddate
                          v_Tbl(k).p_Processdate,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier, /*Points*/
                          v_Tbl(k).Expirationdate, --expiration date
                          'AECC_Basic_Purchase', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0/*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                           LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;

                --logs errors into table so that the process would continue
                Commit;
               elsif (v_Tbl(k).v_onhold = 0) then
                 INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid_Redesign /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          1,
                          v_Tbl(k).Txndate, --txndate
                          --SYSDATE, --pointawarddate
                          v_Tbl(k).p_Processdate,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier, /*Points*/
                          v_Tbl(k).Expirationdate, --expiration date
                          'AECC_Basic_Purchase', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0/*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

              LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AECC_Basic_Purchase: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AECC_Basic_Purchase',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AECC_Basic_Purchase',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AECC_Basic_Purchase');
     END Aecc_Basic_Purchase;

     PROCEDURE Aecc_Basic_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Vckey) AS Vckey,
                      SUM(Amount) AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                      5 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      p_Processdate
               FROM   (SELECT v_Header_Rowkey,
                              v_Vckey,
                              a_Tendertype,
                              --                        ratio_to_report(tt.a_tenderamount) over(partition by tt.a_txnheaderid),           /*Percentage of tenderamount over entire tender amount*/
                              Round(Ht.a_Txnqualpurchaseamt *
                                    Ratio_To_Report(Tt.a_Tenderamount)
                                    Over(PARTITION BY Tt.a_Txnheaderid)) AS Amount
                       FROM   bp_ae.Lw_Virtualcard Vc
                       INNER  JOIN bp_ae.Ats_Memberdetails Md
                       ON     Md.a_Ipcode = Vc.Ipcode
                       INNER  JOIN bp_ae.Ats_Txntender Tt
                       ON     Tt.a_Txnheaderid = v_Txnheaderid
                       INNER  JOIN bp_ae.Ats_Txnheader Ht
                       ON     Ht.a_Txnheaderid = Tt.a_Txnheaderid
                       WHERE  1 = 1
                              AND Vc.Vckey = v_Vckey
                            /*  AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)*/
                              AND v_Txntypeid = 2
                            /*  AND (SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk2
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk3
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk4
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk5
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk6
                                   WHERE  Txnheaderid = v_Txnheaderid) >=
                              v_Txnstartdt */)
               GROUP  BY a_Tendertype
               HAVING a_Tendertype IN('75', '78');
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AEO or Aerie Credit Card Bonus Return';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid_Redesign /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          2,
                          v_Tbl(k).Txndate, --txndate
                          --SYSDATE, --pointawarddate
                          v_Tbl(k).p_Processdate,
                          Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k)
                          .Pointmultiplier, /*Points*/
                          v_Tbl(k).Expirationdate, --expiration date
                          'AECC_Basic_Return', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AECC_Basic_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AECC_Basic_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AECC_Basic_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AECC_Basic_Return');
     END Aecc_Basic_Return;



     --AEO-252
     PROCEDURE Updatetierandnetspend(v_Header_Rowkey      NUMBER,
                                     v_Vckey              NUMBER,
                                     v_Txntypeid          NUMBER,
                                     v_Brandid            NUMBER,
                                     v_Txndate            DATE,
                                     v_Txnqualpurchaseamt FLOAT,
                                     v_Txnemployeeid      NVARCHAR2,
                                     v_Txnheaderid        NVARCHAR2,
                                     v_My_Log_Id          NUMBER,
                                     p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
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
          v_Recordcount NUMBER := 0;
          v_membertierid NUMBER := -1;


          ---v_Processdate        date :=  to_date(p_Processdate,'DD/MM/YYYY HH:MI AM');
          CURSOR Process_Rule IS
               SELECT Vc.Ipcode            AS Ipcode,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Vckey              AS Vckey,
                      nvl(Md.a_Netspend,0) AS Netspend, --AEO-503
                      nvl(lm.membercreatedate, to_date('1/1/900', 'mm/dd/yyyy')) AS enrolldate,
                      v_Txndate            AS Txndate,
                      Vc.Status            AS Status,
                      p_Processdate        AS p_Processdate,
                      --AEO 649 changes here ----------------SCJ
                      v_Txntypeid          AS v_Txntypeid
                      --AEO 649 changes here ----------------SCJ
               FROM   bp_ae.Lw_Virtualcard Vc
               INNER  JOIN bp_ae.Ats_Memberdetails Md    ON     Md.a_Ipcode = Vc.Ipcode
               INNER  JOIN bp_ae.Lw_Loyaltymember  Lm    ON     Lm.Ipcode = Vc.Ipcode
               WHERE  1 = 1
                      AND Vc.Vckey = v_Vckey
                      AND
                      ((v_Txnqualpurchaseamt > 0 AND
                      (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) AND
                      v_Txndate >= v_Txnstartdt) OR
                      (v_Txnqualpurchaseamt < 0 AND
                      (Ae_Isinpilot(Md.a_Extendedplaycode) = 1) AND
                      (SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk2
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk3
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk4
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk5
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk6
                         WHERE  Txnheaderid = v_Txnheaderid) >= v_Txnstartdt));
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl          t_Tab;
          v_Tierid       NUMBER := 0;
          v_Tierdesc     VARCHAR2(50);
          v_fullaccesstier     NUMBER := 0;
          v_extraaccesstier    NUMBER := 0;
          v_Ct_Tierid    NUMBER := 0;
          v_Ct_Updtierid NUMBER := 0;
          v_enddate      TIMESTAMP := SYSDATE; -- AEO-2007 begin/ end

     BEGIN
          SELECT Tr.Tierid
          INTO   v_fullaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Full Access';

          SELECT Tr.Tierid
          INTO   v_extraaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Extra Access';

         SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';

          FOR y IN Process_Rule
          LOOP
               v_membertierid := Hibernate_Sequence.Nextval;

               SELECT COUNT(Mt.Tierid) --mt.tierid
               INTO   v_Ct_Tierid
               FROM   Lw_Membertiers Mt
               WHERE  Mt.Memberid = y.Ipcode
               AND    Trunc(Mt.Todate) > Trunc(SYSDATE); --AEO-1033

               IF v_Ct_Tierid > 0
               THEN
                    SELECT Mt.Tierid, Mt.Description --mt.tierid
                    INTO   v_Tierid, v_Tierdesc
                    FROM   Lw_Membertiers Mt
                    WHERE  Mt.Memberid = y.Ipcode
                    AND    Trunc(Mt.Todate) > Trunc(SYSDATE) --AEO-1033
                    AND    Rownum = 1;
               END IF;

               v_enddate := p_Processdate; -- AEO-2207 begin/end

               IF y.Netspend >= 350
                  AND y.Status = 1
               THEN
                 IF v_Ct_Tierid = 0
                    THEN

                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (v_membertierid,
                               v_extraaccesstier,
                               y.Ipcode,
                               -- y.txndate,
                               p_processdate ,
                               Getexpirationdate(y.enrolldate, p_Processdate) , --To_Date('12/31/2199 ', 'mm/dd/yyyy'),
                               'Base',-- AEO-503
                               -- AEO-2007 begin
                               SYSDATE,
                               SYSDATE);
                               -- AEO-2007 end

                      INSERT INTO Ats_Membernetspend
                           (a_Rowkey,
                            Lwidentifier,
                            a_Ipcode,
                            a_Parentrowkey,
                            a_Membertierid,
                            a_Netspend,
                            Statuscode,
                            Createdate,
                            Updatedate)
                      VALUES
                           (Seq_Rowkey.Nextval,
                            0,
                            y.Ipcode,
                            -1,
                            v_Membertierid,
                            y.Netspend,
                            0,
                            SYSDATE,
                            SYSDATE);

                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;

                    END IF;
                    IF v_Tierid <> v_extraaccesstier
                      AND v_Ct_Tierid > 0
                    THEN

                         -- update all not expired tiers that are not extra access
                         UPDATE Lw_Membertiers Mt1
                         SET    Mt1.Todate = v_enddate -- AEO-2007 begin/end
                         WHERE  Mt1.Memberid = y.Ipcode
                                AND  mt1.todate > p_processdate
                                AND Mt1.Tierid <> v_extraaccesstier;

                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;

                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (v_membertierid,
                               v_extraaccesstier,
                               y.Ipcode,
                               --  y.txndate,
                               v_enddate + 1/86400, -- AEO-2007 begin / end
                               getexpirationdate(y.enrolldate, p_Processdate) ,-- To_Date('12/31/2199 ', 'mm/dd/yyyy
                               'Qualifier',
                               -- AEO-2007 begin
                               SYSDATE,
                               SYSDATE);
                               -- AEO-2007 end

                       INSERT INTO Ats_Membernetspend
                            (a_Rowkey,
                             Lwidentifier,
                             a_Ipcode,
                             a_Parentrowkey,
                             a_Membertierid,
                             a_Netspend,
                             Statuscode,
                             Createdate,
                             Updatedate)
                       VALUES
                            (Seq_Rowkey.Nextval,
                             0,
                             y.ipcode,
                             -1,
                             v_membertierid,
                             y.Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);
                    END IF;
               END IF;

               if y.v_Txntypeid <> 6 then -- AEO-649  changes here -----------------SCJ
                 UPDATE Ats_Memberdetails Mds
                 SET
                        mds.a_lifetimenetspend = CASE
                                              WHEN (Nvl(Mds.a_lifetimenetspend, 0) +
                                                   Nvl(y.Txnqualpurchaseamt, 0) < 0) THEN
                                               0
                                              else
                                               Nvl(Mds.a_lifetimenetspend, 0) +
                                               Nvl(y.Txnqualpurchaseamt, 0)
                                         END,
                        Mds.a_Netspend = CASE
                                              WHEN (Nvl(Mds.a_Netspend, 0) +
                                                   Nvl(y.Txnqualpurchaseamt, 0) < 0) THEN
                                               0
                                              else
                                               Nvl(Mds.a_Netspend, 0) +
                                               Nvl(y.Txnqualpurchaseamt, 0)
                                         END
                 WHERE  Mds.a_Ipcode = y.Ipcode;
               end if;
               v_Cnt := v_Cnt + 1;
               IF MOD(v_Cnt, 1000) = 0
               THEN
                    COMMIT;
               END IF;
          END LOOP;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure UpdateTierandNetSpend: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ipcode;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'UpdateTierandNetSpend',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'UpdateTierandNetSpend',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in UpdateTierandNetSpend');
     END Updatetierandnetspend;

     --AEO-252
     --AEO-364
     PROCEDURE Updatetieronly(v_Header_Rowkey      NUMBER,
                              v_Vckey              NUMBER,
                              v_Txntypeid          NUMBER,
                              v_Brandid            NUMBER,
                              v_Txndate            DATE,
                              v_Txnqualpurchaseamt FLOAT,
                              v_Txnemployeeid      NVARCHAR2,
                              v_Txnheaderid        NVARCHAR2,
                              v_My_Log_Id          NUMBER,
                              p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
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
          v_Recordcount NUMBER := 0;
          v_membertierid NUMBER := -1;
          v_enddate TIMESTAMP := SYSDATE;

          ---v_Processdate        date :=  to_date(p_Processdate,'DD/MM/YYYY HH:MI AM');
          CURSOR Process_Rule IS
               SELECT Vc.Ipcode            AS Ipcode,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      v_Vckey              AS Vckey,
                      nvl(Md.a_Netspend,0)        AS Netspend,  -- AEO-503
                      nvl(lm.membercreatedate,to_date('1/1/1900','mm/dd/yyyy')) AS enrolldate,
                      v_Txndate            AS Txndate,
                      Vc.Status            AS Status,
                      p_Processdate        AS p_Processdate
               FROM   bp_ae.Lw_Virtualcard Vc
               INNER  JOIN bp_ae.Ats_Memberdetails Md  ON     Md.a_Ipcode = Vc.Ipcode
               INNER  JOIN bp_ae.Lw_Loyaltymember  Lm  ON     lm.Ipcode = Vc.Ipcode
               WHERE  1 = 1
                      AND Vc.Vckey = v_Vckey
                      AND
                      ((v_Txnqualpurchaseamt > 0  AND
                      v_Txndate >= v_Txnstartdt) OR
                      (v_Txnqualpurchaseamt < 0  AND
                      (SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk2
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk3
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk4
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk5
                         WHERE  Txnheaderid = v_Txnheaderid
                         UNION ALL
                         SELECT Txnoriginaltxndate
                         FROM   Lw_Txnheader_Wrk6
                         WHERE  Txnheaderid = v_Txnheaderid) >= v_Txnstartdt));

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl          t_Tab;
          v_Tierid       NUMBER := 0;
          v_Tierdesc     VARCHAR2(50);
          v_fullaccesstier     NUMBER := 0;
          v_extraaccesstier    NUMBER := 0;
          v_Ct_Tierid    NUMBER := 0;
          v_Ct_Updtierid NUMBER := 0;


     BEGIN
          SELECT Tr.Tierid
          INTO   v_fullaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Full Access';

          SELECT Tr.Tierid
          INTO   v_extraaccesstier
          FROM   Lw_Tiers Tr
          WHERE  Tr.Tiername = 'Extra Access';

         SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';

          FOR y IN Process_Rule
          LOOP
               SELECT COUNT(Mt.Tierid) --mt.tierid
               INTO   v_Ct_Tierid
               FROM   Lw_Membertiers Mt
               WHERE  Mt.Memberid = y.Ipcode
               AND    Trunc(Mt.Todate) > Trunc(SYSDATE); --AEO-1033


               v_enddate := p_processdate; -- AEO-2007

               IF v_Ct_Tierid > 0
               THEN
                    SELECT Mt.Tierid, Mt.Description --mt.tierid
                    INTO   v_Tierid, v_Tierdesc
                    FROM   Lw_Membertiers Mt
                    WHERE  Mt.Memberid = y.Ipcode
                    AND    Trunc(Mt.Todate) > Trunc(SYSDATE) --AEO-1033
                    AND    Rownum = 1;
               END IF;
               IF y.Netspend >= 350 AND y.Status = 1  THEN
                    IF v_Ct_Tierid = 0 THEN

                         v_membertierid := Hibernate_Sequence.Nextval;

                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (v_membertierid,
                               v_extraaccesstier,
                               y.Ipcode,
                               -- y.txndate,
                               p_Processdate,
                               GetExpirationDate(y.enrolldate, p_processdate),
                               --To_Date('12/31/2199 ', 'mm/dd/yyyy'),
                               'Base',-- AEO-503
                               SYSDATE,
                               SYSDATE);


                       INSERT INTO Ats_Membernetspend
                            (a_Rowkey,
                             Lwidentifier,
                             a_Ipcode,
                             a_Parentrowkey,
                             a_Membertierid,
                             a_Netspend,
                             Statuscode,
                             Createdate,
                             Updatedate)
                       VALUES
                            (Seq_Rowkey.Nextval,
                             0,
                             y.ipcode,
                             -1,
                             v_membertierid,
                             y.Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);

                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;
                    END IF;
                    IF v_Tierid <> v_extraaccesstier
                      AND v_Ct_Tierid > 0
                      THEN

                         UPDATE Lw_Membertiers Mt1
                         SET    Mt1.Todate = v_enddate
                        WHERE  Mt1.Memberid = y.Ipcode
                                AND  mt1.todate > p_processdate
                                AND Mt1.Tierid <> v_extraaccesstier;

                         UPDATE Ats_Memberdetails Md2
                         SET    Md2.a_Aitupdate = 1
                         WHERE  Md2.a_Ipcode = y.Ipcode;

                         v_membertierid := Hibernate_Sequence.Nextval;
                         INSERT INTO Lw_Membertiers
                              (Id,
                               Tierid,
                               Memberid,
                               Fromdate,
                               Todate,
                               Description,
                               Createdate,
                               Updatedate)
                         VALUES
                              (v_membertierid,
                               v_extraaccesstier,
                               y.Ipcode,
                               --  y.txndate,
                                  v_enddate + 1/86400, -- AEO-2007 begin / end,
                              GetExpirationDate(y.enrolldate, p_processdate),
                               --To_Date('12/31/2199 ', 'mm/dd/yyyy'),
                               'Qualifier',
                               -- AEO-2007 begin
                               SYSDATE,
                               SYSDATE);
                               -- AEO-2007 end

                       INSERT INTO Ats_Membernetspend
                            (a_Rowkey,
                             Lwidentifier,
                             a_Ipcode,
                             a_Parentrowkey,
                             a_Membertierid,
                             a_Netspend,
                             Statuscode,
                             Createdate,
                             Updatedate)
                       VALUES
                            (Seq_Rowkey.Nextval,
                             0,
                             y.ipcode,
                             -1,
                             v_membertierid,
                             y.Netspend,
                             0,
                             SYSDATE,
                             SYSDATE);
                    END IF;
               END IF;

               v_Cnt := v_Cnt + 1;
               IF MOD(v_Cnt, 1000) = 0
               THEN
                    COMMIT;
               END IF;
          END LOOP;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure UpdateTierOnly: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Ipcode;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'UpdateTierOnly',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'UpdateTierOnly',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in UpdateTierOnly');
     END Updatetieronly;

     --AEO-364

     --AEO-629
     PROCEDURE AECC_bonus_points(v_Header_Rowkey      NUMBER,
                                   v_Vckey              NUMBER,
                                   v_Txntypeid          NUMBER,
                                   v_Brandid            NUMBER,
                                   v_Txndate            DATE,
                                   v_Txnqualpurchaseamt FLOAT,
                                   v_Txnemployeeid      NVARCHAR2,
                                   v_Txnheaderid        NVARCHAR2,
                                   v_My_Log_Id          NUMBER,
                                   p_Processdate        TIMESTAMP) IS

          v_Tendertype           NUMBER:= 0;
          v_Cnt                  NUMBER:= 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;

          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Vckey) AS Vckey,
                      MAX(a_ordernumber) AS a_ordernumber,
                      SUM(Amount) AS Txnqualpurchaseamt,
                      v_Txndate AS Txndate,
                      5 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      p_Processdate,
                      a_Tendertype,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             SUM(Amount) > 0 then
                         1
                        else
                         0
                      End as v_onhold --Pointsonhold flag
               FROM   (SELECT v_Header_Rowkey,
                              v_Vckey,
                              a_Tendertype,
                              a_ordernumber,
                              nvl(Round(Ht.a_Txnqualpurchaseamt *
                                    Ratio_To_Report(Tt.a_Tenderamount)
                                    Over(PARTITION BY Tt.a_Txnheaderid)),0) AS Amount -- 17Aug2016 MMV begin-end
                       FROM   bp_ae.Lw_Virtualcard Vc
                       INNER  JOIN bp_ae.Ats_Memberdetails Md
                       ON     Md.a_Ipcode = Vc.Ipcode
                       INNER  JOIN bp_ae.Ats_Txntender Tt
                       ON     Tt.a_Txnheaderid = v_Txnheaderid
                       INNER  JOIN bp_ae.Ats_Txnheader Ht
                       ON     Ht.a_Txnheaderid = Tt.a_Txnheaderid
                       WHERE  1 = 1
                              AND Vc.Vckey = v_Vckey
                              AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                              AND v_Txndate >= v_Txnstartdt
                              AND ht.a_txnregisternumber > 199
                              AND ht.a_ordernumber is not null
                              AND (SELECT COUNT(*)
                              FROM bp_ae.ATS_TXNHEADER xh
                              INNER JOIN bp_ae.ATS_TXNTENDER xt
                              ON xt.a_txnheaderid = xh.a_txnheaderid
                              WHERE xh.a_ordernumber = ht.a_ordernumber
                              AND xh.a_vckey = v_Vckey
                              AND xh.a_txnheaderid <> v_Txnheaderid
							  AND xh.a_txntypeid IN (1,2,4,8)
                              AND xt.a_tendertype IN ('75','78')) > 0
                              AND ((v_Txntypeid = 1 OR v_Txntypeid = 4 OR v_Txntypeid = 8) OR
                              (v_Txntypeid = 2 AND
                              (SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk2
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk3
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk4
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk5
                                   WHERE  Txnheaderid = v_Txnheaderid
                                   UNION ALL
                                   SELECT Txnoriginaltxndate
                                   FROM   Lw_Txnheader_Wrk6
                                   WHERE  Txnheaderid = v_Txnheaderid) >= v_Txnstartdt))
                              )
               GROUP  BY a_Tendertype
               HAVING a_Tendertype IN('69');
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid_Redesign
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'AEO or Aerie Credit Card Bonus';
          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
         OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
           FOR k IN 1 .. v_tbl.count
          LOOP
         IF (v_Tbl(k).v_onhold = 1) then

                INSERT INTO Lw_Pointtransaction
                           (Pointtransactionid,
                            Vckey,
                            Pointtypeid,
                            Pointeventid,
                            Transactiontype,
                            Transactiondate,
                            Pointawarddate,
                            Points,
                            Expirationdate,
                            Notes,
                            Ownertype,
                            Ownerid,
                            Rowkey,
                            Parenttransactionid,
                            Pointsconsumed,
                            Pointsonhold,
                            Ptlocationid,
                            Ptchangedby,
                            Createdate,
                            Expirationreason)
                      VALUES
                           (Seq_Pointtransactionid.Nextval,
                            v_Tbl(k).Vckey,
                            v_Pointtypeid_Redesign /*Pointtypeid*/,
                            v_Pointeventid /*pointeventid*/,
                            CASE WHEN v_Txntypeid = 2 THEN 2 ELSE 1 END,
                            v_Tbl(k).Txndate, --txndate
                            v_Tbl(k).p_Processdate,
                            Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier, /*Points*/
                            v_Tbl(k).Expirationdate, --expiration date
                            'AECC_bonus_point', /*Notes*/
                            1,
                            101,
                            v_Tbl(k).Originalrowkey,
                            -1,
                            0 /*Pointsconsumed*/,
                            Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier /*Pointsonhold*/,
                            NULL,
                            'RulesProcessing' /*Ptchangedby*/,
                            SYSDATE,
                            NULL)
                 returning Pointtransactionid into v_Pointtransactionid;

                    --Insert PointsOnHold Record
                INSERT INTO Lw_Pointtransaction
                           (Pointtransactionid,
                            Vckey,
                            Pointtypeid,
                            Pointeventid,
                            Transactiontype,
                            Transactiondate,
                            Pointawarddate,
                            Points,
                            Expirationdate,
                            Notes,
                            Ownertype,
                            Ownerid,
                            Rowkey,
                            Parenttransactionid,
                            Pointsconsumed,
                            Pointsonhold,
                            Ptlocationid,
                            Ptchangedby,
                            Createdate,
                            Expirationreason)
                      VALUES
                           (Seq_Pointtransactionid.Nextval,
                            v_Tbl(k).Vckey,
                            v_Pointtypeid_Redesign /*Pointtypeid*/,
                            v_Pointeventid /*pointeventid*/,
                            3 /*pointsonhold*/,
                            v_Tbl(k).Txndate, --txndate
                            v_Tbl(k).p_Processdate,
                            Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier, /*Points*/
                            v_Tbl(k).Expirationdate, --expiration date
                            'AECC_bonus_point', /*Notes*/
                            1,
                            101,
                            v_Tbl(k).Originalrowkey,
                            v_Pointtransactionid,
                            0 /*Pointsconsumed*/,
                            0 /*Pointsonhold*/,
                            NULL,
                            'RulesProcessing' /*Ptchangedby*/,
                            SYSDATE,
                            NULL)
              LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
              COMMIT;

                 /*Find the Tender Type from the parent transaction*/
                SELECT xt.a_tendertype
                INTO v_Tendertype
                FROM bp_ae.ATS_TXNHEADER xh
                INNER JOIN bp_ae.ATS_TXNTENDER xt
                ON xt.a_txnheaderid = xh.a_txnheaderid
                WHERE xh.a_ordernumber =  v_Tbl(k).a_ordernumber
                AND xh.a_vckey = v_Vckey
                AND xh.a_txnheaderid <> v_Txnheaderid
                AND xh.a_txntypeid IN (1,2,4,8)
                AND xt.a_tendertype IN ('75','78')
                and rownum  = 1 ;

                /*Update the tender record of the incoming transaction from a 69 to match the parent tender type */
                IF v_Tendertype <> 0 THEN
                  UPDATE ATS_TXNTENDER td
                  SET td.a_tendertype = v_Tendertype
                  WHERE td.a_txnheaderid = v_Txnheaderid;

                  COMMIT;
                END IF;


               elsif (v_Tbl(k).v_onhold = 0) then

                INSERT INTO Lw_Pointtransaction
                           (Pointtransactionid,
                            Vckey,
                            Pointtypeid,
                            Pointeventid,
                            Transactiontype,
                            Transactiondate,
                            Pointawarddate,
                            Points,
                            Expirationdate,
                            Notes,
                            Ownertype,
                            Ownerid,
                            Rowkey,
                            Parenttransactionid,
                            Pointsconsumed,
                            Pointsonhold,
                            Ptlocationid,
                            Ptchangedby,
                            Createdate,
                            Expirationreason)
                      VALUES
                           (Seq_Pointtransactionid.Nextval,
                            v_Tbl(k).Vckey,
                            v_Pointtypeid_Redesign /*Pointtypeid*/,
                            v_Pointeventid /*pointeventid*/,
                            CASE WHEN v_Txntypeid = 2 THEN 2 ELSE 1 END,
                            v_Tbl(k).Txndate, --txndate
                            v_Tbl(k).p_Processdate,
                            Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier, /*Points*/
                            v_Tbl(k).Expirationdate, --expiration date
                            'AECC_bonus_point', /*Notes*/
                            1,
                            101,
                            v_Tbl(k).Originalrowkey,
                            -1,
                            0 /*Pointsconsumed*/,
                            0 /*Pointsonhold*/,
                            NULL,
                            'RulesProcessing' /*Ptchangedby*/,
                            SYSDATE,
                            NULL)
              LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
                 COMMIT;
                 /*Find the Tender Type from the parent transaction*/
                SELECT xt.a_tendertype
                INTO v_Tendertype
                FROM bp_ae.ATS_TXNHEADER xh
                INNER JOIN bp_ae.ATS_TXNTENDER xt
                ON xt.a_txnheaderid = xh.a_txnheaderid
                WHERE xh.a_ordernumber =  v_Tbl(k).a_ordernumber
                AND xh.a_vckey = v_Vckey
                AND xh.a_txnheaderid <> v_Txnheaderid
                AND xh.a_txntypeid IN (1,2,4,8)
                AND xt.a_tendertype IN ('75','78')
                and rownum  = 1 ;

                /*Update the tender record of the incoming transaction from a 69 to match the parent tender type */
                IF v_Tendertype <> 0 THEN
                  UPDATE ATS_TXNTENDER td
                  SET td.a_tendertype = v_Tendertype
                  WHERE td.a_txnheaderid = v_Txnheaderid;

                  COMMIT;
                END IF;

               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure AECC_bonus_point: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'AECC_bonus_point',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'AECC_bonus_point',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in AECC_bonus_point');
     END AECC_bonus_points;
     --AEO-629
--AEO-1818 Begin GD
     PROCEDURE DblePoints_Tuesdays_2017(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Amount          FLOAT := 0;
          v_BonusPoints          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      CASE
                        WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                             SUM(Di.Dtlsaleamount) > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM   (SELECT Dw.Txnheaderid,
                                   Dw.Dtlclasscode,
                                   Dw.Dtlsaleamount,
                                   Dw.Ipcode,
                                   Dw.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw2.Txnheaderid,
                                   Dw2.Dtlclasscode,
                                   Dw2.Dtlsaleamount,
                                   Dw2.Ipcode,
                                   Dw2.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw3.Txnheaderid,
                                   Dw3.Dtlclasscode,
                                   Dw3.Dtlsaleamount,
                                   Dw3.Ipcode,
                                   Dw3.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw4.Txnheaderid,
                                   Dw4.Dtlclasscode,
                                   Dw4.Dtlsaleamount,
                                   Dw4.Ipcode,
                                   Dw4.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw5.Txnheaderid,
                                   Dw5.Dtlclasscode,
                                   Dw5.Dtlsaleamount,
                                   Dw5.Ipcode,
                                   Dw5.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw6.Txnheaderid,
                                   Dw6.Dtlclasscode,
                                   Dw6.Dtlsaleamount,
                                   Dw6.Ipcode,
                                   Dw6.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
               WHERE  1 = 1
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                      AND
                      (
                      v_Txndate BETWEEN to_date('10/03/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('10/31/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
                      AND Di.Dtlclasscode NOT IN ('9911')   -- Remove giftcards
                      AND UPPER(TO_CHAR(v_Txndate, 'FmDay')) = 'TUESDAY'
               GROUP  BY Di.Txnheaderid;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = '2017: Double Point Tuesdays';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Amount <> 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          '2017: Double Point Tuesdays' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          '2017: Double Point Tuesdays', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          '2017: Double Point Tuesdays' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_Tuesdays_2017: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_Tuesdays_2017',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_Tuesdays_2017',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_Tuesdays_2017');
     END DblePoints_Tuesdays_2017;

     PROCEDURE DblePoints_Tuesdays_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Count            FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_BonusPoints          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(v_Txnqualpurchaseamt) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      th.a_rowkey AS ParentRowKey
               FROM   (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk hw
                            WHERE  hw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk2 hw2
                            WHERE  hw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk3 hw3
                            WHERE  hw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk4 hw4
                            WHERE  hw4.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk5 hw5
                            WHERE  hw5.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk6 hw6
                            WHERE  hw6.Txnheaderid =  v_Txnheaderid) wh
               INNER JOIN bp_ae.ats_txnheader th
               ON th.a_rowkey = wh.txnoriginaltxnrowkey
               WHERE  1 = 1
                      AND wh.txnheaderid = v_Txnheaderid
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0 --Original purchase is not a gif
                      AND
                      ( --Original Purchase on Tuesday and in the range of dates that the bonus was active
                      th.a_txndate BETWEEN to_date('10/03/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('10/31/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND UPPER(TO_CHAR(th.a_txndate, 'FmDay')) = 'TUESDAY'
               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = '2017: Double Point Tuesdays';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count > 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(NVL(v_AECC_Amount,0), 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  '2017: Double Point Tuesdays Return', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_Tuesdays_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_Tuesdays_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_Tuesdays_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_Tuesdays_Return');
     END DblePoints_Tuesdays_Return;
--AEO-1818 End GD

--AEO-1821 BEGIN LAPP
     PROCEDURE Bonus_Transact_AECC_2017(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Amount          FLOAT := 0;
          v_BonusPoints          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(vc.Ipcode) AS IPCODE,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate
               FROM   (SELECT Dw.Txnheaderid,
                                   Dw.Dtlclasscode,
                                   Dw.Dtlsaleamount,
                                   Dw.Ipcode,--this is vckey
                                   Dw.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw2.Txnheaderid,
                                   Dw2.Dtlclasscode,
                                   Dw2.Dtlsaleamount,
                                   Dw2.Ipcode,--this is vckey
                                   Dw2.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw3.Txnheaderid,
                                   Dw3.Dtlclasscode,
                                   Dw3.Dtlsaleamount,
                                   Dw3.Ipcode,--this is vckey
                                   Dw3.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw4.Txnheaderid,
                                   Dw4.Dtlclasscode,
                                   Dw4.Dtlsaleamount,
                                   Dw4.Ipcode,--this is vckey
                                   Dw4.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw5.Txnheaderid,
                                   Dw5.Dtlclasscode,
                                   Dw5.Dtlsaleamount,
                                   Dw5.Ipcode, --this is vckey
                                   Dw5.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw6.Txnheaderid,
                                   Dw6.Dtlclasscode,
                                   Dw6.Dtlsaleamount,
                                   Dw6.Ipcode, --this is vckey
                                   Dw6.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
						INNER JOIN lw_virtualcard vc on Di.IPCODE = vc.vckey
               WHERE  1 = 1
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                      AND
                      (
                      v_Txndate BETWEEN to_date('10/11/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('10/18/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
                      AND Di.Dtlclasscode NOT IN ('9911')   -- Remove giftcards
                      --VALIDATE THAT THE POINT HAS NOT BEEN ISSUED BEFORE BY CHECKING IN WORK TABLE
                      AND NOT EXISTS (SELECT * FROM bp_ae.AE_TRANSAECCFALL2017 TF WHERE TF.IPCODE = vc.Ipcode)
               GROUP  BY Di.Txnheaderid;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'FALL: 2500 Bonus Points when you use an AECC';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78);

            --This is only issued when there is a credit card sale, not
            IF v_AECC_Amount <> 0 THEN
            --if the member shops with their AE credit card,
            --then they get an additional 2500 points
              v_BonusPoints := 2500;

              INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                VALUES
                       (Seq_Pointtransactionid.Nextval,
                        v_Tbl(k).Vckey,
                        v_Pointtypeid,
                        v_Pointeventid,
                        1 /*purchase*/,
                        v_Tbl(k).Txndate,
                        v_Tbl(k).p_Processdate,
                        v_BonusPoints, /*Points*/
                        v_Tbl(k).Expirationdate,
                        'FALL: 2500 Bonus Points when you use an AECC' /*Notes*/,
                        1,
                        101,
                        v_Tbl(k).Originalrowkey,
                        -1,
                        0 /*Pointsconsumed*/,
                        0 /*Pointsonhold*/,
                        NULL,
                        'RulesProcessing' /*Ptchangedby*/,
                        SYSDATE,
                        NULL)
                  LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
              INSERT INTO AE_TRANSAECCFALL2017 (IPCODE, TXNDATE, CREATEDATE, OriginalRowkey)
                       VALUES(v_Tbl(k).IPCODE, v_Tbl(k).Txndate, sysdate, v_Tbl(k).Originalrowkey);
              COMMIT;
            END IF;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure Bonus_Transact_AECC_2017: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'Bonus_Transact_AECC_2017',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'Bonus_Transact_AECC_2017',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in Bonus_Transact_AECC_2017');
     END Bonus_Transact_AECC_2017;
--AEO-1821 END LAPP

--AEO-1938 BEGIN GD
     PROCEDURE DblePoints_Everything_102017(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      CASE
                        WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                             SUM(Di.Dtlsaleamount) > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM   (SELECT Dw.Txnheaderid,
                                   Dw.Dtlclasscode,
                                   Dw.Dtlsaleamount,
                                   Dw.Ipcode,
                                   Dw.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw2.Txnheaderid,
                                   Dw2.Dtlclasscode,
                                   Dw2.Dtlsaleamount,
                                   Dw2.Ipcode,
                                   Dw2.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw3.Txnheaderid,
                                   Dw3.Dtlclasscode,
                                   Dw3.Dtlsaleamount,
                                   Dw3.Ipcode,
                                   Dw3.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw4.Txnheaderid,
                                   Dw4.Dtlclasscode,
                                   Dw4.Dtlsaleamount,
                                   Dw4.Ipcode,
                                   Dw4.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw5.Txnheaderid,
                                   Dw5.Dtlclasscode,
                                   Dw5.Dtlsaleamount,
                                   Dw5.Ipcode,
                                   Dw5.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw6.Txnheaderid,
                                   Dw6.Dtlclasscode,
                                   Dw6.Dtlsaleamount,
                                   Dw6.Ipcode,
                                   Dw6.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
               WHERE  1 = 1
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                      AND
                      (
                      v_Txndate BETWEEN to_date('10/19/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('10/23/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
                      AND Di.Dtlclasscode NOT IN ('9911')   -- Remove giftcards
               GROUP  BY Di.Txnheaderid;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = '2017 FALL: Double Points on Everything';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Amount <> 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          '2017 FALL: Double Points on Everything' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          '2017 FALL: Double Points on Everything', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          '2017 FALL: Double Points on Everything' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_Everything_102017: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_Everything_102017',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_Everything_102017',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_Everything_102017');
     END DblePoints_Everything_102017;

     PROCEDURE DblePoints_102017_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_AECC_Count          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(v_Txnqualpurchaseamt) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      th.a_rowkey AS ParentRowKey
               FROM   (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk hw
                            WHERE  hw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk2 hw2
                            WHERE  hw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk3 hw3
                            WHERE  hw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk4 hw4
                            WHERE  hw4.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk5 hw5
                            WHERE  hw5.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk6 hw6
                            WHERE  hw6.Txnheaderid =  v_Txnheaderid) wh
               INNER JOIN bp_ae.ats_txnheader th
               ON th.a_rowkey = wh.txnoriginaltxnrowkey
               WHERE  1 = 1
                      AND wh.txnheaderid = v_Txnheaderid
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0 --Original purchase is not a gif
                      AND
                      ( --Original Purchase on Tuesday and in the range of dates that the bonus was active
                      th.a_txndate BETWEEN to_date('10/19/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('10/23/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = '2017 FALL: Double Points on Everything';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count > 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(NVL(v_AECC_Amount,0), 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  '2017 FALL: Double Points on Everything Return', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_102017_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_102017_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_102017_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_102017_Return');
     END DblePoints_102017_Return;
--AEO-1938 END GD
--AEO-1822 BEGIN LAPP
     PROCEDURE DblePnts_JeanBra_112017(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      CASE
                        WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                             SUM(Di.Dtlsaleamount) > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM   (SELECT Dw.Txnheaderid,
                                   Dw.Dtlclasscode,
                                   Dw.Dtlsaleamount,
                                   Dw.Ipcode,
                                   Dw.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw2.Txnheaderid,
                                   Dw2.Dtlclasscode,
                                   Dw2.Dtlsaleamount,
                                   Dw2.Ipcode,
                                   Dw2.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw3.Txnheaderid,
                                   Dw3.Dtlclasscode,
                                   Dw3.Dtlsaleamount,
                                   Dw3.Ipcode,
                                   Dw3.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw4.Txnheaderid,
                                   Dw4.Dtlclasscode,
                                   Dw4.Dtlsaleamount,
                                   Dw4.Ipcode,
                                   Dw4.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw5.Txnheaderid,
                                   Dw5.Dtlclasscode,
                                   Dw5.Dtlsaleamount,
                                   Dw5.Ipcode,
                                   Dw5.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw6.Txnheaderid,
                                   Dw6.Dtlclasscode,
                                   Dw6.Dtlsaleamount,
                                   Dw6.Ipcode,
                                   Dw6.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
               WHERE  1 = 1
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
					  --Transactions between 11/2-11/17
                      AND
                      (
                      v_Txndate BETWEEN to_date('11/2/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('11/17/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
					  --Only transactions that have jeans, bra or bralettes in the detail are elegible
                      AND Di.Dtlclasscode IN (select classcodes from bp_ae.ae_brapromocodes
                                            union
                          select classcodes from bp_ae.ae_jeanspromocodes)
               GROUP  BY Di.Txnheaderid;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'FALL: Double Points on Jeans, Bras and Bralettes';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Amount <> 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
							INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
							Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --If the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'FALL: Double Points on Jeans, Bras and Bralettes' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'FALL: Double Points on Jeans, Bras and Bralettes', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'FALL: Double Points on Jeans, Bras and Bralettes' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_JeanBra_112017: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_JeanBra_112017',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_JeanBra_112017',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_JeanBra_112017');
     END DblePnts_JeanBra_112017;

     PROCEDURE DblePnts_JeanBra_112017_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(pt.points) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      th.a_rowkey AS ParentRowKey
               FROM   (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk hw
                            WHERE  hw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk2 hw2
                            WHERE  hw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk3 hw3
                            WHERE  hw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk4 hw4
                            WHERE  hw4.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk5 hw5
                            WHERE  hw5.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk6 hw6
                            WHERE  hw6.Txnheaderid =  v_Txnheaderid) wh
               INNER JOIN bp_ae.ats_txnheader th
                    ON th.a_rowkey = wh.txnoriginaltxnrowkey
							 INNER JOIN bp_ae.lw_pointtransaction pt
							      ON th.a_rowkey = pt.rowkey and pt.transactiontype = 1
               WHERE  1 = 1
                      AND wh.txnheaderid = v_Txnheaderid
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0 --Original purchase is not a gift
                      AND
                      ( --Original Purchase in the range of dates that the bonus was active
                      th.a_txndate BETWEEN to_date('11/2/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('11/17/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND pt.pointeventid = v_Pointeventid
               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'FALL: Double Points on Jeans, Bras and Bralettes';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            /*SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Amount <> 0 THEN
              SELECT Round(v_Txnqualpurchaseamt * Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey)) AS Amount
              INTO v_AECC_Amount
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey
              AND tt.a_tendertype in (75,78)
              ;
            END IF;*/

            --We just return the same points we gave on the bonus
						--since we already have this when we get the pointevent
            v_BonusPoints := -v_Tbl(k).Txnqualpurchaseamt;

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  'FALL: Double Points on Jeans, Bras and Bralettes', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_JeanBra_112017_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_JeanBra_112017_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_JeanBra_112017_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_JeanBra_112017_Return');
     END DblePnts_JeanBra_112017_Return;
--AEO-1822 END LAPP
--AEO-1888 START LAPP
     PROCEDURE ExtraAccess_Reward_112017(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_RewardCount          NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT v_Vckey AS Vckey,
                      v_Header_Rowkey AS Originalrowkey,
                      v_Txndate AS Txndate,
                      v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
                      p_Processdate AS p_Processdate,
											vc.ipcode as IpCode,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      CASE
                        WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                             v_Txnqualpurchaseamt > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM  bp_ae.lw_virtualcard vc
                 inner join
                 --We validate we get the fartest finishing active tier or the latest created one.
                 (SELECT RANK() OVER (PARTITION BY mtiers.memberid ORDER BY mtiers.todate DESC, mtiers.createdate DESC) rnk, mtiers.*
                 --We validate that the txn is from an active extra acces tier and in the mentioned dates
                  FROM bp_ae.lw_membertiers mtiers where sysdate between mtiers.fromdate and mtiers.todate) mt
                 --We only need the current tier of the new transaction
                  --directly on the ae_rewardcount recording table
               	  on mt.memberid = vc.ipcode and mt.rnk = 1
                 inner join bp_ae.lw_tiers t
                 	  on t.tierid = mt.tierid AND t.tiername = 'Extra Access'
               	 --We will validate if we have already given out the reward to the current ipcode
                 left join bp_ae.ae_rewardcount arc
                   on arc.ipcode = vc.ipcode and arc.pointeventid = v_Pointeventid
               WHERE  1 = 1
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                      --Transactions between 11/10-11/13
                      AND
                      (
                      v_Txndate BETWEEN to_date('11/10/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('11/13/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
											AND vc.vckey = v_Vckey
                      --We validate we have not given the reward to this ipcode
                      AND arc.ipcode is null;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Extra Access: 2500 Bonus Points when you shop 11/10-11/13';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP

            --Check fi we already gave the bonus in the same recordset
          SELECT COUNT(*)
            INTO v_RewardCount
          FROM bp_ae.ae_rewardcount arc
          WHERE arc.ipcode = v_Tbl(k).IpCode and arc.pointeventid = v_Pointeventid;

          IF (v_RewardCount = 0) THEN
            --The reward is fixed to 2500 if it qulifies
          v_BonusPoints := 2500;

          IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Extra Access: 2500 Bonus Points when you shop 11/10-11/13' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'Extra Access: 2500 Bonus Points when you shop 11/10-11/13', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
          ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Extra Access: 2500 Bonus Points when you shop 11/10-11/13' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

                   LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
                   Commit;
          end if;
					insert into ae_rewardcount
					  (ipcode, lasttxnheaderid, createdate, updatedate, pointeventid, count)
					values
					  (v_Tbl(k).IpCode,
					   v_Tbl(k).Originalrowkey,
					   sysdate,
					   sysdate,
					   v_Pointeventid,
					   1);
           COMMIT;
          END IF;


          END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure ExtraAccess_Reward_112017: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'ExtraAccess_Reward_112017',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'ExtraAccess_Reward_112017',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in ExtraAccess_Reward_112017');
     END ExtraAccess_Reward_112017;


     --AEO-1888 END LAPP
     --AEO-1823 START LAPP
     PROCEDURE DblePoints_Everything_122017(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      CASE
                        WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                             SUM(Di.Dtlsaleamount) > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM   (SELECT Dw.Txnheaderid,
                                   Dw.Dtlclasscode,
                                   Dw.Dtlsaleamount,
                                   Dw.Ipcode,
                                   Dw.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw2.Txnheaderid,
                                   Dw2.Dtlclasscode,
                                   Dw2.Dtlsaleamount,
                                   Dw2.Ipcode,
                                   Dw2.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw3.Txnheaderid,
                                   Dw3.Dtlclasscode,
                                   Dw3.Dtlsaleamount,
                                   Dw3.Ipcode,
                                   Dw3.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw4.Txnheaderid,
                                   Dw4.Dtlclasscode,
                                   Dw4.Dtlsaleamount,
                                   Dw4.Ipcode,
                                   Dw4.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw5.Txnheaderid,
                                   Dw5.Dtlclasscode,
                                   Dw5.Dtlsaleamount,
                                   Dw5.Ipcode,
                                   Dw5.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw6.Txnheaderid,
                                   Dw6.Dtlclasscode,
                                   Dw6.Dtlsaleamount,
                                   Dw6.Ipcode,
                                   Dw6.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
               WHERE  1 = 1
                      AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                      AND
                      (
                      v_Txndate BETWEEN to_date('12/14/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('12/26/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
               GROUP  BY Di.Txnheaderid;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'HOLIDAY: Double Points on Everything--INCLUDING GIFT CARDS!';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78);

            IF v_AECC_Amount <> 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
							INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
							Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --If the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);


         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'HOLIDAY: Double Points on Everything--INCLUDING GIFT CARDS!' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'HOLIDAY: Double Points on Everything--INCLUDING GIFT CARDS!', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'HOLIDAY: Double Points on Everything--INCLUDING GIFT CARDS!' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_Everything_122017: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_Everything_122017',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_Everything_122017',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_Everything_122017');
     END DblePoints_Everything_122017;

     PROCEDURE DblePoints_122017_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_AECC_Count          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(v_Txnqualpurchaseamt) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      th.a_rowkey AS ParentRowKey
               FROM   (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk hw
                            WHERE  hw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk2 hw2
                            WHERE  hw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk3 hw3
                            WHERE  hw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk4 hw4
                            WHERE  hw4.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk5 hw5
                            WHERE  hw5.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk6 hw6
                            WHERE  hw6.Txnheaderid =  v_Txnheaderid) wh
               INNER JOIN bp_ae.ats_txnheader th
               ON th.a_rowkey = wh.txnoriginaltxnrowkey
               WHERE  1 = 1
                      AND wh.txnheaderid = v_Txnheaderid
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0 --Original purchase is not a gif
                      AND
                      ( --Original Purchase in the range of dates that the bonus was active
                      th.a_txndate BETWEEN to_date('12/14/2017 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('12/26/2017 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'HOLIDAY: Double Points on Everything--INCLUDING GIFT CARDS!';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
            AND tt.a_tendertype in (75,78);

            IF v_AECC_Count > 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
							INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
							Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(NVL(v_AECC_Amount,0), 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  'HOLIDAY: Double Points on Everything--INCLUDING GIFT CARDS! Return', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_102017_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_122017_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_122017_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_122017_Return');
     END DblePoints_122017_Return;
     --AEO-1823 END LAPP
     --AEO-2100 START LAPP
     PROCEDURE InactivityBonus072018(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_RewardCount          NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
						SELECT v_Vckey AS Vckey,
									v_Header_Rowkey AS Originalrowkey,
									v_Txndate AS Txndate,
									v_Txnqualpurchaseamt AS Txnqualpurchaseamt,
									p_Processdate AS p_Processdate,
									vc.ipcode as IpCode,
									10 AS Pointmultiplier,
									To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
									CASE
										WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
												 v_Txnqualpurchaseamt > 0 THEN
										 1
										ELSE
										 0
									END AS v_onhold --Pointsonhold flag
						FROM  bp_ae.ats_txnrewardredeem trr
						INNER JOIN bp_ae.lw_virtualcard vc
						      ON vc.vckey = trr.a_ipcode  --TXNREWARDREDEEM stores vckey in a_ipcode columns
						INNER JOIN bp_ae.AE_REWARDAUTHCODES ara
									ON ara.authcode = trr.A_CERTIFICATEREDEEMTYPE
						LEFT JOIN bp_ae.ae_rewardcount arc
									ON arc.ipcode = vc.ipcode and arc.pointeventid = v_Pointeventid
						WHERE  1 = 1
									AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
                  AND v_Header_Rowkey = trr.a_parentrowkey
									AND
									(
									v_Txndate BETWEEN to_date('1/1/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
									AND to_date('12/31/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
									)
									AND v_Txnqualpurchaseamt > 0
									AND ara.pointeventid = v_Pointeventid
									AND p_Processdate BETWEEN nvl(ara.validfrom, p_Processdate-1) AND NVL(ara.validto, p_Processdate+1)
									AND arc.ipcode is null;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = '2018: Just for you! AEO Connected Bonus Points';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP

          --Check if we already gave the bonus in the same recordset (it shuldn't but just in case)
          SELECT COUNT(*)
            INTO v_RewardCount
          FROM bp_ae.ae_rewardcount arc
          WHERE arc.ipcode = v_Tbl(k).IpCode and arc.pointeventid = v_Pointeventid;

          IF (v_RewardCount = 0) THEN
            --The reward is fixed to 500 if it qualifies
          v_BonusPoints := 500;

                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          '2018:?Just for you! AEO Connected Bonus Points' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

                   LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
                   Commit;

					insert into ae_rewardcount
					  (ipcode, lasttxnheaderid, createdate, updatedate, pointeventid, count)
					values
					  (v_Tbl(k).IpCode,
					   v_Tbl(k).Originalrowkey,
					   sysdate,
					   sysdate,
					   v_Pointeventid,
					   1);
           COMMIT;
          END IF;


          END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure InactivityBonus072018: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'InactivityBonus072018',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'InactivityBonus072018',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in InactivityBonus072018');
     END InactivityBonus072018;
     --AEO-2100 END LAPP
     --AEO-2111 BEGIN LAPP
     PROCEDURE DbleCred_Bra_012018(v_Detailitem_Rowkey NUMBER,
                        v_Ipcode            NUMBER,
                        v_Txntypeid         NUMBER,
                        v_Dtlsaleamount     FLOAT,
                        v_Txndate           DATE,
                        v_Txnheaderid       NVARCHAR2,
                        v_Dtlclasscode      NVARCHAR2,
                        v_My_Log_Id         NUMBER,
                        p_Processdate       TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Point           NUMBER := 0;
          v_AECC_Count           NUMBER := 0;
          v_BonusPoints          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Ipcode) || ' ' || 'RowKey: ' || TO_CHAR(v_Detailitem_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT v_Ipcode            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate,
                      dtlquantity,
                      CASE
                        WHEN trunc(v_Txndate) >= TRUNC(SYSDATE - 14) AND
                             v_Dtlsaleamount > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM (SELECT Dw.dtl_rowkey,
                             Dw.dtlclearanceitem,
                             Dw.Dtlclasscode,
                             Dw.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk Dw
                      WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw2.dtl_rowkey,
                             Dw2.dtlclearanceitem,
                             Dw2.Dtlclasscode,
                             Dw2.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk2 Dw2
                      WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw3.dtl_rowkey,
                             Dw3.dtlclearanceitem,
                             Dw3.Dtlclasscode,
                             Dw3.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk3 Dw3
                      WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw4.dtl_rowkey,
                             Dw4.dtlclearanceitem,
                             Dw4.Dtlclasscode,
                             Dw4.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk4 Dw4
                      WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw5.dtl_rowkey,
                             Dw5.dtlclearanceitem,
                             Dw5.Dtlclasscode,
                             Dw5.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk5 Dw5
                      WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw6.dtl_rowkey,
                             Dw6.dtlclearanceitem,
                             Dw6.Dtlclasscode,
                             Dw6.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk6 Dw6
                      WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey) tdi
                      INNER JOIN
                      (
                             (SELECT t.Column_Value AS Classcode
                             FROM TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                             FROM Ats_Bonuseventdetail Bd WHERE  Bd.a_Pointeventid = v_Pointeventid),',')) t)
                      ) bde
                      ON bde.Classcode = tdi.dtlclasscode
                      WHERE  1 = 1
                      AND v_Txntypeid = 1
                      AND tdi.dtlclasscode = v_Dtlclasscode
                      AND v_Dtlsaleamount > .01
                      AND nvl(tdi.dtlclearanceitem,0) = 0 -- Exclude Clearance
                      AND
                      (
                      v_Txndate BETWEEN to_date('1/25/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('2/19/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      );
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Bonus Bra Points';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP

         v_BonusPoints := v_Tbl(k).dtlquantity;

         IF (v_Tbl(k).v_onhold = 1) then

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints /*B5G1 Bonus Bra Points*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bonus Bra Points' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          returning Pointtransactionid into v_Pointtransactionid;
                    --Insert PointsOnHold Record
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints /*B5G1 Bonus Bra Points*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bonus Bra Points' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
                          LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;

               elsif (v_Tbl(k).v_onhold = 0) then

                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          --         ,Last_Dml_Date   // AEO-74 Upgrade 4.5 changes begin here -----------SCJ
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Purchase - 1*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints /*B5G1 Bonus Bra Points*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bonus Bra Points' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)
			                    LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DbleCred_Bra_012018: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DbleCred_Bra_012018',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DbleCred_Bra_012018',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DbleCred_Bra_012018');
     END DbleCred_Bra_012018;

     PROCEDURE DbleCred_Bra_012018_Return(v_Detailitem_Rowkey NUMBER,
                               v_Ipcode            NUMBER,
                               v_Txntypeid         NUMBER,
                               v_Dtlsaleamount     FLOAT,
                               v_Txndate           DATE,
                               v_Txnheaderid       NVARCHAR2,
                               v_Dtlclasscode      NVARCHAR2,
                               v_My_Log_Id         NUMBER,
                               p_Processdate       TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Point           NUMBER := 0;
          v_AECC_Count           NUMBER := 0;
          v_BonusPoints          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Ipcode) || ' ' || 'RowKey: ' || TO_CHAR(v_Detailitem_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT v_Ipcode            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate,
                      Dtlquantity
               FROM (SELECT Dw.dtl_rowkey,
                             Dw.dtlclearanceitem,
                             Dw.Dtlclasscode,
                             Dw.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk Dw
                      WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw2.dtl_rowkey,
                             Dw2.dtlclearanceitem,
                             Dw2.Dtlclasscode,
                             Dw2.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk2 Dw2
                      WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw3.dtl_rowkey,
                             Dw3.dtlclearanceitem,
                             Dw3.Dtlclasscode,
                             Dw3.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk3 Dw3
                      WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw4.dtl_rowkey,
                             Dw4.dtlclearanceitem,
                             Dw4.Dtlclasscode,
                             Dw4.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk4 Dw4
                      WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw5.dtl_rowkey,
                             Dw5.dtlclearanceitem,
                             Dw5.Dtlclasscode,
                             Dw5.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk5 Dw5
                      WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                      UNION ALL
                      SELECT Dw6.dtl_rowkey,
                             Dw6.dtlclearanceitem,
                             Dw6.Dtlclasscode,
                             Dw6.Dtlquantity --In case the Record has a quantity <> than 1
                      FROM   Lw_Txndetailitem_Wrk6 Dw6
                      WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey) tdi
                      INNER JOIN
                      (
                             (SELECT t.Column_Value AS Classcode
                             FROM TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes
                             FROM Ats_Bonuseventdetail Bd WHERE  Bd.a_Pointeventid = v_Pointeventid),',')) t)
                      ) bde
                      ON     bde.Classcode = tdi.dtlclasscode
               INNER JOIN
                      (SELECT Txnoriginaltxndate, Txnheaderid, txnoriginaltxnrowkey
                       FROM   Lw_Txnheader_Wrk
                       WHERE  Txnheaderid = v_Txnheaderid
                       UNION ALL
                       SELECT Txnoriginaltxndate, Txnheaderid, txnoriginaltxnrowkey
                       FROM   Lw_Txnheader_Wrk2
                       WHERE  Txnheaderid = v_Txnheaderid
                       UNION ALL
                       SELECT Txnoriginaltxndate, Txnheaderid, txnoriginaltxnrowkey
                       FROM   Lw_Txnheader_Wrk3
                       WHERE  Txnheaderid = v_Txnheaderid
                       UNION ALL
                       SELECT Txnoriginaltxndate, Txnheaderid, txnoriginaltxnrowkey
                       FROM   Lw_Txnheader_Wrk4
                       WHERE  Txnheaderid = v_Txnheaderid
                       UNION ALL
                       SELECT Txnoriginaltxndate, Txnheaderid, txnoriginaltxnrowkey
                       FROM   Lw_Txnheader_Wrk5
                       WHERE  Txnheaderid = v_Txnheaderid
                       UNION ALL
                       SELECT Txnoriginaltxndate, Txnheaderid, txnoriginaltxnrowkey
                       FROM   Lw_Txnheader_Wrk6
                       WHERE  Txnheaderid = v_Txnheaderid)  thw
                  ON thw.Txnheaderid = v_Txnheaderid
               INNER JOIN bp_ae.ats_txnheader purchaseth
                    ON purchaseth.a_rowkey = nvl(thw.txnoriginaltxnrowkey, 0)
               WHERE  1 = 1
                      AND v_Txntypeid = 2
                      AND tdi.dtlclasscode = v_Dtlclasscode
                      AND thw.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND nvl(tdi.dtlclearanceitem,0) = 0 -- Exclude Clearance
                      AND
                      ( --Original Purchase in the range of dates that the bonus was active
                      purchaseth.a_txndate BETWEEN to_date('1/25/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('2/19/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      );
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Bra Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'B5G1 Bonus Bra Points';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          --// AEO-74 Upgrade 4.5 changes end here -----------SCJ
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid=Bra Return*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          (-1 * v_Tbl(k).dtlquantity) /*Bra Return Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Bonus Bra Points Return' /*Notes*/,
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DbleCred_Bra_012018_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DbleCred_Bra_012018_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DbleCred_Bra_012018_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DbleCred_Bra_012018_Return');
     END DbleCred_Bra_012018_Return;
    --AEO-2111 END LAPP

--AEO-2110 Begin GD
     PROCEDURE DblePnts_Jean_012018(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Amount          FLOAT := 0;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Count           NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Vckey) || ' ' || 'RowKey: ' || TO_CHAR(v_Header_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
            SELECT MAX(v_Vckey) AS Vckey,
              MAX(v_Header_Rowkey) AS Originalrowkey,
              MAX(v_Txndate) AS Txndate,
              SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
              MAX(p_Processdate) AS p_Processdate,
              10 AS Pointmultiplier,
              To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
              CASE
                WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                     SUM(Di.Dtlsaleamount) > 0 THEN
                 1
                ELSE
                 0
              END AS v_onhold --Pointsonhold flag
            FROM
              (SELECT Dw.Txnheaderid,
               Dw.Dtlclasscode,
               Dw.Dtlsaleamount,
               Dw.Ipcode,
               Dw.Parentrowkey
              FROM   Lw_Txndetailitem_Wrk Dw
              WHERE  Dw.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw2.Txnheaderid,
               Dw2.Dtlclasscode,
               Dw2.Dtlsaleamount,
               Dw2.Ipcode,
               Dw2.Parentrowkey
              FROM   Lw_Txndetailitem_Wrk2 Dw2
              WHERE  Dw2.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw3.Txnheaderid,
               Dw3.Dtlclasscode,
               Dw3.Dtlsaleamount,
               Dw3.Ipcode,
               Dw3.Parentrowkey
              FROM   Lw_Txndetailitem_Wrk3 Dw3
              WHERE  Dw3.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw4.Txnheaderid,
               Dw4.Dtlclasscode,
               Dw4.Dtlsaleamount,
               Dw4.Ipcode,
               Dw4.Parentrowkey
              FROM   Lw_Txndetailitem_Wrk4 Dw4
              WHERE  Dw4.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw5.Txnheaderid,
               Dw5.Dtlclasscode,
               Dw5.Dtlsaleamount,
               Dw5.Ipcode,
               Dw5.Parentrowkey
              FROM   Lw_Txndetailitem_Wrk5 Dw5
              WHERE  Dw5.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw6.Txnheaderid,
               Dw6.Dtlclasscode,
               Dw6.Dtlsaleamount,
               Dw6.Ipcode,
               Dw6.Parentrowkey
              FROM   Lw_Txndetailitem_Wrk6 Dw6
              WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
            WHERE 1 = 1
            AND v_Txnqualpurchaseamt > 0 --AEO-2334 GD
            AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
            AND
            (
            v_Txndate BETWEEN to_date('01/25/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
            AND to_date('02/19/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
            )
            AND Di.Dtlclasscode IN
            (SELECT t.Column_Value AS Classcode
             FROM TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes FROM Ats_Bonuseventdetail Bd WHERE  Bd.a_Pointeventid = v_Pointeventid),',')) t)
            GROUP  BY Di.Txnheaderid
            ;

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'SPRING 2018: Double Points on Jeans';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count <> 0 THEN
              SELECT sum(Round(v_Tbl(k).Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'SPRING 2018: Double Points on Jeans' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'SPRING 2018: Double Points on Jeans', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'SPRING 2018: Double Points on Jeans' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               COMMIT;
               END IF;
           COMMIT;

           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_Jean_012018: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_Jean_012018',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_Jean_012018',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_Jean_012018');
     END DblePnts_Jean_012018;

     PROCEDURE DblePnts_Jean_012018_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_AECC_Count           NUMBER := 0;
          v_BonusPointsPurchase  FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Vckey) || ' ' || 'RowKey: ' || TO_CHAR(v_Header_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      MAX(th.a_rowkey) AS ParentRowKey
               FROM
                  (SELECT Dw.Txnheaderid,
                   Dw.Dtlclasscode,
                   Dw.Dtlsaleamount,
                   Dw.Ipcode,
                   Dw.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk Dw
                  WHERE  Dw.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw2.Txnheaderid,
                   Dw2.Dtlclasscode,
                   Dw2.Dtlsaleamount,
                   Dw2.Ipcode,
                   Dw2.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk2 Dw2
                  WHERE  Dw2.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw3.Txnheaderid,
                   Dw3.Dtlclasscode,
                   Dw3.Dtlsaleamount,
                   Dw3.Ipcode,
                   Dw3.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk3 Dw3
                  WHERE  Dw3.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw4.Txnheaderid,
                   Dw4.Dtlclasscode,
                   Dw4.Dtlsaleamount,
                   Dw4.Ipcode,
                   Dw4.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk4 Dw4
                  WHERE  Dw4.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw5.Txnheaderid,
                   Dw5.Dtlclasscode,
                   Dw5.Dtlsaleamount,
                   Dw5.Ipcode,
                   Dw5.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk5 Dw5
                  WHERE  Dw5.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw6.Txnheaderid,
                   Dw6.Dtlclasscode,
                   Dw6.Dtlsaleamount,
                   Dw6.Ipcode,
                   Dw6.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk6 Dw6
                  WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
                INNER JOIN
                  (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey
                    FROM   bp_ae.lw_txnheader_wrk hw
                    WHERE  hw.Txnheaderid = v_Txnheaderid
                    UNION ALL
                    SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey
                    FROM   bp_ae.lw_txnheader_wrk2 hw2
                    WHERE  hw2.Txnheaderid = v_Txnheaderid
                    UNION ALL
                    SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey
                    FROM   bp_ae.lw_txnheader_wrk3 hw3
                    WHERE  hw3.Txnheaderid = v_Txnheaderid
                    UNION ALL
                    SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey
                    FROM   bp_ae.lw_txnheader_wrk4 hw4
                    WHERE  hw4.Txnheaderid =  v_Txnheaderid
                    UNION ALL
                    SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey
                    FROM   bp_ae.lw_txnheader_wrk5 hw5
                    WHERE  hw5.Txnheaderid =  v_Txnheaderid
                    UNION ALL
                    SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey
                    FROM   bp_ae.lw_txnheader_wrk6 hw6
                    WHERE  hw6.Txnheaderid =  v_Txnheaderid) wh
               ON wh.txnheaderid = Di.txnheaderid
               INNER JOIN bp_ae.ats_txnheader th
               ON th.a_rowkey = wh.txnoriginaltxnrowkey
               WHERE  1 = 1
                      AND wh.txnheaderid = v_Txnheaderid
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0 --Original purchase is not a gif
                      AND
                      ( --Original Purchase in the range of dates that the bonus was active
                      th.a_txndate BETWEEN to_date('01/25/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('02/19/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND Di.Dtlclasscode IN
                    (SELECT t.Column_Value AS Classcode
                     FROM TABLE(Stringtotable((SELECT Bd.a_Eventclasscodes FROM Ats_Bonuseventdetail Bd WHERE  Bd.a_Pointeventid = v_Pointeventid),',')) t)
               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'SPRING 2018: Double Points on Jeans';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count > 0 THEN
              SELECT sum(Round(v_Tbl(k).Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(NVL(v_AECC_Amount,0), 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  'SPRING 2018: Double Points on Jeans Return', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_Jean_012018_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_Jean_012018_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_Jean_012018_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_Jean_012018_Return');
     END DblePnts_Jean_012018_Return;
--AEO-2110 Begin GD

--AEO-2363 Begin
     PROCEDURE DblePnts_Swimwear_2018(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Amount          FLOAT := 0;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Count           NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Vckey) || ' ' || 'RowKey: ' || TO_CHAR(v_Header_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
            SELECT MAX(v_Vckey) AS Vckey,
              MAX(v_Header_Rowkey) AS Originalrowkey,
              MAX(v_Txndate) AS Txndate,
              SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
              MAX(p_Processdate) AS p_Processdate,
              10 AS Pointmultiplier,
              To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
              CASE
                WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                     SUM(Di.Dtlsaleamount) > 0 THEN
                 1
                ELSE
                 0
              END AS v_onhold --Pointsonhold flag
            FROM
              (SELECT Dw.Txnheaderid,
               Dw.Dtlclasscode,
               Dw.Dtlsaleamount,
               Dw.Ipcode,
               Dw.Parentrowkey,
               hw.txnstoreid
              FROM   Lw_Txndetailitem_Wrk Dw
              Inner join bp_ae.Lw_Txnheader_Wrk hw on Dw.txnheaderid = hw.txnheaderid
              WHERE  Dw.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw2.Txnheaderid,
               Dw2.Dtlclasscode,
               Dw2.Dtlsaleamount,
               Dw2.Ipcode,
               Dw2.Parentrowkey,
               hw2.txnstoreid
              FROM   Lw_Txndetailitem_Wrk2 Dw2
              Inner join bp_ae.Lw_Txnheader_Wrk2 hw2 on Dw2.txnheaderid = hw2.txnheaderid
              WHERE  Dw2.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw3.Txnheaderid,
               Dw3.Dtlclasscode,
               Dw3.Dtlsaleamount,
               Dw3.Ipcode,
               Dw3.Parentrowkey,
               hw3.txnstoreid
              FROM   Lw_Txndetailitem_Wrk3 Dw3
              Inner join bp_ae.Lw_Txnheader_Wrk3 hw3 on Dw3.txnheaderid = hw3.txnheaderid
              WHERE  Dw3.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw4.Txnheaderid,
               Dw4.Dtlclasscode,
               Dw4.Dtlsaleamount,
               Dw4.Ipcode,
               Dw4.Parentrowkey,
               hw4.txnstoreid
              FROM   Lw_Txndetailitem_Wrk4 Dw4
               Inner join bp_ae.Lw_Txnheader_Wrk4 hw4 on Dw4.txnheaderid = hw4.txnheaderid
              WHERE  Dw4.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw5.Txnheaderid,
               Dw5.Dtlclasscode,
               Dw5.Dtlsaleamount,
               Dw5.Ipcode,
               Dw5.Parentrowkey,
               hw5.txnstoreid
              FROM   Lw_Txndetailitem_Wrk5 Dw5
              Inner join bp_ae.Lw_Txnheader_Wrk5 hw5 on Dw5.txnheaderid = hw5.txnheaderid
              WHERE  Dw5.Txnheaderid = v_Txnheaderid
              UNION ALL
              SELECT Dw6.Txnheaderid,
               Dw6.Dtlclasscode,
               Dw6.Dtlsaleamount,
               Dw6.Ipcode,
               Dw6.Parentrowkey,
               hw6.txnstoreid
              FROM   Lw_Txndetailitem_Wrk6 Dw6
              Inner join bp_ae.Lw_Txnheader_Wrk6 hw6 on Dw6.txnheaderid = hw6.txnheaderid
              WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
           inner join bp_ae.lw_storedef st on st.storeid = di.txnstoreid
            WHERE 1 = 1
            AND v_Txnqualpurchaseamt > 0 --AEO-2334 GD
            AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
            AND
            (
            v_Txndate BETWEEN to_date('04/23/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                              AND to_date('05/09/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
            )

            and  st.storenumber  not in  ('2707','2708','2718','2729','2734','2755','2767','2770','2772',
                                       '2793','2799','2808','2862','2865','2870','2872')
            AND Di.Dtlclasscode IN     ( '750','751','752','753','754','1753','1754','1756','2750',
                                         '2752','2756','2758')
            GROUP  BY Di.Txnheaderid
            ;

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Spring 2018: Double Points on Swimwear';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count <> 0 THEN
              SELECT sum(Round(v_Tbl(k).Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) +
                              (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Spring 2018 Promotion: Double Points on Women''s Swimwear' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'Spring 2018 Promotion: Double Points on Women''s Swimwear', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Spring 2018 Promotion: Double Points on Women''s Swimwear' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               COMMIT;
               END IF;
           COMMIT;

           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_Swimwear_2018: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_Swimwear_2018',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_Swimwear_2018',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_Swimwear_2018');
     END DblePnts_Swimwear_2018;

     PROCEDURE DblePnts_Swimwear_2018_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_AECC_Count           NUMBER := 0;
          v_BonusPointsPurchase  FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Vckey) || ' ' || 'RowKey: ' || TO_CHAR(v_Header_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      MAX(th.a_rowkey) AS ParentRowKey
               FROM
                  (SELECT Dw.Txnheaderid,
                   Dw.Dtlclasscode,
                   Dw.Dtlsaleamount,
                   Dw.Ipcode,
                   Dw.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk Dw
                  WHERE  Dw.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw2.Txnheaderid,
                   Dw2.Dtlclasscode,
                   Dw2.Dtlsaleamount,
                   Dw2.Ipcode,
                   Dw2.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk2 Dw2
                  WHERE  Dw2.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw3.Txnheaderid,
                   Dw3.Dtlclasscode,
                   Dw3.Dtlsaleamount,
                   Dw3.Ipcode,
                   Dw3.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk3 Dw3
                  WHERE  Dw3.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw4.Txnheaderid,
                   Dw4.Dtlclasscode,
                   Dw4.Dtlsaleamount,
                   Dw4.Ipcode,
                   Dw4.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk4 Dw4
                  WHERE  Dw4.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw5.Txnheaderid,
                   Dw5.Dtlclasscode,
                   Dw5.Dtlsaleamount,
                   Dw5.Ipcode,
                   Dw5.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk5 Dw5
                  WHERE  Dw5.Txnheaderid = v_Txnheaderid
                  UNION ALL
                  SELECT Dw6.Txnheaderid,
                   Dw6.Dtlclasscode,
                   Dw6.Dtlsaleamount,
                   Dw6.Ipcode,
                   Dw6.Parentrowkey
                  FROM   Lw_Txndetailitem_Wrk6 Dw6
                  WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
                INNER JOIN
                  (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey, hw.txnstoreid
                    FROM   bp_ae.lw_txnheader_wrk hw
                    WHERE  hw.Txnheaderid = v_Txnheaderid
                    UNION ALL
                    SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey, hw2.txnstoreid
                    FROM   bp_ae.lw_txnheader_wrk2 hw2
                    WHERE  hw2.Txnheaderid = v_Txnheaderid
                    UNION ALL
                    SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey, hw3.txnstoreid
                    FROM   bp_ae.lw_txnheader_wrk3 hw3
                    WHERE  hw3.Txnheaderid = v_Txnheaderid
                    UNION ALL
                    SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey, hw4.txnstoreid
                    FROM   bp_ae.lw_txnheader_wrk4 hw4
                    WHERE  hw4.Txnheaderid =  v_Txnheaderid
                    UNION ALL
                    SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey, hw5.txnstoreid
                    FROM   bp_ae.lw_txnheader_wrk5 hw5
                    WHERE  hw5.Txnheaderid =  v_Txnheaderid
                    UNION ALL
                    SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey, hw6.txnstoreid
                    FROM   bp_ae.lw_txnheader_wrk6 hw6
                    WHERE  hw6.Txnheaderid =  v_Txnheaderid
                          ) wh
               ON wh.txnheaderid = Di.txnheaderid
               INNER JOIN bp_ae.ats_txnheader th  ON th.a_rowkey = wh.txnoriginaltxnrowkey
               Inner join bp_ae.lw_storedef st on st.storeid = th.a_txnstoreid
               WHERE  1 = 1
                      AND wh.txnheaderid = v_Txnheaderid
                       ---- AEO-2447 begin
                        and  st.storenumber  not in  ('2707','2708','2718','2729','2734','2755','2767','2770','2772',
                                       '2793','2799','2808','2862','2865','2870','2872')

                        AND Di.Dtlclasscode IN     ( '750','751','752','753','754','1753','1754','1756','2750',
                                         '2752','2756','2758')
                     ---- AEO-2447 end
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0 --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0 --Original purchase is not a gif
                      AND
                            (
                            th.a_txndate BETWEEN to_date('04/23/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                                              AND to_date('05/09/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                            )


               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Spring 2018: Double Points on Swimwear';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count > 0 THEN
              SELECT sum(Round(v_Tbl(k).Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) +
                              (Round(NVL(v_AECC_Amount,0), 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  'Spring 2018 Promotion: Double Points on Women''s Swimwear Return', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_Swimwear_2018_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_Swimwear_2018_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_Swimwear_2018_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_Swimwear_2018_Return');
     END DblePnts_Swimwear_2018_Return;
--AEO-2363 end

-- AEO-2364 begin
PROCEDURE B5g1_Shorts(v_Detailitem_Rowkey NUMBER,
                         v_Ipcode            NUMBER,
                         v_Txntypeid         NUMBER,
                         v_Dtlsaleamount     FLOAT,
                         v_Txndate           DATE,
                         v_Txnheaderid       NVARCHAR2,
                         v_Dtlclasscode      NVARCHAR2,
                         v_My_Log_Id         NUMBER,
                         p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;
          --cursor1-
          CURSOR Process_Rule IS
               SELECT Vc.Vckey            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      tdi.dtlquantity,  -- we need the number of items becuase we will award 1 bonus poitn for each item
                      p_Processdate,
                      Case
                        When trunc(v_Txndate) >= trunc(sysdate - 14) and
                             v_Dtlsaleamount > 0 then
                         tdi.dtlquantity -- also the points on hold will be based on the number of shorts
                        else
                         0
                      End as v_onhold --Pointsonhold flag
               FROM   (SELECT Vckey, Ipcode
                       FROM   Lw_Virtualcard
                       WHERE  Vckey = v_Ipcode) Vc
               INNER  JOIN Ats_Memberdetails Md    ON     Md.a_Ipcode = Vc.Ipcode
               Inner  join ats_txnheader th on th.a_txnheaderid = v_Txnheaderid
               INNER JOIN (SELECT Dw.dtl_rowkey,
                                   Dw.dtlclearanceitem,
                                   dw.dtlquantity,
                                   wk.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            INNER join  Lw_Txnheader_Wrk wk on wk.txnheaderid = dw.txnheaderid
                            WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                            --AEO-2445 begin
                                  and dw.dtlclasscode = v_Dtlclasscode
                                 /* and dw.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                            --AEO-2445 end
                            UNION ALL
                            SELECT Dw2.dtl_rowkey,
                                   Dw2.dtlclearanceitem,
                                   dw2.dtlquantity,
                                   wk2.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            INNER join  Lw_Txnheader_Wrk2 wk2 on wk2.txnheaderid = dw2.txnheaderid
                            WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                               --AEO-2445 begin
                                  and dw2.dtlclasscode = v_Dtlclasscode
                                 /* and dw2.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                            --AEO-2445 end

                            UNION ALL
                            SELECT Dw3.dtl_rowkey,
                                   Dw3.dtlclearanceitem,
                                   dw3.dtlquantity,
                                   wk3.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            INNER join  Lw_Txnheader_Wrk3 wk3 on wk3.txnheaderid = dw3.txnheaderid
                            WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                               --AEO-2445 begin
                                  and dw3.dtlclasscode = v_Dtlclasscode
                                 /* and dw3.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                            --AEO-2445 end

                            UNION ALL
                            SELECT Dw4.dtl_rowkey,
                                   Dw4.dtlclearanceitem,
                                   dw4.dtlquantity,
                                   wk4.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            INNER join  Lw_Txnheader_Wrk4 wk4 on wk4.txnheaderid = dw4.txnheaderid
                            WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                               --AEO-2445 begin
                                  and dw4.dtlclasscode = v_Dtlclasscode
                                 /* and dw4.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                            --AEO-2445 end

                            UNION ALL
                            SELECT Dw5.dtl_rowkey,
                                   Dw5.dtlclearanceitem,
                                   dw5.dtlquantity,
                                   wk5.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            INNER join  Lw_Txnheader_Wrk5 wk5 on wk5.txnheaderid = dw5.txnheaderid
                            WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                               --AEO-2445 begin
                                  and dw5.dtlclasscode = v_Dtlclasscode
                                 /* and dw.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                            --AEO-2445 end

                            UNION ALL
                            SELECT Dw6.dtl_rowkey,
                                   Dw6.dtlclearanceitem,
                                   dw6.dtlquantity,
                                   wk6.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            INNER join  Lw_Txnheader_Wrk6 wk6 on wk6.txnheaderid = dw6.txnheaderid
                            WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey
                             --AEO-2445 begin
                                  and dw6.dtlclasscode = v_Dtlclasscode
                                 /* and dw6.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                            --AEO-2445 end
                                  ) tdi
               ON     tdi.dtl_rowkey = v_Detailitem_rowkey

               WHERE  1 = 1
                      AND v_Txntypeid = 1
                      AND v_Dtlsaleamount > .01
                      AND nvl(tdi.dtlclearanceitem, 0) = 0
                      AND (Ae_Isinpilot(Md.a_Extendedplaycode) = 1)
                      AND v_Txndate >= v_Txnstartdt
                      and v_Txndate >= to_date ('5/3/2018 00:00:00','mm/dd/yyyy HH24:mi:ss')
                      and v_Txndate <=  to_date ('6/15/2018 23:59:59','mm/dd/yyyy HH24:mi:ss') --AEO-2513
               --       and th.a_txnqualpurchaseamt > 0
                      and v_Dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332');

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Summer 2018: B5G1 Jean Shorts Promo';

          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
         OPEN Process_Rule;
          LOOP
           FETCH Process_Rule BULK COLLECT  INTO v_Tbl LIMIT 10000;
           FOR k IN 1 .. v_tbl.count
              LOOP
                IF (v_Tbl(k).v_onhold = 1) then

                               INSERT INTO Lw_Pointtransaction
                                   (Pointtransactionid,
                                    Vckey,
                                    Pointtypeid,
                                    Pointeventid,
                                    Transactiontype,
                                    Transactiondate,
                                    Pointawarddate,
                                    Points,
                                    Expirationdate,
                                    Notes,
                                    Ownertype,
                                    Ownerid,
                                    Rowkey,
                                    Parenttransactionid,
                                    Pointsconsumed,
                                    Pointsonhold,
                                    Ptlocationid,
                                    Ptchangedby,
                                    Createdate,
                                    Expirationreason)
                              VALUES
                                   (Seq_Pointtransactionid.Nextval,
                                    v_Tbl(k).Vckey,
                                    v_Pointtypeid /*Pointtypeid*/,
                                    v_Pointeventid /*pointeventid*/,
                                    v_Tbl(k).v_Txntypeid,
                                    v_Tbl(k).Txndate,
                                    --SYSDATE,
                                    v_Tbl(k).p_Processdate,
                                    nvl(v_Tbl(k).dtlquantity,1) /*Jean Purchase Point*/,
                                    To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                                    'B5G1 Jean Shorts Eligible for B5G1 Jean Credit (5/3/2018 - 6/15/2018)' /*Notes*/, --AEO-2513
                                    1,
                                    104,
                                    v_Tbl(k).Originalrowkey,
                                    -1,
                                    0 /*Pointsconsumed*/,
                                    nvl(v_Tbl(k).v_onhold,1) /*Jean Purchase Pointsonhold*/,
                                    NULL,
                                    'RulesProcessing' /*Ptchangedby*/,
                                    SYSDATE,
                                    NULL)
                                    returning Pointtransactionid into v_Pointtransactionid;
                              --Insert PointsOnHold Record
                              INSERT INTO Lw_Pointtransaction
                                   (Pointtransactionid,
                                    Vckey,
                                    Pointtypeid,
                                    Pointeventid,
                                    Transactiontype,
                                    Transactiondate,
                                    Pointawarddate,
                                    Points,
                                    Expirationdate,
                                    Notes,
                                    Ownertype,
                                    Ownerid,
                                    Rowkey,
                                    Parenttransactionid,
                                    Pointsconsumed,
                                    Pointsonhold,
                                    Ptlocationid,
                                    Ptchangedby,
                                    Createdate,
                                    Expirationreason)
                              VALUES
                                   (Seq_Pointtransactionid.Nextval,
                                    v_Tbl(k).Vckey,
                                    v_Pointtypeid /*Pointtypeid*/,
                                    v_Pointeventid /*pointeventid*/,
                                    3 /*pointsonhold*/,
                                    v_Tbl(k).Txndate,
                                    --SYSDATE,
                                    v_Tbl(k).p_Processdate,
                                    nvl(v_Tbl(k).dtlquantity,1) /*Jean Purchase Point*/,
                                    To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                                    'B5G1 Jean Shorts Eligible for B5G1 Jean Credit (5/3/2018 - 6/15/2018)' /*Notes*/, --AEO-2513
                                    1,
                                    104,
                                    v_Tbl(k).Originalrowkey,
                                    v_Pointtransactionid,
                                    0 /*Pointsconsumed*/,
                                    0 /*Pointsonhold*/,
                                    NULL,
                                    'RulesProcessing' /*Ptchangedby*/,
                                    SYSDATE,
                                    NULL)
                                    LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue

                   elsif (v_Tbl(k).v_onhold = 0) then

                             INSERT INTO Lw_Pointtransaction
                                   (Pointtransactionid,
                                    Vckey,
                                    Pointtypeid,
                                    Pointeventid,
                                    Transactiontype,
                                    Transactiondate,
                                    Pointawarddate,
                                    Points,
                                    Expirationdate,
                                    Notes,
                                    Ownertype,
                                    Ownerid,
                                    Rowkey,
                                    Parenttransactionid,
                                    Pointsconsumed,
                                    Pointsonhold,
                                    Ptlocationid,
                                    Ptchangedby,
                                    Createdate,
                                    Expirationreason)
                              VALUES
                                   (Seq_Pointtransactionid.Nextval,
                                    v_Tbl(k).Vckey,
                                    v_Pointtypeid /*Pointtypeid*/,
                                    v_Pointeventid /*pointeventid*/,
                                    v_Tbl(k).v_Txntypeid,
                                    v_Tbl(k).Txndate,
                                    --SYSDATE,
                                    v_Tbl(k).p_Processdate,
                                    nvl(v_Tbl(k).dtlquantity,1)  /*Jean Purchase Point*/,
                                    To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                                    'B5G1 Jean Shorts Eligible for B5G1 Jean Credit (5/3/2018 - 6/15/2018)' /*Notes*/, --AEO-2513
                                    1,
                                    104,
                                    v_Tbl(k).Originalrowkey,
                                    -1,
                                    0 /*Pointsconsumed*/,
                                    0 /*Pointsonhold*/,
                                    NULL,
                                    'RulesProcessing' /*Ptchangedby*/,
                                    SYSDATE,
                                    NULL)
                                    LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
                   end if;
                COMMIT;
              END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx).Error_Code);
                    v_Reason         := 'Failed Procedure B5G1_Shorts: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'B5G1_Shorts',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'B5G1_Shorts',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in B5G1_Shorts');
     END B5g1_Shorts;


  PROCEDURE B5g1_Shorts_Return(v_Detailitem_Rowkey NUMBER,
                                v_Ipcode            NUMBER,
                                v_Txntypeid         NUMBER,
                                v_Dtlsaleamount     FLOAT,
                                v_Txndate           DATE,
                                v_Txnheaderid       NVARCHAR2,
                                v_Dtlclasscode      NVARCHAR2,
                                v_My_Log_Id         NUMBER,
                                p_Processdate       TIMESTAMP) IS
          v_Cnt          NUMBER := 0;
          v_Pointtypeid  NUMBER;
          v_Pointeventid NUMBER;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt       DATE;
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
          v_Recordcount NUMBER := 0;
          --cursor1-


          CURSOR Process_Rule IS
              SELECT v_Ipcode            AS Vckey,
                      v_Detailitem_Rowkey AS Originalrowkey,
                      v_Txndate           AS Txndate,
                      v_Txntypeid         AS v_Txntypeid,
                      p_Processdate,
                      tdi.dtlquantity -- we need the number of items because we will award 1 bonus point for each item
               FROM
                 (SELECT     Dw.dtl_rowkey,
                             Dw.Dtlclasscode,
                             Dw.dtlclearanceitem,
                             dw.dtlquantity   ,
                             wk.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            INNER join  Lw_Txnheader_Wrk wk on wk.txnheaderid = dw.txnheaderid
                            WHERE  Dw.dtl_rowkey = v_Detailitem_rowkey
                             /* AND  dw.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                                   '332','1333','1332')*/
                                and dw.dtlclasscode = v_Dtlclasscode
                      UNION ALL
                      SELECT Dw2.dtl_rowkey,
                             Dw2.Dtlclasscode,
                             Dw2.dtlclearanceitem,
                             dw2.dtlquantity,
                             wk2.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            INNER join  Lw_Txnheader_Wrk2 wk2 on wk2.txnheaderid = dw2.txnheaderid
                             WHERE  Dw2.dtl_rowkey = v_Detailitem_rowkey
                                and dw2.dtlclasscode = v_Dtlclasscode
                          /* AND  dw2.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                      UNION ALL
                      SELECT Dw3.dtl_rowkey,
                             Dw3.Dtlclasscode,
                             Dw3.dtlclearanceitem,
                              dw3.dtlquantity,
                              wk3.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            INNER JOIN  Lw_Txnheader_Wrk3 wk3 on wk3.txnheaderid = dw3.txnheaderid
                            WHERE  Dw3.dtl_rowkey = v_Detailitem_rowkey
                            and dw3.dtlclasscode = v_Dtlclasscode
                          /* AND  dw3.dtlclasscode in ('3131','3388','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                      UNION ALL
                      SELECT Dw4.dtl_rowkey,
                             Dw4.Dtlclasscode,
                             Dw4.dtlclearanceitem,
                             dw4.dtlquantity,
                             wk4.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            INNER join  Lw_Txnheader_Wrk4 wk4 on wk4.txnheaderid = dw4.txnheaderid
                            WHERE  Dw4.dtl_rowkey = v_Detailitem_rowkey
                           and dw4.dtlclasscode = v_Dtlclasscode
                          /* AND  dw4.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                      UNION ALL
                      SELECT Dw5.dtl_rowkey,
                             Dw5.Dtlclasscode,
                             Dw5.dtlclearanceitem,
                             dw5.dtlquantity,
                             wk5.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            INNER join  Lw_Txnheader_Wrk5 wk5 on wk5.txnheaderid = dw5.txnheaderid
                            WHERE  Dw5.dtl_rowkey = v_Detailitem_rowkey
                              and dw5.dtlclasscode = v_Dtlclasscode
                         /* AND  dw5.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/
                      UNION ALL
                      SELECT Dw6.dtl_rowkey,
                             Dw6.Dtlclasscode,
                             Dw6.dtlclearanceitem,
                             dw6.dtlquantity,
                             wk6.txnoriginaltxnrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            INNER join  Lw_Txnheader_Wrk6 wk6 on wk6.txnheaderid = dw6.txnheaderid
                      WHERE  Dw6.dtl_rowkey = v_Detailitem_rowkey
                             and dw6.dtlclasscode = v_Dtlclasscode
                           /*  AND  dw6.dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332')*/) tdi
                                             inner join bp_ae.ats_txnheader th on th.a_rowkey = tdi.txnoriginaltxnrowkey

               WHERE  1 = 1
                      AND v_Txntypeid = 2
                      AND nvl(tdi.dtlclearanceitem,0) = 0 --Exclude clearance
                      and th.a_txndate >= to_date ('5/3/2018 00:00:00','mm/dd/yyyy HH24:mi:ss')  -- original txn
                      and th.a_txndate <=  to_date ('6/15/2018 23:59:59','mm/dd/yyyy HH24:mi:ss') -- original txn --AEO-2513
                   --   and th.a_txnqualpurchaseamt > 0 -- original txn
                      AND v_Dtlclasscode in ('3131','338','333','1331','1335','5332','6332','3332',
                                             '332','1333','1332');



          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN

          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'Jean Points';


          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Summer 2018: B5G1 Jean Shorts Promo';

          SELECT To_Date(To_Char(VALUE), 'mm/dd/yyyy')
          INTO   v_Txnstartdt
          FROM   Lw_Clientconfiguration Cc
          WHERE  Cc.Key = 'PilotStartDate';
          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT
                    INTO v_Tbl LIMIT 10000;
               FORALL k IN 1 .. v_Tbl.Count SAVE EXCEPTIONS
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid /*Pointtypeid*/,
                          v_Pointeventid /*pointeventid*/,
                          v_Tbl(k).v_Txntypeid,
                          v_Tbl(k).Txndate,
                          --SYSDATE,
                          v_Tbl(k).p_Processdate,
                          -1 * nvl(v_tbl(k).dtlquantity,1) /*Jean Purchase Point*/,
                          To_Date('12/31/2199', 'mm/dd/yyyy'), -- Expiration Date
                          'B5G1 Jean Shorts Eligible for B5G1 Jean Credit (5/3/2018 - 6/15/2018)' /*Notes*/, --AEO-2513
                          1,
                          104,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL);
               COMMIT;
               EXIT WHEN Process_Rule%NOTFOUND;
          END LOOP;
          COMMIT;
          IF Process_Rule%ISOPEN
          THEN
               CLOSE Process_Rule;
          END IF;
          COMMIT;
          --END;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure B5G1_Shorts_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'B5G1_Shorts_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'B5G1_Shorts_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in B5G1_Shorts_Return ');
     END B5g1_Shorts_Return;
-- AEO-2364 end

-- AEO-2484 begin MM

     PROCEDURE DblePoints_Everything_052018(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Txnstartdt           DATE;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(Di.Dtlsaleamount) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      CASE
                        WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                             SUM(Di.Dtlsaleamount) > 0 THEN
                         1
                        ELSE
                         0
                      END AS v_onhold --Pointsonhold flag
               FROM   (SELECT Dw.Txnheaderid,
                                   Dw.Dtlclasscode,
                                   Dw.Dtlsaleamount,
                                   Dw.Ipcode,
                                   Dw.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk Dw
                            WHERE  Dw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw2.Txnheaderid,
                                   Dw2.Dtlclasscode,
                                   Dw2.Dtlsaleamount,
                                   Dw2.Ipcode,
                                   Dw2.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk2 Dw2
                            WHERE  Dw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw3.Txnheaderid,
                                   Dw3.Dtlclasscode,
                                   Dw3.Dtlsaleamount,
                                   Dw3.Ipcode,
                                   Dw3.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk3 Dw3
                            WHERE  Dw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw4.Txnheaderid,
                                   Dw4.Dtlclasscode,
                                   Dw4.Dtlsaleamount,
                                   Dw4.Ipcode,
                                   Dw4.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk4 Dw4
                            WHERE  Dw4.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw5.Txnheaderid,
                                   Dw5.Dtlclasscode,
                                   Dw5.Dtlsaleamount,
                                   Dw5.Ipcode,
                                   Dw5.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk5 Dw5
                            WHERE  Dw5.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT Dw6.Txnheaderid,
                                   Dw6.Dtlclasscode,
                                   Dw6.Dtlsaleamount,
                                   Dw6.Ipcode,
                                   Dw6.Parentrowkey
                            FROM   Lw_Txndetailitem_Wrk6 Dw6
                            WHERE  Dw6.Txnheaderid = v_Txnheaderid) Di
               inner join bp_ae.ats_txnheader th on th.a_txnheaderid = di.txnheaderid
               WHERE  1 = 1
                       and th.a_storenumber not in (105, 120, 123, 188, 204, 2070,
                                                2083, 2086, 2128, 2130, 2131, 2173,
                                                2207, 2343, 2390, 2401, 277, 279,
                                                367, 385, 432, 459, 479, 486, 512,
                                                 514, 620, 623, 64, 760, 821, 852,
                                                 891, 98, 659 )
                      AND (v_Txntypeid = 1 )
                      AND
                      (
                      v_Txndate BETWEEN to_date('5/19/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('5/20/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
                      AND v_Txnqualpurchaseamt > 0
                      AND Di.Dtlclasscode NOT IN ('9911')   -- Remove giftcards
               GROUP  BY Di.Txnheaderid;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Double Points 5/19-5/20';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Amount
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Amount <> 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Double Points on Everything (except Gift Cards) 5/19-5/20' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                       INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'Double Points on Everything (except Gift Cards) 5/19-5/20', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Double Points on Everything (except Gift Cards) 5/19-5/20' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               Commit;
               end if;
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_Everything_052018: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_Everything_052018',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_Everything_052018',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_Everything_052018');
     END DblePoints_Everything_052018;

     PROCEDURE DblePoints_052018_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_AECC_Count          FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
               SELECT MAX(v_Vckey) AS Vckey,
                      MAX(v_Header_Rowkey) AS Originalrowkey,
                      MAX(v_Txndate) AS Txndate,
                      SUM(v_Txnqualpurchaseamt) AS Txnqualpurchaseamt,
                      MAX(p_Processdate) AS p_Processdate,
                      10 AS Pointmultiplier,
                      To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
                      th.a_rowkey AS ParentRowKey
               FROM   (SELECT hw.txnheaderid, hw.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk hw
                            WHERE  hw.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw2.txnheaderid, hw2.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk2 hw2
                            WHERE  hw2.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw3.txnheaderid, hw3.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk3 hw3
                            WHERE  hw3.Txnheaderid = v_Txnheaderid
                            UNION ALL
                            SELECT hw4.txnheaderid, hw4.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk4 hw4
                            WHERE  hw4.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw5.txnheaderid, hw5.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk5 hw5
                            WHERE  hw5.Txnheaderid =  v_Txnheaderid
                            UNION ALL
                            SELECT hw6.txnheaderid, hw6.txnoriginaltxnrowkey
                            FROM   bp_ae.lw_txnheader_wrk6 hw6
                            WHERE  hw6.Txnheaderid =  v_Txnheaderid) wh
               INNER JOIN bp_ae.ats_txnheader th   ON th.a_rowkey = wh.txnoriginaltxnrowkey -- get the header of the purchase
               WHERE  1 = 1
                      and th.a_storenumber not in (105, 120, 123, 188, 204, 2070,
                                                2083, 2086, 2128, 2130, 2131, 2173,
                                                2207, 2343, 2390, 2401, 277, 279,
                                                367, 385, 432, 459, 479, 486, 512,
                                                 514, 620, 623, 64, 760, 821, 852,
                                                 891, 98, 659 )
                      AND wh.txnheaderid = v_Txnheaderid
                      AND v_Txntypeid = 2
                      AND wh.txnoriginaltxnrowkey > 0    --If the return has the original purchase
                      AND v_Header_Rowkey <> th.a_rowkey --RowKey from original purchase is no the same that the return
                      AND th.a_txnqualpurchaseamt > 0    -- for giftcard a_txnqualpusrchaseamt = 0
                      AND
                      ( --Original Purchase on Tuesday and in the range of dates that the bonus was active
                      th.a_txndate BETWEEN to_date('5/19/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS')
                      AND to_date('5/20/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS')
                      )
               GROUP  BY th.a_rowkey;
          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';

          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Double Points 5/19-5/20';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP
            --Check AE CC
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
            AND tt.a_tendertype in (75,78) ;

            IF v_AECC_Count > 0 THEN

              SELECT COUNT(*)
              INTO v_AECC_Count
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey =v_Header_Rowkey
              AND tt.a_tendertype in (75,78) ;

              IF v_AECC_Count > 0 THEN

                SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
                INTO v_AECC_Amount FROM (
                  SELECT tt.a_tendertype, tt.a_parentrowkey,
                   Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
                   FROM bp_ae.ats_txntender tt
                   WHERE tt.a_parentrowkey = v_Header_Rowkey /* v_Tbl(k).ParentRowKey} */) rt
                WHERE rt.a_tendertype in (75,78)
                GROUP BY rt.a_parentrowkey;
              else
                 v_AECC_Amount := 0;

              end if;


            END IF;

            --Double points on the purchase amount (base points is 10 points)
            --but if the member shops with their AE credit card,
            --then they get an additional 5 points per dollar
            v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(NVL(v_AECC_Amount,0), 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  'Double Points on Everything (except Gift Cards) 5/19-5/20', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePoints_052018_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePoints_052018_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePoints_052018_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePoints_052018_Return');
     END DblePoints_052018_Return;

-- AEO-2484 end   MM
-- AEO-2495 BEGIN GD
     PROCEDURE DblePnts_Dorm_062018(v_Header_Rowkey      NUMBER,
                                    v_Vckey              NUMBER,
                                    v_Txntypeid          NUMBER,
                                    v_Brandid            NUMBER,
                                    v_Txndate            DATE,
                                    v_Txnqualpurchaseamt FLOAT,
                                    v_Txnemployeeid      NVARCHAR2,
                                    v_Txnheaderid        NVARCHAR2,
                                    v_My_Log_Id          NUMBER,
                                    p_Processdate        TIMESTAMP) IS
          v_Pointtypeid          NUMBER;
          v_Pointeventid         NUMBER;
          v_AECC_Amount          FLOAT := 0;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Count           NUMBER := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Vckey) || ' ' || 'RowKey: ' || TO_CHAR(v_Header_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          v_Pointtransactionid   Number :=0;

          CURSOR Process_Rule IS
            SELECT v_Vckey AS Vckey,
              v_Header_Rowkey AS Originalrowkey,
              v_Txndate AS Txndate,
              tmp.amount AS Txnqualpurchaseamt,
              p_Processdate AS p_Processdate,
              10 AS Pointmultiplier,
              To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
              CASE
                WHEN trunc(v_Txndate) >= trunc(SYSDATE - 14) AND
                    v_Txnqualpurchaseamt > 0 THEN
                 1
                ELSE
                 0
              END AS v_onhold --Pointsonhold flag
            FROM(
              SELECT NVL(SUM(tdi.a_dtlsaleamount),0) AS amount
              FROM bp_ae.ats_txndetailitem tdi
              INNER JOIN bp_ae.lw_product pt
              ON pt.id = tdi.a_dtlproductid
              WHERE  1 = 1
              AND tdi.a_txnheaderid = v_Txnheaderid
              AND tdi.a_ipcode = v_Vckey
              AND (v_Txntypeid = 1 OR v_Txntypeid = 4)
              AND pt.partnumber IN 
              (
                '26527754','26527762','26527846','28062917','28062925','28062933','28062941','28062958','28062974','28062982',
                '28062990','28063006','28063899','28063907','28063915','28063923','28063931','28063949','28063956','28063964',
                '28063972','28063980','28348423','28424331','28424349','28759413','28759421','28759439','28759447','28759454',
                '28759462','28759470','28759488','28759496','28759504','28759512','28759520','28759538','28759546','28759553',
                '28759561','28759579','28759587','28759595','28759603','28759611','28759629','28759637','28759645'
              )
              AND (v_Txndate BETWEEN to_date('6/15/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS') AND to_date('7/15/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS'))
            ) tmp
            WHERE tmp.amount <> 0
            AND v_Txnqualpurchaseamt > 0 --Purchase amount > 0
            ;

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Summer 2018: Dormify Double Points Promo';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP

          --Check AE CC
          SELECT COUNT(*)
          INTO v_AECC_Count
          FROM bp_ae.ats_txntender tt
          WHERE tt.a_parentrowkey = v_Header_Rowkey
          AND tt.a_tendertype in (75,78)
          ;

          IF v_AECC_Count <> 0 THEN
            SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
            INTO v_AECC_Amount FROM (
              SELECT tt.a_tendertype, tt.a_parentrowkey,
            Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
            WHERE rt.a_tendertype in (75,78)
            GROUP BY rt.a_parentrowkey;
          END IF;

          --Double points on the purchase amount (base points is 10 points)
          --but if the member shops with their AE credit card,
          --then they get an additional 5 points per dollar
          v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

         IF (v_Tbl(k).v_onhold = 1) then
                    INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Summer 2018: Dormify Double Points Promo' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          v_BonusPoints, /*Pointsonhold*/
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)  returning Pointtransactionid into v_Pointtransactionid;

                           INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          3 /*pointsonhold*/,
                          v_Tbl(k).Txndate ,
                          v_Tbl(k).p_Processdate ,
                          v_BonusPoints /*Points*/,
                          v_Tbl(k).Expirationdate ,--new expiration date
                          'Summer 2018: Dormify Double Points Promo', /*Notes*/
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          v_Pointtransactionid,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

               LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED;
                --logs errors into table so that the process would continue
                COMMIT;
               ELSIF (v_Tbl(k).v_onhold = 0) THEN
                   INSERT INTO Lw_Pointtransaction
                         (Pointtransactionid,
                          Vckey,
                          Pointtypeid,
                          Pointeventid,
                          Transactiontype,
                          Transactiondate,
                          Pointawarddate,
                          Points,
                          Expirationdate,
                          Notes,
                          Ownertype,
                          Ownerid,
                          Rowkey,
                          Parenttransactionid,
                          Pointsconsumed,
                          Pointsonhold,
                          Ptlocationid,
                          Ptchangedby,
                          Createdate,
                          Expirationreason)
                    VALUES
                         (Seq_Pointtransactionid.Nextval,
                          v_Tbl(k).Vckey,
                          v_Pointtypeid,
                          v_Pointeventid,
                          1 /*purchase*/,
                          v_Tbl(k).Txndate,
                          v_Tbl(k).p_Processdate,
                          v_BonusPoints, /*Points*/
                          v_Tbl(k).Expirationdate,
                          'Summer 2018: Dormify Double Points Promo' /*Notes*/,
                          1,
                          101,
                          v_Tbl(k).Originalrowkey,
                          -1,
                          0 /*Pointsconsumed*/,
                          0 /*Pointsonhold*/,
                          NULL,
                          'RulesProcessing' /*Ptchangedby*/,
                          SYSDATE,
                          NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
               COMMIT;
               END IF;
           COMMIT;

           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_Dorm_062018: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_Dorm_062018',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_Dorm_062018',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_Dorm_062018');
     END DblePnts_Dorm_062018;

     PROCEDURE DblePnts_Dorm_062018_Return(v_Header_Rowkey      NUMBER,
                                 v_Vckey              NUMBER,
                                 v_Txntypeid          NUMBER,
                                 v_Brandid            NUMBER,
                                 v_Txndate            DATE,
                                 v_Txnqualpurchaseamt FLOAT,
                                 v_Txnemployeeid      NVARCHAR2,
                                 v_Txnheaderid        NVARCHAR2,
                                 v_My_Log_Id          NUMBER,
                                 p_Processdate        TIMESTAMP) IS
          v_Cnt                  NUMBER := 0;
          v_Pointtypeid          NUMBER;
          v_Pointtypeid_Redesign NUMBER;
          v_Pointeventid         NUMBER;
          v_BonusPoints          FLOAT := 0;
          v_AECC_Amount          FLOAT := 0;
          v_AECC_Count           NUMBER := 0;
          v_AECC_Count_Parent    NUMBER := 0;
          v_BonusPointsPurchase  FLOAT := 0;
          Dml_Errors EXCEPTION;
          PRAGMA EXCEPTION_INIT(Dml_Errors, -24381);
          v_Txnstartdt DATE;
          --log job attributes
          --v_my_log_id             NUMBER;
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
          v_Message     VARCHAR2(256) := 'VCKEY: ' || TO_CHAR(v_Vckey) || ' ' || 'RowKey: ' || TO_CHAR(v_Header_Rowkey);
          v_Reason      VARCHAR2(256);
          v_Error       VARCHAR2(256);
          v_Trycount    NUMBER := 0;
          v_Recordcount NUMBER := 0;
          CURSOR Process_Rule IS
          SELECT MAX(v_Vckey) AS Vckey,
            MAX(v_Header_Rowkey) AS Originalrowkey,
            MAX(v_Txndate) AS Txndate,
            SUM(tmp.amount) AS Txnqualpurchaseamt,
            MAX(p_Processdate) AS p_Processdate,
            10 AS Pointmultiplier,
            To_Date('12/31/2199', 'mm/dd/yyyy') AS Expirationdate,
            tmp.a_rowkey AS ParentRowKey
          FROM
          (
            SELECT tho.a_rowkey, NVL(SUM(td.a_dtlsaleamount),0) AS amount
            FROM bp_ae.ats_txnheader th
            INNER JOIN bp_ae.ats_txnheader tho
            ON tho.a_rowkey = th.a_txnoriginaltxnrowkey
            INNER JOIN bp_ae.ats_txndetailitem td
            ON td.a_txnheaderid = th.a_txnheaderid
            INNER JOIN bp_ae.lw_product pt
            ON pt.id = td.a_dtlproductid
            WHERE  1 = 1
            AND td.a_txnheaderid = v_Txnheaderid
            AND td.a_ipcode = v_Vckey
            AND pt.partnumber IN
            (
              '26527754','26527762','26527846','28062917','28062925','28062933','28062941','28062958','28062974','28062982',
              '28062990','28063006','28063899','28063907','28063915','28063923','28063931','28063949','28063956','28063964',
              '28063972','28063980','28348423','28424331','28424349','28759413','28759421','28759439','28759447','28759454',
              '28759462','28759470','28759488','28759496','28759504','28759512','28759520','28759538','28759546','28759553',
              '28759561','28759579','28759587','28759595','28759603','28759611','28759629','28759637','28759645'
            )
            AND (tho.a_txndate BETWEEN to_date('6/15/2018 00:00:00', 'mm/dd/yyyy HH24:MI:SS') AND to_date('7/15/2018 23:59:59', 'mm/dd/yyyy HH24:MI:SS'))
            AND v_Txntypeid = 2
            AND th.a_txnoriginaltxnrowkey > 0 --If the return has the original purchase
            AND v_Header_Rowkey <> tho.a_RowKey --RowKey from original purchase is no the same that the return
            GROUP BY tho.a_rowkey
          ) tmp
          WHERE v_Txnqualpurchaseamt <> 0 --Purchase amount <> 0
          GROUP BY tmp.a_rowkey
          ;

          TYPE t_Tab IS TABLE OF Process_Rule%ROWTYPE;
          v_Tbl t_Tab;
     BEGIN
          SELECT Pointtypeid
          INTO   v_Pointtypeid
          FROM   Lw_Pointtype
          WHERE  NAME = 'AEO Connected Bonus Points';
          SELECT Pointeventid
          INTO   v_Pointeventid
          FROM   Lw_Pointevent
          WHERE  NAME = 'Summer 2018: Dormify Double Points Promo';

          OPEN Process_Rule;
          LOOP
               FETCH Process_Rule BULK COLLECT INTO v_Tbl LIMIT 10000;
               FOR k IN 1 .. v_tbl.count
          LOOP

          --Check AE CC -- Original Purchase
          SELECT COUNT(*)
          INTO v_AECC_Count_Parent
          FROM bp_ae.ats_txntender tt
          WHERE tt.a_parentrowkey = v_Tbl(k).ParentRowKey
          AND tt.a_tendertype in (75,78)
          ;

          --Deduct AECC if the original purchase issue AECC points
          IF v_AECC_Count_Parent <> 0 THEN
            --Check AE CC -- Return
            SELECT COUNT(*)
            INTO v_AECC_Count
            FROM bp_ae.ats_txntender tt
            WHERE tt.a_parentrowkey = v_Header_Rowkey
            AND tt.a_tendertype in (75,78)
            ;

            IF v_AECC_Count <> 0 THEN
              SELECT sum(Round(v_Txnqualpurchaseamt*rt.Ratio)) as Amount
              INTO v_AECC_Amount FROM (
                SELECT tt.a_tendertype, tt.a_parentrowkey,
              Ratio_To_Report(Tt.a_Tenderamount) Over(PARTITION BY tt.a_parentrowkey) AS Ratio
              FROM bp_ae.ats_txntender tt
              WHERE tt.a_parentrowkey = v_Header_Rowkey) rt
              WHERE rt.a_tendertype in (75,78)
              GROUP BY rt.a_parentrowkey;
            END IF;
          ELSE
            v_AECC_Amount := 0;
          END IF;
          
          --Double points on the purchase amount (base points is 10 points)
          --but if the member shops with their AE credit card,
          --then they get an additional 5 points per dollar
          v_BonusPoints := (Round(v_Tbl(k).Txnqualpurchaseamt, 0) * v_Tbl(k).Pointmultiplier) + (Round(v_AECC_Amount, 0) * 5);

            INSERT INTO Lw_Pointtransaction
                 (Pointtransactionid,
                  Vckey,
                  Pointtypeid,
                  Pointeventid,
                  Transactiontype,
                  Transactiondate,
                  Pointawarddate,
                  Points,
                  Expirationdate,
                  Notes,
                  Ownertype,
                  Ownerid,
                  Rowkey,
                  Parenttransactionid,
                  Pointsconsumed,
                  Pointsonhold,
                  Ptlocationid,
                  Ptchangedby,
                  Createdate,
                  Expirationreason)
            VALUES
                 (Seq_Pointtransactionid.Nextval,
                  v_Tbl(k).Vckey,
                  v_Pointtypeid /*Pointtypeid*/,
                  v_Pointeventid /*pointeventid*/,
                  2,
                  v_Tbl(k).Txndate, --txndate
                  v_Tbl(k).p_Processdate,
                  v_BonusPoints, /*Points*/
                  v_Tbl(k).Expirationdate, --expiration date
                  'Summer 2018: Dormify Double Points Promo Return', /*Notes*/
                  1,
                  101,
                  v_Tbl(k).Originalrowkey,
                  -1,
                  0 /*Pointsconsumed*/,
                  0 /*Pointsonhold*/,
                  NULL,
                  'RulesProcessing' /*Ptchangedby*/,
                  SYSDATE,
                  NULL)

             LOG ERRORS INTO err$_Lw_Pointtransaction('INSERT') REJECT LIMIT UNLIMITED; --logs errors into table so that the process would continue
           COMMIT;
           END LOOP;
        EXIT WHEN Process_Rule%NOTFOUND; --<----- always do this last in the loop, exit on at the end of cursor.
      END LOOP;
      COMMIT;
      IF Process_Rule%ISOPEN THEN
        --<--- dont forget to close cursor since we manually opened it.
        CLOSE Process_Rule;
      END IF;
     EXCEPTION
          WHEN Dml_Errors THEN
               FOR Indx IN 1 .. SQL%Bulk_Exceptions.Count
               LOOP
                    v_Messagesfailed := v_Messagesfailed + 1;
                    v_Error          := SQLERRM(-sql%BULK_EXCEPTIONS(Indx)
                                                .Error_Code);
                    v_Reason         := 'Failed Procedure DblePnts_Dorm_062018_Return: ';
                    v_Message        := 'VCKEY: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Vckey || ' ' || 'RowKey: ' || v_Tbl(SQL%BULK_EXCEPTIONS(Indx).Error_Index)
                                       .Originalrowkey;
                    Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                        p_Envkey    => v_Envkey,
                                        p_Logsource => 'DblePnts_Dorm_062018_Return',
                                        p_Filename  => NULL,
                                        p_Batchid   => v_Batchid,
                                        p_Jobnumber => v_My_Log_Id,
                                        p_Message   => v_Message,
                                        p_Reason    => v_Reason,
                                        p_Error     => v_Error,
                                        p_Trycount  => v_Trycount,
                                        p_Msgtime   => SYSDATE);
               END LOOP;
          WHEN OTHERS THEN
               v_Error := SQLERRM;
               Utility_Pkg.Log_Msg(p_Messageid => v_Messageid,
                                   p_Envkey    => v_Envkey,
                                   p_Logsource => 'DblePnts_Dorm_062018_Return',
                                   p_Filename  => NULL,
                                   p_Batchid   => v_Batchid,
                                   p_Jobnumber => v_My_Log_Id,
                                   p_Message   => v_Message,
                                   p_Reason    => v_Reason,
                                   p_Error     => v_Error,
                                   p_Trycount  => v_Trycount,
                                   p_Msgtime   => SYSDATE);
               Raise_Application_Error(-20002,
                                       'Other Exception detected in DblePnts_Dorm_062018_Return');
     END DblePnts_Dorm_062018_Return;
-- AEO-2495 END GD
END Rulesprocessing;
/
