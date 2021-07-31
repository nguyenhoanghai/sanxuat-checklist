using GPRO.Core.Mvc.Controls;
using System;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
namespace GPRO.Core.Mvc.Extension
{
	public static class PagerHelper
	{
		public static MvcHtmlString PagerLinks(this HtmlHelper htmlHelper, string controllerName, string actionName, int pageSize, int pageIndex, int totalRecords)
		{
			return htmlHelper.PagerLinks(controllerName, actionName, pageSize, pageIndex, new
			{

			}, totalRecords, "Total", "records", "First", "Previous", "Next", "Last");
		}
		public static MvcHtmlString PagerLinks(this HtmlHelper htmlHelper, string controllerName, string actionName, int pageSize, int pageIndex, int totalRecords, string totalText, string totalRecordsText, string firstText, string previousText, string nextText, string lastText)
		{
			return htmlHelper.PagerLinks(controllerName, actionName, pageSize, pageIndex, new
			{

			}, totalRecords, totalRecordsText, firstText, previousText, nextText, lastText, "Last");
		}
		public static MvcHtmlString PagerLinks(this HtmlHelper htmlHelper, string controllerName, string actionName, int pageSize, int pageIndex, object routeValues, int totalRecords, string totalText = "Total", string totalRecordsText = "records", string firstText = "First", string previousText = "Previous", string nextText = "Next", string lastText = "Last")
		{
			return new Pager
			{
				ActionName = actionName,
				ControllerName = controllerName,
				FirstText = firstText,
				LastText = lastText,
				NextText = nextText,
				PageIndex = pageIndex,
				PageSize = pageSize,
				PreviousText = previousText,
				RouteValues = routeValues,
				TotalRecords = totalRecords,
				TotalRecordsText = totalRecordsText,
				TotalText = totalText
			}.InitPagerHtml(htmlHelper);
		}
		public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, int pageSize, int pageIndex, int totalRecords, AjaxOptions ajaxOption, int pageRange = 10, string totalText = "Total", string totalRecordsText = "records", string firstText = "First", string previousText = "Previous", string nextText = "Next", string lastText = "Last")
		{
			Pager pager = new Pager
			{
				PageRange = pageRange,
				TotalText = totalText,
				TotalRecordsText = totalRecordsText,
				FirstText = firstText,
				PreviousText = previousText,
				NextText = nextText,
				LastText = lastText
			};
			pager.ActionName = actionName;
			pager.ControllerName = controllerName;
			pager.PageIndex = pageIndex;
			pager.PageSize = pageSize;
			pager.RouteValues = routeValues;
			pager.TotalRecords = totalRecords;
			pager.AjaxOption = ajaxOption;
			return pager.InitPagerAjax(ajaxHelper);
		}
	}
}
