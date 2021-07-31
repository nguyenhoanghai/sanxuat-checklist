using System;
using System.Collections.Generic;
using System.Linq; 
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Business.Model
{
   public class PhaseGroupModel  
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double MaxLevel { get; set; }
        public double MinLevel { get; set; }
        public string ProductTypeName { get; set; }
        public int ActionUser { get; set; }
        public string WorkshopIds { get; set; }
        public string WorkshopNames { get; set; }
        public List<int> intWorkshopIds { get; set; }

    }
}
