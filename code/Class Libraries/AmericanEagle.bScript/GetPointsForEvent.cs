using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Portal;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using AmericanEagle.SDK.Global;


namespace AmericanEagle.bScript
{
    [ExpressionContext(Description = "This function returns points for an given Point event and type",
       DisplayName = "GetPointForEvent",
       ExcludeContext = ExpressionContexts.Email)]
    public class GetPointsForEvent : UnaryOperation
    {
        #region Member Variables
        /// <summary>
        /// Hold argument from outside  
        /// </summary>
        String _providedArg = String.Empty;
        /// <summary>
        /// Hold context member
        /// </summary>
        Member _member = null;
        /// <summary>
        /// For service data APIs
        /// </summary>
        private static LWLogger _logger = null;
        private const string _appName = "bScript";
        private StateValidation stateValidate = null;

        #endregion member variables

        #region Public Methods
        /// <summary>
        /// Constructor for bScript class
        /// </summary>
        /// <param name="arg"></param>
        public GetPointsForEvent()
        {

        }
        public GetPointsForEvent(Expression arg)
            : base("GetPointForEvent", arg)
        {
            // Initialize logging
            _logger = LWLoggerManager.GetLogger(_appName);

            ContextObject cObj = new ContextObject();
            stateValidate = new StateValidation();

            try
            {
                _providedArg = arg.evaluate(cObj).ToString();
            }
            catch (Exception ex)
            {
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to GetMemberAccountDetails.", ex);
            }
        }

        /// <summary>
        /// Syntax to use GetPointForEvent
        /// </summary>
        //       public override string Syntax
        //      {
        //          get
        //          {
        //              return "GetPointForEvent('[GetPointForEvent]')";
        //          }
        //      }

        public override object evaluate(ContextObject contextObject)
        {
            _logger.Trace("Custom bScript", "evaluate1", "Begin");

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            String retValue = String.Empty;

            try
            {
                _member = ((VirtualCard)contextObject.Owner).Member;
                // _logger.Trace("Custom bScript", "_member", _member.IpCode.ToString());
                if (_member == null)
                {
                    return retValue;
                }

                _logger.Trace("Custom bScript", "evaluate", "Determine Call: " + _providedArg.ToUpper());

                string[] inputparm = _providedArg.Split(new char[] { '|' });
                string pointevent = inputparm[0];
                string pointtype = inputparm[1];
                DateTime PstartDate = Convert.ToDateTime(inputparm[2]);
                DateTime PendDate = Convert.ToDateTime(inputparm[3]);
                retValue = GETPOINTSFOREVENT(_member, pointevent, pointtype, PstartDate, PendDate);
                //     }
            }
            catch (Exception ex)
            {
                _logger.Error("Custom bScript", "evaluate", ex.Message);
            }

            return retValue;
        }
        #endregion public methods

        /// <summary>
        //this method takes in a pointevent and returns the points associated to it
        /// </summary>
        /// <param name="pPointEvent">pPointEvent</param>
        /// <returns>points</returns>
        private static string GETPOINTSFOREVENT(Member pMember, string pPointEvent, string pPointType, DateTime pstartdt, DateTime pEnddt)
        {
            _logger.Trace("Custom bScript", "GETPOINTSFOREVENT", "begin");
            PointType pointType = null;
            PointEvent pointEvent = null;
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //Double dblBonusPoints = 0;
            Decimal  dblBonusPoints = 0;
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            DateTime endDate = pEnddt;
            DateTime startdate = pstartdt;
            int index = 0;
            long[] vcKeys = new long[pMember.LoyaltyCards.Count];
            long[] pointTypeIDs = new long[1];
            long[] pointEventIDs = new long[1];

            string _pointType = pPointType;
            string _pointEvent = pPointEvent;

            try
            {
                if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
                {
                    foreach (VirtualCard card in pMember.LoyaltyCards)
                    {
                        vcKeys[index] = card.VcKey;
                        ++index;
                    }

                    pointTypeIDs[0] = pointType.ID;
                    pointEventIDs[0] = pointEvent.ID;
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //dblBonusPoints = pMember.GetPoints(pointType, pointEvent, startdate, endDate);
                    dblBonusPoints = pMember.GetPoints(pointTypeIDs,pointEventIDs,startdate,endDate);
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    _logger.Trace("Custom bScript", "GETPOINTSFOREVENT", Convert.ToString(dblBonusPoints));
                }
            }
            catch (Exception ex)
            {
                _logger.Trace("Custom bScript", "GETPOINTSFOREVENT", ex.Message);

            }
            return Convert.ToString(dblBonusPoints);
        }

    }
}


