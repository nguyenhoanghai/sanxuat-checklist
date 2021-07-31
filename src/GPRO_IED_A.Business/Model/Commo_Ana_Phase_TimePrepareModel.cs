using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
   public class Commo_Ana_Phase_TimePrepareModel  
    {
        public int Id { get; set; }
        public int Commo_Ana_PhaseId { get; set; }
        public int TimePrepareId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string TimeTypePrepareName { get; set; }
        public string Description { get; set; }
        public double TMUNumber { get; set; }
    }
}
