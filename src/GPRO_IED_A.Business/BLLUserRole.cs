using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business
{
    public class BLLUserRole
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLUserRole _Instance;
        public static BLLUserRole Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLUserRole();

                return _Instance;
            }
        }
        private BLLUserRole() { }
        #endregion

        public List<ModelSelectItem> GetUserRolesModelByUserId(int userId, bool IsOwner, int companyId)
        {
            List<ModelSelectItem> roles = null;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (IsOwner)
                    {
                        roles = db.SRoLe.Where(x => !x.IsDeleted && x.CompanyId == companyId).Select(x => new ModelSelectItem()
                        {
                            Name = x.RoleName,
                            Value = x.Id
                        }).ToList();
                    }
                    else
                    {
                        roles = db.SUserRole.Where(x => !x.IsDeleted && x.UserId == userId).Select(x => new ModelSelectItem()
                        {
                            Name = x.SRoLe.RoleName,
                            Value = x.SRoLe.Id
                        }).ToList();
                    } 
                    return roles;
                } 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> GetUserRolesIdByUserId(int userId, SanXuatCheckListEntities db)
        { 
            try
            {
              return (from x in db.SUserRole where !x.IsDeleted && x.UserId == userId && !x.SRoLe.SCompany.IsDeleted select x.RoleId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

    }
}
