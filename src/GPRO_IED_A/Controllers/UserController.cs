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
    public class UserController : BaseController
    { 
        public ActionResult Index()
        {
            List<ModelSelectItem> roles = null;
            List<SelectListItem> rolesItem = new List<SelectListItem>();
            List<SelectListItem> wksItem = new List<SelectListItem>();
            try
            {
                roles = BLLUserRole.Instance.GetUserRolesModelByUserId(UserContext.UserID, UserContext.IsOwner, UserContext.CompanyId);
                if (roles == null)
                {
                    //return Error Page

                }
                rolesItem.AddRange(roles.Select(x => new SelectListItem() { Text = x.Name, Value = x.Value.ToString() }).ToList());
                ViewData["roles"] = rolesItem;
               var wks =  BLLWorkshop.Instance.GetListWorkShop();
                if (wks.Count > 0)
                {
                    wksItem.AddRange(wks.Select(x => new SelectListItem() { Text = x.Name, Value = x.Value.ToString() }).ToList());
                }
                ViewData["workshops"] = wksItem;
            }
            catch (Exception ex)
            {
                // add Error
                throw ex;
            }
            return View();
        }
        public ActionResult Profile()
        {
            var obj = BLLUser.Instance.Get(UserContext.UserID);
            return View(obj);
        }

        [HttpPost]
        public JsonResult Gets(string keyWord, int searchBy, bool isBlock, bool isRequiredChangePass, bool isTimeBlock, bool isForgotPass, int jtStartIndex = 0, int jtPageSize = 1000, string jtSorting = "")
        {
            try
            {
                if (isAuthenticate)
                {
                    var Users = BLLUser.Instance.Gets(keyWord, searchBy, isBlock, isRequiredChangePass, isTimeBlock, isForgotPass, jtStartIndex, jtPageSize, jtSorting, UserContext.UserID, UserContext.CompanyId, UserContext.ChildCompanyId);
                    JsonDataResult.Records = Users;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = Users.TotalItemCount;
                }
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
        public JsonResult Save(UserModel model)
        {
            ResponseBase responseResult = null;
            try
            {
                if (isAuthenticate)
                {
                    model.CompanyId = UserContext.CompanyId;
                    model.ActionUser = UserContext.UserID;
                    responseResult = BLLUser.Instance.InsertOrUpdate(model);
                    if (!responseResult.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.Add(new Error() { MemberName = responseResult.Errors.First().MemberName, Message = "Lỗi: " + responseResult.Errors.First().Message });
                    }
                    else
                        JsonDataResult.Result = "OK";
                }
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
                if (isAuthenticate)
                {
                    responseResult = new ResponseBase();
                    responseResult = BLLUser.Instance.Delete(id, UserContext.UserID);
                    if (!responseResult.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.Add(new Error() { MemberName = responseResult.Errors.First().MemberName, Message = "Lỗi: " + responseResult.Errors.First().Message });
                    }
                    else
                        JsonDataResult.Result = "OK";
                }
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
        public JsonResult ChangePassword(string id, string Password)
        {
            ResponseBase responseResult = null;
            try
            { 
                    responseResult = BLLUser.Instance.UpdatePassword(UserContext.UserID, int.Parse(id), Password);
                    if (!responseResult.IsSuccess)
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.Add(new Error() { MemberName = responseResult.Errors.First().MemberName, Message = "Lỗi: " + responseResult.Errors.First().Message });
                    }
                    else
                        JsonDataResult.Result = "OK";
              }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult ChangeAvatar(string img)
        {
            ResponseBase responseResult = null;
            try
            {
                responseResult =  BLLUser.Instance.ChangeAvatar(UserContext.UserID, img);
                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new Error() { MemberName = responseResult.Errors.First().MemberName, Message = "Lỗi: " + responseResult.Errors.First().Message });
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult ChangeInfo(string mail, string first, string last)
        {
            ResponseBase responseResult = null;
            try
            {
                responseResult = BLLUser.Instance.ChangeInfo(UserContext.UserID, mail, first, last);
                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new Error() { MemberName = responseResult.Errors.First().MemberName, Message = "Lỗi: " + responseResult.Errors.First().Message });
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult ChangePass(string oldPass, string newPass)
        {
            ResponseBase responseResult = null;
            try
            {
                responseResult = BLLUser.Instance.ChangePassword(UserContext.UserID, oldPass, newPass);
                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new Error() { MemberName = responseResult.Errors.First().MemberName, Message = "Lỗi: " + responseResult.Errors.First().Message });
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetSelectList(string userIds)
        {
            try
            { 
                JsonDataResult.Result = "OK";
                JsonDataResult.Records = BLLUser.Instance.GetSelectItem(userIds);
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