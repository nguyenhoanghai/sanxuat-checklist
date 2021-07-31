using System;
using System.Collections.Generic;
using System.Linq; 
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
   public class TechProcessVerDetailModel 
    {
        public int Id { get; set; }
        public int TechProcessVersionId { get; set; }
        public int CA_PhaseId { get; set; }
        public double StandardTMU { get; set; }
        public double Percent { get; set; }
        public double TimeByPercent { get; set; }
        public double Worker { get; set; }
        public string Description { get; set; }
        public int PhaseGroupId { get; set; }
        public string PhaseCode { get; set; }
        public string PhaseName { get; set; }
        public int? EquipmentId { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentGroupCode { get; set; }
        public double TotalTMU { get; set; }
        public double De_Percent { get; set; }
        public int WorkerLevelId { get; set; }
        public string WorkerLevelName { get; set; }
        public double Coefficient { get; set; }
        public double TimePrepare { get; set; }
       public int Index { get; set; }
    }
}
