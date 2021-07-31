using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class TemplateChecklistJobStepModel : Template_CL_JobStep
    {
        public int ActionUser { get; set; }
    }
}
