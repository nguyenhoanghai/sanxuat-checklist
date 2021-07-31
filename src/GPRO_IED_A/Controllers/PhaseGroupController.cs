using GPRO.Core.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Model;

namespace SanXuatCheckList.Controllers
{
    public class PhaseGroupController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Gets(string keyword, int searchBy, int jtStartIndex=0, int jtPageSize=1000, string jtSorting="")
        {
            try
            {
                if (isAuthenticate)
                { 
                    var phaseGroups = BLLPhaseGroup.Instance.GetList(keyword, searchBy, jtStartIndex, jtPageSize, jtSorting,UserContext.WorkshopIds);
                    JsonDataResult.Records = phaseGroups;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = phaseGroups.TotalItemCount;
                }
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List ObjectType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(PhaseGroupModel phaseGroup)
        {
            ResponseBase rs;
            try
            {
                if (isAuthenticate)
                {
                    phaseGroup.ActionUser = UserContext.UserID; 
                    rs = BLLPhaseGroup.Instance.InsertOrUpdate(phaseGroup, isOwner);
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
                throw (ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            ResponseBase result;
            try
            {
                if (isAuthenticate)
                {
                    result = BLLPhaseGroup.Instance.Delete(Id, UserContext.UserID, isOwner);
                    if (!result.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(result.Errors);
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetSelect()
        {
            try
            {

                JsonDataResult.Data = BLLPhaseGroup.Instance.Gets(UserContext.WorkshopIds);
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Delete Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

    }
}