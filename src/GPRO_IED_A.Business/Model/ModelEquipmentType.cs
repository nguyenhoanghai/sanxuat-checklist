using System;
using System.Collections.Generic;
using System.Linq; 
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
    public class ModelEquipmentType  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? EquipTypeDefaultId { get; set; }
        public int? ObjectType { get; set; }
        public int CompanyId { get; set; }
        public int ActionUser { get; set; }
    }
}