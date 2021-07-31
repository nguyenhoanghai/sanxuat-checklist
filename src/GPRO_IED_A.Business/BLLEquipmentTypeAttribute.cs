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

namespace SanXuatCheckList.Business
{
    public class BLLEquipmentTypeAttribute
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLEquipmentTypeAttribute _Instance;
        public static BLLEquipmentTypeAttribute Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEquipmentTypeAttribute();

                return _Instance;
            }
        }
        private BLLEquipmentTypeAttribute() { }
        #endregion

        private bool CheckEquipmentTypeAttributeName(string EquipmentTypeAttributeName, int Id)
        {
            var checkResult = false;
            try
            {
                var checkName = db.T_EquipmentTypeAttribute.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.Name.Trim().ToUpper().Equals(EquipmentTypeAttributeName.Trim().ToUpper()));
                if (checkName == null)
                    checkResult = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return checkResult;
        }

        public ResponseBase Create(ModelEquipmentTypeAttribute model)
        {
            ResponseBase rs = new ResponseBase();
            rs.IsSuccess = false;
            try
            {
                if (model != null)
                {
                    using (db = new SanXuatCheckListEntities())
                    {
                        if (CheckEquipmentTypeAttributeName(model.Name, model.Id))
                        {
                            if (CheckNumberOFEquipmentTypeAtt(model.EquipmentTypeId))
                            {
                                var obj = new T_EquipmentTypeAttribute();
                                Parse.CopyObject(model, ref obj);
                                obj.CreatedUser = model.ActionUser;
                                obj.CreatedDate = DateTime.Now;
                                db.T_EquipmentTypeAttribute.Add(obj);
                                db.SaveChanges();
                                rs.IsSuccess = true;
                            }
                            else
                            {
                                rs.IsSuccess = false;
                                rs.Errors.Add(new Error() { MemberName = "Create EquipmentTypeAttribute", Message = "Số Thuộc Tính Đã Tối Đa.Không Thể Thêm Nữa" });
                            }
                        }
                        else
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Create EquipmentTypeAttribute", Message = "Tên Đã Tồn Tại,Vui Lòng Chọn Tên Khác" });
                        }
                    }
                }
                else
                {
                    rs.IsSuccess = false;
                    rs.Errors.Add(new Error() { MemberName = "Create EquipmentTypeAttribute", Message = "Đối Tượng Không tồn tại" });
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                throw ex;
            }
            return rs;
        }

        public ResponseBase Update(ModelEquipmentTypeAttribute model)
        {

            ResponseBase result = new ResponseBase();
            result.IsSuccess = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (!CheckEquipmentTypeAttributeName(model.Name, model.Id))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "UpdateEquipmentTypeAttribute", Message = "Trùng Tên. Vui lòng chọn lại" });
                    }
                    else
                    {
                        T_EquipmentTypeAttribute obj = db.T_EquipmentTypeAttribute.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                        if (obj != null)
                        {
                            if (!obj.IsDefault)
                            {
                                obj.Name = model.Name;
                                obj.OrderIndex = model.OrderIndex;
                            }
                            obj.IsUseForTime = model.IsUseForTime;
                            obj.UpdatedDate = DateTime.Now;
                            obj.UpdatedUser = model.ActionUser;
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "UpdateEquipmentTypeAttribute", Message = "Thông tin nhập không đúng Vui lòng kiểm tra lại!" });
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
                    var obj = db.T_EquipmentTypeAttribute.FirstOrDefault(c => !c.IsDeleted && c.Id == id);
                    if (obj != null)
                    {
                        if (obj.IsDefault)
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Delete", Message = obj.Name + " là thuộc tính mặc Định của Loại Thiết bị này nên không thể xóa được." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = userId;
                            obj.DeletedDate = DateTime.Now;
                            db.SaveChanges();
                            rs.IsSuccess = true;
                        }
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

        public bool CheckNumberOFEquipmentTypeAtt(int equipmentTypeId)
        {
            var number = db.T_EquipmentTypeAttribute.Count(x => !x.IsDeleted && x.EquipmentTypeId == equipmentTypeId);
            if (number < 20)
                return true;
            return false;
        }

        public PagedList<ModelEquipmentTypeAttribute> GetList(int startIndexRecord, int pageSize, string sorting, int equipmentTypeId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    var objs = db.T_EquipmentTypeAttribute.Where(c => !c.IsDeleted && c.EquipmentTypeId == equipmentTypeId).OrderByDescending(x => x.CreatedDate).Select(c => new ModelEquipmentTypeAttribute()
                    {
                        Id = c.Id,
                        OrderIndex = c.OrderIndex,
                        IsUseForTime = c.IsUseForTime,
                        EquipmentTypeId = c.EquipmentTypeId,
                        EquipmentTypeName = c.T_EquipmentType.Name,
                        Name = c.Name,
                        IsDefault = c.IsDefault,
                    }).ToList();
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<ModelEquipmentTypeAttribute>(objs, pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelEquipmentTypeAttribute> GetEquipmentTypeAttributeByEquipmentTypeId(int equipmentTypeId)
        {
            List<ModelEquipmentTypeAttribute> equipmentTypeAttributes = null;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    equipmentTypeAttributes = db.T_EquipmentTypeAttribute.Where(c => !c.IsDeleted && c.EquipmentTypeId == equipmentTypeId).Select(c => new ModelEquipmentTypeAttribute()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        EquipmentTypeId = c.EquipmentTypeId,
                        EquipmentTypeName = c.T_EquipmentType.Name,
                        OrderIndex = c.OrderIndex,
                        IsDefault = c.IsDefault,
                        IsUseForTime = c.IsUseForTime,
                    }).OrderBy(x => x.OrderIndex).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return equipmentTypeAttributes;
        }

    }
}
