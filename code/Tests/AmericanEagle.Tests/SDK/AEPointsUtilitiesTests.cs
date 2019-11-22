using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using AmericanEagle.SDK.Global;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.ClientDevUtilities.TestingUtilities.Expressions;
using Brierley.ClientDevUtilities.TestingUtilities.Reflection;
using Brierley.Clients.AmericanEagle.DataModel;

namespace AmericanEagle.Tests.SDK
{
    public class AEPointsUtilitiesTests
    {
        private Mock<ILWDataServiceUtil> _dataUtilMock;
        private Dictionary<long, PointType> _dPointTypes;
        private const string mockExceptionMessage = "Mock Exception Error";

        public enum PointsPerType {
            Dollar = 2500,
            BasicDollar = 1500,
            BonusDollar=1000,
            Bra = 7,
            Jean = 5,
            NetSpend = 3000
        }

        public enum PointsOnHoldPerType
        {
            Dollar = 470,
            BasicDollar = 300,
            BonusDollar = 170,
            Bra = 3,
            Jean = 1,
            NetSpend = 0
        }

        [SetUp]
        public void SetUp()
        {
            //reset this mock letting each test start fresh
            _dataUtilMock = new Mock<ILWDataServiceUtil>();
        }


        #region [Tests]
        #region [Tests for GetDollarPointsEllegibleForRewards]
        [Test]
        public void AEOPointsUtilities_GetDollarPointsEllegibleForRewards_Exception()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;

            Member member = new Member()
            {
                IpCode = 200001,
                LoyaltyCards = new List<VirtualCard>() { new VirtualCard() { VcKey = 10001, IsPrimary = true, IpCode = 200001 } }
            };

