using System;
using System.Collections.Generic;
using System.Linq; 
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
    public class ModelEquipment  
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Expend { get; set; }
        public string Description { get; set; }
        public int EquipmentTypeId { get; set; }
        public int EquipmentGroupId { get; set; }
        public int CompanyId { get; set; }
        public string EquipmentTypeName { get; set; }
        public int? EquipTypeDefaultId { get; set; }
        public List<T_EquipmentAttribute> listEquipmentAtt { get; set; }
        public int QuantityUse { get; set; }
        public string Info { get; set; }
        public string EGroupName { get; set; }

        public List<ModelAtribute> EquipAtts { get; set; }
        public int ActionUser { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}

