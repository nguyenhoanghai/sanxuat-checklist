using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace SanXuatCheckList.Business
{
    public class BLLMenu
    {
        #region constructor 
        static object key = new object();
        private static volatile BLLMenu _Instance;
        public static BLLMenu Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLMenu();

                return _Instance;
            }
        }
        private BLLMenu() { }
        #endregion


        private SMenu CheckMenuName(string menuName, int? menuId, SanXuatCheckListEntities db)
        {
            if (menuId == null)
            {
                return (from x in db.SMenu where !x.IsDeleted && x.MenuName.Trim().ToUpper().Equals(menuName.Trim().ToUpper()) select x).FirstOrDefault();
            }
            else
            {
                return (from x in db.SMenu where !x.IsDeleted && x.Id != menuId && x.MenuName.Trim().ToUpper().Equals(menuName.Trim().ToUpper()) select x).FirstOrDefault();
            }
        }

        public ResponseBase InsertOrUpdate(MenuModel model, int userId)
        {
            ResponseBase result = null;
            try
            {
                result = new ResponseBase();
                SMenu menu;
                using (var db = new SanXuatCheckListEntities())
                {
                    if (CheckMenuName(model.MenuName,model.Id, db) != null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { Message = "Tên Menu đã tồn tại. Vui lòng chọn lại tên khác!", MemberName = "Thêm Mới Menu" });
                    }
                    else
                    {
                        model.MenuName = model.MenuName.Trim();
                        model.Link = model.Link != null ? model.Link.Trim() : null;

                        if (model.Id == 0)
                        {
                            menu = new SMenu();
                            Parse.CopyObject(model, ref menu);
                            if (model.Icon != "0" && model.Icon != null)
                                menu.Icon = model.Icon.Split(',').ToList().First();

                            menu.CreatedUser = userId;
                            menu.CreatedDate = DateTime.Now;
                            db.SMenu.Add(menu);
                        }
                        else
                        {
                            menu = db.SMenu.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                            if (menu != null)
                            {
                                menu.MenuName = model.MenuName.Trim();
                                menu.MenuCategoryId = model.MenuCategoryId > 0 ? model.MenuCategoryId : menu.MenuCategoryId;
                                menu.Link = model.Link.Trim();
                                menu.OrderIndex = model.OrderIndex;
                                if (model.Icon != "0" && model.Icon != null)
                                    menu.Icon = model.Icon.Split(',').ToList().FirstOrDefault();

                                menu.IsViewIcon = model.IsViewIcon;
                                menu.Description = model.Description;
                                menu.IsShow = model.IsShow;
                                menu.ModuleId = model.ModuleId;
                                menu.UpdatedUser = userId;
                                menu.UpdatedDate = DateTime.Now;
                                db.Entry<SMenu>(menu).State = EntityState.Modified;
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "UpdateMenu", Message = "Menu đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                            }
                        }
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ResponseBase Delete(int id, int userId)
        {
            ResponseBase result;
            try
            {
                result = new ResponseBase();
                using (var db = new SanXuatCheckListEntities())
                {
                    var menuObj = db.SMenu.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                    if (menuObj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "DeleteMenu", Message = "Menu đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                    }
                    else
                    {
                        menuObj.IsDeleted = true;
                        menuObj.DeletedUser = userId;
                        menuObj.DeletedDate = DateTime.Now;
                        db.Entry<SMenu>(menuObj).State = EntityState.Modified;
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ResponseBase Delete(List<int> listId, int userId)
        {
            throw new NotImplementedException();
        }

        public PagedList<MenuModel> GetList(string keyWord, string searchBy, string categoryId, string moduleId, string moduleName, int startIndexRecord, int pageSize, string sorting, int companyId, int userId)
        {
            List<MenuModel> listMenu = null;
            try
            {
                if (string.IsNullOrEmpty(sorting))
                {
                    sorting = "orderindex aSC";
                }
                if (!string.IsNullOrEmpty(searchBy))
                {
                    listMenu = SearchListMenu(userId, companyId, sorting, moduleName, keyWord, searchBy, categoryId, moduleId);
                }
                else
                {
                    listMenu = GetListMenuByUserId(1, 1, sorting);
                }

                if (listMenu.Count > 0)
                {
                    if (companyId == 0)
                    {
                        listMenu = listMenu.Where(x => !x.CompanyId.HasValue).ToList();
                    }
                    else
                    {
                        listMenu = listMenu.Where(x => x.CompanyId == companyId).ToList();
                    }
                }

                var pageNumber = (startIndexRecord / pageSize) + 1;
                return new PagedList<MenuModel>(listMenu, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region search type
        private List<MenuModel> SearchListMenu(int userId, int companyId, string sorting, string moduleName, string keyWord, string searchBy, string categoryId, string moduleId)
        {
            List<MenuModel> listMenu = null;
            try
            {
                switch (searchBy)
                {
                    case "1": listMenu = SearchByMenuName(userId, companyId, sorting, moduleName, keyWord); break;
                    case "2": listMenu = SearchByCategoryId(userId, companyId, sorting, moduleName, categoryId); break;
                    case "3": listMenu = SearchByUrl(userId, companyId, sorting, moduleName, keyWord); break;
                    case "4": listMenu = SearchByModule(userId, companyId, sorting, moduleName, moduleId); break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listMenu;
        }

        private List<MenuModel> SearchByModule(int userId, int companyId, string sorting, string moduleName, string moduleId)
        {
            List<MenuModel> menus = null;
            try
            {
                using (var db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    int ModuleId = BLLModule.Instance.GetModuleIdBySystemName(moduleName, db);
                    int MODULEID = int.Parse(moduleId);
                    List<string> listPermissionUrl = (List<string>)BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, ModuleId, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        menus = (from x in db.SMenu
                                 where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link.Trim())) || (x.CompanyId != null && x.CompanyId == companyId)) && !x.IsDeleted && !x.SCategory.IsDeleted && x.ModuleId == MODULEID
                                 orderby (sorting)
                                 select new MenuModel()
                                 {
                                     Id = x.Id,
                                     MenuName = x.MenuName,
                                     Icon = x.Icon,
                                     OrderIndex = x.OrderIndex,
                                     Link = x.Link,
                                     IsShow = x.IsShow,
                                     IsViewIcon = x.IsViewIcon,
                                     Description = x.Description,
                                     MenuCategoryId = x.SCategory.Id,
                                     CategoryName = x.SCategory.Name,
                                     CategoryLink = x.SCategory.Link,
                                     CategoryPosition = x.SCategory.Position,
                                     CategoryIcon = x.SCategory.Icon,
                                     CategoryOrderIndex = x.SCategory.OrderIndex,
                                     CreatedDate = x.CreatedDate,
                                     CompanyId = x.CompanyId,
                                     ModuleId = x.ModuleId,
                                     ModuleName = x.SModule.ModuleName
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menus;
        }

        private List<MenuModel> SearchByUrl(int userId, int companyId, string sorting, string moduleName, string keyWord)
        {
            List<MenuModel> menus = null;
            try
            {
                using (var db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    int moduleId = BLLModule.Instance.GetModuleIdBySystemName(moduleName, db);
                    List<string> listPermissionUrl = (List<string>)BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, moduleId, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        menus = (from x
                                 in db.SMenu
                                 where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link.Trim())) || (x.CompanyId != null && x.CompanyId == companyId)) && !x.IsDeleted && !x.SCategory.IsDeleted && x.Link.ToUpper().Contains(keyWord.Trim().ToUpper())
                                 orderby (sorting)
                                 select new MenuModel()
                                 {
                                     Id = x.Id,
                                     MenuName = x.MenuName,
                                     Icon = x.Icon,
                                     OrderIndex = x.OrderIndex,
                                     Link = x.Link,
                                     IsShow = x.IsShow,
                                     IsViewIcon = x.IsViewIcon,
                                     Description = x.Description,
                                     MenuCategoryId = x.SCategory.Id,
                                     CategoryName = x.SCategory.Name,
                                     CategoryLink = x.SCategory.Link,
                                     CategoryPosition = x.SCategory.Position,
                                     CategoryIcon = x.SCategory.Icon,
                                     CategoryOrderIndex = x.SCategory.OrderIndex,
                                     CreatedDate = x.CreatedDate,
                                     CompanyId = x.CompanyId,
                                     ModuleId = x.ModuleId,
                                     ModuleName = x.SModule.ModuleName
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menus;
        }

        private List<MenuModel> SearchByCategoryId(int userId, int companyId, string sorting, string moduleName, string categoryId)
        {
            List<MenuModel> menus = null;
            try
            {
                using (var db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    int moduleId = BLLModule.Instance.GetModuleIdBySystemName(moduleName, db);
                    int categoryID = int.Parse(categoryId);
                    List<string> listPermissionUrl = (List<string>)BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, moduleId, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        menus = (from x
                                 in db.SMenu
                                 where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link.Trim())) || (x.CompanyId != null && x.CompanyId == companyId)) && !x.IsDeleted && !x.SCategory.IsDeleted && x.SCategory.Id == categoryID
                                 orderby (sorting)
                                 select new MenuModel()
                                 {
                                     Id = x.Id,
                                     MenuName = x.MenuName,
                                     Icon = x.Icon,
                                     OrderIndex = x.OrderIndex,
                                     Link = x.Link,
                                     IsShow = x.IsShow,
                                     IsViewIcon = x.IsViewIcon,
                                     Description = x.Description,
                                     MenuCategoryId = x.SCategory.Id,
                                     CategoryName = x.SCategory.Name,
                                     CategoryLink = x.SCategory.Link,
                                     CategoryPosition = x.SCategory.Position,
                                     CategoryIcon = x.SCategory.Icon,
                                     CategoryOrderIndex = x.SCategory.OrderIndex,
                                     CreatedDate = x.CreatedDate,
                                     CompanyId = x.CompanyId,
                                     ModuleId = x.ModuleId,
                                     ModuleName = x.SModule.ModuleName
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menus;
        }

        private List<MenuModel> SearchByMenuName(int userId, int companyId, string sorting, string moduleName, string keyWord)
        {
            List<MenuModel> menus = null;
            try
            {
                using (var db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    int moduleId = BLLModule.Instance.GetModuleIdBySystemName(moduleName, db);
                    List<string> listPermissionUrl = (List<string>)BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, moduleId, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        menus = (from x in db.SMenu
                                 where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link.Trim())) || (x.CompanyId != null && x.CompanyId == companyId)) && !x.IsDeleted && !x.SCategory.IsDeleted && x.MenuName.ToUpper().Contains(keyWord.Trim().ToUpper())
                                 orderby (sorting)
                                 select new MenuModel()
                                 {
                                     Id = x.Id,
                                     MenuName = x.MenuName,
                                     Icon = x.Icon,
                                     OrderIndex = x.OrderIndex,
                                     Link = x.Link,
                                     IsShow = x.IsShow,
                                     IsViewIcon = x.IsViewIcon,
                                     Description = x.Description,
                                     MenuCategoryId = x.SCategory.Id,
                                     CategoryName = x.SCategory.Name,
                                     CategoryLink = x.SCategory.Link,
                                     CategoryPosition = x.SCategory.Position,
                                     CategoryIcon = x.SCategory.Icon,
                                     CategoryOrderIndex = x.SCategory.OrderIndex,
                                     CreatedDate = x.CreatedDate,
                                     CompanyId = x.CompanyId,
                                     ModuleId = x.ModuleId,
                                     ModuleName = x.SModule.ModuleName
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menus;
        }
        #endregion

        public IQueryable<MenuModel> GetListMenuByCheckPermissionType(int moduleId, int companyId, int userId, List<string> listPermissionUrl, int checkPermissionType)
        {
            IQueryable<MenuModel> listModelMenu = null;
            try
            {
                using (var db = new SanXuatCheckListEntities())
                {
                    switch (checkPermissionType)
                    {
                        case (int)eCheckPermissionType.CheckPermissionByModule:
                            {
                                listModelMenu = (from x in db.SMenu
                                                 where !x.IsDeleted && x.IsShow && x.ModuleId == moduleId && !x.SModule.IsDeleted && (x.CompanyId == companyId || x.CompanyId == null)
                                                 select new MenuModel()
                                                 {
                                                     Id = x.Id,
                                                     MenuName = x.MenuName,
                                                     Icon = x.Icon,
                                                     OrderIndex = x.OrderIndex,
                                                     Link = x.Link,
                                                     IsShow = x.IsShow,
                                                     IsViewIcon = x.IsViewIcon,
                                                     Description = x.Description,
                                                     ModuleId = x.ModuleId,
                                                     CompanyId = x.CompanyId,
                                                     MenuCategoryId = x.MenuCategoryId
                                                 });
                                break;
                            }
                        case (int)eCheckPermissionType.CheckPermissionByCompany:
                        case (int)eCheckPermissionType.CheckPermissionByUser:
                            {
                                if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                                {
                                    listModelMenu = (from x in db.SMenu
                                                     where !x.IsDeleted && x.IsShow && x.ModuleId == moduleId && !x.SModule.IsDeleted && ((x.CompanyId == null && listPermissionUrl.Contains(x.Link.Trim())) || (x.CompanyId != null && x.CompanyId.Value == companyId))
                                                     select new MenuModel()
                                                     {
                                                         Id = x.Id,
                                                         MenuName = x.MenuName,
                                                         Icon = x.Icon,
                                                         OrderIndex = x.OrderIndex,
                                                         Link = x.Link,
                                                         IsShow = x.IsShow,
                                                         IsViewIcon = x.IsViewIcon,
                                                         Description = x.Description,
                                                         ModuleId = x.ModuleId,
                                                         CompanyId = x.CompanyId,
                                                         MenuCategoryId = x.MenuCategoryId
                                                     });
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return listModelMenu;
        }

        public List<MenuModel> GetListMenuByUserId(int userId, int companyId, string sorting)
        {
            List<MenuModel> menus = null;
            try
            {
                using (var db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    List<string> listPermissionUrl = (List<string>)BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        menus = (from x in db.SMenu
                                 where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link.Trim())) || (x.CompanyId != null && x.CompanyId == companyId)) && !x.IsDeleted && !x.SCategory.IsDeleted
                                 orderby (sorting)
                                 select new MenuModel()
                                 {
                                     Id = x.Id,
                                     MenuName = x.MenuName,
                                     Icon = x.Icon,
                                     OrderIndex = x.OrderIndex,
                                     Link = x.Link,
                                     IsShow = x.IsShow,
                                     IsViewIcon = x.IsViewIcon,
                                     Description = x.Description,
                                     MenuCategoryId = x.SCategory.Id,
                                     CategoryName = x.SCategory.Name,
                                     CategoryLink = x.SCategory.Link,
                                     CategoryPosition = x.SCategory.Position,
                                     CategoryIcon = x.SCategory.Icon,
                                     CategoryOrderIndex = x.SCategory.OrderIndex,
                                     CreatedDate = x.CreatedDate,
                                     CompanyId = x.CompanyId,
                                     ModuleId = x.ModuleId,
                                     ModuleName = x.SModule.ModuleName
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menus;
        }

        public List<MenuModel> GetListMenuByPosition(List<MenuModel> listMenu, string position)
        {
            List<MenuModel> menus = null;
            try
            {
                menus = listMenu.Where(x => x.CategoryPosition.Equals(position)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menus;
        }

        public List<ModelSelectItem> GetMenuSelectItem(List<MenuModel> listMenu)
        {
            List<ModelSelectItem> menuSelectItem = null;
            try
            {
                menuSelectItem = listMenu.Select(x => new ModelSelectItem()
                {
                    Name = x.CategoryName,
                    Value = x.MenuCategoryId,
                    IsDefault = false,
                    Data = x.MenuCategoryId,
                    Code = x.CategoryName
                }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return menuSelectItem;
        }
    }
}
