using GPRO.Core.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class RoleController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Gets(string keyWord, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                if (isAuthenticate)
                {
                    var roles = BLLRole.Instance.GetListRole(keyWord, jtStartIndex, jtPageSize, jtSorting, UserContext.UserID, UserContext.CompanyId, UserContext.IsOwner);
                    JsonDataResult.Records = roles;
                    JsonDataResult.Result = "OK";
                    JsonDataResult.TotalRecordCount = roles.TotalItemCount;
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

        public ActionResult Create(int? id)
        {
            try
            {
                if (isAuthenticate)
                {
                    ViewData["Modules"] = BLLRole.Instance.GetListModuleByUserId(UserContext.UserID);
                    ViewData["Features"] = BLLRole.Instance.GetListFeatureByUserId(UserContext.UserID);
                    ViewData["Permissions"] = BLLRole.Instance.GetListPermissionByUserId(UserContext.UserID);
                    int ID = id ?? 0; // nullable  default value is 0;
                    if (ID != 0)
                    {
                        var roleDetail = BLLRole.Instance.GetRoleDetailByRoleId(ID);
                        if (roleDetail != null)
                        {
                            ViewData["RoleDetail"] = roleDetail;
                            ViewData["RolePermission"] = BLLRole.Instance.GetListRolePermissionByRoleId(ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        [HttpPost]
        public JsonResult Save(int id, string roleName, string description, List<string> listPermission)
        {
            ResponseBase result;
            try
            {
                if (isAuthenticate)
                {
                    SRoLe role = new SRoLe();
                    role.Id = id;
                    role.RoleName = roleName;
                    role.IsSystem = false;
                    role.CompanyId = UserContext.CompanyId;
                    role.CreatedUser = UserContext.UserID;
                    role.CreatedDate = DateTime.Now;
                    role.Description = description;
                    if (id == 0)
                    {
                        result = BLLRole.Instance.Create(role, listPermission);
                    }
                    else
                    {
                        result = BLLRole.Instance.Update(role, listPermission);
                    }
                    if (result.IsSuccess)
                    {
                        JsonDataResult.Result = "OK";
                    }
                    else
                    {
                        JsonDataResult.Result = "ERROR";
                        JsonDataResult.ErrorMessages.AddRange(result.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                    responseResult = BLLRole.Instance.DeleteById(id, UserContext.UserID);
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
                //add Error
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi Dữ Liệu", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }
    }
}