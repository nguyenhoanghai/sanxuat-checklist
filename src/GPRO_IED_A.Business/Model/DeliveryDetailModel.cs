using SanXuatCheckList.Data;
using System;

namespace SanXuatCheckList.Business.Model
{
    public class DeliveryDetailModel :  DeliveryDetail
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string LotName { get; set; }
        public int LotIndex { get; set; }
        public string MaterialName { get; set; }

        public string MaterialCode { get; set; }
        public int MaterialIndex { get; set; }
        public string WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public int WareHouseIndex { get; set; }
        public double QuantityUsed { get; set; }
        public double QuantityLo { get; set; }
        public DateTime InputDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        //public string UnitName { get; set; }

        //use for export to excel
        public string DeliveryName { get; set; }
        public string DeliveryCode { get; set; }
        public int DeliveryIndex { get; set; }
        public string CustomerName { get; set; }
        public int? ApprovedUser { get; set; }
        public string ApprovedUserName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ReceiverName { get; set; }

        public int MoneyTypeId { get; set; }
        public string MoneyTypeName { get; set; }

        public double Total { get; set; }
    }
}
