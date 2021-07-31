using System;
namespace GPRO.Core.Security
{
	public interface IAuthenticationService
	{
		void Login(string token);
		void Logout();
	}
}
