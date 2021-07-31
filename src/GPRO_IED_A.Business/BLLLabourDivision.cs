using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business
{
    public class BLLLabourDivision
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLLabourDivision _Instance;
        public static BLLLabourDivision Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLLabourDivision();

                return _Instance;
            }
        }
        private BLLLabourDivision() { }
        #endregion

        private T_LabourDivision Check(int Id, int parentId, int lineId)
        {
            return db.T_LabourDivision.FirstOrDefault(x => !x.IsDeleted && x.ParentId == parentId && x.LineId == lineId && x.Id != Id);
        }

        public PagedList<LabourDivisionModel> GetList(int parentId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate ASC";

                    var objs = db.T_LabourDivision.Where(c => !c.IsDeleted && c.ParentId == parentId).OrderByDescending(x => x.CreatedDate).Select(x => new LabourDivisionModel()
                    {
                        Id = x.Id,
                        LineId = x.LineId,
                        LineName = x.T_Line.Name,
                        WorkShopId = x.T_Line.WorkShopId,
                        WorkShopName = string.Empty,
                        TechProVer_Id = x.TechProVer_Id,
                        ParentId = x.ParentId,
                        LastEditer = x.UpdatedUser != null ? x.SUser1.UserName : x.SUser.UserName,
                        LastEditTime = x.UpdatedDate != null ? x.UpdatedDate.Value : x.CreatedDate,
                        TotalPosition = x.TotalPosition
                    }).ToList();
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<LabourDivisionModel>(objs, pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Insert(LabourDivisionModel objModel)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var rs = new ResponseBase();
                    T_LinePosition linePo = null;
                    T_LinePositionDetail linePoDe = null;
                    T_LabourDivision labourObj = null;
                    if (Check(objModel.Id, objModel.ParentId, objModel.LineId) != null)
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "update", Message = "Chuyền này đã có sơ đồ thiết kế chuyền không thể tạo thêm được. Vui Lòng Kiểm Tra Lại" });
                    }
                    else
                    {
                        if (objModel.Id == 0)
                        {
                            #region
                            labourObj = new T_LabourDivision();
                            Parse.CopyObject(objModel, ref labourObj);
                            labourObj.CreatedDate = DateTime.Now;
                            labourObj.CreatedUser = objModel.ActionUser;

                            if (objModel.Positions != null && objModel.Positions.Count > 0)
                            {
                                labourObj.T_LinePosition = new Collection<T_LinePosition>();
                                for (int i = 0; i < objModel.Positions.Count; i++)
                                {
                                    linePo = new T_LinePosition();
                                    Parse.CopyObject(objModel.Positions[i], ref linePo);
                                    linePo.CreatedUser = labourObj.CreatedUser;
                                    linePo.CreatedDate = labourObj.CreatedDate;
                                    linePo.OrderIndex = i + 1;
                                    if (objModel.Positions[i].Details != null && objModel.Positions[i].Details.Count > 0)
                                    {
                                        linePo.T_LinePositionDetail = new Collection<T_LinePositionDetail>();
                                        foreach (var de in objModel.Positions[i].Details)
                                        {
                                            if (de.OrderIndex < objModel.Positions[i].Details.Count)
                                            {
                                                linePoDe = new T_LinePositionDetail();
                                                Parse.CopyObject(de, ref linePoDe);
                                                linePoDe.CreatedUser = labourObj.CreatedUser;
                                                linePoDe.CreatedDate = labourObj.CreatedDate;
                                                linePoDe.T_LinePosition = linePo;
                                                linePo.T_LinePositionDetail.Add(linePoDe);
                                            }
                                        }
                                    }
                                    labourObj.T_LinePosition.Add(linePo);
                                }
                            }

                            db.T_LabourDivision.Add(labourObj);
                            rs.IsSuccess = true;
                            #endregion
                        }
                        else
                        {
                            #region
                            labourObj = Get(objModel.Id);
                            if (labourObj != null)
                            {
                                labourObj.UpdatedUser = objModel.ActionUser;
                                labourObj.UpdatedDate = DateTime.Now;
                                labourObj.TotalPosition = objModel.TotalPosition;
                                labourObj.TechProVer_Id = objModel.TechProVer_Id;
                                labourObj.LineId = objModel.LineId;

                                #region remove old
                                var oldLinePo = db.T_LinePosition.Where(x => !x.IsDeleted && objModel.Id == x.LabourDivisionId);
                                if (oldLinePo != null && oldLinePo.Count() > 0)
                                {
                                    foreach (var item in oldLinePo)
                                    {
                                        item.IsDeleted = true;
                                        var de = db.T_LinePositionDetail.Where(x => !x.IsDeleted && x.Line_PositionId == item.Id);
                                        if (de != null && de.Count() > 0)
                                            foreach (var detail in de)
                                                detail.IsDeleted = true;
                                    }
                                }
                                #endregion

                                #region add new
                                if (objModel.Positions != null && objModel.Positions.Count > 0)
                                {
                                    for (int i = 0; i < objModel.Positions.Count; i++)
                                    {
                                        linePo = new T_LinePosition();
                                        Parse.CopyObject(objModel.Positions[i], ref linePo);
                                        linePo.CreatedUser = objModel.ActionUser;
                                        linePo.CreatedDate = DateTime.Now;
                                        linePo.OrderIndex = i + 1;
                                        linePo.LabourDivisionId = objModel.Id;
                                        if (objModel.Positions[i].Details != null && objModel.Positions[i].Details.Count > 0)
                                        {
                                            linePo.T_LinePositionDetail = new Collection<T_LinePositionDetail>();
                                            foreach (var de in objModel.Positions[i].Details)
                                            {
                                                if (de.OrderIndex < objModel.Positions[i].Details.Count)
                                                {
                                                    linePoDe = new T_LinePositionDetail();
                                                    Parse.CopyObject(de, ref linePoDe);
                                                    linePoDe.CreatedUser = objModel.ActionUser;
                                                    linePoDe.CreatedDate = DateTime.Now;
                                                    linePoDe.T_LinePosition = linePo;
                                                    linePo.T_LinePositionDetail.Add(linePoDe);
                                                }
                                            }
                                        }
                                        db.T_LinePosition.Add(linePo);
                                    }
                                }
                                #endregion

                                rs.IsSuccess = true;
                            }
                            else
                            {
                                rs.IsSuccess = false;
                                rs.Errors.Add(new Error() { MemberName = "update", Message = "Đối Tượng Đã Bị Xóa. Vui Lòng Kiểm Tra Lại" });
                            }
                            #endregion
                        }
                        if (rs.IsSuccess)
                        {
                            db.SaveChanges();
                            rs.IsSuccess = true;
                        }
                    } 
                    return rs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private T_LabourDivision Get(int Id)
        {
            return db.T_LabourDivision.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public ResponseBase Delete(int Id)
        {
            ResponseBase rs;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    rs = new ResponseBase();
                    var obj = Get(Id);
                    if (obj != null)
                    {
                        obj.IsDeleted = true; 
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Đối Tượng Đã Bị Xóa. Vui Lòng Kiểm Tra Lại" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rs;
        }

        public LabourDivisionModel GetById(int labourId )
        {
            var objReturn = new LabourDivisionModel();
            try
            {
                using (db = new SanXuatCheckListEntities())
                { 
                    var obj = Get(labourId);
                    if (obj != null)
                    {
                        Parse.CopyObject(obj, ref objReturn);

                        #region positions
                        var linPos = db.T_LinePosition.Where(x => !x.IsDeleted && x.LabourDivisionId == labourId).Select(x => new LinePositionModel()
                        {
                            Id = x.Id,
                            LabourDivisionId = x.LabourDivisionId,
                            OrderIndex = x.OrderIndex,
                            EmployeeId = x.EmployeeId,
                            EmployeeLastName = x.HR_Employee.LastName,
                            EmployeeName = (x.HR_Employee.FirstName + " " + x.HR_Employee.LastName),
                            LineId = x.T_LabourDivision.LineId,
                            IsHasBTP = x.IsHasBTP,
                            IsHasExitLine = x.IsHasExitLine
                        }).ToList();

                        if (linPos != null && linPos.Count() > 0)
                        {
                            foreach (var item in linPos)
                            {
                                item.Details = db.T_LinePositionDetail.Where(x => !x.IsDeleted && item.Id == x.Line_PositionId).Select(x => new LinePositionDetailModel()
                                {
                                    Id = x.Id,
                                    Line_PositionId = x.Line_PositionId,
                                    TechProVerDe_Id = x.TechProVerDe_Id,
                                    OrderIndex = x.OrderIndex,
                                    Note = x.Note == null ? "" : x.Note,
                                    IsPass = x.IsPass,
                                    PhaseCode = x.T_TechProcessVersionDetail.T_CA_Phase.Code,
                                    PhaseName = x.T_TechProcessVersionDetail.T_CA_Phase.Name,
                                    PhaseGroupId = x.T_TechProcessVersionDetail.T_CA_Phase.T_PhaseGroup.Id,
                                    EquipmentId = x.T_TechProcessVersionDetail.T_CA_Phase.EquipmentId ?? 0,
                                    EquipmentCode = x.T_TechProcessVersionDetail.T_CA_Phase.EquipmentId != null ? x.T_TechProcessVersionDetail.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupCode : "",
                                    EquipmentName = x.T_TechProcessVersionDetail.T_CA_Phase.EquipmentId != null ? x.T_TechProcessVersionDetail.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupName : "",
                                    TotalTMU = x.T_TechProcessVersionDetail.TimeByPercent,
                                    NumberOfLabor = ((x.T_TechProcessVersionDetail.Worker * x.DevisionPercent) / 100),
                                    DevisionPercent = x.DevisionPercent,
                                    DevisionPercent_Temp = x.DevisionPercent,
                                    TotalLabor = x.T_TechProcessVersionDetail.Worker,
                                    Index = x.T_TechProcessVersionDetail.T_CA_Phase.Index
                                }).ToList();
                            }
                            objReturn.Positions.AddRange(linPos);
                        }
                        #endregion

                        #region thong tin TKC
                        objReturn.TechProcess = db.T_TechProcessVersion.Where(x => !x.IsDeleted && x.Id == obj.TechProVer_Id).Select(x => new TechProcessVersionModel()
                        {
                            Id = x.Id,
                            //   CommodityId = x.CommodityId,
                            //   CommodityName = string.Empty, 
                            TimeCompletePerCommo = x.TimeCompletePerCommo,
                            NumberOfWorkers = x.NumberOfWorkers,
                            WorkingTimePerDay = x.WorkingTimePerDay,
                            PacedProduction = x.PacedProduction,
                            ProOfGroupPerDay = x.ProOfGroupPerDay,
                            ProOfGroupPerHour = x.ProOfGroupPerHour,
                            ProOfPersonPerDay = x.ProOfPersonPerDay,
                            Note = x.Note
                        }).FirstOrDefault();

                        if (objReturn.TechProcess != null)
                        {
                            var details = db.T_TechProcessVersionDetail.Where(x => !x.IsDeleted && x.TechProcessVersionId == obj.TechProVer_Id).Select(x => new TechProcessVerDetailModel()
                            {
                                Id = x.Id,
                                TechProcessVersionId = x.TechProcessVersionId,
                                PhaseGroupId = x.T_CA_Phase.T_PhaseGroup.Id,
                                CA_PhaseId = x.CA_PhaseId,
                                PhaseCode = x.T_CA_Phase.Code,
                                PhaseName = x.T_CA_Phase.Name,
                                StandardTMU = Math.Round(x.StandardTMU, 3),
                                Percent = x.Percent,
                                EquipmentId = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.EquipmentId ?? 0 : 0,
                                EquipmentCode = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupCode : string.Empty,
                                EquipmentName = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupName : string.Empty,
                                EquipmentGroupCode = x.T_CA_Phase.EquipmentId != null ? x.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupCode : string.Empty,
                                TimeByPercent = Math.Round(x.TimeByPercent, 3),
                                Worker = x.Worker,
                                De_Percent = 0,
                                Description = x.Description == null ? "" : x.Description,
                                Coefficient = x.T_CA_Phase.SWorkerLevel.Coefficient,
                                WorkerLevelId = x.T_CA_Phase.WorkerLevelId,
                                WorkerLevelName = x.T_CA_Phase.SWorkerLevel.Name,
                                Index = x.T_CA_Phase.Index
                            }).OrderBy(x => x.PhaseCode).ToList();
                            objReturn.TechProcess.details.AddRange(details);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return objReturn;
        }


        public LinePositionsAndTechProcessActiveVersionModel GetLinePositionById(int labourId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var returnObj = new LinePositionsAndTechProcessActiveVersionModel();
                    var linPos = db.T_LinePosition.Where(x => !x.IsDeleted && x.LabourDivisionId == labourId).Select(x => new LinePositionModel()
                    {
                        Id = x.Id,
                        LabourDivisionId = x.LabourDivisionId,
                        OrderIndex = x.OrderIndex,
                        EmployeeId = x.EmployeeId,
                        EmployeeLastName = x.HR_Employee.LastName,
                        EmployeeName = (x.HR_Employee.FirstName + " " + x.HR_Employee.LastName),
                        LineId = x.T_LabourDivision.LineId,
                        LineName = x.T_LabourDivision.T_Line.Name,
                        IsHasBTP = x.IsHasBTP,
                        IsHasExitLine = x.IsHasExitLine,
                        TechProVerId = x.T_LabourDivision.TechProVer_Id,
                    }).ToList();

                    if (linPos != null && linPos.Count() > 0)
                    {
                        foreach (var item in linPos)
                        {
                            item.Details = db.T_LinePositionDetail.Where(x => !x.IsDeleted && item.Id == x.Line_PositionId).Select(x => new LinePositionDetailModel()
                            {
                                Id = x.Id,
                                Line_PositionId = x.Line_PositionId,
                                TechProVerDe_Id = x.TechProVerDe_Id,
                                OrderIndex = x.OrderIndex,
                                NumberOfLabor = x.NumberOfLabor,
                                Note = x.Note == null ? "" : x.Note,
                                IsPass = x.IsPass,
                                PhaseCode = x.T_TechProcessVersionDetail.T_CA_Phase.Code,
                                PhaseName = x.T_TechProcessVersionDetail.T_CA_Phase.Name,
                                PhaseGroupId = x.T_TechProcessVersionDetail.T_CA_Phase.T_PhaseGroup.Id,
                                EquipmentId = x.T_TechProcessVersionDetail.T_CA_Phase.EquipmentId ?? 0,
                                EquipmentCode = x.T_TechProcessVersionDetail.T_CA_Phase.EquipmentId != null ? x.T_TechProcessVersionDetail.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupCode : "",
                                EquipmentName = x.T_TechProcessVersionDetail.T_CA_Phase.EquipmentId != null ? x.T_TechProcessVersionDetail.T_CA_Phase.T_Equipment.T_EquipmentGroup.GroupName : "",
                                TotalTMU = x.T_TechProcessVersionDetail.TimeByPercent,
                                DevisionPercent = x.DevisionPercent,
                                DevisionPercent_Temp = x.DevisionPercent,
                                TotalLabor = x.T_TechProcessVersionDetail.Worker
                            }).ToList();
                        }
                        returnObj.LinePositions.AddRange(linPos);
                    }
                    int techproverId = returnObj.LinePositions[0].TechProVerId;
                    returnObj.TechProcess = db.T_TechProcessVersion.Where(x => !x.IsDeleted && x.Id == techproverId).Select(x => new TechProcessVersionModel()
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        ProductId = x.ProductId,
                        ProductName = x.T_Product.Name,
                        TimeCompletePerCommo = x.TimeCompletePerCommo,
                        NumberOfWorkers = x.NumberOfWorkers,
                        WorkingTimePerDay = x.WorkingTimePerDay,
                        PacedProduction = x.PacedProduction,
                        ProOfGroupPerDay = x.ProOfGroupPerDay,
                        ProOfGroupPerHour = x.ProOfGroupPerHour,
                        ProOfPersonPerDay = x.ProOfPersonPerDay,
                        Note = x.Note
                    }).FirstOrDefault();

                    if (returnObj.TechProcess != null)
                    {
                        var proanaObj = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Id == returnObj.TechProcess.ParentId);
                        if (proanaObj != null)
                        {
                            int id = proanaObj.ParentId;
                            proanaObj = db.T_CommodityAnalysis.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                            if (proanaObj != null)
                                returnObj.TechProcess.WorkShopName = proanaObj.Name;
                        }
                    }

                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

    }
}
