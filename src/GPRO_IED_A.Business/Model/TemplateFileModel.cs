using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class TemplateFileModel :  TemplateFile
    {
        public int ActionUser { get; set; }
        public string Code { get; set; }
        public string ApproveUserName { get; set; }
        public List<ControlModel> Controls { get; set; }
        public TemplateFileModel()
        {
            Controls = new List<ControlModel>();
        }
    }
}
