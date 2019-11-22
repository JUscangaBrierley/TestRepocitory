using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class CSService : ServiceBase
	{
		private const string _className = "CSService";
		private const string CONTACTSTATUS_LIST = "ContactStatusList";
		private const int DEFAULT_MAX_FAILED_PASSWORD_ATTEMPTS = 3;

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_CUSTOMERSERVICE);
		private ICSPermissionCallback _cb = null;

        private CSAgentDao _csAgentDao;
        private CSFunctionDao _csFunctionDao;
        private CSLoginEventDao _csLoginEventDao;
        private CSNoteDao _csNoteDao;
        private CSRoleDao _csRoleDao;
        private CSRoleFunctionDao _csRoleFunctionDao;
        private ContactHistoryDao _contactHistoryDao;
        private ContactStatusDao _contactStatusDao;

        public CSAgentDao CSAgentDao
        {
            get
            {
                if (_csAgentDao == null)
                {
                    _csAgentDao = new CSAgentDao(Database, Config);
                }
                return _csAgentDao;
            }
        }

        public CSFunctionDao CSFunctionDao
        {
            get
            {
                if (_csFunctionDao == null)
                {
                    _csFunctionDao = new CSFunctionDao(Database, Config);
                }
                return _csFunctionDao;
            }
        }

        public CSLoginEventDao CSLoginEventDao
        {
            get
            {
                if (_csLoginEventDao == null)
                {
                    _csLoginEventDao = new CSLoginEventDao(Database, Config);
                }
                return _csLoginEventDao;
            }
        }

        public CSNoteDao CSNoteDao
        {
            get
            {
                if (_csNoteDao == null)
                {
                    _csNoteDao = new CSNoteDao(Database, Config);
                }
                return _csNoteDao;
            }
        }

        public CSRoleDao CSRoleDao
        {
            get
            {
                if (_csRoleDao == null)
                {
                    _csRoleDao = new CSRoleDao(Database, Config);
                }
                return _csRoleDao;
            }
        }

        public CSRoleFunctionDao CSRoleFunctionDao
        {
            get
            {
                if (_csRoleFunctionDao == null)
                {
                    _csRoleFunctionDao = new CSRoleFunctionDao(Database, Config);
                }
                return _csRoleFunctionDao;
            }
        }

        public ContactHistoryDao ContactHistoryDao
        {
            get
            {
                if (_contactHistoryDao == null)
                {
                    _contactHistoryDao = new ContactHistoryDao(Database, Config);
                }
                return _contactHistoryDao;
            }
        }

        public ContactStatusDao ContactStatusDao
        {
            get
            {
                if (_contactStatusDao == null)
                {
                    _contactStatusDao = new ContactStatusDao(Database, Config);
                }
                return _contactStatusDao;
            }
        }
			
		internal CSService(ServiceConfig config)
			: base(config)
		{
		}


		public void CreateCSAgent(CSAgent agent)
		{
            string methodName = "CreateCSAgent";

            try
            {
                _logger.Trace(_className, methodName, "Creating new CSAgent.");
                if (LWPasswordUtil.IsHashingEnabled() && string.IsNullOrEmpty(agent.Salt))
                {
                    agent.Salt = CryptoUtil.GenerateSalt();
                }
                if (!string.IsNullOrEmpty(agent.Password))
                {
                    string newPassword = agent.Password;
                    // ValidatePassword() throws Exception when password is invalid
                    LWPasswordUtil.ValidatePassword(agent.Username, newPassword);

                    /*LWConfiguration config = */
                    LWConfigurationUtil.GetCurrentConfiguration();
                    if (LWPasswordUtil.IsHashingEnabled())
                    {
                        agent.Password = LWPasswordUtil.HashPassword(agent.Salt, newPassword);
                    }
                    else
                    {
                        agent.Password = CryptoUtil.EncodeUTF8(newPassword);
                    }
                }
                CSAgentDao.Create(agent);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error creating CSAgent.", ex);
                throw;
            }
		}

		public void UpdateCSAgent(CSAgent agent)
		{
            string methodName = "CreateCSAgent";

            try
            {
                _logger.Trace(_className, methodName, "Updating new CSAgent.");
                if (LWPasswordUtil.IsHashingEnabled() && string.IsNullOrEmpty(agent.Salt))
                {
                    agent.Salt = CryptoUtil.GenerateSalt();
                }
                CSAgentDao.Update(agent);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error updating CSAgent.", ex);
                throw;
            }
		}

		public CSAgent LoginCSAgent(string identity, string password, string resetCode, ref LoginStatusEnum loginStatus)
		{
            // validate arguments
            if (string.IsNullOrEmpty(identity))
            {
                throw new ArgumentNullException("identity");
            }
            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(resetCode))
            {
                throw new ArgumentNullException("password");
            }

            // load csagent
            string encodedPasswordForLogging = CryptoUtil.EncodeUTF8(password);
            CSAgent csagent = CSAgentDao.Retrieve(identity, (AgentAccountStatus?)null);
            if (csagent == null)
            {
                // no user found with that identity
                CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.UsernameInvalid);
                throw new InvalidCSAgentIdentityException(identity);
            }

            // handle csagent resolved but csagent status doesn't allow login
            switch (csagent.Status)
            {
                case AgentAccountStatus.InActive:
                    CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.AccountInactive);
                    throw new CSAgentStatusException(csagent.Status);

                case AgentAccountStatus.Locked:
                    CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.AccountLocked);
                    throw new CSAgentStatusException(csagent.Status);
            }

            // compare provided password with actual password
            string hashedPassword;
            if (LWPasswordUtil.IsHashingEnabled())
            {
                if (string.IsNullOrEmpty(csagent.Salt))
                {
                    csagent.Salt = CryptoUtil.GenerateSalt();
                }
                hashedPassword = LWPasswordUtil.HashPassword(csagent.Salt, password);
            }
            else
            {
                hashedPassword = encodedPasswordForLogging;
            }
            if (csagent.Password != hashedPassword)
            {
                // Try the reset code or password as the reset code
                if ((!string.IsNullOrEmpty(resetCode) && csagent.ResetCode == resetCode && csagent.ResetCodeDate >= DateTime.Now) ||
                    (!string.IsNullOrEmpty(password) && csagent.ResetCode == password && csagent.ResetCodeDate >= DateTime.Now))
                {
                    string givenPass = csagent.ResetCode == resetCode ? resetCode : encodedPasswordForLogging;
                    CreateCSLoginEvent(identity, givenPass, CSLoginEventType.PasswordExpired);
                    csagent.PasswordChangeRequired = true;
                    csagent.ResetCode = null;
                    csagent.ResetCodeDate = null;
                    UpdateCSAgent(csagent);
                    loginStatus = LoginStatusEnum.PasswordResetRequired;
                    resetCode = string.IsNullOrEmpty(resetCode) ? password : resetCode; // Return the value of the reset code in the reset code parameter
                }
                // invalid password, update # failues
                else
                {
                    // invalid password, update # failues
                    csagent.FailedPasswordAttemptCount = csagent.FailedPasswordAttemptCount + 1;
                    bool enableAccountLock = !StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWCSAccountLockDisabled"), false);
                    string cfgMaxPasswordAttempts = LWConfigurationUtil.GetConfigurationValue("LWCSMaxFailedPasswordAttempts");
                    int maxFailedPasswordAttempts = StringUtils.FriendlyInt32(cfgMaxPasswordAttempts, DEFAULT_MAX_FAILED_PASSWORD_ATTEMPTS);
                    if (enableAccountLock && csagent.FailedPasswordAttemptCount > maxFailedPasswordAttempts)
                    {
                        // lock the account
                        csagent.Status = AgentAccountStatus.Locked;
                        CSAgentDao.Update(csagent);
                        CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.AccountLocked);
                        loginStatus = LoginStatusEnum.LockedOut;
						return null;
                    }
                    else
                    {
                        // not locked yet, so just an invalid password this time
                        CSAgentDao.Update(csagent);
                        CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.PasswordInvalid);
						loginStatus = LoginStatusEnum.Failure;
						return null;
                    }
                }
            }
            else
            {
                // password match
                if (csagent.IsPasswordChangeRequired())
                {
                    // login success, but password is expired
                    CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.PasswordExpired);
                    loginStatus = LoginStatusEnum.PasswordResetRequired;
                }
                else
                {
                    // login success, reset failed count if needed
                    if (csagent.FailedPasswordAttemptCount > 0)
                    {
                        csagent.FailedPasswordAttemptCount = 0;
                        CSAgentDao.Update(csagent);
                    }
                    CreateCSLoginEvent(identity, encodedPasswordForLogging, CSLoginEventType.LoginSuccess);
                    loginStatus = LoginStatusEnum.Success;
                }
            }
            return csagent;
		}

		private void CreateCSLoginEvent(String providedUsername, String providedPassword, CSLoginEventType eventType)
		{
            if (providedUsername.Length > 255) providedUsername = providedUsername.Substring(0, 255);
            if (providedPassword.Length > 255) providedPassword = providedPassword.Substring(0, 255);

            CSLoginEvent csLoginEvent = new CSLoginEvent();
            csLoginEvent.ProvidedUsername = providedUsername;
            csLoginEvent.ProvidedPassword = providedPassword;
            csLoginEvent.RemoteIPAddress = LWPasswordUtil.GetRemoteIPAddress();
            csLoginEvent.RemoteUserName = LWPasswordUtil.GetRemoteUserName();
            csLoginEvent.RemoteUserAgent = LWPasswordUtil.GetRemoteUserAgent();
            csLoginEvent.EventSource = "CustSvcMembershipProvider-" + LWPasswordUtil.GetLocalHostName();
            csLoginEvent.EventType = eventType;
            CSLoginEventDao.Create(csLoginEvent);
		}

		public void CreateCSLoginEvent(CSLoginEvent csLoginEvent)
		{
            if (csLoginEvent == null)
            {
                throw new ArgumentNullException("csLoginEvent");
            }

            CSLoginEventDao.Create(csLoginEvent);
		}

		public void ChangeCSAgentPassword(string identity, string oldPassword, string newPassword)
		{
            // load csagent
            CSAgent csagent = GetCSAgentByUserName(identity, AgentAccountStatus.Active);

            // provided old password must match existing csagent password
            bool oldMatches = true;
            if (string.IsNullOrEmpty(csagent.Password) ^ string.IsNullOrEmpty(oldPassword))
            {
                oldMatches = false;
            }
            else if (!string.IsNullOrEmpty(csagent.Password) && !string.IsNullOrEmpty(oldPassword))
            {
                if (LWPasswordUtil.IsHashingEnabled())
                {
                    try
                    {
                        oldPassword = LWPasswordUtil.HashPassword(csagent.Salt, oldPassword);
                    }
                    catch (LWException ex)
                    {
                        throw new AuthenticationException("Unable to hash 'Old Password': " + ex.Message, ex);
                    }
                }
                else
                {
                    oldPassword = CryptoUtil.EncodeUTF8(oldPassword);
                }
                if (csagent.Password != oldPassword)
                {
                    oldMatches = false;
                }
            }
            if (!oldMatches)
            {
                throw new BadPasswordIncorrectOldPasswordException();
            }

            // new password must be different than old password
            if (!string.IsNullOrEmpty(csagent.Password))
            {
                string tmp;
                if (LWPasswordUtil.IsHashingEnabled())
                {
                    tmp = LWPasswordUtil.HashPassword(csagent.Salt, newPassword);
                }
                else
                {
                    tmp = CryptoUtil.EncodeUTF8(newPassword);
                }
                if (csagent.Password == tmp)
                {
                    throw new BadPasswordMatchesOldPasswordException();
                }
            }

            // try to change password, exception thrown if password is invalid
            ChangeCSAgentPassword(csagent, newPassword);
		}

		public void ChangeCSAgentPassword(CSAgent csagent, string newPassword)
		{
            const string methodName = "ChangeCSAgentPassword";
            if (csagent == null) throw new ArgumentNullException("csagent");
            if (string.IsNullOrEmpty(csagent.Username)) throw new ArgumentNullException("csagent.Username");

            // ValidatePassword() throws Exception when password is invalid
            LWPasswordUtil.ValidatePassword(csagent.Username, newPassword);

            _logger.Debug(_className, methodName, "Change password for csagent: " + csagent.Username);

            /*LWConfiguration config = */
            LWConfigurationUtil.GetCurrentConfiguration();
            if (LWPasswordUtil.IsHashingEnabled())
            {
                if (string.IsNullOrEmpty(csagent.Salt))
                {
                    csagent.Salt = CryptoUtil.GenerateSalt();
                }
                csagent.Password = LWPasswordUtil.HashPassword(csagent.Salt, newPassword);
            }
            else
            {
                csagent.Password = CryptoUtil.EncodeUTF8(newPassword);
            }
            if (csagent.PasswordExpireDate != null && DateTime.Today > csagent.PasswordExpireDate)
            {
                bool enablePasswordExpiry = !StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("LWPasswordExpiryDisabled"), false);
                if (enablePasswordExpiry)
                {
                    // PCI 8.5.9: Change user passwords at least every 90 days. 
                    double daysToAdd = StringUtils.FriendlyDouble(LWConfigurationUtil.GetConfigurationValue("LWPasswordExpiryInterval"), 90);
                    if (daysToAdd > 90) daysToAdd = 90;
                    csagent.PasswordExpireDate = DateTime.Today.AddDays(daysToAdd);
                }
                else
                {
                    csagent.PasswordExpireDate = DateTimeUtil.MaxValue;
                }
            }
            csagent.FailedPasswordAttemptCount = 0;
            csagent.PasswordChangeRequired = false;
            csagent.ResetCode = null;
            csagent.ResetCodeDate = null;
            CSAgentDao.Update(csagent);
		}

        public string GenerateCSAgentResetCode(CSAgent agent, int expiryMinutes)
        {
            const string methodName = "GenerateCSAgentResetCode";
            // generate member's single-use code
            string singleUseCode = Guid.NewGuid().ToString("N").Substring(26, 6); // Get the last 6 digits of the 32-character Guid
            _logger.Trace(_className, methodName, "singleUseCode: " + singleUseCode);
            agent.ResetCode = singleUseCode;
            agent.ResetCodeDate = DateTime.Now.AddMinutes(expiryMinutes);
            UpdateCSAgent(agent);
            return singleUseCode;
        }

		public CSAgent GetCSAgentById(long id)
		{
            return CSAgentDao.Retrieve(id);
		}

		public CSAgent GetCSAgentByUserName(string userName, AgentAccountStatus? status)
		{
            return CSAgentDao.Retrieve(userName, status);
		}

		public CSAgent GetCSAgentByResetCode(string resetCode)
		{
            return CSAgentDao.RetrieveByResetCode(resetCode);
		}

		public List<CSAgent> GetCSAgents(string firstName, string lastName, string email, CSRole role, string phoneNumber, AgentAccountStatus? status)
		{
            long? roleId = null;
            if (role != null)
            {
                roleId = role.Id;
            }

            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToUpper();
            }

            List<CSAgent> agents = CSAgentDao.Retrieve(firstName, lastName, email, roleId, phoneNumber, status);
            if (agents == null)
            {
                agents = new List<CSAgent>();
            }
            return agents;
		}

		public List<CSAgent> GetAllCSAgents(CSRole role)
		{
            List<CSAgent> agents = null;
            if (role != null)
            {
                agents = CSAgentDao.Retrieve(null, null, null, role.Id, null, null);
            }
            else
            {
                agents = CSAgentDao.RetrieveAll();
            }

            if (agents == null)
            {
                agents = new List<CSAgent>();
            }
            return agents;
		}


		public void CreateNote(CSNote note)
		{
			CSNoteDao.Create(note);
		}

		public void UpdateNote(CSNote note)
		{
			CSNoteDao.Update(note);
		}

		public CSNote GetNote(long id)
		{
			return CSNoteDao.Retrieve(id);
		}

		public List<CSNote> GetNotesByMember(long ipcode, DateTime startDate, DateTime endDate)
		{
            List<CSNote> notes = CSNoteDao.RetrieveByMember(ipcode, startDate, endDate);
            if (notes == null)
            {
                notes = new List<CSNote>();
            }
            return notes;
		}

		public List<CSNote> GetAllNotes()
		{
            List<CSNote> notes = CSNoteDao.RetrieveAll();
            if (notes == null)
            {
                notes = new List<CSNote>();
            }
            return notes;
		}

		public void DeleteNote(long id)
		{
			CSNoteDao.Delete(id);
		}


		public void CreateRole(CSRole role)
		{
			CSRoleDao.Create(role);
		}

		public void UpdateRole(CSRole role)
		{
			CSRoleDao.Update(role);
		}

		public CSRole GetRole(long roleId, bool deep)
		{
            CSRole role = CSRoleDao.Retrieve(roleId);
            if (deep && role != null)
            {
                // retrieve its functions as well.
                role.Functions = CSRoleDao.RetrieveFunctions(roleId);
                if (role.Functions == null)
                {
                    role.Functions = new List<CSFunction>();
                }
            }
            return role;
		}

		public CSRole GetRole(string roleName, bool deep)
		{
            CSRole role = CSRoleDao.Retrieve(roleName);
            if (deep && role != null)
            {
                // retrieve its functions as well.
                role.Functions = CSRoleDao.RetrieveFunctions(role.Id);
                if (role.Functions == null)
                {
                    role.Functions = new List<CSFunction>();
                }
            }
            return role;
		}

		public List<CSRole> GetAllRoles(bool deep)
		{
            List<CSRole> roles = CSRoleDao.RetrieveAll();
            if (roles == null)
            {
                roles = new List<CSRole>();
            }
            if (deep)
            {
                foreach (CSRole role in roles)
                {
                    role.Functions = CSRoleDao.RetrieveFunctions(role.Id);
                }
            }
            return roles;
		}

		public void DeleteRole(long roleId)
		{
			CSRoleFunctionDao.DeleteByRole(roleId);
			CSRoleDao.Delete(roleId);
		}

		public CSRole AddFunctionToRole(CSRole role, CSFunction func)
		{
            string methodName = "AddFunctionToRole";

            List<CSRoleFunction> rfList = CSRoleFunctionDao.RetrieveByRoleAndFunction(role.Id, func.Id);
            if (rfList == null || rfList.Count == 0)
            {
                CSRoleFunction rf = new CSRoleFunction();
                rf.FunctionId = func.Id;
                rf.RoleId = role.Id;
                CSRoleFunctionDao.Create(rf);
                return GetRole(role.Id, true);
            }
            else
            {
                string errMsg = string.Format("Roel {0} already has function {1}.", role.Name, func.Name);
                _logger.Error(_className, methodName, errMsg);
                throw new LWException(errMsg);
            }
		}

		public void RemoveFunctionFromRole(CSRole role, CSFunction func)
		{
            string methodName = "RemoveFunctionFromRole";

            IList<CSRoleFunction> rfList = CSRoleFunctionDao.RetrieveByRoleAndFunction(role.Id, func.Id);
            if (rfList != null)
            {
                foreach (CSRoleFunction rf in rfList)
                {
                    CSRoleFunctionDao.Delete(rf.Id);
                }
            }
            else
            {
                _logger.Debug(_className, methodName, string.Format("Role {0} does not have function {1}.", role.Name, func.Name));
            }
		}

		public void CreateFunction(CSFunction func)
		{
			CSFunctionDao.Create(func);
		}

		public void UpdateFunction(CSFunction func)
		{
			CSFunctionDao.Update(func);
		}

		public CSFunction GetFunction(long funcId)
		{
			return CSFunctionDao.Retrieve(funcId);
		}

		public List<CSFunction> GetAllFunctions()
		{
            List<CSFunction> functions = CSFunctionDao.RetrieveAll();
            if (functions == null)
            {
                functions = new List<CSFunction>();
            }
            return functions;
		}

		public void DeleteFunction(long funcId)
		{
            List<CSRoleFunction> usedList = CSRoleFunctionDao.RetrieveByFunction(funcId);
            if (usedList == null || usedList.Count == 0)
            {
                CSFunctionDao.Delete(funcId);
            }
		}

		/// <summary>
		/// This method looks for LWCSPermissionHookClass and LWCSPermissionHookAssembly in the web.config
		/// file.  If these values are found then the class is instantiated and invoked.
		/// </summary>
		/// <returns></returns>
		public ICSPermissionCallback GetPermissionCallback()
		{
            string methodName = "GetPermissionCallback";
            if (_cb == null)
            {
                string className = System.Configuration.ConfigurationManager.AppSettings["LWCSPermissionHookClass"];
                string assemblyName = System.Configuration.ConfigurationManager.AppSettings["LWCSPermissionHookAssembly"];
                _logger.Debug(_className, methodName, "LWCSPermissionHookClass = " + className);
                _logger.Debug(_className, methodName, "LWCSPermissionHookAssembly = " + assemblyName);
                if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(assemblyName))
                {
                    _logger.Debug(_className, methodName, "Instantiating Permission hook.");
                    try
                    {
                        this._cb = (ICSPermissionCallback)ClassLoaderUtil.CreateInstance(assemblyName, className);
                        if (_cb == null)
                        {
                            _logger.Error(_className, methodName, "Unable to instantiate permission hook.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName, "Error instantiating permission hook.", ex);
                    }
                }
            }
            return _cb;
		}

		public void CreateContactHistory(ContactHistory entity)
		{
            ContactHistoryDao.Create(entity);
		}

		public void CreateContactHistories(IList<ContactHistory> entities)
		{
            ContactHistoryDao.Create(entities);
		}

		public void UpdateContactHistory(ContactHistory entity)
		{
            ContactHistoryDao.Update(entity);
		}

		public void UpdateContactHistories(IEnumerable<ContactHistory> entities)
		{
            ContactHistoryDao.Update(entities);
		}

		public ContactHistory GetContactHistory(long ID)
		{
            return ContactHistoryDao.Retrieve(ID);
		}

		public ContactHistory GetContactHistoryByCDWKey(long cdwKey)
		{
            return ContactHistoryDao.RetrieveByCDWKey(cdwKey);
		}

		public List<ContactHistory> GetAllContactHistory(long IPCode)
		{
            return ContactHistoryDao.RetrieveAll(IPCode) ?? new List<ContactHistory>();
		}

		public List<ContactHistory> GetAllContactHistoryInDateRange(long IPCode, DateTime fromDate, DateTime toDate)
		{
            return ContactHistoryDao.RetrieveAllInDateRange(IPCode, fromDate, toDate) ?? new List<ContactHistory>();
		}

		public void DeleteContactHistory(long ID)
		{
            ContactHistoryDao.Delete(ID);
		}

		public ContactStatusMap GetContactStatusMap()
		{
            ContactStatusMap result = (ContactStatusMap)CacheManager.Get(CONTACTSTATUS_LIST, "All");
            if (result == null)
            {
                List<ContactStatus> entities = ContactStatusDao.RetrieveAll();
                if (entities != null && entities.Count > 0)
                {
                    foreach (ContactStatus entity in entities)
                    {
                        result.Add(entity.ID, entity.CSDescr);
                    }
                }
                else
                {
                    result = new ContactStatusMap();
                }
                CacheManager.Update(CONTACTSTATUS_LIST, "All", result);
            }
            return result;
		}
	}
}
