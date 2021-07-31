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
    public class BLLMaterials
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLMaterials _Instance;
        public static BLLMaterials Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLMaterials();

                return _Instance;
            }
        }
        private BLLMaterials() { }
        #endregion

        bool checkPermis(Material  obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ResponseBase InsertOrUpdate(MaterialsModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model, false, model.CompanyId ?? 0))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Update", Message = "Tên nguyên liệu này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                    }
                    else if (!string.IsNullOrEmpty(model.Code) && CheckExists(model, true, model.CompanyId ?? 0))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert", Message = "Mã nguyên liệu này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                    }
                    else
                    {
                        Materials obj;
                        if (model.Id == 0)
                        {
                            obj = new Materials();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Materials.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Materials.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update", Message = "Nguyên liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo nguyên liệu này nên bạn không cập nhật được thông tin cho nguyên liệu này." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Code = model.Code;
                                    if (!string.IsNullOrEmpty(model.Image))
                                        obj.Image = model.Image;

                                    obj.UnitId = model.UnitId;
                                    obj.CompanyId = model.CompanyId;
                                    obj.Note = model.Note;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = model.UpdatedDate;
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

        private bool CheckExists(MaterialsModel model, bool checkCode, int companyId)
        {
            try
            {
                Materials material = null;
                if (checkCode)
                    material = db.Materials.FirstOrDefault(x => !x.IsDeleted && x.CompanyId == companyId && x.Code.Trim().ToUpper().Equals(model.Code) && x.Id != model.Id);
                else
                    material = db.Materials.FirstOrDefault(x => !x.IsDeleted && x.CompanyId == companyId && x.Name.Trim().ToUpper().Equals(model.Name) && x.Id != model.Id);

                if (material == null)
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
                    var obj = db.Materials.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete", Message = "Nguyên liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(obj, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo nguyên liệu này nên bạn không xóa được nguyên liệu này." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = acctionUserId;
                            obj.DeletedDate = DateTime.Now;
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

        public PagedList<MaterialsModel> GetList(string keyWord, int companyId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    IQueryable<Materials> objs = null;
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        objs = db.Materials.Where(c => !c.IsDeleted && !c.Unit.IsDeleted && (!c.CompanyId.HasValue || c.CompanyId == companyId) && (c.Name.Trim().ToUpper().Contains(keyWord) || c.Code.Trim().ToUpper().Contains(keyWord)));
                    }
                    else
                    {
                        objs = db.Materials.Where(c => !c.IsDeleted && !c.Unit.IsDeleted && (!c.CompanyId.HasValue || c.CompanyId == companyId));
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<MaterialsModel>(objs
                        .Select(x => new MaterialsModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Image = x.Image,
                            UnitId = x.UnitId,
                            UnitName = x.Unit.Name,
                            CreatedDate = x.CreatedDate,
                            IsPrivate = (x.CompanyId == null ? true : false),
                            CompanyId = x.CompanyId,
                            Note = x.Note
                        }).OrderBy(sorting).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetSelectItem(int companyId, int[] relationCompanyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var list = new List<ModelSelectItem>();
                    var objs = db.Materials.Where(x => !x.IsDeleted && (x.CompanyId == null || x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId ?? 0))).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name
                        }).ToList();
                    if (objs != null && objs.Count > 0)
                        list.AddRange(objs);
                    else
                        list.Add(new ModelSelectItem() { Name = "không có dữ liệu", Value = 0 });
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
