using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
    public class WorkerLevelModel  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Coefficient { get; set; }
        public string Note { get; set; }
        public int? CompanyId { get; set; }
        public bool IsPrivate { get; set; }
        public int ActionUser { get; set; }
    }
}
