using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class Commo_Ana_Phase_ManiModel 
    {
        public int Id { get; set; }
        public int CA_PhaseId { get; set; }
        public int OrderIndex { get; set; }
        public int? ManipulationId { get; set; }
        public string ManipulationCode { get; set; }
        public string ManipulationName { get; set; }
        public double? TMUEquipment { get; set; }
        public double? TMUManipulation { get; set; }
        public double Loop { get; set; }
        public double TotalTMU { get; set; } 
        public string EquipmentName { get; set; }      
    }
}
