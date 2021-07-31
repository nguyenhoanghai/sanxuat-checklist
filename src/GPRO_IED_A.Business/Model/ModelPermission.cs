using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanXuatCheckList.Business.Model
{
    public class ModelPermission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public int FeatureId { get; set; }
        public string SystemName { get; set; }
        public string Url { get; set; }
        public bool IsDefault { get; set; }
    }
}
