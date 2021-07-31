using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
    public class BigAlertModel
    {
        public int Unread { get; set; }
        public List<ChecklistJobAlertModel> Alerts { get; set; }
        public BigAlertModel()
        {
            Alerts = new List<ChecklistJobAlertModel>();
        }
    }
}
