using GPRO_IED_A.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPRO_IED_A.Business.Model
{
    public class Commo_Ana_Phase_ManiVersionModel : T_CA_Phase_ManiVersion
    {
        public List<Commo_Ana_Phase_ManiVer_DetailModel> Details { get; set; }
        public bool IsDetailChange { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentDes { get; set; }
        public int EquipTypeDefaultId { get; set; }

        public double TimePrepare { get; set; }
    }
}