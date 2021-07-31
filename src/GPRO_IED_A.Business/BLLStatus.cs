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
    public class BLLStatus
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLStatus _Instance;
        public static BLLStatus Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLStatus();

                return _Instance;
            }
        }
        private BLLStatus() { }
        #endregion

        bool checkPermis(Status obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<StatusModel> GetList(int typeId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<Status> objs = null;
                    objs = db.Status.Where(x => !x.IsDeleted && x.StatusTypeId == typeId).OrderByDescending(x => x.CreatedDate);

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<StatusModel>(objs.OrderBy(sorting).Select(x => new StatusModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Note = x.Note,
                        StatusTypeId = x.StatusTypeId
                    }).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(StatusModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên Trạng thái này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        Status obj;
                        if (model.Id == 0)
                        {
                            obj = new Status();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Status.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Status.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
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
                                    obj.StatusTypeId = model.StatusTypeId;
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

        private bool CheckExists(StatusModel model)
        {
            try
            {
                Status obj = null;
                obj = db.Status.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(model.Name.Trim().ToUpper()) && x.StatusTypeId == model.StatusTypeId && x.Id != model.Id);

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
                    var objs = db.Status.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
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
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo mã hàng này nên bạn không xóa được mã hàng này." });
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

        public List<ModelSelectItem> GetSelectItem(string typeCode)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var selectItems = new List<ModelSelectItem>();
                    IQueryable<Status> objs = db.Status.Where(x => !x.IsDeleted && !x.StatusType.IsDeleted && x.StatusType.Code.Trim().ToUpper() == typeCode.Trim().ToUpper());
                    if (objs != null && objs.Count() > 0)
                    {
                        selectItems.AddRange(objs.Select(x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name
                        }).ToList());
                    }
                    else
                        selectItems.Add(new ModelSelectItem() { Value = 0, Name = "  Không có Trạng thái  " });
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
