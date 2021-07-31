using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLTechProcessVersion
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLTechProcessVersion _Instance;
        public static BLLTechProcessVersion Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLTechProcessVersion();

                return _Instance;
            }
        }
        private BLLTechProcessVersion() { }
        #endregion

        bool checkPermis(T_TechProcessVersion obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ExportTechProcessModel GetInfoForExport(int parentId)
        {
            ExportTechProcessModel model = null;
            try
            {
                var techProcessInfo = Get(parentId, "");
                if (techProcessInfo != null)
                {
                    using (db = new SanXuatCheckListEntities())
                    {
                        model = new ExportTechProcessModel();
                        Parse.CopyObject(techProcessInfo, ref model);
                        var listDetail = techProcessInfo.details;
                        if (listDetail != null && listDetail.Count > 0)
                        {
                            var listPhaseId = listDetail.OrderBy(x => x.PhaseCode).Select(c => c.CA_PhaseId).OrderBy(x => x).ToList();
                            var listPhase = db.T_CA_Phase.Where(c => !c.IsDeleted && listPhaseId.Contains(c.Id)).OrderBy(x => x.ParentId).ThenBy(x=>x.Code).ToList();
                            if (listPhase != null)
                            {
                                var listPhaseGroupId = listPhase.Select(c => c.PhaseGroupId).Distinct().ToList();
                                if (listPhaseGroupId != null && listPhaseGroupId.Count > 0)
                                {
                                    model.ListTechProcessGroup = new List<TechProcessVerDetailGroupModel>();
                                    var hasDelete = false;
                                    foreach (var id in listPhaseGroupId)
                                    {
                                        var phaseGroup = db.T_PhaseGroup.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
                                        var phases = listPhase.Where(c => c.PhaseGroupId == id);

                                        if (phases != null)
                                        {
                                            var phaseFirst = phases.First();
                                            var listPhaseIdOfGroup = phases.Select(c => c.Id).ToList();
                                            var techProcessGroup = new TechProcessVerDetailGroupModel();
                                            techProcessGroup.PhaseGroupId = id;
                                            if (phaseGroup != null)
                                            {
                                                techProcessGroup.PhaseGroupName = phaseGroup.Name;
                                                techProcessGroup.ListTechProcessVerDetail = listDetail.Where(c => listPhaseIdOfGroup.Contains(c.CA_PhaseId)).ToList();
                                                model.ListTechProcessGroup.Add(techProcessGroup);
                                            }
                                            else
                                            {
                                                //xoa neu cha bi xoa
                                                var ids = listDetail.Where(c => listPhaseIdOfGroup.Contains(c.CA_PhaseId)).Select(x => x.Id).ToList();
                                                if (ids != null && ids.Count > 0)
                                                {
                                                    var details = db.T_TechProcessVersionDetail.Where(x => ids.Contains(x.Id));
                                                    if (details != null && details.Count() > 0)
                                                    {
                                                        foreach (var item in details)
                                                        {
                                                            item.IsDeleted = true;
                                                            hasDelete = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (hasDelete)
                                        db.SaveChanges();
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
            return model;
        }

        public ResponseBase InsertOrUpdate(TechProcessVersionModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    T_TechProcessVersion version = null;
                    T_TechProcessVersionDetail verDetail = null;
                    model.Id = 0;
                    if (model.Id == 0)
                    {
                        version = new T_TechProcessVersion();
                        Parse.CopyObject(model, ref version);
                        version.CreatedDate = DateTime.Now;
                        version.CreatedUser = model.ActionUser;

                        if (model.details != null && model.details.Count > 0)
                        {
                            version.T_TechProcessVersionDetail = new Collection<T_TechProcessVersionDetail>();
                            foreach (var item in model.details)
                            {
                                verDetail = new T_TechProcessVersionDetail();
                                Parse.CopyObject(item, ref verDetail);
                                verDetail.T_TechProcessVersion = version;
                                verDetail.CreatedDate = version.CreatedDate;
                                verDetail.CreatedUser = version.CreatedUser;
                                version.T_TechProcessVersionDetail.Add(verDetail);
                            }
                        }
                        string query = "update T_TechProcessVersion set IsDeleted = 1 WHERE ParentId =" + version.ParentId;
                        db.Database.ExecuteSqlCommand(query);
                        db.T_TechProcessVersion.Add(version);
                    }
                    else
                    {
                        #region Update
                        version = db.T_TechProcessVersion.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                        if (version == null)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Update", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                            return result;
                        }
                        else
                        {
                            if (!checkPermis(version, model.ActionUser, isOwner))
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo quy trình công nghệ này nên bạn không cập nhật được thông tin cho quy trình công nghệ này." });
                            }
                            else
                            {
                                version.NumberOfWorkers = model.NumberOfWorkers;
                                version.WorkingTimePerDay = model.WorkingTimePerDay;
                                version.PacedProduction = model.PacedProduction;
                                version.TimeCompletePerCommo = model.TimeCompletePerCommo;
                                version.ProOfGroupPerDay = model.ProOfGroupPerDay;
                                version.ProOfGroupPerHour = model.ProOfGroupPerHour;
                                version.ProOfPersonPerDay = model.ProOfPersonPerDay;
                                version.Note = model.Note;
                                version.UpdatedUser = model.ActionUser;
                                version.UpdatedDate = DateTime.Now;
                                version.PricePerSecond = model.PricePerSecond;
                                version.Allowance = model.Allowance;

                                var details = db.T_TechProcessVersionDetail.Where(x => !x.IsDeleted && x.TechProcessVersionId == model.Id).OrderBy(x => x.Id).ToList();
                                if (details.Count > 0)
                                {
                                    model.details = model.details.OrderBy(x => x.Id).ToList();
                                    for (int i = 0; i < details.Count(); i++)
                                    {
                                        details[i].Percent = model.details[i].Percent;
                                        details[i].TimeByPercent = model.details[i].TimeByPercent;
                                        details[i].Worker = model.details[i].Worker;
                                        details[i].Description = model.details[i].Description;
                                        details[i].UpdatedUser = model.ActionUser;
                                        details[i].UpdatedDate = DateTime.Now;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    db.SaveChanges();
                    result.Data = version.Id;
                    result.IsSuccess = true;

                    return result;
                }
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
                    var version = db.T_TechProcessVersion.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (version == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(version, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo quy trình công nghệ này nên bạn không xóa được quy trình công nghệ này." });
                        }
                        else
                        {
                            version.IsDeleted = true;
                            version.DeletedUser = acctionUserId;
                            version.DeletedDate = DateTime.Now;
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

        public TechProcessVersionModel Get(int parentId, string node)
        {
            try
            {
                using (var _db = new SanXuatCheckListEntities())
                {
                    TechProcessVersionModel techVersion = null;
                    techVersion = _db.T_TechProcessVersion.Where(x => !x.IsDeleted && x.ParentId == parentId).Select(x => new TechProcessVersionModel()
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductName = x.T_Product.Name,
                        TimeCompletePerCommo = x.TimeCompletePerCommo,
                        NumberOfWorkers = x.NumberOfWorkers,
                        WorkingTimePerDay = x.WorkingTimePerDay,
                        PacedProduction = x.PacedProduction,
                        ProOfGroupPerDay = x.ProOfGroupPerDay,
                        ProOfGroupPerHour = x.ProOfGroupPerHour,
                        ProOfPersonPerDay = x.ProOfPersonPerDay,
                        //WorkshopId = x.WorkshopId,
                        //WorkShopName = x.T_WorkShop.Name,
                        Note = x.Note,
                        Quantities = 0,
                        Price = 0,
                        PricePerSecond = x.PricePerSecond,
                        Allowance = x.Allowance,
                        CustomerName = x.T_Product.T_Customer.Name
                    }).FirstOrDefault();
                    if (techVersion != null)
                    {
                        #region 
                        var details = (from x in _db.T_TechProcessVersionDetail
                                       where
                                       !x.T_CA_Phase.IsDeleted &&
                                       x.TechProcessVersionId == techVersion.Id
                                       select
                                       new TechProcessVerDetailModel()
                                       {
                                           Id = x.Id,
                                           TechProcessVersionId = x.TechProcessVersionId,
                                           PhaseGroupId = x.T_CA_Phase.T_PhaseGroup.Id,
                                           CA_PhaseId = x.CA_PhaseId,
                                           PhaseCode = x.T_CA_Phase.Code,
                                           PhaseName = x.T_CA_Phase.Name,
                                           //   StandardTMU = Math.Round(x.StandardTMU, 3),
                                           StandardTMU = Math.Round(x.T_CA_Phase.TotalTMU, 3),
                                           Percent = x.Percent,
                                           EquipmentId = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.EquipmentId ?? 0 : 0,
                                           EquipmentCode = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.T_Equipment.Code : "",
                                           EquipmentName = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.T_Equipment.Name : "",
                                           EquipmentGroupCode = x.T_CA_Phase.EquipmentId.HasValue ? x.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupCode : "",
                                           TimeByPercent = Math.Round(x.TimeByPercent, 3),
                                           Worker = x.Worker,
                                           De_Percent = 0,
                                           Description = x.Description == null ? "" : x.Description,
                                           Coefficient = x.T_CA_Phase.SWorkerLevel.Coefficient,
                                           WorkerLevelId = x.T_CA_Phase.WorkerLevelId,
                                           WorkerLevelName = x.T_CA_Phase.SWorkerLevel.Name,
                                           Index = x.T_CA_Phase.Index
                                       }).OrderBy(x => x.Index).ThenBy(x => x.PhaseCode).ToList();

                        details = details.GroupBy(x => x.CA_PhaseId).Select(x => x.First()).ToList();
                        techVersion.details = details;

                        var listEquipmentId = details.Where(c => c.EquipmentId > 0).Select(c => c.EquipmentId).Distinct().ToList();
                        if (listEquipmentId.Count > 0)
                        {
                            techVersion.equipments = new List<ModelEquipment>();
                            foreach (var equipmentId in listEquipmentId)
                            {
                                var equipments = details.Where(c => c.EquipmentId == equipmentId).Select(c => new ModelEquipment()
                                {
                                    Id = c.EquipmentId.Value,
                                    Name = c.EquipmentName,
                                    Code = c.EquipmentCode
                                }).ToList();
                                if (equipments.Count > 0)
                                {
                                    var equipmentFirst = equipments[0];
                                    equipmentFirst.QuantityUse = equipments.Count;
                                    techVersion.equipments.Add(equipmentFirst);
                                }
                            }
                        }
                        var phaseIds = details.Select(x => x.CA_PhaseId).Distinct().ToList();
                        if (phaseIds != null && phaseIds.Count > 0)
                        {
                            var timePrepares = _db.T_CA_Phase_TimePrepare.Where(x => !x.IsDeleted && phaseIds.Contains(x.Commo_Ana_PhaseId)).Select(x => new Commo_Ana_Phase_TimePrepareModel()
                            {
                                Id = x.Id,
                                Commo_Ana_PhaseId = x.Commo_Ana_PhaseId,
                                TMUNumber = x.T_TimePrepare.TMUNumber
                            }).ToList();
                            if (timePrepares != null && timePrepares.Count > 0)
                            {
                                double tmu = 0, time = 0;
                                var cfObj = _db.T_IEDConfig.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(eIEDConfigName.TMU.Trim().ToUpper()));
                                if (cfObj != null && !string.IsNullOrEmpty(cfObj.Value))
                                    double.TryParse(cfObj.Value, out tmu);
                                foreach (var item in details)
                                {
                                    time = timePrepares.Where(x => x.Commo_Ana_PhaseId == item.CA_PhaseId).Sum(x => x.TMUNumber);
                                    item.TimePrepare = time > 0 ? time / tmu : 0;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(node))
                            techVersion.details.AddRange(_db.T_CA_Phase.Where(x => !x.IsDeleted && x.Node.Contains(node) && !phaseIds.Contains(x.Id)).Select(x => new TechProcessVerDetailModel()
                            {
                                Id = 0,
                                TechProcessVersionId = 0,
                                PhaseGroupId = x.T_PhaseGroup.Id,
                                CA_PhaseId = x.Id,
                                PhaseCode = x.Code,
                                Index = x.Index,
                                PhaseName = x.Name,
                                StandardTMU = Math.Round(x.TotalTMU, 3),
                                EquipmentId = x.EquipmentId != null ? x.EquipmentId ?? 0 : 0,
                                EquipmentCode = x.EquipmentId != null ? x.T_Equipment.Code : "",
                                EquipmentName = x.EquipmentId != null ? x.T_Equipment.Name : "",
                                EquipmentGroupCode = x.EquipmentId.HasValue ? x.T_Equipment.T_EquipmentGroup.GroupCode : "",
                                Description = x.Description == null ? "" : x.Description,
                            }).OrderBy(x => x.Index).ThenBy(x => x.PhaseCode).ToList());
                        #endregion
                    }
                    else
                    {
                        techVersion = new TechProcessVersionModel();
                        techVersion.details.AddRange(_db.T_CA_Phase.Where(x => !x.IsDeleted && x.Node.Contains(node)).Select(x => new TechProcessVerDetailModel()
                        {
                            Id = 0,
                            TechProcessVersionId = 0,
                            PhaseGroupId = x.T_PhaseGroup.Id,
                            CA_PhaseId = x.Id,
                            PhaseCode = x.Code,
                            Index = x.Index,
                            PhaseName = x.Name,
                            StandardTMU = Math.Round(x.TotalTMU, 3),
                            EquipmentId = x.EquipmentId != null ? x.EquipmentId ?? 0 : 0,
                            EquipmentCode = x.EquipmentId != null ? x.T_Equipment.Code : "",
                            EquipmentName = x.EquipmentId != null ? x.T_Equipment.Name : "",
                            EquipmentGroupCode = x.EquipmentId.HasValue ? x.T_Equipment.T_EquipmentGroup.GroupCode : "",
                            Description = x.Description == null ? "" : x.Description,
                        }).OrderBy(x => x.Index).ThenBy(x => x.PhaseCode).ToList());
                    }
                    return techVersion;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
