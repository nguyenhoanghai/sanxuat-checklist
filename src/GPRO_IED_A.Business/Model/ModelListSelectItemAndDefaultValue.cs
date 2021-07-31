using System;
using System.Collections.Generic;
using System.Linq; 

namespace SanXuatCheckList.Business.Model
{
    public class ModelListSelectItemAndDefaultValue
    {
        public int defaultValue { get; set; }
        public List<ModelSelectItem> listModelSelectItem { get; set; }
        public ModelListSelectItemAndDefaultValue()
        {
            defaultValue  = 0;
            listModelSelectItem = new List<ModelSelectItem>();
        }
    }
}
