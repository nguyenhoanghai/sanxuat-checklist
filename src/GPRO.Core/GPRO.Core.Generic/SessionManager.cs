using System;
using System.Web;
namespace GPRO.Core.Generic
{
	public class SessionManager
	{
		public static int LoginCount
		{
			get
			{
				int result;
				if (HttpContext.Current.Session == null)
				{
					result = 0;
				}
				else
				{
					result = ((HttpContext.Current.Session["SS_LOGIN_COUNT"] == null) ? 0 : int.Parse(HttpContext.Current.Session["SS_LOGIN_COUNT"].ToString()));
				}
				return result;
			}
			set
			{
				if (value <= 0)
				{
					HttpContext.Current.Session["SS_LOGIN_COUNT"] = null;
				}
				else
				{
					HttpContext.Current.Session["SS_LOGIN_COUNT"] = value;
				}
			}
		}
		public static bool RequireChangePassword
		{
			get
			{
				return HttpContext.Current.Session != null && HttpContext.Current.Session["SS_REQUIRE_CHANGE_PASSWORD"] != null && (bool)HttpContext.Current.Session["SS_REQUIRE_CHANGE_PASSWORD"];
			}
			set
			{
				if (!value)
				{
					HttpContext.Current.Session["SS_REQUIRE_CHANGE_PASSWORD"] = null;
				}
				else
				{
					HttpContext.Current.Session["SS_REQUIRE_CHANGE_PASSWORD"] = value;
				}
			}
		}
	}
}
