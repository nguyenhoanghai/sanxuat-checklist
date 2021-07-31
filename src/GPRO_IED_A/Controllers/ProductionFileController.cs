using GPRO.Core.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Helper;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class ProductionFileController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region ProductionFile
        [HttpPost]
        public JsonResult Gets(string keyword, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLProductionFile.Instance.GetList(keyword, jtStartIndex, jtPageSize, jtSorting);
                if (objs.Count > 0)
                {
                    var users = BLLUser.Instance.GetSelectItem(null);
                    //var materials = WareHouseApi.Instance.GetMaterial(0);
                    ModelSelectItem found;
                    foreach (var item in objs)
                    {
                        found = users.FirstOrDefault(x => x.Value == item.ApprovedUser);
                        if (found != null)
                            item.ApproverName = found.Name;

                        //found = materials.FirstOrDefault(x => x.Value == item.ProductId);
                        //if (found != null)
                        //    item.ProductionBatchName = (item.ProductionBatchName + " (" + found.Name + ")");
                    }
                }
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetById(int index)
        {
            try
            {
                var productionfile = BLLProductionFile.Instance.GetByCode(index);
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = productionfile;
            }
            catch (Exception ex)
            {
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(ProductionFileModel obj)
        {
            ResponseBase responseResult;
            try
            {

                obj.ActionUser = UserContext.UserID;
                responseResult = BLLProductionFile.Instance.CreateOrUpdate(obj);
                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                }
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                // CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            ResponseBase result;
            try
            {
                result = BLLProductionFile.Instance.Delete(Id, UserContext.UserID);
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
        public JsonResult GetSelectList()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLProductionFile.Instance.GetSelectList();
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetNewCode()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLProductionFile.Instance.GetNewCode();
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }
         
        #endregion
    }
}