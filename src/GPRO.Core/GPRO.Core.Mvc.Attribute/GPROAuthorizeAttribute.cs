using GPRO.Core.Generic;
using GPRO.Core.Interface;
using GPRO.Core.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GPRO.Core.Mvc.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class GPROAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string _permission;
        private string[] _permissionSplit = new string[0];
        private string queryStringKey = "token";
        public string Permissions
        {
            get
            {
                return this._permission ?? string.Empty;
            }
            set
            {
                this._permission = value;
                this._permissionSplit = GPROAuthorizeAttribute.SplitString(value);
            }
        }
        public PermissionType[] PermissionType
        {
            get;
            set;
        }
        public CommandPermissionType CommandPermissionType
        {
            get;
            set;
        }
        public string FeatureName
        {
            get;
            set;
        }
        private IEncryptor GetEncryptor(AuthorizationContext filterContext)
        {
            return filterContext.HttpContext.Application["GETENCRYPTOR"] as IEncryptor;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!(DateTime.Now.Date > DateTime.Parse("2020-12-31")))
            {
                Authentication.PassCheckDate = DateTime.Now.Date;
                string accessingResource = string.Empty;
                if (string.IsNullOrEmpty(this.FeatureName))
                {
                    string str = Convert.ToString(filterContext.RequestContext.RouteData.DataTokens["area"]);
                    string str2 = Convert.ToString(filterContext.RequestContext.RouteData.Values["Controller"]);
                    string str3 = Convert.ToString(filterContext.RequestContext.RouteData.Values["Action"]);
                    this.FeatureName = str + str2;
                    accessingResource = "/" + str2 + "/" + str3;
                }
                if (!AjaxRequestExtensions.IsAjaxRequest(filterContext.RequestContext.HttpContext.Request))
                {
                    if (!Authentication.IsAuthenticated)
                    {
                        string text = filterContext.RequestContext.HttpContext.Request.QueryString.Get(this.queryStringKey);
                        if (text != null)
                        {
                            ArrayList arrayList = (ArrayList)this.GetEncryptor(filterContext).Deserialize(this.GetEncryptor(filterContext).Decrypt(text));
                            int num = Convert.ToInt32(arrayList[0]);
                            string b = Convert.ToString(arrayList[1]);
                            if (filterContext.HttpContext.Session.SessionID == b && num > 0)
                            {
                                Authentication.Login(num);
                                string text2 = filterContext.RequestContext.HttpContext.Request.RawUrl;
                                string value = this.queryStringKey.Trim().ToLower();
                                text2 = text2.Trim().ToLower();
                                int num2 = text2.IndexOf("token");
                                if (num2 >= 0)
                                {
                                    string[] array = text2.Substring(num2).Split(new char[]
                                    {
                                        '&'
                                    });
                                    string[] array2 = array;
                                    for (int i = 0; i < array2.Length; i++)
                                    {
                                        string text3 = array2[i];
                                        if (text3.Contains(value))
                                        {
                                            text2 = text2.Replace(text3, "").Replace("&&", "&");
                                            if (text2.EndsWith("&"))
                                            {
                                                text2 = text2.Substring(0, text2.Length - 1);
                                            }
                                        }
                                    }
                                }
                                if (text2.EndsWith("?"))
                                {
                                    text2 = text2.Substring(0, text2.Length - 1);
                                }
                                filterContext.Result = (new RedirectResult(text2));
                            }
                        }
                        else
                        {
                            HttpContextBase httpContext = filterContext.RequestContext.HttpContext;
                            httpContext.Session["$set-cookie$"] = true;
                        }
                    }
                    string value2 = filterContext.RequestContext.HttpContext.Request.QueryString.Get(this.queryStringKey);
                    if (!string.IsNullOrEmpty(value2))
                    {
                        string text2 = filterContext.RequestContext.HttpContext.Request.RawUrl;
                        string value = this.queryStringKey.Trim().ToLower();
                        text2 = text2.Trim().ToLower();
                        int num2 = text2.IndexOf("token");
                        if (num2 >= 0)
                        {
                            string[] array = text2.Substring(num2).Split(new char[]
                            {
                                '&'
                            });
                            string[] array2 = array;
                            for (int i = 0; i < array2.Length; i++)
                            {
                                string text3 = array2[i];
                                if (text3.Contains(value))
                                {
                                    text2 = text2.Replace(text3, "").Replace("&&", "&");
                                    if (text2.EndsWith("&"))
                                    {
                                        text2 = text2.Substring(0, text2.Length - 1);
                                    }
                                }
                            }
                        }
                        if (text2.EndsWith("?"))
                        {
                            text2 = text2.Substring(0, text2.Length - 1);
                        }
                        filterContext.Result = (new RedirectResult(text2));
                    }
                }
                if (!Authentication.IsAuthenticated)
                {
                    this.HandleUnAuthenticatedRequest(filterContext);
                }
                else
                {
                    if (!accessingResource.Trim().Equals("/Shared/HeadMasterPartial") && !accessingResource.Trim().Equals("/Shared/MenuLeftMasterPartial") && !accessingResource.Trim().Equals("/Shared/MenuTopMasterPartial") && !accessingResource.Trim().Equals("/UploadFile/UploadControl") && !accessingResource.Trim().Equals("/UploadFile/Upload") && !accessingResource.Trim().Equals("/UploadFile/UploadMultiFile"))
                    {
                        if (Authentication.User.Permissions == null || Authentication.User.Permissions.FirstOrDefault((string c) => c.Trim().ToUpper().Equals(accessingResource.Trim().ToUpper())) == null)
                        {
                            this.HandleUnauthorizedRequest(filterContext);
                        }
                    }
                }
            }
        }
        private void HandleUnAuthenticatedRequest(AuthorizationContext filterContext)
        {
            if (AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
            {
                JsonDataResult jsonDataResult = new JsonDataResult();
                jsonDataResult.StatusCode = 403;
                jsonDataResult.ErrorMessages.Add(new Error
                {
                    Message = "Bạn chưa đăng nhập hoặc phiên làm việc của bạn đã kết thúc"
                });
                jsonDataResult.Message = "Bạn chưa đăng nhập hoặc phiên làm việc của bạn đã kết thúc";
                JsonResult jsonResult = new JsonResult();
                jsonResult.Data = (jsonDataResult);
                filterContext.Result = (jsonResult);
            }
            else
            {
                ArrayList arrayList = new ArrayList();
                arrayList.Add(filterContext.HttpContext.Session.SessionID);
                arrayList.Add(filterContext.HttpContext.Request.Url.AbsoluteUri);
                arrayList.Add("Login");
                string str = HttpUtility.UrlEncode(this.GetEncryptor(filterContext).Encrypt(this.GetEncryptor(filterContext).Serialize(arrayList)));
                string text = FormsAuthentication.LoginUrl + "?token=" + str;
                filterContext.Result = (new RedirectResult(text));
            }
        }
        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
            {
                JsonDataResult jsonDataResult = new JsonDataResult();
                jsonDataResult.StatusCode = 401;
                jsonDataResult.ErrorMessages.Add(new Error
                {
                    Message = "Tài Khoản của Bạn không có quyền thực hiện hành động này."
                });
                jsonDataResult.Message = "Tài Khoản của Bạn không có quyền thực hiện hành động này.";
                JsonResult jsonResult = new JsonResult();
                jsonResult.Data = (jsonDataResult);
                filterContext.Result = (jsonResult);
            }
            else
            {
                ViewResult viewResult = new ViewResult();
                viewResult.ViewName = ("_Unauthorized");
                filterContext.Result = (viewResult);
            }
        }
        internal static string[] SplitString(string original)
        {
            string[] result;
            if (string.IsNullOrEmpty(original))
            {
                result = new string[0];
            }
            else
            {
                IEnumerable<string> source =
                    from piece in original.Split(new char[]
                    {
                        ','
                    })
                    let trimmed = piece.Trim()
                    where !string.IsNullOrEmpty(trimmed)
                    select trimmed;
                result = source.ToArray<string>();
            }
            return result;
        }
    }
}
