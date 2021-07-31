using System;
using System.Collections.Generic;
namespace GPRO.Core.Interface
{
	public interface IUserService
	{
		string UserName
		{
			get;
			set;
		}
		object State
		{
			get;
			set;
		}
		int  CompanyId
		{
			get;
			set;
		}
		string CompanyName
		{
			get;
			set;
		}
		string Description
		{
			get;
			set;
		}
		string Email
		{
			get;
			set;
		}
		string ImagePath
		{
			get;
			set;
		}
		bool IsOwner
		{
			get;
			set;
		}
		int UserID
		{
			get;
			set;
		}
		string EmployeeName
		{
			get;
			set;
		}
		string[] Permissions
		{
			get;
			set;
		}
		int[] Features
		{
			get;
			set;
		}
		string LogoCompany
		{
			get;
			set;
		}
		int StoreID
		{
			get;
			set;
		}
		string SesssionId
		{
			get;
			set;
		}
		string DepartmentName
		{
			get;
			set;
		}
		List<IPermissionService> PermissionServices
		{
			get;
			set;
		}
		List<IModule> ListModule
		{
			get;
			set;
		}
		List<IMenuCategory> ListMenu
		{
			get;
			set;
		}
		int[] ChildCompanyId
		{
			get;
			set;
		}

        int[] WorkshopIds
        {
            get;
            set;
        }
    }
}
