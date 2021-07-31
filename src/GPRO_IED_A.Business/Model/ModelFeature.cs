 using System.Collections.Generic; 
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data; 

namespace SanXuatCheckList.Business.Model
{
    public class ModelFeature:SFeature
    {
        public List<ModelPermission> Permissions { get; set; }
        public List<ModelModule> Modules { get; set; }
    }
}
