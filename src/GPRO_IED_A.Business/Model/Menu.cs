using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanXuatCheckList.Business.Model
{
   public class Menu
    {
        public string MenuName { get; set; }
        public int OrderIndex { get; set; }
        public string Link { get; set; }
        public bool IsShow { get; set; }
        public bool IsViewIcon { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
