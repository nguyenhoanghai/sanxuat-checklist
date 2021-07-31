using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace SanXuatCheckList.Business.Model
{
    public class LineModel  
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountOfLabours { get; set; }
        public int WorkShopId { get; set; }
        public string WorkShopName { get; set; }
        public int ActionUser { get; set; }
    }
}
