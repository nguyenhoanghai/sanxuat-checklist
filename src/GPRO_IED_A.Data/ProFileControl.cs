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
    
    public partial class ProFileControl
    {
        public int Id { get; set; }
        public int ProductFileId { get; set; }
        public string ControlName { get; set; }
        public int ControlType { get; set; }
        public bool Checked { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ProductionFile ProductionFile { get; set; }
    }
}
