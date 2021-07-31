using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
namespace GPRO.Core
{
	public class ResxManager
	{
		public static readonly ResxType Vi = new ResxType
		{
			Name = "VN",
			UICulture = "vi-VN",
			IsDefault = true
		};
		public static readonly ResxType En = new ResxType
		{
			Name = "US",
			UICulture = "en-US",
			IsDefault = false
		};
		private static ResourceManager resourceMan;
		private static Dictionary<string, CultureInfo> _dicCulture;
		public static ResourceManager ResourceManager
		{
			get
			{
				ResxType resx = ResxManager.GetResx(ResxManager.CurrentLanguage);
				if (object.ReferenceEquals(ResxManager.resourceMan, null) || ResxManager.resourceMan.BaseName != string.Format("Resources.{0}", resx.Name))
				{
					ResourceManager resourceManager = new ResourceManager(string.Format("Resources.{0}", resx.Name), Assembly.Load("App_GlobalResources"));
					ResxManager.resourceMan = resourceManager;
				}
				return ResxManager.resourceMan;
			}
		}
		public static string CurrentLanguage
		{
			get
			{
				string result;
				if (HttpContext.Current.Session != null && HttpContext.Current.Session["SS_LANGUAGE"] != null)
				{
					result = HttpContext.Current.Session["SS_LANGUAGE"].ToString();
				}
				else
				{
					result = "vi-VN";
				}
				return result;
			}
		}
		private static Dictionary<string, CultureInfo> DicCulture
		{
			get
			{
				if (ResxManager._dicCulture == null)
				{
					ResxManager._dicCulture = new Dictionary<string, CultureInfo>();
				}
				return ResxManager._dicCulture;
			}
		}
		public static CultureInfo CurrentCultureInfo
		{
			get
			{
				return ResxManager.GetCulture(ResxManager.CurrentLanguage);
			}
		}
		private static CultureInfo GetCulture(string pLangName)
		{
			if (!ResxManager.DicCulture.ContainsKey(pLangName))
			{
				ResxManager.DicCulture.Add(pLangName, new CultureInfo(pLangName));
			}
			return ResxManager.DicCulture[pLangName];
		}
		public static void SetLanguage(eLanguage language)
		{
			switch (language)
			{
			case eLanguage.Vi:
				HttpContext.Current.Session["SS_LANGUAGE"] = ResxManager.Vi.UICulture;
				break;

			case eLanguage.En:
				HttpContext.Current.Session["SS_LANGUAGE"] = ResxManager.En.UICulture;
				break;

			default:
				HttpContext.Current.Session["SS_LANGUAGE"] = ResxManager.Vi.UICulture;
				break;
			}
		}
		public static void SetLanguage(string uICulture)
		{
			HttpContext.Current.Session["SS_LANGUAGE"] = uICulture;
		}
		public static string GetString(string pResxName, bool pLowerFirstKey = false)
		{
			string text = "";
			try
			{
				text = ResxManager.ResourceManager.GetString(pResxName, ResxManager.CurrentCultureInfo);
				if (pLowerFirstKey)
				{
					text = text.Substring(0, 1).ToLower() + text.Substring(1);
				}
			}
			catch
			{
			}
			return text;
		}
		public static IEnumerable<ResxType> GetResx()
		{
			return new ResxType[]
			{
				ResxManager.Vi,
				ResxManager.En
			};
		}
		public static ResxType GetResx(string uiCulture)
		{
			ResxType resxType = (
				from p in ResxManager.GetResx()
				where p.UICulture == uiCulture
				select p).FirstOrDefault<ResxType>();
			if (resxType == null)
			{
				resxType = (
					from p in ResxManager.GetResx()
					where p.IsDefault
					select p).FirstOrDefault<ResxType>();
				if (resxType == null)
				{
					resxType = ResxManager.Vi;
				}
			}
			return resxType;
		}
	}
}
