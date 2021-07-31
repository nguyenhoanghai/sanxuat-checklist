using GPRO.Core.Mvc;
using GPRO.Ultilities;
using Hugate.Framework;
using PagedList;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLUnit
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLUnit _Instance;
        public static BLLUnit Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLUnit();

                return _Instance;
            }
        }
        private BLLUnit() { }
        #endregion

        bool checkPermis(Unit obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<UnitModel> GetList(int parentId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<Unit> units = null; 
                        units = db.Unit.Where(x => !x.IsDeleted && x.UnitTypeId == parentId).OrderByDescending(x => x.CreatedDate);
                    
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<UnitModel>(units.Select(x => new UnitModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Note = x.Note,
                        UnitTypeId = x.UnitTypeId,
                        TypeName = x.UnitType.Name
                    }).OrderBy(sorting).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
 
        public ResponseBase InsertOrUpdate(UnitModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model, model.Id))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên  Đơn vị này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        Unit obj;
                        if (model.Id == 0)
                        {
                            obj = new Unit();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Unit.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Unit.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Loại Đơn vị bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo mã hàng này nên bạn không cập nhật được thông tin cho mã hàng này." });
                                }
                                else
                                {
                                    obj.UnitTypeId = model.UnitTypeId;
                                    obj.Name = model.Name;
                                    obj.Note = model.Note;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckExists(UnitModel model, int? id)
        {
            try
            {
                Unit obj = null;
                obj = db.Unit.FirstOrDefault(x => !x.IsDeleted && !x.UnitType.IsDeleted && x.UnitTypeId == model.UnitTypeId && x.Name.Trim().ToUpper().Equals(model.Name.Trim().ToUpper()) && x.Id != id);

                if (obj == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Delete(int id, int acctionUserId, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var productType = db.Unit.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (productType == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete Product Type", Message = "Loại Đơn vị bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(productType, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo mã hàng này nên bạn không xóa được mã hàng này." });
                        }
                        else
                        {
                            productType.IsDeleted = true;
                            productType.DeletedUser = acctionUserId;
                            productType.DeletedDate = DateTime.Now;

                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetSelectItem( string typeCode)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var listModelSelect = new List<ModelSelectItem>();
                    var productTypes = db.Unit.Where(x => !x.IsDeleted && x.UnitType.Code == typeCode).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name,
                            Data = x.UnitTypeId,
                            Code = x.Note,
                        }).ToList();

                    if (productTypes != null && productTypes.Count() > 0)
                    {
                       // listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn Đơn vị  - - " });
                        listModelSelect.AddRange(productTypes);
                    }
                    else
                        listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = "  Không có Đơn vị  " });
                    return listModelSelect;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
