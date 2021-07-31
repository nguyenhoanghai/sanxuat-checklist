using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business.Model
{
    public class UserRoleModel : SUserRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string Decription { get; set; }
        public string RoleName { get; set; }
    }
}
