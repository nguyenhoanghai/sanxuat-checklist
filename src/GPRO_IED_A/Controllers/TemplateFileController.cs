using GPRO.Core.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class TemplateFileController : BaseController
    {
        public ActionResult Index(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                ViewBag.type = type;
                var id = BLLTemplateFile.Instance.GetTemplateFileTypeIdByCode(type);
                ViewBag.tempType = id;
            }
            else
            {
                ViewBag.type = "";
                ViewBag.tempType = 0;
            }
            return View();
        }

        //public ActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult Gets(string type, string keyword, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLTemplateFile.Instance.GetList(type, keyword, jtStartIndex, jtPageSize, jtSorting);
                if (objs != null && objs.Count > 0)
                {
                    var employ = BLLUser.Instance.GetSelectItem(null);
                    if (employ != null && employ.Count > 0)
                    {
                        foreach (var item in objs)
                        {
                            if (item.ApprovedUser != null)
                            {
                                var e = employ.FirstOrDefault(x => x.Value == item.ApprovedUser.Value);
                                item.ApproveUserName = e != null ? e.Name : string.Empty;
                            }
                        }
                    }
                }
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetById(int Id)
        {
            try
            {
                var templatefile = BLLTemplateFile.Instance.Get(Id);
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = templatefile;
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(TemplateFileModel obj)
        {
            ResponseBase responseResult;
            try
            {
                obj.ActionUser = UserContext.UserID;
                responseResult = BLLTemplateFile.Instance.CreateOrUpdate(obj);

                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                }
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //  CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        public JsonResult Delete(int Id)
        {
            ResponseBase result;
            try
            {
                // result = bllMaterialType.Delete(Id, UserContext.UserID);
                result = BLLTemplateFile.Instance.Delete(Id, UserContext.UserID);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
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

        [HttpPost]
        public JsonResult GetSelectList(string type)
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLTemplateFile.Instance.GetSelectList(type);
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetLastIndex()
        {
            JsonDataResult.Records = BLLTemplateFile.Instance.GetLastIndex();
            JsonDataResult.Data = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.TemplateFile);
            return Json(JsonDataResult);
        }
    }
}