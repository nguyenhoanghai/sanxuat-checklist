using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hugate.Framework;

namespace SanXuatCheckList.Business
{
    public class BLLWorkerLevel
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLWorkerLevel _Instance;
        public static BLLWorkerLevel Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLWorkerLevel();

                return _Instance;
            }
        }
        private BLLWorkerLevel() { }
        #endregion

        bool checkPermis(SWorkerLevel obj, int actionUser, bool isOwner)
        {
            if (isOwner)
                return true;
            return obj.CreatedUser == actionUser;
        }

        private bool CheckExists(string name, int Id, int companyId, int[] relationCompanyId, SanXuatCheckListEntities db)
        {
            try
            {
                var obj = db.SWorkerLevel.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && (c.CompanyId == null || c.CompanyId == companyId || relationCompanyId.Contains(c.CompanyId ?? 0)) && c.Name.Trim().ToUpper().Equals(name.Trim().ToUpper()));
                if (obj == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(WorkerLevelModel model, int[] relationCompanyId, bool isOwner)
        {
            var result = new ResponseBase();
            result.IsSuccess = false;
            var flag = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (CheckExists(model.Name, model.Id, model.CompanyId ?? 0, relationCompanyId, db))
                    {
                        flag = true;
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create  ", Message = "Tên Bậc Thợ này Đã Tồn Tại, Vui Lòng Chọn Tên Khác" });
                    }

                    if (!flag)
                    {
                        SWorkerLevel obj = null;
                        if (model.Id == 0)
                        {
                            obj = new SWorkerLevel();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CompanyId = null;
                            if (model.IsPrivate)
                                obj.CompanyId = model.CompanyId;
                            db.SWorkerLevel.Add(obj);
                            db.SaveChanges(); ;
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.SWorkerLevel.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                            if (obj != null)
                            {
                                if (!checkPermis(obj, model.ActionUser,isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo bậc thợ này nên bạn không cập nhật được thông tin cho bậc thợ này." });
                                }
                                else
                                {
                                    obj.Coefficient = model.Coefficient;
                                    obj.CompanyId = null;
                                    if (model.IsPrivate)
                                        obj.CompanyId = model.CompanyId;
                                    obj.Name = model.Name;
                                    obj.Note = model.Note;
                                    obj.UpdatedDate = DateTime.Now;
                                    obj.UpdatedUser = model.ActionUser;
                                    db.SaveChanges(); ;
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Dữ Liệu Bậc Thợ bạn đang thao tác không tồn tại hoặc đã bị xóa.\nVui lòng kiểm tra lại!" });
                            }
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ResponseBase Delete(int id, int userId ,bool isOwner)
        {
            ResponseBase responResult;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    responResult = new ResponseBase();
                    var obj = db.SWorkerLevel.Where(c => !c.IsDeleted && c.Id == id).FirstOrDefault();
                    if (obj != null)
                    {
                        if (!checkPermis(obj, userId,isOwner))
                        {
                            responResult.IsSuccess = false;
                            responResult.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo bậc thợ này nên bạn không xóa được bậc thợ này." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = userId;
                            obj.DeletedDate = DateTime.Now;
                            db.SaveChanges(); ;
                            responResult.IsSuccess = true;
                        }
                    }
                    else
                    {
                        responResult.IsSuccess = false;
                        responResult.Errors.Add(new Error() { MemberName = "Delete", Message = "Đối Tượng Đã Bị Xóa,Vui Lòng Kiểm Tra Lại" });
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responResult;
        }

        public PagedList<WorkerLevelModel> Gets(string keyWord, int startIndexRecord, int pageSize, string sorting, int companyId, int[] relationCompanyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var objs = new List<WorkerLevelModel>();
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    if (!string.IsNullOrEmpty(keyWord))
                        objs.AddRange(db.SWorkerLevel.Where(c => !c.IsDeleted && (c.CompanyId == null || c.CompanyId == companyId || relationCompanyId.Contains(c.CompanyId ?? 0)) && c.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())).OrderByDescending(x => x.CreatedDate).Select(x => new WorkerLevelModel()
                        {
                            Id = x.Id,
                            CompanyId = x.CompanyId,
                            Coefficient = x.Coefficient,
                            Name = x.Name,
                            Note = x.Note,
                            IsPrivate = (x.CompanyId != null ? true : false)
                        }).OrderBy(sorting).ToList());
                    else
                        objs.AddRange(db.SWorkerLevel.Where(c => !c.IsDeleted && (c.CompanyId == null || c.CompanyId == companyId || relationCompanyId.Contains(c.CompanyId ?? 0))).OrderByDescending(x => x.CreatedDate).Select(x => new WorkerLevelModel()
                        {
                            Id = x.Id,
                            CompanyId = x.CompanyId,
                            Coefficient = x.Coefficient,
                            Name = x.Name,
                            Note = x.Note,
                            IsPrivate = (x.CompanyId != null ? true : false)
                        }).OrderBy(sorting).ToList());
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<WorkerLevelModel>(objs, pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> Gets(int companyId, int[] relationCompanyId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var objs = new List<ModelSelectItem>();
                try
                {
                    objs.AddRange(db.SWorkerLevel.Where(x => !x.IsDeleted && (x.CompanyId == null || x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId ?? 0))).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name, Double = x.Coefficient }));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return objs;
            }

        }

    }
}
