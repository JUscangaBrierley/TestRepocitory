using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public interface IMemberDao: IDaoBase<Member>
    {
        void Create(IList<Member> members);
        int Delete(long[] ipCodes);
        void Delete(long ipcode);
        void DeleteVirtualCards(Member member);
        void DeleteVirtualCards(long[] ipCodes);
        bool MemberWithAlternateIDExists(long ipCode, string alternateId);
        bool MemberWithEmailAddressExists(long ipCode, string emailAddress);
        bool MemberWithLoyaltyIDExists(long ipCode, string loyaltyIDNumber);
        bool MemberWithUserNameExists(long ipCode, string username);
        List<Member> Retrieve(string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo);
        Member Retrieve(long ipcode, bool retrieveCards);
        List<Member> Retrieve(LWQueryBatchInfo batchInfo);
        List<Member> RetrieveAll(long[] ipCodes, bool loadVirtualCards);
        List<Member> RetrieveAllByUniqueIdentifiers(Member member);
        List<Member> RetrieveAllByVcKeys(long[] vcKeys);
        Member RetrieveByAlternateID(string alternateID, bool retrieveCards);
        Member RetrieveByEmailAddress(string emailAddress, bool retrieveCards);
        Member RetrieveByLoyaltyIDNumber(string loyaltyIDNumber, bool retrieveCards);
        List<Member> RetrieveByName(string firstName, string lastName, string middleName, LWQueryBatchInfo batchInfo);
        List<Member> RetrieveByPhoneNumber(string phoneNumber, LWQueryBatchInfo batchInfo);
        List<Member> RetrieveByPostalCode(string postalCode, LWQueryBatchInfo batchInfo);
        Member RetrieveByResetCode(string resetCode);
        Member RetrieveByUserName(string username, bool retrieveCards);
        Member RetrieveByVcKey(long vcKey, bool retrieveCards);
        List<long> RetrieveIds(string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo);
        void RetrieveVirtualCards(Member member);
        void RetrieveVirtualCards(List<Member> members);
        void Update(IList<Member> members);
    }
}