            this._dataUtilMock.Setup(
                        x => x.LoyaltyDataServiceInstance().GetAllPointTypes()
                    ).Throws(new Exception(mockExceptionMessage)).Verifiable();

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetDollarPointsEllegibleForRewards(member, startDate, endDate);
                 }
            );
        }
        [Test]
        public void AEOPointsUtilities_GetDollarPointsEllegibleForRewards_MemberNotNull()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;
            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint.Dollar, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock);


            Mock<Member> memberMock = new Mock<Member>();
            this.MockDataUtil_Member_GetPoints(PointsPerType.Dollar, memberMock);
            Member member = memberMock.Object;
            member.IpCode = 200001;
            member.LoyaltyCards = new List<VirtualCard>() { new VirtualCard() { VcKey = 10001, IsPrimary = true, IpCode = 200001 } };

            this.MockDataUtil_GetPointsOnHold(PointsOnHoldPerType.Dollar, this._dataUtilMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            decimal expectedValue = ((decimal)((int)PointsPerType.Dollar)) - ((decimal)((int)PointsOnHoldPerType.Dollar));

            Assert.AreEqual(expectedValue, pointsUtility.GetDollarPointsEllegibleForRewards(member, startDate, endDate));

            memberMock.Verify();
            this._dataUtilMock.Verify();
        }
        [Test]
        public void AEOPointsUtilities_GetDollarPointsEllegibleForRewards_MemberNull()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(0, pointsUtility.GetDollarPointsEllegibleForRewards(null, startDate, endDate));

            this._dataUtilMock.Verify();
        }
        #endregion
        #region [Tests for GetDollarPointsOnHold]
        [Test]
        public void AEOPointsUtilities_GetDollarPointsOnHold_Exception()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;            

            Member member = new Member()
            {
                IpCode = 200001,
                LoyaltyCards = new List<VirtualCard>() { new VirtualCard() { VcKey = 10001, IsPrimary = true, IpCode = 200001 } }
            };

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint.Dollar, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock);
            this.MockDataUtil_GetPointsOnHold_Exception(this._dataUtilMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetDollarPointsOnHold(member, startDate, endDate);
                 }
            );
            _dataUtilMock.Verify();
        }
        [Test]
        public void AEOPointsUtilities_GetDollarPointsOnHold_MemberNotNull()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            Member member = new Member()
            {
                IpCode = 200001,
                LoyaltyCards = new List<VirtualCard>() { new VirtualCard() { VcKey = 10001, IsPrimary = true, IpCode = 200001 } }
            };

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint.Dollar, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock);
            this.MockDataUtil_GetPointsOnHold(PointsOnHoldPerType.Dollar, this._dataUtilMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual((decimal)((int)PointsOnHoldPerType.Dollar), pointsUtility.GetDollarPointsOnHold(member, startDate, endDate));

            this._dataUtilMock.Verify();
        }
        [Test]
        public void AEOPointsUtilities_GetDollarPointsOnHold_MemberNull()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(0, pointsUtility.GetDollarPointsOnHold(null, startDate, endDate));

            this._dataUtilMock.Verify();
        }

        #endregion
        #region [Tests for GetTotalDollarPoints]
        [Test]
        public void AEOPointsUtilities_GetTotalDollarPoints_Exception()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            Mock<Member> memberMock = new Mock<Member>();

            this.MockDataUtilFor_GetAllPointTypes_Exception();
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetTotalDollarPoints(memberMock.Object, startDate, endDate);
                 }
            );
        }
        [Test]
        public void AEOPointsUtilities_GetTotalDollarPoints_MemberNotNull()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint.Dollar, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock);
            this.MockDataUtilFor_GetAllPointTypes();

            Mock<Member> member = new Mock<Member>();
            this.MockDataUtil_Member_GetPoints(PointsPerType.Dollar, member);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            IList<long> pointTypeIdsForDollar = pointsUtility.GetPointTypeIdsFilteredByType(TypeOfPoint.Dollar);

            Assert.AreEqual((decimal)((int)PointsPerType.Dollar), pointsUtility.GetTotalDollarPoints(member.Object, startDate, endDate));

            this._dataUtilMock.Verify();
            member.Verify();
        }
        [Test]
        public void AEOPointsUtilities_GetTotalDollarPoints_MemberNull() {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(0, pointsUtility.GetTotalDollarPoints(null, startDate, endDate));

            this._dataUtilMock.Verify();
        }
        #endregion
        #region [Tests for GetTotalBasicDollarPoints]
        [Test]
        public void GetTotalBasicDollarPoints_Exception()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            Mock<Member> memberMock = new Mock<Member>();

            this.MockDataUtilFor_GetAllPointTypes_Exception();
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetTotalBasicDollarPoints(memberMock.Object, startDate, endDate);
                 }
            );
        }
        [Test]
        public void GetTotalBasicDollarPoints_CallWithMemberNotNull_ExpectedReturnedPoints()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint.BasicDollar, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock);
            this.MockDataUtilFor_GetAllPointTypes();

            Mock<Member> member = new Mock<Member>();
            this.MockDataUtil_Member_GetPoints(PointsPerType.BasicDollar, member);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            IList<long> pointTypeIdsForBasicDollar = pointsUtility.GetPointTypeIdsFilteredByType(TypeOfPoint.BasicDollar);

            Assert.AreEqual((decimal)((int)PointsPerType.BasicDollar), pointsUtility.GetTotalBasicDollarPoints(member.Object, startDate, endDate));

            this._dataUtilMock.Verify();
            member.Verify();
        }
        [Test]
        public void GetTotalBasicDollarPoints_CallWithMemberNull_ZeroAsReturnedValue()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(0, pointsUtility.GetTotalBasicDollarPoints(null, startDate, endDate));

            this._dataUtilMock.Verify();
        }
        #endregion
        #region [Tests for GetTotalBonusDollarPoints]
        [Test]
        public void GetTotalBonusDollarPoints_Exception()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            Mock<Member> memberMock = new Mock<Member>();

            this.MockDataUtilFor_GetAllPointTypes_Exception();
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetTotalBonusDollarPoints(memberMock.Object, startDate, endDate);
                 }
            );
        }
        [Test]
        public void GetTotalBonusDollarPoints_CallWithMemberNotNull_ExpectedReturnedPoints()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint.BonusDollar, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock);
            this.MockDataUtilFor_GetAllPointTypes();

            Mock<Member> member = new Mock<Member>();
            this.MockDataUtil_Member_GetPoints(PointsPerType.BonusDollar, member);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            IList<long> pointTypeIdsForBonusDollar = pointsUtility.GetPointTypeIdsFilteredByType(TypeOfPoint.BonusDollar);

            Assert.AreEqual((decimal)((int)PointsPerType.BonusDollar), pointsUtility.GetTotalBonusDollarPoints(member.Object, startDate, endDate));

            this._dataUtilMock.Verify();
            member.Verify();
        }
        [Test]
        public void GetTotalBonusDollarPoints_CallWithMemberNull_ZeroAsReturnedValue()
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(0, pointsUtility.GetTotalBasicDollarPoints(null, startDate, endDate));

            this._dataUtilMock.Verify();
        }
        #endregion

        #region [Tests for GetPointsOnHold]
        [Test]
        [TestCase(PointsOnHoldPerType.Dollar, TypeOfPoint.Dollar,true)]
        [TestCase(PointsOnHoldPerType.BasicDollar, TypeOfPoint.BasicDollar, true)]
        [TestCase(PointsOnHoldPerType.BonusDollar, TypeOfPoint.BonusDollar, true)]
        [TestCase(PointsOnHoldPerType.Bra, TypeOfPoint.Bra,true)]
        [TestCase(PointsOnHoldPerType.Jean, TypeOfPoint.Jean,true)]
        [TestCase(PointsOnHoldPerType.NetSpend, TypeOfPoint.Netspend, false)]
        public void AEOPointsUtilities_GetPointsOnHold_Exception(PointsOnHoldPerType typeToGetPointsOnHold, TypeOfPoint pointType, bool mockGlobalVariable=true)
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            Member member = new Member()
            {
                IpCode = 200001,
                LoyaltyCards = new List<VirtualCard>() { new VirtualCard() { VcKey = 10001, IsPrimary = true, IpCode = 200001 } }
            };

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(pointType, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);
            this.MockDataUtil_GetPointsOnHold_Exception(this._dataUtilMock);
            
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            IList<long> pointTypeIdsForType = pointsUtility.GetPointTypeIdsFilteredByType(pointType);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetPointsOnHold(member, startDate, endDate, pointTypeIdsForType);
                 }
            );
            _dataUtilMock.Verify();
        }
        [Test]
        [TestCase(PointsOnHoldPerType.Dollar, TypeOfPoint.Dollar, true)]
        [TestCase(PointsOnHoldPerType.BasicDollar, TypeOfPoint.BasicDollar, true)]
        [TestCase(PointsOnHoldPerType.BonusDollar, TypeOfPoint.BonusDollar, true)]
        [TestCase(PointsOnHoldPerType.Bra, TypeOfPoint.Bra, true)]
        [TestCase(PointsOnHoldPerType.Jean, TypeOfPoint.Jean, true)]
        [TestCase(PointsOnHoldPerType.NetSpend, TypeOfPoint.Netspend, false)]
        public void AEOPointsUtilities_GetPointsOnHold_MemberNull(PointsOnHoldPerType typeToGetPointsOnHold, TypeOfPoint pointType, bool mockGlobalVariable = true)
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(pointType, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);
            this.MockDataUtil_GetPointsOnHold(typeToGetPointsOnHold, this._dataUtilMock, true);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            IList<long> pointTypeIdsForType = pointsUtility.GetPointTypeIdsFilteredByType(pointType);

            Assert.AreEqual(0, pointsUtility.GetPointsOnHold(null, startDate, endDate, pointTypeIdsForType.ToArray()));

            this._dataUtilMock.Verify();
        }
        [Test]
        [TestCase(PointsOnHoldPerType.Dollar, TypeOfPoint.Dollar, true)]
        [TestCase(PointsOnHoldPerType.BasicDollar, TypeOfPoint.BasicDollar, true)]
        [TestCase(PointsOnHoldPerType.BonusDollar, TypeOfPoint.BonusDollar, true)]
        [TestCase(PointsOnHoldPerType.Bra, TypeOfPoint.Bra, true)]
        [TestCase(PointsOnHoldPerType.Jean, TypeOfPoint.Jean, true)]
        [TestCase(PointsOnHoldPerType.NetSpend, TypeOfPoint.Netspend,false)]
        public void AEOPointsUtilities_GetPointsOnHold_MemberNotNull(PointsOnHoldPerType typeToGetPointsOnHold, TypeOfPoint pointType, bool mockGlobalVariable = true)
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            Member member = new Member()
            {
                IpCode = 200001,
                LoyaltyCards = new List<VirtualCard>() { new VirtualCard() { VcKey = 10001, IsPrimary = true, IpCode = 200001 } }                
            };

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(pointType, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);
            this.MockDataUtil_GetPointsOnHold(typeToGetPointsOnHold, this._dataUtilMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            IList<long> pointTypeIdsForType = pointsUtility.GetPointTypeIdsFilteredByType(pointType);

            Assert.AreEqual((decimal)((int)typeToGetPointsOnHold), pointsUtility.GetPointsOnHold(member, startDate, endDate, pointTypeIdsForType.ToArray()));

            this._dataUtilMock.Verify();

        }
        #endregion
        #region [Tests for GetPointTypeIdsFilteredByType]
        [Test]
        [TestCase(TypeOfPoint.Dollar, 2,true)]
        [TestCase(TypeOfPoint.BasicDollar, 1, true)]
        [TestCase(TypeOfPoint.BonusDollar, 1, true)]
        [TestCase(TypeOfPoint.Bra, 1, true)]
        [TestCase(TypeOfPoint.Jean, 1, true)]
        [TestCase(TypeOfPoint.Netspend, 1, false)]
        public void AEOPointsUtilities_GetPointTypeIdsFilteredByType(TypeOfPoint type,int expectedValue, bool mockGlobalVariable = true)
        {
            string key;
            string globalVaribleMock;
            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(type, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);
            this.MockDataUtilFor_GetAllPointTypes();

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(expectedValue, pointsUtility.GetPointTypeIdsFilteredByType(type).Count);            

            this._dataUtilMock.Verify();
        }
        [Test]
        [TestCase(TypeOfPoint.Dollar)]
        [TestCase(TypeOfPoint.BasicDollar)]
        [TestCase(TypeOfPoint.BonusDollar)]
        [TestCase(TypeOfPoint.Bra)]
        [TestCase(TypeOfPoint.Jean)]
        [TestCase(TypeOfPoint.Netspend)]
        public void AEOPointsUtilities_GetPointTypeIdsFilteredByType_Exception(TypeOfPoint type)
        {
            this.MockDataUtilFor_GetAllPointTypes_Exception();
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetPointTypeIdsFilteredByType(type);
                 }
            );
            
            this._dataUtilMock.Verify();
        }
        #endregion
        #region [Tests for GetPointTypesFromIdsList]
        [Test]
        public void AEOPointsUtilities_GetPointTypesFromIdsList_Exception()
        {
            this.MockDataUtilFor_GetPointType_Exception();
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetPointTypesFromIdsList(new List<long>() { 100000001, 100000002, 100000003 });
                 }
            );
            this._dataUtilMock.Verify();
        }
        [Test]
        public void AEOPointsUtilities_GetPointTypesFromIdsList_ListWithDataProvided()
        {
            this._dPointTypes = new Dictionary<long, PointType>();
            this._dPointTypes.Add(100000001, new PointType() { Name = "Mock Point Type A", ID = 100000001 });
            this._dPointTypes.Add(100000002, new PointType() { Name = "Mock Point Type B", ID = 100000002 });
            this._dPointTypes.Add(100000003, new PointType() { Name = "Mock Point Type C", ID = 100000003 });

            this.MockDataUtilFor_GetPointType(new List<long>() { 100000001, 100000002, 100000003, 100000000 }, this._dataUtilMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            IList<PointType> caseA = pointsUtility.GetPointTypesFromIdsList(new List<long>() { 100000000, 100000001, 100000002 });
            //Should not return a non-existent Item (mocked on the dictionary by ID [Item ID=100000000 => don't exist on dictionary]) 
            Assert.AreEqual(2, caseA.Count);
            foreach (PointType pt in caseA)
                Assert.IsTrue(this._dPointTypes.ContainsKey(pt.ID) && this._dPointTypes.ContainsValue(pt));


            IList<PointType> caseB = pointsUtility.GetPointTypesFromIdsList(new List<long>() { 100000001, 100000002, 100000003 });
            //Should return all the existent Items on the List 
            Assert.AreEqual(3, caseB.Count);
            foreach (PointType pt in caseB)
                Assert.IsTrue(this._dPointTypes.ContainsKey(pt.ID) && this._dPointTypes.ContainsValue(pt));

            this._dataUtilMock.Verify();
        }
        [Test]
        public void AEOPointsUtilities_GetPointTypesFromIdsList_NullOrVoidListProvided()
        {
            this._dPointTypes = new Dictionary<long, PointType>();
            this._dPointTypes.Add(100000001, new PointType() { Name = "Mock Point Type A", ID = 100000001 });
            this._dPointTypes.Add(100000002, new PointType() { Name = "Mock Point Type B", ID = 100000002 });

            this.MockDataUtilFor_GetPointType(new List<long>() { 100000001, 100000002, 100000000 }, this._dataUtilMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            Assert.IsEmpty(pointsUtility.GetPointTypesFromIdsList(null));
            Assert.IsEmpty(pointsUtility.GetPointTypesFromIdsList(new List<long>() { }));

            //Don't apply since the expected conditions should avoid the execution of the MockedData
            //this._dataUtilMock.Verify();
        }
        #endregion
        #region [Tests for GetTotalPoints]
        [Test]
        [TestCase(TypeOfPoint.Dollar, true)]
        [TestCase(TypeOfPoint.BasicDollar, true)]
        [TestCase(TypeOfPoint.BonusDollar, true)]
        [TestCase(TypeOfPoint.Bra, true)]
        [TestCase(TypeOfPoint.Jean, true)]
        [TestCase(TypeOfPoint.Netspend, false)]
        public void AEOPointsUtilities_GetTotalPoints_Exception(TypeOfPoint pointType, bool mockGlobalVariable = true)
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            Mock<Member> memberMock = new Mock<Member>();
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(pointType, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);
            this.MockDataUtilFor_GetAllPointTypes();
            this.MockDataUtilFor_Member_GetPoints_Exception(memberMock);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            IList<long> pointTypeIdsForType = pointsUtility.GetPointTypeIdsFilteredByType(pointType);
            
            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(mockExceptionMessage),
                 delegate {
                     pointsUtility.GetTotalPoints(memberMock.Object, startDate, endDate, pointTypeIdsForType);
                 }
            );
            
            _dataUtilMock.Verify();
            memberMock.Verify();
        }
        [Test]
        [TestCase(TypeOfPoint.Dollar,true)]
        [TestCase(TypeOfPoint.BasicDollar, true)]
        [TestCase(TypeOfPoint.BonusDollar, true)]
        [TestCase(TypeOfPoint.Bra,true)]
        [TestCase(TypeOfPoint.Jean,true)]
        [TestCase(TypeOfPoint.Netspend, false)]
        public void AEOPointsUtilities_GetTotalPoints_MemberNull(TypeOfPoint pointType, bool mockGlobalVariable = true) {
            this.MockDataUtilFor_GetAllPointTypes();
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(pointType, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);
            IList<long> pointTypeIdsForType = pointsUtility.GetPointTypeIdsFilteredByType(pointType);
            
            Assert.AreEqual(0, pointsUtility.GetTotalPoints(null, startDate, endDate, pointTypeIdsForType.ToArray()));
            
            this._dataUtilMock.Verify();
        }
        
        [Test]
        [TestCase(PointsPerType.Dollar,TypeOfPoint.Dollar, true )]
        [TestCase(PointsPerType.BasicDollar, TypeOfPoint.BasicDollar, true)]
        [TestCase(PointsPerType.BonusDollar, TypeOfPoint.BonusDollar, true)]
        [TestCase(PointsPerType.Bra, TypeOfPoint.Bra, true)]
        [TestCase(PointsPerType.Jean, TypeOfPoint.Jean, true)]
        [TestCase(PointsPerType.NetSpend, TypeOfPoint.Netspend, false)]
        public void AEOPointsUtilities_GetTotalPoints_MemberNotNull(PointsPerType typeToGetPoints, TypeOfPoint pointType, bool mockGlobalVariable = true)
        {
            DateTime startDate = (DateTime.Now).AddMonths(-5);
            DateTime endDate = DateTime.Now;
            string key;
            string globalVaribleMock;

            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(pointType, out key, out globalVaribleMock);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVaribleMock, mockGlobalVariable);
            this.MockDataUtilFor_GetAllPointTypes();
            //Member For Type
            Mock<Member> memberForType = new Mock<Member>();
            this.MockDataUtil_Member_GetPoints(typeToGetPoints, memberForType);
            
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            IList<long> pointTypeIdsForDollar = pointsUtility.GetPointTypeIdsFilteredByType(pointType);
            
            Assert.AreEqual((decimal)((int)typeToGetPoints), pointsUtility.GetTotalPoints(memberForType.Object, startDate, endDate, pointTypeIdsForDollar.ToArray()));
            
            this._dataUtilMock.Verify();
            memberForType.Verify();
            
        }
        #endregion

        #region[Tests for GetValidPointTypeNamesByType]
        [Test]
        [TestCase(TypeOfPoint.Dollar)]
        [TestCase(TypeOfPoint.BasicDollar)]
        [TestCase(TypeOfPoint.BonusDollar)]
        [TestCase(TypeOfPoint.Bra)]
        [TestCase(TypeOfPoint.Jean)]
        public void AEOPointsUtilities_GetValidPointTypeNamesByType_GlobalValueMissingException(TypeOfPoint type)
        {
            string key;
            string globalVariableMockValue;
            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(type, out key, out globalVariableMockValue);
            globalVariableMockValue = "";
            this.MockDataUtil_GetValidPointTypeNamesByType(key, "");

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            string expectedErrorMessage = "Global Value Configuration [" + key + "] is missing.";

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(expectedErrorMessage),
                 delegate {
                     pointsUtility.GetValidPointTypeNamesByType(type);
                 }
            );
            _dataUtilMock.Verify();
        }

        [Test]
        [TestCase(TypeOfPoint.Dollar,2)]
        [TestCase(TypeOfPoint.BasicDollar,1)]
        [TestCase(TypeOfPoint.BonusDollar,1)]
        [TestCase(TypeOfPoint.Bra,1)]
        [TestCase(TypeOfPoint.Jean,1)]
        public void AEOPointsUtilities_GetValidPointTypeNamesByType(TypeOfPoint type, int expectedItems)
        {
            string key;
            string globalVariableMockValue;
            this.MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(type, out key, out globalVariableMockValue);
            this.MockDataUtil_GetValidPointTypeNamesByType(key, globalVariableMockValue);

            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            Assert.AreEqual(expectedItems, pointsUtility.GetValidPointTypeNamesByType(type).Count);

            _dataUtilMock.Verify();
        }

        [Test]
        [TestCase(TypeOfPoint.Netspend)]
        public void AEOPointsUtilities_GetValidPointTypeNamesByType_CodeNotImplementedException(TypeOfPoint type)
        {
            
            AEPointsUtilies pointsUtility = new AEPointsUtilies(this._dataUtilMock.Object);

            string expectedErrorMessage = "Code not implemented for Option [TypeOfPoint." + type.ToString() + "]";

            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo(expectedErrorMessage),
                 delegate {
                     pointsUtility.GetValidPointTypeNamesByType(type);
                 }
            );
            _dataUtilMock.Verify();
        }
        #endregion
        #endregion


        #region [Private Members]
        private void MockDataUtilFor_GetAllPointTypes()
        {
            this._dataUtilMock.Setup(
                x => x.LoyaltyDataServiceInstance().GetAllPointTypes()
                ).Returns( 
                    new List<PointType>() {
                        //new PointType(){ Name="Jean Points", ID=100000001 },
                        //new PointType(){ Name="Bra Points",  ID=100000002 },
                        //new PointType(){ Name="NetSpend",  ID=100000003 },
                        //new PointType(){ Name="AEO Connected Points",  ID=100000004 },
                        //new PointType(){ Name="AEO Connected Bonus Points",  ID=100000005 },
                        new PointType(){ Name="Mock Jean Points A", ID=100000001 },
                        new PointType(){ Name="Mock Bra Points A",  ID=100000002 },
                        new PointType(){ Name="NetSpend",  ID=100000003 },
                        new PointType(){ Name="Mock Dollar Points A",  ID=100000004 },
                        new PointType(){ Name="Mock Dollar Points B",  ID=100000005 },
                        new PointType(){ Name="AEO Customer Service Points",  ID=100000006 },
                        new PointType(){ Name="AEO Connected Engagement Points",  ID=100000007 },
                        new PointType(){ Name="AEO Connected Sign Up Bonus",  ID=100000008 },
                        new PointType(){ Name="Adjustment Points",  ID=100000009 },
                        new PointType(){ Name="StartingPoints",  ID=100000010 },
                        new PointType(){ Name="Basic Points",  ID=100000011 },
                        new PointType(){ Name="Bonus Points",  ID=100000012 },
                        new PointType(){ Name="CS Points",  ID=100000013 },
                        new PointType(){ Name="AEO Rewards Base Points",  ID=100000014 },
                        new PointType(){ Name="AEO Rewards Bonus Points",  ID=100000015 },
                        new PointType(){ Name="AEO Rewards CS Points",  ID=100000016 },
                        new PointType(){ Name="AEO Visa Card Points",  ID=100000017 },
                        new PointType(){ Name="Adjustment Bonus Points",  ID=100000018 }
                    }
                ).Verifiable();
        }
        private void MockDataUtilFor_GetAllPointTypes_Exception()
        {
            this._dataUtilMock.Setup(
                x => x.LoyaltyDataServiceInstance().GetAllPointTypes()
                ).Throws(new Exception(mockExceptionMessage)).Verifiable();
        }

        private void MockDataUtilFor_GetPointType_Exception()
        {
            this._dataUtilMock.Setup(
                        x => x.LoyaltyDataServiceInstance().GetPointType(It.IsAny<long>())
                    ).Throws(new Exception(mockExceptionMessage)).Verifiable();
        }

        private void MockDataUtilFor_GetPointType(List<long> ids, Mock<ILWDataServiceUtil> dataUtilMock)
        {
            if (ids != null) {
                foreach(long id in ids){
                    dataUtilMock.Setup(
                        x => x.LoyaltyDataServiceInstance().GetPointType(id)
                    ).Returns(
                        this.MockDataUtil_GetPointType_Action(id)
                    );
                }
            }
        }

        private PointType MockDataUtil_GetPointType_Action(long pointTypeId)
        {
            PointType retObject = null;

            if (this._dPointTypes != null) {
                if (this._dPointTypes.ContainsKey(pointTypeId))
                    retObject = this._dPointTypes[pointTypeId];
            }

            return retObject;
        }

        private void MockDataUtil_Member_GetPoints(PointsPerType type, Mock<Member> member)
        {
            member.Setup(
                x => x.GetPoints(It.IsAny<long[]>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())
            ).Returns(
                (decimal)((int)type)
            ).Verifiable();            
        }

        private void MockDataUtilFor_Member_GetPoints_Exception(Mock<Member> member)
        {
            member.Setup(
                x => x.GetPoints(It.IsAny<long[]>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())
            ).Throws(new Exception(mockExceptionMessage)).Verifiable();
        }

        private void MockDataUtil_GetPointsOnHold(PointsOnHoldPerType type, Mock<ILWDataServiceUtil> dataUtilMock, bool MemberIsNull = false)
        {
            this._dPointTypes = new Dictionary<long, PointType>();
            this._dPointTypes.Add(100000001, new PointType() { Name = "Jean Points", ID = 100000001 });
            this._dPointTypes.Add(100000002, new PointType() { Name = "Bra Points", ID = 100000002 });
            this._dPointTypes.Add(100000003, new PointType() { Name = "AEO Connected Points", ID = 100000003 });
            this._dPointTypes.Add(100000004, new PointType() { Name = "AEO Connected Bonus Points", ID = 100000004 });
            this._dPointTypes.Add(100000005, new PointType() { Name = "AEO Customer Service Points", ID = 100000005 });
            this._dPointTypes.Add(100000006, new PointType() { Name = "AEO Connected Engagement Points", ID = 100000006 });
            this._dPointTypes.Add(100000007, new PointType() { Name = "NetSpend", ID = 100000007 });

            this.MockDataUtilFor_GetPointType(this._dPointTypes.Keys.ToList<long>(), dataUtilMock);

            dataUtilMock.Setup(
                x => x.LoyaltyDataServiceInstance().GetAllPointTypes()
                ).Returns(
                    new List<PointType>(this._dPointTypes.Values)
                ).Verifiable();

            if (!MemberIsNull)
            {
                dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAllPointEvents()
                ).Returns(
                    new List<PointEvent>() {
                        new PointEvent(){  ID=20000001, Name = "Mock Point Event A"},
                        new PointEvent(){  ID=20000002, Name = "Mock Point Event B"}
                    }
                ).Verifiable();

                dataUtilMock.Setup(
                        x => x.LoyaltyDataServiceInstance().GetPointsOnHold(It.IsAny<IList<VirtualCard>>(), It.IsAny<IList<PointType>>(), It.IsAny<IList<PointEvent>>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())
                    ).Returns(
                        (decimal)((int)type)
                    ).Verifiable();
            }
            
        }

        private void MockDataUtil_GetPointsOnHold_Exception(Mock<ILWDataServiceUtil> dataUtilMock)
        {
            this._dPointTypes = new Dictionary<long, PointType>();
            this._dPointTypes.Add(100000001, new PointType() { Name = "Jean Points", ID = 100000001 });
            this._dPointTypes.Add(100000002, new PointType() { Name = "Bra Points", ID = 100000002 });
            this._dPointTypes.Add(100000003, new PointType() { Name = "AEO Connected Points", ID = 100000003 });
            this._dPointTypes.Add(100000004, new PointType() { Name = "AEO Connected Bonus Points", ID = 100000004 });
            this._dPointTypes.Add(100000005, new PointType() { Name = "AEO Customer Service Points", ID = 100000005 });
            this._dPointTypes.Add(100000006, new PointType() { Name = "AEO Connected Engagement Points", ID = 100000006 });
            this._dPointTypes.Add(100000007, new PointType() { Name = "NetSpend", ID = 100000007 });

            this.MockDataUtilFor_GetPointType(this._dPointTypes.Keys.ToList<long>(), dataUtilMock);

            dataUtilMock.Setup(
                x => x.LoyaltyDataServiceInstance().GetAllPointTypes()
                ).Returns(
                    new List<PointType>(this._dPointTypes.Values)
                ).Verifiable();

            dataUtilMock.Setup(
                    x => x.LoyaltyDataServiceInstance().GetAllPointEvents()
                ).Returns(
                    new List<PointEvent>() {
                        new PointEvent(){  ID=20000001, Name = "Mock Point Event A"},
                        new PointEvent(){  ID=20000002, Name = "Mock Point Event B"}
                    }
                ).Verifiable();

            dataUtilMock.Setup(
                        x => x.LoyaltyDataServiceInstance().GetPointsOnHold(It.IsAny<IList<VirtualCard>>(), It.IsAny<IList<PointType>>(), It.IsAny<IList<PointEvent>>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())
                    ).Throws(new Exception(mockExceptionMessage)).Verifiable();
        }
        
        private void MockDataUtil_GetValidPointTypeNamesByType(string key, string mockConfiguredValue, bool implement=true) {
            if(implement)
                _dataUtilMock.Setup(
                    x => x.DataServiceInstance().GetClientConfigProp(key)
                    ).Returns(mockConfiguredValue).Verifiable();
        }

        private void MockDataUtil_GetValidPointTypeNamesByType_GetMockKeyValue(TypeOfPoint type, out string strKey, out string strValue)
        {
            strKey = "";
            strValue = "";
            switch (type) {
                case TypeOfPoint.Dollar:
                    strKey = "PointTypeNamesForDollarReward";
                    strValue = "Mock Dollar Points A,Mock Dollar Points B";
                    break;
                case TypeOfPoint.BonusDollar:
                    strKey = "PointTypeNamesForBonusPoints";
                    strValue = "Mock Dollar Points A";
                    break;
                case TypeOfPoint.BasicDollar:
                    strKey = "PointTypeNamesForBasicPoints";
                    strValue = "Mock Dollar Points B";
                    break;
                case TypeOfPoint.Bra:
                    strKey = "PointTypeNamesForBraReward";
                    strValue = "Mock Bra Points A";
                    break;
                case TypeOfPoint.Jean:
                    strKey = "PointTypeNamesForJeanReward";
                    strValue = "Mock Jean Points A";
                    break;
            }
        }

        //[Test]
        //public void testLoop()
        //{
        //    int x = 0;

        //    foreach (int x1 in returnSomething())
        //    {
        //        x += x1;
        //    }
        //}
        //private List<int> returnSomething() {
        //    List<int> list = new List<int>() { 1,2,3,4,5 };
        //    return list;
        //}
        #endregion

    }
}
