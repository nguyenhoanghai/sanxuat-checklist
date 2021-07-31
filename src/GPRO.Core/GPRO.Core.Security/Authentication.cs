using GPRO.Core.Generic;
using GPRO.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GPRO.Core.Security
{
    public class Authentication
    {
        public static DateTime PassCheckDate { get; set; }
        public static int today { get; set; }
        public static bool Check { get; set; }
        public static string DefaultPage { get; set; }
        public static bool isAuthenticate { get; set; }
        public static bool IsAuthenticated { get { return Authentication.User != null; } }
        public static int UserId { get { return Authentication.User.UserID; } }
        public static int? CompanyID { get { return Authentication.User.CompanyId; } }
        public static string EmployeeName
        {
            get
            {
                string result;
                if (string.IsNullOrEmpty(Authentication.User.EmployeeName))
                {
                    result = "Chưa cập nhật";
                }
                else
                {
                    result = string.Format("{0}", Authentication.User.EmployeeName);
                }
                return result;
            }
        }
        public static bool IsOwner { get { return (Authentication.User != null ? Authentication.User.IsOwner : false); } }

        private static IUserService _user = null;
        public static IUserService User
        {
            get
            {
                //if (_user == null)
                //{
                //    IUserService userService = (
                //        from p in Authentication.UserOnline
                //        where p.SesssionId == HttpContext.Current.Session.SessionID
                //        select p).FirstOrDefault<IUserService>();
                //    if (userService != null)
                //    {
                //        IUserService userService2 = userService;
                //        _user = userService2;
                //    }
                //} 

                //sua loi ko logout ra ngoai dc do cái _user always has value
                IUserService userService = (
                       from p in Authentication.UserOnline
                       where p.SesssionId == HttpContext.Current.Session.SessionID
                       select p).FirstOrDefault<IUserService>();
                if (userService != null)
                {
                    IUserService userService2 = userService;
                    _user = userService2;
                }
                else
                    _user = null;
                return _user;
            }
        }
        public static List<IUserService> UserOnline
        {
            get
            {
                List<IUserService> result;
                if (HttpContext.Current == null)
                {
                    result = new List<IUserService>();
                }
                else
                {
                    List<IUserService> list = (List<IUserService>)HttpContext.Current.Application["APP_AUTHENTICATION"];
                    if (list == null)
                    {
                        list = new List<IUserService>();
                        HttpContext.Current.Application["APP_AUTHENTICATION"] = list;
                    }
                    result = list;
                }
                return result;
            }
            private set
            {
                HttpContext.Current.Application["APP_AUTHENTICATION"] = value;
            }
        }
        public static bool CheckUserOnline(string userName)
        {
            IUserService userService = (
                from p in Authentication.UserOnline
                where p.UserName == userName
                select p).FirstOrDefault<IUserService>();
            return userService != null;
        }
        public static void Logout()
        {
            if (Authentication.IsAuthenticated)
            {
                if (HttpContext.Current.Application["APP_MENU_DISPLAY" + Authentication.User.UserID] != null)
                    HttpContext.Current.Application["APP_MENU_DISPLAY" + Authentication.User.UserID] = null;

                List<IUserService> list = (
                    from p in Authentication.UserOnline
                    where p.UserID == Authentication.User.UserID
                    select p).ToList<IUserService>();
                foreach (IUserService current in list)
                {
                    Authentication.UserOnline.Remove(current);
                }
                HttpContext.Current.Session["SS_AUTHENTICATION"] = null;
                HttpContext.Current.Session.Abandon();
            }
        }
        public static void LogOutOther(string userName)
        {
            List<IUserService> list = (
                from p in Authentication.UserOnline
                where p.UserName == userName
                select p).ToList<IUserService>();
            foreach (IUserService current in list)
            {
                Authentication.UserOnline.Remove(current);
            }
        }
        public static void ChangeStoreWoking(int StoreID)
        {
            if (Authentication.IsAuthenticated)
            {
                Authentication.User.StoreID = StoreID;
            }
        }
        public static void Login(int userId)
        {
            if (Authentication.IsAuthenticated)
                Authentication.Logout();

            IMembershipService membershipService = HttpContext.Current.Application["IMEMBERSHIP_SERVICE"] as IMembershipService;
            IUserService objUser = membershipService.GetUserService(userId);
            if (objUser != null)
            {
                List<IUserService> list = (
                    from p in Authentication.UserOnline
                    where p.UserID == objUser.UserID
                    select p).ToList<IUserService>();
                foreach (IUserService current in list)
                {
                    Authentication.UserOnline.Remove(current);
                }
                objUser.SesssionId = HttpContext.Current.Session.SessionID;
                Authentication.UserOnline.Add(objUser);
                HttpContext.Current.Session["SS_AUTHENTICATION"] = objUser;
                // HttpContext.Current.Session.Timeout = 2;
                //HttpContext.Current.Session.Timeout = 600;
            }
        }
        public static IEnumerable<IPermissionService> GetPermissionByFeatureName(string featureName)
        {
            IEnumerable<IPermissionService> result;
            if (!Authentication.IsAuthenticated)
            {
                result = null;
            }
            else
            {
                IUserService user = Authentication.User;
                object state = user.State;
                lock (state)
                {
                    if (user.PermissionServices == null)
                    {
                        user.PermissionServices = new List<IPermissionService>();
                    }
                    int num = (
                        from p in user.PermissionServices
                        where p.FeatureName.ToLower() == featureName.ToLower()
                        select p.FeatureId).FirstOrDefault<int>();
                    if (num <= 0)
                    {
                        IMembershipService membershipService = HttpContext.Current.Application["IMEMBERSHIP_SERVICE"] as IMembershipService;
                        IPermissionService[] permissionService = membershipService.GetPermissionService(featureName);
                        if (permissionService != null)
                        {
                            user.PermissionServices.AddRange(permissionService);
                        }
                        result = permissionService;
                    }
                    else
                    {
                        IEnumerable<IPermissionService> enumerable =
                            from p in user.PermissionServices
                            where p.FeatureName.ToLower() == featureName.ToLower()
                            select p;
                        result = enumerable;
                    }
                }
            }
            return result;
        }
        public static bool IsPermission(string permission)
        {
            return Authentication.User.Permissions.Contains(permission);
        }
        public static bool IsPermission(PermissionType permissionType, string featureName)
        {
            IPermissionService permissionService = (
                from p in Authentication.GetPermissionByFeatureName(featureName)
                where p.PermissionTypeId == (int)permissionType
                select p).FirstOrDefault<IPermissionService>();
            bool result;
            if (permissionService == null)
            {
                result = false;
            }
            else
            {
                bool flag = Authentication.IsPermission(permissionService.PermissionName);
                result = flag;
            }
            return result;
        }
        public static bool IsPermission(PermissionType permissionType, Type type, string area = "")
        {
            string text = type.Name.Replace("Controller", "");
            if (!string.IsNullOrEmpty(area))
            {
                text = area + text;
            }
            return Authentication.IsPermission(permissionType, text);
        }
    }
}
