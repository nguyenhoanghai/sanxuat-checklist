using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
    public class PO_SellModel : PO_Sell
    {
        public int ActionUser { get; set; }   
        public string CustomerName { get; set; }
        public string MoneyTypeName { get; set; }
        public string StatusName { get; set; }
        public double Total { get; set; }
        public List<PO_SellDetailModel> Details { get; set; }
        public PO_SellModel()
        {
            Details = new List<PO_SellDetailModel>();
        }
    }

    public class PO_SellDetailModel : PO_SellDetail
    {
        public int ActionUser { get; set; } 
        public string ProductSize { get; set; }
        public string ProductUnit { get; set; }
        public string ProductName { get; set; }
        public string ProductSpecifications { get; set; }
        public string ProductNote { get; set; } 
        public double Total { get; set; }
    }
}
