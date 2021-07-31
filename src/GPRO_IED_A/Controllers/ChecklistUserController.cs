using GPRO.Core.Mvc;
using SanXuatChecklist.Business;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class ChecklistUserController : BaseController
    {
        public ActionResult Index(int? Id)
        {
            ViewData["Id"] = Id ?? 0;
            var modelE = new UserInfoModel();
            modelE.UserId = UserContext.UserID;
            modelE.EmployeeName = UserContext.EmployeeName.ToString();
            modelE.LogoCompany = UserContext.LogoCompany != null ? UserContext.LogoCompany.ToString() : "";
            modelE.Email = UserContext.Email == null ? "" : UserContext.Email.ToString();
            modelE.ImagePath = UserContext.ImagePath == null ? "" : UserContext.ImagePath.ToString();
            ViewData["userInfo"] = modelE;
            return View();
        }


        public JsonResult GetAllCheckList(string keyword)
        {
            try
            {
                JsonDataResult.Result = "OK";
                bool isAdmin = UserContext.IsOwner;
                JsonDataResult.Data = BLLChecklist.Instance.GetSelectItem(keyword, UserContext.UserID, isAdmin);
            }
            catch (Exception ex)
            {
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetAllJob(int checklistId)
        {
            try
            {
                bool isAdmin = true;
                JsonDataResult.Result = "OK";
                if (UserContext.Permissions.FirstOrDefault(x => x.Equals("/Checklist/Index")) == null)
                    isAdmin = false;
                //if (isGetReport)
                //{
                var jobs = BLLChecklistJob.Instance.GetJobs(checklistId, UserContext.UserID, isAdmin);
                JsonDataResult.Data = jobs;
                //}
                //else
                //{
                //    var jobs = bllCL.GetAllJob(UserContext.IsOwner, UserContext.UserID, proTimeId, CommonFunction.GetOrganizationFull(UserContext.CompanyID ?? 0), AppGlobal.Account.GetService().GetUsersBycompanyId(UserContext.CompanyID ?? 0), hostUrl);
                //    JsonDataResult.Data = jobs;
                //}
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetJobById(int jobId)
        {
            try
            {
                var job = BLLChecklistJob.Instance.GetJobById(jobId, UserContext.UserID);
                if (job != null && job.Attachs.Count > 0)
                {
                    for (int i = 0; i < job.Attachs.Count; i++)
                    {
                        //var arr = job.Attachs[i].Url.Split('/').ToList();
                        //var id = arr[arr.Count - 1];
                        //if (job.Attachs[i].Url.ToLower().Contains("sampletickets") || job.Attachs[i].Url.ToLower().Contains("phieu-lay-mau"))
                        //    job.Attachs[i].Name = ProManaApi.Instance.GetFileName(1, int.Parse(id));
                        //else if (job.Attachs[i].Url.ToLower().Contains("testrecords") || job.Attachs[i].Url.ToLower().Contains("ho-so"))
                        //    job.Attachs[i].Name = ProManaApi.Instance.GetFileName(2, int.Parse(id));
                    }
                }
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = job;
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult ChangeJobStatus(int jobId, int statusId)
        {
            try
            {
                if (statusId == (int)eStatus.Done)
                {
                    if (UserContext.Permissions.FirstOrDefault(x => x.Equals("/Checklist/Index")) != null)
                    {
                        var rs = BLLChecklistJob.Instance.UpdateStatus(jobId, UserContext.UserID, statusId, UserContext.EmployeeName);
                        if (!rs.IsSuccess)
                        {
                            JsonDataResult.Result = "ERROR";
                            JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                        }
                        else
                            JsonDataResult.Result = "OK";
                    }
                    else
                    {
                        JsonDataResult.Result = "NOPERMISSION";
                        JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "update", Message = "Tài khoản của bạn không có quyền di chuyển công việc sang trạng thái hoàn tất." });
                    }
                }
                else
                {
                    var rs = BLLChecklistJob.Instance.UpdateStatus(jobId, UserContext.UserID, statusId, UserContext.EmployeeName);
                    if (!rs.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                    }
                    else
                        JsonDataResult.Result = "OK";
                }
            }
            catch (Exception ex)
            {
            }
            return Json(JsonDataResult);
        }


        #region comment
        public JsonResult SaveComment(int jobId, string comment)
        {
            try
            {
                var rs = BLLChecklistJobComment.Instance.InsertOrUpdate(jobId, UserContext.UserID, comment, UserContext.EmployeeName);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }
        #endregion

        #region Loi phat sinh
        public JsonResult SaveError(int jobId, string code, DateTime time, string note)
        {
            try
            {
                var rs = BLLChecklistJobError.Instance.InsertOrUpdate(jobId, code, UserContext.UserID, note, time);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult DeleteError(int jobId, int Id)
        {
            try
            {
                var rs = BLLChecklistJobError.Instance.Delete(jobId, Id, UserContext.UserID, UserContext.EmployeeName);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult SaveErrorPro(int jobId, int JobErrId, DateTime time, string note)
        {
            try
            {
                var rs = BLLChecklistJobError.Instance.ErrorProcess(jobId, JobErrId, UserContext.UserID, note, time);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult SaveErrorResult(int jobId, int JobErrId, int result, DateTime time, string sms, string reason, string warning)
        {
            try
            {
                var rs = BLLChecklistJobError.Instance.ErrorResult(jobId, JobErrId, UserContext.UserID, time, result, sms, reason, warning);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        #endregion

        #region attach
        public JsonResult SaveAttach(int jobId, string name, string code, string note)
        {
            try
            {
                //   var mahoa = EncryptString(code, "");
                var rs = BLLChecklistJobAttachment.Instance.InsertOrUpdate(jobId, UserContext.UserID, UserContext.EmployeeName, name, code, note);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult DeleteAttach(int jobId, int Id)
        {
            try
            {
                var rs = BLLChecklistJobAttachment.Instance.Delete(jobId, Id, UserContext.UserID, UserContext.EmployeeName);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        #endregion

        #region Alert
        public JsonResult GetAlerts()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLChecklistJobAlert.Instance.GetAlerts(UserContext.UserID.ToString());
            }
            catch (Exception ex)
            {
                //  CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult DisableAlert(int Id)
        {
            try
            {
                BLLChecklistJobAlert.Instance.DisableAlert(Id);
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //  CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        #endregion

        #region Request Alarm
        public JsonResult RequestAlarm()
        {
            try
            {
                var request = BLLChecklistJob.Instance.RequestAlarm(UserContext.UserID, UserContext.CompanyId);

                return Json(request);
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public JsonResult StopAlarm(int Id)
        {
            try
            {
                var result = BLLChecklistJob.Instance.StopAlarm(Id, UserContext.UserID);
                if (result.IsSuccess)
                    JsonDataResult.Result = "OK";
                else
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
                }
                return Json(JsonDataResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}