﻿using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class LotSuppliesModel : LotSupplies
    {
        public int ActionUser { get; set; }
        public int WareHouseId { get; set; }
        public int MaterialIndex { get; set; }
        public int WareHouseIndex { get; set; }
        public string Code { get; set; }
        public string strWarehouse  { get; set; }
        public string strMaterial  { get; set; }
        public string strCustomer { get; set; }
         
        public string strMaterialUnit { get; set; }

        public int MoneyTypeId { get; set; }
        public string strMoneyType { get; set; }
        public double ExchangeRate { get; set; }

        public DateTime InputDate { get; set; }
        public int StatusId { get; set; }
        public string strStatus { get; set; }
        public int MaterialUnitId { get; set; }
    }
}
