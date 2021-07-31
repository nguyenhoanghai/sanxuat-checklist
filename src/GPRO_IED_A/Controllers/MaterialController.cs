using GPRO.Core.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class MaterialController : BaseController
    {  
        #region Material
        [HttpPost]
        public JsonResult Gets(int mTypeId, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLMaterial.Instance.GetList(mTypeId, jtStartIndex, jtPageSize, jtSorting);
                
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                // CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetNorms(int mId)
        {
            try
            {
                var objs = BLLMaterial.Instance.GetNorms(mId); 
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                // CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save ( MaterialModel obj)
        {
            ResponseBase responseResult;
            try
            {
                obj.ActionUser = UserContext.UserID;
                responseResult = BLLMaterial.Instance.CreateOrUpdate(obj,isOwner);
                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                }
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                //CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete (int Id)
        {
            ResponseBase result;
            try
            {
                result = BLLMaterial.Instance.Delete(Id, UserContext.UserID,isOwner);
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
                //CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetSelectList(int MtypeId)
        {
            try
            {
                JsonDataResult.Result = "OK";
                var objs = BLLMaterial.Instance.GetSelectList(MtypeId); 
                JsonDataResult.Data = objs;
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
            JsonDataResult.Records = BLLMaterial.Instance.GetMaterialLastIndex();
            JsonDataResult.Data = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
            return Json(JsonDataResult);
        }

        public JsonResult GetMaterialSelect_FilterByTextInput(string text)
        {
            return Json(BLLMaterial.Instance.GetMaterialSelect_FilterByTextInput(text), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}