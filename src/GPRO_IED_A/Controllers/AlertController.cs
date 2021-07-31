using GPRO.Core.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class AlertController : BaseController
    { 
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Gets(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var jobs = BLLChecklistJobAlert.Instance.GetList(UserContext.UserID.ToString() ,   jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = jobs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = jobs.TotalItemCount;
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List ObjectType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult ChangeStatus(int Id)
        {
            try
            {
                BLLChecklistJobAlert.Instance.ChangeStatus(Id);
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult Delete(int Id)
        {
            ResponseBase result;
            try
            {
                result = BLLChecklistJobAlert.Instance.Delete(Id, UserContext.UserID);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
                }
                else
                {
                    JsonDataResult.Result = "OK";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
                }
            }
            catch (Exception ex)
            {
               // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

    }
}