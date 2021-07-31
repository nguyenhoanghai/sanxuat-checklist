using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLEquipmentType
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLEquipmentType _Instance;
        public static BLLEquipmentType Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEquipmentType();

                return _Instance;
            }
        }
        private BLLEquipmentType() { }
        #endregion

        public ResponseBase CheckExistInEquipmentAtt(int equipmentTypeId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                ResponseBase result = new ResponseBase();
                var number = db.T_EquipmentTypeAttribute.Count(x => !x.IsDeleted && x.EquipmentTypeId == equipmentTypeId);
                if (number > 0)
                {
                    result.IsSuccess = false;
                    return result;
                }
                else
                    result.IsSuccess = true;
                return result;
            }

        }

        private bool CheckEquipmentTypeName(string EquipmentTypeName, int CompanyId, int Id)
        {
            var checkResult = false;
            try
            {
                var checkName = db.T_EquipmentType.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.CompanyId == CompanyId && c.Name.Trim().ToUpper().Equals(EquipmentTypeName.Trim().ToUpper()));
                if (checkName == null)
                    checkResult = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return checkResult;
        }

        public string GetEquipmentTypeNameById(int Id)
        {
            using (db = new SanXuatCheckListEntities())
            {
                string name = null;
                try
                {
                    name = db.T_EquipmentType.FirstOrDefault(x => !x.IsDeleted && x.Id == Id).Name;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return name;
            }

        }

        public ResponseBase Create(ModelEquipmentType model)
        {
            ResponseBase result = new ResponseBase();
            result.IsSuccess = false;
            try
            {
                if (model != null)
                {
                    using (db = new SanXuatCheckListEntities())
                    {
                        if (CheckEquipmentTypeName(model.Name, model.Id, model.CompanyId))
                        {

                            var obj = new T_EquipmentType();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedUser = model.ActionUser;
                            obj.CreatedDate = DateTime.Now;
                            if (obj.EquipTypeDefaultId != null)
                            {
                                var default_attr = db.T_EquipTypeAttr_Default.Where(x => x.EquipType_DefaultId == obj.EquipTypeDefaultId);
                                if (default_attr != null && default_attr.Count() > 0)
                                {
                                    T_EquipmentTypeAttribute eAttr = null;
                                    obj.T_EquipmentTypeAttribute = new Collection<T_EquipmentTypeAttribute>();
                                    int i = 1;
                                    foreach (var item in default_attr)
                                    {
                                        eAttr = new T_EquipmentTypeAttribute();
                                        eAttr.T_EquipmentType = obj;
                                        eAttr.Name = item.Name;
                                        eAttr.EquipTypeAtrrDefault_Id = item.Id;
                                        eAttr.OrderIndex = i;
                                        eAttr.IsUseForTime = true;
                                        eAttr.IsDefault = true;
                                        eAttr.CreatedUser = obj.CreatedUser;
                                        eAttr.CreatedDate = obj.CreatedDate;
                                        obj.T_EquipmentTypeAttribute.Add(eAttr);
                                        i++;
                                    }
                                }
                            }
                            db.T_EquipmentType.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create EquipmentType", Message = "Tên Đã Tồn Tại,Vui Lòng Chọn Tên Khác" });
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "Create EquipmentType", Message = "Đối Tượng Không tồn tại" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ResponseBase Update(ModelEquipmentType model)
        {

            ResponseBase result = new ResponseBase();
            result.IsSuccess = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (!CheckEquipmentTypeName(model.Name, model.Id, model.CompanyId))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "UpdateEquipmentType", Message = "Trùng Tên. Vui lòng chọn lại" });
                    }
                    else
                    {
                        T_EquipmentType obj = db.T_EquipmentType.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                        if (obj != null)
                        {
                            // obj.Code = model.Code;
                            obj.Name = model.Name;
                            obj.Description = model.Description;
                            obj.CompanyId = model.CompanyId;
                            obj.UpdatedDate = DateTime.Now;
                            obj.UpdatedUser = model.ActionUser;
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "UpdateEquipmentType", Message = "Thông tin nhập không đúng Vui lòng kiểm tra lại!" });
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

        public ResponseBase DeleteById(int id, int userId)
        {
            ResponseBase rs;

            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    rs = new ResponseBase();
                    var obj = db.T_EquipmentType.FirstOrDefault(c => !c.IsDeleted && c.Id == id);
                    if (obj != null)
                    {
                        obj.IsDeleted = true;
                        obj.DeletedUser = userId;
                        obj.DeletedDate = DateTime.Now;
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Đối Tượng Đã Bị Xóa,Vui Lòng Kiểm Tra Lại" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rs;
        }

        public List<ModelSelectItem> GetListEquipmentType()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    List<ModelSelectItem> listModelSelect = new List<ModelSelectItem>();
                    listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = "- - Chọn Loại Thiết Bị - -" });
                    listModelSelect.AddRange(db.T_EquipmentType.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem()
                    {
                        Value = x.Id,
                        Name = x.Name,
                        Data = x.EquipTypeDefaultId ?? 0,
                        //Code = x.Code
                    }));
                    return listModelSelect;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedList<ModelEquipmentType> GetList(string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting, int companyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<T_EquipmentType> equipTypes = null;
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        switch (searchBy)
                        {
                            case 1:
                                equipTypes = db.T_EquipmentType.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord));
                                break;
                                // case 2: equipTypes = db.T_EquipmentType.Where(x => !x.IsDeleted && x.Code.Trim().ToUpper().Contains(keyWord));
                                //   break;
                        }
                    }
                    else
                        equipTypes = db.T_EquipmentType.Where(c => !c.IsDeleted);

                    var EquipmentTypes = equipTypes.OrderByDescending(x => x.CreatedDate).Select(c => new ModelEquipmentType()
                    {
                        Id = c.Id,
                       // Code = c.Code,
                        Name = c.Name,
                        EquipTypeDefaultId = c.EquipTypeDefaultId,
                        Description = c.Description,
                    }).ToList();
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<ModelEquipmentType>(EquipmentTypes, pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ModelSelectItem> GetListEquipmentTypeDefault()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    List<ModelSelectItem> objs = new List<ModelSelectItem>();
                    objs.Add(new ModelSelectItem() { Value = 0, Name = "-- Chọn loại --" });
                    objs.AddRange(db.T_EquipType_Default.Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name }));
                    objs.Add(new ModelSelectItem() { Value = 0, Name = "Khác" });
                    return objs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
