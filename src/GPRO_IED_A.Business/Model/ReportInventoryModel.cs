using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class ReportInventoryDetailModel : LotSupplies
    {
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public int MaterialIndex { get; set; }
        public string MaterialName { get; set; }
        public string MaterialCode { get; set; }
        public int StoreWarehouseId { get; set; }
        public string StoreWareHouseName { get; set; }
        public string StoreWareHouseCode { get; set; }
        public int StoreWareHouseIndex { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string StatusName { get; set; }
        public int MoneyTypeId { get; set; }
        public string MoneyTypeName { get; set; }
        public double ExchangeRate { get; set; }
        public double TotalMoney { get; set; }
        public DateTime InputDate { get; set; }
    }

    public class ReportInventoryModel
    {
        public string MaterialName { get; set; }
        public string WarehouseName { get; set; }
        public List<ReportInventoryDetailModel> Details { get; set; }
        public ReportInventoryModel()
        {
            Details = new List<ReportInventoryDetailModel>();
        }
    }
}
