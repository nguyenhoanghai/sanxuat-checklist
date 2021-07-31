using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
namespace GPRO.Core.Mvc.Attribute
{
	public class ValidateBirthDay : ValidationAttribute
	{
		private bool _isAllowNull = true;
		public ValidateBirthDay()
		{
		}
		public ValidateBirthDay(bool isAllowNull = true)
		{
			this._isAllowNull = isAllowNull;
		}
		public override bool IsValid(object value)
		{
			CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
			cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
			cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
			string s = value.ToString();
			DateTime dateTime = DateTime.Parse(s);
			return DateTime.Now.Year - dateTime.Year >= 18 && DateTime.Now.Year - dateTime.Year <= 45;
		}
	}
}
