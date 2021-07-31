using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace SanXuatCheckList.Business.Model
{
    public class LinePositionDetailModel  
    {
        public int Id { get; set; }
        public int Line_PositionId { get; set; }
        public int TechProVerDe_Id { get; set; }
        public int OrderIndex { get; set; }
        public double DevisionPercent { get; set; }
        public double NumberOfLabor { get; set; }
        public string Note { get; set; }
        public bool IsPass { get; set; }
        public string PhaseCode { get; set; }
        public string PhaseName { get; set; }
        public double TotalLabor { get; set; }
        public double TotalTMU { get; set; }
        public double SkillRequired { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public int PhaseGroupId { get; set; }
        public double DevisionPercent_Temp { get; set; }
        public int Index { get; set; }
    }
}
