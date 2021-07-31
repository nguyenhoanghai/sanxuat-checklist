using System.Collections.Generic;

namespace SanXuatCheckList.Business.Model
{
    public class RequestModel
    {
        public List<Checklist_JobModel> Alarms { get; set; }
        public List<Checklist_JobModel> Refer { get; set; }
        public RequestModel()
        {
            Alarms = new List<Checklist_JobModel>();
            Refer = new List<Checklist_JobModel>();
        }
    }
}
