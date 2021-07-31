 using System;
using System.Collections.Generic; 

namespace SanXuatCheckList.Business.Model
{
    public  class CategoryModel  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int OrderIndex { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsViewIcon { get; set; }
        public string Link { get; set; }
        public int ModuleId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public int CreatedUser { get; set; } 
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedUser { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool isHidden { get; set; }
        public bool isDefault { get; set; }
        public bool isConfigExits { get; set; }
        public string ModuleName { get; set; }
        public List<MenuModel> listMenu { get; set; }
    }
}
