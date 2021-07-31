using System;
using System.Collections.Generic; 
using System.Linq; 
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
    public class UserModel  
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PassWord { get; set; }
        public bool IsLock { get; set; }
        public bool IsRequireChangePW { get; set; }
        public bool IsForgotPassword { get; set; }
        public string NoteForgotPassword { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public Nullable<System.DateTime> LockedTime { get; set; }
        public string LastName { get; set; }
        public string FisrtName { get; set; }
         
        public List<UserRoleModel> UserRoles { get; set; }
        public string OldPassWord { get; set; }
        public int Status { get; set; }
        public bool ChangePic { get; set; }
        public List<int> UserRoleIds { get; set; }
        public string RoleNames { get; set; }
        public int ActionUser { get; set; }

        public string WorkshopIds { get; set; }
        public string WorkshopNames { get; set; }
        public List<int> intWorkshopIds { get; set; }
    }
}
