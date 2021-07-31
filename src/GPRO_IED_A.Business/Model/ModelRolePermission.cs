using System;
using System.Collections.Generic;
using System.Linq;  

namespace SanXuatCheckList.Business.Model
{
    public class ModelRolePermission
    { 
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public int FeatureId { get; set; }
        public int PermissionId { get; set; }
        public string Description { get; set; }
    }
}
