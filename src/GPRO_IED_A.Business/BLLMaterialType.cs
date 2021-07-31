using GPRO.Core.Mvc;
using PagedList;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Hugate.Framework;
using System.Threading.Tasks;
using GPRO.Ultilities;

namespace SanXuatCheckList.Business
{
   public class BLLMaterialType
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLMaterialType _Instance;
        public static BLLMaterialType Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLMaterialType();

                return _Instance;
            }
        }
        private BLLMaterialType() { }
        #endregion

        bool checkPermis(MaterialType obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ResponseBase InsertOrUpdate(MaterialTypeModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model ))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Update", Message = "Tên loại vật tư này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                    } 
                    else
                    {
                        MaterialType obj;
                        if (model.Id == 0)
                        {
                            obj = new MaterialType();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.MaterialType.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.MaterialType.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update", Message = "loại vật tư bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo loại vật tư này nên bạn không cập nhật được thông tin cho loại vật tư này." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Index = model.Index;   
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

        private bool CheckExists( MaterialTypeModel model )
        {
            try
            {
                MaterialType material = null; 
                    material = db.MaterialType.FirstOrDefault(x => !x.IsDeleted   && x.Name.Trim().ToUpper().Equals(model.Name) && x.Id != model.Id);

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
                    var obj = db.MaterialType.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete", Message = "loại vật tư bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(obj, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo loại vật tư này nên bạn không xóa được loại vật tư này." });
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

        public PagedList< MaterialTypeModel> GetList(string keyWord,   int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "CreatedDate DESC";
                    }
                    IQueryable<MaterialType> objs = null;
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        objs = db.MaterialType.Where(c => !c.IsDeleted   &&  c.Name.Trim().ToUpper().Contains(keyWord)  );
                    }
                    else
                     objs = db.MaterialType.Where(c => !c.IsDeleted  );
                    
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return  new PagedList< MaterialTypeModel>(objs
                        .Select(x => new  MaterialTypeModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Index = x.Index, 
                            CreatedDate = x.CreatedDate, 
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

        public List<ModelSelectItem> GetSelectList( )
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var list = new List<ModelSelectItem>();
                    var objs = db.MaterialType.Where(x => !x.IsDeleted ).Select(
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

        public int GetLastIndex()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.MaterialType.Where(x => !x.IsDeleted).OrderByDescending(x => x.Index).FirstOrDefault();
                return obj != null ? obj.Index : 0;
            }
        }

    }
}
