﻿using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
    public class ChecklistJobAlertModel : Checklist_Job_Alert
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string MainContent { get; set; }
        public string[] ReceiverIds { get; set; }
        public ChecklistJobAlertModel()
        {
            ReceiverIds = new string[] { };
        }
    }
}
