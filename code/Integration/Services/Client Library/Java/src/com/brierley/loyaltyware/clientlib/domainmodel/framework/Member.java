package com.brierley.loyaltyware.clientlib.domainmodel.framework;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.brierley.loyaltyware.clientlib.domainmodel.LWAttributeSetContainer;

public class Member extends LWAttributeSetContainer {
	public enum MemberStatusEnum {
		Active, Disabled, Terminated, Locked, NonMember, Merged, PreEnrolled
	};

	public enum VirtualCardRetrieveType { FirstCard, PrimaryCard, MostRecentRegistered, MostRecentIssued, EarliestRegistered, EarliestIssued };

	private long ipCode;
	public long getIpCode() {
		return ipCode;
	}
	public void setIpCode(long ipcode) {
			ipCode = ipcode;
	}

	private Date memberCreateDate;
	public Date getMemberCreateDate() {
		return memberCreateDate;
	}
	public void setMemberCreateDate(Date memberCreateDate) {
		this.memberCreateDate = memberCreateDate;
	}

	private Date memberCloseDate;
	public Date getMemberCloseDate() {
		return memberCloseDate;
	}
	public void setMemberCloseDate(Date memberCloseDate) {
		this.memberCloseDate = memberCloseDate;
	}

	private MemberStatusEnum memberStatus;
	public MemberStatusEnum getMemberStatus() {
		return memberStatus;
	}
	public void setMemberStatus(MemberStatusEnum status) {
		memberStatus = status;
	}

	private Date birthDate;
	public Date getBirthDate() {
		return birthDate;
	}
	public void setBirthDate(Date birthDate) {
		this.birthDate = birthDate;
	}

	private String firstName;
	public String getFirstName() {
		return firstName;
	}
	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}

	private String lastName;
	public String getLastName() {
		return lastName;
	}
	public void setLastName(String lastName) {
		this.lastName = lastName;
	}

	private String middleName;
	public String getMiddleName() {
		return middleName;
	}
	public void setMiddleName(String middleName) {
		this.middleName = middleName;
	}

	private String namePrefix;
	public String getNamePrefix() {
		return namePrefix;
	}
	public void setNamePrefix(String namePrefix) {
		this.namePrefix = namePrefix;
	}

	private String nameSuffix;
	public String getNameSuffix() {
		return nameSuffix;
	}
	public void setNameSuffix(String nameSuffix) {
		this.nameSuffix = nameSuffix;
	}

	private String alternateId;
	public String getAlternateId() {
		return alternateId;
	}
	public void setAlternateId(String alternateId) {
		this.alternateId = alternateId;
	}

	private String username;
	public String getUsername() {
		return username;
	}
	public void setUsername(String username) {
		this.username = username;
	}

	private String password;
	public String getPassword() {
		return password;
	}
	public void setPassword(String password) {
		this.password = password;
	}

	private String primaryEmailAddress;
	public String getPrimaryEmailAddress() {
		return primaryEmailAddress;
	}
	public void setPrimaryEmailAddress(String primaryEmailAddress) {
		this.primaryEmailAddress = primaryEmailAddress;
	}

	private String primaryPhoneNumber;
	public String getPrimaryPhoneNumber() {
		return primaryPhoneNumber;
	}
	public void setPrimaryPhoneNumber(String primaryPhoneNumber) {
		this.primaryPhoneNumber = primaryPhoneNumber;
	}

	private String primaryPostalCode;
	public String getPrimaryPostalCode() {
		return primaryPostalCode;
	}
	public void setPrimaryPostalCode(String primaryPostalCode) {
		this.primaryPostalCode = primaryPostalCode;
	}

	private Date lastActivityDate;
	public Date getLastActivityDate() {
		return lastActivityDate;
	}
	public void setLastActivityDate(Date lastActivityDate) {
		this.lastActivityDate = lastActivityDate;
	}

	private Boolean isEmployee;
	public Boolean getIsEmployee() {
		return isEmployee;
	}
	public void setIsEmployee(Boolean isEmployee) {
		this.isEmployee = isEmployee;
	}

	private String changedBy;
	public String getChangedBy() {
		return changedBy;
	}
	public void setChangedBy(String changedBy) {
		this.changedBy = changedBy;
	}

	public List<VirtualCard> getLoyaltyCards(){
        List<VirtualCard> loyaltyCards = new ArrayList<VirtualCard>();
        for (LWAttributeSetContainer a : getChildren()) {
			if (a instanceof VirtualCard){
				loyaltyCards.add((VirtualCard) a);
			}
        }
        return loyaltyCards;
    }

	public VirtualCard getLoyaltyCard(String loyaltyId){
        VirtualCard theCard = null;
        for (LWAttributeSetContainer a : getChildren()) {
			if (a instanceof VirtualCard){
				VirtualCard vc = (VirtualCard) a;
				if (vc.getLoyaltyIdNumber().equals(loyaltyId)){
					theCard = vc;
					break;
				}
			}
        }
        return theCard;
    }

	public VirtualCard getLoyaltyCardByType(VirtualCardRetrieveType type)
    {
        List<VirtualCard> loyaltyCards = getLoyaltyCards();

        VirtualCard theCard = null;
        switch (type)
        {
        	case FirstCard:
        		theCard = loyaltyCards.get(0);
        		break;
            case PrimaryCard:
                for (VirtualCard vc : loyaltyCards){
                    if (vc.getIsPrimary())
                    {
                        return vc;
                    }
                }
                break;
            case EarliestIssued:
            	for (VirtualCard vc : loyaltyCards){
            		if ( vc.getDateIssued().before(theCard.getDateIssued())){
                        theCard = vc;
                    }
                }
                break;
            case MostRecentIssued:
            	for (VirtualCard vc : loyaltyCards){
                    if (vc.getDateIssued().after(theCard.getDateIssued()))
                    {
                        theCard = vc;
                    }
                }
                break;
            case EarliestRegistered:
            	for (VirtualCard vc : loyaltyCards){
            		if ( vc.getDateRegistered().before(theCard.getDateRegistered())){
                        theCard = vc;
                    }
                }
                break;
            case MostRecentRegistered:
            	for (VirtualCard vc : loyaltyCards){
                    if (vc.getDateRegistered().after(theCard.getDateRegistered()))
                    {
                        theCard = vc;
                    }
                }
                break;
            default:
                return null;
        }
        return theCard;
    }
}
