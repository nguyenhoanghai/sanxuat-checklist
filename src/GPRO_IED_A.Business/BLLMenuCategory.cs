using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLMenuCategory
    {
        #region constructor 
        static object key = new object();
        private static volatile BLLMenuCategory _Instance;
        public static BLLMenuCategory Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLMenuCategory();

                return _Instance;
            }
        }
        private BLLMenuCategory() { }
        #endregion

        private SCategory CheckMenuName(int Id, string menuCategoryName, int companyId, SanXuatCheckListEntities db)
        {
            return (from x in db.SCategory
                    where !x.IsDeleted && (x.CompanyId == companyId || !x.CompanyId.HasValue) && x.Name.Trim().ToUpper().Equals(menuCategoryName.Trim().ToUpper()) && x.Id != Id
                    select x).FirstOrDefault();
        }

        public ResponseBase InsertOrUpdate(CategoryModel model)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                try
                {
                    var result = new ResponseBase();
                    var flag = false;
                    SCategory category = null;
                    if (CheckMenuName(model.Id, model.Name, model.CompanyId ?? 0, db) != null)
                    {
                        flag = true;
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { Message = "Tên Nhóm danh mục đã tồn tại. Vui lòng chọn lại tên khác!", MemberName = "Thêm Mới Menu" });
                    }

                    if (!flag)
                    {
                        if (model.Id == 0)
                        {
                            category = new SCategory();
                            model.Name = model.Name.Trim();
                            Parse.CopyObject(model, ref category);
                            if (model.Icon != "0" && model.Icon != null)
                            { category.Icon = model.Icon.Split(',').ToList().First(); }
                            db.SCategory.Add(category);
                        }
                        else
                        {
                            category = db.SCategory.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                            if (category != null)
                            {
                                category.Name = model.Name.Trim();
                                category.Position = model.Position;
                                category.Link = model.Link;
                                category.OrderIndex = model.OrderIndex;
                                category.ModuleId = model.ModuleId;
                                if (model.Icon != "0" && model.Icon != null)
                                {
                                    category.Icon = model.Icon.Split(',').ToList().FirstOrDefault();
                                }
                                category.IsViewIcon = model.IsViewIcon;
                                category.Description = model.Description;
                                category.ModuleId = model.ModuleId;
                                category.UpdatedUser = model.UpdatedUser;
                                category.UpdatedDate = model.UpdatedDate;
                                db.Entry<SCategory>(category).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Cập Nhật Nhóm Danh Mục", Message = "Nhóm Danh Mục đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                            }
                        }
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public ResponseBase DeleteById(int id, int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ResponseBase result;
                try
                {
                    result = new ResponseBase();
                    var category = db.SCategory.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                    if (category == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Xóa Nhóm Danh Mục", Message = "Nhóm Danh Mục đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                    }
                    else
                    {
                        category.IsDeleted = true;
                        category.DeletedUser = userId;
                        category.DeletedDate = DateTime.Now;
                        db.Entry<SCategory>(category).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
        }

        public ResponseBase DeleteByListId(List<int> listId, int userId)
        {
            ResponseBase responResult = null;
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return responResult;
        }

        public PagedList<CategoryModel> GetList(string keyword, string position, int moduleId, int startIndexRecord, int pageSize, string sorting, int userId, int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<CategoryModel> categories = null;
                IQueryable<SCategory> cateMenus = null;
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        if (companyId != 0)
                        {
                            // lay theo companyId
                            cateMenus = (from x
                                         in db.SCategory
                                         where (listPermissionUrl.Contains(x.Link) && x.CompanyId == companyId) && !x.IsDeleted
                                         orderby (sorting)
                                         select x);
                        }
                        else
                        {
                            // get system
                            cateMenus = (from x
                                         in db.SCategory
                                         where (x.CompanyId == null && listPermissionUrl.Contains(x.Link)) && !x.IsDeleted
                                         orderby (sorting)
                                         select x);
                        }
                        if (cateMenus != null && cateMenus.Count() > 0)
                        {
                            if (!string.IsNullOrEmpty(keyword))
                                cateMenus = cateMenus.Where(x => x.Name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()));
                            if (position != "0")
                                cateMenus = cateMenus.Where(x => x.Position.Trim().ToUpper().Equals(position.Trim().ToUpper()));
                            if (moduleId != 0)
                                cateMenus = cateMenus.Where(x => x.ModuleId == moduleId);

                            categories = cateMenus.Select(x => new CategoryModel()
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Icon = x.Icon,
                                IsViewIcon = x.IsViewIcon,
                                Link = x.Link,
                                OrderIndex = x.OrderIndex,
                                Description = x.Description,
                                CompanyId = x.CompanyId,
                                Position = x.Position,
                                CreatedDate = x.CreatedDate,
                                ModuleId = x.ModuleId,
                                ModuleName = x.SModule.ModuleName
                            }).ToList();
                        }
                    }
                    if (categories == null)
                        categories = new List<CategoryModel>();
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<CategoryModel>(categories, pageNumber, pageSize);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<CategoryModel> GetCategorysByUserId(int userId, int companyId, string sorting, string position)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<CategoryModel> categories = null;
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        categories = (from x in db.SCategory
                                      where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link)) || (x.CompanyId == companyId)) && !x.IsDeleted
                                      orderby (sorting)
                                      select new CategoryModel()
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          Icon = x.Icon,
                                          IsViewIcon = x.IsViewIcon,
                                          Link = x.Link,
                                          OrderIndex = x.OrderIndex,
                                          Description = x.Description,
                                          CompanyId = x.CompanyId,
                                          Position = x.Position,
                                          CreatedDate = x.CreatedDate,
                                          ModuleId = x.ModuleId,
                                          ModuleName = x.SModule.ModuleName
                                      }).ToList();
                        if (categories.Count > 0)
                        {
                            categories = (position != null ? categories.Where(x => x.Position.Equals(position)).ToList() : categories);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return categories;
            }
        }

        public List<CategoryModel> GetCategorysByUserId(int userId, int companyId, string sorting, string position, string moduleName)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<CategoryModel> categories = null;
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    int moduleId = BLLModule.Instance.GetModuleIdBySystemName(moduleName, db);
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);

                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        categories = (from x in db.SCategory
                                      where ((x.CompanyId == null && (x.Link == null || (x.Link != null && listPermissionUrl.Contains(x.Link)))) || (x.CompanyId == companyId)) && !x.IsDeleted && x.ModuleId == moduleId && !x.SModule.IsDeleted
                                      orderby (sorting)
                                      select new CategoryModel()
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          Icon = x.Icon,
                                          IsViewIcon = x.IsViewIcon,
                                          Link = x.Link,
                                          OrderIndex = x.OrderIndex,
                                          Description = x.Description,
                                          CompanyId = x.CompanyId,
                                          Position = x.Position,
                                          CreatedDate = x.CreatedDate,
                                          ModuleId = x.ModuleId,
                                          ModuleName = x.SModule.ModuleName
                                      }).ToList();
                        if (categories.Count > 0)
                        {
                            categories = (position != null ? categories.Where(x => x.Position.Equals(position)).ToList() : categories);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return categories;
            }
        }

        public List<CategoryModel> GetCategorysByUserId(int userId, int companyId, string sorting, string position, string keyword, string searchBy, string moduleId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<CategoryModel> categories = null;
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }

                    IQueryable<SCategory> cateMenus = null;
                    List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                    List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                    if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                    {
                        cateMenus = (from x in db.SCategory
                                     where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link)) || (x.CompanyId == companyId)) && !x.IsDeleted
                                     orderby (sorting)
                                     select x);

                        if (cateMenus != null && cateMenus.Count() > 0)
                        {
                            if (!string.IsNullOrEmpty(keyword))
                                cateMenus = cateMenus.Where(x => x.Name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()));
                            if (!string.IsNullOrEmpty(position))
                                cateMenus = cateMenus.Where(x => x.Position.Equals(position));
                            if (!string.IsNullOrEmpty(moduleId))
                            {
                                int moduleID = int.Parse(moduleId);
                                cateMenus = cateMenus.Where(x => x.ModuleId == moduleID);
                            }
                            categories = cateMenus.Select(x => new CategoryModel()
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Icon = x.Icon,
                                IsViewIcon = x.IsViewIcon,
                                Link = x.Link,
                                OrderIndex = x.OrderIndex,
                                Description = x.Description,
                                CompanyId = x.CompanyId,
                                Position = x.Position,
                                CreatedDate = x.CreatedDate,
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
                return categories;
            }
        }

        private List<CategoryModel> SearchByModule(int userId, int companyId, string sorting, string moduleId, SanXuatCheckListEntities db)
        {

            List<CategoryModel> categories = null;
            try
            {
                List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                int moduleID = int.Parse(moduleId);
                if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                {
                    categories = (from x in db.SCategory
                                  where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link)) || (x.CompanyId == companyId)) && !x.IsDeleted && x.ModuleId == moduleID
                                  orderby (sorting)
                                  select new CategoryModel()
                                  {
                                      Id = x.Id,
                                      Name = x.Name,
                                      Icon = x.Icon,
                                      IsViewIcon = x.IsViewIcon,
                                      Link = x.Link,
                                      OrderIndex = x.OrderIndex,
                                      Description = x.Description,
                                      CompanyId = x.CompanyId,
                                      Position = x.Position,
                                      CreatedDate = x.CreatedDate,
                                      ModuleId = x.ModuleId,
                                      ModuleName = x.SModule.ModuleName
                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return categories;
        }

        private List<CategoryModel> SearchByPosition(int userId, int companyId, string sorting, string position, SanXuatCheckListEntities db)
        {
            List<CategoryModel> categories = null;
            try
            {
                List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                {
                    categories = (from x in db.SCategory
                                  where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link)) || (x.CompanyId == companyId)) && !x.IsDeleted && x.Position.Equals(position)
                                  orderby (sorting)
                                  select new CategoryModel()
                                  {
                                      Id = x.Id,
                                      Name = x.Name,
                                      Icon = x.Icon,
                                      IsViewIcon = x.IsViewIcon,
                                      Link = x.Link,
                                      OrderIndex = x.OrderIndex,
                                      Description = x.Description,
                                      CompanyId = x.CompanyId,
                                      Position = x.Position,
                                      CreatedDate = x.CreatedDate,
                                      ModuleId = x.ModuleId,
                                      ModuleName = x.SModule.ModuleName
                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return categories;
        }

        private List<CategoryModel> SearchByKeyword(int userId, int companyId, string sorting, string keyword, SanXuatCheckListEntities db)
        {
            List<CategoryModel> categories = null;
            try
            {
                List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                List<string> listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, db);
                if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                {
                    categories = (from x in db.SCategory
                                  where ((x.CompanyId == null && listPermissionUrl.Contains(x.Link)) || (x.CompanyId == companyId)) && !x.IsDeleted && x.Name.ToUpper().Contains(keyword.Trim().ToUpper())
                                  orderby (sorting)
                                  select new CategoryModel()
                                  {
                                      Id = x.Id,
                                      Name = x.Name,
                                      Icon = x.Icon,
                                      IsViewIcon = x.IsViewIcon,
                                      Link = x.Link,
                                      OrderIndex = x.OrderIndex,
                                      Description = x.Description,
                                      CompanyId = x.CompanyId,
                                      Position = x.Position,
                                      CreatedDate = x.CreatedDate,
                                      ModuleId = x.ModuleId,
                                      ModuleName = x.SModule.ModuleName
                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return categories;
        }

        public ResponseBase UpdateSystem(CategoryModel menuCategory, int userId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ResponseBase result = null;
                try
                {
                    result = new ResponseBase();
                    SCategory category = db.SCategory.FirstOrDefault(x => x.Id == menuCategory.Id && !x.IsDeleted);
                    if (category != null)
                    {
                        category.Position = menuCategory.Position;
                        category.OrderIndex = menuCategory.OrderIndex;
                        if (menuCategory.Icon != "0" && menuCategory.Icon != null)
                        {
                            category.Icon = menuCategory.Icon.Split(',').ToList().FirstOrDefault();
                        }
                        category.UpdatedUser = userId;
                        category.UpdatedDate = DateTime.Now;
                        db.Entry<SCategory>(category).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Cập Nhật Nhóm Danh Mục", Message = "Nhóm Danh Mục đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
        }

        public List<CategoryModel> GetCategoriesByCompanyIdNotSystem(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                var returnList = new List<CategoryModel>();
                returnList.Add(new CategoryModel() { Id = 0, Name = " Không có Dữ Liệu " });
                try
                {
                    var categories = (from x in db.SCategory
                                      where x.CompanyId == companyId && !x.IsDeleted
                                      select new CategoryModel()
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          CompanyId = x.CompanyId
                                      });
                    if (categories != null && categories.Count() > 0)
                    {
                        returnList[0].Name = " - - Chọn Nhóm Danh Mục - - ";
                        returnList.AddRange(categories);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnList;
            }
        }

        public ModelListSelectItemAndDefaultValue GetModelListSelectItemAndDefaultValueByCompanyId(int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                ModelListHiddenObjectIdAndDefaultValue modelListConfigAndDefault = null;
                List<ModelSelectItem> ListModelSelectItem = new List<ModelSelectItem>();
                try
                {
                    // khoi tao list selectItem
                    ListModelSelectItem.Add(new ModelSelectItem() { Value = 0, Name = "---   Chọn Nhóm Danh Mục   ----" });

                    // get listHiddenObjectId va default value
                    modelListConfigAndDefault = BLLConfig.Instance.GetListModelConfigAndDefaultValueByTableNameAndCompanyId(eTableName.SCATEGORY, companyId, db);
                    if (modelListConfigAndDefault.listHiddenObjectId != null && modelListConfigAndDefault.listHiddenObjectId.Count > 0)
                    {
                        ListModelSelectItem.AddRange((from x in db.SCategory
                                                      where !x.IsDeleted && !modelListConfigAndDefault.listHiddenObjectId.Contains(x.Id)
                                                      select new ModelSelectItem() { Value = x.Id, Name = x.Name }));
                    }
                    else
                    {
                        ListModelSelectItem.AddRange((from x in db.SCategory
                                                      where !x.IsDeleted
                                                      select new ModelSelectItem() { Value = x.Id, Name = x.Name }));
                    }

                    return new ModelListSelectItemAndDefaultValue() { defaultValue = modelListConfigAndDefault.defaultValue, listModelSelectItem = ListModelSelectItem };
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<CategoryModel> GetCategoriesByListCategoryIds(List<int> categoryIds)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<CategoryModel> categories = null;
                try
                {
                    if (categoryIds != null && categoryIds.Count > 0)
                    {
                        categories = (from x in db.SCategory
                                      where !x.IsDeleted && categoryIds.Contains(x.Id)
                                      select new CategoryModel()
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          Icon = x.Icon,
                                          IsViewIcon = x.IsViewIcon,
                                          Link = x.Link,
                                          OrderIndex = x.OrderIndex,
                                          Description = x.Description,
                                          CompanyId = x.CompanyId,
                                          Position = x.Position,
                                          CreatedDate = x.CreatedDate,
                                          ModuleId = x.ModuleId,
                                          ModuleName = x.SModule.ModuleName
                                      }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return categories;
            }
        }

        private IQueryable<CategoryModel> GetCategorysByModuleId(int moduleId, string position, int companyId, SanXuatCheckListEntities db)
        {
            IQueryable<CategoryModel> listMenuCategory = null;
            try
            {
                IQueryable<SCategory> listCategory = null;
                if (!string.IsNullOrEmpty(position))
                    listCategory = (from x
                                    in db.SCategory
                                    where !x.IsDeleted && x.ModuleId == moduleId && !x.SModule.IsDeleted && x.Position.Equals(position) && (x.CompanyId == null || x.CompanyId == companyId)
                                    select x);
                else
                    listCategory = (from x
                                    in db.SCategory
                                    where !x.IsDeleted && x.ModuleId == moduleId && !x.SModule.IsDeleted && (x.CompanyId == null || x.CompanyId == companyId)
                                    select x);
                if (listCategory != null)
                {
                    listMenuCategory = listCategory.Select(x => new CategoryModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Icon = x.Icon,
                        IsViewIcon = x.IsViewIcon,
                        Link = x.Link,
                        OrderIndex = x.OrderIndex,
                        Description = x.Description,
                        ModuleId = x.ModuleId,
                        ModuleName = x.SModule.SystemName,
                        Position = x.Position
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return listMenuCategory;
        }

        #region get menu new method
        public List<CategoryModel> GetMenusAndMenuCategoriesByUserId(int userId, int companyId, string moduleName, string position, int checkPermissionType)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                List<CategoryModel> listMenuCategory = null;
                int moduleId = 0;
                try
                {
                    moduleId = BLLModule.Instance.GetModuleIdBySystemName(moduleName, db);
                    List<string> listPermissionUrl = null;
                    switch (checkPermissionType)
                    {
                        case (int)eCheckPermissionType.CheckPermissionByModule:
                            break;
                        case (int)eCheckPermissionType.CheckPermissionByCompany:
                            listPermissionUrl = BLLRolePermission.Instance.GetSystemNameAndUrlOfPermissionBycompanyId(companyId, db);
                            break;
                        case (int)eCheckPermissionType.CheckPermissionByUser:
                            List<int> listUserRole = BLLUserRole.Instance.GetUserRolesIdByUserId(userId, db);
                            listPermissionUrl = BLLRolePermission.Instance.GetListSystemNameAndUrlOfPermissionByListRoleId(listUserRole, moduleId, db);
                            break;
                    }

                    var menuCategorys = GetCategorysByModuleId(moduleId, position, companyId, db).OrderBy(x => x.OrderIndex).ToList();
                    if (menuCategorys != null && menuCategorys.Count() > 0)
                    {
                        listMenuCategory = new List<CategoryModel>();
                        var menus = BLLMenu.Instance.GetListMenuByCheckPermissionType(moduleId, companyId, userId, listPermissionUrl, checkPermissionType);
                        foreach (var menuCategory in menuCategorys)
                        {
                            bool isAdd = false;
                            if (menus != null && menus.Count() > 0)
                            {
                                var listMenu = menus.Where(x => x.MenuCategoryId == menuCategory.Id).ToList();
                                if (listMenu.Count > 0)
                                {
                                    var modelMenuCategory = new CategoryModel();
                                    Parse.CopyObject(menuCategory, ref modelMenuCategory);
                                    modelMenuCategory.listMenu = listMenu;
                                    listMenuCategory.Add(modelMenuCategory);
                                    isAdd = true;
                                }
                            }
                            if (!isAdd && !string.IsNullOrEmpty(menuCategory.Link))
                            {
                                switch (checkPermissionType)
                                {
                                    case (int)eCheckPermissionType.CheckPermissionByModule:
                                        {
                                            var modelMenuCategory = new CategoryModel();
                                            Parse.CopyObject(menuCategory, ref modelMenuCategory);
                                            listMenuCategory.Add(modelMenuCategory);
                                            break;
                                        }
                                    case (int)eCheckPermissionType.CheckPermissionByCompany:
                                    case (int)eCheckPermissionType.CheckPermissionByUser:
                                        {
                                            if (listPermissionUrl != null && listPermissionUrl.Count > 0)
                                            {
                                                if ((menuCategory.CompanyId == null && listPermissionUrl.Contains(menuCategory.Link.Trim())) || menuCategory.CompanyId != null)
                                                {
                                                    var modelMenuCategory = new CategoryModel();
                                                    Parse.CopyObject(menuCategory, ref modelMenuCategory);
                                                    listMenuCategory.Add(modelMenuCategory);
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return listMenuCategory;
            }
        }
        #endregion

        public List<ModelSelectItem> GetMenuCategoriesByModuleId(int id, int companyId)
        {
            using (var db = new SanXuatCheckListEntities())
            {
                return (from x in db.SCategory where !x.IsDeleted && x.ModuleId == id && (x.CompanyId == companyId || !x.CompanyId.HasValue) select new ModelSelectItem() { Value =x.Id, Name = x.Name }).ToList();
            }
        }
    }
}
