using GPRO.Core.Mvc;
using SanXuatChecklist.Business;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class ChecklistJobController : BaseController
    {
        [HttpPost]
        public JsonResult GetByParentId(int parentId)
        {
            try
            {
                if (isAuthenticate)
                {
                    var objs = BLLChecklistJob.Instance.Gets(parentId);
                    JsonDataResult.Records = objs;
                    JsonDataResult.Result = "OK";
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult AdminUpdate(Checklist_JobModel model)
        {
            ResponseBase responseResult; 
            try
            {
                if (isAuthenticate)
                {
                    model.ActionUser = UserContext.UserID;
                    responseResult = BLLChecklistJob.Instance.AdminUpdate(model, isOwner);
                    if (!responseResult.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                    }
                    else
                        JsonDataResult.Result = "OK";
                }
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Update ", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult DoneJob(int jobId)
        {
            try
            {
                var rs = BLLChecklistJob.Instance.UpdateStatus(jobId, UserContext.UserID, (int)eStatus.Done, UserContext.EmployeeName);
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
            }
            return Json(JsonDataResult);
        }
    }
}