using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class ControlModel :  TemplateControl
    {
        public int proFileId { get; set; }
        public int TemplateControlId { get; set; }
        public int ActionUser { get; set; }
    }
}
