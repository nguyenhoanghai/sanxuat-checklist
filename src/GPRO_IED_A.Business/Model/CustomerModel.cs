using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class CustomerModel :  Customer
    {  
        public int ActionUser { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        public bool IsPrivate { get; set; }
    }
}
