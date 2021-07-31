using GPRO.Core.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using System;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class DeliveryController : BaseController
    {
        // GET: Delivery
        public ActionResult Index()
        {
            return View();
        }

        #region Delivery

        [HttpPost]
        public JsonResult Gets(string keyword, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLDelivery.Instance.GetList(keyword, jtStartIndex, jtPageSize, jtSorting);

                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                //CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetReport(int custId, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLDelivery.Instance.GetList(custId, jtStartIndex, jtPageSize, jtSorting);

                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                //CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }


        [HttpPost]
        public JsonResult Save(DeliveryModel obj)
        {
            ResponseBase rs;
            try
            {
                obj.ActionUser = UserContext.UserID;
                rs = BLLDelivery.Instance.CreateOrUpdate(obj);
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
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            ResponseBase result;
            try
            {
                result = BLLDelivery.Instance.Delete(Id, UserContext.UserID);
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
        public JsonResult GetDeliverySelect()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLDelivery.Instance.GetSelectList();
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetById(int Id)
        {
            try
            {
                var obj = BLLDelivery.Instance.GetById(Id);
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = obj;
            }
            catch (Exception ex)
            {
                //CatchError(ex);
                throw ex;
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetLastIndex()
        {
            JsonDataResult.Records = BLLDelivery.Instance.GetLastIndex();
            JsonDataResult.Data = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Delivery);
            return Json(JsonDataResult);
        }
        #endregion
    }
}