using SanXuatCheckList.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SanXuatCheckList.Business;
using GPRO.Core.Mvc;
using SanXuatCheckList.Helper;

namespace SanXuatCheckList.Controllers
{
    public class EquipmentTypeController : BaseController
    { 

        #region EquipmentType
        public ActionResult Index()
        {
            ViewBag.eType_default = CommonFunction.Instance.GetEquipType_DefaultSelectList( );
            return View();
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            ResponseBase rs;
            try
            {
                if (isAuthenticate)
                {
                    rs = BLLEquipmentType.Instance.DeleteById(Id, UserContext.UserID);
                    if (rs.IsSuccess)
                        JsonDataResult.Result = "OK";
                    else
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Delete EquipmentType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult CheckDelete(int Id)
        {
            ResponseBase responseResult;
            try
            {
                if (isAuthenticate)
                {
                    responseResult = BLLEquipmentType.Instance.CheckExistInEquipmentAtt(Id);
                    if (responseResult.IsSuccess)
                        JsonDataResult.Result = "OK";
                    else
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Delete EquipmentType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult Gets(string keyword, int searchBy, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                if (isAuthenticate)
                {
                    var listEquipmentType = BLLEquipmentType.Instance.GetList(keyword, searchBy, jtStartIndex, jtPageSize, jtSorting, UserContext.CompanyId);
                    JsonDataResult.Records = listEquipmentType;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = listEquipmentType.TotalItemCount;
                }
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List ObjectType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult Save(ModelEquipmentType model)
        {
            ResponseBase responseResult;
            try
            {
                if (isAuthenticate)
                {
                    model.CompanyId = UserContext.CompanyId;
                    model.ActionUser = UserContext.UserID;
                    if (model.Id == 0)
                        responseResult = BLLEquipmentType.Instance.Create(model);
                    else
                        responseResult = BLLEquipmentType.Instance.Update(model);
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

        [HttpPost]
        public JsonResult GetSelects()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLEquipmentType.Instance.GetListEquipmentType();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }
        #endregion

        #region attr
        public ActionResult CreateAttibute(int id)
        {
            if (isAuthenticate)
            {
                try
                {
                    var listModelSelect = BLLEquipmentType.Instance.GetListEquipmentType();
                    List<SelectListItem> listEquipmentType = new List<SelectListItem>();
                    if (listModelSelect != null && listModelSelect.Count > 0)
                    {
                        listEquipmentType = listModelSelect.Select(c => new SelectListItem
                        {
                            Text = c.Name,
                            Value = c.Value.ToString(),
                        }).ToList();
                    }
                    ViewBag.listEquipmentType = listEquipmentType;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                Session["equipmentTypeId"] = id;
                ViewBag.equipmentTypeId = id;
                ViewBag.equipmentTypeName = BLLEquipmentType.Instance.GetEquipmentTypeNameById(id);
            }
            return View();
        }

        public JsonResult DeleteETypeAttr(int Id)
        {
            ResponseBase responseResult;
            try
            {
                if (isAuthenticate)
                {
                    responseResult = BLLEquipmentTypeAttribute.Instance.DeleteById(Id, UserContext.UserID);
                    if (responseResult.IsSuccess)
                        JsonDataResult.Result = "OK";
                    else
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Delete Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetETypeAttr(int equipId, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                if (isAuthenticate)
                {
                    var objs = BLLEquipmentTypeAttribute.Instance.GetList(jtStartIndex, jtPageSize, jtSorting, equipId);
                    JsonDataResult.Records = objs;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = objs.TotalItemCount;
                }
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List ObjectType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult SaveETypeAttr(ModelEquipmentTypeAttribute model)
        {
            ResponseBase responseResult;
            try
            {
                if (isAuthenticate)
                {
                    model.ActionUser = UserContext.UserID;
                    if (model.Id == 0)
                        responseResult = BLLEquipmentTypeAttribute.Instance.Create(model);
                    else
                        responseResult = BLLEquipmentTypeAttribute.Instance.Update(model);
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
        #endregion
    }
}