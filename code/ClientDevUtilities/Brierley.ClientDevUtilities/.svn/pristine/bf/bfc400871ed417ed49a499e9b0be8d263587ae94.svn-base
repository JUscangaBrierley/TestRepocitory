using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Dmc.Exceptions;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Dmc
{
	public class DmcService : IDisposable
	{
		private string _url;
		private string _login;
		private string _password;
		private HttpClient _client;
		private ICommunicationLogger _logger = null;

		private HttpClient Client
		{
			get
			{
				if (_client == null)
				{
					_client = new HttpClient();
					_client.BaseAddress = new Uri(_url);
					// Add an Accept header for JSON format.
					_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var encoding = new ASCIIEncoding();
					string authHeader = string.Format("{0}:{1}", _login, _password);
					_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(encoding.GetBytes(authHeader)));
				}
				return _client;
			}
		}

		public DmcService(ICommunicationLogger logger)
		{
			string url = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Dmc.Constants.DmcUrl);
			string login = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Dmc.Constants.DmcUsername);
			string password = LWConfigurationUtil.GetConfigurationValue(Brierley.FrameWork.Dmc.Constants.DmcPassword);
			if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
			{
				throw new Exception(
					string.Format(
					"DMC setup has not been completed. Please have an administrator complete setup by providing framework settings for {0}, {1} and {2}",
					Brierley.FrameWork.Dmc.Constants.DmcUrl,
					Brierley.FrameWork.Dmc.Constants.DmcUsername,
					Brierley.FrameWork.Dmc.Constants.DmcPassword));
			}
			_logger = logger;
			Init(url, login, password);
		}

		public DmcService(string url, string login, string password, ICommunicationLogger logger)
		{
			_logger = logger;
			Init(url, login, password);
		}

		/// <summary>
		/// Returns the names of all personalizations (fields) used by the specified message.
		/// </summary>
		/// <param name="messageId">The unique identifier of the message to retrieve personalizations for.</param>
		/// <returns></returns>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.NoSuchObjectException">Thrown when the message id is not found</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.InvalidParameterException">Thrown when an input parameter is in an invalid format</exception>
		/// /// <exception cref="Brierley.FrameWork.Dmc.Exceptions.UnexpectedErrorException">Can be thrown if DMC encounters an unexpected exception</exception>
		public async Task<IEnumerable<string>> GetPersonalizationsAsync(long messageId)
		{
			return GetPersonalizations(messageId);
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public IEnumerable<string> GetPersonalizations(long messageId)
		{
			const string path = "message/getUsedPersonalizations?messageId=";
			IEnumerable<string> ret = null;
			HttpResponseMessage response = null;
			try
			{
				response = Client.GetAsync(path + messageId.ToString()).Result;
				string result = response.Content.ReadAsStringAsync().Result;

				if (response.IsSuccessStatusCode)
				{
					ret = JsonConvert.DeserializeObject<List<string>>(result);
				}
				else
				{
					if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
					{
						//error from DMC
						Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
						throw ex;
					}
					else
					{
						ThrowFailedDmcCall(response);
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetPersonalizations, response, path) { MessageId = messageId });
				}
				return ret ?? new List<string>();
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetPersonalizations, response, path, ex) { MessageId = messageId }, out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		/// <summary>
		/// Retrieves a user from the DMC system.
		/// </summary>
		/// <param name="email">Email address of the user to retrieve.</param>
		/// <returns></returns>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.NoSuchObjectException">Thrown when an the user does not exist.</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.UnexpectedErrorException">Can be thrown if DMC encounters an unexpected exception</exception>
		public async Task<User> GetUserAsync(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}
			email = Uri.EscapeDataString(email);

			const string path = "user/getByEmail?email=";
			User ret = null;
			HttpResponseMessage response = null;
			try
			{
				response = await Client.GetAsync(path + email);
				string result = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					ret = JsonConvert.DeserializeObject<User>(result);
				}
				else
				{
					if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
					{
						//error from DMC
						Exception ex = DmcException.GetException(await response.Content.ReadAsStringAsync());
						//NoSuchObjectException is safe to ignore. We'll just return a null user (the API really should have returned a 404).
						if (!(ex is NoSuchObjectException))
						{
							throw ex;
						}
					}
					else
					{
						ThrowFailedDmcCall(response);
					}
				}

				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path));
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path, ex), out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public User GetUser(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}
			email = Uri.EscapeDataString(email);

			const string path = "user/getByEmail?email=";
			User ret = null;
			HttpResponseMessage response = null;
			try
			{
				response = Client.GetAsync(path + email).Result;
				string result = response.Content.ReadAsStringAsync().Result;
				if (response.IsSuccessStatusCode)
				{
					ret = JsonConvert.DeserializeObject<User>(result);
				}
				else
				{
					if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
					{
						//error from DMC
						Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
						//NoSuchObjectException is safe to ignore. We'll just return a null user (the API really should have returned a 404).
						if (!(ex is NoSuchObjectException))
						{
							throw ex;
						}
					}
					else
					{
						ThrowFailedDmcCall(response);
					}
				}

				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path));
				}
				return ret;
			}
			catch (Exception ex)
			{
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path, ex));
				}
				throw;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		/// <summary>
		/// Retrieves a user from the DMC system.
		/// </summary>
		/// <param name="email">Email address of the user to retrieve.</param>
		/// <returns></returns>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.NoSuchObjectException">Thrown when an the user does not exist.</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.UnexpectedErrorException">Can be thrown if DMC encounters an unexpected exception</exception>
		public async Task<User> GetUserByMobilePhoneAsync(string phone)
		{
			if (string.IsNullOrEmpty(phone))
			{
				throw new ArgumentNullException("phone");
			}
			phone = Uri.EscapeDataString(phone);

			const string path = "user/getByMobileNumber?mobileNumber=";
			User ret = null;

			HttpResponseMessage response = null;
			try
			{
				response = await Client.GetAsync(path + phone);
				string result = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					ret = JsonConvert.DeserializeObject<User>(result);
				}
				else
				{
					if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
					{
						//error from DMC
						Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
						//NoSuchObjectException is safe to ignore. We'll just return a null user (the API really should have returned a 404).
						if (!(ex is NoSuchObjectException))
						{
							throw ex;
						}
					}
					else
					{
						ThrowFailedDmcCall(response);
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path));
				}
				return ret;
			}
			catch (Exception ex)
			{
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path, ex));
				}
				throw;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public User GetUserByMobilePhone(string phone)
		{
			if (string.IsNullOrEmpty(phone))
			{
				throw new ArgumentNullException("phone");
			}
			phone = Uri.EscapeDataString(phone);

			const string path = "user/getByMobileNumber?mobileNumber=";
			User ret = null;

			HttpResponseMessage response = null;
			try
			{
				response = Client.GetAsync(path + phone).Result;
				string result = response.Content.ReadAsStringAsync().Result;
				if (response.IsSuccessStatusCode)
				{
					ret = JsonConvert.DeserializeObject<User>(result);
				}
				else
				{
					if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
					{
						//error from DMC
						Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
						//NoSuchObjectException is safe to ignore. We'll just return a null user (the API really should have returned a 404).
						if (!(ex is NoSuchObjectException))
						{
							throw ex;
						}
					}
					else
					{
						ThrowFailedDmcCall(response);
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path));
				}
				return ret;
			}
			catch (Exception ex)
			{
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcGetUser, response, path, ex));
				}
				throw;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		/// <summary>
		/// Creates a new user in the DMC system.
		/// </summary>
		/// <param name="email">The user's email address. This is required.</param>
		/// <param name="mobileNumber">The user's mobile number</param>
		/// <param name="attributes">Additional attributes to set for the user.</param>
		/// <returns>The newly created user</returns>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.InvalidParameterException">Thrown when an input parameter is in an invalid format</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.ObjectAlreadyExistsException">Thrown when a user already exists with the same email address or mobile number</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.UnexpectedErrorException">Can be thrown if DMC encounters an unexpected exception</exception>
		public async Task<User> CreateUserAsync(string email, string mobileNumber = null, List<Attribute> attributes = null)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}
			email = Uri.EscapeDataString(email);

			if (mobileNumber != null)
			{
				mobileNumber = Uri.EscapeDataString(mobileNumber);
			}

			const string path = "user/create?email={0}&mobileNumber={1}";
			User ret = null;

			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(attributes ?? new List<Attribute>());
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = await Client.PostAsync(string.Format(path, email, mobileNumber), body);
					string result = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						ret = JsonConvert.DeserializeObject<User>(result);
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, requestBody) { User = ret ?? new User() { MobileNumber = mobileNumber, Email = email }, Attributes = attributes });
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, requestBody, ex) { User = ret ?? new User() { MobileNumber = mobileNumber, Email = email }, Attributes = attributes }, out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public User CreateUser(string email, string mobileNumber = null, List<Attribute> attributes = null)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}
			email = Uri.EscapeDataString(email);

			if (mobileNumber != null)
			{
				mobileNumber = Uri.EscapeDataString(mobileNumber);
			}

			const string path = "user/create?email={0}&mobileNumber={1}";
			User ret = null;

			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(attributes ?? new List<Attribute>());
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = Client.PostAsync(string.Format(path, email, mobileNumber), body).Result;
					string result = response.Content.ReadAsStringAsync().Result;

					if (response.IsSuccessStatusCode)
					{
						ret = JsonConvert.DeserializeObject<User>(result);
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, requestBody) { User = ret ?? new User() { MobileNumber = mobileNumber, Email = email }, Attributes = attributes });
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, requestBody, ex) { User = ret ?? new User() { MobileNumber = mobileNumber, Email = email }, Attributes = attributes }, out queued);
				}
				if(!queued)
				{
				throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		public async Task<User> CreateUserAsync(string mobileNumber, List<Attribute> attributes = null)
		{
			if (string.IsNullOrEmpty(mobileNumber))
			{
				throw new ArgumentNullException("mobileNumber");
			}
			mobileNumber = Uri.EscapeDataString(mobileNumber);

			const string path = "user/create?mobileNumber={0}";
			User ret = null;
			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(attributes ?? new List<Attribute>());
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = await Client.PostAsync(string.Format(path, mobileNumber), body);
					string result = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						ret = JsonConvert.DeserializeObject<User>(result);
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(await response.Content.ReadAsStringAsync());
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, requestBody) { User = ret });
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, ex), out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public User CreateUser(string mobileNumber, List<Attribute> attributes = null)
		{
			if (string.IsNullOrEmpty(mobileNumber))
			{
				throw new ArgumentNullException("mobileNumber");
			}
			mobileNumber = Uri.EscapeDataString(mobileNumber);

			const string path = "user/create?mobileNumber={0}";
			User ret = null;
			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(attributes ?? new List<Attribute>());
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = Client.PostAsync(string.Format(path, mobileNumber), body).Result;
					string result = response.Content.ReadAsStringAsync().Result;
					if (response.IsSuccessStatusCode)
					{
						ret = JsonConvert.DeserializeObject<User>(result);
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}
				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, requestBody) { User = ret });
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcCreateUser, response, path, ex), out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		/// <summary>
		/// Updates a user profile in the DMC system by mobile number.
		/// </summary>
		/// <param name="mobileNumber">The user's mobile number</param>
		/// <param name="attributes">Additional attributes to set for the user.</param>
		/// <returns>The newly created user</returns>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.InvalidParameterException">Thrown when an input parameter is in an invalid format</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.ObjectAlreadyExistsException">Thrown when a user already exists with the same email address or mobile number</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.UnexpectedErrorException">Can be thrown if DMC encounters an unexpected exception</exception>
		public async Task<User> UpdateProfileByMobileNumberAsync(string mobileNumber = null, List<Attribute> attributes = null)
		{
			if (string.IsNullOrEmpty(mobileNumber))
			{
				throw new ArgumentNullException("mobileNumber");
			}
			mobileNumber = Uri.EscapeDataString(mobileNumber);

			string path = string.Format("user/updateProfileByMobileNumber?&mobileNumber={0}", mobileNumber);
			User ret = null;

			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(attributes ?? new List<Attribute>());
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = await Client.PostAsync(path, body);

					string result = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						ret = JsonConvert.DeserializeObject<User>(result);
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(await response.Content.ReadAsStringAsync());
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}

				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcUpdateProfileByMobileNumber, response, path, requestBody) { User = new User() { MobileNumber = mobileNumber }, Attributes = attributes });
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcUpdateProfileByMobileNumber, response, path, requestBody, ex) { User = new User() { MobileNumber = mobileNumber }, Attributes = attributes }, out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public User UpdateProfileByMobileNumber(string mobileNumber = null, List<Attribute> attributes = null)
		{
			if (string.IsNullOrEmpty(mobileNumber))
			{
				throw new ArgumentNullException("mobileNumber");
			}
			mobileNumber = Uri.EscapeDataString(mobileNumber);

			string path = string.Format("user/updateProfileByMobileNumber?&mobileNumber={0}", mobileNumber);
			User ret = null;

			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(attributes ?? new List<Attribute>());
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = Client.PostAsync(path, body).Result;

					string result = response.Content.ReadAsStringAsync().Result;

					if (response.IsSuccessStatusCode)
					{
						ret = JsonConvert.DeserializeObject<User>(result);
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}

				}
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcUpdateProfileByMobileNumber, response, path, requestBody) { User = new User() { MobileNumber = mobileNumber }, Attributes = attributes });
				}
				return ret;
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcUpdateProfileByMobileNumber, response, path, requestBody, ex) { User = new User() { MobileNumber = mobileNumber }, Attributes = attributes }, out queued);
				}
				if (!queued)
				{
					throw;
				}
				return null;
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}


		/// <summary>
		/// Sends a message through the DMC API.
		/// </summary>
		/// <param name="messageId">The Id of the message in the DMC system.</param>
		/// <param name="userId">The Id of the recipient in the DMC system.</param>
		/// <param name="personalizations">A dictionary of email personalizations and their values.</param>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.InvalidParameterException">Thrown when an input parameter is in an invalid format</exception>
		/// <exception cref="Brierley.FrameWork.Dmc.Exceptions.NoSuchObjectException">Thrown when the message id is not found</exception>
		/// /// <exception cref="Brierley.FrameWork.Dmc.Exceptions.UnexpectedErrorException">Can be thrown if DMC encounters an unexpected exception</exception>
		public async Task SendSingleAsync(long messageId, User user, Dictionary<string, string> personalizations)
		{
			const string path = "message/sendSingle?messageId={0}&recipientId={1}";

			MessageContent content = new MessageContent();
			content.Personalizations = new List<Attribute>();
			content.Attachments = null;

			if (personalizations != null && personalizations.Count > 0)
			{
				foreach (var key in personalizations.Keys)
				{
					content.Personalizations.Add(new Attribute(key, personalizations[key]));
				}
			}

			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(content);
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = await Client.PostAsync(string.Format(path, messageId, user.Id), body);

					if (!response.IsSuccessStatusCode)
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(await response.Content.ReadAsStringAsync());
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}
					if (_logger != null)
					{
						_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcSendSingle, response, path, requestBody) { MessageId = messageId, User = user, Personalizations = personalizations });
					}
				}
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcSendSingle, response, path, requestBody, ex) { MessageId = messageId, User = user, Personalizations = personalizations }, out queued);
				}
				if (!queued)
				{
					throw;
				}
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		[Obsolete("Use the Async method instead. This method will be removed in a future release.")]
		public void SendSingle(long messageId, User user, Dictionary<string, string> personalizations)
		{
			const string path = "message/sendSingle?messageId={0}&recipientId={1}";

			MessageContent content = new MessageContent();
			content.Personalizations = new List<Attribute>();
			content.Attachments = null;

			if (personalizations != null && personalizations.Count > 0)
			{
				foreach (var key in personalizations.Keys)
				{
					content.Personalizations.Add(new Attribute(key, personalizations[key]));
				}
			}

			HttpResponseMessage response = null;
			string requestBody = JsonConvert.SerializeObject(content);
			try
			{
				using (HttpContent body = new StringContent(requestBody))
				{
					body.Headers.ContentType.MediaType = "application/json";
					response = Client.PostAsync(string.Format(path, messageId, user.Id), body).Result;

					if (!response.IsSuccessStatusCode)
					{
						if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
						{
							//error from DMC
							Exception ex = DmcException.GetException(response.Content.ReadAsStringAsync().Result);
							throw ex;
						}
						else
						{
							ThrowFailedDmcCall(response);
						}
					}
					if (_logger != null)
					{
						_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcSendSingle, response, path, requestBody) { MessageId = messageId, User = user, Personalizations = personalizations });
					}
				}
			}
			catch (Exception ex)
			{
				bool queued = false;
				if (_logger != null)
				{
					_logger.LogMessage(new CommunicationLogData(CommunicationType.DmcSendSingle, response, path, requestBody, ex) { MessageId = messageId, User = user, Personalizations = personalizations }, out queued);
				}
				if (!queued)
				{
					throw;
				}
			}
			finally
			{
				if (response != null)
				{
					response.Dispose();
				}
			}
		}

		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
				_client = null;
			}
		}

		private void Init(string url, string login, string password)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ArgumentNullException("url");
			}
			if (string.IsNullOrEmpty(login))
			{
				throw new ArgumentNullException("login");
			}
			if (string.IsNullOrEmpty(login))
			{
				throw new ArgumentNullException("password");
			}

			_url = url.Trim();
			if (!_url.EndsWith("/"))
			{
				_url += "/";
			}

			_login = login;
			_password = password;
		}

		//implement as part of immediate retry policy - need to put a policy in place first, though... got ahead of myself
		//const int _retryAttempts = 1;
		//private HttpResponseMessage Get(string uri, int attempts = 0)
		//{
		//	try
		//	{
		//		HttpResponseMessage response = Client.GetAsync(path + messageId.ToString()).Result;
		//	}
		//	catch (Exception ex)
		//	{
		//		if (attempts < _retryAttempts)
		//		{
		//			return Get(uri, ++attempts);
		//		}
		//	}
		//}

		private void ThrowFailedDmcCall(HttpResponseMessage response)
		{
			string content = string.Empty;
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}

			try
			{
				if (response.Content != null)
				{
					content = response.Content.ReadAsStringAsync().Result;
				}
			}
			catch
			{
			}

			if (string.IsNullOrEmpty(content))
			{
				content = "empty";
			}

			//encode the response - if we're in a web contenxt - to avoid malicious injection into our application.
			if (System.Web.HttpContext.Current != null)
			{
				content = System.Web.HttpContext.Current.Server.HtmlEncode(content);
			}

			throw new Exception(string.Format("DMC API call has failed with status code {0}. Response body received was {1}.", response.StatusCode, content));
		}

		private DmcResult GetFailureType(Exception ex)
		{
			if (ex is System.Net.WebException)
			{
				return DmcResult.ConnectionFailure;
			}
			if (ex.InnerException != null)
			{
				return GetFailureType(ex.InnerException);
			}
			return DmcResult.Unknown;
		}
	}
}
