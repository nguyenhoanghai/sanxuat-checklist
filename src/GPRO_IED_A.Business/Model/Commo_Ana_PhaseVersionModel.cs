using GPRO_IED_A.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GPRO_IED_A.Business.Model
{
    public class Commo_Ana_PhaseVersionModel : T_CA_PhaseVersion
    {
        public List<Commo_Ana_PhaseVer_DetailModel> Details { get; set; }
        public bool isDetailsChange { get; set; }
    }
}
