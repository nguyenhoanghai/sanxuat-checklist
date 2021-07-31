using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business.Model
{
    public class Commo_Ana_PhaseModel  
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? EquipmentId { get; set; }
        public int WorkerLevelId { get; set; }
        public int PhaseGroupId { get; set; }
        public int ParentId { get; set; }
        public double TotalTMU { get; set; }
        public int? ApplyPressuresId { get; set; }
        public double PercentWasteEquipment { get; set; }
        public double PercentWasteManipulation { get; set; }
        public double PercentWasteSpecial { get; set; }
        public double PercentWasteMaterial { get; set; }
        public string Node { get; set; }
        public string Video { get; set; }
        public bool IsTimePrepareChange { get; set; }
        public bool IsAccessoryChange { get; set; } 
        public List<Commo_Ana_Phase_TimePrepareModel> timePrepares { get; set; }
        public double ManiVerTMU { get; set; }
        public double TimePrepareTMU { get; set; }
        public string WorkerLevelName { get; set; }
        public bool HasChild { get; set; }
        public string EquipName { get; set; }
        public string EquipDes { get; set; }
        public int EquipTypeDefaultId { get; set; }
        public int ActionUser { get; set; }
        public bool IsLibrary { get; set; }

        public List<Commo_Ana_Phase_ManiModel> actions { get; set; }
        public Commo_Ana_PhaseModel()
        {
            timePrepares = new List<Commo_Ana_Phase_TimePrepareModel>();
            actions = new List<Commo_Ana_Phase_ManiModel>();
        }
    }
}
