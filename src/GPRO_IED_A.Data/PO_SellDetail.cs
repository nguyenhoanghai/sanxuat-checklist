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
    
    public partial class PO_SellDetail
    {
        public int Id { get; set; }
        public int POId { get; set; }
        public int ProductId { get; set; }
        public int Quantities { get; set; }
        public double Price { get; set; }
        public string Note { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUser { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<int> DeletedUser { get; set; }
    
        public virtual PO_Sell PO_Sell { get; set; }
        public virtual Product Product { get; set; }
    }
}
