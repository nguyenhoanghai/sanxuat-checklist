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
    
    public partial class T_EquipType_Default
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EquipType_Default()
        {
            this.T_EquipmentType = new HashSet<T_EquipmentType>();
            this.T_EquipTypeAttr_Default = new HashSet<T_EquipTypeAttr_Default>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EquipmentType> T_EquipmentType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EquipTypeAttr_Default> T_EquipTypeAttr_Default { get; set; }
    }
}