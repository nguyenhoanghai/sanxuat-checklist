using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace SanXuatCheckList.Business.Model
{
    public class ManipulationLibraryModel  
    {
        public int Id { get; set; }
        public int ManipulationTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double StandardTMU { get; set; }
        public double UserTMU { get; set; }
        public int? EquipmentTypeId { get; set; }
        public int? StopPrecisionId { get; set; }
        public int? ApplyPressureId { get; set; }
        public int? NatureCutsId { get; set; }
        public  double? Distance { get; set; }
        public string EquipmentName { get; set; }
        public string  ManiTypeCode { get; set; }
        public string ManipulationTypeName { get; set; }

        public bool isListAttachFileChange { get; set; }
        public bool isListMachineTMUChange { get; set; }
        public bool isUseMachine { get; set; }
        public int ActionUser { get; set; }
    }
}
