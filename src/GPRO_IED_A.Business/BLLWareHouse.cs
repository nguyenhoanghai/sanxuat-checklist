using GPRO.Core.Mvc;
using GPRO.Ultilities;
using Hugate.Framework;
using PagedList;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLWareHouse
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLWareHouse _Instance;
        public static BLLWareHouse Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLWareHouse();

                return _Instance;
            }
        }
        private BLLWareHouse() { }
        #endregion

        public ResponseBase CreateOrUpdate(WareHouseModel model)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var result = new ResponseBase();
                result.IsSuccess = false;
                try
                {
                    if (!CheckExists(model.Id, model.Name, model.Index, true)) // nếu ko bị trùng tên                
                    {
                        if (!CheckExists(model.Id, model.Name, model.Index, false)) // nếu ko bị trùng index
                            result.IsSuccess = true;
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create", Message = "Mã kho đã bị trùng, Xin chọn mã khác" });
                        }
                        if (result.IsSuccess)
                        {
                            WareHouse warehouse;
                            if (model.Id == 0)  // tạo một khách hàng mới
                            {
                                warehouse = new WareHouse();
                                Parse.CopyObject(model, ref warehouse);
                                warehouse.CreatedDate = DateTime.Now;
                                warehouse.CreatedUser = model.ActionUser;
                                db.WareHouse.Add(warehouse);
                                result.IsSuccess = true;
                            }
                            else // cập nhật
                            {
                                warehouse = GetById(model.Id);
                                if (warehouse != null)
                                {
                                    warehouse.Name = model.Name;
                                    warehouse.Index = model.Index;
                                    warehouse.Note = model.Note;
                                    warehouse.IsAgency = model.IsAgency;
                                    warehouse.UpdatedDate = DateTime.Now;
                                    warehouse.UpdatedUser = model.ActionUser;
                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Kho không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                                }
                            }
                            if (result.IsSuccess)
                            {
                                db.SaveChanges();
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi khi thực hiện SQL, Vui Lòng kiểm tra lại." });
                            }
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create", Message = "Tên kho đã tồn tại, Xin chọn tên khác" });
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi Exception" });
                }
                return result;
            }
        }

        private bool CheckExists(int Id, string value, int index, bool isCheckName)
        {
            WareHouse obj;
            if (!isCheckName)
                obj = db.WareHouse.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Index == index);
            else
                obj = db.WareHouse.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Name.Trim().Equals(value));
            return obj != null ? true : false;
        }

        public ResponseBase Delete(int Id, int actionUserId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var rs = new ResponseBase();
                try
                {
                    var warehouse = GetById(Id);
                    if (warehouse != null)
                    {
                        var now = DateTime.Now;
                        warehouse.IsDeleted = true;
                        warehouse.DeletedDate = now;
                        warehouse.DeletedUser = actionUserId;
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Create", Message = "Kho này không Tồn Tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception)
                {
                    rs.IsSuccess = false;
                    rs.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi Exception" });
                }
                return rs;
            }
        }

        public PagedList<WareHouseModel> GetList(string keyword, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<WareHouse> wareHouses = null;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        keyword = keyword.Trim().ToUpper();
                        wareHouses = db.WareHouse.Where(c => !c.IsDeleted && c.Name.Trim().ToUpper().Contains(keyword));
                    }
                    else
                        wareHouses = db.WareHouse.Where(c => !c.IsDeleted);
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var pagedList = new PagedList<WareHouseModel>(wareHouses.OrderBy(sorting).Select(c => new WareHouseModel()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Index = c.Index,
                        IsAgency = c.IsAgency,
                        Note = c.Note,
                    }).ToList(), pageNumber, pageSize);
                    if (pagedList.Count > 0)
                    {
                        string value = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                        if (pagedList.Count > 0)
                        {
                            foreach (var item in pagedList)
                            {
                                item.Code = value + item.Index;
                            }
                        }
                    }
                    return pagedList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public List<ModelSelectItem> GetSelectList()
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.WareHouse.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name }).ToList();
            }
        }

        public WareHouse GetById(int Id)
        {
            if (db == null)
            {
                using (db = new SanXuatCheckListEntities())
                {
                    return db.WareHouse.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                }
            }
            else
                return db.WareHouse.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public int GetLastIndex()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.WareHouse.Where(x => !x.IsDeleted).OrderByDescending(x => x.Index).FirstOrDefault();
                return obj != null ? obj.Index : 0;
            }
        }

        public List<ModelSelectItem> GetWareHouseSelect_FilterByTextInput(string text) // Lọc tên vật tư khi nhập vào kendo combobox
        {
            using (db = new SanXuatCheckListEntities())
            {
                IEnumerable<WareHouse> warehouses = null;
                var returnList = new List<ModelSelectItem>();

                warehouses = db.WareHouse.Where(x => !x.IsDeleted);
                if (warehouses != null && warehouses.Count() > 0)
                {
                    returnList.Add(new ModelSelectItem() { Value = 0, Code = "Chọn Tất Cả" });
                    var whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                    foreach (var item in warehouses)
                    {
                        returnList.Add(new ModelSelectItem() { Value = item.Id, Code = whvalue + item.Index });
                        returnList.Add(new ModelSelectItem() { Value = item.Id, Code = item.Name });
                    }
                }
                return returnList;
            }
        }
    }
}
