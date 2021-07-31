using GPRO.Core.Security;
using System;
using System.Web.Mvc;
namespace GPRO.Core.Mvc.Extension
{
	public static class Extension
	{
		public static string UserId(this HtmlHelper helper)
		{
			return Authentication.UserId.ToString();
		}
		public static string FullName(string firstName, string lastName)
		{
			string result;
			if (ResxManager.CurrentLanguage == ResxManager.Vi.UICulture)
			{
				result = string.Format("{0} {1}", lastName, firstName);
			}
			else
			{
				result = string.Format("{0} {1}", firstName, lastName);
			}
			return result;
		}
		public static string FullName(this HtmlHelper helper, string firstName, string lastName)
		{
			return Extension.FullName(firstName, lastName);
		}
		public static string FullName(this HtmlHelper helper)
		{
			throw new NotImplementedException();
		}
		public static string Email(this HtmlHelper helper)
		{
			return Authentication.User.Email;
		}
		public static string DateTimeViFormatFull(this HtmlHelper helper, DateTime? dateTime = null)
		{
			if (!dateTime.HasValue)
			{
				dateTime = new DateTime?(DateTime.Now);
			}
			return string.Format("{0:dd/MM/yyyy HH:mm:ss}", dateTime);
		}
		public static MvcHtmlString LogoImage(this HtmlHelper helper, string configKeyPath)
		{
			string format = "<img src='/resource/ImageHandler.ashx?w=61&h=61&imageUrl={0}&configKey={1}&df={2}'/>";
			string text = "";
			if (Authentication.IsAuthenticated)
			{
				string logoCompany = Authentication.User.LogoCompany;
				string arg = "/Files/Images/defaultImg.png";
				text = string.Format(format, logoCompany, configKeyPath, arg);
			}
			return MvcHtmlString.Create(text);
		}
		public static MvcHtmlString AvatarImage(this HtmlHelper helper, string configKeyPath, object attributes)
		{
			return MvcHtmlString.Empty;
		}
		public static MvcHtmlString AvatarImage(this HtmlHelper helper, string configKeyPath)
		{
			string format = "<img src='/resource/ImageHandler.ashx?w=45&h=45&imageUrl={0}&configKey={1}&df=Image_Upload/avatar.png'/>";
			string text = "";
			if (Authentication.IsAuthenticated)
			{
				string imagePath = Authentication.User.ImagePath;
				if (!string.IsNullOrEmpty(imagePath))
				{
					goto IL_38;
				}
				IL_38:
				text = string.Format(format, imagePath, configKeyPath);
			}
			return MvcHtmlString.Create(text);
		}
		public static string DayOfWeekText(this HtmlHelper helper, DateTime datetime)
		{
			string result;
			switch (datetime.DayOfWeek)
			{
			case DayOfWeek.Sunday:
				result = "CN";
				break;

			case DayOfWeek.Monday:
				result = "Hai";
				break;

			case DayOfWeek.Tuesday:
				result = "Ba";
				break;

			case DayOfWeek.Wednesday:
				result = "Tư";
				break;

			case DayOfWeek.Thursday:
				result = "Năm";
				break;

			case DayOfWeek.Friday:
				result = "Sáu";
				break;

			case DayOfWeek.Saturday:
				result = "Bảy";
				break;

			default:
				result = "";
				break;
			}
			return result;
		}
	}
}
