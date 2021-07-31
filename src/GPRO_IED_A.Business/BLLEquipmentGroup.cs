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
    public class BLLEquipmentGroup
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLEquipmentGroup _Instance;
        public static BLLEquipmentGroup Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEquipmentGroup();

                return _Instance;
            }
        }
        private BLLEquipmentGroup() { }
        #endregion

        bool checkPermis(T_EquipmentGroup obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ResponseBase InsertOrUpdate(T_EquipmentGroup model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    bool flag = false;
                    if (CheckExists(model.GroupName.Trim().ToUpper(), model.Id, true))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Tên này đã được sử dụng. Vui lòng chọn lại Tên khác !." });
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(model.GroupCode))
                    {
                        if (CheckExists(model.GroupCode.Trim().ToUpper(), model.Id, false))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Mã này đã được sử dụng. Vui lòng chọn lại Mã khác !." });
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        T_EquipmentGroup obj = null;
                        if (model.Id == 0)
                        {
                            obj = new T_EquipmentGroup();
                            Parse.CopyObject(model, ref obj);
                            db.T_EquipmentGroup.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.T_EquipmentGroup.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.UpdatedUser.Value,isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo nhóm thiết bị này nên bạn không cập nhật được thông tin cho nhóm thiết bị này." });
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(model.Icon))
                                        obj.Icon = model.Icon;
                                    obj.GroupName = model.GroupName;
                                    obj.GroupCode = model.GroupCode;
                                    obj.Note = model.Note;
                                    obj.UpdatedUser = model.UpdatedUser;
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

        private bool CheckExists(string keyword, int id, bool isCheckName)
        {
            try
            {
                T_EquipmentGroup E_Group = null;
                keyword = keyword.Trim().ToUpper();
                if (isCheckName)
                    E_Group = db.T_EquipmentGroup.FirstOrDefault(x => !x.IsDeleted && x.Id != id && x.GroupName.Trim().ToUpper().Equals(keyword));
                else
                    E_Group = db.T_EquipmentGroup.FirstOrDefault(x => !x.IsDeleted && x.Id != id && x.GroupCode.Trim().ToUpper().Equals(keyword));


                if (E_Group == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase DeleteById(int id, int acctionUserId, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var obj = db.T_EquipmentGroup.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete  ", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(obj, acctionUserId,isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete Customer Type", Message = "Bạn không phải là người tạo nhóm thiết bị này nên bạn không xóa được xóa nhóm thiết bị này." });
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
         
        public PagedList<EquipmentGroupModel> GetList(string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<T_EquipmentGroup> E_Groups = null;
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    if (string.IsNullOrEmpty(keyWord))
                        E_Groups = db.T_EquipmentGroup.Where(x => !x.IsDeleted);
                    else
                        switch (searchBy)
                        {
                            case 1:
                                E_Groups = db.T_EquipmentGroup.Where(x => !x.IsDeleted && x.GroupName.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));
                                break;
                            case 2:
                                E_Groups = db.T_EquipmentGroup.Where(x => !x.IsDeleted && x.GroupCode.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));
                                break;
                        }
                    if (E_Groups != null && E_Groups.Count() > 0)
                    {
                        var returnList = E_Groups.Select(x => new EquipmentGroupModel()
                        {
                            Id = x.Id,
                            GroupCode = x.GroupCode,
                            GroupName = x.GroupName,
                            Icon = x.Icon,
                            Note = x.Note
                        }).OrderBy(sorting).ToList();
                        return new PagedList<EquipmentGroupModel>(returnList, pageNumber, pageSize);
                    }
                    else
                        return new PagedList<EquipmentGroupModel>(new List<EquipmentGroupModel>(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<ModelSelectItem> GetE_Group_Select()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var list = db.T_EquipmentGroup.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.GroupName }).ToList();
                    if (list.Count > 0)
                        return list;
                    return new List<ModelSelectItem>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
    }
}
