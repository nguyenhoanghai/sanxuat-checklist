using GPRO.Core.Generic;
using GPRO.Core.Interface;
using GPRO.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using SystemGPRO.Serial;
namespace GPRO.Core.Mvc
{
    public class ControllerCore : Controller
    {
        public List<Error> ErrorMessages { get; set; }
        public JsonDataResult JsonDataResult { get; set; }
        public string Layout { get; set; }
        public IUserService UserContext { get { return Authentication.User; } }
        public ControllerCore()
        {
            this.JsonDataResult = new JsonDataResult();
            this.ErrorMessages = new List<Error>();
            this.JsonDataResult.Result = "OK";
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
        public void CheckLogin(RequestContext requestContext, string productCode)
        {
            try
            {
                string requiredString = requestContext.RouteData.GetRequiredString("controller");
                string requiredString2 = requestContext.RouteData.GetRequiredString("action");
                string accessingResource = "/" + requiredString + "/" + requiredString2;

                if (!accessingResource.Contains("/Shared/UnActived"))
                {
                    if (Authentication.today == null || Authentication.today != DateTime.Now.Day)
                    {
                        SerialKey serialKey = new SerialKey();
                        ModelCheckKey modelCheckKey = serialKey.CheckActive(productCode, System.Web.Hosting.HostingEnvironment.MapPath("~/bin"));
                        Authentication.today = DateTime.Now.Day;
                        if (!modelCheckKey.checkResult && !accessingResource.Equals("/OutOfDate"))
                        {
                            requestContext.HttpContext.Response.Redirect("/OutOfDate");
                            Authentication.Check = false;
                        }
                        else
                            Authentication.Check = true;
                    }
                    else if (Authentication.today == DateTime.Now.Day && !Authentication.Check)
                        requestContext.HttpContext.Response.Redirect("/OutOfDate");
                }

                if (accessingResource.Equals("/Shared/UnActived"))
                {
                    base.Initialize(requestContext);
                }
                else
                {
                    RouteValueDictionary defaults = ((Route)requestContext.RouteData.Route).Defaults;
                    if (defaults != null)
                    {
                        List<object> list = defaults.Values.ToList<object>();
                        Authentication.DefaultPage = "/" + list[0].ToString() + "/" + list[1].ToString();
                    }
                    if (this.UserContext == null)
                    {
                        if (!requiredString.Equals("Authenticate"))
                        {
                            if (AjaxRequestExtensions.IsAjaxRequest(requestContext.HttpContext.Request))
                            {
                                requestContext.HttpContext.Response.StatusCode = 401;
                                requestContext.HttpContext.Response.End();
                            }
                            else
                            {
                                requestContext.HttpContext.Response.Redirect("/Authenticate/Login?Url=" + accessingResource);
                            }
                        }
                        else
                        {
                            base.Initialize(requestContext);
                        }
                    }
                    else
                    {
                        if ((requiredString.Equals("Authenticate") && requiredString2.Equals("Login")) || requiredString.Equals("UploadFile") || (requiredString.Equals("Authenticate") && requiredString2.Equals("Validate")))
                        {
                            base.Initialize(requestContext);
                        }
                        else
                        {
                            if (!requiredString.Equals("Error") && !requiredString2.Equals("Logout"))
                            {
                                bool arg_212_0;
                                if (this.UserContext.Permissions != null)
                                {
                                    arg_212_0 = ((
                                        from c in this.UserContext.Permissions
                                        where c.Trim().ToLower().Equals(accessingResource.Trim().ToLower())
                                        select c).FirstOrDefault<string>() != null);
                                }
                                else
                                {
                                    arg_212_0 = false;
                                }
                                if (!arg_212_0)
                                {
                                    if (AjaxRequestExtensions.IsAjaxRequest(requestContext.HttpContext.Request))
                                    {
                                        Authentication.isAuthenticate = false;
                                        this.JsonDataResult.Result = "ERROR";
                                        this.JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Lỗi quyền truy cập", Message = "Tài khoản của bạn không có quyền thực hiện hành động này!." });
                                        base.Initialize(requestContext);
                                    }
                                    else
                                    {
                                        requestContext.HttpContext.Response.Redirect("~/Error/Index?ErrorType=1");
                                    }
                                }
                                else
                                {
                                    if (AjaxRequestExtensions.IsAjaxRequest(requestContext.HttpContext.Request))
                                    {
                                        Authentication.isAuthenticate = true;
                                    }
                                    base.Initialize(requestContext);
                                }
                            }
                            else
                            {
                                base.Initialize(requestContext);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
