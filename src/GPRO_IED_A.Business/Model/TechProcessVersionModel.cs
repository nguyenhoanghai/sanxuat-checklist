using System;
using System.Collections.Generic;
using System.Linq; 
using SanXuatCheckList.Data; 


namespace SanXuatCheckList.Business.Model
{
    public class TechProcessVersionModel  
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ParentId { get; set; }
        public double TimeCompletePerCommo { get; set; }
        public int NumberOfWorkers { get; set; }
        public double WorkingTimePerDay { get; set; }
        public double PacedProduction { get; set; }
        public double ProOfGroupPerHour { get; set; }
        public double ProOfGroupPerDay { get; set; }
        public double ProOfPersonPerDay { get; set; }
        public double PricePerSecond { get; set; }
        public double Allowance { get; set; }
        public string Note { get; set; }
        public int Quantities { get; set; }
        public double Price { get; set; } 
        public string ProductName { get; set; }
        public string WorkShopName { get; set; }        
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public int ActionUser { get; set; }
        public List<TechProcessVerDetailModel> details { get; set; }
        public List<ModelEquipment> equipments { get; set; }
        public TechProcessVersionModel()
        {
            details = new List<TechProcessVerDetailModel>();
            equipments = new List<ModelEquipment>();
        }
    }
}
