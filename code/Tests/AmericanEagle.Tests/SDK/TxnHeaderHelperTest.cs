using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using AmericanEagle.SDK.BScriptHelpers;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.ClientDevUtilities.TestingUtilities.Expressions;
using Brierley.ClientDevUtilities.TestingUtilities.Reflection;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;

namespace AmericanEagle.Tests.SDK
{
    public class TxnHeaderHelperTest
    {
        private Mock<ILWDataServiceUtil> _dataUtilMock;
        private const string mockExceptionMessage = "Mock Exception Error";

        [SetUp]
        public void SetUp()
        {
            //reset this mock letting each test start fresh
            _dataUtilMock = new Mock<ILWDataServiceUtil>();
        }

        [Test]
        public void GetOriginalTxnHeaderInfoForReturnedItems_NullParameter()
        {
            IList<IClientDataObject> parameters = null;
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);            
            Assert.IsEmpty(txnHHelper.GetOriginalTxnHeaderInfoForReturnedItems(parameters));
        }
        [Test]
        public void GetOriginalTxnHeaderInfoForReturnedItems_NotTxnDetailItemsOnList()
        {
            IList<IClientDataObject> parameters =new List<IClientDataObject>();
            parameters.Add(new TxnHeader() { OrderNumber = "100001", RowKey = 101, TxnDate = new DateTime(2019, 2, 23), TxnStoreId = 1095, TxnNumber = "0" });
            parameters.Add(new TxnHeader() { OrderNumber = "0", RowKey = 102, TxnDate = new DateTime(2019, 2, 23), TxnStoreId = 1095, TxnNumber = "100002" });
            parameters.Add(new TxnHeader() { OrderNumber = "100003", RowKey = 103, TxnDate = new DateTime(2019, 2, 22), TxnStoreId = 1095, TxnNumber = "0" });

            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);
            Assert.IsEmpty(txnHHelper.GetOriginalTxnHeaderInfoForReturnedItems(parameters));
        }
        [Test]
        public void GetOriginalTxnHeaderInfoForReturnedItems_ValidCase()
        {
            IList<IClientDataObject> parameters = new List<IClientDataObject>();
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = "100001", TxnOriginalTxnNumber = "0", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId=1095,  TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId=2 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = null, TxnOriginalTxnNumber = null, TxnOriginalTxnDate = null, TxnOriginalStoreId = null, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 1 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = null, TxnOriginalTxnNumber = null, TxnOriginalTxnDate = null, TxnOriginalStoreId = null, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 1 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = null, TxnOriginalTxnNumber = null, TxnOriginalTxnDate = null, TxnOriginalStoreId = null, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 1 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100001", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100001", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100002", TxnOriginalTxnDate = new DateTime(2019, 2, 22), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = null, TxnOriginalTxnNumber = null, TxnOriginalTxnDate = null, TxnOriginalStoreId = null, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 1 });
            parameters.Add(new TxnDetailItem() { TxnOriginalOrderNumber = null, TxnOriginalTxnNumber = null, TxnOriginalTxnDate = null, TxnOriginalStoreId = null, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 1 });

            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            Assert.IsNotEmpty(txnHHelper.GetOriginalTxnHeaderInfoForReturnedItems(parameters));
            Assert.AreEqual(3, txnHHelper.GetOriginalTxnHeaderInfoForReturnedItems(parameters).Count);
        }

        [Test]
        public void GetTxnHeader_ParameterWithMissingExpectedValue() {
            this.MockDataUtil_GetAttributeSetObjects_OriginalTxnHeader();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);
            //All expected properties as null
            TxnFourPartKey txnHeaderKey = new TxnFourPartKey();            
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey));
            //One Property Missing
            TxnFourPartKey txnHeaderKey_MissingOrderNumber = new TxnFourPartKey() {
                TxnNumber ="0",
                TxnStoreId =1095,
                TxnDate = new DateTime(2019, 2, 23, 10, 10, 10)
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_MissingOrderNumber));
            TxnFourPartKey txnHeaderKey_WhiteOrderNumber = new TxnFourPartKey()
            {
                OrderNumber = "",
                TxnNumber = "0",
                TxnStoreId = 1095,
                TxnDate = new DateTime(2019, 2, 23, 10, 10, 10)
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_WhiteOrderNumber));
            TxnFourPartKey txnHeaderKey_MissingTxnNumber = new TxnFourPartKey()
            {
                OrderNumber = "100001",
                TxnStoreId = 1095,
                TxnDate = new DateTime(2019, 2, 23, 10, 10, 10)
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_MissingTxnNumber));
            TxnFourPartKey txnHeaderKey_WhiteTxnNumber = new TxnFourPartKey()
            {
                OrderNumber = "100001",
                TxnNumber = "",
                TxnStoreId = 1095,
                TxnDate = new DateTime(2019, 2, 23, 10, 10, 10)
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_WhiteTxnNumber));
            TxnFourPartKey txnHeaderKey_MissingTxnStoreId = new TxnFourPartKey()
            {
                OrderNumber = "100001",
                TxnNumber = "0",
                TxnDate = new DateTime(2019, 2, 23, 10, 10, 10)
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_MissingTxnStoreId));
            TxnFourPartKey txnHeaderKey_MissingTxnDate = new TxnFourPartKey()
            {
                OrderNumber = "100001",
                TxnNumber = "0",
                TxnStoreId = 1095
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_MissingTxnDate));

        }
        [Test]
        public void GetTxnHeader_ValidCase()
        {
            this.MockDataUtil_GetAttributeSetObjects_OriginalTxnHeader();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            TxnFourPartKey txnHeaderKey_WhiteValues = new TxnFourPartKey()
            {
                OrderNumber = "100001",
                TxnNumber = "0",
                TxnStoreId = 1095,
                TxnDate = new DateTime(2019, 2, 23)
            };
            Assert.AreEqual(typeof(TxnHeader), txnHHelper.GetTxnHeader(txnHeaderKey_WhiteValues).GetType());
        }
        [Test]
        public void GetTxnHeader_ValidCase_txnHeaderNotFound()
        {
            this.MockDataUtil_GetAttributeSetObjects_OriginalTxnHeader_NotFound();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            TxnFourPartKey txnHeaderKey_WhiteValues = new TxnFourPartKey()
            {
                OrderNumber = "100001",
                TxnNumber = "0",
                TxnStoreId = 1095,
                TxnDate = new DateTime(2019, 2, 23)
            };
            Assert.AreEqual(null, txnHHelper.GetTxnHeader(txnHeaderKey_WhiteValues));
        }
        [Test]
        public void GetTxnPointsOnOriginal_NullParameter()
        {
            this.MockDataUtil_GetTxnPointsOnOriginal();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            Member mockMember = new Member()
            {
                LoyaltyCards = new List<VirtualCard>() {
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10002, IpCode=100000000001, IsPrimary=true },
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10001, IpCode=100000000000, IsPrimary=false }
                }
            };
            TxnHeader mockTxnHeader = new TxnHeader() { RowKey = 1100001, TxnHeaderId = "MOCKtxnHeader0001" };
            PointType mockPointType = new PointType() { ID=1 };
            PointEvent mockPointEvent = new PointEvent() { ID=100 };

            Assert.AreEqual(0, txnHHelper.GetTxnPointsOnOriginal(null,mockTxnHeader,mockPointType,mockPointEvent));
            Assert.AreEqual(0, txnHHelper.GetTxnPointsOnOriginal(mockMember, null, mockPointType, mockPointEvent));
            Assert.AreEqual(0, txnHHelper.GetTxnPointsOnOriginal(mockMember, mockTxnHeader, null, mockPointEvent));
            Assert.AreEqual(0, txnHHelper.GetTxnPointsOnOriginal(mockMember, mockTxnHeader, mockPointType, null));
            Assert.AreEqual(0, txnHHelper.GetTxnPointsOnOriginal(null, null, null, null));

        }
        [Test]
        public void GetTxnPointsOnOriginal_ValidCase()
        {
            this.MockDataUtil_GetTxnPointsOnOriginal();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            Member mockMember = new Member()
            {
                LoyaltyCards = new List<VirtualCard>() {
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10002, IpCode=100000000001, IsPrimary=true },
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10001, IpCode=100000000000, IsPrimary=false }
                }
            };
            TxnHeader mockTxnHeader = new TxnHeader() { RowKey = 1100001, TxnHeaderId = "MOCKtxnHeader0001" };
            PointType mockPointType = new PointType() { ID = 1 };
            PointEvent mockPointEvent = new PointEvent() { ID = 100 };

            Assert.AreEqual((decimal)1250, txnHHelper.GetTxnPointsOnOriginal(mockMember, mockTxnHeader, mockPointType, mockPointEvent));
        }
        [Test]
        public void GetDetailItemsByTxnHeader_NullParameter()
        {
            this.MockDataUtil_GetAttributeSetObjects();
            this.MockDataUtil_GetAttributeSetMetaData_TxnDetailItem();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            Assert.AreNotEqual(null, txnHHelper.GetDetailItemsByTxnHeader(null));
            Assert.IsEmpty(txnHHelper.GetDetailItemsByTxnHeader(null));

        }
        [Test]
        public void GetDetailItemsByTxnHeader_ValidCase()
        {
            this.MockDataUtil_GetAttributeSetObjects();
            this.MockDataUtil_GetAttributeSetMetaData_TxnDetailItem();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            TxnHeader mockTxnHeader = new TxnHeader() { RowKey = 1100001, TxnHeaderId = "MOCKtxnHeader0001" };

            Assert.IsNotEmpty(txnHHelper.GetDetailItemsByTxnHeader(mockTxnHeader));
            Assert.IsNotNull(txnHHelper.GetDetailItemsByTxnHeader(mockTxnHeader));

        }
        [Test]
        public void GetAllReturnedDetailItemsLinkedToTxnHeader_NullParameters()
        {
            this.MockDataUtil_GetAttributeSetObjects_TxnDetailItems();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            Member mockMember = new Member()
            {
                LoyaltyCards = new List<VirtualCard>() {
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10002, IpCode=100000000001, IsPrimary=true },
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10001, IpCode=100000000000, IsPrimary=false }
                }
            };
            TxnHeader mockTxnHeader = new TxnHeader() { RowKey = 1100001, TxnHeaderId = "MOCKtxnHeader0001" };

            Assert.IsNull(txnHHelper.GetAllReturnedDetailItemsLinkedToTxnHeader(null,null));
            Assert.IsNull(txnHHelper.GetAllReturnedDetailItemsLinkedToTxnHeader(mockMember, null));
            Assert.IsNull(txnHHelper.GetAllReturnedDetailItemsLinkedToTxnHeader(null, mockTxnHeader));

        }
        [Test]
        public void GetAllReturnedDetailItemsLinkedToTxnHeader_ValidCase()
        {
            this.MockDataUtil_GetAttributeSetObjects_TxnDetailItems();
            TxnHeaderHelper txnHHelper = new TxnHeaderHelper(_dataUtilMock.Object);

            Member mockMember = new Member()
            {
                LoyaltyCards = new List<VirtualCard>() {
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10002, IpCode=100000000001, IsPrimary=true },
                    new VirtualCard(){ LoyaltyIdNumber="MOCK10001", VcKey=10001, IpCode=100000000000, IsPrimary=false }
                }
            };
            TxnHeader mockTxnHeader = new TxnHeader() { RowKey = 1100001, TxnHeaderId = "MOCKtxnHeader0001" };

            Assert.IsNotNull(txnHHelper.GetAllReturnedDetailItemsLinkedToTxnHeader(mockMember, mockTxnHeader));
            Assert.IsNotEmpty(txnHHelper.GetAllReturnedDetailItemsLinkedToTxnHeader(mockMember, mockTxnHeader));           

        }


        private void MockDataUtil_GetAttributeSetObjects_OriginalTxnHeader()
        {
            _dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAttributeSetObjects(null, "TxnHeader",It.IsAny<LWCriterion>(), It.IsAny<LWQueryBatchInfo>(),false,false)
                ).Returns(
                        new List<IClientDataObject>() {
                            new TxnHeader() { OrderNumber = "100001", RowKey = 101, TxnDate = new DateTime(2019, 2, 23, 10, 10, 10), TxnStoreId = 1095, TxnNumber = "0" },
                            new TxnHeader() { OrderNumber = "100001", RowKey = 101, TxnDate = new DateTime(2019, 2, 23, 11, 11, 11), TxnStoreId = 1095, TxnNumber = "0" }
                        }                        
                );
        }
        private void MockDataUtil_GetAttributeSetObjects_OriginalTxnHeader_NotFound()
        {
            _dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAttributeSetObjects(null, "TxnHeader", It.IsAny<LWCriterion>(), It.IsAny<LWQueryBatchInfo>(), false, false)
                ).Returns(
                        new List<IClientDataObject>()
                );
        }

        private void MockDataUtil_GetAttributeSetObjects_TxnDetailItems()
        {
            _dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAttributeSetObjects(It.IsAny<VirtualCard[]>(), It.IsAny<AttributeSetMetaData>(), It.IsAny<LWCriterion>(), It.IsAny<LWQueryBatchInfo>(), false, false)
                ).Returns(
                        new List<IClientDataObject>()
                        {
                            new TxnDetailItem() { TxnOriginalOrderNumber = "100002", TxnOriginalTxnNumber = "0", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId=1095,  TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId=2 },
                            new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100000", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 }
                        }
                );
        }

        private void MockDataUtil_GetTxnPointsOnOriginal()
        {
            _dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAttributeSetMetaData("TxnHeader")
                ).Returns(
                        new AttributeSetMetaData {
                            Name="TxnHeader",
                            Type = Brierley.FrameWork.Common.AttributeSetType.VirtualCard,
                            ID=1001,
                            ParentID =1000                            
                        }
                );

            _dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetPointBalance(
                        It.IsAny<long[]>(), 
                        It.IsAny<long[]>(), 
                        It.IsAny<long[]>(), 
                        It.IsAny<PointBankTransactionType[]>(),
                        null, null, null, null, null, null,
                        PointTransactionOwnerType.AttributeSet,
                        1001,
                        It.IsAny<long[]>(),
                        false)                        
                ).Returns(1250);
        }
        private void MockDataUtil_GetAttributeSetMetaData_TxnDetailItem()
        {
            _dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAttributeSetMetaData("TxnDetailItem")
                ).Returns(
                        new AttributeSetMetaData
                        {
                            Name = "TxnDetailItem",
                            Type = Brierley.FrameWork.Common.AttributeSetType.VirtualCard,
                            ID = 1002,
                            ParentID = 1001
                        }
                );
        }
        private void MockDataUtil_GetAttributeSetObjects()
        {
            _dataUtilMock.Setup(
                   x => x.LoyaltyDataServiceInstance().GetAttributeSetObjects((IAttributeSetContainer)null, It.IsAny<AttributeSetMetaData>(), String.Empty, It.IsAny<string>(), String.Empty, (LWQueryBatchInfo)null, false, false)
                ).Returns(
                        new List<IClientDataObject>()
                        {
                            new TxnDetailItem() { TxnOriginalOrderNumber = "100001", TxnOriginalTxnNumber = "0", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId=1095,  TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId=2 },                            
                            new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100001", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 },
                            new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100001", TxnOriginalTxnDate = new DateTime(2019, 2, 23), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 },
                            new TxnDetailItem() { TxnOriginalOrderNumber = "0", TxnOriginalTxnNumber = "100002", TxnOriginalTxnDate = new DateTime(2019, 2, 22), TxnOriginalStoreId = 1095, TxnStoreId = 1095, TxnDate = DateTime.Now, DtlTypeId = 2 }
                        }
                );
        }
    }
}
