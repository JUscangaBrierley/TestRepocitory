create or replace package MemberUtility is
type rcursor IS REF CURSOR;

-- Public function and procedure declarations

procedure FindMember(LoyaltyID IN VARCHAR2 DEFAULT NULL,
                      FirstName       IN VARCHAR2 DEFAULT NULL,
                      LastName        IN VARCHAR2 DEFAULT NULL,
                      Email           IN VARCHAR2 DEFAULT NULL,
                      NonMember       IN NUMBER,
                      retval          IN OUT rcursor);

end MemberUtility;
