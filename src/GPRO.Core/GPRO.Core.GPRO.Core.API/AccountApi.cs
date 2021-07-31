using System;
using System.Collections.Generic;
using System.Net.Http;
using SystemAccount.Bussiness.Interface.Model;
using SystemAccount.Object;
namespace GPRO.Core.GPRO.Core.API
{
	public class AccountApi
	{
		private static HttpClient AccountClient;
		private static object key = new object();
		private static volatile AccountApi _Instance;
		public static AccountApi Instance
		{
			get
			{
				if (AccountApi._Instance == null)
				{
					lock (AccountApi.key)
					{
						AccountApi._Instance = new AccountApi();
						AccountApi.AccountClient = new HttpClient();
					}
				}
				return AccountApi._Instance;
			}
		}
		private AccountApi()
		{
		}
		public SMoneyType GetMoneyType(string url, int id)
		{
			SMoneyType result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/moneytypeapi/GetById?id=" + id).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<SMoneyType>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public ModelListSelectItemAndDefaultValue GetMoneyTypes(string url, int companyId)
		{
			ModelListSelectItemAndDefaultValue result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/moneytypeapi/GetSelect?companyId=" + companyId).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<ModelListSelectItemAndDefaultValue>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public List<int> FindMoneyTypeIds(string url, int[] moneyTypeIds)
		{
			List<int> result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string text = "";
				for (int i = 0; i < moneyTypeIds.Length; i++)
				{
					text = text + "existsUserIds=" + moneyTypeIds[i];
					if (i < moneyTypeIds.Length - 1)
					{
						text += "&";
					}
				}
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/moneytypeapi/FindFromIds?" + text).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<List<int>>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public List<SMoneyType> GetMoneyTypes(string url)
		{
			List<SMoneyType> result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/moneytypeapi/GetMoneyTypes").Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<List<SMoneyType>>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public UserService GetUserInfo(string url, int userId, string moduleName)
		{
			UserService result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"userId=",
					userId,
					"&&moduleName=",
					moduleName
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/GetUserInfo?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<UserService>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public SUser GetUserById(string url, int companyId, int userId)
		{
			SUser result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"companyId=",
					companyId,
					"&&userId=",
					userId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/GetById?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<SUser>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public void DeleteByEmail(string url, string email, int companyId, int acctionUserId)
		{
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"email=",
					email,
					"&&companyId=",
					companyId,
					"&&acctionUserId=",
					acctionUserId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/DeleteByEmail?" + str).Result;
				result.EnsureSuccessStatusCode();
			}
			catch (Exception)
			{
			}
		}
		public bool DeleteById(string url, int userId, int companyId, int acctionUserId)
		{
			bool result3;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"userId=",
					userId,
					"&&companyId=",
					companyId,
					"&&acctionUserId=",
					acctionUserId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/DeleteById?" + str).Result;
				result.EnsureSuccessStatusCode();
				ResponseBase result2 = HttpContentExtensions.ReadAsAsync<ResponseBase>(result.Content).Result;
				result3 = result2.get_IsSuccess();
				return result3;
			}
			catch (Exception)
			{
			}
			result3 = false;
			return result3;
		}
		public bool CheckExistsEmail(string url, string email, int companyId)
		{
			bool result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"email=",
					email,
					"&&companyId=",
					companyId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/CheckExistsEmail?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<bool>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = false;
			return result2;
		}
		public void Lock(string url, int userId, int companyId, int acctionUserId)
		{
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"userId=",
					userId,
					"&&companyId=",
					companyId,
					"&&acctionUserId=",
					acctionUserId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/Lock?" + str).Result;
				result.EnsureSuccessStatusCode();
			}
			catch (Exception)
			{
			}
		}
		public void UnLock(string url, int userId, int companyId, int acctionUserId)
		{
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"userId=",
					userId,
					"&&companyId=",
					companyId,
					"&&acctionUserId=",
					acctionUserId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/UnLock?" + str).Result;
				result.EnsureSuccessStatusCode();
			}
			catch (Exception)
			{
			}
		}
		public List<SUser> GetUsers(string url, int companyId)
		{
			List<SUser> result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = "companyId=" + companyId;
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/GetUsers?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<List<SUser>>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public string Add(string url, SUser user, int acctionUserId)
		{
			string result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = string.Concat(new object[]
				{
					"user=",
					user,
					"&&acctionUserId=",
					acctionUserId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/Add?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<string>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public SCompany GetCompany(string url, int userId)
		{
			SCompany result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = "userId=" + userId;
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/GetCompany?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<SCompany>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
		public bool CheckStatusLogin(string url, string toKen)
		{
			bool result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string str = "toKen=" + toKen;
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/CheckStatusLogin?" + str).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<bool>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = false;
			return result2;
		}
		public List<SUser> GetListUserWithoutExistsInList(string url, int[] existsUserIds, string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting, int companyId)
		{
			List<SUser> result2;
			try
			{
				AccountApi.AccountClient.BaseAddress = new Uri(url);
				string text = "";
				for (int i = 0; i < existsUserIds.Length; i++)
				{
					text = text + "existsUserIds=" + existsUserIds[i];
					if (i < existsUserIds.Length - 1)
					{
						text += "&";
					}
				}
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"existsUserIds=0&&keyWord=",
					keyWord,
					"&&searchBy=",
					searchBy,
					"&&startIndexRecord=",
					startIndexRecord,
					"&&pageSize=",
					pageSize,
					"&&sorting=",
					sorting,
					"&&companyId=",
					companyId
				});
				HttpResponseMessage result = AccountApi.AccountClient.GetAsync("api/AccountApi/GetListUserWithoutExistsInList?" + text).Result;
				result.EnsureSuccessStatusCode();
				result2 = HttpContentExtensions.ReadAsAsync<List<SUser>>(result.Content).Result;
				return result2;
			}
			catch (Exception)
			{
			}
			result2 = null;
			return result2;
		}
	}
}
