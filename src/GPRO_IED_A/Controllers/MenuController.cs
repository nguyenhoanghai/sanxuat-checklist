﻿using GPRO.Core.Mvc;
using SanXuatCheckList.App_Global;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Helper;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class MenuController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Module = CommonFunction.Instance.GetModuleSelect(UserContext.CompanyId);
            ViewBag.MenuCategory = CommonFunction.Instance.GetMenuCategorySelect(UserContext.CompanyId);
            return View();
        }

        [HttpPost]
        public JsonResult Gets(string keyWord, string searchBy, string categoryId, string moduleId, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                //if (isAuthenticate)
                //{
                    var listMenu = BLLMenu.Instance.GetList(keyWord, searchBy, categoryId, moduleId, AppGlobal.MODULE_NAME, jtStartIndex, jtPageSize, jtSorting, UserContext.CompanyId, UserContext.UserID);
                    JsonDataResult.Records = listMenu;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = listMenu.TotalItemCount;
              //  }
            }
            catch (Exception ex)
            {
                //add Error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List ObjectType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(MenuModel model)
        {
            ResponseBase rs;
            try
            {
                //if (isAuthenticate)
                //{
                    model.CompanyId = UserContext.CompanyId;
                    rs = BLLMenu.Instance.InsertOrUpdate(model, UserContext.UserID);
                    if (!rs.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                        if (model.Icon != "0")
                        {
                            string path = model.Icon.Split(',').ToList().First();
                            if (System.IO.File.Exists(Server.MapPath(path)))
                            {
                                System.IO.File.Delete(Server.MapPath(path));
                            }
                        }
                    }
                    JsonDataResult.Result = "OK";
                //}
            }
            catch (Exception ex)
            {
                //add Error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponseBase responseResult;
            try
            {
                //if (isAuthenticate)
                //{
                    responseResult = new ResponseBase();
                    responseResult = BLLMenu.Instance.Delete(id, UserContext.UserID);
                    if (!responseResult.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                    }
                    JsonDataResult.Result = "OK";
            //    }
            }
            catch (Exception ex)
            {
                //add Error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetSystemMenus(string keyWord, string searchBy, string categoryName, string moduleId, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                //if (isAuthenticate)
                //{
                    var listMenu = BLLMenu.Instance.GetList(keyWord, searchBy, categoryName, moduleId, AppGlobal.MODULE_NAME, jtStartIndex, jtPageSize, jtSorting, 0, UserContext.UserID);
                    JsonDataResult.Records = listMenu;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = listMenu.TotalItemCount;
               // }
            }
            catch (Exception ex)
            {
                //add Error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List ObjectType", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult UpdateSystemMenu(MenuModel modelMenu)
        {
            ResponseBase responseResult;
            try
            {
                //if (isAuthenticate)
                //{
                    responseResult = BLLMenu.Instance.InsertOrUpdate(modelMenu, UserContext.UserID);
                    if (!responseResult.IsSuccess)
                    {
                        if (modelMenu.Icon != "0")
                        {
                            string path = modelMenu.Icon.Split(',').ToList().First();
                            if (System.IO.File.Exists(Server.MapPath(path)))
                            {
                                System.IO.File.Delete(Server.MapPath(path));
                            }
                        }
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                    }
                    JsonDataResult.Result = "OK";
               // }
            }
            catch (Exception ex)
            {
                //add Error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }
    }
}