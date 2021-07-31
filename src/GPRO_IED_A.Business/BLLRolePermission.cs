using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using SanXuatCheckList.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SanXuatCheckList.Business
{
    public class BLLRolePermission
    {
        #region constructor 
        static object key = new object();
        private static volatile BLLRolePermission _Instance;
        public static BLLRolePermission Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLRolePermission();

                return _Instance;
            }
        }
        private BLLRolePermission() { }
        #endregion

        public List<string> GetListSystemNameAndUrlOfPermissionByListRoleId(List<int> listRoleId, SanXuatCheckListEntities db)
        {
            List<string> systemNameUrlPermission = null;
            try
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    var listPermission = GetListPermissionByListRoleId(listRoleId, db);
                    if (listPermission != null && listPermission.Count() > 0)
                    {
                        systemNameUrlPermission = new List<string>();
                        systemNameUrlPermission.AddRange(listPermission.Select(x => x.SystemName).ToList());
                        foreach (var item in listPermission)
                        {
                            if (item.Url != null)
                            {
                                systemNameUrlPermission.AddRange(item.Url.Split('|').ToList());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return systemNameUrlPermission;
        }

        public List<ModelPermission> GetListPermissionByListRoleId(List<int> listRoleId, SanXuatCheckListEntities db)
        {
            List<ModelPermission> listPermission = null;
            try
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    var permissionIds = (from x in db.SRolePermission
                                         where !x.IsDeleted && !x.SPermission.IsDeleted && !x.SPermission.SFeature.IsDeleted && !x.SPermission.SFeature.SModule.IsDeleted && (listRoleId.Contains(x.RoleId) || (x.SFeature.IsDefault || x.SPermission.IsDefault))
                                         select x.PermissionId).Distinct();
                    if (permissionIds != null)
                    {
                        listPermission = (from x in db.SPermission
                                          where permissionIds.Contains(x.Id)
                                          select new ModelPermission()
                                          {
                                              Id = x.Id,
                                              PermissionName = x.PermissionName,
                                              FeatureId = x.FeatureId,
                                              SystemName = x.SystemName,
                                              Url = x.Url,
                                              IsDefault = x.IsDefault
                                          }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return listPermission;
        }

        public List<string> GetListSystemNameAndUrlOfPermissionByListRoleId(List<int> listRoleId, int moduleId, SanXuatCheckListEntities db)
        {
            List<string> systemNameUrlPermission = null;
            try
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    var listPermission = GetListPermissionByListRoleId(listRoleId, moduleId, db);
                    if (listPermission != null && listPermission.Count() > 0)
                    {
                        systemNameUrlPermission = new List<string>();
                        systemNameUrlPermission.AddRange(listPermission.Select(x => x.SystemName).ToList());
                        foreach (var item in listPermission)
                        {
                            if (item.Url != null)
                            {
                                systemNameUrlPermission.AddRange(item.Url.Split('|').ToList());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return systemNameUrlPermission;
        }

        public List<ModelPermission> GetListPermissionByListRoleId(List<int> listRoleId, int moduleId, SanXuatCheckListEntities db)
        {
            List<ModelPermission> listPermission = null;
            try
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    var permissionIds = (from x
                                         in db.SRolePermission
                                         where !x.IsDeleted && !x.SPermission.IsDeleted && !x.SPermission.SFeature.IsDeleted && !x.SPermission.SFeature.SModule.IsDeleted && (listRoleId.Contains(x.RoleId) || (x.SFeature.IsDefault || x.SPermission.IsDefault)) && x.ModuleId == moduleId
                                         select x.PermissionId).Distinct();
                    var per = (from x in db.SRolePermission
                               where !x.IsDeleted && !x.SPermission.IsDeleted && !x.SPermission.SFeature.IsDeleted && !x.SPermission.SFeature.SModule.IsDeleted && listRoleId.Contains(x.RoleId) && x.ModuleId == moduleId
                               select x).ToList();

                    if (permissionIds != null)
                    {
                        listPermission = (from x in db.SPermission
                                          where permissionIds.Contains(x.Id)
                                          select new ModelPermission()
                                          {
                                              Id = x.Id,
                                              PermissionName = x.PermissionName,
                                              FeatureId = x.FeatureId,
                                              SystemName = x.SystemName,
                                              Url = x.Url,
                                              IsDefault = x.IsDefault

                                          }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listPermission;
        }

        public List<ModelModule> GetListModuleByListRoleId(List<int> listRoleId, SanXuatCheckListEntities db)
        {
            List<ModelModule> listModule = null;
            try
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    var moduleIds = (from x in db.SRolePermission
                                     where listRoleId.Contains(x.RoleId) && !x.IsDeleted && !x.SModule.IsDeleted && x.SModule.IsShow && x.SModule.IsSystem
                                     select x.ModuleId).Distinct();
                    if (moduleIds != null)
                    {
                        listModule = (from c in db.SModule
                                      where moduleIds.Contains(c.Id)
                                      select new ModelModule()
                                      {
                                          Id = c.Id,
                                          IsSystem = c.IsSystem,
                                          SystemName = c.SystemName,
                                          ModuleName = c.ModuleName,
                                          ModuleUrl = c.ModuleUrl,
                                          OrderIndex = c.OrderIndex,
                                          Description = c.Description
                                      }).OrderBy(x => x.OrderIndex).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listModule;
        }

        public List<ModelFeature> GetListFeatureByListRoleId(List<int> listRoleId, SanXuatCheckListEntities db)
        {
            List<ModelFeature> listFeature = null;
            try
            {
                if (listRoleId != null && listRoleId.Count > 0)
                {
                    var featureIds = (from x in db.SRolePermission where !x.IsDeleted && !x.SFeature.IsDeleted && !x.SFeature.SModule.IsDeleted && listRoleId.Contains(x.RoleId) select x.FeatureId).Distinct();
                    if (featureIds != null)
                    {
                        listFeature = (from c in db.SFeature
                                       where featureIds.Contains(c.Id)
                                       select new ModelFeature()
                                       {
                                           Id = c.Id,
                                           FeatureName = c.FeatureName,
                                           SystemName = c.SystemName,
                                           ModuleId = c.ModuleId,
                                           IsDefault = c.IsDefault
                                       }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return listFeature;
        }




        //private List<ModelPermission> GetListPermissionByFeatureIdOfRoles(List<int> listRoleId, int featureId)
        //{
        //    List<ModelPermission> listPermission = null;
        //    try
        //    {
        //        var listPermissionOfAllFeature = GetListPermissionByListRoleId(listRoleId);
        //        if (listPermissionOfAllFeature != null && listPermissionOfAllFeature.Count > 0)
        //        {
        //            listPermission = listPermissionOfAllFeature.Where(c => c.FeatureId == featureId).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    return listPermission;
        //}

        public List<ModelFeature> GetListFeatureByModuleIdOfRoles(List<int> listRoleId, int moduleId, SanXuatCheckListEntities db)
        {
            List<ModelFeature> listFeature = null;
            try
            {
                var listFeatureOfAllModule = GetListFeatureByListRoleId(listRoleId, db);
                if (listFeatureOfAllModule != null && listFeatureOfAllModule.Count() > 0)
                {
                    var listFeatureId = listFeatureOfAllModule.Where(c => c.ModuleId == moduleId).Select(c => c.Id).Distinct();
                    listFeature = listFeatureOfAllModule.Where(c => c.ModuleId == moduleId && listFeatureId.Contains(c.Id)).Select(c => new ModelFeature()
                    {
                        Id = c.Id,
                        FeatureName = c.FeatureName,
                        IsDefault = c.IsDefault
                    }).ToList();
                    if (listFeature.Count > 0)
                    {
                        var listPermissionOfAllRoles = GetListPermissionByListRoleId(listRoleId, db);
                        if (listPermissionOfAllRoles != null && listPermissionOfAllRoles.Count > 0)
                        {
                            foreach (var feature in listFeature)
                            {
                                feature.Permissions = listPermissionOfAllRoles.Where(c => c.FeatureId == feature.Id).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return listFeature;
        }

        public List<int> GetListModuleIdByListRoleId(List<int> RoleIds, SanXuatCheckListEntities db)
        {
            try
            {
                if (RoleIds != null || RoleIds.Count > 0)
                {
                    return (from x in db.SRolePermission where !x.IsDeleted && RoleIds.Contains(x.RoleId) select x.ModuleId).ToList();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<SRolePermission> GetRolesByCompanyId(int companyId)
        //{
        //    try
        //    {
        //        List<SRolePermission> rolePermissions = null;
        //        rolePermissions = repRolePermission.GetMany(x => !x.IsDeleted && x.SRoLe.CompanyId == companyId).ToList();
        //        return rolePermissions;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<string> GetSystemNameAndUrlOfPermissionBycompanyId(int companyId, SanXuatCheckListEntities db)
        {
            try
            {
                List<string> systemNameUrlPermission = null;
                var permissionIds = (from c
                                     in db.SRolePermission
                                     where c.SRoLe.IsSystem && c.SRoLe.CompanyId == companyId && !c.SRoLe.IsDeleted
                                     select c.PermissionId).ToList();
                if (permissionIds != null)
                {
                    var permissions = (from x
                                        in db.SPermission
                                       where permissionIds.Contains(x.Id)
                                       select new ModelPermission()
                                       {
                                           Id = x.Id,
                                           PermissionName = x.PermissionName,
                                           FeatureId = x.FeatureId,
                                           SystemName = x.SystemName,
                                           Url = x.Url,
                                           IsDefault = x.IsDefault
                                       }).ToList();
                    if (permissions.Count() > 0)
                    {
                        systemNameUrlPermission = new List<string>();
                        systemNameUrlPermission.AddRange(permissions.Select(x => x.SystemName).ToList());
                        foreach (var item in permissions)
                        {
                            if (item.Url != null)
                            {
                                systemNameUrlPermission.AddRange(item.Url.Split('|').ToList());
                            }
                        }
                    }
                }
                if (systemNameUrlPermission != null)
                    return systemNameUrlPermission;
                return new List<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool CheckIsUseSystemByCompanyIdAndSystemName(int companyId, string systemName)
        //{
        //    try
        //    {
        //        int moduleId = repModule.Get(x => !x.IsDeleted && x.SystemName.Trim().ToUpper().Equals(systemName.Trim().ToUpper())).Id;
        //        var rolePermissions = repRolePermission.GetMany(x => !x.IsDeleted && x.SRoLe.CompanyId == companyId && x.ModuleId == moduleId);
        //        if (rolePermissions != null && rolePermissions.Count() > 0)
        //            return true;
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
