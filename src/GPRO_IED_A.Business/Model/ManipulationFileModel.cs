using System;
using System.Collections.Generic;
using System.Linq; 
using GPRO_IED_A.Data;

namespace GPRO_IED_A.Business.Model
{
    class ManipulationFileModel : T_ManipulationFile
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
    }
}
