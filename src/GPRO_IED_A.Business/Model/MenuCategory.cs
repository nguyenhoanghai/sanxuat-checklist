using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanXuatCheckList.Business.Model
{
    public class MenuCategory
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Position { get; set; }
        public int OrderIndex { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsViewIcon { get; set; }
        public string Link { get; set; }
        public List<Menu> ListMenu { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public MenuCategory()
        {
            ListMenu = new List<Menu>();
        }
    }
}
