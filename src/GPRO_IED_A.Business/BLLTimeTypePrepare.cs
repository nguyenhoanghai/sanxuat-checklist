using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using Hugate.Framework;

namespace SanXuatCheckList.Business
{
    public class BLLTimeTypePrepare
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLTimeTypePrepare _Instance;
        public static BLLTimeTypePrepare Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLTimeTypePrepare();

                return _Instance;
            }
        }
        private BLLTimeTypePrepare() { }
        #endregion

        bool checkPermis(T_TimeTypePrepare obj, int actionUser, bool isOwner)
        {
            if (isOwner)
                return true;
            return obj.CreatedUser == actionUser;
        }
        #region method
        public ResponseBase InsertOrUpdate(TimeTypePrepareModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    bool flag = false;
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, true, db))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Tên này đã được sử dụng. Vui lòng chọn lại Tên khác !." });
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(model.Code))
                    {
                        if (CheckExists(model.Code.Trim().ToUpper(), model.Id, false, db))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Mã này đã được sử dụng. Vui lòng chọn lại Mã khác !." });
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        T_TimeTypePrepare obj = null;
                        if (model.Id == 0)
                        {
                            obj = new T_TimeTypePrepare();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.T_TimeTypePrepare.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.T_TimeTypePrepare.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser,isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo loại thời gian chuẩn bị này nên bạn không cập nhật được thông tin cho loại thời gian chuẩn bị này." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Code = model.Code;
                                    obj.IsPublic = model.IsPublic;
                                    obj.Description = model.Description;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                }
                            }
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

        private bool CheckExists(string keyword, int id, bool isCheckName, SanXuatCheckListEntities db)
        {
            try
            {
                T_TimeTypePrepare timeType = null;
                keyword = keyword.Trim().ToUpper();
                if (isCheckName)
                    timeType = db.T_TimeTypePrepare.FirstOrDefault(x => !x.IsDeleted && x.Id != id && x.Name.Trim().ToUpper().Equals(keyword));
                else
                    timeType = db.T_TimeTypePrepare.FirstOrDefault(x => !x.IsDeleted && x.Id != id && x.Code.Trim().ToUpper().Equals(keyword));

                if (timeType == null)
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
                    var timeType = db.T_TimeTypePrepare.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (timeType == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete  ", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(timeType, acctionUserId,isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo loại thời gian chuẩn bị này nên bạn không xóa được loại thời gian chuẩn bị này." });
                        }
                        else
                        {
                            timeType.IsDeleted = true;
                            timeType.DeletedUser = acctionUserId;
                            timeType.DeletedDate = DateTime.Now;
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


        public PagedList<TimeTypePrepareModel> Gets(string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<T_TimeTypePrepare> timeTypes = null;
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    if (!string.IsNullOrEmpty(keyWord))
                        switch (searchBy)
                        {
                            case 1:
                                timeTypes = db.T_TimeTypePrepare.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));
                                break;
                            case 2:
                                timeTypes = db.T_TimeTypePrepare.Where(x => !x.IsDeleted && x.Code.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));
                                break;
                        }
                    else
                        timeTypes = db.T_TimeTypePrepare.Where(x => !x.IsDeleted);

                    if (timeTypes != null && timeTypes.Count() > 0)
                    {
                        var returnList = timeTypes.OrderByDescending(x => x.CreatedDate).Select(x => new TimeTypePrepareModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            IsPublic = x.IsPublic,
                            Description = x.Description,
                        }).OrderBy(sorting).ToList();
                        return new PagedList<TimeTypePrepareModel>(returnList, pageNumber, pageSize);
                    }
                    else
                        return new PagedList<TimeTypePrepareModel>(new List<TimeTypePrepareModel>(), pageNumber, pageSize);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetListTimeTypePrepareByworkShopId(int id)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var timeType = db.T_TimeTypePrepare.Where(x => !x.IsDeleted && x.IsPublic);

                    if (timeType != null && timeType.Count() > 0)
                    {
                        return timeType.Select(
                                    x => new ModelSelectItem()
                                    {
                                        Value = x.Id,
                                        Name = x.Name
                                    }).ToList();
                    }
                    else
                        return new List<ModelSelectItem>();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetListTimeTypePrepareSelect(int CompanyId, int[] relationCompanyIds)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var timeType = db.T_TimeTypePrepare.Where(x => !x.IsDeleted);
                    if (timeType != null && timeType.Count() > 0)
                    {
                        return timeType.Select(
                                    x => new ModelSelectItem()
                                    {
                                        Value = x.Id,
                                        Name = x.Name
                                    }).ToList();
                    }
                    else
                        return new List<ModelSelectItem>();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
