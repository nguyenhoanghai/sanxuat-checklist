using GPRO_IED_A.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPRO_IED_A.Business.Model
{
   public class Commo_Ana_PhaseVer_DetailModel : T_CA_PhaseVers_Detail
    { 
        public string Name { get; set; }
        public string Code { get; set; }
        public int WorkerLevelId { get; set; }
        public string   WorkerLevelName { get; set; }
        public double TotalTMU { get; set; }
        public string Description { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentCode { get; set; } 
    }
}
