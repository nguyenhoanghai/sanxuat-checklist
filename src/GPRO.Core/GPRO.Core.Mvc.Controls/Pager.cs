using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
namespace GPRO.Core.Mvc.Controls
{
	public class Pager
	{
		private string _controllerName = string.Empty;
		private string _actionName = string.Empty;
		private object _routeValues;
		private int _pageSize = 10;
		private int _pageIndex = 1;
		private int _totalRecords = 0;
		private int _pageRange = 10;
		private AjaxOptions _ajaxOption;
		private string _totalText = "Total";
		private string _totalRecordsText = "records";
		private string _firstText = "First";
		private string _previousText = "Previous";
		private string _nextText = "Next";
		private string _lastText = "Last";
		private string _pageText = "Pages";
		private bool _showNote = true;
		private string _nextRangeText = "...";
		private string _previousRangeText = "...";
		public string ActionName
		{
			get
			{
				return this._actionName;
			}
			set
			{
				this._actionName = value;
			}
		}
		public int PageSize
		{
			get
			{
				return this._pageSize;
			}
			set
			{
				this._pageSize = value;
			}
		}
		public int PageIndex
		{
			get
			{
				return this._pageIndex;
			}
			set
			{
				this._pageIndex = value;
			}
		}
		public int TotalRecords
		{
			get
			{
				return this._totalRecords;
			}
			set
			{
				this._totalRecords = value;
			}
		}
		public int PageRange
		{
			get
			{
				return this._pageRange;
			}
			set
			{
				this._pageRange = value;
			}
		}
		public AjaxOptions AjaxOption
		{
			get
			{
				return this._ajaxOption;
			}
			set
			{
				this._ajaxOption = value;
			}
		}
		public string ControllerName
		{
			get
			{
				return this._controllerName;
			}
			set
			{
				this._controllerName = value;
			}
		}
		public object RouteValues
		{
			get
			{
				return this._routeValues;
			}
			set
			{
				this._routeValues = value;
			}
		}
		public string TotalRecordsText
		{
			get
			{
				return this._totalRecordsText;
			}
			set
			{
				this._totalRecordsText = value;
			}
		}
		public string TotalText
		{
			get
			{
				return this._totalText;
			}
			set
			{
				this._totalText = value;
			}
		}
		public string FirstText
		{
			get
			{
				return this._firstText;
			}
			set
			{
				this._firstText = value;
			}
		}
		public string PreviousText
		{
			get
			{
				return this._previousText;
			}
			set
			{
				this._previousText = value;
			}
		}
		public string NextText
		{
			get
			{
				return this._nextText;
			}
			set
			{
				this._nextText = value;
			}
		}
		public string LastText
		{
			get
			{
				return this._lastText;
			}
			set
			{
				this._lastText = value;
			}
		}
		public string PageText
		{
			get
			{
				return this._pageText;
			}
			set
			{
				this._pageText = value;
			}
		}
		public bool ShowNote
		{
			get
			{
				return this._showNote;
			}
			set
			{
				this._showNote = value;
			}
		}
		public string NextRangeText
		{
			get
			{
				return this._nextRangeText;
			}
			set
			{
				this._nextRangeText = value;
			}
		}
		public string PreviousRangeText
		{
			get
			{
				return this._previousRangeText;
			}
			set
			{
				this._previousRangeText = value;
			}
		}
		public Pager(string actionName, string controllerName, int pageIndex = 1, int pageSize = 10, int totalRecords = 0, AjaxOptions ajaxOption = null, object routeValues = null)
		{
			this.ActionName = actionName;
			this.ControllerName = controllerName;
			this.PageIndex = this.PageIndex;
			this.PageSize = pageSize;
			this.TotalRecords = totalRecords;
			this.AjaxOption = ajaxOption;
			this.RouteValues = routeValues;
		}
		public Pager()
		{
		}
		public MvcHtmlString InitPagerHtml(HtmlHelper htmlHelper)
		{
			MvcHtmlString result;
			if (this.TotalRecords == 0)
			{
				result = MvcHtmlString.Empty;
			}
			else
			{
				int num = this.TotalRecords / this.PageSize;
				if (this.TotalRecords % this.PageSize > 0)
				{
					num++;
				}
				string text = "<a href='#'>1</a>";
				string value = "<span>{totalText}:</span> <span>{totalRecords}</span> <span>{totalRecordsText}</span>  <span>{first}</span> <span> {previous}</span> <span> {numberRange}</span> <span>{next}</span> <span>{last}</span>  <span>{pageCount} pages</span>";
				if (string.IsNullOrEmpty(this.ControllerName) || string.IsNullOrEmpty(this.ActionName))
				{
					throw new Exception("controllerName and actionName must be specified for PageLinks.");
				}
				RouteValueDictionary routeValueDictionary = new RouteValueDictionary(this.RouteValues);
				routeValueDictionary.Add("controller", this.ControllerName);
				routeValueDictionary.Add("action", this.ActionName);
				routeValueDictionary.Add("page", 1);
				string newValue = LinkExtensions.RouteLink(htmlHelper, this.FirstText, routeValueDictionary).ToString();
				routeValueDictionary["page"] = num;
				string newValue2 = LinkExtensions.RouteLink(htmlHelper, this.LastText, routeValueDictionary).ToString();
				routeValueDictionary["page"] = this.PageIndex + 1;
				string newValue3 = LinkExtensions.RouteLink(htmlHelper, this.NextText, routeValueDictionary).ToString();
				routeValueDictionary["page"] = this.PageIndex - 1;
				string newValue4 = LinkExtensions.RouteLink(htmlHelper, this.PreviousText, routeValueDictionary).ToString();
				if (this.PageIndex == 1)
				{
					newValue = this.FirstText;
					newValue4 = this.PreviousText;
				}
				if (this.PageIndex == num)
				{
					newValue3 = this.NextText;
					newValue2 = this.LastText;
				}
				if (num <= this.PageRange)
				{
					text = "";
					for (int i = 1; i <= num; i++)
					{
						if (this.PageIndex == i)
						{
							text += i.ToString();
						}
						else
						{
							routeValueDictionary["page"] = i;
							text += LinkExtensions.RouteLink(htmlHelper, i.ToString(), routeValueDictionary).ToString();
						}
						text += "  ";
					}
				}
				else
				{
					for (int i = 0; i <= num / this.PageRange; i++)
					{
						int num2 = this.PageRange * i;
						int num3 = this.PageRange * (i + 1);
						if (this.PageIndex > num2 && this.PageIndex <= num3)
						{
							text = "";
							for (int j = num2 + 1; j <= num3; j++)
							{
								if (this.PageIndex == j)
								{
									text += j.ToString();
								}
								else
								{
									routeValueDictionary["page"] = j;
									text += LinkExtensions.RouteLink(htmlHelper, j.ToString(), routeValueDictionary).ToString();
								}
								text += "  ";
							}
						}
					}
				}
				StringBuilder stringBuilder = new StringBuilder(value);
				stringBuilder.Replace("{totalText}", this.TotalText).Replace("{totalRecords}", this.TotalRecords.ToString()).Replace("{totalRecordsText}", this.TotalRecordsText).Replace("{first}", newValue).Replace("{totalRecordsText}", this.TotalRecordsText);
				stringBuilder.Replace("{previous}", newValue4).Replace("{numberRange}", text).Replace("{next}", newValue3).Replace("{last}", newValue2).Replace("{pageCount}", num.ToString());
				result = MvcHtmlString.Create(stringBuilder.ToString());
			}
			return result;
		}
		public MvcHtmlString InitPagerAjax(AjaxHelper ajaxHelper)
		{
			int num = this.TotalRecords / this.PageSize;
			if (this.TotalRecords % this.PageSize > 0)
			{
				num++;
			}
			RouteValueDictionary routeValueDictionary = new RouteValueDictionary(this.RouteValues);
			routeValueDictionary.Add("controller", this.ControllerName);
			routeValueDictionary.Add("action", this.ActionName);
			string text = this.FirstText;
			string text2 = this.PreviousText;
			string text3 = this.NextText;
			string text4 = this.LastText;
			routeValueDictionary.Add("page", 1);
			text = ((this.PageIndex == 1) ? (this.FirstText + " ") : (AjaxExtensions.RouteLink(ajaxHelper, this.FirstText, routeValueDictionary, this.AjaxOption).ToHtmlString() + " "));
			routeValueDictionary["page"] = num;
			text4 = ((this.PageIndex == num) ? (this.LastText + " ") : (AjaxExtensions.RouteLink(ajaxHelper, this.LastText, routeValueDictionary, this.AjaxOption).ToHtmlString() + " "));
			routeValueDictionary["page"] = this.PageIndex + 1;
			text3 = ((this.PageIndex == num) ? (this.NextText + " ") : (AjaxExtensions.RouteLink(ajaxHelper, this.NextText, routeValueDictionary, this.AjaxOption).ToHtmlString() + " "));
			routeValueDictionary["page"] = this.PageIndex - 1;
			text2 = ((this.PageIndex == 1) ? (this.PreviousText + " ") : (AjaxExtensions.RouteLink(ajaxHelper, this.PreviousText, routeValueDictionary, this.AjaxOption).ToHtmlString() + " "));
			string text5 = "";
			if (num <= this.PageRange)
			{
				text5 = "";
				for (int i = 1; i <= num; i++)
				{
					if (this.PageIndex == i)
					{
						text5 += i.ToString();
					}
					else
					{
						routeValueDictionary["page"] = i;
						text5 += AjaxExtensions.RouteLink(ajaxHelper, i.ToString(), routeValueDictionary, this.AjaxOption).ToHtmlString();
					}
					text5 += "  ";
				}
			}
			else
			{
				for (int i = 0; i <= num / this.PageRange; i++)
				{
					int num2 = this.PageRange * i;
					int num3 = (this.PageRange * (i + 1) > num) ? num : (this.PageRange * (i + 1));
					if (this.PageIndex > num2 && this.PageIndex <= num3)
					{
						text5 = "";
						if (num2 > 0)
						{
							routeValueDictionary["page"] = num2;
							text5 += AjaxExtensions.RouteLink(ajaxHelper, this.PreviousRangeText, routeValueDictionary, this.AjaxOption).ToHtmlString();
							text5 += "  ";
						}
						for (int j = num2 + 1; j <= num3; j++)
						{
							if (this.PageIndex == j)
							{
								text5 += j.ToString();
							}
							else
							{
								routeValueDictionary["page"] = j;
								text5 += AjaxExtensions.RouteLink(ajaxHelper, j.ToString(), routeValueDictionary, this.AjaxOption).ToHtmlString();
							}
							text5 += "  ";
						}
						if (num3 < num)
						{
							routeValueDictionary["page"] = num3 + 1;
							text5 += AjaxExtensions.RouteLink(ajaxHelper, this.NextRangeText, routeValueDictionary, this.AjaxOption).ToHtmlString();
							text5 += "  ";
						}
					}
				}
			}
			string text6 = this.ShowNote ? string.Format("{0}:{1} {2} - {3}/{4} {5}: ", new object[]
			{
				this.TotalText,
				this.TotalRecords,
				this.TotalRecordsText,
				this.PageIndex,
				num,
				this.PageText
			}) : "";
			return MvcHtmlString.Create(string.Concat(new string[]
			{
				text6,
				text,
				text2,
				text5,
				text3,
				text4
			}));
		}
	}
}
