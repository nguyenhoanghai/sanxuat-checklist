using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using Hugate.Framework;

namespace SanXuatCheckList.Business
{
    public class BLLPhaseGroup
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLPhaseGroup _Instance;
        public static BLLPhaseGroup Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLPhaseGroup();

                return _Instance;
            }
        }
        private BLLPhaseGroup() { }
        #endregion

        bool checkPermis(T_PhaseGroup obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<PhaseGroupModel> GetList(string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting, int[] workshopIds)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    PagedList<PhaseGroupModel> pgsReturn = null;
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "Id DESC";
                    }
                    IQueryable<T_PhaseGroup> phaseGroups = null;
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        switch (searchBy)
                        {
                            case 1:
                                phaseGroups = db.T_PhaseGroup.Where(c => !c.IsDeleted && c.Name.Trim().ToUpper().Contains(keyWord));
                                break;
                            case 2:
                                phaseGroups = db.T_PhaseGroup.Where(c => !c.IsDeleted && c.Code.Trim().ToUpper().Contains(keyWord));
                                break;
                        }
                    }
                    else
                        phaseGroups = db.T_PhaseGroup.Where(c => !c.IsDeleted);

                    if (phaseGroups != null && phaseGroups.Count() > 0)
                    {
                        var list = phaseGroups .Select(x => new PhaseGroupModel()
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            MinLevel = x.MinLevel,
                            MaxLevel = x.MaxLevel,
                            Description = x.Description,
                            WorkshopIds = x.WorkshopIds
                        }).OrderBy(sorting).ToList();
                        var newList = new List<PhaseGroupModel>();
                        foreach (var item in list)
                        {
                            if (string.IsNullOrEmpty(item.WorkshopIds))
                                newList.Add(item);
                            else
                            {
                                if (workshopIds.Length > 0)
                                {
                                    for (int i = 0; i < workshopIds.Length; i++)
                                    {
                                        if (item.WorkshopIds.Contains((workshopIds[i] + ",")) ||
                                            item.WorkshopIds.Contains(("," + workshopIds[i])) ||
                                            (item.WorkshopIds.IndexOf(',') < 0 && item.WorkshopIds.Contains(workshopIds[i].ToString())))
                                        {
                                            newList.Add(item);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        pgsReturn = new PagedList<PhaseGroupModel>(newList, pageNumber, pageSize);
                    }
                    else
                        pgsReturn = new PagedList<PhaseGroupModel>(new List<PhaseGroupModel>(), pageNumber, pageSize);


                    if (pgsReturn != null && pgsReturn.Count > 0)
                    {
                        var wks = db.T_WorkShop.Where(x => !x.IsDeleted).ToList();
                        if (wks != null && wks.Count() > 0)
                        {
                            foreach (var item in pgsReturn)
                            {
                                item.WorkshopNames = "";
                                if (item.WorkshopIds != null)
                                {
                                    item.intWorkshopIds = item.WorkshopIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                                    for (int i = 0; i < item.intWorkshopIds.Count; i++)
                                    {
                                        var f = wks.FirstOrDefault(x => x.Id == item.intWorkshopIds[i]);
                                        if (f != null)
                                        {
                                            item.WorkshopNames += f.Name;
                                            if (i < (item.intWorkshopIds.Count - 1))
                                                item.WorkshopNames += " ; ";
                                        }
                                    }
                                }
                                else
                                    item.intWorkshopIds = new List<int>();
                            }
                        }
                    }
                    return pgsReturn;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(PhaseGroupModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    bool flag = false;
                    T_PhaseGroup obj = null;
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, true))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên Cụm Công Đoạn này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(model.Code))
                    {
                        if (CheckExists(model.Code.Trim().ToUpper(), model.Id, false))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Insert", Message = "Mã Cụm Công Đoạn này đã tồn tại. Vui lòng chọn lại Mã khác !." });
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        if (model.Id == 0)
                        {
                            obj = new T_PhaseGroup();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.T_PhaseGroup.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.T_PhaseGroup.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo Cụm Công Đoạn này nên bạn không cập nhật được thông tin cho Cụm Công Đoạn này." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Code = model.Code;
                                    obj.MinLevel = model.MinLevel;
                                    obj.MaxLevel = model.MaxLevel;
                                    obj.Description = model.Description;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;
                                    obj.WorkshopIds = model.WorkshopIds;

                                    //  cap nhat ben phan tich mat hang
                                    var commoAna = db.T_CommodityAnalysis.Where(x => !x.IsDeleted && x.ObjectId == obj.Id && x.ObjectType == (int)eObjectType.isPhaseGroup);
                                    if (commoAna != null && commoAna.Count() > 0)
                                    {
                                        foreach (var item in commoAna)
                                        {
                                            item.Name = model.Name;
                                            item.UpdatedUser = model.ActionUser;
                                            item.UpdatedDate = DateTime.Now;

                                        }
                                    }
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

        private bool CheckExists(string keyword, int PhaseGroupId, bool isCheckName)
        {
            try
            {
                T_PhaseGroup phaseGroup = null;
                if (isCheckName)
                    phaseGroup = db.T_PhaseGroup.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(keyword) && x.Id != PhaseGroupId);
                else
                    phaseGroup = db.T_PhaseGroup.FirstOrDefault(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(keyword) && x.Id != PhaseGroupId);
                if (phaseGroup == null)
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
                    var phasegroup = db.T_PhaseGroup.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (phasegroup == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(phasegroup, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo Cụm Công Đoạn này nên bạn không xóa được Cụm Công Đoạn này." });
                        }
                        else
                        {
                            phasegroup.IsDeleted = true;
                            phasegroup.DeletedUser = acctionUserId;
                            phasegroup.DeletedDate = DateTime.Now;
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

        public List<ModelSelectItem> Gets(int[] workshopIds)
        {
            using (db = new SanXuatCheckListEntities())
            {
                List<ModelSelectItem> objs = new List<ModelSelectItem>();
                objs.Add(new ModelSelectItem() { Value = 0, Name = " - Chọn Cụm Công Đoạn - " });
                try
                {
                    var pgs = db.T_PhaseGroup.Where(x => !x.IsDeleted)
                        .Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name, Code = x.WorkshopIds }).ToList();

                    foreach (var item in pgs)
                    {
                        if (string.IsNullOrEmpty(item.Code))
                            objs.Add(item);
                        else
                        {
                            if (workshopIds.Length > 0)
                            {
                                for (int i = 0; i < workshopIds.Length; i++)
                                {
                                    if (item.Code.Contains((workshopIds[i] + ",")) ||
                                        item.Code.Contains(("," + workshopIds[i])) ||
                                        (item.Code.IndexOf(',') < 0 && item.Code.Contains(workshopIds[i].ToString())))
                                    {
                                        objs.Add(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
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
