using System;
using System.Collections.Generic;
using System.Linq; 
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
    public class ProductModel : Product
    { 
        public string SizeName { get; set; }  
        public string UnitName { get; set; }
        public string CustomerName { get; set; }
        public bool IsPrivate { get; set; }
        public int ActionUser { get; set; }
    }
}
