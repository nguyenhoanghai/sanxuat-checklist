using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class UnitModel :Unit
    {
        public int ActionUser { get; set; }
        public string TypeName { get; set; }
    }
}
