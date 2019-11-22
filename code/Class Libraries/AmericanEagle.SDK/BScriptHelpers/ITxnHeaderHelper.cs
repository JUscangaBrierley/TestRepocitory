using System.Collections.Generic;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork;

namespace AmericanEagle.SDK.BScriptHelpers
{
    public interface ITxnHeaderHelper
    {
        IList<IClientDataObject> GetAllReturnedDetailItemsLinkedToTxnHeader(Member member, TxnHeader txnHeader, string currentTxnHeaderId = null);
        IList<IClientDataObject> GetDetailItemsByTxnHeader(TxnHeader txnHeader);
        IList<TxnFourPartKey> GetOriginalTxnHeaderInfoForReturnedItems(IList<IClientDataObject> atsList);
        TxnHeader GetTxnHeader(TxnFourPartKey txnKey);
        decimal GetTxnPointsOnOriginal(Member member, TxnHeader txnHeader, PointType pointType, PointEvent pointEvent);
        IList<IClientDataObject> GetChildAttributesFromContextObject(ContextObject contextObject, string attributeSetName);
    }
}