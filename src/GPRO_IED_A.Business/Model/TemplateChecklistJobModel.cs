using SanXuatCheckList.Data;
using System.Collections.Generic;

namespace SanXuatCheckList.Business.Model
{
    public class TemplateChecklistJobModel : Template_CL_Job
    {
        public int ActionUser { get; set; }
        public List<TemplateChecklistJobModel> SubItems { get; set; }
        public TemplateChecklistJobModel()
        {
            SubItems = new List<TemplateChecklistJobModel>();
        }
    }
}
