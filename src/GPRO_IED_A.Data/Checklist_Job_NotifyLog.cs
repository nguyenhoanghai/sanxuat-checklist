//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SanXuatCheckList.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Checklist_Job_NotifyLog
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsNotifited { get; set; }
        public bool IsDisabled { get; set; }
    
        public virtual Checklist_Job Checklist_Job { get; set; }
    }
}
