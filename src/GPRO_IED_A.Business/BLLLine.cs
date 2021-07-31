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
    public class BLLLine
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLLine _Instance;
        public static BLLLine Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLLine();

                return _Instance;
            }
        }
        private BLLLine() { }
        #endregion
        bool checkPermis(Line obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        private bool CheckExists(string name, string code, int Id, int WorkShopId, SanXuatCheckListEntities db)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    Line objectExists = null;
                    if (!string.IsNullOrEmpty(name))
                        objectExists = db.Line.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.WorkShopId == WorkShopId && c.Name.Trim().ToUpper().Equals(name.Trim().ToUpper()));
                    else
                        objectExists = db.Line.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.WorkShopId == WorkShopId && c.Code.Trim().ToUpper().Equals(code.Trim().ToUpper()));
                    if (objectExists == null)
                        return false;
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(LineModel model, bool isOwner)
        {
            ResponseBase result = new ResponseBase();
            var flag = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (CheckExists(model.Name, null, model.Id, model.WorkShopId, db))
                    {
                        flag = true;
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create  ", Message = "Tên Chuyền này Đã Tồn Tại,Vui Lòng Chọn Tên Khác" });
                    }
                    else if (!string.IsNullOrEmpty(model.Code))
                    {
                        if (CheckExists(null, model.Code, model.Id, model.WorkShopId, db))
                        {
                            flag = true;
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create  ", Message = "Mã Chuyền này Đã Tồn Tại,Vui Lòng Chọn Mã Khác" });
                        }
                    }

                    if (!flag)
                    {
                        Line obj;
                        if (model.Id == 0)
                        {
                            obj = new Line();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Line.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Line.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                            if (obj != null)
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo chuyền này nên bạn không cập nhật được thông tin cho chuyền này." });
                                }
                                else
                                {
                                    obj.Code = model.Code;
                                    obj.Name = model.Name;
                                    obj.CountOfLabours = model.CountOfLabours;
                                    obj.WorkShopId = model.WorkShopId;
                                    obj.Description = model.Description;
                                    obj.UpdatedDate = DateTime.Now;
                                    obj.UpdatedUser = model.ActionUser;
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "UpdateLine", Message = "Chuyền này Không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại!" });
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

        public ResponseBase Delete(int id, int userId, bool isOwner)
        {
            ResponseBase responResult;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    responResult = new ResponseBase();
                    var obj = db.Line.Where(c => !c.IsDeleted && c.Id == id).FirstOrDefault();
                    if (obj != null)
                    {
                        if (!checkPermis(obj, userId, isOwner))
                        {
                            responResult.IsSuccess = false;
                            responResult.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo chuyền này nên bạn không xóa được chuyền này." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = userId;
                            obj.DeletedDate = DateTime.Now;
                            db.SaveChanges();
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

        public List<ModelSelectItem> Gets(int workShopId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var listModelSelect = new List<ModelSelectItem>();
                    var lines = db.Line.Where(x => !x.IsDeleted && x.WorkShopId == workShopId).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name, Data = x.CountOfLabours });
                    if (lines != null && lines.Count() > 0)
                        listModelSelect.AddRange(lines);
                    else
                        listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = " Không có dữ Liệu " });

                    return listModelSelect;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedList<LineModel> Gets(string keyWord, int startIndexRecord, int pageSize, string sorting, int companyId, int[] relationCompanyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<Line> Lines = db.Line.Where(c => !c.IsDeleted && (c.WorkShop.CompanyId == null || c.WorkShop.CompanyId == companyId || relationCompanyId.Contains(c.WorkShop.CompanyId)));
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        Lines = Lines.Where(c => !c.IsDeleted &&
                        (c.WorkShop.CompanyId == null || c.WorkShop.CompanyId == companyId || relationCompanyId.Contains(c.WorkShop.CompanyId)) &&
                       (c.Name.Trim().ToUpper().Contains(keyWord) || c.Code.Trim().ToUpper().Contains(keyWord)));
                    }

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<LineModel>(Lines
                        .OrderBy(sorting)
                        .Select(c => new LineModel()
                        {
                            Id = c.Id,
                            Code = c.Code,
                            Name = c.Name,
                            WorkShopId = c.WorkShopId,
                            Description = c.Description,
                            CountOfLabours = c.CountOfLabours,
                            WorkShopName = c.WorkShop.Name,
                        }).ToList(), pageNumber, pageSize);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
