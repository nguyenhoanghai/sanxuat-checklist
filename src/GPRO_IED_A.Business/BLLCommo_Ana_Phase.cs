using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using Hugate.Framework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLCommo_Ana_Phase
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLCommo_Ana_Phase _Instance;
        public static BLLCommo_Ana_Phase Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLCommo_Ana_Phase();

                return _Instance;
            }
        }
        private BLLCommo_Ana_Phase() { }
        #endregion

        bool checkPermis(T_CA_Phase obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ResponseBase InsertOrUpdate(Commo_Ana_PhaseModel model, List<Commo_Ana_Phase_TimePrepareModel> timePreparesModel, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    T_CA_Phase phase = null;
                    T_CA_Phase_TimePrepare timePrepare = null;
                    T_CA_Phase_Mani maniVerDetail;
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, model.ParentId, false))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên Công Đoạn này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                    }
                    else
                    {
                        if (model.Id == 0)
                        {
                            #region create
                            //  var lastPhase = db.T_CA_Phase.Where(x => !x.IsDeleted && x.ParentId == phaseModel.ParentId).OrderByDescending(x => x.Index).FirstOrDefault();
                            phase = new T_CA_Phase();
                            Parse.CopyObject(model, ref phase);
                            phase.Node = phase.Node + phase.ParentId + ",";
                            phase.CreatedDate = DateTime.Now;
                            phase.CreatedUser = model.ActionUser;
                            //   phase.Index = lastPhase != null ? (lastPhase.Index + 1) : 1;

                            if (timePreparesModel != null && timePreparesModel.Count > 0)
                            {
                                // phase.TotalTMU = timePreparesModel.Sum(x => x.TMUNumber) / 27.8;
                                phase.T_CA_Phase_TimePrepare = new Collection<T_CA_Phase_TimePrepare>();
                                foreach (var item in timePreparesModel)
                                {
                                    timePrepare = new T_CA_Phase_TimePrepare();
                                    Parse.CopyObject(item, ref timePrepare);
                                    timePrepare.T_CA_Phase = phase;
                                    timePrepare.CreatedUser = phase.CreatedUser;
                                    timePrepare.CreatedDate = phase.CreatedDate;
                                    phase.T_CA_Phase_TimePrepare.Add(timePrepare);
                                }
                            }
                            if (model.actions != null && model.actions.Count > 0)
                            {
                                phase.T_CA_Phase_Mani = new Collection<T_CA_Phase_Mani>();

                                foreach (var item in model.actions)
                                {
                                    if (item.OrderIndex < model.actions.Count)
                                    {
                                        maniVerDetail = new T_CA_Phase_Mani();
                                        Parse.CopyObject(item, ref maniVerDetail);
                                        maniVerDetail.ManipulationCode = maniVerDetail.ManipulationCode.Trim();
                                        maniVerDetail.ManipulationName = maniVerDetail.ManipulationName.Trim();
                                        maniVerDetail.ManipulationId = maniVerDetail.ManipulationId == 0 ? null : maniVerDetail.ManipulationId;
                                        maniVerDetail.CreatedUser = phase.CreatedUser;
                                        maniVerDetail.CreatedDate = phase.CreatedDate;
                                        maniVerDetail.T_CA_Phase = phase;
                                        phase.T_CA_Phase_Mani.Add(maniVerDetail);
                                    }
                                }
                            }

                            db.T_CA_Phase.Add(phase);
                            db.SaveChanges();

                            //ktra xem co qtcn chua
                            int paId = (phase.Node.Substring(0, phase.Node.Length - 1).Split(',').Select(x => Convert.ToInt32(x)).ToList()[2] + 1);
                            var qt = db.T_TechProcessVersion.FirstOrDefault(x => !x.IsDeleted && x.ParentId == paId);
                            if (qt != null)
                            {
                                var allDetails = db.T_TechProcessVersionDetail.Where(x => !x.IsDeleted && x.TechProcessVersionId == qt.Id);

                                var verDetail = new T_TechProcessVersionDetail();
                                verDetail.TechProcessVersionId = qt.Id;
                                verDetail.CA_PhaseId = phase.Id;
                                verDetail.StandardTMU = phase.TotalTMU;
                                verDetail.Percent = allDetails.First().Percent;
                                verDetail.TimeByPercent = Math.Round((phase.TotalTMU * 100) / verDetail.Percent, 3);
                                verDetail.CreatedDate = phase.CreatedDate;
                                verDetail.CreatedUser = phase.CreatedUser;


                                qt.TimeCompletePerCommo = Math.Round((qt.TimeCompletePerCommo + verDetail.TimeByPercent), 3);
                                qt.PacedProduction = (qt.NumberOfWorkers == 0 ? 0 : (Math.Round(((qt.TimeCompletePerCommo / qt.NumberOfWorkers)), 3)));
                                qt.ProOfGroupPerHour = Math.Round(((3600 / qt.TimeCompletePerCommo) * qt.NumberOfWorkers), 3);
                                qt.ProOfGroupPerDay = Math.Round((qt.ProOfGroupPerHour * qt.WorkingTimePerDay), 3);
                                qt.ProOfPersonPerDay = (qt.NumberOfWorkers == 0 ? 0 : (Math.Round((qt.ProOfGroupPerDay / qt.NumberOfWorkers), 3)));
                                foreach (var item in allDetails)
                                    item.Worker = (qt.PacedProduction == 0 ? 0 : (Math.Round(((item.TimeByPercent / qt.PacedProduction)), 3)));

                                verDetail.Worker = (qt.PacedProduction == 0 ? 0 : (Math.Round(((verDetail.TimeByPercent / qt.PacedProduction)), 3)));
                                db.T_TechProcessVersionDetail.Add(verDetail);
                            }

                            db.SaveChanges();
                            ReOrderPhase(db, phase.ParentId, phase.Index, phase.Id);
                            result.IsSuccess = true;
                            #endregion
                        }
                        else
                        {
                            #region update
                            phase = db.T_CA_Phase.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (phase != null)
                            {
                                if (!checkPermis(phase, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo công đoạn này nên bạn không cập nhật được thông tin cho công đoạn này." });
                                }
                                else
                                {
                                    phase.Name = model.Name;
                                    phase.Index = model.Index;
                                    phase.WorkerLevelId = model.WorkerLevelId;
                                    phase.Code = model.Code;
                                    phase.Description = model.Description;
                                    phase.EquipmentId = model.EquipmentId;
                                    phase.TotalTMU = model.TotalTMU;
                                    phase.ApplyPressuresId = model.ApplyPressuresId;
                                    phase.PercentWasteEquipment = model.PercentWasteEquipment;
                                    phase.PercentWasteManipulation = model.PercentWasteManipulation;
                                    phase.PercentWasteMaterial = model.PercentWasteMaterial;
                                    phase.PercentWasteSpecial = model.PercentWasteSpecial;
                                    phase.Video = model.Video;
                                    phase.UpdatedDate = DateTime.Now;
                                    phase.UpdatedUser = model.ActionUser;
                                    phase.IsLibrary = model.IsLibrary;

                                    #region time prepare
                                    var oldTimes = db.T_CA_Phase_TimePrepare.Where(x => !x.IsDeleted && x.Commo_Ana_PhaseId == model.Id);
                                    if (oldTimes != null && oldTimes.Count() > 0 && (timePreparesModel == null || timePreparesModel.Count == 0))
                                    {
                                        foreach (var item in oldTimes)
                                        {
                                            item.IsDeleted = true;
                                            item.DeletedUser = model.ActionUser;
                                            item.DeletedDate = DateTime.Now;
                                        }
                                    }
                                    else if (oldTimes != null && oldTimes.Count() > 0 && timePreparesModel != null && timePreparesModel.Count > 0)
                                    {
                                        foreach (var item in oldTimes)
                                        {
                                            var obj = timePreparesModel.FirstOrDefault(x => x.TimePrepareId == item.TimePrepareId);
                                            if (obj == null)
                                            {
                                                item.IsDeleted = true;
                                                item.DeletedUser = model.ActionUser;
                                                item.DeletedDate = DateTime.Now;
                                            }
                                            else
                                                timePreparesModel.Remove(obj);
                                        }

                                        if (timePreparesModel.Count > 0)
                                            foreach (var item in timePreparesModel)
                                            {
                                                timePrepare = new T_CA_Phase_TimePrepare();
                                                timePrepare.Commo_Ana_PhaseId = phase.Id;
                                                timePrepare.TimePrepareId = item.TimePrepareId;
                                                timePrepare.CreatedUser = phase.UpdatedUser ?? 0;
                                                timePrepare.CreatedDate = phase.UpdatedDate ?? DateTime.Now;
                                                db.T_CA_Phase_TimePrepare.Add(timePrepare);
                                            }
                                    }
                                    else if ((oldTimes == null || oldTimes.Count() == 0) && timePreparesModel != null && timePreparesModel.Count > 0)
                                    {
                                        foreach (var item in timePreparesModel)
                                        {
                                            timePrepare = new T_CA_Phase_TimePrepare();
                                            timePrepare.Commo_Ana_PhaseId = phase.Id;
                                            timePrepare.TimePrepareId = item.TimePrepareId;
                                            timePrepare.CreatedUser = phase.UpdatedUser ?? 0;
                                            timePrepare.CreatedDate = phase.UpdatedDate ?? DateTime.Now;
                                            db.T_CA_Phase_TimePrepare.Add(timePrepare);
                                        }
                                    }
                                    #endregion

                                    #region actions
                                    var oldDetails = db.T_CA_Phase_Mani.Where(x => !x.IsDeleted && x.CA_PhaseId == phase.Id);
                                    if (model.actions == null && model.actions.Count == 0 && oldDetails != null && oldDetails.Count() > 0)
                                    {
                                        foreach (var item in oldDetails)
                                        {
                                            item.IsDeleted = true;
                                            item.DeletedUser = phase.UpdatedUser;
                                            item.DeletedDate = phase.UpdatedDate;
                                        }
                                    }
                                    else if (model.actions != null && model.actions.Count > 0 && oldDetails != null && oldDetails.Count() > 0)
                                    {
                                        foreach (var item in oldDetails)
                                        {
                                            var obj = model.actions.FirstOrDefault(x => x.Id == item.Id);
                                            if (obj == null)
                                            {
                                                item.IsDeleted = true;
                                                item.DeletedUser = phase.UpdatedUser;
                                                item.DeletedDate = phase.UpdatedDate;
                                            }
                                            else
                                            {
                                                item.OrderIndex = obj.OrderIndex;
                                                item.ManipulationCode = obj.ManipulationCode.Trim();
                                                item.ManipulationName = obj.ManipulationName.Trim();
                                                item.TMUEquipment = obj.TMUEquipment;
                                                item.TMUManipulation = obj.TMUManipulation;
                                                item.Loop = obj.Loop;
                                                item.TotalTMU = obj.TotalTMU;
                                                item.ManipulationId = obj.ManipulationId == 0 ? null : obj.ManipulationId;
                                                item.UpdatedUser = phase.UpdatedUser;
                                                item.UpdatedDate = phase.UpdatedDate;
                                                model.actions.Remove(obj);
                                            }
                                        }
                                        if (model.actions.Count > 0)
                                            for (int i = 0; i < model.actions.Count; i++)
                                            {
                                                if (i < (model.actions.Count - 1))
                                                {
                                                    maniVerDetail = new T_CA_Phase_Mani();
                                                    Parse.CopyObject(model.actions[i], ref maniVerDetail);
                                                    maniVerDetail.ManipulationId = model.actions[i].ManipulationId == 0 ? null : model.actions[i].ManipulationId;
                                                    maniVerDetail.CA_PhaseId = phase.Id;
                                                    maniVerDetail.CreatedUser = phase.UpdatedUser ?? 0;
                                                    maniVerDetail.CreatedDate = phase.UpdatedDate ?? DateTime.Now;
                                                    db.T_CA_Phase_Mani.Add(maniVerDetail);
                                                }
                                            }
                                    }
                                    else if ((oldDetails == null || oldDetails.Count() == 0) && model.actions != null && model.actions.Count > 0)
                                    {
                                        for (int i = 0; i < model.actions.Count; i++)
                                        {
                                            if (i < (model.actions.Count - 1))
                                            {
                                                maniVerDetail = new T_CA_Phase_Mani();
                                                Parse.CopyObject(model.actions[i], ref maniVerDetail);
                                                maniVerDetail.ManipulationId = model.actions[i].ManipulationId == 0 ? null : model.actions[i].ManipulationId;
                                                maniVerDetail.CA_PhaseId = phase.Id;
                                                maniVerDetail.CreatedUser = phase.UpdatedUser ?? 0;
                                                maniVerDetail.CreatedDate = phase.UpdatedDate ?? DateTime.Now;
                                                db.T_CA_Phase_Mani.Add(maniVerDetail);
                                            }
                                        }
                                    }
                                    #endregion

                                    //ktra xem co qtcn chua
                                    int paId = (phase.Node.Substring(0, phase.Node.Length - 1).Split(',').Select(x => Convert.ToInt32(x)).ToList()[2] + 1);
                                    var qt = db.T_TechProcessVersion.FirstOrDefault(x => !x.IsDeleted && x.ParentId == paId);
                                    if (qt != null)
                                    {
                                        var dt = db.T_TechProcessVersionDetail.FirstOrDefault(x => !x.IsDeleted && x.CA_PhaseId == phase.Id && x.TechProcessVersionId == qt.Id);
                                        if (dt == null)
                                        {
                                            var allDetails = db.T_TechProcessVersionDetail.Where(x => !x.IsDeleted && x.TechProcessVersionId == qt.Id);
                                            var verDetail = new T_TechProcessVersionDetail();
                                            verDetail.TechProcessVersionId = qt.Id;
                                            verDetail.CA_PhaseId = phase.Id;
                                            verDetail.StandardTMU = phase.TotalTMU;
                                            verDetail.Percent = allDetails.First().Percent;
                                            verDetail.TimeByPercent = Math.Round((phase.TotalTMU * 100) / verDetail.Percent, 3);
                                            verDetail.CreatedDate = phase.CreatedDate;
                                            verDetail.CreatedUser = phase.CreatedUser;


                                            qt.TimeCompletePerCommo = Math.Round((qt.TimeCompletePerCommo + verDetail.TimeByPercent), 3);
                                            qt.PacedProduction = (qt.NumberOfWorkers == 0 ? 0 : (Math.Round(((qt.TimeCompletePerCommo / qt.NumberOfWorkers)), 3)));
                                            qt.ProOfGroupPerHour = Math.Round(((3600 / qt.TimeCompletePerCommo) * qt.NumberOfWorkers), 3);
                                            qt.ProOfGroupPerDay = Math.Round((qt.ProOfGroupPerHour * qt.WorkingTimePerDay), 3);
                                            qt.ProOfPersonPerDay = (qt.NumberOfWorkers == 0 ? 0 : (Math.Round((qt.ProOfGroupPerDay / qt.NumberOfWorkers), 3)));
                                            foreach (var item in allDetails)
                                                item.Worker = (qt.PacedProduction == 0 ? 0 : (Math.Round(((item.TimeByPercent / qt.PacedProduction)), 3)));

                                            verDetail.Worker = (qt.PacedProduction == 0 ? 0 : (Math.Round(((verDetail.TimeByPercent / qt.PacedProduction)), 3)));
                                            db.T_TechProcessVersionDetail.Add(verDetail);
                                        }
                                    }

                                    db.SaveChanges();
                                    ReOrderPhase(db, phase.ParentId, phase.Index, phase.Id);
                                    result.IsSuccess = true;
                                }
                            }
                            #endregion
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

        public void ReOrderPhase(SanXuatCheckListEntities db, int parentId, int stt, int phaseId)
        {
            //sap xiep lai thứ tự
            var phases = (from x in db.T_CA_Phase where !x.IsDeleted && x.ParentId == parentId && x.Index >= stt && x.Id != phaseId select x).OrderBy(x => x.Index);
            if (phases != null && phases.Count() > 0)
            {
                int _index = stt + 1;
                foreach (var item in phases)
                {
                    item.Index = _index;
                    if (item.Code.Split('-').Length > 1)
                    {
                        string code = item.Code.Substring(0, item.Code.LastIndexOf('-'));
                        item.Code = code + "-" + item.Index;
                    }
                    else
                    {
                        item.Code = item.Index.ToString();
                    }
                    _index++;
                }
            }
            db.SaveChanges();
        }

        private bool CheckExists(string keyword, int id, int parentId, bool isCheckCode)
        {
            try
            {
                T_CA_Phase phase = null;
                keyword = keyword.Trim().ToUpper();
                if (isCheckCode)
                    phase = db.T_CA_Phase.FirstOrDefault(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(keyword) && x.Id != id && x.ParentId == parentId);
                else
                    phase = db.T_CA_Phase.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(keyword) && x.Id != id && x.ParentId == parentId);

                if (phase == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T_CA_Phase> GetPhasesByParentIds(List<int> parentIds)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var phases = db.T_CA_Phase.Where(x => !x.IsDeleted && parentIds.Contains(x.ParentId));
                    if (phases != null && phases.Count() > 0)
                        return phases.ToList();
                    return new List<T_CA_Phase>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedList<PhaseLibModel> GetsWhichNotLib(string keyword, int startIndexRecord, int pageSize, string sorting)
        {
            var phases = new List<PhaseLibModel>();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Name ASC";

                    var temps = (from x in db.T_CA_Phase
                                 where
                                 !x.IsDeleted &&
                                 !x.T_PhaseGroup.IsDeleted &&
                                 !x.IsLibrary &&
                                  (x.Code.Trim().ToUpper().Contains(keyword) ||
                                   x.Name.Trim().ToUpper().Contains(keyword))
                                 select new PhaseLibModel()
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Code = x.Code,
                                     TotalTMU = x.TotalTMU,
                                     EquipName = x.EquipmentId.HasValue ? x.T_Equipment.Name : "",
                                     Node = x.Node,
                                     Product = "",
                                     GroupPhase = ""
                                 }).ToList();
                    if (temps.Count > 0)
                    {
                        var masters = (from x in db.T_CommodityAnalysis where !x.IsDeleted select new { id = x.Id, name = x.Name, OType = x.ObjectType, OId = x.ObjectId }).ToList();
                        var products = (from x in db.T_Product where !x.IsDeleted select new { Id = x.Id, Name = x.Name }).ToList();
                        var phaseGroups = (from x in db.T_PhaseGroup where !x.IsDeleted select new { Id = x.Id, Name = x.Name }).ToList();
                        foreach (var item in temps)
                        {
                            int[] nodeArr = item.Node.Replace("0,0,", "0,").Split(',').Select(x => !string.IsNullOrEmpty(x) ? Convert.ToInt32(x) : 0).ToArray();
                            var found = masters.FirstOrDefault(x => x.id == nodeArr[1]);
                            if (found != null)
                            {
                                var foundPro = products.FirstOrDefault(x => x.Id == found.OId);
                                if (foundPro != null)
                                    item.Product = found.name;
                            }

                            found = masters.FirstOrDefault(x => x.id == nodeArr[4]);
                            if (found != null)
                            {
                                if (phaseGroups.FirstOrDefault(x => x.Id == found.OId) != null)
                                    item.GroupPhase = found.name;
                            }
                            if (!string.IsNullOrEmpty(item.Product) && !string.IsNullOrEmpty(item.GroupPhase))
                                phases.Add(item);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
            switch (sorting)
            {
                case "Product ASC": phases = phases.OrderBy(x => x.Product).ToList(); break;
                case "Product DESC": phases = phases.OrderByDescending(x => x.Product).ToList(); break;
                case "GroupPhase ASC": phases = phases.OrderBy(x => x.GroupPhase).ToList(); break;
                case "GroupPhase DESC": phases = phases.OrderByDescending(x => x.GroupPhase).ToList(); break;
                case "Code ASC": phases = phases.OrderBy(x => x.Code).ToList(); break;
                case "Code DESC": phases = phases.OrderByDescending(x => x.Code).ToList(); break;
                case "EquipName ASC": phases = phases.OrderBy(x => x.EquipName).ToList(); break;
                case "EquipName DESC": phases = phases.OrderByDescending(x => x.EquipName).ToList(); break;
                case "Name DESC": phases = phases.OrderByDescending(x => x.Code).ToList(); break;
                default: phases = phases.OrderBy(x => x.Name).ToList(); break;
            }

            var pageNumber = (startIndexRecord / pageSize) + 1;
            return new PagedList<PhaseLibModel>(phases, pageNumber, pageSize);
        }

        public PagedList<PhaseLibModel> GetsWhichIsLib(string keyword, int startIndexRecord, int pageSize, string sorting)
        {
            var phases = new List<PhaseLibModel>();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Name ASC";

                    var temps = (from x in db.T_CA_Phase
                                 where
                                 !x.IsDeleted &&
                                 !x.T_PhaseGroup.IsDeleted &&
                                 x.IsLibrary &&
                                 (x.Code.Trim().ToUpper().Contains(keyword) ||
                                   x.Name.Trim().ToUpper().Contains(keyword))
                                 select new PhaseLibModel()
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Code = x.Code,
                                     TotalTMU = x.TotalTMU,
                                     EquipName = x.EquipmentId.HasValue ? x.T_Equipment.Name : "",
                                     Node = x.Node,
                                     Product = "",
                                     GroupPhase = ""
                                 }).ToList();
                    if (temps.Count > 0)
                    {
                        var masters = (from x in db.T_CommodityAnalysis where !x.IsDeleted select new { id = x.Id, name = x.Name, OType = x.ObjectType, OId = x.ObjectId }).ToList();
                        var products = (from x in db.T_Product where !x.IsDeleted select new { Id = x.Id, Name = x.Name }).ToList();
                        var phaseGroups = (from x in db.T_PhaseGroup where !x.IsDeleted select new { Id = x.Id, Name = x.Name }).ToList();
                        foreach (var item in temps)
                        {
                            int[] nodeArr = item.Node.Replace("0,0,", "0,").Split(',').Select(x => !string.IsNullOrEmpty(x) ? Convert.ToInt32(x) : 0).ToArray();
                            var found = masters.FirstOrDefault(x => x.id == nodeArr[1]);
                            if (found != null)
                            {
                                var foundPro = products.FirstOrDefault(x => x.Id == found.OId);
                                if (foundPro != null)
                                    item.Product = found.name;
                            }

                            found = masters.FirstOrDefault(x => x.id == nodeArr[4]);
                            if (found != null)
                            {
                                if (phaseGroups.FirstOrDefault(x => x.Id == found.OId) != null)
                                    item.GroupPhase = found.name;
                            }


                            if (!string.IsNullOrEmpty(item.Product) && !string.IsNullOrEmpty(item.GroupPhase))
                                phases.Add(item);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
            switch (sorting)
            {
                case "Product ASC": phases = phases.OrderBy(x => x.Product).ToList(); break;
                case "Product DESC": phases = phases.OrderByDescending(x => x.Product).ToList(); break;
                case "GroupPhase ASC": phases = phases.OrderBy(x => x.GroupPhase).ToList(); break;
                case "GroupPhase DESC": phases = phases.OrderByDescending(x => x.GroupPhase).ToList(); break;
                case "Code ASC": phases = phases.OrderBy(x => x.Code).ToList(); break;
                case "Code DESC": phases = phases.OrderByDescending(x => x.Code).ToList(); break;
                case "EquipName ASC": phases = phases.OrderBy(x => x.EquipName).ToList(); break;
                case "EquipName DESC": phases = phases.OrderByDescending(x => x.EquipName).ToList(); break;
                case "Name DESC": phases = phases.OrderByDescending(x => x.Code).ToList(); break;
                default: phases = phases.OrderBy(x => x.Name).ToList(); break;
            }
            var pageNumber = (startIndexRecord / pageSize) + 1;
            return new PagedList<PhaseLibModel>(phases.OrderBy(x => x.Product), pageNumber, pageSize);
        }

        public ResponseBase UpdateLibs(string ids)
        {
            ResponseBase rs = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    string query = " ";
                    if (!string.IsNullOrEmpty(ids))
                    {
                        string[] arr = ids.Split(',');
                        query += " UPDATE [dbo].[T_CA_Phase] SET [IsLibrary]=" + 1 + " where ";
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(arr[i]))
                            {
                                if (i == 0)
                                    query += " Id =" + arr[i];
                                if (i > 0)
                                    query += " or Id =" + arr[i];
                            }
                        }
                    }
                    db.Database.ExecuteSqlCommand(query);
                    db.SaveChanges();
                    rs.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
            }
            return rs;
        }

        public ResponseBase RemoveLibs(string ids)
        {
            ResponseBase rs = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    string query = " ";
                    if (!string.IsNullOrEmpty(ids))
                    {
                        string[] arr = ids.Split(',');
                        query += " UPDATE [dbo].[T_CA_Phase] SET [IsLibrary]=" + 0 + " where ";
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(arr[i]))
                            {
                                if (i == 0)
                                    query += " Id =" + arr[i];
                                if (i > 0)
                                    query += " or Id =" + arr[i];
                            }
                        }
                    }
                    db.Database.ExecuteSqlCommand(query);
                    db.SaveChanges();
                    rs.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
            }
            return rs;
        }


        //load lai 
        public PagedList<Commo_Ana_PhaseModel> GetListByNode(string node, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Index ASC";

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var phases = (from x in db.T_CA_Phase
                                  where !x.IsDeleted && !x.T_PhaseGroup.IsDeleted && x.Node.Trim() == (node.Trim())
                                  orderby x.Index
                                  select new Commo_Ana_PhaseModel()
                                  {
                                      Id = x.Id,
                                      Index = x.Index,
                                      Name = x.Name,
                                      Code = x.Code,
                                      TotalTMU = x.TotalTMU,
                                      Description = x.Description,
                                      EquipmentId = x.EquipmentId,
                                      EquipName = x.T_Equipment.Name,
                                      EquipDes = x.T_Equipment.Description,
                                      EquipTypeDefaultId = x.EquipmentId != null ? x.T_Equipment.T_EquipmentType.EquipTypeDefaultId ?? 0 : 0,
                                      WorkerLevelId = x.WorkerLevelId,
                                      WorkerLevelName = x.SWorkerLevel.Name,
                                      ParentId = x.ParentId,
                                      PhaseGroupId = x.PhaseGroupId,
                                      ApplyPressuresId = x.ApplyPressuresId != null ? x.ApplyPressuresId : 0,
                                      PercentWasteEquipment = x.PercentWasteEquipment,
                                      PercentWasteManipulation = x.PercentWasteManipulation,
                                      PercentWasteMaterial = x.PercentWasteMaterial,
                                      PercentWasteSpecial = x.PercentWasteSpecial,
                                      Video = x.Video,
                                      IsLibrary = x.IsLibrary
                                  }).OrderBy(sorting).ToList();

                    var pageListReturn = new PagedList<Commo_Ana_PhaseModel>(phases, pageNumber, pageSize);
                    if (pageListReturn != null && pageListReturn.Count > 0)
                    {
                        double tmu = 27.8;
                        var config = (from x in db.T_IEDConfig
                                      where !x.IsDeleted && x.Name.Trim().ToUpper().Equals(eIEDConfigName.TMU.Trim().ToUpper())
                                      select x).FirstOrDefault();
                        if (config != null)
                            double.TryParse(config.Value, out tmu);
                        foreach (var item in pageListReturn)
                        {
                            item.timePrepares.AddRange((from x in db.T_CA_Phase_TimePrepare
                                                        where !x.IsDeleted && x.Commo_Ana_PhaseId == item.Id
                                                        select new Commo_Ana_Phase_TimePrepareModel()
                                                        {
                                                            Id = x.Id,
                                                            TimePrepareId = x.TimePrepareId,
                                                            Name = x.T_TimePrepare.Name,
                                                            Code = x.T_TimePrepare.Code,
                                                            TimeTypePrepareName = x.T_TimePrepare.T_TimeTypePrepare.Name,
                                                            TMUNumber = x.T_TimePrepare.TMUNumber,
                                                            Description = x.T_TimePrepare.Description,
                                                        }));
                            item.TimePrepareTMU = item.timePrepares.Sum(x => x.TMUNumber);
                            item.actions.AddRange((from x in db.T_CA_Phase_Mani
                                                   where !x.IsDeleted && x.CA_PhaseId == item.Id
                                                   orderby x.OrderIndex
                                                   select new Commo_Ana_Phase_ManiModel()
                                                   {
                                                       Id = x.Id,
                                                       CA_PhaseId = x.CA_PhaseId,
                                                       ManipulationId = x.ManipulationId,
                                                       ManipulationName = x.ManipulationName,
                                                       ManipulationCode = x.ManipulationCode,
                                                       TMUEquipment = x.TMUEquipment,
                                                       TMUManipulation = x.TMUManipulation,
                                                       Loop = x.Loop,
                                                       TotalTMU = x.TotalTMU,
                                                       OrderIndex = x.OrderIndex
                                                   }));
                            item.ManiVerTMU = item.actions.Sum(x => ((x.TMUEquipment ?? 0 * x.Loop) + (x.TMUManipulation ?? 0 * x.Loop)));
                        }
                    }
                    return pageListReturn;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Commo_Ana_Phase_ManiModel> GetPhaseVersionManipulationByManipulationVersionId(int Id)
        {
            try
            {
                //var phaseVerMani = db.T_CA_Phase_ManiVer_De.Where(x => !x.IsDeleted && x.Commo_Ana_Phase_ManiVerId == Id).Select(x => new Commo_Ana_Phase_ManiVer_DetailModel()
                //{
                //    Id = x.Id,
                //    ManipulationId = x.ManipulationId,
                //    ManipulationName = x.T_ManipulationLibrary.Name,
                //    OrderIndex = x.OrderIndex,
                //    Commo_Ana_Phase_ManiVerId = x.Commo_Ana_Phase_ManiVerId,
                //    TMUEquipment = x.TMUEquipment,
                //    TMUManipulation = x.TMUManipulation,
                //    Description = x.Description
                //});
                //if (phaseVerMani != null && phaseVerMani.Count() > 0)
                //{
                //    return phaseVerMani.ToList();
                //}
                return new List<Commo_Ana_Phase_ManiModel>();
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
                    var phase = db.T_CA_Phase.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                    if (phase == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "delete ", Message = "Công Đoạn này đã tồn tại hoặc đã bị xóa trướ đó. Vui lòng kiểm tra lại dữ liệu!." });
                    }
                    else
                    {
                        if (!checkPermis(phase, actionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Bạn không phải là người tạo công đoạn này nên bạn không xóa được xóa công đoạn này." });
                        }
                        else
                        {
                            phase.IsDeleted = true;
                            phase.DeletedUser = actionUserId;
                            phase.DeletedDate = DateTime.Now;

                            int paId = (phase.Node.Substring(0, phase.Node.Length - 1).Split(',').Select(x => Convert.ToInt32(x)).ToList()[2] + 1);
                            var qt = db.T_TechProcessVersion.FirstOrDefault(x => !x.IsDeleted && x.ParentId == paId);
                            if (qt != null)
                            {
                                var deleteObj = db.T_TechProcessVersionDetail.FirstOrDefault(x => !x.IsDeleted && x.TechProcessVersionId == qt.Id && x.CA_PhaseId == phase.Id);
                                if (deleteObj != null)
                                {
                                    deleteObj.IsDeleted = true;
                                    deleteObj.DeletedUser = actionUserId;
                                    deleteObj.DeletedDate = DateTime.Now;
                                }

                                var allDetails = db.T_TechProcessVersionDetail.Where(x => !x.IsDeleted && x.TechProcessVersionId == qt.Id);
                                if (allDetails != null && allDetails.Count() > 0)
                                {
                                    qt.TimeCompletePerCommo = Math.Round(allDetails.Sum(x => x.TimeByPercent), 3);
                                    qt.PacedProduction = qt.NumberOfWorkers != 0 ? Math.Round(((qt.TimeCompletePerCommo / qt.NumberOfWorkers)), 3) : 0;
                                    qt.ProOfGroupPerHour = qt.NumberOfWorkers != 0 ? Math.Round(((3600 / qt.TimeCompletePerCommo) * qt.NumberOfWorkers), 3) : 0;
                                    qt.ProOfGroupPerDay = Math.Round((qt.ProOfGroupPerHour * qt.WorkingTimePerDay), 3);
                                    qt.ProOfPersonPerDay = qt.NumberOfWorkers != 0 ? Math.Round((qt.ProOfGroupPerDay / qt.NumberOfWorkers), 3) : 0;
                                    foreach (var item in allDetails)
                                        item.Worker = qt.PacedProduction != 0 ? Math.Round(((item.TimeByPercent / qt.PacedProduction)), 3) : 0;
                                }
                                else
                                {
                                    qt.IsDeleted = true;
                                    qt.DeletedUser = actionUserId;
                                    qt.DeletedDate = DateTime.Now;
                                }
                            }
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

        public ResponseBase RemovePhaseVideo(int Id, int actionUserId, bool isOwner, ref string videoPath)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var phase = db.T_CA_Phase.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                    if (phase == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "delete ", Message = "Công Đoạn này đã tồn tại hoặc đã bị xóa trước đó. Vui lòng kiểm tra lại dữ liệu!." });
                    }
                    else
                    {
                        if (!checkPermis(phase, actionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Bạn không phải là người tạo công đoạn này nên bạn không xóa được xóa công đoạn này." });
                        }
                        else
                        {
                            videoPath = phase.Video;
                            phase.Video = "";
                            phase.UpdatedUser = actionUserId;
                            phase.UpdatedDate = DateTime.Now;
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

        public ResponseBase Copy(int Id, int actionUserId)
        {
            var rs = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var phase = (from x in db.T_CA_Phase
                                 where !x.IsDeleted && x.Id == Id
                                 select x).FirstOrDefault();
                    if (phase != null)
                    {
                        var phaseAcc = (from x in db.T_CA_Phase_Mani
                                        where !x.IsDeleted && x.CA_PhaseId == Id
                                        select x);
                        var times = (from x in db.T_CA_Phase_TimePrepare
                                     where !x.IsDeleted && x.Commo_Ana_PhaseId == Id
                                     select x);

                        var lastPhase = (from x in db.T_CA_Phase
                                         where !x.IsDeleted && x.ParentId == phase.ParentId
                                         orderby x.Index descending
                                         select x).FirstOrDefault();

                        T_CA_Phase phaseC;
                        T_CA_Phase_Mani maniC;
                        T_CA_Phase_TimePrepare timeC;
                        var now = DateTime.Now;
                        phaseC = new T_CA_Phase();
                        phaseC.Index = lastPhase.Index + 1;
                        phaseC.Name = phase.Name + "-Copy";
                        phaseC.Code = (phase.Code.LastIndexOf('-') == -1 ? phaseC.Index.ToString() : (phase.Code.Substring(0, phase.Code.LastIndexOf('-')) + "-" + phaseC.Index));
                        phaseC.PhaseGroupId = phase.PhaseGroupId;
                        phaseC.Description = phase.Description;
                        phaseC.EquipmentId = phase.EquipmentId;
                        phaseC.PhaseGroupId = phase.PhaseGroupId;
                        phaseC.WorkerLevelId = phase.WorkerLevelId;
                        phaseC.ParentId = phase.ParentId;
                        phaseC.TotalTMU = phase.TotalTMU;
                        phaseC.ApplyPressuresId = phase.ApplyPressuresId;
                        phaseC.PercentWasteEquipment = phase.PercentWasteEquipment;
                        phaseC.PercentWasteManipulation = phase.PercentWasteManipulation;
                        phaseC.PercentWasteMaterial = phase.PercentWasteMaterial;
                        phaseC.PercentWasteSpecial = phase.PercentWasteSpecial;
                        phaseC.Node = phase.Node;
                        phaseC.Video = phase.Video;
                        phaseC.CreatedUser = actionUserId;
                        phaseC.CreatedDate = now;
                        //  db.T_CA_Phase.Add(phaseC);
                        //  db.SaveChanges();

                        if (times != null && times.Count() > 0)
                        {
                            phaseC.T_CA_Phase_TimePrepare = new Collection<T_CA_Phase_TimePrepare>();
                            foreach (var item in times)
                            {
                                timeC = new T_CA_Phase_TimePrepare();
                                timeC.TimePrepareId = item.TimePrepareId;
                                timeC.CreatedUser = actionUserId;
                                timeC.CreatedDate = now;
                                timeC.T_CA_Phase = phaseC;
                                phaseC.T_CA_Phase_TimePrepare.Add(timeC);
                            }
                        }

                        if (phaseAcc != null && phaseAcc.Count() > 0)
                        {
                            phaseC.T_CA_Phase_Mani = new Collection<T_CA_Phase_Mani>();
                            foreach (var item in phaseAcc)
                            {
                                maniC = new T_CA_Phase_Mani();
                                maniC.OrderIndex = item.OrderIndex;
                                maniC.ManipulationId = item.ManipulationId;
                                maniC.ManipulationCode = item.ManipulationCode;
                                maniC.ManipulationName = item.ManipulationName;
                                maniC.TMUEquipment = item.TMUEquipment;
                                maniC.TMUManipulation = item.TMUManipulation;
                                maniC.Loop = item.Loop;
                                maniC.TotalTMU = item.TotalTMU;
                                maniC.CreatedUser = actionUserId;
                                maniC.CreatedDate = now;
                                maniC.T_CA_Phase = phaseC;
                                phaseC.T_CA_Phase_Mani.Add(maniC);
                            }
                        }
                        db.T_CA_Phase.Add(phaseC);
                        db.SaveChanges();

                        //ktra xem co qtcn chua
                        int paId = (phase.Node.Substring(0, phase.Node.Length - 1).Split(',').Select(x => Convert.ToInt32(x)).ToList()[2] + 1);
                        var qt = (from x in db.T_TechProcessVersion
                                  where !x.IsDeleted && x.ParentId == paId
                                  select x).FirstOrDefault();
                        if (qt != null)
                        {
                            var allDetails = (from x in db.T_TechProcessVersionDetail
                                              where !x.IsDeleted && x.TechProcessVersionId == qt.Id
                                              select x);

                            var verDetail = new T_TechProcessVersionDetail();
                            verDetail.TechProcessVersionId = qt.Id;
                            verDetail.CA_PhaseId = phase.Id;
                            verDetail.StandardTMU = phase.TotalTMU;
                            verDetail.Percent = allDetails.First().Percent;
                            verDetail.TimeByPercent = Math.Round((phase.TotalTMU * 100) / verDetail.Percent, 3);
                            verDetail.CreatedDate = phase.CreatedDate;
                            verDetail.CreatedUser = phase.CreatedUser;

                            qt.TimeCompletePerCommo = Math.Round((qt.TimeCompletePerCommo + verDetail.TimeByPercent), 3);
                            qt.PacedProduction = (qt.TimeCompletePerCommo > 0 && qt.NumberOfWorkers > 0 ? Math.Round(((qt.TimeCompletePerCommo / qt.NumberOfWorkers)), 3) : 0);
                            qt.ProOfGroupPerHour = Math.Round(((3600 / qt.TimeCompletePerCommo) * qt.NumberOfWorkers), 3);
                            qt.ProOfGroupPerDay = Math.Round((qt.ProOfGroupPerHour * qt.WorkingTimePerDay), 3);
                            qt.ProOfPersonPerDay = (qt.ProOfGroupPerDay > 0 && qt.NumberOfWorkers > 0 ? Math.Round((qt.ProOfGroupPerDay / qt.NumberOfWorkers), 3) : 0);
                            foreach (var item in allDetails)
                                item.Worker = (item.TimeByPercent > 0 && qt.PacedProduction > 0 ? Math.Round(((item.TimeByPercent / qt.PacedProduction)), 3) : 0);

                            verDetail.Worker = (verDetail.TimeByPercent > 0 && qt.PacedProduction > 0 ? Math.Round(((verDetail.TimeByPercent / qt.PacedProduction)), 3) : 0);
                            db.T_TechProcessVersionDetail.Add(verDetail);
                            db.Entry<T_TechProcessVersion>(qt).State = System.Data.Entity.EntityState.Modified;
                        }
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Errors.Add(new Error() { MemberName = "", Message = "Lỗi SQL" });
            }
            return rs;
        }

        public ExportPhaseActionsModel Export_CommoAnaPhaseManiVer(int Id)
        {
            ExportPhaseActionsModel exportObj = null;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var phase = (from x in db.T_CA_Phase
                                 where !x.IsDeleted && x.Id == Id
                                 select x).FirstOrDefault();
                    if (phase != null)
                    {
                        exportObj = new ExportPhaseActionsModel();
                        exportObj.Details.AddRange((from x in db.T_CA_Phase_Mani
                                                    where !x.IsDeleted && x.CA_PhaseId == Id
                                                    orderby x.OrderIndex
                                                    select new Commo_Ana_Phase_ManiModel()
                                                    {
                                                        Id = x.Id,
                                                        CA_PhaseId = x.CA_PhaseId,
                                                        ManipulationId = x.ManipulationId,
                                                        ManipulationName = x.ManipulationName,
                                                        ManipulationCode = x.ManipulationCode,
                                                        TMUEquipment = x.TMUEquipment,
                                                        TMUManipulation = x.TMUManipulation,
                                                        Loop = x.Loop,
                                                        TotalTMU = x.TotalTMU,
                                                        OrderIndex = x.OrderIndex,
                                                    }));
                        exportObj.TotalTMU = phase.TotalTMU;
                        exportObj.PhaseName = phase.Name;

                        int cAnaId = Convert.ToInt32(phase.Node.Split(',')[1]);
                        var cAnaObj = db.T_CommodityAnalysis.FirstOrDefault(x => x.Id == cAnaId);
                        if (cAnaObj != null)
                        {
                            var proObj = db.T_Product.FirstOrDefault(x => x.Id == cAnaObj.ObjectId);
                            if (proObj != null)
                            {
                                exportObj.ProductName = proObj.Name;
                                exportObj.CustomerName = proObj.Code;
                            }
                            else
                            {
                                exportObj.ProductName = "";
                                exportObj.CustomerName = "";
                            }
                        }


                        //var timePrepares = db.T_CA_Phase_TimePrepare.Where(x => !x.IsDeleted && x.Commo_Ana_PhaseId ==  Id).Select(x => new Commo_Ana_Phase_TimePrepareModel()
                        //{
                        //    Id = x.Id,
                        //    Commo_Ana_PhaseId = x.Commo_Ana_PhaseId,
                        //    TMUNumber = x.T_TimePrepare.TMUNumber
                        //}).ToList();
                        //if (timePrepares.Count > 0)
                        //{
                        //    double tmu = 0, time = 0;
                        //    string strTmu = bllIEDConfig.GetValueByCode(eIEDConfigName.TMU);
                        //    if (!string.IsNullOrEmpty(strTmu))
                        //        double.TryParse(strTmu, out tmu);
                        //    time = timePrepares.Sum(x => x.TMUNumber);
                        //    exportObj.TimePrepare = time > 0 ? time / tmu : 0;
                        //}
                        //else
                        exportObj.TimePrepare = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return exportObj;
        }

        public int GetLastIndex(int ParentId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = (from x in db.T_CA_Phase where !x.IsDeleted && x.ParentId == ParentId orderby x.Index descending select x.Index).FirstOrDefault();
                if (obj != null)
                    return obj;
                return 0;
            }
        }
        public List<ModelSelectItem> GetAllPhasesForSuggest()
        {
            var list = new List<ModelSelectItem>();
            using (var db = new SanXuatCheckListEntities())
            {
                var phases = (from x in db.T_CA_Phase
                              where !x.IsDeleted && !x.T_CommodityAnalysis.IsDeleted &&
                              !x.T_PhaseGroup.IsDeleted &&
                              x.IsLibrary
                              select new // ModelSelectItem()
                              {
                                  Value = x.Id,
                                  Name = x.Name,
                                  Code = x.Code,
                                  Double = x.TotalTMU,
                                  Node = x.Node
                              }).ToList();
                if (phases.Count > 0)
                {
                    var proanas = (from x in db.T_CommodityAnalysis
                                   where
                                      !x.IsDeleted &&
                                      (x.ObjectType == (int)eObjectType.isCommodity || x.ObjectType == (int)eObjectType.isPhaseGroup)
                                   select new
                                   {
                                       Id = x.Id,
                                       objId = x.ObjectId,
                                       objType = x.ObjectType
                                   }).ToList();

                    var products = (from x in db.T_Product
                                    where !x.IsDeleted
                                    select new
                                    {
                                        Id = x.Id,
                                        name = x.Name
                                    }).ToList();
                    var phaseGroups = (from x in db.T_PhaseGroup
                                       where !x.IsDeleted
                                       select new
                                       {
                                           Id = x.Id,
                                           name = x.Name
                                       }).ToList();

                    foreach (var item in phases)
                    {
                        var arr = item.Node.Replace("0,0,", "0,").Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
                        var proObj = proanas.FirstOrDefault(x => x.Id == arr[1]);
                        if (proObj != null)
                        {
                            //check product
                            var pro = products.FirstOrDefault(x => x.Id == proObj.objId);
                            if (pro != null)
                            {
                                proObj = proanas.FirstOrDefault(x => x.Id == arr[4]);
                                if (proObj != null)
                                {
                                    //check phase group
                                    var pg = phaseGroups.FirstOrDefault(x => x.Id == proObj.objId);
                                    if (pg != null)
                                    {
                                        list.Add(new ModelSelectItem()
                                        {
                                            Value = item.Value,
                                            Name = item.Name,
                                            Code = item.Code,
                                            Double = item.Double,
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        public Commo_Ana_PhaseModel GetPhase(int phaseId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    var phaseObj = (from x in db.T_CA_Phase
                                    where !x.IsDeleted && !x.T_PhaseGroup.IsDeleted && x.Id == phaseId
                                    select new Commo_Ana_PhaseModel()
                                    {
                                        Id = x.Id,
                                        Index = x.Index,
                                        Name = x.Name,
                                        Code = x.Code,
                                        TotalTMU = x.TotalTMU,
                                        Description = x.Description,
                                        EquipmentId = x.EquipmentId,
                                        EquipName = x.T_Equipment.Name,
                                        EquipDes = x.T_Equipment.Description,
                                        EquipTypeDefaultId = x.EquipmentId != null ? x.T_Equipment.T_EquipmentType.EquipTypeDefaultId ?? 0 : 0,
                                        WorkerLevelId = x.WorkerLevelId,
                                        WorkerLevelName = x.SWorkerLevel.Name,
                                        ParentId = x.ParentId,
                                        PhaseGroupId = x.PhaseGroupId,
                                        ApplyPressuresId = x.ApplyPressuresId != null ? x.ApplyPressuresId : 0,
                                        PercentWasteEquipment = x.PercentWasteEquipment,
                                        PercentWasteManipulation = x.PercentWasteManipulation,
                                        PercentWasteMaterial = x.PercentWasteMaterial,
                                        PercentWasteSpecial = x.PercentWasteSpecial,
                                        Video = x.Video
                                    }).FirstOrDefault();
                    if (phaseObj != null)
                    {
                        double tmu = 27.8;
                        var config = (from x in db.T_IEDConfig
                                      where !x.IsDeleted && x.Name.Trim().ToUpper().Equals(eIEDConfigName.TMU.Trim().ToUpper())
                                      select x).FirstOrDefault();
                        if (config != null)
                            double.TryParse(config.Value, out tmu);

                        phaseObj.timePrepares.AddRange((from x in db.T_CA_Phase_TimePrepare
                                                        where !x.IsDeleted && x.Commo_Ana_PhaseId == phaseObj.Id
                                                        select new Commo_Ana_Phase_TimePrepareModel()
                                                        {
                                                            Id = x.Id,
                                                            TimePrepareId = x.TimePrepareId,
                                                            Name = x.T_TimePrepare.Name,
                                                            Code = x.T_TimePrepare.Code,
                                                            TimeTypePrepareName = x.T_TimePrepare.T_TimeTypePrepare.Name,
                                                            TMUNumber = x.T_TimePrepare.TMUNumber,
                                                            Description = x.T_TimePrepare.Description,
                                                        }));
                        phaseObj.TimePrepareTMU = phaseObj.timePrepares.Sum(x => x.TMUNumber);
                        phaseObj.actions.AddRange((from x in db.T_CA_Phase_Mani
                                                   where !x.IsDeleted && x.CA_PhaseId == phaseObj.Id
                                                   orderby x.OrderIndex
                                                   select new Commo_Ana_Phase_ManiModel()
                                                   {
                                                       Id = x.Id,
                                                       CA_PhaseId = x.CA_PhaseId,
                                                       ManipulationId = x.ManipulationId,
                                                       ManipulationName = x.ManipulationName,
                                                       ManipulationCode = x.ManipulationCode,
                                                       TMUEquipment = x.TMUEquipment,
                                                       TMUManipulation = x.TMUManipulation,
                                                       Loop = x.Loop,
                                                       TotalTMU = x.TotalTMU,
                                                       OrderIndex = x.OrderIndex
                                                   }));
                        phaseObj.ManiVerTMU = phaseObj.actions.Sum(x => ((x.TMUEquipment ?? 0 * x.Loop) + (x.TMUManipulation ?? 0 * x.Loop)));

                    }
                    return phaseObj;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public ResponseBase TinhLaiCode(List<Commo_Ana_Phase_ManiModel> actions, int equipmentId, int equiptypedefaultId, int applyPressure)
        {
            ResponseBase result = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    for (int i = 0; i < actions.Count  ; i++)
                    { 
                        if (actions[i].ManipulationCode.Substring(0, 1) == "C" || actions[i].ManipulationCode.Substring(0, 2) == "SE")
                        {
                            if(actions[i].ManipulationCode.Length > 4)
                            {
                                string stopPrecisionCode = actions[i].ManipulationCode.Substring(actions[i].ManipulationCode.Length - 1, 1);
                                var stopPrecision = db.T_StopPrecisionLibrary.FirstOrDefault(c => c.Code.Trim().ToUpper().Equals(stopPrecisionCode.Trim().ToUpper()) && !c.IsDeleted);
                                if (stopPrecision == null)
                                    result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin độ dừng chính xác.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                else
                                {
                                    float distance = 0;
                                    if (actions[i].ManipulationCode.Substring(0, 2).Equals("SE"))
                                    {
                                        float.TryParse(actions[i].ManipulationCode.Substring(2, actions[i].ManipulationCode.Length - 3), out distance);
                                        if (distance == 0)
                                            result.Errors.Add(new Error() { Message = "Thông tin khoảng cách may không chính xác, hoặc bằng 0.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else
                                        {
                                            actions[i].TMUEquipment = BLLEquipment.Instance.CalculationMachineTMU(equipmentId, equiptypedefaultId, distance, stopPrecision.TMUNumber, 0, 0);
                                            actions[i].TotalTMU = (actions[i].TMUEquipment * actions[i].Loop)??0; 
                                        }
                                    }
                                    else if (actions[i].ManipulationCode.Substring(0, 1).Equals("C"))
                                    {
                                        float.TryParse(actions[i].ManipulationCode.Substring(1, actions[i].ManipulationCode.Length - 3), out distance);
                                        string natureCutCode = actions[i].ManipulationCode.Substring(actions[i].ManipulationCode.Length - 2, 1);
                                        var natureCut = db.T_NatureCutsLibrary.FirstOrDefault(c => c.Code.Trim().ToUpper().Equals(natureCutCode.Trim().ToUpper()) && !c.IsDeleted);
                                        if (applyPressure == 0)
                                            result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin số lớp cắt.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else if (natureCut == null)
                                            result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin tính chất căt.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else if (distance == 0)
                                            result.Errors.Add(new Error() { Message = "Thông tin khoảng cách may không chính xác, hoặc bằng 0.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else
                                        {

                                            actions[i].TMUEquipment = BLLEquipment.Instance.CalculationMachineTMU(equipmentId, equiptypedefaultId, distance, stopPrecision.TMUNumber, applyPressure, natureCut.Factor);
                                            actions[i].TotalTMU = (actions[i].TMUEquipment * actions[i].Loop)??0;                                             
                                        }
                                    }
                                }
                            }
                        }
                    }
                    result.Data = actions;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}
