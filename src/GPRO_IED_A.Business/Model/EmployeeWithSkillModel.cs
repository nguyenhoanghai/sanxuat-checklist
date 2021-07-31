using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class EmployeeWithSkillModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public List<Employee_PhaseGroupSkillModel> PhaseGroupSkills { get; set; }
        public string LastName { get; set; }
    }
}
