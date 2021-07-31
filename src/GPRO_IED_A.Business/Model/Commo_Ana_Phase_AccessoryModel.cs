using System;
using System.Collections.Generic;
using System.Linq; 
using GPRO_IED_A.Data;

namespace GPRO_IED_A.Business.Model
{
  public  class Commo_Ana_Phase_AccessoryModel : T_CA_Phase_Accessory
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string EquipmentTypeName { get; set; }
        public string Description { get; set; }
        
    }
}
