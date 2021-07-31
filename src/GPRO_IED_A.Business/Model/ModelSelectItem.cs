using System;
using System.Collections.Generic;
using System.Linq; 

namespace SanXuatCheckList.Business.Model
{
    public class ModelSelectItem
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public int Data { get; set; }
        public string Code { get; set; }
        public double Double { get; set; }
        public bool IsDefault { get; set; }
        public dynamic Data1 { get; set; }
        public dynamic Data2 { get; set; }
        public dynamic Data3 { get; set; }
        public dynamic Data4 { get; set; }
    }
}
