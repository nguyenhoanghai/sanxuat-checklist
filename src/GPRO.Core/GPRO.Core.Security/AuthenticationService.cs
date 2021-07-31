using System;
namespace GPRO.Core.Security
{
	public class AuthenticationService : IAuthenticationService
	{
		public void Login(string token)
		{
			int userId = Convert.ToInt32(token);
			Authentication.Login(userId);
		}
		public void Logout()
		{
			Authentication.Logout();
		}
	}
}
