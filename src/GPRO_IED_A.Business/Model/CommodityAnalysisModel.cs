using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business.Model
{
    public class CommodityAnalysisModel
    {
        public List<ProAnaModel> CommoAna { get; set; }
        public List<int> years { get; set; }
        public CommodityAnalysisModel()
        {
            CommoAna = new List<ProAnaModel>();
            years = new List<int>();
        }
    }

    public class ProAnaModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ObjectType { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        public string Node { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
