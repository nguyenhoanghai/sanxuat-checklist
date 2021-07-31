using System;
namespace GPRO.Core.Interface
{
	public interface IMembershipService
	{
		IUserService GetUserService(int userId);
		IPermissionService[] GetPermissionService(string featureName);
	}
}
