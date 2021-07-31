using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;

namespace SanXuatCheckList.Business
{
    public class BLLCommodityAnalysis
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLCommodityAnalysis _Instance;
        public static BLLCommodityAnalysis Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLCommodityAnalysis();

                return _Instance;
            }
        }
        private BLLCommodityAnalysis() { }
        #endregion
        bool checkPermis(T_CommodityAnalysis obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public CommodityAnalysisModel GetList()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var commoAnaModel = new CommodityAnalysisModel();
                    var commoAnalysis = (from x in db.T_CommodityAnalysis
                                         where !x.IsDeleted && x.ObjectType == (int)eObjectType.isCommodity
                                         select new ProAnaModel()
                                         {
                                             Id = x.Id,
                                             Name = x.Name,
                                             Node = x.Node,
                                             ObjectId = x.ObjectId,
                                             ObjectType = x.ObjectType,
                                             ParentId = x.ParentId,
                                             Description = x.Description,
                                             CreatedDate = x.CreatedDate
                                         });
                    if (commoAnalysis != null)
                    {
                        commoAnaModel.CommoAna.AddRange(commoAnalysis);
                        commoAnaModel.years.AddRange(commoAnalysis.Select(x => x.CreatedDate.Month).Distinct());
                    }
                    return commoAnaModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CommodityAnalysisModel GetCommoAnaItemByParentId(int parentId, string value, int Type, int companyId, int[] relationCompanyId, int year, int[] workshopIds)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var commoAnaModel = new CommodityAnalysisModel();
                    int value_ = 0;
                    switch (Type)
                    {
                        case (int)eObjectType.getYear:
                            commoAnaModel.years.AddRange((from x in db.T_CommodityAnalysis where !x.IsDeleted && (x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId)) && x.ObjectType == (int)eObjectType.isCommodity select x.CreatedDate.Year).Distinct());
                            break;
                        case (int)eObjectType.getMonth:
                            value_ = int.Parse(value);
                            commoAnaModel.years.AddRange((from x in db.T_CommodityAnalysis where !x.IsDeleted && (x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId)) && x.ObjectType == (int)eObjectType.isCommodity && x.CreatedDate.Year == value_ select x.CreatedDate.Month).Distinct());
                            break;
                        case (int)eObjectType.isCommodity:
                            value_ = int.Parse(value);
                            commoAnaModel.CommoAna.AddRange((from x in db.T_CommodityAnalysis
                                                             where !x.IsDeleted && (x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId)) && x.ObjectType == (int)eObjectType.isCommodity && x.CreatedDate.Year == year && x.CreatedDate.Month == value_
                                                             select new ProAnaModel()
                                                             {
                                                                 Id = x.Id,
                                                                 Name = x.Name,
                                                                 Node = x.Node,
                                                                 ObjectId = x.ObjectId,
                                                                 ObjectType = x.ObjectType,
                                                                 ParentId = x.ParentId,
                                                                 Description = x.Description,
                                                                 CreatedDate = x.CreatedDate
                                                             }));
                            break;
                        case (int)eObjectType.isWorkShop:
                            if (workshopIds != null && workshopIds.Length > 0)
                                commoAnaModel.CommoAna.AddRange((from x in db.T_CommodityAnalysis
                                                                 where !x.IsDeleted
                                                                 && x.ParentId == parentId
                                                                 && workshopIds.Contains(x.ObjectId)
                                                                 select new ProAnaModel()
                                                                 {
                                                                     Id = x.Id,
                                                                     Name = x.Name,
                                                                     Node = x.Node,
                                                                     ObjectId = x.ObjectId,
                                                                     ObjectType = x.ObjectType,
                                                                     ParentId = x.ParentId,
                                                                     Description = x.Description,
                                                                     CreatedDate = x.CreatedDate,
                                                                 }));
                            break;
                        case (int)eObjectType.isComponent:
                        case (int)eObjectType.isGroupVersion:
                        case (int)eObjectType.isPhaseGroup:
                            commoAnaModel.CommoAna.AddRange((from x in db.T_CommodityAnalysis
                                                             where !x.IsDeleted && x.ParentId == parentId
                                                             select new ProAnaModel()
                                                             {
                                                                 Id = x.Id,
                                                                 Name = x.Name,
                                                                 Node = x.Node,
                                                                 ObjectId = x.ObjectId,
                                                                 ObjectType = x.ObjectType,
                                                                 ParentId = x.ParentId,
                                                                 Description = x.Description,
                                                                 CreatedDate = x.CreatedDate,
                                                             }));
                            break;
                    }

                    if (commoAnaModel.CommoAna.Count > 0)
                    {
                        switch (commoAnaModel.CommoAna.First().ObjectType)
                        {
                            case (int)eObjectType.isCommodity:
                                var commoIds = commoAnaModel.CommoAna.Select(x => x.ObjectId);
                                var commoCheck = db.T_Product.Where(x => !x.IsDeleted && commoIds.Contains(x.Id)).Select(x => x.Id);
                                commoAnaModel.CommoAna = commoAnaModel.CommoAna.Where(x => commoCheck.Contains(x.ObjectId)).ToList();
                                break;
                            case (int)eObjectType.isWorkShop:
                                var _workshopIds = commoAnaModel.CommoAna.Select(x => x.ObjectId);
                                var workshopCheck = db.T_WorkShop.Where(x => !x.IsDeleted && _workshopIds.Contains(x.Id)).Select(x => x.Id);
                                commoAnaModel.CommoAna = commoAnaModel.CommoAna.Where(x => workshopCheck.Contains(x.ObjectId)).ToList();
                                break;
                            case (int)eObjectType.isPhaseGroup:
                                var phaseGroupIds = commoAnaModel.CommoAna.Select(x => x.ObjectId);
                                var phaseGroupCheck = db.T_PhaseGroup.Where(x => !x.IsDeleted && phaseGroupIds.Contains(x.Id)).Select(x => x.Id);
                                commoAnaModel.CommoAna = commoAnaModel.CommoAna.Where(x => phaseGroupCheck.Contains(x.ObjectId)).ToList();

                                break;
                        }
                    }
                    return commoAnaModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(T_CommodityAnalysis noNameModel, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    T_CommodityAnalysis noName = null;
                    if (CheckObjectExists(noNameModel.ObjectId, noNameModel.ObjectType, noNameModel.ParentId))
                    {
                        result.IsSuccess = false;
                        switch (noNameModel.ObjectType)
                        {
                            case (int)eObjectType.isCommodity:
                                result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Mã hàng này đã có bài phân tích. Vui lòng chọn lại Mã hàng khác !." });
                                break;
                            case (int)eObjectType.isWorkShop:
                                result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Phân xưởng này đã được chọn. Vui lòng chọn lại Phân xưởng khác !." });
                                break;
                            case (int)eObjectType.isPhaseGroup:
                                result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Nhóm Công Đoạn này đã được chọn. Vui lòng chọn lại Nhóm Công Đoạn khác !." });
                                break;
                        }
                    }
                    else
                    {
                        if (noNameModel.Id == 0)
                        {
                            using (TransactionScope scope = new TransactionScope())
                            {
                                noName = new T_CommodityAnalysis();
                                Parse.CopyObject(noNameModel, ref noName);
                                noName.Node = noName.Node == "0" ? noName.Id.ToString() : noName.Node + noName.ParentId + ",";

                                if (noName.ObjectType == (int)eObjectType.isVersion)
                                {
                                    noName.ObjectId = FindLastedVersion(noName.ParentId, noName.Node);
                                }
                                db.T_CommodityAnalysis.Add(noName);
                                db.SaveChanges();
                                if (noName.ObjectType == (int)eObjectType.isWorkShop)
                                {
                                    T_CommodityAnalysis newObject;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        newObject = new T_CommodityAnalysis();
                                        newObject.Id = 0;
                                        newObject.Node = noName.Node + noName.Id + ",";
                                        newObject.ParentId = noName.Id;
                                        newObject.ObjectId = 0;
                                        switch (i)
                                        {
                                            case 0:
                                                newObject.Name = "Quy trình công nghệ";
                                                newObject.ObjectType = (int)eObjectType.isGroupVersion;
                                                break;
                                            case 1:
                                                newObject.Name = "Thiết kế chuyền";
                                                newObject.ObjectType = (int)eObjectType.isLabourDivision;
                                                break;
                                            case 2:
                                                newObject.Name = "Thành Phần";
                                                newObject.ObjectType = (int)eObjectType.isComponent;
                                                break;
                                        }
                                        newObject.Description = "";
                                        newObject.CreatedUser = noName.CreatedUser;
                                        newObject.CreatedDate = noName.CreatedDate;
                                        db.T_CommodityAnalysis.Add(newObject);
                                    }
                                }
                                db.SaveChanges();
                                scope.Complete();
                                result.IsSuccess = true;
                            }
                        }
                        else
                        {
                            //Update                     
                            noName = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Id == noNameModel.Id);
                            if (noName == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update  ", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(noName, noNameModel.UpdatedUser.Value, isOwner))
                                {
                                    result.IsSuccess = false;
                                    switch (noName.ObjectType)
                                    {
                                        case (int)eObjectType.isCommodity:
                                            result.Errors.Add(new Error() { MemberName = "update  ", Message = "Bạn không phải là người tạo mã hàng này nên bạn không cập nhật được thông tin cho mã hàng này." });
                                            break;
                                        case (int)eObjectType.isWorkShop:
                                            result.Errors.Add(new Error() { MemberName = "update  ", Message = "Bạn không phải là người tạo phân xưởng này nên bạn không cập nhật được thông tin cho phân xưởng này." });
                                            break;
                                        case (int)eObjectType.isPhaseGroup:
                                            result.Errors.Add(new Error() { MemberName = "update  ", Message = "Bạn không phải là người tạo nhóm công đoạn này nên bạn không cập nhật được thông tin cho nhóm công đoạn này." });
                                            break;
                                    }
                                }
                                else
                                {
                                    if (noName.ObjectType == (int)eObjectType.isPhaseGroup && noName.Description != noNameModel.Description)
                                    {
                                        var phases = db.T_CA_Phase.Where(x => !x.IsDeleted && x.ParentId == noName.Id);
                                        if (phases != null && phases.Count() > 0)
                                            foreach (var item in phases)
                                                item.Code = noNameModel.Description == null || noNameModel.Description == "" ? item.Index.ToString() : noNameModel.Description + "-" + item.Index;

                                    }
                                    noName.Name = noNameModel.Name;
                                    noName.Description = noNameModel.Description;
                                    noName.UpdatedUser = noNameModel.UpdatedUser;
                                    noName.UpdatedDate = noNameModel.UpdatedDate;
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

        private bool CheckObjectExists(int objectId, int objectType, int parentId)
        {
            try
            {
                T_CommodityAnalysis noName = null;
                noName = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.ObjectId == objectId && x.ObjectType == objectType && x.ParentId == parentId);
                if (noName == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int FindLastedVersion(int parentId, string Node)
        {
            try
            {
                var noName = db.T_CommodityAnalysis.Where(x => x.ParentId == parentId && x.Node.Trim().Equals(Node.Trim())).OrderByDescending(x => x.ObjectId);
                if (noName != null && noName.Count() > 0)
                    return noName.FirstOrDefault().ObjectId + 1;
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Delete(int Id, int actionUserId, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var commoAna = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                    if (commoAna != null)
                    {
                        if (!checkPermis(commoAna, actionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            switch (commoAna.ObjectType)
                            {
                                case (int)eObjectType.isCommodity:
                                    result.Errors.Add(new Error() { MemberName = "delete  ", Message = "Bạn không phải là người tạo mã hàng này nên bạn không xóa được xóa mã hàng này." });
                                    break;
                                case (int)eObjectType.isWorkShop:
                                    result.Errors.Add(new Error() { MemberName = "delete  ", Message = "Bạn không phải là người tạo phân xưởng này nên bạn không xóa được xóa phân xưởng này." });
                                    break;
                                case (int)eObjectType.isPhaseGroup:
                                    result.Errors.Add(new Error() { MemberName = "delete  ", Message = "Bạn không phải là người tạo nhóm công đoạn này nên bạn không xóa được xóa nhóm công đoạn này." });
                                    break;
                            }
                        }
                        else
                        {
                            commoAna.IsDeleted = true;
                            commoAna.DeletedUser = actionUserId;
                            commoAna.DeletedDate = DateTime.Now;
                            var node = commoAna.Node + Id + ",";
                            var childCommoAna = db.T_CommodityAnalysis.Where(x => !x.IsDeleted && x.Node.Trim().ToUpper().Contains(node.Trim().ToUpper()));
                            if (childCommoAna != null && childCommoAna.Count() > 0)
                            {
                                foreach (var item in childCommoAna)
                                {
                                    item.IsDeleted = true;
                                    item.DeletedUser = actionUserId;
                                    item.DeletedDate = DateTime.Now;
                                }
                            }

                            if (commoAna.ObjectType == (int)eObjectType.isPhaseGroup)
                            {
                                var phases = db.T_CA_Phase.Where(x => !x.IsDeleted && x.ParentId == commoAna.Id);
                                if (phases != null && phases.Count() > 0)
                                {
                                    foreach (var item in phases)
                                    {
                                        item.IsDeleted = true;
                                        item.DeletedUser = actionUserId;
                                        item.DeletedDate = DateTime.Now;
                                    }
                                }
                            }
                            db.SaveChanges();
                            result.IsSuccess = true;
                            result.Errors.Add(new Error() { MemberName = "", Message = "Xóa Thành Công.!" });
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại.\nVui lòng kiểm tra lại.!" });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Ktra lai
        public ResponseBase Copy_CommoAnaPhaseGroup(int CopyObjectId, int ObjectId, int actionUserId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var commoAna = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Id == ObjectId);
                    if (commoAna != null)
                    {
                        //ktra object copy có còn tồn tại hay không
                        var objCopy = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Id == CopyObjectId);
                        if (objCopy != null)
                        {

                            // ktra coi mat hag dang phan tich co tồn tại nhóm công đoạn này chưa
                            var node = (commoAna.Node + commoAna.Id + ",").Trim();
                            var objExists = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Node.Trim().Equals(node) && x.ObjectId == objCopy.ObjectId);
                            if (objExists != null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "", Message = "Mã hàng Bạn đang Phân Tích đã Tồn Tại Nhóm Công Đoạn này rồi.\nVui lòng kiểm tra lại.!" });
                            }
                            else
                            {
                                using (TransactionScope scope = new TransactionScope())
                                {
                                    //step 1  -  copy phase group
                                    var new_commoAna = new T_CommodityAnalysis();
                                    new_commoAna.Name = objCopy.Name;
                                    new_commoAna.ObjectType = objCopy.ObjectType;
                                    new_commoAna.ObjectId = objCopy.ObjectId;
                                    new_commoAna.ParentId = commoAna.Id;
                                    new_commoAna.Node = node;
                                    new_commoAna.Description = objCopy.Description;
                                    new_commoAna.CreatedUser = actionUserId;
                                    new_commoAna.CreatedDate = DateTime.Now;
                                    db.T_CommodityAnalysis.Add(new_commoAna);
                                    db.SaveChanges();
                                    var parentid = new_commoAna.Id;
                                    //step 2  - copy phase
                                    var commo_ana_phases = db.T_CA_Phase.Where(x => !x.IsDeleted && x.ParentId == objCopy.Id).OrderBy(x => x.CreatedDate);
                                    if (commo_ana_phases != null && commo_ana_phases.Count() > 0)
                                    {
                                        foreach (var item in commo_ana_phases)
                                        {
                                            var new_commoAnaPhase = new T_CA_Phase();
                                            new_commoAnaPhase.Index = item.Index;
                                            new_commoAnaPhase.Name = item.Name;
                                            new_commoAnaPhase.Code = item.Code;
                                            new_commoAnaPhase.PhaseGroupId = item.PhaseGroupId;
                                            new_commoAnaPhase.Description = item.Description;
                                            new_commoAnaPhase.EquipmentId = item.EquipmentId;
                                            new_commoAnaPhase.PhaseGroupId = item.PhaseGroupId;
                                            new_commoAnaPhase.WorkerLevelId = item.WorkerLevelId;
                                            new_commoAnaPhase.ParentId = item.ParentId;
                                            new_commoAnaPhase.TotalTMU = item.TotalTMU;
                                            new_commoAnaPhase.ApplyPressuresId = item.ApplyPressuresId;
                                            new_commoAnaPhase.PercentWasteEquipment = item.PercentWasteEquipment;
                                            new_commoAnaPhase.PercentWasteManipulation = item.PercentWasteManipulation;
                                            new_commoAnaPhase.PercentWasteMaterial = item.PercentWasteMaterial;
                                            new_commoAnaPhase.PercentWasteSpecial = item.PercentWasteSpecial;
                                            new_commoAnaPhase.Node = item.Node;
                                            new_commoAnaPhase.Video = item.Video;
                                            new_commoAnaPhase.CreatedUser = actionUserId;
                                            new_commoAnaPhase.CreatedDate = new_commoAna.CreatedDate;
                                            // step 3 - copy active timeprepare                                            
                                            var listTimePrepareExist = db.T_CA_Phase_TimePrepare.Where(c => c.Commo_Ana_PhaseId == item.Id && !c.IsDeleted).ToList();
                                            if (listTimePrepareExist.Count > 0)
                                            {
                                                var listTimePrepareNew = new Collection<T_CA_Phase_TimePrepare>();
                                                foreach (var timePrepare in listTimePrepareExist)
                                                {
                                                    listTimePrepareNew.Add(new T_CA_Phase_TimePrepare()
                                                    {
                                                        Commo_Ana_PhaseId = item.Id,
                                                        TimePrepareId = timePrepare.TimePrepareId,
                                                        CreatedUser = actionUserId,
                                                        CreatedDate = new_commoAnaPhase.CreatedDate
                                                    });
                                                }
                                                new_commoAnaPhase.T_CA_Phase_TimePrepare = listTimePrepareNew;
                                            }
                                            //check         // step 4  - copy active manipulation version
                                            var phaseAcc = db.T_CA_Phase_Mani.Where(x => !x.IsDeleted && x.CA_PhaseId == item.Id).ToList();
                                            if (phaseAcc != null && phaseAcc.Count() > 0)
                                            {
                                                new_commoAnaPhase.T_CA_Phase_Mani = new Collection<T_CA_Phase_Mani>();
                                                foreach (var acc in phaseAcc)
                                                {
                                                    var maniC = new T_CA_Phase_Mani();
                                                    maniC.OrderIndex = acc.OrderIndex;
                                                    maniC.ManipulationId = acc.ManipulationId;
                                                    maniC.ManipulationCode = acc.ManipulationCode;
                                                    maniC.ManipulationName = acc.ManipulationName;
                                                    maniC.TMUEquipment = acc.TMUEquipment;
                                                    maniC.TMUManipulation = acc.TMUManipulation;
                                                    maniC.Loop = acc.Loop;
                                                    maniC.TotalTMU = acc.TotalTMU;
                                                    maniC.CreatedUser = actionUserId;
                                                    maniC.CreatedDate = new_commoAnaPhase.CreatedDate;
                                                    maniC.T_CA_Phase = new_commoAnaPhase;
                                                    new_commoAnaPhase.T_CA_Phase_Mani.Add(maniC);
                                                }
                                            }

                                            new_commoAnaPhase.T_CommodityAnalysis = new_commoAna;
                                            // new_commoAnaPhase.ParentId = parentid;
                                            new_commoAnaPhase.Node = new_commoAna.Node + new_commoAnaPhase.T_CommodityAnalysis.Id + ",";
                                            new_commoAnaPhase.CreatedUser = actionUserId;
                                            new_commoAnaPhase.CreatedDate = new_commoAna.CreatedDate;
                                            db.T_CA_Phase.Add(new_commoAnaPhase);
                                            db.SaveChanges();
                                        }
                                    }
                                    scope.Complete();
                                    result.IsSuccess = true;
                                }
                            }

                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại.\nVui lòng kiểm tra lại.!" });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CommodityAnalysisModel GetProductByCustomerId(int customerId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var productIds = db.T_Product.Where(x => !x.IsDeleted && !x.T_Customer.IsDeleted && x.CustomerId == customerId).Select(x => x.Id).ToList();
                    
                    var commoAnaModel = new CommodityAnalysisModel();
                    var commoAnalysis = (from x in db.T_CommodityAnalysis
                                         where
                                         !x.IsDeleted &&
                                         x.ObjectType == (int)eObjectType.isCommodity &&
                                         productIds.Contains(x.ObjectId)
                                         select new ProAnaModel()
                                         {
                                             Id = x.Id,
                                             Name = x.Name,
                                             Node = x.Node,
                                             ObjectId = x.ObjectId,
                                             ObjectType = x.ObjectType,
                                             ParentId = x.ParentId,
                                             Description = x.Description,
                                             CreatedDate = x.CreatedDate
                                         });
                    if (commoAnalysis != null)
                    {
                        commoAnaModel.CommoAna.AddRange(commoAnalysis);
                        commoAnaModel.years.AddRange(commoAnalysis.Select(x => x.CreatedDate.Month).Distinct());
                    }
                    return commoAnaModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
