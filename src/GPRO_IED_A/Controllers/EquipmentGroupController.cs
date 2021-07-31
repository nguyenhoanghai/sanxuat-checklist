using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Data;
using SanXuatCheckList.Business.Model;

namespace SanXuatCheckList.Controllers
{
    public class EquipmentGroupController : BaseController
    {
        // GET: EquipmentGroup
        public ActionResult Index()
        {
            return View();
        }
        #region
        [HttpPost]
        public JsonResult Gets(string keyword, int searchBy, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                if (isAuthenticate)
                {
                    var E_Groups = BLLEquipmentGroup.Instance.GetList(keyword, searchBy, jtStartIndex, jtPageSize, jtSorting);
                    JsonDataResult.Records = E_Groups;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = E_Groups.TotalItemCount;
                }
             }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(T_EquipmentGroup E_Group)
        {
            ResponseBase rs;
            try
            {
                if (isAuthenticate)
                {
                    if (E_Group.Id == 0)
                    {
                        E_Group.CreatedUser = UserContext.UserID;
                        E_Group.CreatedDate = DateTime.Now;
                    }
                    else
                    {
                        E_Group.UpdatedUser = UserContext.UserID;
                        E_Group.UpdatedDate = DateTime.Now;
                    }
                    rs = BLLEquipmentGroup.Instance.InsertOrUpdate(E_Group, isOwner);
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
                if (isAuthenticate)
                {
                    result = BLLEquipmentGroup.Instance.DeleteById(Id, UserContext.UserID, isOwner);
                    if (!result.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(result.Errors);
                    }
                    else
                        result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }
        [HttpPost]
        public JsonResult GetSelects()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLEquipmentGroup.Instance.GetE_Group_Select();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }
        #endregion
    }
}