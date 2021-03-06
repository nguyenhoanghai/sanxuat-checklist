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
    
    public partial class SUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUser()
        {
            this.Checklist_Job = new HashSet<Checklist_Job>();
            this.Checklist_JobStep = new HashSet<Checklist_JobStep>();
            this.Employee = new HashSet<Employee>();
            this.SUserRole = new HashSet<SUserRole>();
        }
    
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool IsLock { get; set; }
        public bool IsRequireChangePW { get; set; }
        public bool IsForgotPassword { get; set; }
        public string NoteForgotPassword { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public Nullable<System.DateTime> LockedTime { get; set; }
        public string LastName { get; set; }
        public string FisrtName { get; set; }
        public string WorkshopIds { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedUser { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedUser { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> DeletedUser { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Checklist_Job> Checklist_Job { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Checklist_JobStep> Checklist_JobStep { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual SCompany SCompany { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUserRole> SUserRole { get; set; }
    }
}
