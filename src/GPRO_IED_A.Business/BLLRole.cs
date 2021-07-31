
using GPRO.Core.Mvc;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLRole
    {
        #region constructor 
        static object key = new object();
        private static volatile BLLRole _Instance;
        public static BLLRole Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLRole();

                return _Instance;
            }
        }
        private BLLRole() { }
        #endregion

        public SRoLe CheckName(int Id, string Name, int companyId, SanXuatCheckListEntities db)
        {
            return (from x in db.SRoLe where !x.IsDeleted && x.CompanyId == companyId && x.Id != Id && x.RoleName.Trim().ToUpper().Equals(Name.Trim().ToUpper()) select x).FirstOrDefault();
        }

        public ResponseBase Create(SRoLe role, List<string> listRolePermission)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ResponseBase result = null;
                SRolePermission rolePermission;
                try
                {
                    result = new ResponseBase();
                    if (role != null)
                    {
                        if (CheckName(role.Id, role.RoleName, role.CompanyId, db) != null)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create Role", Message = "Tên Nhóm Quyền này đã tồn tại.\nVui lòng chọn lại Tên khác." });
                        }
                        else
                        {
                            db.SRoLe.Add(role);
                            if (listRolePermission != null && listRolePermission.Count > 0)
                            {
                                // add role permission
                                foreach (var item in listRolePermission)
                                {
                                    var permissionArray = item.Split('|');
                                    rolePermission = new SRolePermission();
                                    rolePermission.RoleId = role.Id;
                                    rolePermission.ModuleId = int.Parse(permissionArray[0]);
                                    rolePermission.FeatureId = int.Parse(permissionArray[1]);
                                    rolePermission.PermissionId = int.Parse(permissionArray[2]);
                                    rolePermission.CreatedUser = role.CreatedUser;
                                    rolePermission.CreatedDate = DateTime.Now;
                                    db.SRolePermission.Add(rolePermission);
                                }
                            }
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create Role", Message = "Không thể tạo được Nhóm Quyền. Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
        }

        public ResponseBase Update(SRoLe role, List<string> listRolePermission)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ResponseBase result = null;
                SRolePermission rolePermission;
                List<SRolePermission> oldRolePermissionList;
                SRoLe oldRole;
                try
                {
                    result = new ResponseBase();
                    if (role != null)
                    {
                        if (CheckName(role.Id, role.RoleName, role.CompanyId, db) != null)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create Role", Message = "Tên Nhóm Quyền này đã tồn tại.\nVui lòng chọn lại Tên khác." });
                        }
                        else
                        {
                            oldRole = db.SRoLe.FirstOrDefault(x => x.Id == role.Id && !x.IsDeleted && !x.SCompany.IsDeleted);
                            if (oldRole != null)
                            {
                                oldRole.RoleName = role.RoleName;
                                oldRole.Description = role.Description;
                                oldRole.UpdatedUser = oldRole.CreatedUser;
                                oldRole.UpdatedDate = DateTime.Now;
                                db.Entry<SRoLe>(oldRole).State = System.Data.Entity.EntityState.Modified;

                                // get all old role permission of role
                                oldRolePermissionList = (from x
                                                         in db.SRolePermission
                                                         where x.RoleId == oldRole.Id && !x.IsDeleted
                                                         select x).ToList();

                                // remove all of old data
                                if (oldRolePermissionList != null && oldRolePermissionList.Count > 0)
                                {
                                    foreach (var roleItem in oldRolePermissionList)
                                    {
                                        roleItem.IsDeleted = true;
                                        roleItem.DeletedUser = oldRole.UpdatedUser;
                                        roleItem.DeletedDate = DateTime.Now;
                                        db.Entry<SRolePermission>(roleItem).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }
                                //add new data
                                if (listRolePermission != null && listRolePermission.Count > 0)
                                {
                                    #region
                                    // add role permission
                                    foreach (var item in listRolePermission)
                                    {
                                        var permissionArray = item.Split('|');
                                        rolePermission = new SRolePermission();
                                        rolePermission.RoleId = oldRole.Id;
                                        rolePermission.ModuleId = int.Parse(permissionArray[0]);
                                        rolePermission.FeatureId = int.Parse(permissionArray[1]);
                                        rolePermission.PermissionId = int.Parse(permissionArray[2]);
                                        rolePermission.CreatedUser = oldRole.CreatedUser;
                                        rolePermission.CreatedDate = DateTime.Now;
                                        db.SRolePermission.Add(rolePermission);
                                    }
                                    #endregion
                                }
                                db.SaveChanges();
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update Role", Message = "Nhóm Quyền đang thao tác không tồn tại. Vui Lòng kiểm tra lại." });
                            }
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create Role", Message = "Không thể tạo được Nhóm Quyền. Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
        }

        public ResponseBase DeleteById(int id, int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ResponseBase result = null;
                SRoLe role = null;
                try
                {
                    result = new ResponseBase();
                    role = (from x in db.SRoLe
                            where x.Id == id && !x.IsDeleted
                            select x).FirstOrDefault();
                    if (role != null)
                    {
                        role.IsDeleted = true;
                        role.DeletedUser = userId;
                        role.DeletedDate = DateTime.Now;
                        db.Entry<SRoLe>(role).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete Role", Message = "Phân Quyền đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
        }

        public List<ModelSelectItem> GetRolesNotSystem(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<ModelSelectItem> roles = null;
                try
                {
                    roles = (from x in db.SRoLe
                             where x.CompanyId == companyId && !x.IsDeleted && !x.IsSystem
                             select new ModelSelectItem()
                             {
                                 Name = x.RoleName,
                                 Value = x.Id
                             }).ToList();
                    return roles;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public ResponseBase DeleteByListId(List<int> listId, int userId)
        {
            throw new NotImplementedException();
        }

        public PagedList<ModelRole> GetListRole(string keyWord, int startIndexRecord, int pageSize, string sorting, int userId, int companyId, bool IsOwner)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<SRoLe> roles = null;
                try
                {
                    roles = new List<SRoLe>();
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    if (IsOwner)
                    {
                        roles = (from x in db.SRoLe
                                 where x.CompanyId == companyId && !x.IsDeleted
                                 orderby (sorting)
                                 select x).ToList();
                    }
                    else
                    {
                        roles = (from x in db.SRoLe
                                 where x.CompanyId == companyId && !x.IsDeleted && x.CreatedUser == userId
                                 orderby (sorting)
                                 select x).ToList();
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1; 
                    return new PagedList<ModelRole>(roles.Select(x => new ModelRole()
                    {
                        Id = x.Id,
                        RoleName = x.RoleName,
                        Description = x.Description,
                        IsSystem = x.IsSystem
                    }), pageNumber, pageSize);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<ModelFeature> GetListFeatureAndPermissionByModuleId(int moduleId, int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<int> listUserRolesIdByUserId = null;
                List<ModelFeature> ListFeatureByListRoleIdAndModuleId = null;
                try
                {
                    listUserRolesIdByUserId = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    if (listUserRolesIdByUserId != null && listUserRolesIdByUserId.Count > 0)
                    {
                        ListFeatureByListRoleIdAndModuleId = BLLRolePermission.Instance.GetListFeatureByModuleIdOfRoles(listUserRolesIdByUserId, moduleId, db);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ListFeatureByListRoleIdAndModuleId;
            }
        }

        #region Get list Module - Feature - Permission
        public List<ModelModule> GetListModuleByUserId(int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<int> listUserRolesIdByUserId = null;
                List<ModelModule> ListModuleByListRoleId = null;
                try
                {
                    listUserRolesIdByUserId = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    ListModuleByListRoleId = BLLRolePermission.Instance.GetListModuleByListRoleId(listUserRolesIdByUserId, db);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (ListModuleByListRoleId != null && ListModuleByListRoleId.Count > 0)
                    return ListModuleByListRoleId.OrderBy(c => c.OrderIndex).ToList();
                return new List<ModelModule>();
            }
        }

        public List<ModelFeature> GetListFeatureByUserId(int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<int> listUserRolesIdByUserId = null;
                List<ModelFeature> ListFeatureByListRoleId = null;
                try
                {
                    listUserRolesIdByUserId = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    if (listUserRolesIdByUserId != null && listUserRolesIdByUserId.Count > 0)
                    {
                        ListFeatureByListRoleId = BLLRolePermission.Instance.GetListFeatureByListRoleId(listUserRolesIdByUserId, db);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ListFeatureByListRoleId;
            }
        }

        public List<ModelPermission> GetListPermissionByUserId(int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<int> listUserRolesIdByUserId = null;
                List<ModelPermission> ListPermissionByListRoleId = null;
                try
                {
                    listUserRolesIdByUserId = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    if (listUserRolesIdByUserId != null && listUserRolesIdByUserId.Count > 0)
                    {
                        ListPermissionByListRoleId = BLLRolePermission.Instance.GetListPermissionByListRoleId(listUserRolesIdByUserId, db);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ListPermissionByListRoleId;
            }
        }
        #endregion


        public ModelRole GetRoleDetailByRoleId(int roleId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ModelRole role;
                try
                {
                    role = (from x in db.SRoLe
                            where x.Id == roleId && !x.IsDeleted && !x.SCompany.IsDeleted
                            select new ModelRole()
                            {
                                Id = x.Id,
                                CompanyId = x.CompanyId,
                                RoleName = x.RoleName,
                                Description = x.Description
                            }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return role;
            }
        }

        public List<ModelRole> GetListRoleByIsSystem()
        {
            using (var db = new SanXuatCheckListEntities())
            {
                return (from c in db.SRoLe
                        where !c.IsDeleted && c.IsSystem
                        select new ModelRole()
                        {
                            CompanyId = c.CompanyId,
                            Id = c.Id,
                            RoleName = c.RoleName,
                        }).ToList();
            }
        }

        public ModelRole GetRoleByCompanyId(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                return (from c in db.SRoLe
                        where !c.IsDeleted && c.IsSystem && c.CompanyId == companyId
                        select new ModelRole()
                        {
                            Id = c.Id,
                            RoleName = c.RoleName,
                        }).FirstOrDefault();
            }
        }

        public List<ModelRolePermission> GetListRolePermissionByRoleId(int roleId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                return (from x in db.SRolePermission
                        where x.RoleId == roleId && !x.IsDeleted && !x.SRoLe.IsDeleted && !x.SModule.IsDeleted && !x.SPermission.IsDeleted
                        select new ModelRolePermission()
                        {
                            Id = x.Id,
                            RoleId = x.RoleId,
                            ModuleId = x.ModuleId,
                            FeatureId = x.FeatureId,
                            PermissionId = x.PermissionId
                        }).ToList();
            }
        }


        public List<int> GetListRoleIdByCompanyId(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                return (from x in db.SRoLe
                        where x.CompanyId == companyId && x.IsSystem && !x.IsDeleted
                        select x.Id).ToList();
            }
        }


        public List<SRoLe> GetListRoleByCompanyId(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                return (from x in db.SRoLe
                        where x.CompanyId == companyId && !x.IsDeleted
                        select x).ToList();
            }
        }
    }
}
