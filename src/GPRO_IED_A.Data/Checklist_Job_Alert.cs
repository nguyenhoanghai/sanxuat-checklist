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
    
    public partial class Checklist_Job_Alert
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Alert { get; set; }
        public bool IsViewed { get; set; }
        public int UserSendId { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public int ObjectType { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Reciever { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual Checklist_Job Checklist_Job { get; set; }
    }
}
