using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLModule
    {
        #region constructor 
        static object key = new object();
        private static volatile BLLModule _Instance;
        public static BLLModule Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLModule();

                return _Instance;
            }
        }
        private BLLModule() { }
        #endregion
         
        public int GetModuleIdBySystemName(string systemName, SanXuatCheckListEntities db)
        {
            int moduleId = 0;
            try
            {
                var module = db.SModule.FirstOrDefault(c => c.SystemName.Trim().ToUpper().Equals(systemName.Trim().ToUpper()) && !c.IsDeleted);
                if (module != null)
                    moduleId = module.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return moduleId;
        }

        public List<ModelSelectItem> GetSelectListModuleByCompanyId(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                try
                {
                    var selectItems = new List<ModelSelectItem>();
                    selectItems.Add(new ModelSelectItem() { Value = 0, Name = " Không có Dữ Liệu " });
                    var roleIds = BLLRole.Instance.GetListRoleIdByCompanyId(companyId);
                    var moduleIds = BLLRolePermission.Instance.GetListModuleIdByListRoleId(roleIds, db);
                    if (moduleIds != null || moduleIds.Count > 0)
                    {
                        var modules = (from x in db.SModule
                                       where !x.IsDeleted && moduleIds.Contains(x.Id) && x.IsShow
                                       select new ModelSelectItem() { Name = x.ModuleName, Value = x.Id }).ToList();
                        if (modules != null && modules.Count > 0)
                        {
                            selectItems[0].Name = " - - Chọn Hệ Thống - - ";
                            selectItems.AddRange(modules);
                        }
                    }
                    return selectItems;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
         
    }
}
