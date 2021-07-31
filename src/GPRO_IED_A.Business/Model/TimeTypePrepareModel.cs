using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace SanXuatCheckList.Business.Model
{
   public class TimeTypePrepareModel  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsPublic { get; set; }
        public string Description { get; set; }
        public string WorkShopName { get; set; }
        public int ActionUser { get; set; }
    }
}
