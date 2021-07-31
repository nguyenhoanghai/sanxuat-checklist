using GPRO.Core.Mvc;
using GPRO.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SanXuatCheckList.Controllers
{
    public class BaseController : ControllerCore
    {
        public string defaultPage = string.Empty;
        public bool isAuthenticate = false;
        public bool isOwner = false;
        public BaseController()
        {
        }

        protected override void Initialize(RequestContext requestContext)
        {
            var routeDefault = ((System.Web.Routing.Route)requestContext.RouteData.Route).Defaults;
            if (routeDefault != null)
            {
                var valuesDefault = routeDefault.Values.ToList();
                defaultPage = "/" + valuesDefault[0].ToString() + "/" + valuesDefault[1].ToString();
            }
            CheckLogin(requestContext, App_Global.AppGlobal.ProductCode);
            isAuthenticate = Authentication.isAuthenticate;
            isOwner = Authentication.IsOwner;
        }
    }
}