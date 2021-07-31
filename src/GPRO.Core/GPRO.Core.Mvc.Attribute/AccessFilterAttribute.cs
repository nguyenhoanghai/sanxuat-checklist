using GPRO.Core.Generic;
using GPRO.Core.Security;
using System;
using System.Web.Mvc;
namespace GPRO.Core.Mvc.Attribute
{
    public class AccessFilterAttribute : ActionFilterAttribute
    {
        public AllowAccess AllowAccess
        {
            get;
            set;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!Authentication.IsAuthenticated)
            {
                filterContext.Result = (new RedirectResult("/"));
            }
            else
            {
                switch (this.AllowAccess)
                {
                    case AllowAccess.Administrator:
                        if (Authentication.User.CompanyId != 0)
                            filterContext.Result = (new RedirectResult("/"));
                        break;

                    case AllowAccess.Company:
                        if (Authentication.User.CompanyId == 0)
                            filterContext.Result = (new RedirectResult("/"));
                        break;
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
