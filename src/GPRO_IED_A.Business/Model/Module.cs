using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanXuatCheckList.Business.Model
{
    public class Module
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string ModuleName { get; set; }
        public bool IsSystem { get; set; }
        public int OrderIndex { get; set; }
        public string Description { get; set; }
        public string ModuleUrl { get; set; }
    }
}
