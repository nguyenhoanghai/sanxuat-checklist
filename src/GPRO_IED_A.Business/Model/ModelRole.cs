 using System.Collections.Generic; 

namespace SanXuatCheckList.Business.Model
{
    public class ModelRole 
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public string[] Permission { get; set; }
        public bool IsPermisionChange { get; set; }
        public List<ModelRolePermission> RolePer_Model { get; set; }
    }
}
