using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanXuatCheckList.Business.Model
{
    public class UserService
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string EmployeeName { get; set; }
        public int[] Features { get; set; }
        public string ImagePath { get; set; }
        public bool IsOwner { get; set; }
        public string LogoCompany { get; set; }
        public string[] Permissions { get; set; }
        public int UserID { get; set; }
        public int[] ChildCompanyId { get; set; }
        public string WorkshopId  { get; set; }
        public int[] intWorkshopIds { get; set; }
        public string UserName { get; set; }
        public List<Module> ListModule { get; set; }
        public List<MenuCategory> ListMenu { get; set; }
        public UserService()
        {
            ListModule = new List<Module>();
            ListMenu = new List<MenuCategory>();
        }
    }
}
