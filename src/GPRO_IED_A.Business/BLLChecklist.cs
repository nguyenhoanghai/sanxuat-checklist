﻿using GPRO.Core.Mvc;
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
    public class BLLChecklist
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLChecklist _Instance;
        public static BLLChecklist Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLChecklist();

                return _Instance;
            }
        }
        private BLLChecklist() { }
        #endregion

        bool checkPermis(Checklist obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ChecklistModel Get(int Id, int userId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    ChecklistModel model = null;
                    Checklist checklist = db.Checklist.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                    if (checklist != null)
                    {
                        model = new ChecklistModel()
                        {
                            Id = checklist.Id,
                            Name = checklist.Name,
                            Note = checklist.Note,
                            CreatedDate = checklist.CreatedDate,
                            LineId = checklist.LineId,
                            LineName = (checklist.LineId.HasValue ? checklist.Line.Name : ""),
                            POId = checklist.POId,
                            ProductId = checklist.ProductId,
                            ProductName = (checklist.ProductId.HasValue ? checklist.Product.Name : ""),
                            CustomerId = checklist.CustomerId,
                            CustomerName = (checklist.CustomerId.HasValue ? checklist.Customer.Name : ""),
                            Productivity = checklist.Productivity,
                            Quantities = checklist.Quantities,
                            ProductionDays = checklist.ProductionDays,
                            DeliveryDate = checklist.DeliveryDate,
                            InputDate = checklist.InputDate,
                            EndDate = checklist.EndDate,
                            RealEndDate = checklist.RealEndDate,
                            StatusId = checklist.StatusId,
                            StatusName = checklist.Status.Name,
                            RelatedEmployees = checklist.RelatedEmployees,
                            ProductUnit = (checklist.ProductId.HasValue ? checklist.Product.Unit.Name : "")
                        };
                        var jobSteps = checklist.Checklist_JobStep.Where(x => !x.IsDeleted).OrderBy(x => x.StepIndex).ToList();
                        if (userId != 0)
                            jobSteps = jobSteps.Where(x => x.EmployeeId.HasValue && x.EmployeeId.Value == userId).ToList();

                        if (jobSteps.Count > 0)
                        {
                            Checklist_JobStepModel jStepObj = null;
                            var employees = db.SUser.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.UserName }).ToList();
                            foreach (var jsteps in jobSteps)
                            {
                                jStepObj = new Checklist_JobStepModel()
                                {
                                    Id = jsteps.Id,
                                    StepIndex = jsteps.StepIndex,
                                    Name = jsteps.Name,
                                    JobStepContent = jsteps.JobStepContent,
                                    EmployeeId = jsteps.EmployeeId,
                                    EmployeeName = (jsteps.EmployeeId.HasValue ? jsteps.SUser.UserName : ""),
                                    RelatedEmployees = jsteps.RelatedEmployees,
                                    StartDate = jsteps.StartDate,
                                    EndDate = jsteps.EndDate,
                                    RealEndDate = jsteps.RealEndDate,
                                    ReminderDate = jsteps.ReminderDate,
                                    Quantities = jsteps.Quantities,
                                    RealQuantities = jsteps.RealQuantities,
                                    StatusId = jsteps.StatusId,
                                    StatusName = jsteps.Status.Name,
                                    Note = jsteps.Note,
                                    UpdatedDate = jsteps.UpdatedDate
                                };
                                jStepObj.RelatedEmployeeName = getRelatedEmployeeName(jsteps.RelatedEmployees, employees);

                                model.JobSteps.Add(jStepObj);
                            }
                        }
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getRelatedEmployeeName(string relatedIds, List<ModelSelectItem> employees)
        {
            if (!string.IsNullOrEmpty(relatedIds))
            {
                var Ids = relatedIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                var relatedEs = employees.Where(x => Ids.Contains(x.Value)).Select(x => x.Name).ToArray();
                return string.Join(",", relatedEs);
            }
            return "";
        }

        public PagedList<ChecklistModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<Checklist> objs = null;
                    if (string.IsNullOrEmpty(keyWord))
                        objs = db.Checklist.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedDate);
                    else
                        objs = db.Checklist.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())).OrderByDescending(x => x.CreatedDate);

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<ChecklistModel>(objs.Select(x => new ChecklistModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Note = x.Note,
                        CreatedDate = x.CreatedDate,
                        LineId = x.LineId,
                        LineName = (x.LineId.HasValue ? x.Line.Name : ""),
                        POId = x.POId,
                        ProductId = x.ProductId,
                        ProductName = (x.ProductId.HasValue ? x.Product.Name : ""),
                        CustomerId = x.CustomerId,
                        CustomerName = (x.CustomerId.HasValue ? x.Customer.Name : ""),
                        Productivity = x.Productivity,
                        Quantities = x.Quantities,
                        ProductionDays = x.ProductionDays,
                        DeliveryDate = x.DeliveryDate,
                        InputDate = x.InputDate,
                        EndDate = x.EndDate,
                        RealEndDate = x.RealEndDate,
                        StatusId = x.StatusId,
                        StatusName = x.Status.Name,
                        ProductUnit = x.Product.Unit.Name,
                        RelatedEmployees = x.RelatedEmployees
                    }).OrderBy(sorting).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ChecklistModel> Gets(string keyword)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    IQueryable<Checklist> objs;
                    if (!string.IsNullOrEmpty(keyword))
                        objs = db.Checklist.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()));
                    else
                        objs = db.Checklist.Where(x => !x.IsDeleted);
                    return objs
                         .OrderByDescending(x => x.CreatedDate)
                  .Select(x => new ChecklistModel()
                  {
                      Id = x.Id,
                      Name = x.Name,
                      Note = x.Note,
                      CreatedDate = x.CreatedDate,
                      LineId = x.LineId,
                      LineName = (x.LineId.HasValue ? x.Line.Name : ""),
                      POId = x.POId,
                      ProductId = x.ProductId,
                      ProductName = (x.ProductId.HasValue ? x.Product.Name : ""),
                      CustomerId = x.CustomerId,
                      CustomerName = (x.CustomerId.HasValue ? x.Customer.Name : ""),
                      SizeName = (x.ProductId.HasValue ? x.Product.Size.Name : ""),
                      Productivity = x.Productivity,
                      Quantities = x.Quantities,
                      ProductionDays = x.ProductionDays,
                      DeliveryDate = x.DeliveryDate,
                      InputDate = x.InputDate,
                      EndDate = x.EndDate,
                      RealEndDate = x.RealEndDate,
                      StatusId = x.StatusId,
                      StatusName = x.Status.Name,
                      ProductUnit = x.Product.Unit.Name,
                      RelatedEmployees = x.RelatedEmployees
                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(ChecklistModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên mẫu này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        Checklist obj;
                        if (model.Id == 0)
                        {
                            obj = new Checklist();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;

                            var templateJobSteps = db.Template_CL_JobStep.Where(x => !x.IsDeleted && !x.Template_Checklist.IsDeleted && x.TemplateId == model.TemplateId).ToList();

                            if (templateJobSteps.Count > 0)
                            {
                                Checklist_JobStep checklist_JobStep = null;
                                Checklist_Job checklistJob = null;
                                obj.Checklist_JobStep = new List<Checklist_JobStep>();
                                foreach (var item in templateJobSteps)
                                {
                                    checklist_JobStep = new Checklist_JobStep();
                                    checklist_JobStep.Checklist = obj;
                                    checklist_JobStep.Name = item.Name;
                                    checklist_JobStep.StepIndex = item.StepIndex;
                                    checklist_JobStep.JobStepContent = item.JobStepContent;
                                    checklist_JobStep.StatusId = 1;
                                    checklist_JobStep.CreatedUser = model.ActionUser;
                                    checklist_JobStep.CreatedDate = obj.CreatedDate;

                                    var jobs = item.Template_CL_Job.Where(x => !x.IsDeleted).ToList();
                                    if (jobs.Count > 0)
                                    {
                                        checklist_JobStep.Checklist_Job = new List<Checklist_Job>();
                                        foreach (var jobItem in jobs)
                                        {
                                            checklistJob = new Checklist_Job();
                                            checklistJob.JobIndex = jobItem.JobIndex;
                                            checklistJob.Name = jobItem.Name;
                                            checklistJob.JobContent = jobItem.JobContent;
                                            checklistJob.FakeId = jobItem.Id;
                                            checklistJob.ParentId = jobItem.ParentId;
                                            checklistJob.Checklist_JobStep = checklist_JobStep;
                                            checklistJob.StatusId = 1;
                                            checklistJob.CreatedUser = model.ActionUser;
                                            checklistJob.CreatedDate = obj.CreatedDate;
                                            checklist_JobStep.Checklist_Job.Add(checklistJob);
                                        }
                                    }
                                    obj.Checklist_JobStep.Add(checklist_JobStep);
                                }
                            }
                            db.Checklist.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                            result.Data = obj.Id;
                        }
                        else
                        {
                            obj = db.Checklist.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Loại mẫu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo mã hàng này nên bạn không cập nhật được thông tin cho mã hàng này." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.LineId = model.LineId;
                                    obj.Quantities = model.Quantities;
                                    obj.ProductionDays = model.ProductionDays;
                                    obj.Productivity = model.Productivity;
                                    obj.DeliveryDate = model.DeliveryDate;
                                    obj.InputDate = model.InputDate;
                                    obj.EndDate = model.EndDate;
                                    obj.StatusId = model.StatusId;
                                    obj.Note = model.Note;
                                    obj.RelatedEmployees = model.RelatedEmployees;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                    result.Data = 0;
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckExists(string code, int? id)
        {
            try
            {
                var obj = db.Checklist.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(code) && x.Id != id);
                if (obj == null)
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
                    var objs = db.Checklist.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (objs == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Loại mẫu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(objs, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo mã hàng này nên bạn không xóa được mã hàng này." });
                        }
                        else
                        {
                            objs.IsDeleted = true;
                            objs.DeletedUser = acctionUserId;
                            objs.DeletedDate = DateTime.Now;

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

        public List<ModelSelectItem> GetSelectItem()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var selectItems = new List<ModelSelectItem>();
                    var objs = db.Checklist.Where(x => !x.IsDeleted).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name
                        }).ToList();

                    if (objs != null && objs.Count() > 0)
                    {
                        selectItems.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn mẫu  - - " });
                        selectItems.AddRange(objs);
                    }
                    else
                        selectItems.Add(new ModelSelectItem() { Value = 0, Name = "  Không có mẫu  " });
                    return selectItems;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetSelectItem(string keyword, int userId, bool isAdmin)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var selectItems = new List<ModelSelectItem>();
                    var objs = db.Checklist.Where(x => !x.IsDeleted);
                    if (!string.IsNullOrEmpty(keyword))
                        objs = objs.Where(x => x.Name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()));
                    if (!isAdmin)
                    {
                        string _userId = ("," + userId + ",");
                        objs = objs.Where(x => x.RelatedEmployees != null && ("," + x.RelatedEmployees + ",").Contains(_userId));
                    }

                    if (objs != null && objs.Count() > 0)
                    {
                        selectItems.AddRange(objs.Select(
                                          x => new ModelSelectItem()
                                          {
                                              Value = x.Id,
                                              Name = x.Name
                                          }).ToList());
                    }
                    return selectItems;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
