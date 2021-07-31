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
    public class BLLStatusType
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLStatusType _Instance;
        public static BLLStatusType Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLStatusType();

                return _Instance;
            }
        }
        private BLLStatusType() { }
        #endregion

        bool checkPermis(StatusType obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<StatusTypeModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<StatusType> objs = null;
                    if (string.IsNullOrEmpty(keyWord))
                        objs = db.StatusType.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedDate);
                    else
                        objs = db.StatusType.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())).OrderByDescending(x => x.CreatedDate);

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<StatusTypeModel>(objs.OrderBy(sorting).Select(x => new StatusTypeModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Note = x.Note,
                    }).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(StatusTypeModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model, true))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Mã loại trạng thái này đã tồn tại. Vui lòng chọn lại Mã khác !." });
                        return result;
                    }
                    if (CheckExists(model, false))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên loại trạng thái này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        StatusType obj;
                        if (model.Id == 0)
                        {
                            obj = new StatusType();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.StatusType.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.StatusType.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Loại Trạng thái bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
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
                                    obj.Code = model.Code;
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

        private bool CheckExists(StatusTypeModel model, bool checkCode)
        {
            try
            {
                StatusType obj = null;
                if (checkCode)
                    obj = db.StatusType.FirstOrDefault(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(model.Code.Trim().ToUpper()) && x.Id != model.Id);
                else
                    obj = db.StatusType.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(model.Name.Trim().ToUpper()) && x.Id != model.Id);

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
                    var objs = db.StatusType.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (objs == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Loại Trạng thái bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(objs, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo loại trạng thái này nên bạn không xóa được loại trạng thái này." });
                        }
                        else
                        {
                            objs.IsDeleted = true;
                            objs.DeletedUser = acctionUserId;
                            objs.DeletedDate = DateTime.Now;

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

        public List<ModelSelectItem> GetSelectItem()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var selectItems = new List<ModelSelectItem>();
                    var objs = db.StatusType.Where(x => !x.IsDeleted).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name
                        }).ToList();

                    if (objs != null && objs.Count() > 0)
                    {
                        // selectItems.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn Trạng thái  - - " });
                        selectItems.AddRange(objs);
                    }
                    else
                        selectItems.Add(new ModelSelectItem() { Value = 0, Name = "  Không có loại trạng thái  " });
                    return selectItems;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
