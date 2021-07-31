using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
namespace GPRO.Core.Mvc.Attribute
{
	public class ValidateDateTime : ValidationAttribute
	{
		private bool _isAllowNull = true;
		public ValidateDateTime()
		{
		}
		public ValidateDateTime(bool isAllowNull = true)
		{
			this._isAllowNull = isAllowNull;
		}
		public override bool IsValid(object value)
		{
			CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
			cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
			cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
			bool flag;
			bool result;
			if (value != null)
			{
				string s = value.ToString();
				DateTime t = DateTime.Parse(s);
				if (t < DateTime.Now)
				{
					flag = false;
					result = flag;
					return result;
				}
			}
			flag = true;
			result = flag;
			return result;
		}
	}
}
